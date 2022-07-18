using System;
using System.Collections.Generic;

using System.Windows.Forms;
using Newtonsoft.Json;

using SocketIOClient;
using SocketIOClient.Transport;

namespace WaterSkiConnectMonitor {
	static class Program {
		private static readonly String myWscWebLocation = "http://ewscdata.com:40000/";
		private static readonly String myWscApplicationKey = "CAD2FB59-3CCB-4691-9D26-7D68C2222788";
		private static readonly int myCountPingMax = 10;

		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static int myCountPing = 0;

		private static SocketIO socketClient = null;

		[STAThread]
		static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Console.WriteLine(
				"Name: " + System.Reflection.Assembly.GetCallingAssembly().GetName()
				+ System.Environment.NewLine + "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version
				+ System.Environment.NewLine + "Location: " + System.Reflection.Assembly.GetExecutingAssembly().Location
				+ System.Environment.NewLine
				); ;

			Console.WriteLine( "Enter Sanction and EventSubId (optional) separated by a comma: " );
			String inputline = Console.ReadLine();
			String[] inputArgs = inputline.Split( ',' );
			if ( inputArgs.Length > 0 ) {
				if ( inputArgs.Length >= 1 ) mySanctionNum = inputArgs[0].Trim();
				if ( inputArgs.Length >= 2 ) myEventSubId = inputArgs[1].Trim();
			}

			execConnect();

			while ( true ) System.Threading.Thread.Sleep( 5000 );
		}

		private static void execConnect() {
			String curMethodName = "WaterSkiConnectMonitor: execConnect: ";
			try {
				/*
				 * Documentation of options
				 * https://github.com/doghappy/socket.io-client-csharp#options
				 * EIO 4 Default 
				 * if your server is using socket.io server v2.x, please explicitly set it to 3
				 */
				socketClient = new SocketIO( myWscWebLocation, new SocketIOOptions {
					Transport = TransportProtocol.WebSocket
					, Reconnection = true
					, ReconnectionDelay = 1000
					, ReconnectionAttempts = 25
					, EIO = 3
				} );

				clientListeners();

				Console.WriteLine( String.Format( "{0}ClientListeners started, begin monitoring for messages from {1}"
					, curMethodName, myWscWebLocation ) );
				socketClient.ConnectAsync();
				Console.WriteLine( String.Format( "{0}ConnectAsync", curMethodName ) );

			} catch ( Exception ex ) {
				showConsoleMsg( "Exception", String.Format( "{0}Exception encounter {1}: StackTrace: {2}", curMethodName, ex.Message, ex.StackTrace ) );
			}
		}

		private static void clientListeners() {
			String curMethodName = "WaterSkiConnectMonitor: clientListeners: ";

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

				socketClient.On( "connectedapplication_response", ( response ) => {
					checkConnectStatus( "status_response", response.GetValue<string>() );
				} );

				socketClient.On( "status_response", ( response ) => {
					checkConnectStatus( "status_response", response.GetValue<string>() );
				} );

				socketClient.On( "boat_times", response => {
					showConsoleMsg( "boat_times", response.GetValue<string>() );
				} );

				socketClient.On( "boatpath_data", ( response ) => {
					showConsoleMsg( "boatpath_data", response.GetValue<string>() );
				} );

				socketClient.On( "scoring_result", ( response ) => {
					showConsoleMsg( "scoring_result", response.GetValue<string>() );
				} );

				socketClient.On( "trickscoring_detail", ( response ) => {
					showConsoleMsg( "trickscoring_detail", response.GetValue<string>() );
				} );

				socketClient.On( "jumpmeasurement_score", ( response ) => {
					showConsoleMsg( "jumpmeasurement_score", response.GetValue<string>() );
				} );

				socketClient.On( "athlete_data", ( response ) => {
					showConsoleMsg( "athlete_data", response.GetValue<string>() );
				} );

				socketClient.On( "pass_data", ( response ) => {
					showConsoleMsg( "pass_data", response.GetValue<string>() );
				} );

				socketClient.On( "scoring_score", ( response ) => {
					showConsoleMsg( "scoring_score", response.GetValue<string>() );
				} );

				socketClient.On( "leaderboard", ( response ) => {
					showConsoleMsg( "leaderboard", response.GetValue<string>() );
				} );

				socketClient.On( "boat_times_scoring", ( response ) => {
					showConsoleMsg( "boat_times_scoring", response.GetValue<string>() );
				} );

				socketClient.On( "officials_data", ( response ) => {
					showConsoleMsg( "officials_data", response.GetValue<string>() );
				} );

				socketClient.On( "boat_data", ( response ) => {
					showConsoleMsg( "boat_data", response.GetValue<string>() );
				} );

				socketClient.On( "start_list", ( response ) => {
					showConsoleMsg( "start_list", response.GetValue<string>() );
				} );

			} catch ( Exception ex ) {
				showConsoleMsg("Exception", String.Format( "{0}Exception encounter {1}: StackTrace: {2}", curMethodName, ex.Message, ex.StackTrace ) );
			}
		}

		private static void handleSocketOnConnected( object sender, EventArgs argData ) {
			String curMethodName = "WaterSkiConnectMonitor: handleSocketOnConnected: ";
			showConsoleMsg( curMethodName, String.Format( "Socket.Id: {0}, Connected: {1}, ServerUri: {2}, argData={3}"
				, socketClient.Id, socketClient.Connected, socketClient.ServerUri, argData == null ? "Null" : argData.ToString() ) );

			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", mySanctionNum }
					, { "eventsubid", myEventSubId }
					, { "provider", "Mass Water Ski Association" }
					, { "application", "WaterskiConnect Monitor" }
					, { "version", "3.0" }
					, { "username", "mawsa@comcast.net" }
					, { "Application_key", myWscApplicationKey }
				};
			String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
			socketClient.EmitAsync( "manual_connection_parameter", jsonData );
			showConsoleMsg( curMethodName, String.Format( "Connection manual server request for monitoring connection: {0}", jsonData ) );
		}

		private static void handleSocketOnPing( object sender, EventArgs argData ) {
			myCountPing++;
			if ( socketClient.Connected && myCountPing > myCountPingMax ) {
				//String curMethodName = "Listener: handleSocketOnPing: ";
				showConsoleMsg( "handleSocketOnPing", String.Format( "Connected:{0}, argData={1}", socketClient.Connected, argData == null ? "Null" : argData.ToString() ) );
				myCountPing = 0;
			}
		}

		private static void handleSocketOnReconnecting( object sender, int argData ) {
			String curMethodName = "Listener: handleSocketOnReconnecting: ";
			showConsoleMsg( curMethodName, String.Format( "Attempt: {0}", argData ) );
		}

		private static void handleSocketOnDisconnected( object sender, String argData ) {
			String curMethodName = "Listener: handleSocketOnDisconnected: ";
			showConsoleMsg( curMethodName, String.Format( "argData: {0}", argData ) );
		}

		private static void handleWscConnectConfirm( String txnName, String argData ) {
			showConsoleMsg( txnName, String.Format( "argData: {0}", argData ) );
		}

		private static void handleConnectHeartBeat( String txnName, String argData ) {
			showConsoleMsg( "handleConnectHeartBeat", argData );
			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
						{ "application", "WaterskiConnect Monitor" }
					};
			String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
			socketClient.EmitAsync( "connectedapplication_response", jsonData );
		}

		private static void checkConnectStatus( String txnName, String argData ) {
			showConsoleMsg( txnName, String.Format( "Connected: {0}, Msg: {1}", socketClient.Connected, argData ) );
		}

		public static int disconnect() {
			String curMethodName = "WaterSkiConnectMonitor: disconnect: ";
			showConsoleMsg( curMethodName, "EXIT request received and being processed" );

			if ( socketClient != null ) socketClient.DisconnectAsync();

			showConsoleMsg( curMethodName, "EXIT request processed and all threads closed" );
			return 1;
		}
		
		private static void showConsoleMsg( String txnName, String argData ) {
			Console.WriteLine( String.Format( "{0} {1}: {2}", DateTime.Now, txnName, argData ) );
			Console.WriteLine( "------------------------------------------------------------" );
			Console.WriteLine( "" );

		}

	}
}
