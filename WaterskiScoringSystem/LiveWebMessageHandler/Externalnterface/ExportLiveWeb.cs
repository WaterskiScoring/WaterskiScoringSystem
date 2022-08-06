using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using LiveWebMessageHandler.Common;

namespace LiveWebMessageHandler.Externalnterface {
    class ExportLiveWeb {
        public static String LiveWebLocation = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";

        public ExportLiveWeb() {
        }

        public static void exportCurrentSkiers( String inEvent, String inSanctionId, byte inRound, String inEventGroup ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSlalomSkiers: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            int curLineCount = 0;
            DataTable curDataTable = new DataTable();

            try {
                if ( inEvent.Equals( "TrickVideo" ) ) {
                    curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round " );
                    curSqlStmt.Append( "From TrickVideo S " );
                    curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
                    curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId " );
                    curSqlStmt.Append( "      AND ER.AgeGroup = S.AgeGroup AND ER.Event = 'Trick' " );
                    curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
                    curSqlStmt.Append( "AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1)" );
                    curSqlStmt.Append( "Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId" );
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

                } else {
                    curSqlStmt.Append( "Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round, ER.EventGroup " );
                    curSqlStmt.Append( "From " + inEvent + "Score S " );
                    curSqlStmt.Append( "Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup " );
                    curSqlStmt.Append( "Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId " );
                    curSqlStmt.Append( "      AND ER.AgeGroup = S.AgeGroup AND ER.Event = '" + inEvent + "' " );
                    curSqlStmt.Append( "Where S.SanctionId = '" + inSanctionId + "' " );
                    curSqlStmt.Append( "AND S.Round = " + inRound + " " );
                    if ( inEventGroup != null ) {
                        if ( inEventGroup.Equals( "All" ) ) {
                        } else if ( inEventGroup.ToUpper().Equals( "MEN A" ) ) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'CM' " );
                        } else if ( inEventGroup.ToUpper().Equals( "WOMEN A" ) ) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'CW' " );
                        } else if ( inEventGroup.ToUpper().Equals( "MEN B" ) ) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'BM' " );
                        } else if ( inEventGroup.ToUpper().Equals( "WOMEN B" ) ) {
                            curSqlStmt.Append( "And ER.AgeGroup = 'BW' " );
                        } else if ( inEventGroup.ToUpper().Equals( "NON TEAM" ) ) {
                            curSqlStmt.Append( "And ER.AgeGroup not in ('CM', 'CW', 'BM', 'BW') " );
                        } else {
                            curSqlStmt.Append( "AND ER.EventGroup = '" + inEventGroup + "' " );
                        }
                    }
                    curSqlStmt.Append( "Order by S.SanctionId, S.Round, ER.EventGroup, S.MemberId, S.AgeGroup" );
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                }

                String curMemberId = "", curAgeGroup = "", curSkierName = "";
                byte curRound = 0;
                foreach ( DataRow curRow in curDataTable.Rows ) {
                    curMemberId = (String)curRow["MemberId"];
                    curAgeGroup = (String)curRow["AgeGroup"];
                    curSkierName = (String)curRow["SkierName"];
                    curRound = (Byte)curRow["Round"];

                    curLineCount++;

                    try {
                        if ( inEvent.Equals( "Slalom" ) ) {
                            exportCurrentSkierSlalom( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
                            continue;

                        } else if ( inEvent.Equals( "Trick" ) ) {
                            exportCurrentSkierTrick( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
                            continue;

                        } else if ( inEvent.Equals( "Jump" ) ) {
                            exportCurrentSkierJump( inSanctionId, curMemberId, curAgeGroup, curRound, 0 );
                            continue;

                        } else if ( inEvent.Equals( "TrickVideo" ) ) {
                            ExportLiveWeb.exportCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
                            continue;
                        }

                        String curErrMsg = String.Format( "Error encountered sending {0} scores for {1} {2}", inEvent, curAgeGroup, curSkierName );
                        Log.WriteFile( curErrMsg );

                    } catch ( Exception ex ) {
                        String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
                        Log.WriteFile( curErrMsg );
                        throw new Exception( curErrMsg );
                    }
                }

            } catch ( Exception ex ) {
                String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
                Log.WriteFile( curErrMsg );
                throw new Exception( curErrMsg );
            }
        }

        public static void exportCurrentSkier( String inSanctionId, String inEvent, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalom: ";

            if ( inEvent.Equals( "Slalom" ) ) {
                exportCurrentSkierSlalom( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
                return;

            } else if ( inEvent.Equals( "Trick" ) ) {
                exportCurrentSkierTrick( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
                return;

            } else if ( inEvent.Equals( "Jump" ) ) {
                exportCurrentSkierJump( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
                return;
            }

            String curErrMsg = String.Format( "{0}Invalid event {1} encountered", curMethodName, inEvent );
            Log.WriteFile( curErrMsg );
            throw new Exception( curErrMsg );
        }

        public static void exportCurrentSkierSlalom( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalom: ";
            if ( inSkierRunNum <= 1 ) {
                exportCurrentSkierSlalomFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
                return;
            }

            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt.Append( "SELECT * FROM SlalomScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            if ( inSkierRunNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
            }
            curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Insert" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierSlalomFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierSlalomFirst: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );

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
            if ( inSkierRunNum == 0 ) {
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
            if ( inSkierRunNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
            }
            curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Update" ) );
            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierTrick( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierTrick: ";
            if ( inSkierRunNum <= 1 ) {
                exportCurrentSkierTrickFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inSkierRunNum );
                return;
            }
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickPass " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            if ( inSkierRunNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
            }
            curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Insert" ) );

            curXml.Append( "</LiveWebRequest>" );

            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierTrickFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inSkierRunNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierTrickFirst: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );
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
            if ( inSkierRunNum == 0 ) {
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
            if ( inSkierRunNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And SkierRunNum = " + inSkierRunNum );
            }
            curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Update" ) );

            curXml.Append( "</LiveWebRequest>" );

            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierTrickVideo(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierTrickVideo: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickVideo " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierJump(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inPassNum) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierJump: ";
            if ( inPassNum <= 1 ) {
                exportCurrentSkierJumpFirst( inSanctionId, inMemberId, inAgeGroup, inRound, inPassNum );
                return;
            }
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt.Append( "SELECT * FROM JumpScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Insert" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpRecap " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            if ( inPassNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And PassNum = " + inPassNum );
            }
            curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Insert" ) );

            curXml.Append( "</LiveWebRequest>" );

            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportCurrentSkierJumpFirst( String inSanctionId, String inMemberId, String inAgeGroup, byte inRound, int inPassNum ) {
            String curMethodName = "ExportLiveWeb: exportCurrentSkierJumpFirst: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curXml.Append( "<LiveWebRequest>" );

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
            if ( inPassNum == 0 ) {
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
            if ( inPassNum == 0 ) {
                curSqlStmt.Append( "And Round = " + inRound );
            } else {
                curSqlStmt.Append( "And Round = " + inRound + " And PassNum = " + inPassNum );
            }
            curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Update" ) );

            curXml.Append( "</LiveWebRequest>" );

            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void exportTourData(String inSanctionId) {
            String curMethodName = "ExportLiveWeb: exportTourData: ";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            StringBuilder curXml = new StringBuilder( "" );

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM Tournament " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
            curXml.Append( exportData( "Tournament", new String[] { "SanctionId" }, curSqlStmt.ToString(), "Update" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void disableSkiers(String inEvent, String inSanctionId, byte inRound, String inEventGroup) {
			String curMethodName = "ExportLiveWeb: disableSkiers: ";
			StringBuilder curSqlStmt = new StringBuilder("");
			int curLineCount = 0;
			DataTable curDataTable = new DataTable();

			try {
				if (inEvent.Equals("TrickVideo")) {
					curSqlStmt.Append("Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round ");
					curSqlStmt.Append("From TrickVideo S ");
					curSqlStmt.Append("Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup ");
					curSqlStmt.Append("Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId ");
					curSqlStmt.Append("      AND ER.AgeGroup = S.AgeGroup AND ER.Event = 'Trick' ");
					curSqlStmt.Append("Where S.SanctionId = '" + inSanctionId + "' ");
					curSqlStmt.Append("AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1)");
					curSqlStmt.Append("Order by S.SanctionId, S.Round, S.AgeGroup, S.MemberId");
					curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());

				} else {
					curSqlStmt.Append("Select S.SanctionId, S.MemberId, TR.SkierName, S.AgeGroup, S.Round, ER.EventGroup ");
					curSqlStmt.Append("From " + inEvent + "Score S ");
					curSqlStmt.Append("Inner Join TourReg TR on TR.SanctionId = S.SanctionId AND TR.MemberId = S.MemberId AND TR.AgeGroup = S.AgeGroup ");
					curSqlStmt.Append("Inner Join EventReg ER on ER.SanctionId = S.SanctionId AND ER.MemberId = S.MemberId ");
					curSqlStmt.Append("      AND ER.AgeGroup = S.AgeGroup AND ER.Event = '" + inEvent + "' ");
					curSqlStmt.Append("Where S.SanctionId = '" + inSanctionId + "' ");
					curSqlStmt.Append("AND S.Round = " + inRound + " ");
					if (inEventGroup != null) {
						if (inEventGroup.Equals("All")) {
						} else if (inEventGroup.ToUpper().Equals("MEN A")) {
							curSqlStmt.Append("And ER.AgeGroup = 'CM' ");
						} else if (inEventGroup.ToUpper().Equals("WOMEN A")) {
							curSqlStmt.Append("And ER.AgeGroup = 'CW' ");
						} else if (inEventGroup.ToUpper().Equals("MEN B")) {
							curSqlStmt.Append("And ER.AgeGroup = 'BM' ");
						} else if (inEventGroup.ToUpper().Equals("WOMEN B")) {
							curSqlStmt.Append("And ER.AgeGroup = 'BW' ");
						} else if (inEventGroup.ToUpper().Equals("NON TEAM")) {
							curSqlStmt.Append("And ER.AgeGroup not in ('CM', 'CW', 'BM', 'BW') ");
						} else {
							curSqlStmt.Append("AND ER.EventGroup = '" + inEventGroup + "' ");
						}
					}
					curSqlStmt.Append("Order by S.SanctionId, S.Round, ER.EventGroup, S.MemberId, S.AgeGroup");
					curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
				}

				String curMemberId = "", curAgeGroup = "", curSkierName = "";
				byte curRound = 0;
				foreach (DataRow curRow in curDataTable.Rows) {
					curMemberId = (String)curRow["MemberId"];
					curAgeGroup = (String)curRow["AgeGroup"];
					curSkierName = (String)curRow["SkierName"];
					curRound = (Byte)curRow["Round"];

					curLineCount++;

                    try {
                        if ( inEvent.Equals( "Slalom" ) ) {
                            disableCurrentSkierSlalom( inSanctionId, curMemberId, curAgeGroup, curRound );
                            continue;

                        } else if ( inEvent.Equals( "Trick" ) ) {
                            disableCurrentSkierTrick( inSanctionId, curMemberId, curAgeGroup, curRound );
                            continue;

                        } else if ( inEvent.Equals( "Jump" ) ) {
                            disableCurrentSkierJump( inSanctionId, curMemberId, curAgeGroup, curRound );
                            continue;

                        } else if ( inEvent.Equals( "TrickVideo" ) ) {
                            ExportLiveWeb.disableCurrentSkierTrickVideo( inSanctionId, curMemberId, curAgeGroup, curRound );
                            continue;
                        }

                        String curErrMsg = String.Format( "Error encountered sending {0} scores for {1} {2}", inEvent, curAgeGroup, curSkierName );
                        Log.WriteFile( curErrMsg );

                    } catch ( Exception ex ) {
                        String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
                        Log.WriteFile( curErrMsg );
                        throw new Exception( curErrMsg );
                    }
				}

            } catch ( Exception ex ) {
                String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
                Log.WriteFile( curErrMsg );
                throw new Exception( curErrMsg );
            }
        }

        public static void disableCurrentSkier( String inSanctionId, String inEvent, String inMemberId, String inAgeGroup, byte inRound ) {
            String curMethodName = "ExportLiveWeb: disableCurrentSkier: ";

            if ( inEvent.Equals( "Slalom" ) ) {
                disableCurrentSkierSlalom( inSanctionId, inMemberId, inAgeGroup, inRound );
                return;

            } else if ( inEvent.Equals( "Trick" ) ) {
                disableCurrentSkierTrick( inSanctionId, inMemberId, inAgeGroup, inRound );
                return;

            } else if ( inEvent.Equals( "Jump" ) ) {
                disableCurrentSkierJump( inSanctionId, inMemberId, inAgeGroup, inRound );
                return;
            }

            String curErrMsg = String.Format( "{0}Invalid event {1} encountered", curMethodName, inEvent );
            Log.WriteFile( curErrMsg );
            throw new Exception( curErrMsg );
        }

        public static void disableCurrentSkierSlalom(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierSlalom: ";
			StringBuilder curSqlStmt = new StringBuilder("");
			StringBuilder curXml = new StringBuilder("");

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM SlalomScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "SlalomScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM SlalomRecap " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "SlalomRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }, curSqlStmt.ToString(), "Delete" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
		}

		public static void disableCurrentSkierTrick(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierTrick: ";
			StringBuilder curSqlStmt = new StringBuilder("");
			StringBuilder curXml = new StringBuilder("");

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "TrickScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickPass " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "TrickPass", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }, curSqlStmt.ToString(), "Delete" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void disableCurrentSkierTrickVideo(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierTrickVideo: ";
			StringBuilder curSqlStmt = new StringBuilder("");
			StringBuilder curXml = new StringBuilder("");

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM TrickVideo " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "TrickVideo", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static void disableCurrentSkierJump(String inSanctionId, String inMemberId, String inAgeGroup, byte inRound) {
			String curMethodName = "ExportLiveWeb: disableCurrentSkierJump: ";
			StringBuilder curSqlStmt = new StringBuilder("");
			StringBuilder curXml = new StringBuilder("");

            curSqlStmt = new StringBuilder( "" );
            curXml.Append( "<LiveWebRequest>" );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpScore " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "JumpScore", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }, curSqlStmt.ToString(), "Delete" ) );

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * FROM JumpRecap " );
            curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "And Round = " + inRound );
            curXml.Append( exportData( "JumpRecap", new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }, curSqlStmt.ToString(), "Delete" ) );

            curXml.Append( "</LiveWebRequest>" );
            String curMsgResp = SendMessageHttp.sendMessagePostXml( LiveWebLocation, curXml.ToString() );
            if ( curMsgResp.Length > 0 ) {
                String curMsg = String.Format( "{0}{1}", curMethodName, curMsgResp );
                Log.WriteFile( curMsg );

            } else {
                String curMsg = String.Format( "{0}Request failed: likely connection issue", curMethodName );
                throw new Exception( curMsg );
            }
        }

        public static String exportData(String inTableName, String[] inKeyColumns, String inSqlStmt, String inCmd) {
            String curMethodName = "ExportLiveWeb: exportData: ";
            char[] singleQuoteDelim = new char[] { '\'' };
            String curValue;
            DataTable curDataTable = new DataTable();
            StringBuilder curXml = new StringBuilder( "" );

            try {
                curDataTable = DataAccess.getDataTable( inSqlStmt );
                if (curDataTable == null) return "No data found to process";

                curXml.Append( "<Table name=\"" + inTableName + "\" command=\"" + inCmd + "\" >" );
                curXml.Append( "<Columns count=\"" + curDataTable.Columns.Count + "\">" );
                foreach ( DataColumn curColumn in curDataTable.Columns ) {
                    if ( !( curColumn.ColumnName.ToUpper().Equals( "PK" ) ) && !( curColumn.ColumnName.ToLower().Equals( "reridereason " ) ) ) {
                        curXml.Append( "<Column>" + curColumn.ColumnName + "</Column>" );
                    }
                }
                curXml.Append( "</Columns>" );

                curXml.Append( "<Keys count=\"" + inKeyColumns.Length + "\">" );
                foreach ( String curColumn in inKeyColumns ) {
                    curXml.Append( "<Key>" + curColumn + "</Key>" );
                }
                curXml.Append( "</Keys>" );

                if ( curDataTable.Rows.Count == 0 && inCmd.ToLower().Equals( "delete" ) ) {
                    //curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' And MemberId = '" + inMemberId + "' And AgeGroup = '" + inAgeGroup + "' " );
                    String[] curDelimValue = { " And " };
                    int curDelimPos = inSqlStmt.ToLower().IndexOf( "where" );
                    String curData = inSqlStmt.Substring( curDelimPos + 6 );
                    String[] curDataList = curData.Split( curDelimValue, StringSplitOptions.RemoveEmptyEntries );
                    curXml.Append( "<Rows count=\"1\">" );
                    curXml.Append( "<Row colCount=\"" + curDataList.Length + "\">" );
                    for ( int curIdx = 0; curIdx < curDataList.Length; curIdx++ ) {
                        String[] curDataDef = curDataList[curIdx].Split( '=' );
                        curXml.Append( "<" + curDataDef[0].Trim() + ">" );
                        curValue = curDataDef[1].Trim();
                        if ( curValue.Substring( 0, 1 ).Equals( "'" ) ) {
                            curXml.Append( curValue.Substring( 1, curValue.Length - 2 ) );
                        } else {
                            curXml.Append( curValue );
                        }
                        curXml.Append( "</" + curDataDef[0].Trim() + ">" );
                    }
                    curXml.Append( "</Row>" );
                    curXml.Append( "</Rows>" );

                } else {
                    if ( curDataTable.Rows.Count == 0 ) return "";

                    curXml.Append( "<Rows count=\"" + curDataTable.Rows.Count + "\">" );
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        curXml.Append( "<Row colCount=\"" + curDataTable.Columns.Count + "\">" );
                        foreach ( DataColumn curColumn in curDataTable.Columns ) {
                            if ( !( curColumn.ColumnName.ToUpper().Equals( "PK" ) ) && !( curColumn.ColumnName.ToLower().Equals( "reridereason " ) ) ) {
                                if ( curColumn.ColumnName.ToLower().Equals( "lastupdatedate" ) || curColumn.ColumnName.ToLower().Equals( "insertdate" ) ) {
                                    curValue = ( (DateTime)curRow[curColumn.ColumnName] ).ToString( "yyyy-MM-dd HH:mm:ss" );
                                    curValue = encodeXmlValue( curValue );
                                    curXml.Append( "<" + curColumn.ColumnName + ">" + curValue + "</" + curColumn.ColumnName + ">" );
                                } else {
                                    curValue = stripLineFeedChar( curRow[curColumn.ColumnName].ToString() );
                                    curValue = HelperFunctions.stringReplace( curValue, singleQuoteDelim, "''" );
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
                return curXml.ToString();

            } catch (Exception ex) {
				String curErrMsg = String.Format( "{0}Exception encountered {1}", curMethodName, ex.Message );
                throw new Exception( curErrMsg );
            }
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

	}
}
