namespace WaterskiScoringSystem.Tournament {
    partial class BoatUseReport {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label nameLabel;
            System.Windows.Forms.Label classLabel;
            System.Windows.Forms.Label sanctionIdLabel;
            System.Windows.Forms.Label eventLocationLabel;
            System.Windows.Forms.Label eventDatesLabel;
            System.Windows.Forms.Label rulesLabel;
            System.Windows.Forms.Label chiefJudgeNameLabel;
            System.Windows.Forms.Label chiefDriverNameLabel;
            System.Windows.Forms.Label RegionLabel;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.Label chiefDriverSigLabel;
            System.Windows.Forms.Label chiefJudgeSigLabel;
            System.Windows.Forms.Label JumpCreditLabel;
            System.Windows.Forms.Label TrickCreditLabel;
            System.Windows.Forms.Label SlalomCreditLabel;
            System.Windows.Forms.Label BoatCreditLabel;
            System.Windows.Forms.Label JumpUsedLabel;
            System.Windows.Forms.Label TrickUsedLabel;
            System.Windows.Forms.Label SlalomUsedLabel;
            System.Windows.Forms.Label EventsUsedLabel;
            System.Windows.Forms.Label SpeedControlLabel;
            System.Windows.Forms.Label ModelYearLabel;
            System.Windows.Forms.Label BoatManuModelLabel;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ReportTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.RegionTextBox = new System.Windows.Forms.TextBox();
            this.tournamentBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.waterskiDataSet = new WaterskiScoringSystem.waterskiDataSet();
            this.tourBoatUseDataGridView = new System.Windows.Forms.DataGridView();
            this.tourBoatUseBindingSource = new System.Windows.Forms.BindingSource( this.components );
            this.chiefDriverNameTextBox = new System.Windows.Forms.TextBox();
            this.chiefJudgeNameTextBox = new System.Windows.Forms.TextBox();
            this.rulesTextBox = new System.Windows.Forms.TextBox();
            this.eventDatesTextBox = new System.Windows.Forms.TextBox();
            this.eventLocationTextBox = new System.Windows.Forms.TextBox();
            this.sanctionIdTextBox = new System.Windows.Forms.TextBox();
            this.classTextBox = new System.Windows.Forms.TextBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tournamentTableAdapter = new WaterskiScoringSystem.waterskiDataSetTableAdapters.TournamentTableAdapter();
            this.tableAdapterManager = new WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager();
            this.tourBoatUseTableAdapter = new WaterskiScoringSystem.waterskiDataSetTableAdapters.TourBoatUseTableAdapter();
            this.PrintButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.ChiefDriverSigTextBox = new System.Windows.Forms.TextBox();
            this.ChiefJudgeSigTextBox = new System.Windows.Forms.TextBox();
            this.BoatCreditGroupBox = new System.Windows.Forms.GroupBox();
            this.PKBoatUse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BoatModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModelYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SpeedControlVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlalomUsed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrickUsed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.JumpUsed = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SlalomCredit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrickCredit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.JumpCredit = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CertOfInsurance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HullId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Owner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InsuranceCompany = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PreEventNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PostEventNotes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Notes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            nameLabel = new System.Windows.Forms.Label();
            classLabel = new System.Windows.Forms.Label();
            sanctionIdLabel = new System.Windows.Forms.Label();
            eventLocationLabel = new System.Windows.Forms.Label();
            eventDatesLabel = new System.Windows.Forms.Label();
            rulesLabel = new System.Windows.Forms.Label();
            chiefJudgeNameLabel = new System.Windows.Forms.Label();
            chiefDriverNameLabel = new System.Windows.Forms.Label();
            RegionLabel = new System.Windows.Forms.Label();
            chiefDriverSigLabel = new System.Windows.Forms.Label();
            chiefJudgeSigLabel = new System.Windows.Forms.Label();
            JumpCreditLabel = new System.Windows.Forms.Label();
            TrickCreditLabel = new System.Windows.Forms.Label();
            SlalomCreditLabel = new System.Windows.Forms.Label();
            BoatCreditLabel = new System.Windows.Forms.Label();
            JumpUsedLabel = new System.Windows.Forms.Label();
            TrickUsedLabel = new System.Windows.Forms.Label();
            SlalomUsedLabel = new System.Windows.Forms.Label();
            EventsUsedLabel = new System.Windows.Forms.Label();
            SpeedControlLabel = new System.Windows.Forms.Label();
            ModelYearLabel = new System.Windows.Forms.Label();
            BoatManuModelLabel = new System.Windows.Forms.Label();
            this.ReportTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.tournamentBindingSource ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.waterskiDataSet ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tourBoatUseDataGridView ) ).BeginInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tourBoatUseBindingSource ) ).BeginInit();
            this.BoatCreditGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameLabel
            // 
            nameLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            nameLabel.Location = new System.Drawing.Point( 5, 5 );
            nameLabel.Margin = new System.Windows.Forms.Padding( 0 );
            nameLabel.Name = "nameLabel";
            nameLabel.Size = new System.Drawing.Size( 125, 18 );
            nameLabel.TabIndex = 0;
            nameLabel.Text = "Tournament:";
            nameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // classLabel
            // 
            classLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            classLabel.Location = new System.Drawing.Point( 530, 5 );
            classLabel.Margin = new System.Windows.Forms.Padding( 0 );
            classLabel.Name = "classLabel";
            classLabel.Size = new System.Drawing.Size( 51, 18 );
            classLabel.TabIndex = 0;
            classLabel.Text = "Class:";
            classLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sanctionIdLabel
            // 
            sanctionIdLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            sanctionIdLabel.Location = new System.Drawing.Point( 481, 65 );
            sanctionIdLabel.Margin = new System.Windows.Forms.Padding( 0 );
            sanctionIdLabel.Name = "sanctionIdLabel";
            sanctionIdLabel.Size = new System.Drawing.Size( 100, 18 );
            sanctionIdLabel.TabIndex = 0;
            sanctionIdLabel.Text = "Sanction No:";
            sanctionIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // eventLocationLabel
            // 
            eventLocationLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            eventLocationLabel.Location = new System.Drawing.Point( 5, 35 );
            eventLocationLabel.Margin = new System.Windows.Forms.Padding( 0 );
            eventLocationLabel.Name = "eventLocationLabel";
            eventLocationLabel.Size = new System.Drawing.Size( 125, 18 );
            eventLocationLabel.TabIndex = 0;
            eventLocationLabel.Text = "Event Location:";
            eventLocationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // eventDatesLabel
            // 
            eventDatesLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            eventDatesLabel.Location = new System.Drawing.Point( 531, 35 );
            eventDatesLabel.Margin = new System.Windows.Forms.Padding( 0 );
            eventDatesLabel.Name = "eventDatesLabel";
            eventDatesLabel.Size = new System.Drawing.Size( 50, 18 );
            eventDatesLabel.TabIndex = 0;
            eventDatesLabel.Text = "Date:";
            eventDatesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // rulesLabel
            // 
            rulesLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            rulesLabel.Location = new System.Drawing.Point( 5, 65 );
            rulesLabel.Margin = new System.Windows.Forms.Padding( 0 );
            rulesLabel.Name = "rulesLabel";
            rulesLabel.Size = new System.Drawing.Size( 125, 18 );
            rulesLabel.TabIndex = 0;
            rulesLabel.Text = "Sport Div:";
            rulesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chiefJudgeNameLabel
            // 
            chiefJudgeNameLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            chiefJudgeNameLabel.Location = new System.Drawing.Point( 371, 703 );
            chiefJudgeNameLabel.Margin = new System.Windows.Forms.Padding( 0 );
            chiefJudgeNameLabel.Name = "chiefJudgeNameLabel";
            chiefJudgeNameLabel.Size = new System.Drawing.Size( 160, 18 );
            chiefJudgeNameLabel.TabIndex = 0;
            chiefJudgeNameLabel.Text = "Name of Chief Judge:";
            chiefJudgeNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chiefDriverNameLabel
            // 
            chiefDriverNameLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            chiefDriverNameLabel.Location = new System.Drawing.Point( 5, 703 );
            chiefDriverNameLabel.Margin = new System.Windows.Forms.Padding( 0 );
            chiefDriverNameLabel.Name = "chiefDriverNameLabel";
            chiefDriverNameLabel.Size = new System.Drawing.Size( 145, 18 );
            chiefDriverNameLabel.TabIndex = 0;
            chiefDriverNameLabel.Text = "Chief Driver Name:";
            chiefDriverNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RegionLabel
            // 
            RegionLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            RegionLabel.Location = new System.Drawing.Point( 638, 5 );
            RegionLabel.Margin = new System.Windows.Forms.Padding( 0 );
            RegionLabel.Name = "RegionLabel";
            RegionLabel.Size = new System.Drawing.Size( 65, 18 );
            RegionLabel.TabIndex = 0;
            RegionLabel.Text = "Region:";
            RegionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ReportTabControl
            // 
            this.ReportTabControl.Controls.Add( this.tabPage1 );
            this.ReportTabControl.Controls.Add( this.tabPage2 );
            this.ReportTabControl.Location = new System.Drawing.Point( 7, 26 );
            this.ReportTabControl.Margin = new System.Windows.Forms.Padding( 4 );
            this.ReportTabControl.Name = "ReportTabControl";
            this.ReportTabControl.SelectedIndex = 0;
            this.ReportTabControl.Size = new System.Drawing.Size( 748, 780 );
            this.ReportTabControl.TabIndex = 0;
            this.ReportTabControl.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add( this.BoatCreditGroupBox );
            this.tabPage1.Controls.Add( SpeedControlLabel );
            this.tabPage1.Controls.Add( ModelYearLabel );
            this.tabPage1.Controls.Add( BoatManuModelLabel );
            this.tabPage1.Controls.Add( chiefDriverSigLabel );
            this.tabPage1.Controls.Add( this.ChiefDriverSigTextBox );
            this.tabPage1.Controls.Add( chiefJudgeSigLabel );
            this.tabPage1.Controls.Add( this.ChiefJudgeSigTextBox );
            this.tabPage1.Controls.Add( RegionLabel );
            this.tabPage1.Controls.Add( this.RegionTextBox );
            this.tabPage1.Controls.Add( this.tourBoatUseDataGridView );
            this.tabPage1.Controls.Add( chiefDriverNameLabel );
            this.tabPage1.Controls.Add( this.chiefDriverNameTextBox );
            this.tabPage1.Controls.Add( chiefJudgeNameLabel );
            this.tabPage1.Controls.Add( this.chiefJudgeNameTextBox );
            this.tabPage1.Controls.Add( rulesLabel );
            this.tabPage1.Controls.Add( this.rulesTextBox );
            this.tabPage1.Controls.Add( eventDatesLabel );
            this.tabPage1.Controls.Add( this.eventDatesTextBox );
            this.tabPage1.Controls.Add( eventLocationLabel );
            this.tabPage1.Controls.Add( this.eventLocationTextBox );
            this.tabPage1.Controls.Add( sanctionIdLabel );
            this.tabPage1.Controls.Add( this.sanctionIdTextBox );
            this.tabPage1.Controls.Add( classLabel );
            this.tabPage1.Controls.Add( this.classTextBox );
            this.tabPage1.Controls.Add( nameLabel );
            this.tabPage1.Controls.Add( this.nameTextBox );
            this.tabPage1.Location = new System.Drawing.Point( 4, 26 );
            this.tabPage1.Margin = new System.Windows.Forms.Padding( 4 );
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding( 4 );
            this.tabPage1.Size = new System.Drawing.Size( 740, 750 );
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Report Page 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // RegionTextBox
            // 
            this.RegionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RegionTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.RegionTextBox.Location = new System.Drawing.Point( 708, 5 );
            this.RegionTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.RegionTextBox.Name = "RegionTextBox";
            this.RegionTextBox.ReadOnly = true;
            this.RegionTextBox.Size = new System.Drawing.Size( 25, 18 );
            this.RegionTextBox.TabIndex = 3;
            this.RegionTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.RegionTextBox.WordWrap = false;
            // 
            // tournamentBindingSource
            // 
            this.tournamentBindingSource.DataMember = "Tournament";
            this.tournamentBindingSource.DataSource = this.waterskiDataSet;
            // 
            // waterskiDataSet
            // 
            this.waterskiDataSet.DataSetName = "waterskiDataSet";
            this.waterskiDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tourBoatUseDataGridView
            // 
            this.tourBoatUseDataGridView.AllowUserToAddRows = false;
            this.tourBoatUseDataGridView.AllowUserToDeleteRows = false;
            this.tourBoatUseDataGridView.AllowUserToResizeColumns = false;
            this.tourBoatUseDataGridView.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font( "Arial Narrow", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tourBoatUseDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tourBoatUseDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tourBoatUseDataGridView.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] {
            this.PKBoatUse,
            this.SanctionId,
            this.BoatModel,
            this.ModelYear,
            this.SpeedControlVersion,
            this.SlalomUsed,
            this.TrickUsed,
            this.JumpUsed,
            this.SlalomCredit,
            this.TrickCredit,
            this.JumpCredit,
            this.CertOfInsurance,
            this.HullId,
            this.Owner,
            this.InsuranceCompany,
            this.PreEventNotes,
            this.PostEventNotes,
            this.Notes} );
            this.tourBoatUseDataGridView.DataSource = this.tourBoatUseBindingSource;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tourBoatUseDataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.tourBoatUseDataGridView.Location = new System.Drawing.Point( 5, 173 );
            this.tourBoatUseDataGridView.Name = "tourBoatUseDataGridView";
            this.tourBoatUseDataGridView.RowHeadersVisible = false;
            this.tourBoatUseDataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.tourBoatUseDataGridView.Size = new System.Drawing.Size( 730, 210 );
            this.tourBoatUseDataGridView.TabIndex = 61;
            // 
            // tourBoatUseBindingSource
            // 
            this.tourBoatUseBindingSource.DataMember = "TourBoatUse";
            this.tourBoatUseBindingSource.DataSource = this.waterskiDataSet;
            // 
            // chiefDriverNameTextBox
            // 
            this.chiefDriverNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chiefDriverNameTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.chiefDriverNameTextBox.Location = new System.Drawing.Point( 155, 703 );
            this.chiefDriverNameTextBox.Margin = new System.Windows.Forms.Padding( 4 );
            this.chiefDriverNameTextBox.Name = "chiefDriverNameTextBox";
            this.chiefDriverNameTextBox.ReadOnly = true;
            this.chiefDriverNameTextBox.Size = new System.Drawing.Size( 200, 18 );
            this.chiefDriverNameTextBox.TabIndex = 90;
            // 
            // chiefJudgeNameTextBox
            // 
            this.chiefJudgeNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chiefJudgeNameTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.chiefJudgeNameTextBox.Location = new System.Drawing.Point( 535, 703 );
            this.chiefJudgeNameTextBox.Margin = new System.Windows.Forms.Padding( 4 );
            this.chiefJudgeNameTextBox.Name = "chiefJudgeNameTextBox";
            this.chiefJudgeNameTextBox.ReadOnly = true;
            this.chiefJudgeNameTextBox.Size = new System.Drawing.Size( 200, 18 );
            this.chiefJudgeNameTextBox.TabIndex = 95;
            // 
            // rulesTextBox
            // 
            this.rulesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rulesTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "Rules", true ) );
            this.rulesTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.rulesTextBox.Location = new System.Drawing.Point( 135, 65 );
            this.rulesTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.rulesTextBox.Name = "rulesTextBox";
            this.rulesTextBox.ReadOnly = true;
            this.rulesTextBox.Size = new System.Drawing.Size( 100, 18 );
            this.rulesTextBox.TabIndex = 6;
            // 
            // eventDatesTextBox
            // 
            this.eventDatesTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eventDatesTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "EventDates", true ) );
            this.eventDatesTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.eventDatesTextBox.Location = new System.Drawing.Point( 585, 35 );
            this.eventDatesTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.eventDatesTextBox.Name = "eventDatesTextBox";
            this.eventDatesTextBox.ReadOnly = true;
            this.eventDatesTextBox.Size = new System.Drawing.Size( 100, 18 );
            this.eventDatesTextBox.TabIndex = 5;
            // 
            // eventLocationTextBox
            // 
            this.eventLocationTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eventLocationTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "EventLocation", true ) );
            this.eventLocationTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.eventLocationTextBox.Location = new System.Drawing.Point( 135, 35 );
            this.eventLocationTextBox.Margin = new System.Windows.Forms.Padding( 4 );
            this.eventLocationTextBox.Name = "eventLocationTextBox";
            this.eventLocationTextBox.ReadOnly = true;
            this.eventLocationTextBox.Size = new System.Drawing.Size( 132, 18 );
            this.eventLocationTextBox.TabIndex = 4;
            // 
            // sanctionIdTextBox
            // 
            this.sanctionIdTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sanctionIdTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "SanctionId", true ) );
            this.sanctionIdTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.sanctionIdTextBox.Location = new System.Drawing.Point( 585, 65 );
            this.sanctionIdTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.sanctionIdTextBox.Name = "sanctionIdTextBox";
            this.sanctionIdTextBox.ReadOnly = true;
            this.sanctionIdTextBox.Size = new System.Drawing.Size( 100, 18 );
            this.sanctionIdTextBox.TabIndex = 7;
            this.sanctionIdTextBox.WordWrap = false;
            // 
            // classTextBox
            // 
            this.classTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.classTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "Class", true ) );
            this.classTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.classTextBox.Location = new System.Drawing.Point( 585, 5 );
            this.classTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.classTextBox.Name = "classTextBox";
            this.classTextBox.ReadOnly = true;
            this.classTextBox.Size = new System.Drawing.Size( 25, 18 );
            this.classTextBox.TabIndex = 2;
            this.classTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.classTextBox.WordWrap = false;
            // 
            // nameTextBox
            // 
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nameTextBox.DataBindings.Add( new System.Windows.Forms.Binding( "Text", this.tournamentBindingSource, "Name", true ) );
            this.nameTextBox.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.nameTextBox.Location = new System.Drawing.Point( 135, 5 );
            this.nameTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size( 350, 18 );
            this.nameTextBox.TabIndex = 1;
            this.nameTextBox.WordWrap = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point( 4, 26 );
            this.tabPage2.Margin = new System.Windows.Forms.Padding( 4 );
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding( 4 );
            this.tabPage2.Size = new System.Drawing.Size( 740, 750 );
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Report Page 2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tournamentTableAdapter
            // 
            this.tournamentTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.CodeValueListTableAdapter = null;
            this.tableAdapterManager.JumpMeterSetupTableAdapter = null;
            this.tableAdapterManager.JumpVideoSetupTableAdapter = null;
            this.tableAdapterManager.MemberListTableAdapter = null;
            this.tableAdapterManager.NopsDataTableAdapter = null;
            this.tableAdapterManager.OfficialWorkTableAdapter = null;
            this.tableAdapterManager.SkierRankingTableAdapter = null;
            this.tableAdapterManager.TourBoatUseTableAdapter = this.tourBoatUseTableAdapter;
            this.tableAdapterManager.TournamentTableAdapter = this.tournamentTableAdapter;
            this.tableAdapterManager.UpdateOrder = WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // tourBoatUseTableAdapter
            // 
            this.tourBoatUseTableAdapter.ClearBeforeFill = true;
            // 
            // PrintButton
            // 
            this.PrintButton.Location = new System.Drawing.Point( 321, 0 );
            this.PrintButton.Name = "PrintButton";
            this.PrintButton.Size = new System.Drawing.Size( 75, 23 );
            this.PrintButton.TabIndex = 60;
            this.PrintButton.Text = "Print";
            this.PrintButton.UseVisualStyleBackColor = true;
            this.PrintButton.Click += new System.EventHandler( this.PrintButton_Click );
            // 
            // RefreshButton
            // 
            this.RefreshButton.Location = new System.Drawing.Point( 222, 0 );
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size( 75, 23 );
            this.RefreshButton.TabIndex = 59;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler( this.RefreshButton_Click );
            // 
            // chiefDriverSigLabel
            // 
            chiefDriverSigLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            chiefDriverSigLabel.Location = new System.Drawing.Point( 5, 726 );
            chiefDriverSigLabel.Margin = new System.Windows.Forms.Padding( 0 );
            chiefDriverSigLabel.Name = "chiefDriverSigLabel";
            chiefDriverSigLabel.Size = new System.Drawing.Size( 145, 18 );
            chiefDriverSigLabel.TabIndex = 0;
            chiefDriverSigLabel.Text = "Signature:";
            chiefDriverSigLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChiefDriverSigTextBox
            // 
            this.ChiefDriverSigTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChiefDriverSigTextBox.Font = new System.Drawing.Font( "Script MT Bold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.ChiefDriverSigTextBox.Location = new System.Drawing.Point( 155, 726 );
            this.ChiefDriverSigTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.ChiefDriverSigTextBox.Name = "ChiefDriverSigTextBox";
            this.ChiefDriverSigTextBox.ReadOnly = true;
            this.ChiefDriverSigTextBox.Size = new System.Drawing.Size( 200, 19 );
            this.ChiefDriverSigTextBox.TabIndex = 98;
            this.ChiefDriverSigTextBox.WordWrap = false;
            // 
            // chiefJudgeSigLabel
            // 
            chiefJudgeSigLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            chiefJudgeSigLabel.Location = new System.Drawing.Point( 371, 726 );
            chiefJudgeSigLabel.Margin = new System.Windows.Forms.Padding( 0 );
            chiefJudgeSigLabel.Name = "chiefJudgeSigLabel";
            chiefJudgeSigLabel.Size = new System.Drawing.Size( 160, 18 );
            chiefJudgeSigLabel.TabIndex = 0;
            chiefJudgeSigLabel.Text = "Signature:";
            chiefJudgeSigLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ChiefJudgeSigTextBox
            // 
            this.ChiefJudgeSigTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ChiefJudgeSigTextBox.Font = new System.Drawing.Font( "Script MT Bold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.ChiefJudgeSigTextBox.Location = new System.Drawing.Point( 535, 726 );
            this.ChiefJudgeSigTextBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.ChiefJudgeSigTextBox.Name = "ChiefJudgeSigTextBox";
            this.ChiefJudgeSigTextBox.ReadOnly = true;
            this.ChiefJudgeSigTextBox.Size = new System.Drawing.Size( 200, 19 );
            this.ChiefJudgeSigTextBox.TabIndex = 99;
            this.ChiefJudgeSigTextBox.WordWrap = false;
            // 
            // BoatCreditGroupBox
            // 
            this.BoatCreditGroupBox.Controls.Add( JumpCreditLabel );
            this.BoatCreditGroupBox.Controls.Add( TrickCreditLabel );
            this.BoatCreditGroupBox.Controls.Add( SlalomCreditLabel );
            this.BoatCreditGroupBox.Controls.Add( BoatCreditLabel );
            this.BoatCreditGroupBox.Controls.Add( JumpUsedLabel );
            this.BoatCreditGroupBox.Controls.Add( TrickUsedLabel );
            this.BoatCreditGroupBox.Controls.Add( SlalomUsedLabel );
            this.BoatCreditGroupBox.Controls.Add( EventsUsedLabel );
            this.BoatCreditGroupBox.Location = new System.Drawing.Point( 421, 115 );
            this.BoatCreditGroupBox.Margin = new System.Windows.Forms.Padding( 0 );
            this.BoatCreditGroupBox.Name = "BoatCreditGroupBox";
            this.BoatCreditGroupBox.Padding = new System.Windows.Forms.Padding( 0 );
            this.BoatCreditGroupBox.Size = new System.Drawing.Size( 315, 54 );
            this.BoatCreditGroupBox.TabIndex = 102;
            this.BoatCreditGroupBox.TabStop = false;
            // 
            // JumpCreditLabel
            // 
            JumpCreditLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            JumpCreditLabel.Location = new System.Drawing.Point( 263, 32 );
            JumpCreditLabel.Margin = new System.Windows.Forms.Padding( 0 );
            JumpCreditLabel.Name = "JumpCreditLabel";
            JumpCreditLabel.Size = new System.Drawing.Size( 47, 18 );
            JumpCreditLabel.TabIndex = 6;
            JumpCreditLabel.Text = "Jump";
            JumpCreditLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // TrickCreditLabel
            // 
            TrickCreditLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            TrickCreditLabel.Location = new System.Drawing.Point( 216, 32 );
            TrickCreditLabel.Margin = new System.Windows.Forms.Padding( 0 );
            TrickCreditLabel.Name = "TrickCreditLabel";
            TrickCreditLabel.Size = new System.Drawing.Size( 45, 18 );
            TrickCreditLabel.TabIndex = 5;
            TrickCreditLabel.Text = "Trick";
            TrickCreditLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // SlalomCreditLabel
            // 
            SlalomCreditLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            SlalomCreditLabel.Location = new System.Drawing.Point( 160, 32 );
            SlalomCreditLabel.Margin = new System.Windows.Forms.Padding( 0 );
            SlalomCreditLabel.Name = "SlalomCreditLabel";
            SlalomCreditLabel.Size = new System.Drawing.Size( 55, 18 );
            SlalomCreditLabel.TabIndex = 8;
            SlalomCreditLabel.Text = "Slalom";
            SlalomCreditLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // BoatCreditLabel
            // 
            BoatCreditLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            BoatCreditLabel.Location = new System.Drawing.Point( 160, 14 );
            BoatCreditLabel.Margin = new System.Windows.Forms.Padding( 0 );
            BoatCreditLabel.Name = "BoatCreditLabel";
            BoatCreditLabel.Size = new System.Drawing.Size( 150, 18 );
            BoatCreditLabel.TabIndex = 7;
            BoatCreditLabel.Text = "Boat Credit";
            BoatCreditLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // JumpUsedLabel
            // 
            JumpUsedLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            JumpUsedLabel.Location = new System.Drawing.Point( 109, 32 );
            JumpUsedLabel.Margin = new System.Windows.Forms.Padding( 0 );
            JumpUsedLabel.Name = "JumpUsedLabel";
            JumpUsedLabel.Size = new System.Drawing.Size( 47, 18 );
            JumpUsedLabel.TabIndex = 2;
            JumpUsedLabel.Text = "Jump";
            JumpUsedLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // TrickUsedLabel
            // 
            TrickUsedLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            TrickUsedLabel.Location = new System.Drawing.Point( 62, 32 );
            TrickUsedLabel.Margin = new System.Windows.Forms.Padding( 0 );
            TrickUsedLabel.Name = "TrickUsedLabel";
            TrickUsedLabel.Size = new System.Drawing.Size( 45, 18 );
            TrickUsedLabel.TabIndex = 1;
            TrickUsedLabel.Text = "Trick";
            TrickUsedLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // SlalomUsedLabel
            // 
            SlalomUsedLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            SlalomUsedLabel.Location = new System.Drawing.Point( 6, 32 );
            SlalomUsedLabel.Margin = new System.Windows.Forms.Padding( 0 );
            SlalomUsedLabel.Name = "SlalomUsedLabel";
            SlalomUsedLabel.Size = new System.Drawing.Size( 55, 18 );
            SlalomUsedLabel.TabIndex = 4;
            SlalomUsedLabel.Text = "Slalom";
            SlalomUsedLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // EventsUsedLabel
            // 
            EventsUsedLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            EventsUsedLabel.Location = new System.Drawing.Point( 6, 14 );
            EventsUsedLabel.Margin = new System.Windows.Forms.Padding( 0 );
            EventsUsedLabel.Name = "EventsUsedLabel";
            EventsUsedLabel.Size = new System.Drawing.Size( 150, 18 );
            EventsUsedLabel.TabIndex = 3;
            EventsUsedLabel.Text = "Events Used";
            EventsUsedLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // SpeedControlLabel
            // 
            SpeedControlLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            SpeedControlLabel.Location = new System.Drawing.Point( 273, 133 );
            SpeedControlLabel.Margin = new System.Windows.Forms.Padding( 0 );
            SpeedControlLabel.Name = "SpeedControlLabel";
            SpeedControlLabel.Size = new System.Drawing.Size( 140, 36 );
            SpeedControlLabel.TabIndex = 103;
            SpeedControlLabel.Text = "Speed Control Version (ex. 6.5ng)";
            SpeedControlLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // ModelYearLabel
            // 
            ModelYearLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            ModelYearLabel.Location = new System.Drawing.Point( 213, 133 );
            ModelYearLabel.Margin = new System.Windows.Forms.Padding( 0 );
            ModelYearLabel.Name = "ModelYearLabel";
            ModelYearLabel.Size = new System.Drawing.Size( 52, 36 );
            ModelYearLabel.TabIndex = 100;
            ModelYearLabel.Text = "Model Year";
            ModelYearLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // BoatManuModelLabel
            // 
            BoatManuModelLabel.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            BoatManuModelLabel.Location = new System.Drawing.Point( 5, 151 );
            BoatManuModelLabel.Margin = new System.Windows.Forms.Padding( 0 );
            BoatManuModelLabel.Name = "BoatManuModelLabel";
            BoatManuModelLabel.Size = new System.Drawing.Size( 200, 18 );
            BoatManuModelLabel.TabIndex = 101;
            BoatManuModelLabel.Text = "Boat Manufacturer Model";
            BoatManuModelLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // PKBoatUse
            // 
            this.PKBoatUse.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PKBoatUse.DataPropertyName = "PK";
            this.PKBoatUse.HeaderText = "PK";
            this.PKBoatUse.Name = "PKBoatUse";
            this.PKBoatUse.ReadOnly = true;
            this.PKBoatUse.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PKBoatUse.Visible = false;
            this.PKBoatUse.Width = 50;
            // 
            // SanctionId
            // 
            this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SanctionId.DataPropertyName = "SanctionId";
            this.SanctionId.HeaderText = "SanctionId";
            this.SanctionId.Name = "SanctionId";
            this.SanctionId.ReadOnly = true;
            this.SanctionId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SanctionId.Visible = false;
            // 
            // BoatModel
            // 
            this.BoatModel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.BoatModel.DataPropertyName = "BoatModel";
            dataGridViewCellStyle2.Font = new System.Drawing.Font( "Arial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.BoatModel.DefaultCellStyle = dataGridViewCellStyle2;
            this.BoatModel.HeaderText = "Boat Manufacturer Model";
            this.BoatModel.Name = "BoatModel";
            this.BoatModel.ReadOnly = true;
            this.BoatModel.Width = 207;
            // 
            // ModelYear
            // 
            this.ModelYear.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ModelYear.DataPropertyName = "ModelYear";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font( "Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ModelYear.DefaultCellStyle = dataGridViewCellStyle3;
            this.ModelYear.HeaderText = "Model Year";
            this.ModelYear.Name = "ModelYear";
            this.ModelYear.ReadOnly = true;
            this.ModelYear.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ModelYear.Width = 55;
            // 
            // SpeedControlVersion
            // 
            this.SpeedControlVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SpeedControlVersion.DataPropertyName = "SpeedControlVersion";
            this.SpeedControlVersion.HeaderText = "Speed Control Version (ex. 6.5ng)";
            this.SpeedControlVersion.Name = "SpeedControlVersion";
            this.SpeedControlVersion.ReadOnly = true;
            this.SpeedControlVersion.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SpeedControlVersion.Width = 175;
            // 
            // SlalomUsed
            // 
            this.SlalomUsed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomUsed.DataPropertyName = "SlalomUsed";
            this.SlalomUsed.FalseValue = "N";
            this.SlalomUsed.HeaderText = "Used Slalom";
            this.SlalomUsed.Name = "SlalomUsed";
            this.SlalomUsed.ReadOnly = true;
            this.SlalomUsed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomUsed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SlalomUsed.TrueValue = "Y";
            this.SlalomUsed.Width = 55;
            // 
            // TrickUsed
            // 
            this.TrickUsed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickUsed.DataPropertyName = "TrickUsed";
            this.TrickUsed.FalseValue = "N";
            this.TrickUsed.HeaderText = "Used Trick";
            this.TrickUsed.Name = "TrickUsed";
            this.TrickUsed.ReadOnly = true;
            this.TrickUsed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickUsed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.TrickUsed.TrueValue = "Y";
            this.TrickUsed.Width = 45;
            // 
            // JumpUsed
            // 
            this.JumpUsed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpUsed.DataPropertyName = "JumpUsed";
            this.JumpUsed.FalseValue = "N";
            this.JumpUsed.HeaderText = "Used Jump";
            this.JumpUsed.Name = "JumpUsed";
            this.JumpUsed.ReadOnly = true;
            this.JumpUsed.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpUsed.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.JumpUsed.TrueValue = "Y";
            this.JumpUsed.Width = 45;
            // 
            // SlalomCredit
            // 
            this.SlalomCredit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomCredit.DataPropertyName = "SlalomCredit";
            this.SlalomCredit.FalseValue = "N";
            this.SlalomCredit.HeaderText = "Credit Slalom";
            this.SlalomCredit.Name = "SlalomCredit";
            this.SlalomCredit.ReadOnly = true;
            this.SlalomCredit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomCredit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SlalomCredit.TrueValue = "Y";
            this.SlalomCredit.Width = 55;
            // 
            // TrickCredit
            // 
            this.TrickCredit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickCredit.DataPropertyName = "TrickCredit";
            this.TrickCredit.FalseValue = "N";
            this.TrickCredit.HeaderText = "Credit Trick";
            this.TrickCredit.Name = "TrickCredit";
            this.TrickCredit.ReadOnly = true;
            this.TrickCredit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickCredit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.TrickCredit.TrueValue = "Y";
            this.TrickCredit.Width = 45;
            // 
            // JumpCredit
            // 
            this.JumpCredit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpCredit.DataPropertyName = "JumpCredit";
            this.JumpCredit.FalseValue = "N";
            this.JumpCredit.HeaderText = "Credit Jump";
            this.JumpCredit.Name = "JumpCredit";
            this.JumpCredit.ReadOnly = true;
            this.JumpCredit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpCredit.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.JumpCredit.TrueValue = "Y";
            this.JumpCredit.Width = 45;
            // 
            // CertOfInsurance
            // 
            this.CertOfInsurance.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CertOfInsurance.DataPropertyName = "CertOfInsurance";
            this.CertOfInsurance.HeaderText = "CertOfInsurance";
            this.CertOfInsurance.Name = "CertOfInsurance";
            this.CertOfInsurance.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.CertOfInsurance.Visible = false;
            // 
            // HullId
            // 
            this.HullId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.HullId.DataPropertyName = "HullId";
            this.HullId.HeaderText = "HullId";
            this.HullId.Name = "HullId";
            this.HullId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.HullId.Visible = false;
            // 
            // Owner
            // 
            this.Owner.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Owner.DataPropertyName = "Owner";
            this.Owner.HeaderText = "Owner";
            this.Owner.Name = "Owner";
            this.Owner.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Owner.Visible = false;
            // 
            // InsuranceCompany
            // 
            this.InsuranceCompany.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.InsuranceCompany.DataPropertyName = "InsuranceCompany";
            this.InsuranceCompany.HeaderText = "InsuranceCompany";
            this.InsuranceCompany.Name = "InsuranceCompany";
            this.InsuranceCompany.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InsuranceCompany.Visible = false;
            // 
            // PreEventNotes
            // 
            this.PreEventNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PreEventNotes.DataPropertyName = "PreEventNotes";
            this.PreEventNotes.HeaderText = "PreEventNotes";
            this.PreEventNotes.Name = "PreEventNotes";
            this.PreEventNotes.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PreEventNotes.Visible = false;
            // 
            // PostEventNotes
            // 
            this.PostEventNotes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PostEventNotes.DataPropertyName = "PostEventNotes";
            this.PostEventNotes.HeaderText = "PostEventNotes";
            this.PostEventNotes.Name = "PostEventNotes";
            this.PostEventNotes.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PostEventNotes.Visible = false;
            // 
            // Notes
            // 
            this.Notes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Notes.DataPropertyName = "Notes";
            this.Notes.HeaderText = "Notes";
            this.Notes.Name = "Notes";
            this.Notes.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Notes.Visible = false;
            // 
            // BoatUseReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 17F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size( 809, 778 );
            this.Controls.Add( this.PrintButton );
            this.Controls.Add( this.RefreshButton );
            this.Controls.Add( this.ReportTabControl );
            this.Font = new System.Drawing.Font( "Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
            this.Margin = new System.Windows.Forms.Padding( 4 );
            this.Name = "BoatUseReport";
            this.Text = "Tow Boat Use";
            this.Load += new System.EventHandler( this.BoatUseReport_Load );
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.BoatUseReport_FormClosed );
            this.ReportTabControl.ResumeLayout( false );
            this.tabPage1.ResumeLayout( false );
            this.tabPage1.PerformLayout();
            ( (System.ComponentModel.ISupportInitialize)( this.tournamentBindingSource ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.waterskiDataSet ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tourBoatUseDataGridView ) ).EndInit();
            ( (System.ComponentModel.ISupportInitialize)( this.tourBoatUseBindingSource ) ).EndInit();
            this.BoatCreditGroupBox.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.TabControl ReportTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private waterskiDataSet waterskiDataSet;
        private System.Windows.Forms.BindingSource tournamentBindingSource;
        private WaterskiScoringSystem.waterskiDataSetTableAdapters.TournamentTableAdapter tournamentTableAdapter;
        private WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox eventDatesTextBox;
        private System.Windows.Forms.TextBox eventLocationTextBox;
        private System.Windows.Forms.TextBox sanctionIdTextBox;
        private System.Windows.Forms.TextBox classTextBox;
        private System.Windows.Forms.TextBox chiefDriverNameTextBox;
        private System.Windows.Forms.TextBox chiefJudgeNameTextBox;
        private System.Windows.Forms.TextBox rulesTextBox;
        private WaterskiScoringSystem.waterskiDataSetTableAdapters.TourBoatUseTableAdapter tourBoatUseTableAdapter;
        private System.Windows.Forms.Button PrintButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.BindingSource tourBoatUseBindingSource;
        private System.Windows.Forms.DataGridView tourBoatUseDataGridView;
        private System.Windows.Forms.TextBox RegionTextBox;
        private System.Windows.Forms.TextBox ChiefDriverSigTextBox;
        private System.Windows.Forms.TextBox ChiefJudgeSigTextBox;
        private System.Windows.Forms.GroupBox BoatCreditGroupBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn PKBoatUse;
        private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
        private System.Windows.Forms.DataGridViewTextBoxColumn BoatModel;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModelYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn SpeedControlVersion;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SlalomUsed;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TrickUsed;
        private System.Windows.Forms.DataGridViewCheckBoxColumn JumpUsed;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SlalomCredit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn TrickCredit;
        private System.Windows.Forms.DataGridViewCheckBoxColumn JumpCredit;
        private System.Windows.Forms.DataGridViewTextBoxColumn CertOfInsurance;
        private System.Windows.Forms.DataGridViewTextBoxColumn HullId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Owner;
        private System.Windows.Forms.DataGridViewTextBoxColumn InsuranceCompany;
        private System.Windows.Forms.DataGridViewTextBoxColumn PreEventNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn PostEventNotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn Notes;
    }
}