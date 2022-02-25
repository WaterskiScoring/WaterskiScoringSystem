using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tools {
    class ImportData {
        private String mySanctionNum;
        private string myTableName = null;
        private char[] myTabDelim = new char[] { '\t' };
        private char[] mySingleQuoteDelim = new char[] { '\'' };

		private bool myTourTypePickAndChoose = false;

		private DataRow myTourRow = null;

		private ImportMatchDialogForm MatchDialog;
        private TourEventReg myTourEventReg;
        private ProgressWindow myProgressInfo;
		private ImportMember myImportMember;

		public ImportData() {
            MatchDialog = new ImportMatchDialogForm();

			myTourRow = null;
			try {
				mySanctionNum = Properties.Settings.Default.AppSanctionNum;
				if ( mySanctionNum == null ) {
					mySanctionNum = "";

				} else {
					if ( mySanctionNum.Length < 6 ) {
						mySanctionNum = "";

					} else {
						myTourRow = getTourData();
                    }
				}

			} catch {
				mySanctionNum = "";
			}
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
            string inputBuffer, curSanctionId = "", colValue, MatchCommand = "", curMatchCommand = "", curLastUpdateDateIn = "";
            string[] inputCols = null, inputColNames = null, inputKeys = null, curImportDataMatchMsg = { "", "", "", "" };
            DateTime curLastUpdateDate = new DateTime(), curDateValue = new DateTime();
            Boolean rowFound = false;
            bool curImportConfirmMsg = true;
            int curInputLineCount = 0;
            int idx = 0, rowsRead = 0, rowsfound = 0, rowsAdded = 0, rowsUpdated = 0, rowsSkipped = 0;
            StringBuilder stmtSelect = new StringBuilder( "" );
            StringBuilder stmtWhere = new StringBuilder( "" );
            StringBuilder stmtInsert = new StringBuilder( "" );
            StringBuilder stmtData = new StringBuilder( "" );
            StreamReader myReader = null;
            DataTable curDataTable = null;

			myProgressInfo = new ProgressWindow();
            ArrayList curFileList = new ArrayList();

            if ( inFileName == null ) {
                curFileList = getImportFileList();
            } else {
                curFileList.Add( inFileName );
            }

            if (curFileList.Count > 0) {
                DialogResult msgResp =
                    MessageBox.Show( "Do you want a confirmation dialog for each successful data type imported?", "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if (msgResp == DialogResult.Yes) {
                    curImportConfirmMsg = true;
                } else {
                    curImportConfirmMsg = false;
                }

				foreach ( String curFileName in curFileList ) {
					myReader = getImportFile( curFileName );
					if ( myReader == null ) return;

					curInputLineCount = 0;
					while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
						curInputLineCount++;
						myProgressInfo.setProgressValue( curInputLineCount );

						rowFound = false;
						inputCols = inputBuffer.Split( myTabDelim );

						if ( inputCols[0].ToLower().Equals( "table:" ) || inputCols[0].ToLower().Equals( "tablename:" ) ) {
							//Display statistics when another table entry is found
							if ( myTableName != null ) {
								if ( myTableName.ToLower().Trim().Equals( "slalomrecap" ) && curSanctionId.Length >= 6 ) {
									execSlalomRecapCheckAndUpdate( curSanctionId );
								}
								if (curSanctionId.Length >= 6 &&
									( myTableName.ToLower().Trim().Equals("slalomscore")
									|| myTableName.ToLower().Trim().Equals("slalomrecap")
									|| myTableName.ToLower().Trim().Equals("jumpscore")
									|| myTableName.ToLower().Trim().Equals("jumprecap")
									|| myTableName.ToLower().Trim().Equals("trickscore")
									)) {
									execInsertDateUpdate(curSanctionId, myTableName);
								}

								if ( curImportConfirmMsg ) {
									MessageBox.Show( "Info: Import data processed for " + myTableName
										+ "\nRows Read: " + rowsRead
										+ "\nRows Added: " + rowsAdded
										+ "\nRows Matched: " + rowsfound
										+ "\nRows Updated: " + rowsUpdated
										+ "\nRows Skipped: " + rowsSkipped
										);
									rowsRead = 0;
									rowsfound = 0;
									rowsAdded = 0;
									rowsUpdated = 0;
									rowsSkipped = 0;
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
							for ( idx = 0; idx < inputCols.Length; idx++ ) {
								inputColNames[idx] = inputCols[idx];
							}

						} else {
							#region Process data rows for table and columns on input file
							//Process data rows.  Table name and column names are required 
							//before data rows can be processed
							if ( myTableName == null ) {
								MessageBox.Show( "Error: Table name not provide.  Unable to process import file." );
								break;

							} else if ( inputColNames == null ) {
								MessageBox.Show( "Error: Column definitions not provide.  Unable to process import file." );
								break;

							} else {
								rowsRead++;
								stmtSelect = new StringBuilder( "" );
								stmtWhere = new StringBuilder( "" );

								if ( inputKeys != null ) {
									//Use update date if available
									curLastUpdateDateIn = findColValue( "LastUpdateDate", inputColNames, inputCols );
									if ( curLastUpdateDateIn == null ) curLastUpdateDateIn = "";

									#region Identify key columns if available
									//Use key column data items to see if input row already exists on database
									foreach ( string keyName in inputKeys ) {
										colValue = findColValue( keyName, inputColNames, inputCols );
										if ( colValue == null ) colValue = "";
										if ( keyName.Trim().ToLower().Equals( "sanctionid" ) ) curSanctionId = colValue;
										if ( stmtSelect.Length > 1 ) {
											stmtSelect.Append( ", " + keyName );
											stmtWhere.Append( " AND " + keyName + " = '" + colValue + "'" );
										} else {
											stmtSelect.Append( "Select " );
											if ( curLastUpdateDateIn.Length > 0 ) {
												stmtSelect.Append( "LastUpdateDate, " );
											}
											stmtSelect.Append( keyName );
											stmtWhere.Append( " Where  " + keyName + " = '" + colValue + "'" );
										}
									}

									curMatchCommand = "";
									curDataTable = DataAccess.getDataTable( stmtSelect.ToString() + " From " + myTableName + stmtWhere.ToString() );
									if ( curDataTable == null ) {
										MessageBox.Show( "Error: Checking " + myTableName + " for import data");
										return;

									} else {
										if ( curDataTable.Rows.Count > 0 ) {
											rowFound = true;
											rowsfound++;
											if ( !( MatchCommand.ToLower().Equals( "skipall" ) ) ) {
												if ( curLastUpdateDateIn.Length > 0 ) {
													try {
														curLastUpdateDate = (DateTime) curDataTable.Rows[0]["LastUpdateDate"];
														curDateValue = Convert.ToDateTime( curLastUpdateDateIn );
														if ( curDateValue > curLastUpdateDate ) {
															curMatchCommand = "Update";
														} else {
															curMatchCommand = "Skip";
														}
													} catch {
														curMatchCommand = "Update";
														curLastUpdateDate = Convert.ToDateTime( "01/01/2000" );
													}
												}
											}
										}
									}
									#endregion
								}

								stmtInsert = new StringBuilder( "" );
								stmtData = new StringBuilder( "" );

								if ( rowFound ) {
									#region Show information if input data found on database
									//Show information if input data found on database
									//Skip display if previoius display specfied to process all records the same
									if ( MatchCommand.Length < 2 ) {
										if ( curMatchCommand.Equals( "" ) || curMatchCommand.ToLower().Equals( "update" ) ) {
											curImportDataMatchMsg[0] = "Table: " + myTableName;
											curImportDataMatchMsg[1] = stmtWhere.ToString();
											if ( curMatchCommand.ToLower().Equals( "update" ) ) {
												curImportDataMatchMsg[2] = "Current record date = " + curLastUpdateDate.ToString();
												curImportDataMatchMsg[3] = " Import record date = " + curLastUpdateDateIn;
											} else {
												curImportDataMatchMsg[2] = "";
												curImportDataMatchMsg[3] = "";
											}
											MatchDialog.ImportKeyDataMultiLine = curImportDataMatchMsg;
											MatchDialog.MatchCommand = MatchCommand;
											if ( MatchDialog.ShowDialog() == DialogResult.OK ) {
												MatchCommand = MatchDialog.MatchCommand;
											}
										}
									}

									if ( curMatchCommand.Equals( "skip" ) ) {
										rowsSkipped++;
										//Re-initialize dialog response unless specified to process rows
										if ( MatchCommand.ToLower().Equals( "skip" ) ) {
											MatchCommand = "";
										}
									} else {
										if ( MatchCommand.ToLower().Equals( "update" )
											|| MatchCommand.ToLower().Equals( "updateall" ) ) {
											//Build update command with input record if specified
											idx = 0;
											foreach ( string colName in inputColNames ) {
												if ( inputKeys.Contains( colName ) || colName.ToLower().Equals( "pk" ) ) {
												} else if ( colName.Equals( "TimeInTol1" )
														|| colName.Equals( "TimeInTol2" )
														|| colName.Equals( "TimeInTol3" )
														|| colName.Equals( "BoatSplitTimeTol" )
														|| colName.Equals( "BoatSplitTime2Tol" )
														|| colName.Equals( "BoatEndTimeTol" )
														|| ( colName.Equals( "Pass1VideoUrl" ) && myTableName.Equals( "TrickScore" ) )
														|| ( colName.Equals( "Pass2VideoUrl" ) && myTableName.Equals( "TrickScore" ) )
														|| ( colName.Equals( "FinalPassNum" ) && myTableName.Equals( "SlalomScore" ) )
														|| ( colName.Equals( "PassNum" ) && myTableName.Equals( "SlalomRecap" ) )
														) {
													//Columns that have been dropped or have data that can't be imported
												} else {
													if ( stmtData.Length > 1 ) {
														stmtData.Append( ", [" + colName + "] = " );
													} else {
														stmtData.Append( "[" + colName + "] = " );
													}
													if ( inputCols[idx].Length > 0 ) {
														String tempValue = stringReplace( inputCols[idx], mySingleQuoteDelim, "''" );
														stmtData.Append( "'" + tempValue + "'" );
													} else {
														if ( inputKeys.Contains( colName ) ) {
															stmtData.Append( " ''" );
														} else {
															stmtData.Append( " null" );
														}
													}
												}
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

											int rowsProc = DataAccess.ExecuteCommand( "Update " + myTableName + " set " + stmtData.ToString() + stmtWhere.ToString() );
											// Exit method if error is detected with last update statement (rowsProc less than zero indicate an exception was encountered)
											if ( rowsProc < 0 ) return;
											rowsUpdated++;

											//Re-initialize dialog response unless specified to process rows
											if ( MatchCommand.ToLower().Equals( "update" ) ) {
												MatchCommand = "";
											}

										} else {
											rowsSkipped++;
											//Re-initialize dialog response unless specified to process rows
											if ( MatchCommand.ToLower().Equals( "skip" ) ) {
												MatchCommand = "";
											}
										}
									}
									#endregion
								} else {
									#region New data identified and will be added
									//Database record does not exist therefore data is added to database
									//Build insert command
									idx = 0;
									foreach ( string colName in inputColNames ) {
										if ( colName.ToLower().Equals( "pk" )
											|| colName.Equals( "TimeInTol1" )
											|| colName.Equals( "TimeInTol2" )
											|| colName.Equals( "TimeInTol3" )
											|| colName.Equals( "BoatSplitTimeTol" )
											|| colName.Equals( "BoatSplitTime2Tol" )
											|| colName.Equals( "BoatEndTimeTol" )
											|| ( colName.Equals( "Pass1VideoUrl" ) && myTableName.Equals( "TrickScore" ) )
											|| ( colName.Equals( "Pass2VideoUrl" ) && myTableName.Equals( "TrickScore" ) )
											|| ( colName.Equals( "FinalPassNum" ) && myTableName.Equals( "SlalomScore" ) )
											|| ( colName.Equals( "PassNum" ) && myTableName.Equals( "SlalomRecap" ) )
											) {
											//Columns that have been dropped or have data that can't be imported
										} else {
											if ( stmtInsert.Length > 1 ) {
												stmtInsert.Append( ", [" + colName + "]" );
												if ( inputCols[idx].Length > 0 ) {
													String tempValue = stringReplace( inputCols[idx], mySingleQuoteDelim, "''" );
													stmtData.Append( ", '" + tempValue + "'" );
												} else {
													if ( inputKeys.Contains( colName ) ) {
														stmtData.Append( ", ''" );
													} else {
														stmtData.Append( ", null" );
													}
												}
											} else {
												stmtInsert.Append( "[" + colName + "]" );
												if ( inputCols[idx].Length > 0 ) {
													String tempValue = stringReplace( inputCols[idx], mySingleQuoteDelim, "''" );
													stmtData.Append( "'" + tempValue + "'" );
												} else {
													if ( inputKeys.Contains( colName ) ) {
														stmtData.Append( "''" );
													} else {
														stmtData.Append( "null" );
													}
												}
											}
										}
										idx++;
									}

									int rowsProc = DataAccess.ExecuteCommand( "Insert " + myTableName + " (" + stmtInsert.ToString() + ") Values (" + stmtData.ToString() + ")" );
									// Exit method if error is detected with last update statement (rowsProc less than zero indicate an exception was encountered)
									if ( rowsProc < 0 ) return;
									rowsAdded++;

									#endregion
								}
							}
							#endregion
						}
					}

					if ( inFileName == null ) {
						if ( myTableName.ToLower().Trim().Equals( "slalomrecap" ) ) {
							execSlalomRecapCheckAndUpdate( curSanctionId );
						}
						if (curSanctionId.Length >= 6 &&
							(myTableName.ToLower().Trim().Equals("slalomscore")
							|| myTableName.ToLower().Trim().Equals("slalomrecap")
							|| myTableName.ToLower().Trim().Equals("jumpscore")
							|| myTableName.ToLower().Trim().Equals("jumprecap")
							|| myTableName.ToLower().Trim().Equals("trickscore")
							)) {
							execInsertDateUpdate(curSanctionId, myTableName);
						}
						if ( curImportConfirmMsg ) {
							MessageBox.Show( "Info: Import data processed for " + myTableName
								+ "\nRows Read: " + rowsRead
								+ "\nRows Added: " + rowsAdded
								+ "\nRows Matched: " + rowsfound
								+ "\nRows Updated: " + rowsUpdated
								+ "\nRows Skipped: " + rowsSkipped
								);
						} else {
							MessageBox.Show( "Info: Total import data processed"
								+ "\nRows Read: " + rowsRead
								+ "\nRows Added: " + rowsAdded
								+ "\nRows Matched: " + rowsfound
								+ "\nRows Updated: " + rowsUpdated
								+ "\nRows Skipped: " + rowsSkipped
								);
						}
						rowsRead = 0;
						rowsAdded = 0;
						rowsfound = 0;
						rowsUpdated = 0;
						rowsSkipped = 0;
					}

				}

				myProgressInfo.Close();
            }
        }

		private void execSlalomRecapCheckAndUpdate(String curSanctionId ) {
			int curSkiYear = 0;
            int.TryParse( curSanctionId.Substring( 0, 2 ), out curSkiYear );
			if ( curSkiYear < 19 ) {
				int rowsUpdate = DataAccess.ExecuteCommand( "Update SlalomRecap "
							+ "Set PassSpeedKph = SUBSTRING ( Note, CHARINDEX('kph', Note) - 2, 2 ) "
							+ "Where SanctionId = '" + curSanctionId + "' and PassSpeedKph is null "
							);
				int curValue = rowsUpdate;
			}

		}

		private void execInsertDateUpdate(String curSanctionId, String curTableName ) {
			StringBuilder sqlStmt = new StringBuilder("");
			sqlStmt = new StringBuilder("Update " + curTableName + " Set InsertDate = LastUpdateDate Where InsertDate is null and SanctionId = '" + curSanctionId + "'");
			int rowsUpdate = DataAccess.ExecuteCommand(sqlStmt.ToString() );
		}

		public bool truncateMemberData() {
            String dialogMsg = "All members will be removed!"
                + "\n This will not affect any tournament registrations or scores."
                + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show( dialogMsg, "Truncate Warning",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            if ( msgResp == DialogResult.Yes ) {
                try {
					int rowsProc = DataAccess.ExecuteCommand( "Delete MemberList ");
                    MessageBox.Show( rowsProc + " members removed" );
                    return true;

				} catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    return false;
                }

			} else if ( msgResp == DialogResult.No ) {
                return false;

			} else {
                return false;
            }
        }

        public bool importMemberDataCurrent( ) {
            return importMemberDataDialog( false );
        }
		public bool importMemberDataCurrent(String tourType) {
			myTourTypePickAndChoose = false;
			if ( tourType != null && tourType.ToLower().Equals( "pick" ) ) {
				DialogResult msgResp = MessageBox.Show( "You have choosen to import registrations using a Pick & Choose format."
					+ "\nThis format will create separate running orders for individual rounds"
					+ "\nAre you sure you want to continue with this format?"
					, "Warning",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1 );
				if ( msgResp == DialogResult.Yes ) myTourTypePickAndChoose = true;
			}
			return importMemberDataDialog( false );
		}
		public bool importMemberDataNcwsa( ) {
            return importMemberDataDialog( true );
        }

        private bool importMemberDataDialog( bool inNcwsa ) {
            bool curReturn = true;
            myProgressInfo = new ProgressWindow();
            try {
                mySanctionNum = Properties.Settings.Default.AppSanctionNum;
                if ( mySanctionNum == null ) {
                    curReturn = false;
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );

				} else {
                    if ( mySanctionNum.Length < 6 ) {
                        curReturn = false;
                        MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    } else {
						if ( myTourRow == null ) {
							myTourRow = getTourData();
						} else {
							if ( mySanctionNum != (String)myTourRow["SanctionId"]) {
								myTourRow = getTourData();
							}
						}

						curReturn = true;
                    }
                }
            } catch {
                curReturn = false;
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            }
            if ( curReturn ) {
                try {
                    curReturn = importMemberData( inNcwsa );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to import member data \n" + excp.Message );
                }
            }
            myProgressInfo.Close();
            return curReturn;
        }

        private bool importMemberData( bool inNcwsa ) {
            bool curReturn = true, curTeamHeaderActive = false, curNcwsa = inNcwsa, curTourRegFmt = false;
            string inputBuffer, myfileName, MemberId;
            String[] inputCols;
            String[] inputColsSaved = null;
            StreamReader myReader;
            DialogResult msgResp;
            int numCk = 0, idxMemberId = 0;
            int curInputLineCount = 0;
            myTourEventReg = new TourEventReg();
            
            //Choose an input file to be processed
            String curPath = Properties.Settings.Default.ExportDirectory;
            OpenFileDialog myFileDialog = new OpenFileDialog();

			if ( myTourTypePickAndChoose ) {
				myImportMember = new ImportMember( myTourRow, "pick" );
			} else {
				myImportMember = new ImportMember( myTourRow );
			}

			myFileDialog.InitialDirectory = curPath;
            myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            myFileDialog.FilterIndex = 2;

            try {
                if ( myFileDialog.ShowDialog() != DialogResult.OK ) return false;

				myfileName = myFileDialog.FileName;
				if ( myfileName == null ) return false;

				myProgressInfo.setProgessMsg( "File selected " + myfileName );
				myProgressInfo.Show();
				myProgressInfo.Refresh();

				//Get file date and prepare to read input data
				File.GetLastWriteTime( myfileName );
				try {
					curInputLineCount = 0;
					myReader = new StreamReader( myfileName );
					while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
						curInputLineCount++;
					}
					myReader.Close();
					myProgressInfo.setProgressMin( 1 );
					myProgressInfo.setProgressMax( curInputLineCount );

				} catch ( Exception ex ) {
					MessageBox.Show( "Error: Could not read file" + myFileDialog.FileName + "\n\nError: " + ex.Message );
					return false;
				}

				curInputLineCount = 0;
				myReader = new StreamReader( myfileName );
				try {
					#region Process each input file data row
					while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
						curInputLineCount++;
						myProgressInfo.setProgressValue( curInputLineCount );

						//Check input row to ensure it is a data row
						if ( inputBuffer.ToLower().IndexOf( "end-of-list" ) > -1
							|| inputBuffer.ToLower().IndexOf( "end of list" ) > -1
							|| inputBuffer.ToLower().IndexOf( "end_of_list" ) > -1
							|| inputBuffer.ToLower().IndexOf( "endoflist" ) > -1
							) {
							break;

						} else {
							#region process registration input records
							inputCols = inputBuffer.Split( myTabDelim );
							if ( inputCols.Length > 3 ) {
								//Check first line of input file to analyze the file format supplied
								// WSTIMS Rel 4.0+ NCWSA Registration Worksheet
								if ( curInputLineCount == 1 && curNcwsa == false ) {
									if ( inputCols[0].ToLower().IndexOf( "wstims" ) > -1 && inputCols[0].ToLower().IndexOf( "registration worksheet" ) > -1 ) {
										curTourRegFmt = true;

									} else if ( inputCols[0].ToLower().IndexOf( "wstims" ) > -1 && inputCols[0].ToLower().IndexOf( "ncwsa" ) > -1 ) {
										curNcwsa = true;
										curTourRegFmt = false;
                                    }
								}

								if ( curInputLineCount > 1 ) {
									if ( inputCols[idxMemberId].Trim().Length > 10 ) {
										MemberId = inputCols[idxMemberId].Substring( 0, 3 ) + inputCols[idxMemberId].Substring( 4, 2 ) + inputCols[idxMemberId].Substring( 7, 4 );
									} else {
										MemberId = "---";
									}

									//If a valid member id is detected in the first column 
									//assume valid record available for input
									if ( int.TryParse( MemberId.Substring( 0, 3 ), out numCk ) ) {
										#region process registration input records that start with appears to be a valid MemberId
										curTeamHeaderActive = false;
										if ( curNcwsa ) {
											inputColsSaved = new String[inputCols.Length];
											curReturn = procMemberInput( inputCols, MemberId, curTourRegFmt, curNcwsa, inputColsSaved );

										} else {
											curReturn = procMemberInput( inputCols, MemberId, curTourRegFmt, curNcwsa, null );
										}
										if ( curReturn ) {
											if ( curNcwsa && inputColsSaved != null ) {
												if ( inputColsSaved[0] != null ) {
													curReturn = procMemberInput( inputColsSaved, MemberId, curTourRegFmt, curNcwsa, null );
												}
											}
										} else {
											msgResp = MessageBox.Show( "Error encountered on last record./n Do you want to continue processing?", "Warning",
												MessageBoxButtons.YesNo,
												MessageBoxIcon.Warning,
												MessageBoxDefaultButton.Button1 );
											if ( msgResp == DialogResult.Yes ) {
												curReturn = true;
											} else {
												curReturn = false;
												break;
											}
										}
										#endregion

									} else {
										#region process registration input records that don't start with a MemberId
										if ( inputCols[idxMemberId].ToLower().Equals( "end-of-list" )
											|| inputCols[idxMemberId].ToLower().Equals( "end of list" )
											|| inputCols[idxMemberId].ToLower().Equals( "end_of_list" )
											|| inputCols[idxMemberId].ToLower().Equals( "endoflist" )
											) {
											break;

										} else if ( ( inputCols[0].ToLower().IndexOf( "team header" ) > -1 )
											|| ( inputCols[0].ToLower().IndexOf( "teamheader" ) > -1 )
											|| ( inputCols[0].ToLower().IndexOf( "team-header" ) > -1 )
											|| ( inputCols[0].ToLower().IndexOf( "team_header" ) > -1 )
											) {
											curTeamHeaderActive = true;
											curReturn = procTeamHeaderInput( inputCols, inNcwsa );

										} else if ( inputCols[idxMemberId].ToLower().Equals( "tourn name:" ) ) {
											curTeamHeaderActive = false;
											String curInputSanctionId = inputCols[6];
											if ( curInputSanctionId.Length > 6 ) {
												curInputSanctionId = curInputSanctionId.Substring( 0, 6 );
											}
											if ( !( curInputSanctionId.Equals( mySanctionNum ) ) ) {
												String dialogMsg = "Sanction number on import file is " + inputCols[6]
													+ "\n Current tournament being scored is " + mySanctionNum
													+ "\n\n Do you still want to continue?";
												msgResp =
													MessageBox.Show( dialogMsg, "Database Copy",
													MessageBoxButtons.YesNo,
													MessageBoxIcon.Question,
													MessageBoxDefaultButton.Button1 );
												if ( msgResp == DialogResult.No ) {
													break;
												}
											}

										} else {
											if ( curTeamHeaderActive ) {
												curReturn = procTeamHeaderInput( inputCols, inNcwsa );
											}
										}
										#endregion
									}
								}

							} else {
								if ( inputCols.Length > 3 ) {
									if ( ( inputCols[0].ToLower().IndexOf( "team header" ) > -1 )
									|| ( inputCols[0].ToLower().IndexOf( "teamheader" ) > -1 )
									|| ( inputCols[0].ToLower().IndexOf( "team-header" ) > -1 )
									|| ( inputCols[0].ToLower().IndexOf( "team_header" ) > -1 )
									) {
										curTeamHeaderActive = true;
										curReturn = procTeamHeaderInput( inputCols, inNcwsa );
									}
								}
							}
							
							#endregion
						}
					}

					myImportMember.displayMemberProcessCounts();
					#endregion

				} finally {
					myReader.Close();
					myReader.Dispose();
				}

			} catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not read file" + myFileDialog.FileName + "\n\nError: " + ex.Message );
                return false;
            }

            return true;
        }

        private bool procMemberInput(string[] inputCols, string MemberId, bool inTourReg, bool inNcwsa, string[] inputColsSaved) {
			Dictionary<string, object> curImportMemberEntry = new Dictionary<string, object>();

			int idxMemberId = 0, idxLastName = 1, idxFirstName = 2,
                idxTeam = 3,
                idxAgeGroup = 4,
                idxSkiYearAge = 5,
                idxCity = 6, idxState = 7,
                idxEventSlalom = 8, idxEventTrick = 9, idxEventJump = 10,
                idxApptdOfficial = 11,
                idxSlalomRank = 12, idxTrickRank = 13, idxJumpRank = 14,
                idxSlalomRating = 15, idxTrickRating = 16, idxJumpRating = 17, idxOverallRating = 18,
                idxTrickBoat = 19,
                idxJumpHeight = 20,

				idxEventClassSlalom = 21, idxEventClassTrick = 22, idxEventClassJump = 23,
				idxMemberStatus = 24, idxNote = 25, idxUpgradeAmt = 26, idxExpireDate = 27,

				idxJudgeSlalomRating = 32, idxJudgeTrickRating = 33, idxJudgeJumpRating = 34,
				idxDriverSlalomRating = 35, idxDriverTrickRating = 36, idxDriverJumpRating = 37,
				idxScorerSlalomRating = 38, idxScorerTrickRating = 39, idxScorerJumpRating = 40,
				idxSafetyRating = 41, idxTechCntlrRating = 42,
				idxSportDiv = 0
                ;

			if ( inTourReg ) {
				if ( inputCols.Length < 28 ) {
					MessageBox.Show( "Invalid tournament registration record detected. Bypassing record"
						+ "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
					);
					return false;
				}
			} else if ( inNcwsa ) {
				if ( inputCols.Length < 13 ) {
					MessageBox.Show( "Invalid tournament registration record detected. Bypassing record"
						+ "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
					);
					return false;
				}
			}

			try {
				curImportMemberEntry.Add( "MemberId", MemberId );
				curImportMemberEntry.Add( "FirstName", stringReplace( inputCols[idxFirstName], mySingleQuoteDelim, "''" ) );
				curImportMemberEntry.Add( "LastName", stringReplace( inputCols[idxLastName], mySingleQuoteDelim, "''" ) );

				curImportMemberEntry.Add( "Team", inputCols[idxTeam] );
				curImportMemberEntry.Add( "AgeGroup", inputCols[idxAgeGroup] );

				curImportMemberEntry.Add( "SkiYearAge", 0 );
                if ( inputCols[idxSkiYearAge].Length > 0 ) {
					int numCk;
					if ( int.TryParse( inputCols[idxSkiYearAge], out numCk ) ) {
						if ( numCk > 0 ) {
							curImportMemberEntry["SkiYearAge"] = numCk;
						}
					}
				}

				curImportMemberEntry.Add( "City", stringReplace( inputCols[idxCity], mySingleQuoteDelim, "''" ) );
				curImportMemberEntry.Add( "State", inputCols[idxState] );
				curImportMemberEntry.Add( "Federation", "USA" );
				curImportMemberEntry.Add( "SportDiv", "AWS" );

				if ( inputCols[idxEventSlalom].Length > 0 ) {
					curImportMemberEntry.Add( "EventSlalom", inputCols[idxEventSlalom] );
				} else {
					curImportMemberEntry.Add( "EventSlalom", "" );
				}
				if ( inputCols[idxEventTrick].Length > 0 ) {
					curImportMemberEntry.Add( "EventTrick", inputCols[idxEventTrick] );
				} else {
					curImportMemberEntry.Add( "EventTrick", "" );
				}
				if ( inputCols[idxEventJump].Length > 0 ) {
					curImportMemberEntry.Add( "EventJump", inputCols[idxEventJump] );
				} else {
					curImportMemberEntry.Add( "EventJump", "" );
				}

				if ( idxApptdOfficial > 0 ) {
					curImportMemberEntry.Add( "ApptdOfficial", inputCols[idxApptdOfficial].Trim() );
				}

				#region Initialize official ratings
				curImportMemberEntry.Add( "JudgeSlalom", "" );
				curImportMemberEntry.Add( "JudgeTrick", "" );
				curImportMemberEntry.Add( "JudgeJump", "" );

				curImportMemberEntry.Add( "DriverSlalom", "" );
				curImportMemberEntry.Add( "DriverTrick", "" );
				curImportMemberEntry.Add( "DriverJump", "" );

				curImportMemberEntry.Add( "ScorerSlalom", "" );
				curImportMemberEntry.Add( "ScorerTrick", "" );
				curImportMemberEntry.Add( "ScorerJump", "" );

				curImportMemberEntry.Add( "Safety", "" );
				curImportMemberEntry.Add( "TechController", "" );

				if ( inputCols.Length > 32 ) {
					if ( inputCols.Length > idxJudgeSlalomRating ) curImportMemberEntry[ "JudgeSlalom"] = inputCols[idxJudgeSlalomRating];
					if ( inputCols.Length > idxJudgeTrickRating ) curImportMemberEntry[ "JudgeTrick"] = inputCols[idxJudgeTrickRating];
					if ( inputCols.Length > idxJudgeJumpRating ) curImportMemberEntry[ "JudgeJump"] = inputCols[idxJudgeJumpRating];

					if ( inputCols.Length > idxDriverSlalomRating ) curImportMemberEntry[ "DriverSlalom"] = inputCols[idxDriverSlalomRating];
					if ( inputCols.Length > idxDriverTrickRating ) curImportMemberEntry[ "DriverTrick"] = inputCols[idxDriverTrickRating];
					if ( inputCols.Length > idxDriverJumpRating ) curImportMemberEntry[ "DriverJump"] = inputCols[idxDriverJumpRating];

					if ( inputCols.Length > idxScorerSlalomRating ) curImportMemberEntry[ "ScorerSlalom"] = inputCols[idxScorerSlalomRating];
					if ( inputCols.Length > idxScorerTrickRating ) curImportMemberEntry[ "ScorerTrick"] = inputCols[idxScorerTrickRating];
					if ( inputCols.Length > idxScorerJumpRating ) curImportMemberEntry[ "ScorerJump"] = inputCols[idxScorerJumpRating];

					if ( inputCols.Length > idxSafetyRating ) curImportMemberEntry[ "Safety"] = inputCols[idxSafetyRating];
					if ( inputCols.Length > idxTechCntlrRating ) curImportMemberEntry[ "TechController"] = inputCols[idxTechCntlrRating];

				}
				#endregion

				if ( inTourReg ) {
					#region Initialize attributes for processing for standard tournament
					curImportMemberEntry.Add( "SlalomRating", inputCols[idxSlalomRating] );
					curImportMemberEntry.Add( "TrickRating", inputCols[idxTrickRating] );
					curImportMemberEntry.Add( "JumpRating", inputCols[idxJumpRating] );
					curImportMemberEntry.Add( "OverallRating", inputCols[idxOverallRating] );

					curImportMemberEntry.Add( "TrickBoat", inputCols[idxTrickBoat] );
					curImportMemberEntry.Add( "JumpHeight", inputCols[idxJumpHeight] );

					curImportMemberEntry.Add( "SlalomRank", (Decimal)0.0 );
					if ( inputCols[idxSlalomRank].Length > 0 ) {
						Decimal numDecCk = 0;
						if ( Decimal.TryParse( inputCols[idxSlalomRank], out numDecCk ) ) {
							curImportMemberEntry ["SlalomRank"] = numDecCk;
						}
					}

					curImportMemberEntry.Add( "TrickRank", (Decimal) 0.0 );
					if ( inputCols[idxTrickRank].Length > 0 ) {
						Decimal numDecCk = 0;
						if ( Decimal.TryParse( inputCols[idxTrickRank], out numDecCk ) ) {
							curImportMemberEntry["TrickRank"] = numDecCk;
						}
					}

					curImportMemberEntry.Add( "JumpRank", (Decimal) 0.0 );
					if ( inputCols[idxJumpRank].Length > 0 ) {
						Decimal numDecCk = 0;
						if ( Decimal.TryParse( inputCols[idxJumpRank], out numDecCk ) ) {
							curImportMemberEntry["JumpRank"] = numDecCk;
						}
					}

					curImportMemberEntry.Add( "EventClassSlalom", inputCols[idxEventClassSlalom] );
					curImportMemberEntry.Add( "EventClassTrick", inputCols[idxEventClassTrick] );
					curImportMemberEntry.Add( "EventClassJump", inputCols[idxEventClassJump] );
					#endregion

				} else if ( inNcwsa ) {
					#region Initialize attributes for processing for collegiate tournament
					if ( inputCols.Length < 20 ) {
						idxMemberStatus = 0;
						idxNote = 0;
						idxNote = 0;
						idxUpgradeAmt = 0;
						idxExpireDate = 0;
						idxSportDiv = 0;

					} else {
						idxTrickBoat = 13;
						idxJumpHeight = 14;
						idxSportDiv = 16;
						idxMemberStatus = 17;
						idxNote = 17;
						idxUpgradeAmt = 18;

						curImportMemberEntry.Add( "TrickBoat", inputCols[idxTrickBoat] );
						curImportMemberEntry.Add( "JumpHeight", inputCols[idxJumpHeight] );
						curImportMemberEntry["SportDiv"] = inputCols[idxSportDiv];
					}

					curImportMemberEntry.Add( "SlalomRank", (Decimal) 0.0 );
					curImportMemberEntry.Add( "TrickRank", (Decimal) 0.0 );
					curImportMemberEntry.Add( "JumpRank", (Decimal) 0.0 );

					curImportMemberEntry.Add( "SlalomRating", "" );
					curImportMemberEntry.Add( "TrickRating", "" );
					curImportMemberEntry.Add( "JumpRating", "" );
					curImportMemberEntry.Add( "OverallRating", "" );

					curImportMemberEntry.Add( "EventClassSlalom", "" );
					curImportMemberEntry.Add( "EventClassTrick", "" );
					curImportMemberEntry.Add( "EventClassJump", "" );
					#endregion
				}

				curImportMemberEntry.Add( "MembershipTypeCode", 0 );
				curImportMemberEntry.Add( "ActiveMember", "Inactive" );
				curImportMemberEntry.Add( "UpgradeAmt", 0.0 );

				curImportMemberEntry.Add( "EffTo", "" );
				curImportMemberEntry.Add( "Note", "" );
				curImportMemberEntry.Add( "MemTypeDesc", "" );
				curImportMemberEntry.Add( "Gender", "" );
				curImportMemberEntry.Add( "CanSki", false );
				curImportMemberEntry.Add( "CanSkiGR", false );
				curImportMemberEntry.Add( "Waiver", 0 );

				#region Analyze and set membership status
				if ( idxMemberStatus > 0 ) {
					curImportMemberEntry["Note"] = inputCols[idxMemberStatus];
				}

				if ( idxMemberStatus > 0 ) {
					curImportMemberEntry[ "Note"] = inputCols[idxNote];
					curImportMemberEntry ["UpgradeAmt"] = inputCols[idxUpgradeAmt];

					if ( inputCols[idxMemberStatus].ToLower().Trim().Equals( "yes" ) ) {
						curImportMemberEntry["ActiveMember"] =  "Active";
						curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
						curImportMemberEntry["Waiver"] = 1 ;

					} else if ( inputCols[idxMemberStatus].ToLower().Trim().Equals( "no" ) ) {
						curImportMemberEntry["ActiveMember"] =  "Inactive";
						curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
						curImportMemberEntry["Waiver"] = 0;

					} else if ( inputCols[idxMemberStatus] == null ) {
						curImportMemberEntry["ActiveMember"] =  "Inactive";
						curImportMemberEntry["EffTo"] = "";
						curImportMemberEntry["Waiver"] = 0;

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "ok to ski" ) > -1 ) {
						curImportMemberEntry["ActiveMember"] =  "Active";
						curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
						curImportMemberEntry["Waiver"] = 1 ;

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "needs upgrade" ) > -1 ) {
						curImportMemberEntry["ActiveMember"] = "Inactive";
						curImportMemberEntry["EffTo"] = "";
						curImportMemberEntry["Waiver"] = 0;

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "pre-reg" ) > -1 ) {
						curImportMemberEntry["ActiveMember"] =  "Active";
						curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
						curImportMemberEntry["Waiver"] = 1 ;

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "needs event waiver" ) > -1 
						|| inputCols[idxMemberStatus].ToLower().IndexOf( "needs annual waiver" ) > -1 ) {
						curImportMemberEntry["ActiveMember"] =  "Active";
						curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
						curImportMemberEntry["Waiver"] = 0;

					} else {

						if ( inNcwsa ) {
							if ( inputCols[idxEventSlalom].Length > 0 || inputCols[idxEventTrick].Length > 0 || inputCols[idxEventJump].Length > 0 ) {
								curImportMemberEntry["ActiveMember"] = "Active"; 
								curImportMemberEntry ["EffTo"] = "12/31/" + System.DateTime.Now.Year.ToString();
								curImportMemberEntry["Waiver"] = 1 ;

							} else {
								curImportMemberEntry["ActiveMember"] =  inputCols[idxMemberStatus];
								curImportMemberEntry ["EffTo"] = "";
								curImportMemberEntry["Waiver"] = 0;
							}

						} else {
							curImportMemberEntry["ActiveMember"] =  inputCols[idxMemberStatus];
							curImportMemberEntry["EffTo"] = inputCols[idxExpireDate];
							curImportMemberEntry["Waiver"] = 0;
						}
					}
					#endregion
				}

				#region Process member entry depending on type of tournamnet and member attributes
				if ( inNcwsa ) {
					String curTeam = ( (String) curImportMemberEntry["Team"] ).ToUpper();
					if ( curTeam.Equals( "OFF" ) || curTeam.Length == 0 ) {
						return myImportMember.importMemberFromAwsa( curImportMemberEntry, inTourReg, inNcwsa );

					} else {
						//Check for A team or B team designation unless skier is an official or not assigned to a team
						return myImportMember.importNcwsMemberFromAwsa( curImportMemberEntry );
					}

				} else {
					return myImportMember.importMemberFromAwsa( curImportMemberEntry, inTourReg, inNcwsa );

				}
				#endregion

			} catch ( Exception ex ) {
                String ExcpMsg = "Error Processing Member "
                    + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                    + "\n\n " + ex.Message;
                MessageBox.Show( ExcpMsg );
				return false;
            }
        }

        private bool procTeamHeaderInput( string[] inputCols, bool inNcwsa ) {
			Dictionary<string, object> curImportMemberEntry = new Dictionary<string, object>();

			int curIdxTeamName = 1, curIdxTeamCode = 3, curIdxAgeGroup = 4;
			int curIdxSlalomRunOrder = 8, curIdxTrickRunOrder = 9, curIdxJumpRunOrder = 10;

			if ( inputCols.Length > 3 ) {
                if ( ( inputCols[1].Length > 0 ) && inputCols[3].Length > 0 ) {
					curImportMemberEntry.Add( "TeamName", inputCols[curIdxTeamName] );
					curImportMemberEntry.Add( "TeamCode", inputCols[curIdxTeamCode] );

					curImportMemberEntry.Add( "AgeGroup", "" );
					curImportMemberEntry.Add( "EventGroup", "" );

					curImportMemberEntry.Add( "SlalomRunOrder", 0 );
					curImportMemberEntry.Add( "TrickRunOrder", 0 );
					curImportMemberEntry.Add( "JumpRunOrder", 0 );

                    if ( !( ((String)inputCols[curIdxTeamCode]).ToLower().Equals( "off" ) ) ) {
                        
						#region check for team rotation assignments
                        if ( inputCols.Length > 4 ) {
							String curAgeGroup = inputCols[curIdxAgeGroup];
							String curEventGroup = "";
							if ( inNcwsa && ( curAgeGroup.ToUpper().Equals( "CM" ) || curAgeGroup.ToUpper().Equals( "CW" ) ) ) {
								curImportMemberEntry["AgeGroup"] = curAgeGroup;
								curImportMemberEntry["Eventroup"] = curEventGroup;

							} else if ( curAgeGroup.ToUpper().Equals( "M" ) || curAgeGroup.ToUpper().Equals( "W" ) ) {
								curImportMemberEntry["AgeGroup"] = "";
								curImportMemberEntry["Eventroup"] = curAgeGroup;

							} else {
								curImportMemberEntry["AgeGroup"] = curAgeGroup;
								curImportMemberEntry["Eventroup"] = "";
							}

							if ( inputCols.Length < 11 ) {
                                //There must be at 11 columns of data otherwise the event run order values are considered unavailable
                            } else {
                                int numCk = 0;
                                if ( int.TryParse( inputCols[curIdxSlalomRunOrder], out numCk ) ) {
									curImportMemberEntry["SlalomRunOrder"] = numCk;
                                }
                                if ( int.TryParse( inputCols[curIdxTrickRunOrder], out numCk ) ) {
									curImportMemberEntry["TrickRunOrder"] = numCk; 
								}
								if ( int.TryParse( inputCols[curIdxJumpRunOrder], out numCk ) ) {
									curImportMemberEntry["JumpRunOrder"] = numCk;
								}
							}
                        }
						#endregion
                    }

					return myImportMember.procTeamHeaderInput( curImportMemberEntry, inNcwsa );

				} else {
					return false;
				}

			} else {
				return false;
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
            String myFileName = "", curPath = "";
            OpenFileDialog myFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
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

        private StreamReader getImportFile(String inFileName) {
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
                if (inFileName.EndsWith( ".tmp" )) {
                } else {
                    MessageBox.Show( "Error: Could not get an import file to process " + "\n\nError: " + ex.Message );
                }
            }

            return myReader;
        }

        private String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
            StringBuilder curNewValue = new StringBuilder( "" );

            String[] curValues = inValue.Split( inCurValue );
            if ( curValues.Length > 1 ) {
                int curCount = 0;
                foreach ( String curValue in curValues ) {
                    curCount++;
                    if ( curCount < curValues.Length ) {
                        curNewValue.Append(curValue + inReplValue);
                    } else {
                        curNewValue.Append(curValue);
                    }
                }
            } else {
                curNewValue.Append( inValue );
            }

            return curNewValue.ToString();
        }
        
		private DataRow getTourData() {
			Decimal curDatabaseVersion = 0M;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue as VersionNumText, MinValue as VersionNum " );
			curSqlStmt.Append( "FROM CodeValueList WHERE ListName = 'DatabaseVersion'");
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				curDatabaseVersion = (decimal) curDataTable.Rows[0]["VersionNum"];
			} else {
				curDatabaseVersion = 0M;
			}

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
			if ( curDatabaseVersion >= 4.15M ) {
				curSqlStmt.Append( ", SanctionEditCode" );
			}
			curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
			curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
			curSqlStmt.Append( "FROM Tournament T " );
			curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
			curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );

			curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

	}
}
