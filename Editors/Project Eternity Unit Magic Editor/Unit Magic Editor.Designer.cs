namespace ProjectEternity.Editors.UnitHubEditor
{
    partial class UnitMagicEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbOriginalUnit = new System.Windows.Forms.GroupBox();
            this.btnSelectOrginalUnit = new System.Windows.Forms.Button();
            this.txtOriginalUnit = new System.Windows.Forms.TextBox();
            this.gbSpells = new System.Windows.Forms.GroupBox();
            this.btnRemoveSpell = new System.Windows.Forms.Button();
            this.btnAddSpell = new System.Windows.Forms.Button();
            this.lstSpells = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.gbOriginalUnit.SuspendLayout();
            this.gbSpells.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(255, 24);
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
            // gbOriginalUnit
            // 
            this.gbOriginalUnit.Controls.Add(this.btnSelectOrginalUnit);
            this.gbOriginalUnit.Controls.Add(this.txtOriginalUnit);
            this.gbOriginalUnit.Location = new System.Drawing.Point(12, 27);
            this.gbOriginalUnit.Name = "gbOriginalUnit";
            this.gbOriginalUnit.Size = new System.Drawing.Size(230, 72);
            this.gbOriginalUnit.TabIndex = 4;
            this.gbOriginalUnit.TabStop = false;
            this.gbOriginalUnit.Text = "Original Unit";
            // 
            // btnSelectOrginalUnit
            // 
            this.btnSelectOrginalUnit.Location = new System.Drawing.Point(6, 43);
            this.btnSelectOrginalUnit.Name = "btnSelectOrginalUnit";
            this.btnSelectOrginalUnit.Size = new System.Drawing.Size(120, 23);
            this.btnSelectOrginalUnit.TabIndex = 2;
            this.btnSelectOrginalUnit.Text = "Select Unit";
            this.btnSelectOrginalUnit.UseVisualStyleBackColor = true;
            this.btnSelectOrginalUnit.Click += new System.EventHandler(this.btnSelectOrginalUnit_Click);
            // 
            // txtOriginalUnit
            // 
            this.txtOriginalUnit.Location = new System.Drawing.Point(6, 17);
            this.txtOriginalUnit.Name = "txtOriginalUnit";
            this.txtOriginalUnit.ReadOnly = true;
            this.txtOriginalUnit.Size = new System.Drawing.Size(218, 20);
            this.txtOriginalUnit.TabIndex = 0;
            // 
            // gbSpells
            // 
            this.gbSpells.Controls.Add(this.btnRemoveSpell);
            this.gbSpells.Controls.Add(this.btnAddSpell);
            this.gbSpells.Controls.Add(this.lstSpells);
            this.gbSpells.Location = new System.Drawing.Point(12, 105);
            this.gbSpells.Name = "gbSpells";
            this.gbSpells.Size = new System.Drawing.Size(231, 150);
            this.gbSpells.TabIndex = 5;
            this.gbSpells.TabStop = false;
            this.gbSpells.Text = "Spells";
            // 
            // btnRemoveSpell
            // 
            this.btnRemoveSpell.Location = new System.Drawing.Point(118, 120);
            this.btnRemoveSpell.Name = "btnRemoveSpell";
            this.btnRemoveSpell.Size = new System.Drawing.Size(107, 23);
            this.btnRemoveSpell.TabIndex = 4;
            this.btnRemoveSpell.Text = "Remove Spell";
            this.btnRemoveSpell.UseVisualStyleBackColor = true;
            this.btnRemoveSpell.Click += new System.EventHandler(this.btnRemoveSpell_Click);
            // 
            // btnAddSpell
            // 
            this.btnAddSpell.Location = new System.Drawing.Point(6, 120);
            this.btnAddSpell.Name = "btnAddSpell";
            this.btnAddSpell.Size = new System.Drawing.Size(107, 23);
            this.btnAddSpell.TabIndex = 3;
            this.btnAddSpell.Text = "Add Spell";
            this.btnAddSpell.UseVisualStyleBackColor = true;
            this.btnAddSpell.Click += new System.EventHandler(this.btnAddSpell_Click);
            // 
            // lstSpells
            // 
            this.lstSpells.FormattingEnabled = true;
            this.lstSpells.Location = new System.Drawing.Point(6, 19);
            this.lstSpells.Name = "lstSpells";
            this.lstSpells.Size = new System.Drawing.Size(218, 95);
            this.lstSpells.TabIndex = 0;
            // 
            // UnitMagicEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(255, 257);
            this.Controls.Add(this.gbSpells);
            this.Controls.Add(this.gbOriginalUnit);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitMagicEditor";
            this.Text = "Unit Magic Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbOriginalUnit.ResumeLayout(false);
            this.gbOriginalUnit.PerformLayout();
            this.gbSpells.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbOriginalUnit;
        private System.Windows.Forms.Button btnSelectOrginalUnit;
        private System.Windows.Forms.TextBox txtOriginalUnit;
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.Button btnAddSpell;
        private System.Windows.Forms.ListBox lstSpells;
        private System.Windows.Forms.Button btnRemoveSpell;
    }
}