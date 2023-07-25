using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	class ListenHandler {
		private static readonly int myDefaultRound = 1;
		private static bool myListenHandlerActive = false;

		private static System.Timers.Timer readProcessTimer;

		public static void startWscMessageHandler() {
			String curMethodName = "ListenHandler:startWscMessageHandler: ";
			myListenHandlerActive = false;
			Log.WriteFile( String.Format( curMethodName + "Sanction: {0}, eventSubId: {1}, useJumpTimes={2}"
				, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId, ConnectMgmtData.useJumpTimes ) );

			HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
			myListenHandlerActive = true;

			// Create a timer with 2 second interval.
			readProcessTimer = new System.Timers.Timer( 2000 );
			readProcessTimer.Elapsed += readProcessMessages;
			readProcessTimer.AutoReset = false;
			readProcessTimer.Enabled = true;
		}

		private static void readProcessMessages( object sender, EventArgs e ) {
			readProcessTimer.Stop();
			readProcessTimer.Elapsed -= readProcessMessages;
			
			int returnMsg = checkForMsgToHandler();
			if ( returnMsg > 0 ) return;
			if ( ConnectMgmtData.sanctionNum.Length == 0 ) return;
		}

		public static int disconnect() {
			String curMethodName = "ListenHandler:disconnect: ";
			
			readProcessTimer.Stop();
			readProcessTimer.Elapsed -= readProcessMessages;
			
			myListenHandlerActive = false;
			HelperFunctions.deleteMonitorHeartBeat( "ListenHandler" );
			Log.WriteFile( String.Format( "{0}Disconnection request processed", curMethodName ) );
			return 1;
		}

		private static int checkForMsgToHandler() {
			String curMethodName = "ListenHandler: checkForMsgToHandler:  ";
			String msgType = "", msgData = "";
			try {
				DataTable curDataTable = getWscMsgReceived();
				if ( curDataTable != null ) {
					foreach ( DataRow curDataRow in curDataTable.Rows ) {
						msgType = (String)curDataRow["MsgType"];
						msgData = (String)curDataRow["MsgData"];
						removeWscMsgHandled( (int)curDataRow["PK"] );

						if ( msgType.Equals( "connect_confirm_listener" ) ) {
							connectConfirm( msgType, msgData );

						} else if ( msgType.Equals( "heartbeat" ) ) {
							HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
							Log.WriteFile( String.Format( "{0}{1}: Message: {2}", curMethodName, msgType, msgData ) );

						} else if ( msgType.Equals( "connectedapplication_check" ) ) {
							Log.WriteFile( String.Format( "{0}{1}: Message: {2}", curMethodName, msgType, msgData ) );

						} else if ( msgType.Equals( "connectedapplication_response" ) ) {
							Log.WriteFile( String.Format( "{0}{1}: Message: {2}", curMethodName, msgType, msgData ) );

						} else if ( msgType.Equals( "status_response" ) ) {
							Log.WriteFile( String.Format( "{0}{1}: Message: {2}", curMethodName, msgType, msgData ) );

						} else if ( msgType.Equals( "boat_times" ) ) {
							saveBoatTimes( msgData );

						} else if ( msgType.Equals( "boatpath_data" ) ) {
							saveBoatPath( msgData );

						} else if ( msgType.Equals( "jumpmeasurement_score" ) ) {
							saveJumpMeasurement( msgData );

						} else if ( msgType.Equals( "trickscoring_score" ) ) {
							saveTrickDetail( msgData );

						} else if ( msgType.Equals( "trickscoring_detail" ) ) {
							saveTrickDetail( msgData );

						} else if ( msgType.Equals( "scoring_result" ) ) {
							showMsg( "scoring_result", msgData );

						} else {
							showMsg( msgType, msgData );
						}
					}
				}

				return 0;

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Exception encountered " + ex.Message );
				return -1;

			} finally {
				readProcessTimer = new System.Timers.Timer( 1500 );
				readProcessTimer.Elapsed += readProcessMessages;
				readProcessTimer.AutoReset = false;
				readProcessTimer.Enabled = true;
			}
		}

		private static void connectConfirm( String msgType, String msg ) {
			String curMethodName = "ListenHandler: connectConfirm:  ";
			Log.WriteFile( String.Format( "{0}{1} {2}", curMethodName, msgType, msg ) );
			HelperFunctions.updateMonitorHeartBeat( "ListenHandler" );
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

			int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
			if ( curRound == 0 ) curRound = myDefaultRound;
			String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );
			String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" );
			Int16 curPassNumber = 0;
			try {
				curPassNumber = Convert.ToInt16( HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				return;
			}

			if ( checkForSkierBoatTimePassExist( curEvent, curRound, curMemberId, curPassNumber ) ) return;

			insertBoatTime( curMsgDataList, curEvent, curRound, curMemberId, curPassNumber );
		}

		private static bool checkForSkierBoatTimePassExist( String curEvent, int curRound, String curMemberId, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:checkForSkierBoatTimePassExist: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curSqlStmt.Append( "Select PK From BoatTime " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curDataTable.Rows.Count == 0 ) return false;

			if ( curPassNumber == 1 ) {
				// If data is for pass number 1 then assume previous data was for simulation passes
				curSqlStmt = new StringBuilder( "Delete From BoatTime " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				int rowsDeleted = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				// If delete fails then error message is shown but return value to skip further processing
				if ( rowsDeleted < 0 ) return true;

				// Row deleted so new row can be added
				Log.WriteFile( curMethodName + "New boat time data received for first skier pass therefore deleting existing and replacing with new data"
					+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				return false;
			}

			// Assuming this is duplicate data and should be ignored
			Log.WriteFile( curMethodName + "Duplicate boat time data received for skier pass therefore bypassing new data"
				+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			return true;
		}

		private static void insertBoatTime( Dictionary<string, object> curMsgDataList, String curEvent, int curRound, String curMemberId, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:insertBoatTime: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			
			try {
				curSqlStmt.Append( "Insert BoatTime ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event" );
				curSqlStmt.Append( ", Round, PassNumber, PassLineLength, PassSpeedKph" );
				curSqlStmt.Append( ", BoatTimeBuoy1, BoatTimeBuoy2, BoatTimeBuoy3,BoatTimeBuoy4, BoatTimeBuoy5, BoatTimeBuoy6, BoatTimeBuoy7" );
				curSqlStmt.Append( ", InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + ConnectMgmtData.sanctionNum + "'" );
				curSqlStmt.Append( ", '" + curMemberId + "'" );
				curSqlStmt.Append( ", '" + curEvent + "'" );

				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + curPassNumber.ToString( "#0" ) );
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
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
		}

		private static void saveBoatPath( String msg ) {
			String curMethodName = "ListenHandler:saveBoatPath: ";
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( msg );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
				return;
			}

			int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
			if ( curRound == 0 ) curRound = myDefaultRound;
			String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );
			String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" );
			Int16 curPassNumber = 0;
			try {
				curPassNumber = Convert.ToInt16( HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				return;
			}

			if ( checkForSkierBoatPathPassExist( curEvent, curRound, curMemberId, curPassNumber ) ) return;

			insertBoatPath( curMsgDataList, curEvent, curRound, curMemberId, curPassNumber );
		}

		private static bool checkForSkierBoatPathPassExist( String curEvent, int curRound, String curMemberId, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:checkForSkierBoatPathPassExist: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curSqlStmt.Append( "Select PK From BoatPath " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			
			if ( curDataTable.Rows.Count == 0 ) return false;
			
			if ( curPassNumber == 1 ) {
				// If data is for pass number 1 then assume previous data was for simulation passes
				curSqlStmt = new StringBuilder( "Delete From BoatPath " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				int rowsDeleted = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				
				// If delete fails then error message is shown but return value to skip further processing
				if ( rowsDeleted < 0 ) return true;
				
				// Row deleted so new row can be added
				Log.WriteFile( curMethodName + "New boat path data received for first skier pass therefore deleting existing and replacing with new data"
					+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				return false; 
			}

			// Assuming this is duplicate data and should be ignored
			Log.WriteFile( curMethodName + "Duplicate boat path data received for skier pass therefore bypassing new data"
				+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			return true;
		}

		private static void insertBoatPath( Dictionary<string, object> curMsgDataList, String curEvent, int curRound, String curMemberId, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:insertBoatPath: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			StringBuilder curBoatInfo = new StringBuilder( "" );

			try {
				curBoatInfo.Append( HelperFunctions.getAttributeValue( curMsgDataList, "boatManufacturer" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatModel" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatYear" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatColour" ) );
				curBoatInfo.Append( ", " + HelperFunctions.getAttributeValue( curMsgDataList, "boatComment" ) );
				String curBoatInfoMsg = HelperFunctions.stringReplace( curBoatInfo.ToString(), HelperFunctions.singleQuoteDelim, "''" );

				curSqlStmt.Append( "Insert BoatPath ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event, DriverMemberId, DriverName, BoatDescription" );
				curSqlStmt.Append( ", homologation, Round, PassNumber, PassLineLength, PassSpeedKph" );
				curSqlStmt.Append( ", PathDevBuoy0, PathDevCum0, PathDevZone0" );
				curSqlStmt.Append( ", PathDevBuoy1, PathDevCum1, PathDevZone1" );
				curSqlStmt.Append( ", PathDevBuoy2, PathDevCum2, PathDevZone2" );
				curSqlStmt.Append( ", PathDevBuoy3, PathDevCum3, PathDevZone3" );
				curSqlStmt.Append( ", PathDevBuoy4, PathDevCum4, PathDevZone4" );
				curSqlStmt.Append( ", PathDevBuoy5, PathDevCum5, PathDevZone5" );
				curSqlStmt.Append( ", PathDevBuoy6, PathDevCum6, PathDevZone6" );
				curSqlStmt.Append( ", RerideNote, InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + ConnectMgmtData.sanctionNum + "'" );
				curSqlStmt.Append( ", '" + curMemberId + "'" );
				curSqlStmt.Append( ", '" + curEvent + "'" );

				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "driverId" ) + "'" );
				curSqlStmt.Append( ", '" + HelperFunctions.stringReplace( HelperFunctions.getAttributeValue( curMsgDataList, "driverName" ).ToString(), HelperFunctions.singleQuoteDelim, "''" ) + "'" );
				curSqlStmt.Append( ", '" + curBoatInfoMsg + "'" );

				curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curMsgDataList, "homologation" ) + "'" );
				curSqlStmt.Append( ", " + curRound );
				curSqlStmt.Append( ", " + curPassNumber.ToString( "#0" ) );
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
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

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

			int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
			if ( curRound == 0 ) curRound = myDefaultRound;
			String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "athleteEvent" );
			String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" );
			Int16 curPassNumber = 0;
			try {
				curPassNumber = Convert.ToInt16( HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );
			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				MessageBox.Show( curMethodName + "Invalid passNumber encountered: " + ex.Message );
				return;
			}

			if ( checkForSkierJumpMeasurementPassExist( curMemberId, curEvent, curRound, curPassNumber ) ) return;

			insertJumpMeasurement( curMsgDataList, curEvent, curRound, curMemberId, curPassNumber );
		}

		private static bool checkForSkierJumpMeasurementPassExist( String curMemberId, String curEvent, int curRound, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:checkForSkierJumpMeasurementPassExist: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curSqlStmt.Append( "Select PK From JumpMeasurement " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			if ( curDataTable.Rows.Count == 0 ) return false;

			if ( curPassNumber == 1 ) {
				// If data is for pass number 1 then assume previous data was for simulation passes
				curSqlStmt = new StringBuilder( "Delete From JumpMeasurement " );
				curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				int rowsDeleted = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				// If delete fails then error message is shown but return value to skip further processing
				if ( rowsDeleted < 0 ) return true;

				// Row deleted so new row can be added
				Log.WriteFile( curMethodName + "New Jump Measurement data received for first skier pass therefore deleting existing and replacing with new data"
					+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
					, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
				return false;
			}

			// Assuming this is duplicate data and should be ignored
			Log.WriteFile( curMethodName + "Duplicate Jump Measurement data received for skier pass therefore bypassing new data"
				+ String.Format( ": Data for SanctionId = '{0}' AND Event = '{1}' AND MemberId = '{2}' AND Round = {3} AND PassNumber = {4}"
				, ConnectMgmtData.sanctionNum, curEvent, curMemberId, curRound, curPassNumber ) );
			return true;
		}

		private static void insertJumpMeasurement( Dictionary<string, object> curMsgDataList, String curEvent, int curRound, String curMemberId, Int16 curPassNumber ) {
			String curMethodName = "ListenHandler:insertJumpMeasurement: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt.Append( "Insert JumpMeasurement ( " );
				curSqlStmt.Append( "SanctionId, MemberId, Event, Round, PassNumber, ScoreFeet, ScoreMeters, InsertDate, LastUpdateDate " );
				curSqlStmt.Append( ") Values ( " );
				curSqlStmt.Append( "'" + ConnectMgmtData.sanctionNum + "'" );

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
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
		}

		private static void saveTrickDetail( String msg ) {
			String curMethodName = "ListenHandler:saveTrickDetail: ";
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

			String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "athleteId" );
			String curSkierName = HelperFunctions.getAttributeValue( curMsgDataList, "athleteName" );
			String curDiv = HelperFunctions.getAttributeValue( curMsgDataList, "athleteDivision" );

			if ( checkTrickSkierExist( curMemberId, curSkierName, curDiv ) ) {
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
				if ( curRound > 0 ) {
					String[] curTrickSkierAttr = getTrickSkierAttributes( curMemberId, curRound, curDiv );
					insertTrickDetail( curMsgDataList, curMemberId, curTrickSkierAttr[0], curTrickSkierAttr[1], curRound );

				} else {
					String curMsg = String.Format( "Round attribute not available on input message for trick skier {0} ({1}), bypassing trick detail message"
						, ConnectMgmtData.sanctionNum, curSkierName, curMemberId );
					Log.WriteFile( curMethodName + curMsg );
					MessageBox.Show( curMethodName + curMsg );
				}
			}
		}

		private static bool checkTrickSkierExist( String curMemberId, String curSkierName, String curDiv ) {
			String curMethodName = "ListenHandler:checkTrickSkierExist: ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK From EventReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = 'Trick' AND MemberId = '{1}'"
				, ConnectMgmtData.sanctionNum, curMemberId ) );
			if ( HelperFunctions.isObjectPopulated( curDiv ) ) curSqlStmt.Append( String.Format( " AND AgeGroup = '{0}'", curDiv ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return true;
			
			Log.WriteFile( curMethodName + String.Format( "Registration for trick skier {0} ({1}) not found, bypassing trick detail message"
				, ConnectMgmtData.sanctionNum, curSkierName, curMemberId ) );
			return false;
		}

		private static String[] getTrickSkierAttributes( String curMemberId, int curRound, String curDiv ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );

			curSqlStmt.Append( "Select AgeGroup, EventClass " );
			curSqlStmt.Append( "From EventReg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND Event = 'Trick' AND MemberId = '{1}'"
				, ConnectMgmtData.sanctionNum, curMemberId ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			String[] curReturnAttributes = { (String)curDataTable.Rows[0]["AgeGroup"], (String)curDataTable.Rows[0]["EventClass"] };

			curSqlStmt = new StringBuilder( "" ); 
			curSqlStmt.Append( "Delete From TrickScore " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}' AND Round = {2} AND AgeGroup = '{3}'"
				, ConnectMgmtData.sanctionNum, curMemberId, curRound, curReturnAttributes[0] ) );
			int curRowsRemoved = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Delete From TrickPass " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MemberId = '{1}' AND Round = {2} AND AgeGroup = '{3}'"
				, ConnectMgmtData.sanctionNum, curMemberId, curRound, curReturnAttributes[0] ) );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			return curReturnAttributes;
		}

		private static void insertTrickDetail( Dictionary<string, object> curMsgDataList, String curMemberId, String curAgeGroup, String curEventClass, int curRound ) {
			String curMethodName = "ListenHandler:insertTrickDetail: ";
			int rowsProc = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				curSqlStmt.Append( "Insert TrickScore (" );
				curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass" );
				curSqlStmt.Append( ", Score, ScorePass1, ScorePass2" );
				curSqlStmt.Append( ", NopsScore, Boat, Status, LastUpdateDate, InsertDate, Note" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + ConnectMgmtData.sanctionNum + "'" );
				curSqlStmt.Append( ", '" + curMemberId + "'" );
				
				curSqlStmt.Append( ", '" + curAgeGroup + "'" );
				curSqlStmt.Append( ", " + curRound.ToString() );
				curSqlStmt.Append( ", '" + curEventClass + "'" );
				
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "Score_Total" ).ToString("#0") );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "Score_Pass_1" ).ToString( "#0" ) );
				curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curMsgDataList, "Score_Pass_2" ).ToString( "#0" ) );

				curSqlStmt.Append( ", 0, '', 'Complete'" );
				curSqlStmt.Append( ", GETDATE(), GETDATE()" );
				curSqlStmt.Append( ", 'eyeTrick'" );
				curSqlStmt.Append( " )" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc <= 0 ) Log.WriteFile( curMethodName + String.Format("Failed to add trick score for {0} round {1} ", curMemberId, curRound ) );

				IEnumerator curPassList = HelperFunctions.getAttributeLists( curMsgDataList, "Pass_Detail" ).GetEnumerator();
				while ( curPassList.MoveNext() ) {
					if ( curPassList.Current == null ) continue;

					Dictionary<string, object> curPassEntry = (Dictionary<string, object>)curPassList.Current;
					IEnumerator curTrickList = HelperFunctions.getAttributeLists( curPassEntry, "Tricks" ).GetEnumerator();
					while ( curTrickList.MoveNext() ) {
						if ( curTrickList.Current == null ) continue;

						Dictionary<string, object> curTrickEntry = (Dictionary<string, object>)curTrickList.Current;
						String curTrickStatus = getTrickStatus( curTrickEntry );


						curSqlStmt = new StringBuilder( "" );
						curSqlStmt.Append( "Insert TrickPass (" );
						curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, PassNum" );
						curSqlStmt.Append( ", Skis, Seq, Score, Code, Results, LastUpdateDate, Note" );
						curSqlStmt.Append( ") Values (" );
						curSqlStmt.Append( "'" + ConnectMgmtData.sanctionNum + "'" );
						curSqlStmt.Append( ", '" + curMemberId + "'" );

						curSqlStmt.Append( ", '" + curAgeGroup + "'" );
						curSqlStmt.Append( ", " + curRound );
						curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curTrickEntry, "passnum" ).ToString( "#0" ) );
						curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curTrickEntry, "skis" ).ToString( "#0" ) );
						curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curTrickEntry, "trickidx" ).ToString( "#0" ) );
						if ( curTrickStatus.Equals( "Credit" ) ) {
							curSqlStmt.Append( ", " + HelperFunctions.getAttributeValueNum( curTrickEntry, "points_trick" ).ToString( "#0" ) );
						} else {
							curSqlStmt.Append( ", 0" );
						}
						curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curTrickEntry, "trickcode" ).ToUpper() + "'" );
						curSqlStmt.Append( ", '" + curTrickStatus + "'" );

						curSqlStmt.Append( ", GETDATE()" );
						curSqlStmt.Append( ", '" + HelperFunctions.getAttributeValue( curTrickEntry, "tricksetid" ) + "'" );
						curSqlStmt.Append( " )" );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
						if ( rowsProc <= 0 ) Log.WriteFile( curMethodName 
							+ String.Format( "Failed to add trick entry for {0} round={1}, Pass={2}, Seq={3}, Trick={4}"
							, curMemberId, curRound
							, HelperFunctions.getAttributeValueNum( curTrickEntry, "passnum" ).ToString( "#0" )
							, HelperFunctions.getAttributeValueNum( curTrickEntry, "trickidx" ).ToString( "#0" )
							, HelperFunctions.getAttributeValue( curTrickEntry, "trickcode" )
							) );
					}
				}


			} catch ( Exception ex ) {
				Log.WriteFile( curMethodName + "Invalid data encountered: " + ex.Message + ", curSqlStmt=" + curSqlStmt.ToString() );
				MessageBox.Show( curMethodName + "Invalid data encountered: " + ex.Message );
			}
		}

		private static String getTrickStatus( Dictionary<string, object> curTrickEntry ) {
			String curReturnStatus = "Credit";
			String curStatus = HelperFunctions.getAttributeValue( curTrickEntry, "credit" );
			switch (curStatus) {
				case "Credit":
					curReturnStatus = "Credit";
					break;

				case "Repeat":
					curReturnStatus = "Repeat";
					break;

				case "Flip":
					curReturnStatus = "Repeat";
					break;

				case "POS":
					curReturnStatus = "Before";
					break;

				case "Review":
					curReturnStatus = "Unresolved";
					break;

				case "Time":
					curReturnStatus = "OOC";
					break;

				case "Fall":
					curReturnStatus = "Fall";
					break;

				default:
					curReturnStatus = "No Credit";
					break;
			}
			return curReturnStatus;
		}

		private static void showMsg( String msgType, String msg ) {
			MessageBox.Show( "msgType: " + msgType + "\nMsg: " + msg );
		}

		private static DataTable getWscMsgReceived() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( "FROM WscMsgListen " );
			curSqlStmt.Append( "WHERE SanctionId = '" + ConnectMgmtData.sanctionNum + "' " );
			curSqlStmt.Append( "Order by CreateDate " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private static void removeWscMsgHandled( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgListen Where PK = " + pkid );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
