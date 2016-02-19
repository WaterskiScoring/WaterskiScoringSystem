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
    class ExportScorebookPublishFmt {
        private int myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private String myTourClass;
        private DataRow myTourRow;
        private DataTable myAgeDivDataTable;
        private ProgressWindow myProgressInfo;

        public ExportScorebookPublishFmt() {
            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                    DataTable curTourDataTable = getTourData( mySanctionNum );
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        myTourClass = myTourRow["Class"].ToString().Trim();

                        int curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
                        try {
                            curSlalomRounds = Convert.ToInt16( myTourRow["SlalomRounds"].ToString() );
                        } catch {
                            curSlalomRounds = 0;
                        }
                        try {
                            curTrickRounds = Convert.ToInt16( myTourRow["TrickRounds"].ToString() );
                        } catch {
                            curTrickRounds = 0;
                        }
                        try {
                            curJumpRounds = Convert.ToInt16( myTourRow["JumpRounds"].ToString() );
                        } catch {
                            curJumpRounds = 0;
                        }
                        if ( curSlalomRounds > myTourRounds ) { myTourRounds = curSlalomRounds; }
                        if ( curTrickRounds > myTourRounds ) { myTourRounds = curTrickRounds; }
                        if ( curJumpRounds > myTourRounds ) { myTourRounds = curJumpRounds; }

                        AgeGroupDropdownList curAgeGroupDropdownList = new AgeGroupDropdownList( myTourRow );
                        myAgeDivDataTable = curAgeGroupDropdownList.AgeDivDataTable;

                    }
                }
            }
        }

        public Boolean ExportScorebookPublishFmtData() {
            String curMethodName = "ExportScorebookPublishFmtData";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curMsg = "", curMemberId = "", curAgeGroup = "", prevAgeGroup = "";
            String curFileFilter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";
            Int16 curRound = 0, curOverallPlcmt = 0;
            DataRow prevRow = null;
            DataRow[] curScoreSlalomRows = null, curScoreTrickRows = null, curScoreJumpRows = null;

            try {
                curMsg = "Exporting Scorebook Publish Data";
                Log.WriteFile( curMethodName + ":begin: " + curMsg );
                String curFilename = mySanctionNum.Trim() + "-publish.txt";
                StreamWriter outBuffer = getExportFile( curFileFilter, curFilename );

                if ( outBuffer == null ) {
                    curMsg = "Output file not available";
                } else {
                    String curPlcmtMethod = "score"
                        , curPlcmtOverallOrg = "agegroup"
                        , curDataType = "best"
                        , curPointsMethod = "nops"
                        , curPlcmtOrg = "div";

                    myProgressInfo = new ProgressWindow();
                    myProgressInfo.setProgessMsg( "Processing Scorebook Publish Data" );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();
                    myProgressInfo.setProgressMax( 10 );

                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();
                    DataTable mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myProgressInfo.setProgressValue( 1 );
                    myProgressInfo.Refresh();
                    DataTable myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myProgressInfo.setProgressValue( 2 );
                    myProgressInfo.Refresh();
                    DataTable myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myProgressInfo.setProgressValue( 3 );
                    myProgressInfo.Refresh();
                    DataTable mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOverallOrg );
                    foreach ( DataRow curRow in mySummaryDataTable.Rows ) {
                        if ( ( (String)curRow["QualifyOverall"] ).ToUpper().Equals( "YES" ) ) {
                        } else {
                            curRow["ScoreOverall"] = 0;
                        }
                    }
                    myProgressInfo.setProgressValue( 4 );
                    myProgressInfo.Refresh();

                    mySummaryDataTable.DefaultView.Sort = "AgeGroup ASC, QualifyOverall Desc, ScoreOverall Desc, SkierName ASC";
                    DataTable curSummaryDataTable = mySummaryDataTable.DefaultView.ToTable();

                    myProgressInfo.setProgressMax( mySummaryDataTable.Rows.Count );
                    myProgressInfo.Refresh();

                    //Build file header line and write to file
                    writeHeader( outBuffer );

                    int curRowCount = 0;
                    foreach ( DataRow curRow in curSummaryDataTable.Rows ) {
                        curRowCount++;
                        myProgressInfo.setProgressValue( curRowCount );
                        myProgressInfo.Refresh();

                        curMemberId = curRow["MemberId"].ToString();
                        curAgeGroup = curRow["AgeGroup"].ToString();
                        if ( curAgeGroup != prevAgeGroup ) {
                            if ( prevAgeGroup.Length > 0 ) {
                                outBuffer.WriteLine( "" );
                                curOverallPlcmt = 0;
                            }

                            //Write skier identification information
                            outLine = new StringBuilder( "" );
                            outLine.Append( writeDivisionHeader( outBuffer, curAgeGroup ) );
                        }

                        curScoreSlalomRows = mySlalomDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        curScoreTrickRows = myTrickDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        curScoreJumpRows = myJumpDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );

                        //Initialize control fields
                        prevAgeGroup = curAgeGroup;

                        //Initialize output buffer
                        outLine = new StringBuilder( "" );

                        //Write skier identification information
                        outLine.Append( writeSkierInfo( curRow, curRound ) );

                        //Write skier performance summary information
                        outLine.Append( writeSkierSlalomScore( curRow, curScoreSlalomRows ) );

                        outLine.Append( writeSkierTrickScore( curRow, curScoreTrickRows ) );

                        outLine.Append( writeSkierJumpScore( curRow, curScoreJumpRows ) );

                        curOverallPlcmt++;
                        outLine.Append( writeSkierOverallScore( curRow, curOverallPlcmt ) );
                        //Write output line to file
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    //Build file footer and write to file
                    outLine = new StringBuilder( "" );
                    outBuffer.WriteLine( outLine.ToString() );

                    returnStatus = true;
                    outBuffer.Close();

                    myProgressInfo.Close();
                    if ( mySummaryDataTable.Rows.Count > 0 ) {
                        curMsg = mySummaryDataTable.Rows.Count + " skiers found and written";
                    } else {
                        curMsg = "No rows found";
                    }
                }
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + ":conplete: " + curMsg );
            } catch ( Exception ex ) {
                MessageBox.Show( "Error:" + curMethodName + " Could not write file from data input\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
            }

            return returnStatus;
        }

        //Write report identification information
        private Boolean writeHeader( StreamWriter outBuffer ) {
            StringBuilder outLine = new StringBuilder( "" );

            outLine.Append( myTourRow["Name"].ToString().ToUpper().PadRight( 28, ' ' ) );
            outLine.Append( " SLALOM       TRICKS   JUMPING   OVERALL  " );
            outBuffer.WriteLine( outLine.ToString() );

            return true;
        }

        //Write report identification information
        private Boolean writeDivisionHeader( StreamWriter outBuffer, String curAgeGroup ) {
            String curDivName = "";
            StringBuilder outLine = new StringBuilder( "" );
            DataRow[] curFindRows;

            curFindRows = myAgeDivDataTable.Select( "Division = '" + curAgeGroup + "'" );
            if ( curFindRows.Length > 0 ) {
                curDivName = (String)curFindRows[0]["DivisionName"];
            } else {
                curDivName = curAgeGroup;
            }

            outLine.Append( curDivName.PadRight( 20, ' ' ) );
            outLine.Append( " ST   BYS/PASS PLC    PTS PLC   FT PLC  POINTS PLC" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "-------------------- --  ----/---- ---   ---- ---  --- ---  ------ ---" );
            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private String writeSkierInfo( DataRow inRow, int inRound ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curTeamCode = "";

            try {
                outLine.Append( inRow["SkierName"].ToString().PadRight( 20, ' ' ) );
                if ( isObjectEmpty( inRow["TeamSlalom"] ) ) {
                } else {
                    if ( inRow["TeamSlalom"].ToString().Length > 0 ) {
                        curTeamCode = inRow["TeamSlalom"].ToString();
                    }
                }
                if ( curTeamCode.Length == 0 ) {
                    if ( isObjectEmpty( inRow["TeamTrick"] ) ) {
                    } else {
                        if ( inRow["TeamTrick"].ToString().Length > 0 ) {
                            curTeamCode = inRow["TeamTrick"].ToString();
                        }
                    }
                }
                if ( curTeamCode.Length == 0 ) {
                    if ( isObjectEmpty( inRow["TeamJump"] ) ) {
                    } else {
                        if ( inRow["TeamJump"].ToString().Length > 0 ) {
                            curTeamCode = inRow["TeamJump"].ToString();
                        }
                    }
                }
                outLine.Append( curTeamCode.PadLeft( 3, ' ' ) );
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing skier info to Scorebook: " + "\n\nError: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierSlalomScore( DataRow inScoreSummary, DataRow[] inEventScore ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "", curValue = "";
            Int16 curRound = 0;

            outLine.Append( curEmptyValue.PadRight( 2, ' ' ) );
            bool isRoundScored = true;
            try {
                if ( inScoreSummary == null ) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundSlalom"];
                        curValue = (String)inScoreSummary["EventClassSlalom"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        if ( curValue.Length == 0 ) {
                            curValue = " ";
                            isRoundScored = false;
                        }
                    } catch {
                        isRoundScored = false;
                    }
                    try {
                        curRound = (Int16)inScoreSummary["RoundOverall"];
                    } catch {
                    }
                }
                if ( isRoundScored ) {
                    Decimal curSlalomScore = 0;
                    try {
                        curSlalomScore = (Decimal)inScoreSummary["ScoreSlalom"];
                    } catch {
                        curSlalomScore = 0M;
                    }
                    try {
                        try {
                            outLine.Append( ( (Decimal)inScoreSummary["FinalPassScore"] ).ToString( "0.00" ) );
                        } catch {
                            outLine.Append( "    " );
                        }
                        if ( curSlalomScore < 6 ) {
                            outLine.Append( "/2300" );
                        } else {
                            try {
                                Decimal curRopeLen = ( Convert.ToDecimal( (String)inScoreSummary["FinalLen"] ) * 100 );
                                if ( curRopeLen == 2300 ) {
                                    try {
                                        outLine.Append( "/" + ( (Byte)inScoreSummary["FinalSpeedKph"] ).ToString( "00" ) + "-K" );
                                    } catch {
                                        outLine.Append( "25-K" );
                                    }
                                } else {
                                    outLine.Append( "/" + curRopeLen.ToString( "0000" ) );
                                }
                            } catch {
                                outLine.Append( "2300" );
                            }
                        }
                        if ( inEventScore.Length == 0 ) {
                            outLine.Append( curEmptyValue.PadRight( 4, ' ' ) );
                        } else {
                            String curPlcmt = (String)inEventScore[0]["PlcmtSlalom"];
                            if ( curPlcmt.Contains( "T" ) ) {
                                curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                            } else {
                                curPlcmt = curPlcmt.Trim() + " ";
                            }
                            outLine.Append( " " + curPlcmt.PadLeft( 4, ' ' ) );
                        }
                    } catch ( Exception ex ) {
                        outLine.Append( "---- ----".PadRight( 14, ' ' ) );
                    }
                } else {
                    outLine.Append( "---- ----".PadRight( 14, ' ' ) );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing slalom scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Slalom Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierTrickScore( DataRow inScoreSummary, DataRow[] inEventScore ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "", curValue = "";
            Int16 curRound = 0;

            outLine.Append( curEmptyValue.PadRight( 1, ' ' ) );
            bool isRoundScored = true;
            try {
                if ( inScoreSummary == null ) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundTrick"];
                        curValue = (String)inScoreSummary["EventClassTrick"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        if ( curValue.Length == 0 ) {
                            curValue = " ";
                            isRoundScored = false;
                        }
                    } catch {
                        isRoundScored = false;
                    }
                    try {
                        curRound = (Int16)inScoreSummary["RoundOverall"];
                    } catch {
                    }
                }
                if ( isRoundScored ) {
                    try {
                        if ( isObjectEmpty( inScoreSummary["ScoreTrick"] ) ) {
                            outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                        } else {
                            outLine.Append( ( (Int16)inScoreSummary["ScoreTrick"] ).ToString( "####0" ).PadLeft( 5, ' ' ) );
                        }
                    } catch {
                        outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                    }
                    String curPlcmt = (String)inEventScore[0]["PlcmtTrick"];
                    if ( curPlcmt.Contains( "T" ) ) {
                        curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                    } else {
                        curPlcmt = curPlcmt.Trim() + " ";
                    }
                    outLine.Append( " " + curPlcmt.PadLeft( 4, ' ' ));

                } else {
                    outLine.Append( " ----".PadRight( 10, ' ' ) );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing trick scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Trick Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierJumpScore( DataRow inScoreSummary, DataRow[] inEventScore ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "", curValue = "";
            Int16 curRound = 0;

            outLine.Append( curEmptyValue.PadRight( 1, ' ' ) );
            bool isRoundScored = true;
            try {
                if ( inScoreSummary == null ) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundJump"];
                        curValue = (String)inScoreSummary["EventClassJump"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        if ( curValue.Length == 0 ) {
                            curValue = " ";
                            isRoundScored = false;
                        }
                    } catch {
                        isRoundScored = false;
                    }
                    try {
                        curRound = (Int16)inScoreSummary["RoundOverall"];
                    } catch {
                    }
                }
                if ( isRoundScored ) {
                    try {
                        if ( isObjectEmpty( inScoreSummary["ScoreFeet"] ) ) {
                            outLine.Append( curEmptyValue.PadRight( 3, ' ' ) );
                        } else {
                            outLine.Append( ( (Decimal)inScoreSummary["ScoreFeet"] ).ToString( "##0" ).PadLeft( 3, ' ' ) );
                        }
                    } catch {
                        outLine.Append( curEmptyValue.PadRight( 3, ' ' ) );
                    }
                    String curPlcmt = (String)inEventScore[0]["PlcmtJump"];
                    if ( curPlcmt.Contains( "T" ) ) {
                        curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                    } else {
                        curPlcmt = curPlcmt.Trim() + " ";
                    }
                    outLine.Append( " " + curPlcmt.PadLeft( 4, ' ' ) );
                } else {
                    outLine.Append( "---".PadRight( 8, ' ' ) );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing jump scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Jump Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierOverallScore( DataRow inSummaryRow, Int16 inOverallPlcmt ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "", curValue = "";
            Decimal curNopsOverall = 0;

            outLine.Append( curEmptyValue.PadRight( 1, ' ' ) );
            try {
                if ( ( (String)inSummaryRow["QualifyOverall"] ).ToLower().Equals( "yes" ) ) {
                    if ( isObjectEmpty( inSummaryRow["ScoreOverall"] ) ) {
                        outLine.Append( curEmptyValue.PadRight( 11, ' ' ) );
                    } else {
                        outLine.Append( ( (Decimal)inSummaryRow["ScoreOverall"] ).ToString( "###0.0" ).PadLeft( 6, ' ' ) );
                        String curPlcmt = inOverallPlcmt.ToString("##0");
                        if ( curPlcmt.Contains( "T" ) ) {
                            curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                        } else {
                            curPlcmt = curPlcmt.Trim() + " ";
                        }
                        outLine.Append( " " + curPlcmt.PadLeft( 4, ' ' ) );
                    }
                    
                } else {
                    outLine.Append( curEmptyValue.PadRight( 11, ' ' ) );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing overall scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Overall Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private StreamWriter getExportFile( String inFileFilter, String inFileName ) {
            String curMethodName = "getExportFile";
            String myFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            if ( inFileFilter == null ) {
                curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else {
                curFileFilter = inFileFilter;
            }

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = curFileFilter;
            myFileDialog.FilterIndex = 1;
            if ( inFileName == null ) {
                myFileDialog.FileName = "";
            } else {
                myFileDialog.FileName = inFileName;
            }

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if ( curFileName.IndexOf( '.' ) < 0 ) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split( '|' );
                            if ( curList.Length > 0 ) {
                                int curDelim = curList[1].IndexOf( "." );
                                if ( curDelim > 0 ) {
                                    curDefaultExt = curList[1].Substring( curDelim - 1 );
                                }
                            }
                            myFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return outBuffer;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactMemberId, ContactAddress, ContactPhone, ContactEmail, CP.SkierName AS ContactName " );
            curSqlStmt.Append( ", ChiefJudgeMemberId, ChiefJudgeAddress, ChiefJudgePhone, ChiefJudgeEmail, CJ.SkierName AS ChiefJudgeName" );
            curSqlStmt.Append( ", ChiefDriverMemberId, ChiefDriverAddress, ChiefDriverPhone, ChiefDriverEmail, CD.SkierName AS ChiefDriverName " );
            curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirAddress, SafetyDirPhone, SafetyDirEmail, SD.SkierName AS SafetyDirName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CP " );
            curSqlStmt.Append( "    ON CP.SanctionId = T.SanctionId AND CP.MemberId = T.ContactMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CJ " );
            curSqlStmt.Append( "    ON CJ.SanctionId = T.SanctionId AND CJ.MemberId = T.ChiefJudgeMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CD " );
            curSqlStmt.Append( "    ON CD.SanctionId = T.SanctionId AND CD.MemberId = T.ChiefDriverMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) SD " );
            curSqlStmt.Append( "    ON SD.SanctionId = T.SanctionId AND SD.MemberId = T.SafetyDirMemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getMemberData( String inSanctionId ) {
            String selectStmt = "SELECT T.SanctionId, Federation, T.MemberId, SkierName, "
                + "Gender, SkiYearAge, State, ReadyToSki, L.CodeValue AS Region, "
                + "RS.TeamCode AS SlalomTeam, RT.TeamCode AS TrickTeam, RJ.TeamCode AS JumpTeam, "
                + "RS.AgeGroup AS SlalomGroup, RT.AgeGroup AS TrickGroup, RJ.AgeGroup AS JumpGroup, "
                + "RS.EventClass AS SlalomClass, RT.EventClass AS TrickClass, RJ.EventClass AS JumpClass, "
                + "T.AgeGroup as AgeGroup, RS.AgeGroup AS SlalomAgeGroup, RT.AgeGroup AS TrickAgeGroup, RJ.AgeGroup AS JumpAgeGroup "
                + "FROM TourReg AS T "
                + "  LEFT OUTER JOIN EventReg AS RS ON T.SanctionId = RS.SanctionId AND T.MemberId = RS.MemberId AND T.AgeGroup = RS.AgeGroup AND RS.Event = 'Slalom'"
                + "  LEFT OUTER JOIN EventReg AS RT ON T.SanctionId = RT.SanctionId AND T.MemberId = RT.MemberId AND T.AgeGroup = RT.AgeGroup AND RT.Event = 'Trick'"
                + "  LEFT OUTER JOIN EventReg AS RJ ON T.SanctionId = RJ.SanctionId AND T.MemberId = RJ.MemberId AND T.AgeGroup = RJ.AgeGroup AND RJ.Event = 'Jump'"
                + "  LEFT OUTER JOIN CodeValueList AS L ON ListName = 'StateRegion' AND ListCode = State "
                + "WHERE (T.SanctionId = '" + inSanctionId + "')"
                + "ORDER BY SkierName, T.AgeGroup"
            ;
            DataTable curDataTable = getData( selectStmt );
            return curDataTable;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }
    }
}
