using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Slalom {
	public partial class NextPassDialog : Form {
		private String myButtonSelect;

		public NextPassDialog() {
			InitializeComponent();
		}

		private void NextPassDialog_Load( object sender, EventArgs e ) {
       }

		public void setWindowLocation(int xPos, int yPost) {
			Point windowLocation = new Point( xPos, yPost );
			this.Location = windowLocation;
		}

		public void setNextLineButtonDefault() {
			myButtonSelect = "Line";
			this.AcceptButton = NextLineButton;
			this.NextLineButton.ForeColor = Color.Yellow;
			this.NextSpeedButton.ForeColor = Color.White;
		}

		public void setNextSpeedButtonDefault() {
			myButtonSelect = "Speed";
			this.AcceptButton = NextSpeedButton;
			this.NextLineButton.ForeColor = Color.White;
			this.NextSpeedButton.ForeColor = Color.Yellow;
		}

		public String getDialogResult() {
			return myButtonSelect;
		}

		private void NextSpeedButton_Click( object sender, EventArgs e ) {
			myButtonSelect = "Speed";
		}

		private void NextSpeedButton_Enter( object sender, EventArgs e ) {
			myButtonSelect = "Speed";
		}

		private void NextLineButton_Click( object sender, EventArgs e ) {
			myButtonSelect = "Line";
		}

		private void NextLineButton_Enter( object sender, EventArgs e ) {
			myButtonSelect = "Line";
		}

		private void NextCancelButton_Enter( object sender, EventArgs e ) {
			myButtonSelect = "";
		}
	}
}
