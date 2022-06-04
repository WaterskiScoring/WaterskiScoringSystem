using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
	public partial class JumpMeasurementImportReport : Form {
		private String mySanctionNum = null;
		private String myTourRules = "";
		private String myOrigItemValue = "";

		private Boolean isDataModified = false;
		private Boolean isDataLoading = false;

		private int myRowIdx = 0;

		private DataRow myTourRow;

		private DataTable myDataTable;
		public JumpMeasurementImportReport() {
			InitializeComponent();
		}

		private void JumpMeasurementImportReport_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.JumpMeasurementImportReport_Width > 0 ) {
				this.Width = Properties.Settings.Default.JumpMeasurementImportReport_Width;
			}
			if ( Properties.Settings.Default.JumpMeasurementImportReport_Height > 0 ) {
				this.Height = Properties.Settings.Default.JumpMeasurementImportReport_Height;
			}
			if ( Properties.Settings.Default.JumpMeasurementImportReport_Location.X > 0
				&& Properties.Settings.Default.JumpMeasurementImportReport_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.JumpMeasurementImportReport_Location;
			}

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;

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
					}
				}
			}
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

		private void JumpMeasurementImportReport_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.JumpMeasurementImportReport_Width = this.Size.Width;
				Properties.Settings.Default.JumpMeasurementImportReport_Height = this.Size.Height;
				Properties.Settings.Default.JumpMeasurementImportReport_Location = this.Location;
			}
		}

		private void navRefresh_Click( object sender, EventArgs e ) {
			this.Cursor = Cursors.WaitCursor;
			myDataTable = getJumpMeasurement();
			loadDataGrid();
			this.Cursor = Cursors.Default;
		}

		private void navExport_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			String filename = "JumpMeasurementImportSlalomExport.txt";
			myExportData.exportData( dataGridView, filename );
		}
		private void loadDataGrid() {
			if ( myDataTable.Rows.Count == 0 ) return;

			DataGridViewRow curViewRow;
			this.Cursor = Cursors.WaitCursor;
			isDataLoading = true;

			dataGridView.Rows.Clear();
			int curIdx = 0;
			foreach ( DataRow curRow in myDataTable.Rows ) {
				curIdx = dataGridView.Rows.Add();
				curViewRow = dataGridView.Rows[curIdx];
				curViewRow.Cells["PK"].Value = (Int64)curRow["PK"];
				curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
				curViewRow.Cells["SkierMemberId"].Value = (String)curRow["MemberId"];
				curViewRow.Cells["SkierName"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["Event"].Value = (String)curRow["Event"];
				curViewRow.Cells["Round"].Value = ( (Byte)curRow["Round"] ).ToString();
				curViewRow.Cells["PassNumber"].Value = ( (Byte)curRow["PassNumber"] ).ToString();
				curViewRow.Cells["ScoreFeet"].Value = ( (Decimal)curRow["ScoreFeet"] ).ToString( "###" );
				curViewRow.Cells["ScoreMeters"].Value = ( (Decimal)curRow["ScoreMeters"] ).ToString( "##.#" );

				curViewRow.Cells["InsertDate"].Value = ( (DateTime)curRow["InsertDate"] ).ToString( "yyyy/MM/dd HH:mm:ss" );
				curViewRow.Cells["LastUpdateDate"].Value = ( (DateTime)curRow["LastUpdateDate"] ).ToString( "yyyy/MM/dd HH:mm:ss" );
				curViewRow.Cells["SkierRunNum"].Value = "";
				if ( curRow["SkierRunNum"] != System.DBNull.Value ) curViewRow.Cells["SkierRunNum"].Value = "Match";
			}

			isDataLoading = false;
			isDataModified = false;
			myRowIdx = 0;
			dataGridView.CurrentCell = dataGridView.Rows[myRowIdx].Cells["SkierMemberId"];
			myOrigItemValue = (String)dataGridView.Rows[myRowIdx].Cells["SkierMemberId"].Value;
			if ( myOrigItemValue == null ) myOrigItemValue = "";
		}

		private void navSave_Click( object sender, EventArgs e ) {
			if ( sender != null ) dataGridView.EndEdit();
			DataGridViewRow curViewRow = dataGridView.Rows[myRowIdx];
			Int64 curPK = (Int64)curViewRow.Cells["PK"].Value;

			String curRound = (String)curViewRow.Cells["Round"].Value;
			String curPassNumber = (String)curViewRow.Cells["PassNumber"].Value;
			String ScoreFeet = (String)curViewRow.Cells["ScoreFeet"].Value;
			String ScoreMeters = (String)curViewRow.Cells["ScoreMeters"].Value;

			String curSkierMemberId = (String)curViewRow.Cells["SkierMemberId"].Value;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Update JumpMeasurement Set " );
			curSqlStmt.Append( " MemberId = '" + curSkierMemberId + "'" );
			curSqlStmt.Append( ", Round = " + curRound );
			curSqlStmt.Append( ", PassNumber = " + curPassNumber );
			curSqlStmt.Append( ", ScoreFeet = " + ScoreFeet );
			curSqlStmt.Append( ", ScoreMeters = " + ScoreMeters );
			curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
			curSqlStmt.Append( "Where PK = " + curPK );
			int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			Log.WriteFile( "JumpMeasurementImportReport:Save:Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
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
						|| curColName.Equals( "Round" )
						|| curColName.Equals( "PassNumber" )
						|| curColName.Equals( "ScoreFeet" )
						|| curColName.Equals( "ScoreMeets" )
						) {
						try {
							myOrigItemValue = (String)dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
							if ( myOrigItemValue == null ) myOrigItemValue = "";
						} catch {
							myOrigItemValue = "";
						}
					}
				}
			}
		}

		private void dataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
			dataGridView.CurrentRow.ErrorText = "";
			myRowIdx = e.RowIndex;
			DataGridViewRow curViewRow = dataGridView.Rows[myRowIdx];

			if ( dataGridView.Rows.Count > 0 ) {
				String curColName = dataGridView.Columns[e.ColumnIndex].Name;
				if ( curColName.Equals( "SkierMemberId" )
					|| curColName.Equals( "Round" )
					|| curColName.Equals( "PassNumber" )
					|| curColName.Equals( "ScoreFeet" )
					|| curColName.Equals( "ScoreMeets" )
					) {
					String curValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
					if ( curValue != myOrigItemValue ) {
						isDataModified = true;
					}
				}
			}
		}

		private DataTable getJumpMeasurement() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT B.PK, B.SanctionId, B.MemberId, T.SkierName, Event" );
			curSqlStmt.Append( ", B.Round, B.PassNumber, B.ScoreFeet, B.ScoreMeters" );
			curSqlStmt.Append( ", B.InsertDate, B.LastUpdateDate " );
			curSqlStmt.Append( ", PassNum as SkierRunNum " );
			curSqlStmt.Append( "FROM JumpMeasurement B " );
			curSqlStmt.Append( "LEFT OUTER JOIN TourReg T ON B.MemberId = T.MemberId AND B.SanctionId = T.SanctionId " );
			curSqlStmt.Append( "LEFT OUTER JOIN JumpRecap R ON B.SanctionId = R.SanctionId AND B.MemberId = R.MemberId AND B.Round = R.Round AND B.PassNumber = R.PassNum " );
			curSqlStmt.Append( "WHERE B.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "ORDER BY B.InsertDate " );
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

}
