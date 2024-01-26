using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WaterskiScoringSystem.Common;
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tournament {
    public partial class BoatUseReport : Form {
        private String mySanctionNum = null;
        private int myBoatRowIdx;
        private bool myDataValid = false;
        private Int16 mySlalomRounds;
        private Int16 myTrickRounds;
        private Int16 myJumpRounds;
        private DataTable myApprovedManufacturersDataTable;
        private DataTable myBoatUseDataTable;
        private DataRow myTourRow;

        public BoatUseReport() {
            InitializeComponent();
        }

        private void BoatUseReport_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.BoatUseReport_Width > 0 ) {
                this.Width = Properties.Settings.Default.BoatUseReport_Width;
            }
            if ( Properties.Settings.Default.BoatUseReport_Height > 0 ) {
                this.Height = Properties.Settings.Default.BoatUseReport_Height;
            }
            if ( Properties.Settings.Default.BoatUseReport_Location.X > 0
                && Properties.Settings.Default.BoatUseReport_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.BoatUseReport_Location;
            }

            // Retrieve data from database
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;

            RefreshButton_Click( null, null );

        }

        private void BoatUseReport_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.BoatUseReport_Width = this.Size.Width;
                Properties.Settings.Default.BoatUseReport_Height = this.Size.Height;
                Properties.Settings.Default.BoatUseReport_Location = this.Location;
            }
        }

        private void NextPageButton_Click( object sender, EventArgs e ) {
            myBoatRowIdx++;
            if ( myBoatUseDataTable.Rows.Count > myBoatRowIdx ) {
                TowboatUseLoad( myBoatRowIdx );
            } else {
                TowboatUseLoad( 0 );
            }

        }

        public void RefreshButton_Click( object sender, EventArgs e ) {
            // Retrieve data from database
            if ( mySanctionNum != null ) {
                Cursor.Current = Cursors.WaitCursor;

                myApprovedManufacturersDataTable = getApprovedManufacturers();
                
                //Retrieve selected tournament attributes
                DataTable curTourDataTable = getTourData( mySanctionNum );
                if (curTourDataTable != null) {
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRow = curTourDataTable.Rows[0];
                        sanctionIdTextBox.Text = (String)myTourRow["SanctionId"];
                        nameTextBox.Text = (String)myTourRow["Name"];
                        eventLocationTextBox.Text = (String)myTourRow["EventLocation"];
                        classTextBox.Text = (String)myTourRow["Class"];
                        eventDatesTextBox.Text = (String)myTourRow["EventDates"];
                        rulesTextBox.Text = (String)myTourRow["Rules"];
                        RegionTextBox.Text = mySanctionNum.Substring( 2, 1 );
                        String curValue = "";
                        if (!( isObjectEmpty( myTourRow["ChiefJudgeName"] ) )) {
                            curValue = (String)myTourRow["ChiefJudgeName"];
                            int curDelim = curValue.IndexOf( ',' );
                            ChiefJudgeSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                            chiefJudgeNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        }
                        if (!( isObjectEmpty( myTourRow["ChiefDriverName"] ) )) {
                            curValue = (String)myTourRow["ChiefDriverName"];
                            int curDelim = curValue.IndexOf( ',' );
                            ChiefDriverSigTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                            chiefDriverNameTextBox.Text = curValue.Substring( curDelim + 1 ) + " " + curValue.Substring( 0, curDelim );
                        }

                        ChiefJudgeDateTextBox.Text = DateTime.Today.ToString( "MM/dd/yyyy" );
                        ChiefDriverDateTextBox.Text = DateTime.Today.ToString( "MM/dd/yyyy" );
                        getTourBoatUseData();

                        if (myBoatUseDataTable.Rows.Count > 0) {
                            TowboatCreditLoad();
                            TowboatUseLoad( 0 );
                        }
                    } else {
                        MessageBox.Show( "Tournament data not available." );
                    }
                }

                sanctionIdTextBox.BeginInvoke( (MethodInvoker)delegate() {
                    Application.DoEvents();
                    Cursor.Current = Cursors.Default;
                } );
            }
        }

        private void TowboatCreditLoad() {
            String curBoatCode = "", curValue = "";
            String prevBoatCode = "";
            int curCreditPos = 0;
            Control[] curFindControl = null;

            foreach (DataRow curRow in myBoatUseDataTable.Rows) {
                curBoatCode = curRow["HullId"].ToString();
                if (curCreditPos < 5) {
                    if (curBoatCode != prevBoatCode || curCreditPos == 0) {
                        prevBoatCode = curBoatCode;
                        curCreditPos++;

                        curFindControl = this.Controls.Find( "BoatManuTextBox" + curCreditPos.ToString(), true );
                        ( (TextBox)curFindControl[0] ).Text = curRow["BoatModel"].ToString();
                        curFindControl = this.Controls.Find( "modelYearTextBox" + curCreditPos.ToString(), true );
                        ( (TextBox)curFindControl[0] ).Text = curRow["ModelYear"].ToString();
                        curFindControl = this.Controls.Find( "speedControlVersionTextBox" + curCreditPos.ToString(), true );
                        ( (TextBox)curFindControl[0] ).Text = curRow["SpeedControlVersion"].ToString();

                        if (curRow["slalomUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "slalomUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["slalomCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "slalomCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["trickUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "trickUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["trickCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "trickCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["jumpUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "jumpUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["jumpCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "jumpCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                    } else {
                        if (curRow["slalomUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "slalomUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["slalomCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "slalomCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["trickUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "trickUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["trickCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "trickCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["jumpUsed"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "jumpUsedCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                        if (curRow["jumpCredit"].ToString().Equals( "Y" )) {
                            curFindControl = this.Controls.Find( "jumpCreditCheckBox" + curCreditPos.ToString(), true );
                            ( (CheckBox)curFindControl[0] ).Checked = true;
                        }
                    }
                }

            }
        }

        private void TowboatUseLoad( int inStartRowIdx ) {
            Control[] curFindControl = null;
            int curBoatRowIdx = inStartRowIdx;
            for (int curPos = 1; curPos < 6; curPos++ ) {
                if (myBoatUseDataTable.Rows.Count > curBoatRowIdx) {
                    DataRow curRow = myBoatUseDataTable.Rows[curBoatRowIdx];
                    curFindControl = this.Controls.Find( "TowboatManuModelTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["BoatModel"].ToString();
                    curFindControl = this.Controls.Find( "HullIdTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["HullId"].ToString();
                    curFindControl = this.Controls.Find( "OwnerTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["Owner"].ToString();
                    curFindControl = this.Controls.Find( "PreEventNotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["PreEventNotes"].ToString();
                    curFindControl = this.Controls.Find( "PostEventNotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["PostEventNotes"].ToString();
                    curFindControl = this.Controls.Find( "NotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = curRow["Notes"].ToString();
                } else {
                    curFindControl = this.Controls.Find( "TowboatManuModelTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                    curFindControl = this.Controls.Find( "HullIdTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                    curFindControl = this.Controls.Find( "OwnerTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                    curFindControl = this.Controls.Find( "PreEventNotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                    curFindControl = this.Controls.Find( "PostEventNotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                    curFindControl = this.Controls.Find( "NotesTextBox" + curPos.ToString(), true );
                    ( (TextBox)curFindControl[0] ).Text = "";
                }

                curBoatRowIdx++;
            }

            if ( myBoatUseDataTable.Rows.Count > myBoatRowIdx ) {
                RowCountMsg.Text = "Row " + ( inStartRowIdx + 1 ).ToString()
                    + " to " + ( myBoatRowIdx + 1 ).ToString()
                    + " of " + myBoatUseDataTable.Rows.Count.ToString();
            } else {
                RowCountMsg.Text = "Row " + ( inStartRowIdx + 1 ).ToString()
                    + " to " + myBoatUseDataTable.Rows.Count.ToString()
                    + " of " + myBoatUseDataTable.Rows.Count.ToString();
            }
        }

        public void ExportReportPrintFile() {
            String curMethodName = "BoatUserReport:ExportReportPrintFile";
            String curValue = "";

            Cursor.Current = Cursors.WaitCursor;
            StringBuilder outLine = new StringBuilder( "" );
            StreamWriter outBuffer = null;

            String curFilename = mySanctionNum + "TU" + ".prn";
            outBuffer = getExportFile( curFilename );
            if (outBuffer != null) {
                try {
                    Log.WriteFile( "Export boat use data file begin: " + curFilename );

                    //Tournament data
                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Boat use data for " + sanctionIdTextBox.Text + " - " + nameTextBox.Text );
                    outLine.Append( Environment.NewLine + InstructionLabel1.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + nameLabel.Text + " " + nameTextBox.Text );
                    outLine.Append( "  " + sanctionIdLabel.Text + " " + sanctionIdTextBox.Text.Substring( 0, 6 ) );
                    outLine.Append( "  " + classLabel + " " + classTextBox.Text );
                    outLine.Append( "  Federation: " + myTourRow["Federation"] );
                    outLine.Append( "  Rules: " + myTourRow["Rules"] );
                    outLine.Append( Environment.NewLine + eventDatesLabel.Text + " " + eventDatesTextBox.Text );
                    outLine.Append( "  " + eventLocationLabel.Text + " " + eventLocationTextBox.Text );
                    outLine.Append( "  " + RegionLabel.Text + " " + RegionTextBox.Text );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Judge
                    outLine.Append( Environment.NewLine + chiefJudgeNameLabel.Text + " " + chiefJudgeNameTextBox.Text );
                    curValue = (String)myTourRow["ChiefJudgeMemberId"];
                    DataRow curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["JudgeSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["JudgeSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["JudgeSlalomRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["ChiefJudgePhone"] );
                    outLine.Append( "  Email: " + myTourRow["ChiefJudgeEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["ChiefJudgeAddress"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    //Chief Safety Director
                    outLine.Append( Environment.NewLine + chiefDriverNameLabel.Text + " " + chiefDriverNameTextBox.Text );
                    curValue = (String)myTourRow["ChiefDriverMemberId"];
                    curMemberRow = getTourMemberRating( curValue );
                    if (curMemberRow != null) {
                        if (curMemberRow["DriverSlalomRating"] == System.DBNull.Value) {
                            curValue = "";
                        } else if (( (String)curMemberRow["DriverSlalomRating"] ).Equals( "Unrated" )) {
                        } else {
                            curValue = (String)curMemberRow["DriverSlalomRating"];
                        }
                    }
                    outLine.Append( "  Rating: " + curValue );
                    outLine.Append( Environment.NewLine + "Phone: " + myTourRow["ChiefDriverPhone"] );
                    outLine.Append( "  Email: " + myTourRow["ChiefDriverEmail"] );
                    outLine.Append( Environment.NewLine + "Address: " + myTourRow["ChiefDriverAddress"] );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "AWSA APPROVED TOWBOATS: " );
                    outBuffer.WriteLine( outLine.ToString() );

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Was the tournament open to all towboat Manufacturers?: " );
                    if (OpenToAllYes.Checked) {
                        outLine.Append( "Yes" );
                    } else if (OpenToAllNo.Checked) {
                        outLine.Append( "No" );
                    } else {
                        outLine.Append( "N/A" );
                    }
                    outBuffer.WriteLine( outLine.ToString() );

                    //Write available data
                    if (myBoatUseDataTable.Rows.Count > 0) {
                        String curBoatCode = "";
                        int curRowCount = 0;
                        DataRow[] curFindRows = null;
                        String[] curValueSplit = null;
                        outLine = new StringBuilder( "" );

                        foreach (DataRow curRow in myBoatUseDataTable.Rows) {
                            curRowCount++;
                            outLine = new StringBuilder( "" );
                            if (isObjectEmpty( curRow["HullId"] )) {
                                MessageBox.Show( "Towboat data must be updated.  Please open the Boat Use Report and force a data save.  Then you may regenerate this report." );
                                continue;
                            } else {
                                curBoatCode = (String)curRow["HullId"];
                            }

                            curValue = "";    
                            curFindRows = myApprovedManufacturersDataTable.Select( "ListCode = '" + curBoatCode.Substring( 0, 2 ).ToUpper() + "'" );
                            if (curFindRows.Length > 0) {
                                curValue = (String)curFindRows[0]["CodeValue"];
                            } else {
                                curValue = curRow["BoatModel"].ToString();
                                if (curValue.Length > 0) {
                                    curValueSplit = curValue.Split( ' ' );
                                    if (curValueSplit.Length > 0) {
                                        curValue = curValueSplit[0];
                                        if (curValue.ToLower().Equals( "correct" )) {
                                            curValue = "Correct Craft";
                                        } else if (curValue.ToLower().Equals( "master" )) {
                                            curValue = "MasterCraft";
                                        }
                                    } else {
                                        curValue = "Unlisted";
                                    }
                                } else {
                                    curValue = "Unlisted";
                                }
                            }
                            outLine.Append( Environment.NewLine + "Manufacturer: " + curValue );
                            outLine.Append( "  Boat Model: " + curRow["BoatModel"].ToString() );
                            outLine.Append( "  Year: " + curRow["ModelYear"].ToString() );
                            outLine.Append( "  SpeedControlVersion: " + curRow["SpeedControlVersion"].ToString() );
                            outLine.Append( "  Owner: " + curRow["Owner"].ToString() );

                            outLine.Append( "  slalomUsed: " + curRow["slalomUsed"].ToString() );
                            outLine.Append( "  slalomCredit: " + curRow["slalomCredit"].ToString() );
                            outLine.Append( "  trickUsed: " + curRow["trickUsed"].ToString() );
                            outLine.Append( "  trickCredit: " + curRow["trickCredit"].ToString() );
                            outLine.Append( "  jumpUsed: " + curRow["jumpUsed"].ToString() );
                            outLine.Append( "  jumpCredit: " + curRow["jumpCredit"].ToString() );
                            outBuffer.WriteLine( outLine.ToString() );

                            outLine = new StringBuilder( "" );
                            outLine.Append( "PreEventNotes: " + curRow["PreEventNotes"].ToString() );
                            outLine.Append( Environment.NewLine + "PostEventNotes: " + curRow["PostEventNotes"].ToString() );
                            outLine.Append( Environment.NewLine + "Notes: " + curRow["Notes"].ToString() );
                            outBuffer.WriteLine( outLine.ToString() );
                        }
                    }

                    outLine = new StringBuilder( "" );
                    outLine.Append( Environment.NewLine + "Report created by " );
                    outLine.Append( Properties.Settings.Default.AppTitle + " Version " + Properties.Settings.Default.BuildVersion );
                    outLine.Append( " on " + DateTime.Now.ToString( "MMMM d, yyyy HH:mm:ss" ) );
                    outBuffer.WriteLine( outLine.ToString() );

                    Log.WriteFile( "Export driver boat use print file complete: " + curFilename );
                    MessageBox.Show( "Export driver boat use print file complete:" );
                } catch (Exception ex) {
                    MessageBox.Show( "Error: Failure detected writing driver boat use data print file (txt)\n\nError: " + ex.Message );
                    String curMsg = curMethodName + ":Exception=" + ex.Message;
                    Log.WriteFile( curMsg );
                }
                outBuffer.Close();
            }
        }

        public void PrintButton_Click(object sender, EventArgs e) {
            int curPageNum = 0;
            FormReportPrinter curFormPrint = new FormReportPrinter( ReportTabControl );
            curFormPrint.ReportHeader = "Tow Boat Use and Performance Document" + sanctionIdTextBox.Text + " - " + nameTextBox.Text;
            curFormPrint.ReportName = sanctionIdTextBox.Text + "TU";

            curFormPrint.CenterHeaderOnPage = true;
            curFormPrint.ReportHeaderFont = new Font( "Arial", 12, FontStyle.Bold, GraphicsUnit.Point );
            curFormPrint.ReportHeaderTextColor = Color.Black;

            curFormPrint.BottomMargin = 40;
            curFormPrint.TopMargin = 50;
            curFormPrint.LeftMargin = 50;
            curFormPrint.RightMargin = 50;

            if ( myBoatUseDataTable.Rows.Count > 0 ) {
                for ( myBoatRowIdx = 0; myBoatRowIdx < myBoatUseDataTable.Rows.Count; myBoatRowIdx += 5 ) {
                    TowboatUseLoad( myBoatRowIdx );

                    foreach ( TabPage curPage in ReportTabControl.TabPages ) {
                        curPage.Select();
                        curPage.Focus();
                        curPage.Show();
                    }
                    ReportTabControl.TabPages[0].Select();
                    ReportTabControl.TabPages[0].Focus();
                    ReportTabControl.TabPages[0].Show();

                    if (myBoatRowIdx > 0) {
                        curPageNum = ( myBoatRowIdx / 5) + 1;
                        curFormPrint.ReportName = sanctionIdTextBox.Text + "TU-P" + curPageNum.ToString();
                    }
                    curFormPrint.Print( false );
                }
            } else {
                ReportTabControl.TabPages[0].Select();
                ReportTabControl.TabPages[0].Focus();
                ReportTabControl.TabPages[0].Show();
                curFormPrint.Print();
            }
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

        private void getTourBoatUseData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT PK, SanctionId, TourBoatSeq , HullId, BoatModel, ModelYear, Owner" );
            curSqlStmt.Append( ", SlalomCredit, SlalomUsed, TrickCredit, TrickUsed, JumpCredit, JumpUsed" );
            curSqlStmt.Append( ", SpeedControlVersion, CertOfInsurance, InsuranceCompany, Notes, PostEventNotes, PreEventNotes " );
            curSqlStmt.Append( "FROM TourBoatUse " );
            curSqlStmt.Append( "WHERE (SanctionId = '" + mySanctionNum + "') " );
            curSqlStmt.Append( "ORDER BY SanctionId, HullId, TourBoatSeq, BoatModel, ModelYear" );
            myBoatUseDataTable = getData( curSqlStmt.ToString() );
        }

        private DataTable getApprovedManufacturers() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, ListCodeNum, CodeValue, SortSeq" );
            curSqlStmt.Append( " FROM CodeValueList" );
            curSqlStmt.Append( " WHERE ListName = 'ApprovedManufacturers'" );
            curSqlStmt.Append( " ORDER BY SortSeq" );
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
				curSqlStmt.Append( ", Coalesce( ML.JudgeSlalomRating, '' ) as JudgeSlalomRating" );
				curSqlStmt.Append( ", Coalesce( ML.JudgeTrickRating, '' ) as JudgeTrickRating" );
				curSqlStmt.Append( ", Coalesce( ML.JudgeJumpRating, '' ) as JudgeJumpRating" );
				curSqlStmt.Append( ", Coalesce( ML.ScorerSlalomRating, '' ) as ScorerSlalomRating" );
				curSqlStmt.Append( ", Coalesce( ML.ScorerTrickRating, '' ) as ScorerTrickRating" );
				curSqlStmt.Append( ", Coalesce( ML.ScorerJumpRating, '' ) as ScorerJumpRating" );
				curSqlStmt.Append( ", Coalesce( ML.DriverSlalomRating, '' ) as DriverSlalomRating" );
				curSqlStmt.Append( ", Coalesce( ML.DriverTrickRating, '' ) as DriverTrickRating" );
				curSqlStmt.Append( ", Coalesce( ML.DriverJumpRating, '' ) as DriverJumpRating" );
				curSqlStmt.Append( ", Coalesce( ML.SafetyOfficialRating, '' ) as SafetyOfficialRating" );
				curSqlStmt.Append( ", Coalesce( ML.TechOfficialRating, '' ) as TechOfficialRating" );
				curSqlStmt.Append( ", Coalesce( ML.AnncrOfficialRating, '' ) as AnncrOfficialRating " );
				curSqlStmt.Append( "FROM TourReg TR " );
				curSqlStmt.Append( "	INNER JOIN MemberList ML ON ML.MemberId = TR.MemberId  " );
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

            SaveFileDialog curFileDialog = new SaveFileDialog();
            String curPath = Properties.Settings.Default.ExportDirectory;
            curFileDialog.InitialDirectory = curPath;
            curFileDialog.FileName = inFileName;

            try {
                if ( curFileDialog.ShowDialog() == DialogResult.OK ) {
                    String curFileName = curFileDialog.FileName;
                    if ( curFileName != null ) {
                        if ( Path.GetExtension(curFileName) == null) curFileName += ".prn";
                        outBuffer = File.CreateText( curFileName );
                    }
                }
            } catch ( Exception ex ) {
                MessageBox.Show( "Error: Could not get a file to export data to " + "\n\nError: " + ex.Message );
            }

            return outBuffer;
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
