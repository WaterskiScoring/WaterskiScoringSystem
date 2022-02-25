using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class MasterSummaryTeam : Form {
        private String myPageBreak = "\\n";
        private bool myShowTeam = false;
        private Int16 myNumPerTeam;
        private String mySanctionNum = null;
        private String myTourRules;
        private DataRow myTourRow;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;
        private DataTable myTeamDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private TourProperties myTourProperties;

        public MasterSummaryTeam() {
            InitializeComponent();
        }

        private void MasterSummaryTeam_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TeamSummary_Width > 0 ) {
                this.Width = Properties.Settings.Default.TeamSummary_Width;
            }
            if ( Properties.Settings.Default.TeamSummary_Height > 0 ) {
                this.Height = Properties.Settings.Default.TeamSummary_Height;
            }
            if ( Properties.Settings.Default.TeamSummary_Location.X > 0
                && Properties.Settings.Default.TeamSummary_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TeamSummary_Location;
            }
            myTourProperties = TourProperties.Instance;

			FilterReportButton.Visible = false;
			FilterReportButton.Enabled = false;

			bestScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "total" )) totalScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "final" )) finalScoreButton.Checked = true;

            pointsScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtMethod.ToLower().Equals( "hcap" )) handicapScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtMethod.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;

            plcmtTourButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "tour" )) plcmtTourButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "div" )) divPlcmtButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtOrg.ToLower().Equals( "group" )) groupPlcmtButton.Checked = true;
            if ( myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "awsa" ) ) plcmtAWSATeamButton.Checked = true;

            plcmtPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;

            CategoryGroupBox.Location = new Point( 825, 49 );
			EventGroupBox.Location = new Point( 825, 85 );
			if ( plcmtAWSATeamButton.Checked ) {
				CategoryGroupBox.Visible = true;
				CategoryGroupBox.Enabled = true;
				EventGroupBox.Visible = true;
				EventGroupBox.Enabled = true;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

				EventGroupList.Visible = false;
				EventGroupList.Enabled = false;
				EventGroupListLabel.Visible = false;
				EventGroupListLabel.Enabled = false;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

			} else {
				CategoryGroupBox.Visible = false;
				CategoryGroupBox.Enabled = false;
				EventGroupBox.Visible = false;
				EventGroupBox.Enabled = false;

				EventGroupList.Visible = true;
				EventGroupList.Enabled = true;
				EventGroupListLabel.Visible = true;
				EventGroupListLabel.Enabled = true;
			}


			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    myNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam );
                    NumPerTeamTextBox.Text = myNumPerTeam.ToString();

                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = ( String ) myTourRow["Rules"];

                        loadGroupList();
                        if ( !( myTourRules.ToLower().Equals( "awsa" ) ) ) {
                            plcmtAWSATeamButton.Visible = false;
                        }
                    }
                }
            }
        }

        private void TeamSummary_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TeamSummary_Width = this.Size.Width;
                Properties.Settings.Default.TeamSummary_Height = this.Size.Height;
                Properties.Settings.Default.TeamSummary_Location = this.Location;
            }
        }

        private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e) {
            MessageBox.Show("Binding Exception: " + e.Exception.Message);
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show("Error happened " + e.Context.ToString());
            if (e.Context == DataGridViewDataErrorContexts.Commit) {
                MessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange) {
                MessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing) {
                MessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl) {
                MessageBox.Show("leave control error");
            }
            if ((e.Exception) is ConstraintException) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        public void navRefresh_Click( object sender, EventArgs e ) {
            this.Cursor = Cursors.WaitCursor;

            // Retrieve data from database
            if ( mySanctionNum != null && myTourRow != null ) {
                String curDataType = "all", curPlcmtMethod = "score";
                String curPlcmtOrg, curPointsMethod;
                CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                // Retrieve data from database depending on selection criteria
                String curMsg = "Tournament scores retrieved ";

                if ( plcmtAWSATeamButton.Checked ) {
                    bestScoreButton.Checked = true;
                    pointsScoreButton.Checked = true;
                    nopsPointsButton.Checked = true;
                }

                if ( bestScoreButton.Checked ) {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                } else if ( totalScoreButton.Checked ) {
                    curDataType = "total";
                    winStatusMsg.Text = curMsg + "- total scores";
                } else if ( finalScoreButton.Checked ) {
                    curDataType = "final";
                    winStatusMsg.Text = curMsg + "- final scores";
                }
                myTourProperties.TeamSummaryDataType = curDataType;

                if ( pointsScoreButton.Checked ) {
                    curPlcmtMethod = "points";
                } else if ( handicapScoreButton.Checked ) {
                    curPlcmtMethod = "hcap";
                } else if ( ratioScoreButton.Checked ) {
                    curPlcmtMethod = "ratio";
                } else {
                    curPlcmtMethod = "points";
                }
                myTourProperties.TeamSummaryPlcmtMethod = curPlcmtMethod;

                if ( divPlcmtButton.Checked ) {
                    curPlcmtOrg = "div";
                } else if (groupPlcmtButton.Checked) {
                    curPlcmtOrg = "group";
                } else if (plcmtTourButton.Checked) {
                    curPlcmtOrg = "tour";
                } else if ( plcmtAWSATeamButton.Checked ) {
                    curPlcmtOrg = "awsa";
                } else {
                    curPlcmtOrg = "tour";
                }
                myTourProperties.TeamSummaryPlcmtOrg = curPlcmtOrg;

                if ( nopsPointsButton.Checked ) {
                    curPointsMethod = "nops";
                } else if ( plcmtPointsButton.Checked ) {
                    curPointsMethod = "plcmt";
                } else if ( kBasePointsButton.Checked ) {
                    curPointsMethod = "kbase";
                } else if ( ratioPointsButton.Checked ) {
                    curPointsMethod = "ratio";
                } else {
                    curPointsMethod = "nops";
                }
                myTourProperties.TeamSummaryPointsMethod = curPointsMethod;

                String curGroupValue = "";
                try {
                    curGroupValue = EventGroupList.SelectedItem.ToString();
                    if (!( curGroupValue.ToLower().Equals( "all" ) )) {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (plcmtTourButton.Checked) {
                                curGroupValue = "All";
                            } else if (curGroupValue.ToUpper().Equals( "MEN A" )) {
                                curGroupValue = "CM";
                            } else if (curGroupValue.ToUpper().Equals( "WOMEN A" )) {
                                curGroupValue = "CW";
                            } else if (curGroupValue.ToUpper().Equals( "MEN B" )) {
                                curGroupValue = "BM";
                            } else if (curGroupValue.ToUpper().Equals( "WOMEN B" )) {
                                curGroupValue = "BW";
                            } else {
                                curGroupValue = "All";
                            }
                        }
                    }
                } catch {
                    curGroupValue = "All";
                }

                if (myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" )) {
                    mySlalomDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadSlalomDataGrid(mySlalomDataTable);
                    myTrickDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Trick", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadTrickDataGrid(myTrickDataTable);
                    myJumpDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Jump", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadJumpDataGrid(myJumpDataTable);

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam(mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam(myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam(myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    if ( plcmtTourButton.Checked ) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary(myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam);
                    }
                } else if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    if (plcmtTourButton.Checked) {
                        curPlcmtOrg = "div";
                    }
                    mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadSlalomDataGrid( mySlalomDataTable );
                    myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadTrickDataGrid( myTrickDataTable );
                    myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadJumpDataGrid( myJumpDataTable );

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamEventCombinedNcwsaSummary( myTeamDataTable, mySanctionNum );
                    }
                
                } else {
                    mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam );
                    }
                    if ( plcmtAWSATeamButton.Checked ) {
                        mySlalomDataTable = curCalcSummary.CalcTeamAwsaEventPoints( mySlalomDataTable, "Slalom" );
                        myTrickDataTable = curCalcSummary.CalcTeamAwsaEventPoints( myTrickDataTable, "Trick" );
                        myJumpDataTable = curCalcSummary.CalcTeamAwsaEventPoints( myJumpDataTable, "Jump" );
                        myTeamDataTable = curCalcSummary.CalcTeamAwsaCombinedSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam );
                    }
                    loadSlalomDataGrid( mySlalomDataTable );
                    loadTrickDataGrid( myTrickDataTable );
                    loadJumpDataGrid( myJumpDataTable );
                }

                String curSortCmd = "";
                if (TeamDiv.Visible) {
                    curSortCmd = "DivOrder ASC, Div ASC, TeamScoreTotal DESC";
                } else {
                    curSortCmd = "TeamScoreTotal DESC";
                }
                myTeamDataTable.DefaultView.Sort = curSortCmd;
                myTeamDataTable = myTeamDataTable.DefaultView.ToTable();
                loadTeamDataGrid( myTeamDataTable );
            }
            this.Cursor = Cursors.Default;
        }

		private void loadGroupList() {
            if (EventGroupList.DataSource == null) {
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    ArrayList curEventGroupList = new ArrayList();
                    curEventGroupList.Add( "All" );
                    curEventGroupList.Add( "Men A" );
                    curEventGroupList.Add( "Women A" );
                    curEventGroupList.Add( "Men B" );
                    curEventGroupList.Add( "Women B" );
                    curEventGroupList.Add( "Non Team" );
                    EventGroupList.DataSource = curEventGroupList;
                } else {
                    loadAgeGroupListFromData();
                }
            } else {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadAgeGroupListFromData();
                    }
                } else {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        ArrayList curEventGroupList = new ArrayList();
                        curEventGroupList.Add( "All" );
                        curEventGroupList.Add( "Men A" );
                        curEventGroupList.Add( "Women A" );
                        curEventGroupList.Add( "Men B" );
                        curEventGroupList.Add( "Women B" );
                        curEventGroupList.Add( "Non Team" );
                        EventGroupList.DataSource = curEventGroupList;
                    } else {
                        loadAgeGroupListFromData();
                    }
                }
            }
        }

        private void loadAgeGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT AgeGroup FROM EventReg "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' "
                + "Order by AgeGroup";
            DataTable curDataTable = getData( curSqlStmt );

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["AgeGroup"] );
            }
            EventGroupList.DataSource = curEventGroupList;
            if (curGroupValue.Length > 0) {
                foreach (String curValue in (ArrayList)EventGroupList.DataSource) {
                    if (curValue.Equals( curGroupValue )) {
                        EventGroupList.SelectedItem = curGroupValue;
                        EventGroupList.Text = curGroupValue;
                        return;
                    }
                }
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            } else {
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            }
        }

        private void loadTeamDataGrid(DataTable inDataTable) {
            String curGroup = "", prevGroup = "", curTeamPlcmtShow = "";
            int curTeamPlcmt = 0;
            Decimal curScore = 0, prevScore = -1;
            DataGridViewRow curViewRow;

            TeamSummaryDataGridView.Rows.Clear();
            int curIdx = 0;
            foreach ( DataRow curRow in inDataTable.Rows ) {
                curIdx = TeamSummaryDataGridView.Rows.Add();
                curViewRow = TeamSummaryDataGridView.Rows[curIdx];
                curScore = (Decimal)curRow["TeamScoreTotal"];

                if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    try {
                        curGroup = (String)curRow["Div"];
                    } catch {
                        curGroup = "";
                    }
                }
                if (curGroup != prevGroup && curIdx > 0) {
                    curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                    TeamSummaryDataGridView.Rows[curIdx].Height = 4;
                    curViewRow.Cells["TeamPlcmtOverall"].Value = myPageBreak;

                    curTeamPlcmt = 0;
                    prevScore = -1;
                    curTeamPlcmtShow = "";
                    curIdx = TeamSummaryDataGridView.Rows.Add();
                    curViewRow = TeamSummaryDataGridView.Rows[curIdx];
                }

                curTeamPlcmt++;
                if (curScore == prevScore && curIdx > 0) {
                    curTeamPlcmtShow = (String)TeamSummaryDataGridView.Rows[curIdx - 1].Cells["TeamPlcmtOverall"].Value;
                    if (curTeamPlcmtShow.Contains( "T" )) {
                    } else {
                        if (curTeamPlcmtShow.Length < 3) {
                        } else {
                            curTeamPlcmtShow = curTeamPlcmtShow.Substring( 0, 3 ) + "T";
                            TeamSummaryDataGridView.Rows[curIdx - 1].Cells["TeamPlcmtOverall"].Value = curTeamPlcmtShow;
                        }
                    }
                } else {
                    curTeamPlcmtShow = curTeamPlcmt.ToString( "##0" ).PadLeft( 3, ' ' );
                    curTeamPlcmtShow += " ";
                }

                try {
                    curViewRow.Cells["TeamPlcmtOverall"].Value = curTeamPlcmtShow;
                } catch {
                    curViewRow.Cells["TeamPlcmtOverall"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamDiv"].Value = (String)curRow["Div"];
                } catch {
                    curViewRow.Cells["TeamDiv"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamCode"].Value = (String)curRow["TeamCode"];
                } catch {
                    curViewRow.Cells["TeamCode"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamName"].Value = (String)curRow["Name"];
                } catch {
                    curViewRow.Cells["TeamName"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamScoreSlalom"].Value = (Decimal)curRow["TeamScoreSlalom"];
                } catch {
                    curViewRow.Cells["TeamScoreSlalom"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamPlcmtSlalom"].Value = (String)curRow["PlcmtSlalom"];
                } catch {
                    curViewRow.Cells["TeamPlcmtSlalom"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamScoreTrick"].Value = (Decimal)curRow["TeamScoreTrick"];
                } catch {
                    curViewRow.Cells["TeamScoreTrick"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamPlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                } catch {
                    curViewRow.Cells["TeamPlcmtTrick"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamScoreJump"].Value = (Decimal)curRow["TeamScoreJump"];
                } catch {
                    curViewRow.Cells["TeamScoreJump"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamPlcmtJump"].Value = (String)curRow["PlcmtJump"];
                } catch {
                    curViewRow.Cells["TeamPlcmtJump"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamScoreTotal"].Value = (Decimal)curRow["TeamScoreTotal"];
                } catch {
                    curViewRow.Cells["TeamScoreTotal"].Value = "";
                }
                prevGroup = curGroup;
                prevScore = curScore;
            }
            loadPrintDataGrid();
        }

        private void loadSlalomDataGrid( DataTable inSlalomDataTable ) {
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            String curGroup = "", prevGroup = "";
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderSlalom ASC, AgeGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamSlalom ASC, SkierName ASC";
            }
            inSlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inSlalomDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            slalomScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            //foreach ( DataRow curRow in curDataTable.Rows ) {
            foreach (DataRow curRow in curDataTable.Rows) {
                curShowRow = true;
                if (myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked) {
                    if (( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
                        || ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
                        ) {
                        curShowRow = true;
                    } else {
                        curShowRow = false;
                    }
                }
                if (curShowRow) {
                    curIdx = slalomScoreSummaryDataGridView.Rows.Add();
                    curViewRow = slalomScoreSummaryDataGridView.Rows[curIdx];

                    curGroup = (String)curRow["TeamSlalom"];
                    if (TeamDiv.Visible) {
                        if (divPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["AgeGroup"];
                        }
                        if (groupPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["EventGroup"];
                        }
                    }
                    if (curGroup != prevGroup && curIdx > 0) {
                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                        slalomScoreSummaryDataGridView.Rows[curIdx].Height = 4;

                        curIdx = slalomScoreSummaryDataGridView.Rows.Add();
                        curViewRow = slalomScoreSummaryDataGridView.Rows[curIdx];
                    }

                    curViewRow.Cells["MemberIdSlalom"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionIdSlalom"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["SkierNameSlalom"].Value = (String)curRow["SkierName"];
                    curViewRow.Cells["EventGroupSlalom"].Value = (String)curRow["EventGroupSlalom"];
                    curViewRow.Cells["AgeGroupSlalom"].Value = (String)curRow["AgeGroup"];
                    curViewRow.Cells["PlcmtSlalom"].Value = (String)curRow["PlcmtSlalom"];
                    try {
                        curViewRow.Cells["ScoreSlalom"].Value = (Decimal)curRow["ScoreSlalom"];
                    } catch {
                        curViewRow.Cells["ScoreSlalom"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["NopsScoreSlalom"].Value = (Decimal)curRow["PointsSlalom"];
                    } catch {
                        curViewRow.Cells["NopsScoreSlalom"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["PlcmtPointsSlalom"].Value = ( Decimal ) curRow["PlcmtPointsSlalom"];
                    } catch {
                        curViewRow.Cells["PlcmtPointsSlalom"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["TeamCodeSlalom"].Value = (String)curRow["TeamSlalom"];
                    } catch {
                        curViewRow.Cells["TeamCodeSlalom"].Value = "";
                    }
                    try {
                        curViewRow.Cells["RoundSlalom"].Value = (Byte)curRow["RoundSlalom"];
                    } catch {
                        curViewRow.Cells["RoundSlalom"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapBaseSlalom"].Value = (Decimal)curRow["HCapBase"];
                    } catch {
                        curViewRow.Cells["HCapBaseSlalom"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapScoreSlalom"].Value = (Decimal)curRow["HCapScore"];
                    } catch {
                        curViewRow.Cells["HCapScoreSlalom"].Value = 0;
                    }
                    prevGroup = curGroup;
                }
            }

            myShowTeam = curShowTeamSav;
            SlalomNumLabel.Text = slalomScoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void loadTrickDataGrid( DataTable inTrickDataTable ) {
            String curGroup = "", prevGroup = "";
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderTrick ASC, AgeGroup ASC, TeamTrick ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamTrick ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamTrick ASC, SkierName ASC";
            }
            inTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inTrickDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            trickScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            foreach (DataRow curRow in curDataTable.Rows) {
                curShowRow = true;
                if (myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked) {
                    if (( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
                        || ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
                        ) {
                        curShowRow = true;
                    } else {
                        curShowRow = false;
                    }
                }
                if (curShowRow) {
                    curIdx = trickScoreSummaryDataGridView.Rows.Add();
                    curViewRow = trickScoreSummaryDataGridView.Rows[curIdx];

                    curGroup = (String)curRow["TeamTrick"];
                    if (TeamDiv.Visible) {
                        if (divPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["AgeGroup"];
                        }
                        if (groupPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["EventGroup"];
                        }
                    }
                    if (curGroup != prevGroup && curIdx > 0) {
                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                        trickScoreSummaryDataGridView.Rows[curIdx].Height = 4;

                        curIdx = trickScoreSummaryDataGridView.Rows.Add();
                        curViewRow = trickScoreSummaryDataGridView.Rows[curIdx];
                    }

                    curViewRow.Cells["MemberIdTrick"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionIdTrick"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["SkierNameTrick"].Value = (String)curRow["SkierName"];
                    curViewRow.Cells["EventGroupTrick"].Value = (String)curRow["EventGroupTrick"];
                    curViewRow.Cells["AgeGroupTrick"].Value = (String)curRow["AgeGroup"];
                    curViewRow.Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                    try {
                        curViewRow.Cells["ScoreTrick"].Value = (Int16)curRow["ScoreTrick"];
                    } catch {
                        curViewRow.Cells["ScoreTrick"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["NopsScoreTrick"].Value = (Decimal)curRow["PointsTrick"];
                    } catch {
                        curViewRow.Cells["NopsScoreTrick"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["PlcmtPointsTrick"].Value = ( Decimal ) curRow["PlcmtPointsTrick"];
                    } catch {
                        curViewRow.Cells["PlcmtPointsTrick"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["TeamCodeTrick"].Value = (String)curRow["TeamTrick"];
                    } catch {
                        curViewRow.Cells["TeamCodeTrick"].Value = "";
                    }
                    try {
                        curViewRow.Cells["RoundTrick"].Value = (Byte)curRow["RoundTrick"];
                    } catch {
                        curViewRow.Cells["RoundTrick"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapBaseTrick"].Value = (Decimal)curRow["HCapBase"];
                    } catch {
                        curViewRow.Cells["HCapBaseTrick"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapScoreTrick"].Value = (Decimal)curRow["HCapScore"];
                    } catch {
                        curViewRow.Cells["HCapScoreTrick"].Value = 0;
                    }
                    prevGroup = curGroup;
                }
            }

            myShowTeam = curShowTeamSav;
            TrickNumLabel.Text = trickScoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void loadJumpDataGrid( DataTable inJumpDataTable ) {
            String curGroup = "", prevGroup = "";
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderJump ASC, AgeGroup ASC, TeamJump ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamJump ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamJump ASC, SkierName ASC";
            }
            inJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inJumpDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            jumpScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            foreach ( DataRow curRow in curDataTable.Rows ) {
                curShowRow = true;
                if (myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked) {
                    if (( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
                        || ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
                        ) {
                        curShowRow = true;
                    } else {
                        curShowRow = false;
                    }
                }
                if (curShowRow) {
                    curIdx = jumpScoreSummaryDataGridView.Rows.Add();
                    curViewRow = jumpScoreSummaryDataGridView.Rows[curIdx];

                    curGroup = (String)curRow["TeamJump"];
                    if (TeamDiv.Visible) {
                        if (divPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["AgeGroup"];
                        }
                        if (groupPlcmtButton.Checked) {
                            curGroup += "-" + (String)curRow["EventGroup"];
                        }
                    }
                    if (curGroup != prevGroup && curIdx > 0) {
                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                        jumpScoreSummaryDataGridView.Rows[curIdx].Height = 4;

                        curIdx = jumpScoreSummaryDataGridView.Rows.Add();
                        curViewRow = jumpScoreSummaryDataGridView.Rows[curIdx];
                    }

                    curViewRow.Cells["MemberIdJump"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionIdJump"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["SkierNameJump"].Value = (String)curRow["SkierName"];
                    curViewRow.Cells["EventGroupJump"].Value = (String)curRow["EventGroupJump"];
                    curViewRow.Cells["AgeGroupJump"].Value = (String)curRow["AgeGroup"];
                    curViewRow.Cells["PlcmtJump"].Value = (String)curRow["PlcmtJump"];

                    ScoreMeters.HeaderText = "Score";
                    try {
                        curViewRow.Cells["ScoreMeters"].Value = (Decimal)curRow["ScoreMeters"];
                    } catch {
                        curViewRow.Cells["ScoreMeters"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["ScoreFeet"].Value = (Decimal)curRow["ScoreFeet"];
                    } catch {
                        curViewRow.Cells["ScoreFeet"].Value = 0;
                    }

                    try {
                        curViewRow.Cells["NopsScoreJump"].Value = (Decimal)curRow["PointsJump"];
                    } catch {
                        curViewRow.Cells["NopsScoreJump"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["PlcmtPointsJump"].Value = ( Decimal ) curRow["PlcmtPointsJump"];
                    } catch {
                        curViewRow.Cells["PlcmtPointsJump"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["TeamCodeJump"].Value = (String)curRow["TeamJump"];
                    } catch {
                        curViewRow.Cells["TeamCodeJump"].Value = "";
                    }
                    try {
                        curViewRow.Cells["RoundJump"].Value = (Byte)curRow["RoundJump"];
                    } catch {
                        curViewRow.Cells["RoundJump"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapBaseJump"].Value = (Decimal)curRow["HCapBase"];
                    } catch {
                        curViewRow.Cells["HCapBaseJump"].Value = 0;
                    }
                    try {
                        curViewRow.Cells["HCapScoreJump"].Value = (Decimal)curRow["HCapScore"];
                    } catch {
                        curViewRow.Cells["HCapScoreJump"].Value = 0;
                    }
                    prevGroup = curGroup;
                }
            }

            JumpNumLabel.Text = jumpScoreSummaryDataGridView.Rows.Count.ToString();
            myShowTeam = curShowTeamSav;
        }

        private void loadPrintDataGrid() {
            if ( myShowTeam ) {
                ShowAllButton_Click( null, null );
            }
			int scoreIdxSlalom = 0, scoreIdxTrick = 1, scoreIdxJump = 2, scoreIdxTotal = 3;
			Decimal[] awsaTeamTotal = new decimal[4] { 0, 0, 0, 0 };

			DataRow[] curSlalomTeamSkierList = null;
            DataRow[] curTrickTeamSkierList = null;
            DataRow[] curJumpTeamSkierList = null;
            //DataRow[] curFindRows;
            int curTeamPlcmt = 0;
            String curFilterCmd = "", curDiv = "", prevDiv = "";
            String curSortCmd = "TeamSlalom ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtSlalom ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtSlalom ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtSlalom ASC";
                PrintTeamDiv.Visible = false;
            }
            curSortCmd += ", SkierName ASC, Round ASC";
            mySlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomDataTable.DefaultView.ToTable();

            curSortCmd = "TeamTrick ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtTrick ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtTrick ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtTrick ASC";
            }
            myTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickDataTable.DefaultView.ToTable();

            curSortCmd = "TeamJump ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtJump ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtJump ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtJump ASC";
            }
            myJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpDataTable.DefaultView.ToTable();

            if ( myTourRules.ToLower().Equals("awsa") && plcmtAWSATeamButton.Checked ) {
                PrintPointsSlalom.HeaderText = "NOPS";
                PrintPointsTrick.HeaderText = "NOPS";
                PrintPointsJump.HeaderText = "NOPS";
            } else {
                PrintPointsSlalom.HeaderText = "Points";
                PrintPointsTrick.HeaderText = "Points";
                PrintPointsJump.HeaderText = "Points";
            }

			//Load data for print data grid
			int curPrintIdx = 0, curSkierIdx = 0, curSkierIdxMax = 0, curTeamSummaryRowIdx = 0, curTeamSummarySummaryRowIdx = 0;
			DataGridViewRow curPrintRow = null;
			PrintDataGridView.Rows.Clear();
			foreach ( DataRow curTeamRow in myTeamDataTable.Rows ) {
				try {
					if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
						if ( divPlcmtButton.Checked ) {
							curDiv = (String) curTeamRow["Div"];
						}
						if ( groupPlcmtButton.Checked ) {
							curDiv = (String) curTeamRow["Div"];
						}
						if ( curDiv != prevDiv ) {
							curTeamPlcmt = 0;
							if ( curPrintIdx > 0 ) {
								curPrintRow.Cells["PrintTeamPlcmt"].Value = myPageBreak;
							}
						}

						curTeamPlcmt++;
						curPrintIdx = PrintDataGridView.Rows.Add();
						curPrintRow = PrintDataGridView.Rows[curPrintIdx];
						curPrintRow.Cells["PrintTeamPlcmt"].Value = curTeamPlcmt.ToString();
						curPrintRow.Cells["PrintTeamDiv"].Value = (String) curTeamRow["Div"];

					} else {
						if ( curPrintIdx == 0 ) {
							curTeamPlcmt = 0;
						}
						curTeamPlcmt++;
						curPrintIdx = PrintDataGridView.Rows.Add();
						curPrintRow = PrintDataGridView.Rows[curPrintIdx];
						curPrintRow.Cells["PrintTeamPlcmt"].Value = curTeamPlcmt.ToString();
					}

					curTeamSummaryRowIdx = curPrintIdx;
					String curValue = PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamPlcmt"].Value.ToString();
                    bool curResult = int.TryParse( curValue, out curTeamSummarySummaryRowIdx );
					if ( curResult ) curTeamSummarySummaryRowIdx -= 1;

					try {
						curPrintRow.Cells["PrintTeam"].Value = (String) curTeamRow["TeamCode"] + "-" + (String) curTeamRow["Name"];
					} catch {
						try {
							curPrintRow.Cells["PrintTeam"].Value = (String) curTeamRow["TeamCode"];
						} catch {
							curPrintRow.Cells["PrintTeam"].Value = "";
						}
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventSlalomButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = ( (Decimal) curTeamRow["TeamScoreSlalom"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curTeamRow["TeamScoreSlalom"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsSlalom"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curTeamRow["PlcmtSlalom"];
					} catch {
						curPrintRow.Cells["PrintPlcmtSlalom"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventTrickButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = ( (Decimal) curTeamRow["TeamScoreTrick"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTeamRow["TeamScoreTrick"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsTrick"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTeamRow["PlcmtTrick"];
					} catch {
						curPrintRow.Cells["PrintPlcmtTrick"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsJump"].Value = ( (Decimal) curTeamRow["TeamScoreJump"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsJump"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curTeamRow["TeamScoreJump"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsJump"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curTeamRow["PlcmtJump"];
					} catch {
						curPrintRow.Cells["PrintPlcmtJump"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) {
								curPrintRow.Cells["PrintTeamScoreTotal"].Value = ( (Decimal) curTeamRow["TeamScoreTotal"] ).ToString( "##,##0" );

							} else {
								curPrintRow.Cells["PrintTeamScoreTotal"].Value = "";
							}

						} else {
							curPrintRow.Cells["PrintTeamScoreTotal"].Value = ( (Decimal) curTeamRow["TeamScoreTotal"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintTeamScoreTotal"].Value = "";
					}

					ArrayList curDivList = new ArrayList();
					if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
						if ( divPlcmtButton.Checked ) {
							curDivList.Add( (String) curTeamRow["Div"] );
						}
						if ( groupPlcmtButton.Checked ) {
							curDivList.Add( (String) curTeamRow["Div"] );
						}
					} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
						curDivList.Add( "CM" );
						curDivList.Add( "CW" );
					} else if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
						curDivList.Add( "Cat1" );
						curDivList.Add( "Cat2" );
						curDivList.Add( "Cat3" );

						awsaTeamTotal = new decimal[4] { 0, 0, 0, 0 };

					} else {
						curDivList.Add( "" );
					}

					String curGroupAttrName = "";
					bool curInsertNewRow = false;
					foreach ( String curDivShow in curDivList ) {
						if ( plcmtAWSATeamButton.Checked
							&& ( CategoryAllButton.Checked
								|| ( Category1Button.Checked && curDivShow.Equals( "Cat1" ) )
								|| ( Category2Button.Checked && curDivShow.Equals( "Cat2" ) )
								|| ( Category3Button.Checked && curDivShow.Equals( "Cat3" ) )
							) ) {
						} else {
							continue;
						}

						if ( curInsertNewRow ) {
							curPrintIdx = PrintDataGridView.Rows.Add();
							curPrintRow = PrintDataGridView.Rows[curPrintIdx];
							curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
							PrintDataGridView.Rows[curPrintIdx].Height = 8;
						}
						curInsertNewRow = true;

						if ( divPlcmtButton.Checked ) {
							curGroupAttrName = "AgeGroup";
						}
						if ( groupPlcmtButton.Checked || plcmtAWSATeamButton.Checked ) {
							curGroupAttrName = "EventGroup";
						}

						if ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventSlalomButton.Checked ) ) {
							curFilterCmd = "TeamSlalom='" + (String) curTeamRow["TeamCode"] + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curSlalomTeamSkierList = curSlalomDataTable.Select( curFilterCmd );

						} else {
							curSlalomTeamSkierList = new DataRow[0];
						}

						if ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventTrickButton.Checked ) ) {
							curFilterCmd = "TeamTrick='" + (String) curTeamRow["TeamCode"] + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curTrickTeamSkierList = curTrickDataTable.Select( curFilterCmd );

						} else {
							curTrickTeamSkierList = new DataRow[0];
						}

						if ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) {
							curFilterCmd = "TeamJump='" + (String) curTeamRow["TeamCode"] + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curJumpTeamSkierList = curJumpDataTable.Select( curFilterCmd );

						} else {
							curJumpTeamSkierList = new DataRow[0];
						}

						curSkierIdxMax = 0;
						if ( curSlalomTeamSkierList.Length > curTrickTeamSkierList.Length ) {
							if ( curSlalomTeamSkierList.Length > curJumpTeamSkierList.Length ) {
								curSkierIdxMax = curSlalomTeamSkierList.Length;
							} else {
								curSkierIdxMax = curJumpTeamSkierList.Length;
							}
						} else {
							if ( curTrickTeamSkierList.Length > curJumpTeamSkierList.Length ) {
								curSkierIdxMax = curTrickTeamSkierList.Length;
							} else {
								curSkierIdxMax = curJumpTeamSkierList.Length;
							}
						}

						for ( curSkierIdx = 0; curSkierIdx < curSkierIdxMax; curSkierIdx++ ) {
							try {
								if ( myNumPerTeam > curSkierIdx || myNumPerTeam == 0 ) {
									curPrintIdx = PrintDataGridView.Rows.Add();
									curPrintRow = PrintDataGridView.Rows[curPrintIdx];
									if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
										curPrintRow.Cells["PrintSkierCategory"].Value = curDivShow;
									} else {
										curPrintRow.Cells["PrintSkierCategory"].Value = "";
									}

									if ( curSlalomTeamSkierList.Length > curSkierIdx ) {
										if ( plcmtAWSATeamButton.Checked = false || ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventSlalomButton.Checked ) ) ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curSlalomTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0.0" );
											curPrintRow.Cells["PrintScoreSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" );
											curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"];
											curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxSlalom] += (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
											}

										}
									}


									if ( curTrickTeamSkierList.Length > curSkierIdx) {
										if ( plcmtAWSATeamButton.Checked = false || ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventTrickButton.Checked ) ) ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curTrickTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0.0" );
											curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
											if ( curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType( "System.Int32" ) ) {
												curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int32) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
											} else {
												curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int16) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
											}
											curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"];

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxTrick] += (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
											}
										}
									}

									if ( curJumpTeamSkierList.Length > curSkierIdx) {
										if ( plcmtAWSATeamButton.Checked = false || ( plcmtAWSATeamButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curJumpTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0.0" );
											curPrintRow.Cells["PrintPlcmtPointsJump"].Value = (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
											curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["PlcmtJump"];
											if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
												PrintScoreJump.HeaderText = "Mtrs";
												curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString( "##0.0" );
											} else {
												PrintScoreJump.HeaderText = "Feet";
												curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString( "##0" );
											}

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxJump] += (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
											}
										}
									}

								} else {
									if ( myNumPerTeam == curSkierIdx ) {
										curPrintIdx = PrintDataGridView.Rows.Add();
										curPrintRow = PrintDataGridView.Rows[curPrintIdx];
										curPrintRow.DefaultCellStyle.BackColor = Color.LightGray;
										PrintDataGridView.Rows[curPrintIdx].Height = 4;
									}
									curPrintIdx = PrintDataGridView.Rows.Add();
									curPrintRow = PrintDataGridView.Rows[curPrintIdx];

									if ( curSlalomTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameSlalom"].Value = "* " + (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curSlalomTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameSlalom"].Value = "* " + (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0.0" );
										curPrintRow.Cells["PrintScoreSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" );
										curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"];
										curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
									}
									if ( curTrickTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameTrick"].Value = "* " + (String) curTrickTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curTrickTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameTrick"].Value = "* " + (String) curTrickTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0.0" );
										curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
										if ( curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType( "System.Int32" ) ) {
											curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int32) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
										} else {
											curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int16) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
										}
										curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"];
									}
									if ( curJumpTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameJump"].Value = "* " + (String) curJumpTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curJumpTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameJump"].Value = "* " + (String) curJumpTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0.0" );
										curPrintRow.Cells["PrintPlcmtPointsJump"].Value = (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
										curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["PlcmtJump"];
										if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
											PrintScoreJump.HeaderText = "Mtrs";
											curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString( "##0.0" );
										} else {
											PrintScoreJump.HeaderText = "Feet";
											curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString( "##0" );
										}
									}
								}
							} catch ( Exception ex ) {
								MessageBox.Show( "Exception processing SkierIdx: " + curSkierIdx + " Message: " + ex.Message );

							}
						}
					}

					curPrintIdx = PrintDataGridView.Rows.Add();
					curPrintRow = PrintDataGridView.Rows[curPrintIdx];
					curPrintRow.DefaultCellStyle.BackColor = Color.MediumBlue;
					PrintDataGridView.Rows[curPrintIdx].Height = 8;
					prevDiv = curDiv;

					if ( CategoryAllButton.Checked ) {
						if ( EventAllButton.Checked ) {

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
                            TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category1Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category2Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category3Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}
					}

				} catch ( Exception ex ) {
					MessageBox.Show( "Exception processing TeamDataTable: " + ex.Message );

				}
			}
		}

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( PrintDataGridView );
        }

        private void navLiveWeb_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            ExportLiveWeb.LiveWebDialog.WebLocation = ExportLiveWeb.LiveWebLocation;
            ExportLiveWeb.LiveWebDialog.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if ( ExportLiveWeb.LiveWebDialog.DialogResult == DialogResult.OK ) {
                if ( ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("Set") ) {
                    ExportLiveWeb.LiveWebLocation = ExportLiveWeb.LiveWebDialog.WebLocation;
                    ExportLiveWeb.exportTourData(mySanctionNum);
                    LiveWebLabel.Visible = true;
                } else if ( ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("TwitterActive") ) {
                    //ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterDefaultAccount;
                } else if ( ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("TwitterAuth") ) {
                    //ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterRequestTokenURL;
                } else if ( ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("Disable") ) {
                    ExportLiveWeb.LiveWebLocation = "";
                    ExportLiveTwitter.TwitterLocation = "";
                    LiveWebLabel.Visible = false;
                } else if ( ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("Resend") || ExportLiveWeb.LiveWebDialog.ActionCmd.Equals("ResendAll") ) {
                    if ( ExportLiveWeb.LiveWebLocation.Length > 1 ) {
                        exportTeamData();
                    }
                }
            }
        }

        public void exportTeamData() {
            String curMethodName = "exportTeamData";
            char[] singleQuoteDelim = new char[] { '\'' };
            char[] numPlaceHolder = new char[] { '#' };
            StringBuilder curXml = new StringBuilder("");
            StringBuilder curXmlTemp = new StringBuilder("");
            int curRowCount = 0;

            String curTeamName = "", curSlalomSkierName = "", curTrickSkierName = "", curJumpSkierName = "";
            String[] curKeyColumns = { "SanctionId", "TeamCode", "AgeGroup" };
            String[] curColumns = { "SanctionId", "TeamCode", "AgeGroup", "Name", "ReportFormat", "OverallPlcmt", "SlalomPlcmt", "TrickPlcmt", "JumpPlcmt", "OverallScore", "SlalomScore", "TrickScore", "JumpScore" };

            curXml.Append("<LiveWebRequest>");

            curXml.Append("<Table name=\"TeamScore\" command=\"Delete\" >");
            curXml.Append("<Columns count=\"1\"><Column>SanctionId</Column></Columns>");
            curXml.Append("<Keys count=\"1\"><Key>SanctionId</Key></Keys>");
            curXml.Append("<Rows count=\"1\"><Row colCount=\"1\">");
            curXml.Append("<SanctionId>" + mySanctionNum + "</SanctionId>");
            curXml.Append("</Row></Rows>");
            curXml.Append("</Table>");

            curXml.Append("<Table name=\"TeamScoreDetail\" command=\"Delete\" >");
            curXml.Append("<Columns count=\"1\"><Column>SanctionId</Column></Columns>");
            curXml.Append("<Keys count=\"1\"><Key>SanctionId</Key></Keys>");
            curXml.Append("<Rows count=\"1\"><Row colCount=\"1\">");
            curXml.Append("<SanctionId>" + mySanctionNum + "</SanctionId>");
            curXml.Append("</Row></Rows>");
            curXml.Append("</Table>");

            curXml.Append("<Table name=\"TeamScore\" command=\"Update\" >");
            curXml.Append("<Columns count=\"" + curColumns.Length + "\">");
            foreach ( String curColumn in curColumns ) {
                curXml.Append("<Column>" + curColumn + "</Column>");
            }
            curXml.Append("</Columns>");

            curXml.Append("<Keys count=\"" + curKeyColumns.Length + "\">");
            foreach ( String curColumn in curKeyColumns ) {
                curXml.Append("<Key>" + curColumn + "</Key>");
            }
            curXml.Append("</Keys>");

            curXmlTemp = new StringBuilder("<Rows count=\"#\">");

            foreach ( DataGridViewRow curViewRow in TeamSummaryDataGridView.Rows ) {
                curTeamName = (String) curViewRow.Cells["TeamCode"].Value;
                if ( curTeamName == null ) curTeamName = "";

                if ( curTeamName.Length > 0 ) {
                    curRowCount++;
                    curXmlTemp.Append("<Row colCount=\"" + curColumns.Length + "\">");
                    
                    curXmlTemp.Append("<SanctionId>" + mySanctionNum + "</SanctionId>");
                    curXmlTemp.Append("<TeamCode>" + (String) curViewRow.Cells["TeamCode"].Value + "</TeamCode>");
                    curXmlTemp.Append("<AgeGroup>" + (String) curViewRow.Cells["TeamDiv"].Value + "</AgeGroup>");
                    curXmlTemp.Append("<Name>" + ExportLiveWeb.encodeXmlValue(HelperFunctions.stringReplace((String) curViewRow.Cells["TeamName"].Value, singleQuoteDelim, "''" ) ) + "</Name>");

                    curXmlTemp.Append("<ReportFormat>");
                    if ( myTourRules.ToLower().Equals("awsa") ) {
                        if ( plcmtAWSATeamButton.Checked ) {
                            curXmlTemp.Append("awsa");
                        } else {
                            curXmlTemp.Append("std");
                        }
                    } else if ( myTourRules.ToLower().Equals("ncwsa") ) {
                        curXmlTemp.Append("ncwsa");
                    } else if ( myTourRules.ToLower().Equals("iwwf") ) {
                        curXmlTemp.Append("std");
                    }
                    curXmlTemp.Append("</ReportFormat>");

                    try {
                        curXmlTemp.Append("<OverallPlcmt>" + (String) curViewRow.Cells["TeamPlcmtOverall"].Value + "</OverallPlcmt>");
                    } catch {
                        curXmlTemp.Append("<OverallPlcmt></OverallPlcmt>");
                    }
                    try {
                        curXmlTemp.Append("<SlalomPlcmt>" + (String) curViewRow.Cells["TeamPlcmtSlalom"].Value + "</SlalomPlcmt>");
                    } catch {
                        curXmlTemp.Append("<SlalomPlcmt></SlalomPlcmt>");
                    }
                    try {
                        curXmlTemp.Append("<TrickPlcmt>" + (String) curViewRow.Cells["TeamPlcmtTrick"].Value + "</TrickPlcmt>");
                    } catch {
                        curXmlTemp.Append("<TrickPlcmt></TrickPlcmt>");
                    }
                    try {
                        curXmlTemp.Append("<JumpPlcmt>" + (String) curViewRow.Cells["TeamPlcmtJump"].Value + "</JumpPlcmt>");
                    } catch {
                        curXmlTemp.Append("<JumpPlcmt></JumpPlcmt>");
                    }
                    try {
                        curXmlTemp.Append("<OverallScore>" + ((Decimal) curViewRow.Cells["TeamScoreTotal"].Value).ToString( "###0.0" ) + "</OverallScore>");
                    } catch {
                        curXmlTemp.Append("<OverallScore>0</OverallScore>");
                    }
                    try {
                        curXmlTemp.Append("<SlalomScore>" + ((Decimal) curViewRow.Cells["TeamScoreSlalom"].Value ).ToString( "###0.0" ) + "</SlalomScore>");
                    } catch {
                        curXmlTemp.Append("<SlalomScore>0</SlalomScore>");
                    }
                    try {
                        curXmlTemp.Append("<TrickScore>" + ((Decimal) curViewRow.Cells["TeamScoreTrick"].Value ).ToString( "###0.0" ) + "</TrickScore>");
                    } catch {
                        curXmlTemp.Append("<TrickScore>0</TrickScore>");
                    }
                    try {
                        curXmlTemp.Append("<JumpScore>" + ((Decimal) curViewRow.Cells["TeamScoreJump"].Value ).ToString( "###0.0" ) + "</JumpScore>");
                    } catch {
                        curXmlTemp.Append("<JumpScore>0</JumpScore>");
                    }
                    curXmlTemp.Append("</Row>");
                }

            }
            curXmlTemp.Append("</Rows>");
            curXmlTemp.Append("</Table>");
            curXml.Append(HelperFunctions.stringReplace(curXmlTemp.ToString(), numPlaceHolder, curRowCount.ToString()));

            curKeyColumns = new String[]{ "SanctionId", "TeamCode", "AgeGroup", "LineNum" };
            curColumns = new String[] { "SanctionId", "TeamCode", "AgeGroup", "SkierCategory", "LineNum"
                , "SlalomSkierName", "SlalomPlcmt", "SlalomScore", "SlalomNops", "SlalomPoints"
                , "TrickSkierName", "TrickPlcmt", "TrickScore", "TrickNops", "TrickPoints"
                , "JumpSkierName", "JumpPlcmt", "JumpScore", "JumpNops", "JumpPoints" };

            curXml.Append("<Table name=\"TeamScoreDetail\" command=\"Update\" >");
            curXml.Append("<Columns count=\"" + curColumns.Length + "\">");
            foreach ( String curColumn in curColumns ) {
                curXml.Append("<Column>" + curColumn + "</Column>");
            }
            curXml.Append("</Columns>");

            curXml.Append("<Keys count=\"" + curKeyColumns.Length + "\">");
            foreach ( String curColumn in curKeyColumns ) {
                curXml.Append("<Key>" + curColumn + "</Key>");
            }
            curXml.Append("</Keys>");

            curRowCount = 0;
            curTeamName = "";
            curSlalomSkierName = "";
            curTrickSkierName = "";
            curJumpSkierName = "";
            String curActiveTeamName = "", curAgeGroup = "", curActiveAgeGroup = "";

            curXmlTemp = new StringBuilder("<Rows count=\"#\">");
            int curSkierLineNum = 0;
            foreach ( DataGridViewRow curViewRow in PrintDataGridView.Rows ) {

                curTeamName = (String) curViewRow.Cells["PrintTeam"].Value;
                if ( curTeamName == null ) curTeamName = "";
                curAgeGroup = (String) curViewRow.Cells["PrintTeamDiv"].Value;
                if ( curAgeGroup == null ) curAgeGroup = "";
                curSlalomSkierName = (String) curViewRow.Cells["PrintSkierNameSlalom"].Value;
                if ( curSlalomSkierName == null ) curSlalomSkierName = "";
                curTrickSkierName = (String) curViewRow.Cells["PrintSkierNameTrick"].Value;
                if ( curTrickSkierName == null ) curTrickSkierName = "";
                curJumpSkierName = (String) curViewRow.Cells["PrintSkierNameJump"].Value;
                if ( curJumpSkierName == null ) curJumpSkierName = "";

                if ( curTeamName.Length == 0
                    && curSlalomSkierName.Length == 0
                    && curTrickSkierName.Length == 0
                    && curJumpSkierName.Length == 0
                    ) {
                } else if ( curTeamName.Length > 0
                    && curSlalomSkierName.Length == 0
                    && curTrickSkierName.Length == 0
                    && curJumpSkierName.Length == 0 
                    ) {
                    curSkierLineNum = 0;
                    curActiveTeamName = curTeamName;
                    curActiveAgeGroup = curAgeGroup;
                } else {
                    curRowCount++;
                    curSkierLineNum++;

                    curXmlTemp.Append("<Row colCount=\"" + curColumns.Length + "\">");
                    curXmlTemp.Append("<SanctionId>" + mySanctionNum + "</SanctionId>");
                    curXmlTemp.Append("<TeamCode>" + curActiveTeamName.Substring(0, curActiveTeamName.IndexOf("-") ) + "</TeamCode>");
                    curXmlTemp.Append("<AgeGroup>" + curActiveAgeGroup + "</AgeGroup>");
                    curXmlTemp.Append("<SkierCategory>" + (String) curViewRow.Cells["PrintSkierCategory"].Value + "</SkierCategory>");
                    
                    curXmlTemp.Append("<LineNum>" + curSkierLineNum + "</LineNum>");
                    if ( plcmtAWSATeamButton.Checked ) {
                        curXmlTemp.Append("<SkierCategory>" + (String) curViewRow.Cells["PrintSkierCategory"].Value + "</SkierCategory>");
                    }
					Decimal curDecimalValue = 0;
					if ( curSlalomSkierName.Length > 0 ) {
                        curXmlTemp.Append("<SlalomSkierName>" + ExportLiveWeb.encodeXmlValue(HelperFunctions.stringReplace((String) curViewRow.Cells["PrintSkierNameSlalom"].Value, singleQuoteDelim, "''")) + "</SlalomSkierName>");
                        curXmlTemp.Append("<SlalomPlcmt>" + (String) curViewRow.Cells["PrintPlcmtSlalom"].Value + "</SlalomPlcmt>");
                        try {
							Decimal.TryParse( (String)curViewRow.Cells["PrintScoreSlalom"].Value, out curDecimalValue );
							curXmlTemp.Append( "<SlalomScore>" + curDecimalValue.ToString( "###0.0" ) + "</SlalomScore>" );
						} catch {
                            curXmlTemp.Append("<SlalomScore></SlalomScore>");
                        }
                        try {
							Decimal.TryParse( (String) curViewRow.Cells["PrintPointsSlalom"].Value, out curDecimalValue );
							curXmlTemp.Append("<SlalomNops>" + curDecimalValue.ToString( "###0.0" ) + "</SlalomNops>");
                        } catch {
                            curXmlTemp.Append("<SlalomNops></SlalomNops>");
                        }
                        try {
							curXmlTemp.Append("<SlalomPoints>" + ((Decimal)curViewRow.Cells["PrintPlcmtPointsSlalom"].Value).ToString( "###0.0" ) + "</SlalomPoints>");
                        } catch (Exception ex) {
                            curXmlTemp.Append("<SlalomPoints></SlalomPoints>");
                        }
                    } else {
                        curXmlTemp.Append("<SlalomSkierName></SlalomSkierName>");
                        curXmlTemp.Append("<SlalomPlcmt></SlalomPlcmt>");
                        curXmlTemp.Append("<SlalomScore></SlalomScore>");
                        curXmlTemp.Append("<SlalomNops></SlalomNops>");
                        curXmlTemp.Append("<SlalomPoints></SlalomPoints>");
                    }

                    if ( curTrickSkierName.Length > 0 ) {
                        curXmlTemp.Append("<TrickSkierName>" + ExportLiveWeb.encodeXmlValue(HelperFunctions.stringReplace((String) curViewRow.Cells["PrintSkierNameTrick"].Value, singleQuoteDelim, "''" ) ) + "</TrickSkierName>");
                        curXmlTemp.Append("<TrickPlcmt>" + (String) curViewRow.Cells["PrintPlcmtTrick"].Value + "</TrickPlcmt>");
                        try {
							Decimal.TryParse( (String) curViewRow.Cells["PrintScoreTrick"].Value, out curDecimalValue );
							curXmlTemp.Append("<TrickScore>" + curDecimalValue.ToString( "###0.0" ) + "</TrickScore>");
                        } catch {
                            curXmlTemp.Append("<TrickScore></TrickScore>");
                        }
                        try {
							Decimal.TryParse( (String) curViewRow.Cells["PrintPointsTrick"].Value, out curDecimalValue );
							curXmlTemp.Append("<TrickNops>" + curDecimalValue.ToString( "###0.0" ) + "</TrickNops>");
                        } catch {
                            curXmlTemp.Append("<TrickNops></TrickNops>");
                        }
                        try {
							curXmlTemp.Append( "<TrickPoints>" + ( (Decimal) curViewRow.Cells["PrintPlcmtPointsTrick"].Value ).ToString( "###0.0" ) + "</TrickPoints>" );
						} catch ( Exception ex ) {
							curXmlTemp.Append("<TrickPoints></TrickPoints>");
                        }
                    } else {
                        curXmlTemp.Append("<TrickSkierName></TrickSkierName>");
                        curXmlTemp.Append("<TrickPlcmt></TrickPlcmt>");
                        curXmlTemp.Append("<TrickScore></TrickScore>");
                        curXmlTemp.Append("<TrickNops></TrickNops>");
                        curXmlTemp.Append("<TrickPoints></TrickPoints>");
                    }

                    if ( curJumpSkierName.Length > 0 ) {
                        curXmlTemp.Append("<JumpSkierName>" + ExportLiveWeb.encodeXmlValue(HelperFunctions.stringReplace((String) curViewRow.Cells["PrintSkierNameJump"].Value, singleQuoteDelim, "''" ) ) + "</JumpSkierName>");
                        curXmlTemp.Append("<JumpPlcmt>" + (String) curViewRow.Cells["PrintPlcmtJump"].Value + "</JumpPlcmt>");
                        try {
							Decimal.TryParse( (String) curViewRow.Cells["PrintScoreJump"].Value, out curDecimalValue );
							curXmlTemp.Append("<JumpScore>" + curDecimalValue.ToString( "###0.0" ) + "</JumpScore>");
                        } catch {
                            curXmlTemp.Append("<JumpScore></JumpScore>");
                        }
                        try {
							Decimal.TryParse( (String) curViewRow.Cells["PrintPointsJump"].Value, out curDecimalValue );
							curXmlTemp.Append("<JumpNops>" + curDecimalValue.ToString( "###0.0" ) + "</JumpNops>");
                        } catch {
                            curXmlTemp.Append("<JumpNops></JumpNops>");
                        }
                        try {
							curXmlTemp.Append( "<JumpPoints>" + ( (Decimal) curViewRow.Cells["PrintPlcmtPointsJump"].Value ).ToString( "###0.0" ) + "</JumpPoints>" );
						} catch ( Exception ex ) {
							curXmlTemp.Append("<JumpPoints></JumpPoints>");
                        }
                    } else {
                        curXmlTemp.Append("<JumpSkierName></JumpSkierName>");
                        curXmlTemp.Append("<JumpPlcmt></JumpPlcmt>");
                        curXmlTemp.Append("<JumpScore></JumpScore>");
                        curXmlTemp.Append("<JumpNops></JumpNops>");
                        curXmlTemp.Append("<JumpPoints></JumpPoints>");
                    }

                    curXmlTemp.Append("</Row>");
                }

            }

            curXmlTemp.Append("</Rows>");
            curXmlTemp.Append("</Table>");
            curXml.Append(HelperFunctions.stringReplace(curXmlTemp.ToString(), numPlaceHolder, curRowCount.ToString()));

            curXml.Append("</LiveWebRequest>");

            try {
                Log.WriteFile(curMethodName + ":" + curXml.ToString());
                SendMessageHttp.sendMessagePostXml(ExportLiveWeb.LiveWebLocation, curXml.ToString());
            } catch ( Exception ex ) {
                MessageBox.Show("Error encountered trying to send data to web location \n\nError: " + ex.Message);
                Log.WriteFile(curMethodName + ":Exception=" + ex.Message);
            }
        }

        private void ShowTeamButton_Click( object sender, EventArgs e ) {
            myShowTeam = true;
            String curFilterCmd = "", curTeamCode = "", curGroup = "";
            if ( TeamSummaryDataGridView.Rows.Count > 0 ) {
                if ( TeamSummaryDataGridView.CurrentRow == null ) {
                    curTeamCode = TeamSummaryDataGridView.Rows[0].Cells["TeamCode"].Value.ToString();
                    if ( TeamDiv.Visible && (divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                        curGroup = TeamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                    }
                } else {
                    curTeamCode = TeamSummaryDataGridView.CurrentRow.Cells["TeamCode"].Value.ToString();
                    if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                        curGroup = TeamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                    }
                }
                if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamSlalom = '" + curTeamCode + "'";
                }
                mySlalomDataTable.DefaultView.RowFilter = curFilterCmd;
                loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );

                if (TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamTrick = '" + curTeamCode + "'";
                }
                myTrickDataTable.DefaultView.RowFilter = curFilterCmd;
                loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );

                if (TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamJump = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamJump = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamJump = '" + curTeamCode + "'";
                }
                myJumpDataTable.DefaultView.RowFilter = curFilterCmd;
                loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
            }
        }

        private void ShowAllButton_Click( object sender, EventArgs e ) {
            myShowTeam = false;
            mySlalomDataTable.DefaultView.RowFilter = "";
            loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );
            myTrickDataTable.DefaultView.RowFilter = "";
            loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );
            myJumpDataTable.DefaultView.RowFilter = "";
            loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
        }

        private void TeamSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( TeamSummaryDataGridView.RowCount > 0 ) {
                if ( myShowTeam ) {
                    if ( TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value != null ) {
                        String curFilterCmd = "", curTeamCode = "", curGroup = "";
                        curTeamCode = TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value.ToString();
                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            curGroup = TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamDiv"].Value.ToString();
                        }
                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamSlalom = '" + curTeamCode + "'";
                        }
                        mySlalomDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );

                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamTrick = '" + curTeamCode + "'";
                        }
                        myTrickDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );

                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamJump = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamJump = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamJump = '" + curTeamCode + "'";
                        }
                        myJumpDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
                    
                    }
                }
            }
        }

        private void NumPerTeamTextBox_TextChanged( object sender, EventArgs e ) {
            try {
                myNumPerTeam = Convert.ToInt16( NumPerTeamTextBox.Text );
                myTourProperties.TeamSummary_NumPerTeam = myNumPerTeam.ToString();
            } catch {
                if ( NumPerTeamTextBox.Text.Length > 0 ) {
                    MessageBox.Show( "Invalid value in textbox.  Must be a proper integer value." );
                    myNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam);
                }
            }
        }

        private void Button_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curButton = (RadioButton)sender;
            if ( curButton.Checked ) {
                if ( curButton.Name.Equals( "handicapPointsButton" ) || curButton.Name.Equals( "ratioPointsButton" ) ) {
                    HCapBaseSlalom.Visible = true;
                    HCapScoreSlalom.Visible = true;
                    HCapBaseTrick.Visible = true;
                    HCapScoreTrick.Visible = true;
                    HCapBaseJump.Visible = true;
                    HCapScoreJump.Visible = true;
                }
            } else {
                if ( curButton.Name.Equals( "handicapPointsButton" ) || curButton.Name.Equals( "ratioPointsButton" ) ) {
                    HCapBaseSlalom.Visible = false;
                    HCapScoreSlalom.Visible = false;
                    HCapBaseTrick.Visible = false;
                    HCapScoreTrick.Visible = false;
                    HCapBaseJump.Visible = false;
                    HCapScoreJump.Visible = false;
                }
            }
        }

        private void groupPlcmtButton_CheckedChanged( object sender, EventArgs e ) {
            if ( divPlcmtButton.Checked ) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Div";
                PrintTeamDiv.HeaderText = "Div";
                EventGroupSlalom.Visible = true;
                EventGroupTrick.Visible = true;
                EventGroupJump.Visible = true;
                EventGroupList.Visible = true;
                EventGroupListLabel.Visible = true;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
            if (groupPlcmtButton.Checked) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Group";
                PrintTeamDiv.HeaderText = "Group";
                EventGroupSlalom.Visible = true;
                EventGroupTrick.Visible = true;
                EventGroupJump.Visible = true;
                EventGroupList.Visible = true;
                EventGroupListLabel.Visible = true;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
        }

        private void ViewReportButton_Click(object sender, EventArgs e) {
            if ( PrintDataGridView.Visible ) {
                PrintDataGridView.Visible = false;
                PrintDataGridView.Enabled = false;

				ViewReportButton.Text = "View Team Results";
            } else {
				PrintDataGridView.Visible = true;
                PrintDataGridView.Enabled = true;
                PrintDataGridView.Location = TeamSummaryDataGridView.Location;
                PrintDataGridView.Width = this.Width - 25;
                PrintDataGridView.Height = TeamSummaryDataGridView.Height;
                ViewReportButton.Text = "Hide Team Results";
            }
        }

		private void FilterReportButton_Click( object sender, EventArgs e ) {
			loadPrintDataGrid();
		}

		private void plcmtTourButton_CheckedChanged( object sender, EventArgs e ) {
            if ( plcmtTourButton.Checked ) {
                TeamDiv.Visible = false;
                EventGroupSlalom.Visible = false;
                EventGroupTrick.Visible = false;
                EventGroupJump.Visible = false;
                EventGroupList.Visible = false;
                EventGroupListLabel.Visible = false;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
        }

        private void plcmtAwsaTeamButton_CheckedChanged( object sender, EventArgs e ) {
            if ( plcmtAWSATeamButton.Checked ) {
                TeamDiv.Visible = true;
                EventGroupSlalom.Visible = false;
                EventGroupTrick.Visible = false;
                EventGroupJump.Visible = false;
                EventGroupList.Visible = false;
                EventGroupListLabel.Visible = false;
                PrintPlcmtPointsSlalom.Visible = true;
                PrintPlcmtPointsTrick.Visible = true;
                PrintPlcmtPointsJump.Visible = true;
                PlcmtPointsSlalom.Visible = true;
                PlcmtPointsTrick.Visible = true;
                PlcmtPointsJump.Visible = true;

				CategoryGroupBox.Visible = true;
				CategoryGroupBox.Enabled = true;
				EventGroupBox.Visible = true;
				EventGroupBox.Enabled = true;

				EventGroupList.Visible = false;
				EventGroupList.Enabled = false;
				EventGroupListLabel.Visible = false;
				EventGroupListLabel.Enabled = false;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

			} else {
				CategoryGroupBox.Visible = false;
                CategoryGroupBox.Enabled = false;
				EventGroupBox.Visible = false;
				EventGroupBox.Enabled = false;

				EventGroupList.Visible = true;
				EventGroupList.Enabled = true;
				EventGroupListLabel.Visible = true;
				EventGroupListLabel.Enabled = true;

				FilterReportButton.Visible = false;
				FilterReportButton.Enabled = false;
			}
		}

		private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight( 15, '*' );
            printFooter.PadLeft( 15, '*' );
            myExportData.exportDataAsHtml( PrintDataGridView, printTitle, printSubtitle, printFooter );
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            Font saveShowDefaultCellStyle = TeamSummaryDataGridView.DefaultCellStyle.Font;
            //TeamSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );
            TeamSummaryDataGridView.RowTemplate.Height = 28;

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = true;
            curPrintDialog.AllowSelection = false;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String curReportTitle = this.Text;
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + curReportTitle;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = curReportTitle + "-Team";
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter( TeamSummaryDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();

                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = curReportTitle + "-Detail";
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }

            TeamSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;
            TeamSummaryDataGridView.RowTemplate.Height = 28;
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

	}
}
