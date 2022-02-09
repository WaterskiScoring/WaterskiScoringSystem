using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using Newtonsoft.Json;

using WscMessageHandler.Common;

using SocketIOClient;
using SocketIOClient.Transport;

namespace WscMessageHandler.Message {
	class Listener {
		public static readonly String myWscApplicationKey = "CAD2FB59-3CCB-4691-9D26-7D68C2222788";
		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static String myLastConnectResponse = "";

		private static SocketIO socketClient = null;

		private static DataRow myTourRow = null;

		public static bool isWscConnected {
			get { return ( ConnectDialog.WscWebLocation.Length > 0 && myLastConnectResponse.Length > 0 ); }
		}

		public static void showPin() {
			MessageBox.Show( myLastConnectResponse );
		}

		public static void startWscListener( String sanctionNum, String eventSubId ) {
			String curMethodName = "Listener: startWscListener: ";
			myLastConnectResponse = "";
			mySanctionNum = sanctionNum;
			myEventSubId = eventSubId;
			Log.WriteFile( String.Format( "{0}start in progress, {1}/{2}", curMethodName, mySanctionNum, myEventSubId ) );

			if ( !(DataAccess.DataAccessOpen()) ) {
				String curMsg = curMethodName + String.Format( "Database not found at the specified location {0}", Properties.Settings.Default.DatabaseConnectionString);
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return;
			}

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

		private static void clientListeners() {
			String curMethodName = "Listener: clientListeners: ";

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

				socketClient.On( "boat_times", response => {
					HelperFunctions.addMsgListenQueue( "boat_times", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}boat_times {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "boatpath_data", ( response ) => {
					HelperFunctions.addMsgListenQueue( "boatpath_data", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}boatpath_data {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "scoring_result", ( response ) => {
					HelperFunctions.addMsgListenQueue( "scoring_result", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}scoring_result {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "trickscoring_detail", ( response ) => {
					HelperFunctions.addMsgListenQueue( "trickscoring_detail", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}trickscoring_detail {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "jumpmeasurement_score", ( response ) => {
					HelperFunctions.addMsgListenQueue( "jumpmeasurement_score", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}jumpmeasurement_score {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "status_response", ( response ) => {
					checkConnectStatus( response.GetValue<string>() );
				} );

				/*
				socketClient.On( "connectedapplication_response", ( data ) => {
					Log.WriteFile( String.Format( "{0}connectedapplication_response {1}", curMethodName, response.GetValue<string>() ) );
				} );

				socketClient.On( "connectedapplication_check", ( data ) => {
					HelperFunctions.addMsgListenQueue( "connectedapplication_check", response.GetValue<string>() );
					Log.WriteFile( String.Format( "{0}connectedapplication_check {1}", curMethodName, response.GetValue<string>() ) );
					/ *
					Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
						{ "application", "WSTIMS" }
					};
					String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
					socketClient.Emit( "connectedapplication_response", jsonData );
					 * /
				} );
				
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
			String curMethodName = "Listener: handleSocketOnConnected: ";
			Log.WriteFile( String.Format( "{0}Socket.Id: {1}, Connected: {2}, ServerUri: {3}, argData={4}"
				, curMethodName, socketClient.Id, socketClient.Connected, socketClient.ServerUri, argData == null ? "Null" : argData.ToString() ) );

			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", mySanctionNum }
					, { "eventsubid", myEventSubId }
					, { "provider", "Mass Water Ski Association" }
					, { "application", "WSTIMS Listener" }
					, { "version", "2.0" }
					, { "username", "mawsa@comcast.net" }
					, { "Application_key", Listener.myWscApplicationKey }
				};
			String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
			socketClient.EmitAsync( "manual_connection_parameter", jsonData );
			Log.WriteFile( String.Format( "{0}Connection manual server request for monitoring connection: {1}", curMethodName, jsonData ) );
		}

		private static void handleSocketOnPing( object sender, EventArgs argData ) {
			if ( socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Listener" );
			//String curMethodName = "Listener: handleSocketOnPing: ";
			//Log.WriteFile( String.Format( "{0}, Connected:{1}, argData={2}", curMethodName, socketClient.Connected, argData == null ? "Null" : argData.ToString() ) );
		}

		private static void handleSocketOnReconnecting( object sender, int argData ) {
			String curMethodName = "Listener: handleSocketOnReconnecting: ";
			Log.WriteFile( String.Format( "{0}Attempt: {1}", curMethodName, argData ) );
		}

		private static void handleSocketOnDisconnected( object sender, String argData ) {
			String curMethodName = "Listener: handleSocketOnDisconnected: ";
			Log.WriteFile( String.Format( "{0}argData: {1}", curMethodName, argData ) );
		}

		private static void handleWscConnectConfirm( String txnName, String argData ) {
			String curMethodName = "Listener: handleWscConnectConfirm: ";
			myLastConnectResponse = argData;
			Log.WriteFile( String.Format( "{0}{1} {2}", curMethodName, txnName, argData ) );
			HelperFunctions.addMsgListenQueue( "connect_confirm_listener", myLastConnectResponse );
			HelperFunctions.updateMonitorHeartBeat( "Listener" );
		}

		private static void handleConnectHeartBeat( String txnName, String argData ) {
			String curMethodName = "Listener: handleConnectHeartBeat: ";
			HelperFunctions.addMsgListenQueue( "connectedapplication_check", argData );
			if ( socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Listener" );
			Log.WriteFile( String.Format( "{0}{1}, Connected: {2}, Msg: {3}", curMethodName, txnName, socketClient.Connected, argData ) );
		}

		private static void checkConnectStatus( String msg ) {
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = socketClient.JsonSerializer.Deserialize<Dictionary<string, object>>( msg );
				String curEventId = HelperFunctions.getAttributeValue( curMsgDataList, "eventId" );

				if ( curEventId.Equals( mySanctionNum ) ) {
					if ( myEventSubId.Length > 0 ) return;

					if ( !( HelperFunctions.getAttributeValue( curMsgDataList, "eventSubId" ).Equals( myEventSubId ) ) ) {
						ConnectDialog.WscWebLocation = "";
						myLastConnectResponse = "";
						myEventSubId = "";
						MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
					}

				} else {
					ConnectDialog.WscWebLocation = "";
					myLastConnectResponse = "";
					myEventSubId = "";
					MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
				}

			} catch ( Exception ex ) {
				ConnectDialog.WscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";
				MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect: " + ex.Message );
			}
		}

		public static int disconnect() {
			String curMethodName = "Listener: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );

			if ( socketClient != null && isWscConnected ) {
				Log.WriteFile( curMethodName + "disconnected" );
				socketClient.DisconnectAsync();
			}

			myEventSubId = "";
			mySanctionNum = "";
			ConnectDialog.WscWebLocation = "";
			myLastConnectResponse = "";
			myTourRow = null;
			HelperFunctions.deleteMonitorHeartBeat( "Listener" );
			Log.WriteFile( curMethodName + "EXIT request processed and all threads closed" );

			return 1;
		}
	}
}
