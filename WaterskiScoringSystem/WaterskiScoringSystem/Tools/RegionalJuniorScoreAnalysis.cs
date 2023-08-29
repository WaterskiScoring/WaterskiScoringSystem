using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;

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
				if ( this.mySanctionNum.Substring( 3, 3 ).Equals( "999" ) ) {
					this.mySanctionNumPrev = ( curSkiYear - 1 ).ToString() + "S" + this.mySanctionNum.Substring( 3, 3 );
					if ( isTourExist( this.mySanctionNumPrev ) ) return;
					this.mySanctionNumPrev = ( curSkiYear - 1 ).ToString() + "M" + this.mySanctionNum.Substring( 3, 3 );
					if ( isTourExist( this.mySanctionNumPrev ) ) return;
					this.mySanctionNumPrev = ( curSkiYear - 1 ).ToString() + "C" + this.mySanctionNum.Substring( 3, 3 );
					if ( isTourExist( this.mySanctionNumPrev ) ) return;
					this.mySanctionNumPrev = ( curSkiYear - 1 ).ToString() + "W" + this.mySanctionNum.Substring( 3, 3 );
					if ( isTourExist( this.mySanctionNumPrev ) ) return;
					this.mySanctionNumPrev = ( curSkiYear - 1 ).ToString() + "E" + this.mySanctionNum.Substring( 3, 3 );
					if ( isTourExist( this.mySanctionNumPrev ) ) return;
					this.mySanctionNumPrev = "";
				}
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

		private void ExecAvgCurYearOnlyButton_Click( object sender, EventArgs e ) {
			myDataTable = getRankingAvgOnlyCur();
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
			CalcNops appNopsCalc;

			myDataTable = getMostImproved();
			if ( myDataTable == null ) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
				return;
			}

			ScoreEntry scoreEntry = new Common.ScoreEntry();
			appNopsCalc = CalcNops.Instance;
			appNopsCalc.LoadDataForTour();

			bool isSlalomActive = true, isTrickActive = true, isJumpActive = true;
			Decimal CurRankAvgNopsSlalom = 0.0M
				, PrevRankAvgNopsSlalom = 0.0M
				, CurRankAvgNopsTrick = 0.0M
				, PrevRankAvgNopsTrick = 0.0M
				, CurRankAvgNopsJump = 0.0M
				, PrevRankAvgNopsJump = 0.0M
				, CurOverall = 0.0M
				, PrevOverall = 0.0M
				, CurRankAvgOverall = 0.0M
				, PrevRankAvgOverall = 0.0M
				;

			foreach ( DataRow curDataRow in myDataTable.Rows ) {
				isSlalomActive = false;
				isTrickActive = false;
				isJumpActive = false;

				CurRankAvgNopsSlalom = 0.0M;
				PrevRankAvgNopsSlalom = 0.0M;
				
				CurRankAvgNopsTrick = 0.0M;
				PrevRankAvgNopsTrick = 0.0M;
				
				CurRankAvgNopsJump = 0.0M;
				PrevRankAvgNopsJump = 0.0M;
				
				CurOverall = 0.0M;
				PrevOverall = 0.0M;
				CurRankAvgOverall = 0.0M;
				PrevRankAvgOverall = 0.0M;

				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "SlalomEvent", "" ) ) 
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "SlalomEventPrev", "" ) )
					) {
					isSlalomActive = true;
					scoreEntry.Event = HelperFunctions.getDataRowColValue( curDataRow, "SlalomEvent", "" );
					scoreEntry.Score = ( Decimal)curDataRow["CurRankAvgSlalom"];
					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["CurRankAvgNopsSlalom"] = scoreEntry.Nops;
					CurRankAvgNopsSlalom = scoreEntry.Nops;

					scoreEntry.Score = (Decimal)curDataRow["PrevRankAvgSlalom"];
					appNopsCalc.calcNops( (String)curDataRow["PrevAgeGroup"], scoreEntry );
					curDataRow["PrevRankAvgNopsSlalom"] = scoreEntry.Nops;
					PrevRankAvgNopsSlalom = scoreEntry.Nops;

					curDataRow["DiffRankAvgNopsSlalom"] = CurRankAvgNopsSlalom - PrevRankAvgNopsSlalom;
				}

				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "TrickEvent", "" ) )
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "TrickEventPrev", "" ) )
					) {
					isTrickActive = true;
					scoreEntry.Event = HelperFunctions.getDataRowColValue( curDataRow, "TrickEvent", "" );
					scoreEntry.Score = (Decimal)curDataRow["CurRankAvgTrick"];
					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["CurRankAvgNopsTrick"] = scoreEntry.Nops;
					CurRankAvgNopsTrick = scoreEntry.Nops;

					scoreEntry.Score = (Decimal)curDataRow["PrevRankAvgTrick"];
					appNopsCalc.calcNops( (String)curDataRow["PrevAgeGroup"], scoreEntry );
					curDataRow["PrevRankAvgNopsTrick"] = scoreEntry.Nops;
					PrevRankAvgNopsTrick = scoreEntry.Nops;

					curDataRow["DiffRankAvgNopsTrick"] = CurRankAvgNopsTrick - PrevRankAvgNopsTrick;
				}

				if ( HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "JumpEvent", "" ) )
					&& HelperFunctions.isObjectPopulated( HelperFunctions.getDataRowColValue( curDataRow, "JumpEventPrev", "" ) )
					) {
					isJumpActive = true;
					scoreEntry.Event = HelperFunctions.getDataRowColValue( curDataRow, "JumpEvent", "" );
					scoreEntry.Score = (Decimal)curDataRow["CurRankAvgJump"];
					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["CurRankAvgNopsJump"] = scoreEntry.Nops;
					CurRankAvgNopsJump = scoreEntry.Nops;

					scoreEntry.Score = (Decimal)curDataRow["PrevRankAvgJump"];
					appNopsCalc.calcNops( (String)curDataRow["PrevAgeGroup"], scoreEntry );
					curDataRow["PrevRankAvgNopsJump"] = scoreEntry.Nops;
					PrevRankAvgNopsJump = scoreEntry.Nops;

					curDataRow["DiffRankAvgNopsJump"] = CurRankAvgNopsJump - PrevRankAvgNopsJump;
				}

				if ( HelperFunctions.getDataRowColValue( curDataRow, "AgGroupNum", "" ).Equals("1" ) 
					&& isSlalomActive && isTrickActive ) {
					CurOverall = (Decimal)curDataRow["CurNopsSlalom"] + (Decimal)curDataRow["CurNopsTrick"];
					PrevOverall = (Decimal)curDataRow["PrevNopsSlalom"] + (Decimal)curDataRow["PrevNopsTrick"];
					CurRankAvgOverall = CurRankAvgNopsSlalom + CurRankAvgNopsTrick;
					PrevRankAvgOverall = PrevRankAvgNopsSlalom + PrevRankAvgNopsTrick;

				} else if ( isSlalomActive && isTrickActive && isJumpActive ) {
					CurOverall = (Decimal)curDataRow["CurNopsSlalom"] + (Decimal)curDataRow["CurNopsTrick"] + (Decimal)curDataRow["CurNopsJump"];
					PrevOverall = (Decimal)curDataRow["PrevNopsSlalom"] + (Decimal)curDataRow["PrevNopsTrick"] + (Decimal)curDataRow["PrevNopsJump"];
					CurRankAvgOverall = CurRankAvgNopsSlalom + CurRankAvgNopsTrick + CurRankAvgNopsJump;
					PrevRankAvgOverall = PrevRankAvgNopsSlalom + PrevRankAvgNopsTrick + PrevRankAvgNopsJump;
				}
				curDataRow["CurOverall"] = CurOverall;
				curDataRow["PrevOverall"] = PrevOverall;
				curDataRow["DiffOverall"] = CurOverall - PrevOverall;
				curDataRow["CurRankAvgOverall"] = CurRankAvgOverall;
				curDataRow["PrevRankAvgOverall"] = PrevRankAvgOverall;
				curDataRow["DiffRankAvgOverall"] = CurRankAvgOverall - PrevRankAvgOverall;

			}

			myExportFileNameDefault = "JuniorMostImproved";
			this.dataGridView.DataSource = myDataTable;
			this.dataGridView.Visible = true;
			this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );

			MessageBox.Show( "Special note regarding overall"
				+ "\n\nThe rank overall for purposes of this report were calculated using the current year NOPS facotrs applied to the "
				+ "skiers slalom, trick, and jump ranking average scores for the current year and the previous year.  "
				+ "This may not match with the overall ranking average found on the ranking list "
				+ "because the overall ranking average is not a value that is downloaded to WSTIMS. "
				+ "Therefore you will need to consult the Ranking List to compare the actual overall ranking averages."
				);


		}

		private void ExecAllStarCalcButton_Click(object sender, EventArgs e) {
			CalcNops appNopsCalc;

			myDataTable = getJuniorAllStarsData();
			if (myDataTable == null) {
				this.dataGridView.DataSource = new DataTable();
				this.MessageLabel.Text = "SQL command completed, 0 rows processed";
				return;
			}

			ScoreEntry scoreEntry = new Common.ScoreEntry();
			appNopsCalc = CalcNops.Instance;
			appNopsCalc.LoadDataForTour();

			// curSqlStmt.Append(", 0.0 as RankingNopsTotal, 0.0 as NopsTotal, , 0.0 as RankingNopsTotal60Pct, 0.0 as NopsTotal40Pct, 0.0 as ScoreAllStar");
			Decimal curRankingNopsTotal = 0.0M, curNopsTotal = 0.0M, curRankingNopsTotal60Pct = 0.0M, curNopsTotal40Pct = 0.0M, curScoreAllStar = 0.0M;
			foreach ( DataRow curDataRow in myDataTable.Rows ) {
				curRankingNopsTotal = 0.0M;
				curNopsTotal = 0.0M;
				curRankingNopsTotal60Pct = 0.0M;
				curNopsTotal40Pct = 0.0M;
				curScoreAllStar = 0.0M;

				if ( curDataRow["EventSlalom"] != System.DBNull.Value && curDataRow["ScoreSlalom"] != System.DBNull.Value ) {
					scoreEntry.Event = (String)curDataRow["EventSlalom"];
					scoreEntry.Score = (Decimal)curDataRow["RankingScoreSlalom"];

					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["RankingNopsSlalom"] = scoreEntry.Nops;

					curRankingNopsTotal += scoreEntry.Nops;
					curNopsTotal += (Decimal)curDataRow["NopsSlalom"];
				}

				if ( curDataRow["EventTrick"] != System.DBNull.Value && curDataRow["ScoreTrick"] != System.DBNull.Value ) {
					scoreEntry.Event = (String)curDataRow["EventTrick"];
					scoreEntry.Score = (Decimal)curDataRow["RankingScoreTrick"];

					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["RankingNopsTrick"] = scoreEntry.Nops;

					curRankingNopsTotal += scoreEntry.Nops;
					curNopsTotal += (Decimal)curDataRow["NopsTrick"];
				}

				if ( curDataRow["EventJump"] != System.DBNull.Value && curDataRow["ScoreJump"] != System.DBNull.Value ) {
					scoreEntry.Event = (String)curDataRow["EventJump"];
					scoreEntry.Score = (Decimal)curDataRow["RankingScoreJump"];

					appNopsCalc.calcNops( (String)curDataRow["AgeGroup"], scoreEntry );
					curDataRow["RankingNopsJump"] = scoreEntry.Nops;

					curRankingNopsTotal += scoreEntry.Nops;
					curNopsTotal += (Decimal)curDataRow["NopsJump"];
				}

				curDataRow["RankingNopsTotal"] = curRankingNopsTotal;
				curDataRow["NopsTotal"] = curNopsTotal;

				curRankingNopsTotal60Pct = Math.Round( curRankingNopsTotal * .60M );
				curNopsTotal40Pct = Math.Round( curNopsTotal * .40M );
				curScoreAllStar = curRankingNopsTotal60Pct + curNopsTotal40Pct;

				curDataRow["RankingNopsTotal60Pct"] = curRankingNopsTotal60Pct;
				curDataRow["NopsTotal40Pct"] = curNopsTotal40Pct;
				curDataRow["ScoreAllStar"] = curScoreAllStar;
			}

			myExportFileNameDefault = "JuniorAllStars";
			this.dataGridView.DataSource = myDataTable;
			this.dataGridView.Visible = true;
			this.MessageLabel.Text = String.Format( "SQL command completed, {0} rows processed", myDataTable.Rows.Count );
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

		private DataTable getRankingAvgOnlyCur() {
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

		private DataTable getMostImproved() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "" );
			curSqlStmt.Append( "SELECT TR.SkierName, TR.AgeGroup, TR2.AgeGroup AS PrevAgeGroup, SUBSTRING( TR.AgeGroup, 2, 1 ) AS AgGroupNum " );

			curSqlStmt.Append( ", ERS.Event AS SlalomEvent, ERS2.Event AS SlalomEventPrev  " );
			curSqlStmt.Append( ", COALESCE(ERS.RankingScore, 0.0 ) as CurRankAvgSlalom " );
			curSqlStmt.Append( ", COALESCE( ERS2.RankingScore, 0.0 ) as PrevRankAvgSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( S1.Score, 0.0 ) > 0 THEN S1.Score ELSE 0.0 END AS CurScoreSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( S1.NopsScore, 0.0 ) > 0 THEN S1.NopsScore ELSE 0.0 END AS CurNopsSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( S2.Score, 0.0 ) > 0 THEN S2.Score ELSE 0.0 END AS PrevScoreSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( S2.NopsScore, 0.0 ) > 0 THEN S2.NopsScore ELSE 0.0 END AS PrevNopsSlalom " );
			curSqlStmt.Append( ", 0.0 as CurRankAvgNopsSlalom, 0.0 as PrevRankAvgNopsSlalom " );

			curSqlStmt.Append( ", CASE WHEN COALESCE( S1.Score, 0.0 ) > 0 AND COALESCE( S2.Score, 0.0 ) > 0 THEN S1.Score - S2.Score ELSE 0.0 END AS DiffScoreSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( S1.Score, 0.0 ) > 0 AND COALESCE( S2.Score, 0.0 ) > 0 THEN S1.NopsScore - S2.NopsScore ELSE 0.0 END AS DiffNopsSlalom " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( ERS.RankingScore, 0.0 ) > 0 AND COALESCE( ERS2.RankingScore, 0.0 ) > 0 THEN ERS.RankingScore - ERS2.RankingScore ELSE 0.0 END AS DiffRankAvgSlalom " );
			curSqlStmt.Append( ", 0.0 as DiffRankAvgNopsSlalom " );

			curSqlStmt.Append( ", ERT.Event AS TrickEvent, ERT2.Event AS TrickEventPrev " );
			curSqlStmt.Append( ", COALESCE( ERT.RankingScore, 0.0 ) as CurRankAvgTrick " );
			curSqlStmt.Append( ", COALESCE( ERT2.RankingScore, 0.0 ) as PrevRankAvgTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( T1.Score, 0.0 ) > 0 THEN T1.Score ELSE 0 END AS CurScoreTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( T1.NopsScore, 0.0 ) > 0 THEN T1.NopsScore ELSE 0.0 END AS CurNopsTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( T2.Score, 0.0 ) > 0 THEN T2.Score ELSE 0 END AS PrevScoreTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( T2.NopsScore, 0.0 ) > 0 THEN T2.NopsScore ELSE 0.0 END AS PrevNopsTrick " );
			curSqlStmt.Append( ", 0.0 as CurRankAvgNopsTrick, 0.0 as PrevRankAvgNopsTrick " );

			curSqlStmt.Append( ", CASE WHEN COALESCE( T1.Score, 0 ) > 0 AND COALESCE( T2.Score, 0 ) > 0 THEN T1.Score - T2.Score ELSE 0 END AS DiffScoreTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( T1.Score, 0 ) > 0 AND COALESCE( T2.Score, 0 ) > 0 THEN T1.NopsScore - T2.NopsScore ELSE 0 END AS DiffNopsTrick " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( ERT.RankingScore, 0.0 ) > 0 AND COALESCE( ERT2.RankingScore, 0.0 ) > 0 THEN ERT.RankingScore - ERT2.RankingScore ELSE 0.0 END AS DiffRankAvgTrick " );
			curSqlStmt.Append( ", 0.0 as DiffRankAvgNopsTrick " );

			curSqlStmt.Append( ", ERJ.Event AS JumpEvent, ERJ2.Event AS JumpEventPrev " );
			curSqlStmt.Append( ", COALESCE( ERJ.RankingScore, 0.0 ) as CurRankAvgJump " );
			curSqlStmt.Append( ", COALESCE( ERJ2.RankingScore, 0.0 ) as PrevRankAvgJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( J1.ScoreFeet, 0.0 ) > 0.0 THEN J1.ScoreFeet ELSE 0 END AS CurScoreJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( J1.NopsScore, 0.0 ) > 0.0 THEN J1.NopsScore ELSE 0.0 END AS CurNopsJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( J2.ScoreFeet, 0.0 ) > 0.0 THEN J2.ScoreFeet ELSE 0 END AS PrevScoreJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( J2.NopsScore, 0.0 ) > 0.0 THEN J2.NopsScore ELSE 0.0 END AS PrevNopsJump " );
			curSqlStmt.Append( ", 0.0 as CurRankAvgNopsJump, 0.0 as PrevRankAvgNopsJump " );

			curSqlStmt.Append( ", CASE WHEN COALESCE( J1.ScoreFeet, 0.0 ) > 0.0 AND COALESCE( J2.ScoreFeet, 0.0 ) > 0 THEN J1.ScoreFeet - J2.ScoreFeet ELSE 0.0 END AS DiffScoreJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( J1.ScoreFeet, 0.0 ) > 0.0 AND COALESCE( J2.ScoreFeet, 0.0 ) > 0 THEN J1.NopsScore - J2.NopsScore ELSE 0.0 END AS DiffNopsJump " );
			curSqlStmt.Append( ", CASE WHEN COALESCE( ERJ.RankingScore, 0.0 ) > 0 AND COALESCE( ERJ2.RankingScore, 0.0 ) > 0 THEN ERJ.RankingScore - ERJ2.RankingScore ELSE 0.0 END AS DiffRankAvgJump " );
			curSqlStmt.Append( ", 0.0 as DiffRankAvgNopsJump " );

			curSqlStmt.Append( ", 0.0 as CurOverall, 0.0 as PrevOverall, 0.0 as DiffOverall, 0.0 as CurRankAvgOverall, 0.0 as PrevRankAvgOverall, 0.0 as DiffRankAvgOverall " );

			curSqlStmt.Append( "From TourReg TR " );
			curSqlStmt.Append( "Inner Join TourReg TR2 ON TR2.MemberId = TR.MemberId " );

			curSqlStmt.Append( "Left Outer Join EventReg ERS ON ERS.SanctionId = TR.SanctionId AND ERS.MemberId = TR.MemberId AND ERS.AgeGroup = TR.AgeGroup AND ERS.Event = 'Slalom' " );
			curSqlStmt.Append( "Left Outer join EventReg ERS2 ON ERS2.SanctionId = TR2.SanctionId AND ERS2.MemberId = TR2.MemberId AND ERS2.MemberId = TR.MemberId AND ERS2.AgeGroup = TR2.AgeGroup AND ERS2.Event = 'Slalom' " );
			curSqlStmt.Append( "Left Outer Join SlalomScore S1 ON S1.SanctionId = ERS.SanctionId AND S1.MemberId = ERS.MemberId AND S1.AgeGroup = ERS.AgeGroup AND S1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join SlalomScore S2 ON S2.SanctionId = ERS2.SanctionId AND S2.MemberId = ERS2.MemberId AND S2.MemberId = TR.MemberId AND S2.AgeGroup = ERS2.AgeGroup AND S2.Round < 25 " );

			curSqlStmt.Append( "Left Outer Join EventReg ERT ON ERT.SanctionId = TR.SanctionId AND ERT.MemberId = TR.MemberId AND ERT.AgeGroup = TR.AgeGroup AND ERT.Event = 'Trick' " );
			curSqlStmt.Append( "Left Outer join EventReg ERT2 ON ERT2.SanctionId = TR2.SanctionId AND ERT2.MemberId = TR2.MemberId AND ERT2.MemberId = TR.MemberId AND ERT2.AgeGroup = TR2.AgeGroup AND ERT2.Event = 'Trick' " );
			curSqlStmt.Append( "Left Outer Join TrickScore T1 ON T1.SanctionId = ERT.SanctionId AND T1.MemberId = ERT.MemberId AND T1.AgeGroup = ERT.AgeGroup AND T1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join TrickScore T2 ON T2.SanctionId = ERT2.SanctionId AND T2.MemberId = ERT2.MemberId AND T2.MemberId = TR2.MemberId AND T2.AgeGroup = ERT2.AgeGroup AND T2.Round < 25 " );

			curSqlStmt.Append( "Left Outer Join EventReg ERJ ON ERJ.SanctionId = TR.SanctionId AND ERJ.MemberId = TR.MemberId AND ERJ.AgeGroup = TR.AgeGroup AND ERJ.Event = 'Jump' " );
			curSqlStmt.Append( "Left Outer join EventReg ERJ2 ON ERJ2.SanctionId = TR2.SanctionId AND ERJ2.MemberId = TR2.MemberId AND ERJ2.MemberId = TR.MemberId AND ERJ2.AgeGroup = TR2.AgeGroup AND ERJ2.Event = 'Jump' " );
			curSqlStmt.Append( "Left Outer Join JumpScore J1 ON J1.SanctionId = ERJ.SanctionId AND J1.MemberId = ERJ.MemberId AND J1.AgeGroup = ERJ.AgeGroup AND J1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join JumpScore J2 ON J2.SanctionId = ERJ2.SanctionId AND J2.MemberId = ERJ2.MemberId AND J2.MemberId = TR2.MemberId AND J2.AgeGroup = ERJ2.AgeGroup AND J2.Round < 25 " );

			curSqlStmt.Append( String.Format( "Where TR.SanctionId = '{0}' AND TR2.SanctionId = '{1}' ", this.mySanctionNum, this.mySanctionNumPrev ) );
			curSqlStmt.Append( "AND SUBSTRING( TR.AgeGroup, 1, 1) in ('B', 'G') AND SUBSTRING( TR2.AgeGroup, 1, 1) in ('B', 'G') " );

			curSqlStmt.Append( "Order by SUBSTRING(TR.AgeGroup, 2, 1), TR.AgeGroup, TR.SkierName " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getMostImprovedBak() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup" );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S1.Score, 0 ) > 0 THEN S1.Score ELSE 0 END AS CurScore " );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S2.Score, 0 ) > 0 THEN S2.Score ELSE 0 END AS PrevYearScore " );
			curSqlStmt.Append( ", (E.RankingScore - E2.RankingScore) as RankingScoreChange" );
			curSqlStmt.Append( ", CASE WHEN E2.RankingScore > 0 THEN CONVERT(numeric(7,2), (((E.RankingScore - E2.RankingScore) / E2.RankingScore) * 100)) ELSE 0 END  as RankingScorePctChange " );
			curSqlStmt.Append( ", (S1.Score - S2.Score) as ScoreChange" );
			curSqlStmt.Append( ", CASE WHEN S2.Score > 0 THEN CONVERT(numeric(7,2), (((S1.Score - S2.Score) / S2.Score) * 100)) ELSE 0 END as ScorePctChange " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.MemberId = E.MemberId AND E2.Event = E.Event " );
			curSqlStmt.Append( "Left Outer Join SlalomScore S1 on S1.SanctionId = T.SanctionId AND S1.MemberId = E.MemberId AND S1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join SlalomScore S2 on S2.SanctionId = E2.SanctionId AND S2.MemberId = E.MemberId AND S2.Round < 25 " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' AND E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "AND E.Event = 'Slalom' AND E2.Event = 'Slalom' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup" );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S1.Score, 0 ) > 0 THEN S1.Score ELSE 0 END AS CurScore " );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S2.Score, 0 ) > 0 THEN S2.Score ELSE 0 END AS PrevYearScore " );
			curSqlStmt.Append( ", (E.RankingScore - E2.RankingScore) as RankingScoreChange" );
			curSqlStmt.Append( ", CASE WHEN E2.RankingScore > 0 THEN CONVERT(numeric(7,2), (((E.RankingScore - E2.RankingScore) / E2.RankingScore) * 100)) ELSE 0 END  as RankingScorePctChange " );
			curSqlStmt.Append( ", (S1.Score - S2.Score) as ScoreChange" );
			curSqlStmt.Append( ", CASE WHEN S2.Score > 0 THEN CONVERT(numeric(7,2), (((S1.Score - S2.Score) / S2.Score) * 100)) ELSE 0 END as ScorePctChange " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.MemberId = E.MemberId AND E2.Event = E.Event " );
			curSqlStmt.Append( "Left Outer Join TrickScore S1 on S1.SanctionId = T.SanctionId AND S1.MemberId = E.MemberId AND S1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join TrickScore S2 on S2.SanctionId = E2.SanctionId AND S2.MemberId = E2.MemberId AND S2.Round < 25 " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' AND E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "AND E.Event = 'Trick' AND E2.Event = 'Trick' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );
			curSqlStmt.Append( "UNION " );

			curSqlStmt.Append( "SELECT E.Event, SkierName, T.AgeGroup, E.RankingScore as CurRankingScore, E2.RankingScore as PrevRankingScore, E2.AgeGroup as PrevAgeGroup" );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S1.ScoreFeet, 0 ) > 0 THEN S1.ScoreFeet ELSE 0 END AS CurScore " );
			curSqlStmt.Append( ", CASE WHEN COALESCE(S2.ScoreFeet, 0 ) > 0 THEN S2.ScoreFeet ELSE 0 END AS PrevYearScore " );
			curSqlStmt.Append( ", (E.RankingScore - E2.RankingScore) as RankingScoreChange" );
			curSqlStmt.Append( ", CASE WHEN E2.RankingScore > 0 THEN CONVERT(numeric(7,2), (((E.RankingScore - E2.RankingScore) / E2.RankingScore) * 100)) ELSE 0 END  as RankingScorePctChange " );
			curSqlStmt.Append( ", (S1.ScoreFeet - S2.ScoreFeet) as ScoreChange" );
			curSqlStmt.Append( ", CASE WHEN S2.ScoreFeet > 0 THEN CONVERT(numeric(7,2), (((S1.ScoreFeet - S2.ScoreFeet) / S2.ScoreFeet) * 100)) ELSE 0 END as ScorePctChange " );
			curSqlStmt.Append( "FROM EventReg E " );
			curSqlStmt.Append( "Inner join TourReg T on T.SanctionId = E.SanctionId and T.MemberId = E.MemberId AND T.AgeGroup = E.AgeGroup " );
			curSqlStmt.Append( "Inner join EventReg E2 on E2.MemberId = E.MemberId AND E2.Event = E.Event " );
			curSqlStmt.Append( "Left Outer Join JumpScore S1 on S1.SanctionId = T.SanctionId AND S1.MemberId = E.MemberId AND S1.Round < 25 " );
			curSqlStmt.Append( "Left Outer Join JumpScore S2 on S2.SanctionId = E2.SanctionId AND S2.MemberId = E2.MemberId AND S2.Round < 25 " );
			curSqlStmt.Append( "Where T.SanctionId = '" + this.mySanctionNum + "' AND E2.SanctionId = '" + this.mySanctionNumPrev + "' " );
			curSqlStmt.Append( "AND E.Event = 'Jump' AND E2.Event = 'Jump' " );
			curSqlStmt.Append( "And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') " );

			curSqlStmt.Append( "Order by E.Event, T.AgeGroup, SkierName " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

		private DataTable getJuniorAllStarsData() {
			StringBuilder curSqlStmt = new StringBuilder("");
			curSqlStmt.Append("SELECT SkierName, T.MemberId, T.AgeGroup ");
			curSqlStmt.Append(", ES.Event as EventSlalom, ES.RankingScore as RankingScoreSlalom, 0.0 as RankingNopsSlalom, SS.Score as ScoreSlalom, SS.NopsScore as NopsSlalom ");
			curSqlStmt.Append(", ET.Event as EventTrick, ET.RankingScore as RankingScoreTrick, 0.0 as RankingNopsTrick, CONVERT(numeric(6, 1), ST.Score) as ScoreTrick, ST.NopsScore as NopsTrick ");
			curSqlStmt.Append(", EJ.Event as EventJump, EJ.RankingScore as RankingScoreJump, 0.0 as RankingNopsJump, SJ.ScoreFeet as ScoreJump, SJ.NopsScore as NopsJump ");
			curSqlStmt.Append(", 0.0 as RankingNopsTotal, 0.0 as NopsTotal, 0.0 as RankingNopsTotal60Pct, 0.0 as NopsTotal40Pct, 0.0 as ScoreAllStar ");
			curSqlStmt.Append("FROM TourReg T ");
			
			curSqlStmt.Append("LEFT OUTER JOIN EventReg ES on T.SanctionId = ES.SanctionId and T.MemberId = ES.MemberId AND T.AgeGroup = ES.AgeGroup AND ES.Event = 'Slalom' ");
			curSqlStmt.Append("LEFT OUTER JOIN SlalomScore SS on SS.SanctionId = T.SanctionId and SS.MemberId = T.MemberId AND SS.AgeGroup = T.AgeGroup And SS.Round = 1 ");

			curSqlStmt.Append("LEFT OUTER JOIN EventReg ET on T.SanctionId = ET.SanctionId and T.MemberId = ET.MemberId AND T.AgeGroup = ET.AgeGroup AND ET.Event = 'Trick' ");
			curSqlStmt.Append("LEFT OUTER JOIN TrickScore ST on ST.SanctionId = T.SanctionId and ST.MemberId = T.MemberId AND ST.AgeGroup = T.AgeGroup And ST.Round = 1 ");

			curSqlStmt.Append("LEFT OUTER JOIN EventReg EJ on T.SanctionId = EJ.SanctionId and T.MemberId = EJ.MemberId AND T.AgeGroup = EJ.AgeGroup AND EJ.Event = 'Jump' ");
			curSqlStmt.Append("LEFT OUTER JOIN JumpScore SJ on SJ.SanctionId = T.SanctionId and SJ.MemberId = T.MemberId AND SJ.AgeGroup = T.AgeGroup And SJ.Round = 1 ");

			curSqlStmt.Append("WHERE T.SanctionId = '" + this.mySanctionNum + "' ");
			curSqlStmt.Append("And SUBSTRING(T.AgeGroup, 1, 1) in ('B', 'G') ");
			curSqlStmt.Append("AND T.AgeGroup not in ('B1', 'G1' ) ");

			curSqlStmt.Append("ORDER BY T.AgeGroup, SkierName, T.MemberId ");
			return DataAccess.getDataTable(curSqlStmt.ToString());
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

		private bool isTourExist( String inStanctionId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, Name, Class FROM Tournament T WHERE T.SanctionId = '" + inStanctionId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count == 0 ) return false;
			return true;
		}
	}
}
