using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class PrintOfficialFormDialog : Form {
        public PrintOfficialFormDialog() {
            InitializeComponent();
        }

        private void PrintOfficialFormDialog_Load(object sender, EventArgs e) {

        }

        private String outReportName = "";
        public String ReportName {
            get { return outReportName; }
            set { outReportName = value; }
        }

        private void SlalomBoatButton_Click(object sender, EventArgs e) {
            ReportName = "SlalomBoatJudgeForm";
        }

        private void SlalomRecapButton_Click(object sender, EventArgs e) {
            ReportName = "SlalomRecapForm";
        }
    }
}
