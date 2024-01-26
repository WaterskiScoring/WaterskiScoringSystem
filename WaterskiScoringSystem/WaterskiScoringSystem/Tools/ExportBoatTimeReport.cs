using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlServerCe;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportBoatTimeReport {
        private String myTourClass;
        //private Int16 mySanctionYearFor134 = 12;
        private DataRow myTourRow;
        private DataRow myTimeRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private DataTable myTimesDataTable;
        private ListSkierClass mySkierClassList;
        private ProgressWindow myProgressInfo;

        public ExportBoatTimeReport() {
        }

        public Boolean exportBoatTimes( String inSanctionId ) {
            String curMethodName = "exportBoatTimes";
            Boolean returnStatus = false;
            Int16 curRowCountSlalom = 0, curRowCountJump = 0;
            StreamWriter outBuffer = null;

            try {
                DataTable curTourDataTable = getTourData( inSanctionId );
                if ( curTourDataTable != null ) {
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        String curFilename = inSanctionId.Trim() + "BT.prn";
                        outBuffer = getExportFile( curFilename );
                    }
                    returnStatus = true;
                }
                if ( outBuffer != null ) {
                    mySkierClassList = new ListSkierClass();
                    mySkierClassList.ListSkierClassLoad();
                    myTourClass = myTourRow["Class"].ToString().ToUpper();
                    myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                    myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];

                    curRowCountJump = exportBoatTimesJump( inSanctionId, outBuffer );

                    StringBuilder outLine = new StringBuilder( Environment.NewLine );
                    outBuffer.WriteLine( outLine.ToString() );

                    curRowCountSlalom = exportBoatTimesSlalom( inSanctionId, outBuffer );

                    outBuffer.Close();
                    MessageBox.Show( curRowCountJump + " jump rows found and written"
                        + "\n" + curRowCountSlalom + " slalom rows found and written" );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: " + curMethodName
                    + "\n\nException: " + ex.Message
                );
            }

            return returnStatus;
        }

        private Int16 exportBoatTimesSlalom( String inSanctionId, StreamWriter outBuffer ) {
            String curMethodName = "exportBoatTimesSlalom";
            Int16 curRowCount = 0, curNumRounds = 0, curRowRoundCount = 0, curSkierCount = 0, curRerideCount = 0;
            String curTourClass = "", curEventDateOut = "", prevSkierName = "";
            StringBuilder outLine = new StringBuilder( "" );
            DataTable curSummaryTable, curStatsTable;
            myProgressInfo = new ProgressWindow();

            try {
                curTourClass = myTourRow["Class"].ToString().Trim();
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

                curNumRounds = Convert.ToInt16( (byte)myTourRow["SlalomRounds"] );
                if ( curNumRounds > 0 ) {
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT ListCode, ListCodeNum, SortSeq, CodeValue, MinValue, MaxValue, CodeDesc " );
                    curSqlStmt.Append( "FROM CodeValueList WHERE ListName = 'SlalomBoatTime' ORDER BY SortSeq" );
                    myTimesDataTable = getData( curSqlStmt.ToString() );

                    DataTable curSpeedByPassDataTable, curBoatTimeResults;
                    Int16 curMaxSpeed, curSpeedKph;
                    Decimal curBoatTime, curScore, curPassLineLengthKph, curActualTime;
                    String curSkierName, curSpeedDesc, curTimeKey, curTimeKeyScore, curEventClass, curRunTime;
                    DataRow[] curTimeRowsFound;

                    for ( byte curRound = 1; curRound <= curNumRounds; curRound++ ) {
                        curSummaryTable = buildSummaryTable();
                        curStatsTable = buildStatsTable();
                        if ( curRound > 1 ) {
                            outLine = new StringBuilder( Environment.NewLine );
                            outBuffer.WriteLine( outLine.ToString() );
                        }

                        outLine = new StringBuilder( "" );
                        outLine.Append( inSanctionId + curTourClass + "    " + myTourRow["Name"].ToString() + " Slalom  Round " + curRound.ToString() );
                        outBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                        outLine.Append( "TIME                STANDARD TIMING  ------ Pass Time ------" );
                        outBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                        outLine.Append( "HHMM SKIER NAME      # SP CL SCORE   -1    5    |    5    1+ " );
                        outBuffer.WriteLine( outLine.ToString() );

                        //Initialize output buffer
                        outLine = new StringBuilder( "" );

                        curSkierCount = 0;
                        curRerideCount = 0;
                        curBoatTimeResults = getSlalomBoatTimeResults( inSanctionId, curRound );
                        if ( curBoatTimeResults != null ) {
                            myProgressInfo.setProgessMsg( "Export slalom boat times " );
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();
                            myProgressInfo.setProgressMax( curBoatTimeResults.Rows.Count );
                            curRowRoundCount = 0;

                            foreach (DataRow curRow in curBoatTimeResults.Rows) {
                                curRowCount++;
                                curRowRoundCount++;
                                myProgressInfo.setProgressValue( curRowRoundCount );
                                myProgressInfo.Refresh();

                                curSkierName = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
                                if ( curSkierName.Length > 15 ) {
                                    curSkierName = curSkierName.Substring( 0, 15 );
                                }
                                if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatTime", "0" ), out curBoatTime ) ) ) curBoatTime = 0;
                                if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "PassScore", "0" ), out curScore ) ) ) curScore = 0;
                                curEventClass = HelperFunctions.getDataRowColValue( curRow, "EventClass", "" );
                                try {
                                    curRunTime = ((DateTime)curRow["LastUpdateDate"]).ToString( "hhmm" );
                                } catch {
                                    curRunTime = "0000";
                                }
                                if ( HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( curRow, "Reride", "N" ) ) ) curRerideCount++;
                                try {
                                    if ( !( Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, "PassScore", "0" ), out curMaxSpeed ) ) ) curMaxSpeed = 0;
                                    if ( !( Int16.TryParse( HelperFunctions.getDataRowColValue( curRow, "PassSpeedKph", "0" ), out curSpeedKph ) ) ) curSpeedKph = 0;
                                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "PassLineLengthKph", "0" ), out curPassLineLengthKph ) ) ) curPassLineLengthKph = 0;

									curSpeedByPassDataTable = getSpeedByPass( curMaxSpeed, curSpeedKph, curPassLineLengthKph );
                                    if ( curSpeedByPassDataTable.Rows.Count > 0 ) {
                                        curSpeedDesc = (String)curSpeedByPassDataTable.Rows[0]["CodeValue"];
                                        curTimeKeyScore = curScore.ToString( "0.00" );
                                        curTimeKey = curSpeedKph.ToString( "00" ) + "-" + getTimeClass(curEventClass) + "-" + curTimeKeyScore.Substring( 0, 1 );
                                        curTimeRowsFound = myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" );
                                        if ( curTimeRowsFound.Length > 0 ) {
                                            myTimeRow = curTimeRowsFound[0];
                                        } else {
                                            myTimeRow = null;
                                        }
                                        curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                                    
                                    } else {
                                        curActualTime = 0;
                                        curSpeedDesc = "";
                                        myTimeRow = null;
                                    }

                                } catch (Exception ex) {
                                    curActualTime = 0;
									curPassLineLengthKph = 0;
									curSpeedKph = 0;
                                    curMaxSpeed = 0;
                                    curSpeedDesc = "";
                                    myTimeRow = null;
                                }

                                if ( prevSkierName == curSkierName ) {
                                    outLine.Append( "".PadRight( 4, ' ' ) ); //Time HHMM (4)
                                    outLine.Append( " " + "".PadRight( 15, ' ' ) ); //Skier Name (15)
                                } else {
                                    curSkierCount++;
                                    outLine.Append( curRunTime ); //Time HHMM (4)
                                    outLine.Append( " " + curSkierName.PadRight( 15, ' ' ) ); //Skier Name (15)
                                }
                                outLine.Append( " 0"); //Pass Number (2) 
                                outLine.Append( " " + curSpeedKph.ToString( "00" ) ); //Boat Speed KM (2)
                                outLine.Append( " " + curEventClass.PadRight(2, ' ') ); //Skier Event Class (2)
                                outLine.Append( " " + curScore.ToString( "#.00" ).PadLeft( 5, ' ' ) ); //Score for Pass (4)
                                outLine.Append( " " + graphTimeDiff( "1", curBoatTime, curActualTime, curSummaryTable, curStatsTable, curScore.ToString( "#" ) ) );

                                //Write output line to file
                                outBuffer.WriteLine( outLine.ToString() );

                                //Initialize output buffer
                                outLine = new StringBuilder( "" );

                                prevSkierName = curSkierName;
                            }
                            writeSummarySlalom( curSummaryTable, curStatsTable, curBoatTimeResults, curSkierCount, curRerideCount, outBuffer );
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: " + curMethodName
                    + "\n\nException: " + ex.Message
                );
            }

            myProgressInfo.Close();
            return curRowCount;
        }

        private void writeSummarySlalom( DataTable inSummaryTable, DataTable inStatsTable, DataTable inTimeResults, Int16 inSkierCount, Int16 inRerideCount, StreamWriter outBuffer ) {
            StringBuilder outLine = new StringBuilder( "" );
            outLine.Append( "".PadRight( 60, '-' ) );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inSkierCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " total SKIERS" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inTimeResults.Rows.Count.ToString( "###0" ).PadLeft( 4, ' ' ) + " total RIDES" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inRerideCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " RERIDES" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "".PadLeft( 29, ' ' ) + "-- FAST -- / -- IDEAL -- / -- SLOW --" );
            outBuffer.WriteLine( outLine.ToString() );

            Int16 curScore = -1, prevScore = -1 ;
            Int16 curIdeal = 0, curSlow = 0, curFast = 0, curPassCount = 0, curDiffCount = 0;
            Int16 curIdealTotal = 0, curSlowTotal = 0, curFastTotal = 0, curPassCountTotal = 0;
            Decimal curPercent;
            inSummaryTable.DefaultView.Sort = "Type ASC, Score ASC, Diff DESC";
            inSummaryTable = inSummaryTable.DefaultView.ToTable();
            foreach ( DataRow curRow in inSummaryTable.Rows ) {
                if ( !( curRow["Type"].Equals( "|" ) ) ) {
                    curScore = (Int16)curRow["Score"];
                    if ( curScore != prevScore && prevScore > -1 ) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( curPassCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " passes for scores LE " + prevScore.ToString( "#0" ).PadLeft( 2, ' ' ) + ":" );
                        
                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curFast ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( curFast.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        
                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curIdeal ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( "  " + curIdeal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        
                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curSlow ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( "    " + curSlow.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        outBuffer.WriteLine( outLine.ToString() );

                        curIdealTotal += curIdeal;
                        curSlowTotal += curSlow;
                        curFastTotal += curFast;
                        curPassCountTotal += curPassCount;

                        curPassCount = 0;
                        curIdeal = 0;
                        curSlow = 0;
                        curFast = 0;
                    }
                    try {
                        curDiffCount = (Int16)curRow["Count"];
                        curPassCount += curDiffCount;
                        if ( (Decimal)curRow["Diff"] > 0 ) {
                            curSlow += curDiffCount;
                        } else if ( (Decimal)curRow["Diff"] < 0 ) {
                            curFast += curDiffCount;
                        } else {
                            curIdeal += curDiffCount;
                        }
                    } catch {
                        MessageBox.Show( "Error in jump time diff check" );
                    }
                    prevScore = curScore;
                }
            }
            if ( prevScore > -1 ) {
                outLine = new StringBuilder( "" );
                outLine.Append( curPassCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " passes for scores LE " + prevScore.ToString( "#0" ).PadLeft( 2, ' ' ) + ":" );
                curPercent = Math.Round( ( ( Convert.ToDecimal( curFast ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                if ( curPassCount > 0 ) {
                } else {
                    curPercent = 0M;
                }
                outLine.Append( curFast.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCount > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curIdeal ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "  " + curIdeal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCount > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curSlow ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "    " + curSlow.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                outBuffer.WriteLine( outLine.ToString() );

                curIdealTotal += curIdeal;
                curSlowTotal += curSlowTotal;
                curFastTotal += curFastTotal;
                curPassCountTotal += curPassCount;

                outLine = new StringBuilder( "" );
                outLine.Append( curPassCountTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " total passes           :" );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curFastTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( curFastTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curIdealTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "  " + curIdealTotal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curSlowTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "    " + curSlowTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                outBuffer.WriteLine( outLine.ToString() );
            }

            //inStatsTable
            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "(In hundreds of seconds) ".PadLeft( 30, ' ' ) + "-- Avg  -- / -- Sprd -- / -- Zero -- / -- Max -- / -- Min -- /" );
            outBuffer.WriteLine( outLine.ToString() );

            Int16 curValue, curValue2, curSpread;
            Decimal curAvg;
            inStatsTable.DefaultView.Sort = "Type ASC, Score ASC";
            inStatsTable = inStatsTable.DefaultView.ToTable();
            foreach ( DataRow curRow in inStatsTable.Rows ) {
                if ( !( curRow["Type"].Equals( "|" ) ) ) {
                    outLine = new StringBuilder( "" );
                    if ( (Int16)curRow["Count"] == 0 ) {
                        curAvg = 0M;
                    } else {
                        curAvg = Math.Round( Convert.ToDecimal( (Int16)curRow["DiffSum"] ) / Convert.ToDecimal( (Int16)curRow["Count"] ), 0 );
                    }
                    curValue = (Int16)curRow["MaxDiff"];
                    curValue2 = (Int16)curRow["MinDiff"];
                    curSpread = Convert.ToInt16( curValue - curValue2);

                    outLine.Append( ((Int16)curRow["Count"]).ToString( "###0" ).PadLeft( 4, ' ' ) );
                    outLine.Append( " passes for scores LE " + (Int16)curRow["Score"] + "".PadLeft( 3, ' ' ) );
                    outLine.Append( curAvg.ToString( "###0" ).PadLeft( 8, ' ' ) );
                    outLine.Append( curSpread.ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["ZeroCount"] ).ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["MaxDiff"] ).ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["MinDiff"] ).ToString( "###0" ).PadLeft( 12, ' ' ) );
                    outBuffer.WriteLine( outLine.ToString() );
                }
            }

            outLine = new StringBuilder( "".PadRight( 115, '*' ) );
            outBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
        }

        private Int16 exportBoatTimesJump( String inSanctionId, StreamWriter outBuffer ) {
            String curMethodName = "exportBoatTimesJump";
            Int16 curRowCount = 0, curNumRounds = 0, curRowRoundCount = 0, curSkierCount = 0, curRerideCount = 0, curSanctionYear = 0;
            String curResults, curSkierName, prevSkierName = "", curTourClass = "", curSkierClass = "", curBoatSpeed = "", curEventDateOut = "", curRunTime = "";
            Decimal curScoreMeters, curScoreFeet;
            Decimal curSplitTime52M, curSplitTime82M, curSplitTime41M;
            Decimal curActualSeg52m, curActualSeg82m, curActualSeg41m;
            byte curBoatSpeedNum;
            StringBuilder outLine = new StringBuilder( "" );
            DataTable curSummaryTable, curStatsTable, curJumpTimes;
            myProgressInfo = new ProgressWindow();

            try {
                curSanctionYear = Convert.ToInt16( inSanctionId.Substring( 0, 2 ) );
                curTourClass = myTourRow["Class"].ToString().Trim();
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

                curNumRounds = Convert.ToInt16( (byte)myTourRow["JumpRounds"] );
                for ( byte curRound = 1; curRound <= curNumRounds; curRound++ ) {
                    curSummaryTable = buildSummaryTable();
                    curStatsTable = buildStatsTable();
                    if ( curRound > 1 ) {
                        outLine = new StringBuilder( Environment.NewLine );
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    outLine = new StringBuilder( "" );
                    outLine.Append( inSanctionId + curTourClass + "    " + myTourRow["Name"].ToString() + " JUMP  Round " + curRound.ToString() );
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );
                    outLine.Append( "TIME                STANDARD TIMING  ------- 1ST SEG -------   ------- 2ND SEG -------   ------- 3rd SEG ------- " );
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );
                    outLine.Append( "HHMM SKIER NAME      # SP DISTANCE   -1    5    |    5    1+   -1    5    |    5    1+   -1    5    |    5    1+ " );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Initialize output buffer
                    outLine = new StringBuilder( "" );

                    curSkierCount = 0;
                    curRerideCount = 0;
                    DataTable curBoatTimeResults = getJumpBoatTimeResults( inSanctionId, curRound );
                    if ( curBoatTimeResults != null ) {
                        myProgressInfo.setProgessMsg( "Export jump boat times " );
                        myProgressInfo.Show();
                        myProgressInfo.setProgressMax( curBoatTimeResults.Rows.Count );
                        myProgressInfo.Refresh();
                        curRowRoundCount = 0;
                        
                        foreach (DataRow curRow in curBoatTimeResults.Rows) {
                            curRowCount++;
                            curRowRoundCount++;
                            myProgressInfo.setProgressValue( curRowRoundCount );
                            myProgressInfo.Refresh();

                            curResults = HelperFunctions.getDataRowColValue( curRow, "Results", "" );
                            if ( curResults.Length > 4 ) {
                                curResults = curResults.Substring( 0, 4 );
                            }
                            curSkierName = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
                            if ( curSkierName.Length > 15 ) {
                                curSkierName = curSkierName.Substring( 0, 15 );
                            }

                            if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatSplitTime", "0" ), out curSplitTime52M ) ) ) curSplitTime52M = 0;
                            if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatSplitTime2", "0" ), out curSplitTime82M ) ) ) curSplitTime82M = 0;
                            if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatEndTime", "0" ), out curSplitTime41M ) ) ) curSplitTime41M = 0;
                            if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreMeters", "0" ), out curScoreMeters ) ) ) curScoreMeters = 0;
                            if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ScoreFeet", "0" ), out curScoreFeet ) ) ) curScoreFeet = 0;

                            try {
                                curBoatSpeed = HelperFunctions.getDataRowColValue( curRow, "BoatSpeed", "0" );
                                curSkierClass = HelperFunctions.getDataRowColValue( curRow, "EventClass", "" );
                                curJumpTimes = getJumpTimes( curBoatSpeed, curSkierClass );
                                if ( curJumpTimes.Rows.Count > 0 ) {
                                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime52mSeg", "0" ), out curActualSeg52m ) ) ) curActualSeg52m = 0;
                                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime82mSeg", "0" ), out curActualSeg82m ) ) ) curActualSeg82m = 0;
                                    if ( !( decimal.TryParse( HelperFunctions.getDataRowColValue( curRow, "ActualTime41mSeg", "0" ), out curActualSeg41m ) ) ) curActualSeg41m = 0;
                                } else {
                                    curActualSeg52m = 0;
                                    curActualSeg82m = 0;
                                    curActualSeg41m = 0;
                                }
                            } catch {
                                curActualSeg52m = 0;
                                curActualSeg82m = 0;
                                curActualSeg41m = 0;
                            }
                            try {
                                curRunTime = ((DateTime)curRow["LastUpdateDate"]).ToString( "hhmm" );
                            } catch {
                                curRunTime = "0000";
                            }
                            if ( HelperFunctions.isValueTrue( HelperFunctions.getDataRowColValue( curRow, "Reride", "N" ) ) ) curRerideCount++;

                            if ( prevSkierName == curSkierName ) {
                                outLine.Append( "".PadRight( 4, ' ' ) ); //Time HHMM (4)
                                outLine.Append( " " + "".PadRight( 15, ' ' ) ); //Skier Name (15)
                            } else {
                                curSkierCount++;
                                outLine.Append( curRunTime ); //Time HHMM (4)
                                outLine.Append( " " + curSkierName.PadRight( 15, ' ' ) ); //Skier Name (15)
                            }
                            outLine.Append( HelperFunctions.getDataRowColValue( curRow, "PassNum", "0" ).PadLeft( 2, ' ' ) ); //Pass Number (2) 
                            if ( byte.TryParse( HelperFunctions.getDataRowColValue( curRow, "BoatSpeed", "0" ), out curBoatSpeedNum ) ) curBoatSpeedNum = 0;
                            outLine.Append( " " + curBoatSpeedNum.ToString( "00" ) ); //Boat Speed KM (2)
                            if ( curResults.ToUpper().Equals( "JUMP" ) || curResults.ToUpper().Equals( "FALL" ) ) {
                                outLine.Append( " " + curScoreFeet.ToString( "#00" ).PadLeft( 3, ' ' ) ); //Distance in Feet (3)
                                outLine.Append( " " + curScoreMeters.ToString( "#0.0" ).PadLeft( 4, ' ' ) ); //Distance in Meters (4)

                                outLine.Append( " " + graphTimeDiff( "1", curSplitTime52M, curActualSeg52m, curSummaryTable, curStatsTable, null ) );
                                outLine.Append( " " + graphTimeDiff( "2", curSplitTime82M, curActualSeg82m, curSummaryTable, curStatsTable, null ) );
                                outLine.Append( " " + graphTimeDiff( "3", curSplitTime41M, curActualSeg41m, curSummaryTable, curStatsTable, null ) );

                            } else {
                                outLine.Append( "     " + curResults.ToUpper().PadLeft( 4, ' ' ) );
                                outLine.Append( " " + graphTimeDiff( "1", curSplitTime52M, curActualSeg52m, curSummaryTable, curStatsTable, null ) );
                                outLine.Append( " " + graphTimeDiff( "2", curSplitTime82M, curActualSeg82m, curSummaryTable, curStatsTable, null ) );
                                outLine.Append( " " + graphTimeDiff( "|", 0, 0, curSummaryTable, curStatsTable, null ) );
                            }

                            //Write output line to file
                            outBuffer.WriteLine( outLine.ToString() );

                            //Initialize output buffer
                            outLine = new StringBuilder( "" );

                            prevSkierName = curSkierName;
                        }
                        writeSummaryJump( curSummaryTable, curStatsTable, curBoatTimeResults, curSkierCount, curRerideCount, outBuffer );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: " + curMethodName
                    + "\n\nException: " + ex.Message
                );
            }
            myProgressInfo.Close();
            return curRowCount;
        }

        private void writeSummaryJump( DataTable inSummaryTable, DataTable inStatsTable, DataTable inTimeResults, Int16 inSkierCount, Int16 inRerideCount, StreamWriter outBuffer ) {
            StringBuilder outLine = new StringBuilder( "" );
            outLine.Append( "".PadRight( 115, '-' ) );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inSkierCount.ToString("###0").PadLeft(4, ' ') + " total SKIERS" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inTimeResults.Rows.Count.ToString( "###0" ).PadLeft( 4, ' ' ) + " total RIDES" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( inRerideCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " RERIDES" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "".PadLeft(30, ' ') + "-- FAST -- / -- IDEAL -- / -- SLOW --" );
            outBuffer.WriteLine( outLine.ToString() );

            String curType = "", prevType = "";
            Int16 curIdeal = 0, curSlow = 0, curFast = 0, curPassCount = 0, curDiffCount = 0;
            Int16 curIdealTotal = 0, curSlowTotal = 0, curFastTotal = 0, curPassCountTotal = 0;
            Decimal curPercent;
            inSummaryTable.DefaultView.Sort = "Type ASC, Diff DESC";
            inSummaryTable = inSummaryTable.DefaultView.ToTable();
            foreach ( DataRow curRow in inSummaryTable.Rows ) {
                if ( !( curRow["Type"].Equals( "|" ) ) ) {
                    curType = (String)curRow["Type"];
                    if ( curType != prevType && prevType != "" ) {
                        outLine = new StringBuilder( "" );
                        outLine.Append( curPassCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " segment " + prevType + " jumps timed    " );

                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curFast ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( curFast.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        
                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curIdeal ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( "   " + curIdeal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        
                        if ( curPassCount > 0 ) {
                            curPercent = Math.Round( ( ( Convert.ToDecimal( curSlow ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                        } else {
                            curPercent = 0M;
                        }
                        outLine.Append( "  " + curSlow.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                        outBuffer.WriteLine( outLine.ToString() );

                        curIdealTotal += curIdeal;
                        curSlowTotal += curSlow;
                        curFastTotal += curFast;
                        curPassCountTotal += curPassCount;

                        curPassCount = 0;
                        curIdeal = 0;
                        curSlow = 0;
                        curFast = 0;
                    }
                    try {
                        curDiffCount = (Int16)curRow["Count"];
                        curPassCount += curDiffCount;
                        if ( (Decimal)curRow["Diff"] > 0 ) {
                            curSlow += curDiffCount;
                        } else if ( (Decimal)curRow["Diff"] < 0 ) {
                            curFast += curDiffCount;
                        } else {
                            curIdeal += curDiffCount;
                        }
                    } catch {
                        MessageBox.Show( "Error in jump time diff check" );
                    }
                    prevType = curType;
                }
            }
            if ( prevType != "" ) {
                outLine = new StringBuilder( "" );
                outLine.Append( curPassCount.ToString( "###0" ).PadLeft( 4, ' ' ) + " segment " + prevType + " jumps timed    " );
                if ( curPassCount > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curFast ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( curFast.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCount > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curIdeal ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "   " + curIdeal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCount > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curSlow ) / Convert.ToDecimal( curPassCount ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "  " + curSlow.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                outBuffer.WriteLine( outLine.ToString() );

                curIdealTotal += curIdeal;
                curSlowTotal += curSlow;
                curFastTotal += curFast;
                curPassCountTotal += curPassCount;

                outLine = new StringBuilder( "" );
                outLine.Append( curPassCountTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " total times              " );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curFastTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( curFastTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curIdealTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "   " + curIdealTotal.ToString( "###0" ).PadLeft( 5, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                if ( curPassCountTotal > 0 ) {
                    curPercent = Math.Round( ( ( Convert.ToDecimal( curSlowTotal ) / Convert.ToDecimal( curPassCountTotal ) ) * 100 ), 1 );
                } else {
                    curPercent = 0M;
                }
                outLine.Append( "  " + curSlowTotal.ToString( "###0" ).PadLeft( 4, ' ' ) + " " + curPercent.ToString( "#0.0" ).PadLeft( 4, ' ' ) + "%" );
                outBuffer.WriteLine( outLine.ToString() );
            }


            //inStatsTable
            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "(In hundreds of seconds)  ".PadLeft( 30, ' ' ) + "-- Avg  -- / -- Sprd -- / -- Zero -- / -- Max -- / -- Min -- /" );
            outBuffer.WriteLine( outLine.ToString() );

            Int16 curValue, curValue2, curSpread;
            Decimal curAvg;
            curType = "";
            prevType = "";
            inStatsTable.DefaultView.Sort = "Type ASC";
            inStatsTable = inStatsTable.DefaultView.ToTable();
            foreach ( DataRow curRow in inStatsTable.Rows ) {
                if ( !( curRow["Type"].Equals( "|" ) ) ) {
                    curType = (String)curRow["Type"];
                    outLine = new StringBuilder( "" );
                    curAvg = Math.Round( Convert.ToDecimal( (Int16)curRow["DiffSum"] ) / Convert.ToDecimal( inTimeResults.Rows.Count ), 0 );
                    curValue = (Int16)curRow["MaxDiff"];
                    curValue2 = (Int16)curRow["MinDiff"];
                    curSpread = Convert.ToInt16( curValue - curValue2);
                    outLine.Append( "".PadLeft( 4, ' ' ) + " segment " + curType + "".PadLeft( 16, ' ' ) );
                    outLine.Append( curAvg.ToString( "###0" ).PadLeft( 8, ' ' ) );
                    outLine.Append( curSpread.ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["ZeroCount"] ).ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["MaxDiff"] ).ToString( "###0" ).PadLeft( 13, ' ' ) );
                    outLine.Append( ( (Int16)curRow["MinDiff"] ).ToString( "###0" ).PadLeft( 12, ' ' ) );
                    outBuffer.WriteLine( outLine.ToString() );
                }
            }
            
            outLine = new StringBuilder( "".PadRight( 115, '*' ) );
            outBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
        }

        private String graphTimeDiff( String inValue, Decimal inBoatTime, Decimal inActualTime, DataTable inSummaryTable, DataTable inStatsTable, String inScore ) {
            String curReturnValue = "";
            int curDiff, curPadLeft, curPadRight;

            if ( inBoatTime > 0M && inActualTime > 0M ) {
                curDiff = Convert.ToInt32( ( inBoatTime * 100 ) - ( inActualTime * 100 ) );
                if ( curDiff < 0 ) {
                    if ( ( curDiff * -1 ) > 10 ) {
                        curReturnValue = "  " + inValue + "|         |         | ";
                    } else if ( ( curDiff * -1 ) == 10 ) {
                        curReturnValue = "   " + inValue + "         |         | ";
                    } else if ( ( curDiff * -1 ) == 9 ) {
                        curReturnValue = "   |" + inValue + "        |         | ";
                    } else {
                        curPadLeft = 10 - ( curDiff * -1 );
                        curPadRight = ( curDiff * -1 ) - 1;
                        curReturnValue = "   |" + inValue.PadLeft( curPadLeft, ' ' ) + "".PadRight( curPadRight, ' ' ) + "|         | ";
                    }
                } else if ( curDiff > 0 ) {
                    if ( curDiff > 10 ) {
                        curReturnValue = "   |         |         |" + inValue;
                    } else if ( curDiff == 10 ) {
                        curReturnValue = "   |         |         " + inValue + " ";
                    } else if ( curDiff == 9 ) {
                        curPadLeft = curDiff;
                        curReturnValue = "   |         |" + inValue.PadLeft( curPadLeft, ' ' ) + "| ";
                    } else {
                        curPadRight = 10 - curDiff - 1;
                        curPadLeft = curDiff;
                        curReturnValue = "   |         |" + inValue.PadLeft( curPadLeft, ' ' );
                        curReturnValue += "".PadRight( curPadRight, ' ' ) + "| ";
                    }
                } else {
                    curReturnValue = "   |         " + inValue + "         | ";
                }

                //Update summary data per segment
                DataRow curRow;
                DataRow[] curRowList;
                if ( inScore == null ) {
                    curRowList = inSummaryTable.Select( "Type = '" + inValue + "' AND Diff = '" + curDiff + "'" );
                } else {
                    if ( inScore.Length > 0 ) {
                        curRowList = inSummaryTable.Select( "Type = '" + inValue + "' AND Diff = '" + curDiff + "' AND Score = " + inScore );
                    } else {
                        curRowList = inSummaryTable.Select( "Type = '" + inValue + "' AND Diff = '" + curDiff + "' AND Score = 0 " );
                    }
                }
                if ( curRowList.Length > 0 ) {
                    curRow = curRowList[0];
                    Int16 curCount = (Int16)curRow["Count"];
                    curCount++;
                    curRow["Count"] = curCount;
                } else {
                    DataRowView newRow = inSummaryTable.DefaultView.AddNew();
                    newRow["Type"] = inValue;
                    newRow["Diff"] = curDiff;
                    newRow["Count"] = 1;
                    if ( inScore != null ) {
                        try {
                            newRow["Score"] = Convert.ToInt16( inScore );
                        } catch {
                            newRow["Score"] = Convert.ToInt16( "0" );
                        }
                    }
                    newRow.EndEdit();
                }

                //Update Summary statistics
                if ( inScore == null ) {
                    curRowList = inStatsTable.Select( "Type = '" + inValue + "'" );
                } else {
                    if ( inScore.Length > 0 ) {
                        curRowList = inStatsTable.Select( "Type = '" + inValue + "' AND Score = " + inScore );
                    } else {
                        curRowList = inStatsTable.Select( "Type = '" + inValue + "' AND Score = 0 " );
                    }
                }
                if ( curRowList.Length > 0 ) {
                    curRow = curRowList[0];
                    Int16 curValue = (Int16)curRow["DiffSum"];
                    curRow["DiffSum"] = Convert.ToInt16( curDiff + curValue );
                    if ( curDiff == 0 ) {
                        curValue = (Int16)curRow["ZeroCount"];
                        curValue++;
                        curRow["ZeroCount"] = curValue;
                    } else {
                        curValue = (Int16)curRow["MaxDiff"];
                        if ( curValue < curDiff ) {
                            curRow["MaxDiff"] = Convert.ToInt16( curDiff );
                        }
                        curValue = (Int16)curRow["MinDiff"];
                        if ( curValue > curDiff ) {
                            curRow["MinDiff"] = Convert.ToInt16( curDiff );
                        }
                    }
                    Int16 curCount = (Int16)curRow["Count"];
                    curCount++;
                    curRow["Count"] = curCount;
                } else {
                    Int16 curValue;
                    DataRowView newRow = inStatsTable.DefaultView.AddNew();
                    newRow["Type"] = inValue;
                    newRow["Count"] = 1;
                    try {
                        newRow["DiffSum"] = Convert.ToInt16( curDiff );
                        newRow["MaxDiff"] = Convert.ToInt16( curDiff );
                        newRow["MinDiff"] = Convert.ToInt16( curDiff );
                    } catch {
                        newRow["DiffSum"] = Convert.ToInt16( "0" );
                        newRow["MaxDiff"] = Convert.ToInt16( "0" );
                        newRow["MinDiff"] = Convert.ToInt16( "0" );
                    }
                    if ( curDiff == 0 ) {
                        curValue = 1;
                    } else {
                        curValue = 0;
                    }
                    newRow["ZeroCount"] = curValue;
                    if ( inScore != null ) {
                        try {
                            newRow["Score"] = Convert.ToInt16( inScore );
                        } catch {
                            newRow["Score"] = Convert.ToInt16( "0" );
                        }
                    }
                    newRow.EndEdit();
                }

            }
            return curReturnValue;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, Name, Class, T.Federation, SlalomRounds, ");
            curSqlStmt.Append( " TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append( " FROM Tournament T ");
            curSqlStmt.Append( " WHERE T.SanctionId = '" + inSanctionId + "'" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getJumpBoatTimeResults( String inSanctionId, Int16 inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT JS.MemberId, JS.SanctionId, TR.SkierName, ER.Event, JS.Round, ER.EventGroup, ER.TeamCode" );
            curSqlStmt.Append( ", JS.EventClass, ER.AgeGroup, JR.PassNum, JR.BoatSpeed, JR.RampHeight, JR.ScoreFeet" );
            curSqlStmt.Append( ", JR.ScoreMeters, JR.Results, JR.BoatSplitTime, JR.BoatSplitTime2, JR.BoatEndTime, JR.LastUpdateDate" );
            curSqlStmt.Append( ", JR.TimeInTol, JR.Reride, JR.RerideReason, JS.PK AS ScorePK, JR.PK AS PassPK, JS.Boat " );
            curSqlStmt.Append( "FROM JumpScore JS " );
            curSqlStmt.Append( "INNER JOIN JumpRecap JR ON JS.MemberId = JR.MemberId AND JS.SanctionId = JR.SanctionId AND JS.AgeGroup = JR.AgeGroup AND JS.Round = JR.Round " );
            curSqlStmt.Append( "INNER JOIN TourReg TR ON JS.MemberId = TR.MemberId AND JS.SanctionId = TR.SanctionId AND JS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "INNER JOIN EventReg ER ON JS.MemberId = ER.MemberId AND JS.SanctionId = ER.SanctionId AND JS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE JS.SanctionId = '" + inSanctionId + "' AND JS.Round = " + inRound.ToString() + " AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "ORDER BY JS.SanctionId, JS.Round, JS.MemberId, JR.PassNum" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getJumpTimes(String inBoatSpeed, String inSkierClass) {
            String curSkierClass = getTimeClass( inSkierClass );
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select T1.ListCode AS TimeCode52mSeg, T1.CodeValue AS ActualTime52mSeg" );
            curSqlStmt.Append( ", T3.ListCode AS TimeCode41mSeg, T3.CodeValue AS ActualTime41mSeg" );
            curSqlStmt.Append( ", T4.ListCode AS TimeCode82mSeg, T4.CodeValue AS ActualTime82mSeg " );
            curSqlStmt.Append( "From CodeValueList AS T1 " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T3 ON T3.ListName = 'JumpBoatTime3Seg' AND T3.ListCode = '" + inBoatSpeed + "-" + curSkierClass + "-41M' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS T4 ON T4.ListName = 'JumpBoatTime3Seg' AND T4.ListCode = '" + inBoatSpeed + "-" + curSkierClass + "-82M' " );
            curSqlStmt.Append( "Where T1.ListName = 'JumpBoatTime3Seg' AND T1.ListCode = '" + inBoatSpeed + "-" + curSkierClass + "-52M' " );
            return getData( curSqlStmt.ToString() );
        }
        
        private DataTable getSpeedByPass( Int16 inMaxSpeed, Int16 inSpeedKph, Decimal inLineLengthKph ) {
            String curListName = "SlalomPass" + ( Convert.ToInt16( inMaxSpeed ) ).ToString();
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCodeNum, CodeValue, MinValue, MaxValue, SortSeq " );
            curSqlStmt.Append( "FROM CodeValueList " );
			curSqlStmt.Append( "WHERE ListName = '" + curListName + "' " );
            curSqlStmt.Append( "AND MaxValue = " + inSpeedKph.ToString( "#0" ) + " " );
			curSqlStmt.Append( "AND MinValue = " + inLineLengthKph.ToString( "#0.00" ) + " " );
            curSqlStmt.Append( " Order by SortSeq " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSlalomBoatTimeResults( String inSanctionId, Int16 inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.SanctionId, S.MemberId, S.AgeGroup, T.SkierName" );
            curSqlStmt.Append( ", E.Event, E.EventGroup, E.TeamCode, E.EventClass" );
            curSqlStmt.Append( ", S.Round, S.MaxSpeed, S.StartSpeed, S.StartLen, S.Score, S.Boat" );
            curSqlStmt.Append( ", R.BoatTime, R.Score AS PassScore, R.TimeInTol, R.ScoreProt, R.Reride" );
			curSqlStmt.Append( ", R.PassLineLength as PassLineLengthKph, PassSpeedKph" );
            curSqlStmt.Append( ", R.RerideReason, R.Note AS PassNotes, S.PK as ScorePK, R.PK as PassPK, R.LastUpdateDate " );
            curSqlStmt.Append( "FROM SlalomScore S " );
            curSqlStmt.Append( "INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round " );
            curSqlStmt.Append( "INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + inSanctionId + "' AND S.Round = " + inRound.ToString() + " AND E.Event = 'Slalom' " );
            curSqlStmt.Append( "ORDER BY S.PK, R.PK " );
            return getData( curSqlStmt.ToString() );
        }

        private String getTimeClass(String inEventClass) {
            String curTimeClass = "C";
            DataRow curClassRow = null;
            DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select("ListCode = '" + inEventClass + "'");
            if ( curRowsFound.Length > 0 ) {
                curClassRow = curRowsFound[0];
            } else {
                curClassRow = mySkierClassList.SkierClassDataTable.Select("ListCode = '" + myTourClass + "'")[0];
            }
            if ( (Decimal)myClassERow["ListCodeNum"] < (Decimal)curClassRow["ListCodeNum"] ) {
                curTimeClass = "R";
            } else if ( (Decimal)myClassCRow["ListCodeNum"] > (Decimal)curClassRow["ListCodeNum"] ) {
                curTimeClass = "W";
            } else {
                curTimeClass = "C";
            }
            return curTimeClass;
        }

        private DataTable getData( String inSelectStmt ) {
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
                        if ( Path.GetExtension( curFileName ) == null ) curFileName += ".prn";
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        private DataTable buildSummaryTable() {
            //Determine number of skiers per placement group
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "Type";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Diff";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0.00;
            curDataTable.Columns.Add( curCol );
            
            curCol = new DataColumn();
            curCol.ColumnName = "Count";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
        }

        private DataTable buildStatsTable() {
            //Determine number of skiers per placement group
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "Type";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "DiffSum";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Count";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "MaxDiff";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "MinDiff";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "ZeroCount";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
        }

    }
}
