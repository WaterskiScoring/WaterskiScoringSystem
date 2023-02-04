using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Trick {
    public partial class ScoreCalc : Form {
        #region instance variables
        private bool isDataModified = false;
        private bool isDataModifiedInProgress = false;
        private bool isPassEnded = false;
        private bool isLoadInProg = false;
        private bool isTrickValid = true;
        private int myEventRegViewIdx = 0;
        private int myBoatListIdx = 0;

        private int mySkierRunCount = 0;
        private int myPassRunCount = 0;
        private int myEventDelaySeconds = 0;

        private DateTime myEventStartTime;
        private DateTime myEventDelayStartTime;

        private String myTitle = "";
        private String myOrigSkisValue = "";
        private String myActiveTrickPass = "";
        private String myOrigCodeValue = "";
        private String myOrigResultsValue = "";
        private String mySortCommand = "";
        private String myFilterCmd = "";
        private String myPrevEventGroup = "";

        private DataTable myTrickListDataTable;
        private DataTable myEventRegDataTable;
        private DataTable myScoreDataTable;
        private DataTable myPass1DataTable;
        private DataTable myPass2DataTable;

        private List<String> myCompletedNotices = new List<String>();

        private DataRow myScoreRow;

        private TourProperties myTourProperties;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private CalcNops appNopsCalc;
        private TrickValidation myTrickValidation;
        private List<Common.ScoreEntry> ScoreList = new List<Common.ScoreEntry>();
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;
        private SkierDoneReason skierDoneReasonDialogForm;
        private CheckOfficials myCheckOfficials;
        #endregion

        public ScoreCalc() {
            InitializeComponent();

            appNopsCalc = CalcNops.Instance;
            appNopsCalc.LoadDataForTour();

            myTrickValidation = new TrickValidation();

            ScoreList.Add( new Common.ScoreEntry( "Trick", 0, "", 0 ) );
        }

        private void ScoreCalc_Load( object sender, EventArgs e ) {
            if ( !( TrickEventData.setEventData() ) ) {
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 15;
                curTimerObj.Tick += new EventHandler( CloseWindowTimer );
                curTimerObj.Start();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            if ( Properties.Settings.Default.TrickCalc_Width > 0 ) this.Width = Properties.Settings.Default.TrickCalc_Width;
            if ( Properties.Settings.Default.TrickCalc_Height > 0 ) this.Height = Properties.Settings.Default.TrickCalc_Height;
            if ( Properties.Settings.Default.TrickCalc_Location.X > 0 && Properties.Settings.Default.TrickCalc_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TrickCalc_Location;
            }

            myTitle = this.Text;
            TeamCode.Visible = false;
            if ( TrickEventData.isCollegiateEvent() ) TeamCode.Visible = true;

            myTourProperties = TourProperties.Instance;
            mySortCommand = myTourProperties.RunningOrderSortTrick;

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

            int curDelim = mySortCommand.IndexOf( "AgeGroup" );
            if ( curDelim == 0 ) {
                mySortCommand = "DivOrder" + mySortCommand.Substring( "AgeGroup".Length );
                myTourProperties.RunningOrderSortTrick = mySortCommand;

            } else if ( curDelim > 0 ) {
                mySortCommand = mySortCommand.Substring( 0, curDelim ) + "DivOrder" + mySortCommand.Substring( curDelim + "AgeGroup".Length );
                myTourProperties.RunningOrderSortTrick = mySortCommand;
            }

            String[] curList = { "SkierName", "Div", "DivOrder", "EventGroup", "RunOrder", "TrickBoat", "TeamCode", "EventClass", "ReadyForPlcmt"
                    , "RankingScore", "RankingRating", "HCapBase", "HCapScore" };
            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnListArray = curList;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnListArray = curList;

            skierDoneReasonDialogForm = new SkierDoneReason();

            //Load skier classes to drop down list
            scoreEventClass.DataSource = TrickEventData.mySkierClassList.DropdownList;
            scoreEventClass.DisplayMember = "ItemName";
            scoreEventClass.ValueMember = "ItemValue";

            myCheckOfficials = new CheckOfficials();

            EventRunInfoLabel.Text = "   Event Start:\n" + "   Event Delay:\n" + "Skiers, Passes:";
            EventRunInfoData.Text = "";
            EventRunPerfLabel.Text = "Event Duration:\n" + "Mins Per Skier:\n" + " Mins Per Pass:";
            EventRunPerfData.Text = "";

            //Retrieve trick list
            getTrickList( TrickEventData.myTourRules );
            if ( myTrickListDataTable.Rows.Count == 0 ) getTrickList( "awsa" );

            myTrickValidation.TourRules = TrickEventData.myTourRules;
            myTrickValidation.SkierClassDataTable = TrickEventData.mySkierClassList.SkierClassDataTable;
            myTrickValidation.TrickListDataTable = myTrickListDataTable;

            //Load round selection list based on number of rounds specified for the tournament
            roundActiveSelect.SelectList_LoadHorztl( TrickEventData.myTourRow["TrickRounds"].ToString(), roundActiveSelect_Click );
            roundActiveSelect.RoundValue = "1";

            //Retrieve list of approved tournament boats
            getApprovedTowboats();
            listApprovedBoatsDataGridView.Visible = false;
            approvedBoatSelectGroupBox.Visible = false;

            //Retrieve and load tournament event entries
            loadEventGroupList( Convert.ToByte( roundActiveSelect.RoundValue ) );

            if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                LiveWebLabel.Visible = true;
            } else {
                LiveWebLabel.Visible = false;
            }
            Cursor.Current = Cursors.Default;
        }
        private void CloseWindowTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( CloseWindowTimer );
            this.Close();
        }

        private void ScoreCalc_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TrickCalc_Width = this.Size.Width;
                Properties.Settings.Default.TrickCalc_Height = this.Size.Height;
                Properties.Settings.Default.TrickCalc_Location = this.Location;
            }
            if ( myPassRunCount > 0 && mySkierRunCount > 0 ) {
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

        private void ScoreCalc_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                CalcScoreButton_Click( null, null );
            }

            e.Cancel = false;
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "DataGridView_DataError occurred. \n Context: " + e.Context.ToString()
                + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
            ViewTrickDataButton_Click( null, null );
        }

        private void ResizeNarrow_Click( object sender, EventArgs e ) {
            ResizeNarrow.Visible = false;
            ResizeNarrow.Enabled = false;
            ResizeWide.Visible = true;
            ResizeWide.Enabled = true;

            TourEventRegDataGridView.Width = 300;
        }

        private void ResizeWide_Click( object sender, EventArgs e ) {
            ResizeNarrow.Visible = true;
            ResizeNarrow.Enabled = true;
            ResizeWide.Visible = false;
            ResizeWide.Enabled = false;

            if ( TeamCode.Visible ) {
                TourEventRegDataGridView.Width = 650;
            } else {
                TourEventRegDataGridView.Width = 600;
            }
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            bool curReturnStatus = true, curMethodReturn;
            if ( isLoadInProg ) return;
            if ( !isDataModified ) return;

            curReturnStatus = saveTrickPass( "Pass1" );
            curMethodReturn = saveTrickPass( "Pass2" );
            if ( curReturnStatus ) curReturnStatus = curMethodReturn;
            if ( !curReturnStatus ) return; // Exit if save processing has failed

            if ( checkPassError() ) {
                setEventRegRowStatus( "3-Error" );

            } else if ( Pass1DataGridView.Rows.Count > 0 ) {
                // Process entries in first pass
                if ( HelperFunctions.isObjectEmpty( Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value ) ) {
                    if ( Pass2DataGridView.Rows.Count > 0 ) {
                        if ( HelperFunctions.isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value ) ) {
                            setEventRegRowStatus( "1-TBD" );
                        } else {
                            setEventRegRowStatus( "2-InProg" );
                        }
                    } else {
                        setEventRegRowStatus( "2-InProg" );
                    }

                } else {
                    if ( TrickEventData.isCollegiateEvent() ) {
                        setEventRegRowStatus( "4-Done" );
                    } else if ( Pass2DataGridView.Rows.Count > 0 ) {
                        if ( HelperFunctions.isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value ) ) {
                            setEventRegRowStatus( "2-InProg" );
                        } else {
                            setEventRegRowStatus( "4-Done" );
                        }
                    } else {
                        setEventRegRowStatus( "2-InProg" );
                    }
                }

            } else {
                // Empty first pass 
                if ( TrickEventData.isCollegiateEvent() ) {
                    setEventRegRowStatus( "1-TBD" );
                } else if ( Pass2DataGridView.Rows.Count > 0 ) {
                    if ( HelperFunctions.isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Code"].Value ) ) {
                        setEventRegRowStatus( "1-TBD" );
                    } else {
                        setEventRegRowStatus( "2-InProg" );
                    }
                } else {
                    setEventRegRowStatus( "1-TBD" );
                }
            }

            try {
                if ( saveTrickScore() ) {
                    isDataModified = false;

                } else {
                    return; // Exit if save processing has failed
                }

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes for trick score \n" + excp.Message );
                return;
            }

            if ( LiveWebLabel.Visible ) {
                String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
                LiveWebHandler.sendCurrentSkier( "Trick", TrickEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
            }

            // Set focus and objection positioning when save processing has been successful
            isDataModified = false;
            if ( Pass1DataGridView.Rows.Count > 1 ) {
                if ( Pass2DataGridView.Rows.Count > 1 ) {
                    noteTextBox.Focus();
                } else if ( Pass2DataGridView.Rows.Count == 1 ) {
                    Pass2DataGridView.Focus();
                    Pass2DataGridView.Select();
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                } else {
                    noteTextBox.Focus();
                }

            } else if ( Pass1DataGridView.Rows.Count == 0 ) {
                if ( Pass2DataGridView.Rows.Count == 1 ) {
                    Pass2DataGridView.Focus();
                    Pass2DataGridView.Select();
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                } else {
                    noteTextBox.Focus();
                }

            } else if ( HelperFunctions.isObjectEmpty( Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value ) ) {
                if ( Pass2DataGridView.Rows.Count == 1 ) {
                    Pass2DataGridView.Focus();
                    Pass2DataGridView.Select();
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                } else {
                    noteTextBox.Focus();
                }

            } else {
                if ( Pass2DataGridView.Rows.Count == 0 ) {
                    noteTextBox.Focus();
                } else if ( Pass2DataGridView.Rows.Count == 1 ) {
                    Pass2DataGridView.Focus();
                    Pass2DataGridView.Select();
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[0].Cells["Pass2Skis"];
                } else {
                    noteTextBox.Focus();
                }
            }
        }

        private bool saveTrickScore() {
            String curMethodName = "Trick:ScoreCalc:saveTrickScore";
            String curMsg = "";
            bool curReturnStatus = true;

            try {
                updateScoreRow();

                String curSanctionId = HelperFunctions.getDataRowColValue( myScoreRow, "SanctionId", "" );
                String curMemberId = HelperFunctions.getDataRowColValue( myScoreRow, "MemberId", "" );
                if ( HelperFunctions.isObjectEmpty( curSanctionId ) || HelperFunctions.isObjectEmpty( curMemberId ) ) return false;

                String curAgeGroup = HelperFunctions.getDataRowColValue( myScoreRow, "AgeGroup", "" );
                String curBoat = calcBoatCodeFromDisplay( TourBoatTextbox.Text );
                Byte curRound = (Byte)HelperFunctions.getDataRowColValueDecimal( myScoreRow, "Round", 0 );
                String curStatus = HelperFunctions.getDataRowColValue( myScoreRow, "Status", "TBD" );
                String curEventClass = (String)scoreEventClass.SelectedValue;

                Int64 curPK = 0;
                try {
                    curPK = (Int64)myScoreRow["PK"];
                } catch {
                    curPK = 0;
                }
                String curNote = "";
                if ( HelperFunctions.isObjectPopulated( noteTextBox.Text ) ) {
                    curNote = noteTextBox.Text;
                    curNote = curNote.Replace( "'", "''" );
                }


                StringBuilder curSqlStmt = new StringBuilder( "" );

                if ( curStatus.Equals( "TBD" ) ) {
                    curSqlStmt.Append( "Delete TrickScore Where PK = " + curPK.ToString() );
                    TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Score"].Value = "";
                    TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = "";

                } else if ( curPK > 0 ) {
                    curSqlStmt.Append( "Update TrickScore Set " );
                    curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                    curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                    curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                    curSqlStmt.Append( ", Round = " + curRound.ToString() );
                    curSqlStmt.Append( ", EventClass = '" + curEventClass + "'" );
                    curSqlStmt.Append( ", Score = " + ( (Int16)myScoreRow["Score"] ).ToString() );
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
                    curSqlStmt.Append( "SanctionId, MemberId, AgeGroup, Round, EventClass, " );
                    curSqlStmt.Append( "Score, ScorePass1, ScorePass2, NopsScore, " );
                    curSqlStmt.Append( "Boat, Status, LastUpdateDate, InsertDate, Note" );
                    curSqlStmt.Append( ") Values (" );
                    curSqlStmt.Append( " '" + curSanctionId + "'" );
                    curSqlStmt.Append( ", '" + curMemberId + "'" );
                    curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                    curSqlStmt.Append( ", " + curRound.ToString() );
                    curSqlStmt.Append( ", '" + curEventClass + "'" );
                    curSqlStmt.Append( ", " + ( (Int16)myScoreRow["Score"] ).ToString() );
                    curSqlStmt.Append( ", " + ( (Int16)myScoreRow["ScorePass1"] ).ToString() );
                    curSqlStmt.Append( ", " + ( (Int16)myScoreRow["ScorePass2"] ).ToString() );
                    curSqlStmt.Append( ", " + ( (Decimal)myScoreRow["NopsScore"] ).ToString( "#####.00" ) );
                    curSqlStmt.Append( ", '" + curBoat + "'" );
                    curSqlStmt.Append( ", '" + (String)myScoreRow["Status"] + "'" );
                    curSqlStmt.Append( ", GETDATE(), GETDATE()" );
                    curSqlStmt.Append( ", '" + curNote.Trim() + "'" );
                    curSqlStmt.Append( " )" );
                }

                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                curReturnStatus = true;
                if ( curPK < 0 ) setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, curRound );

                #region Check to see if score is equal to or great than divisions current record score
                if ( curSanctionId.Length > 1 && curMemberId.Length > 1 ) {
                    String curCheckRecordMsg = TrickEventData.myCheckEventRecord.checkRecordTrick( curAgeGroup, scoreTextBox.Text, (byte)myScoreRow["SkiYearAge"], (string)myScoreRow["Gender"] );

                    if ( curCheckRecordMsg == null ) curCheckRecordMsg = "";
                    if ( curCheckRecordMsg.Length > 1 ) {
                        MessageBox.Show( curCheckRecordMsg );
                    }
                }
                #endregion

            } catch ( Exception excp ) {
                curReturnStatus = false;
                curMsg = ":Error attempting to save trick score \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }

            showEventRunInfo();

            return curReturnStatus;
        }

        private void updateScoreRow() {
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
            myScoreRow["Note"] = noteTextBox.Text;

            try {
                myScoreRow["Status"] = "TBD";
                String curValue = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value;
                if ( curValue.Equals( "4-Done" ) ) myScoreRow["Status"] = "Complete";
                if ( curValue.Equals( "2-InProg" ) ) myScoreRow["Status"] = "InProg";
                if ( curValue.Equals( "3-Error" ) ) myScoreRow["Status"] = "Error";
                if ( curValue.Equals( "1-TBD" ) ) myScoreRow["Status"] = "TBD";
            } catch {
                myScoreRow["Status"] = "TBD";
            }
        }

        private void showEventRunInfo() {
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

        private bool saveTrickPass( String inPassPrefix ) {
            String curMethodName = "Trick:ScoreCalc:saveTrickPass";
            String curMsg = "";
            DataGridView curDataGridView = null;

            if ( inPassPrefix.Equals( "Pass1" ) ) {
                curDataGridView = Pass1DataGridView;
            } else {
                curDataGridView = Pass2DataGridView;
            }

            if ( curDataGridView == null ) return false;

            bool curReturnStatus = true;
            try {
                foreach ( DataGridViewRow curViewRow in curDataGridView.Rows ) {
                    bool curMethodReturn = saveTrickPassEntry( inPassPrefix, curViewRow );
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
            return curReturnStatus;
        }

        private void saveTrickPassUpdatePK( String inPassPrefix, DataGridViewRow inViewRow ) {
            try {
                int curPK = Convert.ToInt32( inViewRow.Cells[inPassPrefix + "PK"].Value );
                if ( curPK >= 0 ) return;
                String curMemberId = (String)inViewRow.Cells[inPassPrefix + "MemberId"].Value;
                String curAgeGroup = (String)inViewRow.Cells[inPassPrefix + "AgeGroup"].Value;
                String curCode = HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Code", "" );
                myTrickValidation.validateNumSkis( HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Skis", "" ), TrickEventData.myTourRules );
                String curSkis = this.myTrickValidation.NumSkis.ToString();

                String curResults = (String)inViewRow.Cells[inPassPrefix + "Results"].Value;
                if ( curResults.ToLower().Equals( "end" ) ) return;
                if ( curCode.Length > 0 && curSkis.Length > 0 && curResults.Length > 0 ) {
                    byte curRound = Convert.ToByte( HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Round", "" ) );
                    byte curPassNum = Convert.ToByte( HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "PassNum", "" ) );
                    byte curSeq = Convert.ToByte( HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Seq", "" ) );

                    DataTable curTrickPassDataTable = getSkierPassByRound( curMemberId, curAgeGroup, curRound, curPassNum, curSeq );
                    if ( curTrickPassDataTable.Rows.Count == 1 ) {
                        inViewRow.Cells[inPassPrefix + "PK"].Value = curTrickPassDataTable.Rows[0]["PK"].ToString();

                    } else {
                        MessageBox.Show( "No row retrieved after adding new trick entry. Insert of trick may have failed"
                            + "\n SanctionNum=" + TrickEventData.mySanctionNum
                            + "\n MemberId=" + curMemberId
                            + "\n Round=" + curRound
                            + "\n PassNum=" + curPassNum
                            + "\n Seq=" + curSeq
                            );
                    }
                }

            } catch {
            }
        }

        private bool saveTrickPassEntry( String inPassPrefix, DataGridViewRow inViewRow ) {
            String curMethodName = "Trick:ScoreCalc:saveTrickPassEntry";
            String curMsg = "";

            int curPK = 0;
            try {
                curPK = int.Parse( (String)inViewRow.Cells[inPassPrefix + "PK"].Value );

            } catch ( FormatException ) {
                curMsg = "Error PK value not set therefore must not be a valid row";
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
                return false;
            }

            String curUpdateStatus = (String)inViewRow.Cells[inPassPrefix + "Updated"].Value;
            try {
                String curSanctionId = (String)inViewRow.Cells[inPassPrefix + "SanctionId"].Value;
                String curMemberId = (String)inViewRow.Cells[inPassPrefix + "MemberId"].Value;
                String curAgeGroup = (String)inViewRow.Cells[inPassPrefix + "AgeGroup"].Value;
                String curNote = HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Note", "" );
                String curSkis = "";
                if ( myTrickValidation.validateNumSkis( HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Skis", "" ), TrickEventData.myTourRules ) >= 0 ) {
                    curSkis = this.myTrickValidation.NumSkis.ToString();
                } else {
                    curSkis = "0";
                }

                if ( curSanctionId.Length == 0 || curMemberId.Length == 0
                        || inViewRow.Cells[inPassPrefix + "Results"].Value.ToString().ToUpper().Equals( "END" )
                        || curUpdateStatus.ToUpper().Equals( "N" )
                ) return true;

                StringBuilder curSqlStmt = new StringBuilder( "" );
                if ( curPK > 0 ) {
                    curSqlStmt.Append( "Update TrickPass Set " );
                    curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                    curSqlStmt.Append( ", MemberId = '" + curMemberId + "'" );
                    curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                    curSqlStmt.Append( ", Round = " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Round", "0" ) );
                    curSqlStmt.Append( ", PassNum = " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "PassNum", "0" ) );
                    curSqlStmt.Append( ", Skis = " + curSkis );
                    curSqlStmt.Append( ", Seq = " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Seq", "0" ) );
                    curSqlStmt.Append( ", Score = " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Points", "0" ) );
                    curSqlStmt.Append( ", Code = '" + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Code", "0" ) + "'" );
                    curSqlStmt.Append( ", Results = '" + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Results", "0" ) + "'" );
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
                    curSqlStmt.Append( ", " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Round", "0" ) );
                    curSqlStmt.Append( ", " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "PassNum", "0" ) );
                    curSqlStmt.Append( ", " + curSkis );
                    curSqlStmt.Append( ", " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Seq", "0" ) );
                    curSqlStmt.Append( ", " + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Points", "0" ) );
                    curSqlStmt.Append( ", '" + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Code", "0" ) + "'" );
                    curSqlStmt.Append( ", '" + HelperFunctions.getViewRowColValue( inViewRow, inPassPrefix + "Results", "0" ) + "'" );
                    curSqlStmt.Append( ", GETDATE()" );
                    curSqlStmt.Append( ", '" + curNote + "' )" );
                }
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                if ( rowsProc > 0 ) return true;

                return false;

            } catch ( Exception excp ) {
                curMsg = ":Error attempting to save changes for trick pass " + inPassPrefix + "\n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
                return false;
            }
        }

        private void calcUpdateScoreTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( calcUpdateScoreTimer );
            if ( isPassEnded ) {
                calcScore();
            } else {
                CalcScoreButton_Click( null, null );
            }
            isDataModifiedInProgress = false;
        }

        /*
         * Reset all repeat tricks to credit so a recalculation can be performed to account for status edits.
         */
        private void CalcScoreButton_Click( object sender, EventArgs e ) {
            String curColPrefix;

            if ( TourEventRegDataGridView.CurrentRow != null ) {
                isDataModifiedInProgress = true;
                isLoadInProg = true;
                if ( Pass1DataGridView.Rows.Count > 0 ) {
                    curColPrefix = "Pass1";
                    foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                        if ( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).Equals( "Repeat" ) ) {
                            curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                        }
                    }
                }

                if ( Pass2DataGridView.Rows.Count > 0 ) {
                    curColPrefix = "Pass2";
                    foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                        if ( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).Equals( "Repeat" ) ) {
                            curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                        }
                    }
                }

                calcScore();

                isLoadInProg = false;
                isDataModifiedInProgress = false;
            }
        }

        private void calcScore() {
            bool curReadyToScore = true;

            if ( TourEventRegDataGridView.CurrentRow == null ) return;

            Cursor.Current = Cursors.WaitCursor;
            isLoadInProg = true;

            //Calculate first pass score
            if ( Pass1DataGridView.Rows.Count > 0 ) curReadyToScore = calcScoreForPass( Pass1DataGridView, "Pass1" );

            if ( Pass2DataGridView.Visible && Pass2DataGridView.Rows.Count > 0 ) curReadyToScore = calcScoreForPass( Pass2DataGridView, "Pass2" );

            isLoadInProg = false;

            isDataModified = true;
            int curTotalScore = 0, curPass1Score = 0, curPass2Score = 0;

            foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                curPass1Score += Convert.ToInt16( curPassRow.Cells["Pass1Points"].Value );
                curPassRow.Cells["Pass1PointsTotal"].Value = curPass1Score.ToString();
            }
            foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                curPass2Score += Convert.ToInt16( curPassRow.Cells["Pass2Points"].Value );
                curPassRow.Cells["Pass2PointsTotal"].Value = curPass2Score.ToString();
            }

            curTotalScore = curPass1Score + curPass2Score;
            scoreTextBox.Text = curTotalScore.ToString();

            hcapScoreTextBox.Text = ( curTotalScore + HelperFunctions.getViewRowColValueDecimal( TourEventRegDataGridView.Rows[myEventRegViewIdx], "HCapScore", "0" ) ).ToString( "##,###0.0" );
            TourEventRegDataGridView.CurrentRow.Cells["Score"].Value = scoreTextBox.Text;
            TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["ScoreWithHcap"].Value = hcapScoreTextBox.Text;
            scorePass1.Text = curPass1Score.ToString();
            scorePass2.Text = curPass2Score.ToString();

            ScoreList[0].Score = Convert.ToDecimal( curTotalScore.ToString() );
            String curAgeGroup = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.CurrentRow, "AgeGroup", "" );
            appNopsCalc.calcNops( curAgeGroup, ScoreList );
            nopsScoreTextBox.Text = Math.Round( ScoreList[0].Nops, 1 ).ToString();

            navSaveItem_Click( null, null );
            refreshScoreSummaryWindow();

            /*
            if ( curReadyToScore ) {
            } else {
				MessageBox.Show( "Invalid data has been detected and can not be saved at this time" );
			}
             */

            Cursor.Current = Cursors.Default;
        }

        private bool calcScoreForPass( DataGridView passDataGridView, String curColPrefix ) {
            bool returnReadyToScore = true;
            bool curPassTerm = false;
            String curTrickCode;
            Int16 curNumSkis = 0;

            foreach ( DataGridViewRow curPassRow in passDataGridView.Rows ) {
                if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ) )
                    || HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ) )
                    ) continue;

                curNumSkis = this.myTrickValidation.validateNumSkis( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ), TrickEventData.myTourRules );
                curTrickCode = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" );
                if ( curPassTerm ) {
                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                    curPassRow.Cells[curColPrefix + "Results"].Value = "OOC";
                    curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                    curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                    continue;
                }

                if ( curTrickCode.Equals( "END" ) || curTrickCode.Equals( "HORN" ) ) {
                    if ( deleteTrickPass( curPassRow.Cells[curColPrefix + "PK"].Value.ToString() ) ) {
                        passDataGridView.Rows.Remove( curPassRow );
                    }
                    continue;
                }

                String curResults = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();
                if ( checkTrickCode( passDataGridView, curPassRow.Index, curTrickCode, curNumSkis, curColPrefix ) ) {
                    isTrickValid = true;
                    curResults = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();
                    if ( curResults.Equals( "CREDIT" ) ) {
                        curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( passDataGridView, curPassRow, curColPrefix ).ToString();
                        curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = SystemColors.ControlText;
                        curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = SystemColors.Window;
                    } else {
                        curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                        curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                        curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                    }
                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";

                    // Current trick has points assigned
                    if ( curResults.Equals( "OOC" ) ) {
                        if ( curPassRow.Index > 0 && curPassRow.Index < ( passDataGridView.Rows.Count - 1 ) ) curPassTerm = true;

                    } else if ( curResults.Equals( "FALL" ) ) {
                        if ( curPassRow.Index > 0 && curPassRow.Index < ( passDataGridView.Rows.Count - 1 ) ) curPassTerm = true;
                    
                    } else if ( curResults.Equals( "Unresolved" ) ) {
                        returnReadyToScore = false;
                    }
                }
            }

            return returnReadyToScore;
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
                //MessageBox.Show( "insertTrickTimer LoadInProg" );
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
            DataGridViewRow curPassRow = null;
            DataGridViewRow newPassRow;
            Int16 curNumSkis;
            int newPosIdx = 0;
            //int curColSeq = -1;
            int curLastIdx = 0;
            String curRound = "", newRowSeqNum = "", curPassNum = "";
            String curMsg = "", curNumSkisDisplay = "";

            curMsg = "curMemberId";
            String curColPrefix = inPassView.Name.Substring( 0, 5 );
            String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
            String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
            if ( inPassView.Rows.Count > 0 ) {
                // Determine position of new row to be added
                if ( inInsertAbove ) {
                    // Add row above current row
                    newPosIdx = inPassView.CurrentRow.Index;
                    curLastIdx = inPassView.Rows.Add();

                    curPassRow = inPassView.CurrentRow;
                    newRowSeqNum = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Seq", "1" );
                    curRound = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Round", "1" );
                    curPassNum = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PassNum", "1" );
                    curNumSkisDisplay = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "1" );
                    curNumSkis = this.myTrickValidation.validateNumSkis( curNumSkisDisplay, TrickEventData.myTourRules );
                    if ( curNumSkis < 0 ) curNumSkis = 1;
                    curMsg = String.Format( "Insert Above: Round={0}, Pass={1}, Seq={2}, NumSkis={3}", curRound, curPassNum, newRowSeqNum, curNumSkisDisplay );

                    // Move rows starting at current row position down one row
                    for ( int curIdx = curLastIdx; curIdx > newPosIdx; curIdx-- ) {
                        for ( int colIdx = 0; colIdx < inPassView.ColumnCount; colIdx++ ) {
                            inPassView.Rows[curIdx].Cells[colIdx].Value = inPassView.Rows[curIdx - 1].Cells[colIdx].Value;
                        }
                        inPassView.Rows[curIdx].Cells[curColPrefix + "Seq"].Value = ( curIdx + 1 ).ToString();
                        inPassView.Rows[curIdx].Cells[curColPrefix + "Updated"].Value = "Y";
                    }

                } else {
                    curMsg = "Insert row at bottom of view";
                    curLastIdx = inPassView.Rows.Count - 1;
                    String curResult = HelperFunctions.getViewRowColValue( inPassView.Rows[curLastIdx], curColPrefix + "Results", "" );
                    if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inPassView.Rows[curLastIdx], curColPrefix + "Code", "" ) )
                        || ( curResult.Equals( "Fall" ) || ( curResult.Equals( "OOC" ) && inPassView.Rows.Count > 1 ) )
                        ) {
                        isLoadInProg = false;
                        return -1;
                    }

                    newPosIdx = inPassView.Rows.Add();
                    curPassRow = inPassView.Rows[curLastIdx];

                    curRound = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Round", "1" );
                    newRowSeqNum = ( 1 + Convert.ToInt16( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Seq", "0" ) ) ).ToString();
                    curNumSkisDisplay = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "1" );
                    curNumSkis = this.myTrickValidation.validateNumSkis( curNumSkisDisplay, TrickEventData.myTourRules );
                    curPassNum = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PassNum", "1" );
                }

            } else {
                curMsg = "Insert first row";
                newPosIdx = inPassView.Rows.Add();
                curRound = roundActiveSelect.RoundValue;
                newRowSeqNum = "1";
                curNumSkis = 1;
                curNumSkisDisplay = curNumSkis.ToString();
                inPassView.CurrentCell = inPassView.Rows[newPosIdx].Cells[curColPrefix + "Skis"];

                if ( curColPrefix.Equals( "Pass1" ) ) {
                    curPassNum = "1";
                } else {
                    curPassNum = "2";
                    if ( Pass1DataGridView.Rows.Count > 0 ) {
                        curNumSkisDisplay = HelperFunctions.getViewRowColValue( Pass1DataGridView.Rows[0], "Pass1Skis", "1" );
                        curNumSkis = this.myTrickValidation.validateNumSkis( curNumSkisDisplay, TrickEventData.myTourRules );
                    }
                }
            }

            newPassRow = inPassView.Rows[newPosIdx];
            newPassRow.Cells[curColPrefix + "PK"].Value = "-1";
            newPassRow.Cells[curColPrefix + "SanctionId"].Value = TrickEventData.mySanctionNum;
            newPassRow.Cells[curColPrefix + "MemberId"].Value = curMemberId;
            newPassRow.Cells[curColPrefix + "AgeGroup"].Value = curAgeGroup;
            newPassRow.Cells[curColPrefix + "Code"].Value = "";
            newPassRow.Cells[curColPrefix + "Round"].Value = curRound;
            newPassRow.Cells[curColPrefix + "Skis"].Value = curNumSkisDisplay;
            newPassRow.Cells[curColPrefix + "Seq"].Value = newRowSeqNum;
            newPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
            newPassRow.Cells[curColPrefix + "PassNum"].Value = curPassNum;
            newPassRow.Cells[curColPrefix + "Points"].Value = "0";
            newPassRow.Cells[curColPrefix + "Updated"].Value = "N";

            if ( myPassRunCount == 0 ) {
                myEventStartTime = DateTime.Now;
                EventRunInfoData.Text = "";
                EventRunPerfData.Text = "";
            }
            myPassRunCount++;
            if ( curPassNum.Equals( "1" ) && newPosIdx == 0 ) {
                mySkierRunCount++;
            } else {
                if ( curPassNum.Equals( "1" ) && mySkierRunCount == 0 ) {
                    mySkierRunCount++;
                }
            }

            isLoadInProg = false;
            return newPosIdx;
        }

        private void deleteTrick( DataGridView inPassView, int inViewRowIdx, String inPassPrefix ) {
            String curValue = "", curPkValue = "";
            if ( HelperFunctions.isObjectPopulated( inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "PK"].Value ) ) {
                curPkValue = inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "PK"].Value.ToString();
            }
            if ( HelperFunctions.isObjectPopulated( inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "Results"].Value ) ) {
                curValue = inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "Results"].Value.ToString();
            }
            if ( curValue.Equals( "Fall" ) ) isPassEnded = false;
            if ( deleteTrickPass( curPkValue ) ) {
                inPassView.Rows.Remove( inPassView.Rows[inViewRowIdx] );
                inViewRowIdx--;
            }

            if ( inPassView.Rows.Count > 0 ) {
                if ( inViewRowIdx < 0 ) inViewRowIdx = 0;
                inPassView.CurrentCell = inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "Skis"];
                inPassView.CurrentCell.Selected = true;
                for ( int idx = inViewRowIdx; idx < inPassView.Rows.Count; idx++ ) {
                    inPassView.Rows[idx].Cells[inPassPrefix + "Seq"].Value = ( idx + 1 ).ToString();
                    inPassView.Rows[idx].Cells[inPassPrefix + "Updated"].Value = "Y";
                }

                inPassView.Focus();
                isDataModified = true;

            } else {
                inViewRowIdx = insertTrick( inPassView, false );
                inPassView.CurrentCell = inPassView.Rows[inViewRowIdx].Cells[inPassPrefix + "Skis"];
                inPassView.CurrentCell.Selected = true;

                inPassView.Focus();
                isDataModified = true;

                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 15;
                curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                curTimerObj.Start();
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
                        curMsg = ":Error attempting to delete trick pass \n" + excp.Message;
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

        private void navFilter_Click( object sender, EventArgs e ) {
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

        private void navExportRecord_Click( object sender, EventArgs e ) {
            ExportRecordAppData myExportData = new ExportRecordAppData();
            DataGridViewRow curViewRow = TourEventRegDataGridView.Rows[myEventRegViewIdx];
            String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
            String curEventGroup = (String)curViewRow.Cells["EventGroup"].Value;
            byte curRound = Convert.ToByte( (String)Pass1DataGridView.Rows[0].Cells["Pass1Round"].Value );
            myExportData.ExportData( TrickEventData.mySanctionNum, "Trick", curMemberId, curAgeGroup, curEventGroup, curRound );
        }

        private void navExportLw_Click( object sender, EventArgs e ) {
            navExport_Click( null, null );
            ExportLiveWeb.uploadExportFile( "Export", "Trick", TrickEventData.mySanctionNum );
        }

        private void navExport_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    return;
                }
            }
            if ( isDataModified ) {
                MessageBox.Show( "Data has been modified.  Request cannot be completed." );
                return;
            }
            ExportData myExportData = new ExportData();
            String[] curTableName = { "TourReg", "EventReg", "EventRunOrder", "TrickScore", "TrickPass", "TourReg", "OfficialWork", "OfficialWorkAsgmt" };

            String[] curSelectCommand = TrickEventData.buildScoreExport( roundActiveSelect.RoundValue, EventGroupList.SelectedItem.ToString(), myFilterCmd );

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void ViewTrickListButton_Click( object sender, EventArgs e ) {
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
                if ( TrickEventData.isCollegiateEvent() ) {
                    myFilterCmd = HelperFunctions.getEventGroupFilterNcwsa( curGroupValue );

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

            roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
            getEventRegData( curGroupValue, Convert.ToByte( roundActiveSelect.RoundValue ) );
            loadTourEventRegView( myEventRegDataTable, mySortCommand, myFilterCmd );
            if ( myEventRegDataTable.Rows.Count > 0 ) {
                setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundActiveSelect.RoundValue ) );
                setTrickPassEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundActiveSelect.RoundValue ) );
            }
        }

        private void navLiveWeb_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            LiveWebHandler.LiveWebDialog.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( LiveWebHandler.LiveWebDialog.DialogResult != DialogResult.OK ) return;

            if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Connect" ) ) {
                if ( LiveWebHandler.connectLiveWebHandler( TrickEventData.mySanctionNum ) ) {
                    LiveWebLabel.Visible = true;

                } else {
                    ExportLiveTwitter.TwitterLocation = "";
                    LiveWebLabel.Visible = false;
                }

            } else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Disable" ) ) {
                LiveWebHandler.disconnectLiveWebHandler();
                LiveWebLabel.Visible = false;

            } else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "Resend" ) ) {
                if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                    String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                    String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                    String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                    String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                    byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
                    LiveWebHandler.sendCurrentSkier( "Trick", TrickEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0 );
                }

            } else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "ResendAll" ) ) {
                if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                    String curEventGroup = EventGroupList.SelectedItem.ToString();
                    byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
                    LiveWebHandler.sendSkiers( "Trick", TrickEventData.mySanctionNum, curRound, curEventGroup );
                    // ExportLiveWeb.exportCurrentSkiers( "Trick", TrickEventData.mySanctionNum, curRound, curEventGroup
                }

            } else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableSkier" ) ) {
                if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                    String curEventGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventGroup"].Value;
                    String curMemberId = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value;
                    String curAgeGroup = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value;
                    String curTeamCode = (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["TeamCode"].Value;
                    byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
                    LiveWebHandler.sendDisableCurrentSkier( "Trick", TrickEventData.mySanctionNum, curMemberId, curAgeGroup, curRound );
                    //ExportLiveWeb.exportCurrentSkierTrick( TrickEventData.mySanctionNum, curMemberId, curAgeGroup, curRound, 0, curEventGroup
                }

            } else if ( LiveWebHandler.LiveWebDialog.ActionCmd.Equals( "DiableAllSkier" ) ) {
                if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                    String curEventGroup = EventGroupList.SelectedItem.ToString();
                    byte curRound = Convert.ToByte( roundActiveSelect.RoundValue );
                    LiveWebHandler.sendDisableSkiers( "Trick", TrickEventData.mySanctionNum, curRound, curEventGroup );
                    //ExportLiveWeb.exportCurrentSkiers( "Trick", TrickEventData.mySanctionNum, curRound, curEventGroup )
                }
            }
        }

        private void loadEventGroupList( int inRound ) {
            isLoadInProg = true;

            if ( EventGroupList.DataSource == null ) {
                if ( TrickEventData.isCollegiateEvent() ) {
                    EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
                } else {
                    loadEventGroupListFromData( inRound );
                }
            } else {
                if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
                    if ( TrickEventData.isCollegiateEvent() ) {
                    } else {
                        loadEventGroupListFromData( inRound );
                    }
                } else {
                    if ( TrickEventData.isCollegiateEvent() ) {
                        EventGroupList.DataSource = HelperFunctions.buildEventGroupListNcwsa();
                    } else {
                        loadEventGroupListFromData( inRound );
                    }
                }
            }
            isLoadInProg = false;
        }

        private void loadEventGroupListFromData( int inRound ) {
            String curGroupValue = "";
            if ( EventGroupList.DataSource != null ) {
                if ( ( (ArrayList)EventGroupList.DataSource ).Count > 0 ) {
                    try {
                        curGroupValue = EventGroupList.SelectedItem.ToString();
                    } catch {
                        curGroupValue = "";
                    }
                }
            }

            EventGroupList.DataSource = HelperFunctions.buildEventGroupList( TrickEventData.mySanctionNum, "Trick", inRound );
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

        private void loadTourEventRegView( DataTable inEventRegDataTable, String inSortCmd, String inFilterCmd ) {
            DataGridViewRow curViewRow;

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
                    return;
                }

                if ( inEventRegDataTable == null || inEventRegDataTable.Rows.Count == 0 ) {
                    Pass1DataGridView.Rows.Clear();
                    Pass2DataGridView.Rows.Clear();
                    TourEventRegDataGridView.Rows.Clear();
                    MessageBox.Show( "No event registration entries found" );
                    return;
                }

                winStatusMsg.Text = "Retrieving tournament entries";
                Cursor.Current = Cursors.WaitCursor;

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

                    String curEventGroup = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                    String curRunOrderGroup = HelperFunctions.getDataRowColValue( curDataRow, "RunOrderGroup", "" );
                    if ( HelperFunctions.isObjectPopulated( curRunOrderGroup ) ) {
                        curViewRow.Cells["EventGroup"].Value = curEventGroup + "-" + curRunOrderGroup;
                    } else {
                        curViewRow.Cells["EventGroup"].Value = curEventGroup;
                    }

                    curViewRow.Cells["RunOrder"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RunOrder", "0" );
                    curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                    curViewRow.Cells["EventClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventClass", "" );

                    curViewRow.Cells["Score"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "Score", "0", 0 );
                    curViewRow.Cells["Score"].ToolTipText = HelperFunctions.getDataRowColValue( curDataRow, "LastUpdateDate", "" );
                    decimal tempScoreValue1 = decimal.Parse( HelperFunctions.getDataRowColValue( curDataRow, "Score", "0" ) );
                    decimal tempScoreValue2 = decimal.Parse( HelperFunctions.getDataRowColValue( curDataRow, "HCapScore", "0" ) );
                    curViewRow.Cells["ScoreWithHcap"].Value = ( tempScoreValue1 + tempScoreValue2 ).ToString( "##,###0" );
                    curViewRow.Cells["ScoreWithHcap"].ToolTipText = HelperFunctions.getDataRowColValue( curDataRow, "LastUpdateDate", "" );
                    curViewRow.Cells["RankingScore"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "RankingScore", "0", 0 );
                    curViewRow.Cells["RankingRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingRating", "" );
                    curViewRow.Cells["HCapBase"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HCapBase", "0", 0 );
                    curViewRow.Cells["HCapScore"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HCapScore", "0", 0 );
                    curViewRow.Cells["TrickBoat"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickBoat", "" );
                    setEventRegRowStatus( HelperFunctions.getDataRowColValue( curDataRow, "Status", "TBD" ) );
                }

                if ( inEventRegDataTable.Rows.Count > 0 ) {
                    myEventRegViewIdx = 0;
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];

                    roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
                    setTrickScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    setTrickPassEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
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

            if ( LiveWebHandler.LiveWebMessageHandlerActive ) {
                LiveWebLabel.Visible = true;
            } else {
                LiveWebLabel.Visible = false;
            }
            String curWarnMsg = "Warn:RunOrder:Round:" + roundActiveSelect.RoundValue;
            if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
                if ( isRunOrderByRound( Convert.ToByte( roundActiveSelect.RoundValue ) ) ) {
                    MessageBox.Show( "WARNING \nThis running order is specific for this round" );
                    myCompletedNotices.Add( curWarnMsg );
                }
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
                roundActiveSelect.RoundValue = (String)myScoreRow["Round"];
            }
        }

        private void roundActiveSelect_Load( object sender, EventArgs e ) {
        }

        private void roundSelect_Click( object sender, EventArgs e ) {
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
                if ( TourEventRegDataGridView.CurrentRow != null ) {
                    if ( !( HelperFunctions.isObjectEmpty( TourEventRegDataGridView.CurrentRow.Cells["MemberId"].Value ) ) ) {
                        setTrickScoreEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundActiveSelect.RoundValue ) );
                        setTrickPassEntry( TourEventRegDataGridView.CurrentRow, Convert.ToByte( roundActiveSelect.RoundValue ) );
                    }
                }
            }
        }

        private void noteTextBox_TextChanged( object sender, EventArgs e ) {
            isDataModified = true;
        }

        private void roundActiveSelect_Click( object sender, EventArgs e ) {
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
                    if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                        myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                    }
                    TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                    roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
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
            if ( curRound >= 25 ) MessageBox.Show( "You currently have the runoff round selected.\nChange the round if that is not desired." );
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
                    if ( curSaveEventRegViewIdx > 0 && TourEventRegDataGridView.Rows.Count > 0 ) {
                        myEventRegViewIdx = curSaveEventRegViewIdx;
                        if ( TourEventRegDataGridView.Rows.Count <= myEventRegViewIdx ) {
                            myEventRegViewIdx = TourEventRegDataGridView.Rows.Count - 1;
                        }
                        if ( myEventRegViewIdx >= 0 ) {
                            TourEventRegDataGridView.CurrentCell = TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"];
                            roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
                            setTrickScoreEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                            setTrickPassEntry( TourEventRegDataGridView.Rows[myEventRegViewIdx], Convert.ToByte( roundActiveSelect.RoundValue ) );
                        }
                    }
                }
            }
        }

        private void scoreEventClass_SelectedIndexChanged( object sender, EventArgs e ) {
            CalcScoreButton.Focus();
        }

        private void scoreEventClass_Validating( object sender, CancelEventArgs e ) {
            String curMethodName = "Trick:ScoreCalc:scoreEventClass_Validating";
            int rowsProc = 0;

            //ListItem curItem = (ListItem)scoreEventClass.SelectedItem;
            //String curEventClass = curItem.ItemValue;
            String curEventClass = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "EventClass", TrickEventData.myTourClass );
            if ( TrickEventData.mySkierClassList.compareClassChange( curEventClass, TrickEventData.myTourClass ) < 0 ) {
                MessageBox.Show( "Class " + curEventClass + " cannot be assigned to a skier in a class " + TrickEventData.myTourClass + " tournament" );
                e.Cancel = true;
                return;
            }

            DataRow curClassRow = TrickEventData.mySkierClassList.SkierClassDataTable.Select( "ListCode = '" + curEventClass.ToUpper() + "'" )[0];
            if ( (Decimal)curClassRow["ListCodeNum"] > (Decimal)TrickEventData.myClassERow["ListCodeNum"] || ( TrickEventData.isIwwfEvent() ) ) {
                bool iwwfMembership = IwwfMembership.validateIwwfMembership(
                    TrickEventData.mySanctionNum, HelperFunctions.getDataRowColValue( TrickEventData.myTourRow, "SanctionEditCode", "0" )
                    , (String)TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value
                    , HelperFunctions.getDataRowColValue( TrickEventData.myTourRow, "EventDates", "0" ) );
                if ( !( iwwfMembership ) ) {
                    curEventClass = "E";
                    scoreEventClass.SelectedValue = curEventClass;
                }
            }

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

        private void DataGridView_Enter( object sender, EventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            curPassView.DefaultCellStyle.BackColor = SystemColors.Window;
            curPassView.DefaultCellStyle.ForeColor = SystemColors.ControlText;
            String curColNameFull = curPassView.Name;
            String curColPrefix = curColNameFull.Substring( 0, 5 );
            String curViewName = curColNameFull.Substring( 5 );
            myActiveTrickPass = curColPrefix;

        }

        private void DataGridView_Leave( object sender, EventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            if ( isDataModified && !( isDataModifiedInProgress ) ) {
                CalcScoreButton_Click( null, null );
            }
            curPassView.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            curPassView.DefaultCellStyle.ForeColor = Color.Silver;
        }

        private void TourEventRegDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
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
                if ( !( HelperFunctions.isObjectEmpty( myDataView.Rows[myEventRegViewIdx].Cells["MemberId"].Value ) ) ) {
                    roundActiveSelect.RoundValue = roundActiveSelect.RoundValue;
                    setTrickScoreEntry( myDataView.Rows[e.RowIndex], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    setTrickPassEntry( myDataView.Rows[e.RowIndex], Convert.ToByte( roundActiveSelect.RoundValue ) );
                    isDataModified = false;

                    if ( myPass1DataTable.Rows.Count == 0 ) {
                        if ( checkForSkierRoundScore( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["MemberId"].Value.ToString()
                            , Convert.ToInt32( roundActiveSelect.RoundValue )
                            , TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["AgeGroup"].Value.ToString() ) ) {
                            Pass1DataGridView.Rows.Clear();
                            Pass2DataGridView.Rows.Clear();
                            MessageBox.Show( "Skier already has a score in this round" );
                            return;
                        }
                    }

                }
            }
        }

        private void setTrickScoreEntry( DataGridViewRow inTourEventRegRow, Byte inRound ) {
            isDataModified = false;
            isLoadInProg = true;
            String curMemberId = (String)inTourEventRegRow.Cells["MemberId"].Value;
            String curAgeGroup = (String)inTourEventRegRow.Cells["AgeGroup"].Value;
            activeSkierName.Text = (String)inTourEventRegRow.Cells["SkierName"].Value;
            this.Text = myTitle + " - " + activeSkierName.Text;

            getSkierScoreByRound( curMemberId, curAgeGroup, inRound );
            if ( myScoreDataTable.Rows.Count > 0 ) {
                myScoreRow = myScoreDataTable.Rows[0];
                roundActiveSelect.RoundValue = ( (Byte)myScoreRow["Round"] ).ToString();

                if ( myScoreRow["Boat"] == System.DBNull.Value ) {
                    TourBoatTextbox.Text = "";
                } else {
                    TourBoatTextbox.Text = setApprovedBoatSelectEntry( (String)myScoreRow["Boat"] );
                    TourBoatTextbox.Select( 0, 0 );
                }

            } else {
                myScoreRow = addNewTrickScoreEntry( inTourEventRegRow, inRound );
            }

            scoreEventClass.SelectedValue = HelperFunctions.getDataRowColValue( myScoreRow, "EventClass", HelperFunctions.getViewRowColValue( inTourEventRegRow, "EventClass", "" ) );
            scoreTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Score", "0" );
            scorePass1.Text = HelperFunctions.getDataRowColValue( myScoreRow, "ScorePass1", "0" );
            scorePass2.Text = HelperFunctions.getDataRowColValue( myScoreRow, "ScorePass2", "0" );
            nopsScoreTextBox.Text = HelperFunctions.getDataRowColValueDecimal( myScoreRow, "NopsScore", "0", 1 );
            noteTextBox.Text = HelperFunctions.getDataRowColValue( myScoreRow, "Note", "" );
            try {
                hcapScoreTextBox.Text = ( (Int16)myScoreRow["Score"] + (Decimal)myScoreRow["HCapScore"] ).ToString( "##,###0.0" );
            } catch {
                hcapScoreTextBox.Text = "";
            }

            isLoadInProg = false;
        }

        private DataRow addNewTrickScoreEntry( DataGridViewRow inTourEventRegRow, Byte inRound ) {
            roundActiveSelect.RoundValue = inRound.ToString();

            DataRowView newDataRow = myScoreDataTable.DefaultView.AddNew();
            newDataRow["SanctionId"] = TrickEventData.mySanctionNum;
            newDataRow["MemberId"] = HelperFunctions.getViewRowColValue( inTourEventRegRow, "MemberId", "" );
            newDataRow["AgeGroup"] = HelperFunctions.getViewRowColValue( inTourEventRegRow, "AgeGroup", "" ); ;
            newDataRow["Round"] = inRound;
            newDataRow["EventClass"] = (String)inTourEventRegRow.Cells["EventClass"].Value;

            newDataRow["PK"] = -1;
            newDataRow["Score"] = 0;
            newDataRow["ScorePass1"] = 0;
            newDataRow["ScorePass2"] = 0;
            newDataRow["NopsScore"] = 0;
            newDataRow["Rating"] = "";

            TourBoatTextbox.Text = buildBoatModelNameDisplay();
            TourBoatTextbox.Select( 0, 0 );
            if ( TourBoatTextbox.Text.Length > 0 ) {
                newDataRow["Boat"] = calcBoatCodeFromDisplay( TourBoatTextbox.Text );
            } else {
                newDataRow["Boat"] = "";
            }

            newDataRow["Note"] = "";
            newDataRow.EndEdit();

            return myScoreDataTable.Rows[0];
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
            if ( TrickEventData.isCollegiateEvent() && HelperFunctions.isGroupNcwsa( curAgeGroup ) ) {
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
            if ( myPass2DataTable != null && myPass2DataTable.Rows.Count > 0
                && Pass2DataGridView != null && Pass2DataGridView.Rows.Count > 0
                ) {
                Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
            }
            if ( myPass1DataTable != null && myPass1DataTable.Rows.Count > 0
                && Pass1DataGridView != null && Pass1DataGridView.Rows.Count > 0
                ) {
                Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
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
            String curColPrefix = inPassView.Name.Substring( 0, 5 );
            String curSkierClass = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "EventClass", TrickEventData.myTourClass );

            DataGridViewRow curViewRow;
            inPassView.Rows.Clear();

            Int16 curPassScore = 0;
            int curViewIdx = 0;
            foreach ( DataRow curDataRow in inPassDataTable.Rows ) {
                if ( HelperFunctions.isObjectEmpty( HelperFunctions.getDataRowColValue( curDataRow, "Code", "" ) ) ) continue;

                curViewIdx = inPassView.Rows.Add();
                curViewRow = inPassView.Rows[curViewIdx];
                curViewRow.Cells[curColPrefix + "Updated"].Value = "N";
                curViewRow.Cells[curColPrefix + "PK"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PK", "" );
                curViewRow.Cells[curColPrefix + "SanctionId"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SanctionId", "" );
                curViewRow.Cells[curColPrefix + "MemberId"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );
                curViewRow.Cells[curColPrefix + "AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                curViewRow.Cells[curColPrefix + "Round"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Round", "" );
                curViewRow.Cells[curColPrefix + "PassNum"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PassNum", "" );
                curViewRow.Cells[curColPrefix + "Seq"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Seq", "" );
                curNumSkis = Int16.Parse( HelperFunctions.getDataRowColValue( curDataRow, "Skis", "1" ) );
                curViewRow.Cells[curColPrefix + "Skis"].Value = myTrickValidation.setNumSkisDisplay( curNumSkis );
                curViewRow.Cells[curColPrefix + "Code"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Code", "" );
                
                curViewRow.Cells[curColPrefix + "Results"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Results", "" );
                if ( !( curViewRow.Cells[curColPrefix + "Results"].Value.ToString().ToUpper().Equals( "CREDIT" ) ) ) {
                    curViewRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                    curViewRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                }

                curViewRow.Cells[curColPrefix + "Points"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Score", "" );
                if ( HelperFunctions.isObjectPopulated( curViewRow.Cells[curColPrefix + "Points"].Value ) ) {
                    curPassScore += (Int16)curDataRow["Score"];
                }
                curViewRow.Cells[curColPrefix + "PointsTotal"].Value = curPassScore.ToString();

                curTrickRow = getTrickRow( curDataRow, curSkierClass );
                if ( curTrickRow == null ) {
                    curViewRow.Cells[curColPrefix + "StartPos"].Value = "";
                    curViewRow.Cells[curColPrefix + "NumTurns"].Value = "";
                    curViewRow.Cells[curColPrefix + "RuleNum"].Value = "";
                    curViewRow.Cells[curColPrefix + "TypeCode"].Value = "";
                
                } else {
                    curRuleNum = (Int16)( (Int16)curTrickRow["RuleNum"] + ( curNumSkis * 100 ) );
                    if ( ( (String)curDataRow["Code"] ).Substring( 0, 1 ).Equals( "R" ) ) curRuleNum = (Int16)( curRuleNum + 200 );
                    curViewRow.Cells[curColPrefix + "StartPos"].Value = HelperFunctions.getDataRowColValue( curTrickRow, "StartPos", "" );
                    curViewRow.Cells[curColPrefix + "NumTurns"].Value = HelperFunctions.getDataRowColValue( curTrickRow, "NumTurns", "" );
                    curViewRow.Cells[curColPrefix + "RuleNum"].Value = curRuleNum.ToString();
                    curViewRow.Cells[curColPrefix + "TypeCode"].Value = HelperFunctions.getDataRowColValue( curTrickRow, "TypeCode", "" );
                }
            }
            isLoadInProg = false;
        }

        private DataTable getTrickPassValues( DataGridView inPassView ) {
            String curColPrefix = inPassView.Name.Substring( 0, 5 );

            DataTable curDataTable = myPass1DataTable.Clone();
            DataColumn curCol = new DataColumn();
            curCol.ColumnName = "StartPos";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "NumTurns";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "TypeCode";
            curCol.DataType = System.Type.GetType( "System.Byte" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            curCol = new DataColumn();
            curCol.ColumnName = "RuleNum";
            curCol.DataType = System.Type.GetType( "System.Int16" );
            curCol.AllowDBNull = false;
            curCol.ReadOnly = false;
            curCol.DefaultValue = 0;
            curDataTable.Columns.Add( curCol );

            foreach ( DataGridViewRow curViewRow in inPassView.Rows ) {
                if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "PK", "" ) ) ) continue;

                DataRow curDataRow = curDataTable.NewRow();
                curDataRow["PK"] = Int64.Parse( (String)curViewRow.Cells[curColPrefix + "PK"].Value );

                curDataRow["SanctionId"] = HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "SanctionId", "" );
                curDataRow["MemberId"] = HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "MemberId", "" );
                curDataRow["AgeGroup"] = HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "AgeGroup", "" );
                curDataRow["Results"] = HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Results", "" );
                curDataRow["Code"] = HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Code", "" );
                curDataRow["Skis"] = myTrickValidation.validateNumSkis( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Skis", "1" ), TrickEventData.myTourRules );

                curDataRow["Round"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Round", "1" ) );
                curDataRow["PassNum"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "PassNum", "0" ) );
                curDataRow["Seq"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Seq", "" ) );
                curDataRow["Score"] = Int16.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "Points", "0" ) );

                if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "StartPos", "" ) ) ) {
                    curDataRow["StartPos"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "StartPos", "0" ) );
                    curDataRow["NumTurns"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "NumTurns", "0" ) );
                    curDataRow["TypeCode"] = Byte.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "TypeCode", "0" ) );
                    curDataRow["RuleNum"] = Int16.Parse( HelperFunctions.getViewRowColValue( curViewRow, curColPrefix + "RuleNum", "0" ) );
                }

                curDataTable.Rows.Add( curDataRow );
            }

            return curDataTable;
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

        private void DataGridView_KeyUp( object sender, KeyEventArgs e ) {
            if ( sender == null ) return;
            DataGridView curPassView = (DataGridView)sender;
            if ( curPassView == null || curPassView.CurrentCell == null ) return;

            try {
                int curRowIdx = curPassView.CurrentCell.RowIndex;
                int curColIdx = curPassView.CurrentCell.ColumnIndex;

                String curColNameFull = curPassView.Columns[curColIdx].Name;
                String curColPrefix = curColNameFull.Substring( 0, 5 );
                String curColName = curColNameFull.Substring( 5 );
                bool isLastRow = false;
                if ( curRowIdx == ( curPassView.Rows.Count - 1 ) ) isLastRow = true;

                if ( e.KeyCode == Keys.Escape ) {
                    e.Handled = true;
                    return;
                } 
                
                if ( e.KeyCode == Keys.Enter ) {
                    if ( curColName.Equals( "Results" ) ) {
                        //noteTextBox.Text += "handleEnterKeyResults, ";
                        if ( handleEnterKeyResults( curPassView, curColPrefix ) ) e.Handled = true;

                    } else if ( curColName.Equals( "Code" ) ) {
                        //noteTextBox.Text += "handleEnterKeyTrickCode, ";
                        if ( handleEnterKeyTrickCode( curPassView, curColPrefix ) ) e.Handled = true;

                    } else if ( curColName.Equals( "Skis" ) ) {
                        //noteTextBox.Text += "handleEnterKeySkis, ";
                        if ( handleEnterKeySkis( curPassView, curColPrefix ) ) e.Handled = true;
                    }

                    return;
                } 
                
                if ( e.KeyCode == Keys.Tab ) {
                    // Analyze current column in the data grid when the TAB key has been pressed
                    if ( curColName.Equals( "Points" ) ) {
                        isLoadInProg = true;
                        if ( isPassEnded ) {
                            curPassView.CurrentCell = curPassView.Rows[0].Cells[curColPrefix + "Code"];

                        } else if ( HelperFunctions.getViewRowColValue( curPassView.Rows[curRowIdx], curColPrefix + "Results", "" ).Equals( "Unresolved" ) ) {
                            isLoadInProg = false;
                            e.Handled = true;

                        } else if ( isLastRow ) {
                            Timer curTimerObj = new Timer();
                            curTimerObj.Interval = 15;
                            curTimerObj.Tick += new EventHandler( insertTrickTimer );
                            curTimerObj.Start();
                        
                        } else {
                            curRowIdx++;
                            curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                        }
                        
                        isLoadInProg = false;
                        e.Handled = true;
                    }
                    return;
                } 
                
                if ( e.KeyCode == Keys.Delete ) {
                    deleteTrick( curPassView, curRowIdx, curColPrefix );
                    return;
                }

                if ( e.KeyCode == Keys.Insert ) {
                    int newRowIdx = insertTrick( curPassView, true );
                    curPassView.CurrentCell = curPassView.Rows[newRowIdx].Cells[curColPrefix + "Code"];
                    return;
                }

            } catch ( Exception exp ) {
                MessageBox.Show( exp.Message );
            }

        }

        /*
         * Analyze results column in the current column of the current data grid when the ENTER key has been pressed
         */
        private bool handleEnterKeyResults( DataGridView curPassView, String curColPrefix ) {
            int curColIdx = curPassView.CurrentCell.ColumnIndex;
            int curRowIdx = curPassView.CurrentCell.RowIndex;

            if ( !( isPassEnded ) && isTrickValid ) {
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 15;
                curTimerObj.Tick += new EventHandler( insertTrickTimer );
                curTimerObj.Start();
                return true;
            }

            if ( !( isPassEnded ) ) return false;

            isLoadInProg = true;
            if ( curColPrefix.Equals( "Pass1" ) ) {
                if ( Pass2DataGridView.Visible ) {
                    if ( Pass2DataGridView.Rows.Count > 1 ) {
                        Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                    
                    } else if ( Pass2DataGridView.Rows.Count == 1 ) {
                        if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1], "Pass2Skis", "" ) ) ) {
                            if ( Pass1DataGridView.Rows.Count > 0 ) {
                                Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = HelperFunctions.getViewRowColValue( Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1], "Pass1Skis", "1" );
                                Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                            } else {
                                Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Skis"];
                            }
                        } else {
                            Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
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
                    curPassView.CurrentCell = curPassView.Rows[0].Cells[curColPrefix + "Code"];
                } else if ( Pass2DataGridView.Rows.Count > 0 ) {
                    curPassView.CurrentCell = curPassView.Rows[Pass2DataGridView.Rows.Count - 1].Cells[curColPrefix + "Code"];
                }
                noteTextBox.Focus();
            }

            isLoadInProg = false;
            isPassEnded = false;
            return true;
        }

        /*
         * Analyze trick code column in the current column of the current data grid when the ENTER key has been pressed
         */
        private bool handleEnterKeyTrickCode( DataGridView curPassView, String curColPrefix ) {
            int curColIdx = curPassView.CurrentCell.ColumnIndex;
            int curRowIdx = curPassView.CurrentCell.RowIndex;

            bool isLastRow = false;
            if ( curRowIdx == ( curPassView.Rows.Count - 1 ) ) isLastRow = true;

            if ( isPassEnded ) {
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
                                if ( HelperFunctions.isObjectEmpty( Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value ) ) {
                                    Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = Pass1DataGridView.Rows[curPassView.Rows.Count - 1].Cells["Pass1Skis"].Value.ToString();
                                    isLoadInProg = true;
                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                    isLoadInProg = false;
                                } else {
                                    isLoadInProg = true;
                                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];
                                    isLoadInProg = false;
                                }

                            } else if ( isTrickValid ) {
                                Timer curTimerObj = new Timer();
                                curTimerObj.Interval = 15;
                                curTimerObj.Tick += new EventHandler( insertTrickTimer );
                                curTimerObj.Start();
                                return true;
                            }

                            isPassEnded = false;
                            Pass2DataGridView.Select();
                            Pass2DataGridView.Focus();
                            return true;

                        } else {
                            isPassEnded = false;
                            noteTextBox.Focus();
                            return true;
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

                return false;
            }

            if ( isLastRow ) {
                if ( HelperFunctions.isObjectEmpty( curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"].Value ) ) return false; // No action required.  Leave last row on pass as is
                if ( HelperFunctions.isObjectEmpty( curPassView.Rows[curRowIdx].Cells[curColPrefix + "Results"].Value ) ) return false; // No action required.  Leave last row on pass as is

                String curResults = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Results"].Value.ToString().ToUpper();
                if ( curResults.Equals( "FALL" ) || ( curResults.Equals( "OOC" ) && curRowIdx > 0 ) ) return false; // No action required.  Leave last row on pass as is

                if ( isTrickValid ) {
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                    curTimerObj.Start();
                    return true;
                }
            }

            return false;
        }

        /*
         * Analyze skis column in the current column of the current data grid when the ENTER key has been pressed
         */
        private bool handleEnterKeySkis( DataGridView curPassView, String curColPrefix ) {
            int curColIdx = curPassView.CurrentCell.ColumnIndex;
            int curRowIdx = curPassView.CurrentCell.RowIndex;

            bool isLastRow = false;
            if ( curRowIdx == ( curPassView.Rows.Count - 1 ) ) isLastRow = true;

            if ( curRowIdx > 0 ) {
                if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curPassView.Rows[curRowIdx], curColPrefix + "Code", "" ) ) ) {
                    isLoadInProg = true;
                    curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                    isLoadInProg = false;

                } else if ( HelperFunctions.getViewRowColValue( curPassView.Rows[curRowIdx], curColPrefix + "Results", "" ).Equals("Unresolved" ) ) {
                    return true;

                } else {
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                    curTimerObj.Start();
                }

                return true;

            }

            if ( curRowIdx == 0 ) {
                isLoadInProg = true;

                if ( curColPrefix.Equals( "Pass2" ) ) {
                    if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( Pass2DataGridView.Rows[0], "Pass2Skis", "" ) ) ) {
                        Pass2DataGridView.Rows[0].Cells["Pass2Skis"].Value = HelperFunctions.getViewRowColValue( Pass1DataGridView.Rows[0], "Pass1Skis", "" );
                    }
                    Pass2DataGridView.CurrentCell = Pass2DataGridView.Rows[Pass2DataGridView.Rows.Count - 1].Cells["Pass2Code"];

                } else {
                    if ( HelperFunctions.isObjectEmpty( Pass1DataGridView.Rows[0].Cells["Pass1Skis"].Value ) ) {
                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Skis"];
                    } else {
                        Pass1DataGridView.CurrentCell = Pass1DataGridView.Rows[Pass1DataGridView.Rows.Count - 1].Cells["Pass1Code"];
                    }
                }

                isLoadInProg = false;
                return false;
            }

            if ( isLastRow ) {
                isLoadInProg = true;
                curPassView.CurrentCell = curPassView.Rows[curRowIdx].Cells[curColPrefix + "Code"];
                isLoadInProg = false;
                return true;
            }

            return false;
        }

        private void DataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            curPassView.CurrentCell = curPassView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring( 0, 5 );
            String curColName = curColNameFull.Substring( 5 );

            if ( curColName.Equals( "Skis" ) ) {
                myOrigSkisValue = HelperFunctions.getViewRowColValue( curPassView.Rows[e.RowIndex], curColNameFull, "" );

            } else if ( curColName.Equals( "Code" ) ) {
                myOrigCodeValue = HelperFunctions.getViewRowColValue( curPassView.Rows[e.RowIndex], curColNameFull, "" );

            } else if ( curColName.Equals( "Results" ) ) {
                myOrigResultsValue = HelperFunctions.getViewRowColValue( curPassView.Rows[e.RowIndex], curColNameFull, "" );
            }
        }

        private void DataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring( 0, 5 );
            String curColName = curColNameFull.Substring( 5 );

            if ( isLoadInProg ) return;

            isPassEnded = false;

            if ( curColName.Equals( "Results" ) ) {
                //noteTextBox.Text += "handleCellValidatingResults, ";
                if ( this.myTrickValidation.validateResultStatus( e.FormattedValue.ToString(), TrickEventData.myTourRules ) ) {
                    if ( e.FormattedValue.ToString().Equals( "Fall" )
                        || e.FormattedValue.ToString().Equals( "End" )
                        ) {
                        isPassEnded = true;
                    } else {
                        if ( e.RowIndex > 1 && e.FormattedValue.ToString().Equals( "OOC" ) ) {
                            isPassEnded = true;
                        }
                    }
                    e.Cancel = false;
                
                } else {
                    MessageBox.Show( this.myTrickValidation.ValidationMessage );
                    e.Cancel = true;
                }
            
            } else if ( curColName.Equals( "Skis" ) ) {
                //noteTextBox.Text += "handleCellValidatingSkis, ";
                if ( this.myTrickValidation.validateNumSkis( e.FormattedValue.ToString(), TrickEventData.myTourRules ) >= 0 ) {
                    e.Cancel = false;
                } else {
                    MessageBox.Show( this.myTrickValidation.ValidationMessage );
                    e.Cancel = true;
                }
            
            } else if ( curColName.Equals( "Code" ) ) {
                //noteTextBox.Text += "handleCellValidatingCode, ";
                e.Cancel = handleCellValidatingTrickCode( curPassView, e.RowIndex, e.FormattedValue, curColPrefix );
            }
        }

        private bool handleCellValidatingTrickCode( DataGridView curPassView, int inRowIndex, object inFormattedValue, String curColPrefix ) {
            bool curPassFall = false;

            if ( HelperFunctions.isObjectEmpty( inFormattedValue ) ) return false;

            if ( inRowIndex > 0 ) {
                if ( curPassView.Rows[inRowIndex - 1].Cells[curColPrefix + "Results"].Value.ToString().Equals( "Fall" ) ) curPassFall = true;

            } else {
                if ( inFormattedValue.ToString().ToUpper().Equals( "NSP" ) ) {
                    isPassEnded = true;

                } else if ( inFormattedValue.ToString().ToUpper().Equals( "FALL" ) ) {
                    DataRow curClassRowSkier = TrickEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
                    if ( (Decimal)curClassRowSkier["ListCodeNum"] > (Decimal)TrickEventData.myClassERow["ListCodeNum"] ) isPassEnded = true;
                }

                String curEventGroup = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "EventGroup", "" );
                String curAgeGroup = HelperFunctions.getViewRowColValue( TourEventRegDataGridView.Rows[myEventRegViewIdx], "AgeGroup", "" );
                if ( !( curEventGroup.Equals( myPrevEventGroup ) ) ) {
                    /*
                     * Provide a warning message for class R events when official assignments have not been entered for the round and event group
                     * These assignments are not mandatory but they are strongly preferred and are very helpful for the TCs
                     */
                    myCheckOfficials.readOfficialAssignments( TrickEventData.mySanctionNum, "Trick", curAgeGroup, curEventGroup, roundActiveSelect.RoundValue );
                    if ( (Decimal)TrickEventData.myClassRowTour["ListCodeNum"] >= (Decimal)TrickEventData.myClassERow["ListCodeNum"] ) {
                        String curWarnMsg = String.Format( "Warn:Officials:Round:{0}:EventGroup:{1}", roundActiveSelect.RoundValue, curEventGroup );
                        if ( !( myCompletedNotices.Contains( curWarnMsg ) ) ) {
                            if ( myCheckOfficials.officialAsgmtCount == 0 ) {
                                MessageBox.Show( "No officials have been assigned for this event group and round "
                                    + "\n\nThese assignments are not mandatory but they are strongly recommended and are very helpful for the TCs" );
                                myCompletedNotices.Add( curWarnMsg );
                            }
                        }
                    }
                }
                myPrevEventGroup = curEventGroup;
            }

            if ( curPassFall ) {
                MessageBox.Show( "No tricks allowed after a fall or trick marked as end" );
                return false;
            }

            if ( inFormattedValue.ToString().ToUpper().Equals( "END" ) || inFormattedValue.ToString().ToUpper().Equals( "HORN" ) ) {
                isPassEnded = true;
                return false;
            }

            if ( inFormattedValue.ToString().ToUpper().Equals( myOrigCodeValue.ToUpper() ) ) return false;

            DataGridViewRow curPassRow = curPassView.Rows[inRowIndex];
            curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
            if ( curPassRow.Cells[curColPrefix + "Skis"].Value == null ) return true;

            Int16 curNumSkis = 0;
            if ( this.myTrickValidation.validateNumSkis( (String)curPassRow.Cells[curColPrefix + "Skis"].Value, TrickEventData.myTourRules ) >= 0 ) {
                curNumSkis = this.myTrickValidation.NumSkis;
                return false;
            }

            MessageBox.Show( this.myTrickValidation.ValidationMessage );
            return true;
        }

        private void DataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curPassView = (DataGridView)sender;
            DataGridViewRow curPassRow = (DataGridViewRow)curPassView.CurrentRow;
            String curColNameFull = curPassView.Columns[e.ColumnIndex].Name;
            String curColPrefix = curColNameFull.Substring( 0, 5 );
            String curColName = curColNameFull.Substring( 5 );

            if ( isLoadInProg ) return;

            if ( curColName.Equals( "Results" ) ) {
                //noteTextBox.Text += "handleCellValidatedResults, ";
                if ( handleCellValidatedResults( curPassView, curPassRow, curColPrefix ) ) {
                    isPassEnded = true;
                    isDataModified = true;
                    isDataModifiedInProgress = true;

                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                    curTimerObj.Start();

                } else if ( isPassEnded ) {
                    isDataModified = true;
                    isDataModifiedInProgress = true;

                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                    curTimerObj.Start();
                }

            } else if ( curColName.Equals( "Skis" ) ) {
                //noteTextBox.Text += "handleCellValidatedSkis, ";
                if ( handleCellValidatedSkis( curPassView, curPassRow, curColPrefix ) ) {
                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( insertTrickTimer );
                    curTimerObj.Start();
                }

            } else if ( curColName.Equals( "Code" ) ) {
                //noteTextBox.Text += "handleCellValidatedCode, ";
                if ( handleCellValidatedCode( curPassView, curPassRow, curColPrefix ) ) {
                    isPassEnded = true;
                    isDataModified = true;
                    isDataModifiedInProgress = true;

                    Timer curTimerObj = new Timer();
                    curTimerObj.Interval = 15;
                    curTimerObj.Tick += new EventHandler( calcUpdateScoreTimer );
                    curTimerObj.Start();
                }
            }
            
            myOrigCodeValue = (String)curPassRow.Cells[e.ColumnIndex].Value;
        }

        /*
         * Validate trick code cell in the current data grid row
         */
        private bool handleCellValidatedCode( DataGridView curPassView, DataGridViewRow curPassRow, String curColPrefix ) {
            Int16 curNumSkis = 0;
            String curCode = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ).ToUpper();
            String curResult = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();
            String curSkis = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ).ToUpper();
            
            if ( HelperFunctions.isObjectEmpty( curCode ) ) return false; // Exit if no value entered or value removed

            if ( curCode.Equals( myOrigCodeValue.ToUpper() ) ) {
                #region No change to trick code but checking other fields to see if they have an impact on the trick code validation
                if ( HelperFunctions.isObjectEmpty( curSkis ) ) return false;
                if ( HelperFunctions.isObjectEmpty( curResult ) ) return false;

                if ( curCode.Equals( "END" ) || curCode.Equals( "HORN" ) ) {
                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                    return true;
                } 
                
                if ( curCode.Equals( "FALL" ) ) {
                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                    curNumSkis = this.myTrickValidation.validateNumSkis( curSkis, TrickEventData.myTourRules );
                    checkTrickCode( curPassView, curPassRow.Index, curCode, curNumSkis, curColPrefix );
                    if ( curPassRow.Index == 0 ) {
                        curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                        DataRow curClassRowSkier = TrickEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
                        if ( (Decimal)TrickEventData.myClassRowSkier["ListCodeNum"] > (Decimal)TrickEventData.myClassERow["ListCodeNum"] ) return true;
                        if ( curColPrefix.Equals( "Pass2" )
                            && Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().ToLower().Equals( "fall" ) ) return true;

                        return false;
                    } 
                    
                    if ( curPassRow.Index == 1 ) curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                    
                    return true;
                } 
                
                if ( curCode.Equals( "NSP" ) ) {
                    curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                    return true;
                }

                curNumSkis = this.myTrickValidation.validateNumSkis( curSkis, TrickEventData.myTourRules );
                if ( checkTrickCode( curPassView, curPassRow.Index, curCode, curNumSkis, curColPrefix ) ) {
                    isTrickValid = true;
                
                } else {
                    isTrickValid = false;
                    curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                    curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                    curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                    curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                    isDataModified = true;
                }

                return false;
                #endregion
            }

            #region Trick code has been changed therefore perform validation
            isDataModified = true;
            curPassRow.Cells[curColPrefix + "Code"].Value = curCode;
            curPassRow.Cells[curColPrefix + "Points"].Value = "0";
            curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";

            if ( HelperFunctions.isObjectEmpty( curSkis ) ) return false;
            if ( HelperFunctions.isObjectEmpty( curResult ) ) return false;

            if ( curCode.Equals( "END" ) || curCode.Equals( "HORN" ) ) {
                curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                return true;
            }

            if ( curCode.Equals( "FALL" ) ) {
                curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";

                curNumSkis = this.myTrickValidation.validateNumSkis( curSkis, TrickEventData.myTourRules );
                checkTrickCode( curPassView, curPassRow.Index, curCode, curNumSkis, curColPrefix );
                if ( curPassRow.Index == 0 ) {
                    curPassRow.Cells[curColPrefix + "Results"].Value = "Before";
                    DataRow curClassRowSkier = TrickEventData.getSkierClass( (String)scoreEventClass.SelectedValue );
                    if ( (Decimal)TrickEventData.myClassRowSkier["ListCodeNum"] > (Decimal)TrickEventData.myClassERow["ListCodeNum"] ) return true;
                    if ( curColPrefix.Equals( "Pass2" )
                        && Pass1DataGridView.Rows[0].Cells["Pass1Code"].Value.ToString().ToLower().Equals( "fall" ) ) return true;

                    return false;
                }

                if ( curPassRow.Index == 1 ) curPassRow.Cells[curColPrefix + "Results"].Value = "Before";

                return true;
            }

            if ( curCode.Equals( "NSP" ) ) {
                curPassRow.Cells[curColPrefix + "Points"].Value = "0";
                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                isPassEnded = true;
            }

            curNumSkis = this.myTrickValidation.validateNumSkis( curSkis, TrickEventData.myTourRules );
            if ( curNumSkis < 0 ) {
                isTrickValid = false;
                curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                return false;
            }

            if ( checkTrickCode( curPassView, curPassRow.Index, curCode, curNumSkis, curColPrefix ) ) {
                // Retrieving results because it could have been changed during validation
                curResult = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();
                isTrickValid = true;
                if ( curResult.Equals( "CREDIT" ) ) {
                    curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
                
                } else {
                    curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
                    int curPoints = 0;
                    Int32.TryParse( (String)curPassRow.Cells[curColPrefix + "Points"].Value, out curPoints );
                    if ( curPoints > 0 ) curPassRow.Cells[curColPrefix + "Results"].Value = "Credit";
                }
                curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = SystemColors.ControlText;
                curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = SystemColors.Window;
                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                return false;
            }

            isTrickValid = false;
            curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
            curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
            curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
            return false;
            #endregion
        }

        /*
         * Validate results cell in the current data grid row
         */
        private bool handleCellValidatedResults( DataGridView curPassView, DataGridViewRow curPassRow, String curColPrefix ) {
            String curResult = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();
            String curSkis = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ).ToUpper();
            String curCode = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ).ToUpper();

            if ( HelperFunctions.isObjectEmpty( curResult ) ) return false; // Exit if no value entered or value removed
            if ( curResult.Equals( myOrigResultsValue.ToUpper() ) ) return false; // Exit if value has not been changed

            if ( curResult.Equals( "CREDIT" ) ) {
                isDataModified = true;
                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
                curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = SystemColors.ControlText;
                curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = SystemColors.Window;
                if ( curPassRow.Index == ( curPassView.Rows.Count - 1 ) ) return true;
                return false;
            
            } else if ( curResult.Equals("OOC")) {
                if ( HelperFunctions.isObjectEmpty( curCode ) || HelperFunctions.isObjectEmpty( curSkis ) ) {
                    isPassEnded = false;
                    return false;
                }
			}

            if ( HelperFunctions.isObjectEmpty( curCode ) ) return false; // Exit if no value entered or value removed
            if ( HelperFunctions.isObjectEmpty( curSkis ) ) return false; // Exit if no value entered or value removed

            isDataModified = true;
            curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
            curPassRow.Cells[curColPrefix + "Points"].Value = "0";

            curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
            curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;

            return false;
        }

        /*
         * Validate skis cell in the current data grid row
         */
        private bool handleCellValidatedSkis( DataGridView curPassView, DataGridViewRow curPassRow, String curColPrefix ) {
            String curSkis = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ).ToUpper();
            String curCode = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ).ToUpper();
            String curResult = HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).ToUpper();

            if ( HelperFunctions.isObjectEmpty( curSkis ) ) return false;
            if ( curSkis.Equals( myOrigSkisValue ) ) return false;
            if ( HelperFunctions.isObjectEmpty( curCode ) ) return false; // Exit if no value entered or value removed

            Int16 curNumSkis = this.myTrickValidation.validateNumSkis( curSkis, TrickEventData.myTourRules );
            if ( curNumSkis < 0 ) {
                isTrickValid = false;
                curPassRow.Cells[curColPrefix + "Results"].Value = "Unresolved";
                curPassRow.Cells[curColPrefix + "Code"].Style.ForeColor = Color.Red;
                curPassRow.Cells[curColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                isDataModified = true;
                curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
                return false;
            }

            if ( curResult.Equals( "CREDIT" ) ) {
                curPassRow.Cells[curColPrefix + "Points"].Value = calcPoints( curPassView, curPassRow, curColPrefix ).ToString();
            } else {
                curPassRow.Cells[curColPrefix + "Points"].Value = "0";
            }
            curPassRow.Cells[curColPrefix + "Updated"].Value = "Y";
            isDataModified = true;
            
            if ( curPassRow.Index == ( curPassView.Rows.Count - 1 ) ) return true;
            return false;
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
                prevTrickRows[0]["TrickCode"] = HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Code", "" );
                prevTrickRows[0]["NumSkis"] = this.myTrickValidation.validateNumSkis( 
                    HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Skis", "0" ), TrickEventData.myTourRules);
                prevTrickRows[0]["Points"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Points", "0" ) );
                if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "StartPos", "" ) ) ) {
                    prevTrickRows[0]["StartPos"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "StartPos", "0" ) );
                    prevTrickRows[0]["NumTurns"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "NumTurns", "0" ) );
                    prevTrickRows[0]["TypeCode"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "TypeCode", "0" ) );
                    prevTrickRows[0]["RuleNum"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "RuleNum", "0" ) );
                }

                if ( curIdx > 0 ) {
                    curIdx--;
                    //NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode
                    prevTrickRows[1] = myTrickListDataTable.NewRow();
                    prevTrickRows[1]["TrickCode"] = HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Code", "" );
                    prevTrickRows[1]["Points"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Points", "0" ) );
                    if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "StartPos", "" ) ) ) {
                        prevTrickRows[1]["StartPos"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "StartPos", "0" ) );
                        prevTrickRows[1]["NumTurns"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "NumTurns", "0" ) );
                        prevTrickRows[1]["TypeCode"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "TypeCode", "0" ) );
                        prevTrickRows[1]["RuleNum"] = Convert.ToInt16( HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "RuleNum", "0" ) );
                        prevTrickRows[1]["NumSkis"] = this.myTrickValidation.validateNumSkis(
                            HelperFunctions.getViewRowColValue( inPassView.Rows[curIdx], inColPrefix + "Skis", "0" ), TrickEventData.myTourRules );
                    }
                }
			}

			/*
              Note: If return code is true than use method UpdatedTrickCode to retrieve the trick code as modified by the validation processing
                    If return code is false then use method ValidationMessage to retrieve the message associated with the reason it failed validation
            */
			curIdx = inPassRowIdx;
            if ( myTrickValidation.validateTrickCode( inCode, inNumSkis, curSkierClass, prevTrickRows ) ) {
                inPassView.Rows[curIdx].Cells[inColPrefix + "Code"].Value = myTrickValidation.UpdatedTrickCode;
                return true;
			}
                
            MessageBox.Show(myTrickValidation.ValidationMessage);
            return false;
        }

        private Int16 calcPoints( DataGridView inPassView, DataGridViewRow inPassRow, String inColPrefix ) {
            DataTable curPass1DataTable, curPass2DataTable;
            DataRow curDataRow;

            curPass1DataTable = getTrickPassValues(Pass1DataGridView);
            curPass2DataTable = getTrickPassValues(Pass2DataGridView);
            String curSkierEventClass = (String) TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["EventClass"].Value;
            if ( inColPrefix.Equals("Pass1") ) {
                curDataRow = curPass1DataTable.Rows[inPassRow.Index];
            } else {
                curDataRow = curPass2DataTable.Rows[inPassRow.Index];
            }

            Int16 returnPoints = myTrickValidation.calcPoints( curPass1DataTable, curPass2DataTable, curDataRow, inPassRow.Index, inColPrefix, curSkierEventClass );
            if ( returnPoints < 0 ) {
                MessageBox.Show(myTrickValidation.ValidationMessage);
				return 0;
			}

            //Update row on DataGridView using curViewRow which should have been updated
            HelperFunctions.getDataRowColValue( curDataRow, "RuleNum", "" );
			inPassRow.Cells[inColPrefix + "Code"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Code", "" );
            inPassRow.Cells[inColPrefix + "Results"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Results", "" );
            inPassRow.Cells[inColPrefix + "StartPos"].Value = HelperFunctions.getDataRowColValue( curDataRow, "StartPos", "" );
            inPassRow.Cells[inColPrefix + "NumTurns"].Value = HelperFunctions.getDataRowColValue( curDataRow, "NumTurns", "" );
            inPassRow.Cells[inColPrefix + "TypeCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TypeCode", "" );
            inPassRow.Cells[inColPrefix + "RuleNum"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RuleNum", "" );
            inPassRow.Cells[inColPrefix + "Points"].Value = returnPoints.ToString();

			if ( inColPrefix.Equals( "Pass1" ) ) {
				curPass1DataTable.Rows[inPassRow.Index]["Score"] = returnPoints;
			} else {
				curPass2DataTable.Rows[inPassRow.Index]["Score"] = returnPoints;
			}

            callRunPointsForPass( curPass1DataTable, Pass1DataGridView, "Pass1" );
            callRunPointsForPass( curPass2DataTable, Pass2DataGridView, "Pass2" );

			if ( myTrickValidation.ValidationMessage.Length > 0 ) {
				MessageBox.Show( myTrickValidation.ValidationMessage );
			}

			return returnPoints;
        }

        private void callRunPointsForPass( DataTable inPassDataTable, DataGridView inPassView, String inColPrefix ) {
            int passScore = 0;
            int curViewIdx = 0;
            foreach ( DataRow curPassDataRow in inPassDataTable.Rows ) {
                DataGridViewRow curViewRow = inPassView.Rows[curViewIdx];
                curViewRow.Cells[inColPrefix + "Results"].Value = curPassDataRow["Results"];
                curViewRow.Cells[inColPrefix + "Points"].Value = ( (Int16)curPassDataRow["Score"] ).ToString();
                passScore += (Int16)curPassDataRow["Score"];
                curViewRow.Cells[inColPrefix + "PointsTotal"].Value = passScore.ToString();

                if ( !( HelperFunctions.getViewRowColValue( curViewRow, inColPrefix + "Results", "" ).ToUpper().Equals( "CREDIT" ) ) ) {
                    curViewRow.Cells[inColPrefix + "Code"].Style.ForeColor = Color.Red;
                    curViewRow.Cells[inColPrefix + "Code"].Style.BackColor = Color.LightBlue;
                }

                curViewIdx++;
            }
        }

        private void ViewTrickDataButton_Click( object sender, EventArgs e ) {
            StringBuilder curMsg = new StringBuilder();
            String curColPrefix = "Pass1";
            foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                curMsg = new StringBuilder();
                if ( !( HelperFunctions.isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value ) ) ) {
                    curMsg.Append( "Pass 1" );
                    curMsg.Append( "\n Update Flag = " + HelperFunctions.getViewRowColValue(curPassRow, curColPrefix + "Updated", "" ));
                    curMsg.Append( "\n PK = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PK", "" ) );
                    curMsg.Append( "\n PassNum = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PassNum", "" ) );
                    curMsg.Append( "\n Seq = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Seq", "" ) );
                    curMsg.Append( "\n NumSkis = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ) );
                    curMsg.Append( "\n Code = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ) );
                    curMsg.Append( "\n Results = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ) );
                    curMsg.Append( "\n RuleNum = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "RuleNum", "" ) );
                    curMsg.Append( "\n StartPos = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "StartPos", "" ) );
                    curMsg.Append( "\n NumTurns = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "NumTurns", "" ) );
                    curMsg.Append( "\n TypeCodeValue = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "TypeCodeValue", "" ) );
                
                } else {
                    curMsg.Append( "Null row\n" );
                }
                
                MessageBox.Show( curMsg.ToString() );
            }

            curColPrefix = "Pass2";
            foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                curMsg = new StringBuilder();
                if ( !( HelperFunctions.isObjectEmpty( curPassRow.Cells[curColPrefix + "Skis"].Value ) ) ) {
                    curMsg.Append( "Pass 2" );
                    curMsg.Append( "\n Update Flag = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Updated", "" ) );
                    curMsg.Append( "\n PK = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PK", "" ) );
                    curMsg.Append( "\n PassNum = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "PassNum", "" ) );
                    curMsg.Append( "\n Seq = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Seq", "" ) );
                    curMsg.Append( "\n NumSkis = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Skis", "" ) );
                    curMsg.Append( "\n Code = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Code", "" ) );
                    curMsg.Append( "\n Results = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ) );
                    curMsg.Append( "\n RuleNum = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "RuleNum", "" ) );
                    curMsg.Append( "\n StartPos = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "StartPos", "" ) );
                    curMsg.Append( "\n NumTurns = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "NumTurns", "" ) );
                    curMsg.Append( "\n TypeCodeValue = " + HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "TypeCodeValue", "" ) );
                } else {
                    curMsg.Append( "Null row\n" );
                }
                MessageBox.Show( curMsg.ToString() );
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

        private void listApprovedBoatsDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
			if ( e.RowIndex > 0 ) {
				myBoatListIdx = e.RowIndex;
				updateBoatSelect();
			}
		}

		private void updateBoatSelect() {
            String curMethodName = "Trick:ScoreCalc:updateBoatSelect";
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
			Pass1DataGridView.Focus();
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
			if ( inBoatCode.Length > 1 ) {
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
					return "";

				}
			} else {
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

		private void ForceCompButton_Click( object sender, EventArgs e ) {
			if ( TourEventRegDataGridView.Rows.Count > 0 ) {
				if ( TrickEventData.isCollegiateEvent() ) {
					if ( Pass1DataGridView.Rows.Count == 1 ) {
                        if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( Pass1DataGridView.Rows[0], "Pass1Code", "" ) ) ) {
							skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
							if ( skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK ) {
								String curReason = skierDoneReasonDialogForm.ReasonText;
								if ( curReason.Length > 3 ) {
									noteTextBox.Text = curReason;
									setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
									if ( saveTrickScore() ) isDataModified = false;
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
				if ( TourEventRegDataGridView.Rows[myEventRegViewIdx].Cells["Status"].Value.Equals( "2-InProg" ) ) {
					skierDoneReasonDialogForm.ReasonText = noteTextBox.Text;
					if ( skierDoneReasonDialogForm.ShowDialog() == DialogResult.OK ) {
						String curReason = skierDoneReasonDialogForm.ReasonText;
						if ( curReason.Length > 3 ) {
							noteTextBox.Text = curReason;
							setEventRegRowStatus( "4-Done", TourEventRegDataGridView.Rows[myEventRegViewIdx] );
							if ( saveTrickScore() ) isDataModified = false;
						
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

        private void navPrintResults_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }
        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printResults();
        }
        private void printResults() {
            Font[] saveFonts = new Font[2];
            System.Drawing.Color[] saveColors = new System.Drawing.Color[4];

            if ( isDataModified ) {
                try {
                    CalcScoreButton_Click (null, null);
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
            DataGridViewRow curPrintRow;
            PrintDataGridView.Rows.Clear();
            for ( int curPassIdx = 0; curPassIdx < curMaxRows; curPassIdx++ ) {
                curPrintIdx = PrintDataGridView.Rows.Add();
                curPrintRow = PrintDataGridView.Rows[curPrintIdx];
                curPassIdx = curPrintIdx;
                if ( curPassIdx < Pass1DataGridView.Rows.Count ) loadPrintPassRow( curPrintRow, Pass1DataGridView.Rows[curPassIdx], "Pass1" );
                if ( curPassIdx < Pass2DataGridView.Rows.Count ) loadPrintPassRow( curPrintRow, Pass2DataGridView.Rows[curPassIdx], "Pass2" );
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

            mySubtitle = new StringRowPrinter( "Round " + roundActiveSelect.RoundValue + " Pass 1",
                0, 100, 325, fontSubTitleLabel.Height,
                Color.DarkBlue, Color.White, fontSubTitleLabel, SubtitleStringFormatLeft );
            myPrintDataGrid.SubtitleRow = mySubtitle;
            mySubtitle = new StringRowPrinter( "Round " + roundActiveSelect.RoundValue + " Pass 2",
                325, 100, 325, fontSubTitleData.Height,
                Color.DarkBlue, Color.White, fontSubTitleLabel, SubtitleStringFormatLeft );
            myPrintDataGrid.SubtitleRow = mySubtitle;

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return;

            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
            myPrintDoc.Print();

            Pass1DataGridView.DefaultCellStyle.BackColor = saveColors[0];
            Pass1DataGridView.DefaultCellStyle.ForeColor = saveColors[1];
            Pass2DataGridView.DefaultCellStyle.BackColor = saveColors[2];
            Pass2DataGridView.DefaultCellStyle.ForeColor = saveColors[3];
            Pass1DataGridView.DefaultCellStyle.Font = saveFonts[0];
            Pass2DataGridView.DefaultCellStyle.Font = saveFonts[1];
        }

        private void loadPrintPassRow(DataGridViewRow inPrintRow, DataGridViewRow inPassRow, String inColPrefix ) {
            inPrintRow.Cells["Print" + inColPrefix+ "Seq"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Seq", "" );
            inPrintRow.Cells["Print" + inColPrefix + "Skis"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Skis", "" );
            inPrintRow.Cells["Print" + inColPrefix+ "Code"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Code", "" );
            inPrintRow.Cells["Print" + inColPrefix + "Results"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Results", "" );
            inPrintRow.Cells["Print" + inColPrefix + "Points"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Points", "" );

            if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "Points", "" ) ) ) {
                inPrintRow.Cells["PrintPass1PointsTotal"].Value = "";
            } else {
                inPrintRow.Cells["Print" + inColPrefix + "PointsTotal"].Value = HelperFunctions.getViewRowColValue( inPassRow, inColPrefix + "PointsTotal", "" );
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private void getApprovedTowboats() {
			int curIdx = 0;
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT Distinct ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq, Owner, Notes, HullId, ModelYear, BoatModel " );
			curSqlStmt.Append( "FROM TourBoatUse BU " );
			curSqlStmt.Append( "INNER JOIN CodeValueList BL ON BL.ListCode = BU.HullId " );
			curSqlStmt.Append( "WHERE BL.ListName = 'ApprovedBoats' AND BU.SanctionId = '" + TrickEventData.mySanctionNum + "' " );
			curSqlStmt.Append( "Union " );
			curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc, SortSeq, '' as Owner, '' as Notes, ListCode as HullId, '' as ModelYear, CodeValue as BoatModel " );
			curSqlStmt.Append( "FROM CodeValueList BL " );
			curSqlStmt.Append( "WHERE ListName = 'ApprovedBoats' AND ListCode IN ('--Select--', 'Unlisted') " );
			curSqlStmt.Append( "ORDER BY SortSeq DESC" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

			listApprovedBoatsDataGridView.Rows.Clear();
			foreach ( DataRow curRow in curDataTable.Rows ) {
				String[] boatSpecList = curRow["CodeDesc"].ToString().Split( '|' );
				listApprovedBoatsDataGridView.Rows.Add( 1 );
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatCode"].Value = curRow["HullId"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatOwner"].Value = curRow["Owner"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatNotes"].Value = curRow["Notes"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatModelApproved"].Value = curRow["BoatModel"].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["HullStatus"].Value = boatSpecList[0].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["EngineSpec"].Value = boatSpecList[1].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["FuelDel"].Value = boatSpecList[2].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["Transmission"].Value = boatSpecList[3].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["Prop"].Value = boatSpecList[4].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["SpeedControl"].Value = boatSpecList[5].ToString();
				listApprovedBoatsDataGridView.Rows[curIdx].Cells["ModelYear"].Value = curRow["ModelYear"].ToString();
				curIdx++;
			}
			myBoatListIdx = 0;

			if ( curDataTable.Rows.Count < 3 ) {
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
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, O.RunOrder, E.RunOrder, E.TeamCode" );
					curSqlStmt.Append( ", COALESCE(O.EventGroup, E.EventGroup) as EventGroup, COALESCE(O.RunOrderGroup, '') as RunOrderGroup" );
					curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, COALESCE(O.RankingScore, E.RankingScore) as RankingScore, E.RankingRating, E.AgeGroup" );
                    curSqlStmt.Append(", E.HCapBase, E.HCapScore, T.TrickBoat, COALESCE (S.Status, 'TBD') AS Status, S.Score, S.LastUpdateDate, E.AgeGroup as Div");
                    curSqlStmt.Append( ", COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     INNER JOIN EventRunOrder O ON E.SanctionId = O.SanctionId AND E.MemberId = O.MemberId AND E.AgeGroup = O.AgeGroup AND E.Event = O.Event AND O.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + TrickEventData.mySanctionNum + "' AND E.Event = 'Trick' " );
                } else {
                    curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.AgeGroup, E.EventGroup, '' as RunOrderGroup, E.RunOrder, E.TeamCode" );
                    curSqlStmt.Append( ", COALESCE(S.EventClass, E.EventClass) as EventClass, E.RankingScore, E.RankingRating, E.AgeGroup, E.HCapBase, E.HCapScore" );
                    curSqlStmt.Append(", T.TrickBoat, COALESCE (S.Status, 'TBD') AS Status, S.Score, S.LastUpdateDate, E.AgeGroup as Div");
                    curSqlStmt.Append(", COALESCE(D.RunOrder, 999) as DivOrder, COALESCE(E.ReadyForPlcmt, 'N') as ReadyForPlcmt ");
                    curSqlStmt.Append( "FROM EventReg E " );
                    curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN TrickScore S ON E.SanctionId = S.SanctionId AND E.MemberId = S.MemberId AND E.AgeGroup = S.AgeGroup AND S.Round = " + inRound.ToString() + " " );
                    curSqlStmt.Append( "     LEFT OUTER JOIN DivOrder D ON D.SanctionId = E.SanctionId AND D.AgeGroup = E.AgeGroup AND D.Event = E.Event " );
                    curSqlStmt.Append( "WHERE E.SanctionId = '" + TrickEventData.mySanctionNum + "' AND E.Event = 'Trick' " );
                }
                if (!(inEventGroup.ToLower().Equals( "all" ))) {
                    if (TrickEventData.isCollegiateEvent()) {
						curSqlStmt.Append( HelperFunctions.getEventGroupFilterNcwsaSql( inEventGroup  ) );
                    
					} else {
                        if (curIdx == 0) {
                            curSqlStmt.Append( "And O.EventGroup = '" + inEventGroup + "' " );
                        } else {
                            curSqlStmt.Append( "And E.EventGroup = '" + inEventGroup + "' " );
                        }
                    }
                }

                myEventRegDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                curRowCount = myEventRegDataTable.Rows.Count;
                curIdx++;
            }
        }

		private Boolean isRunOrderByRound( int inRound ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select count(*) as SkierCount From EventRunOrder " );
			curSqlStmt.Append( "WHERE SanctionId = '" + TrickEventData.mySanctionNum + "' AND Event = 'Trick' AND Round = " + inRound );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( (int) curDataTable.Rows[0]["SkierCount"] > 0 ) {
				return true;
			} else {
				return false;
			}

		}

		private void getSkierScoreByRound( String inMemberId, String inAgeGroup, int inRound ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT S.PK, S.SanctionId, S.MemberId, S.AgeGroup, S.Round, S.EventClass");
            curSqlStmt.Append( ", S.Score, S.ScorePass1, S.ScorePass2, S.NopsScore, E.HCapScore, S.Rating, S.Boat, S.Status, S.Note" );
            curSqlStmt.Append(", Gender, SkiYearAge ");
            curSqlStmt.Append( "FROM TrickScore S " );
            curSqlStmt.Append( "  INNER JOIN TourReg T ON S.SanctionId = T.SanctionId AND S.MemberId = T.MemberId AND S.AgeGroup = T.AgeGroup ");
			curSqlStmt.Append( "  INNER JOIN EventReg E ON S.SanctionId = E.SanctionId AND S.MemberId = E.MemberId AND S.AgeGroup = E.AgeGroup AND E.Event = 'Trick' " );
            curSqlStmt.Append( "WHERE S.SanctionId = '" + TrickEventData.mySanctionNum + "' AND S.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append("  AND S.AgeGroup = '" + inAgeGroup + "' AND S.Round = " + inRound.ToString() + " ");
            curSqlStmt.Append("ORDER BY S.SanctionId, S.MemberId");
            myScoreDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private Boolean checkForSkierRoundScore( String inMemberId, int inRound, String inAgeGroup ) {
			return HelperFunctions.checkForSkierRoundScore( TrickEventData.mySanctionNum, "Trick", inMemberId, inRound, inAgeGroup );
		}

		private DataTable getSkierPassByRound( String inMemberId, String inAgeGroup, int inRound, byte inPass ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum," );
            curSqlStmt.Append( " Seq, Skis, Code, Results, Score, Note" );
            curSqlStmt.Append( " FROM TrickPass " );
            curSqlStmt.Append( " WHERE SanctionId = '" + TrickEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString() + " AND PassNum = " + inPass.ToString() );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private DataTable getSkierPassByRound( String inMemberId, String inAgeGroup, int inRound, byte inPass, byte inSeq ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, MemberId, AgeGroup, Round, PassNum," );
            curSqlStmt.Append( " Seq, Skis, Code, Results, Score, Note" );
            curSqlStmt.Append( " FROM TrickPass " );
            curSqlStmt.Append( " WHERE SanctionId = '" + TrickEventData.mySanctionNum + "' AND MemberId = '" + inMemberId + "'" );
            curSqlStmt.Append( "   AND AgeGroup = '" + inAgeGroup + "' AND Round = " + inRound.ToString()  );
            curSqlStmt.Append( "   AND PassNum = " + inPass.ToString() + " AND Seq = " + inSeq.ToString() );
            curSqlStmt.Append( " ORDER BY SanctionId, MemberId, AgeGroup, Round, PassNum, Seq" );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
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
			}

			return curReturnRow;
        }
        
        private void getTrickList( String inRuleCode ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode " );
            curSqlStmt.Append( " FROM TrickList " );
            curSqlStmt.Append( " WHERE RuleCode = '" + inRuleCode + "'");
            curSqlStmt.Append( " ORDER BY RuleNum, TrickCode" );
            myTrickListDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private bool checkPassError() {
            String curColPrefix = "Pass1";
            foreach ( DataGridViewRow curPassRow in Pass1DataGridView.Rows ) {
                if ( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).Equals( "Unresolved" ) ) return true;
            }

            curColPrefix = "Pass2";
            foreach ( DataGridViewRow curPassRow in Pass2DataGridView.Rows ) {
                if ( HelperFunctions.getViewRowColValue( curPassRow, curColPrefix + "Results", "" ).Equals( "Unresolved" ) ) return true;
            }

            return false;
        }
	}
}
