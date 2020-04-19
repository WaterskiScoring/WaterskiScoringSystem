using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class FilterDialogForm : Form {
        private List<ListItem> OperatorList = new List<ListItem>();
        private String[] myColumnList;

        /* 
         * Class initialization
         */
        public FilterDialogForm() {
            //Initialize component
            InitializeComponent();

            // Load data for dropdown list of sort modes
            OperatorList.Add(new ListItem("Equal", "="));
            OperatorList.Add(new ListItem("Contains", "LIKE"));
            OperatorList.Add(new ListItem("Starts With", "BEGIN"));
            OperatorList.Add(new ListItem("Ends With", "END"));
            OperatorList.Add(new ListItem("Not Equal", "<>"));
            OperatorList.Add(new ListItem("Less Than", "<"));
            OperatorList.Add(new ListItem("Less Than or Equal", "<="));
            OperatorList.Add(new ListItem("Greater than", ">"));
            OperatorList.Add(new ListItem("Greater Than or Equal", ">="));
            OperatorList.Add(new ListItem("In list", "IN"));
            OperatorList.Add(new ListItem("Not Contains ", "NOT LIKE"));
            Operator.DataSource = OperatorList;
            Operator.DisplayMember = "ItemName";
            Operator.ValueMember = "ItemValue";
        }

        private void FilterDialogForm_Load(object sender, EventArgs e) {
        }

        public String FilterCommand {
            get {
                return outFilterCommand;
            }
            set {
                outFilterCommand = value;
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

        private void menuInsert_Click( object sender, EventArgs e ) {
            // Insert new row into datagrid
            if (dataGridView.CurrentRow != null) {
                dataGridView.Rows.Insert(dataGridView.CurrentRow.Index, 1);
            }
        }

        private void menuDelete_Click(object sender, EventArgs e) {
            // Delete row from datagrid
            if (dataGridView.CurrentRow != null) {
                if (dataGridView.Rows[dataGridView.CurrentRow.Index].Cells["ColumnName"].Value != null
                    && dataGridView.Rows[dataGridView.CurrentRow.Index].Cells["Operator"].Value != null
                    ) {
                    dataGridView.Rows.Remove(dataGridView.CurrentRow);
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e) {

        }

        private void okButton_Click(object sender, EventArgs e) {
            // Build sort command based on data from datagrid
            StringBuilder myCommand = new StringBuilder("");
            String myOperator, myConnector;
            for (int idx = 0; idx < dataGridView.RowCount; ++idx) {
                if (dataGridView.Rows[idx].Cells["ColumnName"].Value != null
                    && dataGridView.Rows[idx].Cells["Operator"].Value != null
                    && dataGridView.Rows[idx].Cells["FilterValue"].Value != null
                    ) {
                    if (myCommand.Length > 1) {
                        if (dataGridView.Rows[idx].Cells["Connector"].Value == null) {
                            myConnector = "OR";
                        } else {
                            myConnector = dataGridView.Rows[idx].Cells["Connector"].Value.ToString();
                        }
                        myCommand.Append(" " + myConnector + " "); 
                    }
                    myOperator = dataGridView.Rows[idx].Cells["Operator"].Value.ToString();
                    myCommand.Append(dataGridView.Rows[idx].Cells["ColumnName"].Value.ToString());
                    if ( myOperator.Equals("IN") ) {
                        myCommand.Append(" " + dataGridView.Rows[idx].Cells["Operator"].Value.ToString()
                            + " ('" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString().Replace(",", "','") + "')");
                    } else if (myOperator.Equals("LIKE") ) {
                        myCommand.Append(" " + dataGridView.Rows[idx].Cells["Operator"].Value.ToString()
                            + " '%" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString() + "%'");
                    } else if (myOperator.Equals("NOT LIKE") ) {
                        myCommand.Append(" " + dataGridView.Rows[idx].Cells["Operator"].Value.ToString()
                            + " '%" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString() + "%'");
                    } else if (myOperator.Equals("BEGIN") ) {
                        myCommand.Append(" LIKE '" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString() + "%'");
                    } else if (myOperator.Equals("END") ) {
                        myCommand.Append(" LIKE '%" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString() + "'");
                    } else {
                        myCommand.Append(" " + dataGridView.Rows[idx].Cells["Operator"].Value.ToString()
                            + " '" + dataGridView.Rows[idx].Cells["FilterValue"].Value.ToString() + "'");
                    }
                }
            }
            FilterCommand = myCommand.ToString();
        }

        private void FilterDialogForm_Load_1(object sender, EventArgs e) {

        }

    }
}
