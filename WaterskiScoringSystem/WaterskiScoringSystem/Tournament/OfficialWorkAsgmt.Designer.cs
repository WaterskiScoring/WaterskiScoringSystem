namespace WaterskiScoringSystem.Tournament {
    partial class OfficialWorkAsgmt {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OfficialWorkAsgmt));
			this.activeLabel = new System.Windows.Forms.Label();
			this.officialWorkAsgmtDataGridView = new System.Windows.Forms.DataGridView();
			this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.WorkAsgmt = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.MemberName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StartTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EndTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EndTimeStamp = new System.Windows.Forms.DataGridViewButtonColumn();
			this.Updated = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.winStatus = new System.Windows.Forms.StatusStrip();
			this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
			this.listTourMemberDataGridView = new System.Windows.Forms.DataGridView();
			this.labelMemberSelect = new System.Windows.Forms.Label();
			this.eventGroupBox = new System.Windows.Forms.GroupBox();
			this.EventButtonAll = new System.Windows.Forms.RadioButton();
			this.EventButtonJump = new System.Windows.Forms.RadioButton();
			this.EventButtonTrick = new System.Windows.Forms.RadioButton();
			this.EventButtonSlalom = new System.Windows.Forms.RadioButton();
			this.EventGroupList = new System.Windows.Forms.ComboBox();
			this.topNavBar = new System.Windows.Forms.ToolStrip();
			this.navRefresh = new System.Windows.Forms.ToolStripButton();
			this.navPrint = new System.Windows.Forms.ToolStripButton();
			this.navExport = new System.Windows.Forms.ToolStripButton();
			this.navFilter = new System.Windows.Forms.ToolStripButton();
			this.navSort = new System.Windows.Forms.ToolStripButton();
			this.navSave = new System.Windows.Forms.ToolStripButton();
			this.navAddNewItem = new System.Windows.Forms.ToolStripButton();
			this.navDeleteItem = new System.Windows.Forms.ToolStripButton();
			this.navCopyItem = new System.Windows.Forms.ToolStripButton();
			this.navTemplateButton = new System.Windows.Forms.ToolStripButton();
			this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
			this.labelMemberQuickFind = new System.Windows.Forms.Label();
			this.EnterKeyLabel = new System.Windows.Forms.Label();
			this.EnterKeyLabel2 = new System.Windows.Forms.Label();
			this.roundActiveSelect = new WaterskiScoringSystem.Common.RoundSelect();
			((System.ComponentModel.ISupportInitialize)(this.officialWorkAsgmtDataGridView)).BeginInit();
			this.winStatus.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listTourMemberDataGridView)).BeginInit();
			this.eventGroupBox.SuspendLayout();
			this.topNavBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// activeLabel
			// 
			this.activeLabel.CausesValidation = false;
			this.activeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.activeLabel.Location = new System.Drawing.Point(53, 39);
			this.activeLabel.Margin = new System.Windows.Forms.Padding(0);
			this.activeLabel.Name = "activeLabel";
			this.activeLabel.Size = new System.Drawing.Size(89, 13);
			this.activeLabel.TabIndex = 23;
			this.activeLabel.Text = "Active Round:";
			this.activeLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// officialWorkAsgmtDataGridView
			// 
			this.officialWorkAsgmtDataGridView.AllowUserToAddRows = false;
			this.officialWorkAsgmtDataGridView.AllowUserToDeleteRows = false;
			this.officialWorkAsgmtDataGridView.AllowUserToResizeColumns = false;
			this.officialWorkAsgmtDataGridView.AllowUserToResizeRows = false;
			this.officialWorkAsgmtDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.officialWorkAsgmtDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.officialWorkAsgmtDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.officialWorkAsgmtDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.SanctionId,
            this.MemberId,
            this.Event,
            this.EventGroup,
            this.Round,
            this.WorkAsgmt,
            this.MemberName,
            this.StartTime,
            this.EndTime,
            this.Notes,
            this.EndTimeStamp,
            this.Updated});
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle7.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.officialWorkAsgmtDataGridView.DefaultCellStyle = dataGridViewCellStyle7;
			this.officialWorkAsgmtDataGridView.Location = new System.Drawing.Point(5, 102);
			this.officialWorkAsgmtDataGridView.MultiSelect = false;
			this.officialWorkAsgmtDataGridView.Name = "officialWorkAsgmtDataGridView";
			this.officialWorkAsgmtDataGridView.RowHeadersVisible = false;
			this.officialWorkAsgmtDataGridView.Size = new System.Drawing.Size(793, 307);
			this.officialWorkAsgmtDataGridView.TabIndex = 1;
			this.officialWorkAsgmtDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.officialWorkAsgmtDataGridView_CellContentClick);
			this.officialWorkAsgmtDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.officialWorkAsgmtDataGridView_CellEnter);
			this.officialWorkAsgmtDataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.officialWorkAsgmtDataGridView_CellValidated);
			this.officialWorkAsgmtDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.officialWorkAsgmtDataGridView_CellValueChanged);
			this.officialWorkAsgmtDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
			this.officialWorkAsgmtDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.officialWorkAsgmtDataGridView_RowEnter);
			this.officialWorkAsgmtDataGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.officialWorkAsgmtDataGridView_KeyUp);
			// 
			// PK
			// 
			this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PK.HeaderText = "PK";
			this.PK.Name = "PK";
			this.PK.ReadOnly = true;
			this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.PK.Visible = false;
			this.PK.Width = 40;
			// 
			// SanctionId
			// 
			this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle2.NullValue = " ";
			this.SanctionId.DefaultCellStyle = dataGridViewCellStyle2;
			this.SanctionId.HeaderText = "SanctionId";
			this.SanctionId.Name = "SanctionId";
			this.SanctionId.ReadOnly = true;
			this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SanctionId.Visible = false;
			this.SanctionId.Width = 55;
			// 
			// MemberId
			// 
			this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MemberId.HeaderText = "MemberId";
			this.MemberId.Name = "MemberId";
			this.MemberId.ReadOnly = true;
			this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.MemberId.Visible = false;
			this.MemberId.Width = 40;
			// 
			// Event
			// 
			this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Event.HeaderText = "Event";
			this.Event.Name = "Event";
			this.Event.ReadOnly = true;
			this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Event.Width = 55;
			// 
			// EventGroup
			// 
			this.EventGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.EventGroup.HeaderText = "Event Group";
			this.EventGroup.Name = "EventGroup";
			this.EventGroup.ReadOnly = true;
			this.EventGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.EventGroup.Width = 50;
			// 
			// Round
			// 
			this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Round.HeaderText = "Rd";
			this.Round.Name = "Round";
			this.Round.ReadOnly = true;
			this.Round.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Round.Width = 30;
			// 
			// WorkAsgmt
			// 
			this.WorkAsgmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WorkAsgmt.DefaultCellStyle = dataGridViewCellStyle3;
			this.WorkAsgmt.HeaderText = "Assignment";
			this.WorkAsgmt.Name = "WorkAsgmt";
			this.WorkAsgmt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.WorkAsgmt.Width = 115;
			// 
			// MemberName
			// 
			this.MemberName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle4.NullValue = " ";
			this.MemberName.DefaultCellStyle = dataGridViewCellStyle4;
			this.MemberName.HeaderText = "Official";
			this.MemberName.Name = "MemberName";
			this.MemberName.Width = 115;
			// 
			// StartTime
			// 
			this.StartTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle5.Format = "MM/dd/yy hh:mm tt";
			dataGridViewCellStyle5.NullValue = null;
			this.StartTime.DefaultCellStyle = dataGridViewCellStyle5;
			this.StartTime.HeaderText = "Start Time";
			this.StartTime.Name = "StartTime";
			this.StartTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.StartTime.Width = 90;
			// 
			// EndTime
			// 
			this.EndTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle6.Format = "MM/dd/yy hh:mm tt";
			this.EndTime.DefaultCellStyle = dataGridViewCellStyle6;
			this.EndTime.HeaderText = "End Time";
			this.EndTime.Name = "EndTime";
			this.EndTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.EndTime.Width = 90;
			// 
			// Notes
			// 
			this.Notes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Notes.HeaderText = "Notes";
			this.Notes.Name = "Notes";
			this.Notes.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Notes.Width = 115;
			// 
			// EndTimeStamp
			// 
			this.EndTimeStamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.EndTimeStamp.HeaderText = "Set End Time";
			this.EndTimeStamp.Name = "EndTimeStamp";
			this.EndTimeStamp.ReadOnly = true;
			this.EndTimeStamp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.EndTimeStamp.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// Updated
			// 
			this.Updated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Updated.HeaderText = "Updated";
			this.Updated.MaxInputLength = 1;
			this.Updated.Name = "Updated";
			this.Updated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Updated.Visible = false;
			this.Updated.Width = 25;
			// 
			// winStatus
			// 
			this.winStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg});
			this.winStatus.Location = new System.Drawing.Point(0, 413);
			this.winStatus.Name = "winStatus";
			this.winStatus.Size = new System.Drawing.Size(1087, 22);
			this.winStatus.TabIndex = 2;
			this.winStatus.Text = "statusStrip1";
			// 
			// winStatusMsg
			// 
			this.winStatusMsg.Name = "winStatusMsg";
			this.winStatusMsg.Size = new System.Drawing.Size(0, 17);
			// 
			// listTourMemberDataGridView
			// 
			this.listTourMemberDataGridView.AllowUserToAddRows = false;
			this.listTourMemberDataGridView.AllowUserToDeleteRows = false;
			this.listTourMemberDataGridView.AllowUserToResizeColumns = false;
			this.listTourMemberDataGridView.AllowUserToResizeRows = false;
			this.listTourMemberDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listTourMemberDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.listTourMemberDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle8.Font = new System.Drawing.Font("Arial Narrow", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.listTourMemberDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
			this.listTourMemberDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle9.BackColor = System.Drawing.Color.LightGoldenrodYellow;
			dataGridViewCellStyle9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.ControlDarkDark;
			dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.listTourMemberDataGridView.DefaultCellStyle = dataGridViewCellStyle9;
			this.listTourMemberDataGridView.Location = new System.Drawing.Point(343, 141);
			this.listTourMemberDataGridView.Name = "listTourMemberDataGridView";
			this.listTourMemberDataGridView.Size = new System.Drawing.Size(737, 268);
			this.listTourMemberDataGridView.TabIndex = 0;
			this.listTourMemberDataGridView.TabStop = false;
			this.listTourMemberDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.listTourMemberDataGridView_CellContentDoubleClick);
			this.listTourMemberDataGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listTourMemberDataGridView_KeyUp);
			// 
			// labelMemberSelect
			// 
			this.labelMemberSelect.AutoSize = true;
			this.labelMemberSelect.BackColor = System.Drawing.Color.LightGoldenrodYellow;
			this.labelMemberSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.labelMemberSelect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMemberSelect.Location = new System.Drawing.Point(344, 127);
			this.labelMemberSelect.Name = "labelMemberSelect";
			this.labelMemberSelect.Size = new System.Drawing.Size(300, 14);
			this.labelMemberSelect.TabIndex = 0;
			this.labelMemberSelect.Text = "Double Click or hit enter to assign member as official";
			// 
			// eventGroupBox
			// 
			this.eventGroupBox.Controls.Add(this.EventButtonAll);
			this.eventGroupBox.Controls.Add(this.EventButtonJump);
			this.eventGroupBox.Controls.Add(this.EventButtonTrick);
			this.eventGroupBox.Controls.Add(this.EventButtonSlalom);
			this.eventGroupBox.Location = new System.Drawing.Point(195, 44);
			this.eventGroupBox.Name = "eventGroupBox";
			this.eventGroupBox.Size = new System.Drawing.Size(220, 40);
			this.eventGroupBox.TabIndex = 1;
			this.eventGroupBox.TabStop = false;
			this.eventGroupBox.Text = "Event";
			// 
			// EventButtonAll
			// 
			this.EventButtonAll.AutoSize = true;
			this.EventButtonAll.Location = new System.Drawing.Point(179, 16);
			this.EventButtonAll.Name = "EventButtonAll";
			this.EventButtonAll.Size = new System.Drawing.Size(36, 17);
			this.EventButtonAll.TabIndex = 13;
			this.EventButtonAll.Text = "All";
			this.EventButtonAll.UseVisualStyleBackColor = true;
			this.EventButtonAll.CheckedChanged += new System.EventHandler(this.EventButton_CheckedChanged);
			// 
			// EventButtonJump
			// 
			this.EventButtonJump.AutoSize = true;
			this.EventButtonJump.Location = new System.Drawing.Point(123, 16);
			this.EventButtonJump.Name = "EventButtonJump";
			this.EventButtonJump.Size = new System.Drawing.Size(50, 17);
			this.EventButtonJump.TabIndex = 12;
			this.EventButtonJump.Text = "Jump";
			this.EventButtonJump.UseVisualStyleBackColor = true;
			this.EventButtonJump.CheckedChanged += new System.EventHandler(this.EventButton_CheckedChanged);
			// 
			// EventButtonTrick
			// 
			this.EventButtonTrick.AutoSize = true;
			this.EventButtonTrick.Location = new System.Drawing.Point(68, 16);
			this.EventButtonTrick.Name = "EventButtonTrick";
			this.EventButtonTrick.Size = new System.Drawing.Size(49, 17);
			this.EventButtonTrick.TabIndex = 11;
			this.EventButtonTrick.Text = "Trick";
			this.EventButtonTrick.UseVisualStyleBackColor = true;
			this.EventButtonTrick.CheckedChanged += new System.EventHandler(this.EventButton_CheckedChanged);
			// 
			// EventButtonSlalom
			// 
			this.EventButtonSlalom.AutoSize = true;
			this.EventButtonSlalom.Location = new System.Drawing.Point(6, 16);
			this.EventButtonSlalom.Name = "EventButtonSlalom";
			this.EventButtonSlalom.Size = new System.Drawing.Size(56, 17);
			this.EventButtonSlalom.TabIndex = 10;
			this.EventButtonSlalom.Text = "Slalom";
			this.EventButtonSlalom.UseVisualStyleBackColor = true;
			this.EventButtonSlalom.CheckedChanged += new System.EventHandler(this.EventButton_CheckedChanged);
			// 
			// EventGroupList
			// 
			this.EventGroupList.FormattingEnabled = true;
			this.EventGroupList.Location = new System.Drawing.Point(425, 52);
			this.EventGroupList.Name = "EventGroupList";
			this.EventGroupList.Size = new System.Drawing.Size(121, 21);
			this.EventGroupList.TabIndex = 20;
			this.EventGroupList.SelectedIndexChanged += new System.EventHandler(this.EventGroupList_SelectedIndexChanged);
			// 
			// topNavBar
			// 
			this.topNavBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navRefresh,
            this.navPrint,
            this.navExport,
            this.navFilter,
            this.navSort,
            this.navSave,
            this.navAddNewItem,
            this.navDeleteItem,
            this.navCopyItem,
            this.navTemplateButton});
			this.topNavBar.Location = new System.Drawing.Point(0, 0);
			this.topNavBar.Name = "topNavBar";
			this.topNavBar.Size = new System.Drawing.Size(1087, 38);
			this.topNavBar.TabIndex = 21;
			this.topNavBar.Text = "toolStrip1";
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
			// navExport
			// 
			this.navExport.Image = ((System.Drawing.Image)(resources.GetObject("navExport.Image")));
			this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.navExport.Name = "navExport";
			this.navExport.Size = new System.Drawing.Size(45, 35);
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
			// navSave
			// 
			this.navSave.Image = ((System.Drawing.Image)(resources.GetObject("navSave.Image")));
			this.navSave.Name = "navSave";
			this.navSave.Size = new System.Drawing.Size(35, 35);
			this.navSave.Text = "Save";
			this.navSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navSave.ToolTipText = "Save";
			this.navSave.Click += new System.EventHandler(this.navSave_Click);
			// 
			// navAddNewItem
			// 
			this.navAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("navAddNewItem.Image")));
			this.navAddNewItem.Name = "navAddNewItem";
			this.navAddNewItem.RightToLeftAutoMirrorImage = true;
			this.navAddNewItem.Size = new System.Drawing.Size(33, 35);
			this.navAddNewItem.Text = "Add";
			this.navAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navAddNewItem.Click += new System.EventHandler(this.navAddNewItem_Click);
			// 
			// navDeleteItem
			// 
			this.navDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("navDeleteItem.Image")));
			this.navDeleteItem.Name = "navDeleteItem";
			this.navDeleteItem.RightToLeftAutoMirrorImage = true;
			this.navDeleteItem.Size = new System.Drawing.Size(44, 35);
			this.navDeleteItem.Text = "Delete";
			this.navDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navDeleteItem.Click += new System.EventHandler(this.navDeleteItem_Click);
			// 
			// navCopyItem
			// 
			this.navCopyItem.Image = global::WaterskiScoringSystem.Properties.Resources.folder;
			this.navCopyItem.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.navCopyItem.Name = "navCopyItem";
			this.navCopyItem.Size = new System.Drawing.Size(39, 35);
			this.navCopyItem.Text = "Copy";
			this.navCopyItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navCopyItem.ToolTipText = "Copy";
			this.navCopyItem.Click += new System.EventHandler(this.navCopyItem_Click);
			// 
			// navTemplateButton
			// 
			this.navTemplateButton.Image = global::WaterskiScoringSystem.Properties.Resources.folder;
			this.navTemplateButton.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.navTemplateButton.Name = "navTemplateButton";
			this.navTemplateButton.Size = new System.Drawing.Size(59, 35);
			this.navTemplateButton.Text = "Template";
			this.navTemplateButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.navTemplateButton.Click += new System.EventHandler(this.navTemplateButton_Click);
			// 
			// dataGridViewImageColumn1
			// 
			this.dataGridViewImageColumn1.HeaderText = "";
			this.dataGridViewImageColumn1.Image = global::WaterskiScoringSystem.Properties.Resources.folder;
			this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
			this.dataGridViewImageColumn1.Width = 25;
			// 
			// labelMemberQuickFind
			// 
			this.labelMemberQuickFind.BackColor = System.Drawing.Color.LightGoldenrodYellow;
			this.labelMemberQuickFind.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.labelMemberQuickFind.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMemberQuickFind.Location = new System.Drawing.Point(344, 90);
			this.labelMemberQuickFind.Name = "labelMemberQuickFind";
			this.labelMemberQuickFind.Size = new System.Drawing.Size(312, 32);
			this.labelMemberQuickFind.TabIndex = 0;
			this.labelMemberQuickFind.Text = "Quick Find:     Enter characters in official cell and hit ENTER to jump to a name" +
    " starting with entered characters";
			// 
			// EnterKeyLabel
			// 
			this.EnterKeyLabel.AutoSize = true;
			this.EnterKeyLabel.BackColor = System.Drawing.SystemColors.Control;
			this.EnterKeyLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.EnterKeyLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EnterKeyLabel.Location = new System.Drawing.Point(552, 54);
			this.EnterKeyLabel.Name = "EnterKeyLabel";
			this.EnterKeyLabel.Size = new System.Drawing.Size(91, 14);
			this.EnterKeyLabel.TabIndex = 0;
			this.EnterKeyLabel.Text = "To initiate SAVE";
			// 
			// EnterKeyLabel2
			// 
			this.EnterKeyLabel2.AutoSize = true;
			this.EnterKeyLabel2.BackColor = System.Drawing.SystemColors.Control;
			this.EnterKeyLabel2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.EnterKeyLabel2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.EnterKeyLabel2.Location = new System.Drawing.Point(552, 68);
			this.EnterKeyLabel2.Name = "EnterKeyLabel2";
			this.EnterKeyLabel2.Size = new System.Drawing.Size(227, 14);
			this.EnterKeyLabel2.TabIndex = 0;
			this.EnterKeyLabel2.Text = "Press ENTER in the Start or End time cell";
			// 
			// roundActiveSelect
			// 
			this.roundActiveSelect.AutoScroll = true;
			this.roundActiveSelect.BackColor = System.Drawing.Color.Silver;
			this.roundActiveSelect.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.roundActiveSelect.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.roundActiveSelect.Location = new System.Drawing.Point(5, 54);
			this.roundActiveSelect.Margin = new System.Windows.Forms.Padding(0);
			this.roundActiveSelect.Name = "roundActiveSelect";
			this.roundActiveSelect.RoundValue = "";
			this.roundActiveSelect.Size = new System.Drawing.Size(185, 45);
			this.roundActiveSelect.TabIndex = 11;
			this.roundActiveSelect.Tag = "";
			// 
			// OfficialWorkAsgmt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1087, 435);
			this.Controls.Add(this.roundActiveSelect);
			this.Controls.Add(this.activeLabel);
			this.Controls.Add(this.EnterKeyLabel2);
			this.Controls.Add(this.EnterKeyLabel);
			this.Controls.Add(this.labelMemberQuickFind);
			this.Controls.Add(this.topNavBar);
			this.Controls.Add(this.EventGroupList);
			this.Controls.Add(this.eventGroupBox);
			this.Controls.Add(this.labelMemberSelect);
			this.Controls.Add(this.listTourMemberDataGridView);
			this.Controls.Add(this.winStatus);
			this.Controls.Add(this.officialWorkAsgmtDataGridView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "OfficialWorkAsgmt";
			this.Text = "Official Work Assignments";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OfficialWorkAsgmt_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OfficialWorkAsgmt_FormClosed);
			this.Load += new System.EventHandler(this.OfficialWorkAsgmt_Load);
			((System.ComponentModel.ISupportInitialize)(this.officialWorkAsgmtDataGridView)).EndInit();
			this.winStatus.ResumeLayout(false);
			this.winStatus.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.listTourMemberDataGridView)).EndInit();
			this.eventGroupBox.ResumeLayout(false);
			this.eventGroupBox.PerformLayout();
			this.topNavBar.ResumeLayout(false);
			this.topNavBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView officialWorkAsgmtDataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.DataGridView listTourMemberDataGridView;
        private System.Windows.Forms.Label labelMemberSelect;
        private System.Windows.Forms.GroupBox eventGroupBox;
        private System.Windows.Forms.RadioButton EventButtonSlalom;
        private System.Windows.Forms.RadioButton EventButtonAll;
        private System.Windows.Forms.RadioButton EventButtonJump;
        private System.Windows.Forms.RadioButton EventButtonTrick;
        private System.Windows.Forms.ComboBox EventGroupList;
        private System.Windows.Forms.ToolStrip topNavBar;
        private System.Windows.Forms.ToolStripButton navPrint;
        private System.Windows.Forms.ToolStripButton navRefresh;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSave;
        private System.Windows.Forms.ToolStripButton navDeleteItem;
        private System.Windows.Forms.ToolStripButton navAddNewItem;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.Label labelMemberQuickFind;
        private System.Windows.Forms.Label EnterKeyLabel;
        private System.Windows.Forms.Label EnterKeyLabel2;
        private Common.RoundSelect roundActiveSelect;
        private System.Windows.Forms.Label activeLabel;
        private System.Windows.Forms.ToolStripButton navCopyItem;
		private System.Windows.Forms.DataGridViewTextBoxColumn PK;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn Event;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewComboBoxColumn WorkAsgmt;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberName;
		private System.Windows.Forms.DataGridViewTextBoxColumn StartTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn EndTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
		private System.Windows.Forms.DataGridViewButtonColumn EndTimeStamp;
		private System.Windows.Forms.DataGridViewTextBoxColumn Updated;
		private System.Windows.Forms.ToolStripButton navTemplateButton;
	}
}