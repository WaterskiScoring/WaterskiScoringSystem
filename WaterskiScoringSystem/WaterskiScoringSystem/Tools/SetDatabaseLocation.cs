using System;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.IO;
using System.Windows.Forms;

using Microsoft.Win32;

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
            String myFileName = null;
            String myConnectName = WaterskiScoringSystem.Properties.Settings.Default.AppConnectName;
            String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;

            String curAppRegName = Properties.Settings.Default.AppRegistryName;
            RegistryKey curAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
            if ( curAppRegKey.GetValue( "DataDirectory" ) == null ) {
                try {
                    curDataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
                } catch ( Exception ex ) {
                    curDataDirectory = Application.UserAppDataPath;
                }
            } else {
                curDataDirectory = curAppRegKey.GetValue( "DataDirectory" ).ToString();
            }

            MessageBox.Show( "Current Database Connection String \n "
                + "\n ConnectionString: " + curAppConnectString
                + "\n\n Data location: " + curDataDirectory
                );
            OpenFileDialog myFileDialog = new OpenFileDialog();
            myFileDialog.InitialDirectory = curDataDirectory;
            myFileDialog.Filter = "database files (*.sdf)|*.sdf|All files (*.*)|*.*";
            myFileDialog.FilterIndex = 0;
            myFileDialog.CheckPathExists = false;
            myFileDialog.CheckFileExists = false;

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int posDelim = myFileName.LastIndexOf( "\\" );
                        curDataDirectory = myFileName.Substring( 0, posDelim);
                        curAppRegKey.SetValue( "DataDirectory", curDataDirectory );
                        curDestFileName = myFileName.Substring( posDelim + 1 );
                        curReturn = setConnectionString( curDataDirectory, curDestFileName, inAppRegKey );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get database file " + "\n\nError: " + ex.Message );
            }

            return curReturn;
        }

		public String getDatabaseFilename() {
			String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;

			String curAttrName, curAttrValue;
			String[] curAttrEntry;
			String[] curConnAttrList = curAppConnectString.Split( ';' );
			for ( int idx = 0; idx < curConnAttrList.Length; idx++ ) {
				curAttrEntry = curConnAttrList[idx].Split( '=' );
				curAttrName = curAttrEntry[0];
				curAttrValue = curAttrEntry[1];
				if ( curAttrName.ToLower().Trim().Equals( "data source" ) ) {
					int delimPos = curAttrValue.LastIndexOf( '\\' );
					if ( delimPos > 0 ) {
						return curAttrValue.Substring( delimPos + 1 );
					}
				}
			}

			return "";
		}

		public bool copyDatabaseFile( String inSourDir, String inDestDir, RegistryKey inAppRegKey ) {
            bool curReturn = false;
            String curDataDirectory = "", curSourDir = "";
			String curDestDatabaseRef = "", curDestFileName = "";

            OpenFileDialog myFileDialog = new OpenFileDialog();
            myFileDialog.InitialDirectory = inDestDir;
            myFileDialog.FileName = getDatabaseFilename();
			myFileDialog.Filter = "database files (*.sdf)|*.sdf|All files (*.*)|*.*";
            myFileDialog.FilterIndex = 0;
            myFileDialog.CheckPathExists = false;
            myFileDialog.CheckFileExists = false;

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    curDestDatabaseRef = myFileDialog.FileName;

                    if ( curDestDatabaseRef == null ) { curDestDatabaseRef = ""; }
                    if ( curDestDatabaseRef.Length > 1 ) {
                        int posDelim = curDestDatabaseRef.LastIndexOf( "\\" );
                        curDataDirectory = curDestDatabaseRef.Substring( 0, posDelim );
                        curDestFileName = curDestDatabaseRef.Substring( posDelim + 1 );

                        if ( inSourDir.Substring( inSourDir.Length - 1 ).Equals( "\\" ) ) {
                            curSourDir = inSourDir;
                        } else {
                            curSourDir = inSourDir + "\\";
                        }

                        //Declare and instantiate a new process component.
                        System.Diagnostics.Process curOSProcess = new System.Diagnostics.Process();

                        //Do not receive an event when the process exits.
                        curOSProcess.EnableRaisingEvents = true;

                        String curDestCopyFileName = "";
                        string curCmdLine;
                        if (File.Exists( curDestDatabaseRef )) {
                            curDestCopyFileName = curDestDatabaseRef + "." + DateTime.Now.ToString( "MMddyyHHmm" ) + ".bak";
                            curCmdLine = "/C copy \"" + curDestDatabaseRef + "\" \"" + curDestCopyFileName + "\" \n";
                            System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
                        }

                        //The "/C" Tells Windows to Run The Command then Terminate 
                        curCmdLine = "/C copy \"" + curSourDir + "waterski.sdf\" \"" + curDestDatabaseRef + "\" \n";
                        System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
                        curOSProcess.Close();
                        MessageBox.Show( "copyDatabase command \n" + curCmdLine
                            + "\n\nCheck the destination location for a waterski.sdf"
                            + "\nDestination folder = " + curDestDatabaseRef
                            + "\n\nIf the file is not found then use the following URL to download the database"
                            + "\nhttp://awsaeast.com/scoring/waterski.sdf"
                            );
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
            String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;

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

            Boolean dbConnGood = false;
            SqlCeConnection myDbConn = null;
            try {
                myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                myDbConn.ConnectionString = newConnectionString;
                myDbConn.Open();
                dbConnGood = true;

                UpgradeDatabase curUpgradeDatabase = new UpgradeDatabase();
                curUpgradeDatabase.checkForUpgrade();

                MessageBox.Show( "Database connection successful!"
                    + "\n ConnectionString: " + Properties.Settings.Default.waterskiConnectionStringApp
                    + "\n\n Data location: " + AppDomain.CurrentDomain.GetData( "DataDirectory" )
                    + "\n\n You must close the application and restart it to use the new database selection"
                    );
                curReturn = true;
            } catch ( Exception ex ) {
                dbConnGood = false;
                MessageBox.Show( "Database connection failed!"
                    + "\n ConnectionString: " + myDbConn.ConnectionString
                    + "\n\n Data location: " + inDataDirectory 
                    );

            } finally {
                if ( myDbConn != null ) {
                    myDbConn.Close();
                }
            }

            return curReturn;
        }

		public bool backupDatabaseFile( RegistryKey inAppRegKey ) {
			bool curReturn = false;
			String curBackupDirectory = "";

			String curDataDirectory = "";
			if ( inAppRegKey.GetValue( "DataDirectory" ) == null ) {
				try {
					curDataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
				} catch ( Exception ex ) {
					curDataDirectory = Application.UserAppDataPath;
				}
			} else {
				curDataDirectory = inAppRegKey.GetValue( "DataDirectory" ).ToString();
			}

			String curDatabaseFilename = getDatabaseFilename();

			FolderBrowserDialog curFolderBrowserDialog = new FolderBrowserDialog();
			curFolderBrowserDialog.SelectedPath = curDataDirectory;
			curFolderBrowserDialog.ShowNewFolderButton = true;

			try {
				if ( curFolderBrowserDialog.ShowDialog() == DialogResult.OK ) {
					curBackupDirectory = curFolderBrowserDialog.SelectedPath;

					//Declare and instantiate a new process component
					System.Diagnostics.Process curOSProcess = new System.Diagnostics.Process();

					//Do not receive an event when the process exits.
					curOSProcess.EnableRaisingEvents = true;

					String curCmdLine;
					String curDestCopyFileName = "";
					String curDatabaseLocation = curDataDirectory + "\\" + curDatabaseFilename;

					curDestCopyFileName = curBackupDirectory + "\\" + curDatabaseFilename + "." + DateTime.Now.ToString( "MMddyyHHmm" ) + ".bak";
					curCmdLine = "/C copy \"" + curDatabaseLocation + "\" \"" + curDestCopyFileName + "\" \n";
					System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
					curOSProcess.Close();

					MessageBox.Show( "Current database " + curDatabaseLocation
						+ "\nBacked up to " + curDestCopyFileName
						);
					curReturn = true;
                }

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get database file " + "\n\nError: " + ex.Message );
			}

			return curReturn;
		}

	}
}
