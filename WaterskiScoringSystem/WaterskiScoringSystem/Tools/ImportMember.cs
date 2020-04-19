using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Tools {
    class ImportMember {
        private char[] myTabDelim = new char[] { '\t' };
        private char[] mySingleQuoteDelim = new char[] { '\'' };

		private bool myTourTypePickAndChoose = false;

		private String mySanctionNum;

		private int myCountMemberInput = 0;
		private int myCountMemberAdded = 0;
		private int myCountMemberUpdate = 0;
		private int myCountTourRegAdded = 0;
		private int myCountSlalomAdded = 0;
		private int myCountTrickAdded = 0;
		private int myCountJumpAdded = 0;

		private int myCountInput = 0;
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
			if ( tourType != null && tourType.ToLower().Equals("pick")) myTourTypePickAndChoose = true;

			if ( inTourRow == null ) {
				mySanctionNum = Properties.Settings.Default.AppSanctionNum;
				if ( mySanctionNum == null ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					return;

				} else {
					if ( mySanctionNum.Length < 6 ) {
						MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
						return;

					} else {
						myTourRow = getTourData();
					}
				}

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
			String curOfficialExportListUrl = "http://www.usawaterski.org/admin/GetMemberRegExportJson.asp";
			String curReqstUrl = curOfficialExportListUrl + curQueryString;
			String curSanctionEditCode = (String) myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve officials and ratings.  Enter required value on Tournament Form" );
				return;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			//curResponseDataList = SendMessageHttp.getMessageResponseJsonArray(curReqstUrl, curHeaderParams, curContentType, "wstims", "Slalom38tTrick13Jump250", false);
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList != null && curResponseDataList.Count > 0 ) {
				myProgressInfo = new ProgressWindow();
				myProgressInfo.setProgressMin( 1 );
				myProgressInfo.setProgressMax( curResponseDataList.Count );

				if ( mySanctionNum.Substring( 2, 1 ).ToUpper().Equals( "U" ) ) {
					Dictionary < String, object> curTeamHeaderList = new Dictionary<String, object>();

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

			}

			displayMemberProcessCounts();
        }

		private void buildNcwsaTeamHeader( Dictionary<string, object> curEntry, Dictionary<string, object> curTeamHeaderList ) {
			String curTeam = ( (String) curEntry["Team"] ).Trim().ToUpper();
			String curTeamName = ( (String) curEntry["TeamName"] ).Trim();
			String curDiv = ( (String) curEntry["Div"] ).ToUpper();
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

		public bool importMemberFromAwsa( Dictionary<string, object> curImportMemberEntry, bool inTourReg, bool inNcwsa ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			String curMemberId = "", curFirstName = "", curLastName = "";

			try {
				/*
				 * Validate the member id to ensure it is valid
				 * Update the memeber data record and also update tournament registration record for the member data
				 */
				if ( curImportMemberEntry.ContainsKey( "MemberID" ) ) {
					curMemberId = (String) curImportMemberEntry["MemberID"];
				} else {
					curMemberId = (String) curImportMemberEntry["MemberId"];
				}
				if ( curMemberId.Length == 11 ) {
					curMemberId = curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 );
				}

				curLastName = (String) curImportMemberEntry["LastName"];
				curLastName = stringReplace( curLastName, mySingleQuoteDelim, "''" );
				curFirstName = (String) curImportMemberEntry["FirstName"];
				curFirstName = stringReplace( curFirstName, mySingleQuoteDelim, "''" );

				if ( myProgressInfo != null ) {
					myProgressInfo.setProgessMsg( "Processing " + curFirstName + " " + curLastName );
					myProgressInfo.Show();
					myProgressInfo.Refresh();
				}

				String curAgeGroup = "";
				if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
					curAgeGroup = ( (String) curImportMemberEntry["AgeGroup"] ).ToUpper();
				} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
					curAgeGroup = ( (String) curImportMemberEntry["Div"] ).ToUpper();
				}
				String curTeam = "";
				if ( curImportMemberEntry.ContainsKey( "Team" ) ) {
					curTeam = ( (String) curImportMemberEntry["Team"] ).ToUpper();
				}

				if ( myMemberIdValidate.checkMemberId( curMemberId ) ) {
					importMemberWithRatings( curImportMemberEntry );

				} else {
					MessageBox.Show( "Invalid member id, checksum validation failed." );
					return false;
				}

				String curGender = "";
				if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
					curGender = (String) curImportMemberEntry["Gender"];
				} else if ( curAgeGroup.Length > 1 ) {
					curGender = myTourEventReg.getGenderOfAgeDiv( curAgeGroup );
				}

				#region Register member in tournament events as indicated
				String curEventSlalom = (String) curImportMemberEntry["EventSlalom"];
				if ( curEventSlalom == null ) curEventSlalom = "";
				String curEventTrick = (String) curImportMemberEntry["EventTrick"];
				if ( curEventTrick == null ) curEventTrick = "";
				String curEventJump = (String) curImportMemberEntry["EventJump"];
				if ( curEventJump == null ) curEventJump = "";

				if ( ( curAgeGroup.Equals( "OF" ) )
					|| ( curTeam.Equals( "off" ) )
					|| ( curEventSlalom.Length > 0 )
					|| ( curEventTrick.Length > 0 )
					|| ( curEventJump.Length > 0 )
					) {
					if ( myTourEventReg.validAgeDiv( curAgeGroup ) ) {
					} else {
						MessageBox.Show( "Invalid age group " + curAgeGroup + " detected. Bypassing tournament registration"
							+ "\n" + curMemberId + " " + curFirstName + " " + curLastName
						);
						return false;
					}
				}

				String curTrickBoat = "", curJumpHeight = "", curPreRegNote = "";
				curTrickBoat = (String) curImportMemberEntry["TrickBoat"];
				curJumpHeight = (String) curImportMemberEntry["JumpHeight"];
				if ( curJumpHeight.Length == 0 ) {
					curJumpHeight = "0";
				} else {
					try {
						Decimal tmpJumpHeight = Convert.ToDecimal( curJumpHeight );
						if ( tmpJumpHeight > 6 ) {
							tmpJumpHeight = tmpJumpHeight / 10;
							curJumpHeight = tmpJumpHeight.ToString( "#.#" );
						}
						if ( inNcwsa ) {
							if ( tmpJumpHeight < Convert.ToDecimal( "5.0" ) ) {
								if ( ((String)curImportMemberEntry[ "EventJump"]).Substring( 0, 1 ).ToUpper().Equals( "B" ) ) {
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
					String curPrereg = (String) curImportMemberEntry["Prereg"] ;
					if ( curPrereg == null ) curPrereg = "";
                    if ( curPrereg.Equals( "YES" ) ) {
						curPreRegNote = "Pre-Registration";

					} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Length > 0 ) {
						curPreRegNote = "Appointed Official";
					}

				} else if ( curImportMemberEntry.ContainsKey( "Note" ) ) {
					curPreRegNote = (String) curImportMemberEntry["Note"];
				}
				if ( curPreRegNote.Length == 0 ) curPreRegNote = "Tour Reg Template";

				bool curReqstStatus = false;
                if ( curAgeGroup.Equals( "OF" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;

				} else {
					if ( curTeam.ToLower().Equals( "off" ) ) {
						curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
						if ( curReqstStatus ) myCountTourRegAdded++;
					}

					curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberId, "Slalom", curEventSlalom
						, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight, curTeam );
                    if ( curReqstStatus ) myCountSlalomAdded++;

					curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberId, "Trick", curEventTrick
						, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight, curTeam );
					if ( curReqstStatus ) myCountTrickAdded++;

					curReqstStatus = regSkierForEvent( curImportMemberEntry, curMemberId, "Jump", curEventJump
						, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight, curTeam );
					if ( curReqstStatus ) myCountJumpAdded++;

					if ( curTeam.Length > 0 ) {
						if ( !( inNcwsa ) ) {
							String[] curTeamHeaderCols = { "TeamHeader", curTeam, "", curTeam };
							//procTeamHeaderInput( curTeamHeaderCols, inNcwsa );
						}

					}
				}
				#endregion
				
				#region Analyze and process official ratings for member
				/*
				 * Mark officials that are indicated as the chief, assistant chief, or appointed official ratings
				*/
				if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "JudgeChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "ACJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "JudgeAsstChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "APTJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "JudgeAppointed" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "DriverChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "ACD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "DriverAsstChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "APTD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "DriverAppointed" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CC" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "ScoreChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "ACC" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "ScoreAsstChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "APTS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "ScoreAppointed" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "SafetyChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "ACS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "SafetyAsstChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CT" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "TechChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "ACT" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "TechAsstChief" );

				} else if ( ((String) curImportMemberEntry["ApptdOfficial"]).Equals( "CA" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( curMemberId, "AnncrChief" );
				}
				#endregion

				return true;

			} catch ( Exception ex ) {
				String ExcpMsg = "importMemberFromAwsa: Error processing member "
					+ curMemberId + " " + curFirstName	+ " " + curLastName	+ "\n\n " + ex.Message;
				MessageBox.Show( ExcpMsg );
				return false;
			}

			return true;
		}

		private bool regSkierForEvent( Dictionary<string, object> curImportMemberEntry, String curMemberId, String curEvent
			, String curEventGroup, String curPreRegNote, String curAgeGroup, String curTrickBoat, String curJumpHeight, String curTeam ) {
			int curEventRoundsPaid = 0;
			String curEventClass = "";

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
				if ( curEventGroup.Trim().Length > 0 ) {
					if ( curEventClass.Trim().Length > 1 ) curEventClass = curEventClass.Substring( 0, 1 );

					bool curReqstStatus = myTourEventReg.addTourReg( curMemberId, curPreRegNote, curAgeGroup, curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;

					if ( !( curEventGroup.ToLower().Equals( "of" ) ) ) {
						bool returnStatus = false;
						if ( curEvent.Equals( "Slalom" ) ) returnStatus = myTourEventReg.addEventSlalom( curMemberId, curEventGroup, curEventClass, curAgeGroup, curTeam );
						if ( curEvent.Equals( "Trick" ) ) returnStatus = myTourEventReg.addEventTrick( curMemberId, curEventGroup, curEventClass, curAgeGroup, curTeam );
						if ( curEvent.Equals( "Jump" ) ) returnStatus = myTourEventReg.addEventJump( curMemberId, curEventGroup, curEventClass, curAgeGroup, curTeam );

						if ( curEventRoundsPaid > 0 && myTourTypePickAndChoose ) {
							returnStatus = myTourEventReg.addEventRunorder( curEvent, curMemberId, curEventGroup, curEventClass, curEventRoundsPaid, curAgeGroup );
						}
						return returnStatus;
					}
				}
				return false;

			} catch ( Exception ex ) {
				String curLastName = (String) curImportMemberEntry["LastName"];
				curLastName = stringReplace( curLastName, mySingleQuoteDelim, "''" );
				String curFirstName = (String) curImportMemberEntry["FirstName"];
				curFirstName = stringReplace( curFirstName, mySingleQuoteDelim, "''" );

				String ExcpMsg = "regSkierForEvent: Error processing member "
					+ curMemberId + " " + curFirstName + " " + curLastName + " Event=" + curEvent + "\n\n " + ex.Message;
				MessageBox.Show( ExcpMsg );
				return false;
			}

		}

		public void importMemberWithRatings( Dictionary<string, object> curImportMemberEntry ) {
            bool newMember = false;
			DateTime lastRecModDate = new DateTime();
			StringBuilder curSqlStmt = new StringBuilder("");
			String curEvent = "";
			Decimal curSlalom = 0, curTrick = 0, curJump = 0, curOverall = 0;
			DataTable curDataTable;
			int rowsProc = 0;

			String curMemberId = "";
			if ( curImportMemberEntry.ContainsKey( "MemberID" ) ) {
				curMemberId = (String) curImportMemberEntry["MemberID"];
			} else {
				curMemberId = (String) curImportMemberEntry["MemberId"];
			}

			if ( curMemberId.Length == 11 ) {
				curMemberId = curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 );
			}

			#region Retrieve all member attributes and prepare for processing
			String LastName = (String) curImportMemberEntry["LastName"];
            LastName = stringReplace(LastName, mySingleQuoteDelim, "''");
            String FirstName = (String) curImportMemberEntry["FirstName"];
            FirstName = stringReplace(FirstName, mySingleQuoteDelim, "''");

			String curAgeGroup = "";
			if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
				curAgeGroup = ( (String) curImportMemberEntry["AgeGroup"] ).ToUpper();
			} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
				curAgeGroup = ( (String) curImportMemberEntry["Div"] ).ToUpper();
			}

			String Federation = "USA";
            if ( curImportMemberEntry["Federation"] != null ) {
                Federation = (String) curImportMemberEntry["Federation"];
            }
			String MemberStatus = "InActive";
			if ( curImportMemberEntry["ActiveMember"].GetType() == System.Type.GetType( "System.Boolean" ) ) {
				Boolean ActiveMember = (Boolean) curImportMemberEntry["ActiveMember"];
				if ( ActiveMember ) MemberStatus = "Active";

			} else {
				MemberStatus = (String)curImportMemberEntry["ActiveMember"];
			}

            String MemTypeDesc = (String) curImportMemberEntry["MemTypeDesc"];
			String Note = "Imported";
			if ( curImportMemberEntry.ContainsKey( "Note" ) ) {
				if ( curImportMemberEntry["Note"] != null ) {
					Note = (String) curImportMemberEntry["Note"];
					if ( Note.Length == 0 ) Note = "Imported";
				}
			}

			String curGender = "";
			if ( curImportMemberEntry.ContainsKey( "Gender" ) ) {
				curGender = (String) curImportMemberEntry["Gender"];
			} else if ( curAgeGroup.Length > 1 ) {
				curGender = myTourEventReg.getGenderOfAgeDiv( curAgeGroup );
			}

			String City = (String) curImportMemberEntry["City"];
            City = stringReplace(City, mySingleQuoteDelim, "''");
            String State = (String) curImportMemberEntry["State"];

			String SkiYearAge = "0";
			if ( curImportMemberEntry.ContainsKey( "SkiYearAge" ) ) {
				SkiYearAge = ( (int) curImportMemberEntry["SkiYearAge"] ).ToString();
			} else if ( curImportMemberEntry.ContainsKey( "Age" ) ) {
				SkiYearAge = ( (int) curImportMemberEntry["Age"] ).ToString();
			}
			if ( SkiYearAge.Length > 0 ) {
                int numCk;
                if ( int.TryParse(SkiYearAge, out numCk) ) {
                    if ( numCk < 0 ) {
                        SkiYearAge = "0";
                    }
                } else {
                    SkiYearAge = "0";
                }
            } else {
                SkiYearAge = "0";
            }

            String EffTo = (String) curImportMemberEntry["EffTo"];
			Boolean CanSki = false;
			if ( curImportMemberEntry["CanSki"] != null ) {
				CanSki = (Boolean) curImportMemberEntry["CanSki"];
			}
			Boolean CanSkiGR = false;
			if ( curImportMemberEntry["CanSkiGR"] != null ) {
				CanSkiGR = (Boolean) curImportMemberEntry["CanSkiGR"];
			}
			String Waiver =((int) curImportMemberEntry["Waiver"]).ToString();

            String JudgeSlalomRating = (String)curImportMemberEntry["JudgeSlalom"];
			if ( curImportMemberEntry.Keys.Contains("JudgePanAmSlalom")) {
				if ( ( (String) curImportMemberEntry["JudgePanAmSlalom"] ).ToLower().Equals( "int" ) ) {
					JudgeSlalomRating = "PanAm";
				}
			}
            String JudgeTrickRating = (String) curImportMemberEntry["JudgeTrick"];
			if ( curImportMemberEntry.Keys.Contains( "JudgePanAmTrick") ) {
				if ( ( (String) curImportMemberEntry["JudgePanAmTrick"] ).ToLower().Equals( "int" ) ) {
					JudgeTrickRating = "PanAm";
				}
			}
            String JudgeJumpRating = (String) curImportMemberEntry["JudgeJump"];
			if ( curImportMemberEntry.Keys.Contains( "JudgePanAmJump" ) ) {
				if ( ( (String) curImportMemberEntry["JudgePanAmJump"] ).ToLower().Equals( "int" ) ) {
					JudgeJumpRating = "PanAm";
				}
			}

            String DriverSlalomRating = (String) curImportMemberEntry["DriverSlalom"];
            String DriverTrickRating = (String) curImportMemberEntry["DriverTrick"];
            String DriverJumpRating = (String) curImportMemberEntry["DriverJump"];

            String ScorerSlalomRating = (String) curImportMemberEntry["ScorerSlalom"];
            String ScorerTrickRating = (String) curImportMemberEntry["ScorerTrick"];
            String ScorerJumpRating = (String) curImportMemberEntry["ScorerJump"];

            String SafetyOfficialRating = (String) curImportMemberEntry["Safety"];
            String TechControllerRating = (String) curImportMemberEntry["TechController"];

            curSqlStmt = new StringBuilder( "Select MemberId, UpdateDate from MemberList Where MemberId = '" + curMemberId + "'");
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
                curSqlStmt.Append("'" + curMemberId + "'");
                curSqlStmt.Append(", '" + LastName + "'");
                curSqlStmt.Append(", '" + FirstName + "'");
                curSqlStmt.Append(", " + SkiYearAge);
                curSqlStmt.Append(", '" + curGender + "'");
                curSqlStmt.Append(", '" + City + "'");
                curSqlStmt.Append(", '" + State + "'");
                curSqlStmt.Append(", '" + Federation + "'");
                curSqlStmt.Append(", '" + MemberStatus + "'");
                curSqlStmt.Append(", '" + JudgeSlalomRating + "'");
                curSqlStmt.Append(", '" + JudgeTrickRating + "'");
                curSqlStmt.Append(", '" + JudgeJumpRating + "'");
                curSqlStmt.Append(", '" + DriverSlalomRating + "'");
                curSqlStmt.Append(", '" + DriverTrickRating + "'");
                curSqlStmt.Append(", '" + DriverJumpRating + "'");
                curSqlStmt.Append(", '" + ScorerSlalomRating + "'");
                curSqlStmt.Append(", '" + ScorerTrickRating + "'");
                curSqlStmt.Append(", '" + ScorerJumpRating + "'");
                curSqlStmt.Append(", '" + SafetyOfficialRating + "'");
                curSqlStmt.Append(", '" + TechControllerRating + "'");
				curSqlStmt.Append( ", '" + Note + "'" );
                curSqlStmt.Append(", '" + EffTo + "'");
                curSqlStmt.Append(", getdate(), getdate() )");

                rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                if ( rowsProc > 0 ) myCountMemberAdded++;
                #endregion

            } else {
                #region Update member data
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Update MemberList ");
                curSqlStmt.Append(" Set LastName = '" + LastName + "'");
                curSqlStmt.Append(", FirstName = '" + FirstName + "'");
                curSqlStmt.Append(", SkiYearAge = " + SkiYearAge);
                curSqlStmt.Append(", Gender = '" + curGender + "'");
                curSqlStmt.Append(", City = '" + City + "'");
                curSqlStmt.Append(", State = '" + State + "'");
                curSqlStmt.Append(", Federation = '" + Federation + "'");
                curSqlStmt.Append(", MemberStatus = '" + MemberStatus + "'");
                curSqlStmt.Append(", Note = 'Imported via OfficalRating process'");
                curSqlStmt.Append(", MemberExpireDate = '" + EffTo + "'");
                curSqlStmt.Append(", UpdateDate = getdate()");
                curSqlStmt.Append(", JudgeSlalomRating = '" + JudgeSlalomRating + "'");
                curSqlStmt.Append(", JudgeTrickRating = '" + JudgeTrickRating + "'");
                curSqlStmt.Append(", JudgeJumpRating = '" + JudgeJumpRating + "'");
                curSqlStmt.Append(", DriverSlalomRating = '" + DriverSlalomRating + "'");
                curSqlStmt.Append(", DriverTrickRating = '" + DriverTrickRating + "'");
                curSqlStmt.Append(", DriverJumpRating = '" + DriverJumpRating + "'");
                curSqlStmt.Append(", ScorerSlalomRating = '" + ScorerSlalomRating + "'");
                curSqlStmt.Append(", ScorerTrickRating = '" + ScorerTrickRating + "'");
                curSqlStmt.Append(", ScorerJumpRating = '" + ScorerJumpRating + "'");
                curSqlStmt.Append(", SafetyOfficialRating = '" + SafetyOfficialRating + "'");
                curSqlStmt.Append(", TechOfficialRating = '" + TechControllerRating + "'");
                curSqlStmt.Append(" Where MemberId = '" + curMemberId + "'");
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
				curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curSlalom.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["SlalomRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curSlalom.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["SlalomRating"] + "'" );
					curSqlStmt.Append( ", '" + curAgeGroup + "'" );
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
				curSqlStmt.Append( "   And MemberId = '" + curMemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curAgeGroup + "'" );
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
				curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curTrick.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["TrickRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curTrick.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["TrickRating"] + "'" );
					curSqlStmt.Append( ", '" + curAgeGroup + "'" );
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
				curSqlStmt.Append( "   And MemberId = '" + curMemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curAgeGroup + "'" );
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
				curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
				curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
				curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
				curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update SkierRanking " );
					curSqlStmt.Append( " Set Score = " + curJump.ToString() );
					curSqlStmt.Append( ", Rating = '" + (String) curImportMemberEntry["JumpRating"] + "'" );
					curSqlStmt.Append( "Where MemberId = '" + curMemberId + "' " );
					curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "'" );
					curSqlStmt.Append( "  And Event = '" + curEvent + "'" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert SkierRanking (" );
					curSqlStmt.Append( "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + curMemberId + "'" );
					curSqlStmt.Append( ", '" + curEvent + "', '', 1" );
					curSqlStmt.Append( ", " + curJump.ToString() );
					curSqlStmt.Append( ", '" + (String) curImportMemberEntry["JumpRating"] + "'" );
					curSqlStmt.Append( ", '" + curAgeGroup + "'" );
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
				curSqlStmt.Append( "   And MemberId = '" + curMemberId + "'" );
				curSqlStmt.Append( "   And AgeGroup = '" + curAgeGroup + "'" );
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
			curSqlStmt.Append( String.Format( "Where TR.MemberId = '{0}' AND TR.SanctionId = '{1}'", curMemberId, curSanctionId ) );
			curMemberDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curMemberDataTable.Rows.Count > 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TourReg Set " );
				curSqlStmt.Append( String.Format( "SkierName = '{0}, {1}'", LastName, FirstName ) );
				curSqlStmt.Append( ", SkiYearAge = " + SkiYearAge );
				curSqlStmt.Append( ", Gender = '" + curGender + "'" );
				curSqlStmt.Append( ", City = '" + City + "'" );
				curSqlStmt.Append( ", State = '" + State + "'" );
				curSqlStmt.Append( ", Federation = '" + Federation + "'" );
				curSqlStmt.Append( ", LastUpdateDate = getdate() " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, curMemberId ) );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) myCountMemberTourUpdated++;

				if ( curMemberDataTable.Rows[0]["OfficialMemberId"] != null ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update OfficialWork Set " );
					curSqlStmt.Append( "JudgeSlalomRating = '" + JudgeSlalomRating + "'" );
					curSqlStmt.Append( ", JudgeTrickRating = '" + JudgeTrickRating + "'" );
					curSqlStmt.Append( ", JudgeJumpRating = '" + JudgeJumpRating + "'" );
					curSqlStmt.Append( ", DriverSlalomRating = '" + DriverSlalomRating + "'" );
					curSqlStmt.Append( ", DriverTrickRating = '" + DriverTrickRating + "'" );
					curSqlStmt.Append( ", DriverJumpRating = '" + DriverJumpRating + "'" );
					curSqlStmt.Append( ", ScorerSlalomRating = '" + ScorerSlalomRating + "'" );
					curSqlStmt.Append( ", ScorerTrickRating = '" + ScorerTrickRating + "'" );
					curSqlStmt.Append( ", ScorerJumpRating = '" + ScorerJumpRating + "'" );
					curSqlStmt.Append( ", SafetyOfficialRating = '" + SafetyOfficialRating + "'" );
					curSqlStmt.Append( ", TechOfficialRating = '" + TechControllerRating + "'" );
					curSqlStmt.Append( ", LastUpdateDate = getdate() " );
					curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, curMemberId ) );
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

		public bool importNcwsMemberFromAwsa( Dictionary<string, object> curImportMemberEntry) {
			String curTeamSlalom = "", curTeamTrick = "", curTeamJump = "";
			bool curDataValid = true;
			Int16 numIntCk;

            String inMemberId = "";
			if ( curImportMemberEntry.ContainsKey( "MemberID" ) ) {
				inMemberId = (String) curImportMemberEntry["MemberID"];
			} else {
				inMemberId = (String) curImportMemberEntry["MemberId"];
			}
			if ( inMemberId.Length == 11 ) {
				inMemberId = inMemberId.Substring( 0, 3 ) + inMemberId.Substring( 4, 2 ) + inMemberId.Substring( 7, 4 );
			}

			String inAgeGroup = "";
			if ( curImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
				inAgeGroup = ( (String) curImportMemberEntry["AgeGroup"] ).ToUpper();
			} else if ( curImportMemberEntry.ContainsKey( "Div" ) ) {
				inAgeGroup = ( (String) curImportMemberEntry["Div"] ).ToUpper();
			}
			String curTeam = ( (String) curImportMemberEntry["Team"] ).ToUpper();

			String inFirstName = (String)curImportMemberEntry[ "FirstName"];
			String inLastName = (String)curImportMemberEntry[ "LastName"];

			String inEventGroupSlalom = ((String)curImportMemberEntry[ "EventSlalom"]).Trim();
			//if ( inEventGroupSlalom.Equals( " " ) || inEventGroupSlalom.Equals( "  " ) ) inEventGroupSlalom = "";
			String inEventGroupTrick = ((String)curImportMemberEntry[ "EventTrick"] ).Trim();
			//if ( inEventGroupTrick.Equals( " " ) || inEventGroupTrick.Equals( "  " ) ) inEventGroupTrick = "";
			String inEventGroupJump = ((String)curImportMemberEntry[ "EventJump"] ).Trim();
			//if ( inEventGroupJump.Equals( " " ) || inEventGroupJump.Equals( "  " ) ) inEventGroupJump = "";

			//For collegiate divisions determine if data is valid for collegiate tournaments
			if ( inAgeGroup.ToUpper().Equals( "CM" )
				|| inAgeGroup.ToUpper().Equals( "CW" )
				|| inAgeGroup.ToUpper().Equals( "BM" )
				|| inAgeGroup.ToUpper().Equals( "BW" )
				) {
				#region Check for rotations for collegiate divisions

				#region Check slalom event group to validate for appropriate collegiate values
				if ( inEventGroupSlalom.ToLower().Equals( "of" ) ) {
					//Skier is an official

				} else {
					if ( inEventGroupSlalom.Length == 1 ) {
						MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;

					} else if ( inEventGroupSlalom.Length > 1 ) {
						if ( inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "A" )
							|| inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "B" )
							) {
							curTeamSlalom = inEventGroupSlalom.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupSlalom.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				#region Check trick event group to validate for appropriate collegiate values
				if ( inEventGroupTrick.ToLower().Equals( "of" ) ) {
					//Skier is an official

				} else {
					if ( inEventGroupTrick.Length == 1 ) {
						MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;

					} else if ( inEventGroupTrick.Length > 1 ) {
						if ( inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "A" )
							|| inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "B" )
							) {
							curTeamTrick = inEventGroupTrick.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupTrick.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				#region Check jump event group to validate for appropriate collegiate values
				if ( inEventGroupJump.ToLower().Equals( "of" ) ) {
					//Skier is an official

				} else {
					if ( inEventGroupJump.Length == 1 ) {
						MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;

					} else if ( inEventGroupJump.Length > 1 ) {
						if ( inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "A" )
							|| inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "B" )
							) {
							curTeamJump = inEventGroupJump.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupJump.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				if ( curDataValid ) {
					#region Check data to determine team assignments (A or B) or just to an age group

					if ( curTeamSlalom.Equals( "" ) ) {
						if ( curTeamTrick.Equals( "" ) ) {
							if ( curTeamJump.Equals( "" ) ) {
								curImportMemberEntry["AgeGroup"] = inAgeGroup;

							} else {
								#region Skier registered for jump only
								if ( curTeamJump.Equals( "A" ) ) {
									curImportMemberEntry["AgeGroup"] = inAgeGroup;

								} else {
									if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
										curImportMemberEntry["AgeGroup"] = "BM";
									} else {
										curImportMemberEntry["AgeGroup"] = "BW";
									}
								}
								#endregion
							}

							importMemberFromAwsa( curImportMemberEntry, true, true );

						} else {
							if ( curTeamJump.Equals( "" ) ) {
								#region Skier registered for trick only
								if ( curTeamTrick.Equals( "A" ) ) {
									curImportMemberEntry["AgeGroup"] = inAgeGroup;

								} else {
									if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
										curImportMemberEntry["AgeGroup"] = "BM";
									} else {
										curImportMemberEntry["AgeGroup"] = "BW";
									}
								}

								importMemberFromAwsa( curImportMemberEntry, true, true );
								#endregion

							} else {
								#region Skier registered for trick and jump
								if ( curTeamTrick.Equals( curTeamJump ) ) {
									// Skier on same team for all entered events
									if ( curTeamTrick.Equals( "A" ) ) {
										curImportMemberEntry["AgeGroup"] = inAgeGroup;

									} else {
										if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
											curImportMemberEntry["AgeGroup"] = "BM";
										} else {
											curImportMemberEntry["AgeGroup"] = "BW";
										}
									}

									importMemberFromAwsa( curImportMemberEntry, true, true );

								} else {
									// Skier assigned to both A and B team
									curDataValid = true;
									createSplitTeamEntries( curImportMemberEntry, inAgeGroup, curTeamSlalom, curTeamTrick, curTeamJump  );
                                }
								#endregion
							}
						}

					} else {
						if ( curTeamTrick.Equals( "" ) ) {
							if ( curTeamJump.Equals( "" ) ) {
								#region Skier registered for slalom only
								if ( curTeamSlalom.Equals( "A" ) ) {
									curImportMemberEntry["AgeGroup"] = inAgeGroup;

								} else {
									if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
										curImportMemberEntry["AgeGroup"] = "BM";
									} else {
										curImportMemberEntry["AgeGroup"] = "BW";
									}
								}

								importMemberFromAwsa( curImportMemberEntry, true, true );
								#endregion

							} else {
								#region Skier registered for slalom and jump
								if ( curTeamSlalom.Equals( curTeamJump ) ) {
									// Skier on same team for all entered events
									if ( curTeamSlalom.Equals( "A" ) ) {
										curImportMemberEntry["AgeGroup"] = inAgeGroup;
									} else {
										if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
											curImportMemberEntry["AgeGroup"] = "BM";
										} else {
											curImportMemberEntry["AgeGroup"] = "BW";
										}
									}

									importMemberFromAwsa( curImportMemberEntry, true, true );

								} else {
									// Skier assigned to both A and B team
									curDataValid = true;
									createSplitTeamEntries( curImportMemberEntry, inAgeGroup, curTeamSlalom, curTeamTrick, curTeamJump );
								}
								#endregion
							}

						} else {
							if ( curTeamJump.Equals( "" ) ) {
								#region Skier is registered in slalom nad trick
								if ( curTeamSlalom.Equals( curTeamTrick ) ) {
									if ( curTeamSlalom.Equals( "A" ) ) {
										curImportMemberEntry["AgeGroup"] = inAgeGroup;

									} else {
										if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
											curImportMemberEntry["AgeGroup"] = "BM";
										} else {
											curImportMemberEntry["AgeGroup"] = "BW";
										}
									}

									importMemberFromAwsa( curImportMemberEntry, true, true );

								} else {
									// Skier assigned to both A and B team
									curDataValid = true;
									createSplitTeamEntries( curImportMemberEntry, inAgeGroup, curTeamSlalom, curTeamTrick, curTeamJump );
								}
								#endregion

							} else {
								#region Skier is registered in all 3 events
								if ( curTeamSlalom.Equals( curTeamTrick ) && curTeamSlalom.Equals( curTeamJump ) ) {
									if ( curTeamSlalom.Equals( "A" ) ) {
										curImportMemberEntry["AgeGroup"] = inAgeGroup;

									} else {
										if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
											curImportMemberEntry["AgeGroup"] = "BM";
										} else {
											curImportMemberEntry["AgeGroup"] = "BW";
										}
									}

									importMemberFromAwsa( curImportMemberEntry, true, true );

								} else {
									// Skier assigned to both A and B team
									curDataValid = true;
									createSplitTeamEntries( curImportMemberEntry, inAgeGroup, curTeamSlalom, curTeamTrick, curTeamJump );
								}
								#endregion
							}
						}
					}
					#endregion

				}
				#endregion

			} else {
				#region Check slalom event group to validate for appropriate alumni events values
				if ( inEventGroupSlalom.ToLower().Equals( "of" ) ) {
					//Skier is an official

				} else {
					if ( inEventGroupSlalom.Length == 1 ) {
						MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;

					} else if ( inEventGroupSlalom.Length > 1 ) {
						if ( inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "A" ) ) {
							curTeamSlalom = inEventGroupSlalom.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupSlalom.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				#region Check trick event group to validate for appropriate alumni events values
				if ( inEventGroupTrick.ToLower().Equals( "of" ) ) {
					//Skier is an official

				} else {
					if ( inEventGroupTrick.Length == 1 ) {
						MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;

					} else if ( inEventGroupTrick.Length > 1 ) {
						if ( inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "A" ) ) {
							curTeamTrick = inEventGroupTrick.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupTrick.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				#region Check jump event group to validate for appropriate alumni events values
				if ( inEventGroupJump.ToLower().Equals( "of" ) ) {
					//Skier is an official
				} else {
					if ( inEventGroupJump.Length == 1 ) {
						MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
							+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
						curDataValid = false;
					} else if ( inEventGroupJump.Length > 1 ) {
						if ( inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "A" ) ) {
							curTeamJump = inEventGroupJump.Substring( 0, 1 ).ToUpper();
							if ( Int16.TryParse( inEventGroupJump.Substring( 1 ), out numIntCk ) ) {
							} else {
								MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
									+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
								curDataValid = false;
							}

						} else {
							MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
								+ "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
							curDataValid = false;
						}
					}
				}
				#endregion

				if ( curDataValid ) {
					#region Check data to determine team assignments (A or B) or just to an age group

					if ( curTeamSlalom.Equals( "" ) ) {
						if ( curTeamTrick.Equals( "" ) ) {
							if ( curTeamJump.Equals( "" ) ) {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
							} else {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
								curTeamJump = inAgeGroup.Substring( 0, 1 );
							}
						} else {
							curTeamTrick = inAgeGroup.Substring( 0, 1 );
							if ( curTeamJump.Equals( "" ) ) {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
							} else {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
								curTeamJump = inAgeGroup.Substring( 0, 1 );
							}
						}
					} else {
						curTeamSlalom = inAgeGroup.Substring( 0, 1 );
						if ( curTeamTrick.Equals( "" ) ) {
							if ( curTeamJump.Equals( "" ) ) {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
							} else {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
								curTeamJump = inAgeGroup.Substring( 0, 1 );
							}
						} else {
							curTeamTrick = inAgeGroup.Substring( 0, 1 );
							if ( curTeamJump.Equals( "" ) ) {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
							} else {
								curImportMemberEntry[ "AgeGroup"] = inAgeGroup;
								curTeamJump = inAgeGroup.Substring( 0, 1 );
							}
						}
					}

					if ( curTeamSlalom.Length == 1 ) {
						curImportMemberEntry[ "EventSlalom"] = curTeamSlalom + inEventGroupSlalom.Substring( 1, inEventGroupSlalom.Length - 1 );
					}
					if ( curTeamTrick.Length == 1 ) {
						curImportMemberEntry["EventTrick"] = curTeamTrick + inEventGroupTrick.Substring( 1, inEventGroupTrick.Length - 1 );
                    }
					if ( curTeamJump.Length == 1 ) {
						curImportMemberEntry[ "EventJump"] = curTeamJump + inEventGroupJump.Substring( 1, inEventGroupJump.Length - 1 );
					}

					importMemberFromAwsa( curImportMemberEntry, true, true );

					#endregion
				}

			}


			return curDataValid;
		}

		private void createSplitTeamEntries( Dictionary<string, object> curImportMemberEntry, String inAgeGroup, String curTeamSlalom, String curTeamTrick, String curTeamJump ) {
			//Clone current attributes to create a second B team entry
			Dictionary<string, object> curBTeamMemberEntry = new Dictionary<string, object>();
			foreach ( KeyValuePair<string, object> curEntry in curImportMemberEntry ) {
				curBTeamMemberEntry.Add( curEntry.Key, curEntry.Value );
			}

			String bteamAgeGroup = "";
			if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "BM" ) ) {
				bteamAgeGroup = "BM";
			} else {
				bteamAgeGroup = "BW";
			}

			//Remove B team assignments for current row and only process A team assignments
			curImportMemberEntry["AgeGroup"] = inAgeGroup;
			curBTeamMemberEntry["AgeGroup"] = bteamAgeGroup;

			if ( curTeamSlalom.Equals( "B" ) ) {
				curBTeamMemberEntry["EventSlalom"] = curImportMemberEntry["EventSlalom"];
				curImportMemberEntry["EventSlalom"] = "";
			} else {
				curBTeamMemberEntry["EventSlalom"] = "";
			}

			if ( curTeamTrick.Equals( "B" ) ) {
				curBTeamMemberEntry["EventTrick"] = curImportMemberEntry["EventTrick"];
				curImportMemberEntry["EventTrick"] = "";
			} else {
				curBTeamMemberEntry["EventTrick"] = "";
			}

			if ( curTeamJump.Equals( "B" ) ) {
				curBTeamMemberEntry["EventJump"] = curImportMemberEntry["EventJump"];
				curImportMemberEntry["EventJump"] = "";
			} else {
				curBTeamMemberEntry["EventJump"] = "";
			}

			importMemberFromAwsa( curImportMemberEntry, true, true );
			importMemberFromAwsa( curBTeamMemberEntry, true, true );
		}

		private String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
            StringBuilder curNewValue = new StringBuilder("");

            String[] curValues = inValue.Split(inCurValue);
            if ( curValues.Length > 1 ) {
                int curCount = 0;
                foreach ( String curValue in curValues ) {
                    curCount++;
                    if ( curCount < curValues.Length ) {
                        curNewValue.Append(curValue + inReplValue);
                    } else {
                        curNewValue.Append(curValue);
                    }
                }
            } else {
                curNewValue.Append(inValue);
            }

            return curNewValue.ToString();
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
