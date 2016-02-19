namespace WaterskiScoringSystem.Tools {
    partial class ListMaintenance {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListMaintenance));
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ListCodeNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SortSeq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MinValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MaxValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CodeDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.listNameDropdown = new System.Windows.Forms.ComboBox();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navExportAll = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.winStatus.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            this.DataGridView.AllowUserToResizeRows = false;
            this.DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.DataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.ListName,
            this.ListCode,
            this.ListCodeNum,
            this.SortSeq,
            this.CodeValue,
            this.MinValue,
            this.MaxValue,
            this.CodeDesc});
            this.DataGridView.Location = new System.Drawing.Point(5, 73);
            this.DataGridView.Name = "DataGridView";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView.RowHeadersWidth = 30;
            this.DataGridView.Size = new System.Drawing.Size(800, 298);
            this.DataGridView.TabIndex = 2;
            this.DataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
            this.DataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            // 
            // PK
            // 
            this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            this.PK.ReadOnly = true;
            this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PK.ToolTipText = "Primary Key";
            this.PK.Visible = false;
            this.PK.Width = 40;
            // 
            // ListName
            // 
            this.ListName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ListName.HeaderText = "List Name";
            this.ListName.Name = "ListName";
            this.ListName.ReadOnly = true;
            this.ListName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ListName.ToolTipText = "Name of list";
            this.ListName.Width = 73;
            // 
            // ListCode
            // 
            this.ListCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ListCode.HeaderText = "List Entry Code";
            this.ListCode.Name = "ListCode";
            this.ListCode.ReadOnly = true;
            this.ListCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ListCode.ToolTipText = "Code for entry of list";
            this.ListCode.Width = 95;
            // 
            // ListCodeNum
            // 
            this.ListCodeNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ListCodeNum.HeaderText = "List Entry Code (Num)";
            this.ListCodeNum.Name = "ListCodeNum";
            this.ListCodeNum.ReadOnly = true;
            this.ListCodeNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ListCodeNum.ToolTipText = "Code for entry of list as a numeric";
            this.ListCodeNum.Width = 97;
            // 
            // SortSeq
            // 
            this.SortSeq.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.SortSeq.HeaderText = "Sort Seq";
            this.SortSeq.Name = "SortSeq";
            this.SortSeq.ReadOnly = true;
            this.SortSeq.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SortSeq.ToolTipText = "Sort sequence for entry within list";
            this.SortSeq.Width = 51;
            // 
            // CodeValue
            // 
            this.CodeValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.CodeValue.HeaderText = "Entry Text Value";
            this.CodeValue.Name = "CodeValue";
            this.CodeValue.ReadOnly = true;
            this.CodeValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CodeValue.ToolTipText = "Text value for list entry code";
            this.CodeValue.Width = 101;
            // 
            // MinValue
            // 
            this.MinValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MinValue.HeaderText = "Min Value";
            this.MinValue.Name = "MinValue";
            this.MinValue.ReadOnly = true;
            this.MinValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MinValue.ToolTipText = "Minimum value for an entry range";
            this.MinValue.Width = 73;
            // 
            // MaxValue
            // 
            this.MaxValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.MaxValue.HeaderText = "Max Value";
            this.MaxValue.Name = "MaxValue";
            this.MaxValue.ReadOnly = true;
            this.MaxValue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MaxValue.ToolTipText = "Minimum value for an entry range";
            this.MaxValue.Width = 76;
            // 
            // CodeDesc
            // 
            this.CodeDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CodeDesc.HeaderText = "Entry Description";
            this.CodeDesc.Name = "CodeDesc";
            this.CodeDesc.ReadOnly = true;
            this.CodeDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CodeDesc.ToolTipText = "Description for entry";
            this.CodeDesc.Width = 150;
            // 
            // listNameDropdown
            // 
            this.listNameDropdown.DisplayMember = "ListName";
            this.listNameDropdown.FormattingEnabled = true;
            this.listNameDropdown.Location = new System.Drawing.Point(174, 46);
            this.listNameDropdown.Name = "listNameDropdown";
            this.listNameDropdown.Size = new System.Drawing.Size(200, 21);
            this.listNameDropdown.TabIndex = 1;
            this.listNameDropdown.ValueMember = "ListName";
            this.listNameDropdown.SelectedIndexChanged += new System.EventHandler(this.listNameComboBox_SelectedIndexChanged);
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 374);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(811, 22);
            this.winStatus.TabIndex = 3;
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
            this.RowStatusLabel.Location = new System.Drawing.Point(5, 49);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 8;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navExport,
            this.navExportAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(811, 38);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // navExport
            // 
            this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size(44, 35);
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExport.Click += new System.EventHandler(this.navExport_Click);
            // 
            // navExportAll
            // 
            this.navExportAll.Image = ((System.Drawing.Image)(resources.GetObject("navExportAll.Image")));
            this.navExportAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExportAll.Name = "navExportAll";
            this.navExportAll.Size = new System.Drawing.Size(61, 35);
            this.navExportAll.Text = "Export All";
            this.navExportAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExportAll.Click += new System.EventHandler(this.navExportAll_Click);
            // 
            // ListMaintenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 396);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.listNameDropdown);
            this.Controls.Add(this.DataGridView);
            this.Name = "ListMaintenance";
            this.Text = "List View";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListMaintenance_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ListMaintenance_FormClosed);
            this.Load += new System.EventHandler(this.ListMaintenance_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Common.SortDialogForm sortDialogForm;
        private Common.FilterDialogForm filterDialogForm;
        private Tools.TourExportDialogForm exportDialogForm;
        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.ComboBox listNameDropdown;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ListCodeNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn SortSeq;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn MinValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn MaxValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn CodeDesc;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navExportAll;
    }
}