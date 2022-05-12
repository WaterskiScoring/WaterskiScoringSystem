using System;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Jump {
	public partial class PrintJumpFormDialog : Form {
		public PrintJumpFormDialog() {
			InitializeComponent();
		}

		private void PrintJumpFormDialog_Load( object sender, EventArgs e ) {
		}
		
		private String outReportName = "";
		public String ReportName {
			get { return outReportName; }
			set { outReportName = value; }
		}

		private void JumpOfficialFormButton_Click( object sender, EventArgs e ) {
			ReportName = "JumpOfficialForm";
		}

		private void JumpSkierSpecFormButton_Click( object sender, EventArgs e ) {
			ReportName = "JumpSkierSpecForm";
		}
	}
}
