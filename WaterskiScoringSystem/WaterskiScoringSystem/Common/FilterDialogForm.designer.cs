namespace WaterskiScoringSystem.Common {
    partial class FilterDialogForm {
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.winMenu = new System.Windows.Forms.MenuStrip();
            this.menuInsert = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.Connector = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Operator = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.FilterValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.winMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(252, 259);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(169, 259);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Connector,
            this.ColumnName,
            this.Operator,
            this.FilterValue});
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView.Location = new System.Drawing.Point(10, 30);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(470, 215);
            this.dataGridView.TabIndex = 4;
            // 
            // winMenu
            // 
            this.winMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuInsert,
            this.menuDelete});
            this.winMenu.Location = new System.Drawing.Point(0, 0);
            this.winMenu.Name = "winMenu";
            this.winMenu.Size = new System.Drawing.Size(506, 24);
            this.winMenu.TabIndex = 7;
            this.winMenu.Text = "Window Menu";
            // 
            // menuInsert
            // 
            this.menuInsert.AutoToolTip = true;
            this.menuInsert.Image = global::WaterskiScoringSystem.Properties.Resources.small_plus;
            this.menuInsert.Name = "menuInsert";
            this.menuInsert.Size = new System.Drawing.Size(64, 20);
            this.menuInsert.Text = "Insert";
            this.menuInsert.ToolTipText = "Insert new line";
            this.menuInsert.Click += new System.EventHandler(this.menuInsert_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Image = global::WaterskiScoringSystem.Properties.Resources.minus_sign;
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(66, 20);
            this.menuDelete.Text = "Delete";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // Connector
            // 
            this.Connector.FalseValue = "OR";
            this.Connector.HeaderText = "And";
            this.Connector.IndeterminateValue = "OR";
            this.Connector.Name = "Connector";
            this.Connector.ToolTipText = "Compound condition connector";
            this.Connector.TrueValue = "AND";
            this.Connector.Width = 32;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnName.HeaderText = "Column to Filter";
            this.ColumnName.MinimumWidth = 125;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ToolTipText = "Column name to filter ";
            this.ColumnName.Width = 125;
            // 
            // Operator
            // 
            this.Operator.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Operator.HeaderText = "Operator";
            this.Operator.MaxDropDownItems = 6;
            this.Operator.MinimumWidth = 175;
            this.Operator.Name = "Operator";
            this.Operator.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Operator.ToolTipText = "Filter operation";
            this.Operator.Width = 175;
            // 
            // FilterValue
            // 
            this.FilterValue.HeaderText = "Value";
            this.FilterValue.MaxInputLength = 128;
            this.FilterValue.MinimumWidth = 100;
            this.FilterValue.Name = "FilterValue";
            // 
            // FilterDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 294);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.winMenu);
            this.Name = "FilterDialogForm";
            this.Text = "FilterDialogForm";
            this.Load += new System.EventHandler(this.FilterDialogForm_Load_1);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.winMenu.ResumeLayout(false);
            this.winMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private string outFilterCommand = null;
        private System.Windows.Forms.DataGridViewColumnCollection inputColumnList;

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.MenuStrip winMenu;
        private System.Windows.Forms.ToolStripMenuItem menuInsert;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Connector;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewComboBoxColumn Operator;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilterValue;
    }
}