
namespace HttpMessageHandler.Externalnterface {
	partial class LiveWebConnectDialog {
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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label2 = new System.Windows.Forms.Label();
			this.SetDefaultButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.CancelButton = new System.Windows.Forms.Button();
			this.DisableButton = new System.Windows.Forms.Button();
			this.SetLocationButton = new System.Windows.Forms.Button();
			this.LocationLabel = new System.Windows.Forms.Label();
			this.WebLocationTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(15, 137);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(317, 13);
			this.linkLabel1.TabIndex = 109;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://www.waterskiresults.com/WfwWeb/wfwShowTourList.php";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label2.Location = new System.Drawing.Point(12, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(107, 13);
			this.label2.TabIndex = 101;
			this.label2.Text = "To view results > ";
			// 
			// SetDefaultButton
			// 
			this.SetDefaultButton.AutoSize = true;
			this.SetDefaultButton.Location = new System.Drawing.Point(12, 91);
			this.SetDefaultButton.Name = "SetDefaultButton";
			this.SetDefaultButton.Size = new System.Drawing.Size(114, 23);
			this.SetDefaultButton.TabIndex = 106;
			this.SetDefaultButton.Text = "Set Default Location";
			this.SetDefaultButton.UseVisualStyleBackColor = true;
			this.SetDefaultButton.Click += new System.EventHandler(this.SetDefaultButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(129, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(290, 13);
			this.label1.TabIndex = 102;
			this.label1.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(185, 59);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 108;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// DisableButton
			// 
			this.DisableButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisableButton.Location = new System.Drawing.Point(95, 59);
			this.DisableButton.Name = "DisableButton";
			this.DisableButton.Size = new System.Drawing.Size(75, 23);
			this.DisableButton.TabIndex = 107;
			this.DisableButton.Text = "Disable";
			this.DisableButton.UseVisualStyleBackColor = true;
			this.DisableButton.Click += new System.EventHandler(this.DisableButton_Click);
			// 
			// SetLocationButton
			// 
			this.SetLocationButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SetLocationButton.Location = new System.Drawing.Point(12, 59);
			this.SetLocationButton.Name = "SetLocationButton";
			this.SetLocationButton.Size = new System.Drawing.Size(75, 23);
			this.SetLocationButton.TabIndex = 105;
			this.SetLocationButton.Text = "Set Location";
			this.SetLocationButton.UseVisualStyleBackColor = true;
			this.SetLocationButton.Click += new System.EventHandler(this.SetLocationButton_Click);
			// 
			// LocationLabel
			// 
			this.LocationLabel.AutoSize = true;
			this.LocationLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LocationLabel.Location = new System.Drawing.Point(5, 5);
			this.LocationLabel.Name = "LocationLabel";
			this.LocationLabel.Size = new System.Drawing.Size(356, 14);
			this.LocationLabel.TabIndex = 103;
			this.LocationLabel.Text = "Enter Web Address (URL) where data should be sent";
			// 
			// WebLocationTextBox
			// 
			this.WebLocationTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WebLocationTextBox.Location = new System.Drawing.Point(7, 31);
			this.WebLocationTextBox.Name = "WebLocationTextBox";
			this.WebLocationTextBox.Size = new System.Drawing.Size(468, 22);
			this.WebLocationTextBox.TabIndex = 104;
			// 
			// LiveWebConnectDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(478, 170);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SetDefaultButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.DisableButton);
			this.Controls.Add(this.SetLocationButton);
			this.Controls.Add(this.LocationLabel);
			this.Controls.Add(this.WebLocationTextBox);
			this.Name = "LiveWebConnectDialog";
			this.Text = "LiveWeb Connect";
			this.Load += new System.EventHandler(this.LiveWebConnectDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button SetDefaultButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.Button DisableButton;
		private System.Windows.Forms.Button SetLocationButton;
		private System.Windows.Forms.Label LocationLabel;
		private System.Windows.Forms.TextBox WebLocationTextBox;
	}
}