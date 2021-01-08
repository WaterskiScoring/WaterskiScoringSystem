using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class OfficialImport : Form {
        private bool isLoadInProg = false;
        private String mySanctionNum;
        private ImportMember importOfficialRatings;

        private DataRow myTourRow;

        private RegionDropdownList myRegionDropdownList;
        private StateDropdownList myStateDropdownList;

        public OfficialImport() {
            InitializeComponent();
        }

        private void OfficialImport_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.OfficialImport_Width > 0 ) {
                this.Width = Properties.Settings.Default.OfficialImport_Width;
            }
            if ( Properties.Settings.Default.EditRegMember_Height > 0 ) {
                this.Height = Properties.Settings.Default.OfficialImport_Height;
            }
            if ( Properties.Settings.Default.OfficialImport_Location.X > 0
                && Properties.Settings.Default.OfficialImport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.OfficialImport_Location;
            }

            isLoadInProg = true;
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if ( mySanctionNum == null ) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        importOfficialRatings = new ImportMember(myTourRow);

                        myRegionDropdownList = new RegionDropdownList();
                        myStateDropdownList = new StateDropdownList();

                        RegionListBox.DataSource = myRegionDropdownList.DropdownList;
                        RegionListBox.DisplayMember = "ItemName";
                        RegionListBox.ValueMember = "ItemValue";

                        StateListBox.DataSource = myStateDropdownList.DropdownList;
                        StateListBox.DisplayMember = "ItemName";
                        StateListBox.ValueMember = "ItemValue";

                        ProcessSelectionButton.Visible = false;
                        officialImportDataGridView.Visible = false;
                        RegionListBox.Visible = false;
                        StateListBox.Visible = false;

                        memberIdLabel.Visible = false;
                        memberIdTextBox.Visible = false;
                        FirstNameTextBox.Visible = false;
                        firstNameLabel.Visible = false;
                        LastNameTextBox.Visible = false;
                        lastNameLabel.Visible = false;

                    } else {
                        MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                    }

                }

                isLoadInProg = false;
            }
        }

        private void OfficialImport_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.OfficialImport_Width = this.Size.Width;
                Properties.Settings.Default.OfficialImport_Height = this.Size.Height;
                Properties.Settings.Default.OfficialImport_Location = this.Location;
            }

        }

        private void OfficialImport_FormClosing( object sender, FormClosingEventArgs e ) {
            e.Cancel = false;
        }

        private void ExportButton_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            myExportData.exportData(officialImportDataGridView, "OfficialExport.txt");
        }

        private void BySanctionRadioButton_CheckedChanged( object sender, EventArgs e ) {
            officialImportDataGridView.Visible = false;
            RegionListBox.Visible = false;
            StateListBox.Visible = false;

            memberIdLabel.Visible = false;
            memberIdTextBox.Visible = false;

            FirstNameTextBox.Visible = false;
            firstNameLabel.Visible = false;
            LastNameTextBox.Visible = false;
            lastNameLabel.Visible = false;
        }

        private void ByStateRadioButton_CheckedChanged( object sender, EventArgs e ) {
            officialImportDataGridView.Visible = false;
            RegionListBox.Visible = true;
            StateListBox.Visible = true;

            memberIdLabel.Visible = false;
            memberIdTextBox.Visible = false;

            FirstNameTextBox.Visible = false;
            firstNameLabel.Visible = false;
            LastNameTextBox.Visible = false;
            lastNameLabel.Visible = false;
        }

        private void ByNameRadioButton_CheckedChanged( object sender, EventArgs e ) {
            officialImportDataGridView.Visible = false;
            RegionListBox.Visible = false;
            StateListBox.Visible = false;

            memberIdLabel.Visible = false;
            memberIdTextBox.Visible = false;

            FirstNameTextBox.Visible = true;
            firstNameLabel.Visible = true;
            LastNameTextBox.Visible = true;
            lastNameLabel.Visible = true;
        }

        private void ByMemberIdRadioButton_CheckedChanged( object sender, EventArgs e ) {
            officialImportDataGridView.Visible = false;
            RegionListBox.Visible = false;
            StateListBox.Visible = false;

            memberIdLabel.Visible = true;
            memberIdTextBox.Visible = true;

            FirstNameTextBox.Visible = false;
            firstNameLabel.Visible = false;
            LastNameTextBox.Visible = false;
            lastNameLabel.Visible = false;
        }

        private void regionListBox_SelectedIndexChanged( object sender, EventArgs e ) {
            if ( !( isLoadInProg ) ) {
                myStateDropdownList.Region = (String) RegionListBox.SelectedValue;
                StateListBox.DataSource = myStateDropdownList.DropdownList;
            }
        }

        private void GetOfficialsButton_Click( object sender, EventArgs e ) {
            String curQueryString = "";
            officialImportDataGridView.DataSource = null;

            if ( BySanctionRadioButton.Checked ) {
                curQueryString = "?SanctionId=" + mySanctionNum;

            } else if ( ByMemberIdRadioButton.Checked ) {
                curQueryString = "?MemberId=" + memberIdTextBox.Text;
                if ( memberIdTextBox.Text.Length == 0 ) {
                    MessageBox.Show("You must enter a member number to retrieve an official's ratings");
                    return;
                }

            } else if ( ByNameRadioButton.Checked ) {
                //curQueryString = "?FirstName=" + FirstNameTextBox.Text + "&LastName=" + LastNameTextBox.Text + "&SanctionId=" + mySanctionNum;
				curQueryString = "?FirstName=" + FirstNameTextBox.Text + "&LastName=" + LastNameTextBox.Text;
				if ( FirstNameTextBox.Text.Length == 0 && LastNameTextBox.Text.Length == 0 ) {
                    MessageBox.Show("You must enter search criteria in either the first and last name or both");
                    return;
                }

            } else if ( ByStateRadioButton.Checked ) {
                //curQueryString = "?SanctionId=" + mySanctionNum + "&StateList=";
				curQueryString = "?StateList=";
				if ( StateListBox.SelectedItems.Count > 0 ) {
                    int selectCount = 0;
                    foreach ( ListItem curItem in StateListBox.SelectedItems ) {
                        selectCount++;
                        if ( selectCount > 1 ) {
                            curQueryString += "," + (String) curItem.ItemValue;
                        } else {
                            curQueryString += (String) curItem.ItemValue;
                        }
                    }
					
                } else {
                    MessageBox.Show("You must select at least one state for search criteria");
                    return;
                }


            } else {
                curQueryString = "?SanctionId=" + mySanctionNum;
            }

            sendRequest(curQueryString);

        }

        private void ProcessSelectionButton_Click( object sender, EventArgs e ) {
            ProcessSelectionButton.Visible = false;
        }

        private void sendRequest( String curQueryString ) {
            String curMethodName = "sendRequest";
            /* -----------------------------------------------------------------------
            * Validate TourID value for scores to be Exported.
            * http://www.usawaterski.org/admin/GetOfficialExportJson.asp?MemberId=700040630
            * http://www.usawaterski.org/admin/GetOfficialExportJson.asp?SanctionId=18E024
            * http://www.usawaterski.org/admin/GetOfficialExportJson.asp?StateList=MA,CT
            *
            *HTTP_AUTHORIZATION:Basic wstims:Slalom38tTrick13Jump250\nHTTP_HOST:www.usawaterski.org\nHTTP_USER_AGENT:.NET Framework CustomUserAgent Water Ski Scoring
            ----------------------------------------------------------------------- */

            String curContentType = "application/json; charset=UTF-8";
			//String curOfficialExportListUrl = "http://www.usawaterski.org/admin/GetOfficialExportJson.asp";
			String curOfficialExportListUrl = "http://www.usawaterski.org/admin/GetMemberRegExportJson.asp";
			String curReqstUrl = curOfficialExportListUrl + curQueryString;
			String curSanctionEditCode = (String)myTourRow["SanctionEditCode"];
			if ( (curSanctionEditCode == null) || (curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve officials and ratings.  Enter required value on Tournament Form" );
				return;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			Cursor.Current = Cursors.WaitCursor;
			DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable(curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false);
            if (curDataTable != null && curDataTable.Rows.Count > 0 ) {
                officialImportDataGridView.DataSource = curDataTable;

                officialImportDataGridView.Visible = true;
                ProcessSelectionButton.Visible = true;

                Cursor.Current = Cursors.Default;
            }
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation, SanctionEditCode" );
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation");
            curSqlStmt.Append(", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' ");

			return DataAccess.getDataTable( curSqlStmt.ToString() );
		}

    }
}
