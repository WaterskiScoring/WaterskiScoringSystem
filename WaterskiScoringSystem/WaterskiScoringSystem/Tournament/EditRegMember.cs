using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Tournament {
    public partial class EditRegMember : Form {
        private bool isDataModified = false;
        private bool isMemberIdChanged = false;
        private bool isAgeGroupChanged = false;
        private bool isAgeGroupNew = false;

        private String myReqstStatus = "";
        private String mySkiYearAge = "";
        private String mySanctionNum;
        private String myMemberId;
        private String myAgeGroup;

        private DataRow myTourRow;

		private TourEventReg myTourEventReg;
        private FedDropdownList myFedDropdownList;
        private MemberIdValidate myMemberIdValidate;
        private Dictionary<string, object> myImportMemberEntry = null;

        public EditRegMember() {
            InitializeComponent();
        }

        public String ReqstStatus {
            get {
                return myReqstStatus;
            }
        }

        public bool editMember( String inMemberId, String inAgeGroup, Dictionary<string, object> inMemberDataFromAwsa ) {
            myMemberId = inMemberId;
            myAgeGroup = inAgeGroup;
            myImportMemberEntry = inMemberDataFromAwsa;
            return true;
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
                return;
            }
            if ( mySanctionNum.Length < 6 ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                return;
            }
            DataTable curTourDataTable = getTourData();
            if ( curTourDataTable == null || curTourDataTable.Rows.Count == 0 ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                return;
            }

            myTourRow = curTourDataTable.Rows[0];
            AgeAsOfLabel.Text = AgeAsOfLabel.Text.Substring( 0, AgeAsOfLabel.Text.Length - 2 ) + mySanctionNum.Substring( 0, 2 );

            myFedDropdownList = new FedDropdownList();
            myTourEventReg = new TourEventReg();
            myMemberIdValidate = new MemberIdValidate();

            editFederation.DataSource = myFedDropdownList.DropdownList;
            editFederation.DisplayMember = "ItemName";
            editFederation.ValueMember = "ItemValue";

            editGenderSelect.addClickEvent( editGenderSelect_Click );

            editMemberDataLoad( myMemberId, myAgeGroup );
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

        private void AddButton_Click( object sender, EventArgs e ) {
            if ( !( editMemberId_Validation() ) ) return;
            if ( !( editAgeGroup_Validation() ) ) return;

            if ( !( isAgeGroupChanged ) ) return;
            if ( !(checkSkierEventReg()) ) return;

            isAgeGroupNew = true;
            if ( myImportMemberEntry != null ) {
                ImportMember importMember = new ImportMember( myTourRow );
                if ( myImportMemberEntry.ContainsKey("Div") ) myImportMemberEntry["Div"] = AgeGroupSelect.CurrentValue ;
                if ( myImportMemberEntry.ContainsKey( "AgeGroup" ) ) myImportMemberEntry["AgeGroup"] = AgeGroupSelect.CurrentValue;
                importMember.importMemberFromAwsa( myImportMemberEntry, false, false );
                myReqstStatus = "Added";
                DialogResult = DialogResult.OK;

            } else {
                MemberEntry curMemberEntry = new MemberEntry();
                curMemberEntry.MemberId = editMemberId.Text;
                curMemberEntry.AgeGroup = AgeGroupSelect.CurrentValue;
                curMemberEntry.Note = "Registration added";
                if ( updateMemberData( curMemberEntry ) ) {
                    bool curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, "", "" );
                    if ( curReqstStatus ) {
                        myReqstStatus = "Update";
                    } else {
                        myReqstStatus = "Skipped";
                    }
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void saveButton_Click( object sender, EventArgs e ) {
			if ( !( editMemberId_Validation() ) ) return;
			if ( !(editAgeGroup_Validation()) ) return;
			
			MemberEntry curMemberEntry = new MemberEntry();
			curMemberEntry.MemberId = editMemberId.Text;
			curMemberEntry.AgeGroup = AgeGroupSelect.CurrentValue;
			curMemberEntry.Note = "Registration added";
			
			if ( isMemberIdChanged || isAgeGroupChanged ) {
				if ( isAgeGroupChanged ) {
                    if ( !( checkSkierEventReg() ) ) return;
                    updateMemberKey( curMemberEntry );
                    if ( myImportMemberEntry != null ) {
                        ImportMember importMember = new ImportMember( myTourRow );
                        myImportMemberEntry.Add( "AgeGroup", AgeGroupSelect.CurrentValue );
                        importMember.importMemberFromAwsa( myImportMemberEntry, true, false );
                    }
                
                } else {
					updateMemberKey( curMemberEntry );
				}

			} else {
                if ( myImportMemberEntry != null ) {
                    ImportMember importMember = new ImportMember( myTourRow );
                    importMember.importMemberFromAwsa( myImportMemberEntry, true, false );
                    myReqstStatus = "Updated";
                }
            }

            if ( updateMemberData( curMemberEntry ) ) {
				bool curReqstStatus = myTourEventReg.addTourReg( curMemberEntry, "", "" );
				if ( curReqstStatus ) {
					myReqstStatus = "Updated";
				} else {
					myReqstStatus = "Skipped";
				}
				DialogResult = DialogResult.OK;
			}
		}

		private void updateMemberKey( MemberEntry curMemberEntry ) {
            int rowsProc = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            //Remove all record and update all scores for current tournament
            try {
                curSqlStmt.Append( "Update MemberList Set MemberId = '" + curMemberEntry.MemberId + "' " );
                curSqlStmt.Append( "Where MemberId = '" + myMemberId + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From MemberList Where MemberId = '" + curMemberEntry.MemberId + "')" );
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TourReg Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From TourReg Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update EventReg Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "' " );
                curSqlStmt.Append( "  And not exists (Select 1 From EventReg Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomRecap Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From SlalomRecap Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update SlalomScore Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From SlalomScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickPass Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickPass Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TrickScore Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpRecap Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From JumpRecap Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update JumpScore Set MemberId = '" + curMemberEntry.MemberId + "', AgeGroup = '" + curMemberEntry.AgeGroup + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "' AND AgeGroup = '" + myAgeGroup + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From JumpScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "' AND AgeGroup = '" + curMemberEntry.AgeGroup + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWork Set MemberId = '" + curMemberEntry.MemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From OfficialWork Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update OfficialWorkAsgmt Set MemberId = '" + curMemberEntry.MemberId + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + myMemberId + "'" );
                curSqlStmt.Append( "  And not exists (Select 1 From TrickScore Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "')" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				isMemberIdChanged = false;

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to change member id \n" + excp.Message );
            }
        }

        private bool updateMemberData( MemberEntry curMemberEntry ) {
			string curReadyToSki = "N";

			if ( curMemberEntry.AgeGroup == null || curMemberEntry.AgeGroup.Length == 0 ) {
				if ( !( editAgeGroup_Validation() ) ) return false;
				curMemberEntry.AgeGroup = AgeGroupSelect.CurrentValue;
			}

			if ( !( editFirstName_Validation() ) ) return false;
			curMemberEntry.FirstName = editFirstName.Text;
			if ( !( editLastName_Validation() ) ) return false;
			curMemberEntry.LastName = editLastName.Text;

			if ( !( editGenderSelect_Validation() ) ) return false;
			curMemberEntry.Gender = editGenderSelect.RatingValue;
            String curGenderSelect = (String)editGenderSelect.Tag;

			if ( !( editSkiYearAge_Validation() ) ) return false;
			curMemberEntry.SkiYearAge = Convert.ToByte( editSkiYearAge.Text );

			curMemberEntry.MemberStatus = showMemberStatus.Text;
            if ( curMemberEntry.MemberStatus.ToUpper().Equals( "ACTIVE" ) ) curReadyToSki = "Y";

            if ( !(editState_Validation()) ) return false;
			curMemberEntry.State = editState.Text;
			if ( !( editCity_Validation() ) ) return false;
			curMemberEntry.City = editCity.Text;
			if ( !( editFederation_Validation() ) ) return false;
			curMemberEntry.Federation = (String)editFederation.SelectedValue;

            if ( !( editForeignFederationID_Validation() ) ) return false;
            curMemberEntry.ForeignFederationID = editForeignFederationID.Text;

            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
				curSqlStmt.Append( "Update MemberList " );
				curSqlStmt.Append( " Set FirstName = '" + curMemberEntry.getFirstNameForDB() + "'" );
				curSqlStmt.Append( ", LastName = '" + curMemberEntry.getLastNameForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
                curSqlStmt.Append( ", ForeignFederationID = '" + curMemberEntry.ForeignFederationID + "'" );
                curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge.ToString() );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", MemberStatus = '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", UpdateDate = getdate() " );
				curSqlStmt.Append( " Where MemberId = '" + curMemberEntry.MemberId + "'" );
				int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TourReg " );
				curSqlStmt.Append( " Set SkierName = '" + curMemberEntry.getSkierNameForDB() + "'" );
				curSqlStmt.Append( ", City = '" + curMemberEntry.getCityForDB() + "'" );
				curSqlStmt.Append( ", State = '" + curMemberEntry.State + "'" );
				curSqlStmt.Append( ", Federation = '" + curMemberEntry.Federation + "'" );
                curSqlStmt.Append( ", ForeignFederationID = '" + curMemberEntry.ForeignFederationID + "'" );
                curSqlStmt.Append( ", SkiYearAge = " + curMemberEntry.SkiYearAge.ToString() );
				curSqlStmt.Append( ", Gender = '" + curMemberEntry.Gender + "'" );
				curSqlStmt.Append( ", ReadyToSki = '" + curReadyToSki + "'" );
				curSqlStmt.Append( ", AwsaMbrshpComment = '" + curMemberEntry.MemberStatus + "'" );
				curSqlStmt.Append( ", LastUpdateDate = getdate() " );
				curSqlStmt.Append( " Where SanctionId = '" + mySanctionNum + "' AND MemberId = '" + curMemberEntry.MemberId + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				isDataModified = false;
				return true;
            
			} catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to update member data \n" + excp.Message );
				return false;
            }
        }

        private void cancelButton_Click( object sender, EventArgs e ) {
            isDataModified = false;
            myReqstStatus = "Cancelled";
        }

		private void editMemberDataLoad(String inMemberId, String inAgeGroup) {
			this.Text = "Edit Member Registration";
            if ( myImportMemberEntry != null ) {
                loadMemberImportData();
                showMemberRegList();
                return;
            }

            DataRow curDataRow = getMemberTourReg( inMemberId, inAgeGroup );
			if ( curDataRow != null ) {
                loadMemberTourReg( curDataRow, inAgeGroup );
                showMemberRegList();
                return;
            }

            this.Text = "Add Registration for Member";
            curDataRow = getMemberData( myMemberId );
            if ( curDataRow != null ) {
                loadMemberTourReg( curDataRow, inAgeGroup );
                showMemberRegList();
                return;
            }

            MessageBox.Show( "Member entry not found in tournament registration or local member list " + inMemberId + " in division " + inAgeGroup );
            return;
        }

        private void loadMemberTourReg( DataRow curDataRow, String inAgeGroup ) {
            editMemberId.Text = (String)curDataRow["MemberId"];
            string curSkierName = HelperFunctions.getDataRowColValue( curDataRow, "SkierName", "" );
            string curFirstName = HelperFunctions.getDataRowColValue( curDataRow, "FirstName", "" );
            string curLastName = HelperFunctions.getDataRowColValue( curDataRow, "LastName", "" );
            if ( curSkierName.Length == 0 ) curSkierName = curLastName + ", " + curFirstName;
            if ( curFirstName.Length > 0 && curLastName.Length > 0 ) {
                editFirstName.Text = curFirstName;
                editLastName.Text = curLastName;

            } else {
                String[] curNameList = curSkierName.Split( ',' );
                if ( curNameList.Length > 1 ) {
                    editFirstName.Text = curNameList[1].Trim();
                    editLastName.Text = curNameList[0].Trim();

                } else {
                    editFirstName.Text = "";
                    editLastName.Text = curNameList[0].Trim();
                }
            }

            String curAgeGroup = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
            if ( curAgeGroup.Length == 0 ) curAgeGroup = inAgeGroup;

            editGenderSelect.RatingValue = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
            if ( editGenderSelect.RatingValue.Length == 0 && curAgeGroup.Length > 1 ) {
                editGenderSelect.RatingValue = myTourEventReg.getGenderOfAgeDiv( curAgeGroup );
            }

            editSkiYearAge.Text = "";
            Byte curValue = Convert.ToByte( HelperFunctions.getDataRowColValue( curDataRow, "SkiYearAge", "0" ) );
            editSkiYearAge.Text = curValue.ToString();
            if ( curValue > 1 && curValue < 100 ) {
                if ( editGenderSelect.RatingValue.Length > 0 ) {
                    AgeGroupSelect.SelectList_Load( curValue, editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                } else {
                    AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                }

            } else {
                if ( editGenderSelect.RatingValue.Length > 0 ) {
                    AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                } else {
                    AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                }
                editSkiYearAge.Text = "";
            }
            AgeGroupSelect.CurrentValue = curAgeGroup;

            editCity.Text = HelperFunctions.getDataRowColValue( curDataRow, "City", "" );
            editState.Text = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
            editFederation.SelectedValue = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" ).ToLower();
            editForeignFederationID.Text = HelperFunctions.getDataRowColValue( curDataRow, "ForeignFederationID", "" );
            showMemberStatus.Text = HelperFunctions.getDataRowColValue( curDataRow, "MemberStatus", "Inactive" );
            AddButton.Visible = false;
            AddButton.Enabled = false;
        }

        private void loadMemberImportData() {
            String curMemberId = "";
            if ( myImportMemberEntry.ContainsKey( "MemberID" ) ) {
                curMemberId = HelperFunctions.getAttributeValue( myImportMemberEntry, "MemberID" );
            } else {
                curMemberId = HelperFunctions.getAttributeValue( myImportMemberEntry, "MemberId" );
            }
            editMemberId.Text = curMemberId.Length == 11 ? curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 ) : curMemberId;
            if ( !( myMemberIdValidate.checkMemberId( editMemberId.Text ) ) ) {
                MessageBox.Show( String.Format( "Invalid member id {0}, checksum validation failed", editMemberId.Text ) );
                return;
            }
            editFirstName.Text = (String)myImportMemberEntry["FirstName"];
            editLastName.Text = (String)myImportMemberEntry["LastName"];
            editFederation.SelectedValue = HelperFunctions.getAttributeValue( myImportMemberEntry, "Federation" ).ToLower();
            editForeignFederationID.Text = HelperFunctions.getAttributeValue( myImportMemberEntry, "ForeignFederationID" );
            editCity.Text = HelperFunctions.getAttributeValue( myImportMemberEntry, "City" );
            editState.Text = HelperFunctions.getAttributeValue( myImportMemberEntry, "State" );

            String curAgeGroup = "";
            if ( myImportMemberEntry.ContainsKey( "AgeGroup" ) ) {
                curAgeGroup = HelperFunctions.getAttributeValue( myImportMemberEntry, "AgeGroup" ).ToUpper();
            } else if ( myImportMemberEntry.ContainsKey( "Div" ) ) {
                curAgeGroup = HelperFunctions.getAttributeValue( myImportMemberEntry, "Div" ).ToUpper();
            }

            if ( myImportMemberEntry.ContainsKey( "Gender" ) ) {
                editGenderSelect.RatingValue = (String)myImportMemberEntry["Gender"];
            } else if ( curAgeGroup.Length > 1 ) {
                editGenderSelect.RatingValue = myTourEventReg.getGenderOfAgeDiv( curAgeGroup );
            }

            Byte curSkiYearValue = 0;
            editSkiYearAge.Text = "";
            if ( myImportMemberEntry.ContainsKey( "SkiYearAge" ) ) {
                curSkiYearValue = Convert.ToByte( HelperFunctions.getAttributeValueNum( myImportMemberEntry, "SkiYearAge" ) );

            } else if ( myImportMemberEntry.ContainsKey( "Age" ) ) {
                if ( myImportMemberEntry["Age"].GetType() == System.Type.GetType( "System.String" ) ) {
                   String tempSkiYearAge = (String)myImportMemberEntry["Age"];
                   if ( tempSkiYearAge.Length > 0 ) {
                        int numCk;
                        if ( int.TryParse( tempSkiYearAge, out numCk ) ) {
                            if ( numCk < 0 ) curSkiYearValue = 0;
                            else curSkiYearValue = Convert.ToByte( numCk );
                        } else {
                            curSkiYearValue = 0;
                        }
                    } else {
                        curSkiYearValue = 0;
                    }

                } else if ( myImportMemberEntry["Age"].GetType() == System.Type.GetType( "System.Int32" ) ) {
                    curSkiYearValue = Convert.ToByte( (int)myImportMemberEntry["Age"] );
                }
            }
            editSkiYearAge.Text = curSkiYearValue.ToString();
            if ( curSkiYearValue > 1 && curSkiYearValue < 100 ) {
                if ( editGenderSelect.RatingValue.Length > 0 ) {
                    AgeGroupSelect.SelectList_Load( curSkiYearValue, editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                } else {
                    AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                }

            } else {
                if ( editGenderSelect.RatingValue.Length > 0 ) {
                    AgeGroupSelect.SelectList_Load( editGenderSelect.RatingValue, myTourRow, AgeGroupSelect_Change );
                } else {
                    AgeGroupSelect.SelectList_Load( AgeGroupSelect_Change );
                }
                editSkiYearAge.Text = "";
            }

            AgeGroupSelect.CurrentValue = curAgeGroup;

            bool curMemberCanSki = true;
            bool curMemberCanSkiGR = true;
            bool curMemberWaiver = true;
            if ( myImportMemberEntry.ContainsKey( "CanSki" ) ) {
                if ( myImportMemberEntry["CanSki"] != null ) {
                    if ( myImportMemberEntry["CanSki"].GetType() == System.Type.GetType( "System.String" ) ) {
                        if ( ( (String)myImportMemberEntry["CanSki"] ).ToLower().Equals( "true" ) ) curMemberCanSki = true;
                    } else {
                        curMemberCanSki = (Boolean)myImportMemberEntry["CanSki"];
                    }
                }
            }
            if ( myImportMemberEntry.ContainsKey( "CanSkiGR" ) ) {
                if ( myImportMemberEntry["CanSkiGR"] != null ) {
                    if ( myImportMemberEntry["CanSkiGR"].GetType() == System.Type.GetType( "System.String" ) ) {
                        if ( ( (String)myImportMemberEntry["CanSkiGR"] ).ToLower().Equals( "true" ) ) curMemberCanSkiGR = true;
                    } else {
                        curMemberCanSkiGR = (Boolean)myImportMemberEntry["CanSkiGR"];
                    }
                }
            }
            if ( myImportMemberEntry.ContainsKey( "Waiver" ) ) {
                if ( myImportMemberEntry["Waiver"].GetType() == System.Type.GetType( "System.String" ) ) {
                    curMemberWaiver = HelperFunctions.isValueTrue( (String)myImportMemberEntry["Waiver"] );
                } else {
                    curMemberWaiver = HelperFunctions.isValueTrue( ( (int)myImportMemberEntry["Waiver"] ).ToString() );
                }
            }

            DateTime curMemberExpireDate = new DateTime();
            if (myImportMemberEntry.ContainsKey("MembershipExpiration")) {
                curMemberExpireDate = Convert.ToDateTime(HelperFunctions.getAttributeValue(myImportMemberEntry, "MembershipExpiration"));
            
            } else if (myImportMemberEntry.ContainsKey("EffTo")) {
                curMemberExpireDate = Convert.ToDateTime(HelperFunctions.getAttributeValue(myImportMemberEntry, "EffTo"));
            }
            showMemberStatus.Text = ImportMember.calcMemberStatus(
                HelperFunctions.getAttributeValue( myImportMemberEntry, "MemTypeDesc" )
                , curMemberExpireDate
                , HelperFunctions.getAttributeValue( myImportMemberEntry, "membershipStatusCode" )
                , curMemberCanSki
                , curMemberCanSkiGR
                , curMemberWaiver
                , Convert.ToDateTime( myTourRow["EventDates"] ) );

            AddButton.Visible = false;
            AddButton.Enabled = false;
        }

        private void showMemberRegList() {
            DataTable curDataTable = getAvailableMemberTourReg(myMemberId);
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
                        curViewRow.Cells["City"].Value = HelperFunctions.getDataRowColValue( curDataRow, "City", "" );
                        curViewRow.Cells["State"].Value = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
                        curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
                        curViewRow.Cells["SkiYearAge"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SkiYearAge", "" );
                        curViewRow.Cells["Gender"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
                        curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                        curViewRow.Cells["ReadyToSki"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ReadyToSki", "" );
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
                editAgeGroup_Validation();
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
                if ( myAgeGroup != null && myAgeGroup.Length > 0 ) {
                    if (!( myAgeGroup.Equals( AgeGroupSelect.CurrentValue.Trim().Substring( 0, 2 ) ) ) ) {
                        isAgeGroupChanged = true;
                        AddButton.Visible = true;
                        AddButton.Enabled = true;
                    }
            
                } else {
					isAgeGroupChanged = true;
                    AddButton.Visible = true;
                    AddButton.Enabled = true;
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
                curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if (curDataTable == null) {
                    curReturnStatus = true;
                } else {
                    curReturnStatus = true;
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT ListCode, CodeValue, ListCodeNum, SortSeq FROM CodeValueList WHERE ListName = 'AWSAAgeGroup' AND CodeValue = '" + AgeGroupSelect.CurrentValue + "'" );
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
					if (curDataTable.Rows.Count > 0) {
                        DataRow[] curFindClassERow, curFindEventClassRow;
                        DataRow curClassERow, curTourClassRow;
                        int curClassEValue = 0, curEventClassValue = 0;
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "SELECT ListCode, CodeValue, ListCodeNum, SortSeq FROM CodeValueList WHERE ListName = 'ClassTour' ORDER BY SortSeq" );
                        DataTable curTourClassDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
                curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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

        private bool editForeignFederationID_Validation() {
            if ( HelperFunctions.isObjectEmpty( editForeignFederationID.Text ) ) return true;
            if ( !( Regex.IsMatch( editForeignFederationID.Text, "^[0-9]+$" ) ) ) {
                MessageBox.Show( "ForeignFederationID is not valid.  Must be numeric" );
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

		private DataRow getMemberData( String inMemberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT MemberId, LastName, FirstName, State, City" );
			curSqlStmt.Append( ", Country, SkiYearAge, Federation, ForeignFederationID, Gender, MemberStatus, MemberExpireDate" );
			curSqlStmt.Append( ", InsertDate, UpdateDate " );
			curSqlStmt.Append( "From MemberList " );
			curSqlStmt.Append( "Where MemberId = '" + inMemberId + "'" );

			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

		private DataRow getMemberTourReg(String inMemberId, String inAgeGroup) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.MemberId, TR.SkierName, ML.LastName, ML.FirstName, TR.City, TR.State" );
			curSqlStmt.Append( ", TR.Federation, Coalesce(TR.ForeignFederationID, ML.ForeignFederationID) AS ForeignFederationID" );
            curSqlStmt.Append( ", TR.SkiYearAge, TR.AgeGroup, TR.Gender, TR.ReadyToSki" );
            curSqlStmt.Append(", Coalesce(AwsaMbrshpComment, ML.MemberStatus) AS MemberStatus, Coalesce(ML.MemberExpireDate, '') AS MemberExpireDate ");
			curSqlStmt.Append( "FROM TourReg TR" );
			curSqlStmt.Append( "  LEFT OUTER JOIN MemberList ML ON ML.MemberId = TR.MemberId " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND TR.MemberId = '" + inMemberId + "' AND TR.AgeGroup = '" + inAgeGroup + "'" );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

		private DataTable getAvailableMemberTourReg( String inMemberId ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT TR.MemberId, TR.SkierName, TR.City, TR.State" );
			curSqlStmt.Append( ", TR.Federation, TR.SkiYearAge, TR.AgeGroup, TR.Gender, TR.ReadyToSki " );
			curSqlStmt.Append( "FROM TourReg TR " );
			curSqlStmt.Append( "WHERE TR.SanctionId = '" + mySanctionNum + "' AND TR.MemberId = '" + inMemberId + "' " );
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

	}
}
