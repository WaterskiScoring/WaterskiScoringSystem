using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterskiScoringSystem.Common {
    public partial class SearchMemberDialogForm : Form {
        private DataGridViewRow curSelectedRow;

        public SearchMemberDialogForm() {
            InitializeComponent();
        }

        public DataGridViewRow SelectedRow {
            get {
                return curSelectedRow;
            }
        }

        private void SearchMemberDialogForm_Load(object sender, EventArgs e) {
            inputLastName.Text = "";
            inputFirstName.Text = "";
            inputMemberId.Text = "";
            inputLastName.Focus();
        }

        private void SelectButton_Click(object sender, EventArgs e) {
            curSelectedRow = DataGridView.CurrentRow;
        }

        private void SearchButton_Click(object sender, EventArgs e) {
            this.memberListTableAdapter.FillBy(this.waterskiDataSet.MemberList, inputMemberId.Text, inputLastName.Text + "%", inputFirstName.Text + "%");
        }

        private void inputMemberId_Leave(object sender, EventArgs e) {
            if (inputMemberId.Text.Length > 0) {
                SearchButton_Click(null, null);
            }
        }

        private void inputLastName_Leave(object sender, EventArgs e) {
            if (inputLastName.Text.Length > 0) {
                SearchButton_Click(null, null);
            }
        }

        private void inputFirstName_Leave(object sender, EventArgs e) {
            if (inputFirstName.Text.Length > 0) {
                SearchButton_Click(null, null);
            }
        }

        private void DataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) {
            curSelectedRow = DataGridView.CurrentRow;
        }

    }
}
