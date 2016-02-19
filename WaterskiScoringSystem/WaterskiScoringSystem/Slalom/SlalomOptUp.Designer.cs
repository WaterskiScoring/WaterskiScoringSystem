namespace WaterskiScoringSystem.Slalom {
    partial class SlalomOptUp {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SlalomOptUp));
            this.label1 = new System.Windows.Forms.Label();
            this.SpeedButton = new System.Windows.Forms.Button();
            this.LineLengthButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.awsaRule10_06Msg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(434, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Do you want to increase the speed or shorten the line length?";
            // 
            // SpeedButton
            // 
            this.SpeedButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SpeedButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeedButton.Location = new System.Drawing.Point(39, 38);
            this.SpeedButton.Name = "SpeedButton";
            this.SpeedButton.Size = new System.Drawing.Size(75, 25);
            this.SpeedButton.TabIndex = 1;
            this.SpeedButton.Text = "Speed";
            this.SpeedButton.UseVisualStyleBackColor = true;
            this.SpeedButton.Click += new System.EventHandler(this.SpeedButton_Click);
            // 
            // LineLengthButton
            // 
            this.LineLengthButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.LineLengthButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineLengthButton.Location = new System.Drawing.Point(174, 38);
            this.LineLengthButton.Name = "LineLengthButton";
            this.LineLengthButton.Size = new System.Drawing.Size(75, 25);
            this.LineLengthButton.TabIndex = 2;
            this.LineLengthButton.Text = "Line Length";
            this.LineLengthButton.UseVisualStyleBackColor = true;
            this.LineLengthButton.Click += new System.EventHandler(this.LineLengthButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButton.Location = new System.Drawing.Point(309, 38);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 25);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // awsaRule10_06Msg
            // 
            this.awsaRule10_06Msg.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.awsaRule10_06Msg.Location = new System.Drawing.Point(14, 73);
            this.awsaRule10_06Msg.Multiline = true;
            this.awsaRule10_06Msg.Name = "awsaRule10_06Msg";
            this.awsaRule10_06Msg.ReadOnly = true;
            this.awsaRule10_06Msg.Size = new System.Drawing.Size(425, 84);
            this.awsaRule10_06Msg.TabIndex = 9;
            this.awsaRule10_06Msg.Text = "As of 2016 AWSA Juniors in a class C or lower tournament \r\nDo not have to go the " +
    "division maximum \r\nTo get  scoring credit for shortening the line \r\nSee Rule 10." +
    "06";
            this.awsaRule10_06Msg.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SlalomOptUp
            // 
            this.AcceptButton = this.SpeedButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 169);
            this.Controls.Add(this.awsaRule10_06Msg);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.LineLengthButton);
            this.Controls.Add(this.SpeedButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SlalomOptUp";
            this.Text = "Slalom Opt Up";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SlalomOptUp_FormClosed);
            this.Load += new System.EventHandler(this.SlalomOptUp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SpeedButton;
        private System.Windows.Forms.Button LineLengthButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.TextBox awsaRule10_06Msg;
    }
}