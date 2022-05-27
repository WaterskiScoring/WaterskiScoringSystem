using System;
using System.Windows.Forms;
using System.Drawing;

namespace HttpMessageHandler.Externalnterface {
	public partial class LiveWebConnectDialog : Form {
		private String myWebLocation = "";
		private String myDialogCommand = "";
		private Point myWindowLocation = new Point( 0, 0 );

		public LiveWebConnectDialog() {
			InitializeComponent();
		}

		private void LiveWebConnectDialog_Load( object sender, EventArgs e ) {

		}
		public System.Drawing.Point windowLocation {
			get { return myWindowLocation; }
			set { myWindowLocation = value; }
		}

		public String WebLocation {
			get {
				return myWebLocation;
			}
			set {
				myWebLocation = value;
			}
		}

		public String dialogCommand {
			get { return myDialogCommand; }
			set { myDialogCommand = value; }
		}

		private void CancelButton_Click( object sender, EventArgs e ) {
			myDialogCommand = "Cancel";
		}

		private void SetLocationButton_Click( object sender, EventArgs e ) {
			myWebLocation = WebLocationTextBox.Text;

			if ( myWebLocation.Length > 5 ) {
				if ( myWebLocation.IndexOf( "www.waterskiresults.com" ) < 0 ) {
					myWebLocation += "|" + "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
				}
			} else {
				myWebLocation = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
			}
			myDialogCommand = "Connected";
		}

		private void DisableButton_Click( object sender, EventArgs e ) {
			myWebLocation = "";
			WebLocationTextBox.Text = "";
			myDialogCommand = "Disconnected";
		}

		private void SetDefaultButton_Click( object sender, EventArgs e ) {
			WebLocationTextBox.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
		}
	}
}
