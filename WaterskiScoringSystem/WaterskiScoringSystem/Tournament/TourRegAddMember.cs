using System;
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
    public partial class TourRegAddMember : Form {
        private String mySanctionNum;
        private bool myDataModified;

        private DataTable myMemberListDataTable;

        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private EditRegMember myEditRegMemberDialog;
        private EditMember myEditMemberDialog;

        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

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

                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                    DataGridView.Rows.Clear();
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
            String newEntryMemberId, NewEntryAgeGroup;
            Int16 rowsAdded = 0;
            Int16 rowsSkipped = 0;

            if ( DataGridView.CurrentRow != null ) {

                DataGridView.CurrentRow.Selected = true;
                DataGridViewSelectedRowCollection selectedRows = DataGridView.SelectedRows;
                foreach ( DataGridViewRow curRow in selectedRows ) {
                    myEditRegMemberDialog.editMember( curRow );
                    myEditRegMemberDialog.ShowDialog();
                    curReqstStatus = myEditRegMemberDialog.ReqstStatus;
                    if ( curReqstStatus.Equals( "Added" ) ) {
                        rowsAdded++;
                        myDataModified = true;
                    } else {
                        rowsSkipped++;
                    }
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
            myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
            if ( myMemberListDataTable != null ) {
                loadDataGridView();
            }
        }

        private void inputMemberId_Leave(object sender, EventArgs e) {
            if (inputMemberId.Text.Length > 0) {
                myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
                if ( myMemberListDataTable != null ) {
                    loadDataGridView();
                }
            }
        }

        private void inputLastName_Leave(object sender, EventArgs e) {
            if (inputLastName.Text.Length > 0) {
                myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
                if ( myMemberListDataTable != null ) {
                    loadDataGridView();
                }
            }
        }

        private void inputFirstName_Leave(object sender, EventArgs e) {
            if (inputFirstName.Text.Length > 0) {
                myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
                if ( myMemberListDataTable != null ) {
                    loadDataGridView();
                }
            }
        }

        private void inputState_TextChanged(object sender, EventArgs e) {
            if (inputState.Text.Length == 2) {
                myMemberListDataTable = searchMemberList( inputMemberId.Text, inputLastName.Text, inputFirstName.Text, inputState.Text );
                if ( myMemberListDataTable != null ) {
                    loadDataGridView();
                }
            }
        }

        private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            Cursor.Current = Cursors.WaitCursor;
            int curRowIdx, curSkiYearAge;
            DateTime curDateOfBirth;

            DataGridView.Rows.Clear();

            if ( myMemberListDataTable.Rows.Count > 0 ) {
                DataGridViewRow curViewRow;
                foreach ( DataRow curDataRow in myMemberListDataTable.Rows ) {
                    curRowIdx = DataGridView.Rows.Add();
                    curViewRow = DataGridView.Rows[curRowIdx];

                    curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                    curViewRow.Cells["LastName"].Value = (String)curDataRow["LastName"];
                    curViewRow.Cells["FirstName"].Value = (String)curDataRow["FirstName"];
                    try {
                        curViewRow.Cells["City"].Value = (String)curDataRow["City"];
                    } catch {
                        curViewRow.Cells["City"].Value = "";
                    }
                    try {
                        curViewRow.Cells["State"].Value = (String)curDataRow["State"];
                    } catch {
                        curViewRow.Cells["State"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Federation"].Value = (String)curDataRow["Federation"];
                    } catch {
                        curViewRow.Cells["Federation"].Value = "";
                    }
                    try {
                        curViewRow.Cells["Gender"].Value = (String)curDataRow["Gender"];
                    } catch {
                        curViewRow.Cells["Gender"].Value = "";
                    }
                    try {
                        curSkiYearAge = (byte)curDataRow["SkiYearAge"];
                        curViewRow.Cells["SkiYearAge"].Value = curSkiYearAge.ToString();
                    } catch {
                        curViewRow.Cells["SkiYearAge"].Value = "";
                    }
                    try {
                        curDateOfBirth = (DateTime)curDataRow["DateOfBirth"];
                        curViewRow.Cells["DateOfBirth"].Value = curDateOfBirth.ToString("MM/dd/yy");
                    } catch {
                        curViewRow.Cells["DateOfBirth"].Value = "";
                    }
                    try {
                        curViewRow.Cells["MemberStatus"].Value = (String)curDataRow["MemberStatus"];
                    } catch {
                        curViewRow.Cells["MemberStatus"].Value = "";
                    }
                }
            }
            Cursor.Current = Cursors.Default;
        }
        
        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView.CurrentRow.Selected = true;
        }

        private void newMemberButton_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            myEditMemberDialog.editMember( null );
            myEditMemberDialog.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( myEditMemberDialog.DialogResult == DialogResult.OK ) {
                //New member added
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
            curSqlStmt.Append( "SELECT MemberId, LastName, FirstName, SkiYearAge, State, City, " );
            curSqlStmt.Append( "Country, DateOfBirth, Federation, Gender, MemberStatus, InsertDate, UpdateDate " );
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

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }
    }
}
