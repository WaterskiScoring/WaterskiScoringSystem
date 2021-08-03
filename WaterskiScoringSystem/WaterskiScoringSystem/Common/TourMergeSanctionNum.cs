using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class TourMergeSanctionNum : Form {
        private String myCommandValue = "";
        private String myReason = "";

        public TourMergeSanctionNum() {
            InitializeComponent();
        }

        public String Command {
            get {
                return myCommandValue;
            }
        }

        public String ReasonText {
            get {
                return myReason;
            }
            set {
                myReason = value;
            }
        }

        private void Reason_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.ImportMatch_Location.X > 0
                && Properties.Settings.Default.ImportMatch_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourMergeSanctionNum_Location;
            }
            if ( myCommandValue.ToLower().Equals( "update" ) ) {
                OkButton.Focus();
            } else if ( myCommandValue.ToLower().Equals( "cancel" ) ) {
                CancelButton.Focus();
            } else {
                CancelButton.Focus();
            }
            ReasonTextbox.Text = myReason;
        }

        private void Reason_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TourMergeSanctionNum_Location = this.Location;
            }
        }

        private void UpdateButton_Click( object sender, EventArgs e ) {
            myCommandValue = "update";
        }

        private void CancelButton_Click( object sender, EventArgs e ) {
            myCommandValue = "cancel";
        }

        private void ReasonTextbox_Validated( object sender, EventArgs e ) {
            myReason = ReasonTextbox.Text;
        }

        private void ReasonLabel_Click( object sender, EventArgs e ) {

        }
    
    }
}
