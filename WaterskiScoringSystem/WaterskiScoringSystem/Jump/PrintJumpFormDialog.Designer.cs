
namespace WaterskiScoringSystem.Jump {
	partial class PrintJumpFormDialog {
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
			this.dialogMsg = new System.Windows.Forms.Label();
			this.Cancelbutton = new System.Windows.Forms.Button();
			this.JumpOfficialFormButton = new System.Windows.Forms.Button();
			this.JumpSkierSpecFormButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dialogMsg
			// 
			this.dialogMsg.AutoSize = true;
			this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dialogMsg.Location = new System.Drawing.Point(71, 8);
			this.dialogMsg.Name = "dialogMsg";
			this.dialogMsg.Size = new System.Drawing.Size(258, 18);
			this.dialogMsg.TabIndex = 31;
			this.dialogMsg.Text = "Which form do you want to print?";
			this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cancelbutton
			// 
			this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancelbutton.Location = new System.Drawing.Point(312, 39);
			this.Cancelbutton.Name = "Cancelbutton";
			this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
			this.Cancelbutton.TabIndex = 34;
			this.Cancelbutton.Text = "Cancel";
			this.Cancelbutton.UseVisualStyleBackColor = true;
			// 
			// JumpOfficialFormButton
			// 
			this.JumpOfficialFormButton.AutoSize = true;
			this.JumpOfficialFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.JumpOfficialFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.JumpOfficialFormButton.Location = new System.Drawing.Point(6, 37);
			this.JumpOfficialFormButton.Name = "JumpOfficialFormButton";
			this.JumpOfficialFormButton.Size = new System.Drawing.Size(136, 26);
			this.JumpOfficialFormButton.TabIndex = 1;
			this.JumpOfficialFormButton.Text = "Jump Official Form";
			this.JumpOfficialFormButton.UseVisualStyleBackColor = true;
			this.JumpOfficialFormButton.Click += new System.EventHandler(this.JumpOfficialFormButton_Click);
			// 
			// JumpSkierSpecFormButton
			// 
			this.JumpSkierSpecFormButton.AutoSize = true;
			this.JumpSkierSpecFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.JumpSkierSpecFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.JumpSkierSpecFormButton.Location = new System.Drawing.Point(150, 37);
			this.JumpSkierSpecFormButton.Name = "JumpSkierSpecFormButton";
			this.JumpSkierSpecFormButton.Size = new System.Drawing.Size(154, 26);
			this.JumpSkierSpecFormButton.TabIndex = 3;
			this.JumpSkierSpecFormButton.Text = "Jump Skier Spec Form";
			this.JumpSkierSpecFormButton.UseVisualStyleBackColor = true;
			this.JumpSkierSpecFormButton.Click += new System.EventHandler(this.JumpSkierSpecFormButton_Click);
			// 
			// PrintJumpFormDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(402, 73);
			this.Controls.Add(this.JumpSkierSpecFormButton);
			this.Controls.Add(this.dialogMsg);
			this.Controls.Add(this.Cancelbutton);
			this.Controls.Add(this.JumpOfficialFormButton);
			this.Name = "PrintJumpFormDialog";
			this.Text = "PrintJumpFormDialog";
			this.Load += new System.EventHandler(this.PrintJumpFormDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label dialogMsg;
		private System.Windows.Forms.Button Cancelbutton;
		private System.Windows.Forms.Button JumpOfficialFormButton;
		private System.Windows.Forms.Button JumpSkierSpecFormButton;
	}
}