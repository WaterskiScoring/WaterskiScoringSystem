﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Admin {
	public partial class SanctionSetupDialog : Form {
		private String mySanctionId = "";
		private String myEditCode = "";

		public SanctionSetupDialog() {
			InitializeComponent();
		}

		public String SanctionId {
			get {
				return mySanctionId;
			}
			set {
				mySanctionId = value;
			}
		}

		public String EditCode {
			get {
				return myEditCode;
			}
			set {
				myEditCode = value;
			}
		}

		private void OKButton_Click( object sender, EventArgs e ) {
			mySanctionId = SanctionIDTextBox.Text;
			myEditCode = EditCodeTextBox.Text;
		}
	}
}
