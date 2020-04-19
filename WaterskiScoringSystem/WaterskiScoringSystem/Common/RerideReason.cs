using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class RerideReason : Form {
        private String myCommandValue = "";
        private String myRerideReason = "";

        public RerideReason() {
            InitializeComponent();
        }

        public String Command {
            get {
                return myCommandValue;
            }
        }

        public String RerideReasonText {
            get {
                return myRerideReason;
            }
            set {
                myRerideReason = value;
            }
        }

        private void RerideReason_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.ImportMatch_Location.X > 0
                && Properties.Settings.Default.ImportMatch_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.RerideReason_Location;
            }
            if ( myCommandValue.ToLower().Equals( "update" ) ) {
                UpdateButton.Focus();
            } else if ( myCommandValue.ToLower().Equals( "updatewithprotect" ) ) {
                UpdateWithProtButton.Focus();
            } else if ( myCommandValue.ToLower().Equals( "cancel" ) ) {
                CancelButton.Focus();
            } else {
                CancelButton.Focus();
            }
            RerideReasonTextbox.Text = myRerideReason;
        }

        private void RerideReason_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.RerideReason_Location = this.Location;
            }
        }

        private void UpdateButton_Click( object sender, EventArgs e ) {
            myCommandValue = "update";
        }

        private void CancelButton_Click( object sender, EventArgs e ) {
            myCommandValue = "cancel";
        }

        private void UpdateWithProtButton_Click( object sender, EventArgs e ) {
            myCommandValue = "updatewithprotect";
            
        }

        private void RerideReasonTextbox_Validated( object sender, EventArgs e ) {
            myRerideReason = RerideReasonTextbox.Text;
        }
    
    }
}
