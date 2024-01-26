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
    class ExportBoatTimesSlalom {
        private String myTourClass;
        private DataRow myTourRow;
        private DataRow myTimeRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private DataTable myTimesDataTable;
        private ListSkierClass mySkierClassList;
        private ProgressWindow myProgressInfo;

        public ExportBoatTimesSlalom() {
        }

        public Boolean exportBoatTimes( String inSanctionId ) {
            Boolean returnStatus = false, curActiveTour = false;
            char DoubleQuote = '"';
            String curMemberId = "", curEventDateOut = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;
            myProgressInfo = new ProgressWindow();

            DataTable curTourDataTable = getTourData( inSanctionId );
            if ( curTourDataTable != null ) {
                if ( curTourDataTable.Rows.Count > 0 ) {
                    curActiveTour = true;
                    myTourRow = curTourDataTable.Rows[0];
                    myTourClass = myTourRow["Class"].ToString().ToUpper();
                    String curFilename = inSanctionId.Trim() + "ST.csv";
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
                }
            }

            if ( curActiveTour ) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListCode, ListCodeNum, SortSeq, CodeValue, MinValue, MaxValue, CodeDesc ");
                curSqlStmt.Append( "FROM CodeValueList WHERE ListName = 'SlalomBoatTime' ORDER BY SortSeq" );
                myTimesDataTable = getData( curSqlStmt.ToString() );

                mySkierClassList = new ListSkierClass();
                mySkierClassList.ListSkierClassLoad();
                myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];

                DataTable curSpeedByPassDataTable;
                Int16 curMaxSpeed, curSpeedKph, curPassNum = 0, curRound;
                Decimal curBoatTime, curScore, curPassLineLengthKph, curActualTime;
                String curSpeedDesc, curTimeKey, curTimeKeyScore, curEventClass, curRunTime;
                DataRow[] curTimeRowsFound;
                if ( outBuffer != null ) {
                    outLine.Append( "Sanction" );
                    outLine.Append( ", Skier Class" );
                    outLine.Append( ", Boat" );
                    outLine.Append( ", EventDate" );
                    outLine.Append( ", Time" );
                    outLine.Append( ", SkierName" );
                    outLine.Append( ", Div" );
                    outLine.Append( ", MaxSpeed" );
                    outLine.Append( ", Round" );
                    outLine.Append( ", PassNum" );
                    outLine.Append( ", PassDesc" );
                    outLine.Append( ", PassScore" );
                    outLine.Append( ", BoatTime" );
                    outLine.Append( ", ActualTime" );

                    //Write output line to file
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine = new StringBuilder( "" );

                    myProgressInfo.setProgessMsg( "Export slalom boat times " );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();
                    int curRowCount = 0;

                    DataTable curBoatTimeResults = getBoatTimeResults( inSanctionId );
                    if ( curBoatTimeResults != null ) {
                        myProgressInfo.setProgressMax( curBoatTimeResults.Rows.Count );
                        foreach (DataRow curRow in curBoatTimeResults.Rows) {
                            curRowCount++;
                            myProgressInfo.setProgressValue( curRowCount );
                            myProgressInfo.Refresh();

                            try {
                                curBoatTime = (Decimal)curRow["BoatTime"];
                            } catch {
                                curBoatTime = 0;
                            }
                            try {
                                curScore = ( (Decimal)curRow["PassScore"] );
                            } catch {
                                curScore = 0;
                            }
                            try {
                                curRound = ( (Byte)curRow["Round"] );
                            } catch {
                                curRound = 0;
                            }
                            try {
                                curEventClass = ( (String)curRow["EventClass"] );
                            } catch {
                                curEventClass = "";
                            }
                            try {
                                curRunTime = ( (DateTime)curRow["LastUpdateDate"] ).ToString( "hhmm" );
                            } catch {
                                curRunTime = "0000";
                            }

							try {
                                curMaxSpeed = ( (Byte)curRow["MaxSpeed"] );
								try {
									curSpeedKph = ( (Byte) curRow["PassSpeedKph"] );
									curPassLineLengthKph = ( (Decimal) curRow["PassLineLengthKph"] );
								} catch ( Exception ex ) {
									String msg = ex.Message;
									curSpeedKph = 0;
									curPassLineLengthKph = 0;
								}

								curSpeedByPassDataTable = getSpeedByPass( curMaxSpeed, curSpeedKph, curPassLineLengthKph );
                                if ( curSpeedByPassDataTable.Rows.Count > 0 ) {
                                    curSpeedDesc = (String)curSpeedByPassDataTable.Rows[0]["CodeValue"];
									curPassNum = Convert.ToInt16((Decimal)curSpeedByPassDataTable.Rows[0]["ListCodeNum"]);
									curTimeKeyScore = curScore.ToString( "0.00" );
                                    curTimeKey = curSpeedKph.ToString( "00" ) + "-" + getTimeClass( curEventClass ) + "-" + curTimeKeyScore.Substring( 0, 1 );
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

							} catch ( Exception ex ) {
								curActualTime = 0;
								curSpeedKph = 0;
                                curMaxSpeed = 0;
                                curSpeedDesc = "";
                                myTimeRow = null;
                            }

                            outLine.Append( DoubleQuote.ToString() + inSanctionId + myTourClass + DoubleQuote.ToString() ); // 1. (7) SanctionId with class
                            outLine.Append( "," + DoubleQuote.ToString() + curRow["EventClass"].ToString() + DoubleQuote.ToString() ); // 2. (1) Skier Class
                            outLine.Append( "," + DoubleQuote.ToString() + curRow["Boat"].ToString().PadRight( 11, ' ' ) + DoubleQuote.ToString() ); // 3. (11) Boat
                            outLine.Append( "," + DoubleQuote.ToString() + curEventDateOut + DoubleQuote.ToString() ); // 4. (8) Date YYYYMMDD
                            outLine.Append( "," + DoubleQuote.ToString() + curRunTime + DoubleQuote.ToString() ); // 5. (4) Time HHMM
                            outLine.Append( "," + DoubleQuote.ToString() + curRow["SkierName"].ToString().PadRight( 22, ' ' ) + DoubleQuote.ToString() ); // 6. (22) Skier Name
                            outLine.Append( "," + DoubleQuote.ToString() + curRow["AgeGroup"].ToString() + DoubleQuote.ToString() ); // 7. (2) Age Division
                            outLine.Append( "," + DoubleQuote.ToString() + curMaxSpeed.ToString( "#0" ).PadLeft( 2, ' ' ) + DoubleQuote.ToString() ); // 8. (2) Max Speed for division
                            outLine.Append( "," + DoubleQuote.ToString() + curRound.ToString( "#0" ).PadLeft( 2, ' ' ) + DoubleQuote.ToString() ); // 9. (2) Round
                            outLine.Append( "," + DoubleQuote.ToString() + curPassNum.ToString( "#0" ).PadLeft( 2, ' ' ) + DoubleQuote.ToString() ); // 10. (2) Pass Number
                            outLine.Append( "," + DoubleQuote.ToString() + curSpeedDesc.PadRight( 28, ' ' ) + DoubleQuote.ToString() ); // 11. (28) Boat Speed Desc
                            outLine.Append( "," + DoubleQuote.ToString() + curScore.ToString( "#.00" ).PadLeft( 4, ' ' ) + DoubleQuote.ToString() ); //10. (4) Score for Pass
                            outLine.Append( "," + curBoatTime.ToString( "##.00" ) ); //12. (5 Num) Boat Time
                            outLine.Append( "," + curActualTime.ToString( "##.00" ) ); //13. (5 Num) Actual Time

                            //Write output line to file
                            outBuffer.WriteLine( outLine.ToString() );

                            //Initialize output buffer
                            outLine = new StringBuilder( "" );
                        }
                        myProgressInfo.Close();
                    }

                    returnStatus = true;
                    outBuffer.Close();

                    if ( curBoatTimeResults.Rows.Count > 0 ) {
                        MessageBox.Show( curBoatTimeResults.Rows.Count + " rows found and written" );
                    } else {
                        MessageBox.Show( "No rows found" );
                    }
                }
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

		private DataTable getBoatTimeResults( String inSanctionId ) {
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
            curSqlStmt.Append( " WHERE S.SanctionId = '" + inSanctionId + "' AND E.Event = 'Slalom' " );
            curSqlStmt.Append( " ORDER BY S.SanctionId, S.Round, R.LastUpdateDate, S.PK, R.PK " );
            return getData( curSqlStmt.ToString() );
		}

		private String getTimeClass( String inEventClass ) {
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
