namespace WaterskiScoringSystem.Tournament {
    partial class OfficialImport {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.GetOfficialsButton = new System.Windows.Forms.Button();
            this.WindowDescLabel = new System.Windows.Forms.Label();
            this.RegionListBox = new System.Windows.Forms.ListBox();
            this.StateListBox = new System.Windows.Forms.ListBox();
            this.memberIdTextBox = new System.Windows.Forms.TextBox();
            this.memberIdLabel = new System.Windows.Forms.Label();
            this.lastNameLabel = new System.Windows.Forms.Label();
            this.LastNameTextBox = new System.Windows.Forms.TextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.FirstNameTextBox = new System.Windows.Forms.TextBox();
            this.officialImportDataGridView = new System.Windows.Forms.DataGridView();
            this.ExportButton = new System.Windows.Forms.Button();
            this.BySanctionRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ByMemberIdRadioButton = new System.Windows.Forms.RadioButton();
            this.ByNameRadioButton = new System.Windows.Forms.RadioButton();
            this.ByStateRadioButton = new System.Windows.Forms.RadioButton();
            this.ProcessSelectionButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.officialImportDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GetOfficialsButton
            // 
            this.GetOfficialsButton.AutoSize = true;
            this.GetOfficialsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.GetOfficialsButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.GetOfficialsButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GetOfficialsButton.Location = new System.Drawing.Point(10, 199);
            this.GetOfficialsButton.Name = "GetOfficialsButton";
            this.GetOfficialsButton.Size = new System.Drawing.Size(170, 26);
            this.GetOfficialsButton.TabIndex = 51;
            this.GetOfficialsButton.Text = "Get Officials";
            this.GetOfficialsButton.UseVisualStyleBackColor = true;
            this.GetOfficialsButton.Click += new System.EventHandler(this.GetOfficialsButton_Click);
            // 
            // WindowDescLabel
            // 
            this.WindowDescLabel.AutoSize = true;
            this.WindowDescLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WindowDescLabel.Location = new System.Drawing.Point(95, 9);
            this.WindowDescLabel.Name = "WindowDescLabel";
            this.WindowDescLabel.Size = new System.Drawing.Size(440, 18);
            this.WindowDescLabel.TabIndex = 0;
            this.WindowDescLabel.Text = "Import Officials With Ratings From USA Water Ski";
            this.WindowDescLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // RegionListBox
            // 
            this.RegionListBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegionListBox.FormattingEnabled = true;
            this.RegionListBox.ItemHeight = 16;
            this.RegionListBox.Location = new System.Drawing.Point(10, 94);
            this.RegionListBox.Name = "RegionListBox";
            this.RegionListBox.Size = new System.Drawing.Size(200, 100);
            this.RegionListBox.Sorted = true;
            this.RegionListBox.TabIndex = 100;
            this.RegionListBox.SelectedIndexChanged += new System.EventHandler(this.regionListBox_SelectedIndexChanged);
            // 
            // StateListBox
            // 
            this.StateListBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StateListBox.FormattingEnabled = true;
            this.StateListBox.ItemHeight = 16;
            this.StateListBox.Location = new System.Drawing.Point(213, 94);
            this.StateListBox.Name = "StateListBox";
            this.StateListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.StateListBox.Size = new System.Drawing.Size(75, 100);
            this.StateListBox.TabIndex = 110;
            // 
            // memberIdTextBox
            // 
            this.memberIdTextBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memberIdTextBox.Location = new System.Drawing.Point(307, 116);
            this.memberIdTextBox.Name = "memberIdTextBox";
            this.memberIdTextBox.Size = new System.Drawing.Size(100, 23);
            this.memberIdTextBox.TabIndex = 120;
            // 
            // memberIdLabel
            // 
            this.memberIdLabel.AutoSize = true;
            this.memberIdLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memberIdLabel.Location = new System.Drawing.Point(316, 94);
            this.memberIdLabel.Name = "memberIdLabel";
            this.memberIdLabel.Size = new System.Drawing.Size(82, 16);
            this.memberIdLabel.TabIndex = 0;
            this.memberIdLabel.Text = "MemberId";
            // 
            // lastNameLabel
            // 
            this.lastNameLabel.AutoSize = true;
            this.lastNameLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastNameLabel.Location = new System.Drawing.Point(527, 94);
            this.lastNameLabel.Name = "lastNameLabel";
            this.lastNameLabel.Size = new System.Drawing.Size(85, 16);
            this.lastNameLabel.TabIndex = 0;
            this.lastNameLabel.Text = "Last Name";
            // 
            // LastNameTextBox
            // 
            this.LastNameTextBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastNameTextBox.Location = new System.Drawing.Point(519, 116);
            this.LastNameTextBox.Name = "LastNameTextBox";
            this.LastNameTextBox.Size = new System.Drawing.Size(100, 23);
            this.LastNameTextBox.TabIndex = 140;
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.firstNameLabel.Location = new System.Drawing.Point(421, 94);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(87, 16);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "First Name";
            // 
            // FirstNameTextBox
            // 
            this.FirstNameTextBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FirstNameTextBox.Location = new System.Drawing.Point(414, 116);
            this.FirstNameTextBox.Name = "FirstNameTextBox";
            this.FirstNameTextBox.Size = new System.Drawing.Size(100, 23);
            this.FirstNameTextBox.TabIndex = 130;
            // 
            // officialImportDataGridView
            // 
            this.officialImportDataGridView.AllowUserToAddRows = false;
            this.officialImportDataGridView.AllowUserToDeleteRows = false;
            this.officialImportDataGridView.AllowUserToResizeRows = false;
            this.officialImportDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.officialImportDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.officialImportDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.officialImportDataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.officialImportDataGridView.Location = new System.Drawing.Point(10, 231);
            this.officialImportDataGridView.Name = "officialImportDataGridView";
            this.officialImportDataGridView.RowHeadersVisible = false;
            this.officialImportDataGridView.Size = new System.Drawing.Size(779, 314);
            this.officialImportDataGridView.TabIndex = 200;
            // 
            // ExportButton
            // 
            this.ExportButton.AutoSize = true;
            this.ExportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ExportButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ExportButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExportButton.Location = new System.Drawing.Point(205, 199);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(170, 26);
            this.ExportButton.TabIndex = 52;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // BySanctionRadioButton
            // 
            this.BySanctionRadioButton.AutoSize = true;
            this.BySanctionRadioButton.Location = new System.Drawing.Point(6, 19);
            this.BySanctionRadioButton.Name = "BySanctionRadioButton";
            this.BySanctionRadioButton.Size = new System.Drawing.Size(103, 18);
            this.BySanctionRadioButton.TabIndex = 10;
            this.BySanctionRadioButton.TabStop = true;
            this.BySanctionRadioButton.Text = "By Sanction";
            this.BySanctionRadioButton.UseVisualStyleBackColor = true;
            this.BySanctionRadioButton.CheckedChanged += new System.EventHandler(this.BySanctionRadioButton_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ByMemberIdRadioButton);
            this.groupBox1.Controls.Add(this.ByNameRadioButton);
            this.groupBox1.Controls.Add(this.ByStateRadioButton);
            this.groupBox1.Controls.Add(this.BySanctionRadioButton);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(10, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(491, 52);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Get Officials By Criteria";
            // 
            // ByMemberIdRadioButton
            // 
            this.ByMemberIdRadioButton.AutoSize = true;
            this.ByMemberIdRadioButton.Location = new System.Drawing.Point(358, 19);
            this.ByMemberIdRadioButton.Name = "ByMemberIdRadioButton";
            this.ByMemberIdRadioButton.Size = new System.Drawing.Size(117, 18);
            this.ByMemberIdRadioButton.TabIndex = 13;
            this.ByMemberIdRadioButton.TabStop = true;
            this.ByMemberIdRadioButton.Text = "By Member Id";
            this.ByMemberIdRadioButton.UseVisualStyleBackColor = true;
            this.ByMemberIdRadioButton.CheckedChanged += new System.EventHandler(this.ByMemberIdRadioButton_CheckedChanged);
            // 
            // ByNameRadioButton
            // 
            this.ByNameRadioButton.AutoSize = true;
            this.ByNameRadioButton.Location = new System.Drawing.Point(246, 19);
            this.ByNameRadioButton.Name = "ByNameRadioButton";
            this.ByNameRadioButton.Size = new System.Drawing.Size(84, 18);
            this.ByNameRadioButton.TabIndex = 12;
            this.ByNameRadioButton.TabStop = true;
            this.ByNameRadioButton.Text = "By Name";
            this.ByNameRadioButton.UseVisualStyleBackColor = true;
            this.ByNameRadioButton.CheckedChanged += new System.EventHandler(this.ByNameRadioButton_CheckedChanged);
            // 
            // ByStateRadioButton
            // 
            this.ByStateRadioButton.AutoSize = true;
            this.ByStateRadioButton.Location = new System.Drawing.Point(137, 19);
            this.ByStateRadioButton.Name = "ByStateRadioButton";
            this.ByStateRadioButton.Size = new System.Drawing.Size(81, 18);
            this.ByStateRadioButton.TabIndex = 11;
            this.ByStateRadioButton.TabStop = true;
            this.ByStateRadioButton.Text = "By State";
            this.ByStateRadioButton.UseVisualStyleBackColor = true;
            this.ByStateRadioButton.CheckedChanged += new System.EventHandler(this.ByStateRadioButton_CheckedChanged);
            // 
            // ProcessSelectionButton
            // 
            this.ProcessSelectionButton.AutoSize = true;
            this.ProcessSelectionButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ProcessSelectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ProcessSelectionButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessSelectionButton.Location = new System.Drawing.Point(400, 199);
            this.ProcessSelectionButton.Name = "ProcessSelectionButton";
            this.ProcessSelectionButton.Size = new System.Drawing.Size(170, 26);
            this.ProcessSelectionButton.TabIndex = 53;
            this.ProcessSelectionButton.Text = "Process Selection";
            this.ProcessSelectionButton.UseVisualStyleBackColor = true;
            this.ProcessSelectionButton.Click += new System.EventHandler(this.ProcessSelectionButton_Click);
            // 
            // OfficialImport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(798, 553);
            this.Controls.Add(this.ProcessSelectionButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.officialImportDataGridView);
            this.Controls.Add(this.firstNameLabel);
            this.Controls.Add(this.FirstNameTextBox);
            this.Controls.Add(this.lastNameLabel);
            this.Controls.Add(this.LastNameTextBox);
            this.Controls.Add(this.memberIdLabel);
            this.Controls.Add(this.memberIdTextBox);
            this.Controls.Add(this.StateListBox);
            this.Controls.Add(this.RegionListBox);
            this.Controls.Add(this.WindowDescLabel);
            this.Controls.Add(this.GetOfficialsButton);
            this.Name = "OfficialImport";
            this.Text = "Member Import With Official Ratings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OfficialImport_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OfficialImport_FormClosed);
            this.Load += new System.EventHandler(this.OfficialImport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.officialImportDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button GetOfficialsButton;
        private System.Windows.Forms.Label WindowDescLabel;
        private System.Windows.Forms.ListBox RegionListBox;
        private System.Windows.Forms.ListBox StateListBox;
        private System.Windows.Forms.TextBox memberIdTextBox;
        private System.Windows.Forms.Label memberIdLabel;
        private System.Windows.Forms.Label lastNameLabel;
        private System.Windows.Forms.TextBox LastNameTextBox;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.TextBox FirstNameTextBox;
        private System.Windows.Forms.DataGridView officialImportDataGridView;
        private System.Windows.Forms.Button ExportButton;
        private System.Windows.Forms.RadioButton BySanctionRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton ByMemberIdRadioButton;
        private System.Windows.Forms.RadioButton ByNameRadioButton;
        private System.Windows.Forms.RadioButton ByStateRadioButton;
        private System.Windows.Forms.Button ProcessSelectionButton;
    }
}