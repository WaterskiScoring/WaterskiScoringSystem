using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.Windows.Forms;

using Microsoft.Win32;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Admin {
    public partial class HelpAbout : Form {
        private SqlCeConnection myDbConn = null;

        public HelpAbout() {
            InitializeComponent();
        }

        private void HelpAbout_Load( object sender, EventArgs e ) {
            String curDeployVer = "", curUpdatedVer = "", curDataDir = "";
            String curDatabaseFilename = "";
            String curConnFilename = "\\Not available";

            try {
                curDeployVer = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            } catch {
                curDeployVer = "Not available";
            }
            try {
                curUpdatedVer = ApplicationDeployment.CurrentDeployment.UpdatedVersion.ToString();
            } catch {
                curUpdatedVer = "Not available";
            }
            try {
                curDataDir = ApplicationDeployment.CurrentDeployment.DataDirectory;
                
                try {
                    String curConnString = Properties.Settings.Default.waterskiConnectionStringApp;
                    int curDelimPos1 = curConnString.IndexOf( "\\" );
                    int curDelimPos2 = curConnString.IndexOf( ";" );
                    if ( curDelimPos1 > 0 && curDelimPos2 > 0 ) curConnFilename = curConnString.Substring( curDelimPos1, curDelimPos2 - curDelimPos1 );
                    curDatabaseFilename = curDataDir + curConnFilename;
                } catch {
                    curDatabaseFilename = curDataDir;
                }
            } catch {
                curDataDir = "Not available";
            }
            textDatabasePath.Text = curDatabaseFilename;

            textConnectionString.Text = Properties.Settings.Default.waterskiConnectionStringApp;
            textExecutablePath.Text = Application.ExecutablePath;
            textProductName.Text = Application.ProductName;
            
            textProductVersion.Text = curDeployVer
                + " | " + Application.ProductVersion
                + " | " + Properties.Settings.Default.AppVersion
                + " | " + curUpdatedVer
                + " | " + Properties.Settings.Default.BuildVersion;
            textStartupPath.Text = Application.StartupPath;
            textLocalUserAppDataPath.Text = Application.LocalUserAppDataPath;
            textUserAppDataPath.Text = Application.UserAppDataPath;
            textUserAppDataRegistry.Text = Application.UserAppDataRegistry.Name;

            try {
                if ( openDbConn() ) {
                    SqlCeCommand sqlStmt = myDbConn.CreateCommand();
                    String curSqlStmt = "SELECT MinValue as VersionNum FROM CodeValueList WHERE ListName = 'DatabaseVersion'";
                    DataTable curDataTable = getData( curSqlStmt );
                    if ( curDataTable.Rows.Count > 0 ) {
                        decimal curDatabaseVersion = (decimal)curDataTable.Rows[0]["VersionNum"];
                        textDatabaseVersion.Text = curDatabaseVersion.ToString();
                    }
                } else {
                    textDatabaseVersion.Text = "Not available";
                }
            } catch {
                textDatabaseVersion.Text = "Not available";
            }

            try {
                RegistryKey curAppRegKey = null;
                String curAppRegName = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v3.5";
                curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName );
                if ( curAppRegKey == null ) {
                    NetFramework35Info.Text = "Not available";
                } else {
                    if ( curAppRegKey.GetValue( "Version" ) != null ) {
                        NetFramework35Info.Text = curAppRegKey.GetValue( "Version" ).ToString()
                            + " SP=" + curAppRegKey.GetValue( "SP" ).ToString();
                        curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName + "\\1033" );
                        if ( curAppRegKey != null ) {
                            NetFramework35Info.Text += " (1033:" 
                                + curAppRegKey.GetValue( "Version" ).ToString()
                                + " SP=" + curAppRegKey.GetValue( "SP" ).ToString()
                                + ")";
                        }
                    } else {
                        NetFramework35Info.Text = "Not available";
                    }
                }
            } catch {
                NetFramework35Info.Text = "Not available";
            }

            try {
                RegistryKey curAppRegKey = null;
                String curAppRegName = "SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";
                curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName );
                if ( curAppRegKey == null ) {
                    NetFramework35Info.Text = "Not available";
                } else {
                    if ( curAppRegKey.GetValue( "Version" ) != null ) {
                        NetFramework40Info.Text = curAppRegKey.GetValue( "Version" ).ToString();
                        curAppRegKey = Registry.LocalMachine.OpenSubKey( curAppRegName + "\\1033" );
                        if ( curAppRegKey != null ) {
                            NetFramework40Info.Text += " (1033:"
                                + curAppRegKey.GetValue( "Version" ).ToString()
                                + ")";
                        }
                    } else {
                        NetFramework40Info.Text = "Not available";
                    }
                }
            } catch {
                NetFramework40Info.Text = "Not available";
            }
        }

        private bool openDbConn(  ) {
            bool curReturnValue = true;
            try {
                if ( myDbConn == null ) {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                    myDbConn.Open();
                }
            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                MessageBox.Show( "Error connecting to database " + "\n\nError: " + ExcpMsg );
            }
            return curReturnValue;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
