namespace WaterskiScoringSystem.Tournament {
    partial class EditRegMember {
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
			System.Windows.Forms.Label reqdFlagInfo;
			System.Windows.Forms.Label labelGender;
			System.Windows.Forms.Label ageGroupLabel;
			System.Windows.Forms.Label memberIdLabel;
			System.Windows.Forms.Label lastNameLabel;
			System.Windows.Forms.Label firstNameLabel;
			System.Windows.Forms.Label stateLabel;
			System.Windows.Forms.Label federationLabel;
			System.Windows.Forms.Label skiYearAgeLabel;
			System.Windows.Forms.Label memberStatusLabel;
			System.Windows.Forms.Label cityLabel;
			System.Windows.Forms.Label ForeignFederationIDLabel;
			this.showMemberStatus = new System.Windows.Forms.Label();
			this.AgeAsOfLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.reqdFlagGender = new System.Windows.Forms.Label();
			this.reqdFlagFirstName = new System.Windows.Forms.Label();
			this.reqdFlagLastName = new System.Windows.Forms.Label();
			this.reqdFlagMemberId = new System.Windows.Forms.Label();
			this.editFederation = new System.Windows.Forms.ComboBox();
			this.editMemberId = new System.Windows.Forms.TextBox();
			this.editLastName = new System.Windows.Forms.TextBox();
			this.editFirstName = new System.Windows.Forms.TextBox();
			this.editState = new System.Windows.Forms.TextBox();
			this.editSkiYearAge = new System.Windows.Forms.TextBox();
			this.DataGridView = new System.Windows.Forms.DataGridView();
			this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkiYearAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReadyToSki = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MemberStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RegListLabel = new System.Windows.Forms.Label();
			this.editCity = new System.Windows.Forms.TextBox();
			this.AgeGroupSelect = new WaterskiScoringSystem.Common.AgeGroupSelect();
			this.editGenderSelect = new WaterskiScoringSystem.Common.GenderSelect();
			this.AddButton = new System.Windows.Forms.Button();
			this.editForeignFederationID = new System.Windows.Forms.TextBox();
			reqdFlagInfo = new System.Windows.Forms.Label();
			labelGender = new System.Windows.Forms.Label();
			ageGroupLabel = new System.Windows.Forms.Label();
			memberIdLabel = new System.Windows.Forms.Label();
			lastNameLabel = new System.Windows.Forms.Label();
			firstNameLabel = new System.Windows.Forms.Label();
			stateLabel = new System.Windows.Forms.Label();
			federationLabel = new System.Windows.Forms.Label();
			skiYearAgeLabel = new System.Windows.Forms.Label();
			memberStatusLabel = new System.Windows.Forms.Label();
			cityLabel = new System.Windows.Forms.Label();
			ForeignFederationIDLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// reqdFlagInfo
			// 
			reqdFlagInfo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			reqdFlagInfo.Location = new System.Drawing.Point(185, 4);
			reqdFlagInfo.Name = "reqdFlagInfo";
			reqdFlagInfo.Size = new System.Drawing.Size(137, 13);
			reqdFlagInfo.TabIndex = 132;
			reqdFlagInfo.Text = "= required data items";
			reqdFlagInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelGender
			// 
			labelGender.AutoSize = true;
			labelGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			labelGender.Location = new System.Drawing.Point(166, 111);
			labelGender.Name = "labelGender";
			labelGender.Size = new System.Drawing.Size(51, 15);
			labelGender.TabIndex = 120;
			labelGender.Text = "Gender:";
			labelGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ageGroupLabel
			// 
			ageGroupLabel.AutoSize = true;
			ageGroupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			ageGroupLabel.Location = new System.Drawing.Point(406, 111);
			ageGroupLabel.Name = "ageGroupLabel";
			ageGroupLabel.Size = new System.Drawing.Size(53, 15);
			ageGroupLabel.TabIndex = 110;
			ageGroupLabel.Text = "Division:";
			ageGroupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// memberIdLabel
			// 
			memberIdLabel.AutoSize = true;
			memberIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			memberIdLabel.Location = new System.Drawing.Point(147, 26);
			memberIdLabel.Name = "memberIdLabel";
			memberIdLabel.Size = new System.Drawing.Size(70, 15);
			memberIdLabel.TabIndex = 107;
			memberIdLabel.Text = "Member Id:";
			memberIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lastNameLabel
			// 
			lastNameLabel.AutoSize = true;
			lastNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lastNameLabel.Location = new System.Drawing.Point(147, 80);
			lastNameLabel.Name = "lastNameLabel";
			lastNameLabel.Size = new System.Drawing.Size(70, 15);
			lastNameLabel.TabIndex = 108;
			lastNameLabel.Text = "Last Name:";
			lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// firstNameLabel
			// 
			firstNameLabel.AutoSize = true;
			firstNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			firstNameLabel.Location = new System.Drawing.Point(147, 53);
			firstNameLabel.Name = "firstNameLabel";
			firstNameLabel.Size = new System.Drawing.Size(70, 15);
			firstNameLabel.TabIndex = 109;
			firstNameLabel.Text = "First Name:";
			firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// stateLabel
			// 
			stateLabel.AutoSize = true;
			stateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			stateLabel.Location = new System.Drawing.Point(179, 161);
			stateLabel.Name = "stateLabel";
			stateLabel.Size = new System.Drawing.Size(38, 15);
			stateLabel.TabIndex = 116;
			stateLabel.Text = "State:";
			stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// federationLabel
			// 
			federationLabel.AutoSize = true;
			federationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			federationLabel.Location = new System.Drawing.Point(148, 216);
			federationLabel.Name = "federationLabel";
			federationLabel.Size = new System.Drawing.Size(69, 15);
			federationLabel.TabIndex = 117;
			federationLabel.Text = "Federation:";
			federationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// skiYearAgeLabel
			// 
			skiYearAgeLabel.AutoSize = true;
			skiYearAgeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			skiYearAgeLabel.Location = new System.Drawing.Point(141, 136);
			skiYearAgeLabel.Name = "skiYearAgeLabel";
			skiYearAgeLabel.Size = new System.Drawing.Size(79, 15);
			skiYearAgeLabel.TabIndex = 118;
			skiYearAgeLabel.Text = "Ski Year Age:";
			skiYearAgeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// memberStatusLabel
			// 
			memberStatusLabel.AutoSize = true;
			memberStatusLabel.CausesValidation = false;
			memberStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			memberStatusLabel.Location = new System.Drawing.Point(126, 273);
			memberStatusLabel.Margin = new System.Windows.Forms.Padding(0);
			memberStatusLabel.Name = "memberStatusLabel";
			memberStatusLabel.Size = new System.Drawing.Size(94, 15);
			memberStatusLabel.TabIndex = 113;
			memberStatusLabel.Text = "Member Status:";
			memberStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cityLabel
			// 
			cityLabel.AutoSize = true;
			cityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			cityLabel.Location = new System.Drawing.Point(179, 188);
			cityLabel.Name = "cityLabel";
			cityLabel.Size = new System.Drawing.Size(26, 15);
			cityLabel.TabIndex = 0;
			cityLabel.Text = "City";
			cityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// showMemberStatus
			// 
			this.showMemberStatus.AutoEllipsis = true;
			this.showMemberStatus.AutoSize = true;
			this.showMemberStatus.BackColor = System.Drawing.SystemColors.ControlLight;
			this.showMemberStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.showMemberStatus.CausesValidation = false;
			this.showMemberStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.showMemberStatus.Location = new System.Drawing.Point(224, 273);
			this.showMemberStatus.Margin = new System.Windows.Forms.Padding(0);
			this.showMemberStatus.MinimumSize = new System.Drawing.Size(150, 0);
			this.showMemberStatus.Name = "showMemberStatus";
			this.showMemberStatus.Size = new System.Drawing.Size(150, 18);
			this.showMemberStatus.TabIndex = 0;
			this.showMemberStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AgeAsOfLabel
			// 
			this.AgeAsOfLabel.AutoSize = true;
			this.AgeAsOfLabel.Location = new System.Drawing.Point(260, 137);
			this.AgeAsOfLabel.Name = "AgeAsOfLabel";
			this.AgeAsOfLabel.Size = new System.Drawing.Size(100, 13);
			this.AgeAsOfLabel.TabIndex = 119;
			this.AgeAsOfLabel.Text = "age as of 1/1/2010";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(399, 107);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(10, 12);
			this.label2.TabIndex = 139;
			this.label2.Text = "*";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(330, 303);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 100;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(162, 303);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 90;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(176, 4);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(10, 12);
			this.label1.TabIndex = 136;
			this.label1.Text = "*";
			// 
			// reqdFlagGender
			// 
			this.reqdFlagGender.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagGender.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagGender.Location = new System.Drawing.Point(159, 108);
			this.reqdFlagGender.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagGender.Name = "reqdFlagGender";
			this.reqdFlagGender.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagGender.TabIndex = 133;
			this.reqdFlagGender.Text = "*";
			// 
			// reqdFlagFirstName
			// 
			this.reqdFlagFirstName.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagFirstName.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagFirstName.Location = new System.Drawing.Point(140, 51);
			this.reqdFlagFirstName.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagFirstName.Name = "reqdFlagFirstName";
			this.reqdFlagFirstName.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagFirstName.TabIndex = 130;
			this.reqdFlagFirstName.Text = "*";
			// 
			// reqdFlagLastName
			// 
			this.reqdFlagLastName.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagLastName.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagLastName.Location = new System.Drawing.Point(140, 77);
			this.reqdFlagLastName.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagLastName.Name = "reqdFlagLastName";
			this.reqdFlagLastName.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagLastName.TabIndex = 128;
			this.reqdFlagLastName.Text = "*";
			// 
			// reqdFlagMemberId
			// 
			this.reqdFlagMemberId.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagMemberId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagMemberId.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagMemberId.Location = new System.Drawing.Point(140, 25);
			this.reqdFlagMemberId.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagMemberId.Name = "reqdFlagMemberId";
			this.reqdFlagMemberId.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagMemberId.TabIndex = 129;
			this.reqdFlagMemberId.Text = "*";
			// 
			// editFederation
			// 
			this.editFederation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editFederation.FormattingEnabled = true;
			this.editFederation.Location = new System.Drawing.Point(224, 212);
			this.editFederation.Name = "editFederation";
			this.editFederation.Size = new System.Drawing.Size(125, 23);
			this.editFederation.TabIndex = 80;
			// 
			// editMemberId
			// 
			this.editMemberId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editMemberId.Location = new System.Drawing.Point(224, 23);
			this.editMemberId.MaxLength = 9;
			this.editMemberId.Name = "editMemberId";
			this.editMemberId.Size = new System.Drawing.Size(75, 21);
			this.editMemberId.TabIndex = 10;
			this.editMemberId.WordWrap = false;
			// 
			// editLastName
			// 
			this.editLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editLastName.Location = new System.Drawing.Point(224, 77);
			this.editLastName.MaxLength = 128;
			this.editLastName.Name = "editLastName";
			this.editLastName.Size = new System.Drawing.Size(125, 21);
			this.editLastName.TabIndex = 30;
			this.editLastName.WordWrap = false;
			// 
			// editFirstName
			// 
			this.editFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editFirstName.Location = new System.Drawing.Point(224, 50);
			this.editFirstName.MaxLength = 128;
			this.editFirstName.Name = "editFirstName";
			this.editFirstName.Size = new System.Drawing.Size(125, 21);
			this.editFirstName.TabIndex = 20;
			// 
			// editState
			// 
			this.editState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editState.Location = new System.Drawing.Point(224, 158);
			this.editState.MaxLength = 2;
			this.editState.Name = "editState";
			this.editState.Size = new System.Drawing.Size(30, 21);
			this.editState.TabIndex = 70;
			this.editState.WordWrap = false;
			// 
			// editSkiYearAge
			// 
			this.editSkiYearAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editSkiYearAge.Location = new System.Drawing.Point(224, 133);
			this.editSkiYearAge.MaxLength = 3;
			this.editSkiYearAge.Name = "editSkiYearAge";
			this.editSkiYearAge.Size = new System.Drawing.Size(30, 21);
			this.editSkiYearAge.TabIndex = 50;
			this.editSkiYearAge.WordWrap = false;
			this.editSkiYearAge.Validated += new System.EventHandler(this.editSkiYearAge_Validated);
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
			this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.SkierName,
            this.AgeGroup,
            this.SkiYearAge,
            this.Gender,
            this.City,
            this.State,
            this.Federation,
            this.ReadyToSki,
            this.MemberStatus});
			this.DataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.DataGridView.Location = new System.Drawing.Point(5, 355);
			this.DataGridView.Name = "DataGridView";
			this.DataGridView.RowHeadersWidth = 31;
			this.DataGridView.Size = new System.Drawing.Size(576, 200);
			this.DataGridView.TabIndex = 120;
			// 
			// MemberId
			// 
			this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MemberId.HeaderText = "MemberId";
			this.MemberId.Name = "MemberId";
			this.MemberId.ReadOnly = true;
			this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.MemberId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			this.MemberId.Width = 70;
			// 
			// SkierName
			// 
			this.SkierName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierName.HeaderText = "Skier Name";
			this.SkierName.Name = "SkierName";
			this.SkierName.ReadOnly = true;
			this.SkierName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.SkierName.Width = 80;
			// 
			// AgeGroup
			// 
			this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.AgeGroup.HeaderText = "Div";
			this.AgeGroup.Name = "AgeGroup";
			this.AgeGroup.ReadOnly = true;
			this.AgeGroup.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.AgeGroup.Width = 35;
			// 
			// SkiYearAge
			// 
			this.SkiYearAge.HeaderText = "Ski Year Age";
			this.SkiYearAge.Name = "SkiYearAge";
			this.SkiYearAge.ReadOnly = true;
			this.SkiYearAge.Width = 40;
			// 
			// Gender
			// 
			this.Gender.HeaderText = "Gender";
			this.Gender.Name = "Gender";
			this.Gender.ReadOnly = true;
			this.Gender.Width = 40;
			// 
			// City
			// 
			this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.City.HeaderText = "City";
			this.City.Name = "City";
			this.City.ReadOnly = true;
			this.City.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.City.Width = 80;
			// 
			// State
			// 
			this.State.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.State.HeaderText = "State";
			this.State.Name = "State";
			this.State.ReadOnly = true;
			this.State.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.State.Width = 40;
			// 
			// Federation
			// 
			this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Federation.HeaderText = "Fed";
			this.Federation.Name = "Federation";
			this.Federation.ReadOnly = true;
			this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.Federation.Width = 50;
			// 
			// ReadyToSki
			// 
			this.ReadyToSki.HeaderText = "Eligible Part";
			this.ReadyToSki.Name = "ReadyToSki";
			this.ReadyToSki.ReadOnly = true;
			this.ReadyToSki.Width = 40;
			// 
			// MemberStatus
			// 
			this.MemberStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MemberStatus.HeaderText = "Status";
			this.MemberStatus.Name = "MemberStatus";
			this.MemberStatus.ReadOnly = true;
			this.MemberStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.MemberStatus.Width = 65;
			// 
			// RegListLabel
			// 
			this.RegListLabel.AutoSize = true;
			this.RegListLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RegListLabel.Location = new System.Drawing.Point(153, 338);
			this.RegListLabel.Name = "RegListLabel";
			this.RegListLabel.Size = new System.Drawing.Size(266, 14);
			this.RegListLabel.TabIndex = 0;
			this.RegListLabel.Text = "Existing registrations for this individual";
			// 
			// editCity
			// 
			this.editCity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editCity.Location = new System.Drawing.Point(224, 185);
			this.editCity.MaxLength = 129;
			this.editCity.Name = "editCity";
			this.editCity.Size = new System.Drawing.Size(120, 21);
			this.editCity.TabIndex = 71;
			this.editCity.WordWrap = false;
			// 
			// AgeGroupSelect
			// 
			this.AgeGroupSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.AgeGroupSelect.AutoScroll = true;
			this.AgeGroupSelect.BackColor = System.Drawing.Color.Silver;
			this.AgeGroupSelect.CurrentValue = "";
			this.AgeGroupSelect.Location = new System.Drawing.Point(409, 129);
			this.AgeGroupSelect.Name = "AgeGroupSelect";
			this.AgeGroupSelect.Size = new System.Drawing.Size(172, 125);
			this.AgeGroupSelect.TabIndex = 55;
			// 
			// editGenderSelect
			// 
			this.editGenderSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editGenderSelect.Location = new System.Drawing.Point(224, 104);
			this.editGenderSelect.Margin = new System.Windows.Forms.Padding(0);
			this.editGenderSelect.Name = "editGenderSelect";
			this.editGenderSelect.RatingValue = "";
			this.editGenderSelect.Size = new System.Drawing.Size(145, 29);
			this.editGenderSelect.TabIndex = 40;
			this.editGenderSelect.Tag = "";
			// 
			// AddButton
			// 
			this.AddButton.Enabled = false;
			this.AddButton.Location = new System.Drawing.Point(246, 303);
			this.AddButton.Name = "AddButton";
			this.AddButton.Size = new System.Drawing.Size(75, 23);
			this.AddButton.TabIndex = 140;
			this.AddButton.Text = "Add";
			this.AddButton.UseVisualStyleBackColor = true;
			this.AddButton.Visible = false;
			this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
			// 
			// ForeignFederationIDLabel
			// 
			ForeignFederationIDLabel.AutoSize = true;
			ForeignFederationIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			ForeignFederationIDLabel.Location = new System.Drawing.Point(126, 244);
			ForeignFederationIDLabel.Name = "ForeignFederationIDLabel";
			ForeignFederationIDLabel.Size = new System.Drawing.Size(91, 15);
			ForeignFederationIDLabel.TabIndex = 141;
			ForeignFederationIDLabel.Text = "Foreign Fed ID:";
			ForeignFederationIDLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// editForeignFederationID
			// 
			this.editForeignFederationID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editForeignFederationID.Location = new System.Drawing.Point(224, 241);
			this.editForeignFederationID.MaxLength = 9;
			this.editForeignFederationID.Name = "editForeignFederationID";
			this.editForeignFederationID.Size = new System.Drawing.Size(75, 21);
			this.editForeignFederationID.TabIndex = 142;
			this.editForeignFederationID.WordWrap = false;
			// 
			// EditRegMember
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 561);
			this.Controls.Add(ForeignFederationIDLabel);
			this.Controls.Add(this.editForeignFederationID);
			this.Controls.Add(this.AddButton);
			this.Controls.Add(this.showMemberStatus);
			this.Controls.Add(cityLabel);
			this.Controls.Add(this.editCity);
			this.Controls.Add(this.AgeGroupSelect);
			this.Controls.Add(this.RegListLabel);
			this.Controls.Add(this.DataGridView);
			this.Controls.Add(this.AgeAsOfLabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(reqdFlagInfo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.reqdFlagGender);
			this.Controls.Add(this.reqdFlagFirstName);
			this.Controls.Add(this.reqdFlagLastName);
			this.Controls.Add(this.reqdFlagMemberId);
			this.Controls.Add(labelGender);
			this.Controls.Add(ageGroupLabel);
			this.Controls.Add(this.editFederation);
			this.Controls.Add(memberIdLabel);
			this.Controls.Add(this.editMemberId);
			this.Controls.Add(lastNameLabel);
			this.Controls.Add(this.editLastName);
			this.Controls.Add(firstNameLabel);
			this.Controls.Add(this.editFirstName);
			this.Controls.Add(stateLabel);
			this.Controls.Add(this.editState);
			this.Controls.Add(federationLabel);
			this.Controls.Add(skiYearAgeLabel);
			this.Controls.Add(this.editSkiYearAge);
			this.Controls.Add(memberStatusLabel);
			this.Controls.Add(this.editGenderSelect);
			this.Name = "EditRegMember";
			this.Text = "EditRegMember";
			this.Load += new System.EventHandler(this.EditRegMember_Load);
			((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AgeAsOfLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label reqdFlagGender;
        private System.Windows.Forms.Label reqdFlagFirstName;
        private System.Windows.Forms.Label reqdFlagLastName;
        private System.Windows.Forms.Label reqdFlagMemberId;
        private System.Windows.Forms.ComboBox editFederation;
        private System.Windows.Forms.TextBox editMemberId;
        private System.Windows.Forms.TextBox editLastName;
        private System.Windows.Forms.TextBox editFirstName;
        private System.Windows.Forms.TextBox editState;
        private System.Windows.Forms.TextBox editSkiYearAge;
        private WaterskiScoringSystem.Common.GenderSelect editGenderSelect;
        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.Label RegListLabel;
        private WaterskiScoringSystem.Common.AgeGroupSelect AgeGroupSelect;
        private System.Windows.Forms.TextBox editCity;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierName;
		private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkiYearAge;
		private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
		private System.Windows.Forms.DataGridViewTextBoxColumn City;
		private System.Windows.Forms.DataGridViewTextBoxColumn State;
		private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReadyToSki;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberStatus;
		private System.Windows.Forms.Label showMemberStatus;
		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.TextBox editForeignFederationID;
	}
}