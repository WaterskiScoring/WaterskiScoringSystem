using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class TeamPrintDialogForm : Form {
        public TeamPrintDialogForm() {
            InitializeComponent();
        }

        private void TeamPrintDialogForm_Load(object sender, EventArgs e) {

        }

        private String outReportName = "";
        public String ReportName {
            get { return outReportName; }
            set { outReportName = value; }
        }

        private void TeamResultsButton_Click(object sender, EventArgs e) {
            ReportName = "TeamResults";
        }

        private void SkierListButton_Click(object sender, EventArgs e) {
            ReportName = "SkierList";
        }

    }
}
