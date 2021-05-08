
namespace WaterskiScoringSystem.Tools {
	partial class WaterSkiConnectDialog {
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
			this.ShowPinButton = new System.Windows.Forms.Button();
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.MessageLabel = new System.Windows.Forms.Label();
			this.eventSubIdTextBox = new System.Windows.Forms.TextBox();
			this.eventSubIdLabel = new System.Windows.Forms.Label();
			this.UseJumpTimesCheckBox = new System.Windows.Forms.CheckBox();
			this.ViewAppsButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// ShowPinButton
			// 
			this.ShowPinButton.AutoSize = true;
			this.ShowPinButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ShowPinButton.Location = new System.Drawing.Point(176, 43);
			this.ShowPinButton.Name = "ShowPinButton";
			this.ShowPinButton.Size = new System.Drawing.Size(87, 23);
			this.ShowPinButton.TabIndex = 3;
			this.ShowPinButton.Text = "Show Pin";
			this.ShowPinButton.UseVisualStyleBackColor = true;
			this.ShowPinButton.Click += new System.EventHandler(this.showEwcsPin_Click);
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.DisconnectButton.Location = new System.Drawing.Point(93, 43);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(75, 23);
			this.DisconnectButton.TabIndex = 2;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.execEwcsClose_Click);
			// 
			// ConnectButton
			// 
			this.ConnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ConnectButton.Location = new System.Drawing.Point(10, 43);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(75, 23);
			this.ConnectButton.TabIndex = 1;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.execEwcsConnect_Click);
			// 
			// MessageLabel
			// 
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MessageLabel.Location = new System.Drawing.Point(10, 9);
			this.MessageLabel.Name = "MessageLabel";
			this.MessageLabel.Size = new System.Drawing.Size(183, 14);
			this.MessageLabel.TabIndex = 0;
			this.MessageLabel.Text = "WaterSkiConnect Message";
			// 
			// eventSubIdTextBox
			// 
			this.eventSubIdTextBox.Location = new System.Drawing.Point(123, 81);
			this.eventSubIdTextBox.Name = "eventSubIdTextBox";
			this.eventSubIdTextBox.Size = new System.Drawing.Size(235, 20);
			this.eventSubIdTextBox.TabIndex = 5;
			// 
			// eventSubIdLabel
			// 
			this.eventSubIdLabel.AutoSize = true;
			this.eventSubIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.eventSubIdLabel.Location = new System.Drawing.Point(10, 82);
			this.eventSubIdLabel.Name = "eventSubIdLabel";
			this.eventSubIdLabel.Size = new System.Drawing.Size(107, 18);
			this.eventSubIdLabel.TabIndex = 0;
			this.eventSubIdLabel.Text = "Event Sub Id:";
			this.eventSubIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// UseJumpTimesCheckBox
			// 
			this.UseJumpTimesCheckBox.AutoSize = true;
			this.UseJumpTimesCheckBox.Enabled = false;
			this.UseJumpTimesCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.UseJumpTimesCheckBox.Location = new System.Drawing.Point(127, 108);
			this.UseJumpTimesCheckBox.Name = "UseJumpTimesCheckBox";
			this.UseJumpTimesCheckBox.Size = new System.Drawing.Size(143, 20);
			this.UseJumpTimesCheckBox.TabIndex = 6;
			this.UseJumpTimesCheckBox.Text = "Use Jump Times";
			this.UseJumpTimesCheckBox.UseVisualStyleBackColor = true;
			this.UseJumpTimesCheckBox.Visible = false;
			this.UseJumpTimesCheckBox.CheckedChanged += new System.EventHandler(this.UseJumpTimesCheckBox_CheckedChanged);
			// 
			// ViewAppsButton
			// 
			this.ViewAppsButton.AutoSize = true;
			this.ViewAppsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ViewAppsButton.Location = new System.Drawing.Point(271, 43);
			this.ViewAppsButton.Name = "ViewAppsButton";
			this.ViewAppsButton.Size = new System.Drawing.Size(87, 23);
			this.ViewAppsButton.TabIndex = 4;
			this.ViewAppsButton.Text = "View Apps";
			this.ViewAppsButton.UseVisualStyleBackColor = true;
			this.ViewAppsButton.Click += new System.EventHandler(this.showAppsConnected_Click);
			// 
			// WaterSkiConnectDialog
			// 
			this.AcceptButton = this.ConnectButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(366, 129);
			this.Controls.Add(this.ViewAppsButton);
			this.Controls.Add(this.UseJumpTimesCheckBox);
			this.Controls.Add(this.eventSubIdLabel);
			this.Controls.Add(this.eventSubIdTextBox);
			this.Controls.Add(this.ShowPinButton);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.MessageLabel);
			this.Name = "WaterSkiConnectDialog";
			this.Text = "WaterSkiConnect";
			this.Load += new System.EventHandler(this.WaterSkiConnectDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button ShowPinButton;
		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Button ConnectButton;
		private System.Windows.Forms.Label MessageLabel;
		private System.Windows.Forms.TextBox eventSubIdTextBox;
		private System.Windows.Forms.Label eventSubIdLabel;
		private System.Windows.Forms.CheckBox UseJumpTimesCheckBox;
		private System.Windows.Forms.Button ViewAppsButton;
	}
}