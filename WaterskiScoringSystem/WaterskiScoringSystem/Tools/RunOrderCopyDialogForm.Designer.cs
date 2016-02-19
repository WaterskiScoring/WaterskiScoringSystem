namespace WaterskiScoringSystem.Tools {
    partial class RunOrderCopyDialogForm {
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.CopyByBestButton = new System.Windows.Forms.Button();
            this.CopyByLastButton = new System.Windows.Forms.Button();
            this.CopyByTotalButton = new System.Windows.Forms.Button();
            this.CopyPrevButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CopyByBestButton
            // 
            this.CopyByBestButton.AutoSize = true;
            this.CopyByBestButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopyByBestButton.Location = new System.Drawing.Point(141, 35);
            this.CopyByBestButton.Name = "CopyByBestButton";
            this.CopyByBestButton.Size = new System.Drawing.Size(75, 23);
            this.CopyByBestButton.TabIndex = 20;
            this.CopyByBestButton.Text = "Best Score";
            this.CopyByBestButton.UseVisualStyleBackColor = true;
            this.CopyByBestButton.Click += new System.EventHandler(this.CopyByBestButton_Click);
            // 
            // CopyByLastButton
            // 
            this.CopyByLastButton.AutoSize = true;
            this.CopyByLastButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopyByLastButton.Location = new System.Drawing.Point(230, 35);
            this.CopyByLastButton.Name = "CopyByLastButton";
            this.CopyByLastButton.Size = new System.Drawing.Size(124, 23);
            this.CopyByLastButton.TabIndex = 30;
            this.CopyByLastButton.Text = "Previous Round Score";
            this.CopyByLastButton.UseVisualStyleBackColor = true;
            this.CopyByLastButton.Click += new System.EventHandler(this.CopyByLastButton_Click);
            // 
            // CopyByTotalButton
            // 
            this.CopyByTotalButton.AutoSize = true;
            this.CopyByTotalButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopyByTotalButton.Location = new System.Drawing.Point(368, 35);
            this.CopyByTotalButton.Name = "CopyByTotalButton";
            this.CopyByTotalButton.Size = new System.Drawing.Size(75, 23);
            this.CopyByTotalButton.TabIndex = 40;
            this.CopyByTotalButton.Text = "Total Score";
            this.CopyByTotalButton.UseVisualStyleBackColor = true;
            this.CopyByTotalButton.Click += new System.EventHandler(this.CopyByTotalButton_Click);
            // 
            // CopyPrevButton
            // 
            this.CopyPrevButton.AutoSize = true;
            this.CopyPrevButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.CopyPrevButton.Location = new System.Drawing.Point(5, 35);
            this.CopyPrevButton.Name = "CopyPrevButton";
            this.CopyPrevButton.Size = new System.Drawing.Size(122, 23);
            this.CopyPrevButton.TabIndex = 10;
            this.CopyPrevButton.Text = "Previous Round Order";
            this.CopyPrevButton.UseVisualStyleBackColor = true;
            this.CopyPrevButton.Click += new System.EventHandler(this.CopyPrevButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(437, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select desired method to create running order for round ";
            // 
            // CancelButton
            // 
            this.CancelButton.AutoSize = true;
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(457, 35);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 50;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // RunOrderCopyDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(546, 66);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CopyPrevButton);
            this.Controls.Add(this.CopyByTotalButton);
            this.Controls.Add(this.CopyByLastButton);
            this.Controls.Add(this.CopyByBestButton);
            this.Name = "RunOrderCopyDialogForm";
            this.Text = "New Run Order Method";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RunOrderCopyDialogForm_FormClosed);
            this.Load += new System.EventHandler(this.RunOrderCopyDialogForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CopyByBestButton;
        private System.Windows.Forms.Button CopyByLastButton;
        private System.Windows.Forms.Button CopyByTotalButton;
        private System.Windows.Forms.Button CopyPrevButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button CancelButton;
    }
}