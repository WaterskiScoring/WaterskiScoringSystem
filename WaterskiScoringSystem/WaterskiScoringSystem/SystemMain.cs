using System;
using System.Deployment.Application;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;

using WaterskiScoringSystem.Admin;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Jump;
using WaterskiScoringSystem.Slalom;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Trick;

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

            // Set application window attributes from application configuration file
            if ( Properties.Settings.Default.Mdi_Width > 0 ) this.Width = Properties.Settings.Default.Mdi_Width;
            if ( Properties.Settings.Default.Mdi_Height > 0 ) this.Height = Properties.Settings.Default.Mdi_Height;
            if ( Properties.Settings.Default.Mdi_Location.X > 0
                && Properties.Settings.Default.Mdi_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.Mdi_Location;
            }
            if ( Properties.Settings.Default.Mdi_Title.Length > 0 ) {
                this.Text = Properties.Settings.Default.Mdi_Title;
            } else {
                this.Text = Properties.Settings.Default.AppTitle;
            }

            #region Check system settings and current database connection
            bool curShowDebugMsgs = false;
            //curShowDebugMsgs = true;
            String curDataDirectory = "", curDeploymentDirectory = "", curDeploymentDataDirectory = "", curAppConnectString = "", curRegAppVersion = "";

            try {
                Properties.Settings.Default.AppVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            } catch {
                if ( Properties.Settings.Default.AppVersion == null ) { Properties.Settings.Default.AppVersion = "0.00.00"; }
                if ( Properties.Settings.Default.AppVersion.Length == 0 ) { Properties.Settings.Default.AppVersion = "0.00.00"; }
            }
            
            curDeploymentDataDirectory = Application.LocalUserAppDataPath;
            curDeploymentDirectory = Application.StartupPath;
            if ( curDeploymentDirectory == null ) { curDeploymentDirectory = ""; }

            if ( curDeploymentDirectory.Length < 1 ) {
                curDeploymentDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\bin\\Debug";
            }
            if ( curDeploymentDataDirectory.Length < 1 ) {
                curDeploymentDataDirectory = "C:\\Users\\AllenFamily\\Documents\\Visual Studio 2010\\Projects\\WaterskiScoringSystem\\WaterskiScoringSystem\\bin\\Debug";
            }

            /*
             * Retrieve / Create application entry in Windows registry
             * Retrieve attributes used to establish database location and connection parameters
             */
            myAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
            if ( myAppRegKey == null ) {
                myAppRegKey = Registry.CurrentUser.CreateSubKey( curAppRegName );
                myAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
            }
            if ( myAppRegKey.GetValue( "ShowDebugValues" ) == null ) {
                myAppRegKey.SetValue( "ShowDebugValues", "False" );
            } else {
                curShowDebugMsgs = HelperFunctions.isValueTrue( myAppRegKey.GetValue( "ShowDebugValues" ).ToString() );
            }

            if ( myAppRegKey.GetValue( "DataDirectory" ) == null ) {
                setDatabaseFirstTime( curDeploymentDirectory, curDeploymentDataDirectory, curShowDebugMsgs );
                curDataDirectory = myAppRegKey.GetValue( "DataDirectory" ).ToString();

            } else {
                curDataDirectory = myAppRegKey.GetValue( "DataDirectory" ).ToString();
            }
            //Establish active data directory in active application domain
            AppDomain.CurrentDomain.SetData( "DataDirectory", curDataDirectory );

            if ( myAppRegKey.GetValue( "DatabaseConnectionString" ) == null ) {
                curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;
                myAppRegKey.SetValue( "DatabaseConnectionString", curAppConnectString );

            } else {
                curAppConnectString = myAppRegKey.GetValue( "DatabaseConnectionString" ).ToString();
            }

            if ( myAppRegKey.GetValue( "CurrentVersion" ) == null ) {
                curRegAppVersion = "0.00.00";
                myAppRegKey.SetValue( "CurrentVersion", curRegAppVersion );
            } else {
                curRegAppVersion = myAppRegKey.GetValue( "CurrentVersion" ).ToString();
            }

            if ( curShowDebugMsgs ) {
                MessageBox.Show( "Application Execution Information"
                    + "\n StartupPath=" + Application.StartupPath
                    + "\n UserAppDataPath=" + Application.UserAppDataPath
                    + "\n LocalUserAppDataPath=" + Application.LocalUserAppDataPath
                    + "\n ExecutablePath=" + Application.ExecutablePath
                    + "\n UserAppDataRegistry=" + Application.UserAppDataRegistry
                    + "\n ProductName=" + Application.ProductName
                    + "\n ProductVersion=" + Application.ProductVersion
                    + "\n DataDirectory=" + curDataDirectory
                    + "\n DatabaseConnectionString=" + curAppConnectString
                    );
            }

            String curPropAppVersion = Properties.Settings.Default.AppVersion;

            if ( !( curPropAppVersion.Equals( curRegAppVersion ) ) ) {
                //New version has been detected.  Save new version in registry
                //Determine if valid database exists at current data directory location.
                //If one exists give user option to keep existing or loading application default
                //If one does not exist load application default to user data directory
                Properties.Settings.Default.Upgrade();

                myAppRegKey.SetValue( "CurrentVersion", Properties.Settings.Default.AppVersion );
                if ( checkDbConnection( curAppConnectString ) ) {
                } else {
                    MessageBox.Show( "Database not found at the specified location " + curDataDirectory
                        + "\n\n" + "In the file open dialog please select a location and provide a file name"
                        + "\n" + "The database supplied with the application will be copied to your specified location" );
                    copyDatabase( curDeploymentDataDirectory, curDataDirectory );
                }
            }

            String mySanctionNum = "";
            if ( checkDbConnection( curAppConnectString ) ) {
                UpgradeDatabase curUpgradeDatabase = new UpgradeDatabase();
                curUpgradeDatabase.checkForUpgrade();

                //Check to ensure currently active tournament is a valid tournament in the current database
                mySanctionNum = Properties.Settings.Default.AppSanctionNum;
                if ( mySanctionNum != null ) {
                    if ( mySanctionNum.Length == 6 ) {
                        Log.OpenFile();
                        DataTable curTourDataTable = getTourData( mySanctionNum );
                        if (curTourDataTable.Rows.Count > 0) {
                            TourProperties curTourProperties = TourProperties.Instance;
                            curTourProperties.loadProperties( (String)curTourDataTable.Rows[0]["Rules"], (String)curTourDataTable.Rows[0]["Class"] );
							
							this.Text = Properties.Settings.Default.AppTitle
								+ " - " + Properties.Settings.Default.BuildVersion
								+ " - " + Properties.Settings.Default.AppSanctionNum
								+ " - " + (String)curTourDataTable.Rows[0]["Name"];
						
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
                    + "\n\n DataDirectory: " + curDataDirectory
                    + "\n\n AppConnectString: " + curAppConnectString );
            }

            if (curShowDebugMsgs) {
                MessageBox.Show( "Application Execution Information"
                    + "\n AppSanctionNum=" + Properties.Settings.Default.AppSanctionNum
                    + "\n mySanctionNum=" + mySanctionNum
                    + "\n StartupPath=" + Application.StartupPath
                    + "\n UserAppDataPath=" + Application.UserAppDataPath
                    + "\n LocalUserAppDataPath=" + Application.LocalUserAppDataPath
                    + "\n ExecutablePath=" + Application.ExecutablePath
                    + "\n UserAppDataRegistry=" + Application.UserAppDataRegistry
                    + "\n ProductName=" + Application.ProductName
                    + "\n ProductVersion=" + Application.ProductVersion
                    + "\n DataDirectory=" + curDataDirectory
                    + "\n DatabaseConnectionString=" + curAppConnectString
                    );
            }
            #endregion
        }

        /*
        Establish current data directry using application default
        Analyze default location and establish the desired data directory
         */
        private void setDatabaseFirstTime( String curDeploymentDirectory, String curDeploymentDataDirectory, bool curShowDebugMsgs ) {
            String curDataDirectory = curDeploymentDirectory;
            int posDelim = curDataDirectory.IndexOf( "/Data" );
            if ( posDelim > 0 ) {
                String tmpDataDirectory = curDataDirectory.Substring( 0, posDelim + 5 );
                if ( curShowDebugMsgs ) {
                    MessageBox.Show( "tmpDataDirectory = " + tmpDataDirectory );
                }
                //Establish active data directory in active application domain
                curDataDirectory = tmpDataDirectory;
            }
            MessageBox.Show( "Database location has not been specified"
                + "\n\n" + "In the next dialog that displays please select a location and provide a file name for the database (default name is waterski.sdf) "
                + "\n" + "The database supplied with the application will be copied to your specified location" );
            copyDatabase( curDeploymentDataDirectory, curDataDirectory );
        }

        private bool copyDatabase( String inSourDir, String inDestDir ) {
            SetDatabaseLocation curForm = new SetDatabaseLocation();
            //Application.StartupPath
            return curForm.copyDatabaseFile( inSourDir, inDestDir, myAppRegKey );
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
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.Mdi_Width = this.Size.Width;
                Properties.Settings.Default.Mdi_Height = this.Size.Height;
                Properties.Settings.Default.Mdi_Location = this.Location;
            }
            Properties.Settings.Default.Mdi_Title = this.Text;
            Properties.Settings.Default.Save();
        }
        private void SystemMain_FormClosing( object sender, FormClosingEventArgs e ) {
			e.Cancel = false;
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

        private void navImportLwData_Click( object sender, EventArgs e ) {
            ExportFileDownload curForm = new ExportFileDownload();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navImportTourReg_Click( object sender, EventArgs e ) {
			ImportMember curImportMember = new ImportMember( null );
			mdiStatusMsg.Text = "Import tournament registrations opening";
			curImportMember.importPreRegMembers();
		}

		private void importRegistrationsForWorldWaterSkiersToolStripMenuItem_Click( object sender, EventArgs e ) {
			ImportMember curImportMember = new ImportMember( null );
			mdiStatusMsg.Text = "Import tournament registrations opening";
			curImportMember.importWwsRegistrations();
		}

		private void navImportMemberFile_Click( object sender, EventArgs e ) {
			ImportMemberFile curImportMemberFile = new ImportMemberFile();
			mdiStatusMsg.Text = "Import member file";
			curImportMemberFile.importData();
		}

		private void navImportTourRegPick_Click( object sender, EventArgs e ) {
			ImportMember curImportMember = new ImportMember( null, "pick" );
			mdiStatusMsg.Text = "Import tournament registrations opening";
			curImportMember.importPreRegMembers();
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

        private void navPublishReportDelete_Click( object sender, EventArgs e ) {
            PublishReportDelete curForm = new PublishReportDelete();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navPublishPDF_Click( object sender, EventArgs e ) {
            String mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            ExportLiveWeb.uploadReportFile( "Other", "Tour", mySanctionNum );
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

        private void boatPathDriverUpdateToolStripMenuItem_Click( object sender, EventArgs e ) {
            BoatPathDriverUpdate curForm = new BoatPathDriverUpdate();
            mdiStatusMsg.Text = curForm.Name + " opening";

            // Set the Parent Form and display requested form
            curForm.MdiParent = this;
            curForm.Show();
            mdiStatusMsg.Text = curForm.Name + " open";
        }

        private void navSetDatabase_Click( object sender, EventArgs e ) {
            DataAccess.DataAccessClose( true );
            SetDatabaseLocation curForm = new SetDatabaseLocation();
            curForm.getDatabaseFile( myAppRegKey );
        }

		private void navDatabaseBackup_Click( object sender, EventArgs e ) {
			SetDatabaseLocation curForm = new SetDatabaseLocation();
			curForm.backupDatabaseFile( myAppRegKey );

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

		private void navBoatPathReportSlalom_Click(object sender, EventArgs e) {
			BoatPathExport curForm = new BoatPathExport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Slalom";
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}
		private void navBoatPathSlalomImportReport_Click( object sender, EventArgs e ) {
			BoatPathImportReport curForm = new BoatPathImportReport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Slalom";
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}
		private void navBoatPathJumpImportReport_Click( object sender, EventArgs e ) {
			BoatPathImportReport curForm = new BoatPathImportReport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Jump";
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}

		private void navBoatTimeSlalomImportReport_Click( object sender, EventArgs e ) {
			BoatTimeImportReport curForm = new BoatTimeImportReport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Slalom";
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}
		private void navBoatTimeJumpImportReport_Click( object sender, EventArgs e ) {
			BoatTimeImportReport curForm = new BoatTimeImportReport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Jump";
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}

		private void navJumpMeasurementImportReport_Click( object sender, EventArgs e ) {
			JumpMeasurementImportReport curForm = new JumpMeasurementImportReport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";

		}

		private void navBoatPathReportJump_Click(object sender, EventArgs e) {
			BoatPathExport curForm = new BoatPathExport();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.ActiveEvent = "Jump";
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

		private void overviewToolStripMenuItem_Click( object sender, EventArgs e ) {
            MessageBox.Show( "See the Wstims For Windows Newsletter Archive \nhttp://www.waterskiresults.com/filelist.php?FolderName=Newsletters");
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

		private void regionalJuniorExtractsToolStripMenuItem_Click( object sender, EventArgs e ) {
			RegionalJuniorScoreAnalysis curForm = new RegionalJuniorScoreAnalysis();
			mdiStatusMsg.Text = curForm.Name + " opening";

			// Set the Parent Form and display requested form
			curForm.MdiParent = this;
			curForm.Show();
			mdiStatusMsg.Text = curForm.Name + " open";
		}

		private void databaseToolToolStripMenuItem_Click( object sender, EventArgs e ) {
            SqlCommandTool curForm = new SqlCommandTool();
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
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private void importMemberFileToolStripMenuItem1_Click( object sender, EventArgs e ) {

		}

	}
}

