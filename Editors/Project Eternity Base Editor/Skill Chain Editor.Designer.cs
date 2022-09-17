namespace ProjectEternity.Editors.SkillChainEditor
{
    partial class SkillChainEditor
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
            this.components = new System.ComponentModel.Container();
            this.gbSkillChain = new System.Windows.Forms.GroupBox();
            this.tvSkills = new System.Windows.Forms.TreeView();
            this.cmsSkillChain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmNewEffect = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmNewRequirement = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.lstEffects = new System.Windows.Forms.ListBox();
            this.btnRemoveEffect = new System.Windows.Forms.Button();
            this.btnAddEffects = new System.Windows.Forms.Button();
            this.cboEffectType = new System.Windows.Forms.ComboBox();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.gbRequirements = new System.Windows.Forms.GroupBox();
            this.cboRequirementType = new System.Windows.Forms.ComboBox();
            this.pgRequirement = new System.Windows.Forms.PropertyGrid();
            this.tsmExpendAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.gbSkillChain.SuspendLayout();
            this.cmsSkillChain.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbEffects.SuspendLayout();
            this.gbRequirements.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSkillChain
            // 
            this.gbSkillChain.Controls.Add(this.tvSkills);
            this.gbSkillChain.Location = new System.Drawing.Point(12, 27);
            this.gbSkillChain.Name = "gbSkillChain";
            this.gbSkillChain.Size = new System.Drawing.Size(218, 340);
            this.gbSkillChain.TabIndex = 2;
            this.gbSkillChain.TabStop = false;
            this.gbSkillChain.Text = "Skill Chain";
            // 
            // tvSkills
            // 
            this.tvSkills.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvSkills.ContextMenuStrip = this.cmsSkillChain;
            this.tvSkills.Location = new System.Drawing.Point(6, 19);
            this.tvSkills.Name = "tvSkills";
            this.tvSkills.Size = new System.Drawing.Size(206, 315);
            this.tvSkills.TabIndex = 4;
            this.tvSkills.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSkills_AfterSelect);
            // 
            // cmsSkillChain
            // 
            this.cmsSkillChain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNewEffect,
            this.tsmNewRequirement});
            this.cmsSkillChain.Name = "cmsSkillChain";
            this.cmsSkillChain.Size = new System.Drawing.Size(170, 48);
            // 
            // tsmNewEffect
            // 
            this.tsmNewEffect.Name = "tsmNewEffect";
            this.tsmNewEffect.Size = new System.Drawing.Size(169, 22);
            this.tsmNewEffect.Text = "New Effect";
            this.tsmNewEffect.Click += new System.EventHandler(this.tsmNewEffect_Click);
            // 
            // tsmNewRequirement
            // 
            this.tsmNewRequirement.Name = "tsmNewRequirement";
            this.tsmNewRequirement.Size = new System.Drawing.Size(169, 22);
            this.tsmNewRequirement.Text = "New Requirement";
            this.tsmNewRequirement.Click += new System.EventHandler(this.tsmNewRequirement_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmExpendAll,
            this.tsmCollapseAll});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(617, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.lstEffects);
            this.gbEffects.Controls.Add(this.btnRemoveEffect);
            this.gbEffects.Controls.Add(this.btnAddEffects);
            this.gbEffects.Controls.Add(this.cboEffectType);
            this.gbEffects.Controls.Add(this.pgEffect);
            this.gbEffects.Enabled = false;
            this.gbEffects.Location = new System.Drawing.Point(425, 27);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(183, 340);
            this.gbEffects.TabIndex = 15;
            this.gbEffects.TabStop = false;
            this.gbEffects.Text = "Effects";
            // 
            // lstEffects
            // 
            this.lstEffects.FormattingEnabled = true;
            this.lstEffects.Location = new System.Drawing.Point(6, 19);
            this.lstEffects.Name = "lstEffects";
            this.lstEffects.Size = new System.Drawing.Size(169, 95);
            this.lstEffects.TabIndex = 11;
            this.lstEffects.SelectedIndexChanged += new System.EventHandler(this.lstEffects_SelectedIndexChanged);
            // 
            // btnRemoveEffect
            // 
            this.btnRemoveEffect.Location = new System.Drawing.Point(100, 122);
            this.btnRemoveEffect.Name = "btnRemoveEffect";
            this.btnRemoveEffect.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveEffect.TabIndex = 10;
            this.btnRemoveEffect.Text = "Remove";
            this.btnRemoveEffect.UseVisualStyleBackColor = true;
            // 
            // btnAddEffects
            // 
            this.btnAddEffects.Location = new System.Drawing.Point(6, 122);
            this.btnAddEffects.Name = "btnAddEffects";
            this.btnAddEffects.Size = new System.Drawing.Size(75, 23);
            this.btnAddEffects.TabIndex = 9;
            this.btnAddEffects.Text = "Add";
            this.btnAddEffects.UseVisualStyleBackColor = true;
            // 
            // cboEffectType
            // 
            this.cboEffectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEffectType.FormattingEnabled = true;
            this.cboEffectType.Location = new System.Drawing.Point(6, 151);
            this.cboEffectType.Name = "cboEffectType";
            this.cboEffectType.Size = new System.Drawing.Size(169, 21);
            this.cboEffectType.TabIndex = 3;
            this.cboEffectType.SelectedIndexChanged += new System.EventHandler(this.cboEffectType_SelectedIndexChanged);
            // 
            // pgEffect
            // 
            this.pgEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgEffect.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgEffect.Location = new System.Drawing.Point(6, 178);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(169, 156);
            this.pgEffect.TabIndex = 2;
            this.pgEffect.ToolbarVisible = false;
            // 
            // gbRequirements
            // 
            this.gbRequirements.Controls.Add(this.cboRequirementType);
            this.gbRequirements.Controls.Add(this.pgRequirement);
            this.gbRequirements.Enabled = false;
            this.gbRequirements.Location = new System.Drawing.Point(236, 27);
            this.gbRequirements.Name = "gbRequirements";
            this.gbRequirements.Size = new System.Drawing.Size(183, 340);
            this.gbRequirements.TabIndex = 13;
            this.gbRequirements.TabStop = false;
            this.gbRequirements.Text = "Skill Requirement";
            // 
            // cboRequirementType
            // 
            this.cboRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRequirementType.FormattingEnabled = true;
            this.cboRequirementType.Location = new System.Drawing.Point(6, 19);
            this.cboRequirementType.Name = "cboRequirementType";
            this.cboRequirementType.Size = new System.Drawing.Size(169, 21);
            this.cboRequirementType.TabIndex = 3;
            this.cboRequirementType.SelectedIndexChanged += new System.EventHandler(this.cboRequirementType_SelectedIndexChanged);
            // 
            // pgRequirement
            // 
            this.pgRequirement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgRequirement.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgRequirement.Location = new System.Drawing.Point(6, 46);
            this.pgRequirement.Name = "pgRequirement";
            this.pgRequirement.Size = new System.Drawing.Size(169, 288);
            this.pgRequirement.TabIndex = 2;
            this.pgRequirement.ToolbarVisible = false;
            // 
            // tsmExpendAll
            // 
            this.tsmExpendAll.Name = "tsmExpendAll";
            this.tsmExpendAll.Size = new System.Drawing.Size(74, 20);
            this.tsmExpendAll.Text = "Expend All";
            this.tsmExpendAll.Click += new System.EventHandler(this.tsmExpendAll_Click);
            // 
            // tsmCollapseAll
            // 
            this.tsmCollapseAll.Name = "tsmCollapseAll";
            this.tsmCollapseAll.Size = new System.Drawing.Size(81, 20);
            this.tsmCollapseAll.Text = "Collapse All";
            this.tsmCollapseAll.Click += new System.EventHandler(this.tsmCollapseAll_Click);
            // 
            // SkillChainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 374);
            this.Controls.Add(this.gbEffects);
            this.Controls.Add(this.gbRequirements);
            this.Controls.Add(this.gbSkillChain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SkillChainEditor";
            this.Text = "Skill Chain Editor";
            this.Shown += new System.EventHandler(this.ComboEditor_Shown);
            this.gbSkillChain.ResumeLayout(false);
            this.cmsSkillChain.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbEffects.ResumeLayout(false);
            this.gbRequirements.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbSkillChain;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        protected System.Windows.Forms.TreeView tvSkills;
        private System.Windows.Forms.GroupBox gbEffects;
        private System.Windows.Forms.ListBox lstEffects;
        private System.Windows.Forms.Button btnRemoveEffect;
        private System.Windows.Forms.Button btnAddEffects;
        protected System.Windows.Forms.ComboBox cboEffectType;
        private System.Windows.Forms.PropertyGrid pgEffect;
        private System.Windows.Forms.GroupBox gbRequirements;
        protected System.Windows.Forms.ComboBox cboRequirementType;
        private System.Windows.Forms.PropertyGrid pgRequirement;
        private System.Windows.Forms.ContextMenuStrip cmsSkillChain;
        private System.Windows.Forms.ToolStripMenuItem tsmNewEffect;
        private System.Windows.Forms.ToolStripMenuItem tsmNewRequirement;
        private System.Windows.Forms.ToolStripMenuItem tsmExpendAll;
        private System.Windows.Forms.ToolStripMenuItem tsmCollapseAll;
    }
}