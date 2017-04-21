using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Slalom {
    public partial class SlalomPassDetailExport : Form {
        private String mySanctionNum = null;
        private String myTourRules = "";
        private DataRow myTourRow;
        private DataTable myDataTable;
        private PrintDocument myPrintDoc;
        private DataGridViewPrinter myPrintDataGrid;

        public SlalomPassDetailExport() {
            InitializeComponent();
        }

        private void SlalomPassDetailExport_Load( object sender, EventArgs e ) {
            /*
            if ( Properties.Settings.Default.SlalomPassDetailExport_Width > 0 ) {
                this.Width = Properties.Settings.Default.SlalomPassDetailExport_Width;
            }
            if ( Properties.Settings.Default.SlalomPassDetailExport_Height > 0 ) {
                this.Height = Properties.Settings.Default.SlalomPassDetailExport_Height;
            }
            if ( Properties.Settings.Default.SlalomPassDetailExport_Location.X > 0
                && Properties.Settings.Default.SlalomPassDetailExport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.SlalomPassDetailExport_Location;
            }
            */

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show("An active tournament must be selected from the Administration menu Tournament List option");
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String) myTourRow["Rules"];
                    }
                }
            }

        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show("Error happened " + e.Context.ToString());
            if ( e.Context == DataGridViewDataErrorContexts.Commit ) {
                MessageBox.Show("Commit error");
            }
            if ( e.Context == DataGridViewDataErrorContexts.CurrentCellChange ) {
                MessageBox.Show("Cell change");
            }
            if ( e.Context == DataGridViewDataErrorContexts.Parsing ) {
                MessageBox.Show("parsing error");
            }
            if ( e.Context == DataGridViewDataErrorContexts.LeaveControl ) {
                MessageBox.Show("leave control error");
            }
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView) sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        private void SlalomPassDetailExport_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.SlalomPassDetailExport_Width = this.Size.Width;
                Properties.Settings.Default.SlalomPassDetailExport_Height = this.Size.Height;
                Properties.Settings.Default.SlalomPassDetailExport_Location = this.Location;
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            this.Cursor = Cursors.WaitCursor;
            myDataTable = getSlalomPassData();
            loadDataGrid();
            this.Cursor = Cursors.Default;

        }
        private void loadDataGrid() {
            DataGridViewRow curViewRow;
            this.Cursor = Cursors.WaitCursor;

            //this.dataGridView.DataSource = myDataTable;

            /*
            */
            dataGridView.Rows.Clear();
            int curIdx = 0;
            foreach ( DataRow curRow in myDataTable.Rows ) {
                curIdx = dataGridView.Rows.Add();
                curViewRow = dataGridView.Rows[curIdx];
                curViewRow.Cells["SanctionId"].Value = (String) curRow["SanctionId"];
                curViewRow.Cells["SkierName"].Value = (String) curRow["SkierName"];
                curViewRow.Cells["Event"].Value = (String) curRow["Event"];
                curViewRow.Cells["AgeGroup"].Value = (String) curRow["AgeGroup"];
                curViewRow.Cells["EventGroup"].Value = (String) curRow["EventGroup"];
                curViewRow.Cells["EventClass"].Value = (String) curRow["EventClass"];
                curViewRow.Cells["Round"].Value = (Byte) curRow["Round"];
                try {
                    curViewRow.Cells["Boat"].Value = (String) curRow["Boat"];
                } catch {
                    curViewRow.Cells["Boat"].Value = "";
                }
                try {
                    curViewRow.Cells["Driver"].Value = (String) curRow["Driver"];
                } catch {
                    curViewRow.Cells["Driver"].Value = "";
                }
                curViewRow.Cells["Score"].Value = ( (Decimal) curRow["Score"] ).ToString("###.00");
                curViewRow.Cells["SkierRunNum"].Value = ((Int16) curRow["SkierRunNum"]).ToString();
                curViewRow.Cells["BoatTime"].Value = ( (Decimal) curRow["BoatTime"] ).ToString("#0.00");
                curViewRow.Cells["PassScore"].Value = ( (Decimal) curRow["PassScore"] ).ToString("#.00");
                curViewRow.Cells["TimeInTol"].Value = (String) curRow["TimeInTol"];
                curViewRow.Cells["PassNotes"].Value = (String) curRow["PassNotes"];
                curViewRow.Cells["LastUpdateDate"].Value = ( (DateTime) curRow["LastUpdateDate"] ).ToString("MM/dd/yy HH:mm:ss");
            }

        }

        private void navExport_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            //myExportData.exportData(dataGridView, "SlalomPassDetailExport.txt");
            myExportData.exportData(this.myDataTable, "SlalomPassDetailExport.txt");
        }

        private void navExportHtml_Click( object sender, EventArgs e ) {
            ExportData myExportData = new ExportData();
            String printTitle = Properties.Settings.Default.Mdi_Title;
            String printSubtitle = this.Text + " " + mySanctionNum + " held " + myTourRow["EventDates"].ToString();
            String printFooter = " Scored with " + Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion;
            printFooter.PadRight(15, '*');
            printFooter.PadLeft(15, '*');
            myExportData.exportDataAsHtml(dataGridView, printTitle, printSubtitle, printFooter, "SlalomPassDetailExport.htm");
        }

        private void navPrint_Click( object sender, EventArgs e ) {
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
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins(25, 25, 25, 25);
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter(dataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging);

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintPage);
                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView(e.Graphics);
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getSlalomPassData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT S.SanctionId, T.SkierName, E.Event, S.AgeGroup, E.EventGroup, E.EventClass ");
            curSqlStmt.Append(", S.Round, COALESCE(V.CodeValue, S.Boat) as Boat, TD.SkierName as Driver, S.Score ");
            curSqlStmt.Append(",R.SkierRunNum, R.BoatTime, R.Score AS PassScore, R.TimeInTol, R.Note AS PassNotes ");
            curSqlStmt.Append(", R.LastUpdateDate   ");
            curSqlStmt.Append("FROM SlalomScore S  ");
            curSqlStmt.Append("INNER JOIN SlalomRecap R ON S.MemberId = R.MemberId AND S.SanctionId = R.SanctionId AND S.AgeGroup = R.AgeGroup AND S.Round = R.Round   ");
            curSqlStmt.Append("INNER JOIN TourReg T ON S.MemberId = T.MemberId AND S.SanctionId = T.SanctionId AND S.AgeGroup = T.AgeGroup   ");
            curSqlStmt.Append("INNER JOIN EventReg E ON S.MemberId = E.MemberId AND S.SanctionId = E.SanctionId AND S.AgeGroup = T.AgeGroup  ");
            curSqlStmt.Append("LEFT OUTER JOIN OfficialWorkAsgmt A ON A.SanctionId = S.SanctionId AND A.EventGroup = E.EventGroup AND A.Round = S.Round AND A.Event = 'Slalom' AND A.WorkAsgmt = 'Driver' ");
            curSqlStmt.Append("LEFT OUTER JOIN TourReg TD ON TD.MemberId = A.MemberId AND TD.SanctionId = A.SanctionId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList V ON V.ListName = 'ApprovedBoats' AND V.ListCode = S.Boat ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' AND E.Event = 'Slalom'   ");
            curSqlStmt.Append("ORDER BY S.Round, S.AgeGroup, T.SkierName, R.SkierRunNum, R.PassNum ");
            return getData(curSqlStmt.ToString());
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation");
            curSqlStmt.Append(", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation");
            curSqlStmt.Append(", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName ");
            curSqlStmt.Append("FROM Tournament T ");
            curSqlStmt.Append("LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId ");
            curSqlStmt.Append("LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class ");
            curSqlStmt.Append("WHERE T.SanctionId = '" + mySanctionNum + "' ");
            return getData(curSqlStmt.ToString());
        }

        private DataTable getData( String inSelectStmt ) {
            return DataAccess.getDataTable(inSelectStmt);
        }

    }
}