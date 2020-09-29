namespace WaterskiScoringSystem.Tools {
    partial class PrintOfficialFormDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && ( components != null )) {
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
			this.SlalomRecapButton = new System.Windows.Forms.Button();
			this.SlalomBoatButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// dialogMsg
			// 
			this.dialogMsg.AutoSize = true;
			this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.dialogMsg.Location = new System.Drawing.Point(33, 6);
			this.dialogMsg.Name = "dialogMsg";
			this.dialogMsg.Size = new System.Drawing.Size(258, 18);
			this.dialogMsg.TabIndex = 0;
			this.dialogMsg.Text = "Which form do you want to print?";
			this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Cancelbutton
			// 
			this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancelbutton.Location = new System.Drawing.Point(271, 34);
			this.Cancelbutton.Name = "Cancelbutton";
			this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
			this.Cancelbutton.TabIndex = 30;
			this.Cancelbutton.Text = "Cancel";
			this.Cancelbutton.UseVisualStyleBackColor = true;
			// 
			// SlalomRecapButton
			// 
			this.SlalomRecapButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SlalomRecapButton.Location = new System.Drawing.Point(149, 35);
			this.SlalomRecapButton.Name = "SlalomRecapButton";
			this.SlalomRecapButton.Size = new System.Drawing.Size(114, 23);
			this.SlalomRecapButton.TabIndex = 20;
			this.SlalomRecapButton.Text = "Slalom Recap Form";
			this.SlalomRecapButton.UseVisualStyleBackColor = true;
			this.SlalomRecapButton.Click += new System.EventHandler(this.SlalomRecapButton_Click);
			// 
			// SlalomBoatButton
			// 
			this.SlalomBoatButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.SlalomBoatButton.Location = new System.Drawing.Point(5, 35);
			this.SlalomBoatButton.Name = "SlalomBoatButton";
			this.SlalomBoatButton.Size = new System.Drawing.Size(136, 23);
			this.SlalomBoatButton.TabIndex = 10;
			this.SlalomBoatButton.Text = "Slalom Judge Form";
			this.SlalomBoatButton.UseVisualStyleBackColor = true;
			this.SlalomBoatButton.Click += new System.EventHandler(this.SlalomBoatButton_Click);
			// 
			// PrintOfficialFormDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(355, 69);
			this.Controls.Add(this.dialogMsg);
			this.Controls.Add(this.Cancelbutton);
			this.Controls.Add(this.SlalomRecapButton);
			this.Controls.Add(this.SlalomBoatButton);
			this.Name = "PrintOfficialFormDialog";
			this.Text = "PrintOfficialFormDialog";
			this.Load += new System.EventHandler(this.PrintOfficialFormDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label dialogMsg;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Button SlalomRecapButton;
        private System.Windows.Forms.Button SlalomBoatButton;
    }
}