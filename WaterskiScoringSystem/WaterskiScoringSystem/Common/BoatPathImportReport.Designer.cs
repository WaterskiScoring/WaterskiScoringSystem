
namespace WaterskiScoringSystem.Common {
	partial class BoatPathImportReport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoatPathImportReport));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			this.winNavStrip = new System.Windows.Forms.ToolStrip();
			this.navRefresh = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.navSave = new System.Windows.Forms.ToolStripButton();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierBoatPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassSpeedKph = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PassLineLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierRunNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Boat = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DriverMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DriverName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevBuoy6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevCum6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PathDevZone6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RerideNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winNavStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.winStatus.SuspendLayout();
			this.SuspendLayout();
			// 
			// winNavStrip
			// 
			this.winNavStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.winNavStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navExport,
            this.navSave});
			this.winNavStrip.Location = new System.Drawing.Point(0, 0);
			this.winNavStrip.Name = "winNavStrip";
			this.winNavStrip.Size = new System.Drawing.Size(1080, 42);
			this.winNavStrip.TabIndex = 105;
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
            this.EventClass,
            this.Round,
            this.PassNumber,
            this.SkierBoatPath,
            this.PassSpeedKph,
            this.PassLineLength,
            this.SkierRunNum,
            this.Boat,
            this.DriverMemberId,
            this.DriverName,
            this.PathDevBuoy0,
            this.PathDevCum0,
            this.PathDevZone0,
            this.PathDevBuoy1,
            this.PathDevCum1,
            this.PathDevZone1,
            this.PathDevBuoy2,
            this.PathDevCum2,
            this.PathDevZone2,
            this.PathDevBuoy3,
            this.PathDevCum3,
            this.PathDevZone3,
            this.PathDevBuoy4,
            this.PathDevCum4,
            this.PathDevZone4,
            this.PathDevBuoy5,
            this.PathDevCum5,
            this.PathDevZone5,
            this.PathDevBuoy6,
            this.PathDevCum6,
            this.PathDevZone6,
            this.InsertDate,
            this.LastUpdateDate,
            this.RerideNote});
			this.dataGridView.Location = new System.Drawing.Point(5, 61);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(1070, 542);
			this.dataGridView.TabIndex = 106;
			this.dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
			this.dataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValidated);
			this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(5, 44);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 107;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			// 
			// winStatus
			// 
			this.winStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 606);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(1080, 22);
			this.winStatus.TabIndex = 108;
			this.winStatus.Text = "statusStrip1";
			// 
			// winStatusMsg
			// 
			this.winStatusMsg.Name = "winStatusMsg";
			this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
			// 
			// PK
			// 
			this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PK.HeaderText = "PK";
			this.PK.Name = "PK";
			this.PK.ReadOnly = true;
			this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PK.Visible = false;
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
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.EventClass.DefaultCellStyle = dataGridViewCellStyle2;
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
			// SkierBoatPath
			// 
			this.SkierBoatPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierBoatPath.HeaderText = "Path";
			this.SkierBoatPath.MaxInputLength = 8;
			this.SkierBoatPath.Name = "SkierBoatPath";
			this.SkierBoatPath.ReadOnly = true;
			this.SkierBoatPath.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SkierBoatPath.Width = 45;
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
			// PathDevBuoy0
			// 
			this.PathDevBuoy0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy0.HeaderText = "Dev Gate";
			this.PathDevBuoy0.Name = "PathDevBuoy0";
			this.PathDevBuoy0.ReadOnly = true;
			this.PathDevBuoy0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy0.Width = 50;
			// 
			// PathDevCum0
			// 
			this.PathDevCum0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum0.HeaderText = "Cum Gate";
			this.PathDevCum0.Name = "PathDevCum0";
			this.PathDevCum0.ReadOnly = true;
			this.PathDevCum0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum0.Width = 50;
			// 
			// PathDevZone0
			// 
			this.PathDevZone0.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone0.HeaderText = "Zone Gate";
			this.PathDevZone0.Name = "PathDevZone0";
			this.PathDevZone0.ReadOnly = true;
			this.PathDevZone0.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone0.Width = 50;
			// 
			// PathDevBuoy1
			// 
			this.PathDevBuoy1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy1.HeaderText = "Dev B1";
			this.PathDevBuoy1.Name = "PathDevBuoy1";
			this.PathDevBuoy1.ReadOnly = true;
			this.PathDevBuoy1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy1.Width = 50;
			// 
			// PathDevCum1
			// 
			this.PathDevCum1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum1.HeaderText = "Cum B1";
			this.PathDevCum1.Name = "PathDevCum1";
			this.PathDevCum1.ReadOnly = true;
			this.PathDevCum1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum1.Width = 50;
			// 
			// PathDevZone1
			// 
			this.PathDevZone1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone1.HeaderText = "Zone B1";
			this.PathDevZone1.Name = "PathDevZone1";
			this.PathDevZone1.ReadOnly = true;
			this.PathDevZone1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone1.Width = 50;
			// 
			// PathDevBuoy2
			// 
			this.PathDevBuoy2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy2.HeaderText = "Dev B2";
			this.PathDevBuoy2.Name = "PathDevBuoy2";
			this.PathDevBuoy2.ReadOnly = true;
			this.PathDevBuoy2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy2.Width = 50;
			// 
			// PathDevCum2
			// 
			this.PathDevCum2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum2.HeaderText = "Cum B2";
			this.PathDevCum2.Name = "PathDevCum2";
			this.PathDevCum2.ReadOnly = true;
			this.PathDevCum2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum2.Width = 50;
			// 
			// PathDevZone2
			// 
			this.PathDevZone2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone2.HeaderText = "Zone B2";
			this.PathDevZone2.Name = "PathDevZone2";
			this.PathDevZone2.ReadOnly = true;
			this.PathDevZone2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone2.Width = 50;
			// 
			// PathDevBuoy3
			// 
			this.PathDevBuoy3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy3.HeaderText = "Dev B3";
			this.PathDevBuoy3.Name = "PathDevBuoy3";
			this.PathDevBuoy3.ReadOnly = true;
			this.PathDevBuoy3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy3.Width = 50;
			// 
			// PathDevCum3
			// 
			this.PathDevCum3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum3.HeaderText = "Cum B3";
			this.PathDevCum3.Name = "PathDevCum3";
			this.PathDevCum3.ReadOnly = true;
			this.PathDevCum3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum3.Width = 50;
			// 
			// PathDevZone3
			// 
			this.PathDevZone3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone3.HeaderText = "Zone B3";
			this.PathDevZone3.Name = "PathDevZone3";
			this.PathDevZone3.ReadOnly = true;
			this.PathDevZone3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone3.Width = 50;
			// 
			// PathDevBuoy4
			// 
			this.PathDevBuoy4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy4.HeaderText = "Dev B4";
			this.PathDevBuoy4.Name = "PathDevBuoy4";
			this.PathDevBuoy4.ReadOnly = true;
			this.PathDevBuoy4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy4.Width = 50;
			// 
			// PathDevCum4
			// 
			this.PathDevCum4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum4.HeaderText = "Cum B4";
			this.PathDevCum4.Name = "PathDevCum4";
			this.PathDevCum4.ReadOnly = true;
			this.PathDevCum4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum4.Width = 50;
			// 
			// PathDevZone4
			// 
			this.PathDevZone4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone4.HeaderText = "Zone B4";
			this.PathDevZone4.Name = "PathDevZone4";
			this.PathDevZone4.ReadOnly = true;
			this.PathDevZone4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone4.Width = 50;
			// 
			// PathDevBuoy5
			// 
			this.PathDevBuoy5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy5.HeaderText = "Dev B5";
			this.PathDevBuoy5.Name = "PathDevBuoy5";
			this.PathDevBuoy5.ReadOnly = true;
			this.PathDevBuoy5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy5.Width = 50;
			// 
			// PathDevCum5
			// 
			this.PathDevCum5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum5.HeaderText = "Cum B5";
			this.PathDevCum5.Name = "PathDevCum5";
			this.PathDevCum5.ReadOnly = true;
			this.PathDevCum5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum5.Width = 50;
			// 
			// PathDevZone5
			// 
			this.PathDevZone5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone5.HeaderText = "Zone B5";
			this.PathDevZone5.Name = "PathDevZone5";
			this.PathDevZone5.ReadOnly = true;
			this.PathDevZone5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone5.Width = 50;
			// 
			// PathDevBuoy6
			// 
			this.PathDevBuoy6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevBuoy6.HeaderText = "Dev B6";
			this.PathDevBuoy6.Name = "PathDevBuoy6";
			this.PathDevBuoy6.ReadOnly = true;
			this.PathDevBuoy6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevBuoy6.Width = 50;
			// 
			// PathDevCum6
			// 
			this.PathDevCum6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevCum6.HeaderText = "Cum B6";
			this.PathDevCum6.Name = "PathDevCum6";
			this.PathDevCum6.ReadOnly = true;
			this.PathDevCum6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevCum6.Width = 50;
			// 
			// PathDevZone6
			// 
			this.PathDevZone6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PathDevZone6.HeaderText = "Zone B6";
			this.PathDevZone6.Name = "PathDevZone6";
			this.PathDevZone6.ReadOnly = true;
			this.PathDevZone6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PathDevZone6.Width = 50;
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
			// RerideNote
			// 
			this.RerideNote.HeaderText = "RerideNote";
			this.RerideNote.Name = "RerideNote";
			this.RerideNote.ReadOnly = true;
			// 
			// BoatPathImportReport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1080, 628);
			this.Controls.Add(this.winStatus);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.winNavStrip);
			this.Name = "BoatPathImportReport";
			this.Text = "Boat Path Import Report";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BoatPathImportReport_FormClosed);
			this.Load += new System.EventHandler(this.BoatPathImportReport_Load);
			this.winNavStrip.ResumeLayout(false);
			this.winNavStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.winStatus.ResumeLayout(false);
			this.winStatus.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStrip winNavStrip;
		private System.Windows.Forms.ToolStripButton navRefresh;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.ToolStripButton navSave;
		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventClass;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassNumber;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierBoatPath;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassSpeedKph;
		private System.Windows.Forms.DataGridViewTextBoxColumn PassLineLength;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierRunNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn Boat;
		private System.Windows.Forms.DataGridViewTextBoxColumn DriverMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn DriverName;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy0;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum0;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone0;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy2;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum2;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone2;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy3;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum3;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone3;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy4;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum4;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone4;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy5;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum5;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone5;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevBuoy6;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevCum6;
		private System.Windows.Forms.DataGridViewTextBoxColumn PathDevZone6;
		private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn RerideNote;
	}
}