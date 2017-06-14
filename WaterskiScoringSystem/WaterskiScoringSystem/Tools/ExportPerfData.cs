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
    class ExportPerfData {
        private DataRow myTourRow;
        private int myTourRounds = 0;
        private int mySlalomRounds = 0;
        private int myTrickRounds = 0;
        private int myJumpRounds = 0;
        private int myRoundsMax = 6;
        private String myTourFed = "";
        private TourProperties myTourProperties;
        private ListSkierClass mySkierClassList;
        private CalcEventPlcmt myCalcEventPlcmt;
        private ProgressWindow myProgressInfo;

        public ExportPerfData() {
            myCalcEventPlcmt = new CalcEventPlcmt();
        }

        public Boolean exportTourPerfData( String inSanctionId ) {
            Boolean returnStatus = false;
            String curMemberId = "", curReadyToSki = "", curAgeGroup = "";
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;
            String curFilename = "", curRules = "";
            myTourProperties = TourProperties.Instance;

            DataTable curTourDataTable = getTourData( inSanctionId );
            if ( curTourDataTable != null ) {
                if ( curTourDataTable.Rows.Count > 0 ) {
                    myTourRow = curTourDataTable.Rows[0];
                    curRules = (String)myTourRow["Rules"];

                    curFilename = inSanctionId.Trim() + myTourRow["Class"].ToString().Trim() + ".wsp";
                    outBuffer = getExportFile( curFilename );
                    if ( outBuffer != null ) {
                        Log.WriteFile( "Export performance data file begin: " + curFilename );

                        //Build file header line and write to file
                        outLine.Append( writeHeader( inSanctionId ) );
                        outBuffer.WriteLine( outLine.ToString() );

                        //Initialize output buffer
                        outLine = new StringBuilder( "" );
                    }
                }
            }
            if ( outBuffer != null ) {
                myProgressInfo = new ProgressWindow();
                myProgressInfo.setProgessMsg( "Processing Skier Performance Data File" );
                myProgressInfo.Show();
                myProgressInfo.Refresh();
                myProgressInfo.setProgressMax( 10 );
                
                DataTable curMemberDataTable = getMemberData( inSanctionId );
                if ( curMemberDataTable != null ) {
                    DataRow[] curScoreRows, curScoreSlalomRows, curScoreTrickRows, curScoreJumpRows;
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
                    if ( curPlcmtOrg.ToLower().Equals("div") ) {
                        curPlcmtOverallOrg = "agegroup";
                    } else if ( curPlcmtOrg.ToLower().Equals( "divgr" ) ) {
                        curPlcmtOverallOrg = "agegroupgroup";
                    } else {
                        curPlcmtOverallOrg = "agegroup";
                        curPlcmtOrg = "div";
                    }

                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();
                    DataTable mySummaryDataTable = null;

                    DataTable myMemberData = curCalcSummary.getMemberData( inSanctionId );
                    myProgressInfo.setProgressValue( 1 );
                    myProgressInfo.Refresh();

                    if (curRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" )) {
                        mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, inSanctionId, "Scorebook", curRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null );
                        myProgressInfo.setProgressValue( 5 );
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
                        DataTable newSummaryDataTable = curCalcSummary.buildTourScorebook( inSanctionId, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                        mySummaryDataTable = newSummaryDataTable;
                        myProgressInfo.setProgressValue( 9 );
                        myProgressInfo.Refresh();

                    } else {
                        DataTable mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 2 );
                        myProgressInfo.Refresh();
                        DataTable myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 3 );
                        myProgressInfo.Refresh();
                        DataTable myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                        myProgressInfo.setProgressValue( 4 );
                        myProgressInfo.Refresh();
                        mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOverallOrg );
                        myProgressInfo.setProgressValue( 5 );
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
                        DataTable newSummaryDataTable = curCalcSummary.buildTourScorebook( inSanctionId, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                        mySummaryDataTable = newSummaryDataTable;
                        myProgressInfo.setProgressValue( 9 );
                        myProgressInfo.Refresh();

                    }
                    mySkierClassList = new ListSkierClass();
                    mySkierClassList.ListSkierClassLoad();

                    int curRowCount = 0;
                    myProgressInfo.setProgressMax( curMemberDataTable.Rows.Count );
                    foreach ( DataRow curMemberRow in curMemberDataTable.Rows ) {
                        curRowCount++;
                        myProgressInfo.setProgressValue( curRowCount );
                        myProgressInfo.Refresh();

                        curMemberId = curMemberRow["MemberId"].ToString();
                        curAgeGroup = curMemberRow["AgeGroup"].ToString();
                        curReadyToSki = curMemberRow["ReadyToSki"].ToString();
                        curScoreRows = mySummaryDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        //curScoreSlalomRows = mySlalomDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        //curScoreTrickRows = myTrickDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );
                        //curScoreJumpRows = myJumpDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "'" );

                        if ( curReadyToSki.Equals( "Y" ) ) {
                            if ( curScoreRows.Length > 0 ) {
                                //Write skier identification information
                                outLine.Append( writeSkierInfo( curMemberRow, inSanctionId, curRules, curAgeGroup ) );

                                //Write skier performance summary information
                                //outLine.Append( writeSkierPerfSmry( curMemberId, curAgeGroup, curDataType, curMemberRow, curScoreRows, curScoreSlalomRows, curScoreTrickRows, curScoreJumpRows ) );
                                outLine.Append( writeSkierPerfSmry( curMemberId, curAgeGroup, curDataType, curMemberRow, curScoreRows, curScoreRows, curScoreRows, curScoreRows ) );

                                //Write skier performance summary information
                                outLine.Append( writeSkierPerfData( curMemberId, curAgeGroup, curRules, curMemberRow, curScoreRows ) );

                                //Write output line to file
                                outBuffer.WriteLine( outLine.ToString() );
                            }
                        }

                        //Initialize output buffer
                        outLine = new StringBuilder( "" );
                    }
                    returnStatus = true;
                    outBuffer.Close();

                    myProgressInfo.Close();
                    if ( curMemberDataTable.Rows.Count > 0 ) {
                        MessageBox.Show( curMemberDataTable.Rows.Count + " rows found and written" );
                    } else {
                        MessageBox.Show( "No rows found" );
                    }
                    Log.WriteFile( "Export performance data file complete: " + curFilename );
                }
            }

            return returnStatus;
        }

        //Write skier identification information
        private String writeHeader( String inSanctionId ) {
            myTourFed = myTourRow["Federation"].ToString().ToUpper();
            try {
                mySlalomRounds = Convert.ToInt16( myTourRow["SlalomRounds"].ToString() );
                if (mySlalomRounds > myRoundsMax) {
                    mySlalomRounds = myRoundsMax;
                }
            } catch {
                mySlalomRounds = 0;
            }
            try {
                myTrickRounds = Convert.ToInt16( myTourRow["TrickRounds"].ToString() );
                if (myTrickRounds > myRoundsMax) {
                    myTrickRounds = myRoundsMax;
                }
            } catch {
                myTrickRounds = 0;
            }
            try {
                myJumpRounds = Convert.ToInt16( myTourRow["JumpRounds"].ToString() );
                if (myJumpRounds > myRoundsMax) {
                    myJumpRounds = myRoundsMax;
                }
            } catch {
                myJumpRounds = 0;
            }
            if ( mySlalomRounds > myTourRounds ) { myTourRounds = mySlalomRounds; }
            if ( myTrickRounds > myTourRounds ) { myTourRounds = myTrickRounds; }
            if ( myJumpRounds > myTourRounds ) { myTourRounds = myJumpRounds; }

            String curMixedClass = "Y";
            String curRoundFmt = "M";  //S=single, M=multi
            String curPlcmtBasis = "B"; //T=Total B=Best 2=xxxxx L=xxxxx
            String curOverallBasis = "N"; //N=NOPS 1=xxxxx 2=xxxxx C=xxxxx P=xxxxx
            String curDateString = DateTime.Now.ToString( "yyyyMMdd HH:MM" );
            String curDateOut = curDateString.Substring( 0, 8 );
            String curTimeOut = curDateString.Substring( 9, 5 );
            String curEventDateOut = myTourRow["EventDates"].ToString();
            try {
                DateTime curEventDate = Convert.ToDateTime( curEventDateOut );
                curEventDateOut = curEventDate.ToString( "yyyyMMdd" );
            } catch {
                MessageBox.Show( "The event date of " + curEventDateOut + " is not a valid date and must corrected" );
                curEventDateOut = myTourRow["EventDates"].ToString();
            }
            String curContactName = "", curContactEmail = "", curContactPhone = "";
            try {
                curContactName = myTourRow["ContactName"].ToString();
                curContactEmail = myTourRow["ContactEmail"].ToString();
                curContactPhone = myTourRow["ContactPhone"].ToString();
            } catch {
                curContactName = "";
                curContactEmail = "";
                curContactPhone = "";
            }

            //Write performance header data
            String curHeaderLine = "\"" + myTourFed
                + "\",\"" + inSanctionId.ToUpper().Trim() + myTourRow["Class"].ToString().ToUpper().Trim()
                + "\",\"" + myTourFed + "-----"
                + "\",\"" + myTourRow["Name"].ToString()
                + "\",\"" + myTourRow["Class"].ToString().ToUpper()
                + "\",\"" + curMixedClass
                + "\",\"" + curRoundFmt
                + "\",\"" + curPlcmtBasis
                + "\",\"" + curOverallBasis
                + "\",\"" + curEventDateOut
                + "\",\"" + curContactName
                + "\",\"" + curContactEmail
                + "\",\"" + curContactPhone
                + "\",\"" + curDateOut
                + "\",\"" + curTimeOut
                + "\",\"" + Properties.Settings.Default.AppTitle + " Version "
                + "\",\"" + Properties.Settings.Default.BuildVersion
                + "\"";
            return curHeaderLine;
        }

        //Write skier identification information
        private String writeSkierInfo( DataRow curRow, String inSanctionId, String inRules, String inAgeGroup ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curValue;
            int curTourYear = 0;

            curValue = curRow["Federation"].ToString(); //1. Member Federation
            if ( curValue.Length > 1 ) {
                outLine.Append( "\"" + curValue.ToUpper() + "\"" );
            } else {
                outLine.Append( "\"" + myTourFed + "\"" );
            }

            curValue = curRow["SkierName"].ToString();
            String[] curValueList = curValue.Split( ',' );
            outLine.Append( ",\"" + curRow["MemberId"].ToString() + "\"" ); //2. Member Identifier
            outLine.Append( ",\"" + curValueList[0].Trim() + "\"" ); //3. Last Name
            outLine.Append( ",\"" + curValueList[1].Trim() + "\"" ); //4. First Name
            outLine.Append( ",\"" + curRow["Gender"].ToString() + "\"" ); //5. Gender
            curTourYear = Convert.ToInt16( inSanctionId.Substring( 0, 2 ) );
            curTourYear += 100;
            try {
                Int16 curSkiYearAge = Convert.ToInt16( curRow["SkiYearAge"].ToString() );
                if ( curSkiYearAge > 0 ) {
                    curValue = Convert.ToString( curTourYear - Convert.ToInt16( curRow["SkiYearAge"].ToString() ) - 1 ); //6. Birth year
                    if ( curValue.Length > 2 ) {
                        curValue = curValue.Substring( 1, 2 );
                    }
                } else {
                    curValue = "00";
                }
            } catch {
                curValue = "00";
            }
            outLine.Append( "," + curValue );
            outLine.Append( ",\"" + curRow["State"].ToString() + "\"" ); //7. Skier home state
            curValue = curRow["Region"].ToString(); //8. Skier home region
            if ( curValue.Length > 0 ) {
                outLine.Append( ",\"" + curValue.Substring( 0, 1 ) + "\"" );
            } else {
                outLine.Append( ",\"\"" );
            }

            if ( curRow["SlalomTeam"].ToString().Trim().Length > 0 ) {
                curValue = curRow["SlalomTeam"].ToString().Trim();
            } else if ( curRow["TrickTeam"].ToString().Trim().Length > 0 ) {
                curValue = curRow["TrickTeam"].ToString().Trim();
            } else if ( curRow["JumpTeam"].ToString().Trim().Length > 0 ) {
                curValue = curRow["JumpTeam"].ToString().Trim();
            } else {
                curValue = "   ";
            }
            if ( curValue.Length > 3 ) {
                curValue = curValue.Substring( 1, 3 );
            }
            if ( inRules.ToUpper().Equals( "NCWSA" ) ) {
                if ( inAgeGroup.ToUpper().Equals( "CM" ) || inAgeGroup.ToUpper().Equals( "CW" ) ) {
                    curValue += "/A";
                } else if ( inAgeGroup.ToUpper().Equals( "BM" ) || inAgeGroup.ToUpper().Equals( "BW" ) ) {
                    curValue += "/B";
                } else {
                    curValue = "   ";
                }
            }
            
            outLine.Append( ",\"" + curValue.PadRight( 3, ' ' ) + "\"" ); //9. Skier team code

            return outLine.ToString();
        }

        //Write skier performance summary information
        private String writeSkierPerfSmry( String curMemberId, String curAgeGroup, String curDataType, DataRow curMemberRow, DataRow[] curScoreRows, DataRow[] curScoreSlalomRows, DataRow[] curScoreTrickRows, DataRow[] curScoreJumpRows ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curValue = "", curEventClassSlalom = "", curEventClassTrick = "", curEventClassJump = "";
            String tmpEventGroup = "";
            Decimal curScore = 0;

            outLine.Append( "," + myTourRounds.ToString() ); //10. Number rounds in tournament

            DataRow curScoreRow = null;
            if ( curScoreRows.Length == 0 ) {
                curEventClassSlalom = "";
                curEventClassTrick = "";
                curEventClassJump = "";
            } else {
                for ( int curIdx = 0; curIdx < curScoreRows.Length; curIdx++ ) {
                    if ( (Int16)curScoreRows[curIdx]["RoundOverall"] == 1 ) {
                        curScoreRow = curScoreRows[curIdx];
                    }
                }
                curEventClassSlalom = ( (String)curScoreRow["EventClassSlalom"] ).Trim();
                curEventClassTrick = ( (String)curScoreRow["EventClassTrick"] ).Trim();
                curEventClassJump = ( (String)curScoreRow["EventClassJump"] ).Trim();
            }

            //11. Slalom Event Placement      Char       4       "999T"
            //12. Slalom Placement Points     Num        4       9999
            //13. Rd # of best Slalom Score   Num        1       9
            if ( curScoreRow == null ) {
                outLine.Append( ",\"    \",," );
            } else {
                if (curEventClassSlalom.Length == 0 || curScoreSlalomRows.Length == 0 || curEventClassSlalom.ToUpper().Equals("X") ) {
                    outLine.Append( ",\"    \",," );
                } else {
                    //curScoreSlalomRows
                    String tmpPlcmt = "";
                    String curPlcmt = (String)curScoreRow["PlcmtSlalom"];
                    try {
                        if ( ( (String) curScoreRow["ReadyForPlcmtSlalom"] ).ToLower().Equals("y") ) {
                            if ( curPlcmt.Length > 0 ) {
                                if ( curPlcmt.Substring(curPlcmt.Length - 1, 1).Equals("T") ) {
                                    tmpPlcmt = curPlcmt.PadLeft(4, ' ');
                                } else {
                                    tmpPlcmt = curPlcmt.PadLeft(3, ' ') + " ";
                                }
                            } else {
                                tmpPlcmt = "    ";
                            }
                        } else {
                            tmpPlcmt = "    ";
                        }
                    } catch {
                        tmpPlcmt = "    ";
                    }
                    curPlcmt = tmpPlcmt;
                    outLine.Append( ",\"" + curPlcmt + "\"" );
                    try {
                        curScore = (Decimal)curScoreSlalomRows[0]["Score"];
                    } catch {
                        curScore = 0;
                    }
                    outLine.Append( "," + curScore.ToString("##0.00") );

                    try {
                        if ( curScoreSlalomRows[0]["Round"].GetType() == System.Type.GetType( "System.Byte" ) ) {
                            outLine.Append( "," + ( (Byte)curScoreSlalomRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreSlalomRows[0]["Round"].GetType() == System.Type.GetType( "System.Int16" ) ) {
                            outLine.Append( "," + ( (Int16)curScoreSlalomRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreSlalomRows[0]["Round"].GetType() == System.Type.GetType( "System.Int32" ) ) {
                            outLine.Append( "," + ( (int)curScoreSlalomRows[0]["Round"] ).ToString( "0" ) );
                        } else {
                            outLine.Append( ",0" );
                        }
                    } catch {
                        outLine.Append( ",0" );
                    }
                }
            }

            //14. Trick Event Placement       Char       4       "999T"
            //15. Trick Placement Points      Num        4       9999
            //16. Rd # of best Trick Score    Num        1       9
            if ( curScoreRow == null ) {
                outLine.Append( ",\"    \",," );
            } else {
                if (curEventClassTrick.Length == 0 || curScoreTrickRows.Length == 0 || curEventClassTrick.ToUpper().Equals( "X" )) {
                    outLine.Append( ",\"    \",," );
                } else {
                    //curScoreTrickRows
                    String tmpPlcmt = "";
                    String curPlcmt = (String)curScoreRow["PlcmtTrick"];
                    try {
                        if ( ( (String) curScoreRow["ReadyForPlcmtTrick"] ).ToLower().Equals("y") ) {
                            if ( curPlcmt.Length > 0 ) {
                                if ( curPlcmt.Substring(curPlcmt.Length - 1, 1).Equals("T") ) {
                                    tmpPlcmt = curPlcmt.PadLeft(4, ' ');
                                } else {
                                    tmpPlcmt = curPlcmt.PadLeft(3, ' ') + " ";
                                }
                            } else {
                                tmpPlcmt = "    ";
                            }
                        } else {
                            tmpPlcmt = "    ";
                        }
                    } catch {
                        tmpPlcmt = "    ";
                    }
                    curPlcmt = tmpPlcmt;
                    outLine.Append( ",\"" + curPlcmt + "\"" );
                    try {
                        curScore = Convert.ToDecimal( (Int16)curScoreTrickRows[0]["Score"] );
                    } catch {
                        curScore = 0;
                    }
                    outLine.Append( "," + curScore.ToString( "####0" ) );

                    try {
                        if ( curScoreTrickRows[0]["Round"].GetType() == System.Type.GetType( "System.Byte" ) ) {
                            outLine.Append( "," + ( (Byte)curScoreTrickRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreTrickRows[0]["Round"].GetType() == System.Type.GetType( "System.Int16" ) ) {
                            outLine.Append( "," + ( (Int16)curScoreTrickRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreTrickRows[0]["Round"].GetType() == System.Type.GetType( "System.Int32" ) ) {
                            outLine.Append( "," + ( (int)curScoreTrickRows[0]["Round"] ).ToString( "0" ) );
                        } else {
                            outLine.Append( ",0" );
                        }
                    } catch {
                        outLine.Append( ",0" );
                    }
                }
            }

            //17. Jumping Event Placement     Char       4       "999T"
            //18. Jumping Placement Points    Num        4       9999
            //19. Rd # of best Jump Score     Num        1       9
            if ( curScoreRow == null ) {
                outLine.Append( ",\"    \",," );
            } else {
                if (curEventClassJump.Length == 0 || curScoreJumpRows.Length == 0 || curEventClassJump.ToUpper().Equals( "X" )) {
                    outLine.Append( ",\"    \",," );
                } else {
                    //curScoreJumpRows
                    String tmpPlcmt = "";
                    String curPlcmt = (String)curScoreRow["PlcmtJump"];
                    try {
                        if ( ( (String) curScoreRow["ReadyForPlcmtJump"] ).ToLower().Equals("y") ) {
                            if ( curPlcmt.Length > 0 ) {
                                if ( curPlcmt.Substring(curPlcmt.Length - 1, 1).Equals("T") ) {
                                    tmpPlcmt = curPlcmt.PadLeft(4, ' ');
                                } else {
                                    tmpPlcmt = curPlcmt.PadLeft(3, ' ') + " ";
                                }
                            } else {
                                tmpPlcmt = "    ";
                            }
                        } else {
                            tmpPlcmt = "    ";
                        }
                    } catch {
                        tmpPlcmt = "    ";
                    }
                    curPlcmt = tmpPlcmt;
                    outLine.Append( ",\"" + curPlcmt + "\"" );
                    try {
                        curScore = (Decimal)curScoreJumpRows[0]["ScoreMeters"];
                    } catch {
                        curScore = 0;
                    }
                    outLine.Append( "," + curScore.ToString( "#0.0" ) );

                    try {
                        if ( curScoreJumpRows[0]["Round"].GetType() == System.Type.GetType( "System.Byte" ) ) {
                            outLine.Append( "," + ( (Byte)curScoreJumpRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreJumpRows[0]["Round"].GetType() == System.Type.GetType( "System.Int16" ) ) {
                            outLine.Append( "," + ( (Int16)curScoreJumpRows[0]["Round"] ).ToString( "0" ) );
                        } else if ( curScoreJumpRows[0]["Round"].GetType() == System.Type.GetType( "System.Int32" ) ) {
                            outLine.Append( "," + ( (int)curScoreJumpRows[0]["Round"] ).ToString( "0" ) );
                        } else {
                            outLine.Append( ",0" );
                        }
                    } catch {
                        outLine.Append( ",0" );
                    }
                }
            }

            //20. Overall Placement           Char       4       "999T"
            //21. Overall Placement Points    Num        4       9999
            //22. Rd # of best Overall Score  Num        1       9
            if ( curScoreRow == null ) {
                outLine.Append( ",\"\",," );
            } else {
                outLine.Append( ",\"\",," );
                /*
                String tmpPlcmt = "";
                //String curPlcmt = (String)curScoreRow["PlcmtGroup"];
                String curPlcmt = "";
                if ( curPlcmt.Length > 0 ) {
                    if ( curPlcmt.Substring( curPlcmt.Length - 1, 1 ).Equals( "T" ) ) {
                        tmpPlcmt = curPlcmt.PadLeft( 4, ' ' );
                    } else {
                        tmpPlcmt = curPlcmt.PadLeft( 3, ' ' ) + " ";
                    }
                } else {
                    tmpPlcmt = "    ";
                }
                curPlcmt = tmpPlcmt;
                outLine.Append( ",\"" + curPlcmt + "\"" );
                outLine.Append( "," );
                outLine.Append( "," + ( (Int16)curScoreRow["RoundOverall"] ).ToString( "0" ) );
                 */
            }

            return outLine.ToString();
        }

        //Write skier identification information
        private String writeSkierPerfData( String curMemberId, String curAgeGroup, String inRules, DataRow curMemberRow, DataRow[] curScoreRows ) {
            StringBuilder outLine = new StringBuilder( "" );
            Decimal curValueNum = 0, curSlalomScore = 0;
            String curValue = "", curEventClassSlalom = "", curEventClassTrick = "", curEventClassJump = "";
            DataRow curScoreRow;

            for ( int curRound = 1; curRound <= myTourRounds; curRound++ ) {
                curScoreRow = null;
                for ( int curIdx = 0; curIdx < curScoreRows.Length; curIdx++ ) {
                    if ( (Int16)curScoreRows[curIdx]["RoundOverall"] == curRound ) {
                        curScoreRow = curScoreRows[curIdx];
                    }
                }

                outLine.Append( "," + ((Byte)curRound).ToString("0") ); //23 Current round number

                //24. Slalom Sanction Class (2)  Char        1       "C"
                //25. Slalom Division Code (4)   Char        2       "B2"
                //26. Slalom Boat Model Code (5) Char        2       "MC"
                //27. Slalom End Pass Score (4)  Num         4       9.99
                //28. Slalom End Pass Speed (4)  Num         2       99
                //29. Slalom End Pass Line (4)   Num         4       9999
                //30. Slalom Tot Score (Buoys)   Num         6       999.99
                if ( curScoreRow == null ) {
                    curEventClassSlalom = "";
                    curEventClassTrick = "";
                    curEventClassJump = "";
                } else {
                    curEventClassSlalom = ((String)curScoreRow["EventClassSlalom"]).Trim();
                    curEventClassTrick = ((String)curScoreRow["EventClassTrick"]).Trim();
                    curEventClassJump = ( (String)curScoreRow["EventClassJump"] ).Trim();
                }

                if (curEventClassSlalom.Length == 0 || curEventClassSlalom.ToUpper().Equals( "X" )) {
                    outLine.Append( ",\"\",\"\",\"\",,,," );
                } else {
                    outLine.Append( ",\"" + curEventClassSlalom + "\"" );
                    curValue = curAgeGroup.Trim();
                    if ( inRules.ToUpper().Equals( "NCWSA" ) ) {
                        if ( curAgeGroup.ToUpper().Equals( "BM" ) ) {
                            curValue = "CM";
                        } else if ( curValue.ToUpper().Equals( "BW" ) ) {
                            curValue = "CW";
                        }
                    }
                    outLine.Append( ",\"" + curValue + "\"" );
                    try {
                        outLine.Append( ",\"" + curScoreRow["SlalomBoatCode"].ToString().Trim().PadLeft( 2, ' ' ) + "\"" );
                    } catch {
                        outLine.Append( ",\"  \"" );
                    }

                    try {
                        curSlalomScore = (Decimal)curScoreRow["ScoreSlalom"];
                    } catch {
                        curSlalomScore = 0M;
                    }

                    try {
                        Byte curFinalSpeedKph = (Byte) curScoreRow["FinalSpeedKph"];
                        Byte curCompletedSpeedKph = (Byte) curScoreRow["CompletedSpeedKph"];
                        Decimal curFinalPassScore = (Decimal) curScoreRow["FinalPassScore"];
                        if ( curCompletedSpeedKph > 0 ) {
                            if ( curCompletedSpeedKph != curFinalSpeedKph ) {
                                curFinalPassScore = curFinalPassScore * -1;
                            }
                        }
                        outLine.Append( "," + curFinalPassScore.ToString( "0.00" ) );
                        if ( curSlalomScore < 6 ) {
                            if (inRules.ToLower().Equals( "iwwf" )) {
                                outLine.Append( "," + getIwwfSlalomMin( curAgeGroup ) );
                                outLine.Append( ",1825" );
                                outLine.Append( "," + curSlalomScore.ToString( "##0.00" ) );
                            } else if (( mySkierClassList.compareClassChange( curEventClassSlalom, "L" ) <= 0 )) {
                                outLine.Append( ",25" );
                                outLine.Append( ",1825" );
                                outLine.Append( "," + curSlalomScore.ToString( "##0.00" ) );
                            } else {
                                outLine.Append( ",25" );
                                outLine.Append( ",2300" );
                                outLine.Append( "," + curSlalomScore.ToString( "##0.00" ) );
                            }
                        } else {
                            try {
                                outLine.Append( "," + ( (Byte)curScoreRow["FinalSpeedKph"] ).ToString( "00" ) );
                            } catch {
                                outLine.Append( ",  " );
                            }
                            try {
                                outLine.Append( "," + ( Convert.ToDecimal( (String)curScoreRow["FinalLen"] ) * 100 ).ToString( "0000" ) );
                            } catch {
                                outLine.Append( ",2300" );
                            }
                            outLine.Append( "," + ( curSlalomScore.ToString( "##0.00" ) ).PadLeft( 6, ' ' ) );
                        }
                    } catch ( Exception ex ) {
                        //MessageBox.Show( "Exception encountered processing slalom score final pass data, export file will not be formatted correcting without fixing this error" + "\n\nError: " + ex.Message );
                        outLine.Append( ",  " );
                        outLine.Append( ",    " );
                        outLine.Append( "," + curSlalomScore.ToString( "##0.00" ) );
                    }
                }

                //31. Trick Sanction Class (2)   Char        1       "C"
                //32. Trick Division Code (4)    Char        2       "B2"
                //33. Trick Boat Model Code (5)  Char        2       "MC"
                //34. Trick Score Total Pts      Num         5       99999
                if (curEventClassTrick.Length == 0 || curEventClassTrick.ToUpper().Equals( "X" )) {
                    outLine.Append( ",\"\",\"\",\"\"," );
                } else {
                    try {
                        Decimal curTrickScore = 0;
                        curValue = curScoreRow["ScoreTrick"].ToString().Trim();
                        if ( curValue == null ) curValue = "";
                        curTrickScore = Convert.ToDecimal( curValue );

                        outLine.Append( ",\"" + curEventClassTrick + "\"" );
                        curValue = curAgeGroup.Trim();
                        if ( inRules.ToUpper().Equals( "NCWSA" ) ) {
                            if ( curValue.ToUpper().Equals( "BM" ) ) {
                                curValue = "CM";
                            } else if ( curValue.ToUpper().Equals( "BW" ) ) {
                                curValue = "CW";
                            }
                        }
                        outLine.Append( ",\"" + curValue + "\"" );
                        try {
                            outLine.Append( ",\"" + curScoreRow["TrickBoatCode"].ToString().Trim().PadLeft( 2, ' ' ) + "\"" );
                        } catch {
                            outLine.Append( ",\"  \"" );
                        }

                        curValue = curTrickScore.ToString( "####0" );
                        outLine.Append( "," + curValue.PadLeft( 5, ' ' ) );
                    } catch ( Exception ex ) {
                        MessageBox.Show( "Exception encountered processing trick scores, export file will not be formatted correcting without fixing this error" + "\n\nError: " + ex.Message );
                    }
                }

                //35. Jump Sanction Class (2)    Char        1       "C"
                //36. Jump Division Code (4)     Char        2       "B2"
                //37. Jump Boat Model Code (5)   Char        2       "MC"
                //38. Ramp Height Ratio (4)      Num         4       .999
                //39. Jump Boat Speed (4)        Num         2       99
                //40. Best Distance (Feet)       Num         3       999
                //41. Best Distance (Meters)     Num         4       99.9
                if (curEventClassJump.Length == 0 || curEventClassJump.ToUpper().Equals( "X" )) {
                    outLine.Append( ",\"\",\"\",\"\",,,," );
                } else {
                    try {
                        outLine.Append( ",\"" + curEventClassJump + "\"" );
                        curValue = curAgeGroup.Trim();
                        if ( inRules.ToUpper().Equals( "NCWSA" ) ) {
                            if ( curValue.ToUpper().Equals( "BM" ) ) {
                                curValue = "CM";
                            } else if ( curValue.ToUpper().Equals( "BW" ) ) {
                                curValue = "CW";
                            }
                        }
                        outLine.Append( ",\"" + curValue + "\"" );
                        try {
                            outLine.Append( ",\"" + curScoreRow["JumpBoatCode"].ToString().Trim().PadLeft( 2, ' ' ) + "\"" );
                        } catch {
                            outLine.Append( ",\"  \"" );
                        }

                        Decimal curRampHeight = Convert.ToDecimal( curScoreRow["RampHeight"].ToString().Trim() );
                        if ( curRampHeight == 5.00M ) {
                            curValue = ".235";
                        } else if ( curRampHeight == 5.50M ) {
                            curValue = ".255";
                        } else if ( curRampHeight == 6.00M ) {
                            curValue = ".271";
                        } else if ( curRampHeight == 4.00M ) {
                            curValue = ".215";
                        } else if ( curRampHeight == 4.50M ) {
                            curValue = ".215";
                        } else {
                            curValue = ".235";
                        }
                        outLine.Append( "," + curValue.PadLeft( 4, ' ' ) );
                        outLine.Append( "," + ( (Byte)curScoreRow["SpeedKphJump"] ).ToString( "00" ) );
                        outLine.Append( "," + ( (Decimal)curScoreRow["ScoreFeet"] ).ToString( "##0" ).PadLeft( 3, ' ' ) );
                        outLine.Append( "," + ( (Decimal)curScoreRow["ScoreMeters"] ).ToString( "##.0" ).PadLeft( 4, ' ' ) );
                    } catch ( Exception ex ) {
                        MessageBox.Show( "Exception encountered processing jump scores, export file will not be formatted correcting without fixing this error" + "\n\nError: " + ex.Message );
                    }
                }

                //42. Overall Class (2)          Char        1       "C"
                //43. Overall Division Code (4)  Char        2       "B2"
                //44. Overall Score              Num         6       9999.9
                if ( curScoreRow == null ||
                    ( curEventClassSlalom.Length == 0 && curEventClassTrick.Length == 0 && curEventClassJump.Length == 0 

                    )) {
                    outLine.Append( ",\"\",\"\"," );
                } else {
                    Decimal curOverallScore = 0;
                    String curOverallAgeGroup = curAgeGroup.Trim();

                    if (( (String)curScoreRow["EligOverall"] ).ToLower().Equals( "yes" )) {
                        curValue = curScoreRow["ScoreOverall"].ToString().Trim();
                        if (curValue == null) curValue = "";
                        if (curValue.Length > 0) {
                            curOverallScore = Convert.ToDecimal( curValue );

                            if (curEventClassSlalom.Length == 0) {
                                outLine.Append( ",\"" + curEventClassSlalom + "\"" );
                            } else if (curEventClassTrick.Length == 0) {
                                outLine.Append( ",\"" + curEventClassTrick + "\"" );
                            } else if (curEventClassJump.Length == 0) {
                                outLine.Append( ",\"" + curEventClassJump + "\"" );
                            } else {
                                outLine.Append( ",\" \"" );
                            }
                            outLine.Append( ",\"" + curOverallAgeGroup + "\"" );

                            curValue = curOverallScore.ToString( "####0.0" );
                            outLine.Append( "," + curValue.PadLeft( 6, ' ' ) );
                        } else {
                            outLine.Append( ",\"\",\"\"," );
                        }
                    } else {
                        outLine.Append( ",\"\",\"\"," );
                    }

                }
            }

            return outLine.ToString();
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

        private String getIwwfSlalomMin(String inDiv) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'IWWFSlalomMin' " );
            curSqlStmt.Append( "  AND ListCode = '" + inDiv + "' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                return ( (Decimal)curDataTable.Rows[0]["MaxValue"] ).ToString( "00" );
            } else {
                return "25";
            }
        }

        private StreamWriter getExportFile(String inFileName) {
            StreamWriter outBuffer = null;

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.FileName = inFileName;

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    String myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if ( curFileName.IndexOf( '.' ) < 0 ) {
                            myFileName += ".wsp";
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        public DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
