using System;
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
                if ( inColName.Equals( colName ) ) {
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

        public void importData() {
            importData( null );
        }

        public void importData(String inFileName) {
            bool curImportConfirmMsg = false;
            ArrayList curFileList = new ArrayList();
			myMatchCommand = "";

			if ( inFileName == null ) {
                curFileList = getImportFileList();
            } else {
                curFileList.Add( inFileName );
            }

			if ( curFileList.Count == 0 ) return;
		
			DialogResult msgResp =
				MessageBox.Show( "Do you want a confirmation dialog for each successful data type imported?", "Confirmation",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1 );
			if ( msgResp == DialogResult.Yes ) curImportConfirmMsg = true;

			foreach ( String curFileName in curFileList ) {
				importDataFile( curFileName, curImportConfirmMsg );
			}
		}

		public void importDataFile( String curFileName, bool curImportConfirmMsg ) {
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

						if ( curImportConfirmMsg ) {
							showImportStats( myTableName );
							myImportCounts = new int[] { 0, 0, 0, 0, 0 };
						}

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

					}
				}
			}
			
			handleEndOfTable( curSanctionId );
			if ( curImportConfirmMsg ) {
				showImportStats( myTableName );
			} else {
				showImportStats( null );
			}
			
			myImportCounts = new int[] { 0, 0, 0, 0, 0 };
			myProgressInfo.Close();
			curReader.Close();
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
			DateTime curLastUpdateDate = new DateTime(), curLastUpdateDateIn = new DateTime();
			
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
				isLastUpdateDateAvail = true;
				curLastUpdateDateIn = Convert.ToDateTime( curColValue );
				curLastUpdateDate = (DateTime)curRow["LastUpdateDate"];
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
			//Build update command with input record if specified
			int idx = 0;
			foreach ( string colName in inputColNames ) {
				if ( colName.ToLower().Equals( "pk" ) || HelperFunctions.isColumnObsolete( colName, myTableName ) ) {
					// Bypass columns that have been dropped or have data that can't be imported
					idx++;
					continue;
				}

				if ( colName.Trim().ToLower().Equals( "sanctionid" ) ) curSanctionId = inputCols[idx];

				curColValue = "";
				if ( stmtData.Length > 1 ) stmtData.Append( ", " );
				if ( inputCols[idx].Length > 0 ) {
					curColValue = String.Format( "'{0}'", HelperFunctions.stringReplace( inputCols[idx], HelperFunctions.SingleQuoteDelim, "''" ) );
				} else if ( inputKeys.Contains( colName ) ) {
					curColValue = "''";
				} else {
					curColValue = "null";
				}
				stmtData.Append( String.Format( "[{0}] = {1}", colName, curColValue ) );

				idx++;
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

			int idx = 0;
			foreach ( string colName in inputColNames ) {
				if ( colName.ToLower().Equals( "pk" ) || HelperFunctions.isColumnObsolete(colName, myTableName ) ) {
					// Bypass columns that have been dropped or have data that can't be imported
					idx++;
					continue;
				}
				
				if ( colName.Trim().ToLower().Equals( "sanctionid" ) ) curSanctionId = inputCols[idx];

				if ( stmtInsert.Length > 1 ) {
					stmtInsert.Append( ", " );
					stmtData.Append( ", " );
				}

				stmtInsert.Append( "[" + colName + "]" );

				String tempValue = "";
				if ( inputCols[idx].Length > 0 ) {
					tempValue = String.Format( "'{0}'", HelperFunctions.stringReplace( inputCols[idx], HelperFunctions.SingleQuoteDelim, "''" ) );
				} else if ( inputKeys.Contains( colName ) ) {
					tempValue = "''";
				} else {
					tempValue = "null";
				}
				stmtData.Append( String.Format("{0}", tempValue ) );
				
				idx++;
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

		private void showImportStats( String inTableName ) {
			String curInfoMsg = "";
			if ( inTableName == null ) {
				curInfoMsg = "Total import data processed";
			} else {
				curInfoMsg = "Import data processed for " + inTableName;
			}
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
            OpenFileDialog myFileDialog = new OpenFileDialog();
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if (curPath.Length < 2) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Multiselect = true;
                myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    String[] curFileNames = myFileDialog.FileNames;
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
            OpenFileDialog myFileDialog = new OpenFileDialog();
			
			try {
				String curPath = Properties.Settings.Default.ExportDirectory;
				if ( curPath.Length < 2 ) {
					curPath = Directory.GetCurrentDirectory();
				}
				myFileDialog.InitialDirectory = curPath;
				myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
				myFileDialog.FilterIndex = 2;
				if ( myFileDialog.ShowDialog() != DialogResult.OK ) return null;

				String curFileName = myFileDialog.FileName;
				if ( curFileName == null ) return null;
				int delimPos = curFileName.LastIndexOf( '\\' );
				curPath = curFileName.Substring( 0, delimPos );

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
