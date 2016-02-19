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
            TwitterAccessTextBox.Text = ExportLiveTwitter.TwitterRequestTokenURL;
            if ( ExportLiveTwitter.TwitterRequestTokenURL.Length > 0 ) {
                ExportLiveTwitter.TwitterRequestTokenSaveURL = ExportLiveTwitter.TwitterRequestTokenURL;
            }
            TwitterPinTextBox.Text = ExportLiveTwitter.TwitterAccessPin;
            if (ExportLiveTwitter.TwitterReportByValue.Equals( "Pass" )) {
                twitterByPassRB.Checked = true;
            } else {
                twitterBySkierRB.Checked = true;
            }
        }

        private void ExportLiveWebDialog_FormClosing(object sender, FormClosingEventArgs e) {
            if (myActionCmd.Equals( "TwitterAuth" )) {
                if (ExportLiveTwitter.TwitterAccessPin.Length > 0) {
                } else {
                    e.Cancel = true;
                }
            }
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

        private void TwitterActivateButton_Click(object sender, EventArgs e) {
            if (TwitterAccessTextBox.Text.Equals( ExportLiveTwitter.TwitterDefaultAccount )) {
                myActionCmd = "TwitterActive";
            } else {
                if (ExportLiveTwitter.TwitterAccessPin.Length > 0 && ExportLiveTwitter.TwitterRequestTokenURL.Length > 0) {
                    myActionCmd = "TwitterAuth";
                    ExportLiveTwitter.getAppAccessAuthorize();
                } else {
                    if (ExportLiveTwitter.TwitterAccessPin.Length > 0 && ExportLiveTwitter.TwitterRequestTokenURL.Length == 0) {
                        ExportLiveTwitter.TwitterRequestTokenURL = ExportLiveTwitter.TwitterRequestTokenSaveURL;
                    }
                    ExportLiveTwitter.getAppAccessAuthorize();
                    TwitterAccessTextBox.Text = ExportLiveTwitter.TwitterRequestTokenURL;
                    TwitterPinTextBox.Text = ExportLiveTwitter.TwitterAccessPin;
                    myActionCmd = "TwitterAuth";
                }
            }
        }

        private void SetDefaultButton_Click(object sender, EventArgs e) {
            WebLocationTextBox.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
        }

        private void TwitterPinTextBox_Validated(object sender, EventArgs e) {
            ExportLiveTwitter.TwitterAccessPin = TwitterPinTextBox.Text;
        }

        private void twitterByPassRB_CheckedChanged(object sender, EventArgs e) {
            ExportLiveTwitter.TwitterReportByValue = "Pass";
        }

        private void twitterBySkierRB_CheckedChanged(object sender, EventArgs e) {
            ExportLiveTwitter.TwitterReportByValue = "Skier";
        }

        private void SetDefaultAccountButton_Click(object sender, EventArgs e) {
            ExportLiveTwitter.TwitterRequestTokenURL = ExportLiveTwitter.TwitterDefaultAccount;
            TwitterAccessTextBox.Text = ExportLiveTwitter.TwitterRequestTokenURL;
        }

        private void TwitterHelpButton_Click(object sender, EventArgs e) {
            MessageBox.Show("Use the 'Activate Twitter' button to post scores to a Twitter account as scores are updated.  "
                + "You may use the default account 'WaterSkiScoring' or you may use an account of your own."
                + "\n\nSimply click on the 'Set Default Account' and then the 'Activate Twitter' to use 'WaterSkiScoring' account"
                + "\n\nTo use your own account you must perform the following steps:"
                + "\n1. Click on the 'Activate Twitter' button to get an authorization URL"
                + "\n2. Copy and paste the URL into your browser"
                + "\n3. Login to your Twitter account if you are not automatically logged in"
                + "\n4. Then click on the 'Authorize App' button that is provided"
                + "\n5. You will then be provided a 7 digit PIN number that should be entered in the 'Twitter Pin' textbox on this dialog"
                + "\n6. Click on the 'Activate Twitter' button to complete the authorization process "
                + "   This authorizes the program to post messages to your account"
                + "\n\nAnyone wishing to view the messages can simply follow the twitter account in use"
                + "\n\nUse the 'Disable' button at the top of the dialog to turn off the Twitter activity"
                );
        }

    }
}
