using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    public partial class MetricConv : Form {
        //private Decimal myMetericFactor = 3.28084M;
        private Decimal myMetericFactor = 3.2808M;

        public MetricConv() {
            InitializeComponent();
        }

        private void MetricConv_Load(object sender, EventArgs e) {

        }

        private void FeetToMetersButton_Click(object sender, EventArgs e) {
            try {
                Decimal curValue = Convert.ToDecimal( FeetTextBox.Text ) ;
                Decimal curResults = Convert.ToDecimal( curValue / myMetericFactor );
                curResults = Math.Round( curResults, 2 );
                MetersFromFeetLabel.Text = curResults.ToString( "##0.00" );
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered converting feet to meters"
                    + "\n "
                    + "\n Exception message: " + ex.Message );
            }
        }

        private void MetersToFeetButton_Click( object sender, EventArgs e ) {
            try {
                Decimal curValue = Convert.ToDecimal( MeterTextBox.Text );
                Decimal curResults = Convert.ToDecimal( curValue * myMetericFactor );
                curResults = Math.Round( curResults, 1 );
                FeetFromMetersLabel.Text = curResults.ToString( "##0.0" );
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered converting meters to feet"
                    + "\n "
                    + "\n Exception message: " + ex.Message );
            }

        }
    }
}
