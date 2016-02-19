using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Slalom {
    public partial class SlalomOptUp : Form {
        private String myResponseValue = "";
        private Boolean ruleMsgVisible = false;

        public SlalomOptUp() {
            InitializeComponent();
        }

        private void SlalomOptUp_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.SlalomOptup_Location.X > 0
                && Properties.Settings.Default.SlalomOptup_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.SlalomOptup_Location;
            }
            awsaRule10_06Msg.Visible = ruleMsgVisible;
            SpeedButton.Focus();
        }

        public String Response {
            get {
                return myResponseValue;
            }
        }

        public void showRuleMsg() {
            ruleMsgVisible = true;
            awsaRule10_06Msg.Visible = ruleMsgVisible;
        }

        public void hideRuleMsg() {
            ruleMsgVisible = false;
            awsaRule10_06Msg.Visible = ruleMsgVisible;
        }

        private void SlalomOptUp_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.SlalomOptup_Location = this.Location;
            }
        }

        private void SpeedButton_Click( object sender, EventArgs e ) {
            myResponseValue = "speed";
        }

        private void LineLengthButton_Click( object sender, EventArgs e ) {
            myResponseValue = "line";
        }

        private void CancelButton_Click( object sender, EventArgs e ) {
            myResponseValue = "cancel";
        }

    }
}
