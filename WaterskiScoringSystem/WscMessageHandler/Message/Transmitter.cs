using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

using WscMessageHandler.Common;

using SocketIOClient;
using SocketIOClient.Transport;

namespace WscMessageHandler.Message {
	class Transmitter {
		private static String mySanctionNum = "";
		private static String myEventSubId = "";

		private static String myLastConnectResponse = "";
		private static SocketIO socketClient = null;

		private static DataRow myTourRow = null;
		private static System.Timers.Timer heartBeatTimer = null;
		private static System.Timers.Timer readMessagesTimer = null;

		public static void startWscTransmitter( String sanctionNum, String eventSubId ) {
			String curMethodName = "Transmitter: startWscTransmitter: ";
			mySanctionNum = sanctionNum;
			myEventSubId = eventSubId;
			Log.WriteFile( String.Format( "{0}start in progress, {1}/{2}", curMethodName, mySanctionNum, myEventSubId ) );

			myTourRow = HelperFunctions.getTourData();
			if ( myTourRow == null ) {
				String curMsg = String.Format( "{0}Tournament data for {1} was not found, terminating attempt to starting listener", curMethodName, mySanctionNum );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return;
			}

			try {
				/*
				 * Documentation of options
				 * https://github.com/doghappy/socket.io-client-csharp#options
				 * EIO 4 Default 
				 * if your server is using socket.io server v2.x, please explicitly set it to 3
				 */
				socketClient = new SocketIO( ConnectDialog.WscWebLocation, new SocketIOOptions {
					Transport = TransportProtocol.WebSocket
					, Reconnection = true
					, ReconnectionDelay = 1000
					, ReconnectionAttempts = 25
					, EIO = 3
				} );

				clientListeners();
				
				Log.WriteFile( String.Format( "{0}ClientListeners started, begin monitoring for messages from {1}", curMethodName, ConnectDialog.WscWebLocation ) );
				socketClient.ConnectAsync();
				Log.WriteFile( String.Format( "{0}ConnectAsync", curMethodName ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );

				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void checkMonitorHeartBeat( object sender, EventArgs e ) {
			socketClient.EmitAsync( "connectedapplication_check", "" );
			Log.WriteFile( "Transmitter: checkMonitorHeartBeat: Emit connectedapplication_check" );
		}
		private static void readSendMessages( object sender, EventArgs e ) {
			int returnMsg = checkForMsgToSend();
			if ( returnMsg > 0 ) return;
		}

		private static void clientListeners() {
			String curMethodName = "Transmitter: clientListeners: ";
			Log.WriteFile( String.Format( "{0}", curMethodName ) );

			try {
				socketClient.OnConnected += handleSocketOnConnected;

				socketClient.OnPing += handleSocketOnPing;

				socketClient.OnDisconnected += handleSocketOnDisconnected;

				socketClient.OnReconnectAttempt += handleSocketOnReconnecting;

				socketClient.On( "connect_confirm", ( response ) => {
					handleWscConnectConfirm( "connect_confirm", response.GetValue<string>() );
				} );

				socketClient.On( "connectedapplication_check", ( response ) => {
					handleConnectHeartBeat( "connectedapplication_check", response.GetValue<string>() );
				} );

				/*
				socketClient.OnPong += handleSocketOnPong;

				socketClient.OnAny( ( name, response ) => {
					Log.WriteFile( String.Format( "{0}Default SocketClient callback handler: Name: {1}, Data: {2}", curMethodName, name, response == null ? "Null" : response.GetValue<string>() ) );
				} );
				 */

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );

				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void handleSocketOnConnected( object sender, EventArgs argData ) {
			String curMethodName = "Transmitter: handleSocketOnConnected: ";
			Log.WriteFile( String.Format( "{0}Socket.Id: {1}, Connected: {2}, ServerUri: {3}, argData={4}"
				, curMethodName, socketClient.Id, socketClient.Connected, socketClient.ServerUri, argData == null ? "Null" : argData.ToString() ) );

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
			socketClient.EmitAsync( "manual_connection_parameter", jsonData );
			Log.WriteFile( String.Format( "{0}Submitting manual server request for monitoring connection: {1}", curMethodName, jsonData ) );
		}

		private static void handleSocketOnPing( object sender, EventArgs argData ) {
			if ( socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Transmitter" );
			//String curMethodName = "Transmitter: handleSocketOnPing: ";
			//Log.WriteFile( String.Format( "{0}, Connected:{1}, argData={2}", curMethodName, socketClient.Connected, argData == null ? "Null" : argData.ToString() ) );
		}

		private static void handleSocketOnReconnecting( object sender, int argData ) {
			String curMethodName = "Transmitter: handleSocketOnReconnecting: ";
			Log.WriteFile( String.Format( "{0}Attempt: {1}", curMethodName, argData ) );
		}

		private static void handleSocketOnDisconnected( object sender, String argData ) {
			String curMethodName = "Transmitter: handleSocketOnDisconnected: ";
			Log.WriteFile( String.Format( "{0}argData: {1}", curMethodName, argData ) );
		}

		private static void handleWscConnectConfirm(String txnName, String argData ) {
			String curMethodName = "Transmitter: handleWscConnectConfirm: ";
			myLastConnectResponse = argData;
			Log.WriteFile( String.Format( "{0}{1} {2}", curMethodName, txnName, argData ) );
			HelperFunctions.addMsgListenQueue( "connect_confirm_transmitter", myLastConnectResponse );
			HelperFunctions.updateMonitorHeartBeat( "Transmitter" );

			// Create a timer with 5 minute interval.
			heartBeatTimer = new System.Timers.Timer( 300000 );
			heartBeatTimer.Elapsed += checkMonitorHeartBeat;
			heartBeatTimer.AutoReset = true;
			heartBeatTimer.Enabled = true;

			// Create a timer with 2 second interval.
			readMessagesTimer = new System.Timers.Timer( 2000 );
			readMessagesTimer.Elapsed += readSendMessages;
			readMessagesTimer.AutoReset = true;
			readMessagesTimer.Enabled = true;
		}

		private static void handleConnectHeartBeat( String txnName, String argData ) {
			String curMethodName = "Transmitter: handleConnectHeartBeat: ";
			if ( socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Transmitter" );
			Log.WriteFile( String.Format( "{0}{1}, Connected: {2}, Msg: {3}", curMethodName, txnName, socketClient.Connected, argData ) );
		}

		private static int checkForMsgToSend() {
			String curMethodName = "Transmitter: checkForMsgToSend: ";

			try {
				DataTable curDataTable = getWscMsgSend();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					if ( ( (String)curDataRow["MsgType"] ).Equals( "Exit" ) ) {
						return disconnect( (int)curDataRow["PK"] );
					}

					socketClient.EmitAsync( (String)curDataRow["MsgType"], curDataRow["MsgData"] );
					Log.WriteFile( String.Format( "{0}PK: {1} MsgType: {2} MsgData: {3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] ) );
					removeWscMsgSent( (int)curDataRow["PK"] );
				}
				return 0;

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );
				return -1;
			}
		}

		public static int disconnect( int inMsgPk ) {
			String curMethodName = "Transmitter: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );

			if ( socketClient != null ) {
				Log.WriteFile( curMethodName + "Disconnected" );
				socketClient.DisconnectAsync();
			}

			if ( heartBeatTimer != null ) {
				heartBeatTimer.Stop();
				heartBeatTimer.Elapsed -= checkMonitorHeartBeat;
			}
			if ( readMessagesTimer != null ) {
				readMessagesTimer.Stop();
				readMessagesTimer.Elapsed -= readSendMessages;
			}

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
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}
		
		private static void removeWscMsgSent( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgSend Where PK = " + pkid );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
