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
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportLiveScoreboard {
        private int myTourRounds;
        public static String myScoreboardLocation = "";

        public ExportLiveScoreboard() {
        }

        public static String ScoreboardLocation {
            get {
                return myScoreboardLocation;
            }
            set { myScoreboardLocation = value; }
        }

        public static String getCheckScoreboardLocation() {
            String curPath = Properties.Settings.Default.ExportDirectory;
            if (myScoreboardLocation.Length > 1) {
                DialogResult msgResp = MessageBox.Show( ""
                    + "\nCurrent Scoreboard location: " + myScoreboardLocation
                    + "\nClick Yes to keep this location, click No to select a new location"
                    , "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1 );
                if (msgResp == DialogResult.No) {
                    curPath = myScoreboardLocation;
                    myScoreboardLocation = "";
                }
            }

            if (myScoreboardLocation.Length == 0) {
                using (FolderBrowserDialog curFolderDialog = new FolderBrowserDialog()) {
                    curFolderDialog.ShowNewFolderButton = true;
                    curFolderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                    curFolderDialog.SelectedPath = @curPath;
                    if (FolderBrowserLauncher.ShowFolderBrowser( curFolderDialog, Form.ActiveForm ) == DialogResult.OK) {
                        curPath = curFolderDialog.SelectedPath;
                        if (Log.isDirectoryValid( curPath )) {
                            myScoreboardLocation = curPath;
                        } else {
                            MessageBox.Show( "A valid local data directory must be selected."
                                + "\nPlease use the Scoreboard icon to select a folder on a local drive or attached external drive" );
                            myScoreboardLocation = "";
                        }
                    }
                }
            }
            return myScoreboardLocation;
        }

        public static Boolean exportCurrentSkierSlalom(String inSanctionId, String inMemberId, String inAgeGroup, String inTeamCode, byte inRound, 
            String inSkierName, String inStartSpeed, String inStartLen,
            Decimal inScoreTotal, Decimal inFinalPassScore, String inFinalPassDesc
            ) {
            String curMethodName = "exportCurrentSkierSlalom";
            String curFileName = "LiveScoreboardSlalom.txt";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder("");

            try {
                StreamWriter outBuffer = getExportFile(curFileName);
                if (outBuffer == null) {
                    MessageBox.Show("Warning:" + curMethodName + "\nOutput file not available");
                    returnStatus = false;
                } else {
                    try {
                        outLine.Append("Current Skier = " + transformName(inSkierName));
                        outLine.Append(Environment.NewLine + "Division = " + inAgeGroup);
                        outLine.Append(Environment.NewLine + "Current Skier Start Speed = " + inStartSpeed);
                        outLine.Append(Environment.NewLine + "Current Skier Start Line = " + inStartLen);
                        if (inTeamCode != null) {
                            if (inTeamCode.Length > 0) {
                                DataRow curRow = getTeamData(inSanctionId, inTeamCode);
                                if (curRow == null) {
                                    outLine.Append(Environment.NewLine + "Current Skier Team = " + inTeamCode);
                                } else {
                                    outLine.Append(Environment.NewLine + "Current Skier Team = " + (String)curRow["Name"]);
                                }
                            }
                        }
                        if (inFinalPassDesc != null) {
                            outLine.Append(Environment.NewLine + "Current Skier Total Buoys = " + inScoreTotal.ToString("##0.00"));
                            outLine.Append(Environment.NewLine + "Current Skier Last Pass Buoys = " + inFinalPassScore.ToString("##0.00"));
                            outLine.Append(Environment.NewLine + "Current Skier Last Pass Info = " + inFinalPassDesc);
                        }

                        //Write output line to file
                        outBuffer.WriteLine(outLine.ToString());
                    } catch (Exception ex) {
                        MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                        returnStatus = false;
                    } finally {
                        outBuffer.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                returnStatus = false;
            }

            return returnStatus;
        }

        public static Boolean exportCurrentLeaderSlalom(DataTable inSummaryDataTable, String inSanctionId) {
            String curMethodName = "exportCurrentLeaderSlalom";
            String curFileName = "LiveScoreboardSlalomLeader.txt";
            String curEventClass = "", curTeamCode = "";
            Int16 curCount = 1;
            Boolean isIwwfFormat = false;
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder("");
            if (inSummaryDataTable.Columns.Contains("EventClassSlalom")) {
                isIwwfFormat = true;
            } else {
                isIwwfFormat = false;
            }

            try {
                StreamWriter outBuffer = getExportFile(curFileName);
                if (outBuffer == null) {
                    MessageBox.Show("Warning:" + curMethodName + "\nOutput file not available");
                    returnStatus = false;
                } else {
                    try {
                        foreach (DataRow curRow in inSummaryDataTable.Rows) {
                            try {
                                if (isIwwfFormat) {
                                    curEventClass = (String)curRow["EventClassSlalom"];
                                } else {
                                    curEventClass = (String)curRow["EventClass"];
                                }
                            } catch {
                                curEventClass = "";
                            }

                            if (curEventClass.Length > 0) {
                                outLine = new StringBuilder("");
                                outLine.Append("Position " + curCount.ToString("#0") + " Skier = " + transformName((String)curRow["SkierName"]));
                                outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Division = " + ((String)curRow["AgeGroup"]).Substring(0, 2));
                                try {
                                    curTeamCode = (String)curRow["TeamCode"];
                                } catch {
                                    curTeamCode = "";
                                }
                                if (curTeamCode != null) {
                                    if (curTeamCode.Length > 0) {
                                        DataRow curTeamRow = getTeamData(inSanctionId, curTeamCode);
                                        if (curRow == null) {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + curTeamCode);
                                        } else {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + (String)curRow["Name"]);
                                        }
                                    }
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Buoys = " + ((Decimal)curRow["Score"]).ToString("##0.00"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Buoys = ***.**");
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Speed = " + (Byte)curRow["FinalSpeedMph"] + " Mph");
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Speed = ** Mph");
                                }
                                try {
                                    outLine.Append(" / " + (Byte)curRow["FinalSpeedKph"] + " Kph");
                                } catch {
                                    outLine.Append(" / ** Kph");
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Rope = " + (String)curRow["FinalLenOff"]);
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Rope = *****");
                                }
                                try {
                                    outLine.Append(" / " + (String)curRow["FinalLen"] + "M");
                                } catch {
                                    outLine.Append(" / ***** ");
                                }
                                outLine.Append(Environment.NewLine + "");

                                //Write output line to file
                                outBuffer.WriteLine(outLine.ToString());
                                curCount++;
                            }
                        }
                    } catch (Exception ex) {
                        MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                        returnStatus = false;
                    } finally {
                        outBuffer.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                returnStatus = false;
            }

            return returnStatus;
        }

        public static Boolean exportCurrentSkierJump(String inSanctionId, String inMemberId, String inAgeGroup, String inTeamCode, byte inRound, String inSkierName,
            String inBoatSpeed, String inRampHeight, String inJumpNum, String inResults, String inScoreBestFeet, String inScoreBestMeters, String inScoreFeet, String inScoreMeters, String inActiveStatus
            ) {
            String curMethodName = "exportCurrentSkierJump";
            String curFileName = "LiveScoreboardJump.txt";
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder("");

            try {
                StreamWriter outBuffer = getExportFile(curFileName);
                if (outBuffer == null) {
                    MessageBox.Show("Warning:" + curMethodName + "\nOutput file not available");
                    returnStatus = false;
                } else {
                    try {
                        outLine.Append("Current Skier = " + transformName(inSkierName));
                        outLine.Append(Environment.NewLine + "Division = " + inAgeGroup);
                        outLine.Append(Environment.NewLine + "Current Skier Speed = " + inBoatSpeed);
                        outLine.Append(Environment.NewLine + "Current Skier Ramp Height = " + inRampHeight);
                        if (inTeamCode != null) {
                            if (inTeamCode.Length > 0) {
                                DataRow curRow = getTeamData(inSanctionId, inTeamCode);
                                if (curRow == null) {
                                    outLine.Append(Environment.NewLine + "Current Skier Team = " + inTeamCode);
                                } else {
                                    outLine.Append(Environment.NewLine + "Current Skier Team = " + (String)curRow["Name"]);
                                }
                            }
                        }
                        if (inActiveStatus != null) {
                            outLine.Append(Environment.NewLine + "Current Skier Best Distance Feet = " + inScoreBestFeet);
                            outLine.Append(Environment.NewLine + "Current Skier Best Distance Meters = " + inScoreBestMeters);
                            outLine.Append(Environment.NewLine + "Current Skier Last Jump Num = " + inJumpNum);
                            outLine.Append(Environment.NewLine + "Current Skier Last Results = " + inResults);
                            if (inScoreFeet.ToLower().Equals("null")) {
                                outLine.Append(Environment.NewLine + "Current Skier Last Distance Feet = " + inScoreFeet);
                            }
                            if (inScoreMeters.ToLower().Equals("null")) {
                                outLine.Append(Environment.NewLine + "Current Skier Last Distance Meters = " + inScoreMeters);
                            }
                        }

                        //Write output line to file
                        outBuffer.WriteLine(outLine.ToString());
                    } catch (Exception ex) {
                        MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                        returnStatus = false;
                    } finally {
                        outBuffer.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                returnStatus = false;
            }

            return returnStatus;
        }

        public static Boolean exportCurrentLeaderJump(DataTable inSummaryDataTable, String inSanctionId) {
            String curMethodName = "exportCurrentLeaderJump";
            String curFileName = "LiveScoreboardJumpLeader.txt";
            String curEventClass = "", curTeamCode = "";
            Int16 curCount = 1;
            Boolean isIwwfFormat = false;
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder("");
            if (inSummaryDataTable.Columns.Contains("EventClassJump")) {
                isIwwfFormat = true;
            } else {
                isIwwfFormat = false;
            }

            try {
                StreamWriter outBuffer = getExportFile(curFileName);
                if (outBuffer == null) {
                    MessageBox.Show("Warning:" + curMethodName + "\nOutput file not available");
                    returnStatus = false;
                } else {
                    try {
                        foreach (DataRow curRow in inSummaryDataTable.Rows) {
                            try {
                                if (isIwwfFormat) {
                                    curEventClass = (String)curRow["EventClassJump"];
                                } else {
                                    curEventClass = (String)curRow["EventClass"];
                                }
                            } catch {
                                curEventClass = "";
                            }

                            if (curEventClass.Length > 0) {
                                outLine = new StringBuilder("");
                                outLine.Append("Position " + curCount.ToString("#0") + " Skier = " + transformName((String)curRow["SkierName"]));
                                outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Division = " + ((String)curRow["AgeGroup"]).Substring(0, 2));
                                try {
                                    curTeamCode = (String)curRow["TeamCode"];
                                } catch {
                                    curTeamCode = "";
                                }
                                if (curTeamCode != null) {
                                    if (curTeamCode.Length > 0) {
                                        DataRow curTeamRow = getTeamData(inSanctionId, curTeamCode);
                                        if (curRow == null) {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + curTeamCode);
                                        } else {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + (String)curRow["Name"]);
                                        }
                                    }
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Meters = " + ((Decimal)curRow["ScoreMeters"]).ToString("#0.0"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Meters = **.*");
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Feet = " + ((Decimal)curRow["ScoreFeet"]).ToString("#0.0"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Feet = ***");
                                }
                                outLine.Append(Environment.NewLine + "");

                                //Write output line to file
                                outBuffer.WriteLine(outLine.ToString());
                                curCount++;
                            }
                        }
                    } catch (Exception ex) {
                        MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                        returnStatus = false;
                    } finally {
                        outBuffer.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                returnStatus = false;
            }

            return returnStatus;
        }

        public static Boolean exportCurrentLeaderTrick(DataTable inSummaryDataTable, String inSanctionId) {
            String curMethodName = "exportCurrentLeaderTrick";
            String curFileName = "LiveScoreboardTrickLeader.txt";
            String curEventClass = "", curTeamCode = "";
            Int16 curCount = 1;
            Boolean isIwwfFormat = false;
            Boolean returnStatus = false;
            StringBuilder outLine = new StringBuilder("");
            if (inSummaryDataTable.Columns.Contains("EventClassTrick")) {
                isIwwfFormat = true;
            } else {
                isIwwfFormat = false;
            }

            try {
                StreamWriter outBuffer = getExportFile(curFileName);
                if (outBuffer == null) {
                    MessageBox.Show("Warning:" + curMethodName + "\nOutput file not available");
                    returnStatus = false;
                } else {
                    try {
                        foreach (DataRow curRow in inSummaryDataTable.Rows) {
                            try {
                                if (isIwwfFormat) {
                                    curEventClass = (String)curRow["EventClassTrick"];
                                } else {
                                    curEventClass = (String)curRow["EventClass"];
                                }
                            } catch {
                                curEventClass = "";
                            }

                            if (curEventClass.Length > 0) {
                                outLine = new StringBuilder("");
                                outLine.Append("Position " + curCount.ToString("#0") + " Skier = " + transformName((String)curRow["SkierName"]));
                                outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Division = " + ((String)curRow["AgeGroup"]).Substring(0, 2));
                                try {
                                    curTeamCode = (String)curRow["TeamCode"];
                                } catch {
                                    curTeamCode = "";
                                }
                                if (curTeamCode != null) {
                                    if (curTeamCode.Length > 0) {
                                        DataRow curTeamRow = getTeamData(inSanctionId, curTeamCode);
                                        if (curRow == null) {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + curTeamCode);
                                        } else {
                                            outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Team = " + (String)curRow["Name"]);
                                        }
                                    }
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Pass 1 = " + ((Int16)curRow["ScorePass1"]).ToString("##,##0"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Pass 1 = *****");
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Pass 2 = " + ((Int16)curRow["ScorePass2"]).ToString("##,##0"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Pass 2 = *****");
                                }
                                try {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Total = " + ((Int16)curRow["Score"]).ToString("##,##0"));
                                } catch {
                                    outLine.Append(Environment.NewLine + "Position " + curCount.ToString("#0") + " Total = *****");
                                }
                                outLine.Append(Environment.NewLine + "");

                                //Write output line to file
                                outBuffer.WriteLine(outLine.ToString());
                                curCount++;
                            }
                        }
                    } catch (Exception ex) {
                        MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                        returnStatus = false;
                    } finally {
                        outBuffer.Close();
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show("Error:" + curMethodName + " Could not write file \n\nError: " + ex.Message);
                returnStatus = false;
            }

            return returnStatus;
        }

        private static String transformName(String inSkierName) {
            String curReturnValue = "";
            int curDelim = inSkierName.IndexOf(',');
            String curFirstName = inSkierName.Substring(curDelim + 1);
            String curLastName = inSkierName.Substring(0, curDelim - 1);
            curReturnValue = curFirstName + " " + curLastName;
            return curReturnValue;
        }

        private static DataRow getTeamData(String inSanctionId, String inTeamCode) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT PK, SanctionId, TeamCode, Name, ContactName, ContactInfo, LastUpdateDate, Notes ");
            curSqlStmt.Append(", RunOrder, SlalomRunOrder, TrickRunOrder, JumpRunOrder ");
            curSqlStmt.Append("FROM TeamList ");
            curSqlStmt.Append("WHERE SanctionId = '" + inSanctionId + "' AND TeamCode = '" + inTeamCode + "' ");
            curSqlStmt.Append("Order By TeamCode, Name ");
            DataTable curDataTable = getData(curSqlStmt.ToString());
            if (curDataTable.Rows.Count > 0) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private static StreamWriter getExportFile(String inFileName) {
            String curMethodName = "getExportFile";
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            String myFileName = ScoreboardLocation + "\\" + inFileName;
            try {
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
                    /*
                    if (File.Exists(myFileName)) {
                        outBuffer = File.AppendText(myFileName);
                    } else {
                        outBuffer = File.CreateText(myFileName);
                    }
                     */
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: Could not get a file to export data to " + "\n\nError: " + ex.Message);
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile(curMsg);
            }

            return outBuffer;
        }

        private static DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
