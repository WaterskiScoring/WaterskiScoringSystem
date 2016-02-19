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
using WaterskiScoringSystem.Tools;

namespace WaterskiScoringSystem.Tools {
    public partial class RunOrderClassForm : Form {
        private ListSkierClass mySkierClassList;
        private String myTourRules = "";
        private String myTourClass = "";
        private String mySanctionNum;
        private String myCopyFromClass = "";
        private String myCopyToClass = "";
        private String myCopyGroup = "";


        public RunOrderClassForm() {
            InitializeComponent();
        }

        private void RunOrderClassForm_Load(object sender, EventArgs e) {

        }

        public String CopyToClass {
            get { return myCopyToClass; }
            set { myCopyToClass = value; }
        }

        public String CopyFromClass {
            get { return myCopyFromClass; }
            set { myCopyFromClass = value; }
        }

        public String CopyGroup {
            get { return myCopyGroup; }
            set { myCopyGroup = value; }
        }

        public void showClassChangeWindow(DataRow inTourRow, String inEvent) {
            String curListCode = "";
            String curListValue = "";
            RadioButtonWithValue curRadioButton;
            int curItemLoc = 15;
            int curItemSize = 0;
            myTourRules = (String)inTourRow["Rules"];
            myTourClass = (String)inTourRow["Class"];
            mySanctionNum = (String)inTourRow["SanctionId"];
            myCopyFromClass = myTourClass;
            myCopyToClass = myTourClass;

            mySkierClassList = new ListSkierClass();
            mySkierClassList.ListSkierClassLoad();

            curRadioButton = new RadioButtonWithValue();
            curRadioButton.Parent = FromClassGroupBox;
            curRadioButton.AutoSize = true;
            curRadioButton.Location = new System.Drawing.Point( 15, curItemLoc );
            curRadioButton.Name = "All";
            curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
            curRadioButton.TabIndex = 0;
            curRadioButton.TabStop = true;
            curRadioButton.Text = "All";
            curRadioButton.UseVisualStyleBackColor = true;
            curRadioButton.Value = "All";
            curItemLoc += 17;
            curRadioButton.Click += new System.EventHandler( radioButtonFrom_Click );
            FromClassGroupBox.Controls.Add( curRadioButton );

            foreach (ListItem curEntry in mySkierClassList.DropdownList) {
                curListCode = (String)curEntry.ItemName;
                curListValue = (String)curEntry.ItemValue;
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.Parent = FromClassGroupBox;
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 15, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = 0;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListCode;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListValue;
                if (curListValue.ToLower().Equals( myTourClass.ToLower() )) {
                    curRadioButton.Checked = true;
                }
                curItemLoc += 17;
                curRadioButton.Click += new System.EventHandler( radioButtonFrom_Click );
                FromClassGroupBox.Controls.Add( curRadioButton );
            }

            curItemLoc = 15;
            foreach (ListItem curEntry in mySkierClassList.DropdownList) {
                curListCode = (String)curEntry.ItemName;
                curListValue = (String)curEntry.ItemValue;
                curItemSize = curListValue.Length * 8;

                curRadioButton = new RadioButtonWithValue();
                curRadioButton.Parent = ToClassGroupBox;
                curRadioButton.AutoSize = true;
                curRadioButton.Location = new System.Drawing.Point( 15, curItemLoc );
                curRadioButton.Name = curListValue;
                curRadioButton.Size = new System.Drawing.Size( curItemSize, 15 );
                curRadioButton.TabIndex = 0;
                curRadioButton.TabStop = true;
                curRadioButton.Text = curListCode;
                curRadioButton.UseVisualStyleBackColor = true;
                curRadioButton.Value = curListValue;
                if (curListValue.ToLower().Equals( myTourClass.ToLower() )) {
                    curRadioButton.Checked = true;
                }
                curItemLoc += 17;
                curRadioButton.Click += new System.EventHandler( radioButtonTo_Click );
                ToClassGroupBox.Controls.Add( curRadioButton );

                loadEventGroupList();
            }
        }

        private void radioButtonFrom_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            String rbValue = (String)myRadio.Value ;
            this.Tag = rbValue;
            myCopyFromClass = rbValue;
        }

        private void radioButtonTo_Click(object sender, EventArgs e) {
            RadioButtonWithValue myRadio = (RadioButtonWithValue)sender;
            String rbValue = (String)myRadio.Value;
            this.Tag = rbValue;
            myCopyToClass = rbValue;
        }

        private void loadEventGroupList() {
            if (EventGroupList.DataSource == null) {
                if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    ArrayList curEventGroupList = new ArrayList();
                    curEventGroupList.Add( "All" );
                    curEventGroupList.Add( "Men A" );
                    curEventGroupList.Add( "Women A" );
                    curEventGroupList.Add( "Men B" );
                    curEventGroupList.Add( "Women B" );
                    curEventGroupList.Add( "Non Team" );
                    EventGroupList.DataSource = curEventGroupList;
                } else {
                    loadEventGroupListFromData();
                }
            } else {
                if (( (ArrayList)EventGroupList.DataSource ).Count > 0) {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                    } else {
                        loadEventGroupListFromData();
                    }
                } else {
                    if (myTourRules.ToLower().Equals( "ncwsa" )) {
                        ArrayList curEventGroupList = new ArrayList();
                        curEventGroupList.Add( "All" );
                        curEventGroupList.Add( "Men A" );
                        curEventGroupList.Add( "Women A" );
                        curEventGroupList.Add( "Men B" );
                        curEventGroupList.Add( "Women B" );
                        curEventGroupList.Add( "Non Team" );
                        EventGroupList.DataSource = curEventGroupList;
                    } else {
                        loadEventGroupListFromData();
                    }
                }
            }
        }

        private void loadEventGroupListFromData() {
            String curGroupValue = "";
            ArrayList curEventGroupList = new ArrayList();
            String curSqlStmt = "";
            curEventGroupList.Add( "All" );
            curSqlStmt = "SELECT DISTINCT EventGroup From EventRunOrder "
                + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom' And Round = 1 "
                + "Order by EventGroup";
            DataTable curDataTable = getData( curSqlStmt );
            if (curDataTable.Rows.Count == 0) {
                curSqlStmt = "SELECT DISTINCT EventGroup FROM EventReg "
                    + "WHERE SanctionId = '" + mySanctionNum + "' And Event = 'Slalom'"
                    + "Order by EventGroup";
                curDataTable = getData( curSqlStmt );
            }

            foreach (DataRow curRow in curDataTable.Rows) {
                curEventGroupList.Add( (String)curRow["EventGroup"] );
            }
            EventGroupList.DataSource = curEventGroupList;
            if (curGroupValue.Length > 0) {
                foreach (String curValue in (ArrayList)EventGroupList.DataSource) {
                    if (curValue.Equals( curGroupValue )) {
                        EventGroupList.SelectedItem = curGroupValue;
                        EventGroupList.Text = curGroupValue;
                        return;
                    }
                }
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            } else {
                EventGroupList.SelectedItem = "All";
                EventGroupList.Text = "All";
            }
        }

        private DataTable getData(String inSelectStmt) {
            return DataAccess.getDataTable( inSelectStmt );
        }

        private void EventGroupList_SelectedIndexChanged(object sender, EventArgs e) {
            int curIdx = EventGroupList.SelectedIndex;
            myCopyGroup = EventGroupList.SelectedItem.ToString();
        }

    }
}
