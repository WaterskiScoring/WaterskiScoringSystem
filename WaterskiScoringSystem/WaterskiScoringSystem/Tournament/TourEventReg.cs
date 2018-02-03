using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tournament {
    class TourEventReg {
        private String mySanctionNum;
        private String myMatchCommand = "";
        private String myMatchCommandPrev = "";
        private char[] myTabDelim = new char[] { '\t' };
        private char[] mySingleQuoteDelim = new char[] { '\'' };
        private DataTable myClassToEventDataTable = null;

        private ListSkierClass mySkierClassList;
        private ImportMatchDialogForm myMatchDialog = null;
        private AgeGroupDropdownList myAgeDivList;
        private DataRow myTourRow;

        //Constructor
        public TourEventReg() {
            getTourInfo();

            mySkierClassList = new ListSkierClass();
            mySkierClassList.ListSkierClassLoad();

            myMatchCommand = "";
            myMatchCommandPrev = "";
            myMatchDialog = new ImportMatchDialogForm();

            if ( myTourRow == null ) {
                myAgeDivList = new AgeGroupDropdownList();
            } else {
                String curRules = (String)myTourRow["Rules"];
                myAgeDivList = new AgeGroupDropdownList( myTourRow );
            }
        }

        public DataRow getTourInfo() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            DataTable curTourDataTable = getTourData( mySanctionNum );
            if ( curTourDataTable.Rows.Count > 0 ) {
                myTourRow = curTourDataTable.Rows[0];

                if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
                if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
                if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }
            } else {
                myTourRow = null;
            }
            return myTourRow;
        }

        public bool validAgeDiv( String inAgeDiv ) {
            return myAgeDivList.validAgeDiv(inAgeDiv);
        }

        public String getGenderOfAgeDiv(String inAgeDiv) {
            return myAgeDivList.getGenderOfAgeDiv( inAgeDiv );
        }

        public bool validSkierClass(String inSkierClass, String inTourClass) {
            DataRow[] findRows;

            if (mySkierClassList.SkierClassDataTable == null) {
                return false;
            }
            if (mySkierClassList.SkierClassDataTable.Rows.Count == 0) {
                return false;
            } else {
                findRows = mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + inSkierClass + "'" );
                if (findRows.Length > 0) {
                    if (mySkierClassList.compareClassChange( inSkierClass, inTourClass ) < 0) {
                        return false;
                    } else {
                        return true;
                    }
                }
            }
            return false;
        }

        public String getSkierTourEventClass( String inValue ) {
            String curReturnValue = "";
            DataRow[] findRows;

            if ( myClassToEventDataTable == null ) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListCode, SortSeq, CodeValue, CodeDesc" );
                curSqlStmt.Append( " FROM CodeValueList" );
                curSqlStmt.Append( " WHERE ListName = 'ClassToEvent'" );
                curSqlStmt.Append( " ORDER BY SortSeq" );
				
                myClassToEventDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            }
            if ( myClassToEventDataTable.Rows.Count == 0 ) {
                curReturnValue = "C";
            } else {
                findRows = myClassToEventDataTable.Select( "ListCode = '" + inValue + "'" );
                if ( findRows.Length > 0 ) {
                    curReturnValue = (String)findRows[0]["CodeValue"];
                } else {
                    curReturnValue = "C";
                }
            }
            return curReturnValue;
        }

        public bool addEventSlalom( String inMemberId, String inEventGroup, String inEventClass, String inAgeDiv, String inTeamCode ) {
            if ( myTourRow == null ) {
                return false;
            } else {
                if ( ( (byte)myTourRow["SlalomRounds"] ) > 0 ) {
                    return addEvent( inMemberId, "Slalom", inEventGroup, inEventClass, inAgeDiv, inTeamCode );
                } else {
                    MessageBox.Show( "Request to add skier to slalom event but tournament does not include this event." );
                    return false;
                }
            }
        }

        public bool addEventTrick( String inMemberId, String inEventGroup, String inEventClass, String inAgeDiv, String inTeamCode ) {
            if ( myTourRow == null ) {
                return false;
            } else {
                if ( ( (byte)myTourRow["TrickRounds"] ) > 0 ) {
                    return addEvent( inMemberId, "Trick", inEventGroup, inEventClass, inAgeDiv, inTeamCode );
                } else {
                    MessageBox.Show( "Request to add skier to trick event but tournament does not include this event." );
                    return false;
                }
            }
        }

        public bool addEventJump( String inMemberId, String inEventGroup, String inEventClass, String inAgeDiv, String inTeamCode ) {
            if ( myTourRow == null ) {
                return false;
            } else {
                if ( ( (byte)myTourRow["JumpRounds"] ) > 0 ) {
                    String curAgeDiv = inAgeDiv;
                    String curEventGroup = inEventGroup;
                    if ( curAgeDiv.ToUpper().Equals( "B1" ) ) {
                        curAgeDiv = "B2";
                        if ( inEventGroup.ToUpper().Equals( "B1" ) ) {
                            curEventGroup = curAgeDiv;
                        }
                    } else if ( curAgeDiv.ToUpper().Equals( "G1" ) ) {
                        curAgeDiv = "G2";
                        if ( inEventGroup.ToUpper().Equals( "G1" ) ) {
                            curEventGroup = curAgeDiv;
                        }
                    }
                    return addEvent( inMemberId, "Jump", curEventGroup, inEventClass, curAgeDiv, inTeamCode );
                } else {
                    MessageBox.Show( "Request to add skier to jump event but tournament does not include this event." );
                    return false;
                }
            }
        }

        private bool addEvent( String inMemberId, String inEvent, String inEventGroup, String inEventClass, String inAgeDiv, String inTeamCode ) {
            String curMethodName = "Tournament:TourEventReg:addEvent";
            String curMsg = "";
            Boolean returnStatus = true;
            String curEventGroup, curEventClass, curTeamCode, curRankingRating = "";
            Decimal curRankingScore = 0, curHCapBase = 0, curHCapScore = 0;
            DataRow curTourRegRow, curTourEventRegRow;
            DataRow curSkierRankingRow;
            DataTable curSkierRankingDataTable;
            DataTable curTourEventRegDataTable;

            try {
                if ( inAgeDiv.ToUpper().Equals( "OF" ) ) {
                    returnStatus = false;
                    curMsg = "Member is registered as an official only participant.  Age division must be updated before registering this member for an event";
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                } else if ( inAgeDiv.Trim().Length < 2 ) {
                    returnStatus = false;
                    curMsg = "Member " + inMemberId + " must have a valid division"
                        + "\n Unable to add to event " + inEvent;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
                if ( returnStatus ) {
                    curTourRegRow = getTourMemberRow( mySanctionNum, inMemberId, inAgeDiv );
                    if (curTourRegRow != null) {
                        curTourEventRegDataTable = getDataBySkierEvent( mySanctionNum, inMemberId, inAgeDiv, inEvent );
                        String curReadyForPlcmt = (String)curTourRegRow["ReadyForPlcmt"];

                        if ( inEventClass.Trim().Length > 0 ) {
                            if (validSkierClass( inEventClass, (String)myTourRow["Class"] )) {
                                curEventClass = inEventClass;
                            } else {
                                curEventClass = getSkierTourEventClass( (String)myTourRow["Class"] );
                            }
                            if (curEventClass.ToUpper().Equals( "R" )) {
                                if (( (String)myTourRow["Rules"] ).ToUpper().Equals( "IWWF" )) {
                                    //Leave class as provided on input
                                } else {
                                    if (inAgeDiv.Equals( "OM" ) || inAgeDiv.Equals( "OW" )) {
                                        curEventClass = "R";
                                    } else if (inAgeDiv.Equals( "B1" ) || inAgeDiv.Equals( "G1" )
                                        || inAgeDiv.Equals( "B2" ) || inAgeDiv.Equals( "G2" )
                                        || inAgeDiv.Equals( "M8" ) || inAgeDiv.Equals( "W8" )
                                        || inAgeDiv.Equals( "M9" ) || inAgeDiv.Equals( "W9" )
                                        || inAgeDiv.Equals( "MA" ) || inAgeDiv.Equals( "WA" )
                                        || inAgeDiv.Equals( "MB" ) || inAgeDiv.Equals( "WB" )
                                       ) {
                                        curEventClass = "E";
                                    } else {
                                        curEventClass = "L";
                                    }
                                }
                            }

                        } else {
                            curEventClass = getSkierTourEventClass( (String)myTourRow["Class"] );
                            if (( (String)myTourRow["Class"] ).Equals( "A" )
                                || ( (String)myTourRow["Class"] ).Equals( "B" )
                                || ( (String)myTourRow["Class"] ).Equals( "R" )) {
                                    if (( (String)myTourRow["Rules"] ).ToUpper().Equals( "IWWF" )) {
                                        //Leave class as provided on input
                                    } else {
                                        if (inAgeDiv.Equals( "OM" ) || inAgeDiv.Equals( "OW" )) {
                                            curEventClass = "R";
                                        } else if (inAgeDiv.Equals( "B1" ) || inAgeDiv.Equals( "G1" )
                                            || inAgeDiv.Equals( "B2" ) || inAgeDiv.Equals( "G2" )
                                            || inAgeDiv.Equals( "M8" ) || inAgeDiv.Equals( "W8" )
                                            || inAgeDiv.Equals( "M9" ) || inAgeDiv.Equals( "W9" )
                                            || inAgeDiv.Equals( "MA" ) || inAgeDiv.Equals( "WA" )
                                            || inAgeDiv.Equals( "MB" ) || inAgeDiv.Equals( "WB" )
                                           ) {
                                            if (curEventClass.ToUpper().Equals( "C" )) {
                                            } else {
                                                curEventClass = "E";
                                            }
                                        } else {
                                            if (curEventClass.ToUpper().Equals( "C" ) || curEventClass.ToUpper().Equals( "E" )) {
                                            } else {
                                                curEventClass = "L";
                                            }
                                        }
                                    }
                            }
                        }
                        if ( inEventGroup.Trim().Length > 0 ) {
                            curEventGroup = inEventGroup;
                        } else {
                            curEventGroup = inAgeDiv;
                        }

                        //Get current ranking data for use in tournamnet
                        curSkierRankingDataTable = getSkierByEvent( inMemberId, inEvent, inAgeDiv );
                        if ( curSkierRankingDataTable.Rows.Count > 0 ) {
                            curSkierRankingRow = curSkierRankingDataTable.Rows[0];
                            curRankingRating = (String)curSkierRankingRow["Rating"];
                            curRankingScore = (Decimal)curSkierRankingRow["Score"];
                            curHCapBase = curRankingScore;
                            curHCapScore = calcCapScore( inEvent, curRankingScore );
                        }

                        StringBuilder curSqlStmt = new StringBuilder( "" );
                        if ( curTourEventRegDataTable.Rows.Count == 0 ) {
                            curSqlStmt.Append( "Insert EventReg (" );
                            curSqlStmt.Append( "SanctionId, MemberId, Event, EventGroup, TeamCode, EventClass" );
                            curSqlStmt.Append( ", RunOrder, RankingScore, RankingRating, AgeGroup, ReadyForPlcmt" );
                            curSqlStmt.Append( ", HCapBase, HCapScore, LastUpdateDate" );
                            curSqlStmt.Append( ") Values (" );
                            curSqlStmt.Append( "'" + mySanctionNum + "'" );
                            curSqlStmt.Append( ", '" + inMemberId + "'" );
                            curSqlStmt.Append( ", '" + inEvent + "'" );
                            curSqlStmt.Append( ", '" + curEventGroup + "'" );
                            curSqlStmt.Append( ", '" + inTeamCode + "'" );
                            curSqlStmt.Append( ", '" + curEventClass + "'" );
                            curSqlStmt.Append( ", '1'" );
                            curSqlStmt.Append( ", " + curRankingScore );
                            curSqlStmt.Append( ", '" + curRankingRating + "'" );
                            curSqlStmt.Append( ", '" + inAgeDiv + "'" );
                            curSqlStmt.Append( ", '" + curReadyForPlcmt + "'" );
                            curSqlStmt.Append( ", " + curHCapBase );
                            curSqlStmt.Append( ", " + curHCapScore );
                            curSqlStmt.Append( ", GETDATE() )" );
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            returnStatus = true;
                        } else {
                            #region Show information if input data found on database
                            //Show information if input data found on database
                            //Skip display if previoius display specfied to process all records the same
                            if ( myMatchCommand.Length < 2 ) {
                                String prevEventGroup = "", prevEventClass = "", prevRankingScore = "", prevRankingRating = "", prevHCapBase = "", prevHCapScore = "";
                                curTourEventRegRow = curTourEventRegDataTable.Rows[0];
                                try {
                                    prevEventGroup = (String)curTourEventRegRow["EventGroup"];
                                } catch {
                                    prevEventGroup = "";
                                }
                                try {
                                    prevEventClass = (String)curTourEventRegRow["EventClass"];
                                } catch {
                                    prevEventClass = "";
                                }
                                try {
                                    prevRankingRating = (String)curTourEventRegRow["RankingRating"];
                                } catch {
                                    prevRankingRating = "";
                                }
                                try {
                                    prevRankingScore = ( (Decimal)curTourEventRegRow["RankingScore"] ).ToString();
                                } catch {
                                    prevRankingScore = "";
                                }
                                try {
                                    prevHCapBase = ( (Decimal)curTourEventRegRow["HCapBase"] ).ToString();
                                } catch {
                                    prevHCapBase = "";
                                }
                                try {
                                    prevHCapScore = ( (Decimal)curTourEventRegRow["HCapScore"] ).ToString();
                                } catch {
                                    prevHCapScore = "";
                                }
                                String [] curMessage = new String[8];
                                curMessage[0] = "Skier " + curTourRegRow["SkierName"] + ", " + inMemberId + ", " + inAgeDiv;
                                curMessage[1] = "is already registered for " + inEvent + " event";
                                curMessage[2] = "   EventGroup: Current=" + prevEventGroup + " : Import=" + curEventGroup;
                                curMessage[3] = "   EventClass: Current=" + prevEventClass + " : Import=" + curEventClass;
                                curMessage[4] = " RankingScore: Current=" + prevRankingScore + " : Import=" + curRankingScore;
                                curMessage[5] = "RankingRating: Current=" + prevRankingRating + " : Import=" + curRankingRating;
                                curMessage[6] = "     HCapBase: Current=" + prevHCapBase + " : Import=" + curHCapBase;
                                curMessage[7] = "    HCapScore: Current=" + prevHCapScore + " : Import=" + curHCapScore;
                                myMatchDialog.ImportKeyDataMultiLine = curMessage;

                                myMatchDialog.MatchCommand = myMatchCommand;
                                if ( myMatchDialog.ShowDialog() == DialogResult.OK ) {
                                    myMatchCommand = myMatchDialog.MatchCommand;
                                    myMatchCommandPrev = myMatchCommand;
                                }
                            }
                            if ( myMatchCommand.ToLower().Equals( "update" )
                                || myMatchCommand.ToLower().Equals( "updateall" ) ) {
                                curSqlStmt.Append( "Update EventReg Set " );
                                curSqlStmt.Append( "EventGroup = '" + curEventGroup + "', " );
                                curSqlStmt.Append( "TeamCode = '" + inTeamCode + "', " );
                                curSqlStmt.Append( "EventClass = '" + curEventClass + "', " );
                                curSqlStmt.Append( "RunOrder = '1', " );
                                curSqlStmt.Append( "RankingScore = " + curRankingScore + ", " );
                                curSqlStmt.Append( "RankingRating = '" + curRankingRating + "', " );
                                curSqlStmt.Append( "HCapBase = " + curHCapBase + ", " );
                                curSqlStmt.Append( "HCapScore = " + curHCapScore + ", " );
                                curSqlStmt.Append( "LastUpdateDate = GETDATE() " );
                                curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
                                curSqlStmt.Append( "  AND AgeGroup = '" + inAgeDiv + "' AND Event = '" + inEvent + "'" );
                                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                returnStatus = true;
                            } else {
                                returnStatus = false;
                                //Re-initialize dialog response unless specified to process rows
                                if ( myMatchCommand.ToLower().Equals( "skip" ) ) {
                                    myMatchCommand = "";
                                }
                            }
                            #endregion
                        }
                    } else {
                        returnStatus = false;
                        curMsg = "Member " + inMemberId + " is not registered in tournament"
                            + "\n Unable to add to event " + inEvent ;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                }
            } catch ( Exception ex ) {
                curMsg = "Exception encountered adding skier to " + inEvent + " event \n" + ex.Message ;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
                return false;
            }
            return returnStatus;
        }

        public bool addTourReg( String inMemberId, String inPreRegNote, String inAgeGroup, String inTrickBoat, String inJumpHeight ) {
            String curMethodName = "Tournament:TourEventReg:addTourReg";
            String curMsg = "";
            String curReadyToSki = "", curReadyForPlcmt = "", curJumpHeight = "0", curFed = "", curCity = "", curState = "", curGender = "";
            String curFirstName = "", curLastName = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            Int16 curSkiYearAge = 0;

			try {
				if ( myTourRow == null ) return false;
				if ( getTourMemberRow( mySanctionNum, inMemberId, inAgeGroup ) != null ) return false;

				DataRow curMemberRow = getMemberRow( inMemberId );
				if ( curMemberRow == null ) return false;

				#region Add skier to active tournament for the specified event but bypass if already registered for the event
				try {
					if ( ( (String) curMemberRow["MemberStatus"] ).ToLower().Equals( "active" ) ) {
						curReadyToSki = "Y";
					} else {
						curReadyToSki = "N";
					}
				} catch {
					curReadyToSki = "N";
				}
				curReadyForPlcmt = curReadyToSki;

				try {
					if ( inJumpHeight.Length == 0 ) {
						curJumpHeight = "0";
					} else {
						curJumpHeight = inJumpHeight;
					}
				} catch {
					curJumpHeight = "0";
				}
				try {
					curFed = (String) curMemberRow["Federation"];
				} catch {
					curFed = "";
				}
				try {
					curCity = (String) curMemberRow["City"];
				} catch {
					curCity = "";
				}
				try {
					curState = (String) curMemberRow["State"];
				} catch {
					curState = "";
				}
				try {
					curGender = (String) curMemberRow["Gender"];
				} catch {
					curGender = "";
				}
				try {
					curSkiYearAge = (Byte) curMemberRow["SkiYearAge"];
				} catch {
					curSkiYearAge = 0;
				}
				if ( curSkiYearAge < 1 ) {
					try {
						Int16 curBirthYear = Convert.ToInt16( Convert.ToDateTime( curMemberRow["DateOfBirth"].ToString() ).ToString( "yy" ) );
						Int16 curTourYear = Convert.ToInt16( mySanctionNum.Substring( 0, 2 ) );
						curTourYear += 100;
						curSkiYearAge = Convert.ToInt16( curTourYear - curBirthYear );
					} catch {
						curSkiYearAge = 0;
					}
				}
				try {
					curLastName = ( (String) curMemberRow["LastName"] ).Replace( "'", "''" );
				} catch {
					curLastName = "";
				}
				try {
					curFirstName = ( (String) curMemberRow["FirstName"] ).Replace( "'", "''" );
				} catch {
					curFirstName = "";
				}
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert TourReg (" );
				curSqlStmt.Append( " MemberId, SanctionId, SkierName, AgeGroup, ReadyToSki, ReadyForPlcmt, TrickBoat, JumpHeight " );
				curSqlStmt.Append( " , Federation, Gender, City, State, SkiYearAge, Notes, LastUpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + inMemberId + "'" );
				curSqlStmt.Append( ", '" + mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + stringReplace( curLastName, mySingleQuoteDelim, "''" ) + ", " + stringReplace( curFirstName, mySingleQuoteDelim, "''" ) + "'" );
				curSqlStmt.Append( ", '" + inAgeGroup + "'" );
				curSqlStmt.Append( ", '" + curReadyToSki + "'" );
				curSqlStmt.Append( ", '" + curReadyForPlcmt + "'" );
				curSqlStmt.Append( ", '" + inTrickBoat + "'" );
				curSqlStmt.Append( ", " + curJumpHeight );
				curSqlStmt.Append( ", '" + curFed + "'" );
				curSqlStmt.Append( ", '" + curGender + "'" );
				curSqlStmt.Append( ", '" + stringReplace( curCity, mySingleQuoteDelim, "''" ) + "'" );
				curSqlStmt.Append( ", '" + curState + "'" );
				curSqlStmt.Append( ", " + curSkiYearAge );
				curSqlStmt.Append( ", '" + inPreRegNote + "'" );
				curSqlStmt.Append( ", getdate() )" );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				DataRow curOfficialRow = getOfficialWorkRow( inMemberId );
				if ( curOfficialRow == null ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "INSERT INTO OfficialWork ( " );
					curSqlStmt.Append( "SanctionId, MemberId, LastUpdateDate" );
					curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
					curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
					curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
					curSqlStmt.Append( ", SafetyOfficialRating, TechOfficialRating, AnncrOfficialRating " );
					curSqlStmt.Append( ") VALUES ( " );
					curSqlStmt.Append( "'" + mySanctionNum + "'" );
					curSqlStmt.Append( ", '" + inMemberId + "'" );
					curSqlStmt.Append( ", getdate()" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["JudgeSlalomRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["JudgeTrickRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["JudgeJumpRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["DriverSlalomRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["DriverTrickRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["DriverJumpRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["ScorerSlalomRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["ScorerTrickRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["ScorerJumpRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["SafetyOfficialRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["TechOfficialRating"] + "'" );
					curSqlStmt.Append( ", '" + (String) curMemberRow["AnncrOfficialRating"] + "'" );
					curSqlStmt.Append( " )" );

					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
					if ( rowsProc == 0 ) {
						Log.WriteFile( curMethodName + ":OfficialWork record not added when registering member " + inMemberId + " tournament " + mySanctionNum );
					}

				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update OfficialWork Set " );
					curSqlStmt.Append( "JudgeSlalomRating = '" + (String) curMemberRow["JudgeSlalomRating"] + "'" );
					curSqlStmt.Append( ", JudgeTrickRating = '" + (String) curMemberRow["JudgeTrickRating"] + "'" );
					curSqlStmt.Append( ", JudgeJumpRating = '" + (String) curMemberRow["JudgeJumpRating"] + "'" );
					curSqlStmt.Append( ", DriverSlalomRating = '" + (String) curMemberRow["DriverSlalomRating"] + "'" );
					curSqlStmt.Append( ", DriverTrickRating = '" + (String) curMemberRow["DriverTrickRating"] + "'" );
					curSqlStmt.Append( ", DriverJumpRating = '" + (String) curMemberRow["DriverJumpRating"] + "'" );
					curSqlStmt.Append( ", ScorerSlalomRating = '" + (String) curMemberRow["ScorerSlalomRating"] + "'" );
					curSqlStmt.Append( ", ScorerTrickRating = '" + (String) curMemberRow["ScorerTrickRating"] + "'" );
					curSqlStmt.Append( ", ScorerJumpRating = '" + (String) curMemberRow["ScorerJumpRating"] + "'" );
					curSqlStmt.Append( ", SafetyOfficialRating = '" + (String) curMemberRow["SafetyOfficialRating"] + "'" );
					curSqlStmt.Append( ", TechOfficialRating = '" + (String) curMemberRow["TechOfficialRating"] + "'" );
					curSqlStmt.Append( ", AnncrOfficialRating = '" + (String) curMemberRow["AnncrOfficialRating"] + "'" );
					curSqlStmt.Append( ", LastUpdateDate = getdate() " );
					curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", mySanctionNum, inMemberId ) );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				}

				return true;
				#endregion

			} catch ( Exception ex ) {
				curMsg = " Exception encountered adding skier to tournament \n" + ex.Message + "\n\n" + curSqlStmt.ToString();
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
			}
		}

		public bool addEventOfficial( String inMemberId, String inOfficalPosition ) {
            String curMethodName = "Tournament:TourEventReg:addEventOfficial";
            String curMsg = "";
            Boolean returnStatus = false;
            int rowsProc = 0;
            DataRow curTourRegRow;
            DataRow curOfficialRow;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                curTourRegRow = getTourMemberRow( mySanctionNum, inMemberId, "" );
                if (curTourRegRow != null ) {
                    curOfficialRow = getOfficialWorkRow( inMemberId );
                    if (curOfficialRow == null) {
                        curSqlStmt = new StringBuilder( "" );
						curSqlStmt.Append( "INSERT INTO OfficialWork ( " );
						curSqlStmt.Append( "SanctionId, MemberId" );
						curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
						curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
						curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
						curSqlStmt.Append( ", SafetyOfficialRating, TechOfficialRating, AnncrOfficialRating " );
						curSqlStmt.Append( ") SELECT Distinct " );
						curSqlStmt.Append( "  TourReg.SanctionId, TourReg.MemberId" );
						curSqlStmt.Append( ", Coalesce( MemberList.JudgeSlalomRating, '' ) as JudgeSlalomRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.JudgeTrickRating, '' ) as JudgeTrickRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.JudgeJumpRating, '' ) as JudgeJumpRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.DriverSlalomRating, '' ) as DriverSlalomRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.DriverTrickRating, '' ) as DriverTrickRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.DriverJumpRating, '' ) as DriverJumpRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.ScorerSlalomRating, '' ) as ScorerSlalomRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.ScorerTrickRating, '' ) as ScorerTrickRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.ScorerJumpRating, '' ) as ScorerJumpRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.SafetyOfficialRating, '' ) as SafetyOfficialRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.TechOfficialRating, '' ) as TechOfficialRating" );
						curSqlStmt.Append( ", Coalesce( MemberList.AnncrOfficialRating, '' ) as AnncrOfficialRating " );
						curSqlStmt.Append( "FROM TourReg " );
						curSqlStmt.Append( "	INNER JOIN MemberList ON MemberList.MemberId = TourReg.MemberId  " );
						curSqlStmt.Append( "WHERE TourReg.SanctionId = '" + mySanctionNum + "'" );
						curSqlStmt.Append( "  AND TourReg.MemberId = '" + inMemberId + "'" );

						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
						Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
						if ( rowsProc == 0 ) {
							Log.WriteFile( curMethodName + ":OfficialWork record not added when registering member " + inMemberId + " tournament " + mySanctionNum );
						}
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update OfficialWork " );
                    curSqlStmt.Append( "set " + inOfficalPosition + " = 'Y'" );
                    curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                    curSqlStmt.Append( " Where MemberId = '" + inMemberId + "'" );
                    curSqlStmt.Append( " And SanctionId = '" + mySanctionNum + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    if (inOfficalPosition.ToLower().Equals("judgechief") || inOfficalPosition.ToLower().Equals("judgeasstchief" ) || inOfficalPosition.ToLower().Equals( "judgeappointed" ) ) {
                        if (inOfficalPosition.ToLower().Equals("judgechief")) {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update Tournament ");
                            curSqlStmt.Append("set ChiefJudgeMemberId = '" + inMemberId + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                            curSqlStmt.Append(" Where SanctionId = '" + mySanctionNum + "'");
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update OfficialWork " );
                        curSqlStmt.Append( "set JudgeSlalomCredit = 'Y'" );
                        curSqlStmt.Append( ", JudgeTrickCredit = 'Y'" );
                        curSqlStmt.Append( ", JudgeJumpCredit = 'Y'" );
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append( " Where MemberId = '" + inMemberId + "'" );
                        curSqlStmt.Append( " And SanctionId = '" + mySanctionNum + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					} else if (inOfficalPosition.ToLower().Equals("driverchief") || inOfficalPosition.ToLower().Equals("driverasstchief" ) || inOfficalPosition.ToLower().Equals( "driverappointed" ) ) {
                        if (inOfficalPosition.ToLower().Equals("driverchief")) {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update Tournament ");
                            curSqlStmt.Append("set ChiefDriverMemberId = '" + inMemberId + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                            curSqlStmt.Append(" Where SanctionId = '" + mySanctionNum + "'");
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update OfficialWork " );
                        curSqlStmt.Append( "set DriverSlalomCredit = 'Y'" );
                        curSqlStmt.Append( ", DriverTrickCredit = 'Y'" );
                        curSqlStmt.Append( ", DriverJumpCredit = 'Y'" );
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append( " Where MemberId = '" + inMemberId + "'" );
                        curSqlStmt.Append( " And SanctionId = '" + mySanctionNum + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					} else if (inOfficalPosition.ToLower().Equals("scorechief") || inOfficalPosition.ToLower().Equals("scoreasstchief" ) || inOfficalPosition.ToLower().Equals( "scorerappointed" ) ) {
                        if (inOfficalPosition.ToLower().Equals("scorechief")) {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update Tournament ");
                            curSqlStmt.Append("set ChiefScorerMemberId = '" + inMemberId + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                            curSqlStmt.Append(" Where SanctionId = '" + mySanctionNum + "'");
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update OfficialWork " );
                        curSqlStmt.Append( "set ScoreSlalomCredit = 'Y'" );
                        curSqlStmt.Append( ", ScoreTrickCredit = 'Y'" );
                        curSqlStmt.Append( ", ScoreJumpCredit = 'Y'" );
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append( " Where MemberId = '" + inMemberId + "'" );
                        curSqlStmt.Append( " And SanctionId = '" + mySanctionNum + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					} else if (inOfficalPosition.ToLower().Equals("safetychief") || inOfficalPosition.ToLower().Equals("safetyasstchief")) {
                        if (inOfficalPosition.ToLower().Equals("safetychief")) {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update Tournament ");
                            curSqlStmt.Append("set SafetyDirMemberId = '" + inMemberId + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                            curSqlStmt.Append(" Where SanctionId = '" + mySanctionNum + "'");
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update OfficialWork " );
                        curSqlStmt.Append( "set SafetySlalomCredit = 'Y'" );
                        curSqlStmt.Append( ", SafetyTrickCredit = 'Y'" );
                        curSqlStmt.Append( ", SafetyJumpCredit = 'Y'" );
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append( " Where MemberId = '" + inMemberId + "'" );
                        curSqlStmt.Append( " And SanctionId = '" + mySanctionNum + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					} else if (inOfficalPosition.ToLower().Equals("techchief") || inOfficalPosition.ToLower().Equals("techasstchief")) {
                        curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("Update OfficialWork ");
                        curSqlStmt.Append("set TechSlalomCredit = 'Y'");
                        curSqlStmt.Append(", TechTrickCredit = 'Y'");
                        curSqlStmt.Append(", TechJumpCredit = 'Y'");
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append(" Where MemberId = '" + inMemberId + "'");
                        curSqlStmt.Append(" And SanctionId = '" + mySanctionNum + "'");
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }

                    returnStatus = true;

                } else {
                    returnStatus = false;
                    curMsg = "Member " + inMemberId + " is not registered in tournament"
                        + "\n Unable to add as " + inOfficalPosition ;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }

            } catch ( Exception ex ) {
                curMsg = "Exception encountered adding skier to " + inOfficalPosition + " event \n" + ex.Message ;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
                return false;
            }

            return returnStatus;
        }

        public bool deleteSlalomEntry( String inMemberId, String inAgeGroup ) {
            String curMethodName = "Tournament:TourEventReg:deleteSlalomEntry";
            String curMsg = "";
            bool returnStatus = true;
            String curEvent = "Slalom";
            int rowsProc = 0;
            
            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select PK From SlalomScore " );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                curSqlStmt.Append( " Union" );
                curSqlStmt.Append( " Select PK From SlalomRecap" );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
                    String dialogMsg = "Skier has " + curEvent + " scores!\nDo you still want to remove skier from " + curEvent + " event?";
                    DialogResult msgResp =
                        MessageBox.Show( dialogMsg, "Delete Warning",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
                    if ( msgResp == DialogResult.Yes ) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete SlalomRecap " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete SlalomScore " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        returnStatus = true;
                    } else {
                        returnStatus = false;
                    }
                }
                if ( returnStatus ) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventReg " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventRunOrder " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    returnStatus = true;
                }
            } catch ( Exception excp ) {
                returnStatus = false;
                curMsg = "Error attempting to remove skier from " + curEvent + " event including scores \n" + excp.Message ;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return returnStatus;
        }

        public bool deleteTrickEntry( String inMemberId, String inAgeGroup ) {
            String curMethodName = "Tournament:TourEventReg:deleteTrickEntry";
            String curMsg = "";
            bool returnStatus = true;
            String curEvent = "Trick";
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select PK From TrickScore " );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                curSqlStmt.Append( " Union" );
                curSqlStmt.Append( " Select PK From TrickPass" );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
                    String dialogMsg = "Skier has " + curEvent + " scores!\nDo you still want to remove skier from " + curEvent + " event?";
                    DialogResult msgResp =
                        MessageBox.Show( dialogMsg, "Delete Warning",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
                    if ( msgResp == DialogResult.Yes ) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete TrickScore " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete TrickPass " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        returnStatus = true;
                    } else {
                        returnStatus = false;
                    }
                }
                if ( returnStatus ) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventReg " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventRunOrder " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    returnStatus = true;
                }
            } catch ( Exception excp ) {
                returnStatus = false;
                curMsg = "Error attempting to remove skier from " + curEvent + " event including scores \n" + excp.Message ;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return returnStatus;
        }

        public bool deleteJumpEntry( String inMemberId, String inAgeGroup ) {
            String curMethodName = "Tournament:TourEventReg:deleteJumpEntry";
            String curMsg = "";
            bool returnStatus = true;
            String curEvent = "Jump";
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select PK From JumpScore " );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                curSqlStmt.Append( " Union" );
                curSqlStmt.Append( " Select PK From JumpRecap" );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
                    String dialogMsg = "Skier has " + curEvent + " scores!\nDo you still want to remove skier from " + curEvent + " event?";
                    DialogResult msgResp =
                        MessageBox.Show( dialogMsg, "Delete Warning",
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
                    if ( msgResp == DialogResult.Yes ) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete JumpScore " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Delete JumpRecap " );
                        curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "'" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        returnStatus = true;
                    } else {
                        returnStatus = false;
                    }
                }
                if ( returnStatus ) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventReg " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventRunOrder " );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' And Event = '" + curEvent + "'" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    returnStatus = true;
                }
            } catch ( Exception excp ) {
                returnStatus = false;
                curMsg = "Error attempting to remove skier from " + curEvent + " event including scores \n" + excp.Message ;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return returnStatus;
        }

        public Decimal calcCapScore( String inEvent, Decimal inRankingScore ) {
            Decimal curHcapScore = 0;
            Decimal curHcapBase = (Decimal)myTourRow["Hcap" + inEvent + "Base"];
            Decimal curHcapPct = (Decimal)myTourRow["Hcap" + inEvent + "Pct"];

            if ( curHcapBase > 0 ) {
                curHcapScore = ( curHcapBase - inRankingScore ) * curHcapPct;
                /*
                if ( curHcapBase > inRankingScore ) {
                    curHcapScore = ( curHcapBase - inRankingScore ) * curHcapPct;
                } else {
                    curHcapScore = Convert.ToDecimal( "0" );
                }
                */
            } else {
                curHcapScore = Convert.ToDecimal( "0" );
            }
            return curHcapScore;
        }

        private DataTable getDataBySkierEvent( String inSanctionId, String inMemberId, String inAgeGroup, String inEvent ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.SanctionId, E.MemberId, E.AgeGroup, E.Event, E.EventGroup, E.RunOrder, E.TeamCode, " );
            curSqlStmt.Append(" E.EventClass, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt, E.RankingScore, E.RankingRating, E.HCapBase, E.HCapScore, R.SkierName ");
            curSqlStmt.Append( " FROM EventReg E" );
            curSqlStmt.Append( "      INNER JOIN TourReg R ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup" );
            curSqlStmt.Append( " WHERE E.SanctionId = '" + inSanctionId + "' AND E.MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND E.AgeGroup = '" + inAgeGroup + "' AND E.Event = '" + inEvent + "'" );
            curSqlStmt.Append( " ORDER BY SkierName " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

        private DataRow getTourMemberRow( String inSanctionId, String inMemberId, String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT PK, MemberId, SanctionId, SkierName, AgeGroup, COALESCE(ReadyForPlcmt, 'N') as ReadyForPlcmt, ");
            curSqlStmt.Append(" EntryDue, EntryPaid, PaymentMethod, ReadyToSki, ReadyForPlcmt, AwsaMbrshpPaymt, ");
            curSqlStmt.Append( " AwsaMbrshpComment, Weight, TrickBoat, JumpHeight, Notes" );
            curSqlStmt.Append( " FROM TourReg " );
            curSqlStmt.Append( " WHERE SanctionId = '" + inSanctionId + "' AND MemberId = '" + inMemberId + "'" );
            if ( inAgeGroup.Length > 0 ) {
                curSqlStmt.Append( " AND AgeGroup = '" + inAgeGroup + "'" );
            }
            curSqlStmt.Append( " ORDER BY SkierName " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataRow getMemberRow( String inMemberId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId, LastName, FirstName, Address1, Address2, City, State, Federation, Country, Postalcode" );
            curSqlStmt.Append( ", SkiYearAge, DateOfBirth, Gender, MemberStatus, MemberExpireDate" );
			curSqlStmt.Append( ", Coalesce( JudgeSlalomRating, '' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( JudgeTrickRating, '' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( JudgeJumpRating, '' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( ScorerSlalomRating, '' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( ScorerTrickRating, '' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( ScorerJumpRating, '' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( DriverSlalomRating, '' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( DriverTrickRating, '' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( DriverJumpRating, '' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( SafetyOfficialRating, '' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( TechOfficialRating, '' ) as TechOfficialRating" );
			curSqlStmt.Append( ", Coalesce( AnncrOfficialRating, '' ) as AnncrOfficialRating" );
			curSqlStmt.Append( ", InsertDate, UpdateDate " );
            curSqlStmt.Append( "FROM MemberList " );
            curSqlStmt.Append( "WHERE MemberId = '" + inMemberId + "'" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

		private DataRow getOfficialWorkRow( String inMemberId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, T.SkierName AS MemberName " );
            curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) T " );
            curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' AND O.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "ORDER BY O.SanctionId, O.MemberId, T.SkierName" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if (curDataTable.Rows.Count > 0) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getSkierByEvent(String inMemberId, String inEvent, String inAgeGroup) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * From SkierRanking Where MemberId = '" + inMemberId + "' ");
            curSqlStmt.Append( "And AgeGroup = '" + inAgeGroup + "' And Event = '" + inEvent + "'" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }
        
        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, Name, Class, Federation, TourDataLoc, LastUpdateDate, " );
            curSqlStmt.Append( "    SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation, " );
            curSqlStmt.Append( "    HcapSlalomBase, HcapTrickBase, HcapJumpBase, HcapSlalomPct, HcapTrickPct, HcapJumpPct " );
            curSqlStmt.Append( "FROM Tournament " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
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

    }
}
