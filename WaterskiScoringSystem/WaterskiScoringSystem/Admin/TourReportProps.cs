using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Admin {
    public partial class TourReportProps : Form {
        #region Instance Variables
        private Boolean isDataModified = false;
        private Boolean isDataLoadActive = false;
        private String mySanctionNum;
        private String myTourClass;
        private String myTourRules;
        private String myTourName;
        private TourProperties myTourProperties;
        #endregion

        public TourReportProps() {
            InitializeComponent();
        }

        public void TourChiefOfficialContact_Show(String inSanctionNum, String inTourClass, String inTourRules, String inTourName) {
            //Retrieve tournament list and set current position to active tournament
            mySanctionNum = inSanctionNum;
            myTourClass = inTourClass;
            myTourRules = inTourRules;
            myTourName = inTourName;
            navRefresh();
        }

        private void TourReportProps_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.TourReportProps_Width > 0) {
                this.Width = Properties.Settings.Default.TourReportProps_Width;
            }
            if (Properties.Settings.Default.TourReportProps_Height > 0) {
                this.Height = Properties.Settings.Default.TourReportProps_Height;
            }
            if (Properties.Settings.Default.TourReportProps_Location.X > 0
                && Properties.Settings.Default.TourReportProps_Location.Y > 0) {
                this.Location = Properties.Settings.Default.TourReportProps_Location;
            }
        }

        private void TourReportProps_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                if (isDataModified) {
                    updateTourProperties();
                    if (isDataModified) {
                        String dialogMsg = "Update was not complete "
                            + "\n\n Do you want close the window without correcting errors?";
                        DialogResult msgResp =
                            MessageBox.Show( dialogMsg, "Change Warning",
                                MessageBoxButtons.YesNoCancel,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1 );
                        if (msgResp == DialogResult.Yes) {
                            e.Cancel = false;
                        } else if (msgResp == DialogResult.No) {
                            e.Cancel = true;
                        } else {
                            e.Cancel = true;
                        }
                    }
                }
            } catch (Exception excp) {
                String dialogMsg = "Error attempting to save changes "
                    + "\n" + excp.Message
                    + "\n\n Do you want close the window without correcting errors?";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if (msgResp == DialogResult.Yes) {
                    e.Cancel = true;
                } else if (msgResp == DialogResult.No) {
                    e.Cancel = false;
                } else {
                    e.Cancel = true;
                }
            }

        }

        private void navRefresh() {
            String curPropKey = "", curPropValue = "";
            DataRow[] curFindRows;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PropKey, PropValue " );
            curSqlStmt.Append( "FROM TourProperties " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Order by PropOrder, PropKey, PropValue " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

            isDataLoadActive = true;
            bestScoreButton.Checked = true;
            curPropKey = "MasterSummaryDataType";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
                if (curPropValue.ToLower().Equals( "first" )) firstScoreButton.Checked = true;
                if (curPropValue.ToLower().Equals( "final" )) finalScoreButton.Checked = true;
                if (curPropValue.ToLower().Equals( "total" )) totalScoreButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    bestScoreButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    bestScoreButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    bestScoreButton.Checked = true;
                } else {
                    bestScoreButton.Checked = true;
                }
            }

            nopsPointsButton.Checked = true;
            curPropKey = "MasterSummaryPointsMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
                if (curPropValue.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
                if (curPropValue.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
                if (curPropValue.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtPointsButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    kBasePointsButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    nopsPointsButton.Checked = true;
                } else {
                    nopsPointsButton.Checked = true;
                }
            }

            rawScoreButton.Checked = true;
            curPropKey = "MasterSummaryPlcmtMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "score" )) rawScoreButton.Checked = true;
                if (curPropValue.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    pointsScoreButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    rawScoreButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    rawScoreButton.Checked = true;
                } else {
                    rawScoreButton.Checked = true;
                }
            }

            plcmtDivButton.Checked = true;
            curPropKey = "MasterSummaryPlcmtOrg";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "div" )) plcmtDivButton.Checked = true;
                if (curPropValue.ToLower().Equals( "divgr" )) plcmtDivGrpButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtDivButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    plcmtDivButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    plcmtDivButton.Checked = true;
                } else {
                    plcmtDivButton.Checked = true;
                }
            }
            
            bestScoreOverallButton.Checked = true;
            curPropKey = "MasterSummaryOverallDataType";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "best" )) bestScoreOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "round" )) roundScoreOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "final" )) finalScoreOverallButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    bestScoreOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    bestScoreOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    roundScoreOverallButton.Checked = true;
                } else {
                    roundScoreOverallButton.Checked = true;
                }
            }

            nopsPointsOverallButton.Checked = true;
            curPropKey = "MasterSummaryOverallPointsMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "nops" )) nopsPointsOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "kbase" )) kBasePointsOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "plcmt" )) plcmtPointsOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "ratio" )) ratioPointsOverallButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtPointsOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    kBasePointsOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    nopsPointsOverallButton.Checked = true;
                } else {
                    nopsPointsOverallButton.Checked = true;
                }
            }

            pointsScoreOverallButton.Checked = true;
            curPropKey = "MasterSummaryOverallPlcmtMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "points" )) pointsScoreOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "ratio" )) ratioScoreOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "hcap" )) handicapScoreOverallButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    pointsScoreOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    pointsScoreOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    pointsScoreOverallButton.Checked = true;
                } else {
                    pointsScoreOverallButton.Checked = true;
                }
            }

            plcmtDivOverallButton.Checked = true;
            curPropKey = "MasterSummaryOverallPlcmtOrg";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "div" )) plcmtDivOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "divgr" )) plcmtDivGrpOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "group" )) plcmtGroupOverallButton.Checked = true;
                if (curPropValue.ToLower().Equals( "tour" )) plcmtTourOverallButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtDivOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    plcmtDivOverallButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    plcmtDivOverallButton.Checked = true;
                } else {
                    plcmtDivOverallButton.Checked = true;
                }
            }

            bestScoreTeamButton.Checked = true;
            curPropKey = "TeamSummaryDataType";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "best" )) bestScoreTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "total" )) totalScoreTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "final" )) finalScoreTeamButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    bestScoreTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    bestScoreTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    bestScoreTeamButton.Checked = true;
                } else {
                    bestScoreTeamButton.Checked = true;
                }
            }

            nopsPointsTeamButton.Checked = true;
            curPropKey = "TeamSummaryPointsMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "nops" )) nopsPointsTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "kbase" )) kBasePointsTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "plcmt" )) plcmtPointsTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "ratio" )) ratioPointsTeamButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtPointsTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    kBasePointsTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    nopsPointsTeamButton.Checked = true;
                } else {
                    nopsPointsTeamButton.Checked = true;
                }
            }

            pointsScoreTeamButton.Checked = true;
            curPropKey = "TeamSummaryPlcmtMethod";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "points" )) pointsScoreTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "ratio" )) ratioScoreTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "hcap" )) handicapScoreTeamButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    pointsScoreTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    pointsScoreTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    pointsScoreTeamButton.Checked = true;
                } else {
                    pointsScoreTeamButton.Checked = true;
                }
            }

            plcmtDivTeamButton.Checked = true;
            curPropKey = "TeamSummaryPlcmtOrg";
            curFindRows = curDataTable.Select( "PropKey = '" + curPropKey + "'" );
            if (curFindRows.Length > 0) {
                curPropValue = (String)curFindRows[0]["PropValue"];
                if (curPropValue.ToLower().Equals( "div" )) plcmtDivTeamButton.Checked = true;
                if (curPropValue.ToLower().Equals( "tour" )) plcmtTourTeamButton.Checked = true;
            } else {
                isDataModified = true;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    plcmtDivTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "iwwf" )) {
                    plcmtDivTeamButton.Checked = true;
                } else if (myTourRules.ToLower().Equals( "awsa" )) {
                    plcmtDivTeamButton.Checked = true;
                } else {
                    plcmtDivTeamButton.Checked = true;
                }
            }

            isDataLoadActive = false;
        }

        private void updateTourProperties() {
            String curPropValue = "", curPropKey = "";

            try {
                myTourProperties = TourProperties.Instance;

                //myTourProperties.MasterSummaryDataType
                curPropKey = "MasterSummaryDataType";
                curPropValue = "best";
                if (bestScoreButton.Checked) curPropValue = "best";
                if (firstScoreButton.Checked) curPropValue = "first";
                if (finalScoreButton.Checked) curPropValue = "final";
                if (totalScoreButton.Checked) curPropValue = "total";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 100 );
                updateTournamentProperty( mySanctionNum, "SlalomSummaryDataType", curPropValue, 300 );
                updateTournamentProperty( mySanctionNum, "TrickSummaryDataType", curPropValue, 400 );
                updateTournamentProperty( mySanctionNum, "JumpSummaryDataType", curPropValue, 500 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryDataType = curPropValue;
                    myTourProperties.SlalomSummaryDataType = curPropValue;
                    myTourProperties.TrickSummaryDataType = curPropValue;
                    myTourProperties.JumpSummaryDataType = curPropValue;
                }

                //myTourProperties.MasterSummaryPointsMethod
                curPropKey = "MasterSummaryPointsMethod";
                curPropValue = "nops";
                if (nopsPointsButton.Checked) curPropValue = "nops";
                if (kBasePointsButton.Checked) curPropValue = "kbase";
                if (plcmtPointsButton.Checked) curPropValue = "plcmt";
                if (ratioPointsButton.Checked) curPropValue = "ratio";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 100 );
                updateTournamentProperty( mySanctionNum, "SlalomSummaryPointsMethod", curPropValue, 300 );
                updateTournamentProperty( mySanctionNum, "TrickSummaryPointsMethod", curPropValue, 400 );
                updateTournamentProperty( mySanctionNum, "JumpSummaryPointsMethod", curPropValue, 500 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryPointsMethod = curPropValue;
                    myTourProperties.SlalomSummaryPointsMethod = curPropValue;
                    myTourProperties.TrickSummaryPointsMethod = curPropValue;
                    myTourProperties.JumpSummaryPointsMethod = curPropValue;
                }

                //myTourProperties.MasterSummaryPlcmtMethod
                curPropKey = "MasterSummaryPlcmtMethod";
                curPropValue = "score";
                if (rawScoreButton.Checked) curPropValue = "score";
                if (pointsScoreButton.Checked) curPropValue = "points";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 100 );
                updateTournamentProperty( mySanctionNum, "SlalomSummaryPlcmtMethod", curPropValue, 300 );
                updateTournamentProperty( mySanctionNum, "TrickSummaryPlcmtMethod", curPropValue, 400 );
                updateTournamentProperty( mySanctionNum, "JumpSummaryPlcmtMethod", curPropValue, 500 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryPlcmtMethod = curPropValue;
                    myTourProperties.SlalomSummaryPlcmtMethod = curPropValue;
                    myTourProperties.TrickSummaryPlcmtMethod = curPropValue;
                    myTourProperties.JumpSummaryPlcmtMethod = curPropValue;
                }

                //myTourProperties.MasterSummaryPlcmtOrg
                curPropKey = "MasterSummaryPlcmtOrg";
                curPropValue = "div";
                if (plcmtDivButton.Checked) curPropValue = "div";
                if (plcmtDivGrpButton.Checked) curPropValue = "divgr";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 100 );
                updateTournamentProperty( mySanctionNum, "SlalomSummaryPlcmtOrg", curPropValue, 300 );
                updateTournamentProperty( mySanctionNum, "TrickSummaryPlcmtOrg", curPropValue, 400 );
                updateTournamentProperty( mySanctionNum, "JumpSummaryPlcmtOrg", curPropValue, 500 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryPlcmtOrg = curPropValue;
                    myTourProperties.SlalomSummaryPlcmtOrg = curPropValue;
                    myTourProperties.TrickSummaryPlcmtOrg = curPropValue;
                    myTourProperties.JumpSummaryPlcmtOrg = curPropValue;
                }

                //myTourProperties.MasterSummaryOverallDataType
                curPropKey = "MasterSummaryOverallDataType";
                curPropValue = "best";
                if (bestScoreOverallButton.Checked) curPropValue = "best";
                if (roundScoreOverallButton.Checked) curPropValue = "round";
                if (finalScoreOverallButton.Checked) curPropValue = "final";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 110 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryOverallDataType = curPropValue;
                }

                //myTourProperties.MasterSummaryOverallPointsMethod
                curPropKey = "MasterSummaryOverallPointsMethod";
                curPropValue = "nops";
                if (nopsPointsOverallButton.Checked) curPropValue = "nops";
                if (kBasePointsOverallButton.Checked) curPropValue = "kbase";
                if (plcmtPointsOverallButton.Checked) curPropValue = "plcmt";
                if (ratioPointsOverallButton.Checked) curPropValue = "ratio";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 110 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryOverallPointsMethod = curPropValue;
                }

                //myTourProperties.MasterSummaryOverallPlcmtMethod
                curPropKey = "MasterSummaryOverallPlcmtMethod";
                curPropValue = "points";
                if (pointsScoreOverallButton.Checked) curPropValue = "points";
                if (ratioScoreOverallButton.Checked) curPropValue = "ratio";
                if (handicapScoreOverallButton.Checked) curPropValue = "hcap";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 110 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryOverallPlcmtMethod = curPropValue;
                }

                //myTourProperties.MasterSummaryOverallPlcmtOrg
                curPropKey = "MasterSummaryOverallPlcmtOrg";
                curPropValue = "div";
                if (plcmtDivOverallButton.Checked) curPropValue = "div";
                if (plcmtDivGrpOverallButton.Checked) curPropValue = "divgr";
                if (plcmtGroupOverallButton.Checked) curPropValue = "group";
                if (plcmtTourOverallButton.Checked) curPropValue = "tour";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 110 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.MasterSummaryOverallPlcmtOrg = curPropValue;
                }

                //myTourProperties.TeamSummaryDataType
                curPropKey = "TeamSummaryDataType";
                curPropValue = "best";
                if (bestScoreTeamButton.Checked) curPropValue = "best";
                if (finalScoreTeamButton.Checked) curPropValue = "final";
                if (totalScoreTeamButton.Checked) curPropValue = "total";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 900 );
                updateTournamentProperty( mySanctionNum, "SlalomTeamSummaryDataType", curPropValue, 910 );
                updateTournamentProperty( mySanctionNum, "TrickTeamSummaryDataType", curPropValue, 920 );
                updateTournamentProperty( mySanctionNum, "JumpTeamSummaryDataType", curPropValue, 930 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.TeamSummaryDataType = curPropValue;
                    myTourProperties.SlalomTeamSummaryDataType = curPropValue;
                    myTourProperties.TrickTeamSummaryDataType = curPropValue;
                    myTourProperties.JumpTeamSummaryDataType = curPropValue;
                }

                //myTourProperties.TeamSummaryPointsMethod
                curPropKey = "TeamSummaryPointsMethod";
                curPropValue = "nops";
                if (nopsPointsTeamButton.Checked) curPropValue = "nops";
                if (kBasePointsTeamButton.Checked) curPropValue = "kbase";
                if (plcmtPointsTeamButton.Checked) curPropValue = "plcmt";
                if (ratioPointsTeamButton.Checked) curPropValue = "ratio";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 900 );
                updateTournamentProperty( mySanctionNum, "SlalomTeamSummaryPointsMethod", curPropValue, 910 );
                updateTournamentProperty( mySanctionNum, "TrickTeamSummaryPointsMethod", curPropValue, 920 );
                updateTournamentProperty( mySanctionNum, "JumpTeamSummaryPointsMethod", curPropValue, 930 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.TeamSummaryPointsMethod = curPropValue;
                    myTourProperties.SlalomTeamSummaryPointsMethod = curPropValue;
                    myTourProperties.TrickTeamSummaryPointsMethod = curPropValue;
                    myTourProperties.JumpTeamSummaryPointsMethod = curPropValue;
                }

                //myTourProperties.TeamSummaryPlcmtMethod
                curPropKey = "TeamSummaryPlcmtMethod";
                curPropValue = "points";
                if (pointsScoreTeamButton.Checked) curPropValue = "points";
                if (handicapScoreTeamButton.Checked) curPropValue = "hcap";
                if (ratioScoreTeamButton.Checked) curPropValue = "ratio";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 900 );
                updateTournamentProperty( mySanctionNum, "SlalomTeamSummaryPlcmtMethod", curPropValue, 910 );
                updateTournamentProperty( mySanctionNum, "TrickTeamSummaryPlcmtMethod", curPropValue, 920 );
                updateTournamentProperty( mySanctionNum, "JumpTeamSummaryPlcmtOrg", curPropValue, 930 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.TeamSummaryPlcmtMethod = curPropValue;
                    myTourProperties.SlalomTeamSummaryPlcmtMethod = curPropValue;
                    myTourProperties.TrickTeamSummaryPlcmtMethod = curPropValue;
                    myTourProperties.JumpTeamSummaryPlcmtOrg = curPropValue;
                }

                //myTourProperties.TeamSummaryPlcmtOrg
                curPropKey = "TeamSummaryPlcmtOrg";
                curPropValue = "div";
                if (plcmtDivTeamButton.Checked) curPropValue = "div";
                if (plcmtTourTeamButton.Checked) curPropValue = "Tour";
                updateTournamentProperty( mySanctionNum, curPropKey, curPropValue, 900 );
                updateTournamentProperty( mySanctionNum, "SlalomTeamSummaryPlcmtOrg", curPropValue, 910 );
                updateTournamentProperty( mySanctionNum, "TrickTeamSummaryPlcmtOrg", curPropValue, 920 );
                updateTournamentProperty( mySanctionNum, "JumpTeamSummaryPlcmtOrg", curPropValue, 930 );
                if (Properties.Settings.Default.AppSanctionNum.Equals( mySanctionNum )) {
                    myTourProperties.TeamSummaryPlcmtOrg = curPropValue;
                    myTourProperties.SlalomTeamSummaryPlcmtOrg = curPropValue;
                    myTourProperties.TrickTeamSummaryPlcmtOrg = curPropValue;
                    myTourProperties.JumpTeamSummaryPlcmtOrg = curPropValue;
                }

                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    if (myTourName.ToLower().Contains( "all star" )) {
                        myTourProperties.TeamSummary_NumPerTeam = "10";
                        myTourProperties.SlalomTeamSummary_NumPerTeam = "10";
                        myTourProperties.TrickTeamSummary_NumPerTeam = "10";
                        myTourProperties.JumpTeamSummary_NumPerTeam = "10";
                    } else {
                        myTourProperties.TeamSummary_NumPerTeam = "4";
                        myTourProperties.SlalomTeamSummary_NumPerTeam = "4";
                        myTourProperties.TrickTeamSummary_NumPerTeam = "4";
                        myTourProperties.JumpTeamSummary_NumPerTeam = "4";
                    }
                }

                isDataModified = false;

            } catch (Exception excp) {
                String curMsg = ":Error attempting to update tournament property " + curPropKey + "\n" + excp.Message;
                MessageBox.Show( curMsg );
            }
        }

        private bool updateTournamentProperty(String inViewSanctionId, String inKey, String inValue, Int16 inOrder) {
            bool curReturnValue = true;

            try {
                if (inValue.Length > 0) {
                    StringBuilder curSqlStmt = new StringBuilder( "Update TourProperties " );
                    curSqlStmt.Append( "Set PropValue = '" + inValue + "' " );
                    curSqlStmt.Append( "Where SanctionId = '" + inViewSanctionId + "' " );
                    curSqlStmt.Append( "  And PropKey = '" + inKey + "' " );
                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    if (rowsProc == 0) {
                        curSqlStmt = new StringBuilder( "Insert  TourProperties (" );
                        curSqlStmt.Append( "Sanctionid, PropKey, PropValue, PropOrder" );
                        curSqlStmt.Append( ") Values ( " );
                        curSqlStmt.Append( "'" + inViewSanctionId + "', '" + inKey + "', '" + inValue + "', " + inOrder + " )" );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        if (rowsProc == 0) curReturnValue = false;
                    }
                }
            } catch (Exception excp) {
                curReturnValue = false;
                String curMsg = ":Error attempting to update tournament property " + inKey + "\n" + excp.Message;
                MessageBox.Show( curMsg );
            }
            return curReturnValue;
        }

        private void Button_CheckedChanged(object sender, EventArgs e) {
            RadioButton curButton = (RadioButton)sender;
            if (isDataLoadActive == false) {
                isDataModified = true;
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
