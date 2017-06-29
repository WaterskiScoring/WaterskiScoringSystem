using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class RunningOrderTour : Form {
        #region instance variables
        private Boolean isDataModified = false;
        private Boolean isLoadInProg = true;
        private int myViewIdx;
        private String myWindowTitle;
        private String mySanctionNum;
        private String myOrigItemValue = "";
        private String mySortCmd = "";
        private String myFilterCmd = "";
        private String myTourRules = "";
        private String myTourEventClass = "";
        private String myTourClass = "";

        private Decimal myJump5FootM = .235M;
        private Decimal myJump5HFootM = .255M;
        private Decimal myJump6FootM = .271M;
        private Decimal myJump5Foot = 5M;
        private Decimal myJump5HFoot = 5.5M;
        private Decimal myJump6Foot = 6M;

        private DataRow myTourRow;
        private DataTable myEventRegDataTable;

        private TourProperties myTourProperties;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private ListSkierClass mySkierClassList;
        private DataGridViewPrinter myPrintDataGrid;
        private PrintDocument myPrintDoc;
        #endregion

        public RunningOrderTour() {
            isLoadInProg = true;
            InitializeComponent();
            myWindowTitle = this.Text;
        }

        public void RunningOrderForEvent(String inEvent) {
            myTourProperties = TourProperties.Instance;
            if (inEvent.Equals( "Slalom" )) {
                mySortCmd = myTourProperties.RunningOrderSortSlalom;
                slalomButton.Checked = true;
            } else if ( inEvent.Equals( "Trick" ) ) {
                mySortCmd = myTourProperties.RunningOrderSortTrick;
                trickButton.Checked = true;
            } else if ( inEvent.Equals( "Jump" ) ) {
                mySortCmd = myTourProperties.RunningOrderSortJump;
                jumpButton.Checked = true;
            }
        }

        private void RunningOrderTour_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.RunningOrder_Width > 0) {
                this.Width = Properties.Settings.Default.RunningOrder_Width;
            }
            if (Properties.Settings.Default.RunningOrder_Height > 0) {
                this.Height = Properties.Settings.Default.RunningOrder_Height;
            }
            if (Properties.Settings.Default.RunningOrder_Location.X > 0
                && Properties.Settings.Default.RunningOrder_Location.Y > 0) {
                this.Location = Properties.Settings.Default.RunningOrder_Location;
            }
            isLoadInProg = true;
            myTourProperties = TourProperties.Instance;

            String[] curList = { "SkierName", "EventGroup", "Div", "DivOrder", "RunOrder", "TeamCode", "EventClass", "ReadyForPlcmt", "RankingScore", "RankingRating", "JumpHeight", "HCapBase", "HCapScore" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if (mySanctionNum == null) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                //this.Close();
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                    //this.Close();
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData( mySanctionNum );
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        myTourEventClass = (String)myTourRow["EventClass"];
                        myTourClass = (String)myTourRow["Class"];

                        mySkierClassList = new ListSkierClass();
                        mySkierClassList.ListSkierClassLoad();
                        EventClass.DataSource = mySkierClassList.DropdownList;
                        EventClass.DisplayMember = "ItemName";
                        EventClass.ValueMember = "ItemValue";

                        if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
                        if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
                        if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }
                        if ( Convert.ToInt16( myTourRow["SlalomRounds"] ) == 0 ) {
                            slalomButton.Visible = false;
                        }
                        if ( Convert.ToInt16( myTourRow["TrickRounds"] ) == 0 ) {
                            trickButton.Visible = false;
                        }
                        if ( Convert.ToInt16( myTourRow["JumpRounds"] ) == 0 ) {
                            jumpButton.Visible = false;
                        }

                        myFilterCmd = "";
                        if (slalomButton.Checked) {
                            if ( (Byte) myTourRow["SlalomRounds"] == 0 ) {
                                MessageBox.Show("Slalom event is not active for this tournament");
                            } else {
                                mySortCmd = myTourProperties.RunningOrderSortSlalom;
                            }
                        } else if (trickButton.Checked) {
                            if ( (Byte) myTourRow["TrickRounds"] == 0 ) {
                                MessageBox.Show("Trick event is not active for this tournament");
                            } else {
                                mySortCmd = myTourProperties.RunningOrderSortTrick;
                            }
                        } else if (jumpButton.Checked) {
                            if ( (Byte)myTourRow["JumpRounds"] == 0 ) {
                                MessageBox.Show("Jump event is not active for this tournament");
                            } else {
                                mySortCmd = myTourProperties.RunningOrderSortJump;
                            }
                        }

                        if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                            LiveWebLabel.Visible = true;
                        } else {
                            LiveWebLabel.Visible = false;
                        }
                    } else {
                        MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    }
                }
            }
            isDataModified = false;
            isLoadInProg = false;
        }

        private void RunningOrderTour_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.RunningOrder_Width = this.Size.Width;
                Properties.Settings.Default.RunningOrder_Height = this.Size.Height;
                Properties.Settings.Default.RunningOrder_Location = this.Location;
            }
        }

        private void RunningOrderTour_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                checkModifyPrompt();
                if ( isDataModified ) {
                    e.Cancel = true;
                }
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

        private void loadEventRegView() {
            Decimal curRankingScore, curHCapBase, curHCapScore, curJumpHeight;

            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            if (slalomButton.Checked) {
                TrickBoat.Visible = false;
                JumpHeight.Visible = false;
            } else if (trickButton.Checked) {
                TrickBoat.Visible = true;
                JumpHeight.Visible = false;
            } else if (jumpButton.Checked) {
                TrickBoat.Visible = false;
                JumpHeight.Visible = true;
            }

            try {
                if ( myEventRegDataTable.Rows.Count > 0 ) {
                    EventRegDataGridView.Rows.Clear();
                    myEventRegDataTable.DefaultView.Sort = mySortCmd;
                    myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                    DataTable curDataTable = myEventRegDataTable.DefaultView.ToTable();

                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        isDataModified = false;
                        myViewIdx = EventRegDataGridView.Rows.Add();
                        curViewRow = EventRegDataGridView.Rows[myViewIdx];
                        curViewRow.Cells["Updated"].Value = "N";
                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        
                        try {
                            curViewRow.Cells["TrickBoat"].Value = (String)curDataRow["TrickBoat"];
                        } catch {
                            curViewRow.Cells["TrickBoat"].Value = "";
                        }
                        try {
                            curJumpHeight = (Decimal)curDataRow["JumpHeight"];
                            if ( curJumpHeight == myJump5FootM || curJumpHeight == myJump5Foot ) {
                                curViewRow.Cells["JumpHeight"].Value = "5";
                            } else if ( curJumpHeight == myJump5HFootM || curJumpHeight == myJump5HFoot ) {
                                curViewRow.Cells["JumpHeight"].Value = "H";
                            } else if ( curJumpHeight == myJump6FootM || curJumpHeight == myJump6Foot ) {
                                curViewRow.Cells["JumpHeight"].Value = "6";
                            } else {
                                if ( curJumpHeight < 1M ) {
                                    if ( curJumpHeight < myJump5FootM && curJumpHeight > 0M ) {
                                        curViewRow.Cells["JumpHeight"].Value = "4";
                                    } else {
                                        curViewRow.Cells["JumpHeight"].Value = "";
                                    }
                                } else {
                                    if ( curJumpHeight < myJump5Foot && curJumpHeight > 0M ) {
                                        curViewRow.Cells["JumpHeight"].Value = "4";
                                    } else {
                                        curViewRow.Cells["JumpHeight"].Value = "";
                                    }
                                }
                            }
                        } catch {
                            curViewRow.Cells["JumpHeight"].Value = "";
                        }
                        try {
                            curViewRow.Cells["State"].Value = (String)curDataRow["State"];
                        } catch {
                            curViewRow.Cells["State"].Value = "";
                        }
                        try {
                            curViewRow.Cells["City"].Value = (String)curDataRow["City"];
                        } catch {
                            curViewRow.Cells["City"].Value = "";
                        }
                        try {
                            curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                        } catch {
                            curViewRow.Cells["EventGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["ReadyForPlcmt"].Value = (String) curDataRow["ReadyForPlcmt"];
                        } catch {
                            curViewRow.Cells["ReadyForPlcmt"].Value = "";
                        }
                        try {
                            curViewRow.Cells["EventClass"].Value = (String)curDataRow["EventClass"];
                        } catch {
                            curViewRow.Cells["EventClass"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RunOrder"].Value = ( (Int16)curDataRow["RunOrder"] ).ToString( "##0" );
                        } catch {
                            curViewRow.Cells["RunOrder"].Value = "0";
                        }
                        try {
                            curViewRow.Cells["TeamCode"].Value = (String)curDataRow["TeamCode"];
                        } catch {
                            curViewRow.Cells["TeamCode"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RankingRating"].Value = (String)curDataRow["RankingRating"];
                        } catch {
                            curViewRow.Cells["RankingRating"].Value = "";
                        }
                        if ( trickButton.Checked ) {
                            try {
                                curRankingScore = (Decimal)curDataRow["RankingScore"];
                                curViewRow.Cells["RankingScore"].Value = curRankingScore.ToString( "####0" );
                            } catch {
                                curViewRow.Cells["RankingScore"].Value = "";
                            }
                            try {
                                curHCapBase = (Decimal)curDataRow["HCapBase"];
                                curViewRow.Cells["HCapBase"].Value = curHCapBase.ToString( "####0" );
                            } catch {
                                curViewRow.Cells["HCapBase"].Value = "";
                            }
                            try {
                                curHCapScore = (Decimal)curDataRow["HCapScore"];
                                curViewRow.Cells["HCapScore"].Value = curHCapScore.ToString( "####0" );
                            } catch {
                                curViewRow.Cells["HCapScore"].Value = "";
                            }
                        } else {
                            try {
                                curRankingScore = (Decimal)curDataRow["RankingScore"];
                                curViewRow.Cells["RankingScore"].Value = curRankingScore.ToString( "##0.0" );
                            } catch {
                                curViewRow.Cells["RankingScore"].Value = "";
                            }
                            try {
                                curHCapBase = (Decimal)curDataRow["HCapBase"];
                                curViewRow.Cells["HCapBase"].Value = curHCapBase.ToString( "##0.0" );
                            } catch {
                                curViewRow.Cells["HCapBase"].Value = "";
                            }
                            try {
                                curHCapScore = (Decimal)curDataRow["HCapScore"];
                                curViewRow.Cells["HCapScore"].Value = curHCapScore.ToString( "##0.0" );
                            } catch {
                                curViewRow.Cells["HCapScore"].Value = "";
                            }
                        }
                    }
                    if (EventRegDataGridView.Rows.Count > 0) {
                        myViewIdx = 0;
                        EventRegDataGridView.CurrentCell = EventRegDataGridView.Rows[myViewIdx].Cells["SkierName"];
                        loadPrintDataGrid();
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament entries \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
            try {
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + EventRegDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }
            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                LiveWebLabel.Visible = true;
            } else {
                LiveWebLabel.Visible = false;
            }
        }

        private void loadPrintDataGrid() {
            //Load data for print data grid
            String curPrintGroup = "", prevPrintGroup = "", curPageBreakGroup = "", prevPageBreakGroup = "";
            int curPrintIdx = 0, curGroupCount = 0, curPrintCount = 0;
            Decimal curRankingScore, curHCapBase, curHCapScore, curJumpHeight;
            DataGridViewRow curPrintRow;

            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                if ( myEventRegDataTable.Rows.Count > 0 ) {
                    PrintDataGridView.Rows.Clear();
                    //PrintDataGridView.AutoGenerateColumns
                    myEventRegDataTable.DefaultView.Sort = mySortCmd;
                    myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                    DataTable curDataTable = myEventRegDataTable.DefaultView.ToTable();

                    if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                        PrintEventRotation.Visible = true;
                        PrintEventGroup.Visible = false;
                    } else {
                        PrintEventRotation.Visible = false;
                        PrintEventGroup.Visible = true;
                        try {
                            if ((Int16)myEventRegDataTable.Rows[0]["Rotation"] > 0) {
                                PrintEventRotation.Visible = true;
                            } else {
                                PrintEventRotation.Visible = false;
                            }
                        } catch (Exception e) {
                            PrintEventRotation.Visible = false;
                        }
                    }
                    if ( slalomButton.Checked ) {
                        PrintTrickBoat.Visible = false;
                        PrintJumpHeight.Visible = false;
                    } else if ( trickButton.Checked ) {
                        PrintTrickBoat.Visible = true;
                        PrintJumpHeight.Visible = false;
                    } else if ( jumpButton.Checked ) {
                        PrintTrickBoat.Visible = false;
                        PrintJumpHeight.Visible = true;
                    }

                    foreach ( DataRow curTeamRow in curDataTable.Rows ) {
                        curPrintIdx = PrintDataGridView.Rows.Add();
                        curPrintRow = PrintDataGridView.Rows[curPrintIdx];

                        try {
                            if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                                curPrintGroup = (String)curTeamRow["AgeGroup"] + "-" + (String)curTeamRow["EventGroup"];
                                curPageBreakGroup = (String)curTeamRow["AgeGroup"];
                            } else {
                                curPrintGroup = (String)curTeamRow["EventGroup"];
                                curPageBreakGroup = (String)curTeamRow["EventGroup"];
                            }
                        } catch {
                            curPrintGroup = "";
                        }

                        curPrintCount++;
                        if (!( curPrintGroup.Equals( prevPrintGroup ) )) {
                            curGroupCount = 1;
                            if (curPrintIdx > 0) {
                                if (!( curPageBreakGroup.Equals( prevPageBreakGroup ) )) {
                                    curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                    curPrintRow.Height = 8;
                                }
                                curPrintIdx = PrintDataGridView.Rows.Add();
                                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                            }
                        } else {
                            curGroupCount++;
                        }

                        curPrintRow.Cells["PrintGroupCount"].Value = curGroupCount;
                        curPrintRow.Cells["PrintCount"].Value = curPrintCount;
                        curPrintRow.Cells["PrintSkierName"].Value = (String)curTeamRow["SkierName"];
                        curPrintRow.Cells["PrintEvent"].Value = (String)curTeamRow["Event"];
                        curPrintRow.Cells["PrintAgeGroup"].Value = (String)curTeamRow["AgeGroup"];
                        curPrintRow.Cells["PrintEventGroup"].Value = (String) curTeamRow["EventGroup"];
                        try {
                            //curPrintRow.Cells["PrintEventRotation"].Value = (String)curTeamRow["EventGroup"];
                            curPrintRow.Cells["PrintEventRotation"].Value = ( (Int16)curTeamRow["Rotation"] ).ToString() ;
                        } catch {
                            curPrintRow.Cells["PrintEventRotation"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintEventClass"].Value = (String)curTeamRow["EventClass"];
                        } catch {
                            curPrintRow.Cells["PrintEventClass"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintReadyForPlcmt"].Value = (String) curTeamRow["ReadyForPlcmt"];
                        } catch {
                            curPrintRow.Cells["PrintReadyForPlcmt"].Value = "";
                        }
                        try {
                            if ( ((String) curTeamRow["ReadyForPlcmt"]).Equals("N") ) {
                                curPrintRow.Cells["PrintRunOrder"].Value = "**";
                            } else {
                                curPrintRow.Cells["PrintRunOrder"].Value = ( (Int16) curTeamRow["RunOrder"] ).ToString("##0");
                            }
                        } catch {
                            curPrintRow.Cells["PrintRunOrder"].Value = "0";
                        }
                        try {
                            curPrintRow.Cells["PrintTeam"].Value = (String)curTeamRow["TeamCode"];
                        } catch {
                            curPrintRow.Cells["PrintTeam"].Value = "";
                        }

                        if ( slalomButton.Checked ) {
                            curPrintRow.Cells["PrintTrickBoat"].Value = "";
                            curPrintRow.Cells["PrintJumpHeight"].Value = "";
                        } else if ( trickButton.Checked ) {
                            curPrintRow.Cells["PrintJumpHeight"].Value = "";
                            try {
                                curPrintRow.Cells["PrintTrickBoat"].Value = (String)curTeamRow["TrickBoat"];
                            } catch {
                                curPrintRow.Cells["PrintTrickBoat"].Value = "";
                            }
                        } else if ( jumpButton.Checked ) {
                            try {
                                curJumpHeight = (Decimal)curTeamRow["JumpHeight"];
                                if ( curJumpHeight == myJump5FootM || curJumpHeight == myJump5Foot ) {
                                    curPrintRow.Cells["PrintJumpHeight"].Value = "5";
                                } else if ( curJumpHeight == myJump5HFootM || curJumpHeight == myJump5HFoot ) {
                                    curPrintRow.Cells["PrintJumpHeight"].Value = "H";
                                } else if ( curJumpHeight == myJump6FootM || curJumpHeight == myJump6Foot ) {
                                    curPrintRow.Cells["PrintJumpHeight"].Value = "6";
                                } else {
                                    if ( curJumpHeight < 1M ) {
                                        if ( curJumpHeight < myJump5FootM && curJumpHeight > 0M ) {
                                            curPrintRow.Cells["PrintJumpHeight"].Value = "4";
                                        } else {
                                            curPrintRow.Cells["PrintJumpHeight"].Value = "";
                                        }
                                    } else {
                                        if ( curJumpHeight < myJump5Foot && curJumpHeight > 0M ) {
                                            curPrintRow.Cells["PrintJumpHeight"].Value = "4";
                                        } else {
                                            curPrintRow.Cells["PrintJumpHeight"].Value = "";
                                        }
                                    }
                                }
                            } catch {
                                curPrintRow.Cells["PrintJumpHeight"].Value = "";
                            }

                        }
                        if ( trickButton.Checked ) {
                            try {
                                curRankingScore = (Decimal)curTeamRow["RankingScore"];
                                curPrintRow.Cells["PrintRankingScore"].Value = curRankingScore.ToString( "####0" );
                            } catch {
                                curPrintRow.Cells["PrintRankingScore"].Value = "";
                            }
                            try {
                                curHCapBase = (Decimal)curTeamRow["HCapBase"];
                                curPrintRow.Cells["PrintHCapBase"].Value = curHCapBase.ToString( "####0" );
                            } catch {
                                curPrintRow.Cells["PrintHCapBase"].Value = "";
                            }
                            try {
                                curHCapScore = (Decimal)curTeamRow["HCapScore"];
                                curPrintRow.Cells["PrintHCapScore"].Value = curHCapScore.ToString( "####0" );
                            } catch {
                                curPrintRow.Cells["PrintHCapScore"].Value = "";
                            }
                        } else {
                            try {
                                curRankingScore = (Decimal)curTeamRow["RankingScore"];
                                curPrintRow.Cells["PrintRankingScore"].Value = curRankingScore.ToString( "##0.0" );
                            } catch {
                                curPrintRow.Cells["PrintRankingScore"].Value = "";
                            }
                            try {
                                curHCapBase = (Decimal)curTeamRow["HCapBase"];
                                curPrintRow.Cells["PrintHCapBase"].Value = curHCapBase.ToString( "##0.0" );
                            } catch {
                                curPrintRow.Cells["PrintHCapBase"].Value = "";
                            }
                            try {
                                curHCapScore = (Decimal)curTeamRow["HCapScore"];
                                curPrintRow.Cells["PrintHCapScore"].Value = curHCapScore.ToString( "##0.0" );
                            } catch {
                                curPrintRow.Cells["PrintHCapScore"].Value = "";
                            }
                        }
                        try {
                            curPrintRow.Cells["PrintRankingRating"].Value = (String)curTeamRow["RankingRating"];
                        } catch {
                            curPrintRow.Cells["PrintRankingRating"].Value = "";
                        }

                        prevPrintGroup = curPrintGroup;
                        prevPageBreakGroup = curPageBreakGroup;
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament entries \n" + ex.Message );
            }
            

        }

        private void loadGroupSelectList(String inEvent, EventHandler parentEvent) {
            CheckBox curCheckBox;
            int curItemLoc = 5;
            int curItemSize = 0;
            ArrayList curEventGroupList = new ArrayList();
            myFilterCmd = "";

            if (!isLoadInProg) {
                try {
                    EventGroupPanel.Controls.Clear();

                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        curEventGroupList.Add( "Men A" );
                        curEventGroupList.Add( "Women A" );
                        curEventGroupList.Add( "Men B" );
                        curEventGroupList.Add( "Women B" );
                        curEventGroupList.Add( "Non Team" );
                    } else {
                        String curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                            + "WHERE SanctionId = '" + mySanctionNum + "' And Event = '" + inEvent + "' "
                            + "Order by EventGroup";
                        DataTable curDataTable = getData( curSqlStmt );
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curEventGroupList.Add( (String)curRow["EventGroup"] );
                        }
                    }

                    curItemSize = 28;
                    curCheckBox = new CheckBox();
                    curCheckBox.Checked = true;
                    curCheckBox.AutoSize = true;
                    curCheckBox.Location = new System.Drawing.Point( curItemLoc, 2 );
                    curCheckBox.Name = "GroupAll_CB";
                    curCheckBox.Size = new System.Drawing.Size( curItemSize, 18 );
                    curCheckBox.TabIndex = 0;
                    curCheckBox.TabStop = true;
                    curCheckBox.Text = "All";
                    curCheckBox.Font = new System.Drawing.Font( "Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
                    curCheckBox.Click += new System.EventHandler( parentEvent );
                    curItemLoc += curItemSize + 20;
                    EventGroupPanel.Controls.Add( curCheckBox );

                    foreach (String curGroup in curEventGroupList) {
                        curItemSize = 10 + curGroup.Length * 6;

                        curCheckBox = new CheckBox();
                        curCheckBox.Checked = true;
                        curCheckBox.AutoSize = true;
                        curCheckBox.Location = new System.Drawing.Point( curItemLoc, 2 );
                        curCheckBox.Name = "Group" + curGroup + "_CB";
                        curCheckBox.Size = new System.Drawing.Size( curItemSize, 18 );
                        curCheckBox.TabIndex = 0;
                        curCheckBox.TabStop = false;
                        curCheckBox.Text = curGroup;
                        curCheckBox.Font = new System.Drawing.Font( "Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
                        curCheckBox.Click += new System.EventHandler( parentEvent );

                        curItemLoc += curItemSize + 20;
                        EventGroupPanel.Controls.Add( curCheckBox );
                    }
                } catch (Exception ex) {
                    MessageBox.Show( "Exception encountered \n Exception: " + ex.Message );
                }
            }
        }

        private void checkModifyPrompt() {
            if ( isDataModified ) {
                String dialogMsg = "Changes have been made to currently displayed data!"
                    + "\n Do you want to save the changes before closing the window?";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if ( msgResp == DialogResult.Yes ) {
                    try {
                        navSave_Click( null, null );
                    } catch ( Exception excp ) {
                        MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    }
                } else if ( msgResp == DialogResult.No ) {
                    isDataModified = false;
                }
            }
        }

        private void navSave_Click(object sender, EventArgs e) {
            String curMethodName = "Tournament:RunningOrderTour:navSave";
            String curMsg = "";
            bool curReturn = true;
            int rowsProc = 0;

            try {
                if ( EventRegDataGridView.Rows.Count > 0 ) {
                    printHeaderNote.Focus();
                    EventRegDataGridView.EndEdit();
                    EventRegDataGridView.Focus();
                    DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                    String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                    if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                        try {
                            String curEventGroup, curTeamCode, curEventClass, curRankingRating, curReadyForPlcmt;
                            Decimal curHCapBase, curHCapScore, curRankingScore;
                            Int16 curRunOrder;
                            Int64 curPK = Convert.ToInt64( (String)curViewRow.Cells["PK"].Value );

                            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
                            String curScoreTable = (String)curViewRow.Cells["Event"].Value + "Score";
                            String curRecapTable = (String)curViewRow.Cells["Event"].Value + "Recap";
                            if ( trickButton.Checked ) curRecapTable = "TrickPass";

                            try {
                                curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                            } catch {
                                curEventGroup = "";
                            }
                            try {
                                curReadyForPlcmt = (String) curViewRow.Cells["ReadyForPlcmt"].Value;
                            } catch {
                                curReadyForPlcmt = "";
                            }
                            try {
                                curTeamCode = (String)curViewRow.Cells["TeamCode"].Value;
                            } catch {
                                curTeamCode = "";
                            }
                            try {
                                curEventClass = (String)curViewRow.Cells["EventClass"].Value;
                            } catch {
                                curEventClass = "";
                            }
                            try {
                                curRankingRating = (String)curViewRow.Cells["RankingRating"].Value;
                            } catch {
                                curRankingRating = "";
                            }
                            try {
                                curHCapBase = Convert.ToDecimal((String)curViewRow.Cells["HCapBase"].Value);
                            } catch {
                                curHCapBase = 0;
                            }
                            try {
                                curHCapScore = Convert.ToDecimal((String)curViewRow.Cells["HCapScore"].Value);
                            } catch {
                                curHCapScore = 0;
                            }
                            try {
                                curRankingScore = Convert.ToDecimal( (String)curViewRow.Cells["RankingScore"].Value );
                            } catch {
                                curRankingScore = 0;
                            }
                            try {
                                curRunOrder = Convert.ToInt16( (String)curViewRow.Cells["RunOrder"].Value );
                            } catch {
                                curRunOrder = 0;
                            }

                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update EventReg Set " );
                            curSqlStmt.Append( "EventGroup = '" + curEventGroup + "'" );
                            curSqlStmt.Append( ", TeamCode = '" + curTeamCode + "'" );
                            curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );
                            curSqlStmt.Append(", ReadyForPlcmt = '" + curReadyForPlcmt + "'");
                            curSqlStmt.Append( ", RankingRating = '" + curRankingRating + "'" );
                            curSqlStmt.Append( ", HCapBase = " + curHCapBase.ToString() );
                            curSqlStmt.Append( ", HCapScore = " + curHCapScore.ToString() );
                            curSqlStmt.Append( ", RankingScore = " + curRankingScore.ToString() );
                            curSqlStmt.Append( ", RunOrder = " + curRunOrder.ToString() );
                            curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                            curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            isDataModified = false;

                        } catch ( Exception excp ) {
                            curReturn = false;
                            curMsg = "Error attempting to update skier event information \n" + excp.Message;
                            MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                    }
                }
            } catch (Exception excp) {
                curMsg = "Error attempting to save changes \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void navSort_Click(object sender, EventArgs e) {
            replaceAttr( mySortCmd, "AgeGroup", "DivOrder" );
            sortDialogForm.SortCommand = mySortCmd;
            //sortDialogForm.SortCommand = "";
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if (sortDialogForm.DialogResult == DialogResult.OK) {
                mySortCmd = sortDialogForm.SortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCmd;
                if ( slalomButton.Checked ) {
                    mySortCmd = removeInvalidAttr(mySortCmd, "JumpHeight" );
                    mySortCmd = removeInvalidAttr( mySortCmd, "TrickBoat" );
                    myTourProperties.RunningOrderSortSlalom = mySortCmd;
                } else if (trickButton.Checked) {
                    mySortCmd = removeInvalidAttr( mySortCmd, "JumpHeight" );
                    myTourProperties.RunningOrderSortTrick = mySortCmd;
                } else if (jumpButton.Checked) {
                    mySortCmd = removeInvalidAttr( mySortCmd, "TrickBoat" );
                    myTourProperties.RunningOrderSortJump = mySortCmd;
                }
                loadEventRegView();
            }
        }

        private String removeInvalidAttr(String inSortCmd, String inAttrName) {
            String curReturnValue = "";

            int curDelim = inSortCmd.IndexOf( inAttrName );
            if ( curDelim < 0 ) {
                curReturnValue = inSortCmd;
            } else {
                String tmpSortCmd = "";
                int curDelimComma = inSortCmd.IndexOf( ",", curDelim );
                if ( curDelimComma > 0 ) {
                    tmpSortCmd = inSortCmd.Substring( 0, curDelim ) + inSortCmd.Substring( curDelimComma + 2 );
                } else {
                    tmpSortCmd = inSortCmd.Substring( 0, curDelim - 2 );
                }
                curReturnValue = tmpSortCmd;
            }

            return curReturnValue;
        }

        private String replaceAttr(String inSortCmd, String inAttrName, String inReplaceValue) {
            String curReturnValue = "";

            int curDelim = inSortCmd.IndexOf( inAttrName );
            if (curDelim < 0) {
                curReturnValue = inSortCmd;
            } else if (curDelim > 0) {
                curReturnValue = inSortCmd.Substring( 0, curDelim ) + inReplaceValue + inSortCmd.Substring( curDelim + inAttrName.Length );
            } else {
                curReturnValue = inReplaceValue + inSortCmd.Substring( inAttrName.Length );
            }

            return curReturnValue;
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (filterDialogForm.DialogResult == DialogResult.OK) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadEventRegView();
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[5];
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "DivOrder", "OfficialWork" };

            curSelectCommand[0] = "SELECT * FROM TourReg "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "And EXISTS (SELECT 1 FROM EventReg " 
                + "    WHERE TourReg.SanctionId = EventReg.SanctionId AND TourReg.MemberId = EventReg.MemberId "
                + "      AND TourReg.AgeGroup = EventReg.AgeGroup AND EventReg.Event = '" + curEvent  + "' ";
            if ( isObjectEmpty( myFilterCmd ) ) {
                curSelectCommand[0] = curSelectCommand[0] + ") ";
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[0] = curSelectCommand[0] + "And " + myFilterCmd + ") ";
                } else {
                    curSelectCommand[0] = curSelectCommand[0] + ") ";
                }
            }
            
            curSelectCommand[1] = "Select * from EventReg ";
            if ( isObjectEmpty( myFilterCmd ) ) {
                curSelectCommand[1] = curSelectCommand[1]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = '" + curEvent + "'";
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'"
                        + " And " + myFilterCmd;
                } else {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'";
                }
            }

            curSelectCommand[2] = "Select * from EventRunOrder ";
            if (isObjectEmpty( myFilterCmd )) {
                curSelectCommand[2] = curSelectCommand[2]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = '" + curEvent + "'";
            } else {
                if (myFilterCmd.Length > 0) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'"
                        + " And " + myFilterCmd;
                } else {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'";
                }
            }

            curSelectCommand[3] = "Select * from DivOrder "
                + " Where SanctionId = '" + mySanctionNum + "'"
                + " And Event = '" + curEvent + "'";

            curSelectCommand[4] = "Select * from OfficialWork W Where SanctionId = '" + mySanctionNum + "' "
                + "And W.LastUpdateDate is not null ";

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navClassChangeButton_Click(object sender, EventArgs e) {
            String curMethodName = "RunningOrder:navClassChangeButton_Click";
            String curEvent = "";
            if (slalomButton.Checked) {
                curEvent = "Slalom";
            } else if (trickButton.Checked) {
                curEvent = "Trick";
            } else if (jumpButton.Checked) {
                curEvent = "Jump";
            }

            RunOrderClassForm curForm = new RunOrderClassForm();
            curForm.showClassChangeWindow( myTourRow, curEvent );
            curForm.ShowDialog( this );
            // Determine if the OK button was clicked on the dialog box.
            if (curForm.DialogResult == DialogResult.OK) {
                String curSkierClassNew = curForm.CopyToClass;
                String curSkierClassOld = curForm.CopyFromClass;
                String curSkierGroup = curForm.CopyGroup;
                if (curSkierClassNew.Length > 1) curSkierClassNew = curSkierClassNew.Substring( 0, 1 );
                if (curSkierClassNew.Equals( curSkierClassOld )) {
                } else if (mySkierClassList.compareClassChange( curSkierClassNew, myTourClass ) < 0) {
                    MessageBox.Show( "Class " + curSkierClassNew + " cannot be assigned to a skier in a class " + myTourClass + " tournament" );
                } else {
                    try {
                        StringBuilder curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update EventReg Set " );
                        curSqlStmt.Append( "EventClass = '" + curSkierClassNew + "'" );
                        curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
                        curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                        curSqlStmt.Append( "And Event = '" + curEvent + "' " );
                        if (!curSkierClassOld.ToLower().Equals( "all" )) {
                            curSqlStmt.Append( "And EventClass = '" + curSkierClassOld + "' " );
                        }
                        if (!curSkierGroup.ToLower().Equals( "all" )) {
                            curSqlStmt.Append( "And EventGroup = '" + curSkierGroup + "' " );
                        }
                        int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        isDataModified = false;
                        navRefresh_Click( null, null );                        
                    } catch (Exception excp) {
                        String curMsg = "Error attempting to update skier class for group \n" + excp.Message;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                }
            }
        }

        private void navRecalcHcapButton_Click( object sender, EventArgs e ) {
            String curMethodName = "RunningOrder:navRecalcHcapButton_Click: ";
            String curEvent = "";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            Decimal curHcapScore = 0;
            Decimal curHcapBase = (Decimal) myTourRow["Hcap" + curEvent + "Base"];
            Decimal curHcapPct = (Decimal) myTourRow["Hcap" + curEvent + "Pct"];

            StringBuilder curSqlStmt = new StringBuilder("");
            try {
                curSqlStmt.Append("Update EventReg Set ");
                if ( curHcapBase > 0 && curHcapPct  > 0) {
                    curSqlStmt.Append("HcapScore = (" + curHcapBase + " - HCapBase) * " + curHcapPct);
                } else {
                    curSqlStmt.Append("HcapScore = 0");
                }
                curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' ");
                curSqlStmt.Append("  AND Event = '" + curEvent + "' ");
                int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());

                navRefresh_Click(null, null);

            } catch ( Exception excp ) {
                MessageBox.Show(curMethodName + "Exception encountered:\n" + excp.Message);
            }

        }

        private void navExportRunningOrder_Click(object sender, EventArgs e) {
            String curPageBreak = "\f";
            String curSkierName, curAgeGroup, curRunOrder, 
                curRankingScore, curTeamCode, curState, curJumpHeight, curTrickBoat;
            String curEventGroup = "", prevEventGroup = "", curEventClass = "", prevEventClass = "", curRotation = "", prevRotation = "";
            int rowCount = 0, curOrder = 0, curPageRowCount = 0;
            bool curSideBySide = false;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            if (myTourRules.ToLower().Equals( "ncwsa" )) {
                String dialogMsg = "Do you want a double side by side version?";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Report Type",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2 );
                if (msgResp == DialogResult.Yes) {
                    curSideBySide = true;
                } else if (msgResp == DialogResult.No) {
                    curSideBySide = false;
                }
            }

            try {
                String curTourName = (String)myTourRow["Name"];

                String curEvent = "Slalom", curEventHeader = " SCORE SEQ    ";
                if ( slalomButton.Checked ) {
                    curEvent = "Slalom";
                    curEventHeader = " SCORE SEQ    ";
                } else if ( trickButton.Checked ) {
                    curEvent = "Trick";
                    curEventHeader = " SCORE SEQ BT ";
                } else if ( jumpButton.Checked ) {
                    curEvent = "Jump";
                    curEventHeader = " SCORE SEQ RH ";
                }
                String curFileName = curEvent + "_RunOrder.txt";
                outBuffer = getExportFile(curFileName);
                if ( outBuffer != null ) {

                    foreach ( DataGridViewRow curViewRow in EventRegDataGridView.Rows ) {
                        rowCount++;
                        curOrder++;
                        curPageRowCount++;
                        if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                            curEventGroup = (String)curViewRow.Cells["AgeGroup"].Value;
                            if ( curEventGroup.ToLower().Equals( "cm" ) || curEventGroup.ToLower().Equals( "cw" )
                                 || curEventGroup.ToLower().Equals( "bm" ) || curEventGroup.ToLower().Equals( "bw" )
                                ) {
                                String curValue = (String)curViewRow.Cells["EventGroup"].Value;
                                if ( curValue.Length > 1 ) {
                                    if ( curValue.Length == 2 ) {
                                        curEventGroup += "-" + curValue.Substring( 0, 1 );
                                        curRotation = curValue.Substring( 1, 1 );
                                    } else {
                                        curEventGroup += "-" + curValue.Substring( 1, 1 );
                                        curRotation = curValue.Substring( curValue.Length - 1, 1 );
                                    }
                                }
                            } else {
                                curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                                curRotation = "";
                            }
                        } else {
                            curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                            curRotation = "";
                        }
                        curEventClass = (String)curViewRow.Cells["EventClass"].Value;
                        if ( !(curEventGroup.ToLower().Equals( prevEventGroup.ToLower()) )
                            || !( curEventClass.ToLower().Equals( prevEventClass.ToLower() ) ) 
                            ) {
                            if (prevEventGroup == "" && prevEventClass == "") {
                                outLine = new StringBuilder( " " );
                            } else {
                                if (curPageRowCount > 15) {
                                    outLine = new StringBuilder( curPageBreak );
                                    curPageRowCount = 1;
                                } else {
                                    outLine = new StringBuilder( " " );
                                }
                            }
                            writeExportFile( outBuffer, outLine, curSideBySide );
                            outLine = new StringBuilder( " " + mySanctionNum + " " + curTourName );
                            writeExportFile( outBuffer, outLine, curSideBySide );
                            outLine = new StringBuilder( curEventHeader + "(CL " + curEventClass + ") " + curEventGroup + " " + curEvent + " " );
                            writeExportFile( outBuffer, outLine, curSideBySide );
                            curOrder = 1;
                        }
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (curRotation != prevRotation) {
                                outLine = new StringBuilder( "Rotation " + curRotation );
                                writeExportFile( outBuffer, outLine, curSideBySide );
                            }
                        }

                        prevEventGroup = curEventGroup;
                        prevEventClass = curEventClass;
                        prevRotation = curRotation;

                        outLine = new StringBuilder( "" );
                        curSkierName = (String)curViewRow.Cells["SkierName"].Value;
                        curRankingScore = (String)curViewRow.Cells["RankingScore"].Value;
                        outLine.Append( curRankingScore.PadLeft( 6, ' ' ));
                        curRunOrder = curOrder.ToString( "##0" );
                        outLine.Append( " " + curRunOrder.PadLeft( 3, ' ' ) );
                        if ( slalomButton.Checked ) {
                            outLine.Append( "    " );
                        } else if ( trickButton.Checked ) {
                            curTrickBoat = (String)curViewRow.Cells["TrickBoat"].Value;
                            if ( curTrickBoat.Length > 2 ) {
                                curTrickBoat = curTrickBoat.Substring( 0, 2 );
                            }
                            outLine.Append( " " + curTrickBoat.PadRight(3, ' ' ));
                        } else if ( jumpButton.Checked ) {
                            curJumpHeight = (String)curViewRow.Cells["JumpHeight"].Value;
                            if ( curJumpHeight.Length > 1 ) {
                                curJumpHeight = curJumpHeight.Substring( 0, 1 );
                            }
                            outLine.Append( " " + curJumpHeight.PadRight( 3, ' ' ) );
                        } else {
                            outLine.Append( "    " );
                        }
                        curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
                        curTeamCode = (String)curViewRow.Cells["TeamCode"].Value;
                        curState = (String)curViewRow.Cells["State"].Value;
                        outLine.Append( curAgeGroup + " " + curSkierName );
                        if ( curTeamCode.Trim().Length > 1 ) {
                            outLine.Append( " (" + curTeamCode + ") " );
                        } else {
                            outLine.Append( " (" + curState + ")" );
                        }

                        writeExportFile( outBuffer, outLine, curSideBySide );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write running order data to file\n\nError: " + ex.Message );
            } finally {
                if ( outBuffer != null ) {
                    outBuffer.Close();
                }
            }
        }

        private void navExportSplashEye_Click(object sender, EventArgs e) {
            String curJumpHeight, curTrickBoat;
            String curAgeGroup = "", curEventGroup = "", prevEventGroup = "", prevAgeGroup = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            try {
                String curTourName = (String)myTourRow["Name"];

                String curEvent = "Slalom";
                if (slalomButton.Checked) {
                    curEvent = "Slalom";
                } else if (trickButton.Checked) {
                    curEvent = "Trick";
                } else if (jumpButton.Checked) {
                    curEvent = "Jump";
                }
                String curFileName = curEvent + "_SplashEye.xml";
                outBuffer = getExportFile( curFileName );
                if (outBuffer != null) {
                    outLine = new StringBuilder( "<Tournament name=\"" + curTourName + "\" SanctionId=\"" + mySanctionNum + "\" >" );
                    writeExportFile( outBuffer, outLine, false );
                    outLine = new StringBuilder( "    <Event name=\"" + curEvent + "\" >" );
                    writeExportFile( outBuffer, outLine, false );

                    foreach (DataGridViewRow curViewRow in EventRegDataGridView.Rows) {
                        curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                        curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;

                        if ( curEventGroup.ToLower().Equals( prevEventGroup.ToLower() ) ) {
                            if (!( curAgeGroup.ToLower().Equals( prevAgeGroup.ToLower() ) )) {
                                outLine = new StringBuilder( "            </Div>" );
                                writeExportFile( outBuffer, outLine, false );
                                outLine = new StringBuilder( "            <Div name=\"" + curAgeGroup + "\" >" );
                                writeExportFile( outBuffer, outLine, false );
                            }
                        } else {
                            if (prevEventGroup == "") {
                                outLine = new StringBuilder( "        <EventGroup name=\"" + curEventGroup + "\" >" );
                                writeExportFile( outBuffer, outLine, false );
                                outLine = new StringBuilder( "            <Div name=\"" + curAgeGroup + "\" >" );
                                writeExportFile( outBuffer, outLine, false );
                            } else {
                                outLine = new StringBuilder( "            </Div>" );
                                writeExportFile( outBuffer, outLine, false );
                                outLine = new StringBuilder( "        </EventGroup>" );
                                writeExportFile( outBuffer, outLine, false );
                                outLine = new StringBuilder( "        <EventGroup name=\"" + curEventGroup + "\" >" );
                                writeExportFile( outBuffer, outLine, false );
                                outLine = new StringBuilder( "            <Div name=\"" + curAgeGroup + "\" >" );
                                writeExportFile( outBuffer, outLine, false );
                            }
                        }
                        prevEventGroup = curEventGroup;
                        prevAgeGroup = curAgeGroup;

                        outLine = new StringBuilder( "                <Skier name=\"" + (String)curViewRow.Cells["SkierName"].Value + "\"" );
                        outLine.Append( " RankingScore=\"" + (String)curViewRow.Cells["RankingScore"].Value + "\"" );
                        outLine.Append( " EventClass=\"" + (String)curViewRow.Cells["EventClass"].Value + "\"" );
                        if (slalomButton.Checked) {
                        } else if (trickButton.Checked) {
                            curTrickBoat = (String)curViewRow.Cells["TrickBoat"].Value;
                            if (curTrickBoat.Length > 2) {
                                curTrickBoat = curTrickBoat.Substring( 0, 2 );
                            }
                            outLine.Append( " Boat=\"" + curTrickBoat + "\"" );
                        } else if (jumpButton.Checked) {
                            curJumpHeight = (String)curViewRow.Cells["JumpHeight"].Value;
                            if (curJumpHeight.Length > 1) {
                                curJumpHeight = curJumpHeight.Substring( 0, 1 );
                            }
                            outLine.Append( " Ramp=\"" + curJumpHeight + "\"" );
                        }
                        outLine.Append( " Team=\"" + (String)curViewRow.Cells["TeamCode"].Value + "\"" );
                        outLine.Append( " State=\"" + (String)curViewRow.Cells["State"].Value + "\"" );
                        outLine.Append( " City=\"" + (String)curViewRow.Cells["City"].Value + "\"" );
                        outLine.Append( "/>" );
                        writeExportFile( outBuffer, outLine, false );
                    }

                    outLine = new StringBuilder( "            </Div>" );
                    writeExportFile( outBuffer, outLine, false );
                    outLine = new StringBuilder( "        </EventGroup>" );
                    writeExportFile( outBuffer, outLine, false );
                    outLine = new StringBuilder( "    </Event>" );
                    writeExportFile( outBuffer, outLine, false );
                    outLine = new StringBuilder( "</Tournament>" );
                    writeExportFile( outBuffer, outLine, false );
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not write running order data to file\n\nError: " + ex.Message );
            } finally {
                if (outBuffer != null) {
                    outBuffer.Close();
                }
            }
        }

        private void navSaveAs_Click( object sender, EventArgs e ) {
            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }
            ExportData myExportData = new ExportData();
            myExportData.exportData(EventRegDataGridView, curEvent + "RunOrderList.txt");
        }

        private void writeExportFile(StreamWriter outBuffer, StringBuilder outLine, bool inSideBySide) {
            if (inSideBySide) {
                String curPad = "";
                if (outLine.Length > 50) {
                    outBuffer.WriteLine( outLine.ToString().Substring( 0, 50 ) + " " + outLine.ToString().Substring( 0, 50 ) );
                } else {
                    outBuffer.WriteLine( outLine.ToString() + curPad.PadLeft( 50 - outLine.Length ) + outLine.ToString() );
                }
            } else {
                outBuffer.WriteLine( outLine.ToString() );
            }
        }
        
        private void navRefresh_Click(object sender, EventArgs e) {
            if ( isDataModified ) {
                checkModifyPrompt();
            }
            if (!(isDataModified)) {
                //Retrieve running order data for display
                myEventRegDataTable = getEventRegData();
                loadEventRegView();
            }
        }

        private void slalomButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                this.Text = myWindowTitle + " - Slalom";
                checkSaveChanges();
                if (!( isDataModified )) {
                    mySortCmd = myTourProperties.RunningOrderSortSlalom;

                    loadGroupSelectList( "Slalom", checkBoxGroup_CheckedChanged );

                    winStatusMsg.Text = "Sorted by " + mySortCmd;
                    myEventRegDataTable = getEventRegData();
                    loadEventRegView();
                }
            }
        }

        private void trickButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                this.Text = myWindowTitle + " - Trick";
                checkSaveChanges();
                if (!( isDataModified )) {
                    mySortCmd = myTourProperties.RunningOrderSortTrick;

                    loadGroupSelectList( "Trick", checkBoxGroup_CheckedChanged );

                    winStatusMsg.Text = "Sorted by " + mySortCmd;
                    myEventRegDataTable = getEventRegData();
                    loadEventRegView();
                }
            }
        }

        private void jumpButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                this.Text = myWindowTitle + " - Jump";
                checkSaveChanges();
                if (!( isDataModified )) {
                    mySortCmd = myTourProperties.RunningOrderSortJump;

                    loadGroupSelectList( "Jump", checkBoxGroup_CheckedChanged );

                    winStatusMsg.Text = "Sorted by " + mySortCmd;
                    myEventRegDataTable = getEventRegData();
                    loadEventRegView();
                }
            }
        }

        private void checkBoxGroup_CheckedChanged(object sender, EventArgs e) {
            myFilterCmd = "";
            if (( (CheckBox)sender ).Text.Equals( "All" )) {
                if (( (CheckBox)sender ).Checked) {
                    foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                        curCheckBox.Checked = true;
                    }
                } else {
                    foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                        curCheckBox.Checked = false;
                    }
                }
            } else {
                foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                    if (curCheckBox.Text.Equals( "All" )) {
                        curCheckBox.Checked = false;
                    } else {
                        if (curCheckBox.Checked) {
                            if (myTourRules.ToLower().Equals( "ncwsa" )) {
                                if (curCheckBox.Text.Equals( "Men A" )) {
                                    if (myFilterCmd.Length > 1) {
                                        myFilterCmd += " OR AgeGroup = 'CM'";
                                    } else {
                                        myFilterCmd = "AgeGroup = 'CM'";
                                    }
                                } else if (curCheckBox.Text.Equals( "Women A" )) {
                                    if (myFilterCmd.Length > 1) {
                                        myFilterCmd += " OR AgeGroup = 'CW'";
                                    } else {
                                        myFilterCmd = "AgeGroup = 'CW'";
                                    }
                                } else if (curCheckBox.Text.Equals( "Men B" )) {
                                    if (myFilterCmd.Length > 1) {
                                        myFilterCmd += " OR AgeGroup = 'BM'";
                                    } else {
                                        myFilterCmd = "AgeGroup = 'BM'";
                                    }
                                } else if (curCheckBox.Text.Equals( "Women B" )) {
                                    if (myFilterCmd.Length > 1) {
                                        myFilterCmd += " OR AgeGroup = 'BW'";
                                    } else {
                                        myFilterCmd = "AgeGroup = 'BW'";
                                    }
                                } else if (curCheckBox.Text.Equals( "Non Team" )) {
                                    if (myFilterCmd.Length > 1) {
                                        myFilterCmd += " OR AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
                                    } else {
                                        myFilterCmd = "AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
                                    }
                                } else {
                                    myFilterCmd = "";
                                }
                            } else {
                                if (myFilterCmd.Length > 1) {
                                    myFilterCmd += " OR EventGroup = '" + curCheckBox.Text + "'";
                                } else {
                                    myFilterCmd = "EventGroup = '" + curCheckBox.Text + "'";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ViewEditRoundButton_Click(object sender, EventArgs e) {
            String curEvent = "Slalom";
            if (slalomButton.Checked) {
                curEvent = "Slalom";
            } else if (trickButton.Checked) {
                curEvent = "Trick";
            } else if (jumpButton.Checked) {
                curEvent = "Jump";
            }

            RunningOrderRound curForm = new RunningOrderRound();
            curForm.MdiParent = this.MdiParent;
            curForm.RunningOrderForEvent(myTourRow, curEvent);
            curForm.Show();
        }

        private void checkSaveChanges() {
            if (isDataModified) {
                String dialogMsg = "Changes have been made to currently displayed data!"
                    + "\n Do you want to save the changes before closing the window?";
                DialogResult msgResp =
                    MessageBox.Show(dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                if (msgResp == DialogResult.Yes) {
                    try {
                        navSave_Click( null, null );
                    } catch (Exception excp) {
                        MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                    }
                } else if (msgResp == DialogResult.No) {
                    isDataModified = false;
                }
            }
        }

        private void EventRegDataGridView_Leave( object sender, EventArgs e ) {
            EventRegDataGridView.EndEdit();
        }

        private void EventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView myDataView = (DataGridView)sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
            if ( isDataModified && ( myViewIdx != e.RowIndex ) ) {
                try {
                    navSave_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( myViewIdx != e.RowIndex ) {
                    myViewIdx = e.RowIndex;
                }
            }
        }

        private void EventRegDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            if (EventRegDataGridView.Rows.Count > 0) {
                if (!(EventRegDataGridView.Columns[e.ColumnIndex].ReadOnly)) {
                    myViewIdx = e.RowIndex;
                    String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                    DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                    if ( curColName.Equals( "EventGroup" )
                        || curColName.Equals("TeamCode")
                        || curColName.Equals("EventClass")
                        || curColName.Equals("ReadyForPlcmt")
                        || curColName.Equals( "RankingRating" )
                        || curColName.Equals( "TrickBoat" )
                        || curColName.Equals( "JumpHeight" )
                        ) {
                        try {
                            myOrigItemValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
                        } catch {
                            myOrigItemValue = "";
                        }
                    } else if ( curColName.Equals( "RunOrder" ) ) {
                        try {
                            myOrigItemValue = Convert.ToInt16( (String)curViewRow.Cells[e.ColumnIndex].Value ).ToString();
                        } catch {
                            myOrigItemValue = "0";
                        }
                    } else if ( curColName.Equals( "HCapBase" )
                        || curColName.Equals( "HCapScore" )
                        || curColName.Equals( "RankingScore" )
                        ) {
                        try {
                            myOrigItemValue = Convert.ToDecimal( (String)curViewRow.Cells[e.ColumnIndex].Value ).ToString();
                        } catch {
                            myOrigItemValue = "0";
                        }
                    }
                }
            }
        }

        private void EventRegDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            String curMethodName = "EventRegDataGridView_CellValidating";
            if ( EventRegDataGridView.Rows.Count > 0 ) {
                myViewIdx = e.RowIndex;
                String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                if ( curColName.Equals( "EventClass" ) ) {
                    String curSkierClass = e.FormattedValue.ToString();
                    if (curSkierClass.Length > 1) curSkierClass = e.FormattedValue.ToString().Substring(0, 1);
                    if (mySkierClassList.compareClassChange( curSkierClass, myTourClass ) < 0) {
                        MessageBox.Show( "Class " + curSkierClass + " cannot be assigned to a skier in a class " + myTourClass + " tournament" );
                        e.Cancel = true;
                    }

                } else if (curColName.Equals( "EventGroup" )) {
                    String curValue = e.FormattedValue.ToString();
                    if (isObjectEmpty( curValue )) {
                        MessageBox.Show( "An event group is required" );
                        e.Cancel = true;
                    } else {
                        if (!( isObjectEmpty( myOrigItemValue ) )) {
                            String curEvent = "Slalom";
                            if (slalomButton.Checked) {
                                curEvent = "Slalom";
                            } else if (trickButton.Checked) {
                                curEvent = "Trick";
                            } else if (jumpButton.Checked) {
                                curEvent = "Jump";
                            }
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "SELECT Count(*) as GroupCount " );
                            curSqlStmt.Append( "FROM EventReg E " );
                            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                            curSqlStmt.Append( "  AND Event = '" + curEvent + "' " );
                            curSqlStmt.Append( "  AND EventGroup = '" + myOrigItemValue + "' " );
                            DataTable curDataTable = getData( curSqlStmt.ToString() );
                            if (curDataTable.Rows.Count > 0) {
                                int curReturnValue = (int)curDataTable.Rows[0]["GroupCount"];
                                if (curReturnValue < 2) {
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "SELECT Count(*) as AsgmtCount " );
                                    curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
                                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                                    curSqlStmt.Append( "  AND Event = '" + curEvent + "' " );
                                    curSqlStmt.Append( "  AND EventGroup = '" + myOrigItemValue + "' " );
                                    curDataTable = getData( curSqlStmt.ToString() );
                                    if (curDataTable.Rows.Count > 0) {
                                        curReturnValue = (int)curDataTable.Rows[0]["AsgmtCount"];
                                        if (curReturnValue > 0) {
                                            String dialogMsg = "This is the last skier in this event group that also has official assignments!"
                                                + "\n Do you want to change the official assignments to the new group?"
                                                + "\n If you don't change the assignments they could be lost when exporting event data!";
                                            DialogResult msgResp =
                                                MessageBox.Show( dialogMsg, "Warning",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Warning,
                                                    MessageBoxDefaultButton.Button1 );
                                            if (msgResp == DialogResult.Yes) {
                                                try {
                                                    curSqlStmt = new StringBuilder( "" );
                                                    curSqlStmt.Append( "Update OfficialWorkAsgmt Set " );
                                                    curSqlStmt.Append( "EventGroup = '" + curValue + "' " );
                                                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                                                    curSqlStmt.Append( "  AND Event = '" + curEvent + "' " );
                                                    curSqlStmt.Append( "  AND EventGroup = '" + myOrigItemValue + "' " );
                                                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                                } catch (Exception excp) {
                                                    MessageBox.Show( "Error updating official assignments \n" + excp.Message );
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                } else if (curColName.Equals( "RunOrder" )) {
                    try {
                        Int16 curNum = Convert.ToInt16( e.FormattedValue.ToString() );
                        e.Cancel = false;
                    } catch {
                        e.Cancel = true;
                        MessageBox.Show( curColName + " must be numeric" );
                    }
                } else if ( curColName.Equals( "HCapBase" )
                        || curColName.Equals( "HCapScore" )
                        || curColName.Equals( "RankingScore" )
                        ) {
                    try {
                        if (e.FormattedValue.ToString().Length > 0) {
                            Decimal curNum = Convert.ToDecimal( e.FormattedValue.ToString() );
                            e.Cancel = false;
                        } else {
                            e.Cancel = false;
                        }
                    } catch {
                        e.Cancel = true;
                        MessageBox.Show( curColName + " must be numeric" );
                    }
                }
            }
        }

        private void EventRegDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            if (EventRegDataGridView.Rows.Count > 0) {
                myViewIdx = e.RowIndex;
                String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                if (curColName.Equals("RunOrder")) {
                    if ( myOrigItemValue == null ) myOrigItemValue = "0";
                    if ( isObjectEmpty( curViewRow.Cells[e.ColumnIndex].Value ) ) {
                        if ( Convert.ToInt16( myOrigItemValue ) != 0 ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        Int16 curNum = Convert.ToInt16( (String)curViewRow.Cells[e.ColumnIndex].Value);
                        if ( curNum != Convert.ToInt16(myOrigItemValue) ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "EventGroup" )
                    || curColName.Equals( "TeamCode" )
                    || curColName.Equals( "EventClass" )
                    || curColName.Equals("ReadyForPlcmt")
                    || curColName.Equals( "RankingRating" )
                    || curColName.Equals( "TrickBoat" )
                    || curColName.Equals( "JumpHeight" )
                    ) {
                    if ( isObjectEmpty( curViewRow.Cells[e.ColumnIndex].Value ) ) {
                        if ( !(isObjectEmpty(myOrigItemValue)) ) {
                            if (curColName.Equals( "TrickBoat" ) || curColName.Equals( "JumpHeight" )) {
                                updateTourRegData( curViewRow, curColName, "" );
                            } else {
                                isDataModified = true;
                                curViewRow.Cells["Updated"].Value = "Y";
                            }
                        }
                    } else {
                        String curValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
                        if (curValue != myOrigItemValue) {
                            if ( curColName.Equals( "TrickBoat" ) || curColName.Equals( "JumpHeight" )) {
                                updateTourRegData( curViewRow,  curColName, curValue );
                            } else {
                                isDataModified = true;
                                curViewRow.Cells["Updated"].Value = "Y";
                            }
                        }
                    }
                } else if ( curColName.Equals( "HCapBase" )
                        || curColName.Equals( "HCapScore" )
                        || curColName.Equals( "RankingScore" )
                        ) {
                    if ( myOrigItemValue == null ) myOrigItemValue = "0";
                    if ( isObjectEmpty( curViewRow.Cells[e.ColumnIndex].Value ) ) {
                        if ( Convert.ToInt16( myOrigItemValue) != 0 ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        Decimal curNum = Convert.ToDecimal( (String)curViewRow.Cells[e.ColumnIndex].Value );
                        if ( myOrigItemValue == null ) myOrigItemValue = "0";
                        if ( curNum != Convert.ToDecimal( myOrigItemValue ) ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }
                }
            }
        }

        private void navPrint_Click(object sender, EventArgs e) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            //Font saveShowDefaultCellStyle = PrintDataGridView.DefaultCellStyle.Font;
            //PrintDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 12, FontStyle.Regular);

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 12, FontStyle.Bold);
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if(curPrintDialog.ShowDialog() == DialogResult.OK) {
                RankingScore.HeaderText = "NRS";
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + this.Text;
                myPrintDoc = new PrintDocument();   
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 30, 30, 30, 30 );
                myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
                        CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                if (printHeaderNote.Text.Length > 0) {
                    myPrintDataGrid.SubtitleList();
                    Font fontPrintSubTitle = new Font( "Arial", 12, FontStyle.Bold );
                    StringFormat SubtitleStringFormat = new StringFormat();
                    SubtitleStringFormat.Trimming = StringTrimming.Word;
                    SubtitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    SubtitleStringFormat.Alignment = StringAlignment.Center;
                    StringRowPrinter curSubtitle = new StringRowPrinter( printHeaderNote.Text, 0, 0, 750, 25
                        , Color.DarkBlue, Color.LightGray, fontPrintSubTitle, SubtitleStringFormat );
                    myPrintDataGrid.SubtitleRow = curSubtitle;
                }

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
                RankingScore.HeaderText = "Ranking Score";
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if(more == true)
                e.HasMorePages = true;
        }

        private void navPrintFormButton_Click(object sender, EventArgs e) {
            if (slalomButton.Checked) {
                PrintDialog curPrintDialog = new PrintDialog();
                PrintOfficialFormDialog curDialog = new PrintOfficialFormDialog();
                if (curDialog.ShowDialog() == DialogResult.OK) {
                    String curPrintReport = curDialog.ReportName;
                    if (curPrintReport.Equals( "SlalomBoatJudgeForm" )) {
                        PrintSlalomBoatJudgeForms curPrintForm = new PrintSlalomBoatJudgeForms();
                        curPrintForm.PrintLandscape = true;
                        curPrintForm.ReportHeader = printHeaderNote.Text;
                        curPrintForm.DivInfoDataTable = getSlalomDivMaxMinSpeed();
                        curPrintForm.TourRules = myTourRules;
                        curPrintForm.TourName = (String)myTourRow["Name"];

                        myEventRegDataTable.DefaultView.Sort = mySortCmd;
                        myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                        curPrintForm.ShowDataTable = myEventRegDataTable.DefaultView.ToTable();

                        curPrintForm.Print();
                    } else if (curPrintReport.Equals( "SlalomRecapForm" )) {
                        PrintSlalomRecapForm curPrintForm = new PrintSlalomRecapForm();
                        curPrintForm.PrintLandscape = true;
                        curPrintForm.ReportHeader = printHeaderNote.Text;
                        curPrintForm.DivInfoDataTable = getSlalomDivMaxMinSpeed();
                        curPrintForm.TourRules = myTourRules;
                        curPrintForm.TourName = (String)myTourRow["Name"];

                        myEventRegDataTable.DefaultView.Sort = mySortCmd;
                        myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                        curPrintForm.ShowDataTable = myEventRegDataTable.DefaultView.ToTable();

                        curPrintForm.Print();
                    }
                }
            } else if (jumpButton.Checked) {
                PrintJumpRecapJudgeForm curPrintForm = new PrintJumpRecapJudgeForm();
                curPrintForm.PrintLandscape = true;
                curPrintForm.ReportHeader = printHeaderNote.Text;
                curPrintForm.DivInfoDataTable = getJumpDivMaxSpeedRamp();
                curPrintForm.TourRules = myTourRules;
                curPrintForm.TourName = (String)myTourRow["Name"];

                myEventRegDataTable.DefaultView.Sort = mySortCmd;
                myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                curPrintForm.ShowDataTable = myEventRegDataTable.DefaultView.ToTable();

                curPrintForm.Print();
            } else if (trickButton.Checked) {
                PrintTrickJudgeForm curPrintForm = new PrintTrickJudgeForm();
                curPrintForm.PrintLandscape = true;
                curPrintForm.ReportHeader = printHeaderNote.Text;
                curPrintForm.DivInfoDataTable = getTrickDivList();
                curPrintForm.TourName = (String)myTourRow["Name"];
                curPrintForm.TourRules = myTourRules;
                curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
                curPrintForm.NumJudges = 3;
                String dialogMsg = "Do you want to include a trick timing form?";
                DialogResult msgResp =
                    MessageBox.Show(dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                if ( msgResp == DialogResult.Yes ) {
                    curPrintForm.NumJudges = 4;
                }


                myEventRegDataTable.DefaultView.Sort = mySortCmd;
                myEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                curPrintForm.ShowDataTable = myEventRegDataTable.DefaultView.ToTable();

                curPrintForm.Print();
            }
        }

        private void navLiveWeb_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            ExportLiveWeb.LiveWebDialog.WebLocation = ExportLiveWeb.LiveWebLocation;
            ExportLiveWeb.LiveWebDialog.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if (ExportLiveWeb.LiveWebDialog.DialogResult == DialogResult.OK) {
                if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Set" )) {
                    ExportLiveWeb.LiveWebLocation = ExportLiveWeb.LiveWebDialog.WebLocation;
                    ExportLiveWeb.exportTourData( mySanctionNum );
                    LiveWebLabel.Visible = true;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "TwitterActive" )) {
                    ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterDefaultAccount;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "TwitterAuth" )) {
                    ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterRequestTokenURL;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Disable" )) {
                    ExportLiveWeb.LiveWebLocation = "";
                    ExportLiveTwitter.TwitterLocation = "";
                    LiveWebLabel.Visible = false;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Resend" ) || ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "ResendAll" )) {
                    if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                        String curEvent = "Slalom";
                        if (slalomButton.Checked) {
                            curEvent = "Slalom";
                        } else if (trickButton.Checked) {
                            curEvent = "Trick";
                        } else if (jumpButton.Checked) {
                            curEvent = "Jump";
                        }
                        List<String> curDivList = new List<String>();
                        foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                            if (curCheckBox.Text.Equals( "All" )) {
                                if (curCheckBox.Checked) {
                                    curDivList.Add( curCheckBox.Text );
                                }
                            }
                        }
                        if (curDivList.Count == 0) {
                            foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                                if (curCheckBox.Checked) {
                                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                                        if (curCheckBox.Text.Equals( "Men A" )) {
                                            curDivList.Add( "CM" );
                                        } else if (curCheckBox.Text.Equals( "Women A" )) {
                                            curDivList.Add( "CW" );
                                        } else if (curCheckBox.Text.Equals( "Men B" )) {
                                            curDivList.Add( "BM" );
                                        } else if (curCheckBox.Text.Equals( "Women B" )) {
                                            curDivList.Add( "BW" );
                                        } else if (curCheckBox.Text.Equals( "Non Team" )) {
                                            curDivList.Add( curCheckBox.Text );
                                        }
                                    } else {
                                        curDivList.Add( curCheckBox.Text );
                                    }
                                }
                            }
                        }
                        foreach (String curEventGroup in curDivList) {
                            ExportLiveWeb.exportCurrentSkiersRunOrder( curEvent, mySanctionNum, 0, curEventGroup );
                        }
                    }
                }
            }
        }

        private bool updateTourRegData(DataGridViewRow inViewRow, String inColName, String inValue) {
            String curMethodName = "updateTourRegData";
            bool curReturnValue = true;

            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourReg Set " );
                curSqlStmt.Append( inColName + " = '" + inValue + "'" );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
                curSqlStmt.Append( "Where SanctionId = '" + (String)inViewRow.Cells["SanctionId"].Value + "' " );
                curSqlStmt.Append( "And MemberId = '" + (String)inViewRow.Cells["MemberId"].Value + "' " );
                curSqlStmt.Append( "And AgeGroup = '" + (String)inViewRow.Cells["AgeGroup"].Value + "' " );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
            } catch (Exception excp) {
                curReturnValue = false;
                String curMsg = "Error attempting to update skier " + inColName + " information \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return curReturnValue;
        }

        private DataTable getTourData(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation " );
            curSqlStmt.Append(", HcapSlalomBase, HcapSlalomPct, HcapTrickBase, HcapTrickPct, HcapJumpBase, HcapJumpPct ");
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getEventRegData() {
            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup, E.RunOrder, E.Rotation, " );
            curSqlStmt.Append( "E.TeamCode, COALESCE(E.EventClass, '" + myTourEventClass + "') as EventClass, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt");
            curSqlStmt.Append( ", E.RankingScore, E.RankingRating, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder " );
            curSqlStmt.Append( ", HCapBase, HcapScore, T.State, T.City " );
            if (slalomButton.Checked) {
            } else if (trickButton.Checked) {
                curSqlStmt.Append( ", T.TrickBoat " );
            } else if (jumpButton.Checked) {
                curSqlStmt.Append( ", T.JumpHeight " );
            }
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
            curSqlStmt.Append( "     LEFT OUTER JOIN CodeValueList L ON L.ListCode = E.EventClass AND ListName = 'Class' " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + curEvent + "'" );

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSlalomDivMaxMinSpeed() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS MinSpeed " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSASlalomMax', 'IwwfSlalomMax', 'NcwsaSlalomMax') " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('IwwfSlalomMin', 'NcwsaSlalomMin') ");
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getJumpDivMaxSpeedRamp() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS RampHeight " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSAJumpMax', 'IwwfJumpMax', 'NcwsaJumpMax') " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('AWSARampMax', 'IwwfRampMax', 'NcwsaRampMax') " );
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTrickDivList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private StreamWriter getExportFile( String inFileName ) {
            String myFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = curFileFilter;
            myFileDialog.FilterIndex = 1;
            if ( inFileName == null ) {
                myFileDialog.FileName = "";
            } else {
                myFileDialog.FileName = inFileName;
            }

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if ( curFileName.IndexOf( '.' ) < 0 ) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split( '|' );
                            if ( curList.Length > 0 ) {
                                int curDelim = curList[1].IndexOf( "." );
                                if ( curDelim > 0 ) {
                                    curDefaultExt = curList[1].Substring( curDelim - 1 );
                                }
                            }
                            myFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

    }
}
