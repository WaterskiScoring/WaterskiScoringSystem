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
			curSqlStmt.Append( ", Coalesce( OW.JudgeSlalomRating, '' ) as JudgeSlalomRating" );
			curSqlStmt.Append( ", Coalesce( OW.JudgeTrickRating, '' ) as JudgeTrickRating" );
			curSqlStmt.Append( ", Coalesce( OW.JudgeJumpRating, '' ) as JudgeJumpRating" );
			curSqlStmt.Append( ", Coalesce( OW.ScorerSlalomRating, '' ) as ScorerSlalomRating" );
			curSqlStmt.Append( ", Coalesce( OW.ScorerTrickRating, '' ) as ScorerTrickRating" );
			curSqlStmt.Append( ", Coalesce( OW.ScorerJumpRating, '' ) as ScorerJumpRating" );
			curSqlStmt.Append( ", Coalesce( OW.DriverSlalomRating, '' ) as DriverSlalomRating" );
			curSqlStmt.Append( ", Coalesce( OW.DriverTrickRating, '' ) as DriverTrickRating" );
			curSqlStmt.Append( ", Coalesce( OW.DriverJumpRating, '' ) as DriverJumpRating" );
			curSqlStmt.Append( ", Coalesce( OW.SafetyOfficialRating, '' ) as SafetyOfficialRating" );
			curSqlStmt.Append( ", Coalesce( OW.TechOfficialRating, '' ) as TechOfficialRating" );
			curSqlStmt.Append( ", Coalesce( OW.AnncrOfficialRating, '' ) as AnncrOfficialRating " );
            curSqlStmt.Append( "FROM TourReg TR " );
            curSqlStmt.Append( "INNER JOIN OfficialWork OW ON OW.SanctionId = TR.SanctionId AND OW.MemberId = TR.MemberId " );
            curSqlStmt.Append( "Where TR.SanctionId = '" + mySanctionNum + " ' AND TR.MemberId = '" + myMemberId + "' " );

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

				JudgeSlalomRatingSelect.SelectedItem = (String) curRow["JudgeSlalomRating"];
				JudgeTrickRatingSelect.SelectedItem = (String) curRow["JudgeTrickRating"];
				JudgeJumpRatingSelect.SelectedItem = (String) curRow["JudgeJumpRating"];
				DriverSlalomRatingSelect.SelectedItem = (String) curRow["DriverSlalomRating"];
				DriverTrickRatingSelect.SelectedItem = (String) curRow["DriverTrickRating"];
				DriverJumpRatingSelect.SelectedItem = (String) curRow["DriverJumpRating"];
				ScorerSlalomRatingSelect.SelectedItem = (String) curRow["ScorerSlalomRating"];
				ScorerTrickRatingSelect.SelectedItem = (String) curRow["ScorerTrickRating"];
				ScorerJumpRatingSelect.SelectedItem = (String) curRow["ScorerJumpRating"];
				SafetyRatingSelect.SelectedItem = (String) curRow["SafetyOfficialRating"];
				TechOfficialRatingSelect.SelectedItem = (String) curRow["TechOfficialRating"];
				AnncrOfficialRatingSelect.SelectedItem = (String) curRow["AnncrOfficialRating"];

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
