using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class DatabaseTools {
        private String mySanctionNum;
        private decimal myDatabaseVersion = 0.00M;
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
                                        DataAccess.ExecuteCommand( curSqlStmt );
                                    }
                                    curInputCmd = new StringBuilder( "" );

                                } else {
                                    curInputCmd.Append( inputBuffer );
                                }
                            }
                        }
                    }
                    curSqlStmt = "";

                } catch ( Exception ex ) {
                    curReturn = false;
                    String ExcpMsg = ex.Message;
                    if ( curSqlStmt != null ) {
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
                    String curFileRef = Path.Combine( Application.StartupPath, curTableName[0] + ".tmp" );
                    if ( curTableName[0].Trim().EndsWith( "Backup" ) ) {
                        DataAccess.ExecuteCommand( inSqlStmt );
                    } else {
                        if ( curExportData.exportData( curTableName, curSelectCommand, curFileRef ) ) {
                            DataAccess.ExecuteCommand( inSqlStmt );
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
                    String curFileRef = Path.Combine( Application.StartupPath, curTableName[0] + ".tmp" );

                    if ( DataAccess.ExecuteCommand( inSqlStmt ) > 0 ) {
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

        private bool deleteTempFile( String inFileName ) {
            try {
                File.Delete( inFileName );
                return true;
            
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Deleting temp file " + inFileName + "\n\nException: " + ex.Message );
                return false;
            }
        }

        private String replaceLinefeed( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
        }

        private StreamReader getImportFile() {
            String curFileName, curPath;
            OpenFileDialog curFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) curPath = Directory.GetCurrentDirectory();
                curFileDialog.InitialDirectory = curPath;
                curFileDialog.Filter = "SQL files (*.sql)|*.sql|All files (*.*)|*.*";
                curFileDialog.FilterIndex = 2;
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        myReader = new StreamReader( curFileName );
                        try {
                            myProgressInfo.setProgessMsg( "File selected " + curFileName );
                            myProgressInfo.Show();
                            myProgressInfo.Refresh();

                            string inputBuffer = "";
                            int curInputLineCount = 0;
                            myReader = new StreamReader( curFileName );
                            while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
                                curInputLineCount++;
                            }
                            myReader.Close();
                            myProgressInfo.setProgressMin( 1 );
                            myProgressInfo.setProgressMax( curInputLineCount );
                        
                        } catch ( Exception ex ) {
                            MessageBox.Show( "Error: Could not read file" + curFileDialog.FileName + "\n\nError: " + ex.Message );
                            return null;
                        }
                        myReader = new StreamReader( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
            }

            return myReader;
        }

    }
}
