namespace WaterskiScoringSystem.Tournament {
    partial class TourRegAddMember {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TourRegAddMember));
            this.inputMemberId = new System.Windows.Forms.TextBox();
            this.inputLastName = new System.Windows.Forms.TextBox();
            this.inputFirstName = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.labelMemberId = new System.Windows.Forms.Label();
            this.labelLastName = new System.Windows.Forms.Label();
            this.labelFirstName = new System.Windows.Forms.Label();
            this.DataGridView = new System.Windows.Forms.DataGridView();
            this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.City = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SkiYearAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateOfBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MemberStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelState = new System.Windows.Forms.Label();
            this.inputState = new System.Windows.Forms.TextBox();
            this.AddSkierMsg = new System.Windows.Forms.Label();
            this.newMemberButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // inputMemberId
            // 
            this.inputMemberId.Location = new System.Drawing.Point(298, 19);
            this.inputMemberId.Name = "inputMemberId";
            this.inputMemberId.Size = new System.Drawing.Size(100, 20);
            this.inputMemberId.TabIndex = 3;
            this.inputMemberId.Leave += new System.EventHandler(this.inputMemberId_Leave);
            // 
            // inputLastName
            // 
            this.inputLastName.Location = new System.Drawing.Point(70, 19);
            this.inputLastName.Name = "inputLastName";
            this.inputLastName.Size = new System.Drawing.Size(100, 20);
            this.inputLastName.TabIndex = 1;
            this.inputLastName.Leave += new System.EventHandler(this.inputLastName_Leave);
            // 
            // inputFirstName
            // 
            this.inputFirstName.Location = new System.Drawing.Point(184, 19);
            this.inputFirstName.Name = "inputFirstName";
            this.inputFirstName.Size = new System.Drawing.Size(100, 20);
            this.inputFirstName.TabIndex = 2;
            this.inputFirstName.Leave += new System.EventHandler(this.inputFirstName_Leave);
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(85, 42);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(75, 23);
            this.SearchButton.TabIndex = 10;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(176, 42);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(75, 23);
            this.AddButton.TabIndex = 11;
            this.AddButton.Text = "Add";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(363, 42);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 23);
            this.CloseButton.TabIndex = 15;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // labelMemberId
            // 
            this.labelMemberId.AutoSize = true;
            this.labelMemberId.Location = new System.Drawing.Point(321, 6);
            this.labelMemberId.Name = "labelMemberId";
            this.labelMemberId.Size = new System.Drawing.Size(54, 13);
            this.labelMemberId.TabIndex = 0;
            this.labelMemberId.Text = "MemberId";
            this.labelMemberId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelLastName
            // 
            this.labelLastName.AutoSize = true;
            this.labelLastName.Location = new System.Drawing.Point(91, 6);
            this.labelLastName.Name = "labelLastName";
            this.labelLastName.Size = new System.Drawing.Size(58, 13);
            this.labelLastName.TabIndex = 0;
            this.labelLastName.Text = "Last Name";
            this.labelLastName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelFirstName
            // 
            this.labelFirstName.AutoSize = true;
            this.labelFirstName.Location = new System.Drawing.Point(206, 6);
            this.labelFirstName.Name = "labelFirstName";
            this.labelFirstName.Size = new System.Drawing.Size(57, 13);
            this.labelFirstName.TabIndex = 0;
            this.labelFirstName.Text = "First Name";
            this.labelFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.LastName,
            this.FirstName,
            this.City,
            this.State,
            this.Federation,
            this.SkiYearAge,
            this.Gender,
            this.DateOfBirth,
            this.MemberStatus});
            this.DataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DataGridView.Location = new System.Drawing.Point(4, 90);
            this.DataGridView.Name = "DataGridView";
            this.DataGridView.RowHeadersWidth = 31;
            this.DataGridView.Size = new System.Drawing.Size(626, 305);
            this.DataGridView.TabIndex = 20;
            this.DataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_CellContentDoubleClick);
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
            // LastName
            // 
            this.LastName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.LastName.HeaderText = "LastName";
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.LastName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.LastName.Width = 90;
            // 
            // FirstName
            // 
            this.FirstName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FirstName.HeaderText = "FirstName";
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FirstName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.FirstName.Width = 80;
            // 
            // City
            // 
            this.City.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.City.HeaderText = "City";
            this.City.Name = "City";
            this.City.ReadOnly = true;
            this.City.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.City.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.City.Width = 80;
            // 
            // State
            // 
            this.State.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.State.HeaderText = "State";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            this.State.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.State.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.State.Width = 40;
            // 
            // Federation
            // 
            this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Federation.HeaderText = "Fed";
            this.Federation.Name = "Federation";
            this.Federation.ReadOnly = true;
            this.Federation.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Federation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.Federation.Width = 50;
            // 
            // SkiYearAge
            // 
            this.SkiYearAge.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
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
            this.DateOfBirth.HeaderText = "Date Of Birth";
            this.DateOfBirth.Name = "DateOfBirth";
            this.DateOfBirth.ReadOnly = true;
            this.DateOfBirth.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.DateOfBirth.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.DateOfBirth.Visible = false;
            this.DateOfBirth.Width = 75;
            // 
            // MemberStatus
            // 
            this.MemberStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MemberStatus.HeaderText = "Status";
            this.MemberStatus.Name = "MemberStatus";
            this.MemberStatus.ReadOnly = true;
            this.MemberStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.MemberStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.MemberStatus.Width = 75;
            // 
            // labelState
            // 
            this.labelState.AutoSize = true;
            this.labelState.Location = new System.Drawing.Point(416, 6);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(32, 13);
            this.labelState.TabIndex = 11;
            this.labelState.Text = "State";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // inputState
            // 
            this.inputState.Location = new System.Drawing.Point(412, 19);
            this.inputState.MaxLength = 2;
            this.inputState.Name = "inputState";
            this.inputState.Size = new System.Drawing.Size(40, 20);
            this.inputState.TabIndex = 4;
            this.inputState.TextChanged += new System.EventHandler(this.inputState_TextChanged);
            // 
            // AddSkierMsg
            // 
            this.AddSkierMsg.BackColor = System.Drawing.SystemColors.Info;
            this.AddSkierMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddSkierMsg.ForeColor = System.Drawing.SystemColors.Highlight;
            this.AddSkierMsg.Location = new System.Drawing.Point(11, 67);
            this.AddSkierMsg.Name = "AddSkierMsg";
            this.AddSkierMsg.Size = new System.Drawing.Size(570, 20);
            this.AddSkierMsg.TabIndex = 32;
            this.AddSkierMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // newMemberButton
            // 
            this.newMemberButton.AutoSize = true;
            this.newMemberButton.Location = new System.Drawing.Point(267, 42);
            this.newMemberButton.Name = "newMemberButton";
            this.newMemberButton.Size = new System.Drawing.Size(80, 23);
            this.newMemberButton.TabIndex = 12;
            this.newMemberButton.Text = "New Member";
            this.newMemberButton.UseVisualStyleBackColor = true;
            this.newMemberButton.Click += new System.EventHandler(this.newMemberButton_Click);
            // 
            // TourRegAddMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(633, 407);
            this.Controls.Add(this.newMemberButton);
            this.Controls.Add(this.AddSkierMsg);
            this.Controls.Add(this.labelState);
            this.Controls.Add(this.inputState);
            this.Controls.Add(this.DataGridView);
            this.Controls.Add(this.labelFirstName);
            this.Controls.Add(this.labelLastName);
            this.Controls.Add(this.labelMemberId);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.inputFirstName);
            this.Controls.Add(this.inputLastName);
            this.Controls.Add(this.inputMemberId);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TourRegAddMember";
            this.Text = "TourRegAddMember";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TourRegAddMember_FormClosing);
            this.Load += new System.EventHandler(this.TourRegAddMember_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputMemberId;
        private System.Windows.Forms.TextBox inputLastName;
        private System.Windows.Forms.TextBox inputFirstName;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Label labelMemberId;
        private System.Windows.Forms.Label labelLastName;
        private System.Windows.Forms.Label labelFirstName;
        private System.Windows.Forms.DataGridView DataGridView;
        private System.Windows.Forms.Label labelState;
        private System.Windows.Forms.TextBox inputState;
        private System.Windows.Forms.Label AddSkierMsg;
        private System.Windows.Forms.Button newMemberButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn City;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
        private System.Windows.Forms.DataGridViewTextBoxColumn SkiYearAge;
        private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateOfBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn MemberStatus;
    }
}