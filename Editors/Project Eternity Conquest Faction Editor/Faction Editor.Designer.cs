namespace ProjectEternity.Editors.RosterEditor
{
    partial class ProjectEternityFactionEditor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lsFactions = new System.Windows.Forms.ListBox();
            this.gbFactions = new System.Windows.Forms.GroupBox();
            this.btnRemoveCharacter = new System.Windows.Forms.Button();
            this.btnAddCharacter = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.txtFactionName = new System.Windows.Forms.TextBox();
            this.lblFactionName = new System.Windows.Forms.Label();
            this.gbFactions.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsFactions
            // 
            this.lsFactions.FormattingEnabled = true;
            this.lsFactions.Location = new System.Drawing.Point(6, 19);
            this.lsFactions.Name = "lsFactions";
            this.lsFactions.Size = new System.Drawing.Size(256, 199);
            this.lsFactions.TabIndex = 1;
            // 
            // gbFactions
            // 
            this.gbFactions.Controls.Add(this.lblFactionName);
            this.gbFactions.Controls.Add(this.txtFactionName);
            this.gbFactions.Controls.Add(this.btnRemoveCharacter);
            this.gbFactions.Controls.Add(this.btnAddCharacter);
            this.gbFactions.Controls.Add(this.lsFactions);
            this.gbFactions.Location = new System.Drawing.Point(12, 27);
            this.gbFactions.Name = "gbFactions";
            this.gbFactions.Size = new System.Drawing.Size(270, 299);
            this.gbFactions.TabIndex = 3;
            this.gbFactions.TabStop = false;
            this.gbFactions.Text = "Factions";
            // 
            // btnRemoveCharacter
            // 
            this.btnRemoveCharacter.Location = new System.Drawing.Point(137, 263);
            this.btnRemoveCharacter.Name = "btnRemoveCharacter";
            this.btnRemoveCharacter.Size = new System.Drawing.Size(125, 23);
            this.btnRemoveCharacter.TabIndex = 4;
            this.btnRemoveCharacter.Text = "Remove Faction";
            this.btnRemoveCharacter.UseVisualStyleBackColor = true;
            // 
            // btnAddCharacter
            // 
            this.btnAddCharacter.Location = new System.Drawing.Point(6, 263);
            this.btnAddCharacter.Name = "btnAddCharacter";
            this.btnAddCharacter.Size = new System.Drawing.Size(125, 23);
            this.btnAddCharacter.TabIndex = 3;
            this.btnAddCharacter.Text = "Add Faction";
            this.btnAddCharacter.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(293, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // txtFactionName
            // 
            this.txtFactionName.Location = new System.Drawing.Point(6, 237);
            this.txtFactionName.Name = "txtFactionName";
            this.txtFactionName.Size = new System.Drawing.Size(256, 20);
            this.txtFactionName.TabIndex = 7;
            // 
            // lblFactionName
            // 
            this.lblFactionName.AutoSize = true;
            this.lblFactionName.Location = new System.Drawing.Point(6, 221);
            this.lblFactionName.Name = "lblFactionName";
            this.lblFactionName.Size = new System.Drawing.Size(35, 13);
            this.lblFactionName.TabIndex = 7;
            this.lblFactionName.Text = "Name";
            // 
            // ProjectEternityFactionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 336);
            this.Controls.Add(this.gbFactions);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ProjectEternityFactionEditor";
            this.Text = "Faction Editor";
            this.gbFactions.ResumeLayout(false);
            this.gbFactions.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox lsFactions;
        private System.Windows.Forms.GroupBox gbFactions;
        private System.Windows.Forms.Button btnRemoveCharacter;
        private System.Windows.Forms.Button btnAddCharacter;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.TextBox txtFactionName;
        private System.Windows.Forms.Label lblFactionName;
    }
}

