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

namespace WaterskiScoringSystem.Tournament {
    public partial class EditRegMember : Form {
        private bool isDataModified = false;
        private String myReqstStatus = "";
        private bool isMemberIdChanged = false;
        private bool isAgeGroupChanged = false;
        private String mySkiYearAge = "";
        private String mySanctionNum;
        private String myMemberId;
        private String myAgeGroup;

        private DataGridViewRow myMemberViewRow;
        private DataRow myTourRow;

        private TourEventReg myTourEventReg;
        private FedDropdownList myFedDropdownList;
        private MemberStatusDropdownList myMemberStatusDropdownList;
        private MemberIdValidate myMemberIdValidate;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public EditRegMember() {
            InitializeComponent();
        }

        public String ReqstStatus {
            get {
                return myReqstStatus;
            }
        }

        private void EditRegMember_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.EditRegMember_Width > 0 ) {
                this.Width = Properties.Settings.Default.EditRegMember_Width;
            }
            if ( Properties.Settings.Default.EditRegMember_Height > 0 ) {
                this.Height = Properties.Settings.Default.EditRegMember_Height;
            }
            if ( Properties.Settings.Default.EditRegMember_Location.X > 0
                && Properties.Settings.Default.EditRegMember_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.EditRegMember_Location;
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
                        myTourEventReg = new TourEventReg();
                        myMemberIdValidate = new MemberIdValidate();
    
                        editFederation.DataSource = myFedDropdownList.DropdownList;
                        editFederation.DisplayMember = "ItemName";
                        editFederation.ValueMember = "ItemValue";

                        editMemberStatus.DataSource = myMemberStatusDropdownList.DropdownList;
                        editMemberStatus.DisplayMember = "ItemName";
                        editMemberStatus.ValueMember = "ItemValue";

                        editGenderSelect.addClickEvent( editGenderSelect_Click );

                        if ( myMemberViewRow == null ) {
                            this.Text = "Edit Member Registration";
                            editMemberDataLoad( myMemberId, myAgeGroup );
                        } else {
                            this.Text = "Add Registration for Member";
                            editMemberDataLoad();
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
            bool curContinueUpdate = true;
            if ( editMemberId_Validation() ) {
                if ( editAgeGroup_Validation() ) {
                    if (isMemberIdChanged || isAgeGroupChanged) {
                        if (isAgeGroupChanged) {
                            if (checkSkierEventReg()) {
                                updateMemberKey();
                            } else {
                                curContinueUpdate = false;
                            }
                        } else {
                            updateMemberKey();
                        }
                    }
                    if (curContinueUpdate) {
                        bool curReqstStatus = myTourEventReg.addTourReg( editMemberId.Text, "Registration added", AgeGroupSelect.CurrentValue, "", "" );
                        if (curReqstStatus) {
                            myReqstStatus = "Added";
                        } else {
                            myReqstStatus = "Skipped";
                        }
                        updateMemberData();
                        DialogResult = DialogResult.OK;
                    } else {
                        //DialogResult = DialogResult.Cancel;
                    }
                }
            } else {
                //Invalid data, don't close window
            }
        }

        private void updateMemberKey() {
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curMemberId = editMemberId.Text;
            String curAgeGroup = AgeGroupSelect.CurrentValue;

            //Remove all record and update all scores for current tournament
            try {
                myDbConn.Open();
                mySqlStmt = myDbConn.CreateCommand();
                mySqlStmt.CommandText = "";

                curSqlStmt.Append( "Update MemberList Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where MemberId = '" + myMemberId + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From MemberList Where MemberId = '" + curMemberId + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourReg Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From TourReg Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update EventReg Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From EventReg Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomRecap Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From SlalomRecap Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomScore Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From SlalomScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickPass Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickPass Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickScore Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpRecap Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From JumpRecap Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpScore Set MemberId = '" + curMemberId + "', AgeGroup = '" + curAgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From JumpScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + curAgeGroup + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWork Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From OfficialWork Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "')" );
                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWorkAsgmt Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "')" );
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

        private void updateMemberData() {
            myDbConn.Open();
            mySqlStmt = myDbConn.CreateCommand();
            mySqlStmt.CommandText = "";

            String curMemberId = "", curLastName = "", curFirstName = "", curCity = "", curState = "",
                curFed = "", curGender = "", curAgeGroup = "", curMemberStatus = "", curReadyToSki = "N";
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
            if ( editAgeGroup_Validation() ) {
                curAgeGroup = AgeGroupSelect.CurrentValue;
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
            if ( editState_Validation() ) {
                curState = editState.Text;
            } else {
                curValidStatus = false;
            }
            if (editCity_Validation()) {
                curCity = editCity.Text;
                curCity = curCity.Replace("'", "''");
            } else {
                curValidStatus = false;
            }
            if (editFederation_Validation()) {
                curFed = editFederation.Text;
            } else {
                curValidStatus = false;
            }

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
                    curSqlStmt.Append( ", UpdateDate = getdate() " );
                    curSqlStmt.Append( " Where MemberId = '" + curMemberId + "'" );
                    mySqlStmt.CommandText = curSqlStmt.ToString();
                    int rowsProc = mySqlStmt.ExecuteNonQuery();

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TourReg " );
                    curSqlStmt.Append( " Set SkierName = '" + curLastName + ", " + curFirstName + "'" );
                    curSqlStmt.Append( ", City = '" + curCity + "'" );
                    curSqlStmt.Append( ", State = '" + curState + "'" );
                    curSqlStmt.Append( ", Federation = '" + curFed + "'" );
                    curSqlStmt.Append( ", SkiYearAge = " + curSkiYearAge.ToString() );
                    curSqlStmt.Append( ", Gender = '" + curGender + "'" );
                    curSqlStmt.Append( ", ReadyToSki = '" + curReadyToSki + "'" );
                    curSqlStmt.Append( ", LastUpdateDate = getdate() " );
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
            myReqstStatus = "Cancelled";
        }

        public bool editMember( String inMemberId, String inAgeGroup ) {
            myMemberId = inMemberId;
            myAgeGroup = inAgeGroup;
            myMemberViewRow = null;
            return true;
        }
        public bool editMember( DataGridViewRow inMemberViewRow ) {
            myMemberViewRow = inMemberViewRow;
            myMemberId = (String)inMemberViewRow.Cells["MemberId"].Value;
            try {
                myAgeGroup = (String)inMemberViewRow.Cells["AgeGroup"].Value;
            } catch {
                myAgeGroup = "";
            }
            return true;
        }

        private void editMemberDataLoad() {
            editMemberId.Text = (String)myMemberViewRow.Cells["MemberId"].Value;
            try {
                editFirstName.Text = (String)myMemberViewRow.Cells["FirstName"].Value;
                editLastName.Text = (String)myMemberViewRow.Cells["LastName"].Value;
            } catch {
                String curSkierName = (String)myMemberViewRow.Cells["SkierName"].Value;
                String[] curNameList = curSkierName.Split( ',' );
                if ( curNameList.Length > 1 ) {
                    editLastName.Text = curNameList[0].Trim();
                    editFirstName.Text = curNameList[1].Trim();
                } else {
                    editLastName.Text = curNameList[0].Trim();
                    editFirstName.Text = "";
                }
            }
            try {
                editGenderSelect.RatingValue = (String)myMemberViewRow.Cells["Gender"].Value;
            } catch {
                editGenderSelect.RatingValue = "";
            }
            try {
                Int16 curValue = Convert.ToInt16( (String)myMemberViewRow.Cells["SkiYearAge"].Value );
                editSkiYearAge.Text = curValue.ToString();
            } catch {
                if (editGenderSelect.RatingValue.Length > 0) {
                    AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                } else {
                    AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                }
                AgeGroupSelect.CurrentValue = "";
                editSkiYearAge.Text = "0";
            }
            LoadAgeGroupList();

            try {
                editMemberStatus.Text = (String)myMemberViewRow.Cells["MemberStatus"].Value;
            } catch {
                editMemberStatus.Text = "";
            }
            try {
                editCity.Text = (String)myMemberViewRow.Cells["City"].Value;
            } catch {
                editCity.Text = "";
            }
            try {
                editState.Text = (String)myMemberViewRow.Cells["State"].Value;
            } catch {
                editState.Text = "";
            }
            try {
                editFederation.Text = (String)myMemberViewRow.Cells["Federation"].Value;
            } catch {
                editFederation.Text = "";
            }
            try {
                editMemberStatus.Text = (String)myMemberViewRow.Cells["MemberStatus"].Value;
            } catch {
                editMemberStatus.Text = "";
            }
            editMemberRegDataLoad();
        }
        private void editMemberDataLoad(String inMemberId, String inAgeGroup) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId, SkierName, City, State, Federation, SkiYearAge, AgeGroup, Gender, ReadyToSki " );
            curSqlStmt.Append( "From TourReg " );
            curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + inMemberId + "' AND AgeGroup = '" + inAgeGroup + "'" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable != null ) {
                if ( curDataTable.Rows.Count > 0 ) {
                    DataRow curDataRow = curDataTable.Rows[0];
                    editMemberId.Text = (String)curDataRow["MemberId"];
                    try {
                        String curSkierName = (String)curDataRow["SkierName"];
                        String[] curNameList = curSkierName.Split( ',' );
                        if ( curNameList.Length > 1 ) {
                            editLastName.Text = curNameList[0].Trim();
                            editFirstName.Text = curNameList[1].Trim();
                        } else {
                            editLastName.Text = curNameList[0].Trim();
                            editFirstName.Text = "";
                        }
                    } catch {
                        editLastName.Text = "";
                        editFirstName.Text = "";
                    }
                    try {
                        editGenderSelect.RatingValue = (String)curDataRow["Gender"];
                    } catch {
                        editGenderSelect.RatingValue = "";
                    }
                    try {
                        Byte curValue = (byte)curDataRow["SkiYearAge"];
                        editSkiYearAge.Text = curValue.ToString();
                        if ( curValue > 1 && curValue < 100 ) {
                            if ( editGenderSelect.RatingValue.Length > 0 ) {
                                AgeGroupSelect.SelectList_Load( curValue, editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                            } else {
                                AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                            }
                        } else {
                            if (editGenderSelect.RatingValue.Length > 0) {
                                AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                            } else {
                                AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                            }
                            editSkiYearAge.Text = "";
                        }
                        AgeGroupSelect.CurrentValue = (String)curDataRow["AgeGroup"];
                    } catch {
                        if (editGenderSelect.RatingValue.Length > 0) {
                            AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                        } else {
                            AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                        }
                        AgeGroupSelect.CurrentValue = (String)curDataRow["AgeGroup"];
                        editSkiYearAge.Text = "";
                    }

                    try {
                        editMemberStatus.Text = (String)curDataRow["MemberStatus"];
                    } catch {
                        editMemberStatus.Text = "";
                    }
                    try {
                        editCity.Text = (String)curDataRow["City"];
                    } catch {
                        editCity.Text = "";
                    }
                    try {
                        editState.Text = (String)curDataRow["State"];
                    } catch {
                        editState.Text = "";
                    }
                    try {
                        editFederation.Text = (String)curDataRow["Federation"];
                    } catch {
                        editFederation.Text = "";
                    }
                    try {
                        if ( ((String)curDataRow["ReadyToSki"]).Equals("Y") ) {
                            editMemberStatus.Text = "Active";
                        } else {
                            editMemberStatus.Text = "Inactive";
                        }
                    } catch {
                        editMemberStatus.Text = "Inactive";
                    }
                    editMemberRegDataLoad();

                }
            }
        }

        private void editMemberRegDataLoad() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select MemberId, SkierName, City, State, Federation, SkiYearAge, AgeGroup, Gender, ReadyToSki " );
            curSqlStmt.Append( "From TourReg " );
            curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable != null ) {
                if ( curDataTable.Rows.Count > 0 ) {
                    int curRowIdx = 0;
                    DataGridView.Rows.Clear();
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        curRowIdx = DataGridView.Rows.Add();
                        curViewRow = DataGridView.Rows[curRowIdx];

                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["SkierName"];
                        try {
                            curViewRow.Cells["City"].Value = (String)curDataRow["City"];
                        } catch {
                        }
                        try {
                            curViewRow.Cells["State"].Value = (String)curDataRow["State"];
                        } catch {
                        }
                        try {
                            curViewRow.Cells["Federation"].Value = (String)curDataRow["Federation"];
                        } catch {
                            curViewRow.Cells["Federation"].Value = "";
                        }
                        try {
                            curViewRow.Cells["SkiYearAge"].Value = ( (byte)curDataRow["SkiYearAge"] ).ToString();
                        } catch {
                            curViewRow.Cells["SkiYearAge"].Value = "";
                        }
                        try {
                            curViewRow.Cells["Gender"].Value = (String)curDataRow["Gender"];
                        } catch {
                            curViewRow.Cells["Gender"].Value = "";
                        }
                        try {
                            curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        } catch {
                            curViewRow.Cells["AgeGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["ReadyToSki"].Value = (String)curDataRow["ReadyToSki"];
                        } catch {
                            curViewRow.Cells["ReadyToSki"].Value = "";
                        }
                    }
                }
            }
        }

        private void editGenderSelect_Load( object sender, EventArgs e ) {
        }

        private void LoadAgeGroupList() {
            try {
                Int16 curValue = Convert.ToInt16( editSkiYearAge.Text );
                editSkiYearAge.Text = curValue.ToString();
                if ( curValue > 1 && curValue < 100 ) {
                    if ( editGenderSelect.RatingValue.Length > 0 ) {
                        AgeGroupSelect.SelectList_Load( curValue, editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                    } else {
                        AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                    }
                } else {
                    AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                    editSkiYearAge.Text = "";
                }
                AgeGroupSelect.CurrentValue = "";
            } catch {
            }
        }

        private void AgeGroupSelect_Change( object sender, EventArgs e ) {
            if ( sender != null ) {
                AgeGroupSelect.CurrentValue = ( (RadioButtonWithValue)sender ).Value.ToString();
            }
        }

        private bool editMemberId_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            isMemberIdChanged = false;
            if ( editMemberId.Text.Length == 9 ) {
                if ( !( myMemberId.Equals( editMemberId.Text ) ) ) {
                    if ( myMemberIdValidate.checkMemberId( editMemberId.Text ) ) {
                        isMemberIdChanged = true;
                    } else {
                        curReturnStatus = false;
                        if ( editMemberId.Text.Substring( 0, 7 ).Equals( "0000000" ) ) {
                            MessageBox.Show( "Invalid temporary MemberId, must be between 000000010 and 000000099" );
                        } else {
                            MessageBox.Show( "MemberId is not valid, failed checksum verification" );
                        }
                    }
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

        private bool editAgeGroup_Validation() {
            bool curReturnStatus = true;
            isDataModified = true;
            isAgeGroupChanged = false;

            if ( AgeGroupSelect.CurrentValue.Trim().Length > 1 ) {
                if ( myAgeGroup.Length > 0 ) {
                    if (!( myAgeGroup.Equals( AgeGroupSelect.CurrentValue.Trim().Substring( 0, 2 ) ) ) ) {
                        isAgeGroupChanged = true;
                    }
                }
            } else {
                curReturnStatus = false;
                MessageBox.Show( "Age group is a required field" );
            }
            return curReturnStatus;
        }

        private bool checkSkierEventReg() {
            bool curReturnStatus = true;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            DataTable curDataTable = null;

            if (AgeGroupSelect.isDivisionIntl( AgeGroupSelect.CurrentValue )) {
                //Check for event registrations using the original age group
                String curMemberId = editMemberId.Text;
                curSqlStmt.Append( "Select  MemberId, AgeGroup, Event, EventClass " );
                curSqlStmt.Append( "From EventReg " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberId + "' AND AgeGroup = '" + myAgeGroup + "' " );
                curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable == null) {
                    curReturnStatus = true;
                } else {
                    curReturnStatus = true;
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT ListCode, CodeValue, ListCodeNum, SortSeq FROM CodeValueList WHERE ListName = 'AWSAAgeGroup' AND CodeValue = '" + AgeGroupSelect.CurrentValue + "'" );
                    curDataTable = getData( curSqlStmt.ToString() );
                    if (curDataTable.Rows.Count > 0) {
                        DataRow[] curFindClassERow, curFindEventClassRow;
                        DataRow curClassERow, curTourClassRow;
                        int curClassEValue = 0, curEventClassValue = 0;
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "SELECT ListCode, CodeValue, ListCodeNum, SortSeq FROM CodeValueList WHERE ListName = 'ClassTour' ORDER BY SortSeq" );
                        DataTable curTourClassDataTable = getData( curSqlStmt.ToString() );
                        curFindClassERow = curTourClassDataTable.Select( "ListCode = 'E'" );
                        curClassERow = curFindClassERow[0];
                        curClassEValue = Convert.ToInt32( (Decimal)curClassERow["ListCodeNum"] );

                        foreach (DataRow curEventRegRow in curDataTable.Rows) {
                            curFindEventClassRow = curTourClassDataTable.Select( "ListCode = '" + curEventRegRow["EventClass"] + "'" );
                            if (curFindEventClassRow.Length > 0) {
                                curTourClassRow = curFindEventClassRow[0];
                                curEventClassValue = Convert.ToInt32( (Decimal)curTourClassRow["ListCodeNum"] );
                                if (curEventClassValue > curClassEValue) {
                                } else {
                                    MessageBox.Show( "Skier registered in an event with a class less than allowed for an international division" );
                                    curReturnStatus = false;
                                    break;
                                }
                            }
                        }
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                    curSqlStmt.Append( "FROM EventReg R " );
                    curSqlStmt.Append( "INNER JOIN SlalomScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                    curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Slalom' " );
                    curSqlStmt.Append( "UNION " );
                    curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                    curSqlStmt.Append( "FROM EventReg R " );
                    curSqlStmt.Append( "INNER JOIN TrickScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                    curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Trick' " );
                    curSqlStmt.Append( "UNION " );
                    curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                    curSqlStmt.Append( "FROM EventReg R " );
                    curSqlStmt.Append( "INNER JOIN JumpScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                    curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Jump' " );
                    curDataTable = getData( curSqlStmt.ToString() );
                    if (curDataTable != null) {
                        if (curDataTable.Rows.Count > 0) {
                            MessageBox.Show( "Skier has scores that should be reviewed to determine if they need to be recalculated. "
                                + "Slalom scores will be significantly different when switching from an AWSA division to an IWWF division." 
                                + "\n\nTo recalculate a slalom score simply re-enter the boat time for the last passed and this will trigger a recalculation including rules analysis."
                                + "\n\nJump scores should also be re-calculated by re-entering the boat times for the last pass."
                                + "\n\nTrick scores can be recalculated by using the 'Calc' button."
                                );
                        }
                    }
                }
            } else {
                String curMemberId = editMemberId.Text;
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                curSqlStmt.Append( "FROM EventReg R " );
                curSqlStmt.Append( "INNER JOIN SlalomScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Slalom' " );
                curSqlStmt.Append( "UNION " );
                curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                curSqlStmt.Append( "FROM EventReg R " );
                curSqlStmt.Append( "INNER JOIN TrickScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Trick' " );
                curSqlStmt.Append( "UNION " );
                curSqlStmt.Append( "SELECT S.MemberId, S.AgeGroup, R.Event, S.Round " );
                curSqlStmt.Append( "FROM EventReg R " );
                curSqlStmt.Append( "INNER JOIN JumpScore S ON S.SanctionId = R.SanctionId AND S.MemberId = R.MemberId AND S.AgeGroup = R.AgeGroup " );
                curSqlStmt.Append( "Where R.SanctionId = '" + mySanctionNum + "' AND R.MemberId = '" + curMemberId + "' AND R.AgeGroup = '" + myAgeGroup + "'  AND R.Event = 'Jump' " );
                curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable != null) {
                    if (curDataTable.Rows.Count > 0) {
                        MessageBox.Show( "Skier has scores that should be reviewed to determine if they need to be recalculated. "
                            + "\n\nTo recalculate a slalom score simply re-enter the boat time for the last passed and this will trigger a recalculation including rules analysis."
                            + "\n\nJump scores should also be re-calculated by re-entering the boat times for the last pass."
                            + "\n\nTrick scores can be recalculated by using the 'Calc' button."
                            );
                    }
                }
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
                        } else if ( curValue > 0 ) {
                            if ( editGenderSelect.RatingValue.Length > 0 ) {
                                LoadAgeGroupList();
                            }
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
                        } else {
                            /*
                            if ( editGenderSelect.RatingValue.Length > 0 ) {
                                AgeGroupSelect.SelectList_Load( curValue, editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                            }
                             */
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

        private void editGenderSelect_Click(object sender, EventArgs e) {
            if (sender != null) {
                try {
                    String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                    editGenderSelect.RatingValue = curValue;
                    LoadAgeGroupList();
                } catch (Exception ex) {
                    MessageBox.Show( "editGenderSelect_Click Exception: \n" + ex.Message );
                }
            }
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
