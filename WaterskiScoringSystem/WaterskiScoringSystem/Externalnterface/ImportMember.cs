﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
	class ImportMember {
		private bool myTourTypePickAndChoose = false;

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

		private ProgressWindow myProgressInfo = null;
		private MemberIdValidate myMemberIdValidate;
		private TourEventReg myTourEventReg;

		private static DataRow myTourRow = null;
		private static String mySanctionNum;
		private static DateTime myTourEventDate = new DateTime();
		public static int MembershipStatusCodeActive = 1;
		public static string MembershipStatusTextActive = "Active";
		public static string MembershipStatusNone = "No Membership Status";

		private static Dictionary<int, string> myMembershipStatus = new Dictionary<int, string> {
			{ 0, "No Membership Status" }
			, { 1, "Active" }
			, { 2, "Expired" }
			, { 3, "Waiting - Safe Sport" }
			, { 4, "Pending - Safe Sport Course Required" }
			, { 5, "Pending - Signed Waiver" }
			, { 6, "Pending - Safe Sport & Signed Waiver" }
			, { 7, "Suspended" }
			, { 8, "Deceased" }
		};

		public static string getMembershipStatus( int inStatusCode ) {
			if ( myMembershipStatus.ContainsKey( inStatusCode ) ) return myMembershipStatus[inStatusCode];
			return MembershipStatusNone;
		}

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
				myTourRow = getTourData();

			} else {
				myTourRow = inTourRow;
				mySanctionNum = (String)inTourRow["SanctionId"];
				DateTime.TryParse( HelperFunctions.getDataRowColValue( myTourRow, "EventDates", "" ), out myTourEventDate );
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
			String curContentType = "application/json; charset=UTF-8";
			String curRegExportListUrl = Properties.Settings.Default.UriUsaWaterskiProd + "/admin/GetMemberRegExportJson.asp";
			String curReqstUrl = curRegExportListUrl;
			String curSanctionEditCode = (String)myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve skier registrations and member information.  Enter required value on Tournament Form" );
				return;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			IwwfMembership.warnMessageActive = false;
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList != null && curResponseDataList.Count > 0 ) {
				myProgressInfo = new ProgressWindow();
				myProgressInfo.setProgressMin( 1 );
				myProgressInfo.setProgressMax( curResponseDataList.Count );

				if ( HelperFunctions.isCollegiateSanction( mySanctionNum ) ) {
					Dictionary<String, object> curTeamHeaderList = new Dictionary<String, object>();
					foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
						myCountMemberInput++;
						myProgressInfo.setProgressValue( myCountMemberInput );

						importMemberFromAwsa( curEntry, true, true );

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
			}

			importTourAssignedOfficials();

			IwwfMembership.showBulkWarnMessage();
			IwwfMembership.warnMessageActive = true;

			displayMemberProcessCounts();
		}

		private void importTourAssignedOfficials() {
			/* -----------------------------------------------------------------------
            * Configure URL to retrieve all skiers pre-registered for the active tournament
			* This will include all appointed officials
            ----------------------------------------------------------------------- */
			String curContentType = "application/json; charset=UTF-8";
			String curRegExportListUrl = Properties.Settings.Default.UriUsaWaterskiProd + "/admin/GetTourAssignedOfficialsJson.asp";
			String curReqstUrl = curRegExportListUrl;
			String curSanctionEditCode = (String)myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve skier registrations and member information.  Enter required value on Tournament Form" );
				return;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList == null || curResponseDataList.Count == 0 ) {
				displayMemberProcessCounts();
				return;
			}

			myProgressInfo = new ProgressWindow();
			myProgressInfo.setProgressMin( myCountMemberInput );
			myProgressInfo.setProgressMax( myCountMemberInput + curResponseDataList.Count );

			IwwfMembership.warnMessageActive = false;
			foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
				myCountMemberInput++;
				myProgressInfo.setProgressValue( myCountMemberInput );

				importMemberFromAwsa( curEntry, true, false );
			}

			Cursor.Current = Cursors.Default;
			myProgressInfo.Close();
		}

		public void importWwsRegistrations() {
			/* -----------------------------------------------------------------------
            * Configure URL to retrieve all skiers pre-registered for the active tournament
			* This will include all appointed officials
			* https://global.api.worldwaterskiers.com/wstims/getParticipantsBySanctionID?apiToken=wstims&sanctionID=22S085R
			* ----------------------------------------------------------------------- 
			*/
			if ( !( mySanctionNum.Equals( "22S085" ) ) ) {
				MessageBox.Show( "This test can only be done using sanction 22S085" );
				return;
			}
			//String curQueryString = "&SanctionID=" + mySanctionNum;
			//String curQueryString = "&sanctionID=22S085R";
			String curContentType = "application/json; charset=UTF-8";
			String curRegExportListUrl = "https://global.api.worldwaterskiers.com/wstims/getParticipantsBySanctionID?apiToken=wstims";
			//String curReqstUrl = curRegExportListUrl + curQueryString;
			String curReqstUrl = curRegExportListUrl;
			String curSanctionEditCode = (String)myTourRow["SanctionEditCode"];
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

			if ( HelperFunctions.isCollegiateSanction( mySanctionNum ) ) {
				Dictionary<String, object> curTeamHeaderList = new Dictionary<String, object>();

				foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
					myCountMemberInput++;
					myProgressInfo.setProgressValue( myCountMemberInput );

					importMemberFromAwsa( curEntry, true, true );

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
			IwwfMembership.warnMessageActive = true;

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
					curMemberEntry.MemberId = HelperFunctions.getAttributeValue( curImportMemberEntry, "MemberID" );
				} else {
					curMemberEntry.MemberId = HelperFunctions.getAttributeValue( curImportMemberEntry, "MemberId" );
				}
				if ( !( myMemberIdValidate.checkMemberId( curMemberEntry.MemberId ) ) ) {
					MessageBox.Show( String.Format( "Invalid member id {0}, checksum validation failed", curMemberEntry.MemberId ) );
					return false;
				}

				curMemberEntry.Federation = HelperFunctions.getAttributeValue( curImportMemberEntry, "Federation" );
				curMemberEntry.ForeignFederationID = HelperFunctions.getAttributeValue( curImportMemberEntry, "ForeignFederationID" );
				curMemberEntry.City = HelperFunctions.getAttributeValue( curImportMemberEntry, "City" );
				curMemberEntry.State = HelperFunctions.getAttributeValue( curImportMemberEntry, "State" );
				curMemberEntry.Team = HelperFunctions.getAttributeValue( curImportMemberEntry, "Team" ).ToUpper();

				if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
					curMemberEntry.AgeGroup = HelperFunctions.getAttributeValue( curImportMemberEntry, "AgeGroup" ).ToUpper();
				} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
					curMemberEntry.AgeGroup = HelperFunctions.getAttributeValue( curImportMemberEntry, "Div" ).ToUpper();
				}
				curMemberEntry.FirstName = (String)curImportMemberEntry["FirstName"];
				curMemberEntry.LastName = (String)curImportMemberEntry["LastName"];

				if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
					curMemberEntry.Gender = (String)curImportMemberEntry["Gender"];
				} else if ( curMemberEntry.AgeGroup.Length > 1 ) {
					curMemberEntry.Gender = myTourEventReg.getGenderOfAgeDiv( curMemberEntry.AgeGroup );
				}

				if ( curImportMemberEntry.ContainsKey( "SkiYearAge" ) ) {
					curMemberEntry.SkiYearAge = Convert.ToInt16( HelperFunctions.getAttributeValueNum( curImportMemberEntry, "SkiYearAge" ) );

				} else if ( curImportMemberEntry.ContainsKey( "Age" ) ) {
					if ( curImportMemberEntry["Age"].GetType() == System.Type.GetType( "System.String" ) ) {
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

					} else if ( curImportMemberEntry["Age"].GetType() == System.Type.GetType( "System.Int32" ) ) {
						curMemberEntry.SkiYearAge = Convert.ToInt16( (int)curImportMemberEntry["Age"] );
					}
				}

				//For collegiate divisions determine if data is valid for collegiate tournaments
				curMemberEntry.EventGroupSlalom = HelperFunctions.getAttributeValue( curImportMemberEntry, "EventSlalom" ).Trim().ToUpper();
				curMemberEntry.EventGroupTrick = HelperFunctions.getAttributeValue( curImportMemberEntry, "EventTrick" ).Trim().ToUpper();
				curMemberEntry.EventGroupJump = HelperFunctions.getAttributeValue( curImportMemberEntry, "EventJump" ).Trim().ToUpper();

				if ( inNcwsa ) return importMemberEntryNcwsa( curImportMemberEntry, curMemberEntry );
				return importMemberEntry( curImportMemberEntry, curMemberEntry, inTourReg, inNcwsa );

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "importMemberFromAwsa: Error processing member"
					+ "\nSkier {0} {1} {2} {3}"
					+ "\nException: {4}"
					, curMemberEntry.MemberId, curMemberEntry.FirstName, curMemberEntry.LastName, curMemberEntry.AgeGroup, ex.Message ) );
				return false;
			}
		}

		public bool importMemberEntryNcwsa( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry ) {
			String curTeamSlalom = "", curTeamTrick = "", curTeamJump = "";
			bool curDataValid = true;

			/*
			 * If a member entry is assigned a group code for any event then assuming this is a non skiing official 
			 */
			if ( curMemberEntry.EventGroupSlalom.Length == 0
				&& curMemberEntry.EventGroupTrick.Length == 0
				&& curMemberEntry.EventGroupJump.Length == 0
				) {
				curMemberEntry.Note = "Official";
				importMemberEntry( curImportMemberEntry, curMemberEntry, true, true );
				return true;
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

				if ( !( curDataValid ) ) return curDataValid;
				// Collegiate skier is not registered for any events, treat as an error because it shouldn't get to this point
				if ( curTeamSlalom.Length == 0 && curTeamTrick.Length == 0 && curTeamJump.Length == 0 ) return false;

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

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select MemberId, SkierName, AgeGroup, Withdrawn FROM TourReg " );
				curSqlStmt.Append( String.Format( "Where MemberId = '{0}' AND SanctionId = '{1}' AND AgeGroup = '{2}'"
					, curMemberEntry.MemberId, mySanctionNum, curMemberEntry.AgeGroup ) );
				DataTable curMemberDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curMemberDataTable.Rows.Count > 0 ) {
					String curWithdrawn = HelperFunctions.getDataRowColValue( curMemberDataTable.Rows[0], "Withdrawn", "N" );
					if ( curWithdrawn.Equals( "Y" ) ) return false;
				}

				String curTrickBoat = HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickBoat" );
				String curJumpHeight = "0";
				curJumpHeight = HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpHeight" );
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

				getRegEventClass( curImportMemberEntry, curMemberEntry, "Slalom" );
				getRegEventClass( curImportMemberEntry, curMemberEntry, "Trick" );
				getRegEventClass( curImportMemberEntry, curMemberEntry, "Jump" );

				String curPrereg = HelperFunctions.getAttributeValue( curImportMemberEntry, "Prereg" );
				String curApptOfficial = HelperFunctions.getAttributeValue( curImportMemberEntry, "ApptdOfficial" );
				String curNote = HelperFunctions.getAttributeValue( curImportMemberEntry, "Note" );

				if ( curPrereg == null ) curPrereg = "";
				if ( curPrereg.ToUpper().Equals( "YES" ) ) curMemberEntry.Note = "OLR";
				else if ( curApptOfficial.Length > 0 ) curMemberEntry.Note = "Appointed Official";
				else if ( curNote.Length > 0 ) curMemberEntry.Note = curNote;

				if ( inNcwsa ) {
					if ( curMemberEntry.Note.Length == 0 ) curMemberEntry.Note = "Team Template";
					String curEventWaiver = HelperFunctions.getAttributeValue( curImportMemberEntry, "EventWaiver" );
					if ( curEventWaiver.Length > 0 ) curMemberEntry.Note += ", EventWaiver=" + curEventWaiver;
				}

				importMemberWithRatings( curImportMemberEntry, curMemberEntry );

				#region Register skier for specified events
				bool curReqstStatus = false;
				String curSaveAgeGroup;
				if ( HelperFunctions.isObjectPopulated( curMemberEntry.EventGroupSlalom ) ) {
					curSaveAgeGroup = "";
					if ( inNcwsa || curMemberEntry.AgeGroup.Equals( curMemberEntry.EventGroupSlalom ) ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Slalom", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountSlalomAdded++;

					} else {
						curSaveAgeGroup = curMemberEntry.AgeGroup;
						curMemberEntry.AgeGroup = curMemberEntry.EventGroupSlalom;
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Slalom", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountSlalomAdded++;
						curMemberEntry.AgeGroup = curSaveAgeGroup;
					}
				}

				if ( HelperFunctions.isObjectPopulated( curMemberEntry.EventGroupTrick ) ) {
					curSaveAgeGroup = "";
					if ( inNcwsa || curMemberEntry.AgeGroup.Equals( curMemberEntry.EventGroupTrick ) ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Trick", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountTrickAdded++;

					} else {
						curSaveAgeGroup = curMemberEntry.AgeGroup;
						curMemberEntry.AgeGroup = curMemberEntry.EventGroupTrick;
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Trick", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountTrickAdded++;
						curMemberEntry.AgeGroup = curSaveAgeGroup;
					}
				}

				if ( HelperFunctions.isObjectPopulated( curMemberEntry.EventGroupJump ) ) {
					curSaveAgeGroup = "";
					if ( inNcwsa || curMemberEntry.AgeGroup.Equals( curMemberEntry.EventGroupJump ) ) {
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Jump", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountJumpAdded++;

					} else {
						curSaveAgeGroup = curMemberEntry.AgeGroup;
						curMemberEntry.AgeGroup = curMemberEntry.EventGroupJump;
						curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberEntry, "Jump", curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountJumpAdded++;
						curMemberEntry.AgeGroup = curSaveAgeGroup;
					}
				}

				if ( HelperFunctions.isObjectPopulated( curMemberEntry.Team ) ) {
					if ( !( inNcwsa ) ) {
						String[] curTeamHeaderCols = { "TeamHeader", curMemberEntry.Team, "", curMemberEntry.Team };
						//procTeamHeaderInput( curTeamHeaderCols, inNcwsa );
					}
				}

				if ( inTourReg
					&& HelperFunctions.isObjectEmpty( curMemberEntry.EventGroupSlalom )
					&& HelperFunctions.isObjectEmpty( curMemberEntry.EventGroupTrick )
					&& HelperFunctions.isObjectEmpty( curMemberEntry.EventGroupJump )
					&& HelperFunctions.isObjectEmpty( curMemberEntry.Team )
					&& !( curMemberEntry.AgeGroup.Equals( "OF" ) )
					) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, "", "" );
					if ( curReqstStatus ) myCountTourRegAdded++;
                }
				#endregion

				#region Analyze and process official ratings for member
				/*
				 * Mark officials that are indicated as the chief, assistant chief, or appointed official ratings
					SELECT CJudgePID, TournAppID, 'CJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(CJudgePID) = 1
					SELECT CDriverPID, TournAppID, 'CD' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(CDriverPID) = 1
					SELECT CScorePID, TournAppID, 'CC' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(CScorePID) = 1
					SELECT CSafPID, TournAppID, 'CS' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(CSafPID) = 1
					SELECT TechCPID, TournAppID, 'CT' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(TechCPID) = 1
					SELECT Ap1JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap1JPID) = 1
					SELECT Ap2JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap2JPID) = 1
					SELECT Ap3JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap3JPID) = 1
					SELECT Ap4JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap4JPID) = 1
					SELECT Ap5JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap5JPID) = 1
					SELECT Ap6JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap6JPID) = 1
					SELECT Ap7JPID, TournAppID, 'APTJ' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap7JPID) = 1
					SELECT Ap1SPID, TournAppID, 'APTS' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap1SPID) = 1
					SELECT Ap2SPID, TournAppID, 'APTS' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap2SPID) = 1
					SELECT Ap3SPID, TournAppID, 'APTS' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap3SPID) = 1
					SELECT Ap1DrPID, TournAppID, 'APTD' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap1DrPID) = 1
					SELECT Ap2DrPID, TournAppID, 'APTD' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap2DrPID) = 1
					SELECT Ap3DrPID, TournAppID, 'APTD' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(Ap3DrPID) = 1
					SELECT PanAmPID, TournAppID, 'PanAm' AS OffCode FROM Sanctions.dbo.registration WHERE TournAppID = @SanctionId and ISNUMERIC(PanAmPID) = 1
				 * 
				*/
				if ( curApptOfficial.Length > 0 ) {
					if ( curApptOfficial.ToUpper().Equals( "CJ" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "JudgeChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "ACJ" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "JudgeAsstChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "APTJ" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "JudgeAppointed" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "CD" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "DriverChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "ACD" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "DriverAsstChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "APTD" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "DriverAppointed" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "CC" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "ScoreChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "ACC" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "ScoreAsstChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "APTS" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "ScoreAppointed" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "CS" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "SafetyChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "ACS" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "SafetyAsstChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "CT" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "TechChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "ACT" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "TechAsstChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;

					} else if ( curApptOfficial.ToUpper().Equals( "CA" ) ) {
						curReqstStatus = myTourEventReg.addEventOfficial( curMemberEntry, "AnncrChief" );
						if ( curReqstStatus ) myCountTourRegAdded++;
					}
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

		private void getRegEventClass( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry, String curEvent ) {
			String curEventClass = "";

			if ( curImportMemberEntry.ContainsKey( "EventClass" + curEvent ) ) {
				curEventClass = (String)curImportMemberEntry["EventClass" + curEvent];

			} else if ( curImportMemberEntry.ContainsKey( curEvent + "Paid" ) ) {
				curEventClass = (String)curImportMemberEntry[curEvent + "Paid"];
			}

			if ( curEventClass.Trim().Length == 0 ) return;

			curMemberEntry.setRegEventClass( curEvent, curEventClass );
		}

		private bool regSkierForEvent( Dictionary<string, object> curImportMemberEntry, MemberEntry curMemberEntry, String curEvent
			, String curTrickBoat, String curJumpHeight ) {
			String methodName = "ImportMember: regSkierForEvent: ";
			int curEventRoundsPaid = 0;

			try {
				String curEventGroup = curMemberEntry.getEventGroup( curEvent );
				String curRegEventClass = curMemberEntry.getRegEventClass( curEvent );
				String curEventClass = curRegEventClass;
				if ( curRegEventClass.Trim().Length > 1 ) {
					curEventClass = curRegEventClass.ToString().Substring( 0, 1 );
					try {
						curEventRoundsPaid = int.Parse( curRegEventClass.Substring( 1 ) );
					} catch ( Exception ex ) {
						Log.WriteFile( String.Format( "{0}Exception encounter determining rounds paid for {1} {2} Message: {3} "
							, methodName, curEvent, curMemberEntry.getSkierName(), curEventGroup, ex.Message ) );
						curEventRoundsPaid = 0;
					}
				}
				if ( curEventGroup.Trim().Length == 0 ) return false;

                bool curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, curTrickBoat, curJumpHeight );
				if ( curReqstStatus ) myCountTourRegAdded++;

				if ( curEventGroup.ToLower().Equals( "of" ) ) return false;

				bool returnStatus = false;
				if ( curEvent.Equals( "Slalom" ) ) returnStatus = myTourEventReg.addEventSlalom( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );
				if ( curEvent.Equals( "Trick" ) ) returnStatus = myTourEventReg.addEventTrick( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );
				if (curEvent.Equals( "Jump" )) {
                    String curSaveAgeGroup = "";
                    if (curMemberEntry.AgeGroup.ToUpper().Equals( "B1" )) {
                        curSaveAgeGroup = curMemberEntry.AgeGroup;
                        curMemberEntry.AgeGroup = "B2";

                    } else if (curMemberEntry.AgeGroup.ToUpper().Equals( "G1" )) {
                        curSaveAgeGroup = curMemberEntry.AgeGroup;
                        curMemberEntry.AgeGroup = "G2";
                    }
                    returnStatus = myTourEventReg.addEventJump( curMemberEntry.MemberId, curEventGroup, curEventClass, curMemberEntry.AgeGroup, curMemberEntry.Team );
                    if (HelperFunctions.isObjectPopulated( curSaveAgeGroup )) curMemberEntry.AgeGroup = curSaveAgeGroup;
                }

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
			StringBuilder curSqlStmt = new StringBuilder( "" );
			String curEvent = "";
			Decimal curSlalom = 0, curTrick = 0, curJump = 0, curOverall = 0;
			DataTable curDataTable;
			int rowsProc = 0;

			#region Retrieve all member attributes and prepare for processing
			curMemberEntry.CanSki = HelperFunctions.isValueTrue( HelperFunctions.getAttributeValue( curImportMemberEntry, "CanSki" ) );
			curMemberEntry.CanSkiGR = HelperFunctions.isValueTrue( HelperFunctions.getAttributeValue( curImportMemberEntry, "CanSkiGR" ) );
			curMemberEntry.WaiverSigned = calcWaiverStatus( curImportMemberEntry );
			curMemberEntry.MemberStatus = calcMemberStatus( curImportMemberEntry, curMemberEntry );

			curMemberEntry.JudgeSlalomRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgeSlalom" );
			//String curPanAmValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgePanAmSlalom" );
			//if ( curPanAmValue.Length > 0 ) curMemberEntry.JudgeSlalomRating = "PanAm";

			curMemberEntry.JudgeTrickRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgeTrick" );
			//curPanAmValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgePanAmTrick" );
			//if ( curPanAmValue.Length > 0 ) curMemberEntry.JudgeTrickRating = "PanAm";

			curMemberEntry.JudgeJumpRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgeJump" );
			//curPanAmValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "JudgePanAmJump" );
			//if ( curPanAmValue.Length > 0 ) curMemberEntry.JudgeJumpRating = "PanAm";

			curMemberEntry.DriverSlalomRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "DriverSlalom" );
			curMemberEntry.DriverTrickRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "DriverTrick" );
			curMemberEntry.DriverJumpRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "DriverJump" );

			curMemberEntry.ScorerSlalomRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "ScorerSlalom" );
			curMemberEntry.ScorerTrickRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "ScorerTrick" );
			curMemberEntry.ScorerJumpRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "ScorerJump" );

			curMemberEntry.SafetyOfficialRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "Safety" );
			curMemberEntry.TechControllerSlalomRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "TechControllerSlalom" );
            curMemberEntry.TechControllerTrickRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "TechControllerTrick" );
            curMemberEntry.TechControllerJumpRating = HelperFunctions.getAttributeValue( curImportMemberEntry, "TechControllerJump" );

            curSqlStmt = new StringBuilder( "Select MemberId, UpdateDate from MemberList Where MemberId = '" + curMemberEntry.MemberId + "'" );
			DataTable curMemberDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curMemberDataTable.Rows.Count > 0 ) {
				lastRecModDate = (DateTime)curMemberDataTable.Rows[0]["UpdateDate"];
				newMember = false;
			} else {
				newMember = true;
			}
			#endregion

			#region Insert or update member information
			if ( newMember ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert MemberList (" );
				curSqlStmt.Append( "MemberId, LastName, FirstName, SkiYearAge, Gender, City, State, Federation, ForeignFederationID, MemberStatus" );
				curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
				curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
				curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
				curSqlStmt.Append( ", SafetyOfficialRating, TechControllerSlalomRating, TechControllerTrickRating, TechControllerJumpRating" );
				curSqlStmt.Append( ", Note, MemberExpireDate, InsertDate, UpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getLastNameForDB() + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getFirstNameForDB() + "'" );
				curSqlStmt.Append( ", " + curMemberEntry.SkiYearAge );
				curSqlStmt.Append( ", '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.Federation + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.ForeignFederationID + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.JudgeSlalomRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.JudgeTrickRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.JudgeJumpRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.DriverSlalomRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.DriverTrickRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.DriverJumpRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.ScorerSlalomRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.ScorerTrickRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.ScorerJumpRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.SafetyOfficialRating + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.TechControllerSlalomRating + "'" );
                curSqlStmt.Append( ", '" + curMemberEntry.TechControllerTrickRating + "'" );
                curSqlStmt.Append( ", '" + curMemberEntry.TechControllerJumpRating + "'" );
                curSqlStmt.Append( ", '" + curMemberEntry.Note + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.MemberExpireDate.ToString( "MM/dd/yy" ) + "'" );
				curSqlStmt.Append( ", getdate(), getdate() )" );

				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) myCountMemberAdded++;

			} else {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update MemberList " );
				curSqlStmt.Append( " Set LastName = '" + curMemberEntry.getLastNameForDB() + "'" );
				curSqlStmt.Append( ", FirstName = '" + curMemberEntry.getFirstNameForDB() + "'" );
				curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
				curSqlStmt.Append( ", ForeignFederationID = '" + curMemberEntry.ForeignFederationID + "'" );
				curSqlStmt.Append( ", MemberStatus = '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", Note = '" + curMemberEntry.Note + "'" );
				curSqlStmt.Append( ", MemberExpireDate = '" + curMemberEntry.MemberExpireDate.ToString( "MM/dd/yy" ) + "'" );
				curSqlStmt.Append( ", UpdateDate = getdate()" );
				curSqlStmt.Append( ", JudgeSlalomRating = '" + curMemberEntry.JudgeSlalomRating + "'" );
				curSqlStmt.Append( ", JudgeTrickRating = '" + curMemberEntry.JudgeTrickRating + "'" );
				curSqlStmt.Append( ", JudgeJumpRating = '" + curMemberEntry.JudgeJumpRating + "'" );
				curSqlStmt.Append( ", DriverSlalomRating = '" + curMemberEntry.DriverSlalomRating + "'" );
				curSqlStmt.Append( ", DriverTrickRating = '" + curMemberEntry.DriverTrickRating + "'" );
				curSqlStmt.Append( ", DriverJumpRating = '" + curMemberEntry.DriverJumpRating + "'" );
				curSqlStmt.Append( ", ScorerSlalomRating = '" + curMemberEntry.ScorerSlalomRating + "'" );
				curSqlStmt.Append( ", ScorerTrickRating = '" + curMemberEntry.ScorerTrickRating + "'" );
				curSqlStmt.Append( ", ScorerJumpRating = '" + curMemberEntry.ScorerJumpRating + "'" );
				curSqlStmt.Append( ", SafetyOfficialRating = '" + curMemberEntry.SafetyOfficialRating + "'" );
				curSqlStmt.Append( ", TechControllerSlalomRating = '" + curMemberEntry.TechControllerSlalomRating + "'" );
                curSqlStmt.Append( ", TechControllerTrickRating = '" + curMemberEntry.TechControllerTrickRating + "'" );
                curSqlStmt.Append( ", TechControllerJumpRating = '" + curMemberEntry.TechControllerJumpRating + "'" );
                curSqlStmt.Append( " Where MemberId = '" + curMemberEntry.MemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) myCountMemberUpdated++;
			}
			#endregion

			#region Insert or update skier slalom ranking data
			String curRank = HelperFunctions.getAttributeValue( curImportMemberEntry, "SlalomRank" );
			if ( curRank.Length > 0 ) Decimal.TryParse( curRank, out curSlalom );
			if ( curSlalom > 0 ) {
				curEvent = "Slalom";
				curOverall += curSlalom;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select PK from SkierRanking " );
				curSqlStmt.Append( String.Format( "Where MemberId = '{0}' And AgeGroup = '{1}' And Event = '{2}'"
					, curMemberEntry.MemberId, curMemberEntry.AgeGroup, curEvent ) );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curSlalom.ToString() );
					curSqlStmt.Append( ", Rating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "SlalomRating" ) + "' " );
					curSqlStmt.Append( String.Format( "Where MemberId = '{0}' And AgeGroup = '{1}' And Event = '{2}'"
						, curMemberEntry.MemberId, curMemberEntry.AgeGroup, curEvent ) );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curSlalom.ToString() );
					curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "SlalomRating" ) + "'" );
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
				curSqlStmt.Append( ", RankingRating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "SlalomRating" ) + "' " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Event = '{3}'"
					, mySanctionNum, curMemberEntry.MemberId, curMemberEntry.AgeGroup, curEvent ) );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Insert or update skier trick ranking data
			curRank = HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickRank" );
			if ( curRank.Length > 0 ) Decimal.TryParse( curRank, out curTrick );
			if ( curTrick > 0 ) {
				curEvent = "Trick";
				curOverall += curTrick;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select PK from SkierRanking " );
				curSqlStmt.Append( String.Format( "Where MemberId = '{0}' And AgeGroup = '{1}' And Event = '{2}'"
					, curMemberEntry.MemberId, curMemberEntry.AgeGroup, curEvent ) );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curTrick.ToString() );
					curSqlStmt.Append( ", Rating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickRating" ) + "' " );
					curSqlStmt.Append( String.Format( "Where MemberId = '{0}' And AgeGroup = '{1}' And Event = '{2}'"
						, curMemberEntry.MemberId, curMemberEntry.AgeGroup, curEvent ) );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberEntry.MemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curTrick.ToString() );
					curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickRating" ) + "'" );
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
				curSqlStmt.Append( ", RankingRating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickRating" ) + "'" );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
				curSqlStmt.Append( "   And MemberId = '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "   And Event = '" + curEvent + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Insert or update skier jump ranking data
			curRank = HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpRank" );
			if ( curRank.Length > 0 ) Decimal.TryParse( curRank, out curJump );
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
					curSqlStmt.Append( ", Rating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpRating" ) + "' " );
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
					curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpRating" ) + "'" );
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
				curSqlStmt.Append( ", RankingRating = '" + HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpRating" ) + "'" );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
				curSqlStmt.Append( "   And MemberId = '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curMemberEntry.AgeGroup + "'" );
				curSqlStmt.Append( "   And Event = '" + curEvent + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
			#endregion

			#region Update tournament registration entry if one exists in current tournament for this member
			String curSanctionId = (String)myTourRow["SanctionId"];
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
				curSqlStmt.Append( ", ReadyToSki = '" + curMemberEntry.ReadyToSki + "'" );
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
					curSqlStmt.Append( ", TechControllerSlalomRating = '" + curMemberEntry.TechControllerSlalomRating + "'" );
                    curSqlStmt.Append( ", TechControllerTrickRating = '" + curMemberEntry.TechControllerTrickRating + "'" );
                    curSqlStmt.Append( ", TechControllerJumpRating = '" + curMemberEntry.TechControllerJumpRating + "'" );
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

			String curTeamName = HelperFunctions.getAttributeValue( curImportMemberEntry, "TeamName" );
			String curTeamCode = HelperFunctions.getAttributeValue( curImportMemberEntry, "TeamCode" );
			String curAgeGroup = HelperFunctions.getAttributeValue( curImportMemberEntry, "AgeGroup" );
			String curEventGroup = HelperFunctions.getAttributeValue( curImportMemberEntry, "EventGroup" );

			int curTeamSlalomRunOrder = 1, curTeamTrickRunOrder = 1, curTeamJumpRunOrder = 1;

			if ( curTeamCode.ToLower().Equals( "off" ) ) return false;

			String curValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "SlalomRunOrder" );
			if ( HelperFunctions.isObjectPopulated( curValue ) ) {
				curTeamSlalomRunOrder = int.Parse( curValue );
			} else {
				curTeamSlalomRunOrder = 0;
			}
			curValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "TrickRunOrder" );
			if ( HelperFunctions.isObjectPopulated( curValue ) ) {
				curTeamTrickRunOrder = int.Parse( curValue );
			} else {
				curTeamTrickRunOrder = 0;
			}
			curValue = HelperFunctions.getAttributeValue( curImportMemberEntry, "JumpRunOrder" );
			if ( HelperFunctions.isObjectPopulated( curValue ) ) {
				curTeamJumpRunOrder = int.Parse( curValue );
			} else {
				curTeamJumpRunOrder = 0;
			}

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
				curSqlStmt.Append( ")" );

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

		public static String calcMemberStatus( Dictionary<string, object> inImportMemberEntry, MemberEntry inMemberEntry ) {
			return calcMemberStatus( inImportMemberEntry, null, inMemberEntry );
		}
		public static String calcMemberStatus( DataRow inImportMemberRow ) {
			return calcMemberStatus( null, inImportMemberRow, null );
		}
		public static String calcMemberStatus( Dictionary<string, object> inImportMemberEntry, DataRow inImportMemberRow, MemberEntry inMemberEntry ) {
			String returnMembershipStatus = "";
			int curSkiYearAge = 0, curMembershipStatusCodeNum = 0;
			bool curCanSki = false, curCanSkiGR = false;
			String curMemTypeDesc, curSafeSportStatus, curWaiverStatus;
			DateTime curMemExpireDate = new DateTime();
			DateTime curWaiverExpireDate = new DateTime();
			DateTime curSafeSportExpireDate = new DateTime();
			if ( myTourRow == null ) myTourRow = getTourData();
			DateTime curTourEventDatePlus1 = myTourEventDate.AddDays( 1 );

			if ( inImportMemberRow == null ) {
				curMemTypeDesc = HelperFunctions.getAttributeValue( inImportMemberEntry, "MemTypeDesc" );
				curSafeSportStatus = HelperFunctions.getAttributeValue( inImportMemberEntry, "SafeSportStatus" );
				curWaiverStatus = HelperFunctions.getAttributeValue( inImportMemberEntry, "WaiverStatus" );
				int.TryParse( HelperFunctions.getAttributeValue( inImportMemberEntry, "membershipStatusCode" ), out curMembershipStatusCodeNum );
				DateTime.TryParse( HelperFunctions.getAttributeValue( inImportMemberEntry, "WaiverExpiration" ), out curWaiverExpireDate );
				DateTime.TryParse( HelperFunctions.getAttributeValue( inImportMemberEntry, "SafeSportExpiration" ), out curSafeSportExpireDate );
				DateTime.TryParse( HelperFunctions.getAttributeValue( inImportMemberEntry, "MembershipExpiration" ), out curMemExpireDate );
				curSkiYearAge = inMemberEntry.SkiYearAge;
				curCanSki = inMemberEntry.CanSki;
				curCanSkiGR = inMemberEntry.CanSkiGR;
				inMemberEntry.MemberExpireDate = curMemExpireDate;

			} else {
				curMemTypeDesc = HelperFunctions.getDataRowColValue( inImportMemberRow, "MemTypeDesc", "" );
				curSafeSportStatus = HelperFunctions.getDataRowColValue( inImportMemberRow, "SafeSportStatus", "" );
				curWaiverStatus = HelperFunctions.getDataRowColValue( inImportMemberRow, "WaiverStatus", "" );
				int.TryParse( HelperFunctions.getDataRowColValue( inImportMemberRow, "membershipStatusCode", "0" ), out curMembershipStatusCodeNum );
				DateTime.TryParse( HelperFunctions.getDataRowColValue( inImportMemberRow, "WaiverExpiration", "" ), out curWaiverExpireDate );
				DateTime.TryParse( HelperFunctions.getDataRowColValue( inImportMemberRow, "SafeSportExpiration", "" ), out curSafeSportExpireDate );
				DateTime.TryParse( HelperFunctions.getDataRowColValue( inImportMemberRow, "MembershipExpiration", "" ), out curMemExpireDate );
				int.TryParse( HelperFunctions.getDataRowColValue( inImportMemberRow, "SkiYearAge", "0" ), out curSkiYearAge );

				curCanSki = HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( inImportMemberRow, "CanSki", "0" ) );
				curCanSkiGR = HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( inImportMemberRow, "CanSkiGR", "0" ) );
			}
			returnMembershipStatus = getMembershipStatus( curMembershipStatusCodeNum );


			if ( curMembershipStatusCodeNum == MembershipStatusCodeActive ) {
				if ( curMemExpireDate < curTourEventDatePlus1 ) {
					returnMembershipStatus += " ** Needs Renew";
					curMembershipStatusCodeNum = 0;

				} else if ( curWaiverExpireDate < curTourEventDatePlus1 || curWaiverStatus != "Signed" ) {
					returnMembershipStatus += " ** Needs Annual Waiver Signed";
					curMembershipStatusCodeNum = 0;

				} else if ( curSkiYearAge >= 18 && (curSafeSportExpireDate < curTourEventDatePlus1 || curSafeSportStatus != "Complete") ) {
					returnMembershipStatus += " ** Needs Safe Sport Renewed";
					curMembershipStatusCodeNum = 0;

				} else if ( !( curCanSki ) ) {
					returnMembershipStatus += " ** Needs Upgrade";
					curMembershipStatusCodeNum = 0;

				} else if ( !( curCanSki ) && curCanSkiGR ) {
					returnMembershipStatus += " ** Grass Roots Only";
					curMembershipStatusCodeNum = 0;

				} else {
					return returnMembershipStatus;
				}

			} else if ( returnMembershipStatus.Equals( MembershipStatusNone ) ) {
				returnMembershipStatus += " ** Needs Renew/Upgrade";

			} else if ( curCanSki ) {
				returnMembershipStatus += " ** Needs Renew";
			}

			return returnMembershipStatus;
		}

		public static bool calcWaiverStatus( Dictionary<string, object> inImportMemberEntry ) {
			DateTime curTourEventDatePlus1 = myTourEventDate.AddDays( 5 );
			String curWaiverStatus = HelperFunctions.getAttributeValue( inImportMemberEntry, "WaiverStatus" );
			DateTime curWaiverExpireDate = new DateTime();
			DateTime.TryParse( HelperFunctions.getAttributeValue( inImportMemberEntry, "WaiverExpiration" ), out curWaiverExpireDate );
			if ( curWaiverExpireDate >= curTourEventDatePlus1 && curWaiverStatus == "Signed" ) return true;
			return false;
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

			String curEventGroupSlalom = HelperFunctions.getAttributeValue( curEntry, "EventSlalom" ).Trim().ToUpper();
			String curEventGroupTrick = HelperFunctions.getAttributeValue( curEntry, "EventTrick" ).Trim().ToUpper();
			String curEventGroupJump = HelperFunctions.getAttributeValue( curEntry, "EventJump" ).Trim().ToUpper();

			if ( curEventGroupSlalom.Length > 1 && curEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals("B" ) 
				|| curEventGroupTrick.Length > 1 && curEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "B" )
				|| curEventGroupJump.Length > 1 && curEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "B" )
				) {
				curDiv = "B" + curDiv.Substring( 1, 1 );
			}
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

		private static DataRow getTourData() {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null || mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return null;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, Name, Class, Federation" );
			curSqlStmt.Append( ", SanctionEditCode, Rules, EventDates, EventLocation, TourDataLoc " );
			curSqlStmt.Append( "FROM Tournament " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				DateTime.TryParse( HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "EventDates", "" ), out myTourEventDate );
				return curDataTable.Rows[0];
			}
			return null;
		}

	}
}
