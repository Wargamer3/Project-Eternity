namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class CharacterSkillEditor
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
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSkills = new System.Windows.Forms.TabPage();
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.btnDeleteSpell = new System.Windows.Forms.Button();
            this.btnSetSpell = new System.Windows.Forms.Button();
            this.lsActions = new System.Windows.Forms.ListBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.txtSpell = new System.Windows.Forms.TextBox();
            this.btnAddSpell = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSkills.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(693, 24);
            this.menuStrip1.TabIndex = 66;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbItemInformation
            // 
            this.gbItemInformation.Controls.Add(this.txtDescription);
            this.gbItemInformation.Controls.Add(this.lblDescription);
            this.gbItemInformation.Controls.Add(this.lblName);
            this.gbItemInformation.Controls.Add(this.txtName);
            this.gbItemInformation.Location = new System.Drawing.Point(12, 27);
            this.gbItemInformation.Name = "gbItemInformation";
            this.gbItemInformation.Size = new System.Drawing.Size(395, 145);
            this.gbItemInformation.TabIndex = 67;
            this.gbItemInformation.TabStop = false;
            this.gbItemInformation.Text = "Basic Information";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 58);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(383, 82);
            this.txtDescription.TabIndex = 73;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 42);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 72;
            this.lblDescription.Text = "Description:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 23;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(339, 20);
            this.txtName.TabIndex = 22;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSkills);
            this.tabControl1.Location = new System.Drawing.Point(413, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(275, 383);
            this.tabControl1.TabIndex = 72;
            // 
            // tabSkills
            // 
            this.tabSkills.Controls.Add(this.gbActions);
            this.tabSkills.Location = new System.Drawing.Point(4, 22);
            this.tabSkills.Name = "tabSkills";
            this.tabSkills.Padding = new System.Windows.Forms.Padding(3);
            this.tabSkills.Size = new System.Drawing.Size(267, 357);
            this.tabSkills.TabIndex = 0;
            this.tabSkills.Text = "Skills";
            this.tabSkills.UseVisualStyleBackColor = true;
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnDeleteSpell);
            this.gbActions.Controls.Add(this.btnSetSpell);
            this.gbActions.Controls.Add(this.lsActions);
            this.gbActions.Controls.Add(this.lblSpell);
            this.gbActions.Controls.Add(this.txtSpell);
            this.gbActions.Controls.Add(this.btnAddSpell);
            this.gbActions.Location = new System.Drawing.Point(6, 6);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(256, 175);
            this.gbActions.TabIndex = 68;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // btnDeleteSpell
            // 
            this.btnDeleteSpell.Location = new System.Drawing.Point(163, 94);
            this.btnDeleteSpell.Name = "btnDeleteSpell";
            this.btnDeleteSpell.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteSpell.TabIndex = 68;
            this.btnDeleteSpell.Text = "Delete Spell";
            this.btnDeleteSpell.UseVisualStyleBackColor = true;
            // 
            // btnSetSpell
            // 
            this.btnSetSpell.Location = new System.Drawing.Point(86, 94);
            this.btnSetSpell.Name = "btnSetSpell";
            this.btnSetSpell.Size = new System.Drawing.Size(71, 23);
            this.btnSetSpell.TabIndex = 66;
            this.btnSetSpell.Text = "Set Spell";
            this.btnSetSpell.UseVisualStyleBackColor = true;
            // 
            // lsActions
            // 
            this.lsActions.FormattingEnabled = true;
            this.lsActions.Location = new System.Drawing.Point(9, 19);
            this.lsActions.Name = "lsActions";
            this.lsActions.Size = new System.Drawing.Size(239, 69);
            this.lsActions.TabIndex = 65;
            // 
            // lblSpell
            // 
            this.lblSpell.AutoSize = true;
            this.lblSpell.Location = new System.Drawing.Point(6, 124);
            this.lblSpell.Name = "lblSpell";
            this.lblSpell.Size = new System.Drawing.Size(59, 13);
            this.lblSpell.TabIndex = 28;
            this.lblSpell.Text = "Spell name";
            // 
            // txtSpell
            // 
            this.txtSpell.Location = new System.Drawing.Point(9, 140);
            this.txtSpell.Name = "txtSpell";
            this.txtSpell.ReadOnly = true;
            this.txtSpell.Size = new System.Drawing.Size(163, 20);
            this.txtSpell.TabIndex = 28;
            this.txtSpell.Text = "None";
            // 
            // btnAddSpell
            // 
            this.btnAddSpell.Location = new System.Drawing.Point(9, 94);
            this.btnAddSpell.Name = "btnAddSpell";
            this.btnAddSpell.Size = new System.Drawing.Size(71, 23);
            this.btnAddSpell.TabIndex = 25;
            this.btnAddSpell.Text = "Add Spell";
            this.btnAddSpell.UseVisualStyleBackColor = true;
            // 
            // CharacterSkillEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(693, 414);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "CharacterSkillEditor";
            this.Text = "Skill Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSkills.ResumeLayout(false);
            this.gbActions.ResumeLayout(false);
            this.gbActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSkills;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Button btnDeleteSpell;
        private System.Windows.Forms.Button btnSetSpell;
        private System.Windows.Forms.ListBox lsActions;
        private System.Windows.Forms.Label lblSpell;
        private System.Windows.Forms.TextBox txtSpell;
        private System.Windows.Forms.Button btnAddSpell;
    }
}