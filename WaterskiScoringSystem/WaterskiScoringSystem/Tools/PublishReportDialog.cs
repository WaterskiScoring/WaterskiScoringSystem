using System;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
	public partial class PublishReportDialog : Form {
		private String myReportType;
		private String myEvent;
		private String myReportFileName;
		private String myReportTitle;

		public PublishReportDialog() {
			InitializeComponent();
		}

		public string ReportType { get => myReportType; set => myReportType = value; }
		public string Event { get => myEvent; set => myEvent = value; }
		public string ReportFileName { get => myReportFileName; set => myReportFileName = value; }
		public string ReportTitle { get => myReportTitle; set => myReportTitle =  value ; }

		private void PublishReportDialog_Load( object sender, EventArgs e ) {
			ReportFilenameTextBox.Text = myReportFileName;
			ReportTypeTextBox.Text = myReportType;
			EventTextBox.Text = myEvent;
			ReportTitleTextBox.Text = myReportTitle;
		}

		private void OkButton_Click( object sender, EventArgs e ) {
			myReportTitle = ReportTitleTextBox.Text;
		}
	}
}
