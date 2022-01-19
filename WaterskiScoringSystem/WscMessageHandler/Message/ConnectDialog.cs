using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WscMessageHandler.Common;

namespace WscMessageHandler.Message {
	public partial class ConnectDialog : Form {
		private String myDialogCommand = "";
		private String myEventSubId = "";
		private bool myUseJumpTimes = false;
		private bool myWscConnected = false;
		private System.Drawing.Point myWindowLocation = new System.Drawing.Point( 0,0);

		public ConnectDialog() {
			InitializeComponent();
		}
		public String dialogCommand {
			get { return myDialogCommand; }
			set { myDialogCommand = value; }
		}

		public System.Drawing.Point windowLocation {
			get { return myWindowLocation; }
			set { myWindowLocation = value; }
		}

		public String eventSubId {
			get { return myEventSubId; }
			set { myEventSubId = value; }
		}

		public Boolean useJumpTimes {
			get { return myUseJumpTimes; }
			set { myUseJumpTimes = value; }
		}
		public Boolean isWscConnected {
			get { return myWscConnected; }
			set { myWscConnected = value; }
		}

		private void ConnectDialog_Load( object sender, EventArgs e ) {
			eventSubIdTextBox.Text = myEventSubId;
			UseJumpTimesCheckBox.Checked = myUseJumpTimes;
			this.Location = myWindowLocation;

			if ( isWscConnected ) {
				MessageLabel.Text = "Listener is connected and active";

			} else {
				MessageLabel.Text = "Listener is not connected";
			}
		}

		private void execWscConnect_Click( object sender, EventArgs e ) {
			myEventSubId = eventSubIdTextBox.Text;
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
			myUseJumpTimes = UseJumpTimesCheckBox.Checked;
		}

	}
}
