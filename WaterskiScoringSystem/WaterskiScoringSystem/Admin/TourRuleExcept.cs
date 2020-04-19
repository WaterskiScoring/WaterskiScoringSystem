using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Admin {
    public partial class TourRuleExcept : Form {
        private String mySanctionNum = null;
        private Boolean isDataModified = false;
        private String myOrigItemValue;
        private DataTable myDataTable;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public TourRuleExcept() {
            InitializeComponent();
        }

        public String SanctionNum {
            get { return mySanctionNum; }
            set { mySanctionNum = value; }
        }

        public void TourRuleExcept_Show( String inSanctionNum ) {
            //Retrieve tournament list and set current position to active tournament
            myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
            myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
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

            try {
                myDbConn.Open();
                mySqlStmt = myDbConn.CreateCommand();
                mySqlStmt.CommandText = "";

                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update Tournament Set " );
                curSqlStmt.Append( "LastUpdateDate = getdate() " );
                curSqlStmt.Append( ",RuleExceptions = '" + escapeString(ruleExceptionsTextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleExceptQ1 = '" + escapeString(ruleExceptQ1TextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleExceptQ2 = '" + escapeString(ruleExceptQ2TextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleExceptQ3 = '" + ruleExceptQ3TextBox.Text + "'" );
                curSqlStmt.Append( ",RuleExceptQ4 = '" + ruleExceptQ4TextBox.Text + "'" );
                curSqlStmt.Append( ",RuleInterpretations = '" + escapeString(ruleInterpretationsTextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleInterQ1 = '" + escapeString(ruleInterQ1TextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleInterQ2 = '" + escapeString(ruleInterQ2TextBox.Text) + "'" );
                curSqlStmt.Append( ",RuleInterQ3 = '" + ruleInterQ3TextBox.Text + "'" );
                curSqlStmt.Append( ",RuleInterQ4 = '" + ruleInterQ4TextBox.Text + "'" );
                curSqlStmt.Append( ",SafetyDirPerfReport = '" + escapeString(safetyDirPerfReportTextBox.Text) + "'" );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "'" );

                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();
                if (rowsProc > 0) {
                    isDataModified = false;
                    curReturnValue = true;
                }
                winStatusMsg.Text = "Changes successfully saved";

            } catch (Exception excp) {
                curReturnValue = false;
                MessageBox.Show( "Error attempting to update tournament data \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }
            return curReturnValue;
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            try {
                winStatusMsg.Text = "Retrieving Tournament data";
                Cursor.Current = Cursors.WaitCursor;
                myDataTable = getTourData();
                if (myDataTable != null) {
                    if (myDataTable.Rows.Count > 0) {
                        String curValue = "";
                        DataRow curRow = myDataTable.Rows[0];

                        if (isObjectEmpty( curRow["RuleExceptions"] )) {
                            ruleExceptionsTextBox.Text = "";
                        } else {
                            ruleExceptionsTextBox.Text = (String)curRow["RuleExceptions"];
                        }
                        if (isObjectEmpty( curRow["RuleExceptQ1"] )) {
                            ruleExceptQ1TextBox.Text = "";
                        } else {
                            ruleExceptQ1TextBox.Text = (String)curRow["RuleExceptQ1"];
                        }
                        if (isObjectEmpty( curRow["RuleExceptQ2"] )) {
                            ruleExceptQ2TextBox.Text = "";
                        } else {
                            ruleExceptQ2TextBox.Text = (String)curRow["RuleExceptQ2"];
                        }
                        if (isObjectEmpty( curRow["RuleExceptQ3"] )) {
                            curValue = "";
                        } else {
                            curValue = (String)curRow["RuleExceptQ3"];
                        }
                        ruleExceptQ3TextBox.Text = curValue;
                        if (curValue.Equals( "Y" )) {
                            RuleExceptQ3Yes.Checked = true;
                        } else if (curValue.Equals( "N" )) {
                            RuleExceptQ3No.Checked = true;
                        } else if (curValue.Equals( "A" )) {
                            RuleExceptQ3NA.Checked = true;
                        } else {
                            RuleExceptQ3NA.Checked = true;
                        }
                        if (isObjectEmpty( curRow["RuleExceptQ4"] )) {
                            curValue = "";
                        } else {
                            curValue = (String)curRow["RuleExceptQ4"];
                        }
                        ruleExceptQ4TextBox.Text = curValue;
                        if (curValue.Equals( "Y" )) {
                            RuleExceptQ4Yes.Checked = true;
                        } else if (curValue.Equals( "N" )) {
                            RuleExceptQ4No.Checked = true;
                        } else if (curValue.Equals( "A" )) {
                            RuleExceptQ4NA.Checked = true;
                        } else {
                            RuleExceptQ4NA.Checked = true;
                        }

                        if (isObjectEmpty( curRow["RuleInterpretations"] )) {
                            ruleInterpretationsTextBox.Text = "";
                        } else {
                            ruleInterpretationsTextBox.Text = (String)curRow["RuleInterpretations"];
                        }
                        if (isObjectEmpty( curRow["RuleInterQ1"] )) {
                            ruleInterQ1TextBox.Text = "";
                        } else {
                            ruleInterQ1TextBox.Text = (String)curRow["RuleInterQ1"];
                        }

                        if (isObjectEmpty( curRow["RuleInterQ2"] )) {
                            ruleInterQ2TextBox.Text = "";
                        } else {
                            ruleInterQ2TextBox.Text = (String)curRow["RuleInterQ2"];
                        }
                        if (isObjectEmpty( curRow["RuleInterQ3"] )) {
                            curValue = "";
                        } else {
                            curValue = (String)curRow["RuleInterQ3"];
                        }
                        ruleInterQ3TextBox.Text = curValue;
                        if (curValue.Equals( "Y" )) {
                            RuleInterQ3Yes.Checked = true;
                        } else if (curValue.Equals( "N" )) {
                            RuleInterQ3No.Checked = true;
                        } else if (curValue.Equals( "A" )) {
                            RuleInterQ3NA.Checked = true;
                        } else {
                            RuleInterQ3NA.Checked = true;
                        }

                        if (isObjectEmpty( curRow["RuleInterQ4"] )) {
                            curValue = "";
                        } else {
                            curValue = (String)curRow["RuleInterQ4"];
                        }
                        ruleInterQ4TextBox.Text = curValue;
                        if (curValue.Equals( "Y" )) {
                            RuleInterQ4Yes.Checked = true;
                        } else if (curValue.Equals( "N" )) {
                            RuleInterQ4No.Checked = true;
                        } else if (curValue.Equals( "A" )) {
                            RuleInterQ4NA.Checked = true;
                        } else {
                            RuleInterQ4NA.Checked = true;
                        }

                        if (isObjectEmpty( curRow["ChiefSafetyName"] )) {
                            SafetyDirNameTextBox.Text = "";
                        } else {
                            SafetyDirNameTextBox.Text = (String)curRow["ChiefSafetyName"];
                        }
                        if (isObjectEmpty( curRow["SafetyDirPerfReport"] )) {
                            safetyDirPerfReportTextBox.Text = "";
                        } else {
                            safetyDirPerfReportTextBox.Text = (String)curRow["SafetyDirPerfReport"];
                        }
                    
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
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class" );
            curSqlStmt.Append( "  LEFT OUTER JOIN TourReg AS TourRegCS ON T.SanctionId = TourRegCS.SanctionId AND T.SafetyDirMemberId = TourRegCS.MemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty(object inObject) {
            bool curReturnValue = false;
            if (inObject == null) {
                curReturnValue = true;
            } else if (inObject == System.DBNull.Value) {
                curReturnValue = true;
            } else if (inObject.ToString().Length > 0) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

        private String escapeString(String inValue) {
            String curReturnValue = "";
            char[] singleQuoteDelim = new char[] { '\'' };
            //char[] doubleQuoteDelim = new char[] { '"' };
            curReturnValue = stringReplace( inValue, singleQuoteDelim, "''" );
            //curReturnValue = stringReplace( curValue1, doubleQuoteDelim, "\\\"" );
            return curReturnValue;
        }
        private String stringReplace(String inValue, char[] inCurValue, String inReplValue) {
            StringBuilder curNewValue = new StringBuilder( "" );

            String[] curValues = inValue.Split( inCurValue );
            if (curValues.Length > 1) {
                int curCount = 0;
                foreach (String curValue in curValues) {
                    curCount++;
                    if (curCount < curValues.Length) {
                        curNewValue.Append( curValue + inReplValue );
                    } else {
                        curNewValue.Append( curValue );
                    }
                }
            } else {
                curNewValue.Append( inValue );
            }

            return curNewValue.ToString();
        }
    }
}
