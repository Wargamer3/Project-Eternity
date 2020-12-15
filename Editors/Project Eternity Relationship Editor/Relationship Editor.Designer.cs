namespace ProjectEternity.Editors.RelationshipEditor
{
    partial class ProjectEternityRelationshipEditor
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
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.lstEffects = new System.Windows.Forms.ListBox();
            this.btnRemoveEffect = new System.Windows.Forms.Button();
            this.btnAddEffects = new System.Windows.Forms.Button();
            this.cboEffectType = new System.Windows.Forms.ComboBox();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.gbLevels = new System.Windows.Forms.GroupBox();
            this.lstLevels = new System.Windows.Forms.ListBox();
            this.btnRemoveLevel = new System.Windows.Forms.Button();
            this.btnAddLevel = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbEffects.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbLevels.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.lstEffects);
            this.gbEffects.Controls.Add(this.btnRemoveEffect);
            this.gbEffects.Controls.Add(this.btnAddEffects);
            this.gbEffects.Controls.Add(this.cboEffectType);
            this.gbEffects.Controls.Add(this.pgEffect);
            this.gbEffects.Enabled = false;
            this.gbEffects.Location = new System.Drawing.Point(199, 27);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(183, 340);
            this.gbEffects.TabIndex = 14;
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
            this.btnRemoveEffect.Click += new System.EventHandler(this.btnRemoveEffect_Click);
            // 
            // btnAddEffects
            // 
            this.btnAddEffects.Location = new System.Drawing.Point(6, 122);
            this.btnAddEffects.Name = "btnAddEffects";
            this.btnAddEffects.Size = new System.Drawing.Size(75, 23);
            this.btnAddEffects.TabIndex = 9;
            this.btnAddEffects.Text = "Add";
            this.btnAddEffects.UseVisualStyleBackColor = true;
            this.btnAddEffects.Click += new System.EventHandler(this.btnAddEffects_Click);
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
            this.pgEffect.Location = new System.Drawing.Point(6, 178);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(169, 156);
            this.pgEffect.TabIndex = 2;
            this.pgEffect.ToolbarVisible = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDescription);
            this.groupBox3.Location = new System.Drawing.Point(12, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 157);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(169, 132);
            this.txtDescription.TabIndex = 0;
            // 
            // gbLevels
            // 
            this.gbLevels.Controls.Add(this.lstLevels);
            this.gbLevels.Controls.Add(this.btnRemoveLevel);
            this.gbLevels.Controls.Add(this.btnAddLevel);
            this.gbLevels.Location = new System.Drawing.Point(12, 27);
            this.gbLevels.Name = "gbLevels";
            this.gbLevels.Size = new System.Drawing.Size(181, 177);
            this.gbLevels.TabIndex = 12;
            this.gbLevels.TabStop = false;
            this.gbLevels.Text = "Levels";
            // 
            // lstLevels
            // 
            this.lstLevels.FormattingEnabled = true;
            this.lstLevels.Location = new System.Drawing.Point(6, 19);
            this.lstLevels.Name = "lstLevels";
            this.lstLevels.Size = new System.Drawing.Size(169, 121);
            this.lstLevels.TabIndex = 9;
            this.lstLevels.SelectedIndexChanged += new System.EventHandler(this.lstLevels_SelectedIndexChanged);
            // 
            // btnRemoveLevel
            // 
            this.btnRemoveLevel.Location = new System.Drawing.Point(100, 146);
            this.btnRemoveLevel.Name = "btnRemoveLevel";
            this.btnRemoveLevel.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveLevel.TabIndex = 9;
            this.btnRemoveLevel.Text = "Remove";
            this.btnRemoveLevel.UseVisualStyleBackColor = true;
            this.btnRemoveLevel.Click += new System.EventHandler(this.btnRemoveLevel_Click);
            // 
            // btnAddLevel
            // 
            this.btnAddLevel.Location = new System.Drawing.Point(6, 146);
            this.btnAddLevel.Name = "btnAddLevel";
            this.btnAddLevel.Size = new System.Drawing.Size(75, 23);
            this.btnAddLevel.TabIndex = 8;
            this.btnAddLevel.Text = "Add";
            this.btnAddLevel.UseVisualStyleBackColor = true;
            this.btnAddLevel.Click += new System.EventHandler(this.btnAddLevel_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(389, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // ProjectEternityRelationshipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 374);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbEffects);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbLevels);
            this.Name = "ProjectEternityRelationshipEditor";
            this.Text = "Relationship Editor";
            this.gbEffects.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbLevels.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbEffects;
        private System.Windows.Forms.ListBox lstEffects;
        private System.Windows.Forms.Button btnRemoveEffect;
        private System.Windows.Forms.Button btnAddEffects;
        private System.Windows.Forms.ComboBox cboEffectType;
        private System.Windows.Forms.PropertyGrid pgEffect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.GroupBox gbLevels;
        private System.Windows.Forms.ListBox lstLevels;
        private System.Windows.Forms.Button btnRemoveLevel;
        private System.Windows.Forms.Button btnAddLevel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
    }
}

