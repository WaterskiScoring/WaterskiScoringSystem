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

namespace WaterskiScoringSystem.Trick {
    public partial class TrickSummaryAwards : Form {
        private String myPageBreak = "\\n";
        private String mySanctionNum = null;
        private String myEvent = "Trick";
        private String mySortCommand = "";
        private String myFilterCmd = "";
        private String myTourRules = "";

        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;

        private DataRow myTourRow;
        private TourProperties myTourProperties;
        private DataTable mySummaryDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public TrickSummaryAwards() {
            InitializeComponent();
        }

        private void TrickSummaryAwards_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.TrickScoreSummary_Width > 0) {
                this.Width = Properties.Settings.Default.TrickScoreSummary_Width;
            }
            if (Properties.Settings.Default.TrickScoreSummary_Height > 0) {
                this.Height = Properties.Settings.Default.TrickScoreSummary_Height;
            }
            if (Properties.Settings.Default.TrickScoreSummary_Location.X > 0
                && Properties.Settings.Default.TrickScoreSummary_Location.Y > 0) {
                this.Location = Properties.Settings.Default.TrickScoreSummary_Location;
            }
            myTourProperties = TourProperties.Instance;
            bestScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "total" )) totalScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "final" )) finalScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "first" )) firstScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "round" )) roundScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryDataType.ToLower().Equals( "h2h" )) h2hScoreButton.Checked = true;
            rawScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtMethod.ToLower().Equals( "score" )) rawScoreButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtMethod.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;
            groupPlcmtButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "group" )) groupPlcmtButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "tour" )) plcmtTourButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "div" )) plcmtDivButton.Checked = true;
            if (myTourProperties.TrickSummaryPlcmtOrg.ToLower().Equals( "divgr" )) plcmtDivGrpButton.Checked = true;
            nopsPointsButton.Checked = true;
            if (myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
            if (myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
            if (myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
            if (myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "hcap" )) handicapPointsButton.Checked = true;
            if (myTourProperties.TrickSummaryPointsMethod.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;
            numPrelimTextBox.Text = myTourProperties.TrickSummaryNumPrelim;
            numAwardsTextBox.Text = myTourProperties.TrickSummaryAwardsNum;

            String[] curList = { "MemberId", "SanctionId", "SkierName", "City", "State", "SkiYearAge", "AgeGroup", "EventGroup"
					, "EventClassTrick", "PlcmtTrick", "TeamTrick", "HCapBaseTrick"
                    , "RoundTrick", "ScoreTrick", "Pass1Trick", "Pass2Trick" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if (mySanctionNum == null) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];

                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            Round.Visible = false;
                            EventClass.Visible = false;
                            TeamCodeNcwsa.Visible = true;
                            TeamCode.Visible = false;
                        } else {
                            TeamCodeNcwsa.Visible = false;
                            TeamCode.Visible = true;
                        }

                        if ( myTourProperties.TrickScoreSummary_Sort.Length > 0 ) {
                            mySortCommand = myTourProperties.TrickScoreSummary_Sort;
                        } else {
                            mySortCommand = "";
                        }
                        loadGroupList();
                    }
                }
            }
        }

        public void navRefresh_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( mySanctionNum != null && myEvent != null ) {
                String curDataType = "all", curPlcmtMethod = "score";
                String curPlcmtOrg, curPointsMethod;
                Int16 curNumPrelimRounds = 0;
                CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                // Retrieve data from database depending on selection criteria
                String curMsg = "Tournament scores retrieved ";

                if ( bestScoreButton.Checked ) {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                } else if ( totalScoreButton.Checked ) {
                    curDataType = "total";
                    winStatusMsg.Text = curMsg + "- total scores";
                } else if ( finalScoreButton.Checked ) {
                    curDataType = "final";
                    winStatusMsg.Text = curMsg + "- final scores";
                } else if ( firstScoreButton.Checked ) {
                    curDataType = "first";
                    winStatusMsg.Text = curMsg + "- first scores";
                } else if ( roundScoreButton.Checked ) {
                    curDataType = "round";
                    winStatusMsg.Text = curMsg + "- all scores";
                } else if (h2hScoreButton.Checked) {
                    curDataType = "h2h";
                    winStatusMsg.Text = curMsg + "- head to head";
                } else {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                }
                myTourProperties.TrickSummaryDataType = curDataType;

                if ( rawScoreButton.Checked ) {
                    curPlcmtMethod = "score";
                } else if ( pointsScoreButton.Checked ) {
                    curPlcmtMethod = "points";
                } else {
                    curPlcmtMethod = "score";
                }
                myTourProperties.TrickSummaryPlcmtMethod = curPlcmtMethod;

                if (groupPlcmtButton.Checked) {
                    curPlcmtOrg = "group";
                    EventGroup.Visible = true;
                } else if (plcmtTourButton.Checked) {
                    curPlcmtOrg = "tour";
                    EventGroup.Visible = false;
                } else if (plcmtDivButton.Checked) {
                    curPlcmtOrg = "div";
                    EventGroup.Visible = false;
                } else if (plcmtDivGrpButton.Checked) {
                    curPlcmtOrg = "divgr";
                    EventGroup.Visible = true;
                } else {
                    curPlcmtOrg = "tour";
                    EventGroup.Visible = false;
                }
                myTourProperties.TrickSummaryPlcmtOrg = curPlcmtOrg;

                HCapBase.Visible = false;
                HCapScore.Visible = false;
                if ( nopsPointsButton.Checked ) {
                    curPointsMethod = "nops";
                } else if ( plcmtPointsButton.Checked ) {
                    curPointsMethod = "plcmt";
                } else if ( kBasePointsButton.Checked ) {
                    curPointsMethod = "kbase";
                } else if ( handicapPointsButton.Checked ) {
                    curPointsMethod = "hcap";
                    HCapBase.Visible = true;
                    HCapScore.Visible = true;
                } else if ( ratioPointsButton.Checked ) {
                    curPointsMethod = "ratio";
                    HCapBase.Visible = true;
                    HCapScore.Visible = true;
                } else {
                    curPointsMethod = "nops";
                }
                myTourProperties.TrickSummaryPointsMethod = curPointsMethod;

                if ( myTourRules.ToLower().Equals( "iwwf" ) && kBasePointsButton.Checked ) {
                    HCapBase.Visible = false;
                    HCapScore.Visible = false;
                }

                if ( h2hScoreButton.Checked ) {
                    EventGroup.Visible = true;
                    if ( plcmtTourButton.Checked ) {
                        curPlcmtOrg = "tour";
                    } else {
                        curPlcmtOrg = "div";
                    }
                }
                if ( h2hScoreButton.Checked || finalScoreButton.Checked ) {
                    if ( numPrelimTextBox.Text.Length == 0 ) {
                        MessageBox.Show( "Number of preliminary rounds is required for Head to Head reporting" );
                        return;
                    } else {
                        curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
                    }
                }

                String curGroupValue = "";
                try {
                    curGroupValue = EventGroupList.SelectedItem.ToString();
                    if (!( curGroupValue.ToLower().Equals( "all" ) )) {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (curGroupValue.ToUpper().Equals( "MEN A" )) {
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

                this.Cursor = Cursors.WaitCursor;
                if (curGroupValue.ToLower().Equals( "all" )) {
                    if (myTourRules.ToLower().Equals( "iwwf" )) {
                        curPointsMethod = "kbase";
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Trick", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                    } else if ( h2hScoreButton.Checked || ( finalScoreButton.Checked && curNumPrelimRounds > 0 ) ) {
                        mySummaryDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, curNumPrelimRounds );
                    } else {
                        mySummaryDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    }
                } else {
                    if (myTourRules.ToLower().Equals( "iwwf" )) {
                        curPointsMethod = "kbase";
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Trick", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, curGroupValue );
                    } else if ( h2hScoreButton.Checked || ( finalScoreButton.Checked && curNumPrelimRounds > 0 ) ) {
                        mySummaryDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue, curNumPrelimRounds );
                    } else {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            mySummaryDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        } else {
                            mySummaryDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        }
                    }
                }

                if (mySummaryDataTable.Rows.Count > 0) {
                    if ( h2hScoreButton.Checked && curNumPrelimRounds > 0 ) {
                        mySummaryDataTable = curCalcSummary.buildHeadToHeadSummary( mySummaryDataTable, myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, curNumPrelimRounds, "Trick" );
                    } else {
                        if (mySortCommand.Length > 1) {
                            if (myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" )) {
                            } else {
                                mySummaryDataTable.DefaultView.Sort = mySortCommand;
                                mySummaryDataTable = mySummaryDataTable.DefaultView.ToTable();
                            }
                        }
                    }
                    loadDataGrid( mySummaryDataTable );
                } else {
                    scoreSummaryDataGridView.Rows.Clear();
                    MessageBox.Show( "No data available for report" );
                }
            }
            this.Cursor = Cursors.Default;
        }

        private void loadDataGrid(DataTable inDataTable) {
            bool isShowRow = false;
            Int16 curRound = 0, prevRound = 0, curNumPrelimRounds = 0;
            int curAwardsLimit = 0, curSkierPlcmt = 0;
            String curEventClass = "";
            String curReportGroup = "", prevReportGroup = "";
            String curDiv = "", curGroup = "";
            DataGridViewRow curViewRow;
            scoreSummaryDataGridView.Rows.Clear();

            if ( inDataTable == null || inDataTable.Rows.Count == 0 ) {
                MessageBox.Show( "No data available for display" );
                return;
            }
            try {
                curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
            } catch {
                curNumPrelimRounds = 0;
            }
            try {
                curAwardsLimit = Convert.ToInt32( numAwardsTextBox.Text );
                if ( h2hScoreButton.Checked || finalScoreButton.Checked) {
                    curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
                }
            } catch {
                curAwardsLimit = 0;
                curNumPrelimRounds = 0;
            }

            this.Cursor = Cursors.WaitCursor;
            inDataTable.DefaultView.Sort = mySortCommand;
            inDataTable.DefaultView.RowFilter = myFilterCmd;
            DataTable curDataTable = inDataTable.DefaultView.ToTable();

            int curIdx = 0;
            foreach ( DataRow curRow in curDataTable.Rows ) {
                try {
                    curEventClass = (String)curRow["EventClassTrick"];
                } catch {
                    curEventClass = "";
                }
                prevRound = curRound;
                try {
                    curRound = ( Int16 ) curRow["RoundTrick"];
                } catch {
                    curRound = 0;
                }

                if ( curEventClass.Length > 0 && curRound > 0 ) {
                    if ( h2hScoreButton.Checked || ( finalScoreButton.Checked && curNumPrelimRounds > 0 ) ) {
                        if ( curRound > curNumPrelimRounds ) {
                            isShowRow = true;
                        } else {
                            isShowRow = false;
                        }
                    } else {
                        curSkierPlcmt = getPlcmtNum( (String)curRow["PlcmtTrick"] );
                        if (curSkierPlcmt > 0 && curAwardsLimit > 0 && curSkierPlcmt <= curAwardsLimit) {
                            isShowRow = true;
                        } else {
                            isShowRow = false;
                        }
                    }

                    if (isShowRow) {
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                        prevReportGroup = curReportGroup;

                        #region Determine group, division and any required data breaks
                        curGroup = ( String ) curRow["EventGroup"];
                        if ( ( ( String ) curRow["AgeGroup"] ).Length > 2 ) {
                            curDiv = ( ( String ) curRow["AgeGroup"] ).Substring( 0, 2 );
                        } else {
                            curDiv = ( String ) curRow["AgeGroup"];
                        }
                        if ( groupPlcmtButton.Checked ) {
                            curReportGroup = curGroup;
                        } else if ( plcmtTourButton.Checked ) {
                            if ( h2hScoreButton.Checked || finalScoreButton.Checked ) {
                                if ( curRound > curNumPrelimRounds ) {
                                    curReportGroup = curGroup;
                                } else {
                                    curReportGroup = "";
                                }
                            } else {
                                curReportGroup = "";
                            }
                        } else if ( plcmtDivButton.Checked ) {
                            curReportGroup = curDiv;
                        } else if ( plcmtDivGrpButton.Checked ) {
                            curReportGroup = curDiv + "-" + curGroup;
                        } else {
                            curReportGroup = curGroup;
                        }
                        if ( ( curReportGroup != prevReportGroup ) && prevReportGroup.Length > 0 ) {
                            if ( roundScoreButton.Checked || h2hScoreButton.Checked || finalScoreButton.Checked ) {
                                if ( curRound != prevRound && curIdx > 0 ) {
                                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                                    if ( ( h2hScoreButton.Checked || finalScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
                                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                        scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                        curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                    } else {
                                        scoreSummaryDataGridView.Rows[curIdx].Height = 2;
                                        curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
                                    }
                                    curIdx = scoreSummaryDataGridView.Rows.Add();

                                } else {
                                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                                    if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                        scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                        curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                    } else if ( ( h2hScoreButton.Checked || finalScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
                                        if ( curRound != prevRound ) {
                                            curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                            scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                            curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                        } else {
                                            curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
                                            scoreSummaryDataGridView.Rows[curIdx].Height = 2;
                                        }
                                    }
                                    curIdx = scoreSummaryDataGridView.Rows.Add();
                                }

                            } else {
                                curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                                if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                                    curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                    scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                    curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                } else if ( ( h2hScoreButton.Checked || finalScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
                                    if ( curRound != prevRound ) {
                                        curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
                                        scoreSummaryDataGridView.Rows[curIdx].Height = 2;
                                        curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                    } else {
                                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                        scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                    }
                                }
                                curIdx = scoreSummaryDataGridView.Rows.Add();
                            }
                        } else if ( roundScoreButton.Checked || ( ( h2hScoreButton.Checked || finalScoreButton.Checked ) && prevRound > curNumPrelimRounds ) ) {
                            if ( curRound != prevRound && curIdx > 0 ) {
                                curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                                if ( h2hScoreButton.Checked && prevRound > curNumPrelimRounds ) {
                                    curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                    curViewRow.Cells["PlcmtTrick"].Value = myPageBreak;
                                    scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                                } else {
                                    curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
                                    scoreSummaryDataGridView.Rows[curIdx].Height = 2;
                                }
                                curIdx = scoreSummaryDataGridView.Rows.Add();
                            }
                        }
                        #endregion

                        #region Format report display data
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                        curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                        curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];

                        try {
                            curViewRow.Cells["Hometown"].Value = (String)curRow["City"] + ", " + (String)curRow["State"];
                        } catch {
                            curViewRow.Cells["Hometown"].Value = "";
                            try {
                                curViewRow.Cells["Hometown"].Value = (String)curRow["City"];
                            } catch {
                            }
                            try {
                                if (curViewRow.Cells["Hometown"].Value.ToString().Length > 1) {
                                    curViewRow.Cells["Hometown"].Value = ", " + (String)curRow["State"];
                                } else {
                                    curViewRow.Cells["Hometown"].Value = (String)curRow["State"];
                                }
                            } catch {
                            }
                        }
                        try {
                            curViewRow.Cells["SkiYearAge"].Value = ( (Byte)curRow["SkiYearAge"] ).ToString();
                        } catch {
                            curViewRow.Cells["SkiYearAge"].Value = "";
                        }

                        curViewRow.Cells["EventGroup"].Value = (String)curRow["EventGroup"];
                        if (( (String)curRow["AgeGroup"] ).Length > 2) {
                            curViewRow.Cells["AgeGroup"].Value = ( (String)curRow["AgeGroup"] ).Substring( 0, 2 );
                        } else {
                            curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
                        }
                        curViewRow.Cells["Event"].Value = "Trick";
                        curViewRow.Cells["EventClass"].Value = (String)curRow["EventClassTrick"];
                        curViewRow.Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                        try {
                            curViewRow.Cells["TeamCode"].Value = (String)curRow["TeamTrick"];
                            curViewRow.Cells["TeamCodeNcwsa"].Value = (String)curRow["TeamTrick"];
                        } catch {
                            curViewRow.Cells["TeamCode"].Value = "";
                            curViewRow.Cells["TeamCodeNcwsa"].Value = "";
                        }
                        curViewRow.Cells["HCapBase"].Value = 0;
                        curViewRow.Cells["HCapScore"].Value = 0;
                        try {
                            curViewRow.Cells["Round"].Value = (Int16)curRow["RoundTrick"];
                        } catch {
                            curViewRow.Cells["Round"].Value = 0;
                        }
                        try {
                            curViewRow.Cells["ScorePass1"].Value = (Int16)curRow["Pass1Trick"];
                        } catch {
                            try {
                                curViewRow.Cells["ScorePass1"].Value = (int)curRow["Pass1Trick"];
                            } catch {
                                curViewRow.Cells["ScorePass1"].Value = 0;
                            }
                        }
                        try {
                            curViewRow.Cells["ScorePass2"].Value = (Int16)curRow["Pass2Trick"];
                        } catch {
                            try {
                                curViewRow.Cells["ScorePass2"].Value = (int)curRow["Pass2Trick"];
                            } catch {
                                curViewRow.Cells["ScorePass2"].Value = 0;
                            }
                        }
                        try {
                            curViewRow.Cells["Score"].Value = (Int16)curRow["ScoreTrick"];
                        } catch {
                            try {
                                curViewRow.Cells["Score"].Value = (int)curRow["ScoreTrick"];
                            } catch {
                                curViewRow.Cells["Score"].Value = 0;
                            }
                        }
                        try {
                            curViewRow.Cells["NopsScore"].Value = ( (Decimal)curRow["PointsTrick"] ).ToString( "###0.0" );
                        } catch {
                            curViewRow.Cells["NopsScore"].Value = 0;
                        }
                        #endregion
                    }
                }
            }
            if (ExportLiveScoreboard.ScoreboardLocation.Length > 1) {
                ExportLiveScoreboard.exportCurrentLeaderTrick(mySummaryDataTable, mySanctionNum);
            }
            this.Cursor = Cursors.Default;
            try {
                RowStatusLabel.Text = "Row 1 of " + scoreSummaryDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }
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
                    //loadEventGroupListFromData();
                }
            } else {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadAgeGroupListFromData();
                        //loadEventGroupListFromData();
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
                        //loadEventGroupListFromData();
                    }
                }
            }
        }

        private void loadEventGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Trick'"
                + "Order by EventGroup";
            DataTable curDataTable = getData( curSqlStmt );

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["EventGroup"] );
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

        private void loadAgeGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT AgeGroup FROM EventReg "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Trick' "
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

        private void ScoreSummary_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.TrickScoreSummary_Width = this.Size.Width;
                Properties.Settings.Default.TrickScoreSummary_Height = this.Size.Height;
                Properties.Settings.Default.TrickScoreSummary_Location = this.Location;
            }
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

        private int getPlcmtNum(String inPlcmt) {
            int curReturnValue = 0;
            int curDelimPos = inPlcmt.ToUpper().IndexOf( 'T' );
            if (curDelimPos > 0) {
                try {
                    curReturnValue = Convert.ToInt32( inPlcmt.Substring( 0, curDelimPos ) );
                } catch {
                    curReturnValue = 0;
                }
            } else {
                try {
                    curReturnValue = Convert.ToInt32( inPlcmt );
                } catch {
                    curReturnValue = 0;
                }
            }
            return curReturnValue;
        }

        private void numPrelimTextBox_Validated(object sender, EventArgs e) {
            if (numPrelimTextBox.Text.Length > 0) {
                try {
                    Int16 curValue = Convert.ToInt16( numPrelimTextBox.Text );
                    if (curValue > 10) {
                        MessageBox.Show( "Number of preliminary rounds must be less than 10 " );
                    }
                    myTourProperties.TrickSummaryNumPrelim = numPrelimTextBox.Text;
                } catch {
                    MessageBox.Show( "Number of preliminary rounds must be numeric " );
                }
            }
        }

        private void numAwardsTextBox_Validated(object sender, EventArgs e) {
            if (numAwardsTextBox.Text.Length > 0) {
                try {
                    Int16 curValue = Convert.ToInt16( numAwardsTextBox.Text );
                    myTourProperties.TrickSummaryAwardsNum = numAwardsTextBox.Text;
                } catch {
                    MessageBox.Show( "Number of awards must be numeric " );
                }
            }

        }

        private void navSort_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (sortDialogForm.DialogResult == DialogResult.OK) {
                mySortCommand = sortDialogForm.SortCommand;
                myTourProperties.TrickScoreSummary_Sort = mySortCommand;
                loadDataGrid( mySummaryDataTable );
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.FilterCommand = myFilterCmd;
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if (filterDialogForm.DialogResult == DialogResult.OK) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadDataGrid( mySummaryDataTable );
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( scoreSummaryDataGridView );
        }

        private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight( 15, '*' );
            printFooter.PadLeft( 15, '*' );
            myExportData.exportDataAsHtml( scoreSummaryDataGridView, printTitle, printSubtitle, printFooter );
        }

        private void navScoreboard_Click(object sender, EventArgs e) {
            String curPath = ExportLiveScoreboard.getCheckScoreboardLocation();
        }

        private void scoreSummaryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + scoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void navPublish_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( publishReportTimer );
            curTimerObj.Start();
        }
        private void navPrint_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }

        private void publishReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( publishReportTimer );
            if ( printReport( true ) ) ExportLiveWeb.uploadReportFile( "Results", "Trick", mySanctionNum );
        }
        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printReport( false );
        }

        private bool printReport( bool inPublish ) {
            if ( inPublish && !( LiveWebHandler.LiveWebMessageHandlerActive ) ) LiveWebHandler.connectLiveWebHandler( mySanctionNum );
            if ( inPublish && !( LiveWebHandler.LiveWebMessageHandlerActive ) ) {
                MessageBox.Show( "Request to publish report but live web not successfully connected." );
                return false;
            }

            if ( h2hScoreButton.Checked) {
                if (mySummaryDataTable.Rows.Count > 0) {
                    printHeadToHeadAwards();
					return true;
				} else {
					return false;
				}
			}

            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            Font saveShowDefaultCellStyle = scoreSummaryDataGridView.DefaultCellStyle.Font;
            scoreSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );

			bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;

            StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
            printTitle.Append( "\n Sanction " + mySanctionNum );
            printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
            printTitle.Append( "\n" + this.Text );
            if ( inPublish ) printTitle.Append( HelperFunctions.buildPublishReportTitle( mySanctionNum ) );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = this.Text;
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = false;
            myPrintDataGrid = new DataGridViewPrinter( scoreSummaryDataGridView, myPrintDoc,
                CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

            curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();

            scoreSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;
			return true;
		}

		// The PrintPage action for the PrintDocument control
		private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }

        private void printHeadToHeadAwards() {
            String curFilterCmd = "", curSortCmd = "";
            String curPlcmtOrg = "", curPlcmtMethod = "Score";
            Int16 curNumPrelimRounds = 0;
            try {
                curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
            } catch {
                curNumPrelimRounds = 0;
            }
            if (myFilterCmd.Length > 0) {
            } else {
                curFilterCmd = "Round > " + curNumPrelimRounds;
            }

            curSortCmd = "Round ASC";
            if (plcmtTourButton.Checked) {
            } else {
                curSortCmd = "AgeGroup ASC, Round ASC";
            }
            curSortCmd += ", EventGroup ASC";
            if (rawScoreButton.Checked) {
                curSortCmd += ", ScoreTrick DESC";
            } else if (pointsScoreButton.Checked) {
                curPlcmtMethod = "Points";
                curSortCmd += ", PointsTrick DESC";
            } else {
                curSortCmd += ", ScoreTrick DESC";
            }

            if (plcmtDivButton.Checked) {
                curPlcmtOrg = "div";
            } else {
                curPlcmtOrg = "tour";
            }

            PrintHeadToHeadAwards curPrintForm = new PrintHeadToHeadAwards();
            curPrintForm.PrintLandscape = true;
            curPrintForm.ReportHeader = "Head to Head Results";
            curPrintForm.DivInfoDataTable = getEventDivInfo();
            curPrintForm.TourRules = myTourRules;
            curPrintForm.TourName = (String)myTourRow["Name"];
            curPrintForm.TourPrelimRounds = curNumPrelimRounds;
            curPrintForm.TourEvent = "Trick";
            curPrintForm.TourPlcmtOrg = curPlcmtOrg;
            curPrintForm.TourPlcmtMethod = curPlcmtMethod;

            #region Enhance scoring data with seeding (RunOrder) and seeding score (RankingScore)
            DataTable curEventRegDataTable = getEventRegData( curNumPrelimRounds, "Trick" );

            mySummaryDataTable.DefaultView.Sort = curSortCmd;
            mySummaryDataTable.DefaultView.RowFilter = curFilterCmd;
            DataTable curShowDataTable = mySummaryDataTable.DefaultView.ToTable();
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "RunOrder";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curShowDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curShowDataTable.Columns.Add( curCol );


            DataRow[] curFindRows = null;
            String curMemberId = "", curAgeGroup = "";
            int curRound = 0;
            foreach (DataRow curRow in curShowDataTable.Rows) {
                curMemberId = (String)curRow["MemberId"];
                curAgeGroup = (String)curRow["AgeGroup"];
                curRound = (Int16)curRow["Round"];
                curFindRows = curEventRegDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "' AND Round = " + curRound );
                if (curFindRows.Length > 0) {
                    try {
                        curRow["RunOrder"] = curFindRows[0]["RunOrder"];
                    } catch {
                        curRow["RunOrder"] = 0;
                    }
                    try {
                        curRow["RankingScore"] = curFindRows[0]["RankingScore"];
                    } catch {
                        curRow["RankingScore"] = 0;
                    }
                } else {
                    curRow["RunOrder"] = 0;
                    curRow["RankingScore"] = 0;
                }
            }
            curPrintForm.ShowDataTable = curShowDataTable;
            #endregion

            curPrintForm.Print();
        }

        private DataTable getEventRegData(int inNumPrelimRounds, String inEvent) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.Event, E.SanctionId, E.MemberId, E.AgeGroup, O.Round, O.EventGroup, O.RunOrder" );
            curSqlStmt.Append( ", E.EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore " );
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + inEvent + "' AND O.Round > " + inNumPrelimRounds.ToString() + " " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getEventDivInfo() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return getData( curSqlStmt.ToString() );
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

        private void scoreSummaryDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {

        }

	}
}
