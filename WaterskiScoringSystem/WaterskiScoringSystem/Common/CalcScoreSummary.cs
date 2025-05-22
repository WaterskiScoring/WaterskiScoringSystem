using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
	class CalcScoreSummary {
		private Decimal myMetericFactor = 3.2808M;
		private CalcEventPlcmt myCalcEventPlcmt;

		public CalcScoreSummary() {
			myCalcEventPlcmt = new CalcEventPlcmt();
		}

		public DataTable getSlalomSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			return getSlalomSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", 0 );
		}
		public DataTable getSlalomSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, Int16 inNumPrelimRounds ) {
			return getSlalomSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", inNumPrelimRounds );
		}
		public DataTable getSlalomSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup ) {
			return getSlalomSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, "All", 0 );
		}
		public DataTable getSlalomSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			return getSlalomSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, inDiv, 0 );
		}
		public DataTable getSlalomSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv, Int16 inNumPrelimRounds ) {
			String curSortCmd = "";
			DataTable curPlcmtDataTable = new DataTable();
			DataTable curScoreDataTable = new DataTable();
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curDataTable = getSlalomSummaryData( curSanctionId, inDataType, curRules, inEventGroup, inDiv );
			curScoreDataTable = expandScoreDataTable( curDataTable, "Slalom" );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
					curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg, inDataType, inNumPrelimRounds );
					curScoreDataTable = CalcPointsRoundPlcmt( inTourRow, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Slalom" );
				} else {
					curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
					curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg, inDataType, inNumPrelimRounds );
					curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Slalom" );
				}

			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else {
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmtSlalom DESC, SkierName ASC, RoundSlalom ASC";

			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundSlalom ASC, ReadyForPlcmtSlalom DESC, RunOrderGroup ASC, PlcmtSlalom ASC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC, RoundSlalom ASC";
				}

			} else if ( inPlcmtOrg.ToLower().Equals( "dihttps://duckduckgo.com/vgr" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundSlalom ASC, EventGroup ASC, ReadyForPlcmtSlalom DESC, RunOrderGroup ASC, PlcmtSlalom ASC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC, RoundSlalom ASC";
				}

			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundSlalom ASC, EventGroup ASC, ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC ";
				} else {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "EventGroup ASC, RoundSlalom ASC, ReadyForPlcmtSlalom DESC, RunOrderGroup ASC, PlcmtSlalom ASC, SkierName ASC ";
					} else {
						curSortCmd = "EventGroup ASC, ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC ";
					}
				}

			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundSlalom ASC, ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "RoundSlalom ASC, ReadyForPlcmtSlalom DESC, RunOrderGroup ASC, PlcmtSlalom ASC, SkierName ASC ";
				} else {
					curSortCmd = "ReadyForPlcmtSlalom DESC, PlcmtSlalom ASC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getSlalomScoreDetail( DataRow inTourRow, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			String inDataType = "round";
			String curSortCmd = "";
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];
			Int16 curSlalomRounds = 0;
			try {
				curSlalomRounds = Convert.ToInt16( inTourRow["SlalomRounds"].ToString() );
			} catch {
				curSlalomRounds = 0;
			}

			DataTable curScoreDetailData = getSlalomDetailData( curSanctionId, inEventGroup, inDiv );
			DataTable curScoreDataTable = expandScoreDataTable( curScoreDetailData, "Slalom" );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
				curScoreDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg );
				curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Slalom" );

			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Slalom" );
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
			} else {
				curScoreDataTable = buildOverallSummary( inTourRow, curScoreDataTable, null, null, inDataType );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, SkierName ASC, RoundSlalom ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, PlcmtSlalom ASC, SkierName ASC, RoundSlalom ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroupSlalom ASC, PlcmtSlalom ASC, SkierName ASC, RoundSlalom ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundSlalom ASC, EventGroupSlalom ASC, PlcmtSlalom ASC, SkierName ASC ";
				} else {
					curSortCmd = "EventGroupSlalom ASC, PlcmtSlalom ASC, SkierName ASC ";
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundSlalom ASC, PlcmtSlalom ASC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "SkierName ASC, RoundSlalom ASC ";
				} else {
					curSortCmd = "PlcmtSlalom ASC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getSlalomSummaryTeam( DataTable inScoreDataTable, DataRow inTourRow, int inNumPerTeam, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curTeamDataTable = getTeamData( curSanctionId, inPlcmtOrg, "Slalom", curRules );
			DataTable curTeamScoreDataTable = CalcTeamSummary( curTeamDataTable, inScoreDataTable, "Slalom", inNumPerTeam, inPlcmtMethod, inPlcmtOrg );

			if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "DivOrder ASC, Div ASC, TeamScoreSlalom Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "DivOrder ASC, Div ASC, TeamScoreSlalom Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else {
				curTeamScoreDataTable.DefaultView.Sort = "TeamScoreSlalom Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			}

			String curDiv = "", prevDiv = "", curPlcmtShow = "";
			int curPlcmt = 0, curIdx = 0;
			Decimal curScore = 0, prevScore = -1;
			foreach ( DataRow curRow in curTeamScoreDataTable.Rows ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curDiv = (String)curRow["Div"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curDiv = (String)curRow["Div"];
				} else {
					curDiv = "";
				}
				if ( curDiv != prevDiv ) {
					curPlcmt = 0;
					curScore = 0;
					prevScore = -1;
				}

				curScore = (Decimal)curRow["TeamScoreSlalom"];
				if ( curScore == prevScore && curIdx > 0 ) {
					curPlcmtShow = (String)curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtSlalom"];
					if ( curPlcmtShow.Contains( "T" ) ) {
					} else {
						curPlcmtShow = curPlcmtShow.Substring( 0, 3 ) + "T";
						curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtSlalom"] = curPlcmtShow;
					}
					curPlcmt++;
				} else {
					curPlcmt++;
					curPlcmtShow = curPlcmt.ToString( "##0" ).PadLeft( 3, ' ' );
					curPlcmtShow += " ";
				}

				curRow["PlcmtSlalom"] = curPlcmtShow;
				prevDiv = curDiv;
				prevScore = curScore;
				curIdx++;
			}

			return curTeamScoreDataTable;
		}

		public DataTable getTrickSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			return getTrickSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", 0 );
		}
		public DataTable getTrickSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, Int16 inNumPrelimRounds ) {
			return getTrickSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", inNumPrelimRounds );
		}
		public DataTable getTrickSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup ) {
			return getTrickSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, "All", 0 );
		}
		public DataTable getTrickSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			return getTrickSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, inDiv, 0 );
		}
		public DataTable getTrickSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv, Int16 inNumPrelimRounds ) {
			String curSortCmd = "";
			DataTable curPlcmtDataTable = new DataTable();
			DataTable curScoreDataTable = new DataTable();
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curDataTable = getTrickSummaryData( curSanctionId, inDataType, curRules, inEventGroup, inDiv );
			curScoreDataTable = expandScoreDataTable( curDataTable, "Trick" );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
					curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg, inDataType, inNumPrelimRounds );
					curScoreDataTable = CalcPointsRoundPlcmt( inTourRow, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Trick" );
				} else {
					curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
					curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg );
					curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Trick" );
				}

			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else {
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmtTrick DESC, SkierName ASC, RoundTrick ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundTrick ASC, ReadyForPlcmtTrick DESC, RunOrderGroup ASC, PlcmtTrick ASC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC, RoundTrick ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundTrick ASC, EventGroup ASC, ReadyForPlcmtTrick DESC, RunOrderGroup ASC, PlcmtTrick ASC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC, RoundTrick ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundTrick ASC, EventGroup ASC, ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC ";
				} else {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "EventGroup ASC, RoundTrick ASC, ReadyForPlcmtTrick DESC, RunOrderGroup ASC, PlcmtTrick ASC, SkierName ASC ";
					} else {
						curSortCmd = "EventGroup ASC, ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC ";
					}
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundTrick ASC, ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "RoundTrick ASC, ReadyForPlcmtTrick DESC, RunOrderGroup ASC, PlcmtTrick ASC, SkierName ASC ";
				} else {
					curSortCmd = "ReadyForPlcmtTrick DESC, PlcmtTrick ASC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getTrickScoreDetail( DataRow inTourRow, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			String inDataType = "round";
			String curSortCmd = "";
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];
			Int16 curTrickRounds = 0;
			try {
				curTrickRounds = Convert.ToInt16( inTourRow["TrickRounds"].ToString() );
			} catch {
				curTrickRounds = 0;
			}

			DataTable curScoreDetailData = getTrickDetailData( curSanctionId, inEventGroup, inDiv );
			DataTable curScoreDataTable = expandScoreDataTable( curScoreDetailData, "Trick" );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
				curScoreDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg );
				curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Trick" );
			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Trick" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, curScoreDataTable, null, inDataType );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, SkierName ASC, RoundTrick ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, PlcmtTrick ASC, SkierName ASC, RoundTrick ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, PlcmtTrick ASC, SkierName ASC, RoundTrick ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundTrick ASC, EventGroup ASC, PlcmtTrick ASC, SkierName ASC ";
				} else {
					curSortCmd = "EventGroup ASC, PlcmtTrick ASC, SkierName ASC ";
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundTrick ASC, PlcmtTrick ASC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "SkierName ASC, RoundTrick ASC ";
				} else {
					curSortCmd = "PlcmtTrick ASC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getTrickSummaryTeam( DataTable inScoreDataTable, DataRow inTourRow, int inNumPerTeam, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			return getTrickSummaryTeam( null, inScoreDataTable, inTourRow, inNumPerTeam, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod );
		}
		public DataTable getTrickSummaryTeam( DataTable inTeamScoreDataTable, DataTable inScoreDataTable, DataRow inTourRow, int inNumPerTeam, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];
			DataTable curTeamDataTable = getTeamData( curSanctionId, inPlcmtOrg, "Trick", curRules );
			DataTable curTeamScoreDataTable;
			if ( inTeamScoreDataTable == null ) {
				curTeamScoreDataTable = CalcTeamSummary( curTeamDataTable, inScoreDataTable, "Trick", inNumPerTeam, inPlcmtMethod, inPlcmtOrg );
			} else {
				curTeamScoreDataTable = CalcTeamSummary( inTeamScoreDataTable, curTeamDataTable, inScoreDataTable, "Trick", inNumPerTeam, inPlcmtMethod, inPlcmtOrg );
			}
			if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "Div ASC, TeamScoreTrick Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "DivOrder ASC, Div ASC, TeamScoreTrick Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else {
				curTeamScoreDataTable.DefaultView.Sort = "TeamScoreTrick Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			}
			String curDiv = "", prevDiv = "", curPlcmtShow = "";
			int curPlcmt = 0, curIdx = 0;
			Decimal curScore = 0, prevScore = -1;
			foreach ( DataRow curRow in curTeamScoreDataTable.Rows ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curDiv = (String)curRow["Div"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curDiv = (String)curRow["Div"];
				} else {
					curDiv = "";
				}
				if ( curDiv != prevDiv ) {
					curPlcmt = 0;
					curScore = 0;
					prevScore = -1;
				}

				curScore = (Decimal)curRow["TeamScoreTrick"];
				if ( curScore == prevScore && curIdx > 0 ) {
					curPlcmtShow = (String)curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtTrick"];
					if ( curPlcmtShow.Contains( "T" ) ) {
					} else {
						curPlcmtShow = curPlcmtShow.Substring( 0, 3 ) + "T";
						curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtTrick"] = curPlcmtShow;
					}
					curPlcmt++;
				} else {
					curPlcmt++;
					curPlcmtShow = curPlcmt.ToString( "##0" ).PadLeft( 3, ' ' );
					curPlcmtShow += " ";
				}

				curRow["PlcmtTrick"] = curPlcmtShow;
				prevDiv = curDiv;
				prevScore = curScore;
				curIdx++;
			}

			//curTeamScoreDataTable.DefaultView.Sort = "TeamScoreTotal Desc, Div ASC, TeamCode ASC ";
			//curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			return curTeamScoreDataTable;
		}

		public DataTable getJumpSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			return getJumpSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", 0 );
		}
		public DataTable getJumpSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, Int16 inNumPrelimRounds ) {
			return getJumpSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "All", "All", inNumPrelimRounds );
		}
		public DataTable getJumpSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup ) {
			return getJumpSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, "All", 0 );
		}
		public DataTable getJumpSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			return getJumpSummary( inTourRow, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, inDiv, 0 );
		}
		public DataTable getJumpSummary( DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv, Int16 inNumPrelimRounds ) {
			String curSortCmd = "";
			DataTable curPlcmtDataTable = new DataTable();
			DataTable curScoreDataTable = new DataTable();
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];
			Int16 curJumpRounds = 0;
			try {
				curJumpRounds = Convert.ToInt16( inTourRow["JumpRounds"].ToString() );
			} catch {
				curJumpRounds = 0;
			}

			DataTable curDataTable = getJumpSummaryData( curSanctionId, inDataType, curRules, inEventGroup, inDiv );
			curScoreDataTable = expandScoreJumpDataTable( curDataTable );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
					curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg, inDataType, inNumPrelimRounds );
					curScoreDataTable = CalcPointsRoundPlcmt( inTourRow, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Jump" );
				} else {
					curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
					curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, "score", inPlcmtOrg, inDataType, inNumPrelimRounds );
					curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Jump" );
				}

			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );

			} else {
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType, inNumPrelimRounds );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmtJump DESC, SkierName ASC, RoundJump ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundJump ASC, ReadyForPlcmtJump DESC, RunOrderGroup ASC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC, RoundJump ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundJump ASC, EventGroup ASC, ReadyForPlcmtJump DESC, RunOrderGroup ASC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC, RoundJump ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundJump ASC, EventGroup ASC, ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
				} else {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "EventGroup ASC, RoundJump ASC, ReadyForPlcmtJump DESC, RunOrderGroup ASC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
					} else {
						curSortCmd = "EventGroup ASC, ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
					}
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundJump ASC, ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "RoundJump ASC, ReadyForPlcmtJump DESC, RunOrderGroup ASC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
				} else {
					curSortCmd = "ReadyForPlcmtJump DESC, PlcmtJump ASC, ScoreFeet DESC, ScoreMeters DESC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getJumpScoreDetail( DataRow inTourRow, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			String inDataType = "round";
			String curSortCmd = "";
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];
			Int16 curJumpRounds = 0;
			try {
				curJumpRounds = Convert.ToInt16( inTourRow["JumpRounds"].ToString() );
			} catch {
				curJumpRounds = 0;
			}

			DataTable curScoreDetailData = getJumpDetailData( curSanctionId, inEventGroup, inDiv );
			DataTable curScoreDataTable = expandScoreDataTable( curScoreDetailData, "Jump" );

			if ( inPointsMethod.ToLower().Equals( "nops" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "plcmt" ) ) {
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
				curScoreDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curScoreDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, 0 );
				curScoreDataTable = CalcPointsPlcmt( curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, "Jump" );
			} else if ( inPointsMethod.ToLower().Equals( "kbase" ) ) {
				curScoreDataTable = CalcPointsKBase( curSanctionId, curScoreDataTable, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, curRules, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "hcap" ) ) {
				curScoreDataTable = CalcPointsHcap( curScoreDataTable, curSanctionId, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
			} else if ( inPointsMethod.ToLower().Equals( "ratio" ) ) {
				curScoreDataTable = CalcPointsRatio( curScoreDataTable, curSanctionId, "Jump" );
				curScoreDataTable = buildOverallSummary( inTourRow, null, null, curScoreDataTable, inDataType );
			}

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, SkierName ASC, RoundJump ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, PlcmtJump ASC, SkierName ASC, RoundJump ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, PlcmtJump ASC, SkierName ASC, RoundJump ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundJump ASC, EventGroup ASC, PlcmtJump ASC, SkierName ASC ";
				} else {
					curSortCmd = "EventGroup ASC, PlcmtJump ASC, SkierName ASC ";
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundJump ASC, PlcmtJump ASC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "SkierName ASC, RoundJump ASC ";
				} else {
					curSortCmd = "PlcmtJump ASC, SkierName ASC ";
				}
			}

			curScoreDataTable.DefaultView.Sort = curSortCmd;
			curScoreDataTable = curScoreDataTable.DefaultView.ToTable();
			return curScoreDataTable;
		}

		public DataTable getJumpSummaryTeam( DataTable inScoreDataTable, DataRow inTourRow, int inNumPerTeam, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			return getJumpSummaryTeam( null, inScoreDataTable, inTourRow, inNumPerTeam, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod );
		}
		public DataTable getJumpSummaryTeam( DataTable inTeamScoreDataTable, DataTable inScoreDataTable, DataRow inTourRow, int inNumPerTeam, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod ) {
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curTeamDataTable = getTeamData( curSanctionId, inPlcmtOrg, "Jump", curRules );
			DataTable curTeamScoreDataTable;
			if ( inTeamScoreDataTable == null ) {
				curTeamScoreDataTable = CalcTeamSummary( curTeamDataTable, inScoreDataTable, "Jump", inNumPerTeam, inPlcmtMethod, inPlcmtOrg );
			} else {
				curTeamScoreDataTable = CalcTeamSummary( inTeamScoreDataTable, curTeamDataTable, inScoreDataTable, "Jump", inNumPerTeam, inPlcmtMethod, inPlcmtOrg );
			}
			if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "Div ASC, TeamScoreJump Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curTeamScoreDataTable.DefaultView.Sort = "DivOrder ASC, Div ASC, TeamScoreJump Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			} else {
				curTeamScoreDataTable.DefaultView.Sort = "TeamScoreJump Desc, TeamCode ASC ";
				curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			}
			String curDiv = "", prevDiv = "", curPlcmtShow = "";
			int curPlcmt = 0, curIdx = 0;
			Decimal curScore = 0, prevScore = -1;
			foreach ( DataRow curRow in curTeamScoreDataTable.Rows ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curDiv = (String)curRow["Div"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curDiv = (String)curRow["Div"];
				} else {
					curDiv = "";
				}
				if ( curDiv != prevDiv ) {
					curPlcmt = 0;
					curScore = 0;
					prevScore = -1;
				}

				curScore = (Decimal)curRow["TeamScoreJump"];
				if ( curScore == prevScore && curIdx > 0 ) {
					curPlcmtShow = (String)curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtJump"];
					if ( curPlcmtShow.Contains( "T" ) ) {
					} else {
						curPlcmtShow = curPlcmtShow.Substring( 0, 3 ) + "T";
						curTeamScoreDataTable.Rows[curIdx - 1]["PlcmtJump"] = curPlcmtShow;
					}
					curPlcmt++;
				} else {
					curPlcmt++;
					curPlcmtShow = curPlcmt.ToString( "##0" ).PadLeft( 3, ' ' );
					curPlcmtShow += " ";
				}

				curRow["PlcmtJump"] = curPlcmtShow;
				prevDiv = curDiv;
				prevScore = curScore;
				curIdx++;
			}
			//curTeamScoreDataTable.DefaultView.Sort = "TeamScoreTotal Desc, Div ASC, TeamCode ASC ";
			//curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			return curTeamScoreDataTable;
		}

		public DataTable buildOverallSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, String inDataType ) {
			return buildOverallSummary( inTourRow, inSlalomDataTable, inTrickDataTable, inJumpDataTable, inDataType, "", 0 );
		}
		public DataTable buildOverallSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, String inDataType, Int16 inNumPrelimRounds ) {
			return buildOverallSummary( inTourRow, inSlalomDataTable, inTrickDataTable, inJumpDataTable, inDataType, "", inNumPrelimRounds );
		}
		public DataTable buildOverallSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, String inDataType, String inPlcmtOrg ) {
			return buildOverallSummary( inTourRow, inSlalomDataTable, inTrickDataTable, inJumpDataTable, inDataType, inPlcmtOrg, 0 );
		}
		public DataTable buildOverallSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, String inDataType, String inPlcmtOrg, Int16 inNumPrelimRounds ) {
			String curSortCmd;
			bool calcOverallActive = ( inSlalomDataTable != null ) && ( inSlalomDataTable.Rows.Count > 0 )
				&& ( inTrickDataTable != null ) && ( inTrickDataTable.Rows.Count > 0 )
				&& ( inJumpDataTable != null ) && ( inTrickDataTable.Rows.Count > 0 );

			DataTable curSummaryDataTable = buildOverallSummaryDataTable();
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			if ( inSlalomDataTable != null ) {
				//Load slalom data into overall summary data table
				buildOverallSummarySlalom( curSummaryDataTable, inTourRow, inSlalomDataTable, inDataType, inPlcmtOrg, inNumPrelimRounds, calcOverallActive );
			}

			if ( inTrickDataTable != null ) {
				//Load trick data into overall summary data table
				buildOverallSummaryTrick( curSummaryDataTable, inTourRow, inTrickDataTable, inDataType, inPlcmtOrg, inNumPrelimRounds, calcOverallActive );
			}

			if ( inJumpDataTable != null ) {
				//Load jump data into overall summary data table
				buildOverallSummaryJump( curSummaryDataTable, inTourRow, inJumpDataTable, inDataType, inPlcmtOrg, inNumPrelimRounds, calcOverallActive );
			}

			// Build overall data
			if ( HelperFunctions.isAwsaEvent(curRules) && calcOverallActive && !( curSanctionId.EndsWith( "999" ) ) && !( curSanctionId.EndsWith( "998" ) ) ) {
				checkForEliteOverallDragdown( inTourRow, curSummaryDataTable, inDataType, inPlcmtOrg );
			}
			buildOverallSummaryOverall( curSummaryDataTable, inTourRow, inDataType, inPlcmtOrg, inNumPrelimRounds, calcOverallActive );

			if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
				curSortCmd = "AgeGroup ASC, ReadyForPlcmt DESC, SkierName ASC, RoundOverall ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "agegroupgroup" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroupOverall ASC, ReadyForPlcmt DESC, SkierName ASC, RoundOverall ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "AgeGroup ASC, RoundOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC, RoundOverall ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) ) {
					curSortCmd = "AgeGroup ASC, EventGroupOverall ASC, RoundOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC";
				} else {
					curSortCmd = "AgeGroup ASC, EventGroupOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC, RoundOverall ASC";
				}
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				if ( inDataType.ToLower().Equals( "round" ) ) {
					curSortCmd = "EventGroupOverall ASC, RoundOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC";
				} else {
					curSortCmd = "EventGroupOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC, RoundOverall ASC";
				}
			} else {
				if ( inDataType.ToLower().Equals( "total" ) && HelperFunctions.isCollegiateEvent(curRules) ) {
					curSortCmd = "RoundOverall ASC, ReadyForPlcmt DESC, ScoreOverall DESC, SkierName ASC ";
				} else if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curSortCmd = "ScoreOverall DESC, ReadyForPlcmt DESC, RoundOverall ASC, SkierName ASC ";
				} else {
					curSortCmd = "ScoreOverall DESC, ReadyForPlcmt DESC, SkierName ASC ";
				}
			}

			curSummaryDataTable.DefaultView.Sort = curSortCmd;
			curSummaryDataTable = curSummaryDataTable.DefaultView.ToTable();
			return curSummaryDataTable;
		}

		/*
		 * Load slalom data into overall summary data table
		 */
		public void buildOverallSummarySlalom( DataTable curSummaryDataTable, DataRow inTourRow, DataTable inSlalomDataTable, String inDataType, String inPlcmtOrg, Int16 inNumPrelimRounds, bool calcOverallActive ) {
			String curFilterCommand, curSanctionId, curSkierName, curMemberId, curEventGroup, curEventClass, curAgeGroup, curRunOrderGroup, curFinalLenOff, curFinalLen, curStartLen;
			String curState, curFederation, curCity, curReadyToSki, curReadyForPlcmt, curReadyForPlcmtSlalom;
			String curPlcmtSlalom, curTeamCode;
			String curScoreName, curPointsName, curRoundName, curGroupName, curTeamName, curOrderName, curClassName;

			Decimal curScore, curPoints, curFinalPassScore, curHCapScore, curHCapBase;
			Int16 curRound;
			int curDivOrder = 0;
			Byte curFinalSpeedKph, curFinalSpeedMph, curCompletedSpeedMph, curCompletedSpeedKph, curMaxSpeed, curStartSpeed, curSkiYearAge;

			DataRowView newDataRow;
			DataRow curDataRow;
			DataRow[] curFindRows;

			if ( inSlalomDataTable.Columns.Contains( "EventClassSlalom" ) ) {
				curScoreName = "ScoreSlalom";
				curPointsName = "PointsSlalom";
				curRoundName = "RoundSlalom";
				curOrderName = "DivOrderSlalom";
				curClassName = "EventClassSlalom";
				curGroupName = "EventGroupSlalom";
				curTeamName = "TeamSlalom";
			} else {
				curScoreName = "Score";
				curPointsName = "NopsScore";
				curRoundName = "Round";
				curOrderName = "DivOrder";
				curClassName = "EventClass";
				curGroupName = "EventGroup";
				curTeamName = "TeamCode";
			}

			foreach ( DataRow curRow in inSlalomDataTable.Rows ) {
				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curRoundName, "0" ), out curRound );
				if ( curRound >= 25 ) continue;

				curSanctionId = (String)curRow["SanctionId"];
				curMemberId = (String)curRow["MemberId"];
				curSkierName = (String)curRow["SkierName"];

				/*
				 * DLA: 2025/02/09
				 * This statement checkcs to see if the skier skipped a round and adjusts the rounds for missing scores
				 * This is what the rules state to ensure fairness for overall scores (skiers skiing in the same considitions)
				 * However it is causing way too many problems when calculating placements for all the different requirements.
				 * Therefore I'm disabling it at this time.
				 */
				//if ( !( calcOverallActive ) ) curRound = checkForEventScore( "Slalom", curSanctionId, curMemberId, curRound );

				curCity = HelperFunctions.getDataRowColValue( curRow, "City", "" );
				curState = HelperFunctions.getDataRowColValue( curRow, "State", "" );
				curFederation = HelperFunctions.getDataRowColValue( curRow, "Federation", "" );

				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "" );
				curReadyForPlcmt = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmt", "" );
				curReadyForPlcmtSlalom = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmtSlalom", "" );

				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "SkiYearAge", "0" ), out curSkiYearAge );
				curEventGroup = HelperFunctions.getDataRowColValue( curRow, "EventGroupRound", "" );
				if ( HelperFunctions.isObjectEmpty( curEventGroup ) ) curEventGroup = HelperFunctions.getDataRowColValue( curRow, curGroupName, "" );
				curEventClass = HelperFunctions.getDataRowColValue( curRow, curClassName, HelperFunctions.getDataRowColValue( inTourRow, "Class", "C" ) );
				curTeamCode = HelperFunctions.getDataRowColValue( curRow, curTeamName, "" );
				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, curOrderName, "0" ), out curDivOrder );

				curAgeGroup = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curRunOrderGroup = HelperFunctions.getDataRowColValue( curRow, "RunOrderGroup", "" );
				curPlcmtSlalom = HelperFunctions.getDataRowColValue( curRow, "PlcmtSlalom", "" );

				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curPointsName, "0" ), out curPoints );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapScore", "0" ), out curHCapScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapBase", "0" ), out curHCapBase );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "FinalPassScore", "0" ), out curFinalPassScore );

				curStartLen = HelperFunctions.getDataRowColValue( curRow, "StartLen", "23.00" );
				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "FinalSpeedKph", "25" ), out curFinalSpeedKph );
				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "FinalSpeedMph", "15" ), out curFinalSpeedMph );
				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "MaxSpeed", "25" ), out curMaxSpeed );
				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "StartSpeed", "25" ), out curStartSpeed );
				if ( curStartSpeed > curMaxSpeed ) {
					curFinalSpeedKph = curStartSpeed;
					curFinalSpeedMph = getSlalomSpeedMph( curFinalSpeedKph );
				}

				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "CompletedSpeedKph", "25" ), out curCompletedSpeedKph );
				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "CompletedSpeedMph", "15" ), out curCompletedSpeedMph );

				curFinalLenOff = HelperFunctions.getDataRowColValue( curRow, "FinalLenOff", "" );
				curFinalLen = HelperFunctions.getDataRowColValue( curRow, "FinalLen", "" );

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And RoundOverall = " + curRound
						+ " And AgeGroup = '" + curAgeGroup + "'"
						;

				} else {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And AgeGroup = '" + curAgeGroup + "'";
				}

				curFindRows = curSummaryDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					if ( (inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) )
						&& curRound <= inNumPrelimRounds && (Int16)curFindRows[0]["RoundSlalom"] <= inNumPrelimRounds && (decimal)curFindRows[0]["ScoreSlalom"] >= curScore ) continue;

					curDataRow = curFindRows[0];
					curDataRow["RunOrderGroup"] = curRunOrderGroup;
					curDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					curDataRow["ReadyForPlcmtSlalom"] = curReadyForPlcmtSlalom;
					curDataRow["EventClassSlalom"] = curEventClass;
					curDataRow["TeamSlalom"] = curTeamCode;
					curDataRow["EventGroupSlalom"] = curEventGroup;
					curDataRow["DivOrderSlalom"] = curDivOrder;
					curDataRow["PlcmtSlalom"] = curPlcmtSlalom;
					curDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						curDataRow["RoundSlalom"] = inNumPrelimRounds;
					} else {
						curDataRow["RoundSlalom"] = curRound;
					}

					curDataRow["ScoreSlalom"] = curScore;
					curDataRow["PointsSlalom"] = curPoints;
					curDataRow["HCapScoreSlalom"] = curHCapScore;
					curDataRow["HCapBaseSlalom"] = curHCapBase;

					curDataRow["FinalLen"] = curFinalLen;
					curDataRow["FinalLenOff"] = curFinalLenOff;
					curDataRow["FinalSpeedKph"] = curFinalSpeedKph;
					curDataRow["FinalSpeedMph"] = curFinalSpeedMph;
					curDataRow["FinalPassScore"] = curFinalPassScore;
					curDataRow["CompletedSpeedMph"] = curCompletedSpeedMph;
					curDataRow["CompletedSpeedKph"] = curCompletedSpeedKph;

					curDataRow["MaxSpeed"] = curMaxSpeed;
					curDataRow["StartSpeed"] = curStartSpeed;
					curDataRow["StartLen"] = curStartLen;

					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) || inDataType.ToLower().Equals( "final" ) ) {
						curDataRow["RoundOverall"] = curRound;
					}

				} else {
					newDataRow = curSummaryDataTable.DefaultView.AddNew();
					newDataRow["SanctionId"] = curSanctionId;
					newDataRow["MemberId"] = curMemberId;
					newDataRow["SkierName"] = curSkierName;
					newDataRow["AgeGroup"] = curAgeGroup;
					newDataRow["RunOrderGroup"] = curRunOrderGroup;
					newDataRow["State"] = curState;
					newDataRow["Federation"] = curFederation;
					newDataRow["City"] = curCity;
					newDataRow["SkiYearAge"] = curSkiYearAge;
					newDataRow["DivOrderSlalom"] = curDivOrder;

					newDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					newDataRow["ReadyForPlcmtSlalom"] = curReadyForPlcmtSlalom;
					newDataRow["EventClassSlalom"] = curEventClass;
					newDataRow["TeamSlalom"] = curTeamCode;
					newDataRow["EventGroupSlalom"] = curEventGroup;
					newDataRow["PlcmtSlalom"] = curPlcmtSlalom;
					newDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						newDataRow["RoundSlalom"] = inNumPrelimRounds;
					} else {
						newDataRow["RoundSlalom"] = curRound;
					}

					newDataRow["ScoreSlalom"] = curScore;
					newDataRow["PointsSlalom"] = curPoints;
					newDataRow["HCapScoreSlalom"] = curHCapScore;
					newDataRow["HCapBaseSlalom"] = curHCapBase;

					newDataRow["FinalLen"] = curFinalLen;
					newDataRow["FinalLenOff"] = curFinalLenOff;
					newDataRow["FinalSpeedKph"] = curFinalSpeedKph;
					newDataRow["FinalSpeedMph"] = curFinalSpeedMph;
					newDataRow["FinalPassScore"] = curFinalPassScore;
					newDataRow["CompletedSpeedMph"] = curCompletedSpeedMph;
					newDataRow["CompletedSpeedKph"] = curCompletedSpeedKph;

					newDataRow["MaxSpeed"] = curMaxSpeed;
					newDataRow["StartSpeed"] = curStartSpeed;
					newDataRow["StartLen"] = curStartLen;

					newDataRow["RoundOverall"] = curRound;
					newDataRow.EndEdit();
				}
			}
		}

		//Load trick data into overall summary data table
		public void buildOverallSummaryTrick( DataTable curSummaryDataTable, DataRow inTourRow, DataTable inTrickDataTable, String inDataType, String inPlcmtOrg, Int16 inNumPrelimRounds, bool calcOverallActive ) {
			String curFilterCommand, curSanctionId, curSkierName, curMemberId, curEventGroup, curEventClass, curAgeGroup, curRunOrderGroup;
			String curState, curFederation, curCity, curReadyToSki, curReadyForPlcmt, curReadyForPlcmtTrick;
			String curPlcmtTrick, curTeamCode;
			String curScoreName, curPointsName, curRoundName, curGroupName, curTeamName, curOrderName, curClassName, curScorePass1Name, curScorePass2Name;
			Decimal curPoints, curHCapScore, curHCapBase;

			Int16 curRound, curScoreTrick, curScorePass1, curScorePass2;
			int curDivOrder = 0;
			Byte curSkiYearAge;

			DataRowView newDataRow;
			DataRow curDataRow;
			DataRow[] curFindRows;

			if ( inTrickDataTable.Columns.Contains( "EventClassTrick" ) ) {
				curScoreName = "ScoreTrick";
				curScorePass1Name = "Pass1Trick";
				curScorePass2Name = "Pass2Trick";
				curPointsName = "PointsTrick";
				curRoundName = "RoundTrick";
				curOrderName = "DivOrderTrick";
				curClassName = "EventClassTrick";
				curGroupName = "EventGroupTrick";
				curTeamName = "TeamTrick";
			} else {
				curScoreName = "Score";
				curScorePass1Name = "ScorePass1";
				curScorePass2Name = "ScorePass2";
				curPointsName = "NopsScore";
				curRoundName = "Round";
				curOrderName = "DivOrder";
				curClassName = "EventClass";
				curGroupName = "EventGroup";
				curTeamName = "TeamCode";
			}

			foreach ( DataRow curRow in inTrickDataTable.Rows ) {
				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curRoundName, "0" ), out curRound );
				if ( curRound >= 25 ) continue;

				curSanctionId = (String)curRow["SanctionId"];
				curMemberId = (String)curRow["MemberId"];
				curSkierName = (String)curRow["SkierName"];

                /*
				 * DLA: 2025/02/09
				 * This statement checkcs to see if the skier skipped a round and adjusts the rounds for missing scores
				 * This is what the rules state to ensure fairness for overall scores (skiers skiing in the same considitions)
				 * However it is causing way too many problems when calculating placements for all the different requirements.
				 * Therefore I'm disabling it at this time.
				 */
                //if ( !( calcOverallActive ) ) curRound = checkForEventScore( "Trick", curSanctionId, curMemberId, curRound );

                curCity = HelperFunctions.getDataRowColValue( curRow, "City", "" );
				curState = HelperFunctions.getDataRowColValue( curRow, "State", "" );
				curFederation = HelperFunctions.getDataRowColValue( curRow, "Federation", "" );

				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "" );
				curReadyForPlcmt = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmt", "" );
				curReadyForPlcmtTrick = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmtTrick", "" );

				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "SkiYearAge", "0" ), out curSkiYearAge );
				curEventGroup = HelperFunctions.getDataRowColValue( curRow, "EventGroupRound", "" );
				if ( HelperFunctions.isObjectEmpty( curEventGroup ) ) curEventGroup = HelperFunctions.getDataRowColValue( curRow, curGroupName, "" );
				curEventClass = HelperFunctions.getDataRowColValue( curRow, curClassName, HelperFunctions.getDataRowColValue( inTourRow, "Class", "C" ) );
				curTeamCode = HelperFunctions.getDataRowColValue( curRow, curTeamName, "" );
				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, curOrderName, "0" ), out curDivOrder );

				curAgeGroup = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curRunOrderGroup = HelperFunctions.getDataRowColValue( curRow, "RunOrderGroup", "" );
				curPlcmtTrick = HelperFunctions.getDataRowColValue( curRow, "PlcmtTrick", "" );

				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScoreTrick );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curScorePass1Name, "0" ), out curScorePass1 );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curScorePass2Name, "0" ), out curScorePass2 );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curPointsName, "0" ), out curPoints );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapScore", "0" ), out curHCapScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapBase", "0" ), out curHCapBase );

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And RoundOverall = " + curRound
						+ " And AgeGroup = '" + curAgeGroup + "'";
				} else {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And AgeGroup = '" + curAgeGroup + "'";
				}
				curFindRows = curSummaryDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					if ( ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) )
						&& curRound <= inNumPrelimRounds && (Int16)curFindRows[0]["RoundTrick"] <= inNumPrelimRounds && (Int16)curFindRows[0]["ScoreTrick"] >= curScoreTrick ) continue;

					curDataRow = curFindRows[0];
					curDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					curDataRow["ReadyForPlcmtTrick"] = curReadyForPlcmtTrick;
					curDataRow["EventClassTrick"] = curEventClass;
					curDataRow["TeamTrick"] = curTeamCode;
					curDataRow["EventGroupTrick"] = curEventGroup;
					curDataRow["DivOrderTrick"] = curDivOrder;
					curDataRow["PlcmtTrick"] = curPlcmtTrick;
					curDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						curDataRow["RoundTrick"] = inNumPrelimRounds;
					} else {
						curDataRow["RoundTrick"] = curRound;
					}

					curDataRow["ScoreTrick"] = curScoreTrick;
					curDataRow["Pass1Trick"] = curScorePass1;
					curDataRow["Pass2Trick"] = curScorePass2;
					curDataRow["PointsTrick"] = curPoints;
					curDataRow["HCapScoreTrick"] = curHCapScore;
					curDataRow["HCapBaseTrick"] = curHCapBase;

					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) || inDataType.ToLower().Equals( "final" ) ) {
						curDataRow["RoundOverall"] = curRound;
					}

				} else {
					newDataRow = curSummaryDataTable.DefaultView.AddNew();
					newDataRow["SanctionId"] = curSanctionId;
					newDataRow["MemberId"] = curMemberId;
					newDataRow["SkierName"] = curSkierName;
					newDataRow["AgeGroup"] = curAgeGroup;
					newDataRow["DivOrderTrick"] = curDivOrder;
					newDataRow["State"] = curState;
					newDataRow["Federation"] = curFederation;
					newDataRow["City"] = curCity;
					newDataRow["SkiYearAge"] = curSkiYearAge;

					newDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					newDataRow["ReadyForPlcmtTrick"] = curReadyForPlcmtTrick;
					newDataRow["EventClassTrick"] = curEventClass;
					newDataRow["TeamTrick"] = curTeamCode;
					newDataRow["EventGroupTrick"] = curEventGroup;
					newDataRow["PlcmtTrick"] = curPlcmtTrick;
					newDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						newDataRow["RoundTrick"] = inNumPrelimRounds;
					} else {
						newDataRow["RoundTrick"] = curRound;
					}

					newDataRow["ScoreTrick"] = curScoreTrick;
					newDataRow["Pass1Trick"] = curScorePass1;
					newDataRow["Pass2Trick"] = curScorePass2;
					newDataRow["PointsTrick"] = curPoints;
					newDataRow["HCapScoreTrick"] = curHCapScore;
					newDataRow["HCapBaseTrick"] = curHCapBase;

					newDataRow["RoundOverall"] = curRound;
					newDataRow.EndEdit();
				}
			}
		}

		public void buildOverallSummaryJump( DataTable curSummaryDataTable, DataRow inTourRow, DataTable inJumpDataTable, String inDataType, String inPlcmtOrg, Int16 inNumPrelimRounds, bool calcOverallActive ) {
			String curFilterCommand, curSanctionId, curSkierName, curMemberId, curEventGroup, curEventClass, curAgeGroup, curRunOrderGroup;
			String curState, curFederation, curCity, curReadyToSki, curReadyForPlcmt, curReadyForPlcmtJump;
			String curPlcmtJump, curTeamCode;
			String curPointsName, curRoundName, curGroupName, curTeamName, curOrderName, curClassName, curBoatSpeedName;
			Decimal curPoints, curFeet, curMeters, curRampHeight, curMetersBackup, curHCapScore, curHCapBase;
			Int16 curRound;
			int curDivOrder = 0;
			Byte curBoatSpeedJump, curSkiYearAge;

			DataRowView newDataRow;
			DataRow curDataRow;
			DataRow[] curFindRows;

			//Load jump data into overall summary data table
			if ( inJumpDataTable.Columns.Contains( "EventClassJump" ) ) {
				curPointsName = "PointsJump";
				curRoundName = "RoundJump";
				curOrderName = "DivOrderJump";
				curClassName = "EventClassJump";
				curGroupName = "EventGroupJump";
				curBoatSpeedName = "SpeedKphJump";
				curTeamName = "TeamJump";
			} else {
				curPointsName = "NopsScore";
				curRoundName = "Round";
				curOrderName = "DivOrder";
				curClassName = "EventClass";
				curGroupName = "EventGroup";
				curBoatSpeedName = "BoatSpeed";
				curTeamName = "TeamCode";
			}

			String curRules = (String)inTourRow["Rules"];

			foreach ( DataRow curRow in inJumpDataTable.Rows ) {
				Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, curRoundName, "0" ), out curRound );
				if ( curRound >= 25 ) continue;

				curSanctionId = (String)curRow["SanctionId"];
				curMemberId = (String)curRow["MemberId"];
				curSkierName = (String)curRow["SkierName"];

                /*
				 * DLA: 2025/02/09
				 * This statement checkcs to see if the skier skipped a round and adjusts the rounds for missing scores
				 * This is what the rules state to ensure fairness for overall scores (skiers skiing in the same considitions)
				 * However it is causing way too many problems when calculating placements for all the different requirements.
				 * Therefore I'm disabling it at this time.
				 */
                //if ( !( calcOverallActive ) ) curRound = checkForEventScore( "Jump", curSanctionId, curMemberId, curRound );

				curCity = HelperFunctions.getDataRowColValue( curRow, "City", "" );
				curState = HelperFunctions.getDataRowColValue( curRow, "State", "" );
				curFederation = HelperFunctions.getDataRowColValue( curRow, "Federation", "" );

				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "" );
				curReadyForPlcmt = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmt", "" );
				curReadyForPlcmtJump = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmtJump", "" );

				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "SkiYearAge", "0" ), out curSkiYearAge );
				curEventGroup = HelperFunctions.getDataRowColValue( curRow, "EventGroupRound", "" );
				if ( HelperFunctions.isObjectEmpty( curEventGroup ) ) curEventGroup = HelperFunctions.getDataRowColValue( curRow, curGroupName, "" );
				curEventClass = HelperFunctions.getDataRowColValue( curRow, curClassName, HelperFunctions.getDataRowColValue( inTourRow, "Class", "C" ) );
				curTeamCode = HelperFunctions.getDataRowColValue( curRow, curTeamName, "" );
				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, curOrderName, "0" ), out curDivOrder );

				curAgeGroup = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curRunOrderGroup = HelperFunctions.getDataRowColValue( curRow, "RunOrderGroup", "" );
				curPlcmtJump = HelperFunctions.getDataRowColValue( curRow, "PlcmtJump", "" );

				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curPointsName, "0" ), out curPoints );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "0" ), out curFeet );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "0" ), out curMeters );
				if ( HelperFunctions.isCollegiateEvent( curRules ) && curMeters == 0 && curFeet == 0 ) {
					Boolean curEligForScore = isSkierEligNcwsaJumpScore( curSanctionId, curMemberId, curAgeGroup, curRound );
					if ( !curEligForScore ) curMeters = (decimal)-1.0;
				}
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMetersBackup", "0" ), out curMetersBackup );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapScore", "0" ), out curHCapScore );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "HCapBase", "0" ), out curHCapBase );

				byte.TryParse( HelperFunctions.getDataRowColValue( curRow, curBoatSpeedName, "0" ), out curBoatSpeedJump );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "RampHeight", "0" ), out curRampHeight );

				if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And RoundOverall = " + curRound
						+ " And AgeGroup = '" + curAgeGroup + "'";
				} else {
					curFilterCommand = "MemberId = '" + curMemberId + "'"
						+ " And AgeGroup = '" + curAgeGroup + "'";
				}
				curFindRows = curSummaryDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					if ( ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) )
						&& curRound <= inNumPrelimRounds && (Int16)curFindRows[0]["RoundJump"] <= inNumPrelimRounds && (decimal)curFindRows[0]["ScoreFeet"] >= curFeet ) continue;

					curDataRow = curFindRows[0];
					curDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					curDataRow["ReadyForPlcmtJump"] = curReadyForPlcmtJump;
					curDataRow["EventClassJump"] = curEventClass;
					curDataRow["TeamJump"] = curTeamCode;
					curDataRow["EventGroupJump"] = curEventGroup;
					curDataRow["DivOrderJump"] = curDivOrder;
					curDataRow["PlcmtJump"] = curPlcmtJump;
					curDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						curDataRow["RoundJump"] = inNumPrelimRounds;
					} else {
						curDataRow["RoundJump"] = curRound;
					}

					curDataRow["ScoreMeters"] = curMeters;
					curDataRow["ScoreFeet"] = curFeet;
					curDataRow["PointsJump"] = curPoints;
					curDataRow["HCapScoreJump"] = curHCapScore;
					curDataRow["HCapBaseJump"] = curHCapBase;
					curDataRow["SpeedKphJump"] = curBoatSpeedJump;
					curDataRow["RampHeight"] = curRampHeight;

					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) || inDataType.ToLower().Equals( "final" ) ) {
						curDataRow["RoundOverall"] = curRound;
					}

				} else {
					newDataRow = curSummaryDataTable.DefaultView.AddNew();
					newDataRow["SanctionId"] = curSanctionId;
					newDataRow["MemberId"] = curMemberId;
					newDataRow["SkierName"] = curSkierName;
					newDataRow["AgeGroup"] = curAgeGroup;
					newDataRow["DivOrderJump"] = curDivOrder;
					newDataRow["State"] = curState;
					newDataRow["Federation"] = curFederation;
					newDataRow["City"] = curCity;
					newDataRow["SkiYearAge"] = curSkiYearAge;

					newDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					newDataRow["ReadyForPlcmtJump"] = curReadyForPlcmtJump;
					newDataRow["EventClassJump"] = curEventClass;
					newDataRow["TeamJump"] = curTeamCode;
					newDataRow["EventGroupJump"] = curEventGroup;
					newDataRow["RoundJump"] = curRound;
					newDataRow["PlcmtJump"] = curPlcmtJump;
					newDataRow["Round"] = curRound;
					if ( curRound <= inNumPrelimRounds && ( inDataType.ToLower().Equals( "final" ) || inDataType.ToLower().Equals( "h2h" ) ) ) {
						newDataRow["RoundJump"] = inNumPrelimRounds;
					} else {
						newDataRow["RoundJump"] = curRound;
					}

					newDataRow["ScoreMeters"] = curMeters;
					newDataRow["ScoreFeet"] = curFeet;
					newDataRow["PointsJump"] = curPoints;
					newDataRow["HCapScoreJump"] = curHCapScore;
					newDataRow["HCapBaseJump"] = curHCapBase;
					newDataRow["SpeedKphJump"] = curBoatSpeedJump;
					newDataRow["RampHeight"] = curRampHeight;

					newDataRow["RoundOverall"] = curRound;
					newDataRow.EndEdit();
				}
			}
		}

		public void buildOverallSummaryOverall( DataTable curSummaryDataTable, DataRow inTourRow, String inDataType, String inPlcmtOrg, Int16 inNumPrelimRounds, bool calcOverallActive ) {
			String curMemberId, curAgeGroup, curReadyToSki, curReadyForPlcmt;
			String curGroupSlalom = "", curGroupTrick = "", curGroupJump = "";
			Int16 curEventsReqd, curEventCount;
			int curDivOrderSlalom, curDivOrderTrick, curDivOrderJump, curScoreTrick;
			decimal curScoreSlalom, curScoreMeters, curScoreFeet;
			DataRow[] curFindRows;

			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curEligSkiersDataTable = getOverallSkierList( curSanctionId );

			foreach ( DataRow curRow in curSummaryDataTable.Rows ) {
				curRow["ScoreOverall"] = (Decimal)curRow["PointsSlalom"]
					+ (Decimal)curRow["PointsTrick"]
					+ (Decimal)curRow["PointsJump"];

				curMemberId = (String)curRow["MemberId"];
				curAgeGroup = (String)curRow["AgeGroup"];

				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "" );
				curReadyForPlcmt = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmt", "" );

				curGroupSlalom = HelperFunctions.getDataRowColValue( curRow, "EventGroupSlalom", "" );
				curGroupTrick = HelperFunctions.getDataRowColValue( curRow, "EventGroupTrick", "" );
				curGroupJump = HelperFunctions.getDataRowColValue( curRow, "EventGroupJump", "" );

				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, "DivOrderSlalom", "0" ), out curDivOrderSlalom );
				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, "DivOrderTrick", "0" ), out curDivOrderTrick );
				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, "DivOrderJump", "0" ), out curDivOrderJump );

				Int32.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreTrick", "0" ), out curScoreTrick );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreSlalom", "0" ), out curScoreSlalom );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "0" ), out curScoreMeters );
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "0" ), out curScoreFeet );

				curRow["QualifyOverall"] = "No";
				curRow["EligOverall"] = "No";
				curEventCount = 0;

				//Need to check for a score greater than before incrementing the event count that is used to determine overall qualified.
				if ( curGroupSlalom.Length > 0 ) {
					if ( HelperFunctions.isAwsaEvent(curRules) || HelperFunctions.isCollegiateEvent( curRules ) ) {
						curEventCount++;
					} else {
						if ( curScoreSlalom > 0 ) curEventCount++;
					}
				}
				if ( curGroupTrick.Length > 0 ) {
					if ( HelperFunctions.isAwsaEvent(curRules) || HelperFunctions.isCollegiateEvent( curRules ) ) {
						curEventCount++;
					} else {
						if ( curScoreTrick > 0 ) curEventCount++;
					}
				}
				if ( curGroupJump.Length > 0 ) {
					if ( HelperFunctions.isAwsaEvent(curRules) || HelperFunctions.isCollegiateEvent( curRules ) ) {
						curEventCount++;
					} else {
						if ( curScoreFeet > 0 && curScoreMeters > 0 ) curEventCount++;
					}
				}

				curFindRows = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				if ( curFindRows.Length > 0 ) {
					curRow["EligOverall"] = "Yes";
					Int16.TryParse( HelperFunctions.getDataRowColValue( curFindRows[0], "EventsReqd", "3" ), out curEventsReqd );
				} else {
					curEventsReqd = 3;
				}

				if ( curEventsReqd == 3 && curEventCount > 2 ) {
					curRow["QualifyOverall"] = "Yes";
				} else if ( curEventsReqd == 2 && curEventCount > 1 ) {
					curRow["QualifyOverall"] = "Yes";
				} else {
					//curRow["ScoreOverall"] = 0;
				}

				if ( curGroupSlalom.Length > 0 ) {
					curRow["EventGroupOverall"] = curGroupSlalom;
					curRow["DivOrderOverall"] = curDivOrderSlalom;
					curRow["EventGroup"] = curGroupSlalom;
				} else if ( curGroupTrick.Length > 0 ) {
					curRow["EventGroupOverall"] = curGroupTrick;
					curRow["DivOrderOverall"] = curDivOrderTrick;
					curRow["EventGroup"] = curGroupTrick;
				} else if ( curGroupJump.Length > 0 ) {
					curRow["EventGroupOverall"] = curGroupJump;
					curRow["DivOrderOverall"] = curDivOrderJump;
					curRow["EventGroup"] = curGroupJump;
				} else {
					curRow["EventGroupOverall"] = curAgeGroup;
					curRow["DivOrderOverall"] = 0;
					curRow["EventGroup"] = curAgeGroup;
				}
			}
		}

		private Boolean checkForEliteOverallDragdown( DataRow inTourRow, DataTable curSummaryDataTable, String inDataType, String inPlcmtOrg ) {
			Boolean returnValue = false;
			String curFilterCommand = "";
			DataRow[] curFindRows = null;
			DataRowView newDataRow;
			Boolean addRows = curSummaryDataTable.Rows.Count == 0;

			/*
			 * If calculating overall scores (scores for all 3 events are available)
			 * Check to determine if there are elite skiers with age group scores and are eligible for overall
			 * Retrieve elite scores and build the proper array of scores for those divisions
			 * Than these scores should be made available to check for dragdown scenarios
			 */
			DataTable curDataTable = getEliteSkiersInAgeGroup( "Slalom", (String)inTourRow["SanctionId"] );
			if (curDataTable.Rows.Count > 0) {
				foreach (DataRow curEliteRow in curDataTable.Rows) {
					DataRow curScoreRow = null;
					curFilterCommand = "MemberId = '" + (String)curEliteRow["MemberId"] + "'"
						+ " And AgeGroup = '" + (String)curEliteRow["AgeGroup"] + "'"
						+ " And RoundOverall = " + (byte)curEliteRow["Round"];
					curFindRows = curSummaryDataTable.Select(curFilterCommand);
					if (curFindRows.Length > 0 ) {
						if ( curScoreRow == null || ( (String)curScoreRow["EventClassSlalom"]).Length == 0 ) {
							curScoreRow = curFindRows[0];
							if (((String)curScoreRow["EventClassSlalom"]).Length > 0) curScoreRow = null;
						}

					} else if (addRows) {
						int nextRowIdx = curSummaryDataTable.Rows.Count;
						newDataRow = curSummaryDataTable.DefaultView.AddNew();
						newDataRow["SanctionId"] = (String)curEliteRow["SanctionId"];
						newDataRow["MemberId"] = (String)curEliteRow["MemberId"];
						newDataRow["AgeGroup"] = (String)curEliteRow["AgeGroup"];
						newDataRow["SkierName"] = (String)curEliteRow["SkierName"];
						newDataRow.EndEdit();
						curScoreRow = curSummaryDataTable.Rows[nextRowIdx];
					}

					if (curScoreRow != null) {
						returnValue = true;
						curScoreRow["ReadyForPlcmtSlalom"] = "N";
						curScoreRow["EventClassSlalom"] = (String)curEliteRow["EventClass"];
						//curScoreRow["TeamSlalom"] = "";
						curScoreRow["EventGroupSlalom"] = (String)curEliteRow["AgeGroup"];
						//curScoreRow["DivOrderSlalom"] = (Int32)curEliteRow["DivOrderSlalom"];
						curScoreRow["RoundSlalom"] = (Byte)curEliteRow["Round"];
						curScoreRow["RoundOverall"] = (Byte)curEliteRow["Round"];
						curScoreRow["Round"] = (Byte)curEliteRow["Round"];
						curScoreRow["PlcmtSlalom"] = "*";

						curScoreRow["ScoreSlalom"] = (Decimal)curEliteRow["Score"];
						curScoreRow["PointsSlalom"] = (Decimal)curEliteRow["NopsScore"];
						//curScoreRow["HCapScoreSlalom"] = (Decimal)curEliteRow["HCapScoreSlalom"];
						//curScoreRow["HCapBaseSlalom"] = (Decimal)curEliteRow["HCapBaseSlalom"];

						curScoreRow["FinalLen"] = (String)curEliteRow["FinalLen"];
						curScoreRow["FinalLenOff"] = (String)curEliteRow["FinalLenOff"];
						curScoreRow["FinalSpeedKph"] = (Byte)curEliteRow["FinalSpeedKph"];
						curScoreRow["FinalSpeedMph"] = (Byte)curEliteRow["FinalSpeedMph"];
						curScoreRow["FinalPassScore"] = (Decimal)curEliteRow["FinalPassScore"];
						curScoreRow["CompletedSpeedMph"] = (Byte)curEliteRow["CompletedSpeedMph"];
						curScoreRow["CompletedSpeedKph"] = (Byte)curEliteRow["CompletedSpeedKph"];

						curScoreRow["MaxSpeed"] = (Byte)curEliteRow["MaxSpeed"];
						curScoreRow["StartSpeed"] = (Byte)curEliteRow["StartSpeed"];
						curScoreRow["StartLen"] = (String)curEliteRow["StartLen"];
					}
				}
			}

			curDataTable = getEliteSkiersInAgeGroup( "Trick", (String)inTourRow["SanctionId"] );
			if ( curDataTable.Rows.Count > 0 ) {
				foreach ( DataRow curEliteRow in curDataTable.Rows ) {
					DataRow curScoreRow = null;
					curFilterCommand = "MemberId = '" + (String)curEliteRow["MemberId"] + "'"
						+ " And AgeGroup = '" + (String)curEliteRow["AgeGroup"] + "'"
						+ " And RoundOverall = " + (byte)curEliteRow["Round"];
					curFindRows = curSummaryDataTable.Select(curFilterCommand);
					if (curFindRows.Length > 0) {
						if ( curScoreRow == null || ( (String)curScoreRow["EventClassTrick"]).Length == 0 ) {
							curScoreRow = curFindRows[0];
							if (((String)curScoreRow["EventClassTrick"]).Length > 0) curScoreRow = null;
						}

					} else if (addRows) {
						int nextRowIdx = curSummaryDataTable.Rows.Count;
						newDataRow = curSummaryDataTable.DefaultView.AddNew();
						newDataRow["SanctionId"] = (String)curEliteRow["SanctionId"];
						newDataRow["MemberId"] = (String)curEliteRow["MemberId"];
						newDataRow["AgeGroup"] = (String)curEliteRow["AgeGroup"];
						newDataRow["SkierName"] = (String)curEliteRow["SkierName"];
						newDataRow.EndEdit();
						curScoreRow = curSummaryDataTable.Rows[nextRowIdx];
					}

					if (curScoreRow != null) {
						returnValue = true;
						curScoreRow["ReadyForPlcmtTrick"] = "N";
						curScoreRow["EventClassTrick"] = (String)curEliteRow["EventClass"];
						//curScoreRow["TeamTrick"] = "";
						curScoreRow["EventGroupTrick"] = (String)curEliteRow["AgeGroup"];
						//curScoreRow["DivOrderTrick"] = (Int32)curEliteRow["DivOrder"];
						curScoreRow["RoundTrick"] = (Byte)curEliteRow["Round"];
						curScoreRow["RoundOverall"] = (Byte)curEliteRow["Round"];
						curScoreRow["Round"] = (Byte)curEliteRow["Round"];
						curScoreRow["PlcmtTrick"] = "*";

						curScoreRow["ScoreTrick"] = (Int16)curEliteRow["Score"];
						curScoreRow["Pass1Trick"] = (Int16)curEliteRow["ScorePass1"];
						curScoreRow["Pass2Trick"] = (Int16)curEliteRow["ScorePass2"];
						curScoreRow["PointsTrick"] = (Decimal)curEliteRow["NopsScore"];
						//curScoreRow["HCapScoreTrick"] = (Decimal)curEliteRow["HCapScoreTrick"];
						//curScoreRow["HCapBaseTrick"] = (Decimal)curEliteRow["HCapBaseTrick"];
					}
				}
			}

			curDataTable = getEliteSkiersInAgeGroup( "Jump", (String)inTourRow["SanctionId"] );
			if ( curDataTable.Rows.Count > 0 ) {
				foreach ( DataRow curEliteRow in curDataTable.Rows ) {
					DataRow curScoreRow = null;
					curFilterCommand = "MemberId = '" + (String)curEliteRow["MemberId"] + "'"
						+ " And AgeGroup = '" + (String)curEliteRow["AgeGroup"] + "'"
						+ " And RoundOverall = " + (byte)curEliteRow["Round"];
					curFindRows = curSummaryDataTable.Select(curFilterCommand);
					if (curFindRows.Length > 0) {
						if ( curScoreRow == null || ( (String)curScoreRow["EventClassJump"]).Length == 0) {
							curScoreRow = curFindRows[0];
							if (((String)curScoreRow["EventClassJump"]).Length > 0) curScoreRow = null;
						}

					} else if (addRows) {
						int nextRowIdx = curSummaryDataTable.Rows.Count;
						newDataRow = curSummaryDataTable.DefaultView.AddNew();
						newDataRow["SanctionId"] = (String)curEliteRow["SanctionId"];
						newDataRow["MemberId"] = (String)curEliteRow["MemberId"];
						newDataRow["AgeGroup"] = (String)curEliteRow["AgeGroup"];
						newDataRow["SkierName"] = (String)curEliteRow["SkierName"];
						newDataRow.EndEdit();
						curScoreRow = curSummaryDataTable.Rows[nextRowIdx];
					}

					if (curScoreRow != null) {
						returnValue = true;
						curScoreRow["ReadyForPlcmtJump"] = "N";
						curScoreRow["EventClassJump"] = (String)curEliteRow["EventClass"];
						//curScoreRow["TeamJump"] = "";
						curScoreRow["EventGroupJump"] = (String)curEliteRow["AgeGroup"];
						//curScoreRow["DivOrderJump"] = (Int32)curEliteRow["DivOrderSlalom"];
						curScoreRow["RoundJump"] = (Byte)curEliteRow["Round"];
						curScoreRow["RoundOverall"] = (Byte)curEliteRow["Round"];
						curScoreRow["Round"] = (Byte)curEliteRow["Round"];
						curScoreRow["PlcmtJump"] = "*";

						curScoreRow["ScoreFeet"] = (Decimal)curEliteRow["ScoreFeet"];
						curScoreRow["ScoreMeters"] = (Decimal)curEliteRow["ScoreMeters"];
						curScoreRow["PointsJump"] = (Decimal)curEliteRow["NopsScore"];
						//curScoreRow["HCapScoreJump"] = (Decimal)curEliteRow["HCapScoreJump"];
						//curScoreRow["HCapBaseJump"] = (Decimal)curEliteRow["HCapBaseJump"];

						curScoreRow["SpeedKphJump"] = (Byte)curEliteRow["BoatSpeed"];
						curScoreRow["RampHeight"] = (Decimal)curEliteRow["RampHeight"];
					}
				}
			}
			return returnValue;
		}

		public DataTable buildTourScorebook( String inSanctionId, DataRow inTourRow, DataTable inMemberData, DataTable inSummaryDataTable, DataTable inSlalomDetail, DataTable inTrickDetail, DataTable inJumpDetail ) {
			String curMemberId, curAgeGroup, curReadyToSki, curReadyForPlcmt;
			Int16 curTourRounds = 0, curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0, curEventCount = 0;
			Int16 curRoundSlalom, curRoundTrick, curRoundJump;
			Decimal curOverallScore = 0;
			DataRow curSummaryRow = null, curSlalomDetailRow = null, curTrickDetailRow = null, curJumpDetailRow = null, curOverallRow = null;
			DataRowView newDataRow = null;
			DataRow[] curFindRows;
			Int16 curEventsReqd = 0;
			String curRules = (String)inTourRow["Rules"];

			DataTable curEligSkiersDataTable = getOverallSkierList( inSanctionId );
			DataTable curSummaryDataTable = buildOverallSummaryDataTable();

			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "SlalomRounds", "0" ), out curSlalomRounds);
			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "TrickRounds", "0" ), out curTrickRounds);
			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "JumpRounds", "0" ), out curJumpRounds);
			if ( curSlalomRounds > curTourRounds ) { curTourRounds = curSlalomRounds; }
			if ( curTrickRounds > curTourRounds ) { curTourRounds = curTrickRounds; }
			if ( curJumpRounds > curTourRounds ) { curTourRounds = curJumpRounds; }

			DataTable curEliteSummaryDataTable = null;
			if ( curSlalomRounds > 0 && curTrickRounds  > 0 && curJumpRounds  > 0 ) {
				curEliteSummaryDataTable = buildOverallSummaryDataTable();
				checkForEliteOverallDragdown( inTourRow, curEliteSummaryDataTable, "points", "round" );
			}

			byte curMaxSpeed, curStartSpeed;
			decimal curPoints;

			foreach ( DataRow curRow in inMemberData.Rows ) {
				//Initialize output buffer

				curMemberId = curRow["MemberId"].ToString();
				curAgeGroup = curRow["AgeGroup"].ToString();

				curReadyForPlcmt = HelperFunctions.getDataRowColValue( curRow, "ReadyForPlcmt", "N" );
				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "N" );
				if ( !curReadyToSki.ToUpper().Equals( "Y" ) ) continue;

				curFindRows = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				if ( curFindRows.Length > 0 ) {
					curOverallRow = curFindRows[0];
					Int16.TryParse( HelperFunctions.getDataRowColValue( curOverallRow, "EventsReqd", "3.0" ), out curEventsReqd );
				} else {
					curOverallRow = null;
					curEventsReqd = 3;
				}
				curSummaryRow = getEventMemberEntry( inSummaryDataTable, curMemberId, curAgeGroup );
				if ( curSummaryRow == null ) continue;

				Int16.TryParse( HelperFunctions.getDataRowColValue( curSummaryRow, "RoundSlalom", "0" ), out curRoundSlalom );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curSummaryRow, "RoundTrick", "0" ), out curRoundTrick );
				Int16.TryParse( HelperFunctions.getDataRowColValue( curSummaryRow, "RoundJump", "0" ), out curRoundJump );
				if ( curRoundSlalom == 0 && curRoundTrick == 0 && curRoundJump == 0 ) continue; // If no rounds are found then skier has no detail data so will be excluded from scorebook

				for ( int curRound = 1; curRound <= curTourRounds; curRound++ ) {
					curOverallScore = 0M;
					curEventCount = 0;

					newDataRow = curSummaryDataTable.DefaultView.AddNew();
					newDataRow["SanctionId"] = inSanctionId;
					newDataRow["MemberId"] = curMemberId;
					newDataRow["ReadyToSki"] = curReadyToSki;
					newDataRow["ReadyForPlcmt"] = curReadyForPlcmt;
					newDataRow["SkierName"] = (String)curRow["SkierName"];
					newDataRow["AgeGroup"] = curAgeGroup;

					#region Process slalom information
					curSlalomDetailRow = getEventMemberEntry( inSlalomDetail, curMemberId, curAgeGroup, curRound );
					if ( curSlalomDetailRow == null && curEliteSummaryDataTable != null ) curSlalomDetailRow = getEventMemberEntry( curEliteSummaryDataTable, curMemberId, curAgeGroup, curRound );
					if ( curSlalomDetailRow != null && ( ( (String)curSlalomDetailRow["EventClassSlalom"] ).Length > 0 ) ) {
						curEventCount++;
						newDataRow["EventClassSlalom"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "EventClassSlalom", "" );
						newDataRow["EventGroupSlalom"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "EventGroupSlalom", "" );
						newDataRow["ReadyForPlcmtSlalom"] = HelperFunctions.getDataRowColValue( curSummaryRow, "ReadyForPlcmtSlalom", "" );
						newDataRow["TeamSlalom"] = HelperFunctions.getDataRowColValue( curSummaryRow, "TeamSlalom", "" );

						newDataRow["RoundSlalom"] = curRound;
						newDataRow["PlcmtSlalom"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtSlalom", "" );
						newDataRow["FinalLen"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalLen", "" );
						newDataRow["FinalLenOff"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalLenOff", "" );
						newDataRow["StartLen"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "StartLen", "" );

						byte.TryParse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "MaxSpeed", "0" ), out curMaxSpeed );
						byte.TryParse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "StartSpeed", "0" ), out curStartSpeed );
						newDataRow["MaxSpeed"] = curMaxSpeed;
						newDataRow["StartSpeed"] = curStartSpeed;

						if ( curStartSpeed > curMaxSpeed ) {
							newDataRow["FinalSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalSpeedKph", "0" ) );
							newDataRow["FinalSpeedMph"] = getSlalomSpeedMph( curStartSpeed );
						} else {
							newDataRow["FinalSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalSpeedKph", "0" ) );
							newDataRow["FinalSpeedMph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalSpeedMph", "0" ) );
						}

						newDataRow["CompletedSpeedMph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "CompletedSpeedMph", "0" ) );
						newDataRow["CompletedSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "CompletedSpeedKph", "0" ) );
						newDataRow["FinalPassScore"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "FinalPassScore", "0" ) );

						newDataRow["ScoreSlalom"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "ScoreSlalom", "0" ) );
						curPoints = decimal.Parse( HelperFunctions.getDataRowColValue( curSlalomDetailRow, "PointsSlalom", "0" ) );
						newDataRow["PointsSlalom"] = curPoints;
						curOverallScore += curPoints;
					}
					#endregion

					#region Process trick information
					curTrickDetailRow = getEventMemberEntry( inTrickDetail, curMemberId, curAgeGroup, curRound );
					if ( curTrickDetailRow == null && curEliteSummaryDataTable != null ) curTrickDetailRow = getEventMemberEntry( curEliteSummaryDataTable, curMemberId, curAgeGroup, curRound );
					if ( curTrickDetailRow != null && ( ( (String)curTrickDetailRow["EventClassTrick"] ).Length > 0 ) ) {
						curEventCount++;
						newDataRow["EventClassTrick"] = HelperFunctions.getDataRowColValue( curTrickDetailRow, "EventClassTrick", "" );
						newDataRow["EventGroupTrick"] = HelperFunctions.getDataRowColValue( curTrickDetailRow, "EventGroupTrick", "" );
						newDataRow["ReadyForPlcmtTrick"] = HelperFunctions.getDataRowColValue( curSummaryRow, "ReadyForPlcmtTrick", "" );
						newDataRow["TeamTrick"] = HelperFunctions.getDataRowColValue( curSummaryRow, "TeamTrick", "" );

						newDataRow["RoundTrick"] = curRound;
						newDataRow["PlcmtTrick"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtTrick", "" );

						newDataRow["ScoreTrick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curTrickDetailRow, "ScoreTrick", "0" ) );
						newDataRow["Pass1Trick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curTrickDetailRow, "Pass1Trick", "0" ) );
						newDataRow["Pass2Trick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curTrickDetailRow, "Pass2Trick", "0" ) );

						curPoints = decimal.Parse( HelperFunctions.getDataRowColValue( curTrickDetailRow, "PointsTrick", "0" ) );
						newDataRow["PointsTrick"] = curPoints;
						curOverallScore += curPoints;
					}
					#endregion

					#region Process Jump information
					curJumpDetailRow = getEventMemberEntry( inJumpDetail, curMemberId, curAgeGroup, curRound );
					if ( curJumpDetailRow == null && curEliteSummaryDataTable != null ) curJumpDetailRow = getEventMemberEntry( curEliteSummaryDataTable, curMemberId, curAgeGroup, curRound );
					if ( curJumpDetailRow != null && ( ( (String)curJumpDetailRow["EventClassJump"] ).Length > 0 ) ) {
						curEventCount++;
						newDataRow["EventClassJump"] = HelperFunctions.getDataRowColValue( curJumpDetailRow, "EventClassJump", "" );
						newDataRow["EventGroupJump"] = HelperFunctions.getDataRowColValue( curJumpDetailRow, "EventGroupJump", "" );
						newDataRow["ReadyForPlcmtJump"] = HelperFunctions.getDataRowColValue( curSummaryRow, "ReadyForPlcmtJump", "" );
						newDataRow["TeamJump"] = HelperFunctions.getDataRowColValue( curSummaryRow, "TeamJump", "" );

						newDataRow["RoundJump"] = curRound;
						newDataRow["PlcmtJump"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtJump", "" );

						newDataRow["ScoreMeters"] = decimal.Parse( HelperFunctions.getDataRowColValue( curJumpDetailRow, "ScoreMeters", "0" ) );
						newDataRow["ScoreFeet"] = decimal.Parse( HelperFunctions.getDataRowColValue( curJumpDetailRow, "ScoreFeet", "0" ) );
						newDataRow["SpeedKphJump"] = byte.Parse( HelperFunctions.getDataRowColValue( curJumpDetailRow, "SpeedKphJump", "0" ) );
						newDataRow["RampHeight"] = decimal.Parse( HelperFunctions.getDataRowColValue( curJumpDetailRow, "RampHeight", "0" ) );

						curPoints = decimal.Parse( HelperFunctions.getDataRowColValue( curJumpDetailRow, "PointsJump", "0" ) );
						newDataRow["PointsJump"] = curPoints;
						curOverallScore += curPoints;
					}
					#endregion

					#region Process overall information
					newDataRow["RoundOverall"] = curRound;
					newDataRow["Round"] = curRound;

					newDataRow["QualifyOverall"] = "No";
					newDataRow["EligOverall"] = "No";
					if ( curOverallRow != null ) newDataRow["EligOverall"] = "Yes";

					if ( curEventsReqd == 3 && curEventCount == 3 ) {
						newDataRow["QualifyOverall"] = "Yes";
						if ( curOverallScore > 0 ) {
							newDataRow["ScoreOverall"] = curOverallScore.ToString( "###0.0" );
						}
					} else if ( curEventsReqd == 2 && curEventCount > 1 ) {
						newDataRow["QualifyOverall"] = "Yes";
						if ( curOverallScore > 0 ) {
							newDataRow["ScoreOverall"] = curOverallScore.ToString( "###0.0" );
						}
					}
					if ( curSlalomDetailRow != null ) {
						newDataRow["EventGroupOverall"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "EventGroupSlalom", "" );
						newDataRow["EventGroup"] = HelperFunctions.getDataRowColValue( curSlalomDetailRow, "EventGroupSlalom", "" );

					} else if ( curTrickDetailRow != null ) {
						newDataRow["EventGroupOverall"] = HelperFunctions.getDataRowColValue( curTrickDetailRow, "EventGroupTrick", "" );
						newDataRow["EventGroup"] = HelperFunctions.getDataRowColValue( curTrickDetailRow, "EventGroupTrick", "" );

					} else if ( curJumpDetailRow != null ) {
						newDataRow["EventGroupOverall"] = HelperFunctions.getDataRowColValue( curJumpDetailRow, "EventGroupJump", "" );
						newDataRow["EventGroup"] = HelperFunctions.getDataRowColValue( curJumpDetailRow, "EventGroupJump", "" );

					} else {
						newDataRow["EventGroupOverall"] = HelperFunctions.getDataRowColValue( curSummaryRow, "EventGroupOverall", "" );
						newDataRow["EventGroup"] = HelperFunctions.getDataRowColValue( curSummaryRow, "EventGroupOverall", "" );
					}

					newDataRow.EndEdit();
					#endregion
				}
			}

			return curSummaryDataTable;
		}

		public DataTable buildTourScorebookIwwf( String inSanctionId, DataRow inTourRow, DataTable inMemberData, DataTable inSlalomDetail, DataTable inTrickDetail, DataTable inJumpDetail, DataTable inSummaryDataTable ) {
			String curMemberId, curAgeGroup, curReadyToSki, curEventClassSlalom, curEventClassTrick, curEventClassJump;
			Int16 curTourRounds = 0, curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
			DataRow curSlalomRow = null, curTrickRow = null, curJumpRow = null, curSummaryRow = null, curOverallRow = null;
			DataRowView newDataRow = null;
			DataRow[] curFindRows;
			Int16 curEventsReqd = 0;
			byte curMaxSpeed, curStartSpeed;

			DataTable curEligSkiersDataTable = getOverallSkierList( inSanctionId );
			DataTable curSummaryDataTable = buildOverallSummaryDataTable();

			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "SlalomRounds", "0" ), out curSlalomRounds );
			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "TrickRounds", "0" ), out curTrickRounds );
			Int16.TryParse( HelperFunctions.getDataRowColValue( inTourRow, "JumpRounds", "0" ), out curJumpRounds );
			if ( curSlalomRounds > curTourRounds ) { curTourRounds = curSlalomRounds; }
			if ( curTrickRounds > curTourRounds ) { curTourRounds = curTrickRounds; }
			if ( curJumpRounds > curTourRounds ) { curTourRounds = curJumpRounds; }

			foreach ( DataRow curRow in inMemberData.Rows ) {
				curMemberId = curRow["MemberId"].ToString();
				curAgeGroup = curRow["AgeGroup"].ToString();
				
				curReadyToSki = HelperFunctions.getDataRowColValue( curRow, "ReadyToSki", "N" );
				if ( !curReadyToSki.ToUpper().Equals( "Y" ) ) continue;

				curFindRows = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				if ( curFindRows.Length > 0 ) {
					curOverallRow = curFindRows[0];
					Int16.TryParse( HelperFunctions.getDataRowColValue( curOverallRow, "EventsReqd", "3" ), out curEventsReqd );
				} else {
					curOverallRow = null;
					curEventsReqd = 3;
				}

				for ( int curRound = 1; curRound <= curTourRounds; curRound++ ) {
					curSummaryRow = getEventMemberEntry( inSummaryDataTable, curMemberId, curAgeGroup, curRound );
					if ( curSummaryRow == null ) continue;

					curSlalomRow = getEventMemberEntry( inSlalomDetail, curMemberId, curAgeGroup, curRound );
					curTrickRow = getEventMemberEntry( inTrickDetail, curMemberId, curAgeGroup, curRound );
					curJumpRow = getEventMemberEntry( inJumpDetail, curMemberId, curAgeGroup, curRound );

					newDataRow = curSummaryDataTable.DefaultView.AddNew();
					newDataRow["SanctionId"] = inSanctionId;
					newDataRow["MemberId"] = curMemberId;
					newDataRow["ReadyToSki"] = curReadyToSki;
					newDataRow["SkierName"] = (String)curRow["SkierName"];
					newDataRow["AgeGroup"] = curAgeGroup;

					curEventClassSlalom = HelperFunctions.getDataRowColValue( curSummaryRow, "EventClassSlalom", "" );
					curEventClassTrick = HelperFunctions.getDataRowColValue( curSummaryRow, "EventClassTrick", "" );
					curEventClassJump = HelperFunctions.getDataRowColValue( curSummaryRow, "EventClassJump", "" );

					if ( curSlalomRow != null && HelperFunctions.isObjectPopulated( curEventClassSlalom ) ) {
						newDataRow["EventClassSlalom"] = HelperFunctions.getDataRowColValue( curSlalomRow, "EventClass", "" );
						newDataRow["TeamSlalom"] = HelperFunctions.getDataRowColValue( curSlalomRow, "TeamCode", "" );
						newDataRow["EventGroupSlalom"] = HelperFunctions.getDataRowColValue( curSlalomRow, "EventGroup", "" );

						newDataRow["RoundSlalom"] = curRound;
						newDataRow["PlcmtSlalom"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtSlalom", "" );

						newDataRow["ScoreSlalom"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "ScoreSlalom", "0" ) );
						newDataRow["PointsSlalom"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "PointsSlalom", "0" ) );

						newDataRow["FinalLen"] = HelperFunctions.getDataRowColValue( curSlalomRow, "FinalLen", "" );
						newDataRow["FinalLenOff"] = HelperFunctions.getDataRowColValue( curSlalomRow, "FinalLenOff", "" );
						newDataRow["StartLen"] = HelperFunctions.getDataRowColValue( curSlalomRow, "StartLen", "" );

						byte.TryParse( HelperFunctions.getDataRowColValue( curSummaryRow, "MaxSpeed", "0" ), out curMaxSpeed );
						byte.TryParse( HelperFunctions.getDataRowColValue( curSummaryRow, "StartSpeed", "0" ), out curStartSpeed );
						newDataRow["MaxSpeed"] = curMaxSpeed;
						newDataRow["StartSpeed"] = curStartSpeed;

						if ( curStartSpeed > curMaxSpeed ) {
							newDataRow["FinalSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "FinalSpeedKph", "0" ) );
							newDataRow["FinalSpeedMph"] = getSlalomSpeedMph( curStartSpeed );
						} else {
							newDataRow["FinalSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "FinalSpeedKph", "0" ) );
							newDataRow["FinalSpeedMph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "FinalSpeedMph", "0" ) );
						}

						newDataRow["CompletedSpeedMph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomRow, "CompletedSpeedMph", "0" ) );
						newDataRow["CompletedSpeedKph"] = byte.Parse( HelperFunctions.getDataRowColValue( curSlalomRow, "CompletedSpeedKph", "0" ) );
						newDataRow["FinalPassScore"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSlalomRow, "FinalPassScore", "0" ) );
					}

					if ( curTrickRow != null && HelperFunctions.isObjectPopulated( curEventClassTrick ) ) {
						newDataRow["EventClassTrick"] = HelperFunctions.getDataRowColValue( curTrickRow, "EventClass", "" );
						newDataRow["EventGroupTrick"] = HelperFunctions.getDataRowColValue( curTrickRow, "EventGroup", "" );
						newDataRow["TeamTrick"] = HelperFunctions.getDataRowColValue( curTrickRow, "TeamCode", "" );

						newDataRow["RoundTrick"] = curRound;
						newDataRow["PlcmtTrick"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtTrick", "" );

						newDataRow["ScoreTrick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "ScoreTrick", "0" ) );
						newDataRow["Pass1Trick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "Pass1Trick", "0" ) );
						newDataRow["Pass2Trick"] = Int16.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "Pass2Trick", "0" ) );
						newDataRow["PointsTrick"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "PointsTrick", "0" ) );
					}

					if ( curJumpRow != null && HelperFunctions.isObjectPopulated( curEventClassJump ) ) {
						newDataRow["EventClassJump"] = HelperFunctions.getDataRowColValue( curJumpRow, "EventClass", "" );
						newDataRow["EventGroupJump"] = HelperFunctions.getDataRowColValue( curJumpRow, "EventGroup", "" );
						newDataRow["TeamJump"] = HelperFunctions.getDataRowColValue( curJumpRow, "TeamCode", "" );

						newDataRow["RoundJump"] = curRound;
						newDataRow["PlcmtJump"] = HelperFunctions.getDataRowColValue( curSummaryRow, "PlcmtJump", "" );

						newDataRow["ScoreMeters"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "ScoreMeters", "0" ) );
						newDataRow["ScoreFeet"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "ScoreFeet", "0" ) );
						newDataRow["SpeedKphJump"] = byte.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "SpeedKphJump", "0" ) );
						newDataRow["RampHeight"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "RampHeight", "0" ) );
						newDataRow["PointsJump"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "PointsJump", "0" ) );
					}

					newDataRow["RoundOverall"] = curRound;
					newDataRow["Round"] = curRound;

					newDataRow["QualifyOverall"] = HelperFunctions.getDataRowColValue( curSummaryRow, "QualifyOverall", "" );
					newDataRow["EligOverall"] = HelperFunctions.getDataRowColValue( curSummaryRow, "EligOverall", "" );
					newDataRow["EventGroupOverall"] = HelperFunctions.getDataRowColValue( curSummaryRow, "EventGroupOverall", "" );
					newDataRow["EventGroup"] = HelperFunctions.getDataRowColValue( curSummaryRow, "EventGroup", "" );
					newDataRow["ScoreOverall"] = decimal.Parse( HelperFunctions.getDataRowColValue( curSummaryRow, "ScoreOverall", "0" ) );

					newDataRow.EndEdit();
				}
			}

			return curSummaryDataTable;
		}

		public DataTable buildHeadToHeadSummary( DataTable inSummaryDataTable, DataRow inTourRow, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, Int16 inNumPrelimRounds, String inEvent ) {
			DataTable curPrelimScoreDataTable = new DataTable();
			DataTable curFinalScoreDataTable = new DataTable();
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			if ( inSummaryDataTable.Rows.Count == 0 ) return null;

			// Select all scores for skiers that participated in elimination rounds
			inSummaryDataTable.DefaultView.RowFilter = "Round" + inEvent + " > " + inNumPrelimRounds.ToString();
			curFinalScoreDataTable = inSummaryDataTable.DefaultView.ToTable();

			// Select all scores in the preliminary rounds to calculate placement positions of skiers that didn't make the elimination rounds
			inSummaryDataTable.DefaultView.RowFilter = "Round" + inEvent + " <= " + inNumPrelimRounds.ToString();
			curPrelimScoreDataTable = inSummaryDataTable.DefaultView.ToTable();

			// The placement position for skiers that do not make the elimination or head to head rounds 
			// is equal to the number of skiers that participated in the first elimination round
			DataRow[] curFindRows = inSummaryDataTable.Select( "Round" + inEvent + " = " + ( inNumPrelimRounds + 1 ).ToString() );
			DataTable curFinalPlcmtCountPerGroup = calcFinalPlcmtCountPerGroup( curFindRows, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEvent );

			if ( inDataType == "h2h" ) {
				curFinalScoreDataTable = calcElimRoundPlcmts( curFinalScoreDataTable, curRules, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEvent );
			} else {
				curFinalScoreDataTable = calcElimRoundFinalPlcmts( curFinalScoreDataTable, curRules, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEvent );
			}

			DataTable curBestScoreDataTable = calcPrelimRoundPlcmts( curPrelimScoreDataTable, curFinalScoreDataTable, curRules, curFinalPlcmtCountPerGroup, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEvent );

			String curScoreAttr = "", curMemberId = "", curDiv = "";
			if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
				if ( inEvent.Equals( "Jump" ) ) {
					if ( HelperFunctions.isIwwfEvent(curRules) ) {
						curScoreAttr = "ScoreMeters";
					} else {
						curScoreAttr = "ScoreFeet DESC, ScoreMeters";
					}
				} else {
					curScoreAttr = "Score" + inEvent;
				}
			} else {
				curScoreAttr = "Points" + inEvent;
			}
			String curSortCmd = "MemberId ASC, AgeGroup ASC, " + curScoreAttr + " DESC, RoundOverall DESC";
			curPrelimScoreDataTable.DefaultView.Sort = curSortCmd;

			DataTable sortPrelimScoreDataTable = curPrelimScoreDataTable.DefaultView.ToTable();
			foreach ( DataRow curRow in curBestScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				curDiv = (String)curRow["AgeGroup"];
				DataRow[] curFindMemberRows = sortPrelimScoreDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curDiv + "'" );
				if ( curFindMemberRows.Length > 0 ) {
					curFindMemberRows[0]["Plcmt" + inEvent] = (String)curRow["Plcmt"];
					curFinalScoreDataTable.ImportRow( curFindMemberRows[0] );
				}
			}

			curSortCmd = "";
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				curSortCmd = "Plcmt" + inEvent + " ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " ASC";
			} else {
				curSortCmd = "Plcmt" + inEvent + " DESC";
			}

			//curFinalScoreDataTable.DefaultView.Sort = curSortCmd;
			//return curFinalScoreDataTable.DefaultView.ToTable();
			return curFinalScoreDataTable;
		}

		/*
         * Determine number of skiers per placement group that participated in elimination rounds
         */
		private DataTable calcFinalPlcmtCountPerGroup( DataRow[] curElimRoundSkierRows, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {
			DataTable curFinalPlcmtCountPerGroup = new DataTable();
			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "Group";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curFinalPlcmtCountPerGroup.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Plcmt";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curFinalPlcmtCountPerGroup.Columns.Add( curCol );

			// The placement position for skiers that do not make the elimination or head to head rounds 
			// is equal to the number of skiers that participated in the first elimination round
			DataRowView newDataRow = null;
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				newDataRow = curFinalPlcmtCountPerGroup.DefaultView.AddNew();
				newDataRow["Group"] = inPlcmtOrg;
				newDataRow["Plcmt"] = curElimRoundSkierRows.Length;
				newDataRow.EndEdit();
			} else {
				String curGroup = "";
				foreach ( DataRow curRow in curElimRoundSkierRows ) {
					if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
						curGroup = (String)curRow["EventGroup" + inEvent];
					} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
						curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup" + inEvent];
					} else {
						curGroup = (String)curRow["AgeGroup"];
					}
					if ( curFinalPlcmtCountPerGroup.Select( "Group = '" + curGroup + "'" ).Length == 0 ) {
						newDataRow = curFinalPlcmtCountPerGroup.DefaultView.AddNew();
						newDataRow["Group"] = curGroup;
						newDataRow["Plcmt"] = 1;
						newDataRow.EndEdit();
					} else {
						DataRow curGroupPlcmtRow = curFinalPlcmtCountPerGroup.Select( "Group = '" + curGroup + "'" )[0];
						int curPlcmtCount = (int)curGroupPlcmtRow["Plcmt"];
						curGroupPlcmtRow["Plcmt"] = curPlcmtCount + 1;
					}
				}
			}

			return curFinalPlcmtCountPerGroup;
		}

		/*
         * Calculate placements for final rounds for head to head format
         */
		private DataTable calcElimRoundPlcmts( DataTable inFinalScoreDataTable, String inRules, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {
			String curSortCmd, curScoreAttr, curScoreName;
			String curPlcmtName = "Plcmt" + inEvent;
			if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
				if ( inEvent.Equals( "Jump" ) ) {
					curScoreName = "ScoreFeet";
					if ( HelperFunctions.isIwwfEvent(inRules) ) {
						curScoreAttr = "ScoreMeters";
					} else {
						curScoreAttr = "ScoreFeet DESC, ScoreMeters";
					}
				} else {
					curScoreName = "Score" + inEvent;
					curScoreAttr = "Score" + inEvent;
				}
			} else {
				curScoreName = "Points" + inEvent;
				curScoreAttr = "Points" + inEvent;
			}

			/*
             * Calculate placements based on head to head match ups
            */
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				curSortCmd = "Round" + inEvent + " DESC, EventGroup" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";

			} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = " AgeGroup ASC, Round" + inEvent + " DESC, EventGroup" + inEvent + " ASC, RunOrderGroup DESC, " + curScoreAttr + " DESC, SkierName ASC";
				} else {
					curSortCmd = " EventGroup" + inEvent + " ASC, Round" + inEvent + " DESC, RunOrderGroup DESC, " + curScoreAttr + " DESC, SkierName ASC";
				}

			} else {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = " AgeGroup ASC, Round" + inEvent + " DESC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";
				} else {
					curSortCmd = " EventGroup" + inEvent + " ASC, Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
				}
			}

			String curRound = "", prevRound = "", curGroup = "", prevGroup = "", curPlcmt = "";
			int curIdx = 0, curPlcmtPos = 1;
			Decimal curScore = 0, prevScore = -1;
			inFinalScoreDataTable.DefaultView.Sort = curSortCmd;
			DataTable curFinalsDataTable = inFinalScoreDataTable.DefaultView.ToTable();
			foreach ( DataRow curRow in curFinalsDataTable.Rows ) {
				curPlcmt = "";
				prevRound = curRound;
				prevGroup = curGroup;
				prevScore = curScore;

				if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
					curGroup = (String)curRow["EventGroup" + inEvent];
				} else {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup" + inEvent] + "-" + (String)curRow["RunOrderGroup"];
				}
				if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
					curGroup = (String)curRow["EventGroup" + inEvent];

				} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
					if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
						curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup" + inEvent] + "-" + (String)curRow["RunOrderGroup"];
					} else {
						curGroup = (String)curRow["EventGroup" + inEvent] + "-" + (String)curRow["RunOrderGroup"];
					}

				} else {
					if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
						curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup" + inEvent];
					} else {
						curGroup = (String)curRow["EventGroup" + inEvent];
					}
				}

				curRound = HelperFunctions.getDataRowColValue( curRow, "Round", "1" );
				if ( curRound != prevRound ) {
					curPlcmtPos = 1;
					prevScore = -1;
					prevGroup = "";
				} else if ( curGroup != prevGroup ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}

				// curScoreName
				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );
				if ( curScore == prevScore && curIdx > 0 ) {
					curPlcmt = HelperFunctions.getDataRowColValue( curFinalsDataTable.Rows[curIdx - 1], curPlcmtName, "" );
					if ( curPlcmt.Contains( "T" ) ) {
					} else {
						curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
						curFinalsDataTable.Rows[curIdx - 1][curPlcmtName] = curPlcmt;
					}
				} else {
					curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
					curPlcmt += " ";
				}
				curRow[curPlcmtName] = curPlcmt;
				curPlcmtPos++;
				curIdx++;
			}

			/*
            Calculate final placements
            */
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				curSortCmd = "Round" + inEvent + " DESC, EventGroup" + inEvent + " ASC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";

			} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = " AgeGroup ASC, Round" + inEvent + " DESC, EventGroup" + inEvent + " ASC, RunOrderGroup DESC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";
				} else {
					curSortCmd = " EventGroup" + inEvent + " ASC, Round" + inEvent + " DESC, RunOrderGroup DESC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";
				}

			} else {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = " AgeGroup ASC, Round" + inEvent + " DESC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";
				} else {
					curSortCmd = " EventGroup" + inEvent + " ASC, Round" + inEvent + " DESC, Plcmt" + inEvent + " ASC, " + curScoreAttr + " DESC, SkierName ASC";
				}
			}
			curFinalsDataTable.DefaultView.Sort = curSortCmd;
			curFinalsDataTable = curFinalsDataTable.DefaultView.ToTable();

			return curFinalsDataTable;
		}

		/*
         * Calculate placements for final rounds for head to head format
         */
		private DataTable calcElimRoundFinalPlcmts( DataTable inFinalScoreDataTable, String inRules, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {
			String curSortCmd, curScoreAttr, curScoreName;
			String curPlcmtName = "Plcmt" + inEvent;
			if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
				if ( inEvent.Equals( "Jump" ) ) {
					curScoreName = "ScoreFeet";
					if ( HelperFunctions.isIwwfEvent( inRules ) ) {
						curScoreAttr = "ScoreMeters";
					} else {
						curScoreAttr = "ScoreFeet DESC, ScoreMeters";
					}
				} else {
					curScoreName = "Score" + inEvent;
					curScoreAttr = "Score" + inEvent;
				}
			} else {
				curScoreName = "Points" + inEvent;
				curScoreAttr = "Points" + inEvent;
			}

			/*
             * Calculate placements based on head to head match ups
            */
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				curSortCmd = "Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSortCmd = "EventGroup ASC, Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
			} else {
				curSortCmd = "Round" + inEvent + " DESC, " + curScoreAttr + " DESC, SkierName ASC";
			}
			String curRound = "", prevRound = "", curGroup = "", prevGroup = "", curPlcmt = "";
			int curIdx = 0, curPlcmtPos = 1;
			Decimal curScore = 0, prevScore = -1;
			inFinalScoreDataTable.DefaultView.Sort = curSortCmd;
			DataTable curFinalsDataTable = inFinalScoreDataTable.DefaultView.ToTable();
			foreach ( DataRow curRow in curFinalsDataTable.Rows ) {
				curPlcmt = "";
				prevRound = curRound;
				prevGroup = curGroup;
				prevScore = curScore;

				if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
					curGroup = "";
				} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup" + inEvent];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup" + inEvent];
				} else {
					curGroup = "";
				}

				curRound = HelperFunctions.getDataRowColValue( curRow, "Round", "1" );
				if ( curRound != prevRound ) {
					curPlcmtPos = 1;
					prevScore = -1;
					prevGroup = "";
				} else if ( curGroup != prevGroup ) {
					curPlcmtPos = 1;
					prevScore = -1;
				}

				decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, curScoreName, "0" ), out curScore );
				if ( curScore == prevScore && curIdx > 0 ) {
					curPlcmt = (String)curFinalsDataTable.Rows[curIdx - 1][curPlcmtName];
					if ( curPlcmt.Contains( "T" ) ) {
					} else {
						curPlcmt = curPlcmt.Substring( 0, 3 ) + "T";
						curFinalsDataTable.Rows[curIdx - 1][curPlcmtName] = curPlcmt;
					}
				} else {
					curPlcmt = curPlcmtPos.ToString( "##0" ).PadLeft( 3, ' ' );
					curPlcmt += " ";
				}
				curRow[curPlcmtName] = curPlcmt;
				curPlcmtPos++;
				curIdx++;
			}

			return curFinalsDataTable;
		}

		private DataTable calcPrelimRoundPlcmts( DataTable inPrelimScoreDataTable, DataTable inFinalScoreDataTable, String inRules, DataTable inFinalPlcmtCountPerGroup, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {

			#region Create table to hold best score for each skier in the prelimary rounds that didn't make the finals
			DataTable curBestScoreDataTable = new DataTable();
			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "MemberId";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curBestScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "AgeGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curBestScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curBestScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Score";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curBestScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreBackup";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curBestScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Plcmt";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curBestScoreDataTable.Columns.Add( curCol );

			#endregion

			#region Build table of best scores for each skier in the prelimary rounds that didn't make the finals
			String curDiv = "", curGroup = "", curMemberId = "";
			Decimal curScore = 0, curScoreBackup;
			DataRow[] curFindRows = null;
			DataRowView newDataRow = null;

			foreach ( DataRow curRow in inPrelimScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				curDiv = (String)curRow["AgeGroup"];
				curGroup = (String)curRow["EventGroup" + inEvent];

				curFindRows = inFinalScoreDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curDiv + "'" );
				if ( curFindRows.Length == 0 ) {
					// Only select skiers that are not in the elimination rounds
					try {
						if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
							if ( inEvent.Equals( "Jump" ) ) {
								if ( HelperFunctions.isIwwfEvent(inRules) ) {
									curScore = (Decimal)( curRow["ScoreMeters"] );
									curScoreBackup = 0;
								} else {
									curScore = (Decimal)( curRow["ScoreFeet"] );
									curScoreBackup = (Decimal)( curRow["ScoreMeters"] );
								}
							} else {
								curScoreBackup = 0;
								if ( curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Decimal" ) ) {
									curScore = (Decimal)( curRow["Score" + inEvent] );
								} else if ( curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int16" ) ) {
									curScore = Convert.ToDecimal( (Int16)( curRow["Score" + inEvent] ) );
								} else if ( curRow["Score" + inEvent].GetType() == System.Type.GetType( "System.Int32" ) ) {
									curScore = Convert.ToDecimal( (int)( curRow["Score" + inEvent] ) );
								} else {
									curScore = 0;
								}
							}
						} else {
							curScore = (Decimal)curRow["Points" + inEvent];
							curScoreBackup = 0;
						}
					} catch {
						curScore = 0;
						curScoreBackup = 0;
					}

					curFindRows = curBestScoreDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curDiv + "'" );
					if ( curFindRows.Length > 0 ) {
						if ( curScore > (Decimal)curFindRows[0]["Score"] ) {
							if ( curScoreBackup > 0 ) {
								curFindRows[0]["ScoreBackup"] = curScoreBackup;
							} else {
								if ( (Decimal)curFindRows[0]["ScoreBackup"] < (Decimal)curFindRows[0]["Score"] ) {
									curFindRows[0]["ScoreBackup"] = (Decimal)curFindRows[0]["Score"];
								}
							}
							curFindRows[0]["Score"] = curScore;
						} else {
							if ( (Decimal)curFindRows[0]["ScoreBackup"] < curScore ) {
								curFindRows[0]["ScoreBackup"] = curScore;
							}
						}
					} else {
						newDataRow = curBestScoreDataTable.DefaultView.AddNew();
						newDataRow["MemberId"] = curMemberId;
						newDataRow["AgeGroup"] = curDiv;
						newDataRow["EventGroup"] = curGroup;
						newDataRow["Score"] = curScore;
						newDataRow["ScoreBackup"] = curScoreBackup;
						newDataRow.EndEdit();
					}
				}
			}
			#endregion

			#region Determine skier placement
			String curSortCmd = "", prevGroup = "", curPlcmtTour = "", prevMemberId = "";
			curGroup = "";
			Decimal prevScore = 0, prevScoreBackup = 0;
			int curPlcmt = 1;
			DataRow[] curFindMemberRows;
			if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
				curSortCmd = "Score DESC, ScoreBackup DESC";
			} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSortCmd = "AgeGroup ASC, Score DESC, ScoreBackup DESC";
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSortCmd = "EventGroup ASC, Score DESC, ScoreBackup DESC";
			} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
				curSortCmd = "AgeGroup ASC, EventGroup ASC, Score DESC, ScoreBackup DESC";
			} else {
				curSortCmd = "Score DESC, ScoreBackup DESC";
			}
			curBestScoreDataTable.DefaultView.Sort = curSortCmd;
			DataTable returnBestScoreDataTable = curBestScoreDataTable.DefaultView.ToTable();

			curFindRows = null;
			foreach ( DataRow curRow in returnBestScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				curDiv = (String)curRow["AgeGroup"];
				curScore = (Decimal)curRow["Score"];
				curScoreBackup = (Decimal)curRow["ScoreBackup"];

				if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
					curGroup = inPlcmtOrg;
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else {
					curGroup = (String)curRow["AgeGroup"];
				}
				if ( prevGroup != curGroup ) {
					curFindRows = inFinalPlcmtCountPerGroup.Select( "Group = '" + curGroup + "'" );
					if ( curFindRows.Length > 0 ) {
						curPlcmt = (int)curFindRows[0]["Plcmt"] + 1;
					} else {
						curPlcmt = 1;
					}
					prevScore = -1;
					prevScoreBackup = -1;
				}

				if ( curScore == prevScore ) {
					if ( curScoreBackup == prevScoreBackup ) {
						if ( curPlcmtTour.Contains( "T" ) ) {
						} else {
							curPlcmtTour = curPlcmtTour.Substring( 0, 3 ) + "T";
							curRow["Plcmt"] = curPlcmtTour;
							curFindMemberRows = curBestScoreDataTable.Select( "MemberId = '" + prevMemberId + "' AND AgeGroup = '" + curDiv + "'" );
							for ( int curIdx = 0; curIdx < curFindMemberRows.Length; curIdx++ ) {
								curFindMemberRows[curIdx]["Plcmt"] = curPlcmtTour;
							}
						}
					} else {
						curPlcmtTour = ( curPlcmt.ToString( "##0" ) ).PadLeft( 3, ' ' ) + " ";
						curRow["Plcmt"] = curPlcmtTour;
					}
				} else {
					curPlcmtTour = ( curPlcmt.ToString( "##0" ) ).PadLeft( 3, ' ' ) + " ";
					curRow["Plcmt"] = curPlcmtTour;
				}

				curPlcmt++;
				prevMemberId = curMemberId;
				prevGroup = curGroup;
				prevScore = curScore;
				prevScoreBackup = curScoreBackup;
			}
			#endregion

			return returnBestScoreDataTable;
		}

		private DataTable CalcScoreHcap( DataTable inDataTable, String inSanctionId, String inEvent ) {
			String curFindCommand = "", curMemberId = "";
			Decimal curScore = 0, curHcapScore = 0, curHcapBase = 0, curScoreFeet = 0;
			DataRow curEventRow;
			DataRow[] curFindRows;
			DataTable curScoreDataTable = inDataTable;

			//Determine number of skiers per placement group
			DataTable curEventDataTable = getEventRegData( inSanctionId, inEvent );

			//Calculate points based on placement
			foreach ( DataRow curRow in curScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				try {
					if ( inEvent.ToLower().Equals( "jump" ) ) {
						curScore = (Decimal)( curRow["ScoreFeet"] );
					} else {
						if ( curRow["Score"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
							curScore = (Decimal)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int16" ) ) {
							curScore = (Int16)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int32" ) ) {
							curScore = (int)( curRow["Score"] );
						} else {
							curScore = 0;
						}
					}
				} catch {
					curScore = 0;
				}
				curFindCommand = "MemberId = '" + curMemberId + "'";
				curFindRows = curEventDataTable.Select( curFindCommand );
				if ( curFindRows.Length > 0 ) {
					curEventRow = curFindRows[0];
					try {
						curHcapScore = (Decimal)curEventRow["HcapScore"];
					} catch {
						curHcapScore = 0;
					}
					try {
						curHcapBase = (Decimal)curEventRow["HCapBase"];
					} catch {
						curHcapBase = 0;
					}
				} else {
					curHcapScore = 0;
				}
				if ( curHcapBase > 0 ) {
					curScore += curHcapScore;
				}
				curRow["HCapScore"] = curHcapScore;
				curRow["HCapBase"] = curHcapBase;
				curRow["NopsScore"] = curScore;
			}

			return curScoreDataTable;
		}

		private DataTable CalcScoreRatio( DataTable inDataTable, String inSanctionId, String inEvent ) {
			string curFindCommand = "", curMemberId = "";
			Decimal curScore = 0, curHcapScore = 0, curHcapBase = 0;
			DataRow curEventRow;
			DataRow[] curFindRows;
			DataTable curScoreDataTable = inDataTable;

			//Determine number of skiers per placement group
			DataTable curEventDataTable = getEventRegData( inSanctionId, inEvent );

			//Calculate points based on placement
			foreach ( DataRow curRow in curScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				try {
					if ( inEvent.ToLower().Equals( "jump" ) ) {
						curScore = (Decimal)( curRow["ScoreFeet"] );
					} else {
						if ( curRow["Score"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
							curScore = (Decimal)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int16" ) ) {
							curScore = (Int16)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int32" ) ) {
							curScore = (int)( curRow["Score"] );
						} else {
							curScore = 0;
						}
					}
				} catch {
					curScore = 0;
				}
				curFindCommand = "MemberId = '" + curMemberId + "'";
				curFindRows = curEventDataTable.Select( curFindCommand );
				if ( curFindRows.Length > 0 ) {
					curEventRow = curFindRows[0];
					try {
						curHcapScore = (Decimal)curEventRow["HcapScore"];
					} catch {
						curHcapScore = 0;
					}
					try {
						curHcapBase = (Decimal)curEventRow["HCapBase"];
					} catch {
						curHcapBase = 0;
					}
				} else {
					curHcapScore = 0;
				}

				if ( curScore > 0 && curHcapBase > 0 ) {
					curScore = Math.Round( ( ( curScore / curHcapBase ) * 100 ), 1 );
				}

				curRow["NopsScore"] = curScore;
				curRow["HCapScore"] = curHcapScore;
				curRow["HCapBase"] = curHcapBase;
			}

			return curScoreDataTable;
		}

		private DataTable CalcPointsPlcmt( DataTable inDataTable, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {
			String curGroup = "", prevGroup = "", curSortCmd = "", curPlcmtValue = "", curSelectCmd, curSelectTieCmd;
			Decimal curScore = 0;
			int curPlcmt = 0, curPlcmtMax = 0, curIdx = 0, curTieAdj = 0;
			DataRow[] curFindList;

			if ( inEvent.ToLower().Equals( "jump" ) ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else {
					curSortCmd = "Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
					curPlcmtMax = inDataTable.Rows.Count;
				}
			} else {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else {
					curSortCmd = "Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
					curPlcmtMax = inDataTable.Rows.Count;
				}
			}

			DataView curDataView = inDataTable.DefaultView;
			curDataView.Sort = curSortCmd;
			DataTable curScoreDataTable = curDataView.ToTable();

			//Calculate points based on placement
			foreach ( DataRow curRow in curScoreDataTable.Rows ) {
				if ( inEvent.ToLower().Equals( "jump" ) ) {
					curScore = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreFeet", 0.0m );
				} else {
					curScore = HelperFunctions.getDataRowColValueDecimal( curRow, "Score" + inEvent, 0.0m );
				}

				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curPlcmtValue = (String)curRow["Plcmt" + inEvent];
					curGroup = (String)curRow["AgeGroup"];
					curSelectCmd = "AgeGroup = '" + curGroup + "'";
					curSelectTieCmd = curSelectCmd + " AND Plcmt"  + inEvent + " = '" + curPlcmtValue + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curPlcmtValue = (String)curRow["Plcmt" + inEvent];
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
					curSelectCmd = "AgeGroup = '" + (String)curRow["AgeGroup"] + "' "
						+ "AND " + "EventGroup = '" + (String)curRow["EventGroup"] + "'";
					curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curPlcmtValue = (String)curRow["Plcmt" + inEvent];
					curGroup = (String)curRow["EventGroup"];
					curSelectCmd = "EventGroup = '" + curGroup + "'";
					curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
				} else {
					curPlcmtValue = (String)curRow["Plcmt" + inEvent];
					curGroup = "";
					curSelectCmd = "";
					curSelectTieCmd = curSelectCmd + " Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
				}
				curTieAdj = 0;
				if ( curPlcmtValue.Contains( "T" ) ) {
					curPlcmt = Convert.ToInt32( curPlcmtValue.Substring( 0, curPlcmtValue.IndexOf( "T" ) ) );
					curFindList = curScoreDataTable.Select( curSelectTieCmd );
					if ( curFindList.Length > 0 ) {
						curTieAdj = ( curFindList.Length - 1 ) * 5;
					}
				} else if ( curPlcmtValue.Length > 0 ) {
					curPlcmt = Convert.ToInt32( curPlcmtValue );
				} else {
					curPlcmt = 0;
				}

				if ( !( curGroup.Equals( prevGroup ) ) ) {
					curFindList = curScoreDataTable.Select( curSelectCmd );
					curPlcmtMax = curFindList.Length;
				}

				if ( curPlcmt == 999 ) {
					curScore = 0;

				} else 	if ( curPlcmtMax > 0 && curScore > 0 ) {
					curScore = ( ( ( curPlcmtMax - curPlcmt ) + 1 ) * 10 ) - curTieAdj;
				
				} else if ( curScore == 0 && curPlcmtMax > 0 ) {
					if ( inEvent.ToLower().Equals( "jump" ) ) {
						if ( isSkierEligNcwsaJumpScore( (String)curRow["SanctionId"], (String)curRow["MemberId"], (String)curRow["AgeGroup"], (Int16)curRow["Round"] ) ) {
							curScore = ( ( ( curPlcmtMax - curPlcmt ) + 1 ) * 10 ) - curTieAdj;
						} else {
							curScore = 0;
						}
					} else {
						curScore = ( ( ( curPlcmtMax - curPlcmt ) + 1 ) * 10 ) - curTieAdj;
					}
				}

				curRow["Points" + inEvent] = curScore;
				prevGroup = curGroup;
				curIdx++;
			}

			return curScoreDataTable;
		}

		private DataTable CalcPointsRoundPlcmt( DataRow inTourRow, DataTable inDataTable, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEvent ) {
			String curGroup = "", prevGroup = "", curSortCmd = "", curPlcmtValue = "", curSelectCmd, curSelectTieCmd;
			Decimal curScore = 0;
			int curPlcmt = 0, curPlcmtMax = 0, curTieAdj = 0;
			DataRow[] curFindList;

			Int16 numTourEventRounds = (byte)inTourRow[inEvent + "Rounds"];

			//Sort data
			if ( inEvent.ToLower().Equals( "jump" ) ) {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
				} else {
					curSortCmd = "Plcmt" + inEvent + " DESC, ScoreFeet Desc, ScoreMeters Desc ";
					curPlcmtMax = inDataTable.Rows.Count;
				}
			} else {
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
				} else {
					curSortCmd = "Plcmt" + inEvent + " DESC, Score" + inEvent + " Desc ";
					curPlcmtMax = inDataTable.Rows.Count;
				}
			}
			inDataTable.DefaultView.Sort = curSortCmd;
			DataTable curScoreDataTable = inDataTable.DefaultView.ToTable();

			DataRow[] curRoundRows = null;
			String curFilterCommand = "";
			for ( int curRound = 1; curRound <= numTourEventRounds; curRound++ ) {
				curFilterCommand = "Round" + inEvent + " = " + curRound;
				curRoundRows = curScoreDataTable.Select( curFilterCommand );

				if ( curRoundRows.Length > 0 ) {
					prevGroup = "";
					if ( inPlcmtOrg.ToLower().Equals( "tour" ) || inPlcmtOrg.ToLower().Equals( "awsa" ) ) {
						curPlcmtMax = curRoundRows.Length + 1;
					}

					foreach ( DataRow curRow in curRoundRows ) {
						try {
							if ( inEvent.Equals( "Jump" ) ) {
								curScore = (Decimal)( curRow["ScoreFeet"] );
							} else {
								curScore = (Decimal)( curRow["Score" + inEvent] );
							}
						} catch {
							curScore = 0;
						}

						if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
							curPlcmtValue = (String)curRow["Plcmt" + inEvent];
							curGroup = (String)curRow["AgeGroup"];
							curSelectCmd = "Round = " + curRound.ToString() + " AND AgeGroup = '" + curGroup + "'";
							curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
						} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
							curPlcmtValue = (String)curRow["Plcmt" + inEvent];
							curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
							curSelectCmd = "Round = " + curRound.ToString()
								+ " AND AgeGroup = '" + (String)curRow["AgeGroup"] + "'"
								+ " AND " + "EventGroup = '" + (String)curRow["EventGroup"] + "'";
							curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
						} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
							curPlcmtValue = (String)curRow["Plcmt" + inEvent];
							curGroup = (String)curRow["EventGroup"];
							curSelectCmd = "Round = " + curRound.ToString() + " AND EventGroup = '" + curGroup + "'";
							curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
						} else {
							curPlcmtValue = (String)curRow["Plcmt" + inEvent];
							curGroup = "";
							curSelectCmd = "Round = " + curRound.ToString();
							curSelectTieCmd = curSelectCmd + " AND Plcmt" + inEvent + " = '" + curPlcmtValue + "'";
						}
						curTieAdj = 0;
						if ( curPlcmtValue.Contains( "T" ) ) {
							curPlcmt = Convert.ToInt32( curPlcmtValue.Substring( 0, curPlcmtValue.IndexOf( "T" ) ) );
							curFindList = curScoreDataTable.Select( curSelectTieCmd );
							if ( curFindList.Length > 0 ) {
								curTieAdj = ( curFindList.Length - 1 ) * 5;
							}
						} else {
							curPlcmt = Convert.ToInt32( curPlcmtValue );
						}

						if ( !( curGroup.Equals( prevGroup ) ) ) {
							curFindList = curScoreDataTable.Select( curSelectCmd );
							curPlcmtMax = curFindList.Length;
						}

						if ( curScore > 0 && curPlcmtMax > 0 ) {
							curScore = ( ( ( curPlcmtMax - curPlcmt ) + 1 ) * 10 ) - curTieAdj;
						}
						curRow["Points" + inEvent] = curScore;
						prevGroup = curGroup;
					}
				}
			}

			return curScoreDataTable;
		}

		public DataTable CalcIwwfEventPlcmts( DataRow inTourRow, String inSanctionId, String inEvent, String inRules, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv ) {
			return CalcIwwfEventPlcmts( inTourRow, inSanctionId, inEvent, inRules, inDataType, inPlcmtMethod, inPlcmtOrg, inPointsMethod, inEventGroup, inDiv, 0 );
        }

        public DataTable CalcIwwfEventPlcmts( DataRow inTourRow, String inSanctionId, String inEvent, String inRules, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inEventGroup, String inDiv, Int16 inNumPrelimRounds ) {
			DataTable curSlalomSummary = null;
			DataTable curTrickSummary = null;
			DataTable curJumpSummary = null;
			DataTable curSummaryDataTable = null;

			if ( inEvent.Equals( "Slalom" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
				curSlalomSummary = getSlalomSummaryData( inSanctionId, inDataType, inRules, inEventGroup, inDiv );
				curSlalomSummary = expandScoreDataTable( curSlalomSummary, "Slalom" );
			}
			if ( inEvent.Equals( "Trick" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
				curTrickSummary = getTrickSummaryData( inSanctionId, inDataType, inRules, inEventGroup, inDiv );
				curTrickSummary = expandScoreDataTable( curTrickSummary, "Trick" );
			}
			if ( inEvent.Equals( "Jump" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
				curJumpSummary = getJumpSummaryData( inSanctionId, inDataType, inRules, inEventGroup, inDiv );
				curJumpSummary = expandScoreDataTable( curJumpSummary, "Jump" );
			}
			if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
				curSummaryDataTable = buildOverallSummary( inTourRow, curSlalomSummary, curTrickSummary, curJumpSummary, inDataType, inPlcmtMethod );
			} else {
				if ( inEvent.ToLower().Equals( "scorebook" ) ) {
					curSummaryDataTable = buildOverallSummary( inTourRow, curSlalomSummary, curTrickSummary, curJumpSummary, "round", inPlcmtMethod );
				} else {
					curSummaryDataTable = buildOverallSummary( inTourRow, curSlalomSummary, curTrickSummary, curJumpSummary, inDataType, inPlcmtMethod );
				}
			}

			calcIwwfOverallPoints( curSummaryDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, inEventGroup );

			#region Determine placements by event
			if ( inEvent.Equals( "Slalom" ) ) {
				curSummaryDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curSummaryDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else if ( inEvent.Equals( "Trick" ) ) {
				curSummaryDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curSummaryDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else if ( inEvent.Equals( "Jump" ) ) {
				curSummaryDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curSummaryDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
			} else {
				DataTable curSlalomPlcmtDataTable = buildOverallSummary( inTourRow, curSlalomSummary, null, null, inDataType, inPlcmtMethod );
				curSlalomPlcmtDataTable = myCalcEventPlcmt.setSlalomPlcmt( inTourRow, curSlalomPlcmtDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
				updateIwwfEventPlcmts( "Slalom", curSlalomPlcmtDataTable, curSummaryDataTable );

				DataTable curTrickPlcmtDataTable = buildOverallSummary( inTourRow, null, curTrickSummary, null, inDataType, inPlcmtMethod );
				curTrickPlcmtDataTable = myCalcEventPlcmt.setTrickPlcmt( inTourRow, curTrickPlcmtDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
				updateIwwfEventPlcmts( "Trick", curTrickPlcmtDataTable, curSummaryDataTable );

				DataTable curJumpPlcmtDataTable = buildOverallSummary( inTourRow, null, null, curJumpSummary, inDataType, inPlcmtMethod );
				curJumpPlcmtDataTable = myCalcEventPlcmt.setJumpPlcmt( inTourRow, curJumpPlcmtDataTable, inPlcmtMethod, inPlcmtOrg, inDataType, inNumPrelimRounds );
				updateIwwfEventPlcmts( "Jump", curJumpPlcmtDataTable, curSummaryDataTable );

				curSummaryDataTable = myCalcEventPlcmt.setOverallPlcmtIwwf( inTourRow, curSummaryDataTable, inPlcmtMethod, inPlcmtOrg, inDataType );
			}
			#endregion

			#region Perform final sort
			String curSortCmd = "", curRoundLabel = "", curPlcmtLabel = "";
			if ( inEvent.Equals( "Scorebook" ) ) {
				curRoundLabel = "RoundOverall";
				curPlcmtLabel = "";
				curSortCmd = "AgeGroup ASC, SkierName ASC, MemberId ASC, " + curRoundLabel + " ASC";
			} else {
				if ( inEvent.ToLower().Equals( "overall" ) ) {
					curRoundLabel = "RoundOverall";
					curPlcmtLabel = "EligOverall DESC, QualifyOverall DESC, ScoreOverall DESC";
				} else if ( inEvent.ToLower().Equals( "slalom" ) ) {
					curRoundLabel = "RoundSlalom";
					if ( inDataType.ToLower().Equals( "final" ) ) {
						curPlcmtLabel = "PlcmtSlalom ASC";
					} else if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
						curPlcmtLabel = "PlcmtSlalom ASC, ScoreSlalom  DESC";
					} else {
						curPlcmtLabel = "PlcmtSlalom ASC, PointsSlalom  DESC";
					}
				} else if ( inEvent.ToLower().Equals( "trick" ) ) {
					curRoundLabel = "RoundTrick";
					if ( inDataType.ToLower().Equals( "final" ) ) {
						curPlcmtLabel = "PlcmtTrick ASC";
					} else if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
						curPlcmtLabel = "PlcmtTrick ASC, ScoreTrick  DESC";
					} else {
						curPlcmtLabel = "PlcmtTrick  ASC, PointsTrick  DESC";
					}
				} else if ( inEvent.ToLower().Equals( "jump" ) ) {
					curRoundLabel = "RoundJump";
					if ( inDataType.ToLower().Equals( "final" ) ) {
						curPlcmtLabel = "PlcmtJump ASC";
					} else if ( inPlcmtMethod.ToLower().Equals( "score" ) ) {
						curPlcmtLabel = "PlcmtJump ASC, ScoreMeters DESC";
					} else {
						curPlcmtLabel = "PlcmtJump ASC, PointsJump DESC";
					}
				}

				if ( inPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
					curSortCmd = "AgeGroup ASC, SkierName ASC, MemberId ASC, " + curRoundLabel + " ASC";
				} else if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "AgeGroup ASC, " + curRoundLabel + " ASC, RunOrderGroup ASC, " + curPlcmtLabel + ", SkierName ASC";
					} else {
						curSortCmd = "AgeGroup ASC, " + curPlcmtLabel + ", SkierName ASC, " + curRoundLabel + " ASC";
					}
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "AgeGroup ASC, EventGroup ASC, " + curRoundLabel + " ASC, RunOrderGroup ASC, " + curPlcmtLabel + ", SkierName ASC";
					} else {
						curSortCmd = "AgeGroup ASC, EventGroup ASC, " + curPlcmtLabel + ", SkierName ASC, " + curRoundLabel + " ASC";
					}
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = "EventGroup ASC, " + curPlcmtLabel + ", SkierName ASC ";
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = "EventGroup ASC, " + curRoundLabel + " ASC, RunOrderGroup ASC, " + curPlcmtLabel + ", SkierName ASC";
					} else {
						curSortCmd = "EventGroup ASC, " + curPlcmtLabel + ", SkierName ASC, " + curRoundLabel + " ASC";
					}
				} else {
					if ( inDataType.ToLower().Equals( "round" ) || inDataType.ToLower().Equals( "h2h" ) ) {
						curSortCmd = curRoundLabel + " ASC, RunOrderGroup ASC, " + curPlcmtLabel + ", SkierName ASC";
					} else {
						curSortCmd = curPlcmtLabel + ", SkierName ASC, MemberId ASC, " + curRoundLabel + " ASC";
					}
				}
			}
			#endregion

			curSummaryDataTable.DefaultView.Sort = curSortCmd;
			curSummaryDataTable = curSummaryDataTable.DefaultView.ToTable();

			return curSummaryDataTable;
		}

		/*
		 * IWWF Overall calculations.
		 * There are several variations of this depending on the tournament or the purpose
		 * Rule 24: Ranking List Overall Score Basis
		 *		11.01: Rules for U-10 and U-14 Individual Overall
		 * Overall for World Open Championships Rule 15.04
		 * Overall for World Under 17 Championships Rules
		 * Overall for World 35+ Championships Rules Rule 18.04 
		 * Overall for World Under 17 Championships Rules Rule 16.04 
		 * Overall for World Under 21 Championships Rules Rule 17.04 
		 * 
		 * This calculation essentially applies the various world tournament rules to any particular tournament
		 */
		public void calcIwwfOverallPoints( DataTable inSummaryDataTable, String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, String inEventGroup ) {
			//Determine maximum scores per event and division
			Boolean curTeamPlcmt = false;
			if ( inEventGroup != null ) {
				if ( inEventGroup.ToLower().Equals( "team" ) ) {
					curTeamPlcmt = true;
				}
			}
			/*
			 * Retrieve scores to be used as the overall score basis 
			 * The scorer can use either the World Ranking Basis scores
			 * or the best overall score achieved at the event
			 */
			DataTable curMaxDataTable = buildMaxScoresPerGroup( inSanctionId, inPlcmtOrg, inRules, inEvent, curTeamPlcmt );
			
			//Retrieve overall adjustment factors
			DataTable curOverallAdjDataTable = getIwwfOverallAdj();

			#region Calculate points based on IWWF 1000 point formula including adjustments
			String curGroup = "", curFilterCommand = "", curAgeGroup = "";
			Decimal curScore, curScoreMax, curAdjFactor;
			DataRow[] curFindRows;
			DataRow curMaxRow;
			String curScoreName = "", curPointsName = "";

			foreach ( DataRow curRow in inSummaryDataTable.Rows ) {
				curAgeGroup = (String)( curRow["AgeGroup"] );
				curGroup = (String)curRow["EventGroup"];
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = curAgeGroup;
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = curAgeGroup + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = "";
				}

				curFilterCommand = "Group = '" + curGroup + "'";
				curAdjFactor = 0m;

				curFindRows = curMaxDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					curMaxRow = curFindRows[0];

					//For Slalom ((skiers best event score - AdjFactor) x 1000) / (Best Overall Skiers score - AdjFactor) )
					if ( inEvent.Equals( "Slalom" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
						if ( inSummaryDataTable.Columns.Contains( "ScoreSlalom" ) ) {
							curScoreName = "ScoreSlalom";
							curPointsName = "PointsSlalom";
						} else {
							curScoreName = "Score";
							curPointsName = "NopsScore";
						}
						if ( (Decimal)curRow[curScoreName] > 0M ) {
							curFilterCommand = "ListCode = '" + curAgeGroup + "-S'";
							curFindRows = curOverallAdjDataTable.Select( curFilterCommand );
							if ( curFindRows.Length > 0 ) {
								curAdjFactor = (Decimal)curFindRows[0]["MinValue"];
							} else {
								curAdjFactor = 0M;
							}
							curScore = (Decimal)curRow[curScoreName];
							if ( ( curScore + curAdjFactor ) > 0M ) {
								if ( (Decimal)curMaxRow["ScoreSlalom"] > 0M ) {
									curScoreMax = (Decimal)curMaxRow["ScoreSlalom"];
								} else {
									curScoreMax = (Decimal)curMaxRow["ScoreSlalomMax"];
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( ( curScore + curAdjFactor ) / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}
							} else {
								//If score is less than adjustment factor than than use raw score to calculate points
								if ( (Decimal)curMaxRow["ScoreSlalom"] > 0M ) {
									curScoreMax = (Decimal)curMaxRow["ScoreSlalom"];
								} else {
									curScoreMax = (Decimal)curMaxRow["ScoreSlalomMax"];
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( ( curScore ) / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}
							}
						} else {
							curRow[curPointsName] = 0M;
						}
					}

					//For Tricks (skiers best event score x 1000) / Best Overall Skiers score
					if ( inEvent.Equals( "Trick" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
						if ( inSummaryDataTable.Columns.Contains( "ScoreTrick" ) ) {
							curScoreName = "ScoreTrick";
							curPointsName = "PointsTrick";
						} else {
							curScoreName = "Score";
							curPointsName = "NopsScore";
						}
						if ( (Int16)curRow[curScoreName] > 0 ) {
							curFilterCommand = "ListCode = '" + curAgeGroup + "-T'";
							curFindRows = curOverallAdjDataTable.Select( curFilterCommand );
							if ( curFindRows.Length > 0 ) {
								curAdjFactor = (Decimal)curFindRows[0]["MinValue"];
							} else {
								curAdjFactor = 0M;
							}
							curScore = Convert.ToDecimal( (Int16)curRow[curScoreName] );
							if ( ( curScore + curAdjFactor ) > 0M ) {
								if ( (Int16)curMaxRow["ScoreTrick"] > 0M ) {
									curScoreMax = Convert.ToDecimal( (Int16)curMaxRow["ScoreTrick"] );
								} else {
									curScoreMax = Convert.ToDecimal( (Int16)curMaxRow["ScoreTrickMax"] );
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( ( curScore + curAdjFactor ) / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}

							} else {
								if ( (Int16)curMaxRow["ScoreTrick"] > 0M ) {
									curScoreMax = Convert.ToDecimal( (Int16)curMaxRow["ScoreTrick"] );
								} else {
									curScoreMax = Convert.ToDecimal( (Int16)curMaxRow["ScoreTrickMax"] );
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( curScore / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}
							}
						} else {
							curRow[curPointsName] = 0M;
						}
					}

					/*
                    For jumping the formula is different for men and women:
                    Men: ((skiers best event score minus 25m) x 1000) / (Best Overall Skiers score minus 25m)
                    Women : ((skiers best event score minus 17m) x 1000) / (Best Overall Skiers score minus 17m)
                    A skiers overall score in jumping shall not be reduced below zero.
                     */
					if ( inEvent.Equals( "Jump" ) || inEvent.Equals( "Overall" ) || inEvent.Equals( "Scorebook" ) ) {
						curScoreName = "ScoreMeters";
						if ( inSummaryDataTable.Columns.Contains( "PointsJump" ) ) {
							curPointsName = "PointsJump";
						} else {
							curPointsName = "NopsScore";
						}
						if ( (Decimal)curRow["ScoreMeters"] > 0M ) {
							curFilterCommand = "ListCode = '" + curAgeGroup + "-J'";
							curFindRows = curOverallAdjDataTable.Select( curFilterCommand );
							if ( curFindRows.Length > 0 ) {
								curAdjFactor = (Decimal)curFindRows[0]["MinValue"];
							} else {
								curAdjFactor = 0M;
							}
							curScore = (Decimal)curRow[curScoreName];
							if ( ( curScore + curAdjFactor ) > 0M ) {
								if ( (Decimal)curMaxRow["ScoreJump"] > 0M ) {
									curScoreMax = (Decimal)curMaxRow["ScoreJump"];
								} else {
									curScoreMax = (Decimal)curMaxRow["ScoreJumpMax"];
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( ( curScore + curAdjFactor ) / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}
							} else {
								if ( (Decimal)curMaxRow["ScoreJump"] > 0M ) {
									curScoreMax = (Decimal)curMaxRow["ScoreJump"];
								} else {
									curScoreMax = (Decimal)curMaxRow["ScoreJumpMax"];
								}
								if ( curScore > 0 && curScoreMax > 0 ) {
									curRow[curPointsName] = Math.Round( ( curScore / ( curScoreMax + curAdjFactor ) ) * 1000, 1 );
								} else {
									curRow[curPointsName] = 0;
								}
							}
						} else {
							curRow[curPointsName] = 0M;
						}
					}

					if ( inSummaryDataTable.Columns.Contains( "PointsSlalom" ) ) {
						curRow["ScoreOverall"] = (Decimal)curRow["PointsSlalom"] + (Decimal)curRow["PointsTrick"] + (Decimal)curRow["PointsJump"];
					}
				} else {
					MessageBox.Show( "buildMaxPerPlcmtIwwf" );
				}
			}
			#endregion
		}

		public void calcAwsaOverallKPoints( DataTable inSummaryDataTable, String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, String inEventGroup ) {
			String curGroup = "", curFindCommand = "", curSortCmd = "";
			Decimal curScore = 0, curScoreMax = 0;
			DataRow curMaxRow;
			DataRow[] curFindRows;
			
			bool isJumpEvent = inSummaryDataTable.Columns.Contains( "ScoreMeters" );
			bool isTrickEvent = inSummaryDataTable.Columns.Contains( "ScorePass1" );
			bool isSlalomEvent = inSummaryDataTable.Columns.Contains( "FinalPassScore" );
			String curScoreName = "Score";
			if ( isJumpEvent ) {
				curScoreName = "ScoreFeet";
			}

			//Determine maximum scores per event and division
			DataTable curMaxDataTable = buildMaxScoresPerGroup( inSanctionId, inPlcmtOrg, inRules, inEvent );

			//Calculate points based on placement
			foreach ( DataRow curRow in inSummaryDataTable.Rows ) {
				try {
					if ( curRow[curScoreName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
						curScore = (Decimal)( curRow[curScoreName] );
					} else if ( curRow[curScoreName].GetType() == System.Type.GetType( "System.Int16" ) ) {
						curScore = (Int16)( curRow[curScoreName] );
					} else if ( curRow[curScoreName].GetType() == System.Type.GetType( "System.Int32" ) ) {
						curScore = (int)( curRow[curScoreName] );
					} else {
						curScore = 0;
					}
				} catch {
					curScore = 0;
				}
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
					curFindCommand = "Group = '" + curGroup + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
					curFindCommand = "Group = '" + curGroup + "'";
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
					curFindCommand = "Group = '" + curGroup + "'";
				} else {
					curFindCommand = "Group = ''";
				}
				curFindRows = curMaxDataTable.Select( curFindCommand );
				if ( curFindRows.Length > 0 ) {
					curMaxRow = curFindRows[0];
					curScoreMax = 0;
					if ( isSlalomEvent ) {
						curScoreMax = (Decimal)curMaxRow["ScoreSlalomMax"];
					} else if ( isTrickEvent ) {
						curScoreMax = (Int16)curMaxRow["ScoreTrickMax"];
					} else if ( isJumpEvent ) {
						curScoreMax = (Decimal)curMaxRow["ScoreJumpMax"];
					}
				} else {
					curScoreMax = 0;
				}
				if ( curScore > 0 && curScoreMax > 0 ) {
					if ( isJumpEvent ) {
						/* 
						 * I had the original calculation using the square root of the results 
						 * but further of the rule in 2025 doesn't use the square root
						 * Not sure if this changed or I misinterpreted it originally.
						 * In any event as of 2025 Rules this is correct for jump
						 */
						//curScore = Convert.ToDecimal( Math.Sqrt( Convert.ToDouble( ( curScore * curScore ) / ( curScoreMax * curScoreMax ) ) ) * 1000 );
                        curScore = Convert.ToDecimal( Convert.ToDouble( ( curScore * curScore ) / ( curScoreMax * curScoreMax ) ) * 1000 );
                    } else {
						curScore = Math.Round( ( ( curScore / curScoreMax ) * 1000 ), 1 );
					}
				} else {
					curScore = 0;
				}
				curRow["NopsScore"] = curScore;
			}

		}

		private void updateIwwfEventPlcmts( String inEvent, DataTable inEventPlcmtDataTable, DataTable inSummaryDataTable ) {
			String curMemberId = "", curDiv = "", curRound = "";
			DataRow[] curFindPlcmt = null;

			foreach ( DataRow curRow in inSummaryDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				curDiv = (String)curRow["AgeGroup"];
				try {
					if ( curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Byte" ) ) {
						curRound = ( (Byte)curRow["Round" + inEvent] ).ToString();
					} else if ( curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int16" ) ) {
						curRound = ( (Int16)curRow["Round" + inEvent] ).ToString();
					} else if ( curRow["Round" + inEvent].GetType() == System.Type.GetType( "System.Int32" ) ) {
						curRound = ( (int)curRow["Round" + inEvent] ).ToString();
					} else {
						curRound = "1";
					}
				} catch {
					curRound = "1";
				}

				curFindPlcmt = inEventPlcmtDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curDiv + "'" );
				if ( curFindPlcmt.Length == 0 ) {
				} else if ( curFindPlcmt.Length == 1 ) {
					curRow["Plcmt" + inEvent] = (String)curFindPlcmt[0]["Plcmt" + inEvent];
				} else {
					curFindPlcmt = inEventPlcmtDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curDiv + "' AND Round"+ inEvent + " = " + curRound );
					if ( curFindPlcmt.Length > 0 ) {
						curRow["Plcmt" + inEvent] = (String)curFindPlcmt[0]["Plcmt" + inEvent];
					}
				}

			}
		}

		private DataTable CalcPointsKBase( String inSanctionId, DataTable inDataTable, String inDataType, String inPlcmtMethod, String inPlcmtOrg, String inPointsMethod, String inRules, String inEvent ) {
			DataTable curScoreDataTable = inDataTable;

			if ( HelperFunctions.isIwwfEvent(inRules) ) {
				calcIwwfOverallPoints( curScoreDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, "" );
			
			} else {
				calcAwsaOverallKPoints( curScoreDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, "" );
			}

			return curScoreDataTable;
		}

		private DataTable CalcPointsHcap( DataTable inDataTable, String inSanctionId, String inEvent ) {
			String curFindCommand = "", curMemberId = "";
			Decimal curScore = 0, curHcapScore = 0, curHcapBase = 0;
			DataRow curEventRow;
			DataRow[] curFindRows;
			DataTable curScoreDataTable = inDataTable;

			//Determine number of skiers per placement group
			DataTable curEventDataTable = getEventRegData( inSanctionId, inEvent );

			//Calculate points based on placement
			foreach ( DataRow curRow in curScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				try {
					if ( inEvent.ToLower().Equals( "jump" ) ) {
						curScore = (Decimal)( curRow["ScoreFeet"] );
					} else {
						if ( curRow["Score"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
							curScore = (Decimal)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int16" ) ) {
							curScore = (Int16)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int32" ) ) {
							curScore = (int)( curRow["Score"] );
						} else {
							curScore = 0;
						}
					}
				} catch {
					curScore = 0;
				}
				curFindCommand = "MemberId = '" + curMemberId + "'";
				curFindRows = curEventDataTable.Select( curFindCommand );
				if ( curFindRows.Length > 0 ) {
					curEventRow = curFindRows[0];
					try {
						curHcapScore = (Decimal)curEventRow["HcapScore"];
					} catch {
						curHcapScore = 0;
					}
					try {
						curHcapBase = (Decimal)curEventRow["HCapBase"];
					} catch {
						curHcapBase = 0;
					}
				} else {
					curHcapScore = 0;
				}
				if ( curScore > 0 ) {
					curScore = curScore + curHcapScore;
				}
				curRow["NopsScore"] = curScore;
				curRow["HCapScore"] = curHcapScore;
				curRow["HCapBase"] = curHcapBase;
			}

			return curScoreDataTable;
		}

		private DataTable CalcPointsRatio( DataTable inDataTable, String inSanctionId, String inEvent ) {
			String curFindCommand = "", curMemberId = "";
			Decimal curScore = 0, curHcapScore = 0, curHcapBase = 0;
			DataRow curEventRow;
			DataRow[] curFindRows;

			DataTable curScoreDataTable = inDataTable;

			//Determine number of skiers per placement group
			DataTable curEventDataTable = getEventRegData( inSanctionId, inEvent );

			//Calculate points based on placement
			foreach ( DataRow curRow in curScoreDataTable.Rows ) {
				curMemberId = (String)curRow["MemberId"];
				try {
					if ( inEvent.ToLower().Equals( "jump" ) ) {
						curScore = (Decimal)( curRow["ScoreFeet"] );
					} else {
						if ( curRow["Score"].GetType() == System.Type.GetType( "System.Decimal" ) ) {
							curScore = (Decimal)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int16" ) ) {
							curScore = (Int16)( curRow["Score"] );
						} else if ( curRow["Score"].GetType() == System.Type.GetType( "System.Int32" ) ) {
							curScore = (int)( curRow["Score"] );
						} else {
							curScore = 0;
						}
					}
				} catch {
					curScore = 0;
				}
				curFindCommand = "MemberId = '" + curMemberId + "'";
				curFindRows = curEventDataTable.Select( curFindCommand );
				if ( curFindRows.Length > 0 ) {
					curEventRow = curFindRows[0];
					try {
						curHcapScore = (Decimal)curEventRow["HcapScore"];
					} catch {
						curHcapScore = 0;
					}
					try {
						curHcapBase = (Decimal)curEventRow["HCapBase"];
					} catch {
						curHcapBase = 0;
					}
				} else {
					curHcapScore = 0;
				}
				if ( curScore > 0 && curHcapBase > 0 ) {
					curScore = Math.Round( ( ( curScore / curHcapBase ) * 100 ), 1 );
				}
				curRow["NopsScore"] = curScore;
				curRow["HCapScore"] = curHcapScore;
				curRow["HCapBase"] = curHcapBase;
			}

			return curScoreDataTable;
		}

		private DataTable CalcTeamSummary( DataTable inTeamDataTable, DataTable inScoreDataTable, String inEvent, int inNumPerTeam, String inPlcmtMethod, String inPlcmtOrg ) {
			DataTable curTeamScoreDataTable = buildTeamScoreDataTable();
			return CalcTeamSummary( curTeamScoreDataTable, inTeamDataTable, inScoreDataTable, inEvent, inNumPerTeam, inPlcmtMethod, inPlcmtOrg );
		}
		private DataTable CalcTeamSummary( DataTable inTeamScoreDataTable, DataTable inTeamDataTable, DataTable inScoreDataTable, String inEvent, int inNumPerTeam, String inPlcmtMethod, String inPlcmtOrg ) {
			String curTeamCode, curDiv;
			Decimal curScore = 0, curTeamScore = 0;
			int curNumScoresUsed = 0;
			DataRowView newDataRow;
			DataRow curDataRow;
			DataRow[] curFindRows;
			DataRow[] curFindTeamRows;
			DataTable curTeamScoreDataTable = inTeamScoreDataTable;

			foreach ( DataRow curRow in inScoreDataTable.Rows ) {
				if ( ( (String)curRow["Team" + inEvent] ).Length > 0 ) {
					curTeamCode = (String)curRow["Team" + inEvent];
				} else {
					curTeamCode = "";
				}
				if ( curTeamCode.Length > 0 ) {
					try {
						curScore = (Decimal)( curRow["Points" + inEvent] );
					} catch {
						curScore = 0;
					}
					if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
						curDiv = (String)curRow["AgeGroup"];
						curFindRows = curTeamScoreDataTable.Select( "TeamCode = '" + curTeamCode + "' And Div = '" + curDiv + "'" );
					} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
						curDiv = (String)curRow["EventGroup"];
						curFindRows = curTeamScoreDataTable.Select( "TeamCode = '" + curTeamCode + "' And Div = '" + curDiv + "'" );
					} else {
						curDiv = "";
						curFindRows = curTeamScoreDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
					}
					if ( curFindRows.Length > 0 ) {
						curNumScoresUsed = Convert.ToInt32( curFindRows[0]["NumScoresUsed" + inEvent].ToString() );
						if ( ( inNumPerTeam == 0 ) || ( curNumScoresUsed < inNumPerTeam ) ) {
							curDataRow = curFindRows[0];
							curNumScoresUsed++;
							curDataRow["NumScoresUsed" + inEvent] = curNumScoresUsed;

							curTeamScore = (Decimal)curDataRow["TeamScore" + inEvent];
							curTeamScore += curScore;
							curDataRow["TeamScore" + inEvent] = curTeamScore;
							curTeamScore = (Decimal)curDataRow["TeamScoreSlalom"]
								+ (Decimal)curDataRow["TeamScoreTrick"]
								+ (Decimal)curDataRow["TeamScoreJump"];
							curDataRow["TeamScoreTotal"] = curTeamScore;
						}
					} else {
						curTeamScore = curScore;
						newDataRow = curTeamScoreDataTable.DefaultView.AddNew();
						curNumScoresUsed = 1;
						if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
							curFindTeamRows = inTeamDataTable.Select( "TeamCode = '" + curTeamCode + "' And Div = '" + curDiv + "'" );
						} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
							curFindTeamRows = inTeamDataTable.Select( "TeamCode = '" + curTeamCode + "' And Div = '" + curDiv + "'" );
						} else {
							curFindTeamRows = inTeamDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
						}
						if ( curFindTeamRows.Length == 0 ) {
							if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "group" ) ) {
								curFindTeamRows = inTeamDataTable.Select( "TeamCode = '" + curTeamCode + "' And Div = ''" );
							}
						}
						if ( curFindTeamRows.Length > 0 ) {
							newDataRow["TeamCode"] = curFindTeamRows[0]["TeamCode"].ToString();
							newDataRow["Name"] = curFindTeamRows[0]["Name"].ToString();
							newDataRow["Div"] = curFindTeamRows[0]["Div"].ToString();
							if ( inPlcmtOrg.ToLower().Equals( "div" ) || inPlcmtOrg.ToLower().Equals( "group" ) ) {
								newDataRow["Div"] = curDiv;
							} else {
								newDataRow["Div"] = curFindTeamRows[0]["Div"].ToString();
							}
							newDataRow["DivOrder"] = curFindTeamRows[0]["DivOrder"].ToString();

						} else {
							newDataRow["TeamCode"] = curTeamCode;
							newDataRow["Name"] = "Team " + curTeamCode;
							newDataRow["Div"] = curDiv;
							newDataRow["DivOrder"] = 1;
						}
						newDataRow["TeamScore" + inEvent] = curTeamScore;
						newDataRow["TeamScoreTotal"] = curTeamScore;
						newDataRow["NumScoresUsed" + inEvent] = curNumScoresUsed;
						newDataRow.EndEdit();
					}
				}
			}

			return curTeamScoreDataTable;
		}

		public DataTable CalcTeamCombinedSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, int inNumPerTeam ) {
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curTeamScoreDataTable = expandTeamScoreDataTable( getTeamData( curSanctionId, "Tour", null, curRules ) );

			if ( inSlalomDataTable != null ) {
				curTeamScoreDataTable = CalcTeamEventCombinedSummary( curTeamScoreDataTable, inSlalomDataTable, curSanctionId, curRules, "Slalom", inNumPerTeam );
			}
			if ( inTrickDataTable != null ) {
				curTeamScoreDataTable = CalcTeamEventCombinedSummary( curTeamScoreDataTable, inTrickDataTable, curSanctionId, curRules, "Trick", inNumPerTeam );
			}
			if ( inJumpDataTable != null ) {
				curTeamScoreDataTable = CalcTeamEventCombinedSummary( curTeamScoreDataTable, inJumpDataTable, curSanctionId, curRules, "Jump", inNumPerTeam );
			}

			return curTeamScoreDataTable;
		}

		public DataTable CalcTeamAwsaCombinedSummary( DataRow inTourRow, DataTable inSlalomDataTable, DataTable inTrickDataTable, DataTable inJumpDataTable, int inNumPerTeam ) {
			String curSanctionId = (String)inTourRow["SanctionId"];
			String curRules = (String)inTourRow["Rules"];

			DataTable curTeamScoreDataTable = expandTeamScoreDataTable( getTeamData( curSanctionId, "Tour", null, curRules ) );

			DataTable curTeamScoreFinalDataTable = curTeamScoreDataTable.DefaultView.ToTable();

			if ( inSlalomDataTable != null ) {
				curTeamScoreDataTable = CalcTeamAwsaEventTeamGroupSelect( curTeamScoreDataTable, inSlalomDataTable, "Slalom", inNumPerTeam );
			}
			if ( inTrickDataTable != null ) {
				curTeamScoreDataTable = CalcTeamAwsaEventTeamGroupSelect( curTeamScoreDataTable, inTrickDataTable, "Trick", inNumPerTeam );
			}
			if ( inJumpDataTable != null ) {
				curTeamScoreDataTable = CalcTeamAwsaEventTeamGroupSelect( curTeamScoreDataTable, inJumpDataTable, "Jump", inNumPerTeam );
			}

			curTeamScoreFinalDataTable = CalcTeamAwsaTeamSummary( curTeamScoreFinalDataTable, curTeamScoreDataTable );

			return curTeamScoreFinalDataTable;
		}

		public DataTable CalcTeamEventCombinedNcwsaSummary( DataTable inTeamScoreDataTable, String curSanctionId ) {
			String curEvent = "Tour", curPlcmtOrg = "Tour", curRules = "Ncwsa";
			String curTeamCode;
			Decimal curTeamScore = 0;
			DataRow curTeamRow;
			DataRow curRow;
			DataRow[] curFindRows;
			DataRow[] curSkierList;
			String curFilterCmd = "", curSortCmd = "";

			curFilterCmd = "Div = 'CM' OR Div = 'CW'";
			DataTable curTeamResultsDataTable = expandTeamScoreDataTable( getTeamData( curSanctionId, curPlcmtOrg, curEvent, curRules ) );

			curSkierList = inTeamScoreDataTable.Select( curFilterCmd );

			for ( int curIdx = 0; curIdx < curSkierList.Length; curIdx++ ) {
				curRow = curSkierList[curIdx];

				if ( curRow["TeamCode"] == System.DBNull.Value ) {
					curTeamCode = "";
				} else {
					curTeamCode = (String)curRow["TeamCode"];
				}
				if ( curTeamCode.Length > 0 ) {
					curFindRows = curTeamResultsDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
					if ( curFindRows.Length > 0 ) {
						curTeamRow = curFindRows[0];
						curTeamRow["NumScoresUsedSlalom"] = Convert.ToInt32( curTeamRow["NumScoresUsedSlalom"].ToString() )
							+ Convert.ToInt32( curRow["NumScoresUsedSlalom"].ToString() );
						curTeamRow["NumScoresUsedTrick"] = Convert.ToInt32( curTeamRow["NumScoresUsedTrick"].ToString() )
							+ Convert.ToInt32( curRow["NumScoresUsedTrick"].ToString() );
						curTeamRow["NumScoresUsedJump"] = Convert.ToInt32( curTeamRow["NumScoresUsedJump"].ToString() )
							+ Convert.ToInt32( curRow["NumScoresUsedJump"].ToString() );

						curTeamRow["TeamScoreSlalom"] = (Decimal)curTeamRow["TeamScoreSlalom"] + (Decimal)curRow["TeamScoreSlalom"];
						curTeamRow["TeamScoreTrick"] = (Decimal)curTeamRow["TeamScoreTrick"] + (Decimal)curRow["TeamScoreTrick"];
						curTeamRow["TeamScoreJump"] = (Decimal)curTeamRow["TeamScoreJump"] + (Decimal)curRow["TeamScoreJump"];

						curTeamScore = (Decimal)curTeamRow["TeamScoreSlalom"]
							+ (Decimal)curTeamRow["TeamScoreTrick"]
							+ (Decimal)curTeamRow["TeamScoreJump"];
						curTeamRow["TeamScoreTotal"] = curTeamScore;
					} else {
						MessageBox.Show( "Team code " + curTeamCode + " is not found while building combined NCWSA team results" );
					}
				}
			}

			curSortCmd = "TeamScoreSlalom DESC, TeamCode";
			curTeamResultsDataTable.DefaultView.Sort = curSortCmd;
			curTeamResultsDataTable = curTeamResultsDataTable.DefaultView.ToTable();
			setTeamPlcmt( curTeamResultsDataTable, "Slalom" );
			/*
            int curPlcmt = 0;
            String curPlcmtPos = "";
            Decimal curScore = 0, prevScore = 0;
            foreach (DataRow curScoreRow in curTeamResultsDataTable.Rows) {
                curPlcmt++;
                curScoreRow["PlcmtSlalom"] = curPlcmt;
            }
            */

			curSortCmd = "TeamScoreTrick DESC, TeamCode";
			curTeamResultsDataTable.DefaultView.Sort = curSortCmd;
			curTeamResultsDataTable = curTeamResultsDataTable.DefaultView.ToTable();
			setTeamPlcmt( curTeamResultsDataTable, "Trick" );
			/*
            curPlcmt = 0;
            foreach (DataRow curScoreRow in curTeamResultsDataTable.Rows) {
                curPlcmt++;
                curScoreRow["PlcmtTrick"] = curPlcmt;
            }
            */

			curSortCmd = "TeamScoreJump DESC, TeamCode";
			curTeamResultsDataTable.DefaultView.Sort = curSortCmd;
			curTeamResultsDataTable = curTeamResultsDataTable.DefaultView.ToTable();
			setTeamPlcmt( curTeamResultsDataTable, "Jump" );
			/*
            curPlcmt = 0;
            foreach (DataRow curScoreRow in curTeamResultsDataTable.Rows) {
                curPlcmt++;
                curScoreRow["PlcmtJump"] = curPlcmt;
            }
            */

			curSortCmd = "TeamScoreTotal DESC, TeamCode";
			curTeamResultsDataTable.DefaultView.Sort = curSortCmd;
			curTeamResultsDataTable = curTeamResultsDataTable.DefaultView.ToTable();
			return curTeamResultsDataTable.DefaultView.ToTable();
		}

		private void setTeamPlcmt( DataTable inTeamDataTable, String inEvent ) {
			int curTeamPlcmt = 0, curTieCount = 0, curRowIdx = 0;
			Decimal curScore = 0, prevScore = 0, nextScore = 0;
			String curTeamPlcmtShow = "";

			foreach ( DataRow curRow in inTeamDataTable.Rows ) {
				curScore = (Decimal)curRow["TeamScore" + inEvent];
				if ( curRowIdx > 0 ) {
					prevScore = (Decimal)inTeamDataTable.Rows[curRowIdx - 1]["TeamScore" + inEvent];
					if ( curScore == prevScore ) {
						curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
						curTieCount++;
					} else {
						curTeamPlcmt = curTeamPlcmt + curTieCount + 1;
						curTieCount = 0;
						if ( curRowIdx < ( inTeamDataTable.Rows.Count - 1 ) ) {
							nextScore = (Decimal)inTeamDataTable.Rows[curRowIdx + 1]["TeamScore" + inEvent];
							if ( curScore == nextScore ) {
								curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
							} else {
								curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
							}
						} else {
							curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
						}
					}
				} else {
					if ( curRowIdx < ( inTeamDataTable.Rows.Count - 1 ) ) {
						nextScore = (Decimal)inTeamDataTable.Rows[curRowIdx + 1]["TeamScore" + inEvent];
						if ( curScore == nextScore ) {
							curTeamPlcmt++;
							curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
							curTieCount = 0;
						} else {
							curTeamPlcmt = curTeamPlcmt + curTieCount + 1;
							curTieCount = 0;
							curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
						}
					} else {
						if ( curScore == prevScore ) {
							curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
							curTieCount++;
						} else {
							curTeamPlcmt = curTeamPlcmt + curTieCount + 1;
							curTieCount = 0;
							curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
						}
					}
				}
				curRow["Plcmt" + inEvent] = curTeamPlcmtShow;
				curRowIdx++;
			}
		}

		private DataTable CalcTeamEventCombinedSummary( DataTable inTeamScoreDataTable, DataTable inEventDataTable, String curSanctionId, String curRules, String curEvent, int curNumPerTeam ) {
			String curTeamCode;
			Decimal curScore = 0, curTeamScore = 0;
			int curNumScoresUsed = 0;
			DataTable curDataTable;
			DataRowView newTeamRow;
			DataRow curTeamRow;
			DataRow[] curFindRows;
			DataRow[] curSkierList;
			String curSortCmd = "";
			DataTable curTeamScoreDataTable = inTeamScoreDataTable;

			curSortCmd = "Team" + curEvent + ", Plcmt" + curEvent + " ASC, SkierName";
			inEventDataTable.DefaultView.Sort = curSortCmd;
			curDataTable = inEventDataTable.DefaultView.ToTable();

			foreach ( DataRow curRow in curDataTable.Rows ) {
				if ( curRow["Team" + curEvent] == System.DBNull.Value ) {
					curTeamCode = "";
				} else {
					curTeamCode = (String)curRow["Team" + curEvent];
				}
				if ( curTeamCode.Length > 0 ) {
					try {
						curScore = (Decimal)( curRow["Points" + curEvent] );
					} catch {
						curScore = 0;
					}
					curFindRows = curTeamScoreDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
					if ( curFindRows.Length > 0 ) {
						curNumScoresUsed = Convert.ToInt32( curFindRows[0]["NumScoresUsed" + curEvent].ToString() );
						if ( ( curNumPerTeam == 0 ) || ( curNumScoresUsed < curNumPerTeam ) ) {
							curTeamRow = curFindRows[0];
							curNumScoresUsed++;
							curTeamRow["NumScoresUsed" + curEvent] = curNumScoresUsed;

							curTeamScore = (Decimal)curTeamRow["TeamScore" + curEvent];
							curTeamScore += curScore;
							curTeamRow["TeamScore" + curEvent] = curTeamScore;
							curTeamScore = (Decimal)curTeamRow["TeamScoreSlalom"]
								+ (Decimal)curTeamRow["TeamScoreTrick"]
								+ (Decimal)curTeamRow["TeamScoreJump"];
							curTeamRow["TeamScoreTotal"] = curTeamScore;
						}
					} else {
						curNumScoresUsed = 1;
						curTeamScore = curScore;
						newTeamRow = curTeamScoreDataTable.DefaultView.AddNew();
						newTeamRow["TeamCode"] = curTeamCode;
						newTeamRow["Div"] = "";
						newTeamRow["TeamScore" + curEvent] = curTeamScore;
						newTeamRow["TeamScoreTotal"] = curTeamScore;
						newTeamRow["NumScoresUsed" + curEvent] = curNumScoresUsed;
						newTeamRow.EndEdit();
					}
				}
			}

			curSortCmd = "TeamScore" + curEvent + " DESC, TeamCode";
			curTeamScoreDataTable.DefaultView.Sort = curSortCmd;
			curTeamScoreDataTable = curTeamScoreDataTable.DefaultView.ToTable();
			int curPlcmt = 0;
			foreach ( DataRow curScoreRow in curTeamScoreDataTable.Rows ) {
				curPlcmt++;
				curScoreRow["Plcmt" + curEvent] = curPlcmt;
			}

			return curTeamScoreDataTable;
		}

		public DataTable CalcTeamAwsaEventPoints( DataTable inEventDataTable, String curEvent ) {
			Decimal curScore = 0, curPlcmtPoints = 0;
			int curPlcmtNum = 0;
			String curSortCmd = "", curFilterCmd = "", curDiv = "", curCategory = "", curPlcmt = "";

			curFilterCmd = "Team" + curEvent + " <> ''";
			curSortCmd = "Plcmt" + curEvent + " ASC, Points" + curEvent + " DESC, SkierName";
			inEventDataTable.DefaultView.RowFilter = curFilterCmd;
			inEventDataTable.DefaultView.Sort = curSortCmd;
			DataTable curEventDataTable = inEventDataTable.DefaultView.ToTable();

			int curPlcmtPos = 0, curPlcmtPosCount = inEventDataTable.Rows.Count + 1;
			foreach ( DataRow curRow in curEventDataTable.Rows ) {
				curPlcmtPos++;
				/*
                if ( curRow["Team" + curEvent] == System.DBNull.Value ) {
                    curTeamCode = "";
                } else {
                    curTeamCode = ( String ) curRow["Team" + curEvent];
                }
                */
				curDiv = (String)curRow["AgeGroup"];
				if ( ( curDiv.Substring( 0, 1 ).Equals( "M" ) && !curDiv.Equals( "MW" ) ) || curDiv.Equals( "OM" ) || curDiv.Equals( "MM" ) ) {
					curCategory = "Cat1";
				} else if ( curDiv.Substring( 0, 1 ).Equals( "W" ) || curDiv.Equals( "OW" ) || curDiv.Equals( "MW" ) ) {
					curCategory = "Cat2";
				} else if ( curDiv.Substring( 0, 1 ).Equals( "B" ) ) {
					curCategory = "Cat3";
				} else if ( curDiv.Substring( 0, 1 ).Equals( "G" ) ) {
					curCategory = "Cat3";
				} else {
					curCategory = "";
				}
				if ( curCategory.Length > 0 ) {
					curRow["EventGroup"] = curCategory;
					try {
						curScore = (Decimal)( curRow["Points" + curEvent] );
						curPlcmt = (String)curRow["Plcmt" + curEvent];
						if ( Int32.TryParse( curPlcmt, out curPlcmtNum ) ) {
							curPlcmtPoints = ( curPlcmtPosCount - curPlcmtNum ) * 10;
						} else {
							if ( curPlcmt.Contains( "T" ) ) {
								if ( Int32.TryParse( curPlcmt.Substring( 0, curPlcmt.IndexOf( "T" ) ), out curPlcmtNum ) ) {
									curPlcmtPoints = ( ( curPlcmtPosCount - curPlcmtNum ) * 10 ) + 5; ;
								} else {
									curPlcmtPoints = 0;
								}
							} else {
								curPlcmtPoints = 0;
							}
						}
					} catch {
						curScore = 0;
						curPlcmtPoints = 0;
					}
					curRow["PlcmtPoints" + curEvent] = curPlcmtPoints;
				}
			}

			return curEventDataTable;
		}

		private DataTable CalcTeamAwsaEventTeamGroupSelect( DataTable inTeamScoreDataTable, DataTable inEventDataTable, String curEvent, int curNumPerTeam ) {
			String curTeamCode;
			Decimal curScore = 0, curTeamScore = 0;
			int curNumScoresUsed = 0;
			DataRowView newTeamRow;
			DataRow curTeamRow;
			DataRow[] curFindRows;
			String curSortCmd = "", curTeamCategory = "";

			curSortCmd = "Team" + curEvent + " ASC, EventGroup ASC, PlcmtPoints" + curEvent + " DESC, SkierName";
			inEventDataTable.DefaultView.Sort = curSortCmd;
			inEventDataTable = inEventDataTable.DefaultView.ToTable();
			DataTable curTeamScoreDataTable = inTeamScoreDataTable;

			foreach ( DataRow curRow in inEventDataTable.Rows ) {
				if ( curRow["Team" + curEvent] == System.DBNull.Value ) {
					curTeamCode = "";
				} else {
					curTeamCode = (String)curRow["Team" + curEvent];
					curTeamCategory = (String)curRow["EventGroup"];
				}
				if ( curTeamCode.Length > 0 && curTeamCategory.Length > 0 ) {
					try {
						curScore = (Decimal)( curRow["PlcmtPoints" + curEvent] );
					} catch {
						curScore = 0;
					}
					curFindRows = curTeamScoreDataTable.Select( "TeamCode = '" + curTeamCode + "' AND Div = '" + curTeamCategory + "'" );
					if ( curFindRows.Length > 0 ) {
						curTeamRow = curFindRows[0];
						curNumScoresUsed = Convert.ToInt32( curTeamRow["NumScoresUsed" + curEvent].ToString() );
						if ( ( curNumPerTeam == 0 ) || ( curNumScoresUsed < curNumPerTeam ) ) {
							curNumScoresUsed++;
							curTeamRow["NumScoresUsed" + curEvent] = curNumScoresUsed;

							curTeamScore = (Decimal)curTeamRow["TeamScore" + curEvent];
							curTeamScore += curScore;
							curTeamRow["TeamScore" + curEvent] = curTeamScore;
							curTeamScore = (Decimal)curTeamRow["TeamScoreSlalom"]
								+ (Decimal)curTeamRow["TeamScoreTrick"]
								+ (Decimal)curTeamRow["TeamScoreJump"];
							curTeamRow["TeamScoreTotal"] = curTeamScore;
						}
					} else {
						curNumScoresUsed = 1;
						curTeamScore = curScore;
						newTeamRow = curTeamScoreDataTable.DefaultView.AddNew();
						newTeamRow["TeamCode"] = curTeamCode;
						newTeamRow["Div"] = curTeamCategory;
						newTeamRow["TeamScore" + curEvent] = curTeamScore;
						newTeamRow["TeamScoreTotal"] = curTeamScore;
						newTeamRow["NumScoresUsed" + curEvent] = curNumScoresUsed;
						newTeamRow.EndEdit();
					}
				}
			}

			return curTeamScoreDataTable;
		}

		private DataTable CalcTeamAwsaTeamSummary( DataTable inTeamScoreFinalDataTable, DataTable inTeamScoreDataTable ) {
			String curTeamCode;
			int curNumScoresUsed = 0;
			DataRow curTeamRow;
			DataRow[] curFindRows;
			String curSortCmd = "";

			curSortCmd = "TeamCode ASC, Div ASC";
			inTeamScoreDataTable.DefaultView.Sort = curSortCmd;
			inTeamScoreDataTable = inTeamScoreDataTable.DefaultView.ToTable();
			String prevTeamCode = "";

			foreach ( DataRow curRow in inTeamScoreDataTable.Rows ) {
				curTeamCode = (String)curRow["TeamCode"];
				if ( prevTeamCode.Equals( "" ) ) prevTeamCode = curTeamCode;
				if ( curTeamCode == prevTeamCode ) {
					curFindRows = inTeamScoreFinalDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
					if ( curFindRows.Length > 0 ) {
						curTeamRow = curFindRows[0];
						curTeamRow["TeamScoreSlalom"] = (Decimal)curTeamRow["TeamScoreSlalom"] + (Decimal)curRow["TeamScoreSlalom"];
						curTeamRow["TeamScoreTrick"] = (Decimal)curTeamRow["TeamScoreTrick"] + (Decimal)curRow["TeamScoreTrick"];
						curTeamRow["TeamScoreJump"] = (Decimal)curTeamRow["TeamScoreJump"] + (Decimal)curRow["TeamScoreJump"];
						curTeamRow["TeamScoreTotal"] = (Decimal)curTeamRow["TeamScoreSlalom"]
							+ (Decimal)curTeamRow["TeamScoreTrick"]
							+ (Decimal)curTeamRow["TeamScoreJump"];
						curTeamRow["NumScoresUsedSlalom"] = (int)curTeamRow["NumScoresUsedSlalom"] + (int)curRow["NumScoresUsedSlalom"];
						curTeamRow["NumScoresUsedTrick"] = (int)curTeamRow["NumScoresUsedTrick"] + (int)curRow["NumScoresUsedTrick"];
						curTeamRow["NumScoresUsedJump"] = (int)curTeamRow["NumScoresUsedJump"] + (int)curRow["NumScoresUsedJump"];
						curTeamRow.EndEdit();
					} else {
						MessageBox.Show( "TeamCode not found " + curTeamCode );
					}
				} else {
					curFindRows = inTeamScoreFinalDataTable.Select( "TeamCode = '" + curTeamCode + "'" );
					if ( curFindRows.Length > 0 ) {
						curTeamRow = curFindRows[0];
						curTeamRow["TeamScoreSlalom"] = (Decimal)curRow["TeamScoreSlalom"];
						curTeamRow["TeamScoreTrick"] = (Decimal)curRow["TeamScoreTrick"];
						curTeamRow["TeamScoreJump"] = (Decimal)curRow["TeamScoreJump"];
						curTeamRow["TeamScoreTotal"] = (Decimal)curRow["TeamScoreSlalom"]
							+ (Decimal)curRow["TeamScoreTrick"]
							+ (Decimal)curRow["TeamScoreJump"];
						curTeamRow["NumScoresUsedSlalom"] = curNumScoresUsed;
						curTeamRow["NumScoresUsedTrick"] = curNumScoresUsed;
						curTeamRow["NumScoresUsedJump"] = curNumScoresUsed;
						curTeamRow.EndEdit();
					} else {
						MessageBox.Show( "TeamCode not found " + curTeamCode );
					}
				}
				prevTeamCode = curTeamCode;
			}

			curSortCmd = "TeamScoreTotal DESC, TeamCode ASC";
			inTeamScoreFinalDataTable.DefaultView.Sort = curSortCmd;
			inTeamScoreFinalDataTable = inTeamScoreFinalDataTable.DefaultView.ToTable();

			return inTeamScoreFinalDataTable;
		}

		private DataTable expandScoreJumpDataTable( DataTable inDataTable ) {
			//Add placement columns to data returned from database
			DataTable curScoreDataTable = expandScoreDataTable( inDataTable, "Jump" );

			curScoreDataTable.Columns["ScoreMeters"].ReadOnly = false;
			curScoreDataTable.Columns["ScoreFeet"].ReadOnly = false;

			return curScoreDataTable;
		}

		public DataTable expandScoreDataTable( DataTable inDataTable, String inEvent ) {
			//Add placement columns to data returned from database
			DataTable curScoreDataTable = inDataTable.Copy();

			DataColumn curCol = new DataColumn();

			curCol.ColumnName = "Plcmt" + inEvent;
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.DefaultValue = "";
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapScore";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.DefaultValue = 0;
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curScoreDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapBase";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.DefaultValue = 0;
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curScoreDataTable.Columns.Add( curCol );

			return curScoreDataTable;
		}

		private DataTable buildMaxScoresPerGroup( String inSanctionId, String inPlcmtOrg, String inRules, String inEvent ) {
			return buildMaxScoresPerGroup( inSanctionId, inPlcmtOrg, inRules, inEvent, false );
		}
		private DataTable buildMaxScoresPerGroup( String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, Boolean inTeamPlcmt ) {
			#region Build data table to maintain max scores per event group
			DataTable curMaxDataTable = new DataTable();
			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "Group";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreTrick";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreSlalomMax";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreTrickMax";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreJumpMax";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0.00;
			curMaxDataTable.Columns.Add( curCol );
			#endregion

			bool useWrlOverallScoreBasis = false;
			if ( HelperFunctions.isIwwfEvent(inRules)) {
				DialogResult curDialogResult = MessageBox.Show( "Do you want to use World Ranking List overall basis scores (Yes)"
					+ "\nOr\nUse the best score by an overall skier achieved at this event(No) "
					, "IWwf Overall", MessageBoxButtons.YesNoCancel );
				if ( curDialogResult == DialogResult.Yes ) useWrlOverallScoreBasis = true;
			}
			if ( useWrlOverallScoreBasis ) {
				//String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, Boolean inTeamPlcmt
				buildWrlOverallScoreBasis( curMaxDataTable, inSanctionId, inEvent );

			} else {
				DataTable curEligSkiersDataTable = null;
				if ( inTeamPlcmt ) {
					curEligSkiersDataTable = getTeamSkierList( inSanctionId, inEvent );
				} else {
					curEligSkiersDataTable = getOverallSkierList( inSanctionId );
				}
				if ( inEvent.ToLower().Equals( "slalom" ) || inEvent.ToLower().Equals( "overall" ) || inEvent.ToLower().Equals( "scorebook" ) ) {
					buildSlalomMaxScoresPerGroup( curMaxDataTable, curEligSkiersDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, inTeamPlcmt );
				}
				if ( inEvent.ToLower().Equals( "trick" ) || inEvent.ToLower().Equals( "overall" ) || inEvent.ToLower().Equals( "scorebook" ) ) {
					buildTrickMaxScoresPerGroup( curMaxDataTable, curEligSkiersDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, inTeamPlcmt );
				}
				if ( inEvent.ToLower().Equals( "jump" ) || inEvent.ToLower().Equals( "overall" ) || inEvent.ToLower().Equals( "scorebook" ) ) {
					buildJumpMaxScoresPerGroup( curMaxDataTable, curEligSkiersDataTable, inSanctionId, inPlcmtOrg, inRules, inEvent, inTeamPlcmt );
				}
			}

			return curMaxDataTable;
		}

		/*
		 * Determine maximum slalom scores per event per division
		 */
		private void buildSlalomMaxScoresPerGroup( DataTable curMaxDataTable, DataTable curEligSkiersDataTable, String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, Boolean inTeamPlcmt ) {
			String curGroup = "", curFilterCommand = "", curAgeGroup = "", curMemberId = "";
			Decimal curScore;
			DataRowView newMaxRow;
			DataRow curMaxRow;
			DataRow[] curFindRows = null, curFindRowsOverallElig = null;

			DataTable curDataTable = getSlalomSummaryData( inSanctionId, "Round", inRules, "all", "all" );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curGroup = (String)curRow["EventGroup"];
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = "";
				}

				curMemberId = (String)curRow["MemberId"];
				curAgeGroup = (String)curRow["AgeGroup"];
				try {
					curScore = (Decimal)curRow["Score"];
				} catch {
					curScore = 0;
				}
				if ( curEligSkiersDataTable != null ) {
					curFindRowsOverallElig = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				}

				curFilterCommand = "Group = '" + curGroup + "'";
				curFindRows = curMaxDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					curMaxRow = curFindRows[0];
				} else {
					newMaxRow = curMaxDataTable.DefaultView.AddNew();
					newMaxRow["Group"] = curGroup;
					newMaxRow["ScoreSlalom"] = 0M;
					newMaxRow["ScoreTrick"] = 0;
					newMaxRow["ScoreJump"] = 0M;
					newMaxRow["ScoreSlalomMax"] = 0M;
					newMaxRow["ScoreTrickMax"] = 0;
					newMaxRow["ScoreJumpMax"] = 0M;
					newMaxRow.EndEdit();
					curMaxRow = curMaxDataTable.Rows[curMaxDataTable.Rows.Count - 1];
				}

				if ( curFindRowsOverallElig.Length > 0 && curScore > (Decimal)curMaxRow["ScoreSlalomMax"] ) {
					curMaxRow["ScoreSlalomMax"] = curScore;
				}
				if ( HelperFunctions.isIwwfEvent(inRules) ) {
					if ( inTeamPlcmt ) {
						if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curRow, "TeamCode", "" ) ) ) {
							if ( curScore > (Decimal)curMaxRow["ScoreSlalom"] ) {
								curMaxRow["ScoreSlalom"] = curScore;
							}
						}
					} else {
						if ( curFindRowsOverallElig.Length > 0 ) {
							if ( curScore > (Decimal)curMaxRow["ScoreSlalom"] ) {
								curMaxRow["ScoreSlalom"] = curScore;
							}
						}
					}
				}
			}
		}

		/*
		 * Determine maximum slalom scores per event per division
		 */
		private void buildTrickMaxScoresPerGroup( DataTable curMaxDataTable, DataTable curEligSkiersDataTable, String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, Boolean inTeamPlcmt ) {
			String curGroup = "", curFilterCommand = "", curAgeGroup = "", curMemberId = "";
			Decimal curScore;
			DataRowView newMaxRow;
			DataRow curMaxRow;
			DataRow[] curFindRows = null, curFindRowsOverallElig = null;
			
			DataTable curDataTable = getTrickSummaryData( inSanctionId, "Round", inRules, "all", "all" );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curGroup = (String)curRow["EventGroup"];
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = "";
				}

				curMemberId = (String)curRow["MemberId"];
				curAgeGroup = (String)curRow["AgeGroup"];
				try {
					curScore = Convert.ToDecimal( (Int16)curRow["Score"] );
				} catch {
					curScore = 0;
				}
				if ( curEligSkiersDataTable != null ) {
					curFindRowsOverallElig = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				}

				curFilterCommand = "Group = '" + curGroup + "'";
				curFindRows = curMaxDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					curMaxRow = curFindRows[0];
				} else {
					newMaxRow = curMaxDataTable.DefaultView.AddNew();
					newMaxRow["Group"] = curGroup;
					newMaxRow["ScoreSlalom"] = 0M;
					newMaxRow["ScoreTrick"] = 0;
					newMaxRow["ScoreJump"] = 0M;
					newMaxRow["ScoreSlalomMax"] = 0M;
					newMaxRow["ScoreTrickMax"] = 0;
					newMaxRow["ScoreJumpMax"] = 0M;
					newMaxRow.EndEdit();
					curMaxRow = curMaxDataTable.Rows[curMaxDataTable.Rows.Count - 1];
				}

				if ( curFindRowsOverallElig.Length > 0
					&& curScore > Convert.ToDecimal( (Int16)curMaxRow["ScoreTrickMax"] )
					) {
					curMaxRow["ScoreTrickMax"] = curScore;
				}
				if ( HelperFunctions.isIwwfEvent(inRules) ) {
					if ( inTeamPlcmt ) {
						if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curRow, "TeamCode", "" ) ) ) {
							if ( curScore > (Int16)curMaxRow["ScoreTrick"] ) {
								curMaxRow["ScoreTrick"] = curScore;
							}
						}
					} else {
						if ( curFindRowsOverallElig.Length > 0 ) {
							if ( curScore > Convert.ToDecimal( (Int16)curMaxRow["ScoreTrick"] ) ) {
								curMaxRow["ScoreTrick"] = curScore;
							}
						}
					}
				}
			}
		}

		/*
		 * Determine maximum slalom scores per event per division
		 */
		private void buildJumpMaxScoresPerGroup( DataTable curMaxDataTable, DataTable curEligSkiersDataTable, String inSanctionId, String inPlcmtOrg, String inRules, String inEvent, Boolean inTeamPlcmt ) {
			String curGroup = "", curFilterCommand = "", curAgeGroup = "", curMemberId = "";
			Decimal curScore;
			DataRowView newMaxRow;
			DataRow curMaxRow;
			DataRow[] curFindRows = null, curFindRowsOverallElig = null;
			
			DataTable curDataTable = getJumpSummaryData( inSanctionId, "Round", inRules, "all", "all" );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curGroup = (String)curRow["EventGroup"];
				if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
					curGroup = (String)curRow["AgeGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curGroup = (String)curRow["AgeGroup"] + "-" + (String)curRow["EventGroup"];
				} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
					curGroup = (String)curRow["EventGroup"];
				} else {
					curGroup = "";
				}

				curMemberId = (String)curRow["MemberId"];
				curAgeGroup = (String)curRow["AgeGroup"];
				try {
					if ( HelperFunctions.isIwwfEvent(inRules) ) {
						curScore = (Decimal)curRow["ScoreMeters"];
					} else {
						curScore = (Decimal)curRow["ScoreFeet"];
					}
				} catch {
					curScore = 0;
				}
				if ( curEligSkiersDataTable != null ) {
					curFindRowsOverallElig = curEligSkiersDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
				}

				curFilterCommand = "Group = '" + curGroup + "'";
				curFindRows = curMaxDataTable.Select( curFilterCommand );
				if ( curFindRows.Length > 0 ) {
					curMaxRow = curFindRows[0];
				} else {
					newMaxRow = curMaxDataTable.DefaultView.AddNew();
					newMaxRow["Group"] = curGroup;
					newMaxRow["ScoreSlalom"] = 0M;
					newMaxRow["ScoreTrick"] = 0;
					newMaxRow["ScoreJump"] = 0M;
					newMaxRow["ScoreSlalomMax"] = 0M;
					newMaxRow["ScoreTrickMax"] = 0;
					newMaxRow["ScoreJumpMax"] = 0M;
					newMaxRow.EndEdit();
					curMaxRow = curMaxDataTable.Rows[curMaxDataTable.Rows.Count - 1];
				}

				if ( curFindRowsOverallElig.Length > 0 && curScore > (Decimal)curMaxRow["ScoreJumpMax"] ) {
					curMaxRow["ScoreJumpMax"] = curScore;
				}
				if ( HelperFunctions.isIwwfEvent(inRules) ) {
					if ( inTeamPlcmt ) {
						if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curRow, "TeamCode", "" ) ) ) {
							if ( curScore > (Decimal)curMaxRow["ScoreJump"] ) {
								curMaxRow["ScoreJump"] = curScore;
							}
						}
					} else {
						if ( curFindRowsOverallElig.Length > 0 ) {
							if ( curScore > (Decimal)curMaxRow["ScoreJump"] ) {
								curMaxRow["ScoreJump"] = curScore;
							}
						}
					}
				}
			}
		}

		/*
		 * Retrieve ranking list overall score basis data
		 */
		private void buildWrlOverallScoreBasis( DataTable curMaxDataTable, String inSanctionId, String inEvent ) {
			DataTable curDataTable = getWrlOverallScoreBasis();
			foreach ( DataRow curRow in curDataTable.Rows ) {
				DataRowView newMaxRow = curMaxDataTable.DefaultView.AddNew();
				newMaxRow["Group"] = curRow["Div"];
				newMaxRow["ScoreSlalom"] = curRow["SlalomScoreBasis"];
				newMaxRow["ScoreTrick"] = curRow["TrickScoreBasis"];
				newMaxRow["ScoreJump"] = curRow["JumpScoreBasis"];
				newMaxRow["ScoreSlalomMax"] = curRow["SlalomScoreBasis"]; ;
				newMaxRow["ScoreTrickMax"] = curRow["TrickScoreBasis"]; ;
				newMaxRow["ScoreJumpMax"] = curRow["JumpScoreBasis"]; ;
				newMaxRow.EndEdit();
			}
		}

		private DataTable getWrlOverallScoreBasis() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select S.Div, SlalomScoreBasis, TrickScoreBasis, JumpScoreBasis " );
			curSqlStmt.Append( "From ( Select SUBSTRING( ListCode, 1, 2 ) as Div, MaxValue as SlalomScoreBasis" );
			curSqlStmt.Append( "       From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-S' ) as S " );
			curSqlStmt.Append( "INNER JOIN ( Select SUBSTRING(ListCode, 1, 2) as Div, MaxValue as TrickScoreBasis" );
			curSqlStmt.Append( "             From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-T') as T" );
			curSqlStmt.Append( "             ON T.Div = S.Div " );
			curSqlStmt.Append( "INNER JOIN ( Select SUBSTRING(ListCode, 1, 2) as Div, MaxValue as JumpScoreBasis" );
			curSqlStmt.Append( "             From CodeValueList Where ListName = 'IwwfOverallWrlBasis' AND ListCode like '%-J') as J" );
			curSqlStmt.Append( "             ON J.Div = S.Div" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable buildTeamScoreDataTable() {
			DataTable curDataTable = new DataTable();
			DataColumn curCol = new DataColumn();

			curDataTable = new DataTable();
			curCol = new DataColumn();
			curCol.ColumnName = "TeamCode";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Name";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Div";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "DivOrder";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreTotal";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedSlalom";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedTrick";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedJump";
			curCol.DataType = System.Type.GetType( "System.Int32" );
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
			curCol.ColumnName = "PlcmtTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );
			curCol.DefaultValue = 0;
			return curDataTable;
		}

		private DataTable expandTeamScoreDataTable( DataTable curDataTable ) {
			DataColumn curCol = new DataColumn();

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamScoreTotal";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedSlalom";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedTrick";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "NumScoresUsedJump";
			curCol.DataType = System.Type.GetType( "System.Int32" );
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
			curCol.ColumnName = "PlcmtTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );
			return curDataTable;
		}

		private DataTable buildOverallSummaryDataTable() {
			//Determine number of skiers per placement group
			DataTable curDataTable = new DataTable();

			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "MemberId";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "SanctionId";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "SkierName";
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
			curCol.ColumnName = "RunOrderGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ReadyToSki";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ReadyForPlcmt";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "State";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Federation";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "City";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "SkiYearAge";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Round";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ReadyForPlcmtSlalom";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventClassSlalom";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamSlalom";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroupSlalom";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "DivOrderSlalom";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RoundSlalom";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PointsSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtPointsSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapScoreSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapBaseSlalom";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "FinalPassScore";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "FinalSpeedMph";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "FinalSpeedKph";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "CompletedSpeedMph";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "CompletedSpeedKph";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "FinalLenOff";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "FinalLen";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "MaxSpeed";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "StartSpeed";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "StartLen";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtSlalom";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ReadyForPlcmtTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventClassTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroupTrick";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RoundTrick";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "DivOrderTrick";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreTrick";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Pass1Trick";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Pass2Trick";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PointsTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtPointsTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapScoreTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapBaseTrick";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
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
			curCol.ColumnName = "ReadyForPlcmtJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventClassJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "TeamJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroupJump";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RoundJump";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "DivOrderJump";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "SpeedKphJump";
			curCol.DataType = System.Type.GetType( "System.Byte" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RampHeight";
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
			curCol.ColumnName = "ScoreFeet";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PointsJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtPointsJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapScoreJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "HCapBaseJump";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
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
			curCol.ColumnName = "EventGroupOverall";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RoundOverall";
			curCol.DataType = System.Type.GetType( "System.Int16" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "DivOrderOverall";
			curCol.DataType = System.Type.GetType( "System.Int32" );
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "ScoreOverall";
			curCol.DataType = System.Type.GetType( "System.Decimal" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 0;
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "PlcmtOverall";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "QualifyOverall";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EligOverall";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			return curDataTable;
		}

		public DataTable getSlalomSummaryData( String inSanctionId, String inDataType, String inRules, String inEventGroup, String inDiv ) {
			bool curTeamData = false;
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) ) {
					curEventGroup = "";
				} else if ( inEventGroup.ToLower().Equals( "team" ) ) {
					curTeamData = true;
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, COALESCE(O.EventGroup, ER.EventGroup) as EventGroup, ER.TeamCode" );
			curSqlStmt.Append( ", COALESCE(O.EventGroup, ER.EventGroup) as EventGroupRound, COALESCE(O.RunOrderGroup, '') as RunOrderGroup, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", TR.SkiYearAge, TR.State, TR.Federation, TR.City" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtSlalom" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass" );
			curSqlStmt.Append( ", COALESCE(SS.Round, 0) as Round, SS.Score, SS.NopsScore, SS.Rating, SS.MaxSpeed, SS.StartSpeed, SS.StartLen, SS.Status" );
			curSqlStmt.Append( ", SS.FinalSpeedMph, SS.FinalSpeedKph, SS.FinalLen, SS.FinalLenOff, SS.FinalPassScore" );
			curSqlStmt.Append( ", COALESCE(SS.CompletedSpeedMph, 0) as CompletedSpeedMph, COALESCE(SS.CompletedSpeedKph, 0) as CompletedSpeedKph " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
			curSqlStmt.Append( "  INNER JOIN SlalomScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25" );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventRunOrder O ON O.SanctionId = TR.SanctionId AND O.MemberId = TR.MemberId AND O.AgeGroup = TR.AgeGroup AND O.Event = ER.Event AND O.Round = SS.Round " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			if ( curTeamData ) {
				curSqlStmt.Append( "  INNER JOIN TeamList TM ON TR.SanctionId = TM.SanctionId AND ER.TeamCode = TM.TeamCode " );
			}
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Slalom' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "  And SS.Round in (Select Min(Round) From SlalomScore SX WHERE SanctionId =  SS.SanctionId AND MemberId = SS.MemberId AND AgeGroup = SS.AgeGroup) " );
			}

			if ( inDataType.ToLower().Equals( "best" ) ) {
				curSqlStmt.Append( "  AND SS.Round IN " );
				curSqlStmt.Append( "      (SELECT MIN(Round) AS BestRoundNum FROM SlalomScore AS SS2 " );
				curSqlStmt.Append( "       WHERE SS2.SanctionId = TR.SanctionId AND SS2.MemberId = TR.MemberId AND SS2.AgeGroup = TR.AgeGroup " );
				curSqlStmt.Append( "         AND Score IN " );
				curSqlStmt.Append( "             (SELECT MAX(Score) AS BestScore FROM SlalomScore AS SS3 " );
				curSqlStmt.Append( "              WHERE SS3.MemberId = SS2.MemberId AND SS3.SanctionId = SS2.SanctionId AND SS3.AgeGroup = SS2.AgeGroup AND SS3.Round < 25 " );
				curSqlStmt.Append( "              ) ) " );
				curSqlStmt.Append( "ORDER BY ER.EventGroup, SS.NopsScore DESC, TR.SkierName " );
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "round" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
				curSqlStmt.Append( "ORDER BY RunOrderGroup ASC, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "total" ) ) {
				if ( HelperFunctions.isCollegiateEvent(inRules) ) {
					curSqlStmt.Append( "ORDER BY ER.AgeGroup, TR.SkierName, SS.Round " );
				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass" );
					curSqlStmt.Append( ", SUM(SS.Score) AS Score, SUM(SS.NopsScore) AS NopsScore, 0 AS Round, 'N/A' AS Rating, count(*) as RoundCount " );
					curSqlStmt.Append( ", COALESCE(DV.RunOrder, 999) as DivOrder " );
					curSqlStmt.Append( "FROM TourReg TR " );
					curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
					curSqlStmt.Append( "  LEFT OUTER JOIN SlalomScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25 " );
					curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
					curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Slalom' " );
					if ( curEventGroup.Length > 0 ) {
						curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
					}
					if ( curDiv.Length > 0 ) {
						curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
					}
					curSqlStmt.Append( "GROUP BY TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass, DV.RunOrder " );
				}
			} else if ( inDataType.ToLower().Equals( "all" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			}

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getSlalomDetailData( String inSanctionId, String inEventGroup, String inDiv ) {
			bool curTeamData = false;
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) ) {
					curEventGroup = "";
				} else if ( inEventGroup.ToLower().Equals( "team" ) ) {
					curTeamData = true;
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtSlalom" );
			curSqlStmt.Append( ", SS.Round, SS.Score, SS.NopsScore, SS.Rating, MaxSpeed, StartSpeed, StartLen, Status" );
			curSqlStmt.Append( ", FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore" );
			curSqlStmt.Append( ", COALESCE(CompletedSpeedMph, 0) as CompletedSpeedMph, COALESCE(CompletedSpeedKph, 0) as CompletedSpeedKph " );
			curSqlStmt.Append( "FROM SlalomScore SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			if ( curTeamData ) {
				curSqlStmt.Append( "  INNER JOIN TeamList TM ON SS.SanctionId = TM.SanctionId AND ER.TeamCode = TM.TeamCode " );
			}
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Slalom' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, TR.SkierName, SS.MemberId, SS.Round" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getTrickSummaryData( String inSanctionId, String inDataType, String inRules, String inEventGroup, String inDiv ) {
			bool curTeamData = false;
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) ) {
					curEventGroup = "";
				} else if ( inEventGroup.ToLower().Equals( "team" ) ) {
					curTeamData = true;
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
			curSqlStmt.Append( ", COALESCE(O.EventGroup, '') as EventGroupRound, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", TR.SkiYearAge, TR.State, TR.Federation, TR.City" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtTrick" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, COALESCE(SS.Round, 0) as Round, SS.Score, SS.ScorePass1, SS.ScorePass2, SS.NopsScore, SS.Rating " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
			curSqlStmt.Append( "  INNER JOIN TrickScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25" );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventRunOrder O ON O.SanctionId = TR.SanctionId AND O.MemberId = TR.MemberId AND O.AgeGroup = TR.AgeGroup AND O.Event = ER.Event AND O.Round = SS.Round " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			if ( curTeamData ) {
				curSqlStmt.Append( "  INNER JOIN TeamList TM ON TR.SanctionId = TM.SanctionId AND ER.TeamCode = TM.TeamCode " );
			}
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Trick' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "  And SS.Round in (Select Min(Round) From TrickScore SX WHERE SanctionId =  SS.SanctionId AND MemberId = SS.MemberId AND AgeGroup = SS.AgeGroup) " );
			}

			if ( inDataType.ToLower().Equals( "best" ) ) {
				curSqlStmt.Append( "  AND SS.Round IN " );
				curSqlStmt.Append( "      (SELECT MIN(Round) AS BestRoundNum FROM TrickScore AS SS2 " );
				curSqlStmt.Append( "       WHERE SS2.SanctionId = TR.SanctionId AND SS2.MemberId = TR.MemberId AND SS2.AgeGroup = TR.AgeGroup " );
				curSqlStmt.Append( "         AND Score IN " );
				curSqlStmt.Append( "             (SELECT MAX(Score) AS BestScore FROM TrickScore AS SS3 " );
				curSqlStmt.Append( "              WHERE SS3.MemberId = SS2.MemberId AND SS3.SanctionId = SS2.SanctionId AND SS3.AgeGroup = SS2.AgeGroup AND SS3.Round < 25 " );
				curSqlStmt.Append( "              ) ) " );
				curSqlStmt.Append( "ORDER BY ER.EventGroup, SS.NopsScore DESC, TR.SkierName " );
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "round" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
				curSqlStmt.Append( "ORDER BY RunOrderGroup ASC, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "total" ) ) {
				if ( HelperFunctions.isCollegiateEvent(inRules) ) {
					curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass" );
					curSqlStmt.Append( ", SUM(SS.Score) AS Score, Sum(SS.ScorePass1) as ScorePass1, Sum(SS.ScorePass2) as ScorePass2, SUM(SS.NopsScore) AS NopsScore, 0 AS Round, 'N/A' AS Rating " );
					curSqlStmt.Append( ", COALESCE(DV.RunOrder, 999) as DivOrder " );
					curSqlStmt.Append( "FROM TourReg TR " );
					curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
					curSqlStmt.Append( "  LEFT OUTER JOIN TrickScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25 " );
					curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
					curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Trick' " );
					if ( curEventGroup.Length > 0 ) {
						curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
					}
					if ( curDiv.Length > 0 ) {
						curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
					}
					curSqlStmt.Append( "GROUP BY TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass, DV.RunOrder " );
				}
			} else if ( inDataType.ToLower().Equals( "all" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			}

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getTrickDetailData( String inSanctionId, String inEventGroup, String inDiv ) {
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) ) {
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtTrick" );
			curSqlStmt.Append( ", SS.Round, SS.Rating, '' as BoatCode, SS.Score, SS.NopsScore, SS.ScorePass1, SS.ScorePass2 " );
			curSqlStmt.Append( "FROM TrickScore AS SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg as TR ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg as ER ON SS.SanctionId = ER.SanctionId AND SS.MemberId = ER.MemberId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Trick' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			curSqlStmt.Append( "ORDER BY SS.SanctionId, ER.AgeGroup, TR.SkierName, SS.MemberId, SS.Round" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getJumpSummaryData( String inSanctionId, String inDataType, String inRules, String inEventGroup, String inDiv ) {
			bool curTeamData = false;
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) ) {
					curEventGroup = "";
				} else if ( inEventGroup.ToLower().Equals( "team" ) ) {
					curTeamData = true;
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode" );
			curSqlStmt.Append( ", COALESCE(O.EventGroup, '') as EventGroupRound, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", TR.SkiYearAge, TR.State, TR.Federation, TR.City" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtJump" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass" );
			curSqlStmt.Append( ", COALESCE(SS.Round, 0) as Round, SS.ScoreFeet, SS.ScoreMeters, SS.NopsScore, SS.Rating, SS.BoatSpeed, SS.RampHeight " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
			curSqlStmt.Append( "  INNER JOIN JumpScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25" );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventRunOrder O ON O.SanctionId = TR.SanctionId AND O.MemberId = TR.MemberId AND O.AgeGroup = TR.AgeGroup AND O.Event = ER.Event AND O.Round = SS.Round " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			if ( curTeamData ) {
				curSqlStmt.Append( "  INNER JOIN TeamList TM ON TR.SanctionId = TM.SanctionId AND ER.TeamCode = TM.TeamCode " );
			}
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Jump' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "  And SS.Round in (Select Min(Round) From JumpScore SX WHERE SanctionId =  SS.SanctionId AND MemberId = SS.MemberId AND AgeGroup = SS.AgeGroup) " );
			}

			if ( inDataType.ToLower().Equals( "best" ) ) {
				curSqlStmt.Append( "  AND SS.Round IN " );
				curSqlStmt.Append( "      (SELECT MIN(Round) AS BestRoundNum FROM JumpScore AS SS2 " );
				curSqlStmt.Append( "       WHERE SS2.SanctionId = TR.SanctionId AND SS2.MemberId = TR.MemberId AND SS2.AgeGroup = TR.AgeGroup " );
				curSqlStmt.Append( "         AND ScoreMeters IN " );
				curSqlStmt.Append( "             (SELECT MAX(ScoreMeters) AS BestScore FROM JumpScore AS SS3 " );
				curSqlStmt.Append( "              WHERE SS3.MemberId = SS2.MemberId AND SS3.SanctionId = SS2.SanctionId AND SS3.AgeGroup = SS2.AgeGroup AND SS3.Round < 25 " );
				curSqlStmt.Append( "              ) ) " );
				curSqlStmt.Append( "ORDER BY ER.EventGroup, SS.NopsScore DESC, TR.SkierName " );
			} else if ( inDataType.ToLower().Equals( "final" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "first" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "round" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "h2h" ) ) {
				curSqlStmt.Append( "ORDER BY RunOrderGroup ASC, TR.SkierName, SS.Round " );
			} else if ( inDataType.ToLower().Equals( "total" ) ) {
				if ( HelperFunctions.isCollegiateEvent(inRules) ) {
					curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
				} else {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass" );
					curSqlStmt.Append( ", Sum(SS.ScoreFeet) as ScoreFeet, Sum(SS.ScoreMeters) as ScoreMeters, SUM(SS.NopsScore) AS NopsScore, 0 AS Round, 'N/A' AS Rating " );
					curSqlStmt.Append( ", COALESCE(DV.RunOrder, 999) as DivOrder " );
					curSqlStmt.Append( "FROM TourReg TR " );
					curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
					curSqlStmt.Append( "  LEFT OUTER JOIN JumpScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round < 25 " );
					curSqlStmt.Append( "  LEFT OUTER JOIN EventRunOrder O ON O.SanctionId = TR.SanctionId AND O.MemberId = TR.MemberId AND O.AgeGroup = TR.AgeGroup AND O.Event = ER.Event AND O.Round = SS.Round " );
					curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
					curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Jump' " );
					if ( curEventGroup.Length > 0 ) {
						curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
					}
					if ( curDiv.Length > 0 ) {
						curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
					}
					curSqlStmt.Append( "GROUP BY TR.MemberId, TR.SanctionId, TR.SkierName, ER.Event, ER.AgeGroup, ER.EventGroup, ER.TeamCode, ER.EventClass, DV.RunOrder " );
				}
			} else if ( inDataType.ToLower().Equals( "all" ) ) {
				curSqlStmt.Append( "ORDER BY ER.EventGroup, TR.SkierName, SS.Round " );
			}

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getJumpDetailData( String inSanctionId, String inEventGroup, String inDiv ) {
			String curEventGroup = "";
			if ( inEventGroup == null ) {
				curEventGroup = "";
			} else if ( inEventGroup.Length > 0 ) {
				if ( inEventGroup.ToLower().Equals( "all" ) || inEventGroup.ToLower().Equals( "team" ) ) {
					curEventGroup = "";
				} else {
					curEventGroup = inEventGroup;
				}
			} else {
				curEventGroup = "";
			}
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SS.MemberId, SS.SanctionId, TR.SkierName, ER.Event, SS.BoatSpeed, SS.RampHeight" );
			curSqlStmt.Append( ", ER.AgeGroup,  ER.EventGroup, SS.Round, SS.ScoreFeet, SS.ScoreMeters, SS.NopsScore, SS.Rating" );
			curSqlStmt.Append( ", COALESCE(SS.EventClass, ER.EventClass) as EventClass, COALESCE(DV.RunOrder, 999) as DivOrder" );
			curSqlStmt.Append( ", COALESCE(TR.ReadyForPlcmt, 'N') as ReadyForPlcmt, COALESCE(ER.ReadyForPlcmt, 'N') as ReadyForPlcmtJump" );
			curSqlStmt.Append( ", ER.TeamCode, ER.EventClass, '' as BoatCode " );
			curSqlStmt.Append( "FROM JumpScore AS SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg as TR ON  SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg as ER ON SS.SanctionId = ER.SanctionId AND SS.MemberId = ER.MemberId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "  LEFT OUTER JOIN DivOrder DV ON DV.SanctionId = ER.SanctionId AND DV.AgeGroup = ER.AgeGroup AND DV.Event = ER.Event " );
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Jump' " );
			if ( curEventGroup.Length > 0 ) {
				curSqlStmt.Append( "  And ER.EventGroup = '" + curEventGroup + "' " );
			}
			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And ER.AgeGroup = '" + curDiv + "' " );
			}
			curSqlStmt.Append( " ORDER BY SS.SanctionId, ER.AgeGroup, TR.SkierName, SS.MemberId, SS.Round" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getOverallDetailData( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( " SELECT T.MemberId, T.SanctionId, T.SkierName, S.Round AS Round, M.Class, T.AgeGroup, " );
			curSqlStmt.Append( " S.NopsScore AS Slalom, K.NopsScore AS Trick, J.NopsScore AS Jump, " );
			curSqlStmt.Append( " S.NopsScore + K.NopsScore + J.NopsScore AS Score" );
			curSqlStmt.Append( " FROM TourReg AS T " );
			curSqlStmt.Append( "   INNER JOIN Tournament AS M ON M.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "   INNER JOIN SlalomScore AS S ON T.SanctionId = S.SanctionId AND T.MemberId = S.MemberId AND T.AgeGroup = S.AgeGroup " );
			curSqlStmt.Append( "   INNER JOIN TrickScore AS K ON T.SanctionId = K.SanctionId AND T.MemberId = K.MemberId AND T.AgeGroup = K.AgeGroup AND S.Round = K.Round " );
			curSqlStmt.Append( "   INNER JOIN JumpScore AS J ON T.SanctionId = J.SanctionId AND T.MemberId = J.MemberId AND T.AgeGroup = J.AgeGroup AND S.Round = J.Round " );
			curSqlStmt.Append( " WHERE (T.SanctionId = '" + inSanctionId + "')" );
			curSqlStmt.Append( " ORDER BY T.SanctionId, T.AgeGroup, T.SkierName, S.MemberId, S.Round" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getOverallSkierList( String inSanctionId ) {
			int curSkiYear = Convert.ToInt32( inSanctionId.Substring( 0, 2 ) );
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT R.AgeGroup, R.MemberId, CAST(L.MinValue AS INT) AS EventsReqd, COUNT(*) AS NumSkierEvents " );
			curSqlStmt.Append( "FROM TourReg AS R " );
			curSqlStmt.Append( "INNER JOIN EventReg AS E ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN CodeValueList AS L ON L.ListCode = E.AgeGroup AND ListName = 'OverallEventsReqd' " );
			curSqlStmt.Append( String.Format("WHERE E.SanctionId = '{0}' And R.ReadyForPlcmt = 'Y' ", inSanctionId) );
			curSqlStmt.Append( "GROUP BY R.AgeGroup, R.MemberId, L.MinValue " );
			curSqlStmt.Append( "HAVING COUNT(*) >= L.MinValue " );
			curSqlStmt.Append( "ORDER BY R.AgeGroup, R.MemberId, L.MinValue" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getTeamSkierList( String inSanctionId, String inEvent ) {
			int curSkiYear = Convert.ToInt32( inSanctionId.Substring( 0, 2 ) );
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT R.AgeGroup, R.MemberId " );
			curSqlStmt.Append( "FROM TourReg AS R " );
			curSqlStmt.Append( "  INNER JOIN EventReg AS E ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN TeamList T ON T.TeamCode = E.TeamCode " );
			curSqlStmt.Append( "WHERE E.SanctionId = '" + inSanctionId + "' AND E.Event = '" + inEvent + "' " );
			curSqlStmt.Append( "ORDER BY R.AgeGroup, R.MemberId " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			return curDataTable;
		}

		public DataTable getEventRegData( String inSanctionId, String inEvent ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.EventGroup, E.RunOrder, E.TeamCode" );
			curSqlStmt.Append( ", E.EventClass, E.RankingScore, E.RankingRating, E.AgeGroup, E.HCapBase, E.HCapScore, '1-TBD' as Status " );
			curSqlStmt.Append( " FROM EventReg E INNER JOIN " );
			curSqlStmt.Append( " TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId" );
			curSqlStmt.Append( " WHERE E.SanctionId = '" + inSanctionId + "' AND E.Event = '" + inEvent + "'" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getTeamData( String inSanctionId, String inPlcmtOrg, String inEvent, String inRules ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );

			if ( inPlcmtOrg.ToLower().Equals( "div" ) ) {
				curSqlStmt.Append( "SELECT T.TeamCode, O.AgeGroup AS Div, T.Name, T.ContactName, T.ContactInfo" );
				curSqlStmt.Append( ", COALESCE(D.RunOrder, 999) as DivOrder" );
				if ( inEvent.Equals( "Trick" ) ) {
					curSqlStmt.Append( ", O.TrickRunOrder as RunOrder " );
				} else if ( inEvent.Equals( "Jump" ) ) {
					curSqlStmt.Append( ", O.JumpRunOrder as RunOrder " );
				} else {
					curSqlStmt.Append( ", O.SlalomRunOrder as RunOrder " );
				}
				curSqlStmt.Append( "FROM TeamList T " );
				curSqlStmt.Append( "INNER JOIN TeamOrder O ON O.SanctionId = T.SanctionId AND O.TeamCode = T.TeamCode " );
				curSqlStmt.Append( "LEFT OUTER JOIN DivOrder AS D ON D.SanctionId = T.SanctionId AND D.AgeGroup = O.AgeGroup AND D.Event = '" + inEvent + "' " );
				curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
				curSqlStmt.Append( "Order by O.AgeGroup, T.TeamCode " );
			} else if ( inPlcmtOrg.ToLower().Equals( "group" ) ) {
				curSqlStmt.Append( "SELECT T.TeamCode, O.AgeGroup AS Div, T.Name, T.ContactName, T.ContactInfo" );
				curSqlStmt.Append( ", 1 as DivOrder" );
				if ( inEvent.Equals( "Trick" ) ) {
					curSqlStmt.Append( ", O.TrickRunOrder as RunOrder " );
				} else if ( inEvent.Equals( "Jump" ) ) {
					curSqlStmt.Append( ", O.JumpRunOrder as RunOrder " );
				} else {
					curSqlStmt.Append( ", O.SlalomRunOrder as RunOrder " );
				}
				curSqlStmt.Append( "FROM TeamList T " );
				curSqlStmt.Append( "INNER JOIN TeamOrder O ON O.SanctionId = T.SanctionId AND O.TeamCode = T.TeamCode " );
				curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
				curSqlStmt.Append( "Order by O.AgeGroup, T.TeamCode " );

			} else {
				curSqlStmt.Append( "SELECT Distinct T.TeamCode, '' as Div, T.Name, T.ContactName, T.ContactInfo, 1 as RunOrder, 1 as DivOrder " );
				curSqlStmt.Append( "FROM TeamList T " );
				curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
				curSqlStmt.Append( "Order by T.TeamCode " );
			}

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		public DataTable getMemberData( String inSanctionId ) {
			return getMemberData( inSanctionId, "All" );
		}
		public DataTable getMemberData( String inSanctionId, String inDiv ) {
			String curDiv = "";
			if ( inDiv == null ) {
				curDiv = "";
			} else if ( inDiv.Length > 0 ) {
				if ( inDiv.ToLower().Equals( "all" ) ) {
					curDiv = "";
				} else {
					curDiv = inDiv;
				}
			} else {
				curDiv = "";
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT T.SanctionId, Federation, T.MemberId, SkierName" );
			curSqlStmt.Append( ", Gender, SkiYearAge, City, State, COALESCE(ReadyToSki, 'N') as ReadyToSki, COALESCE(T.ReadyForPlcmt, 'N') as ReadyForPlcmt, L.CodeValue AS Region" );
			curSqlStmt.Append( ", RS.TeamCode AS SlalomTeam, RT.TeamCode AS TrickTeam, RJ.TeamCode AS JumpTeam" );
			curSqlStmt.Append( ", RS.EventGroup AS SlalomGroup, RT.EventGroup AS TrickGroup, RJ.EventGroup AS JumpGroup" );
			curSqlStmt.Append( ", RS.EventClass AS SlalomClass, RT.EventClass AS TrickClass, RJ.EventClass AS JumpClass" );
			curSqlStmt.Append( ", T.AgeGroup as AgeGroup, RS.AgeGroup AS SlalomAgeGroup, RT.AgeGroup AS TrickAgeGroup, RJ.AgeGroup AS JumpAgeGroup " );
			curSqlStmt.Append( ", COALESCE(RS.ReadyForPlcmt, 'N') as ReadyForPlcmtSlalom" );
			curSqlStmt.Append( ", COALESCE(RT.ReadyForPlcmt, 'N') as ReadyForPlcmtTrick" );
			curSqlStmt.Append( ", COALESCE(RJ.ReadyForPlcmt, 'N') as ReadyForPlcmtJump " );
			curSqlStmt.Append( "FROM TourReg AS T " );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventReg AS RS ON T.SanctionId = RS.SanctionId AND T.MemberId = RS.MemberId AND T.AgeGroup = RS.AgeGroup AND RS.Event = 'Slalom'" );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventReg AS RT ON T.SanctionId = RT.SanctionId AND T.MemberId = RT.MemberId AND T.AgeGroup = RT.AgeGroup AND RT.Event = 'Trick'" );
			curSqlStmt.Append( "  LEFT OUTER JOIN EventReg AS RJ ON T.SanctionId = RJ.SanctionId AND T.MemberId = RJ.MemberId AND T.AgeGroup = RJ.AgeGroup AND RJ.Event = 'Jump'" );
			curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList AS L ON ListName = 'StateRegion' AND ListCode = State " );
			curSqlStmt.Append( "WHERE (T.SanctionId = '" + inSanctionId + "')" );

			if ( curDiv.Length > 0 ) {
				curSqlStmt.Append( "  And T.AgeGroup = '" + curDiv + "' " );
			}
			curSqlStmt.Append( "ORDER BY T.AgeGroup, SkierName" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		public DataTable getIncompleteSkiers( String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SS.SanctionId, SS.MemberId, TR.SkierName, ER.AgeGroup, ER.Event, SS.Round, Status " );
			curSqlStmt.Append( "FROM SlalomScore SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Slalom' AND SS.Round < 25 " );
			curSqlStmt.Append( "  And Status in ('InProg', 'Error') " );
			curSqlStmt.Append( "Union " );
			curSqlStmt.Append( "SELECT SS.SanctionId, SS.MemberId, TR.SkierName, ER.AgeGroup, ER.Event, SS.Round, Status " );
			curSqlStmt.Append( "FROM TrickScore SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Trick' AND SS.Round < 25 " );
			curSqlStmt.Append( "  And Status in ('InProg', 'Error') " );
			curSqlStmt.Append( "Union " );
			curSqlStmt.Append( "SELECT SS.SanctionId, SS.MemberId, TR.SkierName, ER.AgeGroup, ER.Event, SS.Round, Status " );
			curSqlStmt.Append( "FROM JumpScore SS " );
			curSqlStmt.Append( "  INNER JOIN TourReg TR ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON SS.MemberId = ER.MemberId AND SS.SanctionId = ER.SanctionId AND SS.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "WHERE SS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Jump' AND SS.Round < 25 " );
			curSqlStmt.Append( "  And Status in ('InProg', 'Error') " );
			curSqlStmt.Append( "Union " );
			curSqlStmt.Append( "SELECT Distinct TR.SanctionId, TR.MemberId, TR.SkierName, ER.AgeGroup, '' as Event, 0 as Round, 'Not Ready' " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + inSanctionId + "' " );
			curSqlStmt.Append( "  And ReadyToSki != 'Y' " );
			curSqlStmt.Append( "ORDER BY SS.SanctionId, SS.MemberId, TR.SkierName, ER.AgeGroup, ER.Event, SS.Round" );

			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		private short checkForEventScore( String inEvent, String inSanctionId, String inMemberId, int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round " );
			curSqlStmt.Append( "FROM " + inEvent + "Score " );
			curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' " );
			curSqlStmt.Append( "  AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( "Order by Round" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			int returnValue = 0;
			if ( curDataTable.Rows.Count > 0 ) {
				for ( int idx = 0; idx < curDataTable.Rows.Count; idx++ ) {
					DataRow curRow = curDataTable.Rows[idx];
					returnValue = idx + 1;
					if ( inRound == Convert.ToInt16( (byte)curRow["Round"] ) ) return Convert.ToInt16( returnValue );
				}
			}
			return (short)returnValue;
		}

		private DataTable getEliteSkiersInAgeGroup( String inEvent, String inSanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select SkierName, TR.SanctionId, TR.MemberId, TR.AgeGroup, RankingRating, RankingScore" );
			curSqlStmt.Append( ", Event, SS.AgeGroup as EliteDiv, SS.EventClass, SS.Round, SS.NopsScore" );
			if ( inEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", SS.Score, SS.MaxSpeed, SS.FinalSpeedMph, SS.FinalSpeedKph, SS.FinalPassScore, SS.CompletedSpeedMph, SS.CompletedSpeedKph" );
				curSqlStmt.Append( ", SS.FinalLen, SS.FinalLenOff, SS.StartSpeed, SS.StartLen" );
			} else if ( inEvent.Equals( "Trick" ) ) {
				curSqlStmt.Append( ", SS.Score, SS.ScorePass1, SS.ScorePass2" );
			} else if ( inEvent.Equals( "Jump" ) ) {
				curSqlStmt.Append( ", SS.ScoreFeet, SS.ScoreMeters, SS.BoatSpeed, SS.RampHeight" );
			}
			curSqlStmt.Append( " From EventReg ER " );
			curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = ER.SanctionId And TR.MemberId = ER.MemberId And ER.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "Inner Join " + inEvent + "Score SS ON SS.SanctionId = TR.SanctionId And SS.MemberId = TR.MemberId " );
			curSqlStmt.Append( "Where Event = '" + inEvent + "' " );
			curSqlStmt.Append( "And TR.SanctionId = '" + inSanctionId + "' " );
			curSqlStmt.Append( "And RankingRating in ('OM', 'OW', 'MM', 'MW') " );
			curSqlStmt.Append( "And TR.AgeGroup not in ('OM', 'OW', 'MM', 'MW') " );
			curSqlStmt.Append( "And SS.AgeGroup in ('OM', 'OW', 'MM', 'MW') " );
			curSqlStmt.Append( "Order By SkierName, TR.MemberId, TR.AgeGroup, SS.Round " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataRow getEventMemberEntry( DataTable inDataTable, String inMemberId ) {
			if ( inDataTable == null ) return null;
			if ( inDataTable.Rows.Count > 0 ) {
				DataRow[] curRowList = inDataTable.Select( "MemberId = '" + inMemberId + "'" );
				if ( curRowList.Length > 0 ) return curRowList[0];
			}
			return null;
		}

		private DataRow getEventMemberEntry( DataTable inDataTable, String inMemberId, int inRound ) {
			if ( inDataTable == null ) return null;
			if ( inDataTable.Rows.Count > 0 ) {
				DataRow[] curRowList = inDataTable.Select( "MemberId = '" + inMemberId + "' AND Round = " + inRound.ToString() );
				if ( curRowList.Length > 0 ) return curRowList[0];
			}
			return null;
		}

		private DataRow getEventMemberEntry( DataTable inDataTable, String inMemberId, String inDiv ) {
            if ( inDataTable == null ) return null;
			if ( inDataTable.Rows.Count > 0 ) {
				DataRow[] curRowList = inDataTable.Select( "MemberId = '" + inMemberId + "' AND SUBSTRING(AgeGroup, 1, 2) = '" + inDiv + "'" );
				if ( curRowList.Length > 0 ) return curRowList[0];
			}

			return null;
        }

        private DataRow getEventMemberEntry( DataTable inDataTable, String inMemberId, String inDiv, int inRound ) {
            if ( inDataTable == null ) return null;
			if ( inDataTable.Rows.Count > 0 ) {
				DataRow[] curRowList = inDataTable.Select( "MemberId = '" + inMemberId + "' AND SUBSTRING(AgeGroup, 1, 2) = '" + inDiv + "' AND Round = " + inRound.ToString() );
				if ( curRowList.Length > 0 ) return curRowList[0];
			}

			return null;
		}

		private Boolean isSkierEligNcwsaJumpScore( String inSanctionId, String inMemberId, String inDiv, Int16 inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, Round, PassNum, Reride, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, " );
			curSqlStmt.Append( " BoatSplitTime, BoatSplitTime2, BoatEndTime, TimeInTol, ScoreProt, ReturnToBase, Results " );
			curSqlStmt.Append( " FROM JumpRecap" );
			curSqlStmt.Append( " WHERE SanctionId = '" + inSanctionId + "' " );
			curSqlStmt.Append( " AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( " AND AgeGroup = '" + inDiv + "' " );
			curSqlStmt.Append( " AND Round = " + inRound + " " );
			curSqlStmt.Append( " AND Results in ('Jump', 'Fall' ) " );
			curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, ScoreMeters DESC" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return true;
			} else {
				return false;
			}

		}

		private byte getSlalomSpeedMph( byte inSpeedKph ) {
            byte curReturnValue = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MinValue, MaxValue" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'SlalomSpeeds' AND MaxValue = " + inSpeedKph );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                curReturnValue = Convert.ToByte( ( (Decimal)curDataTable.Rows[0]["MinValue"] ).ToString( "00" ) );
            }

            return curReturnValue;
        }

        private DataTable getIwwfOverallAdj() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, MinValue, CodeDesc" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'IwwfOverallAdj'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

    }
}
