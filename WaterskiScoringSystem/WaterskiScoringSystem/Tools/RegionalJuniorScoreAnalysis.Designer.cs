namespace WaterskiScoringSystem.Tools {
	partial class RegionalJuniorScoreAnalysis {
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
			this.dataGridView = new System.Windows.Forms.DataGridView();
			this.ExportButton = new System.Windows.Forms.Button();
			this.WindowInstructionsLabel = new System.Windows.Forms.Label();
			this.ExecAvgSameDivButton = new System.Windows.Forms.Button();
			this.MessageLabel = new System.Windows.Forms.Label();
			this.ExecAvgDiffDivButton = new System.Windows.Forms.Button();
			this.ExecAvgCurYearOnlyButton = new System.Windows.Forms.Button();
			this.ExecAvgMostImprovedButton = new System.Windows.Forms.Button();
			this.ExecAvgMostImprovedNewDivButton = new System.Windows.Forms.Button();
			this.WindowSubInstructions = new System.Windows.Forms.Label();
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
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView.Location = new System.Drawing.Point(10, 147);
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.Size = new System.Drawing.Size(1002, 516);
			this.dataGridView.TabIndex = 20;
			this.dataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridView_DataError);
			// 
			// ExportButton
			// 
			this.ExportButton.Location = new System.Drawing.Point(794, 57);
			this.ExportButton.Name = "ExportButton";
			this.ExportButton.Size = new System.Drawing.Size(75, 23);
			this.ExportButton.TabIndex = 6;
			this.ExportButton.Text = "Export";
			this.ExportButton.UseVisualStyleBackColor = true;
			this.ExportButton.Click += new System.EventHandler(this.ExportButton_Click);
			// 
			// WindowInstructionsLabel
			// 
			this.WindowInstructionsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.WindowInstructionsLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WindowInstructionsLabel.Location = new System.Drawing.Point(10, 5);
			this.WindowInstructionsLabel.Name = "WindowInstructionsLabel";
			this.WindowInstructionsLabel.Size = new System.Drawing.Size(1002, 23);
			this.WindowInstructionsLabel.TabIndex = 0;
			this.WindowInstructionsLabel.Text = "Curent tournament must be current year\'s regional sanction";
			this.WindowInstructionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// ExecAvgSameDivButton
			// 
			this.ExecAvgSameDivButton.AutoSize = true;
			this.ExecAvgSameDivButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExecAvgSameDivButton.Location = new System.Drawing.Point(10, 55);
			this.ExecAvgSameDivButton.Name = "ExecAvgSameDivButton";
			this.ExecAvgSameDivButton.Size = new System.Drawing.Size(212, 26);
			this.ExecAvgSameDivButton.TabIndex = 1;
			this.ExecAvgSameDivButton.Text = "Get Ranking Average Same Division";
			this.ExecAvgSameDivButton.UseVisualStyleBackColor = true;
			this.ExecAvgSameDivButton.Click += new System.EventHandler(this.ExecAvgSameDivButton_Click);
			// 
			// MessageLabel
			// 
			this.MessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.BackColor = System.Drawing.SystemColors.Info;
			this.MessageLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.MessageLabel.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MessageLabel.Location = new System.Drawing.Point(13, 117);
			this.MessageLabel.Name = "MessageLabel";
			this.MessageLabel.Size = new System.Drawing.Size(17, 22);
			this.MessageLabel.TabIndex = 0;
			this.MessageLabel.Text = "x";
			// 
			// ExecAvgDiffDivButton
			// 
			this.ExecAvgDiffDivButton.AutoSize = true;
			this.ExecAvgDiffDivButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExecAvgDiffDivButton.Location = new System.Drawing.Point(10, 83);
			this.ExecAvgDiffDivButton.Name = "ExecAvgDiffDivButton";
			this.ExecAvgDiffDivButton.Size = new System.Drawing.Size(212, 26);
			this.ExecAvgDiffDivButton.TabIndex = 2;
			this.ExecAvgDiffDivButton.Text = "Get Ranking Average New Division";
			this.ExecAvgDiffDivButton.UseVisualStyleBackColor = true;
			this.ExecAvgDiffDivButton.Click += new System.EventHandler(this.ExecAvgDiffDivButton_Click);
			// 
			// ExecAvgCurYearOnlyButton
			// 
			this.ExecAvgCurYearOnlyButton.AutoSize = true;
			this.ExecAvgCurYearOnlyButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExecAvgCurYearOnlyButton.Location = new System.Drawing.Point(488, 57);
			this.ExecAvgCurYearOnlyButton.Name = "ExecAvgCurYearOnlyButton";
			this.ExecAvgCurYearOnlyButton.Size = new System.Drawing.Size(274, 26);
			this.ExecAvgCurYearOnlyButton.TabIndex = 5;
			this.ExecAvgCurYearOnlyButton.Text = "Get Ranking Average Current Year  Not Last Year";
			this.ExecAvgCurYearOnlyButton.UseVisualStyleBackColor = true;
			this.ExecAvgCurYearOnlyButton.Click += new System.EventHandler(this.ExecAvgCurYearOnlyButton_Click);
			// 
			// ExecAvgMostImprovedButton
			// 
			this.ExecAvgMostImprovedButton.AutoSize = true;
			this.ExecAvgMostImprovedButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExecAvgMostImprovedButton.Location = new System.Drawing.Point(254, 54);
			this.ExecAvgMostImprovedButton.Name = "ExecAvgMostImprovedButton";
			this.ExecAvgMostImprovedButton.Size = new System.Drawing.Size(202, 26);
			this.ExecAvgMostImprovedButton.TabIndex = 3;
			this.ExecAvgMostImprovedButton.Text = "Most Improved Data Same Division";
			this.ExecAvgMostImprovedButton.UseVisualStyleBackColor = true;
			this.ExecAvgMostImprovedButton.Click += new System.EventHandler(this.ExecAvgMostImprovedButton_Click);
			// 
			// ExecAvgMostImprovedNewDivButton
			// 
			this.ExecAvgMostImprovedNewDivButton.AutoSize = true;
			this.ExecAvgMostImprovedNewDivButton.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ExecAvgMostImprovedNewDivButton.Location = new System.Drawing.Point(254, 86);
			this.ExecAvgMostImprovedNewDivButton.Name = "ExecAvgMostImprovedNewDivButton";
			this.ExecAvgMostImprovedNewDivButton.Size = new System.Drawing.Size(202, 26);
			this.ExecAvgMostImprovedNewDivButton.TabIndex = 4;
			this.ExecAvgMostImprovedNewDivButton.Text = "Most Improved Data New Division";
			this.ExecAvgMostImprovedNewDivButton.UseVisualStyleBackColor = true;
			this.ExecAvgMostImprovedNewDivButton.Click += new System.EventHandler(this.ExecAvgMostImprovedNewDivButton_Click);
			// 
			// WindowSubInstructions
			// 
			this.WindowSubInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.WindowSubInstructions.Location = new System.Drawing.Point(10, 30);
			this.WindowSubInstructions.Name = "WindowSubInstructions";
			this.WindowSubInstructions.Size = new System.Drawing.Size(1002, 23);
			this.WindowSubInstructions.TabIndex = 0;
			this.WindowSubInstructions.Text = " Regionals from last year must also be loaded on current computer";
			this.WindowSubInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// RegionalJuniorScoreAnalysis
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1026, 675);
			this.Controls.Add(this.WindowSubInstructions);
			this.Controls.Add(this.ExecAvgMostImprovedNewDivButton);
			this.Controls.Add(this.ExecAvgMostImprovedButton);
			this.Controls.Add(this.ExecAvgCurYearOnlyButton);
			this.Controls.Add(this.ExecAvgDiffDivButton);
			this.Controls.Add(this.MessageLabel);
			this.Controls.Add(this.ExportButton);
			this.Controls.Add(this.WindowInstructionsLabel);
			this.Controls.Add(this.ExecAvgSameDivButton);
			this.Controls.Add(this.dataGridView);
			this.Name = "RegionalJuniorScoreAnalysis";
			this.Text = "RegionalJuniorScoreAnalysis";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RegionalJuniorScoreAnalysis_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RegionalJuniorScoreAnalysis_FormClosed);
			this.Load += new System.EventHandler(this.RegionalJuniorScoreAnalysis_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView;
		private System.Windows.Forms.Button ExportButton;
		private System.Windows.Forms.Label WindowInstructionsLabel;
		private System.Windows.Forms.Button ExecAvgSameDivButton;
		private System.Windows.Forms.Label MessageLabel;
		private System.Windows.Forms.Button ExecAvgDiffDivButton;
		private System.Windows.Forms.Button ExecAvgCurYearOnlyButton;
		private System.Windows.Forms.Button ExecAvgMostImprovedButton;
		private System.Windows.Forms.Button ExecAvgMostImprovedNewDivButton;
		private System.Windows.Forms.Label WindowSubInstructions;
	}
}