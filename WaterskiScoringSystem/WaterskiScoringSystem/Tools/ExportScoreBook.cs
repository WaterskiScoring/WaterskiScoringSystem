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
    class ExportScoreBook {
        private int myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private String myTourClass;
        private DataRow myTourRow;
        private ProgressWindow myProgressInfo;
        private ListSkierClass mySkierClassList;
        private TourProperties myTourProperties;
        private DataTable mySummaryDataTable = null;

        public ExportScoreBook() {
            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                    mySkierClassList = new ListSkierClass();
                    mySkierClassList.ListSkierClassLoad();
                    
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
                        if ( myTourRounds == 1 ) { myTourRounds = 2; }
                    }
                }
            }
        }

        public Boolean exportScoreBookData() {
            String curMethodName = "exportScoreBookData";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curMsg = "", curMemberId = "", curAgeGroup = "", prevMemberId = "", prevAgeGroup = "", curReadyToSki = "";
            String curFileFilter = "SBK files (*.sbk)|*.sbk|All files (*.*)|*.*";
            Int16 curRound = 0;
            DataRow prevRow = null;
            DataRow[] curScoreRows = null;
            myTourProperties = TourProperties.Instance;

            try {
                curMsg = "Exporting Scorebook";
                Log.WriteFile( curMethodName + ":begin: " + curMsg );
                String curFilename = mySanctionNum.Trim() + myTourClass + ".sbk";
                StreamWriter outBuffer = getExportFile( curFileFilter, curFilename );

                if (outBuffer == null) {
                    curMsg = "Output file not available";
                } else {
                    String curPlcmtMethod = "score", curPlcmtOverallOrg = "agegroup";
                    String curDataType = myTourProperties.MasterSummaryDataType;
                    //String curDataType = Properties.Settings.Default.MasterSummaryV2DataType;
                    if ( curDataType.ToLower().Equals( "total" )
                        || curDataType.ToLower().Equals( "best" )
                        || curDataType.ToLower().Equals( "final" )
                        || curDataType.ToLower().Equals( "first" ) ) {
                    } else {
                        curDataType = "best";
                    }
                    String curPointsMethod = myTourProperties.MasterSummaryPointsMethod;
                    //String curPointsMethod = Properties.Settings.Default.MasterSummaryV2PointsMethod;
                    if ( curPointsMethod.ToLower().Equals( "nops" )
                        || curPointsMethod.ToLower().Equals( "plcmt" )
                        || curPointsMethod.ToLower().Equals( "kbase" )
                        || curPointsMethod.ToLower().Equals( "ratio" ) ) {
                    } else {
                        curPointsMethod = "nops";
                    }
                    String curPlcmtOrg = myTourProperties.MasterSummaryPlcmtOrg;
                    //String curPlcmtOrg = Properties.Settings.Default.MasterSummaryV2PlcmtOrg;
                    if ( curPlcmtOrg.ToLower().Equals( "div" ) ) {
                        curPlcmtOverallOrg = "agegroup";
                    } else if ( curPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                        curPlcmtOverallOrg = "agegroupgroup";
                    } else {
                        curPlcmtOverallOrg = "agegroup";
                        curPlcmtOrg = "div";
                    }

                    myProgressInfo = new ProgressWindow();
                    myProgressInfo.setProgessMsg( "Processing Scorebook" );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();
                    myProgressInfo.setProgressMax( 10 );

                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();
                    mySummaryDataTable = null;

                    DataTable myMemberData = curCalcSummary.getMemberData( mySanctionNum );
                    myProgressInfo.setProgressValue( 1 );
                    myProgressInfo.Refresh();

                    if (myTourRules.ToLower().Equals( "iwwf" )) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 2 );
                        myProgressInfo.Refresh();

                        DataTable mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 6 );
                        myProgressInfo.Refresh();
                        DataTable myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 7 );
                        myProgressInfo.Refresh();
                        DataTable myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 8 );
                        myProgressInfo.Refresh();
                        DataTable newSummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                        mySummaryDataTable = newSummaryDataTable;
                        myProgressInfo.setProgressValue( 9 );
                        myProgressInfo.Refresh();

                    } else {
                        DataTable mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 1 );
                        myProgressInfo.Refresh();
                        DataTable myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 2 );
                        myProgressInfo.Refresh();
                        DataTable myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 3 );
                        myProgressInfo.Refresh();
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOverallOrg );
                        myProgressInfo.setProgressValue( 4 );
                        myProgressInfo.Refresh();

                        DataTable mySlalomDetail = curCalcSummary.getSlalomScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 6 );
                        myProgressInfo.Refresh();
                        DataTable myTrickDetail = curCalcSummary.getTrickScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 7 );
                        myProgressInfo.Refresh();
                        DataTable myJumpDetail = curCalcSummary.getJumpScoreDetail( myTourRow, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 8 );
                        myProgressInfo.Refresh();

                        mySummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                        myProgressInfo.setProgressValue( 9 );
                        myProgressInfo.Refresh();
                    }

                    myProgressInfo.setProgressMax( mySummaryDataTable.Rows.Count );
                    myProgressInfo.Refresh();

                    //Build file header line and write to file
                    writeHeader( outBuffer );

                    int curRowCount = 0;
                    foreach ( DataRow curRow in mySummaryDataTable.Rows ) {
                        curRowCount++;
                        myProgressInfo.setProgressValue( curRowCount );
                        myProgressInfo.Refresh();

                        curMemberId = curRow["MemberId"].ToString();
                        curAgeGroup = curRow["AgeGroup"].ToString();
                        curReadyToSki = curRow["ReadyToSki"].ToString();
                        if ( curMemberId != prevMemberId || curAgeGroup != prevAgeGroup ) {
                            outLine = new StringBuilder( "" );
                            outBuffer.WriteLine( outLine.ToString() );
                            curScoreRows = mySummaryDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        }

                        //Initialize control fields
                        prevMemberId = curMemberId;
                        prevAgeGroup = curAgeGroup;
                        curRound = (Int16)curRow["RoundOverall"];

                        //Initialize output buffer
                        outLine = new StringBuilder( "" );

                        //Write skier identification information
                        outLine.Append( writeSkierInfo( curRow, curRound ) );

                        //Write skier performance summary information
                        outLine.Append( writeSkierSlalomScore( curRow, curScoreRows ) );

                        outLine.Append( writeSkierTrickScore( curRow, curScoreRows ) );

                        outLine.Append( writeSkierJumpScore( curRow, curScoreRows ) );

                        outLine.Append( writeSkierOverallScore( curRow ) );
                        //Write output line to file
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    //Build file footer and write to file
                    outLine = new StringBuilder( "" );
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine.Append( writeFooter() );
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
                Log.WriteFile( curMethodName + ":complete: " + curMsg );
            } catch ( Exception ex ) {
                MessageBox.Show( "Error:" + curMethodName + " Could not write file from DataGridView\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
            }

            return returnStatus;
        }

        public Boolean exportScoreBookDataXClass() {
            String curMethodName = "exportScoreBookDataXClass";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curMsg = "", curMemberId = "", curAgeGroup = "", prevMemberId = "", prevAgeGroup = "", curReadyToSki = "";
            String curFileFilter = "SBK files (*.sbk)|*.sbk|All files (*.*)|*.*";
            Int16 curRound = 0;
            DataRow[] curScoreRows = null;

            try {
                curMsg = "Exporting Scorebook";
                Log.WriteFile( curMethodName + ":begin: " + curMsg );
                String curFilename = mySanctionNum.Trim() + myTourClass + "X.sbk";
                StreamWriter outBuffer = getExportFile( curFileFilter, curFilename );

                if (outBuffer == null) {
                    curMsg = "Output file not available";
                } else {
                    //Build file header line and write to file
                    writeHeader( outBuffer );

                    int curRowCount = 0;
                    foreach ( DataRow curRow in mySummaryDataTable.Rows ) {
                        curRowCount++;
                        myProgressInfo.setProgressValue( curRowCount );
                        myProgressInfo.Refresh();

                        curMemberId = curRow["MemberId"].ToString();
                        curAgeGroup = curRow["AgeGroup"].ToString();
                        curReadyToSki = curRow["ReadyToSki"].ToString();
                        if ( curMemberId != prevMemberId || curAgeGroup != prevAgeGroup ) {
                            if (curScoreRows != null && curScoreRows.Length > 0) {
                                outLine = new StringBuilder( "" );
                                outBuffer.WriteLine( outLine.ToString() );
                            }
                            curScoreRows = mySummaryDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "' "
                            + "AND (EventClassSlalom = 'X' OR EventClassTrick = 'X' OR EventClassJump = 'X')" );
                        }

                        //Initialize control fields
                        prevMemberId = curMemberId;
                        prevAgeGroup = curAgeGroup;
                        curRound = (Int16)curRow["RoundOverall"];

                        if (curScoreRows.Length > 0 ) {
                            //Initialize output buffer
                            outLine = new StringBuilder( "" );

                            //Write skier identification information
                            outLine.Append( writeSkierInfo( curRow, curRound ) );

                            //Write skier performance summary information
                            outLine.Append( writeSkierSlalomScore( curRow, curScoreRows ) );

                            outLine.Append( writeSkierTrickScore( curRow, curScoreRows ) );

                            outLine.Append( writeSkierJumpScore( curRow, curScoreRows ) );

                            outLine.Append( writeSkierOverallScore( curRow ) );
                            //Write output line to file
                            outBuffer.WriteLine( outLine.ToString() );
                        }
                    }

                    //Build file footer and write to file
                    outLine = new StringBuilder( "" );
                    outBuffer.WriteLine( outLine.ToString() );
                    outLine.Append( writeFooter() );
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
                Log.WriteFile( curMethodName + ":complete: " + curMsg );
            } catch (Exception ex) {
                MessageBox.Show( "Error:" + curMethodName + " Could not write file from DataGridView\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
            }

            return returnStatus;
        }

        //Write report identification information
        private Boolean writeHeader( StreamWriter outBuffer ) {
            StringBuilder outLine = new StringBuilder( "" );

            outLine.Append( writeHeaderLine1() );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
            outLine.Append( writeHeaderLine2() );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
            outLine.Append( writeHeaderLine3() );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );
            outLine.Append( writeHeaderLine4() );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );

            return true;
        }

        private String writeHeaderLine1(  ) {
            String curEmptyString = "";
            
            StringBuilder curHeaderLine = new StringBuilder( "" );
            curHeaderLine.Append( curEmptyString.PadRight( 7, ' ' ) );
            curHeaderLine.Append( mySanctionNum.ToUpper().Trim() + myTourRow["Class"].ToString().Trim() );
            curHeaderLine.Append( curEmptyString.PadRight( 10, ' ' ) );
            curHeaderLine.Append( myTourRow["Name"].ToString().ToUpper().PadRight( 29, ' ' ) );
            curHeaderLine.Append( " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion + " Master Scorebook ");
            return curHeaderLine.ToString();
        }

        private String writeHeaderLine2() {
            String curEmptyString = "";
            StringBuilder curHeaderLine = new StringBuilder( "" );
            curHeaderLine.Append( curEmptyString.PadRight( 7, ' ' ) );
            curHeaderLine.Append( "DETAILS OF RESULTS AND PLACEMENTS " );
            curHeaderLine.Append( " AS OF " + DateTime.Now.ToString( "MM/dd/yyyy HH:mm" ) );
            return curHeaderLine.ToString();
        }

        private String writeHeaderLine3() {
            StringBuilder curHeaderLine = new StringBuilder( "" );
            curHeaderLine.Append( "                            ---------SLALOM----------  --------TRICKS--------  ---------JUMPING---------  ---------OVERALL---------" );
            return curHeaderLine.ToString();
        }

        private String writeHeaderLine4() {
            StringBuilder curHeaderLine = new StringBuilder( "" );
            curHeaderLine.Append( "COMPETITOR NAME MEM# DV TM  LST SP LINE TOTAL CR G/PL  P#1  P#2 TOTAL CR G/PL H SP  B/U  MTR  FT CR G/PL  SLM  TRK  JMP  TOTAL RTG" );
            return curHeaderLine.ToString();
        }

        private String writeFooter() {
            StringBuilder curHeaderLine = new StringBuilder( "" );
            curHeaderLine.Append( "******************* " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion + " ****************** " );
            return curHeaderLine.ToString();
        }

        private String writeSkierInfo( DataRow inRow, int inRound ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "";
            
            try {
                if ( inRound == 1 ) {
                    outLine.Append( inRow["SkierName"].ToString().PadRight( 21, ' ' ) );
                    outLine.Append( inRow["AgeGroup"].ToString() );
                    outLine.Append( curEmptyValue.PadRight( 4, ' ' ) );
                } else if ( inRound == 2 ) {
                    String curMemberId = (String)inRow["MemberId"];
                    String curMemberIdFmt = curMemberId.Substring( 0, 3 ) + "-" + curMemberId.Substring( 3, 2 ) + "-" + curMemberId.Substring( 5, 4 );
                    outLine.Append( curEmptyValue.PadRight( 9, ' ' ) );
                    outLine.Append( curMemberIdFmt.PadRight( 18, ' ' ) );
                } else {
                    outLine.Append( curEmptyValue.PadRight( 27, ' ' ) );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing skier info to Scorebook: " + "\n\nError: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierSlalomScore( DataRow inScoreSummary, DataRow [] inEventScore ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEmptyValue = "", curValue = "";
            Int16 curRound = 0;

            bool isRoundScored = true;
            try {
                if ( inScoreSummary == null) {
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

                            if (myTourRules.ToLower().Equals( "iwwf" )) {
                                outLine.Append( "," + getIwwfSlalomMin( (String)inScoreSummary["AgeGroup"] ) );
                                outLine.Append( "1825" );
                            } else if (( mySkierClassList.compareClassChange( (String)inScoreSummary["EventClassSlalom"], "L" ) <= 0 )) {
                                outLine.Append( " 25" );
                                outLine.Append( "1825" );
                            } else {
                                outLine.Append( " 25" );
                                outLine.Append( "2300" );
                            }
                        } else {
                            try {
                                outLine.Append( " " + ( (Byte)inScoreSummary["FinalSpeedKph"] ).ToString( "00" ) );
                            } catch {
                                outLine.Append( " " + "25" );
                            }
                            try {
                                outLine.Append( ( Convert.ToDecimal( (String)inScoreSummary["FinalLen"] ) * 100 ).ToString( "0000" ) );
                            } catch {
                                outLine.Append( "2300" );
                            }
                        }
                    } catch ( Exception ex ) {
                        outLine.Append( "    " );
                        outLine.Append( " " + "25" );
                        outLine.Append( "2300" );
                    }
                    try {
                        outLine.Append( " " + ( (Decimal)inScoreSummary["ScoreSlalom"] ).ToString( "##0.00" ).PadLeft( 6, ' ' ) );
                    } catch {
                        outLine.Append( "       " );
                    }
                    try {
                        curValue = (String)inScoreSummary["EventClassSlalom"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        outLine.Append( " " + curValue );
                    } catch {
                        outLine.Append( "  " );
                    }
                    outLine.Append( " " ); //Ratings no longer used

                    if ( curRound == 1 ) {
                        curValue = (String)inScoreSummary["AgeGroup"];
                        if ( curValue.Length > 0 ) {
                            if ( curValue.Length == 2 ) {
                                outLine.Append( " " + curValue + "   " );
                            } else if ( curValue.Length > 2 ) {
                                outLine.Append( " " + curValue.Substring(0,2) + "   " );
                            } else {
                                outLine.Append( "  " + curValue + "   " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        }
                    } else if ( curRound == 2 ) {
                        if ( ((String) inEventScore[0]["PlcmtSlalom"]).Length == 0 ) {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        } else {
                            String curPlcmt = (String)inEventScore[0]["PlcmtSlalom"];
                            if ( curPlcmt.Contains( "T" ) ) {
                                curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                            } else if ( curPlcmt.Trim().Equals("999") ) {
                                curPlcmt = "**";
                            }
                            outLine.Append( " " + curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                        }
                    } else {
                        outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                    }
                } else {
                    outLine.Append( "---- ------ ------    " );
                    if ( inScoreSummary == null || inEventScore.Length == 0 ) {
                        outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                    } else {
                        if ( curRound == 1 ) {
                            curValue = (String)inScoreSummary["AgeGroup"];
                            if ( curValue.Length > 0 ) {
                                if ( curValue.Length == 2 ) {
                                    outLine.Append( curValue + "   " );
                                } else if ( curValue.Length > 2 ) {
                                    outLine.Append( curValue.Substring( 0, 2 ) + "   " );
                                } else {
                                    outLine.Append( " " + curValue + "   " );
                                }
                            } else {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            }
                        } else if ( curRound == 2 ) {
                            if ( inEventScore.Length == 0 ) {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            } else {
                                String curPlcmt = (String)inEventScore[0]["PlcmtSlalom"];
                                if ( curPlcmt.Contains( "T" ) ) {
                                    curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                                }
                                outLine.Append( curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                        }
                    }
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
                        curValue = ( (Int16)inScoreSummary["Pass1Trick"] ).ToString( "###0" ).PadLeft( 4, ' ' );
                    } catch {
                        curValue = "    ";
                    }
                    outLine.Append( curValue );
                    try {
                        curValue = ( (Int16)inScoreSummary["Pass2Trick"] ).ToString( "###0" ).PadLeft( 4, ' ' );
                    } catch {
                        curValue = "    ";
                    }
                    outLine.Append( " " + curValue );
                    try {
                        curValue = ( (Int16)inScoreSummary["ScoreTrick"] ).ToString( "####0" ).PadLeft( 5, ' ' );
                    } catch {
                        curValue = "     ";
                    }
                    outLine.Append( " " + curValue );
                    try {
                        curValue = (String)inScoreSummary["EventClassTrick"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        outLine.Append( " " + curValue );
                    } catch {
                        outLine.Append( "  " );
                    }
                    outLine.Append( " " ); //Ratings no longer used

                    if ( curRound == 1 ) {
                        curValue = (String)inScoreSummary["AgeGroup"];
                        if ( curValue.Length > 0 ) {
                            if ( curValue.Length == 2 ) {
                                outLine.Append( " " + curValue + "   " );
                            } else if ( curValue.Length > 2 ) {
                                outLine.Append( " " + curValue.Substring( 0, 2 ) + "   " );
                            } else {
                                outLine.Append( "  " + curValue + "   " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        }
                    } else if (curRound == 2) {
                        if ( ( (String) inEventScore[0]["PlcmtTrick"] ).Length == 0 ) {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        } else {
                            String curPlcmt = (String)inEventScore[0]["PlcmtTrick"];
                            if ( curPlcmt.Contains( "T" ) ) {
                                curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                            } else if ( curPlcmt.Trim().Equals("999") ) {
                                curPlcmt = "**";
                            }
                            outLine.Append( " " + curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                        }
                    } else {
                        outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                    }
                } else {
                    if ( inScoreSummary == null || inEventScore.Length == 0 ) {
                        outLine.Append( "---- ---- -----         " );
                    } else {
                        outLine.Append( "---- ---- -----    " );
                        if ( curRound == 1 ) {
                            curValue = (String)inScoreSummary["AgeGroup"];
                            if ( curValue.Length > 0 ) {
                                if ( curValue.Length == 2 ) {
                                    outLine.Append( curValue + "   " );
                                } else if ( curValue.Length > 2 ) {
                                    outLine.Append( curValue.Substring( 0, 2 ) + "   " );
                                } else {
                                    outLine.Append( " " + curValue + "   " );
                                }
                            } else {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            }
                        } else if ( curRound == 2 ) {
                            if ( inEventScore.Length == 0 ) {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            } else {
                                String curPlcmt = (String)inEventScore[0]["PlcmtTrick"];
                                if ( curPlcmt.Contains( "T" ) ) {
                                    curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                                }
                                outLine.Append( curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing trick scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Trick Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierJumpScore( DataRow inScoreSummary, DataRow[] inEventScore ) {
            StringBuilder outLine = new StringBuilder( "" );
            Decimal curRampHeight;
            String curEmptyValue = "", curValue = "";
            Int16 curRound = 0;

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
                        curRampHeight = (Decimal)inScoreSummary["RampHeight"];
                        if ( curRampHeight == 5 ) {
                            curValue = "5";
                        } else if ( curRampHeight == 5.5M ) {
                            curValue = "H";
                        } else if ( curRampHeight == 6 ) {
                            curValue = "6";
                        } else if ( curRampHeight < 5 ) {
                            curValue = "4";
                        } else {
                            curValue = "";
                        }
                    } catch {
                        curValue = "";
                    }
                    outLine.Append( curValue );
                    try {
                        curValue = ( (Byte)inScoreSummary["SpeedKphJump"] ).ToString( "00" );
                    } catch {
                        curValue = "00";
                    }
                    outLine.Append( " " + curValue );

                    String curScoreBackup = "";
                    try {
                        curScoreBackup = getSkierBackupJump( (String)inScoreSummary["SanctionId"], (String)inScoreSummary["MemberId"], curRound, (String)inScoreSummary["AgeGroup"] );
                        if ( curScoreBackup.Length > 0 ) {
                            outLine.Append( " " + curScoreBackup.PadLeft( 4, ' ' ) );
                        } else {
                            outLine.Append( " ----" );
                        }
                    } catch {
                        outLine.Append( " ----" );
                    }

                    try {
                        curValue = ( (Decimal)inScoreSummary["ScoreMeters"] ).ToString( "##.0" ).PadLeft( 4, ' ' );
                    } catch {
                        curValue = "    ";
                    }
                    outLine.Append( " " + curValue );
                    try {
                        curValue = ( (Decimal)inScoreSummary["ScoreFeet"] ).ToString( "##0" ).PadLeft( 3, ' ' );
                    } catch {
                        curValue = "   ";
                    }
                    outLine.Append( " " + curValue );
                    try {
                        curValue = (String)inScoreSummary["EventClassJump"];
                        if ( curValue.Length > 1 ) curValue = curValue.Substring( 0, 1 );
                        outLine.Append( " " + curValue );
                    } catch {
                        outLine.Append( "  " );
                    }
                    outLine.Append( " " ); //Ratings no longer used

                    if ( curRound == 1 ) {
                        curValue = (String)inScoreSummary["AgeGroup"];
                        if ( curValue.Length > 0 ) {
                            if ( curValue.Length == 2 ) {
                                outLine.Append( " " + curValue + "   " );
                            } else if ( curValue.Length > 2 ) {
                                outLine.Append( "  " + curValue.Substring( 0, 2 ) + "   " );
                            } else {
                                outLine.Append( "  " + curValue + "   " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        }
                    } else if ( curRound == 2 ) {
                        if ( ( (String) inEventScore[0]["PlcmtJump"] ).Length == 0 ) {
                            outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                        } else {
                            String curPlcmt = (String)inEventScore[0]["PlcmtJump"];
                            if ( curPlcmt.Contains( "T" ) ) {
                                curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                            } else if ( curPlcmt.Trim().Equals("999") ) {
                                curPlcmt = "**";
                            }
                            outLine.Append( " " + curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                        }
                    } else {
                        outLine.Append( curEmptyValue.PadRight( 6, ' ' ) );
                    }
                } else {
                    if ( inScoreSummary == null || inEventScore.Length == 0 ) {
                        outLine.Append( "- -- ---- ---- ---         " );
                    } else {
                        outLine.Append( "- -- ---- ---- ---    " );
                        if ( curRound == 1 ) {
                            curValue = (String)inEventScore[0]["AgeGroup"];
                            if ( curValue.Length > 0 ) {
                                if ( curValue.Length == 2 ) {
                                    outLine.Append( curValue + "   " );
                                } else if ( curValue.Length > 2 ) {
                                    outLine.Append( curValue.Substring( 0, 2 ) + "   " );
                                } else {
                                    outLine.Append( " " + curValue + "   " );
                                }
                            } else {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            }
                        } else if ( curRound == 2 ) {
                            if ( inEventScore.Length == 0 ) {
                                outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                            } else {
                                String curPlcmt = (String)inEventScore[0]["PlcmtJump"];
                                if ( curPlcmt.Contains( "T" ) ) {
                                    curPlcmt = curPlcmt.Substring( 0, curPlcmt.Length - 1 ).Trim() + "T";
                                }
                                outLine.Append( curPlcmt.Trim().PadLeft( 4, ' ' ) + " " );
                            }
                        } else {
                            outLine.Append( curEmptyValue.PadRight( 5, ' ' ) );
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing jump scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Jump Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private String writeSkierOverallScore( DataRow inSummaryRow ) {
            StringBuilder outLine = new StringBuilder( "" );
            Decimal curNopsSlalom = 0, curNopsTrick = 0, curNopsJump = 0, curNopsOverall = 0;
            String curAgeGroup = "", curEventClass = "";

            try {
                if ( inSummaryRow == null ) {
                    outLine.Append( "----" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassSlalom"];
                        if ( curEventClass.Length > 1 ) curEventClass = curEventClass.Substring( 0, 1 );
                        if ( curEventClass.Length == 0 ) {
                            outLine.Append( "----" );
                        } else {
                            curNopsSlalom = (Decimal)inSummaryRow["PointsSlalom"];
                            outLine.Append( curNopsSlalom.ToString( "###0" ).PadLeft( 4, ' ' ) );
                            curNopsOverall = curNopsSlalom;
                        }
                    } catch {
                        outLine.Append( "----" );
                    }
                }
                if ( inSummaryRow == null ) {
                    outLine.Append( " ----" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassTrick"];
                        if ( curEventClass.Length > 1 ) curEventClass = curEventClass.Substring( 0, 1 );
                        if ( curEventClass.Length == 0 ) {
                            outLine.Append( " ----" );
                        } else {
                            curNopsTrick = (Decimal)inSummaryRow["PointsTrick"];
                            outLine.Append( " " + curNopsTrick.ToString( "###0" ).PadLeft( 4, ' ' ) );
                            curNopsOverall += curNopsTrick;
                        }
                    } catch {
                        outLine.Append( " ----" );
                    }
                }

                curAgeGroup = (String)inSummaryRow["AgeGroup"];
                if ( inSummaryRow == null ) {
                    outLine.Append( " ----" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassJump"];
                        if ( curEventClass.Length > 1 ) curEventClass = curEventClass.Substring( 0, 1 );
                        if ( curEventClass.Length == 0 ) {
                            outLine.Append( " ----" );
                        } else {
                            curNopsJump = (Decimal)inSummaryRow["PointsJump"];
                            outLine.Append( " " + curNopsJump.ToString( "###0" ).PadLeft( 4, ' ' ) );
                            if ( curAgeGroup.ToUpper().Equals( "B1" ) || curAgeGroup.ToUpper().Equals( "G1" ) ) {
                            } else {
                                curNopsOverall += curNopsJump;
                            }
                        }
                    } catch {
                        outLine.Append( " ----" );
                    }
                }
                if ( ( (String)inSummaryRow["QualifyOverall"] ).ToLower().Equals( "yes" ) ) {
                    try {
                        outLine.Append( " " + curNopsOverall.ToString( "###0.0" ).PadLeft( 6, ' ' ) );
                    } catch {
                        outLine.Append( "       " );
                    }
                } else {
                    outLine.Append( "            " );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error writing overall scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "Overall Error: " + ex.Message );
            }

            return outLine.ToString();
        }

        private StreamWriter getExportFile( String inFileFilter, String inFileName ) {
            String curMethodName = "getExportFile";
            String curFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            if ( inFileFilter == null ) {
                curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else {
                curFileFilter = inFileFilter;
            }

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.Filter = curFileFilter;
            curFileDialog.FilterIndex = 1;
            if ( inFileName == null ) {
                curFileDialog.FileName = "";
            } else {
                curFileDialog.FileName = inFileName;
            }

            try {
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        if ( Path.GetExtension( curFileName ) == null ) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split( '|' );
                            if ( curList.Length > 0 ) {
                                int curDelim = curList[1].IndexOf( "." );
                                if ( curDelim > 0 ) {
                                    curDefaultExt = curList[1].Substring( curDelim - 1 );
                                }
                            }
                            curFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return outBuffer;
        }

        private String getSkierBackupJump( String inSanctionId, String inMemberId, int inRound, String inDiv ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, MemberId, Round, PassNum, Reride, BoatSpeed, RampHeight, ScoreFeet, ScoreMeters, " );
            curSqlStmt.Append( " BoatSplitTime, BoatSplitTime2, BoatEndTime, TimeInTol, ScoreProt, ReturnToBase, Results " );
            curSqlStmt.Append( " FROM JumpRecap" );
            curSqlStmt.Append( " WHERE SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( " AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( " AND AgeGroup = '" + inDiv + "' " );
            curSqlStmt.Append( " AND Round = " + inRound + " " );
            curSqlStmt.Append( " AND ( (Results = 'Jump' AND Reride = 'N') " );
            curSqlStmt.Append( "        OR ( Results = 'Jump' AND Reride = 'Y' AND ScoreProt = 'Y') ) " );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, ScoreMeters DESC" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 1 ) {
                try {
                    return ( (Decimal)curDataTable.Rows[1]["ScoreMeters"] ).ToString( "##.0" );
                } catch {
                    return "";
                }
            } else {
                return "";
            }
        }

        private String getIwwfSlalomMin(String inDiv) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'IwwfSlalomMin' " );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                return ( (Decimal)curDataTable.Rows[0]["MaxValue"] ).ToString( "00" );
            } else {
                return "25";
            }
        }

        private DataTable getTourData(String inSanctionId) {
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
    }
}
