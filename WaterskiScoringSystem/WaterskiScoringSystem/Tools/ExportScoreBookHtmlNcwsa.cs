using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tools {
    class ExportScoreBookHtmlNcwsa {
        private Int16 myNumPerTeam;
        private int myTourRounds;
        private String myTourRules;
        private String mySanctionNum = null;
        private DataRow myTourRow;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;
        private DataTable myTeamDataTable;
        private DataTable mySummaryDataTable;
        private DataTable mySlalomAllDataTable;
        private DataTable myTrickAllDataTable;
        private DataTable myJumpAllDataTable;
        private DataTable mySummaryAllDataTable;
        private ProgressWindow myProgressInfo;
        private TourProperties myTourProperties;

        public ExportScoreBookHtmlNcwsa() {
            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            myTourProperties = TourProperties.Instance;

            if (mySanctionNum == null) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    Boolean curContinue = true;
                    myNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam);
                    if (myNumPerTeam == 0) myNumPerTeam = 4;
                    if ( myNumPerTeam != 4 ) {
                        MessageBox.Show("You have specified to use " + myNumPerTeam + "on your team summary window.  Is this correct?");

                        DialogResult msgResp = MessageBox.Show("You have specified to use " + myNumPerTeam + " on your team summary window."
                            + "\nIf you want to continue click OK."
                            + "\nOtherwise click CANCEL and use the Team Summary window to update the number of skiers per team."
                            , "Data Warning"
                            , MessageBoxButtons.OKCancel
                            , MessageBoxIcon.Warning
                            , MessageBoxDefaultButton.Button1);
                        if (msgResp == DialogResult.Cancel) {
                            curContinue = false;
                        }
                    }

                    //Retrieve selected tournament attributes

                    if (curContinue) {
                        DataTable curTourDataTable = getTourData( mySanctionNum );
                        if (curTourDataTable.Rows.Count > 0) {
                            myProgressInfo = new ProgressWindow();
                            myProgressInfo.setProgessMsg( "Processing Scorebook NCWSA HTML" );
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();
                            myProgressInfo.setProgressMax( 12 );

                            myTourRow = curTourDataTable.Rows[0];
                            myTourRules = (String)myTourRow["Rules"];
                            String curDataType = "best", curPlcmtMethod = "points", curPointsMethod = "plcmt", curPlcmtOrg = "div";
                            CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                            mySlalomDataTable = curCalcSummary.getSlalomSummary(myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team");
                            myProgressInfo.setProgressValue( 1 );
                            myProgressInfo.Refresh();
                            
                            myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team" );
                            myProgressInfo.setProgressValue( 2 );
                            myProgressInfo.Refresh();
                            
                            myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team" );
                            myProgressInfo.setProgressValue( 3 );
                            myProgressInfo.Refresh();
                            
                            mySummaryDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, curDataType, curPlcmtOrg );
                            myProgressInfo.setProgressValue( 4 );
                            myProgressInfo.Refresh();

                            myTeamDataTable = curCalcSummary.getSlalomSummaryTeam(mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                            myProgressInfo.setProgressValue( 5 );
                            myProgressInfo.Refresh();
                            
                            myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                            myProgressInfo.setProgressValue( 6 );
                            myProgressInfo.Refresh();
                            myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                            myProgressInfo.setProgressValue( 7 );
                            myProgressInfo.Refresh();

                            String curSortCmd = "Div ASC, TeamScoreTotal DESC";
                            myTeamDataTable.DefaultView.Sort = curSortCmd;
                            myTeamDataTable = myTeamDataTable.DefaultView.ToTable();
                            myProgressInfo.setProgressValue( 8 );
                            myProgressInfo.Refresh();

                            curPointsMethod = "nops";
                            String curPlcmtOverallOrg = "agegroup"; 
                            mySlalomAllDataTable = curCalcSummary.getSlalomSummary(myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                            myProgressInfo.setProgressValue( 9 );
                            myProgressInfo.Refresh();
                            
                            myTrickAllDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                            myProgressInfo.setProgressValue( 10 );
                            myProgressInfo.Refresh();
                            
                            myJumpAllDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                            myProgressInfo.setProgressValue( 11 );
                            myProgressInfo.Refresh();
                            
                            mySummaryAllDataTable = curCalcSummary.buildOverallSummary( myTourRow, mySlalomAllDataTable, myTrickAllDataTable, myJumpAllDataTable, curDataType, curPlcmtOverallOrg );
                            myProgressInfo.setProgressValue( 12 );
                            myProgressInfo.Refresh();
                            myProgressInfo.Close();
                        }
                    }
                }
            }
        }

        public Boolean exportScoreBookData() {
            String curMethodName = "exportScoreBookData";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curMsg = "";
            String curFileFilter = "HTML files (*.htm)|*.htm|All files (*.*)|*.*";

            try {
                curMsg = "Exporting Scorebook Html";
                Log.WriteFile( curMethodName + ":begin: " + curMsg );
                String curFilename = mySanctionNum.Trim() + "CS.HTM";
                StreamWriter outBuffer = getExportFile( curFileFilter, curFilename );

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

                if ( outBuffer == null ) {
                    curMsg = "Output file not available";
                } else {

                    myProgressInfo = new ProgressWindow();
                    myProgressInfo.setProgessMsg( "Exporting data to Scorebook NCWSA HTML file" );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();
                    myProgressInfo.setProgressMax( 5 );

                    //Build file header line and write to file
                    writeHeader(outBuffer);

                    writeTeamCombinedResults( outBuffer );
                    //writeTeamCombinedSkierDetail("Combined", outBuffer);

                    //Men A team scores
                    writeTeamData("CM", "MenA", "Men A", outBuffer); //Write team results
                    writeTeamSkierDetail("CM", "Men A", outBuffer); //Write team detail results
                    writeTeamSlalomResults( "CM", "MenA", "Men A", outBuffer ); //Write slalom results
                    writeTeamTrickResults( "CM", "MenA", "Men A", outBuffer ); //Write Trick results
                    writeTeamJumpResults( "CM", "MenA", "Men A", outBuffer ); //Write Jump results
                    writeSkierOverallDetail( "CM", "MenA", "Men A", outBuffer ); //Write overall results
                    myProgressInfo.setProgressValue( 1 );
                    myProgressInfo.Refresh();

                    //Women A team scores
                    writeTeamData( "CW", "WomenA", "Women A", outBuffer ); //Write team results
                    writeTeamSkierDetail( "CW", "Women A", outBuffer ); //Write team detail results
                    writeTeamSlalomResults( "CW", "WomenA", "Women A", outBuffer );
                    writeTeamTrickResults( "CW", "WomenA", "Women A", outBuffer );
                    writeTeamJumpResults( "CW", "WomenA", "Women A", outBuffer );
                    writeSkierOverallDetail( "CW", "WomenA", "Women A", outBuffer ); //Write overall results
                    myProgressInfo.setProgressValue( 2 );
                    myProgressInfo.Refresh();

                    //Men B team scores
                    writeTeamData("BM", "MenB", "Men B", outBuffer);
                    writeTeamSkierDetail("BM", "Men B", outBuffer);
                    writeTeamSlalomResults( "BM", "MenB", "Men B", outBuffer );
                    writeTeamTrickResults( "BM", "MenB", "Men B", outBuffer );
                    writeTeamJumpResults( "BM", "MenB", "Men B", outBuffer );
                    writeSkierOverallDetail( "BM", "MenB", "Men B", outBuffer );
                    myProgressInfo.setProgressValue( 3 );
                    myProgressInfo.Refresh();

                    //Build team score header for Women B 
                    writeTeamData("BW", "WomenB", "Women B", outBuffer);
                    writeTeamSkierDetail("BW", "Women B", outBuffer);
                    writeTeamSlalomResults( "BW", "WomenB", "Women B", outBuffer );
                    writeTeamTrickResults( "BW", "WomenB", "Women B", outBuffer );
                    writeTeamJumpResults( "BW", "WomenB", "Women B", outBuffer );
                    writeSkierOverallDetail( "BW", "WomenB", "Women B", outBuffer );
                    myProgressInfo.setProgressValue( 4 );
                    myProgressInfo.Refresh();

                    writeIndivSlalomResults(outBuffer);
                    writeIndivTrickResults(outBuffer);
                    writeIndivJumpResults(outBuffer);
                    writeIndivOverallResults(outBuffer);
                    curMsg = "Scorebook web report complete";
                    myProgressInfo.setProgressValue( 5 );
                    myProgressInfo.Refresh();
                    myProgressInfo.Close();

                    returnStatus = true;
                    outBuffer.Close();

                }
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + ":conplete: " + curMsg );
            } catch ( Exception ex ) {
                MessageBox.Show( "Error:" + curMethodName + " Could not write file from DataGridView\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
            }

            return returnStatus;
        }

        //Write file header
        private Boolean writeHeader(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            outLine.Append("\n<html><head>");
            outLine.Append("\n<title>");
            outLine.Append(myTourRow["SanctionId"].ToString().ToUpper().Trim() + myTourRow["Class"].ToString().ToUpper().Trim());
            outLine.Append(" " + myTourRow["Name"].ToString().ToUpper());
            outLine.Append("</title>");
            outLine.Append("\n<style>");
            outLine.Append("\nBody {BACKGROUND-COLOR:#FFFFFF; COLOR:#000000;");
            outLine.Append("\nFont-Family:Arial,Helvetica,sans-serif; FONT-SIZE:1.0em; Font-Style:normal;Font-Weight:normal;} ");
            outLine.Append("\n A:link {text-decoration: none; color: DarkBlue;}");
            outLine.Append("\n A:visited {text-decoration: none; color: DarkBlue;}");
            outLine.Append("\n A:active {text-decoration: none; color: DarkBlue;}");
            outLine.Append("\n A:hover {text-decoration: none; color: Yellow; Background: Maroon;}");

            outLine.Append("\n .ReportTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.3em; FONT-WEIGHT:bold;}");
            outLine.Append("\n .SectionTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.15em; FONT-WEIGHT:bold;}");
            outLine.Append("\n .SubTitle {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:1.1em; FONT-WEIGHT:normal;}");
            outLine.Append("\n .ExtraSkierNote {Width: 100%; Text-Align:Center; margin-left: auto; margin-right: auto; FONT-SIZE:.75em; FONT-WEIGHT:normal;}");
            outLine.Append("\n .MainContent {Text-Align:Center; margin-left: auto; margin-right: auto; }");
            outLine.Append("\n .LineSep {Width: 100%; FONT-SIZE:.05em; BACKGROUND-COLOR:LightGray;}");
            outLine.Append("\n .ExtraSkier {vertical-align: super; font-size: smaller; Color: Blue;}");
            outLine.Append("\n .DataGridView {Text-Align:Center; Border: #AAAAAA .15em solid; margin-left: auto; margin-right: auto; }");
            outLine.Append("\n th {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:2px 5px 2px 5px; Font-Size:.8em; Font-Weight:Bold;}");
            outLine.Append("\n td {Text-Align:Center; Border:#DDDDDD .1em solid; Margin:0pt; Padding:2px 5px 2px 5px; Font-Family:Verdana,Helvetica,sans-serif; Font-Size:.8em; Font-Weight:normal;}");
            outLine.Append("\n .DataLeft {Text-Align:Left; }");
            outLine.Append("\n .DataRight {Text-Align:Right; }");
            outLine.Append("\n</style>");
            outLine.Append("\n</head><body>");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            String curDateString = DateTime.Now.ToString("MMMM d, yyyy HH:MM");
            outLine.Append("\n<a name=PageTop ></a>");
            outLine.Append("\n<div Class=\"ReportTitle\">");
            outLine.Append(myTourRow["SanctionId"].ToString().ToUpper().Trim());
            outLine.Append(myTourRow["Class"].ToString().ToUpper().Trim());
            outLine.Append(" " + myTourRow["Name"].ToString().ToUpper());
            outLine.Append("<br/>Master Scorebook Results as of " + curDateString + "</div>");
            outLine.Append("</div>");
            outLine.Append("\n<div Class=\"SubTitle\">");
            outLine.Append("<br/>Scored with ");
            outLine.Append(Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion);
            outLine.Append("</div>");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("<br/>Index of Scoring Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append( "\n<tr><th>Division</th><td>Team</td><td>Slalom</td><td>Tricks</td><td>Jump</td><td>Overall</td></tr>" );
            outLine.Append( "\n<tr><th>Combined</th><td><a href=#TeamCombined>Team</a></td><td></td><td></td><td></td><td></td></tr>" );
            outLine.Append( "\n<tr><th>Men A</th><td><a href=#MenATeam>Team</a></td><td><a href=#MenASlalom>Slalom</a></td><td><a href=#MenATrick>Tricks</a></td><td><a href=#MenAJump>Jump</a></td><td><a href=#MenAOverall>Overall</a></td></tr>" );
            outLine.Append( "\n<tr><th>Women A</th><td><a href=#WomenATeam>Team</a></td><td><a href=#WomenASlalom>Slalom</a></td><td><a href=#WomenATrick>Tricks</a></td><td><a href=#WomenAJump>Jump</a></td><td><a href=#WomenAOverall>Overall</a></td></tr>" );
            outLine.Append( "\n<tr><th>Men B</th><td><a href=#MenBTeam>Team</a></td><td><a href=#MenBSlalom>Slalom</a></td><td><a href=#MenBTrick>Tricks</a></td><td><a href=#MenBJump>Jump</a></td><td><a href=#MenBOverall>Overall</a></td></tr>" );
            outLine.Append( "\n<tr><th>Women B</th><td><a href=#WomenBTeam>Team</a></td><td><a href=#WomenBSlalom>Slalom</a></td><td><a href=#WomenBTrick>Tricks</a></td><td><a href=#WomenBJump>Jump</a></td><td><a href=#WomenBOverall>Overall</a></td></tr>" );
            outLine.Append( "\n<tr><th>Non Team Skiers</th><td></td><td><a href=#OfficialSlalom>Slalom</a></td><td><a href=#OfficialTrick>Tricks</a></td><td><a href=#OfficialJump>Jump</a></td><td><a href=#OfficialOverall>Overall</a></td></tr>" );
            outBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder("");
            outLine.Append("\n</Table></div>");
            outBuffer.WriteLine(outLine.ToString());

            return true;
        }

        //Write file footer
        private Boolean writeFooter(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            outLine.Append("</table>");
            outLine.Append("<br><div Class=\"footer\">");
            outLine.Append("Printed by ");
            outLine.Append(Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion);
            outLine.Append("\n</div>");
            outLine.Append("\n</div>\n</body></html>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        //Write team data record
        private Boolean writeTeamData(String curDiv, String curLinkName, String curSectionTitle, StreamWriter outBuffer) {
            int curTeamPlcmt = 0, curRowIdx = 0;
            Decimal curScore = 0, prevScore = 0, nextScore = 0;
            String curTeamPlcmtShow = "";
            StringBuilder outLine = new StringBuilder("");

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=" + curLinkName + "Team></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outBuffer.WriteLine(outLine.ToString());
            
            outLine = new StringBuilder("\n<tr>");
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
            outBuffer.WriteLine(outLine.ToString());

            foreach (DataRow curRow in myTeamDataTable.Rows) {
                if (((String)curRow["Div"]).ToUpper().Equals(curDiv)) {
                    curTeamPlcmt++;
                    curScore = (Decimal)curRow["TeamScoreTotal"];
                    if (curTeamPlcmt > 1) {
                        prevScore = (Decimal)myTeamDataTable.Rows[curRowIdx - 1]["TeamScoreTotal"];
                        if (curScore == prevScore) {
                            curTeamPlcmtShow = ( curTeamPlcmt - 1 ).ToString( "##0" ) + "T";
                        } else {
                            if (curRowIdx < ( myTeamDataTable.Rows.Count - 1 )) {
                                nextScore = (Decimal)myTeamDataTable.Rows[curRowIdx + 1]["TeamScoreTotal"];
                                if (curScore == nextScore) {
                                    curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
                                } else {
                                    curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                                }
                            } else {
                                curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                            }
                        }
                    } else {
                        if (curRowIdx < ( myTeamDataTable.Rows.Count - 1 )) {
                            nextScore = (Decimal)myTeamDataTable.Rows[curRowIdx + 1]["TeamScoreTotal"];
                            if (curScore == nextScore) {
                                curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
                            } else {
                                curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                            }
                        } else {
                            curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                        }
                    }

                    outLine = new StringBuilder("");
                    outLine.Append("\n<tr>");
                    outLine.Append( "<td>" + curTeamPlcmtShow + "</td>" );
                    try {
                        outLine.Append("<td>" + (String)curRow["TeamCode"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreSlalom"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        if ( ( (String) curRow["PlcmtSlalom"] ).Length == 0 ) {
                            outLine.Append("<td>&nbsp;</td>");
                        } else {
                            if ( ( (String) curRow["PlcmtSlalom"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) curRow["PlcmtSlalom"] + "</td>");
                            }
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreTrick"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        if ( ( (String) curRow["PlcmtTrick"] ).Length == 0 ) {
                            outLine.Append("<td>&nbsp;</td>");
                        } else {
                            if ( ( (String) curRow["PlcmtTrick"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) curRow["PlcmtTrick"] + "</td>");
                            }
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreJump"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        if ( ( (String) curRow["PlcmtJump"] ).Length == 0 ) {
                            outLine.Append("<td>&nbsp;</td>");
                        } else {
                            if ( ( (String) curRow["PlcmtJump"] ).Trim().Equals("999") ) {
                                outLine.Append("<td>**</td>");
                            } else {
                                outLine.Append("<td>" + (String) curRow["PlcmtJump"] + "</td>");
                            }
                        }
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreTotal"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    outBuffer.WriteLine(outLine.ToString());

                    outLine = new StringBuilder("</tr>");
                    outBuffer.WriteLine(outLine.ToString());
                }
                curRowIdx++;
            }

            outLine = new StringBuilder("");
            outLine.Append("</table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div><br/>");
            outBuffer.WriteLine(outLine.ToString());

            return true;
        }

        private Boolean writeTeamSkierDetail(String curDiv, String curSectionTitle, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");

            outLine = new StringBuilder("");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Skier Details");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"ExtraSkierNote\"><span class=\"ExtraSkier\" style=\"FONT-SIZE:1.25em;\">**</span> team skiers not included in team total</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("\n<tr>");
            outLine.Append("<th colspan=\"9\">Slalom</th>");
            outLine.Append("<th colspan=\"4\">Trick</th>");
            outLine.Append("<th colspan=\"5\">Jump</th>");
            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Buoys</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>MPH</th>");
            outLine.Append("<th>KPH</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Meters</th>");
            outLine.Append("<th>Feet</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");
            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curSlalomTeamSkierList;
            DataRow[] curTrickTeamSkierList;
            DataRow[] curJumpTeamSkierList;
            String curFilterCmd = "";
            String curSortCmd = "AgeGroup, TeamSlalom, PlcmtSlalom ASC, SkierName, RoundSlalom";
            mySlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomDataTable.DefaultView.ToTable();

            curSortCmd = "AgeGroup, TeamTrick, PlcmtTrick ASC, SkierName, RoundTrick";
            myTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickDataTable.DefaultView.ToTable();

            curSortCmd = "AgeGroup, TeamJump, PlcmtJump ASC, SkierName, RoundJump";
            myJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpDataTable.DefaultView.ToTable();

            int curSkierIdx = 0, curSkierIdxMax = 0;
            String curTeamDiv = "", curTeamCode = "", prevTeamCode = "";
            foreach (DataRow curRow in myTeamDataTable.Rows) {
                curTeamDiv = (String)curRow["Div"];
                curTeamCode = (String)curRow["TeamCode"];
                if (curTeamDiv.ToUpper().Equals( curDiv )) {
                    if (!( curTeamCode.Equals( prevTeamCode ) ) || prevTeamCode == "") {
                        outLine = new StringBuilder( "" );
                        outLine.Append( "\n<tr><td colspan=\"99\">" );
                        outLine.Append( "\n<br/>" + curTeamCode + " " + (String)curRow["Name"] );
                        outLine.Append( "\n</td></tr>" );
                        outBuffer.WriteLine( outLine.ToString() );
                    }

                    curFilterCmd = "TeamSlalom='" + (String)curRow["TeamCode"] + "' AND AgeGroup = '" + curTeamDiv + "'";
                    curSlalomTeamSkierList = curSlalomDataTable.Select( curFilterCmd );
                    curFilterCmd = "TeamTrick='" + (String)curRow["TeamCode"] + "' AND AgeGroup = '" + curTeamDiv + "'";
                    curTrickTeamSkierList = curTrickDataTable.Select( curFilterCmd );
                    curFilterCmd = "TeamJump='" + (String)curRow["TeamCode"] + "' AND AgeGroup = '" + curTeamDiv + "'";
                    curJumpTeamSkierList = curJumpDataTable.Select( curFilterCmd );
                    curSkierIdxMax = 0;
                    if (curSlalomTeamSkierList.Length > curTrickTeamSkierList.Length) {
                        if (curSlalomTeamSkierList.Length > curJumpTeamSkierList.Length) {
                            curSkierIdxMax = curSlalomTeamSkierList.Length;
                        } else {
                            curSkierIdxMax = curJumpTeamSkierList.Length;
                        }
                    } else {
                        if (curTrickTeamSkierList.Length > curJumpTeamSkierList.Length) {
                            curSkierIdxMax = curTrickTeamSkierList.Length;
                        } else {
                            curSkierIdxMax = curJumpTeamSkierList.Length;
                        }
                    }

                    for (curSkierIdx = 0; curSkierIdx < curSkierIdxMax; curSkierIdx++) {
                        if (myNumPerTeam > curSkierIdx) {
                            outLine = new StringBuilder( "" );
                            outLine.Append( "\n<tr>" );
                            if (curSlalomTeamSkierList.Length > curSkierIdx) {
                                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curSlalomTeamSkierList[curSkierIdx]["SkierName"] + "</td>" );
                                try {
                                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["FinalPassScore"] ).ToString( "0.00" ) + "</td>" );
                                } catch {
                                    outLine.Append( "\n<td>&nbsp;</td>" );
                                }
                                try {
                                    outLine.Append( "\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLenOff"] + "</td>" );
                                } catch {
                                    outLine.Append( "\n<td>&nbsp;</td>" );
                                }
                                try {
                                    outLine.Append( "\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLen"] + "</td>" );
                                } catch {
                                    outLine.Append( "\n<td>&nbsp;</td>" );
                                }
                                try {
                                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedMph"] ).ToString() + "</td>" );
                                } catch {
                                    outLine.Append( "\n<td>&nbsp;</td>" );
                                }
                                try {
                                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedKph"] ).ToString() + "</td>" );
                                } catch {
                                    outLine.Append( "\n<td>&nbsp;</td>" );
                                }
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" ) + "</td>" );
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0.00" ) + "</td>" );
                                outLine.Append( "\n<td>" + (String)curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"] + "</td>" );

                            } else {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            if (curTrickTeamSkierList.Length > curSkierIdx) {
                                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curTrickTeamSkierList[curSkierIdx]["SkierName"] + "</td>" );
                                if (curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType( "System.Int32" )) {
                                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Int32)curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" ) + "</td>" );
                                } else {
                                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Int16)curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" ) + "</td>" );
                                }
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0.00" ) + "</td>" );
                                outLine.Append( "\n<td>" + (String)curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"] + "</td>" );
                            } else {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            if (curJumpTeamSkierList.Length > curSkierIdx) {
                                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curJumpTeamSkierList[curSkierIdx]["SkierName"] + "</td>" );
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString( "##0.0" ) + "</td>" );
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString( "##0" ) + "</td>" );
                                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0.00" ) + "</td>" );
                                outLine.Append( "\n<td>" + (String)curJumpTeamSkierList[curSkierIdx]["PlcmtJump"] + "</td>" );
                            } else {
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                                outLine.Append( "\n<td>&nbsp;</td>" );
                            }
                            outLine.Append( "\n</tr>" );
                            outBuffer.WriteLine( outLine.ToString() );
                        } else {
                            if ( myNumPerTeam == curSkierIdx ) {
                                outLine = new StringBuilder("");
                                outLine.Append("\n<tr><td class=\"LineSep\" colspan=\"99\">&nbsp;</td></tr>");
                                outBuffer.WriteLine(outLine.ToString());
                            }

                            outLine = new StringBuilder("");
                            outLine.Append("\n<tr>");
                            if ( curSlalomTeamSkierList.Length > curSkierIdx ) {
                                outLine.Append("\n<td class=\"DataLeft\">" + (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"] + "<span class=\"ExtraSkier\">**</span></td>");
                                try {
                                    outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["FinalPassScore"] ).ToString("0.00") + "</td>");
                                } catch {
                                    outLine.Append("\n<td>&nbsp;</td>");
                                }
                                try {
                                    outLine.Append("\n<td class=\"DataRight\">" + (String) curSlalomTeamSkierList[curSkierIdx]["FinalLenOff"] + "</td>");
                                } catch {
                                    outLine.Append("\n<td>&nbsp;</td>");
                                }
                                try {
                                    outLine.Append("\n<td class=\"DataRight\">" + (String) curSlalomTeamSkierList[curSkierIdx]["FinalLen"] + "</td>");
                                } catch {
                                    outLine.Append("\n<td>&nbsp;</td>");
                                }
                                try {
                                    outLine.Append("\n<td class=\"DataRight\">" + ( (Byte) curSlalomTeamSkierList[curSkierIdx]["FinalSpeedMph"] ).ToString() + "</td>");
                                } catch {
                                    outLine.Append("\n<td>&nbsp;</td>");
                                }
                                try {
                                    outLine.Append("\n<td class=\"DataRight\">" + ( (Byte) curSlalomTeamSkierList[curSkierIdx]["FinalSpeedKph"] ).ToString() + "</td>");
                                } catch {
                                    outLine.Append("\n<td>&nbsp;</td>");
                                }
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString("##0.00") + "</td>");
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString("###0.00") + "</td>");
                                outLine.Append("\n<td>" + (String) curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"] + "</td>");

                            } else {
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            if ( curTrickTeamSkierList.Length > curSkierIdx ) {
                                outLine.Append("\n<td class=\"DataLeft\">" + (String) curTrickTeamSkierList[curSkierIdx]["SkierName"] + "<span class=\"ExtraSkier\">**</span></td>");
                                if ( curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType("System.Int32") ) {
                                    outLine.Append("\n<td class=\"DataRight\">" + ( (Int32) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString("##,##0") + "</td>");
                                } else {
                                    outLine.Append("\n<td class=\"DataRight\">" + ( (Int16) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString("##,##0") + "</td>");
                                }
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString("###0.00") + "</td>");
                                outLine.Append("\n<td>" + (String) curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"] + "</td>");
                            } else {
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            if ( curJumpTeamSkierList.Length > curSkierIdx ) {
                                outLine.Append("\n<td class=\"DataLeft\">" + (String) curJumpTeamSkierList[curSkierIdx]["SkierName"] + "<span class=\"ExtraSkier\">**</span></td>");
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString("##0.0") + "</td>");
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString("##0") + "</td>");
                                outLine.Append("\n<td class=\"DataRight\">" + ( (Decimal) curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString("###0.00") + "</td>");
                                outLine.Append("\n<td>" + (String) curJumpTeamSkierList[curSkierIdx]["PlcmtJump"] + "</td>");
                            } else {
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            outLine.Append("\n</tr>");
                            outBuffer.WriteLine(outLine.ToString());
                        }
                    }

                    prevTeamCode = curTeamCode;
                } else {
                }
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeSkierOverallDetail(String curDiv, String curLinkName, String curSectionTitle, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=" + curLinkName + "Overall></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Skier Overall");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("\n<tr>");
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Skier</th>" );
            outLine.Append( "<th>Team</th>" );
            outLine.Append( "<th>Slalom</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Trick</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Jump</th>" );
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Overall</th>" );
            outLine.Append( "<th>Qualified</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine(outLine.ToString());

            String curFilterCmd = "AgeGroup = '" + curDiv + "' AND QualifyOverall = 'Yes'";
            String curPlcmtShow = "", prevPlcmtShow = "";
            int curPlcmt = 0;
            Decimal curScore = 0, prevScore = 0, nextScore = 0;
            DataRow[] curSkierList;
            DataRow curRow;
            curSkierList = mySummaryDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curSkierList.Length; curSkierIdx++) {
                curRow = curSkierList[curSkierIdx];
                outLine = new StringBuilder("");

                outLine.Append( "\n<tr>" );
                curPlcmt++;
                curScore = (Decimal)curRow["ScoreOverall"];
                if (curSkierIdx == ( curSkierList.Length - 1 )) {
                    if (curScore < prevScore) {
                        curPlcmtShow = curPlcmt.ToString( "##0" );
                        outLine.Append( "\n<td class=\"DataRight\">" + curPlcmtShow + "</td>" );
                    } else {
                        outLine.Append( "\n<td class=\"DataRight\">" + ( curPlcmt - 1 ).ToString( "##0" ) + "T</td>" );
                    }
                } else {
                    nextScore = (Decimal)curSkierList[curSkierIdx + 1]["ScoreOverall"];
                    if (curScore > nextScore) {
                        if (curScore < prevScore || curSkierIdx == 0) {
                            curPlcmtShow = curPlcmt.ToString( "##0" );
                            outLine.Append( "\n<td class=\"DataRight\">" + curPlcmtShow + "</td>" );
                        } else {
                            outLine.Append( "\n<td class=\"DataRight\">" + prevPlcmtShow + "</td>" );
                        }
                    } else {
                        if (curScore < prevScore || curSkierIdx == 0) {
                            curPlcmtShow = curPlcmt.ToString( "##0" ) + "T";
                            outLine.Append( "\n<td class=\"DataRight\">" + curPlcmtShow + "</td>" );
                        } else {
                            outLine.Append( "\n<td class=\"DataRight\">" + prevPlcmtShow + "</td>" );
                        }
                    }
                }
                prevScore = curScore;
                prevPlcmtShow = curPlcmtShow;
                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curRow["SkierName"] + "</td>" );
                outLine.Append( "\n<td class=\"DataCenter\">" + (String)curRow["TeamSlalom"] + "</td>" );
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsSlalom"] ).ToString( "###0" ) + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtSlalom"] + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsTrick"] ).ToString( "###0" ) + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtTrick"] + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["PointsJump"] ).ToString( "###0" ) + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + (String)curRow["PlcmtJump"] + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curRow["ScoreOverall"] ).ToString( "###0" ) + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append( "\n<td class=\"DataCenter\">" + (String)curRow["QualifyOverall"] + "</td>" );
                } catch {
                    outLine.Append( "\n<td>&nbsp;</td>" );
                }
                outLine.Append( "\n</tr>" );
                outBuffer.WriteLine( outLine.ToString() );
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeTeamSlalomResults(String curDiv, String curLinkName, String curSectionTitle, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=" + curLinkName + "Slalom></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Slalom Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Team</th>" );
            outLine.Append( "<th>Name</th>" );
            outLine.Append( "<th>Score</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine(outLine.ToString());

            curSortCmd = "Div, TeamScoreSlalom DESC, Name";
            myTeamDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTeamDataTable = myTeamDataTable.DefaultView.ToTable();

            foreach (DataRow curRow in curTeamDataTable.Rows) {
                if (((String)curRow["Div"]).ToUpper().Equals(curDiv)) {
                    outLine = new StringBuilder("");
                    outLine.Append("\n<tr>");
                    try {
                        outLine.Append( "<td>" + (String)curRow["PlcmtSlalom"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + (String)curRow["TeamCode"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreSlalom"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    outBuffer.WriteLine(outLine.ToString());

                    outLine = new StringBuilder("</tr>");
                    outBuffer.WriteLine(outLine.ToString());
                }
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("<br/><br/>");
            outBuffer.WriteLine(outLine.ToString());
            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Team</th>");
            outLine.Append("<th>Buoys</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>MPH</th>");
            outLine.Append("<th>KPH</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");
            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curSlalomTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, PlcmtSlalom ASC, SkierName";
            mySlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup = '" + curDiv + "'";
            curSlalomTeamSkierList = curSlalomDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curSlalomTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamSkierList[curSkierIdx]["TeamSlalom"] + "</td>");
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSlalomTeamSkierList[curSkierIdx]["FinalPassScore"]).ToString("0.00") + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLenOff"] + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLen"] + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedMph"]).ToString() + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedKph"]).ToString() + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" ) + "</td>" );
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0.00" ) + "</td>" );
                outLine.Append( "\n<td>" + (String)curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"] + "</td>" );
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeTeamTrickResults(String curDiv, String curLinkName, String curSectionTitle, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=" + curLinkName + "Trick></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Trick Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Team</th>" );
            outLine.Append( "<th>Name</th>" );
            outLine.Append( "<th>Score</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine(outLine.ToString());

            curSortCmd = "Div, TeamScoreTrick DESC, Name";
            myTeamDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTeamDataTable = myTeamDataTable.DefaultView.ToTable();

            foreach (DataRow curRow in curTeamDataTable.Rows) {
                if (((String)curRow["Div"]).ToUpper().Equals(curDiv)) {
                    outLine = new StringBuilder("");
                    outLine.Append("\n<tr>");
                    try {
                        outLine.Append( "<td>" + (String)curRow["PlcmtTrick"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + (String)curRow["TeamCode"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreTrick"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    outBuffer.WriteLine(outLine.ToString());

                    outLine = new StringBuilder("</tr>");
                    outBuffer.WriteLine(outLine.ToString());
                }
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("<br/><br/>");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Team</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curTrickTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, PlcmtTrick ASC, SkierName";
            myTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup = '" + curDiv + "'";
            curTrickTeamSkierList = curTrickDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curTrickTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curTrickTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curTrickTeamSkierList[curSkierIdx]["TeamTrick"] + "</td>" );
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Int16)curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##0.00" ) + "</td>" );
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0.00" ) + "</td>" );
                outLine.Append( "\n<td>" + (String)curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"] + "</td>" );
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeTeamJumpResults(String curDiv, String curLinkName, String curSectionTitle, StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=" + curLinkName + "Jump></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Jump Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append( "<th>Plcmt</th>" );
            outLine.Append( "<th>Team</th>" );
            outLine.Append( "<th>Name</th>" );
            outLine.Append( "<th>Score</th>" );
            outLine.Append( "</tr>" );
            outBuffer.WriteLine(outLine.ToString());

            curSortCmd = "Div, TeamScoreJump DESC, Name";
            myTeamDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTeamDataTable = myTeamDataTable.DefaultView.ToTable();

            foreach (DataRow curRow in curTeamDataTable.Rows) {
                if (((String)curRow["Div"]).ToUpper().Equals(curDiv)) {
                    outLine = new StringBuilder("");
                    outLine.Append("\n<tr>");
                    try {
                        outLine.Append( "<td>" + (String)curRow["PlcmtJump"] + "</td>" );
                    } catch {
                        outLine.Append( "<td>&nbsp;</td>" );
                    }
                    try {
                        outLine.Append("<td>" + (String)curRow["TeamCode"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    try {
                        outLine.Append("<td>" + ((Decimal)curRow["TeamScoreJump"]).ToString("###,##0.0") + "</td>");
                    } catch {
                        outLine.Append("<td>&nbsp;</td>");
                    }
                    outBuffer.WriteLine(outLine.ToString());

                    outLine = new StringBuilder("</tr>");
                    outBuffer.WriteLine(outLine.ToString());
                }
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("<br/><br/>");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Team</th>");
            outLine.Append("<th>ScoreFeet</th>");
            outLine.Append("<th>ScoreMeters</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curJumpTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, PlcmtJump ASC, SkierName";
            myJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup = '" + curDiv + "'";
            curJumpTeamSkierList = curJumpDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curJumpTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curJumpTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append( "\n<td class=\"DataLeft\">" + (String)curJumpTeamSkierList[curSkierIdx]["TeamJump"] + "</td>" );
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreFeet"]).ToString("##0.00") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreMeters"]).ToString("##0.00") + "</td>");
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0.00" ) + "</td>" );
                outLine.Append( "\n<td>" + (String)curJumpTeamSkierList[curSkierIdx]["PlcmtJump"] + "</td>" );
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeTeamCombinedResults(StreamWriter outBuffer) {
            int curTeamPlcmt = 0, curRowIdx = 0;
            Decimal curScore = 0, prevScore = 0, nextScore = 0;
            String curTeamPlcmtShow = "";
            StringBuilder outLine = new StringBuilder("");

            CalcScoreSummary curCalcSummary = new CalcScoreSummary();
            DataTable curTeamDataTable = curCalcSummary.CalcTeamEventCombinedNcwsaSummary(myTeamDataTable, mySanctionNum);

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=TeamCombined></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("Combined Team Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
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
            outBuffer.WriteLine(outLine.ToString());

            foreach (DataRow curRow in curTeamDataTable.Rows) {
                curTeamPlcmt++;
                curScore = (Decimal)curRow["TeamScoreTotal"];
                if (curTeamPlcmt > 1) {
                    prevScore = (Decimal)curTeamDataTable.Rows[curRowIdx - 1]["TeamScoreTotal"];
                    if (curScore == prevScore) {
                        curTeamPlcmtShow = ( curTeamPlcmt - 1 ).ToString( "##0" ) + "T";
                    } else {
                        if (curRowIdx < ( curTeamDataTable.Rows.Count - 1 )) {
                            nextScore = (Decimal)curTeamDataTable.Rows[curRowIdx + 1]["TeamScoreTotal"];
                            if (curScore == nextScore) {
                                curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
                            } else {
                                curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                            }
                        } else {
                            curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                        }
                    }
                } else {
                    if (curRowIdx < ( curTeamDataTable.Rows.Count - 1 )) {
                        nextScore = (Decimal)curTeamDataTable.Rows[curRowIdx + 1]["TeamScoreTotal"];
                        if (curScore == nextScore) {
                            curTeamPlcmtShow = ( curTeamPlcmt ).ToString( "##0" ) + "T";
                        } else {
                            curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                        }
                    } else {
                        curTeamPlcmtShow = curTeamPlcmt.ToString( "##0 " );
                    }
                }

                outLine = new StringBuilder( "" );
                outLine.Append("\n<tr>");
                outLine.Append( "<td>" + curTeamPlcmtShow + "</td>" );
                try {
                    outLine.Append("<td>" + (String)curRow["TeamCode"] + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("<td class=\"DataLeft\">" + (String)curRow["Name"] + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("<td>" + ((Decimal)curRow["TeamScoreSlalom"]).ToString("###,##0.0") + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                try {
                    outLine.Append( "<td>" + (String)curRow["PlcmtSlalom"] + "</td>" );
                } catch {
                    outLine.Append( "<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append("<td>" + ((Decimal)curRow["TeamScoreTrick"]).ToString("###,##0.0") + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                try {
                    outLine.Append( "<td>" + (String)curRow["PlcmtTrick"] + "</td>" );
                } catch {
                    outLine.Append( "<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append("<td>" + ((Decimal)curRow["TeamScoreJump"]).ToString("###,##0.0") + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                try {
                    outLine.Append( "<td>" + (String)curRow["PlcmtJump"] + "</td>" );
                } catch {
                    outLine.Append( "<td>&nbsp;</td>" );
                }
                try {
                    outLine.Append("<td>" + ((Decimal)curRow["TeamScoreTotal"]).ToString("###,##0.0") + "</td>");
                } catch {
                    outLine.Append("<td>&nbsp;</td>");
                }
                outBuffer.WriteLine(outLine.ToString());

                outLine = new StringBuilder("</tr>");
                outBuffer.WriteLine(outLine.ToString());
                curRowIdx++;
            }

            outLine = new StringBuilder("");
            outLine.Append("</table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div><br/>");
            outBuffer.WriteLine(outLine.ToString());

            return true;
        }

        private Boolean writeTeamCombinedSkierDetail(String curSectionTitle, StreamWriter outBuffer) {
            /*
            */
            StringBuilder outLine = new StringBuilder("");

            outLine = new StringBuilder("");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append(curSectionTitle + " Team Skier Details");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("\n<tr>");
            outLine.Append("<th>Slalom Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Buoys</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>MPH</th>");
            outLine.Append("<th>KPH</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("<th>Trick Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");

            outLine.Append("<th>Jump Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Meters</th>");
            outLine.Append("<th>Feet</th>");
            outLine.Append("<th>Points</th>");
            outLine.Append("<th>Plcmt</th>");
            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            String curFilterCmd = "", curDiv = "";
            DataRow[] curSkierList;
            DataTable curSlalomTeamDataTable = null;
            DataTable curTrickTeamDataTable = null;
            DataTable curJumpTeamDataTable = null;
            String curSortCmd = "TeamCode, NopsScore DESC, SkierName";

            mySlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomDataTable.DefaultView.ToTable();
            myTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickDataTable.DefaultView.ToTable();
            myJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpDataTable.DefaultView.ToTable();

            int curSkierIdx = 0, curSkierIdxMax = 0;
            String curTeamCode = "", prevTeamCode = "";
            foreach (DataRow curRow in myTeamDataTable.Rows) {
                curTeamCode = (String)curRow["TeamCode"];
                if (!(curTeamCode.Equals(prevTeamCode))) {
                    outLine = new StringBuilder("");
                    outLine.Append("\n<tr><td colspan=\"99\">");
                    outLine.Append("\n<br/>" + curTeamCode + " " + (String)curRow["Name"]);
                    outLine.Append("\n</td></tr>");
                    outBuffer.WriteLine(outLine.ToString());
                }

                curFilterCmd = "TeamCode='" + (String)curRow["TeamCode"] + "' AND (AgeGroup = 'CM' OR AgeGroup = 'CW')";
                curSkierList = curSlalomDataTable.DefaultView.ToTable().Select(curFilterCmd);
                if (curSkierList.Length > 0) {
                    curSlalomTeamDataTable = curSkierList.CopyToDataTable();
                    curSlalomTeamDataTable.DefaultView.Sort = curSortCmd;
                    curSlalomTeamDataTable = curSlalomTeamDataTable.DefaultView.ToTable();
                } else {
                    curSlalomTeamDataTable.Clear();
                }

                curSkierList = curTrickDataTable.DefaultView.ToTable().Select(curFilterCmd);
                if (curSkierList.Length > 0) {
                    curTrickTeamDataTable = curSkierList.CopyToDataTable();
                    curTrickTeamDataTable.DefaultView.Sort = curSortCmd;
                    curTrickTeamDataTable = curTrickTeamDataTable.DefaultView.ToTable();
                } else {
                    curTrickTeamDataTable.Clear();
                }

                curSkierList = curJumpDataTable.DefaultView.ToTable().Select(curFilterCmd);
                if (curSkierList.Length > 0) {
                    curJumpTeamDataTable = curSkierList.CopyToDataTable();
                    curJumpTeamDataTable.DefaultView.Sort = curSortCmd;
                    curJumpTeamDataTable = curJumpTeamDataTable.DefaultView.ToTable();
                } else {
                    curJumpTeamDataTable.Clear();
                }

                curSkierIdxMax = 0;
                if (curSlalomTeamDataTable.Rows.Count > curTrickTeamDataTable.Rows.Count) {
                    if (curSlalomTeamDataTable.Rows.Count > curJumpTeamDataTable.Rows.Count) {
                        curSkierIdxMax = curSlalomTeamDataTable.Rows.Count - 1;
                    } else {
                        curSkierIdxMax = curJumpTeamDataTable.Rows.Count - 1;
                    }
                } else {
                    if (curTrickTeamDataTable.Rows.Count > curJumpTeamDataTable.Rows.Count) {
                        curSkierIdxMax = curTrickTeamDataTable.Rows.Count - 1;
                    } else {
                        curSkierIdxMax = curJumpTeamDataTable.Rows.Count - 1;
                    }
                }

                for (curSkierIdx = 0; curSkierIdx < curSkierIdxMax; curSkierIdx++) {
                    if (myNumPerTeam > curSkierIdx) {
                        outLine = new StringBuilder("");
                        outLine.Append("\n<tr>");
                        if (curSkierIdx < curSlalomTeamDataTable.Rows.Count) {
                            outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamDataTable.Rows[curSkierIdx]["SkierName"] + "</td>");
                            outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamDataTable.Rows[curSkierIdx]["AgeGroup"] + "</td>");
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSlalomTeamDataTable.Rows[curSkierIdx]["FinalPassScore"]).ToString("0.00") + "</td>");
                            } catch {
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamDataTable.Rows[curSkierIdx]["FinalLenOff"] + "</td>");
                            } catch {
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamDataTable.Rows[curSkierIdx]["FinalLen"] + "</td>");
                            } catch {
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamDataTable.Rows[curSkierIdx]["FinalSpeedMph"]).ToString() + "</td>");
                            } catch {
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamDataTable.Rows[curSkierIdx]["FinalSpeedKph"]).ToString() + "</td>");
                            } catch {
                                outLine.Append("\n<td>&nbsp;</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSlalomTeamDataTable.Rows[curSkierIdx]["Score"]).ToString("##0.00") + "</td>");
                            } catch {
                                outLine.Append("\n<td>XXXX</td>");
                            }
                            try {
                                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSlalomTeamDataTable.Rows[curSkierIdx]["NopsScore"]).ToString("###0.00") + "</td>");
                            } catch {
                                outLine.Append("\n<td>YYYY</td>");
                            }
                            try {
                                outLine.Append("\n<td>" + (String)curSlalomTeamDataTable.Rows[curSkierIdx]["PlcmtGroup"] + "</td>");
                            } catch {
                                outLine.Append("\n<td>ZZZZ</td>");
                            }

                        } else {
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                        }
                        if (curSkierIdx < curTrickTeamDataTable.Rows.Count) {
                            try {
                                outLine.Append("\n<td class=\"DataLeft\">" + (String)curTrickTeamDataTable.Rows[curSkierIdx]["SkierName"] + "</td>");
                                outLine.Append("\n<td class=\"DataLeft\">" + (String)curTrickTeamDataTable.Rows[curSkierIdx]["AgeGroup"] + "</td>");
                                if (curTrickTeamDataTable.Rows[curSkierIdx]["Score"].GetType() == System.Type.GetType("System.Int32")) {
                                    outLine.Append("\n<td class=\"DataRight\">" + ((Int32)curTrickTeamDataTable.Rows[curSkierIdx]["Score"]).ToString("##,##0") + "</td>");
                                } else {
                                    outLine.Append("\n<td class=\"DataRight\">" + ((Int16)curTrickTeamDataTable.Rows[curSkierIdx]["Score"]).ToString("##,##0") + "</td>");
                                }
                                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curTrickTeamDataTable.Rows[curSkierIdx]["NopsScore"]).ToString("###0.00") + "</td>");
                                outLine.Append("\n<td>" + (String)curTrickTeamDataTable.Rows[curSkierIdx]["PlcmtGroup"] + "</td>");
                            } catch {
                                outLine.Append("\n<td>Error in trick data</td>");
                            }
                        } else {
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                        }
                        if (curSkierIdx < curJumpTeamDataTable.Rows.Count) {
                            try {
                            outLine.Append("\n<td class=\"DataLeft\">" + (String)curJumpTeamDataTable.Rows[curSkierIdx]["SkierName"] + "</td>");
                            outLine.Append("\n<td class=\"DataLeft\">" + (String)curJumpTeamDataTable.Rows[curSkierIdx]["AgeGroup"] + "</td>");
                            outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamDataTable.Rows[curSkierIdx]["ScoreMeters"]).ToString("##0.0") + "</td>");
                            outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamDataTable.Rows[curSkierIdx]["ScoreFeet"]).ToString("##0") + "</td>");
                            outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamDataTable.Rows[curSkierIdx]["NopsScore"]).ToString("###0.00") + "</td>");
                            outLine.Append("\n<td>" + (String)curJumpTeamDataTable.Rows[curSkierIdx]["PlcmtGroup"] + "</td>");
                            } catch {
                                outLine.Append("\n<td>Error in jump data</td>");
                            }
                        } else {
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                            outLine.Append("\n<td>&nbsp;</td>");
                        }
                        outLine.Append("\n</tr>");
                        outBuffer.WriteLine(outLine.ToString());
                    }
                }
                prevTeamCode = curTeamCode;
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());

            return true;
        }

        private Boolean writeIndivSlalomResults(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=OfficialSlalom></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("Non Team Skier Slalom Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Buoys</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>Rope</th>");
            outLine.Append("<th>MPH</th>");
            outLine.Append("<th>KPH</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curSlalomTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, SkierName";
            mySlalomAllDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomAllDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup <> 'CM' And AgeGroup <> 'CW' And AgeGroup <> 'BM' And AgeGroup <> 'BW'";
            curSlalomTeamSkierList = curSlalomDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curSlalomTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSlalomTeamSkierList[curSkierIdx]["AgeGroup"] + "</td>");
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSlalomTeamSkierList[curSkierIdx]["FinalPassScore"]).ToString("0.00") + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLenOff"] + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + (String)curSlalomTeamSkierList[curSkierIdx]["FinalLen"] + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedMph"]).ToString() + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                try {
                    outLine.Append("\n<td class=\"DataRight\">" + ((Byte)curSlalomTeamSkierList[curSkierIdx]["FinalSpeedKph"]).ToString() + "</td>");
                } catch {
                    outLine.Append("\n<td>&nbsp;</td>");
                }
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" ) + "</td>" );
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0.00" ) + "</td>" );
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeIndivTrickResults(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=OfficialTrick></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("Non Team Skier Trick Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Score</th>");
            outLine.Append("<th>Points</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curTrickTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, SkierName";
            myTrickAllDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickAllDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup <> 'CM' And AgeGroup <> 'CW' And AgeGroup <> 'BM' And AgeGroup <> 'BW'";
            curTrickTeamSkierList = curTrickDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curTrickTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curTrickTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curTrickTeamSkierList[curSkierIdx]["AgeGroup"] + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Int16)curTrickTeamSkierList[curSkierIdx]["ScoreTrick"]).ToString("##0.00") + "</td>");
                outLine.Append( "\n<td class=\"DataRight\">" + ( (Decimal)curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0.00" ) + "</td>" );
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeIndivJumpResults(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=OfficialJump></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("Non Team Skier Jump Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Team</th>");
            outLine.Append("<th>ScoreFeet</th>");
            outLine.Append("<th>ScoreMeters</th>");
            outLine.Append("<th>Points</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curJumpTeamSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, SkierName";
            myJumpAllDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpAllDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup <> 'CM' And AgeGroup <> 'CW' And AgeGroup <> 'BM' And AgeGroup <> 'BW'";
            curJumpTeamSkierList = curJumpDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curJumpTeamSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curJumpTeamSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curJumpTeamSkierList[curSkierIdx]["AgeGroup"] + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreFeet"]).ToString("##0.00") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamSkierList[curSkierIdx]["ScoreMeters"]).ToString("##0.00") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curJumpTeamSkierList[curSkierIdx]["PointsJump"]).ToString("###0.00") + "</td>");
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private Boolean writeIndivOverallResults(StreamWriter outBuffer) {
            StringBuilder outLine = new StringBuilder("");
            String curFilterCmd = "";
            String curSortCmd = "";

            outLine = new StringBuilder("");
            outLine.Append("\n<br/><br/><hr/><a name=OfficialOverall></a>");
            outLine.Append("\n<div Class=\"SectionTitle\">");
            outLine.Append("Non Team Skier Overall Results");
            outLine.Append("</div>");
            outLine.Append("\n<div class=\"MainContent\">");
            outBuffer.WriteLine(outLine.ToString());

            outLine = new StringBuilder("");
            outLine.Append("\n<Table Class=\"DataGridView\">");
            outLine.Append("\n<tr>");
            outLine.Append("<th>Skier</th>");
            outLine.Append("<th>Div</th>");
            outLine.Append("<th>Slalom</th>");
            outLine.Append("<th>Trick</th>");
            outLine.Append("<th>Jump</th>");
            outLine.Append("<th>Total</th>");

            outLine.Append("</tr>");
            outBuffer.WriteLine(outLine.ToString());

            DataRow[] curSkierList;
            curFilterCmd = "";
            curSortCmd = "AgeGroup, SkierName";
            mySummaryAllDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = mySummaryAllDataTable.DefaultView.ToTable();
            curFilterCmd = "AgeGroup <> 'CM' And AgeGroup <> 'CW' And AgeGroup <> 'BM' And AgeGroup <> 'BW'";
            curSkierList = curTrickDataTable.Select(curFilterCmd);

            for (int curSkierIdx = 0; curSkierIdx < curSkierList.Length; curSkierIdx++) {
                outLine = new StringBuilder("");
                outLine.Append("\n<tr>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSkierList[curSkierIdx]["SkierName"] + "</td>");
                outLine.Append("\n<td class=\"DataLeft\">" + (String)curSkierList[curSkierIdx]["AgeGroup"] + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSkierList[curSkierIdx]["PointsSlalom"]).ToString("#,##0.0") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSkierList[curSkierIdx]["PointsTrick"]).ToString("#,##0.0") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSkierList[curSkierIdx]["PointsJump"]).ToString("#,##0.0") + "</td>");
                outLine.Append("\n<td class=\"DataRight\">" + ((Decimal)curSkierList[curSkierIdx]["ScoreOverall"]).ToString("#,##0.0") + "</td>");
                outLine.Append("\n</tr>");
                outBuffer.WriteLine(outLine.ToString());
            }

            outLine = new StringBuilder("");
            outLine.Append("\n</Table>");
            outLine.Append("<a href=#PageTop>Return to Index</a>");
            outLine.Append("\n</div>");
            outBuffer.WriteLine(outLine.ToString());
            return true;
        }

        private StreamWriter getExportFile(String inFileFilter, String inFileName) {
            String curMethodName = "getExportFile";
            String myFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            if (inFileFilter == null) {
                curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else {
                curFileFilter = inFileFilter;
            }

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = curFileFilter;
            myFileDialog.FilterIndex = 1;
            if (inFileName == null) {
                myFileDialog.FileName = "";
            } else {
                myFileDialog.FileName = inFileName;
            }

            try {
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    myFileName = myFileDialog.FileName;
                    if (myFileName != null) {
                        int delimPos = myFileName.LastIndexOf('\\');
                        String curFileName = myFileName.Substring(delimPos + 1);
                        if (curFileName.IndexOf('.') < 0) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split('|');
                            if (curList.Length > 0) {
                                int curDelim = curList[1].IndexOf(".");
                                if (curDelim > 0) {
                                    curDefaultExt = curList[1].Substring(curDelim - 1);
                                }
                            }
                            myFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText(myFileName);
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: Could not get a file to export data to " + "\n\nError: " + ex.Message);
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile(curMsg);
            }

            return outBuffer;
        }

        private DataTable getTourData(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            DataTable curDataTable = getData(curSqlStmt.ToString());
            return curDataTable;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
