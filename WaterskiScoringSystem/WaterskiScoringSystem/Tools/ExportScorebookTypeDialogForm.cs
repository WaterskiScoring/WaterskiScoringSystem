using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class ExportScorebookTypeDialogForm : Form {
        public ExportScorebookTypeDialogForm() {
            InitializeComponent();
        }

        private String outReportFormat;

        public String ReportFormat {
            get {
                return outReportFormat;
            }
            set {
                outReportFormat = value;
            }
        }

        private void ExportScorebookTypeDialogForm_Load(object sender, EventArgs e) {

        }

        private void StdFormatButton_Click(object sender, EventArgs e) {
            ReportFormat = "Standard";
        }

        private void IndexFormatButton_Click(object sender, EventArgs e) {
            ReportFormat = "Index";
        }

        private void MagazineFormatButton_Click( object sender, EventArgs e ) {
            ReportFormat = "Magazine";
        }

    }
}
