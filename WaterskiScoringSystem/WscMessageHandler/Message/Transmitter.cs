using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using System.Windows.Forms;

//using Quobject.EngineIoClientDotNet;
using Quobject.SocketIoClientDotNet.Client;
//using Quobject.Collections.Immutable;

using Newtonsoft.Json;
//using System.Web.Script.Serialization;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	class Transmitter {
		private static String mySanctionNum = "";
		private static String myEventSubId = "";

		private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;

		private static DataRow myTourRow = null;
		private static System.Timers.Timer heartBeatTimer;
		private static System.Timers.Timer readMessagesTimer;

		public static void startWscTransmitter( String sanctionNum, String eventSubId ) {
			String curMethodName = "Transmitter: startWscTransmitter: ";
			mySanctionNum = sanctionNum;
			myEventSubId = eventSubId;

			myTourRow = HelperFunctions.getTourData();
			if ( myTourRow == null ) {
				String curMsg = curMethodName + String.Format( "Tournament data for {0} was not found, terminating attempt to starting listener", mySanctionNum );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return;
			}

			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", mySanctionNum }
					, { "eventsubid", myEventSubId }
					, { "provider", "Mass Water Ski Association" }
					, { "application", "WSTIMS Transmitter" }
					, { "version", "2.0" }
					, { "username", "mawsa@comcast.net" }
					, { "Application_key", Listener.myWscApplicationKey }
				};
			String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );

			IO.Options connOptions = new IO.Options();
			connOptions.IgnoreServerCertificateValidation = true;
			connOptions.Timeout = 500;
			connOptions.Reconnection = true;
			connOptions.Transports = Quobject.Collections.Immutable.ImmutableList.Create<string>( "websocket" );
			socketClient = IO.Socket( Listener.myWscWebLocation, connOptions );

			socketClient.On( Socket.EVENT_CONNECT, () => {
				Log.WriteFile( String.Format( curMethodName + "Connection submitted for transmitting to connection: {0}", jsonData ) );
				socketClient.Emit( "manual_connection_parameter", jsonData );
			} );

			startClientListeners();

			// Create a timer with 2 minute interval.
			heartBeatTimer = new System.Timers.Timer( 120000 );
			heartBeatTimer.Elapsed += checkMonitorHeartBeat;
			heartBeatTimer.AutoReset = true;
			heartBeatTimer.Enabled = true;

			// Create a timer with 2 second interval.
			readMessagesTimer = new System.Timers.Timer( 2000 );
			readMessagesTimer.Elapsed += readSendMessages;
			readMessagesTimer.AutoReset = true;
			readMessagesTimer.Enabled = true;
		}
		
		private static void checkMonitorHeartBeat( object sender, EventArgs e ) {
			socketClient.Emit( "connectedapplication_check", "" );
			Log.WriteFile( "Transmitter: checkMonitorHeartBeat: Emit connectedapplication_check" );
		}
		private static void readSendMessages( object sender, EventArgs e ) {
			int returnMsg = checkForMsgToSend();
			if ( returnMsg > 0 ) return;
		}

		private static void startClientListeners() {
			String curMethodName = "Transmitter: startClientListeners: ";

			try {
				socketClient.On( "connect_confirm", ( data ) => {
					Log.WriteFile( String.Format( "{0} connect_confirm {1}", curMethodName, data.ToString() ) );
					HelperFunctions.updateMonitorHeartBeat( "Transmitter" );
					HelperFunctions.addMsgListenQueue( "connect_confirm_transmitter", data.ToString() );
				} );
			
			} catch ( Exception ex ) {
				String curMsg = curMethodName + String.Format( "{0} Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );

				myEventSubId = "";
				mySanctionNum = "";
				myTourRow = null;

				MessageBox.Show( curMsg );
			}
		}

		private static int checkForMsgToSend() {
				String curMethodName = "Transmitter: checkForMsgToSend: ";
			try {
				DataTable curDataTable = getWscMsgSend();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					if ( ( (String)curDataRow["MsgType"] ).Equals( "Exit" ) ) {
						return disconnect( (int)curDataRow["PK"] );
					}

					socketClient.Emit( (String)curDataRow["MsgType"], curDataRow["MsgData"] );
					Log.WriteFile( curMethodName + String.Format( "PK: {0} MsgType: {1} MsgData: {2}", curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] ) );
					removeWscMsgSent( (int)curDataRow["PK"] );
				}
				return 0;

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "checkForMsgToSend: Exception encountered " + ex.Message );
				return -1;
			}
		}

		public static int disconnect( int inMsgPk ) {
			String curMethodName = "Transmitter: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );
			
			socketClient.On( Socket.EVENT_DISCONNECT, () => {
				Log.WriteFile( curMethodName + "Disconnected" );
				socketClient.Disconnect();
			} );

			heartBeatTimer.Stop();
			heartBeatTimer.Elapsed -= checkMonitorHeartBeat;
			readMessagesTimer.Stop();
			readMessagesTimer.Elapsed -= readSendMessages;

			myEventSubId = "";
			mySanctionNum = "";
			myTourRow = null;
			if ( inMsgPk > 0 ) removeWscMsgSent( inMsgPk );
			HelperFunctions.deleteMonitorHeartBeat( "Transmitter" );
			Log.WriteFile( curMethodName + "EXIT request processed and all threads closed" );
			return 1;
		}

		private static DataTable getWscMsgSend() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( "FROM WscMsgSend " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "Order by CreateDate " );
			return HandlerDataAccess.getDataTable( curSqlStmt.ToString() );
		}
		
		private static void removeWscMsgSent( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgSend Where PK = " + pkid );
			int rowsProc = HandlerDataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
