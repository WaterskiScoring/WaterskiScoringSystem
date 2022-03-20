using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class ImportMatchDialogForm : Form {
        public ImportMatchDialogForm() {
            InitializeComponent();
        }

        private void ImportMatchDialogForm_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.ImportMatch_Location.X > 0
                && Properties.Settings.Default.ImportMatch_Location.Y > 0) {
                this.Location = Properties.Settings.Default.ImportMatch_Location;
            }
            if (MatchCommand.ToLower().Equals("update")) {
                UpdateButton.Focus();
            } else if (MatchCommand.ToLower().Equals("updateall")) {
                UpdateAllButton.Focus();
            } else if (MatchCommand.ToLower().Equals("skip")) {
                SkipButton.Focus();
            } else if (MatchCommand.ToLower().Equals("skipall")) {
                SkipAllButton.Focus();
            } else {
                SkipButton.Focus();
            }
        }

        private String myMatchCommand = "";

        public String ImportKeyData {
            get {
                return ShowImportKeyData.Text;
            }
            set {
                ShowImportKeyData.Text = value;
            }
        }

        public String[] ImportKeyDataMultiLine {
            set {
                ShowImportKeyData.Lines = value;
            }
        }

        public String MatchCommand {
            get {
                return myMatchCommand.ToLower();
            }
            set {
                myMatchCommand = value;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e) {
            MatchCommand = "Update";
        }

        private void SkipButton_Click(object sender, EventArgs e) {
            MatchCommand = "Skip";
        }

        private void UpdateAllButton_Click(object sender, EventArgs e) {
            MatchCommand = "UpdateAll";
        }

        private void SkipAllButton_Click(object sender, EventArgs e) {
            MatchCommand = "SkipAll";
        }

        private void ImportMatchDialogForm_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.ImportMatch_Location = this.Location;
            }
        }

    }
}
