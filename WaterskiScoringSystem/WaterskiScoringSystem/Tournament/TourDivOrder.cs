using System;
using System.Collections;
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
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class TourDivOrder : Form {
        #region instance variables
        private Boolean isDataModified = false;
        private int myViewIdx;
        private String myWindowTitle;
        private String mySanctionNum;
        private String myOrigItemValue = "";
        private String mySkierClass;
        private String myTourRules = "";

        private DataRow myTourRow;
        private DataTable myTourDivDataTable;
        private ListSkierClass mySkierClassList;
        private DataGridViewPrinter myPrintDataGrid;
        private PrintDocument myPrintDoc;
        #endregion

        public TourDivOrder() {
            InitializeComponent();
            myWindowTitle = this.Text;
        }

        private void TourDivOrder_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.DivOrder_Width > 0 ) {
                this.Width = Properties.Settings.Default.DivOrder_Width;
            }
            if ( Properties.Settings.Default.DivOrder_Height > 0 ) {
                this.Height = Properties.Settings.Default.DivOrder_Height;
            }
            if ( Properties.Settings.Default.DivOrder_Location.X > 0
                && Properties.Settings.Default.DivOrder_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.DivOrder_Location;
            }

            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                //this.Close();
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    //this.Close();
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];
                        mySkierClass = myTourRow["Class"].ToString();
                        mySkierClassList = new ListSkierClass();
                        mySkierClassList.ListSkierClassLoad();

                        if ( myTourRow["SlalomRounds"] == DBNull.Value ) { myTourRow["SlalomRounds"] = 0; }
                        if ( myTourRow["TrickRounds"] == DBNull.Value ) { myTourRow["TrickRounds"] = 0; }
                        if ( myTourRow["JumpRounds"] == DBNull.Value ) { myTourRow["JumpRounds"] = 0; }
                        if ( Convert.ToInt16( myTourRow["SlalomRounds"] ) == 0 ) {
                            slalomButton.Visible = false;
                        }
                        if ( Convert.ToInt16( myTourRow["TrickRounds"] ) == 0 ) {
                            trickButton.Visible = false;
                        }
                        if ( Convert.ToInt16( myTourRow["JumpRounds"] ) == 0 ) {
                            jumpButton.Visible = false;
                        }

                        if ( slalomButton.Visible ) {
                            slalomButton.Checked = true;
                        } else if ( trickButton.Visible ) {
                            trickButton.Checked = true;
                        } else if ( jumpButton.Visible ) {
                            jumpButton.Checked = true;
                        }

                        myTourDivDataTable = getTourDivData();
                        loadDataView();
                    } else {
                        MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                    }
                }
            }
            isDataModified = false;

        }

        private void TourDivOrder_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.DivOrder_Width = this.Size.Width;
                Properties.Settings.Default.DivOrder_Height = this.Size.Height;
                Properties.Settings.Default.DivOrder_Location = this.Location;
            }
        }

        private void TourDivOrder_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                checkModifyPrompt();
                if ( isDataModified ) {
                    e.Cancel = true;
                } else {
                    e.Cancel = false;
                }
            } else {
                e.Cancel = false;
            }
        }

        private void DataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() );
            if ( e.Context == DataGridViewDataErrorContexts.Commit ) {
                MessageBox.Show( "Commit error" );
            }
            if ( e.Context == DataGridViewDataErrorContexts.CurrentCellChange ) {
                MessageBox.Show( "Cell change" );
            }
            if ( e.Context == DataGridViewDataErrorContexts.Parsing ) {
                MessageBox.Show( "parsing error" );
            }
            if ( e.Context == DataGridViewDataErrorContexts.LeaveControl ) {
                MessageBox.Show( "leave control error" );
            }
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }

        private void checkModifyPrompt() {
            TourDivDataGridView.EndEdit();
            if ( isDataModified ) {
                try {
                    navSave_Click(null, null);
                } catch ( Exception excp ) {
                    MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                }
            }
        }

        private void navSave_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TourDivOrder:navSave";
            String curMsg = "";
            bool curReturn = true;
            int rowsProc = 0;

            try {
                if ( TourDivDataGridView.Rows.Count > 0 ) {
                    LoadDataButton.Focus();
                    TourDivDataGridView.EndEdit();
                    TourDivDataGridView.Focus();
                    DataGridViewRow curViewRow = TourDivDataGridView.Rows[myViewIdx];
                    String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                    if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                        try {
                            Int16 curRunOrder = 0;
                            Int64 curPK = Convert.ToInt64( (String)curViewRow.Cells["PK"].Value );
                            try {
                                curRunOrder = Convert.ToInt16( (String)curViewRow.Cells["RunOrder"].Value );
                            } catch {
                                curRunOrder = 0;
                            }

                            StringBuilder curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update DivOrder Set " );
                            curSqlStmt.Append( "RunOrder = " + curRunOrder.ToString() );
                            curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
                            curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                            isDataModified = false;

                        } catch ( Exception excp ) {
                            curReturn = false;
                            curMsg = "Error attempting to update information \n" + excp.Message;
                            MessageBox.Show( curMsg );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                    } else {
                        isDataModified = false;
                    }
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to save changes \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                checkModifyPrompt();
            }
            if ( !( isDataModified ) ) {
                //Retrieve running order data for display
                myTourDivDataTable = getTourDivData();
                loadDataView();
            }
        }

        private void loadDataView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving data entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                TourDivDataGridView.Rows.Clear();

                if ( myTourDivDataTable.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in myTourDivDataTable.Rows ) {
                        isDataModified = false;
                        myViewIdx = TourDivDataGridView.Rows.Add();
                        curViewRow = TourDivDataGridView.Rows[myViewIdx];
                        curViewRow.Cells["Updated"].Value = "N";
                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        curViewRow.Cells["DivName"].Value = (String)curDataRow["CodeValue"];
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        curViewRow.Cells["RunOrder"].Value = ( (Int16)curDataRow["RunOrder"] ).ToString();
                    }
                    myViewIdx = 0;
                    TourDivDataGridView.CurrentCell = TourDivDataGridView.Rows[myViewIdx].Cells["AgeGroup"];
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving data entries \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
            try {
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + TourDivDataGridView.Rows.Count.ToString();
            } catch {
                RowStatusLabel.Text = "";
            }
        }

        private void LoadDataButton_Click( object sender, EventArgs e ) {
            bool curLoadActive = true;
            String curMethodName = "Tournament:TourDivOrder:LoadDataButton";
            AgeGroupDropdownList curAgeGroupDropdownList = new AgeGroupDropdownList( myTourRow );

            String curMsg = "";
            bool curReturn = true;
            int rowsProc = 0;

            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            try {
                if ( myTourDivDataTable.Rows.Count > 0 ) {
                    String dialogMsg = "You have existing division order data!"
                        + "\n Do you want to reset the data?";
                    DialogResult msgResp =
                        MessageBox.Show( dialogMsg, "Change Warning",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
                    if ( msgResp == DialogResult.OK ) {
                        curLoadActive = true;
                    } else {
                        curLoadActive = false;
                    }
                }
                if ( curLoadActive ) {
                    try {
                        StringBuilder curSqlStmt = new StringBuilder("Delete DivOrder WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + curEvent + "'");
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        int curInsertCount = 0;
                        String curSqlStmtInsert = "Insert Into DivOrder ( "
                            + "SanctionId, Event, AgeGroup, RunOrder, LastUpdateDate "
                            + ") Values ( '" + mySanctionNum + "', '" + curEvent + "'";

                        foreach ( DataRow curRow in curAgeGroupDropdownList.AgeDivDataTable.Rows ) {
                            if ( !(( (String)curRow["Division"] ).Equals( "OF" ) ) ) {
                                curSqlStmt = new StringBuilder(curSqlStmtInsert
                                    + ", '" + (String)curRow["Division"] + "'"
                                    + ", " + ( ((int)curRow["SortSeq"]) * 10 ).ToString()
                                    + ", getDate() )");
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                curInsertCount += rowsProc;
                            }
                        }
                        Log.WriteFile( curMethodName + ":Rows=" + curInsertCount.ToString() + " " + curSqlStmtInsert );
                        MessageBox.Show( "Added rows=" + curInsertCount.ToString() + " to division order" );

                    } catch ( Exception excp ) {
                        curReturn = false;
                        curMsg = "Error attempting to load division data \n" + excp.Message;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    } finally {
                        navRefresh_Click( null, null );
                    }
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to load division data \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void RemoveDataButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TourDivOrder:RemoveDataButton";

            String curMsg = "";
            int rowsProc = 0;

            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            try {
                if ( myTourDivDataTable.Rows.Count > 0 ) {
                    try {
                        StringBuilder curSqlStmt = new StringBuilder("Delete DivOrder WHERE SanctionId = '" + mySanctionNum + "' AND Event = '" + curEvent + "'");
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    } catch ( Exception excp ) {
                        curMsg = "Error attempting to remove division data \n" + excp.Message;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    } finally {
                        navRefresh_Click( null, null );
                    }
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to remove division data \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void slalomButton_CheckedChanged( object sender, EventArgs e ) {
            if ( ( (RadioButton)sender ).Checked ) {
                this.Text = myWindowTitle + " - Slalom";
                checkModifyPrompt();
                if ( !( isDataModified ) ) {
                    myTourDivDataTable = getTourDivData();
                    loadDataView();
                }
            }
        }

        private void trickButton_CheckedChanged( object sender, EventArgs e ) {
            if ( ( (RadioButton)sender ).Checked ) {
                this.Text = myWindowTitle + " - Trick";
                checkModifyPrompt();
                if ( !( isDataModified ) ) {
                    myTourDivDataTable = getTourDivData();
                    loadDataView();
                }
            }
        }

        private void jumpButton_CheckedChanged( object sender, EventArgs e ) {
            if ( ( (RadioButton)sender ).Checked ) {
                this.Text = myWindowTitle + " - Jump";
                checkModifyPrompt();
                if ( !( isDataModified ) ) {
                    myTourDivDataTable = getTourDivData();
                    loadDataView();
                }
            }
        }

        private void TourDivDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView myDataView = (DataGridView)sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + myDataView.Rows.Count.ToString();
            if ( isDataModified && ( myViewIdx != e.RowIndex ) ) {
                try {
                    navSave_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( myViewIdx != e.RowIndex ) {
                    myViewIdx = e.RowIndex;
                }
            }

        }

        private void TourDivDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            if ( TourDivDataGridView.Rows.Count > 0 ) {
                myViewIdx = e.RowIndex;
                String curColName = TourDivDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = TourDivDataGridView.Rows[myViewIdx];
                if ( curColName.Equals( "RunOrder" ) ) {
                    if ( myOrigItemValue == null ) myOrigItemValue = "0";
                    if ( isObjectEmpty( curViewRow.Cells[e.ColumnIndex].Value ) ) {
                        if ( Convert.ToInt16( myOrigItemValue ) != 0 ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    } else {
                        Int16 curNum = Convert.ToInt16( (String)curViewRow.Cells[e.ColumnIndex].Value );
                        if ( curNum != Convert.ToInt16( myOrigItemValue ) ) {
                            isDataModified = true;
                            curViewRow.Cells["Updated"].Value = "Y";
                        }
                    }
                }
            }
        }

        private void TourDivDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            if ( TourDivDataGridView.Rows.Count > 0 ) {
                myViewIdx = e.RowIndex;
                String curColName = TourDivDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = TourDivDataGridView.Rows[myViewIdx];
                if ( curColName.Equals( "RunOrder" ) ) {
                    try {
                        Int16 curNum = Convert.ToInt16( e.FormattedValue.ToString() );
                        e.Cancel = false;
                    } catch {
                        e.Cancel = true;
                        MessageBox.Show( curColName + " must be numeric" );
                    }
                }
            }
        }

        private void TourDivDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( TourDivDataGridView.Rows.Count > 0 ) {
                if ( !( TourDivDataGridView.Columns[e.ColumnIndex].ReadOnly ) ) {
                    myViewIdx = e.RowIndex;
                    String curColName = TourDivDataGridView.Columns[e.ColumnIndex].Name;
                    DataGridViewRow curViewRow = TourDivDataGridView.Rows[myViewIdx];
                    if ( curColName.Equals( "RunOrder" ) ) {
                        try {
                            myOrigItemValue = Convert.ToInt16( (String)curViewRow.Cells[e.ColumnIndex].Value ).ToString();
                        } catch {
                            myOrigItemValue = "0";
                        }
                    }
                }
            }
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            bool CenterOnPage = true;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 12, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = false;
            curPrintDialog.AllowSelection = true;
            curPrintDialog.AllowSomePages = true;
            curPrintDialog.PrintToFile = false;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;

            if ( curPrintDialog.ShowDialog() == DialogResult.OK ) {
                String printTitle = Properties.Settings.Default.Mdi_Title
                    + "\n Sanction " + mySanctionNum + " held on " + myTourRow["EventDates"].ToString()
                    + "\n" + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 50, 50, 50, 50 );
                myPrintDataGrid = new DataGridViewPrinter( TourDivDataGridView, myPrintDoc,
                        CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

        private DataTable getTourDivData() {
            String curEvent = "Slalom";
            if ( slalomButton.Checked ) {
                curEvent = "Slalom";
            } else if ( trickButton.Checked ) {
                curEvent = "Trick";
            } else if ( jumpButton.Checked ) {
                curEvent = "Jump";
            }

            //'AWSAAgeGroup', 'IwwfAgeGroup', 'NcwsaAgeGroup'
            /*
            String curListNames = "";
            if ( myTourRules.ToLower().Equals( "awsa" ) ) {
                if ( mySkierClassList.compareClassChange( mySkierClass, "c" ) > 0 ) {
                    curListNames = "'AWSAAgeGroup', 'IwwfAgeGroup'";
                } else {
                    curListNames = "'AWSAAgeGroup'";
                }
            } else if ( myTourRules.ToLower().Equals( "iwwf" ) ) {
                    curListNames = "'IwwfAgeGroup'";
            } else if ( myTourRules.ToLower().Equals( "ncwsa" ) ) {
                curListNames = "'NcwsaAgeGroup', 'AWSAAgeGroup'";
            }
             */
            
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct E.PK, E.SanctionId, E.Event, E.AgeGroup, E.RunOrder, L.CodeValue " );
            curSqlStmt.Append( "FROM DivOrder E " );
            curSqlStmt.Append( "  INNER JOIN CodeValueList L ON L.ListName LIKE '%AgeGroup'" );
            curSqlStmt.Append( "        AND L.ListCode = E.AgeGroup " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + curEvent + "' " );
            curSqlStmt.Append( "Order by E.RunOrder " );

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactPhone, ContactEmail, M.LastName + ', ' + M.FirstName AS ContactName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN MemberList M ON ContactMemberId = MemberId " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty( object inObject ) {
            bool curReturnValue = false;
            if ( inObject == null ) {
                curReturnValue = true;
            } else if ( inObject == System.DBNull.Value ) {
                curReturnValue = true;
            } else if ( inObject.ToString().Length > 0 ) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

        private void TourDivDataGridView_Leave( object sender, EventArgs e ) {
            TourDivDataGridView.EndEdit();

        }
    }
}
