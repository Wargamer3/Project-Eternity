namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class CharacterBackgroundEditor
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrait = new System.Windows.Forms.Button();
            this.btnAddTrait = new System.Windows.Forms.Button();
            this.lsTraits = new System.Windows.Forms.ListBox();
            this.gbUnlockableRequirements = new System.Windows.Forms.GroupBox();
            this.cbRequirementType = new System.Windows.Forms.ComboBox();
            this.pgRequirement = new System.Windows.Forms.PropertyGrid();
            this.btnDeleteUnlockRequirement = new System.Windows.Forms.Button();
            this.lsUnlockRequirements = new System.Windows.Forms.ListBox();
            this.lblRequirementType = new System.Windows.Forms.Label();
            this.btnAddUnlockRequirement = new System.Windows.Forms.Button();
            this.gbUnlockables = new System.Windows.Forms.GroupBox();
            this.cbUnlockableType = new System.Windows.Forms.ComboBox();
            this.lblUnlockableType = new System.Windows.Forms.Label();
            this.pgUnlockable = new System.Windows.Forms.PropertyGrid();
            this.btnDeleteUnlockable = new System.Windows.Forms.Button();
            this.lsUnlockables = new System.Windows.Forms.ListBox();
            this.btnAddUnlockable = new System.Windows.Forms.Button();
            this.gbLanguages = new System.Windows.Forms.GroupBox();
            this.btnRemoveLanguage = new System.Windows.Forms.Button();
            this.btnAddLanguage = new System.Windows.Forms.Button();
            this.lsLanguages = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbUnlockableRequirements.SuspendLayout();
            this.gbUnlockables.SuspendLayout();
            this.gbLanguages.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(954, 24);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemoveTrait);
            this.groupBox2.Controls.Add(this.btnAddTrait);
            this.groupBox2.Controls.Add(this.lsTraits);
            this.groupBox2.Location = new System.Drawing.Point(12, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 85);
            this.groupBox2.TabIndex = 75;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Traits";
            // 
            // btnRemoveTrait
            // 
            this.btnRemoveTrait.Location = new System.Drawing.Point(273, 48);
            this.btnRemoveTrait.Name = "btnRemoveTrait";
            this.btnRemoveTrait.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveTrait.TabIndex = 75;
            this.btnRemoveTrait.Text = "Remove Trait";
            this.btnRemoveTrait.UseVisualStyleBackColor = true;
            this.btnRemoveTrait.Click += new System.EventHandler(this.btnRemoveTrait_Click);
            // 
            // btnAddTrait
            // 
            this.btnAddTrait.Location = new System.Drawing.Point(273, 19);
            this.btnAddTrait.Name = "btnAddTrait";
            this.btnAddTrait.Size = new System.Drawing.Size(116, 23);
            this.btnAddTrait.TabIndex = 74;
            this.btnAddTrait.Text = "Add Trait";
            this.btnAddTrait.UseVisualStyleBackColor = true;
            this.btnAddTrait.Click += new System.EventHandler(this.btnAddTrait_Click);
            // 
            // lsTraits
            // 
            this.lsTraits.FormattingEnabled = true;
            this.lsTraits.Location = new System.Drawing.Point(6, 19);
            this.lsTraits.Name = "lsTraits";
            this.lsTraits.Size = new System.Drawing.Size(261, 56);
            this.lsTraits.TabIndex = 73;
            // 
            // gbUnlockableRequirements
            // 
            this.gbUnlockableRequirements.Controls.Add(this.cbRequirementType);
            this.gbUnlockableRequirements.Controls.Add(this.pgRequirement);
            this.gbUnlockableRequirements.Controls.Add(this.btnDeleteUnlockRequirement);
            this.gbUnlockableRequirements.Controls.Add(this.lsUnlockRequirements);
            this.gbUnlockableRequirements.Controls.Add(this.lblRequirementType);
            this.gbUnlockableRequirements.Controls.Add(this.btnAddUnlockRequirement);
            this.gbUnlockableRequirements.Enabled = false;
            this.gbUnlockableRequirements.Location = new System.Drawing.Point(681, 16);
            this.gbUnlockableRequirements.Name = "gbUnlockableRequirements";
            this.gbUnlockableRequirements.Size = new System.Drawing.Size(256, 408);
            this.gbUnlockableRequirements.TabIndex = 86;
            this.gbUnlockableRequirements.TabStop = false;
            this.gbUnlockableRequirements.Text = "Unlockable Requirements";
            // 
            // cbRequirementType
            // 
            this.cbRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRequirementType.FormattingEnabled = true;
            this.cbRequirementType.Location = new System.Drawing.Point(6, 120);
            this.cbRequirementType.Name = "cbRequirementType";
            this.cbRequirementType.Size = new System.Drawing.Size(244, 21);
            this.cbRequirementType.TabIndex = 73;
            this.cbRequirementType.SelectedIndexChanged += new System.EventHandler(this.cbRequirementType_SelectedIndexChanged);
            // 
            // pgRequirement
            // 
            this.pgRequirement.Location = new System.Drawing.Point(6, 147);
            this.pgRequirement.Name = "pgRequirement";
            this.pgRequirement.Size = new System.Drawing.Size(244, 255);
            this.pgRequirement.TabIndex = 69;
            // 
            // btnDeleteUnlockRequirement
            // 
            this.btnDeleteUnlockRequirement.Location = new System.Drawing.Point(163, 48);
            this.btnDeleteUnlockRequirement.Name = "btnDeleteUnlockRequirement";
            this.btnDeleteUnlockRequirement.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteUnlockRequirement.TabIndex = 68;
            this.btnDeleteUnlockRequirement.Text = "Delete";
            this.btnDeleteUnlockRequirement.UseVisualStyleBackColor = true;
            this.btnDeleteUnlockRequirement.Click += new System.EventHandler(this.btnDeleteUnlockRequirement_Click);
            // 
            // lsUnlockRequirements
            // 
            this.lsUnlockRequirements.FormattingEnabled = true;
            this.lsUnlockRequirements.Location = new System.Drawing.Point(9, 19);
            this.lsUnlockRequirements.Name = "lsUnlockRequirements";
            this.lsUnlockRequirements.Size = new System.Drawing.Size(148, 82);
            this.lsUnlockRequirements.TabIndex = 65;
            this.lsUnlockRequirements.SelectedIndexChanged += new System.EventHandler(this.lsUnlockRequirements_SelectedIndexChanged);
            // 
            // lblRequirementType
            // 
            this.lblRequirementType.AutoSize = true;
            this.lblRequirementType.Location = new System.Drawing.Point(6, 104);
            this.lblRequirementType.Name = "lblRequirementType";
            this.lblRequirementType.Size = new System.Drawing.Size(94, 13);
            this.lblRequirementType.TabIndex = 28;
            this.lblRequirementType.Text = "Requirement Type";
            // 
            // btnAddUnlockRequirement
            // 
            this.btnAddUnlockRequirement.Location = new System.Drawing.Point(163, 19);
            this.btnAddUnlockRequirement.Name = "btnAddUnlockRequirement";
            this.btnAddUnlockRequirement.Size = new System.Drawing.Size(87, 23);
            this.btnAddUnlockRequirement.TabIndex = 25;
            this.btnAddUnlockRequirement.Text = "Add";
            this.btnAddUnlockRequirement.UseVisualStyleBackColor = true;
            this.btnAddUnlockRequirement.Click += new System.EventHandler(this.btnAddUnlockRequirement_Click);
            // 
            // gbUnlockables
            // 
            this.gbUnlockables.Controls.Add(this.cbUnlockableType);
            this.gbUnlockables.Controls.Add(this.lblUnlockableType);
            this.gbUnlockables.Controls.Add(this.pgUnlockable);
            this.gbUnlockables.Controls.Add(this.btnDeleteUnlockable);
            this.gbUnlockables.Controls.Add(this.lsUnlockables);
            this.gbUnlockables.Controls.Add(this.btnAddUnlockable);
            this.gbUnlockables.Location = new System.Drawing.Point(419, 16);
            this.gbUnlockables.Name = "gbUnlockables";
            this.gbUnlockables.Size = new System.Drawing.Size(256, 408);
            this.gbUnlockables.TabIndex = 85;
            this.gbUnlockables.TabStop = false;
            this.gbUnlockables.Text = "Unlockables";
            // 
            // cbUnlockableType
            // 
            this.cbUnlockableType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnlockableType.FormattingEnabled = true;
            this.cbUnlockableType.Location = new System.Drawing.Point(6, 120);
            this.cbUnlockableType.Name = "cbUnlockableType";
            this.cbUnlockableType.Size = new System.Drawing.Size(244, 21);
            this.cbUnlockableType.TabIndex = 72;
            this.cbUnlockableType.SelectedIndexChanged += new System.EventHandler(this.cbUnlockableType_SelectedIndexChanged);
            // 
            // lblUnlockableType
            // 
            this.lblUnlockableType.AutoSize = true;
            this.lblUnlockableType.Location = new System.Drawing.Point(6, 104);
            this.lblUnlockableType.Name = "lblUnlockableType";
            this.lblUnlockableType.Size = new System.Drawing.Size(88, 13);
            this.lblUnlockableType.TabIndex = 70;
            this.lblUnlockableType.Text = "Unlockable Type";
            // 
            // pgUnlockable
            // 
            this.pgUnlockable.Location = new System.Drawing.Point(6, 147);
            this.pgUnlockable.Name = "pgUnlockable";
            this.pgUnlockable.Size = new System.Drawing.Size(244, 255);
            this.pgUnlockable.TabIndex = 69;
            // 
            // btnDeleteUnlockable
            // 
            this.btnDeleteUnlockable.Location = new System.Drawing.Point(163, 48);
            this.btnDeleteUnlockable.Name = "btnDeleteUnlockable";
            this.btnDeleteUnlockable.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteUnlockable.TabIndex = 68;
            this.btnDeleteUnlockable.Text = "Delete";
            this.btnDeleteUnlockable.UseVisualStyleBackColor = true;
            this.btnDeleteUnlockable.Click += new System.EventHandler(this.btnDeleteUnlockable_Click);
            // 
            // lsUnlockables
            // 
            this.lsUnlockables.FormattingEnabled = true;
            this.lsUnlockables.Location = new System.Drawing.Point(9, 19);
            this.lsUnlockables.Name = "lsUnlockables";
            this.lsUnlockables.Size = new System.Drawing.Size(148, 82);
            this.lsUnlockables.TabIndex = 65;
            this.lsUnlockables.SelectedIndexChanged += new System.EventHandler(this.lsUnlockables_SelectedIndexChanged);
            // 
            // btnAddUnlockable
            // 
            this.btnAddUnlockable.Location = new System.Drawing.Point(163, 19);
            this.btnAddUnlockable.Name = "btnAddUnlockable";
            this.btnAddUnlockable.Size = new System.Drawing.Size(87, 23);
            this.btnAddUnlockable.TabIndex = 25;
            this.btnAddUnlockable.Text = "Add";
            this.btnAddUnlockable.UseVisualStyleBackColor = true;
            this.btnAddUnlockable.Click += new System.EventHandler(this.btnAddUnlockable_Click);
            // 
            // gbLanguages
            // 
            this.gbLanguages.Controls.Add(this.btnRemoveLanguage);
            this.gbLanguages.Controls.Add(this.btnAddLanguage);
            this.gbLanguages.Controls.Add(this.lsLanguages);
            this.gbLanguages.Location = new System.Drawing.Point(12, 269);
            this.gbLanguages.Name = "gbLanguages";
            this.gbLanguages.Size = new System.Drawing.Size(395, 88);
            this.gbLanguages.TabIndex = 84;
            this.gbLanguages.TabStop = false;
            this.gbLanguages.Text = "Languages";
            // 
            // btnRemoveLanguage
            // 
            this.btnRemoveLanguage.Location = new System.Drawing.Point(273, 48);
            this.btnRemoveLanguage.Name = "btnRemoveLanguage";
            this.btnRemoveLanguage.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveLanguage.TabIndex = 70;
            this.btnRemoveLanguage.Text = "Remove Language";
            this.btnRemoveLanguage.UseVisualStyleBackColor = true;
            this.btnRemoveLanguage.Click += new System.EventHandler(this.btnRemoveLanguage_Click);
            // 
            // btnAddLanguage
            // 
            this.btnAddLanguage.Location = new System.Drawing.Point(273, 19);
            this.btnAddLanguage.Name = "btnAddLanguage";
            this.btnAddLanguage.Size = new System.Drawing.Size(116, 23);
            this.btnAddLanguage.TabIndex = 69;
            this.btnAddLanguage.Text = "Add Language";
            this.btnAddLanguage.UseVisualStyleBackColor = true;
            this.btnAddLanguage.Click += new System.EventHandler(this.btnAddLanguage_Click);
            // 
            // lsLanguages
            // 
            this.lsLanguages.FormattingEnabled = true;
            this.lsLanguages.Location = new System.Drawing.Point(6, 19);
            this.lsLanguages.Name = "lsLanguages";
            this.lsLanguages.Size = new System.Drawing.Size(261, 56);
            this.lsLanguages.TabIndex = 66;
            // 
            // CharacterBackgroundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 441);
            this.Controls.Add(this.gbUnlockableRequirements);
            this.Controls.Add(this.gbUnlockables);
            this.Controls.Add(this.gbLanguages);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "CharacterBackgroundEditor";
            this.Text = "Background Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.gbUnlockableRequirements.ResumeLayout(false);
            this.gbUnlockableRequirements.PerformLayout();
            this.gbUnlockables.ResumeLayout(false);
            this.gbUnlockables.PerformLayout();
            this.gbLanguages.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemoveTrait;
        private System.Windows.Forms.Button btnAddTrait;
        private System.Windows.Forms.ListBox lsTraits;
        private System.Windows.Forms.GroupBox gbUnlockableRequirements;
        private System.Windows.Forms.ComboBox cbRequirementType;
        private System.Windows.Forms.PropertyGrid pgRequirement;
        private System.Windows.Forms.Button btnDeleteUnlockRequirement;
        private System.Windows.Forms.ListBox lsUnlockRequirements;
        private System.Windows.Forms.Label lblRequirementType;
        private System.Windows.Forms.Button btnAddUnlockRequirement;
        private System.Windows.Forms.GroupBox gbUnlockables;
        private System.Windows.Forms.ComboBox cbUnlockableType;
        private System.Windows.Forms.Label lblUnlockableType;
        private System.Windows.Forms.PropertyGrid pgUnlockable;
        private System.Windows.Forms.Button btnDeleteUnlockable;
        private System.Windows.Forms.ListBox lsUnlockables;
        private System.Windows.Forms.Button btnAddUnlockable;
        private System.Windows.Forms.GroupBox gbLanguages;
        private System.Windows.Forms.Button btnRemoveLanguage;
        private System.Windows.Forms.Button btnAddLanguage;
        private System.Windows.Forms.ListBox lsLanguages;
    }
}