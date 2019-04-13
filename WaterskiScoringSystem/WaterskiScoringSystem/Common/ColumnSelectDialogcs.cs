using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
	public partial class ColumnSelectDialogcs : Form {
		private DataTable myColumnList;

		/* 
         * Class initialization
         */
		public ColumnSelectDialogcs() {
			InitializeComponent();
		}

		private void ColumnSelectDialogcs_Load( object sender, EventArgs e ) {

		}

		public DataTable ColumnList {
			// Input used to build dropdown list of column names 
			get {
				return myColumnList;
			}
			set {
				myColumnList = value;
				this.dataGridView.DataSource = myColumnList;
			}
		}

		private void okButton_Click( object sender, EventArgs e ) {
		}

	}
}
