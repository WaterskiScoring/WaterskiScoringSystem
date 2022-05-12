
namespace WaterskiScoringSystem.Slalom {
	partial class PrintSlalomFormDialog {
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
			this.SlalomRecapFormButton = new System.Windows.Forms.Button();
			this.SlalomJudgeFormButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dialogMsg
			// 
			this.dialogMsg.AutoSize = true;
			this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dialogMsg.Location = new System.Drawing.Point(64, 8);
			this.dialogMsg.Name = "dialogMsg";
			this.dialogMsg.Size = new System.Drawing.Size(258, 18);
			this.dialogMsg.TabIndex = 31;
			this.dialogMsg.Text = "Which form do you want to print?";
			this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cancelbutton
			// 
			this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancelbutton.Location = new System.Drawing.Point(300, 36);
			this.Cancelbutton.Name = "Cancelbutton";
			this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
			this.Cancelbutton.TabIndex = 34;
			this.Cancelbutton.Text = "Cancel";
			this.Cancelbutton.UseVisualStyleBackColor = true;
			// 
			// SlalomRecapFormButton
			// 
			this.SlalomRecapFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SlalomRecapFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SlalomRecapFormButton.Location = new System.Drawing.Point(150, 37);
			this.SlalomRecapFormButton.Name = "SlalomRecapFormButton";
			this.SlalomRecapFormButton.Size = new System.Drawing.Size(142, 23);
			this.SlalomRecapFormButton.TabIndex = 2;
			this.SlalomRecapFormButton.Text = "Slalom Recap Form";
			this.SlalomRecapFormButton.UseVisualStyleBackColor = true;
			this.SlalomRecapFormButton.Click += new System.EventHandler(this.SlalomRecapFormButton_Click);
			// 
			// SlalomJudgeFormButton
			// 
			this.SlalomJudgeFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SlalomJudgeFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.SlalomJudgeFormButton.Location = new System.Drawing.Point(6, 37);
			this.SlalomJudgeFormButton.Name = "SlalomJudgeFormButton";
			this.SlalomJudgeFormButton.Size = new System.Drawing.Size(136, 23);
			this.SlalomJudgeFormButton.TabIndex = 1;
			this.SlalomJudgeFormButton.Text = "Slalom Judge Form";
			this.SlalomJudgeFormButton.UseVisualStyleBackColor = true;
			this.SlalomJudgeFormButton.Click += new System.EventHandler(this.SlalomJudgeFormButton_Click);
			// 
			// PrintSlalomFormDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(383, 73);
			this.Controls.Add(this.dialogMsg);
			this.Controls.Add(this.Cancelbutton);
			this.Controls.Add(this.SlalomRecapFormButton);
			this.Controls.Add(this.SlalomJudgeFormButton);
			this.Name = "PrintSlalomFormDialog";
			this.Text = "PrintSlalomFormDialog";
			this.Load += new System.EventHandler(this.PrintSlalomFormDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label dialogMsg;
		private System.Windows.Forms.Button Cancelbutton;
		private System.Windows.Forms.Button SlalomRecapFormButton;
		private System.Windows.Forms.Button SlalomJudgeFormButton;
	}
}