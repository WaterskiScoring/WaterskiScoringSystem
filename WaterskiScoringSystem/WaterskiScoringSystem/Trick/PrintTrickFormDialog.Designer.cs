
namespace WaterskiScoringSystem.Trick {
	partial class PrintTrickFormDialog {
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
			this.TrickTimingFormButton = new System.Windows.Forms.Button();
			this.TrickOfficialFormButton = new System.Windows.Forms.Button();
			this.TrickSkierSpecFormButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dialogMsg
			// 
			this.dialogMsg.AutoSize = true;
			this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dialogMsg.Location = new System.Drawing.Point(109, 8);
			this.dialogMsg.Name = "dialogMsg";
			this.dialogMsg.Size = new System.Drawing.Size(258, 18);
			this.dialogMsg.TabIndex = 31;
			this.dialogMsg.Text = "Which form do you want to print?";
			this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cancelbutton
			// 
			this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancelbutton.Location = new System.Drawing.Point(407, 36);
			this.Cancelbutton.Name = "Cancelbutton";
			this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
			this.Cancelbutton.TabIndex = 34;
			this.Cancelbutton.Text = "Cancel";
			this.Cancelbutton.UseVisualStyleBackColor = true;
			// 
			// TrickTimingFormButton
			// 
			this.TrickTimingFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.TrickTimingFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TrickTimingFormButton.Location = new System.Drawing.Point(145, 37);
			this.TrickTimingFormButton.Name = "TrickTimingFormButton";
			this.TrickTimingFormButton.Size = new System.Drawing.Size(142, 23);
			this.TrickTimingFormButton.TabIndex = 2;
			this.TrickTimingFormButton.Text = "Trick Timing Form";
			this.TrickTimingFormButton.UseVisualStyleBackColor = true;
			this.TrickTimingFormButton.Click += new System.EventHandler(this.TrickTimingFormButton_Click);
			// 
			// TrickOfficialFormButton
			// 
			this.TrickOfficialFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.TrickOfficialFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TrickOfficialFormButton.Location = new System.Drawing.Point(6, 37);
			this.TrickOfficialFormButton.Name = "TrickOfficialFormButton";
			this.TrickOfficialFormButton.Size = new System.Drawing.Size(136, 23);
			this.TrickOfficialFormButton.TabIndex = 1;
			this.TrickOfficialFormButton.Text = "Trick Official Sheets";
			this.TrickOfficialFormButton.UseVisualStyleBackColor = true;
			this.TrickOfficialFormButton.Click += new System.EventHandler(this.TrickOfficialFormButton_Click);
			// 
			// TrickSkierSpecFormButton
			// 
			this.TrickSkierSpecFormButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.TrickSkierSpecFormButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TrickSkierSpecFormButton.Location = new System.Drawing.Point(290, 37);
			this.TrickSkierSpecFormButton.Name = "TrickSkierSpecFormButton";
			this.TrickSkierSpecFormButton.Size = new System.Drawing.Size(114, 23);
			this.TrickSkierSpecFormButton.TabIndex = 3;
			this.TrickSkierSpecFormButton.Text = "Trick Skier Spec Form";
			this.TrickSkierSpecFormButton.UseVisualStyleBackColor = true;
			this.TrickSkierSpecFormButton.Click += new System.EventHandler(this.TrickSkierSpecFormButton_Click);
			// 
			// PrintTrickFormDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 73);
			this.Controls.Add(this.TrickSkierSpecFormButton);
			this.Controls.Add(this.dialogMsg);
			this.Controls.Add(this.Cancelbutton);
			this.Controls.Add(this.TrickTimingFormButton);
			this.Controls.Add(this.TrickOfficialFormButton);
			this.Name = "PrintTrickFormDialog";
			this.Text = "PrintTrickFormDialog";
			this.Load += new System.EventHandler(this.PrintTrickFormDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label dialogMsg;
		private System.Windows.Forms.Button Cancelbutton;
		private System.Windows.Forms.Button TrickTimingFormButton;
		private System.Windows.Forms.Button TrickOfficialFormButton;
		private System.Windows.Forms.Button TrickSkierSpecFormButton;
	}
}