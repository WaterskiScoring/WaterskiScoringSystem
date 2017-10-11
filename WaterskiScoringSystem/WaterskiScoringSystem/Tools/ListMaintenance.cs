using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class ListMaintenance : Form {
        private Boolean isDataModified = false;
        private int myViewIdx = 0;
        String myFilterCommand = "";

        public ListMaintenance() {
            InitializeComponent();
        }

        private void ListMaintenance_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.ListMaint_Width > 0) {
                this.Width = Properties.Settings.Default.ListMaint_Width;
            }
            if (Properties.Settings.Default.ListMaint_Height > 0) {
                this.Height = Properties.Settings.Default.ListMaint_Height;
            }
            if (Properties.Settings.Default.ListMaint_Location.X > 0
                && Properties.Settings.Default.ListMaint_Location.Y > 0) {
                this.Location = Properties.Settings.Default.ListMaint_Location;
            }

            isDataModified = false;
            loadListNameDropdown();
            loadDataView();

        }

        private void ListMaintenance_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                try {
                    isDataModified = false;
                } catch (Exception excp) {
                    e.Cancel = true;
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
            e.Cancel = false;
        }

        private void ListMaintenance_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.ListMaint_Width = this.Size.Width;
                Properties.Settings.Default.ListMaint_Height = this.Size.Height;
                Properties.Settings.Default.ListMaint_Location = this.Location;
            }
        }

        private void loadListNameDropdown() {
            ArrayList curDropdownList = new ArrayList();
            DataTable curDataTable = getListOfList();
            foreach (DataRow curRow in curDataTable.Rows) {
                curDropdownList.Add( (String)curRow["ListName"] );
            }
            listNameDropdown.DataSource = curDropdownList;
        }

        private void loadDataView() {
            winStatusMsg.Text = "Retrieving data";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;

                DataTable curDataTable = getListData( myFilterCommand );
                if (curDataTable.Rows.Count > 0) {
                    DataGridView.Rows.Clear();
                    DataGridViewRow curViewRow;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        myViewIdx = DataGridView.Rows.Add();
                        curViewRow = DataGridView.Rows[myViewIdx];
                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["ListName"].Value = ( (String)curDataRow["ListName"] );
                        curViewRow.Cells["ListCode"].Value = ( (String)curDataRow["ListCode"] );
                        if ( isObjectEmpty(curDataRow["ListCodeNum"])) {
                            curViewRow.Cells["ListCodeNum"].Value = "";
                        } else {
                            curViewRow.Cells["ListCodeNum"].Value = ( (Decimal)curDataRow["ListCodeNum"] ).ToString();
                        }
                        if (isObjectEmpty( curDataRow["SortSeq"] )) {
                            curViewRow.Cells["SortSeq"].Value = "";
                        } else {
                            curViewRow.Cells["SortSeq"].Value = ( (int)curDataRow["SortSeq"] );
                        }
                        if (isObjectEmpty( curDataRow["CodeValue"] )) {
                            curViewRow.Cells["CodeValue"].Value = "";
                        } else {
                            curViewRow.Cells["CodeValue"].Value = ( (String)curDataRow["CodeValue"] );
                        }
                        if (isObjectEmpty( curDataRow["MinValue"] )) {
                            curViewRow.Cells["MinValue"].Value = "";
                        } else {
                            curViewRow.Cells["MinValue"].Value = ( (Decimal)curDataRow["MinValue"] ).ToString();
                        }
                        if (isObjectEmpty( curDataRow["MaxValue"] )) {
                            curViewRow.Cells["MaxValue"].Value = "";
                        } else {
                            curViewRow.Cells["MaxValue"].Value = ( (Decimal)curDataRow["MaxValue"] ).ToString();
                        }
                        if (isObjectEmpty( curDataRow["CodeDesc"] )) {
                            curViewRow.Cells["CodeDesc"].Value = "";
                        } else {
                            curViewRow.Cells["CodeDesc"].Value = ( (String)curDataRow["CodeDesc"] );
                        }
                    }
                }
                myViewIdx = 0;
                DataGridView.CurrentCell = DataGridView.Rows[myViewIdx].Cells["ListName"];
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
            } catch (Exception ex) {
                MessageBox.Show( "Error retrieving list entries for current list name \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }
        
        private void listNameComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            myFilterCommand = (String)listNameDropdown.SelectedValue;
            loadDataView();
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            string curSelectCommand = "Select * from CodeValueList ";
            if (myFilterCommand != null && myFilterCommand.Length > 0) {
                curSelectCommand += "Where ListName = '" + myFilterCommand + "' ";
            }
            curSelectCommand += "Order by ListName, SortSeq, CodeValue";
            myExportData.exportData( "CodeValueList", curSelectCommand );
        }
        private void navExportAll_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            string curSelectCommand = "Select * from CodeValueList "
            + "Order by ListName, SortSeq, CodeValue";
            myExportData.exportData( "CodeValueList", curSelectCommand );
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
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

        private DataTable getListOfList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct ListName " );
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "ORDER BY ListName " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getListData(String inListName) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, ListName,ListCode, ListCodeNum, SortSeq, CodeValue, MinValue, MaxValue, CodeDesc ");
            curSqlStmt.Append( "FROM CodeValueList " );
            curSqlStmt.Append( "Where ListName = '" + inListName + "' " );
            curSqlStmt.Append( "ORDER BY ListName, SortSeq " );
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
