
namespace HttpMessageHandler.Message {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.DisconnectButton = new System.Windows.Forms.Button();
			this.ConnectButton = new System.Windows.Forms.Button();
			this.MessageView = new System.Windows.Forms.DataGridView();
			this.CreationDatetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).BeginInit();
			this.SuspendLayout();
			// 
			// DisconnectButton
			// 
			this.DisconnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisconnectButton.Location = new System.Drawing.Point(160, 4);
			this.DisconnectButton.Name = "DisconnectButton";
			this.DisconnectButton.Size = new System.Drawing.Size(145, 23);
			this.DisconnectButton.TabIndex = 8;
			this.DisconnectButton.Text = "Disconnect";
			this.DisconnectButton.UseVisualStyleBackColor = true;
			// 
			// ConnectButton
			// 
			this.ConnectButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ConnectButton.Location = new System.Drawing.Point(5, 4);
			this.ConnectButton.Name = "ConnectButton";
			this.ConnectButton.Size = new System.Drawing.Size(145, 23);
			this.ConnectButton.TabIndex = 6;
			this.ConnectButton.Text = "Connect";
			this.ConnectButton.UseVisualStyleBackColor = true;
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
			this.MessageView.Size = new System.Drawing.Size(792, 412);
			this.MessageView.TabIndex = 5;
			// 
			// CreationDatetime
			// 
			this.CreationDatetime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle3.NullValue = null;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.CreationDatetime.DefaultCellStyle = dataGridViewCellStyle3;
			this.CreationDatetime.HeaderText = "DateTime";
			this.CreationDatetime.MinimumWidth = 100;
			this.CreationDatetime.Name = "CreationDatetime";
			this.CreationDatetime.ReadOnly = true;
			// 
			// Message
			// 
			this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Message.HeaderText = "Message";
			this.Message.MinimumWidth = 200;
			this.Message.Name = "Message";
			this.Message.ReadOnly = true;
			this.Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.Message.Width = 200;
			// 
			// Controller
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.DisconnectButton);
			this.Controls.Add(this.ConnectButton);
			this.Controls.Add(this.MessageView);
			this.Name = "Controller";
			this.Text = "Message Handling Controller";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Controller_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Controller_FormClosed);
			this.Load += new System.EventHandler(this.Controller_Load);
			((System.ComponentModel.ISupportInitialize)(this.MessageView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button DisconnectButton;
		private System.Windows.Forms.Button ConnectButton;
		private System.Windows.Forms.DataGridView MessageView;
		private System.Windows.Forms.DataGridViewTextBoxColumn CreationDatetime;
		private System.Windows.Forms.DataGridViewTextBoxColumn Message;
	}
}