using System;
using System.Data;
using System.Deployment.Application;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

using BoatPathMonitorSim.Common;

namespace BoatPathMonitorSim.Message {
	public partial class Controller : Form {
		public static readonly String myDisplayDateFormat = "ddd MMM dd H:mm:ss";
		private static readonly Font myMsgFontBold = new Font( "Arial Narrow", 12, FontStyle.Bold );
		private static readonly int myCountHeartBeatFailedMax = 4;

		private DataTable myMonitorDataTable = null;
		private int myCountConnectCheckFailed = 0;
		private int myCountHeartBeatFailed = 0;
		private int myHeartBeatViewRowIdx = 0;
		private DateTime myLastMsgGetTime;

		private Timer myConnectTimer = null;
		private Timer myHeartBeatTimer = null;
		private Timer myMsgTimer = null;

		public Controller() {
			InitializeComponent();
		}

		private void Controller_Load( object sender, EventArgs e ) {
			String curDeployVersion = "";
			try {
				curDeployVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
			} catch {
				curDeployVersion = "Not available";
			}
			this.Text += " - " + curDeployVersion;
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) this.Text = Properties.Settings.Default.AppTitle;
			if ( Properties.Settings.Default.MessageController_Width > 0 ) this.Width = Properties.Settings.Default.MessageController_Width;
			if ( Properties.Settings.Default.MessageController_Height > 0 ) this.Height = Properties.Settings.Default.MessageController_Height;
			if ( Properties.Settings.Default.MessageController_Location.X > 0
				&& Properties.Settings.Default.MessageController_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.MessageController_Location;
			}

			WaterSkiConnectButton.Visible = true;
			DisconnectButton.Visible = false;
			ShowPinButton.Visible = false;
			ViewAppsButton.Enabled = false;

			Timer curTimerObj = new Timer() { Interval = 50 };
			curTimerObj.Tick += new EventHandler( execConnectDialog );
			curTimerObj.Start();
		}

		private void Controller_FormClosing( object sender, FormClosingEventArgs e ) {
			String curMethodName = "Controller: Controller_FormClosing: ";
			if ( Listener.isWscConnected ) {
				Log.WriteFile( String.Format( "{0}Exiting WaterSkiConnect handler", curMethodName ) );
				try {
					terminateMonitors( "" );

				} catch ( Exception ex ) {
					MessageBox.Show( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
				}

			} else {
				Log.WriteFile( String.Format( "{0}Exit byassed WaterSkiConnect handler not connected", curMethodName ) );
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
			Log.WriteFile( "Controller_FormClosed: WaterSkiConnect handler closed" );
		}

		private void WaterSkiConnectButton_Click( object sender, EventArgs e ) {
			execConnectDialog( null, null );
		}

		private void ShowPinButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Controller: DisconnectButton_Click: ";
			try {
				Listener.showPin();
			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
			}
		}

		private void ViewAppsButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Controller: ViewAppsButton_Click: ";
			try {
				Transmitter.whoseInTheRoom();

			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message ) );
			}
		}

		private void DisconnectButton_Click( object sender, EventArgs e ) {
			terminateMonitors( "" );
		}
		
		private void execConnectDialog( object sender, EventArgs e ) {
			String curMethodName = "Controller: execConnectDialog: ";
			if ( sender != null ) {
				Timer curTimerObj = (Timer)sender;
				curTimerObj.Stop();
				curTimerObj.Tick -= new EventHandler( execConnectDialog );
			}

			// Display the form as a modal dialog box.
			ConnectDialog connectDialog = new ConnectDialog() {
				windowLocation = new System.Drawing.Point( this.Location.X + 100, this.Location.Y + 100 )
			};
			DialogResult connectDialogResult = connectDialog.ShowDialog();
			if ( connectDialogResult == DialogResult.OK ) {
				if ( connectDialog.dialogCommand.Equals( "Connected" ) ) {
					execHandlerConnectTasks();

				} else if ( connectDialog.dialogCommand.Equals( "Disconnected" ) ) {
					try {
						terminateMonitors( "" );

					} catch ( Exception ex ) {
						String curMsg = String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message );
						Log.WriteFile( curMsg );
						MessageBox.Show( curMsg );
					}
				}
			}
		}

		private void execHandlerConnectTasks() {
			String curMethodName = "Controller: execHandlerConnectTasks: ";
			bool boldMsg = false, errorMsg = false ;
			String curMsg = "";
			myCountConnectCheckFailed = 0;

			try {
				ActivateTournament();

				curMsg = String.Format( "{0}attempting connection to WaterSkiConnect for tournament {1} / {2}"
					, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, boldMsg, errorMsg );

				HelperFunctions.cleanMsgQueues();
				curMsg = String.Format( "{0}message queues cleaned for {1} / {2}"
					, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, boldMsg, errorMsg );

				Task.Factory.StartNew( () => ListenHandler.startMessageHandler() );
				curMsg = String.Format( "{0}activating ListenHandler for {1} / {2}"
					, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, boldMsg, errorMsg );

				//Task.Factory.StartNew( () => Listener.startWscListener( ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId ) );
				Listener.startListener();
				curMsg = String.Format( "{0}starting Listener for {1} / {2}"
					, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, boldMsg, errorMsg );

				//Task.Factory.StartNew( () => Transmitter.startWscTransmitter( ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId ) );
				Transmitter.startTransmitter();
				curMsg = String.Format( "{0}starting Transmitter for {1} / {2}"
					, curMethodName, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, boldMsg, errorMsg );

				// Create a timer with 2.5 second interval
				myConnectTimer = new Timer() { Interval = 2500 };
				myConnectTimer.Tick += new EventHandler( waitForMonitorConnection );
				myConnectTimer.Start();

				WaterSkiConnectButton.Enabled = false;
				DisconnectButton.Enabled = true;
				ShowPinButton.Enabled = true;
				ViewAppsButton.Enabled = true;

			} catch ( Exception ex ) {
				curMsg = String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );
				MessageBox.Show( curMsg );

				WaterSkiConnectButton.Enabled = true;
				DisconnectButton.Enabled = false;
				ShowPinButton.Enabled = false;
				ViewAppsButton.Enabled = false;
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

				WaterSkiConnectButton.Enabled = false;
				DisconnectButton.Enabled = true;
				ShowPinButton.Enabled = true;
				ViewAppsButton.Enabled = true;

				myLastMsgGetTime = System.DateTime.Now;
				// Create a timer with 30 second interval
				myMsgTimer = new Timer() { Interval = 10000 };
				myMsgTimer.Tick += new EventHandler( displayAvailableMessages );
				myMsgTimer.Start();

			} else {
				curMsg = String.Format( "{0}Unable to access tournament data for sanction {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				Log.WriteFile( curMsg );
				addViewMessage( curMsg, true, true );

				WaterSkiConnectButton.Enabled = true;
				DisconnectButton.Enabled = false;
				ShowPinButton.Enabled = false;
				ViewAppsButton.Enabled = false;
			}
		}

		private void showHeartBeatMsg() {
			String curMethodName = "Controller: showHeartBeatMsg: ";
			bool addViewRows = false;
			int curViewIdx = myHeartBeatViewRowIdx - 1;
			if ( myHeartBeatViewRowIdx >= MessageView.Rows.Count ) addViewRows = true;

			foreach ( DataRow curMonitorHeartBeat in myMonitorDataTable.Rows ) {
				String curMonitorName = (String)curMonitorHeartBeat["MonitorName"];
				DateTime curHeartBeat = (DateTime)curMonitorHeartBeat["HeartBeat"];
				String curMsg = String.Format( "{0}{1} heart beat {2} ", curMethodName, curMonitorName, curHeartBeat.ToString( myDisplayDateFormat ) );
				if ( addViewRows ) {
					addViewMessage( curMsg, true, false );
				
				} else {
					DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
					curViewRow.Cells["Message"].Value = curMsg;
					curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
					curViewIdx++;
				}
			}
		}

		private void addViewMessage(String msg, bool boldMessage, bool errorMsg ) {
			Log.WriteFile(msg);
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			if ( boldMessage ) curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			if ( errorMsg ) curViewRow.Cells["Message"].Style.ForeColor = Color.Red;
		}

		private void waitForMonitorConnection( object sender, EventArgs e ) {
			String curMethodName = "Controller: waitForMonitorConnection: ";
			String connectStatus = "", curMsg = "";

			DataRow curHeatBeatRow = HelperFunctions.getMonitorHeartBeat( "Listener" );
			if ( curHeatBeatRow != null ) {
				curMsg = String.Format( "{0}{1} heart beat {2} ", curMethodName, (String)curHeatBeatRow["MonitorName"], ( (DateTime)curHeatBeatRow["HeartBeat"] ).ToString( myDisplayDateFormat ) );
				Log.WriteFile( curMsg );

				curHeatBeatRow = HelperFunctions.getMonitorHeartBeat( "Transmitter" );
			}
			if ( curHeatBeatRow == null ) {
				myCountConnectCheckFailed++;
				if ( myCountConnectCheckFailed < 120 ) return;

				connectStatus = "failed";
				curMsg = String.Format( "{0}Listener connection {1} for {2} / {3}"
					, curMethodName, connectStatus, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, true, true );

				curMsg = String.Format( "{0}Transmitter connection {1} for {2} / {3}"
					, curMethodName, connectStatus, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
				addViewMessage( curMsg, true, true );

				myConnectTimer.Stop();
				myConnectTimer.Tick += new EventHandler( waitForMonitorConnection );
				myConnectTimer = null;
				return;
			}

			curMsg = String.Format( "{0}{1} heart beat {2} ", curMethodName, (String)curHeatBeatRow["MonitorName"], ( (DateTime)curHeatBeatRow["HeartBeat"] ).ToString( myDisplayDateFormat ) );
			Log.WriteFile( curMsg );

			myConnectTimer.Stop();
			myConnectTimer.Tick += new EventHandler( waitForMonitorConnection );
			myConnectTimer = null;

			connectStatus = "completed";
			curMsg = String.Format( "{0}Listener connection {1} for {2} / {3}", curMethodName, connectStatus, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
			addViewMessage( curMsg, false, false );

			curMsg = String.Format( "{0}Transmitter connection {1} for {2} / {3}", curMethodName, connectStatus, ConnectMgmtData.sanctionNum, ConnectMgmtData.eventSubId );
			addViewMessage( curMsg, false, false );

			myCountHeartBeatFailed = 0;
			myMonitorDataTable = HelperFunctions.getMonitorHeartBeatAll();
			myHeartBeatViewRowIdx = MessageView.Rows.Count;
			showHeartBeatMsg();

			WaterSkiConnectButton.Visible = false;
			DisconnectButton.Visible = true;
			ShowPinButton.Visible = true;
			ViewAppsButton.Enabled = true;

			// Create a timer with 5 minute interval.
			myHeartBeatTimer = new Timer() { Interval = 300000 };
			myHeartBeatTimer.Tick += new EventHandler( checkMonitorHeartBeat );
			myHeartBeatTimer.Start();
		}

		private void checkMonitorHeartBeat( object sender, EventArgs e ) {
			String curMethodName = "Controller: checkMonitorHeartBeat: ";
			DataTable curMonitorDataTable = HelperFunctions.getMonitorHeartBeatAll();
			if ( curMonitorDataTable.Rows.Count != 3 ) {
				myCountHeartBeatFailed++;
				if ( myCountHeartBeatFailed > myCountHeartBeatFailedMax ) terminateMonitors( "All" );
				return;
			}
			foreach (  DataRow curMonitorHeartBeat in curMonitorDataTable.Rows ) {
				String curMonitorName = (String)curMonitorHeartBeat["MonitorName"];
				DataRow[] prevMonitorHeartBeat = myMonitorDataTable.Select( "MonitorName = '" + curMonitorName + "'" );
				if ( prevMonitorHeartBeat.Length > 0 ) {
					if ( (DateTime)curMonitorHeartBeat["HeartBeat"] <= (DateTime)prevMonitorHeartBeat[0]["HeartBeat"] ) {
						myCountHeartBeatFailed++;
						Log.WriteFile( String.Format( "{0}Heartbeat for monitor {1} is out of date, failed count = {2}", curMethodName, curMonitorName, myCountHeartBeatFailed ) );
						if ( myCountHeartBeatFailed > myCountHeartBeatFailedMax ) {
							terminateMonitors( curMonitorName );
							return;
						}
					}
				}
			}

			myMonitorDataTable = curMonitorDataTable;
			showHeartBeatMsg();
			myCountHeartBeatFailed = 0;
			return;
		}

		private void displayAvailableMessages( object sender, EventArgs e ) {
			String curMsg = "";
			myMsgTimer.Stop();
			int curViewIdx = MessageView.Rows.Count - 1;
			if ( MessageView.Rows.Count > 0 ) MessageView.CurrentCell = MessageView.Rows[curViewIdx].Cells["Message"];

			Cursor.Current = Cursors.WaitCursor;
			DataTable curDataTable = getMessages( myLastMsgGetTime );
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				//PK, SanctionId, MsgAction, MsgType, MsgData, InsertDate 
				curMsg = String.Format( "PK={0}, InsertDate={1}, MsgAction={2}, MsgType={3}, MsgData={4}"
					, HelperFunctions.getDataRowColValue( curDataRow, "PK", "" )
					, ( (DateTime)curDataRow["InsertDate"] ).ToString( myDisplayDateFormat )
					, HelperFunctions.getDataRowColValue( curDataRow, "MsgAction", "" )
					, HelperFunctions.getDataRowColValue( curDataRow, "MsgType", "" )
					, HelperFunctions.getDataRowColValue( curDataRow, "MsgData", "" )
					);
				if ( curMsg.Contains( "connect_confirm_listener" ) ) addViewMessage( curMsg, true, false );
				else addViewMessage( curMsg, false, false );
			}

			curViewIdx = MessageView.Rows.Count - 1;
			if ( MessageView.Rows.Count > 0 ) {
				MessageView.FirstDisplayedScrollingRowIndex = curViewIdx;
				MessageView.Rows[curViewIdx].Selected = true;
				MessageView.Rows[curViewIdx].Cells[0].Selected = true;
				MessageView.CurrentCell = MessageView.Rows[curViewIdx].Cells["Message"];

				int curRowPos = curViewIdx + 1;
				RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + MessageView.Rows.Count.ToString();

			} else {
				RowStatusLabel.Text = "";
			}
			Cursor.Current = Cursors.Default;

			// Create a timer with 5 second interval
			myLastMsgGetTime = System.DateTime.Now;
			myMsgTimer = new Timer() { Interval = 10000 };
			myMsgTimer.Tick += new EventHandler( displayAvailableMessages );
			myMsgTimer.Start();
		}

		private void terminateMonitors( String curMonitorName ) {
			String curMethodName = "Controller: terminateMonitors: ";
			Log.WriteFile( String.Format( "{0}No heartbeat for monitor {1}", curMethodName, curMonitorName ) );

			if ( myConnectTimer != null ) {
				myConnectTimer.Stop();
				myConnectTimer.Tick += new EventHandler( waitForMonitorConnection );
			}
			if ( myHeartBeatTimer != null ) {
				myHeartBeatTimer.Stop();
				myHeartBeatTimer.Tick -= new EventHandler( checkMonitorHeartBeat );
			}
			String msg = "";
			if ( curMonitorName.Equals( "All" ) ) {
				msg = "HeartBeat for monitors unavailable.  Assuming connection has failed.  Terminating monitoring.";
			
			} else if ( curMonitorName.Length > 0 ) {
				msg = String.Format( "{0}HeartBeat for monitor {1} not available.  Assuming connection has failed.  Terminating monitoring.", curMethodName, curMonitorName );
			}
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			curViewRow.Cells["Message"].Style.ForeColor = Color.Red;

			ListenHandler.disconnect();
			Listener.disconnect();
			Transmitter.disconnect(0);

			deleteMonitorMessages();
			myMonitorDataTable = null;
			WaterSkiConnectButton.Visible = true;
			WaterSkiConnectButton.Enabled = true;
			DisconnectButton.Visible = false;
			ShowPinButton.Visible = false;
			ViewAppsButton.Enabled = false;

			myCountHeartBeatFailed = 0;

			if ( msg.Length > 0 ) MessageBox.Show( msg );
		}

		private DataTable getMessages( DateTime inLastMsgTime ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select PK, SanctionId, MsgAction, MsgType, MsgData, InsertDate " );
			curSqlStmt.Append( "From WscMonitorMsg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}' AND InsertDate >= '{1}'"
				, ConnectMgmtData.sanctionNum, inLastMsgTime.ToString( "yyyy-MM-dd HH:mm:ss" ) ) );
			curSqlStmt.Append( " Order By InsertDate " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			return curDataTable;
		}

		private void deleteMonitorMessages() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Delete From WscMonitorMsg " );
			curSqlStmt.Append( String.Format( "Where SanctionId = '{0}'", ConnectMgmtData.sanctionNum ) );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );
		}
	}
}

