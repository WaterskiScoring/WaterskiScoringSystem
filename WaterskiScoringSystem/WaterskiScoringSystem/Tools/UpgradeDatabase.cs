using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Deployment.Application;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class UpgradeDatabase {
        private String mySanctionNum;
        private String myNewVersionStmt;
        private decimal myDatabaseVersion = 0.00M;
        private ProgressWindow myProgressInfo;

		public static DatabaseUpgradeRemovedColumn[] removedColumns = new DatabaseUpgradeRemovedColumn[] {
			new DatabaseUpgradeRemovedColumn("TimeInTol1", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("TimeInTol2", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("TimeInTol3", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("BoatSplitTimeTol", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("BoatSplitTime2Tol", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("BoatEndTimeTol", "JumpRecap")
			, new DatabaseUpgradeRemovedColumn("Pass1VideoUrl", "TrickScore")
			, new DatabaseUpgradeRemovedColumn("Pass2VideoUrl", "TrickScore")
			, new DatabaseUpgradeRemovedColumn("FinalPassNum", "SlalomScore")
			, new DatabaseUpgradeRemovedColumn("PassNum", "SlalomRecap")
		};

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
            try {
                myNewVersionStmt = "'DatabaseVersion', 'Version', '23.04', 23.04, 1";

                Decimal curVersion = Convert.ToDecimal( myNewVersionStmt.Split( ',' )[3] );
				if ( myDatabaseVersion >= curVersion ) return true;

                copyDatabaseFile();
                
				if (myDatabaseVersion < 22.90M ) {
                    if ( DataAccess.DataAccessOpen() ) {
                        String curFileRef = Application.StartupPath + "\\DatabaseSchemaUpdates.sql";
                        updateSchemaUpgrade( curFileRef );
                    }
                }

				if ( myDatabaseVersion < 23.04M ) {
					if ( DataAccess.DataAccessOpen() ) {
						loadListValues();
					}
				}

				if ( myDatabaseVersion < 23.03M ) {
                    if ( DataAccess.DataAccessOpen() ) {
                        loadTrickList();
                    }
                }

                if (myDatabaseVersion < 23.01M ) {
                    if ( DataAccess.DataAccessOpen() ) {
                        loadNopsData();
                    }
                }

                if (myDatabaseVersion < curVersion ) {
                    if (DataAccess.DataAccessOpen()) {
                        addNewData();
                    }
                }

				return true;

            } catch ( Exception ex ) {
                String ExcpMsg = ex.Message;
                MessageBox.Show( "Error: Performing SQL operations" + "\n\nError: " + ExcpMsg );
				return false;
            }
        }

        private bool addNewData() {
            int rowsProc = 0;
			StringBuilder curSqltStmt = new StringBuilder( "" );

			try {
				// Update database version
				curSqltStmt.Append("Delete CodeValueList Where ListName = 'DatabaseVersion'");
				rowsProc = DataAccess.ExecuteCommand( curSqltStmt.ToString() );
				
				curSqltStmt = new StringBuilder( "" );
				curSqltStmt.Append( "Insert CodeValueList (" );
				curSqltStmt.Append( " ListName, ListCode, CodeValue, MinValue, SortSeq " );
				curSqltStmt.Append(String.Format( ") Values ( {0} )", myNewVersionStmt ) );
				rowsProc = DataAccess.ExecuteCommand( curSqltStmt.ToString() );
				return true;

			} catch ( Exception ex ) {
                String ExcpMsg = ex.Message;
                if ( curSqltStmt != null ) ExcpMsg += "\n" + curSqltStmt.ToString();
				MessageBox.Show( "addNewData: Exception encountered:" + "\n\nError: " + ExcpMsg );
				return false;
            }
        }

		// Insert current NOPS values
		private bool loadNopsData() {
			StringBuilder curSqltStmt = new StringBuilder( "" );
			int rowsProc = 0;

            try {
				curSqltStmt.Append("Delete NopsData");
				rowsProc = DataAccess.ExecuteCommand( curSqltStmt.ToString() );

				String curFileRef = Application.StartupPath + "\\NopsData.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );
				
				return true;

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( curSqltStmt != null ) ExcpMsg += "\n" + curSqltStmt.ToString();
				MessageBox.Show( "loadNopsData: Exception encountered:" + "\n\nError: " + ExcpMsg );
				return false;
			}
		}

		// Insert current NOPS values
		private bool loadTrickList() {
			StringBuilder curSqltStmt = new StringBuilder( "" );
			int rowsProc = 0;

            try {
				curSqltStmt.Append( "Delete TrickList" );
				rowsProc = DataAccess.ExecuteCommand( curSqltStmt.ToString() );

                String curFileRef = Application.StartupPath + "\\TrickList.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );
				return true;

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( curSqltStmt != null ) ExcpMsg += "\n" + curSqltStmt.ToString();
				MessageBox.Show( "loadTrickList: Exception encountered:" + "\n\nError: " + ExcpMsg );
				return false;
			}

        }

        private bool loadListValues() {
			StringBuilder curSqltStmt = new StringBuilder( "" );
			int rowsProc = 0;

			try {
				curSqltStmt.Append( "Delete CodeValueList" );
				rowsProc = DataAccess.ExecuteCommand( curSqltStmt.ToString() );

                String curFileRef = Application.StartupPath + "\\CodeValueLists.txt";
                ImportData myImportData = new ImportData();
                myImportData.importData( curFileRef );
				return true;

			} catch ( Exception ex ) {
				MessageBox.Show( "loadListValues: Exception encountered:" + "\n\nError: " + ex.Message );
				return false;
			}
        }

		/*
		 * Update the database schema using commands provided
		 * Commands processed is determined based on current database version
		 */
		private bool updateSchemaUpgrade(String inFileRef) {
            int curDelimIdx;
            decimal curDatabaseVersion = 0.00M;
            String inputBuffer, curSqlStmt = "";
            StringBuilder curInputCmd = new StringBuilder("");
            
			ImportData curImportData = new ImportData();
			StreamReader myReader = null;
            myProgressInfo = new ProgressWindow();

			try {
				myReader = getImportFile( inFileRef );
				if ( myReader == null ) return false;
				int curInputLineCount = 0;

				MessageBox.Show( "Your database is about to be upgraded.  Please click OK or continue to any dialogs." );

				while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
					curInputLineCount++;
					myProgressInfo.setProgressValue( curInputLineCount );

					if ( inputBuffer.TrimStart( ' ' ).StartsWith( "## " ) ) {
						curDatabaseVersion = Convert.ToDecimal( inputBuffer.Substring( 4 ) );
					}
					
					if ( inputBuffer.TrimStart( ' ' ).StartsWith( "//" ) || inputBuffer.TrimStart( ' ' ).StartsWith( "##" ) ) continue;
					if ( curDatabaseVersion <= myDatabaseVersion ) continue;

					curDelimIdx = inputBuffer.IndexOf( ';' );
					if ( curDelimIdx >= 0 ) {
						if ( curDelimIdx > 0 ) curInputCmd.Append( inputBuffer.Substring( 0, curDelimIdx ) );
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

				curSqlStmt = "";
				SqlCeEngine mySqlEngine = new SqlCeEngine();
				mySqlEngine.LocalConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
				mySqlEngine.Shrink();

				return true;

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( curInputCmd != null ) ExcpMsg += "\n" + curInputCmd.ToString();
				MessageBox.Show( "updateSchemaUpgrade: Exception encountered:" + "\n\nError: " + ExcpMsg );
				return false;
			
			} finally {
				myProgressInfo.Close();
				if ( myReader != null ) myReader.Close();
			}
		}

        private bool execDropTable( String inSqlStmt ) {
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
				return true;

            } catch {
                return false;
            }
        }

        private bool execCreateTable( String inSqlStmt ) {
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
				return true;
			
			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( inSqlStmt.Length > 0 ) ExcpMsg += "\n" + inSqlStmt;
				MessageBox.Show( "execCreateTable: Exception encountered:" + "\n\nError: " + ExcpMsg );
				return false;
			}
        }

        private bool execSchemaCmd( String inSqlStmt ) {
            try {
				int rowsProc = DataAccess.ExecuteCommand( inSqlStmt );
				return true;
            
			} catch ( Exception ex ) {
				if ( inSqlStmt.Trim().ToLower().StartsWith( "drop table" ) ) return false;

				MessageBox.Show( "execSchemaCmd: Exception encountered: "
					+ "\n\nSqlStmt: " + inSqlStmt
					+ "\n\nError: " + ex.Message );
				return false;
            }
        }

        private bool deleteTempFile( String inFileName ) {
			String curMethodName = "UpgradeDatabase: deleteTempFile: ";
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
				Log.WriteFile( String.Format( "{0}Error: CDeleting temp file {1}\n\nError: {2}"
					, curMethodName, inFileName, ex.Message ) );
				return false;
            }
        }

        private bool copyDatabaseFile() {
            bool curReturn = false;
            int delimPos = 0;
            String curDataDirectory = "", curDatabaseFileName = "", curDestFileName = "";

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
			String curMethodName = "UpgradeDatabase: getImportFile: ";
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
						Log.WriteFile( String.Format( "{0}Error: Could not read file {1}\n\nError: {2}"
							, curMethodName, inFileName, ex.Message ) );
                        return null;
                    }
                    myReader = new StreamReader( inFileName );
                }
            
			} catch ( Exception ex ) {
				Log.WriteFile( String.Format( "{0}Error: Unable to access or read input file {1}\n\nError: {2}"
					, curMethodName, inFileName, ex.Message ) );
				return null;
			}

			return myReader;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }

	class DatabaseUpgradeRemovedColumn {
		private string colName;
		private string tableName;

		public DatabaseUpgradeRemovedColumn( string inColName, string inTableName ) {
			colName = inColName;
			tableName = inTableName;
		}

		public string ColName { get => colName.ToLower(); set => colName = value; }
		public string TableName { get => tableName.ToLower(); set => tableName =  value ; }
	}
}
