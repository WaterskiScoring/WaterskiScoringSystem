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

namespace WaterskiScoringSystem.Trick {
    public partial class TrickScorebook : Form {
        private Int16 myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private DataRow myTourRow;
        private DataTable mySummaryDataTable;
        private DataTable myTrickDataTable;
        private DataTable myMemberData;
        private DataTable myTrickDetail;

        private TourProperties myTourProperties;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public TrickScorebook() {
            InitializeComponent();
        }

        private void TrickScorebook_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TrickScorebook_Width > 0 ) {
                this.Width = Properties.Settings.Default.TrickScorebook_Width;
            }
            if ( Properties.Settings.Default.TrickScorebook_Height > 0 ) {
                this.Height = Properties.Settings.Default.TrickScorebook_Height;
            }
            if ( Properties.Settings.Default.TrickScorebook_Location.X > 0
                && Properties.Settings.Default.TrickScorebook_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TrickScorebook_Location;
            }
            myTourProperties = TourProperties.Instance;
            bestScoreButton.Checked = true;
            if ( myTourProperties.TrickScorebookDataType.ToLower().Equals( "total" ) ) totalScoreButton.Checked = true;
            if ( myTourProperties.TrickScorebookDataType.ToLower().Equals( "final" ) ) finalScoreButton.Checked = true;
            if ( myTourProperties.TrickScorebookDataType.ToLower().Equals( "first" ) ) firstScoreButton.Checked = true;
            nopsPointsButton.Checked = true;
            if ( myTourProperties.TrickScorebookPointsMethod.ToLower().Equals( "nops" ) ) nopsPointsButton.Checked = true;
            if ( myTourProperties.TrickScorebookPointsMethod.ToLower().Equals( "plcmt" ) ) plcmtPointsButton.Checked = true;
            if ( myTourProperties.TrickScorebookPointsMethod.ToLower().Equals( "kbase" ) ) kBasePointsButton.Checked = true;
            if ( myTourProperties.TrickScorebookPointsMethod.ToLower().Equals( "ratio" ) ) ratioPointsButton.Checked = true;

            plcmtDivButton.Checked = true;
            if ( myTourProperties.TrickScorebookPlcmtOrg.ToLower().Equals( "div" ) ) plcmtDivButton.Checked = true;
            if ( myTourProperties.TrickScorebookPlcmtOrg.ToLower().Equals( "divgr" ) ) plcmtDivGrpButton.Checked = true;

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

                        if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
                            kBasePointsButton.Checked = true;
                        } else if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                            plcmtPointsButton.Checked = true;
                        }
                        loadEventGroupList();
                    }
                }
            }
        }

        private void TrickScorebook_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TrickScorebook_Width = this.Size.Width;
                Properties.Settings.Default.TrickScorebook_Height = this.Size.Height;
                Properties.Settings.Default.TrickScorebook_Location = this.Location;
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
                }
                myTourProperties.TrickScorebookDataType = curDataType;
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
                myTourProperties.TrickScorebookPointsMethod = curPointsMethod;

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
                myTourProperties.TrickScorebookPlcmtOrg = curPlcmtOrg;

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

                if (curGroupValue.ToLower().Equals( "all" )) {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                    } else {
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, null, myTrickDataTable, null, curDataType, curPlcmtOverallOrg );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, null, myTrickDetail, null );
                    }
                } else {
                    if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                    } else {
                        myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, null, myTrickDataTable, null, curDataType, curPlcmtOverallOrg );

                        myMemberData = curCalcSummary.getMemberData( mySanctionNum, curGroupValue );
                        myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, null, myTrickDetail, null );
                    }
                }

                loadSummaryDataGrid();
                Cursor.Current = Cursors.Default;
            }
        }

        private void loadSummaryDataGrid() {
            DataGridViewRow curViewRow;
            String curMemberId = "", curAgeGroup = "";
            String prevMemberId = "", prevAgeGroup = "";
            Int16 curRound = 0;
            scoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0, curSmryIdx = 0;
            foreach ( DataRow curRow in mySummaryDataTable.Rows ) {
                if (( (String)curRow["EventClassTrick"] ).Length > 0) {
                    curMemberId = (String)curRow["MemberId"];
                    curAgeGroup = (String)curRow["AgeGroup"];
                    curRound = (Int16)curRow["RoundOverall"];
                    if (curMemberId.Equals( prevMemberId ) && curAgeGroup.Equals( prevAgeGroup )) {
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.Cells["SkierName"].Value = "";
                    } else {
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                        if (curIdx > 0) {
                            curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                            curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                            scoreSummaryDataGridView.Rows[curIdx].Height = 4;
                            curIdx = scoreSummaryDataGridView.Rows.Add();
                        }
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];
                    }
                    curSmryIdx = curIdx - curRound + 1;
                    if (curSmryIdx < 0) curSmryIdx = 0;

                    curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                    curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
                    if (EventGroup.Visible) {
                        curViewRow.Cells["EventGroup"].Value = (String)curRow["EventGroupTrick"];
                    }

                    curViewRow.Cells["SepTrick"].Value = " ";
                    curViewRow.Cells["EventClassTrick"].Value = (String)curRow["EventClassTrick"];
                    curViewRow.Cells["RoundTrick"].Value = (Int16)curRow["RoundTrick"];
                    curViewRow.Cells["Pass1Trick"].Value = (Int16)curRow["Pass1Trick"];
                    curViewRow.Cells["Pass2Trick"].Value = (Int16)curRow["Pass2Trick"];
                    curViewRow.Cells["ScoreTrick"].Value = (Int16)curRow["ScoreTrick"];
                    curViewRow.Cells["PointsTrick"].Value = (Decimal)curRow["PointsTrick"];
                    if (myTourRules.ToLower().Equals( "iwwf" )) {
                        curViewRow.Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                    } else if (curRound == 1) {
                        curViewRow.Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                    } else {
                        if (scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value == null) {
                            scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                            scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassTrick"].Value = (String)curRow["EventClassTrick"];
                        } else if (scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value.ToString().Length == 0) {
                            scoreSummaryDataGridView.Rows[curSmryIdx].Cells["PlcmtTrick"].Value = (String)curRow["PlcmtTrick"];
                            scoreSummaryDataGridView.Rows[curSmryIdx].Cells["EventClassTrick"].Value = (String)curRow["EventClassTrick"];
                        }
                    }
                    prevMemberId = curMemberId;
                    prevAgeGroup = curAgeGroup;
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
                    loadEventGroupListFromData();
                }
            } else {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadEventGroupListFromData();
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
                        loadEventGroupListFromData();
                    }
                }
            }
        }

        private void loadEventGroupListFromData() {
            AgeGroupDropdownList curAgeGroupDropdownList = new AgeGroupDropdownList( myTourRow );
            ArrayList curDropdownList = new ArrayList();
            ArrayList curAgeGroupList = curAgeGroupDropdownList.DropdownList;
            curDropdownList.Add( "All" );
            for ( int idx = 0; idx < curAgeGroupList.Count; idx++ ) {
                curDropdownList.Add( curAgeGroupList[idx] );
            }
            EventGroupList.DataSource = curDropdownList;
        }

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( scoreSummaryDataGridView );
        }

        private void scoreSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + scoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            try {
                curPrintDialog.AllowCurrentPage = true;
                curPrintDialog.AllowPrintToFile = false;
                curPrintDialog.AllowSelection = true;
                curPrintDialog.AllowSomePages = true;
                curPrintDialog.PrintToFile = false;
                curPrintDialog.ShowHelp = false;
                curPrintDialog.ShowNetwork = true;
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

                    if ( plcmtDivButton.Checked ) {
                        mySubtitle = new StringRowPrinter( TrickLabel.Text,
                            451, 0, 215, TrickLabel.Size.Height,
                            TrickLabel.ForeColor, TrickLabel.BackColor, TrickLabel.Font, SubtitleStringFormat );
                        myPrintDataGrid.SubtitleRow = mySubtitle;
                    } else {
                        mySubtitle = new StringRowPrinter( TrickLabel.Text,
                            486, 0, 215, TrickLabel.Size.Height,
                            TrickLabel.ForeColor, TrickLabel.BackColor, TrickLabel.Font, SubtitleStringFormat );
                        myPrintDataGrid.SubtitleRow = mySubtitle;
                    }

                    myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                    myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                    myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                    curPreviewDialog.Document = myPrintDoc;
                    curPreviewDialog.ShowDialog();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered during print request"
                    + "\n\nException: " + ex.Message );
            }

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

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
