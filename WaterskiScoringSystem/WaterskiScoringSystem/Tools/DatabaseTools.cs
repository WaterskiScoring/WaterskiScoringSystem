using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using WaterskiScoringSystem.Common;


namespace WaterskiScoringSystem.Tools {
    class DatabaseTools {
        private String mySanctionNum;
        private decimal myDatabaseVersion = 0.00M;
        private SqlCeConnection myDbConn = null;
        private SqlCeCommand mySqlStmt = null;
        private ProgressWindow myProgressInfo;

        public DatabaseTools() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            String curSqlStmt = "SELECT ListName, ListCode, CodeValue as VersionNumText, MinValue as VersionNum "
                + "FROM CodeValueList WHERE ListName = 'DatabaseVersion'";
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
            if ( curDataTable.Rows.Count > 0 ) {
                myDatabaseVersion = (decimal)curDataTable.Rows[0]["VersionNum"];
            }
        }

        public bool execCommandFile() {
            bool curReturn = true;
            int curDelimIdx;
            decimal curDatabaseVersion = 9999.00M;
            String inputBuffer, curSqlStmt = "";
            StringBuilder curInputCmd = new StringBuilder( "" );
            ImportData curImportData = new ImportData();
            StreamReader myReader;
            myProgressInfo = new ProgressWindow();

            #region Process all commands in the input file
            myReader = getImportFile();
            if ( myReader != null ) {
                int curInputLineCount = 0;
                try {
                    while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                        curInputLineCount++;
                        myProgressInfo.setProgressValue( curInputLineCount );

                        if ( inputBuffer.TrimStart( ' ' ).StartsWith( "## " ) ) {
                            curDatabaseVersion = Convert.ToDecimal( inputBuffer.Substring( 4 ) );
                        }
                        if ( inputBuffer.TrimStart( ' ' ).StartsWith( "//" ) || inputBuffer.TrimStart( ' ' ).StartsWith( "##" ) ) {
                        } else {
                            if ( curDatabaseVersion > myDatabaseVersion ) {
                                curDelimIdx = inputBuffer.IndexOf( ';' );
                                if ( curDelimIdx >= 0 ) {
                                    if ( curDelimIdx > 0 ) {
                                        curInputCmd.Append( inputBuffer.Substring( 0, curDelimIdx ) );
                                    }
                                    curSqlStmt = curInputCmd.ToString();
                                    curSqlStmt.TrimStart( ' ' );
                                    if ( curSqlStmt.Trim().ToUpper().StartsWith( "DROP " ) ) {
                                        execDropTable( replaceLinefeed( curSqlStmt ) );
                                    } else if ( curSqlStmt.Trim().ToUpper().StartsWith( "CREATE " ) ) {
                                        execCreateTable( replaceLinefeed( curSqlStmt ) );
                                        curInputCmd = new StringBuilder( "" );
                                    } else {
                                        execSchemaCmd( replaceLinefeed( curSqlStmt ) );
                                    }
                                    curInputCmd = new StringBuilder( "" );

                                } else {
                                    curInputCmd.Append( inputBuffer );
                                }
                            }
                        }
                    }
                    curSqlStmt = "";
                    System.Data.SqlServerCe.SqlCeEngine mySqlEngine = new SqlCeEngine();
                    mySqlEngine.LocalConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                    mySqlEngine.Shrink();

                } catch ( Exception ex ) {
                    curReturn = false;
                    String ExcpMsg = ex.Message;
                    if ( mySqlStmt != null ) {
                        ExcpMsg += "\n" + curSqlStmt;
                    }
                    MessageBox.Show( "Error attempting to update database schema" + "\n\nError: " + ExcpMsg );
                }
            }
            #endregion

            myProgressInfo.Close();
            return curReturn;
        }

        private bool execDropTable( String inSqlStmt ) {
            bool curReturnValue = true;
            int curDelimIdx, curDelimIdx2, curValueLen;
            ExportData curExportData = new ExportData();
            String[] curTableName = new String[1];
            String[] curSelectCommand = new String[1];

            try {
                curDelimIdx = inSqlStmt.ToUpper().IndexOf( "TABLE " );
                if ( curDelimIdx > 0 ) {
                    curDelimIdx2 = inSqlStmt.IndexOf( " ", curDelimIdx + 6 );
                    if ( curDelimIdx2 < 0 ) {
                        curValueLen = inSqlStmt.Length - curDelimIdx - 6;
                    } else {
                        curValueLen = curDelimIdx2 - curDelimIdx - 6;
                    }
                    curTableName[0] = inSqlStmt.Substring( curDelimIdx + 6, curValueLen );
                    curSelectCommand[0] = "Select * from " + curTableName[0];
                    String curFileRef = Application.StartupPath + "\\" + curTableName[0] + ".tmp";

                    if ( curTableName[0].Trim().EndsWith( "Backup" ) ) {
                        execSchemaCmd( inSqlStmt );
                    } else {
                        if ( curExportData.exportData( curTableName, curSelectCommand, curFileRef ) ) {
                            execSchemaCmd( inSqlStmt );
                        }
                    }
                }
            } catch ( Exception ex ) {
                /*
                MessageBox.Show( "Error: Executing drop table command " + inSqlStmt
                    + "\n\nException: " + ex.Message
                 );
                 */
                curReturnValue = false;
            }
            return curReturnValue;
        }

        private bool execCreateTable( String inSqlStmt ) {
            bool curReturnValue = true;
            int curDelimIdx, curDelimIdx2, curValueLen;
            ImportData curImportData = new ImportData();
            String[] curTableName = new String[1];

            try {
                curDelimIdx = inSqlStmt.ToUpper().IndexOf( "TABLE " );
                if ( curDelimIdx > 0 ) {
                    curDelimIdx2 = inSqlStmt.IndexOf( " ", curDelimIdx + 6 );
                    if ( curDelimIdx2 < 0 ) {
                        curValueLen = inSqlStmt.Length - curDelimIdx - 6;
                    } else {
                        curValueLen = curDelimIdx2 - curDelimIdx - 6;
                    }
                    curTableName[0] = inSqlStmt.Substring( curDelimIdx + 6, curValueLen );
                    String curFileRef = Application.StartupPath + "\\" + curTableName[0] + ".tmp";

                    if ( execSchemaCmd( inSqlStmt ) ) {
                        if ( !( curTableName[0].EndsWith( "Backup" ) ) ) {
                            curImportData.importData( ( curFileRef ) );
                            deleteTempFile( curFileRef );
                        }
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Executing drop table command " + inSqlStmt
                    + "\n\nException: " + ex.Message
                 );
                curReturnValue = false;
            }
            return curReturnValue;

        }

        private bool execSchemaCmd( String inSqlStmt ) {
            bool curReturnValue = true;
            try {
                if ( openDbConn() ) {
                    mySqlStmt = myDbConn.CreateCommand();
                    mySqlStmt.CommandText = inSqlStmt;
                    int rowsProc = mySqlStmt.ExecuteNonQuery();
                }
            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n\n" + mySqlStmt.CommandText;
                    if ( inSqlStmt.Trim().ToLower().StartsWith( "drop table" ) ) {
                        //Skip exception message for drop statements
                    } else {
                        MessageBox.Show( "Error: execSchemaCmd"
                            + "\n\nSqlStmt: " + inSqlStmt
                            + "\n\nError: " + ExcpMsg
                            );
                    }
                }
            } finally {
                if ( myDbConn != null ) {
                    myDbConn.Close();
                }
            }
            return curReturnValue;
        }

        private bool deleteTempFile( String inFileName ) {
            try {
                //Declare and instantiate a new process component.
                System.Diagnostics.Process curOSProcess = new System.Diagnostics.Process();

                //Do not receive an event when the process exits.
                curOSProcess.EnableRaisingEvents = true;

                //The "/C" Tells Windows to Run The Command then Terminate 
                string curCmdLine;
                curCmdLine = "/C DEL \"" + inFileName + "\" \n";
                System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
                curOSProcess.Close();
                return true;
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Deleting temp file " + inFileName
                    + "\n\nException: " + ex.Message
                 );
                return false;
            }
        }

        private bool copyDatabaseFile() {
            bool curReturn = false;
            int delimPos = 0;
            String curDataDirectory = "", curFileName = null, curDatabaseFileName = "", curSourDir = "";
            String curDestFileName = "", curDestDatabaseRef = "";

            try {
                String curAppConnectString = Properties.Settings.Default.waterskiConnectionStringApp;
                String myConnectName = WaterskiScoringSystem.Properties.Settings.Default.AppConnectName;
                String curAppRegName = Properties.Settings.Default.AppRegistryName;
                RegistryKey curAppRegKey = Registry.CurrentUser.OpenSubKey( curAppRegName, true );
                if ( curAppRegKey.GetValue( "DataDirectory" ) == null ) {
                    try {
                        curDataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
                    } catch ( Exception ex ) {
                        curDataDirectory = Application.UserAppDataPath;
                    }
                } else {
                    curDataDirectory = curAppRegKey.GetValue( "DataDirectory" ).ToString();
                }

                String curAttrName = "", curAttrValue = "";
                String[] curAttrEntry;
                String[] curConnAttrList = curAppConnectString.Split( ';' );

                for ( int idx = 0; idx < curConnAttrList.Length; idx++ ) {
                    curAttrEntry = curConnAttrList[idx].Split( '=' );
                    curAttrName = curAttrEntry[0];
                    curAttrValue = curAttrEntry[1];
                    if ( curAttrName.ToLower().Trim().Equals( "data source" ) ) {
                        delimPos = curAttrValue.LastIndexOf( '\\' );
                        if ( delimPos > 0 ) {
                            curDatabaseFileName = curAttrValue.Substring( delimPos + 1 );
                            curDestFileName = curDatabaseFileName + ".bak";
                        }
                    }
                }

                if ( curDatabaseFileName.Length > 0 ) {
                    if ( curDataDirectory.Substring( curDataDirectory.Length - 1 ).Equals( "\\" ) ) {
                    } else {
                        curDataDirectory += "\\";
                    }

                    //Declare and instantiate a new process component.
                    System.Diagnostics.Process curOSProcess = new System.Diagnostics.Process();

                    //Do not receive an event when the process exits.
                    curOSProcess.EnableRaisingEvents = true;

                    //The "/C" Tells Windows to Run The Command then Terminate 
                    string curCmdLine;
                    curCmdLine = "/C copy \"" + curDataDirectory + curDatabaseFileName + "\" \"" + curDataDirectory + curDestFileName + "\" \n";
                    System.Diagnostics.Process.Start( "CMD.exe", curCmdLine );
                    curOSProcess.Close();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error backing up current database file " + "\n\nError: " + ex.Message );
            }
            return curReturn;
        }

        private String replaceLinefeed( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
        }

        private bool openDbConn() {
            bool curReturnValue = true;
            try {
                if ( myDbConn == null ) {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                    myDbConn.Open();
                } else {
                    String curState = myDbConn.State.ToString();
                    if ( curState.ToUpper().Equals( "CLOSED" ) ) {
                        myDbConn.Open();
                    }
                }
            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                MessageBox.Show( "Error connecting to database " + "\n\nError: " + ExcpMsg );
            }
            return curReturnValue;
        }

        private StreamReader getImportFile() {
            String myFileName, curPath;
            OpenFileDialog myFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
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
                            while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                                curInputLineCount++;
                            }
                            myReader.Close();
                            myProgressInfo.setProgressMin( 1 );
                            myProgressInfo.setProgressMax( curInputLineCount );
                        } catch ( Exception ex ) {
                            MessageBox.Show( "Error: Could not read file" + myFileDialog.FileName + "\n\nError: " + ex.Message );
                            return null;
                        }
                        myReader = new StreamReader( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
            }

            return myReader;
        }

        private StreamReader getImportFile( String inFileName ) {
            StreamReader myReader = null;
            try {
                if ( inFileName != null ) {
                    myReader = new StreamReader( inFileName );
                    try {
                        myProgressInfo.setProgessMsg( "File selected " + inFileName );
                        myProgressInfo.Show();
                        myProgressInfo.Refresh();

                        string inputBuffer = "";
                        int curInputLineCount = 0;
                        myReader = new StreamReader( inFileName );
                        while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                            curInputLineCount++;
                        }
                        myReader.Close();
                        myProgressInfo.setProgressMin( 1 );
                        myProgressInfo.setProgressMax( curInputLineCount );
                    } catch ( Exception ex ) {
                        MessageBox.Show( "Error: Could not read file" + inFileName + "\n\nError: " + ex.Message );
                        return null;
                    }
                    myReader = new StreamReader( inFileName );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
            }

            return myReader;
        }

    }
}
