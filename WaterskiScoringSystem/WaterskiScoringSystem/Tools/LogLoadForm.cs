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
    public partial class LogLoadForm : Form {
        private static String myLogFileName = "-log.log";
        private String mySanctionNum;
        private ProgressWindow myProgressInfo;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public LogLoadForm() {
            InitializeComponent();
        }

        private void LogLoadForm_Load(object sender, EventArgs e) {
            readLoadDataFromLog();
        }

        public bool readLoadDataFromLog() {
            bool curReturnValue = true;
            String inputBuffer = "";
            String curSlalomDelim = "Slalom:ScoreEntry:saveScore";
            String curTrickDelim = "Trick:ScoreCalc:saveTrick";
            String curJumpDelim = "Jump:ScoreEntrySeg3:saveScore";
            StreamReader myReader;
            myProgressInfo = new ProgressWindow();

            try {
                mySanctionNum = Properties.Settings.Default.AppSanctionNum;
                if (mySanctionNum == null) {
                    mySanctionNum = "";
                } else {
                    if (mySanctionNum.Length < 6) {
                        mySanctionNum = "";
                    }
                }
            } catch {
                mySanctionNum = "";
            }

            myReader = getFile();
            if (myReader != null) {
                int curInputLineCount = 0;
                int curDelimPos = 0;
                try {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                    DataTable curCountDataTable = buildCountDataTable();
                    while (( inputBuffer = myReader.ReadLine() ) != null) {
                        curInputLineCount++;
                        myProgressInfo.setProgressValue( curInputLineCount );

                        if (inputBuffer.IndexOf( curSlalomDelim ) > 0) {
                            findAndExecRequests( inputBuffer, inputBuffer.IndexOf( curSlalomDelim ), curCountDataTable );
                        } else if (inputBuffer.IndexOf( curTrickDelim ) > 0) {
                            findAndExecRequests( inputBuffer, inputBuffer.IndexOf( curTrickDelim ), curCountDataTable );
                        } else if (inputBuffer.IndexOf( curJumpDelim ) > 0) {
                            findAndExecRequests( inputBuffer, inputBuffer.IndexOf( curJumpDelim ), curCountDataTable );
                        }
                    }

                    StringBuilder curMsg = new StringBuilder( "" );
                    foreach (DataRow curRow in curCountDataTable.Rows) {
                        curMsg.Append( (String)curRow["Table"] + " " + (String)curRow["Command"] + " " + ( (Int32)curRow["Count"] ).ToString() + "\n" );
                    }
                    curCountDataTable.DefaultView.Sort = "Table ASC, Command ASC";
                    curCountDataTable = curCountDataTable.DefaultView.ToTable();
                    MessageBox.Show( curMsg.ToString() );

                } catch (Exception ex) {
                    String ExcpMsg = ex.Message;
                    MessageBox.Show( "Error: Performing SQL operations"
                        + "\n\nError: " + ExcpMsg
                        );
                } finally {
                    myReader.Close();
                    myReader.Dispose();
                }
                myProgressInfo.Close();
            }

            return curReturnValue;
        }

        private void findAndExecRequests(String inBuffer, int inDelimPos, DataTable inCountDataTable) {
            String curInsertCols = "", curData = "", curTableName = "", curCommand = "";
            int curDelimPos = 0, curDelimLen = 0, curCount = 0;
            string[] curColList = null;
            DataRow[] curFindRows = null;
            DataRowView newCountRow;

            try {
                int curSqlPos = inBuffer.IndexOf( " ", inDelimPos );
                String curSqlStmt = inBuffer.Substring( curSqlPos + 1 );
                if (curSqlStmt.ToLower().StartsWith( "insert" )) {
                    curCommand = "Insert";
                    curDelimPos = curSqlStmt.IndexOf( "(" ) + 1;
                    curDelimLen = curSqlStmt.IndexOf( ")", curDelimPos ) - curDelimPos;
                    curTableName = curSqlStmt.Substring( 7, curDelimPos - 9 ).Trim();
                    curInsertCols = curSqlStmt.Substring( curDelimPos, curDelimLen );

                    curDelimPos = curSqlStmt.ToLower().IndexOf( ") values (" ) + 10;
                    curDelimLen = curSqlStmt.Length - curDelimPos - 1;
                    curData = curSqlStmt.Substring( curDelimPos, curDelimLen );
                    curColList = curInsertCols.Split( ',' );

                    if (curTableName.Length > 0) {
                        execScoreRequest( curTableName, curColList, getDataFromInsertStmt( curColList, curData ), curSqlStmt );
                    }
                } else if (curSqlStmt.ToLower().StartsWith( "update" )) {
                    curCommand = "Update";
                    curDelimPos = curSqlStmt.ToLower().IndexOf( " set " ) + 5;
                    curDelimLen = curSqlStmt.ToLower().IndexOf( "where", curDelimPos ) - curDelimPos;
                    curTableName = curSqlStmt.Substring( 7, curDelimPos - 12 ).Trim();
                    curData = curSqlStmt.Substring( curDelimPos, curDelimLen );
                }

                if (curCommand.Length > 1) {
                    curFindRows = inCountDataTable.Select( "Table = '" + curTableName + "' AND Command = '" + curCommand + "'" );
                    if (curFindRows.Length > 0) {
                        curCount = (Int32)curFindRows[0]["Count"];
                        curFindRows[0]["Count"] = curCount + 1;
                    } else {
                        newCountRow = inCountDataTable.DefaultView.AddNew();
                        newCountRow["Table"] = curTableName;
                        newCountRow["Command"] = curCommand;
                        newCountRow["Count"] = 1;
                        newCountRow.EndEdit();
                    }
                }

            } catch (Exception ex) {
                String ExcpMsg = ex.Message;
                MessageBox.Show( "Error analyzing slalom log entry \n\n" + "\n\nError: " + ExcpMsg );
            }

        }

        private String[] getDataFromInsertStmt(String[] curColList, String inData) {
            string[] curDataList = new String[curColList.Length];

            int curIdx = 0, curDelimPos = 0, curDelimEnd = 0;
            while (curIdx < curColList.Length) {
                while (inData.Substring( curDelimPos, 1 ).Equals( " " )) {
                    curDelimPos++;
                }
                if (inData.Substring( curDelimPos, 1 ).Equals( "'" )) {
                    curDelimEnd = inData.IndexOf( "', ", curDelimPos + 1 );
                    if (curDelimEnd < 0) curDelimEnd = inData.Length - 1;
                    curDataList[curIdx] = inData.Substring( curDelimPos, curDelimEnd - curDelimPos + 1 );
                    curDelimPos = curDelimEnd + 3;
                } else {
                    curDelimEnd = inData.IndexOf( ", ", curDelimPos );
                    if (curDelimEnd < 0) curDelimEnd = inData.Length;
                    curDataList[curIdx] = inData.Substring( curDelimPos, curDelimEnd - curDelimPos ).Trim();
                    curDelimPos = curDelimEnd + 2;
                }
                curIdx++;
            }

            return curDataList;
        }

        private void execScoreRequest(String inTable, String[] curColList, String[] curDataList, String inSqlStmt) {
            int curUpdateIdxBeg = 4;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            curSqlStmt.Append( "Select * From " + inTable + " " );
            StringBuilder curSqlWhereStmt = new StringBuilder( "Where " );
            curSqlWhereStmt.Append( "SanctionId = " + curDataList[0] + " " );
            curSqlWhereStmt.Append( "AND MemberId = " + curDataList[1] + " " );
            curSqlWhereStmt.Append( "AND AgeGroup = " + curDataList[2] + " " );
            curSqlWhereStmt.Append( "AND Round = " + curDataList[3] + " " );
            if (inTable.Equals( "SlalomRecap" )) {
                curSqlWhereStmt.Append( "AND PassNum = " + curDataList[4] + " " );
                curSqlWhereStmt.Append( "AND SkierRunNum = " + curDataList[5] + " " );
                curUpdateIdxBeg = 6;
            } else if (inTable.Equals( "TrickPass" )) {
                curSqlWhereStmt.Append( "AND PassNum = " + curDataList[4] + " " );
                curSqlWhereStmt.Append( "AND Skis = " + curDataList[5] + " " );
                curSqlWhereStmt.Append( "AND Seq = " + curDataList[6] + " " );
                curUpdateIdxBeg = 7;
            } else if (inTable.Equals( "JumpRecap" )) {
                curSqlWhereStmt.Append( "AND PassNum = " + curDataList[4] + " " );
                curUpdateIdxBeg = 5;
            }
            DataTable curDataTable = getData( curSqlStmt.ToString() + curSqlWhereStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                myDbConn.Open();
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update " + inTable + " Set " );
                int curIdx = curUpdateIdxBeg;
                while (curIdx < curColList.Length) {
                    if (curIdx > curUpdateIdxBeg) {
                        curSqlStmt.Append( ", " + curColList[curIdx] + " = " + curDataList[curIdx] );
                    } else {
                        curSqlStmt.Append( curColList[curIdx] + " = " + curDataList[curIdx] );
                    }
                    curIdx++;
                }
                try {
                    mySqlStmt = myDbConn.CreateCommand();
                    mySqlStmt.CommandText = curSqlStmt.ToString() + " " + curSqlWhereStmt.ToString();
                    int rowsProc = mySqlStmt.ExecuteNonQuery();
                } catch (Exception ex) {
                    String ExcpMsg = ex.Message;
                    if (mySqlStmt != null) {
                        ExcpMsg += "\n" + mySqlStmt.CommandText;
                    }
                    MessageBox.Show( "Error processing Update SQL Statement:" + "\n\nError: " + ExcpMsg );
                    return;
                } finally {
                    myDbConn.Close();
                }
            } else {
                try {
                    myDbConn.Open();
                    mySqlStmt = myDbConn.CreateCommand();
                    mySqlStmt.CommandText = inSqlStmt;
                    int rowsProc = mySqlStmt.ExecuteNonQuery();
                } catch (Exception ex) {
                    String ExcpMsg = ex.Message;
                    if (mySqlStmt != null) {
                        ExcpMsg += "\n" + mySqlStmt.CommandText;
                    }
                    MessageBox.Show( "Error processing Insert SQL Statement:" + "\n\nError: " + ExcpMsg );
                    return;
                } finally {
                    myDbConn.Close();
                }
            }

        }

        private DataTable buildCountDataTable() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "Table";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Command";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Count";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
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
