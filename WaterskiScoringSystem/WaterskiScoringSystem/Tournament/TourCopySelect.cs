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

namespace WaterskiScoringSystem.Tournament {
    public partial class TourCopySelect : Form {
        private String mySanctionNum;

        public TourCopySelect() {
            InitializeComponent();
        }

        private void TourCopySelect_Load(object sender, EventArgs e) {
            loadDataGrid();
        }

        public String SanctionNumToCopy {
            get {
                return mySanctionNum;
            }
            set {
                mySanctionNum = value;
            }
        }

        private void loadDataGrid() {
            this.Cursor = Cursors.WaitCursor;
            DataTable curDataTable = getTourData();
            DataGridViewRow curViewRow;
            DataGridView.Rows.Clear();
            int curIdx = 0;
            foreach (DataRow curRow in curDataTable.Rows) {
                curIdx = DataGridView.Rows.Add();
                curViewRow = DataGridView.Rows[curIdx];
                curViewRow.Cells["SanctionId"].Value = (String)curRow["SanctionId"];
                curViewRow.Cells["TourName"].Value = (String)curRow["Name"];
                curViewRow.Cells["Class"].Value = (String)curRow["Class"];
                curViewRow.Cells["Federation"].Value = (String)curRow["Federation"];
                curViewRow.Cells["SlalomRounds"].Value = ((Byte)curRow["SlalomRounds"]).ToString();
                curViewRow.Cells["TrickRounds"].Value = ((Byte)curRow["TrickRounds"]).ToString();
                curViewRow.Cells["JumpRounds"].Value = ( (Byte)curRow["JumpRounds"] ).ToString();
                curViewRow.Cells["Rules"].Value = (String)curRow["Rules"];
                curViewRow.Cells["EventDates"].Value = (String)curRow["EventDates"];
                curViewRow.Cells["EventLocation"].Value = (String)curRow["EventLocation"];
            }
            this.Cursor = Cursors.Default;
            try {
                RowStatusLabel.Text = "Row 1 of " + curDataTable.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }
        }

        private void navSaveItem_Click(object sender, EventArgs e) {
        }

        private void CopySelectButton_Click(object sender, EventArgs e) {
            if (NewSanctionIdTextbox.Text.Length < 6 || DataGridView.Rows.Count == 0) {
                MessageBox.Show( "New sanction number must be entered" );
                this.DialogResult = DialogResult.None;
            } else {
                String curSanctionNum = DataGridView.CurrentRow.Cells["SanctionId"].Value.ToString();
                SanctionNumToCopy = curSanctionNum;

                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt = new StringBuilder( "Insert Into Tournament ( SanctionId" );
                String curSystemTableSqlStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS "
                    + "WHERE TABLE_NAME = 'Tournament' And COLUMN_NAME != 'PK' And COLUMN_NAME != 'SanctionId' "
                    + "ORDER BY COLUMN_NAME";
                DataTable myDataTable = getData( curSystemTableSqlStmt );
                foreach (DataRow curRow in myDataTable.Rows) {
                    curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                }
                curSqlStmt.Append( ") SELECT '" + NewSanctionIdTextbox.Text + "'" );
                foreach (DataRow curRow in myDataTable.Rows) {
                    curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                }
                curSqlStmt.Append( " FROM Tournament WHERE SanctionId = '" + SanctionNumToCopy + "' " );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                if (rowsProc > 0) {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt = new StringBuilder( "Insert Into TourProperties ( SanctionId" );
                    curSystemTableSqlStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS "
                        + "WHERE TABLE_NAME = 'TourProperties' And COLUMN_NAME != 'PK' And COLUMN_NAME != 'SanctionId' "
                        + "ORDER BY COLUMN_NAME";
                    myDataTable = getData( curSystemTableSqlStmt );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( ") SELECT '" + NewSanctionIdTextbox.Text + "'" );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( " FROM TourProperties WHERE SanctionId = '" + SanctionNumToCopy + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    if (rowsProc > 0) {
                        MessageBox.Show( "Tournament attributes copied from " + SanctionNumToCopy + " to " + NewSanctionIdTextbox.Text );
                    } else {
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt = new StringBuilder( "Insert Into SafetyCheckList ( SanctionId" );
                    curSystemTableSqlStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS "
                        + "WHERE TABLE_NAME = 'SafetyCheckList' And COLUMN_NAME != 'PK' And COLUMN_NAME != 'SanctionId' "
                        + "ORDER BY COLUMN_NAME";
                    myDataTable = getData( curSystemTableSqlStmt );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( ") SELECT '" + NewSanctionIdTextbox.Text + "'" );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( " FROM SafetyCheckList WHERE SanctionId = '" + SanctionNumToCopy + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    if (rowsProc > 0) {
                        MessageBox.Show( "Safety check list attributes copied from " + SanctionNumToCopy + " to " + NewSanctionIdTextbox.Text );
                    } else {
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt = new StringBuilder( "Insert Into TourBoatUse ( SanctionId" );
                    curSystemTableSqlStmt = "SELECT DISTINCT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS "
                        + "WHERE TABLE_NAME = 'TourBoatUse' And COLUMN_NAME != 'PK' And COLUMN_NAME != 'SanctionId' "
                        + "ORDER BY COLUMN_NAME";
                    myDataTable = getData( curSystemTableSqlStmt );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( ") SELECT '" + NewSanctionIdTextbox.Text + "'" );
                    foreach (DataRow curRow in myDataTable.Rows) {
                        curSqlStmt.Append( ",  " + (String)curRow["COLUMN_NAME"] );
                    }
                    curSqlStmt.Append( " FROM TourBoatUse WHERE SanctionId = '" + SanctionNumToCopy + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    if (rowsProc > 0) {
                        MessageBox.Show( "Safety check list attributes copied from " + SanctionNumToCopy + " to " + NewSanctionIdTextbox.Text );
                    } else {
                    }

                } else {
                    MessageBox.Show( "Copy operataion was not successful" );
                    this.DialogResult = DialogResult.None;
                }
            }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            String curSanctionNum = DataGridView.CurrentRow.Cells["SanctionId"].Value.ToString();
            SanctionNumToCopy = curSanctionNum;
        }

        private void DataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            String curSanctionNum = DataGridView.CurrentRow.Cells["SanctionId"].Value.ToString();
            SanctionNumToCopy = curSanctionNum;
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, Name, Class, Federation, TourDataLoc, LastUpdateDate" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", HcapSlalomBase, HcapTrickBase, HcapJumpBase, HcapSlalomPct, HcapTrickPct, HcapJumpPct" );
            curSqlStmt.Append( ", RopeHandlesSpecs, SlalomRopesSpecs, JumpRopesSpecs, SlalomCourseSpecs, JumpCourseSpecs, TrickCourseSpecs" );
            curSqlStmt.Append( ", RuleExceptions, RuleInterpretations, SafetyDirPerfReport" );
            curSqlStmt.Append( ", BuoySpecs, RuleExceptQ1, RuleExceptQ2, RuleExceptQ3, RuleExceptQ4, RuleInterQ1, RuleInterQ2, RuleInterQ3, RuleInterQ4" );
            curSqlStmt.Append( ", ContactMemberId, ContactPhone, ContactEmail, ContactAddress" );
            curSqlStmt.Append( ", ChiefJudgeMemberId, ChiefJudgeAddress, ChiefJudgePhone, ChiefJudgeEmail" );
            curSqlStmt.Append( ", ChiefDriverMemberId, ChiefDriverAddress, ChiefDriverPhone, ChiefDriverEmail" );
            curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirAddress, SafetyDirPhone, SafetyDirEmail" );
            curSqlStmt.Append( ", ChiefScorerMemberId, ChiefScorerAddress, ChiefScorerPhone, ChiefScorerEmail" );
            curSqlStmt.Append( ", LastUpdateDate, PlcmtMethod, OverallMethod " );
            curSqlStmt.Append( "FROM Tournament " );
            curSqlStmt.Append( "ORDER BY SanctionId " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

    }
}
