using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class SortDialogForm : Form {
        private List<ListItem> ModeList = new List<ListItem>();
        private String[] myColumnList;

        /* 
         * Class initialization
         */
        public SortDialogForm() {
            //Initialize component
            InitializeComponent();

            // Load data for dropdown list of sort modes
            ModeList.Add(new ListItem("Ascending", "ASC"));
            ModeList.Add(new ListItem("Descending", "DESC"));
            SortMode.DataSource = ModeList;
            SortMode.DisplayMember = "ItemName";
            SortMode.ValueMember = "ItemValue";
        }

        private void SortDialogForm_Load(object sender, EventArgs e) {
        }

        public String SortCommand {
            get {
                return outSortCommand;
            }
            set {
                outSortCommand = value;
                if (outSortCommand.Length > 1) {
                    String curCmd;
                    String[] cmdAttr;
                    dataGridView.Rows.Clear();
                    String[] cmdList = outSortCommand.Split(new Char[] { ',' });
                    dataGridView.Rows.Insert(0, cmdList.Length);
                    for (int idx = 0; idx < cmdList.Length; idx++) {
                        curCmd = cmdList[idx].Trim();
                        cmdAttr = curCmd.Split(new Char[] { ' ' });
                        dataGridView.Rows[idx].Cells["ColumnName"].Value = cmdAttr[0];
                        dataGridView.Rows[idx].Cells["SortMode"].Value = cmdAttr[1];
                    }
                }
            }
        }

        public DataGridViewColumnCollection ColumnList {
            // Input used to build dropdown list of column names 
            get {
                return inputColumnList;
            }
            set {
                inputColumnList = value;
                for (int idx = 0; idx < inputColumnList.Count; ++idx) {
                    if ( inputColumnList[idx].DataPropertyName != null ) {
                        if ( inputColumnList[idx].DataPropertyName.Length > 0 ) {
                            ColumnName.Items.Add( inputColumnList[idx].DataPropertyName );
                        } else {
                            ColumnName.Items.Add( inputColumnList[idx].Name );
                        }
                    } else {
                        ColumnName.Items.Add( inputColumnList[idx].Name );
                    }
                }
            }
        }

        public String[] ColumnListArray {
            // Input used to build dropdown list of column names 
            get {
                return myColumnList;
            }
            set {
                myColumnList = value;
                for ( int idx = 0; idx < myColumnList.Length; ++idx ) {
                    ColumnName.Items.Add( myColumnList[idx] );
                }
            }
        }

        private void okButton_Click( object sender, EventArgs e ) {
            // Build sort command based on data from datagrid
            StringBuilder myCommand = new StringBuilder("");
            for (int idx = 0; idx < dataGridView.RowCount; ++idx) {
                if (dataGridView.Rows[idx].Cells["ColumnName"].Value != null
                    && dataGridView.Rows[idx].Cells["SortMode"].Value != null
                    ) {
                    if (myCommand.Length > 1) { myCommand.Append(", "); }
                    myCommand.Append(dataGridView.Rows[idx].Cells["ColumnName"].Value.ToString()
                        + " " + dataGridView.Rows[idx].Cells["SortMode"].Value.ToString());
                }
            }
            outSortCommand = myCommand.ToString();
        }

        private void menuInsert_Click(object sender, EventArgs e) {
            // Insert new row into datagrid
            if (dataGridView.CurrentRow != null) {
                dataGridView.Rows.Insert(dataGridView.CurrentRow.Index, 1);
            }
        }

        private void menuDelete_Click(object sender, EventArgs e) {
            // Delete row from datagrid
            if (dataGridView.CurrentRow != null) {
                if (dataGridView.Rows[dataGridView.CurrentRow.Index].Cells["ColumnName"].Value != null
                    && dataGridView.Rows[dataGridView.CurrentRow.Index].Cells["SortMode"].Value != null
                    ) {
                    dataGridView.Rows.Remove(dataGridView.CurrentRow);
                }
            }
        }

        private void dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e) {
            int curRowIdx = e.RowIndex;
            int curColIdx = e.ColumnIndex;
            String curValue = (String)dataGridView.Rows[curRowIdx].Cells["SortMode"].Value;
            if (curValue == null) curValue = "";
            if (curValue.ToUpper().Equals( "ASC" ) || curValue.ToUpper().Equals( "DESC" )) {
            } else {
                ((DataGridViewComboBoxCell)dataGridView.Rows[curRowIdx].Cells["SortMode"]).Value = "ASC";
            }
        }

    }
}
