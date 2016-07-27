using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Admin;

namespace WaterskiScoringSystem.Admin {
    public partial class EditMember : Form {
        private bool isDataModified = false;
        private bool isEditRequest = false;
        private bool isMemberIdChanged = false;

        private String mySkiYearAge = "";
        private String mySanctionNum;
        private String myMemberId;

        private DataRow myTourRow;

        private FedDropdownList myFedDropdownList;
        private MemberStatusDropdownList myMemberStatusDropdownList;
        private MemberIdValidate myMemberIdValidate;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public EditMember() {
            InitializeComponent();
        }

        public String MemberId {
            get {
                return myMemberId;
            }
        }

        private void EditMember_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.EditMember_Width > 0 ) {
                this.Width = Properties.Settings.Default.EditMember_Width;
            }
            if ( Properties.Settings.Default.EditMember_Height > 0 ) {
                this.Height = Properties.Settings.Default.EditMember_Height;
            }
            if ( Properties.Settings.Default.EditMember_Location.X > 0
                && Properties.Settings.Default.EditMember_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.EditMember_Location;
            }
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                    DataTable curTourDataTable = getTourData();
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        AgeAsOfLabel.Text = AgeAsOfLabel.Text.Substring( 0, AgeAsOfLabel.Text.Length - 2 )
                            + mySanctionNum.Substring( 0, 2 );

                        myFedDropdownList = new FedDropdownList();
                        myMemberStatusDropdownList = new MemberStatusDropdownList();
                        myMemberIdValidate = new MemberIdValidate();

                        editFederation.DataSource = myFedDropdownList.DropdownList;
                        editFederation.DisplayMember = "ItemName";
                        editFederation.ValueMember = "ItemValue";

                        editMemberStatus.DataSource = myMemberStatusDropdownList.DropdownList;
                        editMemberStatus.DisplayMember = "ItemName";
                        editMemberStatus.ValueMember = "ItemValue";

                        if ( isEditRequest ) {
                            this.Text = "Edit Member Information";
                            NextAvailableLabel.Visible = false;
                            editMemberDataLoad();
                        } else {
                            this.Text = "Add Member";
                            NextAvailableLabel.Visible = true;
                            initMemberDataLoad();
                        }

                    } else {
                        MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    }
                }
            }
        }

        private void EditMember_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.EditMember_Width = this.Size.Width;
                Properties.Settings.Default.EditMember_Height = this.Size.Height;
                Properties.Settings.Default.EditMember_Location = this.Location;
            }
        }

        private void EditMember_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                e.Cancel = true;
            }
        }

        private void saveButton_Click( object sender, EventArgs e ) {
            if ( editMemberId_Validation() ) {
                if ( isEditRequest ) {
                    if ( isMemberIdChanged ) {
                        updateMemberKey();
                    } else {
                        updateMemberData();
                    }
                } else {
                    insertMemberRecord();
                }
            }
        }

        private void updateMemberKey() {
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curMemberId = editMemberId.Text;

            //Remove all record and update all scores for current tournament
            try {
                myDbConn.Open();
                mySqlStmt = myDbConn.CreateCommand();
                mySqlStmt.CommandText = "";

                curSqlStmt.Append( "Update MemberList Set MemberId = '" + curMemberId + "' ");
                curSqlStmt.Append( "Where MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourReg Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update EventReg Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomRecap Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickPass Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpRecap Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWork Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWorkAsgmt Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                isMemberIdChanged = false;

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to change member id \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }

            updateMemberData();
        }

        private void insertMemberRecord() {
            myDbConn.Open();
            mySqlStmt = myDbConn.CreateCommand();
            mySqlStmt.CommandText = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            String curMemberId = "", curLastName = "", curFirstName = "", curState = "", curCity = "",
                curFed = "", curGender = "", curMemberStatus = "";
            byte curSkiYearAge = 0;
            bool curValidStatus = true;

            if ( editMemberId_Validation() ) {
                curMemberId = editMemberId.Text;

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select MemberId From MemberList Where MemberId like '" + curMemberId + "' " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if ( curDataTable.Rows.Count > 0 ) {
                    curValidStatus = false;
                    MessageBox.Show( "MemberId is already being used " );
                }
            } else {
                curValidStatus = false;
            }
            if ( editFirstName_Validation() ) {
                curFirstName = editFirstName.Text;
            } else {
                curValidStatus = false;
            }
            if ( editLastName_Validation() ) {
                curLastName = editLastName.Text;
                curLastName = curLastName.Replace( "'", "''" );
            } else {
                curValidStatus = false;
            }
            if ( editGenderSelect_Validation() ) {
                curGender = editGenderSelect.RatingValue;
                String curValue = (String)editGenderSelect.Tag;
            } else {
                curValidStatus = false;
            }
            if ( editSkiYearAge_Validation() ) {
                curSkiYearAge = Convert.ToByte( editSkiYearAge.Text );
            } else {
                curValidStatus = false;
            }
            if ( editMemberStatus_Validation() ) {
                curMemberStatus = editMemberStatus.Text;
            } else {
                curValidStatus = false;
            }
            if (editCity_Validation()) {
                curCity = editCity.Text;
            } else {
                curValidStatus = false;
            }
            if (editState_Validation()) {
                curState = editState.Text;
            } else {
                curValidStatus = false;
            }
            if ( editFederation_Validation() ) {
                curFed = editFederation.Text;
            } else {
                curValidStatus = false;
            }
            DateTime curDate = DateTime.Now;


            curSqlStmt = new StringBuilder( "" );
            try {
                if ( curValidStatus ) {
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    curSqlStmt.Append( "Insert MemberList (" );
                    curSqlStmt.Append( "MemberId, LastName, FirstName, State, City, Federation, SkiYearAge, Gender, MemberStatus, InsertDate, UpdateDate" );
                    curSqlStmt.Append( ") Values (" );
                    curSqlStmt.Append( " '" + curMemberId + "'" );
                    curSqlStmt.Append( ", '" + curLastName + "'" );
                    curSqlStmt.Append( ", '" + curFirstName + "'" );
                    curSqlStmt.Append( ", '" + curState + "'" );
                    curSqlStmt.Append( ", '" + curCity + "'" );
                    curSqlStmt.Append( ", '" + curFed + "'" );
                    curSqlStmt.Append( ", " + curSkiYearAge.ToString() );
                    curSqlStmt.Append( ", '" + curGender + "'" );
                    curSqlStmt.Append( ", '" + curMemberStatus + "'" );
                    curSqlStmt.Append( ", '" + curDate.ToString( "yyyy/MM/dd HH:mm:ss" ) + "'" );
                    curSqlStmt.Append( ", '" + curDate.ToString( "yyyy/MM/dd HH:mm:ss" ) + "'" );
                    curSqlStmt.Append( ")" );
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    int rowsProc = mySqlStmt.ExecuteNonQuery();
                    if ( rowsProc > 0 ) {
                        isDataModified = false;
                        myMemberId = curMemberId;
                    } else {
                        MessageBox.Show( "No rows added" );
                    }
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to add new member \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }
        }

        private void updateMemberData() {
            myDbConn.Open();
            mySqlStmt = myDbConn.CreateCommand();
            mySqlStmt.CommandText = "";

            String curMemberId = "", curLastName = "", curFirstName = "", curState = "", curCity = ""
                , curFed = "", curGender = "", curMemberStatus = "", curReadyToSki = "N";
            byte curSkiYearAge = 0;
            bool curValidStatus = true;

            if ( editMemberId_Validation() ) {
                curMemberId = editMemberId.Text;
            } else {
                curValidStatus = false;
            }
            if ( editFirstName_Validation() ) {
                curFirstName = editFirstName.Text;
            } else {
                curValidStatus = false;
            }
            if ( editLastName_Validation() ) {
                curLastName = editLastName.Text;
                curLastName = curLastName.Replace( "'", "''" );
            } else {
                curValidStatus = false;
            }
            if ( editGenderSelect_Validation() ) {
                curGender = editGenderSelect.RatingValue;
                String curValue = (String)editGenderSelect.Tag;
            } else {
                curValidStatus = false;
            }
            if ( editSkiYearAge_Validation() ) {
                curSkiYearAge = Convert.ToByte( editSkiYearAge.Text );
            } else {
                curValidStatus = false;
            }
            if ( editMemberStatus_Validation() ) {
                curMemberStatus = editMemberStatus.Text;
                if ( curMemberStatus.ToUpper().Equals( "ACTIVE" ) ) {
                    curReadyToSki = "Y";
                }
            } else {
                curValidStatus = false;
            }
            if (editCity_Validation()) {
                curCity = editCity.Text;
            } else {
                curValidStatus = false;
            }
            if (editState_Validation()) {
                curState = editState.Text;
            } else {
                curValidStatus = false;
            }
            if ( editFederation_Validation() ) {
                curFed = editFederation.Text;
            } else {
                curValidStatus = false;
            }
            DateTime curDate = DateTime.Now;


            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
                if ( curValidStatus ) {
                    curSqlStmt.Append( "Update MemberList " );
                    curSqlStmt.Append( " Set FirstName = '" + curFirstName + "'" );
                    curSqlStmt.Append( ", LastName = '" + curLastName + "'" );
                    curSqlStmt.Append( ", State = '" + curState + "'" );
                    curSqlStmt.Append( ", City = '" + curCity + "'" );
                    curSqlStmt.Append( ", Federation = '" + curFed + "'" );
                    curSqlStmt.Append( ", SkiYearAge = " + curSkiYearAge.ToString() );
                    curSqlStmt.Append( ", Gender = '" + curGender + "'" );
                    curSqlStmt.Append( ", MemberStatus = '" + curMemberStatus + "'" );
                    curSqlStmt.Append( ", UpdateDate = '" + curDate.ToString( "yyyy/MM/dd HH:mm:ss" ) + "'" );
                    curSqlStmt.Append( " Where MemberId = '" + curMemberId + "'" );
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    int rowsProc = mySqlStmt.ExecuteNonQuery();

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TourReg " );
                    curSqlStmt.Append( " Set SkierName = '" + curLastName + ", " + curFirstName + "'" );
                    curSqlStmt.Append( ", State = '" + curState + "'" );
                    curSqlStmt.Append( ", City = '" + curCity + "'" );
                    curSqlStmt.Append( ", Federation = '" + curFed + "'" );
                    curSqlStmt.Append( ", SkiYearAge = " + curSkiYearAge.ToString() );
                    curSqlStmt.Append( ", Gender = '" + curGender + "'" );
                    curSqlStmt.Append( ", ReadyToSki = '" + curReadyToSki + "'" );
                    curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "'" );
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    rowsProc = mySqlStmt.ExecuteNonQuery();

                    isDataModified = false;
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to update member data \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }
        }

        private void cancelButton_Click( object sender, EventArgs e ) {
            isDataModified = false;
        }

        public bool editMember(String inMemberId) {
            if ( inMemberId == null ) {
                isEditRequest = false;
                myMemberId = "";
            } else {
                isEditRequest = true;
                myMemberId = inMemberId;
            }
            return true;
        }

        private void initMemberDataLoad() {
            editMemberId.Text = "";
            editFirstName.Text = "";
            editLastName.Text = "";
            editGenderSelect.Text = "M";
            editSkiYearAge.Text = "";
            editMemberStatus.Text = "";
            editState.Text = "";
            editCity.Text = "";
            editFederation.Text = "";
            editInsertDateShow.Text = "";
            editUpdateDateShow.Text = "";

            String curNextNumber = "000000010";
            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select Max(MemberId) as LastTempMember From MemberList Where MemberId like '000000x%' " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if ( curDataTable.Rows.Count > 0 ) {
                    if ( curDataTable.Rows[0]["LastTempMember"] == System.DBNull.Value ) {
                        curNextNumber = "000000010";
                    } else {
                        String curTempNumber = (String)curDataTable.Rows[0]["LastTempMember"];
                        int curTempMemberNum = Convert.ToInt32( curTempNumber );
                        curNextNumber = ( curTempMemberNum + 1 ).ToString( "000000000" );
                    }
                }
            } catch {
                curNextNumber = "000000010";
            }
            NextAvailableLabel.Text = "Next Temp Number: " + curNextNumber;
        }

        private void editMemberDataLoad() {
            bool curMemberFound = false;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId, FirstName, LastName, State, City, Federation, SkiYearAge, Gender, MemberStatus, InsertDate, UpdateDate" );
            curSqlStmt.Append( " From MemberList Where MemberId = '" + myMemberId + "'" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable != null ) {
                if ( curDataTable.Rows.Count > 0 ) {
                    curMemberFound = true;
                }
            }
            if ( curDataTable != null ) {
                if ( curDataTable.Rows.Count > 0 ) {
                    DataRow curRow = curDataTable.Rows[0];
                    editMemberId.Text = curRow["MemberId"].ToString();

                    try {
                        editFirstName.Text = curRow["FirstName"].ToString();
                    } catch {
                        editFirstName.Text = "";
                    }
                    try {
                        editLastName.Text = curRow["LastName"].ToString();
                    } catch {
                        editLastName.Text = "";
                    }
                    try {
                        editGenderSelect.RatingValue = curRow["Gender"].ToString();
                    } catch {
                        editGenderSelect.Text = "";
                    }
                    try {
                        Int16 curValue = (byte)curRow["SkiYearAge"];
                        editSkiYearAge.Text = curValue.ToString();
                    } catch {
                        editSkiYearAge.Text = "";
                    }
                    try {
                        editMemberStatus.Text = curRow["MemberStatus"].ToString();
                    } catch {
                        editMemberStatus.Text = "";
                    }
                    try {
                        editState.Text = curRow["State"].ToString();
                    } catch {
                        editState.Text = "";
                    }
                    try {
                        editCity.Text = curRow["City"].ToString();
                    } catch {
                        editCity.Text = "";
                    }
                    try {
                        editFederation.Text = curRow["Federation"].ToString();
                    } catch {
                        editFederation.Text = "";
                    }
                    try {
                        editInsertDateShow.Text = ( (DateTime)curRow["InsertDate"] ).ToString( "MM/dd/yy hh:mm:ss" );
                    } catch {
                        editInsertDateShow.Text = "";
                    }
                    try {
                        editUpdateDateShow.Text = ( (DateTime)curRow["UpdateDate"] ).ToString( "MM/dd/yy hh:mm:ss" );
                    } catch {
                        editUpdateDateShow.Text = "";
                    }
                }
            }
        }

        private void editGenderSelect_Load( object sender, EventArgs e ) {
        }

        private bool editMemberId_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            isMemberIdChanged = false;
            if ( editMemberId.Text.Length == 9 ) {
                if ( myMemberIdValidate.checkMemberId( editMemberId.Text ) ) {
                    if ( isEditRequest ) {
                        if ( !( myMemberId.Equals( editMemberId.Text ) ) ) {
                            isMemberIdChanged = true;
                        }
                    }
                } else {
                    curReturnStatus = false;
                    MessageBox.Show( "MemberId is not valid, failed checksum verification" );
                }
            } else if ( editMemberId.Text.Length == 0 ) {
                curReturnStatus = false;
                MessageBox.Show( "MemberId is a required field" );
            } else {
                curReturnStatus = false;
                MessageBox.Show( "MemberId must be 9 characters in length with no dashes " );
            }
            return curReturnStatus;
        }

        private void editSkiYearAge_Validated( object sender, EventArgs e ) {
            if ( editSkiYearAge.Text.Length > 0 ) {
                if ( editSkiYearAge.Text != mySkiYearAge ) {
                    isDataModified = true;
                    try {
                        Int16 curValue = Convert.ToInt16( editSkiYearAge.Text );
                        if ( curValue > 99 ) {
                            MessageBox.Show( "Ski year age must be a valid age less than 100 " );
                        }
                    } catch {
                        MessageBox.Show( "Ski year age must be numeric " );
                    }
                }
            }
            mySkiYearAge = editSkiYearAge.Text;
        }

        private bool editSkiYearAge_Validation() {
            bool curReturnStatus = true;
            if ( editSkiYearAge.Text.Trim().Length > 0 ) {
                if ( !( editSkiYearAge.Text.Equals( "0" ) ) ) {
                    try {
                        Int16 curValue = Convert.ToInt16( editSkiYearAge.Text );
                        if ( curValue > 99 ) {
                            curReturnStatus = false;
                            MessageBox.Show( "Ski year age must be a valid age less than 100 " );
                        }
                    } catch {
                        curReturnStatus = false;
                        MessageBox.Show( "Ski year age must be numeric " );
                    }
                }
            } else {
                editSkiYearAge.Text = "0";
            }
            return curReturnStatus;
        }

        private bool editLastName_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            if ( editLastName.Text.Trim().Length == 0 ) {
                curReturnStatus = false;
                MessageBox.Show( "Last name is a required field" );
            }
            return curReturnStatus;
        }

        private bool editFirstName_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            if ( editFirstName.Text.Trim().Length == 0 ) {
                curReturnStatus = false;
                MessageBox.Show( "First name is a required field" );
            }
            return curReturnStatus;
        }

        private bool editState_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            return curReturnStatus;
        }

        private bool editCity_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            return curReturnStatus;
        }

        private bool editFederation_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            return curReturnStatus;
        }

        private bool editGenderSelect_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            if ( editGenderSelect.RatingValue.Trim().Length != 1 ) {
                curReturnStatus = false;
                MessageBox.Show( "Gender is a required field" );
            }
            return curReturnStatus;
        }

        private bool editMemberStatus_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            if ( editMemberStatus.Text.Trim().Length == 0 ) {
                curReturnStatus = false;
                MessageBox.Show( "Member status is a required field" );
            }
            return curReturnStatus;
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
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
