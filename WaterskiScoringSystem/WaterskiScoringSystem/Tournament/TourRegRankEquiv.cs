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
    public partial class TourRegRankEquiv : Form {
        private String mySanctionNum;
        private bool myDataModified;
		private char[] mySingleQuoteDelim = new char[] { '\'' };

		private DataTable myMemberListDataTable;
		private DataRow myTourRow = null;

		private List<object> myResponseDataList = null;

		public TourRegRankEquiv() {
            InitializeComponent();
        }

        public bool isDataModified {
            get {
                return myDataModified;
            }
        }

        private void TourRegRankEquiv_Load(object sender, EventArgs e) {
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

            //Initialize search fields and set starting position
            inputLastName.Text = "";
            inputFirstName.Text = "";
            inputMemberId.Text = "";
            inputState.Text = "";
            inputLastName.Focus();
        }

        private void TourRegRankEquiv_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void SearchButton_Click(object sender, EventArgs e) {
			myMemberListDataTable = null;
			myResponseDataList = null;
			DataGridView.Rows.Clear();

			/*
			 * Search USA Water Ski MemberList for skiers matching provided search criteria
			 */
			myMemberListDataTable = sendRequest( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );

			/*
			 * Display skiers found matching search criteria
			 */
			if ( myMemberListDataTable == null ) {
				MessageBox.Show( "No records found for specified search criteria" );
			} else { 
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

			/*
			*/
			if ( myMemberListDataTable.Rows.Count > 0 ) {
				DataGridViewRow curViewRow;
				foreach ( DataRow curDataRow in myMemberListDataTable.Rows ) {
					curRowIdx = DataGridView.Rows.Add();
					curViewRow = DataGridView.Rows[curRowIdx];

					#region Member record information
					try {
						curViewRow.Cells["MemberId"].Value = (String) curDataRow["MemberId"];
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
						curViewRow.Cells["SlalomRank"].Value = (String) curDataRow["SlalomRank"];
					} catch {
						curViewRow.Cells["SlalomRank"].Value = "";
					}
					try {
						curViewRow.Cells["SlalomRankEquiv1"].Value = (String) curDataRow["SlalomRankEquiv1"];
					} catch {
						curViewRow.Cells["SlalomRankEquiv1"].Value = "";
					}
					try {
						curViewRow.Cells["SlalomRankEquiv2"].Value = (String) curDataRow["SlalomRankEquiv2"];
					} catch {
						curViewRow.Cells["SlalomRankEquiv2"].Value = "";
					}
					try {
						curViewRow.Cells["SlalomRankEquiv3"].Value = (String) curDataRow["SlalomRankEquiv3"];
					} catch {
						curViewRow.Cells["SlalomRankEquiv3"].Value = "";
					}
					try {
						curViewRow.Cells["TrickRank"].Value = (String) curDataRow["TrickRank"];
					} catch {
						curViewRow.Cells["TrickRank"].Value = "";
					}
					try {
						curViewRow.Cells["TrickRankEquiv1"].Value = (String) curDataRow["TrickRankEquiv1"];
					} catch {
						curViewRow.Cells["TrickRankEquiv1"].Value = "";
					}
					try {
						curViewRow.Cells["TrickRankEquiv2"].Value = (String) curDataRow["TrickRankEquiv2"];
					} catch {
						curViewRow.Cells["TrickRankEquiv2"].Value = "";
					}
					try {
						curViewRow.Cells["TrickRankEquiv3"].Value = (String) curDataRow["TrickRankEquiv3"];
					} catch {
						curViewRow.Cells["TrickRankEquiv3"].Value = "";
					}
					try {
						curViewRow.Cells["JumpRank"].Value = (String) curDataRow["JumpRank"];
					} catch {
						curViewRow.Cells["JumpRank"].Value = "";
					}
					try {
						curViewRow.Cells["JumpRankEquiv1"].Value = (String) curDataRow["JumpRankEquiv1"];
					} catch {
						curViewRow.Cells["JumpRankEquiv1"].Value = "";
					}
					try {
						curViewRow.Cells["JumpRankEquiv2"].Value = (String) curDataRow["JumpRankEquiv2"];
					} catch {
						curViewRow.Cells["JumpRankEquiv2"].Value = "";
					}
					try {
						curViewRow.Cells["JumpRankEquiv3"].Value = (String) curDataRow["JumpRankEquiv3"];
					} catch {
						curViewRow.Cells["JumpRankEquiv3"].Value = "";
					}
					try {
						curViewRow.Cells["SlalomRating"].Value = (String) curDataRow["SlalomRating"];
					} catch {
						curViewRow.Cells["SlalomRating"].Value = "";
					}
					try {
						curViewRow.Cells["TrickRating"].Value = (String) curDataRow["TrickRating"];
					} catch {
						curViewRow.Cells["TrickRating"].Value = "";
					}
					try {
						curViewRow.Cells["JumpRating"].Value = (String) curDataRow["JumpRating"];
					} catch {
						curViewRow.Cells["JumpRating"].Value = "";
					}
					try {
						curViewRow.Cells["OverallRating"].Value = (String) curDataRow["OverallRating"];
					} catch {
						curViewRow.Cells["OverallRating"].Value = "";
					}
					try {
						curViewRow.Cells["OverallRank"].Value = (String) curDataRow["OverallRank"];
					} catch {
						curViewRow.Cells["OverallRank"].Value = "";
					}
					/*
					 * Analyze information and determine membership status 
					 */
					Boolean curCanSki = false;
					try {
						if ( ( (String) curDataRow["CanSki"] ).ToUpper().Equals( "TRUE" ) ) {
							curCanSki = true;
						} else {
							curCanSki = false;
						}
					} catch {
						curCanSki = false;
					}

					try {
						curEffTo = Convert.ToDateTime( (String) curDataRow["EffTo"] );
						curViewRow.Cells["EffTo"].Value = curEffTo.ToString( "MM/dd/yy" );
						if ( curEffTo >= curTourDate && curCanSki ) {
							curViewRow.Cells["CanSki"].Value = curCanSki;

							try {
								if ( ( (String) curDataRow["ActiveMember"] ).ToLower().Equals( "true" ) ) {
									curViewRow.Cells["MemberStatus"].Value = "Active - " + (String) curDataRow["MemTypeDesc"];
								} else {
									curViewRow.Cells["MemberStatus"].Value = "In-Active - " + (String) curDataRow["MemTypeDesc"];
								}
							} catch {
								curViewRow.Cells["MemberStatus"].Value = "";
							}

						} else {
							curViewRow.Cells["CanSki"].Value = false;
							if ( curEffTo < curTourDate ) {
								if ( curCanSki ) {
									curViewRow.Cells["MemberStatus"].Value = "Needs Renew";

								} else {
									curViewRow.Cells["MemberStatus"].Value = "Needs Renew/Upgrade";
								}

							} else {
							}

						}
					} catch {
						curViewRow.Cells["EffTo"].Value = "";
					}

					#endregion
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

        private DataTable searchMemberList( String inMemberId ) {
            if ( inMemberId.Length > 0 ) {
                return searchMemberList( inMemberId );
            } else {
                return null;
            }
        }
		private DataTable sendRequest( String inMemberId, String inLastName, String inFirstName, String inState ) {
			String curMethodName = "sendRequest";
			/* -----------------------------------------------------------------------
            * Validate TourID value for scores to be Exported.
			* Validate TourID value for scores to be Exported.
			* http://www.usawaterski.org/admin/GetMemberRankEquivExportJson.asp?SanctionId=19U038&MemberId=700040630
			* http://www.usawaterski.org/admin/GetMemberRankEquivExportJson.asp?SanctionId=19U038&FirstName=Jeff&LastName=Clark
			* http://www.usawaterski.org/admin/GetMemberRankEquivExportJson.asp?SanctionId=19U038&State=MA
			* http://www.usawaterski.org/admin/GetMemberRankEquivExportJson.asp?SanctionId=19E013&MemberId=700040630&user=19E013&password=10460
            *
            *HTTP_AUTHORIZATION:Basic wstims:Slalom38tTrick13Jump250\nHTTP_HOST:www.usawaterski.org\nHTTP_USER_AGENT:.NET Framework CustomUserAgent Water Ski Scoring
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
			String curOfficialExportListUrl = "https://www.usawaterski.org/admin/GetMemberRankEquivExportJson.asp";
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

		private String stringReplace( String inValue, char[] inCurValue, String inReplValue ) {
			StringBuilder curNewValue = new StringBuilder( "" );

			String[] curValues = inValue.Split( inCurValue );
			if ( curValues.Length > 1 ) {
				int curCount = 0;
				foreach ( String curValue in curValues ) {
					curCount++;
					if ( curCount < curValues.Length ) {
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

	}
}
