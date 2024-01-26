using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportTourSummary {
        private bool isSlalomActive = false;
        private bool isTrickActive = false;
        private bool isJumpActive = false;
        private String mySanctionNum;
        private String myTourRules;

        private DataRow myTourRow;

        private StreamWriter myOutBuffer = null;
        private TourProperties myTourProperties;

        public ExportTourSummary() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            try {
                if ( mySanctionNum == null ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    if ( mySanctionNum.Length < 6 ) {
                        MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportTourSummary instantiation method"
                    + "\n\nException: " + ex.Message
                );
            }
        }

        public bool ExportData() {
            DataTable curDataTable, curDetailDataTable, curScoresDataTable;
            try {
                //Retrieve selected tournament attributes
                DataTable curTourDataTable = getTourData();
                myTourRow = curTourDataTable.Rows[0];
                myTourRules = (String)myTourRow["Rules"];

                myTourProperties = TourProperties.Instance;

                if (myTourRow["SlalomRounds"] == DBNull.Value) { myTourRow["SlalomRounds"] = 0; }
                if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
                if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }
                Int16 curEventRounds = 0;
                try {
                    curEventRounds = Convert.ToInt16( myTourRow["SlalomRounds"] );
                    if ( curEventRounds > 0 ) {
                        isSlalomActive = true;
                    } else {
                        isSlalomActive = false;
                    }
                } catch {
                    isSlalomActive = false;
                }
                try {
                    curEventRounds = Convert.ToInt16( myTourRow["TrickRounds"] );
                    if ( curEventRounds > 0 ) {
                        isTrickActive = true;
                    } else {
                        isTrickActive = false;
                    }
                } catch {
                    isTrickActive = false;
                }
                try {
                    curEventRounds = Convert.ToInt16( myTourRow["JumpRounds"] );
                    if ( curEventRounds > 0 ) {
                        isJumpActive = true;
                    } else {
                        isJumpActive = false;
                    }
                } catch {
                    isJumpActive = false;
                }

                StringBuilder outLine = new StringBuilder( "" );
                String curFilename = mySanctionNum.Trim() + "TS.prn";
                myOutBuffer = getExportFile( curFilename );
                if ( myOutBuffer != null ) {
                    Log.WriteFile( "Export tournament summary: " + curFilename );

                    writeHeader();
                    OfficialWorkUpdate curWorkRecordUpdate = new OfficialWorkUpdate();
                    curWorkRecordUpdate.updateOfficialWorkRecord();

                    readExportTourSkierCounts();
                    readExportEventSkierCounts();
                    readExportEventSkierGroupCounts();
                    
                    curDataTable = getOfficialWorkAsgmt();
                    readExportOfficialAsgmt(curDataTable);

                    if ( isSlalomActive ) {
                        curScoresDataTable = getSlalomScores();
                        curDetailDataTable = getSlalomDetail();
                        readExportSlalomData( curScoresDataTable, curDetailDataTable );
                    }
                    if ( isTrickActive ) {
                        curScoresDataTable = getTrickScores();
                        curDetailDataTable = getTrickDetail();
                        readExportTrickData( curScoresDataTable, curDetailDataTable );
                    }
                    if ( isJumpActive ) {
                        curScoresDataTable = getJumpScores();
                        curDetailDataTable = getJumpDetail();
                        readExportJumpData( curScoresDataTable, curDetailDataTable );
                    }

                    DataTable curTeamDataTable = getTeamList();
                    if ( curTeamDataTable.Rows.Count > 0 ) {
                        readExportTeamData();
                    }

                    writeFooter();

                    Log.WriteFile( "Export tournament summary complete: " + curFilename );

                    myOutBuffer.Close();
                    return true;
                } else {
                    return false;
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportTourSummary processing"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        //Write tournament information
        private bool writeHeader() {
            StringBuilder outLine = new StringBuilder( "" );

            try {
                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament summary as of " + DateTime.Now.ToString( "HH:MM:ss  MM-dd-yyyy" ));
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( mySanctionNum + myTourRow["Class"].ToString() + " " + myTourRow["Name"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Rules: " + myTourRow["Rules"].ToString() );
                outLine.Append( " Tournament Date: " + myTourRow["EventDates"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Location: " + myTourRow["EventLocation"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Events: Slalom: " + ((byte)myTourRow["SlalomRounds"]).ToString("#0") );
                outLine.Append( "    Trick: " + ( (byte)myTourRow["TrickRounds"] ).ToString( "#0" ) );
                outLine.Append( "    Jump: " + ( (byte)myTourRow["JumpRounds"] ).ToString( "#0" ) );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament Director: " + myTourRow["ContactName"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Chief Judge: " + myTourRow["ChiefJudgeName"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Chief Driver: " + myTourRow["ChiefDriverName"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportTourSummary writeHeader method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        //Write export file footer information
        private bool writeFooter() {
            StringBuilder outLine = new StringBuilder( "" );
            try {
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "******************* SCORED WITH "
                    + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion
                    + " ******************"
                    );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportTourSummary writeFooter method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool readExportTourSkierCounts() {
            StringBuilder outLine = new StringBuilder( "" );
            int curTourCount = 0;

            try {
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                DataTable curDataTable = getTourSkierCounts();
                if ( curDataTable.Rows.Count > 0 ) {
                    curTourCount = (int)curDataTable.Rows[0]["SkierCount"];
                }
                outLine = new StringBuilder( "" );
                outLine.Append( "Total participants " + curTourCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                curDataTable = getTourParticipantsNonSkiing();
                if ( curDataTable.Rows.Count > 0 ) {
                    curTourCount = (int)curDataTable.Rows[0]["SkierCount"];
                }
                outLine = new StringBuilder( "" );
                outLine.Append( "Total non skiing participants " + curTourCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportTourSummary readExportTourStats method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool readExportEventSkierCounts() {
            StringBuilder outLine = new StringBuilder( "" );
            String curMemberId = "", curAgeGroup = "", curEvent = "";
            String prevAgeGroup = null, prevEvent = null;
            int curDivCount = 0, curEventCount = 0, curTourCount = 0;

            try {
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );
                outLine.Append( "Event skier counts by division" );
                myOutBuffer.WriteLine( outLine.ToString() );

                DataTable curDataTable = getEventSkierCounts();
                outLine = new StringBuilder( "" );
                outLine.Append( "Event" );
                outLine.Append( " | AgeGroup" );
                outLine.Append( " | Count" );
                myOutBuffer.WriteLine( outLine.ToString() );

                foreach (DataRow curRow in curDataTable.Rows) {
                    try {
                        curEvent = curRow["Event"].ToString();
                        curAgeGroup = curRow["AgeGroup"].ToString();
                        curMemberId = curRow["MemberId"].ToString();
                        if (prevEvent == null) {
                            prevEvent = curEvent;
                            prevAgeGroup = curAgeGroup;
                        }
                    } catch (Exception ex) {
                        outLine.Append( "Exception encountered: " + ex.Message );
                    }

                    if (curEvent == prevEvent) {
                        if (curAgeGroup == prevAgeGroup) {
                            curDivCount++;
                        } else {
                            outLine = new StringBuilder( "" );
                            outLine.Append( prevEvent );
                            outLine.Append( " | " + prevAgeGroup );
                            outLine.Append( " | " + curDivCount.ToString() );
                            myOutBuffer.WriteLine( outLine.ToString() );

                            curEventCount += curDivCount;
                            curDivCount = 1;
                        }
                    } else {
                        outLine = new StringBuilder( "" );
                        outLine.Append( prevEvent );
                        outLine.Append( " | " + prevAgeGroup );
                        outLine.Append( " | " + curDivCount.ToString() );
                        myOutBuffer.WriteLine( outLine.ToString() );
                        curEventCount += curDivCount;

                        outLine = new StringBuilder( "" );
                        outLine.Append( prevEvent );
                        outLine.Append( " | Total" );
                        outLine.Append( " | " + curEventCount.ToString() );
                        myOutBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                        myOutBuffer.WriteLine( outLine.ToString() );

                        curTourCount += curEventCount;
                        curEventCount = 0;
                    }
                    prevEvent = curEvent;
                    prevAgeGroup = curAgeGroup;
                }

                curEventCount += curDivCount;
                curTourCount += curEventCount;

                outLine = new StringBuilder( "" );
                outLine.Append( prevEvent );
                outLine.Append( " | " + prevAgeGroup );
                outLine.Append( " | " + curDivCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( prevEvent );
                outLine.Append( " | Total" );
                outLine.Append( " | " + curEventCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament" );
                outLine.Append( " | Total " );
                outLine.Append( " | " + curTourCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportTourSummary readExportTourStats method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool readExportEventSkierGroupCounts() {
            StringBuilder outLine = new StringBuilder( "" );
            String curMemberId = "", curEventGroup = "", curEvent = "";
            String prevEventGroup = null, prevEvent = null;
            int curDivCount = 0, curEventCount = 0, curTourCount = 0;

            try {
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );
                outLine.Append( "Event skier counts by group" );
                myOutBuffer.WriteLine( outLine.ToString() );

                DataTable curDataTable = getEventSkierGroupCounts();
                outLine = new StringBuilder( "" );
                outLine.Append( "Event" );
                outLine.Append( " | EventGroup" );
                outLine.Append( " | Count" );
                myOutBuffer.WriteLine( outLine.ToString() );

                foreach (DataRow curRow in curDataTable.Rows) {
                    try {
                        curEvent = curRow["Event"].ToString();
                        curEventGroup = curRow["EventGroup"].ToString();
                        curMemberId = curRow["MemberId"].ToString();
                        if (prevEvent == null) {
                            prevEvent = curEvent;
                            prevEventGroup = curEventGroup;
                        }
                    } catch (Exception ex) {
                        outLine.Append( "Exception encountered: " + ex.Message );
                    }

                    if (curEvent == prevEvent) {
                        if (curEventGroup == prevEventGroup) {
                            curDivCount++;
                        } else {
                            outLine = new StringBuilder( "" );
                            outLine.Append( prevEvent );
                            outLine.Append( " | " + prevEventGroup );
                            outLine.Append( " | " + curDivCount.ToString() );
                            myOutBuffer.WriteLine( outLine.ToString() );

                            curEventCount += curDivCount;
                            curDivCount = 1;
                        }
                    } else {
                        outLine = new StringBuilder( "" );
                        outLine.Append( prevEvent );
                        outLine.Append( " | " + prevEventGroup );
                        outLine.Append( " | " + curDivCount.ToString() );
                        myOutBuffer.WriteLine( outLine.ToString() );
                        curEventCount += curDivCount;

                        outLine = new StringBuilder( "" );
                        outLine.Append( prevEvent );
                        outLine.Append( " | Total" );
                        outLine.Append( " | " + curEventCount.ToString() );
                        myOutBuffer.WriteLine( outLine.ToString() );
                        outLine = new StringBuilder( "" );
                        myOutBuffer.WriteLine( outLine.ToString() );

                        curTourCount += curEventCount;
                        curEventCount = 0;
                    }
                    prevEvent = curEvent;
                    prevEventGroup = curEventGroup;
                }

                curEventCount += curDivCount;
                curTourCount += curEventCount;

                outLine = new StringBuilder( "" );
                outLine.Append( prevEvent );
                outLine.Append( " | " + prevEventGroup );
                outLine.Append( " | " + curDivCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( prevEvent );
                outLine.Append( " | Total" );
                outLine.Append( " | " + curEventCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "Tournament" );
                outLine.Append( " | Total " );
                outLine.Append( " | " + curTourCount.ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch (Exception ex) {
                MessageBox.Show( "Exception encountered in ExportTourSummary readExportTourStats method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private bool readExportOfficialAsgmt(DataTable inDataTable) {
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = false;

            outLine = new StringBuilder( "" );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outLine.Append( "Official Assignments" );
            myOutBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( "Event");
            outLine.Append( "| EventGroup" );
            outLine.Append( "| Round" );
            outLine.Append( "| WorkAsgmt" );
            outLine.Append( "| StartTime" );
            outLine.Append( "| MemberName" );
            outLine.Append( "| Notes" );
            myOutBuffer.WriteLine( outLine.ToString() );
            
            foreach ( DataRow curRow in inDataTable.Rows ) {
                outLine = new StringBuilder( "" );
                outLine.Append( curRow["Event"].ToString() );
                outLine.Append( " | " + curRow["EventGroup"].ToString() );
                outLine.Append( " | " + curRow["Round"].ToString() );
                outLine.Append( " | " + curRow["WorkAsgmt"].ToString() );
                outLine.Append( " | " + curRow["StartTime"].ToString() );
                outLine.Append( " | " + curRow["MemberName"].ToString() );
                outLine.Append( " | " + curRow["Notes"].ToString() );
                myOutBuffer.WriteLine( outLine.ToString() );
            }

            returnStatus = true;
            if ( returnStatus ) {
                //MessageBox.Show( myDataTable.Rows.Count + " rows exported to officials credit report data file." );
            }

            return returnStatus;
        }

        private bool readExportSlalomData( DataTable inScoresDataTable, DataTable inDetailDataTable ) {
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = false;
            DataRow[] curFindRows;

            outLine = new StringBuilder( "" );
            myOutBuffer.WriteLine( outLine.ToString() );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine.Append( "Slalom Scores" );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outLine.Append( "AgeGroup" );
            outLine.Append( " SkierName" );
            outLine.Append( " | Class" );
            outLine.Append( " | Round" );
            outLine.Append( " | Score" );
            outLine.Append( " | Points" );
            outLine.Append( " | Start Speed" );
            outLine.Append( " | Start Line" );
            outLine.Append( " | Mph" );
            outLine.Append( " | Line" );
            outLine.Append( " | Kph" );
            outLine.Append( " | Line" );
            outLine.Append( " | Pass Score" );
            outLine.Append( " | Status" );
            outLine.Append( " | Notes" );
            myOutBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( " SkierRunNum" );
            outLine.Append( " | Note" );
            outLine.Append( " | Score" );
            outLine.Append( " | Judge1Score" );
            outLine.Append( " | Judge2Score" );
            outLine.Append( " | Judge3Score" );
            outLine.Append( " | Judge4Score" );
            outLine.Append( " | Judge5Score" );
            outLine.Append( " | BoatTime" );
            outLine.Append( " | TimeInTol" );
            outLine.Append( " | Reride" );
            outLine.Append( " | ScoreProt" );
            outLine.Append( " | RerideReason" );
            outLine.Append( " | EntryGate1" );
            outLine.Append( " | EntryGate2" );
            outLine.Append( " | EntryGate3" );
            outLine.Append( " | ExitGate1" );
            outLine.Append( " | ExitGate2" );
            outLine.Append( " | ExitGate3" );
            myOutBuffer.WriteLine( outLine.ToString() );


            foreach ( DataRow curRow in inScoresDataTable.Rows ) {
                outLine = new StringBuilder( "" );
                try {
                    outLine.Append( curRow["AgeGroup"].ToString() );
                    outLine.Append( " " + curRow["SkierName"].ToString() );
                    outLine.Append( " | " + curRow["EventClass"].ToString() );
                    outLine.Append( " | " + curRow["Round"].ToString() );
                    outLine.Append( " | " + ( (Decimal)curRow["Score"] ).ToString( "##.00" ) );
                    outLine.Append( " | " + ( (Decimal)curRow["NopsScore"] ).ToString() );
                    outLine.Append( " | " + curRow["StartSpeed"].ToString() );
                    outLine.Append( " | " + curRow["StartLen"].ToString() );
                    outLine.Append( " | " + curRow["FinalSpeedMph"].ToString() );
                    outLine.Append( " | " + curRow["FinalLenOff"].ToString() );
                    outLine.Append( " | " + curRow["FinalSpeedKph"].ToString() );
                    outLine.Append( " | " + curRow["FinalLen"].ToString() );
                    outLine.Append( " | " + ( (Decimal)curRow["FinalPassScore"] ).ToString() );
                    outLine.Append( " | " + curRow["Status"].ToString() );
                    outLine.Append( " | " + curRow["Note"].ToString() );
                } catch ( Exception ex ) {
                    outLine.Append( "Exception encountered: " + ex.Message );
                }
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                curFindRows = inDetailDataTable.Select( "MemberId = '" + curRow["MemberId"].ToString() + "' AND AgeGroup = '" + curRow["AgeGroup"].ToString() + "' AND Round = " + curRow["Round"].ToString() );
                if ( curFindRows.Length > 0 ) {
                    foreach ( DataRow curRowDetail in curFindRows ) {
                        #region write detail records
                        try {
                            outLine = new StringBuilder( "" );
                            outLine.Append( " | " + curRowDetail["SkierRunNum"].ToString() );
                            outLine.Append( " | " + curRowDetail["Note"].ToString() );
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Judge1Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Judge2Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Judge3Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Judge4Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["Judge5Score"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            outLine.Append( " | " + ( (Decimal)curRowDetail["BoatTime"] ).ToString( "##.00" ) );
                            outLine.Append( " | " + curRowDetail["TimeInTol"].ToString() );
                            outLine.Append( " | " + curRowDetail["Reride"].ToString() );
                            outLine.Append( " | " + curRowDetail["ScoreProt"].ToString() );
                            outLine.Append( " | " + curRowDetail["RerideReason"].ToString() );
                            if ( (bool)curRowDetail["EntryGate1"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                            if ( (bool)curRowDetail["EntryGate2"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                            if ( (bool)curRowDetail["EntryGate3"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                            if ( (bool)curRowDetail["ExitGate1"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                            if ( (bool)curRowDetail["ExitGate2"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                            if ( (bool)curRowDetail["ExitGate3"] ) {
                                outLine.Append( " | Y" );
                            } else {
                                outLine.Append( " | N" );
                            }
                        } catch ( Exception ex ) {
                            outLine.Append( "Exception encountered: " + ex.Message );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                        #endregion
                    }
                }
            }

            returnStatus = true;
            if ( returnStatus ) {
                //MessageBox.Show( myDataTable.Rows.Count + " rows exported to officials credit report data file." );
            }

            return returnStatus;
        }

        private bool readExportTrickData( DataTable inScoresDataTable, DataTable inDetailDataTable ) {
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = false;
            byte curPassNum = 0, prevPassNum = 0;
            DataRow[] curFindRows;

            outLine = new StringBuilder( "" );
            myOutBuffer.WriteLine( outLine.ToString() );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine.Append( "Trick Scores" );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outLine.Append( "AgeGroup" );
            outLine.Append( " SkierName" );
            outLine.Append( " | Class" );
            outLine.Append( " | Round" );
            outLine.Append( " | Score" );
            outLine.Append( " | Points" );
            outLine.Append( " | ScorePass1" );
            outLine.Append( " | ScorePass2" );
            outLine.Append( " | Status" );
            outLine.Append( " | Notes" );
            myOutBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( " PassNum" );
            outLine.Append( " | Skis" );
            outLine.Append( " | Seq" );
            outLine.Append( " | Code" );
            outLine.Append( " | Results" );
            outLine.Append( " | Score" );
            outLine.Append( " | Note" );
            myOutBuffer.WriteLine( outLine.ToString() );

            foreach ( DataRow curRow in inScoresDataTable.Rows ) {
                outLine = new StringBuilder( "" );
                try {
                    outLine.Append( curRow["AgeGroup"].ToString() );
                    outLine.Append( " " + curRow["SkierName"].ToString() );
                    outLine.Append( " | " + curRow["EventClass"].ToString() );
                    outLine.Append( " | " + curRow["Round"].ToString() );
                    outLine.Append( " | " + ( (Int16)curRow["Score"] ).ToString( "##,##0" ) );
                    outLine.Append( " | " + ( (Decimal)curRow["NopsScore"] ).ToString("##,##0.0") );
                    outLine.Append( " | " + ( (Int16)curRow["ScorePass1"] ).ToString( "##,##0" ) );
                    outLine.Append( " | " + ( (Int16)curRow["ScorePass2"] ).ToString( "##,##0" ) );
                    outLine.Append( " | " + curRow["Status"].ToString() );
                    outLine.Append( " | " + curRow["Note"].ToString() );
                } catch ( Exception ex ) {
                    outLine.Append( "Exception encountered: " + ex.Message );
                }
                myOutBuffer.WriteLine( outLine.ToString() );
                curPassNum = 0;
                prevPassNum = 0;

                outLine = new StringBuilder( "" );
                curFindRows = inDetailDataTable.Select( "MemberId = '" + curRow["MemberId"].ToString() + "' AND AgeGroup = '" + curRow["AgeGroup"].ToString() + "' AND Round = " + curRow["Round"].ToString() );
                if ( curFindRows.Length > 0 ) {
                    foreach ( DataRow curRowDetail in curFindRows ) {
                        #region write detail records
                        try {
                            outLine = new StringBuilder( "" );
                            curPassNum = (byte)curRowDetail["PassNum"];
                            if ( curPassNum != prevPassNum ) {
                                myOutBuffer.WriteLine( outLine.ToString() );
                            }
                            outLine.Append( " | " + curRowDetail["PassNum"].ToString() );
                            outLine.Append( " | " + curRowDetail["Skis"].ToString() );
                            outLine.Append( " | " + curRowDetail["Seq"].ToString().PadLeft(2, ' ') );
                            outLine.Append( " | " + curRowDetail["Code"].ToString().PadLeft( 8, ' ' ) );
                            outLine.Append( " | " + curRowDetail["Results"].ToString().PadLeft( 10, ' ' ) );
                            try {
                                outLine.Append( " | " + ( (Int16)curRowDetail["Score"] ).ToString( "####" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            outLine.Append( " | " + curRowDetail["Note"].ToString() );
                        } catch ( Exception ex ) {
                            outLine.Append( "Exception encountered: " + ex.Message );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                        prevPassNum = curPassNum;
                        #endregion
                    }
                }
                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );
            }

            returnStatus = true;
            if ( returnStatus ) {
                //MessageBox.Show( myDataTable.Rows.Count + " rows exported to officials credit report data file." );
            }

            return returnStatus;
        }

        private bool readExportJumpData( DataTable inScoresDataTable, DataTable inDetailDataTable ) {
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = false;
            DataRow[] curFindRows;

            outLine = new StringBuilder( "" );
            myOutBuffer.WriteLine( outLine.ToString() );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine.Append( "Jump Scores" );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outLine.Append( "AgeGroup" );
            outLine.Append( " SkierName" );
            outLine.Append( " | Class" );
            outLine.Append( " | Round" );
            outLine.Append( " | Feet" );
            outLine.Append( " | Meters" );
            outLine.Append( " | Points" );
            outLine.Append( " | Speed" );
            outLine.Append( " | Ramp" );
            outLine.Append( " | Status" );
            outLine.Append( " | Notes" );
            myOutBuffer.WriteLine( outLine.ToString() );

            outLine = new StringBuilder( "" );
            outLine.Append( " PassNum" );
            outLine.Append( " | BoatSpeed" );
            outLine.Append( " | RampHeight" );
            outLine.Append( " | Results" );
            outLine.Append( " | BoatSplitTime" );
            outLine.Append( " | BoatSplitTime2" );
            outLine.Append( " | BoatEndTime" );
            outLine.Append( " | ScoreFeet" );
            outLine.Append( " | ScoreMeters" );
            outLine.Append( " | ReturnToBase" );
            outLine.Append( " | TimeInTol" );
            outLine.Append( " | ScoreProt" );
            outLine.Append( " | Reride" );
            outLine.Append( " | RerideReason" );
            outLine.Append( " | Note" );
            myOutBuffer.WriteLine( outLine.ToString() );

            foreach ( DataRow curRow in inScoresDataTable.Rows ) {
                outLine = new StringBuilder( "" );
                try {
                    //ER.AgeGroup, TR.SkierName, ER.EventClass, S.Round
                    //S.ScoreFeet, S.ScoreMeters, S.NopsScore, S.BoatSpeed, S.RampHeight
                    outLine.Append( curRow["AgeGroup"].ToString() );
                    outLine.Append( " " + curRow["SkierName"].ToString() );
                    outLine.Append( " | " + curRow["EventClass"].ToString() );
                    outLine.Append( " | " + curRow["Round"].ToString() );
                    outLine.Append( " | " + ( (Decimal)curRow["ScoreFeet"] ).ToString( "##0" ) );
                    outLine.Append( " | " + ( (Decimal)curRow["ScoreMeters"] ).ToString( "##0.0" ) );
                    outLine.Append( " | " + ( (Decimal)curRow["NopsScore"] ).ToString( "##,##0.0" ) );
                    outLine.Append( " | " + curRow["BoatSpeed"].ToString() );
                    outLine.Append( " | " + curRow["RampHeight"].ToString() );
                    outLine.Append( " | " + curRow["Status"].ToString() );
                    outLine.Append( " | " + curRow["Note"].ToString() );
                } catch ( Exception ex ) {
                    outLine.Append( "Exception encountered: " + ex.Message );
                }
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                curFindRows = inDetailDataTable.Select( "MemberId = '" + curRow["MemberId"].ToString() + "' AND AgeGroup = '" + curRow["AgeGroup"].ToString() + "' AND Round = " + curRow["Round"].ToString() );
                if ( curFindRows.Length > 0 ) {
                    foreach ( DataRow curRowDetail in curFindRows ) {
                        #region write detail records
                        try {
                            //PassNum, BoatSpeed, RampHeight
                            //, Results, BoatSplitTime, BoatSplitTime2, BoatEndTime, ScoreFeet, ScoreMeters
                            outLine = new StringBuilder( "" );
                            outLine.Append( " | " + curRowDetail["PassNum"].ToString() );
                            outLine.Append( " | " + curRowDetail["BoatSpeed"].ToString() );
                            outLine.Append( " | " + curRowDetail["RampHeight"].ToString() );
                            outLine.Append( " | " + curRowDetail["Results"].ToString() );
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["BoatSplitTime"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["BoatSplitTime2"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["BoatEndTime"] ).ToString( "##.00" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["ScoreFeet"] ).ToString( "##0" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            try {
                                outLine.Append( " | " + ( (Decimal)curRowDetail["ScoreMeters"] ).ToString( "##0.0" ) );
                            } catch {
                                outLine.Append( " | " );
                            }
                            outLine.Append( " | " + curRowDetail["ReturnToBase"].ToString() );
                            outLine.Append( " | " + curRowDetail["TimeInTol"].ToString() );
                            outLine.Append( " | " + curRowDetail["ScoreProt"].ToString() );
                            outLine.Append( " | " + curRowDetail["Reride"].ToString() );
                            outLine.Append( " | " + curRowDetail["RerideReason"].ToString() );
                            outLine.Append( " | " + curRowDetail["Note"].ToString() );
                        } catch ( Exception ex ) {
                            outLine.Append( "Exception encountered: " + ex.Message );
                        }
                        myOutBuffer.WriteLine( outLine.ToString() );
                        #endregion
                    }
                    outLine = new StringBuilder( "" );
                    myOutBuffer.WriteLine( outLine.ToString() );
                }
            }

            returnStatus = true;
            if ( returnStatus ) {
                //MessageBox.Show( myDataTable.Rows.Count + " rows exported to officials credit report data file." );
            }

            return returnStatus;
        }

        private bool readExportTeamData () {
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = false;

            Int16 curNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam);
            String curDataType = "best";
            String curPointsMethod = "nops";
            String curPlcmtMethod = "points";
            String curPlcmtOrg = "div";
            CalcScoreSummary curCalcSummary = new CalcScoreSummary();

            if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
                curPointsMethod = "kbase";
            } else if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                curPointsMethod = "plcmt";
            }
            DataTable curSlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team" );
            DataTable curTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team" );
            DataTable curJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team" );

            DataTable curTeamDataTable = curCalcSummary.getSlalomSummaryTeam( curSlalomDataTable, myTourRow, curNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
            curTeamDataTable = curCalcSummary.getTrickSummaryTeam( curTeamDataTable, curTrickDataTable, myTourRow, curNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
            curTeamDataTable = curCalcSummary.getJumpSummaryTeam( curTeamDataTable, curJumpDataTable, myTourRow, curNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

            curTeamDataTable.DefaultView.Sort = "Div ASC, TeamScoreTotal DESC";
            curTeamDataTable = curTeamDataTable.DefaultView.ToTable();

            outLine = new StringBuilder( "" );
            myOutBuffer.WriteLine( outLine.ToString() );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine.Append( "Team Scores" );
            myOutBuffer.WriteLine( outLine.ToString() );
            outLine = new StringBuilder( "" );
            outLine.Append( "Div" );
            outLine.Append( " | Team" );
            outLine.Append( " | Slalom" );
            outLine.Append( " | Trick" );
            outLine.Append( " | Jump" );
            outLine.Append( " | Total" );
            myOutBuffer.WriteLine( outLine.ToString() );

            foreach ( DataRow curRow in curTeamDataTable.Rows ) {
                outLine = new StringBuilder( "" );
                try {
                    try {
                        outLine.Append( curRow["Div"].ToString() );
                    } catch {
                        outLine.Append( " " );
                    }
                    try {
                        outLine.Append( " | " + (String)curRow["TeamCode"]);
                    } catch {
                        outLine.Append( " " );
                    }
                    try {
                        outLine.Append( "-" + (String)curRow["Name"] );
                    } catch {
                        outLine.Append( " " );
                    }
                    try {
                        outLine.Append( " | " + ((Decimal)curRow["TeamScoreSlalom"] ).ToString( "##,##0.00" ) );
                    } catch {
                        outLine.Append( " | " );
                    }
                    try {
                        outLine.Append( " | " + ( (Decimal)curRow["TeamScoreTrick"] ).ToString( "##,##0.00" ) );
                    } catch {
                        outLine.Append( " | " );
                    }
                    try {
                        outLine.Append( " | " + ( (Decimal)curRow["TeamScoreJump"] ).ToString( "##,##0.00" ) );
                    } catch {
                        outLine.Append( " | " );
                    }
                    try {
                        outLine.Append( " | " + ( (Decimal)curRow["TeamScoreTotal"] ).ToString( "##,##0.00" ) );
                    } catch {
                        outLine.Append( " | " );
                    }
                } catch ( Exception ex ) {
                    outLine.Append( "Exception encountered: " + ex.Message );
                }
                myOutBuffer.WriteLine( outLine.ToString() );
                returnStatus = true;
            }

            return returnStatus;
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

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactMemberId, ContactPhone, ContactEmail, CP.SkierName AS ContactName " );
            curSqlStmt.Append( ", ChiefJudgeMemberId, CJ.SkierName AS ChiefJudgeName" );
            curSqlStmt.Append( ", ChiefDriverMemberId, CD.SkierName AS ChiefDriverName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CP " );
            curSqlStmt.Append( "    ON CP.SanctionId = T.SanctionId AND CP.MemberId = T.ContactMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CJ " );
            curSqlStmt.Append( "    ON CJ.SanctionId = T.SanctionId AND CJ.MemberId = T.ChiefJudgeMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CD " );
            curSqlStmt.Append( "    ON CD.SanctionId = T.SanctionId AND CD.MemberId = T.ChiefDriverMemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );


            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getOfficialWorkAsgmt() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT O.PK, O.SanctionId, O.MemberId, O.Event, O.EventGroup, O.Round, O.WorkAsgmt, " );
            curSqlStmt.Append( "O.StartTime, O.EndTime, O.Notes, T.SkierName AS MemberName " );
            curSqlStmt.Append( "FROM OfficialWorkAsgmt O " );
            curSqlStmt.Append( "     INNER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) T " );
            curSqlStmt.Append( "        ON O.MemberId = T.MemberId AND O.SanctionId = T.SanctionId " );
            curSqlStmt.Append( "     INNER JOIN CodeValueList AS L ON L.ListName = 'OfficialAsgmt' AND L.CodeValue = O.WorkAsgmt " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "ORDER BY O.Event, O.Round, O.EventGroup, O.StartTime, O.WorkAsgmt, T.SkierName" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSlalomScores() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.SanctionId, S.MemberId, TR.SkierName, ER.Event, ER.EventGroup, ER.TeamCode" );
            curSqlStmt.Append( ", ER.EventClass, ER.AgeGroup, S.Round, S.Score, S.NopsScore, S.Rating " );
            curSqlStmt.Append( ", MaxSpeed, StartSpeed, StartLen, Status, '' as BoatCode" );
            curSqlStmt.Append( ", FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore, Note " );
            curSqlStmt.Append( "FROM SlalomScore S " );
            curSqlStmt.Append( "  INNER JOIN TourReg TR ON S.SanctionId = TR.SanctionId AND S.MemberId = TR.MemberId AND S.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON S.SanctionId = ER.SanctionId AND S.MemberId = ER.MemberId AND S.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Slalom' " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, ER.AgeGroup, TR.SkierName, S.MemberId, S.Round" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSlalomDetail() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round, SkierRunNum" );
            curSqlStmt.Append( ", Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score" );
            curSqlStmt.Append( ", EntryGate1, EntryGate2, EntryGate3" );
            curSqlStmt.Append( ", ExitGate1, ExitGate2, ExitGate3" );
            curSqlStmt.Append( ", BoatTime, Score, PassLineLength, PassSpeedKph" );
            curSqlStmt.Append( ", Reride, RerideReason, TimeInTol, ScoreProt, Note" );
            curSqlStmt.Append( " FROM SlalomRecap " );
            curSqlStmt.Append( " WHERE SanctionId = '" + mySanctionNum + "'" );
            curSqlStmt.Append( " ORDER BY SanctionId, AgeGroup, MemberId, Round, SkierRunNum" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTrickScores() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.SanctionId, S.MemberId, TR.SkierName, ER.Event, ER.EventGroup, ER.TeamCode" );
            curSqlStmt.Append( ", ER.EventClass, ER.AgeGroup, S.Round, S.Rating, '' as BoatCode" );
            curSqlStmt.Append( ", S.Score, S.NopsScore, S.ScorePass1, S.ScorePass2, Status, Note " );
            curSqlStmt.Append( "FROM TrickScore AS S " );
            curSqlStmt.Append( "  INNER JOIN TourReg TR ON S.SanctionId = TR.SanctionId AND S.MemberId = TR.MemberId AND S.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON S.SanctionId = ER.SanctionId AND S.MemberId = ER.MemberId AND S.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Trick' " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, ER.AgeGroup, TR.SkierName, S.MemberId, S.Round" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTrickDetail() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round, PassNum" );
            curSqlStmt.Append( ", Seq, Skis, Code, Results, Score, Note" );
            curSqlStmt.Append( " FROM TrickPass E " );
            curSqlStmt.Append( " WHERE SanctionId = '" + mySanctionNum + "'" );
            curSqlStmt.Append( " ORDER BY SanctionId, AgeGroup, MemberId, Round, PassNum, Seq" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getJumpScores() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT S.MemberId, S.SanctionId, TR.SkierName, ER.Event, S.BoatSpeed, S.RampHeight" );
            curSqlStmt.Append( ", ER.AgeGroup, S.Round, S.ScoreFeet, S.ScoreMeters, S.NopsScore, S.Rating" );
            curSqlStmt.Append( ", ER.TeamCode, ER.EventClass, boat as BoatCode, Status, Note " );
            curSqlStmt.Append( "FROM JumpScore AS S " );
            curSqlStmt.Append( "  INNER JOIN TourReg TR ON  S.SanctionId = TR.SanctionId AND S.MemberId = TR.MemberId AND S.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN EventReg ER ON S.SanctionId = ER.SanctionId AND S.MemberId = ER.MemberId AND S.AgeGroup = ER.AgeGroup " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, ER.AgeGroup, TR.SkierName, S.MemberId, S.Round" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getJumpDetail() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round, PassNum" );
            curSqlStmt.Append( ", BoatSpeed, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6" );
            curSqlStmt.Append( ", ScoreFeet, ScoreMeters, ScoreTriangle, Results" );
            curSqlStmt.Append( ", BoatSplitTime, BoatEndTime, BoatSplitTime2" );
            curSqlStmt.Append( ", ReturnToBase, ScoreProt, TimeInTol, Reride, RerideReason, Note " );
            curSqlStmt.Append( "FROM JumpRecap E " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "ORDER BY SanctionId, AgeGroup, MemberId, Round, PassNum" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTeamList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT * From TeamList " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTourParticipants() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND ( EXISTS (SELECT 1 AS Expr1 FROM SlalomScore AS SS" );
            curSqlStmt.Append( "        WHERE (SanctionId = TR.SanctionId) AND (MemberId = TR.MemberId) AND (AgeGroup = TR.AgeGroup))" );
            curSqlStmt.Append( "     OR EXISTS (SELECT 1 AS Expr1 FROM TrickScore AS SS" );
            curSqlStmt.Append( "        WHERE (SanctionId = TR.SanctionId) AND (MemberId = TR.MemberId) AND (AgeGroup = TR.AgeGroup))" );
            curSqlStmt.Append( "     OR EXISTS (SELECT 1 AS Expr1 FROM JumpScore AS SS" );
            curSqlStmt.Append( "        WHERE (SanctionId = TR.SanctionId) AND (MemberId = TR.MemberId) AND (AgeGroup = TR.AgeGroup))" );
            curSqlStmt.Append( "     ) " );
            curSqlStmt.Append( "Order by AgeGroup, SkierName" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTourParticipantsNonSkiing() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Count(*) as SkierCount " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND NOT EXISTS (SELECT 1 AS Expr1 FROM EventReg AS ER" );
            curSqlStmt.Append( "      WHERE (SanctionId = TR.SanctionId) AND (MemberId = TR.MemberId) AND (AgeGroup = TR.AgeGroup))" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getTourSkierCounts() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT count(*) as SkierCount " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "AND ( ");
            curSqlStmt.Append( "  EXISTS (SELECT 1 AS Expr1 FROM SlalomScore AS SS " );
            curSqlStmt.Append( "     WHERE SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup) " );
            curSqlStmt.Append( "  OR EXISTS (SELECT 1 AS Expr1 FROM TrickScore AS SS " );
            curSqlStmt.Append( "     WHERE SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup) " );
            curSqlStmt.Append( "  OR EXISTS (SELECT 1 AS Expr1 FROM JumpScore AS SS " );
            curSqlStmt.Append( "     WHERE SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup) " );
            curSqlStmt.Append( ") " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getEventSkierCounts() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, TR.AgeGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN SlalomScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Slalom' " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, TR.AgeGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN TrickScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Trick' " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, TR.AgeGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN JumpScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "Order BY TR.SanctionId, ER.Event, TR.AgeGroup, TR.SkierName " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getEventSkierGroupCounts() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, ER.EventGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN SlalomScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Slalom' " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, ER.EventGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN TrickScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Trick' " );
            curSqlStmt.Append( "UNION " );
            curSqlStmt.Append( "SELECT DISTINCT TR.SanctionId, ER.Event, TR.MemberId, ER.EventGroup, TR.SkierName " );
            curSqlStmt.Append( "FROM TourReg AS TR " );
            curSqlStmt.Append( "  INNER JOIN EventReg AS ER ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup  = TR.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN JumpScore AS SS ON SS.SanctionId = TR.SanctionId AND SS.MemberId = TR.MemberId AND SS.AgeGroup = TR.AgeGroup " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND ER.Event = 'Jump' " );
            curSqlStmt.Append( "Order BY TR.SanctionId, ER.Event, ER.EventGroup, TR.SkierName " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

    }
}
