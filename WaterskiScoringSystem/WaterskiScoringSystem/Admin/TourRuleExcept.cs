using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;

namespace WaterskiScoringSystem.Admin {
    public partial class TourRuleExcept : Form {
        private String mySanctionNum = null;
        private Boolean isDataModified = false;
        private String myOrigItemValue;
        private DataTable myDataTable;

        public TourRuleExcept() {
            InitializeComponent();
        }

        public String SanctionNum {
            get { return mySanctionNum; }
            set { mySanctionNum = value; }
        }

        public void TourRuleExcept_Show( String inSanctionNum ) {
            //Retrieve tournament list and set current position to active tournament
            mySanctionNum = inSanctionNum;
            navRefresh_Click( null, null );
        }

        private void TourRuleExcept_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TourRuleExcept_Width > 0 ) {
                this.Width = Properties.Settings.Default.TourRuleExcept_Width;
            }
            if ( Properties.Settings.Default.TourRuleExcept_Height > 0 ) {
                this.Height = Properties.Settings.Default.TourRuleExcept_Height;
            }
            if ( Properties.Settings.Default.TourRuleExcept_Location.X > 0
                && Properties.Settings.Default.TourRuleExcept_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourRuleExcept_Location;
            }
        }

        private void TourRuleExcept_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void TourRuleExcept_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TourRuleExcept_Width = this.Size.Width;
                Properties.Settings.Default.TourRuleExcept_Height = this.Size.Height;
                Properties.Settings.Default.TourRuleExcept_Location = this.Location;
            }
        }

        private void bindingSource_DataError( object sender, BindingManagerDataErrorEventArgs e ) {
            MessageBox.Show( "tournamentBindingSource_DataError"
                + "\n sender:" + sender.GetType()
                + "\n EventArgs:" + e.GetType()
            );
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void navSaveItem_Click(object sender, EventArgs e) {
            try {
                if (isDataModified) {
                    saveData();
                }
                if (!isDataModified) {
                    this.Close();
                }
            } catch (Exception excp) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveData() {
            bool curReturnValue = false;
            int rowsProc = 0;

            StringBuilder curSqlStmt = new StringBuilder( String.Format( "Select SanctionId From ChiefJudgeReport Where SanctionId = '{0}'", mySanctionNum ) );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable != null && curDataTable.Rows.Count > 0 ) {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update ChiefJudgeReport Set " );
                curSqlStmt.Append( "LastUpdateDate = getdate() " );
                curSqlStmt.Append( ", RuleExceptions = '" + HelperFunctions.escapeString( ruleExceptionsTextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleExceptQ1 = '" + HelperFunctions.escapeString( ruleExceptQ1TextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleExceptQ2 = '" + HelperFunctions.escapeString( ruleExceptQ2TextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleExceptQ3 = '" + ruleExceptQ3TextBox.Text + "'" );
                curSqlStmt.Append( ", RuleExceptQ4 = '" + ruleExceptQ4TextBox.Text + "'" );
                curSqlStmt.Append( ", RuleInterpretations = '" + HelperFunctions.escapeString( ruleInterpretationsTextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleInterQ1 = '" + HelperFunctions.escapeString( ruleInterQ1TextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleInterQ2 = '" + HelperFunctions.escapeString( ruleInterQ2TextBox.Text ) + "'" );
                curSqlStmt.Append( ", RuleInterQ3 = '" + ruleInterQ3TextBox.Text + "'" );
                curSqlStmt.Append( ", RuleInterQ4 = '" + ruleInterQ4TextBox.Text + "'" );
                curSqlStmt.Append( ", SafetyDirPerfReport = '" + HelperFunctions.escapeString( safetyDirPerfReportTextBox.Text ) + "'" );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "'" );
            
            } else {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Insert INTO ChiefJudgeReport( " );
                curSqlStmt.Append( "SanctionId, LastUpdateDate" );
                curSqlStmt.Append( ", RuleExceptions, RuleExceptQ1, RuleExceptQ2, RuleExceptQ3, RuleExceptQ4" );
                curSqlStmt.Append( ", RuleInterpretations, RuleInterQ1, RuleInterQ2, RuleInterQ3, RuleInterQ4" );
                curSqlStmt.Append( ", SafetyDirPerfReport" );
                curSqlStmt.Append( ", RopeHandlesSpecs, SlalomRopesSpecs, JumpRopesSpecs, SlalomCourseSpecs, JumpCourseSpecs, TrickCourseSpecs, BuoySpecs" );

                curSqlStmt.Append( ") Values (" );
                curSqlStmt.Append( String.Format( "'{0}', GetDate()", mySanctionNum ) );
                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleExceptionsTextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleExceptQ1TextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleExceptQ2TextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", ruleExceptQ3TextBox.Text ) );
                curSqlStmt.Append( String.Format( ", '{0}'", ruleExceptQ4TextBox.Text ) );

                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleInterpretationsTextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleInterQ1TextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( ruleInterQ2TextBox.Text ) ) );
                curSqlStmt.Append( String.Format( ", '{0}'", ruleInterQ3TextBox.Text ) );
                curSqlStmt.Append( String.Format( ", '{0}'", ruleInterQ4TextBox.Text ) );

                curSqlStmt.Append( String.Format( ", '{0}'", HelperFunctions.escapeString( safetyDirPerfReportTextBox.Text ) ) );
                curSqlStmt.Append( ", '', '', '', '', '', '', '' )" );
            }

            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
            if ( rowsProc > 0 ) {
                isDataModified = false;
                curReturnValue = true;
            }
            winStatusMsg.Text = "Changes successfully saved";

            return curReturnValue;
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            try {
                winStatusMsg.Text = "Retrieving Tournament data";
                Cursor.Current = Cursors.WaitCursor;
                myDataTable = getTourData();
                if (myDataTable != null) {
                    if (myDataTable.Rows.Count > 0) {
                        DataRow curRow = myDataTable.Rows[0];

                        ruleExceptionsTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleExceptions", "");
                        ruleExceptQ1TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleExceptQ1", "" );
                        ruleExceptQ2TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleExceptQ2", "" );
                        ruleExceptQ3TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleExceptQ3", "" );
                        if ( ruleExceptQ3TextBox.Text.Equals( "Y" )) {
                            RuleExceptQ3Yes.Checked = true;
                        } else if ( ruleExceptQ3TextBox.Text.Equals( "N" )) {
                            RuleExceptQ3No.Checked = true;
                        } else if ( ruleExceptQ3TextBox.Text.Equals( "A" )) {
                            RuleExceptQ3NA.Checked = true;
                        } else {
                            RuleExceptQ3NA.Checked = true;
                        }
                        ruleExceptQ4TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleExceptQ4", "" );
                        if ( ruleExceptQ4TextBox.Text.Equals( "Y" )) {
                            RuleExceptQ4Yes.Checked = true;
                        } else if ( ruleExceptQ4TextBox.Text.Equals( "N" )) {
                            RuleExceptQ4No.Checked = true;
                        } else if ( ruleExceptQ4TextBox.Text.Equals( "A" )) {
                            RuleExceptQ4NA.Checked = true;
                        } else {
                            RuleExceptQ4NA.Checked = true;
                        }

                        ruleInterpretationsTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleInterpretations", "" );
                        ruleInterQ1TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleInterQ1", "" );
                        ruleInterQ2TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleInterQ2", "" );

                        ruleInterQ3TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleInterQ3", "" );
                        if ( ruleInterQ3TextBox.Text.Equals( "Y" )) {
                            RuleInterQ3Yes.Checked = true;
                        } else if ( ruleInterQ3TextBox.Text.Equals( "N" )) {
                            RuleInterQ3No.Checked = true;
                        } else if ( ruleInterQ3TextBox.Text.Equals( "A" )) {
                            RuleInterQ3NA.Checked = true;
                        } else {
                            RuleInterQ3NA.Checked = true;
                        }

                        ruleInterQ4TextBox.Text = HelperFunctions.getDataRowColValue( curRow, "RuleInterQ4", "" );
                        if ( ruleInterQ4TextBox.Text.Equals( "Y" )) {
                            RuleInterQ4Yes.Checked = true;
                        } else if ( ruleInterQ4TextBox.Text.Equals( "N" )) {
                            RuleInterQ4No.Checked = true;
                        } else if ( ruleInterQ4TextBox.Text.Equals( "A" )) {
                            RuleInterQ4NA.Checked = true;
                        } else {
                            RuleInterQ4NA.Checked = true;
                        }

                        SafetyDirNameTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefSafetyName", "" );
                        safetyDirPerfReportTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "SafetyDirPerfReport", "" );
                    
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve tournament members\n" + ex.Message );
            }
        }

        private void EnterItemTextBox( object sender, EventArgs e ) {
            myOrigItemValue = ( (TextBox)sender ).Text;
        }

        private void ItemTextChanged( object sender, EventArgs e ) {
            if ( !( ( (TextBox)sender ).Text.Equals( myOrigItemValue ) ) ) {
                isDataModified = true;
            }
        }

        private void RuleExceptQ3Yes_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ3TextBox.Text = "Y";
                isDataModified = true;
            }
        }

        private void RuleExceptQ3No_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ3TextBox.Text = "N";
                isDataModified = true;
            }
        }

        private void RuleExceptQ3NA_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ3TextBox.Text = "A";
                isDataModified = true;
            }
        }

        private void RuleExceptQ4Yes_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ4TextBox.Text = "Y";
                isDataModified = true;
            }
        }

        private void RuleExceptQ4No_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ4TextBox.Text = "N";
                isDataModified = true;
            }
        }

        private void RuleExceptQ4NA_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleExceptQ4TextBox.Text = "A";
                isDataModified = true;
            }
        }

        private void RuleInterQ3Yes_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ3TextBox.Text = "Y";
                isDataModified = true;
            }
        }

        private void RuleInterQ3No_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ3TextBox.Text = "N";
                isDataModified = true;
            }
        }

        private void RuleInterQ3NA_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ3TextBox.Text = "A";
                isDataModified = true;
            }
        }

        private void RuleInterQ4Yes_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ4TextBox.Text = "Y";
                isDataModified = true;
            }
        }

        private void RuleInterQ4No_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ4TextBox.Text = "N";
                isDataModified = true;
            }
        }

        private void RuleInterQ4NA_CheckedChanged( object sender, EventArgs e ) {
            RadioButton curControl = (RadioButton)sender;
            if ( curControl.Checked ) {
                ruleInterQ4TextBox.Text = "A";
                isDataModified = true;
            }
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", RuleExceptions, RuleExceptQ1, RuleExceptQ2, RuleExceptQ3, RuleExceptQ4" );
            curSqlStmt.Append( ", RuleInterpretations, RuleInterQ1, RuleInterQ2, RuleInterQ3, RuleInterQ4" );
            curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirPerfReport, TourRegCS.SkierName AS ChiefSafetyName " );
            curSqlStmt.Append( "FROM Tournament T" );
            curSqlStmt.Append( "  LEFT OUTER JOIN ChiefJudgeReport C ON C.SanctionId = T.SanctionId" );
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class" );
            curSqlStmt.Append( "  LEFT OUTER JOIN TourReg AS TourRegCS ON T.SanctionId = TourRegCS.SanctionId AND T.SafetyDirMemberId = TourRegCS.MemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
