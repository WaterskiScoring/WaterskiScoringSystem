namespace WaterskiScoringSystem.Tools {
    partial class ExportLiveWebDialog {
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
			this.WebLocationTextBox = new System.Windows.Forms.TextBox();
			this.LocationLabel = new System.Windows.Forms.Label();
			this.SetLocationButton = new System.Windows.Forms.Button();
			this.DisableButton = new System.Windows.Forms.Button();
			this.ResendButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SetDefaultButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// WebLocationTextBox
			// 
			this.WebLocationTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WebLocationTextBox.Location = new System.Drawing.Point(7, 39);
			this.WebLocationTextBox.Margin = new System.Windows.Forms.Padding(4);
			this.WebLocationTextBox.Name = "WebLocationTextBox";
			this.WebLocationTextBox.Size = new System.Drawing.Size(623, 26);
			this.WebLocationTextBox.TabIndex = 10;
			// 
			// LocationLabel
			// 
			this.LocationLabel.AutoSize = true;
			this.LocationLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LocationLabel.Location = new System.Drawing.Point(4, 7);
			this.LocationLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.LocationLabel.Name = "LocationLabel";
			this.LocationLabel.Size = new System.Drawing.Size(442, 18);
			this.LocationLabel.TabIndex = 0;
			this.LocationLabel.Text = "Enter Web Address (URL) where data should be sent";
			// 
			// SetLocationButton
			// 
			this.SetLocationButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SetLocationButton.Location = new System.Drawing.Point(13, 74);
			this.SetLocationButton.Margin = new System.Windows.Forms.Padding(4);
			this.SetLocationButton.Name = "SetLocationButton";
			this.SetLocationButton.Size = new System.Drawing.Size(100, 28);
			this.SetLocationButton.TabIndex = 20;
			this.SetLocationButton.Text = "Set Location";
			this.SetLocationButton.UseVisualStyleBackColor = true;
			this.SetLocationButton.Click += new System.EventHandler(this.SetLocationButton_Click);
			// 
			// DisableButton
			// 
			this.DisableButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisableButton.Location = new System.Drawing.Point(124, 74);
			this.DisableButton.Margin = new System.Windows.Forms.Padding(4);
			this.DisableButton.Name = "DisableButton";
			this.DisableButton.Size = new System.Drawing.Size(100, 28);
			this.DisableButton.TabIndex = 22;
			this.DisableButton.Text = "Disable";
			this.DisableButton.UseVisualStyleBackColor = true;
			this.DisableButton.Click += new System.EventHandler(this.DisableButton_Click);
			// 
			// ResendButton
			// 
			this.ResendButton.AutoSize = true;
			this.ResendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ResendButton.Location = new System.Drawing.Point(235, 74);
			this.ResendButton.Margin = new System.Windows.Forms.Padding(4);
			this.ResendButton.Name = "ResendButton";
			this.ResendButton.Size = new System.Drawing.Size(116, 28);
			this.ResendButton.TabIndex = 23;
			this.ResendButton.Text = "Resend Skier";
			this.ResendButton.UseVisualStyleBackColor = true;
			this.ResendButton.Click += new System.EventHandler(this.ResendButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(531, 74);
			this.CancelButton.Margin = new System.Windows.Forms.Padding(4);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(100, 28);
			this.CancelButton.TabIndex = 25;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(169, 117);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(367, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
			// 
			// SetDefaultButton
			// 
			this.SetDefaultButton.AutoSize = true;
			this.SetDefaultButton.Location = new System.Drawing.Point(13, 111);
			this.SetDefaultButton.Margin = new System.Windows.Forms.Padding(4);
			this.SetDefaultButton.Name = "SetDefaultButton";
			this.SetDefaultButton.Size = new System.Drawing.Size(152, 28);
			this.SetDefaultButton.TabIndex = 21;
			this.SetDefaultButton.Text = "Set Default Location";
			this.SetDefaultButton.UseVisualStyleBackColor = true;
			this.SetDefaultButton.Click += new System.EventHandler(this.SetDefaultButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label2.Location = new System.Drawing.Point(13, 146);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "To view results > ";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label3.Location = new System.Drawing.Point(13, 167);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(461, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "http://www.waterskiresults.com/WfwWeb/WfwShowScores.php";
			// 
			// button1
			// 
			this.button1.AutoSize = true;
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(361, 74);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(159, 28);
			this.button1.TabIndex = 24;
			this.button1.Text = "Resend Visible Skiers";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.ResendAllButton_Click);
			// 
			// ExportLiveWebDialog
			// 
			this.AcceptButton = this.SetLocationButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 204);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SetDefaultButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.ResendButton);
			this.Controls.Add(this.DisableButton);
			this.Controls.Add(this.SetLocationButton);
			this.Controls.Add(this.LocationLabel);
			this.Controls.Add(this.WebLocationTextBox);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "ExportLiveWebDialog";
			this.Text = "ExportLiveWebDialog";
			this.Load += new System.EventHandler(this.ExportLiveWebDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox WebLocationTextBox;
        private System.Windows.Forms.Label LocationLabel;
        private System.Windows.Forms.Button SetLocationButton;
        private System.Windows.Forms.Button DisableButton;
        private System.Windows.Forms.Button ResendButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SetDefaultButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}