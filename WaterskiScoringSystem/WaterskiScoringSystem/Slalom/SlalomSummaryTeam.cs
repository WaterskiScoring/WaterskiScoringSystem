﻿using System;
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
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Slalom {
    public partial class SlalomSummaryTeam : Form {
        private bool myShowTeam = false;
        private String myPageBreak = "\\n";
        private String myEvent = "Slalom";
        private String myTourRules = "";
        private Int16 myNumPerTeam;
        private String mySanctionNum = null;
        private DataRow myTourRow;
        private DataTable myScoreDataTable;
        private DataTable myTeamDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private TourProperties myTourProperties;

        public SlalomSummaryTeam() {
            InitializeComponent();
        }

        private void SlalomSummaryTeam_Load( object sender, EventArgs e ) {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if (Properties.Settings.Default.SlalomTeamSummary_Width > 0) {
                this.Width = Properties.Settings.Default.SlalomTeamSummary_Width;
            }
            if ( Properties.Settings.Default.SlalomTeamSummary_Height > 0 ) {
                this.Height = Properties.Settings.Default.SlalomTeamSummary_Height;
            }
            if ( Properties.Settings.Default.SlalomTeamSummary_Location.X > 0
                && Properties.Settings.Default.SlalomTeamSummary_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.SlalomTeamSummary_Location;
            }
            myTourProperties = TourProperties.Instance;
            bestScoreButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryDataType.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryDataType.ToLower().Equals( "total" ) ) totalScoreButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryDataType.ToLower().Equals( "final" ) ) finalScoreButton.Checked = true;
            rawScoreButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtMethod.ToLower().Equals( "score" )) rawScoreButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtMethod.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;
            plcmtTourButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtOrg.ToLower().Equals( "tour" )) plcmtTourButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtOrg.ToLower().Equals( "div" )) divPlcmtButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtOrg.ToLower().Equals( "group" )) groupPlcmtButton.Checked = true;
            plcmtPointsButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryPointsMethod.ToLower().Equals( "nops" ) ) nopsPointsButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryPointsMethod.ToLower().Equals( "plcmt" ) ) plcmtPointsButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryPointsMethod.ToLower().Equals( "kbase" ) ) kBasePointsButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryPointsMethod.ToLower().Equals( "hcap" ) ) handicapPointsButton.Checked = true;
            if ( myTourProperties.SlalomTeamSummaryPointsMethod.ToLower().Equals( "ratio" ) ) ratioPointsButton.Checked = true;

            // Retrieve data from database
            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    myNumPerTeam = Convert.ToInt16(myTourProperties.SlalomTeamSummary_NumPerTeam);
                    NumPerTeamTextBox.Text = myNumPerTeam.ToString();

                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            Round.Visible = false;
                            EventClass.Visible = false;
                        }
                    }
                    loadGroupList();
                }
            }
        }

        private void SlalomSummaryTeam_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.SlalomTeamSummary_Width = this.Size.Width;
                Properties.Settings.Default.SlalomTeamSummary_Height = this.Size.Height;
                Properties.Settings.Default.SlalomTeamSummary_Location = this.Location;
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
            if ( mySanctionNum != null && myEvent != null ) {
                String curDataType = "all", curPlcmtMethod = "score";
                String curPlcmtOrg, curPointsMethod;
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
                }
                myTourProperties.SlalomTeamSummaryDataType = curDataType;

                if ( rawScoreButton.Checked ) {
                    curPlcmtMethod = "score";
                } else if ( pointsScoreButton.Checked ) {
                    curPlcmtMethod = "points";
                } else {
                    curPlcmtMethod = "score";
                }
                myTourProperties.SlalomTeamSummaryPlcmtMethod = curPlcmtMethod;

                if ( divPlcmtButton.Checked ) {
                    curPlcmtOrg = "div";
                } else if (groupPlcmtButton.Checked) {
                    curPlcmtOrg = "group";
                } else if ( plcmtTourButton.Checked ) {
                    curPlcmtOrg = "tour";
                } else {
                    curPlcmtOrg = "tour";
                }
                
                myTourProperties.SlalomTeamSummaryPlcmtOrg = curPlcmtOrg;

                HCapBase.Visible = false;
                HCapScore.Visible = false;
                if ( nopsPointsButton.Checked ) {
                    curPointsMethod = "nops";
                } else if ( plcmtPointsButton.Checked ) {
                    curPointsMethod = "plcmt";
                    curPlcmtMethod = "score";
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
                myTourProperties.SlalomTeamSummaryPointsMethod = curPointsMethod;

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

                if (myTourRules.ToLower().Equals( "iwwf" )) {
                    curPointsMethod = "kbase";
                    myScoreDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadScoreDataGrid( myScoreDataTable );
                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( myScoreDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary( myTourRow, myScoreDataTable, null, null, myNumPerTeam );
                    }
                } else if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    if (plcmtTourButton.Checked) {
                        curPlcmtOrg = "div";
                    }
                    myScoreDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadScoreDataGrid( myScoreDataTable );
                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( myScoreDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamEventCombinedNcwsaSummary( myTeamDataTable, mySanctionNum );
                    }
                } else {
                    myScoreDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadScoreDataGrid( myScoreDataTable );
                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( myScoreDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary( myTourRow, myScoreDataTable, null, null, myNumPerTeam );
                    }
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

        private void loadTeamDataGrid( DataTable inDataTable ) {
            String curGroup = "", prevGroup = "";
            DataGridViewRow curViewRow;

            teamSummaryDataGridView.Rows.Clear();
            int curIdx = 0;
            foreach (DataRow curRow in inDataTable.Rows) {
                curIdx = teamSummaryDataGridView.Rows.Add();
                curViewRow = teamSummaryDataGridView.Rows[curIdx];

                if (TeamDiv.Visible) {
                    try {
                        curGroup = (String)curRow["Div"];
                    } catch {
                        curGroup = "";
                    }
                }
                if (curGroup != prevGroup && curIdx > 0) {
                    curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                    teamSummaryDataGridView.Rows[curIdx].Height = 4;

                    curIdx = teamSummaryDataGridView.Rows.Add();
                    curViewRow = teamSummaryDataGridView.Rows[curIdx];
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
                    curViewRow.Cells["TeamScore"].Value = (Decimal)curRow["TeamScoreSlalom"];
                } catch {
                    curViewRow.Cells["TeamScore"].Value = "";
                }
                try {
                    curViewRow.Cells["TeamPlcmtSlalom"].Value = (String)curRow["PlcmtSlalom"];
                } catch {
                    curViewRow.Cells["TeamPlcmtSlalom"].Value = "";
                }
                prevGroup = curGroup;
            }
            loadPrintDataGrid();
        }

        private void loadScoreDataGrid( DataTable inDataTable ) {
            bool curShowTeamSav = myShowTeam;
            String curGroup = "", prevGroup = "";
            myShowTeam = false;
            DataGridViewRow curViewRow;
            scoreSummaryDataGridView.Rows.Clear();

            String curSortCmd = "";
            if (TeamDiv.Visible) {
                if (divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderSlalom ASC, AgeGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamSlalom ASC, SkierName ASC";
            }
            inDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inDataTable.DefaultView.ToTable();

            int curIdx = 0;
            foreach (DataRow curRow in curDataTable.Rows) {
                curIdx = scoreSummaryDataGridView.Rows.Add();
                curViewRow = scoreSummaryDataGridView.Rows[curIdx];

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
                    scoreSummaryDataGridView.Rows[curIdx].Height = 4;

                    curIdx = scoreSummaryDataGridView.Rows.Add();
                    curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                }

                curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                curViewRow.Cells["Event"].Value = "Slalom";
                curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];
                curViewRow.Cells["EventGroup"].Value = (String)curRow["EventGroupSlalom"];
                curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
                curViewRow.Cells["EventClass"].Value = (String)curRow["EventClassSlalom"];
                curViewRow.Cells["PlcmtEvent"].Value = (String)curRow["PlcmtSlalom"];
                try {
                    curViewRow.Cells["Score"].Value = (Decimal)curRow["ScoreSlalom"];
                } catch {
                    curViewRow.Cells["Score"].Value = 0;
                }
                try {
                    curViewRow.Cells["NopsScore"].Value = (Decimal)curRow["PointsSlalom"];
                } catch {
                    curViewRow.Cells["NopsScore"].Value = 0;
                }
                try {
                    curViewRow.Cells["SkierTeamCode"].Value = (String)curRow["TeamSlalom"];
                } catch {
                    curViewRow.Cells["SkierTeamCode"].Value = "";
                }
                try {
                    curViewRow.Cells["Round"].Value = (Int16)curRow["RoundSlalom"];
                } catch {
                    curViewRow.Cells["Round"].Value = 0;
                }
                try {
                    curViewRow.Cells["HCapBase"].Value = (Decimal)curRow["HCapBase"];
                } catch {
                    curViewRow.Cells["HCapBase"].Value = 0;
                }
                try {
                    curViewRow.Cells["HCapScore"].Value = (Decimal)curRow["HCapScore"];
                } catch {
                    curViewRow.Cells["HCapScore"].Value = 0;
                }
                prevGroup = curGroup;
            }
            myShowTeam = curShowTeamSav;
        }

        private void loadPrintDataGrid() {
            if ( myShowTeam ) ShowAllButton_Click( null, null );

            DataRow[] curTeamSkierList;
            String curFilterCmd = "", curDiv = "", prevDiv = "";
            String curSortCmd = "TeamSlalom ASC";
            if (TeamDiv.Visible) {
                if (divPlcmtButton.Checked) {
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
            curSortCmd += ", SkierName ASC, RoundSlalom ASC";
            myScoreDataTable.DefaultView.Sort = curSortCmd;
            DataTable curScoreDataTable = myScoreDataTable.DefaultView.ToTable();

            //Load data for print data grid
            int curPrintIdx = 0, curSkierIdx = 0;
            DataGridViewRow curPrintRow = null;
            PrintDataGridView.Rows.Clear();
            foreach ( DataRow curTeamRow in myTeamDataTable.Rows ) {
                if (TeamDiv.Visible) {
                    curDiv = (String)curTeamRow["Div"];
                    if (curDiv != prevDiv) loadPrintDivTeamData(curTeamRow, curPrintIdx, curDiv, prevDiv);

                    curPrintIdx = PrintDataGridView.Rows.Add();
                    curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                    curPrintRow.Cells["PrintTeamPlcmt"].Value = (String)curTeamRow["PlcmtSlalom"];
                    curPrintRow.Cells["PrintTeamDiv"].Value = (String)curTeamRow["Div"];

                } else {
                    if (curPrintIdx == 0) loadPrintTeamData();
                    
                    curPrintIdx = PrintDataGridView.Rows.Add();
                    curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                    curPrintRow.Cells["PrintTeamPlcmt"].Value = (String)curTeamRow["PlcmtSlalom"];
                }
                
                try {
                    curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow["TeamCode"] + "-" + (String)curTeamRow["Name"];
                } catch {
                    try {
                        curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow["TeamCode"];
                    } catch {
                        curPrintRow.Cells["PrintTeam"].Value = "";
                    }
                }
                
                try {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = ((Decimal)curTeamRow["TeamScoreSlalom"]).ToString( "###0" );
                } catch {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = "";
                }

                curFilterCmd = "TeamSlalom='" + (String)curTeamRow["TeamCode"] + "'";
                if ( TeamDiv.Visible ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd += " AND AgeGroup = '" + (String)curTeamRow["Div"] + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd += " AND EventGroup = '" + (String)curTeamRow["Div"] + "'";
                    }
                }
                curTeamSkierList = curScoreDataTable.Select( curFilterCmd );

                for ( curSkierIdx = 0; curSkierIdx < curTeamSkierList.Length; curSkierIdx++ ) {
                    if (curSkierIdx > 0) {
                        curPrintIdx = PrintDataGridView.Rows.Add();
                        curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                    }

                    curPrintRow.Cells["PrintSkierNameSlalom"].Value = (String)curTeamSkierList[curSkierIdx]["SkierName"];
                    curPrintRow.Cells["PrintSkierPointsSlalom"].Value = ((Decimal)curTeamSkierList[curSkierIdx]["PointsSlalom"]).ToString("###0");
                    curPrintRow.Cells["PrintSkierScoreSlalom"].Value = ((Decimal)curTeamSkierList[curSkierIdx]["ScoreSlalom"]).ToString("##0.00");

                    if ((curSkierIdx >= myNumPerTeam) && myNumPerTeam > 0) {
                        curPrintRow.Cells["PrintSkierPlcmtSlalom"].Value = "**";
                    } else {
                        curPrintRow.Cells["PrintSkierPlcmtSlalom"].Value = (String)curTeamSkierList[curSkierIdx]["PlcmtSlalom"];
                    }

                    try {
                        curPrintRow.Cells["PrintFinalSpeedMph"].Value = (Byte)curTeamSkierList[curSkierIdx]["FinalSpeedMph"];
                    } catch {
                        curPrintRow.Cells["PrintFinalSpeedMph"].Value = 0;
                    }
                    try {
                        curPrintRow.Cells["PrintFinalSpeedKph"].Value = (Byte)curTeamSkierList[curSkierIdx]["FinalSpeedKph"];
                    } catch {
                        curPrintRow.Cells["PrintFinalSpeedKph"].Value = 0;
                    }
                    try {
                        curPrintRow.Cells["PrintFinalLen"].Value = (String)curTeamSkierList[curSkierIdx]["FinalLen"];
                    } catch {
                        curPrintRow.Cells["PrintFinalLen"].Value = 0;
                    }
                    try {
                        curPrintRow.Cells["PrintFinalLenOff"].Value = (String)curTeamSkierList[curSkierIdx]["FinalLenOff"];
                    } catch {
                        curPrintRow.Cells["PrintFinalLenOff"].Value = 0;
                    }
                    try {
                        curPrintRow.Cells["PrintFinalPassScore"].Value = (Decimal)curTeamSkierList[curSkierIdx]["FinalPassScore"];
                    } catch {
                        curPrintRow.Cells["PrintFinalPassScore"].Value = 0;
                    }
                }

                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPrintRow.DefaultCellStyle.BackColor = Color.Ivory;
                PrintDataGridView.Rows[curPrintIdx].Height = 8;
                prevDiv = curDiv;
            }
        }

        private void loadPrintTeamData( ) {
            int curPrintIdx = 0;
            DataGridViewRow curPrintRow = null;

            foreach (DataRow curTeamRow2 in myTeamDataTable.Rows) {
                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPrintRow.Cells["PrintTeamPlcmt"].Value = (String)curTeamRow2["PlcmtSlalom"];
                curPrintRow.Cells["PrintTeamDiv"].Value = (String)curTeamRow2["Div"];
                try {
                    curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow2["TeamCode"] + "-" + (String)curTeamRow2["Name"];
                } catch {
                    try {
                        curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow2["TeamCode"];
                    } catch {
                        curPrintRow.Cells["PrintTeam"].Value = "";
                    }
                }
                try {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = ((Decimal)curTeamRow2["TeamScoreSlalom"]).ToString("###0");
                } catch {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = "";
                }
            }
            curPrintIdx = PrintDataGridView.Rows.Add();
            curPrintRow = PrintDataGridView.Rows[curPrintIdx];
            curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
            PrintDataGridView.Rows[curPrintIdx].Height = 4;
            curPrintRow.Cells["PrintTeamPlcmt"].Value = myPageBreak;
        }

        private void loadPrintDivTeamData( DataRow curTeamRow, int curPrintIdx, String curDiv, String prevDiv ) {
            DataRow[] curFindRows;
            DataGridViewRow curPrintRow;
            if (curPrintIdx > 0) {
                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
                PrintDataGridView.Rows[curPrintIdx].Height = 4;
                curPrintRow.Cells["PrintTeamPlcmt"].Value = myPageBreak;
            }

            curFindRows = myTeamDataTable.Select("Div = '" + curDiv + "'");
            foreach (DataRow curTeamRow2 in curFindRows) {
                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPrintRow.Cells["PrintTeamPlcmt"].Value = (String)curTeamRow2["PlcmtSlalom"];
                curPrintRow.Cells["PrintTeamDiv"].Value = (String)curTeamRow2["Div"];
                try {
                    curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow2["TeamCode"] + "-" + (String)curTeamRow2["Name"];
                } catch {
                    try {
                        curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow2["TeamCode"];
                    } catch {
                        curPrintRow.Cells["PrintTeam"].Value = "";
                    }
                }
                try {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = ((Decimal)curTeamRow2["TeamScoreSlalom"]).ToString("###0");
                } catch {
                    curPrintRow.Cells["PrintTeamScoreSlalom"].Value = "";
                }
            }

            curPrintIdx = PrintDataGridView.Rows.Add();
            curPrintRow = PrintDataGridView.Rows[curPrintIdx];
            curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
            curPrintRow.Height = 4;
            curPrintRow.Cells["PrintTeamPlcmt"].Value = myPageBreak;
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

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( PrintDataGridView );
        }

        private void ShowTeamButton_Click( object sender, EventArgs e ) {
            myShowTeam = true;
            String curFilterCommand = "", curTeamCode = "", curGroup = "";
            if ( myScoreDataTable != null ) {
                if ( myScoreDataTable.Rows.Count > 0 ) {
                    if ( teamSummaryDataGridView.CurrentRow == null ) {
                        if ( teamSummaryDataGridView.Rows.Count > 0 ) {
                            curTeamCode = teamSummaryDataGridView.Rows[0].Cells["TeamCode"].Value.ToString();
                            if ( TeamDiv.Visible ) {
                                curGroup = teamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                            }
                        }
                    } else {
                        if ( teamSummaryDataGridView.Rows.Count > 0 ) {
                            curTeamCode = teamSummaryDataGridView.CurrentRow.Cells["TeamCode"].Value.ToString();
                            if ( TeamDiv.Visible ) {
                                curGroup = teamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                            }
                        }
                    }
                    if ( TeamDiv.Visible ) {
                        if (divPlcmtButton.Checked) {
                            curFilterCommand = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                        }
                        if (groupPlcmtButton.Checked) {
                            curFilterCommand = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                        }
                    } else {
                        curFilterCommand = "TeamSlalom = '" + curTeamCode + "'";
                    }
                    myScoreDataTable.DefaultView.RowFilter = curFilterCommand;
                    loadScoreDataGrid( myScoreDataTable.DefaultView.ToTable() );
                }
            }
        }

        private void ShowAllButton_Click( object sender, EventArgs e ) {
            myShowTeam = false;
            myScoreDataTable.DefaultView.RowFilter = "";
            loadScoreDataGrid( myScoreDataTable );
        }

        private void ViewReportButton_Click( object sender, EventArgs e ) {
            if ( PrintDataGridView.Visible ) {
                PrintDataGridView.Visible = false;
                PrintDataGridView.Enabled = false;
                ViewReportButton.Text = "View Team Results";
            } else {
                PrintDataGridView.Visible = true;
                PrintDataGridView.Enabled = true;
                PrintDataGridView.Location = teamSummaryDataGridView.Location;
                PrintDataGridView.Width = this.Width - 25;
                PrintDataGridView.Height = teamSummaryDataGridView.Height;
                ViewReportButton.Text = "Hide Team Results";
            }
        }

        private void teamSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( teamSummaryDataGridView.RowCount > 0 ) {
                if ( myShowTeam ) {
                    if ( teamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value != null ) {
                        String curFilterCommand = "", curTeamCode = "", curGroup = "";
                        curTeamCode = teamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value.ToString();
                        if ( TeamDiv.Visible ) {
                            curGroup = teamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamDiv"].Value.ToString();
                        }
                        if ( TeamDiv.Visible ) {
                            if (divPlcmtButton.Checked) {
                                curFilterCommand = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCommand = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCommand = "TeamSlalom = '" + curTeamCode + "'";
                        }
                        myScoreDataTable.DefaultView.RowFilter = curFilterCommand;
                        loadScoreDataGrid( myScoreDataTable.DefaultView.ToTable() );
                    }
                }
            }
        }

        private void NumPerTeamTextBox_TextChanged( object sender, EventArgs e ) {
            try {
                myNumPerTeam = Convert.ToInt16( NumPerTeamTextBox.Text );
                myTourProperties.SlalomTeamSummary_NumPerTeam = myNumPerTeam.ToString();
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception: \n" +  ex.Message);
                /*
                if (NumPerTeamTextBox.Text.Length > 0) {
                    MessageBox.Show( "Invalid value in textbox.  Must be a proper integer value." );
                    myNumPerTeam = Convert.ToInt16( myTourProperties.SlalomTeamSummary_NumPerTeam);
                }
                 * */
            }
        }

        private void divPlcmtButton_CheckedChanged(object sender, EventArgs e) {
            if (divPlcmtButton.Checked) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Div";
                PrintTeamDiv.HeaderText = "Div";
            }
        }

        private void groupPlcmtButton_CheckedChanged(object sender, EventArgs e) {
            if (groupPlcmtButton.Checked) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Group";
                PrintTeamDiv.HeaderText = "Group";
            }
        }

        private void plcmtTourButton_CheckedChanged( object sender, EventArgs e ) {
            if ( plcmtTourButton.Checked ) {
                TeamDiv.Visible = false;
            }
        }

        private void EventGroupList_SelectedIndexChanged(object sender, EventArgs e) {
            String curValue = ( (ComboBox)sender ).SelectedValue.ToString();
            if (curValue.ToLower().Equals( "all" )) {
            } else {
                divPlcmtButton.Checked = true;
            }
        }

        private void navExportHtml_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight( 15, '*' );
            printFooter.PadLeft( 15, '*' );
            myExportData.exportDataAsHtml( PrintDataGridView, printTitle, printSubtitle, printFooter );
        }

		private void navPublish_Click( object sender, EventArgs e ) {
			if ( printReport( true ) ) ExportLiveWeb.uploadReportFile( "Results", "Slalom", mySanctionNum );
		}
		private void navPrint_Click( object sender, EventArgs e ) {
			printReport( false );
		}
		private bool printReport( bool inPublish ) {
			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            TeamPrintDialogForm curTeamPrintDialog = new TeamPrintDialogForm();
			if ( curTeamPrintDialog.ShowDialog() != DialogResult.OK ) return false;

			String curPrintReport = curTeamPrintDialog.ReportName;
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

			if ( curPrintDialog.ShowDialog() != DialogResult.OK ) return false;

			StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
			printTitle.Append( "\n Sanction " + mySanctionNum );
			printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
			printTitle.Append( "\n" + this.Text );
			if ( !( curPrintReport.Equals( "TeamResults" ) ) ) printTitle.Append( " - Skier List" );
			if ( inPublish ) printTitle.Append( HelperFunctions.buildPublishReportTitle( mySanctionNum ) );

			myPrintDoc = new PrintDocument();
			myPrintDoc.DocumentName = this.Text;
			myPrintDoc.DefaultPageSettings.Margins = new Margins( 50, 50, 75, 75 );
			myPrintDoc.DefaultPageSettings.Landscape = false;

			if ( curPrintReport.Equals( "TeamResults" ) ) {
				myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
					CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );
			} else {
				myPrintDataGrid = new DataGridViewPrinter( scoreSummaryDataGridView, myPrintDoc,
					CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );
			}

			myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
			myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
			myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
			curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.ShowDialog();

			return true;
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
