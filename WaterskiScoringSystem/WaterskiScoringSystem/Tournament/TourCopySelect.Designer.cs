namespace WaterskiScoringSystem.Tournament {
    partial class TourCopySelect {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && ( components != null )) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TourCopySelect));
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.CopySelectButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.NewSanctionIdTextbox = new System.Windows.Forms.TextBox();
            this.NewSanctionIdlabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactMemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TourName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Class = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlalomRounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrickRounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpRounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rules = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventDates = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactPhone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactEmail = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RuleExceptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RuleInterpretations = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafetyDirPerfReport = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RopeHandlesSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlalomRopesSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpRopesSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlalomCourseSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpCourseSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrickCourseSpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BuoySpecs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContactName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            this.DataGridView.AllowUserToResizeRows = false;
            this.DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SanctionId,
            this.ContactMemberId,
            this.TourName,
            this.Class,
            this.Federation,
            this.SlalomRounds,
            this.TrickRounds,
            this.JumpRounds,
            this.Rules,
            this.EventDates,
            this.EventLocation,
            this.ContactPhone,
            this.ContactEmail,
            this.RuleExceptions,
            this.RuleInterpretations,
            this.SafetyDirPerfReport,
            this.RopeHandlesSpecs,
            this.SlalomRopesSpecs,
            this.JumpRopesSpecs,
            this.SlalomCourseSpecs,
            this.JumpCourseSpecs,
            this.TrickCourseSpecs,
            this.BuoySpecs,
            this.ContactName});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView.Location = new System.Drawing.Point(5, 53);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.Size = new System.Drawing.Size(698, 325);
            this.DataGridView.TabIndex = 4;
            this.DataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentDoubleClick);
            this.DataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridView_RowHeaderMouseClick);
            // 
            // CopySelectButton
            // 
            this.CopySelectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopySelectButton.Location = new System.Drawing.Point(192, 24);
            this.CopySelectButton.Name = "CopySelectButton";
            this.CopySelectButton.Size = new System.Drawing.Size(75, 23);
            this.CopySelectButton.TabIndex = 1;
            this.CopySelectButton.Text = "Copy";
            this.CopySelectButton.UseVisualStyleBackColor = true;
            this.CopySelectButton.Click += new System.EventHandler(this.CopySelectButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(466, 22);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(12, 26);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 0;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // NewSanctionIdTextbox
            // 
            this.NewSanctionIdTextbox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewSanctionIdTextbox.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.NewSanctionIdTextbox.Location = new System.Drawing.Point(397, 24);
            this.NewSanctionIdTextbox.MaxLength = 6;
            this.NewSanctionIdTextbox.Name = "NewSanctionIdTextbox";
            this.NewSanctionIdTextbox.Size = new System.Drawing.Size(60, 22);
            this.NewSanctionIdTextbox.TabIndex = 2;
            // 
            // NewSanctionIdlabel
            // 
            this.NewSanctionIdlabel.AutoSize = true;
            this.NewSanctionIdlabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewSanctionIdlabel.Location = new System.Drawing.Point(276, 28);
            this.NewSanctionIdlabel.Name = "NewSanctionIdlabel";
            this.NewSanctionIdlabel.Size = new System.Drawing.Size(112, 14);
            this.NewSanctionIdlabel.TabIndex = 0;
            this.NewSanctionIdlabel.Text = "New SanctionId";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(632, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter new sanction number in the text box provided then select an existing tourna" +
    "ment to copy";
            // 
            // SanctionId
            // 
            this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SanctionId.DataPropertyName = "SanctionId";
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.Width = 60;
            // 
            // ContactMemberId
            // 
            this.ContactMemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContactMemberId.DataPropertyName = "ContactMemberId";
            this.ContactMemberId.HeaderText = "ContactMemberId";
            this.ContactMemberId.Name = "ContactMemberId";
            this.ContactMemberId.ReadOnly = true;
            this.ContactMemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ContactMemberId.Visible = false;
            // 
            // TourName
            // 
            this.TourName.HeaderText = "Name";
            this.TourName.Name = "TourName";
            this.TourName.ReadOnly = true;
            this.TourName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TourName.Width = 125;
            // 
            // Class
            // 
            this.Class.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Class.DataPropertyName = "Class";
            this.Class.HeaderText = "Class";
            this.Class.Name = "Class";
            this.Class.ReadOnly = true;
            this.Class.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Class.Width = 35;
            // 
            // Federation
            // 
            this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Federation.DataPropertyName = "Federation";
            this.Federation.HeaderText = "Fed";
            this.Federation.Name = "Federation";
            this.Federation.ReadOnly = true;
            this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Federation.Width = 40;
            // 
            // SlalomRounds
            // 
            this.SlalomRounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomRounds.DataPropertyName = "SlalomRounds";
            this.SlalomRounds.HeaderText = "Slalom Rounds";
            this.SlalomRounds.Name = "SlalomRounds";
            this.SlalomRounds.ReadOnly = true;
            this.SlalomRounds.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomRounds.Width = 45;
            // 
            // TrickRounds
            // 
            this.TrickRounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickRounds.DataPropertyName = "TrickRounds";
            this.TrickRounds.HeaderText = "Trick Rounds";
            this.TrickRounds.Name = "TrickRounds";
            this.TrickRounds.ReadOnly = true;
            this.TrickRounds.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickRounds.Width = 45;
            // 
            // JumpRounds
            // 
            this.JumpRounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpRounds.DataPropertyName = "JumpRounds";
            this.JumpRounds.HeaderText = "Jump Rounds";
            this.JumpRounds.Name = "JumpRounds";
            this.JumpRounds.ReadOnly = true;
            this.JumpRounds.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpRounds.Width = 45;
            // 
            // Rules
            // 
            this.Rules.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Rules.DataPropertyName = "Rules";
            this.Rules.HeaderText = "Rules";
            this.Rules.Name = "Rules";
            this.Rules.ReadOnly = true;
            this.Rules.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Rules.Width = 40;
            // 
            // EventDates
            // 
            this.EventDates.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.EventDates.DataPropertyName = "EventDates";
            this.EventDates.HeaderText = "Event Dates";
            this.EventDates.Name = "EventDates";
            this.EventDates.ReadOnly = true;
            this.EventDates.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventDates.Width = 80;
            // 
            // EventLocation
            // 
            this.EventLocation.DataPropertyName = "EventLocation";
            this.EventLocation.HeaderText = "EventLocation";
            this.EventLocation.Name = "EventLocation";
            this.EventLocation.ReadOnly = true;
            this.EventLocation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ContactPhone
            // 
            this.ContactPhone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContactPhone.DataPropertyName = "ContactPhone";
            this.ContactPhone.HeaderText = "ContactPhone";
            this.ContactPhone.Name = "ContactPhone";
            this.ContactPhone.ReadOnly = true;
            this.ContactPhone.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ContactPhone.Visible = false;
            // 
            // ContactEmail
            // 
            this.ContactEmail.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContactEmail.DataPropertyName = "ContactEmail";
            this.ContactEmail.HeaderText = "Contact Email";
            this.ContactEmail.Name = "ContactEmail";
            this.ContactEmail.ReadOnly = true;
            this.ContactEmail.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ContactEmail.Visible = false;
            // 
            // RuleExceptions
            // 
            this.RuleExceptions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RuleExceptions.DataPropertyName = "RuleExceptions";
            this.RuleExceptions.HeaderText = "RuleExceptions";
            this.RuleExceptions.Name = "RuleExceptions";
            this.RuleExceptions.ReadOnly = true;
            this.RuleExceptions.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RuleExceptions.Visible = false;
            // 
            // RuleInterpretations
            // 
            this.RuleInterpretations.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RuleInterpretations.DataPropertyName = "RuleInterpretations";
            this.RuleInterpretations.HeaderText = "RuleInterpretations";
            this.RuleInterpretations.Name = "RuleInterpretations";
            this.RuleInterpretations.ReadOnly = true;
            this.RuleInterpretations.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RuleInterpretations.Visible = false;
            // 
            // SafetyDirPerfReport
            // 
            this.SafetyDirPerfReport.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SafetyDirPerfReport.DataPropertyName = "SafetyDirPerfReport";
            this.SafetyDirPerfReport.HeaderText = "SafetyDirPerfReport";
            this.SafetyDirPerfReport.Name = "SafetyDirPerfReport";
            this.SafetyDirPerfReport.ReadOnly = true;
            this.SafetyDirPerfReport.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SafetyDirPerfReport.Visible = false;
            // 
            // RopeHandlesSpecs
            // 
            this.RopeHandlesSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RopeHandlesSpecs.DataPropertyName = "RopeHandlesSpecs";
            this.RopeHandlesSpecs.HeaderText = "RopeHandlesSpecs";
            this.RopeHandlesSpecs.Name = "RopeHandlesSpecs";
            this.RopeHandlesSpecs.ReadOnly = true;
            this.RopeHandlesSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RopeHandlesSpecs.Visible = false;
            // 
            // SlalomRopesSpecs
            // 
            this.SlalomRopesSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomRopesSpecs.DataPropertyName = "SlalomRopesSpecs";
            this.SlalomRopesSpecs.HeaderText = "SlalomRopesSpecs";
            this.SlalomRopesSpecs.Name = "SlalomRopesSpecs";
            this.SlalomRopesSpecs.ReadOnly = true;
            this.SlalomRopesSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomRopesSpecs.Visible = false;
            // 
            // JumpRopesSpecs
            // 
            this.JumpRopesSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpRopesSpecs.DataPropertyName = "JumpRopesSpecs";
            this.JumpRopesSpecs.HeaderText = "JumpRopesSpecs";
            this.JumpRopesSpecs.Name = "JumpRopesSpecs";
            this.JumpRopesSpecs.ReadOnly = true;
            this.JumpRopesSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpRopesSpecs.Visible = false;
            // 
            // SlalomCourseSpecs
            // 
            this.SlalomCourseSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomCourseSpecs.DataPropertyName = "SlalomCourseSpecs";
            this.SlalomCourseSpecs.HeaderText = "SlalomCourseSpecs";
            this.SlalomCourseSpecs.Name = "SlalomCourseSpecs";
            this.SlalomCourseSpecs.ReadOnly = true;
            this.SlalomCourseSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomCourseSpecs.Visible = false;
            // 
            // JumpCourseSpecs
            // 
            this.JumpCourseSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpCourseSpecs.DataPropertyName = "JumpCourseSpecs";
            this.JumpCourseSpecs.HeaderText = "JumpCourseSpecs";
            this.JumpCourseSpecs.Name = "JumpCourseSpecs";
            this.JumpCourseSpecs.ReadOnly = true;
            this.JumpCourseSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpCourseSpecs.Visible = false;
            // 
            // TrickCourseSpecs
            // 
            this.TrickCourseSpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickCourseSpecs.DataPropertyName = "TrickCourseSpecs";
            this.TrickCourseSpecs.HeaderText = "TrickCourseSpecs";
            this.TrickCourseSpecs.Name = "TrickCourseSpecs";
            this.TrickCourseSpecs.ReadOnly = true;
            this.TrickCourseSpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickCourseSpecs.Visible = false;
            // 
            // BuoySpecs
            // 
            this.BuoySpecs.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.BuoySpecs.DataPropertyName = "BuoySpecs";
            this.BuoySpecs.HeaderText = "BuoySpecs";
            this.BuoySpecs.Name = "BuoySpecs";
            this.BuoySpecs.ReadOnly = true;
            this.BuoySpecs.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BuoySpecs.Visible = false;
            // 
            // ContactName
            // 
            this.ContactName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ContactName.DataPropertyName = "ContactName";
            this.ContactName.HeaderText = "ContactName";
            this.ContactName.Name = "ContactName";
            this.ContactName.ReadOnly = true;
            this.ContactName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ContactName.Visible = false;
            // 
            // TourCopySelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 390);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NewSanctionIdlabel);
            this.Controls.Add(this.NewSanctionIdTextbox);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.DataGridView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.CopySelectButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TourCopySelect";
            this.Text = "Tournament Copy Selection";
            this.Load += new System.EventHandler(this.TourCopySelect_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.Button CopySelectButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.TextBox NewSanctionIdTextbox;
        private System.Windows.Forms.Label NewSanctionIdlabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactMemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn TourName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Class;
        private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlalomRounds;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrickRounds;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpRounds;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rules;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventDates;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactPhone;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn RuleExceptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn RuleInterpretations;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafetyDirPerfReport;
        private System.Windows.Forms.DataGridViewTextBoxColumn RopeHandlesSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlalomRopesSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpRopesSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlalomCourseSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpCourseSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrickCourseSpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn BuoySpecs;
        private System.Windows.Forms.DataGridViewTextBoxColumn ContactName;
    }
}