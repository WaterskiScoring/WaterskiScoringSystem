using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlServerCe;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {

    public partial class SqlCommandTool : Form {
        private DataTable myDataTable;

        public SqlCommandTool() {
            InitializeComponent();
        }

        private void SqlCommandTool_Load(object sender, EventArgs e) {
            this.dataGridView.Visible = false;
        }

        private void ExecButton_Click(object sender, EventArgs e) {
            if (SqlcommandTextBox.Text.Length > 1) {
                String curSqlCmd = SqlcommandTextBox.Text.Trim();
                if (curSqlCmd.ToLower().StartsWith("select")) {
                    try {
                        //String curTemp = curSqlCmd.Replace("'", "''");
                        //curSqlCmd = curTemp;
                        myDataTable = DataAccess.getDataTable(curSqlCmd);
                        this.dataGridView.DataSource = myDataTable;
                        this.dataGridView.Visible = true;
                        this.MessageLabel.Text = String.Format("SQL command completed, {0} rows processed", myDataTable.Rows.Count);
                    } catch (Exception ex) {
                        this.dataGridView.Visible = false;
                        this.MessageLabel.Text = ex.Message;
                    }
                } else {
                    try {
                        //String curTemp = curSqlCmd.Replace("'", "''");
                        //curSqlCmd = curTemp;
                        this.dataGridView.Visible = false;
                        int curRowsProc = DataAccess.ExecuteCommand(curSqlCmd);
                        this.MessageLabel.Text = String.Format("SQL command completed, {0} rows processed", curRowsProc);
                    } catch (Exception ex) {
                        this.MessageLabel.Text = ex.Message;
                    }
                }
            } else {
                MessageLabel.Text = "SQL command is required and not available";
            }
        }

        private void ExportButton_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            myExportData.exportData( dataGridView, "SqlTableExport.txt" );
        }

    }
}
