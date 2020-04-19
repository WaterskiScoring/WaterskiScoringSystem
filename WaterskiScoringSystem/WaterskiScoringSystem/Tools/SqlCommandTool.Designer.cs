namespace WaterskiScoringSystem.Tools {
    partial class SqlCommandTool {
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.SqlcommandTextBox = new System.Windows.Forms.TextBox();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.ExecButton = new System.Windows.Forms.Button();
            this.SqlCommandLabel = new System.Windows.Forms.Label();
            this.ExportButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(5, 135);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(587, 364);
            this.dataGridView.TabIndex = 5;
            // 
            // SqlcommandTextBox
            // 
            this.SqlcommandTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SqlcommandTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SqlcommandTextBox.Location = new System.Drawing.Point(116, 10);
            this.SqlcommandTextBox.Multiline = true;
            this.SqlcommandTextBox.Name = "SqlcommandTextBox";
            this.SqlcommandTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.SqlcommandTextBox.Size = new System.Drawing.Size(476, 76);
            this.SqlcommandTextBox.TabIndex = 1;
            // 
            // MessageLabel
            // 
            this.MessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.BackColor = System.Drawing.SystemColors.Info;
            this.MessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MessageLabel.Location = new System.Drawing.Point(5, 95);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(2, 15);
            this.MessageLabel.TabIndex = 4;
            // 
            // ExecButton
            // 
            this.ExecButton.Location = new System.Drawing.Point(18, 35);
            this.ExecButton.Name = "ExecButton";
            this.ExecButton.Size = new System.Drawing.Size(75, 23);
            this.ExecButton.TabIndex = 2;
            this.ExecButton.Text = "Run";
            this.ExecButton.UseVisualStyleBackColor = true;
            this.ExecButton.Click += new System.EventHandler(this.ExecButton_Click);
            // 
            // SqlCommandLabel
            // 
            this.SqlCommandLabel.AutoSize = true;
            this.SqlCommandLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SqlCommandLabel.Location = new System.Drawing.Point(5, 13);
            this.SqlCommandLabel.Name = "SqlCommandLabel";
            this.SqlCommandLabel.Size = new System.Drawing.Size(110, 16);
            this.SqlCommandLabel.TabIndex = 0;
            this.SqlCommandLabel.Text = "SQL Command";
            // 
            // ExportButton
            // 
            this.ExportButton.Location = new System.Drawing.Point(18, 63);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(75, 23);
            this.ExportButton.TabIndex = 3;
            this.ExportButton.Text = "Export";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // SqlCommandTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 501);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.SqlCommandLabel);
            this.Controls.Add(this.ExecButton);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.SqlcommandTextBox);
            this.Controls.Add(this.dataGridView);
            this.Name = "SqlCommandTool";
            this.Text = "Sql Command Tool";
            this.Load += new System.EventHandler(this.SqlCommandTool_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TextBox SqlcommandTextBox;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button ExecButton;
        private System.Windows.Forms.Label SqlCommandLabel;
        private System.Windows.Forms.Button ExportButton;
    }
}