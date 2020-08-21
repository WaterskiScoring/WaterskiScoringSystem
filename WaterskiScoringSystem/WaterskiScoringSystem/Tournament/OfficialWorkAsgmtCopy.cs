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

namespace WaterskiScoringSystem.Tournament {
    public partial class OfficialWorkAsgmtCopy : Form {
        private String myCopyToGroup = "";
        private String myCopyToRound = "";
		private String myCopyFromGroup = "";
		private String myCopyFromRound = "";
		private bool isWindowLoading = true;

        public OfficialWorkAsgmtCopy() {
            InitializeComponent();
        }

        private void OfficialWorkAsgmtCopy_Load(object sender, EventArgs e) {
        }

        public String CopyToGroup {
            get {
                return myCopyToGroup;
            }
            set {
                myCopyToGroup = value;
            }
        }

        public String CopyToRound {
            get {
                return myCopyToRound;
            }
            set {
                myCopyToRound = value;
            }
        }

		public String CopyFromGroup {
			set {
				myCopyFromGroup = value;
			}
		}

		public String CopyFromRound {
			set {
				myCopyFromRound = value;
			}
		}


		public void showAvailable(String inSanctionId, String inRules, String inEvent, Int16 inRounds) {
            StringBuilder curSqlStmt = new StringBuilder( "" );
            isWindowLoading = true;

			CopyFromLabel.Text = "Copying from group " + myCopyFromGroup + " round " + myCopyFromRound;

            if (inRules.ToLower().Equals( "ncwsa" )) {
                DataGridViewRow curViewRow;
                int curViewIdx = 0;
                for (Int16 curRound = 1; curRound <= inRounds; curRound++) {
                    curViewIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curViewIdx];
                    curViewRow.Cells["EventGroup"].Value = "Men A";
                    curViewRow.Cells["Round"].Value = ( (int)curRound ).ToString();

                    curViewIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curViewIdx];
                    curViewRow.Cells["EventGroup"].Value = "Women A";
                    curViewRow.Cells["Round"].Value = ( (int)curRound ).ToString();

                    curViewIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curViewIdx];
                    curViewRow.Cells["EventGroup"].Value = "Men B";
                    curViewRow.Cells["Round"].Value = ( (int)curRound ).ToString();

                    curViewIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curViewIdx];
                    curViewRow.Cells["EventGroup"].Value = "Women B";
                    curViewRow.Cells["Round"].Value = ( (int)curRound ).ToString();

                    curViewIdx = dataGridView.Rows.Add();
                    curViewRow = dataGridView.Rows[curViewIdx];
                    curViewRow.Cells["EventGroup"].Value = "Non Team";
                    curViewRow.Cells["Round"].Value = ( (int)curRound ).ToString();
                }
            } else {
                for (Int16 curRound = 1; curRound <= inRounds; curRound++) {
                    if (curRound > 1) {
                        curSqlStmt.Append( "Union " );
                    }
                    curSqlStmt.Append( "Select Distinct EventGroup, " + curRound + " as Round " );
                    curSqlStmt.Append( "From EventReg ER " );
                    curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
                    curSqlStmt.Append( "And Event = '" + inEvent + "' " );
                    curSqlStmt.Append( "And EventGroup Not IN (Select Distinct EventGroup From OfficialWorkAsgmt OWA " );
                    curSqlStmt.Append( "    Where OWA.SanctionId = ER.SanctionId And OWA.Event = ER.Event And OWA.Round = " + curRound + ") " );
                }
                curSqlStmt.Append( "Union " );
                curSqlStmt.Append( "Select Distinct EventGroup, 25 as Round " );
                curSqlStmt.Append( "From EventReg ER " );
                curSqlStmt.Append( "Where SanctionId = '" + inSanctionId + "' " );
                curSqlStmt.Append( "And Event = '" + inEvent + "' " );
                curSqlStmt.Append( "And EventGroup Not IN (Select Distinct EventGroup From OfficialWorkAsgmt OWA " );
                curSqlStmt.Append( "    Where OWA.SanctionId = ER.SanctionId And OWA.Event = ER.Event And OWA.Round = 25) " );
                curSqlStmt.Append( "Order by Round, EventGroup " );
                DataTable curDataTable = getData( curSqlStmt.ToString() );
                if (curDataTable.Rows.Count > 0) {
                    dataGridView.Rows.Clear();
                    DataGridViewRow curViewRow;
                    int curViewIdx = 0;
                    foreach (DataRow curDataRow in curDataTable.Rows) {
                        curViewIdx = dataGridView.Rows.Add();
                        curViewRow = dataGridView.Rows[curViewIdx];

                        curViewRow.Cells["EventGroup"].Value = (String)curDataRow["EventGroup"];
                        curViewRow.Cells["Round"].Value = ( (int)curDataRow["Round"] ).ToString();
                    }
                }
            }
            isWindowLoading = false;
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private void dataGridView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            CopyToGroup = dataGridView.Rows[e.RowIndex].Cells["EventGroup"].Value.ToString();
            CopyToRound = dataGridView.Rows[e.RowIndex].Cells["Round"].Value.ToString();
        }

        private void dataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            CopyToGroup = dataGridView.Rows[e.RowIndex].Cells["EventGroup"].Value.ToString();
            CopyToRound = dataGridView.Rows[e.RowIndex].Cells["Round"].Value.ToString();
        }

        private void dataGridView_RowEnter(object sender, DataGridViewCellEventArgs e) {
            if (!isWindowLoading) {
                CopyToGroup = dataGridView.Rows[e.RowIndex].Cells["EventGroup"].Value.ToString();
                CopyToRound = dataGridView.Rows[e.RowIndex].Cells["Round"].Value.ToString();
            }
        }
    }
}
