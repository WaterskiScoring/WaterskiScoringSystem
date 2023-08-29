using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Admin {
    public partial class MemberList : Form {
        private String mySanctionNum;
        private String mySortCommand;
        private String myFilterCommand;
        private int myViewIdx;

        private DataTable myMemberListDataTable;

        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EditMember myEditMemberDialog;

        public MemberList() {
            InitializeComponent();
        }

        private void MemberList_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.MemberList_Width > 0 ) {
                this.Width = Properties.Settings.Default.MemberList_Width;
            }
            if ( Properties.Settings.Default.MemberList_Height > 0 ) {
                this.Height = Properties.Settings.Default.MemberList_Height;
            }
            if ( Properties.Settings.Default.MemberList_Location.X > 0
                && Properties.Settings.Default.MemberList_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.MemberList_Location;
            }

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if ( mySanctionNum == null ) {
                mySanctionNum = "";
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    mySanctionNum = "";
                }
            }

            myEditMemberDialog = new EditMember();

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = dataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = dataGridView.Columns;
        }

        private void MemberList_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.MemberList_Width = this.Size.Width;
                Properties.Settings.Default.MemberList_Height = this.Size.Height;
                Properties.Settings.Default.MemberList_Location = this.Location;
            }
        }

        private void dataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "DataGridView_DataError occurred. \n Context: " + e.Context.ToString()
                + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving members";
            Cursor.Current = Cursors.WaitCursor;
            int curRowIdx = myViewIdx;

            myMemberListDataTable.DefaultView.Sort = mySortCommand;
            myMemberListDataTable.DefaultView.RowFilter = myFilterCommand;
            myMemberListDataTable = myMemberListDataTable.DefaultView.ToTable();
            dataGridView.DataSource = myMemberListDataTable;
            winStatusMsg.Text = "Members retrieved";
            Cursor.Current = Cursors.Default;
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            myMemberListDataTable = getMemberList();
            myViewIdx = 0;
            loadDataGridView();
        }

        private void navExport_Click( object sender, EventArgs e ) {
            String[] curSelectCommand = new String[1];
            String[] curTableName = { "MemberList" };
            ExportData myExportData = new ExportData();

            curSelectCommand[0] = "Select * from MemberList ";
            if ( myFilterCommand == null ) {
                curSelectCommand[0] = curSelectCommand[0];
            } else {
                if ( myFilterCommand.Length > 0 ) {
                    curSelectCommand[0] = curSelectCommand[0]
                        + " Where " + myFilterCommand;
                } else {
                    curSelectCommand[0] = curSelectCommand[0];
                }
            }
            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navSort_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCommand;
                loadDataGridView();
            }
        }

        private void navFilter_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCommand = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCommand;
                loadDataGridView();
            }
        }

        private void navInsert_Click( object sender, EventArgs e ) {
            // Open dialog for selecting skiers
            myEditMemberDialog.editMember( null );
            myEditMemberDialog.ShowDialog( this );

            // Refresh data from database
            if ( myEditMemberDialog.DialogResult == DialogResult.OK ) {
                navRefresh_Click( null, null );
            }
        }

        private void navEdit_Click( object sender, EventArgs e ) {
            DataGridView curView = dataGridView;

            //Send current member row
            if ( !( isObjectEmpty( curView.Rows[myViewIdx].Cells["MemberId"].Value ) ) ) {
                // Display the form as a modal dialog box.
                String curMemberId = (String)curView.Rows[myViewIdx].Cells["MemberId"].Value;
                myEditMemberDialog.editMember( curMemberId );
                myEditMemberDialog.ShowDialog( this );

                // Determine if the OK button was clicked on the dialog box.
                if ( myEditMemberDialog.DialogResult == DialogResult.OK ) {
                    navRefresh_Click( null, null );
                }
            }
        }

        private void dataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = ( DataGridView )sender;

            if ( e.RowIndex >= 0 ) {
                myViewIdx = e.RowIndex;

                //Send current member row
                if ( !( isObjectEmpty( curView.Rows[myViewIdx].Cells["MemberId"].Value ) ) ) {
                    // Display the form as a modal dialog box.
                    String curMemberId = (String)curView.Rows[myViewIdx].Cells["MemberId"].Value;
                    myEditMemberDialog.editMember( curMemberId );
                    myEditMemberDialog.ShowDialog( this );

                    // Determine if the OK button was clicked on the dialog box.
                    if ( myEditMemberDialog.DialogResult == DialogResult.OK ) {
                        navRefresh_Click( null, null );
                    }
                }
            }
        }

        private void dataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            myViewIdx = e.RowIndex;
            int curRow = myViewIdx + 1;
            RowStatusLabel.Text = "Row " + curRow.ToString() + " of " + dataGridView.Rows.Count.ToString();
        }

        private void navRemove_Click( object sender, EventArgs e ) {
            String curMemberId = "", curSkierName = "";
            bool curResults = true;

            DataGridViewRow curViewRow = dataGridView.Rows[myViewIdx];
            if ( curViewRow != null ) {
                try {
                    curMemberId = (String)curViewRow.Cells["MemberId"].Value;
                    curSkierName = (String)curViewRow.Cells["FirstName"].Value + " " + (String)curViewRow.Cells["LastName"].Value;

                    try {
						StringBuilder curSqlStmt = new StringBuilder( "" );
						curSqlStmt.Append( "Delete MemberList Where MemberId = '" + curMemberId + "'");
						int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        winStatusMsg.Text = "Skier " + curSkierName + " entry removed";

					} catch ( Exception excp ) {
                        curResults = false;
                        MessageBox.Show( "Error attempting to remove " + curSkierName + "\n" + excp.Message );
                    }

                    if ( curResults ) {
                        navRefresh_Click(null, null);
                    }

                } catch ( Exception excp ) {
                    curResults = false;
                    MessageBox.Show( "Error attempting to remove " + curSkierName + "\n" + excp.Message );
                }
            }
        }

        private void navRemoveAll_Click(object sender, EventArgs e) {
            String dialogMsg = "All members will be removed!"
                + "\n This will not affect any tournament registrations or scores."
                + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show( dialogMsg, "Truncate Warning",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            if (msgResp == DialogResult.Yes) {
                try {
					StringBuilder curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "Delete MemberList" );
					int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

					MessageBox.Show( rowsProc + " members removed" );
                    navRefresh_Click(null, null);

				} catch (Exception excp) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }

        }

        private DataTable getMemberList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT MemberId, LastName, FirstName, SkiYearAge, Gender" );
			curSqlStmt.Append( ", State, City, Federation, ForeignFederationID" );
			curSqlStmt.Append( ", MemberStatus, MemberExpireDate, InsertDate, UpdateDate " );
			curSqlStmt.Append( ", Coalesce( JudgeSlalomRating, '' ) as JudgeSlalom" );
			curSqlStmt.Append( ", Coalesce( JudgeTrickRating, '' ) as JudgeTrick" );
			curSqlStmt.Append( ", Coalesce( JudgeJumpRating, '' ) as JudgeJump" );
			curSqlStmt.Append( ", Coalesce( DriverSlalomRating, '' ) as DriverSlalom" );
			curSqlStmt.Append( ", Coalesce( DriverTrickRating, '' ) as DriverTrick" );
			curSqlStmt.Append( ", Coalesce( DriverJumpRating, '' ) as DriverJump" );
			curSqlStmt.Append( ", Coalesce( ScorerSlalomRating, '' ) as ScorerSlalom" );
			curSqlStmt.Append( ", Coalesce( ScorerTrickRating, '' ) as ScorerTrick" );
			curSqlStmt.Append( ", Coalesce( ScorerJumpRating, '' ) as ScorerJump" );
			curSqlStmt.Append( ", Coalesce( SafetyOfficialRating, '' ) as SafetyOfficial" );
			curSqlStmt.Append( ", Coalesce( TechOfficialRating, '' ) as TechController" );
			curSqlStmt.Append( ", Coalesce( AnncrOfficialRating, '' ) as AnncrOfficial " );
			curSqlStmt.Append( "FROM MemberList " );
            curSqlStmt.Append( " Order by LastName, FirstName" );
			return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

        private void dataGridView_CellContentClick( object sender, DataGridViewCellEventArgs e ) {

        }
    }
}
