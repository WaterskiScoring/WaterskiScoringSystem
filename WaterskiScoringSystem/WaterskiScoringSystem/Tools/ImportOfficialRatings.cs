using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ImportOfficialRatings {
        private char[] myTabDelim = new char[] { '\t' };
        private char[] mySingleQuoteDelim = new char[] { '\'' };
        private int myCountInput = 0;
        private int myCountMemberAdded = 0;
        private int myCountMemberUpdated = 0;
        private int myCountMemberTourUpdated = 0;
        private int myCountMemberTourOfficialUpdated = 0;

        private DataRow myTourRow;
        private ProgressWindow myProgressInfo = null;
        //private MemberIdValidate myMemberIdValidate;

        public ImportOfficialRatings( DataRow inTourRow ) {
            this.myTourRow = inTourRow;
			myProgressInfo = null;
		}

        public void importMembersAndRatings( List<object> curDataList ) {
            myProgressInfo = new ProgressWindow();
            myProgressInfo.setProgressMin(1);
            myProgressInfo.setProgressMax(curDataList.Count);
            myCountInput = 0;
            myCountMemberAdded = 0;
            myCountMemberUpdated = 0;

            foreach ( Dictionary<string, object> curEntry in curDataList ) {
                myCountInput++;
                importMembersAndRating(curEntry);

                myProgressInfo.setProgressValue(myCountInput);
                myProgressInfo.Refresh();
            }

            myProgressInfo.Close();

            MessageBox.Show(
                  "              Member records procesed: " + myCountInput
                + "\n               Member records added: " + myCountMemberAdded
                + "\n             Member records updated: " + myCountMemberUpdated
                + "\n         Tournament Members updated: " + myCountMemberTourUpdated
                + "\nTournament Official Ratings updated: " + myCountMemberTourOfficialUpdated
                );
        }


        public void importMembersAndRating( Dictionary<string, object> curOfficalEntry ) {
            bool newMember = false;
            StringBuilder curSqlStmt = new StringBuilder("");

            String MemberId = (String) curOfficalEntry["MemberID"];

			if ( MemberId.Length == 11 ) {
				MemberId = MemberId.Substring( 0, 3 ) + MemberId.Substring( 4, 2 ) + MemberId.Substring( 7, 4 );
			}

			String LastName = (String) curOfficalEntry["LastName"];
            LastName = stringReplace(LastName, mySingleQuoteDelim, "''");
            String FirstName = (String) curOfficalEntry["FirstName"];
            FirstName = stringReplace(FirstName, mySingleQuoteDelim, "''");

            String Federation = "USA";
            if ( curOfficalEntry["Federation"] != null ) {
                Federation = (String) curOfficalEntry["Federation"];
            }
			String MemberStatus = "InActive";
			if ( curOfficalEntry["ActiveMember"].GetType() == System.Type.GetType( "System.Boolean" ) ) {
				Boolean ActiveMember = (Boolean) curOfficalEntry["ActiveMember"];
				if ( ActiveMember ) MemberStatus = "Active";

			} else {
				MemberStatus = (String)curOfficalEntry["ActiveMember"];
			}

            String MemTypeDesc = (String) curOfficalEntry["MemTypeDesc"];
			String Note = "Imported";
			if ( curOfficalEntry.ContainsKey( "Note" ) ) {
				if ( curOfficalEntry["Note"] != null ) {
					Note = (String) curOfficalEntry["Note"];
					if ( Note.Length == 0 ) Note = "Imported";
				}
			}

			String Gender = (String) curOfficalEntry["Gender"];
            String City = (String) curOfficalEntry["City"];
            City = stringReplace(City, mySingleQuoteDelim, "''");
            String State = (String) curOfficalEntry["State"];
            String SkiYearAge = ((int) curOfficalEntry["Age"]).ToString();
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

            String EffTo = (String) curOfficalEntry["EffTo"];
			Boolean CanSki = false;
			if ( curOfficalEntry["CanSki"] != null ) {
				CanSki = (Boolean) curOfficalEntry["CanSki"];
			}
			Boolean CanSkiGR = false;
			if ( curOfficalEntry["CanSkiGR"] != null ) {
				CanSkiGR = (Boolean) curOfficalEntry["CanSkiGR"];
			}
			String Waiver =((int) curOfficalEntry["Waiver"]).ToString();

            String JudgeSlalomRating = (String)curOfficalEntry["JudgeSlalom"];
			if ( curOfficalEntry.Keys.Contains("JudgePanAmSlalom")) {
				if ( ( (String) curOfficalEntry["JudgePanAmSlalom"] ).ToLower().Equals( "int" ) ) {
					JudgeSlalomRating = "PanAm";
				}
			}
            String JudgeTrickRating = (String) curOfficalEntry["JudgeTrick"];
			if ( curOfficalEntry.Keys.Contains( "JudgePanAmTrick") ) {
				if ( ( (String) curOfficalEntry["JudgePanAmTrick"] ).ToLower().Equals( "int" ) ) {
					JudgeTrickRating = "PanAm";
				}
			}
            String JudgeJumpRating = (String) curOfficalEntry["JudgeJump"];
			if ( curOfficalEntry.Keys.Contains( "JudgePanAmJump" ) ) {
				if ( ( (String) curOfficalEntry["JudgePanAmJump"] ).ToLower().Equals( "int" ) ) {
					JudgeJumpRating = "PanAm";
				}
			}

            String DriverSlalomRating = (String) curOfficalEntry["DriverSlalom"];
            String DriverTrickRating = (String) curOfficalEntry["DriverTrick"];
            String DriverJumpRating = (String) curOfficalEntry["DriverJump"];

            String ScorerSlalomRating = (String) curOfficalEntry["ScorerSlalom"];
            String ScorerTrickRating = (String) curOfficalEntry["ScorerTrick"];
            String ScorerJumpRating = (String) curOfficalEntry["ScorerJump"];

            String SafetyOfficialRating = (String) curOfficalEntry["Safety"];
            String TechControllerRating = (String) curOfficalEntry["TechController"];

            curSqlStmt = new StringBuilder( "Select MemberId, UpdateDate from MemberList Where MemberId = '" + MemberId + "'");
            DataTable curMemberDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
            if ( curMemberDataTable.Rows.Count <= 0 ) {
                newMember = true;
            }

			if ( myProgressInfo != null ) {
				myProgressInfo.setProgessMsg( "Processing " + FirstName + " " + LastName );
				myProgressInfo.Refresh();
			}

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
                curSqlStmt.Append("'" + MemberId + "'");
                curSqlStmt.Append(", '" + LastName + "'");
                curSqlStmt.Append(", '" + FirstName + "'");
                curSqlStmt.Append(", " + SkiYearAge);
                curSqlStmt.Append(", '" + Gender + "'");
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

                int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                if ( rowsProc > 0 ) myCountMemberAdded++;
                #endregion

            } else {
                #region Update member data
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Update MemberList ");
                curSqlStmt.Append(" Set LastName = '" + LastName + "'");
                curSqlStmt.Append(", FirstName = '" + FirstName + "'");
                curSqlStmt.Append(", SkiYearAge = " + SkiYearAge);
                curSqlStmt.Append(", Gender = '" + Gender + "'");
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
                curSqlStmt.Append(" Where MemberId = '" + MemberId + "'");
                int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                if ( rowsProc > 0 ) myCountMemberUpdated++;
                #endregion
            }

			if ( myTourRow != null ) {
				String curSanctionId = (String) myTourRow["SanctionId"];
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Select TR.MemberId as TourMemberId, OW.MemberId as OfficialMemberId " );
				curSqlStmt.Append( "FROM TourReg TR " );
				curSqlStmt.Append( "LEFT OUTER JOIN OfficialWork OW ON TR.SanctionId = OW.SanctionId AND TR.MemberId = OW.MemberId " );
				curSqlStmt.Append( String.Format( "Where TR.MemberId = '{0}' AND TR.SanctionId = '{1}'", MemberId, curSanctionId ) );
				curMemberDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

				if ( curMemberDataTable.Rows.Count > 0 ) {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update TourReg Set " );
					curSqlStmt.Append( String.Format( "SkierName = '{0}, {1}'", LastName, FirstName ) );
					curSqlStmt.Append( ", SkiYearAge = " + SkiYearAge );
					curSqlStmt.Append( ", Gender = '" + Gender + "'" );
					curSqlStmt.Append( ", City = '" + City + "'" );
					curSqlStmt.Append( ", State = '" + State + "'" );
					curSqlStmt.Append( ", Federation = '" + Federation + "'" );
					curSqlStmt.Append( ", LastUpdateDate = getdate() " );
					curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, MemberId ) );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
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
						curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}'", curSanctionId, MemberId ) );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
						if ( rowsProc > 0 ) myCountMemberTourOfficialUpdated++;

					}
				}
			}
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
