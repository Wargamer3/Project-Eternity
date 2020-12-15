namespace ProjectEternity.Editors.CharacterEditor
{
    partial class CharacterStatsEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbStats = new System.Windows.Forms.GroupBox();
            this.dgvStats = new System.Windows.Forms.DataGridView();
            this.clLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clMEL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clRNG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clDEF = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clSKL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clEVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clHIT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clSP = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbTotalStatsIncrease = new System.Windows.Forms.GroupBox();
            this.dgvTotalStatsIncrease = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbLevels = new System.Windows.Forms.GroupBox();
            this.btnFillTheBlanks = new System.Windows.Forms.Button();
            this.txtMaxLevel = new System.Windows.Forms.NumericUpDown();
            this.lblMaxLevel = new System.Windows.Forms.Label();
            this.btnResetStats = new System.Windows.Forms.Button();
            this.gbStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).BeginInit();
            this.gbTotalStatsIncrease.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotalStatsIncrease)).BeginInit();
            this.gbLevels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // gbStats
            // 
            this.gbStats.Controls.Add(this.dgvStats);
            this.gbStats.Location = new System.Drawing.Point(12, 64);
            this.gbStats.Name = "gbStats";
            this.gbStats.Size = new System.Drawing.Size(599, 256);
            this.gbStats.TabIndex = 0;
            this.gbStats.TabStop = false;
            this.gbStats.Text = "Stats";
            // 
            // dgvStats
            // 
            this.dgvStats.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStats.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStats.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clLevel,
            this.clMEL,
            this.clRNG,
            this.clDEF,
            this.clSKL,
            this.clEVA,
            this.clHIT,
            this.clSP});
            this.dgvStats.Location = new System.Drawing.Point(6, 19);
            this.dgvStats.MultiSelect = false;
            this.dgvStats.Name = "dgvStats";
            this.dgvStats.RowHeadersVisible = false;
            this.dgvStats.Size = new System.Drawing.Size(587, 231);
            this.dgvStats.TabIndex = 1;
            this.dgvStats.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStats_CellValueChanged);
            // 
            // clLevel
            // 
            this.clLevel.HeaderText = "Level";
            this.clLevel.Name = "clLevel";
            this.clLevel.ReadOnly = true;
            // 
            // clMEL
            // 
            this.clMEL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clMEL.HeaderText = "MEL";
            this.clMEL.Name = "clMEL";
            // 
            // clRNG
            // 
            this.clRNG.HeaderText = "RNG";
            this.clRNG.Name = "clRNG";
            // 
            // clDEF
            // 
            this.clDEF.HeaderText = "DEF";
            this.clDEF.Name = "clDEF";
            // 
            // clSKL
            // 
            this.clSKL.HeaderText = "SKL";
            this.clSKL.Name = "clSKL";
            // 
            // clEVA
            // 
            this.clEVA.HeaderText = "EVA";
            this.clEVA.Name = "clEVA";
            // 
            // clHIT
            // 
            this.clHIT.HeaderText = "HIT";
            this.clHIT.Name = "clHIT";
            // 
            // clSP
            // 
            this.clSP.HeaderText = "SP";
            this.clSP.Name = "clSP";
            // 
            // gbTotalStatsIncrease
            // 
            this.gbTotalStatsIncrease.Controls.Add(this.dgvTotalStatsIncrease);
            this.gbTotalStatsIncrease.Location = new System.Drawing.Point(12, 320);
            this.gbTotalStatsIncrease.Name = "gbTotalStatsIncrease";
            this.gbTotalStatsIncrease.Size = new System.Drawing.Size(599, 51);
            this.gbTotalStatsIncrease.TabIndex = 3;
            this.gbTotalStatsIncrease.TabStop = false;
            this.gbTotalStatsIncrease.Text = "Total Stats Increase";
            // 
            // dgvTotalStatsIncrease
            // 
            this.dgvTotalStatsIncrease.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTotalStatsIncrease.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTotalStatsIncrease.ColumnHeadersVisible = false;
            this.dgvTotalStatsIncrease.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn8});
            this.dgvTotalStatsIncrease.Location = new System.Drawing.Point(6, 19);
            this.dgvTotalStatsIncrease.MultiSelect = false;
            this.dgvTotalStatsIncrease.Name = "dgvTotalStatsIncrease";
            this.dgvTotalStatsIncrease.ReadOnly = true;
            this.dgvTotalStatsIncrease.RowHeadersVisible = false;
            this.dgvTotalStatsIncrease.Size = new System.Drawing.Size(587, 25);
            this.dgvTotalStatsIncrease.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Level";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.HeaderText = "MEL";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "RNG";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "DEF";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "SKL";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "EVA";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "HIT";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "SP";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.ReadOnly = true;
            // 
            // gbLevels
            // 
            this.gbLevels.Controls.Add(this.btnResetStats);
            this.gbLevels.Controls.Add(this.btnFillTheBlanks);
            this.gbLevels.Controls.Add(this.txtMaxLevel);
            this.gbLevels.Controls.Add(this.lblMaxLevel);
            this.gbLevels.Location = new System.Drawing.Point(18, 6);
            this.gbLevels.Name = "gbLevels";
            this.gbLevels.Size = new System.Drawing.Size(593, 52);
            this.gbLevels.TabIndex = 4;
            this.gbLevels.TabStop = false;
            this.gbLevels.Text = "Levels";
            // 
            // btnFillTheBlanks
            // 
            this.btnFillTheBlanks.Location = new System.Drawing.Point(194, 19);
            this.btnFillTheBlanks.Name = "btnFillTheBlanks";
            this.btnFillTheBlanks.Size = new System.Drawing.Size(94, 23);
            this.btnFillTheBlanks.TabIndex = 6;
            this.btnFillTheBlanks.Text = "Fill the blanks";
            this.btnFillTheBlanks.UseVisualStyleBackColor = true;
            this.btnFillTheBlanks.Click += new System.EventHandler(this.btnFillTheBlanks_Click);
            // 
            // txtMaxLevel
            // 
            this.txtMaxLevel.Location = new System.Drawing.Point(68, 19);
            this.txtMaxLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMaxLevel.Name = "txtMaxLevel";
            this.txtMaxLevel.Size = new System.Drawing.Size(120, 20);
            this.txtMaxLevel.TabIndex = 5;
            this.txtMaxLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMaxLevel.ValueChanged += new System.EventHandler(this.txtMaxLevel_ValueChanged);
            // 
            // lblMaxLevel
            // 
            this.lblMaxLevel.AutoSize = true;
            this.lblMaxLevel.Location = new System.Drawing.Point(6, 21);
            this.lblMaxLevel.Name = "lblMaxLevel";
            this.lblMaxLevel.Size = new System.Drawing.Size(56, 13);
            this.lblMaxLevel.TabIndex = 0;
            this.lblMaxLevel.Text = "Max Level";
            // 
            // btnResetStats
            // 
            this.btnResetStats.Location = new System.Drawing.Point(294, 19);
            this.btnResetStats.Name = "btnResetStats";
            this.btnResetStats.Size = new System.Drawing.Size(94, 23);
            this.btnResetStats.TabIndex = 7;
            this.btnResetStats.Text = "Reset Stats";
            this.btnResetStats.UseVisualStyleBackColor = true;
            this.btnResetStats.Click += new System.EventHandler(this.btnResetStats_Click);
            // 
            // CharacterStatsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 380);
            this.Controls.Add(this.gbLevels);
            this.Controls.Add(this.gbTotalStatsIncrease);
            this.Controls.Add(this.gbStats);
            this.Name = "CharacterStatsEditor";
            this.Text = "Character Stats Editor";
            this.gbStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStats)).EndInit();
            this.gbTotalStatsIncrease.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotalStatsIncrease)).EndInit();
            this.gbLevels.ResumeLayout(false);
            this.gbLevels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbStats;
        public System.Windows.Forms.DataGridView dgvStats;
        private System.Windows.Forms.GroupBox gbTotalStatsIncrease;
        private System.Windows.Forms.GroupBox gbLevels;
        private System.Windows.Forms.Button btnFillTheBlanks;
        private System.Windows.Forms.Label lblMaxLevel;
        public System.Windows.Forms.DataGridView dgvTotalStatsIncrease;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn clLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn clMEL;
        private System.Windows.Forms.DataGridViewTextBoxColumn clRNG;
        private System.Windows.Forms.DataGridViewTextBoxColumn clDEF;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSKL;
        private System.Windows.Forms.DataGridViewTextBoxColumn clEVA;
        private System.Windows.Forms.DataGridViewTextBoxColumn clHIT;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSP;
        public System.Windows.Forms.NumericUpDown txtMaxLevel;
        private System.Windows.Forms.Button btnResetStats;
    }
}