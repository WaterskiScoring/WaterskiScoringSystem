namespace WaterskiScoringSystem.Tools {
    partial class MetricConv {
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
            this.FeetTextBox = new System.Windows.Forms.TextBox();
            this.FeetLabel = new System.Windows.Forms.Label();
            this.MeterLabel = new System.Windows.Forms.Label();
            this.MeterTextBox = new System.Windows.Forms.TextBox();
            this.FeetToMetersButton = new System.Windows.Forms.Button();
            this.MetersFromFeetLabel = new System.Windows.Forms.Label();
            this.FeetFromMetersLabel = new System.Windows.Forms.Label();
            this.MetersToFeetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FeetTextBox
            // 
            this.FeetTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FeetTextBox.Location = new System.Drawing.Point(63, 12);
            this.FeetTextBox.Name = "FeetTextBox";
            this.FeetTextBox.Size = new System.Drawing.Size(60, 21);
            this.FeetTextBox.TabIndex = 1;
            // 
            // FeetLabel
            // 
            this.FeetLabel.AutoSize = true;
            this.FeetLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FeetLabel.Location = new System.Drawing.Point(13, 13);
            this.FeetLabel.Name = "FeetLabel";
            this.FeetLabel.Size = new System.Drawing.Size(34, 15);
            this.FeetLabel.TabIndex = 0;
            this.FeetLabel.Text = "Feet:";
            // 
            // MeterLabel
            // 
            this.MeterLabel.AutoSize = true;
            this.MeterLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeterLabel.Location = new System.Drawing.Point(13, 40);
            this.MeterLabel.Name = "MeterLabel";
            this.MeterLabel.Size = new System.Drawing.Size(47, 15);
            this.MeterLabel.TabIndex = 0;
            this.MeterLabel.Text = "Meters:";
            // 
            // MeterTextBox
            // 
            this.MeterTextBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MeterTextBox.Location = new System.Drawing.Point(63, 39);
            this.MeterTextBox.Name = "MeterTextBox";
            this.MeterTextBox.Size = new System.Drawing.Size(60, 21);
            this.MeterTextBox.TabIndex = 4;
            // 
            // FeetToMetersButton
            // 
            this.FeetToMetersButton.Location = new System.Drawing.Point(129, 13);
            this.FeetToMetersButton.Name = "FeetToMetersButton";
            this.FeetToMetersButton.Size = new System.Drawing.Size(93, 23);
            this.FeetToMetersButton.TabIndex = 2;
            this.FeetToMetersButton.Text = "Feet to Meters";
            this.FeetToMetersButton.UseVisualStyleBackColor = true;
            this.FeetToMetersButton.Click += new System.EventHandler(this.FeetToMetersButton_Click);
            // 
            // MetersFromFeetLabel
            // 
            this.MetersFromFeetLabel.AutoSize = true;
            this.MetersFromFeetLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MetersFromFeetLabel.Location = new System.Drawing.Point(239, 18);
            this.MetersFromFeetLabel.Name = "MetersFromFeetLabel";
            this.MetersFromFeetLabel.Size = new System.Drawing.Size(31, 15);
            this.MetersFromFeetLabel.TabIndex = 3;
            this.MetersFromFeetLabel.Text = "00.0";
            // 
            // FeetFromMetersLabel
            // 
            this.FeetFromMetersLabel.AutoSize = true;
            this.FeetFromMetersLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FeetFromMetersLabel.Location = new System.Drawing.Point(239, 45);
            this.FeetFromMetersLabel.Name = "FeetFromMetersLabel";
            this.FeetFromMetersLabel.Size = new System.Drawing.Size(31, 15);
            this.FeetFromMetersLabel.TabIndex = 6;
            this.FeetFromMetersLabel.Text = "00.0";
            // 
            // MetersToFeetButton
            // 
            this.MetersToFeetButton.Location = new System.Drawing.Point(129, 40);
            this.MetersToFeetButton.Name = "MetersToFeetButton";
            this.MetersToFeetButton.Size = new System.Drawing.Size(93, 23);
            this.MetersToFeetButton.TabIndex = 5;
            this.MetersToFeetButton.Text = "Meters To Feet";
            this.MetersToFeetButton.UseVisualStyleBackColor = true;
            this.MetersToFeetButton.Click += new System.EventHandler(this.MetersToFeetButton_Click);
            // 
            // MetricConv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 82);
            this.Controls.Add(this.FeetFromMetersLabel);
            this.Controls.Add(this.MetersToFeetButton);
            this.Controls.Add(this.MetersFromFeetLabel);
            this.Controls.Add(this.FeetToMetersButton);
            this.Controls.Add(this.MeterLabel);
            this.Controls.Add(this.MeterTextBox);
            this.Controls.Add(this.FeetLabel);
            this.Controls.Add(this.FeetTextBox);
            this.Name = "MetricConv";
            this.Text = "MetericConv";
            this.Load += new System.EventHandler(this.MetricConv_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox FeetTextBox;
        private System.Windows.Forms.Label FeetLabel;
        private System.Windows.Forms.Label MeterLabel;
        private System.Windows.Forms.TextBox MeterTextBox;
        private System.Windows.Forms.Button FeetToMetersButton;
        private System.Windows.Forms.Label MetersFromFeetLabel;
        private System.Windows.Forms.Label FeetFromMetersLabel;
        private System.Windows.Forms.Button MetersToFeetButton;
    }
}