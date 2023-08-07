using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class EventRunStats : Form {
        private String mySanctionNum = null;
        private String myTourRules = "";
        private DataRow myTourRow;
        private DataTable myDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public EventRunStats() {
            InitializeComponent();
        }

        private void EventRunStats_Load( object sender, EventArgs e ) {

            if ( Properties.Settings.Default.EventRunStats_Width > 0 ) {
                this.Width = Properties.Settings.Default.EventRunStats_Width;
            }
            if ( Properties.Settings.Default.EventRunStats_Height > 0 ) {
                this.Height = Properties.Settings.Default.EventRunStats_Height;
            }
            if ( Properties.Settings.Default.EventRunStats_Location.X > 0
                && Properties.Settings.Default.EventRunStats_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.EventRunStats_Location;
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

        private void EventRunStats_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.EventRunStats_Width = this.Size.Width;
                Properties.Settings.Default.EventRunStats_Height = this.Size.Height;
                Properties.Settings.Default.EventRunStats_Location = this.Location;
            }
        }

        public void navRefresh_Click( object sender, EventArgs e ) {
            this.Cursor = Cursors.WaitCursor;
            setTourStats();
            myDataTable = getEventStatsData();
            loadDataGrid();
            this.Cursor = Cursors.Default;
        }

        private void loadDataGrid() {
            DataGridViewRow curViewRow;
            this.Cursor = Cursors.WaitCursor;
            String curEvent = "", prevEvent = "";
            Int16 curRound = 0, prevRound = 0;
            Decimal curTotalEventSkiers = 0, curTotalRoundSkiers = 0;
            Decimal curTotalEventPasses = 0, curTotalRoundPasses = 0;
            TimeSpan curTotalEventTime = new TimeSpan();
            TimeSpan curTotalRoundTime = new TimeSpan();

            scoreSummaryDataGridView.Rows.Clear();
            DateTime curStartTime = new DateTime();
            DateTime curEndTime = new DateTime();
            TimeSpan curTimeDiff;
            Decimal curTotalMins, curNumSkiers, curNumPasses;
            int curIdx = 0;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                curEvent = (String)curRow["Event"];
                curRound = (byte)curRow["Round"];
                if ( ( curEvent != prevEvent ) || ( curRound != prevRound ) ) {
                    if ( curIdx > 0 ) {
                        curIdx = scoreSummaryDataGridView.Rows.Add();
                        curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                        curViewRow.Cells["Event"].Value = prevEvent;
                        curViewRow.Cells["Round"].Value = prevRound.ToString();
                        curViewRow.Cells["NumSkiers"].Value = curTotalRoundSkiers.ToString();
                        curViewRow.Cells["NumPasses"].Value = curTotalRoundPasses.ToString();
                        curViewRow.Cells["EventGroup"].Value = "Total";
                        curViewRow.Cells["EventDuration"].Value = curTotalRoundTime.Hours.ToString( "#0" ) + "h " + curTotalRoundTime.Minutes.ToString( "00" ) + "m";
                        curTotalMins = Convert.ToDecimal( curTotalRoundTime.TotalMinutes );
                        try {
                            curViewRow.Cells["PerSkier"].Value = Convert.ToDecimal( curTotalMins / curTotalRoundSkiers ).ToString( "##0.0" );
                        } catch {
                            curViewRow.Cells["PerSkier"].Value = "";
                        }
                        try {
                            curViewRow.Cells["PerPass"].Value = Convert.ToDecimal( curTotalMins / curTotalEventPasses).ToString( "##0.0" );
                        } catch {
                            curViewRow.Cells["PerPass"].Value = "";
                        }

                        curTotalRoundSkiers = 0;
                        curTotalRoundPasses = 0;
                        curTotalRoundTime = new TimeSpan();

                        if ( ( curEvent != prevEvent ) ) {
                            curIdx = scoreSummaryDataGridView.Rows.Add();
                            curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                            curViewRow.Cells["Event"].Value = prevEvent;
                            curViewRow.Cells["Round"].Value = "";
                            curViewRow.Cells["EventGroup"].Value = "Total";
                            curViewRow.Cells["NumSkiers"].Value = curTotalEventSkiers.ToString();
                            curViewRow.Cells["NumPasses"].Value = curTotalEventPasses.ToString();
                            curViewRow.Cells["EventDuration"].Value = curTotalEventTime.Hours.ToString( "#0" ) + "h " + curTotalEventTime.Minutes.ToString( "00" ) + "m";
                            curTotalMins = Convert.ToDecimal( curTotalEventTime.TotalMinutes );
                            try {
                                curViewRow.Cells["PerSkier"].Value = Convert.ToDecimal( curTotalMins / curTotalEventSkiers ).ToString( "##0.0" );
                            } catch {
                                curViewRow.Cells["PerSkier"].Value = "";
                            }
                            try {
                                curViewRow.Cells["PerPass"].Value = Convert.ToDecimal( curTotalMins / curTotalEventPasses).ToString( "##0.0" );
                            } catch {
                                curViewRow.Cells["PerPass"].Value = "";
                            }

                            curTotalEventSkiers = 0;
                            curTotalEventPasses = 0;
                            curTotalEventTime = new TimeSpan();
                        }
                        scoreSummaryDataGridView.Rows.Add();
                    }
                }

                curIdx = scoreSummaryDataGridView.Rows.Add();
                curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                curViewRow.Cells["Event"].Value = curEvent;
                curViewRow.Cells["Round"].Value = curRound.ToString();
                curViewRow.Cells["EventGroup"].Value = (String)curRow["EventGroup"];
                curViewRow.Cells["NumSkiers"].Value = ( (int)curRow["NumSkiers"] ).ToString();
                curViewRow.Cells["NumPasses"].Value = ( (int)curRow["NumPasses"] ).ToString();
                curViewRow.Cells["StartTime"].Value = ( (DateTime)curRow["StartTime"] ).ToString( "MM/dd/yy hh:mm" );
                curViewRow.Cells["EndTime"].Value = ( (DateTime)curRow["EndTime"] ).ToString( "MM/dd/yy hh:mm" );

                curStartTime = (DateTime)curRow["StartTime"];
                curEndTime = (DateTime)curRow["EndTime"];
                curTimeDiff = curEndTime - curStartTime;
                curViewRow.Cells["EventDuration"].Value = curTimeDiff.Hours.ToString( "#0" ) + "h " + curTimeDiff.Minutes.ToString( "00" ) + "m";

                curNumSkiers = Convert.ToDecimal( ((int)curRow["NumSkiers"] ).ToString());
                curViewRow.Cells["NumPasses"].Value = ( (int)curRow["NumPasses"] ).ToString();
                curNumPasses = Convert.ToDecimal( ( (int)curRow["NumPasses"] ).ToString() );
                curTotalMins = Convert.ToDecimal( curTimeDiff.TotalMinutes );

                try {
                    curViewRow.Cells["PerSkier"].Value = Convert.ToDecimal( curTotalMins / curNumSkiers ).ToString("##0.0");
                } catch {
                    curViewRow.Cells["PerSkier"].Value = "";
                }
                try {
                    curViewRow.Cells["PerPass"].Value = Convert.ToDecimal( curTotalMins / curNumPasses ).ToString( "##0.0" );
                } catch {
                    curViewRow.Cells["PerPass"].Value = "";
                }

                curTotalEventSkiers += curNumSkiers;
                curTotalRoundSkiers += curNumSkiers;
                curTotalEventPasses += curNumPasses;
                curTotalRoundPasses += curNumPasses;
                curTotalEventTime += curTimeDiff;
                curTotalRoundTime += curTimeDiff;

                prevEvent = curEvent;
                prevRound = curRound;
            }

            if ( curIdx > 0 ) {
                curIdx = scoreSummaryDataGridView.Rows.Add();
                curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                curViewRow.Cells["Event"].Value = prevEvent;
                curViewRow.Cells["Round"].Value = "";
                curViewRow.Cells["EventGroup"].Value = "Total";
                curViewRow.Cells["EventDuration"].Value = curTotalRoundTime.Hours.ToString( "#0" ) + "h " + curTotalRoundTime.Minutes.ToString( "00" ) + "m";
                curViewRow.Cells["NumSkiers"].Value = curTotalRoundSkiers.ToString();
                curViewRow.Cells["NumPasses"].Value = curTotalRoundPasses.ToString();
                curTotalMins = Convert.ToDecimal( curTotalEventTime.TotalMinutes );
                try {
                    curViewRow.Cells["PerSkier"].Value = Convert.ToDecimal( curTotalMins / curTotalEventSkiers ).ToString( "##0.0" );
                } catch {
                    curViewRow.Cells["PerSkier"].Value = "";
                }
                try {
                    curViewRow.Cells["PerPass"].Value = Convert.ToDecimal( curTotalMins / curTotalRoundPasses).ToString( "##0.0" );
                } catch {
                    curViewRow.Cells["PerPass"].Value = "";
                }

                curIdx = scoreSummaryDataGridView.Rows.Add();
                curViewRow = scoreSummaryDataGridView.Rows[curIdx];
                curViewRow.Cells["Event"].Value = prevEvent;
                curViewRow.Cells["Round"].Value = "";
                curViewRow.Cells["EventGroup"].Value = "Total";
                curViewRow.Cells["NumSkiers"].Value = curTotalEventSkiers.ToString();
                curViewRow.Cells["NumPasses"].Value = curTotalEventPasses.ToString();
                curViewRow.Cells["EventDuration"].Value = curTotalEventTime.Hours.ToString( "#0" ) + "h " + curTotalEventTime.Minutes.ToString( "00" ) + "m";
                curTotalMins = Convert.ToDecimal( curTotalEventTime.TotalMinutes );
                try {
                    curViewRow.Cells["PerSkier"].Value = Convert.ToDecimal( curTotalMins / curTotalEventSkiers ).ToString( "##0.0" );
                } catch {
                    curViewRow.Cells["PerSkier"].Value = "";
                }
                try {
                    curViewRow.Cells["PerPass"].Value = Convert.ToDecimal( curTotalMins / curTotalEventPasses).ToString( "##0.0" );
                } catch {
                    curViewRow.Cells["PerPass"].Value = "";
                }
            }

            this.Cursor = Cursors.Default;
            try {
                RowStatusLabel.Text = "Row 1 of " + scoreSummaryDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }

        }

        private void setTourStats() {
            String selectStmt = "";
            int curIdx = 0;
            int curNumRides = 0;
            DataGridViewRow curViewRow;
            DataTable curDataTable;
            DataRow curRow;


            if ( HelperFunctions.getDataRowColValueDecimal( myTourRow, "SlalomRounds", 0 ) > 0 ) {
                // Calcualte number of slalom participants
                curIdx = eventStatsDataGridView.Rows.Add();
                curViewRow = eventStatsDataGridView.Rows[curIdx];
                curViewRow.Cells["EventName"].Value = "Slalom";

                selectStmt = "Select count(*) as SkierCount "
                    + "From (SELECT DISTINCT MemberId From SlalomScore "
                    + "WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["SkierCount"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierCount", "0" );
                } else {
                    curViewRow.Cells["SkierCount"].Value = "0";
                }

                // Calcualte number of slalom rides
                selectStmt = "Select count(*) as RideCount "
                    + " From SlalomScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["RideCount"].Value = HelperFunctions.getDataRowColValue( curRow, "RideCount", "0" );
                    curNumRides += (int)curRow["RideCount"];
                
                } else {
                    curViewRow.Cells["RideCount"].Value = "0";
                }

                // Calcualte number of Slalom passes
                selectStmt = "Select count(*) as PassCount "
                    + " From SlalomRecap "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["PassCount"].Value = HelperFunctions.getDataRowColValue( curRow, "PassCount", "0" );
                
                } else {
                    curViewRow.Cells["PassCount"].Value = "0";
                }
            }

            if ( HelperFunctions.getDataRowColValueDecimal( myTourRow, "TrickRounds", 0 ) > 0 ) {
                // Calcualte number of trick participants
                curIdx = eventStatsDataGridView.Rows.Add();
                curViewRow = eventStatsDataGridView.Rows[curIdx];
                curViewRow.Cells["EventName"].Value = "Trick";

                selectStmt = "Select count(*) as SkierCount "
                    + " From (SELECT DISTINCT MemberId From TrickScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["SkierCount"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierCount", "0" );
                
                } else {
                    curViewRow.Cells["SkierCount"].Value = "0";
                }

                // Calcualte number of trick rides
                selectStmt = "Select count(*) as RideCount "
                    + " From TrickScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["RideCount"].Value = HelperFunctions.getDataRowColValue( curRow, "RideCount", "0" );
                    curNumRides += (int)curRow["RideCount"];

                } else {
                    curViewRow.Cells["RideCount"].Value = "0";
                }

                // Calcualte number of trick rides
                selectStmt = "Select count(*) as PassCount "
                    + " From TrickPass "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["PassCount"].Value = HelperFunctions.getDataRowColValue( curRow, "PassCount", "0" );
                
                } else {
                    curViewRow.Cells["PassCount"].Value = "0";
                }
            }

            if ( HelperFunctions.getDataRowColValueDecimal( myTourRow, "JumpRounds", 0 ) > 0 ) {
                curIdx = eventStatsDataGridView.Rows.Add();
                curViewRow = eventStatsDataGridView.Rows[curIdx];
                curViewRow.Cells["EventName"].Value = "Jump";

                // Calcualte number of jump participants
                selectStmt = "Select count(*) as SkierCount "
                    + " From (SELECT DISTINCT MemberId From JumpScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["SkierCount"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierCount", "0" );
                
                } else {
                    curViewRow.Cells["SkierCount"].Value = "0";
                }

                // Calcualte number of jump rides
                selectStmt = "Select count(*) as RideCount "
                    + " From JumpScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData(selectStmt);
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["RideCount"].Value = HelperFunctions.getDataRowColValue( curRow, "RideCount", "0" );
                    curNumRides += (int)curRow["RideCount"];
                
                } else {
                    curViewRow.Cells["RideCount"].Value = "0";
                }

                // Calcualte number of Slalom passes
                selectStmt = "Select count(*) as PassCount "
                    + " From JumpRecap "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    curViewRow.Cells["PassCount"].Value = HelperFunctions.getDataRowColValue( curRow, "PassCount", "0" );
                
                } else {
                    curViewRow.Cells["PassCount"].Value = "0";
                }
            }

            // Total number of ski rides
            curIdx = eventStatsDataGridView.Rows.Add();
            curViewRow = eventStatsDataGridView.Rows[curIdx];
            curViewRow.Cells["EventName"].Value = "Total";
            curViewRow.Cells["RideCount"].Value = curNumRides.ToString();

            // Calcualte total number of participants
            selectStmt = "Select count(*) as SkierCount "
                + " From ( "
                + " SELECT DISTINCT MemberId From EventReg "
                + " WHERE SanctionId = '" + mySanctionNum + "' "
                + "   AND ("
                + "      EXISTS (SELECT 1 FROM SlalomScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + "       OR "
                + "       EXISTS (SELECT 1 FROM TrickScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + "       OR "
                + "       EXISTS (SELECT 1 FROM JumpScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + " ) ) myTable";

            curDataTable = getData(selectStmt);
            if ( curDataTable.Rows.Count > 0 ) {
                curRow = (DataRow) curDataTable.Rows[0];
                curViewRow.Cells["SkierCount"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierCount", "0" );
                TotalSkiersTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "SkierCount", "0" );

            } else {
                TotalSkiersTextBox.Text = "";
            }
        }

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            myExportData.exportData(scoreSummaryDataGridView, "EventStats.txt");
        }

        private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight( 15, '*' );
            printFooter.PadLeft( 15, '*' );
            myExportData.exportDataAsHtml( scoreSummaryDataGridView, printTitle, printSubtitle, printFooter, "EventStats.htm" );
        }

        private DataTable getEventStatsData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumSkiers " );
            curSqlStmt.Append( "FROM SlalomScore AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Slalom' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, R.EventGroup, S.Round " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumSkiers " );
            curSqlStmt.Append( "FROM JumpScore AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Jump' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, R.EventGroup, S.Round " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumSkiers " );
            curSqlStmt.Append( "FROM TrickScore AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Trick' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, S.Round, R.EventGroup " );
            curSqlStmt.Append( "Order BY S.SanctionId, R.Event, S.Round, R.EventGroup " );
            DataTable curScoresDataTable = getData( curSqlStmt.ToString() );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumPasses, MIN(S.LastUpdateDate) AS StartTime, MAX(S.LastUpdateDate) AS EndTime " );
            curSqlStmt.Append( "FROM SlalomRecap AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Slalom' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, R.EventGroup, S.Round " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumPasses, MIN(S.LastUpdateDate) AS StartTime, MAX(S.LastUpdateDate) AS EndTime " );
            curSqlStmt.Append( "FROM JumpRecap AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Jump' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, R.EventGroup, S.Round " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT S.SanctionId, R.Event, R.EventGroup, S.Round, COUNT(*) AS NumPasses, MIN(S.LastUpdateDate) AS StartTime, MAX(S.LastUpdateDate) AS EndTime " );
            curSqlStmt.Append( "FROM TrickPass AS S" );
            curSqlStmt.Append( "  INNER JOIN EventReg AS R ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.Round < 25 AND R.Event = 'Trick' " );
            curSqlStmt.Append( "GROUP BY S.SanctionId, R.Event, S.Round, R.EventGroup " );
            curSqlStmt.Append( "Order BY S.SanctionId, R.Event, S.Round, R.EventGroup " );
            DataTable curStatsDataTable = getData( curSqlStmt.ToString() );

            DataColumn curCol = new DataColumn();
            curCol = new DataColumn();
            curCol.ColumnName = "NumSkiers";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curStatsDataTable.Columns.Add( curCol );

            String curFilterCommand = "";
            DataRow[] curFindRows;
            foreach ( DataRow curRow in curStatsDataTable.Rows ) {
                curFilterCommand = "Event = '" + (String)curRow["Event"] + "' And EventGroup = '" + (String)curRow["EventGroup"] + "' And Round = " + ((Byte)curRow["Round"]).ToString();
                curFindRows = curScoresDataTable.Select( curFilterCommand );
                if ( curFindRows.Length > 0 ) {
                    curRow["NumSkiers"] = (int)curFindRows[0]["NumSkiers"];
                }
            }

            return curStatsDataTable;

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
            return getData( curSqlStmt.ToString() );
        }

        private void scoreSummaryDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + scoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }

        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printReport( false );
        }

        private bool printReport( bool inPublish ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            Font saveShowDefaultCellStyle = eventStatsDataGridView.DefaultCellStyle.Font;
            eventStatsDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );

            bool returnValue = true;
            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;

            StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
            printTitle.Append( "\n Sanction " + mySanctionNum );
            printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
            printTitle.Append( "\n" + this.Text );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = this.Text;
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = false;

            myPrintDataGrid = new DataGridViewPrinter( eventStatsDataGridView, myPrintDoc,
                CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );
            
            myPrintDataGrid.SubtitleList();
            Font fontPrintSubTitle = new Font( "Arial", 12, FontStyle.Bold );
            StringFormat SubtitleStringFormat = new StringFormat();
            SubtitleStringFormat.Trimming = StringTrimming.Word;
            SubtitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            SubtitleStringFormat.Alignment = StringAlignment.Center;
            StringRowPrinter curSubtitle = new StringRowPrinter( "Number of Participants: " + TotalSkiersTextBox.Text, 250, 0, 250, 50
                , Color.DarkBlue, Color.White, fontPrintSubTitle, SubtitleStringFormat );
            myPrintDataGrid.SubtitleRow = curSubtitle;

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
            curPreviewDialog.Document = myPrintDoc;
            curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();

            eventStatsDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;

            /*
             * Print event stat details
             */
            saveShowDefaultCellStyle = scoreSummaryDataGridView.DefaultCellStyle.Font;
            scoreSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );

            curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;

            myPrintDataGrid = new DataGridViewPrinter( scoreSummaryDataGridView, myPrintDoc,
                CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = this.Text;
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = false;

            myPrintDataGrid.SubtitleList();
            fontPrintSubTitle = new Font( "Arial", 12, FontStyle.Bold );
            SubtitleStringFormat = new StringFormat();
            SubtitleStringFormat.Trimming = StringTrimming.Word;
            SubtitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            SubtitleStringFormat.Alignment = StringAlignment.Center;

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
            curPreviewDialog.Document = myPrintDoc;
            curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();

            scoreSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;


            return returnValue;
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
