using System;
using System.Collections.Generic;
using System.Data;

using System.Windows.Forms;

using Quobject.SocketIoClientDotNet.Client;

using Newtonsoft.Json;
using System.Web.Script.Serialization;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	class Listener {
		public static String myWscWebLocation = "";
		public static String myWscWebLocationDefault = "http://ewscdata.com:40000/";
		public static String myWscApplicationKey = "CAD2FB59-3CCB-4691-9D26-7D68C2222788";

		private static String mySanctionNum = "";
		private static String myEventSubId = "";
		private static String myLastConnectResponse = "";
		
		private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;

		private static DataRow myTourRow = null;

		public static bool isWscConnected {
			get { return ( myWscWebLocation.Length > 0 && myLastConnectResponse.Length > 0 ); }
		}

		public static void showPin() {
			MessageBox.Show( myLastConnectResponse );
		}

		public static void startWscListener( String sanctionNum, String eventSubId ) {
			String curMethodName = "Listener: startWscListener: ";
			myLastConnectResponse = "";
			myWscWebLocation = myWscWebLocationDefault;
			mySanctionNum = sanctionNum;
			myEventSubId = eventSubId;

			if ( !(HandlerDataAccess.DataAccessOpen()) ) {
				String curMsg = curMethodName + String.Format( "Database not found at the specified location {0}", Properties.Settings.Default.DatabaseConnectionString);
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
				return;
			}

			myTourRow = HelperFunctions.getTourData();
			if ( myTourRow == null ) {
				String curMsg = curMethodName + String.Format("Tournament data for {0} was not found, terminating attempt to starting listener", mySanctionNum );
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
					, { "application", "WSTIMS Listener" }
					, { "version", "2.0" }
					, { "username", "mawsa@comcast.net" }
					, { "Application_key", Listener.myWscApplicationKey }
				};

			try {
				String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
				Log.WriteFile( String.Format( curMethodName + "Submitting connection request for monitoring connection: {0}", jsonData ) );

				IO.Options connOptions = new IO.Options();
				connOptions.IgnoreServerCertificateValidation = true;
				connOptions.Timeout = 500;
				connOptions.Reconnection = true;
				connOptions.Transports = Quobject.Collections.Immutable.ImmutableList.Create<string>( "websocket" );
				socketClient = IO.Socket( myWscWebLocation, connOptions );

				startClientListeners();
				Log.WriteFile( String.Format( curMethodName + "ClientListeners started, begin monitoring for messages from {0}", myWscWebLocation ) );

				socketClient.On( Socket.EVENT_CONNECT, () => {
					Log.WriteFile( String.Format( curMethodName + "Connection submitted for monitoring connection: {0}", jsonData ) );
					socketClient.Emit( "manual_connection_parameter", jsonData );
				} );

			} catch ( Exception ex ) {
				String curMsg = curMethodName + String.Format( "{0} Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );

				myWscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void startClientListeners() {
			String curMethodName = "Listener: startClientListeners: ";

			try {
				socketClient.On( "connect_confirm", ( data ) => {
					myLastConnectResponse = data.ToString();
					HelperFunctions.updateMonitorHeartBeat( "Listener" );
					HelperFunctions.addMsgListenQueue( "connect_confirm_listener", myLastConnectResponse );
				} );

				socketClient.On( "status_response", ( data ) => {
					checkConnectStatus( data.ToString() );
				} );

				socketClient.On( "connectedapplication_response", ( data ) => {
					Log.WriteFile( String.Format( "{0} connectedapplication_response {1}", curMethodName, data.ToString() ) );
				} );

				socketClient.On( "connectedapplication_check", ( data ) => {
					HelperFunctions.addMsgListenQueue( "connectedapplication_check", data.ToString() );
					Log.WriteFile( String.Format( "{0} connectedapplication_check {1}", curMethodName, data.ToString() ) );
					
					Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
						{ "application", "WSTIMS" }
					};
					String jsonData = JsonConvert.SerializeObject( sendConnectionMsg );
					socketClient.Emit( "connectedapplication_response", jsonData );
				} );

				socketClient.On( "boat_times", ( data ) => {
					HelperFunctions.addMsgListenQueue( "boat_times", data.ToString() );
					Log.WriteFile( String.Format( "{0} boat_times {1}", curMethodName, data.ToString() ) );
				} );

				socketClient.On( "scoring_result", ( data ) => {
					HelperFunctions.addMsgListenQueue( "scoring_result", data.ToString() );
					Log.WriteFile( String.Format( "{0} scoring_result {1}", curMethodName, data.ToString() ) );
				} );

				socketClient.On( "boatpath_data", ( data ) => {
					HelperFunctions.addMsgListenQueue( "boatpath_data", data.ToString() );
					Log.WriteFile( String.Format( "{0} boatpath_data {1}", curMethodName, data.ToString() ) );
				} );

				socketClient.On( "trickscoring_detail", ( data ) => {
					HelperFunctions.addMsgListenQueue( "trickscoring_detail", data.ToString() );
					Log.WriteFile( String.Format( "{0} trickscoring_detail {1}", curMethodName, data.ToString() ) );
				} );

				socketClient.On( "jumpmeasurement_score", ( data ) => {
					HelperFunctions.addMsgListenQueue( "jumpmeasurement_score", data.ToString() );
					Log.WriteFile( String.Format( "{0} jumpmeasurement_score {1}", curMethodName, data.ToString() ) );
				} );

			} catch ( Exception ex ) {
				String curMsg = curMethodName + String.Format( "{0} Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );

				myWscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";

				MessageBox.Show( curMsg );
			}
		}

		private static void checkConnectStatus( String msg ) {
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );
				String curEventId = HelperFunctions.getAttributeValue( curMsgDataList, "eventId" );

				if ( curEventId.Equals( mySanctionNum ) ) {
					if ( myEventSubId.Length > 0 ) return;

					if ( !( HelperFunctions.getAttributeValue( curMsgDataList, "eventSubId" ).Equals( myEventSubId ) ) ) {
						myWscWebLocation = "";
						myLastConnectResponse = "";
						myEventSubId = "";
						MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
					}

				} else {
					myWscWebLocation = "";
					myLastConnectResponse = "";
					myEventSubId = "";
					MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect" );
				}

			} catch ( Exception ex ) {
				myWscWebLocation = "";
				myLastConnectResponse = "";
				myEventSubId = "";
				MessageBox.Show( "** WARNING ** System is no longer connected to WaterskiConnect: " + ex.Message );
			}
		}

		public static int disconnect() {
			String curMethodName = "Listener: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );

			if ( isWscConnected ) {
				socketClient.On( Socket.EVENT_DISCONNECT, () => {
					Log.WriteFile( curMethodName + "disconnected" );
					socketClient.Disconnect();
				} );
			}

			myEventSubId = "";
			mySanctionNum = "";
			myWscWebLocation = "";
			myLastConnectResponse = "";
			myTourRow = null;
			HelperFunctions.deleteMonitorHeartBeat( "Listener" );
			Log.WriteFile( curMethodName + "EXIT request processed and all threads closed" );

			return 1;
		}
	}
}
