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
    class UpgradeDatabase {
        private String mySanctionNum;
        private String myNewVersionStmt;
        private decimal myDatabaseVersion = 0.00M;
        private SqlCeConnection myDbConn = null;
        private SqlCeCommand mySqlStmt = null;
        private ProgressWindow myProgressInfo;

        public UpgradeDatabase() {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            String curSqlStmt = "SELECT ListName, ListCode, CodeValue as VersionNumText, MinValue as VersionNum "
                + "FROM CodeValueList WHERE ListName = 'DatabaseVersion'";
            DataTable curDataTable = getData( curSqlStmt );
            if ( curDataTable.Rows.Count > 0 ) {
                myDatabaseVersion = (decimal)curDataTable.Rows[0]["VersionNum"];
            } else {
                myDatabaseVersion = 19.0M;
            }
        }

        public bool checkForUpgrade() {
            bool curReturnValue = true;

            try {
                myNewVersionStmt = "'DatabaseVersion', 'Version', '22.01', 22.01, 1";

                Decimal curVersion = Convert.ToDecimal( myNewVersionStmt.Split( ',' )[3] );
                if ( myDatabaseVersion < curVersion ) {
                    copyDatabaseFile();
                }
                if (myDatabaseVersion < 22.01M ) {
                    if (openDbConn()) {
                        String curFileRef = Application.StartupPath + "\\DatabaseSchemaUpdates.sql";
                        updateSchemaUpgrade( curFileRef );
                    }
                }
                if (myDatabaseVersion < 21.01M ) {
                    if ( openDbConn() ) {
                        loadTrickList();
                    }
                }
                if (myDatabaseVersion < 21.21M) {
                    if ( openDbConn() ) {
                        loadNopsData();
                    }
                }
                if ( myDatabaseVersion < 21.27M ) {
                    if ( openDbConn() ) {
                        loadListValues();
                    }
                }
                if (myDatabaseVersion < curVersion ) {
                    if (openDbConn()) {
                        addNewData();
                    }
                }
            } catch ( Exception ex ) {
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error: Performing SQL operations"
                    + "\n\nError: " + ExcpMsg
                    );
            } finally {
                if ( myDbConn != null ) {
                    myDbConn.Close();
                }
            }

            return curReturnValue;
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

        private bool addNewData() {
            bool curReturnValue = true;
            int rowsProc = 0;

            try {
                mySqlStmt = myDbConn.CreateCommand();

                #region Update database version
                mySqlStmt.CommandText = "Delete CodeValueList Where ListName = 'DatabaseVersion'";
                rowsProc = mySqlStmt.ExecuteNonQuery();
                mySqlStmt.CommandText = "Insert CodeValueList ("
                    + " ListName, ListCode, CodeValue, MinValue, SortSeq "
                    + ") Values ("
                    + myNewVersionStmt + ")";
                rowsProc = mySqlStmt.ExecuteNonQuery();
                #endregion

            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error during addVerion optionation" + "\n\nError: " + ExcpMsg );
            }

            return curReturnValue;
        }

        private bool loadNopsData() {
            bool curReturnValue = true;
            int rowsProc = 0;

            try {
                #region Insert current NOPS values
                mySqlStmt = myDbConn.CreateCommand();

                mySqlStmt.CommandText = "Delete NopsData";
                rowsProc = mySqlStmt.ExecuteNonQuery();

                String curFileRef = Application.StartupPath + "\\NopsData.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );

                #endregion
            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error during addVerion optionation" + "\n\nError: " + ExcpMsg );
            }

            return curReturnValue;
        }

        private bool loadTrickList() {
            bool curReturnValue = true;
            int rowsProc = 0;

            try {
                #region Insert current NOPS values
                mySqlStmt = myDbConn.CreateCommand();

                mySqlStmt.CommandText = "Delete TrickList";
                rowsProc = mySqlStmt.ExecuteNonQuery();

                String curFileRef = Application.StartupPath + "\\TrickList.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );

                #endregion
            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error during addVerion optionation" + "\n\nError: " + ExcpMsg );
            }

            return curReturnValue;
        }

        private bool loadListValues() {
            bool curReturnValue = true;
            int rowsProc = 0;

            try {
                mySqlStmt = myDbConn.CreateCommand();

                mySqlStmt.CommandText = "Delete CodeValueList";
                rowsProc = mySqlStmt.ExecuteNonQuery();

                String curFileRef = Application.StartupPath + "\\CodeValueLists.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );

            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error during addVerion optionation" + "\n\nError: " + ExcpMsg );
            }

            return curReturnValue;
        }

        private bool updateSchemaUpgrade(String inFileRef) {
            bool curReturnValue = true;
            int curDelimIdx;
            decimal curDatabaseVersion = 0.00M;
            String inputBuffer, curSqlStmt = "";
            StringBuilder curInputCmd = new StringBuilder("");
            ImportData curImportData = new ImportData();
            StreamReader myReader;
            myProgressInfo = new ProgressWindow();

            try {
                #region Process all commands in the input file
                myReader = getImportFile(inFileRef);
                if (myReader != null) {
                    int curInputLineCount = 0;
                    try {
                        MessageBox.Show("Your database is about to be upgraded.  Please click OK or continue to any dialogs.");

                        while ((inputBuffer = myReader.ReadLine()) != null) {
                            curInputLineCount++;
                            myProgressInfo.setProgressValue(curInputLineCount);

                            if (inputBuffer.TrimStart(' ').StartsWith("## ")) {
                                curDatabaseVersion = Convert.ToDecimal(inputBuffer.Substring(4));
                            }
                            if (inputBuffer.TrimStart(' ').StartsWith("//") || inputBuffer.TrimStart(' ').StartsWith("##")) {
                            } else {
                                if (curDatabaseVersion > myDatabaseVersion) {
                                    curDelimIdx = inputBuffer.IndexOf(';');
                                    if (curDelimIdx >= 0) {
                                        if (curDelimIdx > 0) {
                                            curInputCmd.Append(inputBuffer.Substring(0, curDelimIdx));
                                        }
                                        curSqlStmt = curInputCmd.ToString();
                                        curSqlStmt.TrimStart(' ');
                                        if (curSqlStmt.Trim().ToUpper().StartsWith("DROP ")) {
                                            execDropTable(replaceLinefeed(curSqlStmt));
                                        } else if (curSqlStmt.Trim().ToUpper().StartsWith("CREATE ")) {
                                            execCreateTable(replaceLinefeed(curSqlStmt));
                                            curInputCmd = new StringBuilder("");
                                        } else {
                                            execSchemaCmd(replaceLinefeed(curSqlStmt));
                                        }
                                        curInputCmd = new StringBuilder("");

                                    } else {
                                        curInputCmd.Append(inputBuffer);
                                    }
                                }
                            }
                        }

                        curSqlStmt = "";
                        System.Data.SqlServerCe.SqlCeEngine mySqlEngine = new SqlCeEngine();
                        mySqlEngine.LocalConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                        mySqlEngine.Shrink();

                    } catch (Exception ex) {
                        curReturnValue = false;
                        String ExcpMsg = ex.Message;
                        if (mySqlStmt != null) {
                            ExcpMsg += "\n" + curSqlStmt;
                        }
                        MessageBox.Show("Error attempting to update database schema" + "\n\nError: " + ExcpMsg);
                    }
                }
                #endregion

            } catch (Exception ex) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if (mySqlStmt != null) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show("Error attempting to update database schema" + "\n\nError: " + ExcpMsg);
            }
            myProgressInfo.Close();

            return curReturnValue;
        }

        private bool updateSchema(String inFileRef) {
            bool curReturnValue = true;
            int curDelimIdx;
            String inputBuffer, curSqlStmt = "";
            StringBuilder curInputCmd = new StringBuilder( "" );
            ImportData curImportData = new ImportData();
            StreamReader myReader;
            myProgressInfo = new ProgressWindow();

            try {
                #region Process all commands in the input file
                myReader = getImportFile( inFileRef );
                if ( myReader != null ) {
                    int curInputLineCount = 0;
                    try {
                        MessageBox.Show( "Your database is about to be upgraded.  Please click OK or continue to any dialogs." );

                        while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                            curInputLineCount++;
                            myProgressInfo.setProgressValue( curInputLineCount );

                            if ( inputBuffer.TrimStart( ' ' ).StartsWith( "//" ) ) {
                            } else {
                                curDelimIdx = inputBuffer.IndexOf( ';' );
                                if ( curDelimIdx >= 0 ) {
                                    if ( curDelimIdx > 0 ) {
                                        curInputCmd.Append( inputBuffer.Substring( 0, curDelimIdx ) );
                                    }
                                    curSqlStmt = curInputCmd.ToString();
                                    curSqlStmt.TrimStart( ' ' );
                                    if ( curSqlStmt.Trim().ToUpper().StartsWith( "DROP " ) ) {
                                        execDropTable( replaceLinefeed(curSqlStmt) );
                                    } else if ( curSqlStmt.Trim().ToUpper().StartsWith( "CREATE " ) ) {
                                        execCreateTable( replaceLinefeed(curSqlStmt) );
                                        curInputCmd = new StringBuilder( "" );
                                    } else {
                                        execSchemaCmd( replaceLinefeed(curSqlStmt) );
                                    }
                                    curInputCmd = new StringBuilder( "" );

                                } else {
                                    curInputCmd.Append( inputBuffer );
                                }
                            }

                        }
                        curSqlStmt = "";
                        System.Data.SqlServerCe.SqlCeEngine mySqlEngine = new SqlCeEngine();
                        mySqlEngine.LocalConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                        mySqlEngine.Shrink();

                    } catch ( Exception ex ) {
                        curReturnValue = false;
                        String ExcpMsg = ex.Message;
                        if ( mySqlStmt != null ) {
                            ExcpMsg += "\n" + curSqlStmt;
                        }
                        MessageBox.Show( "Error attempting to update database schema" + "\n\nError: " + ExcpMsg );
                    }
                }
                #endregion

            } catch ( Exception ex ) {
                curReturnValue = false;
                String ExcpMsg = ex.Message;
                if ( mySqlStmt != null ) {
                    ExcpMsg += "\n" + mySqlStmt.CommandText;
                }
                MessageBox.Show( "Error attempting to update database schema" + "\n\nError: " + ExcpMsg );
            }
            myProgressInfo.Close();

            return curReturnValue;
        }

        private bool updateTourRegCity() {
            bool curReturnValue = true;
            int rowsProc = 0, curRowsUpdate = 0;
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append( "SELECT Distinct MemberList.MemberId, MemberList.UpdateDate, MemberList.City FROM MemberList " );
            curSqlStmt.Append( "Inner Join TourReg ON MemberList.MemberId = TourReg.MemberId ");
            curSqlStmt.Append( "Where MemberList.City is not null " );
            curSqlStmt.Append( "Order by MemberList.MemberId, MemberList.UpdateDate" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                if ( openDbConn() ) {
                    try {
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update TourReg Set City = '" + (String)curRow["City"] + "' " );
                            curSqlStmt.Append( "Where MemberId = '" + (String)curRow["MemberId"] + "' " );

                            mySqlStmt = myDbConn.CreateCommand();
                            mySqlStmt.CommandText = curSqlStmt.ToString();
                            rowsProc = mySqlStmt.ExecuteNonQuery();
                            curRowsUpdate += rowsProc;
                        }
                    } catch (Exception ex) {
                        curReturnValue = false;
                        if (mySqlStmt == null) {
                        } else {
                            MessageBox.Show( "Error: updateTourRegCity"
                                + "\n\nSqlStmt: " + curSqlStmt.ToString()
                                + "\n\nError: " + ex.Message
                                );
                        }
                    }
                    MessageBox.Show( "Total records updated " + curRowsUpdate );
                } else {
                    curReturnValue = false;
                }
            } else {
                curReturnValue = false;
            }

            return curReturnValue;
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

                    if (execSchemaCmd( inSqlStmt ) ) {
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
                            + "\n\nError: " + ExcpMsg );
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

                try {
                    curDataDirectory = ApplicationDeployment.CurrentDeployment.DataDirectory;
                    if (curDataDirectory == null) {
                        if (curAppRegKey.GetValue( "DataDirectory" ) == null) {
                            curDataDirectory = Application.UserAppDataPath;
                        } else {
                            curDataDirectory = curAppRegKey.GetValue( "DataDirectory" ).ToString();
                        }
                    } else if (curDataDirectory.Length > 3) {
                        //Use value found
                    } else {
                        if (curAppRegKey.GetValue( "DataDirectory" ) == null) {
                            curDataDirectory = Application.UserAppDataPath;
                        } else {
                            curDataDirectory = curAppRegKey.GetValue( "DataDirectory" ).ToString();
                        }
                    }
                } catch {
                    if (curAppRegKey.GetValue( "DataDirectory" ) == null) {
                        curDataDirectory = Application.UserAppDataPath;
                    } else {
                        curDataDirectory = curAppRegKey.GetValue( "DataDirectory" ).ToString();
                    }
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
                            curDestFileName = curDatabaseFileName + "." + DateTime.Now.ToString( "yyyyMMddHHmm") + ".bak";
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
                MessageBox.Show( "Error: Unable to access or read input file " + inFileName + "\n\nError: " + ex.Message );
            }

            return myReader;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
