using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Admin;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Externalnterface;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class TourRegAddMember : Form {
        private String mySanctionNum;
        private bool myDataModified;
		private char[] mySingleQuoteDelim = new char[] { '\'' };

		private DataTable myMemberListDataTable;
		private DataRow myTourRow = null;

		private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EditRegMember myEditRegMemberDialog;
        private EditMember myEditMemberDialog;
		private String myInputMemberId = "";

		public TourRegAddMember() {
            InitializeComponent();
        }

		public void resetInput() {
			inputFirstName.Text = "";
			inputLastName.Text = "";
			inputMemberId.Text = "";
			myInputMemberId = "";
			inputState.Text = "";
			AddButton.Text = "Add";
		}

		public bool isDataModified {
            get {
                return myDataModified;
            }
        }

		public void setInputMemberId( String inputValue ) {
			myInputMemberId = inputValue;
			AddButton.Text = "Update";
		}

		private void TourRegAddMember_Load( object sender, EventArgs e ) {
			mySanctionNum = Properties.Settings.Default.AppSanctionNum;
			if ( mySanctionNum == null ) {
				MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
				this.Close();
			} else {
				if ( mySanctionNum.Length < 6 ) {
					MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
					this.Close();

				} else {
					myDataModified = false;
					DataGridView.Rows.Clear();

					myTourRow = getTourData();
				}
			}

			sortDialogForm = new SortDialogForm();
			sortDialogForm.ColumnList = DataGridView.Columns;

			filterDialogForm = new Common.FilterDialogForm();
			filterDialogForm.ColumnList = DataGridView.Columns;

			myEditRegMemberDialog = new EditRegMember();
			myEditMemberDialog = new EditMember();

			//Initialize search fields and set starting position
			inputLastName.Text = "";
			inputFirstName.Text = "";
			inputMemberId.Text = "";
			inputState.Text = "";
			inputLastName.Focus();

			String curSanctionEditCode = (String) myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				localSearchLoc.Checked = true;
			} else {
				usawsSearchLoc.Checked = true;
			}

			if ( myInputMemberId.Length > 0 ) {
				inputMemberId.Text = myInputMemberId;
				SearchButton_Click(null, null);
			}
		}

		private void TourRegAddMember_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void AddButton_Click(object sender, EventArgs e) {
            String curReqstStatus = "";
			int rowIdx = 0;
			Int16 rowsAdded = 0, rowsSkipped = 0;

			if ( DataGridView.CurrentRow == null ) return;

			Dictionary<string, object> curMemberDataFromAwsa = null;

			DataGridView.CurrentRow.Selected = true;
			DataGridViewSelectedRowCollection selectedRows = DataGridView.SelectedRows;
			foreach ( DataGridViewRow curViewRow in selectedRows ) {
				curMemberDataFromAwsa = null;
				String curMemberIdSelected = (String)curViewRow.Cells["MemberId"].Value;
				String curMemberAgeGroupSelected = (String)curViewRow.Cells["Div"].Value;
				if ( usawsSearchLoc.Checked ) curMemberDataFromAwsa = findAwsaMemberData( curMemberIdSelected, curMemberAgeGroupSelected );

				/*
				 * Processed selected skier to add to the tournament registration
				 */
				myEditRegMemberDialog.editMember( curMemberIdSelected, curMemberAgeGroupSelected, curMemberDataFromAwsa );
				myEditRegMemberDialog.ShowDialog();
				curReqstStatus = myEditRegMemberDialog.ReqstStatus;
				if ( curReqstStatus.Equals( "Added" ) ) {
					rowsAdded++;
					myDataModified = true;
				} else if ( curReqstStatus.Equals( "Updated" ) ) {
					rowsAdded++;
					myDataModified = true;
				} else {
					rowsSkipped++;
				}
				rowIdx++;
			}
			String addMsg = "";
			if ( rowsAdded > 1 ) {
				addMsg += ": " + rowsAdded.ToString() + " skiers added";
			} else if ( rowsAdded == 1 ) {
				addMsg += ": " + rowsAdded.ToString() + " skier added";
			}
			if ( rowsSkipped > 1 ) {
				addMsg += ": " + rowsSkipped.ToString() + " skiers already registered";
			} else if ( rowsSkipped == 1 ) {
				addMsg += ": " + rowsSkipped.ToString() + " skier already registered";
			}
			AddSkierMsg.Text = addMsg;
		}

		/*
		 * Search imported records to find the matching record to the selected skier currently being processed
		 */
		private Dictionary<string, object> findAwsaMemberData( String curMemberIdSelected, String curMemberAgeGroupSelected ) {
			foreach ( DataRow curDataRow in myMemberListDataTable.Rows ) {
				String curMemberId = "", curAgeGroup = ""; ;
				if ( myMemberListDataTable.Columns.Contains( "MemberID" ) ) {
					curMemberId = (String)curDataRow["MemberID"];
				} else {
					curMemberId = (String)curDataRow["MemberId"];
				}
				if ( curMemberId.Length == 11 ) {
					curMemberId = curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 );
				}

				curAgeGroup = HelperFunctions.getDataRowColValue( curDataRow, "Div", "" );
				if ( curAgeGroup.Length == 0 ) curAgeGroup = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );

				/*
				 * Add or update member information for selected skier using imported data
				 */
				if ( curMemberId.Equals( curMemberIdSelected ) && curAgeGroup.Equals( curMemberAgeGroupSelected ) ) {
					return SendMessageHttp.convertDataRowToDictionary( curDataRow );
				}
			}

			return null;
		}

		private void SearchButton_Click(object sender, EventArgs e) {
			myMemberListDataTable = null;
			DataGridView.Rows.Clear();

			if ( localSearchLoc.Checked ) {
				/*
				 * Search local MemberList table for skiers matching provided search criteria
				 */
				myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );

			} else {
				/*
				 * Search USA Water Ski MemberList for skiers matching provided search criteria
				 */
				myMemberListDataTable = sendRequest( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
            }

			/*
			 * Display skiers found matching search criteria
			 */
			if ( myMemberListDataTable == null ) {
				MessageBox.Show( "No records found for specified search criteria" );
			} else { 
                loadDataGridView();
            }
		}

		private void ExportButton_Click(object sender, EventArgs e) {
			ExportData myExportData = new ExportData();
			myExportData.exportData(DataGridView);
		}

		private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            Cursor.Current = Cursors.WaitCursor;
			String curDataValue = "";
			int curRowIdx;
            DateTime curEffTo, curTourDate;
			DataGridView.Rows.Clear();

			if ( myTourRow != null ) {
				curTourDate = Convert.ToDateTime( myTourRow["EventDates"] );

			} else {
				MessageBox.Show( "Tournament data is not available, processing can't be completed" );
				return;
			}

			if ( myMemberListDataTable.Rows.Count > 0 ) {
				DataGridViewRow curViewRow;
				foreach ( DataRow curDataRow in myMemberListDataTable.Rows ) {
					curRowIdx = DataGridView.Rows.Add();
					curViewRow = DataGridView.Rows[curRowIdx];

					#region Member record information common to all data sources
					curViewRow.Cells["LastName"].Value = (String) curDataRow["LastName"];
					curViewRow.Cells["FirstName"].Value = (String) curDataRow["FirstName"];
					try {
						curViewRow.Cells["City"].Value = (String) curDataRow["City"];
					} catch {
						curViewRow.Cells["City"].Value = "";
					}
					try {
						curViewRow.Cells["State"].Value = (String) curDataRow["State"];
					} catch {
						curViewRow.Cells["State"].Value = "";
					}
                    try {
						curViewRow.Cells["Federation"].Value = (String) curDataRow["Federation"];
					} catch {
						curViewRow.Cells["Federation"].Value = "";
					}
					try {
						curViewRow.Cells["Gender"].Value = (String) curDataRow["Gender"];
					} catch {
						curViewRow.Cells["Gender"].Value = "";
					}
					try {
						curViewRow.Cells["JudgeSlalom"].Value = (String) curDataRow["JudgeSlalom"];
					} catch {
						curViewRow.Cells["JudgeSlalom"].Value = "";
					}
					try {
						curViewRow.Cells["JudgeTrick"].Value = (String) curDataRow["JudgeTrick"];
					} catch {
						curViewRow.Cells["JudgeTrick"].Value = "";
					}
					try {
						curViewRow.Cells["JudgeJump"].Value = (String) curDataRow["JudgeJump"];
					} catch {
						curViewRow.Cells["JudgeJump"].Value = "";
					}
					try {
						curViewRow.Cells["DriverSlalom"].Value = (String) curDataRow["DriverSlalom"];
					} catch {
						curViewRow.Cells["DriverSlalom"].Value = "";
					}
					try {
						curViewRow.Cells["DriverTrick"].Value = (String) curDataRow["DriverTrick"];
					} catch {
						curViewRow.Cells["DriverTrick"].Value = "";
					}
					try {
						curViewRow.Cells["DriverJump"].Value = (String) curDataRow["DriverJump"];
					} catch {
						curViewRow.Cells["DriverJump"].Value = "";
					}
					try {
						curViewRow.Cells["ScorerSlalom"].Value = (String) curDataRow["ScorerSlalom"];
					} catch {
						curViewRow.Cells["ScorerSlalom"].Value = "";
					}
					try {
						curViewRow.Cells["ScorerTrick"].Value = (String) curDataRow["ScorerTrick"];
					} catch {
						curViewRow.Cells["ScorerTrick"].Value = "";
					}
					try {
						curViewRow.Cells["ScorerJump"].Value = (String) curDataRow["ScorerJump"];
					} catch {
						curViewRow.Cells["ScorerJump"].Value = "";
					}
					try {
						curViewRow.Cells["Safety"].Value = (String) curDataRow["Safety"];
					} catch {
						curViewRow.Cells["Safety"].Value = "";
					}
					try {
						curViewRow.Cells["TechController"].Value = (String) curDataRow["TechController"];
					} catch {
						curViewRow.Cells["TechController"].Value = "";
					}
					#endregion

					if ( localSearchLoc.Checked ) {
						#region Member record retrieved from local member list

						try {
							curViewRow.Cells["MemberId"].Value = (String) curDataRow["MemberId"];
						} catch {
							curViewRow.Cells["MemberId"].Value = "";
						}

						try {
							curEffTo = (DateTime) curDataRow["MemberExpireDate"];
							curViewRow.Cells["EffTo"].Value = curEffTo.ToString( "MM/dd/yy" );

							if ( curEffTo < curTourDate ) {
								curViewRow.Cells["MemberStatus"].Value = "Needs Upgrade";
								curViewRow.Cells["SkiYearAge"].Value = "";
								curViewRow.Cells["CanSki"].Value = "false";
							} else {
								curViewRow.Cells["MemberStatus"].Value = (String) curDataRow["MemberStatus"];
								curViewRow.Cells["CanSki"].Value = "true";
								try {
									curViewRow.Cells["SkiYearAge"].Value = ((byte) curDataRow["SkiYearAge"]).ToString();
								} catch {
									curViewRow.Cells["SkiYearAge"].Value = "";
								}
							}
						} catch {
							curViewRow.Cells["EffTo"].Value = "";
							curViewRow.Cells["MemberStatus"].Value = "";
							curViewRow.Cells["SkiYearAge"].Value = "";
						}
						#endregion

					} else {
						#region Member record retrieved from USA Water Ski
						String curMemberId = HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );
						if ( curMemberId.Length > 10) {
							curViewRow.Cells["MemberId"].Value = curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 );
						} else {
							curViewRow.Cells["MemberId"].Value = curMemberId;
						}
						curViewRow.Cells["SkiYearAge"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Age", "0" );
						curViewRow.Cells["Div"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Div", "" );
						curViewRow.Cells["OffCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "OffCode", "" );
						curViewRow.Cells["SlalomRank"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomRank", "" );
						curViewRow.Cells["TrickRank"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickRank", "" );
						curViewRow.Cells["JumpRank"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpRank", "" );
						curViewRow.Cells["SlalomRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomRating", "" );
						curViewRow.Cells["TrickRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickRating", "" );
						curViewRow.Cells["JumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpRating", "" );
						curViewRow.Cells["OverallRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "OverallRating", "" );
						curViewRow.Cells["SlmQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlmQfy", "" );
						curViewRow.Cells["TrkQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrkQfy", "" );
						curViewRow.Cells["JmpQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JmpQfy", "" );
						curViewRow.Cells["TtrickBoat"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TtrickBoat", "" );
						curViewRow.Cells["JumpRamp"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpRamp", "" );
						
						curViewRow.Cells["Prereg"].Value = false;
						curDataValue = HelperFunctions.getDataRowColValue( curDataRow, "Prereg", "" ).ToLower();
						if ( curDataValue.Equals( "yes" ) ) curViewRow.Cells["Prereg"].Value = true;
						
						curViewRow.Cells["SlalomClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomPaid", "" );
						curViewRow.Cells["TrickClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickPaid", "" );
						curViewRow.Cells["JumpClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpPaid", "" );
						curViewRow.Cells["Memtype"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Memtype", "" );
						curViewRow.Cells["MemCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MemCode", "" );

						/*
						 * Analyze information and determine membership status 
						 */
						bool curCanSki = false;
						curDataValue = HelperFunctions.getDataRowColValue( curDataRow, "CanSki", "false" ).ToLower();
						if ( curDataValue.Equals( "true" ) ) curCanSki = true;
						curViewRow.Cells["CanSki"].Value = curCanSki;

						bool curCanSkiGR = false;
						curDataValue = HelperFunctions.getDataRowColValue( curDataRow, "CanSkiGR", "false" ).ToLower();
						if ( curDataValue.Equals( "true" ) ) curCanSkiGR = true;
						curViewRow.Cells["CanSkiGR"].Value = curCanSkiGR;

						bool curWaiver = false;
						curDataValue = HelperFunctions.getDataRowColValue( curDataRow, "Waiver", "0" );
						if ( curDataValue.Equals( "1" ) ) curWaiver = true;
						curViewRow.Cells["Waiver"].Value = curWaiver;

						DateTime curMemExpireDate = new DateTime();
						try {
							curMemExpireDate = Convert.ToDateTime( HelperFunctions.getDataRowColValue( curDataRow, "EffTo", "" ) );
							curViewRow.Cells["EffTo"].Value = curMemExpireDate.ToString( "MM/dd/yy" );
						} catch ( Exception ex ) {
							Log.WriteFile( String.Format( "Invalid EffTo date {0} attribute on import record: Exceptioin: {1}"
								, HelperFunctions.getDataRowColValue( curDataRow, "EffTo", "" ), ex.Message ) );
						}

						curViewRow.Cells["MembershipRate"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MembershipRate", "" );
						curViewRow.Cells["CostToUpgrade"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CostToUpgrade", "" );

						curViewRow.Cells["MemberStatus"].Value = ImportMember.calcMemberStatus(
							HelperFunctions.getDataRowColValue( curDataRow, "MemTypeDesc", "" )
							, curMemExpireDate
							, HelperFunctions.getDataRowColValue( curDataRow, "membershipStatusCode", "" )
							, curCanSki
							, curCanSkiGR
							, curWaiver
							, Convert.ToDateTime( myTourRow["EventDates"] ) );
						
						#endregion
					}
				}
			} else {
				MessageBox.Show( "No records found for specified search criteria" );
			}
            Cursor.Current = Cursors.Default;
        }
        
        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView.CurrentRow.Selected = true;
        }

		private void DataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
			int curRow = e.RowIndex + 1;
			RowStatusLabel.Text = "Row " + curRow.ToString() + " of " + DataGridView.Rows.Count.ToString();

		}

		private void newMemberButton_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            myEditMemberDialog.editMember( null );
            myEditMemberDialog.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( myEditMemberDialog.DialogResult == DialogResult.OK ) {
				localSearchLoc.Checked = true;
				myMemberListDataTable = searchMemberList(myEditMemberDialog.MemberId, null, null, null);
                if ( myMemberListDataTable != null ) {
                    loadDataGridView();
                    DataGridView.Rows[0].Selected = true;
                    AddButton_Click(null, null);
                }
            }
        }

        private DataTable searchMemberList( String inMemberId ) {
            if ( inMemberId.Length > 0 ) {
                return searchMemberList( inMemberId, null, null, null );
            } else {
                return null;
            }
        }
        private DataTable searchMemberList( String inMemberId, String inLastName, String inFirstName, String inState ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, LastName, FirstName, State, City" );
			curSqlStmt.Append( ", Country, SkiYearAge, Federation, Gender, MemberStatus, MemberExpireDate" );
			curSqlStmt.Append( ", Coalesce( MemberList.JudgeSlalomRating, '' ) as JudgeSlalom" );
			curSqlStmt.Append( ", Coalesce( MemberList.JudgeTrickRating, '' ) as JudgeTrick" );
			curSqlStmt.Append( ", Coalesce( MemberList.JudgeJumpRating, '' ) as JudgeJump" );
			curSqlStmt.Append( ", Coalesce( MemberList.DriverSlalomRating, '' ) as DriverSlalom" );
			curSqlStmt.Append( ", Coalesce( MemberList.DriverTrickRating, '' ) as DriverTrick" );
			curSqlStmt.Append( ", Coalesce( MemberList.DriverJumpRating, '' ) as DriverJump" );
			curSqlStmt.Append( ", Coalesce( MemberList.ScorerSlalomRating, '' ) as ScorerSlalom" );
			curSqlStmt.Append( ", Coalesce( MemberList.ScorerTrickRating, '' ) as ScorerTrick" );
			curSqlStmt.Append( ", Coalesce( MemberList.ScorerJumpRating, '' ) as ScorerJump" );
			curSqlStmt.Append( ", Coalesce( MemberList.SafetyOfficialRating, '' ) as Safety" );
			curSqlStmt.Append( ", Coalesce( MemberList.TechOfficialRating, '' ) as TechController" );
			curSqlStmt.Append( ", Coalesce( MemberList.AnncrOfficialRating, '' ) as AnncrOfficial " );
			curSqlStmt.Append( ", InsertDate, UpdateDate " );

			String curLastName = "";
			if ( inLastName != null ) {
				curLastName = HelperFunctions.stringReplace( inLastName, mySingleQuoteDelim, "''" );
			}
			String curFirstName = "";
			if ( inFirstName != null ) {
				curFirstName = HelperFunctions.stringReplace( inFirstName, mySingleQuoteDelim, "''" );
			}
			String curState = "";
			if ( inState != null ) {
				curState = inState;
			}

			curSqlStmt.Append( "FROM MemberList " );
            if ( inMemberId.Length > 0 ) {
                curSqlStmt.Append( "Where MemberId = '" + inMemberId + "'" );

			} else {
				if ( curLastName.Length > 0 || curFirstName.Length > 0 ) {
					curSqlStmt.Append( "Where LastName like '" + curLastName + "%'" );
					curSqlStmt.Append( "  And FirstName like '" + curFirstName + "%'" );
					if ( curState.Length > 0 ) {
						curSqlStmt.Append( "  And State = '" + curState + "'" );
					}

				} else {
					if ( curState.Length > 0 ) {
						curSqlStmt.Append( "Where State = '" + curState + "'" );
					}
				}
			}
            curSqlStmt.Append( " Order by LastName, FirstName, SkiYearAge" );

			return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private DataTable sendRequest( String inMemberId, String inLastName, String inFirstName, String inState ) {
			String curMethodName = "sendRequest";
			String curContentType = "application/json; charset=UTF-8";
			String curMemberExportListUrl = "https://usawaterski.org/admin/GetMemberRegExportJson.asp";
			String curMemberSearchListUrl = "https://usawaterski.org/admin/GetMemberExportJson.asp";
			String curExportUrl = "";

			/* -----------------------------------------------------------------------
            * Validate TourID value for scores to be Exported.
            * https://usawaterski.org/admin/GetMemberRegExportJson.asp?SanctionId=18E014&MemberId=700040630
            * https://usawaterski.org/admin/GetMemberRegExportJson.asp?SanctionId=18E014&FirstName=David&LastName=Allen
            *
            *HTTP_AUTHORIZATION:Basic wstims:Slalom38tTrick13Jump250\nHTTP_HOST:www.usawaterski.org\nHTTP_USER_AGENT:.NET Framework CustomUserAgent Water Ski Scoring
            ----------------------------------------------------------------------- */

			StringBuilder curQueryString = new StringBuilder( "" );
			if (inState.Equals("bypass") && inState.Length > 0 && inMemberId.Length == 0 && inFirstName.Length == 0 && inLastName.Length == 0) {
				curExportUrl = curMemberSearchListUrl;
				curQueryString.Append("?State=" + inState);
			} else {
				curExportUrl = curMemberExportListUrl;
				curQueryString.Append("?SanctionId=" + mySanctionNum);
				if (inMemberId.Length > 0) {
					curQueryString.Append("&MemberId=" + inMemberId);
				}
				if (inFirstName.Length > 0 || inLastName.Length > 0) {
					curQueryString.Append("&FirstName=" + inFirstName + "&LastName=" + inLastName);
				}
				if (inState.Length > 0) {
					curQueryString.Append("&State=" + inState);
				}
			}

			String curReqstUrl = curExportUrl + curQueryString.ToString();
			String curSanctionEditCode = (String) myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve officials and ratings.  Enter required value on Tournament Form" );
				return null;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			Cursor.Current = Cursors.WaitCursor;
			DataTable curDataTable = SendMessageHttp.getMessageResponseDataTable( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if (curDataTable != null ) {
				Cursor.Current = Cursors.Default;
				return curDataTable;

			} else {
				return null;
			}
		}

		private DataRow getTourData() {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "SELECT SanctionId, Name, Class, Federation" );
			curSqlStmt.Append( ", SanctionEditCode, Rules, EventDates, EventLocation, TourDataLoc " );
			curSqlStmt.Append( "FROM Tournament " );
			curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
			DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				return curDataTable.Rows[0];
			} else {
				return null;
			}
		}

	}
}
