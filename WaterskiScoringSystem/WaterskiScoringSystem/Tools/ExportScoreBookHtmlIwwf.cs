﻿using System;
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
    class ExportScoreBookHtmlIwwf {
        private int myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private String myTourClass;
        private DataRow myTourRow;
        private ProgressWindow myProgressInfo;
        private TourProperties myTourProperties;

        public ExportScoreBookHtmlIwwf() {
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
                        if ( myTourRounds == 1 ) { myTourRounds = 2; }
                    }
                }
            }
        }

        public Boolean exportScoreBookData() {
            String curMethodName = "ExportScoreBookHtmlIwwf";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curMsg = "", curMemberId = "", curAgeGroup = "", prevMemberId = "", prevAgeGroup = "", curReadyToSki = "";
            String curFileFilter = "HTML files (*.htm)|*.htm|All files (*.*)|*.*";
            Int16 curRound = 0;
            DataRow prevRow = null;
            myTourProperties = TourProperties.Instance;

            ExportScorebookTypeDialogForm curDialogForm = new ExportScorebookTypeDialogForm();
            curDialogForm.ShowDialog();
            if (curDialogForm.DialogResult == DialogResult.OK) {
                String curReportFormat = curDialogForm.ReportFormat;
                if (curReportFormat.Equals( "Magazine" )) {
                    MessageBox.Show( "This feature is not currently supported for IWWF tournaments" );
                } else {
                    try {
                        curMsg = "Exporting Scorebook Html";
                        Log.WriteFile( curMethodName + ":begin: " + curMsg );

                        String curFilename = mySanctionNum.Trim() + "CS.HTM";
                        StreamWriter outBuffer = getExportFile( curFileFilter, curFilename );

                        if (outBuffer == null) {
                            curMsg = "Output file not available";
                        } else {
                            String curPlcmtMethod = "score", curPlcmtOrg = "div", curPointsMethod = "kbase";
                            Int16 curSummaryNumPrelim = 0;
                            Int16.TryParse( myTourProperties.SlalomSummaryNumPrelim, out curSummaryNumPrelim );

                            String curDataType = myTourProperties.MasterSummaryDataType;
                            if (curDataType.ToLower().Equals( "total" )
                                || curDataType.ToLower().Equals( "best" )
                                || curDataType.ToLower().Equals( "final" )
                                || curDataType.ToLower().Equals( "first" )) {
                            } else {
                                curDataType = "best";
                            }

                            myProgressInfo = new ProgressWindow();
                            myProgressInfo.setProgessMsg( "Processing Scorebook HTML" );
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();
                            myProgressInfo.setProgressMax( 10 );
                            CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                            DataTable myMemberData = curCalcSummary.getMemberData( mySanctionNum );
                            myProgressInfo.setProgressValue( 4 );
                            myProgressInfo.Refresh();

                            DataTable mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Scorebook", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curSummaryNumPrelim );
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
                            DataTable newSummaryDataTable = curCalcSummary.buildTourScorebook( mySanctionNum, myTourRow, myMemberData, mySummaryDataTable, mySlalomDetail, myTrickDetail, myJumpDetail );
                            mySummaryDataTable = newSummaryDataTable;
                            myProgressInfo.setProgressValue( 9 );
                            myProgressInfo.Refresh();

                            myProgressInfo.setProgressMax( mySummaryDataTable.Rows.Count );
                            myProgressInfo.Refresh();

                            //Build file header line and write to file
                            writeHeader( outBuffer, curPlcmtOrg );

                            if (curReportFormat.Equals( "Index" )) {
                                //Build index header
                                writeIndexHeader( outBuffer, curPlcmtOrg, mySummaryDataTable );
                            }
                            //Build master summary header
                            writeMasterSummaryHeader( outBuffer, curPlcmtOrg, curReportFormat );
                            int curRowCount = 0;
                            foreach (DataRow curRow in mySummaryDataTable.Rows) {
                                curRowCount++;
                                myProgressInfo.setProgressValue( curRowCount );
                                myProgressInfo.Refresh();

                                curMemberId = curRow["MemberId"].ToString();
                                curAgeGroup = curRow["AgeGroup"].ToString();
                                curReadyToSki = curRow["ReadyToSki"].ToString();

                                //Initialize control fields
                                prevMemberId = curMemberId;
                                prevAgeGroup = curAgeGroup;
                                curRound = (Int16)curRow["RoundOverall"];

                                //Initialize output buffer
                                outLine = new StringBuilder( "<tr>" );

                                //Write skier identification information
                                writeSkierInfo( curRow, curRound, outBuffer );

                                //Write skier performance summary information
                                writeSkierSlalomScore( curRow, curPlcmtOrg, outBuffer );

                                writeSkierTrickScore( curRow, curPlcmtOrg, outBuffer );

                                writeSkierJumpScore( curRow, curPlcmtOrg, outBuffer );

                                writeSkierOverallScore( curRow, curReportFormat, outBuffer );

                                //Write output line to file
                                outLine = new StringBuilder( "</tr>" );
                                outBuffer.WriteLine( outLine.ToString() );
                            }
                            outLine = new StringBuilder( "</table>" );
                            outBuffer.WriteLine( outLine.ToString() );

                            if (curReportFormat.Equals( "Index" )) {
                                outLine = new StringBuilder( "<br/><br/><a href=#PageTop>Return to Index</a><br/></div>" );
                                outBuffer.WriteLine( outLine.ToString() );

                                DataTable mySlalomDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curSummaryNumPrelim );
                                writeIndexSlalomResults( curPlcmtOrg, mySlalomDataTable, outBuffer );

                                DataTable myTrickDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Trick", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curSummaryNumPrelim );
                                writeIndexTrickResults( curPlcmtOrg, myTrickDataTable, outBuffer );

                                DataTable myJumpDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Jump", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curSummaryNumPrelim );
                                writeIndexJumpResults( curPlcmtOrg, myJumpDataTable, outBuffer );

                                DataTable myOverallDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Overall", myTourRules, "best", curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curSummaryNumPrelim );
                                writeIndexOverallResults( curPlcmtOrg, myOverallDataTable, outBuffer );

                                if (isTeamAvailable()) {
                                    //Write team summary
                                    Int16 myNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam);
                                    String curTeamPlcmtOrg = myTourProperties.TeamSummaryPlcmtOrg;
                                    //Int16 myNumPerTeam = Properties.Settings.Default.TeamSummary_NumPerTeam;
                                    //String curTeamPlcmtOrg = Properties.Settings.Default.TeamSummaryPlcmtOrg;
                                    if (curTeamPlcmtOrg.ToLower().Equals( "tour" )
                                        || curTeamPlcmtOrg.ToLower().Equals( "div" )) {
                                    } else {
                                        curTeamPlcmtOrg = "tour";
                                    }
                                    String curTeamPlcmtMethod = "points";
                                    String curTeamDataType = myTourProperties.TeamSummaryDataType;
                                    //String curTeamDataType = Properties.Settings.Default.TeamSummaryDataType;
                                    if (curTeamDataType.ToLower().Equals( "total" )
                                        || curTeamDataType.ToLower().Equals( "best" )
                                        || curTeamDataType.ToLower().Equals( "final" )
                                        || curTeamDataType.ToLower().Equals( "first" )) {
                                    } else {
                                        curTeamDataType = "best";
                                    }
                                    String curTeamPointsMethod = myTourProperties.TeamSummaryPointsMethod;
                                    //String curTeamPointsMethod = Properties.Settings.Default.MasterSummaryV2PointsMethod;
                                    if (curTeamPointsMethod.ToLower().Equals( "nops" )
                                        || curTeamPointsMethod.ToLower().Equals( "plcmt" )
                                        || curTeamPointsMethod.ToLower().Equals( "kbase" )
                                        || curTeamPointsMethod.ToLower().Equals( "ratio" )) {
                                    } else {
                                        curTeamPointsMethod = "nops";
                                    }


                                    String curSortCmd = "";
                                    if (curTeamPlcmtOrg.ToLower().Equals( "div" )) {
                                        curSortCmd = "AgeGroup ASC, PointsSlalom DESC, SkierName ASC";
                                    } else {
                                        curSortCmd = "PointsSlalom DESC, SkierName ASC";
                                    }
                                    mySlalomDataTable.DefaultView.Sort = curSortCmd;
                                    mySlalomDataTable = mySlalomDataTable.DefaultView.ToTable();
                                    DataTable myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( mySlalomDataTable, myTourRow, myNumPerTeam, curTeamDataType, curTeamPlcmtMethod, curTeamPlcmtOrg, curTeamPointsMethod );

                                    if (curTeamPlcmtOrg.ToLower().Equals( "div" )) {
                                        curSortCmd = "AgeGroup ASC, PointsTrick DESC, SkierName ASC";
                                    } else {
                                        curSortCmd = "PointsTrick DESC, SkierName ASC";
                                    }
                                    myTrickDataTable.DefaultView.Sort = curSortCmd;
                                    myTrickDataTable = myTrickDataTable.DefaultView.ToTable();
                                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curTeamDataType, curTeamPlcmtMethod, curTeamPlcmtOrg, curTeamPointsMethod );

                                    if (curTeamPlcmtOrg.ToLower().Equals( "div" )) {
                                        curSortCmd = "AgeGroup ASC, PointsJump DESC, SkierName ASC";
                                    } else {
                                        curSortCmd = "PointsJump DESC, SkierName ASC";
                                    }
                                    myJumpDataTable.DefaultView.Sort = curSortCmd;
                                    myJumpDataTable = myJumpDataTable.DefaultView.ToTable();
                                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curTeamDataType, curTeamPlcmtMethod, curTeamPlcmtOrg, curTeamPointsMethod );

                                    writeIndexTeamResults( myTeamDataTable, myNumPerTeam, curTeamPlcmtOrg, outBuffer );

                                }

                            } else {
                                outLine = new StringBuilder( "</div>" );
                                outBuffer.WriteLine( outLine.ToString() );
                            }

                            //Build file footer and write to file
                            writeFooter( outBuffer );
                            outLine = new StringBuilder( "" );
                            outBuffer.WriteLine( outLine.ToString() );

                            returnStatus = true;
                            outBuffer.Close();

                            myProgressInfo.Close();
                            if (mySummaryDataTable.Rows.Count > 0) {
                                curMsg = mySummaryDataTable.Rows.Count + " skiers found and written";
                            } else {
                                curMsg = "No rows found";
                            }
                        }
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + ":conplete: " + curMsg );
                    } catch (Exception ex) {
                        MessageBox.Show( "Error:" + curMethodName + " Error writing scorebook html file\n\nError: " + ex.Message );
                        curMsg = curMethodName + ":Exception=" + ex.Message;
                        Log.WriteFile( curMsg );
                        returnStatus = false;
                    }
                }
            } else {
                returnStatus = false;
            }

            return returnStatus;
        }

        //Write report identification information
        private bool writeHeader(StreamWriter outBuffer, String inPlcmtOrg) {
            StringBuilder outLine = new StringBuilder( "" );

            outLine = new StringBuilder( "" );
            outLine.Append( "\n<html><head>" );
            outLine.Append( "\n<title>" );
            outLine.Append( mySanctionNum.ToUpper().Trim() + myTourClass.ToUpper().Trim() );
            outLine.Append( " " + myTourRow["Name"].ToString().ToUpper() );
            outLine.Append( "</title>" );
            outLine.Append( "\n<style>" );
            outLine.Append( "\nBody {BACKGROUND-COLOR:#FFFFFF; COLOR:#000000;" );
            outLine.Append( "\nFont-Family:Arial,Helvetica,sans-serif; FONT-SIZE:1.0em; Font-Style:normal;Font-Weight:normal;} " );
            outLine.Append( "\n A:link {text-decoration: none; color: DarkBlue;}" );
            outLine.Append( "\n A:visited {text-decoration: none; color: DarkBlue;}" );
            outLine.Append( "\n A:active {text-decoration: none; color: DarkBlue;}" );
            outLine.Append( "\n A:hover {text-decoration: none; color: Yellow; Background: Maroon;}" );
            outLine.Append( "\n .ReportTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.3em; FONT-WEIGHT:bold;}" );
            outLine.Append( "\n .SectionTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.15em; FONT-WEIGHT:bold;}" );
            outLine.Append( "\n .SubTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.1em; FONT-WEIGHT:normal;}" );
            outLine.Append( "\n .MainContent {Text-Align:Center; margin-left: auto; margin-right: auto; }" );
            outLine.Append( "\n .DataGridView {Text-Align:Center; Border: #AAAAAA .15em solid; margin-left: auto; margin-right: auto; }" );
            outLine.Append( "\n th {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:2px 2px 2px 2px; Font-Size:.8em; Font-Weight:Bold;}" );
            outLine.Append( "\n td {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:2px 2px 2px 2px; Font-Family:Verdana,Helvetica,sans-serif; Font-Size:.8em; Font-Weight:normal;}" );
            outLine.Append( "\n .DataLeft {Text-Align:Left; }" );
            outLine.Append( "\n .DataRight {Text-Align:Right; }" );
            outLine.Append( "\n</style>" );
            outLine.Append( "\n</head><body>" );
            outBuffer.WriteLine( outLine.ToString() );

            String curDateString = DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" );
            outLine = new StringBuilder( "" );
            outLine.Append( "<div Class=\"ReportTitle\">" );
            outLine.Append( mySanctionNum.ToUpper().Trim() );
            outLine.Append( myTourClass.ToUpper().Trim() );
            outLine.Append( " " + myTourRow["Name"].ToString().ToUpper() );
            outLine.Append( "</div>" );
            outLine.Append( "\n<div Class=\"SubTitle\">" );
            outLine.Append( " Master Scorebook " );
            outLine.Append( " as of " + curDateString + "</div>" );
            outBuffer.WriteLine( outLine.ToString() );

            return true;
        }

        private bool writeMasterSummaryHeader(StreamWriter outBuffer, String inPlcmtOrg, String inReportFormat) {
            String curDateString = DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" );
            StringBuilder outLine = new StringBuilder( "" );
            outLine.Append( "\n<div Class=\"MainContent\">" );
            outLine.Append( "\n<br/><hr/><a name=MasterSummary></a>" );
            outLine.Append( "\n<div Class=\"SectionTitle\">" );
            outLine.Append( " Master Scorebook " );
            outLine.Append( " as of " + curDateString + "</div>" );
            outLine.Append( "\n<Table Class=\"DataGridView\">" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( "<tr>" );
            outLine.Append( "<th colspan=\"2\">&nbsp;</th>" );
            outLine.Append( "<th>|</th>" );

            String curColNumSlalom = "8";
            String curColNumTrick = "5";
            String curColNumJump = "6";
            if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                curColNumSlalom = "9";
                curColNumTrick = "6";
                curColNumJump = "7";
            }
            outLine.Append( "<th colspan=\"" + curColNumSlalom + "\">-------- Slalom --------</th>" );
            outLine.Append( "<th>|</th>" );

            outLine.Append( "<th colspan=\"" + curColNumTrick + "\">-------- Trick --------</th>" );
            outLine.Append( "<th>|</th>" );

            outLine.Append( "<th colspan=\"" + curColNumJump + "\">-------- Jump --------</th>" );
            outLine.Append( "<th>|</th>" );
            String curNumOverallCols = "7";
            if (inReportFormat.Equals( "Index" )) {
                curNumOverallCols = "6";
            }

            outLine.Append( "<th colspan=\"" + curNumOverallCols + "7\">-------- Overall --------</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( "<tr>" );
            outLine.Append( "<th>SkierName</th>" );
            outLine.Append( "<th>Div</th>" );
            outLine.Append( "<th>|</th>" );
            outLine.Append( "<th>Class</th>" );
            if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                outLine.Append( "<th>Grp</th>" );
            }
            outLine.Append( "<th>Buoys</th>" );
            outLine.Append( "<th>Rope</th>" );
            outLine.Append( "<th>Rope</th>" );
            outLine.Append( "<th>MPH</th>" );
            outLine.Append( "<th>KPH</th>" );
            outLine.Append( "<th>Score</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            
            outLine.Append( "<th>|</th>" );
            outLine.Append( "<th>Class</th>" );
            if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                outLine.Append( "<th>Grp</th>" );
            }
            outLine.Append( "<th>Pass1</th>" );
            outLine.Append( "<th>Pass2</th>" );
            outLine.Append( "<th>Score</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            
            outLine.Append( "<th>|</th>" );
            outLine.Append( "<th>Class</th>" );
            if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                outLine.Append( "<th>Grp</th>" );
            }
            outLine.Append( "<th>Ht</th>" );
            outLine.Append( "<th>Spd</th>" );
            outLine.Append( "<th>Feet</th>" );
            outLine.Append( "<th>Meters</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            
            outLine.Append( "<th>|</th>" );
            outLine.Append( "<th>Slalom</th>" );
            outLine.Append( "<th>Trick</th>" );
            outLine.Append( "<th>Jump</th>" );
            if (inReportFormat.Equals( "Index" )) {
            } else {
                outLine.Append( "<th>Score</th>" );
            }
            outLine.Append( "<th>Qual</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Rd</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outBuffer.WriteLine( outLine.ToString() );

            return true;
        }

        private void writeIndexHeader(StreamWriter outBuffer, String inPlcmtOrg, DataTable inOverallScores) {
            String curDiv = "", curDivName = "";
            int curDivCountSlalom = 0, curDivCountTrick = 0, curDivCountJump = 0;
            AgeGroupDropdownList myAgeGroupDropdownList = new AgeGroupDropdownList( myTourRow );

            StringBuilder outLine = new StringBuilder( "" );
            outLine.Append( "\n<br/><br/><a name=PageTop></a><div Class=\"SectionTitle\">" );
            outLine.Append( "<br/>Index of Scoring Results" );
            outLine.Append( "</div>" );
            outLine.Append( "\n<div class=\"MainContent\">" );
            outLine.Append( "<br/><a href=#MasterSummary>Go to MasterSummary</a>" );
            if (isTeamAvailable()) {
                outLine.Append( "<br/><a href=#TeamCombined>Team Results</a>" );
            }
            outLine.Append( "<br/><br/>\n<Table Class=\"DataGridView\">" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( "\n<tr><th>Division</th><td>Slalom</td><td>Tricks</td><td>Jump</td><td>Overall</td></tr>" );
            outBuffer.WriteLine( outLine.ToString() );

            foreach (DataRow curRowDiv in myAgeGroupDropdownList.AgeDivDataTable.Rows) {
                outLine = new StringBuilder( "" );
                curDiv = (String)curRowDiv["Division"];
                curDivName = (String)curRowDiv["DivisionName"];

                curDivCountSlalom = getSkierCountDiv( mySanctionNum, "Slalom", curDiv );
                curDivCountTrick = getSkierCountDiv( mySanctionNum, "Trick", curDiv );
                curDivCountJump = getSkierCountDiv( mySanctionNum, "Jump", curDiv );

                if (curDivCountSlalom > 0 || curDivCountTrick > 0 || curDivCountJump > 0) {
                    outLine.Append( "\n<tr><th>" + curDivName + "</th>" );
                    outLine.Append( "<td>" );
                    if (curDivCountSlalom > 0) {
                        outLine.Append( "<a href=#SlalomResults" + curDiv + ">" + curDivName + " Slalom</a>" );
                    } else {
                        outLine.Append( "&nbsp;" );
                    }
                    outLine.Append( "</td>" );

                    outLine.Append( "<td>" );
                    if (curDivCountTrick > 0) {
                        outLine.Append( "<a href=#TrickResults" + curDiv + ">" + curDivName + " Trick</a>" );
                    } else {
                        outLine.Append( "&nbsp;" );
                    }
                    outLine.Append( "</td>" );

                    outLine.Append( "<td>" );
                    if (curDivCountJump > 0) {
                        outLine.Append( "<a href=#JumpResults" + curDiv + ">" + curDivName + " Jump</a>" );
                    } else {
                        outLine.Append( "&nbsp;" );
                    }
                    outLine.Append( "</td>" );

                    outLine.Append( "<td>" );
                    if (curDivCountSlalom > 0 || curDivCountTrick > 0 | curDivCountJump > 0) {
                        outLine.Append( "<a href=#OverallResults" + curDiv + ">" + curDivName + " Overall</a>" );
                    } else {
                        outLine.Append( "&nbsp;" );
                    }
                    outLine.Append( "</td>" );

                    outLine.Append( "</tr>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }
            }

            outLine = new StringBuilder( "" );
            outLine.Append( "\n</Table></div>" );
            outBuffer.WriteLine( outLine.ToString() );

            return;

        }

        private bool writeFooter(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            //outLine.Append( "</table>" );
            outLine.Append( "<br><div Class=\"footer\">" );
            outLine.Append( "Printed by " );
            outLine.Append( Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion );
            outLine.Append( "\n</div>" );
            outLine.Append( "\n</div>\n</body></html>" );
            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeSkierInfo(DataRow inRow, int inRound, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );

            try {
                if (inRound == 1) {
                    outLine.Append( "<td>" + inRow["SkierName"].ToString() + "</td>" );
                    outLine.Append( "<td>" + inRow["AgeGroup"].ToString() + "</td>" );
                } else {
                    outLine.Append( "<td colspan=\"2\">&nbsp;</td>" );
                }
            } catch (Exception ex) {
                outLine.Append( "<td colspan=\"2\">&nbsp;</td>" );
                MessageBox.Show( "Error writing skier info to Scorebook html: " + "\n\nError: " + ex.Message );
            }

            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeSkierSlalomScore(DataRow inScoreSummary, String inPlcmtOrg, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEventClass = "", curColNum = "";
            Int16 curRound = 0;

            bool isRoundScored = true;
            try {
                if (inScoreSummary == null) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundSlalom"];
                        curEventClass = (String)inScoreSummary["EventClassSlalom"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
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

                outLine.Append( "<td>&nbsp;</td>" );
                if (isRoundScored) {
                    outLine.Append( "<td>" + " " + curEventClass + "</td>" );
                    if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                        try {
                            outLine.Append( "<td>" + (String)inScoreSummary["EventGroupSlalom"] + "</td>" );
                        } catch {
                            outLine.Append( "<td>&nbsp;</td>" );
                        }
                    }
                    Decimal curSlalomScore = 0;
                    try {
                        curSlalomScore = (Decimal)inScoreSummary["ScoreSlalom"];
                    } catch {
                        curSlalomScore = 0M;
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)inScoreSummary["FinalPassScore"] ).ToString( "0.00" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    if (curSlalomScore < 6) {
                        String curValue = getIwwfSlalomMin( (String)inScoreSummary["AgeGroup"] );
                        String[] curList = curValue.Split( ',' );
                        outLine.Append( "<td>15 Off</td>" );
                        outLine.Append( "<td>1825</td>" );
                        outLine.Append( "<td>" + curList[1].Trim().Substring( 0, 2 ) + "</td>" );
                        outLine.Append( "<td>" + curList[0].Trim().Substring( 0, 2 ) + "</td>" );
                    } else {
                        try {
                            outLine.Append( "<td>" + (String)inScoreSummary["FinalLenOff"] + "</td>" );
                        } catch {
                            outLine.Append( "<td>Long</td>" );
                        }
                        try {
                            outLine.Append( "<td>" + ( Convert.ToDecimal( (String)inScoreSummary["FinalLen"] ) * 100 ).ToString( "0000" ) + "</td>" );
                        } catch {
                            outLine.Append( "<td>2300</td>" );
                        }
                        try {
                            outLine.Append( "<td>" + ( (Byte)inScoreSummary["FinalSpeedMph"] ).ToString( "00" ) + "</td>" );
                            outLine.Append( "<td>" + ( (Byte)inScoreSummary["FinalSpeedKph"] ).ToString( "00" ) + "</td>" );
                        } catch {
                            outLine.Append( "<td>15.5</td>" );
                            outLine.Append( "<td>25</td>" );
                        }
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)inScoreSummary["ScoreSlalom"] ).ToString( "##0.00" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    if (curRound == 1) {
                        if ( ( (String) inScoreSummary["PlcmtSlalom"] ).Length > 0 ) {
                            if ( ( (String) inScoreSummary["PlcmtSlalom"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) inScoreSummary["PlcmtSlalom"] + "</td>");
                            }
                        } else {
                            outLine.Append("<td>&nbsp;</td>");
                        }
                    } else {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                } else {
                    curColNum = "8";
                    if (curRound == 1) {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    } else {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error writing slalom scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Slalom Error: " + ex.Message + "</td>" );
            }

            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeSkierTrickScore(DataRow inScoreSummary, String inPlcmtOrg, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curEventClass = "", curColNum = "";
            Int16 curRound = 0;

            bool isRoundScored = true;
            try {
                if (inScoreSummary == null) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundTrick"];
                        curEventClass = (String)inScoreSummary["EventClassTrick"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
                            curEventClass = " ";
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

                outLine.Append( "<td>&nbsp;</td>" );
                if (isRoundScored) {
                    outLine.Append( "<td>" + curEventClass + "</td>" );
                    if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                        try {
                            outLine.Append( "<td>" + (String)inScoreSummary["EventGroupTrick"] + "</td>" );
                        } catch {
                            outLine.Append( "<td>&nbsp;</td>" );
                        }
                    }
                    try {
                        outLine.Append( "<td>" + ( (Int16)inScoreSummary["Pass1Trick"] ).ToString( "###0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Int16)inScoreSummary["Pass2Trick"] ).ToString( "###0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Int16)inScoreSummary["ScoreTrick"] ).ToString( "####0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    if (curRound == 1) {
                        if ( ( (String) inScoreSummary["PlcmtTrick"] ).Length > 0 ) {
                            if ( ( (String) inScoreSummary["PlcmtTrick"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) inScoreSummary["PlcmtTrick"] + "</td>");
                            }
                        } else {
                            outLine.Append("<td>&nbsp;</td>");
                        }
                    } else {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                } else {
                    curColNum = "5";
                    if (curRound == 1) {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    } else {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error writing trick scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Trick Error: " + ex.Message + "</td>" );
            }

            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeSkierJumpScore(DataRow inScoreSummary, String inPlcmtOrg, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            Decimal curRampHeight;
            String curEventClass = "", curValue = "", curColNum = "";
            Int16 curRound = 0;

            bool isRoundScored = true;
            try {
                if (inScoreSummary == null) {
                    isRoundScored = false;
                } else {
                    try {
                        curRound = (Int16)inScoreSummary["RoundJump"];
                        curEventClass = (String)inScoreSummary["EventClassJump"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
                            curEventClass = " ";
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

                outLine.Append( "<td>&nbsp;</td>" );
                if (isRoundScored) {
                    outLine.Append( "<td>" + " " + curEventClass + "</td>" );
                    if (inPlcmtOrg.ToLower().Equals( "divgr" )) {
                        try {
                            outLine.Append( "<td>" + (String)inScoreSummary["EventGroupJump"] + "</td>" );
                        } catch {
                            outLine.Append( "<td>&nbsp;</td>" );
                        }
                    }
                    try {
                        curRampHeight = (Decimal)inScoreSummary["RampHeight"];
                        if (curRampHeight == 5) {
                            curValue = "5";
                        } else if (curRampHeight == 5.5M) {
                            curValue = "H";
                        } else if (curRampHeight == 6) {
                            curValue = "6";
                        } else if (curRampHeight < 5) {
                            curValue = "4";
                        } else {
                            curValue = "";
                        }
                        outLine.Append( "<td>" + curValue + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        curValue = ( (Byte)inScoreSummary["SpeedKphJump"] ).ToString( "00" );
                    } catch {
                        curValue = "00";
                    }
                    outLine.Append( "<td>" + " " + curValue + "</td>" );
                    try {
                        outLine.Append( "<td>" + ( (Decimal)inScoreSummary["ScoreFeet"] ).ToString( "##0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)inScoreSummary["ScoreMeters"] ).ToString( "##.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    if (curRound == 1) {
                        if ( ( (String) inScoreSummary["PlcmtJump"] ).Length > 0 ) {
                            if ( ( (String) inScoreSummary["PlcmtJump"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) inScoreSummary["PlcmtJump"] + "</td>");
                            }
                        } else {
                            outLine.Append("<td>&nbsp;</td>");
                        }
                    } else {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                } else {
                    curColNum = "6";
                    if (curRound == 1) {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    } else {
                        outLine.Append( "<td colspan=\"" + curColNum + "\">&nbsp;</td>" );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error writing jump scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Jump Error: " + ex.Message + "</td>" );
            }

            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeSkierOverallScore(DataRow inSummaryRow, String inReportFormat, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            Decimal curNopsSlalom = 0, curNopsTrick = 0, curNopsJump = 0, curNopsOverall = 0;
            String curAgeGroup = "", curEventClass = "";

            outLine.Append( "<td>&nbsp;</td>" );

            try {
                if (inSummaryRow == null) {
                    outLine.Append( "<td>&nbsp;</td>" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassSlalom"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
                            outLine.Append( "<td>&nbsp;</td>" );
                        } else {
                            curNopsSlalom = (Decimal)inSummaryRow["PointsSlalom"];
                            outLine.Append( "<td>" + curNopsSlalom.ToString( "###0" ) + "</td>" );
                            curNopsOverall = curNopsSlalom;
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                }
                if (inSummaryRow == null) {
                    outLine.Append( "<td>&nbsp;</td>" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassTrick"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
                            outLine.Append( "<td>&nbsp;</td>" );
                        } else {
                            curNopsTrick = (Decimal)inSummaryRow["PointsTrick"];
                            outLine.Append( "<td>" + " " + curNopsTrick.ToString( "###0" ) );
                            curNopsOverall += curNopsTrick;
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                }

                curAgeGroup = (String)inSummaryRow["AgeGroup"];
                if (inSummaryRow == null) {
                    outLine.Append( "<td>&nbsp;</td>" );
                } else {
                    try {
                        curEventClass = (String)inSummaryRow["EventClassJump"];
                        if (curEventClass.Length > 1) curEventClass = curEventClass.Substring( 0, 1 );
                        if (curEventClass.Length == 0) {
                            outLine.Append( "<td>&nbsp;</td>" );
                        } else {
                            curNopsJump = (Decimal)inSummaryRow["PointsJump"];
                            outLine.Append( "<td>" + " " + curNopsJump.ToString( "###0" ) + "</td>" );
                            if (curAgeGroup.ToUpper().Equals( "B1" ) || curAgeGroup.ToUpper().Equals( "G1" )) {
                            } else {
                                curNopsOverall += curNopsJump;
                            }
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                }
                if (( (String)inSummaryRow["QualifyOverall"] ).ToLower().Equals( "yes" )) {
                    if (inReportFormat.Equals( "Index" )) {
                    } else {
                        try {
                            outLine.Append( "<td>" + " " + curNopsOverall.ToString( "###0.0" ) + "</td>" );
                        } catch {
                            outLine.Append( "<td>&nbsp;</td>" );
                        }
                    }
                    try {
                        outLine.Append( "<td>" + " " + (String)inSummaryRow["QualifyOverall"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + " " + (String)inSummaryRow["PlcmtOverall"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + " " + (Int16)inSummaryRow["RoundOverall"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                } else {
                    outLine.Append( "<td>&nbsp;</td>" );
                    outLine.Append( "<td>&nbsp;</td>" );
                    outLine.Append( "<td>&nbsp;</td>" );
                    outLine.Append( "<td>&nbsp;</td>" );
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error writing overall scores to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Overall Error: " + ex.Message + "</td>" );
            }

            outBuffer.WriteLine( outLine.ToString() );
            return true;
        }

        private bool writeIndexSlalomResults(String curPlcmtOrg, DataTable curEventResults, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curSkierName = "", curMemberId = "", curDiv = "", curGroup = "", curReportGroup = "", curEventClass = "", prevReportGroup = "", curValue = "", prevMemberId = "", prevDiv = "";
            //DataRow[] curFindRows;

            try {
                foreach (DataRow curRow in curEventResults.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curSkierName = (String)curRow["SkierName"];
                    curDiv = (String)curRow["AgeGroup"];
                    curGroup = (String)curRow["EventGroup"];
                    //ready to ski?
                    if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else {
                        curReportGroup = curDiv;
                    }

                    if (curReportGroup != prevReportGroup) {
                        if (prevReportGroup.Length > 0) {
                            //write footer
                            outLine = new StringBuilder( "" );
                            outLine.Append( "\n</Table>" );
                            outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                            outLine.Append( "\n</div>" );
                            outBuffer.WriteLine( outLine.ToString() );
                        }

                        #region write header
                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<br/><br/><hr/><a name=SlalomResults" + curReportGroup + "></a>" );
                        outLine.Append( "\n<div Class=\"SectionTitle\">" );
                        outLine.Append( curReportGroup + " Slalom Results" );
                        outLine.Append( "</div>" );
                        outLine.Append( "\n<div class=\"MainContent\">" );
                        outBuffer.WriteLine( outLine.ToString() );

                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<Table Class=\"DataGridView\">" );
                        outLine.Append( "\n<tr>" );
                        outLine.Append( "<th>Skier</th>" );
                        outLine.Append( "<th>Home Town</th>" );
                        outLine.Append( "<th>Age</th>" );
                        outLine.Append( "<th>Div</th>" );
                        if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                            try {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            } catch {
                                outLine.Append( "<td>&nbsp;</td>" );
                            }
                        }
                        outLine.Append( "<th>Team</th>" );
                        outLine.Append( "<th>Class</th>" );
                        outLine.Append( "<th>Rd</th>" );

                        outLine.Append( "<th>Final<br/>Buoys</th>" );
                        outLine.Append( "<th>Final<br/>Rope Len</th>" );
                        outLine.Append( "<th>Final<br/>Speed</th>" );
                        outLine.Append( "<th>Score</th>" );
                        outLine.Append( "<th>Points</th>" );
                        outLine.Append( "<th>Plcmt</th>" );
                        outLine.Append( "</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                        #endregion
                    }

                    #region write results data
                    outLine = new StringBuilder( "" );
                    curEventClass = (String)curRow["EventClassSlalom"];
                    if (curEventClass.Length > 0) {
                        outLine.Append( "\n<tr>" );
                        if (curMemberId == prevMemberId && curDiv == prevDiv) {
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            }
                        } else {
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["SkierName"] + "</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">" );
                            try {
                                outLine.Append( (String)curRow["City"] );
                                try {
                                    outLine.Append( ", " + (String)curRow["State"] );
                                } catch {
                                }
                            } catch {
                                try {
                                    outLine.Append( (String)curRow["State"] );
                                } catch {
                                }
                            }
                            outLine.Append( "</td>" );
                            try {
                                outLine.Append( "\n<td>" + ( (Byte)curRow["SkiYearAge"] ).ToString() + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["AgeGroup"] + "</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            }
                            try {
                                outLine.Append( "\n<td>" + (String)curRow["TeamSlalom"] + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                        }
                        try {
                            outLine.Append( "\n<td>" + (String)curRow["EventClassSlalom"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td>" + (Int16)curRow["RoundSlalom"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["FinalPassScore"] ).ToString( "0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }

                        Decimal curSlalomScore = 0;
                        try {
                            curSlalomScore = (Decimal)curRow["ScoreSlalom"];
                        } catch {
                            curSlalomScore = 0M;
                        }
                        if (curSlalomScore < 6) {
                            curValue = getIwwfSlalomMin( curDiv );
                            String[] curList = curValue.Split( ',' );
                            outLine.Append( "<td>18.25 15 Off</td>" );
                            outLine.Append( "<td>" + curValue + "</td>" );
                        } else {
                            try {
                                outLine.Append( "\n<td>" );
                                outLine.Append( (String)curRow["FinalLen"] + " " + (String)curRow["FinalLenOff"] );
                                outLine.Append( "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            try {
                                outLine.Append( "\n<td>"
                                    + ( (Byte)curRow["FinalSpeedKph"] ).ToString() + "kph "
                                    + ( (Byte)curRow["FinalSpeedMph"] ).ToString() + "mph "
                                    + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["ScoreSlalom"] ).ToString( "##0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsSlalom"] ).ToString( "###0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtSlalom"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        outLine.Append( "\n</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                    }
                    #endregion

                    prevReportGroup = curReportGroup;
                    prevMemberId = curMemberId;
                    prevDiv = curDiv;
                }

                if (prevReportGroup.Length > 0) {
                    //write footer
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n</Table>" );
                    outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                    outLine.Append( "\n</div><br/><br/>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Error writing slalom results to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Slalom Error: " + ex.Message + "</td>" );
                return false;
            }
        }

        private bool writeIndexTrickResults(String curPlcmtOrg, DataTable curEventResults, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curSkierName = "", curMemberId = "", curDiv = "", curGroup = "", curReportGroup = "", curEventClass = "", prevReportGroup = "", curValue = "", prevMemberId = "", prevDiv = "";

            try {
                foreach (DataRow curRow in curEventResults.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curSkierName = (String)curRow["SkierName"];
                    curDiv = (String)curRow["AgeGroup"];
                    curGroup = (String)curRow["EventGroup"];
                    //ready to ski?
                    if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else {
                        curReportGroup = curDiv;
                    }

                    if (curReportGroup != prevReportGroup) {
                        if (prevReportGroup.Length > 0) {
                            //write footer
                            outLine = new StringBuilder( "" );
                            outLine.Append( "\n</Table>" );
                            outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                            outLine.Append( "\n</div>" );
                            outBuffer.WriteLine( outLine.ToString() );
                        }

                        //write header
                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<br/><br/><hr/><a name=TrickResults" + curReportGroup + "></a>" );
                        outLine.Append( "\n<div Class=\"SectionTitle\">" );
                        outLine.Append( curReportGroup + " Trick Results" );
                        outLine.Append( "</div>" );
                        outLine.Append( "\n<div class=\"MainContent\">" );
                        outBuffer.WriteLine( outLine.ToString() );

                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<Table Class=\"DataGridView\">" );
                        outLine.Append( "\n<tr>" );
                        outLine.Append( "<th>Skier</th>" );
                        outLine.Append( "<th>Home Town</th>" );
                        outLine.Append( "<th>Age</th>" );
                        outLine.Append( "<th>Div</th>" );
                        if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                            try {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            } catch {
                                outLine.Append( "<td>&nbsp;</td>" );
                            }
                        }
                        
                        outLine.Append( "<th>Team</th>" );
                        outLine.Append( "<th>Class</th>" );
                        outLine.Append( "<th>Rd</th>" );
                        
                        outLine.Append( "<th>Pass 1</th>" );
                        outLine.Append( "<th>Pass 2</th>" );
                        outLine.Append( "<th>Score</th>" );
                        outLine.Append( "<th>Points</th>" );
                        outLine.Append( "<th>Plcmt</th>" );
                        outLine.Append( "</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );

                    }

                    //write results data
                    outLine = new StringBuilder( "" );
                    curEventClass = (String)curRow["EventClassTrick"];
                    if (curEventClass.Length > 0) {
                        outLine.Append( "\n<tr>" );
                        if (curMemberId == prevMemberId && curDiv == prevDiv) {
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            }
                        } else {
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["SkierName"] + "</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">" );
                            try {
                                outLine.Append( (String)curRow["City"] );
                                try {
                                    outLine.Append( ", " + (String)curRow["State"] );
                                } catch {
                                }
                            } catch {
                                try {
                                    outLine.Append( (String)curRow["State"] );
                                } catch {
                                }
                            }
                            outLine.Append( "</td>" );
                            try {
                                outLine.Append( "\n<td>" + ( (Byte)curRow["SkiYearAge"] ).ToString() + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["AgeGroup"] + "</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            }
                            try {
                                outLine.Append( "\n<td>" + (String)curRow["TeamTrick"] + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                        }
                        try {
                            outLine.Append( "\n<td>" + (String)curRow["EventClassTrick"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td>" + (Int16)curRow["RoundTrick"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Int16)curRow["Pass1Trick"] ).ToString( "####0" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Int16)curRow["Pass2Trick"] ).ToString( "####0" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Int16)curRow["ScoreTrick"] ).ToString( "####0" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsTrick"] ).ToString( "###0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtTrick"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        outLine.Append( "\n</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    prevReportGroup = curReportGroup;
                    prevMemberId = curMemberId;
                    prevDiv = curDiv;
                }

                if (prevReportGroup.Length > 0) {
                    //write footer
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n</Table>" );
                    outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                    outLine.Append( "\n</div><br/><br/>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Error writing trick results to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Trick Error: " + ex.Message + "</td>" );
                return false;
            }
        }

        private bool writeIndexJumpResults(String curPlcmtOrg, DataTable curEventResults, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curSkierName = "", curMemberId = "", curDiv = "", curGroup = "", curReportGroup = "", curEventClass = "", prevReportGroup = "", curValue = "", prevMemberId = "", prevDiv = "";

            try {
                foreach (DataRow curRow in curEventResults.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curSkierName = (String)curRow["SkierName"];
                    curDiv = (String)curRow["AgeGroup"];
                    curGroup = (String)curRow["EventGroup"];
                    //ready to ski?
                    if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else {
                        curReportGroup = curDiv;
                    }

                    if (curReportGroup != prevReportGroup) {
                        if (prevReportGroup.Length > 0) {
                            //write footer
                            outLine = new StringBuilder( "" );
                            outLine.Append( "\n</Table>" );
                            outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                            outLine.Append( "\n</div>" );
                            outBuffer.WriteLine( outLine.ToString() );
                        }

                        //write header
                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<br/><br/><hr/><a name=JumpResults" + curReportGroup + "></a>" );
                        outLine.Append( "\n<div Class=\"SectionTitle\">" );
                        outLine.Append( curReportGroup + " Jump Results" );
                        outLine.Append( "</div>" );
                        outLine.Append( "\n<div class=\"MainContent\">" );
                        outBuffer.WriteLine( outLine.ToString() );

                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<Table Class=\"DataGridView\">" );
                        outLine.Append( "\n<tr>" );
                        outLine.Append( "<th>Skier</th>" );
                        outLine.Append( "<th>Home Town</th>" );
                        outLine.Append( "<th>Age</th>" );
                        outLine.Append( "<th>Div</th>" );
                        if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                            try {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            } catch {
                                outLine.Append( "<td>&nbsp;</td>" );
                            }
                        }
                        outLine.Append( "<th>Team</th>" );
                        outLine.Append( "<th>Class</th>" );
                        outLine.Append( "<th>Rd</th>" );

                        outLine.Append( "<th>Ramp<br/>Ht</th>" );
                        outLine.Append( "<th>Speed<br/>KPH</th>" );
                        outLine.Append( "<th>Feet</th>" );
                        outLine.Append( "<th>Meters</th>" );
                        outLine.Append( "<th>Points</th>" );
                        outLine.Append( "<th>Plcmt</th>" );
                        outLine.Append( "</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );

                    }

                    //write results data
                    outLine = new StringBuilder( "" );
                    curEventClass = (String)curRow["EventClassJump"];
                    if (curEventClass.Length > 0) {
                        outLine.Append( "\n<tr>" );
                        if (curMemberId == prevMemberId && curDiv == prevDiv) {
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "\n<td class=\"DataLeft\">&nbsp;</td>" );
                            }
                        } else {
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["SkierName"] + "</td>" );
                            outLine.Append( "\n<td class=\"DataLeft\">" );
                            try {
                                outLine.Append( (String)curRow["City"] );
                                try {
                                    outLine.Append( ", " + (String)curRow["State"] );
                                } catch {
                                }
                            } catch {
                                try {
                                    outLine.Append( (String)curRow["State"] );
                                } catch {
                                }
                            }
                            outLine.Append( "</td>" );
                            try {
                                outLine.Append( "\n<td>" + ( (Byte)curRow["SkiYearAge"] ).ToString() + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["AgeGroup"] + "</td>" );
                            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            }
                            try {
                                outLine.Append( "\n<td>" + (String)curRow["TeamJump"] + "</td>" );
                            } catch {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                        }
                        try {
                            outLine.Append( "\n<td>" + (String)curRow["EventClassJump"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td>" + (Int16)curRow["RoundJump"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }

                        curValue = "";
                        try {
                            Decimal curRampHeight = (Decimal)curRow["RampHeight"];
                            if (curRampHeight == 5) {
                                curValue = "5";
                            } else if (curRampHeight == 5.5M) {
                                curValue = "5.5";
                            } else if (curRampHeight == 6) {
                                curValue = "6";
                            } else if (curRampHeight < 5) {
                                curValue = "4.5";
                            } else {
                                curValue = "";
                            }
                            outLine.Append( "<td>" + curValue + "</td>" );
                        } catch {
                            outLine.Append( "<td>&nbsp;</td>" );
                        }
                        try {
                            curValue = ( (Byte)curRow["SpeedKphJump"] ).ToString( "00" );
                        } catch {
                            curValue = "00";
                        }
                        outLine.Append( "<td>" + " " + curValue + "</td>" );

                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["ScoreFeet"] ).ToString( "##0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["ScoreMeters"] ).ToString( "##0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsJump"] ).ToString( "###0.00" ) + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        try {
                            outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtJump"] + "</td>" );
                        } catch {
                            outLine.Append( "\n<td>&nbsp;</td>" );
                        }
                        outLine.Append( "\n</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    prevReportGroup = curReportGroup;
                    prevMemberId = curMemberId;
                    prevDiv = curDiv;
                }

                if (prevReportGroup.Length > 0) {
                    //write footer
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n</Table>" );
                    outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                    outLine.Append( "\n</div><br/><br/>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Error writing trick results to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Trick Error: " + ex.Message + "</td>" );
                return false;
            }
        }

        private bool writeIndexOverallResults(String curPlcmtOrg, DataTable curEventResults, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder( "" );
            String curSkierName = "", curDiv = "", curGroup = "", curReportGroup = "", prevReportGroup = "";
            int curPlcmt = 1;

            DataTable curDataTable = null;
            if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                curEventResults.DefaultView.Sort = "AgeGroup ASC, EventGroup ASC, ScoreOverall DESC";
                curEventResults.DefaultView.RowFilter = "QualifyOverall = 'Yes'";
            } else {
                curEventResults.DefaultView.Sort = "AgeGroup ASC, EventGroup ASC, ScoreOverall DESC";
                curEventResults.DefaultView.RowFilter = "QualifyOverall = 'Yes'";
            }
            //curEventResults
            curDataTable = curEventResults.DefaultView.ToTable();

            try {
                foreach (DataRow curRow in curDataTable.Rows) {
                    curSkierName = (String)curRow["SkierName"];
                    curDiv = (String)curRow["AgeGroup"];
                    curGroup = (String)curRow["EventGroup"];
                    //ready to ski?
                    if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                        curReportGroup = curDiv + "-" + curGroup;
                    } else {
                        curReportGroup = curDiv;
                    }

                    if (curReportGroup != prevReportGroup) {
                        curPlcmt = 1;
                        if (prevReportGroup.Length > 0) {
                            //write footer
                            outLine = new StringBuilder( "" );
                            outLine.Append( "\n</Table>" );
                            outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                            outLine.Append( "\n</div>" );
                            outBuffer.WriteLine( outLine.ToString() );
                        }

                        //write header
                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<br/><br/><hr/><a name=OverallResults" + curReportGroup + "></a>" );
                        outLine.Append( "\n<div Class=\"SectionTitle\">" );
                        outLine.Append( curReportGroup + " Overall Results" );
                        outLine.Append( "</div>" );
                        outLine.Append( "\n<div class=\"MainContent\">" );
                        outBuffer.WriteLine( outLine.ToString() );

                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<Table Class=\"DataGridView\">" );
                        outLine.Append( "\n<tr>" );
                        outLine.Append( "<th>Skier</th>" );
                        outLine.Append( "<th>Div</th>" );
                        if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                            try {
                                outLine.Append( "<td>" + curGroup + "</td>" );
                            } catch {
                                outLine.Append( "<td>&nbsp;</td>" );
                            }
                        }
                        outLine.Append( "<th>Team</th>" );
                        outLine.Append( "<th>Rd</th>" );
                        outLine.Append( "<th>Slalom</th>" );
                        outLine.Append( "<th>Rd</th>" );
                        outLine.Append( "<th>Trick</th>" );
                        outLine.Append( "<th>Rd</th>" );
                        outLine.Append( "<th>Jump</th>" );
                        outLine.Append( "<th>Overall</th>" );
                        outLine.Append( "<th>Plcmt</th>" );
                        outLine.Append( "</tr>" );
                        outBuffer.WriteLine( outLine.ToString() );

                    }

                    //write results data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n<tr>" );
                    outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["SkierName"] + "</td>" );
                    outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["AgeGroup"] + "</td>" );
                    if (curPlcmtOrg.ToLower().Equals( "divgr" )) {
                        outLine.Append( "<td>" + curGroup + "</td>" );
                    }
                    String curValue = "";
                    if (curRow["TeamSlalom"].ToString().Trim().Length > 0) {
                        curValue = curRow["TeamSlalom"].ToString().Trim();
                    } else if (curRow["TeamTrick"].ToString().Trim().Length > 0) {
                        curValue = curRow["TeamTrick"].ToString().Trim();
                    } else if (curRow["TeamJump"].ToString().Trim().Length > 0) {
                        curValue = curRow["TeamJump"].ToString().Trim();
                    } else {
                        curValue = "   ";
                    }
                    outLine.Append( "\n<td>" + curValue + "</td>" );
                    try {
                        outLine.Append( "\n<td>" + (Int16)curRow["RoundSlalom"] + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsSlalom"] ).ToString( "###0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td>" + (Int16)curRow["RoundTrick"] + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsTrick"] ).ToString( "###0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td>" + (Int16)curRow["RoundJump"] + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsJump"] ).ToString( "###0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["ScoreOverall"] ).ToString( "###0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "\n<td class=\"DataRight\">" + curPlcmt.ToString( "##0" ) + "</td>" );
                    } catch {
                        outLine.Append( "\n<td>&nbsp;</td>" );
                    }
                    outLine.Append( "\n</tr>" );
                    outBuffer.WriteLine( outLine.ToString() );

                    prevReportGroup = curReportGroup;
                    curPlcmt++;
                }

                if (prevReportGroup.Length > 0) {
                    //write footer
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n</Table>" );
                    outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                    outLine.Append( "\n</div><br/><br/>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Error writing overall results to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Overall Error: " + ex.Message + "</td>" );
                return false;
            }
        }

        private bool writeIndexTeamResults(DataTable inTeamDataTable, Int16 inNumPerTeam, String inPlcmtOrg, StreamWriter outBuffer) {
            int curTeamPlcmt = 0;
            StringBuilder outLine = new StringBuilder( "" );
            try {
                String curSortCmd = "";
                if (inPlcmtOrg.ToLower().Equals( "div" )) {
                    curSortCmd = "DivOrder ASC, Div ASC, TeamScoreTotal DESC";
                } else {
                    curSortCmd = "TeamScoreTotal DESC";
                }
                inTeamDataTable.DefaultView.Sort = curSortCmd;
                DataTable curTeamDataTable = inTeamDataTable.DefaultView.ToTable();

                outLine = new StringBuilder( "" );
                outLine.Append( "\n<br/><br/><hr/><a name=TeamCombined></a>" );
                outLine.Append( "\n<div Class=\"SectionTitle\">" );
                outLine.Append( "Combined Team Results" );
                outLine.Append( "</div>" );
                outLine.Append( "\n<div class=\"MainContent\">" );
                outBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "\n<Table Class=\"DataGridView\">" );
                outLine.Append( "\n<tr>" );
                outLine.Append( "<th>Plcmt</th>" );
                outLine.Append( "<th>Team</th>" );
                outLine.Append( "<th>Name</th>" );
                outLine.Append( "<th>Slalom</th>" );
                outLine.Append( "<th>Plcmt</th>" );
                outLine.Append( "<th>Trick</th>" );
                outLine.Append( "<th>Plcmt</th>" );
                outLine.Append( "<th>Jump</th>" );
                outLine.Append( "<th>Plcmt</th>" );
                outLine.Append( "<th>Total</th>" );
                outLine.Append( "</tr>" );
                outBuffer.WriteLine( outLine.ToString() );

                foreach (DataRow curRow in inTeamDataTable.Rows) {
                    curTeamPlcmt++;
                    outLine = new StringBuilder( "" );
                    outLine.Append( "\n<tr>" );
                    outLine.Append( "<td>" + curTeamPlcmt + "</td>" );
                    try {
                        outLine.Append( "<td>" + (String)curRow["TeamCode"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)curRow["TeamScoreSlalom"] ).ToString( "###,##0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + (int)curRow["PlcmtSlalom"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)curRow["TeamScoreTrick"] ).ToString( "###,##0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + (int)curRow["PlcmtTrick"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)curRow["TeamScoreJump"] ).ToString( "###,##0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + (int)curRow["PlcmtJump"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append( "<td>" + ( (Decimal)curRow["TeamScoreTotal"] ).ToString( "###,##0.0" ) + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "</tr>" );
                    outBuffer.WriteLine( outLine.ToString() );
                }

                outLine = new StringBuilder( "" );
                outLine.Append( "</table>" );
                outLine.Append( "<a href=#PageTop>Return to Index</a>" );
                outLine.Append( "\n</div><br/>" );
                outBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Error writing team results to Scorebook: " + "\n\nError: " + ex.Message );
                outLine.Append( "<td>" + "Team Error: " + ex.Message + "</td>" );
                return false;
            }
        }

        private StreamWriter getExportFile(String inFileFilter, String inFileName) {
            String curMethodName = "getExportFile";
            String curFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            if (inFileFilter == null) {
                curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else {
                curFileFilter = inFileFilter;
            }

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.Filter = curFileFilter;
            curFileDialog.FilterIndex = 1;
            if (inFileName == null) {
                curFileDialog.FileName = "";
            } else {
                curFileDialog.FileName = inFileName;
            }

            try {
                if (curFileDialog.ShowDialog() == DialogResult.OK) {
                    curFileName = curFileDialog.FileName;
                    if (curFileName != null) {
                        if ( Path.GetExtension( curFileName ) == null ) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split( '|' );
                            if (curList.Length > 0) {
                                int curDelim = curList[1].IndexOf( "." );
                                if (curDelim > 0) {
                                    curDefaultExt = curList[1].Substring( curDelim - 1 );
                                }
                            }
                            curFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return outBuffer;
        }

        private String getSkierBackupJump(String inSanctionId, String inMemberId, int inRound, String inDiv) {
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
            if (curDataTable.Rows.Count > 1) {
                try {
                    return ( (Decimal)curDataTable.Rows[1]["ScoreMeters"] ).ToString( "##.0" );
                } catch {
                    return "";
                }
            } else {
                return "";
            }
        }

        private DataTable getSlalomLineData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'SlalomLines' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            return getData( curSqlStmt.ToString() );
        }

        private int getSkierCountDiv(String inSanctionId, String inEvent, String inDiv) {
            int curReturnValue = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Count(*) as ScoreCount " );
            curSqlStmt.Append( "FROM " + inEvent + "Score " );
            curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' " );
            curSqlStmt.Append( "And AgeGroup = '" + inDiv + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString());
            if (curDataTable.Rows.Count > 0) {
                curReturnValue = (int)curDataTable.Rows[0]["ScoreCount"];
            } else {
                curReturnValue = 0;
            }
            return curReturnValue;
        }

        private DataTable getSlalomSpeedMph() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MinValue, MaxValue " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'SlalomSpeeds' " );
            curSqlStmt.Append( "ORDER BY SortSeq" );
            return getData( curSqlStmt.ToString() );
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
                return (String)curDataTable.Rows[0]["CodeValue"];
            } else {
                return "46kph, 28mph";
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

        private bool isTeamAvailable() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TeamList " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 1) {
                return true;
            } else {
                return false;
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
