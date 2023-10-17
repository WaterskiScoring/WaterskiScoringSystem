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
					curViewRow.Cells["City"].Value = HelperFunctions.getDataRowColValue( curDataRow, "City", "" );
					curViewRow.Cells["State"].Value = HelperFunctions.getDataRowColValue( curDataRow, "State", "" );
					curViewRow.Cells["Federation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
					curViewRow.Cells["ForeignFederationID"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ForeignFederationID", "" );

					curViewRow.Cells["Gender"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Gender", "" );
					curViewRow.Cells["JudgeSlalom"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeSlalom", "" );
					curViewRow.Cells["JudgeTrick"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeTrick", "" );
					curViewRow.Cells["JudgeJump"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JudgeJump", "" );
					curViewRow.Cells["DriverSlalom"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverSlalom", "" );
					curViewRow.Cells["DriverTrick"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverTrick", "" );
					curViewRow.Cells["DriverJump"].Value = HelperFunctions.getDataRowColValue( curDataRow, "DriverJump", "" );
					curViewRow.Cells["ScorerSlalom"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerSlalom", "" );
					curViewRow.Cells["ScorerTrick"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerTrick", "" );
					curViewRow.Cells["ScorerJump"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ScorerJump", "" );
					curViewRow.Cells["Safety"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Safety", "" );
					curViewRow.Cells["TechController"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TechController", "" );
					#endregion

					if ( localSearchLoc.Checked ) {
						#region Member record retrieved from local member list

						curViewRow.Cells["MemberId"].Value = HelperFunctions.getDataRowColValue( curDataRow, "MemberId", "" );

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
						curViewRow.Cells["OverallRank"].Value = HelperFunctions.getDataRowColValue( curDataRow, "OverallRank", "" );
						curViewRow.Cells["JumpRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpRating", "" );
						curViewRow.Cells["OverallRating"].Value = HelperFunctions.getDataRowColValue( curDataRow, "OverallRating", "" );
						curViewRow.Cells["SlmQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlmQfy", "" );
						curViewRow.Cells["TrkQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrkQfy", "" );
						curViewRow.Cells["JmpQfy"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JmpQfy", "" );
						curViewRow.Cells["TrickBoat"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickBoat", "" );
						curViewRow.Cells["JumpRamp"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpHeight", "" );
						
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
						curViewRow.Cells["membershipStatusCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "membershipStatusCode", "" );
						curViewRow.Cells["membershipStatusText"].Value = HelperFunctions.getDataRowColValue( curDataRow, "membershipStatusText", "" );

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
			curSqlStmt.Append( ", Country, SkiYearAge, Federation, ForeignFederationID, Gender, MemberStatus, MemberExpireDate" );
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
			String curMemberExportListUrl = Properties.Settings.Default.UriUsaWaterski + "/admin/GetMemberRegExportJson.asp";
			String curExportUrl = "";

			StringBuilder curQueryString = new StringBuilder( "" );
			curExportUrl = curMemberExportListUrl;
			String curQueryStringDelim = "?";
			if ( inMemberId.Length > 0 ) {
				curQueryString.Append( curQueryStringDelim + "MemberId=" + inMemberId );
				curQueryStringDelim = "&";
			}
			if ( inFirstName.Length > 0 || inLastName.Length > 0 ) {
				curQueryString.Append( curQueryStringDelim + "FirstName=" + inFirstName + "&LastName=" + inLastName );
				curQueryStringDelim = "&";
			}
			if ( inState.Length > 0 ) {
				curQueryString.Append( curQueryStringDelim + "State=" + inState );
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
