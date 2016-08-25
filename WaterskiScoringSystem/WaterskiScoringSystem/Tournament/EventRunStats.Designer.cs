namespace WaterskiScoringSystem.Tournament {
    partial class EventRunStats {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventRunStats));
            System.Windows.Forms.Label TourStatsLabel6;
            System.Windows.Forms.Label TourStatsLabel5;
            System.Windows.Forms.Label TourStatsLabel4;
            this.scoreSummaryDataGridView = new System.Windows.Forms.DataGridView();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumSkiers = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NumPasses = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PerSkier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PerPass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.winNavStrip = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navPrint = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navExportHtml = new System.Windows.Forms.ToolStripButton();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.TotalRidesTextBox = new System.Windows.Forms.TextBox();
            this.JumpRidesTextBox = new System.Windows.Forms.TextBox();
            this.TrickRidesTextBox = new System.Windows.Forms.TextBox();
            this.SlalomRidesTextBox = new System.Windows.Forms.TextBox();
            this.TotalSkiersTextBox = new System.Windows.Forms.TextBox();
            this.TotalNumTextBox = new System.Windows.Forms.TextBox();
            this.JumpNumTextBox = new System.Windows.Forms.TextBox();
            this.TrickNumTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SlalomNumTextBox = new System.Windows.Forms.TextBox();
            this.TotalStatLabel = new System.Windows.Forms.Label();
            this.JumpStatLabel = new System.Windows.Forms.Label();
            this.TrickStatLabel = new System.Windows.Forms.Label();
            this.SlalomStatLabel = new System.Windows.Forms.Label();
            TourStatsLabel6 = new System.Windows.Forms.Label();
            TourStatsLabel5 = new System.Windows.Forms.Label();
            TourStatsLabel4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.scoreSummaryDataGridView)).BeginInit();
            this.winNavStrip.SuspendLayout();
            this.winStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // scoreSummaryDataGridView
            // 
            this.scoreSummaryDataGridView.AllowUserToAddRows = false;
            this.scoreSummaryDataGridView.AllowUserToDeleteRows = false;
            this.scoreSummaryDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.scoreSummaryDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.scoreSummaryDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.scoreSummaryDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.scoreSummaryDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.scoreSummaryDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Event,
            this.Round,
            this.EventGroup,
            this.NumSkiers,
            this.NumPasses,
            this.EventDuration,
            this.PerSkier,
            this.PerPass,
            this.StartTime,
            this.EndTime});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.scoreSummaryDataGridView.DefaultCellStyle = dataGridViewCellStyle12;
            this.scoreSummaryDataGridView.Location = new System.Drawing.Point(12, 57);
            this.scoreSummaryDataGridView.Name = "scoreSummaryDataGridView";
            this.scoreSummaryDataGridView.RowHeadersVisible = false;
            this.scoreSummaryDataGridView.Size = new System.Drawing.Size(629, 612);
            this.scoreSummaryDataGridView.TabIndex = 101;
            this.scoreSummaryDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.scoreSummaryDataGridView_RowEnter);
            // 
            // Event
            // 
            this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Event.HeaderText = "Event";
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.Width = 50;
            // 
            // Round
            // 
            this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Round.DefaultCellStyle = dataGridViewCellStyle3;
            this.Round.HeaderText = "Rd";
            this.Round.Name = "Round";
            this.Round.ReadOnly = true;
            this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Round.Width = 30;
            // 
            // EventGroup
            // 
            this.EventGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.EventGroup.DefaultCellStyle = dataGridViewCellStyle4;
            this.EventGroup.HeaderText = "Group";
            this.EventGroup.Name = "EventGroup";
            this.EventGroup.ReadOnly = true;
            this.EventGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventGroup.Width = 40;
            // 
            // NumSkiers
            // 
            this.NumSkiers.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.NumSkiers.DefaultCellStyle = dataGridViewCellStyle5;
            this.NumSkiers.HeaderText = "Skiers";
            this.NumSkiers.Name = "NumSkiers";
            this.NumSkiers.ReadOnly = true;
            this.NumSkiers.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.NumSkiers.Width = 45;
            // 
            // NumPasses
            // 
            this.NumPasses.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.NumPasses.DefaultCellStyle = dataGridViewCellStyle6;
            this.NumPasses.HeaderText = "Passes";
            this.NumPasses.Name = "NumPasses";
            this.NumPasses.ReadOnly = true;
            this.NumPasses.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.NumPasses.Width = 45;
            // 
            // EventDuration
            // 
            this.EventDuration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.EventDuration.DefaultCellStyle = dataGridViewCellStyle7;
            this.EventDuration.HeaderText = "Total Minutes";
            this.EventDuration.Name = "EventDuration";
            this.EventDuration.ReadOnly = true;
            this.EventDuration.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventDuration.Width = 80;
            // 
            // PerSkier
            // 
            this.PerSkier.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.PerSkier.DefaultCellStyle = dataGridViewCellStyle8;
            this.PerSkier.HeaderText = "Min / Skier";
            this.PerSkier.Name = "PerSkier";
            this.PerSkier.ReadOnly = true;
            this.PerSkier.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PerSkier.Width = 50;
            // 
            // PerPass
            // 
            this.PerPass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.PerPass.DefaultCellStyle = dataGridViewCellStyle9;
            this.PerPass.HeaderText = "Min / Pass";
            this.PerPass.Name = "PerPass";
            this.PerPass.ReadOnly = true;
            this.PerPass.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PerPass.Width = 50;
            // 
            // StartTime
            // 
            this.StartTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.StartTime.DefaultCellStyle = dataGridViewCellStyle10;
            this.StartTime.HeaderText = "Start Time";
            this.StartTime.Name = "StartTime";
            this.StartTime.ReadOnly = true;
            this.StartTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // EndTime
            // 
            this.EndTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.EndTime.DefaultCellStyle = dataGridViewCellStyle11;
            this.EndTime.HeaderText = "End Time";
            this.EndTime.Name = "EndTime";
            this.EndTime.ReadOnly = true;
            this.EndTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(9, 40);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 102;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // winNavStrip
            // 
            this.winNavStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navPrint,
            this.navExport,
            this.navExportHtml});
            this.winNavStrip.Location = new System.Drawing.Point(0, 0);
            this.winNavStrip.Name = "winNavStrip";
            this.winNavStrip.Size = new System.Drawing.Size(926, 38);
            this.winNavStrip.TabIndex = 103;
            this.winNavStrip.Text = "toolStrip1";
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
            // navPrint
            // 
            this.navPrint.Image = global::WaterskiScoringSystem.Properties.Resources.Printer_Network;
            this.navPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navPrint.Name = "navPrint";
            this.navPrint.Size = new System.Drawing.Size(36, 35);
            this.navPrint.Text = "Print";
            this.navPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navPrint.Click += new System.EventHandler(this.navPrint_Click);
            // 
            // navExport
            // 
            this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size(44, 35);
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExport.ToolTipText = "Export visible data to a tab delimited text file";
            this.navExport.Click += new System.EventHandler(this.navExport_Click);
            // 
            // navExportHtml
            // 
            this.navExportHtml.Image = ((System.Drawing.Image)(resources.GetObject("navExportHtml.Image")));
            this.navExportHtml.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExportHtml.Name = "navExportHtml";
            this.navExportHtml.Size = new System.Drawing.Size(38, 35);
            this.navExportHtml.Text = "Html";
            this.navExportHtml.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExportHtml.ToolTipText = "Export visible data to an HTML file";
            this.navExportHtml.Click += new System.EventHandler(this.navExportHtml_Click);
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 672);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(926, 22);
            this.winStatus.TabIndex = 104;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // TotalRidesTextBox
            // 
            this.TotalRidesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TotalRidesTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalRidesTextBox.Location = new System.Drawing.Point(835, 184);
            this.TotalRidesTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TotalRidesTextBox.Name = "TotalRidesTextBox";
            this.TotalRidesTextBox.ReadOnly = true;
            this.TotalRidesTextBox.Size = new System.Drawing.Size(74, 25);
            this.TotalRidesTextBox.TabIndex = 153;
            this.TotalRidesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // JumpRidesTextBox
            // 
            this.JumpRidesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.JumpRidesTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JumpRidesTextBox.Location = new System.Drawing.Point(835, 160);
            this.JumpRidesTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.JumpRidesTextBox.Name = "JumpRidesTextBox";
            this.JumpRidesTextBox.ReadOnly = true;
            this.JumpRidesTextBox.Size = new System.Drawing.Size(74, 25);
            this.JumpRidesTextBox.TabIndex = 152;
            this.JumpRidesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TrickRidesTextBox
            // 
            this.TrickRidesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TrickRidesTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrickRidesTextBox.Location = new System.Drawing.Point(835, 136);
            this.TrickRidesTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TrickRidesTextBox.Name = "TrickRidesTextBox";
            this.TrickRidesTextBox.ReadOnly = true;
            this.TrickRidesTextBox.Size = new System.Drawing.Size(74, 25);
            this.TrickRidesTextBox.TabIndex = 151;
            this.TrickRidesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SlalomRidesTextBox
            // 
            this.SlalomRidesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SlalomRidesTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlalomRidesTextBox.Location = new System.Drawing.Point(835, 112);
            this.SlalomRidesTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.SlalomRidesTextBox.Name = "SlalomRidesTextBox";
            this.SlalomRidesTextBox.ReadOnly = true;
            this.SlalomRidesTextBox.Size = new System.Drawing.Size(74, 25);
            this.SlalomRidesTextBox.TabIndex = 150;
            this.SlalomRidesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TotalSkiersTextBox
            // 
            this.TotalSkiersTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TotalSkiersTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalSkiersTextBox.Location = new System.Drawing.Point(762, 223);
            this.TotalSkiersTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TotalSkiersTextBox.Name = "TotalSkiersTextBox";
            this.TotalSkiersTextBox.ReadOnly = true;
            this.TotalSkiersTextBox.Size = new System.Drawing.Size(74, 25);
            this.TotalSkiersTextBox.TabIndex = 154;
            this.TotalSkiersTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TotalNumTextBox
            // 
            this.TotalNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TotalNumTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalNumTextBox.Location = new System.Drawing.Point(762, 184);
            this.TotalNumTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TotalNumTextBox.Name = "TotalNumTextBox";
            this.TotalNumTextBox.ReadOnly = true;
            this.TotalNumTextBox.Size = new System.Drawing.Size(74, 25);
            this.TotalNumTextBox.TabIndex = 149;
            this.TotalNumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // JumpNumTextBox
            // 
            this.JumpNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.JumpNumTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JumpNumTextBox.Location = new System.Drawing.Point(762, 160);
            this.JumpNumTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.JumpNumTextBox.Name = "JumpNumTextBox";
            this.JumpNumTextBox.ReadOnly = true;
            this.JumpNumTextBox.Size = new System.Drawing.Size(74, 25);
            this.JumpNumTextBox.TabIndex = 148;
            this.JumpNumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TrickNumTextBox
            // 
            this.TrickNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TrickNumTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrickNumTextBox.Location = new System.Drawing.Point(762, 136);
            this.TrickNumTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.TrickNumTextBox.Name = "TrickNumTextBox";
            this.TrickNumTextBox.ReadOnly = true;
            this.TrickNumTextBox.Size = new System.Drawing.Size(74, 25);
            this.TrickNumTextBox.TabIndex = 147;
            this.TrickNumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(650, 212);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 36);
            this.label3.TabIndex = 145;
            this.label3.Text = "Number of Participants:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SlalomNumTextBox
            // 
            this.SlalomNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SlalomNumTextBox.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlalomNumTextBox.Location = new System.Drawing.Point(762, 112);
            this.SlalomNumTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.SlalomNumTextBox.Name = "SlalomNumTextBox";
            this.SlalomNumTextBox.ReadOnly = true;
            this.SlalomNumTextBox.Size = new System.Drawing.Size(74, 25);
            this.SlalomNumTextBox.TabIndex = 146;
            this.SlalomNumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TotalStatLabel
            // 
            this.TotalStatLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TotalStatLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalStatLabel.Location = new System.Drawing.Point(650, 184);
            this.TotalStatLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TotalStatLabel.Name = "TotalStatLabel";
            this.TotalStatLabel.Size = new System.Drawing.Size(113, 25);
            this.TotalStatLabel.TabIndex = 112;
            this.TotalStatLabel.Text = "Total";
            this.TotalStatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // JumpStatLabel
            // 
            this.JumpStatLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.JumpStatLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JumpStatLabel.Location = new System.Drawing.Point(650, 160);
            this.JumpStatLabel.Margin = new System.Windows.Forms.Padding(0);
            this.JumpStatLabel.Name = "JumpStatLabel";
            this.JumpStatLabel.Size = new System.Drawing.Size(113, 25);
            this.JumpStatLabel.TabIndex = 113;
            this.JumpStatLabel.Text = "Jumping";
            this.JumpStatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TrickStatLabel
            // 
            this.TrickStatLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TrickStatLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrickStatLabel.Location = new System.Drawing.Point(650, 136);
            this.TrickStatLabel.Margin = new System.Windows.Forms.Padding(0);
            this.TrickStatLabel.Name = "TrickStatLabel";
            this.TrickStatLabel.Size = new System.Drawing.Size(113, 25);
            this.TrickStatLabel.TabIndex = 114;
            this.TrickStatLabel.Text = "Tricks";
            this.TrickStatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SlalomStatLabel
            // 
            this.SlalomStatLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SlalomStatLabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlalomStatLabel.Location = new System.Drawing.Point(650, 112);
            this.SlalomStatLabel.Margin = new System.Windows.Forms.Padding(0);
            this.SlalomStatLabel.Name = "SlalomStatLabel";
            this.SlalomStatLabel.Size = new System.Drawing.Size(113, 25);
            this.SlalomStatLabel.TabIndex = 115;
            this.SlalomStatLabel.Text = "Slalom";
            this.SlalomStatLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TourStatsLabel6
            // 
            TourStatsLabel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TourStatsLabel6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TourStatsLabel6.Location = new System.Drawing.Point(835, 59);
            TourStatsLabel6.Margin = new System.Windows.Forms.Padding(0);
            TourStatsLabel6.Name = "TourStatsLabel6";
            TourStatsLabel6.Size = new System.Drawing.Size(74, 54);
            TourStatsLabel6.TabIndex = 121;
            TourStatsLabel6.Text = "Number of Rides Per Event";
            TourStatsLabel6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // TourStatsLabel5
            // 
            TourStatsLabel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TourStatsLabel5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TourStatsLabel5.Location = new System.Drawing.Point(762, 59);
            TourStatsLabel5.Margin = new System.Windows.Forms.Padding(0);
            TourStatsLabel5.Name = "TourStatsLabel5";
            TourStatsLabel5.Size = new System.Drawing.Size(74, 54);
            TourStatsLabel5.TabIndex = 122;
            TourStatsLabel5.Text = "Number of Skiers Per Event";
            TourStatsLabel5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // TourStatsLabel4
            // 
            TourStatsLabel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            TourStatsLabel4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TourStatsLabel4.Location = new System.Drawing.Point(650, 59);
            TourStatsLabel4.Margin = new System.Windows.Forms.Padding(0);
            TourStatsLabel4.Name = "TourStatsLabel4";
            TourStatsLabel4.Size = new System.Drawing.Size(113, 54);
            TourStatsLabel4.TabIndex = 123;
            TourStatsLabel4.Text = "Events";
            TourStatsLabel4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // EventRunStats
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 694);
            this.Controls.Add(this.TotalRidesTextBox);
            this.Controls.Add(this.JumpRidesTextBox);
            this.Controls.Add(this.TrickRidesTextBox);
            this.Controls.Add(this.SlalomRidesTextBox);
            this.Controls.Add(this.TotalSkiersTextBox);
            this.Controls.Add(this.TotalNumTextBox);
            this.Controls.Add(this.JumpNumTextBox);
            this.Controls.Add(this.TrickNumTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SlalomNumTextBox);
            this.Controls.Add(this.TotalStatLabel);
            this.Controls.Add(this.JumpStatLabel);
            this.Controls.Add(this.TrickStatLabel);
            this.Controls.Add(this.SlalomStatLabel);
            this.Controls.Add(TourStatsLabel6);
            this.Controls.Add(TourStatsLabel5);
            this.Controls.Add(TourStatsLabel4);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.winNavStrip);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.scoreSummaryDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EventRunStats";
            this.Text = "Event Statistics";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EventRunStats_FormClosed);
            this.Load += new System.EventHandler(this.EventRunStats_Load);
            ((System.ComponentModel.ISupportInitialize)(this.scoreSummaryDataGridView)).EndInit();
            this.winNavStrip.ResumeLayout(false);
            this.winNavStrip.PerformLayout();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView scoreSummaryDataGridView;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.ToolStrip winNavStrip;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navExportHtml;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn Round;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumSkiers;
        private System.Windows.Forms.DataGridViewTextBoxColumn NumPasses;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn PerSkier;
        private System.Windows.Forms.DataGridViewTextBoxColumn PerPass;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
        private System.Windows.Forms.TextBox TotalRidesTextBox;
        private System.Windows.Forms.TextBox JumpRidesTextBox;
        private System.Windows.Forms.TextBox TrickRidesTextBox;
        private System.Windows.Forms.TextBox SlalomRidesTextBox;
        private System.Windows.Forms.TextBox TotalSkiersTextBox;
        private System.Windows.Forms.TextBox TotalNumTextBox;
        private System.Windows.Forms.TextBox JumpNumTextBox;
        private System.Windows.Forms.TextBox TrickNumTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox SlalomNumTextBox;
        private System.Windows.Forms.Label TotalStatLabel;
        private System.Windows.Forms.Label JumpStatLabel;
        private System.Windows.Forms.Label TrickStatLabel;
        private System.Windows.Forms.Label SlalomStatLabel;
    }
}