using System;
using System.Collections;
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
    public partial class EditMemberOfficialsRatings : Form {
        private String mySanctionNum;
        private String myMemberId;
        private String myTourRules;

        private DataRow myTourRow;

        public EditMemberOfficialsRatings() {
            InitializeComponent();
        }

        public bool editMember(String inMemberId) {
            myMemberId = inMemberId;
            return true;
        }

        private void EditMemberOfficialsRatings_Load(object sender, EventArgs e) {
            if ( Properties.Settings.Default.EditMemberOfficialRatings_Width > 0 ) {
                this.Width = Properties.Settings.Default.EditMemberOfficialRatings_Width;
            }
            if ( Properties.Settings.Default.EditMemberOfficialRatings_Height > 0 ) {
                this.Height = Properties.Settings.Default.EditMemberOfficialRatings_Height;
            }
            if ( Properties.Settings.Default.EditMemberOfficialRatings_Location.X > 0
                && Properties.Settings.Default.EditMemberOfficialRatings_Location.Y > 0 ) {
                this.Location = Properties.Settings.Default.EditMemberOfficialRatings_Location;
            }
            
            mySanctionNum = Properties.Settings.Default.AppSanctionNum;
            if (mySanctionNum == null) {
                MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
            } else {
                if (mySanctionNum.Length < 6) {
                    MessageBox.Show( "An active tournament must be selected from the Administration menu Tournament List option" );
                } else {
                    //Retrieve selected tournament attributes
                    DataTable curTourDataTable = getTourData();
                    if (curTourDataTable.Rows.Count > 0) {
                        myTourRow = curTourDataTable.Rows[0];
                        myTourRules = (String)myTourRow["Rules"];

                        loadRatingSelect( "JudgeRating", JudgeSlalomRatingSelect );
                        loadRatingSelect( "JudgeRating", JudgeTrickRatingSelect );
                        loadRatingSelect( "JudgeRating", JudgeJumpRatingSelect );

                        loadRatingSelect( "DriverRating", DriverSlalomRatingSelect );
                        loadRatingSelect( "DriverRating", DriverTrickRatingSelect );
                        loadRatingSelect( "DriverRating", DriverJumpRatingSelect );

                        loadRatingSelect( "ScorerRating", ScorerSlalomRatingSelect );
                        loadRatingSelect( "ScorerRating", ScorerTrickRatingSelect );
                        loadRatingSelect( "ScorerRating", ScorerJumpRatingSelect );

                        loadRatingSelect( "SafetyRating", SafetyRatingSelect );
                        loadRatingSelect( "TechRating", TechOfficialRatingSelect );
                        loadRatingSelect( "AnnouncerRating", AnncrOfficialRatingSelect );

                        editMemberDataLoad();
                    }
                }
            }
        }

        private void EditMemberOfficialsRatings_FormClosed(object sender, FormClosedEventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                Properties.Settings.Default.EditMemberOfficialRatings_Width = this.Size.Width;
                Properties.Settings.Default.EditMemberOfficialRatings_Height = this.Size.Height;
                Properties.Settings.Default.EditMemberOfficialRatings_Location = this.Location;
            }

        }

        private void editMemberDataLoad() {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "Select Distinct 'OW' as EntityName, TR.SanctionId, TR.MemberId, TR.SkierName" );
            curSqlStmt.Append( ", OW.JudgeSlalomRating, OW.JudgeTrickRating, OW.JudgeJumpRating" );
            curSqlStmt.Append( ", OW.DriverSlalomRating, OW.DriverTrickRating, OW.DriverJumpRating" );
            curSqlStmt.Append( ", OW.ScorerSlalomRating, OW.ScorerTrickRating, OW.ScorerJumpRating" );
            curSqlStmt.Append( ", OW.SafetyOfficialRating, OW.TechOfficialRating, OW.AnncrOfficialRating " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "INNER JOIN OfficialWork OW ON OW.SanctionId = TR.SanctionId AND OW.MemberId = TR.MemberId " );
            curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + myMemberId + "' " );
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
                curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + myMemberId + "' " );
                curDataTable = getData( curSqlStmt.ToString() );
            }

            if (curDataTable.Rows.Count > 0) {
                DataRow curRow = curDataTable.Rows[0];
                editMemberId.Text = curRow["MemberId"].ToString();

                try {
                    editEntityName.Text = curRow["EntityName"].ToString();
                    String curName = curRow["SkierName"].ToString();
                    int curDelim = curName.LastIndexOf( "," );
                    editFirstName.Text = curName.Substring( curDelim + 2 );
                    editLastName.Text = curName.Substring( 0, curDelim );
                } catch {
                    editFirstName.Text = "";
                    editLastName.Text = "";
                }
                if (!( isObjectEmpty( curRow["JudgeSlalomRating"] ) )) {
                    JudgeSlalomRatingSelect.SelectedItem = (String)curRow["JudgeSlalomRating"];
                }
                if (!( isObjectEmpty( curRow["JudgeTrickRating"] ) )) {
                    JudgeTrickRatingSelect.SelectedItem = (String)curRow["JudgeTrickRating"];
                }
                if (!( isObjectEmpty( curRow["JudgeJumpRating"] ) )) {
                    JudgeJumpRatingSelect.SelectedItem = (String)curRow["JudgeJumpRating"];
                }
                if (!( isObjectEmpty( curRow["DriverSlalomRating"] ) )) {
                    DriverSlalomRatingSelect.SelectedItem = (String)curRow["DriverSlalomRating"];
                }
                if (!( isObjectEmpty( curRow["DriverTrickRating"] ) )) {
                    DriverTrickRatingSelect.SelectedItem = (String)curRow["DriverTrickRating"];
                }
                if (!( isObjectEmpty( curRow["DriverJumpRating"] ) )) {
                    DriverJumpRatingSelect.SelectedItem = (String)curRow["DriverJumpRating"];
                }
                if (!( isObjectEmpty( curRow["ScorerSlalomRating"] ) )) {
                    ScorerSlalomRatingSelect.SelectedItem = (String)curRow["ScorerSlalomRating"];
                }
                if (!( isObjectEmpty( curRow["ScorerTrickRating"] ) )) {
                    ScorerTrickRatingSelect.SelectedItem = (String)curRow["ScorerTrickRating"];
                }
                if (!( isObjectEmpty( curRow["ScorerJumpRating"] ) )) {
                    ScorerJumpRatingSelect.SelectedItem = (String)curRow["ScorerJumpRating"];
                }
                if (!( isObjectEmpty( curRow["SafetyOfficialRating"] ) )) {
                    SafetyRatingSelect.SelectedItem = (String)curRow["SafetyOfficialRating"];
                }
                if (!( isObjectEmpty( curRow["TechOfficialRating"] ) )) {
                    TechOfficialRatingSelect.SelectedItem = (String)curRow["TechOfficialRating"];
                }
                if (!( isObjectEmpty( curRow["AnncrOfficialRating"] ) )) {
                    AnncrOfficialRatingSelect.SelectedItem = (String)curRow["AnncrOfficialRating"];
                }
            } else {
                MessageBox.Show( "Error: Member data not found" );
            }
        }

        private void saveButton_Click(object sender, EventArgs e) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            SqlCeConnection curDbConn = new global::System.Data.SqlServerCe.SqlCeConnection();
            SqlCeCommand curSqlCmd = curDbConn.CreateCommand();
            try {
                curDbConn.ConnectionString = Properties.Settings.Default.waterskiConnectionStringApp;
                curDbConn.Open();

                if (editEntityName.Text.Equals( "TR" )) {
                    //Insert OfficialWork
                    curSqlStmt.Append( "Insert INTO OfficialWork (" );
                    curSqlStmt.Append( "SanctionId, MemberId, LastUpdateDate" );
                    curSqlStmt.Append( ", JudgeSlalomRating, JudgeTrickRating, JudgeJumpRating" );
                    curSqlStmt.Append( ", DriverSlalomRating, DriverTrickRating, DriverJumpRating" );
                    curSqlStmt.Append( ", ScorerSlalomRating, ScorerTrickRating, ScorerJumpRating" );
                    curSqlStmt.Append( ", SafetyOfficialRating, TechOfficialRating, AnncrOfficialRating " );
                    curSqlStmt.Append( ") Values (" );
                    curSqlStmt.Append( "'" + mySanctionNum + "'" );
                    curSqlStmt.Append( ", '" + editMemberId.Text + "'" );
                    curSqlStmt.Append( ", getdate() " );
                    curSqlStmt.Append( ", '" + JudgeSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + JudgeTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + JudgeJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + DriverSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + DriverTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + DriverJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + ScorerSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + ScorerTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + ScorerJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + SafetyRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + TechOfficialRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", '" + AnncrOfficialRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ")" );
                } else {
                    //Update OfficialWork
                    curSqlStmt.Append( "Update OfficialWork Set " );
                    curSqlStmt.Append( "LastUpdateDate = GETDATE() " );
                    curSqlStmt.Append( ", JudgeSlalomRating = '" + JudgeSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", JudgeTrickRating = '" + JudgeTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", JudgeJumpRating = '" + JudgeJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", DriverSlalomRating = '" + DriverSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", DriverTrickRating = '" + DriverTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", DriverJumpRating = '" + DriverJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", ScorerSlalomRating = '" + ScorerSlalomRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", ScorerTrickRating = '" + ScorerTrickRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", ScorerJumpRating = '" + ScorerJumpRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", SafetyOfficialRating = '" + SafetyRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", TechOfficialRating = '" + TechOfficialRatingSelect.SelectedItem + "'" );
                    curSqlStmt.Append( ", AnncrOfficialRating = '" + AnncrOfficialRatingSelect.SelectedItem + "' " );
                    curSqlStmt.Append( "Where SanctionId = '" + mySanctionNum + " ' AND MemberId = '" + editMemberId.Text + "' " );
                }
                curSqlCmd.CommandText = curSqlStmt.ToString();
                int rowsProc = curSqlCmd.ExecuteNonQuery();
                if (rowsProc > 0) {
                    MessageBox.Show( "Official ratings for skier have been updated" );
                }
            } catch (Exception ex) {
                String ExcpMsg = ex.Message;
                if (curSqlCmd != null) {
                    ExcpMsg += "\n" + curSqlCmd.CommandText;
                }
                MessageBox.Show( "Error attempting to save officials ratings "  + "\n\nError: " + ExcpMsg
                    );
            } finally {
                curDbConn.Close();
            }
        }

        private void loadRatingSelect(String inListName, ComboBox curSelectBox) {
            ArrayList curDropdownList = new ArrayList();
            String curListCode, curCodeValue;
            curSelectBox.DisplayMember = "ItemName";
            curSelectBox.ValueMember = "ItemValue";

            StringBuilder curSqlStmt = new StringBuilder( "" );
            curSqlStmt.Append( "SELECT ListCode, CodeValue FROM CodeValueList WHERE ListName = '" + inListName + "' ORDER BY SortSeq" );
            DataTable curDataTable = getData( curSqlStmt.ToString() );
            foreach (DataRow curRow in curDataTable.Rows) {
                curListCode = (String)curRow["ListCode"];
                curCodeValue = (String)curRow["CodeValue"];
                //curDropdownList.Add( new ListItem( curCodeValue, curListCode ) );
                curDropdownList.Add( curCodeValue );
            }
            curSelectBox.DataSource = curDropdownList;
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

        public String JudgeSlalomRating {
            get {return (String)JudgeSlalomRatingSelect.SelectedItem;}
        }
        public String JudgeTrickRating {
            get {return (String)JudgeTrickRatingSelect.SelectedItem;}
        }
        public String JudgeJumpRating {
            get {return (String)JudgeJumpRatingSelect.SelectedItem;}
        }
        public String DriverSlalomRating {
            get {return (String)DriverSlalomRatingSelect.SelectedItem;}
        }
        public String DriverTrickRating {
            get {return (String)DriverTrickRatingSelect.SelectedItem;}
        }
        public String DriverJumpRating {
            get {return (String)DriverJumpRatingSelect.SelectedItem;}
        }
        public String ScorerSlalomRating {
            get {return (String)ScorerSlalomRatingSelect.SelectedItem;}
        }
        public String ScorerTrickRating {
            get {return (String)ScorerTrickRatingSelect.SelectedItem;}
        }
        public String ScorerJumpRating {
            get {return (String)ScorerJumpRatingSelect.SelectedItem;}
        }
        public String SafetyRating {
            get {return (String)SafetyRatingSelect.SelectedItem;}
        }
        public String TechOfficialRating {
            get {return (String)TechOfficialRatingSelect.SelectedItem;}
        }
        public String AnncrOfficialRating {
            get {return (String)AnncrOfficialRatingSelect.SelectedItem;}
        }
}
}
