using System;
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
    public partial class BoatUse : Form {
        private String mySanctionNum;
        private bool isDataModified;
        private int myViewRowIdx;
        private int myBoatListIdx;
        private int myOrigNumCellValue;
        private String myOrigStgCellValue;
        private DataTable myTourBoatUseDataTable;

        private DataGridViewPrinter myPrintDataGrid;
        private PrintDocument myPrintDoc;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public BoatUse() {
            InitializeComponent();
        }

        private void BoatUse_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.BoatUse_Width > 0 ) {
                this.Width = Properties.Settings.Default.BoatUse_Width;
            }
            if ( Properties.Settings.Default.BoatUse_Height > 0 ) {
                this.Height = Properties.Settings.Default.BoatUse_Height;
            }
            if ( Properties.Settings.Default.BoatUse_Location.X > 0
                && Properties.Settings.Default.BoatUse_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.BoatUse_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            Cursor.Current = Cursors.WaitCursor;

            isDataModified = false;
            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                    navRefresh_Click( null, null );
                    myViewRowIdx = 0;
                    getApprovedTowboats();
                }
            }
            Cursor.Current = Cursors.Default;
        }

        private void BoatUse_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.BoatUse_Width = this.Size.Width;
                Properties.Settings.Default.BoatUse_Height = this.Size.Height;
                Properties.Settings.Default.BoatUse_Location = this.Location;
            }
        }

        private void BoatUse_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                String dialogMsg = "You have outstanding changes on the window."
                    + "\n Do you want close the window without correcting errors?"
                    + "\n Click Yes to close without saving"
                    + "\n Click No to cancel close so you can save your data";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if (msgResp == DialogResult.Yes) {
                    e.Cancel = false;
                } else if (msgResp == DialogResult.No) {
                    e.Cancel = true;
                } else {
                    e.Cancel = false;
                }
            } else {
                e.Cancel = false;
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

        private void navRefresh_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( isDataModified ) {
                try {
                    String dialogMsg = "You have outstanding changes on the window."
                        + "\n Do you want refresh the data without correcting errors?"
                        + "\n Click Yes to refresh without saving"
                        + "\n Click No to save data and then refresh the data";
                    DialogResult msgResp =
                        MessageBox.Show( dialogMsg, "Change Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button1 );
                    if (msgResp == DialogResult.Yes) {
                        isDataModified = false;
                    } else if (msgResp == DialogResult.No) {
                        navSaveItem_Click( null, null );
                    }
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                loadTourBoatUseView();
            }
        }

        private void loadTourBoatUseView() {
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament boat use entries";
            Cursor.Current = Cursors.WaitCursor;
            int curRowIdx = myViewRowIdx;

            tourBoatUseDataGridView.Rows.Clear();
            getTourBoatUseData();
            tourBoatUseDataGridView.BeginInvoke( (MethodInvoker)delegate() {
                Application.DoEvents();
                winStatusMsg.Text = "Tournament boat use list retrieved";
            } );

            if ( myTourBoatUseDataTable.Rows.Count > 0 ) {
                DataGridViewRow curViewRow;
                isDataModified = false;
                foreach ( DataRow curDataRow in myTourBoatUseDataTable.Rows ) {
                    curRowIdx = tourBoatUseDataGridView.Rows.Add();
                    curViewRow = tourBoatUseDataGridView.Rows[curRowIdx];

                    Int64 curNumValue = (Int64)curDataRow["PK"];
                    curViewRow.Cells["PK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                    curViewRow.Cells["Updated"].Value = "Y";
                    curViewRow.Cells["TourBoatSeq"].Value = ( (Int16)curDataRow["TourBoatSeq"] ).ToString();
                    curViewRow.Cells["SanctionId"].Value = (String)curDataRow["SanctionId"];
                    try {
                        curViewRow.Cells["BoatCode"].Value = (String)curDataRow["HullId"];
                    } catch {
                        curViewRow.Cells["BoatCode"].Value = "";
                    }
                    try {
                        curViewRow.Cells["SlalomUsed"].Value = (String)curDataRow["SlalomUsed"];
                    } catch {
                        curViewRow.Cells["SlalomUsed"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["TrickUsed"].Value = (String)curDataRow["TrickUsed"];
                    } catch {
                        curViewRow.Cells["TrickUsed"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["JumpUsed"].Value = (String)curDataRow["JumpUsed"];
                    } catch {
                        curViewRow.Cells["JumpUsed"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["SlalomCredit"].Value = (String)curDataRow["SlalomCredit"];
                    } catch {
                        curViewRow.Cells["SlalomCredit"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["TrickCredit"].Value = (String)curDataRow["TrickCredit"];
                    } catch {
                        curViewRow.Cells["TrickCredit"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["JumpCredit"].Value = (String)curDataRow["JumpCredit"];
                    } catch {
                        curViewRow.Cells["JumpCredit"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["CertOfInsurance"].Value = (String)curDataRow["CertOfInsurance"];
                    } catch {
                        curViewRow.Cells["CertOfInsurance"].Value = "N";
                    }
                    try {
                    curViewRow.Cells["ModelYear"].Value = ((Int16)curDataRow["ModelYear"]).ToString();
                    } catch {
                        curViewRow.Cells["ModelYear"].Value = "";
                    }
                    try {
                    curViewRow.Cells["BoatModel"].Value = (String)curDataRow["BoatModel"];
                    } catch {
                        curViewRow.Cells["BoatModel"].Value = "";
                    }
                    try {
                    curViewRow.Cells["SpeedControlVersion"].Value = (String)curDataRow["SpeedControlVersion"];
                    } catch {
                        curViewRow.Cells["SpeedControlVersion"].Value = "";
                    }
                    try {
                    curViewRow.Cells["Owner"].Value = (String)curDataRow["Owner"];
                    } catch {
                        curViewRow.Cells["Owner"].Value = "";
                    }
                    try {
                    curViewRow.Cells["InsuranceCompany"].Value = (String)curDataRow["InsuranceCompany"];
                    } catch {
                        curViewRow.Cells["InsuranceCompany"].Value = "";
                    }
                    try {
                    curViewRow.Cells["PreEventNotes"].Value = (String)curDataRow["PreEventNotes"];
                    } catch {
                        curViewRow.Cells["PreEventNotes"].Value = "";
                    }
                    try {
                    curViewRow.Cells["PostEventNotes"].Value = (String)curDataRow["PostEventNotes"];
                    } catch {
                        curViewRow.Cells["PostEventNotes"].Value = "";
                    }
                    try {
                    curViewRow.Cells["Notes"].Value = (String)curDataRow["Notes"];
                    } catch {
                        curViewRow.Cells["Notes"].Value = "";
                    }
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void getApprovedTowboats() {
            int curIdx = 0;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, CodeDesc" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'ApprovedBoats'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );

            foreach ( DataRow curRow in curDataTable.Rows ) {
                String[] boatSpecList = curRow["CodeDesc"].ToString().Split( '|' );
                listApprovedBoatsDataGridView.Rows.Add( 1 );
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatCodeApproved"].Value = curRow["ListCode"].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["BoatModelApproved"].Value = curRow["CodeValue"].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["HullStatus"].Value = boatSpecList[0].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["EngineSpec"].Value = boatSpecList[1].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["FuelDel"].Value = boatSpecList[2].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["Transmission"].Value = boatSpecList[3].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["Prop"].Value = boatSpecList[4].ToString();
                listApprovedBoatsDataGridView.Rows[curIdx].Cells["SpeedControl"].Value = boatSpecList[5].ToString();
                curIdx++;
            }
            myBoatListIdx = 0;
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
                int curPK = 0, rowsProc = 0;
                DataGridViewRow curViewRow = tourBoatUseDataGridView.Rows[myViewRowIdx];
                String curUpdateStatus = (String)curViewRow.Cells["Updated"].Value;
                String curSanctionId = (String)curViewRow.Cells["SanctionId"].Value;
                try {
                    curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );
                } catch {
                    curPK = 0;
                }
                if (curUpdateStatus.ToUpper().Equals( "Y" ) && curSanctionId.Length > 1 ) {
                    if (isDataValid( curViewRow )) {
                        try {
                            myDbConn.Open();
                            mySqlStmt = myDbConn.CreateCommand();
                            mySqlStmt.CommandText = "";
                            String curModelYear = "";
                            try {
                                curModelYear = Convert.ToInt16( curViewRow.Cells["ModelYear"].Value.ToString() ).ToString();
                            } catch {
                                curModelYear = "Null";
                            }
                            String curBoatCode, curBoatModel, curSpeedControlVersion, curOwner, curInsuranceCompany, curPreEventNotes, curPostEventNotes, curNotes, curTourBoatSeq;
                            try {
                                curBoatCode = curViewRow.Cells["BoatCode"].Value.ToString();
                            } catch {
                                curBoatCode = "";
                            }
                            try {
                                curBoatModel = curViewRow.Cells["BoatModel"].Value.ToString();
                                curBoatModel = curBoatModel.Replace( "'", "''" );
                            } catch {
                                curBoatModel = "";
                            }
                            try {
                                curSpeedControlVersion = curViewRow.Cells["SpeedControlVersion"].Value.ToString();
                                curSpeedControlVersion = curSpeedControlVersion.Replace( "'", "''" );
                            } catch {
                                curSpeedControlVersion = "";
                            }
                            try {
                                curOwner = curViewRow.Cells["Owner"].Value.ToString();
                                curOwner = curOwner.Replace( "'", "''" );
                            } catch {
                                curOwner = "";
                            }
                            try {
                                curInsuranceCompany = curViewRow.Cells["InsuranceCompany"].Value.ToString();
                                curInsuranceCompany = curInsuranceCompany.Replace( "'", "''" );
                            } catch {
                                curInsuranceCompany = "";
                            }
                            try {
                                curPreEventNotes = curViewRow.Cells["PreEventNotes"].Value.ToString();
                                curPreEventNotes = curPreEventNotes.Replace( "'", "''" );
                            } catch {
                                curPreEventNotes = "";
                            }
                            try {
                                curPostEventNotes = curViewRow.Cells["PostEventNotes"].Value.ToString();
                                curPostEventNotes = curPostEventNotes.Replace( "'", "''" );
                            } catch {
                                curPostEventNotes = "";
                            }
                            try {
                                curNotes = curViewRow.Cells["Notes"].Value.ToString();
                                curNotes = curNotes.Replace( "'", "''" );
                            } catch {
                                curNotes = "";
                            }
                            try {
                                curTourBoatSeq = curViewRow.Cells["TourBoatSeq"].Value.ToString();
                                if (Convert.ToInt16( curTourBoatSeq ) < 1) {
                                    curTourBoatSeq = curViewRow.Index.ToString( "#0" );
                                }
                            } catch {
                                curTourBoatSeq = curViewRow.Index.ToString( "#0" );
                            }


                            if (curPK > 0) {
                                curSqlStmt.Append( "Update TourBoatUse Set " );
                                curSqlStmt.Append( " SanctionId = '" + curSanctionId + "'" );
                                curSqlStmt.Append( ", TourBoatSeq = " + curTourBoatSeq );
                                curSqlStmt.Append( ", HullId = '" + curBoatCode + "'" );
                                curSqlStmt.Append( ", SlalomUsed = '" + curViewRow.Cells["SlalomUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", TrickUsed = '" + curViewRow.Cells["TrickUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", JumpUsed = '" + curViewRow.Cells["JumpUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", SlalomCredit = '" + curViewRow.Cells["SlalomCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", TrickCredit = '" + curViewRow.Cells["TrickCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", JumpCredit = '" + curViewRow.Cells["JumpCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", CertOfInsurance = '" + curViewRow.Cells["CertOfInsurance"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", ModelYear = " + curModelYear );
                                curSqlStmt.Append( ", BoatModel = '" + curBoatModel + "'" );
                                curSqlStmt.Append( ", SpeedControlVersion = '" + curSpeedControlVersion + "'" );
                                curSqlStmt.Append( ", Owner = '" + curOwner + "'" );
                                curSqlStmt.Append( ", InsuranceCompany = '" + curInsuranceCompany + "'" );
                                curSqlStmt.Append( ", PreEventNotes = '" + curPreEventNotes + "'" );
                                curSqlStmt.Append( ", PostEventNotes = '" + curPostEventNotes + "'" );
                                curSqlStmt.Append( ", Notes = '" + curNotes + "'" );
                                curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                                mySqlStmt.CommandText = curSqlStmt.ToString();
                                rowsProc = mySqlStmt.ExecuteNonQuery();
                            } else {
                                curSqlStmt.Append( "Insert TourBoatUse ( " );
                                curSqlStmt.Append( "SanctionId, TourBoatSeq, HullId, SlalomUsed, TrickUsed, JumpUsed, SlalomCredit, TrickCredit, JumpCredit" );
                                curSqlStmt.Append( ", CertOfInsurance, ModelYear, BoatModel, SpeedControlVersion, Owner, InsuranceCompany, PreEventNotes, PostEventNotes, Notes" );
                                curSqlStmt.Append( ") Values ( " );
                                curSqlStmt.Append( "'" + curSanctionId + "'" );
                                curSqlStmt.Append( ", " + curTourBoatSeq );
                                curSqlStmt.Append( ", '" + curBoatCode + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["SlalomUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["TrickUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["JumpUsed"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["SlalomCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["TrickCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["JumpCredit"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", '" + curViewRow.Cells["CertOfInsurance"].Value.ToString() + "'" );
                                curSqlStmt.Append( ", " + curModelYear );
                                curSqlStmt.Append( ", '" + curBoatModel + "'" );
                                curSqlStmt.Append( ", '" + curSpeedControlVersion + "'" );
                                curSqlStmt.Append( ", '" + curOwner + "'" );
                                curSqlStmt.Append( ", '" + curInsuranceCompany + "'" );
                                curSqlStmt.Append( ", '" + curPreEventNotes + "'" );
                                curSqlStmt.Append( ", '" + curPostEventNotes + "'" );
                                curSqlStmt.Append( ", '" + curNotes + "'" );
                                curSqlStmt.Append( ")" );
                                mySqlStmt.CommandText = curSqlStmt.ToString();
                                rowsProc = mySqlStmt.ExecuteNonQuery();
                                if (rowsProc > 0) {
                                    UpdateNewRowPk( curSanctionId, curTourBoatSeq, myViewRowIdx );
                                }
                            }

                            winStatusMsg.Text = "Changes successfully saved";
                            isDataModified = false;
                        } catch (Exception excp) {
                            MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
                        } finally {
                            myDbConn.Close();
                        }
                    }
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
            }
        }

        private void UpdateNewRowPk( String inSanctionId, String inTourBoatSeq, int inViewRowIdx ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK FROM TourBoatUse " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' AND TourBoatSeq = '" + inTourBoatSeq + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                DataRow curRow = curDataTable.Rows[0];
                Int64 curPk = (Int64)curRow["PK"];
                tourBoatUseDataGridView.Rows[inViewRowIdx].Cells["PK"].Value = curPk.ToString();
            } else {
                MessageBox.Show( "Primary key not found for new entry for sequence number " + inTourBoatSeq );
            }
        }

        private void navAddItem_Click( object sender, EventArgs e ) {
            if (isDataModified) { 
                navSaveItem_Click( null, null ); 
            }
            if (!(isDataModified)) {
                if (listApprovedBoatsDataGridView.Visible) {
                    DataGridViewCellEventArgs curArgs = new DataGridViewCellEventArgs( 0, myBoatListIdx );
                    listApprovedBoatsDataGridView_CellContentDoubleClick( null, curArgs );
                } else {
                    SelectButton_Click( null, null );
                }
            }
        }

        private void navDeleteItem_Click( object sender, EventArgs e ) {
            if (isDataModified) { 
                navSaveItem_Click( null, null ); 
            }
            if (!( isDataModified )) {
                StringBuilder curSqlStmt = new StringBuilder( "" );
                try {
                    int curPK = 0, rowsProc = 0;
                    DataGridViewRow curViewRow = tourBoatUseDataGridView.Rows[myViewRowIdx];
                    try {
                        curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );
                    } catch {
                        curPK = 0;
                    }
                    if (curPK > 0) {
                        myDbConn.Open();
                        mySqlStmt = myDbConn.CreateCommand();
                        mySqlStmt.CommandText = "";
                        curSqlStmt.Append( "Delete TourBoatUse " );
                        curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                        mySqlStmt.CommandText = curSqlStmt.ToString();
                        rowsProc = mySqlStmt.ExecuteNonQuery();
                        winStatusMsg.Text = "Changes successfully saved";
                    }
                    isDataModified = false;
                    tourBoatUseDataGridView.Rows.RemoveAt( myViewRowIdx );
                    myViewRowIdx--;
                    if (myViewRowIdx < 0) myViewRowIdx = 0;
                } catch (Exception excp) {
                    MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
                } finally {
                    myDbConn.Close();
                }
            }
        }
        
        private void navExport_Click( object sender, EventArgs e ) {
            if ( isDataModified ) { navSaveItem_Click( null, null ); }

            ExportData myExportData = new ExportData();
            String mySelectCommand = "Select * from TourBoatUse "
                + " Where SanctionId = '" + mySanctionNum + "'";
            myExportData.exportData( "TourBoatUse", mySelectCommand );
        }

        private void SelectButton_Click( object sender, EventArgs e ) {
            if (isDataModified) { 
                navSaveItem_Click( null, null ); 
            }
            if (!( isDataModified )) {
                listApprovedBoatsDataGridView.Visible = true;
                BoatSelectInfoLabel.Visible = true;
                listApprovedBoatsDataGridView.Focus();
            }
        }

        private void listApprovedBoatsDataGridView_Leave( object sender, EventArgs e ) {
            listApprovedBoatsDataGridView.Visible = false;
            BoatSelectInfoLabel.Visible = false;
        }

        private void listApprovedBoatsDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            myBoatListIdx = e.RowIndex;
        }

        private void listApprovedBoatsDataGridView_CellContentDoubleClick( object sender, DataGridViewCellEventArgs e ) {
            DataGridViewRow curViewRow;
            myViewRowIdx = tourBoatUseDataGridView.Rows.Add();
            isDataModified = false;
            curViewRow = tourBoatUseDataGridView.Rows[myViewRowIdx];

            myBoatListIdx = e.RowIndex;
            curViewRow.Cells["PK"].Value = "0";
            curViewRow.Cells["Updated"].Value = "Y";
            curViewRow.Cells["TourBoatSeq"].Value = "0";
            curViewRow.Cells["SanctionId"].Value = mySanctionNum;
            curViewRow.Cells["BoatModel"].Value = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatModelApproved"].Value.ToString();
            curViewRow.Cells["SpeedControlVersion"].Value = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["SpeedControl"].Value.ToString();
            curViewRow.Cells["BoatCode"].Value = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["BoatCodeApproved"].Value.ToString();

            curViewRow.Cells["SlalomUsed"].Value = "N";
            curViewRow.Cells["TrickUsed"].Value = "N";
            curViewRow.Cells["JumpUsed"].Value = "N";
            curViewRow.Cells["SlalomCredit"].Value = "N";
            curViewRow.Cells["TrickCredit"].Value = "N";
            curViewRow.Cells["JumpCredit"].Value = "N";
            curViewRow.Cells["CertOfInsurance"].Value = "N";
            
            tourBoatUseDataGridView.CurrentCell = tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["ModelYear"];
            BoatSelectInfoLabel.Visible = false;
            listApprovedBoatsDataGridView.Visible = false;
            tourBoatUseDataGridView.Select();
            isDataModified = true;
        }

        private void tourBoatUseDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;

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
                    if ( !( isObjectEmpty( curView.Rows[e.RowIndex].Cells["SanctionId"].Value ) ) ) {
                        myViewRowIdx = e.RowIndex;
                        isDataModified = false;
                    }
                }
            }

        }

        private void tourBoatUseDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;
            String curColName = curView.Columns[e.ColumnIndex].Name;

            if ( curColName.StartsWith( "ModelYear" )
                ) {
                try {
                    myOrigNumCellValue = Convert.ToInt32( curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                } catch {
                    myOrigNumCellValue = 0;
                }
            } else {
                try {
                    myOrigStgCellValue = (String)curView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                } catch {
                    myOrigStgCellValue = "";
                }
            }
        }

        private void tourBoatUseDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            DataGridView curView = (DataGridView)sender;

            if ( tourBoatUseDataGridView.Rows.Count > 0 ) {
                String curColName = curView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curRow = (DataGridViewRow)curView.Rows[e.RowIndex];

                if ( curColName.StartsWith( "ModelYear" ) ) {
                    if ( isObjectEmpty( curRow.Cells[e.ColumnIndex].Value ) ) {
                        isDataModified = true;
                        MessageBox.Show( "Model year is a required field" );
                    } else {
                        int curValue = Convert.ToInt32( curRow.Cells[curColName].Value);
                        if ( curValue != myOrigNumCellValue ) {
                            isDataModified = true;
                        }
                    }
                } else {
                    if ( isObjectEmpty( curRow.Cells[e.ColumnIndex].Value ) ) {
                        if ( !(isObjectEmpty(myOrigStgCellValue )) ) {
                            isDataModified = true;
                        }
                    } else {
                        String curValue = (String)curRow.Cells[curColName].Value;
                        if ( !( curValue.Equals( myOrigStgCellValue ) ) ) {
                            isDataModified = true;
                        }
                    }
                }
            }
        }

        private void tourBoatUseDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView curView = (DataGridView)sender;
            String curColName = curView.Columns[e.ColumnIndex].Name;
            if (curColName.Equals( "SlalomUsed" )
                 || curColName.Equals( "TrickUsed" )
                 || curColName.Equals( "JumpUsed" )
                 || curColName.Equals( "SlalomCredit" )
                 || curColName.Equals( "TrickCredit" )
                 || curColName.Equals( "JumpCredit" )
                 || curColName.Equals( "CertOfInsurance" )
               ) {
                SendKeys.Send( "{TAB}" );
            }
        }

        private bool isDataValid(DataGridViewRow curViewRow) {
            bool curReturnValue = true;

            String curValue = "";
            try {
                curValue = Convert.ToInt16( curViewRow.Cells["ModelYear"].Value.ToString() ).ToString();
            } catch {
                curReturnValue = false;
                MessageBox.Show( "Model Year is a required field" );
            }
            try {
                curValue = curViewRow.Cells["BoatCode"].Value.ToString();
                if (curValue.Length == 0) {
                    curReturnValue = false;
                    MessageBox.Show( "Boat Code is a required field" );
                }
            } catch {
                curReturnValue = false;
                MessageBox.Show( "Boat Code is a required field" );
            }
            try {
                curValue = curViewRow.Cells["BoatModel"].Value.ToString();
                if (curValue.Length == 0) {
                    curReturnValue = false;
                    MessageBox.Show( "Model is a required field" );
                }
            } catch {
                curReturnValue = false;
                MessageBox.Show( "Model is a required field" );
            }

            return curReturnValue;
    }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            bool CenterOnPage = false;
            bool WithTitle = true;
            bool WithPaging = true;
            Font fontPrintTitle = new Font( "Arial Narrow", 14, FontStyle.Bold );
            Font fontPrintFooter = new Font( "Times New Roman", 10 );

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
                String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDoc.DefaultPageSettings.Landscape = false;
                myPrintDataGrid = new DataGridViewPrinter( tourBoatUseDataGridView, myPrintDoc,
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

        private void getTourBoatUseData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT U.PK, U.SanctionId, U.HullId, U.BoatModel, ListCode " );
            curSqlStmt.Append( "FROM TourBoatUse U" );
            curSqlStmt.Append( "     Inner Join CodeValueList on ListName = 'ApprovedBoats' AND CodeValue = U.BoatModel " );
            curSqlStmt.Append( "WHERE (U.SanctionId = '" + mySanctionNum + "') and (U.HullId is null or U.HullId = '') " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count > 0) {
                int rowsProc = 0;
                try {
                    myDbConn.Open();
                    mySqlStmt = myDbConn.CreateCommand();
                    mySqlStmt.CommandText = "";

                    foreach (DataRow curRow in curDataTable.Rows) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update TourBoatUse " );
                        curSqlStmt.Append( "Set HullId = '" + (String)curRow["ListCode"] + "' " );
                        curSqlStmt.Append( "Where PK = " + ((Int64)curRow["PK"]).ToString() );
                        mySqlStmt.CommandText = curSqlStmt.ToString();
                        rowsProc = mySqlStmt.ExecuteNonQuery();
                    }


                } catch (Exception excp) {
                    MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
                } finally {
                    myDbConn.Close();
                }
            }

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, SlalomCredit, SlalomUsed, TrickCredit, TrickUsed, JumpCredit, JumpUsed" );
            curSqlStmt.Append( ", HullId, BoatModel, ModelYear, Owner, CertOfInsurance, InsuranceCompany" );
            curSqlStmt.Append( ", Notes, PostEventNotes, PreEventNotes, SpeedControlVersion, TourBoatSeq " );
            curSqlStmt.Append( "FROM TourBoatUse " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            myTourBoatUseDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getTourData(String inSanctionId) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", ContactMemberId, ContactAddress, ContactPhone, ContactEmail, CP.SkierName AS ContactName " );
            curSqlStmt.Append( ", ChiefJudgeMemberId, ChiefJudgeAddress, ChiefJudgePhone, ChiefJudgeEmail, CJ.SkierName AS ChiefJudgeName" );
            curSqlStmt.Append( ", ChiefDriverMemberId, ChiefDriverAddress, ChiefDriverPhone, ChiefDriverEmail, CD.SkierName AS ChiefDriverName " );
            curSqlStmt.Append( ", SafetyDirMemberId, SafetyDirAddress, SafetyDirPhone, SafetyDirEmail, SD.SkierName AS SafetyDirName " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "  LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CP " );
            curSqlStmt.Append( "    ON CP.SanctionId = T.SanctionId AND CP.MemberId = T.ContactMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CJ " );
            curSqlStmt.Append( "    ON CJ.SanctionId = T.SanctionId AND CJ.MemberId = T.ChiefJudgeMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) CD " );
            curSqlStmt.Append( "    ON CD.SanctionId = T.SanctionId AND CD.MemberId = T.ChiefDriverMemberId " );
            curSqlStmt.Append( "  LEFT OUTER JOIN (Select Distinct SanctionId, MemberId, SkierName From TourReg ) SD " );
            curSqlStmt.Append( "    ON SD.SanctionId = T.SanctionId AND SD.MemberId = T.SafetyDirMemberId " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + inSanctionId + "' " );
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

    }
}
