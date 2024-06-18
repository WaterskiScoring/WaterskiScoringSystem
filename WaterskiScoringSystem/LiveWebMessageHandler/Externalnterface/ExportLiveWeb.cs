using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;

using LiveWebMessageHandler.Common;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace LiveWebMessageHandler.Externalnterface {
    class ExportLiveWeb {
		//public static String LiveWebScoreboardUri = "/import/api/ImportScores";
		public static String LiveWebScoreboardUri = Properties.Settings.Default.UriWaterskiResultsApi + "/ImportScores";
		private static StringBuilder myLastErrorMsg = new StringBuilder( "" );
		private static String myModuleName = "ExportLiveWeb: ";

		public static string LastErrorMsg { get => myLastErrorMsg.ToString(); set => myLastErrorMsg.Append( value ); }

		public static void exportTourData( String inSanctionId ) {
			String curMethodName = myModuleName + "exportTourData: ";
			myLastErrorMsg = new StringBuilder( "" );
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM Tournament " );
			curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
			curTableList.Add( exportTableWithData( "Tournament", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "SELECT Distinct SanctionId FROM TourProperties Where SanctionId = '{0}'", inSanctionId ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "SELECT Distinct SanctionId FROM OfficialWork Where SanctionId = '{0}'", inSanctionId ) );
			curTableList.Add( exportTableWithData( "OfficialWork", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "SELECT Distinct SanctionId FROM DivOrder Where SanctionId = '{0}'", inSanctionId ) );
			curTableList.Add( exportTableWithData( "DivOrder", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourProperties " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' ", inSanctionId ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId", "PropKey" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "SELECT * FROM DivOrder Where SanctionId = '{0}' ", inSanctionId ) );
			curTableList.Add( exportTableWithData( "DivOrder", new String[] { "SanctionId", "Event", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( String.Format( "SELECT * FROM OfficialWork Where SanctionId = '{0}' ", inSanctionId ) );
			curSqlStmt.Append( "AND (" );
			curSqlStmt.Append( "JudgeChief = 'Y' OR JudgeAsstChief = 'Y' OR JudgeAppointed = 'Y' " );
			curSqlStmt.Append( "OR DriverChief = 'Y' OR DriverAsstChief = 'Y' OR DriverAppointed = 'Y' " );
			curSqlStmt.Append( "OR ScoreChief = 'Y' OR ScoreAsstChief = 'Y' OR ScoreAppointed = 'Y' " );
			curSqlStmt.Append( "OR SafetyChief = 'Y' OR SafetyAsstChief = 'Y' OR SafetyAppointed = 'Y' " );
			curSqlStmt.Append( "OR TechChief = 'Y' OR TechAsstChief = 'Y' " );
			curSqlStmt.Append( ") " );
			curTableList.Add( exportTableWithData( "OfficialWork", new String[] { "SanctionId", "MemberId" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.* FROM TourReg TR " );
			curSqlStmt.Append( "INNER JOIN OfficialWork OW on OW.SanctionId = TR.SanctionId AND OW.MemberID = TR.MemberId " );
			curSqlStmt.Append( String.Format( "Where TR.SanctionId = '{0}' ", inSanctionId ) );
			curSqlStmt.Append( "AND (" );
			curSqlStmt.Append( "JudgeChief = 'Y' OR JudgeAsstChief = 'Y' OR JudgeAppointed = 'Y' " );
			curSqlStmt.Append( "OR DriverChief = 'Y' OR DriverAsstChief = 'Y' OR DriverAppointed = 'Y' " );
			curSqlStmt.Append( "OR ScoreChief = 'Y' OR ScoreAsstChief = 'Y' OR ScoreAppointed = 'Y' " );
			curSqlStmt.Append( "OR SafetyChief = 'Y' OR SafetyAsstChief = 'Y' OR SafetyAppointed = 'Y' " );
			curSqlStmt.Append( "OR TechChief = 'Y' OR TechAsstChief = 'Y' " );
			curSqlStmt.Append( ") " );
			curTableList.Add( exportTableWithData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		public static void exportRunningOrder( String inSanctionId, String inEvent, String inEventGroup, int inRound ) {
			String curMethodName = "ExportLiveWeb: exportRunningOrder: ";
			String curGroupFilter;
            StringBuilder curSqlStmt = new StringBuilder( "" );
			myLastErrorMsg = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT TR.* FROM TourReg TR " );
			curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND TR.AgeGroup = ER.AgeGroup " );
			curSqlStmt.Append( String.Format( "Where TR.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
			curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, false ) );
			curTableList.Add( exportTableWithData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

            curSqlStmt = new StringBuilder( "" );
			if (inEventGroup.ToLower().Equals( "all" ) ) {
                curSqlStmt.Append( "SELECT Distinct SanctionId, ER.Event FROM EventReg ER " );
                curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
                curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "Event" }, curSqlStmt.ToString(), "Delete" ) );
			
			} else if ( inSanctionId.Substring( 2, 1 ) == "U" ) {
                curSqlStmt.Append( "SELECT ER.SanctionId, ER.Event, ER.AgeGroup, ER.EventGroup FROM EventReg ER " );
                curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
                curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, false ) );
                curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "Event", "AgeGroup" }, curSqlStmt.ToString(), "Delete" ) );
			
			} else {
                curSqlStmt.Append( "SELECT ER.SanctionId, ER.Event, ER.AgeGroup, ER.EventGroup FROM EventReg ER " );
                curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
                curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, false ) );
                curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "Event", "EventGroup" }, curSqlStmt.ToString(), "Delete" ) );
			}

			curSqlStmt = new StringBuilder( "" );
			if ( inEvent.ToLower().Equals( "all" ) ) {
                curSqlStmt.Append( "SELECT Distinct SanctionId, ER.Event FROM EventRunOrder ER " );
                curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
                curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "Event" }, curSqlStmt.ToString(), "Delete" ) );
			
			} else {
                curSqlStmt.Append( "SELECT Distinct SanctionId, ER.Event, ER.EventGroup FROM EventRunOrder ER " );
                curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
                curGroupFilter = HelperFunctions.getEventGroupFilterSql( inEventGroup, false, true );
                curSqlStmt.Append( curGroupFilter.Replace( "O.", "ER." ) );
                curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "Event", "EventGroup" }, curSqlStmt.ToString(), "Delete" ) );
			}

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM EventReg ER " );
			curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
			curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, false ) );
			curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM EventRunOrder ER " );
			curSqlStmt.Append( String.Format( "Where ER.SanctionId = '{0}' AND ER.Event = '{1}' ", inSanctionId, inEvent ) );
			curGroupFilter = HelperFunctions.getEventGroupFilterSql( inEventGroup, false, true );
			curSqlStmt.Append( curGroupFilter.Replace( "O.", "ER." ) );
			curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourProperties " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND PropKey like '%{1}%' ", inSanctionId, inEvent ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId", "PropKey" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM EventRunOrderFilters " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' ", inSanctionId, inEvent ) );
			curTableList.Add( exportTableWithData( "EventRunOrderFilters", new String[] { "SanctionId", "Event", "FilterName" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM DivOrder " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' ", inSanctionId, inEvent ) );
			curTableList.Add( exportTableWithData( "DivOrder", new String[] { "SanctionId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		public static void exportTeamScores( String inSanctionId ) {
			String curMethodName = "ExportLiveWeb: exportTeamScores: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			myLastErrorMsg = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt.Append( "SELECT TR.* FROM TeamScore TR " );
			curSqlStmt.Append( String.Format( "Where TR.SanctionId = '{0}' ", inSanctionId ) );
			curTableList.Add( exportTableWithData( "TeamScore", new String[] { "SanctionId", "TeamCode", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.* FROM TeamScoreDetail TR " );
			curSqlStmt.Append( String.Format( "Where TR.SanctionId = '{0}' ", inSanctionId ) );
			curTableList.Add( exportTableWithData( "TeamScoreDetail", new String[] { "SanctionId", "TeamCode", "AgeGroup", "LineNum" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		public static void exportCurrentSkiers( String inEvent, String inSanctionId, byte inRound, String inEventGroup ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkiers: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			myLastErrorMsg = new StringBuilder( "" );
			int curLineCount = 0;
			myLastErrorMsg = new StringBuilder( "" );
			DataTable curDataTable = new DataTable();

			try {
				if ( inEvent.Equals( "TrickVideo" ) ) {
					curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
					curSqlStmt.Append( "From TrickVideo S " );
					curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId AND ER.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
					curSqlStmt.Append( "AND ER.Event = 'Trick' " );
					if ( inRound > 0 ) curSqlStmt.Append( "AND S.Round = " + inRound + " " );
					curSqlStmt.Append( "AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1) " );
					curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, true, false ) );
					curSqlStmt.Append( " Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId" );
					curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), false );

				} else {
					curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
					curSqlStmt.Append( "From " + inEvent + "Score S " );
					curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId AND ER.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "LEFT OUTER JOIN EventRunOrder O ON ER.SanctionId = O.SanctionId AND ER.MemberId = O.MemberId AND ER.AgeGroup = O.AgeGroup AND ER.Event = O.Event AND S.Round = O.Round " );
					curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
					curSqlStmt.Append( "AND ER.Event = '" + inEvent + "' " );
					curSqlStmt.Append( "AND S.Round = " + inRound + " " );
					curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, true ) );
					curSqlStmt.Append( "Order by S.SanctionId, S.Round, ER.EventGroup, S.MemberId, S.AgeGroup" );
					curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), false );
				}

				if ( curDataTable == null ) {
					if ( HelperFunctions.isObjectPopulated( DataAccess.LastDataAccessErrorMsg ) ) LastErrorMsg = DataAccess.LastDataAccessErrorMsg;
					return;
				}

				String curMemberId = "", curAgeGroup = "", curSkierName = "";
				byte curRound = 0;
				foreach ( DataRow curRow in curDataTable.Rows ) {
					curMemberId = (String)curRow["MemberId"];
					curAgeGroup = (String)curRow["AgeGroup"];
					curSkierName = (String)curRow["SkierName"];
					curRound = (Byte)curRow["Round"];

					curLineCount++;

					try {
						if ( inEvent.Equals( "Slalom" ) ) {
							exportCurrentSkierSlalom( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
							continue;

						} else if ( inEvent.Equals( "Trick" ) ) {
							exportCurrentSkierTrick( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
							continue;

						} else if ( inEvent.Equals( "Jump" ) ) {
							exportCurrentSkierJump( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
							continue;

						} else if ( inEvent.Equals( "TrickVideo" ) ) {
							exportCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
							continue;
						}

						String curErrMsg = String.Format( "Error encountered sending {0} scores for {1} {2}", inEvent, curAgeGroup, curSkierName );
						Log.WriteFile( curErrMsg );

					} catch ( Exception ex ) {
						String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
						Log.WriteFile( curErrMsg );
						throw new Exception( curErrMsg );
					}
				}

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				throw new Exception( curErrMsg );
			}
		}

		public static void exportCurrentSkier( String inSanctionId, String inEvent, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalom: ";
			myLastErrorMsg = new StringBuilder( "" );

			if ( inEvent.Equals( "Slalom" ) ) {
				exportCurrentSkierSlalom( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
				return;

			} else if ( inEvent.Equals( "Trick" ) ) {
				exportCurrentSkierTrick( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
				return;

			} else if ( inEvent.Equals( "Jump" ) ) {
				exportCurrentSkierJump( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
				return;
			
			} else if ( inEvent.Equals( "TrickVideo" ) ) {
				exportCurrentSkierTrickVideo( inSanctionId, inMemberId, inAgeGroup, inRound );
				return;
			}

			String curErrMsg = String.Format( "{0}Invalid event {1} encountered", curMethodName, inEvent );
			Log.WriteFile( curErrMsg );
			throw new Exception( curErrMsg );
		}

		private static void exportCurrentSkierSlalom( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalom: ";
			if ( inSkierRunNum <= 1 ) {
				exportCurrentSkierSlalomFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
				return;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt.Append( "SELECT * FROM SlalomScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inSkierRunNum > 0 ) curSqlStmt.Append( " And SkierRunNum = " + inSkierRunNum );
			curSqlStmt.Append( " Order by SkierRunNum" );
			curTableList.Add( exportTableWithData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierSlalomFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalomFirst: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt.Append( "SELECT * FROM TourReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' ", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Event = 'Slalom' ", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventRunOrder " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} And Event = 'Slalom' ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM SlalomScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourProperties " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND PropKey like '%{1}%' ", inSanctionId, "Slalom" ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId", "PropKey" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM SlalomRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM SlalomScore " );
			curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
			curSqlStmt.Append( "And Round = " + inRound );
			curTableList.Add( exportTableWithData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inSkierRunNum > 0 ) curSqlStmt.Append( " And SkierRunNum = " + inSkierRunNum );
			curSqlStmt.Append( " Order by SkierRunNum" );
			curTableList.Add( exportTableWithData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierTrick( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierTrick: ";
			if ( inSkierRunNum <= 1 ) {
				exportCurrentSkierTrickFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
				return;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TrickScore " );
			curSqlStmt.Append( String.Format("Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curSqlStmt.Append( "Order by PassNum" );
			curTableList.Add( exportTableWithData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TrickPass " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inSkierRunNum > 0 ) curSqlStmt.Append( " And Seq = " + inSkierRunNum );
			curSqlStmt.Append( "Order by PassNum, Seq" );
			curTableList.Add( exportTableWithData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierTrickFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierTrickFirst: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' ", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Event = 'Trick'", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventRunOrder " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} And Event = 'Trick' ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourProperties " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND PropKey like '%{1}%' ", inSanctionId, "Trick" ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId", "PropKey" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM TrickScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM TrickPass " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TrickScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TrickPass " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inSkierRunNum > 0 ) curSqlStmt.Append( " And Seq = " + inSkierRunNum );
			curSqlStmt.Append( " Order by PassNum, Seq" );
			curTableList.Add( exportTableWithData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierTrickVideo( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierTrickVideo: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TrickVideo " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierJump( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inPassNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierJump: ";
			if ( inPassNum <= 1 ) {
				exportCurrentSkierJumpFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inPassNum );
				return;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt.Append( "SELECT * FROM JumpScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM JumpRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inPassNum > 0 ) curSqlStmt.Append( " And PassNum = " + inPassNum );
			curSqlStmt.Append( " Order by PassNum" );
			curTableList.Add( exportTableWithData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void exportCurrentSkierJumpFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inPassNum ) {
			String curMethodName = "ExportLiveWeb: exportCurrentSkierJumpFirst: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt.Append( "SELECT * FROM TourReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' ", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Event = 'Jump' ", inSanctionId, inMemberId, inAgeGroup ) );
			curTableList.Add( exportTableWithData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from EventRunOrder " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3} And Event = 'Jump' ", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM TourProperties " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND PropKey like '%{1}%' ", inSanctionId, "Jump" ) );
			curTableList.Add( exportTableWithData( "TourProperties", new String[] { "SanctionId", "PropKey" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM JumpScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM JumpRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM JumpScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT * FROM JumpRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			if ( inPassNum > 0 ) curSqlStmt.Append( " And PassNum = " + inPassNum );
			curSqlStmt.Append( " Order by PassNum" );
			curTableList.Add( exportTableWithData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Update" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		public static void disableSkiers( String inEvent, String inSanctionId, byte inRound, String inEventGroup ) {
			String curMethodName = "ExportLiveWeb: disableSkiers: ";
			int curLineCount = 0;
			myLastErrorMsg = new StringBuilder( "" );
			DataTable curDataTable = new DataTable();
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				if ( inEvent.Equals( "TrickVideo" ) ) {
					curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
					curSqlStmt.Append( "From TrickVideo S " );
					curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId AND ER.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( String.Format( "Where S.SanctionId = '{0}' And ER.Event = '{1}' And S.Round = {2} ", inSanctionId, inEvent, inRound ) );
					curSqlStmt.Append( "AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1) " );
					curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, true, false ) );
					curSqlStmt.Append( " Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId" );
					curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), false );

				} else {
					curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
					curSqlStmt.Append( "From " + inEvent + "Score S " );
					curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId AND ER.AgeGroup = S.AgeGroup " );
					curSqlStmt.Append( "LEFT OUTER JOIN EventRunOrder O ON ER.SanctionId = O.SanctionId AND ER.MemberId = O.MemberId AND ER.AgeGroup = O.AgeGroup AND ER.Event = O.Event AND S.Round = O.Round " );
					curSqlStmt.Append( String.Format( "Where S.SanctionId = '{0}' And ER.Event = '{1}' And S.Round = {2} ", inSanctionId, inEvent, inRound ) );
					curSqlStmt.Append( HelperFunctions.getEventGroupFilterSql( inEventGroup, false, true ) );
					curSqlStmt.Append( "Order by S.SanctionId, S.Round, ER.EventGroup, S.MemberId, S.AgeGroup" );
					curDataTable = DataAccess.getDataTable( curSqlStmt.ToString(), false );
				}
				if ( curDataTable == null ) {
					if ( HelperFunctions.isObjectPopulated( DataAccess.LastDataAccessErrorMsg ) ) LastErrorMsg = DataAccess.LastDataAccessErrorMsg;
					return;
				}

				String curMemberId = "", curAgeGroup = "", curSkierName = "";
				byte curRound = 0;
				foreach ( DataRow curRow in curDataTable.Rows ) {
					curMemberId = (String)curRow["MemberId"];
					curAgeGroup = (String)curRow["AgeGroup"];
					curSkierName = (String)curRow["SkierName"];
					curRound = (Byte)curRow["Round"];

					curLineCount++;

					try {
						if ( inEvent.Equals( "Slalom" ) ) {
							disableCurrentSkierSlalom( inSanctionId, curMemberId, curAgeGroup, curRound );
							continue;

						} else if ( inEvent.Equals( "Trick" ) ) {
							disableCurrentSkierTrick( inSanctionId, curMemberId, curAgeGroup, curRound );
							continue;

						} else if ( inEvent.Equals( "Jump" ) ) {
							disableCurrentSkierJump( inSanctionId, curMemberId, curAgeGroup, curRound );
							continue;

						} else if ( inEvent.Equals( "TrickVideo" ) ) {
							ExportLiveWeb.disableCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
							continue;
						}

						String curErrMsg = String.Format( "Error encountered sending {0} scores for {1} {2}", inEvent, curAgeGroup, curSkierName );
						Log.WriteFile( curErrMsg );

					} catch ( Exception ex ) {
						String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
						Log.WriteFile( curErrMsg );
						throw new Exception( curErrMsg );
					}
				}

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				throw new Exception( curErrMsg );
			}
		}

		public static void disableCurrentSkier( String inSanctionId, String inEvent, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkier: ";
			myLastErrorMsg = new StringBuilder( "" );

			if ( inEvent.Equals( "Slalom" ) ) {
				disableCurrentSkierSlalom( inSanctionId, inMemberId, inAgeGroup, inRound );
				return;

			} else if ( inEvent.Equals( "Trick" ) ) {
				disableCurrentSkierTrick( inSanctionId, inMemberId, inAgeGroup, inRound );
				return;

			} else if ( inEvent.Equals( "Jump" ) ) {
				disableCurrentSkierJump( inSanctionId, inMemberId, inAgeGroup, inRound );
				return;
			}

			String curErrMsg = String.Format( "{0}Invalid event {1} encountered", curMethodName, inEvent );
			Log.WriteFile( curErrMsg );
			throw new Exception( curErrMsg );
		}

		private static void disableCurrentSkierSlalom( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierSlalom: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM SlalomScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM SlalomRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void disableCurrentSkierTrick( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierTrick: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM TrickScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM TrickPass " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void disableCurrentSkierTrickVideo( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierTrickVideo: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM TrickVideo " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static void disableCurrentSkierJump( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierJump: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			ArrayList curTableList = new ArrayList();
			Dictionary<string, dynamic> curLiveWebRequest = null;
			Dictionary<string, dynamic> curTables = null;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM JumpScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM JumpRecap " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' And MemberId = '{1}' And AgeGroup = '{2}' And Round = {3}", inSanctionId, inMemberId, inAgeGroup, inRound ) );
			curTableList.Add( exportTableWithData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

			curTables = new Dictionary<string, object> { { "Tables", curTableList } };
			curLiveWebRequest = new Dictionary<string, object> { { "LiveWebRequest", curTables } };

			String curMsgResp = SendMessageHttp.sendMessagePostJson( LiveWebScoreboardUri, JsonConvert.SerializeObject( curLiveWebRequest ) );
			if ( curMsgResp.Length > 0 ) {
				String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
				Log.WriteFile( curMsg );

			} else {
				String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
				throw new Exception( curMsg );
			}
		}

		private static Dictionary<string, dynamic> exportTableWithData( String inTableName, String[] inKeyColumns, String inSqlStmt, String inCmd ) {
			String curMethodName = myModuleName + "exportData: ";
			char[] singleQuoteDelim = new char[] { '\'' };
			String curValue;
			DataTable curDataTable = new DataTable();
			Dictionary<string, dynamic> curTableRow = null;
			ArrayList curListKyes = new ArrayList();
			ArrayList curListColumns = new ArrayList();
			ArrayList curListRows = new ArrayList();

			curDataTable = DataAccess.getDataTable( inSqlStmt, false );
			if ( curDataTable == null ) {
				if ( HelperFunctions.isObjectPopulated( DataAccess.LastDataAccessErrorMsg ) ) LastErrorMsg = DataAccess.LastDataAccessErrorMsg;
				return null;
			}

			foreach ( String curKey in inKeyColumns ) {
				curListKyes.Add( curKey );
			}

			foreach ( DataColumn curColumn in curDataTable.Columns ) {
				if ( !( curColumn.ColumnName.ToUpper().Equals( "PK" ) ) ) {
					curListColumns.Add( curColumn.ColumnName );
				}
			}

			if ( curDataTable.Rows.Count == 0 && inCmd.ToLower().Equals( "delete" ) ) {
				String[] curDelimValue = { " And " };
				int curDelimPos = inSqlStmt.ToLower().IndexOf( "where" );

				curTableRow = new Dictionary<string, object>();
				String curData = inSqlStmt.Substring( curDelimPos + 6 );
				String[] curDataList = curData.Split( curDelimValue, StringSplitOptions.RemoveEmptyEntries );
				for ( int curIdx = 0; curIdx < curDataList.Length; curIdx++ ) {
					String[] curDataDef = curDataList[curIdx].Split( '=' );
					curValue = curDataDef[1].Trim();
					if ( curValue.Substring( 0, 1 ).Equals( "'" ) ) {
						curTableRow.Add( curDataDef[0].Trim(), curValue.Substring( 1, curValue.Length - 2 ) );
					} else {
						curTableRow.Add( curDataDef[0].Trim(), curValue );
					}
				}
				curListRows.Add( curTableRow );

			} else {
				if ( curDataTable.Rows.Count == 0 ) return null;

				foreach ( DataRow curRow in curDataTable.Rows ) {
					curTableRow = new Dictionary<string, object>();

					foreach ( DataColumn curColumn in curDataTable.Columns ) {
						if ( curColumn.ColumnName.ToUpper().Equals( "PK" ) ) continue;

						if ( curColumn.ColumnName.ToLower().Equals( "lastupdatedate" ) || curColumn.ColumnName.ToLower().Equals( "insertdate" ) ) {
							curValue = HelperFunctions.getDataRowColValue( curRow, curColumn.ColumnName, "" );
							if ( HelperFunctions.isObjectEmpty( curValue ) ) curValue = DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" );
							curTableRow.Add( curColumn.ColumnName, curValue );

						} else {
							curValue = HelperFunctions.stripLineFeedChar( HelperFunctions.getDataRowColValue( curRow, curColumn.ColumnName, "" ) );
							curValue = HelperFunctions.stringReplace( curValue, singleQuoteDelim, "''" );
							//curValue = System.Security.SecurityElement.Escape( curValue );
							curTableRow.Add( curColumn.ColumnName, curValue );
						}
					}
					curListRows.Add( curTableRow );
				}
			}

			return new Dictionary<string, object> {
				{ "name", inTableName }
				, { "command", inCmd }
				, { "Keys", curListKyes }
				, { "Columns", curListColumns }
				, { "Rows", curListRows }
				};

		}

	}
}
