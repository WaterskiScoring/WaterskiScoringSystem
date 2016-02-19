using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class MasterSummaryOverallUS : Form {
        private String mySanctionNum = null;
        private String myTourRules = "";
        private DataRow myTourRow;
        private DataTable mySummaryDataTable;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;

        private TourProperties myTourProperties;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public MasterSummaryOverallUS() {
            InitializeComponent();
        }

        private void MasterSummaryOverallUS_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.MasterSummaryOverallUS_Width > 0 ) {
                this.Width = Properties.Settings.Default.MasterSummaryOverallUS_Width;
            }
            if ( Properties.Settings.Default.MasterSummaryOverallUS_Height > 0 ) {
                this.Height = Properties.Settings.Default.MasterSummaryOverallUS_Height;
            }
            if ( Properties.Settings.Default.MasterSummaryOverallUS_Location.X > 0
                && Properties.Settings.Default.MasterSummaryOverallUS_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.MasterSummaryOverallUS_Location;
            }
            myTourProperties = TourProperties.Instance;

            bestScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "round" )) roundScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallDataType.ToLower().Equals( "final" )) finalScoreButton.Checked = true;

            pointsScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtMethod.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtMethod.ToLower().Equals( "hcap" )) handicapScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtMethod.ToLower().Equals( "ratio" )) ratioScoreButton.Checked = true;

            plcmtDivButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "div" )) plcmtDivButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "divgr" )) plcmtDivGrpButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "tour" )) plcmtTourButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPlcmtOrg.ToLower().Equals( "group" )) groupPlcmtButton.Checked = true;

            nopsPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallPointsMethod.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;

            showAllButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallFilter.ToLower().Equals( "all" )) showAllButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallFilter.ToLower().Equals( "qualifed" )) showQlfyButton.Checked = true;
            if (myTourProperties.MasterSummaryOverallFilter.ToLower().Equals( "eligible" )) showEligButton.Checked = true;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];

                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            RoundOverall.Visible = false;
                            RoundSlalom.Visible = false;
                            RoundTrick.Visible = false;
                            RoundJump.Visible = false;
                            EventClassSlalom.Visible = false;
                            EventClassTrick.Visible = false;
                            EventClassJump.Visible = false;
                            TeamCodeNcwsa.Visible = true;
                        } else {
                            RoundOverall.Visible = true;
                            RoundSlalom.Visible = true;
                            RoundTrick.Visible = true;
                            RoundJump.Visible = true;
                            EventClassSlalom.Visible = true;
                            EventClassTrick.Visible = true;
                            EventClassJump.Visible = true;
                            TeamCodeNcwsa.Visible = false;
                        }

                        loadEventGroupList();
                    }
                }
            }
        }

        private void MasterSummaryOverallUS_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.MasterSummaryOverallUS_Width = this.Size.Width;
                Properties.Settings.Default.MasterSummaryOverallUS_Height = this.Size.Height;
                Properties.Settings.Default.MasterSummaryOverallUS_Location = this.Location;
            }
        }

        private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e) {
            MessageBox.Show("Binding Exception: " + e.Exception.Message);
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {

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

                if ( bestScoreButton.Checked ) {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                } else if ( roundScoreButton.Checked ) {
                    curDataType = "round";
                    winStatusMsg.Text = curMsg + "- total scores";
                } else if ( finalScoreButton.Checked ) {
                    curDataType = "final";
                    winStatusMsg.Text = curMsg + "- final scores";
                }
                myTourProperties.MasterSummaryOverallDataType = curDataType;

                if ( pointsScoreButton.Checked ) {
                    curPlcmtMethod = "points";
                } else if ( handicapScoreButton.Checked ) {
                    curPlcmtMethod = "hcap";
                } else if ( ratioScoreButton.Checked ) {
                    curPlcmtMethod = "ratio";
                } else {
                    curPlcmtMethod = "points";
                }
                myTourProperties.MasterSummaryOverallPlcmtMethod = curPlcmtMethod;

                EventGroup.Visible = false;
                if ( groupPlcmtButton.Checked ) {
                    curPlcmtOrg = "group";
                    EventGroup.Visible = true;
                } else if ( plcmtTourButton.Checked ) {
                    curPlcmtOrg = "tour";
                } else if ( plcmtDivButton.Checked ) {
                    curPlcmtOrg = "div";
                } else if ( plcmtDivGrpButton.Checked ) {
                    curPlcmtOrg = "divgr";
                    EventGroup.Visible = true;
                } else {
                    curPlcmtOrg = "tour";
                }
                myTourProperties.MasterSummaryOverallPlcmtOrg = curPlcmtOrg;

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
                myTourProperties.MasterSummaryOverallPointsMethod = curPointsMethod;

                String curFilterSetting = "All";
                if ( showAllButton.Checked ) {
                    curFilterSetting = "All";
                } else if ( showQlfyButton.Checked ) {
                    curFilterSetting = "Qualifed";
                } else {
                    curFilterSetting = "All";
                }
                myTourProperties.MasterSummaryOverallFilter = curFilterSetting;

                /*
                if ( EventGroup.Visible ) {
                    SlalomLabel.Location = new Point( 208, SlalomLabel.Location.Y );
                    TrickLabel.Location = new Point( 573, SlalomLabel.Location.Y );
                    JumpLabel.Location = new Point( 830, SlalomLabel.Location.Y );
                    OverallLabel.Location = new Point( 1062, SlalomLabel.Location.Y );
                } else {
                    SlalomLabel.Location = new Point( 168, SlalomLabel.Location.Y );
                    TrickLabel.Location = new Point( 533, SlalomLabel.Location.Y );
                    JumpLabel.Location = new Point( 790, SlalomLabel.Location.Y );
                    OverallLabel.Location = new Point( 1022, SlalomLabel.Location.Y );
                }
                 */

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


                if ( curGroupValue.ToLower().Equals( "all" ) ) {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Overall", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                    } else {
                        mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOrg );
                    }
                } else {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Overall", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                    } else {
                        mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOrg );
                    }
                }
                loadSummaryDataGrid();
            }
            this.Cursor = Cursors.Default;
        }

        private void loadSummaryDataGrid(  ) {
            String curReportGroup = "", prevReportGroup = "";
            String curDiv = "", curGroup = "";
            int curRound = 0, prevRound = 0;
            Decimal curScore = 0, prevScore = 0;
            DataGridViewRow curViewRow;
            scoreSummaryDataGridView.Rows.Clear();

            //mySummaryDataTable.DefaultView.RowFilter = "TeamSlalom = 'E' OR TeamTrick = 'E' OR TeamJump = 'E' ";
            //DataTable curDataTable = mySummaryDataTable.DefaultView.ToTable();

            int curPlcmt = 0;
            int curIdx = 0;
            foreach (DataRow curRow in mySummaryDataTable.Rows) {
                if ( ( ( (String)curRow["EligOverall"] ).Equals( "Yes" ) && showEligButton.Checked )
                    || ( ( (String)curRow["QualifyOverall"] ).Equals( "Yes" ) && ( (String)curRow["EligOverall"] ).Equals( "Yes" )  && showQlfyButton.Checked )
                    || showAllButton.Checked ) {
                    curPlcmt++;
                    curIdx = scoreSummaryDataGridView.Rows.Add();
                    prevReportGroup = curReportGroup;

                    if (roundScoreButton.Checked) {
                        RoundJump.Visible = false;
                        RoundTrick.Visible = false;
                        RoundSlalom.Visible = false;

                        prevRound = curRound;
                        try {
                            curRound = (Int16)curRow["RoundOverall"];
                        } catch (Exception ex) {
                            MessageBox.Show("Exception enocuntered: " + ex.Message);
                            curRound = 0;
                        }
                    } else {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        } else {
                            RoundJump.Visible = true;
                            RoundTrick.Visible = true;
                            RoundSlalom.Visible = true;
                        }
                    }

                    curGroup = (String)curRow["EventGroupOverall"];
                    if ( ( (String)curRow["AgeGroup"] ).Length > 2 ) {
                        curDiv = ( (String)curRow["AgeGroup"] ).Substring( 0, 2 );
                    } else {
                        curDiv = (String)curRow["AgeGroup"];
                    }
                    if ( groupPlcmtButton.Checked ) {
                        curReportGroup = curGroup;
                    } else if ( plcmtTourButton.Checked ) {
                        curReportGroup = "";
                    } else if ( plcmtDivButton.Checked ) {
                        curReportGroup = curDiv;
                    } else if ( plcmtDivGrpButton.Checked ) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else {
                        curReportGroup = curGroup;
                    }
                    if ((curReportGroup != prevReportGroup) && prevReportGroup.Length > 0) {
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                        scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                        curPlcmt = 1;
                        prevScore = 0;
                    } else if (roundScoreButton.Checked) {
                        if (curRound != prevRound) {
                            curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                            curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
                            scoreSummaryDataGridView.Rows[curIdx].Height = 2;
                            curIdx = scoreSummaryDataGridView.Rows.Add();
                            curPlcmt = 1;
                            prevScore = 0;
                        }
                    }

                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                    curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];
                    curViewRow.Cells["AgeGroup"].Value = curDiv;
                    curViewRow.Cells["EventGroup"].Value = curGroup;
                    curViewRow.Cells["QualifyOverall"].Value = (String)curRow["QualifyOverall"];
                    curViewRow.Cells["EligOverall"].Value = (String)curRow["EligOverall"];

                    curViewRow.Cells["SepSlalom"].Value = " ";
                    if ( ( (String)curRow["EventClassSlalom"] ).Length > 0 ) {
                        curViewRow.Cells["EventClassSlalom"].Value = (String)curRow["EventClassSlalom"];
                        curViewRow.Cells["RoundSlalom"].Value = (Int16)curRow["RoundSlalom"];
                        curViewRow.Cells["ScoreSlalom"].Value = (Decimal)curRow["ScoreSlalom"];
                        curViewRow.Cells["PointsSlalom"].Value = (Decimal)curRow["PointsSlalom"];

                        curViewRow.Cells["LastPassBuoys"].Value = ( (Decimal)curRow["FinalPassScore"] ).ToString( "0.00" );
                        curViewRow.Cells["RopeLengthFeetSlalom"].Value = (String)curRow["FinalLenOff"];
                        curViewRow.Cells["RopeLenghtMetricSlalom"].Value = (String)curRow["FinalLen"];
                        curViewRow.Cells["BoatSpeedMphSlalom"].Value = (Byte)curRow["FinalSpeedMph"];
                        curViewRow.Cells["BoatSpeedKphSlalom"].Value = (Byte)curRow["FinalSpeedKph"];
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            curViewRow.Cells["TeamCodeNcwsa"].Value = (String)curRow["TeamSlalom"];
                        }
                    }

                    curViewRow.Cells["SepTrick"].Value = " ";
                    if ( ( (String)curRow["EventClassTrick"] ).Length > 0 ) {
                        curViewRow.Cells["EventClassTrick"].Value = (String)curRow["EventClassTrick"];
                        curViewRow.Cells["RoundTrick"].Value = (Int16)curRow["RoundTrick"];
                        curViewRow.Cells["ScorePass1Trick"].Value = (Int16)curRow["Pass1Trick"];
                        curViewRow.Cells["ScorePass2Trick"].Value = (Int16)curRow["Pass2Trick"];
                        curViewRow.Cells["ScoreTrick"].Value = (Int16)curRow["ScoreTrick"];
                        curViewRow.Cells["PointsTrick"].Value = (Decimal)curRow["PointsTrick"];
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (curViewRow.Cells["TeamCodeNcwsa"].Value == null) {
                                curViewRow.Cells["TeamCodeNcwsa"].Value = (String)curRow["TeamTrick"];
                            }
                        }
                    }

                    curViewRow.Cells["SepJump"].Value = " ";
                    if ( ( (String)curRow["EventClassJump"] ).Length > 0 ) {
                        curViewRow.Cells["EventClassJump"].Value = (String)curRow["EventClassJump"];
                        curViewRow.Cells["RoundJump"].Value = (Int16)curRow["RoundJump"];
                        curViewRow.Cells["ScoreMeters"].Value = (Decimal)curRow["ScoreMeters"];
                        curViewRow.Cells["ScoreFeet"].Value = (Decimal)curRow["ScoreFeet"];
                        curViewRow.Cells["PointsJump"].Value = (Decimal)curRow["PointsJump"];
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (curViewRow.Cells["TeamCodeNcwsa"].Value == null) {
                                curViewRow.Cells["TeamCodeNcwsa"].Value = (String)curRow["TeamJump"];
                            }
                        }
                    }

                    curScore = (Decimal)curRow["ScoreOverall"];
                    curViewRow.Cells["SepOverall"].Value = " ";
                    curViewRow.Cells["ScoreOverall"].Value = curScore;
                    curViewRow.Cells["RoundOverall"].Value = (Int16)curRow["RoundOverall"];
                    if (curScore < prevScore) {
                        curViewRow.Cells["OverallPlcmt"].Value = curPlcmt;
                    } else {
                        if (curPlcmt > 1) {
                            String curValue = "";
                            if (scoreSummaryDataGridView.Rows[curIdx - 1].Cells["OverallPlcmt"].Value == null) {
                                curValue = curPlcmt.ToString();
                            } else {
                                curValue = scoreSummaryDataGridView.Rows[curIdx - 1].Cells["OverallPlcmt"].Value.ToString();
                            }
                            if (curValue.Contains( "T" )) {
                                curViewRow.Cells["OverallPlcmt"].Value = curValue;
                            } else {
                                curValue = (curPlcmt - 1).ToString( "##0" ) + "T";
                                scoreSummaryDataGridView.Rows[curIdx - 1].Cells["OverallPlcmt"].Value = curValue;
                                curViewRow.Cells["OverallPlcmt"].Value = curValue;
                            }
                        } else {
                            curViewRow.Cells["OverallPlcmt"].Value = curPlcmt;
                        }
                    }
                    prevScore = curScore;
                }
            }
            try {
                RowStatusLabel.Text = "Row 1 of " + scoreSummaryDataGridView.Rows.Count.ToString();

                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    int curLocationSlalom = scoreSummaryDataGridView.GetColumnDisplayRectangle( 9, true ).Location.X;
                    SlalomLabel.Location = new Point( curLocationSlalom, SlalomLabel.Location.Y );
                    int curLocationTrick = scoreSummaryDataGridView.GetColumnDisplayRectangle( 18, true ).Location.X;
                    TrickLabel.Location = new Point( curLocationTrick, TrickLabel.Location.Y );
                    int curLocationJump = scoreSummaryDataGridView.GetColumnDisplayRectangle( 24, true ).Location.X;
                    JumpLabel.Location = new Point( curLocationJump, JumpLabel.Location.Y );
                    int curLocationOverall = scoreSummaryDataGridView.GetColumnDisplayRectangle( 30, true ).Location.X;
                    OverallLabel.Location = new Point( curLocationOverall, OverallLabel.Location.Y );
                } else {
                    int curLocationSlalom = scoreSummaryDataGridView.GetColumnDisplayRectangle( 8, true ).Location.X;
                    SlalomLabel.Location = new Point( curLocationSlalom, SlalomLabel.Location.Y );
                    int curLocationTrick = scoreSummaryDataGridView.GetColumnDisplayRectangle( 17, true ).Location.X;
                    TrickLabel.Location = new Point( curLocationTrick, TrickLabel.Location.Y );
                    int curLocationJump = scoreSummaryDataGridView.GetColumnDisplayRectangle( 23, true ).Location.X;
                    JumpLabel.Location = new Point( curLocationJump, JumpLabel.Location.Y );
                    int curLocationOverall = scoreSummaryDataGridView.GetColumnDisplayRectangle( 30, true ).Location.X;
                    OverallLabel.Location = new Point( curLocationOverall, OverallLabel.Location.Y );
                }


            } catch {
                RowStatusLabel.Text = "";
            }
        }

        private void loadEventGroupList() {
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
                + "WHERE SanctionId = '" + mySanctionNum + "' "
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

        private void overallFilter_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                String curFilterSetting = "All";
                String curName = ( (RadioButton)sender ).Name;
                if (curName.Equals( "showQlfyButton" )) {
                    curFilterSetting = "Qualifed";
                } else if (curName.Equals( "showEligButton" )) {
                    curFilterSetting = "Eligible";
                } else if (curName.Equals( "showAllButton" )) {
                    curFilterSetting = "All";
                }
                myTourProperties.MasterSummaryOverallFilter = curFilterSetting;
                if (mySummaryDataTable != null) {
                    if (mySummaryDataTable.Rows.Count > 0) {
                        loadSummaryDataGrid();
                    }
                }
            }
        }

        private void scoreSummaryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + scoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void navExport_Click( object sender, EventArgs e ) {
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

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            Font saveShowDefaultCellStyle = scoreSummaryDataGridView.DefaultCellStyle.Font;
            scoreSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 10, FontStyle.Regular );

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter( scoreSummaryDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDataGrid.SubtitleList();
                StringRowPrinter mySubtitle;
                StringFormat SubtitleStringFormat = new StringFormat();
                SubtitleStringFormat.Trimming = StringTrimming.Word;
                SubtitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                SubtitleStringFormat.Alignment = StringAlignment.Center;

                int curLabelLocSlalom = 0, curLabelLocTrick = 0, curLabelLocJump = 0, curLabelLocOverall = 0;
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    curLabelLocSlalom = 295;
                    curLabelLocTrick = 572;
                    curLabelLocJump = 737;
                    curLabelLocOverall = 887;
                } else {
                    if (EventGroup.Visible) {
                        curLabelLocSlalom = 244;
                        curLabelLocTrick = 550;
                        curLabelLocJump = 729;
                        curLabelLocOverall = 897;
                    } else {
                        curLabelLocSlalom = 204;
                        curLabelLocTrick = 517;
                        curLabelLocJump = 697;
                        curLabelLocOverall = 862;
                    }
                }

                mySubtitle = new StringRowPrinter( SlalomLabel.Text,
                    curLabelLocSlalom, 0, Convert.ToInt32( SlalomLabel.Width * .8 ), SlalomLabel.Size.Height,
                    SlalomLabel.ForeColor, SlalomLabel.BackColor, SlalomLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( TrickLabel.Text,
                    curLabelLocTrick, 0, Convert.ToInt32( TrickLabel.Width * .65 ), TrickLabel.Size.Height,
                    TrickLabel.ForeColor, TrickLabel.BackColor, TrickLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( JumpLabel.Text,
                    curLabelLocJump, 0, Convert.ToInt32( JumpLabel.Width * .65 ), JumpLabel.Size.Height,
                    JumpLabel.ForeColor, JumpLabel.BackColor, JumpLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( OverallLabel.Text,
                    curLabelLocOverall, 0, OverallLabel.Size.Width, OverallLabel.Size.Height,
                    OverallLabel.ForeColor, OverallLabel.BackColor, OverallLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }

            scoreSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getSlalomScoreDetailData() {
            return getSlalomScoreDetailData( "All" );
        }
        private DataTable getSlalomScoreDetailData( String inDiv ) {
            String curEventGroup = "";
            if ( inDiv == null ) {
                curEventGroup = "";
            } else if ( inDiv.Length > 0 ) {
                if ( inDiv.ToLower().Equals( "all" ) ) {
                    curEventGroup = "";
                } else {
                    curEventGroup = inDiv;
                }
            } else {
                curEventGroup = "";
            }
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.MemberId, T.SanctionId, T.SkierName, T.AgeGroup, S.Round AS SlalomRound, S.Score AS SlalomScore, S.NopsScore AS SlalomNopsScore, " );
            curSqlStmt.Append( "S.Rating AS SlalomRating, S.MaxSpeed, S.StartSpeed, S.StartLen, R.PassNum, R.Score AS PassScore, C.MinValue AS LineLength, " );
            curSqlStmt.Append( "C.MaxValue AS LastSpeed, C.CodeValue AS LastPassInfo, RS.TeamCode AS SlalomTeam, RS.EventGroup AS SlalomGroup, RS.EventClass AS SlalomClass " );
            curSqlStmt.Append( "FROM TourReg AS T " );
            curSqlStmt.Append( "	INNER JOIN EventReg AS RS ON T.SanctionId = RS.SanctionId AND T.MemberId = RS.MemberId AND T.AgeGroup = RS.AgeGroup AND RS.Event = 'Slalom' " );
            curSqlStmt.Append( "	INNER JOIN SlalomScore AS S ON T.SanctionId = S.SanctionId AND T.MemberId = S.MemberId AND T.AgeGroup = S.AgeGroup " );
            curSqlStmt.Append( "	INNER JOIN SlalomRecap AS R ON T.SanctionId = R.SanctionId AND T.MemberId = R.MemberId AND T.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
            curSqlStmt.Append( " 	INNER JOIN CodeValueList AS C ON C.ListCodeNum = R.PassNum AND 'SlalomPass' + CONVERT(nchar(2), S.MaxSpeed) = C.ListName " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            if ( curEventGroup.Length > 0 ) {
                curSqlStmt.Append( "  And T.AgeGroup = '" + curEventGroup + "' " );
            }
            curSqlStmt.Append( "ORDER BY T.SanctionId, T.AgeGroup, T.SkierName, T.MemberId, S.Round, R.PassNum DESC" );
            return getData( curSqlStmt.ToString() );

            //curSqlStmt.Append( "AND R.PassNum IN " );
            //curSqlStmt.Append( "	(SELECT MAX(PassNum) AS LastPass FROM SlalomRecap AS R2 " );
            //curSqlStmt.Append( "	WHERE SanctionId = R.SanctionId AND MemberId = R.MemberId AND R.AgeGroup = AgeGroup AND R.Round = Round) " );
            //curSqlStmt.Append( "ORDER BY SlalomGroup, T.SkierName, T.MemberId, SlalomRound " );
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
