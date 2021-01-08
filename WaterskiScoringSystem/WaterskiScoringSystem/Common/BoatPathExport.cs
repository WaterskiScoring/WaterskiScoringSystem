using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
    public partial class BoatPathExport : Form {
        private String mySanctionNum = null;
		private String myEvent = "Slalom";
        private String myTourRules = "";
        private DataRow myTourRow;
        private DataTable myDataTable;
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

			if (myEvent.Equals("Slalom")) {
				this.Text += " - Slalom";
			} else {
				this.Text += " - Jump";
			}

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String) myTourRow["Rules"];
                    }
                }
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
        private void loadDataGrid() {
            DataGridViewRow curViewRow;
            this.Cursor = Cursors.WaitCursor;

            dataGridView.Rows.Clear();
            int curIdx = 0;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                curIdx = dataGridView.Rows.Add();
                curViewRow = dataGridView.Rows[curIdx];
                curViewRow.Cells["SanctionId"].Value = (String) curRow["SanctionId"];
                curViewRow.Cells["SkierName"].Value = (String) curRow["SkierName"];
                curViewRow.Cells["Event"].Value = (String) curRow["Event"];
                curViewRow.Cells["AgeGroup"].Value = (String) curRow["AgeGroup"];
                curViewRow.Cells["EventGroup"].Value = (String) curRow["EventGroup"];
                curViewRow.Cells["EventClass"].Value = (String) curRow["EventClass"];
                curViewRow.Cells["Round"].Value = (Byte) curRow["Round"];
                try {
                    curViewRow.Cells["Boat"].Value = (String) curRow["Boat"];
                } catch {
                    curViewRow.Cells["Boat"].Value = "";
                }
                try {
                    curViewRow.Cells["Driver"].Value = (String) curRow["Driver"];
                } catch {
                    curViewRow.Cells["Driver"].Value = "";
                }
				
                curViewRow.Cells["RankingScore"].Value = ( (Decimal) curRow["RankingScore"] ).ToString( "###.00" );
				if (curRow["PassNotes"] != System.DBNull.Value) curViewRow.Cells["PassNotes"].Value = (String)curRow["PassNotes"];

				if (myEvent.Equals("Slalom")) {
					ScoreFeet.Visible = false;
					ScoreMeters.Visible = false;
					BoatSplitTime.Visible = false;
					BoatSplitTime2.Visible = false;
					BoatEndTime.Visible = false;

					Decimal curPassScore = (Decimal)curRow["PassScore"];
					curViewRow.Cells["SkierRunNum"].Value = ((Int16)curRow["SkierRunNum"]).ToString();
					curViewRow.Cells["Score"].Value = ((Decimal)curRow["Score"]).ToString("###.00");
					curViewRow.Cells["PassScore"].Value = ((Decimal)curRow["PassScore"]).ToString("#.00");
					curViewRow.Cells["BoatTime"].Value = ((Decimal)curRow["BoatTime"]).ToString("#0.00");

					if (curRow["PathDevBuoy0"] != System.DBNull.Value) curViewRow.Cells["PathDevBuoy0"].Value = ((Decimal)curRow["PathDevBuoy0"]).ToString("#0.00");
					if (curRow["PathDevcum0"] != System.DBNull.Value) curViewRow.Cells["PathDevcum0"].Value = ((Decimal)curRow["PathDevcum0"]).ToString("#0.00");

					if (curPassScore > 0 && (curRow["PathDevBuoy1"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy1"].Value = ((Decimal)curRow["PathDevBuoy1"]).ToString("#0.00");
					if (curPassScore > 0 && (curRow["PathDevcum1"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum1"].Value = ((Decimal)curRow["PathDevcum1"]).ToString("#0.00");

					if (curPassScore > 1 && (curRow["PathDevBuoy2"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy2"].Value = ((Decimal)curRow["PathDevBuoy2"]).ToString("#0.00");
					if (curPassScore > 1 && (curRow["PathDevcum2"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum2"].Value = ((Decimal)curRow["PathDevcum2"]).ToString("#0.00");

					if (curPassScore > 2 && (curRow["PathDevBuoy3"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy3"].Value = ((Decimal)curRow["PathDevBuoy3"]).ToString("#0.00");
					if (curPassScore > 2 && (curRow["PathDevcum3"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum3"].Value = ((Decimal)curRow["PathDevcum3"]).ToString("#0.00");

					if (curPassScore > 3 && (curRow["PathDevBuoy4"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy4"].Value = ((Decimal)curRow["PathDevBuoy4"]).ToString("#0.00");
					if (curPassScore > 3 && (curRow["PathDevcum4"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum4"].Value = ((Decimal)curRow["PathDevcum4"]).ToString("#0.00");

					if (curPassScore > 4 && (curRow["PathDevBuoy5"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy5"].Value = ((Decimal)curRow["PathDevBuoy5"]).ToString("#0.00");
					if (curPassScore > 4 && (curRow["PathDevcum5"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum5"].Value = ((Decimal)curRow["PathDevcum5"]).ToString("#0.00");

					if (curPassScore > 5 && (curRow["PathDevBuoy6"] != System.DBNull.Value)) curViewRow.Cells["PathDevBuoy6"].Value = ((Decimal)curRow["PathDevBuoy6"]).ToString("#0.00");
					if (curPassScore > 5 && (curRow["PathDevcum6"] != System.DBNull.Value)) curViewRow.Cells["PathDevcum6"].Value = ((Decimal)curRow["PathDevcum6"]).ToString("#0.00");

				} else {
					Score.Visible = false;
					PassScore.Visible = false;
					PathDevBuoy4.Visible = false;
					PathDevcum4.Visible = false;
					PathDevBuoy5.Visible = false;
					PathDevcum5.Visible = false;
					PathDevBuoy6.Visible = false;
					PathDevcum6.Visible = false;
					BoatTime.Visible = false;

					curViewRow.Cells["SkierRunNum"].Value = ((Byte)curRow["PassNum"]).ToString();
					if (curRow["ScoreFeet"] != System.DBNull.Value) curViewRow.Cells["ScoreFeet"].Value = ((Decimal)curRow["ScoreFeet"]).ToString("#.00");
					if (curRow["ScoreMeters"] != System.DBNull.Value) curViewRow.Cells["ScoreMeters"].Value = ((Decimal)curRow["ScoreMeters"]).ToString("#.00");

					if (curRow["BoatSplitTime"] != System.DBNull.Value) curViewRow.Cells["BoatSplitTime"].Value = ((Decimal)curRow["BoatSplitTime"]).ToString("#0.00");
					if (curRow["BoatSplitTime2"] != System.DBNull.Value) curViewRow.Cells["BoatSplitTime2"].Value = ((Decimal)curRow["BoatSplitTime2"]).ToString("#0.00");
					if (curRow["BoatEndTime"] != System.DBNull.Value) curViewRow.Cells["BoatEndTime"].Value = ((Decimal)curRow["BoatEndTime"]).ToString("#0.00");

					if (curRow["PathDevBuoy0"] != System.DBNull.Value) curViewRow.Cells["PathDevBuoy0"].Value = ((Decimal)curRow["PathDevBuoy0"]).ToString("#0.00");
					if (curRow["PathDevcum0"] != System.DBNull.Value) curViewRow.Cells["PathDevcum0"].Value = ((Decimal)curRow["PathDevcum0"]).ToString("#0.00");
					if (curRow["PathDevBuoy1"] != System.DBNull.Value) curViewRow.Cells["PathDevBuoy1"].Value = ((Decimal)curRow["PathDevBuoy1"]).ToString("#0.00");
					if (curRow["PathDevcum1"] != System.DBNull.Value) curViewRow.Cells["PathDevcum1"].Value = ((Decimal)curRow["PathDevcum1"]).ToString("#0.00");
					if (curRow["PathDevBuoy2"] != System.DBNull.Value) curViewRow.Cells["PathDevBuoy2"].Value = ((Decimal)curRow["PathDevBuoy2"]).ToString("#0.00");
					if (curRow["PathDevcum2"] != System.DBNull.Value) curViewRow.Cells["PathDevcum2"].Value = ((Decimal)curRow["PathDevcum2"]).ToString("#0.00");
					if (curRow["PathDevBuoy3"] != System.DBNull.Value) curViewRow.Cells["PathDevBuoy3"].Value = ((Decimal)curRow["PathDevBuoy3"]).ToString("#0.00");
					if (curRow["PathDevcum3"] != System.DBNull.Value) curViewRow.Cells["PathDevcum3"].Value = ((Decimal)curRow["PathDevcum3"]).ToString("#0.00");

				}

				if (curRow["ScoreNotes"] != System.DBNull.Value) curViewRow.Cells["ScoreNotes"].Value = (String)curRow["ScoreNotes"];
				curViewRow.Cells["PassDatatime"].Value = ((DateTime)curRow["PassDatatime"]).ToString("yyyy/MM/dd HH:mm:ss");
				curViewRow.Cells["ScoreDatatime"].Value = ( (DateTime) curRow["ScoreDatatime"] ).ToString("yyyy/MM/dd HH:mm:ss");
            }

        }

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
			String filename = "BoatPathSlalomExport.txt";
			if (myEvent.Equals("Jump")) filename = "BoatPathJumpExport.txt";
			myExportData.exportData(dataGridView, filename );
        }

        private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight(15, '*');
            printFooter.PadLeft(15, '*');

			String filename = "BoatPathSlalomExport.htm";
			if (myEvent.Equals("Jump")) filename = "BoatPathJumpExport.htm";
			myExportData.exportDataAsHtml(dataGridView, printTitle, printSubtitle, printFooter, filename );
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getSlalomPassBoatPath() {
            StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append( "SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup, E.EventGroup, E.EventClass, E.RankingScore" );
			curSqlStmt.Append( ", S.Round, COALESCE(V.CodeValue, S.Boat) as Boat, TD.SkierName as Driver, S.Score" );
			curSqlStmt.Append(", R.SkierRunNum, R.BoatTime, R.Score AS PassScore, R.TimeInTol, R.Note AS PassNotes, S.Note as ScoreNotes");
			curSqlStmt.Append(", S.LastUpdateDate as ScoreDatatime, R.InsertDate as PassDatatime");
			curSqlStmt.Append(", BP.PassLineLength, BP.PassSpeedKph");
			curSqlStmt.Append(", BP.PathDevBuoy0, BP.PathDevcum0");
			curSqlStmt.Append(", BP.PathDevBuoy1, BP.PathDevcum1");
			curSqlStmt.Append(", BP.PathDevBuoy2, BP.PathDevcum2");
			curSqlStmt.Append(", BP.PathDevBuoy3, BP.PathDevcum3");
			curSqlStmt.Append(", BP.PathDevBuoy4, BP.PathDevcum4");
			curSqlStmt.Append(", BP.PathDevBuoy5, BP.PathDevcum5");
			curSqlStmt.Append(", BP.PathDevBuoy6, BP.PathDevcum6 ");
			curSqlStmt.Append("FROM SlalomScore S ");
			curSqlStmt.Append("INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round ");
			curSqlStmt.Append("INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append("INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append("LEFT OUTER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round  AND BP.PassNumber = R.SkierRunNum AND BP.Event = E.Event ");
			curSqlStmt.Append("LEFT OUTER JOIN OfficialWorkAsgmt A ON A.SanctionId = S.SanctionId AND A.EventGroup = E.EventGroup AND A.Round = S.Round AND A.Event = E.Event AND A.WorkAsgmt = 'Driver' ");
			curSqlStmt.Append("LEFT OUTER JOIN TourReg TD ON TD.MemberId = A.MemberId AND TD.SanctionId = A.SanctionId ");
			curSqlStmt.Append("LEFT OUTER JOIN CodeValueList V ON V.ListName = 'ApprovedBoats' AND V.ListCode = S.Boat ");
			curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Slalom' " );
			curSqlStmt.Append("ORDER BY S.Round, E.EventGroup, S.AgeGroup, E.RankingScore, T.SkierName, R.SkierRunNum ");

			return DataAccess.getDataTable(curSqlStmt.ToString());
        }

		private DataTable getJumpPassBoatPath() {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup, E.EventGroup, E.EventClass, E.RankingScore");
			curSqlStmt.Append(", S.Round, COALESCE(V.CodeValue, S.Boat) as Boat, TD.SkierName as Driver, R.ScoreFeet, R.ScoreMeters");
			curSqlStmt.Append(", R.PassNum, R.BoatSplitTime, R.BoatSplitTime2, BoatEndTime, R.ScoreFeet as PassScoreFeet, R.ScoreMeters as PassScoreMeters");
			curSqlStmt.Append(", R.TimeInTol, R.Note AS PassNotes, S.Note as ScoreNotes");
			curSqlStmt.Append(", S.LastUpdateDate as ScoreDatatime, R.InsertDate as PassDatatime");
			curSqlStmt.Append(", BP.PassLineLength, BP.PassSpeedKph");
			curSqlStmt.Append(", BP.PathDevBuoy0, BP.PathDevcum0");
			curSqlStmt.Append(", BP.PathDevBuoy1, BP.PathDevcum1");
			curSqlStmt.Append(", BP.PathDevBuoy2, BP.PathDevcum2");
			curSqlStmt.Append(", BP.PathDevBuoy3, BP.PathDevcum3 ");
			curSqlStmt.Append("FROM JumpScore S ");
			curSqlStmt.Append("INNER JOIN JumpRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round ");
			curSqlStmt.Append("INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append("INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append("LEFT OUTER JOIN BoatPath BP ON S.SanctionId = BP.SanctionId AND S.MemberId = BP.MemberId AND BP.Round = R.Round AND BP.PassNumber = R.PassNum AND BP.Event = E.Event ");
			curSqlStmt.Append("LEFT OUTER JOIN OfficialWorkAsgmt A ON A.SanctionId = S.SanctionId AND A.EventGroup = E.EventGroup AND A.Round = S.Round AND A.Event = E.Event AND A.WorkAsgmt = 'Driver' ");
			curSqlStmt.Append("LEFT OUTER JOIN TourReg TD ON TD.MemberId = A.MemberId AND TD.SanctionId = A.SanctionId ");
			curSqlStmt.Append("LEFT OUTER JOIN CodeValueList V ON V.ListName = 'ApprovedBoats' AND V.ListCode = S.Boat ");
			curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Jump' ");
			curSqlStmt.Append("ORDER BY S.Round, E.EventGroup, S.AgeGroup, E.RankingScore, T.SkierName, R.PassNum ");

			return DataAccess.getDataTable(curSqlStmt.ToString());
		}

		private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation");
            curSqlStmt.Append(", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' ");
            return DataAccess.getDataTable(curSqlStmt.ToString());
        }
	}
}