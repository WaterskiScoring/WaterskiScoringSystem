﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
    public partial class BoatPathExport : Form {
        private String mySanctionNum = null;
		private String myEvent = "Slalom";
        private String myTourRules = "";
		private String myFilterCmd = "";

		private Boolean isDataLoading = false;

		private DataRow myTourRow;

		private int myRowIdx = 0;

		private DataTable myDataTable;
		private DataTable myBoatPathDevMax;
		private FilterDialogForm filterDialogForm;

		private PrintDocument myPrintDoc;
		private DataGridViewPrinter myPrintDataGrid;

		public BoatPathExport() {
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

		public bool exportReport(String inSanctionNum, String inEvent) {
			ActiveEvent = inEvent;
			this.mySanctionNum = inSanctionNum;
			if ( myTourRow == null ) {
				bool curReturn = getTourData();
				if ( !curReturn ) return false;
			}

			ExportData myExportData = new ExportData();
			String filename = this.mySanctionNum.Substring(0, 6) + "ST.htm";
			if ( myEvent.Equals( "Jump" ) ) filename = this.mySanctionNum.Substring( 0, 6 ) + "JT.htm";
			
			myBoatPathDevMax = getBoatPathDevMax();
			setColumnsView();
			if ( myEvent.Equals( "Jump" ) ) ScoreFeet.Visible = false;

			navRefresh_Click( null, null );
			
			exportHtml( filename );
			
			return true;
		}

		private void BoatPathExport_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.BoatPathExport_Width > 0 ) {
                this.Width = Properties.Settings.Default.BoatPathExport_Width;
            }
            if ( Properties.Settings.Default.BoatPathExport_Height > 0 ) {
                this.Height = Properties.Settings.Default.BoatPathExport_Height;
            }
            if ( Properties.Settings.Default.BoatPathExport_Location.X > 0
                && Properties.Settings.Default.BoatPathExport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.BoatPathExport_Location;
            }

			bool curReturn = getTourData();
			if ( !curReturn ) return;

			myBoatPathDevMax = getBoatPathDevMax();
			setColumnsView();

			String[] curList = { "DriverName", "Round", "PassScoreTime", "BoatPathTime" };
			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;
		}

		private void setColumnsView() {
			bool showBoatTimes = true;

			PathDevZone0.Visible = false;
			PathDevZone1.Visible = false;
			PathDevZone2.Visible = false;
			PathDevZone3.Visible = false;
			PathDevZone4.Visible = false;
			PathDevZone5.Visible = false;
			PathDevZone6.Visible = false;
			Spacer3.Visible = false;

			if ( myEvent.Equals( "Slalom" ) ) {
				this.Text += " - Slalom";

				BoatTime.Visible = showBoatTimes;
				BoatTimeSlalomSP.Visible = showBoatTimes;

				ScoreFeet.Visible = false;
				ScoreMeters.Visible = false;
				SkierBoatPath.Visible = false;

				BoatSplitTime.Visible = false;
				BoatSplitTime2.Visible = false;
				BoatEndTime.Visible = false;
				BoatTimeBuoy1.Visible = false;
				BoatTimeBuoy2.Visible = false;
				BoatTimeBuoy3.Visible = false;

				Space4.Visible = false;
				PathDevJumpCut.Visible = false;
				PathDevJumpFlight.Visible = false;
				PathDevJumpCumul.Visible = false;

			} else {
				this.Text += " - Jump";

				PassNotes.Visible = false;
				Score.Visible = false;
				PassScore.Visible = false;
				BoatTime.Visible = false;
				BoatTimeSlalomSP.Visible = false;

				ScoreFeet.Visible = true;
				ScoreMeters.Visible = true;
				SkierBoatPath.Visible = true;
                PathDevJumpCut.Visible = true;
                PathDevJumpFlight.Visible = true;
                PathDevJumpCumul.Visible = true;

                BoatSplitTime.Visible = showBoatTimes;
				BoatSplitTime2.Visible = showBoatTimes;
				BoatEndTime.Visible = showBoatTimes;
				
				BoatTimeBuoy1.Visible = showBoatTimes;
				BoatTimeBuoy2.Visible = showBoatTimes;
				BoatTimeBuoy3.Visible = showBoatTimes;

				PathDevBuoy0.HeaderText = "Dev 180M";
				PathDevBuoy1.HeaderText = "Dev ST";
				PathDevBuoy2.HeaderText = "Dev NT";
				PathDevBuoy3.HeaderText = "Dev MT";
				PathDevBuoy4.HeaderText = "Dev ET";
				PathDevBuoy5.HeaderText = "Dev EC";
				PathDevBuoy6.Visible = false;

				PathDevCum0.Visible = false;
				PathDevCum1.Visible = false;
				PathDevCum2.Visible = false;
				PathDevCum3.Visible = false;
				PathDevCum4.Visible = false;
				PathDevCum5.Visible = false;
				PathDevCum6.Visible = false;
			}

		}

		private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show("Error happened " + e.Context.ToString());
            if ( e.Context == DataGridViewDataErrorContexts.Commit ) {
                MessageBox.Show("Commit error");
            }
            if ( e.Context == DataGridViewDataErrorContexts.CurrentCellChange ) {
                MessageBox.Show("Cell change");
            }
            if ( e.Context == DataGridViewDataErrorContexts.Parsing ) {
                MessageBox.Show("parsing error");
            }
            if ( e.Context == DataGridViewDataErrorContexts.LeaveControl ) {
                MessageBox.Show("leave control error");
            }
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView) sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        private void BoatPathExport_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.BoatPathExport_Width = this.Size.Width;
                Properties.Settings.Default.BoatPathExport_Height = this.Size.Height;
                Properties.Settings.Default.BoatPathExport_Location = this.Location;
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            this.Cursor = Cursors.WaitCursor;
			if (myEvent.Equals("Slalom")) {
				myDataTable = getSlalomPassBoatPath();
			} else if (myEvent.Equals("Jump")) {
				myDataTable = getJumpPassBoatPath();
			} else {
				myDataTable = getSlalomPassBoatPath();
			}
			loadDataGrid();
            this.Cursor = Cursors.Default;

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
			Font curFontBold = new Font( "Arial Narrow", 9, FontStyle.Bold );
			Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );

			dataGridView.Rows.Clear();
			if ( myDataTable == null || myDataTable.Rows.Count == 0 ) return;
			isDataLoading = true;
			int curIdx = 0;
			foreach ( DataRow curRow in myDataTable.Rows ) {
				curIdx = dataGridView.Rows.Add();
                curViewRow = dataGridView.Rows[curIdx];
                curViewRow.Cells["SanctionId"].Value = (String) curRow["SanctionId"];
                curViewRow.Cells["SkierName"].Value = (String) curRow["SkierName"];
                curViewRow.Cells["Event"].Value = (String) curRow["Event"];
                curViewRow.Cells["AgeGroup"].Value = (String) curRow["AgeGroup"];
                curViewRow.Cells["EventGroup"].Value = (String) curRow["EventGroup"];
				curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curRow, "EventClass", "" );

				curViewRow.Cells["Round"].Value = (Byte) curRow["Round"];
				
				curViewRow.Cells["Boat"].Value = HelperFunctions.getDataRowColValue( curRow, "Boat", "" );
				curViewRow.Cells["DriverName"].Value = HelperFunctions.getDataRowColValue( curRow, "DriverName", "" );

				curViewRow.Cells["RankingScore"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "RankingScore", "", 2 );

				if ( curRow["PassNotes"] != System.DBNull.Value ) {
					String[] nodesPassNotes = ( (String)curRow["PassNotes"] ).Split( ',' );
					if ( nodesPassNotes.Length > 1 ) {
						curViewRow.Cells["PassNotes"].Value = String.Format( "{0} {1}", nodesPassNotes[0], nodesPassNotes[1]);
					} else {
						curViewRow.Cells["PassNotes"].Value = (String)curRow["PassNotes"];
					}
                }

                curViewRow.Cells["RerideReason"].Value = HelperFunctions.getDataRowColValue( curRow, "RerideReason", "" );
                curViewRow.Cells["ScoreNotes"].Value = HelperFunctions.getDataRowColValue( curRow, "ScoreNotes", "" );
				curViewRow.Cells["ScoreDatetime"].Value = HelperFunctions.getDataRowColValue( curRow, "ScoreDatetime", "" );
				curViewRow.Cells["PassDatetime"].Value = HelperFunctions.getDataRowColValue( curRow, "PassDatetime", "" );
				curViewRow.Cells["BoatPathDatetime"].Value = HelperFunctions.getDataRowColValue( curRow, "BoatPathDatetime", "" );

				if ( myEvent.Equals("Slalom")) {
					loadDataGridSlalom( curRow, curViewRow, curFontBold );
				} else {
					loadDataGridJump( curRow, curViewRow );
				}
            }

			isDataLoading = false;

			myRowIdx = dataGridView.Rows.Count - 1;
			if ( dataGridView.Rows.Count > 0 ) {
				dataGridView.FirstDisplayedScrollingRowIndex = myRowIdx;
				dataGridView.Rows[myRowIdx].Selected = true;
				dataGridView.Rows[myRowIdx].Cells[0].Selected = true;
				dataGridView.CurrentCell = dataGridView.Rows[myRowIdx].Cells["SkierName"];

				int curRowPos = myRowIdx + 1;
				RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + dataGridView.Rows.Count.ToString();

			} else {
				RowStatusLabel.Text = "";
			}
		}

		private void loadDataGridSlalom( DataRow curRow, DataGridViewRow curViewRow, Font curFontBold ) {
			String curSkierRunNum = HelperFunctions.getDataRowColValue( curRow, "SkierRunNum", "" );
			if ( HelperFunctions.isObjectEmpty( curSkierRunNum ) ) return;

			Decimal curPassScore = HelperFunctions.getDataRowColValueDecimal( curRow, "PassScore", 0 );
			curViewRow.Cells["SkierRunNum"].Value = curSkierRunNum;
			curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "Score", 0 );
			curViewRow.Cells["PassScore"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PassScore", 0 );
			curViewRow.Cells["BoatTime"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatTime", 0 );
			curViewRow.Cells["TimeInTol"].Value = HelperFunctions.getDataRowColValue( curRow, "TimeInTol", "" );

			if ( curRow["PathDevBuoy0"] != System.DBNull.Value ) {
				curViewRow.Cells["PathDevBuoy0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy0", 0 );
				if ( (Decimal)myBoatPathDevMax.Rows[0]["MinDev"] > 0
					&& Math.Abs( (Decimal)curRow["PathDevBuoy0"] ) > (Decimal)myBoatPathDevMax.Rows[0]["MinDev"] ) {
					curViewRow.Cells["PathDevBuoy0"].Style.Font = curFontBold;
					curViewRow.Cells["PathDevBuoy0"].Style.ForeColor = Color.Red;
				}
			}
			if ( curRow["PathDevZone0"] != System.DBNull.Value ) {
				curViewRow.Cells["PathDevZone0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevZone0", 0 );
				if ( (Decimal)myBoatPathDevMax.Rows[0]["MinDev"] > 0 
					&& Math.Abs( (Decimal)curRow["PathDevZone0"] ) > (Decimal)myBoatPathDevMax.Rows[0]["MinDev"] ) {
					curViewRow.Cells["PathDevZone0"].Style.Font = curFontBold;
					curViewRow.Cells["PathDevZone0"].Style.ForeColor = Color.Red;
				}
			}
			if ( curRow["PathDevCum0"] != System.DBNull.Value ) {
				curViewRow.Cells["PathDevCum0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum0", 0 );
				if ( (Decimal)myBoatPathDevMax.Rows[0]["MaxDev"] > 0
					&& Math.Abs( (Decimal)curRow["PathDevCum0"] ) > (Decimal)myBoatPathDevMax.Rows[0]["MaxDev"] ) {
					curViewRow.Cells["PathDevCum0"].Style.Font = curFontBold;
					curViewRow.Cells["PathDevCum0"].Style.ForeColor = Color.Red;
				}
			}
			for ( int curIdx = 1; curIdx <= 6; curIdx++ ) {
				if ( curPassScore <= (curIdx - 1) ) break;

				if ( curRow["PathDevBuoy" + curIdx] != System.DBNull.Value ) {
					curViewRow.Cells["PathDevBuoy" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy" + curIdx, 0 );
					if ( Math.Abs( (Decimal)curRow["PathDevBuoy" + curIdx]) > (Decimal)myBoatPathDevMax.Rows[curIdx]["MinDev"] ) {
						curViewRow.Cells["PathDevBuoy" + curIdx].Style.Font = curFontBold;
						curViewRow.Cells["PathDevBuoy" + curIdx].Style.ForeColor = Color.Red;
					}
				}
				if ( curRow["PathDevCum" + curIdx] != System.DBNull.Value ) {
					curViewRow.Cells["PathDevCum" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevCum" + curIdx, 0 );
					if ( Math.Abs( (Decimal)curRow["PathDevCum" + curIdx]) > (Decimal)myBoatPathDevMax.Rows[curIdx]["MaxDev"] ) {
						curViewRow.Cells["PathDevCum" + curIdx].Style.Font = curFontBold;
						curViewRow.Cells["PathDevCum" + curIdx].Style.ForeColor = Color.Red;
					}
				}
				if ( curRow["PathDevZone" + curIdx] != System.DBNull.Value ) {
					curViewRow.Cells["PathDevZone" + curIdx].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevZone" + curIdx, 0 );
					if ( Math.Abs( (Decimal)curRow["PathDevZone" + curIdx]) > (Decimal)myBoatPathDevMax.Rows[curIdx]["MinDev"] ) {
						curViewRow.Cells["PathDevZone" + curIdx].Style.Font = curFontBold;
						curViewRow.Cells["PathDevZone" + curIdx].Style.ForeColor = Color.Red;
					}
				}
				int tempPassScore = Convert.ToInt32( Math.Floor( curPassScore ) ) + 1;
				String timeKey = "BoatTimeBuoy" + tempPassScore;
				curViewRow.Cells["BoatTimeSlalomSP"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, timeKey, "", 2 );
			}
		}

		private void loadDataGridJump( DataRow curRow, DataGridViewRow curViewRow ) {
			curViewRow.Cells["SkierRunNum"].Value = HelperFunctions.getDataRowColValue( curRow, "PassNum", "0" );

			curViewRow.Cells["SkierBoatPath"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierBoatPath", "" );
			curViewRow.Cells["ScoreFeet"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreFeet", 0 );
			curViewRow.Cells["ScoreMeters"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreMeters", 0 );

			curViewRow.Cells["TimeInTol"].Value = HelperFunctions.getDataRowColValue( curRow, "TimeInTol", "" );
			curViewRow.Cells["BoatSplitTime"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSplitTime", "", 2 );
			curViewRow.Cells["BoatSplitTime2"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatSplitTime2", "", 2 );
			curViewRow.Cells["BoatEndTime"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatEndTime", "", 2 );
			curViewRow.Cells["BoatTimeBuoy1"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatTimeBuoy1", "", 2 );
			curViewRow.Cells["BoatTimeBuoy2"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatTimeBuoy2", "", 2 );
			curViewRow.Cells["BoatTimeBuoy3"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "BoatTimeBuoy3", "", 2 );

			curViewRow.Cells["PathDevBuoy0"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy0", 0 );
			curViewRow.Cells["PathDevBuoy1"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy1", 0 );
			curViewRow.Cells["PathDevBuoy2"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy2", 0 );
			curViewRow.Cells["PathDevBuoy3"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy3", 0 );
			curViewRow.Cells["PathDevBuoy4"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy4", 0 );
			curViewRow.Cells["PathDevBuoy5"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy5", 0 );

			curViewRow.Cells["PathDevJumpCut"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy3", 0 ) - HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy2", 0 );
			curViewRow.Cells["PathDevJumpFlight"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy4", 0 ) - HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy3", 0 );
			curViewRow.Cells["PathDevJumpCumul"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy4", 0 ) - HelperFunctions.getDataRowColValueDecimal( curRow, "PathDevBuoy2", 0 );
		}

		private void dataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			if ( isDataLoading ) return;
			if ( myDataTable.Rows.Count == 0 ) return;

			DataGridView curView = (DataGridView)sender;
			curView.EndEdit();

			int curRowPos = e.RowIndex + 1;
			RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + curView.Rows.Count.ToString();
		}

		private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
			String filename = "BoatPathSlalomExport.txt";
			if (myEvent.Equals("Jump")) filename = "BoatPathJumpExport.txt";
			navRefresh_Click( null, null );
			myExportData.exportData(dataGridView, filename );
        }

        private void navExportHtml_Click( object sender, EventArgs e ) {
			// Retrieve data from database
			String filename = "BoatPathSlalomExport.htm";
			if (myEvent.Equals("Jump")) filename = "BoatPathJumpExport.htm";
			exportHtml( filename );
        }
		
		private void exportHtml( String filename ) {
			ExportData myExportData = new ExportData();
			String printTitle = Properties.Settings.Default.Mdi_Title;
			String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
			String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
			printFooter.PadRight( 15, '*' );
			printFooter.PadLeft( 15, '*' );

			myExportData.exportDataAsHtml( dataGridView, printTitle, printSubtitle, printFooter, filename );
		}

		private void navPrintResults_Click( object sender, EventArgs e ) {
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
			if ( dataGridView.Rows.Count == 0 ) {
				MessageBox.Show( "No data available to print" );
				return;
			}

			ScoreDatetime.Visible = false;
			PassDatetime.Visible = false;
			BoatPathDatetime.Visible = false;
			BoatTimeSlalomSP.Visible = false;
			Spacer0.Visible = false;
			Spacer1.Visible = false;
			Spacer2.Visible = false;

			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
			bool CenterOnPage = true;
			bool WithTitle = true;
			bool WithPaging = true;
			Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
			Font fontPrintFooter = new Font( "Times New Roman", 10 );

			PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
			if ( curPrintDialog == null ) return;

			StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
			printTitle.Append( "\n Sanction " + mySanctionNum );
			printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
			printTitle.Append( "\n" + this.Text );

			myPrintDoc = new PrintDocument();
			myPrintDoc.DocumentName = this.Text;
			myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
			myPrintDoc.DefaultPageSettings.Landscape = true;
			myPrintDataGrid = new DataGridViewPrinter( dataGridView, myPrintDoc,
				CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

			myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
			myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
			myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

			curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
			curPreviewDialog.ShowDialog();

			ScoreDatetime.Visible = false;
			PassDatetime.Visible = true;
			BoatPathDatetime.Visible = true;
			BoatTimeSlalomSP.Visible = true;
			Spacer0.Visible = true;
			Spacer1.Visible = true;
			Spacer2.Visible = true;
		}

		// The PrintPage action for the PrintDocument control
		private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
			bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
			if ( more == true )
				e.HasMorePages = true;
		}

        private DataTable getSlalomPassBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append( "SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup, E.EventGroup, E.EventClass, E.RankingScore" );
			curSqlStmt.Append( ", S.Round, BP.BoatDescription as Boat, BP.DriverName" );
			curSqlStmt.Append( ", S.Score, R.SkierRunNum, R.BoatTime, R.Score AS PassScore, R.TimeInTol, R.Note AS PassNotes, S.Note as ScoreNotes, R.RerideReason" );
			curSqlStmt.Append( ", S.LastUpdateDate as ScoreDatetime, R.InsertDate as PassDatetime, BP.InsertDate as BoatPathDatetime" );
			curSqlStmt.Append( ", COALESCE(BP.homologation, '') as homologation, BP.PassLineLength, BP.PassSpeedKph" );
			curSqlStmt.Append( ", BP.PathDevBuoy0, BP.PathDevCum0, PathDevZone0" );
			curSqlStmt.Append( ", BP.PathDevBuoy1, BP.PathDevCum1, PathDevZone1" );
			curSqlStmt.Append( ", BP.PathDevBuoy2, BP.PathDevCum2, PathDevZone2" );
			curSqlStmt.Append( ", BP.PathDevBuoy3, BP.PathDevCum3, PathDevZone3" );
			curSqlStmt.Append( ", BP.PathDevBuoy4, BP.PathDevCum4, PathDevZone4" );
			curSqlStmt.Append( ", BP.PathDevBuoy5, BP.PathDevCum5, PathDevZone5" );
			curSqlStmt.Append( ", BP.PathDevBuoy6, BP.PathDevCum6, PathDevZone6 " );
			curSqlStmt.Append( ", BT.BoatTimeBuoy1, BT.BoatTimeBuoy2, BT.BoatTimeBuoy3, BT.BoatTimeBuoy4, BT.BoatTimeBuoy5, BT.BoatTimeBuoy6, BT.BoatTimeBuoy7 " );
			curSqlStmt.Append( "FROM SlalomScore S " );
			curSqlStmt.Append( "INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			curSqlStmt.Append( "INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "LEFT OUTER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round AND BP.PassNumber = R.SkierRunNum AND BP.Event = E.Event " );
			curSqlStmt.Append( "LEFT OUTER JOIN BoatTime BT ON S.SanctionId = BT.SanctionId AND S.MemberId = BT.MemberId AND BT.Round = R.Round AND BT.PassNumber = R.SkierRunNum AND BT.Event = E.Event " );

			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Slalom' " );
			if ( myFilterCmd.Length > 0 ) {
				String curFilterCmd = myFilterCmd.Replace( "BoatPathTime", "BP.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "PassScoreTime", "R.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "Round", "S.Round" );
				curSqlStmt.Append( "AND " + curFilterCmd + " " );
			}
			curSqlStmt.Append( "ORDER BY R.InsertDate, S.Round, E.EventGroup, S.AgeGroup, T.SkierName, S.MemberId, R.SkierRunNum " );
			return DataAccess.getDataTable(curSqlStmt.ToString());
        }

		private DataTable getJumpPassBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append( "SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup, E.EventGroup, E.EventClass, E.RankingScore" );
			curSqlStmt.Append( ", S.Round,BP.BoatDescription as Boat, BP.DriverName as DriverName" );
			curSqlStmt.Append( ", R.ScoreFeet, R.ScoreMeters, R.SkierBoatPath" );
			curSqlStmt.Append( ", R.PassNum, R.BoatSplitTime, R.BoatSplitTime2, BoatEndTime, R.ScoreFeet as PassScoreFeet, R.ScoreMeters as PassScoreMeters" );
			curSqlStmt.Append( ", R.TimeInTol, R.Note AS PassNotes, S.Note as ScoreNotes, R.RerideReason" );
			curSqlStmt.Append( ", S.LastUpdateDate as ScoreDatetime, R.InsertDate as PassDatetime, BP.InsertDate as BoatPathDatetime" );
			curSqlStmt.Append( ", COALESCE(BP.homologation, '') as homologation, BP.PassLineLength, BP.PassSpeedKph" );
			curSqlStmt.Append( ", BP.PathDevBuoy0, BP.PathDevCum0" );
			curSqlStmt.Append( ", BP.PathDevBuoy1, BP.PathDevCum1" );
			curSqlStmt.Append( ", BP.PathDevBuoy2, BP.PathDevCum2" );
			curSqlStmt.Append( ", BP.PathDevBuoy3, BP.PathDevCum3 " );
			curSqlStmt.Append( ", BP.PathDevBuoy4, BP.PathDevCum4" );
			curSqlStmt.Append( ", BP.PathDevBuoy5, BP.PathDevCum5" );
			curSqlStmt.Append( ", BP.PathDevBuoy6, BP.PathDevCum6 " );
			curSqlStmt.Append( ", BT.BoatTimeBuoy1, BT.BoatTimeBuoy2, BT.BoatTimeBuoy3 " );
			curSqlStmt.Append( "FROM JumpScore S " );
			curSqlStmt.Append( "INNER JOIN JumpRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
			curSqlStmt.Append( "INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup " );
			curSqlStmt.Append( "LEFT OUTER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round AND BP.PassNumber = R.PassNum AND BP.Event = E.Event " );
			curSqlStmt.Append( "LEFT OUTER JOIN BoatTime BT ON S.SanctionId = BT.SanctionId AND S.MemberId = BT.MemberId AND BT.Round = R.Round AND BT.PassNumber = R.PassNum AND BT.Event = E.Event " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Jump' " );
			if ( myFilterCmd.Length > 0 ) {
				String curFilterCmd = myFilterCmd.Replace( "BoatPathTime", "BP.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "PassScoreTime", "R.InsertDate" );
				curFilterCmd = curFilterCmd.Replace( "Round", "S.Round" );
				curSqlStmt.Append( "AND " + curFilterCmd + " " );
			}
			curSqlStmt.Append( "ORDER BY R.InsertDate, S.Round, E.EventGroup, S.AgeGroup, T.SkierName, S.MemberId, R.PassNum " );

			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private bool getTourData() {
			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return false;
			}
			if ( mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return false;
			}
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
			curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );

			DataTable curTourDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curTourDataTable == null || curTourDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return false;
			}
			myTourRow = curTourDataTable.Rows[0];
			myTourRules = (String)myTourRow["Rules"];

			return true;
		}

		private DataTable getBoatPathDevMax() {
			String curListName = myEvent + "BoatPathDevMax";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListCode as Buoy, ListCodeNum as BuoyNum, CodeValue as CodeValueDesc, MaxValue as MaxDev, MinValue as MinDev " );
			curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = '" + curListName + "' " );
			curSqlStmt.Append( "ORDER BY SortSeq" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

	}
}