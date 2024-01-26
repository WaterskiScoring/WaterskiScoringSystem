
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
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.MessageLabel = new System.Windows.Forms.Label();
			this.eventSubIdLabel = new System.Windows.Forms.Label();
			this.CancelButton = new System.Windows.Forms.Button();
			this.serverUriTextBox = new System.Windows.Forms.TextBox();
			this.serverUriLabel = new System.Windows.Forms.Label();
			this.sanctionlabel = new System.Windows.Forms.Label();
			this.sanctionNumTextbox = new System.Windows.Forms.TextBox();
			this.SelectDatabaseButton = new System.Windows.Forms.Button();
			this.TourListComboBox = new System.Windows.Forms.ComboBox();
			this.DatabaseFilenameLabel = new System.Windows.Forms.Label();
			this.databaseFilenameTextBox = new System.Windows.Forms.TextBox();
			this.EventSubIdDropdown = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.AutoSize = true;
			this.DisconnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DisconnectButton.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisconnectButton.Location = new System.Drawing.Point(289, 43);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(100, 28);
			this.DisconnectButton.TabIndex = 2;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.execWscClose_Click);
			// 
			// ConnectButton
			// 
			this.ConnectButton.AutoSize = true;
			this.ConnectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ConnectButton.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectButton.Location = new System.Drawing.Point(164, 43);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(78, 28);
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
			// eventSubIdLabel
			// 
			this.eventSubIdLabel.AutoSize = true;
			this.eventSubIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.eventSubIdLabel.Location = new System.Drawing.Point(55, 184);
			this.eventSubIdLabel.Name = "eventSubIdLabel";
			this.eventSubIdLabel.Size = new System.Drawing.Size(107, 18);
			this.eventSubIdLabel.TabIndex = 0;
			this.eventSubIdLabel.Text = "Event Sub Id:";
			this.eventSubIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// CancelButton
			// 
			this.CancelButton.AutoSize = true;
			this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelButton.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.CancelButton.Location = new System.Drawing.Point(622, 43);
			this.CancelButton.Name = "CancelButton";
			this.CancelButton.Size = new System.Drawing.Size(75, 28);
			this.CancelButton.TabIndex = 6;
			this.CancelButton.Text = "Cancel";
			this.CancelButton.UseVisualStyleBackColor = true;
			// 
			// serverUriTextBox
			// 
			this.serverUriTextBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.serverUriTextBox.Location = new System.Drawing.Point(164, 77);
			this.serverUriTextBox.Name = "serverUriTextBox";
			this.serverUriTextBox.Size = new System.Drawing.Size(534, 26);
			this.serverUriTextBox.TabIndex = 7;
			// 
			// serverUriLabel
			// 
			this.serverUriLabel.AutoSize = true;
			this.serverUriLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.serverUriLabel.Location = new System.Drawing.Point(67, 81);
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
			this.sanctionlabel.Location = new System.Drawing.Point(19, 144);
			this.sanctionlabel.Name = "sanctionlabel";
			this.sanctionlabel.Size = new System.Drawing.Size(143, 18);
			this.sanctionlabel.TabIndex = 0;
			this.sanctionlabel.Text = "Sanction Number:";
			this.sanctionlabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// sanctionNumTextbox
			// 
			this.sanctionNumTextbox.Enabled = false;
			this.sanctionNumTextbox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.sanctionNumTextbox.Location = new System.Drawing.Point(164, 145);
			this.sanctionNumTextbox.Name = "sanctionNumTextbox";
			this.sanctionNumTextbox.Size = new System.Drawing.Size(84, 26);
			this.sanctionNumTextbox.TabIndex = 9;
			// 
			// SelectDatabaseButton
			// 
			this.SelectDatabaseButton.AutoSize = true;
			this.SelectDatabaseButton.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SelectDatabaseButton.Location = new System.Drawing.Point(436, 43);
			this.SelectDatabaseButton.Name = "SelectDatabaseButton";
			this.SelectDatabaseButton.Size = new System.Drawing.Size(139, 28);
			this.SelectDatabaseButton.TabIndex = 5;
			this.SelectDatabaseButton.Text = "Select Database";
			this.SelectDatabaseButton.UseVisualStyleBackColor = true;
			this.SelectDatabaseButton.Click += new System.EventHandler(this.SelectDatabaseButton_Click);
			// 
			// TourListComboBox
			// 
			this.TourListComboBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TourListComboBox.FormattingEnabled = true;
			this.TourListComboBox.Location = new System.Drawing.Point(255, 145);
			this.TourListComboBox.Name = "TourListComboBox";
			this.TourListComboBox.Size = new System.Drawing.Size(443, 26);
			this.TourListComboBox.TabIndex = 10;
			this.TourListComboBox.SelectedIndexChanged += new System.EventHandler(this.TourListComboBox_SelectedIndexChanged);
			// 
			// DatabaseFilenameLabel
			// 
			this.DatabaseFilenameLabel.AutoSize = true;
			this.DatabaseFilenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DatabaseFilenameLabel.Location = new System.Drawing.Point(5, 115);
			this.DatabaseFilenameLabel.Name = "DatabaseFilenameLabel";
			this.DatabaseFilenameLabel.Size = new System.Drawing.Size(157, 18);
			this.DatabaseFilenameLabel.TabIndex = 0;
			this.DatabaseFilenameLabel.Text = "Database Filename:";
			this.DatabaseFilenameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// databaseFilenameTextBox
			// 
			this.databaseFilenameTextBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.databaseFilenameTextBox.Location = new System.Drawing.Point(164, 111);
			this.databaseFilenameTextBox.Name = "databaseFilenameTextBox";
			this.databaseFilenameTextBox.Size = new System.Drawing.Size(534, 26);
			this.databaseFilenameTextBox.TabIndex = 8;
			this.databaseFilenameTextBox.Enter += new System.EventHandler(this.editTextOrigValue);
			this.databaseFilenameTextBox.Validated += new System.EventHandler(this.databaseFilenameTextBox_Validated);
			// 
			// EventSubIdDropdown
			// 
			this.EventSubIdDropdown.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EventSubIdDropdown.FormattingEnabled = true;
			this.EventSubIdDropdown.Items.AddRange(new object[] {
            "No Sub Id",
            "Lake 1",
            "Lake 2",
            "Lake 3",
            "Lake 4",
            "Lake 5"});
			this.EventSubIdDropdown.Location = new System.Drawing.Point(164, 180);
			this.EventSubIdDropdown.Name = "EventSubIdDropdown";
			this.EventSubIdDropdown.Size = new System.Drawing.Size(121, 26);
			this.EventSubIdDropdown.TabIndex = 13;
			// 
			// ConnectDialog
			// 
			this.AcceptButton = this.ConnectButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 224);
			this.Controls.Add(this.EventSubIdDropdown);
			this.Controls.Add(this.DatabaseFilenameLabel);
			this.Controls.Add(this.databaseFilenameTextBox);
			this.Controls.Add(this.TourListComboBox);
			this.Controls.Add(this.SelectDatabaseButton);
			this.Controls.Add(this.sanctionlabel);
			this.Controls.Add(this.sanctionNumTextbox);
			this.Controls.Add(this.serverUriLabel);
			this.Controls.Add(this.serverUriTextBox);
			this.Controls.Add(this.CancelButton);
			this.Controls.Add(this.eventSubIdLabel);
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
		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Button ConnectButton;
		private System.Windows.Forms.Label MessageLabel;
		private System.Windows.Forms.Label eventSubIdLabel;
		private System.Windows.Forms.Button CancelButton;
		private System.Windows.Forms.TextBox serverUriTextBox;
		private System.Windows.Forms.Label serverUriLabel;
		private System.Windows.Forms.Label sanctionlabel;
		private System.Windows.Forms.TextBox sanctionNumTextbox;
		private System.Windows.Forms.Button SelectDatabaseButton;
		private System.Windows.Forms.ComboBox TourListComboBox;
		private System.Windows.Forms.Label DatabaseFilenameLabel;
		private System.Windows.Forms.TextBox databaseFilenameTextBox;
		private System.Windows.Forms.ComboBox EventSubIdDropdown;
	}
}