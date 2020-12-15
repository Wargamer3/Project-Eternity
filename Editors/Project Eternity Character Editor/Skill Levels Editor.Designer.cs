namespace ProjectEternity.Editors.CharacterEditor
{
    partial class SkillLevelsEditor
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
            this.gbLevels = new System.Windows.Forms.GroupBox();
            this.dgvLevels = new System.Windows.Forms.DataGridView();
            this.gbSkillInfo = new System.Windows.Forms.GroupBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblSkillName = new System.Windows.Forms.Label();
            this.clSkillLevels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCharacterLevels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbLevels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLevels)).BeginInit();
            this.gbSkillInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbLevels
            // 
            this.gbLevels.Controls.Add(this.dgvLevels);
            this.gbLevels.Location = new System.Drawing.Point(12, 64);
            this.gbLevels.Name = "gbLevels";
            this.gbLevels.Size = new System.Drawing.Size(599, 256);
            this.gbLevels.TabIndex = 0;
            this.gbLevels.TabStop = false;
            this.gbLevels.Text = "Levels";
            // 
            // dgvLevels
            // 
            this.dgvLevels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLevels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLevels.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clSkillLevels,
            this.clCharacterLevels});
            this.dgvLevels.Location = new System.Drawing.Point(6, 19);
            this.dgvLevels.MultiSelect = false;
            this.dgvLevels.Name = "dgvLevels";
            this.dgvLevels.RowHeadersVisible = false;
            this.dgvLevels.Size = new System.Drawing.Size(587, 231);
            this.dgvLevels.TabIndex = 1;
            // 
            // gbSkillInfo
            // 
            this.gbSkillInfo.Controls.Add(this.lblName);
            this.gbSkillInfo.Controls.Add(this.lblSkillName);
            this.gbSkillInfo.Location = new System.Drawing.Point(18, 6);
            this.gbSkillInfo.Name = "gbSkillInfo";
            this.gbSkillInfo.Size = new System.Drawing.Size(593, 52);
            this.gbSkillInfo.TabIndex = 4;
            this.gbSkillInfo.TabStop = false;
            this.gbSkillInfo.Text = "Skill Info";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(72, 21);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(0, 13);
            this.lblName.TabIndex = 1;
            // 
            // lblSkillName
            // 
            this.lblSkillName.AutoSize = true;
            this.lblSkillName.Location = new System.Drawing.Point(6, 21);
            this.lblSkillName.Name = "lblSkillName";
            this.lblSkillName.Size = new System.Drawing.Size(60, 13);
            this.lblSkillName.TabIndex = 0;
            this.lblSkillName.Text = "Skill Name:";
            // 
            // clSkillLevels
            // 
            this.clSkillLevels.HeaderText = "Character Levels";
            this.clSkillLevels.Name = "clSkillLevels";
            // 
            // clCharacterLevels
            // 
            this.clCharacterLevels.HeaderText = "Skill Levels";
            this.clCharacterLevels.Name = "clCharacterLevels";
            // 
            // SkillLevelsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 327);
            this.Controls.Add(this.gbSkillInfo);
            this.Controls.Add(this.gbLevels);
            this.Name = "SkillLevelsEditor";
            this.Text = "Character Stats Editor";
            this.gbLevels.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLevels)).EndInit();
            this.gbSkillInfo.ResumeLayout(false);
            this.gbSkillInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbLevels;
        public System.Windows.Forms.DataGridView dgvLevels;
        private System.Windows.Forms.GroupBox gbSkillInfo;
        private System.Windows.Forms.Label lblSkillName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clSkillLevels;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCharacterLevels;
    }
}