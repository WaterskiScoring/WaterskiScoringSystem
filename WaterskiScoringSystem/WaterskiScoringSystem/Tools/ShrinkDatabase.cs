using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Tools {
    class ShrinkDatabase {

        public ShrinkDatabase() {
        }

        public bool Shrink() {
            bool curReturn = true;
            System.Data.SqlServerCe.SqlCeEngine mySqlEngine = new SqlCeEngine();

            try {
                String curSqlStmt = "";
                mySqlEngine.LocalConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                mySqlEngine.Shrink();
                MessageBox.Show( "Compression complete for connection \n" + mySqlEngine.LocalConnectionString );

            } catch ( Exception ex ) {
                curReturn = false;
                MessageBox.Show( "Error attempting to shrink database"
                    + "Database connection: " + mySqlEngine.LocalConnectionString
                    + "\n\nError: " + ex.Message );
            }

            return curReturn;
        }
    }
}
