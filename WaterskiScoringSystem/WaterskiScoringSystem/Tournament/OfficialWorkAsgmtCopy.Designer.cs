namespace WaterskiScoringSystem.Tournament {
    partial class OfficialWorkAsgmtCopy {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfficialWorkAsgmtCopy));
			this.AcceptButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.EventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CopyFromLabel = new System.Windows.Forms.Label();
			this.CopyToLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// AcceptButton
			// 
			this.AcceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.AcceptButton.Location = new System.Drawing.Point(48, 35);
			this.AcceptButton.Name = "AcceptButton";
			this.AcceptButton.Size = new System.Drawing.Size(75, 23);
			this.AcceptButton.TabIndex = 10;
			this.AcceptButton.Text = "OK";
			this.AcceptButton.UseVisualStyleBackColor = true;
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(143, 35);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 20;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// dataGridView
			// 
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.EventGroup,
            this.Round});
			this.dataGridView.Location = new System.Drawing.Point(12, 94);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(278, 194);
			this.dataGridView.TabIndex = 2;
			this.dataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentDoubleClick);
			this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
			this.dataGridView.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView_RowHeaderMouseClick);
			// 
			// EventGroup
			// 
			this.EventGroup.HeaderText = "Group";
			this.EventGroup.Name = "EventGroup";
			this.EventGroup.ReadOnly = true;
			// 
			// Round
			// 
			this.Round.HeaderText = "Round";
			this.Round.Name = "Round";
			this.Round.ReadOnly = true;
			// 
			// CopyFromLabel
			// 
			this.CopyFromLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CopyFromLabel.Location = new System.Drawing.Point(9, 9);
			this.CopyFromLabel.Name = "CopyFromLabel";
			this.CopyFromLabel.Size = new System.Drawing.Size(293, 23);
			this.CopyFromLabel.TabIndex = 0;
			this.CopyFromLabel.Text = "Copying from group xx round 1";
			this.CopyFromLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CopyToLabel
			// 
			this.CopyToLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CopyToLabel.ForeColor = System.Drawing.SystemColors.Highlight;
			this.CopyToLabel.Location = new System.Drawing.Point(12, 69);
			this.CopyToLabel.Name = "CopyToLabel";
			this.CopyToLabel.Size = new System.Drawing.Size(293, 23);
			this.CopyToLabel.TabIndex = 0;
			this.CopyToLabel.Text = "Select group and round to load";
			this.CopyToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// OfficialWorkAsgmtCopy
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(303, 297);
			this.Controls.Add(this.CopyToLabel);
			this.Controls.Add(this.CopyFromLabel);
			this.Controls.Add(this.dataGridView);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.AcceptButton);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "OfficialWorkAsgmtCopy";
			this.Text = "OfficialWorkAsgmtCopy";
			this.Load += new System.EventHandler(this.OfficialWorkAsgmtCopy_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button AcceptButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn Round;
        private System.Windows.Forms.Label CopyFromLabel;
        private System.Windows.Forms.Label CopyToLabel;
    }
}