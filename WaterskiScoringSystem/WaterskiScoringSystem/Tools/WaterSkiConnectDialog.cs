using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
	public partial class WaterSkiConnectDialog : Form {

		public WaterSkiConnectDialog() {
			InitializeComponent();
			eventSubIdTextBox.Text = EwscMonitor.eventSubId;
		}

		public String eventSubId {
			set {
				eventSubIdTextBox.Text = value;
			}
		}

		public void setEvent( String inEvent ) {
			if ( inEvent.Equals("Jump")) {
				UseJumpTimesCheckBox.Checked = EwscMonitor.useJumpTimes;
				UseJumpTimesCheckBox.Visible = true;
				UseJumpTimesCheckBox.Enabled = true;
				return;
			}

			UseJumpTimesCheckBox.Checked = false;
			UseJumpTimesCheckBox.Visible = false;
			UseJumpTimesCheckBox.Enabled = false;
			EwscMonitor.useJumpTimes = UseJumpTimesCheckBox.Checked;
		}

		private void WaterSkiConnectDialog_Load( object sender, EventArgs e ) {
			if ( EwscMonitor.EwcsWebLocation.Length > 1 ) {
				MessageLabel.Text = "WaterSkiConnect is connected and active";

			} else {
				MessageLabel.Text = "WaterSkiConnect is not connected";
			}
		}

		private void execEwcsConnect_Click( object sender, EventArgs e ) {
			EwscMonitor.eventSubId = eventSubIdTextBox.Text;
			Task.Factory.StartNew( () => EwscMonitor.execEwscMonitoring() );
			int count = 0;
			while ( count >= 0 ) {
				System.Threading.Thread.Sleep( 500 );
				if ( EwscMonitor.ConnectActive() ) break;
				if ( count > 50 ) break;
				count++;
			}
		}

		private void execEwcsClose_Click( object sender, EventArgs e ) {
			DialogResult msgResp =
				MessageBox.Show( "You have asked to close the connection to WaterSkiConnect", "Close Confirm",
					MessageBoxButtons.OKCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1 );
			if ( msgResp == DialogResult.OK ) {
				EwscMonitor.sendExit();
				
				int count = 0;
				while ( EwscMonitor.ConnectActive() ) {
					System.Threading.Thread.Sleep( 1000 );
					if ( count > 200 ) break;
					count++;
				}
			}
		}

		private void showEwcsPin_Click( object sender, EventArgs e ) {
			EwscMonitor.showPin();
		}

		private void UseJumpTimesCheckBox_CheckedChanged( object sender, EventArgs e ) {
			EwscMonitor.useJumpTimes = UseJumpTimesCheckBox.Checked;
		}
	}
}
