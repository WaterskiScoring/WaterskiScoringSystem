namespace WaterskiScoringSystem.Tournament {
    partial class TourDivOrder {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TourDivOrder));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.TopNavMenu = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navPrint = new System.Windows.Forms.ToolStripButton();
            this.navSave = new System.Windows.Forms.ToolStripButton();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.jumpButton = new System.Windows.Forms.RadioButton();
            this.trickButton = new System.Windows.Forms.RadioButton();
            this.slalomButton = new System.Windows.Forms.RadioButton();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.TourDivDataGridView = new System.Windows.Forms.DataGridView();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DivName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RunOrder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LoadDataButton = new System.Windows.Forms.Button();
            this.RemoveDataButton = new System.Windows.Forms.Button();
            this.TopNavMenu.SuspendLayout();
            this.winStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TourDivDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TopNavMenu
            // 
            this.TopNavMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navPrint,
            this.navSave});
            this.TopNavMenu.Location = new System.Drawing.Point(0, 0);
            this.TopNavMenu.Name = "TopNavMenu";
            this.TopNavMenu.Size = new System.Drawing.Size(409, 38);
            this.TopNavMenu.TabIndex = 12;
            this.TopNavMenu.Text = "toolStrip1";
            // 
            // navRefresh
            // 
            this.navRefresh.Image = global::WaterskiScoringSystem.Properties.Resources.Terminal;
            this.navRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRefresh.Name = "navRefresh";
            this.navRefresh.Size = new System.Drawing.Size(50, 35);
            this.navRefresh.Text = "Refresh";
            this.navRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRefresh.Click += new System.EventHandler(this.navRefresh_Click);
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
            // navSave
            // 
            this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
            this.navSave.Name = "navSave";
            this.navSave.Size = new System.Drawing.Size(35, 35);
            this.navSave.Text = "Save";
            this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSave.Click += new System.EventHandler(this.navSave_Click);
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(4, 43);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 16;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // jumpButton
            // 
            this.jumpButton.AutoSize = true;
            this.jumpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jumpButton.Location = new System.Drawing.Point(271, 39);
            this.jumpButton.Name = "jumpButton";
            this.jumpButton.Size = new System.Drawing.Size(59, 20);
            this.jumpButton.TabIndex = 15;
            this.jumpButton.TabStop = true;
            this.jumpButton.Text = "Jump";
            this.jumpButton.UseVisualStyleBackColor = true;
            this.jumpButton.CheckedChanged += new System.EventHandler(this.jumpButton_CheckedChanged);
            // 
            // trickButton
            // 
            this.trickButton.AutoSize = true;
            this.trickButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trickButton.Location = new System.Drawing.Point(206, 39);
            this.trickButton.Name = "trickButton";
            this.trickButton.Size = new System.Drawing.Size(56, 20);
            this.trickButton.TabIndex = 14;
            this.trickButton.TabStop = true;
            this.trickButton.Text = "Trick";
            this.trickButton.UseVisualStyleBackColor = true;
            this.trickButton.CheckedChanged += new System.EventHandler(this.trickButton_CheckedChanged);
            // 
            // slalomButton
            // 
            this.slalomButton.AutoSize = true;
            this.slalomButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.slalomButton.Location = new System.Drawing.Point(130, 39);
            this.slalomButton.Name = "slalomButton";
            this.slalomButton.Size = new System.Drawing.Size(68, 20);
            this.slalomButton.TabIndex = 13;
            this.slalomButton.TabStop = true;
            this.slalomButton.Text = "Slalom";
            this.slalomButton.UseVisualStyleBackColor = true;
            this.slalomButton.CheckedChanged += new System.EventHandler(this.slalomButton_CheckedChanged);
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 407);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(409, 22);
            this.winStatus.TabIndex = 17;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // TourDivDataGridView
            // 
            this.TourDivDataGridView.AllowUserToAddRows = false;
            this.TourDivDataGridView.AllowUserToDeleteRows = false;
            this.TourDivDataGridView.AllowUserToResizeColumns = false;
            this.TourDivDataGridView.AllowUserToResizeRows = false;
            this.TourDivDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TourDivDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TourDivDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.TourDivDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TourDivDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SanctionId,
            this.Event,
            this.AgeGroup,
            this.DivName,
            this.RunOrder,
            this.PK,
            this.Updated});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.TourDivDataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.TourDivDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.TourDivDataGridView.Location = new System.Drawing.Point(86, 65);
            this.TourDivDataGridView.Name = "TourDivDataGridView";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.NullValue = null;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TourDivDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.TourDivDataGridView.RowHeadersVisible = false;
            this.TourDivDataGridView.RowHeadersWidth = 31;
            this.TourDivDataGridView.Size = new System.Drawing.Size(248, 328);
            this.TourDivDataGridView.TabIndex = 20;
            this.TourDivDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.TourDivDataGridView_CellEnter);
            this.TourDivDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.TourDivDataGridView_CellValidated);
            this.TourDivDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.TourDivDataGridView_CellValidating);
            this.TourDivDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
            this.TourDivDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.TourDivDataGridView_RowEnter);
            this.TourDivDataGridView.Leave += new System.EventHandler(this.TourDivDataGridView_Leave);
            // 
            // SanctionId
            // 
            this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SanctionId.Visible = false;
            this.SanctionId.Width = 70;
            // 
            // Event
            // 
            this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Event.HeaderText = "Event";
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.Visible = false;
            this.Event.Width = 50;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AgeGroup.DefaultCellStyle = dataGridViewCellStyle2;
            this.AgeGroup.HeaderText = "Div";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.Width = 45;
            // 
            // DivName
            // 
            this.DivName.HeaderText = "Name";
            this.DivName.Name = "DivName";
            this.DivName.ReadOnly = true;
            this.DivName.Width = 125;
            // 
            // RunOrder
            // 
            this.RunOrder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.RunOrder.DefaultCellStyle = dataGridViewCellStyle3;
            this.RunOrder.HeaderText = "Order";
            this.RunOrder.Name = "RunOrder";
            this.RunOrder.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RunOrder.Width = 40;
            // 
            // PK
            // 
            this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            this.PK.ReadOnly = true;
            this.PK.Visible = false;
            this.PK.Width = 50;
            // 
            // Updated
            // 
            this.Updated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Updated.HeaderText = "Updated";
            this.Updated.Name = "Updated";
            this.Updated.ReadOnly = true;
            this.Updated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Updated.Visible = false;
            this.Updated.Width = 35;
            // 
            // LoadDataButton
            // 
            this.LoadDataButton.Location = new System.Drawing.Point(5, 65);
            this.LoadDataButton.Name = "LoadDataButton";
            this.LoadDataButton.Size = new System.Drawing.Size(75, 23);
            this.LoadDataButton.TabIndex = 10;
            this.LoadDataButton.Text = "Load";
            this.LoadDataButton.UseVisualStyleBackColor = true;
            this.LoadDataButton.Click += new System.EventHandler(this.LoadDataButton_Click);
            // 
            // RemoveDataButton
            // 
            this.RemoveDataButton.Location = new System.Drawing.Point(5, 95);
            this.RemoveDataButton.Name = "RemoveDataButton";
            this.RemoveDataButton.Size = new System.Drawing.Size(75, 23);
            this.RemoveDataButton.TabIndex = 11;
            this.RemoveDataButton.Text = "Remove";
            this.RemoveDataButton.UseVisualStyleBackColor = true;
            this.RemoveDataButton.Click += new System.EventHandler(this.RemoveDataButton_Click);
            // 
            // TourDivOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 429);
            this.Controls.Add(this.RemoveDataButton);
            this.Controls.Add(this.LoadDataButton);
            this.Controls.Add(this.TourDivDataGridView);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.jumpButton);
            this.Controls.Add(this.trickButton);
            this.Controls.Add(this.slalomButton);
            this.Controls.Add(this.TopNavMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TourDivOrder";
            this.Text = "TourDivOrder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TourDivOrder_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TourDivOrder_FormClosed);
            this.Load += new System.EventHandler(this.TourDivOrder_Load);
            this.TopNavMenu.ResumeLayout(false);
            this.TopNavMenu.PerformLayout();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TourDivDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip TopNavMenu;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.ToolStripButton navSave;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.RadioButton jumpButton;
        private System.Windows.Forms.RadioButton trickButton;
        private System.Windows.Forms.RadioButton slalomButton;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.DataGridView TourDivDataGridView;
        private System.Windows.Forms.Button LoadDataButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn DivName;
        private System.Windows.Forms.DataGridViewTextBoxColumn RunOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn Updated;
        private System.Windows.Forms.Button RemoveDataButton;
    }
}