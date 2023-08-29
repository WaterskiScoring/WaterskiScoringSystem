
namespace WaterskiScoringSystem.Externalnterface {
	partial class IwwfScoresExport {
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
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.LastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MemberId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Federation = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Gender = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SanctionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SlalomScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TrickScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.JumpScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AltScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SkierYob = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EventClass = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Round = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Div = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PerfQual1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PerfQual2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TourEndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SeniorOrJunior = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ExportFlag = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SlalomMiss = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Plcmt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IwwfAthleteId = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TourSiteCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ExportButton = new System.Windows.Forms.Button();
			this.RowStatusLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView
			// 
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LastName,
            this.FirstName,
            this.MemberId,
            this.Federation,
            this.Gender,
            this.SanctionId,
            this.SlalomScore,
            this.TrickScore,
            this.JumpScore,
            this.AltScore,
            this.SkierYob,
            this.EventClass,
            this.Round,
            this.Div,
            this.PerfQual1,
            this.PerfQual2,
            this.TourEndDate,
            this.SeniorOrJunior,
            this.ExportFlag,
            this.SlalomMiss,
            this.Plcmt,
            this.IwwfAthleteId,
            this.TourSiteCode});
			this.dataGridView.Location = new System.Drawing.Point(7, 43);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.Size = new System.Drawing.Size(1159, 508);
			this.dataGridView.TabIndex = 0;
			this.dataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_RowEnter);
			// 
			// LastName
			// 
			this.LastName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.LastName.HeaderText = "LastName";
			this.LastName.Name = "LastName";
			this.LastName.ReadOnly = true;
			this.LastName.Width = 75;
			// 
			// FirstName
			// 
			this.FirstName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.FirstName.HeaderText = "FirstName";
			this.FirstName.Name = "FirstName";
			this.FirstName.ReadOnly = true;
			this.FirstName.Width = 75;
			// 
			// MemberId
			// 
			this.MemberId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.MemberId.HeaderText = "MemberId";
			this.MemberId.Name = "MemberId";
			this.MemberId.ReadOnly = true;
			this.MemberId.Width = 70;
			// 
			// Federation
			// 
			this.Federation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Federation.HeaderText = "Fed";
			this.Federation.Name = "Federation";
			this.Federation.ReadOnly = true;
			this.Federation.Width = 35;
			// 
			// Gender
			// 
			this.Gender.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Gender.HeaderText = "Sex";
			this.Gender.Name = "Gender";
			this.Gender.ReadOnly = true;
			this.Gender.Width = 25;
			// 
			// SanctionId
			// 
			this.SanctionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SanctionId.HeaderText = "SanctionId";
			this.SanctionId.Name = "SanctionId";
			this.SanctionId.ReadOnly = true;
			this.SanctionId.Width = 70;
			// 
			// SlalomScore
			// 
			this.SlalomScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SlalomScore.HeaderText = "Slalom Score";
			this.SlalomScore.Name = "SlalomScore";
			this.SlalomScore.ReadOnly = true;
			this.SlalomScore.Width = 50;
			// 
			// TrickScore
			// 
			this.TrickScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TrickScore.HeaderText = "Trick Score";
			this.TrickScore.Name = "TrickScore";
			this.TrickScore.ReadOnly = true;
			this.TrickScore.Width = 50;
			// 
			// JumpScore
			// 
			this.JumpScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.JumpScore.HeaderText = "Jump Score";
			this.JumpScore.Name = "JumpScore";
			this.JumpScore.ReadOnly = true;
			this.JumpScore.Width = 45;
			// 
			// AltScore
			// 
			this.AltScore.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.AltScore.HeaderText = "Alt Score";
			this.AltScore.Name = "AltScore";
			this.AltScore.ReadOnly = true;
			this.AltScore.Width = 40;
			// 
			// SkierYob
			// 
			this.SkierYob.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SkierYob.HeaderText = "Skier Yob";
			this.SkierYob.Name = "SkierYob";
			this.SkierYob.ReadOnly = true;
			this.SkierYob.Width = 35;
			// 
			// EventClass
			// 
			this.EventClass.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.EventClass.HeaderText = "Event Class";
			this.EventClass.Name = "EventClass";
			this.EventClass.ReadOnly = true;
			this.EventClass.Width = 35;
			// 
			// Round
			// 
			this.Round.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Round.HeaderText = "Rd";
			this.Round.Name = "Round";
			this.Round.ReadOnly = true;
			this.Round.Width = 20;
			// 
			// Div
			// 
			this.Div.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Div.HeaderText = "Div";
			this.Div.Name = "Div";
			this.Div.ReadOnly = true;
			this.Div.Width = 30;
			// 
			// PerfQual1
			// 
			this.PerfQual1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PerfQual1.HeaderText = "Perf Qual1";
			this.PerfQual1.Name = "PerfQual1";
			this.PerfQual1.ReadOnly = true;
			this.PerfQual1.Width = 45;
			// 
			// PerfQual2
			// 
			this.PerfQual2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.PerfQual2.HeaderText = "Perf Qual2";
			this.PerfQual2.Name = "PerfQual2";
			this.PerfQual2.ReadOnly = true;
			this.PerfQual2.Width = 45;
			// 
			// TourEndDate
			// 
			this.TourEndDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TourEndDate.HeaderText = "End Date";
			this.TourEndDate.Name = "TourEndDate";
			this.TourEndDate.ReadOnly = true;
			this.TourEndDate.Width = 60;
			// 
			// SeniorOrJunior
			// 
			this.SeniorOrJunior.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SeniorOrJunior.HeaderText = "Senior Junior";
			this.SeniorOrJunior.Name = "SeniorOrJunior";
			this.SeniorOrJunior.ReadOnly = true;
			this.SeniorOrJunior.Width = 40;
			// 
			// ExportFlag
			// 
			this.ExportFlag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ExportFlag.HeaderText = "Export Flag";
			this.ExportFlag.Name = "ExportFlag";
			this.ExportFlag.ReadOnly = true;
			this.ExportFlag.Width = 35;
			// 
			// SlalomMiss
			// 
			this.SlalomMiss.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.SlalomMiss.HeaderText = "Slalom Miss";
			this.SlalomMiss.Name = "SlalomMiss";
			this.SlalomMiss.ReadOnly = true;
			this.SlalomMiss.Width = 40;
			// 
			// Plcmt
			// 
			this.Plcmt.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.Plcmt.HeaderText = "Plcmt";
			this.Plcmt.Name = "Plcmt";
			this.Plcmt.ReadOnly = true;
			this.Plcmt.Width = 30;
			// 
			// IwwfAthleteId
			// 
			this.IwwfAthleteId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.IwwfAthleteId.HeaderText = "Iwwf AthleteId";
			this.IwwfAthleteId.Name = "IwwfAthleteId";
			this.IwwfAthleteId.ReadOnly = true;
			this.IwwfAthleteId.Width = 75;
			// 
			// TourSiteCode
			// 
			this.TourSiteCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TourSiteCode.HeaderText = "Tour Site Code";
			this.TourSiteCode.Name = "TourSiteCode";
			this.TourSiteCode.ReadOnly = true;
			this.TourSiteCode.Width = 50;
			// 
			// ExportButton
			// 
			this.ExportButton.Location = new System.Drawing.Point(9, 12);
			this.ExportButton.Name = "ExportButton";
			this.ExportButton.Size = new System.Drawing.Size(75, 23);
			this.ExportButton.TabIndex = 4;
			this.ExportButton.Text = "Export";
			this.ExportButton.UseVisualStyleBackColor = true;
			this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
			// 
			// RowStatusLabel
			// 
			this.RowStatusLabel.AutoSize = true;
			this.RowStatusLabel.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.RowStatusLabel.Location = new System.Drawing.Point(104, 16);
			this.RowStatusLabel.Name = "RowStatusLabel";
			this.RowStatusLabel.Size = new System.Drawing.Size(106, 14);
			this.RowStatusLabel.TabIndex = 5;
			this.RowStatusLabel.Text = "Row 1 of 9999";
			// 
			// IwwfScoresExport
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1169, 556);
			this.Controls.Add(this.RowStatusLabel);
			this.Controls.Add(this.ExportButton);
			this.Controls.Add(this.dataGridView);
			this.Name = "IwwfScoresExport";
			this.Text = "Iwwf Scores Export";
			this.Load += new System.EventHandler(this.IwwfScoresExport_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastName;
		private System.Windows.Forms.DataGridViewTextBoxColumn FirstName;
		private System.Windows.Forms.DataGridViewTextBoxColumn MemberId;
		private System.Windows.Forms.DataGridViewTextBoxColumn Federation;
		private System.Windows.Forms.DataGridViewTextBoxColumn Gender;
		private System.Windows.Forms.DataGridViewTextBoxColumn SanctionId;
		private System.Windows.Forms.DataGridViewTextBoxColumn SlalomScore;
		private System.Windows.Forms.DataGridViewTextBoxColumn TrickScore;
		private System.Windows.Forms.DataGridViewTextBoxColumn JumpScore;
		private System.Windows.Forms.DataGridViewTextBoxColumn AltScore;
		private System.Windows.Forms.DataGridViewTextBoxColumn SkierYob;
		private System.Windows.Forms.DataGridViewTextBoxColumn EventClass;
		private System.Windows.Forms.DataGridViewTextBoxColumn Round;
		private System.Windows.Forms.DataGridViewTextBoxColumn Div;
		private System.Windows.Forms.DataGridViewTextBoxColumn PerfQual1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PerfQual2;
		private System.Windows.Forms.DataGridViewTextBoxColumn TourEndDate;
		private System.Windows.Forms.DataGridViewTextBoxColumn SeniorOrJunior;
		private System.Windows.Forms.DataGridViewTextBoxColumn ExportFlag;
		private System.Windows.Forms.DataGridViewTextBoxColumn SlalomMiss;
		private System.Windows.Forms.DataGridViewTextBoxColumn Plcmt;
		private System.Windows.Forms.DataGridViewTextBoxColumn IwwfAthleteId;
		private System.Windows.Forms.DataGridViewTextBoxColumn TourSiteCode;
		private System.Windows.Forms.Button ExportButton;
		private System.Windows.Forms.Label RowStatusLabel;
	}
}