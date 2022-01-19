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
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tournament {
    public partial class RunningOrderRound : Form {
        #region instance variables
        private Boolean isDataModified = false;
        private int myViewIdx;
        private String myWindowTitle;
        private String mySanctionNum;
        private String myOrigItemValue = "";
        private String myTourRules = "";
        private String myEvent = "";
        private String mySortCmd = "";
        private String myPlcmtOrg = "";
        private String myCommandType = "";
        private Int16 myRounds = 0;
        private int myEventRegViewIdx = 0;

        private DataRow myTourRow;

        private RunOrderCopyDialogForm myRunOrderCopyDialog;
        private RunOrderElimDialogForm myRunOrderElimDialog;
        private CalcScoreSummary myCalcSummary;
        private DataGridViewPrinter myPrintDataGrid;
        private PrintDocument myPrintDoc;
        private TourProperties myTourProperties;
        #endregion

        public RunningOrderRound() {
            InitializeComponent();
            myWindowTitle = this.Text;
        }

        public void RunningOrderForEvent(DataRow inTourRow, String inEvent) {
            myTourProperties = TourProperties.Instance;
            myTourRow = inTourRow;
            myEvent = inEvent;
            if (inEvent.Equals("Slalom")) {
                this.Text = myWindowTitle + " - Slalom";
                mySortCmd = myTourProperties.RunningOrderSortSlalom;
            } else if (inEvent.Equals( "Trick" )) {
                this.Text = myWindowTitle + " - Trick";
                mySortCmd = myTourProperties.RunningOrderSortTrick;
            } else if (inEvent.Equals( "Jump" )) {
                this.Text = myWindowTitle + " - Jump";
                mySortCmd = myTourProperties.RunningOrderSortJump;
            }
            myTourRules = (String)myTourRow["Rules"];

            if (myTourRow["SlalomRounds"] == DBNull.Value) {
                myTourRow["SlalomRounds"] = 0;
            }
            if (myTourRow["TrickRounds"] == DBNull.Value) {
                myTourRow["TrickRounds"] = 0;
            }
            if (myTourRow["JumpRounds"] == DBNull.Value) {
                myTourRow["JumpRounds"] = 0;
            }
            if ( myEvent.ToLower().Equals("slalom") ) {
                myRounds = Convert.ToInt16(myTourRow["SlalomRounds"]);
            } else if (myEvent.ToLower().Equals("trick")) {
                myRounds = Convert.ToInt16(myTourRow["TrickRounds"]);
            } else if (myEvent.ToLower().Equals("jump")) {
                myRounds = Convert.ToInt16(myTourRow["JumpRounds"]);
            }

            //Load round selection list based on number of rounds specified for the tournament
            roundActiveSelect.SelectList_LoadHorztl( myRounds.ToString(), roundActiveSelect_Click, false );
            roundActiveSelect.RoundValue = "1";
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

			WaterskiConnectLabel.Visible = false;
			SendSkierListButton.Visible = false;
			SendSkierListButton.Enabled = false;
			if ( WscHandler.isConnectActive ) {
				WaterskiConnectLabel.Visible = true;
				SendSkierListButton.Visible = true;
				SendSkierListButton.Enabled = true;
			}
		}

		private void RunningOrderRound_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.RunningOrderRound_Width > 0) {
                this.Width = Properties.Settings.Default.RunningOrderRound_Width;
            }
            if (Properties.Settings.Default.RunningOrderRound_Height > 0) {
                this.Height = Properties.Settings.Default.RunningOrderRound_Height;
            }
            if (Properties.Settings.Default.RunningOrder_Location.X > 0
                && Properties.Settings.Default.RunningOrderRound_Location.Y > 0) {
                    this.Location = Properties.Settings.Default.RunningOrderRound_Location;
            }

            myTourProperties = TourProperties.Instance;

            myRunOrderCopyDialog = new RunOrderCopyDialogForm();
            myRunOrderElimDialog = new RunOrderElimDialogForm();
            myCalcSummary = new CalcScoreSummary();

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            isDataModified = false;
        }

        private void RunningOrderRound_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.RunningOrderRound_Width = this.Size.Width;
                Properties.Settings.Default.RunningOrderRound_Height = this.Size.Height;
                Properties.Settings.Default.RunningOrderRound_Location = this.Location;
            }
        }

        private void RunningOrderRound_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                checkModifyPrompt();
                if (isDataModified) {
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

        private void checkModifyPrompt() {
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
                        navSave_Click(null, null);
                    } catch (Exception excp) {
                        MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                    }
                } else if (msgResp == DialogResult.No) {
                    isDataModified = false;
                }
            }
        }

        private void navSave_Click(object sender, EventArgs e) {
            String curMethodName = "Tournament:RunningOrderRound:navSave_Click";
            String curMsg = "";
            int rowsProc = 0;

            try {
                if (EventRegDataGridView.Rows.Count > 0) {
                    DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                    String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                    if (curUpdateStatus.ToUpper().Equals("Y")) {
                        try {
                            String curEventGroup = "", curROGroup = "";
                            Int16 curRunOrder;
                            Int64 curPK = Convert.ToInt64((String)curViewRow.Cells["PK"].Value);

                            try {
                                curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                            } catch {
                                curEventGroup = "";
                            }
							try {
								curROGroup = (String) curViewRow.Cells["RunOrderGroup"].Value;
							} catch {
								curROGroup = "";
							}
							try {
                                curRunOrder = Convert.ToInt16((String)curViewRow.Cells["RunOrder"].Value);
                            } catch {
                                curRunOrder = 0;
                            }

                            StringBuilder curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update EventRunOrder Set ");
                            curSqlStmt.Append("EventGroup = '" + curEventGroup + "'");
							curSqlStmt.Append( ", RunOrderGroup = '" + curROGroup + "'" );
							curSqlStmt.Append(", RunOrder = " + curRunOrder.ToString());
                            curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                            curSqlStmt.Append(" Where PK = " + curPK.ToString());
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                            isDataModified = false;

                        } catch (Exception excp) {
                            curMsg = "Error attempting to update skier event information \n" + excp.Message;
                            MessageBox.Show(curMsg);
                            Log.WriteFile(curMethodName + curMsg);
                        }
                    }
                }

			} catch (Exception excp) {
                curMsg = "Error attempting to save changes \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
        }

        private void roundActiveSelect_Click( object sender, EventArgs e ) {
            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSave_Click(null, null);
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }

            if ( !( isDataModified ) ) {
                if ( sender != null ) {
                    String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                    try {
                        roundActiveSelect.RoundValue = curValue;
                        if ( curValue.Equals("1") ) {
                            ElimButton.Visible = false;
                            HeadToHeadNextButton.Visible = false;
                        } else {
                            ElimButton.Visible = true;
                            HeadToHeadNextButton.Visible = true;
                        }
                    } catch ( Exception ex ) {
                        curValue = ex.Message;
                    }
                }

                int curSaveEventRegViewIdx = myEventRegViewIdx;
                navRefresh_Click(null, null);
            }
        }

        private void HeadToHeadButton_Click( object sender, EventArgs e ) {
            isDataModified = false;
            myCommandType = "H2H";

			myRunOrderElimDialog.TourRules = myTourRules;
			myRunOrderElimDialog.TourData = myTourRow;
			myRunOrderElimDialog.getHeadToHeadEntries( myEvent, Convert.ToInt16( roundActiveSelect.RoundValue ) );
			myRunOrderElimDialog.ShowDialog( this );
			if ( myRunOrderElimDialog.DialogResult == DialogResult.OK ) {
				myPlcmtOrg = myRunOrderElimDialog.PlcmtOrg;
				isDataModified = false;
				if ( myPlcmtOrg.Length > 0 ) {
					loadEventRegView( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
				} else {
					navRefresh_Click( null, null );
				}
			}
		}

		private void HeadToHeadNextButton_Click( object sender, EventArgs e ) {
            myCommandType = "H2H";
            if (Convert.ToInt16( roundActiveSelect.RoundValue ) > 1) {
                myRunOrderElimDialog.TourRules = myTourRules;
                myRunOrderElimDialog.TourData = myTourRow;
                myRunOrderElimDialog.getHeadToHeadNexEntries( myEvent, Convert.ToInt16( roundActiveSelect.RoundValue ) );
                myRunOrderElimDialog.ShowDialog( this );
                if (myRunOrderElimDialog.DialogResult == DialogResult.OK) {
                    myPlcmtOrg = myRunOrderElimDialog.PlcmtOrg;
                    isDataModified = false;
                    if (myPlcmtOrg.Length > 0) {
                        loadEventRegView( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
                    } else {
                        navRefresh_Click( null, null );
                    }
                }
            }
        }

        private void CopyFromPrevButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:RunningOrderRound:CopyFromPrevButton_Click";
            StringBuilder curSqlStmt = new StringBuilder("");
            String curMsg = "";
            int rowsProc = 0;
            String curRules = (String)myTourRow["Rules"];

            try {
                if (checkDeleteExisting( Convert.ToInt16( roundActiveSelect.RoundValue ), true )) {
                    try {
                        if ( Convert.ToInt16(roundActiveSelect.RoundValue) > 1 ) {
                            String curDataType = "best", curPlcmtMethod = "score", curPointsMethod = "nops", myPlcmtOrg = "div";
                            DataTable mySummaryDataTable = null;
                            myRunOrderCopyDialog.ShowDialog(this);
                            if ( myRunOrderCopyDialog.DialogResult == DialogResult.OK ) {
                                String curCopyCommand = myRunOrderCopyDialog.Command;
                                if ( curCopyCommand.ToLower().Equals("copy") ) {
                                    curSqlStmt = new StringBuilder("");
                                    curSqlStmt.Append("Insert EventRunOrder ");
                                    curSqlStmt.Append( "( SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, Round, RunOrder, LastUpdateDate, Notes, RankingScore ) " );
                                    curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, " + roundActiveSelect.RoundValue + ", RunOrder, GETDATE(), '', RankingScore " );
                                    curSqlStmt.Append("FROM EventRunOrder ");
                                    curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + myEvent + "' ");
                                    curSqlStmt.Append("AND Round = " + ( Convert.ToInt16(roundActiveSelect.RoundValue) - 1 ));
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

								} else {
                                    if ( curCopyCommand.ToLower().Equals("copybybest") ) {
                                        curDataType = "best";
                                    } else if ( curCopyCommand.ToLower().Equals("copybylast") ) {
                                        curDataType = "round";
                                    } else if ( curCopyCommand.ToLower().Equals("copybytotal") ) {
                                        curDataType = "total";
                                    }
                                    if (myEvent.ToLower().Equals( "trick" )) {
                                        if (curRules.ToLower().Equals( "iwwf" )) {
                                            mySummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Trick", curRules, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        } else {
                                            mySummaryDataTable = myCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        }

									} else if ( myEvent.ToLower().Equals("jump") ) {
                                        if (curRules.ToLower().Equals( "iwwf" )) {
                                            mySummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Jump", curRules, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        } else {
                                            mySummaryDataTable = myCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        }

									} else {
                                        if (curRules.ToLower().Equals( "iwwf" )) {
                                            mySummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Slalom", curRules, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        } else {
                                            mySummaryDataTable = myCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, myPlcmtOrg, curPointsMethod, null, null );
                                        }
                                    }

									insertRoundRunOrder( curCopyCommand, myPlcmtOrg, mySummaryDataTable, roundActiveSelect.RoundValue, myEvent );
                                }
                            }

						} else {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Insert EventRunOrder ");
                            curSqlStmt.Append( "( SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, Round, RunOrder, LastUpdateDate, Notes ) " );
                            curSqlStmt.Append("SELECT SanctionId, MemberId, AgeGroup, EventGroup, '', Event, " + roundActiveSelect.RoundValue + ", RunOrder, GETDATE(), '' ");
                            curSqlStmt.Append("FROM EventReg ");
                            curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + myEvent + "'");
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }

						isDataModified = false;
                        navRefresh_Click(null, null);

					} catch (Exception excp) {
                        curMsg = "Error attempting to copy RunOrder from EventReg \n" + excp.Message;
                        MessageBox.Show(curMsg);
                        Log.WriteFile(curMethodName + curMsg);
                    }
                }

			} catch ( Exception excp ) {
                curMsg = "Error attempting to copy RunOrder from EventReg \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
        }

        private void ElimButton_Click( object sender, EventArgs e ) {
            myCommandType = "Elim";

			myRunOrderElimDialog.TourRules = myTourRules;
			myRunOrderElimDialog.TourData = myTourRow;
			myRunOrderElimDialog.getElimEntries( myEvent, Convert.ToInt16( roundActiveSelect.RoundValue ) );
			myRunOrderElimDialog.ShowDialog( this );
			if ( myRunOrderElimDialog.DialogResult == DialogResult.OK ) {
				myPlcmtOrg = myRunOrderElimDialog.PlcmtOrg;
				isDataModified = false;
				if ( myPlcmtOrg.Length > 0 ) {
					loadEventRegView( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
				} else {
					navRefresh_Click( null, null );
				}
			}
		}

		private void PickAndChoseButton_Click( object sender, EventArgs e ) {
			myCommandType = "Pick";

			myRunOrderElimDialog.TourRules = myTourRules;
			myRunOrderElimDialog.TourData = myTourRow;
			myRunOrderElimDialog.getPickChooseEntries( myEvent, Convert.ToInt16( roundActiveSelect.RoundValue ), mySortCmd);
			myRunOrderElimDialog.ShowDialog( this );
			if ( myRunOrderElimDialog.DialogResult == DialogResult.OK ) {
				navRefresh_Click( null, null );
			}
		}

		private void SendSkierListButton_Click( object sender, EventArgs e ) {
			DataTable curDataTable = getEventRoundSkierOrder( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
			if ( curDataTable.Rows.Count > 0 ) {
				WscHandler.sendRunningOrder( myEvent, Convert.ToInt16( roundActiveSelect.RoundValue ), curDataTable );
				MessageBox.Show( "Running order has been sent to WaterSkiConnect" );
			}

		}

		private Boolean checkDeleteExisting(Int16 inRound) {
            return checkDeleteExisting( inRound, true );
        }
        private Boolean checkDeleteExisting(Int16 inRound, Boolean inConfirm) {
            String curMethodName = "Tournament:RunningOrderRound:checkDeleteExisting";
            Boolean curReturn = true;
            String curMsg = "";
            int rowsProc = 0;

            try {
                StringBuilder curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("SELECT Count(PK) as Count FROM EventRunOrder ");
                curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + myEvent + "' and Round = " + inRound.ToString());
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                int curCount = (int)curDataTable.Rows[0]["Count"];
                if ( curCount > 0 ) {
                    if (inConfirm) {
                        DialogResult msgResp = MessageBox.Show( "Running orders already exist for round " + inRound.ToString()
                            + "\nDo you want to continue and replace existing values?"
                            , "Warning"
                            , MessageBoxButtons.YesNo
                            , MessageBoxIcon.Warning
                            , MessageBoxDefaultButton.Button2 );
                        if (msgResp == DialogResult.Yes) {
                            curReturn = true;
                        } else {
                            curReturn = false;
                        }
                    } else {
                        curReturn = true;
                    }
                    if ( curReturn ) {
                        try {
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Delete EventRunOrder ");
                            curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + myEvent + "' AND Round = " + inRound.ToString());
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        } catch ( Exception excp ) {
                            curMsg = "Error attempting to remove existing running orders for selected round \n" + excp.Message;
                            MessageBox.Show(curMsg);
                            Log.WriteFile(curMethodName + curMsg);
                        }
                    }
                }

            } catch ( Exception excp ) {
                curMsg = "Error attempting to remove existing running orders for selected round \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
            return curReturn;
        }

        private void insertRoundRunOrder( String inCommand, String inPlcmtOrg, DataTable inDataTable, String inRound, String inEvent ) {
            insertRoundRunOrder(inCommand, inPlcmtOrg, inDataTable, inRound, 0, inEvent);
        }
        private void insertRoundRunOrder(String inCommand, String inPlcmtOrg, DataTable inDataTable, String inRound, Int16 inNumSkiers, String inEvent ) {
            String curMethodName = "Tournament:RunningOrderRound:insertRoundRunOrder";
            String curMsg = "", curGroup = "", curROGroup = "", curDiv = "", curMemberId = "", curPlcmt = "", curReportGroup = "", prevReportGroup = "", curSortCmd = "", curRankingScore;
            int rowsProc = 0, curRunOrder = 0, curNewOrder = 0, curNumPlcmt = 0;
            Int16 curNumRound = Convert.ToInt16(inRound);
            StringBuilder curSqlStmt = new StringBuilder("");
            DataTable curSummaryDataTable = new DataTable();

            
            if ( inPlcmtOrg.ToLower().Equals("div") ) {
                curSortCmd = "AgeGroup ASC, Plcmt" + inEvent + " DESC, SkierName ASC";
            } else if ( inPlcmtOrg.ToLower().Equals("divgr") ) {
                curSortCmd = "AgeGroup ASC, EventGroup ASC, Plcmt" + inEvent + " DESC, SkierName ASC, Round ASC";
            } else if ( inPlcmtOrg.ToLower().Equals("group") ) {
                curSortCmd = "EventGroup ASC, Plcmt" + inEvent + " DESC, SkierName ASC ";
            } else {
                curSortCmd = "PlcmtTour DESC, SkierName ASC ";
            }
            if ( inCommand.ToLower().Equals("copybylast") || inCommand.ToLower().Equals("round") ) {
                inDataTable.DefaultView.RowFilter = "Round = " + ( curNumRound - 1 ).ToString();
                curSummaryDataTable = inDataTable.DefaultView.ToTable();
                curSummaryDataTable.DefaultView.Sort = "Round ASC, " + curSortCmd;
                curSummaryDataTable = curSummaryDataTable.DefaultView.ToTable();
            } else {
                inDataTable.DefaultView.Sort = curSortCmd;
                curSummaryDataTable = inDataTable.DefaultView.ToTable();
            }

            try {
                foreach ( DataRow curRow in curSummaryDataTable.Rows ) {
                    curMemberId = (String)curRow["MemberId"];
					curROGroup = "";
					curGroup = (String)curRow["EventGroup"];
                    if ( inNumSkiers > 0 && inPlcmtOrg.ToLower().Equals("tour") ) {
                        curGroup = "XX";
                    }

                    curDiv = (String)curRow["AgeGroup"];
                    curPlcmt = (String)curRow["Plcmt" + inEvent];
                    try {
                        if (inEvent.Equals( "Jump" )) {
                            curRankingScore = ( (Decimal)curRow["ScoreFeet"] ).ToString();
                        } else if (inEvent.Equals( "Slalom" )) {
                            curRankingScore = ( (Decimal)curRow["Score" + inEvent] ).ToString();
                        } else if (inEvent.Equals( "Trick" )) {
                            curRankingScore = ( (Int16)curRow["Score" + inEvent] ).ToString();
                        } else {
                            curRankingScore = "0";
                        }
                    } catch {
                        curRankingScore = "0";
                    }

                    if ( inPlcmtOrg.ToLower().Equals("agegroup") ) {
                        curReportGroup = curDiv;
                    } else if ( inPlcmtOrg.ToLower().Equals("div") ) {
                        curReportGroup = curDiv;
                    } else if ( inPlcmtOrg.ToLower().Equals("divgr") ) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else if ( inPlcmtOrg.ToLower().Equals("group") ) {
                        curReportGroup = curGroup;
                    } else {
                        curReportGroup = "";
                        curPlcmt = (String)curRow["PlcmtTour"];
                    }
                    if ( curReportGroup != prevReportGroup ) {
                        curRunOrder = 1;
                    } else {
                        curRunOrder += 1;
                    }
                    prevReportGroup = curReportGroup;

                    if ( inNumSkiers > 0 ) {
                        if ( curPlcmt.Contains("T") ) {
                            curNumPlcmt = Convert.ToInt32(curPlcmt.Substring(0, curPlcmt.IndexOf("T")));
                        } else {
                            curNumPlcmt = Convert.ToInt32(curPlcmt);
                        }
                        if ( curNumPlcmt <= inNumSkiers ) {
                            curNewOrder = inNumSkiers - curNumPlcmt + 1;
                        } else {
                            curNewOrder = 0;
                        }
                    } else {
                        curNewOrder = curRunOrder;
                    }

                    if ( inNumSkiers == 0 || (inNumSkiers > 0 && curNewOrder > 0) ) {
                        #region Add skiers until specified number has been met
                        curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("Insert EventRunOrder ( ");
                        curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, Round, RunOrder, LastUpdateDate, Notes, RankingScore " );
                        curSqlStmt.Append(") Values ( ");
                        curSqlStmt.Append("'" + mySanctionNum + "'");
                        curSqlStmt.Append(", '" + curMemberId + "'");
                        curSqlStmt.Append(", '" + curDiv + "'");
                        curSqlStmt.Append(", '" + curGroup + "'");
						curSqlStmt.Append( ", '" + curROGroup + "'" );
						curSqlStmt.Append(", '" + myEvent + "'");
                        curSqlStmt.Append(", " + inRound);
                        curSqlStmt.Append(", " + curNewOrder.ToString());
                        curSqlStmt.Append(", GETDATE(), ''");
                        curSqlStmt.Append( ", " + curRankingScore + " " );
                        curSqlStmt.Append(")");
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        #endregion
                    }
                }

                if ( inNumSkiers == 0 ) {
                    #region Add skiers that didn't yet have scores
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("SELECT Distinct E.Event, E.SanctionId, E.MemberId, E.AgeGroup, E.EventGroup ");
                    curSqlStmt.Append("FROM EventReg E ");
                    curSqlStmt.Append("WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + myEvent + "' ");
                    curSqlStmt.Append("  AND Not Exists ( ");
                    curSqlStmt.Append("      Select 1 From EventRunOrder O Where E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound + " ) ");
                    DataTable curDataTable = getData(curSqlStmt.ToString());
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        curMemberId = (String)curRow["MemberId"];
                        curGroup = (String)curRow["EventGroup"];
                        curDiv = (String)curRow["AgeGroup"];
                        curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("Insert EventRunOrder ( ");
                        curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, Round, RunOrder, LastUpdateDate, Notes " );
                        curSqlStmt.Append(") Values ( ");
                        curSqlStmt.Append("'" + mySanctionNum + "'");
                        curSqlStmt.Append(", '" + curMemberId + "'");
                        curSqlStmt.Append(", '" + curDiv + "'");
                        curSqlStmt.Append(", '" + curGroup + "'");
						curSqlStmt.Append( ", '" + curROGroup + "'" );
						curSqlStmt.Append(", '" + myEvent + "'");
                        curSqlStmt.Append(", " + inRound);
                        curSqlStmt.Append(", 1");
                        curSqlStmt.Append(", GETDATE(), '' ");
                        curSqlStmt.Append(")");
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }
                    #endregion
                }
            
            } catch ( Exception excp ) {
                curMsg = "Error attempting to copy RunOrder from EventReg \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void RemoveButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:RunningOrderRound:RemoveButton_Click";
            String curMsg = "";
            int rowsProc = 0;

            try {
                StringBuilder curSqlStmt = new StringBuilder("");
                try {
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("Delete EventRunOrder ");
                    curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + myEvent + "' AND Round = " + roundActiveSelect.RoundValue);
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    isDataModified = false;
                    navRefresh_Click(null, null);
                } catch ( Exception excp ) {
                    curMsg = "Error attempting to copy RunOrder from EventReg \n" + excp.Message;
                    MessageBox.Show(curMsg);
                    Log.WriteFile(curMethodName + curMsg);
                }

            } catch ( Exception excp ) {
                curMsg = "Error attempting to copy RunOrder from EventReg \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            if (isDataModified) {
                checkModifyPrompt();
            }
            if (!(isDataModified)) {
                //Retrieve running order data for display
                loadEventRegView( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
            }
        }

        private void EventGroupList_SelectedIndexChanged(object sender, EventArgs e) {
            if (isDataModified) {
                checkModifyPrompt();
            }
            if (!( isDataModified )) {
                //Retrieve running order data for display
                loadEventRegView( Convert.ToInt16( roundActiveSelect.RoundValue ), myPlcmtOrg, myCommandType );
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[3];
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder" };

            curSelectCommand[0] = "SELECT * FROM TourReg "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "And EXISTS (SELECT 1 FROM EventReg "
                + "    WHERE TourReg.SanctionId = EventReg.SanctionId AND TourReg.MemberId = EventReg.MemberId "
                + "      AND TourReg.AgeGroup = EventReg.AgeGroup AND EventReg.Event = '" + myEvent + "') "
                + "And EXISTS (SELECT 1 FROM EventRunOrder "
                + "    WHERE TourReg.SanctionId = EventRunOrder.SanctionId AND TourReg.MemberId = EventRunOrder.MemberId "
                + "      AND TourReg.AgeGroup = EventRunOrder.AgeGroup AND EventRunOrder.Event = '" + myEvent + "' "
                + "      AND EventRunOrder.Round = " + roundActiveSelect.RoundValue + ") "
                ;

            curSelectCommand[1] = "Select * from EventReg "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "And Event = '" + myEvent + "' "
                + "And EXISTS (SELECT 1 FROM EventRunOrder "
                + "    WHERE EventReg.SanctionId = EventRunOrder.SanctionId AND EventReg.MemberId = EventRunOrder.MemberId "
                + "      AND EventReg.AgeGroup = EventRunOrder.AgeGroup AND EventRunOrder.Event = '" + myEvent + "' "
                + "      AND EventRunOrder.Round = " + roundActiveSelect.RoundValue + ") "
                ;

            curSelectCommand[2] = "Select * from EventRunOrder "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "  And Event = '" + myEvent + "' "
                + "  And Round = " + roundActiveSelect.RoundValue
                ;

            myExportData.exportData( curTableName, curSelectCommand );

        }

        private void navExportRunningOrder_Click(object sender, EventArgs e) {
            String curSkierName, curAgeGroup, curRunOrder,
                curRankingScore, curTeamCode, curState, curJumpHeight, curTrickBoat;
            String curEventGroup = "", prevEventGroup = "", curEventClass = "", prevEventClass = "";
            int rowCount = 0, curOrder = 0;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            try {
                String curTourName = (String)myTourRow["Name"];

                String curEventHeader = " SCORE SEQ    ";
                String curFileName = myEvent + "_RunOrder_R" + roundActiveSelect.RoundValue + ".txt";
                outBuffer = getExportFile( curFileName );
                if (outBuffer != null) {

                    foreach (DataGridViewRow curViewRow in EventRegDataGridView.Rows) {
                        rowCount++;
                        curOrder++;
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            curEventGroup = (String)curViewRow.Cells["AgeGroup"].Value;
                            if (curEventGroup.ToLower().Equals( "cm" ) || curEventGroup.ToLower().Equals( "cw" )) {
                                String curValue = (String)curViewRow.Cells["EventGroup"].Value;
                                if (curValue.Length > 1) {
                                    if (curValue.Length == 2) {
                                        curEventGroup += "-" + curValue.Substring( 0, 1 );
                                    } else {
                                        curEventGroup += "-" + curValue.Substring( 1, 1 );
                                    }
                                }
                            } else {
                                curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                            }
                        } else {
                            curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
                        }
                        curEventClass = (String)curViewRow.Cells["EventClass"].Value;
                        if (!(curEventGroup.ToLower().Equals( prevEventGroup.ToLower() ))
                            || !(curEventClass.ToLower().Equals( prevEventClass.ToLower() ))
                            ) {
                            outLine = new StringBuilder( " " );
                            outBuffer.WriteLine( outLine.ToString() );
                            outLine = new StringBuilder( " " + mySanctionNum + " " + curTourName );
                            outBuffer.WriteLine( outLine.ToString() );
                            outLine = new StringBuilder( curEventHeader + "(CL " + curEventClass + ") " + curEventGroup + " " + myEvent + " " );
                            outBuffer.WriteLine( outLine.ToString() );
                            curOrder = 1;
                        }
                        prevEventGroup = curEventGroup;
                        prevEventClass = curEventClass;

                        outLine = new StringBuilder( "" );
                        curSkierName = (String)curViewRow.Cells["SkierName"].Value;
                        curRankingScore = (String)curViewRow.Cells["RankingScore"].Value;
                        outLine.Append( curRankingScore.PadLeft( 6, ' ' ) );
                        curRunOrder = curOrder.ToString( "##0" );
                        outLine.Append( " " + curRunOrder.PadLeft( 3, ' ' ) );
                        if (myEvent.Equals("Slalom")) {
                            outLine.Append( "    " );
                        } else if (myEvent.Equals( "Trick" )) {
                            curTrickBoat = (String)curViewRow.Cells["TrickBoat"].Value;
                            if (curTrickBoat.Length > 2) {
                                curTrickBoat = curTrickBoat.Substring( 0, 2 );
                            }
                            outLine.Append( " " + curTrickBoat.PadRight( 3, ' ' ) );
                        } else if (myEvent.Equals( "Jump" )) {
                            curJumpHeight = (String)curViewRow.Cells["JumpHeight"].Value;
                            if (curJumpHeight.Length > 1) {
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
                        if (curTeamCode.Trim().Length > 1) {
                            outLine.Append( " (" + curTeamCode + ") " );
                        } else {
                            outLine.Append( " (" + curState + ")" );
                        }

                        outBuffer.WriteLine( outLine.ToString() );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not write running order data to file\n\nError: " + ex.Message );
            } finally {
                if (outBuffer != null) {
                    outBuffer.Close();
                }
            }
        }

        private void loadEventRegView(Int16 inRound, String inPlcmtOrg, String inCommandType) {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;
            
            try {
                EventRegDataGridView.Rows.Clear();
                DataTable curDataTable = getEventRoundSkierOrder( inRound, inPlcmtOrg, inCommandType );
                if (curDataTable.Rows.Count > 0) {
                    DataGridViewRow curViewRow;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        isDataModified = false;
                        myViewIdx = EventRegDataGridView.Rows.Add();
                        curViewRow = EventRegDataGridView.Rows[myViewIdx];
                        curViewRow.Cells["Updated"].Value = "N";
                        curViewRow.Cells["PK"].Value = ((Int64)curDataRow["PK"]).ToString();
                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];

                        try {
                            curViewRow.Cells["Round"].Value = ( (Byte)curDataRow["Round"] ).ToString("0");
                        } catch {
                            curViewRow.Cells["Round"].Value = "0";
                        }
                        try {
                            curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                        } catch {
                            curViewRow.Cells["EventGroup"].Value = "";
                        }
						try {
							curViewRow.Cells["RunOrderGroup"].Value = (String) curDataRow["RunOrderGroup"];
						} catch {
							curViewRow.Cells["RunOrderGroup"].Value = "";
						}
						try {
                            curViewRow.Cells["RunOrder"].Value = ((Int16)curDataRow["RunOrder"]).ToString("##0");
                        } catch {
                            curViewRow.Cells["RunOrder"].Value = "0";
                        }
                        try {
                            if (( (String)curDataRow["Event"] ).Equals( "Slalom" )) {
                                curViewRow.Cells["RankingScore"].Value = ( (Decimal)curDataRow["RankingScore"] ).ToString( "##0.00" );
                            } else if (( (String)curDataRow["Event"] ).Equals( "Trick" )) {
                                curViewRow.Cells["RankingScore"].Value = ( (Decimal)curDataRow["RankingScore"] ).ToString( "####0" );
                            } else if (( (String)curDataRow["Event"] ).Equals( "Jump" )) {
                                curViewRow.Cells["RankingScore"].Value = ( (Decimal)curDataRow["RankingScore"] ).ToString( "##0.0" );
                            }
                        } catch {
                            curViewRow.Cells["RankingScore"].Value = "0";
                        }
                        try {
                            curViewRow.Cells["EventClass"].Value = (String)curDataRow["EventClass"];
                        } catch {
                            curViewRow.Cells["EventClass"].Value = "";
                        }
                        try {
                            curViewRow.Cells["State"].Value = (String)curDataRow["State"];
                        } catch {
                            curViewRow.Cells["State"].Value = "";
                        }
                        try {
                            curViewRow.Cells["TeamCode"].Value = (String)curDataRow["TeamCode"];
                        } catch {
                            curViewRow.Cells["TeamCode"].Value = "";
                        }
                        try {
                            curViewRow.Cells["TrickBoat"].Value = (String)curDataRow["TrickBoat"];
                        } catch {
                            curViewRow.Cells["TrickBoat"].Value = "";
                        }
                        try {
                            curViewRow.Cells["JumpHeight"].Value = (String)curDataRow["JumpHeight"];
                        } catch {
                            curViewRow.Cells["JumpHeight"].Value = "";
                        }
                    }
                    myViewIdx = 0;
                    EventRegDataGridView.CurrentCell = EventRegDataGridView.Rows[myViewIdx].Cells["SkierName"];
                    loadPrintDataGrid( curDataTable );
                }
            } catch (Exception ex) {
                MessageBox.Show("Error retrieving tournament entries \n" + ex.Message);
            }
            Cursor.Current = Cursors.Default;
            try {
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + EventRegDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }
        }

        private void loadPrintDataGrid(DataTable curDataTable) {
            //Load data for print data grid
            String curPrintGroup = "", prevPrintGroup = "";
            int curPrintIdx = 0;
            DataGridViewRow curPrintRow;

            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                if (curDataTable.Rows.Count > 0) {
                    PrintDataGridView.Rows.Clear();

                    if (myTourRules.ToLower().Equals("ncwsa")) {
                        PrintEventRotation.Visible = true;
                        PrintEventGroup.Visible = false;
                    } else {
                        PrintEventRotation.Visible = false;
                        PrintEventGroup.Visible = true;
                    }
                    if (myEvent.Equals( "Slalom" )) {
                        PrintJumpHeight.Visible = false;
                        PrintTrickBoat.Visible = false;
                    } if (myEvent.Equals( "Trick" )) {
                        PrintJumpHeight.Visible = false;
                        PrintTrickBoat.Visible = true;
                    } if (myEvent.Equals( "Jump" )) {
                        PrintJumpHeight.Visible = true;
                        PrintTrickBoat.Visible = false;
                    }
                    foreach (DataRow curTeamRow in curDataTable.Rows) {
                        curPrintIdx = PrintDataGridView.Rows.Add();
                        curPrintRow = PrintDataGridView.Rows[curPrintIdx];

                        try {
                            if (myTourRules.ToLower().Equals("ncwsa")) {
                                curPrintGroup = (String)curTeamRow["AgeGroup"];
                            } else {
                                curPrintGroup = (String)curTeamRow["EventGroup"];
                            }
                        } catch {
                            curPrintGroup = "";
                        }

                        if (!(curPrintGroup.Equals(prevPrintGroup))) {
                            if (curPrintIdx > 0) {
                                curPrintIdx = PrintDataGridView.Rows.Add();
                                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                            }
                        }

                        curPrintRow.Cells["PrintSkierName"].Value = (String)curTeamRow["SkierName"];
                        curPrintRow.Cells["PrintEvent"].Value = (String)curTeamRow["Event"];
                        curPrintRow.Cells["PrintAgeGroup"].Value = (String)curTeamRow["AgeGroup"];
                        try {
                            curPrintRow.Cells["PrintEventGroup"].Value = (String)curTeamRow["EventGroup"];
                            curPrintRow.Cells["PrintEventRotation"].Value = (String)curTeamRow["EventGroup"];

                        } catch {
                            curPrintRow.Cells["PrintEventGroup"].Value = "";
                            curPrintRow.Cells["PrintEventRotation"].Value = "";
                        }
						try {
							curPrintRow.Cells["PrintRunOrderGroup"].Value = (String) curTeamRow["RunOrderGroup"];

						} catch {
							curPrintRow.Cells["PrintRunOrderGroup"].Value = "";
						}
						try {
                            curPrintRow.Cells["PrintRound"].Value = ((Byte)curTeamRow["Round"]).ToString( "0" );
                        } catch {
                            curPrintRow.Cells["PrintRound"].Value = "0";
                        }
                        try {
                            curPrintRow.Cells["PrintRunOrder"].Value = ((Int16)curTeamRow["RunOrder"]).ToString("##0");
                        } catch {
                            curPrintRow.Cells["PrintRunOrder"].Value = "0";
                        }
                        try {
                            curPrintRow.Cells["PrintRankingScore"].Value = ((Decimal)curTeamRow["RankingScore"]).ToString( "####0" );
                        } catch {
                            curPrintRow.Cells["PrintRankingScore"].Value = "0";
                        }
                        try {
                            curPrintRow.Cells["PrintEventClass"].Value = (String)curTeamRow["EventClass"];
                        } catch {
                            curPrintRow.Cells["PrintEventClass"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintState"].Value = (String)curTeamRow["State"];
                        } catch {
                            curPrintRow.Cells["PrintState"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintTeamCode"].Value = (String)curTeamRow["TeamCode"];
                        } catch {
                            curPrintRow.Cells["PrintTeamCode"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintTrickBoat"].Value = (String)curTeamRow["TrickBoat"];
                        } catch {
                            curPrintRow.Cells["PrintTrickBoat"].Value = "";
                        }
                        try {
                            curPrintRow.Cells["PrintJumpHeight"].Value = (String)curTeamRow["JumpHeight"];
                        } catch {
                            curPrintRow.Cells["PrintJumpHeight"].Value = "";
                        }
                        prevPrintGroup = curPrintGroup;
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error retrieving tournament entries \n" + ex.Message);
            }
        }

        private void EventRegDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            DataGridView myDataView = (DataGridView)sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
            if ( isDataModified && ( myViewIdx != e.RowIndex ) ) {
                try {
                    navSave_Click(null, null);
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
            if ( !( isDataModified ) ) {
                if ( myViewIdx != e.RowIndex ) {
                    myViewIdx = e.RowIndex;
                }
            }
        }

        private void EventRegDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( EventRegDataGridView.Rows.Count > 0 ) {
                if ( !( EventRegDataGridView.Columns[e.ColumnIndex].ReadOnly ) ) {
                    myViewIdx = e.RowIndex;
                    String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                    DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                    if ( curColName.Equals("EventGroup") ) {
                        try {
                            myOrigItemValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
                        } catch {
                            myOrigItemValue = "";
                        }
                    } else if ( curColName.Equals("RunOrder") ) {
                        try {
                            myOrigItemValue = Convert.ToInt16((String)curViewRow.Cells[e.ColumnIndex].Value).ToString();
                        } catch {
                            myOrigItemValue = "0";
                        }
                    }
                }
            }
        }

        private void EventRegDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            if ( EventRegDataGridView.Rows.Count > 0 ) {
                myViewIdx = e.RowIndex;
                String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                if ( curColName.Equals("RunOrder") ) {
                    try {
                        Int16 curNum = Convert.ToInt16(e.FormattedValue.ToString());
                        e.Cancel = false;
                    } catch {
                        e.Cancel = true;
                        MessageBox.Show(curColName + " must be numeric");
                    }
                }
            }
        }

        private void EventRegDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            if ( EventRegDataGridView.Rows.Count > 0 ) {
                myViewIdx = e.RowIndex;
                String curColName = EventRegDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = EventRegDataGridView.Rows[myViewIdx];
                if ( curColName.Equals("RunOrder") ) {
                    if ( myOrigItemValue == null )
                        myOrigItemValue = "0";
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        if ( Convert.ToInt16(myOrigItemValue) != 0 ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        Int16 curNum = Convert.ToInt16((String)curViewRow.Cells[e.ColumnIndex].Value);
                        if ( curNum != Convert.ToInt16(myOrigItemValue) ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }

				} else if ( curColName.Equals("EventGroup") ) {
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        if ( !( isObjectEmpty(myOrigItemValue) ) ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        String curValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigItemValue ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }

				} else if ( curColName.Equals( "RunOrderGroup" ) ) {
					if ( isObjectEmpty( curViewRow.Cells[e.ColumnIndex].Value ) ) {
						if ( !( isObjectEmpty( myOrigItemValue ) ) ) {
							isDataModified = true;
							curViewRow.Cells["Updated"].Value = "Y";
						}
					} else {
						String curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
						if ( curValue != myOrigItemValue ) {
							isDataModified = true;
							curViewRow.Cells["Updated"].Value = "Y";
						}
					}
				}
			}
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            int curCount = 0;
            String curTourPlcmtOrg = "tour";
            foreach(DataGridViewRow curRow in EventRegDataGridView.Rows) {
                if (( (String)curRow.Cells["RunOrderGroup"].Value ).Equals( "HH1" )) {
                    curCount++;
                }
            }
            if (curCount > 0) {
                if (curCount > 2) {
                    curTourPlcmtOrg = "group";
                }
                printHeadToHeadAwards( curTourPlcmtOrg );
                return;
            }

            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 12, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);
                myPrintDataGrid = new DataGridViewPrinter(PrintDataGridView, myPrintDoc,
                        CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging);

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }

        private void printHeadToHeadAwards(String curTourPlcmtOrg ) {
            String curSortCmd = "";
            String curPlcmtMethod = "RunOrder";
            Int16 curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
            Int16 curNumPrelimRounds = Convert.ToInt16( curRound - 1 );

            curSortCmd = "Round ASC";
            if (curTourPlcmtOrg.Equals( "div" )) {
                curSortCmd = "AgeGroup ASC, Round ASC";
            }
            curSortCmd += ", EventGroup ASC, RankingScore ASC";

            PrintHeadToHeadAwards curPrintForm = new PrintHeadToHeadAwards();
            curPrintForm.PrintLandscape = true;
            curPrintForm.ReportHeader = "Head to Head Running Order";
            curPrintForm.DivInfoDataTable = getEventDivMaxMinSpeed();
            curPrintForm.TourRules = myTourRules;
            curPrintForm.TourName = (String)myTourRow["Name"];
            curPrintForm.TourPrelimRounds = curNumPrelimRounds;
            curPrintForm.TourEvent = myEvent;
            curPrintForm.TourPlcmtOrg = curTourPlcmtOrg;
            curPrintForm.TourPlcmtMethod = curPlcmtMethod;
            curPrintForm.ShowDataTable = getEventRoundSkierOrder( curRound, curTourPlcmtOrg, "H2H" );
            curPrintForm.Print();
        }

        private void navPrintFormButton_Click(object sender, EventArgs e) {
            Boolean curHeadToHead = false;
            Int16 curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
            String curTourPlcmtOrg = this.myPlcmtOrg, curCommandType = "";
            int curCount = 0;
            foreach (DataGridViewRow curRow in EventRegDataGridView.Rows) {
                if (( (String)curRow.Cells["EventGroup"].Value ).Equals( "HH1" )) {
                    curCount++;
                    curHeadToHead = true;
                }
            }
            if (curCount > 0) {
                curCommandType = "H2H";
                if (curCount > 2) {
                    curTourPlcmtOrg = "div";
                }
            }

            if (myEvent.Equals( "Slalom" )) {
                PrintDialog curPrintDialog = new PrintDialog();
                PrintOfficialFormDialog curDialog = new PrintOfficialFormDialog();
                if (curDialog.ShowDialog() == DialogResult.OK) {
                    String curPrintReport = curDialog.ReportName;
                    if (curPrintReport.Equals( "SlalomJudgeForm" )) {
                        PrintSlalomJudgeForms curPrintForm = new PrintSlalomJudgeForms();
                        curPrintForm.PrintLandscape = true;
                        if (curHeadToHead) {
                            curPrintForm.ReportHeader = "Head to Head Round " + roundActiveSelect.RoundValue;
                            curPrintForm.TourRules = "htoh-" + curTourPlcmtOrg;
                        } else {
                            curPrintForm.ReportHeader = "Round " + roundActiveSelect.RoundValue;
                            curPrintForm.TourRules = myTourRules;
                        }
                        curPrintForm.DivInfoDataTable = getEventDivMaxMinSpeed();
                        curPrintForm.TourName = (String)myTourRow["Name"];
                        curPrintForm.ShowDataTable = getEventRoundSkierOrder( curRound, curTourPlcmtOrg, curCommandType );
                        curPrintForm.Print();
                    } else if (curPrintReport.Equals( "SlalomRecapForm" )) {
                        PrintSlalomRecapForm curPrintForm = new PrintSlalomRecapForm();
                        curPrintForm.PrintLandscape = true;
                        if (curHeadToHead) {
                            curPrintForm.ReportHeader = "Head to Head Round " + roundActiveSelect.RoundValue;
                            curPrintForm.TourRules = "htoh";
                        } else {
                            curPrintForm.ReportHeader = "Round " + roundActiveSelect.RoundValue;
                            curPrintForm.TourRules = myTourRules;
                        }
                        curPrintForm.DivInfoDataTable = getEventDivMaxMinSpeed();
                        curPrintForm.TourName = (String)myTourRow["Name"];
                        curPrintForm.ShowDataTable = getEventRoundSkierOrder( curRound, curTourPlcmtOrg, curCommandType );
                        curPrintForm.Print();
                    }
                }
            } else if (myEvent.Equals( "Jump" )) {
                PrintJumpRecapJudgeForm curPrintForm = new PrintJumpRecapJudgeForm();
                curPrintForm.PrintLandscape = true;
                if (curHeadToHead) {
                    curPrintForm.ReportHeader = "Head to Head Round " + roundActiveSelect.RoundValue;
                    curPrintForm.TourRules = "htoh";
                } else {
                    curPrintForm.ReportHeader = "Round " + roundActiveSelect.RoundValue;
                    curPrintForm.TourRules = myTourRules;
                }
                curPrintForm.DivInfoDataTable = getJumpDivMaxSpeedRamp();
                curPrintForm.TourName = (String)myTourRow["Name"];
                curPrintForm.ShowDataTable = getEventRoundSkierOrder( curRound, curTourPlcmtOrg, curCommandType );
                curPrintForm.Print();
            } else if (myEvent.Equals( "Trick" )) {
                PrintTrickJudgeForm curPrintForm = new PrintTrickJudgeForm();
                curPrintForm.PrintLandscape = true;
                if (curHeadToHead) {
                    curPrintForm.ReportHeader = "Head to Head Round " + roundActiveSelect.RoundValue;
                    curPrintForm.TourRules = "htoh";
                } else {
                    curPrintForm.ReportHeader = "Round " + roundActiveSelect.RoundValue;
                    curPrintForm.TourRules = myTourRules;
                }
                curPrintForm.DivInfoDataTable = getEventDivMaxMinSpeed();
                curPrintForm.TourName = (String)myTourRow["Name"];
                curPrintForm.TourRounds = Convert.ToInt32( myTourRow["TrickRounds"] );
                curPrintForm.ShowDataTable = getEventRoundSkierOrder( curRound, curTourPlcmtOrg, curCommandType );
                curPrintForm.Print();
            }
        }

        private DataTable buildPlcmtDataTable() {
            //Determine number of skiers per placement group
            DataTable curDataTable = new DataTable();
            //SanctionId, MemberId, AgeGroup, EventGroup, Event, Round, RunOrder

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType("System.String");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType("System.String");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "EventGroup";
            curCol.DataType = System.Type.GetType("System.String");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType("System.Int16");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "RunOrder";
            curCol.DataType = System.Type.GetType("System.Int16");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);


            return curDataTable;
        }

        private DataTable getEventRoundSkierOrder(Int16 inRound, String inPlcmtOrg, String inCommandType) {
            String curPlcmtOrg = inPlcmtOrg;
            if (inPlcmtOrg == null) curPlcmtOrg = "";
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, T.SkierName, O.Event, O.Round, O.EventGroup, O.RunOrderGroup, O.AgeGroup, O.RunOrder" );
            curSqlStmt.Append(", COALESCE(O.RankingScore, E.RankingScore) as RankingScore, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt");
            curSqlStmt.Append( ", E.EventClass, E.TeamCode, T.TrickBoat, T.JumpHeight, T.State, O.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder" );
            curSqlStmt.Append( ", O.Round as Round" + myEvent + " " );
			curSqlStmt.Append( ", T.Federation, X.Federation as TourFederation " );
			curSqlStmt.Append( "FROM EventRunOrder O " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON O.SanctionId = T.SanctionId AND O.MemberId = T.MemberId AND O.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "     INNER JOIN EventReg E ON O.SanctionId = E.SanctionId AND O.MemberId = E.MemberId AND O.AgeGroup = E.AgeGroup AND O.Event = E.Event " );
			curSqlStmt.Append( "     INNER JOIN Tournament AS X on X.SanctionId = E.SanctionId " );
			curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON O.SanctionId = D.SanctionId AND O.AgeGroup = D.AgeGroup AND O.Event = D.Event " );
			curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' AND O.Event = '" + myEvent + "' AND O.Round = " + inRound.ToString() + " " );

            if (curPlcmtOrg.Length == 0) {
				if (mySortCmd.Length > 0) {
					curSqlStmt.Append("ORDER BY " + mySortCmd);
				} else {
					//curSqlStmt.Append( "Order by COALESCE(D.RunOrder, 999) ASC, O.AgeGroup ASC, O.RunOrder ASC, E.RankingScore DESC, T.SkierName ASC " );
					curSqlStmt.Append("Order by O.EventGroup ASC, O.RunOrderGroup, O.RunOrder ASC, E.RankingScore ASC, T.SkierName ASC ");
				}

			} else if (inPlcmtOrg.ToLower().Equals( "tour" )) {
                if ( inCommandType.Equals("H2H")) {
                    curSqlStmt.Append( "Order by O.EventGroup ASC, O.RunOrderGroup ASC, O.RunOrder ASC, E.RankingScore DESC, T.SkierName ASC " );
                } else {
                    curSqlStmt.Append( "Order by O.EventGroup ASC, O.RunOrderGroup, O.RunOrder ASC, E.RankingScore ASC, T.SkierName ASC " );
                }
            
			} else {
                if (inCommandType.Equals( "H2H" )) {
					if (inPlcmtOrg.ToLower().Equals("div")) {
						curSqlStmt.Append("Order by O.AgeGroup ASC, COALESCE(D.RunOrder, 999) ASC, O.AgeGroup ASC, O.EventGroup ASC, O.RunOrderGroup ASC, O.RunOrder ASC, E.RankingScore DESC, T.SkierName ASC ");
					
					} else if (inPlcmtOrg.ToLower().Equals("group")) {
						curSqlStmt.Append("Order by O.EventGroup ASC, O.RunOrderGroup, O.RunOrder ASC, E.RankingScore DESC, T.SkierName ASC ");

					} else {
                        curSqlStmt.Append("Order by O.RunOrderGroup, O.RunOrder ASC, E.RankingScore DESC, T.SkierName ASC ");
                    }
                
				} else {
                    curSqlStmt.Append( "Order by COALESCE(D.RunOrder, 999) ASC, O.AgeGroup ASC, O.RunOrderGroup, O.RunOrder ASC, E.RankingScore ASC, T.SkierName ASC " );
                }
            }

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getEventDivMaxMinSpeed() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS MinSpeed " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName IN ('AWSASlalomMax', 'IwwfSlalomMax', 'NcwsaSlalomMax') " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName IN ('IwwfSlalomMin', 'NcwsaSlalomMin') " );
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

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private StreamWriter getExportFile(String inFileName) {
            String myFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = curFileFilter;
            myFileDialog.FilterIndex = 1;
            if (inFileName == null) {
                myFileDialog.FileName = "";
            } else {
                myFileDialog.FileName = inFileName;
            }

            try {
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    myFileName = myFileDialog.FileName;
                    if (myFileName != null) {
                        int delimPos = myFileName.LastIndexOf('\\');
                        String curFileName = myFileName.Substring(delimPos + 1);
                        if (curFileName.IndexOf('.') < 0) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split('|');
                            if (curList.Length > 0) {
                                int curDelim = curList[1].IndexOf(".");
                                if (curDelim > 0) {
                                    curDefaultExt = curList[1].Substring(curDelim - 1);
                                }
                            }
                            myFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText(myFileName);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: Could not get a file to export data to " + "\n\nError: " + ex.Message);
            }

            return outBuffer;
        }

        private bool isObjectEmpty(object inObject) {
            bool curReturnValue = false;
            if (inObject == null) {
                curReturnValue = true;
            } else if (inObject == System.DBNull.Value) {
                curReturnValue = true;
            } else if (inObject.ToString().Length > 0) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }
	}
}
