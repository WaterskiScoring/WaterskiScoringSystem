﻿using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportBoatTimesJump {
        private String myTourClass;
        private DataRow myTourRow;
        private ListSkierClass mySkierClassList;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private ProgressWindow myProgressInfo;

        public ExportBoatTimesJump() {
        }

        public Boolean exportBoatTimes( String inSanctionId ) {
            Boolean returnStatus = false;
            Int16 curSanctionYear = 0; 
            char DoubleQuote = '"';
            String curMemberId = "", curTourClass = "", curEventDateOut = "", curRunTime = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;
            myProgressInfo = new ProgressWindow();

            DataTable curTourDataTable = getTourData( inSanctionId );
            if ( curTourDataTable == null || curTourDataTable.Rows.Count <= 0 ) return false;

            curSanctionYear = Convert.ToInt16( inSanctionId.Substring( 0, 2 ) );
            myTourRow = curTourDataTable.Rows[0];
            curTourClass = myTourRow["Class"].ToString().Trim();
            String curFilename = inSanctionId.Trim() + "JT.csv";
            outBuffer = getExportFile( curFilename );

            curEventDateOut = myTourRow["EventDates"].ToString();
            if ( curEventDateOut.Length > 7 ) {
                try {
                    DateTime curEventDate = Convert.ToDateTime( curEventDateOut );
                    curEventDateOut = curEventDate.ToString( "yyyyMMdd" );
                } catch {
                    curEventDateOut = "";
                }
            } else {
                curEventDateOut = "";
            }
            mySkierClassList = new ListSkierClass();
            mySkierClassList.ListSkierClassLoad();
            myTourClass = myTourRow["Class"].ToString().ToUpper();
            myClassCRow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'C'" )[0];
            myClassERow = mySkierClassList.SkierClassDataTable.Select( "ListCode = 'E'" )[0];

            Decimal curSplitTime30M, curSplitTime52M, curSplitTime82M, curBoatEndTime, curScoreMeters, curScoreFeet;
            Decimal curActualSeg52m, curActualSeg30m, curActualSeg41m, curActualSeg82m;
            String curResults, curTimeClass;
            if ( outBuffer != null ) {
                outLine.Append( "Sanction" );
                outLine.Append( ", Skier Class" );
                outLine.Append( ", Boat" );
                outLine.Append( ", EventDate" );
                outLine.Append( ", Time" );
                outLine.Append( ", SkierName" );
                outLine.Append( ", Div" );
                outLine.Append( ", Pass" );
                outLine.Append( ", Speed" );
                outLine.Append( ", Meters" );
                outLine.Append( ", Feet" );
                outLine.Append( ", Time30M" );
                outLine.Append( ", Time52M" );
                outLine.Append( ", Time82M" );
                outLine.Append( ", Time41M" );
                outLine.Append( ", Actual30M" );
                outLine.Append( ", Actual52M" );
                outLine.Append( ", Actual82M" );
                outLine.Append( ", Actual41M" );

                //Write output line to file
                outBuffer.WriteLine( outLine.ToString() );
                outLine = new StringBuilder( "" );


                myProgressInfo.setProgessMsg( "Export jump boat times " );
                myProgressInfo.Show();
                myProgressInfo.Refresh();
                int curRowCount = 0;
                
                DataTable curBoatTimeResults = getBoatTimeResults( inSanctionId );
                myProgressInfo.setProgressMax( curBoatTimeResults.Rows.Count );

                if ( curBoatTimeResults == null || curBoatTimeResults.Rows.Count <= 0 ) {
                    returnStatus = true;
                    outBuffer.Close();
                    MessageBox.Show( "No rows found" );
                    myProgressInfo.Close();
                }

                foreach ( DataRow curRow in curBoatTimeResults.Rows ) {
                    curRowCount++;
                    myProgressInfo.setProgressValue( curRowCount );
                    myProgressInfo.Refresh();

                    curTimeClass = getTimeClass( HelperFunctions.getDataRowColValue( curRow, "EventClass", "C" ) );
                    curResults = HelperFunctions.getDataRowColValue( curRow, "Results", "" );

                    if ( !(decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatSplitTime", "0" ), out curSplitTime52M ) ) ) curSplitTime52M = 0;
                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatSplitTime2", "0" ), out curSplitTime82M ) ) ) curSplitTime82M = 0;
                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatEndTime", "0" ), out curBoatEndTime ) ) ) curBoatEndTime = 0;
                    curSplitTime30M = curSplitTime82M - curSplitTime52M;

                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "0" ), out curScoreMeters ) ) ) curScoreMeters = 0;
                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "0" ), out curScoreFeet ) ) ) curScoreFeet = 0;
                    if ( curTimeClass.Equals( "R" ) ) {
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime52mSegR", "0" ), out curActualSeg52m ) ) ) curActualSeg52m = 0;
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime41mSegR", "0" ), out curActualSeg41m ) ) ) curActualSeg41m = 0;
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime82mSegR", "0" ), out curActualSeg82m ) ) ) curActualSeg82m = 0;
                    
                    } else {
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime52mSeg", "0" ), out curActualSeg52m ) ) ) curActualSeg52m = 0;
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime41mSeg", "0" ), out curActualSeg41m ) ) ) curActualSeg41m = 0;
                        if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime82mSeg", "0" ), out curActualSeg82m ) ) ) curActualSeg82m = 0;
                    }
                    curActualSeg30m = curActualSeg82m - curActualSeg52m;
                    try {
                        curRunTime = ( (DateTime)curRow["LastUpdateDate"] ).ToString( "hhmm" );
                    } catch {
                        curRunTime = "0000";
                    }

                    outLine.Append( DoubleQuote.ToString() + inSanctionId + curTourClass + DoubleQuote.ToString() ); // 1. (7) SanctionId with class
                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "EventClass", "C" ) + DoubleQuote.ToString() ); // 2. (1) Skier Class
                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "Boat", "C" ).PadRight( 11, ' ' ) + DoubleQuote.ToString() ); // 3. (11) Boat
                    outLine.Append( "," + DoubleQuote.ToString() + curEventDateOut + DoubleQuote.ToString() ); // 4. (8) Date YYYYMMDD
                    outLine.Append( "," + DoubleQuote.ToString() + curRunTime + DoubleQuote.ToString() ); // 5. (4) Time HHMM

                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "SkierName", "" ).PadRight( 22, ' ' ) + DoubleQuote.ToString() );
                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" ) + DoubleQuote.ToString() ); // 7. (2) Age Division
                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "PassNum", "" ).PadLeft( 2, ' ' ) + DoubleQuote.ToString() ); // 8. (2) Pass Number
                    outLine.Append( "," + DoubleQuote.ToString() + HelperFunctions.getDataRowColValue( curRow, "BoatSpeed", "" ) + DoubleQuote.ToString() ); // 9. (2) Boat Speed KM
                    if ( curResults.ToUpper().Equals( "JUMP" ) ) {
                        outLine.Append( "," + DoubleQuote.ToString() + curScoreMeters.ToString( "#0.0" ).PadLeft( 4, ' ' ) + DoubleQuote.ToString() ); //10. (4) Distance in Meters
                        outLine.Append( "," + DoubleQuote.ToString() + curScoreFeet.ToString( "#00" ).PadLeft( 3, ' ' ) + DoubleQuote.ToString() ); //11. (3) Distance in Feet
                    } else {
                        outLine.Append( "," + DoubleQuote.ToString() + curResults.ToUpper() + DoubleQuote.ToString() ); //10. (4) Distance in Meters
                        outLine.Append( "," + DoubleQuote.ToString() + "   " + DoubleQuote.ToString() ); //11. (3) Distance in Feet
                    }
                    outLine.Append( ", " + curSplitTime30M.ToString( "0.00" ) ); //12. (4 Num) 1st segment 30M Time
                    outLine.Append( ", " + curSplitTime52M.ToString( "0.00" ) ); //13. (4 Num) 2nd segment 52M Time
                    outLine.Append( ", " + curSplitTime82M.ToString( "0.00" ) ); //14. (4 Num) 1st & 2nd segment 82M Time
                    outLine.Append( ", " + curBoatEndTime.ToString( "0.00" ) ); //15. (4 Num) 3rd segment 41M Time

                    outLine.Append( ", " + curActualSeg30m.ToString( "0.00" ) ); //16. (4 Num) 1st segment 30M Actual
                    outLine.Append( ", " + curActualSeg52m.ToString( "0.00" ) ); //17. (4 Num) 2nd segment 52M Actual
                    outLine.Append( ", " + curActualSeg82m.ToString( "0.00" ) ); //18. (4 Num) 1st & 2nd segment 82M Actual
                    outLine.Append( ", " + curActualSeg41m.ToString( "0.00" ) ); //19. (4 Num) 3rd segment 41M Actual

                    //Write output line to file
                    outBuffer.WriteLine( outLine.ToString() );

                    //Initialize output buffer
                    outLine = new StringBuilder( "" );
                }
                myProgressInfo.Close();

                returnStatus = true;
                outBuffer.Close();
                MessageBox.Show( curBoatTimeResults.Rows.Count + " rows found and written" );
            }

            return returnStatus;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT T.SanctionId, Name, Class, T.Federation, SlalomRounds, ");
            curSqlStmt.Append(" TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append(" FROM Tournament T ");
            curSqlStmt.Append(" WHERE T.SanctionId = '" + inSanctionId + "'");
            return getData(curSqlStmt.ToString());
        }

        private DataTable getBoatTimeResults( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT JS.MemberId, JS.SanctionId, TR.SkierName, ER.Event, JS.Round, ER.EventGroup, ER.TeamCode" );
            curSqlStmt.Append( ", ER.EventClass, ER.AgeGroup, JR.PassNum, JR.BoatSpeed, JR.RampHeight, JR.ScoreFeet" );
            curSqlStmt.Append( ", JR.ScoreMeters, JR.Results, JR.BoatSplitTime, JR.BoatSplitTime2, JR.BoatEndTime, JR.LastUpdateDate" );
            curSqlStmt.Append( ", JR.TimeInTol, JR.Reride, JR.RerideReason, JS.PK AS ScorePK, JR.PK AS PassPK, JS.Boat" );
            curSqlStmt.Append( ", T1.ListCode AS TimeCode52mSeg, T1.CodeValue AS ActualTime52mSeg" );
            curSqlStmt.Append( ", T3.ListCode AS TimeCode41mSeg, T3.CodeValue AS ActualTime41mSeg" );
            curSqlStmt.Append( ", T4.ListCode AS TimeCode82mSeg, T4.CodeValue AS ActualTime82mSeg " );
            curSqlStmt.Append( ", T1R.ListCode AS TimeCode52mSegR, T1R.CodeValue AS ActualTime52mSegR" );
            curSqlStmt.Append( ", T3R.ListCode AS TimeCode41mSegR, T3R.CodeValue AS ActualTime41mSegR" );
            curSqlStmt.Append( ", T4R.ListCode AS TimeCode82mSegR, T4R.CodeValue AS ActualTime82mSegR " );
            curSqlStmt.Append( "FROM JumpScore JS " );
            curSqlStmt.Append( "INNER JOIN JumpRecap JR ON JS.MemberId = JR.MemberId AND JS.SanctionId = JR.SanctionId AND JS.AgeGroup = JR.AgeGroup AND JS.Round = JR.Round " );
            curSqlStmt.Append( "INNER JOIN TourReg TR ON JS.MemberId = TR.MemberId AND JS.SanctionId = TR.SanctionId AND JS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "INNER JOIN EventReg ER ON JS.MemberId = ER.MemberId AND JS.SanctionId = ER.SanctionId AND JS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T1 ON T1.ListName = 'JumpBoatTime3Seg' AND T1.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-C-52M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T3 ON T3.ListName = 'JumpBoatTime3Seg' AND T3.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-C-41M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T4 ON T4.ListName = 'JumpBoatTime3Seg' AND T4.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-C-82M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T1R ON T1R.ListName = 'JumpBoatTime3Seg' AND T1R.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-R-52M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T3R ON T3R.ListName = 'JumpBoatTime3Seg' AND T3R.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-R-41M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T4R ON T4R.ListName = 'JumpBoatTime3Seg' AND T4R.ListCode = CONVERT(nvarchar(5), JR.BoatSpeed) + '-R-82M' " );
            curSqlStmt.Append( "WHERE JS.SanctionId = '" + inSanctionId + "' AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "ORDER BY JS.SanctionId, JS.Round, JS.MemberId, JR.PassNum" );
            return getData( curSqlStmt.ToString() );
        }

        private String getTimeClass(String inEventClass) {
            String curTimeClass = "C";
            DataRow curClassRow = null;
            DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + inEventClass + "'" );
            if (curRowsFound.Length > 0) {
                curClassRow = curRowsFound[0];
            } else {
                curClassRow = mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + myTourClass + "'" )[0];
            }
            if ((Decimal)myClassERow["ListCodeNum"] < (Decimal)curClassRow["ListCodeNum"]) {
                curTimeClass = "R";
            } else if ((Decimal)myClassCRow["ListCodeNum"] > (Decimal)curClassRow["ListCodeNum"]) {
                curTimeClass = "W";
            } else {
                curTimeClass = "C";
            }
            return curTimeClass;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private StreamWriter getExportFile( String inFileName ) {
            StreamWriter outBuffer = null;

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.FileName = inFileName;

            try {
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    String curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        if ( Path.GetExtension( curFileName ) == null ) curFileName += ".csv";
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

    }
}
