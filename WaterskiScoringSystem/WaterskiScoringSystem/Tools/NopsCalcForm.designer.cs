namespace WaterskiScoringSystem.Tools {
    partial class NopsCalcForm {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.listAgeDivisionsComboBox = new System.Windows.Forms.ComboBox();
            this.DivisionLabel = new System.Windows.Forms.Label();
            this.Event1 = new System.Windows.Forms.Label();
            this.Nops1 = new System.Windows.Forms.Label();
            this.Score1 = new System.Windows.Forms.TextBox();
            this.Score2 = new System.Windows.Forms.TextBox();
            this.Nops2 = new System.Windows.Forms.Label();
            this.Event2 = new System.Windows.Forms.Label();
            this.Score3 = new System.Windows.Forms.TextBox();
            this.Nops3 = new System.Windows.Forms.Label();
            this.Event3 = new System.Windows.Forms.Label();
            this.Nops4 = new System.Windows.Forms.Label();
            this.Event4 = new System.Windows.Forms.Label();
            this.SlalomScoreDesc = new System.Windows.Forms.Label();
            this.printButton = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.AgeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RatingRec = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RatingMedian = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Base = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OverallBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Adj = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OverallExp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EventsReqd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // listAgeDivisionsComboBox
            // 
            this.listAgeDivisionsComboBox.CausesValidation = false;
            this.listAgeDivisionsComboBox.DisplayMember = "Division";
            this.listAgeDivisionsComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.listAgeDivisionsComboBox.FormattingEnabled = true;
            this.listAgeDivisionsComboBox.Location = new System.Drawing.Point(6, 26);
            this.listAgeDivisionsComboBox.Name = "listAgeDivisionsComboBox";
            this.listAgeDivisionsComboBox.Size = new System.Drawing.Size(46, 21);
            this.listAgeDivisionsComboBox.TabIndex = 1;
            this.listAgeDivisionsComboBox.ValueMember = "Division";
            this.listAgeDivisionsComboBox.SelectedIndexChanged += new System.EventHandler(this.listAgeDivisionsComboBox_SelectedIndexChanged);
            // 
            // DivisionLabel
            // 
            this.DivisionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DivisionLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DivisionLabel.Location = new System.Drawing.Point(6, 3);
            this.DivisionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.DivisionLabel.Name = "DivisionLabel";
            this.DivisionLabel.Size = new System.Drawing.Size(70, 23);
            this.DivisionLabel.TabIndex = 0;
            this.DivisionLabel.Text = "Division";
            this.DivisionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Event1
            // 
            this.Event1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Event1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Event1.Location = new System.Drawing.Point(63, 29);
            this.Event1.Name = "Event1";
            this.Event1.Size = new System.Drawing.Size(65, 18);
            this.Event1.TabIndex = 0;
            this.Event1.Text = "Event1:";
            this.Event1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Nops1
            // 
            this.Nops1.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Nops1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Nops1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nops1.ForeColor = System.Drawing.Color.Navy;
            this.Nops1.Location = new System.Drawing.Point(178, 29);
            this.Nops1.Name = "Nops1";
            this.Nops1.Size = new System.Drawing.Size(65, 18);
            this.Nops1.TabIndex = 0;
            this.Nops1.Text = "00.0";
            this.Nops1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Score1
            // 
            this.Score1.AllowDrop = true;
            this.Score1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Score1.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            this.Score1.Location = new System.Drawing.Point(124, 28);
            this.Score1.Margin = new System.Windows.Forms.Padding(0);
            this.Score1.MaxLength = 6;
            this.Score1.Name = "Score1";
            this.Score1.ShortcutsEnabled = false;
            this.Score1.Size = new System.Drawing.Size(50, 21);
            this.Score1.TabIndex = 2;
            this.Score1.Text = "100.25";
            this.Score1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Score1.WordWrap = false;
            this.Score1.TextChanged += new System.EventHandler(this.Score1_TextChanged);
            this.Score1.Enter += new System.EventHandler(this.ScoreTextbox_Enter);
            this.Score1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Score_KeyUp);
            this.Score1.Leave += new System.EventHandler(this.Score1_Leave);
            this.Score1.Validating += new System.ComponentModel.CancelEventHandler(this.Score1_Validating);
            // 
            // Score2
            // 
            this.Score2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Score2.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            this.Score2.Location = new System.Drawing.Point(124, 51);
            this.Score2.Margin = new System.Windows.Forms.Padding(0);
            this.Score2.MaxLength = 6;
            this.Score2.Name = "Score2";
            this.Score2.ShortcutsEnabled = false;
            this.Score2.Size = new System.Drawing.Size(50, 21);
            this.Score2.TabIndex = 3;
            this.Score2.Text = "7525";
            this.Score2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Score2.WordWrap = false;
            this.Score2.TextChanged += new System.EventHandler(this.Score2_TextChanged);
            this.Score2.Enter += new System.EventHandler(this.ScoreTextbox_Enter);
            this.Score2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Score_KeyUp);
            this.Score2.Leave += new System.EventHandler(this.Score2_Leave);
            this.Score2.Validating += new System.ComponentModel.CancelEventHandler(this.Score2_Validating);
            // 
            // Nops2
            // 
            this.Nops2.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Nops2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Nops2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nops2.ForeColor = System.Drawing.Color.Navy;
            this.Nops2.Location = new System.Drawing.Point(178, 52);
            this.Nops2.Name = "Nops2";
            this.Nops2.Size = new System.Drawing.Size(65, 18);
            this.Nops2.TabIndex = 0;
            this.Nops2.Text = "00.0";
            this.Nops2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Event2
            // 
            this.Event2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Event2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Event2.Location = new System.Drawing.Point(63, 52);
            this.Event2.Name = "Event2";
            this.Event2.Size = new System.Drawing.Size(65, 18);
            this.Event2.TabIndex = 0;
            this.Event2.Text = "Event2:";
            this.Event2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Score3
            // 
            this.Score3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Score3.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            this.Score3.Location = new System.Drawing.Point(124, 74);
            this.Score3.Margin = new System.Windows.Forms.Padding(0);
            this.Score3.MaxLength = 7;
            this.Score3.Name = "Score3";
            this.Score3.Size = new System.Drawing.Size(50, 21);
            this.Score3.TabIndex = 4;
            this.Score3.Text = "125";
            this.Score3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Score3.WordWrap = false;
            this.Score3.TextChanged += new System.EventHandler(this.Score3_TextChanged);
            this.Score3.Enter += new System.EventHandler(this.ScoreTextbox_Enter);
            this.Score3.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Score_KeyUp);
            this.Score3.Leave += new System.EventHandler(this.Score3_Leave);
            this.Score3.Validating += new System.ComponentModel.CancelEventHandler(this.Score3_Validating);
            // 
            // Nops3
            // 
            this.Nops3.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Nops3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Nops3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nops3.ForeColor = System.Drawing.Color.Navy;
            this.Nops3.Location = new System.Drawing.Point(178, 75);
            this.Nops3.Name = "Nops3";
            this.Nops3.Size = new System.Drawing.Size(65, 18);
            this.Nops3.TabIndex = 0;
            this.Nops3.Text = "00.0";
            this.Nops3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Event3
            // 
            this.Event3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Event3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Event3.Location = new System.Drawing.Point(63, 75);
            this.Event3.Name = "Event3";
            this.Event3.Size = new System.Drawing.Size(65, 18);
            this.Event3.TabIndex = 0;
            this.Event3.Text = "Event3:";
            this.Event3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Nops4
            // 
            this.Nops4.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.Nops4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Nops4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Nops4.ForeColor = System.Drawing.Color.Navy;
            this.Nops4.Location = new System.Drawing.Point(178, 96);
            this.Nops4.Name = "Nops4";
            this.Nops4.Size = new System.Drawing.Size(65, 18);
            this.Nops4.TabIndex = 0;
            this.Nops4.Text = "00.0";
            this.Nops4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Event4
            // 
            this.Event4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Event4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.Event4.Location = new System.Drawing.Point(63, 96);
            this.Event4.Name = "Event4";
            this.Event4.Size = new System.Drawing.Size(65, 18);
            this.Event4.TabIndex = 0;
            this.Event4.Text = "Event4:";
            this.Event4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SlalomScoreDesc
            // 
            this.SlalomScoreDesc.AutoSize = true;
            this.SlalomScoreDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SlalomScoreDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlalomScoreDesc.Location = new System.Drawing.Point(249, 32);
            this.SlalomScoreDesc.MinimumSize = new System.Drawing.Size(100, 17);
            this.SlalomScoreDesc.Name = "SlalomScoreDesc";
            this.SlalomScoreDesc.Size = new System.Drawing.Size(100, 17);
            this.SlalomScoreDesc.TabIndex = 0;
            // 
            // printButton
            // 
            this.printButton.Location = new System.Drawing.Point(449, 3);
            this.printButton.Name = "printButton";
            this.printButton.Size = new System.Drawing.Size(75, 23);
            this.printButton.TabIndex = 11;
            this.printButton.Text = "Print";
            this.printButton.UseVisualStyleBackColor = true;
            this.printButton.Click += new System.EventHandler(this.printButton_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AgeGroup,
            this.Event,
            this.RatingRec,
            this.RatingMedian,
            this.Base,
            this.OverallBase,
            this.Adj,
            this.OverallExp,
            this.EventsReqd});
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataGridView.Location = new System.Drawing.Point(6, 127);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.Size = new System.Drawing.Size(510, 108);
            this.dataGridView.TabIndex = 12;
            // 
            // AgeGroup
            // 
            this.AgeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.AgeGroup.HeaderText = "Div";
            this.AgeGroup.Name = "AgeGroup";
            this.AgeGroup.ReadOnly = true;
            this.AgeGroup.Width = 30;
            // 
            // Event
            // 
            this.Event.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Event.DefaultCellStyle = dataGridViewCellStyle2;
            this.Event.HeaderText = "Event";
            this.Event.Name = "Event";
            this.Event.ReadOnly = true;
            this.Event.Width = 55;
            // 
            // RatingRec
            // 
            this.RatingRec.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.RatingRec.DefaultCellStyle = dataGridViewCellStyle3;
            this.RatingRec.HeaderText = "Record";
            this.RatingRec.Name = "RatingRec";
            this.RatingRec.ReadOnly = true;
            this.RatingRec.Width = 55;
            // 
            // RatingMedian
            // 
            this.RatingMedian.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.RatingMedian.DefaultCellStyle = dataGridViewCellStyle4;
            this.RatingMedian.HeaderText = "Median";
            this.RatingMedian.Name = "RatingMedian";
            this.RatingMedian.ReadOnly = true;
            this.RatingMedian.Width = 55;
            // 
            // Base
            // 
            this.Base.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Base.DefaultCellStyle = dataGridViewCellStyle5;
            this.Base.HeaderText = "Base";
            this.Base.Name = "Base";
            this.Base.ReadOnly = true;
            this.Base.Width = 50;
            // 
            // OverallBase
            // 
            this.OverallBase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.OverallBase.DefaultCellStyle = dataGridViewCellStyle6;
            this.OverallBase.HeaderText = "Overall";
            this.OverallBase.Name = "OverallBase";
            this.OverallBase.ReadOnly = true;
            this.OverallBase.Width = 60;
            // 
            // Adj
            // 
            this.Adj.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Adj.DefaultCellStyle = dataGridViewCellStyle7;
            this.Adj.HeaderText = "Adjust";
            this.Adj.Name = "Adj";
            this.Adj.ReadOnly = true;
            this.Adj.Width = 55;
            // 
            // OverallExp
            // 
            this.OverallExp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.OverallExp.DefaultCellStyle = dataGridViewCellStyle8;
            this.OverallExp.HeaderText = "Exp";
            this.OverallExp.Name = "OverallExp";
            this.OverallExp.ReadOnly = true;
            this.OverallExp.Width = 50;
            // 
            // EventsReqd
            // 
            this.EventsReqd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.EventsReqd.HeaderText = "Events";
            this.EventsReqd.Name = "EventsReqd";
            this.EventsReqd.ReadOnly = true;
            this.EventsReqd.Width = 50;
            // 
            // NopsCalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(529, 236);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.printButton);
            this.Controls.Add(this.SlalomScoreDesc);
            this.Controls.Add(this.Nops4);
            this.Controls.Add(this.Event4);
            this.Controls.Add(this.Score3);
            this.Controls.Add(this.Nops3);
            this.Controls.Add(this.Event3);
            this.Controls.Add(this.Score2);
            this.Controls.Add(this.Nops2);
            this.Controls.Add(this.Event2);
            this.Controls.Add(this.Score1);
            this.Controls.Add(this.Nops1);
            this.Controls.Add(this.Event1);
            this.Controls.Add(this.DivisionLabel);
            this.Controls.Add(this.listAgeDivisionsComboBox);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximumSize = new System.Drawing.Size(675, 325);
            this.Name = "NopsCalcForm";
            this.Text = "NOPS Calculator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NopsCalcForm_FormClosed);
            this.Load += new System.EventHandler(this.NopsCalcForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox listAgeDivisionsComboBox;
        private System.Windows.Forms.Label DivisionLabel;
        private System.Windows.Forms.Label Event1;
        private System.Windows.Forms.Label Nops1;
        private System.Windows.Forms.TextBox Score1;
        private System.Windows.Forms.TextBox Score2;
        private System.Windows.Forms.Label Nops2;
        private System.Windows.Forms.Label Event2;
        private System.Windows.Forms.TextBox Score3;
        private System.Windows.Forms.Label Nops3;
        private System.Windows.Forms.Label Event3;
        private System.Windows.Forms.Label Nops4;
        private System.Windows.Forms.Label Event4;
        private System.Windows.Forms.Label SlalomScoreDesc;
        private System.Windows.Forms.Button printButton;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn AgeGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.DataGridViewTextBoxColumn RatingRec;
        private System.Windows.Forms.DataGridViewTextBoxColumn RatingMedian;
        private System.Windows.Forms.DataGridViewTextBoxColumn Base;
        private System.Windows.Forms.DataGridViewTextBoxColumn OverallBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn Adj;
        private System.Windows.Forms.DataGridViewTextBoxColumn OverallExp;
        private System.Windows.Forms.DataGridViewTextBoxColumn EventsReqd;
    }
}