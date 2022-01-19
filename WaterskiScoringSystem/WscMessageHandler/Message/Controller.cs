using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;

using System.Windows.Forms;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	public partial class Controller : Form {
		public static readonly String myDisplayDateFormat = "ddd MMM dd H:mm:ss";
		private static readonly Font myMsgFontBold = new Font( "Arial Narrow", 12, FontStyle.Bold );
		private static readonly int myCountHeartBeatFailedMax = 4;

		private DataTable myMonitorDataTable = null;
		private int myCountHeartBeatFailed = 0;
		private int myHeartBeatViewRowIdx = 0;
		
		private Timer myHeartBeatTimer = null;

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
			MessageBox.Show( String.Format( "MessageController_Width={0} this.Size={1}"
				+ "\nMessageController_Height={2} this.SizeWidth={3}"
				+ "\nMessageController_Location={4} this.Location={5}"
				, Properties.Settings.Default.MessageController_Width, this.Size.Width
				, Properties.Settings.Default.MessageController_Height, this.Size.Height
				, Properties.Settings.Default.MessageController_Location.ToString(), this.Location.ToString() ) );

			Log.OpenFile();
			WaterSkiConnectButton.Visible = true;
			DisconnectButton.Visible = false;
			ShowPinButton.Visible = false;

			if ( HandlerDataAccess.DataAccessOpen() ) {
				int curViewIdx = MessageView.Rows.Add();
				DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
				curViewRow.Cells["Message"].Value = "Connected to database: " + Properties.Settings.Default.DatabaseFilename;
				curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );

				curViewIdx = MessageView.Rows.Add();
				curViewRow = MessageView.Rows[curViewIdx];
				curViewRow.Cells["Message"].Value = "Tournament open: " + Properties.Settings.Default.SanctionNum;
				curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );

				curViewIdx = MessageView.Rows.Add();
				curViewRow = MessageView.Rows[curViewIdx];
				curViewRow.Cells["Message"].Value = "DataDirectory: " + Properties.Settings.Default.DataDirectory;
				curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );

				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 50;
				curTimerObj.Tick += new EventHandler( execConnectDialog );
				curTimerObj.Start();

			} else {
				MessageBox.Show( "Database not found at the specified location " + Properties.Settings.Default.DatabaseConnectionString );
			}

		}

		private void Controller_FormClosing( object sender, FormClosingEventArgs e ) {
			String curMethodName = "Controller: Controller_FormClosing: ";
			if ( Listener.isWscConnected ) {
				Log.WriteFile( curMethodName + "Exiting WaterSkiConnect handler" );
				try {
					terminateMonitors( "" );

				} catch ( Exception ex ) {
					MessageBox.Show( String.Format( "{0} Exception encountered: {1}", curMethodName, ex.Message ) );
				}

			} else {
				Log.WriteFile( curMethodName + "Exit byassed WaterSkiConnect handler not connected" );
			}
		}

		private void Controller_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( Properties.Settings.Default.AppTitle.Length > 0 ) Properties.Settings.Default.AppTitle = this.Text;
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.MessageController_Width = this.Size.Width;
				Properties.Settings.Default.MessageController_Height = this.Size.Height;
				Properties.Settings.Default.MessageController_Location = this.Location;
			}
			Log.WriteFile( "Controller_FormClosed: WaterSkiConnect handler closed" );
			MessageBox.Show( String.Format( "MessageController_Width={0} this.Size={1}"
				+ "\nMessageController_Height={2} this.SizeWidth={3}"
				+ "\nMessageController_Location={4} this.Location={5}"
				, Properties.Settings.Default.MessageController_Width, this.Size.Width
				, Properties.Settings.Default.MessageController_Height, this.Size.Height
				, Properties.Settings.Default.MessageController_Location.ToString(), this.Location.ToString() ) );
		}

		private void execConnectDialog( object sender, EventArgs e ) {
			String curMethodName = "WscMessageHandler: Controller: execConnectDialog: ";
			if ( sender  != null ) {
				Timer curTimerObj = (Timer)sender;
				curTimerObj.Stop();
				curTimerObj.Tick -= new EventHandler( execConnectDialog );
			}

			// Display the form as a modal dialog box.
			ConnectDialog connectDialog = new ConnectDialog();
			connectDialog.windowLocation = new System.Drawing.Point( this.Location.X + 100, this.Location.Y + 100 );
			DialogResult connectDialogResult = connectDialog.ShowDialog();
			if ( connectDialogResult == DialogResult.OK ) {
				if ( connectDialog.dialogCommand.Equals( "Connected" ) ) {
					execHandlerConnectTasks( connectDialog.eventSubId, connectDialog.useJumpTimes );

				} else if ( connectDialog.dialogCommand.Equals( "Disconnected" ) ) {
					try {
						terminateMonitors( "" );

					} catch ( Exception ex ) {
						MessageBox.Show( String.Format( "{0} Exception encountered: {1}", curMethodName, ex.Message ) );
					}

				} else if ( connectDialog.dialogCommand.Equals( "ShowPin" ) ) {
					Listener.showPin();

				} else if ( connectDialog.dialogCommand.Equals( "ShowAppsConnected" ) ) {
				}

			}
		}

		private void execHandlerConnectTasks( String eventSubId, bool useJumpTimes ) {
			String curMethodName = "WscMessageHandler: Controller: execHandlerConnectTasks: ";
			bool handlersConnected = false, boldMsg = false;

			String curMsg = String.Format( "{0} attempting connection to WaterSkiConnect for tournament {1} / {2}", curMethodName, Properties.Settings.Default.SanctionNum, eventSubId );
			addViewMessage( curMsg, boldMsg );

			HelperFunctions.cleanMsgQueues();
			curMsg = String.Format( "{0} message queues cleaned for {1} / {2}", curMethodName, Properties.Settings.Default.SanctionNum, eventSubId );
			addViewMessage( curMsg, boldMsg );

			Task.Factory.StartNew( () => ListenHandler.startWscMessageHandler( Properties.Settings.Default.SanctionNum, eventSubId, useJumpTimes ) );
			curMsg = String.Format( "{0} activating ListenHandler for {1} / {2}", curMethodName, Properties.Settings.Default.SanctionNum, eventSubId );
			addViewMessage( curMsg, boldMsg );

			Task.Factory.StartNew( () => Listener.startWscListener( Properties.Settings.Default.SanctionNum, eventSubId ) );
			curMsg = String.Format( "{0} activating Listener for {1} / {2}", curMethodName, Properties.Settings.Default.SanctionNum, eventSubId );
			addViewMessage( curMsg, boldMsg );

			handlersConnected = waitForMonitorConnection( "Listener" );
			String connectStatus = "failed";
			if ( handlersConnected ) connectStatus = "completed";
			curMsg = String.Format( "{0} Listener connection {1} for {2} / {3}", curMethodName, connectStatus, Properties.Settings.Default.SanctionNum, eventSubId );
			if ( !handlersConnected ) boldMsg = true;
			addViewMessage( curMsg, boldMsg );

			Task.Factory.StartNew( () => Transmitter.startWscTransmitter( Properties.Settings.Default.SanctionNum, eventSubId ) );
			curMsg = String.Format( "{0} activating Transmitter for {1} / {2}", curMethodName, Properties.Settings.Default.SanctionNum, eventSubId );
			boldMsg = false;
			addViewMessage( curMsg, boldMsg );

			handlersConnected = waitForMonitorConnection( "Transmitter" );
			connectStatus = "failed";
			if ( handlersConnected ) connectStatus = "completed";

			curMsg = String.Format( "{0} Transmitter connection {1} for {2} / {3}", curMethodName, connectStatus, Properties.Settings.Default.SanctionNum, eventSubId );
			if ( !handlersConnected ) boldMsg = true;
			addViewMessage( curMsg, boldMsg );

			myCountHeartBeatFailed = 0;
			myMonitorDataTable = HelperFunctions.getMonitorHeartBeatAll();
			myHeartBeatViewRowIdx = MessageView.Rows.Count;
			showHeartBeatMsg();

			WaterSkiConnectButton.Visible = false;
			DisconnectButton.Visible = true;
			ShowPinButton.Visible = true;

			// Create a timer with 2.5 minute interval.
			myHeartBeatTimer = new Timer();
			myHeartBeatTimer.Interval = 150000;
			myHeartBeatTimer.Tick += new EventHandler( checkMonitorHeartBeat );
			myHeartBeatTimer.Start();
		}

		private void showHeartBeatMsg() {
			String curMethodName = "WscMessageHandler: Controller: showHeartBeatMsg: ";
			bool addViewRows = false;
			int curViewIdx = myHeartBeatViewRowIdx - 1;
			if ( myHeartBeatViewRowIdx >= MessageView.Rows.Count ) addViewRows = true;

			foreach ( DataRow curMonitorHeartBeat in myMonitorDataTable.Rows ) {
				String curMonitorName = (String)curMonitorHeartBeat["MonitorName"];
				DateTime curHeartBeat = (DateTime)curMonitorHeartBeat["HeartBeat"];
				String curMsg = String.Format( "{0} {1} heart beat {2} ", curMethodName, curMonitorName, curHeartBeat.ToString( myDisplayDateFormat ) );
				if ( addViewRows ) {
					addViewMessage( curMsg, false );
				
				} else {
					DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
					curViewRow.Cells["Message"].Value = curMsg;
					curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
					curViewIdx++;
				}
			}
		}

		private void addViewMessage(String msg, bool boldMessage ) {
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			if ( boldMessage ) {
				curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
				curViewRow.Cells["Message"].Style.ForeColor = Color.Red;
				return;
			}
		}

		private bool waitForMonitorConnection( String monitorName ) {
			String curMethodName = "WscMessageHandler: Controller: waitForMonitorConnection: ";

			int count = 1;
			while ( count < 60 ) {
				try {
					DataRow curHeatBeatRow = HelperFunctions.getMonitorHeartBeat( monitorName );
					if ( curHeatBeatRow != null ) return true;
					
					System.Threading.Thread.Sleep( 2000 );
					count++;

				} catch ( Exception ex ) {
					String curMsg = curMethodName + "Exception encountered " + ex.Message;
					Log.WriteFile( curMsg );
					MessageBox.Show( curMsg );
					return false;
				}
			}
			return false;
		}

		private void checkMonitorHeartBeat( object sender, EventArgs e ) {
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
						if ( myCountHeartBeatFailed > myCountHeartBeatFailedMax ) terminateMonitors( curMonitorName );
						return;
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
			Log.WriteFile( curMethodName + "No heartbeat for monitor " + curMonitorName );
			
			if ( myHeartBeatTimer != null ) {
				myHeartBeatTimer.Stop();
				myHeartBeatTimer.Tick -= new EventHandler( checkMonitorHeartBeat );
			}
			String msg = "";
			if ( curMonitorName.Equals( "All" ) ) {
				msg = "HeartBeat for monitors unavailable.  Assuming connection has failed.  Terminating monitoring.";
			
			} else if ( curMonitorName.Length > 0 ) {
				msg = String.Format( "HeartBeat for monitor {0} not available.  Assuming connection has failed.  Terminating monitoring.", curMonitorName );
			}
			int curViewIdx = MessageView.Rows.Add();
			DataGridViewRow curViewRow = MessageView.Rows[curViewIdx];
			curViewRow.Cells["Message"].Value = curMethodName + msg;
			curViewRow.Cells["CreationDatetime"].Value = DateTime.Now.ToString( myDisplayDateFormat );
			curViewRow.Cells["Message"].Style.Font = myMsgFontBold;
			curViewRow.Cells["Message"].Style.ForeColor = Color.Red;

			ListenHandler.disconnect();
			Listener.disconnect();
			Transmitter.disconnect(0);

			myMonitorDataTable = null;
			WaterSkiConnectButton.Visible = true;
			DisconnectButton.Visible = false;
			ShowPinButton.Visible = false;
			myCountHeartBeatFailed = 0;

			if ( msg.Length > 0 ) MessageBox.Show( msg );
		}

		private void WaterSkiConnectButton_Click( object sender, EventArgs e ) {
			execConnectDialog( null, null);
		}

		private void ShowPinButton_Click( object sender, EventArgs e ) {
			String curMethodName = "Controller: DisconnectButton_Click: ";
			try {
				Listener.showPin();
			} catch ( Exception ex ) {
				MessageBox.Show( String.Format( "{0} Exception encountered: {1}", curMethodName, ex.Message ) );
			}
		}

		private void DisconnectButton_Click( object sender, EventArgs e ) {
			terminateMonitors( "" );
		}

	}
}

