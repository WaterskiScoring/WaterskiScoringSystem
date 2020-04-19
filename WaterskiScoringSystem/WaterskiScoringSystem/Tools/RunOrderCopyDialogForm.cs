using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class RunOrderCopyDialogForm : Form {
        private String myCommand;

        public RunOrderCopyDialogForm() {
            InitializeComponent();
        }

        private void RunOrderCopyDialogForm_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.RunOrderCopy_Width > 0 ) {
                this.Width = Properties.Settings.Default.RunOrderCopy_Width;
            }
            if ( Properties.Settings.Default.RunOrderCopy_Height > 0 ) {
                this.Height = Properties.Settings.Default.RunOrderCopy_Height;
            }
            if ( Properties.Settings.Default.RunOrderCopy_Location.X > 0
                && Properties.Settings.Default.RunOrderCopy_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.RunOrderCopy_Location;
            }
        }

        private void RunOrderCopyDialogForm_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.RunOrderCopy_Width = this.Size.Width;
                Properties.Settings.Default.RunOrderCopy_Height = this.Size.Height;
                Properties.Settings.Default.RunOrderCopy_Location = this.Location;
            }
        }

        public String Command {
            get {
                return myCommand;
            }
            set {
                myCommand = value;
            }
        }

        private void CopyPrevButton_Click( object sender, EventArgs e ) {
            myCommand = "Copy";
        }

        private void CopyByBestButton_Click( object sender, EventArgs e ) {
            myCommand = "CopyByBest";
        }

        private void CopyByLastButton_Click( object sender, EventArgs e ) {
            myCommand = "CopyByLast";
        }

        private void CopyByTotalButton_Click( object sender, EventArgs e ) {
            myCommand = "CopyByTotal";
        }

    }
}
