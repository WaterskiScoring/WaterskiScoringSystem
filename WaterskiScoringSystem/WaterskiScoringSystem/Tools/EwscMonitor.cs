using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Quobject.SocketIoClientDotNet.Client;
using Quobject.EngineIoClientDotNet;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;

using WaterskiScoringSystem.Common;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;

namespace WaterskiScoringSystem.Tools {
	class EwscMonitor {
		private static String mySanctionNum;
		private static String myEventSubId = "";
		private static DataRow myTourRow;
		private static Boolean myConnectActive = false;
		private static DateTime myCurrentDatetime = DateTime.Now;
		private static char[] singleQuoteDelim = new char[] { '\'' };
		private static String myLastConnectResponse = "";

		/*
		 * You can see the different events that are supported here http://www.ewscdata.com/ewscdata_events
		 * 
		 * You can see the JSON message formats for the events here http://www.ewscdata.com/ewscdata_data_formats
		 * 
		 * I think eventually this might be the address but don't think it is active yet
		 * private static String EwcsWebLocationDefault = "http://waterskiconnect.com:40000/";
		 * 
		 */
		public static String EwcsWebLocation = "";
		private static String EwcsWebLocationDefault = "http://ewscdata.com:40000/";
		private static Quobject.SocketIoClientDotNet.Client.Socket socketClient = null;

		public static void setCurrentDate(String inDatetime) {
			myCurrentDatetime = DateTime.Parse(inDatetime);
		}

		public static String eventSubId {
			get {
				return myEventSubId;
			}
			set {
				myEventSubId = value;
			}
		}

		public static void execEwscMonitoring() {
			if (EwscMonitor.EwcsWebLocation.Length > 1) return;
			EwscMonitor.EwcsWebLocation = EwcsWebLocationDefault;
			String returnValue = startEwscMonitoring();
			MessageBox.Show("WaterSkiConnect: Results=" + returnValue);
		}

		public static Boolean ConnectActive() {
			return myConnectActive;
		}

		private static String startEwscMonitoring() {
			String curMethodName = "startEwscMonitoring: ";
			getSanctionNum();

			Dictionary<string, string> sendConnectionMsg = new Dictionary<string, string> {
					{ "loggingdetail", "no" }
					, { "mode", "Tournament" }
					, { "eventid", mySanctionNum }
					, { "eventsubid", myEventSubId }
					, { "provider", "Mass Water Ski Association" }
					, { "application", Properties.Settings.Default.AppTitle }
					, { "version", Properties.Settings.Default.AppVersion }
					, { "username", "mawsa@comcast.net" }
				};
			String jsonData = JsonConvert.SerializeObject(sendConnectionMsg);

			Log.WriteFile(curMethodName + String.Format("Connected to {0}", EwcsWebLocation));
			socketClient = IO.Socket(EwcsWebLocation);

			socketClient.On(Socket.EVENT_CONNECT, () => {
				Log.WriteFile(curMethodName + String.Format("Connection: {0}", jsonData));
				socketClient.Emit("manual_connection_parameter", jsonData);
			});

			startClientListeners();

			int count = 0;
			while (count >= 0) {
				System.Threading.Thread.Sleep(2000);

				if (count > 5) {
					int returnMsg = checkForMsgToSend();
					if (returnMsg > 0) {
						EwscMonitor.EwcsWebLocation = "";
						myConnectActive = false;
						myLastConnectResponse = "";
						myEventSubId = "";
						return "Exit WaterSkiConnect";
					}
					count = 0;
				}

				count++;
			}
			String returnValue = "";
			return returnValue;
		}

		private static void startClientListeners() {
			socketClient.On("connect_confirm", (data) => {
				myLastConnectResponse = data.ToString();
				showMsg("connect_confirm", String.Format("connect_confirm: {0}", data));
			});

			socketClient.On("boat_times", (data) => {
				saveBoatTimes(data.ToString());
			});

			socketClient.On("scoring_result", (data) => {
				showMsg("scoring_result", String.Format("scoring_result {0}", data));
			});

			socketClient.On("boatpath_data", (data) => {
				saveBoatPath(data.ToString());
			});

			socketClient.On("trickscoring_detail", (data) => {
				showMsg("trickscoring_detail", String.Format("trickscoring_detail {0}", data));
			});

			socketClient.On("jumpmeasurement_score", (data) => {
				saveJumpMeasurement(data.ToString());
			});
		}

		private static void saveBoatTimes( String msg ) {
			StringBuilder curSqlStmt = new StringBuilder("");
			Dictionary<string, object> curMsgDataList = null;

			try {
				Log.WriteFile( "saveBoatTimes: Msg received: " + msg );
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( "saveBoatTimes: Invalid data encountered: " + ex.Message );
				try {
					curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg.Substring( 11 ) );

				} catch ( Exception ex2 ) {
					Log.WriteFile( "saveBoatTimes: Invalid data encountered: " + ex2.Message );
					MessageBox.Show( "saveBoatTimes: Invalid data encountered: " + ex2.Message );
					return;
				}
			}

			try {
				int curRound = (int)getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = 1;
				String curEvent = getAttributeValue( curMsgDataList, "athleteEvent" );

				curSqlStmt.Append("Insert BoatTime ( ");
				curSqlStmt.Append("SanctionId, MemberId, Event");
				curSqlStmt.Append(", Round, PassNumber, PassLineLength, PassSpeedKph");
				curSqlStmt.Append(", BoatTimeBuoy1, BoatTimeBuoy2, BoatTimeBuoy3,BoatTimeBuoy4, BoatTimeBuoy5, BoatTimeBuoy6, BoatTimeBuoy7");
				curSqlStmt.Append(", InsertDate, LastUpdateDate ");
				curSqlStmt.Append(") Values ( ");
				curSqlStmt.Append("'" + mySanctionNum + "'");
				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "athleteId" ) + "'" );
				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "athleteEvent" ) + "'" );

				curSqlStmt.Append(", " + curRound);
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "rope" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "speed" ).ToString( "#0" ) );
				
				if ( curEvent.Equals("Jump")) {
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "nt" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "mt" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "et" ) );
					curSqlStmt.Append(", 0, 0, 0, 0 " );

				} else if (curEvent.Equals("Slalom")) {
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b1" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b2" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b3" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b4" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b5" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "b6" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "endgate" ) );
				}

				curSqlStmt.Append(", getdate(), getdate()");
				curSqlStmt.Append(" )");
				int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
				Log.WriteFile(curSqlStmt.ToString());

			} catch (Exception ex) {
					Log.WriteFile( "saveBoatTimes: Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
					MessageBox.Show("saveBoatTimes: Invalid data encountered: " + ex.Message);
			}
		}

		private static void saveBoatPath(String msg) {
			StringBuilder curSqlStmt = new StringBuilder("");
			StringBuilder curBoatInfo = new StringBuilder( "" );
			Dictionary<string, object> curMsgDataList = null;

			try {
				Log.WriteFile( "saveBoatPath: Msg received: " + msg );
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( "saveBoatPath: Invalid data encountered: " + ex.Message );
				try {
					curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg.Substring( 14 ) );

				} catch ( Exception ex2 ) {
					Log.WriteFile( "saveBoatPath: Invalid data encountered: " + ex2.Message );
					MessageBox.Show( "saveBoatPath: Invalid data encountered: " + ex2.Message );
					return;
				}
			}

			try {
				int curRound = (int)getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = 1;
				String curEvent = getAttributeValue( curMsgDataList, "athleteEvent" );

				curBoatInfo.Append( getAttributeValue( curMsgDataList, "boatManufacturer" ) );
				curBoatInfo.Append( ", " + getAttributeValue( curMsgDataList, "boatModel" ) );
				curBoatInfo.Append( ", " + getAttributeValue( curMsgDataList, "boatYear" ) );
				curBoatInfo.Append( ", " + getAttributeValue( curMsgDataList, "boatColour" ) );
				curBoatInfo.Append( ", " + getAttributeValue( curMsgDataList, "boatComment" ) );
				String curBoatInfoMsg = stringReplace( curBoatInfo.ToString(), singleQuoteDelim, "''" );

				curSqlStmt.Append("Insert BoatPath ( ");
				curSqlStmt.Append( "SanctionId, MemberId, Event, DriverMemberId, DriverName, BoatDescription" );
				curSqlStmt.Append(", Round, PassNumber, PassLineLength, PassSpeedKph");
				curSqlStmt.Append( ", PathDevBuoy0, PathDevCum0, PathDevZone0" );
				curSqlStmt.Append( ", PathDevBuoy1, PathDevCum1, PathDevZone1" );
				curSqlStmt.Append( ", PathDevBuoy2, PathDevCum2, PathDevZone2" );
				curSqlStmt.Append( ", PathDevBuoy3, PathDevCum3, PathDevZone3" );
				curSqlStmt.Append( ", PathDevBuoy4, PathDevCum4, PathDevZone4" );
				curSqlStmt.Append( ", PathDevBuoy5, PathDevCum5, PathDevZone5" );
				curSqlStmt.Append( ", PathDevBuoy6, PathDevCum6, PathDevZone6" );
				curSqlStmt.Append(", RerideNote, InsertDate, LastUpdateDate ");
				curSqlStmt.Append(") Values ( ");
				curSqlStmt.Append("'" + mySanctionNum + "'");
				curSqlStmt.Append(", '" + getAttributeValue( curMsgDataList, "athleteId" ) + "'");
				curSqlStmt.Append(", '" + getAttributeValue( curMsgDataList, "athleteEvent" ) + "'");

				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "driverId" ) + "'" );
				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "driverName" ) + "'" );
				curSqlStmt.Append( ", '" + curBoatInfoMsg + "'" );

				curSqlStmt.Append(", " + curRound);
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "passNumber" ).ToString("#0") );
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "rope" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "speed" ).ToString("#0") );

				if (curEvent.Equals("Jump")) {
					curSqlStmt.Append( getJumpBoatPathAttributes( curMsgDataList ) );

				} else if (curEvent.Equals("Slalom")) {
					curSqlStmt.Append( getSlalomBoatPathAttributes( curMsgDataList ) );
				}

				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "reride" ) + "'" );
				curSqlStmt.Append( ", getdate(), getdate()" );

				curSqlStmt.Append(" )");
				int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
				Log.WriteFile(curSqlStmt.ToString());

			} catch (Exception ex ) {
				Log.WriteFile( "saveBoatPath: Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show("saveBoatPath: Invalid data encountered: " + ex.Message);
			}
		}

		/*
Dave, in the “boatpath_data” message I have added a new Key named “m41” in addition
to “st”, “nt” (will always be empty), “mt” and “et”.
		*/
		private static String getJumpBoatPathAttributes( Dictionary<string, object> curMsgDataList ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Dictionary<string, object> curBuoyResults = null;

			// Start of deviation tracking 600 foot (180 meter) buoy
			// 180,ST,NT,MT,ET and EC is good for me as points

			// Start of jump course timing, entry gates
			curBuoyResults = getAttributeList( curMsgDataList, "start" );
			if ( curBuoyResults == null ) curBuoyResults = getAttributeList( curMsgDataList, "180" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( curMsgDataList, "st" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 52 meter timing segment, only used for E, L, R events
			curBuoyResults = getAttributeList( curMsgDataList, "nt" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 82 meter timing segment, mid course timing gates
			curBuoyResults = getAttributeList( curMsgDataList, "mt" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// 41 meter timing segment, end course timing gates
			curBuoyResults = getAttributeList( curMsgDataList, "et" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			// End of oourse and end of deviation tracking, also referred to as ride out buoys
			curBuoyResults = getAttributeList( curMsgDataList, "ec" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curSqlStmt.Append( ", 0, 0, 0" );
			
			return curSqlStmt.ToString();
		}

		private static String getSlalomBoatPathAttributes( Dictionary<string, object> msgAttributeList ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			Dictionary<string, object> curBuoyResults = null;

			curBuoyResults = getAttributeList( msgAttributeList, "gate" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b1" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b2" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b3" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b4" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b5" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			curBuoyResults = getAttributeList( msgAttributeList, "b6" );
			if ( curBuoyResults != null ) {
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "deviation" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "cumulative" ) );
				curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "zone" ) );

			} else {
				curSqlStmt.Append( ", 0, 0, 0" );
			}

			return curSqlStmt.ToString();
		}
		
		private static void saveJumpMeasurement( String msg ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			StringBuilder curBoatInfo = new StringBuilder( "" );
			Dictionary<string, object> curMsgDataList = null;

			try {
				Log.WriteFile( "saveJumpMeasurement: Msg received: " + msg );
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( "saveJumpMeasurement: Invalid data encountered: " + ex.Message );
				try {
					curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg.Substring( 14 ) );

				} catch ( Exception ex2 ) {
					Log.WriteFile( "saveJumpMeasurement: Invalid data encountered: " + ex2.Message );
					MessageBox.Show( "saveJumpMeasurement: Invalid data encountered: " + ex2.Message );
					return;
				}
			}

			try {
				int curRound = (int)getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound == 0 ) curRound = 1;

				curSqlStmt.Append( "Insert JumpMeasurement ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event, Round, PassNumber, ScoreFeet, ScoreMeters, InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + mySanctionNum + "'" );

				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "athleteId" ) + "'" );
				curSqlStmt.Append( ", '" + getAttributeValue( curMsgDataList, "athleteEvent" ) + "'" );

				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );

				Dictionary<string, object> curBuoyResults = getAttributeList( curMsgDataList, "score" );
				if ( curBuoyResults != null ) {
					curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "distanceFeet" ) );
					curSqlStmt.Append( ", " + getAttributeValueNum( curBuoyResults, "distanceMetres" ) );

				} else {
					curSqlStmt.Append( ", 0, 0" );
				}

				curSqlStmt.Append( ", getdate(), getdate()" );
				curSqlStmt.Append( " )" );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				MessageBox.Show( "saveJumpMeasurement: Invalid data encountered: " + ex.Message );
				Log.WriteFile( "saveJumpMeasurement: Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
			}
		}

		private static Dictionary<string, object> getAttributeList( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return null;
			return (Dictionary<string, object>)msgAttributeList[keyName];
		}
		
		private static decimal getAttributeValueNum( Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey( keyName ) ) ) return 0;

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Int32" ) ) {
				if ( Decimal.TryParse( ((int)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.Decimal" ) ) {
				if ( Decimal.TryParse( ( (decimal)msgAttributeList[keyName] ).ToString(), out decimal returnValue ) ) {
					return returnValue;
				}

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType( "System.String" ) ) {
				if ( Decimal.TryParse( (String)msgAttributeList[keyName], out decimal returnValue ) ) {
					return returnValue;
				}
			}

			return 0;
		}

		private static String getAttributeValue(Dictionary<string, object> msgAttributeList, String keyName ) {
			if ( !( msgAttributeList.ContainsKey(keyName) ) ) return "";

			if ( msgAttributeList[keyName].GetType() == System.Type.GetType("System.Int32")) {
				return ((int)msgAttributeList[keyName]).ToString();
			
			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType("System.Decimal")) {
				return ( (decimal)msgAttributeList[keyName] ).ToString();

			} else if ( msgAttributeList[keyName].GetType() == System.Type.GetType("System.String")) {
				return ( (String)msgAttributeList[keyName] );
			}

			return "";
		}

		private static void showMsg(String msgType, String msg) {
			if (msgType.Equals("connect_confirm")) {
				if (myConnectActive == false) {
					myConnectActive = true;
					MessageBox.Show(msg);
				}
			} else {
				//MessageBox.Show(msg);
			}
		}

		/*
         * Check for messages to be sent
         */
		private static int checkForMsgToSend() {
			DataTable curDataTable = getEwscMsg();
			foreach (DataRow curDataRow in curDataTable.Rows) {
				if (((String)curDataRow["MsgType"]).Equals("Exit")) {
					EwscMonitor.EwcsWebLocation = "";
					myConnectActive = false;
					myLastConnectResponse = "";
					myEventSubId = "";

					removeEwscMsg( (int)curDataRow["PK"]);
					return 1;
				}
				socketClient.Emit((String)curDataRow["MsgType"], (String)curDataRow["MsgData"]);
				removeEwscMsg((int)curDataRow["PK"]);
			}
			return 0;
		}

		public static void showPin() {
			MessageBox.Show(myLastConnectResponse);
		}

		public static Boolean sendBoatData(String boatId, String boatManufacturer, String boatModel
			, Int16 boatYear, String boatColor, String boatComment) {
			Dictionary<string, string> sendMsg = new Dictionary<string, string> {
					{ "boatId", boatId }
					, { "boatManufacturer", boatManufacturer }
					, { "boatModel", boatModel }
					, { "boatYear", boatYear.ToString() }
					, { "boatColor", boatColor }
					, { "boatComment", boatComment }
				};
			addEwscMsg("boat_data", JsonConvert.SerializeObject(sendMsg));
			return true;
		}

		public static Boolean sendAthleteData(String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion, String eventGroup
			, String round, Int16 passNumber, Int16 speed, String rope, String split) {
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "round", round }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "split", split }
				};

			DataTable curDataTable = getDriverAssignment(athleteEvent, eventGroup, round);
			if ( curDataTable.Rows.Count > 0 ) {
				DataRow curDataRow = curDataTable.Rows[(curDataTable.Rows.Count - 1)];
				sendMsg.Add("driver", buildOfficialEntry(curDataRow));
			} else {
				sendMsg.Add( "driver", buildOfficialEntry( null ) );
			}

			addEwscMsg("athlete_data", JsonConvert.SerializeObject(sendMsg));
			myCurrentDatetime = DateTime.Now;
			return true;
		}

		public static Boolean sendAthleteScore(String athleteId, String athleteName, String athleteEvent, String athleteCountry, String athleteRegion
			, Int16 passNumber, Int16 speed, String rope, String score) {
			Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "athleteId", athleteId }
					, { "athleteName", athleteName }
					, { "athleteEvent", athleteEvent }
					, { "athleteCountry", athleteCountry.ToUpper() }
					, { "athleteRegion", athleteRegion.ToUpper() }
					, { "passNumber", passNumber }
					, { "speed", speed }
					, { "rope", rope }
					, { "score", score + "@" + speed.ToString() + "/" + rope }
				};
			addEwscMsg("scoring_score", JsonConvert.SerializeObject(sendMsg));
			return true;
		}

		public static Boolean sendRunningOrder( String curEvent, DataTable curDataTable ) {
			//Dictionary<string, object>[] startListAthletes = new Dictionary<string, object>[curDataTable.Rows.Count];
			ArrayList startListAthletes = new ArrayList();

			int curRow = 0;
			String curEventGroup = "", prevEventGroup = "";
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				curEventGroup = (String)curDataRow["EventGroup"];
				if ( prevEventGroup != curEventGroup && startListAthletes.Count > 0 ) {
					sendRunningOrderForGroup( curEvent, prevEventGroup, startListAthletes );
					startListAthletes = new ArrayList();
				}

				String curFederation = (String)curDataRow["Federation"];
				if ( curFederation.Length == 0 ) curFederation = (String)curDataRow["TourFederation"];

				startListAthletes.Add( new Dictionary<string, object> {
                    { "athleteId", (String)curDataRow["memberId"] }
                    , { "athleteName", (String)curDataRow["SkierName"] }
					, { "athleteCountry", curFederation.ToUpper() }
                    , { "athleteRegion", (String)curDataRow["State"] }
                    , { "position_starting", Convert.ToInt32(curRow + 1) }
                    , { "position_seed", Convert.ToInt32(curDataTable.Rows.Count - curRow) }
                    , { "position_current", Convert.ToInt32(curRow + 1) }
                    , { "score_seed", (decimal)curDataRow["RankingScore"] }
                    , { "score_current", Convert.ToDecimal(0) }
                });

				prevEventGroup = curEventGroup;
				curRow++;
			}

			if ( startListAthletes.Count > 0 ) {
				sendRunningOrderForGroup( curEvent, prevEventGroup, startListAthletes );
			}
			
			return true;
        }

		private static void sendRunningOrderForGroup( String curEvent, String curEventGroup, ArrayList startListAthletes ) {
			Dictionary<string, object> startListMsg = new Dictionary<string, object> {
					{ "startlistName", "EventGroup " + curEventGroup }
					, { "round", 0 }
					, { "eventName", curEvent }
					, { "division", curEventGroup }
					, { "group", curEventGroup }
					, { "lake", "" }
					, { "current_athlete_index", 0 }
					, { "current_athlete_name", "" }
					, { "startlist_athletes", startListAthletes }
				};

			addEwscMsg( "start_list", JsonConvert.SerializeObject( startListMsg ) );
		}

		public static Boolean sendOfficialsAssignments(String curEvent, String eventGroup, Int16 round ) {
			int towerJudgeIdx = 1;
			Boolean officialsAvailable = false;
			Dictionary<string, object> curEventOfficials = new Dictionary<string, object>();
			
			DataTable curDataTable = getOfficialAssignments(curEvent, eventGroup, round);
            foreach (DataRow curDataRow in curDataTable.Rows) {
				String curOfficialAsgmt = (String)curDataRow["WorkAsgmt"];
				if (curOfficialAsgmt.Equals("Driver")) {
					if (curEventOfficials.ContainsKey("driver")) continue;
					curEventOfficials.Add("driver", buildOfficialEntry( curDataRow ));
					officialsAvailable = true;

				} else if (curOfficialAsgmt.Equals("Event Judge")) {
					String judgeSuffix = "";
					if (towerJudgeIdx == 1) {
						judgeSuffix = "1a";
					} else if (towerJudgeIdx == 2) {
						judgeSuffix = "1b";
					} else if (towerJudgeIdx == 3) {
						judgeSuffix = "2a";
					} else if (towerJudgeIdx == 4) {
						judgeSuffix = "2b";
					}
					if (towerJudgeIdx <= 4) {
						curEventOfficials.Add("towerJudge" + judgeSuffix, buildOfficialEntry(curDataRow));
						officialsAvailable = true;
					}
					towerJudgeIdx++;

				} else if (curOfficialAsgmt.Equals("Boat Judge")) {
					if (curEventOfficials.ContainsKey("boatJudge")) continue;
					curEventOfficials.Add("boatJudge", buildOfficialEntry(curDataRow));
					officialsAvailable = true;

				} else if (curOfficialAsgmt.Equals("Scorer")) {
					if (curEventOfficials.ContainsKey("scorer")) continue;
					curEventOfficials.Add("scorer", buildOfficialEntry(curDataRow));
					officialsAvailable = true;
				}
			}
			if (officialsAvailable) {
				//String msg = JsonConvert.SerializeObject(curEventOfficials);
				addEwscMsg("officials_data", JsonConvert.SerializeObject(curEventOfficials));
				return true;
			}

			return false;

		}

		public static decimal[] getBoatTime( String curEvent, String curMemberId, String curRound, String curPassNum, Decimal inPassScore ) {
			if ( curEvent.Length == 0 || curMemberId.Length == 0 || curRound.Length == 0 || curPassNum.Length == 0 ) return new decimal[] { };

			StringBuilder curSqlStmt = new StringBuilder("");

			for ( int count = 0; count < 2; count++ ) {
				curSqlStmt = new StringBuilder("");
				curSqlStmt.Append("SELECT * FROM BoatTime ");
				curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' ");
				curSqlStmt.Append("AND MemberId = '" + curMemberId + "' ");
				curSqlStmt.Append("AND Event = '" + curEvent + "' ");
				curSqlStmt.Append("AND Round = " + curRound + " ");
				curSqlStmt.Append("AND PassNumber = " + curPassNum + " ");
				curSqlStmt.Append("Order by InsertDate ");

				DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
				if (curDataTable.Rows.Count > 0) {
					if (curEvent.Equals("Slalom")) {
						int curPassScore = Convert.ToInt32(Math.Floor(inPassScore)) + 1;
						String timeKey = "BoatTimeBuoy" + curPassScore;
						return new decimal[] { (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1][timeKey] };

					} else if (curEvent.Equals("Jump")) {
						return new decimal[] { (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1]["BoatTimeBuoy1"]
							, (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1]["BoatTimeBuoy2"]
							, (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1]["BoatTimeBuoy3"]
						};

					}

				} else {
					Thread.Sleep(500);
				}
			}

			return new decimal[] { };
		}

		public static DataRow getBoatPath(String curEvent, String curMemberId, String curRound, String curPassNum ) {
			if ( curEvent.Length == 0 || curMemberId.Length == 0 || curRound.Length == 0 || curPassNum.Length == 0 ) return null;

			StringBuilder curSqlStmt = new StringBuilder("");

			for (int count = 0; count < 2; count++) {
				curSqlStmt = new StringBuilder("");
				curSqlStmt.Append( "SELECT P.Event, P.Round, P.PassNumber, P.PassLineLength, P.PassspeedKph" );
				curSqlStmt.Append( ", P.PathDevBuoy0, P.PathDevCum0, P.PathDevZone0" );
				curSqlStmt.Append( ", P.PathDevBuoy1, P.PathDevCum1, P.PathDevZone1, T.BoatTimeBuoy1" );
				curSqlStmt.Append( ", P.PathDevBuoy2, P.PathDevCum2, P.PathDevZone2, T.BoatTimeBuoy2" );
				curSqlStmt.Append( ", P.PathDevBuoy3, P.PathDevCum3, P.PathDevZone3, T.BoatTimeBuoy3" );
				curSqlStmt.Append( ", P.PathDevBuoy4, P.PathDevCum4, P.PathDevZone4, T.BoatTimeBuoy4" );
				curSqlStmt.Append( ", P.PathDevBuoy5, P.PathDevCum5, P.PathDevZone5, T.BoatTimeBuoy5" );
				curSqlStmt.Append( ", P.PathDevBuoy6, P.PathDevCum6, P.PathDevZone6, T.BoatTimeBuoy6" );
				curSqlStmt.Append( ", T.BoatTimeBuoy7" );
				curSqlStmt.Append( ", P.InsertDate, P.LastUpdateDate " );
				curSqlStmt.Append( "FROM BoatPath P " );
				curSqlStmt.Append( "Left Outer Join BoatTime T on T.SanctionId = P.SanctionId AND T.MemberId = P.MemberId AND T.Round = P.Round  AND T.PassNumber = P.PassNumber AND T.Event = P.Event " );
				curSqlStmt.Append("WHERE P.SanctionId = '" + mySanctionNum + "' ");
				curSqlStmt.Append( "AND P.MemberId = '" + curMemberId + "' ");
				curSqlStmt.Append( "AND P.Event = '" + curEvent + "' ");
				curSqlStmt.Append( "AND P.Round = " + curRound + " ");
				curSqlStmt.Append( "AND P.PassNumber = " + curPassNum + " ");
				curSqlStmt.Append( "Order by P.InsertDate " );

				DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
				if (curDataTable.Rows.Count > 0) {
					return curDataTable.Rows[curDataTable.Rows.Count - 1];

				} else {
					Thread.Sleep(500);
				}
			}

			return null;
		}

		public static decimal[] getJumpMeasurement(String curEvent, String curMemberId, String curRound, String curPassNum) {
			if ( curEvent.Length == 0 || curMemberId.Length == 0 || curRound.Length == 0 || curPassNum.Length == 0 ) return new decimal[] { };

			StringBuilder curSqlStmt = new StringBuilder("");

			for (int count = 0; count < 2; count++) {
				curSqlStmt = new StringBuilder("");
				curSqlStmt.Append("SELECT * FROM JumpMeasurement ");
				curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' ");
				curSqlStmt.Append("AND MemberId = '" + curMemberId + "' ");
				curSqlStmt.Append("AND Event = '" + curEvent + "' ");
				curSqlStmt.Append("AND Round = " + curRound + " ");
				curSqlStmt.Append("AND PassNumber = " + curPassNum + " ");
				curSqlStmt.Append("Order by InsertDate ");

				DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
				if (curDataTable.Rows.Count > 0) {
					return new decimal[] { (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1]["ScoreFeet"], (decimal)curDataTable.Rows[curDataTable.Rows.Count - 1]["ScoreMeters"]};

				} else {
					Thread.Sleep(500);
				}
			}

			return new decimal[] {};
		}

		private static Dictionary<string, object> buildOfficialEntry(DataRow curDataRow) {
			if ( curDataRow == null ) {
				return new Dictionary<string, object> {
					{ "officialId", "" }
					, { "officialName", "" }
					, { "officialCountry", "" }
					, { "officialRegion", "" } };
			}

			String curFederation = "";
			try {
				curFederation = (String)curDataRow["Federation"];
				if (curFederation.Length == 0) curFederation = (String)curDataRow["TourFederation"];
			} catch {
				if (curFederation.Length == 0) curFederation = (String)curDataRow["TourFederation"];
			}
			return new Dictionary<string, object> {
					{ "officialId", (String)curDataRow["MemberId"] }
					, { "officialName", (String)curDataRow["MemberName"] }
					, { "officialCountry", curFederation.ToUpper() }
					, { "officialRegion", (String)curDataRow["State"] } };
		}

		public static Boolean sendExit() {
			addEwscMsg( "Exit", "Exit WaterSkiConnect" );
			return true;
        }

		private static DataTable getOfficialAssignments( String inEvent, String inEventGroup, Int16 inRound ) {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt");
			curSqlStmt.Append(", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation ");
			curSqlStmt.Append("FROM OfficialWorkAsgmt O ");
			curSqlStmt.Append("     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T ");
			curSqlStmt.Append("        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId ");
			curSqlStmt.Append("     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt ");
			curSqlStmt.Append("     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId ");
			curSqlStmt.Append("WHERE O.SanctionId = '" + mySanctionNum + "' ");
			curSqlStmt.Append("  AND O.Event = '" + inEvent + "' ");
			curSqlStmt.Append("  AND O.EventGroup = '" + inEventGroup + "' ");
			curSqlStmt.Append("  AND O.Round = " + inRound + " ");
			curSqlStmt.Append("ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName");
			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private static DataTable getDriverAssignment(String inEvent, String inEventGroup, String inRound) {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt");
			curSqlStmt.Append(", T.SkierName AS MemberName, T.State, T.Federation, X.Federation as TourFederation ");
			curSqlStmt.Append("FROM OfficialWorkAsgmt O ");
			curSqlStmt.Append("     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName, State, Federation From TourReg ) T ");
			curSqlStmt.Append("        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId ");
			curSqlStmt.Append("     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt ");
			curSqlStmt.Append("     INNER JOIN Tournament AS X on X.SanctionId = O.SanctionId ");
			curSqlStmt.Append("WHERE O.SanctionId = '" + mySanctionNum + "' ");
			curSqlStmt.Append("  AND O.Event = '" + inEvent + "' ");
			curSqlStmt.Append("  AND O.EventGroup = '" + inEventGroup + "' ");
			curSqlStmt.Append("  AND O.Round = " + inRound + " ");
			curSqlStmt.Append("  AND O.WorkAsgmt = 'Driver' ");
			curSqlStmt.Append("ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName");
			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private static void getSanctionNum() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if (mySanctionNum == null) {
                String msg = "An active tournament must be selected from the Administration menu Tournament List option";
                MessageBox.Show(msg);
                throw new Exception(msg);

            } else if (mySanctionNum.Length < 6) {
                String msg = "An active tournament must be selected from the Administration menu Tournament List option";
                MessageBox.Show(msg);
                throw new Exception(msg);

            } else {
                //Retrieve selected tournament attributes
                DataTable curTourDataTable = getTourData();
                myTourRow = curTourDataTable.Rows[0];
            }
        }

        private static void addEwscMsg( String msgType, String msgData ) {
			String curMsgData = stringReplace( msgData, singleQuoteDelim, "''" );

			StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("Insert EwscMsg ( ");
            curSqlStmt.Append("SanctionId, MsgType, MsgData, CreateDate ");
            curSqlStmt.Append(") Values ( ");
            curSqlStmt.Append("'" + mySanctionNum + "'");
            curSqlStmt.Append(", '" + msgType + "'");
            curSqlStmt.Append(", '" + curMsgData + "'");
            curSqlStmt.Append(", getdate()");
            curSqlStmt.Append(")");
            int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
			Log.WriteFile(curSqlStmt.ToString());
		}

		private static void removeEwscMsg( int pkid ) {
            StringBuilder curSqlStmt = new StringBuilder("Delete FROM EwscMsg Where PK = " + pkid);
            int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
        }

        private static DataTable getEwscMsg() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT PK, SanctionId, MsgType, MsgData ");
            curSqlStmt.Append("FROM EwscMsg M ");
            curSqlStmt.Append("WHERE M.SanctionId = '" + mySanctionNum + "' ");
            return DataAccess.getDataTable(curSqlStmt.ToString());
        }

        private static DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation");
            curSqlStmt.Append(", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' ");
            return DataAccess.getDataTable(curSqlStmt.ToString());
        }

        public static String stringReplace(String inValue, char[] inCurValue, String inReplValue) {
            StringBuilder curNewValue = new StringBuilder("");

            String[] curValues = inValue.Split(inCurValue);
            if (curValues.Length > 1) {
                int curCount = 0;
                foreach (String curValue in curValues) {
                    curCount++;
                    if (curCount < curValues.Length) {
                        curNewValue.Append(curValue + inReplValue);
                    } else {
                        curNewValue.Append(curValue);
                    }
                }
            } else {
                curNewValue.Append(inValue);
            }

            return curNewValue.ToString();
        }
    }
}
