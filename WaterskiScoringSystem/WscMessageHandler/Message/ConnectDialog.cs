using System;
using System.Drawing;
using System.Windows.Forms;

namespace WscMessageHandler.Message {
	public partial class ConnectDialog : Form {
		private static String myWscWebLocation = "";
		private static readonly String myWscWebLocationDefault = "http://ewscdata.com:40000/";
		
		private String myDialogCommand = "";
		private String mySanctionNum = "";
		private String myEventSubId = "";
		private bool myUseJumpTimes = false;
		private bool myWscConnected = false;
		private Point myWindowLocation = new Point( 0,0);

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

		public String sanctionNum {
			get { return mySanctionNum; }
			set { mySanctionNum = value; }
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
			myWscWebLocation = myWscWebLocationDefault;
			sanctionNumTextbox.Text = mySanctionNum;
			serverUriTextBox.Text = myWscWebLocation;
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
