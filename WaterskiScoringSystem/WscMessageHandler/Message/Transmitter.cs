using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	class Transmitter {
		private static System.Timers.Timer heartBeatTimer = null;
		private static System.Timers.Timer readMessagesTimer = null;

		public static void startWscTransmitter() {
			String curMethodName = "Transmitter: startWscTransmitter: ";
			Log.WriteFile( String.Format( "{0}start in progress, {1}/{2}", curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId ) );
			
			try {
				HelperFunctions.updateMonitorHeartBeat( "Transmitter" );

				// Create a timer with 5 minute interval.
				heartBeatTimer = new System.Timers.Timer( 300000 );
				heartBeatTimer.Elapsed += checkMonitorHeartBeat;
				heartBeatTimer.AutoReset = true;
				heartBeatTimer.Enabled = true;

				// Create a timer with 2 second interval.
				readMessagesTimer = new System.Timers.Timer( 2000 );
				readMessagesTimer.Elapsed += readSendMessages;
				readMessagesTimer.AutoReset = false;
				readMessagesTimer.Enabled = true;

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}

		private static void checkMonitorHeartBeat( object sender, EventArgs e ) {
			ConnectMgmtData.socketClient.EmitAsync( "heartbeat", "WscMessageHandler: Transmitter: checkMonitorHeartBeat" );
			Log.WriteFile( "Transmitter: checkMonitorHeartBeat: Emit heartbeat" );
			if ( ConnectMgmtData.socketClient.Connected ) HelperFunctions.updateMonitorHeartBeat( "Transmitter" );
		}

		private static void readSendMessages( object sender, EventArgs e ) {
			readMessagesTimer.Stop();
			readMessagesTimer.Elapsed -= readSendMessages;

			checkForMsgToSend();
		}

		private static void checkForMsgToSend() {
			String curMethodName = "Transmitter: checkForMsgToSend: ";

			try {
				DataTable curDataTable = getWscMsgSend();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					if ( ( (String)curDataRow["MsgType"] ).Equals( "Exit" ) ) {
						disconnect( (int)curDataRow["PK"] );
						return;
					}

					ConnectMgmtData.socketClient.EmitAsync( (String)curDataRow["MsgType"], curDataRow["MsgData"] );
					Log.WriteFile( String.Format( "{0}PK: {1} MsgType: {2} MsgData: {3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] ) );
					removeWscMsgSent( (int)curDataRow["PK"] );
				}

				readMessagesTimer = new System.Timers.Timer( 2000 );
				readMessagesTimer.Elapsed += readSendMessages;
				readMessagesTimer.AutoReset = false;
				readMessagesTimer.Enabled = true;

				return;

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );
				return;
			}
		}

		public static int disconnect( int inMsgPk ) {
			String curMethodName = "Transmitter: disconnect: ";
			Log.WriteFile( curMethodName + "EXIT request received and being processed" );

			if ( ConnectMgmtData.socketClient != null ) {
				Log.WriteFile( curMethodName + "Disconnected" );
				ConnectMgmtData.socketClient.DisconnectAsync();
			}

			if ( heartBeatTimer != null ) {
				heartBeatTimer.Stop();
				heartBeatTimer.Elapsed -= checkMonitorHeartBeat;
			}
			if ( readMessagesTimer != null ) {
				readMessagesTimer.Stop();
				readMessagesTimer.Elapsed -= readSendMessages;
			}

			if ( inMsgPk > 0 ) removeWscMsgSent( inMsgPk );
			HelperFunctions.deleteMonitorHeartBeat( "Transmitter" );
			Log.WriteFile( curMethodName + "EXIT request processed and all threads closed" );
			return 1;
		}

		private static DataTable getWscMsgSend() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MsgType, MsgData, CreateDate " );
			curSqlStmt.Append( "FROM WscMsgSend " );
			curSqlStmt.Append( "WHERE SanctionId = '" + ConnectMgmtData.sanctionNum + "' " );
			curSqlStmt.Append( "Order by CreateDate " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}
		
		private static void removeWscMsgSent( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM WscMsgSend Where PK = " + pkid );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}
