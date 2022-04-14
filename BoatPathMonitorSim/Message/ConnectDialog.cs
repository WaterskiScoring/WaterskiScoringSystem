using System;
using System.Drawing;
using System.Windows.Forms;

using BoatPathMonitorSim.Common;

namespace BoatPathMonitorSim.Message {
	public partial class ConnectDialog : Form {
		private static String myWscWebLocation = "";

		private String myDialogCommand = "";
		private bool myWscConnected = false;
		private Point myWindowLocation = new Point( 0, 0 );

		public ConnectDialog() {
			InitializeComponent();
		}

		public static String WscWebLocation {
			get { return myWscWebLocation; }
			set { myWscWebLocation = value; }
		}

		public String dialogCommand {
			get { return myDialogCommand; }
			set { myDialogCommand = value; }
		}

		public System.Drawing.Point windowLocation {
			get { return myWindowLocation; }
			set { myWindowLocation = value; }
		}

		public Boolean isWscConnected {
			get { return myWscConnected; }
			set { myWscConnected = value; }
		}

		private void ConnectDialog_Load( object sender, EventArgs e ) {
			this.Location = myWindowLocation;

			myWscWebLocation = ConnectMgmtData.wscWebLocationDefault;
			sanctionNumTextbox.Text = ConnectMgmtData.sanctionNum;
			serverUriTextBox.Text = myWscWebLocation;
			eventSubIdTextBox.Text = ConnectMgmtData.eventSubId;
			UseJumpTimesCheckBox.Checked = ConnectMgmtData.useJumpTimes;

			if ( isWscConnected ) {
				MessageLabel.Text = "Listener is connected and active";

			} else {
				MessageLabel.Text = "Listener is not connected";
			}
		}

		private void execWscConnect_Click( object sender, EventArgs e ) {
			ConnectMgmtData.sanctionNum = sanctionNumTextbox.Text;
			ConnectMgmtData.eventSubId = eventSubIdTextBox.Text;
			ConnectMgmtData.useJumpTimes = false;
			if ( UseJumpTimesCheckBox.Checked ) ConnectMgmtData.useJumpTimes = true;
			myDialogCommand = "Connected";
		}

		private void execWscClose_Click( object sender, EventArgs e ) {
			myDialogCommand = "Disconnected";
		}

		private void showAppsConnected_Click( object sender, EventArgs e ) {
			myDialogCommand = "ShowAppsConnected";
		}


		private void showWscPin_Click( object sender, EventArgs e ) {
			myDialogCommand = "ShowPin";
		}

		private void UseJumpTimesCheckBox_CheckedChanged( object sender, EventArgs e ) {
			this.Location = myWindowLocation;
			ConnectMgmtData.useJumpTimes = UseJumpTimesCheckBox.Checked;
		}
	}
}
