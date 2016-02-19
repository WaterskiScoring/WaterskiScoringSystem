namespace WaterskiScoringSystem.Tournament {
    partial class Registration {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing ) {
            if (disposing && (components != null)) {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Registration));
            this.tourRegDataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ReadyToSki = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SlalomReg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrickReg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.JumpReg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SlalomGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrickGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntryDue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EntryPaid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentMethod = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Weight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpHeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrickBoat = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AwsaMbrshpPaymt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AwsaMbrshpComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.navMainMenu = new System.Windows.Forms.ToolStrip();
            this.navRefresh = new System.Windows.Forms.ToolStripButton();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.navSort = new System.Windows.Forms.ToolStripButton();
            this.navSave = new System.Windows.Forms.ToolStripButton();
            this.navAdd = new System.Windows.Forms.ToolStripButton();
            this.navEdit = new System.Windows.Forms.ToolStripButton();
            this.navRemove = new System.Windows.Forms.ToolStripButton();
            this.navSaveAs = new System.Windows.Forms.ToolStripButton();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.SlalomRegCount = new System.Windows.Forms.Label();
            this.TrickRegCount = new System.Windows.Forms.Label();
            this.JumpRegCount = new System.Windows.Forms.Label();
            this.RegCountLabel = new System.Windows.Forms.Label();
            this.RegCountSlalomLabel = new System.Windows.Forms.Label();
            this.RegCountTrickLabel = new System.Windows.Forms.Label();
            this.RegCountJumpLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tourRegDataGridView)).BeginInit();
            this.winStatus.SuspendLayout();
            this.navMainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tourRegDataGridView
            // 
            this.tourRegDataGridView.AllowUserToAddRows = false;
            this.tourRegDataGridView.AllowUserToDeleteRows = false;
            this.tourRegDataGridView.AllowUserToResizeColumns = false;
            this.tourRegDataGridView.AllowUserToResizeRows = false;
            this.tourRegDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tourRegDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tourRegDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tourRegDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.SkierName,
            this.AgeGroup,
            this.ReadyToSki,
            this.SlalomReg,
            this.TrickReg,
            this.JumpReg,
            this.SlalomGroup,
            this.TrickGroup,
            this.JumpGroup,
            this.EntryDue,
            this.EntryPaid,
            this.PaymentMethod,
            this.Weight,
            this.JumpHeight,
            this.TrickBoat,
            this.AwsaMbrshpPaymt,
            this.AwsaMbrshpComment,
            this.Notes,
            this.PK,
            this.SanctionId,
            this.Updated});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tourRegDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.tourRegDataGridView.Location = new System.Drawing.Point(5, 55);
            this.tourRegDataGridView.Name = "tourRegDataGridView";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tourRegDataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.tourRegDataGridView.RowHeadersWidth = 31;
            this.tourRegDataGridView.Size = new System.Drawing.Size(1073, 408);
            this.tourRegDataGridView.TabIndex = 1;
            this.tourRegDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tourRegDataGridView_CellContentClick);
            this.tourRegDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.tourRegDataGridView_CellContentDoubleClick);
            this.tourRegDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.tourRegDataGridView_CellEnter);
            this.tourRegDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.tourRegDataGridView_CellValidated);
            this.tourRegDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.tourRegDataGridView_CellValidating);
            this.tourRegDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_DataError);
            this.tourRegDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.tourRegDataGridView_RowEnter);
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberId.Frozen = true;
            this.MemberId.HeaderText = "Member Id";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.Width = 75;
            // 
            // SkierName
            // 
            this.SkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkierName.DefaultCellStyle = dataGridViewCellStyle2;
            this.SkierName.Frozen = true;
            this.SkierName.HeaderText = "Skier Name";
            this.SkierName.Name = "SkierName";
            this.SkierName.ReadOnly = true;
            this.SkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkierName.Width = 79;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AgeGroup.DefaultCellStyle = dataGridViewCellStyle3;
            this.AgeGroup.HeaderText = "Age Group";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.Width = 40;
            // 
            // ReadyToSki
            // 
            this.ReadyToSki.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.NullValue = false;
            this.ReadyToSki.DefaultCellStyle = dataGridViewCellStyle4;
            this.ReadyToSki.FalseValue = "N";
            this.ReadyToSki.HeaderText = "Ready Ski?";
            this.ReadyToSki.Name = "ReadyToSki";
            this.ReadyToSki.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ReadyToSki.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ReadyToSki.TrueValue = "Y";
            this.ReadyToSki.Width = 40;
            // 
            // SlalomReg
            // 
            this.SlalomReg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomReg.FalseValue = "N";
            this.SlalomReg.HeaderText = "Slalom";
            this.SlalomReg.Name = "SlalomReg";
            this.SlalomReg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomReg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SlalomReg.TrueValue = "Y";
            this.SlalomReg.Width = 40;
            // 
            // TrickReg
            // 
            this.TrickReg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickReg.FalseValue = "N";
            this.TrickReg.HeaderText = "Trick";
            this.TrickReg.Name = "TrickReg";
            this.TrickReg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickReg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.TrickReg.TrueValue = "Y";
            this.TrickReg.Width = 40;
            // 
            // JumpReg
            // 
            this.JumpReg.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpReg.FalseValue = "N";
            this.JumpReg.HeaderText = "Jump";
            this.JumpReg.Name = "JumpReg";
            this.JumpReg.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpReg.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.JumpReg.TrueValue = "Y";
            this.JumpReg.Width = 40;
            // 
            // SlalomGroup
            // 
            this.SlalomGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomGroup.HeaderText = "Slalom Group";
            this.SlalomGroup.Name = "SlalomGroup";
            this.SlalomGroup.ReadOnly = true;
            this.SlalomGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomGroup.Width = 40;
            // 
            // TrickGroup
            // 
            this.TrickGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickGroup.HeaderText = "Trick Group";
            this.TrickGroup.Name = "TrickGroup";
            this.TrickGroup.ReadOnly = true;
            this.TrickGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickGroup.Width = 40;
            // 
            // JumpGroup
            // 
            this.JumpGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpGroup.HeaderText = "Jump Group";
            this.JumpGroup.Name = "JumpGroup";
            this.JumpGroup.ReadOnly = true;
            this.JumpGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpGroup.Width = 40;
            // 
            // EntryDue
            // 
            this.EntryDue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "C2";
            dataGridViewCellStyle5.NullValue = null;
            this.EntryDue.DefaultCellStyle = dataGridViewCellStyle5;
            this.EntryDue.HeaderText = "Fee Due";
            this.EntryDue.Name = "EntryDue";
            this.EntryDue.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EntryDue.Width = 45;
            // 
            // EntryPaid
            // 
            this.EntryPaid.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "C2";
            dataGridViewCellStyle6.NullValue = null;
            this.EntryPaid.DefaultCellStyle = dataGridViewCellStyle6;
            this.EntryPaid.HeaderText = "Amt Paid";
            this.EntryPaid.Name = "EntryPaid";
            this.EntryPaid.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EntryPaid.Width = 45;
            // 
            // PaymentMethod
            // 
            this.PaymentMethod.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PaymentMethod.HeaderText = "Payment Method";
            this.PaymentMethod.Name = "PaymentMethod";
            this.PaymentMethod.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PaymentMethod.Width = 60;
            // 
            // Weight
            // 
            this.Weight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N0";
            dataGridViewCellStyle7.NullValue = null;
            this.Weight.DefaultCellStyle = dataGridViewCellStyle7;
            this.Weight.HeaderText = "Weight";
            this.Weight.Name = "Weight";
            this.Weight.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Weight.Width = 40;
            // 
            // JumpHeight
            // 
            this.JumpHeight.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.Format = "N1";
            dataGridViewCellStyle8.NullValue = null;
            this.JumpHeight.DefaultCellStyle = dataGridViewCellStyle8;
            this.JumpHeight.HeaderText = "Jump Hgt";
            this.JumpHeight.Name = "JumpHeight";
            this.JumpHeight.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpHeight.Width = 35;
            // 
            // TrickBoat
            // 
            this.TrickBoat.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickBoat.HeaderText = "Trick Boat";
            this.TrickBoat.Name = "TrickBoat";
            this.TrickBoat.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickBoat.Width = 50;
            // 
            // AwsaMbrshpPaymt
            // 
            this.AwsaMbrshpPaymt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AwsaMbrshpPaymt.HeaderText = "Fed Fees";
            this.AwsaMbrshpPaymt.Name = "AwsaMbrshpPaymt";
            this.AwsaMbrshpPaymt.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AwsaMbrshpPaymt.Width = 50;
            // 
            // AwsaMbrshpComment
            // 
            this.AwsaMbrshpComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AwsaMbrshpComment.HeaderText = "Fed Membership Comments";
            this.AwsaMbrshpComment.Name = "AwsaMbrshpComment";
            this.AwsaMbrshpComment.Width = 120;
            // 
            // Notes
            // 
            this.Notes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Notes.HeaderText = "Notes";
            this.Notes.Name = "Notes";
            this.Notes.Width = 120;
            // 
            // PK
            // 
            this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            this.PK.ReadOnly = true;
            this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PK.Visible = false;
            this.PK.Width = 50;
            // 
            // SanctionId
            // 
            this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.Visible = false;
            // 
            // Updated
            // 
            this.Updated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Updated.HeaderText = "Updated";
            this.Updated.Name = "Updated";
            this.Updated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Updated.Visible = false;
            this.Updated.Width = 50;
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
            this.winStatus.Location = new System.Drawing.Point(0, 466);
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size(1090, 22);
            this.winStatus.TabIndex = 4;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
            // 
            // navMainMenu
            // 
            this.navMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navExport,
            this.navFilter,
            this.navSort,
            this.navSave,
            this.navAdd,
            this.navEdit,
            this.navRemove,
            this.navSaveAs});
            this.navMainMenu.Location = new System.Drawing.Point(0, 0);
            this.navMainMenu.Name = "navMainMenu";
            this.navMainMenu.Size = new System.Drawing.Size(1090, 36);
            this.navMainMenu.TabIndex = 5;
            this.navMainMenu.Text = "toolStrip1";
            // 
            // navRefresh
            // 
            this.navRefresh.Image = global::WaterskiScoringSystem.Properties.Resources.Terminal;
            this.navRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRefresh.Name = "navRefresh";
            this.navRefresh.Size = new System.Drawing.Size(49, 33);
            this.navRefresh.Text = "Refresh";
            this.navRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRefresh.Click += new System.EventHandler(this.navRefresh_Click);
            // 
            // navExport
            // 
            this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size(43, 33);
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navExport.ToolTipText = "Export visible data to a tab delimited text file";
            this.navExport.Click += new System.EventHandler(this.navExport_Click);
            // 
            // navFilter
            // 
            this.navFilter.Image = ((System.Drawing.Image)(resources.GetObject("navFilter.Image")));
            this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFilter.Name = "navFilter";
            this.navFilter.Size = new System.Drawing.Size(35, 33);
            this.navFilter.Text = "Filter";
            this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navFilter.Click += new System.EventHandler(this.navFilter_Click);
            // 
            // navSort
            // 
            this.navSort.Image = ((System.Drawing.Image)(resources.GetObject("navSort.Image")));
            this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSort.Name = "navSort";
            this.navSort.Size = new System.Drawing.Size(31, 33);
            this.navSort.Text = "Sort";
            this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSort.Click += new System.EventHandler(this.navSort_Click);
            // 
            // navSave
            // 
            this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
            this.navSave.Name = "navSave";
            this.navSave.Size = new System.Drawing.Size(35, 33);
            this.navSave.Text = "Save";
            this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSave.Click += new System.EventHandler(this.navSave_Click);
            // 
            // navAdd
            // 
            this.navAdd.Image = global::WaterskiScoringSystem.Properties.Resources.small_plus;
            this.navAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navAdd.Name = "navAdd";
            this.navAdd.Size = new System.Drawing.Size(30, 33);
            this.navAdd.Text = "Add";
            this.navAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navAdd.Click += new System.EventHandler(this.navInsert_Click);
            // 
            // navEdit
            // 
            this.navEdit.Image = ((System.Drawing.Image)(resources.GetObject("navEdit.Image")));
            this.navEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navEdit.Name = "navEdit";
            this.navEdit.Size = new System.Drawing.Size(29, 33);
            this.navEdit.Text = "Edit";
            this.navEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navEdit.Click += new System.EventHandler(this.navEdit_Click);
            // 
            // navRemove
            // 
            this.navRemove.Image = global::WaterskiScoringSystem.Properties.Resources.minus_sign;
            this.navRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navRemove.Name = "navRemove";
            this.navRemove.Size = new System.Drawing.Size(50, 33);
            this.navRemove.Text = "Remove";
            this.navRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navRemove.Click += new System.EventHandler(this.navRemove_Click);
            // 
            // navSaveAs
            // 
            this.navSaveAs.Image = global::WaterskiScoringSystem.Properties.Resources.uLauncher;
            this.navSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSaveAs.Name = "navSaveAs";
            this.navSaveAs.Size = new System.Drawing.Size(47, 33);
            this.navSaveAs.Text = "SaveAs";
            this.navSaveAs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSaveAs.ToolTipText = "Save As";
            this.navSaveAs.Click += new System.EventHandler(this.navSaveAs_Click);
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RowStatusLabel.Location = new System.Drawing.Point(8, 38);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
            this.RowStatusLabel.TabIndex = 6;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // SlalomRegCount
            // 
            this.SlalomRegCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SlalomRegCount.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlalomRegCount.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.SlalomRegCount.Location = new System.Drawing.Point(374, 38);
            this.SlalomRegCount.Name = "SlalomRegCount";
            this.SlalomRegCount.Size = new System.Drawing.Size(40, 14);
            this.SlalomRegCount.TabIndex = 0;
            this.SlalomRegCount.Text = "Slalom";
            this.SlalomRegCount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // TrickRegCount
            // 
            this.TrickRegCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.TrickRegCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrickRegCount.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.TrickRegCount.Location = new System.Drawing.Point(481, 38);
            this.TrickRegCount.Name = "TrickRegCount";
            this.TrickRegCount.Size = new System.Drawing.Size(40, 14);
            this.TrickRegCount.TabIndex = 0;
            this.TrickRegCount.Text = "Trick";
            this.TrickRegCount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // JumpRegCount
            // 
            this.JumpRegCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.JumpRegCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.JumpRegCount.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.JumpRegCount.Location = new System.Drawing.Point(591, 38);
            this.JumpRegCount.Name = "JumpRegCount";
            this.JumpRegCount.Size = new System.Drawing.Size(40, 14);
            this.JumpRegCount.TabIndex = 0;
            this.JumpRegCount.Text = "Jump";
            this.JumpRegCount.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // RegCountLabel
            // 
            this.RegCountLabel.AutoSize = true;
            this.RegCountLabel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegCountLabel.Location = new System.Drawing.Point(231, 39);
            this.RegCountLabel.Name = "RegCountLabel";
            this.RegCountLabel.Size = new System.Drawing.Size(97, 13);
            this.RegCountLabel.TabIndex = 0;
            this.RegCountLabel.Text = "Skier Counts: ";
            this.RegCountLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // RegCountSlalomLabel
            // 
            this.RegCountSlalomLabel.AutoSize = true;
            this.RegCountSlalomLabel.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegCountSlalomLabel.Location = new System.Drawing.Point(333, 38);
            this.RegCountSlalomLabel.Name = "RegCountSlalomLabel";
            this.RegCountSlalomLabel.Size = new System.Drawing.Size(42, 15);
            this.RegCountSlalomLabel.TabIndex = 0;
            this.RegCountSlalomLabel.Text = "Slalom:";
            this.RegCountSlalomLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RegCountTrickLabel
            // 
            this.RegCountTrickLabel.AutoSize = true;
            this.RegCountTrickLabel.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegCountTrickLabel.Location = new System.Drawing.Point(451, 38);
            this.RegCountTrickLabel.Name = "RegCountTrickLabel";
            this.RegCountTrickLabel.Size = new System.Drawing.Size(32, 15);
            this.RegCountTrickLabel.TabIndex = 0;
            this.RegCountTrickLabel.Text = "Trick:";
            this.RegCountTrickLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // RegCountJumpLabel
            // 
            this.RegCountJumpLabel.AutoSize = true;
            this.RegCountJumpLabel.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegCountJumpLabel.Location = new System.Drawing.Point(559, 38);
            this.RegCountJumpLabel.Name = "RegCountJumpLabel";
            this.RegCountJumpLabel.Size = new System.Drawing.Size(36, 15);
            this.RegCountJumpLabel.TabIndex = 0;
            this.RegCountJumpLabel.Text = "Jump:";
            this.RegCountJumpLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // Registration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1090, 488);
            this.Controls.Add(this.RegCountJumpLabel);
            this.Controls.Add(this.RegCountTrickLabel);
            this.Controls.Add(this.RegCountSlalomLabel);
            this.Controls.Add(this.RegCountLabel);
            this.Controls.Add(this.JumpRegCount);
            this.Controls.Add(this.TrickRegCount);
            this.Controls.Add(this.SlalomRegCount);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.navMainMenu);
            this.Controls.Add(this.winStatus);
            this.Controls.Add(this.tourRegDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Registration";
            this.Text = "Registration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Registration_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Registration_FormClosed);
            this.Load += new System.EventHandler(this.Registration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tourRegDataGridView)).EndInit();
            this.winStatus.ResumeLayout(false);
            this.winStatus.PerformLayout();
            this.navMainMenu.ResumeLayout(false);
            this.navMainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView tourRegDataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierFullName;
        private System.Windows.Forms.ToolStrip navMainMenu;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSave;
        private System.Windows.Forms.ToolStripButton navEdit;
        private System.Windows.Forms.ToolStripButton navRemove;
        private System.Windows.Forms.ToolStripButton navAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ReadyToSki;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SlalomReg;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TrickReg;
        private System.Windows.Forms.DataGridViewCheckBoxColumn JumpReg;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlalomGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrickGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryDue;
        private System.Windows.Forms.DataGridViewTextBoxColumn EntryPaid;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentMethod;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weight;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpHeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrickBoat;
        private System.Windows.Forms.DataGridViewTextBoxColumn AwsaMbrshpPaymt;
        private System.Windows.Forms.DataGridViewTextBoxColumn AwsaMbrshpComment;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Updated;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.ToolStripButton navSaveAs;
        private System.Windows.Forms.Label SlalomRegCount;
        private System.Windows.Forms.Label TrickRegCount;
        private System.Windows.Forms.Label JumpRegCount;
        private System.Windows.Forms.Label RegCountLabel;
        private System.Windows.Forms.Label RegCountSlalomLabel;
        private System.Windows.Forms.Label RegCountTrickLabel;
        private System.Windows.Forms.Label RegCountJumpLabel;
    }
}