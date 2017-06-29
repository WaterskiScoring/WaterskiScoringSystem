using System;
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
using ValidationLibrary.Trick;

namespace WaterskiScoringSystem.Trick {
    public partial class ScoreCalc : Form {
        private bool isDataModified = false;
        private bool isDataModifiedInProgress = false;
        private bool isPassEnded = false;
        private bool isLoadInProg = false;
        private bool isTrickValid = true;
        private Int16 myNumJudges;
        private int myEventRegViewIdx = 0;
        private int myBoatListIdx = 0;

        private int mySkierRunCount = 0;
        private int myPassRunCount = 0;
        private int myEventDelaySeconds = 0;

        private DateTime myEventStartTime;
        private DateTime myEventDelayStartTime;

        private String myTitle = "";
        private String myTourRuleCode = ""; 
        private String myOrigSkisValue = "";
        private String myActiveTrickPass = "";
        private String myOrigCodeValue = "";
        private String myOrigResultsValue = "";
        private String mySanctionNum;
        private String mySortCommand = "";
        private String myFilterCmd = "";
        private String myTourRules = "";
        private String myTourClass = "";

        private DataTable myTrickListDataTable;
        private DataTable myEventRegDataTable;
        private DataTable myScoreDataTable;
        private DataTable myPass1DataTable;
        private DataTable myPass2DataTable;

        private DataRow myTourRow;
        private DataRow myScoreRow;
        private DataRow myClassCRow;
        private DataRow myClassERow;

        private TourProperties myTourProperties;
        private ListSkierClass mySkierClassList;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EventGroupSelect eventGroupSelect;
        private CalcNops appNopsCalc;
        private Validation myTrickValidation;
        private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private SkierDoneReason skierDoneReasonDialogForm;
        private CheckEventRecord myCheckEventRecord;
        private ArrayList mySpecialFlipList = new ArrayList();
        private String[] myAllowedRepeatReverseList = {"RS", "RTS", "RB", "RF", "RTB", "RTF"};

        public ScoreCalc() {
            InitializeComponent();

            appNopsCalc = CalcNops.Instance;
            appNopsCalc.LoadDataForTour();

            myTrickValidation = new Validation();

            ScoreList.Add( new Common.ScoreEntry( "Trick", 0, "", 0 ) );
            mySpecialFlipList.Add( "BFLB" );
            mySpecialFlipList.Add( "RBFLB" );
            mySpecialFlipList.Add( "BFLF" );
            mySpecialFlipList.Add( "RBFLF" );
            mySpecialFlipList.Add( "BFLLB" );
            mySpecialFlipList.Add( "RBFLLB" );
            mySpecialFlipList.Add( "FFLF" );
            mySpecialFlipList.Add( "FFLB" );
        }

        private void ScoreCalc_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.TrickCalc_Width > 0) {
                this.Width = Properties.Settings.Default.TrickCalc_Width;
            }
            if (Properties.Settings.Default.TrickCalc_Height > 0) {
                this.Height = Properties.Settings.Default.TrickCalc_Height;
            }
            if (Properties.Settings.Default.TrickCalc_Location.X > 0
                && Properties.Settings.Default.TrickCalc_Location.Y > 0) {
                this.Location = Properties.Settings.Default.TrickCalc_Location;
            }
            myTourProperties = TourProperties.Instance;
            mySortCommand = myTourProperties.RunningOrderSortTrick;

            int curDelim = mySortCommand.IndexOf( "AgeGroup" );
            if (curDelim < 0) {
            } else if (curDelim > 0) {
                mySortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
                myTourProperties.RunningOrderSortTrick = mySortCommand;
            } else {
                mySortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
                myTourProperties.RunningOrderSortTrick = mySortCommand;
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

            String[] curList = { "SkierName", "Div", "DivOrder", "EventGroup", "RunOrder", "TrickBoat", "TeamCode", "EventClass", "RankingScore", "RankingRating", "HCapBase", "HCapScore", "Status" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            eventGroupSelect = new EventGroupSelect();
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
                    myTitle = this.Text;

                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData(mySanctionNum);
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                            TeamCode.Visible = true;
                        } else {
                            TeamCode.Visible = false;
                        }
                        if ( (byte)myTourRow["TrickRounds"] > 0 ) {

                            EventRunInfoLabel.Text = "   Event Start:\n" + "   Event Delay:\n" + "Skiers, Passes:";
                            EventRunInfoData.Text = "";
                            EventRunPerfLabel.Text = "Event Duration:\n" + "Mins Per Skier:\n" + " Mins Per Pass:";
                            EventRunPerfData.Text = "";

                            //Instantiate object for checking for records
                            myCheckEventRecord = new CheckEventRecord(myTourRow);

                            //Retrieve trick list
                            myTourClass = myTourRow["Class"].ToString().ToUpper();
                            myTourRuleCode = (String)myTourRow["Rules"];
                            getTrickList( myTourRuleCode );
                            if ( myTrickListDataTable.Rows.Count == 0 ) {
                                getTrickList( "awsa" );
                            }

                            //Load skier classes to drop down list
                            mySkierClassList = new ListSkierClass();
                            mySkierClassList.ListSkierClassLoad();
                            scoreEventClass.DataSource = mySkierClassList.DropdownList;
                            scoreEventClass.DisplayMember = "ItemName";
                            scoreEventClass.ValueMember = "ItemValue";

                            myClassCRow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'C'")[0];
                            myClassERow = mySkierClassList.SkierClassDataTable.Select("ListCode = 'E'")[0];

                            myTrickValidation.TourRules = myTourRules;
                            myTrickValidation.SkierClassDataTable = mySkierClassList.SkierClassDataTable;
                            myTrickValidation.TrickListDataTable = myTrickListDataTable;

                            //Load round selection list based on number of rounds specified for the tournament
                            roundSelect.SelectList_Load( myTourRow["TrickRounds"].ToString(), roundSelect_Click );
                            roundActiveSelect.SelectList_LoadHorztl( myTourRow["TrickRounds"].ToString(), roundActiveSelect_Click );
                            roundActiveSelect.RoundValue = "1";
                            roundSelect.RoundValue = "1";

                            //Determine required number of judges for event
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Select ListCode, CodeValue, MaxValue, MinValue FROM CodeValueList ");
                            curSqlStmt.Append( "Where ListName = 'TrickJudgesNum' And ListCode = '" + myTourRow["Class"].ToString() + "' ORDER BY SortSeq" );
                            DataTable curNumJudgesDataTable = getData( curSqlStmt.ToString() );
                            if (curNumJudgesDataTable.Rows.Count > 0) {
                                myNumJudges = Convert.ToInt16( (Decimal)curNumJudgesDataTable.Rows[0]["MaxValue"] );
                            } else {
                                myNumJudges = 1;
                            }

                            //Retrieve list of approved tournament boats
                            getApprovedTowboats();
                            listApprovedBoatsDataGridView.Visible = false;
                            BoatSelectInfoLabel.Visible = false;

                            //Retrieve and load tournament event entries
                            loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

                            if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                                LiveWebLabel.Visible = true;
                            } else {
                                LiveWebLabel.Visible = false;
                            }
                        } else {
                            MessageBox.Show( "The trick event is not defined for the active tournament" );
                        }
                    } else {
                        MessageBox.Show( "An active tournament is not properly defined" );
                    }

                }
            }

        }

        private void ScoreCalc_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.TrickCalc_Width = this.Size.Width;
                Properties.Settings.Default.TrickCalc_Height = this.Size.Height;
                Properties.Settings.Default.TrickCalc_Location = this.Location;
            }
            if ( myPassRunCount > 0 && mySkierRunCount > 0) {
                if ( StartTimerButton.Visible ) {
                    StartTimerButton_Click( null, null );
                }
                int curHours, curMins, curSeconds, curTimeDiff;
                String curMethodName = "Trick:ScoreCalc:ScoreCalc_FormClosed";
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

        private void ScoreCalc_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                CalcScoreButton_Click( null, null );
            }
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show( "DataGridView_DataError occurred. \n Context: " + e.Context.ToString() 
                + "\n Exception Message: " + e.Exception.Message );
            if ((e.Exception) is ConstraintException) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
            ViewTrickDataButton_Click( null, null );
        }

        private void ResizeNarrow_Click(object sender, EventArgs e) {
            ResizeNarrow.Visible = false;
            ResizeNarrow.Enabled = false;
            ResizeWide.Visible = true;
            ResizeWide.Enabled = true;

            TourEventRegDataGridView.Width = 286;
        }

        private void ResizeWide_Click(object sender, EventArgs e) {
            ResizeNarrow.Visible = true;
            ResizeNarrow.Enabled = true;
            ResizeWide.Visible = false;
            ResizeWide.Enabled = false;

            if ( TeamCode.Visible ) {
                TourEventRegDataGridView.Width = 565;
            } else {
                TourEventRegDataGridView.Width = 515;
            }
        }

        private void navSaveItem_Click(object sender, EventArgs e) {
            bool curReturnStatus = true, curMethodReturn;
            if (isLoadInProg) return;
            if (!isDataModified) return;
            try {
                curReturnStatus = saveTrickPass( "Pass1" );
                curMethodReturn = saveTrickPass( "Pass2" );
                if ( curReturnStatus ) curReturnStatus = curMethodReturn;
                if ( curReturnStatus ) {
                    try {
                        String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                        String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                        Byte curRound = (Byte)myScoreRow["Round"];
                        if ( checkPassError() ) {
                            setEventRegRowStatus( "3-Error" );
                        } else {
                            if ( Pass1DataGridView.Rows.Count > 0 ) {
                                if ( isObjectEmpty( Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value ) ) {
                                    if ( Pass2DataGridView.Rows.Count > 0 ) {
                                        if ( isObjectEmpty(Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value) ) {
                                            setEventRegRowStatus("1-TBD");
                                        } else {
                                            setEventRegRowStatus("2-InProg");
                                        }
                                    } else {
                                        setEventRegRowStatus("2-InProg");
                                    }
                                } else {
                                    if ( myTourRuleCode.ToLower().Equals( "ncwsa" ) ) {
                                        setEventRegRowStatus( "4-Done" );
                                    } else {
                                        if ( Pass2DataGridView.Rows.Count > 0 ) {
                                            if ( isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value ) ) {
                                                setEventRegRowStatus( "2-InProg" );
                                            } else {
                                                setEventRegRowStatus( "4-Done" );
                                            }
                                        } else {
                                            setEventRegRowStatus( "2-InProg" );
                                        }
                                    }
                                }
                            } else {
                                if ( myTourRuleCode.ToLower().Equals( "ncwsa" ) ) {
                                    setEventRegRowStatus( "1-TBD" );
                                } else {
                                    if ( Pass2DataGridView.Rows.Count > 0 ) {
                                        if ( isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value ) ) {
                                            setEventRegRowStatus( "1-TBD" );
                                        } else {
                                            setEventRegRowStatus( "2-InProg" );
                                        }
                                    } else {
                                        setEventRegRowStatus( "1-TBD" );
                                    }
                                }
                            }
                        }
                    } catch {
                        setEventRegRowStatus( "3-Error" );
                    }
                    curMethodReturn = saveTrickScore();
                    if ( curReturnStatus ) curReturnStatus = curMethodReturn;
                    if (curReturnStatus) {
                        isDataModified = false;

                        if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                            String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                            String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkierTrick( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
                        }
                        if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                            if ( (myTourRules.ToLower().Equals("ncwsa") && Pass1DataGridView.Rows.Count > 0 )
                                || ( !(myTourRules.ToLower().Equals( "ncwsa" )) && Pass1DataGridView.Rows.Count > 0 && Pass2DataGridView.Rows.Count > 0 )
                                ) {
                                StringBuilder curTwitterMessage = new StringBuilder( "" );
                                String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                                String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                                curTwitterMessage.Append( mySanctionNum + " " + curAgeGroup + " " + activeSkierName.Text );
                                if (curTeamCode.Length > 0) {
                                    curTwitterMessage.Append( " Team: " + curTeamCode );
                                }
                                curTwitterMessage.Append( " Score: " + scoreTextBox.Text + " Pass 1: " + scorePass1.Text + " Pass 2: " + scorePass2.Text );
                                curTwitterMessage.Append( " UNOFFICIAL " + DateTime.Now );
                                ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                            }
                        }
                    }
                }
            } catch ( Exception excp ) {
                curReturnStatus = false;
                MessageBox.Show( "Error attempting to save changes for trick score \n" + excp.Message );
            }
            if ( curReturnStatus ) {
                isDataModified = false;
                if ( Pass1DataGridView.Rows.Count > 1 ) {
                    if ( Pass2DataGridView.Rows.Count > 1 ) {
                        noteTextBox.Focus();
                    } else {
                        Pass2DataGridView.Focus();
                        Pass2DataGridView.Select();
                        if ( Pass2DataGridView.Rows.Count == 1 ) {
                            Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                        }
                    }
                } else {
                    if ( Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().Length > 0 ) {
                        if (Pass2DataGridView.Rows.Count > 0) {
                            if (Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value.ToString().Length > 0) {
                                noteTextBox.Focus();
                            } else {
                                Pass2DataGridView.Focus();
                                Pass2DataGridView.Select();
                                if (Pass2DataGridView.Rows.Count == 1) {
                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                                }
                            }
                        } else {
                            noteTextBox.Focus();
                        }
                    } else {
                        Pass1DataGridView.Focus();
                        Pass1DataGridView.Select();
                        if ( Pass1DataGridView.Rows.Count == 1 ) {
                            Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[0].Cells["Pass1Skis"];
                        }
                    }
                }
            }
        }

        private bool saveTrickScore() {
            String curMethodName = "Trick:ScoreCalc:saveTrickScore";
            String curMsg = "";
            bool curReturnStatus = true;
            Int64 curPK = 0;

            try {
                String curBoat = "", curNote;
                String curSanctionId = (String)myScoreRow[ "SanctionId" ];
                String curMemberId = (String)myScoreRow[ "MemberId" ];
                String curAgeGroup = (String)myScoreRow["AgeGroup"];
                Byte curRound = (Byte)myScoreRow["Round"];
                String curEventClass = (String)scoreEventClass.SelectedValue;

                try {
                    curPK = (Int64)myScoreRow[ "PK" ];
                } catch {
                    curPK = 0;
                }
                try {
                    myScoreRow["Score"] = Convert.ToInt16( scoreTextBox.Text );
                } catch {
                    myScoreRow["Score"] = 0;
                }
                try {
                    myScoreRow["ScorePass1"] = Convert.ToInt16( scorePass1.Text );
                } catch {
                    myScoreRow["ScorePass1"] = 0;
                }
                try {
                    myScoreRow["ScorePass2"] = Convert.ToInt16( scorePass2.Text );
                } catch {
                    myScoreRow["ScorePass2"] = 0;
                }
                try {
                    myScoreRow["NopsScore"] = Convert.ToDecimal( nopsScoreTextBox.Text );
                } catch {
                    myScoreRow["NopsScore"] = 0;
                }
                try {
                    myScoreRow["Note"] = noteTextBox.Text;
                    curNote = noteTextBox.Text;
                    curNote = curNote.Replace( "'", "''" );
                } catch {
                    myScoreRow["Note"] = "";
                    curNote = "";
                }

                try {
                    if ( TourBoatTextbox.Text.Equals( "--Select--" ) ) {
                        curBoat = "";
                    } else {
                        curBoat = TourBoatTextbox.Text;
                        curBoat = curBoat.Replace( "'", "''" );
                    }
                } catch {
                    curBoat = "";
                }
                try {
                    myScoreRow["Status"] = "TBD";
                    String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
                    if ( curValue.Equals("4-Done") ) myScoreRow["Status"] = "Complete";
                    if ( curValue.Equals( "2-InProg" ) ) myScoreRow["Status"] = "InProg";
                    if ( curValue.Equals( "3-Error" ) ) myScoreRow["Status"] = "Error";
                    if ( curValue.Equals( "1-TBD" ) ) myScoreRow["Status"] = "TBD";
                } catch {
                    myScoreRow["Status"] = "TBD";
                }


                if ( curSanctionId.Length > 1
                    && curMemberId.Length > 1
                    ) {
                    StringBuilder curSqlStmt = new StringBuilder( "" );

                    if ( ((String)myScoreRow["Status"]).Equals( "TBD" ) ) {
                        curSqlStmt.Append( "Delete TrickScore Where PK = " + curPK.ToString() );
                    } else {
                        if ( curPK > 0 ) {
                            curSqlStmt.Append( "Update TrickScore Set " );
                            curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                            curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                            curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                            curSqlStmt.Append( ", Round = " + curRound.ToString() );
                            curSqlStmt.Append(", EventClass = '" + curEventClass + "'");
                            curSqlStmt.Append(", Score = " + ((Int16)myScoreRow["Score"]).ToString());
                            curSqlStmt.Append( ", ScorePass1 = " + ( (Int16)myScoreRow["ScorePass1"] ).ToString() );
                            curSqlStmt.Append( ", ScorePass2 = " + ( (Int16)myScoreRow["ScorePass2"] ).ToString() );
                            curSqlStmt.Append( ", NopsScore = " + ( (Decimal)myScoreRow["NopsScore"] ).ToString( "#####.00" ) );
                            curSqlStmt.Append( ", Boat = '" + curBoat + "'" );
                            curSqlStmt.Append( ", Status = '" + myScoreRow["Status"] + "'" );
                            curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                            curSqlStmt.Append( ", Note = '" + curNote.Trim() + "'" );
                            curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                        } else {
                            curSqlStmt.Append( "Insert TrickScore (" );
                            curSqlStmt.Append("SanctionId, MemberId, AgeGroup, Round, EventClass, ");
                            curSqlStmt.Append( "Score, ScorePass1, ScorePass2, NopsScore, " );
                            curSqlStmt.Append( "Boat, Status, LastUpdateDate, Note" );
                            curSqlStmt.Append( ") Values (" );
                            curSqlStmt.Append( " '" + curSanctionId + "'" );
                            curSqlStmt.Append( ", '" + curMemberId + "'" );
                            curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                            curSqlStmt.Append( ", " + curRound.ToString() );
                            curSqlStmt.Append(", '" + curEventClass + "'");
                            curSqlStmt.Append(", " + ((Int16)myScoreRow["Score"]).ToString());
                            curSqlStmt.Append( ", " + ( (Int16)myScoreRow["ScorePass1"] ).ToString() );
                            curSqlStmt.Append( ", " + ( (Int16)myScoreRow["ScorePass2"] ).ToString() );
                            curSqlStmt.Append( ", " + ( (Decimal)myScoreRow["NopsScore"] ).ToString( "#####.00" ) );
                            curSqlStmt.Append( ", '" + curBoat + "'" );
                            curSqlStmt.Append( ", '" + (String)myScoreRow["Status"] + "'" );
                            curSqlStmt.Append( ", GETDATE()" );
                            curSqlStmt.Append( ", '" + curNote.Trim() + "'" );
                            curSqlStmt.Append( " )" );
                        }
                    }

                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    curReturnStatus = true;
                    if ( curPK < 0 ) {
                        setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, curRound );
                    }

                    #region Check to see if score is equal to or great than divisions current record score
                    if ( curSanctionId.Length > 1
                        && curMemberId.Length > 1
                        ) {
                        if ( !(( (String) myScoreRow["Status"] ).Equals("TBD")) ) {
                            String curCheckRecordMsg = myCheckEventRecord.checkRecordTrick(curAgeGroup, scoreTextBox.Text, (byte) myScoreRow["SkiYearAge"], (string) myScoreRow["Gender"]);
                            if ( curCheckRecordMsg == null ) curCheckRecordMsg = "";
                            if ( curCheckRecordMsg.Length > 1 ) {
                                MessageBox.Show(curCheckRecordMsg);
                            }
                        }
                    }
                    #endregion

                    int curSeconds = 0, curMins = 0, curHours, curTimeDiff = 0;
                    if ( myPassRunCount > 1 && mySkierRunCount > 0 ) {
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
                        + ", " + myPassRunCount.ToString( "##0" );
                }
            } catch ( Exception excp ) {
                curReturnStatus = false;
                curMsg = ":Error attempting to save trick score \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return curReturnStatus;
        }

        private bool saveTrickPass( String inPassPrefix ) {
            String curMethodName = "Trick:ScoreCalc:saveTrickPass";
            String curMsg = "";
            DataGridView curDataGridView = null;
            bool curReturnStatus = true, curMethodReturn;

            if ( inPassPrefix.Equals("Pass1") ) {
                curDataGridView = Pass1DataGridView;
            } else {
                curDataGridView = Pass2DataGridView;
            }
            if ( curDataGridView == null ) {
                curReturnStatus = false;
            } else {
                try {
                    foreach ( DataGridViewRow curViewRow in curDataGridView.Rows ) {
                        curMethodReturn = saveTrickPassEntry( inPassPrefix, curViewRow );
                        if ( curReturnStatus ) curReturnStatus = curMethodReturn;
                    }
                } catch ( Exception excp ) {
                    curReturnStatus = false;
                    curMsg = ":Error attempting to save trick pass" + inPassPrefix + "\n" + excp.Message;
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
                foreach ( DataGridViewRow curViewRow in curDataGridView.Rows ) {
                    saveTrickPassUpdatePK( inPassPrefix, curViewRow );
                }
            }
            return curReturnStatus;
        }
    
        private void saveTrickPassUpdatePK( String inPassPrefix, DataGridViewRow inViewRow ) {
            try {
                int curPK = Convert.ToInt32( inViewRow.Cells[inPassPrefix + "PK"].Value );
                if ( curPK < 0 ) {String curMemberId = (String)inViewRow.Cells[inPassPrefix + "MemberId"].Value;
                    String curAgeGroup = (String)inViewRow.Cells[inPassPrefix + "AgeGroup"].Value;
                    String curCode = (String)inViewRow.Cells[inPassPrefix + "Code"].Value;
                    this.myTrickValidation.validateNumSkis((String) inViewRow.Cells[inPassPrefix + "Skis"].Value, this.myTourRules);
                    String curSkis = this.myTrickValidation.NumSkis.ToString();

                    String curResults = (String)inViewRow.Cells[inPassPrefix + "Results"].Value;
                    if ( curCode.Length > 0 && curSkis.Length > 0 && curResults.Length > 0 ) {
                        byte curRound = Convert.ToByte( inViewRow.Cells[inPassPrefix + "Round"].Value );
                        byte curPassNum = Convert.ToByte( inViewRow.Cells[inPassPrefix + "PassNum"].Value );
                        byte curSeq = Convert.ToByte( inViewRow.Cells[inPassPrefix + "Seq"].Value );
                        DataTable curTrickPassDataTable = getSkierPassByRound( curMemberId, curAgeGroup, curRound, curPassNum, curSeq );
                        if ( curTrickPassDataTable.Rows.Count == 1 ) {
                            inViewRow.Cells[inPassPrefix + "PK"].Value = curTrickPassDataTable.Rows[0]["PK"].ToString();
                        } else {
                            MessageBox.Show( "No row retrieved after adding new trick entry. Insert of trick may have failed"
                                + "\n SanctionNum=" + mySanctionNum
                                + "\n MemberId=" + curMemberId
                                + "\n Round=" + curRound
                                + "\n PassNum=" + curPassNum
                                + "\n Seq=" + curSeq
                                );
                        }
                    }
                }
            } catch {
            }
        }

        private bool saveTrickPassEntry( String inPassPrefix, DataGridViewRow inViewRow ) {
            String curMethodName = "Trick:ScoreCalc:saveTrickPassEntry";
            String curMsg = "";
            bool curReturnStatus = true;
            
            String curUpdateStatus = (String)inViewRow.Cells[inPassPrefix + "Updated"].Value;
            int curPK = 0;
            try {
                String curSanctionId = (String)inViewRow.Cells[inPassPrefix + "SanctionId"].Value;
                String curMemberId = (String)inViewRow.Cells[inPassPrefix + "MemberId"].Value;
                String curAgeGroup = (String)inViewRow.Cells[inPassPrefix + "AgeGroup"].Value;
                String curNote = "";
                try {
                    curNote = ((String)inViewRow.Cells[inPassPrefix + "Note"].Value).Trim();
                } catch {
                    curNote = "";
                }
                String curSkis = "";
                if ( this.myTrickValidation.validateNumSkis((String) inViewRow.Cells[inPassPrefix + "Skis"].Value, this.myTourRules) >= 0 ) {
                    curSkis = this.myTrickValidation.NumSkis.ToString();
                } else {
                    curSkis = "0";
                }

                try {
                    curPK = Convert.ToInt32( inViewRow.Cells[inPassPrefix + "PK"].Value );
                    if ( curUpdateStatus.ToUpper().Equals( "Y" )
                        && curSanctionId.Length > 1
                        && curMemberId.Length > 1
                        && !(inViewRow.Cells[inPassPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" ))
                        ) {
                        try {
                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            if ( curPK > 0 ) {
                                curSqlStmt.Append( "Update TrickPass Set " );
                                curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                                curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", Round = " + inViewRow.Cells[inPassPrefix + "Round"].Value );
                                curSqlStmt.Append( ", PassNum = " + inViewRow.Cells[inPassPrefix + "PassNum"].Value );
                                curSqlStmt.Append( ", Skis = " + curSkis );
                                curSqlStmt.Append( ", Seq = " + inViewRow.Cells[inPassPrefix + "Seq"].Value );
                                curSqlStmt.Append( ", Score = " + inViewRow.Cells[inPassPrefix + "Points"].Value );
                                curSqlStmt.Append( ", Code = '" + inViewRow.Cells[inPassPrefix + "Code"].Value + "'" );
                                curSqlStmt.Append( ", Results = '" + inViewRow.Cells[inPassPrefix + "Results"].Value + "'" );
                                curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                                curSqlStmt.Append( ", Note = '" + curNote + "'" );
                                curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                            } else {
                                curSqlStmt.Append( "Insert TrickPass (" );
                                curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, PassNum, " );
                                curSqlStmt.Append( "Skis, Seq, Score, Code, Results, LastUpdateDate, Note" );
                                curSqlStmt.Append( ") Values (" );
                                curSqlStmt.Append( " '" + curSanctionId + "'" );
                                curSqlStmt.Append( ", '" + curMemberId + "'" );
                                curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", " + inViewRow.Cells[inPassPrefix + "Round"].Value );
                                curSqlStmt.Append( ", " + inViewRow.Cells[inPassPrefix + "PassNum"].Value );
                                curSqlStmt.Append( ", " + curSkis );
                                curSqlStmt.Append( ", " + inViewRow.Cells[inPassPrefix + "Seq"].Value );
                                curSqlStmt.Append( ", " + inViewRow.Cells[inPassPrefix + "Points"].Value );
                                curSqlStmt.Append( ", '" + inViewRow.Cells[inPassPrefix + "Code"].Value + "'" );
                                curSqlStmt.Append( ", '" + inViewRow.Cells[inPassPrefix + "Results"].Value + "'" );
                                curSqlStmt.Append( ", GETDATE()" );
                                curSqlStmt.Append( ", '" + curNote + "' )" );
                            }
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            if ( rowsProc > 0 ) {
                                curReturnStatus = true;
                            }
                        } catch ( Exception excp ) {
                            curReturnStatus = false;
                            curMsg = ":Error attempting to save changes on trick pass \n" + excp.Message;
                            MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                    } else {
                        curReturnStatus = true;
                    }
                } catch {
                    //PK value not set therefore must not be a valid row
                    curMsg = ":Error PK value not set therefore must not be a valid row";
                    MessageBox.Show( curMsg );
                    Log.WriteFile( curMethodName + curMsg );
                }
            } catch ( Exception excp ) {
                curReturnStatus = false;
                curMsg = ":Error attempting to save changes for trick pass " + inPassPrefix + "\n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return curReturnStatus;
        }

        private void calcUpdateScoreTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( calcUpdateScoreTimer );
            isLoadInProg = true;
            CalcScoreButton_Click( null, null );
            isLoadInProg = false;
            isDataModifiedInProgress = false;
        }

        private void CalcScoreButton_Click( object sender, EventArgs e ) {
            bool curReadyToScore = true;
            int curTotalScore = 0, curPass1Score = 0, curPass2Score = 0;
            String curColPrefix, curTrickCode, curValue;
            Int16 curNumSkis = 0;

            if ( TourEventRegDataGridView.CurrentRow != null ) {
                String curMemberId = (String)TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value;
                String curAgeGroup = (String)TourEventRegDataGridView.CurrentRow.Cells["AgeGroup"].Value;

                //Calculate first pass score
                if ( Pass1DataGridView.Rows.Count > 0 ) {
                    curColPrefix = "Pass1";
                    foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                        if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value )
                            || isObjectEmpty( curPassRow.Cells[curColPrefix + "Code"].Value )
                            ) {
                        } else {
                            curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                            curTrickCode = (String)curPassRow.Cells[curColPrefix + "Code"].Value;
                            if ( curTrickCode.Equals( "END" ) || curTrickCode.Equals( "HORN" ) ) {
                                if ( deleteTrickPass( curPassRow.Cells["Pass1PK"].Value.ToString() ) ) {
                                    Pass1DataGridView.Rows.Remove( curPassRow );
                                }
                            } else {
                                if ( checkTrickCode( Pass1DataGridView, curPassRow.Index, curTrickCode, curNumSkis, curColPrefix ) ) {
                                    if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Points"].Value ) ) {
                                        if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Results"].Value ) ) {
                                            MessageBox.Show( "Empty points and results (shouldn't be able to happen)" );
                                            break;
                                        } else {
                                            if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" ) ) {
                                                if ( curPassRow.Index == ( Pass1DataGridView.Rows.Count - 1 ) ) {
                                                    Pass1DataGridView.Rows.Remove( curPassRow );
                                                } else {
                                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass1Score.ToString();
                                                    MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                    break;
                                                }
                                            }
                                        }
                                    } else {
                                        if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" ) ) {
                                            if ( curPassRow.Index == ( Pass1DataGridView.Rows.Count - 1 ) ) {
                                                Pass1DataGridView.Rows.Remove( curPassRow );
                                            } else {
                                                curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass1Score.ToString();
                                                MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                break;
                                            }
                                        } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals("OOC") ) {
                                            if ( curPassRow.Index > 0 && curPassRow.Index < ( Pass1DataGridView.Rows.Count - 1 ) ) {
                                                curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass1Score.ToString();
                                                MessageBox.Show("Results equals OOC, all subsequent rows will be ignored");
                                                break;
                                            }
                                        } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "FALL" ) ) {
                                            if ( curPassRow.Index == ( Pass1DataGridView.Rows.Count - 1 ) ) {
                                                curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass1Score.ToString();
                                                MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                break;
                                            }
                                        } else {
                                            try {
                                                curPass1Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                            } catch {
                                                curPass1Score += 0;
                                            }
                                        }
                                    }
                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass1Score.ToString();
                                } else {
                                    if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" ) ) {
                                        if ( curPassRow.Index == ( Pass1DataGridView.Rows.Count - 1 ) ) {
                                            Pass1DataGridView.Rows.Remove( curPassRow );
                                            //curPass1Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                        } else {
                                            MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                            break;
                                        }
                                    } else {
                                        curReadyToScore = false;
                                        setEventRegRowStatus( "Error" );
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if ( Pass2DataGridView.Visible ) {
                    if ( Pass2DataGridView.Rows.Count > 0 ) {
                        curColPrefix = "Pass2";
                        foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                            if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value )
                                || isObjectEmpty( curPassRow.Cells[curColPrefix + "Code"].Value )
                                ) {
                            } else {
                                curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                                curTrickCode = (String)curPassRow.Cells[curColPrefix + "Code"].Value;
                                if ( curTrickCode.Equals( "END" ) || curTrickCode.Equals( "HORN" ) ) {
                                    if ( deleteTrickPass( curPassRow.Cells["Pass2PK"].Value.ToString() ) ) {
                                        Pass2DataGridView.Rows.Remove( curPassRow );
                                    }
                                } else if ( curTrickCode.Length == 0 ) {
                                } else {
                                    if ( checkTrickCode( Pass2DataGridView, curPassRow.Index, curTrickCode, curNumSkis, curColPrefix ) ) {
                                        if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Points"].Value ) ) {
                                            if ( isObjectEmpty( curPassRow.Cells[curColPrefix + "Results"].Value ) ) {
                                                MessageBox.Show( "Empty points and results (shouldn't be able to happen)" );
                                            } else {
                                                if ( curPassRow.Index == ( Pass2DataGridView.Rows.Count - 1 ) ) {
                                                    curPass2Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                                } else {
                                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass2Score.ToString();
                                                    MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                    break;
                                                }
                                            }
                                        } else {
                                            if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" ) ) {
                                                if ( curPassRow.Index == ( Pass2DataGridView.Rows.Count - 1 ) ) {
                                                    Pass2DataGridView.Rows.Remove( curPassRow );
                                                    //curPass2Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                                } else {
                                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass2Score.ToString();
                                                    MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                    break;
                                                }
                                            } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals("OOC") ) {
                                                if ( curPassRow.Index > 0 && curPassRow.Index < ( Pass2DataGridView.Rows.Count - 1 ) ) {
                                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass2Score.ToString();
                                                    MessageBox.Show("Results equals OOC, all subsequent rows will be ignored");
                                                    break;
                                                }
                                            } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "FALL" ) ) {
                                                if ( curPassRow.Index == ( Pass2DataGridView.Rows.Count - 1 ) ) {
                                                } else {
                                                    curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass2Score.ToString();
                                                    MessageBox.Show( "Results equals END, all subsequent rows will be ignored" );
                                                    break;
                                                }
                                            } else {
                                                try {
                                                    curPass2Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                                } catch {
                                                    curPass2Score += 0;
                                                }
                                            }
                                        }
                                        curPassRow.Cells[curColPrefix + "PointsTotal"].Value = curPass2Score.ToString();
                                    } else {
                                        curReadyToScore = false;
                                        setEventRegRowStatus( "Error" );
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if ( curReadyToScore ) {
                    isDataModified = true;
                    curTotalScore = curPass1Score + curPass2Score;
                    scoreTextBox.Text = curTotalScore.ToString();
                    TourEventRegDataGridView.CurrentRow.Cells["Score"].Value = scoreTextBox.Text;
                    scorePass1.Text = curPass1Score.ToString();
                    scorePass2.Text = curPass2Score.ToString();

                    ScoreList[0].Score = Convert.ToDecimal( curTotalScore.ToString() );
                    appNopsCalc.calcNops( curAgeGroup, ScoreList );
                    nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();

                    navSaveItem_Click( null, null );
                    refreshScoreSummaryWindow();
                } else {
                    MessageBox.Show( "Invalid data has been detected and can not be saved at this time" );
                }
            }
        }

        private void calcUpdateScore() {
            int curTotalScore = 0, curPass1Score = 0, curPass2Score = 0;
            String curColPrefix;

            if ( TourEventRegDataGridView.CurrentRow != null ) {
                String curAgeGroup = (String)TourEventRegDataGridView.CurrentRow.Cells["AgeGroup"].Value;
                String curMemberId = (String)TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value;

                //Calculate first pass score
                if ( Pass1DataGridView.Rows.Count > 0 ) {
                    curColPrefix = "Pass1";
                    foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                        if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Points"].Value)) ) {
                            if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "End" ) ) {
                                break;
                            } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "Fall" ) ) {
                                break;
                            } else {
                                try {
                                    curPass1Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                } catch {
                                    curPass1Score += 0;
                                }
                            }
                        } else {
                            if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Results"].Value)) ) {
                                if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "End" ) ) {
                                    break;
                                }
                            }
                        }
                    }
                }

                //Calculate second pass score
                if ( Pass2DataGridView.Rows.Count > 0 ) {
                    curColPrefix = "Pass2";
                    foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                        if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Points"].Value)) ) {
                            if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "End" ) ) {
                                break;
                            } else if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "Fall" ) ) {
                                break;
                            } else {
                                try {
                                    curPass2Score += Convert.ToInt16( curPassRow.Cells[curColPrefix + "Points"].Value );
                                } catch {
                                    curPass2Score += 0;
                                }
                            }
                        } else {
                            if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Results"].Value)) ) {
                                if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().Equals( "End" ) ) {
                                    break;
                                }
                            }
                        }
                    }
                }

                curTotalScore = curPass1Score + curPass2Score;
                scoreTextBox.Text = curTotalScore.ToString();
                TourEventRegDataGridView.CurrentRow.Cells["Score"].Value = scoreTextBox.Text;

                scorePass1.Text = curPass1Score.ToString();
                scorePass2.Text = curPass2Score.ToString();

                ScoreList[0].Score = Convert.ToDecimal( curTotalScore.ToString() );
                appNopsCalc.calcNops( curAgeGroup, ScoreList );
                nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();

                navSaveItem_Click( null, null );
                refreshScoreSummaryWindow();
            }
        }

        private void refreshScoreSummaryWindow() {
            // Check for open instance of selected form
            //SystemMain curMdiWindow = (SystemMain)this.Parent.Parent;
            //for ( int idx = 0; idx < curMdiWindow.MdiChildren.Length; idx++ ) {
            //    Form curForm = (Form)( curMdiWindow.MdiChildren[idx] );
            //    if ( curForm.Name.Equals( "TrickSummary" ) ) {
            //        TrickSummary curSummaryWindow = (TrickSummary)curForm;
            //        curSummaryWindow.navRefresh_Click( null, null );
            //    } else if ( curForm.Name.Equals( "TrickSummaryTeam" ) ) {
            //        TrickSummaryTeam curSummaryWindow = (TrickSummaryTeam)curForm;
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

        private void insertTrickTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( insertTrickTimer );

            if ( isLoadInProg ) {
                MessageBox.Show( "insertTrickTimer LoadInProg" );
            } else {
                if ( myActiveTrickPass.Equals( "Pass1" ) ) {
                    insertTrick( Pass1DataGridView, false );
                    if ( Pass1DataGridView.Rows.Count > 1 ) {
                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
                    } else {
                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Skis"];
                    }
                } else if ( myActiveTrickPass.Equals( "Pass2" ) ) {
                    insertTrick( Pass2DataGridView, false );
                    if ( Pass2DataGridView.Rows.Count > 1 ) {
                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                    } else {
                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Skis"];
                    }
                }
            }
        }

        private int insertTrick( DataGridView inPassView, bool inInsertAbove ) {
            isLoadInProg = true;
            DataGridViewRow curPassRow;
            DataGridViewRow newPassRow;
            Int16 newRowSeqNum = 0;
            Int16 curNumSkis, curPassNum, curRound;
            int newPosIdx = 0;
            int curColSeq = -1;
            int curLastIdx = 0;
            String curMsg = "", curValue;

            curMsg = "curMemberId";
            String curColPrefix = inPassView.Name.Substring( 0, 5 );
            String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
            if ( inPassView.Rows.Count > 0 ) {
                try {
                    if ( inInsertAbove ) {
                        newPosIdx = inPassView.CurrentRow.Index;
                        curLastIdx = inPassView.Rows.Add();
                    } else {
                        curLastIdx = inPassView.Rows.Count - 1;
                        if ( isObjectEmpty( (String)inPassView.Rows[curLastIdx].Cells[curColPrefix + "Code"].Value ) ) {
                            newPosIdx = -1;
                        } else {
                            curValue = inPassView.Rows[curLastIdx].Cells[curColPrefix + "Results"].Value.ToString();
                            if ( curValue.Equals( "Fall" ) || ( curValue.Equals( "OOC" ) && inPassView.Rows.Count > 1 ) ) {
                                newPosIdx = -1;
                            } else {
                                newPosIdx = inPassView.Rows.Add();
                            }
                        }
                    }
                    curColSeq = inPassView.Columns[curColPrefix + "Seq"].Index;
                } catch {
                }
            } else {
                curLastIdx = 0;
                newPosIdx = inPassView.Rows.Add();
                curColSeq = inPassView.Columns[curColPrefix + "Seq"].Index;
            }

            try {
                if ( newPosIdx < 0 ) {
                    //MessageBox.Show( "No row added" );
                } else {
                    if ( newPosIdx < curLastIdx ) {
                        curPassRow = inPassView.CurrentRow;
                        curMsg = (String)curPassRow.Cells[curColPrefix + "Round"].Value
                            + ":" + (String)curPassRow.Cells[curColPrefix + "PassNum"].Value
                            + ":" + (String)curPassRow.Cells[curColPrefix + "Skis"].Value;
                        newRowSeqNum = Convert.ToInt16( (String)curPassRow.Cells[curColSeq].Value );
                        curRound = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "Round"].Value );
                        curPassNum = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "PassNum"].Value );
                        curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                        if ( curNumSkis  < 0) curNumSkis = 1;

                        if ( inInsertAbove ) {
                            curMsg = "Insert Above";
                            Int16 curRowSeqNum;
                            for ( int curIdx = curLastIdx; curIdx > newPosIdx; curIdx-- ) {
                                for ( int colIdx = 0; colIdx < inPassView.ColumnCount; colIdx++ ) {
                                    inPassView.Rows[curIdx].Cells[colIdx].Value = inPassView.Rows[curIdx - 1].Cells[colIdx].Value;
                                }
                                curRowSeqNum = Convert.ToInt16( inPassView.Rows[curIdx].Cells[curColSeq].Value );
                                curRowSeqNum++;
                                inPassView.Rows[curIdx].Cells[curColSeq].Value = curRowSeqNum.ToString();
                                inPassView.Rows[curIdx].Cells[curColPrefix + "Updated"].Value = "Y";
                            }
                        } else {
                            curMsg = "Insert Last";
                            curPassRow = inPassView.Rows[curLastIdx];
                            newRowSeqNum = Convert.ToInt16( (String)curPassRow.Cells[curColSeq].Value );
                            newRowSeqNum++;
                            curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                            curRound = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "Round"].Value );
                            curPassNum = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "PassNum"].Value );
                        }
                    } else {
                        if ( curLastIdx == 0 && newPosIdx == 0 ) {
                            curMsg = "Insert Last";
                            curNumSkis = 1;
                            newRowSeqNum = 1;
                            inPassView.CurrentCell = inPassView.Rows[newPosIdx].Cells[curColPrefix + "Skis"];
                            curRound = Convert.ToInt16( roundSelect.RoundValue );
                            if ( curColPrefix.Equals( "Pass1" ) ) {
                                curPassNum = 1;
                            } else {
                                if ( Pass1DataGridView.Rows.Count > 0 ) {
                                    try {
                                        curNumSkis = Convert.ToByte( Pass1DataGridView.Rows[0].Cells["Pass1Skis"].Value );
                                    } catch {
                                        curNumSkis = 1;
                                    }
                                }
                                curPassNum = 2;
                            }
                        } else {
                            curMsg = "Insert Last";
                            curPassRow = inPassView.Rows[curLastIdx];
                            newRowSeqNum = Convert.ToInt16( (String)curPassRow.Cells[curColSeq].Value );
                            newRowSeqNum++;
                            curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                            curRound = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "Round"].Value );
                            curPassNum = Convert.ToInt16( (String)curPassRow.Cells[curColPrefix + "PassNum"].Value );
                        }
                    }
                    newPassRow = inPassView.Rows[newPosIdx];
                    newPassRow.Cells[curColPrefix + "PK"].Value = "-1";
                    newPassRow.Cells[curColPrefix + "SanctionId"].Value = mySanctionNum;
                    newPassRow.Cells[curColPrefix + "MemberId"].Value = curMemberId;
                    newPassRow.Cells[curColPrefix + "AgeGroup"].Value = curAgeGroup;
                    newPassRow.Cells[curColPrefix + "Code"].Value = "";
                    newPassRow.Cells[curColPrefix + "Round"].Value = curRound.ToString();
                    if ( curNumSkis == 0 ) {
                        newPassRow.Cells[curColPrefix + "Skis"].Value = "W";
                    } else if ( curNumSkis == 9 ) {
                        newPassRow.Cells[curColPrefix + "Skis"].Value = "K";
                    } else {
                        newPassRow.Cells[curColPrefix + "Skis"].Value = curNumSkis.ToString( "#" );
                    }
                    newPassRow.Cells[curColPrefix + "Seq"].Value = newRowSeqNum.ToString();
                    newPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                    newPassRow.Cells[curColPrefix + "PassNum"].Value = curPassNum.ToString();
                    newPassRow.Cells[curColPrefix + "Points"].Value = "0";
                    newPassRow.Cells[curColPrefix + "Updated"].Value = "N";

                    if ( myPassRunCount == 0 ) {
                        myEventStartTime = DateTime.Now;
                        EventRunInfoData.Text = "";
                        EventRunPerfData.Text = "";
                    }
                    myPassRunCount++;
                    if ( curPassNum == 1 && newPosIdx == 0 ) {
                        mySkierRunCount++;
                    } else {
                        if ( curPassNum == 1 && mySkierRunCount == 0 ) {
                            mySkierRunCount++;
                        }
                    }

                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to initialize a new row for display \n" + excp.Message + "\n\n" + curColPrefix + curMsg );
            }

            isLoadInProg = false;
            return newPosIdx;
        }

        private void deleteTrick(String inPassView, int inRowIdx) {
            String curValue = "", curPkValue = "";
            int curRowIdx = inRowIdx;
            if ( inPassView.Equals( "Pass1" ) ) {
                if ( isObjectEmpty( Pass1DataGridView.Rows[curRowIdx].Cells["Pass1PK"].Value ) ) {
                    curPkValue = "";
                } else {
                    curPkValue = Pass1DataGridView.Rows[curRowIdx].Cells["Pass1PK"].Value.ToString();
                }
                if ( isObjectEmpty( Pass1DataGridView.Rows[curRowIdx].Cells["Pass1Results"].Value ) ) {
                    curValue = "";
                } else {
                    curValue = Pass1DataGridView.Rows[curRowIdx].Cells["Pass1Results"].Value.ToString();
                }
                if ( curValue.Equals( "Fall" ) ) {
                    isPassEnded = false;
                }
                if ( deleteTrickPass( curPkValue ) ) {
                    Pass1DataGridView.Rows.Remove( Pass1DataGridView.Rows[curRowIdx] );
                    curRowIdx--;
                }
                if ( Pass1DataGridView.Rows.Count > 0 ) {
                    if ( curRowIdx < 0 ) curRowIdx = 0;
                    Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[curRowIdx].Cells["Pass1Code"];
                    Pass1DataGridView.CurrentCell.Selected = true;
                    for ( int idx = curRowIdx; idx < Pass1DataGridView.Rows.Count; idx++ ) {
                        Pass1DataGridView.Rows[idx].Cells["Pass1Seq"].Value = ( idx + 1 ).ToString();
                        Pass1DataGridView.Rows[idx].Cells["Pass1Updated"].Value = "Y";
                    }
                    Pass1DataGridView.Focus();
                } else {
                    isDataModified = true;
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                    curTimerObj.Start();
                    insertTrick( Pass1DataGridView, false );
                }
            } else if ( myActiveTrickPass.Equals( "Pass2" ) ) {
                if ( isObjectEmpty( Pass2DataGridView.Rows[curRowIdx].Cells["Pass2PK"].Value ) ) {
                    curPkValue = "";
                } else {
                    curPkValue = Pass2DataGridView.Rows[curRowIdx].Cells["Pass2PK"].Value.ToString();
                }
                if ( isObjectEmpty( Pass2DataGridView.Rows[curRowIdx].Cells["Pass2Results"].Value ) ) {
                    curValue = "";
                } else {
                    curValue = Pass2DataGridView.Rows[curRowIdx].Cells["Pass2Results"].Value.ToString();
                }
                if ( curValue.Equals( "Fall" ) ) {
                    isPassEnded = false;
                }
                if ( deleteTrickPass( curPkValue ) ) {
                    Pass2DataGridView.Rows.Remove( Pass2DataGridView.Rows[curRowIdx] );
                    curRowIdx--;
                }
                if ( Pass2DataGridView.Rows.Count > 0 ) {
                    if ( curRowIdx < 0 ) curRowIdx = 0;
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[curRowIdx].Cells["Pass2Code"];
                    Pass2DataGridView.CurrentCell.Selected = true;
                    for ( int idx = curRowIdx; idx < Pass2DataGridView.Rows.Count; idx++ ) {
                        Pass2DataGridView.Rows[idx].Cells["Pass2Seq"].Value = ( idx + 1 ).ToString();
                        Pass2DataGridView.Rows[idx].Cells["Pass2Updated"].Value = "Y";
                    }
                    Pass2DataGridView.Focus();
                } else {
                    isDataModified = true;
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                    curTimerObj.Start();
                    insertTrick( Pass2DataGridView, false );
                }
            }
        }

        private bool deleteTrickPass( String inPassPK ) {
            String curMethodName = "Trick:ScoreCalc:deleteTrickPass";
            String curMsg = "";
            bool curReturnStatus = false;
            int curPK = 0;

            try {
                curPK = Convert.ToInt32( inPassPK );
                if ( curPK > 0 ) {
                    try {
                        String curSqlStmt = "Delete TrickPass Where PK = " + curPK.ToString();
                        int rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt );
                        if ( rowsProc > 0 ) {
                            curReturnStatus = true;
                        }
                    } catch ( Exception excp ) {
                        curMsg = ":Error attempting to delete trick pass \n" + excp.Message ;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                } else {
                    curReturnStatus = true;
                }
            } catch ( Exception excp ) {
                curMsg = ":PK value not set therefore must not be a valid row \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            return curReturnStatus;
        }

        private void navSort_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( isDataModified ) {
                MessageBox.Show( "Data has been modified.  Request cannot be completed." );
            } else {
                // Display the form as a modal dialog box.
                sortDialogForm.SortCommand = myTourProperties.RunningOrderSortTrick;
                sortDialogForm.ShowDialog( this );

                // Determine if the OK button was clicked on the dialog box.
                if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                    mySortCommand = sortDialogForm.SortCommand;
                    myTourProperties.RunningOrderSortTrick = mySortCommand;
                    loadTourEventRegView( myEventRegDataTable, mySortCommand, myFilterCmd );
                    winStatusMsg.Text = "Sorted by " + mySortCommand;
                }
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( isDataModified ) {
                MessageBox.Show( "Data has been modified.  Request cannot be completed." );
            } else {
                // Display the form as a modal dialog box.
                filterDialogForm.ShowDialog( this );

                // Determine if the OK button was clicked on the dialog box.
                if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                    myFilterCmd = filterDialogForm.FilterCommand;
                    loadTourEventRegView( myEventRegDataTable, mySortCommand, myFilterCmd );
                }
            }
        }

        private void navExportRecord_Click(object sender, EventArgs e) {
            ExportRecordAppData myExportData = new ExportRecordAppData();
            DataGridViewRow curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
            String curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
            byte curRound = Convert.ToByte( (String)Pass1DataGridView.Rows[0].Cells["Pass1Round"].Value );
            myExportData.ExportData( mySanctionNum, "Trick", curMemberId, curAgeGroup, curEventGroup, curRound );
        }

        private void navExport_Click(object sender, EventArgs e) {
            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( isDataModified ) {
                MessageBox.Show( "Data has been modified.  Request cannot be completed." );
            } else {
                String curFilterCmd = myFilterCmd;
                if (curFilterCmd.Contains( "Div =" )) {
                    curFilterCmd = curFilterCmd.Replace( "Div =", "AgeGroup =" );
                }
                ExportData myExportData = new ExportData();
                String[] curSelectCommand = new String[8];
                String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "TrickScore", "TrickPass", "TourReg", "OfficialWork", "OfficialWorkAsgmt" };

                curSelectCommand[0] = "SELECT * FROM TourReg "
                    + "Where SanctionId = '" + mySanctionNum + "' "
                    + "And EXISTS (SELECT 1 FROM EventReg "
                    + "    WHERE TourReg.SanctionId = EventReg.SanctionId AND TourReg.MemberId = EventReg.MemberId "
                    + "      AND TourReg.AgeGroup = EventReg.AgeGroup AND EventReg.Event = 'Trick' ";
                if ( isObjectEmpty( curFilterCmd ) ) {
                    curSelectCommand[0] = curSelectCommand[0] + ") ";
                } else {
                    if ( curFilterCmd.Length > 0 ) {
                        curSelectCommand[0] = curSelectCommand[0] + "And " + curFilterCmd + ") ";
                    } else {
                        curSelectCommand[0] = curSelectCommand[0] + ") ";
                    }
                }

                curSelectCommand[1] = "Select * from EventReg ";
                if ( isObjectEmpty( curFilterCmd ) ) {
                    curSelectCommand[1] = curSelectCommand[1]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Trick'";
                } else {
                    if ( curFilterCmd.Length > 0 ) {
                        curSelectCommand[1] = curSelectCommand[1]
                            + " Where SanctionId = '" + mySanctionNum + "'"
                            + " And Event = 'Trick'"
                            + " And " + curFilterCmd;
                    } else {
                        curSelectCommand[1] = curSelectCommand[1]
                            + " Where SanctionId = '" + mySanctionNum + "'"
                            + " And Event = 'Trick'";
                    }
                }

                curSelectCommand[2] = "Select * from EventRunOrder ";
                if (isObjectEmpty( curFilterCmd )) {
                    curSelectCommand[2] = curSelectCommand[2]
                        + " Where SanctionId = '" + mySanctionNum + "'"
                        + " And Event = 'Trick' And Round = " + roundActiveSelect.RoundValue + " ";
                } else {
                    if (curFilterCmd.Length > 0) {
                        curSelectCommand[2] = curSelectCommand[2]
                            + " Where SanctionId = '" + mySanctionNum + "'"
                            + " And Event = 'Trick' And Round = " + roundActiveSelect.RoundValue + " "
                            + " And " + curFilterCmd;
                    } else {
                        curSelectCommand[2] = curSelectCommand[2]
                            + " Where SanctionId = '" + mySanctionNum + "'"
                            + " And Event = 'Trick' And Round = " + roundActiveSelect.RoundValue + " ";
                    }
                }

                curSelectCommand[3] = "SELECT * FROM TrickScore "
                    + "Where SanctionId = '" + mySanctionNum + "' And Round = " + roundActiveSelect.RoundValue + " "
                    + "And EXISTS (SELECT 1 FROM EventReg "
                    + "    WHERE TrickScore.SanctionId = EventReg.SanctionId AND TrickScore.MemberId = EventReg.MemberId "
                    + "      AND TrickScore.AgeGroup = EventReg.AgeGroup AND EventReg.Event = 'Trick' ";
                if ( isObjectEmpty( curFilterCmd ) ) {
                    curSelectCommand[3] = curSelectCommand[3] + ") ";
                } else {
                    if ( curFilterCmd.Length > 0 ) {
                        curSelectCommand[3] = curSelectCommand[3] + "And " + curFilterCmd + ") ";
                    } else {
                        curSelectCommand[3] = curSelectCommand[3] + ") ";
                    }
                }

                curSelectCommand[4] = "SELECT * FROM TrickPass "
                    + "Where SanctionId = '" + mySanctionNum + "' And Round = " + roundActiveSelect.RoundValue + " "
                    + "And EXISTS (SELECT 1 FROM EventReg "
                    + "    WHERE TrickPass.SanctionId = EventReg.SanctionId AND TrickPass.MemberId = EventReg.MemberId "
                    + "      AND TrickPass.AgeGroup = EventReg.AgeGroup AND EventReg.Event = 'Trick' ";
                if ( isObjectEmpty( curFilterCmd ) ) {
                    curSelectCommand[4] = curSelectCommand[4] + ") ";
                } else {
                    if ( curFilterCmd.Length > 0 ) {
                        curSelectCommand[4] = curSelectCommand[4] + "And " + curFilterCmd + ") ";
                    } else {
                        curSelectCommand[4] = curSelectCommand[4] + ") ";
                    }
                }

                String tmpFilterCmd = "";
                String curEventGroup = EventGroupList.SelectedItem.ToString();
                if ( curEventGroup.ToLower().Equals( "all" ) ) {
                } else {
                    tmpFilterCmd = "And EventGroup = '" + curEventGroup + "' ";
                }

                curSelectCommand[5] = "SELECT * FROM TourReg T "
                    + "Where SanctionId = '" + mySanctionNum + "' "
                    + "And EXISTS (SELECT 1 FROM OfficialWorkAsgmt O "
                    + "    WHERE T.SanctionId = O.SanctionId AND T.MemberId = O.MemberId And O.Event = 'Trick' And O.Round = " + roundActiveSelect.RoundValue + " ";
                if ( isObjectEmpty( tmpFilterCmd ) ) {
                    curSelectCommand[5] = curSelectCommand[5] + ") ";
                } else {
                    if ( tmpFilterCmd.Length > 0 ) {
                        curSelectCommand[5] = curSelectCommand[5] + tmpFilterCmd + ") ";
                    } else {
                        curSelectCommand[5] = curSelectCommand[5] + ") ";
                    }
                }

                //----------------------------------------
                //Export data related to officials
                //----------------------------------------
                curSelectCommand[6] = "Select * from OfficialWork W Where SanctionId = '" + mySanctionNum + "' "
                    + "And W.LastUpdateDate is not null "
                    + "And EXISTS (SELECT 1 FROM EventReg R"
                    + "    WHERE W.SanctionId = R.SanctionId AND W.MemberId = R.MemberId AND R.Event = 'Trick' ";
                if (isObjectEmpty( tmpFilterCmd )) {
                    curSelectCommand[6] = curSelectCommand[6] + ") ";
                } else {
                    if (tmpFilterCmd.Length > 0) {
                        curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + ") ";
                    } else {
                        curSelectCommand[6] = curSelectCommand[6] + ") ";
                    }
                }
                curSelectCommand[6] = curSelectCommand[6] + "Union "
                    + "Select * from OfficialWork W Where SanctionId = '" + mySanctionNum + "' "
                    + "And W.LastUpdateDate is not null "
                    + "And EXISTS (SELECT 1 FROM OfficialWorkAsgmt O "
                    + "    WHERE W.SanctionId = O.SanctionId AND W.MemberId = O.MemberId And O.Event = 'Trick' And O.Round = " + roundActiveSelect.RoundValue + " ";
                if (isObjectEmpty( tmpFilterCmd )) {
                    curSelectCommand[6] = curSelectCommand[6] + ") ";
                } else {
                    if (tmpFilterCmd.Length > 0) {
                        curSelectCommand[6] = curSelectCommand[6] + tmpFilterCmd + ") ";
                    } else {
                        curSelectCommand[6] = curSelectCommand[6] + ") ";
                    }
                }

                curSelectCommand[7] = "Select * from OfficialWorkAsgmt "
                    + " Where SanctionId = '" + mySanctionNum + "' And Event = 'Trick' And Round = " + roundActiveSelect.RoundValue + " ";
                if ( isObjectEmpty( tmpFilterCmd ) ) {
                } else {
                    if ( tmpFilterCmd.Length > 0 ) {
                        curSelectCommand[7] = curSelectCommand[7] + tmpFilterCmd;
                    } else {
                    }
                }

                myExportData.exportData( curTableName, curSelectCommand );
            }
        }

        private void ViewTrickListButton_Click(object sender, EventArgs e) {
            // Set the Parent Form and display requested form
            TrickListMaint curForm = new TrickListMaint();
            curForm.MdiParent = this.MdiParent;
            curForm.Show();
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
            
            roundSelect.RoundValue = roundActiveSelect.RoundValue;
            getEventRegData( curGroupValue,  Convert.ToByte( roundSelect.RoundValue ) );
            loadTourEventRegView( myEventRegDataTable, mySortCommand, myFilterCmd );
            if ( myEventRegDataTable.Rows.Count > 0 ) {
                setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundSelect.RoundValue ) );
                setTrickPassEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundSelect.RoundValue ) );
            }
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
                            ExportLiveWeb.exportCurrentSkierTrick( mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup );
                        } catch {
                        }
                    }
                    if (ExportLiveTwitter.TwitterLocation.Length > 1) {
                        if (( myTourRules.ToLower().Equals( "ncwsa" ) && Pass1DataGridView.Rows.Count > 0 )
                            || ( !( myTourRules.ToLower().Equals( "ncwsa" ) ) && Pass1DataGridView.Rows.Count > 0 && Pass2DataGridView.Rows.Count > 0 )
                            ) {
                            StringBuilder curTwitterMessage = new StringBuilder( "" );
                            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                            String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                            curTwitterMessage.Append( mySanctionNum + " " + curAgeGroup + " " + activeSkierName.Text );
                            if (curTeamCode.Length > 0) {
                                curTwitterMessage.Append( " Team: " + curTeamCode );
                            }
                            curTwitterMessage.Append( " Score: " + scoreTextBox.Text + " Pass 1: " + scorePass1.Text + " Pass 2: " + scorePass2.Text );
                            curTwitterMessage.Append( " REPOST UNOFFICIAL" );
                            ExportLiveTwitter.sendMessage( curTwitterMessage.ToString() );
                        }
                    }
                } else if (ExportLiveWeb.LiveWebDialog.ActionCmd.Equals( "ResendAll" )) {
                    if (ExportLiveWeb.LiveWebLocation.Length > 1) {
                        try {
                            String curEventGroup = EventGroupList.SelectedItem.ToString();
                            byte curRound = Convert.ToByte( roundSelect.RoundValue );
                            ExportLiveWeb.exportCurrentSkiers( "Trick", mySanctionNum, curRound, curEventGroup );
                        } catch {
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
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Trick' And Round = " + inRound.ToString() + " "
                + "Order by EventGroup";
            DataTable curDataTable = getData( curSqlStmt );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                    + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Trick'"
                    + "Order by EventGroup";
                curDataTable = getData( curSqlStmt );
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

        private void loadTourEventRegView(DataTable inEventRegDataTable, String inSortCmd, String inFilterCmd) {
            DataGridViewRow curViewRow;

            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                if ( isDataModified ) {
                    try {
                        CalcScoreButton_Click( null, null );
                    } catch ( Exception excp ) {
                        MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    }
                }
                if ( isDataModified ) {
                    MessageBox.Show( "Data has been modified.  Request cannot be completed." );
                    //TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells[e.ColumnIndex];
                } else {
                    if ( inEventRegDataTable.Rows.Count > 0 ) {
                        Pass1DataGridView.Rows.Clear();
                        Pass2DataGridView.Rows.Clear();
                        TourEventRegDataGridView.Rows.Clear();

                        inEventRegDataTable.DefaultView.Sort = inSortCmd;
                        inEventRegDataTable.DefaultView.RowFilter = inFilterCmd;
                        DataTable curEventRegDataTable = inEventRegDataTable.DefaultView.ToTable();
                        myEventRegDataTable = curEventRegDataTable;

                        myEventRegViewIdx = 0;
                        foreach ( DataRow curDataRow in curEventRegDataTable.Rows ) {
                            myEventRegViewIdx = TourEventRegDataGridView.Rows.Add();
                            curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
                            curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
                            curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                            curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                            curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                            curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                            curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                            curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                            curViewRow.Cells["RunOrder"].Value = ( (Int16)curDataRow["RunOrder"] ).ToString();
                            try {
                                curViewRow.Cells["TeamCode"].Value = (String)curDataRow["TeamCode"];
                            } catch {
                                curViewRow.Cells["TeamCode"].Value = "";
                            }
                            curViewRow.Cells["EventClass"].Value = (String)curDataRow["EventClass"];
                            try {
                                curViewRow.Cells["Score"].Value = ( (Int16)curDataRow["Score"] ).ToString( "####0" );
                                curViewRow.Cells["Score"].ToolTipText = ( (DateTime)curDataRow["LastUpdateDate"] ).ToString( "MM/dd HH:mm:ss" );
                            } catch {
                                curViewRow.Cells["Score"].Value = "";
                                curViewRow.Cells["Score"].ToolTipText = "";
                            }
                            //LastUpdateDate


                            try {
                                curViewRow.Cells["RankingScore"].Value = ( (Decimal)curDataRow["RankingScore"] ).ToString( "####0" );
                            } catch {
                                curViewRow.Cells["RankingScore"].Value = "";
                            }
                            try {
                                curViewRow.Cells["RankingRating"].Value = (String)curDataRow["RankingRating"];
                            } catch {
                                curViewRow.Cells["RankingRating"].Value = "";
                            }
                            try {
                                curViewRow.Cells["HCapBase"].Value = ( (Decimal)curDataRow["HCapBase"] ).ToString( "##,###.0" );
                            } catch {
                                curViewRow.Cells["HCapBase"].Value = ".0";
                            }
                            try {
                                curViewRow.Cells["HCapScore"].Value = ( (Decimal)curDataRow["HCapScore"] ).ToString( "##,###.0" );
                            } catch {
                                curViewRow.Cells["HCapScore"].Value = ".0";
                            }
                            try {
                                curViewRow.Cells["TrickBoat"].Value = (String)curDataRow["TrickBoat"];
                            } catch {
                                curViewRow.Cells["TrickBoat"].Value = "";
                            }
                            try {
                                setEventRegRowStatus( (String)curDataRow["Status"], curViewRow );
                            } catch {
                                setEventRegRowStatus( "TBD", curViewRow );
                            }
                        }

                        if ( inEventRegDataTable.Rows.Count > 0 ) {
                            myEventRegViewIdx = 0;
                            TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];

                            roundSelect.RoundValue = roundActiveSelect.RoundValue;
                            setTrickScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                            setTrickPassEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                        }
                    } else {
                        Pass1DataGridView.Rows.Clear();
                        Pass2DataGridView.Rows.Clear();
                        TourEventRegDataGridView.Rows.Clear();
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving trick tournament entries \n" + ex.Message );
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
                Log.WriteFile( "Trick:ScoreCalc:Event delayed: " + EventDelayReasonTextBox.Text );
            }
            EventDelayReasonTextBox.Visible = false;
            EventDelayReasonLabel.Visible = false;
            StartTimerButton.Visible = false;
            PauseTimerButton.Visible = true;
        }

        private void roundSelect_Load( object sender, EventArgs e ) {
            if ( myScoreDataTable.Rows.Count > 0 ) {
                roundSelect.RoundValue = (String)myScoreRow["Round"];
            }
        }

        private void roundActiveSelect_Load(object sender, EventArgs e) {
        }

        private void roundSelect_Click(object sender, EventArgs e) {
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
                        roundSelect.RoundValue = curValue;
                    } catch ( Exception ex ) {
                        curValue = ex.Message;
                    }
                }
                if ( TourEventRegDataGridView.CurrentRow != null ) {
                    if ( !( isObjectEmpty( TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value ) ) ) {
                        setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundSelect.RoundValue ) );
                        setTrickPassEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundSelect.RoundValue ) );
                    }
                }
            }
        }

        private void noteTextBox_TextChanged(object sender, EventArgs e) {
            isDataModified = true;
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
                    } catch ( Exception ex ) {
                        curValue = ex.Message;
                    }
                }
                loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

                int curSaveEventRegViewIdx = myEventRegViewIdx;
                navRefresh_Click( null, null );

                if ( curSaveEventRegViewIdx > 0 ) {
                    myEventRegViewIdx = curSaveEventRegViewIdx;
                    if (TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx) {
                        myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                    }
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                    roundSelect.RoundValue = roundActiveSelect.RoundValue;
                    setTrickScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    setTrickPassEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
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
                        myEventRegViewIdx = curSaveEventRegViewIdx;
                        if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                            myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                        }
                        TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                        roundSelect.RoundValue = roundActiveSelect.RoundValue;
                        setTrickScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                        setTrickPassEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    }
                }
            }
        }

        private void scoreEventClass_SelectedIndexChanged(object sender, EventArgs e) {
            CalcScoreButton.Focus();
        }

        private void scoreEventClass_Validating(object sender, CancelEventArgs e) {
            String curMethodName = "Trick:ScoreCalc:scoreEventClass_Validating";
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
                        curSqlStmt.Append( "Update TrickScore Set " );
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

            }
        }

        private void DataGridView_Enter( object sender, EventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            curPassView.DefaultCellStyle.BackColor = SystemColors.Window;
            curPassView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            String curColNameFull = curPassView.Name;
            String curColPrefix = curColNameFull.Substring(0, 5);
            String curViewName = curColNameFull.Substring(5);
            myActiveTrickPass = curColPrefix;

        }

        private void DataGridView_Leave( object sender, EventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            if ( isDataModified && !(isDataModifiedInProgress) ) {
                CalcScoreButton_Click( null, null );
            }
            curPassView.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            curPassView.DefaultCellStyle.ForeColor = Color.Silver;
        }

        private void TourEventRegDataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            DataGridView myDataView = (DataGridView)sender;

            //Update data if changes are detected
            //if ( isDataModified && ( myEventRegViewIdx != e.RowIndex ) ) {
            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            if ( myEventRegViewIdx != e.RowIndex ) {
                myEventRegViewIdx = e.RowIndex;
                int curRowPos = e.RowIndex + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
                if ( !( isObjectEmpty( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
                    roundSelect.RoundValue = roundActiveSelect.RoundValue;
                    setTrickScoreEntry( myDataView.Rows[e.RowIndex], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    setTrickPassEntry( myDataView.Rows[e.RowIndex], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    isDataModified = false;
                }
            }
        }

        public void setTrickScoreEntry( DataGridViewRow inTourEventRegRow, Byte inRound ) {
            isDataModified = false;
            isLoadInProg = true;
            String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
            activeSkierName.Text = (String)inTourEventRegRow.Cells["SkierName"].Value;
            this.Text = myTitle + " - " + activeSkierName.Text;

            getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
            if ( myScoreDataTable.Rows.Count > 0 ) {
                myScoreRow = myScoreDataTable.Rows[0];
                roundSelect.RoundValue = ((Byte)myScoreRow["Round"]).ToString();
            } else {
                myScoreRow = null;

                roundSelect.RoundValue = inRound.ToString();
                DataRowView newDataRow = myScoreDataTable.DefaultView.AddNew();
                newDataRow["SanctionId"] = mySanctionNum;
                newDataRow["MemberId"] = curMemberId;
                newDataRow["AgeGroup"] = curAgeGroup;
                newDataRow["Round"] = inRound;
                newDataRow["EventClass"] = (String)inTourEventRegRow.Cells["EventClass"].Value;

                newDataRow["PK"] = -1;
                newDataRow["Score"] = 0;
                newDataRow["ScorePass1"] = 0;
                newDataRow["ScorePass2"] = 0;
                newDataRow["NopsScore"] = 0;
                newDataRow["Rating"] = "";
                newDataRow["Boat"] = "";
                newDataRow["Note"] = "";
                newDataRow.EndEdit();
                myScoreRow = myScoreDataTable.Rows[0];
            }

            try {
                scoreEventClass.SelectedValue = ((String)myScoreRow["EventClass"]).ToString();
            } catch {
                scoreEventClass.SelectedValue = (String)inTourEventRegRow.Cells["EventClass"].Value;
            }
            try {
                scoreTextBox.Text = ( (Int16)myScoreRow["Score"] ).ToString();
            } catch {
                scoreTextBox.Text = "0";
            }
            try {
                scorePass1.Text = ( (Int16)myScoreRow["ScorePass1"] ).ToString();
            } catch {
                scorePass1.Text = "0";
            }
            try {
                scorePass2.Text = ( (Int16)myScoreRow["ScorePass2"] ).ToString();
            } catch {
                scorePass2.Text = "0";
            }
            try {
                nopsScoreTextBox.Text = ( (Decimal)myScoreRow["NopsScore"] ).ToString( "##,###.00" );
            } catch {
                nopsScoreTextBox.Text = "0";
            }
            try {
                noteTextBox.Text = (String)myScoreRow["Note"];
            } catch {
                noteTextBox.Text = "";
            }
            try {
                TourBoatTextbox.Text = (String)myScoreRow["Boat"];
                foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
                    if ( ( (String)curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( TourBoatTextbox.Text.ToUpper() ) ) {
                        myBoatListIdx = curBoatRow.Index;
                        break;
                    }
                }
            } catch {
                TourBoatTextbox.Text = "";
                foreach ( DataGridViewRow curBoatRow in listApprovedBoatsDataGridView.Rows ) {
                    if ( ( (String)curBoatRow.Cells["BoatCode"].Value ).ToUpper().Equals( "UNDEFINED" ) ) {
                        TourBoatTextbox.Text = (String)curBoatRow.Cells["BoatCode"].Value;
                        myBoatListIdx = curBoatRow.Index;
                        break;
                    }
                }
            }
            isLoadInProg = false;
        }

        public void setTrickPassEntry( DataGridViewRow inTourEventRegRow, byte inRound ) {
            Byte curPass1Num = 1, curPass2Num = 2;
            String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;

            //Populate first pass for current skier
            isLoadInProg = true;
            myPass1DataTable = getSkierPassByRound( curMemberId, curAgeGroup, inRound, curPass1Num );
            if ( myPass1DataTable.Rows.Count > 0 ) {
                setTrickPassValues( Pass1DataGridView, myPass1DataTable );
            } else {
                Pass1DataGridView.Rows.Clear();
                insertTrick( Pass1DataGridView, false );
            }

            //Populate second pass for current skier
            if ( myTourRuleCode.ToLower().Equals( "ncwsa" )
                && ( curAgeGroup.ToLower().Equals( "cm" ) 
                || curAgeGroup.ToLower().Equals( "cw" )
                || curAgeGroup.ToLower().Equals( "bm" )
                || curAgeGroup.ToLower().Equals( "bw" )
                ) ) {
                Pass2DataGridView.Visible = false;
                Pass2DataGridView.Rows.Clear();
            } else {
                Pass2DataGridView.Visible = true;
                isLoadInProg = true;
                myPass2DataTable = getSkierPassByRound( curMemberId, curAgeGroup, inRound, curPass2Num );
                if ( myPass2DataTable.Rows.Count > 0 ) {
                    setTrickPassValues( Pass2DataGridView, myPass2DataTable );
                } else {
                    Pass2DataGridView.Rows.Clear();
                    insertTrick( Pass2DataGridView, false );
                }
            }

            isLoadInProg = true;
            if ( myPass2DataTable != null ) {
                if ( myPass2DataTable.Rows.Count > 0 ) {
                    if ( Pass2DataGridView != null ) {
                        if ( Pass2DataGridView.Rows.Count > 0 ) {
                            Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                        }
                    }
                }
            }
            if ( myPass1DataTable != null ) {
                if ( myPass1DataTable.Rows.Count > 0 ) {
                    if ( Pass1DataGridView != null ) {
                        if ( Pass1DataGridView.Rows.Count > 0 ) {
                            Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
                        }
                    }
                }
            }
            Pass1DataGridView.Select();
            Pass1DataGridView.DefaultCellStyle.BackColor = SystemColors.Window;
            Pass1DataGridView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            Pass2DataGridView.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            Pass2DataGridView.DefaultCellStyle.ForeColor = Color.Silver;
            isLoadInProg = false;
        }

        private void setTrickPassValues( DataGridView inPassView, DataTable inPassDataTable ) {
            isLoadInProg = true;
            Int16 curRuleNum, curNumSkis;
            DataRow curTrickRow;
            isPassEnded = false;
            String curColPrefix = inPassView.Name.Substring(0,5);
            String curSkierClass = (String)( (ListItem)scoreEventClass.SelectedItem ).ItemValue;

            DataGridViewRow curViewRow;
            inPassView.Rows.Clear();

            Int16 curPassScore = 0;
            int curViewIdx = 0;
            foreach ( DataRow curDataRow in inPassDataTable.Rows ) {
                if ( curDataRow["Code"] != System.DBNull.Value ) {
                    curViewIdx = inPassView.Rows.Add();
                    curViewRow = inPassView.Rows[curViewIdx];
                    curViewRow.Cells[curColPrefix + "Updated"].Value = "N";
                    curViewRow.Cells[curColPrefix + "PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                    curViewRow.Cells[curColPrefix + "SanctionId"].Value = (String)curDataRow["SanctionId"];
                    curViewRow.Cells[curColPrefix + "MemberId"].Value = (String)curDataRow["MemberId"];
                    curViewRow.Cells[curColPrefix + "AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                    curViewRow.Cells[curColPrefix + "Round"].Value = ( (Byte)curDataRow["Round"] ).ToString();
                    curViewRow.Cells[curColPrefix + "PassNum"].Value = ( (Byte)curDataRow["PassNum"] ).ToString();
                    curViewRow.Cells[curColPrefix + "Seq"].Value = ( (Byte)curDataRow["Seq"] ).ToString();
                    if ( (byte) curDataRow["Skis"] == 0 ) {
                        curViewRow.Cells[curColPrefix + "Skis"].Value = "W";
                    } else if ( (byte) curDataRow["Skis"] == 9 ) {
                        curViewRow.Cells[curColPrefix + "Skis"].Value = "K";
                    } else {
                        curViewRow.Cells[curColPrefix + "Skis"].Value = ( (byte) curDataRow["Skis"] ).ToString();
                    }
                    curViewRow.Cells[curColPrefix + "Code"].Value = (String)curDataRow["Code"];
                    curViewRow.Cells[curColPrefix + "Results"].Value = (String)curDataRow["Results"];

                    if ( !(curViewRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "CREDIT" ) ) ) {
                        curViewRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                        curViewRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                    }

                    curViewRow.Cells[curColPrefix + "Points"].Value = ( (Int16)curDataRow["Score"] ).ToString();
                    curPassScore += (Int16)curDataRow["Score"];
                    curViewRow.Cells[curColPrefix + "PointsTotal"].Value = curPassScore.ToString();
                    curTrickRow = getTrickRow( curDataRow, curSkierClass );
                    if ( curTrickRow == null ) {
                        curViewRow.Cells[curColPrefix + "StartPos"].Value = "";
                        curViewRow.Cells[curColPrefix + "NumTurns"].Value = "";
                        curViewRow.Cells[curColPrefix + "RuleNum"].Value = "";
                        curViewRow.Cells[curColPrefix + "TypeCode"].Value = "";
                    } else {
                        curNumSkis = (Byte)curDataRow["Skis"];
                        curRuleNum = (Int16)( (Int16) curTrickRow["RuleNum"] + ( ( curNumSkis * 100 ) ) );
                        if ( ( (String)curDataRow["Code"] ).Substring( 0, 1 ).Equals( "R" ) ) curRuleNum = (Int16)( curRuleNum + 200 );
                        curViewRow.Cells[curColPrefix + "StartPos"].Value = ( (Byte)curTrickRow["StartPos"] ).ToString();
                        curViewRow.Cells[curColPrefix + "NumTurns"].Value = ( (Byte)curTrickRow["NumTurns"] ).ToString();
                        curViewRow.Cells[curColPrefix + "RuleNum"].Value = curRuleNum.ToString();
                        curViewRow.Cells[curColPrefix + "TypeCode"].Value = ( (Byte)curTrickRow["TypeCode"] ).ToString();
                    }
                }
            }
            isLoadInProg = false;
        }

        private DataTable getTrickPassValues( DataGridView inPassView) {
            String curColPrefix = inPassView.Name.Substring(0, 5);

            DataTable curDataTable = myPass1DataTable.Clone();
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "StartPos";
            curCol.DataType = System.Type.GetType("System.Byte");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "NumTurns";
            curCol.DataType = System.Type.GetType("System.Byte");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "TypeCode";
            curCol.DataType = System.Type.GetType("System.Byte");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            curCol = new DataColumn();
            curCol.ColumnName = "RuleNum";
            curCol.DataType = System.Type.GetType("System.Int16");
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add(curCol);

            foreach ( DataGridViewRow curViewRow in inPassView.Rows ) {
                DataRow curDataRow = curDataTable.NewRow();
                curDataRow["PK"] = Int64.Parse( (String)curViewRow.Cells[curColPrefix + "PK"].Value );

                curDataRow["SanctionId"] = curViewRow.Cells[curColPrefix + "SanctionId"].Value;
                curDataRow["MemberId"] = curViewRow.Cells[curColPrefix + "MemberId"].Value;
                curDataRow["AgeGroup"] = curViewRow.Cells[curColPrefix + "AgeGroup"].Value;
                curDataRow["Results"] = curViewRow.Cells[curColPrefix + "Results"].Value;
                curDataRow["Code"] = curViewRow.Cells[curColPrefix + "Code"].Value;

                curDataRow["Skis"] = this.myTrickValidation.validateNumSkis((String) curViewRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);

                curDataRow["Round"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "Round"].Value);
                curDataRow["PassNum"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "PassNum"].Value);
                curDataRow["Seq"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "Seq"].Value);

                String curValue = (String) curViewRow.Cells[curColPrefix + "Points"].Value;
                curDataRow["Score"] = Int16.Parse((String) curViewRow.Cells[curColPrefix + "Points"].Value);

                if ( (String) curViewRow.Cells[curColPrefix + "StartPos"].Value != null ) {
                    curDataRow["StartPos"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "StartPos"].Value);
                    curDataRow["NumTurns"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "NumTurns"].Value);
                    curDataRow["TypeCode"] = Byte.Parse((String) curViewRow.Cells[curColPrefix + "TypeCode"].Value);
                    curDataRow["RuleNum"] = Int16.Parse((String) curViewRow.Cells[curColPrefix + "RuleNum"].Value);
                }

                curDataTable.Rows.Add(curDataRow);
            }

            return curDataTable;
        }

        private void setEventRegRowStatus( String inStatus) {
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

        private void DataGridView_KeyUp( object sender, KeyEventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            int curColIdx = 0, curRowIdx = 0;
            String curColNameFull = "", curColPrefix = "", curColName = "";
            if (curPassView.CurrentCell != null) {
                try {
                    curColIdx = curPassView.CurrentCell.ColumnIndex;
                    curRowIdx = curPassView.CurrentCell.RowIndex;
                    curColNameFull = curPassView.Columns[curColIdx].Name;
                    curColPrefix = curColNameFull.Substring( 0, 5 );
                    curColName = curColNameFull.Substring( 5 );
                    bool isLastRow = false;
                    if ( curRowIdx == ( curPassView.Rows.Count - 1 ) ) isLastRow = true;

                    if ( e.KeyCode == Keys.Escape ) {
                        e.Handled = true;
                    } else if ( e.KeyCode == Keys.Enter ) {
                        #region Analyze current column in the data grid when the ENTER key has been pressed
                        if ( curColName.Equals( "Results" ) ) {
                            #region Current column is the results column
                            if ( isPassEnded ) {
                                if ( curColPrefix.Equals( "Pass1" ) ) {
                                    if ( Pass2DataGridView.Visible ) {
                                        if ( Pass2DataGridView.Rows.Count > 1 ) {
                                            isLoadInProg = true;
                                            Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                            isLoadInProg = false;
                                        } else if ( Pass2DataGridView.Rows.Count == 1 ) {
                                            if ( isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value ) ) {
                                                if ( Pass1DataGridView.Rows.Count > 0 ) {
                                                    Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = Pass1DataGridView.Rows[curPassView.Rows.Count - 1].Cells["Pass1Skis"].Value.ToString();
                                                    isLoadInProg = true;
                                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                                    isLoadInProg = false;
                                                } else {
                                                    isLoadInProg = true;
                                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Skis"];
                                                    isLoadInProg = false;
                                                }
                                            } else {
                                                isLoadInProg = true;
                                                Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                                isLoadInProg = false;
                                            }
                                        } else {
                                            //No action required.  Leave last row on pass as is
                                        }
                                        Pass2DataGridView.Select();
                                        Pass2DataGridView.Focus();
                                    } else {
                                        noteTextBox.Focus();
                                    }
                                } else if ( curColPrefix.Equals( "Pass2" ) ) {
                                    if ( Pass2DataGridView.Rows.Count == 1 ) {
                                        isLoadInProg = true;
                                        curPassView.CurrentCell = curPassView.Rows[0].Cells[curColPrefix + "Code"];
                                        isLoadInProg = false;
                                        noteTextBox.Focus();
                                    } else if ( Pass2DataGridView.Rows.Count > 0 ) {
                                        isLoadInProg = true;
                                        curPassView.CurrentCell = curPassView.Rows[Pass2DataGridView.Rows.Count - 1].Cells[curColPrefix + "Code"];
                                        isLoadInProg = false;
                                        noteTextBox.Focus();
                                    }
                                }
                                isPassEnded = false;
                                e.Handled = true;
                            } else {
                                if ( isTrickValid ) {
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 15;
                                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                    curTimerObj.Start();
                                    e.Handled = true;
                                }
                            }
                            #endregion
                        } else if ( curColName.Equals( "Code" ) ) {
                            #region Current column is the trick code column
                            if (isPassEnded) {
                                isLoadInProg = true;
                                curPassView.CurrentCell = curPassView.Rows[0].Cells[curColPrefix + "Code"];
                                isLoadInProg = false;
                                if ( curColPrefix.Equals( "Pass1" ) ) {
                                    if ( Pass1DataGridView.Rows.Count > 0 ) {
                                        isLoadInProg = true;
                                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
                                        isLoadInProg = false;
                                        if ( Pass2DataGridView.Visible ) {
                                            if ( Pass2DataGridView.Rows.Count > 1 ) {
                                                isLoadInProg = true;
                                                Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                                isLoadInProg = false;
                                            } else if ( Pass2DataGridView.Rows.Count == 1 ) {
                                                if ( isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value ) ) {
                                                    Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = Pass1DataGridView.Rows[curPassView.Rows.Count - 1].Cells["Pass1Skis"].Value.ToString();
                                                    isLoadInProg = true;
                                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                                    isLoadInProg = false;
                                                } else {
                                                    isLoadInProg = true;
                                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                                    isLoadInProg = false;
                                                }
                                            } else {
                                                if ( isTrickValid ) {
                                                    Timer curTimerObj = new Timer();
                                                    curTimerObj.Interval = 15;
                                                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                                    curTimerObj.Start();
                                                    e.Handled = true;
                                                }
                                            }
                                            isPassEnded = false;
                                            Pass2DataGridView.Select();
                                            Pass2DataGridView.Focus();
                                            e.Handled = true;
                                        } else {
                                            isPassEnded = false;
                                            noteTextBox.Focus();
                                            e.Handled = true;
                                        }
                                    }
                                } else if ( curColPrefix.Equals( "Pass2" ) ) {
                                    if ( Pass2DataGridView.Rows.Count > 0 ) {
                                        isLoadInProg = true;
                                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                        isLoadInProg = false;
                                    }
                                    noteTextBox.Focus();
                                }
                            } else {
                                if ( isLastRow ) {
                                    if ( isObjectEmpty( curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"].Value ) ) {
                                        //No action required.  Leave last row on pass as is
                                    } else {
                                        if ( isObjectEmpty( curPassView.Rows[curRowIdx].Cells[curColPrefix + "Results"].Value ) ) {
                                            //No action required.  Leave last row on pass as is
                                        } else {
                                            String curResults = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Results"].Value.ToString().ToUpper();
                                            if ( curResults.Equals( "FALL" )
                                                || ( curResults.Equals( "OOC" ) && curRowIdx > 0 )
                                                ) {
                                                //No action required.  Leave last row on pass as is
                                            } else {
                                                if ( isTrickValid ) {
                                                    Timer curTimerObj = new Timer();
                                                    curTimerObj.Interval = 15;
                                                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                                    curTimerObj.Start();
                                                    e.Handled = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        } else if ( curColName.Equals( "Skis" ) ) {
                            #region Current column is the skis column
                            if ( curRowIdx > 0 ) {
                                isLoadInProg = true;
                                curRowIdx--;
                                curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                                isLoadInProg = false;
                                e.Handled = true;
                            } else if (curRowIdx == 0 ) {
                                if ( curColPrefix.Equals( "Pass2" ) ) {
                                    if ( isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value ) ) {
                                        Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = Pass1DataGridView.Rows[curPassView.Rows.Count - 1].Cells["Pass1Skis"].Value.ToString();
                                        isLoadInProg = true;
                                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                        isLoadInProg = false;
                                    } else {
                                        isLoadInProg = true;
                                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                        isLoadInProg = false;
                                    }
                                } else {
                                    if ( isObjectEmpty( Pass1DataGridView.Rows[0].Cells["Pass1Skis"].Value ) ) {
                                        isLoadInProg = true;
                                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Skis"];
                                        isLoadInProg = false;
                                    } else {
                                        isLoadInProg = true;
                                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
                                        isLoadInProg = false;
                                    }
                                }
                            } else {
                                if ( isLastRow ) {
                                    isLoadInProg = true;
                                    curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                                    isLoadInProg = false;
                                    e.Handled = true;
                                }
                            }
                            #endregion
                        }
                        #endregion
                    } else if ( e.KeyCode == Keys.Tab ) {
                        #region Analyze current column in the data grid when the TAB key has been pressed
                        if ( curColName.Equals( "Points" ) ) {
                            if ( isPassEnded ) {
                                curPassView.CurrentCell = curPassView.Rows[0].Cells[curColPrefix + "Code"];
                            } else {
                                if ( isLastRow ) {
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 15;
                                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                    curTimerObj.Start();
                                } else {
                                    curRowIdx++;
                                }
                                isLoadInProg = true;
                                curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                                isLoadInProg = false;
                            }
                            e.Handled = true;
                        }
                        #endregion
                    } else if ( e.KeyCode == Keys.Delete ) {
                        #region Analyze current column in the data grid when the DELETE key has been pressed
                        deleteTrick( curColPrefix, curRowIdx );
                        /*
                        if ( curColName.Equals( "Seq" ) ) {
                            deleteTrick( curColPrefix, curRowIdx );
                        } else if ( curColName.Equals( "Code" ) ) {
                            if ( isObjectEmpty( curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"].Value ) ) {
                                deleteTrick( curColPrefix, curRowIdx );
                            }
                        }
                         */
                        #endregion
                    } else if ( e.KeyCode == Keys.Insert ) {
                        #region Analyze current column in the data grid when the INSERT key has been pressed
                        int newRowIdx = insertTrick( curPassView, true );
                        curPassView.CurrentCell = curPassView.Rows[newRowIdx].Cells[curColPrefix + "Code"];
                        #endregion
                    } else {
                    }
                } catch ( Exception exp ) {
                    MessageBox.Show( exp.Message );
                }
            }

        }

        private void DataGridView_CellEnter(object sender, DataGridViewCellEventArgs e) {
            DataGridView curPassView = (DataGridView)sender;
            curPassView.CurrentCell = curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring(0, 5);
            String curColName = curColNameFull.Substring(5);

            if ( curColName.Equals( "Skis" ) ) {
                try {
                    myOrigSkisValue = (String)curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigSkisValue = "";
                }
            } else if ( curColName.Equals( "Code" ) ) {
                try {
                    myOrigCodeValue = (String)curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigCodeValue = "";
                }
                if ( myOrigCodeValue == null ) myOrigCodeValue = "";
            } else if ( curColName.Equals( "Results" ) ) {
                try {
                    myOrigResultsValue = (String)curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigResultsValue = "";
                }
                if ( myOrigResultsValue == null ) myOrigResultsValue = "";
            }
        }

        private void DataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            DataGridView curPassView = (DataGridView)sender;
            bool curPassFall = false;
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring(0, 5);
            String curColName = curColNameFull.Substring(5);

            if ( !( isLoadInProg ) ) {
                if ( curColName.Equals( "Results" ) ) {
                    if ( this.myTrickValidation.validateResultStatus(e.FormattedValue.ToString(), this.myTourRules) ) {
                        if ( e.FormattedValue.ToString().Equals("Fall")
                            || e.FormattedValue.ToString().Equals("End")
                            ) {
                            isPassEnded = true;
                        } else {
                            if ( e.RowIndex > 1 && e.FormattedValue.ToString().Equals("OOC") ) {
                                isPassEnded = true;
                            }
                        }
                        e.Cancel = false;
                    } else {
                        MessageBox.Show(this.myTrickValidation.ValidationMessage);
                        e.Cancel = true;
                    }
                } else if ( curColName.Equals( "Skis" ) ) {
                    if ( this.myTrickValidation.validateNumSkis(e.FormattedValue.ToString(), this.myTourRules) >= 0 ) {
                        e.Cancel = false;
                    } else {
                        MessageBox.Show(this.myTrickValidation.ValidationMessage);
                        e.Cancel = true;
                    }
                } else if ( curColName.Equals( "Code" ) ) {
                    if ( e.FormattedValue.ToString().Length > 0 ) {
                        if (e.RowIndex > 0) {
                            if (curPassView.Rows[e.RowIndex - 1].Cells[curColPrefix + "Results"].Value.ToString().Equals( "Fall" )) {
                                curPassFall = true;
                            }
                        } else {
                            if (e.FormattedValue.ToString().ToUpper().Equals( "NSP" )) {
                                isPassEnded = true;
                            } else if (e.FormattedValue.ToString().ToUpper().Equals( "FALL" )) {
                                String curSkierClass = (String)( (ListItem)scoreEventClass.SelectedItem ).ItemValue;
                                DataRow curClassRow = getClassRow(curSkierClass);
                                if ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) { 
                                    isPassEnded = true;
                                }
                            }
                        }
                        if ( curPassFall ) {
                            MessageBox.Show( "No tricks allowed after a fall or trick marked as end" );
                            e.Cancel = false;
                        } else {
                            if ( e.FormattedValue.ToString().ToUpper().Equals( "END" ) || e.FormattedValue.ToString().ToUpper().Equals( "HORN" ) ) {
                                isPassEnded = true;
                            } else {
                                if ( !( e.FormattedValue.ToString().ToUpper().Equals( myOrigCodeValue.ToUpper() ) ) ) {
                                    DataGridViewRow curPassRow = curPassView.Rows[e.RowIndex];
                                    curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                                    if ( curPassRow.Cells[curColPrefix + "Skis"].Value == null ) {
                                        e.Cancel = true;
                                    } else {
                                        Int16 curNumSkis = 0;
                                        if ( this.myTrickValidation.validateNumSkis((String)curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules) >= 0 ) {
                                            curNumSkis = this.myTrickValidation.NumSkis;
                                        } else {
                                            MessageBox.Show(this.myTrickValidation.ValidationMessage);
                                            e.Cancel = true;
                                        }
                                    }
                                }
                            }
                        }
                    } else {
                        e.Cancel = false;
                    }
                }
            }
        }

        private void DataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            DataGridView curPassView = (DataGridView)sender;
            DataGridViewRow curPassRow = (DataGridViewRow)curPassView.CurrentRow;
            bool isLastRow = false;
            if ( curPassRow.Index == ( curPassView.Rows.Count - 1 ) ) isLastRow = true;
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring(0, 5);
            String curColName = curColNameFull.Substring(5);

            if ( !( isLoadInProg ) ) {
                if ( curColName.Equals( "Results" ) ) {
                    #region Validation processing for results cell
                    if ( !(isObjectEmpty(curPassView.Rows[e.RowIndex].Cells[curColPrefix + "Code"].Value ))
                        && !(isObjectEmpty(curPassView.Rows[e.RowIndex].Cells[curColPrefix + "Results"].Value ))
                        ) {
                        String curValue = ((String)curPassRow.Cells[e.ColumnIndex].Value).ToUpper();
                        if ( curValue.Equals( "CREDIT" ) ) {
                            if ( !( curValue.Equals( myOrigResultsValue.ToUpper() ) ) ) {
                                    isDataModified = true;
                                if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Skis"].Value))
                                    && !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Code"].Value))
                                    ) {
                                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                    curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
                                    curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = SystemColors.ControlText;
                                    curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = SystemColors.Window;
                                    if ( isLastRow ) {
                                        Timer curTimerObj = new Timer();
                                        curTimerObj.Interval = 15;
                                        curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                        curTimerObj.Start();
                                    }
                                }
                            }
                        } else {
                            if ( !(isObjectEmpty(curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)) ) {
                                if ( !( curValue.Equals( myOrigResultsValue.ToUpper() ) ) ) {
                                    isDataModified = true;
                                }
                                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                curPassRow.Cells[curColPrefix + "Points"].Value = "0";

                                curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                                curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;

                                if ( isPassEnded ) {
                                    isDataModified = true;
                                    isDataModifiedInProgress = true;
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 15;
                                    curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                                    curTimerObj.Start();
                                }
                            }
                        }
                    } else {
                        if ( !(isObjectEmpty(curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)) ) {
                            String curValue = ((String)curPassRow.Cells[e.ColumnIndex].Value).ToUpper();
                            if ( curValue.Equals( "END" ) || curValue.Equals( "HORN" ) ) {
                                isDataModified = true;
                                isDataModifiedInProgress = true;
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 15;
                                curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                                curTimerObj.Start();
                            }
                        }
                    }
                    #endregion
                } else if ( curColName.Equals( "Skis" ) ) {
                    #region Validation processing for skis cell
                    if ( !( isObjectEmpty( curPassRow.Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = (String)curPassRow.Cells[e.ColumnIndex].Value;
                        if ( !( curValue.Equals( myOrigSkisValue ) ) ) {
                            if ( isObjectEmpty(curPassRow.Cells[curColPrefix + "Code"].Value) ) {
                            } else {
                                isDataModified = true;
                                if ( ( curPassRow.Cells[curColPrefix + "Results"].Value ).ToString().ToUpper().Equals( "CREDIT" ) ) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
                                } else {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                                }
                                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                if ( isLastRow ) {
                                    Timer curTimerObj = new Timer();
                                    curTimerObj.Interval = 15;
                                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                    curTimerObj.Start();
                                }
                            }
                        }
                    }
                    #endregion
                } else if ( curColName.Equals( "Code" ) ) {
                    #region Validation processing for trick code cell
                    isPassEnded = false;
                    if ( !( isObjectEmpty( curPassRow.Cells[e.ColumnIndex].Value ) ) ) {
                        String curValue = ((String)curPassRow.Cells[e.ColumnIndex].Value).ToUpper();
                        if ( curValue.Equals( myOrigCodeValue.ToUpper() ) ) {
                            #region No change to trick code but checking other fields to see if they have an impact on the trick code validation
                            curPassRow.Cells[e.ColumnIndex].Value = curValue.ToUpper();
                            if ( !( isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value ) )
                                && !( isObjectEmpty( curPassRow.Cells[curColPrefix + "Results"].Value ) )
                                ) {
                                if ( curValue.Equals( "END" ) || curValue.Equals( "HORN" ) ) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                                    isPassEnded = true;
                                } else if (curValue.Equals( "FALL" )) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                    Int16 curNumSkis = 0;
                                    curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                                    checkTrickCode( curPassView, curPassRow.Index, curValue.ToUpper(), curNumSkis, curColPrefix );
                                    if (curPassRow.Index == 0) {
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                                        String curSkierClass = (String)( (ListItem)scoreEventClass.SelectedItem ).ItemValue;
                                        DataRow curClassRow = getClassRow(curSkierClass);
                                        if ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) {
                                            isPassEnded = true;
                                        } else {
                                            if (curPassView.Name.Equals( "Pass2DataGridView" )) {
                                                if (Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().ToLower().Equals( "fall" )) {
                                                    isPassEnded = true;
                                                }
                                            }
                                        }
                                    } else if (curPassRow.Index == 1) {
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                                        isPassEnded = true;
                                    } else {
                                        isPassEnded = true;
                                    }
                                } else if (curValue.Equals( "NSP" )) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "9";
                                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                    isPassEnded = true;
                                } else {
                                    Int16 curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                                    if ( checkTrickCode( curPassView, curPassRow.Index, curValue.ToUpper(), curNumSkis, curColPrefix ) ) {
                                        isTrickValid = true;
                                    } else {
                                        isTrickValid = false;
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                                        curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                                        curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                                    }
                                }
                            }
                            #endregion

                        } else {
                            #region Trick code has been changed therefore perform validation
                            isDataModified = true;
                            curPassRow.Cells[e.ColumnIndex].Value = curValue.ToUpper();
                            if ( !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Skis"].Value))
                                && !(isObjectEmpty(curPassRow.Cells[curColPrefix + "Results"].Value))
                                ) {
                                if ( curValue.Equals( "END" ) || curValue.Equals( "HORN" ) ) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                                    isPassEnded = true;

                                } else if (curValue.Equals( "FALL" )) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";

                                    Int16 curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                                    checkTrickCode( curPassView, curPassRow.Index, curValue.ToUpper(), curNumSkis, curColPrefix );
                                    if (curPassRow.Index == 0) {
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                                        String curSkierClass = (String)( (ListItem)scoreEventClass.SelectedItem ).ItemValue;
                                        DataRow curClassRow = getClassRow(curSkierClass);
                                        if ( (Decimal) curClassRow["ListCodeNum"] > (Decimal) myClassERow["ListCodeNum"] ) {
                                            isPassEnded = true;
                                        } else {
                                            if (curPassView.Name.Equals( "Pass2DataGridView" )) {
                                                if (Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().ToLower().Equals( "fall" )) {
                                                    isPassEnded = true;
                                                }
                                            }
                                        }
                                    } else if (curPassRow.Index == 1) {
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                                        isPassEnded = true;
                                    } else {
                                        isPassEnded = true;
                                    }

                                } else if (curValue.Equals( "NSP" )) {
                                    curPassRow.Cells[curColPrefix + "Points"].Value = "-1";
                                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                    isPassEnded = true;

                                } else {
                                    Int16 curNumSkis = 0;
                                    if ( this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules) >= 0 ) {
                                        curNumSkis = this.myTrickValidation.NumSkis;
                                        if ( checkTrickCode(curPassView, curPassRow.Index, curValue.ToUpper(), curNumSkis, curColPrefix) ) {
                                            isTrickValid = true;
                                            if ( curPassRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals("CREDIT") ) {
                                                curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints(curPassView, curPassRow, curColPrefix).ToString();
                                            } else {
                                                curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints(curPassView, curPassRow, curColPrefix).ToString();
                                                int curPoints = 0;
                                                Int32.TryParse((String) curPassRow.Cells[curColPrefix + "Points"].Value, out curPoints);
                                                if ( curPoints > 0 ) {
                                                    curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                                                }
                                            }
                                            curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = SystemColors.ControlText;
                                            curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = SystemColors.Window;
                                            curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                                        } else {
                                            isTrickValid = false;
                                            curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                                            curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                                            curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                                        }
                                    } else {
                                        isTrickValid = false;
                                        curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                                        curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                                        curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                                    }
                                }
                            }
                            myOrigCodeValue = (String)curPassRow.Cells[e.ColumnIndex].Value;
                        }
                        #endregion

                        if ( isPassEnded ) {
                            isDataModified = true;
                            isDataModifiedInProgress = true;
                            Timer curTimerObj = new Timer();
                            curTimerObj.Interval = 15;
                            curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                            curTimerObj.Start();
                        }
                    }
                    #endregion
                }
            }
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
        private void fillNewPassRow(DataGridView inPassView, DataGridViewRow inPassRow, Int16 inNumSkis) {
            Int16 curSeqNum, curNumSkis, curPassNum, curRound;
            String curColPrefix = inPassView.Name.Substring(0, 5);
            String curMemberId = (String)TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value;
            int curNewIdx = inPassRow.Index;

            if (curNewIdx > 0) {
                DataGridViewRow curPassRow = (DataGridViewRow)inPassView.Rows[curNewIdx - 1];
                curNumSkis = this.myTrickValidation.validateNumSkis((String) curPassRow.Cells[curColPrefix + "Skis"].Value, this.myTourRules);
                curRound = Convert.ToInt16( curPassRow.Cells[curColPrefix + "Round"].Value);
                curPassNum = Convert.ToInt16( curPassRow.Cells[curColPrefix + "PassNum"].Value);
                curSeqNum = Convert.ToInt16( curPassRow.Cells[curColPrefix + "Seq"].Value );
                curSeqNum++;
            } else {
                curNumSkis = inNumSkis;
                curSeqNum = 1;
                curRound = Convert.ToInt16(roundSelect.RoundValue);
                if (curColPrefix.Equals("Pass1")) {
                    curPassNum = 1;
                } else {
                    curPassNum = 2;
                }
            }

            inPassRow.Cells[curColPrefix + "SanctionId"].Value = mySanctionNum;
            inPassRow.Cells[curColPrefix + "MemberId"].Value = curMemberId;
            inPassRow.Cells[curColPrefix + "Round"].Value = curRound;
            inPassRow.Cells[curColPrefix + "Skis"].Value = curNumSkis;
            inPassRow.Cells[curColPrefix + "Seq"].Value = curSeqNum;
            inPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
            inPassRow.Cells[curColPrefix + "PassNum"].Value = curPassNum;
            inPassRow.Cells[curColPrefix + "Points"].Value = "0";
            inPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
        }

        private void checkForOverall() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            Int16 tempRound = Convert.ToInt16( roundSelect.RoundValue );
            if ( tempRound > 1 && tempRound < 25 ) {
                curSqlStmt.Append( "SELECT R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd, COUNT(*) AS NumSkierEvents " );
                curSqlStmt.Append( "FROM TourReg AS R " );
                curSqlStmt.Append( "  INNER JOIN EventReg AS E ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
                curSqlStmt.Append( "  LEFT OUTER JOIN NopsData AS N ON R.AgeGroup = N.AgeGroup AND N.Event = 'Trick' AND N.SkiYear = SUBSTRING(R.SanctionId, 1, 2) " );
                curSqlStmt.Append( "WHERE R.SanctionId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SanctionId"].Value.ToString() + "' " );
                curSqlStmt.Append( "  And R.MemberId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString() + "' " );
                curSqlStmt.Append( "  And R.AgeGroup = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() + "' " );
                curSqlStmt.Append( "GROUP BY R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd " );
                curSqlStmt.Append( "HAVING COUNT(*) >= N.EventsReqd " );
                curSqlStmt.Append( "ORDER BY R.SkierName, R.AgeGroup, R.MemberId, N.EventsReqd " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if ( curDataTable.Rows.Count > 0 ) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT PK FROM TrickScore " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["SanctionId"].Value.ToString() + "'" );
                    curSqlStmt.Append( "  And MemberId = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString() + "' " );
                    curSqlStmt.Append( "  And AgeGroup = '" + TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() + "' " );
                    curSqlStmt.Append( "  And Round < " + roundSelect.RoundValue );
                    DataTable curDataTable2 = getData( curSqlStmt.ToString() );
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
        }
        
        private bool checkTrickCode ( DataGridView inPassView, int inPassRowIdx, String inCode, Int16 inNumSkis, String inColPrefix ) {
            String curSkierClass = (String) ( (ListItem) scoreEventClass.SelectedItem ).ItemValue;
            int curIdx = inPassRowIdx;
            DataRow[] prevTrickRows = new DataRow[0];

            if ( curIdx > 0 ) {
                if ( curIdx > 1 ) {
                    prevTrickRows = new DataRow[2];
                } else {
                    prevTrickRows = new DataRow[1];
                }

                curIdx--;
                //NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode
                prevTrickRows[0] = myTrickListDataTable.NewRow();
                prevTrickRows[0]["TrickCode"] = (String) inPassView.Rows[curIdx].Cells[inColPrefix + "Code"].Value;
                prevTrickRows[0]["NumSkis"] = this.myTrickValidation.validateNumSkis((String) inPassView.Rows[curIdx].Cells[inColPrefix + "Skis"].Value, this.myTourRules);
                prevTrickRows[0]["Points"] = Convert.ToInt16(inPassView.Rows[curIdx].Cells[inColPrefix + "Points"].Value.ToString());
                if ( (String) inPassView.Rows[curIdx].Cells[inColPrefix + "StartPos"].Value != null ) {
                    prevTrickRows[0]["StartPos"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "StartPos"].Value);
                    prevTrickRows[0]["NumTurns"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "NumTurns"].Value);
                    prevTrickRows[0]["TypeCode"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "TypeCode"].Value);
                    prevTrickRows[0]["RuleNum"] = Convert.ToInt16(inPassView.Rows[curIdx].Cells[inColPrefix + "RuleNum"].Value.ToString());
                }


                if ( curIdx > 0 ) {
                    curIdx--;
                    //NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode
                    prevTrickRows[1] = myTrickListDataTable.NewRow();
                    prevTrickRows[1]["TrickCode"] = (String) inPassView.Rows[curIdx].Cells[inColPrefix + "Code"].Value;
                    prevTrickRows[1]["Points"] = Convert.ToInt16(inPassView.Rows[curIdx].Cells[inColPrefix + "Points"].Value.ToString());
                    if ( (String) inPassView.Rows[curIdx].Cells[inColPrefix + "StartPos"].Value != null ) {
                        prevTrickRows[1]["StartPos"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "StartPos"].Value);
                        prevTrickRows[1]["NumTurns"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "NumTurns"].Value);
                        prevTrickRows[1]["TypeCode"] = Convert.ToInt16((String) inPassView.Rows[curIdx].Cells[inColPrefix + "TypeCode"].Value);
                        prevTrickRows[1]["RuleNum"] = Convert.ToInt16(inPassView.Rows[curIdx].Cells[inColPrefix + "RuleNum"].Value.ToString());
                        prevTrickRows[1]["NumSkis"] = this.myTrickValidation.validateNumSkis((String) inPassView.Rows[curIdx].Cells[inColPrefix + "Skis"].Value, this.myTourRules);
                    }
                }
            }

            /*
              Note: If return code is true than use method UpdatedTrickCode to retrieve the trick code as modified by the validation processing
                    If return code is false then use method ValidationMessage to retrieve the message associated with the reason it failed validation
            */
            curIdx = inPassRowIdx;
            bool returnStatus = myTrickValidation.validateTrickCode(inCode, inNumSkis, curSkierClass, prevTrickRows);
            if ( returnStatus ) {
                inPassView.Rows[curIdx].Cells[inColPrefix + "Code"].Value = myTrickValidation.UpdatedTrickCode;
            } else {
                MessageBox.Show(myTrickValidation.ValidationMessage);
            }

            return returnStatus;
        }

        private Int16 calcPoints( DataGridView inPassView, DataGridViewRow inPassRow, String inColPrefix ) {
            DataTable curPass1DataTable, curPass2DataTable;
            DataRow curViewRow;

            curPass1DataTable = getTrickPassValues(Pass1DataGridView);
            curPass2DataTable = getTrickPassValues(Pass2DataGridView);
            String curSkierEventClass = (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value;
            if ( inColPrefix.Equals("Pass1") ) {
                curViewRow = curPass1DataTable.Rows[inPassRow.Index];
            } else {
                curViewRow = curPass2DataTable.Rows[inPassRow.Index];
            }

            //calcPoints( DataTable inPass1DataTable, DataTable inPass2DataTable, DataRow inViewRow, int inRowIdx, String inColPrefix, String inSkierClass )
            Int16 returnPoints = myTrickValidation.calcPoints( curPass1DataTable, curPass2DataTable, curViewRow, inPassRow.Index, inColPrefix, curSkierEventClass );
            if ( returnPoints < 0 ) {
                returnPoints = 0;
                MessageBox.Show(myTrickValidation.ValidationMessage);
            } else {
                //Update row on DataGridView using curViewRow which should have been updated
                inPassRow.Cells[inColPrefix + "Code"].Value = curViewRow["Code"];
                inPassRow.Cells[inColPrefix + "Results"].Value = curViewRow["Results"];
                inPassRow.Cells[inColPrefix + "StartPos"].Value = curViewRow["StartPos"].ToString();
                inPassRow.Cells[inColPrefix + "NumTurns"].Value = curViewRow["NumTurns"].ToString();
                inPassRow.Cells[inColPrefix + "TypeCode"].Value = curViewRow["TypeCode"].ToString();
                inPassRow.Cells[inColPrefix + "RuleNum"].Value = curViewRow["RuleNum"].ToString();
                inPassRow.Cells[inColPrefix + "Points"].Value = returnPoints.ToString();

                int passScore = 0;
                int curViewIdx = 0;
                foreach ( DataRow curDataRow in curPass1DataTable.Rows ) {
                    Pass1DataGridView.Rows[curViewIdx].Cells["Pass1Results"].Value = curDataRow["Results"];
                    Pass1DataGridView.Rows[curViewIdx].Cells["Pass1Points"].Value = ( (Int16) curDataRow["Score"] ).ToString();
                    passScore += (Int16) curDataRow["Score"];
                    Pass1DataGridView.Rows[curViewIdx].Cells["Pass1PointsTotal"].Value = passScore.ToString();

                    if ( !( Pass1DataGridView.Rows[curViewIdx].Cells["Pass1Results"].Value.ToString().ToUpper().Equals("CREDIT") ) ) {
                        Pass1DataGridView.Rows[curViewIdx].Cells["Pass1Code"].Style.ForeColor = Color.Red;
                        Pass1DataGridView.Rows[curViewIdx].Cells["Pass1Code"].Style.BackColor = Color.LightBlue;
                    }

                    curViewIdx++;
                }
                passScore = 0;
                curViewIdx = 0;
                foreach ( DataRow curDataRow in curPass2DataTable.Rows ) {
                    Pass2DataGridView.Rows[curViewIdx].Cells["Pass2Results"].Value = curDataRow["Results"];
                    Pass2DataGridView.Rows[curViewIdx].Cells["Pass2Points"].Value = ( (Int16) curDataRow["Score"] ).ToString();
                    passScore += (Int16) curDataRow["Score"];
                    Pass2DataGridView.Rows[curViewIdx].Cells["Pass2PointsTotal"].Value = passScore.ToString();

                    if ( !( Pass2DataGridView.Rows[curViewIdx].Cells["Pass2Results"].Value.ToString().ToUpper().Equals("CREDIT") ) ) {
                        Pass2DataGridView.Rows[curViewIdx].Cells["Pass2Code"].Style.ForeColor = Color.Red;
                        Pass2DataGridView.Rows[curViewIdx].Cells["Pass2Code"].Style.BackColor = Color.LightBlue;
                    }

                    curViewIdx++;
                }

                if ( myTrickValidation.ValidationMessage.Length > 0 ) {
                    MessageBox.Show(myTrickValidation.ValidationMessage);
                }
            }

            return returnPoints;
        }

        private Int16 checkForRepeat( Int16 curRuleNum, Int16 tempRuleNum, DataGridViewRow curViewRow, DataGridViewRow curActivePassRow, String inColPrefix, Int16 curActivePoints ) {
            Int16 retPoints = curActivePoints;
            if ( curRuleNum == tempRuleNum ) {
                retPoints = checkCreditAndPoints( curViewRow, curActivePassRow, inColPrefix, retPoints );
            } else if ( curRuleNum > 100 && curRuleNum < 200 ) {
                if ( ( curRuleNum + 200 ) == tempRuleNum ) {
                    //curViewRow
                    if ( curViewRow.Index < curActivePassRow.Index ) {
                        retPoints = checkCreditAndPoints(curViewRow, curActivePassRow, inColPrefix, retPoints);
                    }
                }
            } else if ( curRuleNum > 200 && curRuleNum < 300 ) {
                if ( ( curRuleNum + 200 ) == tempRuleNum ) {
                    retPoints = checkCreditAndPoints( curViewRow, curActivePassRow, inColPrefix, retPoints );
                }
            }

            return retPoints;
        }
        private Int16 checkCreditAndPoints( DataGridViewRow curViewRow, DataGridViewRow curActivePassRow, String inColPrefix, Int16 curActivePoints ) {
            //If trick repeated determine if previous trick was same number but worth more or less points

            Int16 retPoints = curActivePoints;
            if ( curViewRow.Cells[inColPrefix + "Results"].Value.ToString().Equals( "Credit" ) ) {
                if ( Convert.ToInt16( curViewRow.Cells[inColPrefix + "Points"].Value ) < retPoints ) {
                    if ( inColPrefix.Equals("Pass1") && curViewRow.Index > 0 ) {
                        if ( Pass1DataGridView.Rows[curViewRow.Index - 1].Cells[inColPrefix + "Results"].Value.ToString().Equals("Repeat") ) {
                        } else {
                            curViewRow.Cells[inColPrefix + "Points"].Value = "0";
                            curViewRow.Cells[inColPrefix + "Results"].Value = "Repeat";
                        }
                    } else if ( inColPrefix.Equals("Pass2") && curViewRow.Index > 0 ) {
                        if ( Pass2DataGridView.Rows[curViewRow.Index - 1].Cells[inColPrefix + "Results"].Value.ToString().Equals("Repeat") ) {
                        } else {
                            curViewRow.Cells[inColPrefix + "Points"].Value = "0";
                            curViewRow.Cells[inColPrefix + "Results"].Value = "Repeat";
                        }
                    } else {
                        curViewRow.Cells[inColPrefix + "Points"].Value = "0";
                        curViewRow.Cells[inColPrefix + "Results"].Value = "Repeat";
                    }
                } else {
                    //if ( curActivePassRow.DataGridView.Name.StartsWith("Pass2"))
                    curActivePassRow.Cells[curActivePassRow.DataGridView.Name.Substring(0, 5) + "Results"].Value = "Repeat";
                    retPoints = 0;
                }
            }
            return retPoints;
        }

        private void ViewTrickDataButton_Click( object sender, EventArgs e ) {
            StringBuilder curMsg = new StringBuilder();
            String curColPrefix = "Pass1";
            foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                curMsg = new StringBuilder();
                if ( !( isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value ) ) ) {
                    curMsg.Append( "Pass 1" );
                    try {
                        curMsg.Append( "\n Update Flag = " + (String)curPassRow.Cells[curColPrefix + "Updated"].Value );
                    } catch {
                        curMsg.Append( "\n Update Flag = null" );
                    }
                    try {
                        curMsg.Append( "\n PK = " + (String)curPassRow.Cells[curColPrefix + "PK"].Value );
                    } catch {
                        curMsg.Append( "\n PK = null" );
                    }
                    try {
                        curMsg.Append( "\n PassNum = " + (String)curPassRow.Cells[curColPrefix + "PassNum"].Value );
                    } catch {
                        curMsg.Append( "\n PassNum = null" );
                    }
                    try {
                        curMsg.Append( "\n Seq = " + (String)curPassRow.Cells[curColPrefix + "Seq"].Value );
                    } catch {
                        curMsg.Append( "\n Seq = null" );
                    }
                    try {
                        curMsg.Append( "\n NumSkis = " + curPassRow.Cells[curColPrefix + "Skis"].Value );
                    } catch {
                        curMsg.Append( "\n NumSkis = null" );
                    }
                    try {
                        curMsg.Append( "\n Code = " + (String)curPassRow.Cells[curColPrefix + "Code"].Value );
                    } catch {
                        curMsg.Append( "\n Code = null" );
                    }
                    try {
                        curMsg.Append( "\n Results = " + (String)curPassRow.Cells[curColPrefix + "Results"].Value );
                    } catch {
                        curMsg.Append( "\n Results = null" );
                    }
                    try {
                        curMsg.Append( "\n RuleNum = " + (String)curPassRow.Cells[curColPrefix + "RuleNum"].Value );
                    } catch {
                        curMsg.Append( "\n RuleNum = null" );
                    }
                    try {
                        curMsg.Append( "\n StartPos = " + (String)curPassRow.Cells[curColPrefix + "StartPos"].Value );
                    } catch {
                        curMsg.Append( "\n StartPos = null" );
                    }
                    try {
                        curMsg.Append( "\n NumTurns = " + (String)curPassRow.Cells[curColPrefix + "NumTurns"].Value );
                    } catch {
                        curMsg.Append( "\n NumTurns = null" );
                    }
                    try {
                        curMsg.Append( "\n TypeCodeValue = " + (String)curPassRow.Cells[curColPrefix + "TypeCode"].Value );
                    } catch {
                        curMsg.Append( "\n TypeCodeValue = null" );
                    }
                } else {
                    curMsg.Append( "Null row\n" );
                }
                MessageBox.Show( curMsg.ToString() );
            }

            curColPrefix = "Pass2";
            foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                curMsg = new StringBuilder();
                if ( !( isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value ) ) ) {
                    curMsg.Append( "Pass 2" );
                    try {
                        curMsg.Append( "\n Update Flag = " + (String)curPassRow.Cells[curColPrefix + "Updated"].Value );
                    } catch {
                        curMsg.Append( "\n Update Flag = null" );
                    }
                    try {
                        curMsg.Append( "\n PK = " + (String)curPassRow.Cells[curColPrefix + "PK"].Value );
                    } catch {
                        curMsg.Append( "\n PK = null" );
                    }
                    try {
                        curMsg.Append( "\n PassNum = " + (String)curPassRow.Cells[curColPrefix + "PassNum"].Value );
                    } catch {
                        curMsg.Append( "\n PassNum = null" );
                    }
                    try {
                        curMsg.Append( "\n Seq = " + (String)curPassRow.Cells[curColPrefix + "Seq"].Value );
                    } catch {
                        curMsg.Append( "\n Seq = null" );
                    }
                    try {
                        curMsg.Append( "\n NumSkis = " + curPassRow.Cells[curColPrefix + "Skis"].Value );
                    } catch {
                        curMsg.Append( "\n NumSkis = null" );
                    }
                    try {
                        curMsg.Append( "\n Code = " + (String)curPassRow.Cells[curColPrefix + "Code"].Value );
                    } catch {
                        curMsg.Append( "\n Code = null" );
                    }
                    try {
                        curMsg.Append( "\n Results = " + (String)curPassRow.Cells[curColPrefix + "Results"].Value );
                    } catch {
                        curMsg.Append( "\n Results = null" );
                    }
                    try {
                        curMsg.Append( "\n RuleNum = " + (String)curPassRow.Cells[curColPrefix + "RuleNum"].Value );
                    } catch {
                        curMsg.Append( "\n RuleNum = null" );
                    }
                    try {
                        curMsg.Append( "\n StartPos = " + (String)curPassRow.Cells[curColPrefix + "StartPos"].Value );
                    } catch {
                        curMsg.Append( "\n StartPos = null" );
                    }
                    try {
                        curMsg.Append( "\n NumTurns = " + (String)curPassRow.Cells[curColPrefix + "NumTurns"].Value );
                    } catch {
                        curMsg.Append( "\n NumTurns = null" );
                    }
                    try {
                        curMsg.Append( "\n TypeCodeValue = " + (String)curPassRow.Cells[curColPrefix + "TypeCode"].Value );
                    } catch {
                        curMsg.Append( "\n TypeCodeValue = null" );
                    }
                } else {
                    curMsg.Append( "Null row\n" );
                }
                MessageBox.Show( curMsg.ToString() );
            }
        }

        private void BoatSelectButton_Click( object sender, EventArgs e ) {
            if ( BoatSelectInfoLabel.Visible ) {
                BoatSelectInfoLabel.Visible = false;
                listApprovedBoatsDataGridView.Visible = false;
            } else {
                BoatSelectInfoLabel.Visible = true;
                listApprovedBoatsDataGridView.Visible = true;
                listApprovedBoatsDataGridView.Focus();
                listApprovedBoatsDataGridView.CurrentCell = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"];
            }
        }

        private void ForceCompButton_Click( object sender, EventArgs e ) {
            if (TourEventRegDataGridView.Rows.Count > 0) {
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    if (Pass1DataGridView.Rows.Count == 1) {
                        if (Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().Length == 0) {
                            skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
                            if (skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK) {
                                String curReason = skierDoneReasonDialogForm.ReasonText;
                                if (curReason.Length > 3) {
                                    noteTextBox.Text = curReason;
                                    setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
                                    if (saveTrickScore()) isDataModified = false;
                                    return;
                                } else {
                                    MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                                }
                            } else {
                                MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                            }
                        }
                    }
                }

                CalcScoreButton_Click( null, null );
                if (TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" )) {
                    skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
                    if (skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK) {
                        String curReason = skierDoneReasonDialogForm.ReasonText;
                        if (curReason.Length > 3) {
                            noteTextBox.Text = curReason;
                            setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
                            if (saveTrickScore()) isDataModified = false;
                        } else {
                            MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                        }
                    } else {
                        MessageBox.Show( "Reason for forcing turn complete is required, request bypassed" );
                    }
                }
                return;
            }
        }

        private void listApprovedBoatsDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            myBoatListIdx = e.RowIndex;
            updateBoatSelect();
        }

        private void listApprovedBoatsDataGridView_KeyUp( object sender, KeyEventArgs e ) {
            try {
                if ( e.KeyCode == Keys.Enter ) {
                    myBoatListIdx = listApprovedBoatsDataGridView.CurrentCell.RowIndex;
                    myBoatListIdx--;
                    updateBoatSelect();
                }
            } catch ( Exception exp ) {
                MessageBox.Show( exp.Message );
            }

        }

        private void updateBoatSelect() {
            String curMethodName = "Trick:ScoreCalc:updateBoatSelect";
            String curMsg = "";
            String curValue = (String)listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCode"].Value;
            TourBoatTextbox.Text = curValue;
            TourBoatTextbox.Focus();

            if ( myScoreRow != null ) {
                try {
                    Int64 curScorePK = (Int64)myScoreRow["PK"];
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TrickScore Set Boat = '" + TourBoatTextbox.Text + "'" );
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
            BoatSelectInfoLabel.Visible = false;
        }

        private void navPrintResults_Click( object sender, EventArgs e ) {
            //PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();
            Font[] saveFonts = new Font[2];
            System.Drawing.Color[] saveColors = new System.Drawing.Color[4];

            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

            //Load trick pass entries to print data grid
            int curPrintIdx = 0, curMaxRows = 0;
            if ( Pass1DataGridView.Rows.Count > Pass2DataGridView.Rows.Count ) {
                curMaxRows = Pass1DataGridView.Rows.Count;
            } else {
                curMaxRows = Pass2DataGridView.Rows.Count;
            }
            DataGridViewRow curPrintRow, curPassRow;
            PrintDataGridView.Rows.Clear();
            for ( int curPassIdx = 0; curPassIdx < curMaxRows; curPassIdx++ ) {
                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPassIdx = curPrintIdx;
                if ( curPassIdx < Pass1DataGridView.Rows.Count ) {
                    curPassRow = Pass1DataGridView.Rows[curPassIdx];
                    curPrintRow.Cells["PrintPass1Seq"].Value = curPassRow.Cells["Pass1Seq"].Value.ToString();
                    curPrintRow.Cells["PrintPass1Skis"].Value = curPassRow.Cells["Pass1Skis"].Value.ToString();
                    curPrintRow.Cells["PrintPass1Code"].Value = curPassRow.Cells["Pass1Code"].Value.ToString();
                    curPrintRow.Cells["PrintPass1Results"].Value = curPassRow.Cells["Pass1Results"].Value.ToString();
                    curPrintRow.Cells["PrintPass1Points"].Value = curPassRow.Cells["Pass1Points"].Value.ToString();
                    if (isObjectEmpty( curPassRow.Cells["Pass1PointsTotal"].Value )) {
                        curPrintRow.Cells["PrintPass1PointsTotal"].Value = "";
                    } else {
                        curPrintRow.Cells["PrintPass1PointsTotal"].Value = curPassRow.Cells["Pass1PointsTotal"].Value.ToString();
                    }
                }
                if ( curPassIdx < Pass2DataGridView.Rows.Count ) {
                    curPassRow = Pass2DataGridView.Rows[curPassIdx];
                    curPrintRow.Cells["PrintPass2Seq"].Value = curPassRow.Cells["Pass2Seq"].Value.ToString();
                    curPrintRow.Cells["PrintPass2Skis"].Value = curPassRow.Cells["Pass2Skis"].Value.ToString();
                    curPrintRow.Cells["PrintPass2Code"].Value = curPassRow.Cells["Pass2Code"].Value.ToString();
                    curPrintRow.Cells["PrintPass2Results"].Value = curPassRow.Cells["Pass2Results"].Value.ToString();
                    curPrintRow.Cells["PrintPass2Points"].Value = curPassRow.Cells["Pass2Points"].Value.ToString();
                    if (isObjectEmpty( curPassRow.Cells["Pass2PointsTotal"].Value )) {
                        curPrintRow.Cells["PrintPass2PointsTotal"].Value = "";
                    } else {
                        curPrintRow.Cells["PrintPass2PointsTotal"].Value = curPassRow.Cells["Pass2PointsTotal"].Value.ToString();
                    }
                }
            }

            bool CenterOnPage = false;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );
            Font fontSubTitleLabel = new Font( "Arial Narrow", 12, FontStyle.Bold );
            Font fontSubTitleData = new Font( "Calibri", 12);
            Font fontGridView = new Font( "Calibri", 12 );

            saveColors[0] = Pass1DataGridView.DefaultCellStyle.BackColor;
            saveColors[1] = Pass1DataGridView.DefaultCellStyle.ForeColor;
            saveColors[2] = Pass2DataGridView.DefaultCellStyle.BackColor;
            saveColors[3] = Pass2DataGridView.DefaultCellStyle.ForeColor;

            saveFonts[0] = Pass1DataGridView.DefaultCellStyle.Font;
            saveFonts[1] = Pass2DataGridView.DefaultCellStyle.Font;
            
            Pass1DataGridView.DefaultCellStyle.BackColor = SystemColors.Window;
            Pass1DataGridView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            Pass1DataGridView.DefaultCellStyle.Font = fontGridView;
            Pass2DataGridView.DefaultCellStyle.BackColor = SystemColors.Window;
            Pass2DataGridView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            Pass2DataGridView.DefaultCellStyle.Font = fontGridView;

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = true;
            curPrintDialog.UseEXDialog = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title;
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
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 100, 100, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                
                myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

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

                mySubtitle = new StringRowPrinter( myTitle,
                    0, 0, 625, fontSubTitleLabel.Height,
                    Color.DarkBlue, Color.White, fontPrintTitle, SubtitleStringFormatLeft );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( curSkierName,
                    0, 25, 625, fontPrintTitle.Height,
                    Color.DarkBlue, Color.White, fontPrintTitle, SubtitleStringFormatLeft );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( scoreLabel.Text,
                    0, 50, 65, fontSubTitleLabel.Height,
                    Color.Black, Color.White, fontSubTitleLabel, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scoreTextBox.Text,
                    0, 70, 65, fontSubTitleData.Height,
                    Color.Black, Color.White, fontSubTitleData, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( scorePass1Label.Text,
                    75, 50, 65, fontSubTitleLabel.Height,
                    Color.Black, Color.White, fontSubTitleLabel, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scorePass1.Text,
                    75, 70, 65, fontSubTitleData.Height,
                    Color.Black, Color.White, fontSubTitleData, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( scorePass2Label.Text,
                    150, 50, 65, fontSubTitleLabel.Height,
                    Color.Black, Color.White, fontSubTitleLabel, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( scorePass2.Text,
                    150, 70, 65, scorePass2.Size.Height,
                    Color.Black, Color.White, fontSubTitleData, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( nopsScoreLabel.Text,
                    220, 50, 65, fontSubTitleLabel.Height,
                    Color.Black, Color.White, fontSubTitleLabel, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( nopsScoreTextBox.Text,
                    220, 70, 65, fontSubTitleData.Height,
                    Color.Black, Color.White, fontSubTitleData, SubtitleStringFormatCenter );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                mySubtitle = new StringRowPrinter( "Round " + roundSelect.RoundValue + " Pass 1",
                    0, 100, 325, fontSubTitleLabel.Height,
                    Color.DarkBlue, Color.White, fontSubTitleLabel, SubtitleStringFormatLeft );
                myPrintDataGrid.SubtitleRow = mySubtitle;
                mySubtitle = new StringRowPrinter( "Round " + roundSelect.RoundValue + " Pass 2",
                    325, 100, 325, fontSubTitleData.Height,
                    Color.DarkBlue, Color.White, fontSubTitleLabel, SubtitleStringFormatLeft );
                myPrintDataGrid.SubtitleRow = mySubtitle;

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                myPrintDoc.Print();
                //curPreviewDialog.Document = myPrintDoc;
                //curPreviewDialog.ShowDialog();

                Pass1DataGridView.DefaultCellStyle.BackColor = saveColors[0];
                Pass1DataGridView.DefaultCellStyle.ForeColor = saveColors[1];
                Pass2DataGridView.DefaultCellStyle.BackColor = saveColors[2];
                Pass2DataGridView.DefaultCellStyle.ForeColor = saveColors[3];
                Pass1DataGridView.DefaultCellStyle.Font = saveFonts[0];
                Pass2DataGridView.DefaultCellStyle.Font = saveFonts[1];
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            DataTable curDataTable = getData(curSqlStmt.ToString());
            return curDataTable;
        }

        private void getApprovedTowboats() {
            int curIdx = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq " );
            curSqlStmt.Append( "FROM TourBoatUse BU " );
            curSqlStmt.Append( "INNER JOIN CodeValueList BL ON BL.ListCode = BU.HullId " );
            curSqlStmt.Append( "WHERE BL.ListName = 'ApprovedBoats' AND BU.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Union " );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq " );
            curSqlStmt.Append( "FROM CodeValueList BL " );
            curSqlStmt.Append( "WHERE ListName = 'ApprovedBoats' AND ListCode IN ('--Select--', 'Unlisted') " );
            curSqlStmt.Append( "ORDER BY SortSeq " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

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
                    curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating, E.AgeGroup" );
                    curSqlStmt.Append(", E.HCapBase, E.HCapScore, T.TrickBoat, COALESCE (S.Status, 'TBD') AS Status, S.Score, S.LastUpdateDate, E.AgeGroup as Div");
                    curSqlStmt.Append( ", COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Trick' " );
                } else {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup, E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.AgeGroup, E.HCapBase, E.HCapScore" );
                    curSqlStmt.Append(", T.TrickBoat, COALESCE (S.Status, 'TBD') AS Status, S.Score, S.LastUpdateDate, E.AgeGroup as Div");
                    curSqlStmt.Append(", COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Trick' " );
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

                myEventRegDataTable = getData( curSqlStmt.ToString() );
                curRowCount = myEventRegDataTable.Rows.Count;
                curIdx++;
            }
        }

        private void getSkierScoreByRound( String inMemberId, String inAgeGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, S.Round, S.EventClass");
            curSqlStmt.Append(", S.Score, S.ScorePass1, S.ScorePass2, S.NopsScore, S.Rating, S.Boat, S.Status, S.Note");
            curSqlStmt.Append(", Gender, SkiYearAge ");
            curSqlStmt.Append( "FROM TrickScore S " );
            curSqlStmt.Append( "  INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup ");
            curSqlStmt.Append( "WHERE S.SanctionId = '" + mySanctionNum + "' AND S.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append("  AND S.AgeGroup = '" + inAgeGroup + "' AND S.Round = " + inRound.ToString() + " ");
            curSqlStmt.Append("ORDER BY S.SanctionId, S.MemberId");
            myScoreDataTable = getData( curSqlStmt.ToString() );
        }
        
        private DataTable getSkierPassByRound( String inMemberId, String inAgeGroup, int inRound, byte inPass ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum," );
            curSqlStmt.Append( " Seq, Skis, Code, Results, Score, Note" );
            curSqlStmt.Append( " FROM TrickPass " );
            curSqlStmt.Append( " WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " AND PassNum = " + inPass.ToString() );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierPassByRound( String inMemberId, String inAgeGroup, int inRound, byte inPass, byte inSeq ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum," );
            curSqlStmt.Append( " Seq, Skis, Code, Results, Score, Note" );
            curSqlStmt.Append( " FROM TrickPass " );
            curSqlStmt.Append( " WHERE SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString()  );
            curSqlStmt.Append( "   AND PassNum = " + inPass.ToString() + " AND Seq = " + inSeq.ToString() );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return getData( curSqlStmt.ToString() );
        }

        private DataRow getClassRowCurrentSkier() {
            return getClassRow((String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value);
        }
        private DataRow getClassRow( String inClass ) {
            DataRow[] curRowsFound = mySkierClassList.SkierClassDataTable.Select("ListCode = '" + inClass + "'");
            if ( curRowsFound.Length > 0 ) {
                return curRowsFound[0];
            } else {
                return mySkierClassList.SkierClassDataTable.Select("ListCode = '" + myTourClass + "'")[0];
            }
        }

        private DataRow getTrickRow(DataRow inDataRow, String inSkierClass) {
            return getTrickRow( (String)inDataRow["Code"], Convert.ToInt16((Byte)inDataRow["Skis"]), inSkierClass );
        }
        private DataRow getTrickRow(String inTrickCode, Int16 inNumSkies, String inSkierClass) {
            DataRow curReturnRow = null;
            DataRow[] curFoundList;

            curFoundList = myTrickListDataTable.Select( "TrickCode = '" + inTrickCode + "'" + " AND NumSkis = " + inNumSkies );
            if (curFoundList.Length > 0) {
                curReturnRow = curFoundList[0];

                if ( mySkierClassList.compareClassChange(inSkierClass, "L") <= 0 ) {
                    if ( mySpecialFlipList.Contains(inTrickCode) ) {
                        curReturnRow["NumTurns"] = Convert.ToByte("3");
                    }
                }
            }

            return curReturnRow;
        }
        
        private void getTrickList( String inRuleCode ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode " );
            curSqlStmt.Append( " FROM TrickList " );
            curSqlStmt.Append( " WHERE RuleCode = '" + inRuleCode + "'");
            curSqlStmt.Append( " ORDER BY RuleNum, TrickCode" );
            myTrickListDataTable = getData( curSqlStmt.ToString() );
        }

        private bool checkPassError() {
            String curResults = "";
            String curColPrefix = "Pass1";
            foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                curResults = (String)curPassRow.Cells[curColPrefix + "Results"].Value;
                if ( curResults.Equals( "Unresolved" ) ) {
                    return true;
                }
            }

            curColPrefix = "Pass2";
            foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                curResults = (String)curPassRow.Cells[curColPrefix + "Results"].Value;
                if ( curResults.Equals( "Unresolved" ) ) {
                    return true;
                }
            }

            return false;
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
