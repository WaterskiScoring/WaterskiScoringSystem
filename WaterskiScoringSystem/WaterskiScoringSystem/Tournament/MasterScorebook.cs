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
    public partial class MasterScorebook : Form {
        private Int16 myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private DataRow myTourRow;
        private DataTable mySummaryDataTable;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;

        private DataTable myMemberData;
        private DataTable mySlalomDetail;
        private DataTable myTrickDetail;
        private DataTable myJumpDetail;

        private TourProperties myTourProperties;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public MasterScorebook() {
            InitializeComponent();
        }

        private void MasterSummaryV2_Load( object sender, EventArgs e ) {

            if (Properties.Settings.Default.MasterSummaryV2_Width > 0) {
                this.Width = Properties.Settings.Default.MasterSummaryV2_Width;
            }
            if ( Properties.Settings.Default.MasterSummaryV2_Height > 0 ) {
                this.Height = Properties.Settings.Default.MasterSummaryV2_Height;
            }
            if ( Properties.Settings.Default.MasterSummaryV2_Location.X > 0
                && Properties.Settings.Default.MasterSummaryV2_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.MasterSummaryV2_Location;
            }
            myTourProperties = TourProperties.Instance;

            bestScoreButton.Checked = true;
            if (myTourProperties.MasterSummaryDataType.ToLower().Equals( "total" )) finalScoreButton.Checked = true;
            if ( myTourProperties.MasterSummaryDataType.ToLower().Equals( "final" ) ) finalScoreButton.Checked = true;
            if ( myTourProperties.MasterSummaryDataType.ToLower().Equals( "first" ) ) firstScoreButton.Checked = true;

            nopsPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryPointsMethod.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryPointsMethod.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
            if (myTourProperties.MasterSummaryPointsMethod.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
            if (myTourProperties.MasterSummaryPointsMethod.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;

            plcmtDivButton.Checked = true;
            if ( myTourProperties.MasterSummaryPlcmtOrg.ToLower().Equals( "div" ) ) plcmtDivButton.Checked = true;
            if ( myTourProperties.MasterSummaryPlcmtOrg.ToLower().Equals( "divgr" ) ) plcmtDivGrpButton.Checked = true;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData( mySanctionNum );
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        loadEventGroupList();
                    }
                }
            }
        }

        private void MasterSummaryV2_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.MasterSummaryV2_Width = this.Size.Width;
                Properties.Settings.Default.MasterSummaryV2_Height = this.Size.Height;
                Properties.Settings.Default.MasterSummaryV2_Location = this.Location;
            }
        }

        private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e) {
            MessageBox.Show("Binding Exception: " + e.Exception.Message);
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {

        }

        public void navRefresh_Click( object sender, EventArgs e ) {
            if ( mySanctionNum != null && myTourRow != null ) {
                String curDataType = "best", curPlcmtMethod = "score", curPointsMethod = "";
                String curPlcmtOrg = "", curPlcmtOverallOrg = "";
                //String curPlcmtOrg = "div";

                // Retrieve data from database depending on selection criteria
                String curMsg = "Tournament scores retrieved ";

                if ( bestScoreButton.Checked ) {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                } else if ( finalScoreButton.Checked ) {
                    curDataType = "final";
                    winStatusMsg.Text = curMsg + "- final scores";
                } else if ( firstScoreButton.Checked ) {
                    curDataType = "first";
                    winStatusMsg.Text = curMsg + "- first scores";
                }
                myTourProperties.MasterSummaryDataType = curDataType;
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
                myTourProperties.MasterSummaryPointsMethod = curPointsMethod;

                if ( plcmtDivButton.Checked ) {
                    curPlcmtOrg = "div";
                    curPlcmtOverallOrg = "agegroup";
                    EventGroup.Visible = false;
                } else if ( plcmtDivGrpButton.Checked ) {
                    curPlcmtOrg = "divgr";
                    curPlcmtOverallOrg = "agegroupgroup";
                    EventGroup.Visible = true;
                } else {
                    curPlcmtOrg = "div";
                    curPlcmtOverallOrg = "agegroup";
                    EventGroup.Visible = false;
                }
                myTourProperties.MasterSummaryPlcmtOrg = curPlcmtOrg;

                if ( EventGroup.Visible ) {
                    SlalomLabel.Location = new Point( 202, SlalomLabel.Location.Y );
                    TrickLabel.Location = new Point( 572, SlalomLabel.Location.Y );
                    JumpLabel.Location = new Point( 832, SlalomLabel.Location.Y );
                    OverallLabel.Location = new Point( 1083, SlalomLabel.Location.Y );
                } else {
                    SlalomLabel.Location = new Point( 162, SlalomLabel.Location.Y );
                    TrickLabel.Location = new Point( 532, SlalomLabel.Location.Y );
                    JumpLabel.Location = new Point( 792, SlalomLabel.Location.Y );
                    OverallLabel.Location = new Point( 1043, SlalomLabel.Location.Y );
                }

                Cursor.Current = Cursors.WaitCursor;
                scoreSummaryDataGridView.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    winStatusMsg.Text = "Tournament entries retrieved";
                } );
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

                CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                if ( curGroupValue.ToLower().Equals( "all" ) ) {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum );
                        mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                    } else {
                        mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOverallOrg );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum );
                        mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                    }
                } else {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum, curGroupValue );
                        mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                    } else {
                        mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOverallOrg );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum, curGroupValue );
                        mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                    }
                }

                if (plcmtDivGrpButton.Checked) {
                    mySummaryDataTable.DefaultView.Sort = "AgeGroup, EventGroup, SkierName, RoundOverall";
                    mySummaryDataTable = mySummaryDataTable.DefaultView.ToTable();
                }

                loadSummaryDataGrid();
                Cursor.Current = Cursors.Default;
            }
        }

        private void loadSummaryDataGrid() {
            DataGridViewRow curViewRow;
            String curMemberId = "", curAgeGroup = "";
            Decimal curRampHeight;
            Int16 curRound = 0;
            scoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0, curSmryIdx = 0;
            foreach ( DataRow curRow in mySummaryDataTable.Rows ) {
                curMemberId = (String)curRow["MemberId"];
                curAgeGroup = (String)curRow["AgeGroup"];
                curRound = (Int16)curRow["RoundOverall"];
                if ( curRound == 1 ) {
                    curIdx = scoreSummaryDataGridView.Rows.Add();
                    if (curIdx > 0) {
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                        scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                    }
                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                    curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];
                } else {
                    curIdx = scoreSummaryDataGridView.Rows.Add();
                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                    curViewRow.Cells["SkierName"].Value = "";
                }
                curSmryIdx = curIdx - curRound + 1;
                if (curSmryIdx < 0) {
                    curSmryIdx = 0;
                }

                if ( ( (String)curRow["EventClassSlalom"] ).Length > 0
                    || ( (String)curRow["EventClassTrick"] ).Length > 0
                    || ( (String)curRow["EventClassJump"] ).Length > 0
                    ) {
                    curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
                    if ( EventGroup.Visible ) {
                        curViewRow.Cells["EventGroup"].Value = (String)curRow["EventGroupOverall"];
                    }

                    curViewRow.Cells["SepSlalom"].Value = " ";
                    if ( ( (String)curRow["EventClassSlalom"] ).Length > 0 ) {
                        curViewRow.Cells["EventClassSlalom"].Value = (String)curRow["EventClassSlalom"];
                        curViewRow.Cells["RoundSlalom"].Value = (Int16)curRow["RoundSlalom"];
                        curViewRow.Cells["ScoreSlalom"].Value = (Decimal)curRow["ScoreSlalom"];
                        curViewRow.Cells["PointsSlalom"].Value = (Decimal)curRow["PointsSlalom"];
                        curViewRow.Cells["PlcmtSlalom"].Value = "";
                        if ( curRound == 1 ) {
                            if ( ( (String) curRow["PlcmtSlalom"] ).Trim().Equals("999") ) {
                                curViewRow.Cells["PlcmtSlalom"].Value = "**";
                            } else {
                                curViewRow.Cells["PlcmtSlalom"].Value = (String) curRow["PlcmtSlalom"];
                            }
                        } else {
                            if (scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value == null) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassSlalom"].Value = (String)curRow["EventClassSlalom"];
                                if ( ( (String) curRow["PlcmtSlalom"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value = (String) curRow["PlcmtSlalom"];
                                }
                            } else if ( scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value.ToString().Length == 0 ) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassSlalom"].Value = (String) curRow["EventClassSlalom"];
                                if ( ( (String) curRow["PlcmtSlalom"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtSlalom"].Value = (String) curRow["PlcmtSlalom"];
                                }
                            }
                        }
                        try {
                            curViewRow.Cells["LastPassBuoys"].Value = ( (Decimal)curRow["FinalPassScore"] ).ToString( "0.00" );
                        } catch {
                            curViewRow.Cells["LastPassBuoys"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RopeLengthFeetSlalom"].Value = (String)curRow["FinalLenOff"];
                        } catch {
                            curViewRow.Cells["RopeLengthFeetSlalom"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RopeLenghtMetricSlalom"].Value = (String)curRow["FinalLen"];
                        } catch {
                            curViewRow.Cells["RopeLenghtMetricSlalom"].Value = "";
                        }
                        try {
                            curViewRow.Cells["BoatSpeedMphSlalom"].Value = ( (Byte)curRow["FinalSpeedMph"] ).ToString();
                        } catch {
                            curViewRow.Cells["BoatSpeedMphSlalom"].Value = "";
                        }
                        try {
                            curViewRow.Cells["BoatSpeedKphSlalom"].Value = ( (Byte)curRow["FinalSpeedKph"] ).ToString();
                        } catch {
                            curViewRow.Cells["BoatSpeedKphSlalom"].Value = "";
                        }
                    }

                    curViewRow.Cells["SepTrick"].Value = " ";
                    if ( ( (String)curRow["EventClassTrick"] ).Length > 0 ) {
                        curViewRow.Cells["EventClassTrick"].Value = (String)curRow["EventClassTrick"];
                        curViewRow.Cells["RoundTrick"].Value = (Int16)curRow["RoundTrick"];
                        curViewRow.Cells["Pass1Trick"].Value = (Int16)curRow["Pass1Trick"];
                        curViewRow.Cells["Pass2Trick"].Value = (Int16)curRow["Pass2Trick"];
                        curViewRow.Cells["ScoreTrick"].Value = (Int16)curRow["ScoreTrick"];
                        curViewRow.Cells["PointsTrick"].Value = (Decimal)curRow["PointsTrick"];
                        if ( curRound == 1 ) {
                            if ( ( (String) curRow["PlcmtTrick"] ).Trim().Equals("999") ) {
                                curViewRow.Cells["PlcmtTrick"].Value = "**";
                            } else {
                                curViewRow.Cells["PlcmtTrick"].Value = (String) curRow["PlcmtTrick"];
                            }
                        } else {
                            if ( scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value == null ) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassTrick"].Value = (String) curRow["EventClassTrick"];
                                if ( ( (String) curRow["PlcmtTrick"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = (String) curRow["PlcmtTrick"];
                                }
                            } else if ( scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value.ToString().Length == 0 ) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassTrick"].Value = (String) curRow["EventClassTrick"];
                                if ( ( (String) curRow["PlcmtTrick"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = (String) curRow["PlcmtTrick"];
                                }
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

                        try {
                            curRampHeight = (Decimal)curRow["RampHeight"];
                            if ( curRampHeight == 5 ) {
                                curViewRow.Cells["RampHeight"].Value = "5";
                            } else if ( curRampHeight == 5.5M ) {
                                curViewRow.Cells["RampHeight"].Value = "H";
                            } else if ( curRampHeight == 6 ) {
                                curViewRow.Cells["RampHeight"].Value = "6";
                            } else if ( curRampHeight < 5 ) {
                                curViewRow.Cells["RampHeight"].Value = "4";
                            } else {
                                curViewRow.Cells["RampHeight"].Value = "";
                            }
                        } catch {
                            curViewRow.Cells["RampHeight"].Value = "";
                        }
                        try {
                            curViewRow.Cells["SpeedKphJump"].Value = ( (Byte)curRow["SpeedKphJump"] ).ToString( "00" );
                        } catch {
                            curViewRow.Cells["SpeedKphJump"].Value = "";
                        }

                        if ( curRound == 1 ) {
                            if ( ( (String) curRow["PlcmtJump"] ).Trim().Equals("999") ) {
                                curViewRow.Cells["PlcmtJump"].Value = "**";
                            } else {
                                curViewRow.Cells["PlcmtJump"].Value = (String) curRow["PlcmtJump"];
                            }
                        } else {
                            if ( scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value == null ) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassJump"].Value = (String) curRow["EventClassJump"];
                                if ( ( (String) curRow["PlcmtJump"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value = (String) curRow["PlcmtJump"];
                                }
                            } else if ( scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value.ToString().Length == 0 ) {
                                scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassJump"].Value = (String) curRow["EventClassJump"];
                                if ( ( (String) curRow["PlcmtJump"] ).Trim().Equals("999") ) {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value = "**";
                                } else {
                                    scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtJump"].Value = (String) curRow["PlcmtJump"];
                                }
                            }
                        }
                    }

                    curViewRow.Cells["SepOverall"].Value = " ";
                    curViewRow.Cells["QualifyOverall"].Value = (String)curRow["QualifyOverall"];
                    curViewRow.Cells["EligOverall"].Value = (String)curRow["EligOverall"];
                    curViewRow.Cells["RoundOverall"].Value = (Int16)curRow["RoundOverall"];
                    if ( ( (String)curRow["EligOverall"] ).ToLower().Equals( "yes" ) ) {
                        curViewRow.Cells["ScoreOverall"].Value = (Decimal)curRow["ScoreOverall"];
                    }
                }
            }

            try {
                RowStatusLabel.Text = "Row 1 of " + scoreSummaryDataGridView.Rows.Count.ToString();
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

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( scoreSummaryDataGridView );
        }

        private void scoreSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
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
            if ( printReport( true ) ) ExportLiveWeb.uploadReportFile( "Results", "Overall", mySanctionNum );
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

            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;

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

            if ( plcmtDivButton.Checked ) {
                mySubtitle = new StringRowPrinter( SlalomLabel.Text,
                    140, 0, 302, SlalomLabel.Size.Height,
                    SlalomLabel.ForeColor, SlalomLabel.BackColor, SlalomLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( TrickLabel.Text,
                    451, 0, 215, TrickLabel.Size.Height,
                    TrickLabel.ForeColor, TrickLabel.BackColor, TrickLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( JumpLabel.Text,
                    676, 0, 195, JumpLabel.Size.Height,
                    JumpLabel.ForeColor, JumpLabel.BackColor, JumpLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( OverallLabel.Text,
                    885, 0, 83, OverallLabel.Size.Height,
                    OverallLabel.ForeColor, OverallLabel.BackColor, OverallLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;

            } else {
                mySubtitle = new StringRowPrinter( SlalomLabel.Text,
                    175, 0, 302, SlalomLabel.Size.Height,
                    SlalomLabel.ForeColor, SlalomLabel.BackColor, SlalomLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( TrickLabel.Text,
                    486, 0, 215, TrickLabel.Size.Height,
                    TrickLabel.ForeColor, TrickLabel.BackColor, TrickLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( JumpLabel.Text,
                    711, 0, 195, JumpLabel.Size.Height,
                    JumpLabel.ForeColor, JumpLabel.BackColor, JumpLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( OverallLabel.Text,
                    920, 0, 83, OverallLabel.Size.Height,
                    OverallLabel.ForeColor, OverallLabel.BackColor, OverallLabel.Font, SubtitleStringFormat );
                myPrintDataGrid.SubtitleRow = mySubtitle;
            }

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.DefaultPageSettings.Landscape = true;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

            curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();
            return true;

        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
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
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            return curDataTable;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }
        
    }
}
