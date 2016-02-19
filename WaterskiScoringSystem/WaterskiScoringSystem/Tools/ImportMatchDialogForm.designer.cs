namespace WaterskiScoringSystem.Tools {
    partial class ImportMatchDialogForm {
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
            this.UpdateButton = new System.Windows.Forms.Button();
            this.SkipButton = new System.Windows.Forms.Button();
            this.UpdateAllButton = new System.Windows.Forms.Button();
            this.SkipAllButton = new System.Windows.Forms.Button();
            this.WindowTitle = new System.Windows.Forms.Label();
            this.ShowImportKeyData = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // UpdateButton
            // 
            this.UpdateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateButton.Location = new System.Drawing.Point( 3, 194 );
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size( 105, 23 );
            this.UpdateButton.TabIndex = 1;
            this.UpdateButton.Text = "Update Match";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler( this.UpdateButton_Click );
            // 
            // SkipButton
            // 
            this.SkipButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SkipButton.Location = new System.Drawing.Point( 114, 194 );
            this.SkipButton.Name = "SkipButton";
            this.SkipButton.Size = new System.Drawing.Size( 105, 23 );
            this.SkipButton.TabIndex = 2;
            this.SkipButton.Text = "Skip Match";
            this.SkipButton.UseVisualStyleBackColor = true;
            this.SkipButton.Click += new System.EventHandler( this.SkipButton_Click );
            // 
            // UpdateAllButton
            // 
            this.UpdateAllButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateAllButton.Location = new System.Drawing.Point( 225, 194 );
            this.UpdateAllButton.Name = "UpdateAllButton";
            this.UpdateAllButton.Size = new System.Drawing.Size( 105, 23 );
            this.UpdateAllButton.TabIndex = 3;
            this.UpdateAllButton.Text = "Update All Matches";
            this.UpdateAllButton.UseVisualStyleBackColor = true;
            this.UpdateAllButton.Click += new System.EventHandler( this.UpdateAllButton_Click );
            // 
            // SkipAllButton
            // 
            this.SkipAllButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SkipAllButton.Location = new System.Drawing.Point( 336, 194 );
            this.SkipAllButton.Name = "SkipAllButton";
            this.SkipAllButton.Size = new System.Drawing.Size( 105, 23 );
            this.SkipAllButton.TabIndex = 4;
            this.SkipAllButton.Text = "Skip All Matches";
            this.SkipAllButton.UseVisualStyleBackColor = true;
            this.SkipAllButton.Click += new System.EventHandler( this.SkipAllButton_Click );
            // 
            // WindowTitle
            // 
            this.WindowTitle.AutoSize = true;
            this.WindowTitle.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.WindowTitle.Location = new System.Drawing.Point( 4, 5 );
            this.WindowTitle.Name = "WindowTitle";
            this.WindowTitle.Size = new System.Drawing.Size( 438, 20 );
            this.WindowTitle.TabIndex = 4;
            this.WindowTitle.Text = "Current import record  has matching  database record";
            this.WindowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShowImportKeyData
            // 
            this.ShowImportKeyData.Font = new System.Drawing.Font( "Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.ShowImportKeyData.Location = new System.Drawing.Point( 8, 29 );
            this.ShowImportKeyData.Multiline = true;
            this.ShowImportKeyData.Name = "ShowImportKeyData";
            this.ShowImportKeyData.ReadOnly = true;
            this.ShowImportKeyData.Size = new System.Drawing.Size( 430, 160 );
            this.ShowImportKeyData.TabIndex = 0;
            this.ShowImportKeyData.TabStop = false;
            // 
            // ImportMatchDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 446, 225 );
            this.Controls.Add( this.ShowImportKeyData );
            this.Controls.Add( this.WindowTitle );
            this.Controls.Add( this.SkipAllButton );
            this.Controls.Add( this.UpdateAllButton );
            this.Controls.Add( this.SkipButton );
            this.Controls.Add( this.UpdateButton );
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ImportMatchDialogForm";
            this.Text = "ImportMatchDialogForm";
            this.Load += new System.EventHandler( this.ImportMatchDialogForm_Load );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.ImportMatchDialogForm_FormClosed );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button SkipButton;
        private System.Windows.Forms.Button UpdateAllButton;
        private System.Windows.Forms.Button SkipAllButton;
        private System.Windows.Forms.Label WindowTitle;
        private System.Windows.Forms.TextBox ShowImportKeyData;
    }
}