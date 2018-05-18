namespace WaterskiScoringSystem.Slalom {
	partial class NextPassDialog {
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
			this.NextCancelButton = new System.Windows.Forms.Button();
			this.NextLineButton = new System.Windows.Forms.Button();
			this.NextSpeedButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// NextCancelButton
			// 
			this.NextCancelButton.BackColor = System.Drawing.Color.Maroon;
			this.NextCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.NextCancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.NextCancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NextCancelButton.ForeColor = System.Drawing.Color.White;
			this.NextCancelButton.Location = new System.Drawing.Point(186, 2);
			this.NextCancelButton.Name = "NextCancelButton";
			this.NextCancelButton.Size = new System.Drawing.Size(20, 23);
			this.NextCancelButton.TabIndex = 3;
			this.NextCancelButton.Text = "X";
			this.NextCancelButton.UseVisualStyleBackColor = false;
			this.NextCancelButton.Enter += new System.EventHandler(this.NextCancelButton_Enter);
			// 
			// NextLineButton
			// 
			this.NextLineButton.BackColor = System.Drawing.Color.Maroon;
			this.NextLineButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.NextLineButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.NextLineButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NextLineButton.ForeColor = System.Drawing.Color.White;
			this.NextLineButton.Location = new System.Drawing.Point(96, 2);
			this.NextLineButton.Name = "NextLineButton";
			this.NextLineButton.Size = new System.Drawing.Size(85, 23);
			this.NextLineButton.TabIndex = 2;
			this.NextLineButton.Text = "Next Line";
			this.NextLineButton.UseVisualStyleBackColor = false;
			this.NextLineButton.Click += new System.EventHandler(this.NextLineButton_Click);
			this.NextLineButton.Enter += new System.EventHandler(this.NextLineButton_Enter);
			// 
			// NextSpeedButton
			// 
			this.NextSpeedButton.BackColor = System.Drawing.Color.Maroon;
			this.NextSpeedButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.NextSpeedButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.NextSpeedButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.NextSpeedButton.ForeColor = System.Drawing.Color.White;
			this.NextSpeedButton.Location = new System.Drawing.Point(5, 2);
			this.NextSpeedButton.Name = "NextSpeedButton";
			this.NextSpeedButton.Size = new System.Drawing.Size(85, 23);
			this.NextSpeedButton.TabIndex = 1;
			this.NextSpeedButton.Text = "Next Speed";
			this.NextSpeedButton.UseVisualStyleBackColor = false;
			this.NextSpeedButton.Click += new System.EventHandler(this.NextSpeedButton_Click);
			this.NextSpeedButton.Enter += new System.EventHandler(this.NextSpeedButton_Enter);
			// 
			// NextPassDialog
			// 
			this.AcceptButton = this.NextSpeedButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.CancelButton = this.NextCancelButton;
			this.ClientSize = new System.Drawing.Size(212, 26);
			this.ControlBox = false;
			this.Controls.Add(this.NextCancelButton);
			this.Controls.Add(this.NextLineButton);
			this.Controls.Add(this.NextSpeedButton);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.Color.CornflowerBlue;
			this.Name = "NextPassDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Next Pass Select";
			this.Load += new System.EventHandler(this.NextPassDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button NextCancelButton;
		private System.Windows.Forms.Button NextLineButton;
		private System.Windows.Forms.Button NextSpeedButton;
	}
}