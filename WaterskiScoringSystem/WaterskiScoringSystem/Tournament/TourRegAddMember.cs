using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Admin;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class TourRegAddMember : Form {
        private String mySanctionNum;
        private bool myDataModified;

		private DataTable myMemberListDataTable;
		private DataRow myTourRow = null;

		private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EditRegMember myEditRegMemberDialog;
        private EditMember myEditMemberDialog;

		private List<object> myResponseDataList = null;

		public TourRegAddMember() {
            InitializeComponent();
        }

        public bool isDataModified {
            get {
                return myDataModified;
            }
        }

        private void TourRegAddMember_Load(object sender, EventArgs e) {
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if (mySanctionNum == null) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                this.Close();
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
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
        }

        private void TourRegAddMember_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void AddButton_Click(object sender, EventArgs e) {
            String curReqstStatus = "";
			int rowIdx = 0;
			Int16 rowsAdded = 0, rowsSkipped = 0;

            if ( DataGridView.CurrentRow != null ) {

                DataGridView.CurrentRow.Selected = true;
                DataGridViewSelectedRowCollection selectedRows = DataGridView.SelectedRows;
                foreach ( DataGridViewRow curRow in selectedRows ) {

					if ( usawsSearchLoc.Checked ) {
						ImportOfficialRatings importOfficialRatings = new ImportOfficialRatings( myTourRow );
                        importOfficialRatings.importMembersAndRating( (Dictionary<string, object>) myResponseDataList[rowIdx] );
					}

					myEditRegMemberDialog.editMember( (String)curRow.Cells["MemberId"].Value, "" );
                    myEditRegMemberDialog.ShowDialog();
                    curReqstStatus = myEditRegMemberDialog.ReqstStatus;
                    if ( curReqstStatus.Equals( "Added" ) ) {
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
        }

        private void SearchButton_Click(object sender, EventArgs e) {
			myMemberListDataTable = null;
			myResponseDataList = null;

			if ( localSearchLoc.Checked ) {
				myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );

			} else {
				myMemberListDataTable = sendRequest( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
            }

			if ( myMemberListDataTable != null ) {
                loadDataGridView();
            }
		}

		private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            Cursor.Current = Cursors.WaitCursor;
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
						try {
							String curMemberId = (String) curDataRow["MemberId"];
							curViewRow.Cells["MemberId"].Value = curMemberId.Substring( 0, 3 ) + curMemberId.Substring( 4, 2 ) + curMemberId.Substring( 7, 4 );
						} catch {
							curViewRow.Cells["MemberId"].Value = "";
						}
						try {
							curViewRow.Cells["SkiYearAge"].Value = (String) curDataRow["Age"];
						} catch {
							curViewRow.Cells["SkiYearAge"].Value = "";
						}
						try {
							curViewRow.Cells["Div"].Value = (String) curDataRow["Div"];
						} catch {
							curViewRow.Cells["Div"].Value = "";
						}
						try {
							if ( ((String) curDataRow["Waiver"]).Equals( "1" ) ) {
								curViewRow.Cells["Waiver"].Value = true;
							} else {
								curViewRow.Cells["Waiver"].Value = false;
							}
						} catch {
							curViewRow.Cells["Waiver"].Value = "";
						}
						try {
							curViewRow.Cells["OffCode"].Value = (String) curDataRow["OffCode"];
						} catch {
							curViewRow.Cells["OffCode"].Value = "";
						}
						try {
							curViewRow.Cells["SlmSco"].Value = (String) curDataRow["SlmSco"];
						} catch {
							curViewRow.Cells["SlmSco"].Value = "";
						}
						try {
							curViewRow.Cells["TrkSco"].Value = (String) curDataRow["TrkSco"];
						} catch {
							curViewRow.Cells["TrkSco"].Value = "";
						}
						try {
							curViewRow.Cells["JmpSco"].Value = (String) curDataRow["JmpSco"];
						} catch {
							curViewRow.Cells["JmpSco"].Value = "";
						}
						try {
							curViewRow.Cells["SlmRat"].Value = (String) curDataRow["SlmRat"];
						} catch {
							curViewRow.Cells["SlmRat"].Value = "";
						}
						try {
							curViewRow.Cells["TrkRat"].Value = (String) curDataRow["TrkRat"];
						} catch {
							curViewRow.Cells["TrkRat"].Value = "";
						}
						try {
							curViewRow.Cells["JmpRat"].Value = (String) curDataRow["JmpRat"];
						} catch {
							curViewRow.Cells["JmpRat"].Value = "";
						}
						try {
							curViewRow.Cells["OvrRat"].Value = (String) curDataRow["OvrRat"];
						} catch {
							curViewRow.Cells["OvrRat"].Value = "";
						}
						try {
							curViewRow.Cells["SlmQfy"].Value = (String) curDataRow["SlmQfy"];
						} catch {
							curViewRow.Cells["SlmQfy"].Value = "";
						}
						try {
							curViewRow.Cells["TrkQfy"].Value = (String) curDataRow["TrkQfy"];
						} catch {
							curViewRow.Cells["TrkQfy"].Value = "";
						}
						try {
							curViewRow.Cells["JmpQfy"].Value = (String) curDataRow["JmpQfy"];
						} catch {
							curViewRow.Cells["JmpQfy"].Value = "";
						}
						try {
							curViewRow.Cells["TtrickBoat"].Value = (String) curDataRow["TBoat"];
						} catch {
							curViewRow.Cells["TtrickBoat"].Value = "";
						}
						try {
							curViewRow.Cells["JumpRamp"].Value = (String) curDataRow["JRamp"];
						} catch {
							curViewRow.Cells["JumpRamp"].Value = "";
						}
						try {
							if ( ( (String) curDataRow["Prereg"] ).ToLower().Equals( "yes" ) ) {
								curViewRow.Cells["Prereg"].Value = true;
							} else {
								curViewRow.Cells["Prereg"].Value = false;
							}
						} catch {
							curViewRow.Cells["Prereg"].Value = false;
						}
						try {
							curViewRow.Cells["SlalomClass"].Value = (String) curDataRow["SDiv"];
						} catch {
							curViewRow.Cells["SlalomClass"].Value = "";
						}
						try {
							curViewRow.Cells["TrickClass"].Value = (String) curDataRow["TDiv"];
						} catch {
							curViewRow.Cells["TrickClass"].Value = "";
						}
						try {
							curViewRow.Cells["JumpClass"].Value = (String) curDataRow["JDiv"];
						} catch {
							curViewRow.Cells["JumpClass"].Value = "";
						}
						try {
							curViewRow.Cells["SlalomPaid"].Value = (String) curDataRow["SPaid"];
						} catch {
							curViewRow.Cells["SlalomPaid"].Value = "";
						}
						try {
							curViewRow.Cells["TrickPaid"].Value = (String) curDataRow["TPaid"];
						} catch {
							curViewRow.Cells["TrickPaid"].Value = "";
						}
						try {
							curViewRow.Cells["JumpPaid"].Value = (String) curDataRow["JPaid"];
						} catch {
							curViewRow.Cells["JumpPaid"].Value = "";
						}
						try {
							curEffTo = Convert.ToDateTime( (String) curDataRow["EffTo"] );
							curViewRow.Cells["EffTo"].Value = curEffTo.ToString( "MM/dd/yy" );
						} catch {
							curViewRow.Cells["EffTo"].Value = "";
						}
						try {
							curViewRow.Cells["Memtype"].Value = (String) curDataRow["Memtype"];
						} catch {
							curViewRow.Cells["Memtype"].Value = "";
						}
						try {
							curViewRow.Cells["MemCode"].Value = (String) curDataRow["MemCode"];
						} catch {
							curViewRow.Cells["MemCode"].Value = "";
						}
						try {
							if ( ((String) curDataRow["ActiveMember"]).ToLower().Equals("true") ) {
								curViewRow.Cells["MemberStatus"].Value = "Active - " + (String) curDataRow["MemTypeDesc"];
							} else {
								curViewRow.Cells["MemberStatus"].Value = "In-Active - " + (String) curDataRow["MemTypeDesc"];
							}
						} catch {
							curViewRow.Cells["MemberStatus"].Value = "";
						}
						try {
							curViewRow.Cells["MembershipRate"].Value = (String) curDataRow["MembershipRate"];
						} catch {
							curViewRow.Cells["MembershipRate"].Value = "";
						}
						try {
							curViewRow.Cells["CostToUpgrade"].Value = (String) curDataRow["CostToUpgrade"];
						} catch {
							curViewRow.Cells["CostToUpgrade"].Value = "";
						}
						try {
							curViewRow.Cells["CanSki"].Value = (String) curDataRow["CanSki"];
						} catch {
							curViewRow.Cells["CanSki"].Value = "false";
						}
						try {
							curViewRow.Cells["CanSkiGR"].Value = (String) curDataRow["CanSkiGR"];
						} catch {
							curViewRow.Cells["CanSkiGR"].Value = false;
						}
						#endregion
					}
				}
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
                searchMemberList(myEditMemberDialog.MemberId, null, null, null);
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

			curSqlStmt.Append( "FROM MemberList " );
            if ( inMemberId.Length > 0 ) {
                curSqlStmt.Append( "Where MemberId = '" + inMemberId + "'" );
            } else {
                curSqlStmt.Append( "Where LastName like '" + inLastName + "%'" );
                curSqlStmt.Append( "  And FirstName like '" + inFirstName + "%'" );
                if ( inState.Length > 0 ) {
                    curSqlStmt.Append( "  And State = '" + inState + "'" );
                }
            }
            curSqlStmt.Append( " Order by LastName, FirstName, SkiYearAge" );

			return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

		private DataTable sendRequest( String inMemberId, String inLastName, String inFirstName, String inState ) {
			String curMethodName = "sendRequest";
			/* -----------------------------------------------------------------------
            * Validate TourID value for scores to be Exported.
            * http://usawaterski.org/admin/GetMemberRegExportJson.asp?SanctionId=18E014&MemberId=700040630
            * http://usawaterski.org/admin/GetMemberRegExportJson.asp?SanctionId=18E014&FirstName=David&LastName=Allen
            *
            *HTTP_AUTHORIZATION:Basic wstims:Slalom38tTrick13Jump250\nHTTP_HOST:usawaterski.org\nHTTP_USER_AGENT:.NET Framework CustomUserAgent Water Ski Scoring
            ----------------------------------------------------------------------- */

			StringBuilder curQueryString = new StringBuilder( "" );
			curQueryString.Append("?SanctionId=" + mySanctionNum);
			if ( inMemberId.Length > 0 ) {
				curQueryString.Append( "&MemberId=" + inMemberId );
			}
			if ( inFirstName.Length > 0 || inLastName.Length > 0 ) {
				curQueryString.Append( "&FirstName=" + inFirstName + "&LastName=" + inLastName );
			}
			if ( inState.Length > 0 ) {
				curQueryString.Append( "&State=" + inState );
			}

			String curContentType = "application/json; charset=UTF-8";
			String curOfficialExportListUrl = "http://usawaterski.org/admin/GetMemberRegExportJson.asp";
			String curReqstUrl = curOfficialExportListUrl + curQueryString.ToString();
			String curSanctionEditCode = (String) myTourRow["SanctionEditCode"];
			if ( ( curSanctionEditCode == null ) || ( curSanctionEditCode.Length == 0 ) ) {
				MessageBox.Show( "Sanction edit code is required to retrieve officials and ratings.  Enter required value on Tournament Form" );
				return null;
			}

			NameValueCollection curHeaderParams = new NameValueCollection();
			myResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			myResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, mySanctionNum, curSanctionEditCode, false );
			if ( myResponseDataList != null && myResponseDataList.Count > 0 ) {
				DataTable curDataTable = SendMessageHttp.convertDictionaryListToDataTable( myResponseDataList );
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
