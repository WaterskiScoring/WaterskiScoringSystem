namespace WaterskiScoringSystem.Externalnterface {
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
			this.ConnectButton = new System.Windows.Forms.Button();
			this.ResendButton = new System.Windows.Forms.Button();
			this.CancelButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.ResendAllButton = new System.Windows.Forms.Button();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.DisableAllSendButton = new System.Windows.Forms.Button();
			this.DisableSendButton = new System.Windows.Forms.Button();
			this.DisableButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ConnectButton
			// 
			this.ConnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ConnectButton.Location = new System.Drawing.Point(10, 6);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(75, 23);
			this.ConnectButton.TabIndex = 1;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
			// 
			// ResendButton
			// 
			this.ResendButton.AutoSize = true;
			this.ResendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ResendButton.Location = new System.Drawing.Point(183, 6);
			this.ResendButton.Name = "ResendButton";
			this.ResendButton.Size = new System.Drawing.Size(87, 23);
			this.ResendButton.TabIndex = 3;
			this.ResendButton.Text = "Resend Skier";
			this.ResendButton.UseVisualStyleBackColor = true;
			this.ResendButton.Click += new System.EventHandler(this.ResendButton_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Location = new System.Drawing.Point(405, 6);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 23);
			this.CancelButton.TabIndex = 5;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.label2.Location = new System.Drawing.Point(10, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(107, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "To view results > ";
			// 
			// ResendAllButton
			// 
			this.ResendAllButton.AutoSize = true;
			this.ResendAllButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ResendAllButton.Location = new System.Drawing.Point(278, 6);
			this.ResendAllButton.Name = "ResendAllButton";
			this.ResendAllButton.Size = new System.Drawing.Size(119, 23);
			this.ResendAllButton.TabIndex = 4;
			this.ResendAllButton.Text = "Resend Visible Skiers";
			this.ResendAllButton.UseVisualStyleBackColor = true;
			this.ResendAllButton.Click += new System.EventHandler(this.ResendAllButton_Click);
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(110, 64);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(317, 13);
			this.linkLabel1.TabIndex = 100;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "http://www.waterskiresults.com/WfwWeb/wfwShowTourList.php";
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// DisableAllSendButton
			// 
			this.DisableAllSendButton.AutoSize = true;
			this.DisableAllSendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisableAllSendButton.Location = new System.Drawing.Point(277, 33);
			this.DisableAllSendButton.Name = "DisableAllSendButton";
			this.DisableAllSendButton.Size = new System.Drawing.Size(119, 23);
			this.DisableAllSendButton.TabIndex = 7;
			this.DisableAllSendButton.Text = "Disable Visible Skiers";
			this.DisableAllSendButton.UseVisualStyleBackColor = true;
			this.DisableAllSendButton.Click += new System.EventHandler(this.DisableAllSendButton_Click);
			// 
			// DisableSendButton
			// 
			this.DisableSendButton.AutoSize = true;
			this.DisableSendButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisableSendButton.Location = new System.Drawing.Point(183, 33);
			this.DisableSendButton.Name = "DisableSendButton";
			this.DisableSendButton.Size = new System.Drawing.Size(87, 23);
			this.DisableSendButton.TabIndex = 6;
			this.DisableSendButton.Text = "Disable Skier";
			this.DisableSendButton.UseVisualStyleBackColor = true;
			this.DisableSendButton.Click += new System.EventHandler(this.DisableSendButton_Click);
			// 
			// DisableButton
			// 
			this.DisableButton.AutoSize = true;
			this.DisableButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisableButton.Location = new System.Drawing.Point(91, 6);
			this.DisableButton.Name = "DisableButton";
			this.DisableButton.Size = new System.Drawing.Size(87, 23);
			this.DisableButton.TabIndex = 2;
			this.DisableButton.Text = "Disable";
			this.DisableButton.UseVisualStyleBackColor = true;
			this.DisableButton.Click += new System.EventHandler(this.DisableButton_Click);
			// 
			// ExportLiveWebDialog
			// 
			this.AcceptButton = this.ConnectButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 89);
			this.Controls.Add(this.DisableButton);
			this.Controls.Add(this.DisableAllSendButton);
			this.Controls.Add(this.DisableSendButton);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.ResendAllButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.ResendButton);
			this.Controls.Add(this.ConnectButton);
			this.Name = "ExportLiveWebDialog";
			this.Text = "ExportLiveWebDialog";
			this.Load += new System.EventHandler(this.ExportLiveWebDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.Button ResendButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ResendAllButton;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button DisableAllSendButton;
		private System.Windows.Forms.Button DisableSendButton;
		private System.Windows.Forms.Button DisableButton;
	}
}