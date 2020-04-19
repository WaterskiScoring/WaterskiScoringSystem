namespace WaterskiScoringSystem.Common {
    partial class ProgressWindow {
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
            this.processProgressBar = new System.Windows.Forms.ProgressBar();
            this.progressMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // processProgressBar
            // 
            this.processProgressBar.Location = new System.Drawing.Point( 5, 33 );
            this.processProgressBar.Name = "processProgressBar";
            this.processProgressBar.Size = new System.Drawing.Size( 400, 23 );
            this.processProgressBar.TabIndex = 0;
            // 
            // progressMsg
            // 
            this.progressMsg.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.progressMsg.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.progressMsg.Font = new System.Drawing.Font( "Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.progressMsg.Location = new System.Drawing.Point( 5, 5 );
            this.progressMsg.Name = "progressMsg";
            this.progressMsg.Size = new System.Drawing.Size( 400, 23 );
            this.progressMsg.TabIndex = 1;
            this.progressMsg.Text = "Process Status";
            this.progressMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProgressWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 407, 60 );
            this.Controls.Add( this.progressMsg );
            this.Controls.Add( this.processProgressBar );
            this.Name = "ProgressWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressWindow";
            this.Load += new System.EventHandler( this.ProgressWindow_Load );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.ProgressBar processProgressBar;
        private System.Windows.Forms.Label progressMsg;
    }
}