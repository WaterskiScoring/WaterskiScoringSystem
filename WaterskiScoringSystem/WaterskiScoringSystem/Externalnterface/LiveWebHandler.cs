using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
	class LiveWebHandler {
		private static String myMessageHandler = "LiveWebMessageHandler.exe";

		public static bool LiveWebMessageHandlerActive = false;
		private static ExportLiveWebDialog myLiveWebDialog = null;

		public static ExportLiveWebDialog LiveWebDialog {
			get {
				if ( myLiveWebDialog == null ) myLiveWebDialog = new ExportLiveWebDialog();
				return myLiveWebDialog;
			}
		}

		public static bool connectLiveWebHandler( String inSanctionId ) {
			String methodName = "LiveWebHandler: connectLiveWebHandler: ";
			String curDatabaseFilename = DataAccess.getDatabaseFilename();
			LiveWebMessageHandlerActive = false;

			try {
				Process[] curRunningHandlers = Process.GetProcessesByName( "LiveWebMessageHandler" );
				if ( curRunningHandlers.Length > 0 ) {
					LiveWebMessageHandlerActive = true;
					return LiveWebMessageHandlerActive;
				}

				ProcessStartInfo curStartInfo = new ProcessStartInfo( myMessageHandler );
				curStartInfo.WindowStyle = ProcessWindowStyle.Normal;
				curStartInfo.Arguments = String.Format( "{0} \"{1}\"", inSanctionId, curDatabaseFilename );
				curStartInfo.UseShellExecute = true;
				Process.Start( curStartInfo );
				LiveWebMessageHandlerActive = true;

			} catch (Exception ex) {
				String curErrMsg = String.Format( "{0}Exception encountered launching {1}: {2}"
					, methodName, myMessageHandler, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg + "\n\n StackTrace: " + ex.StackTrace );
			}

			return LiveWebMessageHandlerActive;
		}

		/*
		 */
		public static bool disconnectLiveWebHandler() {
			LiveWebMessageHandlerActive = false; 
			return true;
		}

		/*
		 */
		public static bool sendCurrentSkier( String inEvent, String inSanctionId, String inMemberId
			, String inAgeGroup, byte inRound, int inSkierRunNum ) {
			String curMethodName = "LiveWebHandler: sendCurrentSkier: ";

			try {
				Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "sanctionId", inSanctionId }
					, { "memberId", inMemberId }
					, { "event", inEvent }
					, { "ageGroup", inAgeGroup }
					, { "round", inRound }
					, { "passNumber", inSkierRunNum }
				};

				addLiveWebMsgSend( inSanctionId, "CurrentSkier", JsonConvert.SerializeObject( sendMsg ) );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0} Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg );
				return false;
			}
		}

		/*
		 */
		public static bool sendSkiers( String inEvent, String inSanctionId, byte inRound, String inEventGroup ) {
			String curMethodName = "LiveWebHandler: sendSkiers: ";
			
			try {
				Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "sanctionId", inSanctionId }
					, { "event", inEvent }
					, { "eventGroup", inEventGroup }
					, { "round", inRound }
				};

				addLiveWebMsgSend( inSanctionId, "CurrentSkiers", JsonConvert.SerializeObject( sendMsg ) );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0} Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg );
				return false;
			}
		}

		/*
		 */
		public static bool sendRunOrder( String inEvent, String inSanctionId, String inEventGroup, byte inRound ) {
			String curMethodName = "LiveWebHandler: sendRunOrder: ";

			try {
				Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "sanctionId", inSanctionId }
					, { "event", inEvent }
					, { "eventGroup", inEventGroup }
					, { "round", inRound }
				};

				addLiveWebMsgSend( inSanctionId, "RunOrder", JsonConvert.SerializeObject( sendMsg ) );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0} Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg );
				return false;
			}
		}

		/*
		 */
		public static bool sendDisableCurrentSkier( String inEvent, String inSanctionId, String inMemberId, String inAgeGroup, byte inRound ) {
			String curMethodName = "LiveWebHandler: sendDisableCurrentSkier: ";
			
			try {
				Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "sanctionId", inSanctionId }
					, { "memberId", inMemberId }
					, { "event", inEvent }
					, { "ageGroup", inAgeGroup }
					, { "round", inRound }
				};

				addLiveWebMsgSend( inSanctionId, "DisableCurrentSkier", JsonConvert.SerializeObject( sendMsg ) );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0} Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg );
				return false;
			}
		}

		/*
		 */
		public static bool sendDisableSkiers( String inEvent, String inSanctionId, byte inRound, String inEventGroup ) {
			String curMethodName = "LiveWebHandler: sendDisableSkiers: ";
			try {
				Dictionary<string, dynamic> sendMsg = new Dictionary<string, dynamic> {
					{ "sanctionId", inSanctionId }
					, { "event", inEvent }
					, { "eventGroup", inEventGroup }
					, { "round", inRound }
				};

				addLiveWebMsgSend( inSanctionId, "DisableSkiers", JsonConvert.SerializeObject( sendMsg ) );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0} Exception encountered {1}", curMethodName, ex.Message );
				Log.WriteFile( curErrMsg );
				MessageBox.Show( curErrMsg );
				return false;
			}
		}

		/*
		 */
		private static void addLiveWebMsgSend( String inSanctionId, String msgType, String msgData ) {
			String curMsgData = HelperFunctions.stringReplace( msgData, HelperFunctions.SingleQuoteDelim, "''" );
			int curHashCode = msgData.GetHashCode();

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select MsgDataHash From LiveWebMsgSend " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND MsgType = '{1}' AND MsgDataHash = {2}"
				, inSanctionId, msgType, curHashCode ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return;

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert LiveWebMsgSend ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgDataHash, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( String.Format( "'{0}', '{1}', {2}, '{3}'", inSanctionId, msgType, curHashCode, curMsgData ) );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
