namespace WaterskiScoringSystem.Trick {
    partial class SkierVideoMatchDialogForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkierVideoMatchDialogForm));
            this.roundActiveSelect = new WaterskiScoringSystem.Common.RoundSelect();
            this.Pass1RadioButton = new System.Windows.Forms.RadioButton();
            this.Pass2RadioButton = new System.Windows.Forms.RadioButton();
            this.PassSelectBox = new System.Windows.Forms.GroupBox();
            this.FileNameTextbox = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.previewDataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoFileNameLabel = new System.Windows.Forms.Label();
            this.SelectRoundLabel = new System.Windows.Forms.Label();
            this.SelectPassLabel = new System.Windows.Forms.Label();
            this.SelectSkierLabel = new System.Windows.Forms.Label();
            this.PassSelectBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // roundActiveSelect
            // 
            this.roundActiveSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundActiveSelect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.roundActiveSelect.Location = new System.Drawing.Point(105, 39);
            this.roundActiveSelect.Name = "roundActiveSelect";
            this.roundActiveSelect.RoundValue = "";
            this.roundActiveSelect.Size = new System.Drawing.Size(350, 29);
            this.roundActiveSelect.TabIndex = 1;
            this.roundActiveSelect.Tag = "";
            // 
            // Pass1RadioButton
            // 
            this.Pass1RadioButton.AutoSize = true;
            this.Pass1RadioButton.Location = new System.Drawing.Point(7, 12);
            this.Pass1RadioButton.Name = "Pass1RadioButton";
            this.Pass1RadioButton.Size = new System.Drawing.Size(57, 17);
            this.Pass1RadioButton.TabIndex = 2;
            this.Pass1RadioButton.TabStop = true;
            this.Pass1RadioButton.Text = "Pass 1";
            this.Pass1RadioButton.UseVisualStyleBackColor = true;
            // 
            // Pass2RadioButton
            // 
            this.Pass2RadioButton.AutoSize = true;
            this.Pass2RadioButton.Location = new System.Drawing.Point(67, 12);
            this.Pass2RadioButton.Name = "Pass2RadioButton";
            this.Pass2RadioButton.Size = new System.Drawing.Size(57, 17);
            this.Pass2RadioButton.TabIndex = 3;
            this.Pass2RadioButton.TabStop = true;
            this.Pass2RadioButton.Text = "Pass 2";
            this.Pass2RadioButton.UseVisualStyleBackColor = true;
            // 
            // PassSelectBox
            // 
            this.PassSelectBox.Controls.Add(this.Pass1RadioButton);
            this.PassSelectBox.Controls.Add(this.Pass2RadioButton);
            this.PassSelectBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PassSelectBox.Location = new System.Drawing.Point(105, 71);
            this.PassSelectBox.Name = "PassSelectBox";
            this.PassSelectBox.Size = new System.Drawing.Size(136, 36);
            this.PassSelectBox.TabIndex = 4;
            this.PassSelectBox.TabStop = false;
            // 
            // FileNameTextbox
            // 
            this.FileNameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileNameTextbox.Enabled = false;
            this.FileNameTextbox.Location = new System.Drawing.Point(105, 13);
            this.FileNameTextbox.Name = "FileNameTextbox";
            this.FileNameTextbox.Size = new System.Drawing.Size(350, 20);
            this.FileNameTextbox.TabIndex = 0;
            // 
            // OkButton
            // 
            this.OkButton.AutoSize = true;
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(16, 161);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 8;
            this.OkButton.Text = "Select";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.AutoSize = true;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(16, 190);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 9;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // previewDataGridView
            // 
            this.previewDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.previewDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.previewDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.SkierName,
            this.AgeGroup});
            this.previewDataGridView.Location = new System.Drawing.Point(105, 117);
            this.previewDataGridView.Name = "previewDataGridView";
            this.previewDataGridView.Size = new System.Drawing.Size(264, 203);
            this.previewDataGridView.TabIndex = 10;
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberId.HeaderText = "MemberId";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.Visible = false;
            this.MemberId.Width = 80;
            // 
            // SkierName
            // 
            this.SkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SkierName.HeaderText = "Skier";
            this.SkierName.Name = "SkierName";
            this.SkierName.ReadOnly = true;
            this.SkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkierName.Width = 150;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AgeGroup.HeaderText = "Div";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.Width = 40;
            // 
            // VideoFileNameLabel
            // 
            this.VideoFileNameLabel.AutoSize = true;
            this.VideoFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VideoFileNameLabel.Location = new System.Drawing.Point(25, 16);
            this.VideoFileNameLabel.Name = "VideoFileNameLabel";
            this.VideoFileNameLabel.Size = new System.Drawing.Size(77, 15);
            this.VideoFileNameLabel.TabIndex = 0;
            this.VideoFileNameLabel.Text = "File Name:";
            this.VideoFileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectRoundLabel
            // 
            this.SelectRoundLabel.AutoSize = true;
            this.SelectRoundLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectRoundLabel.Location = new System.Drawing.Point(5, 46);
            this.SelectRoundLabel.Name = "SelectRoundLabel";
            this.SelectRoundLabel.Size = new System.Drawing.Size(97, 15);
            this.SelectRoundLabel.TabIndex = 0;
            this.SelectRoundLabel.Text = "Select Round:";
            this.SelectRoundLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectPassLabel
            // 
            this.SelectPassLabel.AutoSize = true;
            this.SelectPassLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectPassLabel.Location = new System.Drawing.Point(16, 82);
            this.SelectPassLabel.Name = "SelectPassLabel";
            this.SelectPassLabel.Size = new System.Drawing.Size(86, 15);
            this.SelectPassLabel.TabIndex = 0;
            this.SelectPassLabel.Text = "Select Pass:";
            this.SelectPassLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SelectSkierLabel
            // 
            this.SelectSkierLabel.AutoSize = true;
            this.SelectSkierLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectSkierLabel.Location = new System.Drawing.Point(14, 117);
            this.SelectSkierLabel.Name = "SelectSkierLabel";
            this.SelectSkierLabel.Size = new System.Drawing.Size(88, 15);
            this.SelectSkierLabel.TabIndex = 0;
            this.SelectSkierLabel.Text = "Select Skier:";
            this.SelectSkierLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SkierVideoMatchDialogForm
            // 
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 323);
            this.Controls.Add(this.SelectSkierLabel);
            this.Controls.Add(this.SelectPassLabel);
            this.Controls.Add(this.SelectRoundLabel);
            this.Controls.Add(this.VideoFileNameLabel);
            this.Controls.Add(this.previewDataGridView);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.FileNameTextbox);
            this.Controls.Add(this.PassSelectBox);
            this.Controls.Add(this.roundActiveSelect);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SkierVideoMatchDialogForm";
            this.Text = "SkierVideoMatchDialogForm";
            this.Load += new System.EventHandler(this.SkierVideoMatchDialogForm_Load);
            this.PassSelectBox.ResumeLayout(false);
            this.PassSelectBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Common.RoundSelect roundActiveSelect;
        private System.Windows.Forms.RadioButton Pass1RadioButton;
        private System.Windows.Forms.RadioButton Pass2RadioButton;
        private System.Windows.Forms.GroupBox PassSelectBox;
        private System.Windows.Forms.TextBox FileNameTextbox;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.DataGridView previewDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.Label VideoFileNameLabel;
        private System.Windows.Forms.Label SelectRoundLabel;
        private System.Windows.Forms.Label SelectPassLabel;
        private System.Windows.Forms.Label SelectSkierLabel;
    }
}