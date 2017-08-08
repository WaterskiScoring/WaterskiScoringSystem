namespace WaterskiScoringSystem.Tournament {
    partial class OfficialWorkRecord {
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
            System.Windows.Forms.Label noteLabel;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfficialWorkRecord));
            this.listTourMemberDataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JudgeSlalomRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JudgeTrickRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JudgeJumpRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverSlalomRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverTrickRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverJumpRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScorerSlalomRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScorerTrickRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScorerJumpRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafetyOfficialRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TechOfficialRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AnncrOfficialRatingDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.editPK = new System.Windows.Forms.TextBox();
            this.editSanctionId = new System.Windows.Forms.TextBox();
            this.editMemberId = new System.Windows.Forms.TextBox();
            this.editJudgeSlalomRating = new System.Windows.Forms.TextBox();
            this.editJudgeJumpRating = new System.Windows.Forms.TextBox();
            this.editJudgeTrickRating = new System.Windows.Forms.TextBox();
            this.editSafetyOfficialRating = new System.Windows.Forms.TextBox();
            this.editScorerSlalomRating = new System.Windows.Forms.TextBox();
            this.editTechOfficialRating = new System.Windows.Forms.TextBox();
            this.editDriverSlalomRating = new System.Windows.Forms.TextBox();
            this.editNote = new System.Windows.Forms.TextBox();
            this.JudgeChiefCB = new System.Windows.Forms.CheckBox();
            this.TechSlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.DriverChiefCB = new System.Windows.Forms.CheckBox();
            this.ScoreChiefCB = new System.Windows.Forms.CheckBox();
            this.SafetyChiefCB = new System.Windows.Forms.CheckBox();
            this.JudgeAsstChiefCB = new System.Windows.Forms.CheckBox();
            this.JudgeAppointedCB = new System.Windows.Forms.CheckBox();
            this.DriverAsstChiefCB = new System.Windows.Forms.CheckBox();
            this.DriverAppointedCB = new System.Windows.Forms.CheckBox();
            this.ScoreAsstChiefCB = new System.Windows.Forms.CheckBox();
            this.ScoreAppointedCB = new System.Windows.Forms.CheckBox();
            this.SafetyAsstChiefCB = new System.Windows.Forms.CheckBox();
            this.SafetyAppointedCB = new System.Windows.Forms.CheckBox();
            this.JudgeSlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.DriverSlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.ScoreSlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.SafetySlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.JudgeTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.DriverTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.ScoreTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.SafetyTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.TechTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.JudgeJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.DriverJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.ScoreJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.SafetyJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.TechJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.JudgeLabel = new System.Windows.Forms.Label();
            this.DriverLabel = new System.Windows.Forms.Label();
            this.ScorerLabel = new System.Windows.Forms.Label();
            this.SafetyLabel = new System.Windows.Forms.Label();
            this.TechnicalLabel = new System.Windows.Forms.Label();
            this.AnncrLabel = new System.Windows.Forms.Label();
            this.AnncrJumpCreditCB = new System.Windows.Forms.CheckBox();
            this.AnncrTrickCreditCB = new System.Windows.Forms.CheckBox();
            this.AnncrSlalomCreditCB = new System.Windows.Forms.CheckBox();
            this.editAnncrOfficialRating = new System.Windows.Forms.TextBox();
            this.AnncrChiefCB = new System.Windows.Forms.CheckBox();
            this.mainNavMenu = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navPrint = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.ExportMemberList = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.navSort = new System.Windows.Forms.ToolStripButton();
            this.navSaveItem = new System.Windows.Forms.ToolStripButton();
            this.navEditOfficials = new System.Windows.Forms.ToolStripButton();
            this.TechAsstChiefCB = new System.Windows.Forms.CheckBox();
            this.TechChiefCB = new System.Windows.Forms.CheckBox();
            this.editDriverTrickRating = new System.Windows.Forms.TextBox();
            this.editDriverJumpRating = new System.Windows.Forms.TextBox();
            this.editScorerTrickRating = new System.Windows.Forms.TextBox();
            this.editScorerJumpRating = new System.Windows.Forms.TextBox();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.exportCreditFile = new System.Windows.Forms.ToolStripButton();
            noteLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.listTourMemberDataGridView)).BeginInit();
            this.winStatus.SuspendLayout();
            this.mainNavMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // noteLabel
            // 
            noteLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            noteLabel.AutoSize = true;
            noteLabel.Location = new System.Drawing.Point(489, 542);
            noteLabel.Name = "noteLabel";
            noteLabel.Size = new System.Drawing.Size(33, 13);
            noteLabel.TabIndex = 77;
            noteLabel.Text = "Note:";
            // 
            // listTourMemberDataGridView
            // 
            this.listTourMemberDataGridView.AllowUserToAddRows = false;
            this.listTourMemberDataGridView.AllowUserToDeleteRows = false;
            this.listTourMemberDataGridView.AllowUserToResizeColumns = false;
            this.listTourMemberDataGridView.AllowUserToResizeRows = false;
            this.listTourMemberDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.listTourMemberDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.listTourMemberDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.listTourMemberDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.SanctionId,
            this.SkierName,
            this.Federation,
            this.JudgeSlalomRatingDesc,
            this.JudgeTrickRatingDesc,
            this.JudgeJumpRatingDesc,
            this.DriverSlalomRatingDesc,
            this.DriverTrickRatingDesc,
            this.DriverJumpRatingDesc,
            this.ScorerSlalomRatingDesc,
            this.ScorerTrickRatingDesc,
            this.ScorerJumpRatingDesc,
            this.SafetyOfficialRatingDesc,
            this.TechOfficialRatingDesc,
            this.AnncrOfficialRatingDesc});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.listTourMemberDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.listTourMemberDataGridView.Location = new System.Drawing.Point(5, 62);
            this.listTourMemberDataGridView.Name = "listTourMemberDataGridView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.listTourMemberDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.listTourMemberDataGridView.RowHeadersVisible = false;
            this.listTourMemberDataGridView.Size = new System.Drawing.Size(478, 491);
            this.listTourMemberDataGridView.StandardTab = true;
            this.listTourMemberDataGridView.TabIndex = 1;
            this.listTourMemberDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.listTourMemberDataGridView_CellContentClick);
            this.listTourMemberDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.listTourMemberDataGridView_CellFormatting);
            this.listTourMemberDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
            this.listTourMemberDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.listTourMemberDataGridView_RowEnter);
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberId.HeaderText = "MemberId";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.Width = 80;
            // 
            // SanctionId
            // 
            this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.Visible = false;
            // 
            // SkierName
            // 
            this.SkierName.HeaderText = "SkierName";
            this.SkierName.Name = "SkierName";
            this.SkierName.ReadOnly = true;
            this.SkierName.Width = 125;
            // 
            // Federation
            // 
            this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Federation.HeaderText = "Federation";
            this.Federation.Name = "Federation";
            this.Federation.ReadOnly = true;
            this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Federation.Visible = false;
            // 
            // JudgeSlalomRatingDesc
            // 
            this.JudgeSlalomRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JudgeSlalomRatingDesc.HeaderText = "Judge Slalom";
            this.JudgeSlalomRatingDesc.Name = "JudgeSlalomRatingDesc";
            this.JudgeSlalomRatingDesc.ReadOnly = true;
            this.JudgeSlalomRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JudgeSlalomRatingDesc.Width = 55;
            // 
            // JudgeTrickRatingDesc
            // 
            this.JudgeTrickRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JudgeTrickRatingDesc.HeaderText = "Judge Trick";
            this.JudgeTrickRatingDesc.Name = "JudgeTrickRatingDesc";
            this.JudgeTrickRatingDesc.ReadOnly = true;
            this.JudgeTrickRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JudgeTrickRatingDesc.Width = 55;
            // 
            // JudgeJumpRatingDesc
            // 
            this.JudgeJumpRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JudgeJumpRatingDesc.HeaderText = "Judge Jump";
            this.JudgeJumpRatingDesc.Name = "JudgeJumpRatingDesc";
            this.JudgeJumpRatingDesc.ReadOnly = true;
            this.JudgeJumpRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JudgeJumpRatingDesc.Width = 55;
            // 
            // DriverSlalomRatingDesc
            // 
            this.DriverSlalomRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DriverSlalomRatingDesc.HeaderText = "Driver Slalom";
            this.DriverSlalomRatingDesc.Name = "DriverSlalomRatingDesc";
            this.DriverSlalomRatingDesc.ReadOnly = true;
            this.DriverSlalomRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DriverSlalomRatingDesc.Width = 55;
            // 
            // DriverTrickRatingDesc
            // 
            this.DriverTrickRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DriverTrickRatingDesc.HeaderText = "Driver Trick";
            this.DriverTrickRatingDesc.Name = "DriverTrickRatingDesc";
            this.DriverTrickRatingDesc.ReadOnly = true;
            this.DriverTrickRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DriverTrickRatingDesc.Width = 55;
            // 
            // DriverJumpRatingDesc
            // 
            this.DriverJumpRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DriverJumpRatingDesc.HeaderText = "Driver Jump";
            this.DriverJumpRatingDesc.Name = "DriverJumpRatingDesc";
            this.DriverJumpRatingDesc.ReadOnly = true;
            this.DriverJumpRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DriverJumpRatingDesc.Width = 55;
            // 
            // ScorerSlalomRatingDesc
            // 
            this.ScorerSlalomRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ScorerSlalomRatingDesc.HeaderText = "Scorer Slalom";
            this.ScorerSlalomRatingDesc.Name = "ScorerSlalomRatingDesc";
            this.ScorerSlalomRatingDesc.ReadOnly = true;
            this.ScorerSlalomRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ScorerSlalomRatingDesc.Width = 55;
            // 
            // ScorerTrickRatingDesc
            // 
            this.ScorerTrickRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ScorerTrickRatingDesc.HeaderText = "Scorer Trick";
            this.ScorerTrickRatingDesc.Name = "ScorerTrickRatingDesc";
            this.ScorerTrickRatingDesc.ReadOnly = true;
            this.ScorerTrickRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ScorerTrickRatingDesc.Width = 55;
            // 
            // ScorerJumpRatingDesc
            // 
            this.ScorerJumpRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ScorerJumpRatingDesc.HeaderText = "Scorer Jump";
            this.ScorerJumpRatingDesc.Name = "ScorerJumpRatingDesc";
            this.ScorerJumpRatingDesc.ReadOnly = true;
            this.ScorerJumpRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ScorerJumpRatingDesc.Width = 55;
            // 
            // SafetyOfficialRatingDesc
            // 
            this.SafetyOfficialRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SafetyOfficialRatingDesc.HeaderText = "Safety Rating";
            this.SafetyOfficialRatingDesc.Name = "SafetyOfficialRatingDesc";
            this.SafetyOfficialRatingDesc.ReadOnly = true;
            this.SafetyOfficialRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SafetyOfficialRatingDesc.Width = 55;
            // 
            // TechOfficialRatingDesc
            // 
            this.TechOfficialRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TechOfficialRatingDesc.HeaderText = "Tech Rating";
            this.TechOfficialRatingDesc.Name = "TechOfficialRatingDesc";
            this.TechOfficialRatingDesc.ReadOnly = true;
            this.TechOfficialRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TechOfficialRatingDesc.Width = 55;
            // 
            // AnncrOfficialRatingDesc
            // 
            this.AnncrOfficialRatingDesc.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AnncrOfficialRatingDesc.HeaderText = "Announcer Rating";
            this.AnncrOfficialRatingDesc.Name = "AnncrOfficialRatingDesc";
            this.AnncrOfficialRatingDesc.ReadOnly = true;
            this.AnncrOfficialRatingDesc.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AnncrOfficialRatingDesc.Width = 60;
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 563);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(764, 22);
            this.winStatus.TabIndex = 0;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // editPK
            // 
            this.editPK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editPK.Enabled = false;
            this.editPK.Location = new System.Drawing.Point(489, 40);
            this.editPK.Margin = new System.Windows.Forms.Padding(1);
            this.editPK.Name = "editPK";
            this.editPK.ReadOnly = true;
            this.editPK.Size = new System.Drawing.Size(85, 20);
            this.editPK.TabIndex = 4;
            // 
            // editSanctionId
            // 
            this.editSanctionId.AcceptsReturn = true;
            this.editSanctionId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editSanctionId.Enabled = false;
            this.editSanctionId.Location = new System.Drawing.Point(580, 40);
            this.editSanctionId.Margin = new System.Windows.Forms.Padding(1);
            this.editSanctionId.Name = "editSanctionId";
            this.editSanctionId.ReadOnly = true;
            this.editSanctionId.Size = new System.Drawing.Size(85, 20);
            this.editSanctionId.TabIndex = 6;
            // 
            // editMemberId
            // 
            this.editMemberId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editMemberId.Enabled = false;
            this.editMemberId.Location = new System.Drawing.Point(671, 40);
            this.editMemberId.Margin = new System.Windows.Forms.Padding(1);
            this.editMemberId.Name = "editMemberId";
            this.editMemberId.ReadOnly = true;
            this.editMemberId.Size = new System.Drawing.Size(85, 20);
            this.editMemberId.TabIndex = 8;
            // 
            // editJudgeSlalomRating
            // 
            this.editJudgeSlalomRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editJudgeSlalomRating.BackColor = System.Drawing.SystemColors.Info;
            this.editJudgeSlalomRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editJudgeSlalomRating.Enabled = false;
            this.editJudgeSlalomRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editJudgeSlalomRating.Location = new System.Drawing.Point(656, 84);
            this.editJudgeSlalomRating.Margin = new System.Windows.Forms.Padding(1);
            this.editJudgeSlalomRating.Name = "editJudgeSlalomRating";
            this.editJudgeSlalomRating.ReadOnly = true;
            this.editJudgeSlalomRating.Size = new System.Drawing.Size(100, 14);
            this.editJudgeSlalomRating.TabIndex = 0;
            this.editJudgeSlalomRating.TabStop = false;
            // 
            // editJudgeJumpRating
            // 
            this.editJudgeJumpRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editJudgeJumpRating.BackColor = System.Drawing.SystemColors.Info;
            this.editJudgeJumpRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editJudgeJumpRating.Enabled = false;
            this.editJudgeJumpRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editJudgeJumpRating.Location = new System.Drawing.Point(656, 124);
            this.editJudgeJumpRating.Name = "editJudgeJumpRating";
            this.editJudgeJumpRating.ReadOnly = true;
            this.editJudgeJumpRating.Size = new System.Drawing.Size(100, 14);
            this.editJudgeJumpRating.TabIndex = 0;
            this.editJudgeJumpRating.TabStop = false;
            // 
            // editJudgeTrickRating
            // 
            this.editJudgeTrickRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editJudgeTrickRating.BackColor = System.Drawing.SystemColors.Info;
            this.editJudgeTrickRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editJudgeTrickRating.Enabled = false;
            this.editJudgeTrickRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editJudgeTrickRating.Location = new System.Drawing.Point(656, 104);
            this.editJudgeTrickRating.Name = "editJudgeTrickRating";
            this.editJudgeTrickRating.ReadOnly = true;
            this.editJudgeTrickRating.Size = new System.Drawing.Size(100, 14);
            this.editJudgeTrickRating.TabIndex = 0;
            this.editJudgeTrickRating.TabStop = false;
            // 
            // editSafetyOfficialRating
            // 
            this.editSafetyOfficialRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editSafetyOfficialRating.BackColor = System.Drawing.SystemColors.Info;
            this.editSafetyOfficialRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editSafetyOfficialRating.Enabled = false;
            this.editSafetyOfficialRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editSafetyOfficialRating.Location = new System.Drawing.Point(656, 325);
            this.editSafetyOfficialRating.Name = "editSafetyOfficialRating";
            this.editSafetyOfficialRating.ReadOnly = true;
            this.editSafetyOfficialRating.Size = new System.Drawing.Size(100, 14);
            this.editSafetyOfficialRating.TabIndex = 0;
            this.editSafetyOfficialRating.TabStop = false;
            // 
            // editScorerSlalomRating
            // 
            this.editScorerSlalomRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editScorerSlalomRating.BackColor = System.Drawing.SystemColors.Info;
            this.editScorerSlalomRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editScorerSlalomRating.Enabled = false;
            this.editScorerSlalomRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editScorerSlalomRating.Location = new System.Drawing.Point(656, 245);
            this.editScorerSlalomRating.Name = "editScorerSlalomRating";
            this.editScorerSlalomRating.ReadOnly = true;
            this.editScorerSlalomRating.Size = new System.Drawing.Size(100, 14);
            this.editScorerSlalomRating.TabIndex = 0;
            this.editScorerSlalomRating.TabStop = false;
            // 
            // editTechOfficialRating
            // 
            this.editTechOfficialRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editTechOfficialRating.BackColor = System.Drawing.SystemColors.Info;
            this.editTechOfficialRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editTechOfficialRating.Enabled = false;
            this.editTechOfficialRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editTechOfficialRating.Location = new System.Drawing.Point(656, 405);
            this.editTechOfficialRating.Name = "editTechOfficialRating";
            this.editTechOfficialRating.ReadOnly = true;
            this.editTechOfficialRating.Size = new System.Drawing.Size(100, 14);
            this.editTechOfficialRating.TabIndex = 0;
            this.editTechOfficialRating.TabStop = false;
            // 
            // editDriverSlalomRating
            // 
            this.editDriverSlalomRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editDriverSlalomRating.BackColor = System.Drawing.SystemColors.Info;
            this.editDriverSlalomRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editDriverSlalomRating.Enabled = false;
            this.editDriverSlalomRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editDriverSlalomRating.Location = new System.Drawing.Point(656, 166);
            this.editDriverSlalomRating.Name = "editDriverSlalomRating";
            this.editDriverSlalomRating.ReadOnly = true;
            this.editDriverSlalomRating.Size = new System.Drawing.Size(100, 14);
            this.editDriverSlalomRating.TabIndex = 0;
            this.editDriverSlalomRating.TabStop = false;
            // 
            // editNote
            // 
            this.editNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editNote.Location = new System.Drawing.Point(522, 538);
            this.editNote.Name = "editNote";
            this.editNote.Size = new System.Drawing.Size(236, 20);
            this.editNote.TabIndex = 70;
            this.editNote.TextChanged += new System.EventHandler(this.ItemChanged);
            // 
            // JudgeChiefCB
            // 
            this.JudgeChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeChiefCB.AutoSize = true;
            this.JudgeChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeChiefCB.Location = new System.Drawing.Point(489, 82);
            this.JudgeChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.JudgeChiefCB.Name = "JudgeChiefCB";
            this.JudgeChiefCB.Size = new System.Drawing.Size(54, 18);
            this.JudgeChiefCB.TabIndex = 10;
            this.JudgeChiefCB.Text = "Chief";
            this.JudgeChiefCB.UseVisualStyleBackColor = true;
            this.JudgeChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // TechSlalomCreditCB
            // 
            this.TechSlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechSlalomCreditCB.AutoSize = true;
            this.TechSlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechSlalomCreditCB.Location = new System.Drawing.Point(582, 403);
            this.TechSlalomCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.TechSlalomCreditCB.Name = "TechSlalomCreditCB";
            this.TechSlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.TechSlalomCreditCB.TabIndex = 55;
            this.TechSlalomCreditCB.Text = "Slalom";
            this.TechSlalomCreditCB.UseVisualStyleBackColor = true;
            this.TechSlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.TechSlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverChiefCB
            // 
            this.DriverChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverChiefCB.AutoSize = true;
            this.DriverChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverChiefCB.Location = new System.Drawing.Point(489, 164);
            this.DriverChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverChiefCB.Name = "DriverChiefCB";
            this.DriverChiefCB.Size = new System.Drawing.Size(54, 18);
            this.DriverChiefCB.TabIndex = 20;
            this.DriverChiefCB.Text = "Chief";
            this.DriverChiefCB.UseVisualStyleBackColor = true;
            this.DriverChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreChiefCB
            // 
            this.ScoreChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreChiefCB.AutoSize = true;
            this.ScoreChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreChiefCB.Location = new System.Drawing.Point(489, 243);
            this.ScoreChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreChiefCB.Name = "ScoreChiefCB";
            this.ScoreChiefCB.Size = new System.Drawing.Size(54, 18);
            this.ScoreChiefCB.TabIndex = 30;
            this.ScoreChiefCB.Text = "Chief";
            this.ScoreChiefCB.UseVisualStyleBackColor = true;
            this.ScoreChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetyChiefCB
            // 
            this.SafetyChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyChiefCB.AutoSize = true;
            this.SafetyChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyChiefCB.Location = new System.Drawing.Point(489, 323);
            this.SafetyChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetyChiefCB.Name = "SafetyChiefCB";
            this.SafetyChiefCB.Size = new System.Drawing.Size(54, 18);
            this.SafetyChiefCB.TabIndex = 40;
            this.SafetyChiefCB.Text = "Chief";
            this.SafetyChiefCB.UseVisualStyleBackColor = true;
            this.SafetyChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetyChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeAsstChiefCB
            // 
            this.JudgeAsstChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeAsstChiefCB.AutoSize = true;
            this.JudgeAsstChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeAsstChiefCB.Location = new System.Drawing.Point(489, 102);
            this.JudgeAsstChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.JudgeAsstChiefCB.Name = "JudgeAsstChiefCB";
            this.JudgeAsstChiefCB.Size = new System.Drawing.Size(80, 18);
            this.JudgeAsstChiefCB.TabIndex = 11;
            this.JudgeAsstChiefCB.Text = "Asst Chief";
            this.JudgeAsstChiefCB.UseVisualStyleBackColor = true;
            this.JudgeAsstChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeAsstChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeAppointedCB
            // 
            this.JudgeAppointedCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeAppointedCB.AutoSize = true;
            this.JudgeAppointedCB.Enabled = false;
            this.JudgeAppointedCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeAppointedCB.Location = new System.Drawing.Point(489, 122);
            this.JudgeAppointedCB.Margin = new System.Windows.Forms.Padding(1);
            this.JudgeAppointedCB.Name = "JudgeAppointedCB";
            this.JudgeAppointedCB.Size = new System.Drawing.Size(83, 18);
            this.JudgeAppointedCB.TabIndex = 12;
            this.JudgeAppointedCB.Text = "Appointed";
            this.JudgeAppointedCB.UseVisualStyleBackColor = true;
            this.JudgeAppointedCB.Visible = false;
            this.JudgeAppointedCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeAppointedCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverAsstChiefCB
            // 
            this.DriverAsstChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverAsstChiefCB.AutoSize = true;
            this.DriverAsstChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverAsstChiefCB.Location = new System.Drawing.Point(489, 184);
            this.DriverAsstChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverAsstChiefCB.Name = "DriverAsstChiefCB";
            this.DriverAsstChiefCB.Size = new System.Drawing.Size(80, 18);
            this.DriverAsstChiefCB.TabIndex = 21;
            this.DriverAsstChiefCB.Text = "Asst Chief";
            this.DriverAsstChiefCB.UseVisualStyleBackColor = true;
            this.DriverAsstChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverAsstChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverAppointedCB
            // 
            this.DriverAppointedCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverAppointedCB.AutoSize = true;
            this.DriverAppointedCB.Enabled = false;
            this.DriverAppointedCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverAppointedCB.Location = new System.Drawing.Point(489, 204);
            this.DriverAppointedCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverAppointedCB.Name = "DriverAppointedCB";
            this.DriverAppointedCB.Size = new System.Drawing.Size(83, 18);
            this.DriverAppointedCB.TabIndex = 22;
            this.DriverAppointedCB.Text = "Appointed";
            this.DriverAppointedCB.UseVisualStyleBackColor = true;
            this.DriverAppointedCB.Visible = false;
            this.DriverAppointedCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverAppointedCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreAsstChiefCB
            // 
            this.ScoreAsstChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreAsstChiefCB.AutoSize = true;
            this.ScoreAsstChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreAsstChiefCB.Location = new System.Drawing.Point(489, 263);
            this.ScoreAsstChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreAsstChiefCB.Name = "ScoreAsstChiefCB";
            this.ScoreAsstChiefCB.Size = new System.Drawing.Size(80, 18);
            this.ScoreAsstChiefCB.TabIndex = 31;
            this.ScoreAsstChiefCB.Text = "Asst Chief";
            this.ScoreAsstChiefCB.UseVisualStyleBackColor = true;
            this.ScoreAsstChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreAsstChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreAppointedCB
            // 
            this.ScoreAppointedCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreAppointedCB.AutoSize = true;
            this.ScoreAppointedCB.Enabled = false;
            this.ScoreAppointedCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreAppointedCB.Location = new System.Drawing.Point(489, 283);
            this.ScoreAppointedCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreAppointedCB.Name = "ScoreAppointedCB";
            this.ScoreAppointedCB.Size = new System.Drawing.Size(83, 18);
            this.ScoreAppointedCB.TabIndex = 32;
            this.ScoreAppointedCB.Text = "Appointed";
            this.ScoreAppointedCB.UseVisualStyleBackColor = true;
            this.ScoreAppointedCB.Visible = false;
            this.ScoreAppointedCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreAppointedCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetyAsstChiefCB
            // 
            this.SafetyAsstChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyAsstChiefCB.AutoSize = true;
            this.SafetyAsstChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyAsstChiefCB.Location = new System.Drawing.Point(489, 342);
            this.SafetyAsstChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetyAsstChiefCB.Name = "SafetyAsstChiefCB";
            this.SafetyAsstChiefCB.Size = new System.Drawing.Size(80, 18);
            this.SafetyAsstChiefCB.TabIndex = 41;
            this.SafetyAsstChiefCB.Text = "Asst Chief";
            this.SafetyAsstChiefCB.UseVisualStyleBackColor = true;
            this.SafetyAsstChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetyAsstChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetyAppointedCB
            // 
            this.SafetyAppointedCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyAppointedCB.AutoSize = true;
            this.SafetyAppointedCB.Enabled = false;
            this.SafetyAppointedCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyAppointedCB.Location = new System.Drawing.Point(489, 361);
            this.SafetyAppointedCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetyAppointedCB.Name = "SafetyAppointedCB";
            this.SafetyAppointedCB.Size = new System.Drawing.Size(83, 18);
            this.SafetyAppointedCB.TabIndex = 42;
            this.SafetyAppointedCB.Text = "Appointed";
            this.SafetyAppointedCB.UseVisualStyleBackColor = true;
            this.SafetyAppointedCB.Visible = false;
            this.SafetyAppointedCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetyAppointedCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeSlalomCreditCB
            // 
            this.JudgeSlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeSlalomCreditCB.AutoSize = true;
            this.JudgeSlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeSlalomCreditCB.Location = new System.Drawing.Point(582, 82);
            this.JudgeSlalomCreditCB.Margin = new System.Windows.Forms.Padding(0);
            this.JudgeSlalomCreditCB.Name = "JudgeSlalomCreditCB";
            this.JudgeSlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.JudgeSlalomCreditCB.TabIndex = 15;
            this.JudgeSlalomCreditCB.Text = "Slalom";
            this.JudgeSlalomCreditCB.UseVisualStyleBackColor = true;
            this.JudgeSlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeSlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverSlalomCreditCB
            // 
            this.DriverSlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverSlalomCreditCB.AutoSize = true;
            this.DriverSlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverSlalomCreditCB.Location = new System.Drawing.Point(582, 164);
            this.DriverSlalomCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverSlalomCreditCB.Name = "DriverSlalomCreditCB";
            this.DriverSlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.DriverSlalomCreditCB.TabIndex = 25;
            this.DriverSlalomCreditCB.Text = "Slalom";
            this.DriverSlalomCreditCB.UseVisualStyleBackColor = true;
            this.DriverSlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverSlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreSlalomCreditCB
            // 
            this.ScoreSlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreSlalomCreditCB.AutoSize = true;
            this.ScoreSlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreSlalomCreditCB.Location = new System.Drawing.Point(582, 243);
            this.ScoreSlalomCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreSlalomCreditCB.Name = "ScoreSlalomCreditCB";
            this.ScoreSlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.ScoreSlalomCreditCB.TabIndex = 35;
            this.ScoreSlalomCreditCB.Text = "Slalom";
            this.ScoreSlalomCreditCB.UseVisualStyleBackColor = true;
            this.ScoreSlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreSlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetySlalomCreditCB
            // 
            this.SafetySlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetySlalomCreditCB.AutoSize = true;
            this.SafetySlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetySlalomCreditCB.Location = new System.Drawing.Point(582, 323);
            this.SafetySlalomCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetySlalomCreditCB.Name = "SafetySlalomCreditCB";
            this.SafetySlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.SafetySlalomCreditCB.TabIndex = 45;
            this.SafetySlalomCreditCB.Text = "Slalom";
            this.SafetySlalomCreditCB.UseVisualStyleBackColor = true;
            this.SafetySlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetySlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeTrickCreditCB
            // 
            this.JudgeTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeTrickCreditCB.AutoSize = true;
            this.JudgeTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeTrickCreditCB.Location = new System.Drawing.Point(582, 102);
            this.JudgeTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.JudgeTrickCreditCB.Name = "JudgeTrickCreditCB";
            this.JudgeTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.JudgeTrickCreditCB.TabIndex = 16;
            this.JudgeTrickCreditCB.Text = "Trick";
            this.JudgeTrickCreditCB.UseVisualStyleBackColor = true;
            this.JudgeTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverTrickCreditCB
            // 
            this.DriverTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverTrickCreditCB.AutoSize = true;
            this.DriverTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverTrickCreditCB.Location = new System.Drawing.Point(582, 184);
            this.DriverTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverTrickCreditCB.Name = "DriverTrickCreditCB";
            this.DriverTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.DriverTrickCreditCB.TabIndex = 26;
            this.DriverTrickCreditCB.Text = "Trick";
            this.DriverTrickCreditCB.UseVisualStyleBackColor = true;
            this.DriverTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreTrickCreditCB
            // 
            this.ScoreTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreTrickCreditCB.AutoSize = true;
            this.ScoreTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreTrickCreditCB.Location = new System.Drawing.Point(582, 263);
            this.ScoreTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreTrickCreditCB.Name = "ScoreTrickCreditCB";
            this.ScoreTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.ScoreTrickCreditCB.TabIndex = 36;
            this.ScoreTrickCreditCB.Text = "Trick";
            this.ScoreTrickCreditCB.UseVisualStyleBackColor = true;
            this.ScoreTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetyTrickCreditCB
            // 
            this.SafetyTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyTrickCreditCB.AutoSize = true;
            this.SafetyTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyTrickCreditCB.Location = new System.Drawing.Point(582, 342);
            this.SafetyTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetyTrickCreditCB.Name = "SafetyTrickCreditCB";
            this.SafetyTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.SafetyTrickCreditCB.TabIndex = 46;
            this.SafetyTrickCreditCB.Text = "Trick";
            this.SafetyTrickCreditCB.UseVisualStyleBackColor = true;
            this.SafetyTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetyTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // TechTrickCreditCB
            // 
            this.TechTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechTrickCreditCB.AutoSize = true;
            this.TechTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechTrickCreditCB.Location = new System.Drawing.Point(582, 421);
            this.TechTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.TechTrickCreditCB.Name = "TechTrickCreditCB";
            this.TechTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.TechTrickCreditCB.TabIndex = 56;
            this.TechTrickCreditCB.Text = "Trick";
            this.TechTrickCreditCB.UseVisualStyleBackColor = true;
            this.TechTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.TechTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeJumpCreditCB
            // 
            this.JudgeJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeJumpCreditCB.AutoSize = true;
            this.JudgeJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeJumpCreditCB.Location = new System.Drawing.Point(582, 122);
            this.JudgeJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.JudgeJumpCreditCB.Name = "JudgeJumpCreditCB";
            this.JudgeJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.JudgeJumpCreditCB.TabIndex = 17;
            this.JudgeJumpCreditCB.Text = "Jump";
            this.JudgeJumpCreditCB.UseVisualStyleBackColor = true;
            this.JudgeJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.JudgeJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // DriverJumpCreditCB
            // 
            this.DriverJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverJumpCreditCB.AutoSize = true;
            this.DriverJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverJumpCreditCB.Location = new System.Drawing.Point(582, 204);
            this.DriverJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.DriverJumpCreditCB.Name = "DriverJumpCreditCB";
            this.DriverJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.DriverJumpCreditCB.TabIndex = 27;
            this.DriverJumpCreditCB.Text = "Jump";
            this.DriverJumpCreditCB.UseVisualStyleBackColor = true;
            this.DriverJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.DriverJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // ScoreJumpCreditCB
            // 
            this.ScoreJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScoreJumpCreditCB.AutoSize = true;
            this.ScoreJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScoreJumpCreditCB.Location = new System.Drawing.Point(582, 283);
            this.ScoreJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.ScoreJumpCreditCB.Name = "ScoreJumpCreditCB";
            this.ScoreJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.ScoreJumpCreditCB.TabIndex = 37;
            this.ScoreJumpCreditCB.Text = "Jump";
            this.ScoreJumpCreditCB.UseVisualStyleBackColor = true;
            this.ScoreJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.ScoreJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // SafetyJumpCreditCB
            // 
            this.SafetyJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyJumpCreditCB.AutoSize = true;
            this.SafetyJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyJumpCreditCB.Location = new System.Drawing.Point(582, 361);
            this.SafetyJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.SafetyJumpCreditCB.Name = "SafetyJumpCreditCB";
            this.SafetyJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.SafetyJumpCreditCB.TabIndex = 47;
            this.SafetyJumpCreditCB.Text = "Jump";
            this.SafetyJumpCreditCB.UseVisualStyleBackColor = true;
            this.SafetyJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.SafetyJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // TechJumpCreditCB
            // 
            this.TechJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechJumpCreditCB.AutoSize = true;
            this.TechJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechJumpCreditCB.Location = new System.Drawing.Point(582, 439);
            this.TechJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.TechJumpCreditCB.Name = "TechJumpCreditCB";
            this.TechJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.TechJumpCreditCB.TabIndex = 57;
            this.TechJumpCreditCB.Text = "Jump";
            this.TechJumpCreditCB.UseVisualStyleBackColor = true;
            this.TechJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.TechJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // JudgeLabel
            // 
            this.JudgeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.JudgeLabel.AutoSize = true;
            this.JudgeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.JudgeLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JudgeLabel.Location = new System.Drawing.Point(489, 62);
            this.JudgeLabel.Name = "JudgeLabel";
            this.JudgeLabel.Padding = new System.Windows.Forms.Padding(81, 1, 81, 1);
            this.JudgeLabel.Size = new System.Drawing.Size(267, 20);
            this.JudgeLabel.TabIndex = 0;
            this.JudgeLabel.Text = "3 Event Judges";
            // 
            // DriverLabel
            // 
            this.DriverLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DriverLabel.AutoSize = true;
            this.DriverLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DriverLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DriverLabel.Location = new System.Drawing.Point(489, 143);
            this.DriverLabel.Name = "DriverLabel";
            this.DriverLabel.Padding = new System.Windows.Forms.Padding(107, 1, 106, 1);
            this.DriverLabel.Size = new System.Drawing.Size(267, 20);
            this.DriverLabel.TabIndex = 0;
            this.DriverLabel.Text = "Drivers";
            // 
            // ScorerLabel
            // 
            this.ScorerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ScorerLabel.AutoSize = true;
            this.ScorerLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ScorerLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScorerLabel.Location = new System.Drawing.Point(489, 223);
            this.ScorerLabel.Name = "ScorerLabel";
            this.ScorerLabel.Padding = new System.Windows.Forms.Padding(105, 1, 104, 1);
            this.ScorerLabel.Size = new System.Drawing.Size(267, 20);
            this.ScorerLabel.TabIndex = 0;
            this.ScorerLabel.Text = "Scorers";
            // 
            // SafetyLabel
            // 
            this.SafetyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SafetyLabel.AutoSize = true;
            this.SafetyLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SafetyLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SafetyLabel.Location = new System.Drawing.Point(489, 303);
            this.SafetyLabel.Name = "SafetyLabel";
            this.SafetyLabel.Padding = new System.Windows.Forms.Padding(79, 1, 78, 1);
            this.SafetyLabel.Size = new System.Drawing.Size(267, 20);
            this.SafetyLabel.TabIndex = 0;
            this.SafetyLabel.Text = "Safety Directors";
            // 
            // TechnicalLabel
            // 
            this.TechnicalLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechnicalLabel.AutoSize = true;
            this.TechnicalLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TechnicalLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechnicalLabel.Location = new System.Drawing.Point(489, 382);
            this.TechnicalLabel.Name = "TechnicalLabel";
            this.TechnicalLabel.Padding = new System.Windows.Forms.Padding(62, 1, 61, 1);
            this.TechnicalLabel.Size = new System.Drawing.Size(267, 20);
            this.TechnicalLabel.TabIndex = 0;
            this.TechnicalLabel.Text = "Technical Controllers";
            // 
            // AnncrLabel
            // 
            this.AnncrLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnncrLabel.AutoSize = true;
            this.AnncrLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.AnncrLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnncrLabel.Location = new System.Drawing.Point(489, 457);
            this.AnncrLabel.Name = "AnncrLabel";
            this.AnncrLabel.Padding = new System.Windows.Forms.Padding(91, 1, 91, 1);
            this.AnncrLabel.Size = new System.Drawing.Size(267, 20);
            this.AnncrLabel.TabIndex = 0;
            this.AnncrLabel.Text = "Announcers";
            this.AnncrLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AnncrJumpCreditCB
            // 
            this.AnncrJumpCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnncrJumpCreditCB.AutoSize = true;
            this.AnncrJumpCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnncrJumpCreditCB.Location = new System.Drawing.Point(582, 517);
            this.AnncrJumpCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.AnncrJumpCreditCB.Name = "AnncrJumpCreditCB";
            this.AnncrJumpCreditCB.Size = new System.Drawing.Size(54, 18);
            this.AnncrJumpCreditCB.TabIndex = 67;
            this.AnncrJumpCreditCB.Text = "Jump";
            this.AnncrJumpCreditCB.UseVisualStyleBackColor = true;
            this.AnncrJumpCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.AnncrJumpCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // AnncrTrickCreditCB
            // 
            this.AnncrTrickCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnncrTrickCreditCB.AutoSize = true;
            this.AnncrTrickCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnncrTrickCreditCB.Location = new System.Drawing.Point(582, 497);
            this.AnncrTrickCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.AnncrTrickCreditCB.Name = "AnncrTrickCreditCB";
            this.AnncrTrickCreditCB.Size = new System.Drawing.Size(50, 18);
            this.AnncrTrickCreditCB.TabIndex = 66;
            this.AnncrTrickCreditCB.Text = "Trick";
            this.AnncrTrickCreditCB.UseVisualStyleBackColor = true;
            this.AnncrTrickCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.AnncrTrickCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // AnncrSlalomCreditCB
            // 
            this.AnncrSlalomCreditCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnncrSlalomCreditCB.AutoSize = true;
            this.AnncrSlalomCreditCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnncrSlalomCreditCB.Location = new System.Drawing.Point(582, 478);
            this.AnncrSlalomCreditCB.Margin = new System.Windows.Forms.Padding(1);
            this.AnncrSlalomCreditCB.Name = "AnncrSlalomCreditCB";
            this.AnncrSlalomCreditCB.Size = new System.Drawing.Size(64, 18);
            this.AnncrSlalomCreditCB.TabIndex = 65;
            this.AnncrSlalomCreditCB.Text = "Slalom";
            this.AnncrSlalomCreditCB.UseVisualStyleBackColor = true;
            this.AnncrSlalomCreditCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.AnncrSlalomCreditCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // editAnncrOfficialRating
            // 
            this.editAnncrOfficialRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editAnncrOfficialRating.BackColor = System.Drawing.SystemColors.Info;
            this.editAnncrOfficialRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editAnncrOfficialRating.Enabled = false;
            this.editAnncrOfficialRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editAnncrOfficialRating.Location = new System.Drawing.Point(656, 480);
            this.editAnncrOfficialRating.Name = "editAnncrOfficialRating";
            this.editAnncrOfficialRating.ReadOnly = true;
            this.editAnncrOfficialRating.Size = new System.Drawing.Size(100, 14);
            this.editAnncrOfficialRating.TabIndex = 0;
            this.editAnncrOfficialRating.TabStop = false;
            // 
            // AnncrChiefCB
            // 
            this.AnncrChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AnncrChiefCB.AutoSize = true;
            this.AnncrChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AnncrChiefCB.Location = new System.Drawing.Point(489, 478);
            this.AnncrChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.AnncrChiefCB.Name = "AnncrChiefCB";
            this.AnncrChiefCB.Size = new System.Drawing.Size(54, 18);
            this.AnncrChiefCB.TabIndex = 60;
            this.AnncrChiefCB.Text = "Chief";
            this.AnncrChiefCB.UseVisualStyleBackColor = true;
            this.AnncrChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.AnncrChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // mainNavMenu
            // 
            this.mainNavMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navPrint,
            this.navExport,
            this.ExportMemberList,
            this.exportCreditFile,
            this.navFilter,
            this.navSort,
            this.navSaveItem,
            this.navEditOfficials});
            this.mainNavMenu.Location = new System.Drawing.Point(0, 0);
            this.mainNavMenu.Name = "mainNavMenu";
            this.mainNavMenu.Size = new System.Drawing.Size(764, 38);
            this.mainNavMenu.TabIndex = 83;
            this.mainNavMenu.Text = "toolStrip1";
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
            this.navPrint.Click += new System.EventHandler(this.printButton_Click);
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
            // ExportMemberList
            // 
            this.ExportMemberList.Image = ((System.Drawing.Image)(resources.GetObject("ExportMemberList.Image")));
            this.ExportMemberList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExportMemberList.Name = "ExportMemberList";
            this.ExportMemberList.Size = new System.Drawing.Size(65, 35);
            this.ExportMemberList.Text = "Export List";
            this.ExportMemberList.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ExportMemberList.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.ExportMemberList.Click += new System.EventHandler(this.ExportMemberList_Click);
            // 
            // navFilter
            // 
            this.navFilter.Image = ((System.Drawing.Image)(resources.GetObject("navFilter.Image")));
            this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFilter.Name = "navFilter";
            this.navFilter.Size = new System.Drawing.Size(37, 35);
            this.navFilter.Text = "Filter";
            this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
            // 
            // navSort
            // 
            this.navSort.Image = ((System.Drawing.Image)(resources.GetObject("navSort.Image")));
            this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSort.Name = "navSort";
            this.navSort.Size = new System.Drawing.Size(32, 35);
            this.navSort.Text = "Sort";
            this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSort.Click += new System.EventHandler(this.navSort_Click);
            // 
            // navSaveItem
            // 
            this.navSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("navSaveItem.Image")));
            this.navSaveItem.Name = "navSaveItem";
            this.navSaveItem.Size = new System.Drawing.Size(35, 35);
            this.navSaveItem.Text = "Save";
            this.navSaveItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSaveItem.Click += new System.EventHandler(this.navSaveItem_Click);
            // 
            // navEditOfficials
            // 
            this.navEditOfficials.Image = ((System.Drawing.Image)(resources.GetObject("navEditOfficials.Image")));
            this.navEditOfficials.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navEditOfficials.Name = "navEditOfficials";
            this.navEditOfficials.Size = new System.Drawing.Size(31, 35);
            this.navEditOfficials.Text = "Edit";
            this.navEditOfficials.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navEditOfficials.Click += new System.EventHandler(this.navEditOfficials_Click);
            // 
            // TechAsstChiefCB
            // 
            this.TechAsstChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechAsstChiefCB.AutoSize = true;
            this.TechAsstChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechAsstChiefCB.Location = new System.Drawing.Point(489, 421);
            this.TechAsstChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.TechAsstChiefCB.Name = "TechAsstChiefCB";
            this.TechAsstChiefCB.Size = new System.Drawing.Size(80, 18);
            this.TechAsstChiefCB.TabIndex = 85;
            this.TechAsstChiefCB.Text = "Asst Chief";
            this.TechAsstChiefCB.UseVisualStyleBackColor = true;
            this.TechAsstChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.TechAsstChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // TechChiefCB
            // 
            this.TechChiefCB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TechChiefCB.AutoSize = true;
            this.TechChiefCB.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TechChiefCB.Location = new System.Drawing.Point(489, 403);
            this.TechChiefCB.Margin = new System.Windows.Forms.Padding(1);
            this.TechChiefCB.Name = "TechChiefCB";
            this.TechChiefCB.Size = new System.Drawing.Size(54, 18);
            this.TechChiefCB.TabIndex = 84;
            this.TechChiefCB.Text = "Chief";
            this.TechChiefCB.UseVisualStyleBackColor = true;
            this.TechChiefCB.CheckedChanged += new System.EventHandler(this.ItemChanged);
            this.TechChiefCB.Click += new System.EventHandler(this.ItemClick);
            // 
            // editDriverTrickRating
            // 
            this.editDriverTrickRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editDriverTrickRating.BackColor = System.Drawing.SystemColors.Info;
            this.editDriverTrickRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editDriverTrickRating.Enabled = false;
            this.editDriverTrickRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editDriverTrickRating.Location = new System.Drawing.Point(656, 186);
            this.editDriverTrickRating.Name = "editDriverTrickRating";
            this.editDriverTrickRating.ReadOnly = true;
            this.editDriverTrickRating.Size = new System.Drawing.Size(100, 14);
            this.editDriverTrickRating.TabIndex = 86;
            this.editDriverTrickRating.TabStop = false;
            // 
            // editDriverJumpRating
            // 
            this.editDriverJumpRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editDriverJumpRating.BackColor = System.Drawing.SystemColors.Info;
            this.editDriverJumpRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editDriverJumpRating.Enabled = false;
            this.editDriverJumpRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editDriverJumpRating.Location = new System.Drawing.Point(656, 206);
            this.editDriverJumpRating.Name = "editDriverJumpRating";
            this.editDriverJumpRating.ReadOnly = true;
            this.editDriverJumpRating.Size = new System.Drawing.Size(100, 14);
            this.editDriverJumpRating.TabIndex = 87;
            this.editDriverJumpRating.TabStop = false;
            // 
            // editScorerTrickRating
            // 
            this.editScorerTrickRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editScorerTrickRating.BackColor = System.Drawing.SystemColors.Info;
            this.editScorerTrickRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editScorerTrickRating.Enabled = false;
            this.editScorerTrickRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editScorerTrickRating.Location = new System.Drawing.Point(656, 265);
            this.editScorerTrickRating.Name = "editScorerTrickRating";
            this.editScorerTrickRating.ReadOnly = true;
            this.editScorerTrickRating.Size = new System.Drawing.Size(100, 14);
            this.editScorerTrickRating.TabIndex = 88;
            this.editScorerTrickRating.TabStop = false;
            // 
            // editScorerJumpRating
            // 
            this.editScorerJumpRating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editScorerJumpRating.BackColor = System.Drawing.SystemColors.Info;
            this.editScorerJumpRating.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.editScorerJumpRating.Enabled = false;
            this.editScorerJumpRating.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editScorerJumpRating.Location = new System.Drawing.Point(656, 285);
            this.editScorerJumpRating.Name = "editScorerJumpRating";
            this.editScorerJumpRating.ReadOnly = true;
            this.editScorerJumpRating.Size = new System.Drawing.Size(100, 14);
            this.editScorerJumpRating.TabIndex = 89;
            this.editScorerJumpRating.TabStop = false;
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(7, 44);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 90;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // exportCreditFile
            // 
            this.exportCreditFile.Image = ((System.Drawing.Image)(resources.GetObject("exportCreditFile.Image")));
            this.exportCreditFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportCreditFile.Name = "exportCreditFile";
            this.exportCreditFile.Size = new System.Drawing.Size(100, 35);
            this.exportCreditFile.Text = "Export Credit File";
            this.exportCreditFile.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.exportCreditFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.exportCreditFile.Click += new System.EventHandler(this.exportCreditFile_Click);
            // 
            // OfficialWorkRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 585);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.editScorerJumpRating);
            this.Controls.Add(this.editScorerTrickRating);
            this.Controls.Add(this.editDriverJumpRating);
            this.Controls.Add(this.editDriverTrickRating);
            this.Controls.Add(this.TechAsstChiefCB);
            this.Controls.Add(this.TechChiefCB);
            this.Controls.Add(this.mainNavMenu);
            this.Controls.Add(this.AnncrChiefCB);
            this.Controls.Add(this.AnncrLabel);
            this.Controls.Add(this.AnncrJumpCreditCB);
            this.Controls.Add(this.AnncrTrickCreditCB);
            this.Controls.Add(this.AnncrSlalomCreditCB);
            this.Controls.Add(this.editAnncrOfficialRating);
            this.Controls.Add(this.TechnicalLabel);
            this.Controls.Add(this.SafetyLabel);
            this.Controls.Add(this.ScorerLabel);
            this.Controls.Add(this.DriverLabel);
            this.Controls.Add(this.JudgeLabel);
            this.Controls.Add(this.TechJumpCreditCB);
            this.Controls.Add(this.SafetyJumpCreditCB);
            this.Controls.Add(this.ScoreJumpCreditCB);
            this.Controls.Add(this.DriverJumpCreditCB);
            this.Controls.Add(this.JudgeJumpCreditCB);
            this.Controls.Add(this.TechTrickCreditCB);
            this.Controls.Add(this.SafetyTrickCreditCB);
            this.Controls.Add(this.ScoreTrickCreditCB);
            this.Controls.Add(this.DriverTrickCreditCB);
            this.Controls.Add(this.JudgeTrickCreditCB);
            this.Controls.Add(this.SafetySlalomCreditCB);
            this.Controls.Add(this.ScoreSlalomCreditCB);
            this.Controls.Add(this.DriverSlalomCreditCB);
            this.Controls.Add(this.JudgeSlalomCreditCB);
            this.Controls.Add(this.SafetyAppointedCB);
            this.Controls.Add(this.SafetyAsstChiefCB);
            this.Controls.Add(this.ScoreAppointedCB);
            this.Controls.Add(this.ScoreAsstChiefCB);
            this.Controls.Add(this.DriverAppointedCB);
            this.Controls.Add(this.DriverAsstChiefCB);
            this.Controls.Add(this.JudgeAppointedCB);
            this.Controls.Add(this.JudgeAsstChiefCB);
            this.Controls.Add(this.SafetyChiefCB);
            this.Controls.Add(this.ScoreChiefCB);
            this.Controls.Add(this.DriverChiefCB);
            this.Controls.Add(this.TechSlalomCreditCB);
            this.Controls.Add(this.JudgeChiefCB);
            this.Controls.Add(this.editPK);
            this.Controls.Add(this.editSanctionId);
            this.Controls.Add(this.editMemberId);
            this.Controls.Add(this.editJudgeSlalomRating);
            this.Controls.Add(this.editJudgeJumpRating);
            this.Controls.Add(this.editJudgeTrickRating);
            this.Controls.Add(this.editSafetyOfficialRating);
            this.Controls.Add(this.editScorerSlalomRating);
            this.Controls.Add(this.editTechOfficialRating);
            this.Controls.Add(this.editDriverSlalomRating);
            this.Controls.Add(noteLabel);
            this.Controls.Add(this.editNote);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.listTourMemberDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OfficialWorkRecord";
            this.Text = "Official Work Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OfficialWorkRecord_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OfficialWorkRecord_FormClosed);
            this.Load += new System.EventHandler(this.OfficialWorkRecord_Load);
            ((System.ComponentModel.ISupportInitialize)(this.listTourMemberDataGridView)).EndInit();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.mainNavMenu.ResumeLayout(false);
            this.mainNavMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView listTourMemberDataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.TextBox editPK;
        private System.Windows.Forms.TextBox editSanctionId;
        private System.Windows.Forms.TextBox editMemberId;
        private System.Windows.Forms.TextBox editJudgeSlalomRating;
        private System.Windows.Forms.TextBox editJudgeJumpRating;
        private System.Windows.Forms.TextBox editJudgeTrickRating;
        private System.Windows.Forms.TextBox editSafetyOfficialRating;
        private System.Windows.Forms.TextBox editScorerSlalomRating;
        private System.Windows.Forms.TextBox editTechOfficialRating;
        private System.Windows.Forms.TextBox editDriverSlalomRating;
        private System.Windows.Forms.TextBox editNote;
        private System.Windows.Forms.CheckBox JudgeChiefCB;
        private System.Windows.Forms.CheckBox TechSlalomCreditCB;
        private System.Windows.Forms.CheckBox DriverChiefCB;
        private System.Windows.Forms.CheckBox ScoreChiefCB;
        private System.Windows.Forms.CheckBox SafetyChiefCB;
        private System.Windows.Forms.CheckBox JudgeAsstChiefCB;
        private System.Windows.Forms.CheckBox JudgeAppointedCB;
        private System.Windows.Forms.CheckBox DriverAsstChiefCB;
        private System.Windows.Forms.CheckBox DriverAppointedCB;
        private System.Windows.Forms.CheckBox ScoreAsstChiefCB;
        private System.Windows.Forms.CheckBox ScoreAppointedCB;
        private System.Windows.Forms.CheckBox SafetyAsstChiefCB;
        private System.Windows.Forms.CheckBox SafetyAppointedCB;
        private System.Windows.Forms.CheckBox JudgeSlalomCreditCB;
        private System.Windows.Forms.CheckBox DriverSlalomCreditCB;
        private System.Windows.Forms.CheckBox ScoreSlalomCreditCB;
        private System.Windows.Forms.CheckBox SafetySlalomCreditCB;
        private System.Windows.Forms.CheckBox JudgeTrickCreditCB;
        private System.Windows.Forms.CheckBox DriverTrickCreditCB;
        private System.Windows.Forms.CheckBox ScoreTrickCreditCB;
        private System.Windows.Forms.CheckBox SafetyTrickCreditCB;
        private System.Windows.Forms.CheckBox TechTrickCreditCB;
        private System.Windows.Forms.CheckBox JudgeJumpCreditCB;
        private System.Windows.Forms.CheckBox DriverJumpCreditCB;
        private System.Windows.Forms.CheckBox ScoreJumpCreditCB;
        private System.Windows.Forms.CheckBox SafetyJumpCreditCB;
        private System.Windows.Forms.CheckBox TechJumpCreditCB;
        private System.Windows.Forms.Label JudgeLabel;
        private System.Windows.Forms.Label DriverLabel;
        private System.Windows.Forms.Label ScorerLabel;
        private System.Windows.Forms.Label SafetyLabel;
        private System.Windows.Forms.Label TechnicalLabel;
        private System.Windows.Forms.Label AnncrLabel;
        private System.Windows.Forms.CheckBox AnncrJumpCreditCB;
        private System.Windows.Forms.CheckBox AnncrTrickCreditCB;
        private System.Windows.Forms.CheckBox AnncrSlalomCreditCB;
        private System.Windows.Forms.TextBox editAnncrOfficialRating;
        private System.Windows.Forms.CheckBox AnncrChiefCB;
        private System.Windows.Forms.ToolStrip mainNavMenu;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton ExportMemberList;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navSaveItem;
        private System.Windows.Forms.CheckBox TechAsstChiefCB;
        private System.Windows.Forms.CheckBox TechChiefCB;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
        private System.Windows.Forms.DataGridViewTextBoxColumn JudgeSlalomRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn JudgeTrickRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn JudgeJumpRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverSlalomRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverTrickRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverJumpRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScorerSlalomRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScorerTrickRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScorerJumpRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafetyOfficialRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn TechOfficialRatingDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn AnncrOfficialRatingDesc;
        private System.Windows.Forms.TextBox editDriverTrickRating;
        private System.Windows.Forms.TextBox editDriverJumpRating;
        private System.Windows.Forms.TextBox editScorerTrickRating;
        private System.Windows.Forms.TextBox editScorerJumpRating;
        private System.Windows.Forms.ToolStripButton navEditOfficials;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.ToolStripButton exportCreditFile;
    }
}