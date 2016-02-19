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
            this.TwitterPinTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TwitterAccessTextBox = new System.Windows.Forms.TextBox();
            this.TwitterActivateButton = new System.Windows.Forms.Button();
            this.twitterByPassRB = new System.Windows.Forms.RadioButton();
            this.twitterBySkierRB = new System.Windows.Forms.RadioButton();
            this.SetDefaultAccountButton = new System.Windows.Forms.Button();
            this.TwitterHelpButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WebLocationTextBox
            // 
            this.WebLocationTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WebLocationTextBox.Location = new System.Drawing.Point(5, 32);
            this.WebLocationTextBox.Name = "WebLocationTextBox";
            this.WebLocationTextBox.Size = new System.Drawing.Size(468, 22);
            this.WebLocationTextBox.TabIndex = 10;
            // 
            // LocationLabel
            // 
            this.LocationLabel.AutoSize = true;
            this.LocationLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationLabel.Location = new System.Drawing.Point(3, 6);
            this.LocationLabel.Name = "LocationLabel";
            this.LocationLabel.Size = new System.Drawing.Size(356, 14);
            this.LocationLabel.TabIndex = 0;
            this.LocationLabel.Text = "Enter Web Address (URL) where data should be sent";
            // 
            // SetLocationButton
            // 
            this.SetLocationButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SetLocationButton.Location = new System.Drawing.Point(10, 60);
            this.SetLocationButton.Name = "SetLocationButton";
            this.SetLocationButton.Size = new System.Drawing.Size(75, 23);
            this.SetLocationButton.TabIndex = 20;
            this.SetLocationButton.Text = "Set Location";
            this.SetLocationButton.UseVisualStyleBackColor = true;
            this.SetLocationButton.Click += new System.EventHandler(this.SetLocationButton_Click);
            // 
            // DisableButton
            // 
            this.DisableButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DisableButton.Location = new System.Drawing.Point(93, 60);
            this.DisableButton.Name = "DisableButton";
            this.DisableButton.Size = new System.Drawing.Size(75, 23);
            this.DisableButton.TabIndex = 22;
            this.DisableButton.Text = "Disable";
            this.DisableButton.UseVisualStyleBackColor = true;
            this.DisableButton.Click += new System.EventHandler(this.DisableButton_Click);
            // 
            // ResendButton
            // 
            this.ResendButton.AutoSize = true;
            this.ResendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ResendButton.Location = new System.Drawing.Point(176, 60);
            this.ResendButton.Name = "ResendButton";
            this.ResendButton.Size = new System.Drawing.Size(87, 23);
            this.ResendButton.TabIndex = 23;
            this.ResendButton.Text = "Resend Skier";
            this.ResendButton.UseVisualStyleBackColor = true;
            this.ResendButton.Click += new System.EventHandler(this.ResendButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.AutoSize = true;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(398, 60);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 25;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(127, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "http://www.waterskiresults.com/WfwWeb/WfwImport.php";
            // 
            // SetDefaultButton
            // 
            this.SetDefaultButton.AutoSize = true;
            this.SetDefaultButton.Location = new System.Drawing.Point(10, 90);
            this.SetDefaultButton.Name = "SetDefaultButton";
            this.SetDefaultButton.Size = new System.Drawing.Size(114, 23);
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
            this.label2.Location = new System.Drawing.Point(10, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "To view results > ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label3.Location = new System.Drawing.Point(10, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(368, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "http://www.waterskiresults.com/WfwWeb/WfwShowScores.php";
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(271, 60);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 23);
            this.button1.TabIndex = 24;
            this.button1.Text = "Resend Visible Skiers";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ResendAllButton_Click);
            // 
            // TwitterPinTextBox
            // 
            this.TwitterPinTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TwitterPinTextBox.Location = new System.Drawing.Point(139, 214);
            this.TwitterPinTextBox.Name = "TwitterPinTextBox";
            this.TwitterPinTextBox.Size = new System.Drawing.Size(64, 22);
            this.TwitterPinTextBox.TabIndex = 42;
            this.TwitterPinTextBox.Validated += new System.EventHandler(this.TwitterPinTextBox_Validated);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(10, 218);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 14);
            this.label4.TabIndex = 0;
            this.label4.Text = "Twitter Pin";
            // 
            // TwitterAccessTextBox
            // 
            this.TwitterAccessTextBox.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TwitterAccessTextBox.Location = new System.Drawing.Point(139, 165);
            this.TwitterAccessTextBox.Name = "TwitterAccessTextBox";
            this.TwitterAccessTextBox.Size = new System.Drawing.Size(303, 22);
            this.TwitterAccessTextBox.TabIndex = 41;
            // 
            // TwitterActivateButton
            // 
            this.TwitterActivateButton.AutoSize = true;
            this.TwitterActivateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.TwitterActivateButton.Location = new System.Drawing.Point(10, 165);
            this.TwitterActivateButton.Name = "TwitterActivateButton";
            this.TwitterActivateButton.Size = new System.Drawing.Size(119, 23);
            this.TwitterActivateButton.TabIndex = 30;
            this.TwitterActivateButton.Text = "Activate Twitter";
            this.TwitterActivateButton.UseVisualStyleBackColor = true;
            this.TwitterActivateButton.Click += new System.EventHandler(this.TwitterActivateButton_Click);
            // 
            // twitterByPassRB
            // 
            this.twitterByPassRB.AutoSize = true;
            this.twitterByPassRB.Location = new System.Drawing.Point(140, 193);
            this.twitterByPassRB.Name = "twitterByPassRB";
            this.twitterByPassRB.Size = new System.Drawing.Size(97, 17);
            this.twitterByPassRB.TabIndex = 35;
            this.twitterByPassRB.TabStop = true;
            this.twitterByPassRB.Text = "Report by Pass";
            this.twitterByPassRB.UseVisualStyleBackColor = true;
            this.twitterByPassRB.CheckedChanged += new System.EventHandler(this.twitterByPassRB_CheckedChanged);
            // 
            // twitterBySkierRB
            // 
            this.twitterBySkierRB.AutoSize = true;
            this.twitterBySkierRB.Location = new System.Drawing.Point(251, 193);
            this.twitterBySkierRB.Name = "twitterBySkierRB";
            this.twitterBySkierRB.Size = new System.Drawing.Size(98, 17);
            this.twitterBySkierRB.TabIndex = 36;
            this.twitterBySkierRB.TabStop = true;
            this.twitterBySkierRB.Text = "Report by Skier";
            this.twitterBySkierRB.UseVisualStyleBackColor = true;
            this.twitterBySkierRB.CheckedChanged += new System.EventHandler(this.twitterBySkierRB_CheckedChanged);
            // 
            // SetDefaultAccountButton
            // 
            this.SetDefaultAccountButton.AutoSize = true;
            this.SetDefaultAccountButton.Location = new System.Drawing.Point(11, 190);
            this.SetDefaultAccountButton.Name = "SetDefaultAccountButton";
            this.SetDefaultAccountButton.Size = new System.Drawing.Size(114, 23);
            this.SetDefaultAccountButton.TabIndex = 31;
            this.SetDefaultAccountButton.Text = "Set Default Account";
            this.SetDefaultAccountButton.UseVisualStyleBackColor = true;
            this.SetDefaultAccountButton.Click += new System.EventHandler(this.SetDefaultAccountButton_Click);
            // 
            // TwitterHelpButton
            // 
            this.TwitterHelpButton.AutoSize = true;
            this.TwitterHelpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TwitterHelpButton.Image = global::WaterskiScoringSystem.Properties.Resources.help;
            this.TwitterHelpButton.Location = new System.Drawing.Point(446, 165);
            this.TwitterHelpButton.Margin = new System.Windows.Forms.Padding(0);
            this.TwitterHelpButton.Name = "TwitterHelpButton";
            this.TwitterHelpButton.Size = new System.Drawing.Size(39, 47);
            this.TwitterHelpButton.TabIndex = 51;
            this.TwitterHelpButton.Text = "Help";
            this.TwitterHelpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.TwitterHelpButton.UseVisualStyleBackColor = true;
            this.TwitterHelpButton.Click += new System.EventHandler(this.TwitterHelpButton_Click);
            // 
            // ExportLiveWebDialog
            // 
            this.AcceptButton = this.SetLocationButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 327);
            this.Controls.Add(this.TwitterHelpButton);
            this.Controls.Add(this.SetDefaultAccountButton);
            this.Controls.Add(this.twitterBySkierRB);
            this.Controls.Add(this.twitterByPassRB);
            this.Controls.Add(this.TwitterActivateButton);
            this.Controls.Add(this.TwitterAccessTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.TwitterPinTextBox);
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
            this.Name = "ExportLiveWebDialog";
            this.Text = "ExportLiveWebDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExportLiveWebDialog_FormClosing);
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
        private System.Windows.Forms.TextBox TwitterPinTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TwitterAccessTextBox;
        private System.Windows.Forms.Button TwitterActivateButton;
        private System.Windows.Forms.RadioButton twitterByPassRB;
        private System.Windows.Forms.RadioButton twitterBySkierRB;
        private System.Windows.Forms.Button SetDefaultAccountButton;
        private System.Windows.Forms.Button TwitterHelpButton;
    }
}