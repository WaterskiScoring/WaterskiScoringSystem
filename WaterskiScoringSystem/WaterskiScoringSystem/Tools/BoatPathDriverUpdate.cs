using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
	public partial class BoatPathDriverUpdate : Form {
		private String mySanctionNum = null;
		private String myEvent = "Slalom";
		private String myTourRules = "";
		private String myOrigItemValue = "";
		private String myFilterCmd = "";

		private Boolean isDataModified = false;
		private Boolean isDataLoading = false;

		private int myRowIdx = 0;
		private DataRow myTourRow;

		private DataTable myDataTable;
		private FilterDialogForm filterDialogForm;

		public BoatPathDriverUpdate() {
			InitializeComponent();
		}

		private void BoatPathDriverUpdate_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.BoatPathDriverUpdate_Width > 0 ) {
				this.Width = Properties.Settings.Default.BoatPathDriverUpdate_Width;
			}
			if ( Properties.Settings.Default.BoatPathDriverUpdate_Height > 0 ) {
				this.Height = Properties.Settings.Default.BoatPathDriverUpdate_Height;
			}
			if ( Properties.Settings.Default.BoatPathDriverUpdate_Location.X > 0
				&& Properties.Settings.Default.BoatPathDriverUpdate_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.BoatPathDriverUpdate_Location;
			}

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return;
			}
			if ( mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return;
			}
			
			DataTable curTourDataTable = getTourData();
			if ( curTourDataTable == null || curTourDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return;
			}
			
			myTourRow = curTourDataTable.Rows[0];
			myTourRules = (String)myTourRow["Rules"];

			SlalomSelectButton.Checked = true;

			String[] curList = { "DriverName", "Round", "InsertDate" };
			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;

		}
		private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
			MessageBox.Show( "Error happened " + e.Context.ToString() );
			if ( e.Context == DataGridViewDataErrorContexts.Commit ) {
				MessageBox.Show( "Commit error" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.CurrentCellChange ) {
				MessageBox.Show( "Cell change" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.Parsing ) {
				MessageBox.Show( "parsing error" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.LeaveControl ) {
				MessageBox.Show( "leave control error" );
			}
			if ( ( e.Exception ) is ConstraintException ) {
				DataGridView view = (DataGridView)sender;
				view.Rows[e.RowIndex].ErrorText = "an error";
				view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

				e.ThrowException = false;
			}
		}

		private void BoatPathDriverUpdate_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.BoatPathDriverUpdate_Width = this.Size.Width;
				Properties.Settings.Default.BoatPathDriverUpdate_Height = this.Size.Height;
				Properties.Settings.Default.BoatPathDriverUpdate_Location = this.Location;
			}
		}

		private void navRefresh_Click( object sender, EventArgs e ) {
			this.Cursor = Cursors.WaitCursor;
			myDataTable = getBoatPath();
			loadDataGrid();
			this.Cursor = Cursors.Default;
		}

		private void navExport_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			String filename = "BoatPathSlalomDriverUpdateExport.txt";
			if ( myEvent.Equals( "Jump" ) ) filename = "BoatPathJumpDriverUpdateExport.txt";
			myExportData.exportData( dataGridView, filename );
		}

		private void navFilter_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			filterDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( filterDialogForm.DialogResult == DialogResult.OK ) {
				myFilterCmd = filterDialogForm.FilterCommand;
				winStatusMsg.Text = "Filtered by " + myFilterCmd;
				navRefresh_Click( null, null );
			}
		}

		private void loadDataGrid() {
			DataGridViewRow curViewRow;
			this.Cursor = Cursors.WaitCursor;
			isDataLoading = true;

			dataGridView.Rows.Clear();
			if ( myDataTable == null || myDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "All boat path drivers matched available driver assignments" );
				return;
			}
			int curIdx = 0;
			foreach ( DataRow curRow in myDataTable.Rows ) {
				curIdx = dataGridView.Rows.Add();
				curViewRow = dataGridView.Rows[curIdx];
				curViewRow.Cells["BPPK"].Value = (Int64)curRow["BPPK"];
				curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
				curViewRow.Cells["SkierMemberId"].Value = (String)curRow["SkierMemberId"];
				curViewRow.Cells["SkierName"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["EventGroup"].Value = HelperFunctions.getDataRowColValue( curRow, "EventGroup", "" );
				curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curRow, "EventClass", "" );

				curViewRow.Cells["Event"].Value = (String)curRow["Event"];
				curViewRow.Cells["Round"].Value = ( (Byte)curRow["Round"] ).ToString();
				curViewRow.Cells["PassNumber"].Value = ( (Byte)curRow["PassNumber"] ).ToString();
				curViewRow.Cells["Boat"].Value = HelperFunctions.getDataRowColValue( curRow, "BoatDescription", "" );

				curViewRow.Cells["BpmsDriverId"].Value = HelperFunctions.getDataRowColValue( curRow, "BpmsDriverId", "" );
				curViewRow.Cells["BpmsDriverName"].Value = HelperFunctions.getDataRowColValue( curRow, "BpmsDriver", "" );
				curViewRow.Cells["AssignedDriverId"].Value = HelperFunctions.getDataRowColValue( curRow, "AssignedDriverId", "" );
				curViewRow.Cells["AssignedDriver"].Value = HelperFunctions.getDataRowColValue( curRow, "AssignedDriver", "" );

				curViewRow.Cells["RerideNote"].Value = HelperFunctions.getDataRowColValue( curRow, "RerideNote", "" );
				curViewRow.Cells["PassNote"].Value = HelperFunctions.getDataRowColValue( curRow, "PassNote", "" );

				curViewRow.Cells["InsertDate"].Value = ( (DateTime)curRow["PassDatatime"] ).ToString( "yyyy/MM/dd HH:mm:ss" );
				curViewRow.Cells["LastUpdateDate"].Value = ( (DateTime)curRow["BoatPathDatatime"] ).ToString( "yyyy/MM/dd HH:mm:ss" );

				curViewRow.Cells["SkierRunNum"].Value = "";
				if ( curRow["SkierRunNum"] != System.DBNull.Value ) curViewRow.Cells["SkierRunNum"].Value = "Match";

				curViewRow.Cells["BoatSpeed"].Value = HelperFunctions.getDataRowColValue( curRow, "BoatSpeed", "" );
				curViewRow.Cells["LineLen"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "LineLen", "", 2 );

				if ( myEvent.Equals( "Slalom" ) ) {
					curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValue( curRow, "Score", "" );
					curViewRow.Cells["PassScore"].Value = HelperFunctions.getDataRowColValue( curRow, "PassScore", "" );
				} else {
					curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "" )
						+ " / " + HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "" );
					curViewRow.Cells["PassScore"].Value = HelperFunctions.getDataRowColValue( curRow, "PassScoreFeet", "" )
						+ " / " + HelperFunctions.getDataRowColValue( curRow, "PassScoreMeters", "" );
				}
			}

			isDataLoading = false;
			isDataModified = false;

			myRowIdx = dataGridView.Rows.Count - 1;
			if ( dataGridView.Rows.Count > 0 ) {
				dataGridView.FirstDisplayedScrollingRowIndex = myRowIdx;
				dataGridView.Rows[myRowIdx].Selected = true;
				dataGridView.Rows[myRowIdx].Cells[0].Selected = true;
				
				dataGridView.CurrentCell = dataGridView.Rows[myRowIdx].Cells["SkierName"];
				myOrigItemValue = (String)dataGridView.Rows[myRowIdx].Cells["SkierName"].Value;

				int curRowPos = myRowIdx + 1;
				RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + dataGridView.Rows.Count.ToString();

			} else {
				RowStatusLabel.Text = "";
			}
			Cursor.Current = Cursors.Default;


			if ( myOrigItemValue == null ) myOrigItemValue = "";
		}

		private void navSave_Click( object sender, EventArgs e ) {
			if ( sender != null ) dataGridView.EndEdit();

			bool curUpdatesAvailable = false;
			int curRowIdx = 0;
			int curRowsUpdated = 0;
			foreach ( DataGridViewRow curViewRow in dataGridView.Rows ) {
				String curRowUpdate = HelperFunctions.getViewRowColValue( curViewRow, "UpdateDriverCheckBox", "FALSE" );
				if ( HelperFunctions.isValueTrue( curRowUpdate ) ) {
					curUpdatesAvailable = true;
					if ( updateDriver( curViewRow ) ) curRowsUpdated++;
				}

				curRowIdx++;
			}

			if ( curUpdatesAvailable && curRowsUpdated > 0 ) MessageBox.Show( "Rows Updated: " + curRowsUpdated );

		}

		private bool updateDriver( DataGridViewRow curViewRow ) {
			Int64 curPK = (Int64)curViewRow.Cells["BPPK"].Value;
			String curRound = (String)curViewRow.Cells["Round"].Value;
			String curPassNumber = (String)curViewRow.Cells["PassNumber"].Value;
			String curPassLineLength = (String)curViewRow.Cells["LineLen"].Value;
			String curPassSpeedKph = (String)curViewRow.Cells["BoatSpeed"].Value;

			String curDriverMemberId = (String)curViewRow.Cells["AssignedDriverId"].Value;
			String curDriverName = (String)curViewRow.Cells["AssignedDriver"].Value;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Update BoatPath Set " );
			curSqlStmt.Append( "DriverMemberId = '" + curDriverMemberId + "'" );
			curSqlStmt.Append( ", DriverName = '" + curDriverName + "'" );
			curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
			curSqlStmt.Append( "Where PK = " + curPK );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			Log.WriteFile( "BoatPathDriverUpdate:Save:Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
			if ( rowsProc > 0 ) return true;
			return false;
		}
	
		private DataTable getBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT BP.PK as BPPK, S.SanctionId, T.SkierName, T.MemberId as SkierMemberId, E.Event, S.AgeGroup as Div, E.EventGroup, E.EventClass" );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", S.Round as Round, R.SkierRunNum as SkierRunNum, BP.PassNumber, BP.PassLineLength as LineLen, BP.PassSpeedKph as BoatSpeed" );
			} else {
				curSqlStmt.Append( ", S.Round as Round, R.PassNum as SkierRunNum, BP.PassNumber, BP.PassSpeedKph as BoatSpeed, '' as LineLen" );
			}
			curSqlStmt.Append( ", BP.DriverName as BpmsDriver, BP.DriverMemberId as BpmsDriverId" );
			curSqlStmt.Append( ", OT.SkierName AS AssignedDriver, OT.MemberId AS AssignedDriverId, O.WorkAsgmt" );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", S.Score, R.Score AS PassScore" );
			} else {
				curSqlStmt.Append( ", S.ScoreFeet, S.ScoreMeters, R.ScoreFeet as PassScoreFeet, R.ScoreMeters as PassScoreMeters" );
			}
			curSqlStmt.Append( ", R.TimeInTol as InTol" );
			curSqlStmt.Append( ", R.Note AS PassNote, S.Note as ScoreNote, BP.RerideNote, R.InsertDate as PassDatatime, BP.InsertDate as BoatPathDatatime " );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( "FROM SlalomScore S " );
				curSqlStmt.Append( "INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			} else {
				curSqlStmt.Append( "FROM JumpScore S " );
				curSqlStmt.Append( "INNER JOIN JumpRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			}
			curSqlStmt.Append( "INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round AND BP.Event = E.Event " );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( "AND BP.PassNumber = R.SkierRunNum " );
			} else {
				curSqlStmt.Append( "AND BP.PassNumber = R.PassNum " );
			}
			curSqlStmt.Append( "INNER JOIN OfficialWorkAsgmt O ON O.SanctionId = S.SanctionId AND O.Event = E.Event AND O.EventGroup = E.EventGroup AND O.Round = S.Round AND O.WorkAsgmt = 'Driver' " );
			curSqlStmt.Append( "INNER JOIN TourReg OT ON O.MemberId = OT.MemberId AND O.SanctionId = OT.SanctionId " );
			curSqlStmt.Append( "Where S.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + myEvent + "' " );
			curSqlStmt.Append( "AND BP.DriverMemberId NOT IN (Select OO.MemberId From OfficialWorkAsgmt OO Where OO.SanctionId = S.SanctionId AND OO.Event = E.Event AND OO.EventGroup = E.EventGroup AND OO.Round = S.Round AND OO.WorkAsgmt = 'Driver' ) " );

			if ( myFilterCmd.Length > 0 ) {
				String curFilterCmd = myFilterCmd.Replace( "InsertDate", "B.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "Round", "B.Round" );
				curSqlStmt.Append( "AND " + curFilterCmd + " " );
			}
			curSqlStmt.Append( "ORDER BY E.Event, S.Round, S.AgeGroup, E.EventGroup, T.SkierName, S.MemberId, BP.PassNumber " );
			try {
				return DataAccess.getDataTable( curSqlStmt.ToString() );
			} catch ( Exception ex ) {
				MessageBox.Show( "Exception encountered retrieving requested data: " + ex.Message );
				return null;
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

		private void SlalomSelectButton_CheckedChanged( object sender, EventArgs e ) {
			if ( SlalomSelectButton.Checked ) myEvent = "Slalom";
			if ( JumpSelectButton.Checked ) myEvent = "Jump";
		}
	}
}
