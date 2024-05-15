using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Trick {
    public partial class LoadVideosFile : Form {
        private String mySanctionNum = null;
        private Int16 myTrickRounds;

		private String myVideoLoadUrl = Properties.Settings.Default.UriVideoLoadApi;
		private String myApiKey = Properties.Settings.Default.UriVideoLoadApiKey;
		private String myApiKeyName = Properties.Settings.Default.UriVideoLoadApiKeyname;
		private String myContentType = "application/x-www-form-urlencoded;charset=UTF-8";
		//private String myVideoLoadUrl = "https://api.sproutvideo.com/v1/videos";
		//private String myVideoListUrl = "https://api.sproutvideo.com/v1/videos";
		//private String myVideoDeleteUrl = "https://api.sproutvideo.com/v1/videos/";
		//private String myApiKey = "36924f60689e082b2626ae7da73d0404";
		//private String myApiKeyName = "SproutVideo-Api-Key";

		private char[] myCharDelimLimit = new char[] { ' ', '_', '-', ',' };

		private List<String> mySelectedFileList = new List<String>();
        private List<SkierVideoEntry> mySkierVideoList = null;
        private OpenFileDialog myFileDialog = null;
        private SkierVideoMatchDialogForm mySkierSelectDialog = null;
        private DataRow myTourRow;
        private ProgressWindow myProgressInfo;
        private DataTable myFullSkierDataTable = null;

        public LoadVideosFile() {
            InitializeComponent();

        }

        private void LoadVideosFile_Load(object sender, EventArgs e) {
            VideoFolderLocTextbox.Text = Properties.Settings.Default.ExportDirectory;
            loadedVideoDataGridView.Visible = false;
            selectedFileDataGridView.Visible = false;
            ReviewVideoMatchDataGridView.Visible = false;

            ExportLoadedButton.Visible = false;
            ExportLoadedButton.Enabled = false;

			selectedFileDataGridView.Location = loadedVideoDataGridView.Location;
            selectedFileDataGridView.Width = this.Width - 25;
            //selectedFileDataGridView.Height = this.Height - loadedVideoDataGridView.Location.X - 25;

            ReviewVideoMatchDataGridView.Location = loadedVideoDataGridView.Location;
            ReviewVideoMatchDataGridView.Width = this.Width - 25;
            //ReviewVideoMatchDataGridView.Height = this.Height - loadedVideoDataGridView.Location.X - 25;

            if ( Properties.Settings.Default.LoadVideosFile_Width > 0 ) {
                this.Width = Properties.Settings.Default.LoadVideosFile_Width;
            }
            if ( Properties.Settings.Default.LoadVideosFile_Height > 0 ) {
                this.Height = Properties.Settings.Default.LoadVideosFile_Height;
            }
            if ( Properties.Settings.Default.LoadVideosFile_Location.X > 0
                && Properties.Settings.Default.LoadVideosFile_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.LoadVideosFile_Location;
            }

            // Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null || mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration > Tournament List" );
				closeWindow();
				return;
			}

            //Retrieve selected tournament attributes
            DataTable curTourDataTable = getTourData();
            if (curTourDataTable.Rows.Count > 0) {
                myTourRow = curTourDataTable.Rows[0];
                myTrickRounds = Convert.ToInt16( (byte)myTourRow["TrickRounds"] );
                getTagProperty();
                    }
        }
		
        private void closeWindow() {
			Timer curTimerObj = new Timer();
			curTimerObj.Interval = 15;
			curTimerObj.Tick += new EventHandler( CloseWindowTimer );
			curTimerObj.Start();
			return;
		}
		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		private void LoadVideosFile_FormClosed(object sender, FormClosedEventArgs e) {
            StringBuilder curPropValue = new StringBuilder( "" );
            foreach (String curEntry in TagsListBox.Items) {
                if (curPropValue.Length > 0) {
                    curPropValue.Append( "," + curEntry );
                } else {
                    curPropValue.Append( curEntry );
                }
            }
            updateProperty( "VideoTags", curPropValue.ToString(), 999 );

            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.LoadVideosFile_Width = this.Size.Width;
                Properties.Settings.Default.LoadVideosFile_Height = this.Size.Height;
                Properties.Settings.Default.LoadVideosFile_Location = this.Location;
            }

        }

        private void LiveWebButton_Click(object sender, EventArgs e) {
			if ( !( LiveWebHandler.LiveWebMessageHandlerActive ) ) LiveWebHandler.connectLiveWebHandler( mySanctionNum );
			if ( !( LiveWebHandler.LiveWebMessageHandlerActive ) ) {
				MessageBox.Show( "Request to publish running order but live web not successfully connected." );
				return;
			}

			LiveWebHandler.sendSkiers( "TrickVideo", mySanctionNum, 0, "All" );
		}

		private void ExportButton_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[1];
            String[] curTableName = { "TrickVideo" };

            curSelectCommand[0] = "SELECT * FROM TrickVideo "
                + "Where SanctionId = '" + mySanctionNum + "' AND (LEN(Pass1VideoUrl) > 1 or LEN(Pass2VideoUrl) > 1)";

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void VideoFileSelectButton_Click(object sender, EventArgs e) {
            String curMethodName = "VideoFileSelectButton_Click";
            try {
                myFileDialog = new OpenFileDialog();
                myFileDialog.Multiselect = true;
                myFileDialog.InitialDirectory = VideoFolderLocTextbox.Text;
                myFileDialog.Filter = "Video files|*.avi;*.mkv;*.3g2;*.3gp;*.asf;*.asx;*.flv;*.mov;*.mp4;*.mpg;*.rm;*.swf;*.vob;*.wma;*.wmv;*.m2ts|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 9;
                if (myFileDialog.ShowDialog() == DialogResult.OK) {
                    mySelectedFileList = new List<string>( myFileDialog.FileNames );
                    if (mySelectedFileList.Count > 0) {
                        mySkierSelectDialog = new SkierVideoMatchDialogForm();
                        mySkierSelectDialog.TourRounds = myTrickRounds;
                        mySkierVideoList = new List<SkierVideoEntry>();
                        DataGridViewRow curViewRow;
                        int curViewIdx = 0;

                        loadedVideoDataGridView.Visible = false;
                        ReviewVideoMatchDataGridView.Visible = false;
                        selectedFileDataGridView.Visible = true;
                        selectedFileDataGridView.Rows.Clear();

                        //Get all avaialble trick skiers if filtered search found zero skiers
                        StringBuilder curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("SELECT Distinct T.MemberId, T.SkierName, T.AgeGroup, T.AgeGroup as Div ");
                        curSqlStmt.Append("FROM TourReg T ");
                        curSqlStmt.Append("     INNER JOIN EventReg R ON T.SanctionId = R.SanctionId AND T.MemberId = R.MemberId AND T.AgeGroup = R.AgeGroup ");
                        curSqlStmt.Append("INNER JOIN TrickScore S ON T.SanctionId = S.SanctionId AND T.MemberId = S.MemberId AND T.AgeGroup = S.AgeGroup ");
                        curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' AND R.Event = 'Trick' ");
                        curSqlStmt.Append("Order by T.SkierName, T.AgeGroup ");
                        myFullSkierDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
                        mySkierSelectDialog.FullSkierDataTable = myFullSkierDataTable;

                        foreach ( String curVideoFileName in mySelectedFileList) {
                            SkierVideoEntry curSkierVideoEntry = new SkierVideoEntry( curVideoFileName, "", "", "", 0, 0, 0 );
                            if (searchForMatchingSkier( curSkierVideoEntry )) {
								mySkierVideoList.Add(curSkierVideoEntry);
								curViewIdx = selectedFileDataGridView.Rows.Add();
								curViewRow = selectedFileDataGridView.Rows[curViewIdx];
								curViewRow.Cells["SelectedFileName"].Value = Path.GetFileName(curVideoFileName);
								curViewRow.Cells["SelectedSkierName"].Value = curSkierVideoEntry.SkierName;
								curViewRow.Cells["SelectedMemberId"].Value = curSkierVideoEntry.MemberId;
								curViewRow.Cells["SelectedAgeGroup"].Value = curSkierVideoEntry.AgeGroup;
								curViewRow.Cells["SelectedRound"].Value = curSkierVideoEntry.Round;
								curViewRow.Cells["SelectedPass"].Value = curSkierVideoEntry.Pass;
								curViewRow.Cells["SelectedLoadStatus"].Value = "";
							}
						}
                        try {
                            if (selectedFileDataGridView.Rows.Count > 0) {
                                selectedFileDataGridView.CurrentCell = selectedFileDataGridView.Rows[0].Cells["SelectedFileName"];
                                int curRowPos = 1;
                                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + selectedFileDataGridView.Rows.Count.ToString();
                            } else {
                                RowStatusLabel.Text = "";
                            }
                        } catch {
                            RowStatusLabel.Text = "";
                        }
                    }
                }
            } catch (Exception ex) {
                MessageBox.Show( curMethodName + ":Error encountered\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ReviewButton_Click( object sender, EventArgs e ) {
			ReviewVideoMatchDataGridView.DataSource = null;
			ReviewVideoMatchDataGridView.Visible = true;

            //Get all avaialble trick skiers if filtered search found zero skiers

            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("Select R.SkierName, S.MemberId, S.AgeGroup, S.Round, Score, V.Pass1VideoUrl, V.Pass2VideoUrl, '' as ResendStatus ");
            curSqlStmt.Append("from TrickScore S ");
            curSqlStmt.Append("JOIN TourReg R ON R.SanctionId = S.SanctionId AND R.MemberId = S.MemberId AND R.AgeGroup = S.AgeGroup ");
            curSqlStmt.Append("LEFT OUTER JOIN TrickVideo V ON V.SanctionId = S.SanctionId AND V.MemberId = S.MemberId ");
            curSqlStmt.Append("AND V.AgeGroup = S.AgeGroup AND V.Round = S.Round ");
            curSqlStmt.Append("where S.sanctionid = '" + mySanctionNum + "' ");
            curSqlStmt.Append("Order by S.AgeGroup, R.SkierName, S.Round ");
            DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());

            ReviewVideoMatchDataGridView.DataSource = curDataTable;

        }

		private void ResendButton_Click( object sender, EventArgs e ) {
			String curMethodName = "ResendButton_Click";
			String curMemberId, curAgeGroup,  curPass1VideoUrl, curPass2VideoUrl;
			byte curRound;

            if ( ReviewVideoMatchDataGridView.Visible == false || ReviewVideoMatchDataGridView.Rows.Count == 0 ) {
                MessageBox.Show( "Data grid not active, no action performed" );
                return;
            }
            if ( LiveWebHandler.LiveWebMessageHandlerActive == false ) {
				MessageBox.Show( "Live Web interface not active therefore video URLs not uploaded" );
				return;
			}

			myProgressInfo = new ProgressWindow();
            try {
				Cursor.Current = Cursors.WaitCursor;
				myProgressInfo.setProgessMsg( "Uploading " + ReviewVideoMatchDataGridView.Rows.Count + " trick matches" );
				myProgressInfo.setProgressMax( ReviewVideoMatchDataGridView.Rows.Count );
				myProgressInfo.Show();
				int curProcessCount = 0;
				foreach ( DataGridViewRow curViewRow in ReviewVideoMatchDataGridView.Rows ) {
					curProcessCount++;
					myProgressInfo.setProgressValue( curProcessCount );
					myProgressInfo.setProgessMsg( "Processing match for skier " + (String)curViewRow.Cells["SkierName"].Value );
					myProgressInfo.Refresh();
					myProgressInfo.Show();

					curPass1VideoUrl = HelperFunctions.getViewRowColValue( curViewRow, "Pass1VideoUrl", "" );
					curPass2VideoUrl = HelperFunctions.getViewRowColValue( curViewRow, "Pass2VideoUrl", "" );
					if ( HelperFunctions.isObjectPopulated( curPass1VideoUrl ) || HelperFunctions.isObjectPopulated( curPass2VideoUrl ) ) {
						curMemberId = HelperFunctions.getViewRowColValue( curViewRow, "MemberId", "" );
						curAgeGroup = HelperFunctions.getViewRowColValue( curViewRow, "AgeGroup", "" );
						curRound = Convert.ToByte( HelperFunctions.getViewRowColValue( curViewRow, "Round", "0" ) );
						LiveWebHandler.sendCurrentSkier( "TrickVideo", mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
						curViewRow.Cells["ResendStatus"].Value = "Video URL attached to Live Web skier";

					} else {
						curViewRow.Cells["ResendStatus"].Value = "No video URL attached";
					}
				}
				myProgressInfo.Close();

			} catch ( Exception ex ) {
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                MessageBox.Show( curMsg );
                myProgressInfo.Close();
			
            } finally {
				Cursor.Current = Cursors.Default;
			}
		}

        private void OkButton_Click(object sender, EventArgs e) {
            String curMethodName = "OkButton_Click";
            String curFileFormName = "source_video";
            DataGridViewRow curViewRow = null; ;
            NameValueCollection curFormData = null;
            NameValueCollection curHeaderParams = null;

			myProgressInfo = new ProgressWindow();
            try {
				Cursor.Current = Cursors.WaitCursor;
				if ( mySkierVideoList != null && mySkierVideoList.Count > 0 ) {
                    myProgressInfo.setProgessMsg( "Uploading " + mySkierVideoList.Count + " trick video files" );
                    myProgressInfo.setProgressMax( mySkierVideoList.Count );
                    myProgressInfo.Show();
                    int curProcessCount = 0, curViewIdx = 0;
                    foreach ( SkierVideoEntry curSkierVideoEntry in mySkierVideoList ) {
                        curProcessCount++;
                        myProgressInfo.setProgressValue( curProcessCount );
                        myProgressInfo.setProgessMsg( "Processing file " + Path.GetFileName( curSkierVideoEntry.VideoFileName ) );
                        myProgressInfo.Refresh();
                        myProgressInfo.Show();

                        curViewRow = selectedFileDataGridView.Rows[curViewIdx];

                        curFormData = new NameValueCollection();
                        curFormData.Add( "title", curSkierVideoEntry.SkierName + " " + curSkierVideoEntry.AgeGroup + " Round " + curSkierVideoEntry.Round + " Pass " + curSkierVideoEntry.Pass );
                        curFormData.Add( "description", curSkierVideoEntry.SkierName + " " + curSkierVideoEntry.AgeGroup + " Round: " + curSkierVideoEntry.Round + " Pass: " + curSkierVideoEntry.Pass );
                        curFormData.Add( "privacy", "2" );

                        int curIdx = 0;
                        foreach ( String curEntry in TagsListBox.CheckedItems ) {
                            curFormData.Add( "tag_names[" + curIdx + "]", curEntry );
                            curIdx++;
                        }
                        curFormData.Add( "tag_names[" + curIdx + "]", mySanctionNum );
                        curIdx++;
                        curFormData.Add( "tag_names[" + curIdx + "]", curSkierVideoEntry.AgeGroup );
                        curIdx++;
                        curFormData.Add( "tag_names[" + curIdx + "]", "Tricks" );
                        curIdx++;
                        curFormData.Add( "tag_names[" + curIdx + "]", "Round " + curSkierVideoEntry.Round );
                        curIdx++;
                        curFormData.Add( "tag_names[" + curIdx + "]", "Pass " + curSkierVideoEntry.Pass );
                        curIdx++;

                        curHeaderParams = new NameValueCollection();
                        curHeaderParams.Add( myApiKeyName, myApiKey );

                        //List<KeyValuePair<String, String>> curResponseDataList = SendMessageHttp.sendMessagePostFileUpload( myVideoLoadUrl, curSkierVideoEntry.VideoFileName, curFileFormName, curHeaderParams, curFormData, null, null );
                        Dictionary<string, object> curResponseDataList = SendMessageHttp.sendMessagePostFileUpload( myVideoLoadUrl, curSkierVideoEntry.VideoFileName, curFileFormName, curHeaderParams, curFormData, null, null );

                        if ( curResponseDataList == null ) {
                            curViewRow.Cells["SelectedLoadStatus"].Value = "Video load failed";
                        } else {
                            if ( curResponseDataList != null && curResponseDataList.Count > 0 ) {
                                if ( curResponseDataList.ContainsKey( "embed_code" ) ) {
                                    bool curResults = false;
                                    foreach ( KeyValuePair<String, object> curEntry in curResponseDataList ) {
                                        if ( curEntry.Key.Equals( "embed_code" ) ) {
                                            curResults = updateSkierScoreVideoUrl( curSkierVideoEntry, (String)curEntry.Value );
                                            if ( curResults ) {
                                                curViewRow.Cells["SelectedLoadStatus"].Value = "Video load complete";
                                            } else {
                                                curViewRow.Cells["SelectedLoadStatus"].Value = "Error encountered attaching video URL to skier";
                                            }
                                            break;
                                        }
                                    }

                                    if ( !curResults ) {
                                        curViewRow.Cells["SelectedLoadStatus"].Value = "Video load failed, API response not recognized";
                                    }

                                } else if ( curResponseDataList.ContainsKey( "Error" ) ) {
                                    curViewRow.Cells["SelectedLoadStatus"].Value = curResponseDataList["Error"].ToString();
                                }
                            } else {
                                curViewRow.Cells["SelectedLoadStatus"].Value = "Video load failed";
                            }
                        }

                        curViewIdx++;
                    }
                    myProgressInfo.Close();
                } else {
                    myProgressInfo.Close();
                }
            
            } catch ( Exception ex ) {
                //MessageBox.Show( curMethodName + ":Error encountered\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
                myProgressInfo.Close();

			} finally {
				Cursor.Current = Cursors.Default;
			}
		}

        private void AddTagButton_Click(object sender, EventArgs e) {
            if (NewTagTextbox.Text.Length > 0) {
                if (!( TagsListBox.Items.Contains( NewTagTextbox.Text ) )) {
                    TagsListBox.Items.Add( NewTagTextbox.Text );
                }
            }
        }

        private bool searchForMatchingSkier(SkierVideoEntry inSkierVideoEntry) {
            String curMethodName = "searchForMatchingSkier";
            String curNumValue = "";
            Int16 curPass = 1, curRound = 1;
            if (myTrickRounds == 1) curRound = myTrickRounds;

            int curDelimIdx = Path.GetFileName( inSkierVideoEntry.VideoFileName ).LastIndexOf( '.' );
            DataRow[] curFindSkiers = null;
            String curFileName = Path.GetFileName( inSkierVideoEntry.VideoFileName).Substring( 0, curDelimIdx );

			//Search for uppercase characters and insert a space to allow the string to be split into nodes by camelcase notation
			String curFileNameMod = Regex.Replace( curFileName, @"(\p{Lu})", " $1" ).TrimStart();
			//char[] myCharDelimLimit = new char[] { ' ', '_', '-', ',' };
			String[] curFileNameNodes = curFileNameMod.Split( myCharDelimLimit, StringSplitOptions.RemoveEmptyEntries );

			Cursor.Current = Cursors.WaitCursor;
			if ( curFileNameNodes.Length > 1) {
				/*
                 * Search list of all skiers to determine if the parsed file name can be matched to a skier
                 */
				// Determine if first 2 nodes of file name matches skier name exactly as lastname, firstname
				String curFilter = String.Format( "SkierName = '{0}, {1}'", curFileNameNodes[0], curFileNameNodes[1] );
				curFindSkiers = myFullSkierDataTable.Select( curFilter );
				if ( curFindSkiers.Length == 0 ) {
					// Determine if first 2 nodes of file name matches skier name exactly as firstname, lastname
					curFilter = String.Format( "SkierName = '{0}, {1}'", curFileNameNodes[1], curFileNameNodes[0] );
					curFindSkiers = myFullSkierDataTable.Select( curFilter );
				}
				if ( curFindSkiers.Length == 0 ) {
					// Determine if first 2 nodes of file name has a familia match to the skier name as starts with lastname section and ends with firstname section
					curFilter = String.Format( "SkierName like '{0}%' AND SkierName like '%{1}'", curFileNameNodes[0], curFileNameNodes[1] );
					curFindSkiers = myFullSkierDataTable.Select( curFilter );
				}
				if ( curFindSkiers.Length == 0 ) {
					// Determine if first 2 nodes of file name has a familia match to the skier name as starts with firstname section and ends with lastname section
					curFilter = String.Format( "SkierName like '{0}%' AND SkierName like '%{1}'", curFileNameNodes[1], curFileNameNodes[0] );
					curFindSkiers = myFullSkierDataTable.Select( curFilter );
				}
                if ( curFindSkiers.Length == 0 ) {
                    curFilter = String.Format("SkierName like '%{0}%' AND SkierName like '%{1}%'", curFileNameNodes[1], curFileNameNodes[2]);
                    curFindSkiers = myFullSkierDataTable.Select(curFilter);
                }
                if ( curFindSkiers.Length == 0 ) {
                    curFilter = String.Format("SkierName like '%{0}%' OR SkierName like '%{1}%' OR SkierName like '%{2}%'", curFileNameNodes[0], curFileNameNodes[1], curFileNameNodes[2]);
                    curFindSkiers = myFullSkierDataTable.Select(curFilter);
                }

                //Find Round Number
                foreach ( String curEntry in curFileNameNodes ) {
                    if ( curEntry.ToLower().StartsWith("r")
                        || curEntry.ToLower().StartsWith("rd")
                        || curEntry.ToLower().StartsWith("round")
                        ) {
                        try {
                            curNumValue = Regex.Match(curEntry, @"\d+").Value;
                            if ( curNumValue.Length > 0 ) {
                                Int16 tempRound = Int16.Parse(curNumValue);
                                if ( tempRound > 0 &&  tempRound <= myTrickRounds ) {
                                    curRound = tempRound;
                                }
                            }
                        } catch ( Exception ex ) {
                            MessageBox.Show(curMethodName + ":Error encountered\n\nError: " + ex.Message);
                        }
                    }
                }

                //Find Pass Number
                foreach ( String curEntry in curFileNameNodes ) {
                    if ( curEntry.ToLower().Equals("p1")
                        || curEntry.ToLower().Equals("p2")
                        || curEntry.ToLower().Equals("pass1")
                        || curEntry.ToLower().Equals("pass2")
						|| curEntry.ToLower().Equals("1")
						|| curEntry.ToLower().Equals("2")
						) {
                        try {
                            curNumValue = Regex.Match(curEntry, @"\d+").Value;
                            if ( curNumValue.Length > 0 ) {
                                curPass = Int16.Parse(curNumValue);
                                if ( curPass < 1 || curPass > 2 ) {
                                    curPass = 1;
                                }
                            }
                        } catch ( Exception ex ) {
                            MessageBox.Show(curMethodName + ":Error encountered\n\nError: " + ex.Message);
                        }
                    }
                }
            }

            if ( curRound == 0 ) curRound = 1;
			if (curFindSkiers != null && curFindSkiers.Length == 1 && curRound > 0 && curPass > 0
				&& checkForUniqueAssignment(inSkierVideoEntry)) {
				//Use record found by filtered search
				inSkierVideoEntry.MemberId = (String)curFindSkiers[0]["MemberId"];
				inSkierVideoEntry.SkierName = (String)curFindSkiers[0]["SkierName"];
				inSkierVideoEntry.AgeGroup = (String)curFindSkiers[0]["AgeGroup"];
				inSkierVideoEntry.Round = curRound;
				inSkierVideoEntry.Pass = curPass;
			}
			if (inSkierVideoEntry.MemberId.Length > 1 && checkForUniqueAssignment(inSkierVideoEntry) ) {
				// Unique skier assigned
			} else {
                if ( curFindSkiers == null ) {
                    mySkierSelectDialog.SkierMatchList = null;

				} else if ( curFindSkiers.Length > 1 ) {
                    mySkierSelectDialog.SkierMatchList = curFindSkiers;

				} else {
                    mySkierSelectDialog.SkierMatchList = null;
                }

                if ( curRound > 0 ) mySkierSelectDialog.SkierRound = curRound;
                if ( curPass > 0 ) mySkierSelectDialog.SkierPass = curPass;
                mySkierSelectDialog.FileName = Path.GetFileName(inSkierVideoEntry.VideoFileName);
                DialogResult curDialogResult = mySkierSelectDialog.ShowDialog();
                if ( curDialogResult == DialogResult.OK ) {
                    inSkierVideoEntry.MemberId = mySkierSelectDialog.SkierMemberId;
                    inSkierVideoEntry.SkierName = mySkierSelectDialog.SkierNameSelected;
                    inSkierVideoEntry.AgeGroup = mySkierSelectDialog.SkierAgeGroup;
                    inSkierVideoEntry.Round = mySkierSelectDialog.SkierRound;
                    inSkierVideoEntry.Pass = mySkierSelectDialog.SkierPass;
                }
            }

            if (inSkierVideoEntry.MemberId.Length > 0 ) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT PK FROM TrickScore " );
                curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( " AND MemberId = '" + inSkierVideoEntry.MemberId + "' " );
                curSqlStmt.Append( " AND AgeGroup = '" + inSkierVideoEntry.AgeGroup + "' " );
                curSqlStmt.Append( " AND Round = " + inSkierVideoEntry.Round + " " );
                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if (curDataTable.Rows.Count == 1) {
                    inSkierVideoEntry.SkierScorePK = (Int64)curDataTable.Rows[0]["PK"];
					Cursor.Current = Cursors.Default;
					return true;
                } else {
					Cursor.Current = Cursors.Default;
					MessageBox.Show( "A matching skier, round and pass could not be identified for this video \n\n" + Path.GetFileName( inSkierVideoEntry.VideoFileName ) );
                    return false;
                }
            } else {
				//MessageBox.Show( "Skier, round and pass have not been selected for this video " + Path.GetFileName(inSkierVideoEntry.VideoFileName) );
				Cursor.Current = Cursors.Default;
				return false;
            }
        }

		private bool checkForUniqueAssignment( SkierVideoEntry inSkierVideoEntry) {
			foreach (SkierVideoEntry curSkierVideoEntry in mySkierVideoList) {
				if ( curSkierVideoEntry.MemberId == inSkierVideoEntry.MemberId
					&& curSkierVideoEntry.AgeGroup == inSkierVideoEntry.AgeGroup
					&& curSkierVideoEntry.Round == inSkierVideoEntry.Round
					&& curSkierVideoEntry.Pass == inSkierVideoEntry.Pass
					) {
					MessageBox.Show(String.Format("Video {0} assigned to {1}, round {2}, and pass {3} that has already been used by {4}"
						, inSkierVideoEntry.VideoFileName, curSkierVideoEntry.SkierName, curSkierVideoEntry.Round, curSkierVideoEntry.Pass, curSkierVideoEntry.VideoFileName) );
					return false;
				}
			}

			return true;
		}

		private bool updateSkierScoreVideoUrl(SkierVideoEntry inSkierVideoEntry, String inVideoUrl) {
            String curMethodName = "updateSkierScoreVideoUrl";
            int rowsProc = 0;

            try {
                if (inSkierVideoEntry.SkierScorePK > 0 && inSkierVideoEntry.Pass > 0 ) {
                    String curVideoUrl = inVideoUrl.Replace( "630", "410" );
                    curVideoUrl = curVideoUrl.Replace( "420", "273" );

                    StringBuilder curSqlStmt = new StringBuilder( "Select count(*) From TrickVideo " );
                    curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                    curSqlStmt.Append( "AND MemberId = '" + inSkierVideoEntry.MemberId + "' " );
                    curSqlStmt.Append( "AND AgeGroup = '" + inSkierVideoEntry.AgeGroup + "' " );
                    curSqlStmt.Append( "AND Round = '" + inSkierVideoEntry.Round + "' " );
                    int curReadCount = (Int32)DataAccess.ExecuteScalarCommand( curSqlStmt.ToString() );
                    if (curReadCount == 0) {
                        //If entry does not currently exist then add it
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Insert into TrickVideo (" );
                        curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, Pass1VideoUrl, Pass2VideoUrl, LastUpdateDate" );
                        curSqlStmt.Append( ") Values (" );
                        curSqlStmt.Append( "'" + mySanctionNum + "' " );
                        curSqlStmt.Append( ", '" + inSkierVideoEntry.MemberId + "' " );
                        curSqlStmt.Append( ",'" + inSkierVideoEntry.AgeGroup + "' " );
                        curSqlStmt.Append( ", " + inSkierVideoEntry.Round + ", '', '', GetDate() ) " );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TrickVideo set " );
                    if (inSkierVideoEntry.Pass == 1) {
                        curSqlStmt.Append( "Pass1VideoUrl = '" + HelperFunctions.escapeString( curVideoUrl ) + "' " );
                    } else {
                        curSqlStmt.Append( "Pass2VideoUrl = '" + HelperFunctions.escapeString( curVideoUrl ) + "' " );
                    }
                    curSqlStmt.Append( ", LastUpdateDate = GetDate() " );
                    curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                    curSqlStmt.Append( "AND MemberId = '" + inSkierVideoEntry.MemberId + "' " );
                    curSqlStmt.Append( "AND AgeGroup = '" + inSkierVideoEntry.AgeGroup + "' " );
                    curSqlStmt.Append( "AND Round = '" + inSkierVideoEntry.Round + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    if (rowsProc > 0) {
						LiveWebHandler.sendCurrentSkier( "TrickVideo", mySanctionNum, inSkierVideoEntry.MemberId, inSkierVideoEntry.AgeGroup, Convert.ToByte( inSkierVideoEntry.Round), 0 );
						return true;
                    } else {
                        return false;
                    }


                } else {
                    return false;
                }
            } catch (Exception ex) {
                MessageBox.Show( curMethodName + ":Error encountered\n\nError: " + ex.Message );
                String curMsg = curMethodName + ":Exception=" + ex.Message;
                Log.WriteFile( curMsg );
            }

            return true;
        }

        private void updateProperty(String inKey, String inValue, Int16 inOrder) {
            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourProperties set " );
                curSqlStmt.Append( "PropValue = '" + inValue + "' " );
                curSqlStmt.Append( ", PropOrder = " + inOrder + " " );
                curSqlStmt.Append( "Where Sanctionid = '" + Properties.Settings.Default.AppSanctionNum + "' " );
                curSqlStmt.Append( "AND PropKey = '" + inKey + "' " );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                if (rowsProc == 0) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert  TourProperties (" );
                    curSqlStmt.Append( "Sanctionid, PropKey, PropValue, PropOrder" );
                    curSqlStmt.Append( ") Values ( " );
                    curSqlStmt.Append( "'" + mySanctionNum + "', '" + inKey + "', '" + inValue + "', " + inOrder.ToString() + " )" );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                }
            } catch (Exception excp) {
                MessageBox.Show( "Exception encountered updating property \n" + inKey + "=" + inValue + "\n" + excp.Message );
            }
        }

        private void getTagProperty() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PropKey, PropValue FROM TourProperties " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND PropKey = 'VideoTags' " );
            curSqlStmt.Append( "Order by PropOrder, PropKey, PropValue " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                String[] curTagList = ( (String)curDataTable.Rows[0]["PropValue"] ).Split( ',' );
                foreach (String curEntry in curTagList) {
                    TagsListBox.Items.Add( curEntry );
                }
            } else {
                TagsListBox.Items.Add( ((String)myTourRow["Name"]).Trim() );
                TagsListBox.Items.Add( ( (String)myTourRow["EventDates"] ).Trim() );
                String[] curEventLocItems = ( (String)myTourRow["EventLocation"] ).Split( ',' );
                foreach (String curEntry in curEventLocItems) {
                    TagsListBox.Items.Add( curEntry.Trim() );
                }
            }
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            RowStatusLabel.Text = "Row " + ( e.RowIndex + 1 ) + " of " + ( (DataGridView)sender ).Rows.Count;
        }

        private void ExportLoadedButton_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( loadedVideoDataGridView );
        }

	}

	public class SkierVideoEntry {
        public String VideoFileName;
        public String MemberId;
        private String mySkierName;
        public String AgeGroup;
        public Int16 Round;
        public Int16 Pass;
        public Int64 SkierScorePK;

        private String myFirstName;
		private String myLastName;

		public SkierVideoEntry(String inFileName, String inMemberId, String inSkierName, String inAgeGroup, Int16 inRound, Int16 inPass, Int64 inPK) {
            VideoFileName = inFileName;
            MemberId = inMemberId;
            SkierName = inSkierName;
            AgeGroup = inAgeGroup;
            Round = inRound;
            Pass = inPass;
            SkierScorePK = inPK;
        }
        public String SkierName {
            get { return mySkierName; }
            set {
                if (value.Length > 0) {
                    int curDelimIdx = value.IndexOf( ',' );
                    if (curDelimIdx > 0) {
                        myLastName = value.Substring( 0, curDelimIdx );
                        myFirstName = value.Substring( curDelimIdx + 1 ).Trim();
                        mySkierName = myFirstName + " " + myLastName;
                    } else {
                        mySkierName = value;
                    }
                } else {
                    mySkierName = value;
                }
            }
        }
        public String LastName {
            get { return myLastName; } set { myLastName = value; }
        }
		public String FirstName {
			get { return myFirstName; }
			set { myFirstName = value; }

		}
	}

}
