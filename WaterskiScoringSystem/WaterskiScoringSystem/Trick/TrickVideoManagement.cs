using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Trick {
	public partial class TrickVideoManagement : Form {
		private String mySanctionNum = null;
		private String myVideoLoadUrl = Properties.Settings.Default.UriVideoLoadApi;
		private String myApiKey = Properties.Settings.Default.UriVideoLoadApiKey;
		private String myApiKeyName = Properties.Settings.Default.UriVideoLoadApiKeyname;
		private String myContentType = "application/x-www-form-urlencoded;charset=UTF-8";

		public TrickVideoManagement() {
			InitializeComponent();
		}
		private void TrickVideoManagement_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.VideoManagement_Width > 0 ) this.Width = Properties.Settings.Default.VideoManagement_Width;
			if ( Properties.Settings.Default.VideoManagement_Height > 0 ) this.Height = Properties.Settings.Default.VideoManagement_Height;
			if ( Properties.Settings.Default.VideoManagement_Location.X > 0
				&& Properties.Settings.Default.VideoManagement_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.VideoManagement_Location;
			}
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			SanctionTextbox.Text = mySanctionNum;
		}

		private void LoadVideosFile_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.VideoManagement_Width = this.Size.Width;
				Properties.Settings.Default.VideoManagement_Height = this.Size.Height;
				Properties.Settings.Default.VideoManagement_Location = this.Location;
			}
		}

		private void ViewButton_Click( object sender, EventArgs e ) {
			String curMethodName = "ViewButton_Click";
			String curQueryOrderBy = "?order_by=updated_at";
			String curQueryTagFilter = "&tag_name=";
			String curReqstUrl = myVideoLoadUrl + curQueryOrderBy + curQueryTagFilter + SanctionTextbox.Text;
			Dictionary<string, object> curResponseDataList = null;
			NameValueCollection curHeaderParams = null;
			Cursor.Current = Cursors.WaitCursor;

			curHeaderParams = new NameValueCollection();
			curHeaderParams.Add( myApiKeyName, myApiKey );

			try {
				loadedVideoDataGridView.Rows.Clear();
				DataGridViewRow curViewRow;
				int curViewIdx = 0;
				String curFileSize = "";

				while ( curReqstUrl.Length > 0 ) {
					curResponseDataList = SendMessageHttp.getMessageResponseJson( curReqstUrl, curHeaderParams, myContentType );
					if ( curResponseDataList == null || curResponseDataList.Count == 0 ) break;
					if ( !( curResponseDataList.ContainsKey( "videos" ) ) ) break;

					ArrayList curList = (ArrayList)curResponseDataList["videos"];
					foreach ( Dictionary<string, object> curVideoEntry in curList ) {
						curViewIdx = loadedVideoDataGridView.Rows.Add();
						curViewRow = loadedVideoDataGridView.Rows[curViewIdx];
						foreach ( KeyValuePair<String, object> curVideoAttr in curVideoEntry ) {
							if ( curVideoAttr.Key.Equals( "id" ) ) curViewRow.Cells["VideoId"].Value = (String)curVideoAttr.Value;
							if ( curVideoAttr.Key.Equals( "title" ) ) curViewRow.Cells["VideoTitle"].Value = (String)curVideoAttr.Value;
							if ( curVideoAttr.Key.Equals( "state" ) ) curViewRow.Cells["VideoState"].Value = (String)curVideoAttr.Value;
							if ( curVideoAttr.Key.Equals( "plays" ) ) curViewRow.Cells["VideoPlays"].Value = (int)curVideoAttr.Value;
							if ( curVideoAttr.Key.Equals( "created_at" ) ) curViewRow.Cells["CreatedDate"].Value = (String)curVideoAttr.Value;
							if ( curVideoAttr.Key.Equals( "hd_video_file_size" ) ) {
								curFileSize = ( (int)curVideoAttr.Value ).ToString();
								if ( curFileSize.Length > 6 ) {
									try {
										curViewRow.Cells["VideoSizeHD"].Value = ( double.Parse( curFileSize ) / 1048576 ).ToString( "N2" ) + "M";
									} catch {
										curViewRow.Cells["VideoSizeHD"].Value = "";
									}
								} else {
									try {
										curViewRow.Cells["VideoSizeHD"].Value = ( double.Parse( curFileSize ) / 1024 ).ToString( "N2" ) + "K";
									} catch {
										curViewRow.Cells["VideoSizeHD"].Value = "";
									}
								}
							}

							if ( curVideoAttr.Key.Equals( "embed_code" ) ) {
								curViewRow.Cells["VideoURL"].Value = (String)curVideoAttr.Value;
							}
							if ( curVideoAttr.Key.Equals( "sd_video_file_size" ) ) {
								curFileSize = ( (int)curVideoAttr.Value ).ToString();
								if ( curFileSize.Length > 6 ) {
									try {
										curViewRow.Cells["VideoSizeSD"].Value = ( double.Parse( curFileSize ) / 1048576 ).ToString( "N2" ) + "M";
									} catch {
										curViewRow.Cells["VideoSizeSD"].Value = "";
									}
								} else {
									try {
										curViewRow.Cells["VideoSizeSD"].Value = ( double.Parse( curFileSize ) / 1024 ).ToString( "N2" ) + "K";
									} catch {
										curViewRow.Cells["VideoSizeSD"].Value = "";
									}
								}
							}
						}

						curViewRow.Cells["SortColumn"].Value = curViewRow.Cells["VideoTitle"].Value + "--" + curViewRow.Cells["CreatedDate"].Value;
                    }

					if ( curResponseDataList.ContainsKey( "next_page" ) ) {
						curReqstUrl = (String)curResponseDataList["next_page"];
					} else {
						break;
					}
				}

				try {
					if ( loadedVideoDataGridView.Rows.Count > 0 ) {
						loadedVideoDataGridView.CurrentCell = loadedVideoDataGridView.Rows[0].Cells["VideoTitle"];
						int curRowPos = 1;
						RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + loadedVideoDataGridView.Rows.Count.ToString();
					} else {
						RowStatusLabel.Text = "No videos found";
					}
				} catch {
					RowStatusLabel.Text = "No videos found";
				}

			} catch ( Exception ex ) {
				MessageBox.Show( "Error encountered trying to send data to web location \n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private void ExportLoadedButton_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			myExportData.exportData( loadedVideoDataGridView );
		}

		private void DeleteButton_Click( object sender, EventArgs e ) {
			if ( loadedVideoDataGridView.Rows.Count == 0 ) return;
            String curMatchCommand = "", curMatchCommandPrev;
            ImportMatchDialogForm curMatchDialog = new ImportMatchDialogForm();

            try {
				Cursor.Current = Cursors.WaitCursor;
				String curSelectValue, curVideoId, curVideoUrl;
				foreach ( DataGridViewRow curViewRow in loadedVideoDataGridView.Rows ) {
					curSelectValue = HelperFunctions.getViewRowColValue( curViewRow, "SelectVideo", "False" );
					if ( HelperFunctions.isValueTrue( curSelectValue ) ) {
						curVideoId = HelperFunctions.getViewRowColValue( curViewRow, "VideoId", "" );
						if ( HelperFunctions.isObjectPopulated( curVideoId ) ) {
							curVideoUrl = extractVideoUrl( curViewRow );
							if ( HelperFunctions.isObjectEmpty( curVideoUrl ) ) continue;
                            StringBuilder curSqlStmt = new StringBuilder( "Select count(*) From TrickVideo " );
                            curSqlStmt.Append( String.Format("Where SanctionId = '{0}' AND (Pass1VideoUrl like '%{1}%' OR Pass2VideoUrl like '%{1}%')"
								, SanctionTextbox.Text, curVideoUrl ) );
                            int curReadCount = (Int32)DataAccess.ExecuteScalarCommand( curSqlStmt.ToString() );
							if ( curReadCount > 0 ) continue;

							if ( HelperFunctions.isObjectEmpty( curMatchCommand ) ) {
                                String[] curMessage = new String[6];
                                curMessage[0] = "Trick Video doesn't appear to be in use";
                                curMessage[1] = "Do you want to delete it from the video server?";
                                curMessage[2] = "";
                                curMessage[3] = curVideoUrl;
                                curMessage[4] = "";
                                curMatchDialog.ImportKeyDataMultiLine = curMessage;

                                curMatchDialog.MatchCommand = curMatchCommand;
                                if ( curMatchDialog.ShowDialog() == DialogResult.OK ) {
                                    curMatchCommand = curMatchDialog.MatchCommand;
                                    curMatchCommandPrev = curMatchCommand;
                                }
                            }

                            if ( curMatchCommand.ToLower().Equals( "update" )
								|| curMatchCommand.ToLower().Equals( "updateall" ) ) {
                                if ( deleteVideo( curVideoId ) ) {
                                    curViewRow.Cells["VideoState"].Value = "Deleted";
                                    curViewRow.Cells["SelectVideo"].Value = "False";
                                }
								if ( curMatchCommand.ToLower().Equals( "update" )) curMatchCommand = "";
                            
							} else {
                                //Re-initialize dialog response unless specified to process rows
                                if ( curMatchCommand.ToLower().Equals( "skip" ) ) curMatchCommand = "";
                            }
                        }
                    }
                }

            } finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private String extractVideoUrl( DataGridViewRow curViewRow ) {
            //curVideoUrl
            String curVideoRef = HelperFunctions.getViewRowColValue( curViewRow, "VideoURL", "" );
			if ( HelperFunctions.isObjectEmpty( curVideoRef ) ) return "";

			int curDelimStart = curVideoRef.IndexOf( "src=" );
			if ( curDelimStart < 0 ) return "";
            int curDelimEnd = curVideoRef.IndexOf( "' ", curDelimStart + 5 );
            if ( curDelimEnd < 0 ) return "";
			return curVideoRef.Substring( curDelimStart + 5, curDelimEnd - curDelimStart - 5);
        }

        private void MatchButton_Click( object sender, EventArgs e ) {
			if ( loadedVideoDataGridView.Rows.Count == 0 ) return;

			try {
				Cursor.Current = Cursors.WaitCursor;
				String curSelectValue, curVideoTitle, curVideoUrl;
				SkierVideoEntry curSkierVideoEntry;

				foreach ( DataGridViewRow curViewRow in loadedVideoDataGridView.Rows ) {
					curSelectValue = HelperFunctions.getViewRowColValue( curViewRow, "SelectVideo", "False" );
					if ( HelperFunctions.isValueTrue( curSelectValue ) ) {
						curVideoTitle = HelperFunctions.getViewRowColValue( curViewRow, "VideoTitle", "" );
						if ( HelperFunctions.isObjectPopulated( curVideoTitle ) ) {
							String[] curAttrList = curVideoTitle.Split( ' ' );

							if ( curAttrList.Length == 7 ) {
								curSkierVideoEntry = new SkierVideoEntry( "", "", curAttrList[1] + ", " + curAttrList[0], curAttrList[2], Int16.Parse( curAttrList[4] ), Int16.Parse( curAttrList[6] ), 0 );
								if ( searchForSkierMatchVideoTitle( curSkierVideoEntry ) ) {
									curVideoUrl = HelperFunctions.getViewRowColValue( curViewRow, "VideoURL", "" );
									if ( updateSkierScoreVideoUrl( curSkierVideoEntry, curVideoUrl ) ) {
										curViewRow.Cells["VideoState"].Value = "Matched";
										curViewRow.Cells["SelectVideo"].Value = "False";
									} else {
										MessageBox.Show( "Match Not Found: " + curVideoTitle );
									}
								}
							}
						}
					}
				}
			
			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private void FindDupsButton_Click( object sender, EventArgs e ) {
			if ( loadedVideoDataGridView.Rows.Count == 0 ) return;

			try {
				Cursor.Current = Cursors.WaitCursor;
				String curVideoTitle, prevVideoTitle = "";
				DataGridViewRow prevViewRow = null;

                int curViewIdx;

				loadedVideoDataGridView.Sort( loadedVideoDataGridView.Columns["SortColumn"], System.ComponentModel.ListSortDirection.Ascending );
				foreach ( DataGridViewRow curViewRow in loadedVideoDataGridView.Rows ) {
					curViewIdx = curViewRow.Index;
					curVideoTitle = HelperFunctions.getViewRowColValue( curViewRow, "VideoTitle", "" );

					if ( curViewIdx == 0 ) {
						prevVideoTitle = curVideoTitle;
                        prevViewRow = curViewRow;
                        continue;
					}

					if ( prevVideoTitle == curVideoTitle ) {
                        prevViewRow.Cells["VideoState"].Value = "Duplicate";
                        prevViewRow.Cells["SelectVideo"].Value = "True";
					}
                    prevVideoTitle = curVideoTitle;
                    prevViewRow = curViewRow;
                }

            } finally {
				Cursor.Current = Cursors.Default;
			}
		}

		private bool deleteVideo( String inVideoId ) {
			String curMethodName = "deleteVideo";
			Dictionary<string, object> curResponseDeleteDataList = null;
			NameValueCollection curHeaderParams = null;
			String curReqstUrl = myVideoLoadUrl + "/" + inVideoId;

			curHeaderParams = new NameValueCollection();
			curHeaderParams.Add( myApiKeyName, myApiKey );

			try {
				curResponseDeleteDataList = SendMessageHttp.deleteMessagePostJsonResp( curReqstUrl, curHeaderParams, "application/json; charset=UTF-8", null );
				if ( curResponseDeleteDataList == null ) return false;
				return inVideoId == HelperFunctions.getAttributeValue( curResponseDeleteDataList, "id" );

			} catch ( Exception ex ) {
				MessageBox.Show( "Error encountered attemptign to delete video \n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );
			}
			return false;
		}

		private bool searchForSkierMatchVideoTitle( SkierVideoEntry inSkierVideoEntry ) {
			String curSqlStmtOrder = "Order by T.SkierName, T.AgeGroup ";
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct S.PK, T.MemberId, T.SkierName, T.AgeGroup, T.AgeGroup as Div " );
			curSqlStmt.Append( "FROM TourReg T " );
			curSqlStmt.Append( "INNER JOIN EventReg R ON T.SanctionId = R.SanctionId AND T.MemberId = R.MemberId AND T.AgeGroup = R.AgeGroup " );
			curSqlStmt.Append( "INNER JOIN TrickScore S ON T.SanctionId = S.SanctionId AND T.MemberId = S.MemberId AND T.AgeGroup = S.AgeGroup " );
			curSqlStmt.Append( "WHERE T.SanctionId = '" + SanctionTextbox.Text + "' AND R.Event = 'Trick' " );
			String curSqlStmtSelect = curSqlStmt.ToString();

			/*
			 * Search for skier by name and division
			 */
			curSqlStmt = new StringBuilder( String.Format( "{0} AND T.SkierName = '{1}' AND T.AgeGroup = '{2}' AND S.Round = {3} {4}"
				, curSqlStmtSelect, inSkierVideoEntry.LastName + ", " + inSkierVideoEntry.FirstName, inSkierVideoEntry.AgeGroup, inSkierVideoEntry.Round, curSqlStmtOrder ) );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) {
				inSkierVideoEntry.MemberId = HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "MemberId", "" );
				inSkierVideoEntry.SkierName = HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "SkierName", "" );
				inSkierVideoEntry.SkierScorePK = long.Parse( HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "PK", "0" ) );
				return true;
			}

			curSqlStmt = new StringBuilder( String.Format( "{0} AND T.SkierName Like '{1}%' AND T.AgeGroup = '{2}' AND S.Round = {3} {4}"
				, curSqlStmtSelect, inSkierVideoEntry.LastName, inSkierVideoEntry.AgeGroup, inSkierVideoEntry.Round, curSqlStmtOrder ) );
			curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count == 1 ) {
				inSkierVideoEntry.MemberId = HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "MemberId", "" );
				inSkierVideoEntry.SkierName = HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "SkierName", "" );
				inSkierVideoEntry.SkierScorePK = long.Parse( HelperFunctions.getDataRowColValue( curDataTable.Rows[0], "PK", "0" ) );
				return true;
			}
			return false;
		}

		private bool updateSkierScoreVideoUrl( SkierVideoEntry inSkierVideoEntry, String inVideoUrl ) {
			String curMethodName = "updateSkierScoreVideoUrl";
			int rowsProc = 0;

			if ( inSkierVideoEntry.SkierScorePK == 0 || inSkierVideoEntry.Pass <= 0 ) return false;

			try {
				String curVideoUrl = inVideoUrl.Replace( "630", "410" );
				curVideoUrl = curVideoUrl.Replace( "420", "273" );

				StringBuilder curSqlStmt = new StringBuilder( "Select count(*) From TrickVideo " );
				curSqlStmt.Append( "Where SanctionId = '" + SanctionTextbox.Text + "' " );
				curSqlStmt.Append( "AND MemberId = '" + inSkierVideoEntry.MemberId + "' " );
				curSqlStmt.Append( "AND AgeGroup = '" + inSkierVideoEntry.AgeGroup + "' " );
				curSqlStmt.Append( "AND Round = '" + inSkierVideoEntry.Round + "' " );
				int curReadCount = (Int32)DataAccess.ExecuteScalarCommand( curSqlStmt.ToString() );
				if ( curReadCount == 0 ) {
					//If entry does not currently exist then add it
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Insert into TrickVideo (" );
					curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, Pass1VideoUrl, Pass2VideoUrl, LastUpdateDate" );
					curSqlStmt.Append( ") Values (" );
					curSqlStmt.Append( "'" + SanctionTextbox.Text + "' " );
					curSqlStmt.Append( ", '" + inSkierVideoEntry.MemberId + "' " );
					curSqlStmt.Append( ",'" + inSkierVideoEntry.AgeGroup + "' " );
					curSqlStmt.Append( ", " + inSkierVideoEntry.Round + ", '', '', GetDate() ) " );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				}

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TrickVideo set " );
				if ( inSkierVideoEntry.Pass == 1 ) {
					curSqlStmt.Append( "Pass1VideoUrl = '" + HelperFunctions.escapeString( curVideoUrl ) + "' " );
				} else {
					curSqlStmt.Append( "Pass2VideoUrl = '" + HelperFunctions.escapeString( curVideoUrl ) + "' " );
				}
				curSqlStmt.Append( ", LastUpdateDate = GetDate() " );
				curSqlStmt.Append( "Where SanctionId = '" + SanctionTextbox.Text + "' " );
				curSqlStmt.Append( "AND MemberId = '" + inSkierVideoEntry.MemberId + "' " );
				curSqlStmt.Append( "AND AgeGroup = '" + inSkierVideoEntry.AgeGroup + "' " );
				curSqlStmt.Append( "AND Round = '" + inSkierVideoEntry.Round + "' " );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) {
					LiveWebHandler.sendCurrentSkier( "TrickVideo", SanctionTextbox.Text, inSkierVideoEntry.MemberId, inSkierVideoEntry.AgeGroup, Convert.ToByte( inSkierVideoEntry.Round ), 0 );
					return true;
				}

			} catch ( Exception ex ) {
				MessageBox.Show( curMethodName + ":Error encountered\n\nError: " + ex.Message );
				String curMsg = curMethodName + ":Exception=" + ex.Message;
				Log.WriteFile( curMsg );
			}

			return false;
		}

		private void DataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			RowStatusLabel.Text = "Row " + ( e.RowIndex + 1 ) + " of " + ( (DataGridView)sender ).Rows.Count;
		}

        private void SelectAllButton_Click( object sender, EventArgs e ) {
            foreach (DataGridViewRow curViewRow in loadedVideoDataGridView.Rows) {
                curViewRow.Cells["SelectVideo"].Value = "True";
            }
        }
    }
}
