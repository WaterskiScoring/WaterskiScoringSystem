
namespace WaterskiScoringSystem.Tools {
	partial class PublishReportDialog {
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
			this.ReportTitleTextBox = new System.Windows.Forms.TextBox();
			this.ReportTitlelabel = new System.Windows.Forms.Label();
			this.EventLabel = new System.Windows.Forms.Label();
			this.ReportFileNameLabel = new System.Windows.Forms.Label();
			this.OkButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.ReportFilenameTextBox = new System.Windows.Forms.TextBox();
			this.EventTextBox = new System.Windows.Forms.TextBox();
			this.ReportTypeTextBox = new System.Windows.Forms.TextBox();
			this.ReportTypeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// ReportTitleTextBox
			// 
			this.ReportTitleTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTitleTextBox.Location = new System.Drawing.Point(145, 7);
			this.ReportTitleTextBox.MaxLength = 256;
			this.ReportTitleTextBox.Name = "ReportTitleTextBox";
			this.ReportTitleTextBox.Size = new System.Drawing.Size(500, 22);
			this.ReportTitleTextBox.TabIndex = 1;
			// 
			// ReportTitlelabel
			// 
			this.ReportTitlelabel.AutoSize = true;
			this.ReportTitlelabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTitlelabel.Location = new System.Drawing.Point(45, 10);
			this.ReportTitlelabel.Name = "ReportTitlelabel";
			this.ReportTitlelabel.Size = new System.Drawing.Size(94, 16);
			this.ReportTitlelabel.TabIndex = 0;
			this.ReportTitlelabel.Text = "Report Title:";
			this.ReportTitlelabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// EventLabel
			// 
			this.EventLabel.AutoSize = true;
			this.EventLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EventLabel.Location = new System.Drawing.Point(88, 60);
			this.EventLabel.Name = "EventLabel";
			this.EventLabel.Size = new System.Drawing.Size(51, 16);
			this.EventLabel.TabIndex = 0;
			this.EventLabel.Text = "Event:";
			this.EventLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ReportFileNameLabel
			// 
			this.ReportFileNameLabel.AutoSize = true;
			this.ReportFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportFileNameLabel.Location = new System.Drawing.Point(5, 85);
			this.ReportFileNameLabel.Name = "ReportFileNameLabel";
			this.ReportFileNameLabel.Size = new System.Drawing.Size(134, 16);
			this.ReportFileNameLabel.TabIndex = 0;
			this.ReportFileNameLabel.Text = "Report File Name:";
			this.ReportFileNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// OkButton
			// 
			this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OkButton.Location = new System.Drawing.Point(145, 110);
			this.OkButton.Name = "OkButton";
			this.OkButton.Size = new System.Drawing.Size(75, 23);
			this.OkButton.TabIndex = 4;
			this.OkButton.Text = "OK";
			this.OkButton.UseVisualStyleBackColor = true;
			this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CancelButton.Location = new System.Drawing.Point(260, 112);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 5;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// ReportFilenameTextBox
			// 
			this.ReportFilenameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportFilenameTextBox.Location = new System.Drawing.Point(145, 82);
			this.ReportFilenameTextBox.MaxLength = 1024;
			this.ReportFilenameTextBox.Name = "ReportFilenameTextBox";
			this.ReportFilenameTextBox.ReadOnly = true;
			this.ReportFilenameTextBox.Size = new System.Drawing.Size(500, 22);
			this.ReportFilenameTextBox.TabIndex = 3;
			// 
			// EventTextBox
			// 
			this.EventTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EventTextBox.Location = new System.Drawing.Point(145, 57);
			this.EventTextBox.MaxLength = 16;
			this.EventTextBox.Name = "EventTextBox";
			this.EventTextBox.ReadOnly = true;
			this.EventTextBox.Size = new System.Drawing.Size(60, 22);
			this.EventTextBox.TabIndex = 2;
			// 
			// ReportTypeTextBox
			// 
			this.ReportTypeTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTypeTextBox.Location = new System.Drawing.Point(145, 32);
			this.ReportTypeTextBox.MaxLength = 16;
			this.ReportTypeTextBox.Name = "ReportTypeTextBox";
			this.ReportTypeTextBox.ReadOnly = true;
			this.ReportTypeTextBox.Size = new System.Drawing.Size(60, 22);
			this.ReportTypeTextBox.TabIndex = 7;
			// 
			// ReportTypeLabel
			// 
			this.ReportTypeLabel.AutoSize = true;
			this.ReportTypeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ReportTypeLabel.Location = new System.Drawing.Point(40, 35);
			this.ReportTypeLabel.Name = "ReportTypeLabel";
			this.ReportTypeLabel.Size = new System.Drawing.Size(99, 16);
			this.ReportTypeLabel.TabIndex = 6;
			this.ReportTypeLabel.Text = "Report Type:";
			this.ReportTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// PublishReportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(668, 149);
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
			this.Name = "PublishReportDialog";
			this.Text = "Publish Report to Live Web";
			this.Load += new System.EventHandler(this.PublishReportDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox ReportTitleTextBox;
		private System.Windows.Forms.Label ReportTitlelabel;
		private System.Windows.Forms.Label EventLabel;
		private System.Windows.Forms.Label ReportFileNameLabel;
		private System.Windows.Forms.Button OkButton;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.TextBox ReportFilenameTextBox;
		private System.Windows.Forms.TextBox EventTextBox;
		private System.Windows.Forms.TextBox ReportTypeTextBox;
		private System.Windows.Forms.Label ReportTypeLabel;
	}
}