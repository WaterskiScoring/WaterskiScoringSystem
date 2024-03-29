﻿namespace WaterskiScoringSystem.Admin {
    partial class EditMember {
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
			System.Windows.Forms.Label memberIdLabel;
			System.Windows.Forms.Label lastNameLabel;
			System.Windows.Forms.Label firstNameLabel;
			System.Windows.Forms.Label stateLabel;
			System.Windows.Forms.Label federationLabel;
			System.Windows.Forms.Label skiYearAgeLabel;
			System.Windows.Forms.Label memberStatusLabel;
			System.Windows.Forms.Label insertDateLabel;
			System.Windows.Forms.Label updateDateLabel;
			System.Windows.Forms.Label cityLabel;
			System.Windows.Forms.Label ForeignFederationIDLabel;
			this.label1 = new System.Windows.Forms.Label();
			this.reqdFlagGender = new System.Windows.Forms.Label();
			this.reqdFlagFirstName = new System.Windows.Forms.Label();
			this.reqdFlagLastName = new System.Windows.Forms.Label();
			this.reqdFlagMemberId = new System.Windows.Forms.Label();
			this.editFederation = new System.Windows.Forms.ComboBox();
			this.editUpdateDateShow = new System.Windows.Forms.Label();
			this.editInsertDateShow = new System.Windows.Forms.Label();
			this.editMemberId = new System.Windows.Forms.TextBox();
			this.editLastName = new System.Windows.Forms.TextBox();
			this.editFirstName = new System.Windows.Forms.TextBox();
			this.editState = new System.Windows.Forms.TextBox();
			this.editSkiYearAge = new System.Windows.Forms.TextBox();
			this.saveButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.AgeAsOfLabel = new System.Windows.Forms.Label();
			this.NextAvailableLabel = new System.Windows.Forms.Label();
			this.editGenderSelect = new WaterskiScoringSystem.Common.GenderSelect();
			this.editCity = new System.Windows.Forms.TextBox();
			this.showMemberStatus = new System.Windows.Forms.Label();
			this.editForeignFederationID = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			reqdFlagInfo = new System.Windows.Forms.Label();
			labelGender = new System.Windows.Forms.Label();
			memberIdLabel = new System.Windows.Forms.Label();
			lastNameLabel = new System.Windows.Forms.Label();
			firstNameLabel = new System.Windows.Forms.Label();
			stateLabel = new System.Windows.Forms.Label();
			federationLabel = new System.Windows.Forms.Label();
			skiYearAgeLabel = new System.Windows.Forms.Label();
			memberStatusLabel = new System.Windows.Forms.Label();
			insertDateLabel = new System.Windows.Forms.Label();
			updateDateLabel = new System.Windows.Forms.Label();
			cityLabel = new System.Windows.Forms.Label();
			ForeignFederationIDLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// reqdFlagInfo
			// 
			reqdFlagInfo.ForeColor = System.Drawing.SystemColors.ActiveCaption;
			reqdFlagInfo.Location = new System.Drawing.Point(19, 7);
			reqdFlagInfo.Name = "reqdFlagInfo";
			reqdFlagInfo.Size = new System.Drawing.Size(107, 13);
			reqdFlagInfo.TabIndex = 0;
			reqdFlagInfo.Text = "= required data items";
			reqdFlagInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelGender
			// 
			labelGender.AutoSize = true;
			labelGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			labelGender.Location = new System.Drawing.Point(58, 114);
			labelGender.Name = "labelGender";
			labelGender.Size = new System.Drawing.Size(51, 15);
			labelGender.TabIndex = 0;
			labelGender.Text = "Gender:";
			labelGender.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// memberIdLabel
			// 
			memberIdLabel.AutoSize = true;
			memberIdLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			memberIdLabel.Location = new System.Drawing.Point(39, 34);
			memberIdLabel.Name = "memberIdLabel";
			memberIdLabel.Size = new System.Drawing.Size(70, 15);
			memberIdLabel.TabIndex = 0;
			memberIdLabel.Text = "Member Id:";
			memberIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lastNameLabel
			// 
			lastNameLabel.AutoSize = true;
			lastNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			lastNameLabel.Location = new System.Drawing.Point(39, 86);
			lastNameLabel.Name = "lastNameLabel";
			lastNameLabel.Size = new System.Drawing.Size(70, 15);
			lastNameLabel.TabIndex = 0;
			lastNameLabel.Text = "Last Name:";
			lastNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// firstNameLabel
			// 
			firstNameLabel.AutoSize = true;
			firstNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			firstNameLabel.Location = new System.Drawing.Point(39, 60);
			firstNameLabel.Name = "firstNameLabel";
			firstNameLabel.Size = new System.Drawing.Size(70, 15);
			firstNameLabel.TabIndex = 0;
			firstNameLabel.Text = "First Name:";
			firstNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// stateLabel
			// 
			stateLabel.AutoSize = true;
			stateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			stateLabel.Location = new System.Drawing.Point(71, 194);
			stateLabel.Name = "stateLabel";
			stateLabel.Size = new System.Drawing.Size(38, 15);
			stateLabel.TabIndex = 0;
			stateLabel.Text = "State:";
			stateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// federationLabel
			// 
			federationLabel.AutoSize = true;
			federationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			federationLabel.Location = new System.Drawing.Point(40, 248);
			federationLabel.Name = "federationLabel";
			federationLabel.Size = new System.Drawing.Size(69, 15);
			federationLabel.TabIndex = 0;
			federationLabel.Text = "Federation:";
			federationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// skiYearAgeLabel
			// 
			skiYearAgeLabel.AutoSize = true;
			skiYearAgeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			skiYearAgeLabel.Location = new System.Drawing.Point(33, 138);
			skiYearAgeLabel.Name = "skiYearAgeLabel";
			skiYearAgeLabel.Size = new System.Drawing.Size(79, 15);
			skiYearAgeLabel.TabIndex = 0;
			skiYearAgeLabel.Text = "Ski Year Age:";
			skiYearAgeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// memberStatusLabel
			// 
			memberStatusLabel.AutoSize = true;
			memberStatusLabel.CausesValidation = false;
			memberStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			memberStatusLabel.Location = new System.Drawing.Point(15, 169);
			memberStatusLabel.Margin = new System.Windows.Forms.Padding(0);
			memberStatusLabel.Name = "memberStatusLabel";
			memberStatusLabel.Size = new System.Drawing.Size(94, 15);
			memberStatusLabel.TabIndex = 0;
			memberStatusLabel.Text = "Member Status:";
			memberStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// insertDateLabel
			// 
			insertDateLabel.AutoSize = true;
			insertDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			insertDateLabel.Location = new System.Drawing.Point(40, 304);
			insertDateLabel.Name = "insertDateLabel";
			insertDateLabel.Size = new System.Drawing.Size(69, 15);
			insertDateLabel.TabIndex = 0;
			insertDateLabel.Text = "Insert Date:";
			insertDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// updateDateLabel
			// 
			updateDateLabel.AutoSize = true;
			updateDateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			updateDateLabel.Location = new System.Drawing.Point(30, 332);
			updateDateLabel.Name = "updateDateLabel";
			updateDateLabel.Size = new System.Drawing.Size(79, 15);
			updateDateLabel.TabIndex = 0;
			updateDateLabel.Text = "Update Date:";
			updateDateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// cityLabel
			// 
			cityLabel.AutoSize = true;
			cityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			cityLabel.Location = new System.Drawing.Point(83, 221);
			cityLabel.Name = "cityLabel";
			cityLabel.Size = new System.Drawing.Size(26, 15);
			cityLabel.TabIndex = 0;
			cityLabel.Text = "City";
			cityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ForeignFederationIDLabel
			// 
			ForeignFederationIDLabel.AutoSize = true;
			ForeignFederationIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			ForeignFederationIDLabel.Location = new System.Drawing.Point(18, 278);
			ForeignFederationIDLabel.Name = "ForeignFederationIDLabel";
			ForeignFederationIDLabel.Size = new System.Drawing.Size(91, 15);
			ForeignFederationIDLabel.TabIndex = 0;
			ForeignFederationIDLabel.Text = "Foreign Fed ID:";
			ForeignFederationIDLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(6, 7);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(10, 12);
			this.label1.TabIndex = 90;
			this.label1.Text = "*";
			// 
			// reqdFlagGender
			// 
			this.reqdFlagGender.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagGender.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagGender.Location = new System.Drawing.Point(49, 114);
			this.reqdFlagGender.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagGender.Name = "reqdFlagGender";
			this.reqdFlagGender.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagGender.TabIndex = 0;
			this.reqdFlagGender.Text = "*";
			// 
			// reqdFlagFirstName
			// 
			this.reqdFlagFirstName.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagFirstName.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagFirstName.Location = new System.Drawing.Point(31, 60);
			this.reqdFlagFirstName.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagFirstName.Name = "reqdFlagFirstName";
			this.reqdFlagFirstName.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagFirstName.TabIndex = 0;
			this.reqdFlagFirstName.Text = "*";
			// 
			// reqdFlagLastName
			// 
			this.reqdFlagLastName.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagLastName.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagLastName.Location = new System.Drawing.Point(32, 87);
			this.reqdFlagLastName.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagLastName.Name = "reqdFlagLastName";
			this.reqdFlagLastName.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagLastName.TabIndex = 0;
			this.reqdFlagLastName.Text = "*";
			// 
			// reqdFlagMemberId
			// 
			this.reqdFlagMemberId.BackColor = System.Drawing.Color.Transparent;
			this.reqdFlagMemberId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.reqdFlagMemberId.ForeColor = System.Drawing.Color.Red;
			this.reqdFlagMemberId.Location = new System.Drawing.Point(32, 33);
			this.reqdFlagMemberId.Margin = new System.Windows.Forms.Padding(0);
			this.reqdFlagMemberId.Name = "reqdFlagMemberId";
			this.reqdFlagMemberId.Size = new System.Drawing.Size(10, 12);
			this.reqdFlagMemberId.TabIndex = 0;
			this.reqdFlagMemberId.Text = "*";
			// 
			// editFederation
			// 
			this.editFederation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editFederation.FormattingEnabled = true;
			this.editFederation.Location = new System.Drawing.Point(116, 244);
			this.editFederation.Name = "editFederation";
			this.editFederation.Size = new System.Drawing.Size(125, 23);
			this.editFederation.TabIndex = 90;
			// 
			// editUpdateDateShow
			// 
			this.editUpdateDateShow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.editUpdateDateShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editUpdateDateShow.Location = new System.Drawing.Point(116, 329);
			this.editUpdateDateShow.Name = "editUpdateDateShow";
			this.editUpdateDateShow.Size = new System.Drawing.Size(125, 20);
			this.editUpdateDateShow.TabIndex = 0;
			this.editUpdateDateShow.Text = "00/00/00 00:00:00";
			this.editUpdateDateShow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editInsertDateShow
			// 
			this.editInsertDateShow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.editInsertDateShow.CausesValidation = false;
			this.editInsertDateShow.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editInsertDateShow.Location = new System.Drawing.Point(116, 301);
			this.editInsertDateShow.Name = "editInsertDateShow";
			this.editInsertDateShow.Size = new System.Drawing.Size(125, 20);
			this.editInsertDateShow.TabIndex = 0;
			this.editInsertDateShow.Text = "00/00/00 00:00:00";
			this.editInsertDateShow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editMemberId
			// 
			this.editMemberId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editMemberId.Location = new System.Drawing.Point(116, 31);
			this.editMemberId.MaxLength = 9;
			this.editMemberId.Name = "editMemberId";
			this.editMemberId.Size = new System.Drawing.Size(75, 21);
			this.editMemberId.TabIndex = 10;
			this.editMemberId.WordWrap = false;
			// 
			// editLastName
			// 
			this.editLastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editLastName.Location = new System.Drawing.Point(116, 83);
			this.editLastName.MaxLength = 128;
			this.editLastName.Name = "editLastName";
			this.editLastName.Size = new System.Drawing.Size(125, 21);
			this.editLastName.TabIndex = 30;
			this.editLastName.WordWrap = false;
			// 
			// editFirstName
			// 
			this.editFirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editFirstName.Location = new System.Drawing.Point(116, 57);
			this.editFirstName.MaxLength = 128;
			this.editFirstName.Name = "editFirstName";
			this.editFirstName.Size = new System.Drawing.Size(125, 21);
			this.editFirstName.TabIndex = 20;
			// 
			// editState
			// 
			this.editState.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editState.Location = new System.Drawing.Point(116, 191);
			this.editState.MaxLength = 2;
			this.editState.Name = "editState";
			this.editState.Size = new System.Drawing.Size(30, 21);
			this.editState.TabIndex = 80;
			this.editState.WordWrap = false;
			// 
			// editSkiYearAge
			// 
			this.editSkiYearAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editSkiYearAge.Location = new System.Drawing.Point(116, 135);
			this.editSkiYearAge.MaxLength = 3;
			this.editSkiYearAge.Name = "editSkiYearAge";
			this.editSkiYearAge.Size = new System.Drawing.Size(30, 21);
			this.editSkiYearAge.TabIndex = 50;
			this.editSkiYearAge.WordWrap = false;
			this.editSkiYearAge.Validated += new System.EventHandler(this.editSkiYearAge_Validated);
			// 
			// saveButton
			// 
			this.saveButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.saveButton.Location = new System.Drawing.Point(51, 360);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(75, 23);
			this.saveButton.TabIndex = 100;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(151, 360);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 105;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// AgeAsOfLabel
			// 
			this.AgeAsOfLabel.AutoSize = true;
			this.AgeAsOfLabel.Location = new System.Drawing.Point(152, 139);
			this.AgeAsOfLabel.Name = "AgeAsOfLabel";
			this.AgeAsOfLabel.Size = new System.Drawing.Size(100, 13);
			this.AgeAsOfLabel.TabIndex = 0;
			this.AgeAsOfLabel.Text = "age as of 1/1/2010";
			// 
			// NextAvailableLabel
			// 
			this.NextAvailableLabel.ForeColor = System.Drawing.SystemColors.WindowText;
			this.NextAvailableLabel.Location = new System.Drawing.Point(131, 7);
			this.NextAvailableLabel.Name = "NextAvailableLabel";
			this.NextAvailableLabel.Size = new System.Drawing.Size(160, 13);
			this.NextAvailableLabel.TabIndex = 0;
			this.NextAvailableLabel.Text = "Next Temp Number: ";
			this.NextAvailableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editGenderSelect
			// 
			this.editGenderSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editGenderSelect.Location = new System.Drawing.Point(116, 107);
			this.editGenderSelect.Margin = new System.Windows.Forms.Padding(0);
			this.editGenderSelect.Name = "editGenderSelect";
			this.editGenderSelect.RatingValue = "";
			this.editGenderSelect.Size = new System.Drawing.Size(145, 29);
			this.editGenderSelect.TabIndex = 40;
			this.editGenderSelect.Tag = "";
			this.editGenderSelect.Load += new System.EventHandler(this.editGenderSelect_Load);
			// 
			// editCity
			// 
			this.editCity.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editCity.Location = new System.Drawing.Point(116, 218);
			this.editCity.MaxLength = 129;
			this.editCity.Name = "editCity";
			this.editCity.Size = new System.Drawing.Size(120, 21);
			this.editCity.TabIndex = 85;
			this.editCity.WordWrap = false;
			// 
			// showMemberStatus
			// 
			this.showMemberStatus.AutoEllipsis = true;
			this.showMemberStatus.BackColor = System.Drawing.SystemColors.ControlLight;
			this.showMemberStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.showMemberStatus.CausesValidation = false;
			this.showMemberStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.showMemberStatus.Location = new System.Drawing.Point(117, 166);
			this.showMemberStatus.Margin = new System.Windows.Forms.Padding(0);
			this.showMemberStatus.Name = "showMemberStatus";
			this.showMemberStatus.Size = new System.Drawing.Size(324, 20);
			this.showMemberStatus.TabIndex = 60;
			this.showMemberStatus.Text = "Active ";
			this.showMemberStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// editForeignFederationID
			// 
			this.editForeignFederationID.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.editForeignFederationID.Location = new System.Drawing.Point(116, 275);
			this.editForeignFederationID.MaxLength = 9;
			this.editForeignFederationID.Name = "editForeignFederationID";
			this.editForeignFederationID.Size = new System.Drawing.Size(75, 21);
			this.editForeignFederationID.TabIndex = 95;
			this.editForeignFederationID.WordWrap = false;
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.Red;
			this.label2.Location = new System.Drawing.Point(25, 138);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(10, 12);
			this.label2.TabIndex = 106;
			this.label2.Text = "*";
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Red;
			this.label3.Location = new System.Drawing.Point(34, 248);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(10, 12);
			this.label3.TabIndex = 107;
			this.label3.Text = "*";
			// 
			// EditMember
			// 
			this.AcceptButton = this.saveButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(444, 394);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(ForeignFederationIDLabel);
			this.Controls.Add(this.editForeignFederationID);
			this.Controls.Add(this.showMemberStatus);
			this.Controls.Add(cityLabel);
			this.Controls.Add(this.editCity);
			this.Controls.Add(this.NextAvailableLabel);
			this.Controls.Add(this.AgeAsOfLabel);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(reqdFlagInfo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.reqdFlagGender);
			this.Controls.Add(this.reqdFlagFirstName);
			this.Controls.Add(this.reqdFlagLastName);
			this.Controls.Add(this.reqdFlagMemberId);
			this.Controls.Add(labelGender);
			this.Controls.Add(this.editFederation);
			this.Controls.Add(this.editUpdateDateShow);
			this.Controls.Add(this.editInsertDateShow);
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
			this.Controls.Add(insertDateLabel);
			this.Controls.Add(updateDateLabel);
			this.Controls.Add(this.editGenderSelect);
			this.Name = "EditMember";
			this.Text = "EditMember";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EditMember_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EditMember_FormClosed);
			this.Load += new System.EventHandler(this.EditMember_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label reqdFlagGender;
        private System.Windows.Forms.Label reqdFlagFirstName;
        private System.Windows.Forms.Label reqdFlagLastName;
        private System.Windows.Forms.Label reqdFlagMemberId;
        private System.Windows.Forms.ComboBox editFederation;
        private System.Windows.Forms.Label editUpdateDateShow;
        private System.Windows.Forms.Label editInsertDateShow;
        private System.Windows.Forms.TextBox editMemberId;
        private System.Windows.Forms.TextBox editLastName;
        private System.Windows.Forms.TextBox editFirstName;
        private System.Windows.Forms.TextBox editState;
        private System.Windows.Forms.TextBox editSkiYearAge;
        private WaterskiScoringSystem.Common.GenderSelect editGenderSelect;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label AgeAsOfLabel;
        internal System.Windows.Forms.Label NextAvailableLabel;
        private System.Windows.Forms.TextBox editCity;
		private System.Windows.Forms.Label showMemberStatus;
		private System.Windows.Forms.TextBox editForeignFederationID;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}