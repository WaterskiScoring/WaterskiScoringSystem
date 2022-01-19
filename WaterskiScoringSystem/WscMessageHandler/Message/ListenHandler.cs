using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using System.Windows.Forms;

using System.Web.Script.Serialization;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	class ListenHandler {
		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static bool myUseJumpTimes = false;
		private static int myDefaultRound = 1;
		private static bool myListenHandlerActive = false;

		private static DataRow myTourRow = null;
		private static System.Timers.Timer readProcessTimer;

		public static Boolean isConnectConfirmed {
			get { return myListenHandlerActive; }
			set { myListenHandlerActive = value; }
		}

		public static void startWscMessageHandler( String sanctionNum, String eventSubId, bool useJumpTimes ) {
			String curMethodName = "ListenHandler:startWscMessageHandler: ";
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
			readProcessTimer.Stop();
			readProcessTimer.Elapsed += readProcessMessages;
			
			myEventSubId = "";
			mySanctionNum = "";
			myTourRow = null;
			myListenHandlerActive = false;
			HelperFunctions.deleteMonitorHeartBeat( "ListenHandler" );
			return 1;
		}

		private static int checkForMsgToHandler() {
			String curMethodName = "ListenHandler: checkForMsgToHandler:  ";
			String msgType = "", msgData = "";
			try {
				DataTable curDataTable = getWscMsgReceived();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					msgType = (String)curDataRow["MsgType"];
					msgData = (String)curDataRow["MsgData"];
					if ( msgType.Equals( "connect_confirm_listener" ) ) {
						connectConfirm( msgType, msgData );

					} else if ( msgType.Equals( "connect_confirm_transmitter" ) ) {
						connectConfirm( msgType, msgData );

					} else if ( msgType.Equals( "connectedapplication_check" ) ) {
						updateMonitorHeartBeat();

					} else if ( msgType.Equals( "boat_times" ) ) {
						saveBoatTimes( msgData );

					} else if ( msgType.Equals( "boatpath_data" ) ) {
						saveBoatPath( msgData );

					} else if ( msgType.Equals( "jumpmeasurement_score" ) ) {
						saveJumpMeasurement( msgData );

					} else if ( msgType.Equals( "trickscoring_detail" ) ) {
						showMsg( "scoring_result", msgData );

					} else if ( msgType.Equals( "scoring_result" ) ) {
						showMsg( "scoring_result", msgData );

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
			Log.WriteFile( String.Format( "{0} connect_confirm {1} {2}", curMethodName, msgType, msg ) );
		}

		private static void updateMonitorHeartBeat() {
			HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
			HelperFunctions.updateMonitorHeartBeat( "Listener" );
			HelperFunctions.updateMonitorHeartBeat( "Transmitter" );
		}

		private static void saveBoatTimes( String msg ) {
			String curMethodName = "ListenHandler: saveBoatTimes:  ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			try {
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = myDefaultRound;
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );

				curSqlStmt.Append( "Insert BoatTime ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event" );
				curSqlStmt.Append( ", Round, PassNumber, PassLineLength, PassSpeedKph" );
				curSqlStmt.Append( ", BoatTimeBuoy1, BoatTimeBuoy2, BoatTimeBuoy3,BoatTimeBuoy4, BoatTimeBuoy5, BoatTimeBuoy6, BoatTimeBuoy7" );
				curSqlStmt.Append( ", InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" ) + "'" );

				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "rope" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "speed" ).ToString( "#0" ) );

				if ( curEvent.Equals( "Jump" ) ) {
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "nt" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "mt" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "et" ) );
					curSqlStmt.Append( ", 0, 0, 0, 0 " );

				} else if ( curEvent.Equals( "Slalom" ) ) {
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b1" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b2" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b3" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b4" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b5" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "b6" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "endgate" ) );
				}

				curSqlStmt.Append( ", getdate(), getdate()" );
				curSqlStmt.Append( " )" );
				int rowsProc = HandlerDataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
		}

		private static void saveBoatPath( String msg ) {
			String curMethodName = "ListenHandler:saveBoatPath: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			StringBuilder curBoatInfo = new StringBuilder( "" );
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			try {
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = myDefaultRound;
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );

				curBoatInfo.Append( HelperFunctions.getAttributeValue( curMsgDataList, "boatManufacturer" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatModel" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatYear" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatColour" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatComment" ) );
				String curBoatInfoMsg = HelperFunctions.stringReplace( curBoatInfo.ToString(), HelperFunctions.singleQuoteDelim, "''" );

				curSqlStmt.Append( "Insert BoatPath ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event, DriverMemberId, DriverName, BoatDescription" );
				curSqlStmt.Append( ", Round, PassNumber, PassLineLength, PassSpeedKph" );
				curSqlStmt.Append( ", PathDevBuoy0, PathDevCum0, PathDevZone0" );
				curSqlStmt.Append( ", PathDevBuoy1, PathDevCum1, PathDevZone1" );
				curSqlStmt.Append( ", PathDevBuoy2, PathDevCum2, PathDevZone2" );
				curSqlStmt.Append( ", PathDevBuoy3, PathDevCum3, PathDevZone3" );
				curSqlStmt.Append( ", PathDevBuoy4, PathDevCum4, PathDevZone4" );
				curSqlStmt.Append( ", PathDevBuoy5, PathDevCum5, PathDevZone5" );
				curSqlStmt.Append( ", PathDevBuoy6, PathDevCum6, PathDevZone6" );
				curSqlStmt.Append( ", RerideNote, InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" ) + "'" );

				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "driverId" ) + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "driverName" ) + "'" );
				curSqlStmt.Append( ", '" + curBoatInfoMsg + "'" );

				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "rope" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "speed" ).ToString( "#0" ) );

				if ( curEvent.Equals( "Jump" ) ) {
					curSqlStmt.Append( getJumpBoatPathAttributes( curMsgDataList ) );

				} else if ( curEvent.Equals( "Slalom" ) ) {
					curSqlStmt.Append( getSlalomBoatPathAttributes( curMsgDataList ) );
				}

				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "reride" ) + "'" );
				curSqlStmt.Append( ", getdate(), getdate()" );

				curSqlStmt.Append( " )" );
				int rowsProc = HandlerDataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
		}

		private static String getJumpBoatPathAttributes( Dictionary<string, object> curMsgDataList ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Dictionary<string, object> curBuoyResults = null;

			// Start of deviation tracking 600 foot (180 meter) buoy
			// 180,ST,NT,MT,ET and EC is good for me as points

			// Start of jump course timing, entry gates
			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "start" );
			if ( curBuoyResults == null ) curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "180" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "st" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 52 meter timing segment, only used for E, L, R events
			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "nt" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 82 meter timing segment, mid course timing gates
			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "mt" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 41 meter timing segment, end course timing gates
			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "et" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// End of oourse and end of deviation tracking, also referred to as ride out buoys
			curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "ec" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curSqlStmt.Append( ", 0, 0, 0" );

			return curSqlStmt.ToString();
		}

		private static String getSlalomBoatPathAttributes( Dictionary<string, object> msgAttributeList ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Dictionary<string, object> curBuoyResults = null;

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "gate" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b1" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b2" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b3" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b4" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b5" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = HelperFunctions.getAttributeList( msgAttributeList, "b6" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			return curSqlStmt.ToString();
		}

		private static void saveJumpMeasurement( String msg ) {
			String curMethodName = "ListenHandler:saveJumpMeasurement: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			StringBuilder curBoatInfo = new StringBuilder( "" );
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			try {
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = myDefaultRound;

				curSqlStmt.Append( "Insert JumpMeasurement ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event, Round, PassNumber, ScoreFeet, ScoreMeters, InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );

				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" ) + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" ) + "'" );

				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );

				Dictionary<string, object> curBuoyResults = HelperFunctions.getAttributeList( curMsgDataList, "score" );
				if ( curBuoyResults != null ) {
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "distanceFeet" ) );
					curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curBuoyResults, "distanceMetres" ) );

				} else {
					curSqlStmt.Append( ", 0, 0" );
				}

				curSqlStmt.Append( ", getdate(), getdate()" );
				curSqlStmt.Append( " )" );
				int rowsProc = HandlerDataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
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
			return HandlerDataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static void removeWscMsgHandled( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgListen Where PK = " + pkid );
			int rowsProc = HandlerDataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
