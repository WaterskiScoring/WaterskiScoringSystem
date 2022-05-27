using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using HttpMessageHandler.Common;
using HttpMessageHandler.Externalnterface;

namespace HttpMessageHandler.Message {
	public partial class Controller : Form {
		private static readonly String myDisplayDateFormat = "ddd MMM dd H:mm:ss";
		private static readonly Font myMsgFontBold = new Font( "Arial Narrow", 12, FontStyle.Bold );
		private static readonly int myCountHeartBeatFailedMax = 4;

		private DataTable myMonitorDataTable = null;
		private int myCountConnectCheckFailed = 0;
		private int myCountHeartBeatFailed = 0;
		private int myHeartBeatViewRowIdx = 0;

		private static System.Timers.Timer myConnectTimer = null;
		private static System.Timers.Timer myHeartBeatTimer = null;
		private static System.Timers.Timer myReadMessagesTimer = null;

		public Controller() {
			InitializeComponent();
		}

		private void Controller_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) this.Text = Properties.Settings.Default.AppTitle;
			if ( Properties.Settings.Default.MessageController_Width > 0 ) this.Width = Properties.Settings.Default.MessageController_Width;
			if ( Properties.Settings.Default.MessageController_Height > 0 ) this.Height = Properties.Settings.Default.MessageController_Height;
			if ( Properties.Settings.Default.MessageController_Location.X > 0
				&& Properties.Settings.Default.MessageController_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.MessageController_Location;
			}

			ConnectButton.Enabled = true;
			DisconnectButton.Enabled = false;

			myConnectTimer = new System.Timers.Timer( 100 );
			myConnectTimer.Elapsed += execConnectDialog;
			myConnectTimer.AutoReset = false;
			myConnectTimer.Enabled = true;
		}

		private void Controller_FormClosing( object sender, FormClosingEventArgs e ) {
			String curMethodName = "Controller: Controller_FormClosing: ";
			Log.WriteFile( String.Format( "{0}Exiting LiveWebConnect handler", curMethodName ) );
			try {
				terminateMonitors( "" );

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

		private void execConnectDialog( object sender, EventArgs e ) {
			String curMethodName = "Controller: execConnectDialog: ";

			// Display the form as a modal dialog box.
			LiveWebConnectDialog connectDialog = new LiveWebConnectDialog() {
				windowLocation = new System.Drawing.Point( this.Location.X + 100, this.Location.Y + 100 )
			};
			DialogResult connectDialogResult = connectDialog.ShowDialog();
			if ( connectDialogResult == DialogResult.OK ) {
				if ( connectDialog.dialogCommand.Equals( "Connected" ) ) {
					ExportLiveWeb.LiveWebLocation = connectDialog.WebLocation;
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
			bool boldMsg = false, errorMsg = false;
			String curMsg = "";
			myCountConnectCheckFailed = 0;

			try {
				ActivateTournament();

				curMsg = String.Format( "{0}attempting connection to LiveWebConnect for tournament {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, boldMsg, errorMsg );

				HelperFunctions.cleanMsgQueues();
				curMsg = String.Format( "{0}message queues cleaned for {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, boldMsg, errorMsg );

				startTransmitter();
				curMsg = String.Format( "{0}starting Transmitter for {1}"
					, curMethodName, ConnectMgmtData.sanctionNum );
				addViewMessage( curMsg, boldMsg, errorMsg );

				ConnectButton.Enabled = false;
				DisconnectButton.Enabled = true;

			} catch ( Exception ex ) {
				curMsg = String.Format( "{0}Exception encountered: {1}", curMethodName, ex.Message );
				Log.WriteFile( curMsg );
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
				Log.WriteFile( curMsg );
				addViewMessage( curMsg, true, true );

				ConnectButton.Enabled = true;
				DisconnectButton.Enabled = false;
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

		private void addViewMessage( String msg, bool boldMessage, bool errorMsg ) {
			Log.WriteFile( msg );
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			if ( boldMessage ) curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			if ( errorMsg ) curViewRow.Cells["Message"].Style.ForeColor = Color.Red;
		}

		private void startTransmitter() {
			String curMethodName = "Controller: startTransmitter: ";
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

		private void checkMonitorHeartBeat( object sender, EventArgs e ) {
			String curMethodName = "Controller: checkMonitorHeartBeat: ";
			DataTable curMonitorDataTable = HelperFunctions.getMonitorHeartBeatAll();
			if ( curMonitorDataTable.Rows.Count != 3 ) {
				myCountHeartBeatFailed++;
				if ( myCountHeartBeatFailed > myCountHeartBeatFailedMax ) terminateMonitors( "All" );
				return;
			}
			foreach ( DataRow curMonitorHeartBeat in curMonitorDataTable.Rows ) {
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

			closeTransmitter( 0 );

			myMonitorDataTable = null;
			ConnectButton.Visible = true;
			ConnectButton.Enabled = true;
			DisconnectButton.Visible = false;

			myCountHeartBeatFailed = 0;

			ExportLiveWeb.LiveWebLocation = "";

			if ( msg.Length > 0 ) MessageBox.Show( msg );
		}
	}
}
