
namespace WaterskiScoringSystem.Common {
	partial class BoatTimeImportReport {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoatTimeImportReport));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.winNavStrip = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navSave = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PassNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PassSpeedKph = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PassLineLength = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierRunNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatTimeBuoy7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winNavStrip.SuspendLayout();
            this.winStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
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
            this.winNavStrip.Size = new System.Drawing.Size(1039, 42);
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
            this.navExport.Size = new System.Drawing.Size(44, 39);
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
            // winStatus
            // 
            this.winStatus.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 713);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(1039, 22);
            this.winStatus.TabIndex = 111;
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
            this.RowStatusLabel.Location = new System.Drawing.Point(5, 43);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 110;
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
            this.PassSpeedKph,
            this.PassLineLength,
            this.SkierRunNum,
            this.BoatTimeBuoy1,
            this.BoatTimeBuoy2,
            this.BoatTimeBuoy3,
            this.BoatTimeBuoy4,
            this.BoatTimeBuoy5,
            this.BoatTimeBuoy6,
            this.BoatTimeBuoy7,
            this.InsertDate,
            this.LastUpdateDate});
            this.dataGridView.Location = new System.Drawing.Point(5, 60);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(1022, 650);
            this.dataGridView.TabIndex = 109;
            this.dataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellEnter);
            this.dataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellValidated);
            this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
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
            this.Event.MaxInputLength = 12;
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Event.Width = 45;
            // 
            // Round
            // 
            this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Round.DefaultCellStyle = dataGridViewCellStyle2;
            this.Round.HeaderText = "Rd";
            this.Round.MaxInputLength = 2;
            this.Round.Name = "Round";
            this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Round.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Round.Width = 30;
            // 
            // PassNumber
            // 
            this.PassNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PassNumber.DefaultCellStyle = dataGridViewCellStyle3;
            this.PassNumber.HeaderText = "Pass";
            this.PassNumber.MaxInputLength = 2;
            this.PassNumber.Name = "PassNumber";
            this.PassNumber.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PassNumber.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PassNumber.Width = 35;
            // 
            // PassSpeedKph
            // 
            this.PassSpeedKph.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Format = "N0";
            dataGridViewCellStyle4.NullValue = null;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PassSpeedKph.DefaultCellStyle = dataGridViewCellStyle4;
            this.PassSpeedKph.HeaderText = "Speed Kph";
            this.PassSpeedKph.MaxInputLength = 2;
            this.PassSpeedKph.Name = "PassSpeedKph";
            this.PassSpeedKph.ReadOnly = true;
            this.PassSpeedKph.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PassSpeedKph.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PassSpeedKph.Width = 50;
            // 
            // PassLineLength
            // 
            this.PassLineLength.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PassLineLength.HeaderText = "Line Length";
            this.PassLineLength.MaxInputLength = 16;
            this.PassLineLength.Name = "PassLineLength";
            this.PassLineLength.ReadOnly = true;
            this.PassLineLength.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PassLineLength.Width = 50;
            // 
            // SkierRunNum
            // 
            this.SkierRunNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SkierRunNum.HeaderText = "Match To Pass";
            this.SkierRunNum.MaxInputLength = 12;
            this.SkierRunNum.Name = "SkierRunNum";
            this.SkierRunNum.ReadOnly = true;
            this.SkierRunNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkierRunNum.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SkierRunNum.Width = 60;
            // 
            // BoatTimeBuoy1
            // 
            this.BoatTimeBuoy1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = null;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy1.DefaultCellStyle = dataGridViewCellStyle5;
            this.BoatTimeBuoy1.HeaderText = "Time Buoy 1";
            this.BoatTimeBuoy1.MaxInputLength = 8;
            this.BoatTimeBuoy1.Name = "BoatTimeBuoy1";
            this.BoatTimeBuoy1.ReadOnly = true;
            this.BoatTimeBuoy1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy1.Width = 50;
            // 
            // BoatTimeBuoy2
            // 
            this.BoatTimeBuoy2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = null;
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy2.DefaultCellStyle = dataGridViewCellStyle6;
            this.BoatTimeBuoy2.HeaderText = "Time Buoy 2";
            this.BoatTimeBuoy2.MaxInputLength = 8;
            this.BoatTimeBuoy2.Name = "BoatTimeBuoy2";
            this.BoatTimeBuoy2.ReadOnly = true;
            this.BoatTimeBuoy2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy2.Width = 50;
            // 
            // BoatTimeBuoy3
            // 
            this.BoatTimeBuoy3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            dataGridViewCellStyle7.NullValue = null;
            dataGridViewCellStyle7.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy3.DefaultCellStyle = dataGridViewCellStyle7;
            this.BoatTimeBuoy3.HeaderText = "Time Buoy 3";
            this.BoatTimeBuoy3.MaxInputLength = 8;
            this.BoatTimeBuoy3.Name = "BoatTimeBuoy3";
            this.BoatTimeBuoy3.ReadOnly = true;
            this.BoatTimeBuoy3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy3.Width = 50;
            // 
            // BoatTimeBuoy4
            // 
            this.BoatTimeBuoy4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            dataGridViewCellStyle8.NullValue = null;
            dataGridViewCellStyle8.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy4.DefaultCellStyle = dataGridViewCellStyle8;
            this.BoatTimeBuoy4.HeaderText = "Time Buoy 4";
            this.BoatTimeBuoy4.MaxInputLength = 8;
            this.BoatTimeBuoy4.Name = "BoatTimeBuoy4";
            this.BoatTimeBuoy4.ReadOnly = true;
            this.BoatTimeBuoy4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy4.Width = 50;
            // 
            // BoatTimeBuoy5
            // 
            this.BoatTimeBuoy5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N2";
            dataGridViewCellStyle9.NullValue = null;
            dataGridViewCellStyle9.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy5.DefaultCellStyle = dataGridViewCellStyle9;
            this.BoatTimeBuoy5.HeaderText = "Time Buoy 5";
            this.BoatTimeBuoy5.MaxInputLength = 8;
            this.BoatTimeBuoy5.Name = "BoatTimeBuoy5";
            this.BoatTimeBuoy5.ReadOnly = true;
            this.BoatTimeBuoy5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy5.Width = 50;
            // 
            // BoatTimeBuoy6
            // 
            this.BoatTimeBuoy6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "N2";
            dataGridViewCellStyle10.NullValue = null;
            dataGridViewCellStyle10.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy6.DefaultCellStyle = dataGridViewCellStyle10;
            this.BoatTimeBuoy6.HeaderText = "Time Buoy 6";
            this.BoatTimeBuoy6.MaxInputLength = 8;
            this.BoatTimeBuoy6.Name = "BoatTimeBuoy6";
            this.BoatTimeBuoy6.ReadOnly = true;
            this.BoatTimeBuoy6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy6.Width = 50;
            // 
            // BoatTimeBuoy7
            // 
            this.BoatTimeBuoy7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle11.Format = "N2";
            dataGridViewCellStyle11.NullValue = null;
            dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy7.DefaultCellStyle = dataGridViewCellStyle11;
            this.BoatTimeBuoy7.HeaderText = "Time Exit";
            this.BoatTimeBuoy7.MaxInputLength = 8;
            this.BoatTimeBuoy7.Name = "BoatTimeBuoy7";
            this.BoatTimeBuoy7.ReadOnly = true;
            this.BoatTimeBuoy7.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BoatTimeBuoy7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BoatTimeBuoy7.Width = 50;
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
            // BoatTimeImportReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1039, 735);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.winNavStrip);
            this.Name = "BoatTimeImportReport";
            this.Text = "Boat Time Import Report";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BoatTimeImportReport_FormClosed);
            this.Load += new System.EventHandler(this.BoatTimeImportReport_Load);
            this.winNavStrip.ResumeLayout(false);
            this.winNavStrip.PerformLayout();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip winNavStrip;
		private System.Windows.Forms.ToolStripButton navRefresh;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.ToolStripButton navSave;
		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierMemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn Round;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassSpeedKph;
        private System.Windows.Forms.DataGridViewTextBoxColumn PassLineLength;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierRunNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy1;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy2;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy3;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy4;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy5;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy6;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatTimeBuoy7;
        private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
    }
}