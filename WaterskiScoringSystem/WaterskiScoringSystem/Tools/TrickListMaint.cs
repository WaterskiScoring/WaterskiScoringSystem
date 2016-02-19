using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing.Printing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;

namespace WaterskiScoringSystem.Tools {
    public partial class TrickListMaint : Form {
        private Boolean isDataModified = false;
        private String myOrigCellValue = "";
        private String mySortCommand = "";
        private String myFilterCmd = "";
        private int myViewIdx = 0;

        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        private DataTable myTrickListDataTable;

        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public TrickListMaint() {
            InitializeComponent();
        }

        private void TrickListMaint_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.TrickList_Width > 0) {
                this.Width = Properties.Settings.Default.TrickList_Width;
            }
            if (Properties.Settings.Default.TrickList_Height > 0) {
                this.Height = Properties.Settings.Default.TrickList_Height;
            }
            if (Properties.Settings.Default.TrickList_Location.X > 0
                && Properties.Settings.Default.TrickList_Location.Y > 0) {
                this.Location = Properties.Settings.Default.TrickList_Location;
            }

            winStatusMsg.Text = "Retrieving Trick list data";
            Cursor.Current = Cursors.WaitCursor;

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = DataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = DataGridView.Columns;

            myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
            myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

            //Get Trick list data
            getTrickList();
            loadDataGridView();
        }

        private void TrickListMaint_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.TrickList_Width = this.Size.Width;
                Properties.Settings.Default.TrickList_Height = this.Size.Height;
                Properties.Settings.Default.TrickList_Location = this.Location;
            }
        }

        private void TrickListMaint_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    e.Cancel = true;
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void loadDataGridView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving trick list entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;

                if ( myTrickListDataTable.Rows.Count > 0 ) {
                    DataGridView.Rows.Clear();
                    myTrickListDataTable.DefaultView.Sort = mySortCommand;
                    myTrickListDataTable.DefaultView.RowFilter = myFilterCmd;
                    DataTable curDataTable = myTrickListDataTable.DefaultView.ToTable();

                    myViewIdx = 0;
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        myViewIdx = DataGridView.Rows.Add();
                        curViewRow = DataGridView.Rows[myViewIdx];

                        //NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode
                        curViewRow.Cells["PK"].Value = ( (int)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["NumSkis"].Value = ( (byte)curDataRow["NumSkis"] ).ToString();
                        curViewRow.Cells["NumTurns"].Value = ( (byte)curDataRow["NumTurns"] ).ToString();
                        curViewRow.Cells["Points"].Value = ( (Int16)curDataRow["Points"] ).ToString();
                        curViewRow.Cells["RuleCode"].Value = (String)curDataRow["RuleCode"];
                        curViewRow.Cells["RuleNum"].Value = ( (byte)curDataRow["RuleNum"] ).ToString();
                        curViewRow.Cells["StartPos"].Value = ( (byte)curDataRow["StartPos"] ).ToString();
                        curViewRow.Cells["TrickCode"].Value = (String)curDataRow["TrickCode"];
                        curViewRow.Cells["TypeCode"].Value = ( (byte)curDataRow["TypeCode"] ).ToString();
                        curViewRow.Cells["Description"].Value = ( (String)curDataRow["Description"] ).ToString();
                        
                    }
                    try {
                        myViewIdx = 0;
                        int curRowPos = myViewIdx + 1;
                        RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
                        DataGridView.CurrentCell = DataGridView.Rows[myViewIdx].Cells["TrickCode"];
                    } catch {
                        RowStatusLabel.Text = "";
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving trick list entries \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[1];
            String[] curTableName = { "TrickList" };

            curSelectCommand[0] = "Select * from TrickList ";
            if ( myFilterCmd == null ) {
                curSelectCommand[0] = curSelectCommand[0];
            } else {
                if ( myFilterCmd.Length > 0 ) {
                    curSelectCommand[0] = curSelectCommand[0]
                        + " Where " + myFilterCmd;
                } else {
                    curSelectCommand[0] = curSelectCommand[0];
                }
            }

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
        }

        private void navSort_Click(object sender, EventArgs e) {
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

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadDataGridView();
            }
        }

        private void textBox1_Validated(object sender, EventArgs e) {
            if (( (TextBox)sender ).Text.Length > 0) {
                myFilterCmd = "TrickCode like '%" + ( (TextBox)sender ).Text + "%' OR Description like '%" + ( (TextBox)sender ).Text + "%'";
            } else {
                myFilterCmd = "";
            }
            winStatusMsg.Text = "Filtered by " + myFilterCmd;
            loadDataGridView();

        }

        private void DataGridView_CellValidated(object sender, DataGridViewCellEventArgs e) {
            //isDataModified = true;
        }

        private void navPrint_Click(object sender, EventArgs e) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = true;
            curPrintDialog.AllowSelection = false;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);
                myPrintDoc.DefaultPageSettings.Landscape = false;
                myPrintDataGrid = new DataGridViewPrinter(DataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging);

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;
        }

        private void getTrickList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT NumSkis, NumTurns, PK, Points, RuleCode, RuleNum, StartPos, TrickCode, TypeCode, Description " );
            curSqlStmt.Append( " FROM TrickList " );
            curSqlStmt.Append( " ORDER BY RuleCode, RuleNum, TrickCode" );
            myTrickListDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private void DataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            if (myViewIdx != e.RowIndex) {
                myViewIdx = e.RowIndex;
                int curRowPos = e.RowIndex + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
            }

        }

    }
}
