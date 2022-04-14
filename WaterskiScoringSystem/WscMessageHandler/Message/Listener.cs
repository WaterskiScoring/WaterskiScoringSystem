using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Newtonsoft.Json;

using WscMessageHandler.Common;

using SocketIOClient;
using SocketIOClient.Transport;

namespace WscMessageHandler.Message {
	class Listener {
		private static String myLastConnectResponse = "";

		public static bool isWscConnected {
			get { return ( ConnectDialog.WscWebLocation.Length > 0 && myLastConnectResponse.Length > 0 ); }
		}

		public static void showPin() {
			MessageBox.Show( myLastConnectResponse );
		}

		public static async void startWscListener() {
			String curMethodName = "Listener: startWscListener: ";
			myLastConnectResponse = "";
			Log.WriteFile( String.Format( "{0}start in progress, {1}/{2}"
				, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId ) );

			try {
				/*
				 * Documentation of options
				 * https://github.com/doghappy/socket.io-client-csharp#options
				 * EIO 4 Default 
				 * if your server is using socket.io server v2.x, please explicitly set it to 3
				 */
				ConnectMgmtData.socketClient = new SocketIO( ConnectDialog.WscWebLocation, new SocketIOOptions {
					Transport = TransportProtocol.WebSocket
					, Reconnection = true
					, ReconnectionDelay = 1000
					, ReconnectionAttempts = 25
					, EIO = 3
				} );

				clientListeners();
				
				Log.WriteFile( String.Format( "{0}ClientListeners started, begin monitoring for messages from {1}"
					, curMethodName, ConnectDialog.WscWebLocation ) );
				await ConnectMgmtData.socketClient.ConnectAsync();
				Log.WriteFile( String.Format( "{0}ConnectAsync", curMethodName ) );
				
			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );

				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";
				ConnectMgmtData.eventSubId = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void clientListeners() {
			String curMethodName = "Listener: clientListeners: ";
			Log.WriteFile( String.Format( "{0}Enter", curMethodName ) );

			try {
				ConnectMgmtData.socketClient.OnConnected += ( sender, response ) => {
					Log.WriteFile( String.Format( "{0}Socket.Id: {1}, Connected: {2}, ServerUri: {3}, response={4}"
						, curMethodName, ConnectMgmtData.socketClient.Id, ConnectMgmtData.socketClient.Connected
						, ConnectMgmtData.socketClient.ServerUri
						, response == null ? "Null" : response.ToString() ) );
					handleSocketOnConnected( sender, response );
				};

				ConnectMgmtData.socketClient.OnPing += handleSocketOnPing;

				ConnectMgmtData.socketClient.OnDisconnected += handleSocketOnDisconnected;

				ConnectMgmtData.socketClient.OnReconnectAttempt += handleSocketOnReconnecting;

				ConnectMgmtData.socketClient.On( "connect_confirm", ( response ) => {
					Log.WriteFile( String.Format( "{0}connect_confirm", curMethodName ) );
					handleWscConnectConfirm( "connect_confirm", response.GetValue<string>() );
				} );

				ConnectMgmtData.socketClient.On( "heartbeat", ( response ) => {
					handleConnectHeartBeat( "heartbeat", response.GetValue<string>() );
				} );

				ConnectMgmtData.socketClient.On( "boat_times", response => {
					HelperFunctions.addMsgListenQueue( "boat_times", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}boat_times {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "boatpath_data", ( response ) => {
					HelperFunctions.addMsgListenQueue( "boatpath_data", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}boatpath_data {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "scoring_result", ( response ) => {
					HelperFunctions.addMsgListenQueue( "scoring_result", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}scoring_result {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "trickscoring_detail", ( response ) => {
					HelperFunctions.addMsgListenQueue( "trickscoring_detail", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}trickscoring_detail {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "jumpmeasurement_score", ( response ) => {
					HelperFunctions.addMsgListenQueue( "jumpmeasurement_score", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}jumpmeasurement_score {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "status_response", ( response ) => {
					checkConnectStatus( response.GetValue<string>() );
				} );

				/*
				ConnectMgmtData.socketClient.On( "connectedapplication_response", ( data ) => {
					Log.WriteFile( String.Format( "{0}connectedapplication_response {1}", curMethodName, response.GetValue<string>() ) );
				} );

				ConnectMgmtData.socketClient.On( "connectedapplication_check", ( data ) => {
					HelperFunctions.addMsgListenQueue( "connectedapplication_check", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}connectedapplication_check {1}", curMethodName, response.GetValue<string>() ) );
					/ *
					Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
						{ "application", "WSTIMS" }
					};
					String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
					ConnectMgmtData.socketClient.Emit( "connectedapplication_response", jsonData );
					 * /
				} );
				
				ConnectMgmtData.socketClient.OnPong += handleSocketOnPong;

				ConnectMgmtData.socketClient.OnAny( ( name, response ) => {
					Log.WriteFile( String.Format( "{0}Default ConnectMgmtData.socketClient callback handler: Name: {1}, Data: {2}", curMethodName, name, response == null ? "Null" : response.GetValue<string>() ) );
				} );

				*/
				Log.WriteFile( String.Format( "{0}Exit", curMethodName ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );

				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void handleSocketOnConnected( object sender, EventArgs argData ) {
			String curMethodName = "Listener: handleSocketOnConnected: ";
			Log.WriteFile( String.Format( "{0}Enter", curMethodName ) );
			
			connectWscServer();
		}

		private static void connectWscServer() {
			String curMethodName = "Listener: connectWscServer: ";
			Log.WriteFile( String.Format( "{0}Enter", curMethodName ) );
			
			try {
				Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", ConnectMgmtData.sanctionNum }
					, { "eventsubid", ConnectMgmtData.eventSubId }
					, { "provider", "Mass Water Ski Association" }
					, { "application", "WSTIMS Listener" }
					, { "version", "2.0" }
					, { "username", "mawsa@comcast.net" }
					, { "Application_key", ConnectMgmtData.wscApplicationKey }
				};
				String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
				ConnectMgmtData.socketClient.EmitAsync( "manual_connection_parameter", jsonData );
				Log.WriteFile( String.Format( "{0}Connection manual server request for monitoring connection: {1}", curMethodName, jsonData ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void handleSocketOnPing( object sender, EventArgs argData ) {
			String curMethodName = "Listener: handleSocketOnPing: ";
			try {
				if ( ConnectMgmtData.socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Listener" );
				//Log.WriteFile( String.Format( "{0}, Connected:{1}, argData={2}", curMethodName, ConnectMgmtData.socketClient.Connected, argData == null ? "Null" : argData.ToString() ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void handleSocketOnReconnecting( object sender, int argData ) {
			String curMethodName = "Listener: handleSocketOnReconnecting: ";
			try {
				Log.WriteFile( String.Format( "{0}Attempt: {1}", curMethodName, argData ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void handleSocketOnDisconnected( object sender, String argData ) {
			String curMethodName = "Listener: handleSocketOnDisconnected: ";
			try {
				Log.WriteFile( String.Format( "{0}argData: {1}", curMethodName, argData ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void handleWscConnectConfirm( String txnName, String argData ) {
			String curMethodName = "Listener: handleWscConnectConfirm: ";
			Log.WriteFile( String.Format( "{0}Enter", curMethodName ) );
			try {
				myLastConnectResponse = argData;
				Log.WriteFile( String.Format( "{0}{1} {2}", curMethodName, txnName, argData ) );
				HelperFunctions.addMsgListenQueue( "connect_confirm_listener", myLastConnectResponse );
				HelperFunctions.updateMonitorHeartBeat( "Listener" );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void handleConnectHeartBeat( String txnName, String argData ) {
			String curMethodName = "Listener: handleConnectHeartBeat: ";
			try {
				HelperFunctions.addMsgListenQueue( "heartbeat", argData );
				if ( ConnectMgmtData.socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Listener" );
				Log.WriteFile( String.Format( "{0}{1}, Connected: {2}, Msg: {3}", curMethodName, txnName, ConnectMgmtData.socketClient.Connected, argData ) );

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void checkConnectStatus( String msg ) {
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = ConnectMgmtData.socketClient.JsonSerializer.Deserialize<Dictionary<string, object>>( msg );
				String curEventId = HelperFunctions.getAttributeValue( curMsgDataList, "eventId" );

				if ( curEventId.Equals( ConnectMgmtData.sanctionNum ) ) {
					if ( ConnectMgmtData.eventSubId.Length > 0 ) return;

					if ( !( HelperFunctions.getAttributeValue( curMsgDataList, "eventSubId" ).Equals( ConnectMgmtData.eventSubId ) ) ) {
						ConnectDialog.WscWebLocation = "";
						myLastConnectResponse = "";
						ConnectMgmtData.eventSubId = "";
						MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
					}

				} else {
					ConnectDialog.WscWebLocation = "";
					myLastConnectResponse = "";
					ConnectMgmtData.eventSubId = "";
					MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
				}

			} catch ( Exception ex ) {
				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";
				ConnectMgmtData.eventSubId = "";
				MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect: " + ex.Message );
			}
		}

		public static int disconnect() {
			String curMethodName = "Listener: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );

			if ( ConnectMgmtData.socketClient != null && isWscConnected ) {
				Log.WriteFile( curMethodName + "disconnected" );
				ConnectMgmtData.socketClient.DisconnectAsync();
			}

			ConnectDialog.WscWebLocation = "";
			myLastConnectResponse = "";
			HelperFunctions.deleteMonitorHeartBeat( "Listener" );
			Log.WriteFile( curMethodName + "EXIT request processed and all threads closed" );

			return 1;
		}
	}
}
