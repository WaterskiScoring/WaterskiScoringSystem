namespace WaterskiScoringSystem.Tools {
    partial class SkierRankingList {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkierRankingList));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.jumpButton = new System.Windows.Forms.RadioButton();
            this.trickButton = new System.Windows.Forms.RadioButton();
            this.slalomButton = new System.Windows.Forms.RadioButton();
            this.OverallButton = new System.Windows.Forms.RadioButton();
            this.sortByScore = new System.Windows.Forms.CheckBox();
            this.AllButton = new System.Windows.Forms.RadioButton();
            this.topNavMenu = new System.Windows.Forms.ToolStrip();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.navSort = new System.Windows.Forms.ToolStripButton();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HCapBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HCapScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SeqNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.winStatus.SuspendLayout();
            this.topNavMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            this.DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
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
            this.SkierName,
            this.AgeGroup,
            this.MemberId,
            this.Event,
            this.Score,
            this.Rating,
            this.HCapBase,
            this.HCapScore,
            this.SeqNum,
            this.Notes});
            this.DataGridView.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.DataGridView.Location = new System.Drawing.Point(5, 64);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.RowHeadersWidth = 31;
            this.DataGridView.Size = new System.Drawing.Size(618, 362);
            this.DataGridView.TabIndex = 10;
            this.DataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValidated);
            this.DataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            this.DataGridView.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowLeave);
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 429);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(635, 22);
            this.winStatus.TabIndex = 3;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // jumpButton
            // 
            this.jumpButton.AutoSize = true;
            this.jumpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jumpButton.Location = new System.Drawing.Point(445, 41);
            this.jumpButton.Name = "jumpButton";
            this.jumpButton.Size = new System.Drawing.Size(59, 20);
            this.jumpButton.TabIndex = 4;
            this.jumpButton.TabStop = true;
            this.jumpButton.Text = "Jump";
            this.jumpButton.UseVisualStyleBackColor = true;
            this.jumpButton.CheckedChanged += new System.EventHandler(this.jumpButton_CheckedChanged);
            // 
            // trickButton
            // 
            this.trickButton.AutoSize = true;
            this.trickButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trickButton.Location = new System.Drawing.Point(375, 41);
            this.trickButton.Name = "trickButton";
            this.trickButton.Size = new System.Drawing.Size(56, 20);
            this.trickButton.TabIndex = 3;
            this.trickButton.TabStop = true;
            this.trickButton.Text = "Trick";
            this.trickButton.UseVisualStyleBackColor = true;
            this.trickButton.CheckedChanged += new System.EventHandler(this.trickButton_CheckedChanged);
            // 
            // slalomButton
            // 
            this.slalomButton.AutoSize = true;
            this.slalomButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slalomButton.Location = new System.Drawing.Point(293, 41);
            this.slalomButton.Name = "slalomButton";
            this.slalomButton.Size = new System.Drawing.Size(68, 20);
            this.slalomButton.TabIndex = 2;
            this.slalomButton.TabStop = true;
            this.slalomButton.Text = "Slalom";
            this.slalomButton.UseVisualStyleBackColor = true;
            this.slalomButton.CheckedChanged += new System.EventHandler(this.slalomButton_CheckedChanged);
            // 
            // OverallButton
            // 
            this.OverallButton.AutoSize = true;
            this.OverallButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OverallButton.Location = new System.Drawing.Point(518, 41);
            this.OverallButton.Name = "OverallButton";
            this.OverallButton.Size = new System.Drawing.Size(69, 20);
            this.OverallButton.TabIndex = 5;
            this.OverallButton.TabStop = true;
            this.OverallButton.Text = "Overall";
            this.OverallButton.UseVisualStyleBackColor = true;
            this.OverallButton.CheckedChanged += new System.EventHandler(this.OverallButton_CheckedChanged);
            // 
            // sortByScore
            // 
            this.sortByScore.AutoSize = true;
            this.sortByScore.Location = new System.Drawing.Point(133, 43);
            this.sortByScore.Name = "sortByScore";
            this.sortByScore.Size = new System.Drawing.Size(91, 17);
            this.sortByScore.TabIndex = 8;
            this.sortByScore.Text = "Sort By Score";
            this.sortByScore.UseVisualStyleBackColor = true;
            this.sortByScore.CheckedChanged += new System.EventHandler(this.sortByScore_CheckedChanged);
            // 
            // AllButton
            // 
            this.AllButton.AutoSize = true;
            this.AllButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AllButton.Location = new System.Drawing.Point(238, 41);
            this.AllButton.Name = "AllButton";
            this.AllButton.Size = new System.Drawing.Size(41, 20);
            this.AllButton.TabIndex = 1;
            this.AllButton.TabStop = true;
            this.AllButton.Text = "All";
            this.AllButton.UseVisualStyleBackColor = true;
            this.AllButton.CheckedChanged += new System.EventHandler(this.AllButton_CheckedChanged);
            // 
            // topNavMenu
            // 
            this.topNavMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navExport,
            this.navFilter,
            this.navSort});
            this.topNavMenu.Location = new System.Drawing.Point(0, 0);
            this.topNavMenu.Name = "topNavMenu";
            this.topNavMenu.Size = new System.Drawing.Size(635, 38);
            this.topNavMenu.TabIndex = 11;
            this.topNavMenu.Text = "toolStrip1";
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
            // navFilter
            // 
            this.navFilter.Enabled = false;
            this.navFilter.Image = ((System.Drawing.Image)(resources.GetObject("navFilter.Image")));
            this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFilter.Name = "navFilter";
            this.navFilter.Size = new System.Drawing.Size(37, 35);
            this.navFilter.Text = "Filter";
            this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navFilter.Visible = false;
            this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
            // 
            // navSort
            // 
            this.navSort.Enabled = false;
            this.navSort.Image = ((System.Drawing.Image)(resources.GetObject("navSort.Image")));
            this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSort.Name = "navSort";
            this.navSort.Size = new System.Drawing.Size(32, 35);
            this.navSort.Text = "Sort";
            this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSort.Visible = false;
            this.navSort.Click += new System.EventHandler(this.navSort_Click);
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(5, 44);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 12;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // PK
            // 
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            this.PK.ReadOnly = true;
            this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PK.ToolTipText = "Primary key";
            this.PK.Visible = false;
            this.PK.Width = 50;
            // 
            // SkierName
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.SkierName.DefaultCellStyle = dataGridViewCellStyle2;
            this.SkierName.HeaderText = "Skier Name";
            this.SkierName.Name = "SkierName";
            this.SkierName.ReadOnly = true;
            this.SkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkierName.ToolTipText = "Skier name";
            this.SkierName.Width = 145;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AgeGroup.DefaultCellStyle = dataGridViewCellStyle3;
            this.AgeGroup.HeaderText = "Age Group";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.Width = 40;
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.MemberId.DefaultCellStyle = dataGridViewCellStyle4;
            this.MemberId.HeaderText = "Member Id";
            this.MemberId.Name = "MemberId";
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.ToolTipText = "Member id";
            this.MemberId.Visible = false;
            this.MemberId.Width = 70;
            // 
            // Event
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Event.DefaultCellStyle = dataGridViewCellStyle5;
            this.Event.HeaderText = "Event";
            this.Event.Name = "Event";
            this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.ToolTipText = "Event";
            this.Event.Width = 55;
            // 
            // Score
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N1";
            dataGridViewCellStyle6.NullValue = null;
            this.Score.DefaultCellStyle = dataGridViewCellStyle6;
            this.Score.HeaderText = "Score";
            this.Score.Name = "Score";
            this.Score.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Score.ToolTipText = "Skier event rating score";
            this.Score.Width = 50;
            // 
            // Rating
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Rating.DefaultCellStyle = dataGridViewCellStyle7;
            this.Rating.HeaderText = "Rating";
            this.Rating.Name = "Rating";
            this.Rating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Rating.ToolTipText = "Skier event rating";
            this.Rating.Width = 50;
            // 
            // HCapBase
            // 
            this.HCapBase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N1";
            dataGridViewCellStyle8.NullValue = null;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.HCapBase.DefaultCellStyle = dataGridViewCellStyle8;
            this.HCapBase.HeaderText = "Handicap Base";
            this.HCapBase.Name = "HCapBase";
            this.HCapBase.ReadOnly = true;
            this.HCapBase.Visible = false;
            this.HCapBase.Width = 60;
            // 
            // HCapScore
            // 
            this.HCapScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N1";
            dataGridViewCellStyle9.NullValue = null;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.HCapScore.DefaultCellStyle = dataGridViewCellStyle9;
            this.HCapScore.HeaderText = "Handicap Score";
            this.HCapScore.Name = "HCapScore";
            this.HCapScore.ReadOnly = true;
            this.HCapScore.Visible = false;
            this.HCapScore.Width = 60;
            // 
            // SeqNum
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.Format = "N0";
            dataGridViewCellStyle10.NullValue = null;
            this.SeqNum.DefaultCellStyle = dataGridViewCellStyle10;
            this.SeqNum.HeaderText = "Seq Num";
            this.SeqNum.Name = "SeqNum";
            this.SeqNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SeqNum.ToolTipText = "Event sequence number";
            this.SeqNum.Visible = false;
            this.SeqNum.Width = 40;
            // 
            // Notes
            // 
            this.Notes.HeaderText = "Notes";
            this.Notes.Name = "Notes";
            this.Notes.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Notes.ToolTipText = "Notes";
            this.Notes.Width = 200;
            // 
            // SkierRankingList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 451);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.topNavMenu);
            this.Controls.Add(this.AllButton);
            this.Controls.Add(this.sortByScore);
            this.Controls.Add(this.OverallButton);
            this.Controls.Add(this.jumpButton);
            this.Controls.Add(this.trickButton);
            this.Controls.Add(this.slalomButton);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.DataGridView);
            this.Name = "SkierRankingList";
            this.Text = "SkierRankingList";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SkierRankingList_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SkierRankingList_FormClosed);
            this.Load += new System.EventHandler(this.SkierRankingList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.topNavMenu.ResumeLayout(false);
            this.topNavMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.RadioButton jumpButton;
        private System.Windows.Forms.RadioButton trickButton;
        private System.Windows.Forms.RadioButton slalomButton;
        private System.Windows.Forms.RadioButton OverallButton;
        private System.Windows.Forms.CheckBox sortByScore;
        private System.Windows.Forms.RadioButton AllButton;
        private System.Windows.Forms.ToolStrip topNavMenu;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn Score;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rating;
        private System.Windows.Forms.DataGridViewTextBoxColumn HCapBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn HCapScore;
        private System.Windows.Forms.DataGridViewTextBoxColumn SeqNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
    }
}