using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WaterskiScoringSystem.Tools;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tournament;
using WaterskiScoringSystem.Externalnterface;

namespace WaterskiScoringSystem.Admin {
    public partial class TourList : Form {
        #region Instance Variables
        private Boolean isDataModified = false;
        private Boolean isDataModifiedSanctionId = false;
        private Boolean isDataModifiedClass = false;
        private Boolean isDataModifiedSlalomRounds = false;
        private Boolean isDataModifiedTrickRounds = false;
        private Boolean isDataModifiedJumpRounds = false;

        private int myTourViewIdx = 0;

        private String myOrigSanctionId = "";
        private String myOrigEventClass = "";
        private String myOrigCellValue = "";
        private String mySortCommand = "";
        private String myFilterCmd = "";

        private DataGridViewRow myTourViewRow;
        private DataTable myTourDataTable;

        private TourProperties myTourProperties;
        private ListTourClass myTourClassList;
        private FedDropdownList myFedDropdownList = new FedDropdownList();
        private RuleDropdownList myRuleDropdownList = new RuleDropdownList();
        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private TourExportDialogForm exportDialogForm;
        private PrintDocument myPrintDoc;
        #endregion 

        public TourList() {
            InitializeComponent();

            editFederation.DataSource = myFedDropdownList.DropdownList;
            editFederation.DisplayMember = "ItemName";
            editFederation.ValueMember = "ItemValue";
            editFederation.SelectedText = "ItemName";
            editFederation.SelectedItem = "ItemValue";

            myTourClassList = new ListTourClass();
            myTourClassList.ListTourClassLoad();
            editClass.DataSource = myTourClassList.DropdownList;
            editClass.DisplayMember = "ItemName";
            editClass.ValueMember = "ItemValue";

            editRules.DataSource = myRuleDropdownList.DropdownList;
            editRules.DisplayMember = "ItemName";
            editRules.ValueMember = "ItemValue";
            editRules.SelectedText = "ItemName";
            editRules.SelectedItem = "ItemValue";

        }

        private void TourList_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.TourList_Width > 0) {
                this.Width = Properties.Settings.Default.TourList_Width;
            }
            if (Properties.Settings.Default.TourList_Height > 0) {
                this.Height = Properties.Settings.Default.TourList_Height;
            }
            if (Properties.Settings.Default.TourList_Location.X > 0
                && Properties.Settings.Default.TourList_Location.Y > 0) {
                this.Location = Properties.Settings.Default.TourList_Location;
            }
            myTourProperties = TourProperties.Instance;
            mySortCommand = "SanctionId ASC";

            sortDialogForm = new SortDialogForm();
            sortDialogForm.ColumnList = dataGridView.Columns;

            filterDialogForm = new Common.FilterDialogForm();
            filterDialogForm.ColumnList = dataGridView.Columns;

            //Retrieve tournament list and set current position to active tournament
            navRefresh_Click( null, null );
        }

        private void navRefresh_Click(object sender, EventArgs e) {
            try {
                winStatusMsg.Text = "Retrieving Tournament list";
                Cursor.Current = Cursors.WaitCursor;

                myTourDataTable = getTourData();
                loadTourView();

            } catch ( Exception ex ) {
                MessageBox.Show("Error attempting to retrieve tournament members\n" + ex.Message);
            }
        }

        private void dataGridView_DataError( object sender, DataGridViewDataErrorEventArgs e ) {
            MessageBox.Show( "Error happened " + e.Context.ToString() + "\n Exception Message: " + e.Exception.Message );
            if ( ( e.Exception ) is ConstraintException ) {
                DataGridView view = (DataGridView)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                e.ThrowException = false;
            }
        }

        private void TourList_FormClosing( object sender, FormClosingEventArgs e ) {
            try {
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
                    if ( msgResp == DialogResult.Yes ) {
                        e.Cancel = true;
                    } else if ( msgResp == DialogResult.No ) {
                        e.Cancel = false;
                    } else {
                        e.Cancel = false;
                    }
                } else {
                    e.Cancel = false;
                }
            
            } catch ( Exception excp ) {
                String dialogMsg = "Error attempting to save changes "
                    + "\n" + excp.Message
                    + "\n\n Do you want close the window without correcting errors?";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if ( msgResp == DialogResult.Yes ) {
                    e.Cancel = true;
                } else if ( msgResp == DialogResult.No ) {
                    e.Cancel = false;
                } else {
                    e.Cancel = false;
                }
            }
        }

        private void TourList_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.TourList_Width = this.Size.Width;
                Properties.Settings.Default.TourList_Height = this.Size.Height;
                Properties.Settings.Default.TourList_Location = this.Location;
            }
        }

        private void loadTourView() {
            int curTourActiveIdx = 0;
            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament list";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;
                isDataModifiedSanctionId = false;
                isDataModifiedClass = false;
                isDataModifiedSlalomRounds = false;
                isDataModifiedTrickRounds = false;
                isDataModifiedJumpRounds = false;

                String curSanctionId = "";
                String curActiveSanctionId = Properties.Settings.Default.AppSanctionNum;
                if ( curActiveSanctionId == null ) {
                    curActiveSanctionId = "";
                } else {
                    if ( curActiveSanctionId.Length < 6 ) {
                        curActiveSanctionId = "";
                    }
                }

                dataGridView.Rows.Clear();
                if ( myTourDataTable.Rows.Count > 0 ) {
                    myTourDataTable.DefaultView.Sort = mySortCommand;
                    myTourDataTable.DefaultView.RowFilter = myFilterCmd;
                    DataTable curDataTable = myTourDataTable.DefaultView.ToTable();
                    
                    DataGridViewRow curViewRow;
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        myTourViewIdx = dataGridView.Rows.Add();
                        curViewRow = dataGridView.Rows[myTourViewIdx];

                        curSanctionId = HelperFunctions.getDataRowColValue( curDataRow, "SanctionId", "" );
                        curViewRow.Cells["SanctionId"].Value = curSanctionId;

                        curViewRow.Cells["SanctionEditCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SanctionEditCode", "" );

                        curViewRow.Cells["TourName"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Name", "" );
                        curViewRow.Cells["TourClass"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Class", "" );
                        curViewRow.Cells["TourFederation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Federation", "" );
                        curViewRow.Cells["EventDates"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventDates", "" );
                        curViewRow.Cells["TourDataLoc"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TourDataLoc", "" );
                        curViewRow.Cells["EventLocation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventLocation", "" );
                        curViewRow.Cells["Rules"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Rules", "" );
                        curViewRow.Cells["SlalomRounds"].Value = HelperFunctions.getDataRowColValue( curDataRow, "SlalomRounds", "" );
                        curViewRow.Cells["TrickRounds"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TrickRounds", "" );
                        curViewRow.Cells["JumpRounds"].Value = HelperFunctions.getDataRowColValue( curDataRow, "JumpRounds", "" );
                        
                        curViewRow.Cells["HcapSlalomBase"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapSlalomBase", "", 0 );
                        curViewRow.Cells["HcapTrickBase"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapTrickBase", "", 0 );
                        curViewRow.Cells["HcapJumpBase"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapJumpBase", "", 0 );
                        curViewRow.Cells["HcapSlalomPct"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapSlalomPct", "", 3 );
                        curViewRow.Cells["HcapTrickPct"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapTrickPct", "", 3 );
                        curViewRow.Cells["HcapJumpPct"].Value = HelperFunctions.getDataRowColValueDecimal( curDataRow, "HcapJumpPct", "", 3 );

                        if ( curSanctionId == curActiveSanctionId ) curTourActiveIdx = myTourViewIdx;
                    }

                    myTourViewIdx = curTourActiveIdx;
                    dataGridView.CurrentCell = dataGridView.Rows[myTourViewIdx].Cells["SanctionId"];
                    myTourViewRow = dataGridView.Rows[myTourViewIdx];
                    setEntryForEdit( myTourViewRow );
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament list \n" + ex.Message );
            }
            Cursor.Current = Cursors.Default;
        }

        private void dataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            DataGridView myDataView = (DataGridView)sender;

            //Update data if changes are detected
            if ( isDataModified && ( myTourViewIdx != e.RowIndex ) ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                if ( myTourViewIdx != e.RowIndex ) {
                    myTourViewIdx = e.RowIndex;
                    if ( HelperFunctions.isObjectPopulated( myDataView.Rows[myTourViewIdx].Cells["SanctionId"].Value ) ) {
                        myTourViewRow = (DataGridViewRow)myDataView.Rows[myTourViewIdx];
                        setEntryForEdit( myTourViewRow );
                    }
                }
            }
        }

        private void setEntryForEdit( DataGridViewRow curViewRow ) {
            editSanctionId.Text = HelperFunctions.getViewRowColValue( curViewRow, "SanctionId", "" );
			myOrigSanctionId = editSanctionId.Text;

            editName.Text = HelperFunctions.getViewRowColValue( curViewRow, "TourName", "" );
            editSanctionEditCode.Text = HelperFunctions.getViewRowColValue( curViewRow, "SanctionEditCode", "" );
            editClass.SelectedValue = HelperFunctions.getViewRowColValue( curViewRow, "TourClass", "" );

            editFederation.SelectedValue = HelperFunctions.getViewRowColValue( curViewRow, "TourFederation", "" );
            editEventDates.Text = HelperFunctions.getViewRowColValue( curViewRow, "EventDates", "" );
            editTourDataLoc.Text = HelperFunctions.getViewRowColValue( curViewRow, "TourDataLoc", "" );
            editEventLocation.Text = HelperFunctions.getViewRowColValue( curViewRow, "EventLocation", "" );
            editRules.SelectedValue = HelperFunctions.getViewRowColValue( curViewRow, "Rules", "" );
            editSlalomRounds.Text = HelperFunctions.getViewRowColValue( curViewRow, "SlalomRounds", "" );
            editTrickRounds.Text = HelperFunctions.getViewRowColValue( curViewRow, "TrickRounds", "" );
            editJumpRounds.Text = HelperFunctions.getViewRowColValue( curViewRow, "JumpRounds", "" );

            Decimal curValue = 0;
            Int16 curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
            try {
                curSlalomRounds = Convert.ToInt16( editSlalomRounds.Text );
            } catch {
                curSlalomRounds = 0;
            }
            if ( curSlalomRounds > 0 ) {
                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapSlalomBase", "0" );
                if ( curValue == 0 ) curValue = getEventRecordValue( editSanctionId.Text, "Slalom" );
                editHcapSlalomBase.Text = curValue.ToString( "##0" );

                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapSlalomPct", "0" );
                if ( curValue == 0 ) curValue = 0.95M;
                editHcapSlalomPct.Text = curValue.ToString( "0.##" );

            } else {
                editHcapSlalomBase.Text = "";
                editHcapSlalomPct.Text = "";
            }

            try {
                curTrickRounds = Convert.ToInt16( editTrickRounds.Text );
            } catch {
                curTrickRounds = 0;
            }
            if ( curTrickRounds > 0 ) {
                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapTrickBase", "0" );
                if ( curValue == 0 ) curValue = getEventRecordValue( editSanctionId.Text, "Trick" );
                editHcapTrickBase.Text = curValue.ToString( "#0000" );

                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapTrickPct", "0" );
                if ( curValue == 0 ) curValue = 0.95M;
                editHcapTrickPct.Text = curValue.ToString( "0.##" );

            } else {
                editHcapTrickBase.Text = "";
                editHcapTrickPct.Text = "";
            }

            try {
                curJumpRounds = Convert.ToInt16( editJumpRounds.Text );
            } catch {
                curJumpRounds = 0;
            }
            if ( curJumpRounds > 0 ) {
                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapJumpBase", "0" );
                if ( curValue == 0 ) curValue = getEventRecordValue( editSanctionId.Text, "Jump" );
                editHcapJumpBase.Text = curValue.ToString( "##0" );

                curValue = HelperFunctions.getViewRowColValueDecimal( curViewRow, "HcapJumpPct", "0" );
                if ( curValue == 0 ) curValue = 0.95M;
                editHcapJumpPct.Text = curValue.ToString( "0.##" );

            } else {
                editHcapJumpBase.Text = "";
                editHcapJumpPct.Text = "";
            }

            isDataModified = false;
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            try {
                ReportPropButton.Focus();
                if ( isDataModified ) {
                    if ( validReqdFields() ) {
						String tempOrigSanctionId = myOrigSanctionId;
						saveTourData();
                        if ( isDataModifiedSanctionId ) {
							myOrigSanctionId = tempOrigSanctionId;
							updateSanctionId();
                            isDataModifiedSanctionId = false;
							myOrigSanctionId = editSanctionId.Text;
						}
						if ( isDataModifiedClass ) {
                            updateEventClass();
                            isDataModifiedClass = false;
                        }

                        foreach ( DataGridViewRow curRow in dataGridView.Rows ) {
                            if ( curRow.Cells["SanctionId"].Value.ToString() == Properties.Settings.Default.AppSanctionNum ) {
								this.MdiParent.Text = Properties.Settings.Default.AppTitle
									+ " - " + Properties.Settings.Default.BuildVersion
									+ " - " + Properties.Settings.Default.AppSanctionNum
									+ " - " + dataGridView.Rows[myTourViewIdx].Cells["TourName"].Value.ToString();
								Properties.Settings.Default.Mdi_Title = this.MdiParent.Text;
                            }
                        }
                    } else {
                        String dialogMsg = "Validation errors encountered during save operation."
                            + "\n Do you want to correct changes?";
                        DialogResult msgResp =
                            MessageBox.Show( dialogMsg, "Change Warning",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1 );
                        if ( msgResp == DialogResult.Cancel ) {
                            isDataModified = false;
                            isDataModifiedSlalomRounds = false;
                            isDataModifiedTrickRounds = false;
                            isDataModifiedJumpRounds = false;
                        }
                    }
                }
            } catch (Exception excp) {
                MessageBox.Show("Error attempting to save changes \n" + excp.Message);
            }
        }

        private bool saveTourData() {
            String curMethodName = "Slalom:ScoreEntry:saveTourData";
            bool curReturn = true, isExistingSanction = false;
            int rowsProc = 0;
            StringBuilder curSqlStmt = null;

            String curViewSanctionId = (String)myTourViewRow.Cells["SanctionId"].Value;
			if ( curViewSanctionId == null ) {
				curViewSanctionId = "";
			} else {
				curSqlStmt = new StringBuilder( String.Format("Select SanctionId From Tournament Where SanctionId = '{0}'", curViewSanctionId ) );
				DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count > 0 ) isExistingSanction = true;

			}

			try {
                curSqlStmt = new StringBuilder( "" );
                try {
                    if ( isExistingSanction ) {
                        curSqlStmt.Append( "Update Tournament Set " );
                        curSqlStmt.Append( "Name = '" + encodeDataForSql(editName.Text.Trim()) + "' " );
                        curSqlStmt.Append( ", Class = '" + editClass.SelectedValue + "' " );
						curSqlStmt.Append( ", SanctionEditCode = '" + editSanctionEditCode.Text + "' " );
                        curSqlStmt.Append( ", Federation = '" + editFederation.SelectedValue + "' ");
                        curSqlStmt.Append( ", Rules = '" + editRules.SelectedValue + "' " );
                        curSqlStmt.Append( ", EventDates = '" + encodeDataForSql(editEventDates.Text) + "' " );
                        curSqlStmt.Append( ", EventLocation = '" + encodeDataForSql(editEventLocation.Text) + "' " );
                        curSqlStmt.Append( ", TourDataLoc = '" + encodeDataForSql(editTourDataLoc.Text) + "' " );
                        curSqlStmt.Append( ", SlalomRounds = " + editSlalomRounds.Text );
                        curSqlStmt.Append( ", JumpRounds = " + editJumpRounds.Text );
                        curSqlStmt.Append( ", TrickRounds = " + editTrickRounds.Text );
                        curSqlStmt.Append( ", HcapSlalomBase = " + editHcapSlalomBase.Text );
                        curSqlStmt.Append( ", HcapTrickBase = " + editHcapTrickBase.Text );
                        curSqlStmt.Append( ", HcapJumpBase = " + editHcapJumpBase.Text );
                        curSqlStmt.Append( ", HcapSlalomPct = " + editHcapSlalomPct.Text );
                        curSqlStmt.Append( ", HcapTrickPct = " + editHcapTrickPct.Text );
                        curSqlStmt.Append( ", HcapJumpPct = " + editHcapJumpPct.Text + " " );
                        curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
                        curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' " );

					} else {
                        curSqlStmt.Append( "Insert Tournament ( " );
                        curSqlStmt.Append( " SanctionId, Name, Class, SanctionEditCode, Federation, Rules, EventDates, EventLocation, TourDataLoc" );
                        curSqlStmt.Append( ", SlalomRounds, JumpRounds, TrickRounds" );
                        curSqlStmt.Append( ", HcapSlalomBase, HcapTrickBase, HcapJumpBase, HcapSlalomPct, HcapTrickPct, HcapJumpPct, LastUpdateDate" );
                        curSqlStmt.Append( ") Values ( " );
                        curSqlStmt.Append( "'" + editSanctionId.Text + "'" );
                        curSqlStmt.Append( ", '" + encodeDataForSql(editName.Text.Trim()) + "' " );
                        curSqlStmt.Append( ", '" + editClass.SelectedValue + "' " );
						curSqlStmt.Append( ", '" + editSanctionEditCode.Text + "' " );
						curSqlStmt.Append( ", '" + editFederation.SelectedValue + "' " );
                        curSqlStmt.Append( ", '" + editRules.SelectedValue + "' " );
                        curSqlStmt.Append( ", '" + encodeDataForSql(editEventDates.Text) + "' " );
                        curSqlStmt.Append( ", '" + encodeDataForSql(editEventLocation.Text) + "' " );
                        curSqlStmt.Append( ", '" + encodeDataForSql(editTourDataLoc.Text) + "' " );
                        curSqlStmt.Append( ", " + editSlalomRounds.Text );
                        curSqlStmt.Append( ", " + editJumpRounds.Text );
                        curSqlStmt.Append( ", " + editTrickRounds.Text );
                        curSqlStmt.Append( ", " + editHcapSlalomBase.Text );
                        curSqlStmt.Append( ", " + editHcapTrickBase.Text );
                        curSqlStmt.Append( ", " + editHcapJumpBase.Text );
                        curSqlStmt.Append( ", " + editHcapSlalomPct.Text );
                        curSqlStmt.Append( ", " + editHcapTrickPct.Text );
                        curSqlStmt.Append( ", " + editHcapJumpPct.Text );
                        curSqlStmt.Append( ", GETDATE()" );
                        curSqlStmt.Append( ") " );
                    }
                    rowsProc = DataAccess.ExecuteCommand(curSqlStmt.ToString());
                    if ( rowsProc > 0 ) {
                        isDataModified = false;
                        if (isDataModifiedSlalomRounds) {
                            Int16 curSlalomRounds = 0;
                            try {
                                curSlalomRounds = Convert.ToInt16( editSlalomRounds.Text );
                            } catch {
                                curSlalomRounds = 0;
                                editSlalomRounds.Text = "0";
                            }
                            //Delete slalom scores and entries from rounds that have been elimiated.
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete SlalomScore " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editSlalomRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //SlalomRecap
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete SlalomRecap " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editSlalomRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //EventRunOrder
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete EventRunOrder " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Slalom' AND Round > " + editSlalomRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //OfficialWorkAsgmt
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Slalom' AND Round > " + editSlalomRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //Delete event registrations if rounds set to zero
                            if (curSlalomRounds == 0) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete EventReg " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Slalom'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete DivOrder " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Slalom'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            }

                            isDataModifiedSlalomRounds = false;
                        }

                        if (isDataModifiedTrickRounds) {
                            Int16 curTrickRounds = 0;
                            try {
                                curTrickRounds = Convert.ToInt16( editTrickRounds.Text );
                            } catch {
                                curTrickRounds = 0;
                                editTrickRounds.Text = "0";
                            }
                            //Delete Trick scores and entries from rounds that have been elimiated.
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete TrickScore " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editTrickRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //TrickRecap
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete TrickPass " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editTrickRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //EventRunOrder
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete EventRunOrder " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Trick' AND Round > " + editTrickRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //OfficialWorkAsgmt
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Trick' AND Round > " + editTrickRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //Delete event registrations if rounds set to zero
                            if (curTrickRounds == 0) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete EventReg " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Trick'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete DivOrder " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Trick'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            }

                            isDataModifiedTrickRounds = false;
                        }

                        if (isDataModifiedJumpRounds) {
                            Int16 curJumpRounds = 0;
                            try {
                                curJumpRounds = Convert.ToInt16( editJumpRounds.Text );
                            } catch {
                                curJumpRounds = 0;
                                editJumpRounds.Text = "0";
                            }
                            //Delete Jump scores and entries from rounds that have been elimiated.
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete JumpScore " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editJumpRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //JumpRecap
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete JumpRecap " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Round > " + editJumpRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //EventRunOrder
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete EventRunOrder " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Jump' AND Round > " + editJumpRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //OfficialWorkAsgmt
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Delete OfficialWorkAsgmt " );
                            curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Jump' AND Round > " + editJumpRounds.Text );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            //Delete event registrations if rounds set to zero
                            if (curJumpRounds == 0) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete EventReg " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Jump'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Delete DivOrder " );
                                curSqlStmt.Append( "Where SanctionId = '" + curViewSanctionId + "' AND Event = 'Jump'" );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            }

                            isDataModifiedJumpRounds = false;
                        }

                        checkDivOrderStatus();

                    }
                } catch ( Exception excp ) {
                    curReturn = false;
                    String curMsg = ":Error attempting to update tournament information \n" + excp.Message;
                    MessageBox.Show( curMethodName + curMsg );
                }

                if ( !( isDataModified ) ) {
                    setViewRowFromEdit();
                }

            } catch ( Exception excp ) {
                curReturn = false;
                String curMsg = ":Error attempting to update tournament information \n" + excp.Message;
                MessageBox.Show( curMethodName + curMsg );
            }

            return curReturn;
        }
        
        private String encodeDataForSql( String inValue ) {
            String curValue = inValue.Replace( "'", "''" );
            return curValue;
        }
        
        private bool validReqdFields() {
            bool curReturn = true;
            if ( editSanctionId.Text.Length == 6 ) {
                try {
                    Int16 curValue = Convert.ToInt16( editSanctionId.Text.Substring(0, 2 ));
                } catch {
                    curReturn = false;
                    MessageBox.Show( "Sanction Id must be in the format ##X###" );
                }
                try {
                    Int16 curValue = Convert.ToInt16( editSanctionId.Text.Substring(3, 3 ));
                } catch {
                    curReturn = false;
                    MessageBox.Show( "Sanction Id must be in the format ##X###" );
                }
 
            } else {
                curReturn = false;
                MessageBox.Show( "Sanction id must be 6 characters" );
            }
            if ( editSlalomRounds.Text.Length == 0
                && editTrickRounds.Text.Length == 0
                && editJumpRounds.Text.Length == 0
                ) {
                curReturn = false;
                MessageBox.Show( "All event rounds must have a numeric value of zero or greater" );
            } else {
                Int16 curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;
                try {
                    curSlalomRounds = Convert.ToInt16( editSlalomRounds.Text );
                } catch {
                    curReturn = false;
                    MessageBox.Show( "Invalid numeric in slalom rounds field" );
                }
                try {
                    curTrickRounds = Convert.ToInt16( editTrickRounds.Text );
                } catch {
                    curReturn = false;
                    MessageBox.Show( "Invalid numeric in trick rounds field" );
                }
                try {
                    curJumpRounds = Convert.ToInt16( editJumpRounds.Text );
                } catch {
                    curReturn = false;
                    MessageBox.Show( "Invalid numeric in jump rounds field" );
                }
                if ( curSlalomRounds == 0 && curTrickRounds == 0 && curJumpRounds == 0 ) {
                    curReturn = false;
                    MessageBox.Show( "At least one event must have rounds greater than zero" );
                }
            }
            if ( editName.Text == null ) {
                curReturn = false;
                MessageBox.Show( "A name must be supplied for the tournament" );
            } else {
                if ( editName.Text.Length == 0 ) {
                    curReturn = false;
                    MessageBox.Show( "A name must be supplied for the tournament" );
                }
            }
            if ( editRules.SelectedValue == null ) {
                curReturn = false;
                MessageBox.Show( "A valid rules value must be selected" );
            } else {
                if ( editRules.Text.Length == 0 ) {
                    curReturn = false;
                    MessageBox.Show( "A valid rules value must be selected" );
                }
            }
            if ( editClass.SelectedValue == null ) {
                curReturn = false;
                MessageBox.Show( "A valid tournament class value must be selected" );
            } else {
                if ( editClass.Text.Length == 0 ) {
                    curReturn = false;
                    MessageBox.Show( "A valid tournament class value must be selected" );
                }
            }

            if ( editEventDates.Text == null ) {
                curReturn = false;
                MessageBox.Show( "A valid tournament event end date value must be provided" );
            } else {
                try {
                    DateTime curValue = Convert.ToDateTime( editEventDates.Text );
                } catch (Exception ex) {
                    curReturn = false;
                    MessageBox.Show( "A valid tournament event end date value must be provided \n\nException: " + ex.Message );
                }
            }

            if (editEventLocation.Text == null) {
                curReturn = false;
                MessageBox.Show( "An event location is required."
                    + "\nPlease enter this information in the following format:\n"
                    + "\nSite Name followed by a comma, then the city, followed by a comma, then the 2 character state abbreviation"
                    );
            } else {
                String curState = "", curCity = "", curSiteName = "";
                try {
                    String[] curEventLocation = editEventLocation.Text.Split( ',' );
                    if (curEventLocation.Length == 3) {
                        curSiteName = curEventLocation[0];
                        curCity = curEventLocation[1];
                        curState = curEventLocation[2];
                        if (curState.Length == 2) {
                            curReturn = false;
                            MessageBox.Show( "An event location is required."
                                + "\nPlease enter this information in the following format:\n"
                                + "\nSite Name followed by a comma, then the city, followed by a comma, "
                                + "\nthen the 2 character state abbreviation"
                                );
                        }
                    } else {
                        curReturn = false;
                        MessageBox.Show( "An event location is required."
                            + "\nPlease enter this information in the following format:\n"
                            + "\nSite Name followed by a comma, then the city, followed by a comma, "
                            + "\nthen the 2 character state abbreviation"
                            );
                    }
                } catch {
                    curReturn = false;
                    MessageBox.Show( "An event location is required."
                        + "\nPlease enter this information in the following format:\n"
                        + "\nSite Name followed by a comma, then the city, followed by a comma, "
                        + "\nthen the 2 character state abbreviation"
                        );
                }
            }

            if ( editTourDataLoc.Text == null ) {
                curReturn = false;
                MessageBox.Show( "A data directory must be supplied for the tournament."
                    + "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
            } else {
                if ( editTourDataLoc.Text.Length == 0 ) {
                    curReturn = false;
                    MessageBox.Show( "A data directory must be supplied for the tournament."
                        + "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
                }
            }

            Decimal curHcapSlalomBase = 0, curHcapTrickBase = 0, curHcapJumpBase = 0;
            Decimal curHcapSlalomPct = 0, curHcapTrickPct = 0, curHcapJumpPct = 0;
            try {
                if ( HelperFunctions.isObjectEmpty( editHcapSlalomBase.Text ) ) {
                    editHcapSlalomBase.Text = "0";
                } else {
                    curHcapSlalomBase = Convert.ToDecimal( editHcapSlalomBase.Text );
                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in slalom rounds field" );
                curHcapSlalomBase = 0;
            }
            try {
                if ( HelperFunctions.isObjectEmpty( editHcapSlalomPct.Text ) ) {
                    editHcapSlalomPct.Text = "0";
                } else {
                    curHcapSlalomPct = Convert.ToDecimal( editHcapSlalomPct.Text );
                    if ( curHcapSlalomPct > 1 ) {
                        curHcapSlalomPct = curHcapSlalomPct / 100;
                        editHcapSlalomPct.Text = curHcapSlalomPct.ToString();
                    }

                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in slalom handicap percentage field" );
                curHcapSlalomPct = 0;
            }

            try {
                if ( HelperFunctions.isObjectEmpty( editHcapTrickBase.Text ) ) {
                    editHcapTrickBase.Text = "0";
                } else {
                    curHcapTrickBase = Convert.ToDecimal( editHcapTrickBase.Text );
                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in Trick rounds field" );
                curHcapTrickBase = 0;
            }
            try {
                if ( HelperFunctions.isObjectEmpty( editHcapTrickPct.Text ) ) {
                    editHcapTrickPct.Text = "0";
                } else {
                    curHcapTrickPct = Convert.ToDecimal( editHcapTrickPct.Text );
                    if ( curHcapTrickPct > 1 ) {
                        curHcapTrickPct = curHcapTrickPct / 100;
                        editHcapTrickPct.Text = curHcapTrickPct.ToString();
                    }

                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in Trick handicap percentage field" );
                curHcapTrickPct = 0;
            }

            try {
                if ( HelperFunctions.isObjectEmpty( editHcapJumpBase.Text ) ) {
                    editHcapJumpBase.Text = "0";
                } else {
                    curHcapJumpBase = Convert.ToDecimal( editHcapJumpBase.Text );
                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in Jump rounds field" );
                curHcapJumpBase = 0;
            }
            try {
                if ( HelperFunctions.isObjectEmpty( editHcapJumpPct.Text ) ) {
                    editHcapJumpPct.Text = "0";
                } else {
                    curHcapJumpPct = Convert.ToDecimal( editHcapJumpPct.Text );
                    if ( curHcapJumpPct > 1 ) {
                        curHcapJumpPct = curHcapJumpPct / 100;
                        editHcapJumpPct.Text = curHcapJumpPct.ToString();
                    }

                }
            } catch {
                curReturn = false;
                MessageBox.Show( "Invalid numeric in Jump handicap percentage field" );
                curHcapJumpPct = 0;
            }
            return curReturn;
        }

        private void setViewRowFromEdit() {
            DataGridViewRow curViewRow = myTourViewRow;

            curViewRow.Cells["SanctionId"].Value = editSanctionId.Text;
            curViewRow.Cells["TourName"].Value = editName.Text;
            curViewRow.Cells["TourClass"].Value = editClass.SelectedValue;
            curViewRow.Cells["SanctionEditCode"].Value = editSanctionEditCode.Text;
            curViewRow.Cells["TourFederation"].Value = (String)editFederation.SelectedValue;
            curViewRow.Cells["EventDates"].Value = editEventDates.Text;
            curViewRow.Cells["TourDataLoc"].Value = editTourDataLoc.Text;
            curViewRow.Cells["EventLocation"].Value = editEventLocation.Text;
            curViewRow.Cells["Rules"].Value = editRules.SelectedValue;
            curViewRow.Cells["SlalomRounds"].Value = editSlalomRounds.Text;
            curViewRow.Cells["TrickRounds"].Value = editTrickRounds.Text;
            curViewRow.Cells["JumpRounds"].Value = editJumpRounds.Text;
            curViewRow.Cells["HcapSlalomBase"].Value = editHcapSlalomBase.Text;
            curViewRow.Cells["HcapTrickBase"].Value = editHcapTrickBase.Text;
            curViewRow.Cells["HcapJumpBase"].Value = editHcapJumpBase.Text;
            curViewRow.Cells["HcapSlalomPct"].Value = editHcapSlalomPct.Text;
            curViewRow.Cells["HcapTrickPct"].Value = editHcapTrickPct.Text;
            curViewRow.Cells["HcapJumpPct"].Value = editHcapJumpPct.Text;

            myTourViewRow = curViewRow;
            setEntryForEdit( myTourViewRow );
        }

        private void navSort_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            sortDialogForm.SortCommand = mySortCommand;
            sortDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( sortDialogForm.DialogResult == DialogResult.OK ) {
                mySortCommand = sortDialogForm.SortCommand;
                winStatusMsg.Text = "Sorted by " + mySortCommand;
                loadTourView();
            }
        }

        private void navFilter_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            filterDialogForm.ShowDialog( this );

            // Determine if the OK button was clicked on the dialog box.
            if ( filterDialogForm.DialogResult == DialogResult.OK ) {
                myFilterCmd = filterDialogForm.FilterCommand;
                winStatusMsg.Text = "Filtered by " + myFilterCmd;
                loadTourView();
            }
        }

        private void navExportLw_Click( object sender, EventArgs e ) {
            navExport_Click( null, null );
            ExportLiveWeb.uploadExportFile( "Export", "Tour", editSanctionId.Text );
        }

        private void navExport_Click(object sender, EventArgs e) {
            // Display the form as a modal dialog box.
            exportDialogForm = new TourExportDialogForm();
            exportDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (exportDialogForm.DialogResult == DialogResult.OK) {
                String exportCommand = exportDialogForm.ExportCommand;
                if (exportCommand.ToLower().Equals("list")) {
                    winStatusMsg.Text = "Export list of tournaments";

                    ExportData myExportData = new ExportData();
                    myExportData.exportData("Tournament", "Select * from Tournament");
                
                } else if (exportCommand.ToLower().Equals("data")) {
                    winStatusMsg.Text = "Export all data for tournament " + editName.Text;

                    ExportData myExportData = new ExportData();
                    myExportData.exportTourData(editSanctionId.Text);
                
                } else if (exportCommand.ToLower().Equals("perf")) {
                    winStatusMsg.Text = "Export performance data for tournament " + TourName;
                    ExportPerfData myExportData = new ExportPerfData();
                    myExportData.exportTourPerfData(editSanctionId.Text);
                }
            }
        }

        private void navCopyItem_Click(object sender, EventArgs e) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            // Display the form as a modal dialog box.
            TourCopySelect CopyDialogForm = new TourCopySelect();
            CopyDialogForm.ShowDialog(this);

            // Determine if the OK button was clicked on the dialog box.
            if (CopyDialogForm.DialogResult == DialogResult.OK) {
                navRefresh_Click( null, null );
            }
        }

        private void navClean_Click( object sender, EventArgs e ) {
            String curMethodName = "TourList:cleanTourData: ";
            int rowsProc = 0;

            String dialogMsg = "You have asked to delete all data for the selected tournament"
                + "\n (" + editSanctionId.Text + " " + editName.Text + ")"
                + "\nand all its associcated scores"
                + "\n\nDo you want to continue?";
            DialogResult msgResp =
                MessageBox.Show(dialogMsg, "Change Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
            if (msgResp == DialogResult.OK) {
                try {
                    StringBuilder curMsg = new StringBuilder( "" );
                    Log.OpenFile();
                    Log.WriteFile( curMethodName + "begin: " );

                    try {
                        for (int idx = 0; idx < DataAccess.TourTableList.Length; idx++) {
                            if ( DataAccess.TourTableList[idx].Equals( "Tournament" ) ) continue;
                            String curSqlStmt = "Delete from " + DataAccess.TourTableList[idx] + " where SanctionId = '" + editSanctionId.Text + "'";
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt );
                            if (rowsProc > 0) {
                                curMsg.Append( String.Format( "{0} Records deleted from {1}{2}"
                                    , rowsProc, DataAccess.TourTableList[idx], Environment.NewLine ) );
                            }
                        }
                    } catch (Exception excp) {
                        Log.WriteFile( curMethodName + "Error attempting to clean tournament information: " + excp.Message );
                        MessageBox.Show( "Error attempting to clean tournament information \n" + excp.Message );
                    }

                    MessageBox.Show( curMsg.ToString() );
                    Log.WriteFile( curMethodName + curMsg.ToString() );

                } catch (Exception excp) {
                    Log.WriteFile( curMethodName + "Error attempting to clean tournament information: " + excp.Message );
                    MessageBox.Show( "Error attempting to clean tournament information \n" + excp.Message );
                }
            }
        }

        private void DataLocButton_Click( object sender, EventArgs e ) {
            String curPath = Properties.Settings.Default.ExportDirectory;
            using (FolderBrowserDialog curFolderDialog = new FolderBrowserDialog()) {
                curFolderDialog.ShowNewFolderButton = true;
                curFolderDialog.RootFolder = Environment.SpecialFolder.Desktop;
                curFolderDialog.SelectedPath = @curPath;
                if (FolderBrowserLauncher.ShowFolderBrowser( curFolderDialog, Form.ActiveForm ) == DialogResult.OK) {
                    curPath = curFolderDialog.SelectedPath;
                    if (Log.isDirectoryValid( curPath )) {
                        editTourDataLoc.Text = curPath;
                        isDataModified = true;
                    } else {
                        MessageBox.Show( "A valid local data directory must be selected."
                            + "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
                    }
                }
            }
        }

        private void dataGridView_RowHeaderMouseDoubleClick( object sender, DataGridViewCellMouseEventArgs e ) {
            //Update data if changes are detected
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                setCurrentTournament();
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e) {
            TourName.Width = dataGridView.Width - 102 ;
        }

        private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            if ( isDataModified ) {
                try {
                    navSaveItem_Click( null, null );
                } catch ( Exception excp ) {
                    MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                }
            }
            if ( !( isDataModified ) ) {
                setCurrentTournament();
            }
        }

        private void setCurrentTournament() {
            MessageBox.Show( "Preparing tournament for scoring.  This will take a moment" );

            String curDataLoc = dataGridView.Rows[myTourViewIdx].Cells["TourDataLoc"].Value.ToString();
            if ( HelperFunctions.isObjectEmpty( curDataLoc )) {
                MessageBox.Show( "A data directory must be supplied before the tournament can be selected"
                    + "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
				return;
            }

			if ( !(Log.isDirectoryValid( curDataLoc )) ) {
				MessageBox.Show( "A valid local data directory must be selected before this tournament can be selected for scoring."
					+ "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
				return;
			}

			if ( WscHandler.isConnectActive ) WscHandler.sendExit();

			Properties.Settings.Default.AppSanctionNum = dataGridView.Rows[myTourViewIdx].Cells["SanctionId"].Value.ToString();
			Properties.Settings.Default.ExportDirectory = dataGridView.Rows[myTourViewIdx].Cells["TourDataLoc"].Value.ToString();
			if ( !( Log.OpenFile() ) ) {
				MessageBox.Show( "Issue encountered opening log file.  Unable to activate tournament" );
				return;
			}

			this.MdiParent.Text = Properties.Settings.Default.AppTitle
				+ " - " + Properties.Settings.Default.BuildVersion
				+ " - " + Properties.Settings.Default.AppSanctionNum
				+ " - " + dataGridView.Rows[myTourViewIdx].Cells["TourName"].Value.ToString();
			Properties.Settings.Default.Mdi_Title = this.MdiParent.Text;
			String[] curLog = { "Tournament activated: " + this.MdiParent.Text };
			Log.WriteFile( curLog );
			bool curResults = myTourProperties.loadProperties( (String)editRules.SelectedValue, (String)editClass.SelectedValue );
			if ( curResults == false ) {
				ReportPropButton_Click( null, null );
			}
            Properties.Settings.Default.Save();

            MessageBox.Show( "Setting current application tournament to "
				+ Properties.Settings.Default.AppSanctionNum
				+ "\n\n Tournament data location: "
				+ Properties.Settings.Default.ExportDirectory
				);

			checkDivOrderStatus();
		}

		private void checkDivOrderStatus() {
            Int16 curSlalomRounds = 0;
            try {
                curSlalomRounds = Convert.ToInt16( editSlalomRounds.Text );
            } catch {
                curSlalomRounds = 0;
            }
            Int16 curTrickRounds = 0;
            try {
                curTrickRounds = Convert.ToInt16( editTrickRounds.Text );
            } catch {
                curTrickRounds = 0;
            }
            Int16 curJumpRounds = 0;
            try {
                curJumpRounds = Convert.ToInt16( editJumpRounds.Text );
            } catch {
                curJumpRounds = 0;
            }

            DataTable curDivDataTable = null;
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            if (curSlalomRounds > 0) {
                curSqlStmt = new StringBuilder( "Select Distinct SanctionId From DivOrder Where SanctionId = '" + Properties.Settings.Default.AppSanctionNum + "' AND Event = 'Slalom' " );
                curDivDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if (curDivDataTable.Rows.Count == 0) {
                    curMsg = "It is recommended, but not required, to setup Division order data for the slalom event";
                }
            }
            if (curTrickRounds > 0) {
                curSqlStmt = new StringBuilder( "Select Distinct SanctionId From DivOrder Where SanctionId = '" + Properties.Settings.Default.AppSanctionNum + "' AND Event = 'Trick'" );
                curDivDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if (curDivDataTable.Rows.Count == 0) {
                    curMsg += "\nIt is recommended, but not required, to setup Division order data for the slalom event";
                }
            }
            if (curJumpRounds > 0) {
                curSqlStmt = new StringBuilder( "Select Distinct SanctionId From DivOrder Where SanctionId = '" + Properties.Settings.Default.AppSanctionNum + "' AND Event = 'Jump'" );
                curDivDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if (curDivDataTable.Rows.Count == 0) {
                    curMsg += "\nIt is recommended, but not required, to setup Division order data for the slalom event";
                }
            }

            //Disable messaging for the time being
            /*
            if (curMsg.Length > 0) {
                MessageBox.Show( curMsg );
                TourDivOrder curForm = new TourDivOrder();
                curForm.MdiParent = this.MdiParent;
                curForm.Show();
            }
             */

        }

        private void editTextValidate(object sender, EventArgs e) {
            if (!(((TextBox)sender).Text.Equals(myOrigCellValue))) {
                isDataModified = true;
                if ((((TextBox)sender).Name.Equals( "editSlalomRounds" ))) {
                    isDataModifiedSlalomRounds = true;
                }
                if ((((TextBox)sender).Name.Equals( "editTrickRounds" ))) {
                    isDataModifiedTrickRounds = true;
                }
                if ((((TextBox)sender).Name.Equals( "editJumpRounds" ))) {
                    isDataModifiedJumpRounds = true;
                }
            }
        }

        private void editTourDataLoc_Validating( object sender, CancelEventArgs e ) {
            if ( ( (TextBox)sender ).Text.Equals( myOrigCellValue ) ) {
                e.Cancel = false;
            } else {
                String curPath = ( (TextBox)sender ).Text;
                if ( Log.isDirectoryValid( curPath ) ) {
                    e.Cancel = false;
                } else {
                    MessageBox.Show( "A valid local data directory must be selected."
                        + "\nPlease use the Browse button to select a folder on a local drive or attached external drive" );
                    e.Cancel = true;
                }
            }
        }

        private void editTextOrigValue( object sender, EventArgs e ) {
            myOrigCellValue = ((TextBox)sender).Text;
        }

        private void editSanctionIdValidate(object sender, EventArgs e) {
            if (!(((TextBox)sender).Text.Equals(myOrigCellValue))) {
                editSanctionId.Text = editSanctionId.Text.ToUpper();
                myOrigSanctionId = myOrigCellValue;
                isDataModified = true;
                isDataModifiedSanctionId = true;
            }
        }

        private void dropdownOrigValue( object sender, EventArgs e ) {
            myOrigCellValue = ( (ComboBox)sender ).SelectedValue.ToString();
        }

        private void dropdown_SelectedValueChanged( object sender, EventArgs e ) {
            isDataModified = true;
            try {
                if ( ( (ComboBox)sender ).Name.Equals( "editClass" ) ) {
                    String curValue = ( (ComboBox)sender ).SelectedValue.ToString();
                    if ( !(curValue.Equals( myOrigCellValue ) ) ) {
                        myOrigEventClass = myOrigCellValue;
                        isDataModifiedClass = true;
                    }
                }
            } catch {
            }
        }

        private void EventTechButton_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Admin.TourEventTech curForm = new WaterskiScoringSystem.Admin.TourEventTech();

            // Check for open instance of selected form
            for (int idx = 0; idx < this.MdiParent.MdiChildren.Length; idx++) {
                if (((Form)this.MdiParent.MdiChildren[idx]).Name == curForm.Name) {
                    ((Form)this.MdiParent.MdiChildren[idx]).Activate();
                    return;
                }
            }

            curForm.MdiParent = this.MdiParent;
            curForm.Show();
            curForm.TourEventTech_Show(editSanctionId.Text);
        }

        private void RuleExceptButton_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Admin.TourRuleExcept curForm = new WaterskiScoringSystem.Admin.TourRuleExcept();

            // Check for open instance of selected form
            for (int idx = 0; idx < this.MdiParent.MdiChildren.Length; idx++) {
                if (((Form)this.MdiParent.MdiChildren[idx]).Name == curForm.Name) {
                    ((Form)this.MdiParent.MdiChildren[idx]).Activate();
                    return;
                }
            }

            curForm.MdiParent = this.MdiParent;
            curForm.Show();
            curForm.TourRuleExcept_Show( editSanctionId.Text );
        }

        private void ReportPropButton_Click(object sender, EventArgs e) {
            WaterskiScoringSystem.Admin.TourReportProps curForm = new WaterskiScoringSystem.Admin.TourReportProps();

            // Check for open instance of selected form
            for (int idx = 0; idx < this.MdiParent.MdiChildren.Length; idx++) {
                if (( (Form)this.MdiParent.MdiChildren[idx] ).Name == curForm.Name) {
                    ( (Form)this.MdiParent.MdiChildren[idx] ).Activate();
                    return;
                }
            }

            curForm.MdiParent = this.MdiParent;
            curForm.Show();
            curForm.TourChiefOfficialContact_Show( editSanctionId.Text, (String)editClass.SelectedValue, (String)editRules.SelectedValue, editName.Text );
        }

        private void OfficialContactButton_Click(object sender, EventArgs e) {
            WaterskiScoringSystem.Admin.TourChiefOfficialContact curForm = new WaterskiScoringSystem.Admin.TourChiefOfficialContact();

            // Check for open instance of selected form
            for (int idx = 0; idx < this.MdiParent.MdiChildren.Length; idx++) {
                if (((Form)this.MdiParent.MdiChildren[idx]).Name == curForm.Name) {
                    ((Form)this.MdiParent.MdiChildren[idx]).Activate();
                    return;
                }
            }

            curForm.MdiParent = this.MdiParent;
            curForm.Show();
            curForm.TourChiefOfficialContact_Show( editSanctionId.Text );
        }

        private void SafetyDirReportButton_Click( object sender, EventArgs e ) {
            WaterskiScoringSystem.Tournament.SafetyCheckList curForm = new WaterskiScoringSystem.Tournament.SafetyCheckList();

            // Check for open instance of selected form
            for (int idx = 0; idx < this.MdiParent.MdiChildren.Length; idx++) {
                if (((Form)this.MdiParent.MdiChildren[idx]).Name == curForm.Name) {
                    ((Form)this.MdiParent.MdiChildren[idx]).Activate();
                    return;
                }
            }

            curForm.MdiParent = this.MdiParent;
            curForm.Show();
            curForm.ShowTour( editSanctionId.Text );
        }

        private void navDeleteItem_Click( object sender, EventArgs e ) {
            DataTable curDataTable = new DataTable();
            String curTableName = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            String curSanctionId = dataGridView.Rows[myTourViewIdx].Cells["SanctionId"].Value.ToString();
            String myTourName = dataGridView.Rows[myTourViewIdx].Cells["TourName"].Value.ToString();
            String dialogMsg = "You have asked to delete tournament " + myTourName + " and all its associcated scores" + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show(dialogMsg, "Change Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);
            if (msgResp == DialogResult.OK) {
                try {
                    dataGridView.Rows.RemoveAt( myTourViewIdx );
                    isDataModified = false;
                    isDataModifiedSanctionId = false;
                    isDataModifiedClass = false;
                    isDataModifiedSlalomRounds = false;
                    isDataModifiedTrickRounds = false;
                    isDataModifiedJumpRounds = false;

                    curSqlStmt.Append("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SanctionId' ORDER BY TABLE_NAME");
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
					if ( curDataTable == null || curDataTable.Rows.Count == 0 ) return;

					foreach ( DataRow curRow in curDataTable.Rows ) {
						curSqlStmt = new StringBuilder( "" );
						curTableName = (String)curRow["TABLE_NAME"];
						if ( !( curTableName.Equals( "tournament" ) ) ) {
							try {
								curSqlStmt.Append( "Delete " + curTableName + " where SanctionId = '" + curSanctionId + "'" );
								int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
								if ( rowsProc < 0 ) {
									MessageBox.Show( "Error attempting to delete tournament records from " + curTableName );
								}
							
							} catch ( Exception ex ) {
								String ExcpMsg = ex.Message;
								if ( curSqlStmt.ToString().Length > 0 ) {
									ExcpMsg += "\n" + curSqlStmt.ToString();
								}
								MessageBox.Show( "Error attempting to delete tournament scores " + curTableName + "\n\nError: " + ExcpMsg );
								return;
							}
						}
					}

					if ( myTourViewIdx == 0 ) return;
					myTourViewIdx -= 1;
					dataGridView.CurrentCell = dataGridView.Rows[myTourViewIdx].Cells["SanctionId"];
					myTourViewRow = dataGridView.Rows[myTourViewIdx];
					setEntryForEdit( myTourViewRow );

				} catch (Exception excp) {
                    MessageBox.Show("Error attempting to delete tournament and all scores \n" + excp.Message);
                }
            
			} else if (msgResp == DialogResult.No) {
                isDataModified = false;
                isDataModifiedSanctionId = false;
                isDataModifiedClass = false;
                isDataModifiedSlalomRounds = false;
                isDataModifiedTrickRounds = false;
                isDataModifiedJumpRounds = false;
            }
        }

        private void updateSanctionId() {
            DataTable curDataTable = new DataTable();
            String curTableName = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                if ( myTourViewRow != null ) {
                    String curSanctionId = myTourViewRow.Cells["SanctionId"].Value.ToString();
                    curSqlStmt.Append( "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME = 'SanctionId' ORDER BY TABLE_NAME" );
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if (curDataTable != null) {
                        foreach (DataRow curRow in curDataTable.Rows) {
                            curTableName = (String)curRow["TABLE_NAME"];
                            if (!( curTableName.Equals( "tournament" ) )) {
                                try {
                                    curSqlStmt = new StringBuilder( "" );
                                    curSqlStmt.Append( "Update " + curTableName + " " );
                                    curSqlStmt.Append( "Set SanctionId = '" + curSanctionId + "' " );
                                    curSqlStmt.Append(" Where SanctionId = '" + myOrigSanctionId + "'");
                                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                    if (rowsProc < 0) {
                                        MessageBox.Show( "Error attempting to change tournament sanction number on table " + curTableName );
                                    }
                                } catch (Exception ex) {
                                    String ExcpMsg = ex.Message;
                                    if (curSqlStmt.Length > 0) {
                                        ExcpMsg += "\n" + curSqlStmt.ToString();
                                    }
                                    MessageBox.Show( "Error attempting to change tournament sanction number on table " + curTableName + "\n\nError: " + ExcpMsg );
                                    return;
                                }
                            }
                        }
                    }
                }
            } catch (Exception excp) {
                MessageBox.Show( "Error attempting to change tournament sanction number on all table " + "\n\nError: " + excp.Message );
            }
        }

        private void updateEventClass() {
            DataTable curDataTable = new DataTable();

            try {
                if ( myTourViewRow != null ) {
                    String curSanctionId = myTourViewRow.Cells["SanctionId"].Value.ToString();
                    String curTourClass = myTourViewRow.Cells["TourClass"].Value.ToString();
                    StringBuilder curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "SELECT CodeValue as EventClass " );
                    curSqlStmt.Append( "FROM CodeValueList Where ListName = 'ClassToEvent' AND ListCode = '" + curTourClass + "' " );
                    curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                    if (curDataTable.Rows.Count > 0) {
                        String curTourEventClass = (String)curDataTable.Rows[0]["EventClass"];
                    
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "SELECT CodeValue as EventClass " );
                        curSqlStmt.Append( "FROM CodeValueList Where ListName = 'ClassToEvent' AND ListCode = '" + myOrigEventClass + "' " );
                        curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                        if (curDataTable.Rows.Count > 0) {
                            String curTourEventClassOrig = (String)curDataTable.Rows[0]["EventClass"];

                            ListSkierClass curSkierClassList = new ListSkierClass();
                            curSkierClassList.ListSkierClassLoad();

                            HelperFunctions.updateEventRegSkierClass( curSanctionId
                                , HelperFunctions.getViewRowColValue( myTourViewRow, "Rules", "AWSA" )
                                , HelperFunctions.getViewRowColValue( myTourViewRow, "SanctionEditCode", "" )
                                , HelperFunctions.getViewRowColValue( myTourViewRow, "EventDates", "" )
                                , "All", "All", curTourEventClassOrig, curTourEventClass, curSkierClassList );

                        }
                    }
                }
            } catch ( Exception excp ) {
                MessageBox.Show( "Error attempting to change tournament class \n" + excp.Message );
            }
        }

        private void navAddNewItem_Click(object sender, EventArgs e) {
            myTourViewIdx = dataGridView.Rows.Add();
            myTourViewRow = dataGridView.Rows[myTourViewIdx];

			Dictionary<string, object> curSanctionEntry = getSanctionFromUSAWS();
			if ( curSanctionEntry == null ) {
				myTourViewRow.Cells["SanctionId"].Value = "";
				myTourViewRow.Cells["SanctionEditCode"].Value = "";
				myTourViewRow.Cells["TourName"].Value = "";
				myTourViewRow.Cells["TourClass"].Value = "C";
				myTourViewRow.Cells["TourFederation"].Value = "usa";
				String curDate = DateTime.Now.ToString();
				myTourViewRow.Cells["EventDates"].Value = curDate.Substring( 0, curDate.IndexOf( ' ' ) );
				myTourViewRow.Cells["TourDataLoc"].Value = "";
				myTourViewRow.Cells["EventLocation"].Value = "";
				myTourViewRow.Cells["Rules"].Value = "awsa";
				myTourViewRow.Cells["SlalomRounds"].Value = "0";
				myTourViewRow.Cells["TrickRounds"].Value = "0";
				myTourViewRow.Cells["JumpRounds"].Value = "0";
				myTourViewRow.Cells["HcapSlalomBase"].Value = "0";
				myTourViewRow.Cells["HcapTrickBase"].Value = "0";
				myTourViewRow.Cells["HcapJumpBase"].Value = "0";
				myTourViewRow.Cells["HcapSlalomPct"].Value = "0";
				myTourViewRow.Cells["HcapTrickPct"].Value = "0";
				myTourViewRow.Cells["HcapJumpPct"].Value = "0";

			} else {
                String curSanctionId = (String)curSanctionEntry["TournAppID"];
                myTourViewRow.Cells["SanctionId"].Value = (String) curSanctionEntry["TournAppID"];
				myTourViewRow.Cells["TourName"].Value = (String) curSanctionEntry["TName"];
				myTourViewRow.Cells["TourClass"].Value = ( (String) curSanctionEntry["TSanction"] ).Substring( 6, 1 );
				myTourViewRow.Cells["SanctionEditCode"].Value = ((int) curSanctionEntry["EditCode"]).ToString();
				myTourViewRow.Cells["TourFederation"].Value = "usa";
				myTourViewRow.Cells["EventDates"].Value = (String) curSanctionEntry["TDateE"] ;
				myTourViewRow.Cells["TourDataLoc"].Value = "";
				myTourViewRow.Cells["EventLocation"].Value = String.Format( "{0} ({1}), {2}, {3}"
                    , (String)curSanctionEntry["TSite"]
                    , (String)curSanctionEntry["TSiteID"]
                    , (String)curSanctionEntry["TCity"]
                    , (String)curSanctionEntry["TState"] );
                if ( curSanctionId.Substring( 2, 1 ).ToUpper().Equals( "U" ) ) {
                    myTourViewRow.Cells["Rules"].Value = "ncwsa";
                } else {
                    myTourViewRow.Cells["Rules"].Value = "awsa";
                }
                if ( (bool) curSanctionEntry["TEventSlalom"] ) {
					myTourViewRow.Cells["SlalomRounds"].Value = "1";
				} else { 
					myTourViewRow.Cells["SlalomRounds"].Value = "0";
				}
				if ( (bool) curSanctionEntry["TEventTrick"] ) {
					myTourViewRow.Cells["TrickRounds"].Value = "1";
				} else {
					myTourViewRow.Cells["TrickRounds"].Value = "0";
				}
				if ( (bool) curSanctionEntry["TEventJump"] ) {
					myTourViewRow.Cells["JumpRounds"].Value = "1";
				} else {
					myTourViewRow.Cells["JumpRounds"].Value = "0";
				}
				myTourViewRow.Cells["HcapSlalomBase"].Value = "0";
				myTourViewRow.Cells["HcapTrickBase"].Value = "0";
				myTourViewRow.Cells["HcapJumpBase"].Value = "0";
				myTourViewRow.Cells["HcapSlalomPct"].Value = "0";
				myTourViewRow.Cells["HcapTrickPct"].Value = "0";
				myTourViewRow.Cells["HcapJumpPct"].Value = "0";
			}

			setEntryForEdit( myTourViewRow );
            dataGridView.CurrentCell = dataGridView.Rows[myTourViewIdx].Cells["TourName"];

            editSanctionId.Focus();
        }

		private Dictionary<string, object> getSanctionFromUSAWS() {
			SanctionSetupDialog curDialog = new SanctionSetupDialog();
			if ( curDialog.ShowDialog() != DialogResult.OK ) return null;

			String curSanctionNum = curDialog.SanctionId;
			String curSanctionEditCode = curDialog.EditCode;
			editSanctionId.Text = curDialog.SanctionId;
			editSanctionEditCode.Text = curDialog.EditCode;

			/* -----------------------------------------------------------------------
            * Configure URL to retrieve all skiers pre-registered for the active tournament
			* This will include all appointed officials
            ----------------------------------------------------------------------- */
			String curContentType = "application/json; charset=UTF-8";
			String curOfficialExportListUrl = "https://www.usawaterski.org/admin/GetSanctionExportJson.asp";
			String curReqstUrl = curOfficialExportListUrl;

			NameValueCollection curHeaderParams = new NameValueCollection();
			List<object> curResponseDataList = null;

			Cursor.Current = Cursors.WaitCursor;
			curResponseDataList = SendMessageHttp.getMessageResponseJsonArray( curReqstUrl, curHeaderParams, curContentType, curSanctionNum, curSanctionEditCode, false );
			if ( curResponseDataList != null && curResponseDataList.Count > 0 ) {
				StringBuilder curMsg = new StringBuilder( "" );
				return (Dictionary<string, object>) curResponseDataList.ElementAt( 0 );

			} else {
				MessageBox.Show( "Sanction not found or hasn't been approved Sanction: " + curSanctionNum );
				return null;
			}
		}

		private void printButton_Click(object sender, EventArgs e) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            myPrintDoc = new PrintDocument();
            curPrintDialog.Document = myPrintDoc;
            CaptureScreen();

            curPrintDialog.AllowCurrentPage = true;
            curPrintDialog.AllowPrintToFile = true;
            curPrintDialog.AllowSelection = false;
            curPrintDialog.AllowSomePages = false;
            curPrintDialog.PrintToFile = true;
            curPrintDialog.ShowHelp = false;
            curPrintDialog.ShowNetwork = false;
            curPrintDialog.UseEXDialog = true;
            curPrintDialog.PrinterSettings.DefaultPageSettings.Landscape = true;

            if (curPrintDialog.ShowDialog() == DialogResult.OK) {
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.PrintPage += new PrintPageEventHandler(printDoc_PrintScreen);
                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);

                curPreviewDialog.Document = myPrintDoc;
                curPreviewDialog.ShowDialog();
            }
        }

        private void printDoc_PrintScreen(object sender, PrintPageEventArgs e) {
            int curXPos = 0;
            int curYPos = 25;
            Font fontPrintTitle = new Font("Arial Narrow", 14, FontStyle.Bold);
            Font fontPrintFooter = new Font("Times New Roman", 10);
            String printTitle = Properties.Settings.Default.Mdi_Title + " - " + this.Text;

            //display a title
            curXPos = 100;
            e.Graphics.DrawString(printTitle, fontPrintTitle, Brushes.Black, curXPos, curYPos);

            //display form
            curXPos = 50;
            curYPos += 50;
            e.Graphics.DrawImage(memoryImage, curXPos, curYPos);

            curXPos = 50;
            curYPos = curYPos + memoryImage.Height + 25;
            string DateString = DateTime.Now.ToString();
            e.Graphics.DrawString(DateString, fontPrintFooter, Brushes.Black, curXPos, curYPos);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);
        private Bitmap memoryImage;
        private void CaptureScreen() {
            Graphics mygraphics = this.CreateGraphics();
            Size s = this.Size;
            memoryImage = new Bitmap(s.Width, s.Height, mygraphics);
            Graphics memoryGraphics = Graphics.FromImage(memoryImage);
            IntPtr dc1 = mygraphics.GetHdc();
            IntPtr dc2 = memoryGraphics.GetHdc();
            BitBlt(dc2, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height, dc1, 0, 0, 13369376);
            mygraphics.ReleaseHdc(dc1);
            memoryGraphics.ReleaseHdc(dc2);
        }

        private DataTable getTourData() {
            return getTourData( null );
        }
        private DataTable getTourData( String inSanctionId ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append("SELECT SanctionId, Name, Class, Federation, TourDataLoc, SanctionEditCode, LastUpdateDate, ");
            curSqlStmt.Append( "    SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation, " );
            curSqlStmt.Append( "    HcapSlalomBase, HcapTrickBase, HcapJumpBase, HcapSlalomPct, HcapTrickPct, HcapJumpPct " );
            curSqlStmt.Append( "FROM Tournament " );
            if ( inSanctionId != null ) {
                curSqlStmt.Append( "WHERE SanctionId = '" + inSanctionId + "' " );
            }
            curSqlStmt.Append( "ORDER BY SanctionId " );
            return DataAccess.getDataTable( curSqlStmt.ToString() );
        }

        private Decimal getEventRecordValue( String inSanctionId, String inEvent ) {
            Decimal curRatingValue = 0;
            Byte curSkiYear = Convert.ToByte( inSanctionId.Substring( 0, 2 ) );
            DataRow curRow = null;

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT Event, AgeGroup, SkiYear, RatingRec " );
            curSqlStmt.Append( "FROM NopsData " );
            curSqlStmt.Append( "WHERE Event = '" + inEvent + "' AND AgeGroup = 'M1' AND SkiYear = " + curSkiYear.ToString() + " " );
            DataTable curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count > 0 ) {
                curRow = curDataTable.Rows[0];
            } else {
                curSqlStmt = new StringBuilder( "" ); 
                curSqlStmt.Append( "SELECT Event, AgeGroup, SkiYear, RatingRec " );
                curSqlStmt.Append( "FROM NopsData " );
                curSqlStmt.Append( "WHERE Event = '" + inEvent + "' AND AgeGroup = 'M1'" );
                curSqlStmt.Append( "  AND SkiYear IN (SELECT MAX(SkiYear) AS Expr1 FROM NopsData) " );
                curDataTable = DataAccess.getDataTable( curSqlStmt.ToString() );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = curDataTable.Rows[0];
                }
            }

            if ( curRow != null ) {
                curRatingValue = (Decimal)curRow["RatingRec"];
            }

            return curRatingValue;
        }

	}
}
