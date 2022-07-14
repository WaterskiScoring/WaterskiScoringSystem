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
                    curViewRow.Cells["BoatCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "HullId", "" );

                    curViewRow.Cells["SlalomUsed"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomUsed", "N" );
                    curViewRow.Cells["TrickUsed"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickUsed", "N" );
                    curViewRow.Cells["JumpUsed"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpUsed", "N" );

                    curViewRow.Cells["SlalomCredit"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomCredit", "N" );
                    curViewRow.Cells["TrickCredit"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickCredit", "N" );
                    curViewRow.Cells["JumpCredit"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpCredit", "N" );

                    curViewRow.Cells["CertOfInsurance"].Value = HelperFunctions.getDataRowColValue( curDataRow, "CertOfInsurance", "N" );
                    curViewRow.Cells["ModelYear"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ModelYear", "" );

                    curViewRow.Cells["BoatModel"].Value = HelperFunctions.getDataRowColValue( curDataRow, "BoatModel", "" );
                    curViewRow.Cells["SpeedControlVersion"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SpeedControlVersion", "" );
                    curViewRow.Cells["Owner"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Owner", "" );

                    curViewRow.Cells["InsuranceCompany"].Value = HelperFunctions.getDataRowColValue( curDataRow, "InsuranceCompany", "" );
                    curViewRow.Cells["PreEventNotes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PreEventNotes", "" );
                    curViewRow.Cells["PostEventNotes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "PostEventNotes", "" );
                    curViewRow.Cells["Notes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Notes", "" );
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
            curSqlStmt.Append( " ORDER BY SortSeq DESC" );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );

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
                            String curModelYear = HelperFunctions.getViewRowColValue( curViewRow, "ModelYear", "Null" );
                            if ( !(curModelYear.Equals("NUll" )) ) {
                                if ( !(int.TryParse(curModelYear, out int tempNumValue )) ) curModelYear = "Null";
                            }

                            String curBoatCode = HelperFunctions.getViewRowColValue( curViewRow, "BoatCode", "" );
                            String curBoatModel = HelperFunctions.getViewRowColValue( curViewRow, "BoatModel", "" );
                            if ( curBoatModel.Length > 0 ) curBoatModel = curBoatModel.Replace( "'", "''" );

                            String curSpeedControlVersion = HelperFunctions.getViewRowColValue( curViewRow, "SpeedControlVersion", "" );
                            if ( curSpeedControlVersion.Length > 0 ) curSpeedControlVersion = curSpeedControlVersion.Replace( "'", "''" );

                            String curOwner = HelperFunctions.getViewRowColValue( curViewRow, "Owner", "" );
                            if ( curOwner.Length > 0 ) curOwner = curOwner.Replace( "'", "''" );

                            String curInsuranceCompany = HelperFunctions.getViewRowColValue( curViewRow, "InsuranceCompany", "" );
                            if ( curInsuranceCompany.Length > 0 ) curInsuranceCompany = curInsuranceCompany.Replace( "'", "''" );

                            String curPreEventNotes = HelperFunctions.getViewRowColValue( curViewRow, "PreEventNotes", "" );
                            if ( curPreEventNotes.Length > 0 ) curPreEventNotes = curPreEventNotes.Replace( "'", "''" );

                            String curPostEventNotes = HelperFunctions.getViewRowColValue( curViewRow, "PostEventNotes", "" );
                            if ( curPostEventNotes.Length > 0 ) curPostEventNotes = curPostEventNotes.Replace( "'", "''" );

                            String curNotes = HelperFunctions.getViewRowColValue( curViewRow, "Notes", "" );
                            if ( curNotes.Length > 0 ) curNotes = curNotes.Replace( "'", "''" );

                            String curTourBoatSeq = HelperFunctions.getViewRowColValue( curViewRow, "TourBoatSeq", "0" );
                            if ( curTourBoatSeq.Length > 0 ) {
                                if ( !( int.TryParse( curTourBoatSeq, out int numTourBoatSeq ) ) ) {
                                    curTourBoatSeq = curViewRow.Index.ToString( "#0" );
                                } else {
                                    if ( numTourBoatSeq <= 0 ) curTourBoatSeq = curViewRow.Index.ToString( "#0" );
                                }
                            } else {
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
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

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
								rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
								if ( rowsProc > 0) {
                                    UpdateNewRowPk( curSanctionId, curTourBoatSeq, myViewRowIdx );
                                }
                            }

                            winStatusMsg.Text = "Changes successfully saved";
                            isDataModified = false;

						} catch (Exception excp) {
                            MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
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
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
            StringBuilder curSqlStmt = new StringBuilder( "" );
            try {
                int curPK = 0, rowsProc = 0;
                DataGridViewRow curViewRow = tourBoatUseDataGridView.Rows[myViewRowIdx];
                try {
                    curPK = Convert.ToInt32( curViewRow.Cells["PK"].Value );
                } catch {
                    curPK = 0;
                }
                if ( curPK > 0 ) {
                    curSqlStmt.Append( "Delete TourBoatUse " );
                    curSqlStmt.Append( " Where PK = " + curPK.ToString() );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    winStatusMsg.Text = "Changes successfully saved";
                }
                isDataModified = false;
                tourBoatUseDataGridView.Rows.RemoveAt( myViewRowIdx );
                myViewRowIdx--;
                if ( myViewRowIdx < 0 ) myViewRowIdx = 0;

            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
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

            tourBoatUseDataGridView.CurrentCell = tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["ModelYear"];

            String curHullStatus = listApprovedBoatsDataGridView.Rows[myBoatListIdx].Cells["HullStatus"].Value.ToString();
            int curModelYear = 0;
            if ( curHullStatus.Length == 4 ) {
                if ( int.TryParse( curHullStatus, out curModelYear ) ) {
                    tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["ModelYear"].Value = curModelYear.ToString();
                    tourBoatUseDataGridView.CurrentCell = tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["Owner"];
                }

            } else if ( curHullStatus.Length > 4 ) {
                if ( int.TryParse( curHullStatus.Substring(0, 4), out curModelYear ) ) {
                    tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["ModelYear"].Value = curModelYear.ToString();
                    tourBoatUseDataGridView.CurrentCell = tourBoatUseDataGridView.Rows[myViewRowIdx].Cells["Owner"];
                }
            }

            curViewRow.Cells["SlalomUsed"].Value = "N";
            curViewRow.Cells["TrickUsed"].Value = "N";
            curViewRow.Cells["JumpUsed"].Value = "N";
            curViewRow.Cells["SlalomCredit"].Value = "N";
            curViewRow.Cells["TrickCredit"].Value = "N";
            curViewRow.Cells["JumpCredit"].Value = "N";
            curViewRow.Cells["CertOfInsurance"].Value = "N";
            
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

            String curValue = HelperFunctions.getViewRowColValue( curViewRow, "ModelYear", "" );
            if ( !(int.TryParse( curValue, out int curModelYear)) ) {
                curReturnValue = false;
                MessageBox.Show( "Model Year is a required field" );
            }

            curValue = HelperFunctions.getViewRowColValue( curViewRow, "BoatCode", "" );
            if ( curValue.Length == 0 ) {
                curReturnValue = false;
                MessageBox.Show( "Boat Code is a required field" );
            }

            curValue = HelperFunctions.getViewRowColValue( curViewRow, "BoatModel", "" );
            if ( curValue.Length == 0 ) {
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
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
			if (curDataTable.Rows.Count > 0) {
                int rowsProc = 0;
                try {
                    foreach (DataRow curRow in curDataTable.Rows) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update TourBoatUse " );
                        curSqlStmt.Append( "Set HullId = '" + (String)curRow["ListCode"] + "' " );
                        curSqlStmt.Append( "Where PK = " + ((Int64)curRow["PK"]).ToString() );
						rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
					}


				} catch (Exception excp) {
                    MessageBox.Show( "Error attempting to update boat information \n" + excp.Message );
                }
            }

            curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, SlalomCredit, SlalomUsed, TrickCredit, TrickUsed, JumpCredit, JumpUsed" );
            curSqlStmt.Append( ", HullId, BoatModel, ModelYear, Owner, CertOfInsurance, InsuranceCompany" );
            curSqlStmt.Append( ", Notes, PostEventNotes, PreEventNotes, SpeedControlVersion, TourBoatSeq " );
            curSqlStmt.Append( "FROM TourBoatUse " );
            curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
            myTourBoatUseDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
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
            return DataAccess.getDataTable( curSqlStmt.ToString() );
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
