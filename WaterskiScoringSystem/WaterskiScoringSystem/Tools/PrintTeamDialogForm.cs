using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class TourExportDialogForm : Form {
        public TourExportDialogForm() {
            InitializeComponent();
        }

        private String outExportCommand;

        public String ExportCommand {
            get {
                return outExportCommand;
            }
            set {
                outExportCommand = value;
            }
        }

        private void TourExportDialogForm_Load(object sender, EventArgs e) {

        }

        private void DataButton_Click(object sender, EventArgs e) {
            ExportCommand = "Data";
        }

        private void ListButton_Click(object sender, EventArgs e) {
            ExportCommand = "List";
        }

        private void WspButton_Click(object sender, EventArgs e) {
            ExportCommand = "Perf";
        }
    }
}
