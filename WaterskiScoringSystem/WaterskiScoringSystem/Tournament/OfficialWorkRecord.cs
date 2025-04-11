using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Admin;

namespace WaterskiScoringSystem.Tournament {
    public partial class OfficialWorkRecord : Form {
        private Boolean isDataModified = false;
        private Boolean isChiefJudge = false;
        private Boolean isChiefDriver = false;
        private Boolean isChiefScore = false;
        private Boolean isChiefSafety = false;

        private String mySanctionNum;
        private String mySortCommand;
        private String myFilterCmd;
        private String myTourRules;

        private TourProperties myTourProperties;
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private ColumnSelectDialogcs columnSelectDialogcs;
        private Dictionary<string, Boolean> myPrintColumnFilter;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        private DataRow myOfficialWorkRow;
        private DataRow myTourRow;
        private DataTable myTourMemberList;

        public OfficialWorkRecord() {
            InitializeComponent();
        }

        private void OfficialWorkRecord_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.OfficialWorkRecord_Width > 0 ) {
                this.Width = Properties.Settings.Default.OfficialWorkRecord_Width;
            }
            if ( Properties.Settings.Default.OfficialWorkRecord_Height > 0 ) {
                this.Height = Properties.Settings.Default.OfficialWorkRecord_Height;
            }
            if ( Properties.Settings.Default.OfficialWorkRecord_Location.X > 0
                && Properties.Settings.Default.OfficialWorkRecord_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.OfficialWorkRecord_Location;
            }
            myTourProperties = TourProperties.Instance;
            mySortCommand = myTourProperties.OfficialWorkRecordSort;

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = PrintDataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = PrintDataGridView.Columns;

            // Retrieve tournament event data from database
            Cursor.Current = Cursors.WaitCursor;
            mySanctionNum = Properties.Settings.Default.AppSanctionNum.Trim();
            if ( !(configWindowAttributes() ) ) {
                Cursor.Current = Cursors.Default;
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                return;
            }

            /*
            * Update officials credits and then load data for viewing
            */
            getRunningOrderColumnfilter();
            navRefresh_Click( null, null );
        }

        private void OfficialWorkRecord_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.OfficialWorkRecord_Width = this.Size.Width;
                Properties.Settings.Default.OfficialWorkRecord_Height = this.Size.Height;
                Properties.Settings.Default.OfficialWorkRecord_Location = this.Location;
            }
        }

        private void OfficialWorkRecord_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                navSaveItem_Click( null, null );
            }
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
			String curMethodName = "OfficialWorkRecord: navRefresh_Click: ";
			try {
                winStatusMsg.Text = "Retrieving tournament member list";
                Cursor.Current = Cursors.WaitCursor;

                OfficialWorkUpdate curWorkRecordUpdate = new OfficialWorkUpdate();
                curWorkRecordUpdate.updateOfficialWorkRecord();

                myTourMemberList = getTourMemberList();
                loadDataGridView();
                Cursor.Current = Cursors.Default;

            } catch ( Exception ex ) {
				String curExcptMsg = "Error attempting to retrieve tournament members\n" + ex.Message;
				MessageBox.Show( curExcptMsg );
				Log.WriteFile( curMethodName + curExcptMsg );
			}
		}

        private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving official entries";

            try {
                listTourMemberDataGridView.Rows.Clear();
                if ( myTourMemberList.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    int curViewIdx = 0;
                    foreach ( DataRow curDataRow in myTourMemberList.Rows ) {
                        curViewIdx = listTourMemberDataGridView.Rows.Add();
                        curViewRow = listTourMemberDataGridView.Rows[curViewIdx];

                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
						curViewRow.Cells["ReadyToSki"].Value = (String)curDataRow["ReadyToSki"];
                        
                        curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
                        curViewRow.Cells["JudgeSlalomRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeSlalomRating", "" );
                        curViewRow.Cells["JudgeTrickRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeTrickRating", "" );
                        curViewRow.Cells["JudgeJumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeJumpRating", "" );
                        curViewRow.Cells["DriverSlalomRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverSlalomRating", "" );
                        curViewRow.Cells["DriverTrickRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverTrickRating", "" );
                        curViewRow.Cells["DriverJumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverJumpRating", "" );
                        curViewRow.Cells["ScorerSlalomRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerSlalomRating", "" );
                        curViewRow.Cells["ScorerTrickRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerTrickRating", "" );
                        curViewRow.Cells["ScorerJumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerJumpRating", "" );
                        curViewRow.Cells["SafetyOfficialRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SafetyOfficialRating", "" );
                        curViewRow.Cells["TechControllerSlalomRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TechControllerSlalomRating", "" );
                        curViewRow.Cells["TechControllerTrickRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TechControllerTrickRating", "" );
                        curViewRow.Cells["TechControllerJumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TechControllerJumpRating", "" );
                        curViewRow.Cells["AnncrOfficialRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AnncrOfficialRating", "" );
                    }
                    listTourMemberDataGridView.CurrentCell = listTourMemberDataGridView.Rows[0].Cells["SkierName"];
                    int curRowPos = curViewIdx + 1;
                    RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + listTourMemberDataGridView.Rows.Count.ToString();
                    Cursor.Current = Cursors.Default;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve available tournament officials \n" + ex.Message );
            }
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            try {
                if ( isDataModified ) saveOfficialData();
            
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveOfficialData() {
			String curMethodName = "OfficialWorkRecord: saveOfficialData: ";
			bool curReturnValue = false;
            int rowsProc = 0;

            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                DataTable curDataTable = DataAccess.getDataTable( "Select MemberID From OfficialWork Where MemberId = '" + editMemberId.Text + "'" );
				if ( curDataTable.Rows.Count > 0 ) {
                    curSqlStmt.Append( "Update OfficialWork Set " );
                    curSqlStmt.Append( "JudgeChief = '" + getValueCheckBox(JudgeChiefCB) + "'" );
                    curSqlStmt.Append( ", JudgeAsstChief = '" + getValueCheckBox(JudgeAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", JudgeAppointed = '" + getValueCheckBox(JudgeAppointedCB) + "'" );
                    curSqlStmt.Append( ", JudgeSlalomCredit = '" + getValueCheckBox(JudgeSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", JudgeTrickCredit = '" + getValueCheckBox(JudgeTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", JudgeJumpCredit = '" + getValueCheckBox(JudgeJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", DriverChief = '" + getValueCheckBox(DriverChiefCB) + "'" );
                    curSqlStmt.Append( ", DriverAsstChief = '" + getValueCheckBox(DriverAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", DriverAppointed = '" + getValueCheckBox(DriverAppointedCB) + "'" );
                    curSqlStmt.Append( ", DriverSlalomCredit = '" + getValueCheckBox(DriverSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", DriverTrickCredit = '" + getValueCheckBox(DriverTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", DriverJumpCredit = '" + getValueCheckBox(DriverJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", ScoreChief = '" + getValueCheckBox(ScoreChiefCB) + "'" );
                    curSqlStmt.Append( ", ScoreAsstChief = '" + getValueCheckBox(ScoreAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", ScoreAppointed = '" + getValueCheckBox(ScoreAppointedCB) + "'" );
                    curSqlStmt.Append( ", ScoreSlalomCredit = '" + getValueCheckBox(ScoreSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", ScoreTrickCredit = '" + getValueCheckBox(ScoreTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", ScoreJumpCredit = '" + getValueCheckBox(ScoreJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", SafetyChief = '" + getValueCheckBox(SafetyChiefCB) + "'" );
                    curSqlStmt.Append( ", SafetyAsstChief = '" + getValueCheckBox(SafetyAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", SafetyAppointed = '" + getValueCheckBox(SafetyAppointedCB) + "'" );
                    curSqlStmt.Append( ", SafetySlalomCredit = '" + getValueCheckBox(SafetySlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", SafetyTrickCredit = '" + getValueCheckBox(SafetyTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", SafetyJumpCredit = '" + getValueCheckBox(SafetyJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", TechChief = '" + getValueCheckBox( TechChiefCB ) + "'" );
                    curSqlStmt.Append( ", TechAsstChief = '" + getValueCheckBox( TechAsstChiefCB ) + "'" );
                    curSqlStmt.Append( ", TechSlalomCredit = '" + getValueCheckBox( TechSlalomCreditCB ) + "'" );
                    curSqlStmt.Append( ", TechTrickCredit = '" + getValueCheckBox(TechTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", TechJumpCredit = '" + getValueCheckBox(TechJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", AnncrChief = '" + getValueCheckBox(AnncrChiefCB) + "'" );
                    curSqlStmt.Append( ", AnncrSlalomCredit = '" + getValueCheckBox(AnncrSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", AnncrTrickCredit = '" + getValueCheckBox(AnncrTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", AnncrJumpCredit = '" + getValueCheckBox(AnncrJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", JudgeSlalomRating = '" + editJudgeSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", JudgeTrickRating = '" + editJudgeTrickRating.Text + "'" );
                    curSqlStmt.Append( ", JudgeJumpRating = '" + editJudgeJumpRating.Text + "'" );
                    
                    curSqlStmt.Append( ", DriverSlalomRating = '" + editDriverSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", DriverTrickRating = '" + editDriverTrickRating.Text + "'" );
                    curSqlStmt.Append( ", DriverJumpRating = '" + editDriverJumpRating.Text + "'" );

                    curSqlStmt.Append( ", ScorerSlalomRating = '" + editScorerSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", ScorerTrickRating = '" + editScorerTrickRating.Text + "'" );
                    curSqlStmt.Append( ", ScorerJumpRating = '" + editScorerJumpRating.Text + "'" );

                    curSqlStmt.Append( ", SafetyOfficialRating = '" + editSafetyOfficialRating.Text + "'" );
                    curSqlStmt.Append( ", TechControllerSlalomRating = '" + editTechControllerSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", TechControllerTrickRating = '" + editTechControllerTrickRating.Text + "'" );
                    curSqlStmt.Append( ", TechControllerJumpRating = '" + editTechControllerJumpRating.Text + "'" );
                    curSqlStmt.Append( ", AnncrOfficialRating = '" + editAnncrOfficialRating.Text + "'" );
                    curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                    curSqlStmt.Append( ", Note = '" + editNote.Text + "'" );
					curSqlStmt.Append( " Where MemberId = '" + editMemberId.Text + "'" );


					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
					if ( rowsProc > 0 ) {
                        isDataModified = false;
                        curReturnValue = true;
                    }
                
                } else {
                    curSqlStmt.Append( "Insert OfficialWork ( " );
					curSqlStmt.Append( "SanctionId, MemberId" );
                    curSqlStmt.Append( ", JudgeChief, JudgeAsstChief, JudgeAppointed, JudgeSlalomCredit, JudgeTrickCredit, JudgeJumpCredit" );
                    curSqlStmt.Append( ", DriverChief, DriverAsstChief, DriverAppointed, DriverSlalomCredit, DriverTrickCredit, DriverJumpCredit" );
                    curSqlStmt.Append( ", ScoreChief, ScoreAsstChief, ScoreAppointed, ScoreSlalomCredit, ScoreTrickCredit, ScoreJumpCredit" );
                    curSqlStmt.Append( ", SafetyChief, SafetyAsstChief, SafetyAppointed, SafetySlalomCredit, SafetyTrickCredit, SafetyJumpCredit" );
                    curSqlStmt.Append( ", TechChief, TechAsstChief, TechSlalomCredit, TechTrickCredit, TechJumpCredit" );
                    curSqlStmt.Append( ", AnncrChief, AnncrSlalomCredit, AnncrTrickCredit, AnncrJumpCredit" );
                    curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
                    curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
                    curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
                    curSqlStmt.Append( ", SafetyOfficialRating, TechControllerSlalomRating, TechControllerTrickRating, TechControllerJumpRating, AnncrOfficialRating, LastUpdateDate, Note " );
                    curSqlStmt.Append( ") Values ( " );
                    curSqlStmt.Append( "'" + mySanctionNum + "'" );
                    curSqlStmt.Append( ", '" + editMemberId.Text + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeAppointedCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(JudgeJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverAppointedCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(DriverJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreAppointedCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(ScoreJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetyChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetyAsstChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetyAppointedCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetySlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetyTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(SafetyJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox( TechChiefCB ) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox( TechAsstChiefCB ) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox( TechSlalomCreditCB ) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox( TechTrickCreditCB ) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox( TechJumpCreditCB ) + "'" );

                    curSqlStmt.Append( ", '" + getValueCheckBox(AnncrChiefCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(AnncrSlalomCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(AnncrTrickCreditCB) + "'" );
                    curSqlStmt.Append( ", '" + getValueCheckBox(AnncrJumpCreditCB) + "'" );

                    curSqlStmt.Append( ", '" + editJudgeSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editJudgeTrickRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editJudgeJumpRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editDriverSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editDriverTrickRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editDriverJumpRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editScorerSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editScorerTrickRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editScorerJumpRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editSafetyOfficialRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editTechControllerSlalomRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editTechControllerTrickRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editTechControllerJumpRating.Text + "'" );
                    curSqlStmt.Append( ", '" + editAnncrOfficialRating.Text + "'" );
                    curSqlStmt.Append( ", getdate()" );
                    curSqlStmt.Append( ", '" + editNote.Text + "'" );
                    curSqlStmt.Append( ")" );
					rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
					if ( rowsProc > 0 ) {
                        isDataModified = false;
                        curReturnValue = true;
                    }
                }

                if (JudgeChiefCB.Checked) {
                    isChiefJudge = true;
                    bool curUpdateComplete = checkAndUpdateChiefOfficial("Judge");
                    if ( curUpdateComplete ) {
                        isDataModified = false;
                        curReturnValue = true;
                    }
                } else {
                    if (isChiefJudge) {
                        isChiefJudge = false;
                        bool curUpdateComplete = checkAndRemoveChiefOfficial("Judge");
                        if ( curUpdateComplete ) {
                            isDataModified = false;
                            curReturnValue = true;
                        }
                    }
                }
                if (DriverChiefCB.Checked) {
                    isChiefDriver = true;
                    bool curUpdateComplete = checkAndUpdateChiefOfficial("Driver");
                    if ( curUpdateComplete ) {
                        isDataModified = false;
                        curReturnValue = true;
                    }
                } else {
                    if ( isChiefDriver) {
                        isChiefDriver = false;
                        bool curUpdateComplete = checkAndRemoveChiefOfficial("Driver");
                        if ( curUpdateComplete ) {
                            isDataModified = false;
                            curReturnValue = true;
                        }
                    }
                }
                if (ScoreChiefCB.Checked) {
                    isChiefScore = true;
                    bool curUpdateComplete = checkAndUpdateChiefOfficial("Scorer");
                    if ( curUpdateComplete ) {
                        isDataModified = false;
                        curReturnValue = true;
                    }
                } else {
                    if (isChiefScore) {
                        isChiefScore = false;
                        bool curUpdateComplete = checkAndRemoveChiefOfficial("Scorer");
                        if ( curUpdateComplete ) {
                            isDataModified = false;
                            curReturnValue = true;
                        }
                    }
                }
                if (SafetyChiefCB.Checked) {
                    isChiefSafety = true;
                    bool curUpdateComplete = checkAndUpdateChiefOfficial("Safety");
                } else {
                    if (isChiefSafety) {
                        isChiefSafety = false;
                        bool curUpdateComplete = checkAndRemoveChiefOfficial("Safety");
                        if ( curUpdateComplete ) {
                            isDataModified = false;
                            curReturnValue = true;
                        }
                    }
                }

                winStatusMsg.Text = "Changes successfully saved";

			} catch ( Exception excp ) {
                curReturnValue = false;
                String curExcptMsg = "Error attempting to update skier official credits \n" + excp.Message;
				MessageBox.Show( curExcptMsg );
				Log.WriteFile( curMethodName + curExcptMsg );
			}

			return curReturnValue;
        }

        private void navEditOfficials_Click(object sender, EventArgs e) {
            EditMemberOfficialsRatings curEditMemberOfficialsRatings;
            curEditMemberOfficialsRatings = new EditMemberOfficialsRatings();

            //Update data if changes are detected
            if (isDataModified) {
                try {
                    saveOfficialData();
                    winStatusMsg.Text = "Previous row saved.";
                } catch (Exception excp) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if (!( isDataModified )) {
                DataGridViewRow curViewRow = listTourMemberDataGridView.CurrentRow;
                //Sent current tournament registration row
                if ( HelperFunctions.isObjectPopulated( curViewRow.Cells["MemberId"].Value ) ) {
                    isDataModified = false;

                    // Display the form as a modal dialog box.
                    String curMemberId = (String)curViewRow.Cells["MemberId"].Value;
                    curEditMemberOfficialsRatings.editMember( curMemberId );
                    curEditMemberOfficialsRatings.ShowDialog( this );
                    // Determine if the OK button was clicked on the dialog box.
                    if (curEditMemberOfficialsRatings.DialogResult == DialogResult.OK) {
                        String curValue = curEditMemberOfficialsRatings.JudgeSlalomRating;
                        if (curValue.Equals( "Unrated" )) {
                            editJudgeSlalomRating.Text = "";
                            curViewRow.Cells["JudgeSlalomRating"].Value = "";
                        } else {
                            editJudgeSlalomRating.Text = curValue;
                            curViewRow.Cells["JudgeSlalomRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.JudgeTrickRating;
                        if (curValue.Equals( "Unrated" )) {
                            editJudgeTrickRating.Text = "";
                            curViewRow.Cells["JudgeTrickRating"].Value = "";
                        } else {
                            editJudgeTrickRating.Text = curValue;
                            curViewRow.Cells["JudgeTrickRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.JudgeJumpRating;
                        if (curValue.Equals( "Unrated" )) {
                            editJudgeJumpRating.Text = "";
                            curViewRow.Cells["JudgeJumpRating"].Value = "";
                        } else {
                            editJudgeJumpRating.Text = curValue;
                            curViewRow.Cells["JudgeJumpRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.ScorerSlalomRating;
                        if (curValue.Equals( "Unrated" )) {
                            editScorerSlalomRating.Text = "";
                            curViewRow.Cells["ScorerSlalomRating"].Value = "";
                        } else {
                            editScorerSlalomRating.Text = curValue;
                            curViewRow.Cells["ScorerSlalomRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.ScorerTrickRating;
                        if (curValue.Equals( "Unrated" )) {
                            editScorerTrickRating.Text = "";
                            curViewRow.Cells["ScorerTrickRating"].Value = "";
                        } else {
                            editScorerTrickRating.Text = curValue;
                            curViewRow.Cells["ScorerTrickRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.ScorerJumpRating;
                        if (curValue.Equals( "Unrated" )) {
                            editScorerJumpRating.Text = "";
                            curViewRow.Cells["ScorerJumpRating"].Value = "";
                        } else {
                            editScorerJumpRating.Text = curValue;
                            curViewRow.Cells["ScorerJumpRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.DriverSlalomRating;
                        if (curValue.Equals( "Unrated" )) {
                            editDriverSlalomRating.Text = "";
                            curViewRow.Cells["DriverSlalomRating"].Value = "";
                        } else {
                            editDriverSlalomRating.Text = curValue;
                            curViewRow.Cells["DriverSlalomRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.DriverTrickRating;
                        if (curValue.Equals( "Unrated" )) {
                            editDriverTrickRating.Text = "";
                            curViewRow.Cells["DriverTrickRating"].Value = "";
                        } else {
                            editDriverTrickRating.Text = curValue;
                            curViewRow.Cells["DriverTrickRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.DriverJumpRating;
                        if (curValue.Equals( "Unrated" )) {
                            editDriverJumpRating.Text = "";
                            curViewRow.Cells["DriverJumpRating"].Value = "";
                        } else {
                            editDriverJumpRating.Text = curValue;
                            curViewRow.Cells["DriverJumpRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.SafetyRating;
                        if (curValue.Equals( "Unrated" )) {
                            editSafetyOfficialRating.Text = "";
                            curViewRow.Cells["SafetyOfficialRating"].Value = "";
                        } else {
                            editSafetyOfficialRating.Text = curValue;
                            curViewRow.Cells["SafetyOfficialRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.TechControllerSlalomRating;
                        if (curValue.Equals( "Unrated" )) {
                            editTechControllerSlalomRating.Text = "";
                            curViewRow.Cells["TechControllerSlalomRating"].Value = "";
                        } else {
                            editTechControllerSlalomRating.Text = curValue;
                            curViewRow.Cells["TechControllerSlalomRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.TechControllerTrickRating;
                        if ( curValue.Equals( "Unrated" ) ) {
                            editTechControllerTrickRating.Text = "";
                            curViewRow.Cells["TechControllerTrickRating"].Value = "";
                        } else {
                            editTechControllerTrickRating.Text = curValue;
                            curViewRow.Cells["TechControllerTrickRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.TechControllerJumpRating;
                        if ( curValue.Equals( "Unrated" ) ) {
                            editTechControllerJumpRating.Text = "";
                            curViewRow.Cells["TechControllerJumpRating"].Value = "";
                        } else {
                            editTechControllerJumpRating.Text = curValue;
                            curViewRow.Cells["TechControllerJumpRating"].Value = curValue;
                        }
                        curValue = curEditMemberOfficialsRatings.AnncrOfficialRating;
                        if (curValue.Equals( "Unrated" )) {
                            editAnncrOfficialRating.Text = "";
                            curViewRow.Cells["AnncrOfficialRating"].Value = "";
                        } else {
                            editAnncrOfficialRating.Text = curValue;
                            curViewRow.Cells["AnncrOfficialRating"].Value = curValue;
                        }
                    }
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
                Properties.Settings.Default.OfficialWorkRecord_Sort = mySortCommand;
                navRefresh_Click( null, null );
            }
        }

        private void navFilter_Click( object sender, EventArgs e ) {
            if ( isDataModified ) { navSaveItem_Click( null, null ); }

            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                navRefresh_Click( null, null );
            }
        }

        private void ExportMemberList_Click( object sender, EventArgs e ) {
            if ( isDataModified ) { navSaveItem_Click( null, null ); }
            ExportData myExportData = new ExportData();
			myExportData.exportData( getOfficialListForExport() );
        }

        private void navExport_Click( object sender, EventArgs e ) {
            if ( isDataModified ) { navSaveItem_Click( null, null ); }

            String[] curSelectCommand = new String[4];
            String[] curTableName = { "TourReg", "OfficialWork", "OfficialWorkAsgmt", "MemberList" };
            ExportData myExportData = new ExportData();

            curSelectCommand[0] = "SELECT XT.* FROM TourReg XT "
                + "INNER JOIN OfficialWork ER on XT.SanctionId = ER.SanctionId AND XT.MemberId = ER.MemberId "
                + "Where XT.SanctionId = '" + mySanctionNum + "' ";
            if ( HelperFunctions.isObjectPopulated(myFilterCmd) ) curSelectCommand[0] = curSelectCommand[0] +" And " + myFilterCmd;

            curSelectCommand[1] = "Select * from OfficialWork "
                + "Where SanctionId = '" + mySanctionNum + "' "
                + "And LastUpdateDate is not null ";
            if ( HelperFunctions.isObjectPopulated( myFilterCmd ) ) curSelectCommand[1] = curSelectCommand[1] + " And " + myFilterCmd;

            curSelectCommand[2] = "Select * from OfficialWorkAsgmt " + " Where SanctionId = '" + mySanctionNum + "'";
            if ( HelperFunctions.isObjectPopulated(myFilterCmd) ) curSelectCommand[2] = curSelectCommand[2] + " And " + myFilterCmd;

            curSelectCommand[3] = "SELECT XT.* FROM MemberList XT "
                + "INNER JOIN TourReg ER on XT.MemberId = ER.MemberId "
                + "Where ER.SanctionId = '" + mySanctionNum + "' ";
            if (  HelperFunctions.isObjectPopulated(myFilterCmd) ) curSelectCommand[3] = curSelectCommand[3] + " And " + myFilterCmd;

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void exportCreditFile_Click( object sender, EventArgs e ) {
            ExportOfficialWorkFile myExportData = new ExportOfficialWorkFile();
            myExportData.ExportData();
            MessageBox.Show("Go to the tournament data directory to view " + mySanctionNum.Trim() + "OD.txt" + " to see current official credit assignments");
        }

        private void navColumnSelect_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            columnSelectDialogcs.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( columnSelectDialogcs.DialogResult == DialogResult.OK ) {
                DataTable curPrintColumnSelectList = columnSelectDialogcs.ColumnList;
                foreach ( DataGridViewColumn curViewCol in this.PrintDataGridView.Columns ) {
                    foreach ( DataRow curColSelect in curPrintColumnSelectList.Rows ) {
                        if ( ( (String)curColSelect["Name"] ).Equals( (String)curViewCol.Name ) ) {
                            curViewCol.Visible = (Boolean)curColSelect["Visible"];

                            if ( myPrintColumnFilter.ContainsKey( (String)curViewCol.Name ) ) {
                                myPrintColumnFilter[(String)curViewCol.Name] = curViewCol.Visible;
                            } else {
                                myPrintColumnFilter.Add( (String)curViewCol.Name, curViewCol.Visible );
                            }
                            break;
                        }
                    }
                }
                //Properties.Settings.Default.OfficialWorkRecordColumnFilter = myPrintColumnFilter;
                winStatusMsg.Text = "Selected columns to view on print";
            }
        }

        private void listTourMemberDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curDataView = (DataGridView)sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + curDataView.Rows.Count.ToString();
            if (isDataModified) { navSaveItem_Click( null, null ); }
            if ( curDataView.CurrentRow != null ) {
                if ( curDataView.Rows[e.RowIndex].Cells["MemberId"].Value != System.DBNull.Value ) {
                    setOfficalWorkRecord( curDataView.Rows[e.RowIndex] );
                }
            }
        }

        private void setOfficalWorkRecord( DataGridViewRow inTourMemberRow ) {
            String curMemberId = (String)inTourMemberRow.Cells["MemberId"].Value;
            myOfficialWorkRow = getOfficialWorkData( curMemberId );
            isChiefJudge = false;
            isChiefDriver = false;
            isChiefScore = false;
            isChiefSafety = false;

			if ( myOfficialWorkRow == null ) {
				MessageBox.Show( "OfficialWork record is not available for memeber" );
				isDataModified = false;
				return;
			}

			//Show existing data
			editMemberId.Text = (String) myOfficialWorkRow["MemberId"];
			editSanctionId.Text = (String) myOfficialWorkRow["SanctionId"];
			try {
				editNote.Text = (String) myOfficialWorkRow["Note"];
			} catch {
				editNote.Text = "";
			}

			initValueCheckBox( myOfficialWorkRow["JudgeChief"], JudgeChiefCB );
			initValueCheckBox( myOfficialWorkRow["JudgeAsstChief"], JudgeAsstChiefCB );
			initValueCheckBox( myOfficialWorkRow["JudgeAppointed"], JudgeAppointedCB );
			initValueCheckBox( myOfficialWorkRow["JudgeSlalomCredit"], JudgeSlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["JudgeTrickCredit"], JudgeTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["JudgeJumpCredit"], JudgeJumpCreditCB );

			initValueCheckBox( myOfficialWorkRow["DriverChief"], DriverChiefCB );
			initValueCheckBox( myOfficialWorkRow["DriverAsstChief"], DriverAsstChiefCB );
			initValueCheckBox( myOfficialWorkRow["DriverAppointed"], DriverAppointedCB );
			initValueCheckBox( myOfficialWorkRow["DriverSlalomCredit"], DriverSlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["DriverTrickCredit"], DriverTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["DriverJumpCredit"], DriverJumpCreditCB );

			initValueCheckBox( myOfficialWorkRow["ScoreChief"], ScoreChiefCB );
			initValueCheckBox( myOfficialWorkRow["ScoreAsstChief"], ScoreAsstChiefCB );
			initValueCheckBox( myOfficialWorkRow["ScoreAppointed"], ScoreAppointedCB );
			initValueCheckBox( myOfficialWorkRow["ScoreSlalomCredit"], ScoreSlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["ScoreTrickCredit"], ScoreTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["ScoreJumpCredit"], ScoreJumpCreditCB );

			initValueCheckBox( myOfficialWorkRow["SafetyChief"], SafetyChiefCB );
			initValueCheckBox( myOfficialWorkRow["SafetyAsstChief"], SafetyAsstChiefCB );
			initValueCheckBox( myOfficialWorkRow["SafetyAppointed"], SafetyAppointedCB );
			initValueCheckBox( myOfficialWorkRow["SafetySlalomCredit"], SafetySlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["SafetyTrickCredit"], SafetyTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["SafetyJumpCredit"], SafetyJumpCreditCB );

			initValueCheckBox( myOfficialWorkRow["TechChief"], TechChiefCB );
			initValueCheckBox( myOfficialWorkRow["TechAsstChief"], TechAsstChiefCB );
			initValueCheckBox( myOfficialWorkRow["TechSlalomCredit"], TechSlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["TechTrickCredit"], TechTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["TechJumpCredit"], TechJumpCreditCB );

			initValueCheckBox( myOfficialWorkRow["AnncrChief"], AnncrChiefCB );
			initValueCheckBox( myOfficialWorkRow["AnncrSlalomCredit"], AnncrSlalomCreditCB );
			initValueCheckBox( myOfficialWorkRow["AnncrTrickCredit"], AnncrTrickCreditCB );
			initValueCheckBox( myOfficialWorkRow["AnncrJumpCredit"], AnncrJumpCreditCB );

			if ( JudgeChiefCB.Checked ) isChiefJudge = true;
			if ( DriverChiefCB.Checked ) isChiefDriver = true;
			if ( ScoreChiefCB.Checked ) isChiefScore = true;
			if ( SafetyChiefCB.Checked ) isChiefSafety = true;


			editJudgeSlalomRating.Text = (String) inTourMemberRow.Cells["JudgeSlalomRating"].Value;
			editJudgeTrickRating.Text = (String) inTourMemberRow.Cells["JudgeTrickRating"].Value;
			editJudgeJumpRating.Text = (String) inTourMemberRow.Cells["JudgeJumpRating"].Value;
			editScorerSlalomRating.Text = (String) inTourMemberRow.Cells["ScorerSlalomRating"].Value;
			editScorerTrickRating.Text = (String) inTourMemberRow.Cells["ScorerTrickRating"].Value;
			editScorerJumpRating.Text = (String) inTourMemberRow.Cells["ScorerJumpRating"].Value;
			editDriverSlalomRating.Text = (String) inTourMemberRow.Cells["DriverSlalomRating"].Value;
			editDriverTrickRating.Text = (String) inTourMemberRow.Cells["DriverTrickRating"].Value;
			editDriverJumpRating.Text = (String) inTourMemberRow.Cells["DriverJumpRating"].Value;
			editSafetyOfficialRating.Text = (String) inTourMemberRow.Cells["SafetyOfficialRating"].Value;
			editTechControllerSlalomRating.Text = (String) inTourMemberRow.Cells["TechControllerSlalomRating"].Value;
            editTechControllerTrickRating.Text = (String)inTourMemberRow.Cells["TechControllerTrickRating"].Value;
            editTechControllerJumpRating.Text = (String)inTourMemberRow.Cells["TechControllerJumpRating"].Value;
            editAnncrOfficialRating.Text = (String) inTourMemberRow.Cells["AnncrOfficialRating"].Value;

            isDataModified = false;
        }

        private void ItemChanged( object sender, EventArgs e ) {
            //isDataModified = true;
        }

        private void ItemClick( object sender, EventArgs e ) {
            CheckBox curControl = (CheckBox)sender;
            bool isSelectValid = true;
            isDataModified = true;
            String curMemberId = (String)listTourMemberDataGridView.CurrentRow.Cells["MemberId"].Value;

            if ( curControl.Checked ) {
                if ( curControl.Name.Equals( "JudgeChiefCB" )
                    || curControl.Name.Equals( "JudgeAsstChiefCB" )
                    || curControl.Name.Equals( "JudgeAppointedCB" ) ) {
                    initValueCheckBox( "Y", JudgeSlalomCreditCB );
                    initValueCheckBox( "Y", JudgeTrickCreditCB );
                    initValueCheckBox( "Y", JudgeJumpCreditCB );
                    if ( curControl.Name.Equals( "JudgeChiefCB" ) ) {
                        String curRating = editJudgeSlalomRating.Text;
                        if (curRating == null) curRating = "";
                        if (curRating.Equals( "A" ) || curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    
                    
                    } else if ( curControl.Name.Equals( "JudgeAsstChiefCB" ) ) {
                        String curRating = editJudgeSlalomRating.Text;
                        if ( curRating == null ) curRating = "";
                        if ( curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0 ) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    }
                }
                
                if ( curControl.Name.Equals( "DriverChiefCB" )
                    || curControl.Name.Equals( "DriverAsstChiefCB" )
                    || curControl.Name.Equals( "DriverAppointedCB" ) ) {
                    initValueCheckBox( "Y", DriverSlalomCreditCB );
                    initValueCheckBox( "Y", DriverTrickCreditCB );
                    initValueCheckBox( "Y", DriverJumpCreditCB );
                    if ( curControl.Name.Equals( "DriverChiefCB" ) ) {
                        String curRating = editDriverSlalomRating.Text;
                        if (curRating == null) curRating = "";
                        if (curRating.Equals( "A" ) || curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    
                    } else if ( curControl.Name.Equals( "DriverAsstChiefCB" ) ) {
                        String curRating = editDriverSlalomRating.Text;
                        if ( curRating == null ) curRating = "";
                        if ( curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0 ) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    }
                }
                
                if ( curControl.Name.Equals( "ScoreChiefCB" )
                    || curControl.Name.Equals( "ScoreAsstChiefCB" )
                    || curControl.Name.Equals( "ScoreAppointedCB" ) ) {
                    initValueCheckBox( "Y", ScoreSlalomCreditCB );
                    initValueCheckBox( "Y", ScoreTrickCreditCB );
                    initValueCheckBox( "Y", ScoreJumpCreditCB );

                    if ( curControl.Name.Equals( "ScoreChiefCB" ) ) {
                        String curRating = editScorerSlalomRating.Text;
                        if (curRating == null) curRating = "";
                        if (curRating.Equals( "A" ) || curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    
                    } else if ( curControl.Name.Equals( "ScoreAsstChiefCB" ) ) {
                        String curRating = editScorerSlalomRating.Text;
                        if ( curRating == null ) curRating = "";
                        if ( curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0 ) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    }
                }
                
                if ( curControl.Name.Equals( "TechChiefCB" )
                    || curControl.Name.Equals( "TechAsstChiefCB" ) ) {
                    initValueCheckBox( "Y", TechSlalomCreditCB );
                    initValueCheckBox( "Y", TechTrickCreditCB );
                    initValueCheckBox( "Y", TechJumpCreditCB );

                    if ( curControl.Name.Equals( "TechChiefCB" ) ) {
                        String curRating = editTechControllerSlalomRating.Text;
                        if (curRating == null) curRating = "";
                        if (curRating.Equals( "A" ) || curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    
                    } else if ( curControl.Name.Equals( "TechAsstChiefCB" ) ) {
                        String curRating = editTechControllerSlalomRating.Text;
                        if ( curRating == null ) curRating = "";
                        if ( curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0 ) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    }
                }
                
                if ( curControl.Name.Equals( "SafetyChiefCB" )
                    || curControl.Name.Equals( "SafetyAsstChiefCB" )
                    || curControl.Name.Equals( "SafetyAppointedCB" ) ) {
                    initValueCheckBox( "Y", SafetySlalomCreditCB );
                    initValueCheckBox( "Y", SafetyTrickCreditCB );
                    initValueCheckBox( "Y", SafetyJumpCreditCB );
                    if ( curControl.Name.Equals( "SafetyChiefCB" ) ) {
                        String curRating = editSafetyOfficialRating.Text;
                        if (curRating == null) curRating = "";
                        if (curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    
                    } else if ( curControl.Name.Equals( "SafetyAsstChiefCB" ) ) {
                        String curRating = editSafetyOfficialRating.Text;
                        if ( curRating == null ) curRating = "";
                        if ( curRating.Equals( "" ) || curRating.Equals( " " ) || curRating.Length == 0 ) {
                            MessageBox.Show( " Warning: EVP approval should be obtained if actual ratings is insufficient for position." );
                        }
                    }
                }
                
                if ( curControl.Name.Equals( "AnncrChiefCB" ) ) {
                    initValueCheckBox( "Y", AnncrSlalomCreditCB );
                    initValueCheckBox( "Y", AnncrTrickCreditCB );
                    initValueCheckBox( "Y", AnncrJumpCreditCB );
                }
            }

            if ( isSelectValid ) {
                SendKeys.Send( "{TAB}" );
            }
        }

        private String getValueCheckBox( CheckBox inCheckBox ) {
            try {
                if ( inCheckBox.Checked ) {
                    return "Y";
                } else {
                    return "N";
                }
            } catch {
                return "N";
            }
        }

        private void initValueCheckBox( Object inValue, CheckBox inCheckBox ) {
            try {
                String curValue = (String)inValue;
                initValueCheckBox( curValue, inCheckBox );
            } catch {
                initValueCheckBox( "N", inCheckBox );
            }
        }
        private void initValueCheckBox( String inValue, CheckBox inCheckBox ) {
            String refControlName = inCheckBox.Name.Substring( 0, inCheckBox.Name.Length - 2 );
            if ( HelperFunctions.isObjectEmpty( inValue ) ) {
                if ( myOfficialWorkRow != null ) {
                    myOfficialWorkRow[refControlName] = "N";
                }
                inCheckBox.Checked = false;
            
            } else {
                if ( myOfficialWorkRow != null ) {
                    myOfficialWorkRow[refControlName] = inValue;
                }
                if ( inValue.Equals( "Y" ) ) {
                    inCheckBox.Checked = true;
                } else {
                    inCheckBox.Checked = false;
                }
            }
        }

        private void checkBoxYNFormat( Object sender, ConvertEventArgs e ) {
            if ( e.Value == System.DBNull.Value ) {
                e.Value = false;
            } else {
                if ( (String)e.Value == "Y" ) {
                    e.Value = true;
                } else {
                    e.Value = false;
                }
            }
        }

        private void checkBoxYNParse( Object sender, ConvertEventArgs e ) {
            if ( (bool)e.Value == true ) {
                e.Value = "Y";
            } else {
                e.Value = "N";
            }
        }

        private void listTourMemberDataGridView_CellFormatting( object sender, DataGridViewCellFormattingEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Columns[e.ColumnIndex].Name.Equals( "JudgeSlalomRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "JudgeTrickRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "JudgeJumpRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "ScorerSlalomRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "ScorerTrickRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "ScorerJumpRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "DriverSlalomRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "DriverTrickRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "DriverJumpRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "SafetyOfficialRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "TechControllerSlalomRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "TechControllerTrickRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "TechControllerJumpRating" )
                || curView.Columns[e.ColumnIndex].Name.Equals( "AnncrOfficialRating" )
                ) {
                if ( e.Value == System.DBNull.Value ) {
                    e.Value = "";
                }
                if ( e.Value.Equals( "Unrated" ) ) {
                    e.Value = "";
                }
            }
        }

        private void listTourMemberDataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {
        }

        private void loadPrintDataGrid() {
            //Load data for print data grid
            int curPrintIdx = 0;
            DataGridViewRow curPrintRow;


            try {
                DataTable tempDataTable = getOfficialListForExport();
                if ( tempDataTable == null || tempDataTable.Rows.Count == 0 ) return;
                
                if ( HelperFunctions.isObjectPopulated( mySortCommand ) ) {
                    String tempSortCmd = mySortCommand.Replace( "Print", "" );
                    tempDataTable.DefaultView.Sort = tempSortCmd;
                }
                if ( HelperFunctions.isObjectPopulated( myFilterCmd ) ) {
                    String tempFilterCmd = myFilterCmd.Replace( "Print", "" );
                    tempDataTable.DefaultView.RowFilter = tempFilterCmd;
                }
                DataTable curDataTable = tempDataTable.DefaultView.ToTable();

                winStatusMsg.Text = "Print officials";
                Cursor.Current = Cursors.WaitCursor;

                PrintDataGridView.Rows.Clear();

                foreach ( DataRow curDataRow in curDataTable.Rows ) {
                    curPrintIdx = PrintDataGridView.Rows.Add();
                    curPrintRow = PrintDataGridView.Rows[curPrintIdx];

                    curPrintRow.Cells["PrintSanctionId"].Value = (String)curDataRow["SanctionId"];
                    curPrintRow.Cells["PrintMemberId"].Value = (String)curDataRow["MemberId"];
                    curPrintRow.Cells["PrintSkierName"].Value = (String)curDataRow["SkierName"];
                    curPrintRow.Cells["PrintReadyToSki"].Value = (String)curDataRow["ReadyToSki"];

                    curPrintRow.Cells["PrintFederation"].Value = getRatingAbrev(HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ));
                    curPrintRow.Cells["PrintAgeGroup"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" ));
                    curPrintRow.Cells["PrintSlalomEventGroup"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "SlalomEventGroup", "" ));
                    curPrintRow.Cells["PrintTrickEventGroup"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "TrickEventGroup", "" ));
                    curPrintRow.Cells["PrintJumpEventGroup"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "JumpEventGroup", "" ));

                    curPrintRow.Cells["PrintJudgeSlalomRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "JudgeSlalomRating", "" ));
                    curPrintRow.Cells["PrintJudgeTrickRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "JudgeTrickRating", "" ));
                    curPrintRow.Cells["PrintJudgeJumpRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "JudgeJumpRating", "" ));
                    curPrintRow.Cells["PrintDriverSlalomRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "DriverSlalomRating", "" ));
                    curPrintRow.Cells["PrintDriverTrickRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "DriverTrickRating", "" ));
                    curPrintRow.Cells["PrintDriverJumpRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "DriverJumpRating", "" ));
                    curPrintRow.Cells["PrintScorerSlalomRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "ScorerSlalomRating", "" ));
                    curPrintRow.Cells["PrintScorerTrickRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "ScorerTrickRating", "" ));
                    curPrintRow.Cells["PrintScorerJumpRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "ScorerJumpRating", "" ));
                    curPrintRow.Cells["PrintSafetyOfficialRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "SafetyOfficialRating", "" ));
                    curPrintRow.Cells["PrintTechControllerSlalomRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "TechControllerSlalomRating", "" ));
                    curPrintRow.Cells["PrintTechControllerTrickRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "TechControllerTrickRating", "" ) );
                    curPrintRow.Cells["PrintTechControllerJumpRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "TechControllerJumpRating", "" ) );
                    curPrintRow.Cells["PrintAnncrOfficialRating"].Value = getRatingAbrev( HelperFunctions.getDataRowColValue( curDataRow, "AnncrOfficialRating", "" ));
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament entries \n" + ex.Message );
            }

            Cursor.Current = Cursors.Default;
        }

        private string getRatingAbrev( String inValue ) {
            if ( inValue.Equals( "Assistant" ) ) return "Asst";
            if ( inValue.Equals( "Regular" ) ) return "Reg";
            if ( inValue.Equals( "Regional" ) ) return "Reg";
            if ( inValue.Equals( "Coordinator" ) ) return "Coor";
            if ( inValue.Equals( "Jr Coordinator" ) ) return "Jr Coor";
            if ( inValue.Equals( "Emeritus" ) ) return "Emert";
            return inValue;
        }

        private void printButton_Click( object sender, EventArgs e ) {
            Timer curTimerObj = new Timer();
            curTimerObj.Interval = 5;
            curTimerObj.Tick += new EventHandler( printReportTimer );
            curTimerObj.Start();
        }

        private void printReportTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( printReportTimer );
            printReport();
        }
        private bool printReport() {
            //PrintDataGridView.Visible = true;
            loadPrintDataGrid();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 12, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            PrintDialog curPrintDialog = HelperPrintFunctions.getPrintSettings();
            if ( curPrintDialog == null ) return false;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            StringBuilder printTitle = new StringBuilder( Properties.Settings.Default.Mdi_Title );
            printTitle.Append( "\n Sanction " + mySanctionNum );
            printTitle.Append( "held on " + myTourRow["EventDates"].ToString() );
            printTitle.Append( "\n" + this.Text );

            myPrintDoc = new PrintDocument();
            myPrintDoc.DocumentName = this.Text;
            myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
            myPrintDoc.DefaultPageSettings.Landscape = true;
            myPrintDataGrid = new DataGridViewPrinter( PrintDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle.ToString(), fontPrintTitle, Color.DarkBlue, WithPaging );

            myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
            myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
            
            myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            curPreviewDialog.Document = myPrintDoc;
			curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
			curPreviewDialog.Focus();
            curPreviewDialog.ShowDialog();

            //PrintDataGridView.Visible = false;
            return true;
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private bool checkForOfficial( String inOfficial, String inMemberId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId FROM OfficialWork " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND MemberId <> '" + inMemberId + "' " );
            curSqlStmt.Append( "  AND " + inOfficial + " = 'Y' " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return true;
            } else {
                return false;
            }
        }
        
        private DataRow getOfficialWorkData( String inMemberId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT distinct O.SanctionId, O.MemberId, T.SkierName, T.ReadyToSki" );
            curSqlStmt.Append( ", O.JudgeChief, O.JudgeAsstChief, O.JudgeAppointed" );
            curSqlStmt.Append( ", O.DriverChief, O.DriverAsstChief, O.DriverAppointed" );
            curSqlStmt.Append( ", O.ScoreChief, O.ScoreAsstChief, O.ScoreAppointed" );
            curSqlStmt.Append( ", O.SafetyChief, O.SafetyAsstChief, O.SafetyAppointed" );
            curSqlStmt.Append( ", O.TechChief, O.TechAsstChief" );

			curSqlStmt.Append( ", O.JudgeSlalomCredit, O.JudgeTrickCredit, O.JudgeJumpCredit" );
            curSqlStmt.Append( ", O.DriverSlalomCredit, O.DriverTrickCredit, O.DriverJumpCredit" );
            curSqlStmt.Append( ", O.ScoreSlalomCredit, O.ScoreTrickCredit, O.ScoreJumpCredit" );
            curSqlStmt.Append( ", O.SafetySlalomCredit, O.SafetyTrickCredit, O.SafetyJumpCredit" );
            curSqlStmt.Append( ", O.TechSlalomCredit, O.TechTrickCredit, O.TechJumpCredit" );
            curSqlStmt.Append( ", O.AnncrTrickCredit, O.AnncrJumpCredit, O.AnncrChief, O.AnncrSlalomCredit" );

			curSqlStmt.Append( ", Coalesce( O.JudgeSlalomRating, ' ' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeTrickRating, ' ' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeJumpRating, ' ' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerSlalomRating, ' ' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerTrickRating, ' ' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerJumpRating, ' ' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverSlalomRating, ' ' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverTrickRating, ' ' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverJumpRating, ' ' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.SafetyOfficialRating, ' ' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( O.TechControllerSlalomRating, ' ' ) as TechControllerSlalomRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerTrickRating, ' ' ) as TechControllerTrickRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerJumpRating, ' ' ) as TechControllerJumpRating" );
            curSqlStmt.Append( ", Coalesce( O.AnncrOfficialRating, ' ' ) as AnncrOfficialRating " );

			curSqlStmt.Append( ", O.Note " );

			curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "	INNER JOIN TourReg T ON T.MemberId = O.MemberId AND T.SanctionId = O.SanctionId " );
            curSqlStmt.Append( "WHERE O.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND O.MemberId = '" + inMemberId + "' " );
            curSqlStmt.Append( "ORDER BY T.SkierName, O.MemberId " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                return curDataTable.Rows[0];
            } else {
                return null;
            }
        }

        private DataTable getTourMemberList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT distinct TR.SanctionId, TR.MemberId, TR.SkierName, TR.ReadyToSki, ML.Federation" );
			curSqlStmt.Append( ", Coalesce( O.JudgeSlalomRating, ' ' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeTrickRating, ' ' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeJumpRating, ' ' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerSlalomRating, ' ' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerTrickRating, ' ' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerJumpRating, ' ' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverSlalomRating, ' ' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverTrickRating, ' ' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverJumpRating, ' ' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.SafetyOfficialRating, ' ' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( O.TechControllerSlalomRating, ' ' ) as TechControllerSlalomRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerTrickRating, ' ' ) as TechControllerTrickRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerJumpRating, ' ' ) as TechControllerJumpRating" );
            curSqlStmt.Append( ", Coalesce( O.AnncrOfficialRating, ' ' ) as AnncrOfficialRating " );
            curSqlStmt.Append( "FROM OfficialWork O " );
            curSqlStmt.Append( "     INNER JOIN TourReg TR ON TR.MemberId = O.MemberId AND TR.SanctionId = O.SanctionId " );
            curSqlStmt.Append( "     LEFT OUTER JOIN MemberList ML ON ML.MemberId = O.MemberId " );
            curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND TR.ReadyToSki = 'Y' AND (TR.Withdrawn is null OR TR.WithDrawn = 'N') " );
            curSqlStmt.Append( "ORDER BY TR.SkierName, TR.MemberId  " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        /*
         * Retrieve selected tournament attributes
         */
        private bool configWindowAttributes() {
            if ( mySanctionNum == null || mySanctionNum.Length < 6 ) return false;
            DataTable curTourDataTable = getTourData();
            if ( curTourDataTable == null || curTourDataTable.Rows.Count == 0 ) return false;

            myTourRow = curTourDataTable.Rows[0];
            myTourRules = (String)myTourRow["Rules"];

            decimal curRoundsSlalom = HelperFunctions.getDataRowColValueDecimal( myTourRow, "SlalomRounds", 0 );
            decimal curRoundsTrick = HelperFunctions.getDataRowColValueDecimal( myTourRow, "TrickRounds", 0 );
            decimal curRoundsJump = HelperFunctions.getDataRowColValueDecimal( myTourRow, "JumpRounds", 0 );

            /*
             * Hide checkboxes not used because event is active for this tournament
             */
            if ( curRoundsSlalom == 0 ) {
                JudgeSlalomCreditCB.Visible = false;
                JudgeSlalomCreditCB.Enabled = false;
                editJudgeSlalomRating.Visible = false;
                editJudgeSlalomRating.Enabled = false;
                JudgeSlalomRating.Visible = false;
                PrintSlalomEventGroup.Visible = false;

                DriverSlalomCreditCB.Visible = false;
                DriverSlalomCreditCB.Enabled = false;
                DriverSlalomRating.Visible = false;
                editDriverSlalomRating.Visible = false;
                editDriverSlalomRating.Enabled = false;

                ScoreSlalomCreditCB.Visible = false;
                ScoreSlalomCreditCB.Enabled = false;
                ScorerSlalomRating.Visible = false;
                editScorerSlalomRating.Visible = false;
                editScorerSlalomRating.Enabled = false;

                SafetySlalomCreditCB.Visible = false;
                SafetySlalomCreditCB.Enabled = false;

                TechSlalomCreditCB.Visible = false;
                TechSlalomCreditCB.Enabled = false;

                AnncrSlalomCreditCB.Visible = false;
                AnncrSlalomCreditCB.Enabled = false;
            }

            if ( curRoundsTrick == 0 ) {
                JudgeTrickCreditCB.Visible = false;
                JudgeTrickCreditCB.Enabled = false;
                editJudgeTrickRating.Visible = false;
                editJudgeTrickRating.Enabled = false;
                JudgeTrickRating.Visible = false;
                PrintJudgeTrickRating.Visible = false;
                PrintTrickEventGroup.Visible = false;

                DriverTrickCreditCB.Visible = false;
                DriverTrickRating.Visible = false;
                PrintDriverTrickRating.Visible = false;
                editDriverTrickRating.Visible = false;
                editDriverTrickRating.Enabled = false;
                DriverTrickCreditCB.Enabled = false;

                ScoreTrickCreditCB.Visible = false;
                ScoreTrickCreditCB.Enabled = false;
                editScorerTrickRating.Visible = false;
                editScorerTrickRating.Enabled = false;
                ScorerTrickRating.Visible = false;
                PrintScorerTrickRating.Visible = false;

                SafetyTrickCreditCB.Visible = false;
                SafetyTrickCreditCB.Enabled = false;

                TechTrickCreditCB.Visible = false;
                TechTrickCreditCB.Enabled = false;

                AnncrTrickCreditCB.Visible = false;
                AnncrTrickCreditCB.Enabled = false;

            }
            if ( curRoundsJump == 0 ) {
                JudgeJumpCreditCB.Visible = false;
                JudgeJumpCreditCB.Enabled = false;
                JudgeJumpRating.Visible = false;
                PrintJudgeJumpRating.Visible = false;
                editJudgeJumpRating.Visible = false;
                editJudgeJumpRating.Enabled = false;
                PrintJumpEventGroup.Visible = false;

                DriverJumpCreditCB.Visible = false;
                DriverJumpCreditCB.Enabled = false;
                DriverJumpRating.Visible = false;
                PrintDriverJumpRating.Visible = false;
                editDriverJumpRating.Visible = false;
                editDriverJumpRating.Enabled = false;

                ScoreJumpCreditCB.Visible = false;
                ScoreJumpCreditCB.Enabled = false;
                ScorerJumpRating.Visible = false;
                PrintScorerJumpRating.Visible = false;
                editScorerJumpRating.Visible = false;
                editScorerJumpRating.Enabled = false;

                SafetyJumpCreditCB.Visible = false;
                SafetyJumpCreditCB.Enabled = false;

                TechJumpCreditCB.Visible = false;
                TechJumpCreditCB.Enabled = false;

                AnncrJumpCreditCB.Visible = false;
                AnncrJumpCreditCB.Enabled = false;

            }

            return true;
		}

		private DataTable getOfficialListForExport() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT distinct TR.SanctionId, TR.MemberId, TR.SkierName, ML.Federation, TR.ReadyToSki" );
			curSqlStmt.Append( ", TR.AgeGroup, ERS.EventGroup as SlalomEventGroup, ERT.EventGroup as TrickEventGroup, ERJ.EventGroup as JumpEventGroup" );
			curSqlStmt.Append( ", Coalesce( O.JudgeSlalomRating, ' ' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeTrickRating, ' ' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.JudgeJumpRating, ' ' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerSlalomRating, ' ' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerTrickRating, ' ' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.ScorerJumpRating, ' ' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverSlalomRating, ' ' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverTrickRating, ' ' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( O.DriverJumpRating, ' ' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( O.SafetyOfficialRating, ' ' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( O.TechControllerSlalomRating, ' ' ) as TechControllerSlalomRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerTrickRating, ' ' ) as TechControllerTrickRating" );
            curSqlStmt.Append( ", Coalesce( O.TechControllerJumpRating, ' ' ) as TechControllerJumpRating" );
            curSqlStmt.Append( ", Coalesce( O.AnncrOfficialRating, ' ' ) as AnncrOfficialRating " );
			curSqlStmt.Append( "FROM OfficialWork O " );
			curSqlStmt.Append( "     INNER JOIN TourReg TR ON TR.MemberId = O.MemberId AND TR.SanctionId = O.SanctionId " );
			curSqlStmt.Append( "     LEFT OUTER JOIN EventReg ERS ON ERS.MemberId = O.MemberId AND ERS.SanctionId = O.SanctionId AND ERS.AgeGroup = TR.AgeGroup and ERS.Event = 'Slalom' " );
			curSqlStmt.Append( "     LEFT OUTER JOIN EventReg ERT ON ERT.MemberId = O.MemberId AND ERT.SanctionId = O.SanctionId AND ERT.AgeGroup = TR.AgeGroup and ERT.Event = 'Trick' " );
			curSqlStmt.Append( "     LEFT OUTER JOIN EventReg ERJ ON ERJ.MemberId = O.MemberId AND ERJ.SanctionId = O.SanctionId AND ERJ.AgeGroup = TR.AgeGroup and ERJ.Event = 'Jump' " );
			curSqlStmt.Append( "     LEFT OUTER JOIN MemberList ML ON ML.MemberId = O.MemberId " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND TR.ReadyToSki = 'Y' AND (TR.Withdrawn is null OR TR.WithDrawn = 'N') " );
            curSqlStmt.Append( "ORDER BY TR.SkierName, TR.MemberId  " );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
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

        private Boolean checkAndRemoveChiefOfficial( String inChiefType ) {
			String curMethodName = "OfficialWorkRecord: checkAndRemoveChiefOfficial: ";
			StringBuilder curSqlStmt = new StringBuilder("");
            String curChiefOfficalAttrName = "Chief" + inChiefType + "MemberId";
            if ( inChiefType.Equals("Safety") ) {
                curChiefOfficalAttrName = "SafetyDirMemberId";
            }

            try {
                isChiefJudge = false;
                curSqlStmt = new StringBuilder("Update Tournament Set " + curChiefOfficalAttrName + " = ''");
                curSqlStmt.Append(", LastUpdateDate = getdate() ");
                curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "'");
                curSqlStmt.Append(" AND " + curChiefOfficalAttrName + " = '" + editMemberId.Text + "' ");
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
				winStatusMsg.Text = "Removal of chief judge successfully saved";
                return true;
            
            } catch ( Exception excp ) {
				String curExcptMsg = "Error removing chief judge \n" + excp.Message;
				MessageBox.Show( curExcptMsg );
				Log.WriteFile( curMethodName + curExcptMsg );
				return false;
            }
        }

        private Boolean checkAndUpdateChiefOfficial(String inChiefType) {
			String curMethodName = "OfficialWorkRecord: checkAndUpdateChiefOfficial: ";
			StringBuilder curSqlStmt = new StringBuilder("");
            String curChiefOfficalAttrName = "Chief" + inChiefType + "MemberId";
            if ( inChiefType.Equals("Safety" ) ) {
                curChiefOfficalAttrName = "SafetyDirMemberId";
            }

            try {
                curSqlStmt.Append("Select " + curChiefOfficalAttrName + " as ChiefMemberId From Tournament ");
                curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "' ");
                DataTable curDataTable = DataAccess.getDataTable(curSqlStmt.ToString());
                bool isUpdateReqd = false;
                if ( curDataTable.Rows.Count > 0 ) {
                    if ( curDataTable.Rows[0]["ChiefMemberId"] == System.DBNull.Value ) {
                        isUpdateReqd = true;
                    } else {
                        String curMemberId = (String) curDataTable.Rows[0]["ChiefMemberId"];
                        if ( curMemberId.Trim().Length == 0 ) {
                            isUpdateReqd = true;
                        } else if ( curMemberId.Equals(editMemberId.Text) ) {
                            isUpdateReqd = false;
                        } else {
                            String dialogMsg = "Primary chief " + inChiefType + " is already assigned, do you want to change that assignment?";
                            DialogResult msgResp =
                                MessageBox.Show(dialogMsg, "Change Warning",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1);
                            if ( msgResp == DialogResult.Yes ) {
                                isUpdateReqd = true;
                            } else {
                                isUpdateReqd = false;
                            }
                        }
                    }
                }

                if ( isUpdateReqd ) {
                    curSqlStmt = new StringBuilder("");
                    curSqlStmt.Append("Update Tournament Set ");
                    curSqlStmt.Append(curChiefOfficalAttrName + " = '" + editMemberId.Text + "' ");
                    curSqlStmt.Append(", LastUpdateDate = getdate() ");
                    curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "' ");
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
					if ( rowsProc > 0 ) {
                        MessageBox.Show("Remember to update the contact information for this new chief official");
                        return true;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }

            } catch ( Exception excp ) {
				String curExcptMsg = "Error updating " + inChiefType + "\n" + excp.Message;
				MessageBox.Show( curExcptMsg );
				Log.WriteFile( curMethodName + curExcptMsg );
				return false;
            }
        }

        private void getRunningOrderColumnfilter() {
            DataRowView newRow;
            myPrintColumnFilter = new Dictionary<string, Boolean>();
            DataTable curPrintColumnSelectList = HelperPrintFunctions.buildPrintColumnList();

            foreach ( DataGridViewColumn curCol in this.PrintDataGridView.Columns ) {
                newRow = curPrintColumnSelectList.DefaultView.AddNew();
                newRow["Name"] = curCol.Name;
                if ( myPrintColumnFilter.ContainsKey( curCol.Name ) ) {
                    newRow["Visible"] = (bool)myPrintColumnFilter[curCol.Name];

                } else {
                    if ( myPrintColumnFilter.ContainsKey( curCol.Name ) ) {
                        newRow["Visible"] = (bool)myPrintColumnFilter[curCol.Name];
                    } else {
                        newRow["Visible"] = curCol.Visible;
                    }
                }
                newRow.EndEdit();
            }
            columnSelectDialogcs = new ColumnSelectDialogcs();
            columnSelectDialogcs.ColumnList = curPrintColumnSelectList;

            curPrintColumnSelectList = columnSelectDialogcs.ColumnList;
            foreach ( DataGridViewColumn curViewCol in this.PrintDataGridView.Columns ) {
                foreach ( DataRow curColSelect in curPrintColumnSelectList.Rows ) {
                    if ( ( (String)curColSelect["Name"] ).Equals( (String)curViewCol.Name ) ) {
                        curViewCol.Visible = (Boolean)curColSelect["Visible"];

                        if ( myPrintColumnFilter.ContainsKey( (String)curViewCol.Name ) ) {
                            myPrintColumnFilter[(String)curViewCol.Name] = curViewCol.Visible;
                        } else {
                            myPrintColumnFilter.Add( (String)curViewCol.Name, curViewCol.Visible );
                        }
                        break;
                    }
                }
            }
        }

    }
}
