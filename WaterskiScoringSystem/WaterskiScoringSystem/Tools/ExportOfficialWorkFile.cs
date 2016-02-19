using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ExportOfficialWorkFile {
        private bool isSlalomActive = false;
        private bool isTrickActive = false;
        private bool isJumpActive = false;
        private StreamWriter myOutBuffer = null;
        private String mySanctionNum;
        private DataRow myTourRow;
        private DataTable myDataTable;

        public ExportOfficialWorkFile() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            try {
                //Retrieve selected tournament attributes
                DataTable curTourDataTable = getTourData();
                if ( curTourDataTable.Rows.Count > 0 ) {
                    myTourRow = curTourDataTable.Rows[0];

                    OfficialWorkUpdate curWorkRecordUpdate = new OfficialWorkUpdate();
                    curWorkRecordUpdate.updateOfficialWorkRecord();

                    myDataTable = getOfficialWorkData();
                } else {
                    MessageBox.Show( "The active tournament is not properly defined.  You must select from the Administration menu Tournament List option" );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile instantiation method"
                    + "\n\nException: " + ex.Message
                );
            }
        }

        public bool ExportData() {
            try {
                if ( myDataTable.Rows.Count > 0 ) {

                    if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
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


                    return readExportData();
                } else {
                    MessageBox.Show( "There is no data available to export" );
                    return false;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile instantiation method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }
        
        private bool readExportData() {
            bool returnStatus = false;
            StringBuilder outLine = new StringBuilder( "" );
            String curFilename = mySanctionNum.Trim() + "OD.txt";
            myOutBuffer = getExportFile( curFilename );

            try {
                if ( myOutBuffer != null ) {
                    //Build file header line and write to file
                    returnStatus = writeHeader();
                    if ( returnStatus ) {
                        foreach ( DataRow curRow in myDataTable.Rows ) {
                            //Write data to output file
                            returnStatus = writeDataRow( curRow );
                            if ( !( returnStatus ) ) { break; }
                        }
                        if ( returnStatus ) { returnStatus = writeFooter(); }
                    }

                    myOutBuffer.Close();
                    returnStatus = true;
                    if ( returnStatus ) {
                        MessageBox.Show( myDataTable.Rows.Count +  " rows exported to officials credit report data file." );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile readExportData method"
                    + "\n\nException: " + ex.Message
                );
            }

            return returnStatus;
        }

        //Write export file identification and header information
        private bool writeHeader() {
            StringBuilder outLine = new StringBuilder( "" );

            try {
                String curEventDate = "";
                try {
                    DateTime curTourDate = Convert.ToDateTime( myTourRow["EventDates"].ToString() );
                    curEventDate = curTourDate.ToString("MM-dd-yyyy");
                } catch ( Exception ex ) {
                    curEventDate = myTourRow["EventDates"].ToString();
                }

                outLine.Append( mySanctionNum );
                outLine.Append( myTourRow["Class"].ToString() + " ENDS " + curEventDate + "  " + myTourRow["Name"].ToString() );
                outLine.Append( "  SUMMARY AS OF  " + DateTime.Now.ToString( "HH:MM:ss  MM-dd-yyyy" ) );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "---- OFFICIALS CREDIT REPORT ----   "
                    + "DRIVE  JUDGE  SCORE  SAFTY  TECH   ANNCR  "
                    + "-- TOTAL EVENT HOURS --"
                    );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                outLine.Append( "NAME OF OFFICIAL       "
                    + "MEMBER ID #  R STJ  R STJ  R STJ  R STJ  R STJ  R STJ  "
                    + "DRIVE JUDGE SCORE SAFTY"
                    );
                myOutBuffer.WriteLine( outLine.ToString() );

                outLine = new StringBuilder( "" );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile writeHeader method"
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
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile writeFooter method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        //Write export file footer information
        private bool writeDataRow( DataRow curRow ) {
            StringBuilder outLine = new StringBuilder( "" );
            String curValue = "";

            try {
                curValue = curRow["SkierName"].ToString();
                if ( curValue.Length > 24 ) {
                    outLine.Append( curValue.Substring( 0, 24 ) );
                } else {
                    //curValue = curValue.PadRight( 24 - curValue.Length, ' ' );
                    outLine.Append( curValue.PadRight( 23, ' ' ) );
                }
                curValue = curRow["MemberId"].ToString();
                outLine.Append( curValue.Substring( 0, 3 ) + "-" + curValue.Substring( 3, 2 ) + "-" + curValue.Substring( 5, 4 ) );

                outLine.Append( "    " );
                curValue = curRow["DriverChief"].ToString();
                if ( curValue.Equals( "Y" ) ) {
                    if (isSlalomActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isTrickActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isJumpActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    curValue = curRow["DriverAsstChief"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        if ( isSlalomActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                    } else {
                        if ( isSlalomActive ) {
                            curValue = curRow["DriverSlalomCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            curValue = curRow["DriverTrickCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            curValue = curRow["DriverJumpCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                    }
                }

                outLine.Append( "    " );
                curValue = curRow["JudgeChief"].ToString();
                if ( curValue.Equals( "Y" ) ) {
                    if (isSlalomActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isTrickActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isJumpActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    curValue = curRow["JudgeAsstChief"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        if ( isSlalomActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                    } else {
                        if ( isSlalomActive ) {
                            curValue = curRow["JudgeSlalomCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            curValue = curRow["JudgeTrickCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            curValue = curRow["JudgeJumpCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                    }
                }

                outLine.Append( "    " );
                curValue = curRow["ScoreChief"].ToString();
                if ( curValue.Equals( "Y" ) ) {
                    if (isSlalomActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isTrickActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isJumpActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    curValue = curRow["ScoreAsstChief"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        if ( isSlalomActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                    } else {
                        if ( isSlalomActive ) {
                            curValue = curRow["ScoreSlalomCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            curValue = curRow["ScoreTrickCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            curValue = curRow["ScoreJumpCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                    }
                }

                outLine.Append( "    " );
                curValue = curRow["SafetyChief"].ToString();
                if ( curValue.Equals( "Y" ) ) {
                    if (isSlalomActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isTrickActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if (isJumpActive) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    curValue = curRow["SafetyAsstChief"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        if ( isSlalomActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                    } else {
                        if ( isSlalomActive ) {
                            curValue = curRow["SafetySlalomCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            curValue = curRow["SafetyTrickCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            curValue = curRow["SafetyJumpCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                    }
                }

                outLine.Append( "    " );
                curValue = curRow["TechChief"].ToString();
                if ( curValue.Equals( "Y" ) ) {
                    if ( isSlalomActive ) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if ( isTrickActive ) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                    if ( isJumpActive ) {
                        outLine.Append( "C" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    curValue = curRow["TechAsstChief"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        if ( isSlalomActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            outLine.Append( "A" );
                        } else {
                            outLine.Append( "-" );
                        }
                    } else {
                        if ( isSlalomActive ) {
                            curValue = curRow["TechSlalomCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isTrickActive ) {
                            curValue = curRow["TechTrickCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                        if ( isJumpActive ) {
                            curValue = curRow["TechJumpCredit"].ToString();
                            if ( curValue.Equals( "Y" ) ) {
                                outLine.Append( "1" );
                            } else {
                                outLine.Append( "-" );
                            }
                        } else {
                            outLine.Append( "-" );
                        }
                    }
                }

                outLine.Append( "    " );
                if ( isSlalomActive ) {
                    curValue = curRow["AnncrSlalomCredit"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        outLine.Append( "1" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    outLine.Append( "-" );
                }
                if ( isTrickActive ) {
                    curValue = curRow["AnncrTrickCredit"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        outLine.Append( "1" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    outLine.Append( "-" );
                }
                if ( isJumpActive ) {
                    curValue = curRow["AnncrJumpCredit"].ToString();
                    if ( curValue.Equals( "Y" ) ) {
                        outLine.Append( "1" );
                    } else {
                        outLine.Append( "-" );
                    }
                } else {
                    outLine.Append( "-" );
                }

                outLine.Append( "    0.0   0.0   0.0   0.0" );
                myOutBuffer.WriteLine( outLine.ToString() );

                return true;
            } catch ( Exception ex ) {
                MessageBox.Show( "Exception encountered in ExportOfficialWorkFile writeDataRow method"
                    + "\n\nException: " + ex.Message
                );
                return false;
            }
        }

        private StreamWriter getExportFile( String inFileName ) {
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

        private DataTable getOfficialWorkData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct O.SanctionId, O.MemberId, T.SkierName" );
            curSqlStmt.Append( ", O.JudgeChief, O.JudgeAsstChief, O.JudgeAppointed" );
            curSqlStmt.Append( ", O.DriverChief, O.DriverAsstChief, O.DriverAppointed" );
            curSqlStmt.Append( ", O.ScoreChief, O.ScoreAsstChief, O.ScoreAppointed" );
            curSqlStmt.Append( ", O.SafetyChief, O.SafetyAsstChief, O.SafetyAppointed" );
            curSqlStmt.Append( ", O.TechChief, O.TechAsstChief" );
            curSqlStmt.Append( ", O.JudgeSlalomCredit, O.JudgeTrickCredit, O.JudgeJumpCredit" );
            curSqlStmt.Append( ", O.ScoreSlalomCredit, O.ScoreTrickCredit, O.ScoreJumpCredit" );
            curSqlStmt.Append( ", O.DriverSlalomCredit, O.DriverTrickCredit, O.DriverJumpCredit" );
            curSqlStmt.Append( ", O.SafetySlalomCredit, O.SafetyTrickCredit, O.SafetyJumpCredit" );
            curSqlStmt.Append( ", O.TechSlalomCredit, O.TechTrickCredit, O.TechJumpCredit" );
            curSqlStmt.Append( ", O.AnncrChief, O.AnncrSlalomCredit, O.AnncrTrickCredit, O.AnncrJumpCredit" );
            curSqlStmt.Append( ", O.JudgeSlalomRating, O.JudgeTrickRating, O.JudgeJumpRating" );
            curSqlStmt.Append( ", O.ScorerSlalomRating, O.ScorerTrickRating, O.ScorerJumpRating" );
            curSqlStmt.Append( ", O.DriverSlalomRating, O.DriverTrickRating, O.DriverJumpRating" );
            curSqlStmt.Append( ", O.SafetyOfficialRating, O.TechOfficialRating, O.AnncrOfficialRating " );
            curSqlStmt.Append( ", O.Note " );
            curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "	INNER JOIN TourReg T ON T.MemberId = O.MemberId AND T.SanctionId = O.SanctionId " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "And (O.JudgeSlalomCredit = 'Y' OR O.JudgeTrickCredit = 'Y' OR O.JudgeJumpCredit = 'Y' " );
            curSqlStmt.Append( " OR O.ScoreSlalomCredit = 'Y' OR O.ScoreTrickCredit = 'Y' OR O.ScoreJumpCredit = 'Y'  " );
            curSqlStmt.Append( " OR O.DriverSlalomCredit = 'Y' OR O.DriverTrickCredit = 'Y' OR O.DriverJumpCredit = 'Y' " );
            curSqlStmt.Append( " OR O.SafetySlalomCredit = 'Y' OR O.SafetyTrickCredit = 'Y' OR O.SafetyJumpCredit = 'Y' " );
            curSqlStmt.Append( " OR O.TechSlalomCredit = 'Y' OR O.TechTrickCredit = 'Y' OR O.TechJumpCredit = 'Y' " );
            curSqlStmt.Append( " OR O.AnncrSlalomCredit = 'Y' OR O.AnncrTrickCredit = 'Y' OR O.AnncrJumpCredit = 'Y' " );
            curSqlStmt.Append( ") ORDER BY T.SkierName, O.MemberId" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
