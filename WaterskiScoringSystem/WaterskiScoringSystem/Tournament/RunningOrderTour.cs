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
		private String myOrigGroupFilterValue = "";
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
		private Dictionary<string, Boolean> myRunningOrderColumnFilter;

		private TourProperties myTourProperties;
        private SortDialogForm sortDialogForm;
        private ListSkierClass mySkierClassList;
		private ColumnSelectDialogcs columnSelectDialogcs;
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

				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterSlalom;

			} else if ( inEvent.Equals( "Trick" ) ) {
                mySortCmd = myTourProperties.RunningOrderSortTrick;
                trickButton.Checked = true;

				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterTrick;

			} else if ( inEvent.Equals( "Jump" ) ) {
                mySortCmd = myTourProperties.RunningOrderSortJump;
                jumpButton.Checked = true;

				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterJump;
			}
			getRunningOrderColumnfilter();
			loadGroupFilterComboBox();
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

            String[] curList = { "SkierName", "EventGroup", "Div", "DivOrder", "RunOrder", "TeamCode", "EventClass", "ReadyForPlcmt", "RankingScore", "RankingRating", "JumpHeight", "TrickBoat", "HCapBase", "HCapScore" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;

			if ( mySanctionNum == null) {
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
                } else {
                    e.Cancel = false;
                }
            } else {
                e.Cancel = false;
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
				EventRegDataGridView.Rows.Clear();
				//Retrieve running order data for display
				myEventRegDataTable = getEventRegData();
				if ( myEventRegDataTable.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in myEventRegDataTable.Rows ) {
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

			PrintHcapBase.Visible = false;
			PrintRankingRating.Visible = false;
			PrintReadyForPlcmt.Visible = false;
			PrintRunOrder.Visible = false;
			PrintHCapScore.Visible = false;

			try {
                if ( myEventRegDataTable.Rows.Count > 0 ) {
                    PrintDataGridView.Rows.Clear();
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

                    foreach ( DataRow curTeamRow in myEventRegDataTable.Rows ) {
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
						if ( PrintEventRotation.Visible ) {
							try {
								curPrintRow.Cells["PrintEventRotation"].Value = (String)curTeamRow["EventGroup"];
							} catch {
								curPrintRow.Cells["PrintEventRotation"].Value = "";
							}
						} else {
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

			Cursor.Current = Cursors.Default;

		}

		private void loadGroupSelectList(String inEvent, EventHandler parentEvent) {
            CheckBox curCheckBox;
            int curItemLoc = 5;
            int curItemSize = 0;
            ArrayList curEventGroupList = new ArrayList();

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
                        DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
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

		private void navColumnSelect_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			columnSelectDialogcs.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( columnSelectDialogcs.DialogResult == DialogResult.OK ) {
				DataTable curPrintColumnSelectList = columnSelectDialogcs.ColumnList;
				foreach ( DataGridViewColumn curViewCol in this.PrintDataGridView.Columns ) {
					foreach ( DataRow curColSelect in curPrintColumnSelectList.Rows ) {
						if ( ( (String) curColSelect["Name"] ).Equals( (String) curViewCol.Name ) ) {
							curViewCol.Visible = (Boolean) curColSelect["Visible"];

							if ( myRunningOrderColumnFilter.ContainsKey( (String) curViewCol.Name ) ) {
								myRunningOrderColumnFilter[(String) curViewCol.Name] = curViewCol.Visible;
							} else { 
								myRunningOrderColumnFilter.Add( (String) curViewCol.Name, curViewCol.Visible );
							}
							break;
                        }
					}
				}
				if ( slalomButton.Checked) {
					this.myTourProperties.RunningOrderColumnFilterSlalom = myRunningOrderColumnFilter;

				} else if ( trickButton.Checked) {
					this.myTourProperties.RunningOrderColumnFilterTrick = myRunningOrderColumnFilter;

				} else if ( jumpButton.Checked) {
					this.myTourProperties.RunningOrderColumnFilterJump = myRunningOrderColumnFilter;
				}
				winStatusMsg.Text = "Selected columns to view on print";
			}
		}

		private void navExport_Click(object sender, EventArgs e) {
			String curEvent = getCurrentEvent();

			ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[5];
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "DivOrder", "OfficialWork" };
            String curFilterCmd = myFilterCmd;
            if ( curFilterCmd.Contains("Div =") ) {
                curFilterCmd = curFilterCmd.Replace("Div =", "XT.AgeGroup =");
            } else if ( curFilterCmd.Contains("AgeGroup =") ) {
                curFilterCmd = curFilterCmd.Replace("AgeGroup =", "XT.AgeGroup =");
            }

            curSelectCommand[0] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = '" + curEvent + "' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[0] = curSelectCommand[0] + "And " + curFilterCmd + " ";
            }

            curSelectCommand[1] = "Select * from EventReg XT ";
            if ( isObjectEmpty(curFilterCmd) ) {
                curSelectCommand[1] = curSelectCommand[1]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = '" + curEvent + "'";
            } else {
                if ( curFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'"
                        + " And " + curFilterCmd;
                } else {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'";
                }
            }

            curSelectCommand[2] = "Select * from EventRunOrder XT ";
            if (isObjectEmpty(curFilterCmd) ) {
                curSelectCommand[2] = curSelectCommand[2]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = '" + curEvent + "'";
            } else {
                if ( curFilterCmd.Length > 0) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = '" + curEvent + "'"
                        + " And " + curFilterCmd;
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
			String curEvent = getCurrentEvent();

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
			String curEvent = getCurrentEvent();

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

        private void navExportSplashEye_Click(object sender, EventArgs e) {
            String curJumpHeight, curTrickBoat;
            String curAgeGroup = "", curEventGroup = "", prevEventGroup = "", prevAgeGroup = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            try {
                String curTourName = (String)myTourRow["Name"];
				String curEvent = getCurrentEvent();
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
			String curEvent = getCurrentEvent();
			ExportData myExportData = new ExportData();
			myEventRegDataTable = getEventRegData();
			loadPrintDataGrid();
			myExportData.exportData( PrintDataGridView, curEvent + "RunOrderList.txt");
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

		private void loadGroupFilterComboBox() {
			String curEvent = "Slalom";
			if ( slalomButton.Checked ) {
				curEvent = "Slalom";
			} else if ( trickButton.Checked ) {
				curEvent = "Trick";
			} else if ( jumpButton.Checked ) {
				curEvent = "Jump";
			}

			ArrayList curDropdownList = new ArrayList();
			curDropdownList.Add( "** New Entry **" );
			String curListCode, curCodeValue;
			GroupFilterComboBox.DisplayMember = "ItemName";
			GroupFilterComboBox.ValueMember = "ItemValue";

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select FilterName " );
			curSqlStmt.Append( "From EventRunOrderFilters " );
			curSqlStmt.Append( "Where SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
			curSqlStmt.Append( "Order by FilterName " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			foreach ( DataRow curRow in curDataTable.Rows ) {
				curListCode = (String) curRow["FilterName"];
				curCodeValue = (String) curRow["FilterName"];
				curDropdownList.Add( curCodeValue );
			}
			GroupFilterComboBox.DataSource = curDropdownList;
			myOrigGroupFilterValue = "** New Entry **";
			GroupFilterComboBox.SelectedIndex = 0;
        }

		private void GroupFilterComboBox_SelectedValueChanged( object sender, EventArgs e ) {
			if ( ( (ComboBox) sender ).Items.Count == 0 ) return;
			try {
				String curFilterName = ( (ComboBox) sender ).SelectedValue.ToString();
				if ( curFilterName.Length == 0 ) return;
				if ( curFilterName.Equals( "** New Entry **" ) ) return;
				if ( curFilterName.Equals( myOrigGroupFilterValue ) ) return;

				String curEvent = getCurrentEvent();

				StringBuilder curSqlStmt = new StringBuilder( "Select FilterName, PrintTitle, GroupFilterCriteria " );
				curSqlStmt.Append( "From EventRunOrderFilters " );
				curSqlStmt.Append( "Where SanctionId = '" + this.mySanctionNum + "' " );
				curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
				curSqlStmt.Append( "AND FilterName = '" + curFilterName + "' " );
				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) {
					foreach ( CheckBox curCheckBox in EventGroupPanel.Controls ) {
						curCheckBox.Checked = false;
					}

					this.printHeaderNote.Text = (String) curDataTable.Rows[0]["PrintTitle"];
					myFilterCmd = (String) curDataTable.Rows[0]["GroupFilterCriteria"];
					setGroupFilters( myFilterCmd );
					myOrigGroupFilterValue = curFilterName;

					loadEventRegView();

				} else {
					MessageBox.Show( curFilterName + " Not Found" );
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "GroupFilterComboBox_SelectedValueChanged: Exception encountered: " + ex.Message );
			}
		}

		private void GroupFilterComboBox_Validated( object sender, EventArgs e ) {
			if ( ( (ComboBox) sender ).Items.Count == 0 ) return;

			try {
				if ( ( (ComboBox) sender ).SelectedIndex < 0 ) {
					String curFilterName = ( (ComboBox) sender ).Text;

					if ( curFilterName.Length == 0 ) {
						if ( curFilterName.Length == 0 ) return;
						if ( curFilterName.Equals( "** New Entry **" ) ) return;
					} else if ( curFilterName.Equals( "** New Entry **" ) ) return;

					StringBuilder curSqlStmt = new StringBuilder( "" );
					String curEvent = getCurrentEvent();

					String curPrintTitle = encodeDataForSql( this.printHeaderNote.Text );
					myFilterCmd = buildFilterCmd();
					if ( myOrigGroupFilterValue.Equals( "** New Entry **" ) ) myOrigGroupFilterValue = curFilterName;

					curSqlStmt = new StringBuilder( "Select SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria " );
					curSqlStmt.Append( "From EventRunOrderFilters " );
					curSqlStmt.Append( "Where SanctionId = '" + this.mySanctionNum + "' " );
					curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
					curSqlStmt.Append( "AND FilterName = '" + myOrigGroupFilterValue + "' " );
					DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
					curSqlStmt = new StringBuilder( "" );
					if ( curDataTable.Rows.Count > 0 ) {
						curSqlStmt.Append( "Update EventRunOrderFilters " );
						curSqlStmt.Append( "Set GroupFilterCriteria = '" + encodeDataForSql(myFilterCmd) + "' " );
						curSqlStmt.Append( ", PrintTitle = '" + curPrintTitle + "' " );
						curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
						curSqlStmt.Append( ", FilterName = '" + curFilterName + "' " );
						curSqlStmt.Append( "Where SanctionId = '" + this.mySanctionNum + "' " );
						curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
						curSqlStmt.Append( "AND FilterName = '" + myOrigGroupFilterValue + "' " );

					} else {
						curSqlStmt.Append( "Insert Into EventRunOrderFilters (" );
						curSqlStmt.Append( "SanctionId, Event, FilterName, PrintTitle, GroupFilterCriteria " );
						curSqlStmt.Append( ") Values ( " );
						curSqlStmt.Append( "'" + this.mySanctionNum + "', '" + curEvent + "', '" + curFilterName + "', '" + curPrintTitle + "', '" + encodeDataForSql( myFilterCmd ) + "' " );
						curSqlStmt.Append( ")" );
					}
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					myOrigGroupFilterValue = curFilterName;
                    loadGroupFilterComboBox();
					GroupFilterComboBox.SelectedItem = curFilterName;

					loadEventRegView();
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "GroupFilterComboBox_Validating: Exception encountered: " + ex.Message );
			}

		}

		private void DeleteFilterButton_Click( object sender, EventArgs e ) {
			if ( GroupFilterComboBox.Items.Count == 0 ) return;
			try {
				String curFilterName = GroupFilterComboBox.SelectedValue.ToString();
				if ( curFilterName.Length == 0 ) return;
				if ( curFilterName.Equals( "** New Entry **" ) ) return;

				String curEvent = getCurrentEvent();
				StringBuilder curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Delete From EventRunOrderFilters " );
				curSqlStmt.Append( "Where SanctionId = '" + this.mySanctionNum + "' " );
				curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
				curSqlStmt.Append( "AND FilterName = '" + curFilterName + "' " );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				loadGroupFilterComboBox();

			} catch ( Exception ex ) {
				MessageBox.Show( "DeleteFilterButton_Click: Exception encountered: " + ex.Message );
			}
		}

		private void setGroupFilters(String inFilterCmd) {
			int curDelimIdx = 0, curStartPos = 0;
			while (true) {
				curDelimIdx = inFilterCmd.IndexOf( " OR ", curStartPos );
				if ( curDelimIdx < 0 ) {
					findSetGroupFilterCB( inFilterCmd.Substring( curStartPos ));
					break;

				} else {
					findSetGroupFilterCB( inFilterCmd.Substring( curStartPos, curDelimIdx - curStartPos ) );
                    curStartPos += curDelimIdx - curStartPos + 4;
                }
			}
		}

		private void findSetGroupFilterCB(String curFoundCond ) {
			int curDelimIdx1 = curFoundCond.IndexOf( "'" );
			int curDelimIdx2 = curFoundCond.IndexOf( "'", curDelimIdx1 + 1 );
			String curGroup = curFoundCond.Substring( curDelimIdx1 + 1, curDelimIdx2 - curDelimIdx1 - 1 );
			if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
				if ( curGroup.Equals("CM") ) curGroup = "Men A";
				else if ( curGroup.Equals( "CW" ) ) curGroup = "Women A";
				else if ( curGroup.Equals( "BM" ) ) curGroup = "Men B";
				else if ( curGroup.Equals( "BW" ) ) curGroup = "Women B";
				else curGroup = "Non Team";
			}
			setGroupFilterCB( "Group" + curGroup + "_CB", true );
		}

		private void setGroupFilterCB(String cbName, bool cbValue ) {
			Control[] foundControls = EventGroupPanel.Controls.Find( cbName, true );
			if ( foundControls.Length == 0 ) return;
			CheckBox groupFilterCB = (CheckBox) foundControls[0];
			groupFilterCB.Checked = cbValue;
			return;
		}

		private void navRefresh_Click(object sender, EventArgs e) {
            if ( isDataModified ) {
                checkModifyPrompt();
            }
            if (!(isDataModified)) {
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

					getRunningOrderColumnfilter();

					loadGroupFilterComboBox();

					winStatusMsg.Text = "Sorted by " + mySortCmd;
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

					getRunningOrderColumnfilter();

					loadGroupFilterComboBox();

					winStatusMsg.Text = "Sorted by " + mySortCmd;
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

					getRunningOrderColumnfilter();

					loadGroupFilterComboBox();

					winStatusMsg.Text = "Sorted by " + mySortCmd;
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
				myFilterCmd = "";

			} else {
                foreach (CheckBox curCheckBox in EventGroupPanel.Controls) {
                    if (curCheckBox.Text.Equals( "All" )) {
                        curCheckBox.Checked = false;
                    } else {
						myFilterCmd = buildFilterCmd();
                    }
                }
            }

			myOrigGroupFilterValue = "** New Entry **";
			GroupFilterComboBox.SelectedIndex = 0;
		}

		private void ViewEditRoundButton_Click(object sender, EventArgs e) {
			String curEvent = getCurrentEvent();
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
							String curEvent = getCurrentEvent();
							StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "SELECT Count(*) as GroupCount " );
                            curSqlStmt.Append( "FROM EventReg E " );
                            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                            curSqlStmt.Append( "  AND Event = '" + curEvent + "' " );
                            curSqlStmt.Append( "  AND EventGroup = '" + myOrigItemValue + "' " );
                            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                            if (curDataTable.Rows.Count > 0) {
                                int curReturnValue = (int)curDataTable.Rows[0]["GroupCount"];
                                if (curReturnValue < 2) {
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "SELECT Count(*) as AsgmtCount " );
                                    curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
                                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                                    curSqlStmt.Append( "  AND Event = '" + curEvent + "' " );
                                    curSqlStmt.Append( "  AND EventGroup = '" + myOrigItemValue + "' " );
                                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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

			myEventRegDataTable = getEventRegData();
			loadPrintDataGrid();

			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

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

                        myEventRegDataTable = getEventRegData();
                        curPrintForm.ShowDataTable = myEventRegDataTable;

                        curPrintForm.Print();
                    } else if (curPrintReport.Equals( "SlalomRecapForm" )) {
                        PrintSlalomRecapForm curPrintForm = new PrintSlalomRecapForm();
                        curPrintForm.PrintLandscape = true;
                        curPrintForm.ReportHeader = printHeaderNote.Text;
                        curPrintForm.DivInfoDataTable = getSlalomDivMaxMinSpeed();
                        curPrintForm.TourRules = myTourRules;
                        curPrintForm.TourName = (String)myTourRow["Name"];

                        myEventRegDataTable = getEventRegData();
                        curPrintForm.ShowDataTable = myEventRegDataTable;

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

                myEventRegDataTable = getEventRegData();
                curPrintForm.ShowDataTable = myEventRegDataTable;

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

                myEventRegDataTable = getEventRegData();
                curPrintForm.ShowDataTable = myEventRegDataTable;

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
					if ( ExportLiveWeb.exportTourData( mySanctionNum ) ) {
						LiveWebLabel.Visible = true;

					} else {
						ExportLiveWeb.LiveWebLocation = "";
						ExportLiveTwitter.TwitterLocation = "";
						LiveWebLabel.Visible = false;
					}

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
						String curEvent = getCurrentEvent();
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
									curDivList.Add( curCheckBox.Text );
								}
							}
                        }
						foreach ( String curEventGroup in curDivList) {
                            if ( ExportLiveWeb.exportCurrentSkiersRunOrder( myTourRules, curEvent, mySanctionNum, 0, curEventGroup ) ) {
								continue;
							} else {
								MessageBox.Show( "Error encountered sending running order data for Group " + curEventGroup + ".  Terminating request" );
								break;
							}
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

		private String buildFilterCmd() {
			String curFilterCmd = "", curGroupFilter = "";

			foreach ( CheckBox curCheckBox in EventGroupPanel.Controls ) {
				if ( !(curCheckBox.Text.Equals( "All" )) ) {
					if ( curCheckBox.Checked ) {
						if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
							if ( curCheckBox.Text.Equals( "Men A" ) ) {
								curGroupFilter = " = 'CM'";
							} else if ( curCheckBox.Text.Equals( "Women A" ) ) {
								curGroupFilter = " = 'CW'";
							} else if ( curCheckBox.Text.Equals( "Men B" ) ) {
								curGroupFilter = " = 'BM'";
							} else if ( curCheckBox.Text.Equals( "Women B" ) ) {
								curGroupFilter = " = 'BW'";
							} else if ( curCheckBox.Text.Equals( "Non Team" ) ) {
								curGroupFilter = " not in ('CM', 'CW', 'BM', 'BW') ";
							}
							if ( curFilterCmd.Length > 1 ) {
								curFilterCmd += " OR E.AgeGroup " + curGroupFilter;
							} else {
								curFilterCmd = "E.AgeGroup"  + curGroupFilter;
							}

						} else {
							if ( curFilterCmd.Length > 1 ) {
								curFilterCmd += " OR EventGroup = '" + curCheckBox.Text + "'";
							} else {
								curFilterCmd = "EventGroup = '" + curCheckBox.Text + "'";
							}
						}
					}
				}
			}

			return curFilterCmd;
        }

		private DataTable buildPrintColumnList() {
			/* **********************************************************
             * Build data tabale definition containing the list of columns 
			 * on the data grid used to print data 
             * ******************************************************* */
			DataTable curDataTable = new DataTable();

			DataColumn curCol = new DataColumn();
			curCol.ColumnName = "Name";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = true;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "Visible";
			curCol.DataType = System.Type.GetType( "System.Boolean" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = 1;
			curDataTable.Columns.Add( curCol );

			return curDataTable;
		}

		private void getRunningOrderColumnfilter() {
			if ( slalomButton.Checked ) {
				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterSlalom;

			} else if ( trickButton.Checked ) {
				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterTrick;

			} else if ( jumpButton.Checked ) {
				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterJump;

			} else {
				myRunningOrderColumnFilter = myTourProperties.RunningOrderColumnFilterSlalom;

			}

			DataRowView newRow;
			DataTable curPrintColumnSelectList = buildPrintColumnList();
			foreach ( DataGridViewColumn curCol in this.PrintDataGridView.Columns ) {
				newRow = curPrintColumnSelectList.DefaultView.AddNew();
				newRow["Name"] = curCol.Name;
				if ( myRunningOrderColumnFilter.ContainsKey( curCol.Name ) ) {
					newRow["Visible"] = myRunningOrderColumnFilter[curCol.Name];

				} else {
					if ( myRunningOrderColumnFilter.ContainsKey( curCol.Name ) ) {
						newRow["Visible"] = (bool) myRunningOrderColumnFilter[curCol.Name];
					} else {
						newRow["Visible"] = curCol.Visible;
					}
				}
				newRow.EndEdit();
			}
			columnSelectDialogcs = new ColumnSelectDialogcs();
			columnSelectDialogcs.ColumnList = curPrintColumnSelectList;

		}

		private String getCurrentEvent() {
			String curEvent = "Slalom";
			if ( slalomButton.Checked ) return "Slalom";
			if ( trickButton.Checked ) return "Trick";
			if ( jumpButton.Checked ) return "Jump";
			return "Slalom";
		}

		private DataTable getTourData(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation " );
            curSqlStmt.Append(", HcapSlalomBase, HcapSlalomPct, HcapTrickBase, HcapTrickPct, HcapJumpBase, HcapJumpPct ");
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getEventRegData() {
			String curEvent = getCurrentEvent();

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
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + curEvent + "' " );
			if ( myFilterCmd.Length > 0 ) curSqlStmt.Append( "AND ( " + myFilterCmd + " ) ");
			if ( mySortCmd.Length > 0 ) curSqlStmt.Append( "ORDER BY " + mySortCmd );

			DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
			//curDataTable.DefaultView.Sort = mySortCmd;
			//return curDataTable.DefaultView.ToTable();
			return curDataTable;
        }

        private DataTable getSlalomDivMaxMinSpeed() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS MinSpeed " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSASlalomMax', 'IwwfSlalomMax', 'NcwsaSlalomMax') " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('IwwfSlalomMin', 'NcwsaSlalomMin') ");
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getJumpDivMaxSpeedRamp() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS RampHeight " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSAJumpMax', 'IwwfJumpMax', 'NcwsaJumpMax') " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('AWSARampMax', 'IwwfRampMax', 'NcwsaRampMax') " );
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTrickDivList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "WHERE L1.ListName LIKE '%AgeGroup' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
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

		private String encodeDataForSql( String inValue ) {
			String curValue = inValue.Replace( "'", "''" );
			return curValue;
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
