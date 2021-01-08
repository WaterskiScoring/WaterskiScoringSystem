namespace WaterskiScoringSystem.Common {
	partial class BoatPathDetail {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BoatPathDetail));
			this.DataGridView = new System.Windows.Forms.DataGridView();
			this.boatPosition = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.boatDeviation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.winStatus.SuspendLayout();
			this.toolStrip1.SuspendLayout();
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
			this.DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
			this.DataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.boatPosition,
            this.boatDeviation});
			this.DataGridView.Location = new System.Drawing.Point(5, 73);
			this.DataGridView.Name = "DataGridView";
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.DataGridView.RowHeadersWidth = 30;
			this.DataGridView.Size = new System.Drawing.Size(224, 326);
			this.DataGridView.TabIndex = 2;
			this.DataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
			this.DataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
			// 
			// boatPosition
			// 
			this.boatPosition.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.boatPosition.HeaderText = "Position";
			this.boatPosition.Name = "boatPosition";
			this.boatPosition.ReadOnly = true;
			this.boatPosition.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.boatPosition.ToolTipText = "Boat position";
			this.boatPosition.Width = 69;
			// 
			// boatDeviation
			// 
			this.boatDeviation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.boatDeviation.HeaderText = "Deviation";
			this.boatDeviation.Name = "boatDeviation";
			this.boatDeviation.ReadOnly = true;
			this.boatDeviation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.boatDeviation.ToolTipText = "Deviation from center line";
			this.boatDeviation.Width = 77;
			// 
			// winStatus
			// 
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 402);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(241, 22);
			this.winStatus.TabIndex = 3;
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
			this.RowStatusLabel.Location = new System.Drawing.Point(5, 49);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 8;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navExport});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(241, 38);
			this.toolStrip1.TabIndex = 9;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// navExport
			// 
			this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
			this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navExport.Name = "navExport";
			this.navExport.Size = new System.Drawing.Size(45, 35);
			this.navExport.Text = "Export";
			this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navExport.Click += new System.EventHandler(this.navExport_Click);
			// 
			// BoatPathDetail
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(241, 424);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.winStatus);
			this.Controls.Add(this.DataGridView);
			this.Name = "BoatPathDetail";
			this.Text = "Boat Path Detail";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BoatPathDetail_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.BoatPathDetail_FormClosed);
			this.Load += new System.EventHandler(this.BoatPathDetail_Load);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.winStatus.ResumeLayout(false);
			this.winStatus.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Common.SortDialogForm sortDialogForm;
		private Common.FilterDialogForm filterDialogForm;
		private Tools.TourExportDialogForm exportDialogForm;
		private System.Windows.Forms.DataGridView DataGridView;
		private System.Windows.Forms.StatusStrip winStatus;
		private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton navExport;
		private System.Windows.Forms.DataGridViewTextBoxColumn boatPosition;
		private System.Windows.Forms.DataGridViewTextBoxColumn boatDeviation;
	}
}