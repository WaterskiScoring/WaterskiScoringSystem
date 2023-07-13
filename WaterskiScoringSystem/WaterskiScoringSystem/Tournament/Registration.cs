using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

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
			if ( Properties.Settings.Default.TourRegList_Width > 0 ) this.Width = Properties.Settings.Default.TourRegList_Width;
			if ( Properties.Settings.Default.TourRegList_Height > 0 ) this.Height = Properties.Settings.Default.TourRegList_Height;
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

			String[] curList = { "SkierName", "AgeGroup", "EligParticipate", "ReadyForPlcmt", "AwsaMbrshpComment", "SlalomReg", "TrickReg", "JumpReg", "SlalomGroup", "TrickGroup", "JumpGroup", "EntryDue", "EntryPaid", "PaymentMethod", "JumpHeight", "TrickBoat", "AwsaMbrshpPaymt" };
			sortDialogForm = new SortDialogForm();
			sortDialogForm.ColumnListArray = curList;

			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null || mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration > Tournament List" );
				closeWindow();
				return;
			}
			//Retrieve selected tournament attributes
			DataTable curTourDataTable = getTourData();
			if ( curTourDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration > Tournament List" );
				closeWindow();
				return;
			}

			myTourRow = curTourDataTable.Rows[0];
			if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
			if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
			if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }

			if ( mySanctionNum.Substring( 2, 1 ).ToUpper().Equals( "U" ) ) {
				Team.Visible = true;
			} else {
				Team.Visible = false;
			}

			if ( Convert.ToInt16( myTourRow["SlalomRounds"] ) == 0 ) {
				SlalomReg.Visible = false;
				SlalomGroup.Visible = false;
				SlalomRegCount.Visible = false;
				SlalomClassReg.Visible = false;
			}
			if ( Convert.ToInt16( myTourRow["TrickRounds"] ) == 0 ) {
				TrickReg.Visible = false;
				TrickGroup.Visible = false;
				TrickRegCount.Visible = false;
				TrickClassReg.Visible = false;
			}
			if ( Convert.ToInt16( myTourRow["JumpRounds"] ) == 0 ) {
				JumpReg.Visible = false;
				JumpGroup.Visible = false;
				JumpRegCount.Visible = false;
				JumpClassReg.Visible = false;
			}
			myTourRegAddDialog = new TourRegAddMember();
			myEditRegMemberDialog = new EditRegMember();
			myTourRegRankEquivDialog = new TourRegRankEquiv();

			myTourEventReg = new TourEventReg();
			myTourRegRowIdx = 0;
			loadTourRegView();

			isDataModified = false;
		}

		private void closeWindow() {
			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 15;
			curTimerObj.Tick += new EventHandler( CloseWindowTimer );
			curTimerObj.Start();
			return;
		}
		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		private void loadTourRegView() {
			//Retrieve data for current tournament
			//Used for initial load and to refresh data after updates
			winStatusMsg.Text = "Retrieving tournament entries";
			Cursor.Current = Cursors.WaitCursor;

			tourRegDataGridView.Rows.Clear();
			myTourRegDataTable = getTourRegData();
			String curTempSortCommand = mySortCommand.Replace( "EligParticipate", "ReadyToSki" );
			String curTempFilterCommand = myFilterCmd.Replace( "EligParticipate", "ReadyToSki" );
			myTourRegDataTable.DefaultView.Sort = curTempSortCommand;
			myTourRegDataTable.DefaultView.RowFilter = curTempFilterCommand;
			DataTable curDataTable = myTourRegDataTable.DefaultView.ToTable();

			if ( curDataTable.Rows.Count == 0 ) {
				winStatusMsg.Text = "Tournament entries retrieved";
				RowStatusLabel.Text = "Row " + ( myTourRegRowIdx + 1 ).ToString() + " of " + tourRegDataGridView.Rows.Count.ToString();
				Cursor.Current = Cursors.Default;
				return;
			}

			DataGridViewRow curViewRow;
			isDataModified = false;
			int curViewIdx = 0;
			foreach ( DataRow curDataRow in curDataTable.Rows ) {
				winStatusMsg.Text = "Loading information for " + (String)curDataRow["SkierName"];

				curViewIdx = tourRegDataGridView.Rows.Add();
				curViewRow = tourRegDataGridView.Rows[curViewIdx];

				curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
				curViewRow.Cells["Updated"].Value = "Y";
				curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
				curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
				curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];

				curViewRow.Cells["State"].Value = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
				curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
				curViewRow.Cells["Team"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Team", "" );
				curViewRow.Cells["EligParticipate"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReadyToSki", "N" );
				curViewRow.Cells["ReadyForPlcmt"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReadyForPlcmt", "N" );
				curViewRow.Cells["Withdrawn"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Withdrawn", "N" );
				curViewRow.Cells["EntryDue"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "EntryDue", "", -2 );
				curViewRow.Cells["EntryPaid"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "EntryPaid", "", -2 );
				curViewRow.Cells["PaymentMethod"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PaymentMethod", "" );
				curViewRow.Cells["JumpHeight"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "JumpHeight", "", -1 );
				curViewRow.Cells["TrickBoat"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickBoat", "" );
				curViewRow.Cells["IwwfLicense"].Value = HelperFunctions.getDataRowColValue( curDataRow, "IwwfLicense", "N" );
				curViewRow.Cells["AwsaMbrshpPaymt"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "AwsaMbrshpPaymt", "", -2 );
				curViewRow.Cells["AwsaMbrshpComment"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AwsaMbrshpComment", "" );
				curViewRow.Cells["Notes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Notes", "" );

				if ( SlalomReg.Visible ) {
					String curEvent = HelperFunctions.getDataRowColValue( curDataRow, "SlalomEvent", "" );
					if ( curEvent.Equals( "Slalom" ) ) {
						curViewRow.Cells["SlalomReg"].Value = "Y";
						curViewRow.Cells["SlalomGroup"].ReadOnly = false;
						curViewRow.Cells["SlalomGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomGroup", "" );
						curViewRow.Cells["SlalomClassReg"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomClassReg", "" );
					} else {
						curViewRow.Cells["SlalomReg"].Value = "N";
						curViewRow.Cells["SlalomGroup"].Value = "";
						curViewRow.Cells["SlalomGroup"].ReadOnly = true;
						curViewRow.Cells["SlalomClassReg"].Value = "";
					}
				}
				if ( TrickReg.Visible ) {
					String curEvent = HelperFunctions.getDataRowColValue( curDataRow, "TrickEvent", "" );
					if ( curEvent.Equals( "Trick" ) ) {
						curViewRow.Cells["TrickReg"].Value = "Y";
						curViewRow.Cells["TrickGroup"].ReadOnly = false;
						curViewRow.Cells["TrickGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickGroup", "" );
						curViewRow.Cells["TrickClassReg"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickClassReg", "" );
					} else {
						curViewRow.Cells["TrickReg"].Value = "N";
						curViewRow.Cells["TrickGroup"].Value = "";
						curViewRow.Cells["TrickGroup"].ReadOnly = true;
						curViewRow.Cells["TrickClassReg"].Value = "";
					}
				}
				if ( JumpReg.Visible ) {
					String curEvent = HelperFunctions.getDataRowColValue( curDataRow, "JumpEvent", "" );
					if ( curEvent.Equals( "Jump" ) ) {
						curViewRow.Cells["JumpReg"].Value = "Y";
						curViewRow.Cells["JumpGroup"].ReadOnly = false;
						curViewRow.Cells["JumpGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpGroup", "" );
						curViewRow.Cells["JumpClassReg"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpClassReg", "" );
					} else {
						curViewRow.Cells["JumpReg"].Value = "N";
						curViewRow.Cells["JumpGroup"].Value = "";
						curViewRow.Cells["JumpGroup"].ReadOnly = true;
						curViewRow.Cells["JumpClassReg"].Value = "";
					}
				}

				String curReadyToSki = HelperFunctions.getDataRowColValue( curDataRow, "ReadyToSki", "N" );
				String curWithdrawn = HelperFunctions.getDataRowColValue( curDataRow, "Withdrawn", "N" );
				if ( curReadyToSki.Equals( "N" ) ) {
					curViewRow.Cells["SkierName"].Style.Font = new Font( "Arial Narrow", 11, System.Drawing.FontStyle.Bold );
					curViewRow.Cells["SkierName"].Style.ForeColor = Color.Red;
				} else if ( curWithdrawn.Equals( "Y" ) ) {
						curViewRow.Cells["SkierName"].Style.Font = new Font( "Arial Narrow", 11, System.Drawing.FontStyle.Bold );
						curViewRow.Cells["SkierName"].Style.ForeColor = Color.Salmon;
				}
			}
			
			if ( tourRegDataGridView.Rows.Count <= myTourRegRowIdx ) {
				myTourRegRowIdx = tourRegDataGridView.Rows.Count - 1;
			}
			tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
			setEventCounts();

			winStatusMsg.Text = "Tournament entries retrieved";
			RowStatusLabel.Text = "Row " + ( myTourRegRowIdx + 1 ).ToString() + " of " + tourRegDataGridView.Rows.Count.ToString();
			Cursor.Current = Cursors.Default;
		}

		private void dataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
			MessageBox.Show( "DataGridView_DataError occurred. \n Context: " + e.Context.ToString()
				+ "\n Exception Message: " + e.Exception.Message );
			if ( ( e.Exception ) is ConstraintException ) {
				DataGridView view = (DataGridView)sender;
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
					navSave_Click( null, null );
					e.Cancel = false;
				} catch ( Exception excp ) {
					e.Cancel = true;
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
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

				String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
				String curSanctionId = (String)curViewRow.Cells["SanctionId"].Value;
				String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
				String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
				Int64 curPK = Convert.ToInt64( (String)curViewRow.Cells["PK"].Value );
				if ( !( curUpdateStatus.ToUpper().Equals( "Y" ) ) && curSanctionId.Length == 0 && curMemberId.Length == 0 ) {
					setEventCounts();
					return;
				}

				try {
					String curJumpHeight = HelperFunctions.getViewRowColValue( curViewRow, "JumpHeight", "0" );
					String curTrickBoat = HelperFunctions.getViewRowColValue( curViewRow, "TrickBoat", "" );
					String curEligParticipate = HelperFunctions.getViewRowColValue( curViewRow, "EligParticipate", "Y" );
					String curReadyForPlcmt = HelperFunctions.getViewRowColValue( curViewRow, "ReadyForPlcmt", "Y" );
					String curEntryDue = HelperFunctions.getViewRowColValue( curViewRow, "EntryDue", "Null" );
					String curEntryPaid = HelperFunctions.getViewRowColValue( curViewRow, "EntryPaid", "Null" );
					String curAwsaMbrshpPaymt = HelperFunctions.getViewRowColValue( curViewRow, "AwsaMbrshpPaymt", "Null" );
					String curPaymentMethod = HelperFunctions.getViewRowColValue( curViewRow, "PaymentMethod", "" );
					String curAwsaMbrshpComment = HelperFunctions.getViewRowColValue( curViewRow, "AwsaMbrshpComment", "" );
					String curNotes = HelperFunctions.getViewRowColValue( curViewRow, "Notes", "" );

					StringBuilder curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update TourReg Set " );
					curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
					curSqlStmt.Append( ", ReadyToSki = '" + curEligParticipate + "'" );
					curSqlStmt.Append( ", ReadyForPlcmt = '" + curReadyForPlcmt + "'" );
					curSqlStmt.Append( ", EntryDue = " + curEntryDue );
					curSqlStmt.Append( ", EntryPaid = " + curEntryPaid );
					curSqlStmt.Append( ", PaymentMethod = '" + curPaymentMethod + "'" );
					curSqlStmt.Append( ", Weight = Null" );
					curSqlStmt.Append( ", JumpHeight = '" + curJumpHeight + "'" );
					curSqlStmt.Append( ", TrickBoat = '" + curTrickBoat + "'" );
					curSqlStmt.Append( ", AwsaMbrshpPaymt = " + curAwsaMbrshpPaymt );
					curSqlStmt.Append( ", AwsaMbrshpComment = '" + curAwsaMbrshpComment + "'" );
					curSqlStmt.Append( ", Notes = '" + curNotes + "'" );
					curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
					curSqlStmt.Append( " Where PK = " + curPK.ToString() );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

					String curEventFlag = HelperFunctions.getViewRowColValue( curViewRow, "SlalomReg", "N" );
					if ( curEventFlag.Equals( "Y" ) ) {
						String curEventGroup = HelperFunctions.getViewRowColValue( curViewRow, "SlalomGroup", "" );
						updateEventGroup( curMemberId, curAgeGroup, "Slalom", curEventGroup );
					}

					curEventFlag = HelperFunctions.getViewRowColValue( curViewRow, "TrickReg", "N" );
					if ( curEventFlag.Equals( "Y" ) ) {
						String curEventGroup = HelperFunctions.getViewRowColValue( curViewRow, "TrickGroup", "" );
						updateEventGroup( curMemberId, curAgeGroup, "Trick", curEventGroup );
					}

					curEventFlag = HelperFunctions.getViewRowColValue( curViewRow, "JumpReg", "N" );
					if ( curEventFlag.Equals( "Y" ) ) {
						String curEventGroup = HelperFunctions.getViewRowColValue( curViewRow, "JumpGroup", "" );
						updateEventGroup( curMemberId, curAgeGroup, "Jump", curEventGroup );
					}

					winStatusMsg.Text = "Changes successfully saved";
					isDataModified = false;

				} catch ( Exception excp ) {
					curMsg = "Error attempting to update skier information \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
				}
				setEventCounts();

			} catch ( Exception excp ) {
				curMsg = "Error attempting to update skier information \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}
		}

		private void navRefresh_Click( object sender, EventArgs e ) {
			// Retrieve data from database
			if ( isDataModified ) {
				try {
					navSave_Click( null, null );
					isDataModified = false;
					winStatusMsg.Text = "Previous row saved.";
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}
			if ( !( isDataModified ) ) {
				loadTourRegView();
			}
		}

		private void navExport_Click( object sender, EventArgs e ) {
			String[] curSelectCommand = new String[7];
			String[] curTableName = { "Tournament", "TourReg", "EventReg", "MemberList", "DivOrder", "OfficialWork", "OfficialWorkAsgmt" };
			String curTempFilterCommand = myFilterCmd.Replace( "EligParticipate", "ReadyToSki" );

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
						+ " And " + curTempFilterCommand;
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
						+ " And " + curTempFilterCommand;
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
						+ "  And " + curTempFilterCommand + ") ";
				} else {
					curSelectCommand[3] = curSelectCommand[3]
						+ "Where TourReg.MemberId = MemberList.MemberId And SanctionId = '" + mySanctionNum + "') ";
				}
			}

			curSelectCommand[4] = "Select * from DivOrder Where SanctionId = '" + mySanctionNum + "'";

			curSelectCommand[5] = "Select * from OfficialWork W Where SanctionId = '" + mySanctionNum + "' ";

			curSelectCommand[6] = "Select * from OfficialWorkAsgmt Where SanctionId = '" + mySanctionNum + "' ";

			myExportData.exportData( curTableName, curSelectCommand );
		}

		private void setEventCounts() {
			DataTable curRegDataTable = null;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			//SlalomRegCountLabel
			if ( SlalomRegCount.Visible ) {
				curSqlStmt.Append( "SELECT count(*) as RegCount From EventReg " );
				curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Slalom' " );
				curRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curRegDataTable.Rows.Count > 0 ) {
					SlalomRegCount.Text = ( (int)curRegDataTable.Rows[0]["RegCount"] ).ToString( "###0" );
				} else {
					SlalomRegCount.Text = "0";
				}
			}

			//TrickRegCountLabel
			if ( TrickRegCount.Visible ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT count(*) as RegCount From EventReg " );
				curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Trick' " );
				curRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curRegDataTable.Rows.Count > 0 ) {
					TrickRegCount.Text = ( (int)curRegDataTable.Rows[0]["RegCount"] ).ToString( "###0" );
				} else {
					TrickRegCount.Text = "0";
				}
			}

			if ( JumpRegCount.Visible ) {
				//JumpRegCountLabel
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT count(*) as RegCount From EventReg " );
				curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND Event = 'Jump' " );
				curRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curRegDataTable.Rows.Count > 0 ) {
					JumpRegCount.Text = ( (int)curRegDataTable.Rows[0]["RegCount"] ).ToString( "###0" );
				} else {
					JumpRegCount.Text = "0";
				}
			}
		}

		private void navSaveAs_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			//tourRegDataGridView
			myExportData.exportData( tourRegDataGridView );
		}

		private void navSort_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			sortDialogForm.SortCommand = mySortCommand;
			sortDialogForm.ShowDialog( this );

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
			filterDialogForm.ShowDialog( this );

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
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			// Open dialog for selecting skiers
			myTourRegAddDialog.resetInput();
			myTourRegAddDialog.ShowDialog( this );

			// Refresh data from database
			if ( myTourRegAddDialog.isDataModified ) {
				loadTourRegView();
			}
		}

		private void navShowMember_Click( object sender, EventArgs e ) {
			if ( isDataModified ) {
				try {
					isDataModified = false;
					winStatusMsg.Text = "Previous row saved.";
				} catch ( Exception excp ) {
					MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
				}
			}

			String memberId = (String)tourRegDataGridView.CurrentRow.Cells["MemberId"].Value;

			// Open dialog for selecting skiers
			myTourRegAddDialog.setInputMemberId( memberId );
			myTourRegAddDialog.ShowDialog( this );
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

		private void navRemove_Click( object sender, EventArgs e ) {
			if ( tourRegDataGridView.SelectedRows.Count > 0 ) {
				foreach ( DataGridViewRow curViewRow in tourRegDataGridView.SelectedRows ) {
					bool curResults = removeRow( curViewRow );
				}
				loadTourRegView();
				if ( tourRegDataGridView.Rows.Count > 0 ) {
					tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
				}
			} else {
				bool curResults = removeRow( tourRegDataGridView.Rows[myTourRegRowIdx] );
				if ( curResults ) {
					loadTourRegView();
					if ( tourRegDataGridView.Rows.Count > 0 ) {
						tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
					}
				}
			}
		}

		private void navWithdraw_Click( object sender, EventArgs e ) {
			if ( tourRegDataGridView.SelectedRows.Count > 0 ) {
				foreach ( DataGridViewRow curViewRow in tourRegDataGridView.SelectedRows ) {
					bool curResults = updateRowWithdraw( curViewRow );
				}
				loadTourRegView();
				if ( tourRegDataGridView.Rows.Count > 0 ) {
					tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
				}
			} else {
				bool curResults = updateRowWithdraw( tourRegDataGridView.Rows[myTourRegRowIdx] );
				if ( curResults ) {
					loadTourRegView();
					if ( tourRegDataGridView.Rows.Count > 0 ) {
						tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[myTourRegRowIdx].Cells["SkierName"];
					}
				}
			}
		}

		private void tourRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			DataGridView curView = (DataGridView)sender;
			int curRowPos = e.RowIndex + 1;
			RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + curView.Rows.Count.ToString();

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
				if ( curView.Rows[e.RowIndex].Cells[e.ColumnIndex] != null ) {
					if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells["MemberId"].Value ) ) ) {
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
					if ( curColName.Equals( "JumpHeight" )
						|| curColName.Equals( "EntryDue" )
						|| curColName.Equals( "EntryPaid" )
						|| curColName.Equals( "AwsaMbrshpPaymt" )
						|| curColName.Equals( "PaymentMethod" )
						|| curColName.Equals( "AwsaMbrshpComment" )
						|| curColName.Equals( "TrickBoat" )
						|| curColName.Equals( "Notes" )
						|| curColName.Equals( "Withdrawn" )
						|| curColName.Equals( "EligParticipate" )
						|| curColName.Equals( "ReadyForPlcmt" )
						|| curColName.Equals( "SlalomReg" )
						|| curColName.Equals( "SlalomGroup" )
						|| curColName.Equals( "TrickReg" )
						|| curColName.Equals( "TrickGroup" )
						|| curColName.Equals( "JumpReg" )
						|| curColName.Equals( "JumpGroup" )
						) {
						try {
							myOrigItemValue = (String)tourRegDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
						} catch {
							myOrigItemValue = "";
						}
					}
				}
			}
		}

		private void tourRegDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
			String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
			if ( curColName.Equals( "SlalomReg" )
				|| curColName.Equals( "TrickReg" )
				|| curColName.Equals( "JumpReg" )
				|| curColName.Equals( "Withdrawn" )
				) {
				SendKeys.Send( "{TAB}" );

			} else if ( curColName.Equals( "IwwfLicense" ) ) {
				DataGridViewRow curViewRow = tourRegDataGridView.Rows[e.RowIndex];
				if ( tourRegDataGridView.Rows.Count > 0 ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "N" );
					if ( curValue.Equals( "N" ) ) checkEmsLicense( curViewRow );
				}
			}
		}

		private void tourRegDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
			if ( tourRegDataGridView.Rows.Count > 0 ) {
				String curColName = tourRegDataGridView.Columns[e.ColumnIndex].Name;
				DataGridViewRow curViewRow = tourRegDataGridView.Rows[e.RowIndex];
				if ( curColName.Equals( "EntryDue" )
					|| curColName.Equals( "EntryPaid" )
					|| curColName.Equals( "AwsaMbrshpPaymt" )
					|| curColName.Equals( "JumpHeight" )
				) {
					if ( isObjectEmpty( e.FormattedValue ) ) {
						e.Cancel = false;
					} else {
						try {
							Decimal curNum = Convert.ToDecimal( e.FormattedValue.ToString() );
							e.Cancel = false;
						} catch {
							e.Cancel = true;
							MessageBox.Show( curColName + " must be numeric" );
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
				if ( curColName.Equals( "SlalomReg" ) ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "N" );
					if ( curValue == myOrigItemValue ) return;
					if ( curValue.Equals( "Y" ) ) {
						execTimerEvent( new EventHandler( addSkierToSlalomTimer ) );
					} else {
						execTimerEvent( new EventHandler( deleteSkierFromSlalomTimer ) );
					}

				} else if ( curColName.Equals( "TrickReg" ) ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "N" );
					if ( curValue == myOrigItemValue ) return;
					if ( curValue.Equals( "Y" ) ) {
						execTimerEvent( new EventHandler( addSkierToTrickTimer ) );
					} else {
						execTimerEvent( new EventHandler( deleteSkierFromTrickTimer ) );
					}
				
				} else if ( curColName.Equals( "JumpReg" ) ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "N" );
					if ( curValue == myOrigItemValue ) return;
					if ( curValue.Equals( "Y" ) ) {
						execTimerEvent( new EventHandler( addSkierToJumpTimer ) );
					} else {
						execTimerEvent( new EventHandler( deleteSkierFromJumpTimer ) );
					}

				} else if ( curColName.Equals( "ReadyForPlcmt" ) ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "" );
					if ( curValue == myOrigItemValue ) return;

					isDataModified = true;
					curViewRow.Cells["Updated"].Value = "Y";
					String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
					String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
					updateEventRegReadyForPlcmt( curMemberId, curAgeGroup, curValue );

				} else if ( curColName.Equals( "Withdrawn" ) ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "N" );
					if ( curValue == myOrigItemValue ) return;

					isDataModified = true;
					curViewRow.Cells["Updated"].Value = "Y";
					updateRowWithdraw( curViewRow );

				} else if ( curColName.Equals( "SlalomGroup" )
					|| curColName.Equals( "TrickGroup" )
					|| curColName.Equals( "JumpGroup" )
					   ) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "" );
					if ( curValue == myOrigItemValue ) return;

					isDataModified = true;
					curViewRow.Cells["Updated"].Value = "Y";

				} else if ( curColName.Equals( "EntryDue" )
					|| curColName.Equals( "EntryPaid" )
					|| curColName.Equals( "AwsaMbrshpPaymt" )
					|| curColName.Equals( "JumpHeight" )
					) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "" );
					if ( curValue == myOrigItemValue ) return;
					isDataModified = true;
					curViewRow.Cells["Updated"].Value = "Y";

				} else if ( curColName.Equals( "PaymentMethod" )
						|| curColName.Equals( "AwsaMbrshpComment" )
						|| curColName.Equals( "TrickBoat" )
						|| curColName.Equals( "Notes" )
						|| curColName.Equals( "EligParticipate" )
					) {
					String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "" );
					if ( curValue == myOrigItemValue ) return;
					isDataModified = true;
					curViewRow.Cells["Updated"].Value = "Y";
				}
			}
		}

		private void execTimerEvent( EventHandler eventHandler ) {
			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 5;
			curTimerObj.Tick += eventHandler;
			curTimerObj.Start();
		}

		private void addSkierToSlalomTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer) sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler(addSkierToSlalomTimer);
            DataGridViewRow curViewRow = tourRegDataGridView.Rows[myTourRegRowIdx];
            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
			String curWithdrawn = HelperFunctions.getViewRowColValue( curViewRow, "Withdrawn", "N" );
			if ( curWithdrawn.Equals("Y" ) ) {
				curViewRow.Cells["SlalomReg"].Value = "N";
				MessageBox.Show( "Unable to register skier for event that is marked as withdrawn" );
				return;
			}

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
			String curWithdrawn = HelperFunctions.getViewRowColValue( curViewRow, "Withdrawn", "N" );
			if ( curWithdrawn.Equals( "Y" ) ) {
				curViewRow.Cells["TrickReg"].Value = "N";
				MessageBox.Show( "Unable to register skier for event that is marked as withdrawn" );
				return;
			}
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
			String curWithdrawn = HelperFunctions.getViewRowColValue( curViewRow, "Withdrawn", "N" );
			if ( curWithdrawn.Equals( "Y" ) ) {
				curViewRow.Cells["JumpReg"].Value = "N";
				MessageBox.Show( "Unable to register skier for event that is marked as withdrawn" );
				return;
			}
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

		private void checkEmsLicense( DataGridViewRow curViewRow ) {
			String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
			String curWithdrawn = HelperFunctions.getViewRowColValue( curViewRow, "Withdrawn", "N" );
			if ( curWithdrawn.Equals( "Y" ) ) {
				curViewRow.Cells["IwwfLicense"].Value = "N";
				MessageBox.Show( "Skier is marked as withdrawn no action required" );
				return;
			}

			if ( myTourEventReg.checkEmsLicense( curMemberId, curAgeGroup, "Y" ) ) {
				curViewRow.Cells["IwwfLicense"].Value = "Y";
				StringBuilder curSqlStmt = new StringBuilder(
					String.Format( "Update TourReg Set IwwfLicense = 'Y' "
					+ " Where MemberId = '{0}' And SanctionId = '{1}' And AgeGroup = '{2}'"
					, curMemberId, mySanctionNum, curAgeGroup ) );
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}
		}

		private bool removeRow( DataGridViewRow curViewRow ) {
			String curMethodName = "Tournament:Registration:removeRow";
			String curMsg = "";
			String curSlalomRegValue = "N", curTrickRegValue = "N", curJumpRegValue = "N";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			if ( curViewRow == null ) return false;

			String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
			String curSkierName = (String)curViewRow.Cells["SkierName"].Value;

			DialogResult msgResp =
				MessageBox.Show( String.Format( "You have requested to remove skier {0} ({1} {2}) from the tournament"
				+ "\n\nConsider marking as withdrawn instead of removing to prevent skier from being re-entered during subsequent registration imports?"
				+ "\n\nPress OK to continue or CANCEL to skip removing skier and mark as withdrawn"
				, curSkierName, curMemberId, curAgeGroup )
				, "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
			if ( msgResp == DialogResult.Cancel ) return updateRowWithdraw( curViewRow );

			curSlalomRegValue = HelperFunctions.getViewRowColValue( curViewRow, "SlalomReg", "N" );
			if ( curSlalomRegValue.Equals( "Y" ) ) {
				if ( deleteSlalomEntry( curViewRow, curMemberId, curAgeGroup ) ) curSlalomRegValue = "N";
			}

			curTrickRegValue = HelperFunctions.getViewRowColValue( curViewRow, "TrickReg", "N" );
			if ( curTrickRegValue.Equals( "Y" ) ) {
				if ( deleteTrickEntry( curViewRow, curMemberId, curAgeGroup ) ) curTrickRegValue = "N";
			}

			curJumpRegValue = HelperFunctions.getViewRowColValue( curViewRow, "JumpReg", "N" );
			if ( curJumpRegValue.Equals( "Y" ) ) {
				if ( deleteJumpEntry( curViewRow, curMemberId, curAgeGroup ) ) curJumpRegValue = "N";
			}

			if ( curSlalomRegValue.Equals( "Y" ) || curTrickRegValue.Equals( "Y" ) || curJumpRegValue.Equals( "Y" ) ) {
				curMsg = String.Format( "Skier {0} ({1} {2}) registered for 1 or more events"
									, curSkierName, curMemberId, curAgeGroup );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
			}

			try {
				winStatusMsg.Text = String.Format("Skier {0} ({1} {2}) registration and event entries removed"
					, curSkierName, curMemberId, curAgeGroup );

				try {
					curSqlStmt = new StringBuilder( "Delete TourReg "
						+ " Where MemberId = '" + curMemberId + "'"
						+ " And SanctionId = '" + mySanctionNum + "'"
						+ " And AgeGroup = '" + curAgeGroup + "'" );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

					if ( !( isMemberInTour( curMemberId ) ) ) {
						curSqlStmt = new StringBuilder( "Delete OfficialWork "
							+ " Where MemberId = '" + curMemberId + "'"
							+ " And SanctionId = '" + mySanctionNum + "'" );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

						curSqlStmt = new StringBuilder( "Delete OfficialWorkAsgmt "
							+ " Where MemberId = '" + curMemberId + "'"
							+ " And SanctionId = '" + mySanctionNum + "'" );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					}
					return true;

				} catch ( Exception excp ) {
					curMsg = "Error attempting to remove skier from tournament \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
					return false;
				}

			} catch ( Exception excp ) {
				curMsg = "Error attempting to remove current entry \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
			}
		}

		private bool updateRowWithdraw( DataGridViewRow curViewRow ) {
			String curMethodName = "Tournament:Registration:updateRowWithdraw";
			String curMsg = "";
			String curSlalomRegValue = "N", curTrickRegValue = "N", curJumpRegValue = "N";
			StringBuilder curSqlStmt = new StringBuilder( "" );

			if ( curViewRow == null ) return false;

			String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
			String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
			String curSkierName = (String)curViewRow.Cells["SkierName"].Value;
			String curWithdrawn = HelperFunctions.getViewRowColValue( curViewRow, "Withdrawn", "N" );

			if ( curWithdrawn.Equals("N") ) {
				try {
					winStatusMsg.Text = String.Format( "Skier {0} ({1} {2}) removed from events and marked as withdrawn"
						, curSkierName, curMemberId, curAgeGroup );
					curSqlStmt = new StringBuilder(
						String.Format( "Update TourReg Set Withdrawn = 'N' "
						+ " Where MemberId = '{0}' And SanctionId = '{1}' And AgeGroup = '{2}'"
						, curMemberId, mySanctionNum, curAgeGroup ) );
					DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					return true;

				} catch ( Exception excp ) {
					curMsg = "Error attempting to remove current entry \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
					return false;
				}
			}

			curSlalomRegValue = HelperFunctions.getViewRowColValue( curViewRow, "SlalomReg", "N" );
			if ( curSlalomRegValue.Equals( "Y" ) ) {
				if ( deleteSlalomEntry( curViewRow, curMemberId, curAgeGroup ) ) curSlalomRegValue = "N";
			}

			curTrickRegValue = HelperFunctions.getViewRowColValue( curViewRow, "TrickReg", "N" );
			if ( curTrickRegValue.Equals( "Y" ) ) {
				if ( deleteTrickEntry( curViewRow, curMemberId, curAgeGroup ) ) curTrickRegValue = "N";
			}

			curJumpRegValue = HelperFunctions.getViewRowColValue( curViewRow, "JumpReg", "N" );
			if ( curJumpRegValue.Equals( "Y" ) ) {
				if ( deleteJumpEntry( curViewRow, curMemberId, curAgeGroup ) ) curJumpRegValue = "N";
			}

			if ( curSlalomRegValue.Equals( "Y" ) || curTrickRegValue.Equals( "Y" ) || curJumpRegValue.Equals( "Y" ) ) {
				curMsg = String.Format( "Skier {0} ({1} {2}) registered for 1 or more events"
									, curSkierName, curMemberId, curAgeGroup );
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
			}

			try {
				winStatusMsg.Text = String.Format( "Skier {0} ({1} {2}) removed from events and marked as withdrawn"
					, curSkierName, curMemberId, curAgeGroup );
				try {
					curSqlStmt = new StringBuilder( "Update TourReg "
						+ "Set Withdrawn = 'Y' " 
						+ "Where MemberId = '" + curMemberId + "' "
						+ "And SanctionId = '" + mySanctionNum + "' "
						+ "And AgeGroup = '" + curAgeGroup + "'" );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					if ( rowsProc > 0 ) {
						curViewRow.Cells["Withdrawn"].Value = "Y";
						curSqlStmt = new StringBuilder( "Delete OfficialWork "
							+ "Where MemberId = '" + curMemberId + "' "
							+ "And SanctionId = '" + mySanctionNum + "' " );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

						curSqlStmt = new StringBuilder( "Delete OfficialWorkAsgmt "
							+ "Where MemberId = '" + curMemberId + "' "
							+ "And SanctionId = '" + mySanctionNum + "'" );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					}
					return true;

				} catch ( Exception excp ) {
					curMsg = "Error attempting to remove skier from tournament \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
					return false;
				}

			} catch ( Exception excp ) {
				curMsg = "Error attempting to remove current entry \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
				return false;
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
            try {
                bool returnStatus = myTourEventReg.deleteSlalomEntry(inMemberId, inAgeGroup);
                if ( returnStatus ) {
                    curViewRow.Cells["SlalomReg"].Value = "N";
                    curViewRow.Cells["SlalomGroup"].Value = "";
                    curViewRow.Cells["SlalomGroup"].ReadOnly = true;
                    return true;

                } else {
                    curViewRow.Cells["SlalomReg"].Value = "Y";
					return false;
				}
			
			} catch {
                return false;
            }
        }

        private bool deleteTrickEntry( DataGridViewRow curViewRow, String inMemberId, String inAgeGroup ) {
            try {
                bool returnStatus = myTourEventReg.deleteTrickEntry(inMemberId, inAgeGroup);
                if ( returnStatus ) {
                    curViewRow.Cells["TrickReg"].Value = "N";
                    curViewRow.Cells["TrickGroup"].Value = "";
                    curViewRow.Cells["TrickGroup"].ReadOnly = true;
					return true;
				
				} else {
                    curViewRow.Cells["TrickReg"].Value = "Y";
					return false;
				}
			
			} catch {
				return false;
			}
        }

        private bool deleteJumpEntry( DataGridViewRow curViewRow, String inMemberId, String inAgeGroup ) {
            try {
                bool returnStatus = myTourEventReg.deleteJumpEntry( inMemberId, inAgeGroup );
                if ( returnStatus ) {
                    curViewRow.Cells["JumpReg"].Value = "N";
                    curViewRow.Cells["JumpGroup"].Value = "";
                    curViewRow.Cells["JumpGroup"].ReadOnly = true;
					return true;
				
				} else {
                    curViewRow.Cells["JumpReg"].Value = "Y";
					return false;
				}
			
			} catch {
				return false;
			}
        }

        private bool updateEventRegReadyForPlcmt( String curMemberId, String curAgeGroup, String curReadyForPlcmt ) {
            try {
                StringBuilder curSqlStmt = new StringBuilder("");
                curSqlStmt.Append("Update EventReg Set ");
                curSqlStmt.Append("  ReadyForPlcmt = '" + curReadyForPlcmt + "' ");
                curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "' ");
                curSqlStmt.Append("  And MemberId = '" + curMemberId + "' ");
                curSqlStmt.Append("  And AgeGroup = '" + curAgeGroup + "' ");
                int rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
				return true;

            } catch {
				return false;
			}
        }

		private void SearchButton_Click( object sender, EventArgs e ) {
			int curFindRow = tourRegDataGridView.Rows.Count - 2;
			String curMemberName = "";
			if ( tourRegDataGridView.Rows.Count == 0 ) curFindRow = -1;
			int curColIdx = tourRegDataGridView.Columns["SkierName"].Index;

			if ( SearchTextbox.Text.Length > 0 ) {
				int curIdx = 0;
				foreach ( DataGridViewRow curRow in tourRegDataGridView.Rows ) {
					curMemberName = (String) curRow.Cells[curColIdx].Value;
					if ( curMemberName == null ) curFindRow = curIdx - 1;
					if ( curMemberName.Length > SearchTextbox.Text.Length ) {
						if ( curMemberName.StartsWith( SearchTextbox.Text, true, null ) ) {
							curFindRow = curIdx;
							break;

						} else if ( curMemberName.CompareTo( SearchTextbox.Text ) >= 0 ) {
							curFindRow = curIdx;
							break;
						}

					} else {
						if ( curMemberName.CompareTo( SearchTextbox.Text ) >= 0 ) {
							curFindRow = curIdx;
							break;
						}
					}
					curIdx++;
				}
			}
			tourRegDataGridView.Focus();
			tourRegDataGridView.CurrentCell = tourRegDataGridView.Rows[curFindRow].Cells["SkierName"];
		}

		private void SearchTextbox_KeyUp( object sender, KeyEventArgs e ) {
			if ( e.KeyCode == Keys.Enter ) {
				SearchButton.Focus();
				SearchButton_Click( null, null );
			}
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
                        myEditRegMemberDialog.editMember( curMemberId, curAgeGroup, null );
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
                    myEditRegMemberDialog.editMember( curMemberId, curAgeGroup, null );
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
            curSqlStmt.Append( "SELECT R.PK, R.MemberId, R.SanctionId, R.SkierName, R.AgeGroup, R.State, R.Team" );
            curSqlStmt.Append( ", R.EntryDue, R.EntryPaid, R.PaymentMethod, R.ReadyToSki, R.Withdrawn, R.ReadyForPlcmt, R.IwwfLicense, R.AwsaMbrshpPaymt" );
            curSqlStmt.Append( ", R.TrickBoat, COALESCE(R.JumpHeight, 'C') as JumpHeight, R.AwsaMbrshpComment, R.Notes" );
			curSqlStmt.Append( ", SlalomClassReg, TrickClassReg, JumpClassReg" );
			curSqlStmt.Append( ", S.Event AS SlalomEvent, S.EventGroup AS SlalomGroup" );
            curSqlStmt.Append( ", T.Event AS TrickEvent, T.EventGroup AS TrickGroup" );
            curSqlStmt.Append( ", J.Event AS JumpEvent, J.EventGroup AS JumpGroup " );
            curSqlStmt.Append( "FROM TourReg AS R" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup AND S.Event = 'Slalom'" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS T ON T.SanctionId = R.SanctionId AND T.MemberId = R.MemberId AND T.AgeGroup = R.AgeGroup AND T.Event = 'Trick'" );
            curSqlStmt.Append( "    LEFT OUTER JOIN EventReg AS J ON J.SanctionId = R.SanctionId AND J.MemberId = R.MemberId AND J.AgeGroup = R.AgeGroup AND J.Event = 'Jump' " );
            curSqlStmt.Append( "WHERE R.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "ORDER BY R.SkierName, R.AgeGroup " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
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

		private void updateEventGroup(String curMemberId, String curAgeGroup, String curEvent, String newEventGroup ) {
			String curMethodName = "updateEventGroup: ";
			int rowsProc = 0;
			StringBuilder curSqlStmt = new StringBuilder("");
			if (isObjectEmpty(newEventGroup)) newEventGroup = "";

			try {
				curSqlStmt.Append("SELECT E.Event, E.SanctionId, E.MemberId, O.MemberId as RunOrderMember, T.SkierName, E.AgeGroup, E.EventGroup, O.EventGroup as EventGroupRO, O.RunOrderGroup, O.Round ");
				curSqlStmt.Append("FROM EventReg E ");
				curSqlStmt.Append("INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup ");
				curSqlStmt.Append("INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND O.Event = E.Event AND E.EventGroup = O.EventGroup ");
				curSqlStmt.Append("Where E.SanctionId = '" + this.mySanctionNum + "'");
				curSqlStmt.Append("  AND E.MemberId = '" + curMemberId + "'");
				curSqlStmt.Append("  AND E.AgeGroup = '" + curAgeGroup + "'");
				curSqlStmt.Append("  AND E.Event = '" + curEvent + "'");
				curSqlStmt.Append("Order by E.Event, O.MemberId, E.AgeGroup, E.EventGroup");
				DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());

				if (curDataTable.Rows.Count > 0) {
					foreach (DataRow curRow in curDataTable.Rows) {
						String curEventGroup = (String)curRow["EventGroup"];
						String curEventGroupRO = (String)curRow["EventGroupRO"];
						int curRound = (byte)curRow["Round"];
						String curRunOrderGroup = "";
						if (curRow["RunOrderGroup"] != System.DBNull.Value ) curRunOrderGroup = (String)curRow["RunOrderGroup"];

						if (newEventGroup.Length > 0) {
							if ( !(newEventGroup.Equals(curEventGroup)) ) {
								curSqlStmt = new StringBuilder("");
								curSqlStmt.Append("Update EventRunOrder Set ");
								curSqlStmt.Append(" EventGroup = '" + newEventGroup + "'");
								if (curRunOrderGroup.Equals(curEventGroup)) {
									curSqlStmt.Append(", RunOrderGroup = '" + newEventGroup + "'");
								}
								curSqlStmt.Append(", LastUpdateDate = GETDATE() ");
								curSqlStmt.Append("Where SanctionId = '" + this.mySanctionNum + "'");
								curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
								curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
								curSqlStmt.Append("  AND Event = '" + curEvent + "'");
								curSqlStmt.Append("  AND Round = " + curRound + " ");
								rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
								Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
							}

						} else {
							curSqlStmt = new StringBuilder("");
							curSqlStmt.Append("Delete From EventRunOrder ");
							curSqlStmt.Append("Where SanctionId = '" + this.mySanctionNum + "'");
							curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
							curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
							curSqlStmt.Append("  AND Event = '" + curEvent + "'");
							curSqlStmt.Append("  AND Round = " + curRound + " ");
							rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
							Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
						}
					}
				}

				if (newEventGroup.Length > 0) {
					curSqlStmt = new StringBuilder("");
					curSqlStmt.Append("Update EventReg Set ");
					curSqlStmt.Append(" EventGroup = '" + newEventGroup + "'");
					curSqlStmt.Append(", LastUpdateDate = GETDATE() ");
					curSqlStmt.Append("Where SanctionId = '" + this.mySanctionNum + "'");
					curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
					curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
					curSqlStmt.Append("  AND Event = '" + curEvent + "'");
					rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
					Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());

				} else {
					curSqlStmt = new StringBuilder("");
					curSqlStmt.Append("Delete From EventReg ");
					curSqlStmt.Append("Where SanctionId = '" + this.mySanctionNum + "'");
					curSqlStmt.Append("  AND MemberId = '" + curMemberId + "'");
					curSqlStmt.Append("  AND AgeGroup = '" + curAgeGroup + "'");
					curSqlStmt.Append("  AND Event = '" + curEvent + "'");
					rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
					Log.WriteFile(curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString());
				}

			} catch (Exception excp) {
				String curMsg = "Error attempting to update skier event group \n" + excp.Message;
				MessageBox.Show(curMsg);
				Log.WriteFile(curMethodName + curMsg);
			}

		}

		private bool isMemberInTour(String inMemberId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT R.PK, R.MemberId, R.SanctionId, R.SkierName, Withdrawn " );
            curSqlStmt.Append( "FROM TourReg AS R " );
            curSqlStmt.Append( "WHERE R.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  And R.MemberId = '" + inMemberId + "'" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return true;
            } else {
                return false;
            }
        }
	}
}
