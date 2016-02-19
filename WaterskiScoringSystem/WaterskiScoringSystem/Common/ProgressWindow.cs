using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class ProgressWindow : Form {
        public ProgressWindow() {
            InitializeComponent();
        }

        private void ProgressWindow_Load( object sender, EventArgs e ) {
        }

        public void setProgessMsg( String inValue ) {
            progressMsg.Text = inValue;
        }

        public void setProgressMin( int inValue ) {
            processProgressBar.Minimum = inValue;
        }
        public void setProgressMax( int inValue ) {
            processProgressBar.Maximum = inValue;
        }
        public void setProgressValue( int inValue ) {
            processProgressBar.Value = inValue;
        }
    }
}
