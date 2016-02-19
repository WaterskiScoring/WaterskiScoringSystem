namespace WaterskiScoringSystem.Trick {
    partial class TrickScorebook {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrickScorebook));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navPrint = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.PointsMethodGroupBox = new System.Windows.Forms.GroupBox();
            this.ratioPointsButton = new System.Windows.Forms.RadioButton();
            this.kBasePointsButton = new System.Windows.Forms.RadioButton();
            this.plcmtPointsButton = new System.Windows.Forms.RadioButton();
            this.nopsPointsButton = new System.Windows.Forms.RadioButton();
            this.TrickLabel = new System.Windows.Forms.Label();
            this.scoreSummaryDataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SepTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RoundTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventClassTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pass1Trick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pass2Trick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScoreTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PointsTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PlcmtTrick = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.plcmtGroupBox = new System.Windows.Forms.GroupBox();
            this.plcmtDivGrpButton = new System.Windows.Forms.RadioButton();
            this.plcmtDivButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.EventGroupList = new System.Windows.Forms.ComboBox();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.firstScoreButton = new System.Windows.Forms.RadioButton();
            this.finalScoreButton = new System.Windows.Forms.RadioButton();
            this.totalScoreButton = new System.Windows.Forms.RadioButton();
            this.bestScoreButton = new System.Windows.Forms.RadioButton();
            this.scoresPlcmtGroupBox = new System.Windows.Forms.GroupBox();
            this.winStatus.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.PointsMethodGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scoreSummaryDataGridView)).BeginInit();
            this.plcmtGroupBox.SuspendLayout();
            this.scoresPlcmtGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 437);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(796, 22);
            this.winStatus.TabIndex = 8;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navPrint,
            this.navExport});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(796, 36);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "navToolStrip";
            // 
            // navRefresh
            // 
            this.navRefresh.Image = global::WaterskiScoringSystem.Properties.Resources.Terminal;
            this.navRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRefresh.Name = "navRefresh";
            this.navRefresh.Size = new System.Drawing.Size(49, 33);
            this.navRefresh.Text = "Refresh";
            this.navRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRefresh.Click += new System.EventHandler(this.navRefresh_Click);
            // 
            // navPrint
            // 
            this.navPrint.Image = global::WaterskiScoringSystem.Properties.Resources.Printer_Network;
            this.navPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navPrint.Name = "navPrint";
            this.navPrint.Size = new System.Drawing.Size(33, 33);
            this.navPrint.Text = "Print";
            this.navPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navPrint.Click += new System.EventHandler(this.navPrint_Click);
            // 
            // navExport
            // 
            this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size(43, 33);
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExport.ToolTipText = "Export visible data to a tab delimited text file";
            this.navExport.Click += new System.EventHandler(this.navExport_Click);
            // 
            // PointsMethodGroupBox
            // 
            this.PointsMethodGroupBox.Controls.Add(this.ratioPointsButton);
            this.PointsMethodGroupBox.Controls.Add(this.kBasePointsButton);
            this.PointsMethodGroupBox.Controls.Add(this.plcmtPointsButton);
            this.PointsMethodGroupBox.Controls.Add(this.nopsPointsButton);
            this.PointsMethodGroupBox.Location = new System.Drawing.Point(4, 62);
            this.PointsMethodGroupBox.Name = "PointsMethodGroupBox";
            this.PointsMethodGroupBox.Size = new System.Drawing.Size(336, 39);
            this.PointsMethodGroupBox.TabIndex = 10;
            this.PointsMethodGroupBox.TabStop = false;
            this.PointsMethodGroupBox.Text = "Points Calculation Method ";
            // 
            // ratioPointsButton
            // 
            this.ratioPointsButton.AutoSize = true;
            this.ratioPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ratioPointsButton.Location = new System.Drawing.Point(250, 15);
            this.ratioPointsButton.Name = "ratioPointsButton";
            this.ratioPointsButton.Size = new System.Drawing.Size(77, 17);
            this.ratioPointsButton.TabIndex = 14;
            this.ratioPointsButton.Text = "Base Ratio";
            this.ratioPointsButton.UseVisualStyleBackColor = true;
            // 
            // kBasePointsButton
            // 
            this.kBasePointsButton.AutoSize = true;
            this.kBasePointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kBasePointsButton.Location = new System.Drawing.Point(161, 15);
            this.kBasePointsButton.Name = "kBasePointsButton";
            this.kBasePointsButton.Size = new System.Drawing.Size(76, 17);
            this.kBasePointsButton.TabIndex = 13;
            this.kBasePointsButton.Text = "1000 Base";
            this.kBasePointsButton.UseVisualStyleBackColor = true;
            // 
            // plcmtPointsButton
            // 
            this.plcmtPointsButton.AutoSize = true;
            this.plcmtPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plcmtPointsButton.Location = new System.Drawing.Point(73, 15);
            this.plcmtPointsButton.Margin = new System.Windows.Forms.Padding(0);
            this.plcmtPointsButton.Name = "plcmtPointsButton";
            this.plcmtPointsButton.Size = new System.Drawing.Size(75, 17);
            this.plcmtPointsButton.TabIndex = 12;
            this.plcmtPointsButton.Text = "Placement";
            this.plcmtPointsButton.UseVisualStyleBackColor = true;
            // 
            // nopsPointsButton
            // 
            this.nopsPointsButton.AutoSize = true;
            this.nopsPointsButton.Checked = true;
            this.nopsPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nopsPointsButton.Location = new System.Drawing.Point(5, 15);
            this.nopsPointsButton.Name = "nopsPointsButton";
            this.nopsPointsButton.Size = new System.Drawing.Size(55, 17);
            this.nopsPointsButton.TabIndex = 11;
            this.nopsPointsButton.TabStop = true;
            this.nopsPointsButton.Text = "NOPS";
            this.nopsPointsButton.UseVisualStyleBackColor = true;
            // 
            // TrickLabel
            // 
            this.TrickLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TrickLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrickLabel.Location = new System.Drawing.Point(203, 104);
            this.TrickLabel.Name = "TrickLabel";
            this.TrickLabel.Size = new System.Drawing.Size(289, 15);
            this.TrickLabel.TabIndex = 0;
            this.TrickLabel.Text = "Trick";
            this.TrickLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // scoreSummaryDataGridView
            // 
            this.scoreSummaryDataGridView.AllowUserToAddRows = false;
            this.scoreSummaryDataGridView.AllowUserToDeleteRows = false;
            this.scoreSummaryDataGridView.AllowUserToResizeColumns = false;
            this.scoreSummaryDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.scoreSummaryDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.scoreSummaryDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.MemberId,
            this.SanctionId,
            this.SkierName,
            this.AgeGroup,
            this.EventGroup,
            this.SepTrick,
            this.RoundTrick,
            this.EventClassTrick,
            this.Pass1Trick,
            this.Pass2Trick,
            this.ScoreTrick,
            this.PointsTrick,
            this.PlcmtTrick});
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.scoreSummaryDataGridView.DefaultCellStyle = dataGridViewCellStyle12;
            this.scoreSummaryDataGridView.Location = new System.Drawing.Point(4, 121);
            this.scoreSummaryDataGridView.Name = "scoreSummaryDataGridView";
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle13.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle13.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle13.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle13.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.scoreSummaryDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle13;
            this.scoreSummaryDataGridView.RowHeadersVisible = false;
            this.scoreSummaryDataGridView.Size = new System.Drawing.Size(785, 314);
            this.scoreSummaryDataGridView.TabIndex = 110;
            this.scoreSummaryDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.scoreSummaryDataGridView_RowEnter);
            // 
            // MemberId
            // 
            this.MemberId.HeaderText = "MemberId";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.Visible = false;
            // 
            // SanctionId
            // 
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.Visible = false;
            // 
            // SkierName
            // 
            this.SkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SkierName.DefaultCellStyle = dataGridViewCellStyle3;
            this.SkierName.HeaderText = "SkierName";
            this.SkierName.Name = "SkierName";
            this.SkierName.ReadOnly = true;
            this.SkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkierName.Width = 125;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AgeGroup.DefaultCellStyle = dataGridViewCellStyle4;
            this.AgeGroup.HeaderText = "Div";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.Width = 30;
            // 
            // EventGroup
            // 
            this.EventGroup.HeaderText = "Group";
            this.EventGroup.Name = "EventGroup";
            this.EventGroup.ReadOnly = true;
            this.EventGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventGroup.Width = 40;
            // 
            // SepTrick
            // 
            this.SepTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.SepTrick.DefaultCellStyle = dataGridViewCellStyle5;
            this.SepTrick.HeaderText = "|";
            this.SepTrick.MinimumWidth = 2;
            this.SepTrick.Name = "SepTrick";
            this.SepTrick.ReadOnly = true;
            this.SepTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SepTrick.Width = 2;
            // 
            // RoundTrick
            // 
            this.RoundTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RoundTrick.HeaderText = "Rd";
            this.RoundTrick.Name = "RoundTrick";
            this.RoundTrick.ReadOnly = true;
            this.RoundTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RoundTrick.Width = 35;
            // 
            // EventClassTrick
            // 
            this.EventClassTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.EventClassTrick.DefaultCellStyle = dataGridViewCellStyle6;
            this.EventClassTrick.HeaderText = "Class";
            this.EventClassTrick.Name = "EventClassTrick";
            this.EventClassTrick.ReadOnly = true;
            this.EventClassTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventClassTrick.Width = 33;
            // 
            // Pass1Trick
            // 
            this.Pass1Trick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N0";
            dataGridViewCellStyle7.NullValue = null;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Pass1Trick.DefaultCellStyle = dataGridViewCellStyle7;
            this.Pass1Trick.HeaderText = "Pass1";
            this.Pass1Trick.Name = "Pass1Trick";
            this.Pass1Trick.ReadOnly = true;
            this.Pass1Trick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Pass1Trick.Width = 45;
            // 
            // Pass2Trick
            // 
            this.Pass2Trick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N0";
            dataGridViewCellStyle8.NullValue = null;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Pass2Trick.DefaultCellStyle = dataGridViewCellStyle8;
            this.Pass2Trick.HeaderText = "Pass2";
            this.Pass2Trick.Name = "Pass2Trick";
            this.Pass2Trick.ReadOnly = true;
            this.Pass2Trick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Pass2Trick.Width = 45;
            // 
            // ScoreTrick
            // 
            this.ScoreTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.Format = "N0";
            dataGridViewCellStyle9.NullValue = null;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ScoreTrick.DefaultCellStyle = dataGridViewCellStyle9;
            this.ScoreTrick.HeaderText = "Score";
            this.ScoreTrick.Name = "ScoreTrick";
            this.ScoreTrick.ReadOnly = true;
            this.ScoreTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ScoreTrick.Width = 50;
            // 
            // PointsTrick
            // 
            this.PointsTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle10.Format = "N1";
            dataGridViewCellStyle10.NullValue = null;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsTrick.DefaultCellStyle = dataGridViewCellStyle10;
            this.PointsTrick.HeaderText = "Points";
            this.PointsTrick.Name = "PointsTrick";
            this.PointsTrick.ReadOnly = true;
            this.PointsTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PointsTrick.Width = 50;
            // 
            // PlcmtTrick
            // 
            this.PlcmtTrick.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.PlcmtTrick.DefaultCellStyle = dataGridViewCellStyle11;
            this.PlcmtTrick.HeaderText = "Plcmt";
            this.PlcmtTrick.Name = "PlcmtTrick";
            this.PlcmtTrick.ReadOnly = true;
            this.PlcmtTrick.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PlcmtTrick.Width = 35;
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle14.BackColor = System.Drawing.Color.Black;
            dataGridViewCellStyle14.NullValue = ((object)(resources.GetObject("dataGridViewCellStyle14.NullValue")));
            this.dataGridViewImageColumn1.DefaultCellStyle = dataGridViewCellStyle14;
            this.dataGridViewImageColumn1.HeaderText = "|";
            this.dataGridViewImageColumn1.Image = global::WaterskiScoringSystem.Properties.Resources.ColSep;
            this.dataGridViewImageColumn1.MinimumWidth = 2;
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewImageColumn1.Width = 2;
            // 
            // plcmtGroupBox
            // 
            this.plcmtGroupBox.Controls.Add(this.plcmtDivGrpButton);
            this.plcmtGroupBox.Controls.Add(this.plcmtDivButton);
            this.plcmtGroupBox.Location = new System.Drawing.Point(347, 62);
            this.plcmtGroupBox.Name = "plcmtGroupBox";
            this.plcmtGroupBox.Size = new System.Drawing.Size(138, 39);
            this.plcmtGroupBox.TabIndex = 20;
            this.plcmtGroupBox.TabStop = false;
            this.plcmtGroupBox.Text = "Placement";
            // 
            // plcmtDivGrpButton
            // 
            this.plcmtDivGrpButton.AutoSize = true;
            this.plcmtDivGrpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plcmtDivGrpButton.Location = new System.Drawing.Point(51, 15);
            this.plcmtDivGrpButton.Name = "plcmtDivGrpButton";
            this.plcmtDivGrpButton.Size = new System.Drawing.Size(75, 17);
            this.plcmtDivGrpButton.TabIndex = 22;
            this.plcmtDivGrpButton.Text = "Div/Group";
            this.plcmtDivGrpButton.UseVisualStyleBackColor = true;
            // 
            // plcmtDivButton
            // 
            this.plcmtDivButton.AutoSize = true;
            this.plcmtDivButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plcmtDivButton.Location = new System.Drawing.Point(6, 15);
            this.plcmtDivButton.Name = "plcmtDivButton";
            this.plcmtDivButton.Size = new System.Drawing.Size(41, 17);
            this.plcmtDivButton.TabIndex = 21;
            this.plcmtDivButton.Text = "Div";
            this.plcmtDivButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(254, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a division and click Refresh icon";
            // 
            // EventGroupList
            // 
            this.EventGroupList.FormattingEnabled = true;
            this.EventGroupList.Location = new System.Drawing.Point(144, 38);
            this.EventGroupList.Name = "EventGroupList";
            this.EventGroupList.Size = new System.Drawing.Size(100, 21);
            this.EventGroupList.TabIndex = 1;
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(7, 40);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 115;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // firstScoreButton
            // 
            this.firstScoreButton.AutoSize = true;
            this.firstScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.firstScoreButton.Location = new System.Drawing.Point(121, 15);
            this.firstScoreButton.Name = "firstScoreButton";
            this.firstScoreButton.Size = new System.Drawing.Size(44, 17);
            this.firstScoreButton.TabIndex = 14;
            this.firstScoreButton.Text = "First";
            this.firstScoreButton.UseVisualStyleBackColor = true;
            // 
            // finalScoreButton
            // 
            this.finalScoreButton.AutoSize = true;
            this.finalScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finalScoreButton.Location = new System.Drawing.Point(66, 15);
            this.finalScoreButton.Name = "finalScoreButton";
            this.finalScoreButton.Size = new System.Drawing.Size(47, 17);
            this.finalScoreButton.TabIndex = 13;
            this.finalScoreButton.Text = "Final";
            this.finalScoreButton.UseVisualStyleBackColor = true;
            // 
            // totalScoreButton
            // 
            this.totalScoreButton.AutoSize = true;
            this.totalScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalScoreButton.Location = new System.Drawing.Point(173, 15);
            this.totalScoreButton.Name = "totalScoreButton";
            this.totalScoreButton.Size = new System.Drawing.Size(49, 17);
            this.totalScoreButton.TabIndex = 12;
            this.totalScoreButton.Text = "Total";
            this.totalScoreButton.UseVisualStyleBackColor = true;
            // 
            // bestScoreButton
            // 
            this.bestScoreButton.AutoSize = true;
            this.bestScoreButton.Checked = true;
            this.bestScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bestScoreButton.Location = new System.Drawing.Point(12, 15);
            this.bestScoreButton.Name = "bestScoreButton";
            this.bestScoreButton.Size = new System.Drawing.Size(46, 17);
            this.bestScoreButton.TabIndex = 11;
            this.bestScoreButton.TabStop = true;
            this.bestScoreButton.Text = "Best";
            this.bestScoreButton.UseVisualStyleBackColor = true;
            // 
            // scoresPlcmtGroupBox
            // 
            this.scoresPlcmtGroupBox.Controls.Add(this.firstScoreButton);
            this.scoresPlcmtGroupBox.Controls.Add(this.finalScoreButton);
            this.scoresPlcmtGroupBox.Controls.Add(this.totalScoreButton);
            this.scoresPlcmtGroupBox.Controls.Add(this.bestScoreButton);
            this.scoresPlcmtGroupBox.Location = new System.Drawing.Point(494, 62);
            this.scoresPlcmtGroupBox.Name = "scoresPlcmtGroupBox";
            this.scoresPlcmtGroupBox.Size = new System.Drawing.Size(236, 39);
            this.scoresPlcmtGroupBox.TabIndex = 116;
            this.scoresPlcmtGroupBox.TabStop = false;
            this.scoresPlcmtGroupBox.Text = "Scores to Use";
            // 
            // TrickScorebook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 459);
            this.Controls.Add(this.scoresPlcmtGroupBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventGroupList);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.plcmtGroupBox);
            this.Controls.Add(this.TrickLabel);
            this.Controls.Add(this.PointsMethodGroupBox);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.scoreSummaryDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TrickScorebook";
            this.Text = "Trick Scorebook";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrickScorebook_FormClosed);
            this.Load += new System.EventHandler(this.TrickScorebook_Load);
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.PointsMethodGroupBox.ResumeLayout(false);
            this.PointsMethodGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scoreSummaryDataGridView)).EndInit();
            this.plcmtGroupBox.ResumeLayout(false);
            this.plcmtGroupBox.PerformLayout();
            this.scoresPlcmtGroupBox.ResumeLayout(false);
            this.scoresPlcmtGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.GroupBox PointsMethodGroupBox;
        private System.Windows.Forms.RadioButton kBasePointsButton;
        private System.Windows.Forms.RadioButton plcmtPointsButton;
        private System.Windows.Forms.RadioButton nopsPointsButton;
        private System.Windows.Forms.Label TrickLabel;
        private System.Windows.Forms.DataGridView scoreSummaryDataGridView;
        private System.Windows.Forms.RadioButton ratioPointsButton;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.GroupBox plcmtGroupBox;
        private System.Windows.Forms.RadioButton plcmtDivGrpButton;
        private System.Windows.Forms.RadioButton plcmtDivButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox EventGroupList;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.RadioButton firstScoreButton;
        private System.Windows.Forms.RadioButton finalScoreButton;
        private System.Windows.Forms.RadioButton totalScoreButton;
        private System.Windows.Forms.RadioButton bestScoreButton;
        private System.Windows.Forms.GroupBox scoresPlcmtGroupBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn SepTrick;
        private System.Windows.Forms.DataGridViewTextBoxColumn RoundTrick;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventClassTrick;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pass1Trick;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pass2Trick;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScoreTrick;
        private System.Windows.Forms.DataGridViewTextBoxColumn PointsTrick;
        private System.Windows.Forms.DataGridViewTextBoxColumn PlcmtTrick;
    }
}