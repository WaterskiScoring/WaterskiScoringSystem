﻿namespace WaterskiScoringSystem.Trick {
	partial class TrickVideoManagement {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.loadedVideoDataGridView = new System.Windows.Forms.DataGridView();
            this.SelectVideo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.VideoTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoPlays = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoSizeSD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoSizeHD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoURL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VideoId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SortColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MatchButton = new System.Windows.Forms.Button();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.ExportLoadedButton = new System.Windows.Forms.Button();
            this.ViewButton = new System.Windows.Forms.Button();
            this.RowStatusLabel = new System.Windows.Forms.Label();
            this.SanctionTextbox = new System.Windows.Forms.TextBox();
            this.FindDupsButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.loadedVideoDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // loadedVideoDataGridView
            // 
            this.loadedVideoDataGridView.AllowUserToAddRows = false;
            this.loadedVideoDataGridView.AllowUserToDeleteRows = false;
            this.loadedVideoDataGridView.AllowUserToResizeRows = false;
            this.loadedVideoDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.loadedVideoDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.loadedVideoDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.loadedVideoDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectVideo,
            this.VideoTitle,
            this.VideoState,
            this.VideoPlays,
            this.VideoSizeSD,
            this.VideoSizeHD,
            this.CreatedDate,
            this.VideoURL,
            this.VideoId,
            this.SortColumn});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.loadedVideoDataGridView.DefaultCellStyle = dataGridViewCellStyle6;
            this.loadedVideoDataGridView.Location = new System.Drawing.Point(2, 58);
            this.loadedVideoDataGridView.Name = "loadedVideoDataGridView";
            this.loadedVideoDataGridView.RowHeadersVisible = false;
            this.loadedVideoDataGridView.Size = new System.Drawing.Size(795, 390);
            this.loadedVideoDataGridView.TabIndex = 1;
            this.loadedVideoDataGridView.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView_RowEnter);
            // 
            // SelectVideo
            // 
            this.SelectVideo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SelectVideo.FalseValue = "False";
            this.SelectVideo.HeaderText = "Select";
            this.SelectVideo.IndeterminateValue = "False";
            this.SelectVideo.Name = "SelectVideo";
            this.SelectVideo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SelectVideo.TrueValue = "True";
            this.SelectVideo.Width = 35;
            // 
            // VideoTitle
            // 
            this.VideoTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.VideoTitle.HeaderText = "Title";
            this.VideoTitle.Name = "VideoTitle";
            this.VideoTitle.ReadOnly = true;
            this.VideoTitle.Width = 200;
            // 
            // VideoState
            // 
            this.VideoState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoState.DefaultCellStyle = dataGridViewCellStyle2;
            this.VideoState.HeaderText = "Status";
            this.VideoState.Name = "VideoState";
            this.VideoState.ReadOnly = true;
            this.VideoState.Width = 65;
            // 
            // VideoPlays
            // 
            this.VideoPlays.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoPlays.DefaultCellStyle = dataGridViewCellStyle3;
            this.VideoPlays.HeaderText = "Plays";
            this.VideoPlays.Name = "VideoPlays";
            this.VideoPlays.ReadOnly = true;
            this.VideoPlays.Width = 40;
            // 
            // VideoSizeSD
            // 
            this.VideoSizeSD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoSizeSD.DefaultCellStyle = dataGridViewCellStyle4;
            this.VideoSizeSD.HeaderText = "SD Size";
            this.VideoSizeSD.Name = "VideoSizeSD";
            this.VideoSizeSD.ReadOnly = true;
            this.VideoSizeSD.Width = 60;
            // 
            // VideoSizeHD
            // 
            this.VideoSizeHD.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.VideoSizeHD.DefaultCellStyle = dataGridViewCellStyle5;
            this.VideoSizeHD.HeaderText = "HD Size";
            this.VideoSizeHD.Name = "VideoSizeHD";
            this.VideoSizeHD.ReadOnly = true;
            this.VideoSizeHD.Width = 60;
            // 
            // CreatedDate
            // 
            this.CreatedDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.CreatedDate.HeaderText = "Created Date";
            this.CreatedDate.Name = "CreatedDate";
            this.CreatedDate.ReadOnly = true;
            this.CreatedDate.Width = 125;
            // 
            // VideoURL
            // 
            this.VideoURL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.VideoURL.HeaderText = "URL";
            this.VideoURL.Name = "VideoURL";
            this.VideoURL.ReadOnly = true;
            this.VideoURL.Width = 150;
            // 
            // VideoId
            // 
            this.VideoId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.VideoId.HeaderText = "ID";
            this.VideoId.Name = "VideoId";
            this.VideoId.ReadOnly = true;
            this.VideoId.Width = 75;
            // 
            // SortColumn
            // 
            this.SortColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.SortColumn.HeaderText = "Sort";
            this.SortColumn.Name = "SortColumn";
            this.SortColumn.ReadOnly = true;
            this.SortColumn.Width = 300;
            // 
            // MatchButton
            // 
            this.MatchButton.AutoSize = true;
            this.MatchButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.MatchButton.Location = new System.Drawing.Point(469, 6);
            this.MatchButton.Name = "MatchButton";
            this.MatchButton.Size = new System.Drawing.Size(79, 23);
            this.MatchButton.TabIndex = 21;
            this.MatchButton.Text = "Match";
            this.MatchButton.UseVisualStyleBackColor = true;
            this.MatchButton.Click += new System.EventHandler(this.MatchButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.AutoSize = true;
            this.DeleteButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.DeleteButton.Location = new System.Drawing.Point(390, 6);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(79, 23);
            this.DeleteButton.TabIndex = 20;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // ExportLoadedButton
            // 
            this.ExportLoadedButton.Location = new System.Drawing.Point(301, 6);
            this.ExportLoadedButton.Name = "ExportLoadedButton";
            this.ExportLoadedButton.Size = new System.Drawing.Size(89, 23);
            this.ExportLoadedButton.TabIndex = 19;
            this.ExportLoadedButton.Text = "Export Loaded";
            this.ExportLoadedButton.UseVisualStyleBackColor = true;
            this.ExportLoadedButton.Click += new System.EventHandler(this.ExportLoadedButton_Click);
            // 
            // ViewButton
            // 
            this.ViewButton.AutoSize = true;
            this.ViewButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ViewButton.Location = new System.Drawing.Point(222, 6);
            this.ViewButton.Name = "ViewButton";
            this.ViewButton.Size = new System.Drawing.Size(79, 23);
            this.ViewButton.TabIndex = 18;
            this.ViewButton.Text = "View Loaded";
            this.ViewButton.UseVisualStyleBackColor = true;
            this.ViewButton.Click += new System.EventHandler(this.ViewButton_Click);
            // 
            // RowStatusLabel
            // 
            this.RowStatusLabel.AutoSize = true;
            this.RowStatusLabel.Location = new System.Drawing.Point(9, 5);
            this.RowStatusLabel.MinimumSize = new System.Drawing.Size(100, 18);
            this.RowStatusLabel.Name = "RowStatusLabel";
            this.RowStatusLabel.Size = new System.Drawing.Size(100, 18);
            this.RowStatusLabel.TabIndex = 0;
            this.RowStatusLabel.Text = "Row 1 of 0000";
            this.RowStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SanctionTextbox
            // 
            this.SanctionTextbox.Location = new System.Drawing.Point(115, 4);
            this.SanctionTextbox.Name = "SanctionTextbox";
            this.SanctionTextbox.Size = new System.Drawing.Size(100, 20);
            this.SanctionTextbox.TabIndex = 1;
            // 
            // FindDupsButton
            // 
            this.FindDupsButton.AutoSize = true;
            this.FindDupsButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.FindDupsButton.Location = new System.Drawing.Point(550, 6);
            this.FindDupsButton.Name = "FindDupsButton";
            this.FindDupsButton.Size = new System.Drawing.Size(79, 23);
            this.FindDupsButton.TabIndex = 22;
            this.FindDupsButton.Text = "Find Dups";
            this.FindDupsButton.UseVisualStyleBackColor = true;
            this.FindDupsButton.Click += new System.EventHandler(this.FindDupsButton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Location = new System.Drawing.Point(2, 29);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(75, 23);
            this.SelectAllButton.TabIndex = 23;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // TrickVideoManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SelectAllButton);
            this.Controls.Add(this.FindDupsButton);
            this.Controls.Add(this.SanctionTextbox);
            this.Controls.Add(this.RowStatusLabel);
            this.Controls.Add(this.MatchButton);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.ExportLoadedButton);
            this.Controls.Add(this.ViewButton);
            this.Controls.Add(this.loadedVideoDataGridView);
            this.Name = "TrickVideoManagement";
            this.Text = "TrickVideoManagement";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoadVideosFile_FormClosed);
            this.Load += new System.EventHandler(this.TrickVideoManagement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.loadedVideoDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.DataGridView loadedVideoDataGridView;
		private System.Windows.Forms.Button MatchButton;
		private System.Windows.Forms.Button DeleteButton;
		private System.Windows.Forms.Button ExportLoadedButton;
		private System.Windows.Forms.Button ViewButton;
		private System.Windows.Forms.Label RowStatusLabel;
		private System.Windows.Forms.TextBox SanctionTextbox;
		private System.Windows.Forms.Button FindDupsButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectVideo;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoState;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoPlays;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoSizeSD;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoSizeHD;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoURL;
        private System.Windows.Forms.DataGridViewTextBoxColumn VideoId;
        private System.Windows.Forms.DataGridViewTextBoxColumn SortColumn;
        private System.Windows.Forms.Button SelectAllButton;
    }
}