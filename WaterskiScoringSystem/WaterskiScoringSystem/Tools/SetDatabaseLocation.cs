using System;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class SetDatabaseLocation : Form {
        public SetDatabaseLocation() {
            InitializeComponent();
        }

        private void SetDatabaseLocation_Load( object sender, EventArgs e ) {
        }

        public bool getDatabaseFile(RegistryKey inAppRegKey) {
            bool curReturn = false;
            String curDataDirectory = "", curDestFileName = "";
            String curAppConnectString = DataAccess.getConnectionString();
            String curFileName = DataAccess.getDatabaseFilename();
            curDataDirectory = Path.GetDirectoryName( curFileName );

            //Establish active data directory in active application domain
            MessageBox.Show( "Current Database Connection String \n "
                + "\n ConnectionString: " + curAppConnectString
                + "\n\n Data location: " + curDataDirectory
                );
            OpenFileDialog curFileDialog = new OpenFileDialog();
            curFileDialog.InitialDirectory = curDataDirectory;
            curFileDialog.Filter = "database files (*.sdf)|*.sdf|All files (*.*)|*.*";
            curFileDialog.FilterIndex = 0;
            curFileDialog.CheckPathExists = false;
            curFileDialog.CheckFileExists = false;

            try {
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        curDataDirectory = Path.GetDirectoryName( curFileName );
                        inAppRegKey.SetValue( "DataDirectory", curDataDirectory );
                        curDestFileName = Path.GetFileName( curFileName );
                        curReturn = setConnectionString( curDataDirectory, curDestFileName, inAppRegKey );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get database file " + "\n\nError: " + ex.Message );
            }

            return curReturn;
        }

        public bool setConnectionString( String inDataDirectory, String inDatabaseFileName, RegistryKey inAppRegKey ) {
            bool curReturn = false;
            String curAttrName, curAttrValue, newConnectionString = "";
            String curAppConnectString = "";
            if ( inAppRegKey.GetValue( "DatabaseConnectionString" ) == null ) {
                curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;

            } else {
                curAppConnectString = inAppRegKey.GetValue( "DatabaseConnectionString" ).ToString();
            }

            String[] curAttrEntry;
            String[] curConnAttrList = curAppConnectString.Split( ';' );
            for ( int idx = 0; idx < curConnAttrList.Length; idx++ ) {
                curAttrEntry = curConnAttrList[idx].Split( '=' );
                curAttrName = curAttrEntry[0];
                curAttrValue = curAttrEntry[1];
                if ( newConnectionString.Length > 1 ) { newConnectionString += ";"; }
                if ( curAttrName.ToLower().Trim().Equals( "data source" ) ) {
                    newConnectionString += curAttrName + "=|DataDirectory|\\" + inDatabaseFileName;
                } else {
                    newConnectionString += curAttrName + "=" + curAttrValue;
                }
            }
            Properties.Settings.Default.waterskiConnectionStringApp = newConnectionString;
            inAppRegKey.SetValue( "DataDirectory", inDataDirectory );
            AppDomain.CurrentDomain.SetData( "DataDirectory", inDataDirectory );

            SqlCeConnection curDbConn = null;
            try {
                DataAccess.DataAccessClose( true );

                curDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                curDbConn.ConnectionString = newConnectionString;
                curDbConn.Open();

                inAppRegKey.SetValue( "DatabaseConnectionString", newConnectionString );
                UpgradeDatabase curUpgradeDatabase = new UpgradeDatabase();
                curUpgradeDatabase.checkForUpgrade();

                MessageBox.Show( "Database connection successful!"
                    + "\n ConnectionString: " + newConnectionString
                    + "\n\n Data location: " + AppDomain.CurrentDomain.GetData( "DataDirectory" )
                    + "\n\n You must close the application and restart it to use the new database selection"
                    );
                curReturn = true;
            
            } catch ( Exception ex ) {
                MessageBox.Show( "Database connection failed!"
                    + "\n ConnectionString: " + curDbConn.ConnectionString
                    + "\n\n Data location: " + inDataDirectory 
                    );

            } finally {
                if ( curDbConn != null ) {
                    curDbConn.Close();
                }
            }

            return curReturn;
        }

		public bool backupDatabaseFile( RegistryKey inAppRegKey ) {

			try {
				String curDatabaseFilename = DataAccess.getDatabaseFilename();
				String curDestFileName = curDatabaseFilename + "." + DateTime.Now.ToString( "yyyyMMddHHmm" ) + ".bak";
				File.Copy( curDatabaseFilename, curDestFileName );
				MessageBox.Show( "Current database " + curDatabaseFilename + "\nBacked up to " + curDestFileName );
				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get database file " + "\n\nError: " + ex.Message );
			}

			return false;
		}

	}
}
