
namespace WaterskiScoringSystem.Tools {
	partial class ExportFileDownload {
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
			this.DownloadButton = new System.Windows.Forms.Button();
			this.DataGridView = new System.Windows.Forms.DataGridView();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReportTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ExportFileUri = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// DownloadButton
			// 
			this.DownloadButton.AutoSize = true;
			this.DownloadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DownloadButton.Location = new System.Drawing.Point(6, 5);
			this.DownloadButton.Name = "DownloadButton";
			this.DownloadButton.Size = new System.Drawing.Size(195, 28);
			this.DownloadButton.TabIndex = 3;
			this.DownloadButton.Text = "Download Selected File";
			this.DownloadButton.UseVisualStyleBackColor = true;
			this.DownloadButton.Click += new System.EventHandler(this.DownloadButton_Click);
			// 
			// DataGridView
			// 
			this.DataGridView.AllowUserToAddRows = false;
			this.DataGridView.AllowUserToDeleteRows = false;
			this.DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.ReportTitle,
            this.ExportFileUri});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.DataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this.DataGridView.Location = new System.Drawing.Point(6, 39);
			this.DataGridView.Name = "DataGridView";
			this.DataGridView.Size = new System.Drawing.Size(936, 416);
			this.DataGridView.TabIndex = 2;
			// 
			// PK
			// 
			this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PK.HeaderText = "PK";
			this.PK.Name = "PK";
			this.PK.ReadOnly = true;
			this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PK.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.PK.Visible = false;
			this.PK.Width = 75;
			// 
			// ReportTitle
			// 
			this.ReportTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ReportTitle.HeaderText = "Report Title";
			this.ReportTitle.Name = "ReportTitle";
			this.ReportTitle.ReadOnly = true;
			this.ReportTitle.Width = 250;
			// 
			// ExportFileUri
			// 
			this.ExportFileUri.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ExportFileUri.HeaderText = "Export File URI";
			this.ExportFileUri.Name = "ExportFileUri";
			this.ExportFileUri.ReadOnly = true;
			this.ExportFileUri.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.ExportFileUri.Width = 600;
			// 
			// ExportFileDownload
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(946, 458);
			this.Controls.Add(this.DownloadButton);
			this.Controls.Add(this.DataGridView);
			this.Name = "ExportFileDownload";
			this.Text = "ExportFileDownload";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExportFileDownload_FormClosed);
			this.Load += new System.EventHandler(this.ExportFileDownload_Load);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button DownloadButton;
		private System.Windows.Forms.DataGridView DataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReportTitle;
		private System.Windows.Forms.DataGridViewTextBoxColumn ExportFileUri;
	}
}