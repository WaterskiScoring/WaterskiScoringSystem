
namespace WaterskiScoringSystem.Externalnterface {
	partial class PublishExportDialog {
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
			this.ReportTypeTextBox = new System.Windows.Forms.TextBox();
			this.ReportTypeLabel = new System.Windows.Forms.Label();
			this.EventTextBox = new System.Windows.Forms.TextBox();
			this.ReportFilenameTextBox = new System.Windows.Forms.TextBox();
			this.CancelButton = new System.Windows.Forms.Button();
			this.OkButton = new System.Windows.Forms.Button();
			this.ReportFileNameLabel = new System.Windows.Forms.Label();
			this.EventLabel = new System.Windows.Forms.Label();
			this.ReportTitlelabel = new System.Windows.Forms.Label();
			this.ReportTitleTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ReportTypeTextBox
			// 
			this.ReportTypeTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTypeTextBox.Location = new System.Drawing.Point(143, 27);
			this.ReportTypeTextBox.MaxLength = 16;
			this.ReportTypeTextBox.Name = "ReportTypeTextBox";
			this.ReportTypeTextBox.ReadOnly = true;
			this.ReportTypeTextBox.Size = new System.Drawing.Size(60, 22);
			this.ReportTypeTextBox.TabIndex = 17;
			// 
			// ReportTypeLabel
			// 
			this.ReportTypeLabel.AutoSize = true;
			this.ReportTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTypeLabel.Location = new System.Drawing.Point(38, 30);
			this.ReportTypeLabel.Name = "ReportTypeLabel";
			this.ReportTypeLabel.Size = new System.Drawing.Size(99, 16);
			this.ReportTypeLabel.TabIndex = 16;
			this.ReportTypeLabel.Text = "Report Type:";
			this.ReportTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// EventTextBox
			// 
			this.EventTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EventTextBox.Location = new System.Drawing.Point(143, 52);
			this.EventTextBox.MaxLength = 16;
			this.EventTextBox.Name = "EventTextBox";
			this.EventTextBox.ReadOnly = true;
			this.EventTextBox.Size = new System.Drawing.Size(60, 22);
			this.EventTextBox.TabIndex = 12;
			// 
			// ReportFilenameTextBox
			// 
			this.ReportFilenameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportFilenameTextBox.Location = new System.Drawing.Point(143, 77);
			this.ReportFilenameTextBox.MaxLength = 1024;
			this.ReportFilenameTextBox.Name = "ReportFilenameTextBox";
			this.ReportFilenameTextBox.ReadOnly = true;
			this.ReportFilenameTextBox.Size = new System.Drawing.Size(500, 22);
			this.ReportFilenameTextBox.TabIndex = 13;
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CancelButton.Location = new System.Drawing.Point(258, 107);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 15;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// OkButton
			// 
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OkButton.Location = new System.Drawing.Point(143, 105);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(75, 23);
			this.OkButton.TabIndex = 14;
			this.OkButton.Text = "OK";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// ReportFileNameLabel
			// 
			this.ReportFileNameLabel.AutoSize = true;
			this.ReportFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportFileNameLabel.Location = new System.Drawing.Point(3, 80);
			this.ReportFileNameLabel.Name = "ReportFileNameLabel";
			this.ReportFileNameLabel.Size = new System.Drawing.Size(134, 16);
			this.ReportFileNameLabel.TabIndex = 8;
			this.ReportFileNameLabel.Text = "Report File Name:";
			this.ReportFileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// EventLabel
			// 
			this.EventLabel.AutoSize = true;
			this.EventLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EventLabel.Location = new System.Drawing.Point(86, 55);
			this.EventLabel.Name = "EventLabel";
			this.EventLabel.Size = new System.Drawing.Size(51, 16);
			this.EventLabel.TabIndex = 9;
			this.EventLabel.Text = "Event:";
			this.EventLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ReportTitlelabel
			// 
			this.ReportTitlelabel.AutoSize = true;
			this.ReportTitlelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTitlelabel.Location = new System.Drawing.Point(43, 5);
			this.ReportTitlelabel.Name = "ReportTitlelabel";
			this.ReportTitlelabel.Size = new System.Drawing.Size(94, 16);
			this.ReportTitlelabel.TabIndex = 10;
			this.ReportTitlelabel.Text = "Report Title:";
			this.ReportTitlelabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ReportTitleTextBox
			// 
			this.ReportTitleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTitleTextBox.Location = new System.Drawing.Point(143, 2);
			this.ReportTitleTextBox.MaxLength = 256;
			this.ReportTitleTextBox.Name = "ReportTitleTextBox";
			this.ReportTitleTextBox.Size = new System.Drawing.Size(500, 22);
			this.ReportTitleTextBox.TabIndex = 11;
			// 
			// PublishExportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(666, 135);
			this.Controls.Add(this.ReportTypeTextBox);
			this.Controls.Add(this.ReportTypeLabel);
			this.Controls.Add(this.EventTextBox);
			this.Controls.Add(this.ReportFilenameTextBox);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.OkButton);
			this.Controls.Add(this.ReportFileNameLabel);
			this.Controls.Add(this.EventLabel);
			this.Controls.Add(this.ReportTitlelabel);
			this.Controls.Add(this.ReportTitleTextBox);
			this.Name = "PublishExportDialog";
			this.Text = "PublishExportDialog";
			this.Load += new System.EventHandler(this.PublishExportDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox ReportTypeTextBox;
		private System.Windows.Forms.Label ReportTypeLabel;
		private System.Windows.Forms.TextBox EventTextBox;
		private System.Windows.Forms.TextBox ReportFilenameTextBox;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Label ReportFileNameLabel;
		private System.Windows.Forms.Label EventLabel;
		private System.Windows.Forms.Label ReportTitlelabel;
		private System.Windows.Forms.TextBox ReportTitleTextBox;
	}
}