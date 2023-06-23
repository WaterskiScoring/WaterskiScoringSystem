using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class RunOrderElimDialogForm : Form {
        private String myWindowTitle;
        private String myCommand;
        private String myEvent;
		private String mySortCmd = "";
		private String myPlcmtMethod;
        private String myPlcmtOrg;
        private String myPointsMethod;
        private String myDataType;
        private String myTourRules = "";
        private String myDivFilter = null;
		private String myGroupFilter = null;
		private Int16 myNumSkiers = 0;
        private Int16 myRound = 0;
        private DataRow myTourRow;
        private CalcScoreSummary myCalcSummary;

        public RunOrderElimDialogForm() {
            InitializeComponent();
            myWindowTitle = this.Text;
        }

        private void RunOrderElimDialogForm_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.RunOrderElim_Width > 0 ) {
                this.Width = Properties.Settings.Default.RunOrderElim_Width;
            }
            if ( Properties.Settings.Default.RunOrderElim_Height > 0 ) {
                this.Height = Properties.Settings.Default.RunOrderElim_Height;
            }
            if ( Properties.Settings.Default.RunOrderElim_Location.X > 0
                && Properties.Settings.Default.RunOrderElim_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.RunOrderElim_Location;
            }

            myCalcSummary = new CalcScoreSummary();

            try {
                NumSkiersTextbox.Text = Properties.Settings.Default.RunOrderElimNumSkiers.ToString();
                myNumSkiers = Convert.ToInt16( NumSkiersTextbox.Text );
            } catch {
                NumSkiersTextbox.Text = "0";
                myNumSkiers = 0;
            }

            //setDataSelectOptions();
        }

        private void RunOrderElimDialogForm_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.RunOrderElim_Width = this.Size.Width;
                Properties.Settings.Default.RunOrderElim_Height = this.Size.Height;
                Properties.Settings.Default.RunOrderElim_Location = this.Location;
            }
            try {
                Properties.Settings.Default.RunOrderElimNumSkiers = Convert.ToInt32(NumSkiersTextbox.Text);
            } catch {
                Properties.Settings.Default.RunOrderElimNumSkiers = 0;
            }
        }

        public DataRow TourData {
            get {
                return myTourRow;
            }
            set {
                myTourRow = value;
            }
        }

        public String TourRules {
            get {
                return myTourRules;
            }
            set {
                myTourRules = value;
            }
        }

        public String PlcmtMethod {
            get {
                return myPlcmtMethod;
            }
            set {
                myPlcmtMethod = value;
            }
        }

        public String PlcmtOrg {
            get {
                return myPlcmtOrg;
            }
            set {
                myPlcmtOrg = value;
            }
        }

        public String PointsMethod {
            get {
                return myPointsMethod;
            }
            set {
                myPointsMethod = value;
            }
        }

        public String DataType {
            get {
                return myDataType;
            }
            set {
                myDataType = value;
            }
        }

        public Int16 NumSkiers {
            get {
                return myNumSkiers;
            }
            set {
                myNumSkiers = value;
            }
        }

        public void getElimEntries(String inEvent, Int16 inRound) {
            myCommand = "Elim";
            myEvent = inEvent;
            myRound = inRound;
            this.Text = myWindowTitle + " " + myRound.ToString() + " - " + myEvent + " - Elimination";
            setDataSelectOptions();

            RemoveUnscoredButton.Visible = false;
            BestScoreButton.Visible = true;
            LastScoreButton.Visible = true;
            TotalScoreButton.Visible = true;
            SelectButton.Visible = false;
            RefreshButton.Visible = false;
            RefreshButton.Location = RemoveUnscoredButton.Location;

            groupPlcmtButton.Visible = true;
            plcmtDivGrpButton.Visible = true;
            plcmtDivButton.Visible = true;
            plcmtTourButton.Visible = true;
			plcmtTourButton.Checked = true;


			PointsMethodGroupBox.Visible = false;
			plcmtPointsButton.Visible = false;
			kBasePointsButton.Visible = false;
			handicapPointsButton.Checked = false;
			nopsPointsButton.Checked = true;
			kBasePointsButton.Checked = false;
			plcmtPointsButton.Checked = false;
			ratioPointsButton.Checked = false;
			plcmtPointsButton.Visible = false;
			kBasePointsButton.Visible = false;

			NumSkiersTextbox.Visible = true;
			NumSkiersLabel.Visible = true;

			h2hNextGroupBox.Visible = false;
            plcmtMethodGroupBox.Visible = true;
            if (pointsScoreButton.Checked) {
                PointsMethodGroupBox.Visible = true;
            } else {
                PointsMethodGroupBox.Visible = false;
            }

            previewDataGridView.Rows.Clear();
        }

		public void getPickChooseEntries( String inEvent, Int16 inRound, String inSortCmd ) {
			myCommand = "Pick";
			myEvent = inEvent;
			myRound = inRound;
			mySortCmd = inSortCmd;

			this.Text = myWindowTitle + " " + myRound.ToString() + " - " + myEvent + " - Pick And Choose";
			setDataSelectOptions();

			h2hNextGroupBox.Visible = false;
			plcmtMethodGroupBox.Visible = false;
			plcmtGroupBox.Visible = false;
			PointsMethodGroupBox.Visible = false;

			EventGroupList.Visible = false;
			EventGroupListLabel.Visible = false;

			RemoveUnscoredButton.Visible = false;
			BestScoreButton.Visible = false;
			LastScoreButton.Visible = false;
			TotalScoreButton.Visible = false;

			SelectButton.Visible = true;
			RefreshButton.Visible = true;
			RefreshButton.Location = RemoveUnscoredButton.Location;

			plcmtDivButton.Visible = true;
			groupPlcmtButton.Visible = false;
			plcmtDivGrpButton.Visible = false;
			plcmtTourButton.Visible = false;

			rawScoreButton.Checked = true;

            PointsMethodGroupBox.Visible = false;
			kBasePointsButton.Visible = false;
			handicapPointsButton.Checked = false;
			handicapPointsButton.Visible = false;
            nopsPointsButton.Checked = true;
			nopsPointsButton.Visible = false;
			kBasePointsButton.Checked = false;
			kBasePointsButton.Visible = false;
			plcmtPointsButton.Visible = false;
			plcmtPointsButton.Checked = false;
			ratioPointsButton.Checked = false;
			kBasePointsButton.Visible = false;

			NumSkiersTextbox.Visible = false;
			NumSkiersLabel.Visible = false;

			previewDataGridView.Rows.Clear();

			loadDataGridViewPick();

		}

		public void getHeadToHeadEntries(String inEvent, Int16 inRound) {
            myCommand = "HeadToHead";
            myEvent = inEvent;
            myRound = inRound;
            this.Text = myWindowTitle + " " + myRound.ToString() + " - " + myEvent + " - Head to Head";
            setDataSelectOptions();

			NumSkiersTextbox.Visible = true;
            RemoveUnscoredButton.Visible = false;
            BestScoreButton.Visible = true;
            LastScoreButton.Visible = true;
            TotalScoreButton.Visible = true;
            SelectButton.Visible = false;
            RefreshButton.Visible = false;
            RefreshButton.Location = RemoveUnscoredButton.Location;

            groupPlcmtButton.Visible = true;
            plcmtDivGrpButton.Visible = false;
            plcmtDivButton.Visible = true;
            plcmtTourButton.Visible = true;
            if (groupPlcmtButton.Checked) plcmtTourButton.Checked = true;
            if (plcmtDivGrpButton.Checked) plcmtTourButton.Checked = true;

            h2hNextGroupBox.Visible = false;

			PointsMethodGroupBox.Visible = false;
			plcmtPointsButton.Visible = false;
			kBasePointsButton.Visible = false;
			handicapPointsButton.Checked = false;
			nopsPointsButton.Checked = true;
			kBasePointsButton.Checked = false;
			plcmtPointsButton.Checked = false;
			ratioPointsButton.Checked = false;
			plcmtPointsButton.Visible = false;
			kBasePointsButton.Visible = false;

			plcmtMethodGroupBox.Visible = true;
            previewDataGridView.Rows.Clear();

            if (inRound == 1) {
                BestScoreButton.Visible = false;
                LastScoreButton.Visible = false;
                TotalScoreButton.Visible = false;
                RefreshButton.Visible = true;
                SelectButton.Visible = true;
                PointsMethodGroupBox.Visible = false;
            }

			if ( pointsScoreButton.Checked ) {
				PointsMethodGroupBox.Visible = true;
			} else {
				PointsMethodGroupBox.Visible = false;
			}
		}

		public void getHeadToHeadNexEntries(String inEvent, Int16 inRound) {
            myCommand = "HeadToHeadNext";
            myEvent = inEvent;
            myRound = inRound;
            this.Text = myWindowTitle + " " + myRound.ToString() + " - " + myEvent + " - Head to Head Next";
            setDataSelectOptions();

            NumSkiersTextbox.Visible = true;
			NumSkiersLabel.Visible = true;
			RemoveUnscoredButton.Visible = false;

            rawScoreButton.Checked = true;

            BestScoreButton.Visible = false;
            LastScoreButton.Visible = false;
            TotalScoreButton.Visible = false;
            SelectButton.Visible = false;
            
            RefreshButton.Location = RemoveUnscoredButton.Location;
            RefreshButton.Visible = true;

            groupPlcmtButton.Visible = true;
            plcmtDivGrpButton.Visible = false;
            plcmtDivButton.Visible = true;
            plcmtTourButton.Visible = true;
            plcmtTourButton.Checked = true;

            PointsMethodGroupBox.Visible = false;
            handicapPointsButton.Checked = false;
            nopsPointsButton.Checked = true;
            kBasePointsButton.Checked = false;
            plcmtPointsButton.Checked = false;
            ratioPointsButton.Checked = false;
			plcmtPointsButton.Visible = false;
			kBasePointsButton.Visible = false;

			plcmtMethodGroupBox.Visible = true;
            h2hNextGroupBox.Visible = true;

			if ( pointsScoreButton.Checked ) {
				PointsMethodGroupBox.Visible = true;
			} else {
				PointsMethodGroupBox.Visible = false;
			}

			previewDataGridView.Rows.Clear();
        }

        private void setDataSelectOptions() {
            myPlcmtMethod = Properties.Settings.Default.RunOrderElimPlcmtMethod;
            if (Properties.Settings.Default.RunOrderElimPlcmtMethod.ToLower().Equals( "points" )) {
                pointsScoreButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPlcmtMethod.ToLower().Equals( "score" )) {
                rawScoreButton.Checked = true;
            } else {
                rawScoreButton.Checked = true;
            }

            myPlcmtOrg = Properties.Settings.Default.RunOrderElimPlcmtOrg;
            if (Properties.Settings.Default.RunOrderElimPlcmtOrg.ToLower().Equals( "tour" )) {
                plcmtTourButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPlcmtOrg.ToLower().Equals( "div" )) {
                plcmtDivButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPlcmtOrg.ToLower().Equals( "divgr" )) {
                plcmtDivGrpButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPlcmtOrg.ToLower().Equals( "group" )) {
                groupPlcmtButton.Checked = true;
            } else {
                groupPlcmtButton.Checked = true;
            }

            myPointsMethod = Properties.Settings.Default.RunOrderElimPointsMethod;
            if (Properties.Settings.Default.RunOrderElimPointsMethod.ToLower().Equals( "nops" )) {
                nopsPointsButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPointsMethod.ToLower().Equals( "kbase" )) {
                kBasePointsButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPointsMethod.ToLower().Equals( "hcap" )) {
                handicapPointsButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPointsMethod.ToLower().Equals( "ratio" )) {
                ratioPointsButton.Checked = true;
            } else if (Properties.Settings.Default.RunOrderElimPointsMethod.ToLower().Equals( "nops" )) {
                nopsPointsButton.Checked = true;
            } else {
                nopsPointsButton.Checked = true;
            }

            if (myTourRules.ToLower().Equals( "iwwf" )) {
                kBasePointsButton.Checked = true;
                pointsScoreButton.Checked = true;
            }

        }

        private void RefreshButton_Click(object sender, EventArgs e) {
            if (myCommand.Equals( "HeadToHeadNext" )) {
                loadDataGridViewH2HN();
                SelectButton.Visible = true;
            } else if (myCommand.Equals( "HeadToHead" )) {
                loadDataGridViewH2H();
			} else if ( myCommand.Equals( "Pick" ) ) {
				loadDataGridViewPick();
			} else {
                loadDataGridViewElim();
            }
        }

        private void RemoveUnscoredButton_Click(object sender, EventArgs e) {
            myDataType = "RemoveUnscored";
        }

        private void BestScoreButton_Click( object sender, EventArgs e ) {
            myDataType = "Best";
            BestScoreButton.Visible = true;
            RemoveUnscoredButton.Visible = false;
            LastScoreButton.Visible = false;
            TotalScoreButton.Visible = false;
            SelectButton.Visible = true;
            if (myCommand.Equals( "HeadToHeadNext" )) {
                loadDataGridViewH2HN();
            } else if (myCommand.Equals( "HeadToHead" )) {
                loadDataGridViewH2H();
            } else {
                loadDataGridViewElim();
            }
        }

        private void LastScoreButton_Click( object sender, EventArgs e ) {
            myDataType = "Round";
            BestScoreButton.Visible = false;
            RemoveUnscoredButton.Visible = false;
            LastScoreButton.Visible = true;
            TotalScoreButton.Visible = false;
            SelectButton.Visible = true;
            if (myCommand.Equals( "HeadToHeadNext" )) {
                loadDataGridViewH2HN();
            } else if (myCommand.Equals( "HeadToHead" )) {
                loadDataGridViewH2H();
            } else {
                loadDataGridViewElim();
            }
        }

        private void TotalScoreButton_Click( object sender, EventArgs e ) {
            myDataType = "Total";
            BestScoreButton.Visible = false;
            RemoveUnscoredButton.Visible = false;
            LastScoreButton.Visible = false;
            TotalScoreButton.Visible = true;
            SelectButton.Visible = true;
            if (myCommand.Equals( "HeadToHeadNext" )) {
                loadDataGridViewH2HN();
            } else if (myCommand.Equals( "HeadToHead" )) {
                loadDataGridViewH2H();
            } else {
                loadDataGridViewElim();
            }
        }

        private void NumSkiersTextbox_Validating( object sender, CancelEventArgs e ) {
            try {
                Int16 curNum = Convert.ToInt16(((TextBox)sender).Text);
                e.Cancel = false;
            } catch {
                e.Cancel = true;
                MessageBox.Show("Number of skiers must be numeric");
            }
        }

        private void NumSkiersTextbox_Validated( object sender, EventArgs e ) {
            myNumSkiers = Convert.ToInt16(( (TextBox)sender ).Text);
        }

        private void setPointsMethod_CheckedChanged(object sender, EventArgs e) {
            if (((RadioButton)sender).Checked) {
                String curName = ((RadioButton)sender).Name;
                if (curName.Equals( "nopsPointsButton" )) {
                    myPointsMethod = "nops";
                } else if (curName.Equals( "plcmtPointsButton" )) {
                    myPointsMethod = "plcmt";
                } else if (curName.Equals( "kBasePointsButton" )) {
                    myPointsMethod = "kbase";
                } else if (curName.Equals( "handicapPointsButton" )) {
                    myPointsMethod = "hcap";
                } else if (curName.Equals( "ratioPointsButton" )) {
                    myPointsMethod = "ratio";
                } else {
                    myPointsMethod = "nops";
                }
                Properties.Settings.Default.RunOrderElimPointsMethod = myPointsMethod;
            }
        }

        private void setPlcmtMethod_CheckedChanged(object sender, EventArgs e) {
            if (((RadioButton)sender).Checked) {
                String curName = ((RadioButton)sender).Name;
                if (curName.Equals( "rawScoreButton" )) {
                    myPlcmtMethod = "score";
                    PointsMethodGroupBox.Visible = false;
                } else if (curName.Equals( "pointsScoreButton" )) {
                    myPlcmtMethod = "points";
                    PointsMethodGroupBox.Visible = true;
                    nopsPointsButton.Checked = true;
                } else {
                    myPlcmtMethod = "score";
                    PointsMethodGroupBox.Visible = false;
                    
                    nopsPointsButton.Checked = false;
                    handicapPointsButton.Checked = false;
                    kBasePointsButton.Checked = false;
                    plcmtPointsButton.Checked = false;
                    ratioPointsButton.Checked = false;
                }
                Properties.Settings.Default.RunOrderElimPlcmtMethod = myPlcmtMethod;
            }
        }

        private void setPlcmtOrg_CheckedChanged(object sender, EventArgs e) {
            if (((RadioButton)sender).Checked) {
                String curName = ((RadioButton)sender).Name;
                if (curName.Equals( "groupPlcmtButton" )) {
                    myPlcmtOrg = "group";
					loadGroupList();

				} else if (curName.Equals( "plcmtTourButton" )) {
                    myPlcmtOrg = "tour";

				} else if (curName.Equals( "plcmtDivButton" )) {
                    myPlcmtOrg = "div";
					loadGroupList();

				} else if (curName.Equals( "plcmtDivGrpButton" )) {
                    myPlcmtOrg = "divgr";

				} else {
                    myPlcmtOrg = "tour";
                }
                Properties.Settings.Default.RunOrderElimPlcmtOrg = myPlcmtOrg;

                if (myPlcmtOrg.ToLower().Equals( "div" )) {
                    EventGroupList.Visible = true;
                    EventGroupListLabel.Visible = true;
				} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
					EventGroupList.Visible = true;
					EventGroupListLabel.Visible = true;
				} else {
                    EventGroupList.Visible = false;
                    EventGroupListLabel.Visible = false;
                }
            }
        }

        private void loadDataGridViewElim() {
            String curMethodName = "Tournament:RunOrderElimDialogForm:loadDataGridViewElim";
            String curGroup = "", curDiv = "", curPlcmt = "", curSortCmd = "", curReportGroup = "", prevReportGroup = "";
            String curPlcmtName = "", curRoundName = "", curGroupName = "", curScoreName = "", curFilterCmd = "";
            int curIdx = 0, curNumPlcmt = 0, curRunOrder = 0, curSkierSeed = 0, curGroupNumSkiers = 0, curMaxSkiersSelect = 0;
            DataTable curDataTable = null;
            DataTable curSummaryDataTable = null;
            DataRow[] curFindRows = null;
            myDivFilter = null;
			myGroupFilter = null;
			if ( myCommand.Equals("Pick")) {
				curSortCmd = mySortCmd;

			} else if ( myPlcmtOrg.ToLower().Equals( "div" ) ) {
				myDivFilter = EventGroupList.SelectedItem.ToString();
				myGroupFilter = "All";

			} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
				myGroupFilter = EventGroupList.SelectedItem.ToString();
				myDivFilter = "All";

			} else {
                myDivFilter = "All";
				myGroupFilter = "All";
            }

            try {
				Cursor.Current = Cursors.WaitCursor;
				if ( myEvent.ToLower().Equals( "trick" ) ) {
					if ( myRound > 1 ) {
						if ( myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" ) ) {
							curSummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String) myTourRow["SanctionId"], "Trick", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						} else {
							curSummaryDataTable = myCalcSummary.getTrickSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						}
					} else {
						curSummaryDataTable = getEventRegData( myDivFilter, myGroupFilter );
					}
					curPlcmtName = "PlcmtTrick";
					curRoundName = "RoundTrick";
					curGroupName = "EventGroupTrick";
					curScoreName = "ScoreTrick";

				} else if ( myEvent.ToLower().Equals( "jump" ) ) {
					if ( myRound > 1 ) {
						if ( myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" ) ) {
							curSummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String) myTourRow["SanctionId"], "Jump", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						} else {
							curSummaryDataTable = myCalcSummary.getJumpSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						}
					} else {
						curSummaryDataTable = getEventRegData( myDivFilter, myGroupFilter );
					}
					curPlcmtName = "PlcmtJump";
					curRoundName = "RoundJump";
					curGroupName = "EventGroupJump";
					curScoreName = "ScoreMeters";

				} else {
					if ( myRound > 1 ) {
						if ( myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" ) ) {
							curSummaryDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String) myTourRow["SanctionId"], "Slalom", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						} else {
							curSummaryDataTable = myCalcSummary.getSlalomSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
						}
					} else {
						curSummaryDataTable = getEventRegData( myDivFilter, myGroupFilter );
					}
					curPlcmtName = "PlcmtSlalom";
					curRoundName = "RoundSlalom";
					curGroupName = "EventGroupSlalom";
					curScoreName = "ScoreSlalom";
				}

				if ( myPlcmtOrg.ToLower().Equals( "div" ) ) {
					curSortCmd = "AgeGroup ASC, " + curPlcmtName + " ASC, SkierName ASC";
				} else if ( myPlcmtOrg.ToLower().Equals( "divgr" ) ) {
					curSortCmd = "AgeGroup ASC, " + curGroupName + " ASC, " + curPlcmtName + " ASC, SkierName ASC, " + curRoundName + " ASC";
				} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
					curSortCmd = curGroupName + " ASC, " + curPlcmtName + " ASC, SkierName ASC ";
				} else {
					curSortCmd = curPlcmtName + " ASC, SkierName ASC ";
				}

				if ( myDataType.ToLower().Equals( "round" ) ) {
					curSummaryDataTable.DefaultView.Sort = curRoundName + " ASC, " + curSortCmd;
					curDataTable = curSummaryDataTable.DefaultView.ToTable();
					curDataTable.DefaultView.RowFilter = curRoundName + " = " + ( myRound - 1 ).ToString();
					curDataTable = curDataTable.DefaultView.ToTable();
				} else {
					curSummaryDataTable.DefaultView.Sort = curSortCmd;
					curDataTable = curSummaryDataTable.DefaultView.ToTable();
				}

				DataTable curEntriesDataTable = getCurrentRoundEntries( myDivFilter, myGroupFilter );
				int curEntryCount = curEntriesDataTable.Rows.Count;

				previewDataGridView.Rows.Clear();
				if ( curDataTable.Rows.Count > 0 ) {
					curGroupNumSkiers = curDataTable.Rows.Count;
                    curMaxSkiersSelect = myNumSkiers;
                    if ( curDataTable.Rows.Count < myNumSkiers ) curMaxSkiersSelect = curDataTable.Rows.Count;
                    DataGridViewRow curViewRow;
					foreach ( DataRow curDataRow in curDataTable.Rows ) {
						curIdx = previewDataGridView.Rows.Add();

						curDiv = (String) curDataRow["AgeGroup"];
						curGroup = (String) curDataRow[curGroupName];
						curPlcmt = (String) curDataRow[curPlcmtName];
						if ( myPlcmtOrg.ToLower().Equals( "agegroup" ) ) {
							curReportGroup = curDiv;
							curFilterCmd = "AgeGroup = '" + curDiv + "'";
						} else if ( myPlcmtOrg.ToLower().Equals( "div" ) ) {
							curReportGroup = curDiv;
							curFilterCmd = "AgeGroup = '" + curDiv + "'";
						} else if ( myPlcmtOrg.ToLower().Equals( "divgr" ) ) {
							curReportGroup = curDiv + "-" + curGroup;
							curFilterCmd = "AgeGroup = '" + curDiv + "' AND " + curGroupName + " = '" + curGroup + "'";
						} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
							curReportGroup = curGroup;
							curFilterCmd = curGroupName + " = '" + curGroup + "'";
						} else {
							curReportGroup = "";
							curPlcmt = (String) curDataRow[curPlcmtName];
							curFilterCmd = "";
						}

						if ( curReportGroup != prevReportGroup ) {
							curFindRows = curDataTable.Select( curFilterCmd );
							curGroupNumSkiers = curFindRows.Length;
							if ( curGroupNumSkiers > curMaxSkiersSelect ) {
								curRunOrder = curMaxSkiersSelect;
							} else {
								curRunOrder = curGroupNumSkiers;
							}
							curSkierSeed = 1;
							if ( prevReportGroup.Length > 0 ) {
								curViewRow = previewDataGridView.Rows[curIdx];
								curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
								previewDataGridView.Rows[curIdx].Height = 4;
								curIdx = previewDataGridView.Rows.Add();
							}
						} else {
							curSkierSeed += 1;
							if ( curSkierSeed > curMaxSkiersSelect ) {
								curRunOrder = curGroupNumSkiers - curSkierSeed + curMaxSkiersSelect + 1;
							} else {
								curRunOrder = curMaxSkiersSelect - curSkierSeed + 1;
							}
						}
						if ( curPlcmt.Contains( "T" ) ) {
							curNumPlcmt = Convert.ToInt32( curPlcmt.Substring( 0, curPlcmt.IndexOf( "T" ) ) );
						} else {
							curNumPlcmt = Convert.ToInt32( curPlcmt );
						}
						prevReportGroup = curReportGroup;

						curViewRow = previewDataGridView.Rows[curIdx];
						if ( curEntryCount > 0 ) {
							curFindRows = curEntriesDataTable.Select( "MemberId = '" + (String) curDataRow["MemberId"] + "' AND AgeGroup = '" + (String) curDataRow["AgeGroup"] + "'" );
							if ( curFindRows.Length > 0 ) {
								curViewRow.Cells["previewSelected"].Value = true;
							} else {
								curViewRow.Cells["previewSelected"].Value = false;
							}
						} else {
							if ( curSkierSeed <= curMaxSkiersSelect ) {
								curViewRow.Cells["previewSelected"].Value = true;
							} else {
								if ( curSkierSeed < ( curMaxSkiersSelect + 1 ) ) {
									curViewRow = previewDataGridView.Rows[curIdx];
									curViewRow.DefaultCellStyle.BackColor = Color.LightCyan;
									previewDataGridView.Rows[curIdx].Height = 2;
									curIdx = previewDataGridView.Rows.Add();
									curViewRow = previewDataGridView.Rows[curIdx];
									curViewRow.Cells["previewSelected"].Value = false;
								} else {
									curViewRow.Cells["previewSelected"].Value = false;
								}
							}
						}
						curViewRow.Cells["previewSanctionId"].Value = (String) curDataRow["SanctionId"];
						curViewRow.Cells["previewMemberId"].Value = (String) curDataRow["MemberId"];
						curViewRow.Cells["previewSkierName"].Value = (String) curDataRow["SkierName"];
						curViewRow.Cells["previewEvent"].Value = myEvent;
						curViewRow.Cells["previewAgeGroup"].Value = (String) curDataRow["AgeGroup"];
						curViewRow.Cells["previewRound"].Value = myRound.ToString( "0" );
						try {
							curViewRow.Cells["previewEventGroup"].Value = curGroup;
						} catch {
							curViewRow.Cells["previewEventGroup"].Value = "";
						}
						curViewRow.Cells["previewSeed"].Value = curSkierSeed.ToString();
						curViewRow.Cells["previewOrder"].Value = curRunOrder.ToString();
						curViewRow.Cells["PreviewScore"].Value = curDataRow[curScoreName].ToString();
						curViewRow.Cells["previewPlcmt"].Value = curPlcmt;

					}
					try {
						int curRowPos = curIdx + 1;
						RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + previewDataGridView.Rows.Count.ToString();
					} catch {
						RowStatusLabel.Text = "";
					}

					curIdx = 0;
					previewDataGridView.CurrentCell = previewDataGridView.Rows[curIdx].Cells["previewSkierName"];
				}
				Cursor.Current = Cursors.Default;

			} catch (Exception excp) {
                MessageBox.Show( "Error attempting to prepare for head to head selection \n" + excp.Message );
            }
        }

        private void loadDataGridViewH2H() {
            String curMethodName = "Tournament:RunOrderElimDialogForm:loadDataGridViewH2H";
            String curGroup = "", curRoGroup = "", curDiv = "", curPlcmt = "", curSortCmd = "", curReportGroup = "", prevReportGroup = "", curNewRoGroup= "";
            String curPlcmtName = "", curRoundName = "", curGroupName = "", curScoreName = "";
            if (myPlcmtOrg.ToLower().Equals( "div" )) {
                myDivFilter = EventGroupList.SelectedItem.ToString();
				myGroupFilter = "All";

			} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
				myGroupFilter = EventGroupList.SelectedItem.ToString();
				myDivFilter = "All";

			} else {
                myDivFilter = null;
				myGroupFilter = null;
            }

            int curIdx = 0, curNumPlcmt = 0, curRunOrder = 0, curNumSkierSelected = 0;
            DataTable curDataTable = null;

            try {
                Cursor.Current = Cursors.WaitCursor;
                int curMaxGroups = Convert.ToInt32( myNumSkiers ) >> 1;

                if (myEvent.ToLower().Equals( "trick" )) {
                    curPlcmtName = "PlcmtTrick";
                    curRoundName = "RoundTrick";
                    curGroupName = "EventGroupTrick";
                    if (pointsScoreButton.Checked) {
                        curScoreName = "PointsTrick";
                    } else {
                        curScoreName = "ScoreTrick";
                    }
                    if (myRound > 1) {
                        if (myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" )) {
                            curDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String)myTourRow["SanctionId"], "Trick", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, null, myDivFilter );
                        } else {
                            curDataTable = myCalcSummary.getTrickSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, null, myDivFilter );
                        }
                    } else {
						curGroupName = "EventGroup";
						curDataTable = getEventRegData( myDivFilter, myGroupFilter );
                    }

				} else if (myEvent.ToLower().Equals( "jump" )) {
                    curPlcmtName = "PlcmtJump";
                    curRoundName = "RoundJump";
                    curGroupName = "EventGroupJump";
                    if (pointsScoreButton.Checked) {
                        curScoreName = "PointsJump";
                    } else {
                        curScoreName = "ScoreMeters";
                    }
                    if (myRound > 1) {
                        if (myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" )) {
                            curDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String)myTourRow["SanctionId"], "Jump", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, null, myDivFilter );
                        } else {
                            curDataTable = myCalcSummary.getJumpSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, null, myDivFilter );
                        }
                    } else {
                        curDataTable = getEventRegData( myDivFilter, myGroupFilter );
						curGroupName = "EventGroup";
					}

				} else {
                    curPlcmtName = "PlcmtSlalom";
                    curRoundName = "RoundSlalom";
                    curGroupName = "EventGroupSlalom";
                    if (pointsScoreButton.Checked) {
                        curScoreName = "PointsSlalom";
                    } else {
                        curScoreName = "ScoreSlalom";
                    }
                    if (myRound > 1) {
                        if (myTourRules.ToLower().Equals( "iwwf" ) && myPointsMethod.ToLower().Equals( "kbase" )) {
                            curDataTable = myCalcSummary.CalcIwwfEventPlcmts( myTourRow, (String)myTourRow["SanctionId"], "Slalom", myTourRules, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, null, myDivFilter );
                        } else {
                            curDataTable = myCalcSummary.getSlalomSummary( myTourRow, myDataType, myPlcmtMethod, myPlcmtOrg, myPointsMethod, myGroupFilter, myDivFilter );
                        }
                    } else {
                        curDataTable = getEventRegData( myDivFilter, myGroupFilter );
						curGroupName = "EventGroup";
					}
				}

                if (myDataType != null && myDataType.ToLower().Equals( "round" )) {
                    curDataTable.DefaultView.RowFilter = curRoundName + " = " + ( myRound - 1 ).ToString();
                    curDataTable = curDataTable.DefaultView.ToTable();
                }

                DataTable curEntriesDataTable = getCurrentRoundEntries(myDivFilter, myGroupFilter );
                int curEntryCount = curEntriesDataTable.Rows.Count;
                DataRow[] curFindRows = null;

                previewDataGridView.Rows.Clear();
                if (curDataTable.Rows.Count > 0) {
                    DataGridViewRow curViewRow;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        curIdx = previewDataGridView.Rows.Add();

                        curDiv = (String)curDataRow["AgeGroup"];
                        curGroup = (String)curDataRow[curGroupName];
                        curPlcmt = (String)curDataRow[curPlcmtName];
                        if (myPlcmtOrg.ToLower().Equals( "agegroup" )) {
                            curReportGroup = curDiv;
                        } else if (myPlcmtOrg.ToLower().Equals( "div" )) {
                            curReportGroup = curDiv;
                        } else if (myPlcmtOrg.ToLower().Equals( "divgr" )) {
                            curReportGroup = curDiv + "-" + curGroup;
                        } else if (myPlcmtOrg.ToLower().Equals( "group" )) {
                            curReportGroup = curGroup;
                        } else {
                            curReportGroup = "";
                            curPlcmt = (String)curDataRow[curPlcmtName];
                        }

                        if (curReportGroup != prevReportGroup) {
                            curNumSkierSelected = 0;
                            curRunOrder = 1;
                            if (prevReportGroup.Length > 0) {
                                curViewRow = previewDataGridView.Rows[curIdx];
                                curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                previewDataGridView.Rows[curIdx].Height = 4;
                                curIdx = previewDataGridView.Rows.Add();
                            }
                        } else {
                            curRunOrder += 1;
                        }
                        if ( myRound == 1 ) {
                            curNumPlcmt = curRunOrder;
                        } else {
                            if (curPlcmt.Contains( "T" )) {
                                curNumPlcmt = Convert.ToInt32( curPlcmt.Substring( 0, curPlcmt.IndexOf( "T" ) ) );
                            } else {
                                curNumPlcmt = Convert.ToInt32( curPlcmt );
                            }
                        }
                        prevReportGroup = curReportGroup;

                        curViewRow = previewDataGridView.Rows[curIdx];
                        if (curEntryCount > 0) {
                            curFindRows = curEntriesDataTable.Select( "MemberId = '" + (String)curDataRow["MemberId"] + "' AND AgeGroup = '" + (String)curDataRow["AgeGroup"] + "'" );
                            if (curFindRows.Length > 0) {
                                if (curNumSkierSelected < myNumSkiers) {
                                    curNewRoGroup = (String)curFindRows[0]["EventGroup"];
                                    curViewRow.Cells["previewSelected"].Value = true;
                                    curNumSkierSelected++;
                                } else {
                                    if (curNumPlcmt == ( myNumSkiers + 1 )) {
                                        curViewRow = previewDataGridView.Rows[curIdx];
                                        curViewRow.DefaultCellStyle.BackColor = Color.LightCyan;
                                        previewDataGridView.Rows[curIdx].Height = 2;
                                        curIdx = previewDataGridView.Rows.Add();
                                        curViewRow = previewDataGridView.Rows[curIdx];

                                        curNewRoGroup = (String)curFindRows[0]["EventGroup"];
                                        curViewRow.Cells["previewSelected"].Value = false;
                                    } else {
                                        curNewRoGroup = (String)curFindRows[0]["EventGroup"];
                                        curViewRow.Cells["previewSelected"].Value = false;
                                    }
                                }
                            } else {
                                if (curNumPlcmt == ( myNumSkiers + 1 )) {
                                    curViewRow = previewDataGridView.Rows[curIdx];
                                    curViewRow.DefaultCellStyle.BackColor = Color.LightCyan;
                                    previewDataGridView.Rows[curIdx].Height = 2;
                                    curIdx = previewDataGridView.Rows.Add();
                                    curViewRow = previewDataGridView.Rows[curIdx];

                                    curNewRoGroup = "";
                                    curViewRow.Cells["previewSelected"].Value = false;
                                } else {
                                    curNewRoGroup = "";
                                    curViewRow.Cells["previewSelected"].Value = false;
                                }
                            }
                        } else {
                            if (curNumPlcmt <= myNumSkiers && curNumSkierSelected < myNumSkiers) {
                                curViewRow.Cells["previewSelected"].Value = true;
                                curNumSkierSelected++;

                                if (curRunOrder <= myNumSkiers) {
                                    if (curRunOrder > curMaxGroups) {
                                        curNewRoGroup = "HH" + ( myNumSkiers - curRunOrder + 1 ).ToString();
                                    } else {
                                        curNewRoGroup = "HH" + curRunOrder.ToString();
                                    }
                                } else {
                                    curNewRoGroup = "";
                                }

                            } else {
                                curNewRoGroup = "";
                                if (curNumPlcmt == ( myNumSkiers + 1 )) {
                                    curViewRow = previewDataGridView.Rows[curIdx];
                                    curViewRow.DefaultCellStyle.BackColor = Color.LightCyan;
                                    previewDataGridView.Rows[curIdx].Height = 2;
                                    curIdx = previewDataGridView.Rows.Add();
                                    curViewRow = previewDataGridView.Rows[curIdx];
                                    curViewRow.Cells["previewSelected"].Value = false;
                                } else {
                                    curViewRow.Cells["previewSelected"].Value = false;
                                }
                            }
                        }
                        curViewRow.Cells["previewSanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["previewMemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["previewSkierName"].Value = (String)curDataRow["SkierName"];
                        curViewRow.Cells["previewEvent"].Value = myEvent;
                        curViewRow.Cells["previewAgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        curViewRow.Cells["previewRound"].Value = myRound.ToString( "0" );
                        curViewRow.Cells["previewSeed"].Value = curRunOrder.ToString();
                        curViewRow.Cells["previewOrder"].Value = curRunOrder.ToString();
						curViewRow.Cells["PreviewScore"].Value = curDataRow[curScoreName].ToString();
						curViewRow.Cells["previewPlcmt"].Value = curPlcmt;
                        curViewRow.Cells["previewEventGroup"].Value = curGroup;
						curViewRow.Cells["previewRunOrderGroup"].Value = curNewRoGroup;
					}
					Cursor.Current = Cursors.Default;
                    try {
                        int curRowPos = curIdx + 1;
                        RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + previewDataGridView.Rows.Count.ToString();
                    } catch {
                        RowStatusLabel.Text = "";
                    }

                    curIdx = 0;
                    previewDataGridView.CurrentCell = previewDataGridView.Rows[curIdx].Cells["previewSkierName"];
                }

            } catch (Exception excp) {
                MessageBox.Show( "Error attempting to prepare for head to head selection \n" + excp.Message );
            }
        }

        private void loadDataGridViewH2HN() {
            String curMethodName = "Tournament:RunOrderElimDialogForm:loadDataGridViewH2HN";
            String curDiv = "", curGroup = "", curReportGroup = "", prevReportGroup = "", curNewRoGroup = "";
            int curIdx = 0, curRunOrder = 0;
            DataTable curDataTable = null;

			if ( myPlcmtOrg.ToLower().Equals( "div" ) ) {
				myDivFilter = EventGroupList.SelectedItem.ToString();
				myGroupFilter = "All";

			} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
				myGroupFilter = EventGroupList.SelectedItem.ToString();
				myDivFilter = "All";

			} else {
				myDivFilter = null;
				myGroupFilter = null;
			}

			try {
                curDataTable = getEventScoresH2HN();

                int curMaxGroups = Convert.ToInt32( myNumSkiers ) >> 1;

                previewDataGridView.Rows.Clear();
                if (curDataTable.Rows.Count > 0) {
                    Cursor.Current = Cursors.WaitCursor;
                    DataGridViewRow curViewRow;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        curIdx = previewDataGridView.Rows.Add();

						curReportGroup = "";
						curDiv = (String)curDataRow["AgeGroup"];
						curGroup = (String) curDataRow["EventGroup"];
						if ( myPlcmtOrg.ToLower().Equals( "div" )) {
                            curReportGroup = curDiv;

						} else if ( myPlcmtOrg.ToLower().Equals( "group" ) ) {
							curReportGroup = curGroup;
						}

						if ( curReportGroup != prevReportGroup) {
                            curRunOrder = 1;
                            if (prevReportGroup.Length > 0) {
                                curViewRow = previewDataGridView.Rows[curIdx];
                                curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                previewDataGridView.Rows[curIdx].Height = 4;
                                curIdx = previewDataGridView.Rows.Add();
                            }
                        } else {
                            curRunOrder += 1;
                        }
                        prevReportGroup = curReportGroup;

                        curViewRow = previewDataGridView.Rows[curIdx];
                        if ((byte)curDataRow["Selected"] == 1) {
                            curViewRow.Cells["previewSelected"].Value = true;
                            if (curRunOrder > curMaxGroups) {
                                curNewRoGroup = "HH" + ( myNumSkiers - curRunOrder + 1 ).ToString();
                            } else {
                                curNewRoGroup = "HH" + curRunOrder.ToString();
                            }
                        } else {
                            curNewRoGroup = "";
                            curViewRow.Cells["previewSelected"].Value = false;
                        }

                        curViewRow.Cells["previewSanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["previewMemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["previewSkierName"].Value = (String)curDataRow["SkierName"];
                        curViewRow.Cells["previewEvent"].Value = myEvent;
                        curViewRow.Cells["previewAgeGroup"].Value = (String)curDataRow["AgeGroup"];

                        curViewRow.Cells["previewRound"].Value = myRound.ToString( "0" );
                        try {
							curViewRow.Cells["previewEventGroup"].Value = curGroup;
							curViewRow.Cells["previewRunOrderGroup"].Value = curNewRoGroup;
						} catch {
                            curViewRow.Cells["previewEventGroup"].Value = "";
							curViewRow.Cells["previewRunOrderGroup"].Value = curNewRoGroup;
						}
						curViewRow.Cells["previewSeed"].Value = ( (Int16)curDataRow["Seed"] ).ToString();
                        curViewRow.Cells["previewOrder"].Value = ( (Int16)curDataRow["Seed"] ).ToString();
                        curViewRow.Cells["PreviewScore"].Value = curDataRow["Score"].ToString();

                        curViewRow.Cells["previewPlcmt"].Value = "";
                    }
                    Cursor.Current = Cursors.Default;
                    try {
                        int curRowPos = curIdx + 1;
                        RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + previewDataGridView.Rows.Count.ToString();
                    } catch {
                        RowStatusLabel.Text = "";
                    }

                    curIdx = 0;
                    previewDataGridView.CurrentCell = previewDataGridView.Rows[curIdx].Cells["previewSkierName"];
                }

            } catch (Exception excp) {
                MessageBox.Show( "Error attempting to prepare for head to head selection \n" + excp.Message );
            }
        }

		private void loadDataGridViewPick() {
			String curMethodName = "Tournament:RunOrderElimDialogForm:loadDataGridViewPick";
			String curGroup = "", curDiv = "", curPlcmt = "";
			int curIdx = 0, curGroupNumSkiers = 0;
			DataTable curDataTable = null;

			try {
				Cursor.Current = Cursors.WaitCursor;
				curDataTable = getEventRegData( "All", "All" );

				previewDataGridView.Rows.Clear();
				if ( curDataTable.Rows.Count > 0 ) {
					curGroupNumSkiers = curDataTable.Rows.Count;
					curIdx = 0;
					DataGridViewRow curViewRow;
					foreach ( DataRow curDataRow in curDataTable.Rows ) {
						curIdx = previewDataGridView.Rows.Add();
						curViewRow = previewDataGridView.Rows[curIdx];

						curDiv = (String) curDataRow["AgeGroup"];
						curGroup = (String) curDataRow["EventGroup"];
						curViewRow.Cells["previewSanctionId"].Value = (String) curDataRow["SanctionId"];
						curViewRow.Cells["previewMemberId"].Value = (String) curDataRow["MemberId"];
						curViewRow.Cells["previewSkierName"].Value = (String) curDataRow["SkierName"];
						curViewRow.Cells["previewEvent"].Value = myEvent;
						curViewRow.Cells["previewAgeGroup"].Value = (String) curDataRow["AgeGroup"];
						curViewRow.Cells["previewEventGroup"].Value = (String) curDataRow["EventGroup"];
						curViewRow.Cells["previewRound"].Value = myRound.ToString( "0" );
						curViewRow.Cells["previewSeed"].Value = curDataRow["RunOrder"].ToString();
						curViewRow.Cells["previewOrder"].Value = curDataRow["RunOrder"].ToString();
						curViewRow.Cells["PreviewScore"].Value = curDataRow["RankingScore"].ToString();
						curViewRow.Cells["previewPlcmt"].Value = curPlcmt;
						curViewRow.Cells["previewSelected"].Value = true;
						if (curDataRow["RunOrderMember"] == System.DBNull.Value) {
							curViewRow.Cells["previewSelected"].Value = false;
						}
					}
					try {
						int curRowPos = curIdx + 1;
						RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + previewDataGridView.Rows.Count.ToString();
					} catch {
						RowStatusLabel.Text = "";
					}

					curIdx = 0;
					previewDataGridView.CurrentCell = previewDataGridView.Rows[curIdx].Cells["previewSkierName"];
				}
				Cursor.Current = Cursors.Default;

			} catch ( Exception excp ) {
				MessageBox.Show( "Error attempting to prepare for pick and choose selection \n" + excp.Message );
			}
		}

		private void loadGroupList() {
			if ( groupPlcmtButton.Checked ) {
				loadEventGroupListFromData();

			} else if ( plcmtDivButton.Checked ) {
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
				}

			}
		}

        private void loadAgeGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT AgeGroup FROM EventReg "
                + "WHERE SanctionId = '" + (String)myTourRow["SanctionId"] + "' And Event = 'Slalom' "
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

		private void loadEventGroupListFromData() {
			String curGroupValue = "";
			ArrayList curEventGroupList = new ArrayList();
			String curSqlStmt = "";
			curEventGroupList.Add( "All" );
			curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
				+ "WHERE SanctionId = '" + (String) myTourRow["SanctionId"] + "' And Event = '" + this.myEvent + "' "
				+ "Order by EventGroup";
			DataTable curDataTable = getData( curSqlStmt );

			foreach ( DataRow curRow in curDataTable.Rows ) {
				curEventGroupList.Add( (String) curRow["EventGroup"] );
			}
			EventGroupList.DataSource = curEventGroupList;
			if ( curGroupValue.Length > 0 ) {
				foreach ( String curValue in (ArrayList) EventGroupList.DataSource ) {
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

		private void SelectButton_Click(object sender, EventArgs e) {
            if (myCommand.Equals( "HeadToHead" )) {
                insertHeadToHeadOrder();
            } else if (myCommand.Equals( "HeadToHeadNext" )) {
                insertHeadToHeadOrder();
            } else if (myCommand.Equals( "Elim" )) {
                insertElimSelect();
			} else if ( myCommand.Equals( "Pick" ) ) {
				insertPickAndChooseSelect();
			}
		}

        private void insertHeadToHeadOrder() {
            String curMethodName = "Tournament:RunningOrderRound:insertHeadToHeadOrder";
            String curMsg = "";
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                try {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventRunOrder " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + (String)myTourRow["SanctionId"] + "' AND Event = '" + myEvent + "' AND Round = " + myRound.ToString() );
					if ( myDivFilter != null ) {
						if ( myDivFilter.Length > 0 && !( myDivFilter.ToLower().Equals( "all" ) ) ) {
							curSqlStmt.Append( " AND AgeGroup = '" + myDivFilter + "' " );
						}
					}
					if ( myGroupFilter != null ) {
						if ( myGroupFilter.Length > 0 && !( myGroupFilter.ToLower().Equals( "all" ) ) ) {
							curSqlStmt.Append( " AND EventGroup = '" + myDivFilter + "' " );
						}
					}
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                } catch (Exception excp) {
                    curMsg = "Error attempting to remove existing head to head running order entries before re-adding  \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }

                foreach (DataGridViewRow curViewRow in previewDataGridView.Rows) {
                    if (curViewRow.Cells["previewSanctionId"].Value != null) {
                        if ((bool)curViewRow.Cells["previewSelected"].Value) {
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Insert EventRunOrder ( " );
                            curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, EventGroup, RunOrderGroup, Event, Round, RunOrder, RankingScore, LastUpdateDate, Notes " );
                            curSqlStmt.Append( ") Values ( " );
                            curSqlStmt.Append( "'" + (String)curViewRow.Cells["previewSanctionId"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewMemberId"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewAgeGroup"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewEventGroup"].Value + "'" );
							curSqlStmt.Append( ", '" + (String) curViewRow.Cells["previewRunOrderGroup"].Value + "'" );
							curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewEvent"].Value + "'" );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["previewRound"].Value );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["previewSeed"].Value );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["PreviewScore"].Value );
                            curSqlStmt.Append( ", GETDATE(), '' " );
                            curSqlStmt.Append( ")" );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }
                    }
                }
            } catch (Exception excp) {
                curMsg = "Error attempting to generate head to head running order \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void insertElimSelect() {
            String curMethodName = "Tournament:RunningOrderRound:insertElimSelect";
            String curMsg = "";
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                try {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete EventRunOrder " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + (String)myTourRow["SanctionId"] + "' AND Event = '" + myEvent + "' AND Round = " + myRound.ToString() );
                    if (myDivFilter != null) {
                        if (myDivFilter.Length > 0 && !(myDivFilter.ToLower().Equals("all")) ) {
                            curSqlStmt.Append( " AND AgeGroup = '" + myDivFilter + "' " );
                        }
                    }
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                } catch (Exception excp) {
                    curMsg = "Error attempting to remove existing running order entries before re-adding  \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }

                foreach (DataGridViewRow curViewRow in previewDataGridView.Rows) {
                    if (curViewRow.Cells["previewSanctionId"].Value != null) {
                        if ((bool)curViewRow.Cells["previewSelected"].Value) {
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Insert EventRunOrder ( " );
                            curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, EventGroup, Event, Round, RunOrder, RankingScore, LastUpdateDate, Notes " );
                            curSqlStmt.Append( ") Values ( " );
                            curSqlStmt.Append( "'" + (String)curViewRow.Cells["previewSanctionId"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewMemberId"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewAgeGroup"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewEventGroup"].Value + "'" );
                            curSqlStmt.Append( ", '" + (String)curViewRow.Cells["previewEvent"].Value + "'" );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["previewRound"].Value );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["previewOrder"].Value );
                            curSqlStmt.Append( ", " + (String)curViewRow.Cells["PreviewScore"].Value );
                            curSqlStmt.Append( ", GETDATE(), '' " );
                            curSqlStmt.Append( ")" );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }
                    }

                }
            } catch (Exception excp) {
                curMsg = "Error attempting to generate head to head running order \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

		private void insertPickAndChooseSelect() {
			String curMethodName = "Tournament:RunningOrderRound:insertPickAndChooseSelect";
			String curMsg = "";
			int rowsProc = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );

			try {
				try {
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Delete EventRunOrder " );
					curSqlStmt.Append( "WHERE SanctionId = '" + (String) myTourRow["SanctionId"] + "' AND Event = '" + myEvent + "' AND Round = " + myRound.ToString() );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				} catch ( Exception excp ) {
					curMsg = "Error attempting to remove existing running order entries before re-adding  \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
				}

				foreach ( DataGridViewRow curViewRow in previewDataGridView.Rows ) {
					if ( curViewRow.Cells["previewSanctionId"].Value != null ) {
						if ( (bool) curViewRow.Cells["previewSelected"].Value ) {
							curSqlStmt = new StringBuilder( "" );
							curSqlStmt.Append( "Insert EventRunOrder ( " );
							curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, EventGroup, Event, Round, RunOrder, RankingScore, LastUpdateDate, Notes " );
							curSqlStmt.Append( ") Values ( " );
							curSqlStmt.Append( "'" + (String) curViewRow.Cells["previewSanctionId"].Value + "'" );
							curSqlStmt.Append( ", '" + (String) curViewRow.Cells["previewMemberId"].Value + "'" );
							curSqlStmt.Append( ", '" + (String) curViewRow.Cells["previewAgeGroup"].Value + "'" );
							curSqlStmt.Append( ", '" + (String) curViewRow.Cells["previewEventGroup"].Value + "'" );
							curSqlStmt.Append( ", '" + (String) curViewRow.Cells["previewEvent"].Value + "'" );
							curSqlStmt.Append( ", " + (String) curViewRow.Cells["previewRound"].Value );
							curSqlStmt.Append( ", " + (String) curViewRow.Cells["previewOrder"].Value );
							curSqlStmt.Append( ", " + (String) curViewRow.Cells["PreviewScore"].Value );
							curSqlStmt.Append( ", GETDATE(), '' " );
							curSqlStmt.Append( ")" );
							rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
							Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
						}
					}

				}
			} catch ( Exception excp ) {
				curMsg = "Error attempting to generate head to head running order \n" + excp.Message;
				MessageBox.Show( curMsg );
				Log.WriteFile( curMethodName + curMsg );
			}
		}

		private DataTable getEventRegData(String inDivFilter, String inGroupFilter ) {
            String curPlcmtName = "Plcmt" + myEvent;
            String curRoundName = "Round" + myEvent;
            String curGroupName = "EventGroup" + myEvent;
            String curScoreName = "Score" + myEvent;
			String curPointsName = "Points" + myEvent;

			StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT E.Event, E.SanctionId, E.MemberId, O.MemberId as RunOrderMember, T.SkierName, E.EventGroup, E.RunOrder, ");
            curSqlStmt.Append( "E.TeamCode, E.EventClass, E.RankingScore, E.RankingRating, E.AgeGroup, '" + myRound.ToString() + "' as Round " );
            curSqlStmt.Append( ", CONVERT(nvarchar(3), E.RunOrder) AS PlcmtGroup, CONVERT(nvarchar(3), E.RunOrder) AS PlcmtTour " );
            curSqlStmt.Append( ", E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder " );
            curSqlStmt.Append( ", E.RankingScore AS " + curScoreName + ", (E.RankingScore + E.HCapScore) AS " + curPointsName );
            curSqlStmt.Append( ", " + myRound.ToString() + " AS " + curRoundName );
            curSqlStmt.Append( ", '0' AS " + curPlcmtName + ", '' AS " + curGroupName + ", COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt " );
            curSqlStmt.Append( " FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
			curSqlStmt.Append( "     LEFT OUTER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND O.Event = E.Event AND O.Round = " + myRound + " ");
			curSqlStmt.Append( "WHERE E.SanctionId = '" + (String)myTourRow["SanctionId"] + "' AND E.Event = '" + myEvent + "' " );
            if (inDivFilter == null || inDivFilter.ToLower().Equals( "all" )) {
            } else {
                curSqlStmt.Append( " AND E.AgeGroup = '" + inDivFilter + "' " );
            }
			if ( inGroupFilter == null || inGroupFilter.ToLower().Equals( "all" ) ) {
			} else {
				curSqlStmt.Append( " AND E.EventGroup = '" + inGroupFilter + "' " );
			}
			if (myCommand.Equals("Pick")) {
				curSqlStmt.Append("Order by " + mySortCmd);

			} else if ( groupPlcmtButton.Checked) {
                curSqlStmt.Append( "Order by E.EventGroup, E.RunOrder, E.RankingScore DESC" );
            } else if (plcmtTourButton.Checked) {
                curSqlStmt.Append( "Order by E.RunOrder, E.RankingScore DESC" );
            } else if (plcmtDivButton.Checked) {
                curSqlStmt.Append( "Order by COALESCE(D.RunOrder, 999), E.AgeGroup, E.RunOrder, E.RankingScore DESC" );
            } else if (plcmtDivGrpButton.Checked) {
                curSqlStmt.Append( "Order by COALESCE(D.RunOrder, 999), E.AgeGroup, E.EventGroup, E.RunOrder, E.RankingScore DESC" );
            } else {
				curSqlStmt.Append( "Order by COALESCE(D.RunOrder, 999), E.AgeGroup, E.RunOrder, E.RankingScore DESC" );
            }

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getEventScoresH2HN() {

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventClass" );
            curSqlStmt.Append( ", S.Round, O.EventGroup, O.RunOrderGroup, O.RunOrder, E.TeamCode, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder" );
            if (myEvent.Equals( "Slalom" )) {
                curSqlStmt.Append( ", S.Score, S.NopsScore, E.HCapScore, S.Score + E.HCapScore as HCapScoreTotal " );
            } else if (myEvent.Equals( "Trick" )) {
                curSqlStmt.Append( ", S.Score, S.NopsScore, E.HCapScore, S.Score + E.HCapScore as HCapScoreTotal " );
            } else if (myEvent.Equals( "Jump" )) {
                curSqlStmt.Append( ", S.ScoreFeet, S.ScoreMeters as Score, S.NopsScore, E.HCapScore, S.ScoreFeet + E.HCapScore as HCapScoreTotal " );
            }
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            if (myEvent.Equals( "Slalom" )) {
                curSqlStmt.Append( "     INNER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup " );
            } else if (myEvent.Equals( "Trick" )) {
                curSqlStmt.Append( "     INNER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup " );
            } else if (myEvent.Equals( "Jump" )) {
                curSqlStmt.Append( "     INNER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup " );
            }
            curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND S.Round = O.Round " );
            curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + (String)myTourRow["SanctionId"] + "' AND E.Event = '" + myEvent + "' " );
            curSqlStmt.Append( "AND S.Round = " + (myRound - 1).ToString() + " ");

			if ( plcmtDivButton.Checked ) {
				if ( myDivFilter != null ) {
					if ( myDivFilter.Length > 0 && !( myDivFilter.ToLower().Equals( "all" ) ) ) {
						curSqlStmt.Append( " AND E.AgeGroup = '" + myDivFilter + "' " );
					}
				}

				if ( pointsScoreButton.Checked ) {
					if ( handicapPointsButton.Checked ) {
						if ( myEvent.Equals( "Slalom" ) ) {
							curSqlStmt.Append( "Order by E.AgeGroup, (S.Score + E.HCapScore) DESC " );
						} else if ( myEvent.Equals( "Trick" ) ) {
							curSqlStmt.Append( "Order by E.AgeGroup, (S.Score + E.HCapScore) DESC " );
						} else if ( myEvent.Equals( "Jump" ) ) {
							curSqlStmt.Append( "Order by E.AgeGroup, (S.ScoreMeters + E.HCapScore) DESC " );
						}
					} else {
						curSqlStmt.Append( "Order by E.AgeGroup,, S.NopsScore DESC " );
					}

				} else {
					if ( myEvent.Equals( "Slalom" ) ) {
						curSqlStmt.Append( "Order by E.AgeGroup,, S.Score DESC " );
					} else if ( myEvent.Equals( "Trick" ) ) {
						curSqlStmt.Append( "Order by E.AgeGroup,, S.Score DESC " );
					} else if ( myEvent.Equals( "Jump" ) ) {
						curSqlStmt.Append( "Order by E.AgeGroup,, S.ScoreFeet DESC, S.ScoreMeters DESC " );
					}
				}

			} else if ( groupPlcmtButton.Checked ) {
				if ( myGroupFilter != null ) {
					if ( myGroupFilter.Length > 0 && !( myGroupFilter.ToLower().Equals( "all" ) ) ) {
						curSqlStmt.Append( " AND E.EventGroup = '" + myGroupFilter + "' " );
					}
				}

				if ( pointsScoreButton.Checked ) {
					if ( handicapPointsButton.Checked ) {
						if ( myEvent.Equals( "Slalom" ) ) {
							curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, (S.Score + E.HCapScore) DESC " );
						} else if ( myEvent.Equals( "Trick" ) ) {
							curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, (S.Score + E.HCapScore) DESC " );
						} else if ( myEvent.Equals( "Jump" ) ) {
							curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, (S.ScoreMeters + E.HCapScore) DESC " );
						}
					} else {
						curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, S.NopsScore DESC " );
					}

				} else {
					if ( myEvent.Equals( "Slalom" ) ) {
						curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, S.Score DESC " );
					} else if ( myEvent.Equals( "Trick" ) ) {
						curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, S.Score DESC " );
					} else if ( myEvent.Equals( "Jump" ) ) {
						curSqlStmt.Append( "Order by O.EventGroup, O.RunOrderGroup, S.ScoreFeet DESC, S.ScoreMeters DESC " );
					}
				}

			} else {
                if (pointsScoreButton.Checked) {
                    if (handicapPointsButton.Checked) {
                        if (myEvent.Equals( "Slalom" )) {
                            curSqlStmt.Append( "Order by O.EventGroup, (S.Score + E.HCapScore) DESC, O.RunOrder " );
                        } else if (myEvent.Equals( "Trick" )) {
                            curSqlStmt.Append( "Order by O.EventGroup, (S.Score + E.HCapScore) DESC, O.RunOrder " );
                        } else if (myEvent.Equals( "Jump" )) {
                            curSqlStmt.Append( "Order by O.EventGroup, (S.ScoreFeet = E.HCapScore) DESC, (S.ScoreMeters + E.HCapScore) DESC, O.RunOrder " );
                        }
                    } else {
                        curSqlStmt.Append( "Order by O.EventGroup, S.NopsScore DESC, O.RunOrder " );
                    }

				} else {
                    if (myEvent.Equals( "Slalom" )) {
                        curSqlStmt.Append( "Order by O.EventGroup, S.Score DESC, O.RunOrder " );
                    } else if (myEvent.Equals( "Trick" )) {
                        curSqlStmt.Append( "Order by O.EventGroup, S.Score DESC, O.RunOrder " );
                    } else if (myEvent.Equals( "Jump" )) {
                        curSqlStmt.Append( "Order by O.EventGroup, S.ScoreFeet DESC, S.ScoreMeters DESC, O.RunOrder " );
                    }
                }
            }

            DataTable curDataTable = getData( curSqlStmt.ToString() );
            DataTable curPlcmtTable = buildPlcmtList();
            DataRowView newSkierScoreRow;
            DataRow curDataRow1, curDataRow2, curDataRowSkier1, curDataRowSkier2;
            String curScoreAttr = "";

            if (curDataTable.Rows.Count > 0) {
                int curIdxSkier1 = 0, curIdxSkier2 = 1;
                Int16 curDivCount = 0, curDivSkierCount = 0, curDivSkierCountMax = 0, curBracket = 1;
                String curDiv = "", prevDiv = "";
                if (pointsScoreButton.Checked) {
					if ( nopsPointsButton.Checked || kBasePointsButton.Checked ) {
						curScoreAttr = "NopsScore";
					} else if ( ratioPointsButton.Checked ) {
						curScoreAttr = "HCapScoreTotal";
					} else if ( handicapPointsButton.Checked ) {
						curScoreAttr = "HCapScoreTotal";
					} else {
						curScoreAttr = "NopsScore";
					}
				} else {
                    curScoreAttr = "Score";
                }

                while (curIdxSkier1 < curDataTable.Rows.Count) {
                    curDataRowSkier1 = curDataTable.Rows[curIdxSkier1];
                    curDataRowSkier2 = curDataTable.Rows[curIdxSkier2];
                    curIdxSkier1 += 2;
                    curIdxSkier2 += 2;

                    if (myEvent.Equals( "Trick" ) && rawScoreButton.Checked) {
                        if ((Int16)curDataRowSkier1[curScoreAttr] > (Int16)curDataRowSkier2[curScoreAttr]) {
                            curDataRow1 = curDataRowSkier1;
                            curDataRow2 = curDataRowSkier2;
                        } else if ((Int16)curDataRowSkier1[curScoreAttr] < (Int16)curDataRowSkier2[curScoreAttr]) {
                            curDataRow1 = curDataRowSkier2;
                            curDataRow2 = curDataRowSkier1;
                        } else {
                            curDataRow1 = curDataRowSkier1;
                            curDataRow2 = curDataRowSkier2;
                        }
                    } else {
                        if ((Decimal)curDataRowSkier1[curScoreAttr] > (Decimal)curDataRowSkier2[curScoreAttr]) {
                            curDataRow1 = curDataRowSkier1;
                            curDataRow2 = curDataRowSkier2;
                        } else if ((Decimal)curDataRowSkier1[curScoreAttr] < (Decimal)curDataRowSkier2[curScoreAttr]) {
                            curDataRow1 = curDataRowSkier2;
                            curDataRow2 = curDataRowSkier1;
                        } else {
                            curDataRow1 = curDataRowSkier1;
                            curDataRow2 = curDataRowSkier2;
                        }
                    }

                    newSkierScoreRow = curPlcmtTable.DefaultView.AddNew();
                    newSkierScoreRow["SanctionId"] = (String)curDataRow1["SanctionId"];
                    newSkierScoreRow["MemberId"] = (String)curDataRow1["MemberId"];
                    newSkierScoreRow["SkierName"] = (String)curDataRow1["SkierName"];
                    newSkierScoreRow["AgeGroup"] = (String)curDataRow1["AgeGroup"];
					newSkierScoreRow["EventGroup"] = (String) curDataRow1["EventGroup"];
					newSkierScoreRow["RunOrderGroup"] = (String) curDataRow1["RunOrderGroup"];
					newSkierScoreRow["Event"] = (String)curDataRow1["Event"];
                    newSkierScoreRow["Round"] = (Byte)curDataRow1["Round"];
                    newSkierScoreRow["Seed"] = (Int16)curDataRow1["RunOrder"];

                    if (pointsScoreButton.Checked) {
						newSkierScoreRow["Score"] = (Decimal) curDataRow1["HCapScoreTotal"];
                    } else {
                        if (myEvent.Equals( "Trick" )) {
                            newSkierScoreRow["Score"] = Convert.ToDecimal( (Int16)curDataRow1["Score"] );
                        } else {
                            newSkierScoreRow["Score"] = (Decimal)curDataRow1["Score"];
                        }
                    }

                    newSkierScoreRow["NopsScore"] = (Decimal)curDataRow1["NopsScore"];
                    newSkierScoreRow["Selected"] = 1;
                    if (reseedButton.Checked) {
                        newSkierScoreRow["Bracket"] = 0;
                    } else if (bracketButton.Checked) {
                        newSkierScoreRow["Bracket"] = curBracket;
                    }
                    newSkierScoreRow.EndEdit();

                    newSkierScoreRow = curPlcmtTable.DefaultView.AddNew();
                    newSkierScoreRow["SanctionId"] = (String)curDataRow2["SanctionId"];
                    newSkierScoreRow["MemberId"] = (String)curDataRow2["MemberId"];
                    newSkierScoreRow["SkierName"] = (String)curDataRow2["SkierName"];
                    newSkierScoreRow["AgeGroup"] = (String)curDataRow2["AgeGroup"];
					newSkierScoreRow["EventGroup"] = (String) curDataRow2["EventGroup"];
					newSkierScoreRow["RunOrderGroup"] = (String) curDataRow2["RunOrderGroup"];
					newSkierScoreRow["Event"] = (String)curDataRow2["Event"];
                    newSkierScoreRow["Round"] = (Byte)curDataRow2["Round"];
                    newSkierScoreRow["Seed"] = (Int16)curDataRow2["RunOrder"];
					if ( pointsScoreButton.Checked ) {
						newSkierScoreRow["Score"] = (Decimal) curDataRow2["HCapScoreTotal"];
					} else {
                        if (myEvent.Equals( "Trick" )) {
                            newSkierScoreRow["Score"] = Convert.ToDecimal( (Int16)curDataRow2["Score"] );
                        } else {
                            newSkierScoreRow["Score"] = (Decimal)curDataRow2["Score"];
                        }
                    }

                    newSkierScoreRow["NopsScore"] = (Decimal)curDataRow2["NopsScore"];
                    newSkierScoreRow["Selected"] = 0;
                    newSkierScoreRow["Bracket"] = 0;
                    newSkierScoreRow.EndEdit();

                    if (plcmtDivButton.Checked) {
                        //curDiv = (String)curDataRow1["AgeGroup"];
                        if (curDiv == prevDiv) {
                            //curDivSkierCount++;
                        } else {
                            //curDivCount++;
                            //if (curDivSkierCount > curDivSkierCountMax) {
                            //    curDivSkierCountMax = curDivSkierCount;
                            //}
                            //curDivSkierCount = 1;
                            curBracket = 0;
                        }
                        prevDiv = curDiv;
                    }
                    curBracket++;
                }

                /*
                 */
                if (plcmtTourButton.Checked) {
                    //myNumSkiers = Convert.ToInt16( curDataTable.Rows.Count >> 1 );
                } else {
                    curDivCount++;
                    if (curDivSkierCount > curDivSkierCountMax) {
                        curDivSkierCountMax = curDivSkierCount;
                    }
                    //myNumSkiers = curDivSkierCountMax;
                }
                //NumSkiersTextbox.Text = myNumSkiers.ToString();

                if (reseedButton.Checked) {
					if ( plcmtDivButton.Checked ) {
						curPlcmtTable.DefaultView.Sort = "AgeGroup DESC, Selected DESC, Seed ASC";
					} else if ( groupPlcmtButton.Checked ) {
						curPlcmtTable.DefaultView.Sort = "EventGroup DESC, Selected DESC, Seed ASC";
					} else {
                        curPlcmtTable.DefaultView.Sort = "Selected DESC, Seed ASC";
                    }

				} else if (bracketButton.Checked) {
                    if (plcmtDivButton.Checked) {
                        curPlcmtTable.DefaultView.Sort = "AgeGroup DESC, Selected DESC, Bracket DESC, Seed ASC";
					} else if ( groupPlcmtButton.Checked ) {
						curPlcmtTable.DefaultView.Sort = "EventGroup DESC, Selected DESC, Bracket DESC, Seed ASC";
					} else {
                        curPlcmtTable.DefaultView.Sort = "Selected DESC, Bracket DESC, Seed ASC";
                    }
                }
                return curPlcmtTable.DefaultView.ToTable();
            }

            return curPlcmtTable;
        }

        private DataTable getCurrentRoundEntries(String inDivFilter, String inGroupFilter ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT EO.PK, EO.SanctionId, EO.MemberId, EO.AgeGroup, EO.EventGroup, EO.Event, EO.Round, EO.RunOrder, TR.SkierName " );
            curSqlStmt.Append( "FROM EventRunOrder AS EO " );
            curSqlStmt.Append( "INNER JOIN TourReg AS TR ON TR.SanctionId = EO.SanctionId AND TR.MemberId = EO.MemberId AND TR.AgeGroup = EO.AgeGroup " );
            curSqlStmt.Append( "WHERE EO.SanctionId = '" + (String)myTourRow["SanctionId"] + "' AND EO.Event = '" + myEvent + "' and EO.Round = " + myRound.ToString() );
            if (inDivFilter == null) {
            } else if (inDivFilter.ToLower().Equals( "all" )) {
            } else {
                curSqlStmt.Append( " AND EO.AgeGroup = '" + inDivFilter + "' " );
            }
			if ( inGroupFilter == null || inGroupFilter.ToLower().Equals( "all" ) ) {
			} else {
				curSqlStmt.Append( " AND EO.EventGroup = '" + inGroupFilter + "' " );
			}
			return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private DataTable buildPlcmtList() {
            /* **********************************************************
             * Build data tabale definition containing the data required to 
             * resolve placement ties based on initial event score
             * ******************************************************* */
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "SanctionId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "SkierName";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "AgeGroup";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "EventGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
			curCol.ColumnName = "RunOrderGroup";
			curCol.DataType = System.Type.GetType( "System.String" );
			curCol.AllowDBNull = false;
			curCol.ReadOnly = false;
			curCol.DefaultValue = "";
			curDataTable.Columns.Add( curCol );

			curCol = new DataColumn();
            curCol.ColumnName = "Event";
            curCol.DataType = System.Type.GetType( "System.String" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = "";
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Round";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Bracket";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Seed";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Score";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "NopsScore";
            curCol.DataType = System.Type.GetType( "System.Decimal" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "Selected";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
        }

    }
}
