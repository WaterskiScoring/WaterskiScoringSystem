using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Admin;

namespace WaterskiScoringSystem.Tournament {
    public partial class Registration : Form {
        private Boolean isDataModified = false;
        private String mySanctionNum;
        private String myOrigItemValue = "";
        private String mySortCommand = "";
        private String myFilterCmd = "";

        private int myTourRegRowIdx;
        private DataRow myTourRow;
        private TourRegAddMember myTourRegAddDialog;
		private TourRegRankEquiv myTourRegRankEquivDialog;
		private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private TourEventReg myTourEventReg;
        private EditRegMember myEditRegMemberDialog;

        private DataTable myTourRegDataTable;

        public Registration() {
            InitializeComponent();
        }

        private void Registration_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TourRegList_Width > 0 ) {
                this.Width = Properties.Settings.Default.TourRegList_Width;
            }
            if ( Properties.Settings.Default.TourRegList_Height > 0 ) {
                this.Height = Properties.Settings.Default.TourRegList_Height;
            }
            if ( Properties.Settings.Default.TourRegList_Location.X > 0
                && Properties.Settings.Default.TourRegList_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourRegList_Location;
            }
            if ( Properties.Settings.Default.TourRegList_Sort.Length > 0 ) {
                mySortCommand = Properties.Settings.Default.TourRegList_Sort;
            } else {
                mySortCommand = "SkierName ASC, AgeGroup ASC";
                Properties.Settings.Default.TourRegList_Sort = mySortCommand;
            }

            String[] curList = { "SkierName", "AgeGroup", "ReadyToSki", "ReadyForPlcmt", "SlalomReg", "TrickReg", "JumpReg", "SlalomGroup", "TrickGroup", "JumpGroup", "EntryDue", "EntryPaid", "PaymentMethod", "JumpHeight", "TrickBoat", "AwsaMbrshpPaymt" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if ( mySanctionNum == null ) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                //this.Close();
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                    //this.Close();
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];

                        if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
                        if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
                        if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }
                        if ( Convert.ToInt16(myTourRow["SlalomRounds"]) == 0 ) {
                            SlalomReg.Visible = false;
                            SlalomGroup.Visible = false;
                            RegCountSlalomLabel.Visible = false;
                            SlalomRegCount.Visible = false;
                        }
                        if ( Convert.ToInt16(myTourRow["TrickRounds"]) == 0 ) {
                            TrickReg.Visible = false;
                            TrickGroup.Visible = false;
                            RegCountTrickLabel.Visible = false;
                            TrickRegCount.Visible = false;
                        }
                        if ( Convert.ToInt16(myTourRow["JumpRounds"]) == 0 ) {
                            JumpReg.Visible = false;
                            JumpGroup.Visible = false;
                            RegCountJumpLabel.Visible = false;
                            JumpRegCount.Visible = false;
                        }
                        myTourRegAddDialog = new TourRegAddMember();
                        myEditRegMemberDialog = new EditRegMember();
						myTourRegRankEquivDialog = new TourRegRankEquiv();

						myTourEventReg = new TourEventReg();
                        myTourRegRowIdx = 0;
                        loadTourRegView();
                    } else {
                        MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                    }

                }
            }

            isDataModified = false;
        }

        private void loadTourRegView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;
            int curRowIdx = myTourRegRowIdx;

            tourRegDataGridView.Rows.Clear();
            myTourRegDataTable = getTourRegData();
            myTourRegDataTable.DefaultView.Sort = mySortCommand;
            myTourRegDataTable.DefaultView.RowFilter = myFilterCmd;
            DataTable curDataTable = myTourRegDataTable.DefaultView.ToTable();

            if ( curDataTable.Rows.Count > 0 ) {
                DataGridViewRow curViewRow;
                isDataModified = false;
                int curViewIdx = 0;
                String curMemberId, curEvent;
                foreach ( DataRow curDataRow in curDataTable.Rows ) {
                    winStatusMsg.Text = "Loading information for " + (String) curDataRow["SkierName"];

                    curMemberId = (String) curDataRow["MemberId"];
                    curViewIdx = tourRegDataGridView.Rows.Add();
                    curViewRow = tourRegDataGridView.Rows[curViewIdx];

                    curViewRow.Cells["PK"].Value = ( (Int64) curDataRow["PK"] ).ToString();
                    curViewRow.Cells["Updated"].Value = "Y";
                    curViewRow.Cells["SanctionId"].Value = (String) curDataRow["SanctionId"];
                    curViewRow.Cells["MemberId"].Value = (String) curDataRow["MemberId"];
                    curViewRow.Cells["SkierName"].Value = (String) curDataRow["SkierName"];
                    try {
                        curViewRow.Cells["AgeGroup"].Value = (String) curDataRow["AgeGroup"];
                    } catch {
                        curViewRow.Cells["AgeGroup"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ReadyToSki"].Value = (String) curDataRow["ReadyToSki"];
                    } catch {
                        curViewRow.Cells["ReadyToSki"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ReadyForPlcmt"].Value = (String) curDataRow["ReadyForPlcmt"];
                    } catch {
                        curViewRow.Cells["ReadyForPlcmt"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["EntryDue"].Value = ( (Decimal) curDataRow["EntryDue"] ).ToString("###.##");
                    } catch {
                        curViewRow.Cells["EntryDue"].Value = "";
                    }
                    try {
                        curViewRow.Cells["EntryPaid"].Value = ( (Decimal) curDataRow["EntryPaid"] ).ToString("###.##");
                    } catch {
                        curViewRow.Cells["EntryPaid"].Value = "";
                    }
                    try {
                        curViewRow.Cells["PaymentMethod"].Value = (String) curDataRow["PaymentMethod"];
                    } catch {
                        curViewRow.Cells["PaymentMethod"].Value = "";
                    }
                    try {
                        curViewRow.Cells["JumpHeight"].Value = ( (Decimal) curDataRow["JumpHeight"] ).ToString("#.#");
                    } catch {
                        curViewRow.Cells["JumpHeight"].Value = "";
                    }
                    try {
                        curViewRow.Cells["TrickBoat"].Value = (String) curDataRow["TrickBoat"];
                    } catch {
                        curViewRow.Cells["TrickBoat"].Value = "";
                    }
                    try {
                        curViewRow.Cells["AwsaMbrshpPaymt"].Value = ( (Decimal) curDataRow["AwsaMbrshpPaymt"] ).ToString("####.##");
                    } catch {
                        curViewRow.Cells["AwsaMbrshpPaymt"].Value = "";
                    }
                    try {
                        curViewRow.Cells["AwsaMbrshpComment"].Value = (String) curDataRow["AwsaMbrshpComment"];
                    } catch {
                        curViewRow.Cells["AwsaMbrshpComment"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Notes"].Value = (String) curDataRow["Notes"];
                    } catch {
                        curViewRow.Cells["Notes"].Value = "";
                    }

                    if ( SlalomReg.Visible ) {
                        try {
                            curEvent = (String) curDataRow["SlalomEvent"];
                            if ( curEvent.Equals("Slalom") ) {
                                curViewRow.Cells["SlalomReg"].Value = "Y";
                                curViewRow.Cells["SlalomGroup"].ReadOnly = false;
                                try {
                                    curViewRow.Cells["SlalomGroup"].Value = (String) curDataRow["SlalomGroup"];
                                } catch {
                                    curViewRow.Cells["SlalomGroup"].Value = "";
                                }
                            } else {
                                curViewRow.Cells["SlalomReg"].Value = "N";
                                curViewRow.Cells["SlalomGroup"].Value = "";
                                curViewRow.Cells["SlalomGroup"].ReadOnly = true;
                            }
                        } catch {
                            curViewRow.Cells["SlalomReg"].Value = "N";
                            curViewRow.Cells["SlalomGroup"].Value = "";
                            curViewRow.Cells["SlalomGroup"].ReadOnly = true;
                        }
                    }
                    if ( TrickReg.Visible ) {
                        try {
                            curEvent = (String) curDataRow["TrickEvent"];
                            if ( curEvent.Equals("Trick") ) {
                                curViewRow.Cells["TrickReg"].Value = "Y";
                                curViewRow.Cells["TrickGroup"].ReadOnly = false;
                                try {
                                    curViewRow.Cells["TrickGroup"].Value = (String) curDataRow["TrickGroup"];
                                } catch {
                                    curViewRow.Cells["TrickGroup"].Value = "";
                                }
                            } else {
                                curViewRow.Cells["TrickReg"].Value = "N";
                                curViewRow.Cells["TrickGroup"].Value = "";
                                curViewRow.Cells["TrickGroup"].ReadOnly = true;
                            }
                        } catch {
                            curViewRow.Cells["TrickReg"].Value = "N";
                            curViewRow.Cells["TrickGroup"].Value = "";
                            curViewRow.Cells["TrickGroup"].ReadOnly = true;
                        }
                    }
                    if ( JumpReg.Visible ) {
                        try {
                            curEvent = (String) curDataRow["JumpEvent"];
                            if ( curEvent.Equals("Jump") ) {
                                curViewRow.Cells["JumpReg"].Value = "Y";
                                curViewRow.Cells["JumpGroup"].ReadOnly = false;
                                try {
                                    curViewRow.Cells["JumpGroup"].Value = (String) curDataRow["JumpGroup"];
                                } catch {
                                    curViewRow.Cells["JumpGroup"].Value = "";
                                }
                            } else {
                                curViewRow.Cells["JumpReg"].Value = "N";
                                curViewRow.Cells["JumpGroup"].Value = "";
                                curViewRow.Cells["JumpGroup"].ReadOnly = true;
                            }
                        } catch {
                            curViewRow.Cells["JumpReg"].Value = "N";
                            curViewRow.Cells["JumpGroup"].Value = "";
                            curViewRow.Cells["JumpGroup"].ReadOnly = true;
                        }
                    }
                }
                if ( tourRegDataGridView.Rows.Count > curRowIdx ) {
                    myTourRegRowIdx = curRowIdx;
                } else {
                    myTourRegRowIdx = tourRegDataGridView.Rows.Count - 1;
                }
                tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
                setEventCounts();
            }
            winStatusMsg.Text = "Tournament entries retrieved";
            int curRowPos = curRowIdx + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + tourRegDataGridView.Rows.Count.ToString();
            Cursor.Current = Cursors.Default;
        }

        private void dataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show("DataGridView_DataError occurred. \n Context: " + e.Context.ToString()
                + "\n Exception Message: " + e.Exception.Message);
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView) sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void Registration_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TourRegList_Width = this.Size.Width;
                Properties.Settings.Default.TourRegList_Height = this.Size.Height;
                Properties.Settings.Default.TourRegList_Location = this.Location;
            }
        }

        private void Registration_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                try {
                    navSave_Click(null, null);
                    e.Cancel = false;
                } catch ( Exception excp ) {
                    e.Cancel = true;
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            } else {
                e.Cancel = false;
            }
        }

        private void navSave_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:Registration:navSave_Click";
            String curMsg = "";
            try {
                DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];

                String curUpdateStatus = (String) curViewRow.Cells["Updated"].Value;
                String curSanctionId = (String) curViewRow.Cells["SanctionId"].Value;
                String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
                String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
                Int64 curPK = Convert.ToInt64((String) curViewRow.Cells["PK"].Value);
                if ( curUpdateStatus.ToUpper().Equals("Y")
                    && curSanctionId.Length > 1
                    && curMemberId.Length > 1
                    ) {
                    try {
                        String curEntryDue, curEntryPaid, curJumpHeight, curTrickBoat, curNotes
                            , curAwsaMbrshpPaymt, curReadyToSki, curReadyForPlcmt, curPaymentMethod, curAwsaMbrshpComment;

                        try {
                            curJumpHeight = Convert.ToDecimal((String) curViewRow.Cells["JumpHeight"].Value).ToString();
                        } catch {
                            curJumpHeight = "Null";
                        }
                        try {
                            curTrickBoat = (String) curViewRow.Cells["TrickBoat"].Value;
                        } catch {
                            curTrickBoat = "";
                        }
                        try {
                            curReadyToSki = (String) curViewRow.Cells["ReadyToSki"].Value;
                        } catch {
                            curReadyToSki = "";
                        }
                        try {
                            curReadyForPlcmt = (String) curViewRow.Cells["ReadyForPlcmt"].Value;
                        } catch {
                            curReadyForPlcmt = "Y";
                        }
                        try {
                            curEntryDue = Convert.ToDecimal((String) curViewRow.Cells["EntryDue"].Value).ToString();
                        } catch {
                            curEntryDue = "Null";
                        }
                        try {
                            curEntryPaid = Convert.ToDecimal((String) curViewRow.Cells["EntryPaid"].Value).ToString();
                        } catch {
                            curEntryPaid = "Null";
                        }
                        try {
                            curAwsaMbrshpPaymt = Convert.ToDecimal((String) curViewRow.Cells["AwsaMbrshpPaymt"].Value).ToString();
                        } catch {
                            curAwsaMbrshpPaymt = "Null";
                        }
                        try {
                            curPaymentMethod = (String) curViewRow.Cells["PaymentMethod"].Value;
                        } catch {
                            curPaymentMethod = "";
                        }
                        try {
                            curAwsaMbrshpComment = (String) curViewRow.Cells["AwsaMbrshpComment"].Value;
                        } catch {
                            curAwsaMbrshpComment = "";
                        }
                        try {
                            curNotes = (String) curViewRow.Cells["Notes"].Value;
                        } catch {
                            curNotes = "";
                        }

                        StringBuilder curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("Update TourReg Set ");
                        curSqlStmt.Append(" SanctionId = '" + curSanctionId + "'");
                        curSqlStmt.Append(", ReadyToSki = '" + curReadyToSki + "'");
                        curSqlStmt.Append(", ReadyForPlcmt = '" + curReadyForPlcmt + "'");
                        curSqlStmt.Append(", EntryDue = " + curEntryDue);
                        curSqlStmt.Append(", EntryPaid = " + curEntryPaid);
                        curSqlStmt.Append(", PaymentMethod = '" + curPaymentMethod + "'");
                        curSqlStmt.Append(", Weight = Null");
                        curSqlStmt.Append(", JumpHeight = " + curJumpHeight);
                        curSqlStmt.Append(", TrickBoat = '" + curTrickBoat + "'");
                        curSqlStmt.Append(", AwsaMbrshpPaymt = " + curAwsaMbrshpPaymt);
                        curSqlStmt.Append(", AwsaMbrshpComment = '" + curAwsaMbrshpComment + "'");
                        curSqlStmt.Append(", Notes = '" + curNotes + "'");
                        curSqlStmt.Append(", LastUpdateDate = GETDATE()");
                        curSqlStmt.Append(" Where PK = " + curPK.ToString());
                        int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                        Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());

                        String curEventGroup = "";
                        String curEventFlag = "";
                        try {
                            curEventFlag = curViewRow.Cells["SlalomReg"].Value.ToString();
                        } catch {
                            curEventFlag = "";
                        }
                        if ( curEventFlag.Equals("Y") ) {
                            curEventGroup = curViewRow.Cells["SlalomGroup"].Value.ToString();
                            if ( isObjectEmpty(curEventGroup) ) curEventGroup = "";
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update EventReg Set ");
                            curSqlStmt.Append(" EventGroup = '" + curEventGroup + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE() ");
                            curSqlStmt.Append("Where SanctionId = '" + curSanctionId + "'");
                            curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
                            curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
                            curSqlStmt.Append("  AND Event = 'Slalom'");
                            rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                            Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
                        }
                        try {
                            curEventFlag = curViewRow.Cells["TrickReg"].Value.ToString();
                        } catch {
                            curEventFlag = "";
                        }
                        if ( curEventFlag.Equals("Y") ) {
                            curEventGroup = curViewRow.Cells["TrickGroup"].Value.ToString();
                            if ( isObjectEmpty(curEventGroup) ) curEventGroup = "";
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update EventReg Set ");
                            curSqlStmt.Append(" EventGroup = '" + curEventGroup + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE() ");
                            curSqlStmt.Append("Where SanctionId = '" + curSanctionId + "'");
                            curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
                            curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
                            curSqlStmt.Append("  AND Event = 'Trick'");
                            rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                            Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
                        }
                        try {
                            curEventFlag = curViewRow.Cells["JumpReg"].Value.ToString();
                        } catch {
                            curEventFlag = "";
                        }
                        if ( curEventFlag.Equals("Y") ) {
                            curEventGroup = curViewRow.Cells["JumpGroup"].Value.ToString();
                            if ( isObjectEmpty(curEventGroup) ) curEventGroup = "";
                            curSqlStmt = new StringBuilder("");
                            curSqlStmt.Append("Update EventReg Set ");
                            curSqlStmt.Append(" EventGroup = '" + curEventGroup + "'");
                            curSqlStmt.Append(", LastUpdateDate = GETDATE() ");
                            curSqlStmt.Append("Where SanctionId = '" + curSanctionId + "'");
                            curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
                            curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
                            curSqlStmt.Append("  AND Event = 'Jump'");
                            rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                            Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
                        }

                        winStatusMsg.Text = "Changes successfully saved";
                        isDataModified = false;
                    } catch ( Exception excp ) {
                        curMsg = "Error attempting to update skier information \n" + excp.Message;
                        MessageBox.Show(curMsg);
                        Log.WriteFile(curMethodName + curMsg);
                    }
                }
                setEventCounts();
            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier information \n" + excp.Message;
                MessageBox.Show(curMsg);
                Log.WriteFile(curMethodName + curMsg);
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( isDataModified ) {
                try {
                    navSave_Click(null, null);
                    isDataModified = false;
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
            if ( !( isDataModified ) ) {
                loadTourRegView();
            }
        }

        private void navExport_Click( object sender, EventArgs e ) {
            String[] curSelectCommand = new String[7];
            String[] curTableName = { "Tournament", "TourReg", "EventReg", "MemberList", "DivOrder", "OfficialWork", "OfficialWorkAsgmt" };
            ExportData myExportData = new ExportData();

            curSelectCommand[0] = "Select * from Tournament Where SanctionId = '" + mySanctionNum + "'";

            curSelectCommand[1] = "Select * from TourReg ";
            if ( myFilterCmd == null ) {
                curSelectCommand[1] = curSelectCommand[1]
                    + " Where SanctionId = '" + mySanctionNum + "'";
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And " + myFilterCmd;
                } else {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'";
                }
            }

            curSelectCommand[2] = "Select * from EventReg ";
            if ( myFilterCmd == null ) {
                curSelectCommand[2] = curSelectCommand[2]
                    + " Where SanctionId = '" + mySanctionNum + "'";
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And " + myFilterCmd;
                } else {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'";
                }
            }

            curSelectCommand[3] = "Select * from MemberList "
                + " Where EXISTS (Select 1 From TourReg ";
            if ( myFilterCmd == null ) {
                curSelectCommand[3] = curSelectCommand[3]
                + " Where TourReg.MemberId = MemberList.MemberId And SanctionId = '" + mySanctionNum + "') ";
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[3] = curSelectCommand[3]
                        + "Where TourReg.MemberId = MemberList.MemberId And SanctionId = '" + mySanctionNum + "' "
                        + "  And " + myFilterCmd + ") ";
                } else {
                    curSelectCommand[3] = curSelectCommand[3]
                        + "Where TourReg.MemberId = MemberList.MemberId And SanctionId = '" + mySanctionNum + "') ";
                }
            }

            curSelectCommand[4] = "Select * from DivOrder Where SanctionId = '" + mySanctionNum + "'";

            curSelectCommand[5] = "Select * from OfficialWork W Where SanctionId = '" + mySanctionNum + "' ";

            curSelectCommand[6] = "Select * from OfficialWorkAsgmt Where SanctionId = '" + mySanctionNum + "' ";

            myExportData.exportData(curTableName, curSelectCommand);
        }

        private void setEventCounts() {
            DataTable curRegDataTable = null;
            StringBuilder curSqlStmt = new StringBuilder("");

            //SlalomRegCountLabel
            if ( SlalomRegCount.Visible ) {
                curSqlStmt.Append("SELECT count(*) as RegCount From EventReg ");
                curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Slalom' ");
                curRegDataTable = getData(curSqlStmt.ToString());
                if ( curRegDataTable.Rows.Count > 0 ) {
                    SlalomRegCount.Text = ( (int) curRegDataTable.Rows[0]["RegCount"] ).ToString("###0");
                } else {
                    SlalomRegCount.Text = "0";
                }
            }

            //TrickRegCountLabel
            if ( TrickRegCount.Visible ) {
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("SELECT count(*) as RegCount From EventReg ");
                curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Trick' ");
                curRegDataTable = getData(curSqlStmt.ToString());
                if ( curRegDataTable.Rows.Count > 0 ) {
                    TrickRegCount.Text = ( (int) curRegDataTable.Rows[0]["RegCount"] ).ToString("###0");
                } else {
                    TrickRegCount.Text = "0";
                }
            }

            if ( JumpRegCount.Visible ) {
                //JumpRegCountLabel
                curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("SELECT count(*) as RegCount From EventReg ");
                curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Jump' ");
                curRegDataTable = getData(curSqlStmt.ToString());
                if ( curRegDataTable.Rows.Count > 0 ) {
                    JumpRegCount.Text = ( (int) curRegDataTable.Rows[0]["RegCount"] ).ToString("###0");
                } else {
                    JumpRegCount.Text = "0";
                }
            }

        }

        private void navSaveAs_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            //tourRegDataGridView
            myExportData.exportData(tourRegDataGridView);
        }

        private void navSort_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;

                if ( mySortCommand.Length > 0 ) {
                    Properties.Settings.Default.TourRegList_Sort = mySortCommand;
                } else {
                    mySortCommand = "SkierName ASC, AgeGroup ASC";
                    Properties.Settings.Default.TourRegList_Sort = mySortCommand;
                }

                winStatusMsg.Text = "Sorted by " + mySortCommand;
                loadTourRegView();
            }
        }

        private void navFilter_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadTourRegView();
            }
        }

        private void navInsert_Click( object sender, EventArgs e ) {
            // Ensure row focus change processing performed
            if ( isDataModified ) {
                try {
                    isDataModified = false;
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }

            // Open dialog for selecting skiers
            myTourRegAddDialog.ShowDialog(this);

            // Refresh data from database
            if ( myTourRegAddDialog.isDataModified ) {
                loadTourRegView();
            }
        }

		private void navImportRankEquiv_Click( object sender, EventArgs e ) {
			// Ensure row focus change processing performed
			if ( isDataModified ) {
				try {
					isDataModified = false;
					winStatusMsg.Text = "Previous row saved.";
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			// Open dialog for selecting skiers
			myTourRegRankEquivDialog.ShowDialog( this );

			// Refresh data from database
			if ( myTourRegRankEquivDialog.isDataModified ) {
				//loadTourRegView();
			}
		}

		private bool removeRow( DataGridViewRow curViewRow ) {
            String curMethodName = "Tournament:Registration:removeRow";
            bool curReturnValue = true;
            String curMsg = "";
            String curSlalomRegValue = "N", curTrickRegValue = "N", curJumpRegValue = "N";
            String curMemberId = "", curAgeGroup = "";
            StringBuilder curSqlStmt = new StringBuilder("");
            bool curResults = true;

            if ( curViewRow != null ) {
                curMemberId = (String) curViewRow.Cells["MemberId"].Value;
                curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;

                if ( !( isObjectEmpty(curViewRow.Cells["SlalomReg"].Value) ) ) {
                    curSlalomRegValue = (String) curViewRow.Cells["SlalomReg"].Value;
                    if ( curSlalomRegValue.Equals("Y") ) {
                        deleteSlalomEntry(curViewRow, curMemberId, curAgeGroup);
                    }
                }

                if ( !( isObjectEmpty(curViewRow.Cells["TrickReg"].Value) ) ) {
                    curTrickRegValue = (String) curViewRow.Cells["TrickReg"].Value;
                    if ( curTrickRegValue.Equals("Y") ) {
                        deleteTrickEntry(curViewRow, curMemberId, curAgeGroup);
                    }
                }

                if ( !( isObjectEmpty(curViewRow.Cells["JumpReg"].Value) ) ) {
                    curJumpRegValue = (String) curViewRow.Cells["JumpReg"].Value;
                    if ( curJumpRegValue.Equals("Y") ) {
                        deleteJumpEntry(curViewRow, curMemberId, curAgeGroup);
                    }
                }
                if ( curSlalomRegValue.Equals("N")
                    && curTrickRegValue.Equals("N")
                    && curJumpRegValue.Equals("N")
                    ) {
                    try {
                        winStatusMsg.Text = "Skier " + curMemberId + " registration and event entries removed";
                        try {
                            curSqlStmt = new StringBuilder("Delete TourReg "
                                + " Where MemberId = '" + curMemberId + "'"
                                + " And SanctionId = '" + mySanctionNum + "'"
                                + " And AgeGroup = '" + curAgeGroup + "'");
                            int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());

                            if ( !( isMemberInTour(curMemberId) ) ) {
                                curSqlStmt = new StringBuilder("Delete OfficialWork "
                                    + " Where MemberId = '" + curMemberId + "'"
                                    + " And SanctionId = '" + mySanctionNum + "'");
                                rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());

                                curSqlStmt = new StringBuilder("Delete OfficialWorkAsgmt "
                                    + " Where MemberId = '" + curMemberId + "'"
                                    + " And SanctionId = '" + mySanctionNum + "'");
                                rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                            }
                            curResults = true;
                        } catch ( Exception excp ) {
                            curResults = false;
                            curMsg = "Error attempting to remove skier from tournament \n" + excp.Message;
                            MessageBox.Show(curMsg);
                            Log.WriteFile(curMethodName + curMsg);
                        }


                    } catch ( Exception excp ) {
                        curResults = false;
                        curMsg = "Error attempting to remove current entry \n" + excp.Message;
                        MessageBox.Show(curMsg);
                        Log.WriteFile(curMethodName + curMsg);
                    }
                }
            }

            return curReturnValue;
        }

        private void navRemove_Click( object sender, EventArgs e ) {
            if ( tourRegDataGridView.SelectedRows.Count > 0 ) {
                foreach ( DataGridViewRow curViewRow in tourRegDataGridView.SelectedRows ) {
                    bool curResults = removeRow(curViewRow);
                }
                loadTourRegView();
				if ( tourRegDataGridView.Rows.Count > 0 ) {
					tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
				}
			} else {
                bool curResults = removeRow(tourRegDataGridView.Rows[myTourRegRowIdx]);
                if ( curResults ) {
                    loadTourRegView();
					if ( tourRegDataGridView.Rows.Count > 0 ) {
						tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
					}
				}
			}
        }

        private void tourRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView) sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + curView.Rows.Count.ToString();

            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSave_Click(null, null);
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
            if ( !( isDataModified ) ) {
                //Sent current tournament registration row
                if ( curView.Rows[e.RowIndex].Cells[e.ColumnIndex] != null ) {
                    if ( !( isObjectEmpty(curView.Rows[e.RowIndex].Cells["MemberId"].Value) ) ) {
                        myTourRegRowIdx = e.RowIndex;
                        isDataModified = false;
                    }
                }
            }
        }

        private void tourRegDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( tourRegDataGridView.Rows.Count > 0 ) {
                if ( !( tourRegDataGridView.Columns[e.ColumnIndex].ReadOnly ) ) {
                    String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
                    if ( curColName.Equals("JumpHeight")
                        || curColName.Equals("EntryDue")
                        || curColName.Equals("EntryPaid")
                        || curColName.Equals("AwsaMbrshpPaymt")
                        || curColName.Equals("PaymentMethod")
                        || curColName.Equals("AwsaMbrshpComment")
                        || curColName.Equals("TrickBoat")
                        || curColName.Equals("Notes")
                        || curColName.Equals("ReadyToSki")
                        || curColName.Equals("ReadyForPlcmt")
                        || curColName.Equals("SlalomReg")
                        || curColName.Equals("SlalomGroup")
                        || curColName.Equals("TrickReg")
                        || curColName.Equals("TrickGroup")
                        || curColName.Equals("JumpReg")
                        || curColName.Equals("JumpGroup")
                        ) {
                        try {
                            myOrigItemValue = (String) tourRegDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        } catch {
                            myOrigItemValue = "";
                        }
                    }
                }
            }
        }

        private void tourRegDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
            String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
            if ( curColName.Equals("SlalomReg")
                || curColName.Equals("TrickReg")
                || curColName.Equals("JumpReg")
                ) {
                SendKeys.Send("{TAB}");
            }
        }

        private void tourRegDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            if ( tourRegDataGridView.Rows.Count > 0 ) {
                String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = tourRegDataGridView.Rows[e.RowIndex];
                if ( curColName.Equals("EntryDue")
                    || curColName.Equals("EntryPaid")
                    || curColName.Equals("AwsaMbrshpPaymt")
                    || curColName.Equals("JumpHeight")
                ) {
                    if ( isObjectEmpty(e.FormattedValue) ) {
                        e.Cancel = false;
                    } else {
                        try {
                            Decimal curNum = Convert.ToDecimal(e.FormattedValue.ToString());
                            e.Cancel = false;
                        } catch {
                            e.Cancel = true;
                            MessageBox.Show(curColName + " must be numeric");
                        }
                    }
                }
            }

        }

        private void tourRegDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            tourRegDataGridView.CurrentRow.ErrorText = "";
            myTourRegRowIdx = e.RowIndex;
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];

            if ( tourRegDataGridView.Rows.Count > 0 ) {
                String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals("SlalomReg") ) {
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 5;
                        curTimerObj.Tick += new EventHandler(deleteSkierFromSlalomTimer);
                        curTimerObj.Start();
                    } else {
                        String curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigItemValue ) {
                            if ( curValue.Equals("Y") ) {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(addSkierToSlalomTimer);
                                curTimerObj.Start();
                            } else {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(deleteSkierFromSlalomTimer);
                                curTimerObj.Start();
                            }
                        }
                    }
                } else if ( curColName.Equals("TrickReg") ) {
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 5;
                        curTimerObj.Tick += new EventHandler(deleteSkierFromTrickTimer);
                        curTimerObj.Start();

                    } else {
                        String curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigItemValue ) {
                            if ( curValue.Equals("Y") ) {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(addSkierToTrickTimer);
                                curTimerObj.Start();
                            } else {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(deleteSkierFromTrickTimer);
                                curTimerObj.Start();
                            }
                        }
                    }
                } else if ( curColName.Equals("JumpReg") ) {
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 5;
                        curTimerObj.Tick += new EventHandler(deleteSkierFromJumpTimer);
                        curTimerObj.Start();

                    } else {
                        String curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigItemValue ) {
                            if ( curValue.Equals("Y") ) {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(addSkierToJumpTimer);
                                curTimerObj.Start();
                            } else {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 5;
                                curTimerObj.Tick += new EventHandler(deleteSkierFromJumpTimer);
                                curTimerObj.Start();
                            }
                        }
                    }

                } else if ( curColName.Equals("ReadyForPlcmt") ) {
                    String curValue = "";
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        curValue = "";
                    } else {
                        curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                    }
                    if ( curValue != myOrigItemValue ) {
                        isDataModified = true;
                        curViewRow.Cells["Updated"].Value = "Y";
                        String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
                        String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
                        updateEventRegReadyForPlcmt(curMemberId, curAgeGroup, curValue);
                    }

                } else if ( curColName.Equals("SlalomGroup")
                    || curColName.Equals("TrickGroup")
                    || curColName.Equals("JumpGroup")
                       ) {
                    String curValue = "";
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        curValue = "";
                    } else {
                        curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                    }
                    if ( curValue != myOrigItemValue ) {
                        isDataModified = true;
                        curViewRow.Cells["Updated"].Value = "Y";
                    }

                } else if ( curColName.Equals("EntryDue")
                    || curColName.Equals("EntryPaid")
                    || curColName.Equals("AwsaMbrshpPaymt")
                    || curColName.Equals("JumpHeight")
                    ) {
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        if ( myOrigItemValue.Length > 0 ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        Decimal curNum = Convert.ToDecimal((String) curViewRow.Cells[e.ColumnIndex].Value);
                        if ( curNum.ToString() != myOrigItemValue ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }

                } else if ( curColName.Equals("PaymentMethod")
                        || curColName.Equals("AwsaMbrshpComment")
                        || curColName.Equals("TrickBoat")
                        || curColName.Equals("Notes")
                        || curColName.Equals("ReadyToSki")
                        || curColName.Equals("ReadyForPlcmt")
                    ) {
                    String curValue = "";
                    if ( isObjectEmpty(curViewRow.Cells[e.ColumnIndex].Value) ) {
                        curValue = "";
                    } else {
                        curValue = (String) curViewRow.Cells[e.ColumnIndex].Value;
                    }
                    if ( curValue != myOrigItemValue ) {
                        isDataModified = true;
                        curViewRow.Cells["Updated"].Value = "Y";
                    }
                }
            }
        }

        private void addSkierToSlalomTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(addSkierToSlalomTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            //if ( myTourEventReg.addEventSlalom( curMemberId, curAgeGroup, (String)myTourRow["Class"], curAgeGroup, "" ) ) {
            if ( myTourEventReg.addEventSlalom(curMemberId, curAgeGroup, "", curAgeGroup, "") ) {
                curViewRow.Cells["SlalomGroup"].ReadOnly = false;
                curViewRow.Cells["SlalomGroup"].Value = curAgeGroup;
            } else {
                curViewRow.Cells["SlalomReg"].Value = "N";
            }
        }

        private void addSkierToTrickTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(addSkierToTrickTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            //if ( myTourEventReg.addEventTrick( curMemberId, curAgeGroup, (String)myTourRow["Class"], curAgeGroup, "" ) ) {
            if ( myTourEventReg.addEventTrick(curMemberId, curAgeGroup, "", curAgeGroup, "") ) {
                curViewRow.Cells["TrickGroup"].ReadOnly = false;
                curViewRow.Cells["TrickGroup"].Value = curAgeGroup;
            } else {
                curViewRow.Cells["TrickReg"].Value = "N";
            }
        }

        private void addSkierToJumpTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(addSkierToJumpTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            //if ( myTourEventReg.addEventJump( curMemberId, curAgeGroup, (String)myTourRow["Class"], curAgeGroup, "" ) ) {
            if ( myTourEventReg.addEventJump(curMemberId, curAgeGroup, "", curAgeGroup, "") ) {
                curViewRow.Cells["JumpGroup"].ReadOnly = false;
                if ( curAgeGroup.ToUpper().Equals("B1") ) {
                    curAgeGroup = "B2";
                } else if ( curAgeGroup.ToUpper().Equals("G1") ) {
                    curAgeGroup = "G2";
                }
                curViewRow.Cells["JumpGroup"].Value = curAgeGroup;
            } else {
                curViewRow.Cells["JumpReg"].Value = "N";
            }
        }

        private void deleteSkierFromSlalomTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(deleteSkierFromSlalomTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            deleteSlalomEntry(curViewRow, curMemberId, curAgeGroup);
        }
        private void deleteSkierFromTrickTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(deleteSkierFromTrickTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            deleteTrickEntry(curViewRow, curMemberId, curAgeGroup);
        }
        private void deleteSkierFromJumpTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(deleteSkierFromJumpTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String) curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String) curViewRow.Cells["AgeGroup"].Value;
            deleteJumpEntry(curViewRow, curMemberId, curAgeGroup);
        }

        private bool deleteSlalomEntry( DataGridViewRow curViewRow, String inMemberId, String inAgeGroup ) {
            bool returnStatus = true;

            try {
                returnStatus = myTourEventReg.deleteSlalomEntry(inMemberId, inAgeGroup);
                if ( returnStatus ) {
                    curViewRow.Cells["SlalomReg"].Value = "N";
                    curViewRow.Cells["SlalomGroup"].Value = "";
                    curViewRow.Cells["SlalomGroup"].ReadOnly = true;
                    returnStatus = true;
                } else {
                    curViewRow.Cells["SlalomReg"].Value = "Y";
                }
            } catch {
                returnStatus = false;
            }

            return returnStatus;
        }

        private bool deleteTrickEntry( DataGridViewRow curViewRow, String inMemberId, String inAgeGroup ) {
            bool returnStatus = true;

            try {
                returnStatus = myTourEventReg.deleteTrickEntry(inMemberId, inAgeGroup);
                if ( returnStatus ) {
                    curViewRow.Cells["TrickReg"].Value = "N";
                    curViewRow.Cells["TrickGroup"].Value = "";
                    curViewRow.Cells["TrickGroup"].ReadOnly = true;
                    returnStatus = true;
                } else {
                    curViewRow.Cells["TrickReg"].Value = "Y";
                }
            } catch {
                returnStatus = false;
            }

            return returnStatus;
        }

        private bool deleteJumpEntry( DataGridViewRow curViewRow, String inMemberId, String inAgeGroup ) {
            bool returnStatus = true;

            try {
                returnStatus = myTourEventReg.deleteJumpEntry(inMemberId, inAgeGroup);
                if ( returnStatus ) {
                    curViewRow.Cells["JumpReg"].Value = "N";
                    curViewRow.Cells["JumpGroup"].Value = "";
                    curViewRow.Cells["JumpGroup"].ReadOnly = true;
                    returnStatus = true;
                } else {
                    curViewRow.Cells["JumpReg"].Value = "Y";
                }
            } catch {
                returnStatus = false;
            }

            return returnStatus;
        }

        private bool updateEventRegReadyForPlcmt( String curMemberId, String curAgeGroup, String curReadyForPlcmt ) {
            bool returnStatus = true;

            try {
                StringBuilder curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Update EventReg Set ");
                curSqlStmt.Append("  ReadyForPlcmt = '" + curReadyForPlcmt + "' ");
                curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "' ");
                curSqlStmt.Append("  And MemberId = '" + curMemberId + "' ");
                curSqlStmt.Append("  And AgeGroup = '" + curAgeGroup + "' ");
                int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());

            } catch {
                returnStatus = false;
            }

            return returnStatus;
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

        private void tourRegDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;

            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSave_Click( null, null );
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( e.RowIndex >= 0 ) {
                    //Sent current tournament registration row
                    if ( !( isObjectEmpty( curView.Rows[myTourRegRowIdx].Cells["MemberId"].Value ) ) ) {
                        myTourRegRowIdx = e.RowIndex;
                        isDataModified = false;

                        // Display the form as a modal dialog box.
                        String curMemberId = curView.Rows[myTourRegRowIdx].Cells["MemberId"].Value.ToString();
                        String curAgeGroup = curView.Rows[myTourRegRowIdx].Cells["AgeGroup"].Value.ToString();
                        myEditRegMemberDialog.editMember( curMemberId, curAgeGroup );
                        myEditRegMemberDialog.ShowDialog( this );

                        // Determine if the OK button was clicked on the dialog box.
                        if ( myEditRegMemberDialog.DialogResult == DialogResult.OK ) {
                            loadTourRegView();
                        }
                    }
                }
            }
        }

        private void navEdit_Click( object sender, EventArgs e ) {
            DataGridView curView = tourRegDataGridView;

            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSave_Click( null, null );
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                //Sent current tournament registration row
                if ( !( isObjectEmpty( curView.Rows[myTourRegRowIdx].Cells["MemberId"].Value ) ) ) {
                    isDataModified = false;

                    // Display the form as a modal dialog box.
                    String curMemberId = (String)curView.Rows[myTourRegRowIdx].Cells["MemberId"].Value;
                    String curAgeGroup = (String)curView.Rows[myTourRegRowIdx].Cells["AgeGroup"].Value;
                    myEditRegMemberDialog.editMember( curMemberId, curAgeGroup );
                    myEditRegMemberDialog.ShowDialog( this );

                    // Determine if the OK button was clicked on the dialog box.
                    if ( myEditRegMemberDialog.DialogResult == DialogResult.OK ) {
                        loadTourRegView();
                    }
                }
            }
            
        }

        private DataTable getTourRegData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT R.PK, R.MemberId, R.SanctionId, R.SkierName, R.AgeGroup, ");
            curSqlStmt.Append("R.EntryDue, R.EntryPaid, R.PaymentMethod, R.ReadyToSki, R.ReadyForPlcmt, R.AwsaMbrshpPaymt, R.AwsaMbrshpComment, ");
            curSqlStmt.Append( "R.TrickBoat, R.JumpHeight, R.Notes, " );
            curSqlStmt.Append( "S.Event AS SlalomEvent, S.EventGroup AS SlalomGroup, " );
            curSqlStmt.Append( "T.Event AS TrickEvent, T.EventGroup AS TrickGroup, " );
            curSqlStmt.Append( "J.Event AS JumpEvent, J.EventGroup AS JumpGroup " );
            curSqlStmt.Append( "FROM TourReg AS R" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup AND S.Event = 'Slalom'" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS T ON T.SanctionId = R.SanctionId AND T.MemberId = R.MemberId AND T.AgeGroup = R.AgeGroup AND T.Event = 'Trick'" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS J ON J.SanctionId = R.SanctionId AND J.MemberId = R.MemberId AND J.AgeGroup = R.AgeGroup AND J.Event = 'Jump'" );
            curSqlStmt.Append( " WHERE R.SanctionId = '" + mySanctionNum + "'" );
            curSqlStmt.Append( "ORDER BY R.SkierName, R.AgeGroup " );
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

        private bool isMemberInTour(String inMemberId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT R.PK, R.MemberId, R.SanctionId, R.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS R " );
            curSqlStmt.Append( "WHERE R.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  And R.MemberId = '" + inMemberId + "'" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return true;
            } else {
                return false;
            }
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

	}
}
