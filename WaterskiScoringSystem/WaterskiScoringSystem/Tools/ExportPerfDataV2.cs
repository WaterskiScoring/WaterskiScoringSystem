using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tools {
	class ExportPerfDataV2 {
		private DataRow myTourRow;

		private String myTourFed = "";
		private String myTourRules = "";
		private String myTourClass = "";

		private TourProperties myTourProperties;
		private ListSkierClass mySkierClassList;
		private CalcScoreSummary myCalcScoreSummary;
		private ProgressWindow myProgressInfo;

		public ExportPerfDataV2() {
			myProgressInfo = new ProgressWindow();
			myTourProperties = TourProperties.Instance;
		}

		public Boolean exportTourPerfData( String inSanctionId ) {
			String curMethodName = "ExportPerfDataV2:exportTourPerfData: ";
			StreamWriter outBuffer = null;
			//Retrieve selected tournament attributes
			myTourRow = getTourData( inSanctionId );
			if ( myTourRow == null ) {
				MessageBox.Show( "An active tournament must be provided to select performance data to be exported, terminating report processing" );
				return false;
			}

			//Extract tournament attributes
			myTourRules = HelperFunctions.getDataRowColValue( myTourRow, "Rules", "AWSA" ).Trim();
			myTourClass = HelperFunctions.getDataRowColValue( myTourRow, "Class", "C" ).Trim();
			myTourFed = HelperFunctions.getDataRowColValue( myTourRow, "Federation", "USA" ).Trim();

			//Retrieve list of registered skiers for tournament
			DataRow[] curSkierScoreRows;
			myCalcScoreSummary = new CalcScoreSummary();
			DataTable curTourSkierList = myCalcScoreSummary.getMemberData( inSanctionId );
			if ( curTourSkierList == null || curTourSkierList.Rows.Count == 0 ) {
				MessageBox.Show( "No registered skiers found for tournament, terminating report processing" );
				return false;
			}

			//Retrieve output report file
			String curSkierName = "", curMemberId = "", curReadyToSki = "", curAgeGroup = "";
			String curFilename = inSanctionId.Trim() + myTourClass + ".wsp";
			outBuffer = HelperFunctions.getExportFile( "wsp files (*.wsp)|*.wsp|All files (*.*)|*.*", curFilename );
			if ( outBuffer == null ) {
				MessageBox.Show( "Failed to open report file, terminating report processing" );
				return false;
			}
			String curReportFilenameFull = ( (FileStream)( outBuffer.BaseStream ) ).Name;
			Log.WriteFile( "Export performance data file begin: " + curReportFilenameFull );

			Dictionary<string, dynamic> curSkierEntry = new Dictionary<string, dynamic>();

			try {
				outBuffer.WriteLine( "{\"PerfExport\":" );
				//writeOutputData( outBuffer, writeFileHeader( inSanctionId ) );
				String curOutputRecord = JsonConvert.SerializeObject( writeFileHeader( inSanctionId ) );
				writeOutputData( outBuffer, curOutputRecord.Substring(0, curOutputRecord.Length - 1) );
				writeOutputData( outBuffer, ", \"SkierEntries\" : [" );
				
				mySkierClassList = new ListSkierClass();
				mySkierClassList.ListSkierClassLoad();

				myProgressInfo.setProgessMsg( "Processing skier performance data file exports" );
				myProgressInfo.Show();
				myProgressInfo.Refresh();
				myProgressInfo.setProgressMax( curTourSkierList.Rows.Count );

				DataTable curPerfSummaryDataTable = calcPerfSummaryDataTable( inSanctionId, curTourSkierList );

				String curEntryPrefix = "";
				int curRowCount = 0, curWriteCount = 0;
				myProgressInfo.setProgressMax( curTourSkierList.Rows.Count );
				foreach ( DataRow curSkierRow in curTourSkierList.Rows ) {
					curRowCount++;

					curMemberId = (String)curSkierRow["MemberId"];
					curSkierName = (String)curSkierRow["SkierName"];
					curAgeGroup = (String)curSkierRow["AgeGroup"];
					curReadyToSki = (String)curSkierRow["ReadyToSki"];

					myProgressInfo.setProgressValue( curRowCount );
					myProgressInfo.setProgessMsg( String.Format( "Record {0} Skier: {1} {2}", curRowCount, curSkierName, curAgeGroup ) );
					myProgressInfo.Refresh();

					curSkierScoreRows = curPerfSummaryDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );

					if ( HelperFunctions.isValueTrue(curReadyToSki) && curSkierScoreRows.Length > 0 ) {
						try {
							curSkierEntry = new Dictionary<string, dynamic>(); 
							
							curSkierEntry.Add( "SkierHeader", writeSkierHeaderInfo( inSanctionId, curSkierRow ) );
							
							curSkierEntry.Add( "SkierPerfEntries", writeSkierPerfData( inSanctionId, curSkierRow, curSkierScoreRows ) );

							curOutputRecord = JsonConvert.SerializeObject( curSkierEntry );
							if ( curWriteCount > 0 ) curEntryPrefix = ", { ";
							else curEntryPrefix = "{ ";
							writeOutputData( outBuffer, curEntryPrefix + curOutputRecord.Substring( 1, curOutputRecord.Length - 1 ) );
							curWriteCount++;

						} catch ( Exception ex ) {
							String curMsg = curMethodName + "Exception encountered processing skier " + curSkierName + "\n\nError: " + ex.Message;
							Log.WriteFile( curMsg );
							MessageBox.Show( curMsg );
						}
					}
				}

				outBuffer.WriteLine( "] } }" );
				MessageBox.Show( "Processing complete for " + curRowCount + " skiers" );

			} catch ( Exception ex ) {
				String curMsg = curMethodName + "Exception encountered processing performance results for registered skiers" + "\n\nError: " + ex.Message;
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );

			} finally {
				myProgressInfo.Close();
				outBuffer.Flush();
				outBuffer.Close();
				outBuffer = null; 
			}


			Log.WriteFile( "Export performance data file complete: " + curReportFilenameFull );
			return true;
		}

		private DataTable calcPerfSummaryDataTable( String inSanctionId, DataTable curTourSkierList ) {
			DataTable curPerfSummaryDataTable = null;

			String curPlcmtMethod = "score", curPlcmtOverallOrg = "agegroup";
			String curDataType = myTourProperties.MasterSummaryDataType;
			//String curDataType = Properties.Settings.Default.MasterSummaryV2DataType;
			if ( curDataType.ToLower().Equals( "total" )
				|| curDataType.ToLower().Equals( "best" )
				|| curDataType.ToLower().Equals( "final" )
				|| curDataType.ToLower().Equals( "first" ) ) {
			} else {
				curDataType = "best";
			}

			String curPointsMethod = myTourProperties.MasterSummaryPointsMethod;
			//String curPointsMethod = Properties.Settings.Default.MasterSummaryV2PointsMethod;
			if ( curPointsMethod.ToLower().Equals( "nops" )
				|| curPointsMethod.ToLower().Equals( "plcmt" )
				|| curPointsMethod.ToLower().Equals( "kbase" )
				|| curPointsMethod.ToLower().Equals( "ratio" ) ) {
			} else {
				curPointsMethod = "nops";
			}

			String curPlcmtOrg = myTourProperties.MasterSummaryPlcmtOrg;
			//String curPlcmtOrg = Properties.Settings.Default.MasterSummaryV2PlcmtOrg;
			if ( curPlcmtOrg.ToLower().Equals( "div" ) ) {
				curPlcmtOverallOrg = "agegroup";
			} else if ( curPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curPlcmtOverallOrg = "agegroupgroup";
			} else {
				curPlcmtOverallOrg = "agegroup";
				curPlcmtOrg = "div";
			}

			String curProgessMsgPrefix = "Calculate event placements ";
			String curProgessMsg = curProgessMsgPrefix;
			myProgressInfo.setProgessMsg( curProgessMsg );
			myProgressInfo.Show();
			myProgressInfo.Refresh();
			myProgressInfo.setProgressMax( 10 );

			myProgressInfo.setProgressValue( 1 );
			myProgressInfo.Refresh();

			if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
				myProgressInfo.setProgressValue( 5 );
				curProgessMsg = curProgessMsgPrefix + "Iwwf event placements";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				curPerfSummaryDataTable = myCalcScoreSummary.CalcIwwfEventPlcmts( myTourRow, inSanctionId, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 6 );
				curProgessMsg = curProgessMsgPrefix + "slalom detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curSlalomDetail = myCalcScoreSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 7 );
				curProgessMsg = curProgessMsgPrefix + "trick detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curTrickDetail = myCalcScoreSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 8 );
				curProgessMsg = curProgessMsgPrefix + "jump detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curJumpDetail = myCalcScoreSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 9 );
				curProgessMsg = curProgessMsgPrefix + "build scorebook";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable newSummaryDataTable = myCalcScoreSummary.buildTourScorebook( inSanctionId, myTourRow, curTourSkierList, curPerfSummaryDataTable, curSlalomDetail, curTrickDetail, curJumpDetail );
				curPerfSummaryDataTable = newSummaryDataTable;

			} else {
				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "slalom summary";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curSlalomDataTable = myCalcScoreSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "trick summary";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curTrickDataTable = myCalcScoreSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "jump summary";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curJumpDataTable = myCalcScoreSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "build scorebook summary";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				curPerfSummaryDataTable = myCalcScoreSummary.buildOverallSummary( myTourRow, curSlalomDataTable, curTrickDataTable, curJumpDataTable, curDataType, curPlcmtOverallOrg );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "slalom detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curSlalomDetail = myCalcScoreSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "trick detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curTrickDetail = myCalcScoreSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 2 );
				curProgessMsg = curProgessMsgPrefix + "jump detail";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable curJumpDetail = myCalcScoreSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

				myProgressInfo.setProgressValue( 8 );
				curProgessMsg = curProgessMsgPrefix + "build scorebook summary final";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();
				DataTable newSummaryDataTable = myCalcScoreSummary.buildTourScorebook( inSanctionId, myTourRow, curTourSkierList, curPerfSummaryDataTable, curSlalomDetail, curTrickDetail, curJumpDetail );
				curPerfSummaryDataTable = newSummaryDataTable;
				myProgressInfo.setProgressValue( 9 );
				curProgessMsg = curProgessMsgPrefix + "build scorebook summary final complete";
				myProgressInfo.setProgessMsg( curProgessMsg );
				myProgressInfo.Refresh();

			}

			return curPerfSummaryDataTable;
		}

		/*
		 * Write tournament identification attributes
		 */
		private Dictionary<string, dynamic> writeFileHeader( String inSanctionId ) {
			String curDateString = DateTime.Now.ToString( "yyyyMMdd HH:MM" );
			String curFileCreateDate = curDateString.Substring( 0, 8 );
			String curFileCreateTime = curDateString.Substring( 9, 5 );

			String curEventDateOut = myTourRow["EventDates"].ToString();
			try {
				DateTime curEventDate = Convert.ToDateTime( curEventDateOut );
				curEventDateOut = curEventDate.ToString( "yyyy/MM/dd" );
			} catch {
				MessageBox.Show( "The event date of " + curEventDateOut + " is not a valid date and must corrected" );
				curEventDateOut = myTourRow["EventDates"].ToString();
			}

			Dictionary<string, dynamic> curOutputEntry = new Dictionary<string, dynamic> {
				{ "SanctionId", inSanctionId.ToUpper().Trim() + myTourClass }
				, { "Name",  HelperFunctions.getDataRowColValue(myTourRow, "Name", "NotAvailable") }
				, { "Class", myTourClass }
				, { "Federation", myTourFed }
				, { "MixedClass", "Y"}
				, { "RoundFmt", "M"} //S=single, M=multi
				, { "PlcmtBasis", "B"} //T=Total B=Best 2=xxxxx L=xxxxx
				, { "OverallBasis", "N"} //N=NOPS 1=xxxxx 2=xxxxx C=xxxxx P=xxxxx
				, { "EventDate", curEventDateOut}
				, { "ContactName", HelperFunctions.getDataRowColValue(myTourRow, "ContactName", "NotAvailable") }
				, { "ContactEmail", HelperFunctions.getDataRowColValue(myTourRow, "ContactEmail", "NotAvailable")}
				, { "ContactPhone", HelperFunctions.getDataRowColValue(myTourRow, "ContactPhone", "NotAvailable")}
				, { "ChiefJudgeName", HelperFunctions.getDataRowColValue(myTourRow, "ChiefJudgeName", "NotAvailable") }
				, { "ChiefJudgeEmail", HelperFunctions.getDataRowColValue(myTourRow, "ChiefJudgeEmail", "NotAvailable")}
				, { "ChiefScorerName", HelperFunctions.getDataRowColValue(myTourRow, "ChiefScorerName", "NotAvailable") }
				, { "ChiefScorerEmail", HelperFunctions.getDataRowColValue(myTourRow, "ChiefScorerEmail", "NotAvailable")}
				, { "FileCreateDate", curFileCreateDate }
				, { "FileCreateTime", curFileCreateTime }
				, { "AppTitle", Properties.Settings.Default.AppTitle}
				, { "AppVersion", Properties.Settings.Default.BuildVersion}
			};
			return new Dictionary<string, dynamic> { { "PerfFiletHeader", curOutputEntry } };
		}

		/*
		 * Write skier identification information
		 */
		private Dictionary<string, dynamic> writeSkierHeaderInfo( String inSanctionId, DataRow curSkierRow ) {
			int curSkiYearAge = 0, curBirthYear;
			String curSkierFirstName = "", curSkierLastName = "";
			String curSkierName = HelperFunctions.getDataRowColValue( curSkierRow, "SkierName", "" );
			String[] curValueList = curSkierName.Split( ',' );
			if ( curValueList.Length == 2 ) {
				curSkierLastName = curValueList[0].Trim(); // Last Name
				curSkierFirstName = curValueList[1].Trim(); // First Name
			} else {
				MessageBox.Show( "Invalid SkierName=" + curSkierName );
			}

			int curTourYear = Convert.ToInt16( inSanctionId.Substring( 0, 2 ) );
			curTourYear += 2000;
			Int32.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, "SkiYearAge", "0" ), out curSkiYearAge );
			if ( curSkiYearAge > 0 ) {
				curBirthYear = curTourYear - curSkiYearAge - (Int16)1; //6. Birth year
			} else {
				curBirthYear = curTourYear;
			}

			String curSkierTeam = "";
			String curSlalomTeam = HelperFunctions.getDataRowColValue( curSkierRow, "SlalomTeam", "" );
			String curTrickTeam = HelperFunctions.getDataRowColValue( curSkierRow, "TrickTeam", "" );
			String curJumpTeam = HelperFunctions.getDataRowColValue( curSkierRow, "JumpTeam", "" );
			if ( HelperFunctions.isObjectPopulated( curSlalomTeam ) ) {
				curSkierTeam = curSlalomTeam;
			} else if ( HelperFunctions.isObjectPopulated( curTrickTeam ) ) {
				curSkierTeam = curTrickTeam;
			} else if ( HelperFunctions.isObjectPopulated( curJumpTeam ) ) {
				curSkierTeam = curJumpTeam;
			}
			if ( HelperFunctions.isCollegiateEvent(myTourRules) ) {
				String curAgeGroup = HelperFunctions.getDataRowColValue( curSkierRow, "AgeGroup", "" ).ToUpper();
				if ( curAgeGroup.Equals( "CM" ) || curAgeGroup.Equals( "CW" ) ) {
					curSkierTeam += "/A";
				} else if ( curAgeGroup.Equals( "BM" ) || curAgeGroup.Equals( "BW" ) ) {
					curSkierTeam += "/B";
				}
			}

			return new Dictionary<string, dynamic> {
				{ "MemberId", HelperFunctions.getDataRowColValue(curSkierRow, "MemberId", "") }
				, { "SkierName", HelperFunctions.getDataRowColValue(curSkierRow, "SkierName", "") }
				, { "FirstName", curSkierFirstName }
				, { "LastName", curSkierLastName }
				, { "AgeGroup", HelperFunctions.getDataRowColValue(curSkierRow, "AgeGroup", "") }
				, { "Federation", HelperFunctions.getDataRowColValue(curSkierRow, "Federation", myTourFed) }
				, { "SkiYearAge", curSkiYearAge }
				, { "BirthYear", curBirthYear }
				, { "Gender", HelperFunctions.getDataRowColValue(curSkierRow, "Gender", "") }
				, { "City", HelperFunctions.getDataRowColValue(curSkierRow, "City", "") }
				, { "State", HelperFunctions.getDataRowColValue(curSkierRow, "State", "") }
				, { "Region", HelperFunctions.getDataRowColValue(curSkierRow, "Region", "") }
				, { "TeamCode", curSkierTeam }
			};
		}

		/*
		 * Write skier identification information
		 */
		private ArrayList writeSkierPerfData( String inSanctionId, DataRow curSkierRow, DataRow[] curSkierScoreRows ) {
			byte curRound;
			String curEventClass;
			String curMemberId = (String)curSkierRow["MemberId"];
			String curSkierName = (String)curSkierRow["SkierName"];
			String curAgeGroup = (String)curSkierRow["AgeGroup"];
			ArrayList curSkierPerfList = new ArrayList();
			Dictionary<string, dynamic> curEventEntry = null;

			foreach (DataRow curScoreRow in curSkierScoreRows ) {
				byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "RoundOverall", "0" ), out curRound );
				if ( curRound == 0 ) continue;

				curEventClass = HelperFunctions.getDataRowColValue( curScoreRow, "EventClassSlalom", "" );
				if ( HelperFunctions.isObjectPopulated( curEventClass) ) {
					curEventEntry = writeSkierPerfSlalom( inSanctionId, curSkierRow, curScoreRow );
					if ( curEventEntry != null ) curSkierPerfList.Add( curEventEntry );
				}

				curEventClass = HelperFunctions.getDataRowColValue( curScoreRow, "EventClassTrick", "" );
				if ( HelperFunctions.isObjectPopulated( curEventClass ) ) {
					curEventEntry = writeSkierPerfTrick( inSanctionId, curSkierRow, curScoreRow );
					if ( curEventEntry != null ) curSkierPerfList.Add( curEventEntry );
				}

				curEventClass = HelperFunctions.getDataRowColValue( curScoreRow, "EventClassJump", "" );
				if ( HelperFunctions.isObjectPopulated( curEventClass ) ) {
					curEventEntry = writeSkierPerfJump( inSanctionId, curSkierRow, curScoreRow );
					if ( curEventEntry != null ) curSkierPerfList.Add( curEventEntry );
				}

			}

			return curSkierPerfList;
		}

		/*
		 * Slalom score and all required attributes needed for ranking list
		 */
		private Dictionary<string, dynamic> writeSkierPerfSlalom( String inSanctionId, DataRow curSkierRow, DataRow curScoreRow ) {
			byte curRound = 0;
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "RoundSlalom", "" ), out curRound );
			if ( curRound == 0 ) return null;

			String curAgeGroup = HelperFunctions.getDataRowColValue( curScoreRow, "AgeGroup", "" );
			if ( HelperFunctions.isCollegiateEvent(myTourRules ) ) {
				if ( curAgeGroup.ToUpper().Equals( "BM" ) ) curAgeGroup = "CM";
				else if ( curAgeGroup.ToUpper().Equals( "BW" ) ) curAgeGroup = "CW";
			}

			byte curMaxSpeedKph = 0, curFinalSpeedKph = 0, curCompletedSpeedKph = 0;
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "MaxSpeed", "0" ), out curMaxSpeedKph );
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "FinalSpeedKph", "0" ), out curFinalSpeedKph );
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "CompletedSpeedKph", "0" ), out curCompletedSpeedKph );
			decimal curFinalLenMeters = HelperFunctions.getDataRowColValueDecimal( curScoreRow, "FinalLen", 23.0m );
			decimal curFinalPassScore = HelperFunctions.getDataRowColValueDecimal( curScoreRow, "FinalPassScore", 0.0m );
			decimal curSlalomScore = HelperFunctions.getDataRowColValueDecimal( curScoreRow, "ScoreSlalom", 0.0m );

			if ( curCompletedSpeedKph > 0 ) {
				if ( curCompletedSpeedKph < curFinalSpeedKph ) {
					if ( curFinalSpeedKph <= curMaxSpeedKph ) {
						curFinalPassScore = curFinalPassScore * -1;
					}
				}
			}

			return new Dictionary<string, dynamic> {
				{ "MemberId", HelperFunctions.getDataRowColValue(curScoreRow, "MemberId", "") }
				, { "AgeGroup", curAgeGroup }
				, { "Event", "Slalom" }
				, { "Class", HelperFunctions.getDataRowColValue( curScoreRow, "EventClassSlalom", "C" ) }
				, { "Round", curRound }
				, { "BoatCode", HelperFunctions.getDataRowColValue( curScoreRow, "SlalomBoatCode", "" ) }
				, { "Score", curSlalomScore }
				, { "MaxSpeedKph", curMaxSpeedKph }
				, { "FinalSpeedKph", curFinalSpeedKph }
				, { "FinalLenMeters", curFinalLenMeters }
				, { "FinalPassScore", curFinalPassScore }
				, { "CompletedSpeedKph", curCompletedSpeedKph }
				, { "Placement", HelperFunctions.getDataRowColValue( curScoreRow, "PlcmtSlalom", "999" ) }
			};
		}

		/* 
		 * 31. Trick Sanction Class (2)   Char        1       "C"
		 * 32. Trick Division Code (4)    Char        2       "B2"
		 * 33. Trick Boat Model Code (5)  Char        2       "MC"
		 * 34. Trick Score Total Pts      Num         5       99999
		 */
		private Dictionary<string, dynamic> writeSkierPerfTrick( String inSanctionId, DataRow curSkierRow, DataRow curScoreRow ) {
			byte curRound = 0;
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "RoundTrick", "" ), out curRound );
			if ( curRound == 0 ) return null;

			String curAgeGroup = HelperFunctions.getDataRowColValue( curScoreRow, "AgeGroup", "" );
			if ( HelperFunctions.isCollegiateEvent( myTourRules ) ) {
				if ( curAgeGroup.ToUpper().Equals( "BM" ) ) curAgeGroup = "CM";
				else if ( curAgeGroup.ToUpper().Equals( "BW" ) ) curAgeGroup = "CW";
			}
			int curTrickScore = 0, curScorePass1 = 0, curScorePass2 = 0;
			int.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreTrick", "0" ), out curTrickScore);
			int.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "Pass1Trick", "0" ), out curScorePass1 );
			int.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "Pass2Trick", "0" ), out curScorePass2 );

			return new Dictionary<string, dynamic> {
				{ "MemberId", HelperFunctions.getDataRowColValue(curSkierRow, "MemberId", "") }
				, { "AgeGroup", curAgeGroup }
				, { "Event", "Trick" }
				, { "Class", HelperFunctions.getDataRowColValue( curScoreRow, "EventClassTrick", "" ) }
				, { "Round", curRound }
				, { "BoatCode", HelperFunctions.getDataRowColValue( curScoreRow, "TrickBoatCode", "" ) }
				, { "Score", curTrickScore }
				, { "ScorePass1", curScorePass1 }
				, { "ScorePass2", curScorePass2 }
				, { "Placement", HelperFunctions.getDataRowColValue( curScoreRow, "PlcmtTrick", "999" ) }
			};
		}

		/*
		 * 35. Jump Sanction Class (2)    Char        1       "C"
		 * 36. Jump Division Code (4)     Char        2       "B2"
		 * 37. Jump Boat Model Code (5)   Char        2       "MC"
		 * 38. Ramp Height Ratio (4)      Num         4       .999
		 * 39. Jump Boat Speed (4)        Num         2       99
		 * 40. Best Distance (Feet)       Num         3       999
		 * 41. Best Distance (Meters)     Num         4       99.9
		 */
		private Dictionary<string, dynamic> writeSkierPerfJump( String inSanctionId, DataRow curSkierRow, DataRow curScoreRow ) {
			byte curRound = 0;
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "RoundJump", "" ), out curRound );
			if ( curRound == 0 ) return null;

			String curAgeGroup = HelperFunctions.getDataRowColValue( curScoreRow, "AgeGroup", "" );
			if ( HelperFunctions.isCollegiateEvent( myTourRules ) ) {
				if ( curAgeGroup.ToUpper().Equals( "BM" ) ) curAgeGroup = "CM";
				else if ( curAgeGroup.ToUpper().Equals( "BW" ) ) curAgeGroup = "CW";
			}

			byte curSpeedKphJump = 0;
			byte.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "SpeedKphJump", "0" ), out curSpeedKphJump );

			Int16 curScoreFeet = 0;
			String curValue = HelperFunctions.getDataRowColValue( curScoreRow, "ScoreFeet", "0" );
			Int16.TryParse( HelperFunctions.getDataRowColValue( curScoreRow, "ScoreFeet", "0" ), out curScoreFeet );
			curScoreFeet = (Int16)HelperFunctions.getDataRowColValueDecimal( curScoreRow, "ScoreFeet", 0.0m );
			decimal curScoreMeters = HelperFunctions.getDataRowColValueDecimal( curScoreRow, "ScoreMeters", 0.0m );

			decimal curRampHeightRatio = .235m;
			Decimal curRampHeight = Convert.ToDecimal( curScoreRow["RampHeight"].ToString().Trim() );
			if ( curRampHeight == 5.00M ) curRampHeightRatio = .235m;
			else if ( curRampHeight == 5.50M ) curRampHeightRatio = .255m;
			else if ( curRampHeight == 6.00M ) curRampHeightRatio = .271m;
			else if ( curRampHeight == 4.00M ) curRampHeightRatio = .215m;
			else if ( curRampHeight == 4.50M ) curRampHeightRatio = .215m;

			return new Dictionary<string, dynamic> {
				{ "MemberId", HelperFunctions.getDataRowColValue(curSkierRow, "MemberId", "") }
				, { "AgeGroup", curAgeGroup }
				, { "Event", "Jump" }
				, { "Class", HelperFunctions.getDataRowColValue( curScoreRow, "EventClassJump", "" ) }
				, { "Round", curRound }
				, { "BoatCode", HelperFunctions.getDataRowColValue( curScoreRow, "JumpBoatCode", "" ) }
				, { "ScoreFeet", curScoreFeet }
				, { "ScoreMeters", curScoreMeters }
				, { "SpeedKph", curSpeedKphJump }
				, { "RampHeightRatio", curRampHeightRatio }
				, { "Placement", HelperFunctions.getDataRowColValue( curScoreRow, "PlcmtJump", "999" ) }
			};
		}

		private void writeOutputData( StreamWriter outBuffer, Dictionary<string, dynamic> inDataEntry ) {
			String curMethodName = "ExportPerfDataV2:writeOutputData: ";
			if ( inDataEntry == null ) return;
			try {
				outBuffer.WriteLine( JsonConvert.SerializeObject( inDataEntry ) );
			
			} catch ( Exception ex ) {
				String curMsg = curMethodName + "Exception encountered writing to report output file" + "\n\nError: " + ex.Message;
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}
		private void writeOutputData( StreamWriter outBuffer, String inDataEntry ) {
			String curMethodName = "ExportPerfDataV2:writeOutputData: ";
			if ( HelperFunctions.isObjectEmpty( inDataEntry ) ) return;
			try {
				outBuffer.WriteLine( inDataEntry );

			} catch ( Exception ex ) {
				String curMsg = curMethodName + "Exception encountered writing to report output file" + "\n\nError: " + ex.Message;
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private DataRow getTourData( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT T.SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
			curSqlStmt.Append( ", ContactMemberId, ContactAddress, ContactPhone, ContactEmail, CP.SkierName AS ContactName " );
			curSqlStmt.Append( ", ChiefJudgeMemberId, ChiefJudgeEmail, CJ.SkierName AS ChiefJudgeName" );
			curSqlStmt.Append( ", ChiefScorerMemberId, ChiefScorerEmail, CC.SkierName AS ChiefScorerName" );
			curSqlStmt.Append( ", ChiefDriverMemberId, ChiefDriverAddress, ChiefDriverPhone, ChiefDriverEmail, CD.SkierName AS ChiefDriverName " );
			curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirAddress, SafetyDirPhone, SafetyDirEmail, SD.SkierName AS SafetyDirName " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CP " );
			curSqlStmt.Append( "    ON CP.SanctionId = T.SanctionId AND CP.MemberId = T.ContactMemberId " );
			curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CJ " );
			curSqlStmt.Append( "    ON CJ.SanctionId = T.SanctionId AND CJ.MemberId = T.ChiefJudgeMemberId " );
			curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CC " );
			curSqlStmt.Append( "    ON CC.SanctionId = T.SanctionId AND CC.MemberId = T.ChiefScorerMemberId " );
			curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CD " );
			curSqlStmt.Append( "    ON CD.SanctionId = T.SanctionId AND CD.MemberId = T.ChiefDriverMemberId " );
			curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) SD " );
			curSqlStmt.Append( "    ON SD.SanctionId = T.SanctionId AND SD.MemberId = T.SafetyDirMemberId " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}

		private byte getIwwfSlalomMin( String inDiv ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = 'IwwfSlalomMin' " );
			curSqlStmt.Append( "  AND ListCode = '" + inDiv + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return Convert.ToByte( HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "MaxValue", "0" ) );
			} else {
				return 25;
			}
		}
	}
}
