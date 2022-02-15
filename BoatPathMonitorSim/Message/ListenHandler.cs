using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

using Newtonsoft.Json;

using BoatPathMonitorSim.Common;

namespace BoatPathMonitorSim.Message {
	class ListenHandler {
		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static bool myUseJumpTimes = false;
		private static readonly int myDefaultRound = 1;
		private static bool myListenHandlerActive = false;

		private static DataRow myTourRow = null;
		private static System.Timers.Timer readProcessTimer;

		public static Boolean isConnectConfirmed {
			get { return myListenHandlerActive; }
			set { myListenHandlerActive = value; }
		}

		public static void startMessageHandler( String sanctionNum, String eventSubId, bool useJumpTimes ) {
			String curMethodName = "ListenHandler:startMessageHandler: ";
			mySanctionNum = sanctionNum;
			myEventSubId = eventSubId;
			myUseJumpTimes = useJumpTimes;
			myListenHandlerActive = false;
			Log.WriteFile( String.Format( curMethodName + "Sanction: {0}, myEventSubId: {1}, myUseJumpTimes={2}", mySanctionNum, myEventSubId, myUseJumpTimes ) );

			myTourRow = HelperFunctions.getTourData();
			if ( myTourRow == null ) {
				String curMsg = curMethodName + String.Format( "Tournament data for {0} was not found, terminating attempt to starting listener", mySanctionNum );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return;
			}

			HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
			myListenHandlerActive = true;

			// Create a timer with 2 second interval.
			readProcessTimer = new System.Timers.Timer( 2000 );
			readProcessTimer.Elapsed += readProcessMessages;
			readProcessTimer.AutoReset = true;
			readProcessTimer.Enabled = true;
		}

		private static void readProcessMessages( object sender, EventArgs e ) {
			int returnMsg = checkForMsgToHandler();
			if ( returnMsg > 0 ) return;
			if ( mySanctionNum.Length == 0 ) return;
		}

		public static int disconnect() {
			String curMethodName = "ListenHandler:disconnect: ";
			
			readProcessTimer.Stop();
			readProcessTimer.Elapsed -= readProcessMessages;
			
			myEventSubId = "";
			mySanctionNum = "";
			myTourRow = null;
			myListenHandlerActive = false;
			HelperFunctions.deleteMonitorHeartBeat( "ListenHandler" );
			Log.WriteFile( String.Format( "{0}Disconnection request processed", curMethodName ) );
			return 1;
		}

		private static int checkForMsgToHandler() {
			String curMethodName = "ListenHandler: checkForMsgToHandler:  ";
			String msgType = "", msgData = "", msgDataTemp = "";
			try {
				DataTable curDataTable = getWscMsgReceived();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					msgType = (String)curDataRow["MsgType"];
					msgDataTemp = (String)curDataRow["MsgData"];
					if ( msgDataTemp.Length > 4 && msgDataTemp.StartsWith("[" ) ) {
						msgData = msgDataTemp.Substring( 2, msgDataTemp.Length - 4 );
					} else {
						msgData = msgDataTemp;
					}
					if ( msgType.Equals( "connect_confirm_listener" ) ) {
						connectConfirm( msgType, msgData );

					} else if ( msgType.Equals( "connect_confirm_transmitter" ) ) {
						connectConfirm( msgType, msgData );

					} else if ( msgType.Equals( "connectedapplication_check" ) ) {
						HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
						Log.WriteFile( String.Format( "{0}connectedapplication_check", curMethodName ) );
					
					} else if ( msgType.Equals( "pass_data" ) ) {
						sendBoatPathData( msgType, msgData );
						sendBoatTimeData( msgType, msgData );
						sendJumpDistances( msgType, msgData );

					} else {
						showMsg( msgType, msgData );
					}

					removeWscMsgHandled( (int)curDataRow["PK"] );
				}
				return 0;

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Exception encountered " + ex.Message );
				return -1;
			}
		}

		private static void connectConfirm( String msgType, String msg ) {
			String curMethodName = "ListenHandler: connectConfirm:  ";
			Log.WriteFile( String.Format( "{0}{1} {2}", curMethodName, msgType, msg ) );
			HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
		}

		private static void showMsg( String msgType, String msg ) {
			MessageBox.Show( "msgType: " + msgType + "\nMsg: " + msg );
		}

		private static DataTable getWscMsgReceived() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( "FROM WscMsgListen " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "Order by CreateDate " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static void removeWscMsgHandled( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgListen Where PK = " + pkid );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
		private static void sendBoatPathData( String msgType, String msg ) {
			String curMethodName = "ListenHandler: sendBoatPathData:  ";
			String curMsg = msg.Replace( "\\", "" );
			Log.WriteFile( String.Format( "{0} {1} {2}", curMethodName, msgType, curMsg ) );

			Dictionary<string, object> curMsgDataList = null;
			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( curMsg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from BoatPath P " );
			curSqlStmt.Append( "INNER JOIN TourReg R ON R.SanctionId = P.SanctionId AND R.MemberId = P.MemberId " );
			curSqlStmt.Append( "Where P.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( " AND P.Event = '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" ) + "' " );
			curSqlStmt.Append( " AND P.MemberId = '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "' " );
			curSqlStmt.Append( " AND P.round = " + HelperFunctions.getAttributeValue( curMsgDataList, "round" ) + " " );
			curSqlStmt.Append( " AND P.passNumber = " + HelperFunctions.getAttributeValue( curMsgDataList, "passNumber" ) + " " );
			curSqlStmt.Append( "Order by InsertDate DESC " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count == 0 ) return;

			DataRow curDataRow = curDataTable.Rows[0];
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
				{ "athleteId", (String)curDataRow["MemberId"] }
				, { "athleteName", (String)curDataRow["SkierName"] }
				, { "athleteEvent", (String)curDataRow["Event"] }
				, { "round", (byte)curDataRow["Round"] }
				, { "passNumber", (byte)curDataRow["PassNumber"] }
				, { "speed", (byte)curDataRow["PassSpeedKph"] }
				, { "rope", (Decimal)curDataRow["PassLineLength"] }
				, { "driverId", (String)curDataRow["DriverMemberId"] }
				, { "driverName", (String)curDataRow["DriverName"] }
				};

			if ( ( (String)curDataRow["Event"] ).Equals( "Slalom" ) ) {
				buildBoatPathDataSlalom( curDataRow, sendMsg );
				addWscMsgSend( "boatpath_data", JsonConvert.SerializeObject( sendMsg ) );

			} else if ( ( (String)curDataRow["Event"] ).Equals( "Jump" ) ) {
				buildBoatPathDataJump( curDataRow, sendMsg );
				addWscMsgSend( "boatpath_data", JsonConvert.SerializeObject( sendMsg ) );

			} else {
				Log.WriteFile( String.Format( "{0} Invalid event {1} value encountered", curMethodName, (String)curDataRow["Event"] ) );
			}
		}

		private static void buildBoatPathDataSlalom( DataRow curDataRow, Dictionary<string, dynamic> sendMsg ) {
			Dictionary<string, object> curBuoyResults = null;
			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy0"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum0"] }
				};
			sendMsg.Add( "gate", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy1"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum1"] }
				};
			sendMsg.Add( "b1", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy2"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum2"] }
				};
			sendMsg.Add( "b2", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy3"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum3"] }
				};
			sendMsg.Add( "b3", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy4"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum4"] }
				};
			sendMsg.Add( "b4", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy5"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum5"] }
				};
			sendMsg.Add( "b5", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy6"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum6"] }
				};
			sendMsg.Add( "b6", curBuoyResults );
		}
		
		private static void buildBoatPathDataJump( DataRow curDataRow, Dictionary<string, dynamic> sendMsg ) {
			Dictionary<string, object> curBuoyResults = null;
			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy0"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum0"] }
				};
			sendMsg.Add( "start", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy1"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum1"] }
				};
			sendMsg.Add( "st", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy2"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum2"] }
				};
			sendMsg.Add( "nt", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy3"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum3"] }
				};
			sendMsg.Add( "mt", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy4"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum4"] }
				};
			sendMsg.Add( "et", curBuoyResults );

			curBuoyResults = new Dictionary<string, dynamic> {
					{ "deviation", (Decimal)curDataRow["PathDevBuoy5"] }
					, { "cumulative", (Decimal)curDataRow["PathDevCum5"] }
				};
			sendMsg.Add( "ec", curBuoyResults );
		}

		private static void sendBoatTimeData( String msgType, String msg ) {
			String curMethodName = "ListenHandler: sendBoatTimeData:  ";
			String curMsg = msg.Replace( "\\", "" );
			Log.WriteFile( String.Format( "{0} {1} {2}", curMethodName, msgType, curMsg ) );

			Dictionary<string, object> curMsgDataList = null;
			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( curMsg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from BoatTime P " );
			curSqlStmt.Append( "INNER JOIN TourReg R ON R.SanctionId = P.SanctionId AND R.MemberId = P.MemberId " );
			curSqlStmt.Append( "Where P.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( " AND P.Event = 'Slalom' " );
			curSqlStmt.Append( " AND P.MemberId = '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "' " );
			curSqlStmt.Append( " AND P.round = " + HelperFunctions.getAttributeValue( curMsgDataList, "round" ) + " " );
			curSqlStmt.Append( " AND P.passNumber = " + HelperFunctions.getAttributeValue( curMsgDataList, "passNumber" ) + " " );
			curSqlStmt.Append( "Order by InsertDate DESC " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count == 0 ) return;

			DataRow curDataRow = curDataTable.Rows[0];
			if ( ( (String)curDataRow["Event"] ).Equals( "Slalom" ) ) {
				Dictionary<string, dynamic> sendMsg = buildBoatTimeDataSlalom( curDataRow );
				addWscMsgSend( "boat_times", JsonConvert.SerializeObject( sendMsg ) );

			} else if ( ( (String)curDataRow["Event"] ).Equals( "Jump" ) ) {
				Dictionary<string, dynamic> sendMsg = buildBoatTimeDataJump( curDataRow );
				addWscMsgSend( "boat_times", JsonConvert.SerializeObject( sendMsg ) );

			} else {
				Log.WriteFile( String.Format( "{0} Invalid event {1} value encountered", curMethodName, (String)curDataRow["Event"] ) );
			}
		}
		
		private static Dictionary<string, dynamic> buildBoatTimeDataSlalom( DataRow curDataRow ) {
			return new Dictionary<string, dynamic> {
				{ "athleteId", (String)curDataRow["MemberId"] }
				, { "athleteName", (String)curDataRow["SkierName"] }
				, { "athleteEvent", (String)curDataRow["Event"] }
				, { "round", (byte)curDataRow["Round"] }
				, { "passNumber", (byte)curDataRow["PassNumber"] }
				, { "speed", (byte)curDataRow["PassSpeedKph"] }
				, { "rope", (Decimal)curDataRow["PassLineLength"] }
				, { "b1", (Decimal)curDataRow["BoatTimeBuoy1"] }
				, { "b2", (Decimal)curDataRow["BoatTimeBuoy2"] }
				, { "b3", (Decimal)curDataRow["BoatTimeBuoy3"] }
				, { "b4", (Decimal)curDataRow["BoatTimeBuoy4"] }
				, { "b5", (Decimal)curDataRow["BoatTimeBuoy5"] }
				, { "b6", (Decimal)curDataRow["BoatTimeBuoy6"] }
				, { "endgate", (Decimal)curDataRow["BoatTimeBuoy7"] }
				};
		}
		
		private static Dictionary<string, dynamic> buildBoatTimeDataJump( DataRow curDataRow ) {
			return new Dictionary<string, dynamic> {
				{ "athleteId", (String)curDataRow["MemberId"] }
				, { "athleteName", (String)curDataRow["SkierName"] }
				, { "athleteEvent", (String)curDataRow["Event"] }
				, { "round", (byte)curDataRow["Round"] }
				, { "passNumber", (byte)curDataRow["PassNumber"] }
				, { "speed", (byte)curDataRow["PassSpeedKph"] }
				, { "rope", (Decimal)curDataRow["PassLineLength"] }
				, { "nt", (Decimal)curDataRow["BoatTimeBuoy1"] }
				, { "mt", (Decimal)curDataRow["BoatTimeBuoy2"] }
				, { "et", (Decimal)curDataRow["BoatTimeBuoy3"] }
				};
		}

		private static void sendJumpDistances( String msgType, String msg ) {
			String curMethodName = "ListenHandler: sendJumpDistances:  ";
			String curMsg = msg.Replace( "\\", "" );
			Log.WriteFile( String.Format( "{0} {1} {2}", curMethodName, msgType, curMsg ) );

			Dictionary<string, object> curMsgDataList = null;
			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( curMsg );
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );
				if ( !(curEvent.Equals( "Jump" )) ) return;

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}
			
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from JumpMeasurement P " );
			curSqlStmt.Append( "INNER JOIN TourReg R ON R.SanctionId = P.SanctionId AND R.MemberId = P.MemberId " );
			curSqlStmt.Append( "Where P.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( " AND P.Event = 'Jump' " );
			curSqlStmt.Append( " AND P.MemberId = '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "' " );
			curSqlStmt.Append( " AND P.round = " + HelperFunctions.getAttributeValue( curMsgDataList, "round" ) + " " );
			curSqlStmt.Append( " AND P.passNumber = " + HelperFunctions.getAttributeValue( curMsgDataList, "passNumber" ) + " " );
			curSqlStmt.Append( "Order by InsertDate DESC " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count == 0 ) return;

			DataRow curDataRow = curDataTable.Rows[0];
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
				{ "athleteId", (String)curDataRow["MemberId"] }
				, { "athleteName", (String)curDataRow["SkierName"] }
				, { "athleteEvent", (String)curDataRow["Event"] }
				, { "round", (byte)curDataRow["Round"] }
				, { "passNumber", (byte)curDataRow["PassNumber"] }
				};
			Dictionary<string, object> curScoreResults = null;
			curScoreResults = new Dictionary<string, dynamic> {
					{ "distanceMetres", (Decimal)curDataRow["ScoreMeters"] }
					, { "distanceFeet", (Decimal)curDataRow["ScoreFeet"] }
				};
			sendMsg.Add( "score", curScoreResults );
			addWscMsgSend( "jumpmeasurement_score", JsonConvert.SerializeObject( sendMsg ) );
		}

		private static void addWscMsgSend( String msgType, String msgData ) {
			String curMsgData = HelperFunctions.stringReplace( msgData, HelperFunctions.singleQuoteDelim, "''" );

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert WscMsgSend ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( "'" + mySanctionNum + "'" );
			curSqlStmt.Append( ", '" + msgType + "'" );
			curSqlStmt.Append( ", '" + curMsgData + "'" );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

	}
}
