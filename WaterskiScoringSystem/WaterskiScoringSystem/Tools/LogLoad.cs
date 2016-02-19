using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class LogLoad {
        private static String myLogFileName = "-log.log";
        private String mySanctionNum;
        private ProgressWindow myProgressInfo;

        private LogLoad() {
            // Load data rating values
        }

        public bool readLoadDataFromLog() {
            bool curReturnValue = true;
            String inputBuffer = "";
            String curSlalomDelim = "Slalom:ScoreEntry:saveScore";
            String curTrickDelim = "Trick:ScoreCalc:saveTrick";
            String curJumpDelim = "Jump:ScoreEntrySeg3:saveScore";

            StringBuilder stmtSelect = new StringBuilder( "" );
            StringBuilder stmtWhere = new StringBuilder( "" );
            StringBuilder stmtInsert = new StringBuilder( "" );
            StringBuilder stmtData = new StringBuilder( "" );
            StreamReader myReader;
            SqlCeCommand sqlStmt = null;
            SqlCeConnection myDbConn = null;
            DataTable curDataTable = null;
            myProgressInfo = new ProgressWindow();

                try {
                    mySanctionNum = Properties.Settings.Default.AppSanctionNum;
                    if ( mySanctionNum == null ) {
                        mySanctionNum = "";
                    } else {
                        if ( mySanctionNum.Length < 6 ) {
                            mySanctionNum = "";
                        }
                    }
                } catch {
                    mySanctionNum = "";
                }

            myReader = getFile();
            if ( myReader != null ) {
                int curInputLineCount = 0;
                int curDelimPos = 0;
                try {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                    myDbConn.Open();

                    while (( inputBuffer = myReader.ReadLine() ) != null) {
                        curInputLineCount++;
                        myProgressInfo.setProgressValue( curInputLineCount );

                        if (inputBuffer.IndexOf( curSlalomDelim ) > 0) {
                            execSlalomData( inputBuffer, inputBuffer.IndexOf( curSlalomDelim ) );
                        } else if (inputBuffer.IndexOf( curTrickDelim ) > 0) {
                            execTrickData( inputBuffer, inputBuffer.IndexOf( curSlalomDelim ) );
                        } else if (inputBuffer.IndexOf( curJumpDelim ) > 0) {
                            execJumpData( inputBuffer, inputBuffer.IndexOf( curSlalomDelim ) );
                        }


                    }

                } catch (Exception ex) {
                    String ExcpMsg = ex.Message;
                    if (sqlStmt != null) {
                        ExcpMsg += "\n" + sqlStmt.CommandText;
                    }
                    MessageBox.Show( "Error: Performing SQL operations"
                        + "\n\nError: " + ExcpMsg
                        );
                } finally {
                    myDbConn.Close();
                    myReader.Close();
                    myReader.Dispose();
                }
                myProgressInfo.Close();
            }

            return curReturnValue;
        }

        private void execSlalomData(String inBuffer, int inDelimPos) {
            String curInsertCols = "", curInsertData = "", curUpdateData = "";
            int curDelimPos = 0, curDelimLen = 0;
            string[] curColList = null;
            string[] curDataList = null;

            int curSqlPos = inBuffer.IndexOf( " ", inDelimPos );
            String curSqlStmt = inBuffer.Substring( curSqlPos + 1 );
            if (curSqlStmt.ToLower().StartsWith( "insert" )) {
                curDelimPos = curSqlStmt.IndexOf("(") + 1;
                curDelimLen = curSqlStmt.IndexOf( "(" ) - curDelimPos;
                curInsertCols = curSqlStmt.Substring( curDelimPos, curDelimLen );
                curDelimPos = curSqlStmt.ToLower().IndexOf( ") values (" ) + 10;
                curDelimLen = curSqlStmt.IndexOf( ")" ) - curDelimPos;
                curInsertData = curSqlStmt.Substring( curDelimPos, curDelimLen );

                curColList = curInsertCols.Split( ',' );
                curDataList = new String[curColList.Length];

            } else if (curSqlStmt.ToLower().StartsWith( "update" )) {
                curDelimPos = curSqlStmt.ToLower().IndexOf( " set " ) + 1;
                curDelimLen = curSqlStmt.ToLower().IndexOf( "where" ) - curDelimPos;
                curUpdateData = curSqlStmt.Substring( curDelimPos, curDelimLen );
            }
        }

        private void execTrickData(String inBuffer, int inDelimPos) {
        }

        private void execJumpData(String inBuffer, int inDelimPos) {
        }

        private StreamReader getFile() {
            String myFileName, curPath;
            OpenFileDialog myFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if (curPath.Length < 2) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "txt files (*.log)|*.log|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    myFileName = myFileDialog.FileName;
                    if (myFileName != null) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        curPath = myFileName.Substring( 0, delimPos );
                        myReader = new StreamReader( myFileName );
                        try {
                            myProgressInfo.setProgessMsg( "File selected " + myFileName );
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();

                            string inputBuffer = "";
                            int curInputLineCount = 0;
                            myReader = new StreamReader( myFileName );
                            while (( inputBuffer = myReader.ReadLine() ) != null) {
                                curInputLineCount++;
                            }
                            myReader.Close();
                            myProgressInfo.setProgressMin( 1 );
                            myProgressInfo.setProgressMax( curInputLineCount );
                        } catch (Exception ex) {
                            MessageBox.Show( "Error: Could not read file" + myFileDialog.FileName + "\n\nError: " + ex.Message );
                            return null;
                        }
                        myReader = new StreamReader( myFileName );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not get a file to process " + "\n\nError: " + ex.Message );
            }

            return myReader;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    
    }
}
