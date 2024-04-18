using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    class CalcEventPlcmt {
        private String mySanctionNum;

        public CalcEventPlcmt() {
        }

        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setSlalomPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "", 0 );
        }
        public DataTable setSlalomPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, Int16 inNumPrelimRounds ) {
            /* **********************************************************
             * Use results from slalom event to determine skier placment within age division
             * Analzye and assign placements by age group and / or event groups as specifeid by request
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            int.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "SlalomRounds", "1" ), out curEventRounds );

            String curPlcmt, curGroup, curGroupName, curSelectCmd;
            DataRow[] curSkierList;
            DataTable curPlcmtResults, curSkierScoreList, curTiePlcmtList;

            String curPlcmtName = "PlcmtSlalom";
            if ( HelperFunctions.isIwwfEvent( curRules ) ) {
                curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Slalom", inNumPrelimRounds );
            } else {
                curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Slalom", inNumPrelimRounds );
            }

            // Tiebreaker functioniality not used for collegiate events
            if ( HelperFunctions.isCollegiateEvent( curRules ) ) return curPlcmtResults;

            if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                /*
				 * Analysis when placement specifications doesn't use divisions or groups.
				 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
				 */
                curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
                foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                    curPlcmt = (String)curRow[curPlcmtName];
                    curFilterCmd = curPlcmtName + " = '" + curPlcmt + "'";
                    curSkierList = curPlcmtResults.Select( curFilterCmd );

                    //Analyze tied placments and gather all data needed for tie breakers
                    curSkierScoreList = getSlalomTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

                    //Analyze data for tied placments utilizing tie breakers to determine placments
                    setSlalomTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                    //Analyze results of tie breakers and update event skier placments
                    setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
                }

                return curPlcmtResults;
            }

            /*
			 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
			 */
            curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
            foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
                curPlcmt = (String)curRow[curPlcmtName];
                curGroup = "";
                if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                    curGroupName = "AgeGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "'";
                } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                    curSelectCmd = "AgeGroup ASC, EventGroup ASC";
                    curGroupName = "AgeGroup,EventGroup";
                    curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
                    curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
                        + "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
                } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                    curGroupName = "EventGroup";
                    curGroup = (String)curRow[curGroupName];
                    curSelectCmd = curGroupName + " = '" + curGroup + "'";
                } else {
                    curGroupName = "";
                    curGroup = "";
                    curSelectCmd = "";
                }

                //Get results for the skiers that are tied for the placement by age group and position
                curFilterCmd = curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'";
                curSkierList = curPlcmtResults.Select( curFilterCmd );

                //Analyze tied placments and gather all data needed for tie breakers
                curSkierScoreList = getSlalomTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

                //Analyze data for tied placments utilizing tie breakers to determine placments
                setSlalomTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

                //Analyze results of tie breakers and update event skier placments
                setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
            }

            return curPlcmtResults;
        }

        public DataTable setTrickPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setTrickPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "", 0 );
        }
        public DataTable setTrickPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, Int16 inNumPrelimRounds ) {
            /* **********************************************************
             * Use results from trick event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
			String curFilterCmd = "";
			int curEventRounds = 0;
			int.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "TrickRounds", "1" ), out curEventRounds );

			String curPlcmt, curGroup, curGroupName, curSelectCmd;
            DataRow[] curSkierList;
			DataTable curPlcmtResults, curSkierScoreList, curTiePlcmtList;

			String curPlcmtName = "PlcmtTrick";
			if ( HelperFunctions.isIwwfEvent( curRules ) ) {
				curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Trick", inNumPrelimRounds );
			} else {
				curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Trick", inNumPrelimRounds );
			}

			// Tiebreaker functioniality not used for collegiate events
			if ( HelperFunctions.isCollegiateEvent( curRules ) ) return curPlcmtResults;

			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				/*
				 * Analysis when placement specifications doesn't use divisions or groups.
				 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
				 */
				curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
				foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
					curPlcmt = (String)curRow[curPlcmtName];
					curFilterCmd = curPlcmtName + " = '" + curPlcmt + "'";
					curSkierList = curPlcmtResults.Select( curFilterCmd );

					//Analyze tied placments and gather all data needed for tie breakers
					curSkierScoreList = getTrickTieBreakerData( curSkierList, curEventRounds, curRules, inDataType );

					//Analyze data for tied placments utilizing tie breakers to determine placments
					setTrickTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

					//Analyze results of tie breakers and update event skier placments
					setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
				}
				return curPlcmtResults;
			}

			/*
			 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
			 */
			curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
			foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
				curPlcmt = (String)curRow[curPlcmtName];
				curGroup = "";
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroupName = "AgeGroup";
					curGroup = (String)curRow[curGroupName];
					curSelectCmd = curGroupName + " = '" + curGroup + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSelectCmd = "AgeGroup ASC, EventGroup ASC";
					curGroupName = "AgeGroup,EventGroup";
					curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
					curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
						+ "AND EventGroup = '" + (String)curRow["EventGroup"] + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroupName = "EventGroup";
					curGroup = (String)curRow[curGroupName];
					curSelectCmd = curGroupName + " = '" + curGroup + "'";
				} else {
					curGroupName = "";
					curGroup = "";
					curSelectCmd = "";
				}

				//Get results for the skiers that are tied for the placement by age group and position
				curFilterCmd = curSelectCmd + " AND " + curPlcmtName + " = '" + curPlcmt + "'";
				curSkierList = curPlcmtResults.Select( curFilterCmd );

				//Analyze tied placments and gather all data needed for tie breakers
				curSkierScoreList = getTrickTieBreakerData( curSkierList, curEventRounds, curRules, inDataType );

				//Analyze data for tied placments utilizing tie breakers to determine placments
				setTrickTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

				//Analyze results of tie breakers and update event skier placments
				setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
			}

			return curPlcmtResults;
        }

        public DataTable setJumpPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg ) {
            return setJumpPlcmt( inTourRow, inEventResults, inPlcmtMethod, inPlcmtOrg, "", 0 );
        }
        public DataTable setJumpPlcmt( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, Int16 inNumPrelimRounds ) {
            /* **********************************************************
             * Use results from jump event to determine skier placment 
             * within age division
             * ******************************************************* */
            mySanctionNum = (String)inTourRow["SanctionId"];
            String curRules = (String)inTourRow["Rules"];
            String curFilterCmd = "";
            int curEventRounds = 0;
            int.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "JumpRounds", "1" ), out curEventRounds );

            String curPlcmt, curGroup, curGroupName, curSelectCmd;
            DataRow[] curSkierList;
            DataTable curPlcmtResults, curSkierScoreList, curTiePlcmtList;

            String curPlcmtName = "PlcmtJump";
            if ( HelperFunctions.isIwwfEvent( curRules ) ) {
                curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Jump", inNumPrelimRounds );
            } else {
                curPlcmtResults = setInitEventPlcmt( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, curPlcmtName, "Jump", inNumPrelimRounds );
            }

			if ( HelperFunctions.isCollegiateEvent( curRules ) ) {
				/*
				 * Collegiate placement calculations - Analzye and assign placements by age group and / or event groups as specifeid by request
				 */
				curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
				foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
					curPlcmt = (String)curRow[curPlcmtName];
					if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
						curGroupName = "AgeGroup";
						curGroup = (String)curRow[curGroupName];
						curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
					} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
						curSelectCmd = "AgeGroup ASC, EventGroup ASC";
						curGroupName = "AgeGroup,EventGroup";
						curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
						curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
							+ "AND EventGroup = '" + (String)curRow["EventGroup"] + "' AND ";
					} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
						curGroupName = "EventGroup";
						curGroup = (String)curRow[curGroupName];
						curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
					} else {
						curGroupName = "";
						curGroup = "";
						curSelectCmd = "";
					}

					//Get results for the skiers that are tied for the placement by age group and position
					curFilterCmd = curSelectCmd + curPlcmtName + " = '" + curPlcmt + "'";
					curSkierList = curPlcmtResults.Select( curFilterCmd );

					//Analyze tied placments and gather all data needed for tie breakers
					curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

					//Analyze data for tied placments utilizing tie breakers to determine placments
					setJumpTieBreakerNcwsaPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

					//Analyze results of tie breakers and update event skier placments
					setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
				}
				
                return curPlcmtResults;
			}

			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				/*
				 * Analysis when placement specifications doesn't use divisions or groups.
				 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
				 */
				curTiePlcmtList = findEventPlcmtTiesTour( curPlcmtName, curPlcmtResults );
				foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
					curPlcmt = (String)curRow[curPlcmtName];
					curFilterCmd = curPlcmtName + " = '" + curPlcmt + "'";
					curSkierList = curPlcmtResults.Select( curFilterCmd );

					//Analyze tied placments and gather all data needed for tie breakers
					curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

					//Analyze data for tied placments utilizing tie breakers to determine placments
					setJumpTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

					//Analyze results of tie breakers and update event skier placments
					setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
				}
				
                return curPlcmtResults;
            }

			/*
			 * Identify placement ties and gather scores to be used for breaking ties depending specified criteria
			 */
			curTiePlcmtList = findEventPlcmtTiesGroup( curPlcmtName, inPlcmtOrg, curPlcmtResults );
			foreach ( DataRow curRow in curTiePlcmtList.Rows ) {
				curPlcmt = (String)curRow[curPlcmtName];
				curGroup = "";
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroupName = "AgeGroup";
					curGroup = (String)curRow[curGroupName];
					curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSelectCmd = "AgeGroup ASC, EventGroup ASC";
					curGroupName = "AgeGroup,EventGroup";
					curGroup = (String)curRow["AgeGroup"] + "," + (String)curRow["EventGroup"];
					curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
						+ "AND EventGroup = '" + (String)curRow["EventGroup"] + "' AND ";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroupName = "EventGroup";
					curGroup = (String)curRow[curGroupName];
					curSelectCmd = curGroupName + " = '" + curGroup + "' AND ";
				} else {
					curGroupName = "";
					curGroup = "";
					curSelectCmd = "";
				}

				//Get results for the skiers that are tied for the placement by age group and position
				curSkierList = curPlcmtResults.Select( curSelectCmd + curPlcmtName + " = '" + curPlcmt + "'" );

				//Analyze tied placments and gather all data needed for tie breakers
				curSkierScoreList = getJumpTieBreakerData( curSkierList, curEventRounds, curRules, curPlcmtName, inDataType );

				//Analyze data for tied placments utilizing tie breakers to determine placments
				setJumpTieBreakerPlcmt( curSkierScoreList, curPlcmtName, curPlcmt );

				//Analyze results of tie breakers and update event skier placments
				setEventFinalPlcmt( curSkierScoreList, curPlcmtResults, inPlcmtMethod, inPlcmtOrg, curPlcmtName, curPlcmtName );
			}

			return curPlcmtResults;
        }

        public DataTable setOverallPlcmtIwwf( DataRow inTourRow, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType ) {
            /* **********************************************************
             * Use results from Overall event to determine skier placment 
             * within age division
             * ******************************************************* */
            DataTable curPlcmtResults = setInitEventPlcmtIwwf( inEventResults, inPlcmtMethod, inPlcmtOrg, inDataType, "Overall", 0 );

            return curPlcmtResults;
        }

        private DataTable getSlalomTieBreakerData( DataRow[] inSkierList, int inEventRounds, String inRules, String inPlcmtName, String inDataType ) {
            /* **********************************************************
             * Retrieve all data required for a slalom tie breaker
             * ******************************************************* */
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curScoreName = "ScoreSlalom";
            String curRoundName = "RoundSlalom";
            String curPointsName = "PointsSlalom";
            String curFilter;
            byte curRound, curFinalSpeedKph;
            decimal curScore, curScoreTemp, curRunoffScore, curRankingScore, curFirstScore, curBackupScore, curPassScore, curFinalLineLength;

            DataRowView newSkierScoreRow;
            DataRow[] curSkierScoreRow;
            DataTable curSkierResultsAll;
            DataTable curSkierScoreList = buildSkierSlalomScoreList();

            foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
                if ( inDataType.ToLower().Equals( "final" ) ) {
                    byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, "Round", "1" ), out curRound );
                } else {
                    byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, curRoundName, "1" ), out curRound );
                }
                newSkierScoreRow["Round"] = curRound;
                newSkierScoreRow["Score"] = 0;
                newSkierScoreRow["Speed"] = 0;
                newSkierScoreRow["PassScore"] = 0;
                newSkierScoreRow["RunoffScore"] = 0;
                newSkierScoreRow["FirstScore"] = 0;
                newSkierScoreRow["BackupScore"] = 0;
                newSkierScoreRow["RankingScore"] = 0;

                if ( inRules.ToLower().Equals( "iwwf" ) ) {
                    newSkierScoreRow["LineLength"] = 18.25M;
                } else {
                    newSkierScoreRow["LineLength"] = 23M;
                }

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
                curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore" );
                curSqlStmt.Append( ", SS.Round as " + curRoundName + ", SS.Score as " + curScoreName + ", SS.NopsScore as " + curPointsName );
                curSqlStmt.Append( ", SS.MaxSpeed, SS.StartSpeed, SS.StartLen, SS.Status, SS.FinalSpeedMph, SS.FinalSpeedKph, SS.FinalLen, SS.FinalLenOff, SS.FinalPassScore" );
                curSqlStmt.Append( ", COALESCE( RO.Score , 0) as ScoreRunoff " );
                curSqlStmt.Append( "FROM SlalomScore SS " );
                curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
                curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
                curSqlStmt.Append( "  LEFT OUTER JOIN SlalomScore RO ON RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round >= 25 " );
                curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Slalom'" );
                curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
                curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
                curSqlStmt.Append( "  And SS.Round < 25 " );
                curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
                curSkierResultsAll = DataAccess.getDataTable( curSqlStmt.ToString() );

                if ( curSkierResultsAll.Rows.Count <= 0 ) {
                    newSkierScoreRow.EndEdit();
                    continue;
                }

                //Retrieve tie score details, related final pass score, line length, final pass speed
                curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curRound );
                curSkierScoreRow = curSkierResultsAll.Select( curFilter );
                if ( curSkierScoreRow.Length <= 0 ) {
                    newSkierScoreRow.EndEdit();
                    continue;
                }

                decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScore );
                decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "FinalPassScore", "0" ), out curPassScore );
                decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "RankingScore", "0" ), out curRankingScore );
                decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreRunoff", "0" ), out curRunoffScore );
                byte.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "FinalSpeedKph", "0" ), out curFinalSpeedKph );
                decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "FinalLen", "0" ), out curFinalLineLength );

                newSkierScoreRow["Score"] = curScore;
                newSkierScoreRow["PassScore"] = curPassScore;
                newSkierScoreRow["RankingScore"] = curRankingScore;
                newSkierScoreRow["RunoffScore"] = curRunoffScore;

                newSkierScoreRow["Speed"] = curFinalSpeedKph;
                newSkierScoreRow["LineLength"] = curFinalLineLength;
                if ( inRules.ToLower().Equals( "iwwf" ) && curFinalLineLength == 23M ) newSkierScoreRow["LineLength"] = 18.25M;

                /*
				Only utilize backup score when runoff score is not available
				*/
                curBackupScore = 0;
				if ( curRunoffScore == 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}
				if ( inDataType.ToLower().Equals( "best" ) ) {
					if ( inEventRounds == 1 ) {
						newSkierScoreRow.EndEdit();
						continue;
					}

					//Retrieve tie score details, related final pass score, line length, final pass speed
					curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = 1", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"] );
					curSkierScoreRow = curSkierResultsAll.Select( curFilter );
					if ( curSkierScoreRow.Length <= 0 ) continue;

					for ( Byte curSkierRound = 1; curSkierRound <= inEventRounds; curSkierRound++ ) {
						curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curSkierRound );
						curSkierScoreRow = curSkierResultsAll.Select( curFilter );
						if ( curSkierScoreRow.Length <= 0 ) continue;

						if ( curSkierRound == 1 ) {
							decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curFirstScore );
							newSkierScoreRow["FirstScore"] = curFirstScore;
						}

						if ( curRound != curSkierRound ) {
							decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScoreTemp );
							if ( curScoreTemp > curBackupScore ) {
								curBackupScore = curScoreTemp;
								newSkierScoreRow["BackupScore"] = curBackupScore;
							}
						}
					}


				} else if ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, curScoreName, "0" ), out curScore );
					for ( Byte curSkierRound = 1; curSkierRound <= inEventRounds; curSkierRound++ ) {
						curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curSkierRound );
						curSkierScoreRow = curSkierResultsAll.Select( curFilter );
						if ( curSkierScoreRow.Length <= 0 ) continue;
						if ( curRound == curSkierRound ) continue;

						decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScoreTemp );
						if ( curScoreTemp > curBackupScore ) {
							curBackupScore = curScoreTemp;
							newSkierScoreRow["BackupScore"] = curBackupScore;
						}
					}
				}

				newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setSlalomTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * curGroup, curGroupName
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0, curPlcmtNum;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] == (String)curCheckScoreRow["MemberId"] ) continue;

                    if ( (Decimal)curSkierScoreRow["RunoffScore"] == (Decimal)curCheckScoreRow["RunoffScore"] ) {
                        if ( (Decimal)curSkierScoreRow["RunoffScore"] > 0 ) {
                            nextPlctAdj--;
                        } else {
                            if ( (Decimal)curSkierScoreRow["BackupScore"] == (Decimal)curCheckScoreRow["BackupScore"] ) {
                                if ( (Decimal)curSkierScoreRow["FirstScore"] == (Decimal)curCheckScoreRow["FirstScore"] ) {
                                    nextPlctAdj--;
                                } else if ( (Decimal)curSkierScoreRow["FirstScore"] > (Decimal)curCheckScoreRow["FirstScore"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Decimal)curSkierScoreRow["BackupScore"] > (Decimal)curCheckScoreRow["BackupScore"] ) {
                                nextPlctAdj--;
                            }
                        }
                    } else if ( (Decimal)curSkierScoreRow["RunoffScore"] > (Decimal)curCheckScoreRow["RunoffScore"] ) {
                        nextPlctAdj--;
                    }
                }

                curPlcmtNum = curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1;
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt;
                } else {
                    curPlcmt = curPlcmtNum.ToString( "##0" );
                    if ( curPlcmt.Length > 3 ) {
                        curSkierScoreRow[inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                    } else {
                        curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                    }
                }
            }
        }

        private DataTable getTrickTieBreakerData( DataRow[] inSkierList, int inEventRounds, String inRules, String inDataType ) {
			/* **********************************************************
             * Retrieve all data required for a trick tie breaker
             * ******************************************************* */
			StringBuilder curSqlStmt = new StringBuilder( "" );
            String curScoreName = "ScoreTrick";
            String curRoundName = "RoundTrick";
            String curPointsName = "PointsTrick";
			String curFilter;
            byte curRound, curRoundTemp;
            Int16 curScore, curScoreTemp, curRunoffScore, curBackupScore, curPass1Score, curPass2Score, curPass1ScoreTemp, curPass2ScoreTemp;
			decimal curRankingScore;

			DataRowView newSkierScoreRow;
			DataRow[] curSkierScoreRow;
			DataTable curSkierResultsAll;
			DataTable curSkierScoreList = buildSkierTrickScoreList();
			foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
				if ( inDataType.ToLower().Equals( "final" ) ) {
					byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, "Round", "1" ), out curRound );
				} else {
					byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, curRoundName, "1" ), out curRound );
				}
				newSkierScoreRow["Round"] = curRound;
				newSkierScoreRow["Score"] = 0;
                newSkierScoreRow["PassScore1"] = 0;
                newSkierScoreRow["PassScore2"] = 0;
                newSkierScoreRow["PassScore3"] = 0;
                newSkierScoreRow["PassScore4"] = 0;
                newSkierScoreRow["RunoffScore"] = 0;
                newSkierScoreRow["RankingScore"] = 0;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
				curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore" );
				curSqlStmt.Append( ", SS.Round as " + curRoundName + ", SS.Score as " + curScoreName + ", SS.NopsScore as " + curPointsName );
				curSqlStmt.Append( ", SS.ScorePass1, SS.ScorePass2, COALESCE( RO.Score, 0) as ScoreRunoff " );
				curSqlStmt.Append( "FROM TrickScore SS " );
				curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
				curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
				curSqlStmt.Append( "  LEFT OUTER JOIN TrickScore RO ON RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round >= 25 " );
				curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Trick'" );
				curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
				curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
				curSqlStmt.Append( "  And SS.Round < 25 " );
				curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
				curSkierResultsAll = DataAccess.getDataTable( curSqlStmt.ToString() );

				if ( curSkierResultsAll.Rows.Count <= 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}

				//Retrieve tie score details, related final pass score, line length, final pass speed
				curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curRound );
				curSkierScoreRow = curSkierResultsAll.Select( curFilter );
				if ( curSkierScoreRow.Length <= 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}

				Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScore );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreRunoff", "0" ), out curRunoffScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "RankingScore", "0" ), out curRankingScore );

				newSkierScoreRow["Score"] = curScore;
				newSkierScoreRow["RunoffScore"] = curRunoffScore;
				newSkierScoreRow["RankingScore"] = curRankingScore;

				if ( inRules.ToLower().Equals( "iwwf" ) ) {
					newSkierScoreRow.EndEdit();
					continue;
				}

				Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScorePass1", "0" ), out curPass1Score );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScorePass2", "0" ), out curPass2Score );
                if ( curPass2Score > curPass1Score ) {
					newSkierScoreRow["PassScore1"] = curPass2Score;
					newSkierScoreRow["PassScore2"] = curPass1Score;
				} else {
					newSkierScoreRow["PassScore1"] = curPass1Score;
					newSkierScoreRow["PassScore2"] = curPass2Score;
				}

				curBackupScore = 0;
				if ( curRunoffScore == 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}
                
                /*
                 * Identify backup scores that can be used for tie breaking scenerios 
                 * Different criteria are used when placements are based on a skier's best score of all rounds 
                 * versus when placement is based on a skiers final round score
                 */
                if ( inDataType.ToLower().Equals( "best" ) ) {
					#region Tie breaker data based on criteria used when placements are based on the skier's best score of all rounds 
					if ( inEventRounds == 1 ) {
                        newSkierScoreRow.EndEdit();
                        continue;
                    }

                    if ( curRound == 1 ) {
						newSkierScoreRow["FirstScore"] = curScore;
					
                    } else {
						curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = 1", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"] );
						curSkierScoreRow = curSkierResultsAll.Select( curFilter );
						if ( curSkierScoreRow.Length > 0 ) {
							Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScoreTemp );
							newSkierScoreRow["FirstScore"] = curScoreTemp;
							
                            if ( curScoreTemp > curBackupScore  ) {
								curBackupScore = curScoreTemp;
								newSkierScoreRow["BackupScore"] = curBackupScore;
							}
						}
					}

					// Check all passes and determine the best individual passes that should be used for tie breaking purposes
                    for ( Byte curSkierRound = 1; curSkierRound <= inEventRounds; curSkierRound++ ) {
						curSkierScoreRow = curSkierResultsAll.Select( "MemberId = '" + (String)curSkierRow["MemberId"] + "'"
							+ " AND AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' AND " + curRoundName + " = " + curSkierRound.ToString() );
                        if ( curSkierScoreRow.Length <= 0 ) continue;

						Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScoreTemp );
						byte.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "Round", "0" ), out curRoundTemp );
                        if ( curRoundTemp == curSkierRound || curRoundTemp == 0 ) continue;

						if ( curScoreTemp > curBackupScore ) {
							curBackupScore = curScoreTemp;
							newSkierScoreRow["BackupScore"] = curBackupScore;
						}

						//Save individual pass scores in score order
						Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScorePass1", "0" ), out curPass1ScoreTemp );
						Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScorePass2", "0" ), out curPass2ScoreTemp );
						
                        if ( curPass1ScoreTemp > (Int16)newSkierScoreRow["PassScore1"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
							newSkierScoreRow["PassScore2"] = newSkierScoreRow["PassScore1"];
							newSkierScoreRow["PassScore1"] = curPass1ScoreTemp;
                        } else if ( curPass1ScoreTemp > (Int16)newSkierScoreRow["PassScore2"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
							newSkierScoreRow["PassScore2"] = curPass1ScoreTemp;
						} else if ( curPass1ScoreTemp > (Int16)newSkierScoreRow["PassScore3"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = curPass1ScoreTemp;
						} else if ( curPass1ScoreTemp > (Int16)newSkierScoreRow["PassScore4"] ) {
							newSkierScoreRow["PassScore4"] = curPass1ScoreTemp;
						}

						if ( curPass2ScoreTemp > (Int16)newSkierScoreRow["PassScore1"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
							newSkierScoreRow["PassScore2"] = newSkierScoreRow["PassScore1"];
							newSkierScoreRow["PassScore1"] = curPass2ScoreTemp;
						} else if ( curPass2ScoreTemp > (Int16)newSkierScoreRow["PassScore2"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = newSkierScoreRow["PassScore2"];
							newSkierScoreRow["PassScore2"] = curPass2ScoreTemp;
						} else if ( curPass2ScoreTemp > (Int16)newSkierScoreRow["PassScore3"] ) {
							newSkierScoreRow["PassScore4"] = newSkierScoreRow["PassScore3"];
							newSkierScoreRow["PassScore3"] = curPass2ScoreTemp;
						} else if ( curPass2ScoreTemp > (Int16)newSkierScoreRow["PassScore4"] ) {
							newSkierScoreRow["PassScore4"] = curPass2ScoreTemp;
						}
					}
					#endregion

				} else if ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					for ( Byte curSkierRound = 1; curSkierRound <= inEventRounds; curSkierRound++ ) {
						curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curSkierRound );
						curSkierScoreRow = curSkierResultsAll.Select( curFilter );
						if ( curSkierScoreRow.Length <= 0 ) continue;
						if ( curRound == curSkierRound ) continue;

						Int16.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], curScoreName, "0" ), out curScoreTemp );
						if ( curScoreTemp > curBackupScore ) {
							curBackupScore = curScoreTemp;
							newSkierScoreRow["BackupScore"] = curBackupScore;
						}
					}

				}

				newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setTrickTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Int16)curSkierScoreRow["BackupScore"] == (Int16)curCheckScoreRow["BackupScore"] ) {
                            if ( (Int16)curSkierScoreRow["FirstScore"] == (Int16)curCheckScoreRow["FirstScore"] ) {
                                if ( (Int16)curSkierScoreRow["PassScore1"] == (Int16)curCheckScoreRow["PassScore1"] ) {
                                    if ( (Int16)curSkierScoreRow["PassScore2"] == (Int16)curCheckScoreRow["PassScore2"] ) {
                                        if ( (Int16)curSkierScoreRow["PassScore3"] == (Int16)curCheckScoreRow["PassScore3"] ) {
                                            if ( (Int16)curSkierScoreRow["PassScore4"] == (Int16)curCheckScoreRow["PassScore4"] ) {

                                                if ( (Int16)curSkierScoreRow["RunoffScore"] == (Int16)curCheckScoreRow["RunoffScore"] ) {
                                                    nextPlctAdj--;
                                                } else if ( (Int16)curSkierScoreRow["RunoffScore"] > (Int16)curCheckScoreRow["RunoffScore"] ) {
                                                    nextPlctAdj--;
                                                }
                                            } else if ( (Int16)curSkierScoreRow["PassScore4"] > (Int16)curCheckScoreRow["PassScore4"] ) {
                                                nextPlctAdj--;
                                            }
                                        } else if ( (Int16)curSkierScoreRow["PassScore3"] > (Int16)curCheckScoreRow["PassScore3"] ) {
                                            nextPlctAdj--;
                                        }
                                    } else if ( (Int16)curSkierScoreRow["PassScore2"] > (Int16)curCheckScoreRow["PassScore2"] ) {
                                        nextPlctAdj--;
                                    }
                                } else if ( (Int16)curSkierScoreRow["PassScore1"] > (Int16)curCheckScoreRow["PassScore1"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Int16)curSkierScoreRow["FirstScore"] > (Int16)curCheckScoreRow["FirstScore"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Int16)curSkierScoreRow["BackupScore"] > (Int16)curCheckScoreRow["BackupScore"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private DataTable getJumpTieBreakerData( DataRow[] inSkierList, int inEventRounds, String inRules, String inPlcmtName, String inDataType ) {
            /* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curRoundName = "RoundJump";
            String curPointsName = "PointsJump";
			String curFilter;

            byte curRound, curRoundTemp;
			decimal curScoreFeet, curScoreMeters, curScoreFeetTemp, curScoreMetersTemp, curRunoffScoreFeet, curRunoffScoreMeters, curBackupScoreFeet, curBackupScoreMeters, curRankingScore;
			//Decimal[] curRunoffScores;

			DataRowView newSkierScoreRow;
			DataRow[] curSkierScoreRow;
			DataTable curSkierResultsAll;
			DataTable curSkierScoreList = buildSkierJumpScoreList();


			foreach ( DataRow curSkierRow in inSkierList ) {
                newSkierScoreRow = curSkierScoreList.DefaultView.AddNew();
                newSkierScoreRow["MemberId"] = (String)curSkierRow["MemberId"];
                newSkierScoreRow["AgeGroup"] = (String)curSkierRow["AgeGroup"];
				if ( inDataType.ToLower().Equals( "final" ) ) {
					byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, "Round", "1" ), out curRound );
				} else {
					byte.TryParse( HelperFunctions.getDataRowColValue( curSkierRow, curRoundName, "1" ), out curRound );
				}
				newSkierScoreRow["Round"] = curRound;
				newSkierScoreRow["ScoreFeet"] = 0;
                newSkierScoreRow["ScoreMeters"] = 0;
                newSkierScoreRow["RunoffScoreFeet"] = 0;
                newSkierScoreRow["RunoffScoreMeters"] = 0;
                newSkierScoreRow["BackupScoreFeet"] = 0;
                newSkierScoreRow["BackupScoreMeters"] = 0;
                newSkierScoreRow["RankingScore"] = 0;

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, SS.BoatSpeed, SS.RampHeight, ER.TeamCode, '' as BoatCode " );
                curSqlStmt.Append( ", ER.AgeGroup,  ER.EventGroup, COALESCE(SS.EventClass, ER.EventClass) as EventClass, ER.RankingScore" );
                curSqlStmt.Append( ", SS.ScoreFeet, SS.ScoreMeters, SS.Round as " + curRoundName + ", SS.NopsScore as " + curPointsName );
                curSqlStmt.Append( ", RO.ScoreFeet as RunoffScoreFeet, RO.ScoreMeters as RunoffScoreMeters " );
				curSqlStmt.Append( "FROM JumpScore AS SS " );
                curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
                curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
				curSqlStmt.Append( "  LEFT OUTER JOIN JumpScore RO ON RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId AND RO.AgeGroup = TR.AgeGroup AND RO.Round >= 25 " );
				curSqlStmt.Append( "WHERE SS.SanctionId = '" + (String)curSkierRow["SanctionId"] + "' AND ER.Event = 'Jump'" );
                curSqlStmt.Append( "  And SS.MemberId = '" + (String)curSkierRow["Memberid"] + "' " );
                curSqlStmt.Append( "  And ER.AgeGroup = '" + (String)curSkierRow["AgeGroup"] + "' " );
                curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, SS.MemberId, SS.Round" );
                curSkierResultsAll = DataAccess.getDataTable( curSqlStmt.ToString() );

				if ( curSkierResultsAll.Rows.Count <= 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}

				//Retrieve tie score details, related final pass score, line length, final pass speed
				curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curRound );
				curSkierScoreRow = curSkierResultsAll.Select( curFilter );
				if ( curSkierScoreRow.Length <= 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}

				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreFeet", "0" ), out curScoreFeet );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreMeters", "0" ), out curScoreMeters );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "RankingScore", "0" ), out curRankingScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "RunoffScoreFeet", "0" ), out curRunoffScoreFeet );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "RunoffScoreMeters", "0" ), out curRunoffScoreMeters );

				newSkierScoreRow["ScoreFeet"] = curScoreFeet;
				newSkierScoreRow["ScoreMeters"] = curScoreMeters;
				newSkierScoreRow["RankingScore"] = curRankingScore;
				newSkierScoreRow["RunoffScoreFeet"] = curRunoffScoreFeet;
				newSkierScoreRow["RunoffScoreMeters"] = curRunoffScoreMeters;

				/*
				 * Only utilize backup score when runoff score is not available
				*/
                curBackupScoreFeet = 0;
                curBackupScoreMeters = 0;
				if ( curBackupScoreFeet == 0 || curBackupScoreMeters == 0 ) {
					newSkierScoreRow.EndEdit();
					continue;
				}
                
                if ( inDataType.ToLower().Equals( "best" ) ) {
                    if ( inEventRounds == 1 || !(HelperFunctions.isIwwfEvent(inRules)) ) {
                        newSkierScoreRow.EndEdit();
                        continue;
                    }

                    foreach ( DataRow curRow in curSkierResultsAll.Rows ) {
						decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "0" ), out curScoreFeetTemp );
						decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "0" ), out curScoreMetersTemp );

                        if ( curScoreFeetTemp > (Decimal)newSkierScoreRow["BackupScoreFeet"] 
                            || ( curScoreFeetTemp == (Decimal)newSkierScoreRow["BackupScoreFeet"] && curScoreMetersTemp > (Decimal)newSkierScoreRow["BackupScoreMeters"] )
							) {
							newSkierScoreRow["BackupScoreFeet"] = curScoreFeetTemp;
							newSkierScoreRow["BackupScoreMeters"] = curScoreMetersTemp;
						}
					}
				} else if ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					for ( Byte curSkierRound = 1; curSkierRound <= inEventRounds; curSkierRound++ ) {
						curFilter = String.Format( "MemberId = '{0}' AND AgeGroup = '{1}' AND " + curRoundName + " = {2}", (String)curSkierRow["MemberId"], (String)curSkierRow["AgeGroup"], curSkierRound );
						curSkierScoreRow = curSkierResultsAll.Select( curFilter );
						if ( curSkierScoreRow.Length <= 0 ) continue;
						if ( curRound == curSkierRound ) continue;

						decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreFeet", "0" ), out curScoreFeetTemp );
						decimal.TryParse( HelperFunctions.getDataRowColValue( curSkierScoreRow[0], "ScoreMeters", "0" ), out curScoreMetersTemp );

						if ( curScoreFeetTemp > curBackupScoreFeet || ( curScoreFeetTemp == curBackupScoreFeet && curScoreMetersTemp > curBackupScoreMeters ) ) {
							curBackupScoreFeet = curScoreFeetTemp;
							curBackupScoreMeters = curScoreMetersTemp;
							newSkierScoreRow["BackupScoreFeet"] = curBackupScoreFeet;
							newSkierScoreRow["BackupScoreMeters"] = curBackupScoreMeters;
						}
					}
				}

				newSkierScoreRow.EndEdit();
            }

            return curSkierScoreList;
        }

        private void setJumpTieBreakerPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["ScoreMeters"] == (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            if ( (Decimal)curSkierScoreRow["BackupScoreFeet"] == (Decimal)curCheckScoreRow["BackupScoreFeet"] ) {
                                if ( (Decimal)curSkierScoreRow["BackupScoreMeters"] == (Decimal)curCheckScoreRow["BackupScoreMeters"] ) {
                                    if ( (Decimal)curSkierScoreRow["RunoffScoreFeet"] == (Decimal)curCheckScoreRow["RunoffScoreFeet"] ) {
                                        if ( (Decimal)curSkierScoreRow["RunoffScoreMeters"] == (Decimal)curCheckScoreRow["RunoffScoreMeters"] ) {
                                            nextPlctAdj--;
                                        } else if ( (Decimal)curSkierScoreRow["RunoffScoreMeters"] > (Decimal)curCheckScoreRow["RunoffScoreMeters"] ) {
                                            nextPlctAdj--;
                                        }
                                    } else if ( (Decimal)curSkierScoreRow["RunoffScoreFeet"] > (Decimal)curCheckScoreRow["RunoffScoreFeet"] ) {
                                        nextPlctAdj--;
                                    }
                                } else if ( (Decimal)curSkierScoreRow["BackupScoreMeters"] > (Decimal)curCheckScoreRow["BackupScoreMeters"] ) {
                                    nextPlctAdj--;
                                }
                            } else if ( (Decimal)curSkierScoreRow["BackupScoreFeet"] > (Decimal)curCheckScoreRow["BackupScoreFeet"] ) {
                                nextPlctAdj--;
                            }
                        } else if ( (Decimal)curSkierScoreRow["ScoreMeters"] > (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private void setJumpTieBreakerNcwsaPlcmt( DataTable inSkierScoreList, String inPlcmtName, String inPlcmt ) {
            /* **********************************************************
             * Analyze data for tied placments utilizing tie breakers to determine placments
             * ******************************************************* */
            String curPlcmt = "";
            int nextPlcmtPos = 0, nextPlctAdj = 0;
            int curOrigPlcmtPos = Convert.ToInt32( inPlcmt.Substring( 0, inPlcmt.Length - 1 ) );

            foreach ( DataRow curSkierScoreRow in inSkierScoreList.Rows ) {
                nextPlcmtPos = inSkierScoreList.Rows.Count;
                nextPlctAdj = 0;
                foreach ( DataRow curCheckScoreRow in inSkierScoreList.Rows ) {
                    if ( (String)curSkierScoreRow["MemberId"] != (String)curCheckScoreRow["MemberId"] ) {
                        if ( (Decimal)curSkierScoreRow["ScoreMeters"] == (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        } else if ( (Decimal)curSkierScoreRow["ScoreMeters"] > (Decimal)curCheckScoreRow["ScoreMeters"] ) {
                            nextPlctAdj--;
                        }
                    }
                }
                curPlcmt = Convert.ToString( curOrigPlcmtPos + nextPlcmtPos + nextPlctAdj - 1 );
                if ( curPlcmt.Contains( "T" ) ) {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 4, ' ' );
                } else {
                    curSkierScoreRow[inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                }
            }
        }

        private void setEventFinalPlcmt( DataTable inSkierScoreList, DataTable inEventResults, String inPlcmtMethod, String inPlcmtOrg, String inPlcmtName, String inPlcmtNameSmry ) {
            /* **********************************************************
             * Analyze results of tie breakers and update event skier placments
             * ******************************************************* */
            inSkierScoreList.DefaultView.Sort = inPlcmtNameSmry + " ASC, MemberId ASC";
            DataTable curSkierScoreList = inSkierScoreList.DefaultView.ToTable();
            DataRow prevSkierScoreRow;
            DataRow[] curSkierList;
            int prevIdx = 0, curIdx = 0;
            String curPlcmt = "";
            foreach ( DataRow curSkierScoreRow in curSkierScoreList.Rows ) {
                if ( curIdx > 0 ) {
                    prevSkierScoreRow = curSkierScoreList.Rows[prevIdx];
                    if ( curSkierScoreRow[inPlcmtNameSmry].Equals( prevSkierScoreRow[inPlcmtNameSmry] ) ) {
                        curSkierList = inEventResults.Select( "MemberId = '" + prevSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + prevSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)prevSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + "T";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + "T";
                            }
                        }
                        curSkierList = inEventResults.Select( "MemberId = '" + curSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + curSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)curSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + "T";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + "T";
                            }
                        }
                    } else {
                        curSkierList = inEventResults.Select( "MemberId = '" + curSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + curSkierScoreRow["AgeGroup"] + "'" );
                        if ( curSkierList.Length > 0 ) {
                            curPlcmt = ( (String)curSkierScoreRow[inPlcmtNameSmry] );
                            if ( curPlcmt.Length > 3 ) {
                                curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                            } else {
                                curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                            }
                        }
                        if ( curIdx == 1 ) {
                            curSkierList = inEventResults.Select( "MemberId = '" + prevSkierScoreRow["MemberId"] + "' AND AgeGroup = '" + prevSkierScoreRow["AgeGroup"] + "'" );
                            if ( curSkierList.Length > 0 ) {
                                curPlcmt = ( (String)prevSkierScoreRow[inPlcmtNameSmry] );
                                if ( curPlcmt.Length > 3 ) {
                                    curSkierList[0][inPlcmtName] = curPlcmt.Substring( 0, 3 ) + " ";
                                } else {
                                    curSkierList[0][inPlcmtName] = curPlcmt.PadLeft( 3, ' ' ) + " ";
                                }
                            }
                        }
                    }
                }
                prevIdx = curIdx;
                curIdx++;
            }
        }

		/* **********************************************************
		 * Analyze results and determine skier placment within age division
		 * ******************************************************* */
		private DataTable setInitEventPlcmt( DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inPlcmtName, String inEvent, Int16 inNumPrelimRounds ) {
            if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
                return setInitEventPlcmtNoGroups( inResults, inPlcmtMethod, inPlcmtOrg, inDataType, inPlcmtName, inEvent, inNumPrelimRounds );
			}

			/*
             * Calculate placement based on requested attributes and organization (divisions and groups
             */
			DataTable curPlcmtResults = inResults;
			String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "", curReadyForPlcmt = "";
			int curIdx = 0, curPlcmtPos = 1, curRoundInt = 0, prevRoundInt = 0;
			Decimal curScore = 0, prevScore = -1;

			String curRoundName = "Round" + inEvent;
			String curScoreName = "Score" + inEvent;
			if ( inEvent.ToLower().Equals( "jump" ) ) curScoreName = "ScoreFeet";
			if ( !( inPlcmtMethod.ToLower().Equals( "score" ) ) ) curScoreName = "Points" + inEvent;

			if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmt" + inEvent + " DESC, ";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, ReadyForPlcmt" + inEvent + " DESC, ";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSortCmd = "EventGroup ASC, ReadyForPlcmt" + inEvent + " DESC, ";
			} else {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmt" + inEvent + " DESC, ";
			}
			if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
				curSortCmd += curRoundName + " ASC, ";
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSortCmd += curRoundName + " DESC, ";
			} else if ( inDataType.ToLower().Equals( "first" ) ) {
				curSortCmd += curRoundName + " DESC, ";
			}
			
            if ( inPlcmtMethod.ToLower().Equals( "score" ) && inEvent.ToLower().Equals( "jump" ) ) {
				curSortCmd += "ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
			} else {
				curSortCmd += curScoreName + " DESC, SkierName ASC ";
			}

			curPlcmtResults.DefaultView.Sort = curSortCmd;
			DataTable tempPlcmtResults = curPlcmtResults.DefaultView.ToTable();
			curPlcmtResults = tempPlcmtResults;

			foreach ( DataRow curRow in curPlcmtResults.Rows ) {
				curPlcmt = "";
				prevGroup = curGroup;
				prevScore = curScore;
				prevRoundInt = curRoundInt;

				curRound = HelperFunctions.getDataRowColValue( curRow, curRoundName, "1" );
				curRoundInt = Convert.ToInt32( curRound );

				if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = (String)curRow["AgeGroup"];
				}
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) curGroup += "-" + curRound;
				if ( !( curGroup.Equals( prevGroup ) ) ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}
				if ( inDataType.ToLower().Equals( "final" ) && curRoundInt <= inNumPrelimRounds && !( curRoundInt.Equals( prevRoundInt ) ) ) prevScore = -1;

				curReadyForPlcmt = (String)curRow["ReadyForPlcmt" + inEvent];
				if ( curReadyForPlcmt == null ) curReadyForPlcmt = "N";
				if ( inDataType.ToLower().Equals( "first" ) && !( curRound.Equals( "1" ) ) ) curReadyForPlcmt = "N";

				if ( curReadyForPlcmt.Equals( "Y" ) ) {
					if ( curRoundInt > 0 ) {
						decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );
						if ( curScore == prevScore && curIdx > 0 ) {
							curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1][inPlcmtName];
							if ( curPlcmt.Contains( "T" ) ) {
							} else {
								curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
								curPlcmtResults.Rows[curIdx - 1][inPlcmtName] = curPlcmt;
							}
						} else {
							curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
							curPlcmt += " ";
						}
						curPlcmtPos++;
					} else {
						curPlcmt = "";
						curScore = -1;
					}
					curRow[inPlcmtName] = curPlcmt;

				} else {
					curRow[inPlcmtName] = "  999 ";
					curPlcmt = "";
					curScore = -1;
				}
				curIdx++;
			}

			return curPlcmtResults;
        }

		/*
		 * Calculate placement for all tournament participants ignoring all divisions and groups
         */
		private DataTable setInitEventPlcmtNoGroups( DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inPlcmtName, String inEvent, Int16 inNumPrelimRounds ) {
			/* **********************************************************
             * Analyze results and determine skier placment within age division
             * ******************************************************* */
			DataTable curPlcmtResults = inResults;
			String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "", curReadyForPlcmt = "";
			int curIdx = 0, curPlcmtPos = 1, curRoundInt = 0, prevRoundInt = 0;
			Decimal curScore = 0, prevScore = -1;
			
            String curRoundName = "Round" + inEvent;
			String curScoreName = "Score" + inEvent;
            if ( inEvent.ToLower().Equals( "jump" ) ) curScoreName = "ScoreFeet";
			if ( !( inPlcmtMethod.ToLower().Equals( "score" ) ) ) curScoreName = "Points" + inEvent;

			if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
				curSortCmd = curRoundName + " ASC, ReadyForPlcmt" + inEvent + " DESC, ";
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSortCmd = curRoundName + " DESC, ReadyForPlcmt" + inEvent + " DESC, ";
			} else {
				curSortCmd = "";
			}
			if ( inPlcmtMethod.ToLower().Equals( "score" ) && inEvent.ToLower().Equals( "jump" ) ) {
				curSortCmd += "ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
			} else {
				curSortCmd += curScoreName + " DESC, SkierName ASC ";
			}
			
            curPlcmtResults.DefaultView.Sort = curSortCmd;
			curPlcmtResults = curPlcmtResults.DefaultView.ToTable();
			foreach ( DataRow curRow in curPlcmtResults.Rows ) {
				curPlcmt = "";
				prevGroup = curGroup;
				prevScore = curScore;
				prevRoundInt = curRoundInt;

				curRound = HelperFunctions.getDataRowColValue( curRow, curRoundName, "1" );
				curRoundInt = Convert.ToInt32( curRound );

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) curGroup += "-" + curRound;
				if ( !( curGroup.Equals( prevGroup ) ) ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}
				if ( inDataType.ToLower().Equals( "final" ) && curRoundInt <= inNumPrelimRounds && !( curRoundInt.Equals( prevRoundInt ) ) ) prevScore = -1;

				curReadyForPlcmt = (String)curRow["ReadyForPlcmt" + inEvent];
				if ( curReadyForPlcmt == null ) curReadyForPlcmt = "N";
				if ( inDataType.ToLower().Equals( "first" ) && !( curRound.Equals( "1" ) ) ) curReadyForPlcmt = "N";

				if ( curReadyForPlcmt.Equals( "Y" ) ) {
					if ( curRoundInt > 0 ) {
						decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );
						if ( curScore == prevScore && curIdx > 0 ) {
							curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1][inPlcmtName];
							if ( curPlcmt.Contains( "T" ) ) {
							} else {
								curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
								curPlcmtResults.Rows[curIdx - 1][inPlcmtName] = curPlcmt;
							}
						} else {
							curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
							curPlcmt += " ";
						}
						curRow[inPlcmtName] = curPlcmt;
						curPlcmtPos++;
					} else {
						curPlcmt = "";
						curScore = -1;
					}
					curRow[inPlcmtName] = curPlcmt;
					curIdx++;

				} else {
					curRow[inPlcmtName] = "  999 ";
					curPlcmt = "";
					curScore = -1;
				}
			}
			
            return curPlcmtResults;

		}

		/* **********************************************************
		 * Analyze results and calculate placement based on requested organization
		 * ******************************************************* */
		private DataTable setInitEventPlcmtIwwf(DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inEvent, Int16 inNumPrelimRounds ) {
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				return setInitEventPlcmtNoGroupsIwwf( inResults, inPlcmtMethod, inPlcmtOrg, inDataType, inEvent, inNumPrelimRounds );
			}
			
            DataTable curPlcmtResults = inResults;
            String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "";
			int curIdx = 0, curPlcmtPos = 1, curRoundInt = 0, prevRoundInt = 0;
            Decimal curScore = 0, prevScore = 0;

			String curRoundName = "Round" + inEvent;
			String curScoreName = "Score" + inEvent;
			if ( inEvent.ToLower().Equals( "jump" ) ) curScoreName = "ScoreMeters";
			if ( !( inPlcmtMethod.ToLower().Equals( "score" ) ) ) curScoreName = "Points" + inEvent;
            if ( inEvent.ToLower().Equals( "overall" ) ) curScoreName = "ScoreOverall";

			if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, ";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSortCmd = "EventGroup ASC, ";
			} else {
				curSortCmd = "AgeGroup ASC, ";
			}
			if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
				curSortCmd += curRoundName + " ASC, ";
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSortCmd += curRoundName + " DESC, ";
			}
			curSortCmd += curScoreName + " DESC, SkierName ASC ";
			curPlcmtResults.DefaultView.Sort = curSortCmd;
			curPlcmtResults = curPlcmtResults.DefaultView.ToTable();

			foreach ( DataRow curRow in curPlcmtResults.Rows) {
				curPlcmt = "";
				prevGroup = curGroup;
				prevScore = curScore;
				prevRoundInt = curRoundInt;

				curRound = HelperFunctions.getDataRowColValue( curRow, curRoundName, "1" );
				curRoundInt = Convert.ToInt32( curRound );

				if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = (String)curRow["AgeGroup"];
				}

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) curGroup += "-" + curRound;
				if ( !( curGroup.Equals( prevGroup ) ) ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}
				if ( inDataType.ToLower().Equals( "final" ) && curRoundInt <= inNumPrelimRounds && !( curRoundInt.Equals( prevRoundInt ) ) ) prevScore = -1;

				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );

				if ( ( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) ) ) {
					if ( ( (String)( curRow["QualifyOverall"] ) ).ToLower().Equals( "yes" ) ) {
						if ( curScore == prevScore && curRoundInt == prevRoundInt && curIdx > 0 ) {
							curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
							if ( curPlcmt.Length > 0 ) {
								if ( curPlcmt.Contains( "T" ) ) {
								} else {
									curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
									curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
								}
							} else {
								curPlcmt = "   ";
							}
						} else {
							curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
							curPlcmt += " ";
						}
						curRow["Plcmt" + inEvent] = curPlcmt;
						curPlcmtPos++;
					} else {
						curRow["Plcmt" + inEvent] = "";
					}
				} else {
					if ( curScore == prevScore && curRoundInt == prevRoundInt && curIdx > 0 ) {
						curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
						if ( curPlcmt.Length > 0 ) {
							if ( curPlcmt.Contains( "T" ) ) {
							} else {
								curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
								curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
							}
						} else {
							curPlcmt = "   ";
						}
					} else {
						curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
						curPlcmt += " ";
					}
					curRow["Plcmt" + inEvent] = curPlcmt;
					curPlcmtPos++;
				}
				curIdx++;
			}

			return curPlcmtResults;
        }

		/* **********************************************************
		 * Analyze results and determine skier placment ignoring all divisions and groups
		 * ******************************************************* */
		private DataTable setInitEventPlcmtNoGroupsIwwf( DataTable inResults, String inPlcmtMethod, String inPlcmtOrg, String inDataType, String inEvent, Int16 inNumPrelimRounds ) {
			DataTable curPlcmtResults = inResults;
			String curPlcmt = "", prevGroup = "", curGroup = "", curSortCmd = "", curRound = "";
			int curIdx = 0, curPlcmtPos = 1, curRoundInt = 0, prevRoundInt = 0;
			Decimal curScore = 0, prevScore = 0;

			String curRoundName = "Round" + inEvent;
			String curScoreName = "Score" + inEvent;
			if ( inEvent.ToLower().Equals( "jump" ) ) curScoreName = "ScoreMeters";
			if ( !( inPlcmtMethod.ToLower().Equals( "score" ) ) ) curScoreName = "Points" + inEvent;

			if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
				curSortCmd += curRoundName + " ASC, ";
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSortCmd += curRoundName + " DESC, ";
			} else {
				curSortCmd = "";
			}
			curSortCmd += curScoreName + " DESC, SkierName ASC ";
			curPlcmtResults.DefaultView.Sort = curSortCmd;
			curPlcmtResults = curPlcmtResults.DefaultView.ToTable();

			foreach ( DataRow curRow in curPlcmtResults.Rows ) {
				curPlcmt = "";
				prevGroup = curGroup;
				prevScore = curScore;
				prevRoundInt = curRoundInt;

				curRound = HelperFunctions.getDataRowColValue( curRow, curRoundName, "1" );
				curRoundInt = Convert.ToInt32( curRound );

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) curGroup += "-" + curRound;
				if ( !( curGroup.Equals( prevGroup ) ) ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}
				if ( inDataType.ToLower().Equals( "final" ) && curRoundInt <= inNumPrelimRounds && !( curRoundInt.Equals( prevRoundInt ) ) ) prevScore = -1;

				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );

				if ( ( inEvent.Equals( "Overall" ) || inEvent.Equals( "Scoreboard" ) ) ) {
					if ( ( (String)( curRow["QualifyOverall"] ) ).ToLower().Equals( "yes" ) ) {
						if ( curScore == prevScore && curIdx > 0 ) {
							curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
							if ( curPlcmt.Contains( "T" ) ) {
							} else {
								curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
								curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
							}
						} else {
							curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
							curPlcmt += " ";
						}
						curRow["Plcmt" + inEvent] = curPlcmt;
						curPlcmtPos++;
					} else {
						curRow["Plcmt" + inEvent] = "";
					}
				} else {
					if ( curScore == prevScore && curIdx > 0 ) {
						curPlcmt = (String)curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent];
						if ( curPlcmt.Contains( "T" ) ) {
						} else {
							curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
							curPlcmtResults.Rows[curIdx - 1]["Plcmt" + inEvent] = curPlcmt;
						}
					} else {
						curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
						curPlcmt += " ";
					}
					curRow["Plcmt" + inEvent] = curPlcmt;
					curPlcmtPos++;
				}
				curIdx++;
			}

			return curPlcmtResults;
		}

		private DataTable findEventPlcmtTiesGroup(String inPlcmtName, String inPlcmtOrg, DataTable inResults) {
            /* **********************************************************
             * Analyze event results to identify ties
             * ******************************************************* */
            String curPlcmt = "", curDiv = "", curGroup = "", curPlcmtValue = "", prevPlcmtValue = "";
            DataRowView newPlcmtRow;
            DataTable curPlcmtList = new DataTable();

            #region Build placement table to hold and calculate placements
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = inPlcmtName;
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curPlcmtList.Columns.Add( curCol );

            if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );

                curCol = new DataColumn();
                curCol.ColumnName = "EventGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                curCol = new DataColumn();
                curCol.ColumnName = "EventGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            } else {
                curCol = new DataColumn();
                curCol.ColumnName = "AgeGroup";
                curCol.DataType = System.Type.GetType( "System.String" );
                curCol.AllowDBNull = false;
                curCol.ReadOnly = false;
                curCol.DefaultValue = "";
                curPlcmtList.Columns.Add( curCol );
            }
            #endregion

            #region Find ties in initial placement determinations
            foreach ( DataRow curRow in inResults.Rows ) {
                curPlcmt = curRow[inPlcmtName].ToString();
                curPlcmtValue = "";
                if ( curPlcmt.Contains( "T" ) ) {
                    if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
                        curDiv = (String)curRow["AgeGroup"];
                        curPlcmtValue = curDiv + "-" + curPlcmt;
                    } else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                        curDiv = (String)curRow["AgeGroup"];
                        curGroup = (String)curRow["EventGroup"];
                        curPlcmtValue = curDiv + "-" + curGroup + "-" + curPlcmt;
                    } else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
                        curGroup = (String)curRow["EventGroup"];
                        curPlcmtValue = curGroup + "-" + curPlcmt;
                    } else {
                        curDiv = (String)curRow["AgeGroup"];
                        curPlcmtValue = curDiv + "-" + curPlcmt;
                    }

                    if ( !( curPlcmtValue.Equals( prevPlcmtValue ) ) ) {
                        newPlcmtRow = curPlcmtList.DefaultView.AddNew();
                        if ( curDiv.Length > 0 ) {
                            newPlcmtRow["AgeGroup"] = curDiv;
                        }
                        if ( curGroup.Length > 0 ) {
                            newPlcmtRow["EventGroup"] = curGroup;
                        }
                        newPlcmtRow[inPlcmtName] = curPlcmt;
                        newPlcmtRow.EndEdit();
                    }
                    prevPlcmtValue = curPlcmtValue;
                } else {
                    prevPlcmtValue = "";
                }
            }
            #endregion

            return curPlcmtList;
        }

        private DataTable findEventPlcmtTiesTour( String inPlcmtName, DataTable inResults ) {
            /* **********************************************************
             * Analyze event results to identify ties
             * ******************************************************* */
            String curPlcmt = "", curPlcmtValue = "", prevPlcmtValue = "";
            DataRowView newPlcmtRow;
            DataTable curPlcmtList = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = inPlcmtName;
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curPlcmtList.Columns.Add( curCol );

            foreach ( DataRow curRow in inResults.Rows ) {
                curPlcmt = curRow[inPlcmtName].ToString();
                curPlcmtValue = "";
                if ( curPlcmt.Contains( "T" ) ) {
                    curPlcmtValue = curPlcmt;
                    if ( !( curPlcmtValue.Equals( prevPlcmtValue ) ) ) {
                        newPlcmtRow = curPlcmtList.DefaultView.AddNew();
                        newPlcmtRow[inPlcmtName] = curPlcmt;
                        newPlcmtRow.EndEdit();
                    }
                    prevPlcmtValue = curPlcmtValue;
                } else {
                    prevPlcmtValue = "";
                }
            }

            return curPlcmtList;
        }

        private DataTable buildSkierSlalomScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtSlalom";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Speed";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "LineLength";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierTrickScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtTrick";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore1";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore2";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore3";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PassScore4";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierJumpScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtJump";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScoreFeet";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScoreMeters";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType("System.Decimal");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            return curDataTable;
        }

        private DataTable buildSkierOverallScoreList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "PlcmtTour";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "BackupScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FirstScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RunoffScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
        }

        private Decimal getOverallRunoffScore( String inMemberId, String inAgeGroup, DataTable inResultsAll ) {
            Decimal curReturnScore = 0;
            DataRow[] curFindRows = inResultsAll.Select(
                "MemberId = '" + inMemberId + "'"
                    + " AND AgeGroup = '" + inAgeGroup + "'"
                    + " AND Round >= 25 "
                );
            if ( curFindRows.Length > 0 ) {
                curReturnScore = (Decimal)curFindRows[0]["Score"];
            }
            return curReturnScore;
        }

    }
}
