using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlServerCe;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class SafetyCheckList : Form {
        #region Instance Variables
        private String mySanctionNum = null;
        private String myTourRules;
        private bool isDataModified;
        private bool myDataValid = false;
        private Int16 mySlalomRounds;
        private Int16 myTrickRounds;
        private Int16 myJumpRounds;
        private DataRow myTourRow;
        private DataRow mySafetyCheckList;

        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;
        #endregion 

        public SafetyCheckList() {
            InitializeComponent();
        }

        private void SafetyCheckList_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.SafetyCheckList_Width > 0 ) {
                this.Width = Properties.Settings.Default.SafetyCheckList_Width;
            }
            if ( Properties.Settings.Default.SafetyCheckList_Height > 0 ) {
                this.Height = Properties.Settings.Default.SafetyCheckList_Height;
            }
            if ( Properties.Settings.Default.SafetyCheckList_Location.X > 0
                && Properties.Settings.Default.SafetyCheckList_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.SafetyCheckList_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if ( mySanctionNum == null ) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if ( mySanctionNum.Length < 6 ) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
                    myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

                    DataTable curTourDataTable = getTourData();
                    if ( curTourDataTable.Rows.Count > 0 ) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];

                        RefreshButton_Click( null, null );
                        isDataModified = false;
                    }
                }
            }
        }

        private void SafetyCheckList_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.SafetyCheckList_Width = this.Size.Width;
                Properties.Settings.Default.SafetyCheckList_Height = this.Size.Height;
                Properties.Settings.Default.SafetyCheckList_Location = this.Location;
            }
        }

        private void SafetyCheckList_FormClosing( object sender, FormClosingEventArgs e ) {
            if ( isDataModified ) {
                SaveButton_Click( null, null );
            }
        }

        private void SaveButton_Click( object sender, EventArgs e ) {
            String curMethodName = "Tournament:SafetyCheckList:saveButton_Click";
            bool curReturn = true;
            int rowsProc = 0;
            StringBuilder curSqlStmt = null;

            try {
                curSqlStmt = new StringBuilder( "Select PK From SafetycheckList WHERE SanctionId = '" + mySanctionNum + "' " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );

                try {
                    myDbConn.Open();
                    mySqlStmt = myDbConn.CreateCommand();
                    mySqlStmt.CommandText = "";

                    if ( curDataTable.Rows.Count > 0 ) {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Update SafetycheckList Set " );
                        curSqlStmt.Append( "SponsorClubName = '" + sponsorClubNameTextBox.Text + "' " );
                        curSqlStmt.Append( ", NumInjuries = '" + numInjuriesTextBox.Text + "' " );
                        curSqlStmt.Append( ", CommAvail = '" + commAvailTextBox.Text + "' " );
                        curSqlStmt.Append( ", MedAccptCheck = '" + medAccptCheckTextBox.Text + "' " );
                        curSqlStmt.Append( ", MedAvail = '" + medAvailTextBox.Text + "' " );
                        curSqlStmt.Append( ", PostedMedRoute = '" + postedMedRouteTextBox.Text + "' " );
                        curSqlStmt.Append( ", HazFree = '" + hazFreeTextBox.Text + "' " );
                        curSqlStmt.Append( ", ObstructionsMarked = '" + obstructionsMarkedTextBox.Text + "' " );
                        curSqlStmt.Append( ", LandingClear = '" + landingClearTextBox.Text + "' " );
                        curSqlStmt.Append( ", DockChecked = '" + dockCheckedTextBox.Text + "' " );
                        curSqlStmt.Append( ", JumpInspect = '" + jumpInspectTextBox.Text + "' " );
                        curSqlStmt.Append( ", JumpSecure = '" + jumpSecureTextBox.Text + "' " );
                        curSqlStmt.Append( ", JumpSurfaceSafe = '" + jumpSurfaceSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", JumpColor = '" + jumpColorTextBox.Text + "' " );
                        curSqlStmt.Append( ", JumpAlgaeRemoved = '" + jumpAlgaeRemovedTextBox.Text + "' " );
                        curSqlStmt.Append( ", CourseSafeDist = '" + courseSafeDistTextBox.Text + "' " );
                        curSqlStmt.Append( ", TowerStable = '" + towerStableTextBox.Text + "' " );
                        curSqlStmt.Append( ", TowerLadderSafe = '" + towerLadderSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", TowerFloorSafe = '" + towerFloorSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", RefuelFireExtn = '" + refuelFireExtnTextBox.Text + "' " );
                        curSqlStmt.Append( ", RefuelSignsPosted = '" + refuelSignsPostedTextBox.Text + "' " );
                        curSqlStmt.Append( ", RefuelGrounded = '" + refuelGroundedTextBox.Text + "' " );
                        curSqlStmt.Append( ", SafetyPfd = '" + safetyPfdTextBox.Text + "' " );
                        curSqlStmt.Append( ", SafetyRadio = '" + safetyRadioTextBox.Text + "' " );
                        curSqlStmt.Append( ", SafetyVolunteers = '" + safetyVolunteersTextBox.Text + "' " );
                        curSqlStmt.Append( ", SafetyBoats = '" + safetyBoatsTextBox.Text + "' " );
                        curSqlStmt.Append( ", FirstAidArea = '" + firstAidAreaTextBox.Text + "' " );
                        curSqlStmt.Append( ", SpineBoard = '" + spineBoardTextBox.Text + "' " );
                        curSqlStmt.Append( ", SafetyCid = '" + safetyCidTextBox.Text + "' " );
                        curSqlStmt.Append( ", FirstAidKit = '" + firstAidKitTextBox.Text + "' " );
                        curSqlStmt.Append( ", LastUpdateDate = GETDATE() " );
                        curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "' " );
                        mySqlStmt.CommandText = curSqlStmt.ToString();
                    } else {
                        curSqlStmt = new StringBuilder( "" );
                        curSqlStmt.Append( "Insert SafetycheckList ( " );
                        curSqlStmt.Append( "SanctionId, SponsorClubName, NumInjuries, MedAccptCheck" );
                        curSqlStmt.Append( ", CommAvail, MedAvail, PostedMedRoute, HazFree, ObstructionsMarked" );
                        curSqlStmt.Append( ", LandingClear, DockChecked, JumpInspect, JumpSecure, JumpSurfaceSafe, JumpColor, JumpAlgaeRemoved" );
                        curSqlStmt.Append( ", CourseSafeDist, TowerStable, TowerLadderSafe, TowerFloorSafe, RefuelFireExtn, RefuelSignsPosted, RefuelGrounded" );
                        curSqlStmt.Append( ", SafetyPfd, SafetyRadio, SafetyVolunteers, SafetyBoats, FirstAidArea, SpineBoard, SafetyCid, FirstAidKit, LastUpdateDate " );
                        curSqlStmt.Append( ") Values ( " );
                        curSqlStmt.Append( "'" + mySanctionNum + "'" );
                        curSqlStmt.Append( ", '" + sponsorClubNameTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + numInjuriesTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + medAccptCheckTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + commAvailTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + medAvailTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + postedMedRouteTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + hazFreeTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + obstructionsMarkedTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + landingClearTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + dockCheckedTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + jumpInspectTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + jumpSecureTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + jumpSurfaceSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + jumpColorTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + jumpAlgaeRemovedTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + courseSafeDistTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + towerStableTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + towerLadderSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + towerFloorSafeTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + refuelFireExtnTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + refuelSignsPostedTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + refuelGroundedTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + safetyPfdTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + safetyRadioTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + safetyVolunteersTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + safetyBoatsTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + firstAidAreaTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + spineBoardTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + safetyCidTextBox.Text + "' " );
                        curSqlStmt.Append( ", '" + firstAidKitTextBox.Text + "' " );
                        curSqlStmt.Append( ", GETDATE() )" );
                        mySqlStmt.CommandText = curSqlStmt.ToString();
                    }
                    rowsProc = mySqlStmt.ExecuteNonQuery();
                    if ( rowsProc > 0 ) {
                        isDataModified = false;
                    }
                } catch ( Exception excp ) {
                    curReturn = false;
                    String curMsg = ":Error attempting to update SafetycheckList information \n" + excp.Message;
                    MessageBox.Show( curMsg );
                } finally {
                    myDbConn.Close();
                }
            } catch ( Exception excp ) {
                curReturn = false;
                String curMsg = ":Error attempting to update SafetycheckList information \n" + excp.Message;
                MessageBox.Show( curMsg );
            }
            
        }

        public void ShowTour( String inSanctionId ) {
            mySanctionNum = inSanctionId;
            RefreshButton_Click( null, null );
        }

        public void RefreshButton_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( mySanctionNum != null ) {
                if ( isDataModified ) {
                    SaveButton_Click( null, null );
                }
                Cursor.Current = Cursors.WaitCursor;
                isDataModified = false;
                regionTextBox.Text = mySanctionNum.Substring( 2, 1 );
                setTourData();
                setSafetyData();
                setTourStats();
                validateData();
            }
        }

        public void ExportButton_Click( object sender, EventArgs e ) {
            if ( mySanctionNum != null && myTourRow != null ) {
                //ExportReportTxt();
                //ExportReportCsv(); This version is not currently being used.
                ExportReportTxtV2();
            }
        }

        private void setTourData() {
            CheckBox curCheckBox;
            String curValue;

            sanctionIdTextBox.Text = mySanctionNum;
            nameTextBox.Text = (String)myTourRow["Name"];
            classTextBox.Text = (String)myTourRow["EventScoreClass"];
            eventDatesTextBox.Text = (String)myTourRow["EventDates"];
            eventLocationTextBox.Text = (String)myTourRow["EventLocation"];
            regionTextBox.Text = mySanctionNum.Substring( 2, 1 );

            if ( myTourRow["ChiefJudgeName"] != System.DBNull.Value ) {
                curValue = (String)myTourRow["ChiefJudgeName"];
                int curDelim = curValue.IndexOf( ',' );
                ChiefJudgeSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                ChiefJudgeNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                ChiefJudgeDateTextBox.Text = DateTime.Today.ToString( "MM/dd/yyyy" );

                try {
                    chiefJudgeAddressTextBox.Text = (String)myTourRow["ChiefJudgeAddress"];
                } catch {
                    chiefJudgeAddressTextBox.Text = "";
                }
                try {
                    chiefJudgePhoneTextBox.Text = (String)myTourRow["ChiefJudgePhone"];
                } catch {
                    chiefJudgePhoneTextBox.Text = "";
                }
                try {
                    chiefJudgeEmailTextBox.Text = (String)myTourRow["ChiefJudgeEmail"];
                } catch {
                    chiefJudgeEmailTextBox.Text = "";
                }
            }

            if ( myTourRow["SafetyDirName"] != System.DBNull.Value ) {
                curValue = (String)myTourRow["SafetyDirName"];
                int curDelim = curValue.IndexOf( ',' );
                ChiefSafetySigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                ChiefSafetyNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                ChiefSafetyDateTextBox.Text = DateTime.Today.ToString( "MM/dd/yyyy" );
                try {
                    safetyDirAddressTextBox.Text = (String)myTourRow["SafetyDirAddress"];
                } catch {
                    safetyDirAddressTextBox.Text = "";
                }
                try {
                    safetyDirPhoneTextBox.Text = (String)myTourRow["SafetyDirPhone"];
                } catch {
                    safetyDirPhoneTextBox.Text = "";
                }
                try {
                    safetyDirEmailTextBox.Text = (String)myTourRow["SafetyDirEmail"];
                } catch {
                    safetyDirEmailTextBox.Text = "";
                }
            }

            if ( myTourRow["Rules"] != System.DBNull.Value ) {
                curValue = (String)myTourRow["Rules"];
                foreach ( Control curControl in ReportPage1.Controls ) {
                    String curType = (String)curControl.GetType().ToString();
                    if ( curType.IndexOf( "CheckBox" ) > 0 ) {
                        curCheckBox = (CheckBox)curControl;
                        if ( curCheckBox.Tag.Equals( curValue ) ) {
                            curCheckBox.Checked = true;
                        }
                    }
                }
            }
        }

        private void setSafetyData() {
            TextBox curTextBox;
            CheckedListBox curListBox;
            Control[] findControlList;
            String curControlName, curTextBoxName, curDataItemName;

            DataTable curDataTable = getSafetyData();
            if ( curDataTable.Rows.Count > 0 ) {
                DataRow curDataRow = curDataTable.Rows[0];
                try {
                    sponsorClubNameTextBox.Text = (String)curDataRow["SponsorClubName"];
                } catch {
                    sponsorClubNameTextBox.Text = "";
                }
                try {
                    numInjuriesTextBox.Text = ( (Byte)curDataRow["NumInjuries"] ).ToString();
                } catch {
                    numInjuriesTextBox.Text = "";
                }

                foreach ( Control curControl in ReportPage2.Controls ) {
                    String curType = (String)curControl.GetType().ToString();
                    if ( curType.IndexOf( "CheckedListBox" ) > 0 ) {
                        curListBox = (CheckedListBox)curControl;
                        curControlName = curListBox.Name;
                        curDataItemName = curControlName.Substring( 0, curControlName.Length - 3 );
                        curTextBoxName =  curDataItemName + "TextBox";
                        findControlList = Controls.Find( curTextBoxName, true );
                        if ( findControlList.Length > 0 ) {
                            curTextBox = (TextBox)findControlList[0];
                            try {
                                curTextBox.Text = (String)curDataRow[curDataItemName];
                                if ( curTextBox.Text.Equals( "Y" ) ) {
                                    curListBox.Text = "Yes";
                                    curListBox.SetItemChecked( 0, true );
                                } else if ( curTextBox.Text.Equals( "N" ) ) {
                                    curListBox.Text = "No";
                                    curListBox.SetItemChecked( 1, true );
                                } else if ( curTextBox.Text.Equals( "A" ) ) {
                                    curListBox.Text = "N/A";
                                    curListBox.SetItemChecked( 2, true );
                                }
                            } catch {
                                curListBox.Text = "N/A";
                                curListBox.SetItemChecked( 2, true );
                            }
                        }
                    }
                }
            } else {
                mySafetyCheckList = null;
                foreach ( Control curControl in ReportPage2.Controls ) {
                    String curType = (String)curControl.GetType().ToString();
                    if ( curType.IndexOf( "CheckedListBox" ) > 0 ) {
                        curListBox = (CheckedListBox)curControl;
                        curControlName = curListBox.Name;
                        curDataItemName = curControlName.Substring( 0, curControlName.Length - 3 );
                        curTextBoxName = curDataItemName + "TextBox";
                        findControlList = Controls.Find( curTextBoxName, true );
                        if ( findControlList.Length > 0 ) {
                            curTextBox = (TextBox)findControlList[0];
                            curTextBox.Text = "N";
                            if ( curTextBox.Text.Equals( "Y" ) ) {
                                curListBox.Text = "Yes";
                                curListBox.SetItemChecked( 0, true );
                            } else if ( curTextBox.Text.Equals( "N" ) ) {
                                curListBox.Text = "No";
                                curListBox.SetItemChecked( 1, true );
                            } else if ( curTextBox.Text.Equals( "A" ) ) {
                                curListBox.Text = "N/A";
                                curListBox.SetItemChecked( 2, true );
                            }
                        }
                    }
                }
            }
        }

        private void setTourStats() {
            String selectStmt = "";
            DataTable curDataTable;
            DataRow curRow;
            int curNumRides = 0;
            int curSlalomRounds = 0, curTrickRounds = 0, curJumpRounds = 0;

            if ( myTourRow["SlalomRounds"] != DBNull.Value ) {
                curSlalomRounds = (Byte)myTourRow["SlalomRounds"];
            }
            if ( myTourRow["TrickRounds"] != DBNull.Value ) {
                curTrickRounds = (Byte)myTourRow["TrickRounds"];
            }
            if ( myTourRow["JumpRounds"] != DBNull.Value ) {
                curJumpRounds = (Byte)myTourRow["JumpRounds"];
            }

            if ( curSlalomRounds > 0 ) {
                // Calcualte number of slalom participants
                selectStmt = "Select count(*) as SkierCount "
                    + " From (SELECT DISTINCT MemberId From SlalomScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    SlalomNumTextBox.Text = curRow["SkierCount"].ToString();
                } else {
                    SlalomNumTextBox.Text = "";
                }

                // Calcualte number of slalom rides
                selectStmt = "Select count(*) as RideCount "
                    + " From SlalomScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    SlalomRidesTextBox.Text = curRow["RideCount"].ToString();
                    curNumRides += (int)curRow["RideCount"];
                } else {
                    SlalomRidesTextBox.Text = "";
                }
            } else {
                SlalomNumTextBox.Text = "";
                SlalomRidesTextBox.Text = "";
            }

            if ( curTrickRounds > 0 ) {
                // Calcualte number of trick participants
                selectStmt = "Select count(*) as SkierCount "
                    + " From (SELECT DISTINCT MemberId From TrickScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    TrickNumTextBox.Text = curRow["SkierCount"].ToString();
                } else {
                    TrickNumTextBox.Text = "";
                }

                // Calcualte number of trick rides
                selectStmt = "Select count(*) as RideCount "
                    + " From TrickScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    TrickRidesTextBox.Text = curRow["RideCount"].ToString();
                    curNumRides += (int)curRow["RideCount"];
                } else {
                    TrickRidesTextBox.Text = "";
                }
            } else {
                TrickNumTextBox.Text = "";
                TrickRidesTextBox.Text = "";
            }

            if ( curJumpRounds > 0 ) {
                // Calcualte number of jump participants
                selectStmt = "Select count(*) as SkierCount "
                    + " From (SELECT DISTINCT MemberId From JumpScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')) myTable";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    JumpNumTextBox.Text = curRow["SkierCount"].ToString();
                } else {
                    JumpNumTextBox.Text = "";
                }

                // Calcualte number of jump rides
                selectStmt = "Select count(*) as RideCount "
                    + " From JumpScore "
                    + " WHERE (SanctionId = '" + mySanctionNum + "')";
                curDataTable = getData( selectStmt );
                if ( curDataTable.Rows.Count > 0 ) {
                    curRow = (DataRow)curDataTable.Rows[0];
                    JumpRidesTextBox.Text = curRow["RideCount"].ToString();
                    curNumRides += (int)curRow["RideCount"];
                } else {
                    JumpRidesTextBox.Text = "";
                }
            } else {
                JumpNumTextBox.Text = "";
                JumpRidesTextBox.Text = "";
            }

            // Calcualte total number of ski rides
            TotalRidesTextBox.Text = curNumRides.ToString();

            // Calcualte total number of participants
            selectStmt = "Select count(*) as SkierCount "
                + " From ( "
                + " SELECT DISTINCT MemberId From EventReg "
                + " WHERE SanctionId = '" + mySanctionNum + "' "
                + "   AND ("
                + "      EXISTS (SELECT 1 FROM SlalomScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + "       OR "
                + "       EXISTS (SELECT 1 FROM TrickScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + "       OR "
                + "       EXISTS (SELECT 1 FROM JumpScore "
                + "       WHERE SanctionId = EventReg.SanctionId AND MemberId = EventReg.MemberId AND AgeGroup = EventReg.AgeGroup) "
                + " ) ) myTable";

            curDataTable = getData( selectStmt );
            if ( curDataTable.Rows.Count > 0 ) {
                curRow = (DataRow)curDataTable.Rows[0];
                TotalSkiersTextBox.Text = curRow["SkierCount"].ToString();
                TotalNumTextBox.Text = TotalSkiersTextBox.Text;
            } else {
                TotalSkiersTextBox.Text = "";
                TotalNumTextBox.Text = "";
            }
        }

        private void ItemSelectedValueChanged( object sender, EventArgs e ) {
            CheckedListBox curControl = (CheckedListBox)sender;
            String curControlName = curControl.Name;
            String curTextBoxName = curControlName.Substring( 0, curControlName.Length - 3 ) + "TextBox";
            Control[] curTextBoxList = Controls.Find( curTextBoxName, true );
            if ( curTextBoxList.Length > 0 ) {
                isDataModified = true;
                TextBox curTextBox = (TextBox)curTextBoxList[0];
                if ( curControl.Text.Equals( "Yes" ) ) {
                    curTextBox.Text = "Y";
                    curControl.SetItemChecked( 1, false );
                    curControl.SetItemChecked( 2, false );
                } else if ( curControl.Text.Equals( "No" ) ) {
                    curTextBox.Text = "N";
                    curControl.SetItemChecked( 0, false );
                    curControl.SetItemChecked( 2, false );
                } else if ( curControl.Text.Equals( "N/A" ) ) {
                    curTextBox.Text = "A";
                    curControl.SetItemChecked( 0, false );
                    curControl.SetItemChecked( 1, false );
                } else {
                    curTextBox.Text = "";
                    curControl.SetItemChecked( 0, false );
                    curControl.SetItemChecked( 1, false );
                    curControl.SetItemChecked( 2, false );
                }
            }
        }

        private bool validateData() {
            myDataValid = true;
            StringBuilder curDataValidMsg = new StringBuilder( "" );

            try {
                mySlalomRounds = Convert.ToInt16( myTourRow["SlalomRounds"].ToString() );
            } catch {
                mySlalomRounds = 0;
            }
            try {
                myTrickRounds = Convert.ToInt16( myTourRow["TrickRounds"].ToString() );
            } catch {
                myTrickRounds = 0;
            }
            try {
                myJumpRounds = Convert.ToInt16( myTourRow["JumpRounds"].ToString() );
            } catch {
                myJumpRounds = 0;
            }

            if ( myTourRow["ChiefJudgeName"] == System.DBNull.Value ) {
                myDataValid = false;
                curDataValidMsg.Append( "\n" + "Missing chief judge name" );
            } else {
                if ( ( (String)myTourRow["ChiefJudgeName"] ).Length < 5 ) {
                    myDataValid = false;
                    curDataValidMsg.Append( "\n" + "Missing chief judge name" );
                }
            }

            if ( myTourRow["SafetyDirName"] == System.DBNull.Value ) {
                myDataValid = false;
                curDataValidMsg.Append( "\n" + "Missing chief safety name" );
            } else {
                if ( ( (String)myTourRow["SafetyDirName"] ).Length < 5 ) {
                    myDataValid = false;
                    curDataValidMsg.Append( "\n" + "Missing chief safety name" );
                }
            }

            if ( !( myDataValid ) ) {
                MessageBox.Show( "Missing or invalid data detected.  You must correct the data before the report can be produced."
                    + "\n" + curDataValidMsg.ToString() );
            }

            return myDataValid;
        }

        private void ExportReportTxt() {
            String curMethodName = "SafetyCheckList:ExportReportTxt";
            //char[] tabDelim = new char[] { '\t' };
            //char[] singleQuoteDelim = new char[] { '\'' };
            String tabDelim = "\t";
            String curValue = "";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = sanctionIdTextBox.Text + "SD" + ".txt";
            String curReportTitle = "safety check list Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if ( outBuffer != null ) {
                try {
                    Log.WriteFile( "Export safety check list data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType");
                    outLine.Append( tabDelim + "SanctionNumber");
                    outLine.Append( tabDelim + "name");
                    outLine.Append( tabDelim + "eventLocation");
                    outLine.Append( tabDelim + "class");
                    outLine.Append( tabDelim + "eventDates");
                    outLine.Append( tabDelim + "sponsorClubName");
                    outLine.Append( tabDelim + "rules");
                    outLine.Append( tabDelim + "Region");
                    outLine.Append( tabDelim + "ChiefJudgeName");
                    outLine.Append( tabDelim + "ChiefJudgeAddress");
                    outLine.Append( tabDelim + "ChiefJudgePhone");
                    outLine.Append( tabDelim + "ChiefJudgeEmail");
                    outLine.Append( tabDelim + "ChiefSafetyName");
                    outLine.Append( tabDelim + "SafetyDirAddress");
                    outLine.Append( tabDelim + "SafetyDirPhone");
                    outLine.Append( tabDelim + "SafetyDirEmail");
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "Tournament");
                    outLine.Append( tabDelim + sanctionIdTextBox.Text + myTourRow["Class"]);
                    outLine.Append( tabDelim + nameTextBox.Text);
                    outLine.Append( tabDelim + eventLocationTextBox.Text);
                    outLine.Append( tabDelim + classTextBox.Text);
                    outLine.Append( tabDelim + eventDatesTextBox.Text);
                    outLine.Append( tabDelim + sponsorClubNameTextBox.Text);
                    outLine.Append( tabDelim + myTourRow["Rules"].ToString());
                    outLine.Append( tabDelim + regionTextBox.Text);
                    outLine.Append( tabDelim + ChiefJudgeNameTextBox.Text);
                    outLine.Append( tabDelim + chiefJudgeAddressTextBox.Text);
                    outLine.Append( tabDelim + chiefJudgePhoneTextBox.Text);
                    outLine.Append( tabDelim + chiefJudgeEmailTextBox.Text);
                    outLine.Append( tabDelim + ChiefSafetyNameTextBox.Text);
                    outLine.Append( tabDelim + safetyDirAddressTextBox.Text);
                    outLine.Append( tabDelim + safetyDirPhoneTextBox.Text);
                    outLine.Append( tabDelim + safetyDirEmailTextBox.Text);
                    outBuffer.WriteLine( outLine.ToString() );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" );
                    outLine.Append( tabDelim + "SlalomNum" );
                    outLine.Append( tabDelim + "SlalomRides" );
                    outLine.Append( tabDelim + "TrickNum");
                    outLine.Append( tabDelim + "TrickRides");
                    outLine.Append( tabDelim + "JumpNum");
                    outLine.Append( tabDelim + "JumpRides");
                    outLine.Append( tabDelim + "TotalNum");
                    outLine.Append( tabDelim + "TotalRides");
                    outLine.Append( tabDelim + "TotalSkiers");
                    outLine.Append( tabDelim + "numInjuries");
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "EventStats");
                    outLine.Append( tabDelim + SlalomNumTextBox.Text);
                    outLine.Append( tabDelim + SlalomRidesTextBox.Text);
                    outLine.Append( tabDelim + TrickNumTextBox.Text);
                    outLine.Append( tabDelim + TrickRidesTextBox.Text);
                    outLine.Append( tabDelim + JumpNumTextBox.Text);
                    outLine.Append( tabDelim + JumpRidesTextBox.Text);
                    outLine.Append( tabDelim + TotalNumTextBox.Text);
                    outLine.Append( tabDelim + TotalRidesTextBox.Text);
                    outLine.Append( tabDelim + TotalSkiersTextBox.Text);
                    outLine.Append( tabDelim + numInjuriesTextBox.Text);
                    outBuffer.WriteLine( outLine.ToString() );

                    if ( mySafetyCheckList != null ) {
                        TextBox curTextBox;
                        CheckedListBox curListBox;
                        Control[] findControlList;
                        String curControlName, curTextBoxName, curType;
                        StringBuilder outLineHeader = new StringBuilder( "DataType" );
                        StringBuilder outLineData = new StringBuilder( "SafetyCheckList" );

                        foreach ( Control curControl in ReportPage2.Controls ) {
                            curType = (String)curControl.GetType().ToString();
                            if ( curType.IndexOf( "CheckedListBox" ) > 0 ) {
                                curListBox = (CheckedListBox)curControl;
                                curControlName = curListBox.Name;
                                curTextBoxName = curControlName.Substring( 0, curControlName.Length - 3 ) + "TextBox";
                                findControlList = Controls.Find( curTextBoxName, true );
                                if ( findControlList.Length > 0 ) {
                                    curTextBox = (TextBox)findControlList[0];
                                    outLineHeader.Append( tabDelim + curControlName.Substring( 0, curControlName.Length - 3 ) );
                                    outLineData.Append( tabDelim + curTextBox.Text );
                                }
                            }
                        }
                        outBuffer.WriteLine( outLineHeader.ToString() );
                        outBuffer.WriteLine( outLineData.ToString() );

                    }
                    Log.WriteFile( "Export safety check list data file complete: " + curFilename );

                } catch ( Exception ex ) {
                    MessageBox.Show( "Error: Failure detected writing safety check list Extract file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        private void ExportReportTxtV2() {
            String curMethodName = "SafetyCheckList:ExportReportTxtV2";
            //char[] tabDelim = new char[] { '\t' };
            //char[] singleQuoteDelim = new char[] { '\'' };
            String tabDelim = "\t";
            String curValue = "";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = sanctionIdTextBox.Text + "SD" + ".txt";
            String curReportTitle = "safety check list Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if (outBuffer != null) {
                try {
                    Log.WriteFile( "Export safety check list data file begin: " + curFilename );

                    //if (mySafetyCheckList != null) {
                    //}
                    TextBox curTextBox;
                    CheckedListBox curListBox;
                    Control[] findControlList;
                    String curControlName, curTextBoxName, curType;
                    StringBuilder outLineHeader = new StringBuilder( "" );
                    StringBuilder outLineData = new StringBuilder( "" );

                    foreach (Control curControl in ReportPage2.Controls) {
                        curType = (String)curControl.GetType().ToString();
                        if (curType.IndexOf( "CheckedListBox" ) > 0) {
                            curListBox = (CheckedListBox)curControl;
                            curControlName = curListBox.Name;
                            curTextBoxName = curControlName.Substring( 0, curControlName.Length - 3 ) + "TextBox";
                            findControlList = Controls.Find( curTextBoxName, true );
                            if (findControlList.Length > 0) {
                                curTextBox = (TextBox)findControlList[0];
                                outLineHeader.Append( tabDelim + curControlName.Substring( 0, curControlName.Length - 3 ) );
                                outLineData.Append( tabDelim + curTextBox.Text );
                            }
                        }
                    }

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "SanctionNumber" );
                    outLine.Append( tabDelim + "name" );
                    //outLine.Append( tabDelim + "eventLocation" );
                    //outLine.Append( tabDelim + "class" );
                    //outLine.Append( tabDelim + "eventDates" );
                    outLine.Append( tabDelim + "sponsorClubName" );
                    //outLine.Append( tabDelim + "rules" );
                    //outLine.Append( tabDelim + "Region" );
                    //outLine.Append( tabDelim + "ChiefJudgeName" );
                    //outLine.Append( tabDelim + "ChiefJudgeAddress" );
                    //outLine.Append( tabDelim + "ChiefJudgePhone" );
                    //outLine.Append( tabDelim + "ChiefJudgeEmail" );
                    //outLine.Append( tabDelim + "ChiefSafetyName" );
                    //outLine.Append( tabDelim + "SafetyDirAddress" );
                    //outLine.Append( tabDelim + "SafetyDirPhone" );
                    //outLine.Append( tabDelim + "SafetyDirEmail" );
                    
                    outLine.Append( outLineHeader.ToString() );

                    //EventStats
                    outLine.Append( tabDelim + "SlalomNum" );
                    outLine.Append( tabDelim + "SlalomRides" );
                    outLine.Append( tabDelim + "TrickNum" );
                    outLine.Append( tabDelim + "TrickRides" );
                    outLine.Append( tabDelim + "JumpNum" );
                    outLine.Append( tabDelim + "JumpRides" );
                    outLine.Append( tabDelim + "TotalNum" );
                    outLine.Append( tabDelim + "TotalRides" );
                    outLine.Append( tabDelim + "TotalSkiers" );
                    outLine.Append( tabDelim + "numInjuries" );

                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( sanctionIdTextBox.Text.Substring( 0, 6 ) );
                    outLine.Append( tabDelim + nameTextBox.Text );
                    //outLine.Append( tabDelim + eventLocationTextBox.Text );
                    //outLine.Append( tabDelim + classTextBox.Text );
                    //outLine.Append( tabDelim + eventDatesTextBox.Text );
                    outLine.Append( tabDelim + sponsorClubNameTextBox.Text );
                    //outLine.Append( tabDelim + myTourRow["Rules"].ToString() );
                    //outLine.Append( tabDelim + regionTextBox.Text );
                    //outLine.Append( tabDelim + ChiefJudgeNameTextBox.Text );
                    //outLine.Append( tabDelim + chiefJudgeAddressTextBox.Text );
                    //outLine.Append( tabDelim + chiefJudgePhoneTextBox.Text );
                    //outLine.Append( tabDelim + chiefJudgeEmailTextBox.Text );
                    //outLine.Append( tabDelim + ChiefSafetyNameTextBox.Text );
                    //outLine.Append( tabDelim + safetyDirAddressTextBox.Text );
                    //outLine.Append( tabDelim + safetyDirPhoneTextBox.Text );
                    //outLine.Append( tabDelim + safetyDirEmailTextBox.Text );

                    outLine.Append( outLineData.ToString() );

                    //EventStats
                    outLine.Append( tabDelim + SlalomNumTextBox.Text );
                    outLine.Append( tabDelim + SlalomRidesTextBox.Text );
                    outLine.Append( tabDelim + TrickNumTextBox.Text );
                    outLine.Append( tabDelim + TrickRidesTextBox.Text );
                    outLine.Append( tabDelim + JumpNumTextBox.Text );
                    outLine.Append( tabDelim + JumpRidesTextBox.Text );
                    outLine.Append( tabDelim + TotalNumTextBox.Text );
                    outLine.Append( tabDelim + TotalRidesTextBox.Text );
                    outLine.Append( tabDelim + TotalSkiersTextBox.Text );
                    outLine.Append( tabDelim + numInjuriesTextBox.Text );
                    
                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export safety check list data file complete: " + curFilename );

                } catch (Exception ex) {
                    MessageBox.Show( "Error: Failure detected writing safety check list Extract file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        private void ExportReportCsv() {
            String curMethodName = "SafetyCheckList:ExportReportCsv";
            //char[] tabDelim = new char[] { '\t' };
            //char[] singleQuoteDelim = new char[] { '\'' };
            String tabDelim = ",";
            String curValue = "";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = sanctionIdTextBox.Text + "SD" + ".csv";
            String curReportTitle = "safety check list Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            outBuffer = getExportFile( curFilename );
            if ( outBuffer != null ) {
                try {
                    Log.WriteFile( "Export safety check list data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" );
                    outLine.Append( tabDelim + "SanctionNumber" );
                    outLine.Append( tabDelim + "name" );
                    outLine.Append( tabDelim + "eventLocation" );
                    outLine.Append( tabDelim + "class" );
                    outLine.Append( tabDelim + "eventDates" );
                    outLine.Append( tabDelim + "sponsorClubName" );
                    outLine.Append( tabDelim + "rules" );
                    outLine.Append( tabDelim + "Region" );
                    outLine.Append( tabDelim + "ChiefJudgeName" );
                    outLine.Append( tabDelim + "ChiefJudgeAddress" );
                    outLine.Append( tabDelim + "ChiefJudgePhone" );
                    outLine.Append( tabDelim + "ChiefJudgeEmail" );
                    outLine.Append( tabDelim + "ChiefSafetyName" );
                    outLine.Append( tabDelim + "SafetyDirAddress" );
                    outLine.Append( tabDelim + "SafetyDirPhone" );
                    outLine.Append( tabDelim + "SafetyDirEmail" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "Tournament" );
                    outLine.Append( tabDelim + sanctionIdTextBox.Text + myTourRow["Class"] );
                    outLine.Append( tabDelim + encodeSpecialChar( nameTextBox.Text ) );
                    outLine.Append( tabDelim + encodeSpecialChar( eventLocationTextBox.Text ) );
                    outLine.Append( tabDelim + classTextBox.Text );
                    outLine.Append( tabDelim + encodeSpecialChar( eventDatesTextBox.Text ) );
                    outLine.Append( tabDelim + encodeSpecialChar( sponsorClubNameTextBox.Text ) );
                    outLine.Append( tabDelim + myTourRow["Rules"].ToString() );
                    outLine.Append( tabDelim + regionTextBox.Text );
                    outLine.Append( tabDelim + encodeSpecialChar( ChiefJudgeNameTextBox.Text ) );
                    outLine.Append( tabDelim + encodeSpecialChar( chiefJudgeAddressTextBox.Text ) );
                    outLine.Append( tabDelim + chiefJudgePhoneTextBox.Text );
                    outLine.Append( tabDelim + chiefJudgeEmailTextBox.Text );
                    outLine.Append( tabDelim + encodeSpecialChar( ChiefSafetyNameTextBox.Text ) );
                    outLine.Append( tabDelim + encodeSpecialChar( safetyDirAddressTextBox.Text ) );
                    outLine.Append( tabDelim + safetyDirPhoneTextBox.Text );
                    outLine.Append( tabDelim + safetyDirEmailTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( "DataType" );
                    outLine.Append( tabDelim + "SlalomNum" );
                    outLine.Append( tabDelim + "SlalomRides" );
                    outLine.Append( tabDelim + "TrickNum" );
                    outLine.Append( tabDelim + "TrickRides" );
                    outLine.Append( tabDelim + "JumpNum" );
                    outLine.Append( tabDelim + "JumpRides" );
                    outLine.Append( tabDelim + "TotalNum" );
                    outLine.Append( tabDelim + "TotalRides" );
                    outLine.Append( tabDelim + "TotalSkiers" );
                    outLine.Append( tabDelim + "numInjuries" );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( "EventStats" );
                    outLine.Append( tabDelim + SlalomNumTextBox.Text );
                    outLine.Append( tabDelim + SlalomRidesTextBox.Text );
                    outLine.Append( tabDelim + TrickNumTextBox.Text );
                    outLine.Append( tabDelim + TrickRidesTextBox.Text );
                    outLine.Append( tabDelim + JumpNumTextBox.Text );
                    outLine.Append( tabDelim + JumpRidesTextBox.Text );
                    outLine.Append( tabDelim + TotalNumTextBox.Text );
                    outLine.Append( tabDelim + TotalRidesTextBox.Text );
                    outLine.Append( tabDelim + TotalSkiersTextBox.Text );
                    outLine.Append( tabDelim + numInjuriesTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    if ( mySafetyCheckList != null ) {
                        TextBox curTextBox;
                        CheckedListBox curListBox;
                        Control[] findControlList;
                        String curControlName, curTextBoxName, curType;
                        StringBuilder outLineHeader = new StringBuilder( "DataType" );
                        StringBuilder outLineData = new StringBuilder( "SafetyCheckList" );

                        foreach ( Control curControl in ReportPage2.Controls ) {
                            curType = (String)curControl.GetType().ToString();
                            if ( curType.IndexOf( "CheckedListBox" ) > 0 ) {
                                curListBox = (CheckedListBox)curControl;
                                curControlName = curListBox.Name;
                                curTextBoxName = curControlName.Substring( 0, curControlName.Length - 3 ) + "TextBox";
                                findControlList = Controls.Find( curTextBoxName, true );
                                if ( findControlList.Length > 0 ) {
                                    curTextBox = (TextBox)findControlList[0];
                                    outLineHeader.Append( tabDelim + curControlName.Substring( 0, curControlName.Length - 3 ) );
                                    outLineData.Append( tabDelim + curTextBox.Text );
                                }
                            }
                        }
                        outBuffer.WriteLine( outLineHeader.ToString() );
                        outBuffer.WriteLine( outLineData.ToString() );

                    }
                    Log.WriteFile( "Export safety check list data file complete: " + curFilename );

                } catch ( Exception ex ) {
                    MessageBox.Show( "Error: Failure detected writing safety check list Extract file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        public void ExportReportPrintFile() {
            String curMethodName = "SafetyCheckList:ExportReportPrintFile";
            String curValue = "";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = sanctionIdTextBox.Text + "SD" + ".prn";
            outBuffer = getExportFile( curFilename );
            if (outBuffer != null) {
                try {
                    Log.WriteFile( "Export safety check list data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Safety check list for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text);
                    outLine.Append( Environment.NewLine + InstructionLabel4.Text );
                    outLine.Append( Environment.NewLine + InstructionLabel6.Text );
                    outLine.Append( Environment.NewLine + InstructionLabel8.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + nameLabel.Text + " " + nameTextBox.Text );
                    outLine.Append( "  " + sanctionIdLabel.Text + " " + sanctionIdTextBox.Text.Substring( 0, 6 ) );
                    outLine.Append( "  " + classLabel + " " + classTextBox.Text );
                    outLine.Append( "  Federation: " + myTourRow["Federation"] );
                    outLine.Append( "  Rules: " + myTourRow["Rules"] );
                    outLine.Append( Environment.NewLine + eventDatesLabel.Text + " " + eventDatesTextBox.Text );
                    outLine.Append( "  " + eventLocationLabel.Text + " " + eventLocationTextBox.Text );
                    outLine.Append( Environment.NewLine + sponsorClubNameLabel.Text + " " + sponsorClubNameTextBox.Text );
                    outLine.Append( "  " + regionLabel.Text + " " + regionTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Judge
                    outLine.Append( Environment.NewLine + chiefJudgeNameLabel.Text + " " + ChiefJudgeNameTextBox.Text );
                    curValue = (String)myTourRow["ChiefJudgeMemberId"];
                    DataRow curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["SafetyOfficialRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["SafetyOfficialRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + chiefJudgeAddressLabel.Text + " " + chiefJudgeAddressTextBox.Text );
                    outLine.Append( Environment.NewLine + chiefJudgePhoneLabel.Text + " " + chiefJudgePhoneTextBox.Text );
                    outLine.Append( "  " + chiefJudgeEmailLabel.Text + " " + chiefJudgeEmailTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Safety Director
                    outLine.Append( Environment.NewLine + "Chief Safety: " + ChiefSafetyNameTextBox.Text );
                    curValue = (String)myTourRow["SafetyDirMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["SafetyOfficialRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["SafetyOfficialRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["SafetyOfficialRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + safetyDirAddressLabel.Text + " " + safetyDirAddressTextBox.Text );
                    outLine.Append( Environment.NewLine + safetyDirPhoneLabel.Text + " " + safetyDirPhoneTextBox.Text );
                    outLine.Append( "  " + safetyDirEmailLabel.Text + " " + safetyDirEmailTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //EventStats
                    outLine.Append( Environment.NewLine + SlalomStatLabel.Text + " Skiers: " + SlalomNumTextBox.Text );
                    outLine.Append( "  Passes: " + SlalomRidesTextBox.Text );
                    outLine.Append( Environment.NewLine + TrickStatLabel.Text + " Skiers: " + TrickNumTextBox.Text );
                    outLine.Append( "  Passes: " + TrickRidesTextBox.Text );
                    outLine.Append( Environment.NewLine + JumpStatLabel.Text + " Skiers: " + JumpNumTextBox.Text );
                    outLine.Append( "  Passes: " + JumpRidesTextBox.Text );
                    outLine.Append( Environment.NewLine + TotalStatLabel.Text + " Skiers: " + TotalNumTextBox.Text );
                    outLine.Append( "  Passes: " + TotalRidesTextBox.Text );
                    outLine.Append( Environment.NewLine + label3.Text + " Skiers: " + TotalSkiersTextBox.Text );
                    outLine.Append( Environment.NewLine + numInjuriesLabel.Text + " " + numInjuriesTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    TextBox curTextBox;
                    CheckedListBox curListBox;
                    Control[] findControlList;
                    String curControlName, curTextBoxName, curType;
                    outLine = new StringBuilder( "" );

                    foreach (Control curControl in ReportPage2.Controls) {
                        curType = (String)curControl.GetType().ToString();
                        if (curType.IndexOf( "CheckedListBox" ) > 0) {
                            curListBox = (CheckedListBox)curControl;
                            curControlName = curListBox.Name;
                            curTextBoxName = curControlName.Substring( 0, curControlName.Length - 3 ) + "TextBox";
                            findControlList = Controls.Find( curTextBoxName, true );
                            if (findControlList.Length > 0) {
                                curTextBox = (TextBox)findControlList[0];
                                outLine.Append( Environment.NewLine + curControlName.Substring( 0, curControlName.Length - 3 ) );
                                outLine.Append( ": " + curTextBox.Text );
                            }
                        }
                    }
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Report created by " );
                    outLine.Append( Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion );
                    outLine.Append( " on " + DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export safety check list print file complete: " + curFilename );
                    MessageBox.Show( "Export safety check list print file complete:" );
                } catch (Exception ex) {
                    MessageBox.Show( "Error: Failure detected writing safety check list data print file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        private String replaceLinefeed(String inValue) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            return curValue;
        }

        private String encodeSpecialChar( String inValue ) {
            String curValue = inValue;
            curValue = curValue.Replace( '\n', ' ' );
            curValue = curValue.Replace( '\r', ' ' );
            curValue = curValue.Replace( '\t', ' ' );
            curValue = curValue.Replace( ",", "%2C" );
            return curValue;
        }

        private DataTable getSafetyData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select PK, SanctionId, SponsorClubName, NumInjuries, MedAccptCheck" );
            curSqlStmt.Append( ", CommAvail, MedAvail, PostedMedRoute, HazFree, ObstructionsMarked" );
            curSqlStmt.Append( ", LandingClear, DockChecked, JumpInspect, JumpSecure, JumpSurfaceSafe, JumpColor, JumpAlgaeRemoved" );
            curSqlStmt.Append( ", CourseSafeDist, TowerStable, TowerLadderSafe, TowerFloorSafe, RefuelFireExtn, RefuelSignsPosted, RefuelGrounded" );
            curSqlStmt.Append( ", SafetyPfd, SafetyRadio, SafetyVolunteers, SafetyBoats, FirstAidArea, SpineBoard, SafetyCid, FirstAidKit, LastUpdateDate " );
            curSqlStmt.Append( "FROM SafetycheckList " );
            curSqlStmt.Append( " WHERE SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getTourData() {
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
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataRow getTourMemberRating(String inMemberId) {
            DataRow curRow = null;
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select Distinct 'OW' as EntityName, TR.SanctionId, TR.MemberId, TR.SkierName" );
            curSqlStmt.Append( ", OW.JudgeSlalomRating, OW.JudgeTrickRating, OW.JudgeJumpRating" );
            curSqlStmt.Append( ", OW.DriverSlalomRating, OW.DriverTrickRating, OW.DriverJumpRating" );
            curSqlStmt.Append( ", OW.ScorerSlalomRating, OW.ScorerTrickRating, OW.ScorerJumpRating" );
            curSqlStmt.Append( ", OW.SafetyOfficialRating, OW.TechOfficialRating, OW.AnncrOfficialRating " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "INNER JOIN OfficialWork OW ON OW.SanctionId = TR.SanctionId AND OW.MemberId = TR.MemberId " );
            curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + inMemberId + "' " );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append( "Select Distinct 'TR' as EntityName, TR.SanctionId, TR.MemberId, TR.SkierName" );
                curSqlStmt.Append( ", JSL.CodeValue AS JudgeSlalomRating, JTL.CodeValue AS JudgeTrickRating, JJL.CodeValue AS JudgeJumpRating" );
                curSqlStmt.Append( ", SSL.CodeValue AS ScorerSlalomRating, STL.CodeValue AS ScorerTrickRating, SJL.CodeValue AS ScorerJumpRating" );
                curSqlStmt.Append( ", DSL.CodeValue AS DriverSlalomRating, DTL.CodeValue AS DriverTrickRating, DJL.CodeValue AS DriverJumpRating" );
                curSqlStmt.Append( ", SL.CodeValue AS SafetyOfficialRating, TL.CodeValue AS TechOfficialRating, AL.CodeValue AS AnncrOfficialRating " );
                curSqlStmt.Append( "FROM TourReg TR " );
                curSqlStmt.Append( "LEFT OUTER JOIN MemberList AS ML ON ML.MemberId = TR.MemberId " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JSL ON ML.JudgeSlalomRating = JSL.ListCode AND JSL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JTL ON ML.JudgeTrickRating = JTL.ListCode AND JTL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS JJL ON ML.JudgeJumpRating = JJL.ListCode AND JJL.ListName = 'JudgeRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SSL ON ML.ScorerSlalomRating = SSL.ListCode AND SSL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS STL ON ML.ScorerTrickRating = STL.ListCode AND STL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SJL ON ML.ScorerJumpRating = SJL.ListCode AND SJL.ListName = 'ScorerRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DSL ON ML.DriverSlalomRating = DSL.ListCode AND DSL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DTL ON ML.DriverTrickRating = DTL.ListCode AND DTL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS DJL ON ML.DriverJumpRating = DJL.ListCode AND DJL.ListName = 'DriverRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS SL ON ML.SafetyOfficialRating = SL.ListCode AND SL.ListName = 'SafetyRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS TL ON ML.TechOfficialRating = TL.ListCode AND TL.ListName = 'TechRating' " );
                curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList AS AL ON ML.AnncrOfficialRating = AL.ListCode AND AL.ListName = 'AnnouncerRating' " );
                curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + inMemberId + "' " );
                curDataTable = getData( curSqlStmt.ToString() );
            }

            if (curDataTable.Rows.Count > 0) {
                curRow = curDataTable.Rows[0];
                return curRow;
            } else {
                return null;
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private StreamWriter getExportFile( String inFileName ) {
            StreamWriter outBuffer = null;

            SaveFileDialog myFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            myFileDialog.InitialDirectory = curPath;
            myFileDialog.FileName = inFileName;

            try {
                if ( myFileDialog.ShowDialog() == DialogResult.OK ) {
                    String myFileName = myFileDialog.FileName;
                    if ( myFileName != null ) {
                        int delimPos = myFileName.LastIndexOf( '\\' );
                        String curFileName = myFileName.Substring( delimPos + 1 );
                        if ( curFileName.IndexOf( '.' ) < 0 ) {
                            myFileName += ".txt";
                        }
                        outBuffer = File.CreateText( myFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
        }

        public void PrintButton_Click( object sender, EventArgs e ) {
            foreach ( TabPage curPage in ReportTabControl.TabPages ) {
                curPage.Select();
                curPage.Focus();
                curPage.Show();
            }
            ReportTabControl.TabPages[0].Select();
            ReportTabControl.TabPages[0].Focus();
            ReportTabControl.TabPages[0].Show();

            FormReportPrinter curFormPrint = new FormReportPrinter( ReportTabControl );
            curFormPrint.ReportName = sanctionIdTextBox.Text + "SD";
            curFormPrint.ReportHeader = "Safety Director's Report for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            curFormPrint.CenterHeaderOnPage = true;
            curFormPrint.ReportHeaderFont = new Font( "Arial", 12, FontStyle.Bold, GraphicsUnit.Point );
            curFormPrint.ReportHeaderTextColor = Color.Black;

            curFormPrint.BottomMargin = 25;
            curFormPrint.TopMargin = 50;
            curFormPrint.LeftMargin = 25;
            curFormPrint.RightMargin = 25;

            curFormPrint.Print(false);
        }

    }
}
