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

			//setColumnsView();

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
			if ( myDataTable == null || myDataTable.Rows.Count == 0 ) return;
			int curIdx = 0;
			foreach ( DataRow curRow in myDataTable.Rows ) {
				curIdx = dataGridView.Rows.Add();
				curViewRow = dataGridView.Rows[curIdx];
				curViewRow.Cells["PK"].Value = (Int64)curRow["PK"];
				curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
				curViewRow.Cells["SkierMemberId"].Value = (String)curRow["MemberId"];
				curViewRow.Cells["SkierName"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curRow, "homologation", "" );

				curViewRow.Cells["Event"].Value = (String)curRow["Event"];
				curViewRow.Cells["Round"].Value = ( (Byte)curRow["Round"] ).ToString();
				curViewRow.Cells["PassNumber"].Value = ( (Byte)curRow["PassNumber"] ).ToString();
				curViewRow.Cells["Boat"].Value = HelperFunctions.getDataRowColValue( curRow, "BoatDescription", "" );

				curViewRow.Cells["DriverMemberId"].Value = HelperFunctions.getDataRowColValue( curRow, "DriverMemberId", "" );
				curViewRow.Cells["DriverName"].Value = HelperFunctions.getDataRowColValue( curRow, "DriverName", "" );
				curViewRow.Cells["DriverMemberId"].Value = HelperFunctions.getDataRowColValue( curRow, "DriverMemberId", "" );

				curViewRow.Cells["InsertDate"].Value = ( (DateTime)curRow["InsertDate"] ).ToString( "yyyy/MM/dd HH:mm:ss" );
				curViewRow.Cells["LastUpdateDate"].Value = ( (DateTime)curRow["LastUpdateDate"] ).ToString( "yyyy/MM/dd HH:mm:ss" );

				curViewRow.Cells["RerideNote"].Value = HelperFunctions.getDataRowColValue( curRow, "RerideNote", "" );
				curViewRow.Cells["SkierRunNum"].Value = "";
				if ( curRow["SkierRunNum"] != System.DBNull.Value ) curViewRow.Cells["SkierRunNum"].Value = "Match";

				if ( myEvent.Equals( "Slalom" ) ) {
					loadDataGridSlalom( curRow, curViewRow );
				} else {
					loadDataGridJump( curRow, curViewRow );
				}
			}

			isDataLoading = false;
			isDataModified = false;

			myRowIdx = dataGridView.Rows.Count - 1;
			if ( dataGridView.Rows.Count > 0 ) {
				dataGridView.FirstDisplayedScrollingRowIndex = myRowIdx;
				dataGridView.Rows[myRowIdx].Selected = true;
				dataGridView.Rows[myRowIdx].Cells[0].Selected = true;
				dataGridView.CurrentCell = dataGridView.Rows[myRowIdx].Cells["SkierMemberId"];
				myOrigItemValue = (String)dataGridView.Rows[myRowIdx].Cells["SkierMemberId"].Value;

				int curRowPos = myRowIdx + 1;
				RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + dataGridView.Rows.Count.ToString();

			} else {
				RowStatusLabel.Text = "";
			}
			Cursor.Current = Cursors.Default;


			if ( myOrigItemValue == null ) myOrigItemValue = "";
		}

		void loadDataGridSlalom( DataRow curRow, DataGridViewRow curViewRow ) {
			curViewRow.Cells["PassSpeedKph"].Value = HelperFunctions.getDataRowColValue( curRow, "PassSpeedKph", "" );
			curViewRow.Cells["PassLineLength"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PassLineLength", "", 2 );

			curViewRow.Cells["PathDevBuoy0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy0", "", 2 );
			curViewRow.Cells["PathDevCum0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum0", "", 2 );
			curViewRow.Cells["PathDevZone0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevZone0", "", 2 );

			for ( int curIdx = 1; curIdx <= 6; curIdx++ ) {
				curViewRow.Cells["PathDevBuoy" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy" + curIdx, "", 2 );
				curViewRow.Cells["PathDevCum" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum" + curIdx, "", 2 );
				curViewRow.Cells["PathDevZone" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevZone" + curIdx, "", 2 );
			}
		}

		private void loadDataGridJump( DataRow curRow, DataGridViewRow curViewRow ) {
			curViewRow.Cells["SkierBoatPath"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierBoatPath", "" );
			curViewRow.Cells["PassSpeedKph"].Value = HelperFunctions.getDataRowColValue( curRow, "PassSpeedKph", "" );

			curViewRow.Cells["PathDevBuoy0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy0", "", 2 );
			curViewRow.Cells["PathDevCum0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum0", "", 2 );

			curViewRow.Cells["PathDevBuoy1"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy1", "", 2 );
			curViewRow.Cells["PathDevCum1"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum1", "", 2 );

			curViewRow.Cells["PathDevBuoy2"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy2", "", 2 );
			curViewRow.Cells["PathDevCum2"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum2", "", 2 );

			curViewRow.Cells["PathDevBuoy3"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy3", "", 2 );
			curViewRow.Cells["PathDevCum3"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum3", "", 2 );

			curViewRow.Cells["PathDevBuoy4"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy4", "", 2 );
			curViewRow.Cells["PathDevCum4"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum4", "", 2 );

			curViewRow.Cells["PathDevBuoy5"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy5", "", 2 );
			curViewRow.Cells["PathDevCum5"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum5", "", 2 );
		}

		private void navSave_Click( object sender, EventArgs e ) {
			if ( sender != null ) dataGridView.EndEdit();
			DataGridViewRow curViewRow = dataGridView.Rows[myRowIdx];
			Int64 curPK = (Int64)curViewRow.Cells["PK"].Value;

			String curRound = (String)curViewRow.Cells["Round"].Value;
			String curPassNumber = (String)curViewRow.Cells["PassNumber"].Value;
			String curPassLineLength = (String)curViewRow.Cells["PassLineLength"].Value;
			String curPassSpeedKph = (String)curViewRow.Cells["PassSpeedKph"].Value;

			String curSkierMemberId = (String)curViewRow.Cells["SkierMemberId"].Value;
			String curDriverMemberId = (String)curViewRow.Cells["DriverMemberId"].Value;
			String curDriverName = (String)curViewRow.Cells["DriverName"].Value;
			String curBoat = (String)curViewRow.Cells["Boat"].Value;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Update BoatPath Set " );
			curSqlStmt.Append( " MemberId = '" + curSkierMemberId + "'" );
			curSqlStmt.Append( ", DriverMemberId = '" + curDriverMemberId + "'" );
			curSqlStmt.Append( ", DriverName = '" + curDriverName + "'" );
			curSqlStmt.Append( ", Round = " + curRound );
			curSqlStmt.Append( ", PassNumber = " + curPassNumber );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", PassLineLength = " + curPassLineLength );
				curSqlStmt.Append( ", PassSpeedKph = " + curPassSpeedKph );
			}
			curSqlStmt.Append( ", BoatDescription = '" + curBoat + "'" );
			curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
			curSqlStmt.Append( "Where PK = " + curPK );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			Log.WriteFile( "BoatPathDriverUpdate:Save:Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
			isDataModified = false;
		}

		private void dataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			if ( isDataLoading ) return;
			if ( myDataTable.Rows.Count == 0 ) return;

			DataGridView curView = (DataGridView)sender;
			curView.EndEdit();

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
					if ( !( HelperFunctions.isObjectEmpty( curView.Rows[e.RowIndex].Cells["SkierMemberId"].Value ) ) ) {
						myRowIdx = e.RowIndex;
						isDataModified = false;
					}
				}
			}
		}

		private void dataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
			if ( dataGridView.Rows.Count > 0 ) {
				if ( !( dataGridView.Columns[e.ColumnIndex].ReadOnly ) ) {
					String curColName = dataGridView.Columns[e.ColumnIndex].Name;
					if ( curColName.Equals( "SkierMemberId" )
						|| curColName.Equals( "DriverMemberId" )
						|| curColName.Equals( "PassSpeedKph" )
						|| curColName.Equals( "PassLineLength" )
						|| curColName.Equals( "Round" )
						|| curColName.Equals( "PassNumber" )
						|| curColName.Equals( "Boat" )
						) {
						myOrigItemValue = HelperFunctions.getViewRowColValue( dataGridView.Rows[e.RowIndex], curColName, "" );
					}
				}
			}
		}
		private DataTable getBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup as Div, E.EventGroup as Grp, E.EventClass as Cls" );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", S.Round as Rd, R.SkierRunNum as RunNum, BP.PassLineLength as LineLen, BP.PassSpeedKph as BoatSpeed" );
			} else {
				curSqlStmt.Append( ", S.Round as Rd, R.PassNum as RunNum, BP.PassSpeedKph as BoatSpeed" );
			}
			curSqlStmt.Append( ", BP.DriverName DriverBpms, BP.DriverMemberId as DriverIdBpms" );
			curSqlStmt.Append( ", OT.SkierName AS DriverAssigned, OT.MemberId AS DriverIdAssigned, O.WorkAsgmt" );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( ", S.Score, R.Score AS PassScore" );
			} else {
				curSqlStmt.Append( ", S.Score, R.ScoreFeet, R.ScoreMeters" );
			}
			curSqlStmt.Append( ", R.TimeInTol as InTol" );
			curSqlStmt.Append( ", BP.RerideNote, R.Note AS PassNotes, S.Note as ScoreNotes, R.InsertDate as PassDatatime, BP.InsertDate as BoatPathDatatime " );
			if ( myEvent.Equals( "Slalom" ) ) {
				curSqlStmt.Append( "FROM SlalomScore S " );
				curSqlStmt.Append( "INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			} else {
				curSqlStmt.Append( "FROM JumpScore S " );
				curSqlStmt.Append( "INNER JOIN JumpRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			}
			curSqlStmt.Append( "INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round  AND BP.PassNumber = PassNum AND BP.Event = E.Event " );
			curSqlStmt.Append( "INNER JOIN OfficialWorkAsgmt O ON O.SanctionId = S.SanctionId AND O.Event = E.Event AND O.EventGroup = E.EventGroup AND O.Round = S.Round AND O.WorkAsgmt = 'Driver' " );
			curSqlStmt.Append( "INNER JOIN TourReg OT ON O.MemberId = OT.MemberId AND O.SanctionId = OT.SanctionId " );
			curSqlStmt.Append( "Where S.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + myEvent + "' " );
			curSqlStmt.Append( "AND BP.DriverMemberId != O.MemberId " );
			if ( myFilterCmd.Length > 0 ) {
				String curFilterCmd = myFilterCmd.Replace( "InsertDate", "B.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "Round", "B.Round" );
				curSqlStmt.Append( "AND " + curFilterCmd + " " );
			}
			curSqlStmt.Append( "ORDER BY E.Event, S.Round, S.AgeGroup, E.EventGroup, T.SkierName, S.MemberId, RunNum " );
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
	}
}
