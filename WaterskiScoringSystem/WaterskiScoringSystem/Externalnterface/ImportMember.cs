using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Externalnterface {
	class ImportMember {
		private bool myTourTypePickAndChoose = false;

		private String mySanctionNum;

		private int myCountMemberInput = 0;
		private int myCountMemberAdded = 0;
		private int myCountMemberUpdate = 0;
		private int myCountTourRegAdded = 0;
		private int myCountSlalomAdded = 0;
		private int myCountTrickAdded = 0;
		private int myCountJumpAdded = 0;

        private int myCountMemberUpdated = 0;
        private int myCountMemberTourUpdated = 0;
        private int myCountMemberTourOfficialUpdated = 0;

        private DataRow myTourRow;
        private ProgressWindow myProgressInfo = null;
        private MemberIdValidate myMemberIdValidate;
		private TourEventReg myTourEventReg;

		public ImportMember( DataRow inTourRow ) {
			initImportMember( inTourRow, null );
        }
		public ImportMember( DataRow inTourRow, String tourType ) {
			initImportMember( inTourRow, tourType );
		}

		private void initImportMember( DataRow inTourRow, String tourType ) {
			myTourTypePickAndChoose = false;
			if ( tourType != null && tourType.ToLower().Equals( "pick" ) ) {
				DialogResult msgResp = MessageBox.Show( "You have choosen to import registrations using a Pick & Choose format."
					+ "\nThis format will create separate running orders for individual rounds"
					+ "\nAre you sure you want to continue with this format?"
					, "Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1 );
				if ( msgResp == DialogResult.Yes ) {
					myTourTypePickAndChoose = true;
				
				} else {
					return;
				}
			}

			if ( inTourRow == null ) {
				mySanctionNum = Properties.Settings.Default.AppSanctionNum;
				if ( mySanctionNum == null ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					return;
				}
				if ( mySanctionNum.Length < 6 ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					return;
				}
				
				myTourRow = getTourData();

			} else {
				myTourRow = inTourRow;
				mySanctionNum = (String) inTourRow["SanctionId"];
			}

			myProgressInfo = null;

			myMemberIdValidate = new MemberIdValidate();

			myTourEventReg = new TourEventReg();

			myCountMemberInput = 0;
			myCountMemberAdded = 0;
			myCountMemberUpdate = 0;
			myCountTourRegAdded = 0;
			myCountSlalomAdded = 0;
			myCountTrickAdded = 0;
			myCountJumpAdded = 0;
		}

		public void displayMemberProcessCounts() {
			MessageBox.Show( "Info: Member import processed"
				+ "\nMember records read: " + myCountMemberInput
				+ "\nmyCountMemberAdded: " + myCountMemberAdded
				+ "\nmyCountMemberUpdate: " + myCountMemberUpdate
				+ "\nmyCountTourRegAdded: " + myCountTourRegAdded
				+ "\nmyCountSlalomAdded: " + myCountSlalomAdded
				+ "\nmyCountTrickAdded: " + myCountTrickAdded
				+ "\nmyCountJumpAdded: " + myCountJumpAdded
				);

		}

		public void importPreRegMembers() {
			/* -----------------------------------------------------------------------
            * Configure URL to retrieve all skiers pre-registered for the active tournament
			* This will include all appointed officials
            ----------------------------------------------------------------------- */
			String curQueryString = "?SanctionId=" + mySanctionNum;
			String curContentType = "application/json; charset=UTF-8";
			String curRegExportListUrl = "http://www.usawaterski.org/admin/GetMemberRegExportJson.asp";
			String curReqstUrl = curRegExportListUrl + curQueryString;
			String curSanctionEditCode = (String) myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve skier registrations and member information.  Enter required value on Tournament Form" );
				return;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			//curResponseDataList = SendMessageHttp.getMessageResponseJsonArray(curReqstUrl, curHeaderParams, curContentType, "wstims", "Slalom38tTrick13Jump250", false);
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList == null || curResponseDataList.Count == 0 ) {
				displayMemberProcessCounts();
				return;
			}

			myProgressInfo = new ProgressWindow();
			myProgressInfo.setProgressMin( 1 );
			myProgressInfo.setProgressMax( curResponseDataList.Count );

			IwwfMembership.warnMessageActive = false;

			if ( mySanctionNum.Substring( 2, 1 ).ToUpper().Equals( "U" ) ) {
				Dictionary<String, object> curTeamHeaderList = new Dictionary<String, object>();

				foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
					myCountMemberInput++;
					myProgressInfo.setProgressValue( myCountMemberInput );

					importNcwsMemberFromAwsa( curEntry );

					buildNcwsaTeamHeader( curEntry, curTeamHeaderList );
				}

				foreach ( Dictionary<string, object> curEntry in curTeamHeaderList.Values ) {
					procTeamHeaderInput( curEntry, true );
				}

			} else {
				foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
					myCountMemberInput++;
					myProgressInfo.setProgressValue( myCountMemberInput );

					importMemberFromAwsa( curEntry, true, false );
				}
			}

			Cursor.Current = Cursors.Default;
			myProgressInfo.Close();
			IwwfMembership.showBulkWarnMessage();
			IwwfMembership.warnMessageActive = false;

			displayMemberProcessCounts();
        }

		/*
		 * Update the memeber data record and also update tournament registration record for the member data
		 */
		public bool importMemberFromAwsa( Dictionary<string, object> curImportMemberEntry, bool inTourReg, bool inNcwsa ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			MemberEntry curMemberEntry = new MemberEntry();

			try {
				if ( curImportMemberEntry.ContainsKey( "MemberID" ) ) {
					curMemberEntry.MemberId = (String)curImportMemberEntry["MemberID"];
				} else {
					curMemberEntry.MemberId = (String)curImportMemberEntry["MemberId"];
				}
				if ( !( myMemberIdValidate.checkMemberId( curMemberEntry.MemberId ) ) ) {
					MessageBox.Show( String.Format( "Invalid member id {0}, checksum validation failed", curMemberEntry.MemberId ) );
					return false;
				}

				if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
					curMemberEntry.AgeGroup = ( (String)curImportMemberEntry["AgeGroup"] ).ToUpper();
				} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
					curMemberEntry.AgeGroup = ( (String)curImportMemberEntry["Div"] ).ToUpper();
				}
				curMemberEntry.FirstName = (String)curImportMemberEntry["FirstName"];
				curMemberEntry.LastName = (String)curImportMemberEntry["LastName"];
				
				if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
					curMemberEntry.Gender = (String)curImportMemberEntry["Gender"];
				} else if ( curMemberEntry.AgeGroup.Length > 1 ) {
					curMemberEntry.Gender = myTourEventReg.getGenderOfAgeDiv( curMemberEntry.AgeGroup );
				}
				
				if ( curImportMemberEntry.ContainsKey( "Team" ) ) {
					curMemberEntry.Team = ( (String)curImportMemberEntry["Team"] ).ToUpper().Trim();
				}

				//For collegiate divisions determine if data is valid for collegiate tournaments
				curMemberEntry.EventGroupSlalom = ( (String)curImportMemberEntry["EventSlalom"] ).Trim();
				if ( curMemberEntry.EventGroupSlalom.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupSlalom = "";
				curMemberEntry.EventGroupTrick = ( (String)curImportMemberEntry["EventTrick"] ).Trim();
				if ( curMemberEntry.EventGroupTrick.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupTrick = "";
				curMemberEntry.EventGroupJump = ( (String)curImportMemberEntry["EventJump"] ).Trim();
				if ( curMemberEntry.EventGroupJump.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupJump = "";

				importMemberEntry( curImportMemberEntry, curMemberEntry, inTourReg, inNcwsa );

				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "importMemberFromAwsa: Error processing member"
					+ "\nSkier {0} {1} {2} {3}"
					+ "\nException: {4}"
					, curMemberEntry.MemberId, curMemberEntry.FirstName, curMemberEntry.LastName, curMemberEntry.AgeGroup, ex.Message ) );
				return false;
			}
		}

		/*
		 * Update the memeber data record and also update tournament registration record for the member data
		 */
		private bool importMemberEntry( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry, bool inTourReg, bool inNcwsa ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			
			try {
				if ( myProgressInfo != null ) {
					myProgressInfo.setProgessMsg( "Processing " + curMemberEntry.FirstName + " " + curMemberEntry.LastName );
					myProgressInfo.Show();
					myProgressInfo.Refresh();
				}

				String curTrickBoat = "", curJumpHeight = "0";
				curTrickBoat = (String)curImportMemberEntry["TrickBoat"];
				curJumpHeight = (String)curImportMemberEntry["JumpHeight"];
				if ( curJumpHeight.Length > 0 ) {
					try {
						Decimal tmpJumpHeight = Convert.ToDecimal( curJumpHeight );
						if ( tmpJumpHeight > 6 ) {
							tmpJumpHeight = tmpJumpHeight / 10;
							curJumpHeight = tmpJumpHeight.ToString( "#.#" );
						}
						if ( inNcwsa ) {
							if ( tmpJumpHeight < Convert.ToDecimal( "5.0" ) ) {
								if ( ( (String)curImportMemberEntry["EventJump"] ).Substring( 0, 1 ).ToUpper().Equals( "B" ) ) {
								} else {
									curJumpHeight = "5.0";
								}
							}
						}
					} catch {
						curJumpHeight = "0";
					}
				}

				if ( curImportMemberEntry.ContainsKey( "Prereg" ) ) {
					String curPrereg = (String)curImportMemberEntry["Prereg"];
					if ( curPrereg == null ) curPrereg = "";
					if ( curPrereg.Equals( "YES" ) ) {
						curMemberEntry.Note = "OLR";

					} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Length > 0 ) {
						curMemberEntry.Note = "Appointed Official";
					}

				} else if ( curImportMemberEntry.ContainsKey( "Note" ) ) {
					curMemberEntry.Note = (String)curImportMemberEntry["Note"];
				}

				if ( mySanctionNum.Substring( 2, 1 ).Equals( "U" ) ) {
					if ( curMemberEntry.Note.Length == 0 ) curMemberEntry.Note = "Team Template";
					try {
						curMemberEntry.Note += ", EventWaiver=" + (String)curImportMemberEntry["EventWaiver"];
					} catch {
						curMemberEntry.Note += ", EventWaiver=N/A";
					}
				}

				importMemberWithRatings( curImportMemberEntry, curMemberEntry );

				bool curReqstStatus = false;
				if ( curMemberEntry.AgeGroup.Equals( "OF" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;

				} else {
					if ( curMemberEntry.Team.ToUpper().Equals( "OFF" ) ) {
						curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountTourRegAdded++;
					}

					if ( curMemberEntry.EventGroupSlalom.Length > 1 ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Slalom", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountSlalomAdded++;
					}

					if ( curMemberEntry.EventGroupTrick.Length > 1 ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Trick", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountTrickAdded++;
					}

					if ( curMemberEntry.EventGroupJump.Length > 1 ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Jump", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountJumpAdded++;
					}

					if ( curMemberEntry.Team.Length > 0 ) {
						if ( !( inNcwsa ) ) {
							String[] curTeamHeaderCols = { "TeamHeader", curMemberEntry.Team, "", curMemberEntry.Team };
							//procTeamHeaderInput( curTeamHeaderCols, inNcwsa );
						}

					}
				}

				#region Analyze and process official ratings for member
				/*
				 * Mark officials that are indicated as the chief, assistant chief, or appointed official ratings
				*/
				if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "JudgeChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "ACJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "JudgeAsstChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "APTJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "JudgeAppointed" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "DriverChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "ACD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "DriverAsstChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "APTD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "DriverAppointed" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CC" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "ScoreChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "ACC" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "ScoreAsstChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "APTS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "ScoreAppointed" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "SafetyChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "ACS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "SafetyAsstChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CT" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "TechChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "ACT" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "TechAsstChief" );

				} else if ( ( (String)curImportMemberEntry["ApptdOfficial"] ).Equals( "CA" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry.MemberId, "AnncrChief" );
				}
				#endregion

				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "importMemberEntry: Error processing member"
					+ "\nSkier {0} {1} {2} {3}"
					+ "\nException: {4}"
					, curMemberEntry.MemberId, curMemberEntry.FirstName, curMemberEntry.LastName, curMemberEntry.AgeGroup, ex.Message ) );
				return false;
			}
		}

		private bool regSkierForEvent( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry, String curEvent
			, String curTrickBoat, String curJumpHeight ) {
			int curEventRoundsPaid = 0;
			String curEventClass = "";
			
			String curEventGroup = "";
			if ( curEvent.Equals( "Slalom" ) ) curEventGroup = curMemberEntry.EventGroupSlalom;
			else if ( curEvent.Equals( "Trick" ) ) curEventGroup = curMemberEntry.EventGroupTrick;
			else if ( curEvent.Equals( "Jump" ) ) curEventGroup = curMemberEntry.EventGroupJump;

			try {
				if ( curImportMemberEntry.ContainsKey( "EventClass" + curEvent ) ) {
					curEventClass = (String) curImportMemberEntry["EventClass" + curEvent];
				} else if ( curImportMemberEntry.ContainsKey( curEvent + "Paid" ) ) {
					curEventClass = (String) curImportMemberEntry[curEvent + "Paid"];
					if ( curEventClass.Trim().Length > 1 ) {
						curEventClass = curEventClass.ToString().Substring( 0, 1 );
						curEventRoundsPaid = int.Parse( ( (String) curImportMemberEntry[curEvent + "Paid"] ).ToString().Substring( 1 ) );
					}
				}
				if ( curEventGroup.Trim().Length == 0 ) return false;
				
				if ( curEventClass.Trim().Length > 1 ) curEventClass = curEventClass.Substring( 0, 1 );
				bool curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
				if ( curReqstStatus ) myCountTourRegAdded++;

				if ( curEventGroup.ToLower().Equals( "of" ) ) return false;

				bool returnStatus = false;
				if ( curEvent.Equals( "Slalom" ) ) returnStatus = myTourEventReg.addEventSlalom( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );
				if ( curEvent.Equals( "Trick" ) ) returnStatus = myTourEventReg.addEventTrick( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );
				if ( curEvent.Equals( "Jump" ) ) returnStatus = myTourEventReg.addEventJump( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );

				if ( returnStatus && curEventRoundsPaid > 0 && myTourTypePickAndChoose ) {
					returnStatus = myTourEventReg.addEventRunorder( curEvent, curMemberEntry.MemberId, curEventGroup, curEventClass, curEventRoundsPaid, curMemberEntry.AgeGroup );
				}
				return returnStatus;

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "importMemberEntry: Error processing member"
					+ "\nSkier {0} {1} {2} {3}"
					+ "\nException: {4}"
					, curMemberEntry.MemberId, curMemberEntry.FirstName, curMemberEntry.LastName, curMemberEntry.AgeGroup, ex.Message ) );
				return false;
			}
		}

		public void importMemberWithRatings( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry ) {
            bool newMember = false;
			DateTime lastRecModDate = new DateTime();
			StringBuilder curSqlStmt = new StringBuilder("");
			String curEvent = "";
			Decimal curSlalom = 0, curTrick = 0, curJump = 0, curOverall = 0;
			DataTable curDataTable;
			int rowsProc = 0;

			#region Retrieve all member attributes and prepare for processing
            if ( curImportMemberEntry["Federation"] != null ) {
				curMemberEntry.Federation = (String) curImportMemberEntry["Federation"];
            }

			if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
				curMemberEntry.Gender = (String) curImportMemberEntry["Gender"];
			} else if ( curMemberEntry.AgeGroup.Length > 1 ) {
				curMemberEntry.Gender = myTourEventReg.getGenderOfAgeDiv( curMemberEntry.AgeGroup );
			}

			curMemberEntry.City = (String) curImportMemberEntry["City"];
			curMemberEntry.State = (String) curImportMemberEntry["State"];

			if (curImportMemberEntry.ContainsKey("SkiYearAge")) {
				curMemberEntry.SkiYearAge = ((Int16)curImportMemberEntry["SkiYearAge"]);
			
			} else if (curImportMemberEntry.ContainsKey("Age")) {
				if (curImportMemberEntry["Age"].GetType() == System.Type.GetType("System.String")) {
					String tempSkiYearAge = (String)curImportMemberEntry["Age"];
					if ( tempSkiYearAge.Length > 0 ) {
						int numCk;
						if ( int.TryParse( tempSkiYearAge, out numCk ) ) {
							if ( numCk < 0 ) curMemberEntry.SkiYearAge = 0;
							else curMemberEntry.SkiYearAge = Convert.ToInt16( numCk );
						} else {
							curMemberEntry.SkiYearAge = 0;
						}
					} else {
						curMemberEntry.SkiYearAge = 0;
					}
				
				} else if (curImportMemberEntry["Age"].GetType() == System.Type.GetType("System.Int32")) {
					curMemberEntry.SkiYearAge = Convert.ToInt16((int)curImportMemberEntry["Age"]);
				}
			}

			if (curImportMemberEntry.ContainsKey("CanSki")) {
				if (curImportMemberEntry["CanSki"] != null) {
					if (curImportMemberEntry["CanSki"].GetType() == System.Type.GetType("System.String")) {
						if (((String)curImportMemberEntry["CanSki"]).ToLower().Equals("true")) curMemberEntry.CanSki = true;
					} else {
						curMemberEntry.CanSki = (Boolean)curImportMemberEntry["CanSki"];
					}
				}
			}
			if (curImportMemberEntry.ContainsKey("CanSkiGR")) {
				if (curImportMemberEntry["CanSkiGR"] != null) {
					if (curImportMemberEntry["CanSkiGR"].GetType() == System.Type.GetType("System.String")) {
						if (((String)curImportMemberEntry["CanSkiGR"]).ToLower().Equals("true")) curMemberEntry.CanSkiGR = true;
					} else {
						curMemberEntry.CanSkiGR = (Boolean)curImportMemberEntry["CanSkiGR"];
					}
				}
			}
			if (curImportMemberEntry.ContainsKey("Waiver")) {
				if (curImportMemberEntry["Waiver"].GetType() == System.Type.GetType("System.String")) {
					curMemberEntry.WaiverSigned = HelperFunctions.isValueTrue( (String)curImportMemberEntry["Waiver"]);
				} else {
					curMemberEntry.WaiverSigned = HelperFunctions.isValueTrue( ((int)curImportMemberEntry["Waiver"]).ToString() );
				}
			}

			//String MemTypeDesc = (String)curImportMemberEntry["MemTypeDesc"];
			curMemberEntry.MemberStatus = "In-Active";
			Boolean ActiveMember = false;
			String EffTo = (String)curImportMemberEntry["EffTo"];
			if ( curImportMemberEntry["ActiveMember"].GetType() == System.Type.GetType( "System.Boolean" ) ) {
				ActiveMember = (Boolean)curImportMemberEntry["ActiveMember"];
			} else if ( curImportMemberEntry["ActiveMember"].GetType() == System.Type.GetType( "System.String" ) ) {
				String curValue = (String)curImportMemberEntry["ActiveMember"];
				if ( curValue.ToLower().Equals( "true" ) ) ActiveMember = true;
			}
			if ( ActiveMember ) curMemberEntry.MemberStatus = "Active";
			try {
				DateTime curEffToDate = Convert.ToDateTime(EffTo);
				EffTo = curEffToDate.ToString("MM/dd/yy");

				DateTime curTourDate = Convert.ToDateTime(myTourRow["EventDates"]);
				if ((curEffToDate >= curTourDate) && curMemberEntry.CanSki ) {
					curMemberEntry.MemberStatus = "Active";
					if (curMemberEntry.WaiverSigned.Equals("0") ) curMemberEntry.MemberStatus += " ** Needs Annual Waiver";
					if ( curMemberEntry.CanSkiGR ) curMemberEntry.MemberStatus += " ** Grass Roots Only";
					if ( !curMemberEntry.CanSki ) curMemberEntry.MemberStatus = "Needs Upgrade";

				} else {
					if ( curMemberEntry.CanSki ) {
						curMemberEntry.MemberStatus = "Needs Renew";
					} else {
						curMemberEntry.MemberStatus = "Needs Renew/Upgrade";
					}
				}

			} catch {
				curMemberEntry.MemberStatus = (String)curImportMemberEntry["ActiveMember"];
			}

			curMemberEntry.JudgeSlalomRating = (String)curImportMemberEntry["JudgeSlalom"];
			if ( curImportMemberEntry.Keys.Contains("JudgePanAmSlalom")) {
				if ( ( (String) curImportMemberEntry["JudgePanAmSlalom"] ).ToLower().Equals( "int" ) ) {
					curMemberEntry.JudgeSlalomRating = "PanAm";
				}
			}
			curMemberEntry.JudgeTrickRating = (String) curImportMemberEntry["JudgeTrick"];
			if ( curImportMemberEntry.Keys.Contains( "JudgePanAmTrick") ) {
				if ( ( (String) curImportMemberEntry["JudgePanAmTrick"] ).ToLower().Equals( "int" ) ) {
					curMemberEntry.JudgeTrickRating = "PanAm";
				}
			}
			curMemberEntry.JudgeJumpRating = (String) curImportMemberEntry["JudgeJump"];
			if ( curImportMemberEntry.Keys.Contains( "JudgePanAmJump" ) ) {
				if ( ( (String) curImportMemberEntry["JudgePanAmJump"] ).ToLower().Equals( "int" ) ) {
					curMemberEntry.JudgeJumpRating = "PanAm";
				}
			}

			curMemberEntry.DriverSlalomRating = (String) curImportMemberEntry["DriverSlalom"];
			curMemberEntry.DriverTrickRating = (String) curImportMemberEntry["DriverTrick"];
			curMemberEntry.DriverJumpRating = (String) curImportMemberEntry["DriverJump"];

			curMemberEntry.ScorerSlalomRating = (String) curImportMemberEntry["ScorerSlalom"];
			curMemberEntry.ScorerTrickRating = (String) curImportMemberEntry["ScorerTrick"];
			curMemberEntry.ScorerJumpRating = (String) curImportMemberEntry["ScorerJump"];

			curMemberEntry.SafetyOfficialRating = (String) curImportMemberEntry["Safety"];
			curMemberEntry.TechControllerRating = (String) curImportMemberEntry["TechController"];

			curSqlStmt = new StringBuilder( "Select MemberId, UpdateDate from MemberList Where MemberId = '" + curMemberEntry.MemberId + "'");
            DataTable curMemberDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
			if ( curMemberDataTable.Rows.Count > 0 ) {
				lastRecModDate = (DateTime) curMemberDataTable.Rows[0]["UpdateDate"];
				newMember = false;
			} else { 
                newMember = true;
            }
			#endregion

			if ( newMember ) {
                #region Insert new member to databse
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Insert MemberList (");
                curSqlStmt.Append("MemberId, LastName, FirstName, SkiYearAge, Gender, City, State, Federation, MemberStatus");
                curSqlStmt.Append(", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating");
                curSqlStmt.Append(", DriverSlalomRating, DriverTrickRating, DriverJumpRating");
                curSqlStmt.Append(", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating");
                curSqlStmt.Append(", SafetyOfficialRating, TechOfficialRating");
                curSqlStmt.Append(", Note, MemberExpireDate, InsertDate, UpdateDate");
                curSqlStmt.Append(") Values (");
                curSqlStmt.Append("'" + curMemberEntry.MemberId + "'");
                curSqlStmt.Append(", '" + curMemberEntry.getLastNameForDB() + "'");
                curSqlStmt.Append(", '" + curMemberEntry.getFirstNameForDB() + "'");
                curSqlStmt.Append(", " + curMemberEntry.SkiYearAge );
                curSqlStmt.Append(", '" + curMemberEntry.Gender + "'");
                curSqlStmt.Append(", '" + curMemberEntry.getCityForDB() + "'");
                curSqlStmt.Append(", '" + curMemberEntry.State + "'");
                curSqlStmt.Append(", '" + curMemberEntry.Federation + "'");
                curSqlStmt.Append(", '" + curMemberEntry.MemberStatus + "'");
                curSqlStmt.Append(", '" + curMemberEntry.JudgeSlalomRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.JudgeTrickRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.JudgeJumpRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.DriverSlalomRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.DriverTrickRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.DriverJumpRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.ScorerSlalomRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.ScorerTrickRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.ScorerJumpRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.SafetyOfficialRating + "'");
                curSqlStmt.Append(", '" + curMemberEntry.TechControllerRating + "'");
				curSqlStmt.Append( ", '" + curMemberEntry.Note + "'" );
                curSqlStmt.Append(", '" + EffTo + "'");
                curSqlStmt.Append(", getdate(), getdate() )");

                rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                if ( rowsProc > 0 ) myCountMemberAdded++;
                #endregion

            } else {
                #region Update member data
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Update MemberList ");
                curSqlStmt.Append(" Set LastName = '" + curMemberEntry.getLastNameForDB() + "'");
                curSqlStmt.Append(", FirstName = '" + curMemberEntry.getFirstNameForDB() + "'");
                curSqlStmt.Append(", SkiYearAge = " + curMemberEntry.SkiYearAge );
                curSqlStmt.Append(", Gender = '" + curMemberEntry.Gender + "'");
                curSqlStmt.Append(", City = '" + curMemberEntry.getCityForDB() + "'");
                curSqlStmt.Append(", State = '" + curMemberEntry.State + "'");
                curSqlStmt.Append(", Federation = '" + curMemberEntry.Federation + "'");
                curSqlStmt.Append(", MemberStatus = '" + curMemberEntry.MemberStatus + "'");
                curSqlStmt.Append(", Note = '" + curMemberEntry.Note +"'");
                curSqlStmt.Append(", MemberExpireDate = '" + EffTo + "'");
                curSqlStmt.Append(", UpdateDate = getdate()");
                curSqlStmt.Append(", JudgeSlalomRating = '" + curMemberEntry.JudgeSlalomRating + "'");
                curSqlStmt.Append(", JudgeTrickRating = '" + curMemberEntry.JudgeTrickRating + "'");
                curSqlStmt.Append(", JudgeJumpRating = '" + curMemberEntry.JudgeJumpRating + "'");
                curSqlStmt.Append(", DriverSlalomRating = '" + curMemberEntry.DriverSlalomRating + "'");
                curSqlStmt.Append(", DriverTrickRating = '" + curMemberEntry.DriverTrickRating + "'");
                curSqlStmt.Append(", DriverJumpRating = '" + curMemberEntry.DriverJumpRating + "'");
                curSqlStmt.Append(", ScorerSlalomRating = '" + curMemberEntry.ScorerSlalomRating + "'");
                curSqlStmt.Append(", ScorerTrickRating = '" + curMemberEntry.ScorerTrickRating + "'");
                curSqlStmt.Append(", ScorerJumpRating = '" + curMemberEntry.ScorerJumpRating + "'");
                curSqlStmt.Append(", SafetyOfficialRating = '" + curMemberEntry.SafetyOfficialRating + "'");
                curSqlStmt.Append(", TechOfficialRating = '" + curMemberEntry.TechControllerRating + "'");
                curSqlStmt.Append(" Where MemberId = '" + curMemberEntry.MemberId + "'");
                rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                if ( rowsProc > 0 ) myCountMemberUpdated++;
                #endregion
            }

			#region Insert or update skier slalom ranking data
			if ( curImportMemberEntry.Keys.Contains( "SlalomRank" ) ) {
				if ( curImportMemberEntry["SlalomRank"].GetType() == System.Type.GetType( "System.String" ) ) {
					Decimal.TryParse( (String) curImportMemberEntry["SlalomRank"], out curSlalom );
				} else if ( curImportMemberEntry["SlalomRank"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
					curSlalom = (Decimal) curImportMemberEntry["SlalomRank"];
				}
			}
			if ( curSlalom > 0 ) {
				curEvent = "Slalom";
				curOverall += curSlalom;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select PK from SkierRanking " );
				curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curSlalom.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["SlalomRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curSlalom.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["SlalomRating"] + "'" );
					curSqlStmt.Append( ", '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}

				/*
				 * Update slalom ranking score for member in current tournament if previously added
				 * If it doesn't exist then it will be added when the skier is registered for the event
				 */
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update EventReg " );
				curSqlStmt.Append( " Set RankingScore = " + curSlalom.ToString() );
				curSqlStmt.Append( ", RankingRating = '" + (String) curImportMemberEntry["SlalomRating"] + "'" );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
				curSqlStmt.Append( "   And MemberId = '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "   And Event = '" + curEvent + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Insert or update skier trick ranking data
			if ( curImportMemberEntry.Keys.Contains( "TrickRank" ) ) {
				if ( curImportMemberEntry["TrickRank"].GetType() == System.Type.GetType( "System.String" ) ) {
					Decimal.TryParse( (String) curImportMemberEntry["TrickRank"], out curTrick );
				} else if ( curImportMemberEntry["TrickRank"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
					curTrick = (Decimal) curImportMemberEntry["TrickRank"];
				}
			}
			if ( curTrick > 0 ) {
				curEvent = "Trick";
				curOverall += curTrick;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select PK from SkierRanking " );
				curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curTrick.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["TrickRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curTrick.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["TrickRating"] + "'" );
					curSqlStmt.Append( ", '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}

				/*
				 * Update Trick ranking score for member in current tournament if previously added
				 * If it doesn't exist then it will be added when the skier is registered for the event
				 */
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update EventReg " );
				curSqlStmt.Append( " Set RankingScore = " + curTrick.ToString() );
				curSqlStmt.Append( ", RankingRating = '" + (String) curImportMemberEntry["TrickRating"] + "'" );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
				curSqlStmt.Append( "   And MemberId = '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "   And Event = '" + curEvent + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Insert or update skier jump ranking data
			if ( curImportMemberEntry.Keys.Contains( "JumpRank" ) ) {
				if ( curImportMemberEntry["JumpRank"].GetType() == System.Type.GetType( "System.String" ) ) {
					Decimal.TryParse( (String) curImportMemberEntry["JumpRank"], out curJump );
				} else if ( curImportMemberEntry["JumpRank"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
					curJump = (Decimal) curImportMemberEntry["JumpRank"];
				}
			}
			if ( curJump > 0 ) {
				curEvent = "Jump";
				curOverall += curJump;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select PK from SkierRanking " );
				curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curJump.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["JumpRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberEntry.MemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curJump.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["JumpRating"] + "'" );
					curSqlStmt.Append( ", '" + curMemberEntry.AgeGroup + "'" );
					curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}

				/*
				 * Update Jump ranking score for member in current tournament if previously added
				 * If it doesn't exist then it will be added when the skier is registered for the event
				 */
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update EventReg " );
				curSqlStmt.Append( " Set RankingScore = " + curJump.ToString() );
				curSqlStmt.Append( ", RankingRating = '" + (String) curImportMemberEntry["JumpRating"] + "'" );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
				curSqlStmt.Append( "   And MemberId = '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "   And Event = '" + curEvent + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Update tournament registration entry if one exists in current tournament for this member
			String curSanctionId = (String) myTourRow["SanctionId"];
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select TR.MemberId as TourMemberId, OW.MemberId as OfficialMemberId " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "LEFT OUTER JOIN OfficialWork OW ON TR.SanctionId = OW.SanctionId AND TR.MemberId = OW.MemberId " );
			curSqlStmt.Append( String.Format( "Where TR.MemberId = '{0}' AND TR.SanctionId = '{1}'", curMemberEntry.MemberId, curSanctionId ) );
			curMemberDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curMemberDataTable.Rows.Count > 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TourReg Set " );
				curSqlStmt.Append( String.Format( "SkierName = '{0}'", curMemberEntry.getSkierNameForDB() ) );
				curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", AwsaMbrshpComment = '" + curMemberEntry.MemberStatus + "'" );
				
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
				curSqlStmt.Append( ", LastUpdateDate = getdate() " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, curMemberEntry.MemberId ) );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) myCountMemberTourUpdated++;

				if ( curMemberDataTable.Rows[0]["OfficialMemberId"] != null ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update OfficialWork Set " );
					curSqlStmt.Append( "JudgeSlalomRating = '" + curMemberEntry.JudgeSlalomRating + "'" );
					curSqlStmt.Append( ", JudgeTrickRating = '" + curMemberEntry.JudgeTrickRating + "'" );
					curSqlStmt.Append( ", JudgeJumpRating = '" + curMemberEntry.JudgeJumpRating + "'" );
					curSqlStmt.Append( ", DriverSlalomRating = '" + curMemberEntry.DriverSlalomRating + "'" );
					curSqlStmt.Append( ", DriverTrickRating = '" + curMemberEntry.DriverTrickRating + "'" );
					curSqlStmt.Append( ", DriverJumpRating = '" + curMemberEntry.DriverJumpRating + "'" );
					curSqlStmt.Append( ", ScorerSlalomRating = '" + curMemberEntry.ScorerSlalomRating + "'" );
					curSqlStmt.Append( ", ScorerTrickRating = '" + curMemberEntry.ScorerTrickRating + "'" );
					curSqlStmt.Append( ", ScorerJumpRating = '" + curMemberEntry.ScorerJumpRating + "'" );
					curSqlStmt.Append( ", SafetyOfficialRating = '" + curMemberEntry.SafetyOfficialRating + "'" );
					curSqlStmt.Append( ", TechOfficialRating = '" + curMemberEntry.TechControllerRating + "'" );
					curSqlStmt.Append( ", LastUpdateDate = getdate() " );
					curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, curMemberEntry.MemberId ) );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					if ( rowsProc > 0 ) myCountMemberTourOfficialUpdated++;

				}
			}
			#endregion
		}

		public bool procTeamHeaderInput( Dictionary<string, object> curImportMemberEntry, bool inNcwsa ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );

			String curTeamName = (String) curImportMemberEntry["TeamName"];
			String curTeamCode = (String) curImportMemberEntry["TeamCode"];
			String curAgeGroup = (String) curImportMemberEntry["AgeGroup"];
			String curEventGroup = (String) curImportMemberEntry["EventGroup"];

			int curTeamSlalomRunOrder = 1, curTeamTrickRunOrder = 1, curTeamJumpRunOrder = 1;

			if ( curTeamCode.ToLower().Equals( "off" ) ) return false;

			curTeamSlalomRunOrder = (int)curImportMemberEntry["SlalomRunOrder"];
			curTeamTrickRunOrder = (int) curImportMemberEntry["TrickRunOrder"];
			curTeamJumpRunOrder = (int) curImportMemberEntry["JumpRunOrder"];

			#region Insert or update team information 
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK from TeamList " );
			curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curDataTable.Rows.Count > 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TeamList " );
				curSqlStmt.Append( "Set Name = '" + curTeamName + "'" );
				curSqlStmt.Append( ", LastUpdateDate = getdate() " );
				curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
				curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );

			} else {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert TeamList (" );
				curSqlStmt.Append( "SanctionId, Name, TeamCode, LastUpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + curTeamName + "'" );
				curSqlStmt.Append( ", '" + curTeamCode + "'" );
				curSqlStmt.Append( ", getdate()" );
                curSqlStmt.Append( ")");

			}
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			#region Insert or update team order information (qualify by division or group if provided )
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK from TeamOrder " );
			curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
			if ( inNcwsa ) {
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
			} else {
				if ( curAgeGroup.Length > 0 ) {
					curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
				}
				if ( curEventGroup.Length > 0 ) {
					curSqlStmt.Append( "  And EventGroup = '" + curEventGroup + "' " );
				}
			}
			curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TeamOrder " );
				curSqlStmt.Append( "Set SlalomRunOrder = " + curTeamSlalomRunOrder );
				curSqlStmt.Append( ", TrickRunOrder = " + curTeamTrickRunOrder );
				curSqlStmt.Append( ", JumpRunOrder = " + curTeamJumpRunOrder );
				curSqlStmt.Append( ", LastUpdateDate = getdate() " );
				curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
				curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
				if ( inNcwsa ) {
					curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
				} else {
					if ( curAgeGroup.Length > 0 ) {
						curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
					}
					if ( curEventGroup.Length > 0 ) {
						curSqlStmt.Append( "  And EventGroup = '" + curEventGroup + "' " );
					}
				}

			} else {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert TeamOrder (" );
				curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + curTeamCode + "'" );
				curSqlStmt.Append( ", '" + curAgeGroup + "'" );
				curSqlStmt.Append( ", '" + curEventGroup + "'" );
				curSqlStmt.Append( ", " + curTeamSlalomRunOrder );
				curSqlStmt.Append( ", " + curTeamTrickRunOrder );
				curSqlStmt.Append( ", " + curTeamJumpRunOrder );
				curSqlStmt.Append( ", getdate()" );
				curSqlStmt.Append( ")" );

			}
			rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			#endregion

			return true;
		}

		public bool importNcwsMemberFromAwsa( Dictionary<string, object> curImportMemberEntry ) {
			String curTeamSlalom = "", curTeamTrick = "", curTeamJump = "";
			MemberEntry curMemberEntry = new MemberEntry();
			bool curDataValid = true;

			if ( curImportMemberEntry.ContainsKey( "MemberID" ) ) {
				curMemberEntry.MemberId = (String) curImportMemberEntry["MemberID"];
			} else {
				curMemberEntry.MemberId = (String) curImportMemberEntry["MemberId"];
			}
			if ( !(myMemberIdValidate.checkMemberId( curMemberEntry.MemberId )) ) {
				MessageBox.Show( String.Format("Invalid member id {0}, checksum validation failed", curMemberEntry.MemberId ) );
				return false;
			}

			if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
				curMemberEntry.AgeGroup = ( (String) curImportMemberEntry["AgeGroup"] ).ToUpper();
			} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
				curMemberEntry.AgeGroup = ( (String) curImportMemberEntry["Div"] ).ToUpper();
			}
			String curTeam = ( (String) curImportMemberEntry["Team"] ).ToUpper().Trim();

			curMemberEntry.FirstName = (String)curImportMemberEntry[ "FirstName"];
			curMemberEntry.LastName = (String)curImportMemberEntry[ "LastName"];
			
			if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
				curMemberEntry.Gender = (String)curImportMemberEntry["Gender"];
			} else if ( curMemberEntry.AgeGroup.Length > 1 ) {
				curMemberEntry.Gender = myTourEventReg.getGenderOfAgeDiv( curMemberEntry.AgeGroup );
			}

			if ( curImportMemberEntry.ContainsKey( "Team" ) ) {
				curMemberEntry.Team = ( (String)curImportMemberEntry["Team"] ).ToUpper().Trim();
			}

			//For collegiate divisions determine if data is valid for collegiate tournaments
			curMemberEntry.EventGroupSlalom = ( (String)curImportMemberEntry["EventSlalom"] ).Trim();
			if ( curMemberEntry.EventGroupSlalom.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupSlalom = "";
			curMemberEntry.EventGroupTrick = ( (String)curImportMemberEntry["EventTrick"] ).Trim();
			if ( curMemberEntry.EventGroupTrick.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupTrick = "";
			curMemberEntry.EventGroupJump = ( (String)curImportMemberEntry["EventJump"] ).Trim();
			if ( curMemberEntry.EventGroupJump.ToLower().Equals( "of" ) ) curMemberEntry.EventGroupJump = "";

			/*
			 * If a member entry is assigned a group code for any event then assuming this is a non skiing official 
			 */
			if ( curMemberEntry.EventGroupSlalom.Length == 0
				&& curMemberEntry.EventGroupTrick.Length == 0
				&& curMemberEntry.EventGroupJump.Length == 0
				) {
				curMemberEntry.Note = "Official";
				importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
				return curDataValid;
			}

			if ( curMemberEntry.AgeGroup.ToUpper().Equals( "CM" )
				|| curMemberEntry.AgeGroup.ToUpper().Equals( "CW" )
				|| curMemberEntry.AgeGroup.ToUpper().Equals( "BM" )
				|| curMemberEntry.AgeGroup.ToUpper().Equals( "BW" )
				) {
				
				// Validate event groups for collegiate divisions which must include rotation sequence
				curDataValid = isValidNcwsaEventGroup( "Slalom", curMemberEntry.EventGroupSlalom, curMemberEntry );
				if ( curDataValid && curMemberEntry.EventGroupSlalom.Length > 1 ) curTeamSlalom = curMemberEntry.EventGroupSlalom.Substring( 0, 1 ).ToUpper();

				curDataValid = isValidNcwsaEventGroup( "Trick", curMemberEntry.EventGroupTrick, curMemberEntry );
				if ( curDataValid && curMemberEntry.EventGroupTrick.Length > 1 ) curTeamTrick = curMemberEntry.EventGroupTrick.Substring( 0, 1 ).ToUpper();

				curDataValid = isValidNcwsaEventGroup( "Jump", curMemberEntry.EventGroupJump, curMemberEntry );
				if ( curDataValid && curMemberEntry.EventGroupJump.Length > 1 ) curTeamJump = curMemberEntry.EventGroupJump.Substring( 0, 1 ).ToUpper();

				if ( !(curDataValid) ) return curDataValid;
				// Collegiate skier is not registered for any events, treat as an error because it shouldn't get to this point
				if ( curTeamSlalom.Length == 0 && curTeamSlalom.Length == 0 && curTeamJump.Length == 0 ) return false;

				#region Check data to determine team assignments (A or B)

				// Check if skier registered for slalom only
				if ( curTeamSlalom.Length > 0 && curTeamTrick.Length == 0 && curTeamJump.Length == 0 ) {
					if ( curTeamSlalom.Equals( "B" ) ) {
						if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
						else curMemberEntry.AgeGroup = "BW";
					}

					importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
					return curDataValid;
				}

				// Check if skier registered for trick only
				if ( curTeamSlalom.Length == 0 && curTeamTrick.Length > 0 && curTeamJump.Length == 0 ) {
					if ( curTeamTrick.Equals( "B" ) ) {
						if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
						else curMemberEntry.AgeGroup = "BW";
					}

					importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
					return curDataValid;
				}

				// Check if skier registered for jump only
				if ( curTeamSlalom.Length == 0 && curTeamTrick.Length == 0 && curTeamJump.Length > 0 ) {
					if ( curTeamJump.Equals( "B" ) ) {
						if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
						else curMemberEntry.AgeGroup = "BW";
					}
					
					importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
					return curDataValid;
				}

				// Check if skier is registered in all 3 events
				if ( curTeamSlalom.Length > 0 && curTeamTrick.Length > 0 && curTeamJump.Length > 0 ) {
					if ( curTeamSlalom.Equals( "A" ) && curTeamTrick.Equals( "A" ) && curTeamJump.Equals( "A" ) ) {
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
					
					} else if ( curTeamSlalom.Equals( "B" ) && curTeamTrick.Equals( "B" ) && curTeamJump.Equals( "B" ) ) {
						if ( curTeamSlalom.Equals( "B" ) ) {
							if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
							else curMemberEntry.AgeGroup = "BW";
						}
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
					
					} else {
						createSplitTeamEntries( curImportMemberEntry, curMemberEntry, curTeamSlalom, curTeamTrick, curTeamJump );
					}
					return curDataValid;
				}

				// Check if skier is registered for slalom and trick only 
				if ( curTeamSlalom.Length > 0 && curTeamTrick.Length > 0 && curTeamJump.Length == 0 ) {
					if ( curTeamSlalom.Equals( "A" ) && curTeamTrick.Equals( "A" ) ) {
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else if ( curTeamSlalom.Equals( "B" ) && curTeamTrick.Equals( "B" ) ) {
						if ( curTeamSlalom.Equals( "B" ) ) {
							if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
							else curMemberEntry.AgeGroup = "BW";
						}
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else {
						createSplitTeamEntries( curImportMemberEntry, curMemberEntry, curTeamSlalom, curTeamTrick, curTeamJump );
					}
					return curDataValid;
				}

				// Check if skier is registered for slalom and jump only 
				if ( curTeamSlalom.Length > 0 && curTeamJump.Length > 0 ) {
					if ( curTeamSlalom.Equals( "A" ) && curTeamJump.Equals( "A" ) ) {
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else if ( curTeamSlalom.Equals( "B" ) && curTeamJump.Equals( "B" ) ) {
						if ( curTeamSlalom.Equals( "B" ) ) {
							if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
							else curMemberEntry.AgeGroup = "BW";
						}
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else {
						createSplitTeamEntries( curImportMemberEntry, curMemberEntry, curTeamSlalom, curTeamTrick, curTeamJump );
					}
					return curDataValid;
				}

				// Check if skier is registered for trick and jump only 
				if ( curTeamTrick.Length > 0 && curTeamJump.Length > 0 ) {
					if ( curTeamTrick.Equals( "A" ) && curTeamJump.Equals( "A" ) ) {
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else if ( curTeamTrick.Equals( "B" ) && curTeamJump.Equals( "B" ) ) {
						if ( curTeamTrick.Equals( "B" ) ) {
							if ( curMemberEntry.Gender.Equals( "M" ) ) curMemberEntry.AgeGroup = "BM";
							else curMemberEntry.AgeGroup = "BW";
						}
						importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );

					} else {
						createSplitTeamEntries( curImportMemberEntry, curMemberEntry, curTeamSlalom, curTeamTrick, curTeamJump );
					}
					return curDataValid;
				}

				#endregion

			} else {
				// Check group to validate for appropriate skiing officials
				if ( curMemberEntry.EventGroupSlalom.Length > 0 ) curMemberEntry.Team = "OFF";
				if ( curMemberEntry.EventGroupTrick.Length > 0 ) curMemberEntry.Team = "OFF";
				if ( curMemberEntry.EventGroupJump.Length > 0 ) curMemberEntry.Team = "OFF";
				importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
			}

			return curDataValid;
		}

		/*
		 * Check for rotations for collegiate divisions
		 * Check event group to validate for appropriate collegiate values
		 */
		private bool isValidNcwsaEventGroup( String inEvent, String inEventGroup, MemberEntry inMemberEntry ) {
			Int16 numIntCk;

			if ( inEventGroup.Length == 1 ) {
				showNcwsaEventGroupErrorMsg( "Slalom", inEventGroup, inMemberEntry );
				return false;
			}

			if ( inEventGroup.Length > 1 ) {
				if ( inEventGroup.Substring( 0, 1 ).ToUpper().Equals( "A" )
					|| inEventGroup.Substring( 0, 1 ).ToUpper().Equals( "B" )
					) {
					if ( Int16.TryParse( inEventGroup.Substring( 1 ), out numIntCk ) ) {
					} else {
						showNcwsaEventGroupErrorMsg( inEvent, inEventGroup, inMemberEntry );
						return false;
					}

				} else {
					showNcwsaEventGroupErrorMsg( inEvent, inEventGroup, inMemberEntry );
					return false;
				}
			}

			return true;
		}

		private void showNcwsaEventGroupErrorMsg( String inEvent, String inEventGroup, MemberEntry inMemberEntry ) {
			MessageBox.Show( String.Format( "For Skier {0} {1} {2} {3}"
				+ "\n{4} event group {5} is not valid."
				+ "\nIt must start with the letter A or B followed by a numeric rotation number"
				, inMemberEntry.MemberId, inMemberEntry.FirstName, inMemberEntry.LastName, inMemberEntry.AgeGroup, inEvent, inEventGroup ) );
		}

		private void buildNcwsaTeamHeader( Dictionary<string, object> curEntry, Dictionary<string, object> curTeamHeaderList ) {
			String curTeam = ( (String)curEntry["Team"] ).ToUpper().Trim();
			String curTeamName = ( (String)curEntry["TeamName"] ).Trim();
			String curDiv = ( (String)curEntry["Div"] ).ToUpper();
			String curTeamKey = curTeam + "-" + curDiv;

			if ( curTeam.Length > 0 ) {
				if ( !( curTeamHeaderList.ContainsKey( curTeamKey ) ) ) {
					Dictionary<string, object> curTeamEntry = new Dictionary<string, object>();
					curTeamEntry.Add( "TeamName", curTeamName );
					curTeamEntry.Add( "TeamCode", curTeam );

					curTeamEntry.Add( "AgeGroup", curDiv );
					curTeamEntry.Add( "EventGroup", "" );

					curTeamEntry.Add( "SlalomRunOrder", 0 );
					curTeamEntry.Add( "TrickRunOrder", 0 );
					curTeamEntry.Add( "JumpRunOrder", 0 );

					curTeamHeaderList.Add( curTeamKey, curTeamEntry );
				}
			}
		}

		private void createSplitTeamEntries( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry, String curTeamSlalom, String curTeamTrick, String curTeamJump ) {
			//Clone current attributes to create a second B team entry
			Dictionary<string, object> curImportMemberEntryTeamB = new Dictionary<string, object>();
			foreach ( KeyValuePair<string, object> curEntry in curImportMemberEntry ) {
				curImportMemberEntryTeamB.Add( curEntry.Key, curEntry.Value );
			}

			//Type entryType = 
			MemberEntry curMemberEntryTeamB = new MemberEntry();
			PropertyInfo[] propList = curMemberEntry.GetType().GetProperties();
			foreach ( PropertyInfo curProp in propList ) {
				//object curValue = curProp.GetValue( curMemberEntry );
				PropertyInfo copyProp = curMemberEntryTeamB.GetType().GetProperty( curProp.Name );
				curMemberEntryTeamB.GetType().GetProperty( curProp.Name ).SetValue( curMemberEntryTeamB, curProp.GetValue( curMemberEntry ) );
			}

			String curAgeGroupTeamB = "";
			if ( curMemberEntry.AgeGroup.Equals( "CM" ) || curMemberEntry.AgeGroup.Equals( "BM" ) ) {
				curAgeGroupTeamB = "BM";
			} else {
				curAgeGroupTeamB = "BW";
			}

			curMemberEntryTeamB.AgeGroup = curAgeGroupTeamB;

			//Remove B team assignments for current row and only process A team assignments
			if ( curTeamSlalom.Equals( "B" ) ) {
				curImportMemberEntry["EventSlalom"] = "";
				curMemberEntry.EventGroupSlalom = "";
			
			} else if ( curTeamSlalom.Equals( "A" ) ) {
				curImportMemberEntryTeamB["EventSlalom"] = "";
				curMemberEntryTeamB.EventGroupSlalom = "";
			}
			
			if ( curTeamTrick.Equals( "B" ) ) {
				curImportMemberEntry["EventTrick"] = "";
				curMemberEntry.EventGroupTrick = "";

			} else if ( curTeamTrick.Equals( "A" ) ) {
				curImportMemberEntryTeamB["EventTrick"] = "";
				curMemberEntryTeamB.EventGroupTrick = "";
			}

			if ( curTeamJump.Equals( "B" ) ) {
				curImportMemberEntry["EventJump"] = "";
				curMemberEntry.EventGroupJump = "";

			} else if ( curTeamJump.Equals( "A" ) ) {
				curImportMemberEntryTeamB["EventJump"] = "";
				curMemberEntryTeamB.EventGroupJump = "";
			}

			importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
			importMemberEntry( curImportMemberEntryTeamB, curMemberEntryTeamB, true, true );
		}

		private DataRow getTourData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, Name, Class, Federation" );
			curSqlStmt.Append( ", SanctionEditCode, Rules, EventDates, EventLocation, TourDataLoc " );
			curSqlStmt.Append( "FROM Tournament " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

	}
}
