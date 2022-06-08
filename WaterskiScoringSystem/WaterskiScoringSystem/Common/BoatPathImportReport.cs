using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
	public partial class BoatPathImportReport : Form {
		private String mySanctionNum = null;
		private String myEvent = "Slalom";
		private String myTourRules = "";
		private String myOrigItemValue = "";

		private Boolean isDataModified = false;
		private Boolean isDataLoading = false;

		private int myRowIdx = 0;
		private DataRow myTourRow;

		private DataTable myDataTable;

		public BoatPathImportReport() {
			InitializeComponent();
		}

		public String ActiveEvent {
			get {
				return myEvent;
			}
			set {
				myEvent = value;
			}
		}

		private void BoatPathImportReport_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.BoatPathImportReport_Width > 0 ) {
				this.Width = Properties.Settings.Default.BoatPathImportReport_Width;
			}
			if ( Properties.Settings.Default.BoatPathImportReport_Height > 0 ) {
				this.Height = Properties.Settings.Default.BoatPathImportReport_Height;
			}
			if ( Properties.Settings.Default.BoatPathImportReport_Location.X > 0
				&& Properties.Settings.Default.BoatPathImportReport_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.BoatPathImportReport_Location;
			}

			setColumnsView();

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

		private void setColumnsView() {
			PathDevZone0.Visible = false;
			PathDevZone1.Visible = false;
			PathDevZone2.Visible = false;
			PathDevZone3.Visible = false;
			PathDevZone4.Visible = false;
			PathDevZone5.Visible = false;
			PathDevZone6.Visible = false;

			if ( myEvent.Equals( "Slalom" ) ) {
				this.Text += " - Slalom";
				SkierBoatPath.Visible = false;

			} else {
				this.Text += " - Jump";

				PathDevBuoy0.HeaderText = "Dev 180M";
				PathDevBuoy1.HeaderText = "Dev ST";
				PathDevBuoy2.HeaderText = "Dev NT";
				PathDevBuoy3.HeaderText = "Dev MT";
				PathDevBuoy4.HeaderText = "Dev ET";
				PathDevBuoy5.HeaderText = "Dev EC";
				SkierBoatPath.Visible = true;
				PathDevBuoy6.Visible = false;

				PathDevCum0.Visible = false;
				PathDevCum1.Visible = false;
				PathDevCum2.Visible = false;
				PathDevCum3.Visible = false;
				PathDevCum4.Visible = false;
				PathDevCum5.Visible = false;
				PathDevCum6.Visible = false;
				PassLineLength.Visible = false;
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

		private void BoatPathImportReport_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.BoatPathImportReport_Width = this.Size.Width;
				Properties.Settings.Default.BoatPathImportReport_Height = this.Size.Height;
				Properties.Settings.Default.BoatPathImportReport_Location = this.Location;
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
			String filename = "BoatPathImportSlalomExport.txt";
			if ( myEvent.Equals( "Jump" ) ) filename = "BoatPathImportJumpExport.txt";
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
				curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curRow, "homologation", "" );
				
				curViewRow.Cells["Event"].Value = (String)curRow["Event"];
				curViewRow.Cells["Round"].Value = ((Byte)curRow["Round"]).ToString();
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
			myRowIdx = 0;
			dataGridView.CurrentCell = dataGridView.Rows[myRowIdx].Cells["SkierMemberId"];
			myOrigItemValue = (String)dataGridView.Rows[myRowIdx].Cells["SkierMemberId"].Value;
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
			Log.WriteFile( "BoatPathImportReport:Save:Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
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
				String curValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
				if ( curColName.Equals( "PassSpeedKph" )
					|| curColName.Equals( "PassLineLength" )
					|| curColName.Equals( "Round" )
					|| curColName.Equals( "PassNumber" )
					|| curColName.Equals( "Boat" )
					) {
					if ( curValue != myOrigItemValue ) isDataModified = true;
				
				} else if ( curColName.Equals( "SkierMemberId" ) ) {
					if ( curValue != myOrigItemValue ) {
						isDataModified = true;
						curViewRow.Cells["SkierName"].Value = getMemberName( curValue );
					}

				} else if ( curColName.Equals( "DriverMemberId" ) ) {
					if ( curValue != myOrigItemValue ) {
						isDataModified = true;
						curViewRow.Cells["DriverName"].Value = getMemberName( curValue );
					}
				}

			}
		}

		private DataTable getBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT B.PK, B.SanctionId, B.MemberId, T.SkierName, Event, COALESCE(B.homologation, '') as homologation" );
			curSqlStmt.Append( ", B.Round, B.PassNumber, B.PassLineLength, B.PassSpeedKph" );
			curSqlStmt.Append( ", BoatDescription, DriverMemberId, DriverName, B.RerideNote" );
			curSqlStmt.Append( ", PathDevBuoy0, PathDevCum0, PathDevZone0" );
			curSqlStmt.Append( ", PathDevBuoy1, PathDevCum1, PathDevZone1" );
			curSqlStmt.Append( ", PathDevBuoy2, PathDevCum2, PathDevZone2" );
			curSqlStmt.Append( ", PathDevBuoy3, PathDevCum3, PathDevZone3" );
			curSqlStmt.Append( ", PathDevBuoy4, PathDevCum4, PathDevZone4" );
			curSqlStmt.Append( ", PathDevBuoy5, PathDevCum5, PathDevZone5" );
			curSqlStmt.Append( ", PathDevBuoy6, PathDevCum6, PathDevZone6 " );
			curSqlStmt.Append( ", B.InsertDate, B.LastUpdateDate " );
			if ( myEvent.Equals( "Jump" ) ) {
				curSqlStmt.Append( ", PassNum as SkierRunNum, SkierBoatPath " );
			} else {
				curSqlStmt.Append( ", SkierRunNum " );
			}
			curSqlStmt.Append( "FROM BoatPath B " );
			curSqlStmt.Append( "LEFT OUTER JOIN TourReg T ON B.MemberId = T.MemberId AND B.SanctionId = T.SanctionId " );
			if ( myEvent.Equals( "Jump" ) ) {
				curSqlStmt.Append( "LEFT OUTER JOIN JumpRecap R ON B.SanctionId = R.SanctionId AND B.MemberId = R.MemberId AND B.Round = R.Round AND B.PassNumber = R.PassNum " );
			} else {
				curSqlStmt.Append( "LEFT OUTER JOIN SlalomRecap R ON B.SanctionId = R.SanctionId AND B.MemberId = R.MemberId AND B.Round = R.Round AND B.PassNumber = R.SkierRunNum " );
			}
			curSqlStmt.Append( "WHERE B.SanctionId = '" + mySanctionNum + "' AND B.Event = '" + myEvent + "' " );
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

		private String getMemberName(String memberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SkierName From TourReg " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "'" );
			curSqlStmt.Append( "  AND MemberId = '" + memberId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) return (String)curDataTable.Rows[0]["SkierName"];
			return "MemberId Not Found";
		}

	}
}