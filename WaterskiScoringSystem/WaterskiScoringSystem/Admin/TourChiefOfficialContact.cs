using System;
using System.Collections;
using System.Data;
using System.Data.SqlServerCe;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Admin {
    public partial class TourChiefOfficialContact : Form {
        private String mySanctionNum = null;
        private Boolean isDataModified = false;
        private String myOrigItemValue;
        private DataTable myDataTable;
        private SqlCeCommand mySqlStmt = null;

        public TourChiefOfficialContact() {
            InitializeComponent();
        }

        public String SanctionNum {
            get { return mySanctionNum; }
            set { mySanctionNum = value; }
        }

        public void TourChiefOfficialContact_Show( String inSanctionNum ) {
            //Retrieve tournament list and set current position to active tournament
            mySanctionNum = inSanctionNum;
            checkChiefOfficial();
            navRefresh_Click( null, null );
        }

        private void TourChiefOfficialContact_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TourChiefOfficial_Width > 0 ) {
                this.Width = Properties.Settings.Default.TourChiefOfficial_Width;
            }
            if ( Properties.Settings.Default.TourChiefOfficial_Height > 0 ) {
                this.Height = Properties.Settings.Default.TourChiefOfficial_Height;
            }
            if ( Properties.Settings.Default.TourChiefOfficial_Location.X > 0
                && Properties.Settings.Default.TourChiefOfficial_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourChiefOfficial_Location;
            }

        }

        private void TourChiefOfficialContact_FormClosing( object sender, FormClosingEventArgs e ) {
        }

        private void TourChiefOfficialContact_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TourChiefOfficial_Width = this.Size.Width;
                Properties.Settings.Default.TourChiefOfficial_Height = this.Size.Height;
                Properties.Settings.Default.TourChiefOfficial_Location = this.Location;
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
                    saveContactData();
                }
                if (!isDataModified) {
                    this.Close();
                }

            } catch (Exception excp) {
                MessageBox.Show("Error attempting to save changes \n" + excp.Message);
            }
        }

        private bool saveContactData() {
            bool curReturnValue = false;
            int rowsProc = 0;

            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append("Update Tournament Set");
                curSqlStmt.Append(" ContactMemberId = '" + contactNameSelect.SelectedValue + "'");
                curSqlStmt.Append( ", ContactAddress = '" + escapeString(contactAddressTextBox.Text) + "'" );
                curSqlStmt.Append(", ContactPhone = '" + contactPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ContactEmail = '" + escapeString(contactEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefJudgeAddress = '" + escapeString(chiefJudgeAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefJudgePhone = '" + chiefJudgePhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefJudgeEmail = '" + escapeString(chiefJudgeEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefDriverAddress = '" + escapeString(chiefDriverAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefDriverPhone = '" + chiefDriverPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefDriverEmail = '" + escapeString(chiefDriverEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefScorerAddress = '" + escapeString(chiefScorerAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefScorerPhone = '" + chiefScorerPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefScorerEmail = '" + escapeString(chiefScorerEmailTextBox.Text) + "'");
                curSqlStmt.Append(", SafetyDirAddress = '" + escapeString(safetyDirAddressTextBox.Text) + "'");
                curSqlStmt.Append(", SafetyDirPhone = '" + safetyDirPhoneTextBox.Text + "'");
                curSqlStmt.Append( ", SafetyDirEmail = '" + escapeString( safetyDirEmailTextBox.Text ) + "'" );
                curSqlStmt.Append(", LastUpdateDate = getdate() ");
                curSqlStmt.Append("Where SanctionId = '" + mySanctionNum + "'");
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                if (rowsProc > 0) {
                    isDataModified = false;
                    curReturnValue = true;
                }
                winStatusMsg.Text = "Changes successfully saved";

            } catch (Exception excp) {
                curReturnValue = false;
                MessageBox.Show("Error attempting to update official contact data \n" + excp.Message);
            } finally {
            }
            return curReturnValue;
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            ArrayList curDropdownList = new ArrayList();
            DataTable curTourMemberList;

            try {
                winStatusMsg.Text = "Retrieving Tournament data";
                Cursor.Current = Cursors.WaitCursor;
                myDataTable = getTourData(mySanctionNum);
                this.chiefJudgeNameTextBox.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                    winStatusMsg.Text = "Tournament list retrieved";
                } );

                if (myDataTable != null) {
                    if (myDataTable.Rows.Count > 0) {

                        curTourMemberList = getTourRegList();
                        String curListCode, curCodeValue;
                        if (curTourMemberList.Rows.Count > 0) {
                            foreach (DataRow curListRow in curTourMemberList.Rows) {
                                curListCode = curListRow["MemberId"].ToString();
                                curCodeValue = curListRow["SkierName"].ToString() + "(" + curListRow["AgeGroup"].ToString() + ")";
                                curDropdownList.Add( new ListItem( curCodeValue, curListCode ) );
                            }
                            contactNameSelect.DataSource = curDropdownList;
                            contactNameSelect.DisplayMember = "ItemName";
                            contactNameSelect.ValueMember = "ItemValue";
                        }

                        DataRow curRow = myDataTable.Rows[0];
                        try {
                            contactNameSelect.SelectedValue = (String)curRow["ContactMemberId"];
                        } catch {
                            contactNameSelect.SelectedValue = "";
                        }
                        try {
                            contactAddressTextBox.Text = (String)curRow["ContactAddress"];
                        } catch {
                            contactAddressTextBox.Text = "";
                        }
                        try {
                            contactPhoneTextBox.Text = (String)curRow["ContactPhone"];
                        } catch {
                            contactPhoneTextBox.Text = "";
                        }
                        try {
                            contactEmailTextBox.Text = (String)curRow["ContactEmail"];
                        } catch {
                            contactEmailTextBox.Text = "";
                        }
                        try {
                            chiefJudgeNameTextBox.Text = (String)curRow["ChiefJudgeName"];
                        } catch {
                            chiefJudgeNameTextBox.Text = "";
                        }
                        try {
                            chiefJudgeAddressTextBox.Text = (String)curRow["ChiefJudgeAddress"];
                        } catch {
                            chiefJudgeAddressTextBox.Text = "";
                        }
                        try {
                            chiefJudgePhoneTextBox.Text = (String)curRow["ChiefJudgePhone"];
                        } catch {
                            chiefJudgePhoneTextBox.Text = "";
                        }
                        try {
                            chiefJudgeEmailTextBox.Text = (String)curRow["ChiefJudgeEmail"];
                        } catch {
                            chiefJudgeEmailTextBox.Text = "";
                        }
                        try {
                            chiefDriverNameTextBox.Text = (String)curRow["ChiefDriverName"];
                        } catch {
                            chiefDriverNameTextBox.Text = "";
                        }
                        try {
                            chiefDriverAddressTextBox.Text = (String)curRow["ChiefDriverAddress"];
                        } catch {
                            chiefDriverAddressTextBox.Text = "";
                        }
                        try {
                            chiefDriverPhoneTextBox.Text = (String)curRow["ChiefDriverPhone"];
                        } catch {
                            chiefDriverPhoneTextBox.Text = "";
                        }
                        try {
                            chiefDriverEmailTextBox.Text = (String)curRow["ChiefDriverEmail"];
                        } catch {
                            chiefDriverEmailTextBox.Text = "";
                        }
                        try {
                            chiefScorerNameTextBox.Text = (String)curRow["ChiefScorerName"];
                        } catch {
                            chiefScorerNameTextBox.Text = "";
                        }
                        try {
                            chiefScorerAddressTextBox.Text = (String)curRow["ChiefScorerAddress"];
                        } catch {
                            chiefScorerAddressTextBox.Text = "";
                        }
                        try {
                            chiefScorerPhoneTextBox.Text = (String)curRow["ChiefScorerPhone"];
                        } catch {
                            chiefScorerPhoneTextBox.Text = "";
                        }
                        try {
                            chiefScorerEmailTextBox.Text = (String)curRow["ChiefScorerEmail"];
                        } catch {
                            chiefScorerEmailTextBox.Text = "";
                        }
                        try {
                            chiefSafetyNameTextBox.Text = (String)curRow["ChiefSafetyName"];
                        } catch {
                            chiefSafetyNameTextBox.Text = "";
                        }
                        try {
                            safetyDirAddressTextBox.Text = (String)curRow["SafetyDirAddress"];
                        } catch {
                            safetyDirAddressTextBox.Text = "";
                        }
                        try {
                            safetyDirPhoneTextBox.Text = (String)curRow["SafetyDirPhone"];
                        } catch {
                            safetyDirPhoneTextBox.Text = "";
                        }
                        try {
                            safetyDirEmailTextBox.Text = (String)curRow["SafetyDirEmail"];
                        } catch {
                            safetyDirEmailTextBox.Text = "";
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

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct T.SanctionId, T.Name, T.Class, T.Federation, T.TourDataLoc, T.LastUpdateDate" );
            curSqlStmt.Append( ", T.SlalomRounds, T.TrickRounds, T.JumpRounds, T.Rules, T.EventDates, T.EventLocation" );
            curSqlStmt.Append( ", T.HcapSlalomBase, T.HcapTrickBase, T.HcapJumpBase, T.HcapSlalomPct, T.HcapTrickPct, T.HcapJumpPct " );
            curSqlStmt.Append( ", T.RopeHandlesSpecs, T.SlalomRopesSpecs, T.JumpRopesSpecs, T.SlalomCourseSpecs, T.JumpCourseSpecs, T.TrickCourseSpecs, T.BuoySpecs" );
            curSqlStmt.Append( ", T.SafetyDirPerfReport, T.RuleExceptions, T.RuleExceptQ1, T.RuleExceptQ2, T.RuleExceptQ3, T.RuleExceptQ4" );
            curSqlStmt.Append( ", T.RuleInterpretations, T.RuleInterQ1, T.RuleInterQ2, T.RuleInterQ3, T.RuleInterQ4" );
            curSqlStmt.Append( ", T.ContactMemberId, TourRegCO.SkierName AS ContactName, T.ContactPhone, T.ContactEmail, T.ContactAddress" );
            curSqlStmt.Append( ", T.ChiefJudgeMemberId, TourRegCJ.SkierName AS ChiefJudgeName, T.ChiefJudgeAddress, T.ChiefJudgePhone, T.ChiefJudgeEmail" );
            curSqlStmt.Append( ", T.ChiefDriverMemberId, TourRegCD.SkierName AS ChiefDriverName, T.ChiefDriverAddress, T.ChiefDriverPhone, T.ChiefDriverEmail" );
            curSqlStmt.Append( ", T.SafetyDirMemberId, TourRegCS.SkierName AS ChiefSafetyName, T.SafetyDirAddress, T.SafetyDirPhone, T.SafetyDirEmail" );
            curSqlStmt.Append( ", T.ChiefScorerMemberId, TourRegCC.SkierName AS ChiefScorerName, T.ChiefScorerAddress, T.ChiefScorerPhone, T.ChiefScorerEmail " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append("	    LEFT OUTER JOIN TourReg AS TourRegCC ON T.SanctionId = TourRegCC.SanctionId AND T.ChiefScorerMemberId = TourRegCC.MemberId ");
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCJ ON T.SanctionId = TourRegCJ.SanctionId AND T.ChiefJudgeMemberId = TourRegCJ.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCD ON T.SanctionId = TourRegCD.SanctionId AND T.ChiefDriverMemberId = TourRegCD.MemberId " );
            curSqlStmt.Append( "	LEFT OUTER JOIN TourReg AS TourRegCS ON T.SanctionId = TourRegCS.SanctionId AND T.SafetyDirMemberId = TourRegCS.MemberId " );
            curSqlStmt.Append("	    LEFT OUTER JOIN TourReg AS TourRegCO ON T.SanctionId = TourRegCO.SanctionId AND T.ContactMemberId = TourRegCO.MemberId ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + inSanctionId + "' ");
            curSqlStmt.Append( "ORDER BY T.SanctionId " );
            return getData( curSqlStmt.ToString() );
        }

        private void checkChiefOfficial() {
            String curMethodName = "Admin:TourChiefOfficialContact:checkChiefOfficial";
            String curMemberId = "";
            DataTable curDataTable, curOfficialDataTable;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
                curSqlStmt = new StringBuilder( "SELECT ChiefJudgeMemberId, ChiefDriverMemberId, ChiefScorerMemberId, SafetyDirMemberId FROM Tournament " );
                curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable.Rows.Count > 0) {
                    if (curDataTable.Rows[0]["ChiefJudgeMemberId"] == System.DBNull.Value) {
                        curMemberId = "";
                    } else {
                        curMemberId = (String)curDataTable.Rows[0]["ChiefJudgeMemberId"];
                    }
                    if (curMemberId.Length < 1) {
                        curSqlStmt = new StringBuilder( "SELECT MemberId, JudgeChief FROM OfficialWork " );
                        curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' And JudgeChief = 'Y'" );
                        curOfficialDataTable = getData( curSqlStmt.ToString() );
                        if (curOfficialDataTable.Rows.Count > 0) {
                            curMemberId = (String)curOfficialDataTable.Rows[0]["MemberId"];
                            curSqlStmt = new StringBuilder( "Update Tournament Set ChiefJudgeMemberId = '" + curMemberId + "' WHERE SanctionId = '" + mySanctionNum + "' " );
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        }
                    }

                    if (curDataTable.Rows[0]["ChiefDriverMemberId"] == System.DBNull.Value) {
                        curMemberId = "";
                    } else {
                        curMemberId = (String)curDataTable.Rows[0]["ChiefDriverMemberId"];
                    }
                    if (curMemberId.Length < 1) {
                        curSqlStmt = new StringBuilder( "SELECT MemberId, DriverChief FROM OfficialWork " );
                        curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' And DriverChief = 'Y'" );
                        curOfficialDataTable = getData( curSqlStmt.ToString() );
                        if (curOfficialDataTable.Rows.Count > 0) {
                            curMemberId = (String)curOfficialDataTable.Rows[0]["MemberId"];
                            curSqlStmt = new StringBuilder( "Update Tournament Set ChiefDriverMemberId = '" + curMemberId + "' WHERE SanctionId = '" + mySanctionNum + "' " );
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        }
                    }

                    if (curDataTable.Rows[0]["ChiefScorerMemberId"] == System.DBNull.Value) {
                        curMemberId = "";
                    } else {
                        curMemberId = (String)curDataTable.Rows[0]["ChiefScorerMemberId"];
                    }
                    if (curMemberId.Length < 1) {
                        curSqlStmt = new StringBuilder( "SELECT MemberId, ScoreChief FROM OfficialWork " );
                        curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' And ScoreChief = 'Y'" );
                        curOfficialDataTable = getData( curSqlStmt.ToString() );
                        if (curOfficialDataTable.Rows.Count > 0) {
                            curMemberId = (String)curOfficialDataTable.Rows[0]["MemberId"];
                            curSqlStmt = new StringBuilder( "Update Tournament Set ChiefScorerMemberId = '" + curMemberId + "' WHERE SanctionId = '" + mySanctionNum + "' " );
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        }
                    }

                    if (curDataTable.Rows[0]["SafetyDirMemberId"] == System.DBNull.Value) {
                        curMemberId = "";
                    } else {
                        curMemberId = (String)curDataTable.Rows[0]["SafetyDirMemberId"];
                    }
                    if (curMemberId.Length < 1) {
                        curSqlStmt = new StringBuilder( "SELECT MemberId, SafetyChief FROM OfficialWork " );
                        curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' And SafetyChief = 'Y'" );
                        curOfficialDataTable = getData( curSqlStmt.ToString() );
                        if (curOfficialDataTable.Rows.Count > 0) {
                            curMemberId = (String)curOfficialDataTable.Rows[0]["MemberId"];
                            curSqlStmt = new StringBuilder( "Update Tournament Set SafetyDirMemberId = '" + curMemberId + "' WHERE SanctionId = '" + mySanctionNum + "' " );
                            int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        }
                    }
                }

            } catch (Exception ex) {
                MessageBox.Show( curMethodName + ":" + ex.Message );
            }
        }

        private DataTable getTourRegList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT Distinct MemberId, SkierName, AgeGroup ");
            curSqlStmt.Append("FROM TourReg ");
            curSqlStmt.Append("WHERE SanctionId = '" + mySanctionNum + "' ");
            curSqlStmt.Append( "Order by SkierName, AgeGroup" );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
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

        private String escapeString(String inValue) {
            String curReturnValue = "";
            char[] singleQuoteDelim = new char[] { '\'' };
            curReturnValue = stringReplace( inValue, singleQuoteDelim, "''" );
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

		private void textItem_Validated( object sender, EventArgs e ) {
			if ( !( ( (TextBox) sender ).Text.Equals( myOrigItemValue ) ) ) {
				isDataModified = true;
			}
		}

		private void textItem_Validating( object sender, System.ComponentModel.CancelEventArgs e ) {
			if ( ( (TextBox) sender ).Name == "contactEmailTextBox"
				|| ( (TextBox) sender ).Name == "chiefJudgeEmailTextBox"
				|| ( (TextBox) sender ).Name == "chiefDriverEmailTextBox"
				|| ( (TextBox) sender ).Name == "chiefScorerEmailTextBox"
				|| ( (TextBox) sender ).Name == "safetyDirEmailTextBox"
				) {
				String emailAddress = ( (TextBox) sender ).Text.Trim();
				if ( IsValidEmail( emailAddress ) ) {
					( (TextBox) sender ).Text = emailAddress;
				} else {
					e.Cancel = true;
					MessageBox.Show( "Email Address is not a valid email" );
				}
			}
		}

		private bool IsValidEmail( String email ) {
			if ( String.IsNullOrWhiteSpace( email ) ) return false;

			return Regex.IsMatch( email
				, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z"
				, RegexOptions.IgnoreCase );
		}

		private void contactNameSelect_SelectedIndexChanged( object sender, EventArgs e ) {
			isDataModified = true;
		}
	}
}
