using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class ExportLiveWebDialog : Form {
        private String myWebLocation = "";
        private String myActionCmd = "";

        public ExportLiveWebDialog() {
            InitializeComponent();
        }

        private void ExportLiveWebDialog_Load(object sender, EventArgs e) {
            WebLocationTextBox.Text = myWebLocation;
        }

        public String WebLocation {
            get {
                return myWebLocation;
            }
            set {
                myWebLocation = value;
            }
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

        private void SetLocationButton_Click(object sender, EventArgs e) {
            myWebLocation = WebLocationTextBox.Text;

            if (myWebLocation.Length > 5) {
                if (myWebLocation.IndexOf( "www.waterskiresults.com" ) < 0) {
                    myWebLocation += "|" + "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
                }
            } else {
                myWebLocation = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
            }
            myActionCmd = "Set";
        }

        private void DisableButton_Click(object sender, EventArgs e) {
            myWebLocation = "";
            WebLocationTextBox.Text = "";
            ExportLiveTwitter.TwitterRequestTokenURL = "";
            myActionCmd = "Disable";
        }

        private void ResendButton_Click(object sender, EventArgs e) {
            myActionCmd = "Resend";
        }

        private void ResendAllButton_Click(object sender, EventArgs e) {
            myActionCmd = "ResendAll";
        }

        private void SetDefaultButton_Click(object sender, EventArgs e) {
            WebLocationTextBox.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
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
