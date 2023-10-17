using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace WaterskiScoringSystem.Admin {
    public partial class TourChiefOfficialContact : Form {
        private String mySanctionNum = null;
        private Boolean isDataModified = false;
        private String myOrigItemValue;
        private DataTable myDataTable;

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

        private void loadButton_Click( object sender, EventArgs e ) {
            myDataTable = getTourData( mySanctionNum );
            if ( myDataTable == null || myDataTable.Rows.Count == 0 ) {
                MessageBox.Show( "Chief official references not found" );
                return;
            }
            DataRow curRow = myDataTable.Rows[0];
            
            String curSanctionEditCode = HelperFunctions.getDataRowColValue( curRow, "SanctionEditCode", "" );
            String curContactMemberId = HelperFunctions.getDataRowColValue( curRow, "ContactMemberId", "" );
            String curChiefJudgeMemberId = HelperFunctions.getDataRowColValue( curRow, "ChiefJudgeMemberId", "" );
            String curChiefDriverMemberId = HelperFunctions.getDataRowColValue( curRow, "ChiefDriverMemberId", "" );
            String curChiefScorerMemberId = HelperFunctions.getDataRowColValue( curRow, "ChiefScorerMemberId", "" );
            String curSafetyDirMemberId = HelperFunctions.getDataRowColValue( curRow, "SafetyDirMemberId", "" );

            Cursor.Current = Cursors.WaitCursor;
            //Dictionary<string, object> curContactData = getSanctionFromUSAWS();
            List<object> curResponseDataList = getChiefOfficialContacts( curSanctionEditCode, curContactMemberId, curChiefJudgeMemberId, curChiefDriverMemberId, curChiefScorerMemberId, curSafetyDirMemberId );

            if ( curResponseDataList == null || curResponseDataList.Count == 0 ) {
                MessageBox.Show( "Chief official contact information was not found on the AWSA database" );
                return;
            }
            String curMemberId;
            foreach ( Dictionary<string, object> curEntry in curResponseDataList ) {
                curMemberId = HelperFunctions.getAttributeValue( curEntry, "MemberId" );
                if ( curMemberId.Equals( curContactMemberId ) ) {
                    contactAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" ) 
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Address2" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                    contactPhoneTextBox.Text = formatPhone( HelperFunctions.getAttributeValue( curEntry, "Phone" ) )
                        + ", " + formatPhone( HelperFunctions.getAttributeValue( curEntry, "MobilePhone" ) );
                    contactEmailTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Email" );
                } 
                if ( curMemberId.Equals( curChiefJudgeMemberId ) ) {
                    /*
                    chiefJudgeNameTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "FirstName" ) + " " + HelperFunctions.getAttributeValue( curEntry, "LastName" );
                    chiefJudgeAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Address2" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                     */
                    chiefJudgeAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                    chiefJudgePhoneTextBox.Text = formatPhone( HelperFunctions.getAttributeValue( curEntry, "Phone" ) )
                        + ", " + formatPhone( HelperFunctions.getAttributeValue( curEntry, "MobilePhone" ) );
                    chiefJudgeEmailTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Email" );
                } 
                if ( curMemberId.Equals( curChiefDriverMemberId ) ) {
                    /*
                    chiefDriverNameTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "FirstName" ) + " " + HelperFunctions.getAttributeValue( curEntry, "LastName" );
                    chiefDriverAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Address2" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                     */
                    chiefDriverAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                    chiefDriverPhoneTextBox.Text = formatPhone( HelperFunctions.getAttributeValue( curEntry, "Phone" ) )
                        + ", " + formatPhone( HelperFunctions.getAttributeValue( curEntry, "MobilePhone" ) );
                    chiefDriverEmailTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Email" );
                } 
                if ( curMemberId.Equals( curChiefScorerMemberId ) ) {
                    /*
                    chiefScorerNameTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "FirstName" ) + " " + HelperFunctions.getAttributeValue( curEntry, "LastName" );
                    chiefScorerAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Address2" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                     */
                    chiefScorerAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                    chiefScorerPhoneTextBox.Text = formatPhone( HelperFunctions.getAttributeValue( curEntry, "Phone" ) )
                        + ", " + formatPhone( HelperFunctions.getAttributeValue( curEntry, "MobilePhone" ) );
                    chiefScorerEmailTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Email" );
                } 
                if ( curMemberId.Equals( curSafetyDirMemberId ) ) {
                    /*
                    chiefSafetyNameTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "FirstName" ) + " " + HelperFunctions.getAttributeValue( curEntry, "LastName" );
                    safetyDirAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Address1" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Address2" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                     */
                    safetyDirAddressTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "City" )
                        + ", " + HelperFunctions.getAttributeValue( curEntry, "State" )
                        + " " + HelperFunctions.getAttributeValue( curEntry, "Zip" );
                    safetyDirPhoneTextBox.Text = formatPhone( HelperFunctions.getAttributeValue( curEntry, "Phone" ) )
                        + ", " + formatPhone( HelperFunctions.getAttributeValue( curEntry, "MobilePhone" ) );
                    safetyDirEmailTextBox.Text = HelperFunctions.getAttributeValue( curEntry, "Email" );
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private String formatPhone( String inPhone ) {
            String curTempValue;
            if ( HelperFunctions.isObjectEmpty( inPhone ) ) return inPhone;

            if ( inPhone.Substring(0, 2).Equals("+1")) {
                curTempValue = inPhone.Substring( 2 ).Trim();
                if ( curTempValue.Length == 10 ) {
                    return "(" + curTempValue.Substring( 0, 3 ) + ") " + curTempValue.Substring( 3, 3 ) + "-" + curTempValue.Substring( 6 );
                } else {
                    return curTempValue;
                }
            
            } else {
                return inPhone;
            }
        }

        private bool saveContactData() {
            bool curReturnValue = false;
            int rowsProc = 0;

            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append("Update Tournament Set");
                curSqlStmt.Append(" ContactMemberId = '" + contactNameSelect.SelectedValue + "'");
                curSqlStmt.Append( ", ContactAddress = '" + HelperFunctions.escapeString(contactAddressTextBox.Text) + "'" );
                curSqlStmt.Append(", ContactPhone = '" + contactPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ContactEmail = '" + HelperFunctions.escapeString(contactEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefJudgeAddress = '" + HelperFunctions.escapeString(chiefJudgeAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefJudgePhone = '" + chiefJudgePhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefJudgeEmail = '" + HelperFunctions.escapeString(chiefJudgeEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefDriverAddress = '" + HelperFunctions.escapeString(chiefDriverAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefDriverPhone = '" + chiefDriverPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefDriverEmail = '" + HelperFunctions.escapeString(chiefDriverEmailTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefScorerAddress = '" + HelperFunctions.escapeString(chiefScorerAddressTextBox.Text) + "'");
                curSqlStmt.Append(", ChiefScorerPhone = '" + chiefScorerPhoneTextBox.Text + "'");
                curSqlStmt.Append(", ChiefScorerEmail = '" + HelperFunctions.escapeString(chiefScorerEmailTextBox.Text) + "'");
                curSqlStmt.Append(", SafetyDirAddress = '" + HelperFunctions.escapeString(safetyDirAddressTextBox.Text) + "'");
                curSqlStmt.Append(", SafetyDirPhone = '" + safetyDirPhoneTextBox.Text + "'");
                curSqlStmt.Append( ", SafetyDirEmail = '" + HelperFunctions.escapeString( safetyDirEmailTextBox.Text ) + "'" );
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
                        contactNameSelect.SelectedValue = HelperFunctions.getDataRowColValue( curRow, "ContactMemberId", "" );
                        contactAddressTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ContactAddress", "" );
                        contactPhoneTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ContactPhone", "" );
                        contactEmailTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ContactEmail", "" );

                        chiefJudgeNameTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefJudgeName", "" );
                        chiefJudgeAddressTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefJudgeAddress", "" );
                        chiefJudgePhoneTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefJudgePhone", "" );
                        chiefJudgeEmailTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefJudgeEmail", "" );

                        chiefDriverNameTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefDriverName", "" );
                        chiefDriverAddressTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefDriverAddress", "" );
                        chiefDriverPhoneTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefDriverPhone", "" );
                        chiefDriverEmailTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefDriverEmail", "" );

                        chiefScorerNameTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefScorerName", "" );
                        chiefScorerAddressTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefScorerAddress", "" );
                        chiefScorerPhoneTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefScorerPhone", "" );
                        chiefScorerEmailTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefScorerEmail", "" );

                        chiefSafetyNameTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "ChiefSafetyName", "" );
                        safetyDirAddressTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "SafetyDirAddress", "" );
                        safetyDirPhoneTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "SafetyDirPhone", "" );
                        safetyDirEmailTextBox.Text = HelperFunctions.getDataRowColValue( curRow, "SafetyDirEmail", "" );
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

        private List<object> getChiefOfficialContacts( String curSanctionEditCode, String curContactMemberId, String curChiefJudgeMemberId, String curChiefDriverMemberId, String curChiefScorerMemberId, String curSafetyDirMemberId ) {
            NameValueCollection curHeaderParams = new NameValueCollection();

            /* -----------------------------------------------------------------------
            * Configure URL to retrieve all skiers pre-registered for the active tournament
			* This will include all appointed officials
            ----------------------------------------------------------------------- */
            String curQueryString = String.Format( "?ContactMemberId={0}&CJMemberId={1}&CDMemberId={2}&CCMemberId={3}&CSMemberId={4}"
                , curContactMemberId, curChiefJudgeMemberId, curChiefDriverMemberId, curChiefScorerMemberId, curSafetyDirMemberId );
            String curContentType = "application/json; charset=UTF-8";
            String curExportListUrl = Properties.Settings.Default.UriUsaWaterski + "/admin/GetChiefOfficalContactExportJson.asp";
            String curReqstUrl = curExportListUrl + curQueryString;

            List<object> curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
            if ( curResponseDataList != null && curResponseDataList.Count > 0 ) return curResponseDataList;

            MessageBox.Show( "Contact information could not be retrieved for this tournaments chief officials " );
            return null;
        }

        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct T.SanctionId, T.Name, T.Class, T.Federation, T.TourDataLoc, SanctionEditCode, T.LastUpdateDate" );
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
					if ( String.IsNullOrWhiteSpace( emailAddress ) ) return;
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
