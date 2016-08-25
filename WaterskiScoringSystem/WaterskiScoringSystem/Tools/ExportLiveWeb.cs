using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
//using System.Web;
using System.Xml;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlServerCe;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportLiveWeb {
        private static int myTourRounds;
        private static ExportLiveWebDialog myLiveWebDialog = null;
        
        public static String LiveWebLocation = "";

        public ExportLiveWeb() {
        }

        public static ExportLiveWebDialog LiveWebDialog {
            get {
                if (myLiveWebDialog == null) {
                    myLiveWebDialog = new ExportLiveWebDialog();
                }
                return myLiveWebDialog; 
            }
        }

        public static Boolean exportCurrentSkiers(String inEvent, String inSanctionId, byte inRound, String inEventGroup) {
            String curMethodName = "exportCurrentSlalomSkiers";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curMsg = "";
            Boolean returnStatus = false;
            int curLineCount = 0;
            DataTable curDataTable = new DataTable();
            ProgressWindow myProgressInfo = new ProgressWindow();

            try {
                if (inEvent.Equals( "TrickVideo" )) {
                    curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
                    curSqlStmt.Append( "From TrickVideo S " );
                    curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
                    curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId " );
                    curSqlStmt.Append( "      AND ER.AgeGroup = S.AgeGroup AND ER.Event = 'Trick' " );
                    curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
                    curSqlStmt.Append( "AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1)" );
                    curSqlStmt.Append("Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId");
                    curDataTable = getData(curSqlStmt.ToString());
                } else {
                    curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round, ER.EventGroup " );
                    curSqlStmt.Append( "From " + inEvent + "Score S " );
                    curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
                    curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId " );
                    curSqlStmt.Append( "      AND ER.AgeGroup = S.AgeGroup AND ER.Event = '" + inEvent + "' " );
                    curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
                    curSqlStmt.Append( "AND S.Round = " + inRound + " " );
                    if (inEventGroup != null) {
                        if (inEventGroup.Equals( "All" )) {
                        } else if (inEventGroup.ToUpper().Equals( "MEN A" )) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'CM' " );
                        } else if (inEventGroup.ToUpper().Equals( "WOMEN A" )) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'CW' " );
                        } else if (inEventGroup.ToUpper().Equals( "MEN B" )) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'BM' " );
                        } else if (inEventGroup.ToUpper().Equals( "WOMEN B" )) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'BW' " );
                        } else if (inEventGroup.ToUpper().Equals( "NON TEAM" )) {
                            curSqlStmt.Append( "And ER.AgeGroup not in ('CM', 'CW', 'BM', 'BW') " );
                        } else {
                            curSqlStmt.Append( "AND ER.EventGroup = '" + inEventGroup + "' " );
                        }
                    }
                    curSqlStmt.Append( "Order by S.SanctionId, S.Round, ER.EventGroup, S.MemberId, S.AgeGroup" );
                    curDataTable = getData( curSqlStmt.ToString() );
                }

                myProgressInfo.setProgressMin( 1 );
                myProgressInfo.setProgressMax( curDataTable.Rows.Count );

                String curMemberId = "", curAgeGroup = "", curSkierName = "";
                byte curRound = 0;
                foreach (DataRow curRow in curDataTable.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curAgeGroup = (String)curRow["AgeGroup"];
                    curSkierName = (String)curRow["SkierName"];
                    curRound = (Byte)curRow["Round"];

                    curLineCount++;
                    myProgressInfo.setProgressValue( curLineCount );
                    myProgressInfo.setProgessMsg( "Processing " + curSkierName );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();

                    if (inEvent.Equals( "Slalom" )) {
                        exportCurrentSkierSlalom( inSanctionId, curMemberId, curAgeGroup, curRound, 0, inEvent );
                    } else if (inEvent.Equals( "Trick" )) {
                        exportCurrentSkierTrick( inSanctionId, curMemberId, curAgeGroup, curRound, 0, inEvent );
                    } else if (inEvent.Equals( "Jump" )) {
                        exportCurrentSkierJump( inSanctionId, curMemberId, curAgeGroup, curRound, 0, inEvent );
                    } else if ( inEvent.Equals("TrickVideo") ) {
                        ExportLiveWeb.exportCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
                    }
                }

                myProgressInfo.Close();
            } catch (Exception ex) {
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkierSlalom(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum, String inEventGroup) {
            String curMethodName = "exportCurrentSkierSlalom";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );
                if (inSkierRunNum > 1) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM SlalomScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append(exportData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ));

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Insert" ) );
                } else {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TourReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curXml.Append( exportData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Slalom'" );
                    curXml.Append( exportData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventRunOrder " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Slalom' And Round = " + inRound );
                    curXml.Append( exportData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM SlalomScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM SlalomRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM SlalomScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Update" ) );
                }

                curXml.Append( "</LiveWebRequest>" );
                SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                returnStatus = true;
            } catch (Exception ex) {
                returnStatus = false;
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                Log.WriteFile( curMethodName + ":Exception=" + ex.Message );
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkierTrick(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum, String inEventGroup) {
            String curMethodName = "exportCurrentSkierTrick";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );
                if (inSkierRunNum > 1) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TrickScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TrickPass " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Insert" ) );
                } else {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TourReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curXml.Append( exportData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Trick'" );
                    curXml.Append( exportData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventRunOrder " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Trick' And Round = " + inRound );
                    curXml.Append( exportData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM TrickScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM TrickPass " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TrickScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TrickPass " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inSkierRunNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
                    }
                    curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Update" ) );
                }

                curXml.Append( "</LiveWebRequest>" );
                SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                returnStatus = true;
            } catch (Exception ex) {
                returnStatus = false;
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                Log.WriteFile( curMethodName + ":Exception=" + ex.Message );
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkierTrickVideo(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
            String curMethodName = "exportCurrentSkierTrickVideo";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT * FROM TrickVideo " );
                curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                curSqlStmt.Append( "And Round = " + inRound );
                curXml.Append( exportData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

                curXml.Append( "</LiveWebRequest>" );
                SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                returnStatus = true;
            } catch (Exception ex) {
                returnStatus = false;
                //MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                Log.WriteFile( curMethodName + ":Exception=" + ex.Message );
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkierJump(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inPassNum, String inEventGroup) {
            String curMethodName = "exportCurrentSkierJump";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );
                if (inPassNum > 1) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM JumpScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM JumpRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inPassNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And PassNum = " + inPassNum );
                    }
                    curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Insert" ) );
                } else {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TourReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curXml.Append( exportData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Jump'" );
                    curXml.Append( exportData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventRunOrder " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = 'Jump' And Round = " + inRound );
                    curXml.Append( exportData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round FROM JumpScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT Distinct SanctionId, MemberId, AgeGroup, Round FROM JumpRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inPassNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And PassNum = " + inPassNum );
                    }
                    curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM JumpScore " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    curSqlStmt.Append( "And Round = " + inRound );
                    curXml.Append( exportData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM JumpRecap " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    if (inPassNum == 0) {
                        curSqlStmt.Append( "And Round = " + inRound );
                    } else {
                        curSqlStmt.Append( "And Round = " + inRound + " And PassNum = " + inPassNum );
                    }
                    curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Update" ) );
                }

                curXml.Append( "</LiveWebRequest>" );
                SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                returnStatus = true;
            } catch (Exception ex) {
                returnStatus = false;
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                Log.WriteFile( curMethodName + ":Exception=" + ex.Message );
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkiersRunOrder(String inEvent, String inSanctionId, byte inRound, String inEventGroup) {
            String curMethodName = "exportCurrentSkiersRunOrder";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            String curMsg = "";
            Boolean returnStatus = false;
            int curLineCount = 0;
            DataTable curDataTable = new DataTable();
            ProgressWindow myProgressInfo = new ProgressWindow();

            try {
                curSqlStmt.Append( "Select TR.SanctionId, TR.MemberId, TR.SkierName, TR.AgeGroup, ER.EventGroup, " + inRound + " as Round " );
                curSqlStmt.Append( "From TourReg TR " );
                curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId " );
                curSqlStmt.Append( "      AND TR.AgeGroup = ER.AgeGroup AND ER.Event = '" + inEvent + "' " );
                if (inRound > 0) {
                    curSqlStmt.Append( "Inner Join EventRunOrder RO on RO.SanctionId = TR.SanctionId AND RO.MemberId = TR.MemberId " );
                    curSqlStmt.Append( "      AND RO.AgeGroup = TR.AgeGroup AND RO.Event = ER.Event AND RO.Round = " + inRound + " " );
                }
                curSqlStmt.Append( "Where TR.SanctionId = '" + inSanctionId + "' " );
                if (inEventGroup != null) {
                    if (inEventGroup.Equals( "All" )) {
                    } else if (inEventGroup.ToUpper().Equals( "MEN A" )) {
                        curSqlStmt.Append( "And ER.AgeGroup = 'CM' " );
                    } else if (inEventGroup.ToUpper().Equals( "WOMEN A" )) {
                        curSqlStmt.Append( "And ER.AgeGroup = 'CW' " );
                    } else if (inEventGroup.ToUpper().Equals( "MEN B" )) {
                        curSqlStmt.Append( "And ER.AgeGroup = 'BM' " );
                    } else if (inEventGroup.ToUpper().Equals( "WOMEN B" )) {
                        curSqlStmt.Append( "And ER.AgeGroup = 'BW' " );
                    } else if (inEventGroup.ToUpper().Equals( "NON TEAM" )) {
                        curSqlStmt.Append( "And ER.AgeGroup not in ('CM', 'CW', 'BM', 'BW') " );
                    } else {
                        curSqlStmt.Append( "AND ER.EventGroup = '" + inEventGroup + "' " );
                    }
                }
                curSqlStmt.Append( "Order by TR.SanctionId, ER.EventGroup, TR.MemberId, TR.AgeGroup" );
                curDataTable = getData( curSqlStmt.ToString() );

                myProgressInfo.setProgressMin( 1 );
                myProgressInfo.setProgressMax( curDataTable.Rows.Count );

                String curMemberId = "", curAgeGroup = "", curSkierName = "";
                byte curRound = 0;
                foreach (DataRow curRow in curDataTable.Rows) {
                    curMemberId = (String)curRow["MemberId"];
                    curAgeGroup = (String)curRow["AgeGroup"];
                    curSkierName = (String)curRow["SkierName"];
                    curRound = (byte)((int)curRow["Round"]);

                    curLineCount++;
                    myProgressInfo.setProgressValue( curLineCount );
                    myProgressInfo.setProgessMsg( "Processing " + curSkierName );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();

                    curXml = new StringBuilder( "" );
                    curXml.Append( "<LiveWebRequest>" );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT * FROM TourReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + curMemberId + "' And AgeGroup = '" + curAgeGroup + "' " );
                    curXml.Append( exportData( "TourReg", new String[] { "SanctionId", "MemberId", "AgeGroup" }, curSqlStmt.ToString(), "Update" ) );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select * from EventReg " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + curMemberId + "' And AgeGroup = '" + curAgeGroup + "' " );
                    curSqlStmt.Append( "And Event = '" + inEvent + "' " );
                    curXml.Append( exportData( "EventReg", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }, curSqlStmt.ToString(), "Update" ) );

                    if (inRound > 0) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Select * from EventRunOrder " );
                        curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + curMemberId + "' And AgeGroup = '" + curAgeGroup + "' " );
                        curSqlStmt.Append( "And Event = '" + inEvent + "' And Round = " + inRound );
                        curXml.Append( exportData( "EventRunOrder", new String[] { "SanctionId", "MemberId", "AgeGroup", "Event", "Round" }, curSqlStmt.ToString(), "Update" ) );
                    }

                    curXml.Append( "</LiveWebRequest>" );
                    SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                    returnStatus = true;
                }

                myProgressInfo.Close();
            } catch (Exception ex) {
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return returnStatus;
        }

        public static Boolean exportTourData(String inSanctionId) {
            String curMethodName = "exportTourData";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );
            Boolean returnStatus = false;

            try {
                curSqlStmt = new StringBuilder( "" );
                curXml.Append( "<LiveWebRequest>" );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT * FROM Tournament " );
                curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
                curXml.Append( exportData( "Tournament", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Update" ) );

                curXml.Append( "</LiveWebRequest>" );
                SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
                Log.WriteFile( curMethodName + ":Connection successsful to " + LiveWebLocation + " for tournament " + inSanctionId );

                returnStatus = true;
            } catch (Exception ex) {
                returnStatus = false;
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                Log.WriteFile( curMethodName + ":Exception=" + ex.Message );
            }

            return returnStatus;
        }

        public static String exportData(String inTableName, String[] inKeyColumns, String inSqlStmt, String inCmd) {
            String curMethodName = "exportData";
            char[] singleQuoteDelim = new char[] { '\'' };
            String curMsg = "";
            String curValue;
            DataTable curDataTable = new DataTable();
            StringBuilder curXml = new StringBuilder( "" );

            try {
                curDataTable = getData( inSqlStmt );
                if (curDataTable == null) {
                    //curMsg = "No data found";
                } else {
                    curXml.Append( "<Table name=\"" + inTableName + "\" command=\"" + inCmd + "\" >" );
                    curXml.Append( "<Columns count=\"" + curDataTable.Columns.Count + "\">" );
                    foreach (DataColumn curColumn in curDataTable.Columns) {
                        if ( !(curColumn.ColumnName.ToUpper().Equals( "PK" )) ) {
                            curXml.Append( "<Column>" + curColumn.ColumnName + "</Column>" );
                        }
                    }
                    curXml.Append( "</Columns>" );

                    curXml.Append( "<Keys count=\"" + inKeyColumns.Length + "\">" );
                    foreach (String curColumn in inKeyColumns) {
                        curXml.Append( "<Key>" + curColumn + "</Key>" );
                    }
                    curXml.Append( "</Keys>" );

                    if (curDataTable.Rows.Count == 0 && inCmd.ToLower().Equals( "delete" )) {
                        //curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                        String[] curDelimValue = {" And " };
                        int curDelimPos = inSqlStmt.ToLower().IndexOf( "where" );
                        String curData = inSqlStmt.Substring( curDelimPos + 6 );
                        String[] curDataList = curData.Split( curDelimValue, StringSplitOptions.RemoveEmptyEntries );
                        curXml.Append( "<Rows count=\"1\">" );
                        curXml.Append( "<Row colCount=\"" + curDataList.Length + "\">" );
                        for (int curIdx = 0; curIdx < curDataList.Length; curIdx++) {
                            String[] curDataDef = curDataList[curIdx].Split( '=' );
                            curXml.Append( "<" + curDataDef[0].Trim() + ">");
                            curValue = curDataDef[1].Trim();
                            if (curValue.Substring( 0, 1 ).Equals( "'" )) {
                                curXml.Append( curValue.Substring( 1, curValue.Length - 2 ) );
                            } else {
                                curXml.Append( curValue );
                            }
                            curXml.Append( "</" + curDataDef[0].Trim() + ">" );
                        }
                        curXml.Append( "</Row>" );
                        curXml.Append( "</Rows>" );
                    } else {
                        curXml.Append( "<Rows count=\"" + curDataTable.Rows.Count + "\">" );
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curXml.Append( "<Row colCount=\"" + curDataTable.Columns.Count + "\">" );
                            foreach (DataColumn curColumn in curDataTable.Columns) {
                                if (!( curColumn.ColumnName.ToUpper().Equals( "PK" ) )) {
                                    if (curColumn.ColumnName.ToLower().Equals( "lastupdatedate" )) {
                                        curValue = ( (DateTime)curRow[curColumn.ColumnName] ).ToString( "yyyy-MM-dd HH:mm:ss" );
                                        curValue = encodeXmlValue( curValue );
                                        //curValue = stringReplace( curValue, singleQuoteDelim, "''" );
                                        curXml.Append( "<" + curColumn.ColumnName + ">" + curValue + "</" + curColumn.ColumnName + ">" );
                                    } else {
                                        curValue = stripLineFeedChar( curRow[curColumn.ColumnName].ToString() );
                                        curValue = stringReplace( curValue, singleQuoteDelim, "''" );
                                        curValue = encodeXmlValue( curValue );
                                        curXml.Append( "<" + curColumn.ColumnName + ">" + curValue + "</" + curColumn.ColumnName + ">" );
                                    }
                                }
                            }
                            curXml.Append( "</Row>" );
                        }
                        curXml.Append( "</Rows>" );
                    }
                    curXml.Append( "</Table>" );
                    Log.WriteFile( curMethodName + ":" + curXml.ToString() );
                    //MessageBox.Show( curXml.ToString() );
                }
                if (curMsg.Length > 1) {
                    MessageBox.Show( curMsg );
                }
                Log.WriteFile( curMethodName + ":conplete:" + curMsg );
            } catch (Exception ex) {
                MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return curXml.ToString();
        }

        private static DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        public static String encodeXmlValue(String inValue) {
            //String curEncodedXml = inValue.Replace( "&", "&amp;" ).Replace( "<", "&lt;" ).Replace( ">", "&gt;" ).Replace( "\"", "&quot;" ).Replace( "'", "&apos;" );
            String curEncodedXml = System.Security.SecurityElement.Escape( inValue );
            return curEncodedXml;
        }
        public static String stripLineFeedChar(String inValue) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
        }
        public static String stringReplace(String inValue, char[] inCurValue, String inReplValue) {
            StringBuilder curNewValue = new StringBuilder( "" );

            String[] curValues = inValue.Split( inCurValue );
            if (curValues.Length > 1) {
                int curCount = 0;
                foreach (String curValue in curValues) {
                    curCount++;
                    if ( curCount < curValues.Length) {
                        curNewValue.Append( curValue + inReplValue );
                    } else {
                        curNewValue.Append( curValue );
                    }
                }
            } else {
                curNewValue.Append( inValue );
            }

            return curNewValue.ToString();
        }

    }
}
