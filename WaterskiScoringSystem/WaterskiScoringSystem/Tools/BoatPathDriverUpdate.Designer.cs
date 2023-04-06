
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winNavStrip = new System.Windows.Forms.ToolStrip();
			this.navRefresh = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.navSave = new System.Windows.Forms.ToolStripButton();
			this.navFilter = new System.Windows.Forms.ToolStripButton();
			this.label1 = new System.Windows.Forms.Label();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassSpeedKph = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassLineLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierRunNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Boat = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DriverMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus.SuspendLayout();
			this.winNavStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(-132, -32);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 111;
			this.RowStatusLabel.Text = "Row 1 of 9999";
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
			// 
			// navSave
			// 
			this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
			this.navSave.Name = "navSave";
			this.navSave.Size = new System.Drawing.Size(35, 39);
			this.navSave.Text = "Save";
			this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
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
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 14);
			this.label1.TabIndex = 115;
			this.label1.Text = "Row 1 of 9999";
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.SanctionId,
            this.SkierMemberId,
            this.SkierName,
            this.Event,
            this.EventClass,
            this.Round,
            this.PassNumber,
            this.PassSpeedKph,
            this.PassLineLength,
            this.SkierRunNum,
            this.Boat,
            this.DriverMemberId,
            this.DriverName,
            this.InsertDate,
            this.LastUpdateDate});
			this.dataGridView.Location = new System.Drawing.Point(12, 60);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(1166, 518);
			this.dataGridView.TabIndex = 114;
			this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
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
			// EventClass
			// 
			this.EventClass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.EventClass.DefaultCellStyle = dataGridViewCellStyle6;
			this.EventClass.HeaderText = "Class";
			this.EventClass.MaxInputLength = 16;
			this.EventClass.Name = "EventClass";
			this.EventClass.ReadOnly = true;
			this.EventClass.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.EventClass.Width = 40;
			// 
			// Round
			// 
			this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.Round.DefaultCellStyle = dataGridViewCellStyle7;
			this.Round.HeaderText = "Rd";
			this.Round.Name = "Round";
			this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Round.Width = 30;
			// 
			// PassNumber
			// 
			this.PassNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.DefaultCellStyle = dataGridViewCellStyle8;
			this.PassNumber.HeaderText = "Pass";
			this.PassNumber.Name = "PassNumber";
			this.PassNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PassNumber.Width = 35;
			// 
			// PassSpeedKph
			// 
			this.PassSpeedKph.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PassSpeedKph.HeaderText = "Speed Kph";
			this.PassSpeedKph.Name = "PassSpeedKph";
			this.PassSpeedKph.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PassSpeedKph.Width = 50;
			// 
			// PassLineLength
			// 
			this.PassLineLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PassLineLength.HeaderText = "Line Length";
			this.PassLineLength.Name = "PassLineLength";
			this.PassLineLength.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PassLineLength.Width = 50;
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
			this.Boat.Width = 125;
			// 
			// DriverMemberId
			// 
			this.DriverMemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DriverMemberId.HeaderText = "Driver MemberId";
			this.DriverMemberId.Name = "DriverMemberId";
			this.DriverMemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.DriverMemberId.Width = 65;
			// 
			// DriverName
			// 
			this.DriverName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.DriverName.HeaderText = "Driver Name";
			this.DriverName.Name = "DriverName";
			this.DriverName.ReadOnly = true;
			this.DriverName.Width = 125;
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
			this.Controls.Add(this.label1);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.winNavStrip);
			this.Controls.Add(this.RowStatusLabel);
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
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStrip winNavStrip;
		private System.Windows.Forms.ToolStripButton navRefresh;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.ToolStripButton navSave;
		private System.Windows.Forms.ToolStripButton navFilter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventClass;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassSpeedKph;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassLineLength;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierRunNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn Boat;
		private System.Windows.Forms.DataGridViewTextBoxColumn DriverMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn DriverName;
		private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
	}
}