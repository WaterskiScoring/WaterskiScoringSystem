namespace WaterskiScoringSystem.Admin {
    partial class MemberList {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if ( disposing && ( components != null ) ) {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemberList));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.navMainMenu = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.navSort = new System.Windows.Forms.ToolStripButton();
            this.navEdit = new System.Windows.Forms.ToolStripButton();
            this.navAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.navRemoveAll = new System.Windows.Forms.ToolStripButton();
            this.navDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkiYearAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.navMainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.winStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // navMainMenu
            // 
            this.navMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navExport,
            this.navFilter,
            this.navSort,
            this.navEdit,
            this.navAddNewItem,
            this.navRemoveAll,
            this.navDeleteItem});
            this.navMainMenu.Location = new System.Drawing.Point(0, 0);
            this.navMainMenu.Name = "navMainMenu";
            this.navMainMenu.Size = new System.Drawing.Size(735, 38);
            this.navMainMenu.TabIndex = 5;
            this.navMainMenu.Text = "toolStrip1";
            // 
            // navRefresh
            // 
            this.navRefresh.Image = global::WaterskiScoringSystem.Properties.Resources.Terminal;
            this.navRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRefresh.Name = "navRefresh";
            this.navRefresh.Size = new System.Drawing.Size(50, 35);
            this.navRefresh.Text = "Refresh";
            this.navRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRefresh.Click += new System.EventHandler(this.navRefresh_Click);
            // 
            // navExport
            // 
            this.navExport.Image = global::WaterskiScoringSystem.Properties.Resources.DVDVR;
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size(44, 35);
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExport.ToolTipText = "Export member list as shown to tab delimited text file";
            this.navExport.Click += new System.EventHandler(this.navExport_Click);
            // 
            // navFilter
            // 
            this.navFilter.Image = global::WaterskiScoringSystem.Properties.Resources.clipboard;
            this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFilter.Name = "navFilter";
            this.navFilter.Size = new System.Drawing.Size(37, 35);
            this.navFilter.Text = "Filter";
            this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navFilter.ToolTipText = "Filter member list by selected criteria using dialog window";
            this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
            // 
            // navSort
            // 
            this.navSort.Image = global::WaterskiScoringSystem.Properties.Resources.REFBAR;
            this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSort.Name = "navSort";
            this.navSort.Size = new System.Drawing.Size(32, 35);
            this.navSort.Text = "Sort";
            this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSort.ToolTipText = "Sort member list by selected attributes using dialog window";
            this.navSort.Click += new System.EventHandler(this.navSort_Click);
            // 
            // navEdit
            // 
            this.navEdit.Image = ((System.Drawing.Image)(resources.GetObject("navEdit.Image")));
            this.navEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navEdit.Name = "navEdit";
            this.navEdit.Size = new System.Drawing.Size(31, 35);
            this.navEdit.Text = "Edit";
            this.navEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navEdit.Click += new System.EventHandler(this.navEdit_Click);
            // 
            // navAddNewItem
            // 
            this.navAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("navAddNewItem.Image")));
            this.navAddNewItem.Name = "navAddNewItem";
            this.navAddNewItem.RightToLeftAutoMirrorImage = true;
            this.navAddNewItem.Size = new System.Drawing.Size(40, 35);
            this.navAddNewItem.Text = "Insert";
            this.navAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navAddNewItem.Click += new System.EventHandler(this.navInsert_Click);
            // 
            // navRemoveAll
            // 
            this.navRemoveAll.Image = global::WaterskiScoringSystem.Properties.Resources.minus_sign;
            this.navRemoveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRemoveAll.Name = "navRemoveAll";
            this.navRemoveAll.Size = new System.Drawing.Size(71, 35);
            this.navRemoveAll.Text = "Remove All";
            this.navRemoveAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRemoveAll.Click += new System.EventHandler(this.navRemoveAll_Click);
            // 
            // navDeleteItem
            // 
            this.navDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("navDeleteItem.Image")));
            this.navDeleteItem.Name = "navDeleteItem";
            this.navDeleteItem.RightToLeftAutoMirrorImage = true;
            this.navDeleteItem.Size = new System.Drawing.Size(44, 35);
            this.navDeleteItem.Text = "Delete";
            this.navDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navDeleteItem.Click += new System.EventHandler(this.navRemove_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowDrop = true;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.LastName,
            this.FirstName,
            this.SkiYearAge,
            this.City,
            this.State,
            this.Federation,
            this.MemberStatus,
            this.UpdateDate});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView.Location = new System.Drawing.Point(5, 57);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.Size = new System.Drawing.Size(724, 339);
            this.dataGridView.TabIndex = 10;
            this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
            this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberId.HeaderText = "Member Id";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.Width = 80;
            // 
            // LastName
            // 
            this.LastName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.LastName.HeaderText = "Last Name";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LastName.Width = 90;
            // 
            // FirstName
            // 
            this.FirstName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FirstName.HeaderText = "First Name";
            this.FirstName.MaxInputLength = 128;
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FirstName.Width = 80;
            // 
            // SkiYearAge
            // 
            this.SkiYearAge.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.SkiYearAge.DefaultCellStyle = dataGridViewCellStyle3;
            this.SkiYearAge.HeaderText = "Ski Year Age";
            this.SkiYearAge.Name = "SkiYearAge";
            this.SkiYearAge.ReadOnly = true;
            this.SkiYearAge.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkiYearAge.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SkiYearAge.Width = 35;
            // 
            // City
            // 
            this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.ReadOnly = true;
            this.City.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.City.Width = 75;
            // 
            // State
            // 
            this.State.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.State.HeaderText = "State";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            this.State.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.State.Width = 45;
            // 
            // Federation
            // 
            this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Federation.HeaderText = "Fed";
            this.Federation.Name = "Federation";
            this.Federation.ReadOnly = true;
            this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Federation.Width = 40;
            // 
            // MemberStatus
            // 
            this.MemberStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            this.MemberStatus.DefaultCellStyle = dataGridViewCellStyle4;
            this.MemberStatus.HeaderText = "Status";
            this.MemberStatus.Name = "MemberStatus";
            this.MemberStatus.ReadOnly = true;
            this.MemberStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberStatus.Width = 65;
            // 
            // UpdateDate
            // 
            this.UpdateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.UpdateDate.HeaderText = "Update Date";
            this.UpdateDate.MaxInputLength = 10;
            this.UpdateDate.Name = "UpdateDate";
            this.UpdateDate.ReadOnly = true;
            this.UpdateDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 402);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(735, 22);
            this.winStatus.TabIndex = 7;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(5, 40);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 0;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // MemberList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 424);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.navMainMenu);
            this.Name = "MemberList";
            this.Text = "MemberList";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MemberList_FormClosed);
            this.Load += new System.EventHandler(this.MemberList_Load);
            this.navMainMenu.ResumeLayout(false);
            this.navMainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip navMainMenu;
        private System.Windows.Forms.ToolStripButton navAddNewItem;
        private System.Windows.Forms.ToolStripButton navDeleteItem;
        private System.Windows.Forms.ToolStripButton navRemoveAll;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.ToolStripButton navEdit;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkiYearAge;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdateDate;
    }
}