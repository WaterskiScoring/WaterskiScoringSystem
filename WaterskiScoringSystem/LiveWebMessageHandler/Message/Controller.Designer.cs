
namespace LiveWebMessageHandler.Message {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Controller));
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.MessageView = new System.Windows.Forms.DataGridView();
			this.CreationDatetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QueueView = new System.Windows.Forms.DataGridView();
			this.QueueCreateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QueueMsgType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MsgDataHash = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QueueMsgData = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QueueButton = new System.Windows.Forms.Button();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.QueueView)).BeginInit();
			this.SuspendLayout();
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisconnectButton.Location = new System.Drawing.Point(288, 4);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(145, 23);
			this.DisconnectButton.TabIndex = 2;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
			// 
			// ConnectButton
			// 
			this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectButton.Location = new System.Drawing.Point(133, 4);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(145, 23);
			this.ConnectButton.TabIndex = 1;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
			this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
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
            this.CreationDatetime,
            this.Message});
			this.MessageView.Location = new System.Drawing.Point(5, 33);
			this.MessageView.Name = "MessageView";
			this.MessageView.ReadOnly = true;
			this.MessageView.Size = new System.Drawing.Size(805, 412);
			this.MessageView.TabIndex = 4;
			this.MessageView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MessageView_CellContentDoubleClick);
			// 
			// CreationDatetime
			// 
			this.CreationDatetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle1.NullValue = null;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.CreationDatetime.DefaultCellStyle = dataGridViewCellStyle1;
			this.CreationDatetime.HeaderText = "DateTime";
			this.CreationDatetime.MinimumWidth = 100;
			this.CreationDatetime.Name = "CreationDatetime";
			this.CreationDatetime.ReadOnly = true;
			// 
			// Message
			// 
			this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Message.HeaderText = "Message";
			this.Message.Name = "Message";
			this.Message.ReadOnly = true;
			this.Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Message.Width = 500;
			// 
			// QueueView
			// 
			this.QueueView.AllowUserToDeleteRows = false;
			this.QueueView.AllowUserToOrderColumns = true;
			this.QueueView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.QueueView.BackgroundColor = System.Drawing.SystemColors.Info;
			this.QueueView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.QueueView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QueueCreateDate,
            this.QueueMsgType,
            this.MsgDataHash,
            this.QueueMsgData});
			this.QueueView.Enabled = false;
			this.QueueView.Location = new System.Drawing.Point(42, 74);
			this.QueueView.Name = "QueueView";
			this.QueueView.ReadOnly = true;
			this.QueueView.Size = new System.Drawing.Size(758, 274);
			this.QueueView.TabIndex = 0;
			this.QueueView.Visible = false;
			this.QueueView.Leave += new System.EventHandler(this.QueueView_Leave);
			// 
			// QueueCreateDate
			// 
			this.QueueCreateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.NullValue = null;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.QueueCreateDate.DefaultCellStyle = dataGridViewCellStyle2;
			this.QueueCreateDate.HeaderText = "Create Time";
			this.QueueCreateDate.MaxInputLength = 50;
			this.QueueCreateDate.MinimumWidth = 75;
			this.QueueCreateDate.Name = "QueueCreateDate";
			this.QueueCreateDate.ReadOnly = true;
			this.QueueCreateDate.Width = 120;
			// 
			// QueueMsgType
			// 
			this.QueueMsgType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.QueueMsgType.HeaderText = "Message Type";
			this.QueueMsgType.Name = "QueueMsgType";
			this.QueueMsgType.ReadOnly = true;
			this.QueueMsgType.Width = 75;
			// 
			// MsgDataHash
			// 
			this.MsgDataHash.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MsgDataHash.HeaderText = "Hash";
			this.MsgDataHash.MaxInputLength = 64;
			this.MsgDataHash.Name = "MsgDataHash";
			this.MsgDataHash.ReadOnly = true;
			this.MsgDataHash.Width = 75;
			// 
			// QueueMsgData
			// 
			this.QueueMsgData.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.QueueMsgData.HeaderText = "Message";
			this.QueueMsgData.Name = "QueueMsgData";
			this.QueueMsgData.ReadOnly = true;
			this.QueueMsgData.Width = 75;
			// 
			// QueueButton
			// 
			this.QueueButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.QueueButton.Location = new System.Drawing.Point(450, 4);
			this.QueueButton.Name = "QueueButton";
			this.QueueButton.Size = new System.Drawing.Size(145, 23);
			this.QueueButton.TabIndex = 3;
			this.QueueButton.Text = "Queue";
			this.QueueButton.UseVisualStyleBackColor = true;
			this.QueueButton.Click += new System.EventHandler(this.QueueButton_Click);
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.BackColor = System.Drawing.SystemColors.Info;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(9, 4);
			this.RowStatusLabel.Margin = new System.Windows.Forms.Padding(0);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Padding = new System.Windows.Forms.Padding(5);
			this.RowStatusLabel.Size = new System.Drawing.Size(108, 23);
			this.RowStatusLabel.TabIndex = 5;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			this.RowStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Controller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(813, 450);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.QueueButton);
			this.Controls.Add(this.QueueView);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.MessageView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Controller";
			this.Text = "Live Web Message Handler";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Controller_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Controller_FormClosed);
			this.Load += new System.EventHandler(this.Controller_Load);
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.QueueView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Button ConnectButton;
		private System.Windows.Forms.DataGridView MessageView;
		private System.Windows.Forms.DataGridView QueueView;
		private System.Windows.Forms.DataGridViewTextBoxColumn CreationDatetime;
		private System.Windows.Forms.DataGridViewTextBoxColumn Message;
		private System.Windows.Forms.Button QueueButton;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.DataGridViewTextBoxColumn QueueCreateDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn QueueMsgType;
		private System.Windows.Forms.DataGridViewTextBoxColumn MsgDataHash;
		private System.Windows.Forms.DataGridViewTextBoxColumn QueueMsgData;
	}
}