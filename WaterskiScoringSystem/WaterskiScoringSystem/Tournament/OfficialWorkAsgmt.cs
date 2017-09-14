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
    public partial class OfficialWorkAsgmt : Form {
        private Boolean isDataModified = false;
        private Boolean isSetMemberActive = false;
        private Boolean isLoadActive = false;
        private String mySanctionNum;
        private String mySortCommand;
        private String myFilterCmd;
        private String myOrigTextValue;
        private String myEvent;
        private String myTourRules;
        private int myViewRowIdx;
        private int myTourRounds;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        private DataRow myTourRow;
        private ArrayList myEventGroupDropdownList = new ArrayList();
        private DataTable myWorkAsgmtListDataTable;
        private DataTable myOfficialWorkAsgmtDataTable;
        private DataTable myListTourMemberDataTable;

        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        [System.Runtime.InteropServices.DllImport( "user32.dll" )]
        private static extern IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wp, IntPtr lp );

        public OfficialWorkAsgmt() {
            InitializeComponent();
        }

        private void OfficialWorkAsgmt_Load( object sender, EventArgs e ) {
            if (Properties.Settings.Default.OfficialWorkAsgmt_Width > 0) {
                this.Width = Properties.Settings.Default.OfficialWorkAsgmt_Width;
            }
            if (Properties.Settings.Default.OfficialWorkAsgmt_Height > 0) {
                this.Height = Properties.Settings.Default.OfficialWorkAsgmt_Height;
            }
            if (Properties.Settings.Default.OfficialWorkAsgmt_Location.X > 0
                && Properties.Settings.Default.OfficialWorkAsgmt_Location.Y > 0) {
                this.Location = Properties.Settings.Default.OfficialWorkAsgmt_Location;
            }
            if ( Properties.Settings.Default.OfficialWorkAsgmt_Sort.Length > 0 ) {
                mySortCommand = Properties.Settings.Default.OfficialWorkAsgmt_Sort;
            } else {
                mySortCommand = "";
                Properties.Settings.Default.OfficialWorkAsgmt_Sort = mySortCommand;
            }
            labelMemberSelect.Visible = false;
            labelMemberQuickFind.Visible = false;
            listTourMemberDataGridView.Visible = false;

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = officialWorkAsgmtDataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = officialWorkAsgmtDataGridView.Columns;

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum.Trim();
            Cursor.Current = Cursors.WaitCursor;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];

                        myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                        myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                        setEventList();
                        setAsgmtList();

                        roundActiveSelect.SelectList_LoadHorztl( myTourRounds.ToString(), roundActiveSelect_Click );
                        roundActiveSelect.RoundValue = "1";
                        myViewRowIdx = 0;
                        isLoadActive = true;
                        EventButtonAll.Checked = true;
                        isLoadActive = false;
                        //roundActiveSelect.Visible = false;
                        //activeLabel.Visible = false;
                        //loadTourMemberView();
                        //navRefresh_Click( null, null );
                    } else {
                        MessageBox.Show( "The active tournament is not properly defined.  You must select from the Administration menu Tournament List option" );
                    }
                }
            }
        }

        private void OfficialWorkAsgmt_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.OfficialWorkAsgmt_Width = this.Size.Width;
                Properties.Settings.Default.OfficialWorkAsgmt_Height = this.Size.Height;
                Properties.Settings.Default.OfficialWorkAsgmt_Location = this.Location;
            }
        }

        private void OfficialWorkAsgmt_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                checkModifyPrompt();
                if ( isDataModified ) {
                    e.Cancel = true;
                }
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            try {
                if ( isDataModified ) {
                    checkModifyPrompt();
                    isDataModified = false;
                }
                winStatusMsg.Text = "Retrieving tournament official assignment list";
                Cursor.Current = Cursors.WaitCursor;

                loadTourMemberView();
                navRefreshByEvent();
                isDataModified = false;

            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve tournament official assignments\n" + ex.Message );
            }
        }

        private void loadTourMemberView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;
            isLoadActive = true;

            try {
                listTourMemberDataGridView.Rows.Clear();
                myListTourMemberDataTable = getTourMemberList();
                listTourMemberDataGridView.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                    winStatusMsg.Text = "Tournament member list retrieved";
                } );

                if ( myListTourMemberDataTable.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    int curViewIdx = 0;
                    foreach ( DataRow curDataRow in myListTourMemberDataTable.Rows ) {
                        curViewIdx = listTourMemberDataGridView.Rows.Add();
                        curViewRow = listTourMemberDataGridView.Rows[curViewIdx];

                        curViewRow.Cells["SanctionIdTour"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberIdTour"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierNameTour"].Value = (String)curDataRow["SkierName"];
                        try {
                            curViewRow.Cells["FederationTour"].Value = (String)curDataRow["Federation"];
                        } catch {
                            curViewRow.Cells["FederationTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["JudgeSlalomRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["JudgeSlalomRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["JudgeSlalomRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["JudgeSlalomRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["JudgeTrickRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["JudgeTrickRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["JudgeTrickRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["JudgeTrickRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["JudgeJumpRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["JudgeJumpRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["JudgeJumpRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["JudgeJumpRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["SafetyOfficialRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["SafetyOfficialRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["SafetyOfficialRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["SafetyOfficialRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["ScorerSlalomRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["ScorerSlalomRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["ScorerSlalomRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["ScorerSlalomRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["ScorerTrickRatingDesc"];
                            if (curValue.ToUpper().Equals( "UNRATED" )) {
                                curViewRow.Cells["ScorerTrickRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["ScorerTrickRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["ScorerTrickRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["ScorerJumpRatingDesc"];
                            if (curValue.ToUpper().Equals( "UNRATED" )) {
                                curViewRow.Cells["ScorerJumpRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["ScorerJumpRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["ScorerJumpRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["TechOfficialRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["TechOfficialRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["TechOfficialRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["TechOfficialRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["DriverSlalomRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["DriverSlalomRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["DriverSlalomRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["DriverSlalomRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["DriverTrickRatingDesc"];
                            if (curValue.ToUpper().Equals( "UNRATED" )) {
                                curViewRow.Cells["DriverTrickRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["DriverTrickRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["DriverTrickRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["DriverJumpRatingDesc"];
                            if (curValue.ToUpper().Equals( "UNRATED" )) {
                                curViewRow.Cells["DriverJumpRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["DriverJumpRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["DriverJumpRatingDescTour"].Value = "";
                        }
                        try {
                            String curValue = (String)curDataRow["AnncrOfficialRatingDesc"];
                            if ( curValue.ToUpper().Equals( "UNRATED" ) ) {
                                curViewRow.Cells["AnncrOfficialRatingDescTour"].Value = "";
                            } else {
                                curViewRow.Cells["AnncrOfficialRatingDescTour"].Value = curValue;
                            }
                        } catch {
                            curViewRow.Cells["AnncrOfficialRatingDescTour"].Value = "";
                        }
                    }
                    curViewIdx = listTourMemberDataGridView.Rows.Add();
                    listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[0].Cells["SkierNameTour"];
                    Cursor.Current = Cursors.Default;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve available tournament officials \n" + ex.Message );
            }
            isLoadActive = false;
        }

        private void navRefreshByEvent(  ) {
            try {
                if ( isDataModified ) {
                    checkModifyPrompt();
                    isDataModified = false;
                }
                winStatusMsg.Text = "Retrieving tournament official assignment list";
                Cursor.Current = Cursors.WaitCursor;

                if ( myEvent.Equals( "All" ) ) {
                    myOfficialWorkAsgmtDataTable = getOfficialWorkAsgmt();
                } else {
                    String curGroup = "";
                    if ( isObjectEmpty( EventGroupList.SelectedItem ) ) {
                        curGroup = "";
                    } else {
                        curGroup = EventGroupList.SelectedItem.ToString();
                    }
                    String curRound = roundActiveSelect.RoundValue;
                    activeLabel.Visible = true;

                    if ( myListTourMemberDataTable == null ) {
                        loadTourMemberView();
                    } else {
                        if ( myListTourMemberDataTable.Rows.Count == 0 ) {
                            loadTourMemberView();
                        }
                    }
                    myOfficialWorkAsgmtDataTable = getOfficialWorkAsgmt( myEvent, curGroup, curRound );
                }
                loadOfficialWorkAsgmtView();
            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve tournament official assignments\n" + ex.Message );
            }
        }

        private void loadOfficialWorkAsgmtView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                officialWorkAsgmtDataGridView.Rows.Clear();
                myOfficialWorkAsgmtDataTable.DefaultView.Sort = mySortCommand;
                myOfficialWorkAsgmtDataTable.DefaultView.RowFilter = myFilterCmd;
                DataTable curDataTable = myOfficialWorkAsgmtDataTable.DefaultView.ToTable();

                if ( curDataTable.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    int curViewIdx = 0;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        curViewIdx = officialWorkAsgmtDataGridView.Rows.Add();
                        curViewRow = officialWorkAsgmtDataGridView.Rows[curViewIdx];

                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["Updated"].Value = "N";
                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["MemberName"].Value = (String)curDataRow["MemberName"];
                        try {
                            curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        } catch {
                            curViewRow.Cells["Event"].Value = "";
                        }
                        try {
                            curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                        } catch {
                            curViewRow.Cells["EventGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["Round"].Value = ( (Byte)curDataRow["Round"] ).ToString();
                        } catch {
                            curViewRow.Cells["Round"].Value = "";
                        }
                        try {
                            curViewRow.Cells["WorkAsgmt"].Value = (String)curDataRow["WorkAsgmt"];
                        } catch {
                            curViewRow.Cells["WorkAsgmt"].Value = "";
                        }
                        try {
                            curViewRow.Cells["StartTime"].Value = ( (DateTime)curDataRow["StartTime"] ).ToString( "MM/dd/yy hh:mm tt" );
                        } catch {
                            curViewRow.Cells["StartTime"].Value = "";
                        }
                        try {
                            curViewRow.Cells["EndTime"].Value = ( (DateTime)curDataRow["EndTime"] ).ToString( "MM/dd/yy hh:mm tt" );
                        } catch {
                            curViewRow.Cells["EndTime"].Value = "";
                        }
                        try {
                            curViewRow.Cells["Notes"].Value = (String)curDataRow["Notes"];
                        } catch {
                            curViewRow.Cells["Notes"].Value = "";
                        }
                    }
                    if ( myEvent.Equals( "All" ) ) {
                        officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[curViewIdx].Cells["WorkAsgmt"];
                    } else {
                        if ( isObjectEmpty( EventGroupList.SelectedItem ) ) {
                            officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[curViewIdx].Cells["WorkAsgmt"];
                        } else {
                            navAddNewItem_Click( null, null );
                        }
                    }
                } else {
                    if ( myEvent.Equals( "All" ) ) {
                    } else {
                        if ( isObjectEmpty( EventGroupList.SelectedItem ) ) {
                        } else {
                            navAddNewItem_Click( null, null );
                        }
                    }
                }
                Cursor.Current = Cursors.Default;
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament official assignments \n" + ex.Message );
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
                    isSetMemberActive = false;
                }
            }
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void saveDataRow( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( saveDataRow );
            navSave_Click( null, null );
        }

        private void navSave_Click( object sender, EventArgs e ) {
            try {
                this.Validate();

                if ( saveOfficialWorkAsgmt( myViewRowIdx ) ) {
                    isDataModified = false;
                    isSetMemberActive = false;
                    navAddNewItem_Click( null, null );
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveOfficialWorkAsgmt( int inRowIdx ) {
            bool curReturn = true;
            int rowsProc;
            Int64 curPK = 0;
            try {
                DataGridViewRow curViewRow = officialWorkAsgmtDataGridView.Rows[inRowIdx];
                String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );
                if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                    if (validateRow( myViewRowIdx )) {
                        try {
                            myDbConn.Open();
                            mySqlStmt = myDbConn.CreateCommand();
                            mySqlStmt.CommandText = "";

                            String curSanctionId = (String)curViewRow.Cells["SanctionId"].Value;
                            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
                            String curEvent = curViewRow.Cells["Event"].Value.ToString();
                            String curEventGroup = curViewRow.Cells["EventGroup"].Value.ToString();
                            String curRound = curViewRow.Cells["Round"].Value.ToString();
                            String curStartTime = curViewRow.Cells["StartTime"].Value.ToString();
                            String curEndTime = "";
                            String curWorkAsgmt = curViewRow.Cells["WorkAsgmt"].Value.ToString();
                            /*
                            DataRow[] curFindRows = myWorkAsgmtListDataTable.Select( "CodeValue = '" + curWorkAsgmt + "'" );
                            if ( curFindRows.Length > 0 ) {
                                curWorkAsgmt = (String)curFindRows[0]["ListCode"];
                            }
                             */
                            try {
                                curEndTime = curViewRow.Cells["EndTime"].Value.ToString();
                                if (curEndTime.Length > 1) {
                                    curEndTime = "'" + curEndTime + "'";
                                } else {
                                    curEndTime = "Null";
                                }
                            } catch {
                                curEndTime = "Null";
                            }
                            String curNotes = "";
                            try {
                                curNotes = curViewRow.Cells["Notes"].Value.ToString();
                            } catch {
                                curNotes = "";
                            }

                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            if (curPK > 0) {
                                curSqlStmt.Append( "Update OfficialWorkAsgmt Set " );
                                curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                                curSqlStmt.Append( ", Event = '" + curEvent + "'" );
                                curSqlStmt.Append( ", EventGroup = '" + curEventGroup + "'" );
                                curSqlStmt.Append( ", Round = " + curRound );
                                curSqlStmt.Append( ", WorkAsgmt = '" + curWorkAsgmt + "'" );
                                curSqlStmt.Append( ", StartTime = '" + curStartTime + "'" );
                                curSqlStmt.Append( ", EndTime = " + curEndTime );
                                curSqlStmt.Append( ", Notes = '" + curNotes + "'" );
                                curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                                mySqlStmt.CommandText = curSqlStmt.ToString();
                                rowsProc = mySqlStmt.ExecuteNonQuery();
                                if (rowsProc > 0) {
                                    curViewRow.Cells["Updated"].Value = "N";
                                }
                            } else {
                                curSqlStmt.Append( "Insert OfficialWorkAsgmt ( " );
                                curSqlStmt.Append( "SanctionId, MemberId, Event, EventGroup, Round, WorkAsgmt, StartTime, EndTime, Notes " );
                                curSqlStmt.Append( ") Values ( " );
                                curSqlStmt.Append( "'" + curSanctionId + "'" );
                                curSqlStmt.Append( ", '" + curMemberId + "'" );
                                curSqlStmt.Append( ", '" + curEvent + "'" );
                                curSqlStmt.Append( ", '" + curEventGroup + "'" );
                                curSqlStmt.Append( "," + curRound );
                                curSqlStmt.Append( ", '" + curWorkAsgmt + "'" );
                                curSqlStmt.Append( ", '" + curStartTime + "'" );
                                curSqlStmt.Append( "," + curEndTime );
                                curSqlStmt.Append( ", '" + curNotes + "'" );
                                curSqlStmt.Append( ")" );
                                mySqlStmt.CommandText = curSqlStmt.ToString();
                                rowsProc = mySqlStmt.ExecuteNonQuery();
                                if (rowsProc > 0) {
                                    curViewRow.Cells["Updated"].Value = "N";
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "Select max(PK) as MaxPK From OfficialWorkAsgmt" );
                                    DataTable curDataTable = getData( curSqlStmt.ToString() );
                                    if (curDataTable.Rows.Count > 0) {
                                        curPK = (Int64)curDataTable.Rows[0]["MaxPK"];
                                        curViewRow.Cells["PK"].Value = curPK.ToString();
                                    }
                                }
                            }

                            winStatusMsg.Text = "Changes successfully saved";
                            isDataModified = false;
                        } catch (Exception excp) {
                            curReturn = false;
                            MessageBox.Show( "Error attempting to update skier information \n" + excp.Message );
                        } finally {
                            myDbConn.Close();
                        }
                    } else {
                        curReturn = false;
                    }
                }
            } catch ( Exception excp ) {
                curReturn = false;
                MessageBox.Show( "Error attempting to update skier information \n" + excp.Message );
            }

            return curReturn;
        }

        private void roundActiveSelect_Click( object sender, EventArgs e ) {
            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSave_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            if ( !( isDataModified ) ) {
                if ( sender != null ) {
                    String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                    try {
                        roundActiveSelect.RoundValue = curValue;
                        if ( !( isLoadActive ) ) {
                            navRefreshByEvent();
                        }
                    } catch ( Exception ex ) {
                        curValue = ex.Message;
                    }
                }
            }
        }

        private void EventButton_CheckedChanged( object sender, EventArgs e ) {
            String curEventAttr = "";
            RadioButton curButton = (RadioButton)sender;
            if ( curButton.Checked ) {
                if ( curButton.Name.Equals( "EventButtonSlalom" ) ) {
                    myEvent = "Slalom";
                    curEventAttr = "And Event = '" + myEvent + "' ";
                } else if ( curButton.Name.Equals( "EventButtonTrick" ) ) {
                    myEvent = "Trick";
                    curEventAttr = "And Event = '" + myEvent + "' ";
                } else if ( curButton.Name.Equals( "EventButtonJump" ) ) {
                    myEvent = "Jump";
                    curEventAttr = "And Event = '" + myEvent + "' ";
                } else {
                    myEvent = "All";
                    curEventAttr = "";
                }

                if ( myEvent.ToUpper().Equals( "ALL" ) ) {
                    EventGroupList.Visible = false;
                    roundActiveSelect.Visible = false;
                    activeLabel.Visible = false;
                } else {
                    EventGroupList.Visible = true;
                    roundActiveSelect.Visible = true;
                    activeLabel.Visible = true;
                   
                    myEventGroupDropdownList = new ArrayList();
                    //EventGroupList.DataSource = myEventGroupDropdownList;
                    if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                        myEventGroupDropdownList.Add( "Men A" );
                        myEventGroupDropdownList.Add( "Women A" );
                        myEventGroupDropdownList.Add( "Men B" );
                        myEventGroupDropdownList.Add( "Women B" );
                        myEventGroupDropdownList.Add( "Non Team" );
                    } else {
                        String curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                            + "WHERE SanctionId = '" + mySanctionNum + "' "
                            + curEventAttr
                            + "Order by EventGroup";
                        DataTable curDataTable = getData( curSqlStmt );
                        foreach ( DataRow curRow in curDataTable.Rows ) {
                            myEventGroupDropdownList.Add( (String)curRow["EventGroup"] );
                        }
                    }
                    EventGroupList.DataSource = myEventGroupDropdownList;
                }
                /*
                 */
                if ( !( isLoadActive ) ) {
                    navRefreshByEvent();
                }
            }

        }

        private void setEventList() {
            if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
            if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
            if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }

            //Load round selection list based on number of rounds specified for the tournament
            Int16 curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
            myTourRounds = 0;
            try {
                curSlalomRounds = Convert.ToInt16( myTourRow["SlalomRounds"].ToString() );
            } catch {
                curSlalomRounds = 0;
            }
            try {
                curTrickRounds = Convert.ToInt16( myTourRow["TrickRounds"].ToString() );
            } catch {
                curTrickRounds = 0;
            }
            try {
                curJumpRounds = Convert.ToInt16( myTourRow["JumpRounds"].ToString() );
            } catch {
                curJumpRounds = 0;
            }
            if ( curSlalomRounds > myTourRounds ) { myTourRounds = curSlalomRounds; }
            if ( curTrickRounds > myTourRounds ) { myTourRounds = curTrickRounds; }
            if ( curJumpRounds > myTourRounds ) { myTourRounds = curJumpRounds; }

            if ( curSlalomRounds > 0 ) {
                EventButtonSlalom.Visible = true;
            } else {
                EventButtonSlalom.Visible = false;
            }
            if ( curTrickRounds > 0 ) {
                EventButtonTrick.Visible = true;
            } else {
                EventButtonTrick.Visible = false;
            }
            if ( curJumpRounds > 0 ) {
                EventButtonJump.Visible = true;
            } else {
                EventButtonJump.Visible = false;
            }
        }

        private void setAsgmtList() {
            ArrayList curDropdownList = new ArrayList();
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue, SortSeq " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'OfficialAsgmt' " );
            curSqlStmt.Append( "ORDER BY CodeValue" );
            myWorkAsgmtListDataTable = getData( curSqlStmt.ToString() );
            foreach ( DataRow curRow in myWorkAsgmtListDataTable.Rows ) {
                curDropdownList.Add( (String)curRow["CodeValue"] );
            }
            WorkAsgmt.DataSource = curDropdownList;
        }

        private void EventGroupList_SelectedIndexChanged( object sender, EventArgs e ) {
            int curIdx = EventGroupList.SelectedIndex;
            String curValue = EventGroupList.SelectedItem.ToString();
            officialWorkAsgmtDataGridView.Focus();
            navRefreshByEvent();
        }

        private void navAddNewItem_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                try {
                    navSave_Click( null, null );
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( myEvent.ToUpper().Equals( "ALL" ) ) {
                    MessageBox.Show( "Must select an event before a new assignment can be made" );
                } else {
                    string DateString = DateTime.Now.ToString( "MM/dd/yy hh:mm tt" );
                    myViewRowIdx = officialWorkAsgmtDataGridView.Rows.Add();
                    DataGridViewRow curViewRow = officialWorkAsgmtDataGridView.Rows[myViewRowIdx];

                    curViewRow.Cells["PK"].Value = "-1";
                    curViewRow.Cells["Updated"].Value = "Y";
                    curViewRow.Cells["SanctionId"].Value = mySanctionNum;
                    curViewRow.Cells["Event"].Value = myEvent;
                    curViewRow.Cells["EventGroup"].Value = EventGroupList.SelectedValue.ToString();
                    curViewRow.Cells["Round"].Value = roundActiveSelect.RoundValue.ToString();
                    curViewRow.Cells["MemberId"].Value = "";
                    curViewRow.Cells["MemberName"].Value = "";
                    curViewRow.Cells["WorkAsgmt"].Value = "";
                    curViewRow.Cells["StartTime"].Value = DateString;
                    curViewRow.Cells["EndTime"].Value = "";
                    curViewRow.Cells["Notes"].Value = "";
                    officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["WorkAsgmt"];

                    labelMemberQuickFind.Visible = true;
                }
            }
        }

        private void navDeleteItem_Click( object sender, EventArgs e ) {
            try {
                int rowsProc;
                DataGridViewRow curViewRow = officialWorkAsgmtDataGridView.Rows[myViewRowIdx];
                Int64 curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );

                myDbConn.Open();
                mySqlStmt = myDbConn.CreateCommand();
                mySqlStmt.CommandText = "";

                StringBuilder curSqlStmt = new StringBuilder( "" );
                if (curPK > 0) {
                    curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
                    curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    rowsProc = mySqlStmt.ExecuteNonQuery();
                    if (rowsProc > 0) {
                        officialWorkAsgmtDataGridView.Rows.Remove( curViewRow );
                        winStatusMsg.Text = "Current row deleted";
                        isDataModified = false;
                    }
                } else {
                    officialWorkAsgmtDataGridView.Rows.Remove( curViewRow );
                    winStatusMsg.Text = "Current row deleted";
                    isDataModified = false;
                }

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to delete current official assignment \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }
        }

        private void officialWorkAsgmtDataGridView_KeyUp( object sender, KeyEventArgs e ) {
            String curColName = "";
            try {
                if ( e.KeyCode == Keys.Enter ) {
                    curColName = officialWorkAsgmtDataGridView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
                    if ( curColName.Equals( "StartTime" )
                    || curColName.Equals( "EndTime" )
                    || curColName.Equals( "Notes" )
                        ) {
                        e.Handled = true;
                        if (validateRow( myViewRowIdx, false )) {
                            Timer curTimerObj = new Timer();
                            curTimerObj.Interval = 5;
                            curTimerObj.Tick += new EventHandler( saveDataRow );
                            curTimerObj.Start();
                        }
                    }
                }
            } catch ( Exception exp ) {
                MessageBox.Show( "Exception in slalomRecapDataGridView_KeyUp \n" + exp.Message );
            }
        }

        private void officialWorkAsgmtDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;

            //Update data if changes are detected
            if ( isDataModified && (myViewRowIdx != e.RowIndex) ) {
                try {
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 5;
                    curTimerObj.Tick += new EventHandler( saveDataRow );
                    curTimerObj.Start();
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            } else {
                myViewRowIdx = e.RowIndex;
            }
        }

        private void officialWorkAsgmtDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals( "Event" )
                    || curColName.Equals( "EventGroup" )
                    || curColName.Equals( "MemberName" )
                    || curColName.Equals( "WorkAsgmt" )
                    || curColName.Equals( "Notes" )
                    ) {
                    if ( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) {
                        myOrigTextValue = "";
                    } else {
                        myOrigTextValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                } else if ( curColName.Equals( "StartTime" )
                    || curColName.Equals( "EndTime" )
                    ) {
                    if ( isObjectEmpty(curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) ) {
                        myOrigTextValue = "";
                    } else {
                        myOrigTextValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                }

                if ( curColName.Equals( "MemberName" ) ) {
                    isSetMemberActive = true;
                    listTourMemberDataGridView.Visible = true;
                    labelMemberSelect.Visible = true;
                    listTourMemberDataGridView.Visible = true;
                    if ( myOrigTextValue.Length > 0 ) {
                        int curIdx = findMemberRow( myOrigTextValue );
                        listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[curIdx].Cells["SkierNameTour"];
                    } else {
                        labelMemberQuickFind.Visible = true;
                    }
                } else {
                    isSetMemberActive = false;
                    listTourMemberDataGridView.Visible = false;
                    labelMemberSelect.Visible = false;
                    labelMemberQuickFind.Visible = false;
                }
            }
        }

        private void officialWorkAsgmtDataGridView_CellValueChanged( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals( "MemberName" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        int curIdx = findMemberRow( (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value );
                        listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[curIdx].Cells["SkierNameTour"];
                    }
                }
            }

        }

        private void officialWorkAsgmtDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals( "Event" ) ) {
                    if ( isObjectEmpty(curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "EventGroup" ) ) {
                    if ( !(isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "WorkAsgmt" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "MemberName" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "Notes" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
                } else if ( curColName.Equals( "StartTime" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                            curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = curValue.ToUpper();
                        }
                    }
                } else if ( curColName.Equals( "EndTime" ) ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        if ( curValue != myOrigTextValue ) {
                            isDataModified = true;
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                            curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = curValue.ToUpper();
                        }
                    }
                }
            }
        }

        private bool validateRow(int inRowIdx) {
            return validateRow( inRowIdx, true );
        }
        private bool validateRow( int inRowIdx, bool inShowMsg ) {
            DataGridView curView = officialWorkAsgmtDataGridView;
            bool curValidData = true;
            String curValue = "";
            if ( curView.Rows[inRowIdx].Cells["MemberId"].Value != System.DBNull.Value 
                && curView.Rows[inRowIdx].Cells["MemberId"].Value != null ) {
                curValue = (String)curView.Rows[inRowIdx].Cells["MemberId"].Value;
                if ( curValue.Length <= 1 ) {
                    curValidData = false;
                    if ( inShowMsg ) MessageBox.Show( "A member must be assigned" );
                }
            } else {
                curValidData = false;
                if (inShowMsg) MessageBox.Show( "A member must be assigned" );
            }
            if ( curView.Rows[inRowIdx].Cells["Event"].Value != System.DBNull.Value
                && curView.Rows[inRowIdx].Cells["Event"].Value != null ) {
                curValue = (String)curView.Rows[inRowIdx].Cells["Event"].Value;
                if ( curValue.Length <= 1 ) {
                    curValidData = false;
                    if (inShowMsg) MessageBox.Show( "An event must be selected" );
                }
            } else {
                curValidData = false;
                if (inShowMsg) MessageBox.Show( "An event must be selected" );
            }
            if ( curView.Rows[inRowIdx].Cells["EventGroup"].Value != System.DBNull.Value
                && curView.Rows[inRowIdx].Cells["EventGroup"].Value != null ) {
                curValue = (String)curView.Rows[inRowIdx].Cells["EventGroup"].Value;
                if ( curValue.Length < 1 ) {
                    curValidData = false;
                    if (inShowMsg) MessageBox.Show( "An event group must be selected" );
                }
            } else {
                curValidData = false;
                if (inShowMsg) MessageBox.Show( "An event group must be selected" );
            }
            if ( curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value != System.DBNull.Value
                && curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value != null ) {
                curValue = (String)curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value;
                if ( curValue.Length <= 1 ) {
                    curValidData = false;
                    if (inShowMsg) MessageBox.Show( "A work assignment must be assigned" );
                }
            } else {
                curValidData = false;
                if (inShowMsg) MessageBox.Show( "A work assignment must be assigned" );
            }
            return curValidData;
        }

        private void listTourMemberDataGridView_KeyUp(object sender, KeyEventArgs e) {
            try {
                if (e.KeyCode == Keys.Enter) {
                    assignMemberOfficial( (DataGridView)sender, ( (DataGridView)sender ).CurrentCell.RowIndex - 1 );
                }
            } catch ( Exception exp ) {
                MessageBox.Show( exp.Message );
            }

        }

        private void listTourMemberDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            assignMemberOfficial( (DataGridView)sender, e.RowIndex );
        }

        private void assignMemberOfficial( DataGridView inView, int inRowIdx ) {
            if ( !(isObjectEmpty( inView.Rows[inRowIdx].Cells["MemberIdTour"].Value) ) ) {
                String curMemberId = (String)inView.Rows[inRowIdx].Cells["MemberIdTour"].Value;
                String curSkierName = (String)inView.Rows[inRowIdx].Cells["SkierNameTour"].Value;

                officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberId"].Value = curMemberId;
                officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberName"].Value = curSkierName;
                officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["Updated"].Value = "Y";

                isDataModified = true;
                isSetMemberActive = false;
                listTourMemberDataGridView.Visible = false;
                labelMemberSelect.Visible = false;
                officialWorkAsgmtDataGridView.Focus();
                SendKeys.Send( "{TAB}" );
                //officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["StartTime"];
            }
        }

        private int findMemberRow(String inValue) {
            String curMemberName = "";
            int curReturnIdx = 0;
            int curColIdx = listTourMemberDataGridView.Columns["SkierNameTour"].Index;

            if ( inValue.Length > 0 ) {
                int curIdx = 0;
                foreach ( DataGridViewRow curRow in listTourMemberDataGridView.Rows ) {
                    curMemberName = (String)curRow.Cells[curColIdx].Value;
                    if ( curMemberName.Length > inValue.Length ) {
                        if ( curMemberName.StartsWith( inValue, true, null ) ) {
                            curReturnIdx = curIdx;
                            break;
                        } else if ( curMemberName.CompareTo(inValue) >= 0 ) {
                            curReturnIdx = curIdx;
                            break;
                        }
                    } else {
                        if ( curMemberName.CompareTo( inValue ) >= 0 ) {
                            curReturnIdx = curIdx;
                            break;
                        }
                    }
                    curIdx++;
                }
            }

            return curReturnIdx;
        }

        private void navCopyItem_Click(object sender, EventArgs e) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curGroup = "";

            if (myEvent.Equals( "All" )) {
                MessageBox.Show( "You must select an event to be copied" );
            } else {
                if (isObjectEmpty( EventGroupList.SelectedItem )) {
                    MessageBox.Show( "You must select an event group to be copied" );
                } else {
                    curGroup = EventGroupList.SelectedItem.ToString();
                    if (curGroup.ToLower().Equals( "all" )) {
                        MessageBox.Show( "You must select an event group to be copied" );
                    } else {
                        if (officialWorkAsgmtDataGridView.Rows.Count > 0) {
                            String curRound = roundActiveSelect.RoundValue;
                            Int16 curRounds = Convert.ToInt16( myTourRow[myEvent + "Rounds"].ToString() );
                            OfficialWorkAsgmtCopy curDialog = new OfficialWorkAsgmtCopy();

                            curDialog.showAvailable( mySanctionNum, myTourRules, myEvent, curRounds );
                            curDialog.ShowDialog( this );
                            if (curDialog.DialogResult == DialogResult.OK) {
                                String curCopyToRound = curDialog.CopyToRound;
                                String curCopyToGroup = curDialog.CopyToGroup;

                                if (curCopyToRound.Length > 0 && curCopyToGroup.Length > 0) {
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "Select count(*) as OfficialCount " );
                                    curSqlStmt.Append( "From OfficialWorkAsgmt " );
                                    curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                    curSqlStmt.Append( "And Round = " + curCopyToRound + " " );
                                    curSqlStmt.Append( "And EventGroup = '" + curCopyToGroup + "' " );
                                    curSqlStmt.Append( "And Event = '" + myEvent + "' " );

                                    DataTable curDataTable = getData( curSqlStmt.ToString() );
                                    if (curDataTable.Rows.Count > 0) {
                                        if ((int)curDataTable.Rows[0]["OfficialCount"] > 0) {
                                            MessageBox.Show( "Officials already assigned to " + curCopyToGroup + " round " + curCopyToRound );
                                        } else {
                                            try {
                                                myDbConn.Open();
                                                mySqlStmt = myDbConn.CreateCommand();
                                                mySqlStmt.CommandText = "";

                                                curSqlStmt = new StringBuilder( "" );
                                                curSqlStmt.Append( "Insert OfficialWorkAsgmt ( " );
                                                curSqlStmt.Append( "SanctionId, MemberId, Event, EventGroup, Round, WorkAsgmt, StartTime, EndTime, Notes " );
                                                curSqlStmt.Append( ") Select SanctionId, MemberId, Event, '" + curCopyToGroup + "', " + curCopyToRound + ", WorkAsgmt, getdate(), null, null  " );
                                                curSqlStmt.Append( "From OfficialWorkAsgmt " );
                                                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                                curSqlStmt.Append( "And Round = " + curRound + " " );
                                                curSqlStmt.Append( "And EventGroup = '" + curGroup + "' " );
                                                curSqlStmt.Append( "And Event = '" + myEvent + "' " );
                                                mySqlStmt.CommandText = curSqlStmt.ToString();
                                                int rowsProc = mySqlStmt.ExecuteNonQuery();
                                                if (rowsProc > 0) {
                                                    MessageBox.Show( "Rows copied = " + rowsProc );
                                                    winStatusMsg.Text = "Changes successfully saved";

                                                    roundActiveSelect.RoundValue = curCopyToRound;
                                                    EventGroupList.SelectedItem = curCopyToGroup;
                                                    navRefreshByEvent();
                                                }
                                                isDataModified = false;
                                            } catch (Exception excp) {
                                                MessageBox.Show( "Error attempting to copy officials information \n" + excp.Message );
                                            } finally {
                                                myDbConn.Close();
                                            }

                                        }
                                    }
                                }
                            }
                        } else {
                            MessageBox.Show( "No rows available to be copied" );
                        }
                    }
                }
            }
        }

        private void navSort_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;
                Properties.Settings.Default.OfficialWorkReport_Sort = mySortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCommand;
                loadOfficialWorkAsgmtView();
            }
        }

        private void navFilter_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadOfficialWorkAsgmtView();
            }
        }

        private void navExport_Click( object sender, EventArgs e ) {
            String[] curSelectCommand = new String[4];
            String[] curTableName = { "TourReg", "OfficialWork", "OfficialWorkAsgmt", "MemberList" };
            ExportData myExportData = new ExportData();

            curSelectCommand[0] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN OfficialWork ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(myFilterCmd) ) && myFilterCmd.Length > 0 ) {
                curSelectCommand[0] = curSelectCommand[0] + " And " + myFilterCmd;
            }

            curSelectCommand[1] = "Select * from OfficialWork "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "And LastUpdateDate is not null ";
            if ( myFilterCmd != null ) {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1] + " And " + myFilterCmd;
                }
            }

            curSelectCommand[2] = "Select * from OfficialWorkAsgmt "
                + " Where SanctionId = '" + mySanctionNum + "'";
            if ( myFilterCmd != null ) {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[2] = curSelectCommand[2] + " And " + myFilterCmd;
                }
            }

            curSelectCommand[3] = "SELECT XT.* FROM MemberList XT "
                + "INNER JOIN TourReg ER on XT.MemberId = ER.MemberId "
                + "Where ER.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(myFilterCmd) ) && myFilterCmd.Length > 0 ) {
                curSelectCommand[3] = curSelectCommand[3] + " And " + myFilterCmd;
            }

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

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

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDataGrid = new DataGridViewPrinter( officialWorkAsgmtDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getOfficialWorkAsgmt() {
            return getOfficialWorkAsgmt( null, null, null );
        }
        private DataTable getOfficialWorkAsgmt(String inEvent, String inEventGroup, String inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt" );
            curSqlStmt.Append( ", O.StartTime, O.EndTime, O.Notes, T.SkierName AS MemberName " );
            curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
            curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) T " );
            curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
            curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
            if ( inEvent != null ) {
                curSqlStmt.Append( "  AND O.Event = '" + inEvent + "' " );
            }
            if ( inEventGroup != null ) {
                if ( inEventGroup.Length > 0 ) {
                curSqlStmt.Append( "  AND O.EventGroup = '" + inEventGroup + "' " );
                }
            }
            if ( inRound != null ) {
                curSqlStmt.Append( "  AND O.Round = " + inRound + " " );
            }
            curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTourMemberList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT TourReg.SanctionId, TourReg.MemberId, TourReg.SkierName, MemberList.Federation" );
            curSqlStmt.Append( ", OfficialWork.JudgeSlalomRating AS JudgeSlalomRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.JudgeTrickRating AS JudgeTrickRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.JudgeJumpRating AS JudgeJumpRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.ScorerSlalomRating AS ScorerSlalomRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.ScorerTrickRating AS ScorerTrickRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.ScorerJumpRating AS ScorerJumpRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.DriverSlalomRating AS DriverSlalomRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.DriverTrickRating AS DriverTrickRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.DriverJumpRating AS DriverJumpRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.SafetyOfficialRating AS SafetyOfficialRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.TechOfficialRating AS TechOfficialRatingDesc" );
            curSqlStmt.Append( ", OfficialWork.AnncrOfficialRating AS AnncrOfficialRatingDesc " );
            curSqlStmt.Append( "FROM OfficialWork " );
            curSqlStmt.Append( "     INNER JOIN TourReg ON TourReg.MemberId = OfficialWork.MemberId AND TourReg.SanctionId = OfficialWork.SanctionId " );
            curSqlStmt.Append( "     LEFT OUTER JOIN MemberList ON MemberList.MemberId = OfficialWork.MemberId " );
            curSqlStmt.Append( "WHERE TourReg.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "ORDER BY TourReg.SkierName, TourReg.MemberId  " );
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
