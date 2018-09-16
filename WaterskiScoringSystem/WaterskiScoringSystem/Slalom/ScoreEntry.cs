using System;
using System.Deployment.Application;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Slalom {
    public partial class ScoreEntry : Form {
        #region instance variables
        private bool myWindowFocus = true;
        private bool isDataModified = false;
        private bool isLoadInProg = false;
        private bool isAddRecapRowInProg = false;
        private bool isRecapRowEnterHandled = false;
        private bool myGateValueChg = false;
		private bool isLastPassSelectActive = false;

		private int myEventRegViewIdx = 0;
        private int myStartCellIndex = 0;
        private int myBoatListIdx = 0;
        private int mySkierRunCount = 0;
        private int myPassRunCount = 0;
        private int myEventDelaySeconds = 0;

        private Int16 myNumJudges;
        
        private DateTime myEventStartTime;
        private DateTime myEventDelayStartTime;
        private String myOrigCellValue = "";
        private String mySortCommand = "";
        private String myTourClass;
        private String myFilterCmd = "";
        private String myModCellValue = "";
        private String mySanctionNum;
        private String myRecapColumn;
        private String myTourRules;
        private String myLastPassSelectValue = "";
		private String myPrevEventGroup = "";

        private DataGridViewRow myRecapRow;
        private DataRow myTourRow;
        private DataRow myScoreRow;
        private DataRow myPassRow;
        private DataRow myTimeRow;
        private DataRow myMinSpeedRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;

        private TourProperties myTourProperties;
        private DataTable myTourEventRegDataTable;
        private DataTable myScoreDataTable;
        private DataTable myRecapDataTable;
        private DataTable myTimesDataTable;
        private DataTable myPassDataTable;

        private ListSkierClass mySkierClassList;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private RerideReason rerideReasonDialogForm;
        private SkierDoneReason skierDoneReasonDialogForm;
        private SlalomOptUp OptUpDialogForm;
        private CalcNops appNopsCalc;
		private CheckOfficials myCheckOfficials;
		private NextPassDialog myNextPassDialog;

		private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private CheckEventRecord myCheckEventRecord;
        private DataTable myDivisionIntlDataTable = null;
        private DataTable myDivisionAwsaJuniorDataTable = null;
        #endregion

        public ScoreEntry() {
            InitializeComponent();

            appNopsCalc = CalcNops.Instance;
            appNopsCalc.LoadDataForTour();
            ScoreList.Add( new Common.ScoreEntry( "Slalom", 0, "", 0 ) );

        }

        private void ScoreEntry_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.SlalomEntry_Width > 0) {
                this.Width = Properties.Settings.Default.SlalomEntry_Width;
            }
            if (Properties.Settings.Default.SlalomEntry_Height > 0) {
                this.Height = Properties.Settings.Default.SlalomEntry_Height;
            }
            if (Properties.Settings.Default.SlalomEntry_Location.X > 0
                && Properties.Settings.Default.SlalomEntry_Location.Y > 0) {
                this.Location = Properties.Settings.Default.SlalomEntry_Location;
            }
            myTourProperties = TourProperties.Instance;
            mySortCommand = myTourProperties.RunningOrderSortSlalom;
            int curDelim = mySortCommand.IndexOf( "AgeGroup" );
            if (curDelim < 0) {
            } else if (curDelim > 0) {
                mySortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
                myTourProperties.RunningOrderSortSlalom = mySortCommand;
            } else {
                mySortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
                myTourProperties.RunningOrderSortSlalom = mySortCommand;
            }

            ResizeNarrow.Visible = false;
            ResizeNarrow.Enabled = false;
            ResizeWide.Visible = true;
            ResizeWide.Enabled = true;
            ResizeNarrow.Location = ResizeWide.Location;

            EventDelayReasonTextBox.Visible = false;
            EventDelayReasonLabel.Visible = false;
            StartTimerButton.Visible = false;
            PauseTimerButton.Visible = true;
            StartTimerButton.Location = PauseTimerButton.Location;

            String[] curList = { "SkierName", "Div", "DivOrder", "EventGroup", "RunOrder", "ReadyForPlcmt", "TeamCode", "EventClass", "RankingScore", "RankingRating", "HCapBase", "HCapScore", "Status" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            rerideReasonDialogForm = new RerideReason();
            skierDoneReasonDialogForm = new SkierDoneReason();
            OptUpDialogForm = new SlalomOptUp();

			myNextPassDialog = new NextPassDialog();
			myNextPassDialog.StartPosition = FormStartPosition.Manual;


			// Retrieve data from database
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            Cursor.Current = Cursors.WaitCursor;

            if (mySanctionNum == null) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData(mySanctionNum);
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                            TeamCode.Visible = true;
                        } else {
                            TeamCode.Visible = false;
                        }
                        if ( (byte)myTourRow["SlalomRounds"] > 0 ) {
                            mySortCommand = myTourProperties.RunningOrderSortSlalom;

                            EventRunInfoLabel.Text = "   Event Start:\n" + "   Event Delay:\n" + "Skiers, Passes:";
                            EventRunInfoData.Text = "";
                            EventRunPerfLabel.Text = "Event Duration:\n" + "Mins Per Skier:\n" + " Mins Per Pass:";
                            EventRunPerfData.Text = "";
                            
                            //Instantiate object for checking for records
                            myCheckEventRecord = new CheckEventRecord(myTourRow);

                            //Load round selection list based on number of rounds specified for the tournament
                            roundSelect.SelectList_Load( myTourRow["SlalomRounds"].ToString(), roundSelect_Click );
                            roundActiveSelect.SelectList_LoadHorztl( myTourRow["SlalomRounds"].ToString(), roundActiveSelect_Click );
                            roundActiveSelect.RoundValue = "1";

                            //Load rope length selection list
                            SlalomLineSelect.SelectList_Load( SlalomLineSelect_Change );

                            //Load slalom speed selection list
                            SlalomSpeedSelect.SelectList_Load( SlalomSpeedSelect_Change );

                            mySkierClassList = new ListSkierClass();
                            mySkierClassList.ListSkierClassLoad();
                            scoreEventClass.DataSource = mySkierClassList.DropdownList;
                            scoreEventClass.DisplayMember = "ItemName";
                            scoreEventClass.ValueMember = "ItemValue";
                            myTourClass = myTourRow["Class"].ToString().ToUpper();
                            myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                            myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];

                            //Determine required number of judges for event
                            myNumJudges = 5;
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Select ListCode, CodeValue, MaxValue, MinValue FROM CodeValueList " );
                            curSqlStmt.Append( "Where ListName = 'SlalomJudgesNum' And ListCode = '" + myTourClass + "' ORDER BY SortSeq" );
                            DataTable curNumJudgesDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                            if (curNumJudgesDataTable.Rows.Count > 0) {
                                setShowByNumJudges( Convert.ToInt16( (Decimal)curNumJudgesDataTable.Rows[0]["MaxValue"] ));
                            } else {
                                setShowByNumJudges ( 1 );
                            }
                            if ( myNumJudges > 3 ) {
                                ClassC5JudgeCB.Visible = false;
                                ClassC5JudgeCB.Enabled = false;
                                ClassR3JudgeCB.Visible = true;
                                ClassR3JudgeCB.Enabled = true;
                                ClassR3JudgeCB.Checked = true;
                            } else {
                                ClassC5JudgeCB.Visible = true;
                                ClassC5JudgeCB.Enabled = true;
                                ClassR3JudgeCB.Visible = false;
                                ClassR3JudgeCB.Enabled = false;
                            }

                            //Retrieve and load boat times
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, SortSeq, CodeValue, MinValue, MaxValue, CodeDesc " );
                            curSqlStmt.Append( "FROM CodeValueList WHERE ListName = 'SlalomBoatTime' ORDER BY SortSeq" );
                            myTimesDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

                            //Retrieve list of approved tournament boats
                            getApprovedTowboats();
                            listApprovedBoatsDataGridView.Visible = false;
                            approvedBoatSelectGroupBox.Visible = false;

                            //Retrieve and load tournament event entries
                            loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

                            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                                LiveWebLabel.Visible = true;
                            } else {
                                LiveWebLabel.Visible = false;
                            }

							DataRow curClassRow = getClassRow( myTourRow["EventScoreClass"].ToString().ToUpper() );
							if ( (Decimal) curClassRow["ListCodeNum"] >= (Decimal) myClassERow["ListCodeNum"] ) {
								myCheckOfficials = new CheckOfficials();
							} else {
								myCheckOfficials = null;
                            }

						} else {
                            MessageBox.Show( "The slalom event is not defined for the active tournament" );
                        }
                    } else {
                        MessageBox.Show( "The active tournament is not properly defined.  You must select from the Administration menu Tournament List option" );
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        public void setShowByNumJudges( Int16 inNumJudges ) {
            myNumJudges = inNumJudges;

            RoundRecap.DisplayIndex = 0;
            skierPassRecap.DisplayIndex = 1;
            PassNumRecap.DisplayIndex = 2;
            CellBorder1.DisplayIndex = 3;
            GateEntry2Recap.DisplayIndex = 4;
            Judge2ScoreRecap.DisplayIndex = 5;
            GateExit2Recap.DisplayIndex = 6;
            CellBorder2.DisplayIndex = 7;
            GateEntry3Recap.DisplayIndex = 8;
            Judge3ScoreRecap.DisplayIndex = 9;
            GateExit3Recap.DisplayIndex = 10;
            CellBorder3.DisplayIndex = 11;
            Judge4ScoreRecap.DisplayIndex = 12;
            CellBorder4.DisplayIndex = 13;
            Judge5ScoreRecap.DisplayIndex = 14;
            CellBorder5.DisplayIndex = 15;
            GateEntry1Recap.DisplayIndex = 16;
            Judge1ScoreRecap.DisplayIndex = 17;
            GateExit1Recap.DisplayIndex = 18;
            CellBorder6.DisplayIndex = 19;
            TimeInTolRecap.DisplayIndex = 20;
            TimeInTolImg.DisplayIndex = 21;
            BoatTimeRecap.DisplayIndex = 22;
            ScoreRecap.DisplayIndex = 23;
            ScoreProtRecap.DisplayIndex = 24;
            RerideRecap.DisplayIndex = 25;
            NoteRecap.DisplayIndex = 26;
            RerideReasonRecap.DisplayIndex = 27;
            SanctionIdRecap.DisplayIndex = 28;
            MemberIdRecap.DisplayIndex = 29;
            PKRecap.DisplayIndex = 30;
            Updated.DisplayIndex = 31;

            Judge5ScoreRecap.Visible = true;
            CellBorder5.Visible = true;
            Judge4ScoreRecap.Visible = true;
            CellBorder4.Visible = true;
            Judge3ScoreRecap.Visible = true;
            GateEntry3Recap.Visible = true;
            GateExit3Recap.Visible = true;
            CellBorder3.Visible = true;
            Judge2ScoreRecap.Visible = true;
            GateEntry2Recap.Visible = true;
            GateExit2Recap.Visible = true;
            CellBorder2.Visible = true;

            if ( myNumJudges == 1 ) {
                myStartCellIndex = slalomRecapDataGridView.Columns["Judge1ScoreRecap"].Index;
            } else {
                myStartCellIndex = slalomRecapDataGridView.Columns["Judge2ScoreRecap"].Index;
            }

            if ( myNumJudges < 5 ) {
                Judge5ScoreRecap.Visible = false;
                CellBorder5.Visible = false;
                if ( myNumJudges < 4 ) {
                    Judge4ScoreRecap.Visible = false;
                    CellBorder4.Visible = false;

                    if ( myNumJudges < 3 ) {
                        Judge3ScoreRecap.Visible = false;
                        GateEntry3Recap.Visible = false;
                        GateExit3Recap.Visible = false;
                        CellBorder3.Visible = false;

                        if ( myNumJudges < 2 ) {
                            Judge2ScoreRecap.Visible = false;
                            GateEntry2Recap.Visible = false;
                            GateExit2Recap.Visible = false;
                            CellBorder2.Visible = false;
                        }
                    }
                }
            } else {
                RoundRecap.DisplayIndex = 0;
                skierPassRecap.DisplayIndex = 1;
                PassNumRecap.DisplayIndex = 2;
                CellBorder1.DisplayIndex = 3;
                GateEntry2Recap.DisplayIndex = 4;
                Judge2ScoreRecap.DisplayIndex = 5;
                CellBorder2.DisplayIndex = 6;
                GateEntry3Recap.DisplayIndex = 7;
                Judge3ScoreRecap.DisplayIndex = 8;
                CellBorder3.DisplayIndex = 9;
                Judge4ScoreRecap.DisplayIndex = 10;
                GateExit3Recap.DisplayIndex = 11;
                CellBorder4.DisplayIndex = 12;
                Judge5ScoreRecap.DisplayIndex = 13;
                GateExit2Recap.DisplayIndex = 14;
                CellBorder5.DisplayIndex = 15;
                GateEntry1Recap.DisplayIndex = 16;
                Judge1ScoreRecap.DisplayIndex = 17;
                GateExit1Recap.DisplayIndex = 18;
                CellBorder6.DisplayIndex = 19;
                TimeInTolRecap.DisplayIndex = 20;
                TimeInTolImg.DisplayIndex = 21;
                BoatTimeRecap.DisplayIndex = 22;
                ScoreRecap.DisplayIndex = 23;
                ScoreProtRecap.DisplayIndex = 24;
                RerideRecap.DisplayIndex = 25;
                NoteRecap.DisplayIndex = 26;
                RerideReasonRecap.DisplayIndex = 27;
                SanctionIdRecap.DisplayIndex = 28;
                MemberIdRecap.DisplayIndex = 29;
                PKRecap.DisplayIndex = 30;
                Updated.DisplayIndex = 31;
            }
        }

        private void ResizeNarrow_Click(object sender, EventArgs e) {
            ResizeNarrow.Visible = false;
            ResizeNarrow.Enabled = false;
            ResizeWide.Visible = true;
            ResizeWide.Enabled = true;

            TourEventRegDataGridView.Width = 265;
        }

        private void ResizeWide_Click(object sender, EventArgs e) {
            ResizeNarrow.Visible = true;
            ResizeNarrow.Enabled = true;
            ResizeWide.Visible = false;
            ResizeWide.Enabled = false;
            if ( TeamCode.Visible ) {
                TourEventRegDataGridView.Width = 540;
            } else {
                TourEventRegDataGridView.Width = 490;
            }
        }

        private void ClassR3JudgeCB_CheckedChanged(object sender, EventArgs e) {
            CheckBox curControl = (CheckBox)sender;
            if ( curControl.Checked ) {
                setShowByNumJudges( 3 );
            } else {
                setShowByNumJudges( 5 );
            }
        }

        private void ClassC5JudgeCB_CheckedChanged( object sender, EventArgs e ) {
            CheckBox curControl = (CheckBox)sender;
            if ( curControl.Checked ) {
                setShowByNumJudges( 5 );
            } else {
                setShowByNumJudges( 3 );
            }
        }

        private void ScoreEntry_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.SlalomEntry_Width = this.Size.Width;
                Properties.Settings.Default.SlalomEntry_Height = this.Size.Height;
                Properties.Settings.Default.SlalomEntry_Location = this.Location;
            }
            if ( myPassRunCount > 0 ) {
                if ( StartTimerButton.Visible ) {
                    StartTimerButton_Click( null, null );
                }
                int curHours, curMins, curSeconds, curTimeDiff;
                String curMethodName = "Slalom:ScoreEntry:FormClosed";
                try {
                    curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
                } catch {
                    myEventStartTime = DateTime.Now;
                    EventRunInfoData.Text = "";
                    EventRunPerfData.Text = "";
                    curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
                }
                Log.WriteFile( curMethodName + ":    Event Start=" + myEventStartTime.ToString( "MM/dd/yyyy HH:mm" ) );
                Log.WriteFile( curMethodName + ":      Event End=" + DateTime.Now.ToString( "MM/dd/yyyy HH:mm" ) );
                curHours = Math.DivRem( curTimeDiff, 3600, out curSeconds );
                curMins = Math.DivRem( curSeconds, 60, out curSeconds );
                Log.WriteFile( curMethodName + ": Event Duration=" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" ) );
                curHours = Math.DivRem( myEventDelaySeconds, 3600, out curSeconds );
                curMins = Math.DivRem( curSeconds, 60, out curSeconds );
                Log.WriteFile( curMethodName + ":    Event Delay=" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" ) );
                Log.WriteFile( curMethodName + ":         Skiers=" + mySkierRunCount.ToString( "##0" ) );
                Log.WriteFile( curMethodName + ":         Passes=" + myPassRunCount.ToString( "##0" ) );
                curMins = Math.DivRem( ( curTimeDiff / mySkierRunCount ), 60, out curSeconds );
                Log.WriteFile( curMethodName + ": Mins Per Skier=" + ( curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" ) ) );
                curMins = Math.DivRem( ( curTimeDiff / myPassRunCount ), 60, out curSeconds );
                Log.WriteFile( curMethodName + ": Mins Per Pass =" + ( curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" ) ) );
            }
        }

        private void ScoreEntry_FormClosing(object sender, FormClosingEventArgs e) {
            if ( isDataModified ) {
                saveScore();
            }

            e.Cancel = false;
        }

		private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show("Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message);
            if ((e.Exception) is ConstraintException) {
                DataGridView view = (DataGridView)sender;
                if (e.RowIndex >= 0) {
                    view.Rows[e.RowIndex].ErrorText = "an error";
                }
                e.ThrowException = false;
            }
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            try {
                if ( saveScore() ) {
                    isDataModified = false;
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveScore() {
            String curMethodName = "Slalom:ScoreEntry:saveScore";
            bool curReturn = true;
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            if ( StartTimerButton.Visible ) {
                StartTimerButton_Click( null, null );
            }

            try {
                String curUpdateStatus = "";
                if (myRecapRow == null || myRecapRow.Index < 0) {
                    curUpdateStatus = "N";
                } else {
                    try {
                        curUpdateStatus = (String)myRecapRow.Cells["Updated"].Value;
                    } catch (Exception excp) {
                        if (slalomRecapDataGridView.Rows.Count > 0) {
                            curUpdateStatus = "Y";
                            curReturn = true;
                            String curMsg = ":Error checking slalom pass update status \n" + excp.Message;
                            //MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        } else {
                            curUpdateStatus = "N";
                        }
                    }
                }
                if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                    try {
                        if ( !( roundActiveSelect.RoundValue.Equals( roundSelect.RoundValue ) ) ) {
                            DialogResult msgResp = MessageBox.Show( "Skier score being updated in round other than active round!"
                                + "\nDo you want to continue with update?"
                                , "Update Warning"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Warning
                                , MessageBoxDefaultButton.Button1 );
                            if ( msgResp == DialogResult.No ) {
                                return false;
                            }
                        }
                        String curSanctionId = (String)myRecapRow.Cells["SanctionIdRecap"].Value;
                        String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
                        String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
                        byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );

                        if ( !(curMemberId.Equals((String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value))) {
                            MessageBox.Show("Unable to continue save processing"
                                + "\n Current id of skier being scored       = " + curMemberId
                                + "\n Current id skier in running order list = " + (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value );
                            return false;
                        }
                        #region Update skier score for current pass
                        rowsProc = 0;
                        Int64 curRecapPK = Convert.ToInt64( myRecapRow.Cells["PKRecap"].Value );
                        String curScoreRecap, curJudge1Score, curJudge2Score, curJudge3Score, curJudge4Score, curJudge5Score;
                        String curBoatTime, curReride, curScoreProt, curTimeInTol, curRerideReason, curNote, curPassLine;
                        String curEntryGate1, curEntryGate2, curEntryGate3, curExitGate1, curExitGate2, curExitGate3;

                        Int16 curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );
                        int curSkierRunNum = myRecapRow.Index + 1;

                        try {
                            curPassLine = (String)myRecapRow.Cells["PassLineLengthRecap"].Value;
                            if ( curPassLine.Length < 4 ) {
                                curPassLine = ( (Decimal)myPassRow["MinValue"] ).ToString( "#0.00" );
                            }
                        } catch {
                            curPassLine = ( (Decimal)myPassRow["MinValue"] ).ToString( "#0.00" );
                        }
                        try {
                            curReride = (String)myRecapRow.Cells["RerideRecap"].Value;
                        } catch {
                            curReride = "N";
                        }
                        try {
                            curJudge1Score = (String)myRecapRow.Cells["Judge1ScoreRecap"].Value;
                            if ( curJudge1Score.Length < 1 ) curJudge1Score = "Null";
                        } catch {
                            curJudge1Score = "Null";
                        }
                        try {
                            curJudge2Score = (String)myRecapRow.Cells["Judge2ScoreRecap"].Value;
                            if ( curJudge2Score.Length < 1 ) curJudge2Score = "Null";
                        } catch {
                            curJudge2Score = "Null";
                        }
                        try {
                            curJudge3Score = (String)myRecapRow.Cells["Judge3ScoreRecap"].Value;
                            if ( curJudge3Score.Length < 1 ) curJudge3Score = "Null";
                        } catch {
                            curJudge3Score = "Null";
                        }
                        try {
                            curJudge4Score = (String)myRecapRow.Cells["Judge4ScoreRecap"].Value;
                            if ( curJudge4Score.Length < 1 ) curJudge4Score = "Null";
                        } catch {
                            curJudge4Score = "Null";
                        }
                        try {
                            curJudge5Score = (String)myRecapRow.Cells["Judge5ScoreRecap"].Value;
                            if ( curJudge5Score.Length < 1 ) curJudge5Score = "Null";
                        } catch {
                            curJudge5Score = "Null";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateEntry1Recap"].Value ) {
                                curEntryGate1 = "1";
                            } else {
                                curEntryGate1 = "0";
                            }
                        } catch {
                            curEntryGate1 = "0";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateEntry2Recap"].Value ) {
                                curEntryGate2 = "1";
                            } else {
                                curEntryGate2 = "0";
                            }
                        } catch {
                            curEntryGate2 = "0";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateEntry3Recap"].Value ) {
                                curEntryGate3 = "1";
                            } else {
                                curEntryGate3 = "0";
                            }
                        } catch {
                            curEntryGate3 = "0";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateExit1Recap"].Value ) {
                                curExitGate1 = "1";
                            } else {
                                curExitGate1 = "0";
                            }
                        } catch {
                            curExitGate1 = "0";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateExit2Recap"].Value ) {
                                curExitGate2 = "1";
                            } else {
                                curExitGate2 = "0";
                            }
                        } catch {
                            curExitGate2 = "0";
                        }
                        try {
                            if ( (bool)myRecapRow.Cells["GateExit3Recap"].Value ) {
                                curExitGate3 = "1";
                            } else {
                                curExitGate3 = "0";
                            }
                        } catch {
                            curExitGate3 = "0";
                        }
                        try {
                            curScoreRecap = (String)myRecapRow.Cells["ScoreRecap"].Value;
                            if ( curScoreRecap.Length < 1 ) curScoreRecap = "0";
                        } catch {
                            curScoreRecap = "0";
                        }
                        try {
                            curTimeInTol = (String)myRecapRow.Cells["TimeInTolRecap"].Value;
                        } catch {
                            curTimeInTol = "N";
                        }
                        try {
                            curBoatTime = (String)myRecapRow.Cells["BoatTimeRecap"].Value;
                            if ( curBoatTime.Length == 0 ) curBoatTime = "0";
                        } catch {
                            curBoatTime = "0";
                        }
                        try {
                            curScoreProt = (String)myRecapRow.Cells["ScoreProtRecap"].Value;
                        } catch {
                            curScoreProt = "N";
                        }
                        try {
                            curRerideReason = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
                            curRerideReason = curRerideReason.Replace( "'", "''" );
                        } catch {
                            curRerideReason = "";
                        }
                        try {
                            curNote = (String)myRecapRow.Cells["NoteRecap"].Value;
                            curNote = curNote.Replace( "'", "''" );
                        } catch {
                            curNote = "";
                        }

                        curSqlStmt = new StringBuilder( "" );
                        if ( curRecapPK > 0 ) {
                            curSqlStmt.Append( "Update SlalomRecap Set " );
                            curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                            curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                            curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                            curSqlStmt.Append( ", Round = " + curRound );
                            curSqlStmt.Append( ", PassNum = " + curPassNum );
                            curSqlStmt.Append( ", SkierRunNum = " + curSkierRunNum );
                            curSqlStmt.Append( ", PassLineLength = " + curPassLine );
                            curSqlStmt.Append( ", Reride = '" + curReride + "'" );
                            curSqlStmt.Append( ", Judge1Score = " + curJudge1Score );
                            curSqlStmt.Append( ", Judge2Score = " + curJudge2Score );
                            curSqlStmt.Append( ", Judge3Score = " + curJudge3Score );
                            curSqlStmt.Append( ", Judge4Score = " + curJudge4Score );
                            curSqlStmt.Append( ", Judge5Score = " + curJudge5Score );
                            curSqlStmt.Append( ", EntryGate1 = " + curEntryGate1 );
                            curSqlStmt.Append( ", EntryGate2 = " + curEntryGate2 );
                            curSqlStmt.Append( ", EntryGate3 = " + curEntryGate3 );
                            curSqlStmt.Append( ", ExitGate1 = " + curExitGate1 );
                            curSqlStmt.Append( ", ExitGate2 = " + curExitGate2 );
                            curSqlStmt.Append( ", ExitGate3 = " + curExitGate3 );
                            curSqlStmt.Append( ", BoatTime = " + curBoatTime );
                            curSqlStmt.Append( ", Score = " + curScoreRecap );
                            curSqlStmt.Append( ", TimeInTol = '" + curTimeInTol + "'" );
                            curSqlStmt.Append( ", ScoreProt = '" + curScoreProt + "'" );
                            curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                            curSqlStmt.Append( ", RerideReason = '" + curRerideReason + "'" );
                            curSqlStmt.Append( ", Note = '" + curNote + "'" );
                            curSqlStmt.Append( " Where PK = " + curRecapPK.ToString() );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        } else {
                            curSqlStmt.Append( "Insert SlalomRecap ( " );
                            curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round" );
                            curSqlStmt.Append(", PassNum, SkierRunNum, Reride, PassLineLength");
                            curSqlStmt.Append( ", Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score" );
                            curSqlStmt.Append( ", EntryGate1, EntryGate2, EntryGate3" );
                            curSqlStmt.Append( ", ExitGate1, ExitGate2, ExitGate3" );
                            curSqlStmt.Append( ", BoatTime, Score, TimeInTol, ScoreProt, LastUpdateDate, RerideReason, Note" );
                            curSqlStmt.Append( ") Values ( " );
                            curSqlStmt.Append( "'" + curSanctionId + "'" );
                            curSqlStmt.Append( ", '" + curMemberId + "'" );
                            curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                            curSqlStmt.Append( ", " + curRound );
                            curSqlStmt.Append( ", " + curPassNum );
                            curSqlStmt.Append( ", " + curSkierRunNum );
                            curSqlStmt.Append( ", '" + curReride + "'" );
                            curSqlStmt.Append( ", " + curPassLine );
                            curSqlStmt.Append( ", " + curJudge1Score );
                            curSqlStmt.Append( ", " + curJudge2Score );
                            curSqlStmt.Append( ", " + curJudge3Score );
                            curSqlStmt.Append( ", " + curJudge4Score );
                            curSqlStmt.Append( ", " + curJudge5Score );
                            curSqlStmt.Append( ", " + curEntryGate1 );
                            curSqlStmt.Append( ", " + curEntryGate2 );
                            curSqlStmt.Append( ", " + curEntryGate3 );
                            curSqlStmt.Append( ", " + curExitGate1 );
                            curSqlStmt.Append( ", " + curExitGate2 );
                            curSqlStmt.Append( ", " + curExitGate3 );
                            curSqlStmt.Append( ", " + curBoatTime );
                            curSqlStmt.Append( ", " + curScoreRecap );
                            curSqlStmt.Append( ", '" + curTimeInTol + "'" );
                            curSqlStmt.Append( ", '" + curScoreProt + "'" );
                            curSqlStmt.Append( ", getdate()" );
                            curSqlStmt.Append( ", '" + curRerideReason + "'" );
                            curSqlStmt.Append( ", '" + curNote + "'" );
                            curSqlStmt.Append( ")" );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        }
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        if ( rowsProc > 0 ) {
                            isDataModified = false;
                            myRecapRow.Cells["Updated"].Value = "N";
                            refreshScoreSummaryWindow();
                            if ( curRecapPK < 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Select max(PK) as MaxPK From SlalomRecap" );
                                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                                if ( curDataTable.Rows.Count > 0 ) {
                                    Int64 curPK = (Int64)curDataTable.Rows[0]["MaxPK"];
                                    myRecapRow.Cells["PKRecap"].Value = curPK.ToString();
                                }
                            }
                        }
                        #endregion

                        #region Update skier total score for round
                        rowsProc = 0;
                        Int64 curScorePK = 0;
                        String curStartLen = "", curStatus = "TBD", curBoatCode = "", curScorePassNum = "", curEventClass = "" ;
                        Int16 curStartSpeed = 0;
                        Int16 curMaxSpeed = SlalomSpeedSelect.MaxValue;
                        if ( myScoreRow == null ) {
                            curScorePK = -1;
                            curStartLen = SlalomLineSelect.CurrentValue;
                            curStartSpeed = SlalomSpeedSelect.CurrentValue;
                        } else {
                            curScorePK = (Int64)myScoreRow["PK"];
                            curStartLen = (String)myScoreRow["StartLen"];
                            curStartSpeed = (byte)myScoreRow["StartSpeed"];
                        }

                        curEventClass = (String)scoreEventClass.SelectedValue;

                        Decimal curScore = 0, curNopsScore = 0;
                        try {
                            curScore = Convert.ToDecimal( scoreTextBox.Text );
                        } catch {
                            curScore = 0;
                        }
                        TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScore.ToString( "###.00" );
						TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = hcapScoreTextBox.Text;
						

						try {
                            curNopsScore = Convert.ToDecimal( nopsScoreTextBox.Text );
                        } catch {
                            curNopsScore = 0;
                        }

                        try {
                            curNote = noteTextBox.Text;
                            curNote = curNote.Replace( "'", "''" );
                        } catch {
                            curNote = "";
                        }

						curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

						try {
                            String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
                            if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
                            if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
                            if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
                            if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
                        } catch {
                            curStatus = "TBD";
                        }

                        Int16 curSpeedKph = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                        String curRopeLenMetric = ( (Decimal)myPassRow["MinValue"] ).ToString( "#0.00" );
                        String[] curPassAttr = ((String)myPassRow["CodeValue"]).Split(',');
                        curScorePassNum = (String)myPassRow["ListCode"];
                        String curFinalRopeOff = curPassAttr[2];
                        String curCompletedSpeedKph = "0";
                        String curCompletedSpeedMph = "0";

                        DataRow curClassRow = getClassRow(curEventClass);
                        if ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) {
                            curRopeLenMetric = (String) myRecapRow.Cells["PassLineLengthRecap"].Value;
                            DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = " + SlalomLineSelect.CurrentShowValueNum);
                            String curValue = (String) curLineRow[0]["CodeValue"];
                            curFinalRopeOff = curValue.Substring(0, curValue.IndexOf("-") - 1);
                        } else if ( isQualifiedAltScoreMethod(curAgeGroup, (String) curClassRow["ListCode"]) 
                            && (curSpeedKph < curMaxSpeed) && (curScore >= 6) ) {
                            curRopeLenMetric = (String) myRecapRow.Cells["PassLineLengthRecap"].Value;
                            DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = " + SlalomLineSelect.CurrentShowValueNum);
                            String curValue = (String) curLineRow[0]["CodeValue"];
                            curFinalRopeOff = curValue.Substring(0, curValue.IndexOf("-") - 1);
                        }

                        try {
                            if ( Convert.ToDecimal( curScoreRecap ) < 0M ) {
                                /*
                                * I don't know why I did this but it created a problem when it was checking for less than or equal to zero.
                                * For example, when a skier missed the entrace gates the final pass score is showing as 6 but should be zero
                                * The final score is correct but the final pass buoys was being set to 6
                                */
                                if ( myRecapRow.Index > 0 ) {
                                    curScoreRecap = "6";
                                    curPassNum = Convert.ToInt16( curScorePassNum );
                                }
                            }
                            if ( Convert.ToDecimal(curScoreRecap) == 6M 
                                && curStatus != "Complete"
                                && curReride != "Y"
                                ) {
                                curCompletedSpeedKph = curSpeedKph.ToString();
                                curCompletedSpeedMph = curPassAttr[3].Substring(0, 4);
                            } else {
                                for ( int idx = myRecapRow.Index - 1; idx >= 0; idx-- ) {
                                    try {
                                        DataGridViewRow tempRecapRow = slalomRecapDataGridView.Rows[idx];
                                        Decimal tempScoreRecap = Decimal.Parse((String) tempRecapRow.Cells["ScoreRecap"].Value);
                                        String tempReride = (String) tempRecapRow.Cells["RerideRecap"].Value;
                                        String tempScoreProtRecap = (String) tempRecapRow.Cells["ScoreProtRecap"].Value;
                                        if ( tempScoreRecap == 6M ) {
                                            if ( tempReride.Equals("Y") ) {
                                                if ( tempScoreProtRecap.Equals("Y") ) {
                                                    Int16 tempPassNum = Convert.ToInt16((String) tempRecapRow.Cells["PassNumRecap"].Value);
                                                    DataRow tempPassRow = getPassRow(tempPassNum);
                                                    Int16 tempSpeedKph = Convert.ToInt16((Decimal) tempPassRow["MaxValue"]);
                                                    String[] tempPassAttr = ( (String) tempPassRow["CodeValue"] ).Split(',');

                                                    curCompletedSpeedKph = tempSpeedKph.ToString();
                                                    curCompletedSpeedMph = tempPassAttr[3].Substring(0, 4);
                                                    break;
                                                }
                                            } else {
                                                Int16 tempPassNum = Convert.ToInt16((String) tempRecapRow.Cells["PassNumRecap"].Value);
                                                DataRow tempPassRow = getPassRow(tempPassNum);
                                                Int16 tempSpeedKph = Convert.ToInt16((Decimal) tempPassRow["MaxValue"]);
                                                String[] tempPassAttr = ( (String) tempPassRow["CodeValue"] ).Split(',');

                                                curCompletedSpeedKph = tempSpeedKph.ToString();
                                                curCompletedSpeedMph = tempPassAttr[3].Substring(0, 4);
                                                break;
                                            }
                                        }
                                    } catch ( Exception ex ) {
                                    }
                                }
                            }
                        } catch {
                        }

                        if ( curTimeInTol.Equals( "Y" ) ) {
                            if ( curScorePK > 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Update SlalomScore Set " );
                                curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                                curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );
                                curSqlStmt.Append( ", Round = " + curRound );
                                curSqlStmt.Append( ", Score = " + curScore.ToString( "##0.00" ) );
                                curSqlStmt.Append( ", NopsScore = " + curNopsScore.ToString( "##0.00" ) );
                                curSqlStmt.Append( ", StartSpeed = " + curStartSpeed.ToString( "00" ) );
                                curSqlStmt.Append( ", StartLen = '" + curStartLen + "'" );
                                curSqlStmt.Append( ", MaxSpeed = " + curMaxSpeed.ToString( "00" ) );
                                curSqlStmt.Append( ", Boat = '" + curBoatCode + "'" );

                                curSqlStmt.Append( ", Status = '" + curStatus + "'" );
                                curSqlStmt.Append( ", FinalPassNum = " + curScorePassNum.ToString() );
                                curSqlStmt.Append( ", FinalSpeedMph = " + curPassAttr[3].Substring( 0, 4 ) );
                                curSqlStmt.Append( ", FinalSpeedKph = " + curSpeedKph.ToString() );
                                curSqlStmt.Append( ", FinalLen = '" + curRopeLenMetric + "'" );
                                curSqlStmt.Append( ", FinalLenOff = '" + curFinalRopeOff + "'" );
                                curSqlStmt.Append( ", FinalPassScore = " + curScoreRecap );

                                if ( curCompletedSpeedKph.Length > 0 && curCompletedSpeedMph.Length > 0 ) {
                                    curSqlStmt.Append(", CompletedSpeedMph = " + curCompletedSpeedMph);
                                    curSqlStmt.Append(", CompletedSpeedKph = " + curCompletedSpeedKph);
                                }

                                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                                curSqlStmt.Append( ", Note = '" + curNote + "'" );
                                curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            }
                            if ( rowsProc == 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Insert SlalomScore ( " );
                                curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass, Score, NopsScore" );
                                curSqlStmt.Append( ", StartSpeed, StartLen, MaxSpeed, Boat" );
                                curSqlStmt.Append( ", Status, FinalPassNum, FinalSpeedMph, FinalSpeedKph, FinalLen, FinalLenOff, FinalPassScore" );
                                if ( curCompletedSpeedKph.Length > 0 && curCompletedSpeedMph.Length > 0 ) {
                                    curSqlStmt.Append(", CompletedSpeedMph, CompletedSpeedKph");
                                }
                                curSqlStmt.Append( ", LastUpdateDate, Note" );
                                curSqlStmt.Append( ") Values ( " );
                                curSqlStmt.Append( "'" + curSanctionId + "'" );
                                curSqlStmt.Append( ", '" + curMemberId + "'" );
                                curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", " + curRound );
                                curSqlStmt.Append(", '" + curEventClass + "'");
                                curSqlStmt.Append(", " + curScore.ToString("##0.00"));
                                curSqlStmt.Append( ", " + curNopsScore.ToString( "##0.00" ) );
                                curSqlStmt.Append( ", " + curStartSpeed.ToString( "00" ) );
                                curSqlStmt.Append( ", '" + curStartLen + "'" );
                                curSqlStmt.Append( ", " + curMaxSpeed.ToString( "00" ) );
                                curSqlStmt.Append( ", '" + curBoatCode + "'" );

                                curSqlStmt.Append( ", '" + curStatus + "'" );
                                curSqlStmt.Append( ", " + curScorePassNum.ToString() );
                                curSqlStmt.Append( ", " + curPassAttr[3].Substring( 0, 4 ) );
                                curSqlStmt.Append( ", " + curSpeedKph.ToString() );
                                curSqlStmt.Append( ", '" + curRopeLenMetric + "'" );
                                curSqlStmt.Append( ", '" + curFinalRopeOff + "'" );
                                curSqlStmt.Append( ", '" + curScoreRecap + "'" );

                                if ( curCompletedSpeedKph.Length > 0 && curCompletedSpeedMph.Length > 0 ) {
                                    curSqlStmt.Append(", " + curCompletedSpeedMph);
                                    curSqlStmt.Append(", " + curCompletedSpeedKph);
                                }

                                curSqlStmt.Append( ", GETDATE()" );
                                curSqlStmt.Append( ", '" + curNote + "'" );
                                curSqlStmt.Append( ")" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                if (rowsProc == 0) {
                                    MessageBox.Show( "Warning: Score for skier was not successfully updated.  Plese try again." );
                                }
                            }
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            if (rowsProc > 0) {
                                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScore.ToString( "###.00" );
								TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = hcapScoreTextBox.Text;

								getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
                                if (myScoreDataTable.Rows.Count > 0) {
                                    myScoreRow = myScoreDataTable.Rows[0];
                                }
                                
                                //Send scores and information to any defined external reporting system
                                transmitExternalScoreboard( curSanctionId, curMemberId, curAgeGroup
                                    , curRound, curPassNum, curSkierRunNum, curStatus
                                    , curStartLen, curPassAttr, curSpeedKph, curRopeLenMetric, curScoreRecap, curScore );

                                #region Check to see if score is equal to or great than divisions current record score
                                String curCheckRecordMsg = myCheckEventRecord.checkRecordSlalom( curAgeGroup, scoreTextBox.Text, (byte)myScoreRow["SkiYearAge"], (string)myScoreRow["Gender"]);
                                if (curCheckRecordMsg == null) curCheckRecordMsg = "";
                                if (curCheckRecordMsg.Length > 1) {
                                    MessageBox.Show( curCheckRecordMsg );
                                }
                                #endregion
                            }
                        }
                        #endregion

                        #region Update run statistics
                        if ( rowsProc > 0 ) {
                            int curSeconds = 0, curMins = 0, curHours, curTimeDiff = 0;
                            if ( myPassRunCount > 1 ) {
                                try {
                                    curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
                                } catch {
                                    myEventStartTime = DateTime.Now;
                                    EventRunInfoData.Text = "";
                                    EventRunPerfData.Text = "";
                                    curTimeDiff = Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventStartTime ) ).TotalSeconds ) - myEventDelaySeconds;
                                }
                                curHours = Math.DivRem( curTimeDiff, 3600, out curSeconds );
                                curMins = Math.DivRem( curSeconds, 60, out curSeconds );
                                String curEventDuration = curHours.ToString( "00" ) + ":" + curMins.ToString( "00" );
                                curMins = Math.DivRem( ( curTimeDiff / mySkierRunCount ), 60, out curSeconds );
                                String curMinsPerSkier = curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" );
                                curMins = Math.DivRem( ( curTimeDiff / myPassRunCount ), 60, out curSeconds );
                                String curMinsPerPass = curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" );
                                EventRunPerfData.Text = curEventDuration + "\n" + curMinsPerSkier + "\n" + curMinsPerPass;
                            } else {
                                myEventStartTime = DateTime.Now;
                                EventRunInfoData.Text = "";
                                EventRunPerfData.Text = "";

                            }
                            curHours = Math.DivRem( myEventDelaySeconds, 3600, out curSeconds );
                            curMins = Math.DivRem( curSeconds, 60, out curSeconds );
                            EventRunInfoData.Text = myEventStartTime.ToString( "HH:mm" )
                                + "\n" + curHours.ToString( "00" ) + ":" + curMins.ToString( "00" )
                                + "\n" + mySkierRunCount.ToString( "##0" ) 
                                + ", " + myPassRunCount.ToString("##0");
                        }
                        #endregion
                    } catch (Exception excp) {
                        curReturn = false;
                        String curMsg = ":Error attempting to update skier score \n" + excp.Message + "\n" + curSqlStmt.ToString();
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                }

            } catch ( Exception excp ) {
                curReturn = false;
                String curMsg = ":Error attempting to update skier information \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return curReturn;
        }

        private void transmitExternalScoreboard(String curSanctionId, String curMemberId, String curAgeGroup
            , byte curRound, short curPassNum, int curSkierRunNum, String curStatus
            , String curStartLen, String[] curPassAttr, Int16 curSpeedKph, String curRopeLenMetric, String curScoreRecap, decimal curScore) {
            

            String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
            String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
            DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select( "ListCode = '" + curStartLen + "'" );
            String curStartLenOff = "";
            if (curLineRow.Length > 0) {
                curStartLenOff = (String)curLineRow[0]["CodeValue"];
            }

            if (ExportLiveScoreboard.ScoreboardLocation.Length > 1) {
                String curSpeedMph = curPassAttr[3].Substring( 0, 2 );
                curStartLen = SlalomLineSelect.CurrentValue;
                ExportLiveScoreboard.exportCurrentSkierSlalom( mySanctionNum
                    , curMemberId, curAgeGroup, curTeamCode, curRound
                    , curSkierName, SlalomSpeedSelect.CurrentValueDesc, curStartLenOff + "/" + curStartLen
                    , curScore, Convert.ToDecimal( curScoreRecap ), (String)myPassRow["CodeValue"] );
            }

            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                ExportLiveWeb.exportCurrentSkierSlalom( mySanctionNum, curMemberId, curAgeGroup, curRound, curSkierRunNum, curEventGroup );
            }

            if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                StringBuilder curTwitterMessage = new StringBuilder( "" );
                if (ExportLiveTwitter.TwitterReportByValue.Equals( "Pass" )) {
                    curTwitterMessage.Append( curSanctionId + " " + curAgeGroup + " " + curSkierName );
                    if (curTeamCode.Length > 0) {
                        curTwitterMessage.Append( " Team: " + curTeamCode );
                    }
                    curTwitterMessage.Append( " Score: " + curScore.ToString( "##0.00" ) );
                    curTwitterMessage.Append( " Pass: " + curScoreRecap.ToString() + " buoys " );
                    curTwitterMessage.Append( " " + curPassAttr[2] + " " + curPassAttr[3].Substring( 0, 4 ) + "mph" );
                    curTwitterMessage.Append( " " + curRopeLenMetric + "M " + curSpeedKph.ToString() + "kph" );
                    curTwitterMessage.Append( " UNOFFICIAL " + DateTime.Now );
                    ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                } else {
                    if (curStatus.ToLower().Equals( "complete" )) {
                        curTwitterMessage.Append( curSanctionId + " " + curAgeGroup + " " + curSkierName );
                        if (curTeamCode.Length > 0) {
                            curTwitterMessage.Append( " Team: " + curTeamCode );
                        }
                        curTwitterMessage.Append( " Score: " + curScore.ToString( "##0.00" ) );
                        curTwitterMessage.Append( " Pass: " + curScoreRecap.ToString() + " buoys " );
                        curTwitterMessage.Append( " " + curPassAttr[2] + " " + curPassAttr[3].Substring( 0, 4 ) + "mph" );
                        curTwitterMessage.Append( " " + curRopeLenMetric + "M " + curSpeedKph.ToString() + "kph" );
                        curTwitterMessage.Append( " UNOFFICIAL " + DateTime.Now );
                        ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                    }
                }

            }
        }

        private void loadTourEventRegView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false; 

                if ( myTourEventRegDataTable.Rows.Count > 0 ) {
                    slalomRecapDataGridView.Rows.Clear();
                    TourEventRegDataGridView.Rows.Clear();
                    myTourEventRegDataTable.DefaultView.Sort = mySortCommand;
                    myTourEventRegDataTable.DefaultView.RowFilter = myFilterCmd;
                    DataTable curDataTable = myTourEventRegDataTable.DefaultView.ToTable();
                    
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        myEventRegViewIdx = TourEventRegDataGridView.Rows.Add();
                        curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];

                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        try {
                            curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                        } catch {
                            curViewRow.Cells["EventGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        } catch {
                            curViewRow.Cells["AgeGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["EventClass"].Value = (String)curDataRow["EventClass"];
                        } catch {
                            curViewRow.Cells["EventClass"].Value = "";
                        }
                        try {
                            curViewRow.Cells["TeamCode"].Value = (String)curDataRow["TeamCode"];
                        } catch {
                            curViewRow.Cells["TeamCode"].Value = "";
                        }
                        try {
                            curViewRow.Cells["Order"].Value = ((Int16)curDataRow["RunOrder"]).ToString("##0");
                        } catch {
                            curViewRow.Cells["Order"].Value = "0";
                        }
                        try {
                            curViewRow.Cells["Score"].Value = ( (Decimal)curDataRow["Score"] ).ToString( "###.00" );
                        } catch {
                            curViewRow.Cells["Score"].Value = "";
                        }
						try {
							TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = ( (Decimal) curDataRow["Score"] + (Decimal) curDataRow["HCapScore"] ).ToString( "##,###0.0" );
						} catch {
							hcapScoreTextBox.Text = "";
						}
						try {
                            curViewRow.Cells["RankingScore"].Value = ((Decimal)curDataRow["RankingScore"]).ToString("###0.0");
                        } catch {
                            curViewRow.Cells["RankingScore"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RankingRating"].Value = (String)curDataRow["RankingRating"];
                        } catch {
                            curViewRow.Cells["RankingRating"].Value = "";
                        }
                        try {
                            setEventRegRowStatus( (String)curDataRow["Status"], curViewRow );
                        } catch {
                            setEventRegRowStatus( "TBD", curViewRow );
                        }
                        try {
                            curViewRow.Cells["HCapBase"].Value = ( (Decimal) curDataRow["HCapBase"] ).ToString("###0.0");
                        } catch {
                            curViewRow.Cells["HCapBase"].Value = "";
                        }
                        try {
                            curViewRow.Cells["HCapScore"].Value = ( (Decimal) curDataRow["HCapScore"] ).ToString("###0.0");
                        } catch {
                            curViewRow.Cells["HCapScore"].Value = "";
                        }
                    }

                    if ( curDataTable.Rows.Count > 0 ) {
                        myEventRegViewIdx = 0;
                        TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                        setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setNewRowPos();
                    }
                } else {
                    slalomRecapDataGridView.Rows.Clear();
                    TourEventRegDataGridView.Rows.Clear();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving slalom tournament entries \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
            try {
                int curRowPos = myEventRegViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + TourEventRegDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }

            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                LiveWebLabel.Visible = true;
            } else {
                LiveWebLabel.Visible = false;
            }
        }

        private void loadBoatTimeView(Int16 inSpeed) {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving boat times";
            Cursor.Current = Cursors.WaitCursor;

            try {
                listBoatTimesDataGridView.Rows.Clear();
                myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-" + getTimeClass() + "-%'";
                DataTable curDataTable = myTimesDataTable.DefaultView.ToTable();

                if ( curDataTable.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    int curViewIdx = 0;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        curViewIdx = listBoatTimesDataGridView.Rows.Add();
                        curViewRow = listBoatTimesDataGridView.Rows[curViewIdx];
                        try {
                            curViewRow.Cells["BoatTimeKey"].Value = (String)curDataRow["ListCode"];
                        } catch {
                            curViewRow.Cells["BoatTimeKey"].Value = "";
                        }
                        try {
                            curViewRow.Cells["ListCodeNum"].Value = ( (Decimal)curDataRow["ListCodeNum"] ).ToString( "##0" );
                        } catch {
                            curViewRow.Cells["ListCodeNum"].Value = "";
                        }
                        try {
                            curViewRow.Cells["ActualTime"].Value = (String)curDataRow["CodeValue"];
                        } catch {
                            curViewRow.Cells["ActualTime"].Value = "";
                        }
                        try {
                            curViewRow.Cells["FastTimeTol"].Value = ( (Decimal)curDataRow["MinValue"] ).ToString( "#0.00" );
                        } catch {
                            curViewRow.Cells["FastTimeTol"].Value = "";
                        }
                        try {
                            curViewRow.Cells["SlowtimeTol"].Value = ( (Decimal)curDataRow["MaxValue"] ).ToString( "#0.00" );
                        } catch {
                            curViewRow.Cells["SlowtimeTol"].Value = "";
                        }
                        try {
                            curViewRow.Cells["TimeKeyDesc"].Value = (String)curDataRow["CodeDesc"];
                        } catch {
                            curViewRow.Cells["TimeKeyDesc"].Value = "";
                        }
                    }
                    listBoatTimesDataGridView.CurrentCell = listBoatTimesDataGridView.Rows[0].Cells["BoatTimeKey"];
                }
                Cursor.Current = Cursors.Default;
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving boat times \n" + ex.Message );
            }
        }

        private void PauseTimerButton_Click( object sender, EventArgs e ) {
            myEventDelayStartTime = DateTime.Now;
            EventDelayReasonTextBox.Visible = true;
            EventDelayReasonLabel.Visible = true;
            EventDelayReasonTextBox.Text = "";
            EventDelayReasonTextBox.Focus();
            StartTimerButton.Visible = true;
            PauseTimerButton.Visible = false;
        }

        private void StartTimerButton_Click( object sender, EventArgs e ) {
            myEventDelaySeconds += Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventDelayStartTime ) ).TotalSeconds );
            if ( EventDelayReasonTextBox.Text.Length > 1 ) {
                Log.WriteFile( "Slalom:ScoreEntry:Event delayed: " + EventDelayReasonTextBox.Text );
            }
            EventDelayReasonTextBox.Visible = false;
            EventDelayReasonLabel.Visible = false;
            StartTimerButton.Visible = false;
            PauseTimerButton.Visible = true;
        }

        private String getTimeClass() {
            String curTimeClass = "C";
            DataRow curClassRow = getClassRow((String) scoreEventClass.SelectedValue);
            if ( (Decimal)myClassERow["ListCodeNum"] < (Decimal)curClassRow["ListCodeNum"] ) {
                curTimeClass = "R";
            } else if ( (Decimal)myClassCRow["ListCodeNum"] > (Decimal)curClassRow["ListCodeNum"] ) {
                curTimeClass = "W";
            } else {
                curTimeClass = "C";
            }
            return curTimeClass;
        }

        private void setBoatTimeRow( Int16 inSpeed, Int16 inScore ) {
            String curTimeKey = inSpeed.ToString() + "-" + getTimeClass() + "-" + inScore.ToString();
            DataRow[] curTimeRowsFound = myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" );
            if ( curTimeRowsFound.Length > 0 ) {
                myTimeRow = curTimeRowsFound[0];
            } else {
                myTimeRow = null;
            }
        }

        private void setSlalomScoreEntry( DataGridViewRow inTourEventRegRow, int inRound ) {
            Int16 curMinSpeed = 0;

            String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
			String curSkierEventClass = (String) inTourEventRegRow.Cells["EventClass"].Value;
			activeSkierName.Text = (String)inTourEventRegRow.Cells["SkierName"].Value;

			Int16 curDefaultStartSpeed = getMaxSpeedOrigData( curAgeGroup );
			Int16 curMaxSpeed = 0;
			DataRow curClassRow = getClassRow( curSkierEventClass );
			if ( (Decimal) curClassRow["ListCodeNum"] >= (Decimal) myClassERow["ListCodeNum"] ) {
				curMaxSpeed = curDefaultStartSpeed;
			} else {
				curMaxSpeed = getMaxSpeedData( curAgeGroup );
			}

            SlalomSpeedSelect.MaxValue = curMaxSpeed;
            DataTable curMinSpeedDataTable = getMinSpeedData(curAgeGroup);
            if ( curMinSpeedDataTable.Rows.Count > 0 ) {
                myMinSpeedRow = curMinSpeedDataTable.Rows[0];
                curMinSpeed = Convert.ToInt16((Decimal) myMinSpeedRow["MaxValue"]);
                SlalomSpeedSelect.MinValue = curMinSpeed;
            } else {
                myMinSpeedRow = null;
                curMinSpeed = 0;
                SlalomSpeedSelect.MinValue = curMinSpeed;
                SlalomLineSelect.showLongLine();
            }
            SlalomLineSelect.showLongLine();

            getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
            if ( myScoreDataTable.Rows.Count > 0 ) {
                myScoreRow = myScoreDataTable.Rows[0];
                roundSelect.RoundValue = ( (Byte) myScoreRow["Round"] ).ToString();
                try {
                    SlalomLineSelect.CurrentValue = (String) myScoreRow["StartLen"];
                } catch {
                    DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = 18.25");
                    SlalomLineSelect.CurrentValue = (String) curLineRow[0]["ListCode"];
                }
                try {
                    SlalomSpeedSelect.CurrentValue = (Byte) myScoreRow["StartSpeed"];
                } catch {
                    SlalomSpeedSelect.CurrentValue = curMaxSpeed;
                }
                try {
                    scoreEventClass.SelectedValue = (String) myScoreRow["EventClass"];
                } catch {
                    scoreEventClass.SelectedValue = (String) inTourEventRegRow.Cells["EventClass"].Value;
                }
                if ( ( mySkierClassList.compareClassChange((String) scoreEventClass.SelectedValue, "L") <= 0 ) || myTourRules.ToLower().Equals("iwwf") ) {
                    SlalomLineSelect.hideLongLine();
                }
                try {
                    scoreTextBox.Text = ( (Decimal) myScoreRow["Score"] ).ToString("##0.00");
                } catch {
                    scoreTextBox.Text = "";
                }
                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = scoreTextBox.Text;

                try {
                    nopsScoreTextBox.Text = ( (Decimal) myScoreRow["NopsScore"] ).ToString("##,###0.0");
                } catch {
                    nopsScoreTextBox.Text = "";
                }
				
                try {
					hcapScoreTextBox.Text = ( (Decimal) myScoreRow["Score"] + Decimal.Parse((String)inTourEventRegRow.Cells["HCapScore"].Value) ).ToString( "##,###0.0" );
				} catch {
					hcapScoreTextBox.Text = "";
				}

				try {
                    noteTextBox.Text = (String) myScoreRow["Note"];
                } catch {
                    noteTextBox.Text = "";
                }

				if ( myScoreRow["Boat"] == System.DBNull.Value ) {
					TourBoatTextbox.Text = "";
				} else {
					TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String) myScoreRow["Boat"] );
					TourBoatTextbox.Select( 0, 0 );
                }

			} else {
				myScoreRow = null;
                DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = 18.25");

                SlalomLineSelect.CurrentValue = (String) curLineRow[0]["ListCode"];
                SlalomSpeedSelect.CurrentValue = curDefaultStartSpeed;
                scoreEventClass.SelectedValue = (String) inTourEventRegRow.Cells["EventClass"].Value;
                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";
				TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = "";
				if ( ( mySkierClassList.compareClassChange((String) scoreEventClass.SelectedValue, "L") <= 0 ) || myTourRules.ToLower().Equals("iwwf") ) {
                    SlalomLineSelect.hideLongLine();
                } else {
                    SlalomLineSelect.showLongLine();
                }

				scoreTextBox.Text = "";
                nopsScoreTextBox.Text = "";
				hcapScoreTextBox.Text = "";
                noteTextBox.Text = "";
				TourBoatTextbox.Text = buildBoatModelNameDisplay();
				TourBoatTextbox.Select( 0, 0 );
				scoreEntryBegin();
                loadBoatTimeView(curMaxSpeed);
            }
        }

        private void setSlalomRecapEntry(DataGridViewRow inTourEventRegRow, int inRound) {
            Int16 curPassNum;
            String curScoreValue, curFlag;
            Decimal curScore = 0M, curPassLine = 0M;
			isLastPassSelectActive = false;

			Int16 curMaxSpeed = SlalomSpeedSelect.MaxValue;
            String curListName = "SlalomPass" + (Convert.ToInt16(curMaxSpeed)).ToString();
            getPassData( curListName );

			if ( slalomRecapDataGridView.Rows.Count > 0) slalomRecapDataGridView.Rows.Clear();
            String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
            getSkierRecapByRound( curMemberId, curAgeGroup, inRound );

            if ( myRecapDataTable.Rows.Count > 0 ) {
                DataGridViewRow curViewRow;
                int curViewIdx = 0;
                int curViewIdxMax = myRecapDataTable.Rows.Count - 1;
                foreach ( DataRow curDataRow in myRecapDataTable.Rows ) {
                    curViewIdx = slalomRecapDataGridView.Rows.Add();
                    curViewRow = slalomRecapDataGridView.Rows[curViewIdx];

                    curViewRow.Cells["Updated"].Value = "N";
                    curViewRow.Cells["PKRecap"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                    curViewRow.Cells["SanctionIdRecap"].Value = (String)curDataRow["SanctionId"];
                    curViewRow.Cells["MemberIdRecap"].Value = (String)curDataRow["MemberId"];
                    curViewRow.Cells["AgeGroupRecap"].Value = (String)curDataRow["AgeGroup"];
                    curViewRow.Cells["RoundRecap"].Value = ( (Byte)curDataRow["Round"] ).ToString();
                    curViewRow.Cells["RoundRecap"].ToolTipText = ( (DateTime)curDataRow["LastUpdateDate"] ).ToString( "MM/dd HH:mm:ss" );

                    curViewRow.Cells["PassNumRecap"].Value = ((Byte)curDataRow["PassNum"]).ToString();
                    myPassRow = getPassRow( (Byte)curDataRow["PassNum"] );
                    curViewRow.Cells["skierPassRecap"].Value = ( curViewIdx + 1 ).ToString();
                    try {
                        curPassLine = ( (Decimal)curDataRow["PassLineLength"] );
                        curViewRow.Cells["PassLineLengthRecap"].Value = curPassLine.ToString( "00.00" );
                    } catch {
                        if ( myPassRow != null ) {
                            try {
                                curPassLine = (Decimal)myPassRow["MinValue"];
                                curViewRow.Cells["PassLineLengthRecap"].Value = curPassLine.ToString( "00.00" );
                            } catch {
                                curPassLine = 0;
                                curViewRow.Cells["PassLineLengthRecap"].Value = "";
                            }
                        }
                    }
                    try {
                        curFlag = (String)curDataRow["TimeInTol"];
                    } catch {
                        curFlag = "N";
                    }
                    curViewRow.Cells["TimeInTolRecap"].Value = curFlag;
                    if ( curFlag.Equals( "Y" ) ) {
                        curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
                    } else {
                        curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    }
                    try {
                        curViewRow.Cells["BoatTimeRecap"].Value = ( (Decimal)curDataRow["BoatTime"] ).ToString( "#0.00" );
                    } catch {
                        curViewRow.Cells["BoatTimeRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ScoreRecap"].Value = ((Decimal)curDataRow["Score"]).ToString("#.00");
                    } catch {
                        curViewRow.Cells["ScoreRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ScoreProtRecap"].Value = (String)curDataRow["ScoreProt"];
                    } catch {
                        curViewRow.Cells["ScoreProtRecap"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["RerideRecap"].Value = (String)curDataRow["Reride"];
                    } catch {
                        curViewRow.Cells["RerideRecap"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["RerideReasonRecap"].Value = (String)curDataRow["RerideReason"];
                    } catch {
                        curViewRow.Cells["RerideReasonRecap"].Value = "";
                    }

                    try {
                        curViewRow.Cells["Judge1ScoreRecap"].Value = ( (Decimal)curDataRow["Judge1Score"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["Judge1ScoreRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Judge2ScoreRecap"].Value = ( (Decimal)curDataRow["Judge2Score"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["Judge2ScoreRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Judge3ScoreRecap"].Value = ( (Decimal)curDataRow["Judge3Score"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["Judge3ScoreRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Judge4ScoreRecap"].Value = ( (Decimal)curDataRow["Judge4Score"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["Judge4ScoreRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Judge5ScoreRecap"].Value = ( (Decimal)curDataRow["Judge5Score"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["Judge5ScoreRecap"].Value = "";
                    }

                    try {
                        curViewRow.Cells["GateEntry1Recap"].Value = (bool)curDataRow["EntryGate1"];
                    } catch {
                        curViewRow.Cells["GateEntry1Recap"].Value = false;
                    }
                    try {
                        curViewRow.Cells["GateEntry2Recap"].Value = (bool)curDataRow["EntryGate2"];
                    } catch {
                        curViewRow.Cells["GateEntry2Recap"].Value = false;
                    }
                    try {
                        curViewRow.Cells["GateEntry3Recap"].Value = (bool)curDataRow["EntryGate3"];
                    } catch {
                        curViewRow.Cells["GateEntry3Recap"].Value = false;
                    }
                    try {
                        curViewRow.Cells["GateExit1Recap"].Value = (bool)curDataRow["ExitGate1"];
                    } catch {
                        curViewRow.Cells["GateExit1Recap"].Value = false;
                    }
                    try {
                        curViewRow.Cells["GateExit2Recap"].Value = (bool)curDataRow["ExitGate2"];
                    } catch {
                        curViewRow.Cells["GateExit2Recap"].Value = false;
                    }
                    try {
                        curViewRow.Cells["GateExit3Recap"].Value = (bool)curDataRow["ExitGate3"];
                    } catch {
                        curViewRow.Cells["GateExit3Recap"].Value = false;
                    }

                    if ( myPassRow != null ) {
                        curScoreValue = curViewRow.Cells["ScoreRecap"].Value.ToString();
                        if ( curScoreValue.Length == 0 ) { curScoreValue = "0"; }
                        curScore = Convert.ToDecimal( curScoreValue );
                        try {
                            curViewRow.Cells["NoteRecap"].Value = (String)curDataRow["Note"];
                            if ( isObjectEmpty( (String)curViewRow.Cells["NoteRecap"].Value ) ) {
                                curViewRow.Cells["NoteRecap"].Value = (String)myPassRow["CodeValue"];
                            }
                        } catch {
                            curViewRow.Cells["NoteRecap"].Value = "";
                        }

                        Int16 curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                        setBoatTimeRow( curSpeed, 6 );
                        loadBoatTimeView( curSpeed );
                        Decimal curMinTime = (Decimal)myTimeRow["MinValue"];
                        Decimal curMaxTime = (Decimal)myTimeRow["MaxValue"];
                        Decimal curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                        ActivePassDesc.Text = (String)curViewRow.Cells["NoteRecap"].Value;
                        if ( curViewIdx < curViewIdxMax ) {
                            /*
                             */
                            foreach (DataGridViewCell curCell in curViewRow.Cells) {
                                if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                                    curCell.ReadOnly = true;
                                }
                            }
                        } else {
                            if ( roundActiveSelect.RoundValue.Equals( roundSelect.RoundValue ) ) {
                                skierPassMsg.Text = "Boat times full course "
                                    + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                            } else {
                                foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
                                    if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                                        curCell.ReadOnly = true;
                                    }
                                }
                            }
                            if ( curPassLine == 0 ) {
                                curPassLine = (Decimal)myPassRow["MinValue"];
                            }
                            if ( curPassLine < SlalomLineSelect.CurrentValueNum ) {
                                SlalomLineSelect.showCurrentValue( curPassLine );
                            }
                            if ( curSpeed > SlalomSpeedSelect.CurrentValue ) {
                                SlalomSpeedSelect.showCurrentValue( curSpeed );
                            }

                        }
                    }
                }
                myRecapRow = slalomRecapDataGridView.Rows[curViewIdxMax];
                scoreEntryInprogress();
                if ( ( !( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
                    && ( !( isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
                    if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
                        if ( (String)myRecapRow.Cells["TimeInTolRecap"].Value == "Y" ) {
                            if ( roundActiveSelect.RoundValue.Equals( roundSelect.RoundValue ) ) {
                                if ( isExitGatesGood() ) {
                                    isAddRecapRowInProg = true;
                                    isRecapRowEnterHandled = false;
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 5;
                                    curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                                    curTimerObj.Start();
                                } else {
                                    skierPassMsg.ForeColor = Color.Red;
                                    skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
                                }
                            }
                        }
                    }
                }
            } else {
				if ( scoreTextBox.Text.Length > 0 ) {
					try {
						StringBuilder curSqlStmt = new StringBuilder( "Delete SlalomScore " );
						curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
						curSqlStmt.Append( " AND MemberId = '" + (String) inTourEventRegRow.Cells["MemberId"].Value + "' " );
						curSqlStmt.Append( " AND AgeGroup = '" + (String) inTourEventRegRow.Cells["AgeGroup"].Value + "' " );
						curSqlStmt.Append( " AND Round = '" + inRound + "' " );
						int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
						Log.WriteFile( "setSlalomRecapEntry:Found score with no passes.  Deleting score" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

						setSlalomScoreEntry( inTourEventRegRow, inRound );

                    } catch ( Exception excp ) {
						Log.WriteFile( "setSlalomRecapEntry:Error deleting skier score \n" + excp.Message );
					}
					myScoreRow = null;
				}

				scoreEntryBegin();
            }
        }

		private void setEventRegRowStatus( DataGridViewRow curRow ) {
            String curStatus = "", curTimeInTol, curReride, curScoreProt, curMemberId;
            Decimal curScore;
            try {
                curTimeInTol = (String)curRow.Cells["TimeInTolRecap"].Value;
                curReride = (String)curRow.Cells["RerideRecap"].Value;
                curScoreProt = (String)curRow.Cells["ScoreProtRecap"].Value;
                curScore = Convert.ToDecimal( (String)curRow.Cells["ScoreRecap"].Value );
                if ( curTimeInTol.ToUpper().Equals( "Y" ) ) {
                    if ( curScore < 6 ) {
                        curStatus = "4-Done";
                    } else {
                        if ( myNumJudges == 1 ) {
                            if ( (Boolean)curRow.Cells["GateExit1Recap"].Value ) {
                                curStatus = "2-InProg";
                            }
                        } else {
                            int curGateExitValue = 0;
                            if ( !( (Boolean)curRow.Cells["GateExit1Recap"].Value ) ) curGateExitValue++;
                            if ( !( (Boolean)curRow.Cells["GateExit2Recap"].Value ) ) curGateExitValue++;
                            if ( !( (Boolean)curRow.Cells["GateExit3Recap"].Value ) ) curGateExitValue++;
                            if ( curGateExitValue < 2 ) {
                                curStatus = "2-InProg";
                            } else {
                                curStatus = "4-Done";
                            }
                        }
                    }
                } else {
                    if ( curScoreProt.ToUpper().Equals( "Y" ) ) {
                        curStatus = "4-Done";
                    } else {
                        curStatus = "3-Error";
                    }
                }
            } catch {
                curStatus = "1-TBD";
            }
            setEventRegRowStatus( curStatus );
        }

        private void setEventRegRowStatus( String inStatus ) {
            setEventRegRowStatus( inStatus, TourEventRegDataGridView.Rows[myEventRegViewIdx] );
        }
        private void setEventRegRowStatus( String inStatus, DataGridViewRow inRow ) {
            DataGridViewRow curRow = inRow;
            if ( inStatus.Equals( "4-Done" ) || inStatus.Equals( "Done" ) || inStatus.Equals( "Complete" ) ) {
                Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
                curRow.Cells["SkierName"].Style.Font = curFont;
                curRow.Cells["SkierName"].Style.ForeColor = Color.DarkBlue;
                curRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
                curRow.Cells["Status"].Value = "4-Done";
            } if ( inStatus.Equals( "2-InProg" ) || inStatus.Equals( "InProg" ) ) {
                Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
                curRow.Cells["SkierName"].Style.Font = curFont;
                curRow.Cells["SkierName"].Style.ForeColor = Color.White;
                curRow.Cells["SkierName"].Style.BackColor = Color.LimeGreen;
                curRow.Cells["Status"].Value = "2-InProg";
            } if ( inStatus.Equals( "3-Error" ) || inStatus.Equals( "Error" ) ) {
                Font curFont = new Font( "Arial Narrow", 9, FontStyle.Bold );
                curRow.Cells["SkierName"].Style.Font = curFont;
                curRow.Cells["SkierName"].Style.ForeColor = Color.White;
                curRow.Cells["SkierName"].Style.BackColor = Color.Red;
                curRow.Cells["Status"].Value = "3-Error";
            } if ( inStatus.Equals( "1-TBD" ) || inStatus.Equals( "TBD" ) ) {
                Font curFont = new Font( "Arial Narrow", 9, FontStyle.Regular );
                curRow.Cells["SkierName"].Style.Font = curFont;
                curRow.Cells["SkierName"].Style.ForeColor = SystemColors.ControlText;
                curRow.Cells["SkierName"].Style.BackColor = SystemColors.Window;
                curRow.Cells["Status"].Value = "1-TBD";
            }
        }

        private void addPassButton_Click( object sender, EventArgs e ) {
            int cellIndex = 0, rowIndex = 0;
            if (slalomRecapDataGridView.Rows.Count > 0) {
                rowIndex = slalomRecapDataGridView.Rows.Count - 1;
                cellIndex = 0;
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[cellIndex];
            } else {
				if ( checkForSkierRoundScore( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString()
					, Convert.ToInt32( roundSelect.RoundValue )
					, TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() ) ) {
					MessageBox.Show( "Skier already has a score in this round" );
					return;

				} else {
					setEventRegRowStatus( "2-InProg" );

					if ( myCheckOfficials  != null ) {
						/*
						 * Provide a warning message for class R events when official assignments have not been entered for the round and event group
						 * These assignments are not mandatory but they are strongly preferred and are very helpful for the TCs
						 */
						String curEventGroup = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value.ToString();
						if ( !(curEventGroup.Equals(myPrevEventGroup)) ) {
							if ( !( myCheckOfficials.checkOfficialAssignments( mySanctionNum, "Slalom", curEventGroup, roundSelect.RoundValue ) ) ) {
								MessageBox.Show( "No officials have been assigned for this event group and round "
									+ "\n\nThese assignments are not mandatory but they are strongly recommended and are very helpful for the TCs" );
							}

							myPrevEventGroup = curEventGroup;
						}
					}
				}
			}

			myLastPassSelectValue = "";
            addRecapRow();
            setNewRowPos();
        }

        private void addRecapRowTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( addRecapRowTimer );

			isAddRecapRowInProg = false;
			isRecapRowEnterHandled = false;

			addRecapRow();
            setNewRowPos();
        }

        private void addRecapRow() {
            Byte curPassNum = 0;
            Int16 curSkierPass, curMaxSpeed, curPassSpeed, curStartSpeed;
            Int16 curSpeed = 0;
            Decimal curPassLine = 0M, curPassScore = 0M;
            String curRerideInd, curScoreProtInd, curTimeInTolInd;
            isAddRecapRowInProg = true;
            addPassButton.Enabled = false;

            curMaxSpeed = SlalomSpeedSelect.MaxValue;
            String curListName = "SlalomPass" + (Convert.ToInt16(curMaxSpeed)).ToString();
            getPassData( curListName );

            if ( slalomRecapDataGridView.Rows.Count > 0 ) {
                #region Add new pass when at least one pass already exists
                try {
                    curPassNum = Convert.ToByte( (String)myRecapRow.Cells["PassNumRecap"].Value );
                    myPassRow = getPassRow( curPassNum );
                    if ( myPassRow != null ) {
                        curPassSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                        try {
                            curPassLine = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
                            if ( curPassLine > SlalomLineSelect.CurrentValueNum ) {
                                curPassLine = SlalomLineSelect.CurrentValueNum;
                            }
                        } catch {
                            curPassLine = SlalomLineSelect.CurrentValueNum;
                        }
                        try {
                            curPassScore = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreRecap"].Value );
                        } catch {
                            curPassScore = 0M;
                        }
                        curRerideInd = (String)myRecapRow.Cells["RerideRecap"].Value;
                        curScoreProtInd = (String)myRecapRow.Cells["ScoreProtRecap"].Value;
                        curTimeInTolInd = (String)myRecapRow.Cells["TimeInTolRecap"].Value;
                        curSkierPass = (Int16) int.Parse((String) myRecapRow.Cells["skierPassRecap"].Value);

                        if ( ( curTimeInTolInd.Equals( "Y" ) && curRerideInd.Equals( "N" ) && curPassScore == 6 )
                            || ( curTimeInTolInd.Equals( "N" ) && curScoreProtInd.Equals( "Y" ) && curPassScore == 6 )
                            || ( curTimeInTolInd.Equals( "Y" ) && curRerideInd.Equals( "Y" ) && curScoreProtInd.Equals( "N" ) && curPassScore == 6 )
                            || ( curRerideInd.Equals("Y") && curTimeInTolInd.Equals("Y") && curSkierPass == 1 && isExitGatesGood() && !( isEntryGatesGood() ) ) 
                            ) {
                            if ( curPassSpeed < curMaxSpeed ) {
                                String curAgeGroup = ( String ) myRecapRow.Cells["AgeGroupRecap"].Value;
                                DataRow curClassRow = getClassRowCurrentSkier();
                                if ( isQualifiedAltScoreMethod(curAgeGroup, (String)curClassRow["ListCode"] ) ) {
                                    /*
                                    Determine if skier division and class qualify the skier to use the alternate scoring method.
                                    This alternate scoring method no longer required that the skier be scored at long line when at less than max speed
                                    Rule 10.06 for ski year 2017
                                    
                                     */
                                    if ( curRerideInd.Equals("N") 
                                        || (curRerideInd.Equals("Y") && curTimeInTolInd.Equals("Y") && curSkierPass == 1 && isExitGatesGood() && !( isEntryGatesGood()) ) ) {
                                        nextPassWithOption();
                                    } else {
                                        AddNewRecapRow();
                                    }
                                } else {
                                    // Next pass as determined by the standard scoring rules
                                    curPassNum++;
                                    myPassRow = getPassRow(curPassNum);
                                    if ( myPassRow != null ) {
                                        curPassSpeed = Convert.ToInt16((Decimal) myPassRow["MaxValue"]);
                                        if ( curPassSpeed < curMaxSpeed ) {
                                            // Set next pass when skier is still at less than the division maximum speed
                                            if ( isObjectEmpty(myRecapRow.Cells["PassLineLengthRecap"].Value) ) {
                                                curPassLine = SlalomLineSelect.CurrentValueNum;
                                            } else {
                                                curPassLine = Convert.ToDecimal((String) myRecapRow.Cells["PassLineLengthRecap"].Value);
                                                if ( curPassLine > SlalomLineSelect.CurrentValueNum ) {
                                                    curPassLine = SlalomLineSelect.CurrentValueNum;
                                                }
                                            }
                                        } else {
                                            // Set next when skier is at the division maximum speed
                                            if ( isObjectEmpty(myRecapRow.Cells["PassLineLengthRecap"].Value) ) {
                                                curPassLine = SlalomLineSelect.CurrentValueNum;
                                            } else {
                                                curPassLine = Convert.ToDecimal((String) myRecapRow.Cells["PassLineLengthRecap"].Value);
                                            }
                                            myPassRow = getPassRow(curMaxSpeed, curPassLine);
                                            if ( myPassRow != null ) {
                                                curPassNum = Convert.ToByte((Decimal) myPassRow["ListCodeNum"]);
                                            } else {
                                                curPassNum = 0;
                                            }
                                        }

                                        AddNewRecapRow();

                                    } else {
                                        myPassRow = null;
                                        MessageBox.Show("Unable to continue, missing slalom data for pass number " + curPassNum.ToString());
                                    }
                                }

                            } else {
                                addPassNextLine();
                            }
                        } else {
                            if ( curRerideInd.Equals("Y") ) {
                                AddNewRecapRow();
                            }
                        }

                    } else {
                        myPassRow = null;
                        MessageBox.Show( "Unable to continue, missing slalom data for pass number " + curPassNum.ToString() );
                    }

                } catch (Exception ex) {
                    myPassRow = null;
                    MessageBox.Show( String.Format( "Exception encountered on pass number {0}: Exception: {1}", curPassNum.ToString(), ex.Message) );
                }
                #endregion

            } else {
                #region Add new pass when this is the first pass being added
                scoreEntryInprogress();
                curPassLine = SlalomLineSelect.CurrentValueNum;

				curStartSpeed = SlalomSpeedSelect.CurrentValue;
                if ( curMaxSpeed > curStartSpeed ) {
                    curPassLine = Convert.ToDecimal("23.000");

				} else if ( curStartSpeed > curMaxSpeed ) {
                    SlalomSpeedSelect.CurrentValue = curStartSpeed;
                    MessageBox.Show("You have selected a starting speed above the division maximum."
                        + "\n" + "This is allowed although the skier will only be scored at the division maximum speed."
                        + "\n" + "If this was not intended you should delete the pass and re-select the starting speed."
                        );
                }
                Int16 curStartSpeedMph = 0;
                String curStartLenOff = "";
                myPassRow = getPassRow(curStartSpeed, curPassLine);
                if ( myPassRow != null ) {
                    curPassNum = Convert.ToByte((Decimal) myPassRow["ListCodeNum"]);
                    String[] curPassAttr = ( (String) myPassRow["CodeValue"] ).Split(',');
                    curStartLenOff = curPassAttr[2];
                    curStartSpeedMph = Convert.ToInt16(curPassAttr[3].Substring(0, 2));

				} else {
                    curPassNum = 0;
                }
                curPassLine = SlalomLineSelect.CurrentValueNum;
                curSpeed = curStartSpeed;

                AddNewRecapRow();

                if ( ExportLiveScoreboard.ScoreboardLocation.Length > 1 ) {
                    ExportLiveScoreboard.exportCurrentSkierSlalom(mySanctionNum
                        , TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString()
                        , TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString()
                        , (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value
                        , Convert.ToByte(roundSelect.RoundValue)
                        , TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value.ToString()
                        , SlalomSpeedSelect.CurrentValueDesc, curStartLenOff + "/" + SlalomLineSelect.CurrentValue
                        , 0, 0, null);
                }
                #endregion
            }
        }

        private void setNewRowPos() {
            if ( slalomRecapDataGridView.Rows.Count > 0 ) {
                if ( roundActiveSelect.RoundValue.Equals( roundSelect.RoundValue ) ) {
                    optionUpButton.Enabled = true;
                    deletePassButton.Enabled = true;
                    ForceCompButton.Enabled = true;
                    int rowIndex = slalomRecapDataGridView.Rows.Count - 1;
                    slalomRecapDataGridView.Select();
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
                    myRecapRow = slalomRecapDataGridView.Rows[rowIndex];
                } else {
                    foreach (DataGridViewRow curViewRow in slalomRecapDataGridView.Rows) {
                        curViewRow.DefaultCellStyle.BackColor = Color.Salmon;
                        foreach (DataGridViewCell curCell in curViewRow.Cells) {
                            if (curCell.Visible) {
                                curCell.ReadOnly = true;
                            }
                        }
                    }
                    deletePassButton.Enabled = false;
                    addPassButton.Enabled = false;
                    ForceCompButton.Enabled = false;
                    optionUpButton.Enabled = false;
                    int rowIndex = slalomRecapDataGridView.Rows.Count - 1;
                    slalomRecapDataGridView.Select();
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
                    myRecapRow = slalomRecapDataGridView.Rows[rowIndex];
                }
            } else {
                optionUpButton.Enabled = false;
                deletePassButton.Enabled = false;
                ForceCompButton.Enabled = false;
            }

			if ( myLastPassSelectValue.Equals( "Speed" ) ) {
				myNextPassDialog.setNextSpeedButtonDefault();
			} else if ( myLastPassSelectValue.Equals( "Line" ) ) {
				myNextPassDialog.setNextLineButtonDefault();
			} else {
				myNextPassDialog.setNextSpeedButtonDefault();
			}

        }

		private void deletePassButton_Click(object sender, EventArgs e) {
            String curMethodName = "Slalom:ScoreEntry:deletePassButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            int rowsProc = 0, curRowIdx = 0, curRecapPK = -1;
            Int16 curSpeed = 0;
            Byte curPassNum;
            DataRow[] curRowsFound;
            Decimal curPassLine = 0M;

            if ( slalomRecapDataGridView.Rows.Count > 0) {
                try {
                    try {
                        myRecapRow = slalomRecapDataGridView.Rows[slalomRecapDataGridView.Rows.Count - 1];
                        curRowIdx = myRecapRow.Index;
                        curRecapPK = Convert.ToInt32( (String)myRecapRow.Cells["PKRecap"].Value );
                    } catch {
                        curRowIdx = 0;
                        curRecapPK = -1;
                    }
                    if ( curRecapPK > 0 ) {
                        #region Delete row that has previously been scored
                        try {
                            curSqlStmt = new StringBuilder( "Delete SlalomRecap Where PK = " + curRecapPK.ToString() );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        } catch (Exception excp) {
                            curMsg = curMethodName + "Error deleting slalom pass \n" + excp.Message;
                            MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                        if ( rowsProc > 0 ) {
                            myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
                            curPassNum = Convert.ToByte( (String)myRecapRow.Cells["PassNumRecap"].Value );
                            myPassRow = getPassRow( curPassNum );
                            if ( myPassRow != null ) {
                                curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                                if ( curSpeed != SlalomSpeedSelect.CurrentShowValueNum ) {
                                    SlalomSpeedSelect.showCurrentValue(curSpeed);
                                }
                                try {
                                    curPassLine = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
                                } catch {
                                    curPassLine = 0;
                                }
                                if ( curPassLine < SlalomLineSelect.CurrentShowValueNum ) {
                                    curPassLine = SlalomLineSelect.CurrentShowValueNum;
                                }
                                if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                                    SlalomLineSelect.showCurrentValue( curPassLine );
                                }
                            }
                            slalomRecapDataGridView.Rows.RemoveAt( curRowIdx );
                            if ( curRowIdx > 0 ) {
                                curRowIdx--;
                                myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
                                curPassNum = Convert.ToByte( (String)myRecapRow.Cells["PassNumRecap"].Value );
                                myPassRow = getPassRow( curPassNum );
                                if ( myPassRow != null ) {
                                    curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                                    if ( curSpeed != SlalomSpeedSelect.CurrentShowValueNum ) {
                                        SlalomSpeedSelect.showCurrentValue( curSpeed );
                                    }
                                    try {
                                        curPassLine = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value);
                                    } catch {
                                        curPassLine = 0;
                                    }
                                    if ( curPassLine < SlalomLineSelect.CurrentShowValueNum ) {
                                        curPassLine = SlalomLineSelect.CurrentShowValueNum;
                                    }
                                    if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                                        SlalomLineSelect.showCurrentValue( curPassLine );
                                    }
                                }
                                SlalomScoreCalc( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreRecap"].Value ) );

                                if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                                    String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                                    String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                                    String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                                    byte curRound = Convert.ToByte( roundSelect.RoundValue );
                                    ExportLiveWeb.exportCurrentSkierSlalom( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
                                }

                                scoreEntryInprogress();
                                setEventRegRowStatus( "2-InProg" );
                                foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
                                    if ( curCell.OwningColumn.Name.StartsWith( "Judge" )
                                        || curCell.OwningColumn.Name.StartsWith( "Gate" )
                                        || curCell.OwningColumn.Name.StartsWith( "Time" )
                                        || curCell.OwningColumn.Name.StartsWith( "BoatTime" )
                                        || curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
                                        || curCell.OwningColumn.Name.StartsWith( "Reride" )
                                        || curCell.OwningColumn.Name.StartsWith( "Note" )
                                        || curCell.OwningColumn.Name.StartsWith( "Updated" )
                                        ) {
                                        curCell.ReadOnly = false;
                                    }
                                }
                                setNewRowPos();
                                ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
                                if ( curSpeed > 0 ) {
                                    setBoatTimeRow( curSpeed, 6 );
                                    loadBoatTimeView( curSpeed );
                                    Decimal curMinTime = (Decimal)myTimeRow["MinValue"];
                                    Decimal curMaxTime = (Decimal)myTimeRow["MaxValue"];
                                    Decimal curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                                    skierPassMsg.Text = "Boat times full course "
                                        + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                                }
                            } else {
                                myRecapRow = null;
								try {
									curSqlStmt = new StringBuilder( "Delete SlalomScore " );
									curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
									curSqlStmt.Append( " AND MemberId = '" + (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value + "' " );
									curSqlStmt.Append( " AND AgeGroup = '" + (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value + "' " );
									curSqlStmt.Append( " AND Round = '" + Convert.ToByte( roundSelect.RoundValue ) + "' " );
									rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
									Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

									if ( ExportLiveWeb.LiveWebLocation.Length > 1 ) {
										String curEventGroup = (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
										String curMemberId = (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
										String curAgeGroup = (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
										byte curRound = Convert.ToByte( roundSelect.RoundValue );
										ExportLiveWeb.exportCurrentSkierSlalom( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
									}

								} catch ( Exception excp ) {
									curMsg = ":Error deleting skier score \n" + excp.Message;
									Log.WriteFile( curMethodName + curMsg );
								}
								myScoreRow = null;

                                SlalomSpeedSelect.CurrentValue = SlalomSpeedSelect.CurrentValue;
                                SlalomLineSelect.CurrentValue = SlalomLineSelect.CurrentValue;

                                addPassButton.Enabled = true;
                                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = ""; 
                                scoreTextBox.Text = "";
                                nopsScoreTextBox.Text = "";
								hcapScoreTextBox.Text = "";
								noteTextBox.Text = "";
								scoreEntryBegin();
                                setEventRegRowStatus( "1-TBD" );
                                refreshScoreSummaryWindow();
                                setNewRowPos();
                            }
                        }
                        #endregion
                    } else {
                        #region Delete row that has not yet been scored
                        try {
                            curPassNum = Convert.ToByte( (String)myRecapRow.Cells["PassNumRecap"].Value );
                            myPassRow = getPassRow( curPassNum );
                            if ( myPassRow != null ) {
                                curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                                if ( curSpeed != SlalomSpeedSelect.CurrentShowValueNum ) {
                                    SlalomSpeedSelect.showCurrentValue( curSpeed );
                                }
                                try {
                                    curPassLine = Convert.ToDecimal( (String)myRecapRow.Cells["PassLineLengthRecap"].Value );
                                } catch {
                                    curPassLine = 0;
                                }
                                if ( curPassLine > SlalomLineSelect.CurrentShowValueNum ) {
                                    curPassLine = SlalomLineSelect.CurrentShowValueNum;
                                }
                                if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                                    SlalomLineSelect.showCurrentValue(curPassLine);
                                }
                            }
                            slalomRecapDataGridView.Rows.RemoveAt( curRowIdx );
                        } catch {
                            curRowIdx = -1;
                        }
                        if ( curRowIdx > 0 ) {
                            curSpeed = 0;
                            curRowIdx--;
                            myRecapRow = (DataGridViewRow)slalomRecapDataGridView.Rows[curRowIdx];
                            curPassNum = Convert.ToByte( (String)myRecapRow.Cells["PassNumRecap"].Value );
                            myPassRow = getPassRow( curPassNum );
                            if ( myPassRow != null ) {
                                curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                                if ( curSpeed != SlalomSpeedSelect.CurrentShowValueNum ) {
                                    SlalomSpeedSelect.showCurrentValue(curSpeed);
                                }
                                try {
                                    curPassLine = Convert.ToDecimal((String) myRecapRow.Cells["PassLineLengthRecap"].Value);
                                } catch {
                                    curPassLine = 0;
                                }
                                if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                                    SlalomLineSelect.showCurrentValue(curPassLine);
                                }
                            }
                            SlalomScoreCalc( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreRecap"].Value ) );
                            scoreEntryInprogress();
                            setEventRegRowStatus( "2-InProg" );
                            foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
                                if ( curCell.OwningColumn.Name.StartsWith( "Judge" )
                                    || curCell.OwningColumn.Name.StartsWith( "Gate" )
                                    || curCell.OwningColumn.Name.StartsWith( "Time" )
                                    || curCell.OwningColumn.Name.StartsWith( "BoatTime" )
                                    || curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
                                    || curCell.OwningColumn.Name.StartsWith( "Reride" )
                                    || curCell.OwningColumn.Name.StartsWith( "Note" )
                                    || curCell.OwningColumn.Name.StartsWith( "Updated" )
                                    ) {
                                    curCell.ReadOnly = false;
                                }
                            }
                            setNewRowPos();

                            ActivePassDesc.Text = (String)myRecapRow.Cells["NoteRecap"].Value;
                            if ( curSpeed > 0 ) {
                                setBoatTimeRow( curSpeed, 6 );
                                loadBoatTimeView( curSpeed );
                                Decimal curMinTime = (Decimal)myTimeRow["MinValue"];
                                Decimal curMaxTime = (Decimal)myTimeRow["MaxValue"];
                                Decimal curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                                skierPassMsg.Text = "Boat times full course "
                                    + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                            }
                        } else {
                            myScoreRow = null;
                            addPassButton.Enabled = true;
                            TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";
                            scoreTextBox.Text = "";
                            nopsScoreTextBox.Text = "";
							hcapScoreTextBox.Text = "";
							noteTextBox.Text = "";
                            scoreEntryBegin();
                            setEventRegRowStatus( "1-TBD" );
                            refreshScoreSummaryWindow();
                            setNewRowPos();
                        }
                        #endregion
                    }
                } catch ( Exception excp ) {
                    curMsg = ":Error attempting to save changes \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
            }
            slalomRecapDataGridView.Focus();
        }

        private void nextPassWithOption() {
			if ( isLastPassSelectActive ) {
				return;
			}
			String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
            Int16 curMaxSpeed = getMaxSpeedOrigData(curAgeGroup);
            Int16 curSpeed = Convert.ToInt16((Decimal) myPassRow["MaxValue"]);
            if ( curSpeed < curMaxSpeed ) {
                int curRowIdx = slalomRecapDataGridView.Rows.Count - 1;
				//
				int curLocationX = this.MdiParent.Location.X + this.Location.X
					+ slalomRecapDataGridView.Location.X 
					+ slalomRecapDataGridView.GetCellDisplayRectangle(NoteRecap.Index, curRowIdx, false).Location.X;
				// 40 + 25 + 60 + 
				int curLocationY = 85 + this.MdiParent.Location.Y + this.Location.Y
					+ slalomRecapDataGridView.Location.Y 
					+ slalomRecapDataGridView.GetCellDisplayRectangle(NoteRecap.Index, curRowIdx, false).Location.Y + 22;
				myNextPassDialog.setWindowLocation(curLocationX, curLocationY);

				if ( myLastPassSelectValue.Equals("Speed") ) {
					myNextPassDialog.setNextSpeedButtonDefault();
				} else if ( myLastPassSelectValue.Equals("Line") ) {
					myNextPassDialog.setNextLineButtonDefault();
				} else {
					myNextPassDialog.setNextSpeedButtonDefault();
				}

				isAddRecapRowInProg = false;
				isRecapRowEnterHandled = false;
				isLastPassSelectActive = true;
				if ( myNextPassDialog.ShowDialog() == DialogResult.OK ) {
					if ( myNextPassDialog.getDialogResult().Equals( "Line" ) ) {
						myLastPassSelectValue = myNextPassDialog.getDialogResult();
                        addPassNextLine();
						isAddRecapRowInProg = true;

					} else if ( myNextPassDialog.getDialogResult().Equals( "Speed" ) ) {
						myLastPassSelectValue = myNextPassDialog.getDialogResult();
						addPassNextSpeed();
						isAddRecapRowInProg = true;
                    }
				}
				isLastPassSelectActive = false;

			} else {
                addPassNextLine();
				isAddRecapRowInProg = true;
			}

			return;
		}

		private void addPassNextSpeed() {
            myLastPassSelectValue = "Speed";

            Byte curPassNum = Convert.ToByte((String) myRecapRow.Cells["PassNumRecap"].Value);
            curPassNum++;
            myPassRow = getPassRow(curPassNum);
            if ( myPassRow != null ) {
                Decimal curPassLine = 0M;
                if ( isObjectEmpty(myRecapRow.Cells["PassLineLengthRecap"].Value) ) {
                    curPassLine = SlalomLineSelect.CurrentShowValueNum;
                } else {
                    curPassLine = Convert.ToDecimal((String) myRecapRow.Cells["PassLineLengthRecap"].Value);
                    if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                        SlalomLineSelect.showCurrentValue(curPassLine);
                    }
                }

                Int16 curMaxSpeed = SlalomSpeedSelect.MaxValue;
                Int16 curSpeed = Convert.ToInt16((Decimal) myPassRow["MaxValue"]);
                if ( curSpeed >= curMaxSpeed ) {
                    Decimal nextPassLine = (Decimal) myPassRow["MinValue"];
                    if ( nextPassLine > curPassLine ) {
                        myPassRow = getPassRow(curSpeed, curPassLine);
                    }
                }

                AddNewRecapRow();
            }
        }

        private void addPassNextLine() {
            myLastPassSelectValue = "Line";

            int curViewIdx = myRecapRow.Index;
            String curPassLineValue = (String) myRecapRow.Cells["PassLineLengthRecap"].Value;
            if ( curPassLineValue.Length > 1 ) {
                String curLineLength = SlalomLineSelect.getNextValue(curPassLineValue);
                Decimal curPassLine = Convert.ToDecimal(curLineLength);
                if ( curPassLine != SlalomLineSelect.CurrentShowValueNum ) {
                    SlalomLineSelect.showCurrentValue(curPassLine);
                }
                Int16 curPassSpeed = Convert.ToInt16((Decimal) myPassRow["MaxValue"]);
                myPassRow = getPassRow(curPassSpeed, curPassLine);
                AddNewRecapRow();

			} else {
                MessageBox.Show("Line length for pass not available.  Opting up by line length has failed");
            }

		}

		/*
         * Add new slalom pass row to data grid view
         */
		private void AddNewRecapRow() {
            if ( myRecapRow != null ) {
                foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
                    if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                        curCell.ReadOnly = true;
                    }
                }
            }

            slalomRecapDataGridView.Focus();
            int curViewIdx = slalomRecapDataGridView.Rows.Add();
            myRecapRow = slalomRecapDataGridView.Rows[curViewIdx];

            Int16 curPassNum = Convert.ToByte((Decimal) myPassRow["ListCodeNum"]);
            Int16 curPassSpeed = Convert.ToInt16((Decimal) myPassRow["MaxValue"]);
            SlalomSpeedSelect.showCurrentValue(curPassSpeed);

            myRecapRow.Cells["Updated"].Value = "Y";
            myRecapRow.Cells["PKRecap"].Value = "-1";
            myRecapRow.Cells["SanctionIdRecap"].Value = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SanctionId"].Value.ToString();
            myRecapRow.Cells["MemberIdRecap"].Value = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString();
            myRecapRow.Cells["AgeGroupRecap"].Value = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString();
            myRecapRow.Cells["RoundRecap"].Value = roundSelect.RoundValue;
            myRecapRow.Cells["RoundRecap"].ToolTipText = "";

            myRecapRow.Cells["PassNumRecap"].Value = curPassNum.ToString("#0");
            myRecapRow.Cells["skierPassRecap"].Value = Convert.ToInt16(myRecapRow.Index + 1).ToString("#0");
            myRecapRow.Cells["PassLineLengthRecap"].Value = SlalomLineSelect.CurrentShowValueNum.ToString("00.00");

            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.clock;
            myRecapRow.Cells["BoatTimeRecap"].Value = "";
            myRecapRow.Cells["ScoreRecap"].Value = "";
            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapRow.Cells["RerideRecap"].Value = "N";
            myRecapRow.Cells["RerideReasonRecap"].Value = "";
            myRecapRow.Cells["Judge1ScoreRecap"].Value = "";
            myRecapRow.Cells["Judge2ScoreRecap"].Value = "";
            myRecapRow.Cells["Judge3ScoreRecap"].Value = "";
            myRecapRow.Cells["Judge4ScoreRecap"].Value = "";
            myRecapRow.Cells["Judge5ScoreRecap"].Value = "";

            myRecapRow.Cells["GateEntry1Recap"].Value = true;
            myRecapRow.Cells["GateEntry2Recap"].Value = true;
            myRecapRow.Cells["GateEntry3Recap"].Value = true;
            myRecapRow.Cells["GateExit1Recap"].Value = true;
            myRecapRow.Cells["GateExit2Recap"].Value = true;
            myRecapRow.Cells["GateExit3Recap"].Value = true;

            try {
                DataRow curClassRow = getClassRowCurrentSkier();
                if ( isQualifiedAltScoreMethod((String) myRecapRow.Cells["AgeGroupRecap"].Value, (String) curClassRow["ListCode"]) ) {
                    //e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
                    String[] curPassDescNodes = ( (String) myPassRow["CodeValue"] ).Split(',');
                    DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = " + SlalomLineSelect.CurrentShowValueNum.ToString("00.00"));

                    String curLineLengthMetric = (String) curLineRow[0]["ListCode"];
                    String curLineLengthStd = (String) curLineRow[0]["CodeValue"];

                    int curDelimIdx = curLineLengthStd.IndexOf(" - ");
                    curLineLengthStd = curLineLengthStd.Substring(0, curDelimIdx);

                    myRecapRow.Cells["NoteRecap"].Value =
                        curLineLengthMetric
                        + "," + curPassDescNodes[1]
                        + "," + curLineLengthStd
                        + "," + curPassDescNodes[3];
                } else {
                    myRecapRow.Cells["NoteRecap"].Value = (String) myPassRow["CodeValue"];
                }

                //Int16 curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                setBoatTimeRow(curPassSpeed, 6);
                loadBoatTimeView(curPassSpeed);
                Decimal curMinTime = (Decimal) myTimeRow["MinValue"];
                Decimal curMaxTime = (Decimal) myTimeRow["MaxValue"];
                Decimal curActualTime = Convert.ToDecimal((String) myTimeRow["CodeValue"]);
                skierPassMsg.Text = "Boat times full course "
                    + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();

                setNewRowPos();

            } catch {
                myRecapRow.Cells["NoteRecap"].Value = "Can not continue pass with missing slalom pass data";
            }
            ActivePassDesc.Text = (String) myRecapRow.Cells["NoteRecap"].Value;
            isAddRecapRowInProg = false;
            myPassRunCount++;
            if ( curViewIdx == 0 ) {
                mySkierRunCount++;
            } else {
                if ( mySkierRunCount == 0 ) {
                    mySkierRunCount++;
                }
            }

        }

        private void optionUpButton_Click(object sender, EventArgs e) {
            Byte curPassNum = 0;
            Int16 curMaxSpeed = 0;
            Decimal curPassLine = 23M;

            if ( slalomRecapDataGridView.Rows.Count == 0 ) return;

			bool curOptAllow = true;
			curMaxSpeed = SlalomSpeedSelect.MaxValue;
			String curAgeGroup = (String) myRecapRow.Cells["AgeGroupRecap"].Value;

			String curListName = "SlalomPass" + ( Convert.ToInt16( curMaxSpeed ) ).ToString();
			getPassData( curListName );
			if ( !( isObjectEmpty( (String) myRecapRow.Cells["ScoreRecap"].Value ) ) ) {
				Decimal curPassScore = 0M;
				try {
					curPassScore = Convert.ToDecimal( (String) myRecapRow.Cells["ScoreRecap"].Value );
				} catch {
					curPassScore = 0M;
				}
				String curRerideInd = (String) myRecapRow.Cells["RerideRecap"].Value;
				String curScoreProtInd = (String) myRecapRow.Cells["ScoreProtRecap"].Value;
				String curTimeInTolInd = (String) myRecapRow.Cells["TimeInTolRecap"].Value;
				if ( curScoreProtInd.Equals( "Y" ) && curPassScore == 6 ) {
					addRecapRow();
				} else if ( curRerideInd.Equals( "N" ) ) {
					if ( curTimeInTolInd.Equals( "Y" ) ) {
						MessageBox.Show( "Opting up is not allowed when current pass has a valid time"
							+ "\nand no explict request and reason for a reride" );
						curOptAllow = false;
					} else {
						MessageBox.Show( "Opting up is not allowed when current pass has a time out of tolerance"
							+ "\nand no explict request and reason for a reride" );
						curOptAllow = false;
					}
				} else if ( curRerideInd.Equals( "Y" ) ) {
					addRecapRow();
				}
			}

			if ( curOptAllow ) {
				String curOptUpType = "next";
				Int16 curSpeed = 0;
				curPassNum = Convert.ToByte( (String) myRecapRow.Cells["PassNumRecap"].Value );
				myPassRow = getPassRow( curPassNum );
				if ( myPassRow != null ) {
					curSpeed = Convert.ToInt16( (Decimal) myPassRow["MaxValue"] );
				}

				if ( curSpeed < curMaxSpeed ) {
					if ( OptUpDialogForm.ShowDialog() == DialogResult.OK ) {
						String curResponse = OptUpDialogForm.Response;
						if ( curResponse.Equals( "speed" ) ) {
							curOptUpType = "next";
						} else if ( curResponse.Equals( "line" ) ) {
							curOptUpType = curResponse;
						} else {
							curOptUpType = "next";
						}
					} else {
						curOptUpType = "";
					}
				}

				if ( curOptUpType.Equals( "next" ) ) {
					if ( curSpeed < curMaxSpeed ) {
						curPassNum++;
						myPassRow = getPassRow( curPassNum );
						if ( myPassRow != null ) {
							curSpeed = Convert.ToInt16( (Decimal) myPassRow["MaxValue"] );
							if ( curSpeed != SlalomSpeedSelect.CurrentValue ) {
								SlalomSpeedSelect.showCurrentValue( curSpeed );
							}

							Int16 curMaxSpeedDiv = getMaxSpeedOrigData( (String) myRecapRow.Cells["AgeGroupRecap"].Value );
							if ( curSpeed < curMaxSpeedDiv ) {
								if ( isObjectEmpty( myRecapRow.Cells["PassLineLengthRecap"].Value ) ) {
									curPassLine = SlalomLineSelect.CurrentValueNum;
								} else {
									curPassLine = Convert.ToDecimal( (String) myRecapRow.Cells["PassLineLengthRecap"].Value );
									if ( curPassLine > SlalomLineSelect.CurrentValueNum ) {
										curPassLine = SlalomLineSelect.CurrentValueNum;
									}
								}
							} else {
								curPassLine = (Decimal) myPassRow["MinValue"];
								if ( curPassLine > SlalomLineSelect.CurrentShowValueNum ) {
									curPassLine = SlalomLineSelect.CurrentShowValueNum;
								}
								if ( curPassLine == 0M ) {
									curPassLine = SlalomLineSelect.CurrentShowValueNum;
								}
								if ( myRecapRow.Index > 0 ) {
									Decimal curPassLineTemp = Convert.ToDecimal( (String) slalomRecapDataGridView.Rows[myRecapRow.Index - 1].Cells["PassLineLengthRecap"].Value );
									if ( curPassLine < curPassLineTemp ) {
										curPassLine = curPassLineTemp;
										SlalomLineSelect.showCurrentValue( curPassLine );
                                    } else {
										if ( curPassLine > SlalomLineSelect.CurrentShowValueNum ) {
											curPassLine = SlalomLineSelect.CurrentShowValueNum;
										}
									}
									myPassRow = getPassRow( curSpeed, curPassLine );
									curPassNum = Convert.ToByte( (Decimal) myPassRow["ListCodeNum"] );
								}
							}
						}
					} else {
						curPassNum++;
						myPassRow = getPassRow( curPassNum );
						if ( myPassRow != null ) {
							curPassLine = (Decimal) myPassRow["MinValue"];
						} else {
							curPassLine = 23M;
						}
						if ( curPassLine < SlalomLineSelect.CurrentValueNum ) {
							SlalomLineSelect.showCurrentValue( curPassLine );
						}
					}

					if ( myPassRow != null ) {
						myRecapRow.Cells["PassNumRecap"].Value = curPassNum.ToString();
						myRecapRow.Cells["PassLineLengthRecap"].Value = curPassLine.ToString( "00.00" );

						DataRow curClassRow = getClassRowCurrentSkier();
						if ( isQualifiedAltScoreMethod( (String) myRecapRow.Cells["AgeGroupRecap"].Value, (String) curClassRow["ListCode"] ) ) {
							//e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
							String[] curPassDescNodes = ( (String) myPassRow["CodeValue"] ).Split( ',' );
							DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select( "ListCodeNum = " + curPassLine.ToString( "00.00" ) );
							String curLineLengthMetric = (String) curLineRow[0]["ListCode"];
							String curLineLengthStd = (String) curLineRow[0]["CodeValue"];
							int curDelimIdx = curLineLengthStd.IndexOf( " - " );
							curLineLengthStd = curLineLengthStd.Substring( 0, curDelimIdx );

							myRecapRow.Cells["NoteRecap"].Value = "Opt up to "
								+ curLineLengthMetric
								+ "," + curPassDescNodes[1]
								+ "," + curLineLengthStd
								+ "," + curPassDescNodes[3];
						} else {
							myRecapRow.Cells["NoteRecap"].Value = "Opt up to " + (String) myPassRow["CodeValue"];
						}

						ActivePassDesc.Text = (String) myRecapRow.Cells["NoteRecap"].Value;
						setNewRowPos();
						curSpeed = Convert.ToInt16( (Decimal) myPassRow["MaxValue"] );
						loadBoatTimeView( curSpeed );
						if ( curSpeed != SlalomSpeedSelect.CurrentValue ) {
							SlalomSpeedSelect.showCurrentValue( curSpeed );
						}
						if ( curPassLine < SlalomLineSelect.CurrentValueNum ) {
							SlalomLineSelect.showCurrentValue( curPassLine );
						}
					} else {
						myPassRow = null;
						MessageBox.Show( "Unable to continue, missing slalom data for pass number " + curPassNum.ToString() );
					}

				} else if ( curOptUpType.Equals( "line" ) ) {
					String curPassLineValue = (String) myRecapRow.Cells["PassLineLengthRecap"].Value;
					if ( curPassLineValue.Length > 1 ) {
						String curLineLength = SlalomLineSelect.getNextValue( curPassLineValue );
						myRecapRow.Cells["PassLineLengthRecap"].Value = curLineLength;
						curPassLine = Convert.ToDecimal( curLineLength );
						if ( curPassLine < SlalomLineSelect.CurrentValueNum ) {
							SlalomLineSelect.showCurrentValue( curPassLine );
						}

						DataRow curClassRow = getClassRowCurrentSkier();
						if ( isQualifiedAltScoreMethod( (String) myRecapRow.Cells["AgeGroupRecap"].Value, (String) curClassRow["ListCode"] ) ) {
							//e.g. 23.00M,25kph,Long,15.5mph  OR  18.25M,46kph,15 Off,28.6mph
							String[] curPassDescNodes = ( (String) myPassRow["CodeValue"] ).Split( ',' );
							DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select( "ListCodeNum = " + curPassLine.ToString( "00.00" ) );
							String curLineLengthMetric = (String) curLineRow[0]["ListCode"];
							String curLineLengthStd = (String) curLineRow[0]["CodeValue"];
							int curDelimIdx = curLineLengthStd.IndexOf( " - " );
							curLineLengthStd = curLineLengthStd.Substring( 0, curDelimIdx );

							myRecapRow.Cells["NoteRecap"].Value = "Opt up to "
								+ curLineLengthMetric
								+ "," + curPassDescNodes[1]
								+ "," + curLineLengthStd
								+ "," + curPassDescNodes[3];
						} else {
							myRecapRow.Cells["NoteRecap"].Value = "Opting up to " + curLineLength + " at " + curSpeed + "kph";
						}
						ActivePassDesc.Text = (String) myRecapRow.Cells["NoteRecap"].Value;

						MessageBox.Show( "Opting up to " + curLineLength + " at " + curSpeed + "kph" );
					} else {
						MessageBox.Show( "Line length for pass not available.  Opting up by line length has failed" );
					}
				}
			}
			slalomRecapDataGridView.Focus();
		}

		private void refreshScoreSummaryWindow() {
            // Check for open instance of selected form
            //SystemMain curMdiWindow = (SystemMain)this.Parent.Parent;
            //for ( int idx = 0; idx < curMdiWindow.MdiChildren.Length; idx++ ) {
            //    Form curForm = (Form)( curMdiWindow.MdiChildren[idx] );
            //    if ( curForm.Name.Equals( "SlalomSummary" ) ) {
            //        SlalomSummary curSummaryWindow = (SlalomSummary)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "SlalomSummaryTeam" ) ) {
            //        SlalomSummaryTeam curSummaryWindow = (SlalomSummaryTeam)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "MasterSummaryOverall" ) ) {
            //        MasterSummaryOverall curSummaryWindow = (MasterSummaryOverall)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "MasterSummaryV2" ) ) {
            //        MasterSummaryV2 curSummaryWindow = (MasterSummaryV2)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "MasterSummaryOverallUS" ) ) {
            //        MasterSummaryOverallUS curSummaryWindow = (MasterSummaryOverallUS)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "MasterSummaryTeam" ) ) {
            //        MasterSummaryTeam curSummaryWindow = (MasterSummaryTeam)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    }
            //}
        }

        private void navSort_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;
                myTourProperties.RunningOrderSortSlalom = mySortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCommand;
                loadTourEventRegView();
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadTourEventRegView();
            }
        }

        private void navExportRecord_Click(object sender, EventArgs e) {
            ExportRecordAppData myExportData = new ExportRecordAppData();
            DataGridViewRow curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
            String curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
            byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );
            myExportData.ExportData( mySanctionNum, "Slalom", curMemberId, curAgeGroup, curEventGroup, curRound );
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[8];
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "SlalomScore", "SlalomRecap", "TourReg", "OfficialWork", "OfficialWorkAsgmt" };
            String curFilterCmd = myFilterCmd;
            if ( curFilterCmd.Contains("Div =") ) {
                curFilterCmd = curFilterCmd.Replace("Div =", "XT.AgeGroup =");

			} else if ( curFilterCmd.Contains( "AgeGroup not in" ) ) {
				curFilterCmd = curFilterCmd.Replace( "AgeGroup not in", "XT.AgeGroup not in" );

			} else if ( curFilterCmd.Contains("AgeGroup =") ) {
                curFilterCmd = curFilterCmd.Replace("AgeGroup =", "XT.AgeGroup =");
            }
			// 
			curSelectCommand[0] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[0] = curSelectCommand[0] + "And " + curFilterCmd + " ";
            }
            
            curSelectCommand[1] = "Select * from EventReg XT ";
            if ( isObjectEmpty( curFilterCmd ) ) {
                curSelectCommand[1] = curSelectCommand[1]
                    + "Where SanctionId = '" + mySanctionNum + "' "
                    + "And Event = 'Slalom' ";
            } else {
                if ( curFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + "Where SanctionId = '" + mySanctionNum + "' "
                        + "And Event = 'Slalom' "
                        + "And " + curFilterCmd;
                } else {
                    curSelectCommand[1] = curSelectCommand[1]
                        + "Where SanctionId = '" + mySanctionNum + "' "
                        + "And Event = 'Slalom'";
                }
            }

            curSelectCommand[2] = "Select * from EventRunOrder XT ";
            if (isObjectEmpty( curFilterCmd )) {
                curSelectCommand[2] = curSelectCommand[2]
                    + "Where SanctionId = '" + mySanctionNum + "' "
                    + "And Event = 'Slalom' And Round = " + roundActiveSelect.RoundValue + " ";
            } else {
                if (curFilterCmd.Length > 0) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + "Where SanctionId = '" + mySanctionNum + "' "
                        + "And Event = 'Slalom' And Round = " + roundActiveSelect.RoundValue + " "
                        + "And " + curFilterCmd;
                } else {
                    curSelectCommand[2] = curSelectCommand[2]
                        + "Where SanctionId = '" + mySanctionNum + "' "
                        + "And Event = 'Slalom' And Round = " + roundActiveSelect.RoundValue + " ";
                }
            }

            curSelectCommand[3] = "SELECT XT.* FROM SlalomScore XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And Round = " + roundActiveSelect.RoundValue + " ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[3] = curSelectCommand[3] + "And " + curFilterCmd + " ";
            }

            curSelectCommand[4] = "SELECT XT.* FROM SlalomRecap XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Slalom' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And Round = " + roundActiveSelect.RoundValue + " ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[4] = curSelectCommand[4] + "And " + curFilterCmd + " ";
            }

            //----------------------------------------
            //Export data related to officials
            //----------------------------------------
            String tmpFilterCmd = "";
            String curEventGroup = EventGroupList.SelectedItem.ToString();
            if ( curEventGroup.ToLower().Equals("all") ) {
            } else {
                tmpFilterCmd = "And EventGroup = '" + curEventGroup + "' ";
            }

            //----------------------------------------
            //Export data related to officials
            //----------------------------------------
            curSelectCommand[5] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' AND ER.Round = " + roundActiveSelect.RoundValue + " "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[5] = curSelectCommand[5] + tmpFilterCmd + " ";
            }

            curSelectCommand[6] = "SELECT XT.* FROM OfficialWork XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And XT.LastUpdateDate is not null ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + " ";
            }
            curSelectCommand[6] = curSelectCommand[6] + "Union "
                + "SELECT XT.* FROM OfficialWork XT "
                + "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Slalom' AND ER.Round = " + roundActiveSelect.RoundValue + " "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And XT.LastUpdateDate is not null ";
            if ( !(isObjectEmpty( tmpFilterCmd )) && tmpFilterCmd.Length > 0 ) {
                    curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + " ";
            }

            curSelectCommand[7] = "Select * from OfficialWorkAsgmt XT "
                + "Where SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' And Round = " + roundActiveSelect.RoundValue + " ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[7] = curSelectCommand[7] + tmpFilterCmd + " ";
            }

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navRefreshTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( navRefreshTimer );
            navRefresh_Click( null, null );
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            myFilterCmd = "";
            String curGroupValue = "All";
            try {
                curGroupValue = EventGroupList.SelectedItem.ToString();
                if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                    if ( curGroupValue.ToUpper().Equals( "MEN A" ) ) {
                        myFilterCmd = "AgeGroup = 'CM' ";
                    } else if ( curGroupValue.ToUpper().Equals( "WOMEN A" ) ) {
                        myFilterCmd = "AgeGroup = 'CW' ";
                    } else if ( curGroupValue.ToUpper().Equals( "MEN B" ) ) {
                        myFilterCmd = "AgeGroup = 'BM' ";
                    } else if ( curGroupValue.ToUpper().Equals( "WOMEN B" ) ) {
                        myFilterCmd = "AgeGroup = 'BW' ";
                    } else if (curGroupValue.ToUpper().Equals( "ALL" )) {
                        myFilterCmd = "";
                    } else {
                        myFilterCmd = "AgeGroup not in ('CM', 'CW', 'BM', 'BW') " ;
                    }
                } else {
                    if ( curGroupValue.ToLower().Equals( "all" ) ) {
                        myFilterCmd = "";
                    } else {
                        myFilterCmd = "EventGroup = '" + curGroupValue + "' ";
                    }
                }
            } catch {
                curGroupValue = "All";
            }

            // Retrieve data from database
            roundSelect.RoundValue = roundActiveSelect.RoundValue;
            getEventRegData( curGroupValue, Convert.ToByte( roundSelect.RoundValue ) );
            loadTourEventRegView();
        }

        private void navScoreboard_Click(object sender, EventArgs e) {
            String curPath = ExportLiveScoreboard.getCheckScoreboardLocation();
        }

        private void navLiveWeb_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            ExportLiveWeb.LiveWebDialog.WebLocation = ExportLiveWeb.LiveWebLocation;
            ExportLiveWeb.LiveWebDialog.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if (ExportLiveWeb.LiveWebDialog.DialogResult == DialogResult.OK) {
                if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Set" )) {
                    ExportLiveWeb.LiveWebLocation = ExportLiveWeb.LiveWebDialog.WebLocation;
                    ExportLiveWeb.exportTourData( mySanctionNum );
                    LiveWebLabel.Visible = true;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "TwitterActive" )) {
                    ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterDefaultAccount;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "TwitterAuth" )) {
                    ExportLiveTwitter.TwitterLocation = ExportLiveTwitter.TwitterRequestTokenURL;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Disable" )) {
                    ExportLiveWeb.LiveWebLocation = "";
                    ExportLiveTwitter.TwitterLocation = "";
                    LiveWebLabel.Visible = false;
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "Resend" )) {
                    if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                        try {
                            String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                            String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                            String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkierSlalom( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
                        } catch {
                        }
                    }
                    if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                        if (slalomRecapDataGridView.Rows.Count > 0) {
                            StringBuilder curTwitterMessage = new StringBuilder( "" );
                            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                            String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                            curTwitterMessage.Append( mySanctionNum + " " + curAgeGroup + " " + activeSkierName.Text );
                            if (curTeamCode.Length > 0) {
                                curTwitterMessage.Append( " Team: " + curTeamCode );
                            }
                            curTwitterMessage.Append( " Score: " + scoreTextBox.Text );
                            curTwitterMessage.Append( " Pass: " + slalomRecapDataGridView.Rows[slalomRecapDataGridView.Rows.Count - 1].Cells["ScoreRecap"].Value + " buoys" );
                            curTwitterMessage.Append( " " + ActivePassDesc.Text );
                            curTwitterMessage.Append( " REPOST UNOFFICIAL" );
                            ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                        }
                    }
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "ResendAll" )) {
                    if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                        try {
                            String curEventGroup = EventGroupList.SelectedItem.ToString();
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkiers( "Slalom", mySanctionNum, curRound, curEventGroup );
                        } catch {
                            MessageBox.Show( "Exception encounter sending message to twitter" );
                        }
                    }
                }
            }
        }

        private void loadEventGroupList(int inRound) {
            isLoadInProg = true;

            if (EventGroupList.DataSource == null) {
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    ArrayList curEventGroupList = new ArrayList();
                    curEventGroupList.Add( "All" );
                    curEventGroupList.Add( "Men A" );
                    curEventGroupList.Add( "Women A" );
                    curEventGroupList.Add( "Men B" );
                    curEventGroupList.Add( "Women B" );
                    curEventGroupList.Add( "Non Team" );
                    EventGroupList.DataSource = curEventGroupList;
                } else {
                    loadEventGroupListFromData( inRound );
                }
            } else {
                if (((ArrayList)EventGroupList.DataSource).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadEventGroupListFromData( inRound );
                    }
                } else {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        ArrayList curEventGroupList = new ArrayList();
                        curEventGroupList.Add( "All" );
                        curEventGroupList.Add( "Men A" );
                        curEventGroupList.Add( "Women A" );
                        curEventGroupList.Add( "Men B" );
                        curEventGroupList.Add( "Women B" );
                        curEventGroupList.Add( "Non Team" );
                        EventGroupList.DataSource = curEventGroupList;
                    } else {
                        loadEventGroupListFromData( inRound );
                    }
                }
            }
            isLoadInProg = false;
        }

        private void loadEventGroupListFromData(int inRound) {
            String curGroupValue = "";
            if (EventGroupList.DataSource != null) {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    try {
                        curGroupValue = EventGroupList.SelectedItem.ToString();
                    } catch {
                        curGroupValue = "";
                    }
                }
            }

            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT EventGroup From EventRunOrder "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' And Round = " + inRound.ToString() + " "
                + "Order by EventGroup";
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                    + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom'"
                    + "Order by EventGroup";
                curDataTable = DataAccess.getDataTable( curSqlStmt );
            }

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["EventGroup"] );
            }
            EventGroupList.DataSource = curEventGroupList;
            if (curGroupValue.Length > 0) {
                foreach (String curValue in (ArrayList)EventGroupList.DataSource) {
                    if ( curValue.Equals(curGroupValue)) {
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

        private void slalomRecapDataGridView_KeyUp( object sender, KeyEventArgs e ) {
            try {
                if (slalomRecapDataGridView != null) {
                    if (e.KeyCode == Keys.Enter) {
                        if (isRecapRowEnterHandled || isAddRecapRowInProg) {
                            isAddRecapRowInProg = false;
                            isRecapRowEnterHandled = false;
                        } else {
                            myRecapColumn = slalomRecapDataGridView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
                            Timer curTimerObj = new Timer();
                            curTimerObj.Interval = 15;
                            curTimerObj.Tick += new EventHandler( setRecapNextCell );
                            curTimerObj.Start();
                        }
                        e.Handled = true;

					} else if (e.KeyCode == Keys.Tab) {
                        isAddRecapRowInProg = false;
                        String curColName = slalomRecapDataGridView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
                        if (curColName.Equals( "ScoreRecap" )) {
                            slalomRecapDataGridView.CurrentCell = myRecapRow.Cells[myStartCellIndex];
                        }
                        e.Handled = true;

					} else if (e.KeyCode == Keys.Escape) {
                        isAddRecapRowInProg = false;
                        e.Handled = true;

					} else if (e.KeyCode == Keys.Delete) {
                        isAddRecapRowInProg = false;
                        isDataModified = true;
                        myRecapRow.Cells["Updated"].Value = "Y";
                        String curColName = slalomRecapDataGridView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
                        if (curColName.StartsWith( "Judge" )) {
                            myRecapRow.Cells[curColName].Value = "";
                            myRecapRow.Cells["BoatTimeRecap"].Value = "";
                            skierPassMsg.Text = "";
                            e.Handled = true;
                        } else if (curColName.Equals( "BoatTimeRecap" )) {
                            myRecapRow.Cells[curColName].Value = "";
                            skierPassMsg.Text = "";
                            e.Handled = true;
                        }

                    } else {
                        isAddRecapRowInProg = false;
                    }
                }
            } catch (Exception exp) {
                MessageBox.Show( "Exception in slalomRecapDataGridView_KeyUp \n" + exp.Message );
            }
        }

        private void setRecapNextCell( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( setRecapNextCell );
            
            if ( myRecapColumn.Equals( "Judge1ScoreRecap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];
            } else if ( myRecapColumn.Equals( "Judge2ScoreRecap" ) ) {
                if ( myNumJudges > 2 ) {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];
                } else {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
                }
            } else if ( myRecapColumn.Equals( "Judge3ScoreRecap" ) ) {
                if ( myNumJudges > 3 ) {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"];
                } else {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
                }
            } else if ( myRecapColumn.Equals( "Judge4ScoreRecap" ) ) {
                if ( myNumJudges > 4 ) {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge5ScoreRecap"];
                } else {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
                }
            } else if ( myRecapColumn.Equals( "Judge5ScoreRecap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
            } else if ( myRecapColumn.Equals( "BoatTimeRecap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells[myStartCellIndex];
            } else if ( myRecapColumn.Equals( "GateEntry2Recap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge2ScoreRecap"];
            } else if ( myRecapColumn.Equals( "GateEntry3Recap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];
            } else if ( myRecapColumn.Equals( "GateEntry1Recap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
            } else if ( myRecapColumn.Equals( "GateExit2Recap" ) ) {
                if ( myNumJudges > 2 ) {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge3ScoreRecap"];
                } else {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
                }
            } else if ( myRecapColumn.Equals( "GateExit3Recap" ) ) {
                if ( myNumJudges > 3 ) {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge4ScoreRecap"];
                } else {
                    slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["Judge1ScoreRecap"];
                }
            } else if ( myRecapColumn.Equals( "GateExit1Recap" ) ) {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];
            } else {
                slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[myRecapRow.Index].Cells["BoatTimeRecap"];
            }
        }

        private void TourEventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView myDataView = (DataGridView)sender;

            //Update data if changes are detected
            if ( isDataModified && ( myEventRegViewIdx != e.RowIndex ) ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if (isDataModified) {
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 5;
                curTimerObj.Tick += new EventHandler( TourEventRegDataGridView_RowEnterReset );
                curTimerObj.Start();

			} else {
				myRecapRow = null;
				myScoreRow = null;

				if (e.RowIndex >= 0) {
                    if (myEventRegViewIdx != e.RowIndex) {
                        skierPassMsg.Text = "";
                        ActivePassDesc.Text = "";
                        myEventRegViewIdx = e.RowIndex;
                        int curRowPos = e.RowIndex + 1;
                        RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
                        if (!( isObjectEmpty( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) )) {
                            setSlalomScoreEntry( myDataView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                            setSlalomRecapEntry( myDataView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                            setNewRowPos();
                        }
                    }
                }
            }
        }

        private void TourEventRegDataGridView_RowEnterReset( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( TourEventRegDataGridView_RowEnterReset );

            TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
        }

        private void roundSelect_Load( object sender, EventArgs e ) {
            if ( myScoreDataTable != null ) {
                if ( myScoreDataTable.Rows.Count > 0 ) {
                    roundSelect.RoundValue = (String)myScoreRow["Round"];
                }
            }
        }

        private void roundActiveSelect_Load(object sender, EventArgs e) {
        }

        private void roundSelect_Click(object sender, EventArgs e) {
            if ( sender != null ) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    roundSelect.RoundValue = curValue;
                } catch ( Exception ex ) {
                    curValue = ex.Message;
                }
            }
            if ( TourEventRegDataGridView.CurrentRow != null ) {
                if (TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value != System.DBNull.Value) {
                    setSlalomScoreEntry(TourEventRegDataGridView.CurrentRow, Convert.ToInt16(roundSelect.RoundValue));
                    setSlalomRecapEntry(TourEventRegDataGridView.CurrentRow, Convert.ToInt16(roundSelect.RoundValue));
                    setNewRowPos();
                }
            }
        }

        private void roundActiveSelect_Click(object sender, EventArgs e) {
            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            if ( !( isDataModified ) ) {
                if ( sender != null ) {
                    String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                    try {
                        roundActiveSelect.RoundValue = curValue;
                    } catch (Exception ex) {
                        curValue = ex.Message;
                    }
                }

                //Retrieve and load tournament event entries
                loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

                int curSaveEventRegViewIdx = myEventRegViewIdx;
                navRefresh_Click( null, null );

                if ( curSaveEventRegViewIdx > 0 ) {
                    myEventRegViewIdx = curSaveEventRegViewIdx;
                    if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                        myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                    }
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                    roundSelect.RoundValue = roundActiveSelect.RoundValue;
                    setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    setNewRowPos();
                }
            }
        }

        private void EventGroupList_SelectedIndexChanged( object sender, EventArgs e ) {
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            if (!( isDataModified )) {
                if ( !( isLoadInProg ) ) {
                    int curSaveEventRegViewIdx = myEventRegViewIdx;
                    navRefresh_Click( null, null );
                    if ( curSaveEventRegViewIdx > 0 ) {
                        myEventRegViewIdx = curSaveEventRegViewIdx;
                        if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                            myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                        }
                        TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                        setSlalomScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setSlalomRecapEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setNewRowPos();
                    }
                    Int16 curRound = 0;
                    try {
                        curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
                    } catch {
                        curRound = 0;
                    }
                    if (curRound >= 25) {
                        MessageBox.Show( "You currently have the runoff round selected.\nChange the round if that is not desired." );
                    }
                }
            }
        }

        private void SlalomSpeedSelect_Load( object sender, EventArgs e ) {
            if ( myScoreRow != null ) {
                SlalomSpeedSelect.CurrentValue = (Byte)myScoreRow["StartSpeed"];
            }
        }

        private void SlalomLineSelect_Load(object sender, EventArgs e) {
            if (myScoreRow != null) {
                SlalomLineSelect.CurrentValue = (String)myScoreRow["StartLen"];
            }
        }

        private void SlalomSpeedSelect_Change(object sender, EventArgs e) {
            if ( sender != null ) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    SlalomSpeedSelect.CurrentValue = Convert.ToInt16( curValue );
                    addPassButton.Focus();
                } catch {
                    curValue = e.ToString();
                }
            }
        }

        private void SlalomLineSelect_Change(object sender, EventArgs e) {
            if ( sender != null ) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    SlalomLineSelect.CurrentValue = curValue;
                    addPassButton.Focus();
                } catch {
                    curValue = e.ToString();
                }
            }
        }

        private void scoreEventClass_SelectedIndexChanged(object sender, EventArgs e) {
            addPassButton.Focus();
        }

        private void scoreEventClass_Validating(object sender, CancelEventArgs e) {
            String curMethodName = "Slalom:ScoreEntry:scoreEventClass_Validating";
            int rowsProc = 0;

            ListItem curItem = (ListItem)scoreEventClass.SelectedItem;
            String curEventClass = curItem.ItemValue;
            if ( mySkierClassList.compareClassChange( curEventClass, myTourClass ) < 0 ) {
                MessageBox.Show( "Class " + curEventClass + " cannot be assigned to a skier in a class " + myTourClass + " tournament" );
                e.Cancel = true;
            } else {
                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value = curEventClass;
                try {
                    Int64 curScorePK = 0;
                    if ( myScoreRow == null ) {
                        curScorePK = -1;
                    } else {
                        curScorePK = (Int64)myScoreRow["PK"];
                    }
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    if ( curScorePK > 0 ) {
                        curSqlStmt.Append( "Update SlalomScore Set " );
                        curSqlStmt.Append( "EventClass = '" + curEventClass + "'" );
                        curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                        curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }
                } catch ( Exception excp ) {
                    String curMsg = ":Error attempting to update skier class \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
                isDataModified = true;
            }
        }

        private void slalomRecapDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e) {

		}

		private void scoreEntryInprogress() {
            SlalomSpeedSelect.Enabled = false;
            SlalomLineSelect.Enabled = false;
            addPassButton.Enabled = false;
            SlalomSpeedSelect.resetMaxSpeed();
        }

        private void scoreEntryBegin() {
            SlalomSpeedSelect.Enabled = true;
            SlalomLineSelect.Enabled = true;
            roundSelect.Enabled = true;
            addPassButton.Enabled = true;
        }

        private void slalomRecapDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex >= 0) {
                String curColName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
                if (!( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly )) {
                    if (curColName.Equals( "RerideRecap" )) {
                        if (slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals( "N" )) {
                            int rowIndex = slalomRecapDataGridView.Rows.Count - 1;
                            slalomRecapDataGridView.Select();
                            slalomRecapDataGridView.CurrentCell = slalomRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
                        }

					} else if (curColName.StartsWith( "Gate" )) {
                        myGateValueChg = true;

					} else if ( curColName.StartsWith( "BoatTimeRecap" ) ) {
						myOrigCellValue = (String)slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
				}
            }
        }

        private void slalomRecapDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            if ( myWindowFocus ) {
				if ( myRecapRow == null ) return;
				if ( e.RowIndex != myRecapRow.Index ) return;
				if ( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

				myRecapRow.ErrorText = "";
				myModCellValue = "";

				String curColName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
				if ( curColName.StartsWith( "Judge" ) ) {
					#region Validate any of the judge score cells
					String curJudgeEntry = (String) e.FormattedValue;
					if ( isObjectEmpty( curJudgeEntry ) ) {
						myModCellValue = "";
					} else {
						if ( curJudgeEntry != myOrigCellValue ) {
							if ( curJudgeEntry.IndexOf( '.' ) < 0 ) {
								String modJudgeEntry = "";
								String curLastChar = curJudgeEntry.Substring( curJudgeEntry.Length - 1, 1 ).ToUpper();
								if ( curLastChar.Equals( "Q" ) ) {
									if ( curJudgeEntry.Length > 1 ) {
										modJudgeEntry = curJudgeEntry.Substring( 0, curJudgeEntry.Length - 1 ) + ".25";
									} else {
										modJudgeEntry = ".25";
									}
								} else if ( curLastChar.Equals( "H" ) ) {
									if ( curJudgeEntry.Length > 1 ) {
										modJudgeEntry = curJudgeEntry.Substring( 0, curJudgeEntry.Length - 1 ) + ".5";
									} else {
										modJudgeEntry = ".5";
									}
								} else {
									modJudgeEntry = curJudgeEntry.Substring( 0, 1 ) + "." + curJudgeEntry.Substring( 1 );
								}
								curJudgeEntry = modJudgeEntry;
								myModCellValue = curJudgeEntry;
							}
							try {
								Decimal curJudgeScore = Convert.ToDecimal( curJudgeEntry );
								if ( curJudgeScore > 6 ) {
									myRecapRow.ErrorText = " Value in " + curColName + " must be less than or equal to 6";
									MessageBox.Show( myRecapRow.ErrorText );
									e.Cancel = true;
								} else {
									Int32 posDelim = curJudgeEntry.IndexOf( '.' );
									if ( posDelim > -1 ) {
										if ( curJudgeEntry.Substring( posDelim ).Equals( ".25" )
											|| curJudgeEntry.Substring( posDelim ).Equals( ".5" )
											|| curJudgeEntry.Substring( posDelim ).Equals( ".50" )
											|| curJudgeEntry.Substring( posDelim ).Equals( ".0" )
											|| curJudgeEntry.Substring( posDelim ).Equals( ".00" )
											|| curJudgeEntry.Substring( posDelim ).Equals( "." )
											) {
											//Value is valid
											myModCellValue = curJudgeScore.ToString( "0.00" );
										} else if ( curJudgeEntry.Substring( posDelim ).Equals( ".75" ) ) {
											if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
												if ( myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "CM" )
													|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "CW" )
													|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "BM" )
													|| myRecapRow.Cells["AgeGroupRecap"].Value.Equals( "BW" )
													) {
													//Value is valid
													myModCellValue = curJudgeScore.ToString( "0.00" );
												} else {
													myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
													MessageBox.Show( myRecapRow.ErrorText );
													e.Cancel = true;
												}
											} else {
												myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
												MessageBox.Show( myRecapRow.ErrorText );
												e.Cancel = true;
											}
										} else {
											myRecapRow.ErrorText = " Decimal value in " + curColName + " must only contain .0 or .25 or .5 or .75";
											MessageBox.Show( myRecapRow.ErrorText );
											e.Cancel = true;
										}
									}
								}
							} catch ( Exception exp ) {
								myRecapRow.ErrorText = "Value in " + curColName + " must be numeric (if Q or H must be last characer";
								MessageBox.Show( myRecapRow.ErrorText );
								e.Cancel = true;
							}
						}
					}
					#endregion

				} else if ( curColName.StartsWith( "BoatTime" ) && myRecapRow.Cells[e.ColumnIndex].Value != null ) {
					#region Validate boat time
					String curValue = (String) e.FormattedValue;
					if ( isObjectEmpty( curValue ) ) {
						myModCellValue = "";
					} else {
						if ( curValue != myOrigCellValue ) {
							if ( curValue.ToUpper().Equals( "OK" ) ) {
								myModCellValue = "00.00";
							} else if ( curValue.ToUpper().Equals( "NONE" ) ) {
								myModCellValue = "-1.00";
							} else {
								try {
									Decimal curNum = Convert.ToDecimal( curValue );
								} catch ( Exception exp ) {
									myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
									MessageBox.Show( myRecapRow.ErrorText );
									e.Cancel = true;
								}
							}
						}
					}
					#endregion
				}
			}
		}

        private void slalomRecapDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            if ( myWindowFocus ) {
				if ( myRecapRow == null ) return;
				if ( e.RowIndex != myRecapRow.Index ) return;
				if ( slalomRecapDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly ) return;

				String curColName = slalomRecapDataGridView.Columns[e.ColumnIndex].Name;
				if ( curColName.StartsWith( "Judge" ) ) {
					#region Validate any of the judge score cells
					String curInputValue = (String) myRecapRow.Cells[e.ColumnIndex].Value;
					if ( isObjectEmpty( curInputValue ) ) {
						if ( isObjectEmpty( myOrigCellValue ) ) {
							//No new data input, no action required
						} else {
							isDataModified = true;
							myRecapRow.Cells["Updated"].Value = "Y";
						}
					} else {
						if ( curInputValue != myOrigCellValue ) {
							isDataModified = true;
							myRecapRow.Cells["Updated"].Value = "Y";
							try {
								if ( myModCellValue.Length > 0 ) curInputValue = myModCellValue;
								Decimal curJudgeScore = Convert.ToDecimal( curInputValue );
								myRecapRow.Cells[e.ColumnIndex].Value = curJudgeScore.ToString( "#.00" );
								if ( !( isObjectEmpty( myRecapRow.Cells["BoatTimeRecap"].Value.ToString() ) ) ) {
									SlalomTimeValidate();
								}
								myOrigCellValue = (String) myRecapRow.Cells[e.ColumnIndex].Value;
							} catch ( Exception ex ) {
								myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
								MessageBox.Show( "Exception: " + ex.Message );
								return;
							}
							if ( ( !( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
								&& ( !( isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
								if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
									if ( (String) myRecapRow.Cells["TimeInTolRecap"].Value == "Y" ) {
										if ( isExitGatesGood() ) {
											isAddRecapRowInProg = true;
											Timer curTimerObj = new Timer();
											curTimerObj.Interval = 5;
											curTimerObj.Tick += new EventHandler( addRecapRowTimer );
											curTimerObj.Start();
										} else {
											skierPassMsg.ForeColor = Color.Red;
											skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
										}
									}
								}
							}
						}
					}
					#endregion
				} else if ( curColName.Equals( "BoatTimeRecap" ) ) {
					#region Validate boat time
					String curInputValue = (String) myRecapRow.Cells[e.ColumnIndex].Value;
					if ( isObjectEmpty( curInputValue ) ) {
						if ( isObjectEmpty( myOrigCellValue ) ) {
							//No new data input, no action required
						} else {
							isRecapRowEnterHandled = true;
							isDataModified = true;
							myRecapRow.Cells["Updated"].Value = "Y";
							myRecapRow.Cells["TimeInTolRecap"].Value = "N";
							myRecapRow.Cells["ScoreProtRecap"].Value = "N";
							myRecapRow.Cells["RerideRecap"].Value = "N";
							myRecapRow.Cells["ScoreRecap"].Value = "";
						}
					} else {
						if ( myModCellValue.Length > 0 ) curInputValue = myModCellValue;
						if ( curInputValue == myOrigCellValue ) {
							/*
							if ( ( !( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
								&& ( !( isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
								if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
									if ( myRecapRow.Cells["BoatTimeRecap"].ReadOnly ) {
									} else {
										if ( (String) myRecapRow.Cells["TimeInTolRecap"].Value == "Y" ) {
											if ( isExitGatesGood() ) {
												isRecapRowEnterHandled = true;
												isAddRecapRowInProg = true;
												Timer curTimerObj = new Timer();
												curTimerObj.Interval = 5;
												curTimerObj.Tick += new EventHandler( addRecapRowTimer );
												curTimerObj.Start();
											} else {
												skierPassMsg.ForeColor = Color.Red;
												skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
											}
										}
									}
								}
							}
							*/

						} else {
							isRecapRowEnterHandled = true;
							isDataModified = true;
							myRecapRow.Cells["Updated"].Value = "Y";
							SlalomTimeValidate();
							myOrigCellValue = (String) myRecapRow.Cells[e.ColumnIndex].Value;
							if ( ( !( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
								&& ( !( isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
								if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
									if ( myRecapRow.Cells["BoatTimeRecap"].ReadOnly ) {
									} else if ( (String) myRecapRow.Cells["TimeInTolRecap"].Value == "Y" ) {
										if ( isExitGatesGood() ) {
											isAddRecapRowInProg = true;
											Timer curTimerObj = new Timer();
											curTimerObj.Interval = 5;
											curTimerObj.Tick += new EventHandler( addRecapRowTimer );
											curTimerObj.Start();
										} else {
											skierPassMsg.ForeColor = Color.Red;
											skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
										}
									} else if ( (String) myRecapRow.Cells["ScoreProtRecap"].Value == "Y" ) {
										if ( isExitGatesGood() ) {
											isAddRecapRowInProg = true;
											Timer curTimerObj = new Timer();
											curTimerObj.Interval = 5;
											curTimerObj.Tick += new EventHandler( addRecapRowTimer );
											curTimerObj.Start();
										}
									} else if ( (String) myRecapRow.Cells["RerideRecap"].Value == "Y" ) {
										if ( isExitGatesGood() ) {
											isAddRecapRowInProg = true;
											Timer curTimerObj = new Timer();
											curTimerObj.Interval = 5;
											curTimerObj.Tick += new EventHandler( addRecapRowTimer );
											curTimerObj.Start();
										}
									}
								} else {
									if ( (String) myRecapRow.Cells["RerideRecap"].Value == "Y" ) {
										isAddRecapRowInProg = true;
										Timer curTimerObj = new Timer();
										curTimerObj.Interval = 5;
										curTimerObj.Tick += new EventHandler( addRecapRowTimer );
										curTimerObj.Start();
									}
								}
							}
						}
					}
					#endregion
				} else if ( curColName.StartsWith( "Gate" ) ) {
					#region Validate entrance and exit gate cells
					try {
						if ( myGateValueChg ) {
							Boolean curGateValue = (Boolean) myRecapRow.Cells[curColName].Value;
							if ( !( isObjectEmpty( myRecapRow.Cells["BoatTimeRecap"].Value ) ) ) {
								SlalomTimeValidate();
								if ( ( !( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) )
									&& ( !( isObjectEmpty( myRecapRow.Cells["TimeInTolRecap"].Value ) ) ) ) {
									if ( Convert.ToDecimal( myRecapRow.Cells["ScoreRecap"].Value ) == 6 ) {
										if ( (String) myRecapRow.Cells["TimeInTolRecap"].Value == "Y" ) {
											if ( isExitGatesGood() ) {
												if ( curGateValue ) {
													isAddRecapRowInProg = true;
													Timer curTimerObj = new Timer();
													curTimerObj.Interval = 5;
													curTimerObj.Tick += new EventHandler( addRecapRowTimer );
													curTimerObj.Start();
												}
											} else {
												skierPassMsg.ForeColor = Color.Red;
												skierPassMsg.Text = "Time good, score 6, no continue missed exit gates";
												MessageBox.Show( skierPassMsg.Text );
											}
										}
									}
								}
							}
						}
					} catch {
						//This situation only seems to be possible when the window is closing 
						//so just ending method to allow window to close.
						return;
					}
					#endregion
				} else if ( curColName.Equals( "RerideRecap" ) ) {
					#region Validate reride cell check box
					if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
						if ( isObjectEmpty( myRecapRow.Cells["ScoreRecap"].Value ) ) {
							myRecapRow.Cells["ScoreRecap"].Value = "0";
						} else {
						}
						rerideReasonDialogForm.RerideReasonText = (String) myRecapRow.Cells["RerideReasonRecap"].Value;
						if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
							setEventRegRowStatus( "InProg" );
							String curCommand = rerideReasonDialogForm.Command;
							myRecapRow.Cells["Updated"].Value = "Y";
							myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
							if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) {
								myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
							}
							saveScore();
							Timer curTimerObj = new Timer();
							curTimerObj.Interval = 5;
							curTimerObj.Tick += new EventHandler( addRecapRowTimer );
							curTimerObj.Start();
						} else {
							myRecapRow.Cells["Updated"].Value = "Y";
							rerideReasonDialogForm.RerideReasonText = "";
							MessageBox.Show( "Reride can not be granted without a reason being specified." );
							myRecapRow.Cells["RerideRecap"].Value = "N";
							myRecapRow.Cells["Updated"].Value = "Y";
							saveScore();
						}
					} else {
						myRecapRow.Cells["Updated"].Value = "Y";
						setEventRegRowStatus( myRecapRow );
						saveScore();
					}
					#endregion
				}
			}
		}

        private void SlalomTimeValidate() {
            bool curReadyToScore = false;
            Decimal [] curJudgeScore = new Decimal[myNumJudges];
            int [] curGateEntry = new int[myNumJudges];
            int [] curGateExit = new int[myNumJudges];
            int curGateEntryValue = 0;
            Decimal curScore = 0;
            DataRow curClassRow = getClassRowCurrentSkier();

            if ( !(isObjectEmpty( myRecapRow.Cells["BoatTimeRecap"].Value))) {
                #region Determine if sufficient information is available to calculate a score
                if ( myNumJudges == 1 ) {
                    if ( !(isObjectEmpty( myRecapRow.Cells["Judge1ScoreRecap"].Value))) {
                        curReadyToScore = true;
                        try {
                            curJudgeScore[0] = Convert.ToDecimal((String)myRecapRow.Cells["Judge1ScoreRecap"].Value);
                            if ( curJudgeScore[0] < 6 ) myRecapRow.Cells["GateExit1Recap"].Value = false;
                            if ( isEntryGatesGood() == false ) {
                                curJudgeScore[0] = 0;
                            }
                            curScore = SlalomRecapScoreCalc(curJudgeScore);
                        } catch {
                            curScore = 0;
                        }
                    }
                } else if ( myNumJudges == 3 ) {
                    if ( ( !( isObjectEmpty( myRecapRow.Cells["Judge1ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge2ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge3ScoreRecap"].Value ) ) )
                       ) {
                        curReadyToScore = true;
                        try {
                            curJudgeScore[0] = Convert.ToDecimal( (String)myRecapRow.Cells["Judge1ScoreRecap"].Value );
                            curJudgeScore[1] = Convert.ToDecimal((String)myRecapRow.Cells["Judge2ScoreRecap"].Value);
                            curJudgeScore[2] = Convert.ToDecimal( (String)myRecapRow.Cells["Judge3ScoreRecap"].Value );

                            if ( isEntryGatesGood() ) {
                                curScore = SlalomRecapScoreCalc( curJudgeScore );
                            } else {
                                if ((Decimal)curClassRow["ListCodeNum"] < (Decimal)myClassERow["ListCodeNum"] && myRecapRow.Index == 0 ) {
                                    curScore = SlalomRecapScoreCalc( curJudgeScore );
                                    if (curScore < 6) {
                                        curScore = 0;
                                    }
                                } else {
                                    curScore = 0;
                                }
                            }
                        } catch ( Exception ex ) {
                            MessageBox.Show( "Processing error detected in SlalomTimeValidate \n" + ex.Message );
                            curScore = 0;
                        }
                    }
                } else if (myNumJudges == 5) {
                    if ( ( !( isObjectEmpty( myRecapRow.Cells["Judge1ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge2ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge3ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge4ScoreRecap"].Value ) ) )
                        && ( !( isObjectEmpty( myRecapRow.Cells["Judge5ScoreRecap"].Value ) ) )
                       )  {
                        curReadyToScore = true;
                        try {
                            curJudgeScore[0] = Convert.ToDecimal((String)myRecapRow.Cells["Judge1ScoreRecap"].Value);
                            curJudgeScore[1] = Convert.ToDecimal((String)myRecapRow.Cells["Judge2ScoreRecap"].Value);
                            curJudgeScore[2] = Convert.ToDecimal((String)myRecapRow.Cells["Judge3ScoreRecap"].Value);
                            curJudgeScore[3] = Convert.ToDecimal((String)myRecapRow.Cells["Judge4ScoreRecap"].Value);
                            curJudgeScore[4] = Convert.ToDecimal((String)myRecapRow.Cells["Judge5ScoreRecap"].Value);

                            if ( isEntryGatesGood() ) {
                                curScore = SlalomRecapScoreCalc( curJudgeScore );
                            } else {
                                curScore = 0;
                            }
                        } catch {
                            curScore = 0;
                        }
                    }
                }
                #endregion

                if ( curReadyToScore ) {
                    #region Analyze the boat time and fill when determined that short hand entry has been used
                    try {
                        Int16 curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                        if ( curSpeed < SlalomSpeedSelect.CurrentValue ) {
                            curSpeed = SlalomSpeedSelect.CurrentValue;
                        }
                        setBoatTimeRow( curSpeed, Convert.ToInt16( curScore.ToString().Substring( 0, 1 ) ) );
                        loadBoatTimeView( curSpeed );
                        Decimal curMinTime = (Decimal)myTimeRow["MinValue"];
                        Decimal curMaxTime = (Decimal)myTimeRow["MaxValue"];
                        Decimal curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                        Decimal tempTime, tempMinDiff, tempMaxDiff;

                        String curBoatTimeOrigValue = myRecapRow.Cells["BoatTimeRecap"].Value.ToString();
                        if ( curBoatTimeOrigValue.ToUpper().Equals("OK") ) {
                            myRecapRow.Cells["BoatTimeRecap"].Value = curActualTime.ToString("#0.00");
                            curBoatTimeOrigValue = curActualTime.ToString("#0.00");
                        } else if ( curBoatTimeOrigValue.ToUpper().Equals("NONE") ) {
                            myRecapRow.Cells["BoatTimeRecap"].Value = ( curActualTime * -1 ).ToString("##0.00");
                            curBoatTimeOrigValue = (curActualTime * -1).ToString("##0.00");
                        } else if ( curBoatTimeOrigValue.Length == 1 ) {
                            curBoatTimeOrigValue = "0" + curBoatTimeOrigValue;
                        }
                        if ( curBoatTimeOrigValue.Length == 2 ) {
                            if ( !( curBoatTimeOrigValue.Contains( "." ) ) ) {
                                Int32 delimPos = 0;
                                String newValue = "";

                                delimPos = curActualTime.ToString().IndexOf( '.' );
                                newValue = curActualTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
                                tempTime = Convert.ToDecimal( newValue );
                                if ( ( tempTime < curMinTime ) || ( tempTime > curMaxTime ) ) {
                                    delimPos = curMinTime.ToString().IndexOf( '.' );
                                    newValue = curMinTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
                                    tempTime = Convert.ToDecimal( newValue );
                                    if ( tempTime < curMinTime ) {
                                        tempMinDiff = curMinTime - tempTime;
                                        newValue = curMaxTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
                                        tempTime = Convert.ToDecimal( newValue );
                                        if ( tempTime > curMaxTime ) {
                                            tempMaxDiff = tempTime - curMaxTime;
                                            if ( tempMaxDiff > tempMinDiff ) {
                                                newValue = curMinTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
                                            } else {
                                                newValue = curMaxTime.ToString().Substring( 0, delimPos + 1 ) + curBoatTimeOrigValue;
                                            }
                                        }
                                    }
                                }
                                myRecapRow.Cells["BoatTimeRecap"].Value = newValue;
                            }
                        }
                        Decimal curBoatTime = Convert.ToDecimal( myRecapRow.Cells["BoatTimeRecap"].Value.ToString() );
                        if ( curBoatTime < 0 ) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            skierPassMsg.ForeColor = Color.Red;
                            skierPassMsg.Text = "No time was available";
                            myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
                            SlalomScoreCalc(curScore);
                        } else if ( curBoatTime > curMaxTime ) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            if ( curScore == 6.0M ) {
                                myRecapRow.Cells["RerideRecap"].Value = "Y";
                            }
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            skierPassMsg.ForeColor = Color.Red;
                            skierPassMsg.Text = "SLOW Boat time "
                                + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                            myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
                            SlalomScoreCalc( curScore );
                        } else if ( curBoatTime < curMinTime ) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            skierPassMsg.ForeColor = Color.Red;
                            skierPassMsg.Text = "FAST Boat time "
                                + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                            myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
                            SlalomScoreCalc( curScore );
                        } else {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "Y";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
                            skierPassMsg.ForeColor = Color.DarkGray;
                            skierPassMsg.Text = "Boat times - current score "
                                + curMinTime.ToString() + " " + curActualTime.ToString() + " " + curMaxTime.ToString();
                            if ( !( myRecapRow.Cells["RerideRecap"].Value.Equals( "Y" ) ) ) {
                                myRecapRow.Cells["RerideReasonRecap"].Value = "";
                            }
                            SlalomScoreCalc( curScore );
                        }
                    } catch ( Exception exp ) {
                        MessageBox.Show( "Processing error detected in SlalomTimeValidate \n" + exp.Message );
                    }
                }
                #endregion
            }
        }

        private Decimal SlalomRecapScoreCalc(Decimal [] inJudgeScores) {
            Decimal returnScore = -1;

            if (myNumJudges == 1) {
                returnScore = inJudgeScores[0];
            } else if (myNumJudges == 3) {
                Int16[] usedIndex = { 9, 9, 9 };
                Decimal[] myJudgeScore = new Decimal[myNumJudges];
                for (Int16 idxSort = 0; idxSort < inJudgeScores.Length; idxSort++) {
                    for (Int16 idx = 0; idx < inJudgeScores.Length; idx++) {
                        if (!(usedIndex.Contains(idx))) {
                            if (inJudgeScores[idx] > myJudgeScore[idxSort]) {
                                myJudgeScore[idxSort] = inJudgeScores[idx];
                                usedIndex[idxSort] = idx;
                            }
                        }
                    }
                }
                returnScore = myJudgeScore[1];
            } else if (myNumJudges == 5) {
                Int16[] usedIndex = { 9, 9, 9, 9, 9 };
                Decimal[] myJudgeScore = new Decimal[myNumJudges];
                for (Int16 idxSort = 0; idxSort < inJudgeScores.Length; idxSort++) {
                    for (Int16 idx = 0; idx < inJudgeScores.Length; idx++) {
                        if (!(usedIndex.Contains(idx))) {
                            if (inJudgeScores[idx] > myJudgeScore[idxSort]) {
                                myJudgeScore[idxSort] = inJudgeScores[idx];
                                usedIndex[idxSort] = idx;
                            }
                        }
                    }
                }
                returnScore = myJudgeScore[2];
            }
            return returnScore;
        }

        private void SlalomScoreCalc(Decimal inScore) {
            String curMethodName = "SlalomScoreCalc";
            Decimal skierScore = 0, curPassScore, prevPassScore;
            Int16 prevPassNum = 0, curPassNumMinSpeed = 0, curPassNumLineAdjust = 0;
            Boolean curTimeGood, curScoreProtGood;
            String prevRerideInd, prevProtectInd, prevTimeGood;
            DataRow curClassRow;
            try {
                try {
                    String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
                    if ( !( curMemberId.Equals( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
                        MessageBox.Show( curMethodName + ": Unable to continue processing"
                            + "\n Current id of skier being scored       = " + curMemberId
                            + "\n Current id skier in running order list = " + (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value );
                        return;
                    }
                } catch {
                    //No actions needed
                }

                String curAgeGroup = (String)TourEventRegDataGridView.CurrentRow.Cells["AgeGroup"].Value;
                curClassRow = getClassRowCurrentSkier();
                String curRerideReason = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
                if ( ( (String)myRecapRow.Cells["ScoreProtRecap"].Value ).Equals( "Y" ) ) {
                    curScoreProtGood = true;
                } else {
                    curScoreProtGood = false;
                }
                if ( ( (String)myRecapRow.Cells["TimeInTolRecap"].Value ).Equals( "Y" ) ) {
                    curTimeGood = true;
                } else {
                    curTimeGood = false;
                }
                myRecapRow.Cells["ScoreRecap"].Value = inScore.ToString("#.00");
                Int16 curSkierPassNum = Convert.ToInt16( (String)myRecapRow.Cells["skierPassRecap"].Value );
                Int16 curPassNum = Convert.ToInt16((String)myRecapRow.Cells["PassNumRecap"].Value);
                Decimal curPassLineLength = Convert.ToDecimal((String) ( myRecapRow.Cells["PassLineLengthRecap"].Value ));


                #region Check for adjustment for division or class with a minimum speed
                if ( myMinSpeedRow == null ) {
                    curPassNumMinSpeed = 0;
                } else {
                    curPassNumMinSpeed = Convert.ToInt16( (Decimal)myMinSpeedRow["MinValue"] );
                    myPassRow = getPassRow( curPassNum );
                    if ( myPassRow != null ) {
                        Int16 curSpeed = Convert.ToInt16( (Decimal)myPassRow["MaxValue"] );
                        if ( curSpeed >= SlalomSpeedSelect.MaxValue ) {
                            if ( curSkierPassNum == 1 ) {
                                if (isDivisionIntl( curAgeGroup )) {
                                    //Reduce pass number for IWWF reduce by an additional 6 bouys for minimum rope length is 18.25M
                                    curPassNumMinSpeed--;
                                }
                            } else {
                                if ( inScore == 6 ) {
                                    if (isDivisionIntl( curAgeGroup )) {
                                        //Reduce pass number for IWWF reduce by an additional 6 bouys for minimum rope length is 18.25M
                                        curPassNumMinSpeed--;
                                    }
                                } else {
                                    Int16 tmpPassNum = Convert.ToInt16( (String)slalomRecapDataGridView.Rows[myRecapRow.Index - 1].Cells["PassNumRecap"].Value );
                                    DataRow tmpPassRow = getPassRow( tmpPassNum );
                                    if ( tmpPassRow != null ) {
                                        Int16 tmpSpeed = Convert.ToInt16( (Decimal)tmpPassRow["MaxValue"] );
                                        if ( tmpSpeed >= SlalomSpeedSelect.MaxValue ) {
                                            if (isDivisionIntl( curAgeGroup )) {
                                                //Reduce pass number for IWWF reduce by an additional 6 bouys for minimum rope length is 18.25M
                                                curPassNumMinSpeed--;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                curPassNum += curPassNumMinSpeed;
                #endregion

                #region Check for adjustment for division or class that allows scoring for line shortening alternate score method
                /*
                Skiers in qualified divisions can increase their score either by increasing speed or shortening the rope or both
                It is no longer required that the skier be scored at long line when at less than max speed
                 */
                if ( isQualifiedAltScoreMethod( curAgeGroup, (String) curClassRow["ListCode"]) ) {
                    Int16 curSpeed = Convert.ToInt16( ( Decimal ) myPassRow["MaxValue"] );

                    if ( curPassLineLength < 23 ) {
                        int curLineSortSeq = 0;
                        if ( curSpeed >= SlalomSpeedSelect.MaxValue ) {
                            for ( int idx = myRecapRow.Index - 1; idx >= 0; idx-- ) {
                                try {
                                    DataGridViewRow tempRecapRow = slalomRecapDataGridView.Rows[idx];
                                    String tempReride = (String) tempRecapRow.Cells["RerideRecap"].Value;
                                    String tempScoreProtRecap = (String) tempRecapRow.Cells["ScoreProtRecap"].Value;
                                    if ( tempReride.Equals("Y") ) {
                                        if ( tempScoreProtRecap.Equals("Y") ) {
                                            Int16 tempPassNum = Convert.ToInt16((String) tempRecapRow.Cells["PassNumRecap"].Value);
                                            DataRow tempPassRow = getPassRow(tempPassNum);
                                            Int16 tempSpeedKph = Convert.ToInt16((Decimal) tempPassRow["MaxValue"]);
                                            if ( tempSpeedKph < SlalomSpeedSelect.MaxValue ) {
                                                Decimal prevPassLineLength = Convert.ToDecimal((String) ( tempRecapRow.Cells["PassLineLengthRecap"].Value ));
                                                curLineSortSeq = ( (int) SlalomLineSelect.myDataTable.Select("ListCodeNum = " + prevPassLineLength)[0]["SortSeq"] ) - 1;
                                            }
                                            break;
                                        }
                                    } else {
                                        if ( (inScore < 6 && myRecapRow.Index > 0) || ( inScore == 6 && myRecapRow.Index > 0 && !isExitGatesGood() ) ) {
                                            Int16 tempPassNum = Convert.ToInt16((String) tempRecapRow.Cells["PassNumRecap"].Value);
                                            DataRow tempPassRow = getPassRow(tempPassNum);
                                            Int16 tempSpeedKph = Convert.ToInt16((Decimal) tempPassRow["MaxValue"]);
                                            if ( tempSpeedKph < SlalomSpeedSelect.MaxValue ) {
                                                Decimal prevPassLineLength = Convert.ToDecimal((String) ( tempRecapRow.Cells["PassLineLengthRecap"].Value ));
                                                curLineSortSeq = ( (int) SlalomLineSelect.myDataTable.Select("ListCodeNum = " + prevPassLineLength)[0]["SortSeq"] ) - 1;
                                            }
                                        }
                                        break;
                                    }
                                } catch ( Exception ex ) {
                                }
                            }
                        } else {
                            DataRow[] curLineRow = SlalomLineSelect.myDataTable.Select("ListCodeNum = " + curPassLineLength);
                            curLineSortSeq = (int) curLineRow[0]["SortSeq"] - 1;

                            if ( ( inScore < 6 && myRecapRow.Index > 0 ) || ( inScore == 6 && myRecapRow.Index > 0 && !isExitGatesGood() ) ) {
                                int prevIdx = myRecapRow.Index - 1;
                                prevRerideInd = (String) slalomRecapDataGridView.Rows[prevIdx].Cells["RerideRecap"].Value;

                                while ( prevRerideInd.Equals("Y") && prevIdx > 0 ) {
                                    prevIdx--;
                                    prevRerideInd = (String) slalomRecapDataGridView.Rows[prevIdx].Cells["RerideRecap"].Value;
                                }

                                Decimal prevPassLineLength = Convert.ToDecimal((String) ( slalomRecapDataGridView.Rows[prevIdx].Cells["PassLineLengthRecap"].Value ));
                                curLineSortSeq = ( (int) SlalomLineSelect.myDataTable.Select("ListCodeNum = " + prevPassLineLength)[0]["SortSeq"] ) - 1;

                            }
                        }

                        curPassNumLineAdjust += (Int16) curLineSortSeq;
                    }
                }
                curPassNum += curPassNumLineAdjust;
                #endregion

                if ( curSkierPassNum == 1 ) {
                    #region Scoring for first pass
                    if (curTimeGood) {
                        if ( inScore == 6 && isExitGatesGood() ) {
                            if (isEntryGatesGood()) {
                                skierScore = inScore * curPassNum;
                                setEventRegRowStatus( "2-InProg" );
                            } else {
                                if ((Decimal)curClassRow["ListCodeNum"] < (Decimal)myClassERow["ListCodeNum"]) {
                                    skierScore = 0;
                                    setEventRegRowStatus( "2-InProg" );
                                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                                    skierPassMsg.Text = "Allowed to continue per rule 10.03 (C)";
                                    myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
                                } else {
                                    skierScore = inScore * curPassNum;
                                    setEventRegRowStatus( "2-InProg" );
                                }
                            }
                        } else {
                            skierScore = inScore;
                            setEventRegRowStatus( "4-Done" );
                        }
                    } else {
                        if ( curScoreProtGood ) {
                            if ( inScore == 6 && isExitGatesGood() ) {
                                skierScore = inScore * curPassNum;
                                setEventRegRowStatus( "2-InProg" );
                            } else {
                                skierScore = inScore;
                                setEventRegRowStatus( "3-Error" );
                            }
                        } else {
                            skierScore = 0;
                            setEventRegRowStatus( "3-Error" );
                        }
                    }
                    #endregion
                } else {
                    #region Scoring for all passes after the first
                    if ( curTimeGood) {
                        #region Scoring when time for current pass is good
                        if ( inScore == 6 && isExitGatesGood() ) {
                            #region Scoring when time for current pass is good and full pass
                            setEventRegRowStatus( "2-InProg" );
                            if ( (Decimal)curClassRow["ListCodeNum"] >= (Decimal)myClassERow["ListCodeNum"] ) {
                                #region Scoring when time for current pass is good and full pass for class E and above skiers
                                int curIdx = myRecapRow.Index - 1;
                                prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                if ( prevRerideInd.Equals("Y") ) {
                                    prevTimeGood = (String)slalomRecapDataGridView.Rows[curIdx].Cells["TimeInTolRecap"].Value;
                                    if (prevTimeGood.Equals("Y")) {
                                        prevProtectInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreProtRecap"].Value;
                                        if ( prevProtectInd.Equals( "Y" ) ) {
                                            skierScore = inScore * curPassNum;
                                        } else {
                                            curPassScore = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreRecap"].Value );
                                            skierScore = curPassScore;
                                            for ( curIdx--; curIdx >= 0; curIdx-- ) {
                                                prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                                if ( prevRerideInd.Equals( "N" ) ) {
                                                    prevPassNum = Convert.ToInt16( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                                    prevPassNum += curPassNumMinSpeed;
                                                    prevPassNum += curPassNumLineAdjust;
                                                    skierScore = curPassScore + ( prevPassNum * 6 );
                                                    break;
                                                }
                                            }
                                        }
                                    } else {
                                        prevProtectInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreProtRecap"].Value;
                                        if (prevProtectInd.Equals("Y")) {
                                            skierScore = inScore * curPassNum;
                                        } else {
                                            prevTimeGood = (String)slalomRecapDataGridView.Rows[curIdx].Cells["TimeInTolRecap"].Value;
                                            if ( prevTimeGood.Equals( "Y" ) ) {
                                                curPassScore = inScore;
                                                skierScore = curPassScore;
                                                for ( curIdx--; curIdx >= 0; curIdx-- ) {
                                                    prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                                    if ( prevRerideInd.Equals( "N" ) ) {
                                                        prevPassNum = Convert.ToByte( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                                        prevPassNum += curPassNumMinSpeed;
                                                        prevPassNum += curPassNumLineAdjust;
                                                        skierScore = curPassScore + ( prevPassNum * 6 );
                                                        break;
                                                    }
                                                }
                                            } else {
                                                if ( (Decimal)curClassRow["ListCodeNum"] >= (Decimal)myClassERow["ListCodeNum"] ) {
                                                    prevPassScore = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value );
                                                    if ( prevPassScore >= inScore ) {
                                                        skierScore = inScore * curPassNum;
                                                    } else {
                                                        myRecapRow.Cells["ScoreRecap"].Value = prevPassScore.ToString("#.00");
                                                        curPassScore = prevPassScore;
                                                        skierScore = curPassScore;
                                                        for ( curIdx--; curIdx >= 0; curIdx-- ) {
                                                            prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                                            if ( prevRerideInd.Equals( "N" ) ) {
                                                                prevPassNum = Convert.ToByte( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                                                prevPassNum += curPassNumMinSpeed;
                                                                prevPassNum += curPassNumLineAdjust;
                                                                skierScore = curPassScore + ( prevPassNum * 6 );
                                                                break;
                                                            }
                                                        }
                                                    }
                                                } else {
                                                    skierScore = inScore * curPassNum;
                                                }
                                            }
                                        }
                                    }
                                } else {
                                    skierScore = inScore * curPassNum;
                                }
                                #endregion
                            } else {
                                skierScore = inScore * curPassNum;
                            }
                            #endregion
                        } else {
                            #region Scoring when time for current pass is good but less than a full pass
                            int curIdx = myRecapRow.Index - 1;
                            if ((Decimal)curClassRow["ListCodeNum"] < (Decimal)myClassERow["ListCodeNum"]) {
                                //For Class C and below tournaments check to see if this is the first score-able pass 
                                //after being allowed to continued after the first pass due to Rule 10.03C
                                int curTempIdx = curIdx;
                                for (curTempIdx = curIdx; curTempIdx >= 0; curTempIdx-- ) {
                                    if ( ((String)slalomRecapDataGridView.Rows[curTempIdx].Cells["RerideRecap"].Value ).Equals( "Y" )
                                        && ((String)slalomRecapDataGridView.Rows[curIdx].Cells["TimeInTolRecap"].Value).Equals( "Y" )
                                        ) {
                                    } else {
                                        break;
                                    }
                                }
                                if (curTempIdx < 0 ) {
                                    curIdx = 0;
                                }
                            }
                            
                            //Check previous passes to determine the final score 
                            prevPassNum = 0;
                            prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                            if ( prevRerideInd.Equals("Y") ) {
                                prevTimeGood = (String)slalomRecapDataGridView.Rows[curIdx].Cells["TimeInTolRecap"].Value;
                                if ( prevTimeGood.Equals("Y") ) {
                                    prevProtectInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreProtRecap"].Value;
                                    if ( prevProtectInd.Equals("Y") ) {
                                        prevPassScore = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value );
                                        if ( prevPassScore > inScore ) {
                                            curPassScore = prevPassScore;
                                            myRecapRow.Cells["ScoreRecap"].Value = prevPassScore.ToString( "#.00" );
                                        } else {
                                            if ( ( (String) slalomRecapDataGridView.Rows[curIdx].Cells["RerideReasonRecap"].Value ).ToLower().Contains("slow")
                                                && ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] )
                                                ) {
                                                curPassScore = prevPassScore;
                                            } else {
                                                curPassScore = inScore;
                                            }
                                        }
                                    } else {
                                        if ( curIdx == 0 && (Decimal) curClassRow["ListCodeNum"] < (Decimal) myClassERow["ListCodeNum"] ) {
                                            if ( ( (String) slalomRecapDataGridView.Rows[curIdx].Cells["RerideReasonRecap"].Value ).Contains("10.03 (C)") ) {
                                                curPassScore = 0;
                                            } else {
                                                curPassScore = inScore;
                                            }
                                        } else if ( ( (String) slalomRecapDataGridView.Rows[curIdx].Cells["RerideReasonRecap"].Value ).ToLower().Contains("slow") 
                                                    && ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] )
                                                ) {
                                            prevPassScore = Convert.ToDecimal((String) slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value);
                                            if ( prevPassScore < inScore ) {
                                                curPassScore = prevPassScore;
                                            } else {
                                                curPassScore = inScore;
                                            }
                                        } else {
                                            curPassScore = inScore;
                                        }
                                    }

                                    skierScore = curPassScore;
                                    for (curIdx--; curIdx >= 0; curIdx--) {
                                        prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                        if (prevRerideInd.Equals("N")) {
                                            prevPassNum = Convert.ToByte((String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value);
                                            prevPassNum += curPassNumMinSpeed;
                                            prevPassNum += curPassNumLineAdjust;
                                            prevPassScore = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value );
                                            if (prevPassScore == 6) {
                                                skierScore = curPassScore + (prevPassNum * 6);
                                                break;
                                            }
                                        }
                                    }
                                } else {
                                    if ( (curIdx == 1) && ((Decimal)curClassRow["ListCodeNum"] < (Decimal)myClassERow["ListCodeNum"] )
                                        && (((String)slalomRecapDataGridView.Rows[0].Cells["RerideReasonRecap"].Value ).Contains( "10.03 (C)" ))) {
                                            curPassScore = 0;
                                    } else {
                                        prevPassScore = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value );
                                        prevProtectInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreProtRecap"].Value;
                                        if (prevProtectInd.Equals( "Y" )) {
                                            if (prevPassScore > inScore) {
                                                curPassScore = prevPassScore;
                                                myRecapRow.Cells["ScoreRecap"].Value = prevPassScore.ToString( "#.00" );
                                            } else {
                                                curPassScore = inScore;
                                            }
                                        } else {
                                            if ((Decimal)curClassRow["ListCodeNum"] >= (Decimal)myClassERow["ListCodeNum"]) {
                                                if (prevPassScore < inScore) {
                                                    curPassScore = prevPassScore;
                                                    myRecapRow.Cells["ScoreRecap"].Value = prevPassScore.ToString( "#.00" );
                                                } else {
                                                    curPassScore = inScore;
                                                }
                                            } else {
                                                curPassScore = inScore;
                                            }
                                        }
                                    }

                                    skierScore = curPassScore;
                                    for (curIdx--; curIdx >= 0; curIdx--) {
                                        prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                        if (prevRerideInd.Equals("N")) {
                                            prevPassNum = Convert.ToByte((String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value);
                                            prevPassNum += curPassNumMinSpeed;
                                            prevPassNum += curPassNumLineAdjust;
                                            prevPassScore = Convert.ToDecimal( (String)slalomRecapDataGridView.Rows[curIdx].Cells["ScoreRecap"].Value );
                                            if (prevPassScore == 6) {
                                                skierScore = curPassScore + (prevPassNum * 6);
                                                break;
                                            }
                                        }
                                    }
                                }
                            } else {
                                prevPassNum = Convert.ToByte( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                prevPassNum += curPassNumMinSpeed;
                                prevPassNum += curPassNumLineAdjust;
                                skierScore = inScore + ( prevPassNum * 6 );
                            }
                            setEventRegRowStatus( "4-Done" );
                            #endregion
                        }
                        #endregion
                    } else {
                        #region Scoring when time for current pass is out of tolerance
                        if ( curScoreProtGood ) {
                            if ( inScore == 6 && isExitGatesGood() ) {
                                skierScore = inScore * curPassNum;
                                setEventRegRowStatus( "2-InProg" );
                            } else {
                                skierScore = inScore;
                                int curIdx = myRecapRow.Index - 1;
                                if ((Decimal)curClassRow["ListCodeNum"] < (Decimal)myClassERow["ListCodeNum"]) {
                                    //For Class C and below tournaments check to see if this is the first score-able pass 
                                    //after being allowed to continued after the first pass due to Rule 10.03C
                                    int curTempIdx = curIdx;
                                    for (curTempIdx = curIdx; curTempIdx >= 0; curTempIdx--) {
                                        if (( (String)slalomRecapDataGridView.Rows[curTempIdx].Cells["RerideRecap"].Value ).Equals( "Y" )
                                            && ( (String)slalomRecapDataGridView.Rows[curIdx].Cells["TimeInTolRecap"].Value ).Equals( "Y" )
                                            ) {
                                        } else {
                                            break;
                                        }
                                    }
                                    if (curTempIdx < 0) {
                                        curIdx = 0;
                                        skierScore = 0;
                                    }
                                }
                                if (curIdx >= 0) {
                                    for (curIdx = myRecapRow.Index - 1; curIdx >= 0; curIdx--) {
                                        prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                        if (prevRerideInd.Equals( "N" )) {
                                            prevPassNum = Convert.ToByte( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                            prevPassNum += curPassNumMinSpeed;
                                            prevPassNum += curPassNumLineAdjust;
                                            skierScore += ( prevPassNum * 6 );
                                            break;
                                        }
                                    }
                                }
                                setEventRegRowStatus( "4-Done" );
                            }
                        } else {
                            skierScore = 0;
                            for ( int curIdx = myRecapRow.Index - 1; curIdx >= 0; curIdx-- ) {
                                prevRerideInd = (String)slalomRecapDataGridView.Rows[curIdx].Cells["RerideRecap"].Value;
                                if ( prevRerideInd.Equals( "N" ) ) {
                                    prevPassNum = Convert.ToByte( (String)slalomRecapDataGridView.Rows[curIdx].Cells["PassNumRecap"].Value );
                                    prevPassNum += curPassNumMinSpeed;
                                    prevPassNum += curPassNumLineAdjust;
                                    skierScore += ( prevPassNum * 6 );
                                    break;
                                }
                            }
                            setEventRegRowStatus( "3-Error" );
                        }
                        #endregion
                    }
                    #endregion
                }

                scoreTextBox.Text = skierScore.ToString();
                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = skierScore.ToString( "###.00" );
				hcapScoreTextBox.Text = ( skierScore + Decimal.Parse( (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["HCapScore"].Value ) ).ToString( "##,###0.0" );


				ScoreList[0].Score = Convert.ToDecimal(skierScore.ToString());

                appNopsCalc.calcNops(curAgeGroup, ScoreList);
                nopsScoreTextBox.Text = Math.Round(ScoreList[0].Nops, 1).ToString();

                isDataModified = true;
                myRecapRow.Cells["Updated"].Value = "Y";
                //Reset 
                if ( curSkierPassNum > 1 ) {
                    if ( inScore > 0M || ( inScore == 0M && curTimeGood ) ) {
                        curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );
                        if ( prevPassNum > 0 ) {
                            if ( curPassNum > ( prevPassNum - curPassNumMinSpeed + curPassNumLineAdjust  + 1 ) ) {
                                curPassNum = prevPassNum;
                                curPassNum -= curPassNumMinSpeed;
                                curPassNum -= curPassNumLineAdjust;
                                curPassNum++;
                            }
                        }
                    } else {
                        if (prevPassNum > 0) {
                            curPassNum = prevPassNum;
                            curPassNum -= curPassNumMinSpeed;
                            curPassNum -= curPassNumLineAdjust;
                        } else {
                            curPassNum--;
                            curPassNum -= curPassNumMinSpeed;
                            curPassNum -= curPassNumLineAdjust;
                        }
                    }
                } else {
                    curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );
                }
                myPassRow = getPassRow( curPassNum );
                saveScore();

            } catch (Exception ex) {
                MessageBox.Show(curMethodName + ": \n" + ex.Message);
            }
        }

        private void ForceCompButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Slalom:ScoreEntry:ForceCompButton_Click";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            if ( TourEventRegDataGridView.Rows.Count > 0 ) {
                if ( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" )
                    || TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "3-Error" ) 
                    ) {

                    String myBoatTime = "";
                    try {
                        myBoatTime = myRecapRow.Cells["BoatTimeRecap"].Value.ToString();
                    } catch {
                        myBoatTime = "";
                    }
                    if ( myBoatTime.Length < 2 ) {
                        deletePassButton_Click( null, null );
                    }
                    try {
                        if ( myScoreRow != null ) {
                            skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
                            if ( skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK ) {
                                String curReason = skierDoneReasonDialogForm.ReasonText;
                                if ( curReason.Length > 3 ) {
                                    noteTextBox.Text = curReason;
                                    myScoreRow["Note"] = curReason;
                                    curReason = curReason.Replace( "'", "''" );
                                    Int64 curScorePK = (Int64)myScoreRow["PK"];
                                    String curStatus = "TBD";
                                    try {
                                        setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
                                        String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
                                        if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
                                        if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
                                        if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
                                        if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
                                    } catch {
                                        curStatus = "TBD";
                                    }

                                    curSqlStmt.Append( "Update SlalomScore Set " );
                                    curSqlStmt.Append( "Status = '" + curStatus + "'" );
                                    curSqlStmt.Append( ", Note = '" + curReason + "'" );
                                    curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                                    curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                }
                            } else {
                                MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                            }
                        } else {
                            MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                        }
                    } catch ( Exception excp ) {
                        String curMsg = ":Error attempting to update skier score status\n" + excp.Message + "\n" + curSqlStmt.ToString();
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                    
                    isDataModified = false;
                }
            }
        }

		private void BoatSelectButton_Click( object sender, EventArgs e ) {
			if ( approvedBoatSelectGroupBox.Visible ) {
				approvedBoatSelectGroupBox.Visible = false;
				listApprovedBoatsDataGridView.Visible = false;
			} else {
				approvedBoatSelectGroupBox.Visible = true;
				listApprovedBoatsDataGridView.Visible = true;
				listApprovedBoatsDataGridView.Focus();
				listApprovedBoatsDataGridView.CurrentCell = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"];
			}
		}

		private void selectBoatButton_Click( object sender, EventArgs e ) {
			myBoatListIdx = listApprovedBoatsDataGridView.CurrentRow.Index;
            if ( myBoatListIdx > 0 ) {
				updateBoatSelect();
			}
		}

		private void refreshBoatListButton_Click( object sender, EventArgs e ) {
            getApprovedTowboats();
			calcBoatCodeFromDisplay( TourBoatTextbox.Text );
		}

		private void listApprovedBoatsDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            if (e.RowIndex > 0) {
                myBoatListIdx = e.RowIndex;
				updateBoatSelect();
            }
        }

		private void updateBoatSelect() {
            String curMethodName = "Slalom:ScoreEntry:updateBoatSelect";
            String curMsg = "";

			TourBoatTextbox.Text = buildBoatModelNameDisplay();
            TourBoatTextbox.Focus();
			TourBoatTextbox.Select( 0, 0 );
			String curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			if ( myScoreRow != null ) {
                try {
                    Int64 curScorePK = (Int64)myScoreRow["PK"];

                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update SlalomScore Set Boat = '" + curBoatCode + "'" );
                    curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				} catch ( Exception excp ) {
                    curMsg = ":Error attempting to update boat selection \n" + excp.Message ;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
            }

            listApprovedBoatsDataGridView.Visible = false;
            approvedBoatSelectGroupBox.Visible = false;
			slalomRecapDataGridView.Focus();
        }

		private String calcBoatCodeFromDisplay(String inBoatModelNameDisplay ) {
			try {
				if ( inBoatModelNameDisplay.Equals( "--Select--" ) ) {
					myBoatListIdx = 0;
					return "--Select--";

				} else if ( inBoatModelNameDisplay.Equals( "" ) ) {
					myBoatListIdx = 0;
					return "--Select--";

				} else {
					int delimIdx = inBoatModelNameDisplay.IndexOf( " (KEY: " );
					delimIdx += 7;
					int lenBoatCode = inBoatModelNameDisplay.Length - delimIdx - 1;
					String curBoatCode = inBoatModelNameDisplay.Substring( delimIdx, lenBoatCode );
					curBoatCode = curBoatCode.Replace( "'", "''" );
					setApprovedBoatSelectEntry( curBoatCode );
                    return curBoatCode;
				}

			} catch {
				return "";
			}
		}

		private String setApprovedBoatSelectEntry( String inBoatCode ) {
			try {
				foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
					if ( ( (String) curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( inBoatCode.ToUpper() ) ) {
						myBoatListIdx = curBoatRow.Index;
						return buildBoatModelNameDisplay();
					}
				}

				myBoatListIdx = 0;
				return "";

			} catch {
				TourBoatTextbox.Text = "";
				foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
					if ( ( (String) curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( "UNDEFINED" ) ) {
						myBoatListIdx = curBoatRow.Index;
						return buildBoatModelNameDisplay();
					}
				}

				myBoatListIdx = 0;
				return "";

			}
		}

		private String buildBoatModelNameDisplay() {
			if ( myBoatListIdx > 0 ) {
				String curBoatCode = (String) listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCode"].Value;
				String curBoatModelName = (String) listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"].Value;
				return curBoatModelName + " (KEY: " + curBoatCode + ")";
			} else {
				return "";
			}
		}

		private DataRow getPassRow( Int16 inPassNum ) {
            DataRow curRow;
            String curPassSearchCmd = "ListCodeNum = " + ( (Byte)inPassNum ).ToString();
            DataRow[] curRowsFound = myPassDataTable.Select( curPassSearchCmd );
            if ( curRowsFound.Length > 0 ) {
                curRow = curRowsFound[0];
            } else {
                curRow = null;
            }
            return curRow;
        }
        private DataRow getPassRow( Int16 inSpeed, Decimal inPassLine ) {
            String curPassSearchCmd = "";
            Decimal curPassLine = inPassLine;
            DataRow curRow;
            Int16 curMaxSpeed = SlalomSpeedSelect.MaxValue;
            if ( curMaxSpeed > inSpeed ) {
                curPassLine = Convert.ToDecimal( "23.000" );
                curPassSearchCmd = "MinValue = '" + curPassLine + "' AND MaxValue = '" + inSpeed + "'";
            } else if ( inSpeed > curMaxSpeed ) {
                SlalomSpeedSelect.CurrentValue = inSpeed;
                curPassSearchCmd = "MinValue = '" + curPassLine + "' AND MaxValue = '" + curMaxSpeed + "'";
            } else {
                curPassSearchCmd = "MinValue = '" + curPassLine + "' AND MaxValue = '" + inSpeed + "'";
            }
            DataRow[] curRowsFound = myPassDataTable.Select( curPassSearchCmd );
            if ( curRowsFound.Length > 0 ) {
                curRow = curRowsFound[0];
            } else {
                curRow = null;
            }
            return curRow;
        }

        private void printButton_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            myPrintDoc = new PrintDocument();
            curPrintDialog.Document = myPrintDoc;
            CaptureScreen();

            curPrintDialog.AllowCurrentPage = false;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = false;
            curPrintDialog.AllowSomePages = false;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if(curPrintDialog.ShowDialog() == DialogResult.OK) {
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintScreen );
                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 10, 10, 10, 10 );
                myPrintDoc.DefaultPageSettings.Landscape = true;

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        private void printDoc_PrintScreen( object sender, PrintPageEventArgs e ) {
            int curXPos = 0;
            int curYPos = 25;
            Font fontPrintTitle = new Font( "Arial", 16, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            //display a title
            curXPos = 100;
            e.Graphics.DrawString( myPrintDoc.DocumentName, fontPrintTitle, Brushes.Black, curXPos, curYPos );

            //display form
            curXPos = 50;
            curYPos += 50;
            e.Graphics.DrawImage( memoryImage, curXPos, curYPos );

            curXPos = 50;
            curYPos = curYPos + memoryImage.Height + 25;
            String curFooter = "Date: " + DateTime.Now.ToString( "MM/dd/yyyy hh:mm" );
            e.Graphics.DrawString( curFooter, fontPrintFooter, Brushes.Black, curXPos, curYPos );
        }

        [System.Runtime.InteropServices.DllImport( "gdi32.dll" )]
        public static extern long BitBlt( IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop );
        private Bitmap memoryImage;
        private void CaptureScreen() {
            Graphics mygraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap( s.Width, s.Height, mygraphics );
            Graphics memoryGraphics = Graphics.FromImage( memoryImage );
            IntPtr dc1 = mygraphics.GetHdc();
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt( dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376 );
            mygraphics.ReleaseHdc( dc1 );
            memoryGraphics.ReleaseHdc( dc2 );
        }

        private void navPrintResults_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            DataGridView[] myViewList = new DataGridView[1];
            myViewList[0] = slalomRecapDataGridView;

            bool CenterOnPage = false;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;
                myPrintDoc = new PrintDocument();

                String curDocName = "";
                String curAgeGroup = (String)TourEventRegDataGridView.CurrentRow.Cells["AgeGroup"].Value;
                String curSkierName = (String)TourEventRegDataGridView.CurrentRow.Cells["SkierName"].Value;
                String[] curNameParts = curSkierName.Split( ',' );
                if ( curNameParts.Length > 1 ) {
                    curDocName = curNameParts[1].Trim() + curNameParts[0].Trim() + "_" + curAgeGroup;
                } else if ( curNameParts.Length == 1 ) {
                    curDocName = curNameParts[1].Trim() + curNameParts[0].Trim() + "_" + curAgeGroup;
                } else {
                    curDocName = "Unknown";
                }
                curSkierName += "  " + curAgeGroup;

                myPrintDoc.DocumentName = curDocName;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 125, 125, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter( myViewList, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                //Build titles for each data grid view
                myPrintDataGrid.GridViewTitleList();
                StringRowPrinter myGridTitle;
                StringFormat GridTitleStringFormat = new StringFormat();
                GridTitleStringFormat.Trimming = StringTrimming.Word;
                GridTitleStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                GridTitleStringFormat.Alignment = StringAlignment.Near;


                myGridTitle = new StringRowPrinter( "Slalom Pass Round " + roundSelect.RoundValue,
                    0, 20, 325, fontPrintTitle.Height,
                    Color.DarkBlue, Color.White, fontPrintTitle, GridTitleStringFormat );
                myPrintDataGrid.GridViewTitleRow = myGridTitle;

                //Build report page subtitles
                myPrintDataGrid.SubtitleList();
                StringRowPrinter mySubtitle;
                StringFormat SubtitleStringFormatLeft = new StringFormat();
                SubtitleStringFormatLeft.Trimming = StringTrimming.Word;
                SubtitleStringFormatLeft.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                SubtitleStringFormatLeft.Alignment = StringAlignment.Near;

                StringFormat SubtitleStringFormatCenter = new StringFormat();
                SubtitleStringFormatCenter.Trimming = StringTrimming.Word;
                SubtitleStringFormatCenter.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                SubtitleStringFormatCenter.Alignment = StringAlignment.Center;

                mySubtitle = new StringRowPrinter( curSkierName, 0, 0, 325, fontPrintTitle.Height,
                    Color.DarkBlue, Color.White, fontPrintTitle, SubtitleStringFormatLeft );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( scoreLabel.Text,
                    0, 25, 50, scoreLabel.Size.Height,
                    Color.Black, Color.White, scoreLabel.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scoreTextBox.Text,
                    0, 40, 50, scoreTextBox.Size.Height,
                    Color.Black, Color.White, scoreTextBox.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( nopsScoreLabel.Text,
                    170, 25, 50, nopsScoreLabel.Size.Height,
                    Color.Black, Color.White, nopsScoreLabel.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( nopsScoreTextBox.Text,
                    170, 40, 55, nopsScoreTextBox.Size.Height,
                    Color.Black, Color.White, nopsScoreTextBox.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private void ScoreEntry_Deactivate( object sender, EventArgs e ) {
            myWindowFocus = false;
        }

        private void ScoreEntry_Activated( object sender, EventArgs e ) {
            myWindowFocus = true;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
            return curDataTable;
		}

		private void getApprovedTowboats() {
            int curIdx = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq " );
            curSqlStmt.Append( "FROM TourBoatUse BU " );
            curSqlStmt.Append( "INNER JOIN CodeValueList BL ON BL.ListCode = BU.HullId " );
            curSqlStmt.Append( "WHERE BL.ListName = 'ApprovedBoats' AND BU.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Union " );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq " );
            curSqlStmt.Append( "FROM CodeValueList BL " );
            curSqlStmt.Append( "WHERE ListName = 'ApprovedBoats' AND ListCode IN ('--Select--', 'Unlisted') " );
            curSqlStmt.Append( "ORDER BY SortSeq DESC" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			listApprovedBoatsDataGridView.Rows.Clear();
            foreach ( DataRow curRow in curDataTable.Rows ) {
                String[] boatSpecList = curRow["CodeDesc"].ToString().Split( '|' );
                listApprovedBoatsDataGridView.Rows.Add( 1 );
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatCode"].Value = curRow["ListCode"].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatModelApproved"].Value = curRow["CodeValue"].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["HullStatus"].Value = boatSpecList[0].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["EngineSpec"].Value = boatSpecList[1].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["FuelDel"].Value = boatSpecList[2].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["Transmission"].Value = boatSpecList[3].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["Prop"].Value = boatSpecList[4].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["SpeedControl"].Value = boatSpecList[5].ToString();
                curIdx++;
            }
            myBoatListIdx = 0;

            if (curDataTable.Rows.Count < 3) {
                MessageBox.Show( "You have no boats defined for your tournament."
                + "\nUse the Boat Use window to add boats to your tournament "
                + "and make them available for event selection" );
            }

        }

        private void getSkierScoreByRound( String inMemberId, String inAgeGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, S.Round, S.EventClass");
            curSqlStmt.Append(", S.Score, S.NopsScore, S.Rating, S.MaxSpeed, S.StartLen, S.StartSpeed, S.Boat, S.Status");
            curSqlStmt.Append(", S.FinalPassNum, S.FinalSpeedMph, S.FinalSpeedKph, S.FinalLen, S.FinalLenOff, S.FinalPassScore, S.Note, S.LastUpdateDate");
            curSqlStmt.Append(", Gender, SkiYearAge ");
            curSqlStmt.Append( "FROM SlalomScore S " );
            curSqlStmt.Append("     INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup ");
            curSqlStmt.Append("WHERE S.SanctionId = '" + mySanctionNum + "' AND S.MemberId = '" + inMemberId + "'");
            curSqlStmt.Append("  AND S.AgeGroup = '" + inAgeGroup + "' AND S.Round = " + inRound.ToString() );
            curSqlStmt.Append(" ORDER BY S.SanctionId, S.MemberId");
            myScoreDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void getSkierRecapByRound( String inMemberId, String inAgeGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum, SkierRunNum, " );
            curSqlStmt.Append( "BoatTime, EntryGate1, EntryGate2, EntryGate3, ExitGate1, ExitGate2, ExitGate3, " );
            curSqlStmt.Append( "Judge1Score, Judge2Score, Judge3Score, Judge4Score, Judge5Score, PassLineLength, " );
            curSqlStmt.Append( "Reride, RerideReason, Score, ScoreProt, TimeInTol, Note, LastUpdateDate " );
            curSqlStmt.Append( "FROM SlalomRecap " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, SkierRunNum" );
            myRecapDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private Boolean checkForSkierRoundScore( String inMemberId, int inRound, String inAgeGroup ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round " );
			curSqlStmt.Append( "FROM SlalomScore " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( " AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( " AND Round = " + inRound + " " );
			if ( mySanctionNum.EndsWith("999") || mySanctionNum.EndsWith( "998" ) ) {
				curSqlStmt.Append( " AND AgeGroup = '" + inAgeGroup + "' " );
			}
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return true;
			} else {
				return false;
			}
		}

		private void getEventRegData(int inRound) {
            getEventRegData( "All", inRound );
        }
        private void getEventRegData( String inEventGroup, int inRound ) {
            int curIdx = 0, curRowCount = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            while (curIdx < 2 && curRowCount == 0) {
                curSqlStmt = new StringBuilder( "" );
                if (curIdx == 0) {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, O.EventGroup,  O.RunOrder, E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append(", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating, E.HCapBase");
                    curSqlStmt.Append(", E.HCapScore, COALESCE (S.Status, 'TBD') AS Status, S.Score, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Slalom' " );
                } else {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup,  E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append(", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.HCapBase, E.HCapScore");
                    curSqlStmt.Append(", COALESCE (S.Status, 'TBD') AS Status, S.Score, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN SlalomScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Slalom' " );
                }
                if (!(inEventGroup.ToLower().Equals( "all" ))) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        if (inEventGroup.ToUpper().Equals( "MEN A" )) {
                            curSqlStmt.Append( "And E.AgeGroup = 'CM' " );
                        } else if (inEventGroup.ToUpper().Equals( "WOMEN A" )) {
                            curSqlStmt.Append( "And E.AgeGroup = 'CW' " );
                        } else if (inEventGroup.ToUpper().Equals( "MEN B" )) {
                            curSqlStmt.Append( "And E.AgeGroup = 'BM' " );
                        } else if (inEventGroup.ToUpper().Equals( "WOMEN B" )) {
                            curSqlStmt.Append( "And E.AgeGroup = 'BW' " );
                        } else {
                            curSqlStmt.Append( "And E.AgeGroup not in ('CM', 'CW', 'BM', 'BW') " );
                        }
                    } else {
                        if (curIdx == 0) {
                            curSqlStmt.Append( "And O.EventGroup = '" + inEventGroup + "' " );
                        } else {
                            curSqlStmt.Append( "And E.EventGroup = '" + inEventGroup + "' " );
                        }
                    }
                }

                myTourEventRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                curRowCount = myTourEventRegDataTable.Rows.Count;
                curIdx++;
            }
        }

        private void getPassData( String inListName ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListName, ListCode, ListCodeNum, CodeValue, MinValue, MaxValue, CodeDesc" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = '" + inListName + "'" );
            curSqlStmt.Append( " ORDER BY SortSeq, ListCode" );
            myPassDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private Int16 getMaxSpeedData( String inAgeGroup ) {
            Int16 curMaxSpeed = 55;

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE (ListName like '%SlalomMax' AND ListCode = '" + inAgeGroup + "')" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            DataTable curMaxSpeedDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
            if ( curMaxSpeedDataTable.Rows.Count > 0 ) {
                curMaxSpeed = Convert.ToInt16((Decimal) curMaxSpeedDataTable.Rows[0]["MaxValue"]);
            } else {
                curMaxSpeed = SlalomSpeedSelect.MaxValue;
            }

            return curMaxSpeed;
        }

        private Int16 getMaxSpeedOrigData( String inAgeGroup ) {
            Int16 curMaxSpeedDiv = 0;

            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
            curSqlStmt.Append(" FROM CodeValueList");
            curSqlStmt.Append(" WHERE (ListName like '%SlalomMaxDiv%' AND ListCode = '" + inAgeGroup + "')");
            curSqlStmt.Append(" ORDER BY SortSeq");

            DataTable curMaxSpeedDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
            if ( curMaxSpeedDataTable.Rows.Count > 0 ) {
                curMaxSpeedDiv = Convert.ToInt16((Decimal) curMaxSpeedDataTable.Rows[0]["MaxValue"]);
            } else {
                curMaxSpeedDiv = getMaxSpeedData( inAgeGroup );
            }

            return curMaxSpeedDiv;
        }

        private DataTable getMinSpeedData( String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );

            // Disabling the Alumni functionality at this time 8/8/17.  
            // Do not want it used inadvertently
            if ( ((String)myTourRow["Name"]).Contains("##Alumni##") ) {
                curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
                curSqlStmt.Append(" FROM CodeValueList");
                curSqlStmt.Append(" WHERE ListName = 'AlumniMinSlalom' AND ListCode = '" + inAgeGroup + "'");
                curSqlStmt.Append(" ORDER BY SortSeq");
            } else {
                curSqlStmt.Append("SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue");
                curSqlStmt.Append(" FROM CodeValueList");
                curSqlStmt.Append(" WHERE ListName like '%SlalomMin' AND ListCode = '" + inAgeGroup + "'");
                curSqlStmt.Append(" ORDER BY SortSeq");
            }

            return DataAccess.getDataTable( curSqlStmt.ToString());
        }

        private Boolean isDivisionIntl(String inAgeGroup) {
            if (myDivisionIntlDataTable == null) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT Distinct ListName, ListCode as Division, CodeValue as DivisionName " );
                curSqlStmt.Append( "FROM CodeValueList " );
                curSqlStmt.Append( "WHERE ListName = 'IwwfAgeGroup'" );
                myDivisionIntlDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            }
            if (myDivisionIntlDataTable == null) {
                return false;
            } else {
                DataRow[] curRowsFound = myDivisionIntlDataTable.Select( "Division = '" + inAgeGroup + "'" );
                if (curRowsFound.Length > 0) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        private Boolean isQualifiedAltScoreMethod( String inAgeGroup, String inSkierClass ) {
            /*
             * Rule 10.06 for ski year 2016
             * Also Collegiate divisions
            */
            if ( myDivisionAwsaJuniorDataTable == null ) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append("SELECT Distinct ListName, ListCode, SortSeq, CodeValue as Description ");
                curSqlStmt.Append("FROM CodeValueList " );
                curSqlStmt.Append("WHERE ListName = 'SlalomAltScoreMethodQual'");
                myDivisionAwsaJuniorDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            }
            if ( myDivisionAwsaJuniorDataTable == null ) {
                return false;
            } else {
                DataRow[] curRowsFound = myDivisionAwsaJuniorDataTable.Select("ListCode = '" + inAgeGroup + "'" );
                if ( curRowsFound.Length > 0 ) {
                    return true;
                } else {
                    curRowsFound = myDivisionAwsaJuniorDataTable.Select("ListCode = '" + inAgeGroup + "-" + inSkierClass + "'");
                    if ( curRowsFound.Length > 0 ) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        }

        private DataRow getClassRowCurrentSkier() {
            return getClassRow( ( String ) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value );
        }
        private DataRow getClassRow(String inClass) {
            DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + inClass + "'" );
            if ( curRowsFound.Length > 0 ) {
                return curRowsFound[0];
            } else {
                return mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + myTourClass + "'" )[0];
            }
        }

        private bool isEntryGatesGood() {
            int curGateEntryValue = 0;
            if (myNumJudges > 1) {
                if ((Boolean)myRecapRow.Cells["GateEntry1Recap"].Value) curGateEntryValue++;
                if ((Boolean)myRecapRow.Cells["GateEntry2Recap"].Value) curGateEntryValue++;
                if ((Boolean)myRecapRow.Cells["GateEntry3Recap"].Value) curGateEntryValue++;
                if (curGateEntryValue > 1) {
                    return true;
                } else {
                    return false;
                }
            } else {
                if ((Boolean)myRecapRow.Cells["GateEntry1Recap"].Value) curGateEntryValue++;
                if (curGateEntryValue > 0) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        private bool isExitGatesGood(DataRow inRow) {
            Boolean curGatesGood = true;
            Boolean curGateValue;

            int curGateExitValue = 0;
            try {
                curGateValue = (Boolean)inRow["ExitGate1"];
            } catch ( Exception ex ) {
                curGateValue = false;
                MessageBox.Show( "isExitGatesGood Exception:" + ex.Message );
            }
            if ( !( curGateValue ) ) curGateExitValue++;

            if ( myNumJudges > 1 ) {
                try {
                    curGateValue = (Boolean)inRow["ExitGate2"];
                } catch {
                    curGateValue = false;
                }
                if ( !( curGateValue ) ) curGateExitValue++;
            }
            if ( myNumJudges > 2 ) {
                try {
                    curGateValue = (Boolean)inRow["ExitGate3"];
                } catch {
                    curGateValue = false;
                }
                if ( !( curGateValue ) ) curGateExitValue++;
            }
            if ( myNumJudges > 2 ) {
                if ( curGateExitValue < 2 ) {
                    curGatesGood = true;
                } else {
                    curGatesGood = false;
                }
            } else {
                if ( curGateExitValue < 2 ) {
                    curGatesGood = true;
                } else {
                    curGatesGood = false;
                }
            }
            return curGatesGood;
        }
        private bool isExitGatesGood() {
            Boolean curGatesGood = true;
            int curGateExitValue = 0;
            if ( !( (Boolean)myRecapRow.Cells["GateExit1Recap"].Value ) ) curGateExitValue++;
            if ( myNumJudges > 1 ) {
                if ( !( (Boolean)myRecapRow.Cells["GateExit2Recap"].Value ) ) curGateExitValue++;
            }
            if ( myNumJudges > 2 ) {
                if ( !( (Boolean)myRecapRow.Cells["GateExit3Recap"].Value ) ) curGateExitValue++;
            }
            if ( myNumJudges > 2 ) {
                if ( curGateExitValue < 2 ) {
                    curGatesGood = true;
                } else {
                    curGatesGood = false;
                }
            } else {
                if ( curGateExitValue < 2 ) {
                    curGatesGood = true;
                } else {
                    curGatesGood = false;
                }
            }
            return curGatesGood;
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

		private void TourBoatLabel_Click( object sender, EventArgs e ) {

		}

		private void TourBoatTextbox_TextChanged( object sender, EventArgs e ) {

		}
	}
}
