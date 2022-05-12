using System;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Trick {
	public partial class PrintTrickFormDialog : Form {
		public PrintTrickFormDialog() {
			InitializeComponent();
		}

		private void PrintTrickFormDialog_Load( object sender, EventArgs e ) {
		}
		
		private String outReportName = "";
		public String ReportName {
			get { return outReportName; }
			set { outReportName = value; }
		}

		private void TrickOfficialFormButton_Click( object sender, EventArgs e ) {
			ReportName = "TrickOfficialForm";
		}

		private void TrickTimingFormButton_Click( object sender, EventArgs e ) {
			ReportName = "TrickTimingForm";
		}

		private void TrickSkierSpecFormButton_Click( object sender, EventArgs e ) {
			ReportName = "TrickSkierSpecForm";
		}
	}
}
