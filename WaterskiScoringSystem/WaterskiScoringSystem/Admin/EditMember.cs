using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Admin {
    public partial class EditMember : Form {
        private bool isDataModified = false;
        private bool isEditRequest = false;
        private bool isMemberIdChanged = false;

        private String mySkiYearAge = "";
        private String mySanctionNum;
        private String myMemberId;
        private String myTourRules;

        private DataRow myTourRow;

        private FedDropdownList myFedDropdownList;
        private MemberIdValidate myMemberIdValidate;

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
			if ( mySanctionNum == null || mySanctionNum.Length < 6 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return;
			}

			DataTable curTourDataTable = getTourData();
			if ( curTourDataTable.Rows.Count == 0 ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				return;
			}

			myTourRow = curTourDataTable.Rows[0];
			AgeAsOfLabel.Text = AgeAsOfLabel.Text.Substring( 0, AgeAsOfLabel.Text.Length - 2 ) + mySanctionNum.Substring( 0, 2 );
            myTourRules = (String)myTourRow["Rules"];

            myFedDropdownList = new FedDropdownList();
			myMemberIdValidate = new MemberIdValidate();

			editFederation.DataSource = myFedDropdownList.DropdownList;
			editFederation.DisplayMember = "ItemName";
			editFederation.ValueMember = "ItemValue";

			if ( isEditRequest ) {
				this.Text = "Edit Member Information";
				NextAvailableLabel.Visible = false;
				editMemberDataLoad();
			
			} else {
				this.Text = "Add Member";
				NextAvailableLabel.Visible = true;
				initMemberDataLoad();
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
            if ( isDataModified ) e.Cancel = true;
        }

        private void saveButton_Click( object sender, EventArgs e ) {
            if ( editMemberId_Validation() ) {
                if ( isEditRequest ) {
                    if ( isMemberIdChanged ) updateMemberKey();
					
					updateMemberData();

				} else {
                    insertMemberRecord();
                }
            }
        }

		/*
		 * Update all tournament records when a MemberId has been updated
		 */
        private void updateMemberKey() {
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            String curMemberId = editMemberId.Text;

            try {
                curSqlStmt.Append( "Update MemberList Set MemberId = '" + curMemberId + "' ");
                curSqlStmt.Append( "Where MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourReg Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update EventReg Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomRecap Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickPass Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpRecap Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpScore Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWork Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWorkAsgmt Set MemberId = '" + curMemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				isMemberIdChanged = false;

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to change member id \n" + excp.Message );
            }
        }

        private bool insertMemberRecord() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
			MemberEntry curMemberEntry = validateInput( true );
			if ( curMemberEntry == null ) return false;

			try {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert MemberList (" );
				curSqlStmt.Append( "MemberId, LastName, FirstName, State, City, Federation, ForeignFederationID, SkiYearAge, Gender, MemberStatus, InsertDate, UpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( " '" + curMemberEntry.MemberId + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getLastNameForDB() + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getFirstNameForDB() + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.getCityForDB() + "'" );
                curSqlStmt.Append( ", '" + curMemberEntry.Federation + "'" );
                curSqlStmt.Append( ", '" + curMemberEntry.ForeignFederationID + "'" );
                curSqlStmt.Append( ", " + curMemberEntry.SkiYearAge.ToString() );
				curSqlStmt.Append( ", '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", GetDate() " );
				curSqlStmt.Append( ", GetDate() " );
				curSqlStmt.Append( ")" );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
				if ( rowsProc > 0 ) {
					isDataModified = false;
					myMemberId = curMemberEntry.MemberId;
					return true;
				
				} else {
					MessageBox.Show( "No rows added" );
					return false;
				}

			} catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to add new member \n" + excp.Message );
				return false;
			}
        }

        private bool updateMemberData() {
            String curReadyToSki = "N";
			MemberEntry curMemberEntry = validateInput( false );
			if ( curMemberEntry == null ) return false;

			if ( curMemberEntry.MemberStatus.ToUpper().Equals( "ACTIVE" ) ) curReadyToSki = "Y";

            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
				curSqlStmt.Append( "Update MemberList " );
				curSqlStmt.Append( "Set FirstName = '" + curMemberEntry.getFirstNameForDB() + "'" );
				curSqlStmt.Append( ", LastName = '" + curMemberEntry.getLastNameForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
                curSqlStmt.Append( ", ForeignFederationID = '" + curMemberEntry.ForeignFederationID + "'" );
                curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge.ToString() );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", MemberStatus = '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", UpdateDate = GetDate() " );
				curSqlStmt.Append( " Where MemberId = '" + curMemberEntry.MemberId + "'" );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TourReg " );
				curSqlStmt.Append( "Set SkierName = '" + curMemberEntry.getSkierNameForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
                curSqlStmt.Append( ", ForeignFederationID = '" + curMemberEntry.ForeignFederationID + "'" );
                curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge.ToString() );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", ReadyToSki = '" + curReadyToSki + "'" );
                curSqlStmt.Append( ", LastUpdateDate = GetDate() " );
                curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				isDataModified = false;
				return true;
            
			} catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to update member data \n" + excp.Message );
				return false;
            }
        }

		private MemberEntry validateInput( bool newMember ) {
			MemberEntry curMemberEntry = new MemberEntry();

			if ( !( editMemberId_Validation() ) ) return null;
			curMemberEntry.MemberId = editMemberId.Text;
			DataRow curMemberRow = getMemberEntry( curMemberEntry.MemberId );
			if ( curMemberRow != null && newMember ) {
				MessageBox.Show( "MemberId is already being used " );
				return null;
			}
			if ( curMemberRow == null && !(newMember) ) {
				MessageBox.Show( "Expected member entry doesn't exist" );
				return null;
			}

			if ( !( editFirstName_Validation() ) ) return null;
			curMemberEntry.FirstName = editFirstName.Text;
			if ( !( editLastName_Validation() ) ) return null;
			curMemberEntry.LastName = editLastName.Text;
			if ( !( editGenderSelect_Validation() ) ) return null;

			if ( !( editGenderSelect_Validation() ) ) return null;
			curMemberEntry.Gender = editGenderSelect.RatingValue;
			String curGenderSelect = (String)editGenderSelect.Tag;

			if ( !( editSkiYearAge_Validation() ) ) return null;
			curMemberEntry.SkiYearAge = Convert.ToByte( editSkiYearAge.Text );

			curMemberEntry.MemberStatus = showMemberStatus.Text;

			if ( !( editState_Validation() ) ) return null;
			curMemberEntry.State = editState.Text;
			if ( !( editCity_Validation() ) ) return null;
			curMemberEntry.City = editCity.Text;
			if ( !( editFederation_Validation() ) ) return null;
            curMemberEntry.Federation = ((String)editFederation.SelectedValue).ToUpper();
            curMemberEntry.ForeignFederationID = editForeignFederationID.Text;
            return curMemberEntry;
		}

		private void cancelButton_Click( object sender, EventArgs e ) {
            isDataModified = false;
        }

        public bool editMember(String inMemberId) {
            if ( inMemberId == null ) {
                isEditRequest = false;
                myMemberId = "";
				return true;
            }

			DataRow curMemberRow = getMemberEntry( inMemberId );
			if ( curMemberRow == null ) {
				isEditRequest = false;
				myMemberId = "";
				return true;
			}

			isEditRequest = true;
			myMemberId = inMemberId;
			return true;
		}

		private void initMemberDataLoad() {
            editMemberId.Text = "";
            editFirstName.Text = "";
            editLastName.Text = "";
            editGenderSelect.Text = "";
            editSkiYearAge.Text = "";
			showMemberStatus.Text = "Inactive";
            editState.Text = "";
            editCity.Text = "";
            editFederation.SelectedValue = "";
            editForeignFederationID.Text = "";
            editInsertDateShow.Text = "";
            editUpdateDateShow.Text = "";

            String curNextNumber = "000000010";
            try {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select Max(MemberId) as LastTempMember From MemberList Where MemberId like '000000x%' " );
				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
			DataRow curMemberRow = getMemberEntry( myMemberId );
			editMemberId.Text = HelperFunctions.getDataRowColValue( curMemberRow, "MemberId", "" );
			editFirstName.Text = HelperFunctions.getDataRowColValue( curMemberRow, "FirstName", "" );
			editLastName.Text = HelperFunctions.getDataRowColValue( curMemberRow, "LastName", "" );
			editGenderSelect.RatingValue = HelperFunctions.getDataRowColValue( curMemberRow, "Gender", "" );
			editSkiYearAge.Text = HelperFunctions.getDataRowColValue( curMemberRow, "SkiYearAge", "" );
			showMemberStatus.Text = HelperFunctions.getDataRowColValue( curMemberRow, "MemberStatus", "" );
			editCity.Text = HelperFunctions.getDataRowColValue( curMemberRow, "City", "" );
			editState.Text = HelperFunctions.getDataRowColValue( curMemberRow, "State", "" );
            editFederation.SelectedValue = HelperFunctions.getDataRowColValue( curMemberRow, "Federation", "" ).ToLower();
            editForeignFederationID.Text = HelperFunctions.getDataRowColValue( curMemberRow, "ForeignFederationID", "" );

            editInsertDateShow.Text = HelperFunctions.getDataRowColValue( curMemberRow, "InsertDate", "" );
			editUpdateDateShow.Text = HelperFunctions.getDataRowColValue( curMemberRow, "UpdateDate", "" );
        }

        private void editGenderSelect_Load( object sender, EventArgs e ) {
        }

        private bool editMemberId_Validation() {
            isDataModified = true;
            isMemberIdChanged = false;

            if ( Regex.IsMatch( editMemberId.Text, "^[0-9]{9}" ) ) {
                if ( myMemberIdValidate.checkMemberId( editMemberId.Text ) ) {
                    if ( isEditRequest ) {
                        if ( !( myMemberId.Equals( editMemberId.Text ) ) ) {
                            isMemberIdChanged = true;
                        }
                    }
                } else {
                    if ( HelperFunctions.isObjectPopulated( (String)editFederation.SelectedValue )
                        && !( ( (String)editFederation.SelectedValue ).Equals( "USA" ) )
                        ) {
                        showMemberStatus.Text = "ACTIVE";
                        return true;
                    }
                    MessageBox.Show( "MemberId is not valid, failed checksum verification" );
                    return false;
                }
            
            } else if ( editMemberId.Text.Length == 0 ) {
                MessageBox.Show( "MemberId is a required field" );
                return false;
            
            } else {
                if ( Regex.IsMatch( editMemberId.Text, "^[0-9]+$" ) ) {
                    if ( HelperFunctions.isObjectPopulated( (String)editFederation.SelectedValue )
                        && !( ( (String)editFederation.SelectedValue ).Equals( "USA" ) )
                        ) {
                        showMemberStatus.Text = "ACTIVE";
                        return true;
                    }
                }
                MessageBox.Show( "MemberId must be 9 numeric characters with no dashes " );
                return false;

            }

            return true;
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
            } else {
                editSkiYearAge.Text = "0";
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
            if ( HelperFunctions.isObjectEmpty( editFederation.SelectedValue ) ) {
                MessageBox.Show( "Federation is required" );
                return false;
            }
            isDataModified = true;
            return true;
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

		private DataRow getMemberEntry(string memberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * From MemberList Where MemberId like '" + memberId + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count == 0 ) return null;
			return curDataTable.Rows[0];
		}
	}
}
