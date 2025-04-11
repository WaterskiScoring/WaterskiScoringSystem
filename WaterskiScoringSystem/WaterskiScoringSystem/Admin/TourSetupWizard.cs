using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Admin {
    public partial class TourSetupWizard : Form {
        private String mySanctionNum = null;
        private String myTourRules;
        private String myTourClass;

        private int myTourSlalomRounds;
        private int myTourTrickRounds;
        private int myTourJumpRounds;

        private DataRow myTourRow;
        
        private TourProperties myTourProperties;

        public TourSetupWizard() {
            InitializeComponent();
        }

        private void TourSetupWizard_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TourSetupWizard_Location.X > 0
                && Properties.Settings.Default.TourSetupWizard_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourSetupWizard_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null || mySanctionNum.Length < 6 ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                return;
            }

            //Retrieve selected tournament attributes
            DataTable curTourDataTable = getTourData( mySanctionNum );
            if ( curTourDataTable == null || curTourDataTable.Rows.Count == 0 ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                return;
            }

            myTourRow = curTourDataTable.Rows[0];
            myTourRules = (String)myTourRow["Rules"];
            myTourClass = HelperFunctions.getDataRowColValue( myTourRow, "Class", "C" );
            myTourSlalomRounds = HelperFunctions.ConvertToInt(HelperFunctions.getDataRowColValue( myTourRow, "SlalomRounds", "0" ));
            myTourTrickRounds = HelperFunctions.ConvertToInt( HelperFunctions.getDataRowColValue( myTourRow, "TrickRounds", "0" ) );
            myTourJumpRounds = HelperFunctions.ConvertToInt( HelperFunctions.getDataRowColValue( myTourRow, "JumpRounds", "0" ) );

            myTourProperties = TourProperties.Instance;
            myTourProperties.loadProperties( myTourRules, myTourClass );

            if ( myTourSlalomRounds == 0 ) {
                SlalomPlcmtGroupBox.Visible = false;
                SlalomScoresGroupBox.Visible = false;
                SlalomPointsMethodGroupBox.Visible = false;
                SlalomPlcmtGroupsBox.Visible = false;
            
            } else {
                SlalomPlcmtBestButton.Checked = true;
                if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "best" ) ) SlalomPlcmtBestButton.Checked = true;
                if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "final" ) ) SlalomPlcmtFinalButton.Checked = true;
                if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "total" ) ) SlalomPlcmtTotalButton.Checked = true;
                if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "first" ) ) SlalomPlcmtFirstButton.Checked = true;
                //if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "h2h" ) ) SlalomPlcmtH2hButton.Checked = true;

                SlalomScoreRawButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtMethod.ToLower().Equals( "score" ) ) SlalomScoreRawButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtMethod.ToLower().Equals( "points" ) ) SlalomPointsButton.Checked = true;

                SlalomPointsNopsButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "nops" ) ) SlalomPointsNopsButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "plcmt" ) ) SlalomPointsPlcmtButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "kbase" ) ) SlalomPointsKbaseButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "hcap" ) ) SlalomPointsHcapButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "ratio" ) ) SlalomPointsHcapButton.Checked = true;
                if ( HelperFunctions.isIwwfEvent( myTourRules ) ) SlalomPointsKbaseButton.Checked = true;

                SlalomGroupDivButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "div" ) ) SlalomGroupDivButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "group" ) ) SlalomGroupDivButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "tour" ) ) SlalomGroupTourButton.Checked = true;
                if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "divgr" ) ) SlalomGroupDivGroupButton.Checked = true;

                SlalomNumPrelimTextBox.Text = myTourProperties.SlalomSummaryNumPrelim;
            }


            if ( myTourTrickRounds == 0 ) {
                TrickPlcmtGroupBox.Visible = false;
                TrickScoresGroupBox.Visible = false;
                TrickPointsMethodGroupBox.Visible = false;
                TrickPlcmtGroupsBox.Visible = false;

            } else {
                TrickPlcmtBestButton.Checked = true;
                if ( myTourProperties.TrickSummaryDataType.ToLower().Equals( "best" ) ) TrickPlcmtBestButton.Checked = true;
                if ( myTourProperties.TrickSummaryDataType.ToLower().Equals( "final" ) ) TrickPlcmtFinalButton.Checked = true;
                if ( myTourProperties.TrickSummaryDataType.ToLower().Equals( "total" ) ) TrickPlcmtTotalButton.Checked = true;
                if ( myTourProperties.TrickSummaryDataType.ToLower().Equals( "first" ) ) TrickPlcmtFirstButton.Checked = true;
                //if ( myTourProperties.TrickSummaryDataType.ToLower().Equals( "h2h" ) ) TrickPlcmtH2hButton.Checked = true;

                TrickScoreRawButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtMethod.ToLower().Equals( "score" ) ) TrickScoreRawButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtMethod.ToLower().Equals( "points" ) ) TrickPointsButton.Checked = true;

                TrickPointsNopsButton.Checked = true;
                if ( myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "nops" ) ) TrickPointsNopsButton.Checked = true;
                if ( myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "plcmt" ) ) TrickPointsPlcmtButton.Checked = true;
                if ( myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "kbase" ) ) TrickPointsKbaseButton.Checked = true;
                if ( myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "hcap" ) ) TrickPointsHcapButton.Checked = true;
                if ( myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "ratio" ) ) TrickPointsHcapButton.Checked = true;
                if ( HelperFunctions.isIwwfEvent( myTourRules ) ) TrickPointsKbaseButton.Checked = true;

                TrickGroupDivButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "div" ) ) TrickGroupDivButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "group" ) ) TrickGroupDivButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "tour" ) ) TrickGroupTourButton.Checked = true;
                if ( myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "divgr" ) ) TrickGroupDivGroupButton.Checked = true;

                TrickNumPrelimTextBox.Text = myTourProperties.TrickSummaryNumPrelim;
            }

            if ( myTourJumpRounds == 0 ) {
                JumpPlcmtGroupBox.Visible = false;
                JumpScoresGroupBox.Visible = false;
                JumpPointsMethodGroupBox.Visible = false;
                JumpPlcmtGroupsBox.Visible = false;

            } else {
                JumpPlcmtBestButton.Checked = true;
                if ( myTourProperties.JumpSummaryDataType.ToLower().Equals( "best" ) ) JumpPlcmtBestButton.Checked = true;
                if ( myTourProperties.JumpSummaryDataType.ToLower().Equals( "final" ) ) JumpPlcmtFinalButton.Checked = true;
                if ( myTourProperties.JumpSummaryDataType.ToLower().Equals( "total" ) ) JumpPlcmtTotalButton.Checked = true;
                if ( myTourProperties.JumpSummaryDataType.ToLower().Equals( "first" ) ) JumpPlcmtFirstButton.Checked = true;
                //if ( myTourProperties.JumpSummaryDataType.ToLower().Equals( "h2h" ) ) JumpPlcmtH2hButton.Checked = true;

                JumpScoreRawButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtMethod.ToLower().Equals( "score" ) ) JumpScoreRawButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtMethod.ToLower().Equals( "points" ) ) JumpPointsButton.Checked = true;

                JumpPointsNopsButton.Checked = true;
                if ( myTourProperties.JumpSummaryPointsMethod.ToLower().Equals( "nops" ) ) JumpPointsNopsButton.Checked = true;
                if ( myTourProperties.JumpSummaryPointsMethod.ToLower().Equals( "plcmt" ) ) JumpPointsPlcmtButton.Checked = true;
                if ( myTourProperties.JumpSummaryPointsMethod.ToLower().Equals( "kbase" ) ) JumpPointsKbaseButton.Checked = true;
                if ( myTourProperties.JumpSummaryPointsMethod.ToLower().Equals( "hcap" ) ) JumpPointsHcapButton.Checked = true;
                if ( myTourProperties.JumpSummaryPointsMethod.ToLower().Equals( "ratio" ) ) JumpPointsHcapButton.Checked = true;
                if ( HelperFunctions.isIwwfEvent( myTourRules ) ) JumpPointsKbaseButton.Checked = true;

                JumpGroupDivButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtOrg.ToLower().Equals( "div" ) ) JumpGroupDivButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtOrg.ToLower().Equals( "group" ) ) JumpGroupDivButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtOrg.ToLower().Equals( "tour" ) ) JumpGroupTourButton.Checked = true;
                if ( myTourProperties.JumpSummaryPlcmtOrg.ToLower().Equals( "divgr" ) ) JumpGroupDivGroupButton.Checked = true;

                JumpNumPrelimTextBox.Text = myTourProperties.JumpSummaryNumPrelim;
            }


            if ( myTourSlalomRounds == 0 || myTourTrickRounds == 0 || myTourJumpRounds == 0 ) {
                OverallPlcmtGroupBox.Visible = false;
                OverallScoresGroupBox.Visible = false;
                OverallPointsMethodGroupBox.Visible = false;
                OverallPlcmtGroupsBox.Visible = false;

            } else {
                OverallPlcmtBestButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "best" ) ) OverallPlcmtBestButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "final" ) ) OverallPlcmtFinalButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "total" ) ) OverallPlcmtTotalButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "first" ) ) OverallPlcmtFirstButton.Checked = true;

                OverallPointsButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtMethod.ToLower().Equals( "points" ) ) OverallPointsButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtMethod.ToLower().Equals( "hcap" ) ) OverallHandicapButton.Checked = true;

                OverallPointsNopsButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "nops" ) ) OverallPointsNopsButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "plcmt" ) ) OverallPointsPlcmtButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "kbase" ) ) OverallPointsKbaseButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "hcap" ) ) OverallPointsHcapButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "ratio" ) ) OverallPointsHcapButton.Checked = true;
                if ( HelperFunctions.isIwwfEvent( myTourRules ) ) OverallPointsKbaseButton.Checked = true;

                OverallGroupDivButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "div" ) ) OverallGroupDivButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "group" ) ) OverallGroupDivButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "tour" ) ) OverallGroupTourButton.Checked = true;
                if ( myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "divgr" ) ) OverallGroupDivGroupButton.Checked = true;

                if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                    SlalomPlcmtBestButton.Checked = true;
                    SlalomPointsButton.Checked = true;
                    SlalomPointsPlcmtButton.Checked = true;
                    SlalomGroupDivButton.Checked = true;

                    TrickPlcmtBestButton.Checked = true;
                    TrickPointsButton.Checked = true;
                    TrickPointsPlcmtButton.Checked = true;
                    TrickGroupDivButton.Checked = true;

                    JumpPlcmtBestButton.Checked = true;
                    JumpPointsButton.Checked = true;
                    JumpPointsPlcmtButton.Checked = true;
                    JumpGroupDivButton.Checked = true;

                    OverallPlcmtBestButton.Checked = true;
                    OverallPointsButton.Checked = true;
                }

            }

        }

        private void TourSetupWizard_FormClosing( object sender, FormClosingEventArgs e ) {
            myTourProperties.SlalomSummaryNumPrelim = SlalomNumPrelimTextBox.Text;
            if ( SlalomPlcmtBestButton.Checked ) myTourProperties.SlalomSummaryDataType = "best";
            if ( SlalomPlcmtFinalButton.Checked ) myTourProperties.SlalomSummaryDataType = "final";
            if ( SlalomPlcmtTotalButton.Checked ) myTourProperties.SlalomSummaryDataType = "total"; 
            if ( SlalomPlcmtFirstButton.Checked ) myTourProperties.SlalomSummaryDataType = "first";
            //if ( SlalomPlcmtH2hButton.Checked ) myTourProperties.SlalomSummaryDataType = "h2h";

            if ( SlalomScoreRawButton.Checked ) myTourProperties.SlalomSummaryPlcmtMethod = "score";
            if ( SlalomPointsButton.Checked ) myTourProperties.SlalomSummaryPlcmtMethod = "points";

            if ( SlalomPointsNopsButton.Checked ) myTourProperties.SlalomSummaryPointsMethod = "nops";
            if ( SlalomPointsPlcmtButton.Checked ) myTourProperties.SlalomSummaryPointsMethod = "plcmt";
            if ( SlalomPointsKbaseButton.Checked ) myTourProperties.SlalomSummaryPointsMethod = "kbase";
            if ( SlalomPointsHcapButton.Checked ) myTourProperties.SlalomSummaryPointsMethod = "hcap";
            if ( SlalomPointsHcapButton.Checked ) myTourProperties.SlalomSummaryPointsMethod = "ratio";

            if ( SlalomGroupDivButton.Checked ) myTourProperties.SlalomSummaryPlcmtOrg = "div";
            if ( SlalomGroupDivButton.Checked ) myTourProperties.SlalomSummaryPlcmtOrg = "group";
            if ( SlalomGroupTourButton.Checked ) myTourProperties.SlalomSummaryPlcmtOrg = "tour";
            if ( SlalomGroupDivGroupButton.Checked ) myTourProperties.SlalomSummaryPlcmtOrg = "divgr";

            myTourProperties.TrickSummaryNumPrelim = TrickNumPrelimTextBox.Text;
            if ( TrickPlcmtBestButton.Checked ) myTourProperties.TrickSummaryDataType = "best";
            if ( TrickPlcmtFinalButton.Checked ) myTourProperties.TrickSummaryDataType = "final";
            if ( TrickPlcmtTotalButton.Checked ) myTourProperties.TrickSummaryDataType = "total";
            if ( TrickPlcmtFirstButton.Checked ) myTourProperties.TrickSummaryDataType = "first";
            //if ( TrickPlcmtH2hButton.Checked ) myTourProperties.TrickSummaryDataType = "h2h";

            if ( TrickScoreRawButton.Checked ) myTourProperties.TrickSummaryPlcmtMethod = "score";
            if ( TrickPointsButton.Checked ) myTourProperties.TrickSummaryPlcmtMethod = "points";

            if ( TrickPointsNopsButton.Checked ) myTourProperties.TrickSummaryPointsMethod = "nops";
            if ( TrickPointsPlcmtButton.Checked ) myTourProperties.TrickSummaryPointsMethod = "plcmt";
            if ( TrickPointsKbaseButton.Checked ) myTourProperties.TrickSummaryPointsMethod = "kbase";
            if ( TrickPointsHcapButton.Checked ) myTourProperties.TrickSummaryPointsMethod = "hcap";
            if ( TrickPointsHcapButton.Checked ) myTourProperties.TrickSummaryPointsMethod = "ratio";

            if ( TrickGroupDivButton.Checked ) myTourProperties.TrickSummaryPlcmtOrg = "div";
            if ( TrickGroupDivButton.Checked ) myTourProperties.TrickSummaryPlcmtOrg = "group";
            if ( TrickGroupTourButton.Checked ) myTourProperties.TrickSummaryPlcmtOrg = "tour";
            if ( TrickGroupDivGroupButton.Checked ) myTourProperties.TrickSummaryPlcmtOrg = "divgr";

            myTourProperties.JumpSummaryNumPrelim = JumpNumPrelimTextBox.Text;
            if ( JumpPlcmtBestButton.Checked ) myTourProperties.JumpSummaryDataType = "best";
            if ( JumpPlcmtFinalButton.Checked ) myTourProperties.JumpSummaryDataType = "final";
            if ( JumpPlcmtTotalButton.Checked ) myTourProperties.JumpSummaryDataType = "total";
            if ( JumpPlcmtFirstButton.Checked ) myTourProperties.JumpSummaryDataType = "first";
            //if ( JumpPlcmtH2hButton.Checked ) myTourProperties.JumpSummaryDataType = "h2h";

            if ( JumpScoreRawButton.Checked ) myTourProperties.JumpSummaryPlcmtMethod = "score";
            if ( JumpPointsButton.Checked ) myTourProperties.JumpSummaryPlcmtMethod = "points";

            if ( JumpPointsNopsButton.Checked ) myTourProperties.JumpSummaryPointsMethod = "nops";
            if ( JumpPointsPlcmtButton.Checked ) myTourProperties.JumpSummaryPointsMethod = "plcmt";
            if ( JumpPointsKbaseButton.Checked ) myTourProperties.JumpSummaryPointsMethod = "kbase";
            if ( JumpPointsHcapButton.Checked ) myTourProperties.JumpSummaryPointsMethod = "hcap";
            if ( JumpPointsHcapButton.Checked ) myTourProperties.JumpSummaryPointsMethod = "ratio";

            if ( JumpGroupDivButton.Checked ) myTourProperties.JumpSummaryPlcmtOrg = "div";
            if ( JumpGroupDivButton.Checked ) myTourProperties.JumpSummaryPlcmtOrg = "group";
            if ( JumpGroupTourButton.Checked ) myTourProperties.JumpSummaryPlcmtOrg = "tour";
            if ( JumpGroupDivGroupButton.Checked ) myTourProperties.JumpSummaryPlcmtOrg = "divgr";

            if ( OverallPlcmtBestButton.Checked ) myTourProperties.MasterSummaryOverallDataType = "best";
            if ( OverallPlcmtFinalButton.Checked ) myTourProperties.MasterSummaryOverallDataType = "final";
            if ( OverallPlcmtTotalButton.Checked ) myTourProperties.MasterSummaryOverallDataType = "total";
            if ( OverallPlcmtFirstButton.Checked ) myTourProperties.MasterSummaryOverallDataType = "first";

            if ( OverallPointsButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtMethod = "points";
            if ( OverallHandicapButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtMethod = "hcap";

            if ( OverallPointsNopsButton.Checked ) myTourProperties.MasterSummaryOverallPointsMethod = "nops";
            if ( OverallPointsPlcmtButton.Checked ) myTourProperties.MasterSummaryOverallPointsMethod = "plcmt";
            if ( OverallPointsKbaseButton.Checked ) myTourProperties.MasterSummaryOverallPointsMethod = "kbase";
            if ( OverallPointsHcapButton.Checked ) myTourProperties.MasterSummaryOverallPointsMethod = "hcap";
            if ( OverallPointsHcapButton.Checked ) myTourProperties.MasterSummaryOverallPointsMethod = "ratio";

            if ( OverallGroupDivButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtOrg = "div";
            if ( OverallGroupDivButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtOrg = "group";
            if ( OverallGroupTourButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtOrg = "tour";
            if ( OverallGroupDivGroupButton.Checked ) myTourProperties.MasterSummaryOverallPlcmtOrg = "divgr";
            
            Properties.Settings.Default.Save();

            if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                LiveWebHandler.sendTourProperties( mySanctionNum );
            }

        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            return curDataTable;
        }

        private void DoneButton_Click( object sender, EventArgs e ) {
            this.Close();
        }
    }
}
