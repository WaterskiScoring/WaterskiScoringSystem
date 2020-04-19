namespace WaterskiScoringSystem.Tools {
    partial class RunOrderClassForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && ( components != null )) {
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
            this.FromClassGroupBox = new System.Windows.Forms.GroupBox();
            this.ToClassGroupBox = new System.Windows.Forms.GroupBox();
            this.EventGroupList = new System.Windows.Forms.ComboBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FromClassGroupBox
            // 
            this.FromClassGroupBox.Location = new System.Drawing.Point(12, 44);
            this.FromClassGroupBox.Name = "FromClassGroupBox";
            this.FromClassGroupBox.Size = new System.Drawing.Size(200, 129);
            this.FromClassGroupBox.TabIndex = 20;
            this.FromClassGroupBox.TabStop = false;
            this.FromClassGroupBox.Text = "From Class";
            // 
            // ToClassGroupBox
            // 
            this.ToClassGroupBox.Location = new System.Drawing.Point(224, 44);
            this.ToClassGroupBox.Name = "ToClassGroupBox";
            this.ToClassGroupBox.Size = new System.Drawing.Size(200, 129);
            this.ToClassGroupBox.TabIndex = 25;
            this.ToClassGroupBox.TabStop = false;
            this.ToClassGroupBox.Text = "To Class";
            // 
            // EventGroupList
            // 
            this.EventGroupList.FormattingEnabled = true;
            this.EventGroupList.Location = new System.Drawing.Point(165, 12);
            this.EventGroupList.Name = "EventGroupList";
            this.EventGroupList.Size = new System.Drawing.Size(121, 21);
            this.EventGroupList.TabIndex = 10;
            this.EventGroupList.SelectedIndexChanged += new System.EventHandler(this.EventGroupList_SelectedIndexChanged);
            // 
            // OKButton
            // 
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(137, 190);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 30;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(224, 190);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 35;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // RunOrderClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 236);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.EventGroupList);
            this.Controls.Add(this.ToClassGroupBox);
            this.Controls.Add(this.FromClassGroupBox);
            this.Name = "RunOrderClassForm";
            this.Text = "Change Skier Class";
            this.Load += new System.EventHandler(this.RunOrderClassForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox FromClassGroupBox;
        private System.Windows.Forms.GroupBox ToClassGroupBox;
        private System.Windows.Forms.ComboBox EventGroupList;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelButton;

    }
}