using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Trick {
    public partial class SkierVideoMatchDialogForm : Form {
        private String myFileName = "";
        private String mySkierMemberId = "";
        private String mySkierName = "";
        private String mySkierAgeGroup = "";
        private Int16 myTourRounds = 0;
        private Int16 mySkierRound = 0;
        private Int16 mySkierPass = 0;
        private DataTable myDataTable = null;

        public SkierVideoMatchDialogForm() {
            InitializeComponent();
        }

        private void SkierVideoMatchDialogForm_Load(object sender, EventArgs e) {
        }

        public Int16 TourRounds {
            get { return myTourRounds; }
            set {
                myTourRounds = value;
                roundActiveSelect.SelectList_LoadHorztl( myTourRounds.ToString(), roundActiveSelect_Click );
                roundActiveSelect.RoundValue = "1";
            }
        }

        public Int16 SkierRound {
            get { return mySkierRound; }
            set {
                mySkierRound = value;
                if (mySkierRound <= myTourRounds) {
                    roundActiveSelect.RoundValue = "mySkierRound";
                }
            }
        }

        public Int16 SkierPass {
            get { return mySkierPass; }
            set {
                mySkierPass = value;
                if (mySkierPass == 1) Pass1RadioButton.Checked = true;
                if (mySkierPass == 2) Pass2RadioButton.Checked = true;
            }
        }

        public String FileName {
            get { return myFileName; }
            set { 
                myFileName = value;
                FileNameTextbox.Text = myFileName;
            }
        }

        public String SkierNameSelected {
            get { return mySkierName; }
            set {
                mySkierName = value;
            }
        }

        public String SkierMemberId {
            get { return mySkierMemberId; }
            set {
                mySkierMemberId = value;
            }
        }

        public String SkierAgeGroup {
            get { return mySkierAgeGroup; }
            set {
                mySkierAgeGroup = value;
            }
        }

        public DataTable SkierMatchList {
            get { return myDataTable; }
            set {
                myDataTable = value;
                previewDataGridView.Rows.Clear();
                int curRowIdx = 0;
                foreach (DataRow curRow in myDataTable.Rows) {
                    curRowIdx = previewDataGridView.Rows.Add();
                    DataGridViewRow curViewRow = previewDataGridView.Rows[curRowIdx];
                    curViewRow.Cells["SkierName"].Value = (String)curRow["SkierName"];
                    curViewRow.Cells["MemberId"].Value = (String)curRow["MemberId"];
                    curViewRow.Cells["AgeGroup"].Value = (String)curRow["AgeGroup"];
                }
            }
        }

        private void roundActiveSelect_Click(object sender, EventArgs e) {
            if (sender != null) {
                String curValue = ( (RadioButtonWithValue)sender ).Value.ToString();
                try {
                    roundActiveSelect.RoundValue = curValue;
                    mySkierRound = Int16.Parse(curValue);
                } catch (Exception ex) {
                    MessageBox.Show("Exception encountered selecting round: \n\n" + ex.Message);
                    mySkierRound = 0;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e) {
            int curViewIdx = previewDataGridView.CurrentRow.Index;
            mySkierMemberId = (String)previewDataGridView.Rows[curViewIdx].Cells["MemberId"].Value;
            mySkierName = (String)previewDataGridView.Rows[curViewIdx].Cells["SkierName"].Value;
            mySkierAgeGroup = (String)previewDataGridView.Rows[curViewIdx].Cells["AgeGroup"].Value;
            if (Pass1RadioButton.Checked) mySkierPass = 1;
            if (Pass2RadioButton.Checked) mySkierPass = 2;
        }

    }
}
