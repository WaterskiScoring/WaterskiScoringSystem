namespace WaterskiScoringSystem.Common {
    partial class RerideReason {
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
            this.RerideReasonTextbox = new System.Windows.Forms.TextBox();
            this.RerideReasonLabel = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.UpdateWithProtButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // RerideReasonTextbox
            // 
            this.RerideReasonTextbox.Font = new System.Drawing.Font( "Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.RerideReasonTextbox.Location = new System.Drawing.Point( 125, 5 );
            this.RerideReasonTextbox.Multiline = true;
            this.RerideReasonTextbox.Name = "RerideReasonTextbox";
            this.RerideReasonTextbox.Size = new System.Drawing.Size( 400, 45 );
            this.RerideReasonTextbox.TabIndex = 1;
            this.RerideReasonTextbox.Validated += new System.EventHandler( this.RerideReasonTextbox_Validated );
            // 
            // RerideReasonLabel
            // 
            this.RerideReasonLabel.AutoSize = true;
            this.RerideReasonLabel.Font = new System.Drawing.Font( "Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.RerideReasonLabel.Location = new System.Drawing.Point( 5, 5 );
            this.RerideReasonLabel.Name = "RerideReasonLabel";
            this.RerideReasonLabel.Size = new System.Drawing.Size( 116, 16 );
            this.RerideReasonLabel.TabIndex = 0;
            this.RerideReasonLabel.Text = "Reride Reason:";
            this.RerideReasonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // UpdateButton
            // 
            this.UpdateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateButton.Location = new System.Drawing.Point( 125, 57 );
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
            this.CancelButton.Location = new System.Drawing.Point( 450, 57 );
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.CancelButton.TabIndex = 10;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler( this.CancelButton_Click );
            // 
            // UpdateWithProtButton
            // 
            this.UpdateWithProtButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.UpdateWithProtButton.Location = new System.Drawing.Point( 247, 57 );
            this.UpdateWithProtButton.Name = "UpdateWithProtButton";
            this.UpdateWithProtButton.Size = new System.Drawing.Size( 158, 23 );
            this.UpdateWithProtButton.TabIndex = 5;
            this.UpdateWithProtButton.Text = "Update With Protected Score";
            this.UpdateWithProtButton.UseVisualStyleBackColor = true;
            this.UpdateWithProtButton.Click += new System.EventHandler( this.UpdateWithProtButton_Click );
            // 
            // RerideReason
            // 
            this.AcceptButton = this.UpdateButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton;
            this.ClientSize = new System.Drawing.Size( 534, 88 );
            this.Controls.Add( this.UpdateWithProtButton );
            this.Controls.Add( this.CancelButton );
            this.Controls.Add( this.UpdateButton );
            this.Controls.Add( this.RerideReasonLabel );
            this.Controls.Add( this.RerideReasonTextbox );
            this.Name = "RerideReason";
            this.Text = "RerideReason";
            this.Load += new System.EventHandler( this.RerideReason_Load );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.RerideReason_FormClosed );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox RerideReasonTextbox;
        private System.Windows.Forms.Label RerideReasonLabel;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button UpdateWithProtButton;
    }
}