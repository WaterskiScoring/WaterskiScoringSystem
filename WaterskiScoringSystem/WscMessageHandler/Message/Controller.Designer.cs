﻿
namespace WscMessageHandler.Message {
	partial class Controller {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Controller));
			this.MessageView = new System.Windows.Forms.DataGridView();
			this.LastUpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.WaterSkiConnectButton = new System.Windows.Forms.Button();
			this.ShowPinButton = new System.Windows.Forms.Button();
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.ViewAppsButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).BeginInit();
			this.SuspendLayout();
			// 
			// MessageView
			// 
			this.MessageView.AllowUserToDeleteRows = false;
			this.MessageView.AllowUserToOrderColumns = true;
			this.MessageView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MessageView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.MessageView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LastUpdateDate,
            this.Message});
			this.MessageView.Location = new System.Drawing.Point(10, 33);
			this.MessageView.Name = "MessageView";
			this.MessageView.ReadOnly = true;
			this.MessageView.Size = new System.Drawing.Size(958, 508);
			this.MessageView.TabIndex = 4;
			this.MessageView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MessageView_CellContentDoubleClick);
			// 
			// LastUpdateDate
			// 
			this.LastUpdateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle1.NullValue = null;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.LastUpdateDate.DefaultCellStyle = dataGridViewCellStyle1;
			this.LastUpdateDate.HeaderText = "DateTime";
			this.LastUpdateDate.MinimumWidth = 100;
			this.LastUpdateDate.Name = "LastUpdateDate";
			this.LastUpdateDate.ReadOnly = true;
			// 
			// Message
			// 
			this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.Message.HeaderText = "Message";
			this.Message.MinimumWidth = 200;
			this.Message.Name = "Message";
			this.Message.ReadOnly = true;
			this.Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Message.Width = 200;
			// 
			// WaterSkiConnectButton
			// 
			this.WaterSkiConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WaterSkiConnectButton.Location = new System.Drawing.Point(131, 5);
			this.WaterSkiConnectButton.Name = "WaterSkiConnectButton";
			this.WaterSkiConnectButton.Size = new System.Drawing.Size(145, 23);
			this.WaterSkiConnectButton.TabIndex = 1;
			this.WaterSkiConnectButton.Text = "WaterSkiConnect";
			this.WaterSkiConnectButton.UseVisualStyleBackColor = true;
			this.WaterSkiConnectButton.Click += new System.EventHandler(this.WaterSkiConnectButton_Click);
			// 
			// ShowPinButton
			// 
			this.ShowPinButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ShowPinButton.Location = new System.Drawing.Point(287, 5);
			this.ShowPinButton.Name = "ShowPinButton";
			this.ShowPinButton.Size = new System.Drawing.Size(104, 23);
			this.ShowPinButton.TabIndex = 2;
			this.ShowPinButton.Text = "Show Pin";
			this.ShowPinButton.UseVisualStyleBackColor = true;
			this.ShowPinButton.Click += new System.EventHandler(this.ShowPinButton_Click);
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisconnectButton.Location = new System.Drawing.Point(500, 5);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(145, 23);
			this.DisconnectButton.TabIndex = 3;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.BackColor = System.Drawing.SystemColors.Info;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(10, 5);
			this.RowStatusLabel.Margin = new System.Windows.Forms.Padding(0);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Padding = new System.Windows.Forms.Padding(5);
			this.RowStatusLabel.Size = new System.Drawing.Size(108, 23);
			this.RowStatusLabel.TabIndex = 0;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			this.RowStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ViewAppsButton
			// 
			this.ViewAppsButton.AutoSize = true;
			this.ViewAppsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.ViewAppsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ViewAppsButton.Location = new System.Drawing.Point(402, 4);
			this.ViewAppsButton.Name = "ViewAppsButton";
			this.ViewAppsButton.Size = new System.Drawing.Size(87, 25);
			this.ViewAppsButton.TabIndex = 5;
			this.ViewAppsButton.Text = "View Apps";
			this.ViewAppsButton.UseVisualStyleBackColor = true;
			this.ViewAppsButton.Click += new System.EventHandler(this.ViewAppsButton_Click);
			// 
			// Controller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(976, 553);
			this.Controls.Add(this.ViewAppsButton);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ShowPinButton);
			this.Controls.Add(this.WaterSkiConnectButton);
			this.Controls.Add(this.MessageView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Controller";
			this.Text = "Message Handling Controller";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Controller_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Controller_FormClosed);
			this.Load += new System.EventHandler(this.Controller_Load);
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView MessageView;
		private System.Windows.Forms.Button WaterSkiConnectButton;
		private System.Windows.Forms.Button ShowPinButton;
		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdateDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn Message;
		private System.Windows.Forms.Button ViewAppsButton;
	}
}