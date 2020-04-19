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

namespace WaterskiScoringSystem.Admin {
    public partial class TourEventTech : Form {
        private String mySanctionNum = null;
        private Boolean isDataModified = false;
        private String myOrigItemValue;
        private DataTable myDataTable;
        private SqlCeCommand mySqlStmt = null;
        private SqlCeConnection myDbConn = null;

        public TourEventTech() {
            InitializeComponent();
        }

        public String SanctionNum {
            get { return mySanctionNum; }
            set { mySanctionNum = value; }
        }

        public void TourEventTech_Show( String inSanctionNum ) {
            //Retrieve tournament list and set current position to active tournament
            myDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
            myDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;

            mySanctionNum = inSanctionNum;
            navRefresh_Click( null, null );
        }

        private void TourEventTech_Load( object sender, EventArgs e ) {
            if ( Properties.Settings.Default.TourEventTech_Width > 0 ) {
                this.Width = Properties.Settings.Default.TourEventTech_Width;
            }
            if ( Properties.Settings.Default.TourEventTech_Height > 0 ) {
                this.Height = Properties.Settings.Default.TourEventTech_Height;
            }
            if ( Properties.Settings.Default.TourEventTech_Location.X > 0
                && Properties.Settings.Default.TourEventTech_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.TourEventTech_Location;
            }

        }

        private void cancelButton_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void TourEventTech_FormClosing(object sender, FormClosingEventArgs e) {
        }

        private void TourEventTech_FormClosed( object sender, FormClosedEventArgs e ) {
            if ( this.WindowState == FormWindowState.Normal ) {
                Properties.Settings.Default.TourEventTech_Width = this.Size.Width;
                Properties.Settings.Default.TourEventTech_Height = this.Size.Height;
                Properties.Settings.Default.TourEventTech_Location = this.Location;
            }
        }

        private void bindingSource_DataError( object sender, BindingManagerDataErrorEventArgs e ) {
            MessageBox.Show( "tournamentBindingSource_DataError"
                + "\n sender:" + sender.GetType()
                + "\n EventArgs:" + e.GetType()
            );
        }

        private void navSaveItem_Click( object sender, EventArgs e ) {
            try {
                if (isDataModified) {
                    saveData();
                }
                if (!isDataModified) {
                    this.Close();
                }
            } catch (Exception excp) {
                MessageBox.Show( "Error attempting to save changes \n" + excp.Message );
            }
        }

        private bool saveData() {
            bool curReturnValue = false;
            int rowsProc = 0;

            try {
                String curValue = "";
                String curSlalomRopesSpecs = "Rope 1"
                    + ";" + Rope1Line2300TextBox.Text
                    + ";" + Rope1Line1825TextBox.Text
                    + ";" + Rope1Line1600TextBox.Text
                    + ";" + Rope1Line1425TextBox.Text
                    + ";" + Rope1Line1300TextBox.Text
                    + ";" + Rope1Line1200TextBox.Text
                    + ";" + Rope1Line1125TextBox.Text
                    + ";" + Rope1Line1075TextBox.Text
                    + ";" + Rope1Line1025TextBox.Text
                    + ";Rope 2"
                    + ";" + Rope2Line2300TextBox.Text
                    + ";" + Rope2Line1825TextBox.Text
                    + ";" + Rope2Line1600TextBox.Text
                    + ";" + Rope2Line1425TextBox.Text
                    + ";" + Rope2Line1300TextBox.Text
                    + ";" + Rope2Line1200TextBox.Text
                    + ";" + Rope2Line1125TextBox.Text
                    + ";" + Rope2Line1075TextBox.Text
                    + ";" + Rope2Line1025TextBox.Text
                    + ";Rope 3"
                    + ";" + Rope3Line2300TextBox.Text
                    + ";" + Rope3Line1825TextBox.Text
                    + ";" + Rope3Line1600TextBox.Text
                    + ";" + Rope3Line1425TextBox.Text
                    + ";" + Rope3Line1300TextBox.Text
                    + ";" + Rope3Line1200TextBox.Text
                    + ";" + Rope3Line1125TextBox.Text
                    + ";" + Rope3Line1075TextBox.Text
                    + ";" + Rope3Line1025TextBox.Text
                    ;
                curValue = escapeString( curSlalomRopesSpecs );
                curSlalomRopesSpecs = curValue;

                String curRopeHandlesSpecs = "Rope Handle"
                    + ";" + RopeHandle1TextBox.Text
                    + ";" + RopeHandle2TextBox.Text
                    + ";" + RopeHandle3TextBox.Text
                    + ";" + RopeHandle4TextBox.Text
                    ;
                curValue = escapeString( curRopeHandlesSpecs );
                curRopeHandlesSpecs = curValue;

                String curJumpRopesSpecs = "Jump Line"
                    + ";" + JumpLine1TextBox.Text
                    + ";" + JumpLine2TextBox.Text
                    + ";" + JumpLine3TextBox.Text
                    + ";" + JumpLine4TextBox.Text
                    + ";" + "Jump Handle"
                    + ";" + JumpHandle1TextBox.Text
                    + ";" + JumpHandle2TextBox.Text
                    + ";" + JumpHandle3TextBox.Text
                    + ";" + JumpHandle4TextBox.Text
                    ;
                curValue = escapeString( curJumpRopesSpecs );
                curJumpRopesSpecs = curValue;

                myDbConn.Open();
                mySqlStmt = myDbConn.CreateCommand();
                mySqlStmt.CommandText = "";

                StringBuilder curSqlStmt = new StringBuilder( "" );
                curSqlStmt.Append("Update Tournament Set ");
                curSqlStmt.Append( "LastUpdateDate = getdate() " );
                curSqlStmt.Append( ", SlalomRopesSpecs = '" + curSlalomRopesSpecs + "'" );
                curSqlStmt.Append( ", RopeHandlesSpecs = '" + curRopeHandlesSpecs + "'" );
                curSqlStmt.Append( ", JumpRopesSpecs = '" + curJumpRopesSpecs + "'" );
                curSqlStmt.Append( ", SlalomCourseSpecs = '" + escapeString(slalomCourseSpecsTextBox.Text) + "'" );
                curSqlStmt.Append( ", JumpCourseSpecs = '" + escapeString(jumpCourseSpecsTextBox.Text) + "'" );
                curSqlStmt.Append( ", TrickCourseSpecs = '" + escapeString(trickCourseSpecsTextBox.Text) + "'" );
                curSqlStmt.Append( ", BuoySpecs = '" + escapeString(buoySpecsTextBox.Text) + "' " );
                curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + "'" );

                mySqlStmt.CommandText = curSqlStmt.ToString();
                rowsProc = mySqlStmt.ExecuteNonQuery();
                if (rowsProc > 0) {
                    isDataModified = false;
                    curReturnValue = true;
                }
                winStatusMsg.Text = "Changes successfully saved";

            } catch (Exception excp) {
                curReturnValue = false;
                MessageBox.Show( "Error attempting to update tournament data \n" + excp.Message );
            } finally {
                myDbConn.Close();
            }
            return curReturnValue;
        }

        private void navRefresh_Click(object sender, EventArgs e) {
            try {
                winStatusMsg.Text = "Retrieving Tournament data";
                Cursor.Current = Cursors.WaitCursor;
                myDataTable = getTourData();
                if (myDataTable != null) {
                    if (myDataTable.Rows.Count > 0) {
                        String[] curValueList;
                        DataRow curRow = myDataTable.Rows[0];
                        if ( isObjectEmpty( curRow["SlalomRopesSpecs"]) ) {
                        } else {
                            curValueList = ( (String)curRow["SlalomRopesSpecs"] ).Split( ';' );

                            Rope1Line2300TextBox.Text = curValueList[1];
                            Rope1Line1825TextBox.Text = curValueList[2];
                            Rope1Line1600TextBox.Text = curValueList[3];
                            Rope1Line1425TextBox.Text = curValueList[4];
                            Rope1Line1300TextBox.Text = curValueList[5];
                            Rope1Line1200TextBox.Text = curValueList[6];
                            Rope1Line1125TextBox.Text = curValueList[7];
                            Rope1Line1075TextBox.Text = curValueList[8];
                            Rope1Line1025TextBox.Text = curValueList[9];
                            Rope2Line2300TextBox.Text = curValueList[11];
                            Rope2Line1825TextBox.Text = curValueList[12];
                            Rope2Line1600TextBox.Text = curValueList[13];
                            Rope2Line1425TextBox.Text = curValueList[14];
                            Rope2Line1300TextBox.Text = curValueList[15];
                            Rope2Line1200TextBox.Text = curValueList[16];
                            Rope2Line1125TextBox.Text = curValueList[17];
                            Rope2Line1075TextBox.Text = curValueList[18];
                            Rope2Line1025TextBox.Text = curValueList[19];
                            Rope3Line2300TextBox.Text = curValueList[21];
                            Rope3Line1825TextBox.Text = curValueList[22];
                            Rope3Line1600TextBox.Text = curValueList[23];
                            Rope3Line1425TextBox.Text = curValueList[24];
                            Rope3Line1300TextBox.Text = curValueList[25];
                            Rope3Line1200TextBox.Text = curValueList[26];
                            Rope3Line1125TextBox.Text = curValueList[27];
                            Rope3Line1075TextBox.Text = curValueList[28];
                            Rope3Line1025TextBox.Text = curValueList[29];
                        }

                        if ( isObjectEmpty( curRow["RopeHandlesSpecs"]) ) {
                        } else {
                            curValueList = ( (String)curRow["RopeHandlesSpecs"] ).Split( ';' );
                            RopeHandle1TextBox.Text = curValueList[1];
                            RopeHandle2TextBox.Text = curValueList[2];
                            RopeHandle3TextBox.Text = curValueList[3];
                            RopeHandle4TextBox.Text = curValueList[4];
                        }

                        if ( isObjectEmpty( curRow["JumpRopesSpecs"]) ) {
                        } else {
                            curValueList = ( (String)curRow["JumpRopesSpecs"] ).Split( ';' );
                            JumpLine1TextBox.Text = curValueList[1];
                            JumpLine2TextBox.Text = curValueList[2];
                            JumpLine3TextBox.Text = curValueList[3];
                            JumpLine4TextBox.Text = curValueList[4];
                            JumpHandle1TextBox.Text = curValueList[6];
                            JumpHandle2TextBox.Text = curValueList[7];
                            JumpHandle3TextBox.Text = curValueList[8];
                            JumpHandle4TextBox.Text = curValueList[9];
                        }
                        if (isObjectEmpty( curRow["SlalomCourseSpecs"] )) {
                            slalomCourseSpecsTextBox.Text = "";
                        } else {
                            slalomCourseSpecsTextBox.Text = (String)curRow["SlalomCourseSpecs"];
                        }
                        if (isObjectEmpty( curRow["JumpCourseSpecs"] )) {
                            jumpCourseSpecsTextBox.Text = "";
                        } else {
                            jumpCourseSpecsTextBox.Text = (String)curRow["JumpCourseSpecs"];
                        }
                        if (isObjectEmpty( curRow["TrickCourseSpecs"] )) {
                            trickCourseSpecsTextBox.Text = "";
                        } else {
                            trickCourseSpecsTextBox.Text = (String)curRow["TrickCourseSpecs"];
                        }
                        if (isObjectEmpty( curRow["BuoySpecs"] )) {
                            buoySpecsTextBox.Text = "";
                        } else {
                            buoySpecsTextBox.Text = (String)curRow["BuoySpecs"];
                        }
                    }
                }

            } catch ( Exception ex ) {
                MessageBox.Show( "Error attempting to retrieve tournament members\n" + ex.Message );
            }
        }

        private void EnterItemTextBox( object sender, EventArgs e ) {
            myOrigItemValue = ( (TextBox)sender ).Text;
        }

        private void ItemTextChanged( object sender, EventArgs e ) {
            if ( !( ( (TextBox)sender ).Text.Equals( myOrigItemValue ) ) ) {
                isDataModified = true;
            }
        }

        private DataTable getTourData() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT SanctionId, ContactMemberId, Name, Class, COALESCE(L.CodeValue, 'C') as EventScoreClass, T.Federation" );
            curSqlStmt.Append( ", SlalomRounds, TrickRounds, JumpRounds, Rules, EventDates, EventLocation" );
            curSqlStmt.Append( ", SlalomRopesSpecs, RopeHandlesSpecs, JumpRopesSpecs" );
            curSqlStmt.Append( ", SlalomCourseSpecs, JumpCourseSpecs, TrickCourseSpecs, BuoySpecs " );
            curSqlStmt.Append( "FROM Tournament T " );
            curSqlStmt.Append( "LEFT OUTER JOIN CodeValueList L ON ListName = 'ClassToEvent' AND ListCode = T.Class " );
            curSqlStmt.Append( "WHERE T.SanctionId = '" + mySanctionNum + "' " );
            return getData( curSqlStmt.ToString() );
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private bool isObjectEmpty(object inObject) {
            bool curReturnValue = false;
            if (inObject == null) {
                curReturnValue = true;
            } else if (inObject == System.DBNull.Value) {
                curReturnValue = true;
            } else if (inObject.ToString().Length > 0) {
                curReturnValue = false;
            } else {
                curReturnValue = true;
            }
            return curReturnValue;
        }

        private String escapeString(String inValue) {
            String curReturnValue = "";
            char[] singleQuoteDelim = new char[] { '\'' };
            //char[] doubleQuoteDelim = new char[] { '"' };
            curReturnValue = stringReplace( inValue, singleQuoteDelim, "''" );
            //curReturnValue = stringReplace( curValue1, doubleQuoteDelim, "\\\"" );
            return curReturnValue;
        }
        private String stringReplace(String inValue, char[] inCurValue, String inReplValue) {
            StringBuilder curNewValue = new StringBuilder( "" );

            String[] curValues = inValue.Split( inCurValue );
            if (curValues.Length > 1) {
                int curCount = 0;
                foreach (String curValue in curValues) {
                    curCount++;
                    if (curCount < curValues.Length) {
                        curNewValue.Append( curValue + inReplValue );
                    } else {
                        curNewValue.Append( curValue );
                    }
                }
            } else {
                curNewValue.Append( inValue );
            }

            return curNewValue.ToString();
        }

    }
}
