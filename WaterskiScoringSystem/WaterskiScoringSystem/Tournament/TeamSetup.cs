﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class TeamSetup : Form {
        #region instance variables
        private bool isDataModified = false;
        private bool myShowTeam = false;
        private bool myViewLoad = false;
        private int myTeamRowIdx;
        private int mySlalomRowIdx;
        private int myTrickRowIdx;
        private int myJumpRowIdx;

		private int myTeamImportIdxTeamCode = 0
			, myTeamImportIdxTeamName = 1
			, myTeamImportIdxEvent = 2
			, myTeamImportIdxDiv = 3
			, myTeamImportIdxSkierName = 4
			, myTeamImportIdxMemberId = 5;

		private String myWindowTitle;
        private String mySanctionNum;
        private String myRules;
        private String myOrigItemValue = "";
        private String mySortCmd = "";
        private String myFilterCmd = "";
        private String myTourClass;
		private StringBuilder myImportMsg;

        private DataRow myTourRow;
        private DataTable myTeamDataTable;
        private DataTable mySlalomDataTable;
        private DataTable myTrickDataTable;
        private DataTable myJumpDataTable;

        private SortDialogForm sortDialogForm;
        private FilterDialogForm filterDialogForm;
        private ListSkierClass myTourClassList;
        private DataGridViewPrinter myPrintDataGrid;
        private PrintDocument myPrintDoc;
        #endregion

        public TeamSetup() {
            InitializeComponent();
        }

        private void TeamSetup_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TeamSetup_Width > 0 ) {
                this.Width = Properties.Settings.Default.TeamSetup_Width;
            }
            if ( Properties.Settings.Default.TeamSetup_Height > 0 ) {
                this.Height = Properties.Settings.Default.TeamSetup_Height;
            }
            if ( Properties.Settings.Default.TeamSetup_Location.X > 0
                && Properties.Settings.Default.TeamSetup_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TeamSetup_Location;
            }

            // Retrieve data from database
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
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myRules = (String)myTourRow["Rules"];
                        if ( myRules.ToLower().Equals( "ncwsa" ) ) {
                            //StateTeamsButton.Visible = false;
                            StateTeamsButton.Enabled = false;
                            //RegionTeamsButton.Visible = false;
                            RegionTeamsButton.Enabled = false;
                            //FebTeamsButton.Visible = false;
                            FebTeamsButton.Enabled = false;
                        } else {
                            //RandomOrderButton.Visible = false;
                            //SkierOrderButton.Visible = false;
                            //SkierOrderButton.Enabled = false;
                        }

                        navRefresh_Click( null, null );
                    }
                }
            }
        }

        private void TeamSetup_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TeamSetup_Width = this.Size.Width;
                Properties.Settings.Default.TeamSetup_Height = this.Size.Height;
                Properties.Settings.Default.TeamSetup_Location = this.Location;
            }
        }

        private void TeamSetup_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                checkModifyPrompt();
                if ( isDataModified ) {
                    e.Cancel = true;
                }
            }
        }

        private void navSave_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:navSave_Click";
            String curMsg = "", curAgeGroup = "", curEventGroup = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            int rowsProc = 0;

            try {
                if ( TeamDataGridView.Rows.Count > 0 ) {
                    DataGridViewRow curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
                    String curUpdateStatus = (String)curViewRow.Cells["TeamUpdated"].Value;
                    if ( curUpdateStatus.ToUpper().Equals( "Y" ) ) {
                        try {
                            String curTeamName, curTeamCode, curContactName, curContactInfo, curNotes;
                            Int16 curRunOrder, curSlalomOrder, curTrickOrder, curJumpOrder;
                            Int64 curTeamPK, curOrderPK;

                            bool curCmdResult = Int64.TryParse( HelperFunctions.getViewRowColValue( curViewRow, "TeamPK", "0" ), out curTeamPK );
                            if ( curCmdResult == false ) curTeamPK = -1;

                            curCmdResult = Int64.TryParse( HelperFunctions.getViewRowColValue( curViewRow, "OrderPK", "0" ), out curOrderPK );
                            if ( curCmdResult == false ) curOrderPK = -1;

                            curTeamCode = HelperFunctions.getViewRowColValue( curViewRow, "TeamCode", "" );
                            if ( HelperFunctions.isObjectEmpty( curTeamCode ) ) return;

                            curTeamName = HelperFunctions.getViewRowColValue( curViewRow, "TeamName", "" );
                            curAgeGroup = HelperFunctions.getViewRowColValue( curViewRow, "AgeGroup", "" );
                            curEventGroup = HelperFunctions.getViewRowColValue( curViewRow, "EventGroup", "" );

                            curCmdResult = Int16.TryParse( HelperFunctions.getViewRowColValue( curViewRow, "TeamSlalomOrder", "1" ), out curSlalomOrder );
                            if ( curCmdResult == false ) curSlalomOrder = 1;
                            curCmdResult = Int16.TryParse( HelperFunctions.getViewRowColValue( curViewRow, "TeamTrickOrder", "1" ), out curTrickOrder );
                            if ( curCmdResult == false ) curTrickOrder = 1;
                            curCmdResult = Int16.TryParse( HelperFunctions.getViewRowColValue( curViewRow, "TeamJumpOrder", "1" ), out curJumpOrder );
                            if ( curCmdResult == false ) curJumpOrder = 1;

                            curContactName = HelperFunctions.getViewRowColValue( curViewRow, "TeamContactName", "" );
                            curContactInfo = encodeSpecialChar( HelperFunctions.getViewRowColValue( curViewRow, "TeamContactInfo", "" ));
                            curNotes = encodeSpecialChar( HelperFunctions.getViewRowColValue( curViewRow, "TeamNotes", "" ) );

                            if ( curTeamPK < 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Select PK From TeamList " );
                                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
                                DataTable curDataTable = getData( curSqlStmt.ToString() );
                                if ( curDataTable.Rows.Count > 0 ) {
                                    curTeamPK = (Int64)curDataTable.Rows[0]["PK"];
                                    curViewRow.Cells["TeamPK"].Value = curTeamPK.ToString();
                                    curViewRow.Cells["TeamCode"].ReadOnly = true;
                                }
                            }

                            curSqlStmt = new StringBuilder( "" );
                            if ( curTeamPK > 0 ) {
                                curSqlStmt.Append( "Update TeamList Set " );
                                curSqlStmt.Append( "TeamCode = '" + curTeamCode + "'" );
                                curSqlStmt.Append( ", Name = '" + curTeamName + "'" );
                                curSqlStmt.Append( ", ContactName = '" + curContactName + "'" );
                                curSqlStmt.Append( ", ContactInfo = '" + curContactInfo + "'" );
                                curSqlStmt.Append( ", LastUpdateDate = getdate()");
                                curSqlStmt.Append( ", Notes = '" + curNotes + "'" );
                                curSqlStmt.Append( "Where PK = " + curTeamPK );
                            } else {
                                curSqlStmt.Append( "Insert TeamList (" );
                                curSqlStmt.Append( "SanctionId, TeamCode, Name, ContactName, ContactInfo" );
                                curSqlStmt.Append( ", LastUpdateDate, Notes" );
                                curSqlStmt.Append( ") Values (" );
                                curSqlStmt.Append( "'" + mySanctionNum + "'");
                                curSqlStmt.Append( ", '" + curTeamCode + "'");
                                curSqlStmt.Append( ", '" + curTeamName + "'");
                                curSqlStmt.Append( ", '" + curContactName + "'");
                                curSqlStmt.Append( ", '" + curContactInfo + "'");
                                curSqlStmt.Append( ", getdate()" );
                                curSqlStmt.Append( ", '" + curNotes + "' " );
                                curSqlStmt.Append( ")" );
                            }
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            if ( rowsProc > 0 ) {
                                curViewRow.Cells["TeamUpdated"].Value = "N";
                            }

                            if ( curTeamPK < 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Select PK From TeamList " );
                                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
                                DataTable curDataTable = getData( curSqlStmt.ToString() );
                                if ( curDataTable.Rows.Count > 0 ) {
                                    curTeamPK = (Int64)curDataTable.Rows[0]["PK"];
                                    curViewRow.Cells["TeamPK"].Value = curTeamPK.ToString();
                                    curViewRow.Cells["TeamCode"].ReadOnly = true;
                                }
                            }

                            curSqlStmt = new StringBuilder( "" );
                            if ( curOrderPK > 0 ) {
                                curSqlStmt.Append( "Update TeamOrder Set " );
                                curSqlStmt.Append( "TeamCode = '" + curTeamCode + "'" );
                                curSqlStmt.Append( ", AgeGroup = '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", EventGroup = '" + curEventGroup + "'" );
                                curSqlStmt.Append( ", SlalomRunOrder = " + curSlalomOrder );
                                curSqlStmt.Append( ", TrickRunOrder = " + curTrickOrder );
                                curSqlStmt.Append( ", JumpRunOrder = " + curJumpOrder );
                                curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                                curSqlStmt.Append( "Where PK = " + curOrderPK );
                            } else {
                                curSqlStmt.Append( "Insert TeamOrder (" );
                                curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, EventGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, LastUpdateDate" );
                                curSqlStmt.Append( ") Values (" );
                                curSqlStmt.Append( "'" + mySanctionNum + "'" );
                                curSqlStmt.Append( ", '" + curTeamCode + "'" );
                                curSqlStmt.Append( ", '" + curAgeGroup + "'" );
                                curSqlStmt.Append( ", '" + curEventGroup + "'" );
                                curSqlStmt.Append( ", " + curSlalomOrder );
                                curSqlStmt.Append( ", " + curTrickOrder );
                                curSqlStmt.Append( ", " + curJumpOrder );
                                curSqlStmt.Append( ", getdate()" );
                                curSqlStmt.Append( ")" );
                            }
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            if ( rowsProc > 0 ) {
                                curViewRow.Cells["TeamUpdated"].Value = "N";
                            }

                            if ( curOrderPK < 0 ) {
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Select PK From TeamOrder " );
                                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                                curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
                                curSqlStmt.Append( "  And (AgeGroup = '" + curAgeGroup + "' OR EventGroup = '" + curEventGroup + "') " );
                                DataTable curDataTable = getData( curSqlStmt.ToString() );
                                if ( curDataTable.Rows.Count > 0 ) {
                                    curOrderPK = (Int64)curDataTable.Rows[0]["PK"];
                                    curViewRow.Cells["OrderPK"].Value = curOrderPK.ToString();
                                }
                            }
                            
                            isDataModified = false;
                        } catch ( Exception excp ) {
                            curMsg = "Error attempting to update team information \n" + excp.Message;
                            MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + curMsg );
                        }
                    }
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to save changes \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void checkModifyPrompt() {
            if ( isDataModified ) {
                String dialogMsg = "Changes have been made to currently displayed data!"
                    + "\n Do you want to save the changes before closing the window?";
                DialogResult msgResp =
                    MessageBox.Show( dialogMsg, "Change Warning",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button1 );
                if ( msgResp == DialogResult.Yes ) {
                    try {
                        navSave_Click( null, null );
                    } catch ( Exception excp ) {
                        MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    }
                } else if ( msgResp == DialogResult.No ) {
                    isDataModified = false;
                }
            }
        }

        private void navRefresh_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                checkModifyPrompt();
            }
            if ( !( isDataModified ) ) {
                //Retrieve running order data for display
                loadTeamDataView();
                mySlalomDataTable = getTeamAsgmtData( "Slalom" );
                loadSlalomDataView( mySlalomDataTable );
                myTrickDataTable = getTeamAsgmtData( "Trick" );
                loadTrickDataView( myTrickDataTable );
                myJumpDataTable = getTeamAsgmtData( "Jump" );
                loadJumpDataView( myJumpDataTable );
            }
        }

        private void navExport_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            String[] curSelectCommand = new String[3];
            String[] curTableName = { "TeamList", "TeamOrder", "EventReg" };

            //----------------------------------------
            //Export data related to teams
            //----------------------------------------
            curSelectCommand[0] = "SELECT * FROM TeamList Where SanctionId = '" + mySanctionNum + "' ";
            curSelectCommand[1] = "SELECT * FROM TeamOrder Where SanctionId = '" + mySanctionNum + "' ";
            curSelectCommand[2] = "SELECT SanctionId, MemberId, AgeGroup, TeamCode, Event, LastUpdateDate FROM EventReg Where SanctionId = '" + mySanctionNum + "' AND TeamCode is not null ";

            myExportData.exportData( curTableName, curSelectCommand );
        }

        private void navSaveAs_Click(object sender, EventArgs e) {
            ExportData myExportData = new ExportData();
            loadPrintTeamSkierList();
            PrintMemberId.Visible = true;
            myExportData.exportData(PrintTeamDataGridView);
            PrintMemberId.Visible = false;
        }

        private void SlalomOrderButton_CheckedChanged(object sender, EventArgs e) {
            loadTeamDataView();
        }

        private void TrickOrderButton_CheckedChanged( object sender, EventArgs e ) {
            loadTeamDataView();
        }

        private void JumpOrderButton_CheckedChanged( object sender, EventArgs e ) {
            loadTeamDataView();
        }

        private void AllOrderButton_CheckedChanged( object sender, EventArgs e ) {
            loadTeamDataView();
        }

        private void loadTeamDataView() {
            DataGridViewRow curViewRow;
            myViewLoad = true;

            //Retrieve data for current tournament
            //Used for initial load and to refresh data after updates
            winStatusMsg.Text = "Retrieving tournament entries";
            Cursor.Current = Cursors.WaitCursor;

            try {
                isDataModified = false;
                Int16 curRunOrder;
                String curValue;
                myTeamRowIdx = 0;
                TeamDataGridView.Rows.Clear();
                myTeamDataTable = getTeamData();
                if ( myTeamDataTable.Rows.Count > 0 ) {
                    if ( myRules.ToLower().Equals( "ncwsa" ) ) {
                        DataRow[] curFindRows;
                        String[] curDivList = { "CM", "CW", "BM", "BW" };
                        foreach ( String curDiv in curDivList ) {
                            curFindRows = myTeamDataTable.Select( "AgeGroup = '" + curDiv + "'" );
                            if ( curFindRows.Length <= 0 ) continue;
                            foreach ( DataRow curDataRow in curFindRows ) {
                                myTeamRowIdx = TeamDataGridView.Rows.Add();
                                curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
                                curViewRow.Cells["TeamUpdated"].Value = "N";
                                curViewRow.Cells["TeamPK"].Value = ( (Int64)curDataRow["TeamPK"] ).ToString();
                                curViewRow.Cells["OrderPK"].Value = ( (Int64)curDataRow["OrderPK"] ).ToString();
                                curViewRow.Cells["TeamSanctionId"].Value = (String)curDataRow["SanctionId"];
                                
                                curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                                curViewRow.Cells["TeamName"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Name", "" );
                                curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                                curViewRow.Cells["EventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                                curValue = HelperFunctions.getDataRowColValue( curDataRow, "SlalomRunOrder", "0" ); 
                                if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                    curViewRow.Cells["TeamSlalomOrder"].Value = Convert.ToInt16( curValue );
                                } else {
                                    curViewRow.Cells["TeamSlalomOrder"].Value = 0;
                                }
                                curValue = HelperFunctions.getDataRowColValue( curDataRow, "TrickRunOrder", "0" );
                                if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                    curViewRow.Cells["TeamTrickOrder"].Value = Convert.ToInt16( curValue );
                                } else {
                                    curViewRow.Cells["TeamTrickOrder"].Value = 0;
                                }
                                curValue = HelperFunctions.getDataRowColValue( curDataRow, "JumpRunOrder", "0" );
                                if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                    curViewRow.Cells["TeamJumpOrder"].Value = Convert.ToInt16( curValue );
                                } else {
                                    curViewRow.Cells["TeamJumpOrder"].Value = 0;
                                }
                                curViewRow.Cells["TeamContactName"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ContactName", "" );
                                curViewRow.Cells["TeamContactInfo"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ContactInfo", "" );
                                curViewRow.Cells["TeamNotes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Notes", "" );
                            }
                        }
                    } else {
                        foreach ( DataRow curDataRow in myTeamDataTable.Rows ) {
                            myTeamRowIdx = TeamDataGridView.Rows.Add();
                            curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
                            curViewRow.Cells["TeamUpdated"].Value = "N";
                            curViewRow.Cells["TeamPK"].Value = ( (Int64)curDataRow["TeamPK"] ).ToString();
                            curViewRow.Cells["OrderPK"].Value = ( (Int64)curDataRow["OrderPK"] ).ToString();
                            curViewRow.Cells["TeamSanctionId"].Value = (String)curDataRow["SanctionId"];

                            curViewRow.Cells["TeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                            curViewRow.Cells["TeamName"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Name", "" );
                            curViewRow.Cells["AgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                            curViewRow.Cells["EventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                            curValue = HelperFunctions.getDataRowColValue( curDataRow, "SlalomRunOrder", "" );
                            if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                curViewRow.Cells["TeamSlalomOrder"].Value = Convert.ToInt16( curValue );
                            } else {
                                curViewRow.Cells["TeamSlalomOrder"].Value = "";
                            }
                            curValue = HelperFunctions.getDataRowColValue( curDataRow, "TrickRunOrder", "" );
                            if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                curViewRow.Cells["TeamTrickOrder"].Value = Convert.ToInt16( curValue );
                            } else {
                                curViewRow.Cells["TeamTrickOrder"].Value = "";
                            }
                            curValue = HelperFunctions.getDataRowColValue( curDataRow, "JumpRunOrder", "" );
                            if ( HelperFunctions.isObjectPopulated( curValue ) ) {
                                curViewRow.Cells["TeamJumpOrder"].Value = Convert.ToInt16( curValue );
                            } else {
                                curViewRow.Cells["TeamJumpOrder"].Value = "";
                            }
                            curViewRow.Cells["TeamContactName"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ContactName", "" );
                            curViewRow.Cells["TeamContactInfo"].Value = HelperFunctions.getDataRowColValue( curDataRow, "ContactInfo", "" );
                            curViewRow.Cells["TeamNotes"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Notes", "" );
                        }
                    }
                    myTeamRowIdx = 0;
                    TeamDataGridView.CurrentCell = TeamDataGridView.Rows[myTeamRowIdx].Cells["TeamCode"];
                    isDataModified = false;
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament teams \n" + ex.Message );
            }

            Cursor.Current = Cursors.Default;
            myViewLoad = false;
        }

        private void loadSlalomDataView(DataTable inDataTable) {
            Decimal curRankingScore;
            DataGridViewRow curViewRow;

            try {
                TeamSlalomDataGridView.Rows.Clear();
                SlalomNumLabel.Text = "";
                if ( inDataTable.Rows.Count > 0 ) {
                    foreach ( DataRow curDataRow in inDataTable.Rows ) {
                        mySlalomRowIdx = TeamSlalomDataGridView.Rows.Add();
                        curViewRow = TeamSlalomDataGridView.Rows[mySlalomRowIdx];
                        curViewRow.Cells["SlalomUpdated"].Value = "N";
                        curViewRow.Cells["SlalomPK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["SlalomSanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["SlalomMemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["SlalomSkierName"].Value = (String)curDataRow["SkierName"];
                        
                        curViewRow.Cells["SlalomTeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                        curViewRow.Cells["SlalomRunOrder"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RunOrder", "1" );
                        curViewRow.Cells["SlalomRotation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Rotation", "1" );
                        curViewRow.Cells["SlalomAgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                        curViewRow.Cells["SlalomEventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                        curViewRow.Cells["SlalomRankingScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingScore", "" );
                    }
                    mySlalomRowIdx = 0;
                    TeamSlalomDataGridView.CurrentCell = TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomSkierName"];
                    SlalomNumLabel.Text = TeamSlalomDataGridView.Rows.Count.ToString();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament slalom team assignments \n" + ex.Message );
            }

        }

        private void loadTrickDataView( DataTable inDataTable ) {
            Decimal curRankingScore;
            DataGridViewRow curViewRow;

            try {
                TeamTrickDataGridView.Rows.Clear();
                TrickNumLabel.Text = "";

                if ( inDataTable.Rows.Count > 0 ) {
                    foreach ( DataRow curDataRow in inDataTable.Rows ) {
                        myTrickRowIdx = TeamTrickDataGridView.Rows.Add();
                        curViewRow = TeamTrickDataGridView.Rows[myTrickRowIdx];
                        curViewRow.Cells["TrickUpdated"].Value = "N";
                        curViewRow.Cells["TrickPK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["TrickSanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["TrickMemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["TrickSkierName"].Value = (String)curDataRow["SkierName"];

                        curViewRow.Cells["TrickTeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                        curViewRow.Cells["TrickRunOrder"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RunOrder", "1" );
                        curViewRow.Cells["TrickRotation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Rotation", "1" );
                        curViewRow.Cells["TrickAgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                        curViewRow.Cells["TrickEventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                        curViewRow.Cells["TrickRankingScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingScore", "" );
                    }
                    myTrickRowIdx = 0;
                    TeamTrickDataGridView.CurrentCell = TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickSkierName"];
                    TrickNumLabel.Text = TeamTrickDataGridView.Rows.Count.ToString();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament Trick team assignments \n" + ex.Message );
            }
        }

        private void loadJumpDataView( DataTable inDataTable ) {
            Decimal curRankingScore;
            DataGridViewRow curViewRow;

            try {
                JumpNumLabel.Text = "";
                TeamJumpDataGridView.Rows.Clear();

                if ( inDataTable.Rows.Count > 0 ) {
                    foreach ( DataRow curDataRow in inDataTable.Rows ) {
                        myJumpRowIdx = TeamJumpDataGridView.Rows.Add();
                        curViewRow = TeamJumpDataGridView.Rows[myJumpRowIdx];
                        curViewRow.Cells["JumpUpdated"].Value = "N";
                        curViewRow.Cells["JumpPK"].Value = ( (Int64)curDataRow["PK"] ).ToString();
                        curViewRow.Cells["JumpSanctionId"].Value = (String)curDataRow["SanctionId"];
                        curViewRow.Cells["JumpMemberId"].Value = (String)curDataRow["MemberId"];
                        curViewRow.Cells["JumpSkierName"].Value = (String)curDataRow["SkierName"];

                        curViewRow.Cells["JumpTeamCode"].Value = HelperFunctions.getDataRowColValue( curDataRow, "TeamCode", "" );
                        curViewRow.Cells["JumpRunOrder"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RunOrder", "1" );
                        curViewRow.Cells["JumpRotation"].Value = HelperFunctions.getDataRowColValue( curDataRow, "Rotation", "1" );
                        curViewRow.Cells["JumpAgeGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "AgeGroup", "" );
                        curViewRow.Cells["JumpEventGroup"].Value = HelperFunctions.getDataRowColValue( curDataRow, "EventGroup", "" );
                        curViewRow.Cells["JumpRankingScore"].Value = HelperFunctions.getDataRowColValue( curDataRow, "RankingScore", "" );
                    }
                    myJumpRowIdx = 0;
                    TeamJumpDataGridView.CurrentCell = TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpSkierName"];
                    JumpNumLabel.Text = TeamJumpDataGridView.Rows.Count.ToString();
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error retrieving tournament Jump team assignments \n" + ex.Message );
            }
        }

        private void loadPrintTeamSkierList() {
            DataGridViewRow curViewRow;
            String curTeamCode = "", prevTeamCode = "", curEvent = "", prevEvent = "";
            Cursor.Current = Cursors.WaitCursor;

            try {
                int curRowIdx = 0;
                PrintTeamDataGridView.Rows.Clear();
                DataTable curDataTable = getTeamSkierList();
                if ( curDataTable.Rows.Count > 0 ) {
                    foreach ( DataRow curDataRow in curDataTable.Rows ) {
                        curRowIdx = PrintTeamDataGridView.Rows.Add();
                        curViewRow = PrintTeamDataGridView.Rows[curRowIdx];

                        curTeamCode = (String) curDataRow["TeamCode"];
                        curEvent = (String) curDataRow["Event"];

                        if ( curRowIdx > 0) {
                            if ( curTeamCode == prevTeamCode ) {
                                if ( curEvent == prevEvent ) {
                                    curViewRow.Cells["PrintTeamCode"].Value = "";
                                    curViewRow.Cells["PrintTeamName"].Value = "";
                                    curViewRow.Cells["PrintEvent"].Value = "";
                                } else {
                                    curViewRow.DefaultCellStyle.BackColor = Color.Silver;
                                    curViewRow.Height = 8;

                                    curRowIdx = PrintTeamDataGridView.Rows.Add();
                                    curViewRow = PrintTeamDataGridView.Rows[curRowIdx];

                                    curViewRow.Cells["PrintTeamCode"].Value = "";
                                    curViewRow.Cells["PrintTeamName"].Value = "";
                                    try {
                                        curViewRow.Cells["PrintEvent"].Value = ( (String) curDataRow["Event"] ).ToString();
                                    } catch {
                                        curViewRow.Cells["PrintEvent"].Value = "";
                                    }
                                }
                            } else {
                                curViewRow.DefaultCellStyle.BackColor = Color.DarkGray;
                                curViewRow.Height = 24;

                                curRowIdx = PrintTeamDataGridView.Rows.Add();
                                curViewRow = PrintTeamDataGridView.Rows[curRowIdx];

                                try {
                                    curViewRow.Cells["PrintTeamCode"].Value = (String) curDataRow["TeamCode"];
                                } catch {
                                    curViewRow.Cells["PrintTeamCode"].Value = "";
                                }
                                try {
                                    curViewRow.Cells["PrintTeamName"].Value = (String) curDataRow["Name"];
                                } catch {
                                    curViewRow.Cells["PrintTeamName"].Value = "";
                                }
                                try {
                                    curViewRow.Cells["PrintEvent"].Value = ( (String) curDataRow["Event"] ).ToString();
                                } catch {
                                    curViewRow.Cells["PrintEvent"].Value = "";
                                }
                            }
                        } else {
                            try {
                                curViewRow.Cells["PrintTeamCode"].Value = (String) curDataRow["TeamCode"];
                            } catch {
                                curViewRow.Cells["PrintTeamCode"].Value = "";
                            }
                            try {
                                curViewRow.Cells["PrintTeamName"].Value = (String) curDataRow["Name"];
                            } catch {
                                curViewRow.Cells["PrintTeamName"].Value = "";
                            }
                            try {
                                curViewRow.Cells["PrintEvent"].Value = ( (String) curDataRow["Event"] ).ToString();
                            } catch {
                                curViewRow.Cells["PrintEvent"].Value = "";
                            }
                        }

                        try {
                            curViewRow.Cells["PrintSkierDiv"].Value = ( (String) curDataRow["AgeGroup"] ).ToString();
                        } catch {
                            curViewRow.Cells["PrintSkierDiv"].Value = "";
                        }
                        try {
                            curViewRow.Cells["PrintSkierEventGroup"].Value = ( (String) curDataRow["EventGroup"] ).ToString();
                        } catch {
                            curViewRow.Cells["PrintSkierEventGroup"].Value = "";
                        }
                        try {
                            curViewRow.Cells["PrintSkierName"].Value = ( (String) curDataRow["SkierName"] ).ToString();
                        } catch {
                            curViewRow.Cells["PrintSkierName"].Value = "";
                        }
                        try {
                            curViewRow.Cells["PrintMemberId"].Value = ( (String) curDataRow["MemberId"] ).ToString();
                        } catch {
                            curViewRow.Cells["PrintMemberId"].Value = "";
                        }
                        try {
                            curViewRow.Cells["PrintRankingScore"].Value = ( (Decimal) curDataRow["RankingScore"] ).ToString();
                        } catch {
                            curViewRow.Cells["PrintRankingScore"].Value = "";
                        }

                        prevTeamCode = curTeamCode;
                        prevEvent = curEvent;

                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show("Error retrieving tournament teams \n" + ex.Message);
            }

            Cursor.Current = Cursors.Default;
            myViewLoad = false;
        }

        private void ShowTeamButton_Click( object sender, EventArgs e ) {
            myShowTeam = true;
            String curFilterCommand = "", curTeamCode = "", curDiv = "", curGroup = "";
            if ( TeamDataGridView.Rows.Count > 0 ) {
                if ( TeamDataGridView.CurrentRow == null ) {
                    curTeamCode = TeamDataGridView.Rows[0].Cells["TeamCode"].Value.ToString();
                    curDiv = TeamDataGridView.Rows[0].Cells["AgeGroup"].Value.ToString();
                    curGroup = TeamDataGridView.Rows[0].Cells["EventGroup"].Value.ToString();
                } else {
                    curTeamCode = TeamDataGridView.CurrentRow.Cells["TeamCode"].Value.ToString();
                    curDiv = TeamDataGridView.CurrentRow.Cells["AgeGroup"].Value.ToString();
                    curGroup = TeamDataGridView.CurrentRow.Cells["EventGroup"].Value.ToString();
                }
                curFilterCommand = "TeamCode = '" + curTeamCode + "'";
                if (curDiv.Length > 0) {
                    curFilterCommand += " AND AgeGroup = '" + curDiv + "'";
                }
                if (curGroup.Length > 0) {
                    curFilterCommand += " AND EventGroup = '" + curGroup + "'";
                }
                
                mySlalomDataTable.DefaultView.RowFilter = curFilterCommand;
                loadSlalomDataView( mySlalomDataTable.DefaultView.ToTable() );
                myTrickDataTable.DefaultView.RowFilter = curFilterCommand;
                loadTrickDataView( myTrickDataTable.DefaultView.ToTable() );
                myJumpDataTable.DefaultView.RowFilter = curFilterCommand;
                loadJumpDataView( myJumpDataTable.DefaultView.ToTable() );
            }
        }

        private void ShowAllButton_Click( object sender, EventArgs e ) {
            myShowTeam = false;
            mySlalomDataTable.DefaultView.RowFilter = "";
            loadSlalomDataView( mySlalomDataTable.DefaultView.ToTable() );
            myTrickDataTable.DefaultView.RowFilter = "";
            loadTrickDataView( myTrickDataTable.DefaultView.ToTable() );
            myJumpDataTable.DefaultView.RowFilter = "";
            loadJumpDataView( myJumpDataTable.DefaultView.ToTable() );
        }

        private void ShowUnAsgmtButton_Click( object sender, EventArgs e ) {
            String curFilterCommand = "TeamCode = '' OR TeamCode is null";
            mySlalomDataTable.DefaultView.RowFilter = curFilterCommand;
            loadSlalomDataView( mySlalomDataTable.DefaultView.ToTable() );
            
            myTrickDataTable.DefaultView.RowFilter = curFilterCommand;
            loadTrickDataView( myTrickDataTable.DefaultView.ToTable() );
            
            myJumpDataTable.DefaultView.RowFilter = curFilterCommand;
            loadJumpDataView( myJumpDataTable.DefaultView.ToTable() );
        }

        private void SlalomAddButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:SlalomAddButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;
                String curTeamCode = "";
                try {
                    curTeamCode = TeamDataGridView.Rows[myTeamRowIdx].Cells["TeamCode"].Value.ToString();
                } catch {
                    curTeamCode = "";
                }
                Int64 curPK = Convert.ToInt64( TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomPK"].Value.ToString() );
                TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomTeamCode"].Value = curTeamCode;

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '" + curTeamCode + "' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;
                if ( myRules.ToLower().Equals( "ncwsa" ) ) verifyNcwsaTeamSize();

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier slalom team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void SlalomRemoveButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:SlalomRemoveButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;
                Int64 curPK = Convert.ToInt64( TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomPK"].Value.ToString() );
                TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomTeamCode"].Value = "";

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier slalom team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void TrickAddButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:TrickAddButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;
                String curTeamCode = "";
                try {
                    curTeamCode = TeamDataGridView.Rows[myTeamRowIdx].Cells["TeamCode"].Value.ToString();
                } catch {
                    curTeamCode = "";
                }
                Int64 curPK = Convert.ToInt64( TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickPK"].Value.ToString() );
                TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickTeamCode"].Value = curTeamCode;

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '" + curTeamCode + "' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;

                if ( myRules.ToLower().Equals( "ncwsa" ) ) verifyNcwsaTeamSize();

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier Trick team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void TrickRemoveButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:TrickRemoveButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;

                Int64 curPK = Convert.ToInt64( TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickPK"].Value.ToString() );
                TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickTeamCode"].Value = "";

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier Trick team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void JumpAddButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:JumpAddButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;
                String curTeamCode = "";
                try {
                    curTeamCode = TeamDataGridView.Rows[myTeamRowIdx].Cells["TeamCode"].Value.ToString();
                } catch {
                    curTeamCode = "";
                }
                Int64 curPK = Convert.ToInt64( TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpPK"].Value.ToString() );
                TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpTeamCode"].Value = curTeamCode;

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '" + curTeamCode + "' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;

                if ( myRules.ToLower().Equals( "ncwsa" ) ) verifyNcwsaTeamSize();

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier Jump team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void JumpRemoveButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:JumpRemoveButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                Int64 curPK = Convert.ToInt64( TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpPK"].Value.ToString() );
                TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpTeamCode"].Value = "";
                isDataModified = true;

                curSqlStmt.Append( "Update EventReg " );
                curSqlStmt.Append( "Set TeamCode = '' " );
                curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                curSqlStmt.Append( "Where PK = " + curPK.ToString() );
                int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                isDataModified = false;

            } catch ( Exception excp ) {
                curMsg = "Error attempting to update skier Jump team assignment \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void TeamSlalomDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( !( myViewLoad ) ) {
                mySlalomRowIdx = e.RowIndex;
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 5;
                curTimerObj.Tick += new EventHandler( setPosTeamSlalomViewTimer );
                curTimerObj.Start();
            }
        }

        private void TeamTrickDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( !( myViewLoad ) ) {
                myTrickRowIdx = e.RowIndex;
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 5;
                curTimerObj.Tick += new EventHandler( setPosTeamTrickViewTimer );
                curTimerObj.Start();
            }
        }

        private void TeamJumpDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( !( myViewLoad ) ) {
                myJumpRowIdx = e.RowIndex;
                Timer curTimerObj = new Timer();
                curTimerObj.Interval = 5;
                curTimerObj.Tick += new EventHandler( setPosTeamJumpViewTimer );
                curTimerObj.Start();
            }
        }

        private void setPosTeamSlalomViewTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( setPosTeamSlalomViewTimer );
            String curMemberId = TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomMemberId"].Value.ToString();
            setPosTeamTrickView( curMemberId );
            setPosTeamJumpView( curMemberId );
        }

        private void setPosTeamSlalomView( String inMemberId ) {
            bool curFound = false;
            foreach ( DataGridViewRow curRow in TeamSlalomDataGridView.Rows ) {
                if ( curRow.Cells["SlalomMemberId"].Value.Equals( inMemberId ) ) {
                    curFound = true;
                    mySlalomRowIdx = curRow.Index;
                    break;
                }
            }
            if ( curFound ) {
                TeamSlalomDataGridView.CurrentCell = TeamSlalomDataGridView.Rows[mySlalomRowIdx].Cells["SlalomTeamCode"];
            }
        }

        private void setPosTeamTrickViewTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( setPosTeamTrickViewTimer );
            String curMemberId = TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickMemberId"].Value.ToString();
            setPosTeamSlalomView( curMemberId );
            setPosTeamJumpView( curMemberId );
        }

        private void setPosTeamTrickView( String inMemberId ) {
            bool curFound = false;
            foreach ( DataGridViewRow curRow in TeamTrickDataGridView.Rows ) {
                if ( curRow.Cells["TrickMemberId"].Value.Equals( inMemberId ) ) {
                    curFound = true;
                    myTrickRowIdx = curRow.Index;
                    break;
                }
            }
            if ( curFound ) {
                TeamTrickDataGridView.CurrentCell = TeamTrickDataGridView.Rows[myTrickRowIdx].Cells["TrickTeamCode"];
            }
        }

        private void setPosTeamJumpViewTimer( object sender, EventArgs e ) {
            Timer curTimerObj = (Timer)sender;
            curTimerObj.Stop();
            curTimerObj.Tick -= new EventHandler( setPosTeamJumpViewTimer );
            String curMemberId = TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpMemberId"].Value.ToString();
            setPosTeamSlalomView( curMemberId );
            setPosTeamTrickView( curMemberId );
        }

        private void setPosTeamJumpView( String inMemberId ) {
            bool curFound = false;
            foreach ( DataGridViewRow curRow in TeamJumpDataGridView.Rows ) {
                if ( curRow.Cells["JumpMemberId"].Value.Equals( inMemberId ) ) {
                    curFound = true;
                    myJumpRowIdx = curRow.Index;
                    break;
                }
            }
            if ( curFound ) {
                TeamJumpDataGridView.CurrentCell = TeamJumpDataGridView.Rows[myJumpRowIdx].Cells["JumpTeamCode"];
            }
        }

        private void TeamDataGridView_RowEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( TeamDataGridView.RowCount > 0 ) {
                if ( isDataModified && ( myTeamRowIdx != e.RowIndex ) ) {
                    try {
                        navSave_Click( null, null );
                    } catch ( Exception excp ) {
                        MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
                    }
                }
                if ( !( isDataModified ) ) {
                    if ( myTeamRowIdx != e.RowIndex ) {
                        myTeamRowIdx = e.RowIndex;
                        if ( myShowTeam ) {
                            if ( TeamDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value != null ) {
                                String curFilterCommand = "", curTeamCode = "", curGroup = "", curDiv = "";
                                curTeamCode = TeamDataGridView.Rows[e.RowIndex].Cells["TeamCode"].Value.ToString();
                                curDiv = TeamDataGridView.Rows[e.RowIndex].Cells["AgeGroup"].Value.ToString();
                                curGroup = TeamDataGridView.Rows[e.RowIndex].Cells["EventGroup"].Value.ToString();
                                curFilterCommand = "TeamCode = '" + curTeamCode + "'";
                                if (curDiv.Length > 0) {
                                    curFilterCommand += " AND AgeGroup = '" + curDiv + "'";
                                }
                                if (curGroup.Length > 0) {
                                    curFilterCommand += " AND EventGroup = '" + curGroup + "'";
                                }

                                mySlalomDataTable.DefaultView.RowFilter = curFilterCommand;
                                loadSlalomDataView( mySlalomDataTable.DefaultView.ToTable() );
                                myTrickDataTable.DefaultView.RowFilter = curFilterCommand;
                                loadTrickDataView( myTrickDataTable.DefaultView.ToTable() );
                                myJumpDataTable.DefaultView.RowFilter = curFilterCommand;
                                loadJumpDataView( myJumpDataTable.DefaultView.ToTable() );
                            }
                        }
                    }
                }

            } else {
                myTeamRowIdx = 0;
            }
        }

        private void TeamDataGridView_CellEnter( object sender, DataGridViewCellEventArgs e ) {
            if ( TeamDataGridView.Rows.Count > 0 ) {
                myTeamRowIdx = e.RowIndex;
                String curColName = TeamDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
                if ( curColName.Equals( "TeamCode" )
                    || curColName.Equals( "TeamName" )
                    || curColName.Equals( "AgeGroup" )
                    || curColName.Equals( "EventGroup" )
                    || curColName.Equals( "TeamContactName" )
                    || curColName.Equals( "TeamContactInfo" )
                    || curColName.Equals( "TeamNotes" )
                    ) {
                    try {
                        myOrigItemValue = (String)curViewRow.Cells[e.ColumnIndex].Value;
                    } catch {
                        myOrigItemValue = "";
                    }
                } else if ( curColName.Equals( "TeamSlalomOrder" )
                    || curColName.Equals( "TeamTrickOrder" )
                    || curColName.Equals( "TeamJumpOrder" )
                    ) {
                    if ( HelperFunctions.IsObjectInt( HelperFunctions.getViewRowColValue( curViewRow, curColName, "0" ) ) ) {
                        myOrigItemValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "0" );
                    }
                }
            }
        }

        private void TeamDataGridView_CellValidating( object sender, DataGridViewCellValidatingEventArgs e ) {
            if ( TeamDataGridView.Rows.Count > 0 ) {
                myTeamRowIdx = e.RowIndex;
                String curColName = TeamDataGridView.Columns[e.ColumnIndex].Name;
                DataGridViewRow curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
                if ( curColName.Equals( "TeamCode" )
                    || curColName.Equals( "TeamName" )
                    || curColName.Equals( "AgeGroup" )
                    || curColName.Equals( "EventGroup" )
                    || curColName.Equals( "TeamContactName" )
                    || curColName.Equals( "TeamContactInfo" )
                    || curColName.Equals( "TeamNotes" )
                    ) {
                } else if ( curColName.Equals( "TeamSlalomOrder" )
                    || curColName.Equals( "TeamTrickOrder" )
                    || curColName.Equals( "TeamJumpOrder" )
                    ) {
                    if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( curViewRow, "TeamCode", "" ) ) ) {
                        if ( HelperFunctions.IsObjectInt( e.FormattedValue.ToString() ) ) {
                            e.Cancel = false;
                        } else {
                            e.Cancel = true;
                            MessageBox.Show( curColName + " must be numeric" );
                        }
                    }
                }
            }
        }

        private void TeamDataGridView_CellValidated( object sender, DataGridViewCellEventArgs e ) {
            if ( TeamDataGridView.Rows.Count == 0 ) return;

            myTeamRowIdx = e.RowIndex;
            String curColName = TeamDataGridView.Columns[e.ColumnIndex].Name;
            DataGridViewRow curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
            if ( curColName.Equals( "TeamCode" )
                || curColName.Equals( "TeamName" )
                || curColName.Equals( "AgeGroup" )
                || curColName.Equals( "EventGroup" )
                || curColName.Equals( "TeamContactName" )
                || curColName.Equals( "TeamContactInfo" )
                || curColName.Equals( "TeamNotes" )
                ) {

                if ( HelperFunctions.isObjectEmpty( HelperFunctions.getViewRowColValue( curViewRow, curColName, "" ) ) ) {
                    if ( !( HelperFunctions.isObjectEmpty( myOrigItemValue ) ) ) {
                        isDataModified = true;
                        curViewRow.Cells["TeamUpdated"].Value = "Y";
                    }
                } else {
                    String curValue = HelperFunctions.getViewRowColValue( curViewRow, curColName, "" );
                    if ( curValue != myOrigItemValue ) {
                        isDataModified = true;
                        curViewRow.Cells["TeamUpdated"].Value = "Y";
                    }
                }
            
            } else if ( curColName.Equals( "TeamSlalomOrder" )
                || curColName.Equals( "TeamTrickOrder" )
                || curColName.Equals( "TeamJumpOrder" )
                ) {
                if ( HelperFunctions.isObjectPopulated( HelperFunctions.getViewRowColValue( curViewRow, "TeamCode", "" ) ) ) {
                    Int16 curNum;
                    if ( HelperFunctions.IsObjectInt( HelperFunctions.getViewRowColValue( curViewRow, curColName, "0" ) ) ) {
                        curNum = Convert.ToInt16( HelperFunctions.getViewRowColValue( curViewRow, curColName, "0" ) );
                        if ( curNum != Convert.ToInt16( myOrigItemValue ) ) {
                            isDataModified = true;
                            curViewRow.Cells["TeamUpdated"].Value = "Y";
                        }
                    } else {
                        if ( Int16.TryParse( myOrigItemValue, out curNum ) ) {
                            if ( curNum > 0 ) {
                                isDataModified = true;
                                curViewRow.Cells["TeamUpdated"].Value = "Y";
                            }
                        }
                    }
                }
            }
        }

        private void TeamDataGridView_KeyUp( object sender, KeyEventArgs e ) {

        }

        private void navAdd_Click( object sender, EventArgs e ) {
            myTeamRowIdx = TeamDataGridView.Rows.Add();
            DataGridViewRow curViewRow = TeamDataGridView.Rows[myTeamRowIdx];
            curViewRow.Cells["TeamSanctionId"].Value = mySanctionNum;
            curViewRow.Cells["TeamUpdated"].Value = "Y";
            curViewRow.Cells["TeamPK"].Value = "-1";
            curViewRow.Cells["OrderPK"].Value = "-1";
            curViewRow.Cells["TeamCode"].ReadOnly = false;
            TeamDataGridView.CurrentCell = TeamDataGridView.Rows[myTeamRowIdx].Cells["TeamCode"];
            isDataModified = true;
        }

        private void navRemove_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:RemoveTeamButton_Click";
            String curMsg = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                isDataModified = true;
                String curTeamCode = HelperFunctions.getViewRowColValue( TeamDataGridView.Rows[myTeamRowIdx], "TeamCode", "" );
                if ( HelperFunctions.isObjectEmpty(curTeamCode) ) {
                    TeamDataGridView.Rows.Remove(TeamDataGridView.Rows[myTeamRowIdx]);
                    return;
                }

                Int64 curTeamPK = Convert.ToInt64( HelperFunctions.getViewRowColValue( TeamDataGridView.Rows[myTeamRowIdx], "TeamPK", "0" ) );
                String curAgeGroup = HelperFunctions.getViewRowColValue( TeamDataGridView.Rows[myTeamRowIdx], "AgeGroup", "" );
                String curEventGroup = HelperFunctions.getViewRowColValue( TeamDataGridView.Rows[myTeamRowIdx], "EventGroup", "" );
                Int64 curOrderPK = Convert.ToInt64( HelperFunctions.getViewRowColValue( TeamDataGridView.Rows[myTeamRowIdx], "OrderPK", "0" ) );
                if ( curOrderPK > 0 ) {
                    curSqlStmt.Append( "Delete TeamOrder Where PK = " + curOrderPK.ToString() );
                    int rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Select PK From TeamOrder " );
                    curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                    curSqlStmt.Append( "  And TeamCode = '" + curTeamCode + "' " );
                    DataTable curDataTable = getData( curSqlStmt.ToString() );
                    if ( curDataTable.Rows.Count > 0 ) {
                    } else {
                        curSqlStmt = new StringBuilder( "Delete TeamList Where PK = " + curTeamPK.ToString() );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                        if ( rowsProc > 0 ) {
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update EventReg " );
                            curSqlStmt.Append( "Set TeamCode = '' " );
                            curSqlStmt.Append( ", LastUpdateDate = GETDATE()" );
                            curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                            curSqlStmt.Append( "  AND TeamCode = '" + curTeamCode + "' " );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }
                    }

                    isDataModified = false;
                    navRefresh_Click( null, null );
                }

            } catch ( Exception excp ) {
                curMsg = "Error attempting to remove team \n" + excp.Message;
                MessageBox.Show( curMsg );
                Log.WriteFile( curMethodName + curMsg );
            }
        }

        private void StateTeamsButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:StateTeamsButton_Click";
            String curTeam = "", curMsg;
            int rowsProc;
            Int64 curPK;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            String dialogMsg = "This request will over write all previous team values assigned to skiers."
                + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show( dialogMsg, "Change Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            if ( msgResp == DialogResult.OK ) {
                Cursor.Current = Cursors.WaitCursor;

                try {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update EventReg Set " );
                    curSqlStmt.Append( "TeamCode = '' " );
                    curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    DataTable curDataTable = getSkierStateRegionData();
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        curPK = (Int64)curRow["PK"];
                        try {
                            curTeam = (String)curRow["State"];
                        } catch {
                            curTeam = "";
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update EventReg Set " );
                        curSqlStmt.Append( "TeamCode = '" + curTeam + "' " );
                        curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                        curSqlStmt.Append( "Where PK = " + curPK );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamList WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamOrder WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamList (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, Name, Notes) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, 'State of ' + TeamCode, 'State teams' " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    curSqlStmt.Append( "AND NOT EXISTS (SELECT 1 FROM TeamList " );
                    curSqlStmt.Append( "    WHERE EventReg.SanctionId = SanctionId AND EventReg.TeamCode = TeamCode ) " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamOrder (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder ) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, '', 1, 1, 1 " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamList Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamOrder Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    //Refresh data
                    navRefresh_Click( null, null );
                } catch ( Exception excp ) {
                    curMsg = "Error attempting to update skier team to state \n" + excp.Message;
                    MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + curMsg );
                }
                Cursor.Current = Cursors.Default;
            }

        }

        private void RegionTeamsButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:RegionTeamsButton_Click";
            String curTeam = "", curMsg;
            int rowsProc;
            Int64 curPK;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            String dialogMsg = "This request will over write all previous team values assigned to skiers."
                + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show( dialogMsg, "Change Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            if ( msgResp == DialogResult.OK ) {
                Cursor.Current = Cursors.WaitCursor;

                try {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update EventReg Set " );
                    curSqlStmt.Append( "TeamCode = '' " );
                    curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    DataTable curDataTable = getSkierStateRegionData();
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        curPK = (Int64)curRow["PK"];
                        try {
                            curTeam = (String)curRow["Region"];
                        } catch {
                            curTeam = "";
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update EventReg Set " );
                        curSqlStmt.Append( "TeamCode = '" + curTeam.Substring( 0, 1 ) + "' " );
                        curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                        curSqlStmt.Append( "Where PK = " + curPK );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamList WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamOrder WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamList (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, Name, Notes) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, 'Region of ' + TeamCode, 'Region teams' " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    curSqlStmt.Append( "AND NOT EXISTS (SELECT 1 FROM TeamList " );
                    curSqlStmt.Append( "    WHERE EventReg.SanctionId = SanctionId AND EventReg.TeamCode = TeamCode ) " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamOrder (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder ) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, '', 1, 1, 1 " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamList Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamOrder Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    //Refresh data
                    navRefresh_Click( null, null );
                } catch ( Exception excp ) {
                    curMsg = "Error attempting to update skier team to region\n" + excp.Message;
                    MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + curMsg );
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void FebTeamsButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:FebTeamsButton_Click";
            String curTeam = "", curMsg;
            int rowsProc;
            Int64 curPK;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            String dialogMsg = "This request will over write all previous team values assigned to skiers."
                + "\n Do you want to continue?";
            DialogResult msgResp =
                MessageBox.Show( dialogMsg, "Change Warning",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1 );
            if ( msgResp == DialogResult.OK ) {
                Cursor.Current = Cursors.WaitCursor;

                try {
                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update EventReg Set " );
                    curSqlStmt.Append( "TeamCode = '' " );
                    curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    DataTable curDataTable = getSkierFedData();
                    foreach ( DataRow curRow in curDataTable.Rows ) {
                        curPK = (Int64)curRow["PK"];
                        try {
                            curTeam = (String)curRow["Federation"];
                        } catch {
                            curTeam = "";
                        }

                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update EventReg Set " );
                        curSqlStmt.Append( "TeamCode = '" + curTeam + "' " );
                        curSqlStmt.Append( ", LastUpdateDate = getdate()" );
                        curSqlStmt.Append( "Where PK = " + curPK );
                        rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                        Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                    }

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamList WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Delete TeamOrder WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamList (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, Name, Notes) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, 'Country of ' + TeamCode, 'National teams' " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    curSqlStmt.Append( "AND NOT EXISTS (SELECT 1 FROM TeamList " );
                    curSqlStmt.Append( "    WHERE EventReg.SanctionId = SanctionId AND EventReg.TeamCode = TeamCode ) " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Insert into TeamOrder (" );
                    curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder ) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, '', 1, 1, 1 " );
                    curSqlStmt.Append( "FROM EventReg WHERE SanctionId = '" + mySanctionNum + "' AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamList Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    curSqlStmt = new StringBuilder( "" );
                    curSqlStmt.Append( "Update TeamOrder Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                    rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                    //Refresh data
                    navRefresh_Click( null, null );
                } catch ( Exception excp ) {
                    curMsg = "Error attempting to update skier team to federation \n" + excp.Message;
                    MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                    Log.WriteFile( curMethodName + curMsg );
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void RandomOrderButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:RandomOrderButton_Click";
            bool curFindActive = false, curNumFound = false, curFindNextActive = false;
            int curOrderNum = 0, curIdx, rowsProc;
            Int64 curPK;
            String curMsg = "", curGroup = "";
            StringBuilder curSqlStmt = new StringBuilder( "" );
            int curNumTeams = 0;
            int[] curUsedNums = new int[curNumTeams];
            Random curRandomNum = new Random();
            DataRow[] curFindTeamRows;

            try {
                Cursor.Current = Cursors.WaitCursor;

                if (TeamDataGridView.Rows.Count > 0) {
                    bool curTeamsByDiv = false, curTeamsByGroup = false;
                    try {
                        if (( (String)myTeamDataTable.Rows[0]["AgeGroup"] ).Length > 0) {
                            curTeamsByDiv = true;
                        }
                    } catch {
                        curTeamsByDiv = false;
                    }
                    try {
                        if (( (String)myTeamDataTable.Rows[0]["EventGroup"] ).Length > 0) {
                            curTeamsByGroup = true;
                        }
                    } catch {
                        curTeamsByGroup = false;
                    }
                    if (curTeamsByDiv && curTeamsByGroup) {
                        MessageBox.Show( "Teams may only be defined by division or by event group and not by both" );
                        return;
                    }
                    ArrayList curGroupList = new ArrayList();
                    if (curTeamsByDiv || curTeamsByGroup) {
                        foreach (DataRow curRow in myTeamDataTable.Rows) {
                            if (curTeamsByDiv) curGroup = (String)curRow["AgeGroup"];
                            if (curTeamsByGroup) curGroup = (String)curRow["EventGroup"];
                            if (curGroup.Length > 0) {
                                if (curGroupList.Contains( curGroup )) {
                                } else {
                                    curGroupList.Add( curGroup );
                                }
                            }
                        }
                    } else {
                        curGroupList.Add( "" );
                    }
                    foreach (String curTeamGroup in curGroupList) {
                        if (curTeamsByDiv || curTeamsByGroup) {
                            if (curTeamsByDiv) {
                                curFindTeamRows = myTeamDataTable.Select( "AgeGroup = '" + curTeamGroup + "'" );
                                curNumTeams = myTeamDataTable.Select( "AgeGroup = '" + curTeamGroup + "'" ).Length;
                            } else {
                                curFindTeamRows = myTeamDataTable.Select( "EventGroup like '" + curTeamGroup + "%'" );
                                curNumTeams = myTeamDataTable.Select( "EventGroup = '" + curTeamGroup + "'" ).Length;
                            }
                        } else {
                            curFindTeamRows = myTeamDataTable.Select( "SanctionId = '" + mySanctionNum + "' AND TeamCode <> ''" );
                            curNumTeams = myTeamDataTable.Rows.Count;
                        }
                        
                        curUsedNums = new int[curNumTeams];
                        foreach (DataRow curDataRow in curFindTeamRows) {
                            curFindNextActive = true;
                            while (curFindNextActive) {
                                curOrderNum = curRandomNum.Next( curFindTeamRows.Length + 1 );
                                curFindActive = true;
                                curNumFound = false;
                                curIdx = 0;
                                if (curOrderNum > 0) {
                                    while (curFindActive) {
                                        if (curIdx < curUsedNums.Length) {
                                            if (curUsedNums[curIdx] == 0) {
                                                curFindActive = false;
                                                curFindNextActive = false;
                                                curUsedNums[curIdx] = curOrderNum;
                                            } else {
                                                if (curUsedNums[curIdx] == curOrderNum) {
                                                    curFindActive = false;
                                                    curNumFound = true;
                                                }
                                            }
                                        } else {
                                            curFindActive = false;
                                        }
                                        curIdx++;
                                    }
                                }
                            }

                            curPK = (Int64)curDataRow["OrderPK"];
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update TeamOrder Set " );
                            curSqlStmt.Append( "LastUpdateDate = getdate()" );
                            if (AllOrderButton.Checked) {
                                curDataRow["SlalomRunOrder"] = Convert.ToInt16( curOrderNum );
                                curDataRow["TrickRunOrder"] = Convert.ToInt16( curOrderNum );
                                curDataRow["JumpRunOrder"] = Convert.ToInt16( curOrderNum );
                                curSqlStmt.Append( ", SlalomRunOrder = " + curOrderNum.ToString() );
                                curSqlStmt.Append( ", TrickRunOrder = " + curOrderNum.ToString() );
                                curSqlStmt.Append( ", JumpRunOrder = " + curOrderNum.ToString() );
                            }
                            if (SlalomOrderButton.Checked) {
                                curDataRow["SlalomRunOrder"] = Convert.ToInt16( curOrderNum );
                                curSqlStmt.Append( ", SlalomRunOrder = " + curOrderNum.ToString() );
                            }
                            if (TrickOrderButton.Checked) {
                                curDataRow["TrickRunOrder"] = Convert.ToInt16( curOrderNum );
                                curSqlStmt.Append( ", TrickRunOrder = " + curOrderNum.ToString() );
                            }
                            if (JumpOrderButton.Checked) {
                                curDataRow["JumpRunOrder"] = Convert.ToInt16( curOrderNum );
                                curSqlStmt.Append( ", JumpRunOrder = " + curOrderNum.ToString() );
                            }
                            curSqlStmt.Append( " Where PK = " + curPK );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }
                    }
                    Cursor.Current = Cursors.Default;
                }
            } catch ( Exception excp ) {
                curMsg = "Error attempting to update team running order\n" + excp.Message;
                MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + curMsg );
            }
            Cursor.Current = Cursors.Default;
            loadTeamDataView();
            
        }

        private void SkierOrderButton_Click( object sender, EventArgs e ) {
            if ( isDataModified ) {
                navSave_Click( null, null );
            }
            if ( !( isDataModified ) ) {
                //if (myRules.ToLower().Equals( "ncwsa" )) {
                //}
                if (myTeamDataTable.Rows.Count > 0) {
                        Cursor.Current = Cursors.WaitCursor;
                        if (SlalomOrderButton.Checked || AllOrderButton.Checked) {
                            updateSkierOrder( mySlalomDataTable, "Slalom" );
                        }
                        if (TrickOrderButton.Checked || AllOrderButton.Checked) {
                            updateSkierOrder( myTrickDataTable, "Trick" );
                        }
                        if (JumpOrderButton.Checked || AllOrderButton.Checked) {
                            updateSkierOrder( myJumpDataTable, "Jump" );
                        }
                        Cursor.Current = Cursors.Default;

                        //Refresh data
                        navRefresh_Click( null, null );
                    }
            }
        }

        private void GenTeamButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:TeamSetup:GenTeamButton_Click";
            int rowsProc;
            StringBuilder curSqlStmt = new StringBuilder( "" );

            try {
                Cursor.Current = Cursors.WaitCursor;

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Insert Into TeamList (SanctionId, TeamCode, Name, Notes ) " );
                curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, TeamCode, 'Generated team' " );
                curSqlStmt.Append( "FROM EventReg " );
                curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                curSqlStmt.Append( "  AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                curSqlStmt.Append( "  AND NOT EXISTS (SELECT 1 FROM TeamList " );
                curSqlStmt.Append( "      WHERE TeamList.SanctionId = EventReg.SanctionId AND TeamList.TeamCode = EventReg.TeamCode ) " );
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Insert into TeamOrder (" );
                if ( myRules.ToLower().Equals( "ncwsa" ) ) {
                    curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder ) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, AgeGroup, 1, 1, 1 " );
                    curSqlStmt.Append( "FROM EventReg " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                    curSqlStmt.Append( "  AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    curSqlStmt.Append( "  AND AgeGroup in ('CM', 'CW', 'BM', 'BW') " );
                    curSqlStmt.Append( "  AND NOT EXISTS (SELECT 1 FROM TeamOrder " );
                    curSqlStmt.Append( "      WHERE TeamOrder.SanctionId = EventReg.SanctionId AND TeamOrder.TeamCode = EventReg.TeamCode AND TeamOrder.AgeGroup = EventReg.AgeGroup ) " );

                } else {
                    curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder ) " );
                    curSqlStmt.Append( "SELECT DISTINCT SanctionId, TeamCode, '', 1, 1, 1 " );
                    curSqlStmt.Append( "FROM EventReg " );
                    curSqlStmt.Append( "WHERE SanctionId = '" + mySanctionNum + "' " );
                    curSqlStmt.Append( "  AND TeamCode IS NOT NULL AND TeamCode <> '' " );
                    curSqlStmt.Append( "  AND NOT EXISTS (SELECT 1 FROM TeamOrder " );
                    curSqlStmt.Append( "      WHERE TeamOrder.SanctionId = EventReg.SanctionId AND TeamOrder.TeamCode = EventReg.TeamCode ) " );
                }
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TeamList Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Update TeamOrder Set LastUpdateDate = getdate() WHERE SanctionId = '" + mySanctionNum + "' " );
                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

            } catch ( Exception excp ) {
                String curMsg = "Error attempting to update team running order\n" + excp.Message;
                MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + curMsg );
            }
            Cursor.Current = Cursors.Default;
            loadTeamDataView();
        }

        private void updateSkierOrder( DataTable inDataTable, String inEvent ) {
            String curMethodName = "Tournament:TeamSetup:updateSkierOrder";
            String curTeam = "", curGroup = "", curMsg = "", curSkierEventGroup = "";
            bool curTeamsByDiv = false, curTeamsByGroup = false;
            int rowsProc, curRotationNum = 0, curRunOrder = 0, curNumTeams = 0;
            Int64 curPK;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            DataRow[] curFindTeamRows;
            DataRow[] curFindSkierRows;
            curNumTeams = myTeamDataTable.Rows.Count;

            Cursor.Current = Cursors.WaitCursor;

            try {
                if (( (String)myTeamDataTable.Rows[0]["AgeGroup"] ).Length > 0) {
                    curTeamsByDiv = true;
                }
                if ( myTeamDataTable.Rows[0]["EventGroup"] != System.DBNull.Value ) {
                    if ( ( (String) myTeamDataTable.Rows[0]["EventGroup"] ).Length > 0 ) {
                        curTeamsByGroup = true;
                    }
                }
                if (curTeamsByDiv && curTeamsByGroup) {
                    MessageBox.Show( "Teams may only be defined by division or by event group and not by both" );
                    return;
                }

                ArrayList curGroupList = new ArrayList();
                if (curTeamsByDiv || curTeamsByGroup) {
                    foreach (DataRow curRow in myTeamDataTable.Rows) {
                        if (curTeamsByDiv) curGroup = (String)curRow["AgeGroup"];
                        if (curTeamsByGroup) curGroup = (String)curRow["EventGroup"];
                        if (curGroup.Length > 0) {
                            if (curGroupList.Contains( curGroup )) {
                            } else {
                                curGroupList.Add( curGroup );
                            }
                        }
                    }
                } else {
                    curGroupList.Add( "" );
                }

                foreach (String curTeamGroup in curGroupList) {
                    if (curTeamsByDiv || curTeamsByGroup) {
                        if (curTeamsByDiv) {
                            curFindSkierRows = inDataTable.Select( "AgeGroup = '" + curTeamGroup + "'" );
                            curNumTeams = myTeamDataTable.Select( "AgeGroup = '" + curTeamGroup + "'" ).Length;
                        } else {
                            curFindSkierRows = inDataTable.Select( "EventGroup like '" + curTeamGroup + "%'" );
                            curNumTeams = myTeamDataTable.Select( "EventGroup = '" + curTeamGroup + "'" ).Length;
                        }
                    } else {
                        curFindSkierRows = inDataTable.Select( "SanctionId = '" + mySanctionNum + "' AND TeamCode <> ''" );
                        curNumTeams = myTeamDataTable.Rows.Count;
                    }
                    
                    //Calculate skier run order as determined by rotation assignment and number of teams 
                    foreach (DataRow curDataRow in curFindSkierRows) {
                        curRotationNum = -1;
                        curPK = (Int64)curDataRow["PK"];
                        try {
                            curTeam = (String)curDataRow["TeamCode"];
                        } catch {
                            curTeam = "";
                        }
                        try {
                            if (curTeamsByDiv) {
                                curGroup = (String)curDataRow["AgeGroup"];
                            } else if (curTeamsByGroup) {
                                curGroup = (String)curDataRow["EventGroup"];
                            } else {
                                curGroup = "";
                            }
                        } catch {
                            curGroup = "";
                        }
                        if (( curTeam.Length > 0 ) && ( curGroup.Length > 0 )) {
                            curSkierEventGroup = (String)curDataRow["EventGroup"];
                            if (curSkierEventGroup.Length == 1) {
                                try {
                                    curRotationNum = Convert.ToInt32( curSkierEventGroup );
                                } catch {
                                    try {
                                        curRotationNum = (int)curDataRow["Rotation"];
                                    } catch {
                                        curRotationNum = 0;
                                    }
                                }
                            } else {
                                try {
                                    curRotationNum = Convert.ToInt32( curSkierEventGroup.Substring(1));
                                    //Special case to handle an issue with collegiate all stars that have 10 skiers and tenth is set to zero
                                    if (curRotationNum == 0) curRotationNum = 10; 
                                } catch {
                                    try {
                                        curRotationNum = (int)curDataRow["Rotation"];
                                    } catch {
                                        curRotationNum = 0;
                                    }
                                }
                            }
                        } else {
                            curRotationNum = 0;
                        }
                        if (curRotationNum > 0) {
                            if (curTeamsByDiv) {
                                curFindTeamRows = myTeamDataTable.Select( "TeamCode = '" + curTeam + "' AND AgeGroup = '" + curTeamGroup + "'" );
                            } else if (curTeamsByGroup) {
                                curFindTeamRows = myTeamDataTable.Select( "TeamCode = '" + curTeam + "' AND EventGroup = '" + curTeamGroup + "'" );
                            } else {
                                curFindTeamRows = myTeamDataTable.Select( "TeamCode = '" + curTeam + "'" );
                            }
                            if (curFindTeamRows.Length > 0) {
                                Int16 curTeamRunOrder = (Int16)curFindTeamRows[0][inEvent + "RunOrder"];
                                if (curTeamRunOrder == 0) curTeamRunOrder = 1;
                                curRunOrder = ( ( curRotationNum - 1 ) * curNumTeams ) + curTeamRunOrder;
                                curDataRow["RunOrder"] = curRunOrder;
                                curDataRow["Rotation"] = curRotationNum;
                                
                                curSqlStmt = new StringBuilder( "" );
                                curSqlStmt.Append( "Update EventReg Set " );
                                curSqlStmt.Append( "RunOrder = " + curRunOrder.ToString() );
                                curSqlStmt.Append( ", Rotation = " + curRotationNum.ToString() );
                                if (curTeamsByGroup) {
                                    curSqlStmt.Append( ", EventGroup = '" + curTeamGroup + "'" );
                                }
                                curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                                curSqlStmt.Append( "Where PK = " + curPK );
                                rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                                Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                            }
                        } else {
                            curRotationNum = 1;

                            curRunOrder = inDataTable.Rows.Count + curRotationNum;
                            curDataRow["RunOrder"] = curRunOrder;
                            curSqlStmt = new StringBuilder( "" );
                            curSqlStmt.Append( "Update EventReg Set " );
                            curSqlStmt.Append( "RunOrder = " + curRunOrder.ToString() );
                            curSqlStmt.Append( ", LastUpdateDate = getdate() " );
                            curSqlStmt.Append( "Where PK = " + curPK );
                            rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
                            Log.WriteFile( curMethodName + ":Rows=" + rowsProc.ToString() + " " + curSqlStmt.ToString() );
                        }
                    }
                }
            } catch (Exception excp) {
                curMsg = "Error attempting to update skier running order\n" + excp.Message;
                MessageBox.Show( curMsg + "\n\n" + curSqlStmt.ToString() );
                Log.WriteFile( curMethodName + curMsg );
            }
            Cursor.Current = Cursors.Default;
        }

         private DataTable getTeamData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT L.PK as TeamPk, O.PK as OrderPK, L.SanctionId, L.TeamCode, L.Name, O.AgeGroup, O.EventGroup" );
            curSqlStmt.Append( ", L.ContactName, L.ContactInfo, L.LastUpdateDate, L.Notes" );
            curSqlStmt.Append( ", O.SlalomRunOrder, O.TrickRunOrder, O.JumpRunOrder, O.SeedingScore " );
            curSqlStmt.Append( "FROM TeamList L " );
            curSqlStmt.Append( "Inner Join TeamOrder O ON O.SanctionId = L.SanctionId AND O.TeamCode = L.TeamCode ");
            curSqlStmt.Append( "WHERE L.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Order By Name , L.TeamCode, O.AgeGroup, O.EventGroup" );
            return getData( curSqlStmt.ToString());
        }

        private DataTable getTeamAsgmtData( String inEvent ) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.Event, E.SanctionId, E.MemberId, T.SkierName, E.EventGroup, E.RunOrder, COALESCE(Rotation, 0) as Rotation" );
            curSqlStmt.Append( ", E.TeamCode, E.EventClass, E.RankingScore, E.RankingRating, E.AgeGroup, E.HCapBase, E.HCapScore" );
            curSqlStmt.Append( ", T.TrickBoat, T.JumpHeight, T.State, T.Federation " );
            curSqlStmt.Append( "FROM EventReg E " );
            curSqlStmt.Append( "     INNER JOIN TourReg T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' AND E.Event = '" + inEvent + "' AND E.ReadyForPlcmt = 'Y' ");
            if ( myRules.ToLower().Equals( "ncwsa" ) ) {
                curSqlStmt.Append( "  AND E.AgeGroup in ('CM', 'CW', 'BM', 'BW') " );
            }
            curSqlStmt.Append( "Order By E.TeamCode, E.AgeGroup, E.EventGroup, T.SkierName " );

            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierFedData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.SanctionId, E.MemberId, E.Event, T.SkierName, E.TeamCode, E.AgeGroup, T.State, T.Federation " );
            curSqlStmt.Append( "FROM EventReg AS E " );
            curSqlStmt.Append( "  INNER JOIN TourReg AS T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "  AND T.Federation is not null " );
            curSqlStmt.Append( "  AND T.Federation <> '' " );
            curSqlStmt.Append( "Order By E.Event, E.AgeGroup, T.Federation, T.SkierName " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getSkierStateRegionData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT E.PK, E.SanctionId, E.MemberId, E.Event, T.SkierName, E.TeamCode, E.AgeGroup" );
            curSqlStmt.Append( ", T.State, T.Federation, COALESCE (L.CodeValue, N'') AS Region " );
            curSqlStmt.Append( "FROM EventReg AS E " );
            curSqlStmt.Append( "  INNER JOIN TourReg AS T ON E.SanctionId = T.SanctionId AND E.MemberId = T.MemberId AND E.AgeGroup = T.AgeGroup " );
            curSqlStmt.Append( "  INNER JOIN CodeValueList AS L ON L.ListName = 'StateRegion' AND L.ListCode = T.State " );
            curSqlStmt.Append( "WHERE E.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Order By E.Event, E.AgeGroup, T.State, T.SkierName " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTeamSkierList() {
            StringBuilder curSqlStmt = new StringBuilder("");
            curSqlStmt.Append("Select T.TeamCode, T.Name, R.SkierName, R.MemberId, E.Agegroup, E.Event, E.EventGroup, E.RankingScore ");
            curSqlStmt.Append( "From TeamList T ");
            curSqlStmt.Append( "Inner Join EventReg E ON E.SanctionId = T.SanctionId AND E.TeamCode = T.TeamCode ");
            curSqlStmt.Append( "Inner Join TourReg R ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "Where T.SanctionId = '" + mySanctionNum + "' ");
            curSqlStmt.Append("Order by T.Name, E.Event, E.AgeGroup, R.SkierName ");
            return getData(curSqlStmt.ToString());
        }

        private void verifyNcwsaTeamSize() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT T.TeamCode, O.AgeGroup, O.EventGroup, E.Event, count(*) as SkierCount " );
            curSqlStmt.Append( "FROM TeamList T " );
            curSqlStmt.Append( "Inner Join TeamOrder O ON O.SanctionId = T.SanctionId AND O.TeamCode = T.TeamCode " );
            curSqlStmt.Append( "Inner Join EventReg E ON E.SanctionId = T.SanctionId AND E.TeamCode = T.TeamCode AND E.AgeGroup = O.AgeGroup " );
            curSqlStmt.Append( "Inner Join TourReg R ON E.SanctionId = R.SanctionId AND E.MemberId = R.MemberId AND E.AgeGroup = R.AgeGroup " );
            curSqlStmt.Append( "Where T.SanctionId = '" + mySanctionNum + "' " );
            curSqlStmt.Append( "Group By E.Event, T.TeamCode, O.AgeGroup, O.EventGroup " );
            curSqlStmt.Append( "Having count(*) > 5" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if ( curDataTable.Rows.Count == 0 ) return;

            StringBuilder curWarnMsg = new StringBuilder( "" );
            foreach(DataRow curDataRow in curDataTable.Rows ) {
                if ( curWarnMsg.Length > 0 ) curWarnMsg.Append( "\n" );
                curWarnMsg.Append( String.Format( "Team={0}, Div={1}, Event={2}, Count={3}"
                    , (String)curDataRow["TeamCode"]
                    , (String)curDataRow["AgeGroup"]
                    , (String)curDataRow["Event"] 
                    , (int)curDataRow["SkierCount"]
                    ) );

            }
            MessageBox.Show( "The following teams have more then 5 skiers assigned \n\n" + curWarnMsg.ToString() );
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

        private String encodeSpecialChar( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            curValue = curValue.Replace( ",", "%2C" );
            return curValue;
        }

        private void navPrint_Click( object sender, EventArgs e ) {
            PrintPreviewDialog curPreviewDialog = new PrintPreviewDialog();
            PrintDialog curPrintDialog = new PrintDialog();

            loadPrintTeamSkierList();

            bool CenterOnPage = true;
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
                String printTitle = Properties.Settings.Default.Mdi_Title + " - Team List";
                myPrintDoc = new PrintDocument();
                myPrintDoc.DocumentName = this.Text;
                myPrintDoc.DefaultPageSettings.Margins = new Margins( 25, 25, 25, 25 );
                myPrintDoc.DefaultPageSettings.Landscape = true;
                myPrintDataGrid = new DataGridViewPrinter(PrintTeamDataGridView, myPrintDoc,
                    CenterOnPage, WithTitle, printTitle, fontPrintTitle, Color.DarkBlue, WithPaging );

                myPrintDoc.PrinterSettings = curPrintDialog.PrinterSettings;
                myPrintDoc.DefaultPageSettings = curPrintDialog.PrinterSettings.DefaultPageSettings;
                myPrintDoc.PrintPage += new PrintPageEventHandler( printDoc_PrintPage );
                curPreviewDialog.Document = myPrintDoc;
				curPreviewDialog.Size = new System.Drawing.Size( this.Width, this.Height );
                curPreviewDialog.Focus();
				curPreviewDialog.ShowDialog();
            }

        }

        // The PrintPage action for the PrintDocument control
        private void printDoc_PrintPage( object sender, System.Drawing.Printing.PrintPageEventArgs e ) {
            bool more = myPrintDataGrid.DrawDataGridView( e.Graphics );
            if ( more == true )
                e.HasMorePages = true;
        }

		private void navImport_Click( object sender, EventArgs e ) {
			String inputBuffer, curTeamCode = "", curEvent = "", curDiv = "";
			int curInputLineCount = 0, rowsProc = 0;
			Boolean rowFound = false;
			String[] inputCols = null, inputColNames = null;
			char[] tabDelim = new char[] { '\t' };
			/*
			*/

			DataTable curDataTable = null;

			StringBuilder curSqlStmt = new StringBuilder( "" );
			myImportMsg = new StringBuilder( "" );
			StreamReader myReader = null;

			try {
				myReader = getImportFile();
				if ( myReader != null ) {
					isDataModified = false;
					curInputLineCount = 0;
					inputBuffer = myReader.ReadLine();
					inputColNames = inputBuffer.Split( tabDelim );

					while ( ( inputBuffer = myReader.ReadLine() ) != null ) {
						curInputLineCount++;
						rowFound = false;
						inputCols = inputBuffer.Split( tabDelim );

						if ( inputCols.Length >= 6 ) {
							if ( inputCols[myTeamImportIdxTeamCode].Length > 0 ) {
								importTeamData( inputCols );
								curTeamCode = inputCols[myTeamImportIdxTeamCode];
							}

							if ( inputCols[myTeamImportIdxEvent].Length > 0 ) {
								curEvent = inputCols[myTeamImportIdxEvent];
							}
							if ( inputCols[myTeamImportIdxDiv].Length > 0 ) {
								curDiv = inputCols[myTeamImportIdxDiv];
							}

							String curMemberId = "";
							if ( inputCols[myTeamImportIdxMemberId].Length > 0 ) {
								curMemberId = inputCols[myTeamImportIdxMemberId];
								if ( curMemberId.Length < 9 ) {
									curMemberId = curMemberId.PadLeft( 9, '0' );
								}

							} else {
								if ( inputCols[myTeamImportIdxSkierName].Length > 0 ) {
									String curSkierName = inputCols[myTeamImportIdxSkierName];
									if ( curSkierName.StartsWith( "\"" ) ) {
										curSkierName = curSkierName.Substring( 1, curSkierName.Length - 2 );
									}

									curMemberId = searchForRegisteredSkierByName( curSkierName, curDiv, curEvent );
								}
							}

							if ( curMemberId.Length > 0 ) {
								curSqlStmt = new StringBuilder( "" );
								curSqlStmt.Append( "Select SanctionId, MemberId, AgeGroup, EventGroup, Event, TeamCode from EventReg " );
								curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
								curSqlStmt.Append( "And MemberId = '" + curMemberId + "' " );
								curSqlStmt.Append( "And AgeGroup = '" + curDiv + "' " );
								curSqlStmt.Append( "And Event = '" + curEvent + "' " );
								curDataTable = getData( curSqlStmt.ToString() );
								if ( curDataTable.Rows.Count > 0 ) {
									curSqlStmt = new StringBuilder( "" );
									curSqlStmt.Append( "Update EventReg Set TeamCode = '" + curTeamCode + "' " );
									curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
									curSqlStmt.Append( "And MemberId = '" + curMemberId + "' " );
									curSqlStmt.Append( "And AgeGroup = '" + curDiv + "' " );
									curSqlStmt.Append( "And Event = '" + curEvent + "' " );
									rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

									myImportMsg.Append( "\n* " +
										String.Format( "Added Skier {0} {1} Div {2} {3} Event to team {4}"
										, inputCols[myTeamImportIdxSkierName], curMemberId, curDiv, curEvent, curTeamCode ) );

								} else {
									myImportMsg.Append( "\n* " +
										String.Format( "Not Registered: Skier {0} {1} Div {2} for {3} Event "
										, inputCols[myTeamImportIdxSkierName], curMemberId, curDiv, curEvent ) );
								}
							}

						} else {
							//Assume no data on this line, bypassing record
						}
					}
				}

			} catch ( Exception ex ) {
				String ExcpMsg = ex.Message;
				if ( curSqlStmt != null ) {
					ExcpMsg += "\n" + curSqlStmt.ToString();
				}
				myImportMsg.Append( "\n\nError: Performing SQL operations" + "\nError: " + ExcpMsg );

			} finally {
				if ( myReader != null ) {
					myReader.Close();
					myReader.Dispose();
				}

				isDataModified = false;
				MessageBox.Show( "Team import processing complete: \n" + myImportMsg.ToString() );
				navRefresh_Click( null, null );
			}
		}

		private String searchForRegisteredSkierByName( String curSkierName, String curDiv, String curEvent) {
			String returnMemberId = "";
			String[] curNameNodes = curSkierName.Split( ',' );

			/*
			Search for skier in registration by name
			*/
			StringBuilder curSqlStmt = new StringBuilder( "" );
			// Search skier name assuming name matches registration name exactly (LastName, FirstName)
			curSqlStmt.Append( "SELECT ER.SanctionId, ER.MemberId, ER.AgeGroup, ER.EventGroup, ER.Event, ER.TeamCode, TR.SkierName " );
			curSqlStmt.Append( "FROM EventReg ER " );
			curSqlStmt.Append( "INNER JOIN TourReg TR ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
			curSqlStmt.Append( "WHERE ER.SanctionId = '" + mySanctionNum + "' " );
			curSqlStmt.Append( "AND TR.SkierName = '" + curSkierName + "' " );
			curSqlStmt.Append( "AND ER.AgeGroup = '" + curDiv + "' " );
			curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
			DataTable curDataTable = getData( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				returnMemberId = (String)curDataTable.Rows[0]["MemberId"];

			} else {
				/*
				Begin search using nodes from name provided
				Will need to provide a select list if more than 1 skier is found matching the search critera
				*/
				// Search skier name using just first node (LastName).  Will be selected if this last name is the only one in the specified division
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "SELECT ER.SanctionId, ER.MemberId, ER.AgeGroup, ER.EventGroup, ER.Event, ER.TeamCode, TR.SkierName " );
				curSqlStmt.Append( "FROM EventReg ER " );
				curSqlStmt.Append( "INNER JOIN TourReg TR ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
				curSqlStmt.Append( "WHERE ER.SanctionId = '" + mySanctionNum + "' " );
				curSqlStmt.Append( "AND TR.SkierName like '" + curNameNodes[0] + "%' " );
				curSqlStmt.Append( "AND ER.AgeGroup = '" + curDiv + "' " );
				curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
				curDataTable = getData( curSqlStmt.ToString() );
				if ( curDataTable.Rows.Count == 1 ) {
					returnMemberId = (String) curDataTable.Rows[0]["MemberId"];

				} else if ( curNameNodes.Length > 1 ) { 
					curSqlStmt = new StringBuilder( "" );
					// Search skier name using just first node (LastName) and 2nd node with wildcard looking for the FirstName as a portion of the full name.  Will be selected if this last name is the only one in the specified division
					curSqlStmt.Append( "SELECT ER.SanctionId, ER.MemberId, ER.AgeGroup, ER.EventGroup, ER.Event, ER.TeamCode, TR.SkierName " );
					curSqlStmt.Append( "FROM EventReg ER " );
					curSqlStmt.Append( "INNER JOIN TourReg TR ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
					curSqlStmt.Append( "WHERE ER.SanctionId = '" + mySanctionNum + "' " );
					curSqlStmt.Append( "AND TR.SkierName like '" + curNameNodes[0] + "%' " );
					curSqlStmt.Append( "AND TR.SkierName like '%" + curNameNodes[1] + "%' " );
					curSqlStmt.Append( "AND ER.AgeGroup = '" + curDiv + "' " );
					curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
					curDataTable = getData( curSqlStmt.ToString() );
					if ( curDataTable.Rows.Count == 1 ) {
						returnMemberId = (String) curDataTable.Rows[0]["MemberId"];

					} else {
						returnMemberId = "";
						myImportMsg.Append( "\n* Not Registered: "
						+ String.Format( "Skier {0} Div {1} for {2} Event"
							, curSkierName, curDiv, curEvent ) );
					}

				} else {
					// Search skier name chechking to see if name was input as FirstName LastName (separated by space)
					curNameNodes = curSkierName.Split( ' ' );
					curSqlStmt = new StringBuilder( "" );
					curSqlStmt.Append( "SELECT ER.SanctionId, ER.MemberId, ER.AgeGroup, ER.EventGroup, ER.Event, ER.TeamCode, TR.SkierName " );
					curSqlStmt.Append( "FROM EventReg ER " );
					curSqlStmt.Append( "INNER JOIN TourReg TR ON ER.SanctionId = TR.SanctionId AND ER.MemberId = TR.MemberId AND ER.AgeGroup = TR.AgeGroup " );
					curSqlStmt.Append( "WHERE ER.SanctionId = '" + mySanctionNum + "' " );
					curSqlStmt.Append( "AND TR.SkierName like '" + curNameNodes[1] + "%' " );
					curSqlStmt.Append( "AND TR.SkierName like '%" + curNameNodes[0] + "%' " );
					curSqlStmt.Append( "AND ER.AgeGroup = '" + curDiv + "' " );
					curSqlStmt.Append( "AND Event = '" + curEvent + "' " );
					curDataTable = getData( curSqlStmt.ToString() );
					if ( curDataTable.Rows.Count == 1 ) {
						returnMemberId = (String) curDataTable.Rows[0]["MemberId"];

					} else {
						returnMemberId = "";
						myImportMsg.Append( "\n* Not Registered: "
						+ String.Format( "Skier {0} Div {1} for {2} Event"
							, curSkierName, curDiv, curEvent ) );
					}
				}
			}

			return returnMemberId;
		}

		private void importTeamData( String[] inputCols ) {
			StringBuilder curSqlStmt = new StringBuilder( "" );
			String curTeamCode = "";
			int rowsProc = 0;

			curTeamCode = inputCols[myTeamImportIdxTeamCode];

			curSqlStmt = new StringBuilder( "" );
			curSqlStmt.Append( "Select * from TeamList " );
			curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' And TeamCode = '" + inputCols[myTeamImportIdxTeamCode] + "'" );
			DataTable curDataTable = getData( curSqlStmt.ToString() );
			if ( curDataTable.Rows.Count > 0 ) {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Update TeamList Set Name = '" + inputCols[myTeamImportIdxTeamName] + "' " );
				curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' And TeamCode = '" + inputCols[myTeamImportIdxTeamCode] + "'" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

			} else {
				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert TeamList (" );
				curSqlStmt.Append( "SanctionId, TeamCode, Name, LastUpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + mySanctionNum + "' " );
				curSqlStmt.Append( ", '" + inputCols[myTeamImportIdxTeamCode] + "' " );
				if ( inputCols[myTeamImportIdxTeamName].Length > 0 ) {
					curSqlStmt.Append( ", '" + inputCols[myTeamImportIdxTeamName] + "' " );
				} else {
					curSqlStmt.Append( ", '" + inputCols[myTeamImportIdxTeamCode] + "' " );
				}
				curSqlStmt.Append( ", getdate()" );
				curSqlStmt.Append( ")" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );

				curSqlStmt = new StringBuilder( "" );
				curSqlStmt.Append( "Insert TeamOrder (" );
				curSqlStmt.Append( "SanctionId, TeamCode, AgeGroup, SlalomRunOrder, TrickRunOrder, JumpRunOrder, Notes, LastUpdateDate" );
				curSqlStmt.Append( ") Values (" );
				curSqlStmt.Append( "'" + mySanctionNum + "' " );
				curSqlStmt.Append( ", '" + inputCols[myTeamImportIdxTeamCode] + "', '' " );
				curSqlStmt.Append( ", 1, 1, 1, 'Import', getdate()" );
				curSqlStmt.Append( ")" );
				rowsProc = DataAccess.ExecuteCommand( curSqlStmt.ToString() );
			}

		}

		private StreamReader getImportFile() {
            String curFileName = "", curPath = "";
            OpenFileDialog myFileDialog = new OpenFileDialog();
            StreamReader myReader = null;
            try {
                curPath = Properties.Settings.Default.ExportDirectory;
                if ( curPath.Length < 2 ) {
                    curPath = Directory.GetCurrentDirectory();
                }
                myFileDialog.InitialDirectory = curPath;
                myFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                myFileDialog.FilterIndex = 2;
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    curFileName = myFileDialog.FileName;
                    if ( curFileName != null ) {
                        myReader = new StreamReader(curFileName);                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show("Error: Could not get an import file to process " + "\n\nError: " + ex.Message);
            }

            return myReader;
        }

    }
}
