using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

using LiveWebMessageHandler.Common;
using LiveWebMessageHandler.Externalnterface;

namespace LiveWebMessageHandler.Message {
	public partial class Controller : Form {
		private static readonly String myDisplayDateFormat = "ddd MMM dd H:mm:ss";
		private static readonly Font myMsgFontBold = new Font( "Arial Narrow", 12, FontStyle.Bold );
		private static int myReadSendSleepTimeDefault = 2000;
		private static int myReadSendError1SleepTime = 60000;
		private static int myReadSendError2SleepTime = 300000;
		private static int myReadSendError1Count = 3;
		private static int myReadSendError2Count = 10;

		private int myReadSendSleepTime = myReadSendSleepTimeDefault;
		private int myReadSendErrorCount = 0;

		public Controller() {
			InitializeComponent();
		}

		private void Controller_Load( object sender, EventArgs e ) {
			String curMethodName = "Controller: Controller_Load: ";
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) this.Text = Properties.Settings.Default.AppTitle;
			if ( Properties.Settings.Default.MessageController_Width > 0 ) this.Width = Properties.Settings.Default.MessageController_Width;
			if ( Properties.Settings.Default.MessageController_Height > 0 ) this.Height = Properties.Settings.Default.MessageController_Height;
			if ( Properties.Settings.Default.MessageController_Location.X > 0
				&& Properties.Settings.Default.MessageController_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.MessageController_Location;
			}

			ConnectButton.Enabled = true;
			DisconnectButton.Enabled = false;
			QueueView.Visible = false;
			QueueView.Enabled = false;
			QueueView.Location = new System.Drawing.Point( MessageView.Location.X + 25, MessageView.Location.Y + 50 );

			String curMsg = String.Format( "{0}Open tournament: {1}", curMethodName, Properties.Settings.Default.SanctionNum );
			addViewMessage( curMsg, true, false );

			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 50;
			curTimerObj.Tick += new EventHandler( execHandlerConnectTasks );
			curTimerObj.Start();
		}

		private void Controller_FormClosing( object sender, FormClosingEventArgs e ) {
			String curMethodName = "Controller: Controller_FormClosing: ";
			Log.WriteFile( String.Format( "{0}Exiting LiveWebConnect handler", curMethodName ) );
			try {
				terminateMonitors( "LiveWeb" );

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
			}
		}

		private void Controller_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) Properties.Settings.Default.AppTitle = this.Text;
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.MessageController_Width = this.Size.Width;
				Properties.Settings.Default.MessageController_Height = this.Size.Height;
				Properties.Settings.Default.MessageController_Location = this.Location;
			}
			Properties.Settings.Default.Save();
			Log.WriteFile( "Controller_FormClosed: connect handler closed" );
		}

		private void execHandlerConnectTasks( object sender, EventArgs e ) {
			String curMethodName = "Controller: execHandlerConnectTasks: ";
			bool boldMsg = false, errorMsg = false;
			String curMsg = "";

			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( execHandlerConnectTasks );

			try {
				ActivateTournament();

				curMsg = String.Format( "{0}Connecting to LiveWeb for tournament {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, boldMsg, errorMsg );

				startTransmitter();
				curMsg = String.Format( "{0}starting Transmitter for {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, true, errorMsg );

				ConnectButton.Enabled = false;
				DisconnectButton.Enabled = true;

			} catch ( Exception ex ) {
				curMsg = String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message );
				MessageBox.Show( curMsg );

				ConnectButton.Enabled = true;
				DisconnectButton.Enabled = false;
			}
		}

		private void ActivateTournament() {
			String curMethodName = "Controller: ActivateTournament: ";
			String curMsg = "";

			if ( ConnectMgmtData.initConnectMgmtData() ) {
				curMsg = String.Format( "{0}Connected to database: {1}", curMethodName, Properties.Settings.Default.DatabaseFilename );
				addViewMessage( curMsg, false, false );

				curMsg = String.Format( "{0}Tournament open: {1}", curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, false, false );

				curMsg = String.Format( "{0}DataDirectory: {1}", curMethodName, Properties.Settings.Default.DataDirectory );
				addViewMessage( curMsg, false, false );

				ConnectButton.Enabled = false;
				DisconnectButton.Enabled = true;

			} else {
				curMsg = String.Format( "{0}Unable to access tournament data for sanction {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, true, true );

				ConnectButton.Enabled = true;
				DisconnectButton.Enabled = false;
				throw new Exception( curMsg );
			}
		}

		private void addViewMessage( String msg, bool boldMessage, bool errorMsg ) {
			//Log.WriteFile( msg );
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			if ( boldMessage ) curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			if ( errorMsg ) curViewRow.Cells["Message"].Style.ForeColor = Color.Red;
		}

		private void startTransmitter() {
			String curMethodName = "Controller: startTransmitter: ";
			Log.WriteFile( String.Format( "{0}start in progress, {1}", curMethodName, ConnectMgmtData.sanctionNum ) );
			myReadSendSleepTime = myReadSendSleepTimeDefault;
			myReadSendErrorCount = 0;

			try {
				// Create a timer with 2 second interval.
				Timer curReadTimerObj = new Timer();
				curReadTimerObj.Interval = myReadSendSleepTime;
				curReadTimerObj.Tick += new EventHandler( readSendMessages );
				curReadTimerObj.Start();

			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}: {2}", curMethodName, ex.Message, ex.StackTrace );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );
			}
		}
		
		private void readSendMessages( object sender, EventArgs e ) {
			String curMethodName = "Controller: readSendMessages: ";
			Timer curReadTimerObj = (Timer)sender;
			curReadTimerObj.Stop();
			bool curReturn = true;

			try {
				DataTable curDataTable = getLiveWebMsgSend();
				foreach ( DataRow curDataRow in curDataTable.Rows ) {
					String msgType = (String)curDataRow["MsgType"];
					if ( msgType.Equals( "Exit" ) ) {
						removeLiveWebMsgSent( (int)curDataRow["PK"] );
						terminateMonitors( "LiveWeb" );
						return;
					
					} else if ( msgType.Equals( "CurrentSkier" ) ) {
						curReturn = handleCurrentSkierMsg( curDataRow );

					} else if ( msgType.Equals( "CurrentSkiers" ) ) {
						curReturn = handleCurrentSkiersMsg( curDataRow );

					} else if ( msgType.Equals( "DisableCurrentSkier" ) ) {
						curReturn = handleDisableCurrentSkierMsg( curDataRow );

					} else if ( msgType.Equals( "DisableSkiers" ) ) {
						curReturn = handleDisableSkiersMsg( curDataRow );
					}
					if ( !curReturn ) break;
				}
				
			} catch ( Exception ex ) {
				String curMsg = String.Format( "{0}Exception encounter {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );
				curReturn = false;
			}

			if ( curReturn ) {
				myReadSendSleepTime = myReadSendSleepTimeDefault;
				myReadSendErrorCount = 0;

			} else {
				myReadSendErrorCount++;
				if ( myReadSendErrorCount > myReadSendError1Count ) {
					if ( myReadSendErrorCount > myReadSendError2Count ) {
						myReadSendSleepTime = myReadSendError2SleepTime;
					} else {
						myReadSendSleepTime = myReadSendError1SleepTime;
					}
				}
			}

			curReadTimerObj.Interval = myReadSendSleepTime;
			curReadTimerObj.Start();

			return;
		}

		private bool handleCurrentSkierMsg( DataRow curDataRow ) {
			String curMethodName = "Controller: handleCurrentSkierMsg: ";
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( (String)curDataRow["MsgData"] );

				String curSanctionId = HelperFunctions.getAttributeValue( curMsgDataList, "sanctionId" );
				String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "memberId" );
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "event" );
				String curAgeGroup = HelperFunctions.getAttributeValue( curMsgDataList, "ageGroup" );
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );
				Int16 curPassNumber = Convert.ToInt16( HelperFunctions.getAttributeValueNum( curMsgDataList, "passNumber" ).ToString( "#0" ) );

				ExportLiveWeb.exportCurrentSkier( curSanctionId, curEvent, curMemberId, curAgeGroup, (byte)curRound, curPassNumber );

				String curMsg = String.Format( "{0}Message Sent: PK={1} MsgType={2} MsgData={3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] );
				addViewMessage( curMsg, false, false );
				Log.WriteFile( curMsg );

				removeLiveWebMsgSent( (int)curDataRow["PK"] );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Request failed: {1}", curMethodName, ex.Message );
				addViewMessage( curErrMsg, true, true );
				Log.WriteFile( curErrMsg );
				return false;
			}
		}

		private bool handleCurrentSkiersMsg( DataRow curDataRow ) {
			String curMethodName = "Controller: handleCurrentSkiersMsg: ";
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( (String)curDataRow["MsgData"] );

				String curSanctionId = HelperFunctions.getAttributeValue( curMsgDataList, "sanctionId" );
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "event" );
				String curEventGroup = HelperFunctions.getAttributeValue( curMsgDataList, "eventGroup" );
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );

				/*
				String inEvent, String inSanctionId, byte inRound, String inEventGroup
				*/
				ExportLiveWeb.exportCurrentSkiers( curEvent, curSanctionId, (byte)curRound, curEventGroup );

				String curMsg = String.Format( "{0}Message Sent: PK={1} MsgType={2} MsgData={3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] );
				addViewMessage( curMsg, false, false );
				Log.WriteFile( curMsg );

				removeLiveWebMsgSent( (int)curDataRow["PK"] );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Request failed: {1}", curMethodName, ex.Message );
				addViewMessage( curErrMsg, true, true );
				Log.WriteFile( curErrMsg );
				return false;
			}
		}

		private bool handleDisableCurrentSkierMsg( DataRow curDataRow ) {
			String curMethodName = "Controller: handleDisableCurrentSkierMsg: ";
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( (String)curDataRow["MsgData"] );

				String curSanctionId = HelperFunctions.getAttributeValue( curMsgDataList, "sanctionId" );
				String curMemberId = HelperFunctions.getAttributeValue( curMsgDataList, "memberId" );
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "event" );
				String curAgeGroup = HelperFunctions.getAttributeValue( curMsgDataList, "ageGroup" );
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );

				/*
					String inEvent, String inSanctionId, byte inRound, String inEventGroup
				*/
				ExportLiveWeb.disableCurrentSkier( curSanctionId, curEvent, curMemberId, curAgeGroup, (byte)curRound );

				String curMsg = String.Format( "{0}Message Sent: PK={1} MsgType={2} MsgData={3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] );
				addViewMessage( curMsg, false, false );
				Log.WriteFile( curMsg );

				removeLiveWebMsgSent( (int)curDataRow["PK"] );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Request failed: {1}", curMethodName, ex.Message );
				addViewMessage( curErrMsg, true, true );
				Log.WriteFile( curErrMsg );
				return false;
			}
		}

		private bool handleDisableSkiersMsg( DataRow curDataRow ) {
			String curMethodName = "Controller: handleDisableSkiersMsg: ";
			Dictionary<string, object> curMsgDataList = null;

			try {
				curMsgDataList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>( (String)curDataRow["MsgData"] );

				String curSanctionId = HelperFunctions.getAttributeValue( curMsgDataList, "sanctionId" );
				String curEvent = HelperFunctions.getAttributeValue( curMsgDataList, "event" );
				String curEventGroup = HelperFunctions.getAttributeValue( curMsgDataList, "eventGroup" );
				int curRound = (int)HelperFunctions.getAttributeValueNum( curMsgDataList, "round" );

				ExportLiveWeb.disableSkiers( curEvent, curSanctionId, (byte)curRound, curEventGroup );

				String curMsg = String.Format( "{0}Message Sent: PK={1} MsgType={2} MsgData={3}", curMethodName, curDataRow["PK"], curDataRow["MsgType"], curDataRow["MsgData"] );
				addViewMessage( curMsg, false, false );
				Log.WriteFile( curMsg );

				removeLiveWebMsgSent( (int)curDataRow["PK"] );
				return true;

			} catch ( Exception ex ) {
				String curErrMsg = String.Format( "{0}Request failed: {1}", curMethodName, ex.Message );
				addViewMessage( curErrMsg, true, true );
				Log.WriteFile( curErrMsg );
				return false;
			}
		}

		private void terminateMonitors( String curMonitorName ) {
			String curMethodName = "Controller: terminateMonitors: ";
			String curMsg = String.Format( "{0}{1} Monitor closed", curMethodName, curMonitorName );
			Log.WriteFile( curMsg );
			
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = curMsg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			curViewRow.Cells["Message"].Style.ForeColor = Color.Red;

			ConnectButton.Visible = true;
			ConnectButton.Enabled = true;
			DisconnectButton.Enabled = false;
		}

		private DataTable getLiveWebMsgSend() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT PK, SanctionId, MsgType, MsgData, MsgDataHash, CreateDate " );
			curSqlStmt.Append( "FROM LiveWebMsgSend " );
			curSqlStmt.Append( "WHERE SanctionId = '" + ConnectMgmtData.sanctionNum + "' " );
			curSqlStmt.Append( "Order by CreateDate " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private void removeLiveWebMsgSent( int pkid ) {
			StringBuilder curSqlStmt = new StringBuilder( "Delete FROM LiveWebMsgSend Where PK = " + pkid );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		private void DisconnectButton_Click( object sender, EventArgs e ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Insert LiveWebMsgSend ( " );
			curSqlStmt.Append( "SanctionId, MsgType, MsgDataHash, MsgData, CreateDate " );
			curSqlStmt.Append( ") Values ( " );
			curSqlStmt.Append( String.Format( "'{0}', '{1}', {2}, '{3}'", ConnectMgmtData.sanctionNum, "Exit", 0, "Exit" ) );
			curSqlStmt.Append( ", getdate()" );
			curSqlStmt.Append( ")" );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}

		private void ConnectButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Controller: ConnectButton_Click: ";
			String curMsg = String.Format( "{0}Open tournament: {1}", curMethodName, Properties.Settings.Default.SanctionNum );
			addViewMessage( curMsg, true, false );

			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 50;
			curTimerObj.Tick += new EventHandler( execHandlerConnectTasks );
			curTimerObj.Start();

		}

		private void QueueButton_Click( object sender, EventArgs e ) {
			QueueView.Rows.Clear();
			QueueView.Visible = true;
			QueueView.Enabled = true;
			QueueView.Focus();

			DataTable curDataTable = getLiveWebMsgSend();
			foreach (DataRow curRow in curDataTable.Rows ) {
				int curViewIdx = QueueView.Rows.Add();
				DataGridViewRow curViewRow = QueueView.Rows[curViewIdx];
				curViewRow.Cells["QueueMsgType"].Value = HelperFunctions.getDataRowColValue( curRow, "MsgType", "" );
				curViewRow.Cells["QueueMsgData"].Value = HelperFunctions.getDataRowColValue( curRow, "MsgData", "" );
				curViewRow.Cells["MsgDataHash"].Value = HelperFunctions.getDataRowColValue( curRow, "MsgDataHash", "" );
				curViewRow.Cells["QueueCreateDate"].Value = HelperFunctions.getDataRowColValue( curRow, "CreateDate", "" );
			}
		}

		private void QueueView_Leave( object sender, EventArgs e ) {
			QueueView.Visible = false;
			QueueView.Enabled = false;
			QueueView.Rows.Clear();
		}
	}
}
