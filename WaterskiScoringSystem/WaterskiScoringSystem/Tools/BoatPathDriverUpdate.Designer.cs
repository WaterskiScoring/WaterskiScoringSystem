
namespace WaterskiScoringSystem.Tools {
	partial class BoatPathDriverUpdate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoatPathDriverUpdate));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winNavStrip = new System.Windows.Forms.ToolStrip();
			this.navRefresh = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.navSave = new System.Windows.Forms.ToolStripButton();
			this.navFilter = new System.Windows.Forms.ToolStripButton();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.SlalomSelectButton = new System.Windows.Forms.RadioButton();
			this.JumpSelectButton = new System.Windows.Forms.RadioButton();
			this.BPPK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BoatSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LineLen = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Score = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierRunNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Boat = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BpmsDriverId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BpmsDriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AssignedDriverId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AssignedDriver = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.UpdateDriverCheckBox = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.PassNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RerideNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus.SuspendLayout();
			this.winNavStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// winStatusMsg
			// 
			this.winStatusMsg.Name = "winStatusMsg";
			this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
			// 
			// winStatus
			// 
			this.winStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 581);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(1190, 22);
			this.winStatus.TabIndex = 112;
			this.winStatus.Text = "statusStrip1";
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
			this.winNavStrip.Size = new System.Drawing.Size(1190, 42);
			this.winNavStrip.TabIndex = 113;
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
			// navSave
			// 
			this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
			this.navSave.Name = "navSave";
			this.navSave.Size = new System.Drawing.Size(35, 39);
			this.navSave.Text = "Save";
			this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navSave.Click += new System.EventHandler(this.navSave_Click);
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
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(12, 43);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 115;
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
            this.BPPK,
            this.SanctionId,
            this.SkierMemberId,
            this.SkierName,
            this.Event,
            this.EventGroup,
            this.EventClass,
            this.Round,
            this.PassNumber,
            this.BoatSpeed,
            this.LineLen,
            this.Score,
            this.PassScore,
            this.SkierRunNum,
            this.Boat,
            this.BpmsDriverId,
            this.BpmsDriverName,
            this.AssignedDriverId,
            this.AssignedDriver,
            this.UpdateDriverCheckBox,
            this.PassNote,
            this.RerideNote,
            this.InsertDate,
            this.LastUpdateDate});
			this.dataGridView.Location = new System.Drawing.Point(12, 60);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(1166, 518);
			this.dataGridView.TabIndex = 114;
			this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
			// 
			// SlalomSelectButton
			// 
			this.SlalomSelectButton.AutoSize = true;
			this.SlalomSelectButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SlalomSelectButton.Location = new System.Drawing.Point(324, 37);
			this.SlalomSelectButton.Name = "SlalomSelectButton";
			this.SlalomSelectButton.Size = new System.Drawing.Size(75, 20);
			this.SlalomSelectButton.TabIndex = 116;
			this.SlalomSelectButton.TabStop = true;
			this.SlalomSelectButton.Text = "Slalom";
			this.SlalomSelectButton.UseVisualStyleBackColor = true;
			this.SlalomSelectButton.CheckedChanged += new System.EventHandler(this.SlalomSelectButton_CheckedChanged);
			// 
			// JumpSelectButton
			// 
			this.JumpSelectButton.AutoSize = true;
			this.JumpSelectButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.JumpSelectButton.Location = new System.Drawing.Point(415, 37);
			this.JumpSelectButton.Name = "JumpSelectButton";
			this.JumpSelectButton.Size = new System.Drawing.Size(65, 20);
			this.JumpSelectButton.TabIndex = 117;
			this.JumpSelectButton.TabStop = true;
			this.JumpSelectButton.Text = "Jump";
			this.JumpSelectButton.UseVisualStyleBackColor = true;
			this.JumpSelectButton.CheckedChanged += new System.EventHandler(this.SlalomSelectButton_CheckedChanged);
			// 
			// BPPK
			// 
			this.BPPK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.BPPK.HeaderText = "BPPK";
			this.BPPK.Name = "BPPK";
			this.BPPK.ReadOnly = true;
			this.BPPK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.BPPK.Width = 45;
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
			this.SkierMemberId.Visible = false;
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
			// EventGroup
			// 
			this.EventGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.EventGroup.HeaderText = "Grp";
			this.EventGroup.Name = "EventGroup";
			this.EventGroup.ReadOnly = true;
			this.EventGroup.Width = 45;
			// 
			// EventClass
			// 
			this.EventClass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.EventClass.DefaultCellStyle = dataGridViewCellStyle2;
			this.EventClass.HeaderText = "Cls";
			this.EventClass.MaxInputLength = 16;
			this.EventClass.Name = "EventClass";
			this.EventClass.ReadOnly = true;
			this.EventClass.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.EventClass.Width = 40;
			// 
			// Round
			// 
			this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.Round.DefaultCellStyle = dataGridViewCellStyle3;
			this.Round.HeaderText = "Rd";
			this.Round.Name = "Round";
			this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Round.Width = 30;
			// 
			// PassNumber
			// 
			this.PassNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.DefaultCellStyle = dataGridViewCellStyle4;
			this.PassNumber.HeaderText = "Pass";
			this.PassNumber.Name = "PassNumber";
			this.PassNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.Width = 35;
			// 
			// BoatSpeed
			// 
			this.BoatSpeed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.BoatSpeed.HeaderText = "Speed Kph";
			this.BoatSpeed.Name = "BoatSpeed";
			this.BoatSpeed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.BoatSpeed.Width = 50;
			// 
			// LineLen
			// 
			this.LineLen.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.LineLen.HeaderText = "Line Length";
			this.LineLen.Name = "LineLen";
			this.LineLen.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.LineLen.Width = 50;
			// 
			// Score
			// 
			this.Score.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Score.HeaderText = "Score";
			this.Score.Name = "Score";
			this.Score.ReadOnly = true;
			// 
			// PassScore
			// 
			this.PassScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PassScore.HeaderText = "PassScore";
			this.PassScore.Name = "PassScore";
			this.PassScore.ReadOnly = true;
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
			// Boat
			// 
			this.Boat.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Boat.HeaderText = "Boat";
			this.Boat.Name = "Boat";
			this.Boat.Visible = false;
			this.Boat.Width = 125;
			// 
			// BpmsDriverId
			// 
			this.BpmsDriverId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.BpmsDriverId.HeaderText = "BPMS DriverId";
			this.BpmsDriverId.Name = "BpmsDriverId";
			this.BpmsDriverId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.BpmsDriverId.Width = 65;
			// 
			// BpmsDriverName
			// 
			this.BpmsDriverName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.BpmsDriverName.HeaderText = "BPMS Driver";
			this.BpmsDriverName.Name = "BpmsDriverName";
			this.BpmsDriverName.ReadOnly = true;
			this.BpmsDriverName.Width = 125;
			// 
			// AssignedDriverId
			// 
			this.AssignedDriverId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.AssignedDriverId.HeaderText = "Assigned DriverId";
			this.AssignedDriverId.Name = "AssignedDriverId";
			this.AssignedDriverId.ReadOnly = true;
			this.AssignedDriverId.Width = 65;
			// 
			// AssignedDriver
			// 
			this.AssignedDriver.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.AssignedDriver.HeaderText = "Assigned Driver";
			this.AssignedDriver.Name = "AssignedDriver";
			this.AssignedDriver.ReadOnly = true;
			this.AssignedDriver.Width = 125;
			// 
			// UpdateDriverCheckBox
			// 
			this.UpdateDriverCheckBox.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.UpdateDriverCheckBox.HeaderText = "Upd";
			this.UpdateDriverCheckBox.Name = "UpdateDriverCheckBox";
			this.UpdateDriverCheckBox.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.UpdateDriverCheckBox.Width = 40;
			// 
			// PassNote
			// 
			this.PassNote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PassNote.HeaderText = "PassNote";
			this.PassNote.Name = "PassNote";
			this.PassNote.ReadOnly = true;
			this.PassNote.Width = 125;
			// 
			// RerideNote
			// 
			this.RerideNote.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.RerideNote.HeaderText = "RerideNote";
			this.RerideNote.Name = "RerideNote";
			this.RerideNote.ReadOnly = true;
			this.RerideNote.Width = 80;
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
			// BoatPathDriverUpdate
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1190, 603);
			this.Controls.Add(this.JumpSelectButton);
			this.Controls.Add(this.SlalomSelectButton);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.winNavStrip);
			this.Controls.Add(this.winStatus);
			this.Name = "BoatPathDriverUpdate";
			this.Text = "BoatPathDriverUpdate";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BoatPathDriverUpdate_FormClosed);
			this.Load += new System.EventHandler(this.BoatPathDriverUpdate_Load);
			this.winStatus.ResumeLayout(false);
			this.winStatus.PerformLayout();
			this.winNavStrip.ResumeLayout(false);
			this.winNavStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStrip winNavStrip;
		private System.Windows.Forms.ToolStripButton navRefresh;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.ToolStripButton navSave;
		private System.Windows.Forms.ToolStripButton navFilter;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.RadioButton SlalomSelectButton;
		private System.Windows.Forms.RadioButton JumpSelectButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn BPPK;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventClass;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn BoatSpeed;
		private System.Windows.Forms.DataGridViewTextBoxColumn LineLen;
		private System.Windows.Forms.DataGridViewTextBoxColumn Score;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassScore;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierRunNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn Boat;
		private System.Windows.Forms.DataGridViewTextBoxColumn BpmsDriverId;
		private System.Windows.Forms.DataGridViewTextBoxColumn BpmsDriverName;
		private System.Windows.Forms.DataGridViewTextBoxColumn AssignedDriverId;
		private System.Windows.Forms.DataGridViewTextBoxColumn AssignedDriver;
		private System.Windows.Forms.DataGridViewCheckBoxColumn UpdateDriverCheckBox;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassNote;
		private System.Windows.Forms.DataGridViewTextBoxColumn RerideNote;
		private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
	}
}