using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class SkierRankingList : Form {
        private Boolean isDataModified = false;
        private int myViewIdx = 0;
        private String myFilterCommand = "";

        private Common.SortDialogForm sortDialogForm;
        private Common.FilterDialogForm filterDialogForm;
        private DataTable myDataTable;
        private PrintDocument myPrintDoc;

        public SkierRankingList() {
            InitializeComponent();
        }

        private void SkierRankingList_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.SkierRanking_Width > 0) {
                this.Width = Properties.Settings.Default.SkierRanking_Width;
            }
            if (Properties.Settings.Default.SkierRanking_Height > 0) {
                this.Height = Properties.Settings.Default.SkierRanking_Height;
            }
            if (Properties.Settings.Default.SkierRanking_Location.X > 0
                && Properties.Settings.Default.SkierRanking_Location.Y > 0) {
                this.Location = Properties.Settings.Default.SkierRanking_Location;
            }

            // TODO: This line of code loads data into the 'waterskiDataSet.SkierRanking' table. You can move, or remove it, as needed.
            winStatusMsg.Text = "Retrieving skier ranking list data";
            Cursor.Current = Cursors.WaitCursor;

            this.DataGridView.BeginInvoke((MethodInvoker)delegate() {
                Application.DoEvents();
                Cursor.Current = Cursors.Default;
                winStatusMsg.Text = "Skier ranking list retrieved";
            });
            isDataModified = false;

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = DataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = DataGridView.Columns;

            isDataModified = false;
            myDataTable = getSkierRankingData();
            loadDataView();
        }

        private void SkierRankingList_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                try {
                    isDataModified = false;
                } catch (Exception excp) {
                    e.Cancel = true;
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
        }

        private void SkierRankingList_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.SkierRanking_Width = this.Size.Width;
                Properties.Settings.Default.SkierRanking_Height = this.Size.Height;
                Properties.Settings.Default.SkierRanking_Location = this.Location;
            }
        }

        private void loadDataView() {
            winStatusMsg.Text = "Retrieving data";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;

                if (myDataTable.Rows.Count > 0) {
                    myDataTable.DefaultView.RowFilter = myFilterCommand;
                    DataTable curDataTable = myDataTable.DefaultView.ToTable();

                    DataGridView.Rows.Clear();
                    DataGridViewRow curViewRow;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        myViewIdx = DataGridView.Rows.Add();
                        curViewRow = DataGridView.Rows[myViewIdx];
                        //PK, MemberId, AgeGroup, Event, Score, Rating, HCapBase, HCapScore, Notes, SeqNum
                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["MemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SkierName"].Value = (String)curDataRow["LastName"] + ", " + (String)curDataRow["FirstName"];
                        curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        curViewRow.Cells["Score"].Value = ((Decimal)curDataRow["Score"] ).ToString();
                        curViewRow.Cells["Rating"].Value = (String)curDataRow["Rating"];
                        curViewRow.Cells["SeqNum"].Value = (byte)curDataRow["SeqNum"];

                        if (isObjectEmpty( curDataRow["HCapBase"] )) {
                            curViewRow.Cells["HCapBase"].Value = "";
                        } else {
                            curViewRow.Cells["HCapBase"].Value = ( (Decimal)curDataRow["HCapBase"] ).ToString();
                        }
                        if (isObjectEmpty( curDataRow["HCapScore"] )) {
                            curViewRow.Cells["HCapScore"].Value = "";
                        } else {
                            curViewRow.Cells["HCapScore"].Value = ( (Decimal)curDataRow["HCapScore"] ).ToString();
                        }
                        if (isObjectEmpty( curDataRow["Notes"] )) {
                            curViewRow.Cells["Notes"].Value = "";
                        } else {
                            curViewRow.Cells["Notes"].Value = ( (String)curDataRow["Notes"] ).ToString();
                        }
                    }
                }
                myViewIdx = 0;
                DataGridView.CurrentCell = DataGridView.Rows[myViewIdx].Cells["AgeGroup"];
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
            } catch (Exception ex) {
                MessageBox.Show( "Error retrieving list entries for current list name \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

        private void skierRankingBindingNavigatorSaveItem_Click(object sender, EventArgs e) {
            try {
                isDataModified = false;
            } catch (Exception excp) {
                MessageBox.Show("Error attempting to save changes \n" + excp.Message);
            }
        }

        private void navSort_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            sortDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (sortDialogForm.DialogResult == DialogResult.OK) {
                String sortCommand = sortDialogForm.SortCommand;
                this.Cursor = Cursors.WaitCursor;
                winStatusMsg.Text = "Sorted by " + sortCommand;
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (filterDialogForm.DialogResult == DialogResult.OK) {
                String filterCommand = filterDialogForm.FilterCommand;
                this.Cursor = Cursors.WaitCursor;
                winStatusMsg.Text = "Filtered by " + filterCommand;
                this.DataGridView.BeginInvoke((MethodInvoker)delegate() {
                    Application.DoEvents();
                    this.Cursor = Cursors.Default;
                });
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String mySelectCommand = "Select * from SkierRanking ";
            String filterCommand = "";
            if (filterCommand != null) {
                if ( filterCommand.Length > 1 ) {
                    mySelectCommand = mySelectCommand + "Where " + filterCommand;
                }
            }
            myExportData.exportData( "SkierRanking", mySelectCommand );
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            MessageBox.Show("Error happened " + e.Context.ToString());
            if (e.Context == DataGridViewDataErrorContexts.Commit) {
                MessageBox.Show("Commit error");
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange) {
                MessageBox.Show("Cell change");
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing) {
                MessageBox.Show("parsing error");
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl) {
                MessageBox.Show("leave control error");
            }
            if ((e.Exception) is ConstraintException) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        private void slalomButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                myFilterCommand = "Event = 'Slalom'";
                loadDataView();
            }
        }

        private void trickButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                myFilterCommand = "Event = 'Trick'";
                loadDataView();
            }
        }

        private void jumpButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                myFilterCommand = "Event = 'Jump'";
                loadDataView();
            }
        }

        private void OverallButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                myFilterCommand = "Event = 'Overall'";
                loadDataView();
            }
        }

        private void AllButton_CheckedChanged(object sender, EventArgs e) {
            if (( (RadioButton)sender ).Checked) {
                myFilterCommand = "";
                loadDataView();
            }
        }

        private void sortByScore_CheckedChanged(object sender, EventArgs e) {
            if (( (CheckBox)sender ).Checked) {
                myFilterCommand = "";
                myDataTable.DefaultView.RowFilter = myFilterCommand;
                myDataTable.DefaultView.Sort = "Event ASC, Score DESC, LastName ASC, FirstName ASC";
                myDataTable = myDataTable.DefaultView.ToTable();
                loadDataView();
            } else {
                myFilterCommand = "";
                myDataTable.DefaultView.RowFilter = myFilterCommand;
                myDataTable.DefaultView.Sort = "LastName ASC, FirstName ASC, SeqNum ASC";
                myDataTable = myDataTable.DefaultView.ToTable();
                loadDataView();
            }
        }

        private void DataGridView_RowLeave(object sender, DataGridViewCellEventArgs e) {
            try {
                isDataModified = false;
            } catch (Exception excp) {
                MessageBox.Show("Error attempting to save changes \n" + excp.Message);
            }
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
        }

        private void DataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            isDataModified = true;
        }

        private DataTable getSkierRankingData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SR.MemberId, ML.FirstName, ML.LastName, AgeGroup, Event, Score, Rating, HCapBase, HCapScore, Notes, SeqNum " );
            curSqlStmt.Append( "From SkierRanking SR " );
            curSqlStmt.Append( "INNER JOIN MemberList ML on ML.MemberId = SR.MemberId " );
            curSqlStmt.Append( "Order by ML.LastName, ML.FirstName, SeqNum " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty(object inObject) {
            bool curReturnValue = false;
            if (inObject == null) {
                curReturnValue = true;
            } else if (inObject == System.DBNull.Value) {
                curReturnValue = true;
            } else if (inObject.ToString().Length > 0) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

    }
}
