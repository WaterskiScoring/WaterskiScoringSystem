namespace WaterskiScoringSystem.Tools {
    partial class RunOrderElimDialogForm {
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			this.NumSkiersLabel = new System.Windows.Forms.Label();
			this.NumSkiersTextbox = new System.Windows.Forms.TextBox();
			this.CancelButton = new System.Windows.Forms.Button();
			this.TotalScoreButton = new System.Windows.Forms.Button();
			this.LastScoreButton = new System.Windows.Forms.Button();
			this.BestScoreButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.PointsMethodGroupBox = new System.Windows.Forms.GroupBox();
			this.ratioPointsButton = new System.Windows.Forms.RadioButton();
			this.handicapPointsButton = new System.Windows.Forms.RadioButton();
			this.kBasePointsButton = new System.Windows.Forms.RadioButton();
			this.plcmtPointsButton = new System.Windows.Forms.RadioButton();
			this.nopsPointsButton = new System.Windows.Forms.RadioButton();
			this.plcmtGroupBox = new System.Windows.Forms.GroupBox();
			this.plcmtDivGrpButton = new System.Windows.Forms.RadioButton();
			this.plcmtDivButton = new System.Windows.Forms.RadioButton();
			this.plcmtTourButton = new System.Windows.Forms.RadioButton();
			this.groupPlcmtButton = new System.Windows.Forms.RadioButton();
			this.plcmtMethodGroupBox = new System.Windows.Forms.GroupBox();
			this.pointsScoreButton = new System.Windows.Forms.RadioButton();
			this.rawScoreButton = new System.Windows.Forms.RadioButton();
			this.RemoveUnscoredButton = new System.Windows.Forms.Button();
			this.previewDataGridView = new System.Windows.Forms.DataGridView();
			this.previewSanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewEvent = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewRound = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewSkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewAgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewEventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewRunOrderGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewplcmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewSeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PreviewOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PreviewScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.previewSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.SelectButton = new System.Windows.Forms.Button();
			this.RefreshButton = new System.Windows.Forms.Button();
			this.reseedButton = new System.Windows.Forms.RadioButton();
			this.h2hNextGroupBox = new System.Windows.Forms.GroupBox();
			this.bracketButton = new System.Windows.Forms.RadioButton();
			this.EventGroupListLabel = new System.Windows.Forms.Label();
			this.EventGroupList = new System.Windows.Forms.ComboBox();
			this.PointsMethodGroupBox.SuspendLayout();
			this.plcmtGroupBox.SuspendLayout();
			this.plcmtMethodGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).BeginInit();
			this.h2hNextGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// NumSkiersLabel
			// 
			this.NumSkiersLabel.AutoSize = true;
			this.NumSkiersLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NumSkiersLabel.Location = new System.Drawing.Point(495, 146);
			this.NumSkiersLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.NumSkiersLabel.Name = "NumSkiersLabel";
			this.NumSkiersLabel.Size = new System.Drawing.Size(98, 18);
			this.NumSkiersLabel.TabIndex = 0;
			this.NumSkiersLabel.Text = "# of Skiers";
			this.NumSkiersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// NumSkiersTextbox
			// 
			this.NumSkiersTextbox.Location = new System.Drawing.Point(605, 143);
			this.NumSkiersTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.NumSkiersTextbox.Name = "NumSkiersTextbox";
			this.NumSkiersTextbox.Size = new System.Drawing.Size(35, 22);
			this.NumSkiersTextbox.TabIndex = 72;
			this.NumSkiersTextbox.Validating += new System.ComponentModel.CancelEventHandler(this.NumSkiersTextbox_Validating);
			this.NumSkiersTextbox.Validated += new System.EventHandler(this.NumSkiersTextbox_Validated);
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(599, 41);
			this.CancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(100, 28);
			this.CancelButton.TabIndex = 27;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// TotalScoreButton
			// 
			this.TotalScoreButton.AutoSize = true;
			this.TotalScoreButton.Location = new System.Drawing.Point(385, 41);
			this.TotalScoreButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.TotalScoreButton.Name = "TotalScoreButton";
			this.TotalScoreButton.Size = new System.Drawing.Size(100, 28);
			this.TotalScoreButton.TabIndex = 23;
			this.TotalScoreButton.Text = "Total Score";
			this.TotalScoreButton.UseVisualStyleBackColor = true;
			this.TotalScoreButton.Click += new System.EventHandler(this.TotalScoreButton_Click);
			// 
			// LastScoreButton
			// 
			this.LastScoreButton.AutoSize = true;
			this.LastScoreButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.LastScoreButton.Location = new System.Drawing.Point(260, 41);
			this.LastScoreButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.LastScoreButton.Name = "LastScoreButton";
			this.LastScoreButton.Size = new System.Drawing.Size(114, 27);
			this.LastScoreButton.TabIndex = 22;
			this.LastScoreButton.Text = "Previous Score";
			this.LastScoreButton.UseVisualStyleBackColor = true;
			this.LastScoreButton.Click += new System.EventHandler(this.LastScoreButton_Click);
			// 
			// BestScoreButton
			// 
			this.BestScoreButton.AutoSize = true;
			this.BestScoreButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.BestScoreButton.Location = new System.Drawing.Point(161, 41);
			this.BestScoreButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.BestScoreButton.Name = "BestScoreButton";
			this.BestScoreButton.Size = new System.Drawing.Size(87, 27);
			this.BestScoreButton.TabIndex = 21;
			this.BestScoreButton.Text = "Best Score";
			this.BestScoreButton.UseVisualStyleBackColor = true;
			this.BestScoreButton.Click += new System.EventHandler(this.BestScoreButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(85, 11);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(483, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select methods to create running order for round ";
			// 
			// PointsMethodGroupBox
			// 
			this.PointsMethodGroupBox.Controls.Add(this.ratioPointsButton);
			this.PointsMethodGroupBox.Controls.Add(this.handicapPointsButton);
			this.PointsMethodGroupBox.Controls.Add(this.kBasePointsButton);
			this.PointsMethodGroupBox.Controls.Add(this.plcmtPointsButton);
			this.PointsMethodGroupBox.Controls.Add(this.nopsPointsButton);
			this.PointsMethodGroupBox.Location = new System.Drawing.Point(521, 73);
			this.PointsMethodGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.PointsMethodGroupBox.Name = "PointsMethodGroupBox";
			this.PointsMethodGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.PointsMethodGroupBox.Size = new System.Drawing.Size(317, 68);
			this.PointsMethodGroupBox.TabIndex = 60;
			this.PointsMethodGroupBox.TabStop = false;
			this.PointsMethodGroupBox.Text = "Points Calculation Method ";
			// 
			// ratioPointsButton
			// 
			this.ratioPointsButton.AutoSize = true;
			this.ratioPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ratioPointsButton.Location = new System.Drawing.Point(111, 44);
			this.ratioPointsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ratioPointsButton.Name = "ratioPointsButton";
			this.ratioPointsButton.Size = new System.Drawing.Size(98, 21);
			this.ratioPointsButton.TabIndex = 34;
			this.ratioPointsButton.Text = "Base Ratio";
			this.ratioPointsButton.UseVisualStyleBackColor = true;
			this.ratioPointsButton.CheckedChanged += new System.EventHandler(this.setPointsMethod_CheckedChanged);
			// 
			// handicapPointsButton
			// 
			this.handicapPointsButton.AutoSize = true;
			this.handicapPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.handicapPointsButton.Location = new System.Drawing.Point(111, 18);
			this.handicapPointsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.handicapPointsButton.Name = "handicapPointsButton";
			this.handicapPointsButton.Size = new System.Drawing.Size(89, 21);
			this.handicapPointsButton.TabIndex = 33;
			this.handicapPointsButton.Text = "Handicap";
			this.handicapPointsButton.UseVisualStyleBackColor = true;
			this.handicapPointsButton.CheckedChanged += new System.EventHandler(this.setPointsMethod_CheckedChanged);
			// 
			// kBasePointsButton
			// 
			this.kBasePointsButton.AutoSize = true;
			this.kBasePointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.kBasePointsButton.Location = new System.Drawing.Point(7, 44);
			this.kBasePointsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.kBasePointsButton.Name = "kBasePointsButton";
			this.kBasePointsButton.Size = new System.Drawing.Size(97, 21);
			this.kBasePointsButton.TabIndex = 32;
			this.kBasePointsButton.Text = "1000 Base";
			this.kBasePointsButton.UseVisualStyleBackColor = true;
			this.kBasePointsButton.CheckedChanged += new System.EventHandler(this.setPointsMethod_CheckedChanged);
			// 
			// plcmtPointsButton
			// 
			this.plcmtPointsButton.AutoSize = true;
			this.plcmtPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.plcmtPointsButton.Location = new System.Drawing.Point(218, 18);
			this.plcmtPointsButton.Margin = new System.Windows.Forms.Padding(0);
			this.plcmtPointsButton.Name = "plcmtPointsButton";
			this.plcmtPointsButton.Size = new System.Drawing.Size(95, 21);
			this.plcmtPointsButton.TabIndex = 35;
			this.plcmtPointsButton.Text = "Placement";
			this.plcmtPointsButton.UseVisualStyleBackColor = true;
			this.plcmtPointsButton.CheckedChanged += new System.EventHandler(this.setPointsMethod_CheckedChanged);
			// 
			// nopsPointsButton
			// 
			this.nopsPointsButton.AutoSize = true;
			this.nopsPointsButton.Checked = true;
			this.nopsPointsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.nopsPointsButton.Location = new System.Drawing.Point(7, 18);
			this.nopsPointsButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.nopsPointsButton.Name = "nopsPointsButton";
			this.nopsPointsButton.Size = new System.Drawing.Size(68, 21);
			this.nopsPointsButton.TabIndex = 31;
			this.nopsPointsButton.TabStop = true;
			this.nopsPointsButton.Text = "NOPS";
			this.nopsPointsButton.UseVisualStyleBackColor = true;
			this.nopsPointsButton.CheckedChanged += new System.EventHandler(this.setPointsMethod_CheckedChanged);
			// 
			// plcmtGroupBox
			// 
			this.plcmtGroupBox.Controls.Add(this.plcmtDivGrpButton);
			this.plcmtGroupBox.Controls.Add(this.plcmtDivButton);
			this.plcmtGroupBox.Controls.Add(this.plcmtTourButton);
			this.plcmtGroupBox.Controls.Add(this.groupPlcmtButton);
			this.plcmtGroupBox.Location = new System.Drawing.Point(319, 73);
			this.plcmtGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtGroupBox.Name = "plcmtGroupBox";
			this.plcmtGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtGroupBox.Size = new System.Drawing.Size(197, 68);
			this.plcmtGroupBox.TabIndex = 50;
			this.plcmtGroupBox.TabStop = false;
			this.plcmtGroupBox.Text = "Placement";
			// 
			// plcmtDivGrpButton
			// 
			this.plcmtDivGrpButton.AutoSize = true;
			this.plcmtDivGrpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.plcmtDivGrpButton.Location = new System.Drawing.Point(88, 44);
			this.plcmtDivGrpButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtDivGrpButton.Name = "plcmtDivGrpButton";
			this.plcmtDivGrpButton.Size = new System.Drawing.Size(93, 21);
			this.plcmtDivGrpButton.TabIndex = 54;
			this.plcmtDivGrpButton.Text = "Div/Group";
			this.plcmtDivGrpButton.UseVisualStyleBackColor = true;
			this.plcmtDivGrpButton.CheckedChanged += new System.EventHandler(this.setPlcmtOrg_CheckedChanged);
			// 
			// plcmtDivButton
			// 
			this.plcmtDivButton.AutoSize = true;
			this.plcmtDivButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.plcmtDivButton.Location = new System.Drawing.Point(7, 44);
			this.plcmtDivButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtDivButton.Name = "plcmtDivButton";
			this.plcmtDivButton.Size = new System.Drawing.Size(49, 21);
			this.plcmtDivButton.TabIndex = 52;
			this.plcmtDivButton.Text = "Div";
			this.plcmtDivButton.UseVisualStyleBackColor = true;
			this.plcmtDivButton.CheckedChanged += new System.EventHandler(this.setPlcmtOrg_CheckedChanged);
			// 
			// plcmtTourButton
			// 
			this.plcmtTourButton.AutoSize = true;
			this.plcmtTourButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.plcmtTourButton.Location = new System.Drawing.Point(88, 18);
			this.plcmtTourButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtTourButton.Name = "plcmtTourButton";
			this.plcmtTourButton.Size = new System.Drawing.Size(98, 21);
			this.plcmtTourButton.TabIndex = 53;
			this.plcmtTourButton.Text = "No Groups";
			this.plcmtTourButton.UseVisualStyleBackColor = true;
			this.plcmtTourButton.CheckedChanged += new System.EventHandler(this.setPlcmtOrg_CheckedChanged);
			// 
			// groupPlcmtButton
			// 
			this.groupPlcmtButton.AutoSize = true;
			this.groupPlcmtButton.Checked = true;
			this.groupPlcmtButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.groupPlcmtButton.Location = new System.Drawing.Point(7, 18);
			this.groupPlcmtButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.groupPlcmtButton.Name = "groupPlcmtButton";
			this.groupPlcmtButton.Size = new System.Drawing.Size(69, 21);
			this.groupPlcmtButton.TabIndex = 51;
			this.groupPlcmtButton.TabStop = true;
			this.groupPlcmtButton.Text = "Group";
			this.groupPlcmtButton.UseVisualStyleBackColor = true;
			this.groupPlcmtButton.CheckedChanged += new System.EventHandler(this.setPlcmtOrg_CheckedChanged);
			// 
			// plcmtMethodGroupBox
			// 
			this.plcmtMethodGroupBox.Controls.Add(this.pointsScoreButton);
			this.plcmtMethodGroupBox.Controls.Add(this.rawScoreButton);
			this.plcmtMethodGroupBox.Location = new System.Drawing.Point(161, 73);
			this.plcmtMethodGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtMethodGroupBox.Name = "plcmtMethodGroupBox";
			this.plcmtMethodGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.plcmtMethodGroupBox.Size = new System.Drawing.Size(152, 68);
			this.plcmtMethodGroupBox.TabIndex = 40;
			this.plcmtMethodGroupBox.TabStop = false;
			this.plcmtMethodGroupBox.Text = "Placement Method";
			// 
			// pointsScoreButton
			// 
			this.pointsScoreButton.AutoSize = true;
			this.pointsScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pointsScoreButton.Location = new System.Drawing.Point(8, 44);
			this.pointsScoreButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.pointsScoreButton.Name = "pointsScoreButton";
			this.pointsScoreButton.Size = new System.Drawing.Size(68, 21);
			this.pointsScoreButton.TabIndex = 42;
			this.pointsScoreButton.Text = "Points";
			this.pointsScoreButton.UseVisualStyleBackColor = true;
			this.pointsScoreButton.CheckedChanged += new System.EventHandler(this.setPlcmtMethod_CheckedChanged);
			// 
			// rawScoreButton
			// 
			this.rawScoreButton.AutoSize = true;
			this.rawScoreButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rawScoreButton.Location = new System.Drawing.Point(8, 18);
			this.rawScoreButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.rawScoreButton.Name = "rawScoreButton";
			this.rawScoreButton.Size = new System.Drawing.Size(97, 21);
			this.rawScoreButton.TabIndex = 41;
			this.rawScoreButton.Text = "Raw Score";
			this.rawScoreButton.UseVisualStyleBackColor = true;
			this.rawScoreButton.CheckedChanged += new System.EventHandler(this.setPlcmtMethod_CheckedChanged);
			// 
			// RemoveUnscoredButton
			// 
			this.RemoveUnscoredButton.AutoSize = true;
			this.RemoveUnscoredButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.RemoveUnscoredButton.Location = new System.Drawing.Point(7, 41);
			this.RemoveUnscoredButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.RemoveUnscoredButton.Name = "RemoveUnscoredButton";
			this.RemoveUnscoredButton.Size = new System.Drawing.Size(142, 27);
			this.RemoveUnscoredButton.TabIndex = 15;
			this.RemoveUnscoredButton.Text = "Remove Un-Scored";
			this.RemoveUnscoredButton.UseVisualStyleBackColor = true;
			this.RemoveUnscoredButton.Click += new System.EventHandler(this.RemoveUnscoredButton_Click);
			// 
			// previewDataGridView
			// 
			this.previewDataGridView.AllowUserToAddRows = false;
			this.previewDataGridView.AllowUserToDeleteRows = false;
			this.previewDataGridView.AllowUserToResizeColumns = false;
			this.previewDataGridView.AllowUserToResizeRows = false;
			this.previewDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.previewDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.previewDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.previewDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.previewDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.previewSanctionId,
            this.previewMemberId,
            this.previewEvent,
            this.previewRound,
            this.previewSkierName,
            this.previewAgeGroup,
            this.previewEventGroup,
            this.previewRunOrderGroup,
            this.previewplcmt,
            this.previewSeed,
            this.PreviewOrder,
            this.PreviewScore,
            this.previewSelected});
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.previewDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
			this.previewDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.previewDataGridView.Location = new System.Drawing.Point(47, 170);
			this.previewDataGridView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.previewDataGridView.Name = "previewDataGridView";
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle10.NullValue = null;
			dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.previewDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
			this.previewDataGridView.RowHeadersVisible = false;
			this.previewDataGridView.RowHeadersWidth = 31;
			this.previewDataGridView.Size = new System.Drawing.Size(739, 412);
			this.previewDataGridView.TabIndex = 100;
			// 
			// previewSanctionId
			// 
			this.previewSanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.previewSanctionId.HeaderText = "SanctionId";
			this.previewSanctionId.MaxInputLength = 6;
			this.previewSanctionId.Name = "previewSanctionId";
			this.previewSanctionId.ReadOnly = true;
			this.previewSanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewSanctionId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.previewSanctionId.Visible = false;
			this.previewSanctionId.Width = 70;
			// 
			// previewMemberId
			// 
			this.previewMemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.previewMemberId.HeaderText = "MemberId";
			this.previewMemberId.MaxInputLength = 9;
			this.previewMemberId.Name = "previewMemberId";
			this.previewMemberId.ReadOnly = true;
			this.previewMemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewMemberId.Visible = false;
			this.previewMemberId.Width = 70;
			// 
			// previewEvent
			// 
			this.previewEvent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.previewEvent.HeaderText = "Event";
			this.previewEvent.MaxInputLength = 6;
			this.previewEvent.Name = "previewEvent";
			this.previewEvent.ReadOnly = true;
			this.previewEvent.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewEvent.Visible = false;
			this.previewEvent.Width = 50;
			// 
			// previewRound
			// 
			this.previewRound.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.previewRound.DefaultCellStyle = dataGridViewCellStyle2;
			this.previewRound.HeaderText = "Round";
			this.previewRound.MaxInputLength = 2;
			this.previewRound.Name = "previewRound";
			this.previewRound.ReadOnly = true;
			this.previewRound.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewRound.Visible = false;
			this.previewRound.Width = 40;
			// 
			// previewSkierName
			// 
			this.previewSkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.previewSkierName.DefaultCellStyle = dataGridViewCellStyle3;
			this.previewSkierName.HeaderText = "Skier Name";
			this.previewSkierName.MaxInputLength = 32;
			this.previewSkierName.Name = "previewSkierName";
			this.previewSkierName.ReadOnly = true;
			this.previewSkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewSkierName.Width = 125;
			// 
			// previewAgeGroup
			// 
			this.previewAgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.previewAgeGroup.DefaultCellStyle = dataGridViewCellStyle4;
			this.previewAgeGroup.HeaderText = "Div";
			this.previewAgeGroup.MaxInputLength = 2;
			this.previewAgeGroup.Name = "previewAgeGroup";
			this.previewAgeGroup.ReadOnly = true;
			this.previewAgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewAgeGroup.Width = 45;
			// 
			// previewEventGroup
			// 
			this.previewEventGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.previewEventGroup.DefaultCellStyle = dataGridViewCellStyle5;
			this.previewEventGroup.HeaderText = "Group";
			this.previewEventGroup.MaxInputLength = 12;
			this.previewEventGroup.Name = "previewEventGroup";
			this.previewEventGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewEventGroup.Width = 50;
			// 
			// previewRunOrderGroup
			// 
			this.previewRunOrderGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.previewRunOrderGroup.HeaderText = "RO Grp";
			this.previewRunOrderGroup.MaxInputLength = 12;
			this.previewRunOrderGroup.Name = "previewRunOrderGroup";
			this.previewRunOrderGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewRunOrderGroup.Width = 50;
			// 
			// previewplcmt
			// 
			this.previewplcmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.previewplcmt.DefaultCellStyle = dataGridViewCellStyle6;
			this.previewplcmt.HeaderText = "Plcmt";
			this.previewplcmt.MaxInputLength = 4;
			this.previewplcmt.Name = "previewplcmt";
			this.previewplcmt.ReadOnly = true;
			this.previewplcmt.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewplcmt.Width = 45;
			// 
			// previewSeed
			// 
			this.previewSeed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.previewSeed.DefaultCellStyle = dataGridViewCellStyle7;
			this.previewSeed.HeaderText = "Seed";
			this.previewSeed.MaxInputLength = 3;
			this.previewSeed.Name = "previewSeed";
			this.previewSeed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewSeed.Width = 45;
			// 
			// PreviewOrder
			// 
			this.PreviewOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.PreviewOrder.DefaultCellStyle = dataGridViewCellStyle8;
			this.PreviewOrder.HeaderText = "Order";
			this.PreviewOrder.MaxInputLength = 3;
			this.PreviewOrder.Name = "PreviewOrder";
			this.PreviewOrder.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PreviewOrder.Width = 45;
			// 
			// PreviewScore
			// 
			this.PreviewScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PreviewScore.HeaderText = "Score";
			this.PreviewScore.MaxInputLength = 8;
			this.PreviewScore.Name = "PreviewScore";
			this.PreviewScore.ReadOnly = true;
			this.PreviewScore.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PreviewScore.Width = 55;
			// 
			// previewSelected
			// 
			this.previewSelected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.previewSelected.FillWeight = 45F;
			this.previewSelected.HeaderText = "Selected";
			this.previewSelected.Name = "previewSelected";
			this.previewSelected.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.previewSelected.Width = 50;
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(57, 145);
			this.RowStatusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(130, 18);
			this.RowStatusLabel.TabIndex = 0;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			// 
			// SelectButton
			// 
			this.SelectButton.AutoSize = true;
			this.SelectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SelectButton.Location = new System.Drawing.Point(492, 41);
			this.SelectButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.SelectButton.Name = "SelectButton";
			this.SelectButton.Size = new System.Drawing.Size(100, 28);
			this.SelectButton.TabIndex = 26;
			this.SelectButton.Text = "Select";
			this.SelectButton.UseVisualStyleBackColor = true;
			this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
			// 
			// RefreshButton
			// 
			this.RefreshButton.AutoSize = true;
			this.RefreshButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.RefreshButton.Location = new System.Drawing.Point(627, 10);
			this.RefreshButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.RefreshButton.Name = "RefreshButton";
			this.RefreshButton.Size = new System.Drawing.Size(68, 27);
			this.RefreshButton.TabIndex = 14;
			this.RefreshButton.Text = "Refresh";
			this.RefreshButton.UseVisualStyleBackColor = true;
			this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
			// 
			// reseedButton
			// 
			this.reseedButton.AutoSize = true;
			this.reseedButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reseedButton.Location = new System.Drawing.Point(5, 44);
			this.reseedButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.reseedButton.Name = "reseedButton";
			this.reseedButton.Size = new System.Drawing.Size(85, 21);
			this.reseedButton.TabIndex = 32;
			this.reseedButton.Text = "Re-Seed";
			this.reseedButton.UseVisualStyleBackColor = true;
			// 
			// h2hNextGroupBox
			// 
			this.h2hNextGroupBox.Controls.Add(this.bracketButton);
			this.h2hNextGroupBox.Controls.Add(this.reseedButton);
			this.h2hNextGroupBox.Location = new System.Drawing.Point(7, 73);
			this.h2hNextGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.h2hNextGroupBox.Name = "h2hNextGroupBox";
			this.h2hNextGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.h2hNextGroupBox.Size = new System.Drawing.Size(149, 68);
			this.h2hNextGroupBox.TabIndex = 30;
			this.h2hNextGroupBox.TabStop = false;
			this.h2hNextGroupBox.Text = "Matchup Method";
			// 
			// bracketButton
			// 
			this.bracketButton.AutoSize = true;
			this.bracketButton.Checked = true;
			this.bracketButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.bracketButton.Location = new System.Drawing.Point(5, 18);
			this.bracketButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.bracketButton.Name = "bracketButton";
			this.bracketButton.Size = new System.Drawing.Size(77, 21);
			this.bracketButton.TabIndex = 31;
			this.bracketButton.TabStop = true;
			this.bracketButton.Text = "Bracket";
			this.bracketButton.UseVisualStyleBackColor = true;
			// 
			// EventGroupListLabel
			// 
			this.EventGroupListLabel.AutoSize = true;
			this.EventGroupListLabel.Location = new System.Drawing.Point(255, 148);
			this.EventGroupListLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.EventGroupListLabel.Name = "EventGroupListLabel";
			this.EventGroupListLabel.Size = new System.Drawing.Size(96, 17);
			this.EventGroupListLabel.TabIndex = 0;
			this.EventGroupListLabel.Text = "Filter Division:";
			// 
			// EventGroupList
			// 
			this.EventGroupList.FormattingEnabled = true;
			this.EventGroupList.Location = new System.Drawing.Point(355, 142);
			this.EventGroupList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.EventGroupList.Name = "EventGroupList";
			this.EventGroupList.Size = new System.Drawing.Size(111, 24);
			this.EventGroupList.TabIndex = 71;
			// 
			// RunOrderElimDialogForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(844, 588);
			this.Controls.Add(this.EventGroupListLabel);
			this.Controls.Add(this.EventGroupList);
			this.Controls.Add(this.h2hNextGroupBox);
			this.Controls.Add(this.RefreshButton);
			this.Controls.Add(this.SelectButton);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.previewDataGridView);
			this.Controls.Add(this.RemoveUnscoredButton);
			this.Controls.Add(this.PointsMethodGroupBox);
			this.Controls.Add(this.plcmtGroupBox);
			this.Controls.Add(this.plcmtMethodGroupBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TotalScoreButton);
			this.Controls.Add(this.LastScoreButton);
			this.Controls.Add(this.BestScoreButton);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.NumSkiersLabel);
			this.Controls.Add(this.NumSkiersTextbox);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "RunOrderElimDialogForm";
			this.Text = "Run Order For Round";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RunOrderElimDialogForm_FormClosed);
			this.Load += new System.EventHandler(this.RunOrderElimDialogForm_Load);
			this.PointsMethodGroupBox.ResumeLayout(false);
			this.PointsMethodGroupBox.PerformLayout();
			this.plcmtGroupBox.ResumeLayout(false);
			this.plcmtGroupBox.PerformLayout();
			this.plcmtMethodGroupBox.ResumeLayout(false);
			this.plcmtMethodGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).EndInit();
			this.h2hNextGroupBox.ResumeLayout(false);
			this.h2hNextGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NumSkiersLabel;
        private System.Windows.Forms.TextBox NumSkiersTextbox;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button TotalScoreButton;
        private System.Windows.Forms.Button LastScoreButton;
        private System.Windows.Forms.Button BestScoreButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox PointsMethodGroupBox;
        private System.Windows.Forms.RadioButton ratioPointsButton;
        private System.Windows.Forms.RadioButton handicapPointsButton;
        private System.Windows.Forms.RadioButton kBasePointsButton;
        private System.Windows.Forms.RadioButton plcmtPointsButton;
        private System.Windows.Forms.RadioButton nopsPointsButton;
        private System.Windows.Forms.GroupBox plcmtGroupBox;
        private System.Windows.Forms.RadioButton plcmtDivGrpButton;
        private System.Windows.Forms.RadioButton plcmtDivButton;
        private System.Windows.Forms.RadioButton plcmtTourButton;
        private System.Windows.Forms.RadioButton groupPlcmtButton;
        private System.Windows.Forms.GroupBox plcmtMethodGroupBox;
        private System.Windows.Forms.RadioButton pointsScoreButton;
        private System.Windows.Forms.RadioButton rawScoreButton;
        private System.Windows.Forms.Button RemoveUnscoredButton;
        private System.Windows.Forms.DataGridView previewDataGridView;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.RadioButton reseedButton;
        private System.Windows.Forms.GroupBox h2hNextGroupBox;
        private System.Windows.Forms.RadioButton bracketButton;
        private System.Windows.Forms.Label EventGroupListLabel;
        private System.Windows.Forms.ComboBox EventGroupList;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewSanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewMemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewEvent;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewRound;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewSkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewAgeGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewEventGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewRunOrderGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewplcmt;
		private System.Windows.Forms.DataGridViewTextBoxColumn previewSeed;
		private System.Windows.Forms.DataGridViewTextBoxColumn PreviewOrder;
		private System.Windows.Forms.DataGridViewTextBoxColumn PreviewScore;
		private System.Windows.Forms.DataGridViewCheckBoxColumn previewSelected;
	}
}