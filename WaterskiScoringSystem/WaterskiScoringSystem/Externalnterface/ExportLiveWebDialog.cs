using System;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Externalnterface {
    public partial class ExportLiveWebDialog : Form {
        private String myWebLocation = "";
        private String myActionCmd = "";

        public ExportLiveWebDialog() {
            InitializeComponent();
        }

        private void ExportLiveWebDialog_Load(object sender, EventArgs e) {
            if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                ConnectButton.Enabled = false;
                ConnectButton.Visible = false;
            } else {
                ConnectButton.Enabled = true;
                ConnectButton.Visible = true;
            }

            ResendButton.Enabled = LiveWebHandler.LiveWebMessageHandlerActive;
            ResendButton.Visible = LiveWebHandler.LiveWebMessageHandlerActive;

            ResendAllButton.Enabled = LiveWebHandler.LiveWebMessageHandlerActive;
            ResendAllButton.Visible = LiveWebHandler.LiveWebMessageHandlerActive;

            DisableSendButton.Enabled = LiveWebHandler.LiveWebMessageHandlerActive;
            DisableSendButton.Visible = LiveWebHandler.LiveWebMessageHandlerActive;

            DisableAllSendButton.Enabled = LiveWebHandler.LiveWebMessageHandlerActive;
            DisableAllSendButton.Visible = LiveWebHandler.LiveWebMessageHandlerActive;
        }

        public String ActionCmd {
            get {
                return myActionCmd;
            }
            set {
                myActionCmd = value;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            myActionCmd = "Cancel";
        }

        private void ConnectButton_Click( object sender, EventArgs e ) {
            myActionCmd = "Connect";
        }

        private void DisableButton_Click( object sender, EventArgs e ) {
            myActionCmd = "Disable";
        }

        private void ResendButton_Click(object sender, EventArgs e) {
            myActionCmd = "Resend";
        }

        private void ResendAllButton_Click(object sender, EventArgs e) {
            myActionCmd = "ResendAll";
        }

		private void DisableSendButton_Click(object sender, EventArgs e) {
			myActionCmd = "DiableSkier";

		}

		private void DisableAllSendButton_Click(object sender, EventArgs e) {
			myActionCmd = "DiableAllSkier";
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start(linkLabel1.Text);
			linkLabel1.LinkVisited = true;
		}

	}
}
