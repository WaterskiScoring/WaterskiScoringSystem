using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportBoatUse {
        private int myTourRounds;
        private String mySanctionNum = null;
        private String myTourRules;
        private String myTourClass;
        private DataRow myTourRow;
        private ProgressWindow myProgressInfo;

        public ExportBoatUse() {
            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if (mySanctionNum == null) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                    DataTable curTourDataTable = getTourData( mySanctionNum );
                    if (curTourDataTable.Rows.Count > 0) {
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
                        if (curSlalomRounds > myTourRounds) { myTourRounds = curSlalomRounds; }
                        if (curTrickRounds > myTourRounds) { myTourRounds = curTrickRounds; }
                        if (curJumpRounds > myTourRounds) { myTourRounds = curJumpRounds; }
                        if (myTourRounds == 1) { myTourRounds = 2; }
                    }
                }
            }
        }

        public Boolean ExportData() {
            String curMethodName = "ExportData";
            StringBuilder outLine = new StringBuilder( "" );
            bool returnStatus = true;
            int curRowCount = 0, curEntriesWritten = 0 ;
            String curMsg = "";
            String tabDelim = "\t";
            String curValue = "";
            String[] curValueSplit = null;
            DataRow[] curFindRows = null;

            try {
                curMsg = "Exporting boat use data";
                Log.WriteFile( curMethodName + ":begin export" );

                DataTable curAprvManuDataTable = getApprovedManufacturers();
                DataTable curDataTable = getTourBoatUseData();

                String curDateString = DateTime.Now.ToString( "yyyyMMdd HH:MM" );
                String curDateOut = curDateString.Substring( 0, 8 );
                String curTimeOut = curDateString.Substring( 9, 5 );
                String curEventDateOut = (String)myTourRow["EventDates"];
                try {
                    DateTime curEventDate = Convert.ToDateTime( curEventDateOut );
                    curEventDateOut = curEventDate.ToString( "MM/dd/yyyy" );
                } catch {
                    MessageBox.Show( "The event date of " + curEventDateOut + " is not a valid date and must corrected" );
                    curEventDateOut = myTourRow["EventDates"].ToString();
                }

                String curState = "", curCity = "", curSiteName = "";
                try {
                    String[] curEventLocation = ((String)myTourRow["EventLocation"]).Split( ',' );
                    if (curEventLocation.Length == 3) {
                        curSiteName = curEventLocation[0];
                        curCity = curEventLocation[1];
                        curState = curEventLocation[2];
                        if ( curState.Length != 2 ) {
							MessageBox.Show( "State value must the valid 2 character abbreviation"
								+ "\nPlease enter this information on the tournament window in the following format:\n"
								+ "\nSite Name followed by a comma, then the city, followed by a comma, then the 2 character state abbreviation"
								);
						}
					} else {
                        MessageBox.Show( "An event location is required."
                            + "\nPlease enter this information on the tournament window in the following format:\n"
                            + "\nSite Name followed by a comma, then the city, followed by a comma, then the 2 character state abbreviation"
                            );
                    }
                } catch {
                    MessageBox.Show( "An event location is required."
                        + "\nPlease enter this information on the tournament window in the following format:\n"
                        + "\nSite Name followed by a comma, then the city, followed by a comma, then the 2 character state abbreviation"
                        );
                }

                Cursor.Current = Cursors.WaitCursor;
                StreamWriter outBuffer = null;

                String curFilename = mySanctionNum + "TU" + ".txt";
                String curReportTitle = "Boat Use data for " + mySanctionNum + " - " + (String)myTourRow["Name"];
                outBuffer = getExportFile( curFilename );


                if (outBuffer == null) {
                    curMsg = "Output file not available";
                } else {
                    Log.WriteFile( "Export boat use data file begin: " + curFilename );
                    myProgressInfo = new ProgressWindow();
                    myProgressInfo.setProgessMsg( "Processing boat use data" );
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();
                    myProgressInfo.setProgressMax( curDataTable.Rows.Count );

                    //Write file header
                    outLine = new StringBuilder( "" );
                    outLine.Append( "SkiYear" + tabDelim );
                    outLine.Append( "SanctionNum" + tabDelim );
                    outLine.Append( "Regions" + tabDelim );
                    outLine.Append( "Tournament Name" + tabDelim );
                    outLine.Append( "Class" + tabDelim );
                    outLine.Append( "City" + tabDelim );
                    outLine.Append( "State" + tabDelim );
                    outLine.Append( "Start Date" + tabDelim );
                    outLine.Append( "Division" + tabDelim );

                    outLine.Append( "Manufacturers" + tabDelim );
                    outLine.Append( "Boat Model" + tabDelim );
                    outLine.Append( "Year" + tabDelim );
                    outLine.Append( "SL" + tabDelim );
                    outLine.Append( "TR" + tabDelim );
                    outLine.Append( "JU" + tabDelim );
                    outLine.Append( "Credit" + tabDelim );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Write available data
                    if (curDataTable.Rows.Count > 0) {
                        String curBoatCode = "", prevBoatCode = "";
                        String curSlalomCredit = "", curTrickCredit = "", curJumpCredit = "", curCredit = "";
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curRowCount++;
                            if ( isObjectEmpty(curRow["HullId"])) {
                                MessageBox.Show( "Towboat data must be updated.  Please open the Boat Use Report and force a data save.  Then you may regenerate this report." );
                            } else {
                                curBoatCode = (String)curRow["HullId"];
                            }

                            if (curBoatCode != prevBoatCode) {
                                if (curRowCount > 1) {
                                    outLine.Append( curSlalomCredit + tabDelim + curTrickCredit + tabDelim + curJumpCredit + tabDelim + curCredit );
                                    outBuffer.WriteLine( outLine.ToString() );
                                    curEntriesWritten++;
                                }

                                curSlalomCredit = "FALSE";
                                curTrickCredit = "FALSE";
                                curJumpCredit = "FALSE";
                                curCredit = "FALSE";

                                outLine = new StringBuilder( "" );
                                outLine.Append( "20" + mySanctionNum.Substring( 0, 2 ) + tabDelim );
                                outLine.Append( "\"" + mySanctionNum + "\"" + tabDelim );
                                curValue = mySanctionNum.Substring( 2, 1 );
                                if (curValue.ToUpper().Equals( "E" )) {
                                    outLine.Append( "EAST" + tabDelim );
                                } else if (curValue.ToUpper().Equals( "W" )) {
                                    outLine.Append( "WEST" + tabDelim );
                                } else if (curValue.ToUpper().Equals( "S" )) {
                                    outLine.Append( "SOUTH" + tabDelim );
                                } else if (curValue.ToUpper().Equals( "C" )) {
                                    outLine.Append( "SOUTHCENTRAL" + tabDelim );
                                } else if (curValue.ToUpper().Equals( "M" )) {
                                    outLine.Append( "MIDWEST" + tabDelim );
                                } else {
                                    outLine.Append( curValue + tabDelim );
                                }
                                outLine.Append( (String)myTourRow["Name"] + tabDelim );
                                outLine.Append( (String)myTourRow["Class"] + tabDelim );
                                outLine.Append( curCity + tabDelim );
                                outLine.Append( curState + tabDelim );
                                outLine.Append( curEventDateOut + tabDelim );
                                outLine.Append( ((String)myTourRow["Rules"]).ToUpper() + tabDelim );

                                curValue = curRow["HullId"].ToString();
                                curFindRows = curAprvManuDataTable.Select( "ListCode = '" + curValue.Substring( 0, 2 ).ToUpper() + "'" );
                                if (curFindRows.Length > 0) {
                                    curValue = (String)curFindRows[0]["CodeValue"];
                                } else {
                                    curValue = curRow["BoatModel"].ToString();
                                    if (curValue.Length > 0) {
                                        curValueSplit = curValue.Split( ' ' );
                                        if (curValueSplit.Length > 0) {
                                            curValue = curValueSplit[0];
                                            if (curValue.ToLower().Equals( "correct" )) {
                                                curValue = "Correct Craft";
                                            } else if (curValue.ToLower().Equals( "master" )) {
                                                curValue = "MasterCraft";
                                            }
                                        } else {
                                            curValue = "Unlisted";
                                        }
                                    } else {
                                        curValue = "Unlisted";
                                    }
                                }
                                outLine.Append( curValue + tabDelim );
                                outLine.Append( curRow["BoatModel"].ToString() + tabDelim );
                                outLine.Append( curRow["ModelYear"].ToString() + tabDelim );
                            }

                            curValue = curRow["slalomCredit"].ToString();
                            if (curValue.Equals( "Y" )) {
                                curSlalomCredit = "TRUE";
                                curCredit = "TRUE";
                            }
                            curValue = curRow["trickCredit"].ToString();
                            if (curValue.Equals( "Y" )) {
                                curTrickCredit = "TRUE";
                                curCredit = "TRUE";
                            }
                            curValue = curRow["jumpCredit"].ToString();
                            if (curValue.Equals( "Y" )) {
                                curJumpCredit = "TRUE";
                                curCredit = "TRUE";
                            }
                            prevBoatCode = curBoatCode;
                            myProgressInfo.setProgressValue( curRowCount );
                            myProgressInfo.Refresh();
                        }
                        if (curRowCount > 0) {
                            outLine.Append( curSlalomCredit + tabDelim + curTrickCredit + tabDelim + curJumpCredit + tabDelim + curCredit );
                            outBuffer.WriteLine( outLine.ToString() );
                            curEntriesWritten++;
                        }
                    }

                    outBuffer.Close();
                    myProgressInfo.Close();
                    if (curDataTable.Rows.Count > 0) {
                        curMsg = curDataTable.Rows.Count + " entries found, " + curEntriesWritten.ToString() + " unique entries selected and written";
                    } else {
                        curMsg = "No rows found";
                    }
                }
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + ":conplete: " + curMsg );
            } catch (Exception ex) {
                MessageBox.Show( "Error:" + curMethodName + " Could not write to file\n\nError: " + ex.Message );
                curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                returnStatus = false;
            }

            return returnStatus;
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

        private DataTable getTourBoatUseData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, TourBoatSeq , HullId, BoatModel, ModelYear, Owner" );
            curSqlStmt.Append( ", SlalomCredit, SlalomUsed, TrickCredit, TrickUsed, JumpCredit, JumpUsed" );
            curSqlStmt.Append( ", SpeedControlVersion, CertOfInsurance, InsuranceCompany, Notes, PostEventNotes, PreEventNotes " );
            curSqlStmt.Append( "FROM TourBoatUse " );
            curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') " );
            curSqlStmt.Append( "ORDER BY SanctionId, HullId, TourBoatSeq, BoatModel, ModelYear" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getApprovedManufacturers() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, SortSeq" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'ApprovedManufacturers'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private StreamWriter getExportFile(String inFileName) {
            StreamWriter outBuffer = null;

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.FileName = inFileName;

            try {
                if (curFileDialog.ShowDialog() == DialogResult.OK) {
                    String curFileName = curFileDialog.FileName;
                    if (curFileName != null) {
                        if ( Path.GetExtension( curFileName ) == null ) curFileName += ".txt";
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        private bool isObjectEmpty(object inObject) {
            bool curReturnValue = false;
            if (inObject == null) {
                curReturnValue = true;
            } else if (inObject == System.DBNull.Value) {
                curReturnValue = true;
            } else if (inObject.ToString().Length > 0) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

    }
}
