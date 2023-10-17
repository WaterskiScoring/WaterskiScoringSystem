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
		private String myMemberSelectFilter;
		private String myTourClass;

		private int myViewRowIdx;
		private int mySearchViewRowIdx;
		private int myTourRounds;

		private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
		private ListSkierClass mySkierClassList;

		private DataRow myTourRow;

		private ArrayList myEventGroupDropdownList = new ArrayList();
        private DataTable myWorkAsgmtListDataTable;
        private DataTable myOfficialWorkAsgmtDataTable;
        private DataTable myListTourMemberDataTable;

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

            // Retrieve and setup tournament event data
            mySanctionNum = Properties.Settings.Default.AppSanctionNum.Trim();
            if ( getTourEventData() ) {
                isLoadActive = true;
                setEventList();
                setAsgmtList();

                roundActiveSelect.SelectList_LoadHorztl( myTourRounds.ToString(), roundActiveSelect_Click );
                roundActiveSelect.RoundValue = "1";
                myViewRowIdx = 0;
                EventButtonAll.Checked = true;

                sortDialogForm = new SortDialogForm();
                sortDialogForm.ColumnList = officialWorkAsgmtDataGridView.Columns;

                filterDialogForm = new Common.FilterDialogForm();
                filterDialogForm.ColumnList = officialWorkAsgmtDataGridView.Columns;

                isLoadActive = false;
            }

            Cursor.Current = Cursors.WaitCursor;
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

				myListTourMemberDataTable = getTourMemberList();
				loadTourMemberView();
                navRefreshByEvent();
                isDataModified = false;
				myViewRowIdx = 0;

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
			DataTable curDataTable = myListTourMemberDataTable;
			curDataTable.DefaultView.RowFilter = "";
			if ( myMemberSelectFilter.Length > 1 ) {
				curDataTable.DefaultView.RowFilter = myMemberSelectFilter;
				curDataTable = curDataTable.DefaultView.ToTable();
			}
			listTourMemberDataGridView.DataSource = curDataTable;

			try {
				//if ( listTourMemberDataGridView.Rows.Count > 0 ) listTourMemberDataGridView.Rows.Clear();
                listTourMemberDataGridView.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                    winStatusMsg.Text = "Tournament member list retrieved";
                } );

				listTourMemberDataGridView.Columns["MemberIdOfficial"].HeaderText = "MemberId";
				listTourMemberDataGridView.Columns["MemberIdOfficial"].ReadOnly = true;
				listTourMemberDataGridView.Columns["MemberNameOfficial"].HeaderText = "SkierName";
				listTourMemberDataGridView.Columns["MemberNameOfficial"].ReadOnly = true;
				listTourMemberDataGridView.Columns["Federation"].HeaderText = "Federation";
				listTourMemberDataGridView.Columns["Federation"].ReadOnly = true;

				listTourMemberDataGridView.Columns["JudgeSlalomRatingDesc"].HeaderText = "Judge Slalom";
				listTourMemberDataGridView.Columns["JudgeSlalomRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["JudgeTrickRatingDesc"].HeaderText = "Judge Trick";
				listTourMemberDataGridView.Columns["JudgeTrickRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["JudgeJumpRatingDesc"].HeaderText = "Judge Jump";
				listTourMemberDataGridView.Columns["JudgeJumpRatingDesc"].ReadOnly = true;

				listTourMemberDataGridView.Columns["ScorerSlalomRatingDesc"].HeaderText = "Scorer Slalom";
				listTourMemberDataGridView.Columns["ScorerSlalomRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["ScorerTrickRatingDesc"].HeaderText = "Scorer Trick";
				listTourMemberDataGridView.Columns["ScorerTrickRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["ScorerJumpRatingDesc"].HeaderText = "Scorer Jump";
				listTourMemberDataGridView.Columns["ScorerJumpRatingDesc"].ReadOnly = true;

				listTourMemberDataGridView.Columns["DriverSlalomRatingDesc"].HeaderText = "Driver Slalom";
				listTourMemberDataGridView.Columns["DriverSlalomRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["DriverTrickRatingDesc"].HeaderText = "Driver Trick";
				listTourMemberDataGridView.Columns["DriverTrickRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["DriverJumpRatingDesc"].HeaderText = "Driver Jump";
				listTourMemberDataGridView.Columns["DriverJumpRatingDesc"].ReadOnly = true;

				listTourMemberDataGridView.Columns["SafetyOfficialRatingDesc"].HeaderText = "Safety";
				listTourMemberDataGridView.Columns["SafetyOfficialRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["TechOfficialRatingDesc"].HeaderText = "Tech";
				listTourMemberDataGridView.Columns["TechOfficialRatingDesc"].ReadOnly = true;
				listTourMemberDataGridView.Columns["AnncrOfficialRatingDesc"].HeaderText = "Announcer";
				listTourMemberDataGridView.Columns["AnncrOfficialRatingDesc"].ReadOnly = true;

				if ( listTourMemberDataGridView.Rows.Count > 0 ) {
					listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[0].Cells["MemberNameOfficial"];
				}
				Cursor.Current = Cursors.Default;

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
                    if ( HelperFunctions.isObjectPopulated( EventGroupList.SelectedItem ) ) curGroup = EventGroupList.SelectedItem.ToString();
                    String curRound = roundActiveSelect.RoundValue;
                    activeLabel.Visible = true;

                    if ( myListTourMemberDataTable == null || myListTourMemberDataTable.Rows.Count == 0 ) {
						myListTourMemberDataTable = getTourMemberList();
						loadTourMemberView();
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
                        curViewRow.Cells["Event"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Event", "" );
                        curViewRow.Cells["EventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                        curViewRow.Cells["Round"].Value = Convert.ToByte( HelperFunctions.getDataRowColValue( curDataRow, "Round", "1" ));
                        curViewRow.Cells["WorkAsgmt"].Value = HelperFunctions.getDataRowColValue( curDataRow, "WorkAsgmt", "" );
                        curViewRow.Cells["StartTime"].Value = HelperFunctions.getDataRowColValue( curDataRow, "StartTime", "" );
                        curViewRow.Cells["EndTime"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EndTime", "" );
                        curViewRow.Cells["Notes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Notes", "" );
                    }
                    if ( myEvent.Equals( "All" ) ) {
                        officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[curViewIdx].Cells["WorkAsgmt"];
                    } else {
                        if ( HelperFunctions.isObjectEmpty( EventGroupList.SelectedItem ) ) {
                            officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[curViewIdx].Cells["WorkAsgmt"];
                        } else {
                            navAddNewItem_Click( null, null );
                        }
                    }
                } else {
                    if ( myEvent.Equals( "All" ) ) {
                    } else {
                        if ( HelperFunctions.isObjectEmpty( EventGroupList.SelectedItem ) ) {
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
					if ( ( myViewRowIdx == ( officialWorkAsgmtDataGridView.Rows.Count - 1) )
						&& ( (String) officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberName"].Value ).Length > 0 ) {
						navAddNewItem_Click( null, null );

					} else {
						myViewRowIdx = officialWorkAsgmtDataGridView.CurrentRow.Index;
						String curColName = officialWorkAsgmtDataGridView.Columns[officialWorkAsgmtDataGridView.CurrentCell.ColumnIndex].Name;
						if ( curColName.Equals( "StartTime" ) || curColName.Equals( "EndTime" ) || curColName.Equals( "Notes" ) ) {
							officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberName"];
                        }
					}
				}

			} catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveOfficialWorkAsgmt( int inRowIdx ) {
			bool curReturn = true;
            int rowsProc;
            Int64 curPK = 0;
			if ( officialWorkAsgmtDataGridView.Rows.Count == 0 ) return true;

			try {
                DataGridViewRow curViewRow = officialWorkAsgmtDataGridView.Rows[inRowIdx];
                String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );

				if ( curViewRow.Cells["MemberId"].Value != System.DBNull.Value
					&& curViewRow.Cells["MemberId"].Value != null
					&& ( (String) curViewRow.Cells["MemberId"].Value ).Length > 1
					&& ( (String) curViewRow.Cells["MemberName"].Value ).Length > 1
					&& ( (String) curViewRow.Cells["Updated"].Value ).Equals( "Y" ) ) {
                    if (validateRow( inRowIdx ) ) {
                        try {
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
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
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
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                if (rowsProc > 0) {
                                    curViewRow.Cells["Updated"].Value = "N";
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "Select max(PK) as MaxPK From OfficialWorkAsgmt" );
                                    DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
                            loadEventGroupList();
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

                loadEventGroupList();
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
            myWorkAsgmtListDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
					DataGridViewRow curViewRow = addWorkAsgmtRow();
				}
			}
        }

		private DataGridViewRow addWorkAsgmtRow() {
			string DateString = DateTime.Now.ToString( "MM/dd/yy hh:mm:ss" );
			DataGridViewRow curViewRow = null;
            try {
				myViewRowIdx = officialWorkAsgmtDataGridView.Rows.Add();
				curViewRow = officialWorkAsgmtDataGridView.Rows[myViewRowIdx];

			} catch {
				try {
					officialWorkAsgmtDataGridView.Rows.Clear();
					myViewRowIdx = 0;
					officialWorkAsgmtDataGridView.Rows.Insert( myViewRowIdx );
					curViewRow = officialWorkAsgmtDataGridView.Rows[myViewRowIdx];
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to add new official row \n" + excp.Message );
				}
			}

			if ( curViewRow != null ) {
				curViewRow.Cells["PK"].Value = "-1";
				curViewRow.Cells["Updated"].Value = "N";
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

				if ( officialWorkAsgmtDataGridView.Rows.Count == 0 ) myViewRowIdx = officialWorkAsgmtDataGridView.Rows.Add(curViewRow);

				officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["WorkAsgmt"];
			}

			labelMemberQuickFind.Visible = true;
			return curViewRow;

		}

		private void navDeleteItem_Click( object sender, EventArgs e ) {
			if ( myViewRowIdx < 0 ) return;

            try {
                int rowsProc;
                DataGridViewRow curViewRow = officialWorkAsgmtDataGridView.Rows[myViewRowIdx];
                Int64 curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );

				StringBuilder curSqlStmt = new StringBuilder( "" );
                if (curPK > 0) {
					curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
                    curSqlStmt.Append( " Where PK = " + curPK.ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                }

				officialWorkAsgmtDataGridView.Rows.Remove( curViewRow );
				winStatusMsg.Text = "Current row deleted";
				isDataModified = false;

				if ( myViewRowIdx >= officialWorkAsgmtDataGridView.Rows.Count ) {
					myViewRowIdx = officialWorkAsgmtDataGridView.Rows.Count - 1;
				}
				if ( officialWorkAsgmtDataGridView.Rows.Count == 0 ) {
					curViewRow = addWorkAsgmtRow();
				}

			} catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to delete current official assignment \n" + excp.Message );
            }
        }

        private void officialWorkAsgmtDataGridView_KeyUp( object sender, KeyEventArgs e ) {
			DataGridView curView = (DataGridView) sender;
			String curColName = "";
            try {
				curColName = curView.Columns[curView.CurrentCell.ColumnIndex].Name;
				if (e.KeyCode == Keys.Enter) {
					if (curColName.Equals("MemberName")) {
						if (((String)curView.Rows[mySearchViewRowIdx].Cells["MemberId"].Value).Length == 0
							&& ((String)curView.Rows[mySearchViewRowIdx].Cells["MemberName"].Value).Length > 0
						) {
							e.SuppressKeyPress = true;
							curView.CurrentCell = curView.Rows[mySearchViewRowIdx].Cells["MemberName"];
							labelMemberSelect.Visible = true;
							listTourMemberDataGridView.Visible = true;
							listTourMemberDataGridView.Focus();
							listTourMemberDataGridView.BackgroundColor = Color.LightCyan;
						
						}

					} else if ((curColName.Equals("StartTime") || curColName.Equals("EndTime") || curColName.Equals("Notes"))
						&& ((String)curView.Rows[mySearchViewRowIdx].Cells["MemberId"].Value).Length > 0
						) {
						if (myViewRowIdx == (curView.Rows.Count - 1)) {
							e.Handled = true;
							if (validateRow(myViewRowIdx, false)) {
								Timer curTimerObj = new Timer();
								curTimerObj.Interval = 5;
								curTimerObj.Tick += new EventHandler(saveDataRow);
								curTimerObj.Start();
							}

						} else if (myViewRowIdx < (curView.Rows.Count - 1)) {
							mySearchViewRowIdx++;
							curView.CurrentCell = curView.Rows[mySearchViewRowIdx].Cells["MemberName"];
						}
					}
				}

			} catch (Exception exp) {
				MessageBox.Show( "Exception in slalomRecapDataGridView_KeyUp \n" + exp.Message );
            }
        }

		private void officialWorkAsgmtDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			if ( curView.Rows.Count <= 0 ) return;

			String curColName = curView.Columns[e.ColumnIndex].Name;

			if ( curColName.Equals( "MemberName" )
					|| curColName.Equals( "WorkAsgmt" )
					|| curColName.Equals( "Notes" )
					) {
                myOrigTextValue = "";
                if ( HelperFunctions.isObjectPopulated( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) {
					myOrigTextValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
				}

			} else if ( curColName.Equals( "StartTime" ) || curColName.Equals( "EndTime" ) ) {
                myOrigTextValue = "";
                if ( HelperFunctions.isObjectPopulated( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) {
                    myOrigTextValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                }
			}

			if ( curColName.Equals( "MemberName" ) ) {
				isSetMemberActive = true;
				setMemberSelectFilter( (String) curView.Rows[e.RowIndex].Cells["WorkAsgmt"].Value );
				if ( HelperFunctions.isObjectPopulated(myOrigTextValue) ) {
                    if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curView.Rows[e.RowIndex], "MemberId", "" ) ) ) {
						int curIdx = findMemberRow(myOrigTextValue);
						if (curIdx >= 0) listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[curIdx].Cells["MemberNameOfficial"];
						labelMemberSelect.Visible = true;
						listTourMemberDataGridView.Visible = true;
						listTourMemberDataGridView.Focus();
						listTourMemberDataGridView.BackgroundColor = Color.LightCyan;

					} else {
						labelMemberSelect.Visible = false;
						listTourMemberDataGridView.Visible = false;
						listTourMemberDataGridView.BackgroundColor = Color.LightGray;
					}

				} else {
					labelMemberSelect.Visible = true;
					listTourMemberDataGridView.Visible = true;
					listTourMemberDataGridView.BackgroundColor = Color.LightGray;
				}

			} else {
				isSetMemberActive = false;
				listTourMemberDataGridView.Visible = false;
				listTourMemberDataGridView.BackgroundColor = Color.LightGray;
				labelMemberSelect.Visible = false;
				labelMemberQuickFind.Visible = false;
			}
		}

		private void officialWorkAsgmtDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView) sender;

			if ( curView.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 ) {
				curView.Rows[e.RowIndex].Cells["EndTime"].Value = DateTime.Now.ToString( "yyyy/MM/dd HH:mm:ss" );
				curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";

				isDataModified = true;
				SendKeys.Send( "{TAB}" );
			}
		}

		private void officialWorkAsgmtDataGridView_CellValueChanged( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			if ( curView.Rows.Count <= 0 ) return;

			String curColName = curView.Columns[e.ColumnIndex].Name;
			if ( curColName.Equals( "MemberName" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					mySearchViewRowIdx = e.RowIndex;
					int curIdx = findMemberRow((String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
					if (curIdx >= 0) {
						listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[curIdx].Cells["MemberNameOfficial"];
					}
					if (isSetMemberActive)curView.Rows[e.RowIndex].Cells["MemberId"].Value = "";
				}
			}
		}

		private void officialWorkAsgmtDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView) sender;
			if ( curView.Rows.Count <= 0 ) return;

			String curColName = curView.Columns[e.ColumnIndex].Name;
			if ( curColName.Equals( "WorkAsgmt" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					String curValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigTextValue ) {
                        String tempMemberId = HelperFunctions.getViewRowColValue( curView.Rows[e.RowIndex], "MemberId", "" );
                        String curEvent = HelperFunctions.getViewRowColValue( curView.Rows[e.RowIndex], "Event", "" );
                        if ( HelperFunctions.isObjectPopulated( tempMemberId ) ) {
                            if ( !(validateMemberForRole( tempMemberId, curValue, curEvent ) ) ) {
                                MessageBox.Show( String.Format( "{0} does not have a rating for the selected '{1}' role "
                                    , HelperFunctions.getViewRowColValue( curView.Rows[e.RowIndex], "MemberName", "" ), curValue ) );
                                curView.Rows[e.RowIndex].Cells["WorkAsgmt"].Value = myOrigTextValue;
                                return;
							}
                            curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
                        }
                    }
				}

			} else if ( curColName.Equals( "MemberName" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					String curValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigTextValue ) {
						if ( curView.Rows[e.RowIndex].Cells["MemberId"].Value != System.DBNull.Value
							&& curView.Rows[e.RowIndex].Cells["MemberId"].Value != null
							&& ( (String) curView.Rows[e.RowIndex].Cells["MemberId"].Value ).Length > 1
							) {
							curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
						}
					}
				}

			} else if ( curColName.Equals( "Notes" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					String curValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigTextValue ) {
						curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
					}
				}

			} else if ( curColName.Equals( "StartTime" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					String curValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigTextValue ) {
						curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
						curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = curValue.ToUpper();
					}
				}

			} else if ( curColName.Equals( "EndTime" ) ) {
				if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ) ) ) {
					String curValue = (String) curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigTextValue ) {
						curView.Rows[e.RowIndex].Cells["Updated"].Value = "Y";
						curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = curValue.ToUpper();
					}
				}
			}

			if ( curView.Rows[e.RowIndex].Cells["MemberId"].Value != System.DBNull.Value
				&& curView.Rows[e.RowIndex].Cells["MemberId"].Value != null
				&& ((String)curView.Rows[e.RowIndex].Cells["MemberId"].Value).Length > 1
				&& ( (String) curView.Rows[e.RowIndex].Cells["Updated"].Value ).Equals( "Y" ) ) {
				isDataModified = true;
			}
		}

		private void officialWorkAsgmtDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView) sender;

			//Update data if changes are detected
			if ( myViewRowIdx != e.RowIndex && myViewRowIdx < curView.Rows.Count ) {

				if ( curView.Rows[myViewRowIdx].Cells["MemberId"].Value != System.DBNull.Value
					&& curView.Rows[myViewRowIdx].Cells["MemberId"].Value != null
					&& ( (String) curView.Rows[myViewRowIdx].Cells["MemberId"].Value ).Length > 1
					&& ( (String) curView.Rows[myViewRowIdx].Cells["MemberName"].Value ).Length > 1
					&& ( (String) curView.Rows[myViewRowIdx].Cells["WorkAsgmt"].Value ).Length > 1
					&& ( (String) curView.Rows[myViewRowIdx].Cells["Updated"].Value ).Equals( "Y" )
					) {
					try {
						Timer curTimerObj = new Timer();
						curTimerObj.Interval = 5;
						curTimerObj.Tick += new EventHandler( saveDataRow );
						curTimerObj.Start();
						return;

					} catch ( Exception excp ) {
						MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
					}
				} else {
					myViewRowIdx = e.RowIndex;
                }
			}
			return;
		}

		private bool validateRow(int inRowIdx) {
            return validateRow( inRowIdx, true );
        }
        private bool validateRow( int inRowIdx, bool inShowMsg ) {
            DataGridView curView = officialWorkAsgmtDataGridView;
            String curValue = "";
			if ( curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value != System.DBNull.Value
                && curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value != null ) {
                curValue = (String)curView.Rows[inRowIdx].Cells["WorkAsgmt"].Value;
                if ( curValue.Length <= 1 ) {
                    if (inShowMsg) MessageBox.Show( "A work assignment must be assigned" );
					return false;
				}
			} else {
                if (inShowMsg) MessageBox.Show( "A work assignment must be assigned" );
				return false;
			}
			return true;
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
            if ( HelperFunctions.isObjectEmpty( inView.Rows[inRowIdx].Cells["MemberIdOfficial"].Value ) ) return;

            isDataModified = true;
            isSetMemberActive = false;

            String curMemberId = (String)inView.Rows[inRowIdx].Cells["MemberIdOfficial"].Value;
            String curMemberName = (String)inView.Rows[inRowIdx].Cells["MemberNameOfficial"].Value;

            officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberId"].Value = curMemberId;
            officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["MemberName"].Value = curMemberName;
            officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["Updated"].Value = "Y";

            listTourMemberDataGridView.Visible = false;
            labelMemberSelect.Visible = false;
            officialWorkAsgmtDataGridView.Focus();
            SendKeys.Send( "{TAB}" );
            //officialWorkAsgmtDataGridView.CurrentCell = officialWorkAsgmtDataGridView.Rows[myViewRowIdx].Cells["StartTime"];
        }

        private int findMemberRow(String inValue) {
            String curMemberName = "";
			if ( listTourMemberDataGridView.Rows.Count == 0 ) return -1;
			int curColIdx = listTourMemberDataGridView.Columns["MemberNameOfficial"].Index;

			if ( inValue.Length > 0 ) {
                int curIdx = 0;
                foreach ( DataGridViewRow curRow in listTourMemberDataGridView.Rows ) {
                    curMemberName = (String)curRow.Cells[curColIdx].Value;
					if ( curMemberName == null ) return curIdx - 1; 
                    if ( curMemberName.Length > inValue.Length ) {
                        if ( curMemberName.StartsWith( inValue, true, null ) ) {
                            return curIdx;
                        } else if ( curMemberName.CompareTo(inValue) >= 0 ) {
                            return curIdx;
                        }
                    } else {
                        if ( curMemberName.CompareTo( inValue ) >= 0 ) {
							return curIdx;
						}
					}
                    curIdx++;
                }
            }

            return listTourMemberDataGridView.Rows.Count - 2;
        }

		private void setMemberSelectFilter( String inRole ) {
			myMemberSelectFilter = "";

			listTourMemberDataGridView.Columns["JudgeSlalomRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["JudgeTrickRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["JudgeJumpRatingDesc"].Visible = false;

			listTourMemberDataGridView.Columns["ScorerSlalomRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["ScorerTrickRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["ScorerJumpRatingDesc"].Visible = false;

			listTourMemberDataGridView.Columns["DriverSlalomRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["DriverTrickRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["DriverJumpRatingDesc"].Visible = false;

			listTourMemberDataGridView.Columns["SafetyOfficialRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["TechOfficialRatingDesc"].Visible = false;
			listTourMemberDataGridView.Columns["AnncrOfficialRatingDesc"].Visible = false;

            if ( inRole.Equals("Boat Judge") || inRole.Equals("Event Judge") || inRole.Equals("Event Judge ACJ") ) {
                if (myEvent.Equals("Slalom")) {
                    myMemberSelectFilter = "JudgeSlalomRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeSlalomRatingDesc"].Visible = true;

				} else if (myEvent.Equals("Trick")) {
                    myMemberSelectFilter = "JudgeTrickRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeTrickRatingDesc"].Visible = true;

				} else if (myEvent.Equals("Jump")) {
                    myMemberSelectFilter = "JudgeJumpRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeJumpRatingDesc"].Visible = true;
                }

            } else if ( inRole.Equals("End Course Official") ) {
                if (myEvent.Equals("Slalom")) {
                    myMemberSelectFilter = "JudgeSlalomRatingDesc <> '' OR DriverSlalomRatingDesc <> '' OR TechOfficialRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeSlalomRatingDesc"].Visible = true;
                    listTourMemberDataGridView.Columns["DriverSlalomRatingDesc"].Visible = true;
					listTourMemberDataGridView.Columns["TechOfficialRatingDesc"].Visible = true;

				} else if (myEvent.Equals("Trick")) {
                    myMemberSelectFilter = "JudgeTrickRatingDesc <> '' OR DriverTrickRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeTrickRatingDesc"].Visible = true;

				} else if (myEvent.Equals("Jump")) {
                    myMemberSelectFilter = "JudgeJumpRatingDesc <> '' OR DriverJumpRatingDesc <> ''";
                    listTourMemberDataGridView.Columns["JudgeJumpRatingDesc"].Visible = true;
                }

            } else if ( inRole.Equals( "Driver") ) {
				if ( myEvent.Equals( "Slalom" ) ) {
					myMemberSelectFilter = "DriverSlalomRatingDesc <> ''";
					listTourMemberDataGridView.Columns["DriverSlalomRatingDesc"].Visible = true;

				} else if ( myEvent.Equals( "Trick" ) ) {
					myMemberSelectFilter = "DriverTrickRatingDesc <> ''";
					listTourMemberDataGridView.Columns["DriverTrickRatingDesc"].Visible = true;

				} else if ( myEvent.Equals( "Jump" ) ) {
					myMemberSelectFilter = "DriverJumpRatingDesc <> ''";
					listTourMemberDataGridView.Columns["DriverJumpRatingDesc"].Visible = true;
				}

			} else if ( inRole.Equals( "Scorer" ) ) {
				if ( myEvent.Equals( "Slalom" ) ) {
					myMemberSelectFilter = "ScorerSlalomRatingDesc <> ''";
					listTourMemberDataGridView.Columns["ScorerSlalomRatingDesc"].Visible = true;

				} else if ( myEvent.Equals( "Trick" ) ) {
					myMemberSelectFilter = "ScorerTrickRatingDesc <> ''";
					listTourMemberDataGridView.Columns["ScorerTrickRatingDesc"].Visible = true;

				} else if ( myEvent.Equals( "Jump" ) ) {
					myMemberSelectFilter = "ScorerJumpRatingDesc <> ''";
					listTourMemberDataGridView.Columns["ScorerJumpRatingDesc"].Visible = true;
				}

			} else if ( inRole.Equals( "Safety" ) ) {
				myMemberSelectFilter = "SafetyOfficialRatingDesc <> ''";
				listTourMemberDataGridView.Columns["SafetyOfficialRatingDesc"].Visible = true;

			} else if ( inRole.Equals( "Announcer" ) ) {
				myMemberSelectFilter = "AnncrOfficialRatingDesc <> ''";
				listTourMemberDataGridView.Columns["AnncrOfficialRatingDesc"].Visible = true;

			} else if ( inRole.Equals( "Technical Controller" ) ) {
				myMemberSelectFilter = "TechOfficialRatingDesc <> ''";
				listTourMemberDataGridView.Columns["TechOfficialRatingDesc"].Visible = true;

			} else {
				myMemberSelectFilter = "";
            }

			loadTourMemberView();
		}

		private void navCopyItem_Click(object sender, EventArgs e) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curFromGroup = "";

            if (myEvent.Equals( "All" )) {
                MessageBox.Show( "You must select an event to be copied" );
            } else {
                if (HelperFunctions.isObjectEmpty( EventGroupList.SelectedItem )) {
                    MessageBox.Show( "You must select an event group to be copied" );
                } else {
					curFromGroup = EventGroupList.SelectedItem.ToString();
                    if (curFromGroup.ToLower().Equals( "all" )) {
                        MessageBox.Show( "You must select an event group to be copied" );
                    } else {
                        if (officialWorkAsgmtDataGridView.Rows.Count > 1) {
                            String curFromRound = roundActiveSelect.RoundValue;
                            Int16 curRounds = Convert.ToInt16( myTourRow[myEvent + "Rounds"].ToString() );
                            OfficialWorkAsgmtCopy curDialog = new OfficialWorkAsgmtCopy();
							curDialog.CopyFromGroup = curFromGroup;
							curDialog.CopyFromRound = curFromRound;

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

                                    DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                                    if (curDataTable.Rows.Count > 0) {
                                        if ((int)curDataTable.Rows[0]["OfficialCount"] > 0) {
                                            MessageBox.Show( "Officials already assigned to " + curCopyToGroup + " round " + curCopyToRound );
                                        } else {
                                            try {
                                                curSqlStmt = new StringBuilder( "" );
                                                curSqlStmt.Append( "Insert OfficialWorkAsgmt ( " );
                                                curSqlStmt.Append( "SanctionId, MemberId, Event, EventGroup, Round, WorkAsgmt, StartTime, EndTime, Notes " );
                                                curSqlStmt.Append( ") Select SanctionId, MemberId, Event, '" + curCopyToGroup + "', " + curCopyToRound + ", WorkAsgmt, getdate(), null, null  " );
                                                curSqlStmt.Append( "From OfficialWorkAsgmt " );
                                                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                                curSqlStmt.Append( "And Round = " + curFromRound + " " );
                                                curSqlStmt.Append( "And EventGroup = '" + curFromGroup + "' " );
                                                curSqlStmt.Append( "And Event = '" + myEvent + "' " );
												int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
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
            if ( !( HelperFunctions.isObjectEmpty(myFilterCmd) ) && myFilterCmd.Length > 0 ) {
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
            if ( !( HelperFunctions.isObjectEmpty(myFilterCmd) ) && myFilterCmd.Length > 0 ) {
                curSelectCommand[3] = curSelectCommand[3] + " And " + myFilterCmd;
            }

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }

        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printReport();
        }
        private void printReport() {
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

		private void navTemplateButton_Click( object sender, EventArgs e ) {
			if ( EventButtonAll.Checked ) {
				MessageBox.Show( "Event must be selected to enter officials" );
				return;
			}
			if ( officialWorkAsgmtDataGridView.Rows.Count > 1 ) {
				MessageBox.Show( "Use of template only allowed when officials not already assigned" );
				return;
			}
			if ( officialWorkAsgmtDataGridView.Rows.Count == 1 ) {
				if ( ( (String) officialWorkAsgmtDataGridView.Rows[0].Cells["WorkAsgmt"].Value ).Length == 0
					&& ( officialWorkAsgmtDataGridView.Rows[0].Cells["MemberId"].Value == System.DBNull.Value
						|| officialWorkAsgmtDataGridView.Rows[0].Cells["MemberId"].Value != null )
						|| ( (String) officialWorkAsgmtDataGridView.Rows[0].Cells["MemberId"].Value ).Length > 1
					) {
					officialWorkAsgmtDataGridView.Rows.Clear();

				} else {
					MessageBox.Show( "Use of template only allowed when officials not already assigned" );
					return;
				}
			} else if ( officialWorkAsgmtDataGridView.Rows.Count == 0 ) {
				officialWorkAsgmtDataGridView.Rows.Clear();
			}

			int curAddNumRows = 6;
			Int16 classCompare = mySkierClassList.compareClassChange( "C", myTourClass );
            if ( mySkierClassList.compareClassChange( "C", myTourClass ) > 0 ) {
				curAddNumRows = 11;
			}

			this.myViewRowIdx = 0;
			for ( int rowIdx = 0; rowIdx < curAddNumRows; rowIdx++ ) {
				DataGridViewRow curViewRow = addWorkAsgmtRow();

				if ( curAddNumRows > 6 ) {
					if ( rowIdx == 0 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Driver";
					} else if ( rowIdx == 1 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Boat Judge";
					} else if ( rowIdx == 2 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Event Judge";
					} else if ( rowIdx == 3 || rowIdx == 4 || rowIdx == 5 || rowIdx == 6 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Event Judge";
					} else if ( rowIdx == 7 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Scorer";
					} else if ( rowIdx == 8 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Safety";
					} else if ( rowIdx == 9 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Technical Controller";
					} else if ( rowIdx == 10 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Event Judge ACJ";
					}

				} else {
					if ( rowIdx == 0 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Driver";
					} else if ( rowIdx == 1 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Boat Judge";
					} else if ( rowIdx == 2 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Event Judge";
					} else if ( rowIdx == 3 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Event Judge";
					} else if ( rowIdx == 4 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Scorer";
					} else if ( rowIdx == 5 ) {
						curViewRow.Cells["WorkAsgmt"].Value = "Safety";
					}
				}
			}
		}

        private bool getTourEventData() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            DataTable curTourDataTable = getTourData();
            if ( curTourDataTable.Rows.Count <= 0 ) {
                MessageBox.Show( "An active tournament is not properly defined" );
                return false;
            }
            
            myTourRow = curTourDataTable.Rows[0];
            myTourRules = (String)myTourRow["Rules"];
            myTourClass = myTourRow["Class"].ToString().ToUpper();

            mySkierClassList = new ListSkierClass();
            mySkierClassList.ListSkierClassLoad();

            return true;
        }

        private void loadEventGroupList() {
            if ( myEvent.ToUpper().Equals( "ALL" ) ) {
                EventGroupList.Visible = false;
                roundActiveSelect.Visible = false;
                activeLabel.Visible = false;
                return;
            }

            EventGroupList.Visible = true;
            roundActiveSelect.Visible = true;
            activeLabel.Visible = true;

            myEventGroupDropdownList = new ArrayList();
            String curGroupValue = "";
            if ( EventGroupList.DataSource != null ) {
                if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
                    try {
                        curGroupValue = EventGroupList.SelectedItem.ToString();
                    } catch {
                        curGroupValue = "";
                    }
                }
            }

            if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                myEventGroupDropdownList = HelperFunctions.buildEventGroupListNcwsa();

            } else {
                myEventGroupDropdownList = HelperFunctions.buildEventGroupList( mySanctionNum, myEvent, Convert.ToByte( roundActiveSelect.RoundValue ) );
            }
            myEventGroupDropdownList.RemoveAt( 0 );
            EventGroupList.DataSource = myEventGroupDropdownList;

            if ( curGroupValue.Length > 0 ) {
                foreach ( String curValue in (ArrayList)EventGroupList.DataSource ) {
                    if ( curValue.Equals( curGroupValue ) ) {
                        EventGroupList.SelectedItem = curGroupValue;
                        EventGroupList.Text = curGroupValue;
                        return;
                    }
                }
            }

            return;
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
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTourMemberList() {
			myMemberSelectFilter = "";

			StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct TourReg.MemberId as MemberIdOfficial, TourReg.SkierName as MemberNameOfficial, MemberList.Federation" );
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
			curSqlStmt.Append( "  AND TourReg.ReadyToSki = 'Y' AND (TourReg.Withdrawn is null OR TourReg.WithDrawn = 'N') " );
			curSqlStmt.Append( "ORDER BY TourReg.SkierName, TourReg.MemberId  " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private bool validateMemberForRole( String inMemberId, String inRole, String inEvent ) {
            String curRoleName = "";
            String curEvent = myEvent;
            if ( HelperFunctions.isObjectPopulated( inEvent ) ) curEvent = inEvent;

            if ( inRole.Equals( "Boat Judge" ) || inRole.Equals( "Event Judge" ) || inRole.Equals( "Event Judge ACJ" ) ) {
                if ( curEvent.Equals( "Slalom" ) ) {
                    curRoleName = "JudgeSlalomRating";

                } else if ( curEvent.Equals( "Trick" ) ) {
                    curRoleName = "JudgeTrickRating";

                } else if ( curEvent.Equals( "Jump" ) ) {
                    curRoleName = "JudgeJumpRating";
                }

            } else if ( inRole.Equals( "End Course Official" ) ) {
                if ( curEvent.Equals( "Slalom" ) ) {
                    curRoleName = "JudgeSlalomRating !='' OR DriverSlalomRating !='' OR TechOfficialRating";

                } else if ( curEvent.Equals( "Trick" ) ) {
                    curRoleName = "JudgeTrickRating !='' OR DriverTrickRating";

                } else if ( curEvent.Equals( "Jump" ) ) {
                    curRoleName = "JudgeJumpRating !='' OR DriverJumpRating";
                }

            } else if ( inRole.Equals( "Driver" ) ) {
                if ( curEvent.Equals( "Slalom" ) ) {
                    curRoleName = "DriverSlalomRating";

                } else if ( curEvent.Equals( "Trick" ) ) {
                    curRoleName = "DriverTrickRating";

                } else if ( curEvent.Equals( "Jump" ) ) {
                    curRoleName = "DriverJumpRating";
                }

            } else if ( inRole.Equals( "Scorer" ) ) {
                if ( curEvent.Equals( "Slalom" ) ) {
                    curRoleName = "ScorerSlalomRating";

                } else if ( curEvent.Equals( "Trick" ) ) {
                    curRoleName = "ScorerTrickRating";

                } else if ( curEvent.Equals( "Jump" ) ) {
                    curRoleName = "ScorerJumpRating";
                }

            } else if ( inRole.Equals( "Safety" ) ) {
                curRoleName = "SafetyOfficialRating";

            } else if ( inRole.Equals( "Announcer" ) ) {
                curRoleName = "AnncrOfficialRating";

            } else if ( inRole.Equals( "Technical Controller" ) ) {
                curRoleName = "TechOfficialRating";

            } else {
                curRoleName = "";
            }

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId FROM OfficialWork " );
            curSqlStmt.Append( String.Format( "WHERE SanctionId = '{0}' AND MemberId = '{1}' AND ({2} != '') ", mySanctionNum, inMemberId, curRoleName ) );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return true;
            return false;
        }

        private void removeUnratedValues() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set JudgeSlalomRating = '' Where SanctionId = '" + mySanctionNum + "' AND JudgeSlalomRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch (Exception ex) {
				Log.WriteFile( String.Format("Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString()) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set JudgeTrickRating = '' Where SanctionId = '" + mySanctionNum + "' AND JudgeTrickRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set JudgeJumpRating = '' Where SanctionId = '" + mySanctionNum + "' AND JudgeJumpRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set ScorerSlalomRating = '' Where SanctionId = '" + mySanctionNum + "' AND ScorerSlalomRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set ScorerTrickRating = '' Where SanctionId = '" + mySanctionNum + "' AND ScorerTrickRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set ScorerJumpRating = '' Where SanctionId = '" + mySanctionNum + "' AND ScorerJumpRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set DriverSlalomRating = '' Where SanctionId = '" + mySanctionNum + "' AND DriverSlalomRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set DriverTrickRating = '' Where SanctionId = '" + mySanctionNum + "' AND DriverTrickRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set DriverJumpRating = '' Where SanctionId = '" + mySanctionNum + "' AND DriverJumpRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set SafetyOfficialRating = '' Where SanctionId = '" + mySanctionNum + "' AND SafetyOfficialRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set TechOfficialRating = '' Where SanctionId = '" + mySanctionNum + "' AND TechOfficialRating = 'Unrated'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}
			try {
				curSqlStmt = new StringBuilder( "Update OfficialWork Set AnncrOfficialRating = '' Where SanctionId = '" + mySanctionNum + "'" );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "Exception encountered removing unrated values {0}, {1}", ex.Message, curSqlStmt.ToString() ) );
			}

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
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

	}
}
