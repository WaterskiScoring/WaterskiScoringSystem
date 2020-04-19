namespace WaterskiScoringSystem.Tools {
    partial class TourExportDialogForm {
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
            this.ListButton = new System.Windows.Forms.Button();
            this.DataButton = new System.Windows.Forms.Button();
            this.Cancelbutton = new System.Windows.Forms.Button();
            this.dialogMsg = new System.Windows.Forms.Label();
            this.WspButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ListButton
            // 
            this.ListButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ListButton.Location = new System.Drawing.Point(12, 41);
            this.ListButton.Name = "ListButton";
            this.ListButton.Size = new System.Drawing.Size(95, 23);
            this.ListButton.TabIndex = 1;
            this.ListButton.Text = "Tournament List";
            this.ListButton.UseVisualStyleBackColor = true;
            this.ListButton.Click += new System.EventHandler(this.ListButton_Click);
            // 
            // DataButton
            // 
            this.DataButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.DataButton.Location = new System.Drawing.Point(107, 41);
            this.DataButton.Name = "DataButton";
            this.DataButton.Size = new System.Drawing.Size(107, 23);
            this.DataButton.TabIndex = 2;
            this.DataButton.Text = "Tournament Data";
            this.DataButton.UseVisualStyleBackColor = true;
            this.DataButton.Click += new System.EventHandler(this.DataButton_Click);
            // 
            // Cancelbutton
            // 
            this.Cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelbutton.Location = new System.Drawing.Point(321, 40);
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
            this.dialogMsg.Location = new System.Drawing.Point(100, 9);
            this.dialogMsg.Name = "dialogMsg";
            this.dialogMsg.Size = new System.Drawing.Size(224, 18);
            this.dialogMsg.TabIndex = 0;
            this.dialogMsg.Text = "What do you want to export?";
            this.dialogMsg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WspButton
            // 
            this.WspButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.WspButton.Location = new System.Drawing.Point(214, 41);
            this.WspButton.Name = "WspButton";
            this.WspButton.Size = new System.Drawing.Size(100, 23);
            this.WspButton.TabIndex = 3;
            this.WspButton.Text = "Performance Data";
            this.WspButton.UseVisualStyleBackColor = true;
            this.WspButton.Click += new System.EventHandler(this.WspButton_Click);
            // 
            // TourExportDialogForm
            // 
            this.AcceptButton = this.ListButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancelbutton;
            this.ClientSize = new System.Drawing.Size(408, 73);
            this.Controls.Add(this.WspButton);
            this.Controls.Add(this.dialogMsg);
            this.Controls.Add(this.Cancelbutton);
            this.Controls.Add(this.DataButton);
            this.Controls.Add(this.ListButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TourExportDialogForm";
            this.Text = "TourExportDialogForm";
            this.Load += new System.EventHandler(this.TourExportDialogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ListButton;
        private System.Windows.Forms.Button DataButton;
        private System.Windows.Forms.Button Cancelbutton;
        private System.Windows.Forms.Label dialogMsg;
        private System.Windows.Forms.Button WspButton;
    }
}