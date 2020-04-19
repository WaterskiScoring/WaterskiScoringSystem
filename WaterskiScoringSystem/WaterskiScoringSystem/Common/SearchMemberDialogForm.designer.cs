namespace WaterskiScoringSystem.Common {
    partial class SearchMemberDialogForm {
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
            this.components = new System.ComponentModel.Container();
            this.inputMemberId = new System.Windows.Forms.TextBox();
            this.inputLastName = new System.Windows.Forms.TextBox();
            this.inputFirstName = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.SelectButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.labelMemberId = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.waterskiDataSet = new WaterskiScoringSystem.waterskiDataSet();
            this.memberListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.memberListTableAdapter = new WaterskiScoringSystem.waterskiDataSetTableAdapters.MemberListTableAdapter();
            this.tableAdapterManager = new WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Address2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PostalCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkiYearAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateOfBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberExpireDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlalomOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JumpOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrickOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SafetyOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScoreOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TechOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DriverOfficialRating = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InsertDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.waterskiDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberListBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // inputMemberId
            // 
            this.inputMemberId.Location = new System.Drawing.Point(226, 22);
            this.inputMemberId.Name = "inputMemberId";
            this.inputMemberId.Size = new System.Drawing.Size(100, 20);
            this.inputMemberId.TabIndex = 3;
            this.inputMemberId.Leave += new System.EventHandler(this.inputMemberId_Leave);
            // 
            // inputLastName
            // 
            this.inputLastName.Location = new System.Drawing.Point(10, 22);
            this.inputLastName.Name = "inputLastName";
            this.inputLastName.Size = new System.Drawing.Size(100, 20);
            this.inputLastName.TabIndex = 1;
            this.inputLastName.Leave += new System.EventHandler(this.inputLastName_Leave);
            // 
            // inputFirstName
            // 
            this.inputFirstName.Location = new System.Drawing.Point(120, 22);
            this.inputFirstName.Name = "inputFirstName";
            this.inputFirstName.Size = new System.Drawing.Size(100, 20);
            this.inputFirstName.TabIndex = 2;
            this.inputFirstName.Leave += new System.EventHandler(this.inputFirstName_Leave);
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(17, 49);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 4;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // SelectButton
            // 
            this.SelectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.SelectButton.Location = new System.Drawing.Point(127, 49);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(75, 23);
            this.SelectButton.TabIndex = 5;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(237, 49);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // labelMemberId
            // 
            this.labelMemberId.AutoSize = true;
            this.labelMemberId.Location = new System.Drawing.Point(249, 6);
            this.labelMemberId.Name = "labelMemberId";
            this.labelMemberId.Size = new System.Drawing.Size(54, 13);
            this.labelMemberId.TabIndex = 0;
            this.labelMemberId.Text = "MemberId";
            this.labelMemberId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(31, 6);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(58, 13);
            this.labelLastName.TabIndex = 0;
            this.labelLastName.Text = "Last Name";
            this.labelLastName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelFirstName
            // 
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Location = new System.Drawing.Point(142, 6);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(57, 13);
            this.labelFirstName.TabIndex = 0;
            this.labelFirstName.Text = "First Name";
            this.labelFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waterskiDataSet
            // 
            this.waterskiDataSet.DataSetName = "waterskiDataSet";
            this.waterskiDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // memberListBindingSource
            // 
            this.memberListBindingSource.DataMember = "MemberList";
            this.memberListBindingSource.DataSource = this.waterskiDataSet;
            // 
            // memberListTableAdapter
            // 
            this.memberListTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager
            // 
            this.tableAdapterManager.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager.CodeValueListTableAdapter = null;
            this.tableAdapterManager.JumpMeterSetupTableAdapter = null;
            this.tableAdapterManager.JumpVideoSetupTableAdapter = null;
            this.tableAdapterManager.MemberListTableAdapter = this.memberListTableAdapter;
            this.tableAdapterManager.NopsDataTableAdapter = null;
            this.tableAdapterManager.OfficialWorkTableAdapter = null;
            this.tableAdapterManager.SkierRankingTableAdapter = null;
            this.tableAdapterManager.UpdateOrder = WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
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
            this.DataGridView.AutoGenerateColumns = false;
            this.DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MemberId,
            this.LastName,
            this.FirstName,
            this.Address1,
            this.Address2,
            this.City,
            this.State,
            this.Federation,
            this.Country,
            this.PostalCode,
            this.AgeGroup,
            this.SkiYearAge,
            this.Gender,
            this.DateOfBirth,
            this.MemberStatus,
            this.MemberExpireDate,
            this.SlalomOfficialRating,
            this.JumpOfficialRating,
            this.TrickOfficialRating,
            this.SafetyOfficialRating,
            this.ScoreOfficialRating,
            this.TechOfficialRating,
            this.DriverOfficialRating,
            this.InsertDate,
            this.UpdateDate,
            this.Note});
            this.DataGridView.DataSource = this.memberListBindingSource;
            this.DataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DataGridView.Location = new System.Drawing.Point(4, 90);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.RowHeadersWidth = 31;
            this.DataGridView.Size = new System.Drawing.Size(590, 297);
            this.DataGridView.TabIndex = 10;
            this.DataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentDoubleClick);
            // 
            // MemberId
            // 
            this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberId.DataPropertyName = "MemberId";
            this.MemberId.HeaderText = "MemberId";
            this.MemberId.Name = "MemberId";
            this.MemberId.ReadOnly = true;
            this.MemberId.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.MemberId.Width = 70;
            // 
            // LastName
            // 
            this.LastName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.LastName.DataPropertyName = "LastName";
            this.LastName.HeaderText = "LastName";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LastName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.LastName.Width = 70;
            // 
            // FirstName
            // 
            this.FirstName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FirstName.DataPropertyName = "FirstName";
            this.FirstName.HeaderText = "FirstName";
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FirstName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.FirstName.Width = 60;
            // 
            // Address1
            // 
            this.Address1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Address1.DataPropertyName = "Address1";
            this.Address1.HeaderText = "Address";
            this.Address1.Name = "Address1";
            this.Address1.ReadOnly = true;
            this.Address1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Address1.Visible = false;
            this.Address1.Width = 70;
            // 
            // Address2
            // 
            this.Address2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Address2.DataPropertyName = "Address2";
            this.Address2.HeaderText = "Address2";
            this.Address2.Name = "Address2";
            this.Address2.ReadOnly = true;
            this.Address2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Address2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Address2.Visible = false;
            // 
            // City
            // 
            this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.City.DataPropertyName = "City";
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.ReadOnly = true;
            this.City.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.City.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.City.Width = 49;
            // 
            // State
            // 
            this.State.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.State.DataPropertyName = "State";
            this.State.HeaderText = "State";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            this.State.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.State.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.State.Width = 57;
            // 
            // Federation
            // 
            this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Federation.DataPropertyName = "Federation";
            this.Federation.HeaderText = "Fed";
            this.Federation.Name = "Federation";
            this.Federation.ReadOnly = true;
            this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Federation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Federation.Width = 50;
            // 
            // Country
            // 
            this.Country.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Country.DataPropertyName = "Country";
            this.Country.HeaderText = "Country";
            this.Country.Name = "Country";
            this.Country.ReadOnly = true;
            this.Country.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Country.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Country.Visible = false;
            this.Country.Width = 68;
            // 
            // PostalCode
            // 
            this.PostalCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.PostalCode.DataPropertyName = "Postalcode";
            this.PostalCode.HeaderText = "Postal Code";
            this.PostalCode.Name = "PostalCode";
            this.PostalCode.ReadOnly = true;
            this.PostalCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.PostalCode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.PostalCode.Visible = false;
            this.PostalCode.Width = 89;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AgeGroup.DataPropertyName = "AgeGroup";
            this.AgeGroup.HeaderText = "Age Group";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.Width = 45;
            // 
            // SkiYearAge
            // 
            this.SkiYearAge.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SkiYearAge.DataPropertyName = "SkiYearAge";
            this.SkiYearAge.HeaderText = "Ski Age";
            this.SkiYearAge.Name = "SkiYearAge";
            this.SkiYearAge.ReadOnly = true;
            this.SkiYearAge.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SkiYearAge.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.SkiYearAge.Width = 40;
            // 
            // Gender
            // 
            this.Gender.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Gender.DataPropertyName = "Gender";
            this.Gender.HeaderText = "M/F";
            this.Gender.Name = "Gender";
            this.Gender.ReadOnly = true;
            this.Gender.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Gender.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Gender.Width = 40;
            // 
            // DateOfBirth
            // 
            this.DateOfBirth.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DateOfBirth.DataPropertyName = "DateOfBirth";
            this.DateOfBirth.HeaderText = "Date Of Birth";
            this.DateOfBirth.Name = "DateOfBirth";
            this.DateOfBirth.ReadOnly = true;
            this.DateOfBirth.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DateOfBirth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.DateOfBirth.Visible = false;
            this.DateOfBirth.Width = 67;
            // 
            // MemberStatus
            // 
            this.MemberStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberStatus.DataPropertyName = "MemberStatus";
            this.MemberStatus.HeaderText = "Status";
            this.MemberStatus.Name = "MemberStatus";
            this.MemberStatus.ReadOnly = true;
            this.MemberStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.MemberStatus.Width = 62;
            // 
            // MemberExpireDate
            // 
            this.MemberExpireDate.DataPropertyName = "MemberExpireDate";
            this.MemberExpireDate.HeaderText = "Expire Date";
            this.MemberExpireDate.Name = "MemberExpireDate";
            this.MemberExpireDate.ReadOnly = true;
            this.MemberExpireDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberExpireDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.MemberExpireDate.Visible = false;
            this.MemberExpireDate.Width = 80;
            // 
            // SlalomOfficialRating
            // 
            this.SlalomOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SlalomOfficialRating.DataPropertyName = "SlalomOfficialRating";
            this.SlalomOfficialRating.HeaderText = "Slalom Official Rating";
            this.SlalomOfficialRating.Name = "SlalomOfficialRating";
            this.SlalomOfficialRating.ReadOnly = true;
            this.SlalomOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SlalomOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.SlalomOfficialRating.Visible = false;
            this.SlalomOfficialRating.Width = 121;
            // 
            // JumpOfficialRating
            // 
            this.JumpOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.JumpOfficialRating.DataPropertyName = "JumpOfficialRating";
            this.JumpOfficialRating.HeaderText = "Jump Official Rating";
            this.JumpOfficialRating.Name = "JumpOfficialRating";
            this.JumpOfficialRating.ReadOnly = true;
            this.JumpOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.JumpOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.JumpOfficialRating.Visible = false;
            this.JumpOfficialRating.Width = 115;
            // 
            // TrickOfficialRating
            // 
            this.TrickOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TrickOfficialRating.DataPropertyName = "TrickOfficialRating";
            this.TrickOfficialRating.HeaderText = "Trick Official Rating";
            this.TrickOfficialRating.Name = "TrickOfficialRating";
            this.TrickOfficialRating.ReadOnly = true;
            this.TrickOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TrickOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.TrickOfficialRating.Visible = false;
            this.TrickOfficialRating.Width = 115;
            // 
            // SafetyOfficialRating
            // 
            this.SafetyOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SafetyOfficialRating.DataPropertyName = "SafetyOfficialRating";
            this.SafetyOfficialRating.HeaderText = "Safety Official Rating";
            this.SafetyOfficialRating.Name = "SafetyOfficialRating";
            this.SafetyOfficialRating.ReadOnly = true;
            this.SafetyOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.SafetyOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.SafetyOfficialRating.Visible = false;
            this.SafetyOfficialRating.Width = 120;
            // 
            // ScoreOfficialRating
            // 
            this.ScoreOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ScoreOfficialRating.DataPropertyName = "ScoreOfficialRating";
            this.ScoreOfficialRating.HeaderText = "Score Official Rating";
            this.ScoreOfficialRating.Name = "ScoreOfficialRating";
            this.ScoreOfficialRating.ReadOnly = true;
            this.ScoreOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ScoreOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ScoreOfficialRating.Visible = false;
            // 
            // TechOfficialRating
            // 
            this.TechOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.TechOfficialRating.DataPropertyName = "TechOfficialRating";
            this.TechOfficialRating.HeaderText = "Tech Official Rating";
            this.TechOfficialRating.Name = "TechOfficialRating";
            this.TechOfficialRating.ReadOnly = true;
            this.TechOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TechOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.TechOfficialRating.Visible = false;
            this.TechOfficialRating.Width = 115;
            // 
            // DriverOfficialRating
            // 
            this.DriverOfficialRating.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.DriverOfficialRating.DataPropertyName = "DriverOfficialRating";
            this.DriverOfficialRating.HeaderText = "Driver Official Rating";
            this.DriverOfficialRating.Name = "DriverOfficialRating";
            this.DriverOfficialRating.ReadOnly = true;
            this.DriverOfficialRating.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DriverOfficialRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.DriverOfficialRating.Visible = false;
            this.DriverOfficialRating.Width = 118;
            // 
            // InsertDate
            // 
            this.InsertDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.InsertDate.DataPropertyName = "InsertDate";
            this.InsertDate.HeaderText = "Insert Date";
            this.InsertDate.Name = "InsertDate";
            this.InsertDate.ReadOnly = true;
            this.InsertDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.InsertDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.InsertDate.Visible = false;
            this.InsertDate.Width = 78;
            // 
            // UpdateDate
            // 
            this.UpdateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.UpdateDate.DataPropertyName = "UpdateDate";
            this.UpdateDate.HeaderText = "Update Date";
            this.UpdateDate.Name = "UpdateDate";
            this.UpdateDate.ReadOnly = true;
            this.UpdateDate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.UpdateDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.UpdateDate.Visible = false;
            this.UpdateDate.Width = 86;
            // 
            // Note
            // 
            this.Note.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Note.DataPropertyName = "Note";
            this.Note.HeaderText = "Note";
            this.Note.Name = "Note";
            this.Note.ReadOnly = true;
            this.Note.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Note.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Note.Visible = false;
            this.Note.Width = 55;
            // 
            // SearchMemberDialogForm
            // 
            this.AcceptButton = this.SelectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 399);
            this.Controls.Add(this.DataGridView);
            this.Controls.Add(this.labelFirstName);
            this.Controls.Add(this.labelLastName);
            this.Controls.Add(this.labelMemberId);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SelectButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.inputFirstName);
            this.Controls.Add(this.inputLastName);
            this.Controls.Add(this.inputMemberId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SearchMemberDialogForm";
            this.Text = "SearchMemberDialogForm";
            this.Load += new System.EventHandler(this.SearchMemberDialogForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.waterskiDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.memberListBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputMemberId;
        private System.Windows.Forms.TextBox inputLastName;
        private System.Windows.Forms.TextBox inputFirstName;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label labelMemberId;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelFirstName;
        private waterskiDataSet waterskiDataSet;
        private System.Windows.Forms.BindingSource memberListBindingSource;
        private WaterskiScoringSystem.waterskiDataSetTableAdapters.MemberListTableAdapter memberListTableAdapter;
        private WaterskiScoringSystem.waterskiDataSetTableAdapters.TableAdapterManager tableAdapterManager;
        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Address2;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
        private System.Windows.Forms.DataGridViewTextBoxColumn Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn PostalCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkiYearAge;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateOfBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberExpireDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlalomOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn JumpOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrickOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn SafetyOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScoreOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn TechOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn DriverOfficialRating;
        private System.Windows.Forms.DataGridViewTextBoxColumn InsertDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdateDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn Note;
    }
}