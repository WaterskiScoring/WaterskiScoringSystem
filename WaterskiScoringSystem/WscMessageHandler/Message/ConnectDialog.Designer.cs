﻿
namespace WscMessageHandler.Message {
	partial class ConnectDialog {
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
			this.CancelButton = new System.Windows.Forms.Button();
			this.serverUriTextBox = new System.Windows.Forms.TextBox();
			this.serverUriLabel = new System.Windows.Forms.Label();
			this.sanctionlabel = new System.Windows.Forms.Label();
			this.sanctionNumTextbox = new System.Windows.Forms.TextBox();
			this.SelectDatabaseButton = new System.Windows.Forms.Button();
			this.TourListComboBox = new System.Windows.Forms.ComboBox();
			this.DatabaseFilenameLabel = new System.Windows.Forms.Label();
			this.databaseFilenameTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// ShowPinButton
			// 
			this.ShowPinButton.AutoSize = true;
			this.ShowPinButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ShowPinButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ShowPinButton.Location = new System.Drawing.Point(327, 43);
			this.ShowPinButton.Name = "ShowPinButton";
			this.ShowPinButton.Size = new System.Drawing.Size(87, 25);
			this.ShowPinButton.TabIndex = 3;
			this.ShowPinButton.Text = "Show Pin";
			this.ShowPinButton.UseVisualStyleBackColor = true;
			this.ShowPinButton.Click += new System.EventHandler(this.showWscPin_Click);
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.AutoSize = true;
			this.DisconnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisconnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisconnectButton.Location = new System.Drawing.Point(239, 43);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(88, 25);
			this.DisconnectButton.TabIndex = 2;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.execWscClose_Click);
			// 
			// ConnectButton
			// 
			this.ConnectButton.AutoSize = true;
			this.ConnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectButton.Location = new System.Drawing.Point(164, 43);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(75, 25);
			this.ConnectButton.TabIndex = 1;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.execWscConnect_Click);
			// 
			// MessageLabel
			// 
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MessageLabel.Location = new System.Drawing.Point(10, 9);
			this.MessageLabel.Name = "MessageLabel";
			this.MessageLabel.Size = new System.Drawing.Size(121, 14);
			this.MessageLabel.TabIndex = 0;
			this.MessageLabel.Text = "Connect Message";
			// 
			// eventSubIdTextBox
			// 
			this.eventSubIdTextBox.Location = new System.Drawing.Point(164, 164);
			this.eventSubIdTextBox.Name = "eventSubIdTextBox";
			this.eventSubIdTextBox.Size = new System.Drawing.Size(235, 20);
			this.eventSubIdTextBox.TabIndex = 11;
			// 
			// eventSubIdLabel
			// 
			this.eventSubIdLabel.AutoSize = true;
			this.eventSubIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.eventSubIdLabel.Location = new System.Drawing.Point(55, 165);
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
			this.UseJumpTimesCheckBox.Location = new System.Drawing.Point(164, 193);
			this.UseJumpTimesCheckBox.Name = "UseJumpTimesCheckBox";
			this.UseJumpTimesCheckBox.Size = new System.Drawing.Size(143, 20);
			this.UseJumpTimesCheckBox.TabIndex = 12;
			this.UseJumpTimesCheckBox.Text = "Use Jump Times";
			this.UseJumpTimesCheckBox.UseVisualStyleBackColor = true;
			this.UseJumpTimesCheckBox.Visible = false;
			this.UseJumpTimesCheckBox.CheckedChanged += new System.EventHandler(this.UseJumpTimesCheckBox_CheckedChanged);
			// 
			// ViewAppsButton
			// 
			this.ViewAppsButton.AutoSize = true;
			this.ViewAppsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ViewAppsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewAppsButton.Location = new System.Drawing.Point(414, 43);
			this.ViewAppsButton.Name = "ViewAppsButton";
			this.ViewAppsButton.Size = new System.Drawing.Size(87, 25);
			this.ViewAppsButton.TabIndex = 4;
			this.ViewAppsButton.Text = "View Apps";
			this.ViewAppsButton.UseVisualStyleBackColor = true;
			this.ViewAppsButton.Click += new System.EventHandler(this.showAppsConnected_Click);
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CancelButton.Location = new System.Drawing.Point(623, 43);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 25);
			this.CancelButton.TabIndex = 6;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// serverUriTextBox
			// 
			this.serverUriTextBox.Location = new System.Drawing.Point(164, 77);
			this.serverUriTextBox.Name = "serverUriTextBox";
			this.serverUriTextBox.Size = new System.Drawing.Size(534, 20);
			this.serverUriTextBox.TabIndex = 7;
			// 
			// serverUriLabel
			// 
			this.serverUriLabel.AutoSize = true;
			this.serverUriLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.serverUriLabel.Location = new System.Drawing.Point(67, 78);
			this.serverUriLabel.Name = "serverUriLabel";
			this.serverUriLabel.Size = new System.Drawing.Size(95, 18);
			this.serverUriLabel.TabIndex = 0;
			this.serverUriLabel.Text = "Server URI:";
			this.serverUriLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sanctionlabel
			// 
			this.sanctionlabel.AutoSize = true;
			this.sanctionlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.sanctionlabel.Location = new System.Drawing.Point(19, 136);
			this.sanctionlabel.Name = "sanctionlabel";
			this.sanctionlabel.Size = new System.Drawing.Size(143, 18);
			this.sanctionlabel.TabIndex = 0;
			this.sanctionlabel.Text = "Sanction Number:";
			this.sanctionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sanctionNumTextbox
			// 
			this.sanctionNumTextbox.Enabled = false;
			this.sanctionNumTextbox.Location = new System.Drawing.Point(164, 135);
			this.sanctionNumTextbox.Name = "sanctionNumTextbox";
			this.sanctionNumTextbox.Size = new System.Drawing.Size(84, 20);
			this.sanctionNumTextbox.TabIndex = 9;
			// 
			// SelectDatabaseButton
			// 
			this.SelectDatabaseButton.AutoSize = true;
			this.SelectDatabaseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SelectDatabaseButton.Location = new System.Drawing.Point(501, 43);
			this.SelectDatabaseButton.Name = "SelectDatabaseButton";
			this.SelectDatabaseButton.Size = new System.Drawing.Size(122, 25);
			this.SelectDatabaseButton.TabIndex = 5;
			this.SelectDatabaseButton.Text = "Select Database";
			this.SelectDatabaseButton.UseVisualStyleBackColor = true;
			this.SelectDatabaseButton.Click += new System.EventHandler(this.SelectDatabaseButton_Click);
			// 
			// TourListComboBox
			// 
			this.TourListComboBox.FormattingEnabled = true;
			this.TourListComboBox.Location = new System.Drawing.Point(255, 135);
			this.TourListComboBox.Name = "TourListComboBox";
			this.TourListComboBox.Size = new System.Drawing.Size(443, 21);
			this.TourListComboBox.TabIndex = 10;
			this.TourListComboBox.SelectedIndexChanged += new System.EventHandler(this.TourListComboBox_SelectedIndexChanged);
			// 
			// DatabaseFilenameLabel
			// 
			this.DatabaseFilenameLabel.AutoSize = true;
			this.DatabaseFilenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DatabaseFilenameLabel.Location = new System.Drawing.Point(5, 107);
			this.DatabaseFilenameLabel.Name = "DatabaseFilenameLabel";
			this.DatabaseFilenameLabel.Size = new System.Drawing.Size(157, 18);
			this.DatabaseFilenameLabel.TabIndex = 0;
			this.DatabaseFilenameLabel.Text = "Database Filename:";
			this.DatabaseFilenameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// databaseFilenameTextBox
			// 
			this.databaseFilenameTextBox.Location = new System.Drawing.Point(164, 106);
			this.databaseFilenameTextBox.Name = "databaseFilenameTextBox";
			this.databaseFilenameTextBox.Size = new System.Drawing.Size(534, 20);
			this.databaseFilenameTextBox.TabIndex = 8;
			// 
			// ConnectDialog
			// 
			this.AcceptButton = this.ConnectButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 224);
			this.Controls.Add(this.DatabaseFilenameLabel);
			this.Controls.Add(this.databaseFilenameTextBox);
			this.Controls.Add(this.TourListComboBox);
			this.Controls.Add(this.SelectDatabaseButton);
			this.Controls.Add(this.sanctionlabel);
			this.Controls.Add(this.sanctionNumTextbox);
			this.Controls.Add(this.serverUriLabel);
			this.Controls.Add(this.serverUriTextBox);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.ViewAppsButton);
			this.Controls.Add(this.UseJumpTimesCheckBox);
			this.Controls.Add(this.eventSubIdLabel);
			this.Controls.Add(this.eventSubIdTextBox);
			this.Controls.Add(this.ShowPinButton);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.MessageLabel);
			this.Name = "ConnectDialog";
			this.Text = "Connect to WaterSkiConnect";
			this.Load += new System.EventHandler(this.ConnectDialog_Load);
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
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.TextBox serverUriTextBox;
		private System.Windows.Forms.Label serverUriLabel;
		private System.Windows.Forms.Label sanctionlabel;
		private System.Windows.Forms.TextBox sanctionNumTextbox;
		private System.Windows.Forms.Button SelectDatabaseButton;
		private System.Windows.Forms.ComboBox TourListComboBox;
		private System.Windows.Forms.Label DatabaseFilenameLabel;
		private System.Windows.Forms.TextBox databaseFilenameTextBox;
	}
}