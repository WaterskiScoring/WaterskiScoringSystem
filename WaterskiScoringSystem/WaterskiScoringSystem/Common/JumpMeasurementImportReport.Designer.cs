
namespace WaterskiScoringSystem.Common {
	partial class JumpMeasurementImportReport {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JumpMeasurementImportReport));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.navSave = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.winNavStrip = new System.Windows.Forms.ToolStrip();
			this.navRefresh = new System.Windows.Forms.ToolStripButton();
			this.navFilter = new System.Windows.Forms.ToolStripButton();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierRunNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ScoreFeet = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ScoreMeters = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.winNavStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// winStatus
			// 
			this.winStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 645);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(901, 22);
			this.winStatus.TabIndex = 114;
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
			this.RowStatusLabel.Location = new System.Drawing.Point(5, 47);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 113;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.SanctionId,
            this.SkierMemberId,
            this.SkierName,
            this.Event,
            this.Round,
            this.PassNumber,
            this.SkierRunNum,
            this.ScoreFeet,
            this.ScoreMeters,
            this.InsertDate,
            this.LastUpdateDate});
			this.dataGridView.Location = new System.Drawing.Point(5, 64);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(884, 578);
			this.dataGridView.TabIndex = 112;
			this.dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
			this.dataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValidated);
			this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
			// 
			// navSave
			// 
			this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
			this.navSave.Name = "navSave";
			this.navSave.Size = new System.Drawing.Size(35, 39);
			this.navSave.Text = "Save";
			this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navSave.Click += new System.EventHandler(this.navSave_Click);
			// 
			// navExport
			// 
			this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
			this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navExport.Name = "navExport";
			this.navExport.Size = new System.Drawing.Size(45, 39);
			this.navExport.Text = "Export";
			this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navExport.ToolTipText = "Export visible data to a tab delimited text file";
			this.navExport.Click += new System.EventHandler(this.navExport_Click);
			// 
			// winNavStrip
			// 
			this.winNavStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.winNavStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navExport,
            this.navSave,
            this.navFilter});
			this.winNavStrip.Location = new System.Drawing.Point(0, 0);
			this.winNavStrip.Name = "winNavStrip";
			this.winNavStrip.Size = new System.Drawing.Size(901, 42);
			this.winNavStrip.TabIndex = 115;
			this.winNavStrip.Text = "toolStrip1";
			// 
			// navRefresh
			// 
			this.navRefresh.Image = global::WaterskiScoringSystem.Properties.Resources.Terminal;
			this.navRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navRefresh.Name = "navRefresh";
			this.navRefresh.Size = new System.Drawing.Size(50, 39);
			this.navRefresh.Text = "Refresh";
			this.navRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navRefresh.Click += new System.EventHandler(this.navRefresh_Click);
			// 
			// navFilter
			// 
			this.navFilter.Image = ((System.Drawing.Image)(resources.GetObject("navFilter.Image")));
			this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navFilter.Name = "navFilter";
			this.navFilter.Size = new System.Drawing.Size(37, 39);
			this.navFilter.Text = "Filter";
			this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navFilter.ToolTipText = "Filter skier list by specified criteria using dialog window";
			this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
			// 
			// PK
			// 
			this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PK.HeaderText = "PK";
			this.PK.Name = "PK";
			this.PK.ReadOnly = true;
			this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PK.Width = 45;
			// 
			// SanctionId
			// 
			this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SanctionId.HeaderText = "SanctionId";
			this.SanctionId.Name = "SanctionId";
			this.SanctionId.ReadOnly = true;
			this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SanctionId.Visible = false;
			this.SanctionId.Width = 50;
			// 
			// SkierMemberId
			// 
			this.SkierMemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierMemberId.HeaderText = "MemberId";
			this.SkierMemberId.Name = "SkierMemberId";
			this.SkierMemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SkierMemberId.Width = 65;
			// 
			// SkierName
			// 
			this.SkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierName.HeaderText = "Skier";
			this.SkierName.Name = "SkierName";
			this.SkierName.ReadOnly = true;
			this.SkierName.Width = 125;
			// 
			// Event
			// 
			this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Event.HeaderText = "Event";
			this.Event.Name = "Event";
			this.Event.ReadOnly = true;
			this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Event.Width = 45;
			// 
			// Round
			// 
			this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.Round.DefaultCellStyle = dataGridViewCellStyle2;
			this.Round.HeaderText = "Rd";
			this.Round.Name = "Round";
			this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Round.Width = 30;
			// 
			// PassNumber
			// 
			this.PassNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.DefaultCellStyle = dataGridViewCellStyle3;
			this.PassNumber.HeaderText = "Pass";
			this.PassNumber.Name = "PassNumber";
			this.PassNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.Width = 35;
			// 
			// SkierRunNum
			// 
			this.SkierRunNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierRunNum.HeaderText = "Match To Pass";
			this.SkierRunNum.Name = "SkierRunNum";
			this.SkierRunNum.ReadOnly = true;
			this.SkierRunNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SkierRunNum.Width = 60;
			// 
			// ScoreFeet
			// 
			this.ScoreFeet.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.ScoreFeet.DefaultCellStyle = dataGridViewCellStyle4;
			this.ScoreFeet.HeaderText = "Score Feet";
			this.ScoreFeet.Name = "ScoreFeet";
			this.ScoreFeet.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ScoreFeet.Width = 50;
			// 
			// ScoreMeters
			// 
			this.ScoreMeters.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			this.ScoreMeters.DefaultCellStyle = dataGridViewCellStyle5;
			this.ScoreMeters.HeaderText = "Score Meters";
			this.ScoreMeters.Name = "ScoreMeters";
			this.ScoreMeters.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ScoreMeters.Width = 50;
			// 
			// InsertDate
			// 
			this.InsertDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.InsertDate.HeaderText = "InsertDate";
			this.InsertDate.Name = "InsertDate";
			this.InsertDate.ReadOnly = true;
			this.InsertDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.InsertDate.Width = 120;
			// 
			// LastUpdateDate
			// 
			this.LastUpdateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.LastUpdateDate.HeaderText = "LastUpdateDate";
			this.LastUpdateDate.Name = "LastUpdateDate";
			this.LastUpdateDate.ReadOnly = true;
			this.LastUpdateDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.LastUpdateDate.Width = 120;
			// 
			// JumpMeasurementImportReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(901, 667);
			this.Controls.Add(this.winNavStrip);
			this.Controls.Add(this.winStatus);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.dataGridView);
			this.Name = "JumpMeasurementImportReport";
			this.Text = "Jump Measurement Import Report";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JumpMeasurementImportReport_FormClosed);
			this.Load += new System.EventHandler(this.JumpMeasurementImportReport_Load);
			this.winStatus.ResumeLayout(false);
			this.winStatus.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.winNavStrip.ResumeLayout(false);
			this.winNavStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.ToolStripButton navSave;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.ToolStrip winNavStrip;
		private System.Windows.Forms.ToolStripButton navRefresh;
		private System.Windows.Forms.ToolStripButton navFilter;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierRunNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn ScoreFeet;
		private System.Windows.Forms.DataGridViewTextBoxColumn ScoreMeters;
		private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
	}
}