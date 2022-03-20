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
			this.navMainMenu.Size = new System.Drawing.Size(985, 38);
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
			this.navExport.Size = new System.Drawing.Size(45, 35);
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
			this.navAddNewItem.Enabled = false;
			this.navAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("navAddNewItem.Image")));
			this.navAddNewItem.Name = "navAddNewItem";
			this.navAddNewItem.RightToLeftAutoMirrorImage = true;
			this.navAddNewItem.Size = new System.Drawing.Size(40, 35);
			this.navAddNewItem.Text = "Insert";
			this.navAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navAddNewItem.Visible = false;
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
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView.Location = new System.Drawing.Point(8, 66);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.Size = new System.Drawing.Size(965, 514);
			this.dataGridView.TabIndex = 10;
			this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
			this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
			this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
			this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
			// 
			// winStatus
			// 
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 583);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(985, 22);
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
			this.ClientSize = new System.Drawing.Size(985, 605);
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
    }
}