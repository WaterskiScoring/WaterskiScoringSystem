using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class LogRecoveryUtility : Form {
        private String mySanctionNum;
        private String myLogRecoveryCmdFileName;

        private decimal myDatabaseVersion = 0.00M;
        private ProgressWindow myProgressInfo;
        private StreamWriter outBuffer;
        private StreamWriter outDiscardBuffer;

        public LogRecoveryUtility() {
            InitializeComponent();

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            String curSqlStmt = "SELECT ListName, ListCode, CodeValue as VersionNumText, MinValue as VersionNum "
                + "FROM CodeValueList WHERE ListName = 'DatabaseVersion'";
            DataTable curDataTable = getData(curSqlStmt);
            if ( curDataTable.Rows.Count > 0 ) {
                myDatabaseVersion = (decimal) curDataTable.Rows[0]["VersionNum"];
            }
        }

        private void LogRecoveryUtility_Load( object sender, EventArgs e ) {
            readLoadDataFromLog();
        }

        private void button1_Click( object sender, EventArgs e ) {
            loadRecoveredData();
        }

        private void loadRecoveredData() {
            String inputBuffer, curSqlStmt = "";
            int rowsProc = 0, curInputLineCount = 0;
            StreamReader myReader = null;
            myProgressInfo = new ProgressWindow();

            #region Process all commands in the input file

            myReader = new StreamReader(myLogRecoveryCmdFileName);

            if ( myReader != null ) {
                try {
                    myProgressInfo.setProgessMsg("File selected " + myLogRecoveryCmdFileName);
                    myProgressInfo.Show();
                    myProgressInfo.Refresh();

                    curInputLineCount = 0;
                    myReader = new StreamReader(myLogRecoveryCmdFileName);
                    while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                        curInputLineCount++;
                    }
                    myReader.Close();
                    myProgressInfo.setProgressMin(1);
                    myProgressInfo.setProgressMax(curInputLineCount);

                } catch ( Exception ex ) {
                    MessageBox.Show("Error: Could not read file" + myLogRecoveryCmdFileName + "\n\nError: " + ex.Message);
                    return;

                } finally {
                    myProgressInfo.Close();
                    if ( myReader != null ) {
                        myReader.Close();
                        myReader.Dispose();
                    }
                }


                curInputLineCount = 0;
                try {
                    myReader = new StreamReader(myLogRecoveryCmdFileName);
                    while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                        curInputLineCount++;
                        myProgressInfo.setProgressValue(curInputLineCount);

                        try {
                            curSqlStmt = inputBuffer;
                            if ( curSqlStmt.IndexOf("Where PK =") <= 0 ) {
                                rowsProc = DataAccess.ExecuteCommand(curSqlStmt);
                            }

                        } catch ( Exception ex ) {
                            String ExcpMsg = ex.Message;
                            if ( curSqlStmt != null ) {
                                ExcpMsg += "\n" + curSqlStmt;
                            }
                            MessageBox.Show("Error attempting to execute statement" + "\n\nError: " + ExcpMsg);
                        }
                    }

                } catch ( Exception ex ) {
                    MessageBox.Show("Error attempting to execute statement" + "\n\nError: " + ex.Message);

                } finally {
                    myProgressInfo.Close();
                    myReader.Close();
                    myReader.Dispose();

                }
            }
            #endregion
        }

        private bool readLoadDataFromLog() {
            bool curReturnValue = true;
            String inputBuffer = "";

            /*
            For data cleanup
            Delete TourReg Where SanctionId = '17E042'
            Delete EventReg Where SanctionId = '17E042'
            Delete OfficialWork Where SanctionId = '17E042'

            Delete SlalomScore Where SanctionId = '17E042'
            Delete SlalomRecap Where SanctionId = '17E042'
            
            Delete TrickScore Where SanctionId = '17E042'
            Delete TrickPass Where SanctionId = '17E042'

            Delete JumpScore Where SanctionId = '17E042'
            Delete JumpRecap Where SanctionId = '17E042'
            */
            String curTourRegUpdateDelim = "Tournament:Registration:navSave_Click";
            String curTourRegDelim = "Tournament:TourEventReg:addTourReg";
            String curEventRegDelim = "Tournament:TourEventReg:addEvent";
            String curEventOfficialDelim = "Tournament:TourEventReg:addEventOfficial";
            String curRunningOrderDelim = "Tournament:RunningOrderTour:navSave";

            String curSlalomScoreDelim = "Slalom:ScoreEntry:saveScore";
            String curDeleteSlalomPassDelim = "ScoreEntry:deletePassButton";
            String curSlalomForceCompDelim = "Slalom:ScoreEntry:ForceCompButton_Click";
            String curSlalomEventClassDelim = "Slalom:ScoreEntry:scoreEventClass_Validating";
            String curSlalomBoatSelectDelim = "Slalom:ScoreEntry:updateBoatSelect";

            String curTrickScoreDelim = "Trick:ScoreCalc:saveTrickScore";
            String curTrickPassDelim = "Trick:ScoreCalc:saveTrickPassEntry";
            String curDeleteTrickPassDelim = "Trick:ScoreCalc:deleteTrickPass";
            String curTrickForceCompDelim = "Trick:ScoreCalc:ForceCompButton_Click";
            String curTrickEventClassDelim = "Trick:ScoreCalc:scoreEventClass_Validating";
            String curTrickBoatSelectDelim = "Trick:ScoreCalc:updateBoatSelect";

            String curJumpScoreDelim = "Jump:ScoreEntrySeg3:saveScore";
            String curDeleteJumpPassDelim = "Jump:ScoreEntrySeg3:deletePassButton";
            String curJumpForceCompDelim = "Jump:ScoreEntrySeg3:ForceCompButton_Click";
            String curJumpEventClassDelim = "Jump:ScoreEntrySeg3:scoreEventClass_Validating";
            String curJumpBoatSelectDelim = "Jump:ScoreEntrySeg3:updateBoatSelect";

            //StringBuilder stmtSelect = new StringBuilder( "" );
            //StringBuilder stmtWhere = new StringBuilder( "" );
            //StringBuilder stmtInsert = new StringBuilder( "" );
            //StringBuilder stmtData = new StringBuilder( "" );

            outBuffer = getExportFile(null, "LogLoadStatements.txt");

            StreamReader myReader;
            myProgressInfo = new ProgressWindow();

            myReader = getInputLogFile();
            if ( myReader != null ) {
                int curInputLineCount = 0;
                int curDelimPos = 0;
                try {
                    while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                        curInputLineCount++;
                        myProgressInfo.setProgressValue(curInputLineCount);

                        if ( inputBuffer.IndexOf(curTourRegDelim) > 0 ) {
                            extractTourEventRegData(inputBuffer, inputBuffer.IndexOf(curTourRegDelim));
                        } else if ( inputBuffer.IndexOf(curTourRegUpdateDelim) > 0 ) {
                            extractTourEventRegData(inputBuffer, inputBuffer.IndexOf(curTourRegUpdateDelim));
                        } else if ( inputBuffer.IndexOf(curEventRegDelim) > 0 ) {
                            extractTourEventRegData(inputBuffer, inputBuffer.IndexOf(curEventRegDelim));
                        } else if ( inputBuffer.IndexOf(curEventOfficialDelim) > 0 ) {
                            extractEventOfficialData(inputBuffer, inputBuffer.IndexOf(curEventOfficialDelim));
                        } else if ( inputBuffer.IndexOf(curRunningOrderDelim) > 0 ) {
                            extractTourEventRegData(inputBuffer, inputBuffer.IndexOf(curRunningOrderDelim));

                        } else if ( inputBuffer.IndexOf(curSlalomScoreDelim) > 0 ) {
                            extractSlalomScoreData(inputBuffer, inputBuffer.IndexOf(curSlalomScoreDelim));
                        } else if ( inputBuffer.IndexOf(curDeleteSlalomPassDelim) > 0 ) {
                            extractDeleteSlalomPassData(inputBuffer, inputBuffer.IndexOf(curDeleteSlalomPassDelim));
                        } else if ( inputBuffer.IndexOf(curSlalomForceCompDelim) > 0 ) {
                            extractSlalomForceCompData(inputBuffer, inputBuffer.IndexOf(curSlalomForceCompDelim));
                        } else if ( inputBuffer.IndexOf(curSlalomEventClassDelim) > 0 ) {
                            extractSlalomEventClassData(inputBuffer, inputBuffer.IndexOf(curSlalomEventClassDelim));
                        } else if ( inputBuffer.IndexOf(curSlalomBoatSelectDelim) > 0 ) {
                            extractBoatSelectionData(inputBuffer, inputBuffer.IndexOf(curSlalomBoatSelectDelim));

                        } else if ( inputBuffer.IndexOf(curTrickScoreDelim) > 0 ) {
                            extractTrickScoreData(inputBuffer, inputBuffer.IndexOf(curTrickScoreDelim));
                        } else if ( inputBuffer.IndexOf(curTrickPassDelim) > 0 ) {
                            extractTrickScoreData(inputBuffer, inputBuffer.IndexOf(curTrickPassDelim));
                        } else if ( inputBuffer.IndexOf(curDeleteTrickPassDelim) > 0 ) {
                            extractTrickScoreData(inputBuffer, inputBuffer.IndexOf(curDeleteTrickPassDelim));
                        } else if ( inputBuffer.IndexOf(curTrickForceCompDelim) > 0 ) {
                            extractTrickForceCompData(inputBuffer, inputBuffer.IndexOf(curTrickForceCompDelim));
                        } else if ( inputBuffer.IndexOf(curTrickEventClassDelim) > 0 ) {
                            extractTrickEventClassData(inputBuffer, inputBuffer.IndexOf(curTrickEventClassDelim));
                        } else if ( inputBuffer.IndexOf(curTrickBoatSelectDelim) > 0 ) {
                            extractBoatSelectionData(inputBuffer, inputBuffer.IndexOf(curTrickBoatSelectDelim));

                        } else if ( inputBuffer.IndexOf(curJumpScoreDelim) > 0 ) {
                            extractJumpScoreData(inputBuffer, inputBuffer.IndexOf(curJumpScoreDelim));
                        } else if ( inputBuffer.IndexOf(curDeleteJumpPassDelim) > 0 ) {
                            extractJumpScoreData(inputBuffer, inputBuffer.IndexOf(curDeleteJumpPassDelim));
                        } else if ( inputBuffer.IndexOf(curJumpForceCompDelim) > 0 ) {
                            extractJumpForceCompData(inputBuffer, inputBuffer.IndexOf(curJumpForceCompDelim));
                        } else if ( inputBuffer.IndexOf(curJumpEventClassDelim) > 0 ) {
                            extractJumpEventClassData(inputBuffer, inputBuffer.IndexOf(curJumpEventClassDelim));
                        } else if ( inputBuffer.IndexOf(curJumpBoatSelectDelim) > 0 ) {
                            extractBoatSelectionData(inputBuffer, inputBuffer.IndexOf(curJumpBoatSelectDelim));

                        } else {
                            SaveUnselectedStatements(inputBuffer);
                        }


                    }

                } catch ( Exception ex ) {
                    String ExcpMsg = ex.Message;
                    MessageBox.Show("Error: Performing SQL operations"
                        + "\n\nError: " + ExcpMsg
                        );

                } finally {
                    myReader.Close();
                    myReader.Dispose();

                    outBuffer.Close();
                    outBuffer.Dispose();
                    outDiscardBuffer.Close();
                    outDiscardBuffer.Dispose();
                }

                myProgressInfo.Close();
            }

            return curReturnValue;
        }

        #region Extract slalom data
        private void extractSlalomScoreData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert") ) {
                writeLogRecoveryOutput(curSqlStmt);

            } else if ( curSqlStmt.ToLower().StartsWith("update") ) {
                if ( curSqlStmt.IndexOf("Where PK =") > 0 ) {
                    if ( curSqlStmt.ToLower().StartsWith("update slalomscore")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }));

                    } else if ( curSqlStmt.ToLower().StartsWith("update slalomrecap")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        && curSqlStmt.Contains("SkierRunNum")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "SkierRunNum" }));

                    } else {
                        SaveUnselectedStatements(inBuffer);
                    }

                } else {
                    writeLogRecoveryOutput(curSqlStmt);
                }

            } else {
                SaveUnselectedStatements(inBuffer);
            }
        }

        private void extractDeleteSlalomPassData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        private void extractSlalomForceCompData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        private void extractSlalomEventClassData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        #endregion

        #region Extract trick data
        private void extractTrickScoreData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert") ) {
                writeLogRecoveryOutput(curSqlStmt);

            } else if ( curSqlStmt.ToLower().StartsWith("update") ) {
                if ( curSqlStmt.IndexOf("Where PK =") > 0 ) {
                    if ( curSqlStmt.ToLower().StartsWith("update trickscore")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }));

                    } else if ( curSqlStmt.ToLower().StartsWith("update trickpass")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        && curSqlStmt.Contains("PassNum")
                        && curSqlStmt.Contains("Seq")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum", "Seq" }));

                    } else {
                        SaveUnselectedStatements(inBuffer);
                    }

                } else {
                    writeLogRecoveryOutput(curSqlStmt);
                }

            } else {
                SaveUnselectedStatements(inBuffer);
            }
        }

        private void extractTrickForceCompData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        private void extractTrickEventClassData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }
        #endregion

        #region Extract jump data
        private void extractJumpScoreData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert") ) {
                writeLogRecoveryOutput(curSqlStmt);

            } else if ( curSqlStmt.ToLower().StartsWith("update") ) {
                if ( curSqlStmt.IndexOf("Where PK =") > 0 ) {
                    if ( curSqlStmt.ToLower().StartsWith("update jumpscore")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round" }));

                    } else if ( curSqlStmt.ToLower().StartsWith("update jumprecap")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Round")
                        && curSqlStmt.Contains("PassNum")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Round", "PassNum" }));

                    } else {
                        SaveUnselectedStatements(inBuffer);
                    }

                } else {
                    writeLogRecoveryOutput(curSqlStmt);
                }

            } else {
                SaveUnselectedStatements(inBuffer);
            }
        }

        private void extractJumpForceCompData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        private void extractJumpEventClassData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }
        #endregion

        #region Extract general tournament data
        private void extractBoatSelectionData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        private void extractTourEventRegData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert") ) {
                writeLogRecoveryOutput(curSqlStmt);

            } else if ( curSqlStmt.ToLower().StartsWith("update") ) {
                if ( curSqlStmt.IndexOf("Where PK =") > 0 ) {
                    if ( curSqlStmt.ToLower().StartsWith("update tourreg") 
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup" } ));

                    } else if ( curSqlStmt.ToLower().StartsWith("update eventreg")
                        && curSqlStmt.Contains("MemberId")
                        && curSqlStmt.Contains("AgeGroup")
                        && curSqlStmt.Contains("Event")
                        ) {
                        writeLogRecoveryOutput(replacePkWhereClause(curSqlStmt, new String[] { "SanctionId", "MemberId", "AgeGroup", "Event" }));

                    } else {
                        SaveUnselectedStatements(inBuffer);
                    }

                } else {
                    writeLogRecoveryOutput(curSqlStmt);
                }

            } else {
                SaveUnselectedStatements(inBuffer);
            }

        }

        private void extractEventOfficialData( String inBuffer, int inDelimPos ) {
            int curSqlPos = inBuffer.IndexOf(" ", inDelimPos);
            String curSqlStmt = inBuffer.Substring(curSqlPos + 1);
            if ( curSqlStmt.ToLower().StartsWith("insert")
                || curSqlStmt.ToLower().StartsWith("update")
                ) {
                writeLogRecoveryOutput(curSqlStmt);
            }

        }

        #endregion

        private String replacePkWhereClause( String inBuffer , String[] argList) {
            int delimWhereClause = inBuffer.IndexOf("Where PK =");
            if ( delimWhereClause > 0 ) {
                StringBuilder updateStmt = new StringBuilder(inBuffer.Substring(0, ( delimWhereClause - 1 )));

                StringBuilder whereStmt = new StringBuilder(" Where ");
                int dataDelimStart = inBuffer.IndexOf(" Set ") + 5;
                int dataDelimEnd = inBuffer.IndexOf("Where PK =") - 1;
                String updateData = inBuffer.Substring(dataDelimStart, dataDelimEnd - dataDelimStart);

                String[] stmtNodes = updateData.Split(',');
                foreach ( String curNode in stmtNodes ) {
                    foreach ( String curArg in argList ) {
                        if ( curNode.Contains(curArg) ) {
                            if ( whereStmt.Length > 7 ) {
                                whereStmt.Append(" And " + curNode);
                            } else {
                                whereStmt.Append(curNode);
                            }
                        }
                    }
                }

                if ( whereStmt.Length > 7 ) {
                    updateStmt.Append(whereStmt.ToString());
                    return updateStmt.ToString();

                } else {
                    return inBuffer;
                }

            } else {
                return inBuffer;
            }

        }

        private void SaveUnselectedStatements( String inBuffer ) {
                writeLogDiscardOutput(inBuffer);
        }

        private void writeLogRecoveryOutput( String outputBuffer ) {
            try {
                outBuffer.WriteLine(outputBuffer);

            } catch ( Exception ex ) {
                MessageBox.Show("Error writing to output file: " + "\n\nError: " + ex.Message);
            }
        }

        private void writeLogDiscardOutput( String outputBuffer ) {
            try {
                outDiscardBuffer.WriteLine(outputBuffer);

            } catch ( Exception ex ) {
                MessageBox.Show("Error writing to output discard file: " + "\n\nError: " + ex.Message);
            }
        }

        private StreamReader getInputLogFile() {
            String myFileName, curPath;
            OpenFileDialog myFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "txt files (*.log)|*.log|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf('\\');
                        curPath = myFileName.Substring(0, delimPos);
                        myReader = new StreamReader(myFileName);
                        try {
                            myProgressInfo.setProgessMsg("File selected " + myFileName);
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();

                            string inputBuffer = "";
                            int curInputLineCount = 0;
                            myReader = new StreamReader(myFileName);
                            while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                                curInputLineCount++;
                            }
                            myReader.Close();
                            myProgressInfo.setProgressMin(1);
                            myProgressInfo.setProgressMax(curInputLineCount);
                        } catch ( Exception ex ) {
                            MessageBox.Show("Error: Could not read file" + myFileDialog.FileName + "\n\nError: " + ex.Message);
                            return null;
                        }
                        myReader = new StreamReader(myFileName);
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show("Error: Could not get a file to process " + "\n\nError: " + ex.Message);
            }

            return myReader;
        }

        private StreamWriter getExportFile( String inFileFilter, String inFileName ) {
            String curMethodName = "getExportFile";
            String curDiscardFileName;
            StreamWriter outBuffer = null;
            String curFileFilter = "";
            if ( inFileFilter == null ) {
                curFileFilter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            } else {
                curFileFilter = inFileFilter;
            }

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = curFileFilter;
            myFileDialog.FilterIndex = 1;
            if ( inFileName == null ) {
                myFileDialog.FileName = "";
            } else {
                myFileDialog.FileName = inFileName;
            }

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myLogRecoveryCmdFileName = myFileDialog.FileName;
                    if ( myLogRecoveryCmdFileName != null ) {
                        int delimPos = myLogRecoveryCmdFileName.LastIndexOf('\\');
                        String curFileName = myLogRecoveryCmdFileName.Substring(delimPos + 1);
                        if ( curFileName.IndexOf('.') < 0 ) {
                            String curDefaultExt = ".txt";
                            String[] curList = curFileFilter.Split('|');
                            if ( curList.Length > 0 ) {
                                int curDelim = curList[1].IndexOf(".");
                                if ( curDelim > 0 ) {
                                    curDefaultExt = curList[1].Substring(curDelim - 1);
                                }
                            }
                            myLogRecoveryCmdFileName += curDefaultExt;
                        }
                        outBuffer = File.CreateText(myLogRecoveryCmdFileName);

                        curDiscardFileName = myLogRecoveryCmdFileName.Substring(0, myLogRecoveryCmdFileName.LastIndexOf('.') - 1) + "-discard.txt";
                        outDiscardBuffer = File.CreateText(curDiscardFileName);

                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show("Error: Could not get a file to export data to " + "\n\nError: " + ex.Message);
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile(curMsg);
            }

            return outBuffer;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable(inSelectStmt);
        }

    }
}
