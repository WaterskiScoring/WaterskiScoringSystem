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
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Common {
    public partial class BoatPathDetail : Form {
        private Boolean isDataModified = false;
        private int myViewIdx = 0;
        String myFilterCommand = "";
		private DataRow myBoatPathDataRow;

		public BoatPathDetail() {
            InitializeComponent();
        }

		public DataRow BoatPathDataRow {
			get {
				return myBoatPathDataRow;
			}
			set {
				myBoatPathDataRow = value;
			}
		}

		private void BoatPathDetail_Load(object sender, EventArgs e) {
            isDataModified = false;
            loadDataView();
        }

        private void BoatPathDetail_FormClosing(object sender, FormClosingEventArgs e) {
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

        private void BoatPathDetail_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
            }
        }

        private void loadDataView() {
            winStatusMsg.Text = "Retrieving data";
            Cursor.Current = Cursors.WaitCursor;

            try {
				String curPathPosLocValues = (String)myBoatPathDataRow["PathPosLocValues"];
				String curPathPosDevValues = (String)myBoatPathDataRow["PathPosDevValues"];

				if (curPathPosLocValues.Length == 0 && curPathPosLocValues.Length == 0) return;

				String[] curPosValueList = curPathPosLocValues.Split(',');
				String[] curPosDevList = curPathPosDevValues.Split(',');
				DataGridView.Rows.Clear();
				DataGridViewRow curViewRow;

				for (int idx = 0; idx < curPosValueList.Length; idx++) {
					myViewIdx = DataGridView.Rows.Add();
					curViewRow = DataGridView.Rows[myViewIdx];
					curViewRow.Cells["boatPosition"].Value = curPosValueList[idx];
					curViewRow.Cells["boatDeviation"].Value = curPosDevList[idx];
				}

			} catch (Exception ex) {
                MessageBox.Show( "Error retrieving list entries for current list name \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
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
