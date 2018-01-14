using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    class ImportData {
        private String mySanctionNum;
        private string myTableName = null;
        private DateTime myImportFileDate;
        private char[] myTabDelim = new char[] { '\t' };
        private char[] mySingleQuoteDelim = new char[] { '\'' };

        private int myCountMemberInput = 0;
        private int myCountMemberAdded = 0;
        private int myCountMemberUpdate = 0;
        private int myCountTourRegAdded = 0;
        private int myCountSlalomAdded = 0;
        private int myCountTrickAdded = 0;
        private int myCountJumpAdded = 0;

        private ImportMatchDialogForm MatchDialog;
        private TourEventReg myTourEventReg;
        private ProgressWindow myProgressInfo;
        private MemberIdValidate myMemberIdValidate;
		private ImportOfficialRatings myImportOfficialRatings;

		public ImportData() {
            MatchDialog = new ImportMatchDialogForm();
        }

        public String TableName {
            get {
                return myTableName;
            }
            set {
                myTableName = value;
            }
        }

        public DateTime ImportFileDate {
            get {
                return myImportFileDate;
            }
            set {
                myImportFileDate = value;
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
            string inputBuffer, colValue, MatchCommand = "", curMatchCommand = "", curLastUpdateDateIn = "";
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
            } else {
                curFileList.Add( inFileName );
                mySanctionNum = "";
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
														) {
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
											) {
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
            string[] inputCols;
            string[] inputColsSaved = null;
            DateTime curFileDate;
            StreamReader myReader;
            DialogResult msgResp;
            int numCk = 0, idxMemberId = 0;
            int curInputLineCount = 0;
            myTourEventReg = new TourEventReg();
            myMemberIdValidate = new MemberIdValidate();
            
            myCountMemberInput = 0;
            myCountMemberAdded = 0;
            myCountMemberUpdate = 0;
            myCountTourRegAdded = 0;
            myCountSlalomAdded = 0;
            myCountTrickAdded = 0;
            myCountJumpAdded = 0;

            //Choose an input file to be processed
            String curPath = Properties.Settings.Default.ExportDirectory;
            OpenFileDialog myFileDialog = new OpenFileDialog();

			myImportOfficialRatings = new ImportOfficialRatings( null );

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
				curFileDate = File.GetLastWriteTime( myfileName );
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
							inputCols = inputBuffer.Split( myTabDelim );
							if ( inputCols.Length > 3 ) {
								//Check first line of input file to analyze the file format supplied
								// WSTIMS Rel 4.0+ NCWSA Registration Worksheet
								if ( curInputLineCount == 1 && curNcwsa == false ) {
									if ( inputCols[0].ToLower().IndexOf( "wstims" ) > -1 && inputCols[0].ToLower().IndexOf( "registration worksheet" ) > -1 ) {
										if ( inputCols[0].ToLower().IndexOf( "wstims" ) > -1 ) {
											curNcwsa = true;
										} else {
											curTourRegFmt = true;
										}
									} else if ( inputCols[0].ToLower().IndexOf( "wstims" ) > -1 && inputCols[0].ToLower().IndexOf( "ncwsa" ) > -1 ) {
										curNcwsa = true;
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
										curTeamHeaderActive = false;
										if ( curNcwsa ) {
											inputColsSaved = new String[inputCols.Length];
											curReturn = procMemberInput( inputCols, MemberId, curFileDate, curTourRegFmt, curNcwsa, inputColsSaved );

										} else {
											curReturn = procMemberInput( inputCols, MemberId, curFileDate, curTourRegFmt, curNcwsa, null );
										}
										if ( curReturn ) {
											if ( curNcwsa && inputColsSaved != null ) {
												if ( inputColsSaved[0] != null ) {
													curReturn = procMemberInput( inputColsSaved, MemberId, curFileDate, curTourRegFmt, curNcwsa, null );
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

									} else {
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
						}
					}
					MessageBox.Show( "Info: Member import processed"
						+ "\nMember records read: " + myCountMemberInput
						+ "\nmyCountMemberAdded: " + myCountMemberAdded
						+ "\nmyCountMemberUpdate: " + myCountMemberUpdate
						+ "\nmyCountTourRegAdded: " + myCountTourRegAdded
						+ "\nmyCountSlalomAdded: " + myCountSlalomAdded
						+ "\nmyCountTrickAdded: " + myCountTrickAdded
						+ "\nmyCountJumpAdded: " + myCountJumpAdded
						);
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

        private bool procMemberInput(string[] inputCols, string MemberId, DateTime curFileDate, bool inTourReg, bool inNcwsa, string[] inputColsSaved) {
            bool newMember = false, curReqstStatus = false, curDataVaid = true;
            string curEvent, curNote, curScore, ExpireDate, MemberStatus, SkiYearAge, Gender, curAppointedOfficialCode;
            string curSqlStmt = "";
            DataTable curDataTable;
            DateTime lastRecModDate = new DateTime();
            Decimal curSlalom = 0, curTrick = 0, curJump = 0, curOverall = 0, numDecCk = 0;
            long rankingPK = 0;
			int rowsProc = 0;
            int numCk = 0,
                idxMemberId = 0, idxLastName = 1, idxFirstName = 2,
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

            try {
                #region Prepare indexes depending on the type of input file 
                if ( inTourReg ) {
                    if ( inputCols.Length < 28 ) {
                        MessageBox.Show( "Invalid tournament registration record detected. Bypassing record"
                            + "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                        );
                        return false;
                    }

				} else if ( inNcwsa ) {
                    idxTrickBoat = 13;
                    idxJumpHeight = 14;
                    idxSportDiv = 16;
                    idxMemberStatus = 17;
                    idxNote = 17;
					idxUpgradeAmt = 18;
					idxSlalomRank = 0;
                    idxTrickRank = 0;
                    idxJumpRank = 0;
                    idxSlalomRating = 0;
                    idxTrickRating = 0;
                    idxJumpRating = 0;
                    idxOverallRating = 0;

					if ( inputCols.Length < 11 ) {
                        MessageBox.Show( "Invalid tournament registration record detected. Bypassing record"
                            + "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                        );
                        return false;

					} else {
                        if ( inputCols.Length < 13 ) {
                            idxMemberStatus = 0;
                        }
                        if ( inputCols.Length < 15 ) {
                            idxNote = 0;
							idxUpgradeAmt = 0;
							idxExpireDate = 0;
							idxSportDiv = 0;
							idxMemberStatus = 0;
							idxNote = 0;
						}
						if ( inputCols.Length < 12 ) {
							idxApptdOfficial = 0;
                        }

                        //Check for A team or B team designation unless skier is an official or not assigned to a team
                        if ( inNcwsa ) {
                            if ( inputCols[idxTeam].ToUpper().Equals( "OFF" )
                                || inputCols[idxTeam].Length == 0 ) { 
                                //This skier is an official or non assigned to a team so no validation is required
                            } else {
                                curDataVaid = calcNcwsaDiv( inputCols, inputColsSaved, idxMemberId, idxFirstName, idxLastName, idxAgeGroup, idxEventSlalom, idxEventTrick, idxEventJump );
                                if (curDataVaid) {
                                } else {
                                    return false;
                                }
                            }
                        }
                    }

				} else {
                    if ( inputCols.Length < 22 ) {
                        MessageBox.Show( "Invalid tournament registration record detected. Bypassing record"
                            + "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                        );
                        return false;
                    } else {
                        if ( inputCols.Length > 23 ) {
                            if ( inputCols.Length > 24 ) {
                            } else {
                                idxNote = 0;
								idxUpgradeAmt = 0;
								idxExpireDate = 0;
							}
						} else {
                            idxMemberStatus = 0;
							idxUpgradeAmt = 0;
							idxExpireDate = 0;
						}
					}
                }
                
				//Validate the member id to ensure it is valid
                if ( myMemberIdValidate.checkMemberId( MemberId) ) {
                } else {
                    MessageBox.Show( "Invalid member id, checksum validation failed." );
                    return false;
                }

                #endregion

                curSqlStmt = "Select MemberId, UpdateDate from MemberList Where MemberId = '" + MemberId + "'";
				curDataTable = DataAccess.getDataTable( curSqlStmt );
				if ( curDataTable.Rows.Count > 0 ) {
                    lastRecModDate = (DateTime)curDataTable.Rows[0]["UpdateDate"];
					myCountMemberUpdate++;
				} else {
                    newMember = true;
					myCountMemberAdded++;
				}

				#region Retrieve and analyze data for processing
				curAppointedOfficialCode = "";
                if ( idxApptdOfficial > 0 ) {
					curAppointedOfficialCode = inputCols[idxApptdOfficial].Trim();
				}

				//OF in age group indicates an official for the current tournament
				if ( inputCols[idxAgeGroup].Length > 1 ) {
                    Gender = myTourEventReg.getGenderOfAgeDiv( inputCols[idxAgeGroup].ToUpper() );
                } else {
                    Gender = "";
                }

                inputCols[idxLastName] = stringReplace(inputCols[idxLastName], mySingleQuoteDelim, "''");
                inputCols[idxFirstName] = stringReplace(inputCols[idxFirstName], mySingleQuoteDelim, "''");
                inputCols[idxCity] = stringReplace(inputCols[idxCity], mySingleQuoteDelim, "''");
                if ( inputCols[idxSkiYearAge].Length > 0 ) {
                    if ( int.TryParse( inputCols[idxSkiYearAge], out numCk ) ) {
                        SkiYearAge = inputCols[idxSkiYearAge];
                        if ( numCk < 0 ) {
                            SkiYearAge = "0";
                        }
                    } else {
                        SkiYearAge = "0";
                    }
                } else {
                    SkiYearAge = "0";
                }
                if ( idxMemberStatus > 0 ) {
                    if ( inputCols[idxMemberStatus].ToLower().Trim().Equals( "yes" ) ) {
                        MemberStatus = "Active";
						ExpireDate = inputCols[idxExpireDate];

					} else if ( inputCols[idxMemberStatus].ToLower().Trim().Equals( "no" ) ) {
                        MemberStatus = "Inactive";
                        ExpireDate = inputCols[idxExpireDate];

					} else if ( inputCols[idxMemberStatus] == null ) {
                        MemberStatus = "";
                        ExpireDate = "";

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "ok to ski" ) > -1 ) {
                        MemberStatus = "Active";
						ExpireDate = inputCols[idxExpireDate];

					} else if ( inputCols[idxMemberStatus].ToLower().IndexOf( "pre-reg" ) > -1 ) {
                        MemberStatus = "Active";
						ExpireDate = inputCols[idxExpireDate];

					} else if (inputCols[idxMemberStatus].ToLower().IndexOf( "nds evt wvr" ) > -1) {
                        MemberStatus = "Active";
						ExpireDate = inputCols[idxExpireDate];

					} else {
                        if (inNcwsa) {
                            if (inputCols[idxEventSlalom].Length > 0 || inputCols[idxEventTrick].Length > 0 || inputCols[idxEventJump].Length > 0) {
                                MemberStatus = "Active";
                                ExpireDate = "12/31/" + System.DateTime.Now.Year.ToString();
                            } else {
                                MemberStatus = inputCols[idxMemberStatus];
                                ExpireDate = "";
                            }
                        } else {
                            MemberStatus = inputCols[idxMemberStatus];
							ExpireDate = inputCols[idxExpireDate];
						}
					}

				} else {
                    if (inNcwsa) {
                        if (inputCols[idxEventSlalom].Length > 0 || inputCols[idxEventTrick].Length > 0 || inputCols[idxEventJump].Length > 0) {
                            MemberStatus = "Active";
                            ExpireDate = "12/31/" + System.DateTime.Now.Year.ToString();
                        } else {
                            MemberStatus = "";
                            ExpireDate = "";
                        }
                    } else {
                        MemberStatus = "";
                        ExpireDate = "";
                    }
                }

				if (MemberStatus.Length > 12) MemberStatus = MemberStatus.Substring( 0, 12 );
                try {
                    if ( idxNote > 0 && idxNote < inputCols.Length ) {
                        if ( inputCols[idxNote] == null ) {
                            curNote = "";
                        } else {
                            curNote = stringReplace(inputCols[idxNote], mySingleQuoteDelim, "''");
                            if ( idxUpgradeAmt > 0 && idxUpgradeAmt < inputCols.Length ) {
                                if ( inputCols[idxUpgradeAmt] != null ) {
                                    curNote += " " + inputCols[idxUpgradeAmt];
                                }
                            }
                        }
                    } else {
                        curNote = "";
                    }
                } catch {
                    curNote = "";
                }
				#endregion

				#region Insert or update member record and update offical ratings
				Dictionary<string, object> curOfficalEntry = new Dictionary<string, object>();
				// inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
				curOfficalEntry.Add( "MemberID", MemberId );
				curOfficalEntry.Add( "FirstName", inputCols[idxFirstName] );
				curOfficalEntry.Add( "LastName", inputCols[idxLastName] );

				curOfficalEntry.Add( "Federation", "USA" );
				curOfficalEntry.Add( "MembershipTypeCode", 0 );
				curOfficalEntry.Add( "ActiveMember", MemberStatus );
				curOfficalEntry.Add( "MemTypeDesc", "" );
				curOfficalEntry.Add( "Gender", Gender );
				curOfficalEntry.Add( "City", inputCols[idxCity] );
				curOfficalEntry.Add( "State", inputCols[idxState] );
				curOfficalEntry.Add( "Age", int.Parse( SkiYearAge ) );
				curOfficalEntry.Add( "EffTo", ExpireDate );
				curOfficalEntry.Add( "Note", curNote );
				curOfficalEntry.Add( "CanSki", true );
				curOfficalEntry.Add( "CanSkiGR", true );
				curOfficalEntry.Add( "Waiver", 0 );

				curOfficalEntry.Add( "JudgeSlalom", inputCols[idxJudgeSlalomRating] );
				curOfficalEntry.Add( "JudgeTrick", inputCols[idxJudgeTrickRating] );
				curOfficalEntry.Add( "JudgeJump", inputCols[idxJudgeJumpRating] );

				curOfficalEntry.Add( "DriverSlalom", inputCols[idxDriverSlalomRating] );
				curOfficalEntry.Add( "DriverTrick", inputCols[idxDriverTrickRating] );
				curOfficalEntry.Add( "DriverJump", inputCols[idxDriverJumpRating] );

				curOfficalEntry.Add( "ScorerSlalom", inputCols[idxScorerSlalomRating] );
				curOfficalEntry.Add( "ScorerTrick", inputCols[idxScorerTrickRating] );
				curOfficalEntry.Add( "ScorerJump", inputCols[idxScorerJumpRating] );

				curOfficalEntry.Add( "Safety", inputCols[idxSafetyRating] );
				curOfficalEntry.Add( "TechController", inputCols[idxTechCntlrRating] );

				myImportOfficialRatings.importMembersAndRating( curOfficalEntry );
				#endregion

				#region Insert or update skier slalom ranking data
				if ( idxSlalomRank > 0 ) {
                    curScore = inputCols[idxSlalomRank];
                    if ( curScore.Length > 1 ) {
                        if ( Decimal.TryParse( curScore, out numDecCk ) ) {
                            curSlalom = Convert.ToDecimal( curScore );
                            curEvent = "Slalom";
                            curOverall += curSlalom;

                            curSqlStmt = "Select PK from SkierRanking "
                                + "Where MemberId = '" + MemberId + "' "
                                + "  And Event = '" + curEvent + "'";
							curDataTable = DataAccess.getDataTable( curSqlStmt );
							if ( curDataTable.Rows.Count > 0 ) {
                                rankingPK = (Int64)curDataTable.Rows[0]["PK"];
                                curSqlStmt = "Update SkierRanking "
                                    + " Set Score = " + curSlalom.ToString()
                                    + ", Rating = '" + inputCols[idxSlalomRating] + "'"
                                    + ", AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + " Where PK = " + rankingPK;
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

							} else {
                                curSqlStmt = "Insert SkierRanking ("
                                    + "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup"
                                    + ") Values ("
                                    + "'" + MemberId + "'"
                                    + ", '" + curEvent + "', '', 1"
                                    + ", " + curSlalom.ToString()
                                    + ", '" + inputCols[idxSlalomRating] + "'"
                                    + ", '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + ")";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
							}

							curSqlStmt = "Update EventReg "
                                + " Set RankingScore = " + curSlalom.ToString()
                                + ", RankingRating = '" + inputCols[idxSlalomRating] + "'"
                                + " Where SanctionId = '" + mySanctionNum + "'"
                                + "   And MemberId = '" + MemberId + "'"
                                + "   And AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                + "   And Event = '" + curEvent + "'";
							rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
                        }
                    }
                }
                #endregion

                #region Insert or update skier trick ranking data
                curScore = inputCols[idxTrickRank];
                if ( idxTrickRank > 0 ) {
                    curScore = inputCols[idxTrickRank];
                    if ( curScore.Length > 1 ) {
                        if ( Decimal.TryParse( curScore, out numDecCk ) ) {
                            curEvent = "Trick";
                            curTrick = Convert.ToDecimal( curScore );
                            curOverall += curTrick;

                            curSqlStmt = "Select PK from SkierRanking "
                                + "Where MemberId = '" + MemberId + "' "
                                + "  And Event = '" + curEvent + "'";
							curDataTable = DataAccess.getDataTable( curSqlStmt );
							if ( curDataTable.Rows.Count > 0 ) {
                                rankingPK = (Int64)curDataTable.Rows[0]["PK"];
                                curSqlStmt = "Update SkierRanking "
                                    + " Set Score = " + curTrick.ToString()
                                    + ", Rating = '" + inputCols[idxTrickRating] + "'"
                                    + ", AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + " Where PK = " + rankingPK;
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

							} else {
                                curSqlStmt = "Insert SkierRanking ("
                                    + "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup"
                                    + ") Values ("
                                    + "'" + MemberId + "'"
                                    + ", '" + curEvent + "', '', 1"
                                    + ", " + curTrick.ToString()
                                    + ", '" + inputCols[idxTrickRating] + "'"
                                    + ", '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + ")";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
							}

							curSqlStmt = "Update EventReg "
                                + " Set RankingScore = " + curTrick.ToString()
                                + ", RankingRating = '" + inputCols[idxSlalomRating] + "'"
                                + " Where SanctionId = '" + mySanctionNum + "'"
                                + "   And MemberId = '" + MemberId + "'"
                                + "   And AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                + "   And Event = '" + curEvent + "'";
							rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
						}
					}
                }
                #endregion

                #region Insert or update skier jump ranking data
                curScore = inputCols[idxJumpRank];
                if ( idxJumpRank > 0 ) {
                    curScore = inputCols[idxJumpRank];
                    if ( curScore.Length > 1 ) {
                        if ( Decimal.TryParse( curScore, out numDecCk ) ) {
                            curEvent = "Jump";
                            curJump = Convert.ToDecimal( curScore );
                            curOverall += curJump;

                            curSqlStmt = "Select PK from SkierRanking "
                                + "Where MemberId = '" + MemberId + "' "
                                + "  And Event = '" + curEvent + "'";
							curDataTable = DataAccess.getDataTable( curSqlStmt );
							if ( curDataTable.Rows.Count > 0 ) {
                                rankingPK = (Int64)curDataTable.Rows[0]["PK"];
                                curSqlStmt = "Update SkierRanking "
                                    + " Set Score = " + curJump.ToString()
                                    + ", Rating = '" + inputCols[idxJumpRating] + "'"
                                    + ", AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + " Where PK = " + rankingPK;
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

							} else {
                                curSqlStmt = "Insert SkierRanking ("
                                    + "MemberId, Event, Notes, SeqNum, Score, Rating, AgeGroup"
                                    + ") Values ("
                                    + "'" + MemberId + "'"
                                    + ", '" + curEvent + "', '', 1"
                                    + ", " + curJump.ToString()
                                    + ", '" + inputCols[idxJumpRating] + "'"
                                    + ", '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                    + ")";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
							}
							curSqlStmt = "Update EventReg "
                                + " Set RankingScore = " + curJump.ToString()
                                + ", RankingRating = '" + inputCols[idxSlalomRating] + "'"
                                + " Where SanctionId = '" + mySanctionNum + "'"
                                + "   And MemberId = '" + MemberId + "'"
                                + "   And AgeGroup = '" + inputCols[idxAgeGroup].ToUpper() + "'"
                                + "   And Event = '" + curEvent + "'";
							rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
						}
					}
                }
                #endregion

                #region Register member in tournament events as indicated

                if ( (inputCols[idxAgeGroup].ToLower().Equals( "of" ) )
                    || ( inputCols[idxTeam].ToLower().Equals( "off" ) )
                    || ( inputCols[idxEventSlalom].Trim().Length > 0 ) 
                    || ( inputCols[idxEventTrick].Trim().Length > 0 ) 
                    || ( inputCols[idxEventJump].Trim().Length > 0 ) 
                    ) {
                    if (myTourEventReg.validAgeDiv( inputCols[idxAgeGroup].ToUpper() )) {
                    } else {
                        MessageBox.Show( "Invalid age group " + inputCols[idxAgeGroup].ToUpper() + " detected. Bypassing tournament registration"
                            + "\n" + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                        );
                        return false;
                    }
                }
                
                String curEventGroup = "", curEventClass = "", curTrickBoat = "", curJumpHeight = "", inPreRegNote = "";
                if (idxTrickBoat > 0) {
                    curTrickBoat = inputCols[idxTrickBoat];
                }
                if (idxJumpHeight > 0) {
                    curJumpHeight = inputCols[idxJumpHeight];
                }
                if (curJumpHeight.Length == 0) {
                    curJumpHeight = "0";
                } else {
                    try {
                        Decimal tmpJumpHeight = Convert.ToDecimal( curJumpHeight );
                        if (tmpJumpHeight > 6) {
                            tmpJumpHeight = tmpJumpHeight / 10;
                            curJumpHeight = tmpJumpHeight.ToString( "#.#" );
                        }
                        if (inNcwsa) {
                            if (tmpJumpHeight < Convert.ToDecimal( "5.0" )) {
                                if (inputCols[idxEventJump].Substring( 0, 1 ).ToUpper().Equals( "B" )) {
                                } else {
                                    curJumpHeight = "5.0";
                                }
                            }
                        }
                    } catch {
                        curJumpHeight = "0";
                    }
                }
                if (inTourReg) {
                    try {
                        if ( idxNote > 0 ) {
                            inPreRegNote = inputCols[idxNote];
                        } else {
                            inPreRegNote = "Tour Reg Template";
                        }
                    } catch {
                        inPreRegNote = "Tour Reg Template";
                    }
                } else {
                    inPreRegNote = "Tour Reg Template";
                }

                if ( inputCols[idxAgeGroup].ToLower().Equals( "of" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;

				} else {
                    if ( inputCols[idxTeam].ToLower().Equals( "off" ) ) {
                        curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                        if ( curReqstStatus ) myCountTourRegAdded++;
                    } 
                    if ( inputCols[idxEventSlalom].Trim().Length > 0 ) {
                        curEventGroup = inputCols[idxEventSlalom];
                        if ( inTourReg ) {
                            try {
                                if ( inputCols.Length >= idxEventClassSlalom ) {
                                    if ( inputCols[idxEventClassSlalom].Trim().Length > 0 ) {
                                        curEventClass = inputCols[idxEventClassSlalom].Substring( 0, 1 );
                                    } else {
                                        curEventClass = "";
                                    }
                                } else {
                                    curEventClass = "";
                                }
                            } catch {
                                curEventClass = "";
                            }
                        } else {
                            curEventClass = "";
                        }
                        curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                        if ( curReqstStatus ) myCountTourRegAdded++;
                        if ( inputCols[idxEventSlalom].ToLower().Equals( "of" ) ) {
                        } else {
                            curReqstStatus = myTourEventReg.addEventSlalom( MemberId, inputCols[idxEventSlalom], curEventClass, inputCols[idxAgeGroup].ToUpper(), inputCols[idxTeam] );
                            if ( curReqstStatus ) myCountSlalomAdded++;
                        }
                    }

					if ( inputCols[idxEventTrick].Trim().Length > 0 ) {
                        curEventGroup = inputCols[idxEventTrick];
                        if ( inTourReg ) {
                            try {
                                if ( inputCols.Length > idxEventClassTrick ) {
                                    if ( inputCols[idxEventClassTrick].Trim().Length > 0 ) {
                                        curEventClass = inputCols[idxEventClassTrick].Substring( 0, 1 );
                                    } else {
                                        curEventClass = "";
                                    }
                                } else {
                                    curEventClass = "";
                                }
                            } catch {
                                curEventClass = "";
                            }
                        } else {
                            curEventClass = "";
                        }
                        curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                        if ( curReqstStatus ) myCountTourRegAdded++;
                        if ( inputCols[idxEventTrick].ToLower().Equals( "of" ) ) {
                        } else {
                            curReqstStatus = myTourEventReg.addEventTrick( MemberId, inputCols[idxEventTrick], curEventClass, inputCols[idxAgeGroup].ToUpper(), inputCols[idxTeam] );
                            if ( curReqstStatus ) myCountTrickAdded++;
                        }
                    }

					if ( inputCols[idxEventJump].Trim().Length > 0 ) {
                        curEventGroup = inputCols[idxEventJump];
                        if ( inputCols[idxAgeGroup].ToUpper().Equals( "B1" )
                            || inputCols[idxAgeGroup].ToUpper().Equals( "G1" ) ) {
                            MessageBox.Show( "Jump event not allowed for B1 or G1 divisions.\nSkipping event registration." );
                        } else {
                            if ( inTourReg ) {
                                try {
                                    if ( inputCols.Length > idxEventClassJump ) {
                                        if ( inputCols[idxEventClassJump].Trim().Length > 0 ) {
                                            curEventClass = inputCols[idxEventClassJump].Substring( 0, 1 );
                                        } else {
                                            curEventClass = "";
                                        }
                                    } else {
                                        curEventClass = "";
                                    }
                                } catch {
                                    curEventClass = "";
                                }
                            } else {
                                curEventClass = "";
                            }
                            curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                            if ( curReqstStatus ) myCountTourRegAdded++;
                            if ( inputCols[idxEventJump].ToLower().Equals( "of" ) ) {
                            } else {
                                curReqstStatus = myTourEventReg.addEventJump( MemberId, inputCols[idxEventJump], curEventClass, inputCols[idxAgeGroup].ToUpper(), inputCols[idxTeam] );
                                if ( curReqstStatus ) myCountJumpAdded++;
                            }
                        }
                    }

					if ( inputCols[idxTeam].Length > 0 ) {
                        if ( !(inNcwsa) ) {
                            string[] curTeamHeaderCols = { "TeamHeader", inputCols[idxTeam], "", inputCols[idxTeam] };
                            procTeamHeaderInput( curTeamHeaderCols, inNcwsa );
                        }

                    }
                }

				/*
				 * Mark officials that are indicated as the chief, assistant chief, or appointed official ratings
				*/
				if ( curAppointedOfficialCode.Equals( "CJ" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "JudgeChief" );

				} else if ( curAppointedOfficialCode.Equals( "ACJ" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "JudgeAsstChief" );

				} else if ( curAppointedOfficialCode.Equals( "APTJ" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "JudgeAppointed" );

				} else if ( curAppointedOfficialCode.Equals( "CD" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "DriverChief" );

				} else if ( curAppointedOfficialCode.Equals( "ACD" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "DriverAsstChief" );

				} else if ( curAppointedOfficialCode.Equals( "APTD" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "DriverAppointed" );

				} else if ( curAppointedOfficialCode.Equals( "CC" )) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "ScoreChief" );

				} else if ( curAppointedOfficialCode.Equals( "ACC" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "ScoreAsstChief" );

				} else if ( curAppointedOfficialCode.Equals( "APTS" ) ) {
					curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
					if ( curReqstStatus ) myCountTourRegAdded++;
					curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "ScoreAppointed" );

				} else if ( curAppointedOfficialCode.Equals( "CS" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "SafetyChief" );

				} else if ( curAppointedOfficialCode.Equals( "ACS" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "SafetyAsstChief" );

				} else if ( curAppointedOfficialCode.Equals("CT")) {
                    curReqstStatus = myTourEventReg.addTourReg(MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight);
                    if (curReqstStatus) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial(MemberId, "TechChief");

				} else if ( curAppointedOfficialCode.Equals("ACT")) {
                    curReqstStatus = myTourEventReg.addTourReg(MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight);
                    if (curReqstStatus) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial(MemberId, "TechAsstChief");

				} else if ( curAppointedOfficialCode.Equals( "CA" ) ) {
                    curReqstStatus = myTourEventReg.addTourReg( MemberId, inPreRegNote, inputCols[idxAgeGroup].ToUpper(), curTrickBoat, curJumpHeight );
                    if ( curReqstStatus ) myCountTourRegAdded++;
                    curReqstStatus = myTourEventReg.addEventOfficial( MemberId, "AnncrChief" );
                }

				#endregion

				return true;

			} catch ( Exception ex ) {
                String ExcpMsg = "Error Processing Member "
                    + inputCols[idxMemberId] + " " + inputCols[idxFirstName] + " " + inputCols[idxLastName]
                    + "\n\n " + ex.Message;
                /*
                if ( sqlStmt != null ) {
                    ExcpMsg += "\n\n SQL Statement: " + curSqlStmt;
                }
                 */
                MessageBox.Show( ExcpMsg );
				return false;
            }
        }

        private bool procTeamHeaderInput( string[] inputCols, bool inNcwsa ) {
            bool curReturnValue = false;
            String curSqlStmt = "", curTeamName = "", curTeamCode = "", curAgeGroup= "";
             
            String curTeamSlalomRunOrder = "1", curTeamTrickRunOrder = "1", curTeamJumpRunOrder = "1";
            Int64 curTeamPK, curOrderPK;
            int rowsProc = 0;
            int curIdxTeamName = 1, curIdxTeamCode = 3, curIdxAgeGroup = 4;
            int curIdxSlalomRunOrder = 8, curIdxTrickRunOrder = 9, curIdxJumpRunOrder = 10;
            DataTable curDataTable;

            if ( inputCols.Length > 3 ) {
                if ( ( inputCols[1].Length > 0 ) && inputCols[3].Length > 0 ) {
                    curTeamName = inputCols[curIdxTeamName];
                    curTeamCode = inputCols[curIdxTeamCode];
                    curAgeGroup = "";
                    if ( !( curTeamCode.ToLower().Equals( "off" ) ) ) {
                        curTeamSlalomRunOrder = "0";
                        curTeamTrickRunOrder = "0";
                        curTeamJumpRunOrder = "0";
                        #region check for team rotation assignments
                        if ( inputCols.Length > 4 ) {
                            curAgeGroup = inputCols[curIdxAgeGroup];
                            if ( inputCols.Length < 11 ) {
                                MessageBox.Show( "" );
                            } else {
                                int numCk = 0;
                                if ( int.TryParse( inputCols[curIdxSlalomRunOrder], out numCk ) ) {
                                    curTeamSlalomRunOrder = inputCols[curIdxSlalomRunOrder];
                                } else {
                                    curTeamSlalomRunOrder = "0";
                                }
                                if ( int.TryParse( inputCols[curIdxTrickRunOrder], out numCk ) ) {
                                    curTeamTrickRunOrder = inputCols[curIdxTrickRunOrder];
                                } else {
                                    curTeamTrickRunOrder = "0";
                                }
                                if ( int.TryParse( inputCols[curIdxJumpRunOrder], out numCk ) ) {
                                    curTeamJumpRunOrder = inputCols[curIdxJumpRunOrder];
                                } else {
                                    curTeamJumpRunOrder = "0";
                                }
                            }
                        }
						#endregion

						curSqlStmt = "Select PK from TeamList " 
                            + "Where SanctionId = '" + mySanctionNum + "' "
                            + "  And TeamCode = '" + curTeamCode + "'";
						curDataTable = DataAccess.getDataTable( curSqlStmt );
						if ( curDataTable.Rows.Count > 0 ) {
                            if ( inputCols.Length > 4 ) {
                                #region Update team information if a division or group is provided
                                curTeamPK = (Int64)curDataTable.Rows[0]["PK"];
								curSqlStmt = "Update TeamList "
                                    + "Set Name = '" + curTeamName + "'"
                                    + ", LastUpdateDate = getdate() "
                                    + "Where PK = " + curTeamPK;
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt);

                                curSqlStmt = "Select PK from TeamOrder "
                                    + "Where SanctionId = '" + mySanctionNum + "' "
                                    + "  And TeamCode = '" + curTeamCode + "'"
                                    + "  And ( AgeGroup = '" + curAgeGroup + "' OR EventGroup = '" + curAgeGroup + "')";
								curDataTable = DataAccess.getDataTable( curSqlStmt );
                                if ( curDataTable.Rows.Count > 0 ) {
                                    curOrderPK = (Int64)curDataTable.Rows[0]["PK"];
									curSqlStmt = "Update TeamOrder "
                                        + "Set SlalomRunOrder = " + curTeamSlalomRunOrder + " "
                                        + ", TrickRunOrder = " + curTeamTrickRunOrder + " "
                                        + ", JumpRunOrder = " + curTeamJumpRunOrder + " "
                                        + "Where PK = " + curOrderPK;
									rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
									if ( rowsProc > 0 ) curReturnValue = true;

								} else {
                                    // myAgeDivList.validAgeDiv(inAgeDiv);
                                    if (curAgeGroup.ToUpper().Equals( "M" ) || curAgeGroup.ToUpper().Equals( "W" )) {
										curSqlStmt = "Insert TeamOrder ("
                                            + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                            + ") Values ("
                                            + "'" + mySanctionNum + "'"
                                            + ", '" + curTeamCode + "'"
                                            + ", '', '" + curAgeGroup + "'"
                                            + ", " + curTeamSlalomRunOrder
                                            + ", " + curTeamTrickRunOrder
                                            + ", " + curTeamJumpRunOrder
                                            + ", getdate()"
                                            + ")";
                                    } else {
										curSqlStmt = "Insert TeamOrder ("
                                            + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                            + ") Values ("
                                            + "'" + mySanctionNum + "'"
                                            + ", '" + curTeamCode + "'"
                                            + ", '" + curAgeGroup + "', ''"
                                            + ", " + curTeamSlalomRunOrder
                                            + ", " + curTeamTrickRunOrder
                                            + ", " + curTeamJumpRunOrder
                                            + ", getdate()"
                                            + ")";
                                    }
									rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
									if ( rowsProc > 0 ) curReturnValue = true;
                                }
                                #endregion
                            }
                        } else {
							curSqlStmt = "Insert TeamList ("
                                + "SanctionId, Name, TeamCode, LastUpdateDate"
                                + ") Values ("
                                + "'" + mySanctionNum + "'"
                                + ", '" + curTeamName + "'"
                                + ", '" + curTeamCode + "'"
                                + ", getdate()"
                                + ")";
							rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

							if ( inNcwsa && ( curAgeGroup.ToUpper().Equals( "CM" ) || curAgeGroup.ToUpper().Equals( "CW" ) )) {
                                curSqlStmt = "Insert TeamOrder ("
                                    + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                    + ") Values ("
                                    + "'" + mySanctionNum + "', '" + curTeamCode + "', 'CM', ''"
                                    + ", " + curTeamSlalomRunOrder + ", " + curTeamTrickRunOrder + ", " + curTeamJumpRunOrder
                                    + ", getdate())";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

								curSqlStmt = "Insert TeamOrder ("
                                    + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                    + ") Values ("
                                    + "'" + mySanctionNum + "', '" + curTeamCode + "', 'CW', ''"
                                    + ", " + curTeamSlalomRunOrder + ", " + curTeamTrickRunOrder + ", " + curTeamJumpRunOrder
                                    + ", getdate())";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

								curSqlStmt = "Insert TeamOrder ("
                                    + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                    + ") Values ("
                                    + "'" + mySanctionNum + "', '" + curTeamCode + "', 'BM', ''"
                                    + ", " + curTeamSlalomRunOrder + ", " + curTeamTrickRunOrder + ", " + curTeamJumpRunOrder
                                    + ", getdate())";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );

								curSqlStmt = "Insert TeamOrder ("
                                    + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                    + ") Values ("
                                    + "'" + mySanctionNum + "', '" + curTeamCode + "', 'BW', ''"
                                    + ", " + curTeamSlalomRunOrder + ", " + curTeamTrickRunOrder + ", " + curTeamJumpRunOrder
                                    + ", getdate())";
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
								if ( rowsProc > 0 ) curReturnValue = true;

							} else {
                                // myAgeDivList.validAgeDiv(inAgeDiv);
                                if (curAgeGroup.ToUpper().Equals( "M" ) || curAgeGroup.ToUpper().Equals( "W" )) {
                                    curSqlStmt = "Insert TeamOrder ("
                                        + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                        + ") Values ("
                                        + "'" + mySanctionNum + "'"
                                        + ", '" + curTeamCode + "'"
                                        + ", '', '" + curAgeGroup + "'"
                                        + ", " + curTeamSlalomRunOrder
                                        + ", " + curTeamTrickRunOrder
                                        + ", " + curTeamJumpRunOrder
                                        + ", getdate()"
                                        + ")";
                                } else {
                                    curSqlStmt = "Insert TeamOrder ("
                                        + "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate"
                                        + ") Values ("
                                        + "'" + mySanctionNum + "'"
                                        + ", '" + curTeamCode + "'"
                                        + ", '" + curAgeGroup + "', ''"
                                        + ", " + curTeamSlalomRunOrder
                                        + ", " + curTeamTrickRunOrder
                                        + ", " + curTeamJumpRunOrder
                                        + ", getdate()"
                                        + ")";
                                }
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
								if ( rowsProc > 0 ) curReturnValue = true;
                            }
                        }
                    }
                }
            }

            return curReturnValue;
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

                        ImportFileDate = File.GetLastWriteTime( myFileName );
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
                    ImportFileDate = File.GetLastWriteTime( inFileName );
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

        private bool calcNcwsaDiv(String[] inputCols, String[] inputColsSaved, int idxMemberId, int idxFirstName, int idxLastName, int idxAgeGroup, int idxEventSlalom, int idxEventTrick, int idxEventJump) {
            String inMemberId = inputCols[idxMemberId];
            String inFirstName = inputCols[idxFirstName];
            String inLastName = inputCols[idxLastName];
            String inAgeGroup = inputCols[idxAgeGroup];
            String inEventGroupSlalom = inputCols[idxEventSlalom];
            String inEventGroupTrick = inputCols[idxEventTrick];
            String inEventGroupJump = inputCols[idxEventJump];
            String curTeamSlalom = "", curTeamTrick = "", curTeamJump = "";
            bool curDataValid = true;
            Int16 numIntCk;

            //For collegiate divisions determine if data is valid for collegiate tournaments
            if ( inAgeGroup.ToUpper().Equals( "CM" )
                || inAgeGroup.ToUpper().Equals( "CW" )
                || inAgeGroup.ToUpper().Equals( "BM" )
                || inAgeGroup.ToUpper().Equals( "BW" )
                ) {
                    #region Check slalom event group to validate for appropriate collegiate values
                    if (inEventGroupSlalom.ToLower().Equals( "of" )) {
                        //Skier is an official
                    } else {
                        if (inEventGroupSlalom.Length == 1) {
                            MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        } else if (inEventGroupSlalom.Length > 1) {
                            if (inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "A" )
                                || inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "B" )
                                ) {
                                curTeamSlalom = inEventGroupSlalom.Substring( 0, 1 ).ToUpper();
                                if (Int16.TryParse( inEventGroupSlalom.Substring( 1 ), out numIntCk )) {
                                } else {
                                    MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                        + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                    curDataValid = false;
                                }

                            } else {
                                MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }
                        }
                    }
                    #endregion

                    #region Check trick event group to validate for appropriate collegiate values
                    if (inEventGroupTrick.ToLower().Equals( "of" )) {
                        //Skier is an official
                    } else {
                        if (inEventGroupTrick.Length == 1) {
                            MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        } else if (inEventGroupTrick.Length > 1) {
                            if (inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "A" )
                                || inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "B" )
                                ) {
                                curTeamTrick = inEventGroupTrick.Substring( 0, 1 ).ToUpper();
                                if (Int16.TryParse( inEventGroupTrick.Substring( 1 ), out numIntCk )) {
                                } else {
                                    MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                        + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                    curDataValid = false;
                                }

                            } else {
                                MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }
                        }
                    }
                    #endregion

                    #region Check jump event group to validate for appropriate collegiate values
                    if (inEventGroupJump.ToLower().Equals( "of" )) {
                        //Skier is an official
                    } else {
                        if (inEventGroupJump.Length == 1) {
                            MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        } else if (inEventGroupJump.Length > 1) {
                            if (inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "A" )
                                || inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "B" )
                                ) {
                                curTeamJump = inEventGroupJump.Substring( 0, 1 ).ToUpper();
                                if (Int16.TryParse( inEventGroupJump.Substring( 1 ), out numIntCk )) {
                                } else {
                                    MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                        + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                    curDataValid = false;
                                }

                            } else {
                                MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A or B followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }
                        }
                    }
                    #endregion

                    if (curDataValid) {
                        if (curTeamSlalom.Equals( "" )) {
                            if (curTeamTrick.Equals( "" )) {
                                if (curTeamJump.Equals( "" )) {
                                    inputCols[idxAgeGroup] = inAgeGroup;
                                } else {
                                    #region Skier registered for jump only
                                    if (curTeamJump.Equals( "A" )) {
                                        inputCols[idxAgeGroup] = inAgeGroup;
                                    } else {
                                        if (inAgeGroup.ToUpper().Equals( "CM" )
                                            || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                            inputCols[idxAgeGroup] = "BM";
                                        } else {
                                            inputCols[idxAgeGroup] = "BW";
                                        }
                                    }
                                    #endregion
                                }
                            } else {
                                if (curTeamJump.Equals( "" )) {
                                    #region Skier registered for trick only
                                    if (curTeamTrick.Equals( "A" )) {
                                        inputCols[idxAgeGroup] = inAgeGroup;
                                    } else {
                                        if (inAgeGroup.ToUpper().Equals( "CM" )
                                            || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                            inputCols[idxAgeGroup] = "BM";
                                        } else {
                                            inputCols[idxAgeGroup] = "BW";
                                        }
                                    }
                                    #endregion
                                } else {
                                    if (curTeamTrick.Equals( curTeamJump )) {
                                        #region Skier registered for trick and jump
                                        if (curTeamTrick.Equals( "A" )) {
                                            inputCols[idxAgeGroup] = inAgeGroup;
                                        } else {
                                            if (inAgeGroup.ToUpper().Equals( "CM" )
                                                || inAgeGroup.ToUpper().Equals( "BM" )
                                                ) {
                                                inputCols[idxAgeGroup] = "BM";
                                            } else {
                                                inputCols[idxAgeGroup] = "BW";
                                            }
                                        }
                                        #endregion
                                    } else {
                                        #region Checking skiers that are assigned to both A and B team
                                        if (inputColsSaved == null) {
                                            curDataValid = false;
                                        } else {
                                            curDataValid = true;
                                            //Save current row of columns
                                            for (int curIdx = 0; curIdx < inputCols.Length; curIdx++) {
                                                inputColsSaved[curIdx] = inputCols[curIdx];
                                            }
                                            //Remove B team assignments for current row and only process A team assignments
                                            inputCols[idxAgeGroup] = inAgeGroup;
                                            if (curTeamSlalom.Equals( "B" )) {
                                                inputCols[idxEventSlalom] = "";
                                            }
                                            if (curTeamTrick.Equals( "B" )) {
                                                inputCols[idxEventTrick] = "";
                                            }
                                            if (curTeamJump.Equals( "B" )) {
                                                inputCols[idxEventJump] = "";
                                            }
                                            //Updated saved columns to indicate just the B team assignments
                                            if (inAgeGroup.ToUpper().Equals( "CM" )
                                                || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                                inputColsSaved[idxAgeGroup] = "BM";
                                            } else {
                                                inputColsSaved[idxAgeGroup] = "BW";
                                            }
                                            if (curTeamSlalom.Equals( "A" )) {
                                                inputColsSaved[idxEventSlalom] = "";
                                            }
                                            if (curTeamTrick.Equals( "A" )) {
                                                inputColsSaved[idxEventTrick] = "";
                                            }
                                            if (curTeamJump.Equals( "A" )) {
                                                inputColsSaved[idxEventJump] = "";
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }
                        } else {
                            if (curTeamTrick.Equals( "" )) {
                                if (curTeamJump.Equals( "" )) {
                                    #region Skier registered for slalom only
                                    if (curTeamSlalom.Equals( "A" )) {
                                        inputCols[idxAgeGroup] = inAgeGroup;
                                    } else {
                                        if (inAgeGroup.ToUpper().Equals( "CM" )
                                            || inAgeGroup.ToUpper().Equals( "BM" )
                                        ) {
                                            inputCols[idxAgeGroup] = "BM";
                                        } else {
                                            inputCols[idxAgeGroup] = "BW";
                                        }
                                    }
                                    #endregion
                                } else {
                                    #region Skier registered for slalom and jump
                                    if (curTeamSlalom.Equals( curTeamJump )) {
                                        if (curTeamSlalom.Equals( "A" )) {
                                            inputCols[idxAgeGroup] = inAgeGroup;
                                        } else {
                                            if (inAgeGroup.ToUpper().Equals( "CM" )
                                                || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                                inputCols[idxAgeGroup] = "BM";
                                            } else {
                                                inputCols[idxAgeGroup] = "BW";
                                            }
                                        }
                                    } else {
                                        //MessageBox.Show( "Not allowed to assign a skier to both the A and B team on a single row"
                                        //    + "\nCreate one row to assign a skier to the A team in one event and another row to assign the skier to the B team in another event"
                                        //    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                        if (inputColsSaved == null) {
                                            curDataValid = false;
                                        } else {
                                            curDataValid = true;
                                            //Save current row of columns
                                            for (int curIdx = 0; curIdx < inputCols.Length; curIdx++) {
                                                inputColsSaved[curIdx] = inputCols[curIdx];
                                            }
                                            //Remove B team assignments for current row and only process A team assignments
                                            inputCols[idxAgeGroup] = inAgeGroup;
                                            if (curTeamSlalom.Equals( "B" )) {
                                                inputCols[idxEventSlalom] = "";
                                            }
                                            if (curTeamTrick.Equals( "B" )) {
                                                inputCols[idxEventTrick] = "";
                                            }
                                            if (curTeamJump.Equals( "B" )) {
                                                inputCols[idxEventJump] = "";
                                            }
                                            //Updated saved columns to indicate just the B team assignments
                                            if (inAgeGroup.ToUpper().Equals( "CM" )
                                                || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                                inputColsSaved[idxAgeGroup] = "BM";
                                            } else {
                                                inputColsSaved[idxAgeGroup] = "BW";
                                            }
                                            if (curTeamSlalom.Equals( "A" )) {
                                                inputColsSaved[idxEventSlalom] = "";
                                            }
                                            if (curTeamTrick.Equals( "A" )) {
                                                inputColsSaved[idxEventTrick] = "";
                                            }
                                            if (curTeamJump.Equals( "A" )) {
                                                inputColsSaved[idxEventJump] = "";
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            } else {
                                if (curTeamSlalom.Equals( curTeamTrick )) {
                                    if (curTeamJump.Equals( "" )) {
                                        #region Skier registered for slalom and trick
                                        if (curTeamSlalom.Equals( "A" )) {
                                            inputCols[idxAgeGroup] = inAgeGroup;
                                        } else {
                                            if (inAgeGroup.ToUpper().Equals( "CM" )
                                                || inAgeGroup.ToUpper().Equals( "BM" )
                                            ) {
                                                inputCols[idxAgeGroup] = "BM";
                                            } else {
                                                inputCols[idxAgeGroup] = "BW";
                                            }
                                        }
                                        #endregion
                                    } else {
                                        if (curTeamSlalom.Equals( curTeamJump )) {
                                            #region Skier registered for slalom, trick and jump
                                            if (curTeamSlalom.Equals( "A" )) {
                                                inputCols[idxAgeGroup] = inAgeGroup;
                                            } else {
                                                if (inAgeGroup.ToUpper().Equals( "CM" )
                                                    || inAgeGroup.ToUpper().Equals( "BM" )
                                                ) {
                                                    inputCols[idxAgeGroup] = "BM";
                                                } else {
                                                    inputCols[idxAgeGroup] = "BW";
                                                }
                                            }
                                            #endregion
                                        } else {
                                            #region Checking skiers that are assigned to both A and B team
                                            if (inputColsSaved == null) {
                                                curDataValid = false;
                                            } else {
                                                curDataValid = true;
                                                //Save current row of columns
                                                for (int curIdx = 0; curIdx < inputCols.Length; curIdx++) {
                                                    inputColsSaved[curIdx] = inputCols[curIdx];
                                                }
                                                //Remove B team assignments for current row and only process A team assignments
                                                inputCols[idxAgeGroup] = inAgeGroup;
                                                if (curTeamSlalom.Equals( "B" )) {
                                                    inputCols[idxEventSlalom] = "";
                                                }
                                                if (curTeamTrick.Equals( "B" )) {
                                                    inputCols[idxEventTrick] = "";
                                                }
                                                if (curTeamJump.Equals( "B" )) {
                                                    inputCols[idxEventJump] = "";
                                                }
                                                //Updated saved columns to indicate just the B team assignments
                                                if (inAgeGroup.ToUpper().Equals( "CM" )
                                                    || inAgeGroup.ToUpper().Equals( "BM" )
                                                ) {
                                                    inputColsSaved[idxAgeGroup] = "BM";
                                                } else {
                                                    inputColsSaved[idxAgeGroup] = "BW";
                                                }
                                                if (curTeamSlalom.Equals( "A" )) {
                                                    inputColsSaved[idxEventSlalom] = "";
                                                }
                                                if (curTeamTrick.Equals( "A" )) {
                                                    inputColsSaved[idxEventTrick] = "";
                                                }
                                                if (curTeamJump.Equals( "A" )) {
                                                    inputColsSaved[idxEventJump] = "";
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                } else {
                                    #region Checking skiers that are assigned to both A and B team
                                    if (inputColsSaved == null) {
                                        curDataValid = false;
                                    } else {
                                        curDataValid = true;
                                        //Save current row of columns
                                        for (int curIdx = 0; curIdx < inputCols.Length; curIdx++) {
                                            inputColsSaved[curIdx] = inputCols[curIdx];
                                        }
                                        //Remove B team assignments for current row and only process A team assignments
                                        inputCols[idxAgeGroup] = inAgeGroup;
                                        if (curTeamSlalom.Equals( "B" )) {
                                            inputCols[idxEventSlalom] = "";
                                        }
                                        if (curTeamTrick.Equals( "B" )) {
                                            inputCols[idxEventTrick] = "";
                                        }
                                        if (curTeamJump.Equals( "B" )) {
                                            inputCols[idxEventJump] = "";
                                        }
                                        //Updated saved columns to indicate just the B team assignments
                                        if (inAgeGroup.ToUpper().Equals( "CM" )
                                            || inAgeGroup.ToUpper().Equals( "BM" )
                                        ) {
                                            inputColsSaved[idxAgeGroup] = "BM";
                                        } else {
                                            inputColsSaved[idxAgeGroup] = "BW";
                                        }
                                        if (curTeamSlalom.Equals( "A" )) {
                                            inputColsSaved[idxEventSlalom] = "";
                                        }
                                        if (curTeamTrick.Equals( "A" )) {
                                            inputColsSaved[idxEventTrick] = "";
                                        }
                                        if (curTeamJump.Equals( "A" )) {
                                            inputColsSaved[idxEventJump] = "";
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
            } else {
                //Check for rotations for non collegiate divisions because this is used for alumni events
                #region Check slalom event group to validate for appropriate alumni events values
                if (inEventGroupSlalom.ToLower().Equals( "of" )) {
                    //Skier is an official
                } else {
                    if (inEventGroupSlalom.Length == 1) {
                        MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                            + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                        curDataValid = false;
                    } else if (inEventGroupSlalom.Length > 1) {
                        if (inEventGroupSlalom.Substring( 0, 1 ).ToUpper().Equals( "A" )) {
                            curTeamSlalom = inEventGroupSlalom.Substring( 0, 1 ).ToUpper();
                            if (Int16.TryParse( inEventGroupSlalom.Substring( 1 ), out numIntCk )) {
                            } else {
                                MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }

                        } else {
                            MessageBox.Show( "Slalom event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        }
                    }
                }
                #endregion

                #region Check trick event group to validate for appropriate alumni events values
                if (inEventGroupTrick.ToLower().Equals( "of" )) {
                    //Skier is an official
                } else {
                    if (inEventGroupTrick.Length == 1) {
                        MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                            + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                        curDataValid = false;
                    } else if (inEventGroupTrick.Length > 1) {
                        if (inEventGroupTrick.Substring( 0, 1 ).ToUpper().Equals( "A" )) {
                            curTeamTrick = inEventGroupTrick.Substring( 0, 1 ).ToUpper();
                            if (Int16.TryParse( inEventGroupTrick.Substring( 1 ), out numIntCk )) {
                            } else {
                                MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }

                        } else {
                            MessageBox.Show( "Trick event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        }
                    }
                }
                #endregion

                #region Check jump event group to validate for appropriate alumni events values
                if (inEventGroupJump.ToLower().Equals( "of" )) {
                    //Skier is an official
                } else {
                    if (inEventGroupJump.Length == 1) {
                        MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                            + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                        curDataValid = false;
                    } else if (inEventGroupJump.Length > 1) {
                        if (inEventGroupJump.Substring( 0, 1 ).ToUpper().Equals( "A" )) {
                            curTeamJump = inEventGroupJump.Substring( 0, 1 ).ToUpper();
                            if (Int16.TryParse( inEventGroupJump.Substring( 1 ), out numIntCk )) {
                            } else {
                                MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                    + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                                curDataValid = false;
                            }

                        } else {
                            MessageBox.Show( "Jump event group is not valid.\nIt must start with the letter A followed by a numeric rotation number"
                                + "\n" + inMemberId + " " + inFirstName + " " + inLastName + " " + inAgeGroup );
                            curDataValid = false;
                        }
                    }
                }
                #endregion

                if (curDataValid) {
                    if (curTeamSlalom.Equals( "" )) {
                        if (curTeamTrick.Equals( "" )) {
                            if (curTeamJump.Equals( "" )) {
                                inputCols[idxAgeGroup] = inAgeGroup;
                            } else {
                                inputCols[idxAgeGroup] = inAgeGroup;
                                curTeamJump = inAgeGroup.Substring( 0, 1 );
                            }
                        } else {
                            curTeamTrick = inAgeGroup.Substring( 0, 1 );
                            if (curTeamJump.Equals( "" )) {
                                inputCols[idxAgeGroup] = inAgeGroup;
                            } else {
                                inputCols[idxAgeGroup] = inAgeGroup;
                                curTeamJump = inAgeGroup.Substring( 0, 1 );
                            }
                        }
                    } else {
                        curTeamSlalom = inAgeGroup.Substring( 0, 1 );
                        if (curTeamTrick.Equals( "" )) {
                            if (curTeamJump.Equals( "" )) {
                                inputCols[idxAgeGroup] = inAgeGroup;
                            } else {
                                inputCols[idxAgeGroup] = inAgeGroup;
                                curTeamJump = inAgeGroup.Substring( 0, 1 );
                            }
                        } else {
                            curTeamTrick = inAgeGroup.Substring( 0, 1 );
                            if (curTeamJump.Equals( "" )) {
                                inputCols[idxAgeGroup] = inAgeGroup;
                            } else {
                                inputCols[idxAgeGroup] = inAgeGroup;
                                curTeamJump = inAgeGroup.Substring( 0, 1 );
                            }
                        }
                    }

                    if (curTeamSlalom.Length == 1) {
                        inputCols[idxEventSlalom] = curTeamSlalom + inputCols[idxEventSlalom].Substring(1, inputCols[idxEventSlalom].Length - 1);
                    }
                    if (curTeamTrick.Length == 1) {
                        inputCols[idxEventTrick] = curTeamTrick + inputCols[idxEventTrick].Substring(1, inputCols[idxEventTrick].Length - 1);
                    }
                    if (curTeamJump.Length == 1) {
                        inputCols[idxEventJump] = curTeamJump + inputCols[idxEventJump].Substring( 1, inputCols[idxEventJump].Length - 1 );
                    }
                
                }
            }

            return curDataValid;
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
        
        private String getOfficalRatingValue (DataRow inRow, String inAttrName, String inAttrNameTour ) {
            String curMemberRating = "", curMemberRatingTour = "";
            try {
                curMemberRating = ((String)inRow[inAttrName]) ;
            } catch {
                curMemberRating = "";
            }
            try {
                curMemberRatingTour = ((String)inRow[inAttrNameTour]);
            } catch {
                curMemberRatingTour = "";
            }

            if ( curMemberRating.Equals( curMemberRatingTour ) ) {
                return null;
            } else {
                if ( curMemberRating.Length > 0 ) {
                    return "'" + curMemberRating + "'";
                } else {
                    return "null";
                }
            }
        }
    }
}
