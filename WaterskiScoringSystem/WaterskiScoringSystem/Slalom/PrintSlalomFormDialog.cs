using System;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Slalom {
	public partial class PrintSlalomFormDialog : Form {
		public PrintSlalomFormDialog() {
			InitializeComponent();
		}

		private void PrintSlalomFormDialog_Load( object sender, EventArgs e ) {
		}
		
		private String outReportName = "";
		public String ReportName {
			get { return outReportName; }
			set { outReportName = value; }
		}

		private void SlalomJudgeFormButton_Click( object sender, EventArgs e ) {
			ReportName = "SlalomJudgeForm";
		}

		private void SlalomRecapFormButton_Click( object sender, EventArgs e ) {
			ReportName = "SlalomRecapForm";
		}

	}
}
