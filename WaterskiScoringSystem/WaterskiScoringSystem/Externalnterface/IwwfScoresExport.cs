using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Externalnterface {
	public partial class IwwfScoresExport : Form {
		private String mySanctionNum;

		public IwwfScoresExport() {
			InitializeComponent();
		}

		private void IwwfScoresExport_Load( object sender, EventArgs e ) {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			loadDataView();
		}

        private void loadDataView() {
            Cursor.Current = Cursors.WaitCursor;

            try {
                DataRow curTourRow = getTourData();
                if ( curTourRow  == null ) {
                    MessageBox.Show( "Invalid Sanction, no tournament availabale" );
                    return;
                }
                String curTourEndDate = HelperFunctions.getDataRowColValue( curTourRow, "EventDates", "" );
                String curEventLocation = HelperFunctions.getDataRowColValue( curTourRow, "EventLocation", "" );
                String curSiteCode = "Not Available";
                int curDelimStart = curEventLocation.IndexOf( "(" );
                if ( curDelimStart > 0 ) {
                    int curDelimEnd = curEventLocation.IndexOf( ")" );
                    curSiteCode = curEventLocation.Substring( curDelimStart + 1, curDelimEnd - curDelimStart - 2 );
                }

                RowStatusLabel.Text = "";
                dataGridView.Rows.Clear();
                DataTable curDataTable = getIwwfScores();
                if ( curDataTable != null && curDataTable.Rows.Count > 0 ) {
                    loadScores( curDataTable, curTourEndDate, curSiteCode );
                }

                this.dataGridView.Visible = true;
                try {
                    RowStatusLabel.Text = "Row 1 of " + dataGridView.Rows.Count.ToString();
                } catch {
                    RowStatusLabel.Text = "";
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving list of available published reports \n" + ex.Message );
            
            } finally {
                Cursor.Current = Cursors.Default;
            }
        }

        private void loadScores( DataTable curDataTable, String curTourEndDate, String curSiteCode ) {
            int curViewIdx = 0, curSkiYearAge;
            DataGridViewRow curViewRow;
            int curSkiYear;
            int.TryParse( mySanctionNum.Substring( 0, 2 ), out curSkiYear );
            decimal curScore;
            curSkiYear += 2000;
            String curDiv, curEvent;

            foreach ( DataRow curDataRow in curDataTable.Rows ) {
                String curMemberId = HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );
                String[] curSkierNameParts = getSkierName( curDataRow );
                if ( curSkierNameParts == null || curSkierNameParts.Length != 2 ) {
                    MessageBox.Show( String.Format( "Bypassing skier with member Id {0}, has missing or invalid name", curMemberId ) );
                    continue;
                }

                curViewIdx = dataGridView.Rows.Add();
                curViewRow = dataGridView.Rows[curViewIdx];
                curScore = 0;

                curViewRow.Cells["SanctionId"].Value = mySanctionNum;
                curViewRow.Cells["TourEndDate"].Value = curTourEndDate;
                curViewRow.Cells["TourSiteCode"].Value = curSiteCode;

                curViewRow.Cells["MemberId"].Value = curMemberId;
                curViewRow.Cells["LastName"].Value = curSkierNameParts[0];
                curViewRow.Cells["FirstName"].Value = curSkierNameParts[1];
                curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ).ToUpper();
                curViewRow.Cells["IwwfAthleteId"].Value = getIwwfAthleteId( curDataRow );

                curViewRow.Cells["Gender"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
                int.TryParse( HelperFunctions.getDataRowColValue( curDataRow, "SkiYearAge", "0" ), out curSkiYearAge );
                curViewRow.Cells["SkierYob"].Value = curSkiYear - curSkiYearAge - 1;

                curViewRow.Cells["SeniorOrJunior"].Value = "S";
                curDiv = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                if ( curDiv.Equals( "OM" ) || curDiv.Equals( "OM" ) ) {
                    curViewRow.Cells["SeniorOrJunior"].Value = "S";
                } else if ( curSkiYearAge < 18 ) curViewRow.Cells["SeniorOrJunior"].Value = "J";

                curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventClass", "" );
                curViewRow.Cells["Round"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Round", "" );
                curViewRow.Cells["Div"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );

                curEvent = HelperFunctions.getDataRowColValue( curDataRow, "Event", "" );
                if ( curEvent.Equals( "Slalom" ) ) {
                    curViewRow.Cells["SlalomScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
                    curViewRow.Cells["AltScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "FinalPassScore", "" );

                    decimal.TryParse( HelperFunctions.getDataRowColValue( curDataRow, "Score", "0" ), out curScore );
                    if ( curScore < 6 ) curViewRow.Cells["SlalomMiss"].Value = "*";
                    curViewRow.Cells["PerfQual1"].Value = HelperFunctions.getDataRowColValue( curDataRow, "FinalLen", "" );
                    curViewRow.Cells["PerfQual2"].Value = HelperFunctions.getDataRowColValue( curDataRow, "FinalSpeedKph", "" );

                } else if ( curEvent.Equals( "Trick" ) ) {
                    decimal.TryParse( HelperFunctions.getDataRowColValue( curDataRow, "Score", "0" ), out curScore );
                    curViewRow.Cells["TrickScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
                    curViewRow.Cells["PerfQual1"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorePass1", "" );
                    curViewRow.Cells["PerfQual2"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorePass2", "" );

                } else if ( curEvent.Equals( "Jump" ) ) {
                    decimal.TryParse( HelperFunctions.getDataRowColValue( curDataRow, "Score", "0" ), out curScore );
                    curViewRow.Cells["JumpScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
                    curViewRow.Cells["AltScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "FinalPassScore", "" );

                    Decimal curRampHeight = Convert.ToDecimal( curDataRow["FinalLen"].ToString().Trim() );
                    if ( curRampHeight == 5.00M ) {
                        curViewRow.Cells["PerfQual1"].Value = ".235";
                    } else if ( curRampHeight == 5.50M ) {
                        curViewRow.Cells["PerfQual1"].Value = ".255";
                    } else if ( curRampHeight == 6.00M ) {
                        curViewRow.Cells["PerfQual1"].Value = ".271";
                    } else if ( curRampHeight == 4.00M ) {
                        curViewRow.Cells["PerfQual1"].Value = ".215";
                    } else if ( curRampHeight == 4.50M ) {
                        curViewRow.Cells["PerfQual1"].Value = ".215";
                    } else {
                        curViewRow.Cells["PerfQual1"].Value = ".235";
                    }
                    curViewRow.Cells["PerfQual2"].Value = HelperFunctions.getDataRowColValue( curDataRow, "FinalSpeedKph", "" );
                }

                curViewRow.Cells["ExportFlag"].Value = "N";
                if ( curScore > 0m ) curViewRow.Cells["ExportFlag"].Value = "Y";
                curViewRow.Cells["Plcmt"].Value = "1";
            }
        }

        private void dataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = String.Format("Row {0} of {1}", curRowPos, dataGridView.Rows.Count );
        }

        private String[] getSkierName( DataRow curDataRow ) {
            String curSkierName = HelperFunctions.getDataRowColValue( curDataRow, "SkierName", "" );
            if ( HelperFunctions.isObjectEmpty( curSkierName ) ) return null;
            HelperFunctions.stringReplace( curSkierName, HelperFunctions.SingleQuoteDelim, "" );
            return curSkierName.Split( ',' );
        }

        private String getIwwfAthleteId( DataRow curDataRow ) {
            String curFedCode = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ).ToUpper();
            if ( HelperFunctions.isObjectEmpty( curFedCode ) || curFedCode.Equals( "USA" ) ) {
                return "USA" + HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );
            } else {
                return curFedCode + HelperFunctions.getDataRowColValue( curDataRow, "ForeignFederationID", "" );
            }
        }

        private DataTable getIwwfScores() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, TR.Gender, TR.SkiYearAge, TR.Federation, TR.ForeignFederationID" );
            curSqlStmt.Append( ", ER.Event, ER.AgeGroup" );
            curSqlStmt.Append( ", SS.EventClass, SS.Round as Round" );
            curSqlStmt.Append( ", CONVERT( numeric( 8, 2 ), SS.Score ) as Score, 0 AS ScorePass1, 0 AS ScorePass2" );
            curSqlStmt.Append( ", SS.FinalSpeedKph AS FinalSpeedKph, CONVERT( numeric( 6, 2 ), SS.FinalLen ) AS FinalLen, CONVERT(numeric( 6, 2 ), SS.FinalPassScore) AS FinalPassScore " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
            curSqlStmt.Append( "  INNER JOIN SlalomScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round > 0 AND SS.Round < 25 " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Slalom' " );
            curSqlStmt.Append( "  AND SS.EventClass IN ('L', 'R')" );
            curSqlStmt.Append( " UNION " );
            curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, TR.Gender, TR.SkiYearAge, TR.Federation, TR.ForeignFederationID" );
            curSqlStmt.Append( ", ER.Event, ER.AgeGroup" );
            curSqlStmt.Append( ", SS.EventClass, COALESCE(SS.Round, 0) as Round" );
            curSqlStmt.Append( ", CONVERT(numeric(8,2), SS.Score) as Score, SS.ScorePass1 AS ScorePass1, SS.ScorePass2 AS ScorePass2" );
            curSqlStmt.Append( ", 0 AS FinalSpeedKph, 0.0 AS FinalLen, 0.0 AS FinalPassScore " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
            curSqlStmt.Append( "  INNER JOIN TrickScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round > 0 AND SS.Round < 25 " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Trick' " );
            curSqlStmt.Append( "  AND SS.EventClass IN ('L', 'R') " );
            curSqlStmt.Append( " UNION " );
            curSqlStmt.Append( "SELECT TR.MemberId, TR.SanctionId, TR.SkierName, TR.Gender, TR.SkiYearAge, TR.Federation, TR.ForeignFederationID" );
            curSqlStmt.Append( ", ER.Event, ER.AgeGroup" );
            curSqlStmt.Append( ", SS.EventClass, SS.Round as Round" );
            curSqlStmt.Append( ", CONVERT( numeric( 8, 2 ), SS.ScoreMeters ) as Score, 0 AS ScorePass1, 0 AS ScorePass2" );
            curSqlStmt.Append( ", SS.BoatSpeed AS FinalSpeedKph, SS.RampHeight AS FinalLen, SS.ScoreFeet AS FinalPassScore " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON TR.MemberId = ER.MemberId AND TR.SanctionId = ER.SanctionId AND TR.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN Tournament T ON T.SanctionId = TR.SanctionId " );
            curSqlStmt.Append( "  INNER JOIN JumpScore SS ON SS.MemberId = TR.MemberId AND SS.SanctionId = TR.SanctionId AND SS.AgeGroup = TR.AgeGroup AND SS.Round > 0 AND SS.Round < 25 " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "  AND SS.EventClass IN ('L', 'R') " );
            curSqlStmt.Append( "ORDER BY TR.SkierName, TR.MemberId, ER.Event, Round " );

            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataRow getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId,Name, Class, Federation, EventDates, EventLocation " );
            curSqlStmt.Append( "FROM Tournament " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString());
            if ( curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
            return null;
        }

		private void ExportButton_Click( object sender, EventArgs e ) {
            String curMethodName = "ExportButton_Click";
            int curCountWritten = 0, curCountBypassed = 0;
            String curMsg = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;
            String curExportFilename = mySanctionNum + "RS.txt";

            try {
                Log.WriteFile( curMethodName + ":begin exporting to file " + curExportFilename );
                outBuffer = HelperFunctions.getExportFile( null, curExportFilename );
                if ( outBuffer == null ) {
                    curMsg = "Export file not available";
                    MessageBox.Show( curMsg );
                    return;
                }

                outLine = new StringBuilder( "" );

                foreach ( DataGridViewRow curViewRow in dataGridView.Rows ) {
                    if ( !( HelperFunctions.isValueTrue( HelperFunctions.getViewRowColValue( curViewRow, "ExportFlag", "" ) ) ) ) {
                        curCountBypassed++;
                        continue;
                    }

                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "LastName", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "FirstName", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "MemberId", "" ) + ";;" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "Federation", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "Gender", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "SanctionId", "" ) + ";" );

                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "SlalomScore", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "TrickScore", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "JumpScore", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "AltScore", "" ) + ";" );

                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "SkierYob", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "EventClass", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "Round", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "Div", "" ) + ";" );

                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "PerfQual1", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "PerfQual2", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "TourEndDate", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "SeniorOrJunior", "" ) + ";" );

                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "ExportFlag", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "SlalomMiss", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "Plcmt", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "IwwfAthleteId", "" ) + ";" );
                    outLine.Append( HelperFunctions.getViewRowColValue( curViewRow, "TourSiteCode", "" ) + "" );

                    outBuffer.WriteLine( outLine.ToString() );
                    curCountWritten++;
                    outLine = new StringBuilder( "" );
                }
                outBuffer.Close();

                if ( dataGridView.Rows.Count > 0 ) {
                    curMsg = String.Format( "Export file written {0} with {1} Records {2} Bypassed", curExportFilename, curCountWritten, curCountBypassed );
                } else {
                    curMsg = "No rows found";
                }
                MessageBox.Show( curMsg );

                Log.WriteFile( curMethodName + ":conplete:" + curMsg );

            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not write file from data table\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );

            } finally {
                if ( outBuffer != null ) outBuffer.Close();
            }

        }
    }
}
