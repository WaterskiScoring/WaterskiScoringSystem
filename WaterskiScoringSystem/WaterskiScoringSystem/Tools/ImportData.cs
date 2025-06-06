﻿using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ImportData {
        private String mySanctionNum;
        private String myTableName = null;
		private String myMatchCommand = "";
		private String[] myColumnSkip = { "tournament:tourdataloc" };
		
		private int idxRowsRead = 0, idxRowsFound = 1, idxRowsAdded = 2, idxRowsUpdated = 3, idxRowsSkipped = 4;
		private int[] myImportCounts = new int[] { 0, 0, 0, 0, 0 };

		private ImportMatchDialogForm MatchDialog;
        private ProgressWindow myProgressInfo;

		public ImportData() {
            MatchDialog = new ImportMatchDialogForm();

			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) mySanctionNum = "";
			else if ( mySanctionNum.Length < 6 ) mySanctionNum = "";
		}

		public String TableName {
            get {
                return myTableName;
            }
            set {
                myTableName = value;
            }
        }

        private string findColValue( string inColName, string[] inputColNames, string[] inputCols ) {
            int idx = 0;
            foreach ( string colName in inputColNames ) {
                if ( inColName.ToLower().Equals( colName.ToLower() ) ) {
                    return inputCols[idx].ToString();
                }
                idx++;
            }
            return null;
        }

        private String[] getTableKeys( String inTableName ) {
            String[] inputKeys = null;
            if ( inTableName.ToLower().Equals( "memberlist" ) ) {
                inputKeys = new String[1];
                inputKeys[0] = "MemberId";
            } else {
                String selectStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS"
                    + " WHERE TABLE_NAME = '" + inTableName + "' AND IS_NULLABLE = 'NO' And COLUMN_NAME != 'PK'"
                    + " ORDER BY COLUMN_NAME";
				DataTable curDataTable = DataAccess.getDataTable( selectStmt );
                if ( curDataTable != null ) {
                    int idx = 0;
                    inputKeys = new String[curDataTable.Rows.Count];
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        inputKeys[idx] = (String)curRow["COLUMN_NAME"];
                        idx++;
                    }
                }
            }
            return inputKeys;
        }

        private String[] getTableColumns( String inTableName ) {
            String[] inputColumns = null;
            String selectStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS"
                + " WHERE TABLE_NAME = '" + inTableName + "' And COLUMN_NAME != 'PK'"
                + " ORDER BY COLUMN_NAME";
            DataTable curDataTable = DataAccess.getDataTable( selectStmt );
            if ( curDataTable != null ) {
                int idx = 0;
                inputColumns = new String[curDataTable.Rows.Count];
                foreach ( DataRow curRow in curDataTable.Rows ) {
                    inputColumns[idx] = (String)curRow["COLUMN_NAME"];
                    idx++;
                }
            }
            return inputColumns;
        }

        public void importData() {
            importData( null );
        }

        public void importData(String inFileName) {
            ArrayList curFileList = new ArrayList();
			myMatchCommand = "";

			if ( inFileName == null ) {
                curFileList = getImportFileList();
            } else {
                curFileList.Add( inFileName );
            }

			if ( curFileList.Count == 0 ) return;
		
			foreach ( String curFileName in curFileList ) {
				importDataFile( curFileName );
			}
		}

		public void importDataFile( String curFileName ) {
			string inputBuffer, curSanctionId = "";
			string[] inputCols = null, inputColNames = null, inputKeys = null;
			int curInputLineCount = 0;
			StreamReader curReader = null;
			myProgressInfo = new ProgressWindow();

			if ( curFileName ==null || curFileName.Length == 0 ) return;

			curReader = getImportFile( curFileName );
			if ( curReader == null ) return;

			curInputLineCount = 0;
			while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
				curInputLineCount++;
				myProgressInfo.setProgressValue( curInputLineCount );

				inputCols = inputBuffer.Split( HelperFunctions.TabDelim );

                if ( inputCols[0].ToLower().Equals( "table:" ) || inputCols[0].ToLower().Equals( "tablename:" ) ) {
                    //Display statistics when another table entry is found
                    if ( myTableName != null ) {
                        handleEndOfTable( curSanctionId );
                        curSanctionId = "";
                    }

                    //Check for table name and assume all subsequent records are for this table
                    TableName = inputCols[1];
                    myProgressInfo.setProgessMsg( "Processing " + TableName );
                    myProgressInfo.Refresh();

                    inputColNames = null;
                    inputKeys = getTableKeys( TableName );

                } else if ( inputColNames == null ) {
                    //Column names are required and must preceed the data rows
                    inputColNames = new string[inputCols.Length];
                    for ( int idx = 0; idx < inputCols.Length; idx++ ) {
                        inputColNames[idx] = inputCols[idx];
                        //Check for column names that have changed or been deleted
                        if ( TableName.Equals( "OfficialWork" ) && inputCols[idx].ToLower().Equals( "techofficialrating" ) ) inputColNames[idx] = "TechControllerSlalomRating";
                    }

                } else {
                    /*
					 * Process data rows
					 * Table name and column names are required to process data rows
					 */
                    if ( myTableName == null ) {
                        MessageBox.Show( "Error: Table name not provide.  Unable to process import file." );
                        break;

                    } else if ( inputColNames == null ) {
                        MessageBox.Show( "Error: Column definitions not provide.  Unable to process import file." );
                        break;

                    } else {
                        myImportCounts[idxRowsRead]++;

                        String returnSanctionId = "";
                        DataRow curRow = null;
                        if ( inputKeys != null ) curRow = findRowExist( inputKeys, inputColNames, inputCols );

                        if ( curRow != null ) {
                            returnSanctionId = updateMatchedRow( inputKeys, inputColNames, inputCols, curRow );
                        } else {
                            returnSanctionId = addNewDataRow( inputKeys, inputColNames, inputCols );
                        }
                        if ( returnSanctionId.Length > 0 ) curSanctionId = returnSanctionId;

                        if ( myTableName.Equals( "Tournament" ) ) {
                            CheckTournamentTableNeedSplit( inputKeys, inputColNames, inputCols );
                        }


                    }
                }
            }

            handleEndOfTable( curSanctionId );
			showImportStats();
			
			myImportCounts = new int[] { 0, 0, 0, 0, 0 };
			myProgressInfo.Close();
			curReader.Close();
		}

        /*
		 * Special handling when importing the tournament table because at the start of 2025 I split the table 
		 * I moved all attributes related to the chief judges repport to its own table ChiefJudgeReport
		 */
        private void CheckTournamentTableNeedSplit( String[] inputKeys, String[] inputColNames, String[] inputCols ) {
            DataRow curRow = null;
            bool isNeedSplitTable = false;
			foreach ( String key in inputColNames ) {
				if ( key.Equals( "RuleExceptions" ) ) {
                    isNeedSplitTable = true;
					break;
                }
            }

			if ( isNeedSplitTable ) {
				string curTableNameSave = myTableName;
				myTableName = "ChiefJudgeReport";
                curRow = findRowExist( inputKeys, inputColNames, inputCols );
                if ( curRow != null ) {
                    updateMatchedRow( inputKeys, inputColNames, inputCols, curRow );
                } else {
                    addNewDataRow( inputKeys, inputColNames, inputCols );
                }
				myTableName = curTableNameSave;
            }
        }

        /*
		 * Use key column data items to see if input row already exists on database
		 * If available use the LastUpdateDate as a criteria (
		 */
        private DataRow findRowExist( String[] inputKeys, String[] inputColNames, String[] inputCols ) {
			String curColValue = "";
			bool isLastUpdateDateAvail = false;
			StringBuilder stmtSelect = new StringBuilder( "" );
			StringBuilder stmtWhere = new StringBuilder( "" );
			DataTable curDataTable = null;

			//Use update date if available
			curColValue = findColValue( "LastUpdateDate", inputColNames, inputCols );
			if ( curColValue != null ) isLastUpdateDateAvail = true;

			//Use key column data items to see if input row already exists on database
			foreach ( string keyName in inputKeys ) {
				curColValue = findColValue( keyName, inputColNames, inputCols );
				if ( curColValue == null ) curColValue = "";
				if ( stmtSelect.Length > 1 ) {
					stmtSelect.Append( ", " + keyName );
					stmtWhere.Append( String.Format( " AND {0} = '{1}'", keyName, curColValue ) );
				
				} else {
					stmtSelect.Append( "Select " );
					if ( isLastUpdateDateAvail ) stmtSelect.Append( "LastUpdateDate, " );
					stmtSelect.Append( keyName );
					stmtWhere.Append( String.Format( " Where {0} = '{1}'", keyName, curColValue ) );
				}
			}

			String sqlStmt = String.Format( "{0} From {1} {2}", stmtSelect.ToString(), myTableName, stmtWhere.ToString() );
			curDataTable = DataAccess.getDataTable( sqlStmt );
			if ( curDataTable == null ) {
				MessageBox.Show( "Error: Checking " + myTableName + " for import data" );
				return null;
			}

			if ( curDataTable.Rows.Count == 0 ) return null;

			myImportCounts[idxRowsFound]++;
			return curDataTable.Rows[0];
		}

		/*
		 * Show information if input data found on database
		 * Skip display if previoius display specfied to process all records the same
		 */
		private string updateMatchedRow( string[] inputKeys, string[] inputColNames, string[] inputCols, DataRow curRow ) {
			string curSanctionId = "", curMatchCommand = "", curColValue = "";
			string[] curImportDataMatchMsg = { "", "", "", "" };
			bool isLastUpdateDateAvail = false;
			DateTime curLastUpdateDate = Convert.ToDateTime("2000-01-01 00:00:00")
				, curLastUpdateDateIn = Convert.ToDateTime( "2000-01-01 00:00:00" );
			
			StringBuilder stmtSelect = new StringBuilder( "" );
			StringBuilder stmtWhere = new StringBuilder( "" );
			StringBuilder stmtData = new StringBuilder( "" );

			if ( myMatchCommand.ToLower().Equals( "skipall" ) ) {
				myImportCounts[idxRowsSkipped]++;
				return "";
			}

			/*
			 * Determine if LastUpdateDate is available for use
			 * If available then input data will be used to update database if more current as specified by LastUpdateDate
			 */
			curColValue = findColValue( "LastUpdateDate", inputColNames, inputCols );
			if ( curColValue != null && curColValue.Length > 0 ) {
				curLastUpdateDateIn = Convert.ToDateTime( curColValue );
				String curDateValue = HelperFunctions.getDataRowColValue( curRow, "LastUpdateDate", "" );
				if ( curDateValue.Length > 4 ) {
					isLastUpdateDateAvail = true;
					curLastUpdateDate = (DateTime)curRow["LastUpdateDate"];
				}
			}

			// Skip update if input data is not more current than database entry
			if ( isLastUpdateDateAvail && curLastUpdateDateIn <= curLastUpdateDate ) {
				myImportCounts[idxRowsSkipped]++;
				return "";
			}

			// Build clause use key column data items to specify what data row will be updated
			foreach ( string keyName in inputKeys ) {
				curColValue = findColValue( keyName, inputColNames, inputCols );
				if ( curColValue == null ) curColValue = "";
				if ( stmtWhere.Length > 1 ) {
					stmtWhere.Append( String.Format( " AND {0} = '{1}'", keyName, curColValue ) );

				} else {
					stmtWhere.Append( String.Format( " Where {0} = '{1}'", keyName, curColValue ) );
				}
			}

			if ( myMatchCommand.ToLower().Equals( "updateall" ) ) {
				// Skip display if previoius display specfied to process all records the same
				curMatchCommand = "update";

			} else {
				// Show information being updated and allow user to determine whether to update or skip
				curMatchCommand = "update";

				curImportDataMatchMsg[0] = "Table: " + myTableName;
				curImportDataMatchMsg[1] = stmtWhere.ToString();
				if ( isLastUpdateDateAvail ) {
					curImportDataMatchMsg[2] = "Current record date = " + curLastUpdateDate.ToString();
					curImportDataMatchMsg[3] = " Import record date = " + curLastUpdateDateIn;
				} else {
					curImportDataMatchMsg[2] = "";
					curImportDataMatchMsg[3] = "";
				}
				MatchDialog.ImportKeyDataMultiLine = curImportDataMatchMsg;
				MatchDialog.MatchCommand = curMatchCommand;
				if ( MatchDialog.ShowDialog() == DialogResult.OK ) curMatchCommand = MatchDialog.MatchCommand;
				
				if ( curMatchCommand.Equals( "updateall" ) ) {
					myMatchCommand = curMatchCommand;
					curMatchCommand = "update";

				} else if ( curMatchCommand.Equals( "skipall" ) ) {
					myMatchCommand = curMatchCommand;
					myImportCounts[idxRowsSkipped]++;
					return "";
				
				} else if ( curMatchCommand.Equals( "skip" ) ) {
					myImportCounts[idxRowsSkipped]++;
					return "";
				}
			}

            /*
			 * Loop thru the column names for the current table as defined by the system information
			 * If the column data is not available on the import file then skip the column from the insert statement.
			 * This assumes the attribute allows nulls
			 */
            string curStmtDelim = "";
            String[] curTableColumns = getTableColumns( myTableName );
            foreach ( string colName in curTableColumns ) {
				if ( myColumnSkip.Contains( myTableName.ToLower() + ":" + colName.ToLower() ) ) continue;
				//if ( myTableName.ToLower().Equals( "tournament" ) && colName.ToLower().Equals( "tourdataloc" ) ) continue;
                curColValue = findColValue( colName, inputColNames, inputCols );
                if ( curColValue == null ) continue; // If column is not available on the import record then bypass including in the update
                
                if ( colName.Trim().ToLower().Equals( "sanctionid" ) ) curSanctionId = curColValue;

                if ( HelperFunctions.isObjectPopulated( curColValue ) ) {
                    curColValue = String.Format( "'{0}'", HelperFunctions.stringReplace( curColValue, HelperFunctions.SingleQuoteDelim, "''" ) );
				} else if ( inputKeys.Contains( colName ) ) {
					curColValue = "''";
				} else {
					curColValue = "null";
				}
				stmtData.Append( String.Format( "{0}[{1}] = {2}", curStmtDelim, colName, curColValue ) );
				curStmtDelim = ", ";
            }

			//Update database with input record if specified
			//Delete detail if event scores which assumes the detail will also be imported
			if ( myTableName.ToLower().Equals( "slalomscore" ) ) {
				int rowsDeleted = DataAccess.ExecuteCommand( "Delete SlalomRecap " + stmtWhere.ToString() );

			} else if ( myTableName.ToLower().Equals( "trickscore" ) ) {
				int rowsDeleted = DataAccess.ExecuteCommand( "Delete TrickPass " + stmtWhere.ToString() );

			} else if ( myTableName.ToLower().Equals( "jumpscore" ) ) {
				int rowsDeleted = DataAccess.ExecuteCommand( "Delete JumpRecap " + stmtWhere.ToString() );
			}

			String sqlStmt = String.Format( "Update {0} Set {1} {2}", myTableName, stmtData.ToString(), stmtWhere.ToString() );
			int rowsProc = DataAccess.ExecuteCommand( sqlStmt );
			// Exit method if error is detected with last update statement (rowsProc less than zero indicate an exception was encountered)
			if ( rowsProc > 0 ) myImportCounts[idxRowsUpdated]++;

			return curSanctionId;
		}

		/*
		 * New data identified and will be added
		 * Database record does not exist therefore data is being added to database
		 * An insert command is build using available column names and column data
		 */
		private String addNewDataRow( string[] inputKeys, string[] inputColNames, string[] inputCols ) {
			String curSanctionId = "";
			StringBuilder stmtInsert = new StringBuilder( "" );
			StringBuilder stmtData = new StringBuilder( "" );

			/*
			 * Loop thru the column names for the current table as defined by the system information
			 * If the column data is not available on the import file then skip the column from the insert statement.
			 * This assumes the attribute allows nulls
			 */
			string curColValue;
			string curStmtDelim = "";
            String[] curTableColumns = getTableColumns( myTableName );
            foreach ( string colName in curTableColumns ) {
                curColValue = findColValue( colName, inputColNames, inputCols );
                if ( curColValue == null ) continue; // If column is not available on the import record then bypass including in the insert
                if ( colName.Trim().ToLower().Equals( "sanctionid" ) ) curSanctionId = curColValue;

				stmtInsert.Append( curStmtDelim + "[" + colName + "]" );

				String tempValue = "";
				if ( HelperFunctions.isObjectPopulated( curColValue ) ) {
					tempValue = String.Format( "'{0}'", HelperFunctions.stringReplace( curColValue, HelperFunctions.SingleQuoteDelim, "''" ) );
				} else if ( inputKeys.Contains( colName ) ) {
					tempValue = "''";
				} else {
					tempValue = "null";
				}
				stmtData.Append( curStmtDelim + String.Format("{0}", tempValue ) );

				curStmtDelim = ", ";
            }

			/*
			 * Add new data record to database
			 */
			String sqlStmt = String.Format( "Insert {0}  ({1}) Values ({2})", myTableName, stmtInsert.ToString(), stmtData.ToString() );
			int rowsProc = DataAccess.ExecuteCommand( sqlStmt ) ;
			if ( rowsProc > 0 ) myImportCounts[idxRowsAdded]++;
			
			return curSanctionId;
		}

		private void handleEndOfTable( string curSanctionId ) {
			if ( curSanctionId.Length == 0 ) return;

			if ( myTableName.ToLower().Trim().Equals( "slalomrecap" ) ) {
				execSlalomRecapUpdateNullPassSpeedKph( curSanctionId );
			}
			if ( myTableName.ToLower().Trim().Equals( "slalomscore" )
				|| myTableName.ToLower().Trim().Equals( "slalomrecap" )
				|| myTableName.ToLower().Trim().Equals( "jumpscore" )
				|| myTableName.ToLower().Trim().Equals( "jumprecap" )
				|| myTableName.ToLower().Trim().Equals( "trickscore" )
				) {
				execInsertDateUpdate( curSanctionId, myTableName );
			}
		}

		private void showImportStats() {
			String curInfoMsg = "";
			curInfoMsg = "Total import data processed";
			MessageBox.Show( String.Format("Info: {0}"
				+ "\nRows Read: {1}"
				+ "\nRows Added: {2}"
				+ "\nRows Matched: {3}"
				+ "\nRows Updated: {4}"
				+ "\nRows Skipped: {5}"
				, curInfoMsg, myImportCounts[idxRowsRead], myImportCounts[idxRowsAdded], myImportCounts[idxRowsFound]
				, myImportCounts[idxRowsUpdated], myImportCounts[idxRowsSkipped] ) );
		}

		private void execSlalomRecapUpdateNullPassSpeedKph( String curSanctionId ) {
			StringBuilder sqlStmtBuilder = new StringBuilder( "" );
			sqlStmtBuilder.Append( "Update SlalomRecap " );
			sqlStmtBuilder.Append( "Set PassSpeedKph = SUBSTRING ( Note, CHARINDEX('kph', Note) - 2, 2 ) " );
			sqlStmtBuilder.Append( "Where SanctionId = '{0}' and PassSpeedKph is null " );
			String sqlStmt = String.Format( sqlStmtBuilder.ToString(), curSanctionId );
			int rowsUpdate = DataAccess.ExecuteCommand( sqlStmt );
			if ( rowsUpdate > 0 ) {
				Log.WriteFile( String.Format( "{0} RowsUpdated={1} with statement: {2}"
					, "ImportData:execSlalomRecapUpdateNullPassSpeedKph:", rowsUpdate, sqlStmt ) );
			}
		}

		private void execInsertDateUpdate(String curSanctionId, String curTableName ) {
			String sqlStmt = String.Format( "Update {0} Set InsertDate = LastUpdateDate Where InsertDate is null and SanctionId = '{1}'", curTableName, curSanctionId );
			int rowsUpdate = DataAccess.ExecuteCommand(sqlStmt );
			if ( rowsUpdate > 0 ) {
				Log.WriteFile( String.Format( "{0} RowsUpdated={1} with statement: {2}"
					, "ImportData:execInsertDateUpdate:", rowsUpdate, sqlStmt ) );
			}
		}

		private ArrayList getImportFileList() {
            ArrayList curFileNameList = new ArrayList();
            String curPath = "";
            OpenFileDialog curFileDialog = new OpenFileDialog();
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if (curPath.Length < 2) {
                    curPath = Directory.GetCurrentDirectory();
                }
                curFileDialog.InitialDirectory = curPath;
                curFileDialog.Multiselect = true;
                curFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                curFileDialog.FilterIndex = 2;
                if (curFileDialog.ShowDialog() == DialogResult.OK) {
                    String[] curFileNames = curFileDialog.FileNames;
                    foreach (String curFileName in curFileNames) {
                        curFileNameList.Add( curFileName );
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
            }

            return curFileNameList;
        }

        private StreamReader getImportFile() {
            OpenFileDialog curFileDialog = new OpenFileDialog();
			
			try {
				String curPath = Properties.Settings.Default.ExportDirectory;
				if ( curPath.Length < 2 ) {
					curPath = Directory.GetCurrentDirectory();
				}
				curFileDialog.InitialDirectory = curPath;
				curFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
				curFileDialog.FilterIndex = 2;
				if ( curFileDialog.ShowDialog() != DialogResult.OK ) return null;

				String curFileName = curFileDialog.FileName;
				if ( curFileName == null ) return null;
				return getImportFile( curFileName );

			} catch ( Exception ex ) {
				MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
				return null;
			}
		}

        private StreamReader getImportFile(String curFileName ) {
			String curMethodName = "ImportData: getImportFile: ";
			StreamReader curReader = null;
			
			try {
				myProgressInfo.setProgessMsg( "File selected " + curFileName );
				myProgressInfo.Show();
				myProgressInfo.Refresh();

				string inputBuffer = "";
				int curInputLineCount = 0;
				curReader = new StreamReader( curFileName );
				while ( ( inputBuffer = curReader.ReadLine() ) != null ) {
					curInputLineCount++;
				}
				curReader.Close();
				curReader = null;
				myProgressInfo.setProgressMin( 1 );
				myProgressInfo.setProgressMax( curInputLineCount );

				return new StreamReader( curFileName );

			} catch ( Exception ex ) {
				if ( curFileName.EndsWith( ".tmp" ) ) {
					Log.WriteFile( String.Format( "{0}Error: Unable to access or read input file {1}\n\nError: {2}"
						, curMethodName, curFileName, ex.Message ) );
				} else {
					MessageBox.Show( "Error: Could not read file" + curFileName + "\n\nError: " + ex.Message );
				}
				return null;

			} finally {
				if ( curReader != null ) curReader.Close();
			}
		}

	}
}
