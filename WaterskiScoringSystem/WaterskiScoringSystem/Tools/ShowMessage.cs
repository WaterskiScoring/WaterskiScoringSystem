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
	public partial class ShowMessage : Form {
		public ShowMessage() {
			InitializeComponent();
		}

		private void ShowMessage_Load( object sender, EventArgs e ) {
			this.OKButton.Focus();

		}
		public String Message {
			set {
				this.MessageTextBox.Text = value;
			}
		}

		private void ExportButton_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			myExportData.exportString( this.MessageTextBox.Text, "IwwfValidateMsgs.txt" );
		}
	}
}
