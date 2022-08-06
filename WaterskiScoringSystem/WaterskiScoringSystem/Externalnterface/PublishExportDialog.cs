using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Externalnterface {
	public partial class PublishExportDialog : Form {
		private String myReportType;
		private String myEvent;
		private String myReportFileName;
		private String myReportTitle;

		public PublishExportDialog() {
			InitializeComponent();
		}

		public string ReportType { get => myReportType; set => myReportType = value; }
		public string Event { get => myEvent; set => myEvent = value; }
		public string ReportFileName { get => myReportFileName; set => myReportFileName = value; }
		public string ReportTitle { get => myReportTitle; set => myReportTitle = value; }

		private void PublishExportDialog_Load( object sender, EventArgs e ) {
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
