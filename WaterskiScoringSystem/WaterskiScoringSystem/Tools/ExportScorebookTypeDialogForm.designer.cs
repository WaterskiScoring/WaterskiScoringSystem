namespace WaterskiScoringSystem.Tools {
    partial class ExportScorebookTypeDialogForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.StdFormatButton = new System.Windows.Forms.Button();
            this.IndexFormatButton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.dialogMsg = new System.Windows.Forms.Label();
            this.MagazineFormatButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // StdFormatButton
            // 
            this.StdFormatButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.StdFormatButton.Location = new System.Drawing.Point(12, 41);
            this.StdFormatButton.Name = "StdFormatButton";
            this.StdFormatButton.Size = new System.Drawing.Size(95, 23);
            this.StdFormatButton.TabIndex = 1;
            this.StdFormatButton.Text = "Standard";
            this.StdFormatButton.UseVisualStyleBackColor = true;
            this.StdFormatButton.Click += new System.EventHandler(this.StdFormatButton_Click);
            // 
            // IndexFormatButton
            // 
            this.IndexFormatButton.AutoSize = true;
            this.IndexFormatButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.IndexFormatButton.Location = new System.Drawing.Point(115, 41);
            this.IndexFormatButton.Name = "IndexFormatButton";
            this.IndexFormatButton.Size = new System.Drawing.Size(120, 23);
            this.IndexFormatButton.TabIndex = 2;
            this.IndexFormatButton.Text = "Championship / Index";
            this.IndexFormatButton.UseVisualStyleBackColor = true;
            this.IndexFormatButton.Click += new System.EventHandler(this.IndexFormatButton_Click);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Location = new System.Drawing.Point(346, 41);
            this.Cancelbutton.Name = "Cancelbutton";
            this.Cancelbutton.Size = new System.Drawing.Size(75, 23);
            this.Cancelbutton.TabIndex = 4;
            this.Cancelbutton.Text = "Cancel";
            this.Cancelbutton.UseVisualStyleBackColor = true;
            // 
            // dialogMsg
            // 
            this.dialogMsg.AutoSize = true;
            this.dialogMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dialogMsg.Location = new System.Drawing.Point(137, 11);
            this.dialogMsg.Name = "dialogMsg";
            this.dialogMsg.Size = new System.Drawing.Size(170, 18);
            this.dialogMsg.TabIndex = 0;
            this.dialogMsg.Text = "Select Report Format";
            this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MagazineFormatButton
            // 
            this.MagazineFormatButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.MagazineFormatButton.Location = new System.Drawing.Point(243, 41);
            this.MagazineFormatButton.Name = "MagazineFormatButton";
            this.MagazineFormatButton.Size = new System.Drawing.Size(95, 23);
            this.MagazineFormatButton.TabIndex = 3;
            this.MagazineFormatButton.Text = "Magazine";
            this.MagazineFormatButton.UseVisualStyleBackColor = true;
            this.MagazineFormatButton.Click += new System.EventHandler(this.MagazineFormatButton_Click);
            // 
            // ExportScorebookTypeDialogForm
            // 
            this.AcceptButton = this.StdFormatButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancelbutton;
            this.ClientSize = new System.Drawing.Size(433, 73);
            this.Controls.Add(this.MagazineFormatButton);
            this.Controls.Add(this.dialogMsg);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.IndexFormatButton);
            this.Controls.Add(this.StdFormatButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ExportScorebookTypeDialogForm";
            this.Text = "ExportScorebookTypeDialogForm";
            this.Load += new System.EventHandler(this.ExportScorebookTypeDialogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StdFormatButton;
        private System.Windows.Forms.Button IndexFormatButton;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Label dialogMsg;
        private System.Windows.Forms.Button MagazineFormatButton;
    }
}