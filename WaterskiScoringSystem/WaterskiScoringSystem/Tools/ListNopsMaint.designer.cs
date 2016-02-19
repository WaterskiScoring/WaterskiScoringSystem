namespace WaterskiScoringSystem.Tools {
    partial class ListNopsMaint {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ListNopsMaint ) );
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.PK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkiYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventBasePoints = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaseAdj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RatingMedian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RatingRec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RatingOpen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OverallBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OverallExp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventsReqd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Updated = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.winStatus = new System.Windows.Forms.StatusStrip();
            this.winStatusMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.topMenuNav = new System.Windows.Forms.ToolStrip();
            this.navExport = new System.Windows.Forms.ToolStripButton();
            this.navFilter = new System.Windows.Forms.ToolStripButton();
            this.navSort = new System.Windows.Forms.ToolStripButton();
            this.navSaveItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            ( (System.ComponentModel.ISupportInitialize)( this.DataGridView ) ).BeginInit();
            this.winStatus.SuspendLayout();
            this.topMenuNav.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridView
            // 
            this.DataGridView.AllowUserToAddRows = false;
            this.DataGridView.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.NullValue = "0";
            this.DataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridView.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.PK,
            this.SkiYear,
            this.Event,
            this.AgeGroup,
            this.EventBasePoints,
            this.BaseAdj,
            this.RatingMedian,
            this.RatingRec,
            this.RatingOpen,
            this.OverallBase,
            this.OverallExp,
            this.EventsReqd,
            this.Updated} );
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle9.Format = "N2";
            dataGridViewCellStyle9.NullValue = "null";
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.DataGridView.Location = new System.Drawing.Point( 5, 63 );
            this.DataGridView.Name = "DataGridView";
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.DataGridView.RowHeadersWidth = 30;
            this.DataGridView.Size = new System.Drawing.Size( 699, 355 );
            this.DataGridView.TabIndex = 1;
            this.DataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler( this.DataGridView_RowEnter );
            this.DataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler( this.DataGridView_CellValidated );
            this.DataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler( this.DataGridView_CellValidating );
            this.DataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler( this.DataGridView_DataError );
            this.DataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler( this.DataGridView_CellEnter );
            // 
            // PK
            // 
            this.PK.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PK.HeaderText = "PK";
            this.PK.Name = "PK";
            this.PK.ReadOnly = true;
            this.PK.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PK.Width = 50;
            // 
            // SkiYear
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Format = "00";
            dataGridViewCellStyle3.NullValue = null;
            this.SkiYear.DefaultCellStyle = dataGridViewCellStyle3;
            this.SkiYear.HeaderText = "Ski Year";
            this.SkiYear.Name = "SkiYear";
            this.SkiYear.ReadOnly = true;
            this.SkiYear.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkiYear.Width = 40;
            // 
            // Event
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.NullValue = null;
            this.Event.DefaultCellStyle = dataGridViewCellStyle4;
            this.Event.HeaderText = "Event";
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            this.Event.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.Width = 70;
            // 
            // AgeGroup
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.AgeGroup.DefaultCellStyle = dataGridViewCellStyle5;
            this.AgeGroup.HeaderText = "Age Group";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.AgeGroup.ToolTipText = "AWSA age group";
            this.AgeGroup.Width = 50;
            // 
            // EventBasePoints
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "#,##0.##";
            dataGridViewCellStyle6.NullValue = "0";
            this.EventBasePoints.DefaultCellStyle = dataGridViewCellStyle6;
            this.EventBasePoints.HeaderText = "Base Points";
            this.EventBasePoints.Name = "EventBasePoints";
            this.EventBasePoints.ReadOnly = true;
            this.EventBasePoints.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventBasePoints.ToolTipText = "Event base points";
            this.EventBasePoints.Width = 60;
            // 
            // BaseAdj
            // 
            dataGridViewCellStyle7.Format = "#,##0.##";
            dataGridViewCellStyle7.NullValue = "0";
            this.BaseAdj.DefaultCellStyle = dataGridViewCellStyle7;
            this.BaseAdj.HeaderText = "Base Adj";
            this.BaseAdj.Name = "BaseAdj";
            this.BaseAdj.ReadOnly = true;
            this.BaseAdj.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.BaseAdj.ToolTipText = "Event base adjustment";
            this.BaseAdj.Width = 50;
            // 
            // RatingMedian
            // 
            this.RatingMedian.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.RatingMedian.HeaderText = "Rating Median";
            this.RatingMedian.Name = "RatingMedian";
            this.RatingMedian.ReadOnly = true;
            this.RatingMedian.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RatingMedian.Width = 55;
            // 
            // RatingRec
            // 
            this.RatingRec.HeaderText = "Event Record";
            this.RatingRec.Name = "RatingRec";
            this.RatingRec.ReadOnly = true;
            this.RatingRec.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RatingRec.ToolTipText = "Points for event record";
            this.RatingRec.Width = 60;
            // 
            // RatingOpen
            // 
            this.RatingOpen.HeaderText = "Rating Open";
            this.RatingOpen.Name = "RatingOpen";
            this.RatingOpen.ReadOnly = true;
            this.RatingOpen.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.RatingOpen.ToolTipText = "Points for open rating";
            this.RatingOpen.Width = 55;
            // 
            // OverallBase
            // 
            this.OverallBase.HeaderText = "Overall Base";
            this.OverallBase.Name = "OverallBase";
            this.OverallBase.ReadOnly = true;
            this.OverallBase.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OverallBase.Width = 50;
            // 
            // OverallExp
            // 
            this.OverallExp.HeaderText = "Overall Exp";
            this.OverallExp.Name = "OverallExp";
            this.OverallExp.ReadOnly = true;
            this.OverallExp.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.OverallExp.Width = 50;
            // 
            // EventsReqd
            // 
            dataGridViewCellStyle8.Format = "N0";
            dataGridViewCellStyle8.NullValue = "0";
            this.EventsReqd.DefaultCellStyle = dataGridViewCellStyle8;
            this.EventsReqd.HeaderText = "Events Reqd";
            this.EventsReqd.Name = "EventsReqd";
            this.EventsReqd.ReadOnly = true;
            this.EventsReqd.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.EventsReqd.ToolTipText = "Events required to qualify for overall";
            this.EventsReqd.Width = 50;
            // 
            // Updated
            // 
            this.Updated.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Updated.HeaderText = "Updated";
            this.Updated.Name = "Updated";
            this.Updated.ReadOnly = true;
            this.Updated.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Updated.Visible = false;
            this.Updated.Width = 50;
            // 
            // winStatus
            // 
            this.winStatus.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.winStatusMsg} );
            this.winStatus.Location = new System.Drawing.Point( 0, 421 );
            this.winStatus.Name = "winStatus";
            this.winStatus.Size = new System.Drawing.Size( 716, 22 );
            this.winStatus.TabIndex = 3;
            this.winStatus.Text = "statusStrip1";
            // 
            // winStatusMsg
            // 
            this.winStatusMsg.Name = "winStatusMsg";
            this.winStatusMsg.Size = new System.Drawing.Size( 0, 17 );
            // 
            // topMenuNav
            // 
            this.topMenuNav.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.navExport,
            this.navFilter,
            this.navSort,
            this.navSaveItem,
            this.bindingNavigatorDeleteItem,
            this.bindingNavigatorAddNewItem} );
            this.topMenuNav.Location = new System.Drawing.Point( 0, 0 );
            this.topMenuNav.Name = "topMenuNav";
            this.topMenuNav.Size = new System.Drawing.Size( 716, 36 );
            this.topMenuNav.TabIndex = 4;
            this.topMenuNav.Text = "toolStrip1";
            // 
            // navExport
            // 
            this.navExport.Image = ( (System.Drawing.Image)( resources.GetObject( "navExport.Image" ) ) );
            this.navExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navExport.Name = "navExport";
            this.navExport.Size = new System.Drawing.Size( 43, 33 );
            this.navExport.Text = "Export";
            this.navExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // navFilter
            // 
            this.navFilter.Image = ( (System.Drawing.Image)( resources.GetObject( "navFilter.Image" ) ) );
            this.navFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navFilter.Name = "navFilter";
            this.navFilter.Size = new System.Drawing.Size( 35, 33 );
            this.navFilter.Text = "Filter";
            this.navFilter.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // navSort
            // 
            this.navSort.Image = ( (System.Drawing.Image)( resources.GetObject( "navSort.Image" ) ) );
            this.navSort.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.navSort.Name = "navSort";
            this.navSort.Size = new System.Drawing.Size( 31, 33 );
            this.navSort.Text = "Sort";
            this.navSort.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // navSaveItem
            // 
            this.navSaveItem.Enabled = false;
            this.navSaveItem.Image = ( (System.Drawing.Image)( resources.GetObject( "navSaveItem.Image" ) ) );
            this.navSaveItem.Name = "navSaveItem";
            this.navSaveItem.Size = new System.Drawing.Size( 35, 33 );
            this.navSaveItem.Text = "Save";
            this.navSaveItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.navSaveItem.Visible = false;
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.Enabled = false;
            this.bindingNavigatorDeleteItem.Image = ( (System.Drawing.Image)( resources.GetObject( "bindingNavigatorDeleteItem.Image" ) ) );
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size( 42, 33 );
            this.bindingNavigatorDeleteItem.Text = "Delete";
            this.bindingNavigatorDeleteItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorDeleteItem.Visible = false;
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.Enabled = false;
            this.bindingNavigatorAddNewItem.Image = ( (System.Drawing.Image)( resources.GetObject( "bindingNavigatorAddNewItem.Image" ) ) );
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size( 30, 33 );
            this.bindingNavigatorAddNewItem.Text = "Add";
            this.bindingNavigatorAddNewItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.bindingNavigatorAddNewItem.Visible = false;
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Font = new System.Drawing.Font( "Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.RowStatusLabel.Location = new System.Drawing.Point( 5, 41 );
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size( 106, 14 );
            this.RowStatusLabel.TabIndex = 7;
            this.RowStatusLabel.Text = "Row 1 of 9999";
            // 
            // ListNopsMaint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 716, 443 );
            this.Controls.Add( this.RowStatusLabel );
            this.Controls.Add( this.topMenuNav );
            this.Controls.Add( this.winStatus );
            this.Controls.Add( this.DataGridView );
            this.Name = "ListNopsMaint";
            this.Text = "ListNopsMaint";
            this.Load += new System.EventHandler( this.ListNopsMaint_Load );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.ListNopsMaint_FormClosed );
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.ListNopsMaint_FormClosing );
            ( (System.ComponentModel.ISupportInitialize)( this.DataGridView ) ).EndInit();
            this.winStatus.ResumeLayout( false );
            this.winStatus.PerformLayout();
            this.topMenuNav.ResumeLayout( false );
            this.topMenuNav.PerformLayout();
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.StatusStrip winStatus;
        private System.Windows.Forms.ToolStripStatusLabel winStatusMsg;
        private System.Windows.Forms.ToolStrip topMenuNav;
        private System.Windows.Forms.ToolStripButton navExport;
        private System.Windows.Forms.ToolStripButton navFilter;
        private System.Windows.Forms.ToolStripButton navSort;
        private System.Windows.Forms.ToolStripButton navSaveItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.Label RowStatusLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn PK;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkiYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventBasePoints;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaseAdj;
        private System.Windows.Forms.DataGridViewTextBoxColumn RatingMedian;
        private System.Windows.Forms.DataGridViewTextBoxColumn RatingRec;
        private System.Windows.Forms.DataGridViewTextBoxColumn RatingOpen;
        private System.Windows.Forms.DataGridViewTextBoxColumn OverallBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn OverallExp;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventsReqd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Updated;
    }
}