namespace WaterskiScoringSystem.Tools {
    partial class TeamPrintDialogForm {
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
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.SkierListButton = new System.Windows.Forms.Button();
            this.TeamResultsButton = new System.Windows.Forms.Button();
            this.dialogMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Location = new System.Drawing.Point(253, 37);
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.Cancelbutton.TabIndex = 7;
            this.Cancelbutton.Text = "Cancel";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // SkierListButton
            // 
            this.SkierListButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SkierListButton.Location = new System.Drawing.Point(125, 38);
            this.SkierListButton.Name = "SkierListButton";
            this.SkierListButton.Size = new System.Drawing.Size(107, 23);
            this.SkierListButton.TabIndex = 20;
            this.SkierListButton.Text = "Skier List by Team";
            this.SkierListButton.UseVisualStyleBackColor = true;
            this.SkierListButton.Click += new System.EventHandler(this.SkierListButton_Click);
            // 
            // TeamResultsButton
            // 
            this.TeamResultsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.TeamResultsButton.Location = new System.Drawing.Point(9, 38);
            this.TeamResultsButton.Name = "TeamResultsButton";
            this.TeamResultsButton.Size = new System.Drawing.Size(95, 23);
            this.TeamResultsButton.TabIndex = 10;
            this.TeamResultsButton.Text = "Team Results";
            this.TeamResultsButton.UseVisualStyleBackColor = true;
            this.TeamResultsButton.Click += new System.EventHandler(this.TeamResultsButton_Click);
            // 
            // dialogMsg
            // 
            this.dialogMsg.AutoSize = true;
            this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dialogMsg.Location = new System.Drawing.Point(37, 9);
            this.dialogMsg.Name = "dialogMsg";
            this.dialogMsg.Size = new System.Drawing.Size(268, 18);
            this.dialogMsg.TabIndex = 30;
            this.dialogMsg.Text = "Which report do you want to print?";
            this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TeamPrintDialogForm
            // 
            this.AcceptButton = this.TeamResultsButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancelbutton;
            this.ClientSize = new System.Drawing.Size(348, 72);
            this.Controls.Add(this.dialogMsg);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.SkierListButton);
            this.Controls.Add(this.TeamResultsButton);
            this.Name = "TeamPrintDialogForm";
            this.Text = "TeamPrintDialogForm";
            this.Load += new System.EventHandler(this.TeamPrintDialogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Button SkierListButton;
        private System.Windows.Forms.Button TeamResultsButton;
        private System.Windows.Forms.Label dialogMsg;
    }
}