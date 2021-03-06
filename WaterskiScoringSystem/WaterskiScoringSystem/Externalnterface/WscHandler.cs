﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Externalnterface {
	class WscHandler {
		private static String mySanctionNum;

		private static DataRow myTourRow;

		private static Boolean myConnectActive = false;
		private static DateTime myCurrentDatetime = DateTime.Now;

		private static char[] singleQuoteDelim = new char[] { '\'' };

		private static String myLastConnectResponse = "";
		private static Boolean myUseJumpTimes = false;
		private static int myDefaultRound = 0;

		public static Boolean useJumpTimes {
			get {
				return myUseJumpTimes;
			}
			set {
				myUseJumpTimes = value;
			}
		}

		public static void startWscMessageHhandler() {
			Process exeProcess = new Process();
			//exeProcess.
			exeProcess.StartInfo.FileName = "Notepad.exe";
			exeProcess.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
			exeProcess.Start();
		}

		public static void showPin() {
			MessageBox.Show( myLastConnectResponse );
		}

		public static Boolean sendBoatData( String boatId, String boatManufacturer, String boatModel
			, Int16 boatYear, String boatColor, String boatComment ) {
			Dictionary<string, string> sendMsg = new Dictionary<string, string> {
					{ "boatId", boatId }
					, { "boatManufacturer", boatManufacturer }
					, { "boatModel", boatModel }
					, { "boatYear", boatYear.ToString() }
					, { "boatColor", boatColor }
					, { "boatComment", boatComment }
				};
			addEwscMsg( "boat_data", JsonConvert.SerializeObject( sendMsg ) );
			return true;
		}

		public static Boolean sendPassData( String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion, String eventGroup, String div, String gender
			, String round, Int16 passNumber, Int16 speed, String rope, String split, String driverMemberId ) {
			int curRound = int.Parse( round );
			myDefaultRound = curRound;
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteDivision", div }
					, { "athleteGender", gender }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "round", curRound }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "split", split }
				};

			if ( driverMemberId != null && driverMemberId.Length > 0 ) {
				DataTable curDataTable = getDriverAssignment( driverMemberId );
				if ( curDataTable.Rows.Count > 0 ) {
					DataRow curDataRow = curDataTable.Rows[( curDataTable.Rows.Count - 1 )];
					sendMsg.Add( "driver", buildOfficialEntry( curDataRow ) );
				} else {
					sendMsg.Add( "driver", buildOfficialEntry( null ) );
				}

			} else {
				sendMsg.Add( "driver", buildOfficialEntry( null ) );
			}

			addEwscMsg( "pass_data", JsonConvert.SerializeObject( sendMsg ) );
			myCurrentDatetime = DateTime.Now;
			return true;
		}

		public static Boolean sendAthleteScore( String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion, String athleteGroup
			, Int16 round, Int16 passNumber, Int16 speed, String rope, String score ) {
			myDefaultRound = round;
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "round", round }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "score", score + "@" + speed.ToString() + "/" + rope }
				};
			addEwscMsg( "scoring_score", JsonConvert.SerializeObject( sendMsg ) );

			sendLeaderBoard( athleteId, athleteEvent, athleteGroup, round );

			return true;
		}

		private static void sendLeaderBoard( String athleteId, String athleteEvent, String athleteGroup, Int16 inRound ) {
			ArrayList leaderBoard = new ArrayList();
			Dictionary<string, dynamic> curSkier = null;
			Dictionary<string, dynamic> curLeader = null;
			String curFederation = "";

			int curRow = 0;
			DataTable curDataTable = getLeaderBoardData( athleteEvent, athleteGroup, inRound );
			if ( curDataTable == null || curDataTable.Rows.Count <= 0 ) return;

			try {
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					if ( ( (String)curDataRow["MemberId"] ).Equals( athleteId ) ) {
						curFederation = "";
						if ( curDataRow["Federation"] != System.DBNull.Value ) curFederation = (String)curDataRow["Federation"];
						if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];
						if ( athleteEvent.Equals( "Slalom" ) ) {
							curSkier = new Dictionary<string, object> {
							{ "athleteId", (String)curDataRow["MemberId"] }
							, { "athleteName", (String)curDataRow["SkierName"] }
							, { "athleteCountry", curFederation.ToUpper() }
							, { "athleteRegion", (String)curDataRow["State"] }
							, { "athleteDivision", (String)curDataRow["AgeGroup"] }
							, { "athleteGroup", (String)curDataRow["EventGroup"] }
							, { "position_current", Convert.ToInt32(curRow + 1) }
							, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["FinalPassScore"], (String)curDataRow["FinalLen"], (byte)curDataRow["FinalSpeedKph"]) }
							, { "score_buoys", (decimal)curDataRow["Score"] }
							};

						} else if ( athleteEvent.Equals( "Jump" ) ) {
							curSkier = new Dictionary<string, object> {
							{ "athleteId", (String)curDataRow["MemberId"] }
							, { "athleteName", (String)curDataRow["SkierName"] }
							, { "athleteCountry", curFederation.ToUpper() }
							, { "athleteRegion", (String)curDataRow["State"] }
							, { "athleteDivision", (String)curDataRow["AgeGroup"] }
							, { "athleteGroup", (String)curDataRow["EventGroup"] }
							, { "position_current", Convert.ToInt32(curRow + 1) }
							, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["ScoreMeters"], (decimal)curDataRow["ScoreFeet"], (byte)curDataRow["BoatSpeed"]) }
							};
						}

						break;
					}

					curRow++;
				}
			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "Exception encountered on row {0} : {1}", curRow, ex.Message ) );
			}

			try {
				curRow = 0;
				DataRow curDataRow = curDataTable.Rows[0];
				curFederation = "";
				if ( curDataRow["Federation"] != System.DBNull.Value ) curFederation = (String)curDataRow["Federation"];
				if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];
				if ( athleteEvent.Equals( "Slalom" ) ) {
					curLeader = new Dictionary<string, object> {
						{ "athleteId", (String)curDataRow["MemberId"] }
						, { "athleteName", (String)curDataRow["SkierName"] }
						, { "athleteCountry", curFederation.ToUpper() }
						, { "athleteRegion", (String)curDataRow["State"] }
						, { "athleteDivision", (String)curDataRow["AgeGroup"] }
						, { "athleteGroup", (String)curDataRow["EventGroup"] }
						, { "position_current", Convert.ToInt32(curRow + 1) }
						, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["FinalPassScore"], (String)curDataRow["FinalLen"], (byte)curDataRow["FinalSpeedKph"]) }
						, { "score_buoys", (decimal)curDataRow["Score"] }
					};
				} else if ( athleteEvent.Equals( "Jump" ) ) {
					curLeader = new Dictionary<string, object> {
						{ "athleteId", (String)curDataRow["MemberId"] }
						, { "athleteName", (String)curDataRow["SkierName"] }
						, { "athleteCountry", curFederation.ToUpper() }
						, { "athleteRegion", (String)curDataRow["State"] }
						, { "athleteDivision", (String)curDataRow["AgeGroup"] }
						, { "athleteGroup", (String)curDataRow["EventGroup"] }
						, { "position_current", Convert.ToInt32(curRow + 1) }
						, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["ScoreMeters"], (decimal)curDataRow["ScoreFeet"], (byte)curDataRow["BoatSpeed"]) }
					};
				}

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "Exception encountered on row {0} : {1}", curRow, ex.Message ) );
			}

			curRow = 0;
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				try {
					curFederation = "";
					if ( curDataRow["Federation"] != System.DBNull.Value ) curFederation = (String)curDataRow["Federation"];
					if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];

					if ( athleteEvent.Equals( "Slalom" ) ) {
						leaderBoard.Add( new Dictionary<string, object> {
							{ "athleteId", (String)curDataRow["MemberId"] }
							, { "athleteName", (String)curDataRow["SkierName"] }
							, { "athleteCountry", curFederation.ToUpper() }
							, { "athleteRegion", (String)curDataRow["State"] }
							, { "athleteDivision", (String)curDataRow["AgeGroup"] }
							, { "athleteGroup", (String)curDataRow["EventGroup"] }
							, { "position_current", Convert.ToInt32(curRow + 1) }
							, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["FinalPassScore"], (String)curDataRow["FinalLen"], (byte)curDataRow["FinalSpeedKph"]) }
							, { "score_buoys", (decimal)curDataRow["Score"] }
							} );

					} else if ( athleteEvent.Equals( "Jump" ) ) {
						leaderBoard.Add( new Dictionary<string, object> {
							{ "athleteId", (String)curDataRow["MemberId"] }
							, { "athleteName", (String)curDataRow["SkierName"] }
							, { "athleteCountry", curFederation.ToUpper() }
							, { "athleteRegion", (String)curDataRow["State"] }
							, { "athleteDivision", (String)curDataRow["AgeGroup"] }
							, { "athleteGroup", (String)curDataRow["EventGroup"] }
							, { "position_current", Convert.ToInt32(curRow + 1) }
							, { "score_current", String.Format("{0}/{1}@{2}", (decimal)curDataRow["ScoreMeters"], (decimal)curDataRow["ScoreFeet"], (byte)curDataRow["BoatSpeed"]) }
							} );
					}

					curRow++;

				} catch ( Exception ex ) {
					MessageBox.Show( String.Format( "Exception encountered on row {0} : {1}", curRow, ex.Message ) );
					String curValue = ex.Message;
				}
			}

			sendLeaderBoardMsg( athleteEvent, inRound, athleteGroup, curSkier, curLeader, leaderBoard );
		}

		/*
		 * Send boat_times manually entered into WSTIMS
		 */
		public static void sendJumpBoatTimes( String athleteId, String athleteName, String athleteEvent
			, Int16 round, Int16 passNumber, Int16 speed, decimal splitTime, decimal splitTime2, decimal endTime ) {
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
				{ "athleteId", athleteId }
				, { "athleteName", athleteName }
				, { "athleteEvent", athleteEvent }
				, { "round", round }
				, { "passNumber", passNumber }
				, { "speed", speed }
				, { "rope", "" }
				, { "nt", splitTime.ToString("0.00") }
				, { "mt", splitTime2.ToString("0.00") }
				, { "et", endTime.ToString("0.00") }
				};
			addEwscMsg( "boat_times_scoring", JsonConvert.SerializeObject( sendMsg ) );
		}

		public static Boolean sendRunningOrder( String curEvent, int curRound, DataTable curDataTable ) {
			ArrayList startListAthletes = new ArrayList();

			int curRow = 0;
			String curEventGroup = "", prevEventGroup = "";
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				try {
					curEventGroup = (String)curDataRow["EventGroup"];
					if ( prevEventGroup != curEventGroup && startListAthletes.Count > 0 ) {
						sendRunningOrderForGroup( curEvent, curRound, prevEventGroup, startListAthletes );
						startListAthletes = new ArrayList();
					}

					String curFederation = "";
					if ( curDataRow["Federation"] != System.DBNull.Value ) curFederation = (String)curDataRow["Federation"];
					if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];

					startListAthletes.Add( new Dictionary<string, object> {
					{ "athleteId", (String)curDataRow["memberId"] }
					, { "athleteName", (String)curDataRow["SkierName"] }
					, { "athleteCountry", curFederation.ToUpper() }
					, { "athleteRegion", (String)curDataRow["State"] }
					, { "athleteDivision", (String)curDataRow["AgeGroup"] }
					, { "athleteGroup", (String)curDataRow["EventGroup"] }
					, { "position_starting", Convert.ToInt32(curRow + 1) }
					, { "position_seed", Convert.ToInt32(curDataTable.Rows.Count - curRow) }
					, { "position_current", Convert.ToInt32(curRow + 1) }
					, { "score_seed", (decimal)curDataRow["RankingScore"] }
					, { "score_current", Convert.ToDecimal(0) }
					} );

					prevEventGroup = curEventGroup;
					curRow++;

				} catch ( Exception ex ) {
					MessageBox.Show( String.Format( "Exception encountered on row {0} : {1}", curRow, ex.Message ) );
					String curValue = ex.Message;
				}
			}

			if ( startListAthletes.Count > 0 ) {
				sendRunningOrderForGroup( curEvent, curRound, prevEventGroup, startListAthletes );
			}

			return true;
		}

		public static Boolean sendOfficialsAssignments( String curEvent, String eventGroup, Int16 round ) {
			int towerJudgeIdx = 1;
			Boolean officialsAvailable = false;
			Dictionary<string, object> curEventOfficials = new Dictionary<string, object>();

			DataTable curDataTable = getOfficialAssignments( curEvent, eventGroup, round );
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				String curOfficialAsgmt = (String)curDataRow["WorkAsgmt"];
				if ( curOfficialAsgmt.Equals( "Driver" ) ) {
					if ( curEventOfficials.ContainsKey( "driver" ) ) continue;
					curEventOfficials.Add( "driver", buildOfficialEntry( curDataRow ) );
					officialsAvailable = true;

				} else if ( curOfficialAsgmt.Equals( "Event Judge" ) ) {
					String judgeSuffix = "";
					if ( towerJudgeIdx == 1 ) {
						judgeSuffix = "1a";
					} else if ( towerJudgeIdx == 2 ) {
						judgeSuffix = "1b";
					} else if ( towerJudgeIdx == 3 ) {
						judgeSuffix = "2a";
					} else if ( towerJudgeIdx == 4 ) {
						judgeSuffix = "2b";
					}
					if ( towerJudgeIdx <= 4 ) {
						curEventOfficials.Add( "towerJudge" + judgeSuffix, buildOfficialEntry( curDataRow ) );
						officialsAvailable = true;
					}
					towerJudgeIdx++;

				} else if ( curOfficialAsgmt.Equals( "Boat Judge" ) ) {
					if ( curEventOfficials.ContainsKey( "boatJudge" ) ) continue;
					curEventOfficials.Add( "boatJudge", buildOfficialEntry( curDataRow ) );
					officialsAvailable = true;

				} else if ( curOfficialAsgmt.Equals( "Scorer" ) ) {
					if ( curEventOfficials.ContainsKey( "scorer" ) ) continue;
					curEventOfficials.Add( "scorer", buildOfficialEntry( curDataRow ) );
					officialsAvailable = true;
				}
			}
			if ( officialsAvailable ) {
				addEwscMsg( "officials_data", JsonConvert.SerializeObject( curEventOfficials ) );
				return true;
			}

			return false;

		}

		public static decimal[] getBoatTime( String curEvent, String curMemberId, String curRound, String curPassNum, Decimal curPassLineLength, Int16 curPassSpeedKph, Decimal inPassScore ) {
			if ( curEvent.Length == 0 || curMemberId.Length == 0 || curRound.Length == 0 || curPassNum.Length == 0 ) return new decimal[] { };
			if ( curEvent.Equals( "Jump" ) && myUseJumpTimes == false ) return new decimal[] { };
			if ( mySanctionNum == null || mySanctionNum.Length == 0 ) getSanctionNum();

			Int64 btPK = getBoatTimeKey( curEvent, curMemberId, curRound, curPassNum, curPassLineLength, curPassSpeedKph );
			if ( btPK <= 0 ) return new decimal[] { };

			/*
			 * Retrieve boat time record for the specific record found during the search
			 */
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM BoatTime Where PK = " + btPK );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				if ( curEvent.Equals( "Slalom" ) ) {
					int curPassScore = Convert.ToInt32( Math.Floor( inPassScore ) ) + 1;
					String timeKey = "BoatTimeBuoy" + curPassScore;
					return new decimal[] { (decimal)curDataTable.Rows[0][timeKey] };

				} else if ( curEvent.Equals( "Jump" ) ) {
					return new decimal[] { (decimal)curDataTable.Rows[0]["BoatTimeBuoy1"]
							, (decimal)curDataTable.Rows[0]["BoatTimeBuoy2"]
							, (decimal)curDataTable.Rows[0]["BoatTimeBuoy3"]
						};

				}
			}

			return new decimal[] { };
		}

		public static DataRow getBoatPath( String curEvent, String curMemberId, String curRound, String curPassNum, Decimal curPassLineLength, Int16 curPassSpeedKph ) {
			if ( curEvent.Length == 0 || curMemberId.Length == 0 || curRound.Length == 0 || curPassNum.Length == 0 ) return null;
			if ( mySanctionNum == null || mySanctionNum.Length == 0 ) getSanctionNum();

			Int64 bpPK = getBoatPathKey( curEvent, curMemberId, curRound, curPassNum, curPassLineLength, curPassSpeedKph );
			Int64 btPK = getBoatTimeKey( curEvent, curMemberId, curRound, curPassNum, curPassLineLength, curPassSpeedKph );
			if ( btPK <= 0 || bpPK <= 0 ) return null;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT P.Event, P.Round, P.PassNumber, P.PassLineLength, P.PassspeedKph" );
			curSqlStmt.Append( ", P.PathDevBuoy0, P.PathDevCum0, P.PathDevZone0" );
			curSqlStmt.Append( ", P.PathDevBuoy1, P.PathDevCum1, P.PathDevZone1, T.BoatTimeBuoy1" );
			curSqlStmt.Append( ", P.PathDevBuoy2, P.PathDevCum2, P.PathDevZone2, T.BoatTimeBuoy2" );
			curSqlStmt.Append( ", P.PathDevBuoy3, P.PathDevCum3, P.PathDevZone3, T.BoatTimeBuoy3" );
			curSqlStmt.Append( ", P.PathDevBuoy4, P.PathDevCum4, P.PathDevZone4, T.BoatTimeBuoy4" );
			curSqlStmt.Append( ", P.PathDevBuoy5, P.PathDevCum5, P.PathDevZone5, T.BoatTimeBuoy5" );
			curSqlStmt.Append( ", P.PathDevBuoy6, P.PathDevCum6, P.PathDevZone6, T.BoatTimeBuoy6, T.BoatTimeBuoy7" );
			curSqlStmt.Append( ", P.InsertDate, P.LastUpdateDate " );
			curSqlStmt.Append( "FROM BoatPath P " );
			curSqlStmt.Append( "Left Outer Join BoatTime T on T.PK = " + btPK + " " );
			curSqlStmt.Append( "WHERE P.PK = " + bpPK );

			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];

			return null;
		}

		private static void sendRunningOrderForGroup( String curEvent, int curRound, String curEventGroup, ArrayList startListAthletes ) {
			Dictionary<string, object> startListMsg = new Dictionary<string, object> {
					{ "startlistName", "EventGroup " + curEventGroup }
					, { "round", curRound }
					, { "eventName", curEvent }
					, { "division", curEventGroup }
					, { "group", curEventGroup }
					, { "lake", "" }
					, { "current_athlete_index", 0 }
					, { "current_athlete_name", "" }
					, { "startlist_athletes", startListAthletes }
				};

			addEwscMsg( "start_list", JsonConvert.SerializeObject( startListMsg ) );
		}

		private static void sendLeaderBoardMsg( String curEvent, int curRound, String curEventGroup, Dictionary<string, dynamic> curSkier, Dictionary<string, dynamic> curLeader, ArrayList leaderBoard ) {
			Dictionary<string, object> leaderboardMsg = new Dictionary<string, object> {
					{ "leaderboardName", curEventGroup }
					, { "division", curEventGroup }
					, { "eventName", curEvent }
					, { "round", curRound }
					, { "current_skier", curSkier }
					, { "current_leader", curLeader }
					, { "leaderboard_athletes", leaderBoard }
				};

			addEwscMsg( "leaderboard", JsonConvert.SerializeObject( leaderboardMsg ) );
		}

		private static void addEwscMsg( String msgType, String msgData ) {
			String curMsgData = stringReplace( msgData, singleQuoteDelim, "''" );

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert EwscMsg ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( "'" + mySanctionNum + "'" );
			curSqlStmt.Append( ", '" + msgType + "'" );
			curSqlStmt.Append( ", '" + curMsgData + "'" );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		public static String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
			StringBuilder curNewValue = new StringBuilder( "" );

			String[] curValues = inValue.Split( inCurValue );
			if ( curValues.Length > 1 ) {
				int curCount = 0;
				foreach ( String curValue in curValues ) {
					curCount++;
					if ( curCount < curValues.Length ) {
						curNewValue.Append( curValue + inReplValue );
					} else {
						curNewValue.Append( curValue );
					}
				}
			} else {
				curNewValue.Append( inValue );
			}

			return curNewValue.ToString();
		}

		private static void getSanctionNum() {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				String msg = "An active tournament must be selected from the Administration menu Tournament List option";
				MessageBox.Show( msg );
				throw new Exception( msg );

			} else if ( mySanctionNum.Length < 6 ) {
				String msg = "An active tournament must be selected from the Administration menu Tournament List option";
				MessageBox.Show( msg );
				throw new Exception( msg );

			} else {
				//Retrieve selected tournament attributes
				DataTable curTourDataTable = getTourData();
				myTourRow = curTourDataTable.Rows[0];
			}
		}

		private static Dictionary<string, object> buildOfficialEntry( DataRow curDataRow ) {
			if ( curDataRow == null ) {
				return new Dictionary<string, object> {
					{ "officialId", "" }
					, { "officialName", "" }
					, { "officialCountry", "" }
					, { "officialRegion", "" } };
			}

			String curFederation = "";
			try {
				curFederation = (String)curDataRow["Federation"];
				if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];
			} catch {
				if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];
			}
			return new Dictionary<string, object> {
					{ "officialId", (String)curDataRow["MemberId"] }
					, { "officialName", (String)curDataRow["MemberName"] }
					, { "officialCountry", curFederation.ToUpper() }
					, { "officialRegion", (String)curDataRow["State"] } };
		}
		private static Int64 getBoatPathKey( String curEvent, String curMemberId, String curRound, String curPassNum, Decimal curPassLineLength, Int16 curPassSpeedKph ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Int64 bpPK = -1;


			curSqlStmt.Append( "SELECT PK FROM BoatPath P " );
			curSqlStmt.Append( "WHERE P.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "AND P.MemberId = '" + curMemberId + "' " );
			curSqlStmt.Append( "AND P.Event = '" + curEvent + "' " );
			curSqlStmt.Append( "AND P.Round = " + curRound + " " );
			curSqlStmt.Append( "AND P.PassNumber = " + curPassNum + " " );
			curSqlStmt.Append( "Order by P.PK " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			for ( int count = 0; count < 5; count++ ) {
				if ( curDataTable.Rows.Count > 0 ) {
					bpPK = (Int64)curDataTable.Rows[0]["PK"];
					break;
				}
				Thread.Sleep( 500 );
			}
			if ( bpPK <= 0 ) return -1;
			if ( curDataTable.Rows.Count == 1 ) return bpPK;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK FROM BoatPath P " );
			curSqlStmt.Append( "WHERE P.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "AND P.MemberId = '" + curMemberId + "' " );
			curSqlStmt.Append( "AND P.Event = '" + curEvent + "' " );
			curSqlStmt.Append( "AND P.Round = " + curRound + " " );
			curSqlStmt.Append( "AND P.PassNumber = " + curPassNum + " " );
			curSqlStmt.Append( "AND P.PassLineLength = " + curPassLineLength + " " );
			curSqlStmt.Append( "AND P.PassSpeedKph = " + curPassSpeedKph + " " );
			curSqlStmt.Append( "Order by P.PK " );
			curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return (Int64)curDataTable.Rows[0]["PK"];

			return bpPK;
		}

		/*
		 * Check for a matching boat time record for the skier, round, and pass
		 * Search for 2.5 seconds and if a record is not found then return an empty result
		 * Save the PK index of the first matching record
		 */
		private static Int64 getBoatTimeKey( String curEvent, String curMemberId, String curRound, String curPassNum, Decimal curPassLineLength, Int16 curPassSpeedKph ) {
			Int64 btPK = -1;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT count(*) as rowCount FROM BoatTime T WHERE T.SanctionId = '" + mySanctionNum + "' " );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK FROM BoatTime T " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "AND T.MemberId = '" + curMemberId + "' " );
			curSqlStmt.Append( "AND T.Event = '" + curEvent + "' " );
			curSqlStmt.Append( "AND T.Round = " + curRound + " " );
			curSqlStmt.Append( "AND T.PassNumber = " + curPassNum + " " );
			curSqlStmt.Append( "Order by T.PK " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			for ( int count = 0; count < 5; count++ ) {
				if ( curDataTable.Rows.Count > 0 ) {
					btPK = (Int64)curDataTable.Rows[0]["PK"];
					break;
				}
				Thread.Sleep( 500 );
			}
			if ( btPK <= 0 ) return -1;
			if ( curDataTable.Rows.Count == 1 ) return btPK;

			/*
			 * If more than 1 matching record is found then search for a more specific record in case the pass was deleted and retried
			 */
			if ( curDataTable.Rows.Count > 1 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT PK FROM BoatTime T " );
				curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
				curSqlStmt.Append( "AND T.MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "AND T.Event = '" + curEvent + "' " );
				curSqlStmt.Append( "AND T.Round = " + curRound + " " );
				curSqlStmt.Append( "AND T.PassNumber = " + curPassNum + " " );
				curSqlStmt.Append( "AND T.PassLineLength = " + curPassLineLength + " " );
				curSqlStmt.Append( "AND T.PassSpeedKph = " + curPassSpeedKph + " " );
				curSqlStmt.Append( "Order by T.PK " );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) return (Int64)curDataTable.Rows[0]["PK"];
			}

			return btPK;
		}

		private static DataTable getOfficialAssignments( String inEvent, String inEventGroup, Int16 inRound ) {
			if ( mySanctionNum == null || mySanctionNum.Length == 0 ) getSanctionNum();
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt" );
			curSqlStmt.Append( ", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation " );
			curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
			curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T " );
			curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
			curSqlStmt.Append( "     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId " );
			curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
			curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
			curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
			curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getDriverAssignment( String inEvent, String inEventGroup, String inRound ) {
			if ( mySanctionNum == null || mySanctionNum.Length == 0 ) getSanctionNum();
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt" );
			curSqlStmt.Append( ", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation " );
			curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
			curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T " );
			curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
			curSqlStmt.Append( "     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId " );
			curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
			curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
			curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
			curSqlStmt.Append( "  AND O.WorkAsgmt = 'Driver' " );
			curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}
		private static DataTable getDriverAssignment( String inMemberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select Distinct T.SanctionId, T.MemberId, T.SkierName AS MemberName, T.State, X.Federation as TourFederation " );
			curSqlStmt.Append( "From TourReg T" );
			curSqlStmt.Append( "     INNER JOIN Tournament AS X on X.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "  AND T.MemberId = '" + inMemberId + "' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static DataTable getLeaderBoardData( String athleteEvent, String athleteGroup, Int16 inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			if ( athleteEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, TR.State" );
				curSqlStmt.Append( ", COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt, TR.Federation, T.Federation as TourFederation" );
				curSqlStmt.Append( ", SS.Round, SS.Score, SS.FinalSpeedMph, SS.FinalSpeedKph, SS.FinalLen, SS.FinalLenOff, SS.FinalPassScore " );
				curSqlStmt.Append( "FROM TourReg TR " );
				curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup" );
				curSqlStmt.Append( "  INNER JOIN SlalomScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup" );
				curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
				curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.EventGroup = '" + athleteGroup + "' " );
				curSqlStmt.Append( "AND ER.Event = '" + athleteEvent + "' AND SS.Round = " + inRound + " " );
				curSqlStmt.Append( "ORDER BY ER.ReadyForPlcmt DESC, ER.EventGroup, SS.Score DESC, TR.SkierName, SS.Round " );

			} else if ( athleteEvent.Equals( "Jump" ) ) {
				curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, TR.State" );
				curSqlStmt.Append( ", COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmt, TR.Federation, T.Federation as TourFederation" );
				curSqlStmt.Append( ", SS.Round, SS.ScoreFeet, SS.ScoreMeters, SS.BoatSpeed " );
				curSqlStmt.Append( "FROM TourReg TR " );
				curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup" );
				curSqlStmt.Append( "  INNER JOIN JumpScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup" );
				curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
				curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.EventGroup = '" + athleteGroup + "' " );
				curSqlStmt.Append( "AND ER.Event = '" + athleteEvent + "' AND SS.Round = " + inRound + " " );
				curSqlStmt.Append( "ORDER BY ER.ReadyForPlcmt DESC, ER.EventGroup, SS.ScoreMeters DESC, ScoreFeet DESC, TR.SkierName, SS.Round " );
			} else {
				return null;
			}

			return DataAccess.getDataTable( curSqlStmt.ToString() );

		}

		private static DataTable getTourData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
			curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

	}
}
