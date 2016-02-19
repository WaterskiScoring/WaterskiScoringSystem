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
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tools {
    public partial class ListNopsMaint : Form {
        private Boolean isDataModified = false;
        private String myOrigCellValue = "";
        private String myFilterCmd = "";
        private String mySortCmd = "";
        private int myViewIdx = 0;

        private SortDialogForm mySortDialogForm;
        private FilterDialogForm myFilterDialogForm;
        private TourExportDialogForm myExportDialogForm;

        public ListNopsMaint() {
            InitializeComponent();
        }

        private void ListNopsMaint_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.NopsMaint_Width > 0) {
                this.Width = Properties.Settings.Default.NopsMaint_Width;
            }
            if (Properties.Settings.Default.NopsMaint_Height > 0) {
                this.Height = Properties.Settings.Default.NopsMaint_Height;
            }
            if (Properties.Settings.Default.NopsMaint_Location.X > 0
                && Properties.Settings.Default.NopsMaint_Location.Y > 0) {
                this.Location = Properties.Settings.Default.NopsMaint_Location;
            }

            winStatusMsg.Text = "Retrieving NOPS data";
            Cursor.Current = Cursors.WaitCursor;

            mySortDialogForm = new SortDialogForm();
            mySortDialogForm.ColumnList = DataGridView.Columns;

            myFilterDialogForm = new Common.FilterDialogForm();
            myFilterDialogForm.ColumnList = DataGridView.Columns;

            LoadDataGridView();
        }

        private void ListNopsMaint_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.NopsMaint_Width = this.Size.Width;
                Properties.Settings.Default.NopsMaint_Height = this.Size.Height;
                Properties.Settings.Default.NopsMaint_Location = this.Location;
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

        private void LoadDataGridView() {
            winStatusMsg.Text = "Retrieving NOPS data entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;

                DataTable curDataTable = getDataList();
                if ( curDataTable.Rows.Count > 0 ) {
                    DataGridView.Rows.Clear();

                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        myViewIdx = DataGridView.Rows.Add();
                        curViewRow = DataGridView.Rows[myViewIdx];
                        curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["SkiYear"].Value = ( (byte)curDataRow["SkiYear"] ).ToString();
                        curViewRow.Cells["Event"].Value = (String)curDataRow["Event"];
                        curViewRow.Cells["AgeGroup"].Value = (String)curDataRow["AgeGroup"];
                        curViewRow.Cells["EventBasePoints"].Value = ( (Decimal)curDataRow["Base"] ).ToString();
                        curViewRow.Cells["BaseAdj"].Value = ( (Decimal)curDataRow["Adj"] ).ToString();
                        curViewRow.Cells["RatingMedian"].Value = ( (Decimal)curDataRow["RatingMedian"] ).ToString();
                        curViewRow.Cells["RatingRec"].Value = ( (Decimal)curDataRow["RatingRec"] ).ToString();
                        curViewRow.Cells["RatingOpen"].Value = ( (Decimal)curDataRow["RatingOpen"] ).ToString();
                        curViewRow.Cells["OverallBase"].Value = ( (Decimal)curDataRow["OverallBase"] ).ToString();
                        curViewRow.Cells["OverallExp"].Value = ( (Decimal)curDataRow["OverallExp"] ).ToString();
                        curViewRow.Cells["EventsReqd"].Value = ( (byte)curDataRow["EventsReqd"] ).ToString();
                    }
                }
                myViewIdx = 0;
                DataGridView.CurrentCell = DataGridView.Rows[myViewIdx].Cells["PK"];
                int curRowPos = myViewIdx + 1;
                RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + DataGridView.Rows.Count.ToString();
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving NOPS data entries \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            String curMethodName = "Tools:ListNopsMaint:navSaveItem_Click";
            String curMsg = "";
            bool curDataValid = true;
            int rowsProc = 0;
            Int64 curPK = 0;

            try {
                DataGridViewRow curViewRow = DataGridView.Rows[myViewIdx];
                String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                try {
                    curPK = Convert.ToInt64( (String)curViewRow.Cells["PK"].Value );
                } catch {
                    curPK = -1;
                }
                if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                    try {
                        String curEvent = "", curAgeGroup = "";
                        byte curSkiYear = 0, curEventsReqd = 0;
                        Decimal curEventBasePoints = 0, curBaseAdj = 0, curRatingMedian = 0, curRatingRec = 0
                            , curRatingOpen = 0, curOverallBase = 0, curOverallExp = 0;

                        try {
                            curSkiYear = Convert.ToByte((String)curViewRow.Cells["SkiYear"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curEvent = (String)curViewRow.Cells["Event"].Value;
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curAgeGroup = (String)curViewRow.Cells["AgeGroup"].Value;
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curEventsReqd = Convert.ToByte( (String)curViewRow.Cells["EventsReqd"].Value );
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curEventBasePoints = Convert.ToDecimal((String)curViewRow.Cells["EventBasePoints"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curBaseAdj = Convert.ToDecimal((String)curViewRow.Cells["BaseAdj"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curRatingMedian = Convert.ToDecimal((String)curViewRow.Cells["RatingMedian"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curRatingRec = Convert.ToDecimal((String)curViewRow.Cells["RatingRec"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curRatingOpen = Convert.ToDecimal((String)curViewRow.Cells["RatingOpen"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curOverallBase = Convert.ToDecimal((String)curViewRow.Cells["OverallBase"].Value);
                        } catch {
                            curDataValid = false;
                        }
                        try {
                            curOverallExp = Convert.ToDecimal((String)curViewRow.Cells["OverallExp"].Value);
                        } catch {
                            curDataValid = false;
                        }

                        StringBuilder curSqlStmt = new StringBuilder( "" );
                        if ( curDataValid ) {
                            if ( curPK > 0 ) {
                                curSqlStmt.Append( "Update NopsData Set " );
                                curSqlStmt.Append( "SkiYear = " + curSkiYear.ToString() + " " );
                                curSqlStmt.Append( ", Event = '" + curEvent + "' " );
                                curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "' " );
                                curSqlStmt.Append( ", Base = " + curEventBasePoints.ToString() + " " );
                                curSqlStmt.Append( ", Adj = " + curBaseAdj.ToString() + " " );
                                curSqlStmt.Append( ", RatingOpen = " + curRatingOpen.ToString() + " " );
                                curSqlStmt.Append( ", RatingRec = " + curRatingRec.ToString() + " " );
                                curSqlStmt.Append( ", RatingMedian = " + curRatingMedian.ToString() + " " );
                                curSqlStmt.Append( ", OverallBase = " + curOverallBase.ToString() + " " );
                                curSqlStmt.Append( ", OverallExp = " + curOverallExp.ToString() + " " );
                                curSqlStmt.Append( ", EventsReqd  = " + curEventsReqd.ToString() + " " );
                                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            } else {
                                curSqlStmt.Append( "Insert NopsData ( " );
                                curSqlStmt.Append( "SkiYear, Event, AgeGroup, Base, Adj, RatingOpen, " );
                                curSqlStmt.Append( "RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd " );
                                curSqlStmt.Append( ") Values ( " );
                                curSqlStmt.Append( curSkiYear.ToString() + " " );
                                curSqlStmt.Append( ", '" + curEvent + "' " );
                                curSqlStmt.Append( ", '" + curAgeGroup + "' " );
                                curSqlStmt.Append( ", " + curEventBasePoints.ToString() + " " );
                                curSqlStmt.Append( ", " + curBaseAdj.ToString() + " " );
                                curSqlStmt.Append( ", " + curRatingOpen.ToString() + " " );
                                curSqlStmt.Append( ", " + curRatingRec.ToString() + " " );
                                curSqlStmt.Append( ", " + curRatingMedian.ToString() + " " );
                                curSqlStmt.Append( ", " + curOverallBase.ToString() + " " );
                                curSqlStmt.Append( ", " + curOverallExp.ToString() + " " );
                                curSqlStmt.Append( ", " + curEventsReqd.ToString() + " " );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            }
                            winStatusMsg.Text = "Changes successfully saved";

                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            if ( rowsProc > 0 ) {
                                if ( curPK > 0 ) {
                                    DataTable curDataTable = getNewDataList( curSkiYear, curEvent, curAgeGroup );
                                    if ( curDataTable.Rows.Count > 0 ) {
                                        DataRow curDataRow = curDataTable.Rows[0];
                                        curViewRow.Cells["PK"].Value = (Int64)curDataRow["PK"];
                                        try {
                                            curViewRow.Cells["SortSeq"].Value = ((byte)curDataRow["SortSeq"]).ToString();
                                        } catch {
                                            curViewRow.Cells["SortSeq"].Value = "0";
                                        }
                                        
                                    }
                                }
                            }
                        }
                    } catch ( Exception excp ) {
                        curMsg = "Error attempting to update NOPS data \n" + excp.Message;
                        MessageBox.Show( curMsg );
                        Log.WriteFile( curMethodName + curMsg );
                    }
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to NOPS data \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void navSort_Click( object sender, EventArgs e ) {
            // Display the form as a modal dialog box.
            mySortDialogForm.SortCommand = mySortCmd;
            mySortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( mySortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCmd = mySortDialogForm.SortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCmd;
                LoadDataGridView();
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            myFilterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( myFilterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = myFilterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                LoadDataGridView();
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[1];
            String[] curTableName = { "NopsData" };

            curSelectCommand[0] = "Select * from NopsData ";
            myExportData.exportData( curTableName, curSelectCommand );

        }

        private void ListNopsMaint_FormClosing(object sender, FormClosingEventArgs e) {
            if (isDataModified) {
                String dialogMsg = "Changes have been made to currently displayed data!"
                    + "\n Do you want to save the changes before navigating to a new tournament?";
                DialogResult msgResp =
                    MessageBox.Show(dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1);
                if (msgResp == DialogResult.Yes) {
                    try {
                    } catch (Exception excp) {
                        e.Cancel = true;
                        MessageBox.Show("Error attempting to save changes \n" + excp.Message);
                    }
                } else if (msgResp == DialogResult.No) {
                    e.Cancel = true;
                } else {
                    e.Cancel = false;
                }
            }
        }

        private void DataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            int curRowPos = e.RowIndex + 1;
            RowStatusLabel.Text = "Row " + curRowPos.ToString() + " of " + curView.Rows.Count.ToString();

            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                    winStatusMsg.Text = "Previous row saved.";
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                //Sent current tournament registration row
                if ( curView.Rows[e.RowIndex].Cells[e.ColumnIndex] != null ) {
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells["AgeGroup"].Value ) ) ) {
                        myViewIdx = e.RowIndex;
                        isDataModified = false;
                    }
                }
            }
        }

        private void DataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Rows.Count > 0 ) {
                if ( !( curView.Columns[e.ColumnIndex].ReadOnly ) ) {
                    String curColName = curView.Columns[e.ColumnIndex].Name;
                    try {
                        myOrigCellValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    } catch {
                        myOrigCellValue = "";
                    }
                }
            }
        }

        private void DataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            if ( curView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = curView.Rows[e.RowIndex];
                if ( curColName.Equals( "Event" )
                    || curColName.Equals( "AgeGroup" )
                    ) {
                    if ( isObjectEmpty( e.FormattedValue ) ) {
                        e.Cancel = true;
                        MessageBox.Show( curColName + " is a required" );
                    } else {
                        e.Cancel = false;
                    }
                } else if ( curColName.Equals( "PK" ) ) {
                    if ( isObjectEmpty( e.FormattedValue ) ) {
                        e.Cancel = false;
                    } else {
                        try {
                            Int64 curNum = Convert.ToInt64( e.FormattedValue.ToString() );
                            e.Cancel = false;
                        } catch {
                            e.Cancel = true;
                            MessageBox.Show( curColName + " must be numeric (integer)" );
                        }
                    }
                } else if ( curColName.Equals( "SkiYear" )
                    || curColName.Equals( "EventsReqd" )
                ) {
                    if ( isObjectEmpty( e.FormattedValue ) ) {
                        e.Cancel = false;
                    } else {
                        try {
                            Byte curNum = Convert.ToByte( e.FormattedValue.ToString() );
                            e.Cancel = false;
                        } catch {
                            e.Cancel = true;
                            MessageBox.Show( curColName + " must be numeric (integer)" );
                        }
                    }
                } else if ( curColName.Equals( "EventBasePoints" )
                    || curColName.Equals( "BaseAdj" )
                    || curColName.Equals( "RatingOpen" )
                    || curColName.Equals( "RatingRec" )
                    || curColName.Equals( "RatingMedian" )
                    || curColName.Equals( "OverallBase" )
                    || curColName.Equals( "OverallExp" )
                ) {
                    if ( isObjectEmpty( e.FormattedValue ) ) {
                        e.Cancel = false;
                    } else {
                        try {
                            Decimal curNum = Convert.ToDecimal( e.FormattedValue.ToString() );
                            e.Cancel = false;
                        } catch {
                            e.Cancel = true;
                            MessageBox.Show( curColName + " must be numeric (integer)" );
                        }
                    }
                }
            }
        }

        private void DataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            curView.CurrentRow.ErrorText = "";
            myViewIdx = e.RowIndex;
            DataGridViewRow curViewRow = curView.Rows[myViewIdx];

            if ( curView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                if ( curColName.Equals( "xxxxx" ) ) {
                }
            }
        }

        private DataTable getNewDataList(byte inSkiYear, String inEvent, String inAgeGroup ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT  N.PK, SkiYear, Event, AgeGroup, N.SortSeq, Base, Adj, " );
            curSqlStmt.Append( "RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd " );
            curSqlStmt.Append( "FROM NopsData N LEFT OUTER JOIN CodeValueList V ON N.AgeGroup = V.ListCode " );
            curSqlStmt.Append( "WHERE V.ListName = 'AWSAAgeGroup' " );
            curSqlStmt.Append( "  And N.SkiYear = " + inSkiYear + " " );
            curSqlStmt.Append( "  And N.Event = '" + inEvent + "' " );
            curSqlStmt.Append( "  And N.AgeGroup = '" + inAgeGroup + "' " );
            curSqlStmt.Append( "ORDER BY N.SkiYear DESC, V.SortSeq, N.SortSeq " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getDataList() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Distinct N.PK, SkiYear, Event, AgeGroup, V.SortSeq as SortSeqDiv, N.SortSeq, Base, Adj, " );
            curSqlStmt.Append( "RatingOpen, RatingRec, RatingMedian, OverallBase, OverallExp, EventsReqd " );
            curSqlStmt.Append( "FROM NopsData N INNER JOIN CodeValueList V ON N.AgeGroup = V.ListCode " );
            curSqlStmt.Append( "WHERE V.ListName in ('AWSAAgeGroup', 'NcwsaAgeGroup', 'IwwfAgeGroup') " );
            curSqlStmt.Append( "ORDER BY N.SkiYear DESC, V.SortSeq, N.SortSeq " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData( String inSelectStmt ) {
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

    }
}
