﻿using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Slalom {
	public partial class SlalomSummary : Form {
		private String mySanctionNum = null;
		private String myPageBreak = "\\n";
		private String myEvent = "Slalom";
		private String mySortCommand = "";
		private String myFilterCmd = "";
		private String myTourRules = "";
		private SortDialogForm sortDialogForm;
		private FilterDialogForm filterDialogForm;
		private DataRow myTourRow;
		private DataTable mySummaryDataTable;
		private PrintDocument myPrintDoc;
		private DataGridViewPrinter myPrintDataGrid;
		private TourProperties myTourProperties;

		public SlalomSummary() {
			InitializeComponent();
		}

		private void SlalomSummary_Load( object sender, EventArgs e ) {
			if ( Properties.Settings.Default.SlalomScoreSummary_Width > 0 ) {
				this.Width = Properties.Settings.Default.SlalomScoreSummary_Width;
			}
			if ( Properties.Settings.Default.SlalomScoreSummary_Height > 0 ) {
				this.Height = Properties.Settings.Default.SlalomScoreSummary_Height;
			}
			if ( Properties.Settings.Default.SlalomScoreSummary_Location.X > 0
				&& Properties.Settings.Default.SlalomScoreSummary_Location.Y > 0 ) {
				this.Location = Properties.Settings.Default.SlalomScoreSummary_Location;
			}
			myTourProperties = TourProperties.Instance;

			bestScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "best" ) ) bestScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "total" ) ) totalScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "final" ) ) finalScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "first" ) ) firstScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "round" ) ) roundScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryDataType.ToLower().Equals( "h2h" ) ) h2hScoreButton.Checked = true;

			rawScoreButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPlcmtMethod.ToLower().Equals( "points" ) ) pointsScoreButton.Checked = true;
			groupPlcmtButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "tour" ) ) plcmtTourButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "div" ) ) plcmtDivButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPlcmtOrg.ToLower().Equals( "divgr" ) ) plcmtDivGrpButton.Checked = true;
			nopsPointsButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "nops" ) ) nopsPointsButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "plcmt" ) ) plcmtPointsButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "kbase" ) ) kBasePointsButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "hcap" ) ) handicapPointsButton.Checked = true;
			if ( myTourProperties.SlalomSummaryPointsMethod.ToLower().Equals( "ratio" ) ) ratioPointsButton.Checked = true;
			numPrelimTextBox.Text = myTourProperties.SlalomSummaryNumPrelim;

			String[] curList = { "MemberId", "SkierName", "City", "State", "SkiYearAge", "AgeGroup", "EventGroup", "Event"
					, "EventClassSlalom", "PlcmtSlalom", "TeamSlalom", "HCapBaseSlalom"
					, "RoundSlalom", "ScoreSlalom", "PointsSlalom"
					, "FinalSpeedMph", "FinalSpeedKph", "FinalLen", "FinalLenOff", "FinalPassScore" };
			sortDialogForm = new SortDialogForm();
			sortDialogForm.ColumnListArray = curList;

			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnListArray = curList;

			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;

			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
			} else {
				if ( mySanctionNum.Length < 6 ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				} else {
					//Retrieve selected tournament attributes
					DataTable curTourDataTable = getTourData();
					if ( curTourDataTable.Rows.Count > 0 ) {
						myTourRow = curTourDataTable.Rows[0];
						myTourRules = (String)myTourRow["Rules"];

						if ( myTourProperties.SlalomScoreSummary_Sort.Length > 0 ) {
							mySortCommand = myTourProperties.SlalomScoreSummary_Sort;
						} else {
							mySortCommand = "";
						}
						if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
							Round.Visible = false;
							EventClass.Visible = false;
							TeamCodeNcwsa.Visible = true;
							TeamCode.Visible = false;
						} else {
							TeamCodeNcwsa.Visible = false;
							TeamCode.Visible = true;
						}

						loadGroupList();
					}
				}
			}
		}

		public void navRefresh_Click( object sender, EventArgs e ) {
			// Retrieve data from database
			if ( mySanctionNum != null && myEvent != null ) {
				String curDataType = "all", curPlcmtMethod = "score";
				String curPlcmtOrg, curPointsMethod;
				Int16 curNumPrelimRounds = 0;
				CalcScoreSummary curCalcSummary = new CalcScoreSummary();

				// Retrieve data from database depending on selection criteria
				String curMsg = "Tournament scores retrieved ";

				if ( bestScoreButton.Checked ) {
					curDataType = "best";
					winStatusMsg.Text = curMsg + "- best scores ";
				} else if ( totalScoreButton.Checked ) {
					curDataType = "total";
					winStatusMsg.Text = curMsg + "- total scores";
				} else if ( finalScoreButton.Checked ) {
					curDataType = "final";
					winStatusMsg.Text = curMsg + "- final scores";
				} else if ( firstScoreButton.Checked ) {
					curDataType = "first";
					winStatusMsg.Text = curMsg + "- first scores";
				} else if ( roundScoreButton.Checked ) {
					curDataType = "round";
					winStatusMsg.Text = curMsg + "- all scores";
				} else if ( h2hScoreButton.Checked ) {
					curDataType = "h2h";
					winStatusMsg.Text = curMsg + "- head to head";
				} else {
					curDataType = "best";
					winStatusMsg.Text = curMsg + "- best scores ";
				}
				myTourProperties.SlalomSummaryDataType = curDataType;

				if ( rawScoreButton.Checked ) {
					curPlcmtMethod = "score";
				} else if ( pointsScoreButton.Checked ) {
					curPlcmtMethod = "points";
				} else {
					curPlcmtMethod = "score";
				}
				myTourProperties.SlalomSummaryPlcmtMethod = curPlcmtMethod;

				if ( groupPlcmtButton.Checked ) {
					curPlcmtOrg = "group";
					EventGroup.Visible = true;
				} else if ( plcmtTourButton.Checked ) {
					curPlcmtOrg = "tour";
					EventGroup.Visible = false;
				} else if ( plcmtDivButton.Checked ) {
					curPlcmtOrg = "div";
					EventGroup.Visible = false;
				} else if ( plcmtDivGrpButton.Checked ) {
					curPlcmtOrg = "divgr";
					EventGroup.Visible = true;
				} else {
					curPlcmtOrg = "tour";
					EventGroup.Visible = false;
				}
				myTourProperties.SlalomSummaryPlcmtOrg = curPlcmtOrg;

				HCapBase.Visible = false;
				HCapScore.Visible = false;
				if ( nopsPointsButton.Checked ) {
					curPointsMethod = "nops";
				} else if ( plcmtPointsButton.Checked ) {
					curPointsMethod = "plcmt";
				} else if ( kBasePointsButton.Checked ) {
					curPointsMethod = "kbase";
				} else if ( handicapPointsButton.Checked ) {
					curPointsMethod = "hcap";
					HCapBase.Visible = true;
					HCapScore.Visible = true;
				} else if ( ratioPointsButton.Checked ) {
					curPointsMethod = "ratio";
					HCapBase.Visible = true;
					HCapScore.Visible = true;
				} else {
					curPointsMethod = "nops";
				}
				myTourProperties.SlalomSummaryPointsMethod = curPointsMethod;

				if ( myTourRules.ToLower().Equals( "iwwf" ) && kBasePointsButton.Checked ) {
					HCapBase.Visible = false;
					HCapScore.Visible = false;
				}

				if ( myTourRules.ToLower().Equals( "ncwsa" )
					&& plcmtPointsButton.Checked
					&& pointsScoreButton.Checked
					) {
					rawScoreButton.Checked = true;
					curPlcmtMethod = "score";
					myTourProperties.SlalomSummaryPlcmtMethod = curPlcmtMethod;
				}

				if ( h2hScoreButton.Checked || finalScoreButton.Checked ) {
					if ( numPrelimTextBox.Text.Length == 0 ) {
						MessageBox.Show( "Number of preliminary rounds is required for Head to Head reporting" );
						return;
					} else {
						curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
                        myTourProperties.SlalomSummaryNumPrelim = numPrelimTextBox.Text;

                    }
				}

				String curGroupValue = "";
				try {
					curGroupValue = EventGroupList.SelectedItem.ToString();
					if ( !( curGroupValue.ToLower().Equals( "all" ) ) ) {
						if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
							if ( curGroupValue.ToUpper().Equals( "MEN A" ) ) {
								curGroupValue = "CM";
							} else if ( curGroupValue.ToUpper().Equals( "WOMEN A" ) ) {
								curGroupValue = "CW";
							} else if ( curGroupValue.ToUpper().Equals( "MEN B" ) ) {
								curGroupValue = "BM";
							} else if ( curGroupValue.ToUpper().Equals( "WOMEN B" ) ) {
								curGroupValue = "BW";
							} else {
								curGroupValue = "All";
							}
						}
					}
				} catch {
					curGroupValue = "All";
				}

				this.Cursor = Cursors.WaitCursor;
				if ( curGroupValue.ToLower().Equals( "all" ) ) {
					if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
						curPointsMethod = "kbase";
						mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, null, null, curNumPrelimRounds );
					} else if ( h2hScoreButton.Checked || ( finalScoreButton.Checked && curNumPrelimRounds > 0 ) ) {
						mySummaryDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, curNumPrelimRounds );
					} else {
						mySummaryDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod );
					}
				} else {
					if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
						curPointsMethod = "kbase";
						mySummaryDataTable = curCalcSummary.CalcIwwfEventPlcmts( myTourRow, mySanctionNum, "Slalom", myTourRules, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue, curNumPrelimRounds );
					} else if ( h2hScoreButton.Checked || ( finalScoreButton.Checked && curNumPrelimRounds > 0 ) ) {
						mySummaryDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue, curNumPrelimRounds );
					} else {
						if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
							mySummaryDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
						} else {
							mySummaryDataTable = curCalcSummary.getSlalomSummary( myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, "All", curGroupValue );
						}
					}
				}

				if ( mySummaryDataTable.Rows.Count > 0 ) {
					if ( h2hScoreButton.Checked && curNumPrelimRounds > 0 ) {
						mySummaryDataTable = curCalcSummary.buildHeadToHeadSummary( mySummaryDataTable, myTourRow, curDataType, curPlcmtMethod, curPlcmtOrg, curPointsMethod, curNumPrelimRounds, "Slalom" );
					} else {
						if ( mySortCommand.Length > 1 ) {
							if ( myTourRules.ToLower().Equals( "iwwf" ) && curPointsMethod.ToLower().Equals( "kbase" ) ) {
							} else {
								mySummaryDataTable.DefaultView.Sort = mySortCommand;
								mySummaryDataTable = mySummaryDataTable.DefaultView.ToTable();
							}
						}
					}
					loadDataGrid( mySummaryDataTable );
				} else {
					scoreSummaryDataGridView.Rows.Clear();
					MessageBox.Show( "No data available for report" );
				}
			}
			this.Cursor = Cursors.Default;
		}

		private void loadDataGrid( DataTable inDataTable ) {
			Int16 curRound = 0, prevRound = 0;
			String curEventClass = "", curH2hGroup = "";
			String curReportGroup = "", prevReportGroup = "";
			String curDiv = "", curGroup = "";
			Int16 curNumPrelimRounds = 0;
			DataGridViewRow curViewRow;
			scoreSummaryDataGridView.Rows.Clear();

			if ( h2hScoreButton.Checked ) {
				h2hGroup.Visible = true;
			} else {
				h2hGroup.Visible = false;
			}

			try {
				curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
			} catch {
				curNumPrelimRounds = 0;
			}

			if ( inDataTable == null || inDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "No data available for display" );
				return;
			}

			this.Cursor = Cursors.WaitCursor;
			inDataTable.DefaultView.Sort = mySortCommand;
			inDataTable.DefaultView.RowFilter = myFilterCmd;
			DataTable curDataTable = inDataTable.DefaultView.ToTable();

			int curIdx = 0;
			foreach ( DataRow curRow in curDataTable.Rows ) {
				try {
					curEventClass = (String)curRow["EventClassSlalom"];
				} catch {
					curEventClass = "";
				}
				try {
					prevRound = curRound;
					curRound = (Int16)curRow["RoundSlalom"];
				} catch {
					curRound = 0;
				}

				if ( curEventClass.Length > 0 && ( ( curRound > 0 ) || totalScoreButton.Checked ) ) {
					curIdx = scoreSummaryDataGridView.Rows.Add();
					prevReportGroup = curReportGroup;

					#region Determine group, division and any required data breaks
					curGroup = (String)curRow["EventGroup"];
					if ( h2hScoreButton.Checked ) {
						curH2hGroup = (String)curRow["RunOrderGroup"];
					} else {
						curH2hGroup = "";
					}

					if ( ( (String)curRow["AgeGroup"] ).Length > 2 ) {
						curDiv = ( (String)curRow["AgeGroup"] ).Substring( 0, 2 );
					} else {
						curDiv = (String)curRow["AgeGroup"];
					}
					if ( groupPlcmtButton.Checked ) {
						curReportGroup = curGroup;
					} else if ( plcmtTourButton.Checked ) {
						curReportGroup = "";
					} else if ( plcmtDivButton.Checked ) {
						curReportGroup = curDiv;
					} else if ( plcmtDivGrpButton.Checked ) {
						curReportGroup = curDiv + "-" + curGroup;
					} else {
						curReportGroup = curGroup;
					}

					if ( ( curReportGroup != prevReportGroup ) && prevReportGroup.Length > 0 ) {
						if ( roundScoreButton.Checked || h2hScoreButton.Checked || finalScoreButton.Checked ) {
							if ( curRound != prevRound && curIdx > 0 ) {
								curViewRow = scoreSummaryDataGridView.Rows[curIdx];
								if ( ( finalScoreButton.Checked || h2hScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
									curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
									scoreSummaryDataGridView.Rows[curIdx].Height = 4;
									curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
								} else {
									scoreSummaryDataGridView.Rows[curIdx].Height = 2;
									curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
								}
								curIdx = scoreSummaryDataGridView.Rows.Add();

							} else {
								curViewRow = scoreSummaryDataGridView.Rows[curIdx];
								if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
									curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
									scoreSummaryDataGridView.Rows[curIdx].Height = 4;
									curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
								} else if ( ( finalScoreButton.Checked || h2hScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
									if ( curRound != prevRound ) {
										curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
										scoreSummaryDataGridView.Rows[curIdx].Height = 4;
										curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
									} else {
										curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
										scoreSummaryDataGridView.Rows[curIdx].Height = 2;
									}
								}
								curIdx = scoreSummaryDataGridView.Rows.Add();
							}
						} else {
							curViewRow = scoreSummaryDataGridView.Rows[curIdx];
							if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
								curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
								scoreSummaryDataGridView.Rows[curIdx].Height = 4;
								curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
							} else if ( ( finalScoreButton.Checked || h2hScoreButton.Checked ) && prevRound > curNumPrelimRounds ) {
								if ( curRound != prevRound ) {
									curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
									scoreSummaryDataGridView.Rows[curIdx].Height = 2;
									curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
								} else {
									curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
									scoreSummaryDataGridView.Rows[curIdx].Height = 4;
								}
							}
							curIdx = scoreSummaryDataGridView.Rows.Add();
						}
					} else if ( roundScoreButton.Checked || ( ( h2hScoreButton.Checked || finalScoreButton.Checked ) && prevRound > curNumPrelimRounds ) ) {
						if ( curRound != prevRound && curIdx > 0 ) {
							curViewRow = scoreSummaryDataGridView.Rows[curIdx];
							if ( ( finalScoreButton.Checked || h2hScoreButton.Checked ) && prevRound >= curNumPrelimRounds ) {
								curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
								curViewRow.Cells["PlcmtSlalom"].Value = myPageBreak;
								scoreSummaryDataGridView.Rows[curIdx].Height = 4;
							} else {
								curViewRow.DefaultCellStyle.BackColor = Color.AntiqueWhite;
								scoreSummaryDataGridView.Rows[curIdx].Height = 2;
							}
							curIdx = scoreSummaryDataGridView.Rows.Add();
						}
					}
					#endregion

					#region Format report display data
					curViewRow = scoreSummaryDataGridView.Rows[curIdx];
					curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
					curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
					curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];

					try {
						curViewRow.Cells["Hometown"].Value = (String)curRow["City"] + ", " + (String)curRow["State"];
					} catch {
						curViewRow.Cells["Hometown"].Value = "";
						try {
							curViewRow.Cells["Hometown"].Value = (String)curRow["City"];
						} catch {
						}
						try {
							if ( curViewRow.Cells["Hometown"].Value.ToString().Length > 1 ) {
								curViewRow.Cells["Hometown"].Value = ", " + (String)curRow["State"];
							} else {
								curViewRow.Cells["Hometown"].Value = (String)curRow["State"];
							}
						} catch {
						}
					}
					try {
						curViewRow.Cells["SkiYearAge"].Value = ( (Byte)curRow["SkiYearAge"] ).ToString();
					} catch {
						curViewRow.Cells["SkiYearAge"].Value = "";
					}

					curViewRow.Cells["EventGroup"].Value = curGroup;
					curViewRow.Cells["h2hGroup"].Value = curH2hGroup;

					if ( ( (String)curRow["AgeGroup"] ).Length > 2 ) {
						curViewRow.Cells["AgeGroup"].Value = ( (String)curRow["AgeGroup"] ).Substring( 0, 2 );
					} else {
						curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
					}

					curViewRow.Cells["Event"].Value = "Slalom";
					curViewRow.Cells["EventClass"].Value = (String)curRow["EventClassSlalom"];
					if ( ( (String)curRow["PlcmtSlalom"] ).Trim().Equals( "999" ) ) {
						curViewRow.Cells["PlcmtSlalom"].Value = "**";
					} else {
						curViewRow.Cells["PlcmtSlalom"].Value = (String)curRow["PlcmtSlalom"];
					}
					try {
						curViewRow.Cells["TeamCode"].Value = (String)curRow["TeamSlalom"];
						curViewRow.Cells["TeamCodeNcwsa"].Value = (String)curRow["TeamSlalom"];
					} catch {
						curViewRow.Cells["TeamCode"].Value = "";
						curViewRow.Cells["TeamCodeNcwsa"].Value = "";
					}
					try {
						curViewRow.Cells["HCapBase"].Value = ( (Decimal)curRow["HCapBaseSlalom"] ).ToString( "###0.0" );
					} catch {
						curViewRow.Cells["HCapBase"].Value = "";
					}
					try {
						curViewRow.Cells["HCapScore"].Value = ( (Decimal)curRow["HCapScoreSlalom"] ).ToString( "###0.0" );
					} catch {
						curViewRow.Cells["HCapScore"].Value = "";
					}
					try {
						curViewRow.Cells["Round"].Value = (Int16)curRow["RoundSlalom"];
					} catch {
						curViewRow.Cells["Round"].Value = 0;
					}
					try {
						curViewRow.Cells["Score"].Value = (Decimal)curRow["ScoreSlalom"];
					} catch {
						curViewRow.Cells["Score"].Value = 0;
					}
					try {
						curViewRow.Cells["NopsScore"].Value = ( (Decimal)curRow["PointsSlalom"] ).ToString( "###0.0" );
					} catch {
						curViewRow.Cells["NopsScore"].Value = 0;
					}
					try {
						curViewRow.Cells["FinalSpeedMph"].Value = (Byte)curRow["FinalSpeedMph"];
					} catch {
						curViewRow.Cells["FinalSpeedMph"].Value = 0;
					}
					try {
						curViewRow.Cells["FinalSpeedKph"].Value = (Byte)curRow["FinalSpeedKph"];
					} catch {
						curViewRow.Cells["FinalSpeedKph"].Value = 0;
					}
					try {
						curViewRow.Cells["FinalLen"].Value = (String)curRow["FinalLen"];
					} catch {
						curViewRow.Cells["FinalLen"].Value = 0;
					}
					try {
						curViewRow.Cells["FinalLenOff"].Value = (String)curRow["FinalLenOff"];
					} catch {
						curViewRow.Cells["FinalLenOff"].Value = 0;
					}
					try {
						curViewRow.Cells["FinalPassScore"].Value = (Decimal)curRow["FinalPassScore"];
					} catch {
						curViewRow.Cells["FinalPassScore"].Value = 0;
					}
					#endregion
				}
			}

			//Post data for external scoreboard applications
			if ( ExportLiveScoreboard.ScoreboardLocation.Length > 1 ) {
				ExportLiveScoreboard.exportCurrentLeaderSlalom( mySummaryDataTable, mySanctionNum );
			}
			this.Cursor = Cursors.Default;
			try {
				RowStatusLabel.Text = "Row 1 of " + scoreSummaryDataGridView.Rows.Count.ToString();
			} catch {
				RowStatusLabel.Text = "";
			}
		}

		private void loadGroupList() {
			if ( EventGroupList.DataSource == null ) {
				if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
					ArrayList curEventGroupList = new ArrayList();
					curEventGroupList.Add( "All" );
					curEventGroupList.Add( "Men A" );
					curEventGroupList.Add( "Women A" );
					curEventGroupList.Add( "Men B" );
					curEventGroupList.Add( "Women B" );
					curEventGroupList.Add( "Non Team" );
					EventGroupList.DataSource = curEventGroupList;
				} else {
					loadAgeGroupListFromData();
					//loadEventGroupListFromData();
				}
			} else {
				if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
					if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
					} else {
						loadAgeGroupListFromData();
						//loadEventGroupListFromData();
					}
				} else {
					if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
						ArrayList curEventGroupList = new ArrayList();
						curEventGroupList.Add( "All" );
						curEventGroupList.Add( "Men A" );
						curEventGroupList.Add( "Women A" );
						curEventGroupList.Add( "Men B" );
						curEventGroupList.Add( "Women B" );
						curEventGroupList.Add( "Non Team" );
						EventGroupList.DataSource = curEventGroupList;
					} else {
						loadAgeGroupListFromData();
						//loadEventGroupListFromData();
					}
				}
			}
		}

		private void loadEventGroupListFromData() {
			String curGroupValue = "";
			ArrayList curEventGroupList = new ArrayList();
			String curSqlStmt = "";
			curEventGroupList.Add( "All" );
			curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
				+ "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom'"
				+ "Order by EventGroup";
			DataTable curDataTable = getData( curSqlStmt );

			foreach ( DataRow curRow in curDataTable.Rows ) {
				curEventGroupList.Add( (String)curRow["EventGroup"] );
			}
			EventGroupList.DataSource = curEventGroupList;
			if ( curGroupValue.Length > 0 ) {
				foreach ( String curValue in (ArrayList)EventGroupList.DataSource ) {
					if ( curValue.Equals( curGroupValue ) ) {
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

		private void loadAgeGroupListFromData() {
			String curGroupValue = "";
			ArrayList curEventGroupList = new ArrayList();
			String curSqlStmt = "";
			curEventGroupList.Add( "All" );
			curSqlStmt = "SELECT DISTINCT AgeGroup FROM EventReg "
				+ "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' "
				+ "Order by AgeGroup";
			DataTable curDataTable = getData( curSqlStmt );

			foreach ( DataRow curRow in curDataTable.Rows ) {
				curEventGroupList.Add( (String)curRow["AgeGroup"] );
			}
			EventGroupList.DataSource = curEventGroupList;
			if ( curGroupValue.Length > 0 ) {
				foreach ( String curValue in (ArrayList)EventGroupList.DataSource ) {
					if ( curValue.Equals( curGroupValue ) ) {
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

		private void ScoreSummary_FormClosed( object sender, FormClosedEventArgs e ) {
			if ( this.WindowState == FormWindowState.Normal ) {
				Properties.Settings.Default.SlalomScoreSummary_Width = this.Size.Width;
				Properties.Settings.Default.SlalomScoreSummary_Height = this.Size.Height;
				Properties.Settings.Default.SlalomScoreSummary_Location = this.Location;
			}
		}

		private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
			MessageBox.Show( "Error happened " + e.Context.ToString() );
			if ( e.Context == DataGridViewDataErrorContexts.Commit ) {
				MessageBox.Show( "Commit error" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.CurrentCellChange ) {
				MessageBox.Show( "Cell change" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.Parsing ) {
				MessageBox.Show( "parsing error" );
			}
			if ( e.Context == DataGridViewDataErrorContexts.LeaveControl ) {
				MessageBox.Show( "leave control error" );
			}
			if ( ( e.Exception ) is ConstraintException ) {
				DataGridView view = (DataGridView)sender;
				view.Rows[e.RowIndex].ErrorText = "an error";
				view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

				e.ThrowException = false;
			}
		}

		private void numPrelimTextBox_Validated( object sender, EventArgs e ) {
			if ( numPrelimTextBox.Text.Length > 0 ) {
				try {
					Int16 curValue = Convert.ToInt16( numPrelimTextBox.Text );
					if ( curValue > 10 ) {
						MessageBox.Show( "Number of preliminary rounds must be less than 10 " );
					}
					myTourProperties.SlalomSummaryNumPrelim = numPrelimTextBox.Text;
				} catch {
					MessageBox.Show( "Number of preliminary rounds must be numeric " );
				}
			}
		}

		private void scoreSummaryDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			int curRowPos = e.RowIndex + 1;
			RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + scoreSummaryDataGridView.Rows.Count.ToString();
		}

		private void navSort_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			sortDialogForm.SortCommand = mySortCommand;
			sortDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( sortDialogForm.DialogResult == DialogResult.OK ) {
				mySortCommand = sortDialogForm.SortCommand;
				myTourProperties.SlalomScoreSummary_Sort = mySortCommand;
				loadDataGrid( mySummaryDataTable );
			}
		}

		private void navFilter_Click( object sender, EventArgs e ) {
			// Display the form as a modal dialog box.
			filterDialogForm.FilterCommand = myFilterCmd;
			filterDialogForm.ShowDialog( this );

			// Determine if the OK button was clicked on the dialog box.
			if ( filterDialogForm.DialogResult == DialogResult.OK ) {
				myFilterCmd = filterDialogForm.FilterCommand;
				myFilterCmd = myFilterCmd.Replace( "TeamCode", "TeamSlalom" );
				winStatusMsg.Text = "Filtered by " + myFilterCmd;
				loadDataGrid( mySummaryDataTable );
			}
		}

		private void navExport_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			myExportData.exportData( scoreSummaryDataGridView );
		}

		private void navExportHtml_Click( object sender, EventArgs e ) {
			ExportData myExportData = new ExportData();
			String printTitle = Properties.Settings.Default.Mdi_Title;
			String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
			String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
			printFooter.PadRight( 15, '*' );
			printFooter.PadLeft( 15, '*' );
			myExportData.exportDataAsHtml( scoreSummaryDataGridView, printTitle, printSubtitle, printFooter );
		}

		private void navScoreboard_Click( object sender, EventArgs e ) {
			String curPath = ExportLiveScoreboard.getCheckScoreboardLocation();
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
			if ( printReport( true ) ) ExportLiveWeb.uploadReportFile( "Results", "Slalom", mySanctionNum );
		}
		private void printReportTimer( object sender, EventArgs e ) {
			Timer curTimerObj = (Timer)sender;
			curTimerObj.Stop();
			curTimerObj.Tick -= new EventHandler( printReportTimer );
			printReport( false );
		}

		private bool printReport( bool inPublish ) {
			if ( inPublish && !( LiveWebHandler.LiveWebMessageHandlerActive ) ) LiveWebHandler.connectLiveWebHandler( mySanctionNum );
			if ( inPublish && !( LiveWebHandler.LiveWebMessageHandlerActive ) ) {
				MessageBox.Show( "Request to publish report but live web not successfully connected." );
				return false;
			}

			if ( h2hScoreButton.Checked ) {
				if ( mySummaryDataTable.Rows.Count > 0 ) {
					printHeadToHeadAwards();
					return true;
				} else {
					return false;
				}
			}

			PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
			Font saveShowDefaultCellStyle = scoreSummaryDataGridView.DefaultCellStyle.Font;
			if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
				scoreSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 12, FontStyle.Regular );
			} else {
				scoreSummaryDataGridView.DefaultCellStyle.Font = new Font( "Tahoma", 11, FontStyle.Regular );
			}

			bool returnValue = true;
			bool CenterOnPage = true;
			bool WithTitle = true;
			bool WithPaging = true;
			Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
			Font fontPrintFooter = new Font( "Times New Roman", 10 );

			PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
			if ( curPrintDialog == null ) return false;

			StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
			printTitle.Append( "\n Sanction " + mySanctionNum );
			printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
			printTitle.Append( "\n" + this.Text );
			if ( inPublish ) printTitle.Append( HelperFunctions.buildPublishReportTitle( mySanctionNum ) );

			myPrintDoc = new PrintDocument();
			myPrintDoc.DocumentName = this.Text;
			myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
			myPrintDoc.DefaultPageSettings.Landscape = false;
			myPrintDataGrid = new DataGridViewPrinter( scoreSummaryDataGridView, myPrintDoc,
				CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

			myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
			myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
			myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
			
			curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
			curPreviewDialog.ShowDialog();

			scoreSummaryDataGridView.DefaultCellStyle.Font = saveShowDefaultCellStyle;
			return returnValue;
		}

		// The PrintPage action for the PrintDocument control
		private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }

        private void printHeadToHeadAwards() {
            String curFilterCmd = "", curSortCmd = "";
            String curPlcmtOrg = "", curPlcmtMethod = "Score";
            Int16 curNumPrelimRounds = 0;
            try {
                curNumPrelimRounds = Convert.ToInt16( numPrelimTextBox.Text );
            } catch {
                curNumPrelimRounds = 0;
            }
            if (myFilterCmd.Length > 0) {
            } else {
                curFilterCmd = "Round > " + curNumPrelimRounds;
            }

            curSortCmd = "";
            if (plcmtTourButton.Checked) {
				curSortCmd = "Round ASC, RunOrderGroup ASC";
			} else if ( plcmtDivButton.Checked ) {
                curSortCmd = "AgeGroup ASC, Round ASC, RunOrderGroup ASC";
			} else if ( groupPlcmtButton.Checked ) {
				curSortCmd = "EventGroup ASC, Round ASC, RunOrderGroup ASC";
			} else {
				curSortCmd = "AgeGroup ASC, EventGroupSlalom ASC, Round ASC, RunOrderGroup ASC";
			}

            if (rawScoreButton.Checked) {
                curSortCmd += ", ScoreSlalom DESC";
            } else if (pointsScoreButton.Checked) {
                curPlcmtMethod = "Points";
                curSortCmd += ", PointsSlalom DESC";
            } else {
                curSortCmd += ", ScoreSlalom DESC";
            }

            if (plcmtDivButton.Checked) {
                curPlcmtOrg = "div";
			} else if ( groupPlcmtButton.Checked ) {
				curPlcmtOrg = "group";
			} else {
                curPlcmtOrg = "tour";
            }

            PrintHeadToHeadAwards curPrintForm = new PrintHeadToHeadAwards();
            curPrintForm.PrintLandscape = true;
            curPrintForm.ReportHeader = "Head to Head Results";
            curPrintForm.DivInfoDataTable = getEventDivMaxMinSpeed();
            curPrintForm.TourRules = myTourRules;
            curPrintForm.TourName = (String)myTourRow["Name"];
            curPrintForm.TourPrelimRounds = curNumPrelimRounds;
            curPrintForm.TourEvent = "Slalom";
            curPrintForm.TourPlcmtOrg = curPlcmtOrg;
            curPrintForm.TourPlcmtMethod = curPlcmtMethod;

            #region Enhance scoring data with seeding (RunOrder) and seeding score (RankingScore)
            DataTable curEventRegDataTable = getEventRegData( curNumPrelimRounds, "Slalom" );

            mySummaryDataTable.DefaultView.Sort = curSortCmd;
            mySummaryDataTable.DefaultView.RowFilter = curFilterCmd;
            DataTable curShowDataTable = mySummaryDataTable.DefaultView.ToTable();
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "RunOrder";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curShowDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RankingScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curShowDataTable.Columns.Add( curCol );


            DataRow[] curFindRows = null;
            String curMemberId = "", curAgeGroup = "", curEventGroup = "";
            int curRound = 0;
            foreach (DataRow curRow in curShowDataTable.Rows) {
                curMemberId = (String)curRow["MemberId"];
                curAgeGroup = (String)curRow["AgeGroup"];
				curEventGroup = (String) curRow["EventGroupSlalom"];
				curRound = (Int16)curRow["Round"];
				if ( plcmtDivButton.Checked ) {
					curFindRows = curEventRegDataTable.Select( "MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "' AND Round = " + curRound );
				} else if ( groupPlcmtButton.Checked ) {
					curFindRows = curEventRegDataTable.Select( "MemberId = '" + curMemberId + "' AND EventGroup = '" + curEventGroup + "' AND Round = " + curRound );
				} else {
					curFindRows = curEventRegDataTable.Select( "MemberId = '" + curMemberId + "' AND Round = " + curRound );
				}
				if ( curFindRows.Length > 0) {
                    try {
                        curRow["RunOrder"] = curFindRows[0]["RunOrder"];
                    } catch {
                        curRow["RunOrder"] = 0;
                    }
                    try {
                        curRow["RankingScore"] = curFindRows[0]["RankingScore"];
                    } catch {
                        curRow["RankingScore"] = 0;
                    }
                } else {
                    curRow["RunOrder"] = 0;
                    curRow["RankingScore"] = 0;
                }
            }
            curPrintForm.ShowDataTable = curShowDataTable;
            #endregion

            curPrintForm.Print();
        }

        private DataTable getEventRegData(int inNumPrelimRounds, String inEvent) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.Event, E.SanctionId, E.MemberId, E.AgeGroup, O.Round, O.EventGroup, O.RunOrder" );
            curSqlStmt.Append( ", E.EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore " );
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + inEvent + "' AND O.Round > " + inNumPrelimRounds.ToString() + " " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getEventDivMaxMinSpeed() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L1.ListCode as Div, L1.SortSeq as SortSeq, L1.CodeValue as DivName, L2.CodeValue AS MaxSpeed, L3.CodeValue AS MinSpeed " );
            curSqlStmt.Append( "FROM CodeValueList AS L1 " );
            curSqlStmt.Append( "INNER JOIN CodeValueList AS L2 ON L2.ListCode = L1.ListCode AND L2.ListName like '%SlalomMax' " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS L3 ON L3.ListCode = L1.ListCode AND L3.ListName like '%SlalomMin' " );
            curSqlStmt.Append( "WHERE L1.ListName like '%AgeGroup' " );
            return getData( curSqlStmt.ToString() );
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
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
