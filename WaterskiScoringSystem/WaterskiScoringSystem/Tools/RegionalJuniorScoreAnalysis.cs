using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tools {
	public partial class RegionalJuniorScoreAnalysis : Form {
		private String mySanctionNum;
		private String mySanctionNumPrev;
		private String myExportFileNameDefault;

		private DataRow myTourRow;

		private DataTable myDataTable;

		public RegionalJuniorScoreAnalysis() {
			InitializeComponent();
		}

		private void RegionalJuniorScoreAnalysis_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.RegionJuniorInfo_Width > 0 ) {
				this.Width = Properties.Settings.Default.RegionJuniorInfo_Width;
			}
			if ( Properties.Settings.Default.RegionJuniorInfo_Height > 0 ) {
				this.Height = Properties.Settings.Default.RegionJuniorInfo_Height;
			}
			if ( Properties.Settings.Default.RegionJuniorInfo_Location.X > 0
				&& Properties.Settings.Default.RegionJuniorInfo_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.RegionJuniorInfo_Location;
			}

			this.dataGridView.Visible = false;
            MessageLabel.Text = "";
			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				//this.Close();

			} else if ( mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				//this.Close();

			} else {
				//Retrieve selected tournament attributes
				DataTable curTourDataTable = getTourData();
				myTourRow = curTourDataTable.Rows[0];

				int curSkiYear = int.Parse( this.mySanctionNum.Substring( 0, 2 ) );
				this.mySanctionNumPrev = (curSkiYear - 1).ToString() + this.mySanctionNum.Substring( 2, 4 );
			}
		}

		private void RegionalJuniorScoreAnalysis_FormClosing( object sender, FormClosingEventArgs e ) {

		}
		private void RegionalJuniorScoreAnalysis_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.RegionJuniorInfo_Width = this.Size.Width;
				Properties.Settings.Default.RegionJuniorInfo_Height = this.Size.Height;
				Properties.Settings.Default.RegionJuniorInfo_Location = this.Location;
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
				DataGridView view = (DataGridView) sender;
				view.Rows[e.RowIndex].ErrorText = "an error";
				view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

				e.ThrowException = false;
			}
		}

		private void ExecAvgSameDivButton_Click( object sender, EventArgs e ) {
			myDataTable = getRankingAvgSameDiv();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
                this.MessageLabel.Text = "SQL command completed, 0 rows processed";
			} else {
				myExportFileNameDefault = "JuniorAverageSameDiv";
				this.dataGridView.DataSource = myDataTable;
				this.dataGridView.Visible = true;
				this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
			}
		}

		private void ExecAvgDiffDivButton_Click( object sender, EventArgs e ) {
			myDataTable = getRankingAvgNewDiv();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
			} else {
				myExportFileNameDefault = "JuniorAverageNewDiv";
				this.dataGridView.DataSource = myDataTable;
				this.dataGridView.Visible = true;
				this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
			}
		}

		private void ExecAvgCurYearOnlyButton_Click( object sender, EventArgs e ) {
			myDataTable = getRankingAvgOnlyCurYear();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
			} else {
				myExportFileNameDefault = "JuniorAverageCurrentYearOnly";
				this.dataGridView.DataSource = myDataTable;
				this.dataGridView.Visible = true;
				this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
			}
		}

		private void ExecAvgMostImprovedButton_Click( object sender, EventArgs e ) {
			myDataTable = getMostImprovedSameDiv();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
			} else {
				myExportFileNameDefault = "JuniorAverageImprovedSameDiv";
				this.dataGridView.DataSource = myDataTable;
				this.dataGridView.Visible = true;
				this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
			}
		}

		private void ExecAvgMostImprovedNewDivButton_Click( object sender, EventArgs e ) {
			myDataTable = getMostImprovedNewDiv();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
			} else {
				myExportFileNameDefault = "JuniorAverageImprovedNewDiv";
				this.dataGridView.DataSource = myDataTable;
				this.dataGridView.Visible = true;
				this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
			}
		}

		private void ExportButton_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			myExportData.exportData( dataGridView, myExportFileNameDefault + ".txt" );
		}

		private DataTable getRankingAvgSameDiv() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
            curSqlStmt.Append( "       AND E2.MemberId = E.MemberId AND E2.AgeGroup = E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getRankingAvgNewDiv() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "       AND E2.MemberId = E.MemberId AND E2.AgeGroup != E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getRankingAvgOnlyCurYear() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Left Outer Join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "       AND E2.MemberId = E.MemberId AND E2.Event = E.Event " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "And E2.AgeGroup is null " );
			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getMostImprovedSameDiv() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, S1.Score as CurYearScore, S2.Score as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "       AND E2.MemberId = E.MemberId AND E2.AgeGroup = E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join SlalomScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join SlalomScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Slalom' AND E2.Event = 'Slalom' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, CONVERT(numeric(6,1), S1.Score) as CurYearScore, CONVERT(numeric(6,1), S2.Score) as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "      AND E2.MemberId = E.MemberId AND E2.AgeGroup = E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join TrickScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join TrickScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E2.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Trick' AND E2.Event = 'Trick' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, S1.ScoreFeet as CurYearScore, S2.ScoreFeet as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "      AND E2.MemberId = E.MemberId AND E2.AgeGroup = E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join JumpScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join JumpScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E2.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Jump' AND E2.Event = 'Jump' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );

			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getMostImprovedNewDiv() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, S1.Score as CurYearScore, S2.Score as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "       AND E2.MemberId = E.MemberId AND E2.AgeGroup != E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join SlalomScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join SlalomScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Slalom' AND E2.Event = 'Slalom' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, CONVERT(numeric(6,1), S1.Score) as CurYearScore, CONVERT(numeric(6,1), S2.Score) as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "      AND E2.MemberId = E.MemberId AND E2.AgeGroup != E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join TrickScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join TrickScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E2.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Trick' AND E2.Event = 'Trick' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup, S1.ScoreFeet as CurYearScore, S2.ScoreFeet as PrevYearScore " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "      AND E2.MemberId = E.MemberId AND E2.AgeGroup != E.AgeGroup AND E2.Event = E.Event " );
			curSqlStmt.Append( "Inner join JumpScore S1 on S1.SanctionId = '" + this.mySanctionNum + "' AND S1.MemberId = E.MemberId AND S1.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join JumpScore S2 on S2.SanctionId = '" + this.mySanctionNumPrev + "' AND S2.MemberId = E2.MemberId AND S2.AgeGroup = E2.AgeGroup " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' " );
			curSqlStmt.Append( "AND E.Event = 'Jump' AND E2.Event = 'Jump' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );

			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
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
}
