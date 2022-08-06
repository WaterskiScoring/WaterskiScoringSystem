
namespace WaterskiScoringSystem.Externalnterface {
	partial class PublishReportDelete {
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
			this.DataGridView = new System.Windows.Forms.DataGridView();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReportType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReportTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DeleteButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.SuspendLayout();
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
            this.Event,
            this.ReportType,
            this.ReportTitle});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.DataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			this.DataGridView.Location = new System.Drawing.Point(10, 44);
			this.DataGridView.Name = "DataGridView";
			this.DataGridView.Size = new System.Drawing.Size(757, 373);
			this.DataGridView.TabIndex = 0;
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
			// Event
			// 
			this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Event.HeaderText = "Event";
			this.Event.Name = "Event";
			this.Event.ReadOnly = true;
			this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Event.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Event.Width = 75;
			// 
			// ReportType
			// 
			this.ReportType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ReportType.HeaderText = "Report Type";
			this.ReportType.Name = "ReportType";
			this.ReportType.ReadOnly = true;
			this.ReportType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// ReportTitle
			// 
			this.ReportTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ReportTitle.HeaderText = "Report Title";
			this.ReportTitle.Name = "ReportTitle";
			this.ReportTitle.ReadOnly = true;
			this.ReportTitle.Width = 500;
			// 
			// DeleteButton
			// 
			this.DeleteButton.AutoSize = true;
			this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DeleteButton.Location = new System.Drawing.Point(10, 10);
			this.DeleteButton.Name = "DeleteButton";
			this.DeleteButton.Size = new System.Drawing.Size(192, 28);
			this.DeleteButton.TabIndex = 1;
			this.DeleteButton.Text = "Delete Selected Report";
			this.DeleteButton.UseVisualStyleBackColor = true;
			this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
			// 
			// PublishReportDelete
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(779, 428);
			this.Controls.Add(this.DeleteButton);
			this.Controls.Add(this.DataGridView);
			this.Name = "PublishReportDelete";
			this.Text = "PublishReportDelete";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PublishReportDelete_FormClosed);
			this.Load += new System.EventHandler(this.PublishReportDelete_Load);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView DataGridView;
		private System.Windows.Forms.Button DeleteButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReportType;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReportTitle;
	}
}