namespace WaterskiScoringSystem.Admin {
	partial class SanctionSetupDialog {
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
			this.SanctionIdLabel = new System.Windows.Forms.Label();
			this.EditCodeLabel = new System.Windows.Forms.Label();
			this.SanctionIDTextBox = new System.Windows.Forms.TextBox();
			this.EditCodeTextBox = new System.Windows.Forms.TextBox();
			this.DialogTitle = new System.Windows.Forms.Label();
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// SanctionIdLabel
			// 
			this.SanctionIdLabel.AutoSize = true;
			this.SanctionIdLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SanctionIdLabel.Location = new System.Drawing.Point(17, 87);
			this.SanctionIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.SanctionIdLabel.Name = "SanctionIdLabel";
			this.SanctionIdLabel.Size = new System.Drawing.Size(111, 24);
			this.SanctionIdLabel.TabIndex = 0;
			this.SanctionIdLabel.Text = "Sanction ID:";
			// 
			// EditCodeLabel
			// 
			this.EditCodeLabel.AutoSize = true;
			this.EditCodeLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EditCodeLabel.Location = new System.Drawing.Point(348, 87);
			this.EditCodeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.EditCodeLabel.Name = "EditCodeLabel";
			this.EditCodeLabel.Size = new System.Drawing.Size(97, 24);
			this.EditCodeLabel.TabIndex = 0;
			this.EditCodeLabel.Text = "Edit Code:";
			// 
			// SanctionIDTextBox
			// 
			this.SanctionIDTextBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SanctionIDTextBox.Location = new System.Drawing.Point(145, 82);
			this.SanctionIDTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.SanctionIDTextBox.MaxLength = 6;
			this.SanctionIDTextBox.Name = "SanctionIDTextBox";
			this.SanctionIDTextBox.Size = new System.Drawing.Size(132, 32);
			this.SanctionIDTextBox.TabIndex = 10;
			// 
			// EditCodeTextBox
			// 
			this.EditCodeTextBox.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EditCodeTextBox.Location = new System.Drawing.Point(460, 82);
			this.EditCodeTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.EditCodeTextBox.MaxLength = 10;
			this.EditCodeTextBox.Name = "EditCodeTextBox";
			this.EditCodeTextBox.Size = new System.Drawing.Size(132, 32);
			this.EditCodeTextBox.TabIndex = 11;
			// 
			// DialogTitle
			// 
			this.DialogTitle.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DialogTitle.Location = new System.Drawing.Point(5, 9);
			this.DialogTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.DialogTitle.Name = "DialogTitle";
			this.DialogTitle.Size = new System.Drawing.Size(687, 62);
			this.DialogTitle.TabIndex = 0;
			this.DialogTitle.Text = "Enter required data to retrieve sanction information from USAWSWS to help with to" +
    "urnament setup";
			this.DialogTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OKButton.Location = new System.Drawing.Point(145, 135);
			this.OKButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(133, 28);
			this.OKButton.TabIndex = 21;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CancelButton.Location = new System.Drawing.Point(460, 135);
			this.CancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(133, 28);
			this.CancelButton.TabIndex = 22;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 178);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(687, 32);
			this.label1.TabIndex = 23;
			this.label1.Text = "Click Cancel to setup tournament manually";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// SanctionSetupDialog
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(696, 217);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.DialogTitle);
			this.Controls.Add(this.EditCodeTextBox);
			this.Controls.Add(this.SanctionIDTextBox);
			this.Controls.Add(this.EditCodeLabel);
			this.Controls.Add(this.SanctionIdLabel);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "SanctionSetupDialog";
			this.Text = "SanctionSetupDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label SanctionIdLabel;
		private System.Windows.Forms.Label EditCodeLabel;
		private System.Windows.Forms.TextBox SanctionIDTextBox;
		private System.Windows.Forms.TextBox EditCodeTextBox;
		private System.Windows.Forms.Label DialogTitle;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Label label1;
	}
}