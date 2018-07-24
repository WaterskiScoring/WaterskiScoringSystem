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

namespace WaterskiScoringSystem.Jump {
    public partial class ScoreEntrySeg3 : Form {
        #region instance variables 
        private bool isAddRecapRowInProg = false;
        private bool isRecapRowEnterHandled = false;
        private bool isDataModified;
        private bool isLoadInProg = false;

        private Decimal myMetericFactor = 3.2808M;
        private Decimal myMeterDistTol;
        private Decimal myMeterZeroTol;
        private Decimal myJumpHeight;
        private Decimal myMeter6Tol;

        private int myEventRegViewIdx = 0;
        private int myStartCellIndex;
        private int myBoatListIdx = 0;
        private Int16 myMaxSpeed;
        private Int16 myNumJudges;
        private Int16 mySanctionYear = 0;
        private Int16 mySanctionYear2012 = 12;

        private String mySanctionNum;
        private String myTourClass;
        private String myRuleType;
        private String myModCellValue;
        private String myOrigCellValue;
        private String myTriangleTolMsg;
        private String myTimeTolMsg;
        private String mySortCommand = "";
        private String myFilterCmd = "";
        private String myTourRules = "";
		private String myPrevEventGroup = "";

		private int mySkierRunCount = 0;
        private int myPassRunCount = 0;
        private DateTime myEventStartTime;
        private DateTime myEventDelayStartTime;
        private int myEventDelaySeconds = 0;

        private DataGridViewRow myRecapRow;
        private DataRow myTourRow;
        private DataRow myScoreRow;
        private DataRow myTimeRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;
        private DataRow myClassRow;

        private DataTable myTourEventRegDataTable;
        private DataTable myScoreDataTable;
        private DataTable myRecapDataTable;
        private DataTable myTimesDataTable;
        private DataTable myMaxSpeedDataTable;
        private DataTable myMinSpeedDataTable;
        private DataTable myMaxRampDataTable;
        private DataTable myJump3TimesDivDataTable;

        private ListSkierClass mySkierClassList;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EventGroupSelect eventGroupSelect;
        private SkierDoneReason skierDoneReasonDialogForm;
        private RerideReason rerideReasonDialogForm;
        private CheckEventRecord myCheckEventRecord;
        private AgeGroupDropdownList myAgeDivList;
		private CheckOfficials myCheckOfficials;

		private TourProperties myTourProperties;
        private CalcNops appNopsCalc;
        private JumpCalc myJumpCalc;
        private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        #endregion

        public ScoreEntrySeg3() {
            InitializeComponent();

            appNopsCalc = CalcNops.Instance;
            appNopsCalc.LoadDataForTour();
            ScoreList.Add(new Common.ScoreEntry("Jump", 0, "", 0));
            TimeInTolImg.DisplayIndex = 16;

            jumpRecapDataGridView.AutoGenerateColumns = false;

            //RampHeightTextBox.Visible = false;
            //BoatSpeedTextBox.Visible = false;
        }

        public Int16 NumJudges {
            get {
                return myNumJudges;
            }
            set {
                jumpRecapDataGridView.Width = 868;
                myNumJudges = value;
                if (myNumJudges == 0) {
                    jumpRecapDataGridView.Visible = true;
                    ScoreFeetRecap.ReadOnly = false;
                    ScoreMetersRecap.ReadOnly = false;
                    ScoreTriangleRecap.Visible = false;

                    Meter6Recap.Visible = false;
                    Meter5Recap.Visible = false;
                    Meter4Recap.Visible = false;
                    Meter3Recap.Visible = false;
                    Meter2Recap.Visible = false;
                    Meter1Recap.Visible = false;
                    jumpRecapDataGridView.Width += -285;
                } else if ( myNumJudges == 3 ) {
                    ScoreFeetRecap.ReadOnly = true;
                    ScoreMetersRecap.ReadOnly = true;
                    ScoreTriangleRecap.Visible = true;

                    Meter6Recap.Visible = false;
                    Meter5Recap.Visible = false;
                    Meter4Recap.Visible = false;
                    Meter3Recap.Visible = true;
                    Meter2Recap.Visible = true;
                    Meter1Recap.Visible = true;
                    jumpRecapDataGridView.Width += -120;
                } else {
                    ScoreFeetRecap.ReadOnly = true;
                    ScoreMetersRecap.ReadOnly = true;
                    ScoreTriangleRecap.Visible = true;

                    Meter6Recap.Visible = true;
                    Meter5Recap.Visible = true;
                    Meter4Recap.Visible = true;
                    Meter3Recap.Visible = true;
                    Meter2Recap.Visible = true;
                    Meter1Recap.Visible = true;
                }
                myStartCellIndex = jumpRecapDataGridView.Columns["ResultsRecap"].Index;

                if ( myNumJudges > 0 ) {
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT ListName, ListCode, CodeValue as TriangleFeet FROM CodeValueList " );
                    curSqlStmt.Append( "WHERE ListName in ('JumpMeter6Tol', 'JumpTriangle', 'JumpTriangleZero') " );
                    curSqlStmt.Append( "And ListCode = '" + myTourClass + "' " );
                    curSqlStmt.Append( "ORDER BY SortSeq" );
                    DataTable curTolDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if ( curTolDataTable.Rows.Count > 0 ) {
                        DataRow myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangle'" )[0];
                        myMeterDistTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );
                        myTolRow = curTolDataTable.Select( "ListName = 'JumpTriangleZero'" )[0];
                        myMeterZeroTol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );
                        myTolRow = curTolDataTable.Select( "ListName = 'JumpMeter6Tol'" )[0];
                        myMeter6Tol = Convert.ToDecimal( (String)myTolRow["TriangleFeet"] );

                        bool isAllDataValid = true;
                        myJumpCalc = new JumpCalc( mySanctionNum );
                        if ( myJumpCalc.distAtoB > 0 ) {
                            isAllDataValid = myJumpCalc.ValidateMeterSetup();
                            if ( isAllDataValid ) {
                                if ( myMeterZeroTol < myJumpCalc.TriangleZero ) {
                                    jumpRecapDataGridView.Visible = false;
                                    isAllDataValid = false;
                                    MessageBox.Show( "Setup triangle at jump is not within allowed tolerance"
                                        + "\nCurrent calculated triangle at jump is " + myJumpCalc.TriangleZero.ToString( "#0.00" )
                                        + "\nAllowed tolerance for tournament class " + myTourClass + " is " + myMeterZeroTol.ToString( "#0.00" )
                                        );
                                    if ( myMeterZeroTol < myJumpCalc.Triangle15M ) {
                                        jumpRecapDataGridView.Visible = false;
                                        isAllDataValid = false;
                                        MessageBox.Show( "Setup triangle at 15 meter timing buoy is not within allowed tolerance"
                                            + "\nCurrent calculated triangle at 15meter timing buoy is " + myJumpCalc.Triangle15M.ToString( "#0.00" )
                                            + "\nAllowed tolerance for tournament class " + myTourClass + " is " + myMeterZeroTol.ToString( "#0.00" )
                                            );
                                    }
                                } else {
                                    if ( myMeterZeroTol < myJumpCalc.Triangle15M ) {
                                        jumpRecapDataGridView.Visible = false;
                                        isAllDataValid = false;
                                        MessageBox.Show("Setup triangle at 15 meter timing buoy is not within allowed tolerance"
                                            + "\nCurrent calculated triangle at 15meter timing buoy is " + myJumpCalc.Triangle15M.ToString("#0.00")
                                            + "\nAllowed tolerance for tournament class " + myTourClass + " is " + myMeterZeroTol.ToString("#0.00")
                                            );
                                    }
                                }
                            } else {
                                jumpRecapDataGridView.Visible = false;
                                isAllDataValid = false;
                                MessageBox.Show( "Jump meter setup has not been successfully completed."
                                    + "\n Setup must be complete before distances can be calculated."
                                    );
                            }
                        } else {
                            jumpRecapDataGridView.Visible = false;
                            isAllDataValid = false;
                            MessageBox.Show( "Jump meter setup has not been successfully completed."
                                + "\n Setup must be complete before distances can be calculated."
                                );
                        }
                    } else {
                        myNumJudges = 0;
                        jumpRecapDataGridView.Visible = true;
                        ScoreFeetRecap.ReadOnly = false;
                        ScoreMetersRecap.ReadOnly = false;
                        ScoreTriangleRecap.Visible = false;

                        Meter6Recap.Visible = false;
                        Meter5Recap.Visible = false;
                        Meter4Recap.Visible = false;
                        Meter3Recap.Visible = false;
                        Meter2Recap.Visible = false;
                        Meter1Recap.Visible = false;
                        jumpRecapDataGridView.Width = 868;
                        jumpRecapDataGridView.Width += -285;
                        JumpCalcMetersCB.Checked = false;
                        JumpCalcVideoCB.Checked = false;
                    }
                }
                myTourProperties.JumpEntryNumJudges = myNumJudges.ToString();

            }
        }

        private void ScoreEntry_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.JumpEntry_Width > 0) {
                this.Width = Properties.Settings.Default.JumpEntry_Width;
            }
            if (Properties.Settings.Default.JumpEntry_Height > 0) {
                this.Height = Properties.Settings.Default.JumpEntry_Height;
            }
            if (Properties.Settings.Default.JumpEntry_Location.X > 0
                && Properties.Settings.Default.JumpEntry_Location.Y > 0) {
                this.Location = Properties.Settings.Default.JumpEntry_Location;
            }
            myTourProperties = TourProperties.Instance;
            mySortCommand = myTourProperties.RunningOrderSortJump;
            int curDelim = mySortCommand.IndexOf( "AgeGroup" );
            if (curDelim < 0) {
            } else if (curDelim > 0) {
                mySortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
                myTourProperties.RunningOrderSortJump = mySortCommand;
            } else {
                mySortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
                myTourProperties.RunningOrderSortJump = mySortCommand;
            }

            ResizeNarrow.Visible = false;
            ResizeNarrow.Enabled = false;
            ResizeWide.Visible = true;
            ResizeWide.Enabled = true;
            ResizeNarrow.Location = ResizeWide.Location;

            EventRunInfoLabel.Text = "   Event Start:\n" + "   Event Delay:\n" + "Skiers, Passes:";
            EventRunInfoData.Text = "";
            EventRunPerfLabel.Text = "Event Duration:\n" + "Mins Per Skier:\n" + " Mins Per Pass:";
            EventRunPerfData.Text = "";
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

            eventGroupSelect = new EventGroupSelect();
            rerideReasonDialogForm = new RerideReason();
            skierDoneReasonDialogForm = new SkierDoneReason();

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

                        if ( (byte)myTourRow["JumpRounds"] > 0 ) {
                            mySanctionYear = Convert.ToInt16( mySanctionNum.Substring( 0, 2 ) );

                            //Load round selection list based on number of rounds specified for the tournament
                            roundSelect.SelectList_Load( myTourRow["JumpRounds"].ToString(), roundSelect_Click );
                            roundActiveSelect.SelectList_LoadHorztl( myTourRow["JumpRounds"].ToString(), roundActiveSelect_Click );
                            roundActiveSelect.RoundValue = "1";
                            roundSelect.RoundValue = "1";

                            //Instantiate object for checking for records
                            myCheckEventRecord = new CheckEventRecord(myTourRow);

                            //Load jump speed selection list
                            JumpSpeedSelect.SelectList_Load( JumpSpeedSelect_Change );

                            //Load ramp height selection list
                            RampHeightSelect.SelectList_Load( RampHeightSelect_Change );

                            mySkierClassList = new ListSkierClass();
                            mySkierClassList.ListSkierClassLoad();
                            scoreEventClass.DataSource = mySkierClassList.DropdownList;
                            scoreEventClass.DisplayMember = "ItemName";
                            scoreEventClass.ValueMember = "ItemValue";

                            myRuleType = ((String)myTourRow["Rules"]).ToUpper();
                            myTourClass = myTourRow["Class"].ToString().ToUpper();
                            myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                            myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];
                            myAgeDivList = new AgeGroupDropdownList( myTourRow );

                            //Setup for meter input if using meters to calculate distances
                            NumJudges = Convert.ToInt16( myTourProperties.JumpEntryNumJudges);
                            if ( NumJudges > 0 ) {
                                JumpCalcMetersCB.Checked = true;
                                JumpCalcVideoCB.Checked = false;
                            } else {
                                JumpCalcMetersCB.Checked = false;
                                JumpCalcVideoCB.Checked = true;
                            }

                            //Retrieve boat times
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "SELECT ListCode, CodeValue, MinValue, MaxValue, CodeDesc FROM CodeValueList WHERE ListName = 'JumpBoatTime3Seg' ORDER BY SortSeq" );
                            myTimesDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

                            //Retrieve maximum speeds by age group
                            getMaxSpeedData();

                            //Retrieve minimum speeds by class
                            getMinSpeedData();

                            //Retrieve maximum ramp heights by age group
                            getMaxRampData();

                            //Retrieve list of divisions and scores that require 3 segment validation for L and R tournaments
                            try {
                                DataTable curTourClassDataTable = mySkierClassList.getTourClassList();
                                myClassRow = curTourClassDataTable.Select( "ListCode = '" + myTourClass + "'" )[0];
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    getJump3TimesDivData();
                                } else {
                                    myJump3TimesDivDataTable = null;
                                }

								if ( (Decimal) myClassRow["ListCodeNum"] >= (Decimal) myClassERow["ListCodeNum"] ) {
									myCheckOfficials = new CheckOfficials();
								} else {
									myCheckOfficials = null;
								}

							} catch {
                                myJump3TimesDivDataTable = null;
                            }


							//Get list of approved tow boats
							getApprovedTowboats();
							approvedBoatSelectGroupBox.Visible = false;
                            listApprovedBoatsDataGridView.Visible = false;

                            //Retrieve and load tournament event entries
                            loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

                            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                                LiveWebLabel.Visible = true;
                            } else {
                                LiveWebLabel.Visible = false;
                            }
                        } else {
                            MessageBox.Show( "The jump event is not defined for the active tournament" );
                        }
                    } else {
                        MessageBox.Show( "The active tournament is not properly defined.  You must select from the Administration menu Tournament List option" );
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void ScoreEntry_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.JumpEntry_Width = this.Size.Width;
                Properties.Settings.Default.JumpEntry_Height = this.Size.Height;
                Properties.Settings.Default.JumpEntry_Location = this.Location;
            }
            if ( myPassRunCount > 0 ) {
                if ( StartTimerButton.Visible ) {
                    StartTimerButton_Click( null, null );
                }
                int curHours, curMins, curSeconds, curTimeDiff;
                String curMethodName = "Jump:ScoreEntry:FormClosed";
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
            if (isDataModified) {
                saveScore();
            }
            e.Cancel = false;
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
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

        private void navSaveItem_Click(object sender, EventArgs e) {
            roundActiveSelect.Focus();
            jumpRecapDataGridView.EndEdit();
            jumpRecapDataGridView.Focus();
            try {
                if ( saveScore() ) {
                    isDataModified = false;
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveScore() {
            String curMethodName = "Jump:ScoreEntrySeg3:saveScore";
            bool curReturn = true;
            int rowsProc = 0;
            if ( StartTimerButton.Visible ) {
                StartTimerButton_Click( null, null );
            }
            if (myRecapRow == null || myRecapRow.Index < 0 || TourEventRegDataGridView.Rows.Count == 0 ) {
                isDataModified = false;
            } else {
                try {
                    int curIdx = myRecapRow.Index;
                    //myRecapRow = jumpRecapDataGridView.Rows[curIdx];
                    String curUpdateStatus = (String)myRecapRow.Cells["Updated"].Value;
                    if (curUpdateStatus.ToUpper().Equals( "Y" )) {
                        if (!(roundActiveSelect.RoundValue.Equals( roundSelect.RoundValue ))) {
                            DialogResult msgResp = MessageBox.Show( "Skier score being updated in round other than active round!"
                                + "\nDo you want to continue with update?"
                                , "Update Warning"
                                , MessageBoxButtons.YesNo
                                , MessageBoxIcon.Warning
                                , MessageBoxDefaultButton.Button1 );
                            if (msgResp == DialogResult.No) {
                                return false;
                            }
                        }

                        String curScoreNote = "", curTourBoat = "";
                        String curSanctionId = (String)myRecapRow.Cells["SanctionIdRecap"].Value;
                        String curMemberId = (String)myRecapRow.Cells["MemberIdRecap"].Value;
                        String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;
                        String curTeamCode = TeamCodeTextBox.Text;
                        String curEventClass = (String)scoreEventClass.SelectedValue;
                        byte curRound = Convert.ToByte( (String)myRecapRow.Cells["RoundRecap"].Value );

                        #region Update skier total score for round
                        Int64 curScorePK = 0;

                        getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
                        if (myScoreDataTable.Rows.Count > 0) {
                            myScoreRow = myScoreDataTable.Rows[0];
                            curScorePK = (Int64)myScoreRow["PK"];
                        } else {
                            myScoreRow = null;
                            curScorePK = -1;
                        }

                        String curScoreFeet, curScoreMeters, curNopsScore, curBoatSpeed, curRampHeight;
                        try {
                            curScoreFeet = scoreFeetTextBox.Text;
                            if (curScoreFeet.Length == 0) curScoreFeet = "Null";
                        } catch {
                            curScoreFeet = "Null";
                        }
                        try {
                            curScoreMeters = scoreMetersTextBox.Text;
                            if (curScoreMeters.Length == 0) curScoreMeters = "Null";
                        } catch {
                            curScoreMeters = "Null";
                        }
                        try {
                            curNopsScore = nopsScoreTextBox.Text;
                            if (curNopsScore.Length == 0) curNopsScore = "Null";
                        } catch {
                            curNopsScore = "Null";
                        }
                        try {
                            curRampHeight = RampHeightSelect.CurrentValue.ToString( "0.0" );
                            if (curRampHeight.Length == 0) curRampHeight = "Null";
                        } catch {
                            curRampHeight = "Null";
                        }
                        try {
                            curBoatSpeed = BoatSpeedTextBox.Text;
                            if (curBoatSpeed.Length == 0) curBoatSpeed = "Null";
                        } catch {
                            curBoatSpeed = "Null";
                        }

						curTourBoat = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

						try {
                            curScoreNote = noteTextBox.Text;
                            curScoreNote = curScoreNote.Replace( "'", "''" );
                        } catch {
                            curScoreNote = "";
                        }

                        String curStatus = "TBD";
                        try {
                            String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
                            if (curValue.Equals( "4-Done" )) curStatus = "Complete";
                            if (curValue.Equals( "2-InProg" )) curStatus = "InProg";
                            if (curValue.Equals( "3-Error" )) curStatus = "Error";
                            if (curValue.Equals( "1-TBD" )) curStatus = "TBD";
                        } catch {
                            curStatus = "TBD";
                            MessageBox.Show( "Exception detected when checking status" );
                        }

                        if ((!(curBoatSpeed.ToLower().Equals( "null" )))
                            && !(curScoreFeet.ToLower().Equals( "null" ))
                            && !(curScoreMeters.ToLower().Equals( "null" ))
                            ) {
                            try {
                                StringBuilder curSqlStmt = new StringBuilder( "" );
                                if (curScorePK > 0) {
                                    curSqlStmt.Append( "Update JumpScore Set " );
                                    curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                    curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                                    curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                                    curSqlStmt.Append( ", Round = " + curRound );
                                    curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );
                                    curSqlStmt.Append( ", BoatSpeed = " + curBoatSpeed );
                                    curSqlStmt.Append( ", RampHeight = " + curRampHeight );
                                    curSqlStmt.Append( ", ScoreFeet = " + curScoreFeet );
                                    curSqlStmt.Append( ", ScoreMeters = " + curScoreMeters );
                                    curSqlStmt.Append( ", NopsScore = " + curNopsScore );
                                    curSqlStmt.Append( ", Boat = '" + curTourBoat + "'" );
                                    curSqlStmt.Append( ", Status = '" + curStatus + "'" );
                                    curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                                    curSqlStmt.Append( ", Note = '" + curScoreNote + "'" );
                                    curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                } else {
                                    curSqlStmt.Append( "Insert JumpScore ( " );
                                    curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass, BoatSpeed, RampHeight" );
                                    curSqlStmt.Append( ", ScoreFeet, ScoreMeters, NopsScore, Status, Boat, LastUpdateDate, Note" );
                                    curSqlStmt.Append( ") Values ( " );
                                    curSqlStmt.Append( "'" + curSanctionId + "'" );
                                    curSqlStmt.Append( ", '" + curMemberId + "'" );
                                    curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                                    curSqlStmt.Append( ", " + curRound );
                                    curSqlStmt.Append( ", '" + curEventClass + "'" );
                                    curSqlStmt.Append( ", " + curBoatSpeed );
                                    curSqlStmt.Append( ", " + curRampHeight );
                                    curSqlStmt.Append( ", " + curScoreFeet );
                                    curSqlStmt.Append( ", " + curScoreMeters );
                                    curSqlStmt.Append( ", " + curNopsScore );
                                    curSqlStmt.Append( ", '" + curTourBoat + "'" );
                                    curSqlStmt.Append( ", '" + curStatus + "'" );
                                    curSqlStmt.Append( ", GETDATE()" );
                                    curSqlStmt.Append( ", '" + curScoreNote + "'" );
                                    curSqlStmt.Append( ")" );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                }
                                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                if (rowsProc > 0) {
                                    getSkierScoreByRound( curMemberId, curAgeGroup, curRound );
                                    if (myScoreDataTable.Rows.Count > 0) {
                                        myScoreRow = myScoreDataTable.Rows[0];
                                    }
                                }

                                if (myTourRules.ToLower().Equals( "iwwf" )) {
                                    TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScoreMeters;
                                } else {
                                    TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = curScoreFeet;
                                }
                        #endregion

                                #region Update skier score for current pass
                                rowsProc = 0;
                                String curMeter1, curMeter2, curMeter3, curMeter4, curMeter5, curMeter6, curScoreTriangle;
                                String curBoatSplitTime, curBoatSplitTime2, curBoatEndTime, curReride, curScoreProt, curTimeInTol;
                                String curRerideReason, curResults, curReturnToBase, curNote;
                                String curSplit52TimeTol, curSplit82TimeTol, curSplit41TimeTol;
                                Int64 curRecapPK = Convert.ToInt64( myRecapRow.Cells["PKRecap"].Value );
                                Int16 curPassNum = Convert.ToInt16( (String)myRecapRow.Cells["PassNumRecap"].Value );
                                try {
                                    curMeter1 = (String)myRecapRow.Cells["Meter1Recap"].Value;
                                    if (curMeter1.Length == 0) curMeter1 = "Null";
                                } catch {
                                    curMeter1 = "Null";
                                }
                                try {
                                    curMeter2 = (String)myRecapRow.Cells["Meter2Recap"].Value;
                                    if (curMeter2.Length == 0) curMeter2 = "Null";
                                } catch {
                                    curMeter2 = "Null";
                                }
                                try {
                                    curMeter3 = (String)myRecapRow.Cells["Meter3Recap"].Value;
                                    if (curMeter3.Length == 0) curMeter3 = "Null";
                                } catch {
                                    curMeter3 = "Null";
                                }
                                try {
                                    curMeter4 = (String)myRecapRow.Cells["Meter4Recap"].Value;
                                    if (curMeter4.Length == 0) curMeter4 = "Null";
                                } catch {
                                    curMeter4 = "Null";
                                }
                                try {
                                    curMeter5 = (String)myRecapRow.Cells["Meter5Recap"].Value;
                                    if (curMeter5.Length == 0) curMeter5 = "Null";
                                } catch {
                                    curMeter5 = "Null";
                                }
                                try {
                                    curMeter6 = (String)myRecapRow.Cells["Meter6Recap"].Value;
                                    if (curMeter6.Length == 0) curMeter6 = "Null";
                                } catch {
                                    curMeter6 = "Null";
                                }
                                try {
                                    curScoreTriangle = (String)myRecapRow.Cells["ScoreTriangleRecap"].Value;
                                    if (curScoreTriangle.Length == 0) curScoreTriangle = "Null";
                                } catch {
                                    curScoreTriangle = "Null";
                                }
                                try {
                                    curBoatSpeed = (String)myRecapRow.Cells["BoatSpeedRecap"].Value;
                                } catch {
                                    curBoatSpeed = "Null";
                                }
                                try {
                                    curScoreFeet = (String)myRecapRow.Cells["ScoreFeetRecap"].Value;
                                    if (curScoreFeet.Length == 0) curScoreFeet = "Null";
                                } catch {
                                    curScoreFeet = "Null";
                                }
                                try {
                                    curScoreMeters = (String)myRecapRow.Cells["ScoreMetersRecap"].Value;
                                    if (curScoreMeters.Length == 0) curScoreMeters = "Null";
                                } catch {
                                    curScoreMeters = "Null";
                                }
                                try {
                                    curReride = (String)myRecapRow.Cells["RerideRecap"].Value;
                                } catch {
                                    curReride = "N";
                                }
                                try {
                                    curBoatSplitTime = (String)myRecapRow.Cells["BoatSplitTimeRecap"].Value;
                                    if (curBoatSplitTime.Length == 0) curBoatSplitTime = "Null";
                                } catch {
                                    curBoatSplitTime = "Null";
                                }
                                try {
                                    curBoatSplitTime2 = (String)myRecapRow.Cells["BoatSplitTime2Recap"].Value;
                                    if (curBoatSplitTime2.Length == 0) curBoatSplitTime2 = "Null";
                                } catch {
                                    curBoatSplitTime2 = "Null";
                                }
                                try {
                                    curBoatEndTime = (String)myRecapRow.Cells["BoatEndTimeRecap"].Value;
                                    if (curBoatEndTime.Length == 0) curBoatEndTime = "Null";
                                } catch {
                                    curBoatEndTime = "Null";
                                }
                                try {
                                    curSplit52TimeTol = (String)myRecapRow.Cells["Split52TimeTolRecap"].Value;
                                    if (curSplit52TimeTol.Length == 0) curSplit52TimeTol = "0";
                                } catch {
                                    curSplit52TimeTol = "0";
                                }
                                try {
                                    curSplit82TimeTol = (String)myRecapRow.Cells["Split82TimeTolRecap"].Value;
                                    if (curSplit82TimeTol.Length == 0) curSplit82TimeTol = "0";
                                } catch {
                                    curSplit82TimeTol = "0";
                                }
                                try {
                                    curSplit41TimeTol = (String)myRecapRow.Cells["Split41TimeTolRecap"].Value;
                                    if (curSplit41TimeTol.Length == 0) curSplit41TimeTol = "0";
                                } catch {
                                    curSplit41TimeTol = "0";
                                }
                                try {
                                    curTimeInTol = (String)myRecapRow.Cells["TimeInTolRecap"].Value;
                                } catch {
                                    curTimeInTol = "N";
                                }
                                try {
                                    curScoreProt = (String)myRecapRow.Cells["ScoreProtRecap"].Value;
                                } catch {
                                    curScoreProt = "N";
                                }
                                try {
                                    curReturnToBase = (String)myRecapRow.Cells["ReturnToBaseRecap"].Value;
                                } catch {
                                    curReturnToBase = "N";
                                }
                                try {
                                    curResults = (String)myRecapRow.Cells["ResultsRecap"].Value;
                                } catch {
                                    curResults = "";
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
                                if (curRecapPK > 0) {
                                    curSqlStmt.Append( "Update JumpRecap Set " );
                                    curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                    curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                                    curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                                    curSqlStmt.Append( ", Round = " + curRound );
                                    curSqlStmt.Append( ", PassNum = " + curPassNum );
                                    curSqlStmt.Append( ", Reride = '" + curReride + "'" );
                                    curSqlStmt.Append( ", BoatSpeed = " + curBoatSpeed );
                                    curSqlStmt.Append( ", RampHeight = " + curRampHeight );
                                    curSqlStmt.Append( ", Meter1 = " + curMeter1 );
                                    curSqlStmt.Append( ", Meter2 = " + curMeter2 );
                                    curSqlStmt.Append( ", Meter3 = " + curMeter3 );
                                    curSqlStmt.Append( ", Meter4 = " + curMeter4 );
                                    curSqlStmt.Append( ", Meter5 = " + curMeter5 );
                                    curSqlStmt.Append( ", Meter6 = " + curMeter6 );
                                    curSqlStmt.Append( ", ScoreTriangle = " + curScoreTriangle );
                                    curSqlStmt.Append( ", ScoreFeet = " + curScoreFeet );
                                    curSqlStmt.Append( ", ScoreMeters = " + curScoreMeters );
                                    curSqlStmt.Append( ", BoatSplitTime = " + curBoatSplitTime );
                                    curSqlStmt.Append( ", BoatSplitTime2 = " + curBoatSplitTime2 );
                                    curSqlStmt.Append( ", BoatEndTime = " + curBoatEndTime );
                                    curSqlStmt.Append( ", TimeInTol = '" + curTimeInTol + "'" );
                                    curSqlStmt.Append( ", ScoreProt = '" + curScoreProt + "'" );
                                    curSqlStmt.Append( ", ReturnToBase = '" + curReturnToBase + "'" );
                                    curSqlStmt.Append( ", Split52TimeTol = " + curSplit52TimeTol );
                                    curSqlStmt.Append( ", Split82TimeTol = " + curSplit82TimeTol );
                                    curSqlStmt.Append( ", Split41TimeTol = " + curSplit41TimeTol );
                                    curSqlStmt.Append( ", Results = '" + curResults + "'" );
                                    curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                                    curSqlStmt.Append( ", RerideReason = '" + curRerideReason + "'" );
                                    curSqlStmt.Append( ", Note = '" + curNote + "'" );
                                    curSqlStmt.Append( " Where PK = " + curRecapPK.ToString() );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                } else {
                                    curSqlStmt.Append( "Insert JumpRecap ( " );
                                    curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, PassNum" );
                                    curSqlStmt.Append( ", BoatSpeed, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6, ScoreTriangle" );
                                    curSqlStmt.Append( ", ScoreFeet, ScoreMeters, BoatSplitTime, BoatSplitTime2, BoatEndTime, Split52TimeTol, Split82TimeTol, Split41TimeTol" );
                                    curSqlStmt.Append( ", Reride, TimeInTol, ScoreProt, ReturnToBase, Results, LastUpdateDate, RerideReason, Note" );
                                    curSqlStmt.Append( ") Values (" );
                                    curSqlStmt.Append( " '" + curSanctionId + "'" );
                                    curSqlStmt.Append( ", '" + curMemberId + "'" );
                                    curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                                    curSqlStmt.Append( ", " + curRound );
                                    curSqlStmt.Append( ", " + curPassNum );
                                    curSqlStmt.Append( ", " + curBoatSpeed );
                                    curSqlStmt.Append( ", " + curRampHeight );
                                    curSqlStmt.Append( ", " + curMeter1 );
                                    curSqlStmt.Append( ", " + curMeter2 );
                                    curSqlStmt.Append( ", " + curMeter3 );
                                    curSqlStmt.Append( ", " + curMeter4 );
                                    curSqlStmt.Append( ", " + curMeter5 );
                                    curSqlStmt.Append( ", " + curMeter6 );
                                    curSqlStmt.Append( ", " + curScoreTriangle );
                                    curSqlStmt.Append( ", " + curScoreFeet );
                                    curSqlStmt.Append( ", " + curScoreMeters );
                                    curSqlStmt.Append( ", " + curBoatSplitTime );
                                    curSqlStmt.Append( ", " + curBoatSplitTime2 );
                                    curSqlStmt.Append( ", " + curBoatEndTime );
                                    curSqlStmt.Append( ", " + curSplit52TimeTol );
                                    curSqlStmt.Append( ", " + curSplit82TimeTol );
                                    curSqlStmt.Append( ", " + curSplit41TimeTol );
                                    curSqlStmt.Append( ", '" + curReride + "'" );
                                    curSqlStmt.Append( ", '" + curTimeInTol + "'" );
                                    curSqlStmt.Append( ", '" + curScoreProt + "'" );
                                    curSqlStmt.Append( ", '" + curReturnToBase + "'" );
                                    curSqlStmt.Append( ", '" + curResults + "'" );
                                    curSqlStmt.Append( ", getdate()" );
                                    curSqlStmt.Append( ", '" + curRerideReason + "'" );
                                    curSqlStmt.Append( ", '" + curNote + "'" );
                                    curSqlStmt.Append( ")" );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                }
                                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                if (rowsProc > 0) {
                                    myRecapRow.Cells["Updated"].Value = "N";
                                    refreshScoreSummaryWindow();
                                    if (curRecapPK < 0) {
                                        curSqlStmt = new StringBuilder( "" );
                                        curSqlStmt.Append( "Select max(PK) as MaxPK From JumpRecap" );
                                        DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                                        if (curDataTable.Rows.Count > 0) {
                                            Int64 curPK = (Int64)curDataTable.Rows[0]["MaxPK"];
                                            myRecapRow.Cells["PKRecap"].Value = curPK.ToString();
                                        }
                                    }
                                    int curSeconds = 0, curMins = 0, curHours, curTimeDiff = 0;
                                    if (myPassRunCount > 1) {
                                        try {
                                            curTimeDiff = Convert.ToInt32( ((TimeSpan)DateTime.Now.Subtract( myEventStartTime )).TotalSeconds ) - myEventDelaySeconds;
                                        } catch {
                                            myEventStartTime = DateTime.Now;
                                            EventRunInfoData.Text = "";
                                            EventRunPerfData.Text = "";
                                            curTimeDiff = Convert.ToInt32( ((TimeSpan)DateTime.Now.Subtract( myEventStartTime )).TotalSeconds ) - myEventDelaySeconds;
                                        }
                                        curHours = Math.DivRem( curTimeDiff, 3600, out curSeconds );
                                        curMins = Math.DivRem( curSeconds, 60, out curSeconds );
                                        String curEventDuration = curHours.ToString( "00" ) + ":" + curMins.ToString( "00" );
                                        curMins = Math.DivRem( (curTimeDiff / mySkierRunCount), 60, out curSeconds );
                                        String curMinsPerSkier = curMins.ToString( "00" ) + ":" + curSeconds.ToString( "00" );
                                        curMins = Math.DivRem( (curTimeDiff / myPassRunCount), 60, out curSeconds );
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
                                        + ", " + myPassRunCount.ToString( "##0" );
                                }
                                #endregion

                                transmitExternalScoreboard( curSanctionId, curMemberId, curAgeGroup, curTeamCode, curRound, curPassNum, curStatus, curResults, curScoreFeet, curScoreMeters );

                                #region Check to see if score is equal to or great than divisions current record score
                                String curCheckRecordMsg = myCheckEventRecord.checkRecordJump( curAgeGroup, scoreFeetTextBox.Text, scoreMetersTextBox.Text, (byte) myScoreRow["SkiYearAge"], (string) myScoreRow["Gender"]);
                                if ( curCheckRecordMsg == null) curCheckRecordMsg = "";
                                if (curCheckRecordMsg.Length > 1) {
                                    MessageBox.Show( curCheckRecordMsg );
                                }
                                #endregion
                            } catch (Exception excp) {
                                curReturn = false;
                                String curMsg = ":Error attempting to update skier score \n" + excp.Message;
                                MessageBox.Show( curMsg );
                                Log.WriteFile( curMethodName + curMsg );
                            }
                        }
                    }

                    isDataModified = false;
                } catch (Exception excp) {
                    curReturn = false;
                    String curMsg = ":Error attempting to update skier score \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    MessageBox.Show(excp.StackTrace);
                    Log.WriteFile( curMethodName + curMsg );
                }
            }

            return curReturn;
        }

        private void transmitExternalScoreboard(String curSanctionId, String curMemberId, String curAgeGroup
            , String curTeamCode, byte curRound, short curPassNum, String curStatus, String curResults, String curScoreFeet, String curScoreMeters) {
            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                ExportLiveWeb.exportCurrentSkierJump( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
            }
            if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
                StringBuilder curTwitterMessage = new StringBuilder( "" );
                if (ExportLiveTwitter.TwitterReportByValue.Equals( "Pass" )) {
                    curTwitterMessage.Append( curSanctionId + " " + curAgeGroup + " " + curSkierName );
                    if (curTeamCode.Length > 0) {
                        curTwitterMessage.Append( " Team: " + curTeamCode );
                    }
                    curTwitterMessage.Append( " Jump # " + curPassNum.ToString( "#0" ) );
                    curTwitterMessage.Append( " Results: " + curResults );
                    curTwitterMessage.Append( " Dist: " + curScoreFeet + "Ft, " + curScoreMeters + "M" );
                    curTwitterMessage.Append( " Speed: " + JumpSpeedSelect.CurrentValueDesc + " Ramp: " + RampHeightSelect.CurrentValue.ToString( "0.0" ) );
                    curTwitterMessage.Append( " UNOFFICIAL " + DateTime.Now );
                    ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                } else {
                    if (curStatus.ToLower().Equals( "complete" )) {
                        curTwitterMessage.Append( curSanctionId + " " + curAgeGroup + " " + curSkierName );
                        if (curTeamCode.Length > 0) {
                            curTwitterMessage.Append( " Team: " + curTeamCode );
                        }
                        curTwitterMessage.Append( " Jump # " + curPassNum.ToString( "#0" ) );
                        curTwitterMessage.Append( " Results: " + curResults );
                        curTwitterMessage.Append( " Dist: " + curScoreFeet + "Ft, " + curScoreMeters + "M" );
                        curTwitterMessage.Append( " Speed: " + JumpSpeedSelect.CurrentValueDesc + " Ramp: " + RampHeightSelect.CurrentValue.ToString( "0.0" ) );
                        curTwitterMessage.Append( " UNOFFICIAL " + DateTime.Now );
                        ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                    }
                }
            }
            if (ExportLiveScoreboard.ScoreboardLocation.Length > 1) {
                String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
                ExportLiveScoreboard.exportCurrentSkierJump( curSanctionId, curMemberId, curAgeGroup, curTeamCode, curRound,
                    curSkierName, JumpSpeedSelect.CurrentValueDesc, RampHeightSelect.CurrentValueDesc,
                    curPassNum.ToString( "#0" ), curResults, scoreFeetTextBox.Text, scoreMetersTextBox.Text, curScoreFeet, curScoreMeters, curStatus );
            }
        }

        private void JumpCalcVideoCB_CheckedChanged( object sender, EventArgs e ) {
            CheckBox curControl = (CheckBox)sender;
            if ( curControl.Checked ) {
                NumJudges = 0;
                JumpCalcMetersCB.Checked = false;
            }
        }

        private void JumpCalcMetersCB_CheckedChanged( object sender, EventArgs e ) {
            CheckBox curControl = (CheckBox)sender;
            if ( curControl.Checked ) {
                //Determine required number of judges for event
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT ListCode, CodeValue, MaxValue, MinValue, CodeDesc FROM CodeValueList WHERE ListName = 'TrickJudgesNum' ORDER BY SortSeq" );
                DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if ( curDataTable.Rows.Count > 0 ) {
                    int curNumJudges = Convert.ToInt16( (Decimal)curDataTable.Rows[0]["MaxValue"] );
                    //Override to 3 judges for now because 6 meter calculations are not yet supported
                    if ( curNumJudges > 3 ) {
                        MessageBox.Show( "Distance calculations for more than 3 judges is not currently supported. \n Defaulting to 3 meters." );
                    }
                    NumJudges = 3;
                } else {
                    NumJudges = 3;
                }
            }
        }

        private void refreshScoreSummaryWindow() {
            // Check for open instance of selected form
            //SystemMain curMdiWindow = (SystemMain)this.Parent.Parent;
            //for ( int idx = 0; idx < curMdiWindow.MdiChildren.Length; idx++ ) {
            //    Form curForm = (Form)( curMdiWindow.MdiChildren[idx] );
            //    if ( curForm.Name.Equals( "JumpSummary" ) ) {
            //        JumpSummary curSummaryWindow = (JumpSummary)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "JumpSummaryTeam" ) ) {
            //        JumpSummaryTeam curSummaryWindow = (JumpSummaryTeam)curForm;
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

        private void scoreEntryInprogress() {
            RampHeightSelect.Enabled = true;
            roundSelect.Enabled = true;
            addPassButton.Enabled = true;
        }

        private void scoreEntryBegin() {
            RampHeightSelect.Enabled = true;
            roundSelect.Enabled = true;
            addPassButton.Enabled = true;
        }

        private void deletePassButton_Click(object sender, EventArgs e) {
            String curMethodName = "Jump:ScoreEntrySeg3:deletePassButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            int rowsProc = 0;

            if ( jumpRecapDataGridView.Rows.Count > 0 ) {
                try {
                    int curRowIdx = jumpRecapDataGridView.Rows.Count - 1;
                    myRecapRow = jumpRecapDataGridView.Rows[curRowIdx];
                    Int64 curRecapPK = Convert.ToInt64( (String)myRecapRow.Cells["PKRecap"].Value );
                    if ( curRecapPK > 0 ) {
                        try {
                            curSqlStmt = new StringBuilder( "Delete JumpRecap Where PK = " + curRecapPK.ToString() );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        } catch ( Exception excp ) {
                            curMsg = curMethodName + "Error deleting jump pass \n" + excp.Message;
                            MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                        if ( rowsProc > 0 ) {
                            jumpRecapDataGridView.Rows.RemoveAt( curRowIdx );
                            if ( curRowIdx > 0 ) {
                                curRowIdx--;
                                jumpRecapDataGridView.Rows[curRowIdx].Cells["Updated"].Value = "Y";
                                myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[curRowIdx];
                                setRecapCellsForEdit();
                                CalcSkierScore();
                                if ( checkRoundContinue() ) {
                                    scoreEntryInprogress();
                                } else {
                                    addPassButton.Enabled = false;
                                }
                                saveScore();
                            } else {
                                try {
                                    myRecapRow = null;
                                    Int64 curScorePK = (Int64)myScoreRow["PK"];
                                    String curMemberId = (String)myScoreRow["MemberId"];
                                    String curAgeGroup = (String)myScoreRow["AgeGroup"];
                                    String curTeamCode = TeamCodeTextBox.Text;
                                    byte curRound = (byte)myScoreRow["Round"];
                                    short curPassNum = 0;
                                    curSqlStmt = new StringBuilder( "Delete JumpScore Where PK = " + curScorePK.ToString() );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                                    transmitExternalScoreboard( mySanctionNum, curMemberId, curAgeGroup, curTeamCode, curRound, curPassNum, "TBD", "", "", "" );

                                } catch ( Exception excp ) {
                                    curMsg = curMethodName + "Error deleting skier score \n" + excp.Message;
                                    MessageBox.Show( curMsg );
                                    Log.WriteFile( curMethodName + curMsg );
                                }
                                myScoreRow = null;
                                scoreFeetTextBox.Text = "";
                                scoreMetersTextBox.Text = "";
                                nopsScoreTextBox.Text = "";
                                noteTextBox.Text = "";
                                RampHeightTextBox.Text = "";
                                BoatSpeedTextBox.Text = "";
                                TeamCodeTextBox.Text = "";
                                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["score"].Value = "";

                                JumpSpeedSelect.CurrentValue = myMaxSpeed;
                                scoreEntryBegin();

                                setEventRegRowStatus( "1-TBD" );
                                refreshScoreSummaryWindow();
                                scoreEntryBegin();
                            }
                        }
                    } else {
                        jumpRecapDataGridView.Rows.RemoveAt( curRowIdx );
                        if ( curRowIdx > 0 ) {
                            curRowIdx--;
                            myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[curRowIdx];
                            setRecapCellsForEdit();
                            CalcSkierScore();
                            if ( checkRoundContinue() ) {
                                scoreEntryInprogress();
                            } else {
                                addPassButton.Enabled = false;
                            }
                            saveScore();
                        } else {
                            if (myScoreRow != null) {
                                try {
                                    myRecapRow = null;
                                    Int64 curScorePK = (Int64)myScoreRow["PK"];
                                    curSqlStmt = new StringBuilder( "Delete JumpScore Where PK = " + curScorePK.ToString() );
                                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                } catch (Exception excp) {
                                    curMsg = curMethodName + "Error deleting skier score \n" + excp.Message;
                                    MessageBox.Show( curMsg );
                                    Log.WriteFile( curMethodName + curMsg );
                                }
                            }
                            myScoreRow = null;
                            scoreFeetTextBox.Text = "";
                            scoreMetersTextBox.Text = "";
                            nopsScoreTextBox.Text = "";
                            noteTextBox.Text = "";
                            RampHeightTextBox.Text = "";
                            TeamCodeTextBox.Text = "";
                            BoatSpeedTextBox.Text = "";
                            //RampHeightSelect.CurrentValue = curMaxRamp;
                            JumpSpeedSelect.CurrentValue = myMaxSpeed;
                            //JumpSpeedSelect_Change( null, null );
                            scoreEntryBegin();

                            setEventRegRowStatus( "1-TBD" );
                            refreshScoreSummaryWindow();
                        }
                    }
                } catch ( Exception excp ) {
                    curMsg = curMethodName + "Error deleting save changes  \n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
                jumpRecapDataGridView.Select();
            }
        }

        private void setRecapCellsForEdit() {
            foreach ( DataGridViewCell curCell in myRecapRow.Cells ) {
                if ( myNumJudges == 0 ) {
                    if ( curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
                        || curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
                        || curCell.OwningColumn.Name.StartsWith( "ScoreFeet" )
                        || curCell.OwningColumn.Name.StartsWith( "ScoreMeters" )
                        || curCell.OwningColumn.Name.StartsWith( "Note" )
                        || curCell.OwningColumn.Name.StartsWith( "Updated" )
                        || curCell.OwningColumn.Name.StartsWith( "Results" )
                        || curCell.OwningColumn.Name.StartsWith( "Reride" )
                        || curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
                        || curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
                        || curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
                        ) {
                        curCell.ReadOnly = false;
                    }
                } else if ( myNumJudges == 3 ) {
                    if ( curCell.OwningColumn.Name.StartsWith( "Meter1" )
                        || curCell.OwningColumn.Name.StartsWith( "Meter2" )
                        || curCell.OwningColumn.Name.StartsWith( "Meter3" )
                        || curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
                        || curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
                        || curCell.OwningColumn.Name.StartsWith( "Note" )
                        || curCell.OwningColumn.Name.StartsWith( "Updated" )
                        || curCell.OwningColumn.Name.StartsWith( "Results" )
                        || curCell.OwningColumn.Name.StartsWith( "Reride" )
                        || curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
                        || curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
                        || curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
                        ) {
                        curCell.ReadOnly = false;
                    }
                } else if ( myNumJudges == 6 ) {
                    if ( curCell.OwningColumn.Name.StartsWith( "Meter" )
                        || curCell.OwningColumn.Name.StartsWith( "BoatSplit" )
                        || curCell.OwningColumn.Name.StartsWith( "BoatEnd" )
                        || curCell.OwningColumn.Name.StartsWith( "Note" )
                        || curCell.OwningColumn.Name.StartsWith( "Updated" )
                        || curCell.OwningColumn.Name.StartsWith( "Results" )
                        || curCell.OwningColumn.Name.StartsWith( "Reride" )
                        || curCell.OwningColumn.Name.StartsWith( "ScoreProt" )
                        || curCell.OwningColumn.Name.StartsWith( "ReturnToBase" )
                        || curCell.OwningColumn.Name.StartsWith( "TimeInTol" )
                        ) {
                        curCell.ReadOnly = false;
                    }
                }
            }
        }

        private void addPassButton_Click(object sender, EventArgs e) {
            if ( myBoatListIdx == 0 ) {
                MessageBox.Show( "Boat selection has not been made.  This is required before skier can be scored." );
            } else {
                if ( isDataModified ) {
                    saveScore();
                }

                if (jumpRecapDataGridView.Rows.Count > 0) {
                    int rowIndex = jumpRecapDataGridView.Rows.Count - 1;
                    jumpRecapDataGridView.CurrentCell = jumpRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];

				} else {
					if ( checkForSkierRoundScore( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString()
						, Convert.ToInt32( roundSelect.RoundValue )
						, TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() ) ) {
						MessageBox.Show( "Skier already has a score in this round" );
						return;

					} else {
						if ( myCheckOfficials != null ) {
							/*
							 * Provide a warning message for class R events when official assignments have not been entered for the round and event group
							 * These assignments are not mandatory but they are strongly preferred and are very helpful for the TCs
							 */
							String curEventGroup = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value.ToString();
							if ( !( curEventGroup.Equals( myPrevEventGroup ) ) ) {
								if ( !( myCheckOfficials.checkOfficialAssignments( mySanctionNum, "Jump", curEventGroup, roundSelect.RoundValue ) ) ) {
									MessageBox.Show( "No officials have been assigned for this event group and round "
										+ "\n\nThese assignments are not mandatory but they are strongly recommended and are very helpful for the TCs" );
								}

								myPrevEventGroup = curEventGroup;
							}
						}

						Decimal cur1stRampHeight = getSkier1stRoundRampHeight( (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value, (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value );
						if ( cur1stRampHeight > 0 ) {
							if ( cur1stRampHeight != myJumpHeight ) {
								MessageBox.Show( " Skier used a ramp height of " + cur1stRampHeight.ToString( "0.0" ) + " in the first round."
									+ "\n Current ramp height is " + myJumpHeight.ToString( "0.0" )
									+ "\n Ensure these selections are desired."
									);
							}
						}
					}
				}

                if ( jumpRecapDataGridView.Rows.Count > 0 ) {
                    if ( checkRoundContinue() ) {
                        isAddRecapRowInProg = true;
                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 15;
                        curTimerObj.Tick += new EventHandler(addRecapRowTimer);
                        curTimerObj.Start();
                    }
                } else {
                    isAddRecapRowInProg = true;
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler(addRecapRowTimer);
                    curTimerObj.Start();
                }
            }
        }

        private void navSort_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;
                myTourProperties.RunningOrderSortJump = mySortCommand;
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
            myExportData.ExportData( mySanctionNum, "Jump", curMemberId, curAgeGroup, curEventGroup, curRound );
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[8];
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "JumpScore", "JumpRecap", "TourReg", "OfficialWork", "OfficialWorkAsgmt" };
            String curFilterCmd = myFilterCmd;
			if ( curFilterCmd.Contains( "Div =" ) ) {
				curFilterCmd = curFilterCmd.Replace( "Div =", "XT.AgeGroup =" );

			} else if ( curFilterCmd.Contains( "AgeGroup not in" ) ) {
				curFilterCmd = curFilterCmd.Replace( "AgeGroup not in", "XT.AgeGroup not in" );

			} else if ( curFilterCmd.Contains( "AgeGroup =" ) ) {
				curFilterCmd = curFilterCmd.Replace( "AgeGroup =", "XT.AgeGroup =" );
			}

			curSelectCommand[0] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[0] = curSelectCommand[0] + "And " + curFilterCmd + " ";
            }

            curSelectCommand[1] = "Select * from EventReg XT ";
            if ( isObjectEmpty(curFilterCmd) ) {
                curSelectCommand[1] = curSelectCommand[1]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = 'Jump'";
            } else {
                if ( curFilterCmd.Length > 0 ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Jump'"
                        + " And " + curFilterCmd;
                } else {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Jump'";
                }
            }

            curSelectCommand[2] = "Select * from EventRunOrder XT ";
            if ( isObjectEmpty(curFilterCmd) ) {
                curSelectCommand[2] = curSelectCommand[2]
                    + " Where SanctionId = '" + mySanctionNum + "'"
                    + " And Event = 'Jump' And Round = " + roundActiveSelect.RoundValue + " ";
            } else {
                if ( curFilterCmd.Length > 0 ) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Jump' And Round = " + roundActiveSelect.RoundValue + " "
                        + " And " + curFilterCmd;
                } else {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Jump' And Round = " + roundActiveSelect.RoundValue + " ";
                }
            }

            curSelectCommand[3] = "SELECT XT.* FROM JumpScore XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And Round = " + roundActiveSelect.RoundValue + " ";
            if ( !( isObjectEmpty(curFilterCmd) ) && curFilterCmd.Length > 0 ) {
                curSelectCommand[3] = curSelectCommand[3] + "And " + curFilterCmd + " ";
            }

            curSelectCommand[4] = "SELECT XT.* FROM JumpRecap XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND XT.AgeGroup = ER.AgeGroup AND ER.Event = 'Jump' "
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
                + "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Jump' AND ER.Round = " + roundActiveSelect.RoundValue + " "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[5] = curSelectCommand[5] + tmpFilterCmd + " ";
            }

            curSelectCommand[6] = "SELECT XT.* FROM OfficialWork XT "
                + "INNER JOIN EventReg ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Jump' "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And XT.LastUpdateDate is not null ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + " ";
            }
            curSelectCommand[6] = curSelectCommand[6] + "Union "
                + "SELECT XT.* FROM OfficialWork XT "
                + "INNER JOIN OfficialWorkAsgmt ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId AND ER.Event = 'Jump' AND ER.Round = " + roundActiveSelect.RoundValue + " "
                + "Where XT.SanctionId = '" + mySanctionNum + "' And XT.LastUpdateDate is not null ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + " ";
            }

            curSelectCommand[7] = "Select * from OfficialWorkAsgmt "
                + " Where SanctionId = '" + mySanctionNum + "' And Event = 'Jump' And Round = " + roundActiveSelect.RoundValue + " ";
            if ( !( isObjectEmpty(tmpFilterCmd) ) && tmpFilterCmd.Length > 0 ) {
                curSelectCommand[7] = curSelectCommand[7] + tmpFilterCmd + " ";
            }

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navRefreshTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( navRefreshTimer );
            navRefresh_Click(null, null);
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
                        myFilterCmd = "AgeGroup not in ('CM', 'CW', 'BM', 'BW') ";
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
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkierJump( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
                        } catch {
                        }
                    }
                    if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                        if (jumpRecapDataGridView.Rows.Count > 0) {
                            StringBuilder curTwitterMessage = new StringBuilder( "" );
                            String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
                            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                            String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                            curTwitterMessage.Append( mySanctionNum + " " + curAgeGroup + " " + curSkierName );
                            if (curTeamCode.Length > 0) {
                                curTwitterMessage.Append( " Team: " + curTeamCode );
                            }
                            curTwitterMessage.Append( " Dist: " + scoreFeetTextBox.Text + "Ft, " + scoreMetersTextBox.Text + "M" );
                            curTwitterMessage.Append( " Speed: " + JumpSpeedSelect.CurrentValueDesc + " Ramp: " + RampHeightSelect.CurrentValue.ToString( "0.0" ) );
                            curTwitterMessage.Append( " REPOST UNOFFICIAL" );
                            ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                        }
                    }
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "ResendAll" )) {
                    if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                        try {
                            String curEventGroup = EventGroupList.SelectedItem.ToString();
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkiers( "Jump", mySanctionNum, curRound, curEventGroup );
                        } catch {
                        }
                    }
                }
            }
        }

        private void roundSelect_Load(object sender, EventArgs e) {
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
                    MessageBox.Show( "roundSelect_Click:Exception:" + curValue );
                }
            }
            if ( TourEventRegDataGridView.Rows.Count > 0 ) {
                if ( !( isObjectEmpty( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
                    jumpRecapDataGridView.Rows.Clear();
                    setJumpScoreEntry( Convert.ToInt16( roundSelect.RoundValue ) );
                    setJumpRecapEntry( Convert.ToInt16( roundSelect.RoundValue ) );
                    //setNewRowPos();
                }
            }
        }

        private void roundActiveSelect_Click(object sender, EventArgs e) {
            //Update data if changes are detected
            if ( isDataModified ) {
                MessageBox.Show( "You must save your changes before selecting a new round" );
                return;
                /*
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
                if ( !( isDataModified ) ) {
                }
                 */
            }

            if (sender != null) {
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
                    jumpRecapDataGridView.Rows.Clear();
                    myEventRegViewIdx = curSaveEventRegViewIdx;
                    if (TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx) {
                        myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                    }
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                    roundSelect.RoundValue = roundActiveSelect.RoundValue;
                    setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
                    curTimerObj.Start();
                }
        }

        private void JumpSpeedSelect_Load(object sender, EventArgs e) {
            //JumpSpeedSelect.CurrentValue = Convert.ToInt16( boatSpeedTextBox.Text );
        }

        private void JumpSpeedSelect_Change(object sender, EventArgs e) {
            if ( sender != null ) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    JumpSpeedSelect.CurrentValue = Convert.ToInt16( Convert.ToDecimal( curValue) );
                } catch (Exception ex) {
                    curValue = ex.Message;
                }
            }

            String curEventClass = getSkierClass();
            if ( JumpSpeedSelect.CurrentValue < myMaxSpeed ) {
                //Set to class C times when speed less than max and tournament greater than class C (e.g. class R)
                if ( (Decimal)myClassCRow["ListCodeNum"] < (Decimal)myClassRow["ListCodeNum"] ) {
                    curEventClass = "C";
                }
            }
            if ( myRecapRow == null ) {
                loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );
            } else {
                if ( jumpRecapDataGridView.Rows.Count > 0 ) {
                    if ( myRecapRow.Index == ( jumpRecapDataGridView.Rows.Count - 1 ) ) {
                        if ( isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value )
                            && isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value ) ) {
                            myRecapRow.Cells["BoatSpeedRecap"].Value = JumpSpeedSelect.CurrentValue.ToString();
                            if ( !( isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value ) )
                                && !( isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value ) )
                                ) {
                                TimeValidate();
                            }
                        }
                        loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );
                    }
                }
            }
        }

        private void RampHeightSelect_Load( object sender, EventArgs e ) {
            if ( sender != null ) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    RampHeightSelect.CurrentValue = Convert.ToDecimal( curValue );
                } catch ( Exception ex ) {
                    curValue = ex.Message;
                    MessageBox.Show( "JumpSpeedSelect_Change:Exception:" + curValue );
                }
            }
        }

        private void RampHeightSelect_Change(object sender, EventArgs e) {
            String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
            RampHeightSelect.CurrentValue = Convert.ToDecimal( curValue );
            myJumpHeight = Convert.ToDecimal( curValue );
            RampHeightTextBox.Text = myJumpHeight.ToString( "0.0" );

            try {
                DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
                String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
                String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;
                Int16 curRound = Convert.ToInt16( roundSelect.RoundValue );
                
                StringBuilder curSqlStmt = new StringBuilder( "Update JumpScore " );
                curSqlStmt.Append( "Set RampHeight = " + RampHeightTextBox.Text + " " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( "  And MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "  And Round = '" + curRound.ToString() + "' " );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "Update JumpRecap " );
                curSqlStmt.Append( "Set RampHeight = " + RampHeightTextBox.Text + " " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( "  And MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "  And AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "  And Round = '" + curRound.ToString() + "' " );
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

            } catch ( Exception excp ) {
                MessageBox.Show( "Error updating skier jump height \n" + excp.Message );
            }

        }

        private void scoreEventClass_SelectedIndexChanged(object sender, EventArgs e) {
            addPassButton.Focus();
        }

        private void scoreEventClass_Validating(object sender, CancelEventArgs e) {
            String curMethodName = "Jump:ScoreEntrySeg3:scoreEventClass_Validating";
            int rowsProc = 0;
            
            ListItem curItem = (ListItem)scoreEventClass.SelectedItem;
            String curSkierClass = curItem.ItemValue;
            if (mySkierClassList.compareClassChange(curSkierClass, myTourClass) < 0) {
                MessageBox.Show("Class " + curSkierClass + " cannot be assigned to a skier in a class " + myTourClass + " tournament");
                e.Cancel = true;
            } else {
                String curEventClass = getSkierClass();
                TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value = curSkierClass;
                try {
                    Int64 curScorePK = 0;
                    if ( myScoreRow == null ) {
                        curScorePK = -1;
                    } else {
                        curScorePK = (Int64)myScoreRow["PK"];
                    }
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    if ( curScorePK > 0 ) {
                        curSqlStmt.Append( "Update JumpScore Set " );
                        curSqlStmt.Append( "EventClass = '" + curSkierClass + "'" );
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
            }
        }

        private void tourEventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView myDataView = (DataGridView)sender;

            //Update data if changes are detected
            if ( isDataModified && ( myEventRegViewIdx != e.RowIndex ) ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( myEventRegViewIdx != e.RowIndex ) {
                    jumpRecapDataGridView.Rows.Clear();
                    skierPassMsg.Text = "";
                    myEventRegViewIdx = e.RowIndex;
                    int curRowPos = e.RowIndex + 1;
                    RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
                    if ( !( isObjectEmpty( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
                        setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );

                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 15;
                        curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
                        curTimerObj.Start();
                    }
                }
            }
        }

        private void EventGroupList_SelectedIndexChanged( object sender, EventArgs e ) {
            Int16 curRound = 0;
            try {
                curRound = Convert.ToInt16( roundActiveSelect.RoundValue );
            } catch {
                curRound = 0;
            }
            if ( curRound >= 25 ) {
                MessageBox.Show( "You currently have the runoff round selected.\nChange the round if that is not desired." );
            }
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            if ( !( isDataModified ) ) {
                if ( !( isLoadInProg ) ) {
                    int curSaveEventRegViewIdx = myEventRegViewIdx;
                    navRefresh_Click( null, null );
                    if ( curSaveEventRegViewIdx > 0 ) {
                        jumpRecapDataGridView.Rows.Clear();
                        myEventRegViewIdx = curSaveEventRegViewIdx;
                        if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                            myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                        }
                        TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                        roundSelect.RoundValue = roundActiveSelect.RoundValue;
                        setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                        Timer curTimerObj = new Timer();
                        curTimerObj.Interval = 15;
                        curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
                        curTimerObj.Start();
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
                    if (TourEventRegDataGridView.Rows.Count > 0) {
                        TourEventRegDataGridView.Rows.Clear();
                    }
                    if (jumpRecapDataGridView.Rows.Count > 0) {
                        try {
                            jumpRecapDataGridView.Rows.Clear();
                        } catch {
                            jumpRecapDataGridView.EndEdit();
                            while (jumpRecapDataGridView.Rows.Count > 0) {
                                jumpRecapDataGridView.Rows.RemoveAt( jumpRecapDataGridView.Rows.Count - 1 );
                            }
                        }
                    }
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
                            curViewRow.Cells["Order"].Value = ( (Int16)curDataRow["RunOrder"] ).ToString( "##0" );
                        } catch {
                            curViewRow.Cells["Order"].Value = "0";
                        }
                        try {
                            if (myTourRules.ToLower().Equals( "iwwf" )) {
                                curViewRow.Cells["Score"].Value = ( (Decimal)curDataRow["ScoreMeters"] ).ToString( "##.#" );
                            } else {
                                curViewRow.Cells["Score"].Value = ( (Decimal)curDataRow["ScoreFeet"] ).ToString( "###" );
                            }
                        } catch {
                            curViewRow.Cells["Score"].Value = "";
                        }
                        try {
                            curViewRow.Cells["RankingScore"].Value = ( (Decimal)curDataRow["RankingScore"] ).ToString( "###0.0" );
                        } catch {
                            curViewRow.Cells["RankingScore"].Value = "";
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
                        try {
                            curViewRow.Cells["RankingRating"].Value = (String)curDataRow["RankingRating"];
                        } catch {
                            curViewRow.Cells["RankingRating"].Value = "";
                        }
                        try {
                            curViewRow.Cells["JumpHeight"].Value = (String)curDataRow["JumpHeight"];
                        } catch {
                            curViewRow.Cells["JumpHeight"].Value = "";
                        }
                        try {
                            curViewRow.Cells["SkiYearAge"].Value = ((Byte)curDataRow["SkiYearAge"]).ToString();
                            if ( curViewRow.Cells["SkiYearAge"].Value.ToString().Equals("") ) {
                                curViewRow.Cells["SkiYearAge"].Value = "1";
                            }
                        } catch {
                            curViewRow.Cells["SkiYearAge"].Value = "1";
                        }
                        try {
                            setEventRegRowStatus( (String)curDataRow["Status"], curViewRow );
                        } catch {
                            setEventRegRowStatus( "TBD", curViewRow );
                        }
                    }
                    myEventRegViewIdx = 0;
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];

                    jumpRecapDataGridView.Rows.Clear();
                    setJumpScoreEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    setJumpRecapEntry( Convert.ToInt16( roundActiveSelect.RoundValue ) );
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
                    curTimerObj.Start();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving jump tournament entries \n" + ex.Message );
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
                if (((ArrayList)EventGroupList.DataSource).Count > 0) {
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
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Jump' And Round = " + inRound.ToString() + " "
                + "Order by EventGroup";
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                    + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Jump'"
                    + "Order by EventGroup";
                curDataTable = DataAccess.getDataTable( curSqlStmt );
            }

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["EventGroup"] );
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

        private void PauseTimerButton_Click(object sender, EventArgs e) {
            myEventDelayStartTime = DateTime.Now;
            EventDelayReasonTextBox.Visible = true;
            EventDelayReasonLabel.Visible = true;
            EventDelayReasonTextBox.Text = "";
            EventDelayReasonTextBox.Focus();
            StartTimerButton.Visible = true;
            PauseTimerButton.Visible = false;
        }

        private void StartTimerButton_Click( object sender, EventArgs e ) {
            try {
                myEventDelaySeconds += Convert.ToInt32( ( (TimeSpan)DateTime.Now.Subtract( myEventDelayStartTime ) ).TotalSeconds );
                if ( EventDelayReasonTextBox.Text.Length > 1 ) {
                    Log.WriteFile( "Jump:ScoreEntrySeg3:Event delayed: " + EventDelayReasonTextBox.Text );
                }
            } catch {
            }
            EventDelayReasonTextBox.Visible = false;
            EventDelayReasonLabel.Visible = false;
            StartTimerButton.Visible = false;
            PauseTimerButton.Visible = true;
        }

        private void ForceCompButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Jump:ScoreEntrySeg3:ForceCompButton_Click";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            if ( TourEventRegDataGridView.Rows.Count > 0 ) {
                if ( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" ) ) {
                    String myBoatTime = "";
                    try {
                        myBoatTime = myRecapRow.Cells["BoatSplitTimeRecap"].Value.ToString();
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

                                    curSqlStmt.Append( "Update JumpScore Set " );
                                    curSqlStmt.Append( "Status = '" + curStatus + "'" );
                                    curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                                    curSqlStmt.Append( ", Note = '" + curReason + "'" );
                                    curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                                } else {
                                    MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                                }
                            } else {
                                MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                            }
                        }
                    } catch ( Exception excp ) {
                        String curMsg = ":Error attempting to perform skier complete \n" + excp.Message + "\n" + curSqlStmt.ToString();
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }

                    isDataModified = false;
                }
            }

        }

        private void loadBoatTimeView( String inSkierClass, Int16 inSpeed ) {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving boat times";
            Cursor.Current = Cursors.WaitCursor;

            try {
                listBoatTimesDataGridView.Rows.Clear();
                myTimesDataTable.DefaultView.RowFilter = "ListCode like '" + inSpeed.ToString() + "-" + inSkierClass + "-%'";
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

        public void setJumpScoreEntry( int inRound ) {
            Decimal curMaxRamp = Convert.ToDecimal("5.0");

            Cursor.Current = Cursors.WaitCursor;
            DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;
            activeSkierName.Text = (String)curEventRegRow.Cells["SkierName"].Value;

            DataRow[] curFoundRows = myMaxSpeedDataTable.Select( "ListCode = '" + curAgeGroup + "'" );
            if (curFoundRows.Length == 0) {
                MessageBox.Show( "Max speed entry not found for divsion code " + curAgeGroup );
                return;
            }
            DataRow curDataRow = curFoundRows[0];
            myMaxSpeed = Convert.ToInt16( (Decimal)curDataRow["MaxValue"] );
            Int16 curMinSpeed = 0;
            if ( myMinSpeedDataTable.Rows.Count > 0 ) {
                curDataRow = myMinSpeedDataTable.Rows[0];
                curMinSpeed = Convert.ToInt16( (String)curDataRow["Speed"] );
            }
            JumpSpeedSelect.SetMinMaxValue( curMinSpeed, myMaxSpeed );

            curDataRow = myMaxRampDataTable.Select( "ListCode = '" + curAgeGroup + "'" )[0];
            curMaxRamp = Convert.ToDecimal((Decimal)curDataRow["MaxValue"]);
            RampHeightSelect.SetMaxValue(curMaxRamp);

            if (myJumpHeight == 0) {
                myJumpHeight = curMaxRamp;
            } if (myJumpHeight > curMaxRamp) {
                myJumpHeight = curMaxRamp;
            }

            getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
            if ( myScoreDataTable.Rows.Count > 0 ) {
                myScoreRow = myScoreDataTable.Rows[0];
                try {
                    scoreEventClass.SelectedValue = (String)myScoreRow["EventClass"];
                } catch {
                    scoreEventClass.SelectedValue = (String)curEventRegRow.Cells["EventClass"].Value;
                }
                try {
                    roundSelect.RoundValue = ( (Byte)myScoreRow["Round"] ).ToString();
                } catch {
                    roundSelect.RoundValue = "1";
                }
                try {
                    scoreFeetTextBox.Text = ( (Decimal)myScoreRow["ScoreFeet"] ).ToString( "##0" );
                } catch {
                    scoreFeetTextBox.Text = "0";
                }
                try {
                    scoreMetersTextBox.Text = ((Decimal)myScoreRow["ScoreMeters"] ).ToString( "#0.0" );
                } catch {
                    scoreMetersTextBox.Text = "0";
                }
                try {
                    nopsScoreTextBox.Text = ( (Decimal)myScoreRow["NopsScore"] ).ToString( "#,##0.0" );
                } catch {
                    nopsScoreTextBox.Text = "0";
                }
                try {
                    noteTextBox.Text = (String)myScoreRow["Note"];
                } catch {
                    noteTextBox.Text = "";
                }
                try {
                    myJumpHeight = (Decimal)myScoreRow["RampHeight"];
                    RampHeightSelect.CurrentValue = myJumpHeight;
                } catch {
                    RampHeightSelect.CurrentValue = myJumpHeight;
                }
                RampHeightTextBox.Text = myJumpHeight.ToString( "0.0" );
                try {
                    JumpSpeedSelect.CurrentValue = Convert.ToInt16((Byte)myScoreRow["BoatSpeed"]);
                    BoatSpeedTextBox.Text = "";
                } catch {
                    JumpSpeedSelect.CurrentValue = myMaxSpeed;
                    BoatSpeedTextBox.Text = "";
                }
                TeamCodeTextBox.Text = (String)myScoreRow["TeamCode"];
				if ( myScoreRow["Boat"] == System.DBNull.Value ) {
					TourBoatTextbox.Text = "";
				} else {
					TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String) myScoreRow["Boat"] );
					TourBoatTextbox.Select( 0, 0 );
				}

				if ( myScoreRow["Boat"] == System.DBNull.Value ) {
					TourBoatTextbox.Text = "";
				} else {
					TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String) myScoreRow["Boat"] );
					TourBoatTextbox.Select( 0, 0 );
				}
				
				//JumpSpeedSelect_Change( null, null );
				scoreEntryInprogress();
            } else {
                myScoreRow = null;

                scoreEventClass.SelectedValue = (String)curEventRegRow.Cells["EventClass"].Value;
                scoreFeetTextBox.Text = "";
                scoreMetersTextBox.Text = "";
                nopsScoreTextBox.Text = "";
                noteTextBox.Text = "";
                RampHeightTextBox.Text = "";
                BoatSpeedTextBox.Text = "";
                TeamCodeTextBox.Text = "";
                RampHeightSelect.CurrentValue = myJumpHeight;
                JumpSpeedSelect.CurrentValue = myMaxSpeed;

				TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String) listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCode"].Value );
				TourBoatTextbox.Select( 0, 0 );

                scoreEntryBegin();
            }

            /*
            Decimal cur1stRampHeight = getSkier1stRoundRampHeight( curMemberId, curAgeGroup );
            if (cur1stRampHeight > 0) {
                if (cur1stRampHeight != myJumpHeight) {
                    MessageBox.Show( " Skier used a ramp height of " + cur1stRampHeight.ToString( "0.0" ) + " in the first round."
                        + "\n Current ramp height is " + myJumpHeight.ToString( "0.0" )
                        + "\n Ensure these selections are desired."
                        );
                }
            }
             * */
            
            String curEventClass = getSkierClass();
            loadBoatTimeView( curEventClass, JumpSpeedSelect.CurrentValue );
            Cursor.Current = Cursors.Default;
        }

        public void setJumpRecapEntry(int inRound) {
            myTimeTolMsg = "";
            myTriangleTolMsg = "";
            Int16 curBoatSpeed = JumpSpeedSelect.CurrentValue;

            Cursor.Current = Cursors.WaitCursor;
            skierPassMsg.Text = "";
            DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;

            jumpRecapDataGridView.Rows.Clear();
            getSkierRecapByRound( curMemberId, curAgeGroup, inRound );
            if ( myRecapDataTable.Rows.Count > 0 ) {
                DataGridViewRow curViewRow;
                int curViewIdx = 0;
                int curViewIdxMax = myRecapDataTable.Rows.Count - 1;
                foreach ( DataRow curDataRow in myRecapDataTable.Rows ) {
                    curViewIdx = jumpRecapDataGridView.Rows.Add();
                    curViewRow = jumpRecapDataGridView.Rows[curViewIdx];

                    curViewRow.Cells["Updated"].Value = "N";
                    curViewRow.Cells["PKRecap"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                    curViewRow.Cells["SanctionIdRecap"].Value = (String)curDataRow["SanctionId"];
                    curViewRow.Cells["MemberIdRecap"].Value = (String)curDataRow["MemberId"];
                    curViewRow.Cells["AgeGroupRecap"].Value = (String)curDataRow["AgeGroup"];
                    curViewRow.Cells["RoundRecap"].Value = ( (Byte)curDataRow["Round"] ).ToString();
                    curViewRow.Cells["RoundRecap"].ToolTipText = ( (DateTime)curDataRow["LastUpdateDate"] ).ToString( "MM/dd HH:mm:ss" );
                    curViewRow.Cells["PassNumRecap"].Value = (curViewIdx + 1).ToString();

                    try {
                        curViewRow.Cells["BoatSpeedRecap"].Value = ( (Byte)curDataRow["BoatSpeed"] ).ToString();
                    } catch {
                        curViewRow.Cells["BoatSpeedRecap"].Value = JumpSpeedSelect.CurrentValue;
                    }
                    try {
                        curViewRow.Cells["ReturnToBaseRecap"].Value = (String)curDataRow["ReturnToBase"];
                    } catch {
                    }
                    try {
                        curViewRow.Cells["ResultsRecap"].Value = (String)curDataRow["Results"];
                    } catch {
                        curViewRow.Cells["ResultsRecap"].Value = "Jump";
                    }
                    try {
                        curViewRow.Cells["Meter1Recap"].Value = ( (Decimal)curDataRow["Meter1"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter1Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Meter2Recap"].Value = ( (Decimal)curDataRow["Meter2"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter2Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Meter3Recap"].Value = ( (Decimal)curDataRow["Meter3"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter3Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Meter4Recap"].Value = ( (Decimal)curDataRow["Meter4"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter4Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Meter5Recap"].Value = ( (Decimal)curDataRow["Meter5"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter5Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Meter6Recap"].Value = ( (Decimal)curDataRow["Meter6"] ).ToString( "##0.00" );
                    } catch {
                        curViewRow.Cells["Meter6Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["BoatSplitTimeRecap"].Value = ( (Decimal)curDataRow["BoatSplitTime"] ).ToString("#.00");
                    } catch {
                        curViewRow.Cells["BoatSplitTimeRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["BoatSplitTime2Recap"].Value = ( (Decimal)curDataRow["BoatSplitTime2"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["BoatSplitTime2Recap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["BoatEndTimeRecap"].Value = ( (Decimal)curDataRow["BoatEndTime"] ).ToString( "#.00" );
                    } catch {
                        curViewRow.Cells["BoatEndTimeRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Split52TimeTolRecap"].Value = ((Int16)curDataRow["Split52TimeTol"]).ToString();
                    } catch {
                        curViewRow.Cells["Split52TimeTolRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Split82TimeTolRecap"].Value = ((Int16)curDataRow["Split82TimeTol"]).ToString();
                    } catch {
                        curViewRow.Cells["Split82TimeTolRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Split41TimeTolRecap"].Value = ((Int16)curDataRow["Split41TimeTol"]).ToString();
                    } catch {
                        curViewRow.Cells["Split41TimeTolRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ScoreFeetRecap"].Value = ( (Decimal)curDataRow["ScoreFeet"] ).ToString( "##0" );
                    } catch {
                        curViewRow.Cells["ScoreFeetRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["ScoreMetersRecap"].Value = ( (Decimal)curDataRow["ScoreMeters"] ).ToString( "#0.0" );
                    } catch {
                        curViewRow.Cells["ScoreMetersRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["TimeInTolRecap"].Value = (String)curDataRow["TimeInTol"];
                    } catch {
                        curViewRow.Cells["TimeInTolRecap"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["RerideRecap"].Value = (String)curDataRow["Reride"];
                    } catch {
                        curViewRow.Cells["RerideRecap"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["ScoreProtRecap"].Value = (String)curDataRow["ScoreProt"];
                    } catch {
                        curViewRow.Cells["ScoreProtRecap"].Value = "N";
                    }
                    try {
                        curViewRow.Cells["ScoreTriangleRecap"].Value = ( (Decimal)curDataRow["ScoreTriangle"] ).ToString( "#0.00" );
                    } catch {
                        curViewRow.Cells["ScoreTriangleRecap"].Value = "";
                    }
                    try {
                        curViewRow.Cells["RerideReasonRecap"].Value = (String)curDataRow["RerideReason"];
                    } catch {
                        curViewRow.Cells["RerideReasonRecap"].Value = "";
                    }
                    try {
                        //curViewRow.Cells["RampHeightRecap"].Value = ( (Decimal)curDataRow["RampHeight"] ).ToString( "#.0" );
                        curViewRow.Cells["RampHeightRecap"].Value = RampHeightTextBox.Text;
                    } catch {
                        //curViewRow.Cells["RampHeightRecap"].Value = "";
                        curViewRow.Cells["RampHeightRecap"].Value = RampHeightTextBox.Text;
                    }
                    try {
                        curViewRow.Cells["NoteRecap"].Value = (String)curDataRow["Note"];
                    } catch {
                        curViewRow.Cells["NoteRecap"].Value = "";
                    }

                    if ( curViewRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
                        curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
                    } else {
                        curViewRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    }

                    if ( curViewIdx < curViewIdxMax ) {
                        foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
                            if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                                if ( !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideRecap" ) )
                                    && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideReasonRecap" ) )
                                    && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "ScoreProtRecap" ) )
                                    ) {
                                    curCell.ReadOnly = true;
                                }
                            }
                        }
                    } else {
                        try {
                            JumpSpeedSelect.CurrentValue = Convert.ToInt16( myRecapRow.Cells["BoatSpeedRecap"].Value.ToString() );
                        } catch {
                            JumpSpeedSelect.CurrentValue = myMaxSpeed;
                        }
                    }
                    myRecapRow = curViewRow;
                }
            } else {
                skierPassMsg.Text = "";
                myRecapRow = null;
                scoreEntryBegin();
            }
            Cursor.Current = Cursors.Default;
        }

        private String getSkierClass() {
            String curEventClass = (String)scoreEventClass.SelectedValue;
            myClassRow = mySkierClassList.SkierClassDataTable.Select("ListCode = '" + curEventClass + "'")[0];
            //Use class R times for all Classes greater than class C (e.g. class E, L, X, etc..)
            //Use class C times for all Classes less than or equal to class C (e.g. class C, N, F, G)
            if ( (Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassCRow["ListCodeNum"] ) {
                curEventClass = "R";
            } else {
                curEventClass = "C";
            }

            return curEventClass;
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

        private void jumpRecapDataGridView_Leave( object sender, EventArgs e ) {
            jumpRecapDataGridView.EndEdit();
        }

        private void jumpRecapDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            DataGridView curView = (DataGridView)sender;
            Int64 curPK = 0;
            if (isObjectEmpty( (String)curView.Rows[e.RowIndex].Cells["PKRecap"].Value )) {
            } else {
                curPK = Convert.ToInt64( (String)curView.Rows[e.RowIndex].Cells["PKRecap"].Value );
            }
            if ( curPK > 0 ) {
                skierPassMsg.Text = (String)curView.Rows[e.RowIndex].Cells["RerideReasonRecap"].Value;
            }
        }

        private void jumpRecapDataGridView_KeyUp(object sender, KeyEventArgs e) {
            DataGridView curView = (DataGridView)sender;

            if ( e.KeyCode == Keys.Enter ) {
                if (isRecapRowEnterHandled || isAddRecapRowInProg) {
                    isAddRecapRowInProg = false;
                    isRecapRowEnterHandled = false;
                } else {
                    String curColName = curView.Columns[( (DataGridView)sender ).CurrentCell.ColumnIndex].Name;
                    if (curColName.Equals( "ScoreMetersRecap" )
                        || ( curColName.Equals( "ScoreMetersRecap" ) 
                            && ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
                                || myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" ) ) )
                        ) {
                    } else {
                        SendKeys.Send( "{TAB}" );
                    }
                }
                e.Handled = true;
            } else if (e.KeyCode == Keys.Tab) {
                if ( isAddRecapRowInProg ) {
                    isAddRecapRowInProg = false;
                    e.Handled = true;
                } else {
                    e.Handled = true;
                }
            } else if ( e.KeyCode == Keys.Escape ) {
                isAddRecapRowInProg = false;
                e.Handled = true;
            } else if ( e.KeyCode == Keys.Delete ) {
                isAddRecapRowInProg = false;
                if ( !( curView.CurrentCell.ReadOnly ) ) {
                    curView.CurrentCell.Value = "";
                    e.Handled = true;
                }
            } else {
                isAddRecapRowInProg = false;
            }
        }

        private void jumpRecapDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            String curColName = curView.Columns[e.ColumnIndex].Name;
            if ( curColName.Equals( "ReturnToBaseRecap" )
                 || curColName.Equals( "RerideRecap" )
               ) {
                SendKeys.Send( "{TAB}" );
            }
        }

        private void jumpRecapDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            myRecapRow = (DataGridViewRow)curView.Rows[e.RowIndex];
            String curColName = curView.Columns[e.ColumnIndex].Name;
            if ( curColName.StartsWith( "Meter" )
                || curColName.Equals("ScoreFeetRecap")
                || curColName.Equals("ScoreMetersRecap")
                || curColName.Equals("BoatEndTimeRecap")
                || curColName.Equals("BoatSplitTimeRecap")
                || curColName.Equals( "BoatSplitTime2Recap" )
                ) {
                try {
                    myOrigCellValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigCellValue = "";
                }
            } else if ( curColName.Equals( "ScoreProtRecap" )
                || curColName.Equals( "RerideRecap" )
                || curColName.Equals( "ReturnToBaseRecap" )
                || curColName.Equals( "ResultsRecap" )
                ) {
                try {
                    myOrigCellValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigCellValue = "";
                }
            } else if ( curColName.Equals( "RerideReasonRecap" )) {
                if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) {
                    checkRoundCalcSkierScore();
                }

            }

        }

        private void jumpRecapDataGridView_SetNewRowCell( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();

            if ( jumpRecapDataGridView.Rows.Count > 0 ) {
                int rowIndex = jumpRecapDataGridView.Rows.Count - 1;
                jumpRecapDataGridView.Select();
                jumpRecapDataGridView.CurrentCell = jumpRecapDataGridView.Rows[rowIndex].Cells[myStartCellIndex];
                JumpSpeedSelect.CurrentValue = Convert.ToInt16( jumpRecapDataGridView.CurrentRow.Cells["BoatSpeedRecap"].Value.ToString() );
                //JumpSpeedSelect_Change( null, null );
            }

            curTimerObj.Tick -= new EventHandler( jumpRecapDataGridView_SetNewRowCell );
        }

        private void jumpRecapDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            myRecapRow = (DataGridViewRow)curView.Rows[e.RowIndex];
            myRecapRow.ErrorText = "";
            isRecapRowEnterHandled = false;

            String curColName = curView.Columns[e.ColumnIndex].Name;
            if (myRecapRow.Cells[e.ColumnIndex].Value != null) {
                if ( curColName.StartsWith("Meter")) {
                    myModCellValue = "";
                    if (!(e.FormattedValue.Equals(""))) {
                        try {
                            Decimal curCellNumValue = Convert.ToDecimal(e.FormattedValue);
                            String curCellValue = (String)e.FormattedValue;
                            int delimPos = curCellValue.IndexOf( '.' );
                            if ( delimPos < 0 ) {
                                myModCellValue = curCellValue.Substring( 0, curCellValue.Length - 1 ) + "." + curCellValue.Substring( curCellValue.Length - 1, 1 );
                            } else {
                                myModCellValue = curCellValue;
                            }
                        } catch ( Exception exp ) {
                            myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
                            MessageBox.Show(myRecapRow.ErrorText);
                            e.Cancel = true;
                        }
                    }
                } else if ( curColName.Equals( "BoatSplitTimeRecap" )
                    || curColName.Equals( "BoatSplitTime2Recap" )
                    || curColName.Equals("BoatEndTimeRecap")
                    ) {
                    String curValue = (String)e.FormattedValue;
                    if ( isObjectEmpty( curValue ) ) {
                        myModCellValue = "";
                    } else {
                        if ( curValue != myOrigCellValue ) {
                            if ( curValue.ToUpper().Equals("OK") ) {
                                myModCellValue = "";
                            } else if ( curValue.ToUpper().Equals("NONE") ) {
                                myModCellValue = "";
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
                } else if ( curColName.Equals( "ScoreFeetRecap" ) ) {
                        if ( (myNumJudges == 0) && !(e.FormattedValue.Equals(""))) {
                        try {
                            Decimal scoreFeet = Convert.ToDecimal( e.FormattedValue );
                            if ( !(isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value) ) ) {
                                Decimal scoreMeters = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreMetersRecap"].Value);
                                String curMsg = CalcDistValidate( scoreFeet, scoreMeters );
                                if ( curMsg.Length > 0 ) {
                                    myRecapRow.ErrorText = curMsg;
                                    MessageBox.Show( myRecapRow.ErrorText );
                                    e.Cancel = true;
                                } else {
                                    e.Cancel = false;
                                }
                            }
                        } catch ( Exception exp ) {
                            myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
                            MessageBox.Show(myRecapRow.ErrorText);
                            e.Cancel = true;
                        }
                    }
                } else if (curColName.Equals("ScoreMetersRecap") ) {
                    myModCellValue = "";
                    if ( ( myNumJudges == 0 ) && !( e.FormattedValue.Equals( "" ) ) ) {
                        try {
                            isRecapRowEnterHandled = true;
                            Decimal curCellNumValue = Convert.ToDecimal( e.FormattedValue );
                            String curCellValue = (String)e.FormattedValue;
                            int delimPos = curCellValue.IndexOf( '.' );
                            if ( delimPos < 0 ) {
                                myModCellValue = curCellValue.Substring( 0, curCellValue.Length - 1 ) + "." + curCellValue.Substring( curCellValue.Length - 1, 1 );
                            } else {
                                myModCellValue = curCellValue;
                            }
                            Decimal scoreMeters = Convert.ToDecimal( myModCellValue );
                            if ( !(isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value) ) ) {
                                Decimal scoreFeet = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value );
                                String curMsg = CalcDistValidate( scoreFeet, scoreMeters );
                                if ( curMsg.Length > 0 ) {
                                    myRecapRow.ErrorText = curMsg;
                                    MessageBox.Show( myRecapRow.ErrorText );
                                    e.Cancel = true;
                                } else {
                                    e.Cancel = false;
                                }
                            }
                        } catch ( Exception exp ) {
                            myRecapRow.ErrorText = "Value in " + curColName + " must be numeric";
                            MessageBox.Show( myRecapRow.ErrorText );
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void jumpRecapDataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            DataGridView curView = (DataGridView)sender;
            myRecapRow = (DataGridViewRow)curView.Rows[e.RowIndex];
            myRecapRow.ErrorText = "";

            if ( e.RowIndex < (curView.Rows.Count - 1) ) {
                //Check for changes to information on passes other than the last one
                //Only indications for rerides are allowed to be updated on these rows
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals( "RerideRecap" ) ) {
                    #region Validate reride attribute
                    if ( (String)myRecapRow.Cells["RerideRecap"].Value != myOrigCellValue ) {
                        if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                            rerideReasonDialogForm.RerideReasonText = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
                            if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
                                String curCommand = rerideReasonDialogForm.Command;
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
                                if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) {
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                }
                                checkRoundCalcSkierScore();
                            } else {
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                rerideReasonDialogForm.RerideReasonText = "";
                                MessageBox.Show( "Reride can not be granted without a reason being specified." );
                                myRecapRow.Cells["RerideRecap"].Value = "N";
                                checkRoundCalcSkierScore();
                            }
                        } else {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            checkRoundCalcSkierScore();
                        }
                    }
                    #endregion
                }
            
            } else {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if (curColName.StartsWith("Meter")) {
                    #region Validate meter attributes
                    try {
                        if ( isObjectEmpty( myRecapRow.Cells[e.ColumnIndex].Value ) ) {
                            if ( (String)myRecapRow.Cells[e.ColumnIndex].Value != myOrigCellValue ) {
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                            }
                        } else {
                            Decimal curMeterScore = Convert.ToDecimal( (String)myRecapRow.Cells[e.ColumnIndex].Value );
                            if ( myModCellValue.Length > 0 ) {
                                curMeterScore = Convert.ToDecimal( myModCellValue );
                                myRecapRow.Cells[e.ColumnIndex].Value = curMeterScore.ToString( "##0.00" );
                                if ( (String)myRecapRow.Cells[e.ColumnIndex].Value != myOrigCellValue ) {
                                    isDataModified = true;
                                    myRecapRow.Cells["Updated"].Value = "Y";
                                }
                            }
                            if ( isDataModified ) {
                                if ( !(isObjectEmpty(myRecapRow.Cells["Meter1Recap"].Value) )
                                    && !(isObjectEmpty(myRecapRow.Cells["Meter2Recap"].Value) )
                                    && !( isObjectEmpty( myRecapRow.Cells["Meter3Recap"].Value ) )
                                    ) {
                                    if ( myNumJudges == 3 ) {
                                        bool curCalcComp = CalcDistByMeters();
                                        checkRoundCalcSkierScore();
                                    } else if (myNumJudges == 6) {
                                        if ( !( isObjectEmpty( myRecapRow.Cells["Meter4Recap"].Value ) )
                                            && !( isObjectEmpty( myRecapRow.Cells["Meter5Recap"].Value ) )
                                            && !( isObjectEmpty( myRecapRow.Cells["Meter6Recap"].Value ) )
                                            ) {
                                            bool curCalcComp = CalcDistByMeters();
                                            checkRoundCalcSkierScore();
                                        }
                                    }
                                }
                            }
                        }
                    } catch {
                        MessageBox.Show( "Meter value must be numeric" );
                    }
                    #endregion
                
                } else if ( curColName.Equals("BoatSplitTimeRecap") ) {
                    // Validate BoatSplitTime (52M segment) if entered
                    if ( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value ) ) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        return;
                    } else {
                        skierPassMsg.Text = "";
                        SplitTimeFormat();
                        if ( (String) myRecapRow.Cells["BoatSplitTimeRecap"].Value != myOrigCellValue ) {
                            myOrigCellValue = (String) myRecapRow.Cells["BoatSplitTimeRecap"].Value;

                            /*
                            * If all 3 times have been entered then they will be validated
                            * Also if scores available determine if score for round should be updated.
                            */
                            checkNeedTimeValidate();
                        }
                    }

                } else if ( curColName.Equals( "BoatSplitTime2Recap" ) ) {
                    // Validate BoatSplitTime2 (82M segment) if entered
                    if ( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value) ) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        return;
                    } else {
                        skierPassMsg.Text = "";
                        Split2TimeFormat();
                        if ( (String) myRecapRow.Cells["BoatSplitTime2Recap"].Value != myOrigCellValue ) {
                            myOrigCellValue = (String) myRecapRow.Cells["BoatSplitTime2Recap"].Value;

                            /*
                            * If all 3 times have been entered then they will be validated
                            * Also if scores available determine if score for round should be updated.
                            */
                            checkNeedTimeValidate();
                        }
                    }

                } else if ( curColName.Equals( "BoatEndTimeRecap" ) ) {
                    // Validate BoatEndTime (41M segment) if entered
                    if ( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) ) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        return;
                    } else {
                        skierPassMsg.Text = "";
                        EndTimeFormat();
                        if ( (String) myRecapRow.Cells["BoatEndTimeRecap"].Value != myOrigCellValue ) {
                            myOrigCellValue = (String) myRecapRow.Cells["BoatEndTimeRecap"].Value;

                            /*
                            * If all 3 times have been entered then they will be validated
                            * Also if scores available determine if score for round should be updated.
                            */
                            checkNeedTimeValidate();
                        }
                    }

                } else if ( curColName.Equals( "ScoreFeetRecap" ) ) {
                    #region Validate ScoreFeet attribute
                    if ( myNumJudges == 0 ) {
                        if ( !( isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value ) ) ) {
                            if ( (String)myRecapRow.Cells["ScoreFeetRecap"].Value != myOrigCellValue ) {
                                myOrigCellValue = (String)myRecapRow.Cells["ScoreFeetRecap"].Value;
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                if ( !( isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value ) )
                                    && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value) )
                                    && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTime2Recap"].Value) )
                                    && !( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) )
                                    ) {
                                    if ( (Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"] ) {
                                        TimeValidate();
                                    }
                                    CalcDist();
                                }
                            }
                        }
                    }
                    #endregion

                } else if ( curColName.Equals( "ScoreMetersRecap" ) ) {
                    #region Validate ScoreMeters attribute
                    if (myNumJudges == 0) {
                        if ( isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value ) ) {
                            if ( (String)myRecapRow.Cells["ScoreMetersRecap"].Value == myOrigCellValue ) {
                                if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) {
                                    if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
                                        || myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
                                        ) {
                                        checkRoundCalcSkierScore();
                                    }
                                }
                            
                            } else {
                                myOrigCellValue = (String)myRecapRow.Cells["ScoreMetersRecap"].Value;
                                isRecapRowEnterHandled = true;
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
                                    || myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
                                    ) {
                                    checkRoundCalcSkierScore();
                                }
                            }
                        
                        } else {
                            Decimal curScore = Convert.ToDecimal( (String)myRecapRow.Cells[e.ColumnIndex].Value );
                            if ( myModCellValue.Length > 0 ) {
                                curScore = Convert.ToDecimal( myModCellValue );
                                myRecapRow.Cells[e.ColumnIndex].Value = curScore.ToString( "#0.0" );
                            }
                            if ( (String)myRecapRow.Cells["ScoreMetersRecap"].Value == myOrigCellValue ) {
                                if ( myRecapRow.Cells["Updated"].Value.Equals( "Y" ) ) {
                                    if ( !( isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value ) ) ) {
                                        if ( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value ) > 0
                                            && Convert.ToDecimal( (String)myRecapRow.Cells["ScoreMetersRecap"].Value ) > 0
                                            && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value) )
                                            && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTime2Recap"].Value) )
                                            && !( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) )
                                            ) {
                                            checkRoundCalcSkierScore();
                                        }
                                    }
                                
                                } else {
                                    if (checkRoundContinue()) {
                                        isAddRecapRowInProg = true;
                                        Timer curTimerObj = new Timer();
                                        curTimerObj.Interval = 15;
                                        curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                                        curTimerObj.Start();
                                    }
                                }
                            
                            } else {
                                myOrigCellValue = (String)myRecapRow.Cells["ScoreMetersRecap"].Value;
                                isRecapRowEnterHandled = true;
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                if ( !( isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value ) ) ) {
                                    if ( Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value ) > 0
                                        && Convert.ToDecimal( (String)myRecapRow.Cells["ScoreMetersRecap"].Value ) > 0 
                                        && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value) )
                                        && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTime2Recap"].Value) )
                                        && !( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) )
                                        ) {

                                        if ( (Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"] ) {
                                            TimeValidate();
                                        }
                                        checkRoundCalcSkierScore();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                } else if ( curColName.Equals( "RerideRecap" ) ) {
                    #region Validate reride attribute
                    if ( (String)myRecapRow.Cells["RerideRecap"].Value != myOrigCellValue ) {
                        if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                            if ( isObjectEmpty( myRecapRow.Cells["ScoreFeetRecap"].Value )
                                || isObjectEmpty( myRecapRow.Cells["ScoreMetersRecap"].Value )
                                || isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value )
                                || isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value )
                                ) {
                                myRecapRow.Cells["ResultsRecap"].Value = "Pass";
                            }
                            rerideReasonDialogForm.RerideReasonText = (String)myRecapRow.Cells["RerideReasonRecap"].Value;
                            if ( rerideReasonDialogForm.ShowDialog() == DialogResult.OK ) {
                                String curCommand = rerideReasonDialogForm.Command;
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                myRecapRow.Cells["RerideReasonRecap"].Value = rerideReasonDialogForm.RerideReasonText;
                                if ( curCommand.ToLower().Equals( "updatewithprotect" ) ) {
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                }
                                checkRoundCalcSkierScore("Y");
                            
                            } else {
                                isDataModified = true;
                                myRecapRow.Cells["Updated"].Value = "Y";
                                rerideReasonDialogForm.RerideReasonText = "";
                                MessageBox.Show( "Reride can not be granted without a reason being specified." );
                                myRecapRow.Cells["RerideRecap"].Value = "N";
                                checkRoundCalcSkierScore();
                            }
                        
                        } else {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            checkRoundCalcSkierScore();
                        }
                    }
                    #endregion
                
                } else if ( curColName.Equals( "ResultsRecap" ) ) {
                    #region Validate Results attribute
                    if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )
                        && !(myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( myOrigCellValue ))
                        ) {
                        if ( !( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value))
                            && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value))
                            ) {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            TimeValidate();

                            if ( !( isObjectEmpty(myRecapRow.Cells["ScoreFeetRecap"].Value))
                                && !( isObjectEmpty(myRecapRow.Cells["ScoreMetersRecap"].Value))
                                ) {
                                CalcDist();
                                saveScore();
                                if ( checkRoundContinue() ) {
                                    isAddRecapRowInProg = true;
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 15;
                                    curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                                    curTimerObj.Start();
                                }
                            }
                        }
                    } else if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Fall" )
                        && !( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( myOrigCellValue ) )
                        ) {
                        if ( !( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value))
                            && !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value))
                            ) {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            TimeValidate();
                            if ( myOrigCellValue.Equals( "Jump" ) ) { CalcDist(); }
                            saveScore();
                            if ( checkRoundContinue() ) {
                                isAddRecapRowInProg = true;
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 15;
                                curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                                curTimerObj.Start();
                            }
                        }
                    } else if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Pass" )
                        && !( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( myOrigCellValue ) )
                        ) {
                        if ( !( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value)) ) {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            TimeValidate();
                            saveScore();
                            if ( checkRoundContinue() ) {
                                isAddRecapRowInProg = true;
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 15;
                                curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                                curTimerObj.Start();
                            }
                        }
                    }
                    #endregion
                } else if ( curColName.Equals( "ReturnToBaseRecap" ) ) {
                    #region Validate ReturnToBase attribute
                    if ( (String)myRecapRow.Cells["ReturnToBaseRecap"].Value != myOrigCellValue ) {
                        String curValue = curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                        winStatusMsg.Text = "ReturnToBaseRecap = " + curValue;
                        if ( !( isObjectEmpty( myRecapRow.Cells["BoatEndTimeRecap"].Value ) )
                            && !( isObjectEmpty( myRecapRow.Cells["BoatSplitTimeRecap"].Value ) )
                            ) {
                            isDataModified = true;
                            myRecapRow.Cells["Updated"].Value = "Y";
                            TimeValidate();
                        }
                    }
                    #endregion
                }
            }
        }

        /*
         * Method used when a time entry attribute for a pass has been modified
         * Check all 3 time attributes and validate the times if all have been entered
        */
        private void checkNeedTimeValidate() {
            isDataModified = true;
            myRecapRow.Cells["Updated"].Value = "Y";

            // BoatSplitTime (52M segment)
                if ( isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value) ) return;

            // BoatSplitTime2 (82M segment)
            if ( isObjectEmpty(myRecapRow.Cells["BoatSplitTime2Recap"].Value) ) return;

            if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals("Pass") ) {
                // Validate times
                TimeValidate();

                // Check to determine if score for round should be update
                checkRoundCalcSkierScore();

            } else if ( myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals("Fall") ) {
                // BoatEndTime (41M segment)
                if ( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) ) return;

                // Validate times
                TimeValidate();

                // Check to determine if score for round should be update
                checkRoundCalcSkierScore();

            } else {
                if ( isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value) ) return;

                // Validate times
                TimeValidate();

                // If feet and meters have been entered then check to determine if score for round should be update
                if ( isObjectEmpty(myRecapRow.Cells["ScoreFeetRecap"].Value) ) return;
                if ( isObjectEmpty(myRecapRow.Cells["ScoreMetersRecap"].Value) ) return;
                checkRoundCalcSkierScore();

            }
        }

        private void checkRoundCalcSkierScore() {
            checkRoundCalcSkierScore("");
        }
        private void checkRoundCalcSkierScore(String inRerideInd) {
            CalcSkierScore();
            saveScore();
            if ( checkRoundContinue() ) {
                isAddRecapRowInProg = true;
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 15;
                curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                curTimerObj.Start();
            } else if ( !(isObjectEmpty( inRerideInd ) ) ) {
                if ( inRerideInd.Equals( "Y" ) ) {
                    isAddRecapRowInProg = true;
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( addRecapRowTimer );
                    curTimerObj.Start();
                }
            }
        }

        private bool checkRoundContinue() {
            String curMethodName = "Jump:ScoreEntrySeg3:checkRoundContinue";
            Int16 curGoodPassCount = 0, curAcptPassCount = 0;
            Int16 curTolSplit52Time = 0, curTolSplit82Time = 0, curTolEndTime = 0;
            bool curReturnValue = false;
            Decimal scoreFeet, scoreMeters;
            String curSkierStatus = "";
            DataGridView curView = jumpRecapDataGridView;

            //if ( curRow.Cells["ResultsRecap"].Value.Equals( "Pass" ) ) {

            if ( ( myRecapRow.Cells["ResultsRecap"].Value.Equals("Jump") || myRecapRow.Cells["ResultsRecap"].Value.Equals("fall") )
                && (
                isObjectEmpty( scoreFeetTextBox.Text )
                || isObjectEmpty( scoreMetersTextBox.Text )
                || isObjectEmpty(myRecapRow.Cells["BoatSplitTimeRecap"].Value)
                || isObjectEmpty(myRecapRow.Cells["BoatSplitTime2Recap"].Value)
                || isObjectEmpty(myRecapRow.Cells["BoatEndTimeRecap"].Value)
                ) ) {
                curReturnValue = false;
            
            } else {
                Decimal skierScoreFeet = Convert.ToDecimal( scoreFeetTextBox.Text );
                Decimal skierScoreMeters = Convert.ToDecimal( scoreMetersTextBox.Text );

                foreach ( DataGridViewRow curRow in jumpRecapDataGridView.Rows ) {
                    if ( curRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
                        if ( curRow.Cells["RerideRecap"].Value.ToString().Equals( "N" ) ) {
                            if ( curRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" ) ) {
                                if ( curRow.Cells["ScoreFeetRecap"].Value.ToString().Length > 0
                                    && curRow.Cells["ScoreMetersRecap"].Value.ToString().Length > 0
                                    ) {
                                    curGoodPassCount++;
                                }
                            } else {
                                curGoodPassCount++;
                            }
                        }
                    }
                    if ( curRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                        if ( curRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
                            if ( isObjectEmpty( curRow.Cells["RerideReasonRecap"].Value ) ) {
                                curSkierStatus = "3-Error";
                                curAcptPassCount++;
                                MessageBox.Show( "Reride request must have a reason when times are in tolerance" );
                            }
                        
                        } else {
                            if ( curRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" ) ) {
                                if (curRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" )) {
                                    if (curRow.Cells["ScoreFeetRecap"].Value.ToString().Length > 0
                                        && curRow.Cells["ScoreMetersRecap"].Value.ToString().Length > 0
                                        ) {
                                        Decimal curPassScoreFeet = Convert.ToDecimal( curRow.Cells["ScoreFeetRecap"].Value.ToString() );
                                        Decimal curPassScoreMeters = Convert.ToDecimal( curRow.Cells["ScoreMetersRecap"].Value.ToString() );
                                        if (curPassScoreFeet > skierScoreFeet && curPassScoreMeters > skierScoreMeters) {
                                            curRow.Cells["ScoreProtRecap"].Value = "N";
                                        }
                                    }
                                
                                } else {
                                    if (curRow.Cells["ScoreFeetRecap"].Value.ToString().Length > 0
                                        && curRow.Cells["ScoreMetersRecap"].Value.ToString().Length > 0
                                        ) {
                                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                            try {
                                                scoreMeters = Convert.ToDecimal( curRow.Cells["ScoreMetersRecap"].Value.ToString() );
                                            } catch {
                                                scoreMeters = 0;
                                            }
                                            if (skierScoreMeters > scoreMeters) {
                                                curAcptPassCount++;
                                            }
                                        
                                        } else {
                                            try {
                                                scoreFeet = Convert.ToDecimal( curRow.Cells["ScoreFeetRecap"].Value.ToString() );
                                            } catch {
                                                scoreFeet = 0;
                                            }
                                            if (skierScoreFeet > scoreFeet) {
                                                curAcptPassCount++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    
                    } else {
                        curAcptPassCount++;
                    }
                }

                if ( curAcptPassCount < 3 ) {
                    if ( jumpRecapDataGridView.Rows.Count == 0 ) {
                        curSkierStatus = "1-TBD";
                        curReturnValue = true;
                    
                    } else {
                        curSkierStatus = "2-InProg";
                        DataGridViewRow curRow = jumpRecapDataGridView.Rows[jumpRecapDataGridView.Rows.Count - 1];
                        if ( !( isObjectEmpty( curRow.Cells["ScoreFeetRecap"].Value ) )
                            && !( isObjectEmpty( curRow.Cells["ScoreMetersRecap"].Value ) )
                            && !( isObjectEmpty( curRow.Cells["BoatSplitTimeRecap"].Value ) )
                            && !( isObjectEmpty( curRow.Cells["BoatEndTimeRecap"].Value ) )
                            ) {
                            curReturnValue = true;
                        
                        } else {
                            if ( curRow.Cells["ResultsRecap"].Value.Equals( "Fall" ) ) {
                                if ( !( isObjectEmpty( curRow.Cells["BoatSplitTimeRecap"].Value ) )
                                && !( isObjectEmpty( curRow.Cells["BoatEndTimeRecap"].Value ) )
                                ) {
                                    curReturnValue = true;
                                }
                            } else if ( curRow.Cells["ResultsRecap"].Value.Equals( "Pass" ) ) {
                                if ( isObjectEmpty( curRow.Cells["BoatSplitTimeRecap"].Value ) ) {
                                    if ( curRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                                        curReturnValue = true;
                                    }
                                } else {
                                    curReturnValue = true;
                                }
                            }
                        }
                    }
                
                } else {
                    if ( curGoodPassCount < 3 ) {
                        curSkierStatus = "2-InProg";
                    } else {
                        curSkierStatus = "4-Done";
                    }
                    String curMessage = "Round complete";
                    int curCountOptions = 0;
                    foreach ( DataGridViewRow curRow in jumpRecapDataGridView.Rows ) {
                        if ( curRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                            if ( curRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "N" ) ) {
                                if ( curRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "N" ) ) {
                                    if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                        try {
                                            scoreMeters = Convert.ToDecimal( curRow.Cells["ScoreMetersRecap"].Value.ToString() );
                                        } catch {
                                            scoreMeters = 0;
                                        }
                                        if ( skierScoreMeters > scoreMeters ) {
                                            curMessage += "  Required reride for fast speed overridden by longer jump.";
                                            curGoodPassCount++;
                                        }
                                    } else {
                                        try {
                                            scoreFeet = Convert.ToDecimal( curRow.Cells["ScoreFeetRecap"].Value.ToString() );
                                        } catch {
                                            scoreFeet = 0;
                                        }
                                        if ( skierScoreFeet > scoreFeet ) {
                                            try {
                                                curTolSplit82Time = Convert.ToInt16( curRow.Cells["Split82TimeTolRecap"].Value );
                                            } catch {
                                                curTolSplit82Time = 0;
                                            }
                                            try {
                                                curTolEndTime = Convert.ToInt16( curRow.Cells["Split41TimeTolRecap"].Value );
                                            } catch {
                                                curTolEndTime = 0;
                                            }
                                            if (curTolEndTime > 0 && curTolSplit82Time < 0) {
                                                curRow.Cells["ScoreProtRecap"].Value = "Y";
                                                curCountOptions++;
                                                if ((curAcptPassCount - curCountOptions) < 3) {
                                                    curMessage += "\n Optional reride is still available";
                                                    curReturnValue = true;
                                                }
                                            } else {
                                                curMessage += "\n Required reride for fast speed overridden by longer jump.";
                                                curGoodPassCount++;
                                            }
                                        }
                                    }
                                } else {
                                }
                            } else {
                            }
                        } else {
                            if ( curRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "N" ) ) {
                                curSkierStatus = "2-InProg";
                                curCountOptions++;
                                if ((curAcptPassCount - curCountOptions) < 3) {
                                    curMessage = "Round complete"
                                        + "\n Optional reride is available";
                                    curReturnValue = true;
                                }
                            }
                        }
                    }
                    if ( curGoodPassCount >= 3 ) {
                        //curMessage = "Round complete";
                        curSkierStatus = "4-Done";
                    }

                    skierPassMsg.Text = curMessage;
                }
                setEventRegRowStatus( curSkierStatus );
                int rowsProc = 0;
                Int64 curScorePK = 0;
                try {
                    curScorePK = (Int64)myScoreRow["PK"];
                } catch {
                    curScorePK = -1;
                }
                if ( curScorePK > 0 ) {
                    try {
                        String curStatus = "TBD", curNote = "";
                        try {
                            String curValue = curSkierStatus;
                            if ( curValue.Equals( "4-Done" ) ) curStatus = "Complete";
                            if ( curValue.Equals( "2-InProg" ) ) curStatus = "InProg";
                            if ( curValue.Equals( "3-Error" ) ) curStatus = "Error";
                            if ( curValue.Equals( "1-TBD" ) ) curStatus = "TBD";
                        } catch {
                            curStatus = "TBD";
                        }
                        try {
                            curNote = noteTextBox.Text;
                            curNote = curNote.Replace( "'", "''" );
                        } catch {
                            curNote = "";
                        }

                        StringBuilder curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update JumpScore Set " );
                        curSqlStmt.Append( "  Status = '" + curStatus + "'" );
                        curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                        curSqlStmt.Append( ", Note = '" + curNote + "'" );
                        curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    } catch ( Exception excp ) {
                        curReturnValue = false;
                        String curMsg = ":Error while attempting to determine if the skier is done \n" + excp.Message;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                }
                isDataModified = false;

            }

            return curReturnValue;
        }

        private void addRecapRowTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( addRecapRowTimer );
            addPass();
        }

        private void addPass() {
            Int16 curPassNum = 0, curBoatSpeed;
            Decimal curRampHeight;
            String curRTBValue = "N";
            isAddRecapRowInProg = true;
            isRecapRowEnterHandled = false;
            addPassButton.Enabled = false;
            DataGridViewRow curEventRegRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curEventRegRow.Cells["AgeGroup"].Value;

            curRampHeight = RampHeightSelect.CurrentValue;
            curBoatSpeed = JumpSpeedSelect.CurrentValue;

            if (jumpRecapDataGridView.Rows.Count > 0) {
                addPassButton.Enabled = false;
                myRecapRow = (DataGridViewRow)jumpRecapDataGridView.Rows[jumpRecapDataGridView.Rows.Count - 1];
                curRTBValue = myRecapRow.Cells["ReturnToBaseRecap"].Value.ToString();
                curPassNum = Convert.ToInt16( jumpRecapDataGridView.Rows.Count + 1 );
            } else {
                //curAddPass = false;
                /*
                StringBuilder curSqlStmt = new StringBuilder( "" );
                Int16 tempRound = Convert.ToInt16( roundSelect.RoundValue );
                if ( tempRound > 1 && tempRound < 25) {
                    curSqlStmt.Append( "SELECT R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd, COUNT(*) AS NumSkierEvents " );
                    curSqlStmt.Append( "FROM TourReg AS R " );
                    curSqlStmt.Append( "  INNER JOIN EventReg AS E ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
                    curSqlStmt.Append( "  LEFT OUTER JOIN NopsData AS N ON R.AgeGroup = N.AgeGroup AND N.Event = 'Slalom' AND N.SkiYear = SUBSTRING(R.SanctionId, 1, 2) " );
                    curSqlStmt.Append( "WHERE R.SanctionId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SanctionId"].Value.ToString() + "' " );
                    curSqlStmt.Append( "  And R.MemberId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString() + "' " );
                    curSqlStmt.Append( "  And R.AgeGroup = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() + "' " );
                    curSqlStmt.Append( "GROUP BY R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd " );
                    curSqlStmt.Append( "HAVING COUNT(*) >= N.EventsReqd " );
                    curSqlStmt.Append( "ORDER BY R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd " );
                    DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if ( curDataTable.Rows.Count > 0 ) {
                        curSqlStmt = new StringBuilder("");
                        curSqlStmt.Append("SELECT PK FROM JumpScore ");
                        curSqlStmt.Append( "WHERE SanctionId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SanctionId"].Value.ToString() + "'" );
                        curSqlStmt.Append( "  And MemberId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString() + "' " );
                        curSqlStmt.Append( "  And AgeGroup = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() + "' " );
                        curSqlStmt.Append( "  And Round < " + roundSelect.RoundValue );
                        DataTable curDataTable2 = DataAccess.getDataTable( curSqlStmt.ToString() );
                        if ( curDataTable2.Rows.Count == 0 ) {
                            MessageBox.Show( "This skier participates in overall"
                                + "\nYou are entering a score in round " + roundSelect.RoundValue
                                + "\nThis skier does not have a round 1 score"
                                + "\nYou should consider entering this score in round 1 to ensure overall is computed properly"
                                );
                        } else if ( curDataTable2.Rows.Count == ( tempRound - 1 ) ) {
                            //curAddPass = true;
                        } else {
                            MessageBox.Show( "This skier participates in overall"
                                + "\nYou are entering a score in round " + roundSelect.RoundValue
                                + "\nThis skier does not have scores for all earlier rounds"
                                + "\nYou should consider entering this score in an earlier round to ensure overall is computed properly"
                                );
                        }
                    }
                }
                */
                scoreEntryInprogress();
                curPassNum = 1;

                if (ExportLiveScoreboard.ScoreboardLocation.Length > 1) {
                    String curSkierName = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkierName"].Value;
                    String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                    ExportLiveScoreboard.exportCurrentSkierJump(mySanctionNum, curMemberId, curAgeGroup, curTeamCode, Convert.ToByte(roundSelect.RoundValue),
                        curSkierName, JumpSpeedSelect.CurrentValueDesc, RampHeightSelect.CurrentValueDesc, curPassNum.ToString("#0"), null, null, null, null, null, null);
                }
            }
            int curViewIdx = jumpRecapDataGridView.Rows.Add();
            myRecapRow = jumpRecapDataGridView.Rows[curViewIdx];

            myRecapRow.Cells["PKRecap"].Value = "-1";
            myRecapRow.Cells["SanctionIdRecap"].Value = mySanctionNum;
            myRecapRow.Cells["MemberIdRecap"].Value = curMemberId;
            myRecapRow.Cells["AgeGroupRecap"].Value = curAgeGroup;
            myRecapRow.Cells["RoundRecap"].Value = roundSelect.RoundValue;
            myRecapRow.Cells["PassNumRecap"].Value = curPassNum.ToString("#0");
            myRecapRow.Cells["BoatSpeedRecap"].Value = curBoatSpeed.ToString("00");
            myRecapRow.Cells["RampHeightRecap"].Value = curRampHeight.ToString("0.0");

            myRecapRow.Cells["Meter1Recap"].Value = "";
            myRecapRow.Cells["Meter2Recap"].Value = "";
            myRecapRow.Cells["Meter3Recap"].Value = "";
            myRecapRow.Cells["Meter4Recap"].Value = "";
            myRecapRow.Cells["Meter5Recap"].Value = "";
            myRecapRow.Cells["Meter6Recap"].Value = "";
            myRecapRow.Cells["ScoreTriangleRecap"].Value = "";
            myRecapRow.Cells["ScoreFeetRecap"].Value = "";
            myRecapRow.Cells["ScoreMetersRecap"].Value = "";

            myRecapRow.Cells["BoatSplitTimeRecap"].Value = "";
            myRecapRow.Cells["BoatSplitTime2Recap"].Value = "";
            myRecapRow.Cells["BoatEndTimeRecap"].Value = "";

            myRecapRow.Cells["Split52TimeTolRecap"].Value = "0";
            myRecapRow.Cells["Split82TimeTolRecap"].Value = "0";
            myRecapRow.Cells["Split41TimeTolRecap"].Value = "0";

            myRecapRow.Cells["RerideRecap"].Value = "N";
            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
            myRecapRow.Cells["ReturnToBaseRecap"].Value = curRTBValue;

            myRecapRow.Cells["ResultsRecap"].Value = "Jump";
            myRecapRow.Cells["RerideReasonRecap"].Value = "";
            myRecapRow.Cells["NoteRecap"].Value = "";
            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.clock;
            myRecapRow.Cells["Updated"].Value = "N";
            isDataModified = false;
            isAddRecapRowInProg = false;

            myPassRunCount++;
            if ( curViewIdx == 0 ) {
                mySkierRunCount++;
            } else {
                if ( mySkierRunCount == 0 ) {
                    mySkierRunCount++;
                }
            }

            curViewIdx = 0;
            int curViewIdxMax = jumpRecapDataGridView.Rows.Count - 1;
            foreach ( DataGridViewRow curViewRow in jumpRecapDataGridView.Rows ) {
                curViewIdx = curViewRow.Index;
                if ( curViewIdx < curViewIdxMax ) {
                    foreach ( DataGridViewCell curCell in curViewRow.Cells ) {
                        if ( curCell.Visible && !( curCell.ReadOnly ) ) {
                            if ( !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideRecap" ) )
                                && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "RerideReasonRecap" ) )
                                && !( jumpRecapDataGridView.Columns[curCell.ColumnIndex].Name.Equals( "ScoreProtRecap" ) )
                                ) {
                                curCell.ReadOnly = true;
                            }
                        }
                    }
                }
            }

            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 15;
            curTimerObj.Tick += new EventHandler( jumpRecapDataGridView_SetNewRowCell );
            curTimerObj.Start();
        }

        private String CalcDistValidate( Decimal inScoreFeet, Decimal inScoreMeters ) {
            String returnMsg = "";
            if ( ( inScoreFeet > 0 ) && ( inScoreMeters > 0 ) ) {
                Decimal convScoreMeters = Convert.ToDecimal( inScoreFeet / myMetericFactor );
                convScoreMeters = Math.Round( convScoreMeters, 1 );
                Decimal convScoreMetersMin = convScoreMeters - .55M;
                Decimal convScoreMetersMax = convScoreMeters + .55M;
                if ( ( inScoreMeters >= convScoreMetersMin ) && ( inScoreMeters <= convScoreMetersMax ) ) {
                    returnMsg = "";
                } else {
                    Decimal convScoreFeet = Convert.ToDecimal( inScoreMeters * myMetericFactor );
                    returnMsg = "Meters don't convert to recorded feet plus or minus .55M" 
                    + "\n Input " + inScoreMeters + " meters converts to " + convScoreFeet.ToString( "##0" ) + " feet"
                    + "\n Input " + inScoreFeet + " feet converts to " + convScoreMeters.ToString( "##0.0" ) + " meters"
                    ;
                }
            } else {
                returnMsg = "";
            }
            return returnMsg;
        }

        private bool CalcDist() {
            bool curReturnValue = false;
            Decimal scoreFeet = 0, scoreMeters = 0;
            try {
                scoreFeet = Convert.ToDecimal( (String)myRecapRow.Cells["ScoreFeetRecap"].Value );
            } catch {
                scoreFeet = 0;
            }
            try {
                scoreMeters = Convert.ToDecimal((String)myRecapRow.Cells["ScoreMetersRecap"].Value);
            } catch {
                scoreMeters = 0;
            }
            Int16 curBoatSpeed = Convert.ToInt16( myRecapRow.Cells["BoatSpeedRecap"].Value.ToString() );
            Decimal curRampHeight = Convert.ToDecimal(myRecapRow.Cells["RampHeightRecap"].Value.ToString());

            if ( ( scoreFeet > 0 ) && ( scoreMeters > 0 )
                && myRecapRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" ) ) {
                if ( myRecapRow.Cells["RerideRecap"].Value.ToString().Equals( "Y" ) ) {
                    if ( myRecapRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
                        if ( myRecapRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" ) ) {
                            curReturnValue = CalcSkierScore();
                        }
                    } else {
                        if ( myRecapRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" ) ) {
                            curReturnValue = CalcSkierScore();
                        }
                    }
                } else {
                    if ( myRecapRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" ) ) {
                        curReturnValue = CalcSkierScore();
                    } else {
                        if ( myRecapRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" ) ) {
                            curReturnValue = CalcSkierScore();
                        }
                    }
                }
            } else {
                if ( ( scoreFeet > 0 ) && ( scoreMeters > 0 ) ) {
                    curReturnValue = CalcSkierScore();
                }
            }
            return curReturnValue;
        }

        private bool CalcSkierScore() {
            bool curReturnValue = true;
            Decimal skierScoreFeetBest = 0, skierScoreMetersBest = 0, skierScoreFeetFastBest = 0, skierScoreMetersFastBest = 0, skierScoreFeetFastMax = 0, skierScoreMetersFastMax = 0;
            Decimal scoreFeet, scoreMeters;
            Int16 curTolSplit82Time = 0, curTolSplit52Time = 0, curTolEndTime = 0, curOptCount = 0;
            Int16 skierBoatSpeed = 0, skierBoatSpeedBest = 0, skierBoatSpeedFastBest = 0;

            String curAgeGroup = (String)myRecapRow.Cells["AgeGroupRecap"].Value;

            if ( jumpRecapDataGridView.Rows.Count > 0 ) {
                foreach (DataGridViewRow curRow in jumpRecapDataGridView.Rows) {
                    #region Get score and time tolerance values
                    if (curRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )) {
                        try {
                            scoreFeet = Convert.ToDecimal( (String)curRow.Cells["ScoreFeetRecap"].Value );
                        } catch {
                            scoreFeet = 0;
                        }
                        try {
                            scoreMeters = Convert.ToDecimal( (String)curRow.Cells["ScoreMetersRecap"].Value );
                        } catch {
                            scoreMeters = 0;
                        }
                        try {
                            curTolSplit82Time = Convert.ToInt16( curRow.Cells["Split82TimeTolRecap"].Value );
                        } catch {
                            curTolSplit82Time = 0;
                        }
                        try {
                            curTolSplit52Time = Convert.ToInt16( curRow.Cells["Split52TimeTolRecap"].Value );
                        } catch {
                            curTolSplit52Time = 0;
                        }
                        try {
                            curTolEndTime = Convert.ToInt16( curRow.Cells["Split41TimeTolRecap"].Value );
                        } catch {
                            curTolEndTime = 0;
                        }
                    } else {
                        scoreFeet = 0;
                        scoreMeters = 0;
                        curTolSplit82Time = 0;
                        curTolSplit52Time = 0;
                        curTolEndTime = 0;
                    }
                    try {
                        skierBoatSpeed = Convert.ToInt16( curRow.Cells["BoatSpeedRecap"].Value );
                    } catch {
                        skierBoatSpeed = 0;
                    }
                    #endregion

                    if ((scoreFeet > 0) && (scoreMeters > 0) && curRow.Cells["ResultsRecap"].Value.ToString().Equals( "Jump" )) {
                        if (curRow.Cells["TimeInTolRecap"].Value.ToString().Equals( "Y" )) {
                            if (curRow.Cells["RerideRecap"].Value.ToString().Equals( "N" )) {
                                #region Good scoring jump
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    if (scoreMeters > skierScoreMetersBest) {
                                        if (( curRow.Index > (curOptCount + 2) ) && ( skierScoreMetersFastBest > 0 )) {
                                            if (scoreMeters > skierScoreMetersFastBest) {
                                                skierScoreFeetBest = skierScoreFeetFastBest;
                                                skierScoreMetersBest = skierScoreMetersFastBest;
                                                skierBoatSpeedBest = skierBoatSpeedFastBest;
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        } else {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    }
                                } else {
                                    if (scoreFeet > skierScoreFeetBest) {
                                        if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreFeetFastBest > 0 )) {
                                            if (scoreFeet > skierScoreFeetFastBest) {
                                                skierScoreFeetBest = skierScoreFeetFastBest;
                                                skierScoreMetersBest = skierScoreMetersFastBest;
                                                skierBoatSpeedBest = skierBoatSpeedFastBest;
                                            } else if (scoreFeet < skierScoreFeetFastBest) {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            } else {
                                                if (scoreMeters > skierScoreMetersFastBest) {
                                                    skierScoreFeetBest = skierScoreFeetFastBest;
                                                    skierScoreMetersBest = skierScoreMetersFastBest;
                                                    skierBoatSpeedBest = skierBoatSpeedFastBest;
                                                } else {
                                                    skierScoreFeetBest = scoreFeet;
                                                    skierScoreMetersBest = scoreMeters;
                                                    skierBoatSpeedBest = skierBoatSpeed;
                                                }
                                            }
                                        } else {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                            if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreMetersFastBest > 0 )) {
                                                if (scoreMeters > skierScoreMetersFastBest) {
                                                    skierScoreFeetBest = skierScoreFeetFastBest;
                                                    skierScoreMetersBest = skierScoreMetersFastBest;
                                                    skierBoatSpeedBest = skierBoatSpeedFastBest;
                                                } else {
                                                    skierScoreFeetBest = scoreFeet;
                                                    skierScoreMetersBest = scoreMeters;
                                                    skierBoatSpeedBest = skierBoatSpeed;
                                                }
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            } else {
                                curOptCount++;
                                if (curRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" )) {
                                    #region Good scoring jump
                                    if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                        if (scoreMeters > skierScoreMetersBest) {
                                            if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreMetersFastBest > 0 )) {
                                                if (scoreMeters > skierScoreMetersFastBest) {
                                                    skierScoreFeetBest = skierScoreFeetFastBest;
                                                    skierScoreMetersBest = skierScoreMetersFastBest;
                                                    skierBoatSpeedBest = skierBoatSpeedFastBest;
                                                } else {
                                                    skierScoreFeetBest = scoreFeet;
                                                    skierScoreMetersBest = scoreMeters;
                                                    skierBoatSpeedBest = skierBoatSpeed;
                                                }
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        }
                                    } else {
                                        if (scoreFeet > skierScoreFeetBest) {
                                            if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreFeetFastBest > 0 )) {
                                                skierScoreFeetBest = skierScoreFeetFastBest;
                                                skierScoreMetersBest = skierScoreMetersFastBest;
                                                skierBoatSpeedBest = skierBoatSpeedFastBest;
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        } else {
                                            if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                                if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreMetersFastBest > 0 )) {
                                                    if (scoreMeters > skierScoreMetersFastBest) {
                                                        skierScoreFeetBest = skierScoreFeetFastBest;
                                                        skierScoreMetersBest = skierScoreMetersFastBest;
                                                        skierBoatSpeedBest = skierBoatSpeedFastBest;
                                                    } else {
                                                        skierScoreFeetBest = scoreFeet;
                                                        skierScoreMetersBest = scoreMeters;
                                                        skierBoatSpeedBest = skierBoatSpeed;
                                                    }
                                                } else {
                                                    skierScoreFeetBest = scoreFeet;
                                                    skierScoreMetersBest = scoreMeters;
                                                    skierBoatSpeedBest = skierBoatSpeed;
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        } else {
                            if (curRow.Cells["ScoreProtRecap"].Value.ToString().Equals( "Y" )) {
                                #region Good scoring jump
                                curOptCount++;
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    if (scoreMeters > skierScoreMetersBest) {
                                        if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreMetersFastBest > 0 )) {
                                            if (scoreMeters > skierScoreMetersFastBest) {
                                                skierScoreFeetBest = skierScoreFeetFastBest;
                                                skierScoreMetersBest = skierScoreMetersFastBest;
                                                skierBoatSpeedBest = skierBoatSpeedFastBest;
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        } else {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    }
                                } else {
                                    if (scoreFeet > skierScoreFeetBest) {
                                        if (( curRow.Index > ( curOptCount + 2 ) ) && ( skierScoreFeetFastBest > 0 )) {
                                            skierScoreFeetBest = skierScoreFeetFastBest;
                                            skierScoreMetersBest = skierScoreMetersFastBest;
                                            skierBoatSpeedBest = skierBoatSpeedFastBest;
                                        } else {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                            if ((curRow.Index > 2) && (skierScoreMetersFastBest > 0)) {
                                                if (scoreMeters > skierScoreMetersFastBest) {
                                                    skierScoreFeetBest = skierScoreFeetFastBest;
                                                    skierScoreMetersBest = skierScoreMetersFastBest;
                                                    skierBoatSpeedBest = skierBoatSpeedFastBest;
                                                } else {
                                                    skierScoreFeetBest = scoreFeet;
                                                    skierScoreMetersBest = scoreMeters;
                                                    skierBoatSpeedBest = skierBoatSpeed;
                                                }
                                            } else {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            if (curTolSplit82Time == 0 && curTolSplit52Time > 0 && curTolEndTime > 0) {
                                #region Fast 52M and Fast 41M, optional but score not protected
                                curOptCount++;
                                if (jumpRecapDataGridView.Rows.Count <= 3) {
                                    if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                        if (scoreMeters > skierScoreMetersBest) {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet > skierScoreFeetBest) {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        } else {
                                            if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            } else if (curTolSplit82Time > 0 && curTolSplit52Time < 0 && curTolEndTime == 0) {
                                #region Fast 82M segment, Slow 52M segment, good end time, scoring jump, mandatory reride if best and can't improve score
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    if (scoreMeters > skierScoreMetersBest) {
                                        if (scoreMeters > skierScoreMetersFastBest) {
                                            skierScoreFeetFastBest = scoreFeet;
                                            skierScoreMetersFastBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    }
                                } else {
                                    if (scoreFeet > skierScoreFeetBest) {
                                        if (scoreFeet > skierScoreFeetFastBest) {
                                            skierScoreFeetFastBest = scoreFeet;
                                            skierScoreMetersFastBest = scoreMeters;
                                            skierScoreFeetFastMax = scoreFeet;
                                            skierScoreMetersFastMax = scoreMeters;
                                            skierBoatSpeedFastBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                            if (scoreMeters > skierScoreMetersFastBest) {
                                                skierScoreFeetFastBest = scoreFeet;
                                                skierScoreMetersFastBest = scoreMeters;
                                                skierScoreFeetFastMax = scoreFeet;
                                                skierScoreMetersFastMax = scoreMeters;
                                                skierBoatSpeedFastBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            } else if (curTolSplit82Time == 0 && curTolSplit52Time == 0 && curTolEndTime > 0) {
                                #region Fast end time scoring jump, mandatory reride if best and score not protected, improvement allowed for IWWF but not for AWSA
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                } else {
                                    if (scoreFeet > skierScoreFeetBest) {
                                        if (scoreFeet > skierScoreFeetFastBest) {
                                            skierScoreFeetFastBest = scoreFeet;
                                            skierScoreMetersFastBest = scoreMeters;
                                            skierScoreFeetFastMax = scoreFeet;
                                            skierScoreMetersFastMax = scoreMeters;
                                            skierBoatSpeedFastBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                            if (scoreMeters > skierScoreMetersFastBest) {
                                                skierScoreFeetFastBest = scoreFeet;
                                                skierScoreMetersFastBest = scoreMeters;
                                                skierScoreFeetFastMax = scoreFeet;
                                                skierScoreMetersFastMax = scoreMeters;
                                                skierBoatSpeedFastBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            } else if (curTolSplit82Time == 0 && curTolSplit52Time < 0 && curTolEndTime >= 0) {
                                #region Slow 52M segment, Fast or good end time scoring jump, mandatory reride if best and can't improve score
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    if (scoreMeters > skierScoreMetersBest) {
                                        if (scoreMeters > skierScoreMetersFastBest) {
                                            skierScoreFeetFastBest = scoreFeet;
                                            skierScoreMetersFastBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    }
                                } else {
                                    if (scoreFeet > skierScoreFeetBest) {
                                        if (scoreFeet > skierScoreFeetFastBest) {
                                            skierScoreFeetFastBest = scoreFeet;
                                            skierScoreMetersFastBest = scoreMeters;
                                            skierScoreFeetFastMax = scoreFeet;
                                            skierScoreMetersFastMax = scoreMeters;
                                            skierBoatSpeedFastBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                            if (scoreMeters > skierScoreMetersFastBest) {
                                                skierScoreFeetFastBest = scoreFeet;
                                                skierScoreMetersFastBest = scoreMeters;
                                                skierScoreFeetFastMax = scoreFeet;
                                                skierScoreMetersFastMax = scoreMeters;
                                                skierBoatSpeedFastBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            } else if (curTolSplit82Time > 0 || curTolSplit52Time > 0 || curTolEndTime > 0) {
                                #region Fast scoring jump, mandatory reride if best and score not protected, improvement allowed
                                #endregion
                            } else if ( curTolSplit52Time < 0 || curTolEndTime < 0) {
                                #region Slow time optional but score not protected
                                curOptCount++;
                                if (jumpRecapDataGridView.Rows.Count <= 3) {
                                    if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                        if (scoreMeters > skierScoreMetersBest) {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        }
                                    } else {
                                        if (scoreFeet > skierScoreFeetBest) {
                                            skierScoreFeetBest = scoreFeet;
                                            skierScoreMetersBest = scoreMeters;
                                            skierBoatSpeedBest = skierBoatSpeed;
                                        } else {
                                            if (scoreFeet == skierScoreFeetBest && scoreMeters > skierScoreMetersBest) {
                                                skierScoreFeetBest = scoreFeet;
                                                skierScoreMetersBest = scoreMeters;
                                                skierBoatSpeedBest = skierBoatSpeed;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }

                scoreFeetTextBox.Text = skierScoreFeetBest.ToString();
                scoreMetersTextBox.Text = skierScoreMetersBest.ToString();
                BoatSpeedTextBox.Text = skierBoatSpeed.ToString( "00" );

                ScoreList[0].Score = skierScoreFeetBest;
                appNopsCalc.calcNops( curAgeGroup, ScoreList );
                nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();
            } else {
                scoreFeetTextBox.Text = "";
                scoreMetersTextBox.Text = "";
                BoatSpeedTextBox.Text = skierBoatSpeed.ToString( "00" );
                refreshScoreSummaryWindow();
                setJumpScoreEntry( Convert.ToInt16( roundSelect.RoundValue ) );
            }


            return curReturnValue;
        }

        private bool CalcDistByMeters() {
            bool curReturnValue = false;
            Decimal curDistExtd;
            curReturnValue = true;
            myTriangleTolMsg = "";

            try {
                Double numAngleA = Convert.ToDouble( myRecapRow.Cells["Meter1Recap"].Value.ToString() );
                Double numAngleB = Convert.ToDouble( myRecapRow.Cells["Meter2Recap"].Value.ToString() );
                Double numAngleC = Convert.ToDouble( myRecapRow.Cells["Meter3Recap"].Value.ToString() );
                Int32[] calcDistResults = myJumpCalc.calcDistance( numAngleA, numAngleB, numAngleC );
                Int32 curDist = calcDistResults[0];
                if ( myMeterDistTol < myJumpCalc.TriangleJump ) {
                    myTriangleTolMsg = "Wide triangle, from = " + calcDistResults[2].ToString( "##0" ) + " to " + calcDistResults[1].ToString( "##0" );
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                    curDist = calcDistResults[2];
                    curDistExtd = calcDistResults[4];
                } else {
                    myRecapRow.Cells["RerideRecap"].Value = "N";
                    curDistExtd = calcDistResults[3];
                }
                Decimal convScoreMeters = ( curDistExtd / 100 ) / myMetericFactor;
                myRecapRow.Cells["ScoreFeetRecap"].Value = curDist.ToString( "##0" );
                myRecapRow.Cells["ScoreMetersRecap"].Value = Math.Round( convScoreMeters, 1 ).ToString( "##0.#" );
                myRecapRow.Cells["ScoreTriangleRecap"].Value = myJumpCalc.TriangleJump.ToString( "##0.##" );
            } catch ( Exception exp ) {
                MessageBox.Show( "Exception encountered when calculating distance using meter readings \n\n " + exp.Message );
                curReturnValue = false;
            }

            skierPassMsg.Text = myTimeTolMsg;
            if ( myTriangleTolMsg.Length > 0 ) skierPassMsg.Text += " : " + myTriangleTolMsg;
            myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
            return curReturnValue;
        }

        private void TimeValidate() {
            Int16 curTolSplit82Time = 0, curTolSplit52Time = 0, curTolEndTime = 0, curTolSplit82TimeMax = 0, curTolSplit52TimeMax = 0, curTolEndTimeMax = 0;
            Decimal curBoatEndTime, curBoatSplit52Time, curBoatSplit82Time;
            String curTimeKey82, curTimeKey41, curTimeKey52, curTimeKey82Max = "", curTimeKey41Max = "", curTimeKey52Max = ""; 
            DataRow curTimeMaxSpeedRow = null;
            Decimal curMinTime, curMaxTime, curActualTime;

            myTimeTolMsg = "";
            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapRow.Cells["ResultsRecap"].Value;
            String curRtb = (String)myRecapRow.Cells["ReturnToBaseRecap"].Value;
            String curDiv = (String)myRecapRow.Cells["AgeGroupRecap"].Value;

            #region For class L,R tournaments get IWWF equivalent divisions
            DataRow[] curJump3TimesDiv = null;
            if ( myJump3TimesDivDataTable != null ) {
                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                    curJump3TimesDiv = myJump3TimesDivDataTable.Select( "ListCode = '" + curDiv + "'" );
                    if (curJump3TimesDiv.Length == 0) {
                        ArrayList curIwwfEligDiv = new ArrayList();
                        Int16 curSkiYearAge = 0;
                        try {
                            curSkiYearAge = Convert.ToInt16( (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SkiYearAge"].Value );
                        } catch {
                            curSkiYearAge = 0;
                        }
                        if ( curSkiYearAge == 0 ) {
                            curSkiYearAge = (short)myAgeDivList.getMinAgeForDiv(curDiv);
                        }
                        if ( curSkiYearAge > 0) {
                            curIwwfEligDiv = myAgeDivList.getDivListForAgeIwwf( curSkiYearAge, curDiv );
                        }
                        if ( curIwwfEligDiv.Count > 0 ) {
                            foreach ( DataRow curRow in myJump3TimesDivDataTable.Rows ) {
                                foreach ( String curEligDiv in curIwwfEligDiv ) {
                                    if ( curEligDiv.Substring(0, 2).Equals((String) curRow["ListCode"]) ) {
                                        if ( curJump3TimesDiv.Length > 0 ) {
                                            if ( (Decimal) curJump3TimesDiv[0]["MaxValue"] > (Decimal) curRow["MaxValue"] ) {
                                                curJump3TimesDiv = myJump3TimesDivDataTable.Select("ListCode = '" + (String) curRow["ListCode"] + "'");
                                            }
                                        } else {
                                            curJump3TimesDiv = myJump3TimesDivDataTable.Select("ListCode = '" + (String) curRow["ListCode"] + "'");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (curJump3TimesDiv == null) {
                curJump3TimesDiv = myMaxRampDataTable.Select( "ListCode = 'n/a'" );
            }
            #endregion

            #region Get score and tolerance data needed for making time validation determinations.
            Decimal curScoreMeters = 0;
            try {
                curScoreMeters = Convert.ToDecimal((String)myRecapRow.Cells["ScoreMetersRecap"].Value);
            } catch {
                curScoreMeters = 0;
            }
            String curEventClass = getSkierClass();
            try {
                curBoatEndTime = Convert.ToDecimal( (String)myRecapRow.Cells["BoatEndTimeRecap"].Value);
            } catch ( Exception exp ) {
                curBoatEndTime = 0;
            }
            try {
                curBoatSplit52Time = Convert.ToDecimal( (String)myRecapRow.Cells["BoatSplitTimeRecap"].Value );
            } catch ( Exception exp ) {
                curBoatSplit52Time = 0;
            }
            try {
                curBoatSplit82Time = Convert.ToDecimal( (String)myRecapRow.Cells["BoatSplitTime2Recap"].Value );
            } catch (Exception exp) {
                curBoatSplit82Time = 0;
            }
            #endregion

            //Change to class C boat times when boat speed is less than max and skier class great than class C
            if ( curBoatSpeed < myMaxSpeed ) {
                if ( (Decimal)myClassCRow["ListCodeNum"] < (Decimal)myClassRow["ListCodeNum"] ) {
                    curEventClass = "C";
                }
            }
            if ( curBoatSpeed != JumpSpeedSelect.CurrentValue ) {
                JumpSpeedSelect.CurrentValue = curBoatSpeed;
                JumpSpeedSelect_Change( null, null );
            }

            //-----------------------------------
            //Validate 82M segment time
            //-----------------------------------
            #region Validate 82M segment time
            try {
                curTimeKey82 = curBoatSpeed.ToString() + "-" + curEventClass;
                if (curResults.ToLower().Equals("pass")) {
                    curTimeKey82 = curTimeKey82 + "-Balk";
                } else {
                    curTimeKey82 = curTimeKey82 + "-82M";
                    if (curBoatSpeed < myMaxSpeed) {
                        curTimeKey82Max = myMaxSpeed.ToString() + "-" + curEventClass + "-52M";
                    }
                }
                myTimeRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey82 + "'" )[0];
                curMinTime = (Decimal)myTimeRow["MinValue"];
                curMaxTime = (Decimal)myTimeRow["MaxValue"];
                curActualTime = Convert.ToDecimal((String)myTimeRow["CodeValue"]);
                if ( curBoatSplit82Time > curMaxTime ) {
                    curTolSplit82Time = -1; //Slow split time
                } else if ( curBoatSplit82Time < curMinTime ) {
                    curTolSplit82Time = 1; //Fast split time
                    if (curBoatSpeed < myMaxSpeed) {
                        curTimeMaxSpeedRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey82Max + "'" )[0];
                        curMinTime = (Decimal)curTimeMaxSpeedRow["MinValue"];
                        if ( curBoatSplit82Time < curMinTime ) {
                            curTolSplit82TimeMax = 1; //Fast split for max speed
                        }
                    }
                } else {
                    curTolSplit82Time = 0;
                }
            } catch (Exception exp) {
                MessageBox.Show("Exception encountered for 82M segment time \n " + exp.Message);
                curTolSplit82Time = 0;
            }
            myRecapRow.Cells["Split82TimeTolRecap"].Value = (curTolSplit82Time + curTolSplit82TimeMax).ToString();
            #endregion

            //-----------------------------------
            //Validate first segment time
            //-----------------------------------
            #region Validate 52M segment time
            try {
                curTimeKey52 = curBoatSpeed.ToString() + "-" + curEventClass + "-52M";
                if ( curBoatSpeed < myMaxSpeed ) {
                    curTimeKey52Max = myMaxSpeed.ToString() + "-" + curEventClass + "-52M";
                }
                DataRow[] curTimeRowResults = myTimesDataTable.Select( "ListCode = '" + curTimeKey52 + "'" );
                if ( curTimeRowResults.Length > 0 ) {
                    myTimeRow = curTimeRowResults[0];
                    curMinTime = (Decimal)myTimeRow["MinValue"];
                    curMaxTime = (Decimal)myTimeRow["MaxValue"];
                    curActualTime = Convert.ToDecimal( (String)myTimeRow["CodeValue"] );
                    if ( curBoatSplit52Time > curMaxTime ) {
                        curTolSplit52Time = -1; //Slow split time
                    } else if ( curBoatSplit52Time < curMinTime ) {
                        curTolSplit52Time = 1; //Fast split time
                        if ( curBoatSpeed < myMaxSpeed ) {
                            curTimeMaxSpeedRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey82Max + "'" )[0];
                            curMinTime = (Decimal)curTimeMaxSpeedRow["MinValue"];
                            if ( curBoatSplit52Time < curMinTime ) {
                                curTolSplit52TimeMax = 1; //Fast split for max speed
                            }
                        }
                    } else {
                        curTolSplit52Time = 0;
                    }
                } else {
                    curTolSplit52Time = 1; //Default to fast split time
                }
            } catch ( Exception exp ) {
                MessageBox.Show( "Exception encountered for 52M segment split time \n " + exp.Message );
                curTolSplit52Time = 0;
            }
            if (curJump3TimesDiv.Length > 0) {
                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                    myRecapRow.Cells["Split52TimeTolRecap"].Value = (curTolSplit52Time + curTolSplit52TimeMax).ToString();
                } else {
                    myRecapRow.Cells["Split52TimeTolRecap"].Value = "0";
                }
            } else {
                myRecapRow.Cells["Split52TimeTolRecap"].Value = "0";
            }
            #endregion

            //-----------------------------------
            //Validate 41M segment (end) time
            //-----------------------------------
            #region Validate 41M segment time
            try {
                if (curResults.ToLower().Equals( "pass" )) {
                        curTolEndTime = 0;
                } else {
                    curTimeKey41 = curBoatSpeed.ToString() + "-" + curEventClass + "-41M";
                    if (curRtb.Equals("Y")) {
                        curTimeKey41 = curTimeKey41 + "-RTB";
                        if (curBoatSpeed < myMaxSpeed) {
                            curTimeKey41Max = myMaxSpeed.ToString() + "-" + curEventClass + "-41M-RTB";
                        } else {
                            curTimeKey41Max = myMaxSpeed.ToString() + "-" + curEventClass + "-41M";
                        }
                    } else {
                        if (curBoatSpeed < myMaxSpeed) {
                            curTimeKey41Max = myMaxSpeed.ToString() + "-" + curEventClass + "-41M";
                        }
                    }
                    myTimeRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey41 + "'" )[0];
                    curMinTime = (Decimal)myTimeRow["MinValue"];
                    curMaxTime = (Decimal)myTimeRow["MaxValue"];
                    curActualTime = Convert.ToDecimal((String)myTimeRow["CodeValue"]);
                    if (curBoatEndTime > curMaxTime) {
                        curTolEndTime = -1; //Slow end course time
                    } else if (curBoatEndTime < curMinTime) {
                        curTolEndTime = 1; //Fast end course time
                        if (curBoatSpeed < myMaxSpeed) {
                            curTimeMaxSpeedRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey41Max + "'" )[0];
                            curMinTime = (Decimal)curTimeMaxSpeedRow["MinValue"];
                            if (curBoatEndTime < curMinTime) {
                                curTolEndTimeMax = 1; //Fast end course time for max speed
                            }
                        } else {
                            if (curRtb.Equals( "Y" )) {
                                curTimeMaxSpeedRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey41Max + "'" )[0];
                                curMinTime = (Decimal)curTimeMaxSpeedRow["MinValue"];
                                if (curBoatEndTime < curMinTime) {
                                    curTolEndTimeMax = 1; //Fast end course time for fast second segment speed
                                }
                            }
                        }
                    } else {
                        curTolEndTime = 0;
                    }
                }
            } catch (Exception exp) {
                MessageBox.Show("Exception encountered for end time \n " + exp.Message);
                curTolEndTime = 0;
            }
            myRecapRow.Cells["Split41TimeTolRecap"].Value = (curTolEndTime + curTolEndTimeMax).ToString();
            #endregion

            if (curTolSplit82Time == 0) {
                if ( curTolEndTime == 0 ) {
                    #region 82M good, 41M good (processing confirmed 6/7/14)
                    myRecapRow.Cells["TimeInTolRecap"].Value = "Y";
                    myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                    myRecapRow.Cells["RerideRecap"].Value = "N";
                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;

                    if (curBoatSpeed >= myMaxSpeed) {
                        if (curJump3TimesDiv.Length > 0) {
                            if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                if (curTolSplit52Time == 0) {
                                } else if (curTolSplit52Time > 0) {
                                    myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                    myTimeTolMsg = "Fast 52M segment, optional reride with protected score (See IWWF 13.04)";
                                } else if (curTolSplit52Time < 0) {
                                    myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                    myTimeTolMsg = "Slow 52M segment, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                }
                            }
                        }
                    } else {
                        if (curJump3TimesDiv.Length > 0) {
                            if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                if (curTolSplit52Time == 0) {
                                } else if (curTolSplit52Time > 0) {
                                    if (curTolSplit52TimeMax > 0) {
                                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                        myTimeTolMsg = "Fast 52M segment for max speed, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                    } else {
                                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                        myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                        myTimeTolMsg = "Fast 52M segment, optional reride with protected score (See IWWF 13.04)";
                                    }
                                } else if (curTolSplit52Time < 0) {
                                    myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                    myTimeTolMsg = "Slow 52M segment, optional reride, score protected (See IWWF 13.04)";
                                }
                            }
                        }
                    }
                    #endregion
                } else if ( curTolEndTime > 0 ) {
                    #region 82M good, 41M fast (processing confirmed 7/2/14)
                    if (curResults.ToLower().Equals( "fall" )) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Fast 41M end time, immediate optional reride for fall (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Fast 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                        }
                    } else {
                        if (curBoatSpeed >= myMaxSpeed) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                //For class L,R tournaments use IWWF rules
                                myTimeTolMsg = "Fast 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                            } else {
                                //Otherwise use AWSA rules
                                myTimeTolMsg = "Fast 41M end time, mandatory reride if best, no improvement allowed, score not protected (See 9.10.B.4.b.ii and 9.10.B.5.b)";
                            }
                            if (curJump3TimesDiv.Length > 0) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Fast 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, Fast 41M end time, optional reride, score not protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, Fast 41M end time, mandatory reride if best, score not protected, no improvement allowed (See IWWF 13.04)";
                                    }
                                }
                            }
                        } else {
                            if (curTolEndTimeMax > 0) {
                                //82M good, 41M fast for max speed
                                myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                myRecapRow.Cells["RerideRecap"].Value = "Y";
                                myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, no improvement allowed, score not protected (See 9.10.B.4.b.ii and 9.10.B.5.b)";
                                }
                                if (curJump3TimesDiv.Length > 0) {
                                    if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                        if (curTolSplit52Time == 0) {
                                            myTimeTolMsg = "Fast 41M end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                        } else if (curTolSplit52Time > 0) {
                                            if (curTolSplit52TimeMax > 0) {
                                                myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                            } else {
                                                myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 41M end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                            }
                                        } else if (curTolSplit52Time < 0) {
                                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                            myTimeTolMsg = "Slow 52M segment, Fast 41M end time for max speed, mandatory reride if best, score protected, otherwise optional with protected score (See IWWF 13.04)";
                                        }
                                    }
                                }
                            } else {
                                //82M good, 41M fast but good for max speed
                                myRecapRow.Cells["TimeInTolRecap"].Value = "Y";
                                myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                myRecapRow.Cells["RerideRecap"].Value = "N";
                                myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
                                myTimeTolMsg = "Fast 41M end time but good for max speed, jump good";

                                if (curJump3TimesDiv.Length > 0) {
                                    if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                        if (curTolSplit52Time == 0) {
                                            myTimeTolMsg = "Fast 41M end time but good for max speed, jump good";
                                        } else if (curTolSplit52Time > 0) {
                                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                            if (curTolSplit52TimeMax > 0) {
                                                myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time but good for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                                myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                            } else {
                                                myTimeTolMsg = "Fast 52M segment for max speed, Fast 41M end time but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                            }
                                        } else if (curTolSplit52Time < 0) {
                                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                            myTimeTolMsg = "Slow 52M segment, Fast 41M end time but good for max speed, mandatory reride if best, optional reride, score protected (See IWWF 13.04)";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                } else if ( curTolEndTime < 0 ) {
                    #region 82M good, 41M slow (processing confirmed 7/2/14)
                    myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    if (curResults.ToLower().Equals( "fall" )) {
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Slow Fast 41M end time, immediate optional reride for fall (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Slow 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                        }
                    } else {
                        if (curBoatSpeed >= myMaxSpeed) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Slow 41M end time, optional reride, score protected, use RTB slow column if applicable. (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Slow 41M end time, optional reride, score protected (See 9.10.B.5.c), use RTB slow column if applicable. (See 9.17)";
                            }
                            if (curJump3TimesDiv.Length > 0) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 41M end time, optional reride, score protected. (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, Slow 41M end time, optional reride, score protected. (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                        myTimeTolMsg = "Slow 52M segment, Slow 41M end time, optional reride, score not protected. (See IWWF 13.04)";
                                    }
                                }
                            }
                        } else {
                            //82M good, 41M slow, speed below max speed
                            myRecapRow.Cells["TimeInTolRecap"].Value = "Y";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["RerideRecap"].Value = "N";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeInTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Slow 41M end time but no reride because speed less than max (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Slow 41M end time but no reride because speed less than max (See  9.10.B.5.c)";
                            }
                            if (curJump3TimesDiv.Length > 0) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 41M end time but no reride because speed less than max (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                        if (curTolSplit52TimeMax > 0) {
                                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                            myTimeTolMsg = "Fast 52M segment for max speed, Slow 41M end time, mandatory reride if best, otherwise optional with protected score (See IWWF 13.04)";
                                        } else {
                                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                            myRecapRow.Cells["RerideRecap"].Value = "N";
                                            myTimeTolMsg = "Fast 52M segment but not for max speed, Slow 41M end time, optional reride, score protected (See IWWF 13.04)";
                                        }
                                    } else if (curTolSplit52Time < 0) {
                                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                        myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                        myTimeTolMsg = "Slow 52M segment, Slow 41M end time, optional reride, score protected. (See IWWF 13.04)";
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            } else if ( curTolSplit82Time > 0 ) {
                if ( curTolEndTime == 0 ) {
                    #region 82M fast, 41M good (processing confirmed 6/7/14)
                    if (curResults.ToLower().Equals( "fall" )) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Fast 82M segment, immediate optional reride for fall (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Fast 82M segment, immediate optional reride for fall (See 9.10.B.2)";
                        }
                    } else {
                        if (curBoatSpeed >= myMaxSpeed) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Fast 82M segment, mandatory reride if best, improvement allowed, score not protected (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Fast 82M segment, mandatory reride if best, improvement allowed, score not protected (See 9.10(B)(5)(a))";
                            }
                            if (curJump3TimesDiv.Length > 0) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Fast 82M segment, mandatory reride if best, score not protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, Fast 82M segment, mandatory reride if best, score not protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, fast 82M segment, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                    }
                                }
                            }
                        } else {
                            if (curTolSplit82TimeMax > 0) {
                                //82M fast for max speed, 41M good
                                myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                myRecapRow.Cells["RerideRecap"].Value = "Y";
                                myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best jump, score not protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best jump, score not protected (See 9.10.B.2(i))";
                                }
                                if (curJump3TimesDiv.Length > 0) {
                                    if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                        if (curTolSplit52Time == 0) {
                                            myTimeTolMsg = "Fast 82M segment for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                        } else if (curTolSplit52Time > 0) {
                                            if (curTolSplit52TimeMax > 0) {
                                                myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment for max speed, mandatory reride, score not protected (See IWWF 13.04)";
                                            } else {
                                                myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment for max speed, mandatory reride if best jump, score not protected (See IWWF 13.04)";
                                            }
                                        } else if (curTolSplit52Time < 0) {
                                            myTimeTolMsg = "Slow 52M segment, fast 82M segment for max speed, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                        }
                                    }
                                }
                            } else {
                                //82M fast but good for max speed, 41M good
                                myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                                myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                myRecapRow.Cells["RerideRecap"].Value = "Y";
                                myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected (See 9.10(b)(1)(ii)(a))";
                                }
                                if (curJump3TimesDiv.Length > 0) {
                                    if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                        if (curTolSplit52Time == 0) {
                                            myTimeTolMsg = "Fast 82M segment but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                        } else if (curTolSplit52Time > 0) {
                                            if (curTolSplit52TimeMax > 0) {
                                                myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                                myTimeTolMsg = "Fast 52M segment for max speed, Fast 82M segment but good for max speed, mandatory reride if best jump, score not protected (See IWWF 13.04)";
                                            } else {
                                                myTimeTolMsg = "Fast 52M segment but good for max speed, Fast 82M segment but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                            }
                                        } else if (curTolSplit52Time < 0) {
                                            myTimeTolMsg = "Slow 52M segment, Fast 82M segment but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                } else if ( curTolEndTime > 0 ) {
                    #region 82M fast, 41M fast (processing confirmed 6/17/14)
                    if (curBoatSpeed >= myMaxSpeed) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Fast 82M segment, Fast 41M end time, mandatory reride, score not protected (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Fast 82M segment, Fast 41M end time, mandatory reride if best, score not protected (See 9.10.B.2(i))";
                        }
                        if (curJump3TimesDiv.Length > 0) {
                            if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                if (curTolSplit52Time == 0) {
                                    myTimeTolMsg = "Fast 82M segment, Fast 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else if (curTolSplit52Time > 0) {
                                    myTimeTolMsg = "Fast 52M segment, Fast 82M segment, Fast 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else if (curTolSplit52Time < 0) {
                                    myTimeTolMsg = "Slow 52M segment, Fast 82M segment, Fast 41M end time, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                }
                            }
                        }
                    } else {
                        if (curTolSplit82TimeMax > 0) {
                            myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                            myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                            myRecapRow.Cells["RerideRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if (curTolEndTimeMax > 0) {
                                //82M fast for max speed, 41M fast for max speed
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment for max speed, fast end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment for max speed, fast end time for max speed, mandatory reride if best, no improvement allowed, score not protected (See 9.10.B.2(i))";
                                }
                            } else {
                                //82M fast for max speed, 41M fast but good for max speed
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment for max speed, fast end time but good for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment for max speed, fast end time but good for max speed, mandatory reride if best, no improvement allowed, score not protected (See 9.10.B.2(i))";
                                }
                            }
                        } else {
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if (curTolEndTimeMax > 0) {
                                //82M fast but good for max speed, 41M fast for max speed
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast end time for max speed, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast end time for max speed, mandatory reride if best, no improvement allowed, score not protected (See 9.10(b)(1)(ii)(a))";
                                }
                            } else {
                                //82M fast but good for max speed, 41M fast but good for max speed
                                myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast end time but good for max speed, optional reride, score protected (See IWWF 13.04)";
                                } else {
                                    myTimeTolMsg = "Fast 82M segment but good for max speed, fast end time but good for max speed, optional reride, score protected (See 9.10(b)(1)(ii)(a))";
                                }
                            }
                        }
                    }
                    #endregion
                } else if ( curTolEndTime < 0 ) {
                    #region 82M fast, 41M slow (processing confirmed 7/2/12)
                    if (curBoatSpeed >= myMaxSpeed) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Fast 82M segment, Slow 41M end time, mandatory reride, score not protected (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Fast 82M segment, Slow 41M end time, mandatory reride if best, score not protected (See 9.10(B)(5)(a))";
                        }
                        if (curJump3TimesDiv.Length > 0) {
                            if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                if (curTolSplit52Time == 0) {
                                    myTimeTolMsg = "Fast 82M segment, Slow 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else if (curTolSplit52Time > 0) {
                                    myTimeTolMsg = "Fast 52M segment, Fast 82M segment, Slow 41M end time, mandatory reride if best, score not protected (See IWWF 13.04)";
                                } else if (curTolSplit52Time < 0) {
                                    myTimeTolMsg = "Slow 52M segment, Fast 82M segment, Slow 41M end time, mandatory reride if best, no improvement allowed, score not protected (See IWWF 13.04)";
                                }
                            }
                        }
                    } else {
                        if (curTolSplit82TimeMax > 0) {
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Fast 82M segment for max speed, Slow 41M end time, mandatory reride if best, score not protected, otherwise optional with protected score (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Fast 82M segment for max speed, Slow 41M end time, mandatory reride, score not protected (See 9.10 (B)(4)(a))";
                            }
                        } else {
                            myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                            myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Fast 82M segment but good for max speed, Slow 41M end time, optional reride, score protected (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Fast 82M segment but good for max speed, Slow 41M end time, optional reride, score protected (See 9.10 (B)(4)(b)(i))";
                            }
                        }
                    }
                    #endregion
                }
            } else if ( curTolSplit82Time < 0 ) {
                if ( curTolEndTime == 0 ) {
                    #region 82M slow, 41M good (processing confirmed 7/2/14)
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Slow 82M segment, optional reride, score protected (See 9.10(B)(5)(c))";
                        }
                        if (curJump3TimesDiv.Length > 0) {
                            if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                if (curBoatSpeed >= myMaxSpeed) {
                                    //Speed at max, 82M slow, 41M good
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                                        myTimeTolMsg = "Slow 52M segment, slow 82M segment, optional reride, score not protected (See IWWF 13.04)";
                                    }
                                } else {
                                    //Speed below max, 82M slow, 41M good
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        if (curTolSplit52TimeMax > 0) {
                                            myTimeTolMsg = "Fast 52M segment for max speed, Slow 82M segment, mandatory reride if best, otherwise optional reride with score protected (See IWWF 13.04)";
                                        } else {
                                            myTimeTolMsg = "Fast 52M segment but good for max speed, Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                                        }
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, Slow 82M segment, optional reride, score protected (See IWWF 13.04)";
                                    }
                                }
                            }
                        }
                    #endregion
                } else if ( curTolEndTime > 0 ) {
                    #region 82M slow, 41M fast (processing confirmed 7/2/14)
                    if (curBoatSpeed >= myMaxSpeed) {
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if (curResults.ToLower().Equals( "fall" )) {
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Slow 82M segment time, Fast 41M end time, immediate optional reride for fall (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Slow 82M segment time, Fast 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                            }
                        } else {
                            if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                                myTimeTolMsg = "Slow 82M segment time, Fast 41M end time, mandatory reride if best, score not protected, otherwise optional reride, score protected (See IWWF 13.04)";
                            } else {
                                myTimeTolMsg = "Slow 82M segment time, Fast 41M end time, mandatory reride if best, score not protected (See 9.10(B)(5)(b))";
                            }
                            if (curJump3TimesDiv.Length > 0) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 82M segment time, Fast 41M end time, optional reride, score protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, Slow 82M segment time, Fast 41M end time, optional reride, score protected (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, slow 82M segment time, Fast 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    }
                                }
                            }
                        }
                    } else {
                        //82M slow, 41M fast
                        myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        myRecapRow.Cells["RerideRecap"].Value = "Y";
                        myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            if (curTolEndTimeMax > 0) {
                                myTimeTolMsg = "Slow 82M segment, Fast end time for max speed, mandatory reride if best, score not protected, otherwise optional with protected score (See IWWF 13.04)";
                            } else {
                                myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                myTimeTolMsg = "Slow 82M segment, fast end time but good for max speed, optional reride, protected score (See IWWF 13.04)";
                            }
                        } else {
                            if (curTolEndTimeMax > 0) {
                                myTimeTolMsg = "Slow 82M segment, Fast end time for max speed, mandatory reride if best, score not protected, no improvement allowed, otherwise optional with protected score (See 9.10(B)(4)(b)(ii))";
                            } else {
                                myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                                myTimeTolMsg = "Slow 82M segment, fast end time but good for max speed, optional reride, protected score (See 9.10(B)(4)(b)(i))";
                            }
                        }
                    }
                    #endregion
                } else if ( curTolEndTime < 0 ) {
                    #region 82M slow, 41M slow (processing confirmed 7/2/14)
                    myRecapRow.Cells["TimeInTolRecap"].Value = "N";
                    myRecapRow.Cells["ScoreProtRecap"].Value = "Y";
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                    myRecapRow.Cells["TimeInTolImg"].Value = WaterskiScoringSystem.Properties.Resources.TimeOutTol;
                    if (curResults.ToLower().Equals( "fall" )) {
                        myRecapRow.Cells["ScoreProtRecap"].Value = "N";
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Slow 82M segment time, Slow 41M end time, immediate optional reride for fall (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Slow 82M segment time, Slow 41M end time, immediate optional reride for fall (See 9.10.B.2)";
                        }
                    } else {
                        if ((Decimal)myClassRow["ListCodeNum"] > (Decimal)myClassERow["ListCodeNum"]) {
                            myTimeTolMsg = "Slow 82M segment, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                        } else {
                            myTimeTolMsg = "Slow 82M segment, Slow 41M end time, optional reride, protected score (See 9.10(B)(5)(c))";
                        }
                        if (curJump3TimesDiv.Length > 0) {
                            if (curBoatSpeed >= myMaxSpeed) {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        myTimeTolMsg = "Fast 52M segment, slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    }
                                }
                            } else {
                                if (curScoreMeters >= (Decimal)curJump3TimesDiv[0]["MaxValue"]) {
                                    if (curTolSplit52Time == 0) {
                                        myTimeTolMsg = "Slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    } else if (curTolSplit52Time > 0) {
                                        if (curTolSplit52TimeMax > 0) {
                                            myTimeTolMsg = "Fast 52M segment for max speed, Slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                        } else {
                                            myTimeTolMsg = "Fast 52M segment, slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                        }
                                    } else if (curTolSplit52Time < 0) {
                                        myTimeTolMsg = "Slow 52M segment, slow 82M segment time, Slow 41M end time, optional reride, protected score (See IWWF 13.04)";
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            skierPassMsg.Text = myTimeTolMsg;
            if ( myTriangleTolMsg.Length > 0 ) skierPassMsg.Text += " : " + myTriangleTolMsg;
            myRecapRow.Cells["RerideReasonRecap"].Value = skierPassMsg.Text;
            if ( myTriangleTolMsg.Length > 1 ) {
                if ( myRecapRow.Cells["RerideRecap"].Value.Equals( "N" ) ) {
                    myRecapRow.Cells["RerideRecap"].Value = "Y";
                }
            }
        }

        private void SplitTimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            Int16 curBoatSpeed = Convert.ToInt16( (String)myRecapRow.Cells["BoatSpeedRecap"].Value );
            String curResults = (String)myRecapRow.Cells["ResultsRecap"].Value;
            String curEventClass = getSkierClass();
            if ( curBoatSpeed < myMaxSpeed ) {
                //Set to class C times when speed less than max and tournament greater than class C (e.g. class R)
                //Use class N times for all Classes less than class C (e.g. class N, F)
                if ( (Decimal)myClassCRow["ListCodeNum"] < (Decimal)myClassRow["ListCodeNum"] ) {
                    curEventClass = "C";
                }
            }

            //-----------------------------------
            //Validate first segment time
            //-----------------------------------
            try {
                String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass + "-52M";
                myTimeRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" )[0];
                curMinTime = (Decimal)myTimeRow["MinValue"];
                curMaxTime = (Decimal)myTimeRow["MaxValue"];
                curActualTime = Convert.ToDecimal((String)myTimeRow["CodeValue"]);

                curTimeOrigValue = (String)myRecapRow.Cells["BoatSplitTimeRecap"].Value;
                if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                    curTimeOrigValue = curActualTime.ToString( "#0.00" );
                    myRecapRow.Cells["BoatSplitTimeRecap"].Value = curTimeOrigValue;
                } else if ( curTimeOrigValue.ToUpper().Equals("NONE") ) {
                    curTimeOrigValue = .01m.ToString("#0.00");
                    myRecapRow.Cells["BoatSplitTimeRecap"].Value = curTimeOrigValue;
                }

                if ( curTimeOrigValue.Length == 1) {
                    curTimeOrigValue = "0" + curTimeOrigValue;
                }
                if (curTimeOrigValue.Length == 2) {
                    if (!(curTimeOrigValue.Contains("."))) {
                        Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
                        Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
                        tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
                        if ( tempTime < curActualTime ) {
                            tempDiff1 = curActualTime - tempTime;
                        } else {
                            tempDiff1 = tempTime - curActualTime;
                        }
                        tempTime = Convert.ToDecimal( (curDigits + 1).ToString() + "." + curTimeOrigValue );
                        tempDiff2 = tempTime - curActualTime;
                        tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
                        tempDiff3 = curActualTime - tempTime;

                        if ( tempDiff1 < tempDiff2 ) {
                            if ( tempDiff1 < tempDiff3 ) {
                                myRecapRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString("#0.00");
                            } else {
                                myRecapRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        } else {
                            if ( tempDiff2 < tempDiff3 ) {
                                myRecapRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            } else {
                                myRecapRow.Cells["BoatSplitTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        }
                    }
                }
            } catch (Exception exp) {
                MessageBox.Show("Exception encountered for first split time \n " + exp.Message);
            }

        }

        private void Split2TimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            Int16 curBoatSpeed = Convert.ToInt16( myRecapRow.Cells["BoatSpeedRecap"].Value.ToString() );
            String curResults = myRecapRow.Cells["ResultsRecap"].Value.ToString();
            String curEventClass = getSkierClass();
            if ( curBoatSpeed < myMaxSpeed ) {
                //Set to class C times when speed less than max and tournament greater than class C (e.g. class R)
                //Use class N times for all Classes less than class C (e.g. class N, F)
                if ( (Decimal)myClassCRow["ListCodeNum"] < (Decimal)myClassRow["ListCodeNum"] ) {
                    curEventClass = "C";
                }
            }

            //-----------------------------------
            //Validate second segment time
            //-----------------------------------
            try {
                String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass;
                curTimeKey += "-82M";
                myTimeRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" )[0];
                curMinTime = (Decimal)myTimeRow["MinValue"];
                curMaxTime = (Decimal)myTimeRow["MaxValue"];
                curActualTime = ( curMinTime + curMaxTime ) / 2;

                curTimeOrigValue = myRecapRow.Cells["BoatSplitTime2Recap"].Value.ToString();
                if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                    curTimeOrigValue = curActualTime.ToString( "#0.00" );
                    myRecapRow.Cells["BoatSplitTime2Recap"].Value = curTimeOrigValue;
                } else if ( curTimeOrigValue.ToUpper().Equals("NONE") ) {
                    curTimeOrigValue = .01m.ToString("#0.00");
                    myRecapRow.Cells["BoatSplitTime2Recap"].Value = curTimeOrigValue;
                }
                if ( curTimeOrigValue.Length == 1 ) {
                    curTimeOrigValue = "0" + curTimeOrigValue;
                }
                if ( curTimeOrigValue.Length == 2 ) {
                    if ( !( curTimeOrigValue.Contains( "." ) ) ) {
                        Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
                        Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
                        tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
                        if ( tempTime < curActualTime ) {
                            tempDiff1 = curActualTime - tempTime;
                        } else {
                            tempDiff1 = tempTime - curActualTime;
                        }
                        tempTime = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue );
                        tempDiff2 = tempTime - curActualTime;
                        tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
                        tempDiff3 = curActualTime - tempTime;

                        if ( tempDiff1 < tempDiff2 ) {
                            if ( tempDiff1 < tempDiff3 ) {
                                myRecapRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            } else {
                                myRecapRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        } else {
                            if ( tempDiff2 < tempDiff3 ) {
                                myRecapRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            } else {
                                myRecapRow.Cells["BoatSplitTime2Recap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        }
                    }
                }
            } catch ( Exception exp ) {
                MessageBox.Show( "Exception encountered for second split time \n " + exp.Message );
            }
        }

        private void EndTimeFormat() {
            Decimal curMinTime, curMaxTime, curActualTime;
            Decimal tempTime, tempDiff1, tempDiff2, tempDiff3;
            String curTimeOrigValue;

            String curRtb = myRecapRow.Cells["ReturnToBaseRecap"].Value.ToString();
            Int16 curBoatSpeed = Convert.ToInt16(myRecapRow.Cells["BoatSpeedRecap"].Value.ToString());
            String curEventClass = getSkierClass();
            if ( curBoatSpeed < myMaxSpeed ) {
                //Set to class C times when speed less than max and tournament greater than class C (e.g. class R)
                //Use class N times for all Classes less than class C (e.g. class N, F)
                if ( (Decimal)myClassCRow["ListCodeNum"] < (Decimal)myClassRow["ListCodeNum"] ) {
                    curEventClass = "C";
                }
            }

            //-----------------------------------
            //Validate end course segment time
            //-----------------------------------
            try {
                String curTimeKey = curBoatSpeed.ToString() + "-" + curEventClass + "-41M";
                if ( curRtb.Equals( "Y" ) ) {
                    curTimeKey = curTimeKey + "-RTB";
                }
                myTimeRow = myTimesDataTable.Select( "ListCode = '" + curTimeKey + "'" )[0];
                curMinTime = (Decimal)myTimeRow["MinValue"];
                curMaxTime = (Decimal)myTimeRow["MaxValue"];
                curActualTime = Convert.ToDecimal((String)myTimeRow["CodeValue"]);

                curTimeOrigValue = myRecapRow.Cells["BoatEndTimeRecap"].Value.ToString();
                if ( curTimeOrigValue.ToUpper().Equals( "OK" ) ) {
                    curTimeOrigValue = curActualTime.ToString( "#0.00" );
                    myRecapRow.Cells["BoatEndTimeRecap"].Value = curTimeOrigValue;
                } else if ( curTimeOrigValue.ToUpper().Equals("NONE") ) {
                    curTimeOrigValue = .01m.ToString("#0.00");
                    myRecapRow.Cells["BoatEndTimeRecap"].Value = curTimeOrigValue;
                }
                if ( curTimeOrigValue.Length == 1 ) {
                    curTimeOrigValue = "0" + curTimeOrigValue;
                }
                if ( curTimeOrigValue.Length == 2 ) {
                    if ( !(curTimeOrigValue.Contains(".")) ) {
                        Int32 delimPos = curActualTime.ToString().IndexOf( '.' );
                        Int32 curDigits = Convert.ToInt32( curActualTime.ToString().Substring( 0, delimPos ) );
                        tempTime = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue );
                        if ( tempTime < curActualTime ) {
                            tempDiff1 = curActualTime - tempTime;
                        } else {
                            tempDiff1 = tempTime - curActualTime;
                        }
                        tempTime = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue );
                        tempDiff2 = tempTime - curActualTime;
                        tempTime = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue );
                        tempDiff3 = curActualTime - tempTime;

                        if ( tempDiff1 < tempDiff2 ) {
                            if ( tempDiff1 < tempDiff3 ) {
                                myRecapRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( curDigits.ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            } else {
                                myRecapRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        } else {
                            if ( tempDiff2 < tempDiff3 ) {
                                myRecapRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits + 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            } else {
                                myRecapRow.Cells["BoatEndTimeRecap"].Value = Convert.ToDecimal( ( curDigits - 1 ).ToString() + "." + curTimeOrigValue ).ToString( "#0.00" );
                            }
                        }
                    }
                }
            } catch (Exception exp) {
                MessageBox.Show("Exception encountered for end time \n " + exp.Message);
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
			if ( e.RowIndex > 0 ) {
				myBoatListIdx = e.RowIndex;
				updateBoatSelect();
			}
		}

		private void updateBoatSelect() {
			String curMethodName = "Trick:ScoreEntry:updateBoatSelect";
			String curMsg = "";

			TourBoatTextbox.Text = buildBoatModelNameDisplay();
			TourBoatTextbox.Focus();
			TourBoatTextbox.Select( 0, 0 );
			String curBoatCode = calcBoatCodeFromDisplay( TourBoatTextbox.Text );

			if ( myScoreRow != null ) {
				try {
					Int64 curScorePK = (Int64) myScoreRow["PK"];

					StringBuilder curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Update TrickScore Set Boat = '" + curBoatCode + "'" );
					curSqlStmt.Append( " Where PK = " + curScorePK.ToString() );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

				} catch ( Exception excp ) {
					curMsg = ":Error attempting to update boat selection \n" + excp.Message;
					MessageBox.Show( curMsg );
					Log.WriteFile( curMethodName + curMsg );
				}
			}

			listApprovedBoatsDataGridView.Visible = false;
			approvedBoatSelectGroupBox.Visible = false;
			jumpRecapDataGridView.Focus();
		}

		private String calcBoatCodeFromDisplay( String inBoatModelNameDisplay ) {
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

		private void navPrintResults_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            DataGridView[] myViewList = new DataGridView[1];
            myViewList[0] = jumpRecapDataGridView;

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


                myGridTitle = new StringRowPrinter( "Jump Pass Round " + roundSelect.RoundValue,
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

                mySubtitle = new StringRowPrinter( scoreFeetLabel.Text,
                    0, 25, 50, scoreFeetLabel.Size.Height,
                    Color.Black, Color.White, scoreFeetLabel.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scoreFeetTextBox.Text,
                    0, 40, 50, scoreFeetTextBox.Size.Height,
                    Color.Black, Color.White, scoreFeetTextBox.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( scoreMetersLabel.Text,
                    170, 25, 50, scoreMetersLabel.Size.Height,
                    Color.Black, Color.White, scoreMetersLabel.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scoreMetersTextBox.Text,
                    170, 40, 50, scoreMetersTextBox.Size.Height,
                    Color.Black, Color.White, scoreMetersTextBox.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( nopsScoreLabel.Text,
                    370, 25, 50, nopsScoreLabel.Size.Height,
                    Color.Black, Color.White, nopsScoreLabel.Font, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( nopsScoreTextBox.Text,
                    370, 40, 55, nopsScoreTextBox.Size.Height,
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

        private void getEventRegData(int inRound) {
            getEventRegData( "All", inRound );
        }
        private void getEventRegData(String inEventGroup, int inRound) {
            int curIdx = 0, curRowCount = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            while (curIdx < 2 && curRowCount == 0) {
                curSqlStmt = new StringBuilder( "" );
                if (curIdx == 0) {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, O.EventGroup,  O.RunOrder, E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating" );
                    curSqlStmt.Append( ", E.HCapBase, E.HCapScore, T.JumpHeight, T.SkiYearAge, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.NopsScore" );
                    curSqlStmt.Append(", COALESCE (S.Status, 'TBD') AS Status, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Jump' " );
                } else {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup,  E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.HCapBase, E.HCapScore" );
                    curSqlStmt.Append( ", T.JumpHeight, T.SkiYearAge, S.ScoreFeet as Score, S.ScoreFeet, S.ScoreMeters, S.NopsScore" );
                    curSqlStmt.Append(", COALESCE (S.Status, 'TBD') AS Status, E.AgeGroup as Div, COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN JumpScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Jump' " );
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
            curSqlStmt.Append( "SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, Round, S.EventClass, COALESCE(E.TeamCode, '') as TeamCode" );
            curSqlStmt.Append( ", ScoreFeet, ScoreMeters , NopsScore, Rating, BoatSpeed, RampHeight, Status, Boat, Note" );
            curSqlStmt.Append(", Gender, SkiYearAge ");
            curSqlStmt.Append( "FROM JumpScore S " );
            curSqlStmt.Append( "  INNER JOIN EventReg E ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND E.Event = 'Jump'" );
            curSqlStmt.Append( "  INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup ");
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND S.AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "ORDER BY S.SanctionId, S.MemberId" );
            myScoreDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private Boolean checkForSkierRoundScore( String inMemberId, int inRound, String inAgeGroup ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, MemberId, AgeGroup, Round " );
			curSqlStmt.Append( "FROM JumpScore " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( " AND MemberId = '" + inMemberId + "' " );
			curSqlStmt.Append( " AND Round = " + inRound + " " );
			if ( mySanctionNum.EndsWith( "999" ) || mySanctionNum.EndsWith( "998" ) ) {
				curSqlStmt.Append( " AND AgeGroup = '" + inAgeGroup + "' " );
			}
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return true;
			} else {
				return false;
			}
		}

		private Decimal getSkier1stRoundRampHeight(String inMemberId, String inAgeGroup) {
            Decimal curReturnValue = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT RampHeight FROM JumpScore " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = 1 " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                curReturnValue = (Decimal)curDataTable.Rows[0]["RampHeight"];
            }
            return curReturnValue;
        }

        private void getSkierRecapByRound(String inMemberId, String inAgeGroup, int inRound) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum" );
            curSqlStmt.Append( ", BoatSpeed, RampHeight, Meter1, Meter2, Meter3, Meter4, Meter5, Meter6" );
            curSqlStmt.Append( ", ScoreFeet, ScoreMeters, ScoreTriangle, Results" );
            curSqlStmt.Append( ", BoatSplitTime, BoatEndTime, BoatSplitTime2" );
            curSqlStmt.Append( ", Split52TimeTol, Split82TimeTol, Split41TimeTol" );
            curSqlStmt.Append( ", ReturnToBase, ScoreProt, TimeInTol, Reride, RerideReason, Note, LastUpdateDate " );
            curSqlStmt.Append( "FROM JumpRecap " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "  AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " " );
            curSqlStmt.Append( "ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum" );
            myRecapDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void getMinSpeedData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MIN(SUBSTRING(ListCode, 1, 2)) AS Speed" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'JumpBoatTime3Seg' AND ListCode LIKE '%-52M'" );
            myMinSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void getMaxSpeedData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName LIKE '%JumpMax' " );
            curSqlStmt.Append( "ORDER BY SortSeq " );
            myMaxSpeedDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void getMaxRampData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, MinValue, MaxValue " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName like '%RampMax' " );
            curSqlStmt.Append( "ORDER BY SortSeq " );
            myMaxRampDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private void getJump3TimesDivData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, MaxValue " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "WHERE ListName = 'IwwfJump3TimesDiv' " );
            curSqlStmt.Append( "ORDER BY SortSeq " );
            myJump3TimesDivDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable buildSummaryDataTable() {
            //Determine number of skiers per placement group
            DataTable curDataTable = new DataTable();

            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "MemberId";
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
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "GoodPass";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "SlowPass";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "FastPass";
            curCol.DataType = System.Type.GetType( "System.Int32" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            return curDataTable;
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
    }
}
