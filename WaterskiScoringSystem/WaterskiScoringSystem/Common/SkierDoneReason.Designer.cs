namespace WaterskiScoringSystem.Common {
    partial class SkierDoneReason {
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
            this.ReasonTextbox = new System.Windows.Forms.TextBox();
            this.ReasonLabel = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ReasonTextbox
            // 
            this.ReasonTextbox.Font = new System.Drawing.Font( "Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.ReasonTextbox.Location = new System.Drawing.Point( 71, 5 );
            this.ReasonTextbox.Multiline = true;
            this.ReasonTextbox.Name = "ReasonTextbox";
            this.ReasonTextbox.Size = new System.Drawing.Size( 400, 45 );
            this.ReasonTextbox.TabIndex = 1;
            this.ReasonTextbox.TextChanged += new System.EventHandler( this.ReasonTextbox_Validated );
            // 
            // ReasonLabel
            // 
            this.ReasonLabel.AutoSize = true;
            this.ReasonLabel.Font = new System.Drawing.Font( "Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.ReasonLabel.Location = new System.Drawing.Point( 5, 5 );
            this.ReasonLabel.Name = "ReasonLabel";
            this.ReasonLabel.Size = new System.Drawing.Size( 66, 16 );
            this.ReasonLabel.TabIndex = 0;
            this.ReasonLabel.Text = "Reason:";
            this.ReasonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UpdateButton
            // 
            this.UpdateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateButton.Location = new System.Drawing.Point( 181, 57 );
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size( 75, 23 );
            this.UpdateButton.TabIndex = 2;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler( this.UpdateButton_Click );
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point( 262, 57 );
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.CancelButton.TabIndex = 10;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler( this.CancelButton_Click );
            // 
            // SkierDoneReason
            // 
            this.AcceptButton = this.UpdateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 480, 88 );
            this.Controls.Add( this.CancelButton );
            this.Controls.Add( this.UpdateButton );
            this.Controls.Add( this.ReasonLabel );
            this.Controls.Add( this.ReasonTextbox );
            this.Name = "SkierDoneReason";
            this.Text = "Skier Done Reason";
            this.Load += new System.EventHandler( this.Reason_Load );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.Reason_FormClosed );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReasonTextbox;
        private System.Windows.Forms.Label ReasonLabel;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button CancelButton;
    }
}