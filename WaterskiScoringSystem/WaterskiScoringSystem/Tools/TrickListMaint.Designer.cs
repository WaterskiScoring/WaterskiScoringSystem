namespace WaterskiScoringSystem.Tools {
    partial class TrickListMaint {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrickListMaint));
			this.DataGridView = new System.Windows.Forms.DataGridView();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RuleCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TrickCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.NumSkis = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StartPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.NumTurns = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RuleNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TypeCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Points = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.navPrint = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.navFilter = new System.Windows.Forms.ToolStripButton();
			this.navSort = new System.Windows.Forms.ToolStripButton();
			this.navSaveItem = new System.Windows.Forms.ToolStripButton();
			this.navDeleteItem = new System.Windows.Forms.ToolStripButton();
			this.navAddNewItem = new System.Windows.Forms.ToolStripButton();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// DataGridView
			// 
			this.DataGridView.AllowUserToAddRows = false;
			this.DataGridView.AllowUserToDeleteRows = false;
			this.DataGridView.AllowUserToResizeColumns = false;
			this.DataGridView.AllowUserToResizeRows = false;
			this.DataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.RuleCode,
            this.TrickCode,
            this.NumSkis,
            this.StartPos,
            this.NumTurns,
            this.RuleNum,
            this.TypeCode,
            this.Points,
            this.Description});
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.DataGridView.DefaultCellStyle = dataGridViewCellStyle4;
			this.DataGridView.Location = new System.Drawing.Point(30, 29);
			this.DataGridView.Name = "DataGridView";
			this.DataGridView.Size = new System.Drawing.Size(704, 392);
			this.DataGridView.TabIndex = 2;
			this.DataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellValidated);
			this.DataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
			// 
			// PK
			// 
			this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PK.HeaderText = "PK";
			this.PK.Name = "PK";
			this.PK.ReadOnly = true;
			this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PK.Visible = false;
			this.PK.Width = 35;
			// 
			// RuleCode
			// 
			this.RuleCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.RuleCode.HeaderText = "Rule Type";
			this.RuleCode.Name = "RuleCode";
			this.RuleCode.ReadOnly = true;
			this.RuleCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.RuleCode.Width = 50;
			// 
			// TrickCode
			// 
			this.TrickCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TrickCode.HeaderText = "Trick Code";
			this.TrickCode.Name = "TrickCode";
			this.TrickCode.ReadOnly = true;
			this.TrickCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.TrickCode.Width = 65;
			// 
			// NumSkis
			// 
			this.NumSkis.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.NumSkis.HeaderText = "Skis";
			this.NumSkis.Name = "NumSkis";
			this.NumSkis.ReadOnly = true;
			this.NumSkis.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.NumSkis.Width = 30;
			// 
			// StartPos
			// 
			this.StartPos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.StartPos.HeaderText = "Start Pos";
			this.StartPos.Name = "StartPos";
			this.StartPos.ReadOnly = true;
			this.StartPos.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.StartPos.Width = 35;
			// 
			// NumTurns
			// 
			this.NumTurns.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.NumTurns.HeaderText = "Num Turns";
			this.NumTurns.Name = "NumTurns";
			this.NumTurns.ReadOnly = true;
			this.NumTurns.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.NumTurns.Width = 35;
			// 
			// RuleNum
			// 
			this.RuleNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.RuleNum.HeaderText = "Rule";
			this.RuleNum.Name = "RuleNum";
			this.RuleNum.ReadOnly = true;
			this.RuleNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.RuleNum.Width = 35;
			// 
			// TypeCode
			// 
			this.TypeCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TypeCode.HeaderText = "Type";
			this.TypeCode.Name = "TypeCode";
			this.TypeCode.ReadOnly = true;
			this.TypeCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.TypeCode.Width = 35;
			// 
			// Points
			// 
			this.Points.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Points.HeaderText = "Points";
			this.Points.Name = "Points";
			this.Points.ReadOnly = true;
			this.Points.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Points.Width = 45;
			// 
			// Description
			// 
			this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Description.HeaderText = "Description";
			this.Description.Name = "Description";
			this.Description.ReadOnly = true;
			this.Description.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Description.Width = 300;
			// 
			// winStatus
			// 
			this.winStatus.Location = new System.Drawing.Point(0, 424);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(746, 22);
			this.winStatus.TabIndex = 4;
			this.winStatus.Text = "statusStrip1";
			// 
			// winStatusMsg
			// 
			this.winStatusMsg.Name = "winStatusMsg";
			this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
			// 
			// navPrint
			// 
			this.navPrint.Image = global::WaterskiScoringSystem.Properties.Resources.Printer_Network;
			this.navPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navPrint.Name = "navPrint";
			this.navPrint.Size = new System.Drawing.Size(36, 35);
			this.navPrint.Text = "Print";
			this.navPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navPrint.Click += new System.EventHandler(this.navPrint_Click);
			// 
			// navExport
			// 
			this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
			this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navExport.Name = "navExport";
			this.navExport.Size = new System.Drawing.Size(44, 35);
			this.navExport.Text = "Export";
			this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navExport.Click += new System.EventHandler(this.navExport_Click);
			// 
			// navFilter
			// 
			this.navFilter.Image = ((System.Drawing.Image)(resources.GetObject("navFilter.Image")));
			this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navFilter.Name = "navFilter";
			this.navFilter.Size = new System.Drawing.Size(37, 35);
			this.navFilter.Text = "Filter";
			this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
			// 
			// navSort
			// 
			this.navSort.Image = ((System.Drawing.Image)(resources.GetObject("navSort.Image")));
			this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navSort.Name = "navSort";
			this.navSort.Size = new System.Drawing.Size(32, 35);
			this.navSort.Text = "Sort";
			this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navSort.Click += new System.EventHandler(this.navSort_Click);
			// 
			// navSaveItem
			// 
			this.navSaveItem.Enabled = false;
			this.navSaveItem.Image = ((System.Drawing.Image)(resources.GetObject("navSaveItem.Image")));
			this.navSaveItem.Name = "navSaveItem";
			this.navSaveItem.Size = new System.Drawing.Size(35, 33);
			this.navSaveItem.Text = "Save";
			this.navSaveItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navSaveItem.Visible = false;
			this.navSaveItem.Click += new System.EventHandler(this.navSaveItem_Click);
			// 
			// navDeleteItem
			// 
			this.navDeleteItem.Enabled = false;
			this.navDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("navDeleteItem.Image")));
			this.navDeleteItem.Name = "navDeleteItem";
			this.navDeleteItem.RightToLeftAutoMirrorImage = true;
			this.navDeleteItem.Size = new System.Drawing.Size(44, 35);
			this.navDeleteItem.Text = "Delete";
			this.navDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navDeleteItem.Visible = false;
			// 
			// navAddNewItem
			// 
			this.navAddNewItem.Enabled = false;
			this.navAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("navAddNewItem.Image")));
			this.navAddNewItem.Name = "navAddNewItem";
			this.navAddNewItem.RightToLeftAutoMirrorImage = true;
			this.navAddNewItem.Size = new System.Drawing.Size(33, 35);
			this.navAddNewItem.Text = "Add";
			this.navAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navAddNewItem.Visible = false;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(238, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(128, 20);
			this.textBox1.TabIndex = 1;
			this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
			this.textBox1.Validated += new System.EventHandler(this.textBox1_Validated);
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(27, 5);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(150, 14);
			this.RowStatusLabel.TabIndex = 0;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			this.RowStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(205, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(396, 6);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Searches trick code and description";
			// 
			// TrickListMaint
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(746, 446);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.winStatus);
			this.Controls.Add(this.DataGridView);
			this.Name = "TrickListMaint";
			this.Text = "Trick List";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrickListMaint_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TrickListMaint_FormClosed);
			this.Load += new System.EventHandler(this.TrickListMaint_Load);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navSaveItem;
        private System.Windows.Forms.ToolStripButton navDeleteItem;
        private System.Windows.Forms.ToolStripButton navAddNewItem;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn RuleCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn TrickCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn NumSkis;
		private System.Windows.Forms.DataGridViewTextBoxColumn StartPos;
		private System.Windows.Forms.DataGridViewTextBoxColumn NumTurns;
		private System.Windows.Forms.DataGridViewTextBoxColumn RuleNum;
		private System.Windows.Forms.DataGridViewTextBoxColumn TypeCode;
		private System.Windows.Forms.DataGridViewTextBoxColumn Points;
		private System.Windows.Forms.DataGridViewTextBoxColumn Description;
	}
}