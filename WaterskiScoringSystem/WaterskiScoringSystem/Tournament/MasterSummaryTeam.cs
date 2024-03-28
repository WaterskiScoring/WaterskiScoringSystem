using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class MasterSummaryTeam : Form {
        private String myPageBreak = "\\n";
        private bool myShowTeam = false;
        private Int16 myNumPerTeam;
        private String mySanctionNum = null;
        private String myTourRules;
        private DataRow myTourRow;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;
        private DataTable myTeamDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private TourProperties myTourProperties;

        public MasterSummaryTeam() {
            InitializeComponent();
        }

        private void MasterSummaryTeam_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TeamSummary_Width > 0 ) {
                this.Width = Properties.Settings.Default.TeamSummary_Width;
            }
            if ( Properties.Settings.Default.TeamSummary_Height > 0 ) {
                this.Height = Properties.Settings.Default.TeamSummary_Height;
            }
            if ( Properties.Settings.Default.TeamSummary_Location.X > 0
                && Properties.Settings.Default.TeamSummary_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TeamSummary_Location;
            }

			bool isSanctionAvailable = true;
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				isSanctionAvailable = false;

			} else if ( mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				isSanctionAvailable = false;
			} else {
				//Retrieve selected tournament attributes
				myTourRow = getTourData();
				if ( myTourRow == null ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					isSanctionAvailable = false;
				}
			}
			if ( !isSanctionAvailable ) {
				Timer curTimerObj = new Timer();
				curTimerObj.Interval = 15;
				curTimerObj.Tick += new EventHandler( CloseWindowTimer );
				curTimerObj.Start();
				return;
			}
			myTourRules = HelperFunctions.getDataRowColValue(myTourRow, "Rules", "");

			loadGroupList();
			if ( !( myTourRules.ToLower().Equals( "awsa" ) ) ) plcmtAWSATeamButton.Visible = false;

			myTourProperties = TourProperties.Instance;

			FilterReportButton.Visible = false;
			FilterReportButton.Enabled = false;

			bool isConvertComplete = Int16.TryParse( myTourProperties.TeamSummary_NumPerTeam, out myNumPerTeam );
            if ( !isConvertComplete ) myNumPerTeam = 4;
            NumPerTeamTextBox.Text = myNumPerTeam.ToString();

			bestScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "best" )) bestScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "total" )) totalScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryDataType.ToLower().Equals( "final" )) finalScoreButton.Checked = true;

            pointsScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtMethod.ToLower().Equals( "hcap" )) handicapScoreButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtMethod.ToLower().Equals( "points" )) pointsScoreButton.Checked = true;

            plcmtTourButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "tour" )) plcmtTourButton.Checked = true;
            if (myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "div" )) divPlcmtButton.Checked = true;
            if (myTourProperties.SlalomTeamSummaryPlcmtOrg.ToLower().Equals( "group" )) groupPlcmtButton.Checked = true;
            if ( myTourProperties.TeamSummaryPlcmtOrg.ToLower().Equals( "awsa" ) ) plcmtAWSATeamButton.Checked = true;

            plcmtPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "nops" )) nopsPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "plcmt" )) plcmtPointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "kbase" )) kBasePointsButton.Checked = true;
            if (myTourProperties.TeamSummaryPointsMethod.ToLower().Equals( "ratio" )) ratioPointsButton.Checked = true;

            CategoryGroupBox.Location = new Point( 825, 49 );
			EventGroupBox.Location = new Point( 825, 85 );
			if ( plcmtAWSATeamButton.Checked ) {
				CategoryGroupBox.Visible = true;
				CategoryGroupBox.Enabled = true;
				EventGroupBox.Visible = true;
				EventGroupBox.Enabled = true;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

				EventGroupList.Visible = false;
				EventGroupList.Enabled = false;
				EventGroupListLabel.Visible = false;
				EventGroupListLabel.Enabled = false;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

			} else {
				CategoryGroupBox.Visible = false;
				CategoryGroupBox.Enabled = false;
				EventGroupBox.Visible = false;
				EventGroupBox.Enabled = false;

				EventGroupList.Visible = true;
				EventGroupList.Enabled = true;
				EventGroupListLabel.Visible = true;
				EventGroupListLabel.Enabled = true;
			}
        }

		private void CloseWindowTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
			this.Close();
		}

		private void TeamSummary_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TeamSummary_Width = this.Size.Width;
                Properties.Settings.Default.TeamSummary_Height = this.Size.Height;
                Properties.Settings.Default.TeamSummary_Location = this.Location;
            }
        }

        private void BindingSource_DataError(object sender, BindingManagerDataErrorEventArgs e) {
            MessageBox.Show("Binding Exception: " + e.Exception.Message);
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show("Error happened " + e.Context.ToString());
            if (e.Context == DataGridViewDataErrorContexts.Commit) {
                MessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange) {
                MessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing) {
                MessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl) {
                MessageBox.Show("leave control error");
            }
            if ((e.Exception) is ConstraintException) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        public void navRefresh_Click( object sender, EventArgs e ) {
            this.Cursor = Cursors.WaitCursor;

            // Retrieve data from database
            if ( mySanctionNum != null && myTourRow != null ) {
                String curDataType = "all", curPlcmtMethod = "score";
                String curPlcmtOrg, curPointsMethod;
                CalcScoreSummary curCalcSummary = new CalcScoreSummary();

                // Retrieve data from database depending on selection criteria
                String curMsg = "Tournament scores retrieved ";

                if ( plcmtAWSATeamButton.Checked ) {
                    bestScoreButton.Checked = true;
                    pointsScoreButton.Checked = true;
                    nopsPointsButton.Checked = true;
                }

                if ( bestScoreButton.Checked ) {
                    curDataType = "best";
                    winStatusMsg.Text = curMsg + "- best scores ";
                } else if ( totalScoreButton.Checked ) {
                    curDataType = "total";
                    winStatusMsg.Text = curMsg + "- total scores";
                } else if ( finalScoreButton.Checked ) {
                    curDataType = "final";
                    winStatusMsg.Text = curMsg + "- final scores";
                }
                myTourProperties.TeamSummaryDataType = curDataType;

                if ( pointsScoreButton.Checked ) {
                    curPlcmtMethod = "points";
                } else if ( handicapScoreButton.Checked ) {
                    curPlcmtMethod = "hcap";
                } else if ( ratioScoreButton.Checked ) {
                    curPlcmtMethod = "ratio";
                } else {
                    curPlcmtMethod = "points";
                }
                myTourProperties.TeamSummaryPlcmtMethod = curPlcmtMethod;

                if ( divPlcmtButton.Checked ) {
                    curPlcmtOrg = "div";
                } else if (groupPlcmtButton.Checked) {
                    curPlcmtOrg = "group";
                } else if (plcmtTourButton.Checked) {
                    curPlcmtOrg = "tour";
                } else if ( plcmtAWSATeamButton.Checked ) {
                    curPlcmtOrg = "awsa";
                } else {
                    curPlcmtOrg = "tour";
                }
                myTourProperties.TeamSummaryPlcmtOrg = curPlcmtOrg;

                if ( nopsPointsButton.Checked ) {
                    curPointsMethod = "nops";
                } else if ( plcmtPointsButton.Checked ) {
                    curPointsMethod = "plcmt";
                } else if ( kBasePointsButton.Checked ) {
                    curPointsMethod = "kbase";
                } else if ( ratioPointsButton.Checked ) {
                    curPointsMethod = "ratio";
                } else {
                    curPointsMethod = "nops";
                }
                myTourProperties.TeamSummaryPointsMethod = curPointsMethod;

                String curGroupValue = "";
                try {
                    curGroupValue = EventGroupList.SelectedItem.ToString();
                    if (!( curGroupValue.ToLower().Equals( "all" ) )) {
                        if (myTourRules.ToLower().Equals( "ncwsa" )) {
                            if (plcmtTourButton.Checked) {
                                curGroupValue = "All";
                            } else if (curGroupValue.ToUpper().Equals( "MEN A" )) {
                                curGroupValue = "CM";
                            } else if (curGroupValue.ToUpper().Equals( "WOMEN A" )) {
                                curGroupValue = "CW";
                            } else if (curGroupValue.ToUpper().Equals( "MEN B" )) {
                                curGroupValue = "BM";
                            } else if (curGroupValue.ToUpper().Equals( "WOMEN B" )) {
                                curGroupValue = "BW";
                            } else {
                                curGroupValue = "All";
                            }
                        }
                    }
                } catch {
                    curGroupValue = "All";
                }

                if (myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" )) {
                    mySlalomDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadSlalomDataGrid(mySlalomDataTable);
                    myTrickDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Trick", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadTrickDataGrid(myTrickDataTable);
                    myJumpDataTable = curCalcSummary.CalcIwwfEventPlcmts(myTourRow, mySanctionNum, "Jump", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue);
                    loadJumpDataGrid(myJumpDataTable);

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam(mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam(myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam(myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod);
                    if ( plcmtTourButton.Checked ) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary(myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam);
                    }
                } else if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    if (plcmtTourButton.Checked) curPlcmtOrg = "div";
                    mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadSlalomDataGrid( mySlalomDataTable );
                    myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadTrickDataGrid( myTrickDataTable );
                    myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    loadJumpDataGrid( myJumpDataTable );

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );

                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamEventCombinedNcwsaSummary( myTeamDataTable, mySanctionNum );
                    }
                
                } else {
                    mySlalomDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    myTrickDataTable = curCalcSummary.getTrickSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );
                    myJumpDataTable = curCalcSummary.getJumpSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "Team", curGroupValue );

                    myTeamDataTable = curCalcSummary.getSlalomSummaryTeam( mySlalomDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getTrickSummaryTeam( myTeamDataTable, myTrickDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    myTeamDataTable = curCalcSummary.getJumpSummaryTeam( myTeamDataTable, myJumpDataTable, myTourRow, myNumPerTeam, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
                    if (plcmtTourButton.Checked) {
                        myTeamDataTable = curCalcSummary.CalcTeamCombinedSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam );
                    }
                    if ( plcmtAWSATeamButton.Checked ) {
                        mySlalomDataTable = curCalcSummary.CalcTeamAwsaEventPoints( mySlalomDataTable, "Slalom" );
                        myTrickDataTable = curCalcSummary.CalcTeamAwsaEventPoints( myTrickDataTable, "Trick" );
                        myJumpDataTable = curCalcSummary.CalcTeamAwsaEventPoints( myJumpDataTable, "Jump" );
                        myTeamDataTable = curCalcSummary.CalcTeamAwsaCombinedSummary( myTourRow, mySlalomDataTable, myTrickDataTable, myJumpDataTable, myNumPerTeam );
                    }
                    loadSlalomDataGrid( mySlalomDataTable );
                    loadTrickDataGrid( myTrickDataTable );
                    loadJumpDataGrid( myJumpDataTable );
                }

                String curSortCmd = "";
                if (TeamDiv.Visible) {
                    curSortCmd = "DivOrder ASC, Div ASC, TeamScoreTotal DESC";
                } else {
                    curSortCmd = "TeamScoreTotal DESC";
                }
                myTeamDataTable.DefaultView.Sort = curSortCmd;
                myTeamDataTable = myTeamDataTable.DefaultView.ToTable();
                loadTeamDataGrid( myTeamDataTable );
            }
            this.Cursor = Cursors.Default;
        }

		private void loadGroupList() {
            if (EventGroupList.DataSource == null) {
                if ( HelperFunctions.isCollegiateEvent( myTourRules ) ) {
					EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
				} else {
                    loadAgeGroupListFromData();
                }
            
            } else {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadAgeGroupListFromData();
                    }
                } else {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
						EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
					} else {
                        loadAgeGroupListFromData();
                    }
                }
            }
        }

        private void loadAgeGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT AgeGroup FROM EventReg "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' "
                + "Order by AgeGroup";
            DataTable curDataTable = getData( curSqlStmt );

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["AgeGroup"] );
            }
            EventGroupList.DataSource = curEventGroupList;
            if (curGroupValue.Length > 0) {
                foreach (String curValue in (ArrayList)EventGroupList.DataSource) {
                    if (curValue.Equals( curGroupValue )) {
                        EventGroupList.SelectedItem = curGroupValue;
                        EventGroupList.Text = curGroupValue;
                        return;
                    }
                }
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            } else {
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            }
        }

        private void loadTeamDataGrid(DataTable inDataTable) {
            String curGroup = "", prevGroup = "", curTeamPlcmtShow = "";
            int curTeamPlcmt = 0;
            Decimal curScore = 0, prevScore = -1;
            DataGridViewRow curViewRow;

            TeamSummaryDataGridView.Rows.Clear();
            int curIdx = 0;
            foreach ( DataRow curRow in inDataTable.Rows ) {
                curIdx = TeamSummaryDataGridView.Rows.Add();
                curViewRow = TeamSummaryDataGridView.Rows[curIdx];
                curScore = HelperFunctions.getDataRowColValueDecimal( curRow, "TeamScoreTotal", 0.0m );

                if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
					curGroup = HelperFunctions.getDataRowColValue( curRow, "Div", "" );
                }
                if (curGroup != prevGroup && curIdx > 0) {
                    curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                    TeamSummaryDataGridView.Rows[curIdx].Height = 4;
                    curViewRow.Cells["TeamPlcmtOverall"].Value = myPageBreak;

                    curTeamPlcmt = 0;
                    prevScore = -1;
                    curTeamPlcmtShow = "";
                    curIdx = TeamSummaryDataGridView.Rows.Add();
                    curViewRow = TeamSummaryDataGridView.Rows[curIdx];
                }

                curTeamPlcmt++;
                if (curScore == prevScore && curIdx > 0) {
                    curTeamPlcmtShow = (String)TeamSummaryDataGridView.Rows[curIdx - 1].Cells["TeamPlcmtOverall"].Value;
                    if (curTeamPlcmtShow.Contains( "T" )) {
                    } else {
                        if (curTeamPlcmtShow.Length < 3) {
                        } else {
                            curTeamPlcmtShow = curTeamPlcmtShow.Substring( 0, 3 ) + "T";
                            TeamSummaryDataGridView.Rows[curIdx - 1].Cells["TeamPlcmtOverall"].Value = curTeamPlcmtShow;
                        }
                    }
                } else {
                    curTeamPlcmtShow = curTeamPlcmt.ToString( "##0" ).PadLeft( 3, ' ' );
                    curTeamPlcmtShow += " ";
                }


				//HelperFunctions
				curViewRow.Cells["TeamPlcmtOverall"].Value = curTeamPlcmtShow;
				curViewRow.Cells["TeamDiv"].Value = HelperFunctions.getDataRowColValue( curRow, "Div", "" );
				curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curRow, "TeamCode", "" );
				curViewRow.Cells["TeamName"].Value = HelperFunctions.getDataRowColValue( curRow, "Name", "" );
				curViewRow.Cells["TeamScoreSlalom"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "TeamScoreSlalom", 0.0m ).ToString("#,##0");
				curViewRow.Cells["TeamPlcmtSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtSlalom", "" );
				curViewRow.Cells["TeamScoreTrick"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "TeamScoreTrick", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["TeamPlcmtTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtTrick", "" );
				curViewRow.Cells["TeamScoreJump"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "TeamScoreJump", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["TeamPlcmtJump"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtJump", "" );
				curViewRow.Cells["TeamScoreTotal"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "TeamScoreTotal", 0.0m ).ToString( "#,##0" );
				prevGroup = curGroup;
                prevScore = curScore;
            }
            loadPrintDataGrid();
        }

        private void loadSlalomDataGrid( DataTable inSlalomDataTable ) {
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            String curGroup = "", prevGroup = "";
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderSlalom ASC, AgeGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamSlalom ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamSlalom ASC, SkierName ASC";
            }
            inSlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inSlalomDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            slalomScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            //foreach ( DataRow curRow in curDataTable.Rows ) {
            foreach (DataRow curRow in curDataTable.Rows) {
                curShowRow = true;
                if ( HelperFunctions.isCollegiateEvent( myTourRules ) && plcmtTourButton.Checked) {
                    if (( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
                        || ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
                        ) {
                        curShowRow = true;
                    } else {
                        curShowRow = false;
                    }
                }
                if ( !curShowRow ) continue;

				curIdx = slalomScoreSummaryDataGridView.Rows.Add();
				curViewRow = slalomScoreSummaryDataGridView.Rows[curIdx];
				
				curGroup = HelperFunctions.getDataRowColValue( curRow, "TeamSlalom", "" );
				if ( TeamDiv.Visible ) {
					if ( divPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
					}
					if ( groupPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "EventGroup", "" );
					}
				}
				if ( curGroup != prevGroup && curIdx > 0 ) {
					curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
					slalomScoreSummaryDataGridView.Rows[curIdx].Height = 4;

					curIdx = slalomScoreSummaryDataGridView.Rows.Add();
					curViewRow = slalomScoreSummaryDataGridView.Rows[curIdx];
				}

				curViewRow.Cells["MemberIdSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "MemberId", "" );
				curViewRow.Cells["SanctionIdSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "SanctionId", "" );
				curViewRow.Cells["SkierNameSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["EventGroupSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "EventGroupSlalom", "" );
				curViewRow.Cells["AgeGroupSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curViewRow.Cells["PlcmtSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtSlalom", "" );

				curViewRow.Cells["ScoreSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "ScoreSlalom", "" );
				curViewRow.Cells["NopsScoreSlalom"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PointsSlalom", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["PlcmtPointsSlalom"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PlcmtPointsSlalom", 0.0m ).ToString("#,##0");
				curViewRow.Cells["TeamCodeSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "TeamSlalom", "" );
				curViewRow.Cells["RoundSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "RoundSlalom", "" );
				curViewRow.Cells["HCapBaseSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapBase", "" );
				curViewRow.Cells["HCapScoreSlalom"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapScore", "" );
				prevGroup = curGroup;
			}

			myShowTeam = curShowTeamSav;
            SlalomNumLabel.Text = slalomScoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void loadTrickDataGrid( DataTable inTrickDataTable ) {
            String curGroup = "", prevGroup = "";
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderTrick ASC, AgeGroup ASC, TeamTrick ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamTrick ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamTrick ASC, SkierName ASC";
            }
            inTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inTrickDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            trickScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            foreach (DataRow curRow in curDataTable.Rows) {
                curShowRow = true;
				if ( HelperFunctions.isCollegiateEvent( myTourRules ) && plcmtTourButton.Checked ) {
					if ( ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
						|| ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
						) {
						curShowRow = true;
					} else {
						curShowRow = false;
					}
				}
				if ( !curShowRow ) continue;

				curIdx = trickScoreSummaryDataGridView.Rows.Add();
				curViewRow = trickScoreSummaryDataGridView.Rows[curIdx];

				curGroup = HelperFunctions.getDataRowColValue( curRow, "TeamTrick", "" );
				if ( TeamDiv.Visible ) {
					if ( divPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
					}
					if ( groupPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "EventGroup", "" );
					}
				}

				if ( curGroup != prevGroup && curIdx > 0 ) {
					curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
					trickScoreSummaryDataGridView.Rows[curIdx].Height = 4;

					curIdx = trickScoreSummaryDataGridView.Rows.Add();
					curViewRow = trickScoreSummaryDataGridView.Rows[curIdx];
				}

				curViewRow.Cells["MemberIdTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "MemberId", "" );
				curViewRow.Cells["SanctionIdTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "SanctionId", "" );
				curViewRow.Cells["SkierNameTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["EventGroupTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "EventGroupTrick", "" );
				curViewRow.Cells["AgeGroupTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curViewRow.Cells["PlcmtTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtTrick", "" );

				curViewRow.Cells["ScoreTrick"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreTrick", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["NopsScoreTrick"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PointsTrick", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["PlcmtPointsTrick"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PlcmtPointsTrick", 0.0m ).ToString( "#,##0" );

				curViewRow.Cells["TeamCodeTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "TeamTrick", "" );
				curViewRow.Cells["RoundTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "RoundTrick", "" );
				curViewRow.Cells["HCapBaseTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapBase", "" );
				curViewRow.Cells["HCapScoreTrick"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapScore", "" );

				prevGroup = curGroup;
			}

			myShowTeam = curShowTeamSav;
            TrickNumLabel.Text = trickScoreSummaryDataGridView.Rows.Count.ToString();
        }

        private void loadJumpDataGrid( DataTable inJumpDataTable ) {
            String curGroup = "", prevGroup = "";
            bool curShowTeamSav = myShowTeam;
            bool curShowRow = true;
            myShowTeam = false;

            String curSortCmd = "";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd = "DivOrderJump ASC, AgeGroup ASC, TeamJump ASC, SkierName ASC";
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd = "EventGroup ASC, TeamJump ASC, SkierName ASC";
                }
            } else {
                curSortCmd = "TeamJump ASC, SkierName ASC";
            }
            inJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curDataTable = inJumpDataTable.DefaultView.ToTable();

            DataGridViewRow curViewRow;
            jumpScoreSummaryDataGridView.Rows.Clear();

            int curIdx = 0;
            foreach ( DataRow curRow in curDataTable.Rows ) {
                curShowRow = true;
				if ( HelperFunctions.isCollegiateEvent( myTourRules ) && plcmtTourButton.Checked ) {
					if ( ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cm" )
                        || ( (String)curRow["AgeGroup"] ).ToLower().Equals( "cw" )
                        ) {
                        curShowRow = true;
                    } else {
                        curShowRow = false;
                    }
                }
				if ( !curShowRow ) continue;

				curIdx = jumpScoreSummaryDataGridView.Rows.Add();
				curViewRow = jumpScoreSummaryDataGridView.Rows[curIdx];

				curGroup = HelperFunctions.getDataRowColValue( curRow, "TeamJump", "" );
				if ( TeamDiv.Visible ) {
					if ( divPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
					}
					if ( groupPlcmtButton.Checked ) {
						curGroup += "-" + HelperFunctions.getDataRowColValue( curRow, "EventGroup", "" );
					}
				}

				if ( curGroup != prevGroup && curIdx > 0 ) {
					curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
					jumpScoreSummaryDataGridView.Rows[curIdx].Height = 4;

					curIdx = jumpScoreSummaryDataGridView.Rows.Add();
					curViewRow = jumpScoreSummaryDataGridView.Rows[curIdx];
				}

				curViewRow.Cells["MemberIdJump"].Value = HelperFunctions.getDataRowColValue( curRow, "MemberId", "" );
				curViewRow.Cells["SanctionIdJump"].Value = HelperFunctions.getDataRowColValue( curRow, "SanctionId", "" );
				curViewRow.Cells["SkierNameJump"].Value = HelperFunctions.getDataRowColValue( curRow, "SkierName", "" );
				curViewRow.Cells["EventGroupJump"].Value = HelperFunctions.getDataRowColValue( curRow, "EventGroupJump", "" );
				curViewRow.Cells["AgeGroupJump"].Value = HelperFunctions.getDataRowColValue( curRow, "AgeGroup", "" );
				curViewRow.Cells["PlcmtJump"].Value = HelperFunctions.getDataRowColValue( curRow, "PlcmtJump", "" );

				curViewRow.Cells["ScoreMeters"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreMeters", 0.0m ).ToString( "###.0" );
				curViewRow.Cells["ScoreFeet"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "ScoreFeet", 0.0m ).ToString( "##0" );
				curViewRow.Cells["NopsScoreJump"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PointsJump", 0.0m ).ToString( "#,##0" );
				curViewRow.Cells["PlcmtPointsJump"].Value = HelperFunctions.getDataRowColValueDecimal( curRow, "PlcmtPointsJump", 0.0m ).ToString( "#,##0" );

				curViewRow.Cells["TeamCodeJump"].Value = HelperFunctions.getDataRowColValue( curRow, "TeamJump", "" );
				curViewRow.Cells["RoundJump"].Value = HelperFunctions.getDataRowColValue( curRow, "RoundJump", "" );
				curViewRow.Cells["HCapBaseJump"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapBase", "" );
				curViewRow.Cells["HCapScoreJump"].Value = HelperFunctions.getDataRowColValue( curRow, "HCapScore", "" );

				ScoreMeters.HeaderText = "Score";
				prevGroup = curGroup;
			}

			JumpNumLabel.Text = jumpScoreSummaryDataGridView.Rows.Count.ToString();
            myShowTeam = curShowTeamSav;
        }

        private void loadPrintDataGrid() {
            if ( myShowTeam ) ShowAllButton_Click( null, null );
			int scoreIdxSlalom = 0, scoreIdxTrick = 1, scoreIdxJump = 2, scoreIdxTotal = 3;
			Decimal[] awsaTeamTotal = new decimal[4] { 0, 0, 0, 0 };

			DataRow[] curSlalomTeamSkierList = null;
            DataRow[] curTrickTeamSkierList = null;
            DataRow[] curJumpTeamSkierList = null;
            //DataRow[] curFindRows;
            int curTeamPlcmt = 0;
            String curFilterCmd = "", curDiv = "", prevDiv = "";
            String curSortCmd = "TeamSlalom ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtSlalom ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtSlalom ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtSlalom ASC";
                PrintTeamDiv.Visible = false;
            }
            curSortCmd += ", SkierName ASC, Round ASC";
            mySlalomDataTable.DefaultView.Sort = curSortCmd;
            DataTable curSlalomDataTable = mySlalomDataTable.DefaultView.ToTable();

            curSortCmd = "TeamTrick ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtTrick ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtTrick ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtTrick ASC";
            }
            myTrickDataTable.DefaultView.Sort = curSortCmd;
            DataTable curTrickDataTable = myTrickDataTable.DefaultView.ToTable();

            curSortCmd = "TeamJump ASC";
            if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                if ( divPlcmtButton.Checked) {
                    curSortCmd += ", AgeGroup ASC, PlcmtJump ASC";
                    PrintTeamDiv.Visible = true;
                }
                if (groupPlcmtButton.Checked) {
                    curSortCmd += ", EventGroup ASC, PlcmtJump ASC";
                    PrintTeamDiv.Visible = true;
                }
            } else {
                curSortCmd += ", PlcmtJump ASC";
            }
            myJumpDataTable.DefaultView.Sort = curSortCmd;
            DataTable curJumpDataTable = myJumpDataTable.DefaultView.ToTable();

            if ( myTourRules.ToLower().Equals("awsa") && plcmtAWSATeamButton.Checked ) {
                PrintPointsSlalom.HeaderText = "NOPS";
                PrintPointsTrick.HeaderText = "NOPS";
                PrintPointsJump.HeaderText = "NOPS";
            } else {
                PrintPointsSlalom.HeaderText = "Points";
                PrintPointsTrick.HeaderText = "Points";
                PrintPointsJump.HeaderText = "Points";
            }

			//Load data for print data grid
			int curPrintIdx = 0, curSkierIdx = 0, curSkierIdxMax = 0, curTeamSummaryRowIdx = 0, curTeamSummarySummaryRowIdx = 0;
            String curTeamCode;
			DataGridViewRow curPrintRow = null;
			PrintDataGridView.Rows.Clear();
			foreach ( DataRow curTeamRow in myTeamDataTable.Rows ) {
                curTeamCode = HelperFunctions.getDataRowColValue( curTeamRow, "TeamCode", "" );
				try {
					if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
						if ( divPlcmtButton.Checked ) curDiv = (String) curTeamRow["Div"];
						if ( groupPlcmtButton.Checked ) curDiv = (String) curTeamRow["Div"];
						if ( curDiv != prevDiv ) {
							curTeamPlcmt = 0;
							if ( curPrintIdx > 0 ) curPrintRow.Cells["PrintTeamPlcmt"].Value = myPageBreak;
						}

						curTeamPlcmt++;
						curPrintIdx = PrintDataGridView.Rows.Add();
						curPrintRow = PrintDataGridView.Rows[curPrintIdx];
						curPrintRow.Cells["PrintTeamPlcmt"].Value = curTeamPlcmt.ToString();
						curPrintRow.Cells["PrintTeamDiv"].Value = (String) curTeamRow["Div"];

					} else {
						if ( curPrintIdx == 0 ) curTeamPlcmt = 0;
						curTeamPlcmt++;
						curPrintIdx = PrintDataGridView.Rows.Add();
						curPrintRow = PrintDataGridView.Rows[curPrintIdx];
						curPrintRow.Cells["PrintTeamPlcmt"].Value = curTeamPlcmt.ToString();
					}

					curTeamSummaryRowIdx = curPrintIdx;
					String curValue = PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamPlcmt"].Value.ToString();
                    bool curResult = int.TryParse( curValue, out curTeamSummarySummaryRowIdx );
					if ( curResult ) curTeamSummarySummaryRowIdx -= 1;

                    curPrintRow.Cells["PrintTeam"].Value = curTeamCode + "-" + HelperFunctions.getDataRowColValue( curTeamRow, "Name", "" );

					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventSlalomButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = ( (Decimal) curTeamRow["TeamScoreSlalom"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curTeamRow["TeamScoreSlalom"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsSlalom"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curTeamRow["PlcmtSlalom"];
					} catch {
						curPrintRow.Cells["PrintPlcmtSlalom"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventTrickButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = ( (Decimal) curTeamRow["TeamScoreTrick"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTeamRow["TeamScoreTrick"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsTrick"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTeamRow["PlcmtTrick"];
					} catch {
						curPrintRow.Cells["PrintPlcmtTrick"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) {
								curPrintRow.Cells["PrintPlcmtPointsJump"].Value = ( (Decimal) curTeamRow["TeamScoreJump"] ).ToString( "##,##0" );

							} else {
								if ( EventSlalomButton.Checked && curTeamPlcmt > 0 ) {
									curPrintRow.Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

								} else {
									curPrintRow.Cells["PrintPlcmtPointsJump"].Value = "";
								}
							}

						} else {
							curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curTeamRow["TeamScoreJump"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintPointsJump"].Value = "";
					}
					try {
						curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curTeamRow["PlcmtJump"];
					} catch {
						curPrintRow.Cells["PrintPlcmtJump"].Value = "";
					}
					try {
						if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
							if ( CategoryAllButton.Checked && ( EventAllButton.Checked || EventJumpButton.Checked ) ) {
								curPrintRow.Cells["PrintTeamScoreTotal"].Value = ( (Decimal) curTeamRow["TeamScoreTotal"] ).ToString( "##,##0" );

							} else {
								curPrintRow.Cells["PrintTeamScoreTotal"].Value = "";
							}

						} else {
							curPrintRow.Cells["PrintTeamScoreTotal"].Value = ( (Decimal) curTeamRow["TeamScoreTotal"] ).ToString( "##,##0" );
						}
					} catch {
						curPrintRow.Cells["PrintTeamScoreTotal"].Value = "";
					}

					ArrayList curDivList = new ArrayList();
					if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
						if ( divPlcmtButton.Checked ) {
							curDivList.Add( (String) curTeamRow["Div"] );
						}
						if ( groupPlcmtButton.Checked ) {
							curDivList.Add( (String) curTeamRow["Div"] );
						}
					} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
						curDivList.Add( "CM" );
						curDivList.Add( "CW" );
					} else if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
						curDivList.Add( "Cat1" );
						curDivList.Add( "Cat2" );
						curDivList.Add( "Cat3" );

						awsaTeamTotal = new decimal[4] { 0, 0, 0, 0 };

					} else {
						curDivList.Add( "" );
					}

					String curGroupAttrName = "";
					bool curInsertNewRow = false;
					foreach ( String curDivShow in curDivList ) {
                        if ( plcmtAWSATeamButton.Checked && Category1Button.Checked && !curDivShow.Equals( "Cat1" ) ) continue;
						if ( plcmtAWSATeamButton.Checked && Category2Button.Checked && !curDivShow.Equals( "Cat1" ) ) continue;
						if ( plcmtAWSATeamButton.Checked && Category3Button.Checked && !curDivShow.Equals( "Cat3" ) ) continue;

						if ( curInsertNewRow ) {
							curPrintIdx = PrintDataGridView.Rows.Add();
							curPrintRow = PrintDataGridView.Rows[curPrintIdx];
							curPrintRow.DefaultCellStyle.BackColor = Color.DarkGray;
							PrintDataGridView.Rows[curPrintIdx].Height = 8;
						}
						curInsertNewRow = true;

						if ( divPlcmtButton.Checked ) curGroupAttrName = "AgeGroup";
						if ( groupPlcmtButton.Checked || plcmtAWSATeamButton.Checked ) curGroupAttrName = "EventGroup";

						if (EventAllButton.Checked || EventSlalomButton.Checked ) {
							curFilterCmd = "TeamSlalom='" + curTeamCode + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curSlalomTeamSkierList = curSlalomDataTable.Select( curFilterCmd );

						} else {
							curSlalomTeamSkierList = new DataRow[0];
						}

						if ( EventAllButton.Checked || EventTrickButton.Checked ) {
							curFilterCmd = "TeamTrick='" + curTeamCode + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curTrickTeamSkierList = curTrickDataTable.Select( curFilterCmd );

						} else {
							curTrickTeamSkierList = new DataRow[0];
						}

						if ( EventAllButton.Checked || EventJumpButton.Checked) {
							curFilterCmd = "TeamJump='" + curTeamCode + "'";
							if ( curGroupAttrName.Length > 0 ) {
								curFilterCmd += " AND " + curGroupAttrName + " = '" + curDivShow + "'";
							} else if ( myTourRules.ToLower().Equals( "ncwsa" ) && plcmtTourButton.Checked ) {
								curFilterCmd += " AND AgeGroup = '" + curDivShow + "'";
							}
							curJumpTeamSkierList = curJumpDataTable.Select( curFilterCmd );

						} else {
							curJumpTeamSkierList = new DataRow[0];
						}

						curSkierIdxMax = 0;
						if ( curSlalomTeamSkierList.Length > curTrickTeamSkierList.Length ) {
							if ( curSlalomTeamSkierList.Length > curJumpTeamSkierList.Length ) {
								curSkierIdxMax = curSlalomTeamSkierList.Length;
							} else {
								curSkierIdxMax = curJumpTeamSkierList.Length;
							}
						} else {
							if ( curTrickTeamSkierList.Length > curJumpTeamSkierList.Length ) {
								curSkierIdxMax = curTrickTeamSkierList.Length;
							} else {
								curSkierIdxMax = curJumpTeamSkierList.Length;
							}
						}

						for ( curSkierIdx = 0; curSkierIdx < curSkierIdxMax; curSkierIdx++ ) {
							try {
								if ( myNumPerTeam > curSkierIdx || myNumPerTeam == 0 ) {
									curPrintIdx = PrintDataGridView.Rows.Add();
									curPrintRow = PrintDataGridView.Rows[curPrintIdx];
									if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
										curPrintRow.Cells["PrintSkierCategory"].Value = curDivShow;
									} else {
										curPrintRow.Cells["PrintSkierCategory"].Value = "";
									}

									if ( curSlalomTeamSkierList.Length > curSkierIdx ) {
										if ( EventAllButton.Checked || EventSlalomButton.Checked ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curSlalomTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0" );
											curPrintRow.Cells["PrintScoreSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" );
											curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"];
											curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxSlalom] += (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
											}

										}
									}


									if ( curTrickTeamSkierList.Length > curSkierIdx) {
										if ( EventAllButton.Checked || EventTrickButton.Checked ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curTrickTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0" );
											curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
											if ( curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType( "System.Int32" ) ) {
												curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int32) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
											} else {
												curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int16) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
											}
											curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"];

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxTrick] += (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
											}
										}
									}

									if ( curJumpTeamSkierList.Length > curSkierIdx) {
										if ( EventAllButton.Checked || EventJumpButton.Checked ) {
											if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
												curPrintRow.Cells["PrintSkierNameJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curJumpTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
											} else {
												curPrintRow.Cells["PrintSkierNameJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["SkierName"];
											}
											curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0" );
											curPrintRow.Cells["PrintPlcmtPointsJump"].Value = (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
											curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["PlcmtJump"];
											if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
												PrintScoreJump.HeaderText = "Mtrs";
												curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString( "##0.0" );
											} else {
												PrintScoreJump.HeaderText = "Feet";
												curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString( "##0" );
											}

											if ( myTourRules.ToLower().Equals( "awsa" ) && plcmtAWSATeamButton.Checked ) {
												awsaTeamTotal[scoreIdxJump] += (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
												awsaTeamTotal[scoreIdxTotal] += (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
											}
										}
									}

								} else {
									if ( myNumPerTeam == curSkierIdx ) {
										curPrintIdx = PrintDataGridView.Rows.Add();
										curPrintRow = PrintDataGridView.Rows[curPrintIdx];
										curPrintRow.DefaultCellStyle.BackColor = Color.LightGray;
										PrintDataGridView.Rows[curPrintIdx].Height = 4;
									}
									curPrintIdx = PrintDataGridView.Rows.Add();
									curPrintRow = PrintDataGridView.Rows[curPrintIdx];

									if ( curSlalomTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameSlalom"].Value = "* " + (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curSlalomTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameSlalom"].Value = "* " + (String) curSlalomTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["PointsSlalom"] ).ToString( "###0" );
										curPrintRow.Cells["PrintScoreSlalom"].Value = ( (Decimal) curSlalomTeamSkierList[curSkierIdx]["ScoreSlalom"] ).ToString( "##0.00" );
										curPrintRow.Cells["PrintPlcmtSlalom"].Value = (String) curSlalomTeamSkierList[curSkierIdx]["PlcmtSlalom"];
										curPrintRow.Cells["PrintPlcmtPointsSlalom"].Value = (Decimal) curSlalomTeamSkierList[curSkierIdx]["PlcmtPointsSlalom"];
									}
									if ( curTrickTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameTrick"].Value = "* " + (String) curTrickTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curTrickTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameTrick"].Value = "* " + (String) curTrickTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsTrick"].Value = ( (Decimal) curTrickTeamSkierList[curSkierIdx]["PointsTrick"] ).ToString( "###0" );
										curPrintRow.Cells["PrintPlcmtPointsTrick"].Value = (Decimal) curTrickTeamSkierList[curSkierIdx]["PlcmtPointsTrick"];
										if ( curTrickTeamSkierList[curSkierIdx]["ScoreTrick"].GetType() == System.Type.GetType( "System.Int32" ) ) {
											curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int32) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
										} else {
											curPrintRow.Cells["PrintScoreTrick"].Value = ( (Int16) curTrickTeamSkierList[curSkierIdx]["ScoreTrick"] ).ToString( "##,##0" );
										}
										curPrintRow.Cells["PrintPlcmtTrick"].Value = (String) curTrickTeamSkierList[curSkierIdx]["PlcmtTrick"];
									}
									if ( curJumpTeamSkierList.Length > curSkierIdx ) {
										if ( curDivShow.Length > 0 || plcmtAWSATeamButton.Checked ) {
											curPrintRow.Cells["PrintSkierNameJump"].Value = "* " + (String) curJumpTeamSkierList[curSkierIdx]["SkierName"] + " (" + (String) curJumpTeamSkierList[curSkierIdx]["AgeGroup"] + ")";
										} else {
											curPrintRow.Cells["PrintSkierNameJump"].Value = "* " + (String) curJumpTeamSkierList[curSkierIdx]["SkierName"];
										}
										curPrintRow.Cells["PrintPointsJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["PointsJump"] ).ToString( "###0" );
										curPrintRow.Cells["PrintPlcmtPointsJump"].Value = (Decimal) curJumpTeamSkierList[curSkierIdx]["PlcmtPointsJump"];
										curPrintRow.Cells["PrintPlcmtJump"].Value = (String) curJumpTeamSkierList[curSkierIdx]["PlcmtJump"];
										if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
											PrintScoreJump.HeaderText = "Mtrs";
											curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreMeters"] ).ToString( "##0.0" );
										} else {
											PrintScoreJump.HeaderText = "Feet";
											curPrintRow.Cells["PrintScoreJump"].Value = ( (Decimal) curJumpTeamSkierList[curSkierIdx]["ScoreFeet"] ).ToString( "##0" );
										}
									}
								}
							} catch ( Exception ex ) {
								MessageBox.Show( "Exception processing SkierIdx: " + curSkierIdx + " Message: " + ex.Message );

							}
						}
					}

					curPrintIdx = PrintDataGridView.Rows.Add();
					curPrintRow = PrintDataGridView.Rows[curPrintIdx];
					curPrintRow.DefaultCellStyle.BackColor = Color.MediumBlue;
					PrintDataGridView.Rows[curPrintIdx].Height = 8;
					prevDiv = curDiv;

					if ( CategoryAllButton.Checked ) {
						if ( EventAllButton.Checked ) {

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
                            TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category1Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category2Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}

					} else if ( Category3Button.Checked ) {
						if ( EventAllButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = (
								awsaTeamTotal[scoreIdxSlalom] + awsaTeamTotal[scoreIdxTrick] + awsaTeamTotal[scoreIdxJump]
								).ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

						} else if ( EventSlalomButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxSlalom].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventTrickButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxTrick].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = "";

						} else if ( EventJumpButton.Checked ) {
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintPlcmtPointsJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							PrintDataGridView.Rows[curTeamSummaryRowIdx].Cells["PrintTeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );

							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreJump"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTotal"].Value = awsaTeamTotal[scoreIdxJump].ToString( "##,##0" );
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreTrick"].Value = "";
							TeamSummaryDataGridView.Rows[curTeamSummarySummaryRowIdx].Cells["TeamScoreSlalom"].Value = "";
						}
					}

				} catch ( Exception ex ) {
					MessageBox.Show( "Exception processing TeamDataTable: " + ex.Message );

				}
			}
		}

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( PrintDataGridView );
        }

        private void navLiveWeb_Click( object sender, EventArgs e ) {
            int curRowCount = 0;
			StringBuilder curSqlStmtInsert = new StringBuilder( "" );
			StringBuilder curSqlStmt = new StringBuilder( "" );
            String curReportFormat, curTeamName = "", curSlalomSkierName = "", curTrickSkierName = "", curJumpSkierName = "";

			if ( !( LiveWebHandler.LiveWebMessageHandlerActive ) ) LiveWebHandler.connectLiveWebHandler( mySanctionNum );
			if ( !( LiveWebHandler.LiveWebMessageHandlerActive ) ) {
				MessageBox.Show( "Request to publish running order but live web not successfully connected." );
				return;
			}

			curSqlStmt = new StringBuilder( String.Format("Delete From TeamScore Where SanctionId = '{0}'", mySanctionNum) );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			curSqlStmt = new StringBuilder( String.Format( "Delete From TeamScoreDetail Where SanctionId = '{0}'", mySanctionNum ) );
			DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			curSqlStmtInsert.Append( "INSERT INTO TeamScore ( " );
			curSqlStmtInsert.Append( "SanctionId, TeamCode, AgeGroup, Name, ReportFormat, OverallPlcmt, SlalomPlcmt, TrickPlcmt, JumpPlcmt, OverallScore, SlalomScore, TrickScore, JumpScore, LastUpdateDate " );
			curSqlStmtInsert.Append( ") VALUES ( " );
            String curSqlInsertData = "'{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', {9}, {10}, {11}, {12}, GetDate() )";
			
            foreach ( DataGridViewRow curViewRow in TeamSummaryDataGridView.Rows ) {
                curTeamName = HelperFunctions.getViewRowColValue( curViewRow, "TeamCode", "" );
                if ( HelperFunctions.isObjectEmpty( curTeamName ) ) continue;

				curRowCount++;
                curReportFormat = "";
				if ( myTourRules.ToLower().Equals( "awsa" ) ) {
					if ( plcmtAWSATeamButton.Checked ) {
						curReportFormat = "awsa";
					} else {
						curReportFormat = "std";
					}
				} else if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
					curReportFormat = "ncwsa";
				} else if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
					curReportFormat = "iwwf";
				}

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( curSqlStmtInsert );
                curSqlStmt.Append( String.Format( curSqlInsertData, mySanctionNum
                    , HelperFunctions.getViewRowColValue( curViewRow, "TeamCode", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "TeamDiv", "" )
					, HelperFunctions.escapeString( HelperFunctions.getViewRowColValue( curViewRow, "TeamName", "" ))
					, curReportFormat
					, HelperFunctions.getViewRowColValue( curViewRow, "TeamPlcmtOverall", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "TeamPlcmtSlalom", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "TeamPlcmtTrick", "" )
					, HelperFunctions.getViewRowColValue( curViewRow, "TeamPlcmtJump", "" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "TeamScoreTotal", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "TeamScoreSlalom", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "TeamScoreTrick", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "TeamScoreJump", "" ).ToString( "###0.0" )
				) ) ;

                DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}

			curSqlStmtInsert = new StringBuilder( "" );
			curSqlStmtInsert.Append( "INSERT INTO TeamScoreDetail ( " );
			curSqlStmtInsert.Append( "SanctionId, TeamCode, AgeGroup, LineNum, SkierCategory"
				+ ", SlalomSkierName, SlalomPlcmt, SlalomScore, SlalomNops, SlalomPoints"
				+ ", TrickSkierName, TrickPlcmt, TrickScore, TrickNops, TrickPoints"
				+ ", JumpSkierName, JumpPlcmt, JumpScore, JumpNops, JumpPoints" );
			curSqlStmtInsert.Append( ") VALUES ( " );

			curRowCount = 0;
            curTeamName = "";
            curSlalomSkierName = "";
            curTrickSkierName = "";
            curJumpSkierName = "";
            String curActiveTeamName = "", curAgeGroup = "", curActiveAgeGroup = "";

            int curSkierLineNum = 0;
            foreach ( DataGridViewRow curViewRow in PrintDataGridView.Rows ) {
                curTeamName = HelperFunctions.getViewRowColValue( curViewRow, "PrintTeam", "" );
                curAgeGroup = HelperFunctions.getViewRowColValue( curViewRow, "PrintTeamDiv", "" );
                curSlalomSkierName = HelperFunctions.getViewRowColValue( curViewRow, "PrintSkierNameSlalom", "" );
				curTrickSkierName = HelperFunctions.getViewRowColValue( curViewRow, "PrintSkierNameTrick", "" );
				curJumpSkierName = HelperFunctions.getViewRowColValue( curViewRow, "PrintSkierNameJump", "" );

                if ( curTeamName.Length == 0
                    && curSlalomSkierName.Length == 0
                    && curTrickSkierName.Length == 0
                    && curJumpSkierName.Length == 0
                    ) continue;
                
                if ( curTeamName.Length > 0
                    && curSlalomSkierName.Length == 0
                    && curTrickSkierName.Length == 0
                    && curJumpSkierName.Length == 0 
                    ) {
                    curSkierLineNum = 0;
                    curActiveTeamName = curTeamName.Substring(0, curTeamName.IndexOf("-"));
                    curActiveAgeGroup = curAgeGroup;
                    continue;
                }

				curRowCount++;
				curSkierLineNum++;

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( curSqlStmtInsert );
                curSqlStmt.Append( String.Format( "'{0}', '{1}', '{2}', {3}, '{4}'"
                    , mySanctionNum, curActiveTeamName, curActiveAgeGroup, curSkierLineNum, HelperFunctions.getViewRowColValue( curViewRow, "PrintSkierCategory", "" ) ) );

                if ( curSlalomSkierName.Length > 0 ) {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4}"
                    , HelperFunctions.escapeString( curSlalomSkierName ) 
                    , HelperFunctions.getViewRowColValue( curViewRow, "PrintPlcmtSlalom", "" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintScoreSlalom", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPointsSlalom", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPlcmtPointsSlalom", "" ).ToString( "###0.0" )
					) );

                } else {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4}", "", "", 0, 0, 0 ) );
				}

				if ( curTrickSkierName.Length > 0 ) {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4}"
					, HelperFunctions.escapeString( curTrickSkierName )
					, HelperFunctions.getViewRowColValue( curViewRow, "PrintPlcmtTrick", "" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintScoreTrick", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPointsTrick", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPlcmtPointsTrick", "" ).ToString( "###0.0" )
					) );

				} else {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4}", "", "", 0, 0, 0 ) );
				}

				if ( curJumpSkierName.Length > 0 ) {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4} )"
					, HelperFunctions.escapeString( curJumpSkierName )
					, HelperFunctions.getViewRowColValue( curViewRow, "PrintPlcmtJump", "" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintScoreJump", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPointsJump", "" ).ToString( "###0.0" )
					, HelperFunctions.getViewRowColValueDecimal( curViewRow, "PrintPlcmtPointsJump", "" ).ToString( "###0.0" )
					) );

				} else {
					curSqlStmt.Append( String.Format( ", '{0}', '{1}', {2}, {3}, {4} )", "", "", 0, 0, 0 ) );
				}
				DataAccess.ExecuteCommand( curSqlStmt.ToString() );
            }

			LiveWebHandler.sendTeamScore( mySanctionNum );

		}

		private void ShowTeamButton_Click( object sender, EventArgs e ) {
            myShowTeam = true;
            String curFilterCmd = "", curTeamCode = "", curGroup = "";
            if ( TeamSummaryDataGridView.Rows.Count > 0 ) {
                if ( TeamSummaryDataGridView.CurrentRow == null ) {
                    curTeamCode = TeamSummaryDataGridView.Rows[0].Cells["TeamCode"].Value.ToString();
                    if ( TeamDiv.Visible && (divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                        curGroup = TeamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                    }
                } else {
                    curTeamCode = TeamSummaryDataGridView.CurrentRow.Cells["TeamCode"].Value.ToString();
                    if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                        curGroup = TeamSummaryDataGridView.Rows[0].Cells["TeamDiv"].Value.ToString();
                    }
                }
                if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamSlalom = '" + curTeamCode + "'";
                }
                mySlalomDataTable.DefaultView.RowFilter = curFilterCmd;
                loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );

                if (TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamTrick = '" + curTeamCode + "'";
                }
                myTrickDataTable.DefaultView.RowFilter = curFilterCmd;
                loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );

                if (TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                    if (divPlcmtButton.Checked) {
                        curFilterCmd = "TeamJump = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                    }
                    if (groupPlcmtButton.Checked) {
                        curFilterCmd = "TeamJump = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                    }
                } else {
                    curFilterCmd = "TeamJump = '" + curTeamCode + "'";
                }
                myJumpDataTable.DefaultView.RowFilter = curFilterCmd;
                loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
            }
        }

        private void ShowAllButton_Click( object sender, EventArgs e ) {
            myShowTeam = false;
            mySlalomDataTable.DefaultView.RowFilter = "";
            loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );
            myTrickDataTable.DefaultView.RowFilter = "";
            loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );
            myJumpDataTable.DefaultView.RowFilter = "";
            loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
        }

        private void TeamSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( TeamSummaryDataGridView.RowCount > 0 ) {
                if ( myShowTeam ) {
                    if ( TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value != null ) {
                        String curFilterCmd = "", curTeamCode = "", curGroup = "";
                        curTeamCode = TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value.ToString();
                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            curGroup = TeamSummaryDataGridView.Rows[e.RowIndex].Cells["TeamDiv"].Value.ToString();
                        }
                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamSlalom = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamSlalom = '" + curTeamCode + "'";
                        }
                        mySlalomDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadSlalomDataGrid( mySlalomDataTable.DefaultView.ToTable() );

                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamTrick = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamTrick = '" + curTeamCode + "'";
                        }
                        myTrickDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadTrickDataGrid( myTrickDataTable.DefaultView.ToTable() );

                        if ( TeamDiv.Visible && ( divPlcmtButton.Checked || groupPlcmtButton.Checked ) ) {
                            if ( divPlcmtButton.Checked) {
                                curFilterCmd = "TeamJump = '" + curTeamCode + "' AND AgeGroup = '" + curGroup + "'";
                            }
                            if (groupPlcmtButton.Checked) {
                                curFilterCmd = "TeamJump = '" + curTeamCode + "' AND EventGroup = '" + curGroup + "'";
                            }
                        } else {
                            curFilterCmd = "TeamJump = '" + curTeamCode + "'";
                        }
                        myJumpDataTable.DefaultView.RowFilter = curFilterCmd;
                        loadJumpDataGrid( myJumpDataTable.DefaultView.ToTable() );
                    
                    }
                }
            }
        }

        private void NumPerTeamTextBox_TextChanged( object sender, EventArgs e ) {
            bool isConvertComplete = Int16.TryParse( NumPerTeamTextBox.Text, out myNumPerTeam );
            if ( isConvertComplete ) {
                myTourProperties.TeamSummary_NumPerTeam = myNumPerTeam.ToString();

            } else {
                MessageBox.Show( "Invalid value in textbox.  Must be a proper integer value." );
                myNumPerTeam = Convert.ToInt16( myTourProperties.TeamSummary_NumPerTeam );
            }
        }

        private void Button_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curButton = (RadioButton)sender;
            if ( curButton.Checked ) {
                if ( curButton.Name.Equals( "handicapPointsButton" ) || curButton.Name.Equals( "ratioPointsButton" ) ) {
                    HCapBaseSlalom.Visible = true;
                    HCapScoreSlalom.Visible = true;
                    HCapBaseTrick.Visible = true;
                    HCapScoreTrick.Visible = true;
                    HCapBaseJump.Visible = true;
                    HCapScoreJump.Visible = true;
                }
            } else {
                if ( curButton.Name.Equals( "handicapPointsButton" ) || curButton.Name.Equals( "ratioPointsButton" ) ) {
                    HCapBaseSlalom.Visible = false;
                    HCapScoreSlalom.Visible = false;
                    HCapBaseTrick.Visible = false;
                    HCapScoreTrick.Visible = false;
                    HCapBaseJump.Visible = false;
                    HCapScoreJump.Visible = false;
                }
            }
        }

        private void groupPlcmtButton_CheckedChanged( object sender, EventArgs e ) {
            if ( divPlcmtButton.Checked ) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Div";
                PrintTeamDiv.HeaderText = "Div";
                EventGroupSlalom.Visible = true;
                EventGroupTrick.Visible = true;
                EventGroupJump.Visible = true;
                EventGroupList.Visible = true;
                EventGroupListLabel.Visible = true;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
            if (groupPlcmtButton.Checked) {
                TeamDiv.Visible = true;
                TeamDiv.HeaderText = "Group";
                PrintTeamDiv.HeaderText = "Group";
                EventGroupSlalom.Visible = true;
                EventGroupTrick.Visible = true;
                EventGroupJump.Visible = true;
                EventGroupList.Visible = true;
                EventGroupListLabel.Visible = true;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
        }

        private void ViewReportButton_Click(object sender, EventArgs e) {
            if ( PrintDataGridView.Visible ) {
                PrintDataGridView.Visible = false;
                PrintDataGridView.Enabled = false;

				ViewReportButton.Text = "View Team Results";
            } else {
				PrintDataGridView.Visible = true;
                PrintDataGridView.Enabled = true;
                PrintDataGridView.Location = TeamSummaryDataGridView.Location;
                PrintDataGridView.Width = this.Width - 25;
                PrintDataGridView.Height = TeamSummaryDataGridView.Height;
                ViewReportButton.Text = "Hide Team Results";
            }
        }

		private void FilterReportButton_Click( object sender, EventArgs e ) {
			loadPrintDataGrid();
		}

		private void plcmtTourButton_CheckedChanged( object sender, EventArgs e ) {
            if ( plcmtTourButton.Checked ) {
                TeamDiv.Visible = false;
                EventGroupSlalom.Visible = false;
                EventGroupTrick.Visible = false;
                EventGroupJump.Visible = false;
                EventGroupList.Visible = false;
                EventGroupListLabel.Visible = false;
                PrintPlcmtPointsSlalom.Visible = false;
                PrintPlcmtPointsTrick.Visible = false;
                PrintPlcmtPointsJump.Visible = false;
                PlcmtPointsSlalom.Visible = false;
                PlcmtPointsTrick.Visible = false;
                PlcmtPointsJump.Visible = false;
            }
        }

        private void plcmtAwsaTeamButton_CheckedChanged( object sender, EventArgs e ) {
            if ( plcmtAWSATeamButton.Checked ) {
                TeamDiv.Visible = true;
                EventGroupSlalom.Visible = false;
                EventGroupTrick.Visible = false;
                EventGroupJump.Visible = false;
                EventGroupList.Visible = false;
                EventGroupListLabel.Visible = false;
                PrintPlcmtPointsSlalom.Visible = true;
                PrintPlcmtPointsTrick.Visible = true;
                PrintPlcmtPointsJump.Visible = true;
                PlcmtPointsSlalom.Visible = true;
                PlcmtPointsTrick.Visible = true;
                PlcmtPointsJump.Visible = true;

				CategoryGroupBox.Visible = true;
				CategoryGroupBox.Enabled = true;
				EventGroupBox.Visible = true;
				EventGroupBox.Enabled = true;

				EventGroupList.Visible = false;
				EventGroupList.Enabled = false;
				EventGroupListLabel.Visible = false;
				EventGroupListLabel.Enabled = false;

				FilterReportButton.Visible = true;
				FilterReportButton.Enabled = true;

			} else {
				CategoryGroupBox.Visible = false;
                CategoryGroupBox.Enabled = false;
				EventGroupBox.Visible = false;
				EventGroupBox.Enabled = false;

				EventGroupList.Visible = true;
				EventGroupList.Enabled = true;
				EventGroupListLabel.Visible = true;
				EventGroupListLabel.Enabled = true;

				FilterReportButton.Visible = false;
				FilterReportButton.Enabled = false;
			}
		}

		private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight( 15, '*' );
            printFooter.PadLeft( 15, '*' );
            myExportData.exportDataAsHtml( PrintDataGridView, printTitle, printSubtitle, printFooter );
        }

        private void navPublish_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( publishReportTimer );
            curTimerObj.Start();
        }
        private void navPrint_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }

        private void publishReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( publishReportTimer );
            if ( printReport( true ) ) ExportLiveWeb.uploadReportFile( "Results", "Overall", mySanctionNum );
        }
        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printReport( false );
        }
        private bool printReport( bool inPublish ) {
			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            Font saveShowDefaultCellStyle = TeamSummaryDataGridView.DefaultCellStyle.Font;
            //TeamSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );
            TeamSummaryDataGridView.RowTemplate.Height = 28;

			bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;

            String curReportTitle = this.Text;
            StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
            printTitle.Append( "\n Sanction " + mySanctionNum );
            printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
            printTitle.Append( "\n" + curReportTitle );
            if ( inPublish ) printTitle.Append( HelperFunctions.buildPublishReportTitle( mySanctionNum ) );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = curReportTitle + "-Team";
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = true;
            myPrintDataGrid = new DataGridViewPrinter( TeamSummaryDataGridView, myPrintDoc,
                CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
            curPreviewDialog.Document = myPrintDoc;
            curPreviewDialog.ShowDialog();
            if ( inPublish ) ExportLiveWeb.uploadReportFile( "Results", "Overall", mySanctionNum );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = curReportTitle + "-Detail";
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = true;
            myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
                CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
            curPreviewDialog.Document = myPrintDoc;
            curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();

            TeamSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;
            TeamSummaryDataGridView.RowTemplate.Height = 28;
			return true;
		}

		// The PrintPage action for the PrintDocument control
		private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataRow getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable != null && curDataTable.Rows.Count > 0 ) return curDataTable.Rows[0];
			return null;
		}

		private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

	}
}
