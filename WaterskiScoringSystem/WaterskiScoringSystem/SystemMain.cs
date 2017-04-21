using System;
using System.Configuration;
using System.Deployment.Application;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Admin;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Slalom;
using WaterskiScoringSystem.Trick;
using WaterskiScoringSystem.Jump;
using Microsoft.Win32;

namespace WaterskiScoringSystem {
    public partial class SystemMain : Form {
        private RegistryKey myAppRegKey = null;

        public SystemMain() {
            InitializeComponent();
        }

        private void SystemMain_Load( object sender, EventArgs e ) {
            //Calculate the application registry key
            String curUserAppRegName = Application.UserAppDataRegistry.ToString();
            String[] curNodes = curUserAppRegName.Split( '/' );
            if ( curNodes.Length <= 1 ) curNodes = curUserAppRegName.Split( '\\' );
            String curAppRegName = "";
            for ( int idx = 1; idx < curNodes.Length - 1; idx++ ) {
                if ( curAppRegName.Length > 0 ) curAppRegName += '\\';
                curAppRegName += curNodes[idx];
            }
            Properties.Settings.Default.AppRegistryName = curAppRegName;

            #region Set application window attributes from application configuration file
            if ( Properties.Settings.Default.Mdi_Width > 0 ) {
                this.Width = Properties.Settings.Default.Mdi_Width;
            }
            if ( Properties.Settings.Default.Mdi_Height > 0 ) {
                this.Height = Properties.Settings.Default.Mdi_Height;
            }
            if ( Properties.Settings.Default.Mdi_Location.X > 0
                && Properties.Settings.Default.Mdi_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.Mdi_Location;
            }
            if ( Properties.Settings.Default.Mdi_Title.Length > 0 ) {
                this.Text = Properties.Settings.Default.Mdi_Title;
            } else {
                this.Text = Properties.Settings.Default.AppTitle;
            }
            #endregion

            #region Check system settings and current database connection
            bool IsFirstRun = false;
            bool showDebugMsgs = false;
            //showDebugMsgs = true;
            String myDataDirectory = "", myDeploymentDirectory = "", myDeploymentDataDirectory = "", curAppConnectString = "";

            myAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
            if ( myAppRegKey != null ) {
                if ( myAppRegKey.GetValue( "ShowDebugValues" ) != null ) {
                    String curShowDebugRegValue = myAppRegKey.GetValue( "ShowDebugValues" ).ToString();
                    if ( curShowDebugRegValue.Equals( "true" ) ) {
                        showDebugMsgs = true;
                    }
                }
            }

            if ( showDebugMsgs ) {
                MessageBox.Show( "Application Execution Information"
                    + "\n StartupPath=" + Application.StartupPath
                    + "\n\n UserAppDataPath=" + Application.UserAppDataPath
                    + "\n\n LocalUserAppDataPath=" + Application.LocalUserAppDataPath
                    + "\n\n ExecutablePath=" + Application.ExecutablePath
                    + "\n\n UserAppDataRegistry=" + Application.UserAppDataRegistry
                    + "\n\n ProductName=" + Application.ProductName
                    + "\n\n ProductVersion=" + Application.ProductVersion
                    );
            }

            //Check and set application version
            try {
                IsFirstRun = ApplicationDeployment.CurrentDeployment.IsFirstRun;
            } catch (Exception ex) {
                IsFirstRun = false;
            }
            try {
                Properties.Settings.Default.AppVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            } catch ( Exception ex ) {
                if ( Properties.Settings.Default.AppVersion == null ) { Properties.Settings.Default.AppVersion = "0.00.00"; }
                if ( Properties.Settings.Default.AppVersion.Length == 0 ) { Properties.Settings.Default.AppVersion = "0.00.00"; }
            }
            //Check and set user data directory
            try {
                myDeploymentDataDirectory = Application.LocalUserAppDataPath;
                myDeploymentDirectory = Application.StartupPath;
                if (myDeploymentDirectory == null) { myDeploymentDirectory = ""; }
                curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;
            } catch (Exception ex) {
                myDeploymentDirectory = "";
                if (myAppRegKey.GetValue( "DataDirectory" ) == null) {
                    myDataDirectory = Application.UserAppDataPath;
                    myAppRegKey.SetValue( "DataDirectory", myDataDirectory );
                }
                myDataDirectory = myAppRegKey.GetValue( "DataDirectory" ).ToString();
                AppDomain.CurrentDomain.SetData( "DataDirectory", myDataDirectory );
            }

            try {
                //Determine if data directory is available in the registry
                //If available in registry than assume that is the most current
                if ( myDeploymentDirectory.Length < 1 ) {
                    myDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\bin\\Debug";
                }
                if ( myDeploymentDataDirectory.Length < 1 ) {
                    myDeploymentDataDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\bin\\Debug";
                }
                
                if ( myAppRegKey.GetValue( "DataDirectory" ) == null ) {
                    myDataDirectory = "";
                } else {
                    myDataDirectory = myAppRegKey.GetValue( "DataDirectory" ).ToString();
                }
                #region Debug Msgs
                if ( showDebugMsgs ) {
                    StringBuilder tmpMsg = new StringBuilder();
                    try {
                        tmpMsg.Append( "\n DataDirectory=" + ApplicationDeployment.CurrentDeployment.DataDirectory );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n CommonAppDataPath=" + Application.CommonAppDataPath );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n\n CurrentVersion=" + ApplicationDeployment.CurrentDeployment.CurrentVersion );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n\n IsFirstRun=" + ApplicationDeployment.CurrentDeployment.IsFirstRun );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n\n AppConnectString=" + curAppConnectString );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n\n myDataDirectory=" + myDataDirectory );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n myDeploymentDirectory=" + myDeploymentDirectory );
                    } catch {
                    }
                    try {
                        tmpMsg.Append( "\n myDeploymentDataDirectory=" + myDeploymentDataDirectory );
                    } catch {
                    }
                    MessageBox.Show( "Application Execution Information - CurrentDeployment" + tmpMsg.ToString() );
                }
                #endregion
                if (myDataDirectory.Length > 1) {
                    if ( showDebugMsgs ) {
                        MessageBox.Show( "Application Execution Information - CurrentDeployment"
                            + "\n myAppRegKey DataDirectory=" + myDataDirectory );
                    }
                    //If first run after a new version has been loaded or on first loaded
                    //IsFirstRun = true;
                    if ( IsFirstRun ) {
                        //Save current version to registry
                        myAppRegKey.SetValue( "CurrentVersion", Properties.Settings.Default.AppVersion );
                        //Establish active data directory in active application domain
                        AppDomain.CurrentDomain.SetData( "DataDirectory", myDataDirectory );
                        //Check for existing database in active data directory
                        //If one exists give user option to keep existing or loading application default
                        //If one does not exist load application default to user data directory
                        if ( checkDbConnection( curAppConnectString ) ) {
                            //Eliminate the option to replace database because data was being inadvertently overlayed.
                        } else {
                            MessageBox.Show( "Database not found at the specified location " + myDataDirectory
                                + "\n\n" + "In the file open dialog to follow please select a location and provide a file name"
                                + "\n" + "The database supplied with the application will be copied to your specified location" );
                            copyDatabase( myDeploymentDataDirectory, myDataDirectory, myAppRegKey );
                        }
                    } else {
                        //Establish active data directory in active application domain
                        AppDomain.CurrentDomain.SetData( "DataDirectory", myDataDirectory );
                        String curPropAppVersion = Properties.Settings.Default.AppVersion;
                        String curRegAppVersion = "";
                        try {
                            curRegAppVersion = myAppRegKey.GetValue( "CurrentVersion" ).ToString();
                        } catch ( Exception ex ) {
                            curRegAppVersion = "0.00.00";
                        }

                        if ( !( curPropAppVersion.Equals( curRegAppVersion ) ) ) {
                            //New version has been detected.  Save new version in registry
                            //Determine if valid database exists at current data directory location.
                            //If one exists give user option to keep existing or loading application default
                            //If one does not exist load application default to user data directory
                            myAppRegKey.SetValue( "CurrentVersion", Properties.Settings.Default.AppVersion );
                            if ( checkDbConnection( curAppConnectString ) ) {
                            } else {
                                MessageBox.Show( "Database not found at the specified location " + myDataDirectory
                                    + "\n\n" + "In the file open dialog to follow please select a location and provide a file name"
                                    + "\n" + "The database supplied with the application will be copied to your specified location" );
                                copyDatabase( myDeploymentDataDirectory, myDataDirectory, myAppRegKey );
                            }
                        }
                    }
                } else {
                    //Establish current data directry using application default
                    //Analyze default location and establish the desired data directory
                    myDataDirectory = myDeploymentDirectory;
                    int posDelim = myDataDirectory.IndexOf( "/Data" );
                    if ( posDelim > 0 ) {
                        String tmpDataDirectory = myDataDirectory.Substring( 0, posDelim + 5 );
                        if ( showDebugMsgs ) {
                            MessageBox.Show( "tmpDataDirectory = " + tmpDataDirectory );
                        }
                        //Establish active data directory in active application domain
                        myDataDirectory = tmpDataDirectory;
                    }
                    MessageBox.Show( "Database location has not been specified"
                        + "\n\n" + "In the next dialog that displays please select a location and provide a file name for the database (default name is waterski.sdf) "
                        + "\n" + "The database supplied with the application will be copied to your specified location" );
                    copyDatabase( myDeploymentDataDirectory, myDataDirectory, myAppRegKey );
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered during application load \n" + ex.Message );
            }

            myDataDirectory = myAppRegKey.GetValue( "DataDirectory" ).ToString();
            curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;
            if ( checkDbConnection( curAppConnectString ) ) {
                UpgradeDatabase curUpgradeDatabase = new UpgradeDatabase();
                curUpgradeDatabase.checkForUpgrade();

                //Check to ensure currently active tournament is a valid tournament in the current database
                String mySanctionNum = Properties.Settings.Default.AppSanctionNum;
                if ( mySanctionNum != null ) {
                    if ( mySanctionNum.Length == 6 ) {
                        Log.OpenFile( null );
                        DataTable curTourDataTable = getTourData( mySanctionNum );
                        if (curTourDataTable.Rows.Count > 0) {
                            TourProperties curTourProperties = TourProperties.Instance;
                            curTourProperties.loadProperties( (String)curTourDataTable.Rows[0]["Rules"], (String)curTourDataTable.Rows[0]["Class"] );
                        } else {
                            Properties.Settings.Default.AppSanctionNum = "";
                            this.Text = Properties.Settings.Default.AppTitle;
                        }
                    } else {
                        Properties.Settings.Default.AppSanctionNum = "";
                        this.Text = Properties.Settings.Default.AppTitle;
                    }
                }

            } else {
                Properties.Settings.Default.AppSanctionNum = "";
                this.Text = Properties.Settings.Default.AppTitle;

                MessageBox.Show( "Database connection failed"
                    + "\n Use the Tools menu,  SetDatabaseLocaton option, to find and set your database"
                    + "\n\n DataDirectory: " + myDataDirectory
                    + "\n\n AppConnectString: " + curAppConnectString );
            }

            if (showDebugMsgs) {
                MessageBox.Show( "Application Execution Information"
                    + "\n StartupPath=" + Application.StartupPath
                    + "\n\n UserAppDataPath=" + Application.UserAppDataPath
                    + "\n\n LocalUserAppDataPath=" + Application.LocalUserAppDataPath
                    + "\n\n ExecutablePath=" + Application.ExecutablePath
                    + "\n\n UserAppDataRegistry=" + Application.UserAppDataRegistry
                    + "\n\n ProductName=" + Application.ProductName
                    + "\n\n ProductVersion=" + Application.ProductVersion
                    );
            }
            #endregion
        }

        private bool copyDatabase( String inSourDir, String inDestDir, RegistryKey inAppRegKey ) {
            SetDatabaseLocation curForm = new SetDatabaseLocation();
            //Application.StartupPath
            return curForm.copyDatabaseFile( inSourDir, inDestDir, inAppRegKey );
        }

        private bool checkDbConnection( String inConnectString ) {
            bool dbConnGood = false;
            SqlCeConnection myDbConn = null;
            try {
                myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                myDbConn.ConnectionString = inConnectString;
                myDbConn.Open();
                dbConnGood = true;
            } catch ( Exception ex ) {
                dbConnGood = false;
                MessageBox.Show( "checkDbConnection:Exception: " + ex.Message );
            } finally {
                if ( myDbConn != null ) {
                    myDbConn.Close();
                }
            }
            return dbConnGood;
        }

        private void SystemMain_FormClosed( object sender, FormClosedEventArgs e ) {
            DataAccess.DataAccessClose( true );
            Log.CloseFile();
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.Mdi_Width = this.Size.Width;
                Properties.Settings.Default.Mdi_Height = this.Size.Height;
                Properties.Settings.Default.Mdi_Location = this.Location;
            }
            Properties.Settings.Default.Mdi_Title = this.Text;
            Properties.Settings.Default.Save();
        }

        private void navTournamentList_Click( object sender, EventArgs e ) {
            TourList curForm = new TourList();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Check for open instance of selected form
            for ( int idx = 0; idx < this.MdiChildren.Length; idx++ ) {
                if ( ( (Form)this.MdiChildren[idx] ).Name == curForm.Name ) {
                    ( (Form)this.MdiChildren[idx] ).Activate();
                    return;
                }
            }

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navMemberList_Click( object sender, EventArgs e ) {
            MemberList curForm = new MemberList();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Check for open instance of selected form
            for ( int idx = 0; idx < this.MdiChildren.Length; idx++ ) {
                if ( ( (Form)this.MdiChildren[idx] ).Name == curForm.Name ) {
                    ( (Form)this.MdiChildren[idx] ).Activate();
                    return;
                }
            }

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private void navImportData_Click( object sender, EventArgs e ) {
            ImportData myImportData = new ImportData();
            myImportData.importData();
        }

        private void navNopsCalculator_Click( object sender, EventArgs e ) {
            NopsCalcForm curForm = new NopsCalcForm();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navListMaintenance_Click( object sender, EventArgs e ) {
            ListMaintenance curForm = new ListMaintenance();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navNopsDataMainenance_Click( object sender, EventArgs e ) {
            ListNopsMaint curForm = new ListNopsMaint();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navViewRankingList_Click( object sender, EventArgs e ) {
            SkierRankingList curForm = new SkierRankingList();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSetDatabase_Click( object sender, EventArgs e ) {
            DataAccess.DataAccessClose( true );
            Log.CloseFile();
            SetDatabaseLocation curForm = new SetDatabaseLocation();
            curForm.getDatabaseFile( myAppRegKey );
        }

        private void shrinkDatabaseToolStripMenuItem_Click( object sender, EventArgs e ) {
            ShrinkDatabase curForm = new ShrinkDatabase();
            bool curReturn = curForm.Shrink();
        }

        private void navRegistration_Click( object sender, EventArgs e ) {
            Registration curForm = new Registration();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTourRunningOrder_Click( object sender, EventArgs e ) {
            RunningOrderTour curForm = new RunningOrderTour();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navDivOrder_Click( object sender, EventArgs e ) {
            TourDivOrder curForm = new TourDivOrder();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private void navTeamMngt_Click( object sender, EventArgs e ) {
            TeamSetup curForm = new TeamSetup();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private void navSlalomEntry_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Slalom.ScoreEntry curForm = new WaterskiScoringSystem.Slalom.ScoreEntry();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomEntryR3_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Slalom.ScoreEntry curForm = new WaterskiScoringSystem.Slalom.ScoreEntry();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            curForm.setShowByNumJudges( 3 );
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navExit_Click( object sender, EventArgs e ) {
            this.Close();
        }

        private void navTrick_Click( object sender, EventArgs e ) {
        }

        private void navTrickListMaint_Click( object sender, EventArgs e ) {
            TrickListMaint curForm = new TrickListMaint();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickCalc_Click( object sender, EventArgs e ) {
            Trick.ScoreCalc curForm = new Trick.ScoreCalc();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpEntry_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Jump.ScoreEntrySeg3 curForm = new WaterskiScoringSystem.Jump.ScoreEntrySeg3();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            curForm.NumJudges = 3;
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpEntryVideo3Seg_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Jump.ScoreEntrySeg3 curForm = new WaterskiScoringSystem.Jump.ScoreEntrySeg3();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomRunningOrder_Click( object sender, EventArgs e ) {
            RunningOrderTour curForm = new RunningOrderTour();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            curForm.RunningOrderForEvent( "Slalom" );
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickRunningOrder_Click( object sender, EventArgs e ) {
            RunningOrderTour curForm = new RunningOrderTour();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            curForm.RunningOrderForEvent( "Trick" );
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpRunningOrder_Click( object sender, EventArgs e ) {
            RunningOrderTour curForm = new RunningOrderTour();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            curForm.RunningOrderForEvent( "Jump" );
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomSummary_Click( object sender, EventArgs e ) {
            SlalomSummary curForm = new SlalomSummary();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomSummaryAwards_Click(object sender, EventArgs e) {
            SlalomSummaryAwards curForm = new SlalomSummaryAwards();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomPassDetailExport_Click( object sender, EventArgs e ) {
            SlalomPassDetailExport curForm = new SlalomPassDetailExport();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickSummary_Click(object sender, EventArgs e) {
            TrickSummary curForm = new TrickSummary();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickSummaryAwards_Click(object sender, EventArgs e) {
            TrickSummaryAwards curForm = new TrickSummaryAwards();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpSummary_Click(object sender, EventArgs e) {
            JumpSummary curForm = new JumpSummary();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpSummaryAward_Click(object sender, EventArgs e) {
            JumpSummaryAwards curForm = new JumpSummaryAwards();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomScorebook_Click(object sender, EventArgs e) {
            SlalomScorebook curForm = new SlalomScorebook();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickScorebook_Click( object sender, EventArgs e ) {
            TrickScorebook curForm = new TrickScorebook();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpScorebook_Click( object sender, EventArgs e ) {
            JumpScorebook curForm = new JumpScorebook();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navOfficialWorkAssignment_Click( object sender, EventArgs e ) {
            Tournament.OfficialWorkAsgmt curForm = new Tournament.OfficialWorkAsgmt();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navOfficialsWorkRecord_Click( object sender, EventArgs e ) {
            Tournament.OfficialWorkRecord curForm = new Tournament.OfficialWorkRecord();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navMasterScorebook_Click( object sender, EventArgs e ) {
            Tournament.MasterScorebook curForm = new Tournament.MasterScorebook();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navMetricTool_Click( object sender, EventArgs e ) {
            Tools.MetricConv curForm = new Tools.MetricConv();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navMeterSetup_Click( object sender, EventArgs e ) {
            JumpMeterSetup curForm = new JumpMeterSetup();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navLoadIwwfHomologation_Click( object sender, EventArgs e ) {
            LoadIwwfHomologation curLoadFile = new LoadIwwfHomologation();
            curLoadFile.LoadFile();
        }

        private void navTourPackage_Click( object sender, EventArgs e ) {
            TourPackageBuild curForm = new TourPackageBuild();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navBoatUse_Click( object sender, EventArgs e ) {
            BoatUse curForm = new BoatUse();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSlalomTeamSummary_Click( object sender, EventArgs e ) {
            SlalomSummaryTeam curForm = new SlalomSummaryTeam();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navJumpTeamSummary_Click( object sender, EventArgs e ) {
            JumpSummaryTeam curForm = new JumpSummaryTeam();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTrickTeamSummary_Click( object sender, EventArgs e ) {
            TrickSummaryTeam curForm = new TrickSummaryTeam();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navTeamSummary_Click( object sender, EventArgs e ) {
            //TeamSummary curForm = new TeamSummary();
            MasterSummaryTeam curForm = new MasterSummaryTeam();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navMasterSummary_Click( object sender, EventArgs e ) {
            //TeamSummary curForm = new TeamSummary();
            MasterSummaryOverallUS curForm = new MasterSummaryOverallUS();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private void navEventRunStats_Click( object sender, EventArgs e ) {
            EventRunStats curForm = new EventRunStats();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private void navImportFile_Click( object sender, EventArgs e ) {
            ImportData myImportData = new ImportData();
            if ( myImportData.importMemberDataCurrent() ) {
                MessageBox.Show( "Member data import complete." );
            }
        }

        private void navImportNcwsaFile_Click( object sender, EventArgs e ) {
            ImportData myImportData = new ImportData();
            if ( myImportData.importMemberDataNcwsa() ) {
                MessageBox.Show( "Member data import complete." );
            }
        }

        private void overviewToolStripMenuItem_Click( object sender, EventArgs e ) {
            MessageBox.Show( "See the WstimsForWindowsRefGuide.pdf for full documentation."
                + "\n\n Document can be downloaded from \n http://awsaeast.com/scoring/WstimsForWindowsRefGuide.pdf" );

            //"WstimsForWindowsRefGuide.pdf"
            //        + "\n StartupPath=" + Application.StartupPath
            //        + "\n\n UserAppDataPath=" + Application.UserAppDataPath

            //System.xxDiagnostics.Process.Start( Application.StartupPath + "\\WstimsForWindowsRefGuide.pdf" );
        }

        private void navHelpAbout_Click( object sender, EventArgs e ) {
            HelpAbout curForm = new HelpAbout();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void fontTestToolStripMenuItem_Click( object sender, EventArgs e ) {
            FontDialog curFontDialog = new FontDialog();
            if ( curFontDialog.ShowDialog() == DialogResult.OK ) {
                Font curFont = curFontDialog.Font;
            }
        }

        private void databaseToolToolStripMenuItem_Click( object sender, EventArgs e ) {
            SqlCommandTool curForm = new SqlCommandTool();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void loadDataFromLog_Click(object sender, EventArgs e) {
            LogLoadForm curForm = new LogLoadForm();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navLoadVideoFiles_Click(object sender, EventArgs e) {
            LoadVideosFile curForm = new LoadVideosFile();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";

        }

        private DataTable getTourData(String inSanctionNum) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
