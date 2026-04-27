namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class CharacterEditor
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
            this.gbSpells = new System.Windows.Forms.GroupBox();
            this.btnDeleteAction = new System.Windows.Forms.Button();
            this.lsActions = new System.Windows.Forms.ListBox();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmQuotes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRelationships = new System.Windows.Forms.ToolStripMenuItem();
            this.progressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inventoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.gbPassiveSkills = new System.Windows.Forms.GroupBox();
            this.btnDeleteSkill = new System.Windows.Forms.Button();
            this.lsPassiveSkills = new System.Windows.Forms.ListBox();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lblPassiveSkill = new System.Windows.Forms.Label();
            this.txtPassiveSkill = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDeity = new System.Windows.Forms.TextBox();
            this.btnSetDeity = new System.Windows.Forms.Button();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.btnSetClass = new System.Windows.Forms.Button();
            this.txtBackground = new System.Windows.Forms.TextBox();
            this.btnSetBackground = new System.Windows.Forms.Button();
            this.txtAncestry = new System.Windows.Forms.TextBox();
            this.btnSetAncestry = new System.Windows.Forms.Button();
            this.cbSex = new System.Windows.Forms.ComboBox();
            this.lblSex = new System.Windows.Forms.Label();
            this.lblAge = new System.Windows.Forms.Label();
            this.txtAge = new System.Windows.Forms.NumericUpDown();
            this.lblDeity = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblBackground = new System.Windows.Forms.Label();
            this.lblAncestry = new System.Windows.Forms.Label();
            this.gbLanguages = new System.Windows.Forms.GroupBox();
            this.btnRemoveLanguage = new System.Windows.Forms.Button();
            this.btnAddLanguage = new System.Windows.Forms.Button();
            this.lsLanguages = new System.Windows.Forms.ListBox();
            this.gbPreview = new System.Windows.Forms.GroupBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.txtLevel = new System.Windows.Forms.NumericUpDown();
            this.txtInitiative = new System.Windows.Forms.TextBox();
            this.lblInitiative = new System.Windows.Forms.Label();
            this.txtPerception = new System.Windows.Forms.TextBox();
            this.lblPerception = new System.Windows.Forms.Label();
            this.txtWill = new System.Windows.Forms.TextBox();
            this.lblWill = new System.Windows.Forms.Label();
            this.txtReflex = new System.Windows.Forms.TextBox();
            this.lblReflex = new System.Windows.Forms.Label();
            this.txtFortitude = new System.Windows.Forms.TextBox();
            this.lblFortitude = new System.Windows.Forms.Label();
            this.txtCHA = new System.Windows.Forms.TextBox();
            this.lblCHA = new System.Windows.Forms.Label();
            this.txtWIS = new System.Windows.Forms.TextBox();
            this.lblWIS = new System.Windows.Forms.Label();
            this.txtINT = new System.Windows.Forms.TextBox();
            this.lblINT = new System.Windows.Forms.Label();
            this.txtCON = new System.Windows.Forms.TextBox();
            this.lblCON = new System.Windows.Forms.Label();
            this.txtDEX = new System.Windows.Forms.TextBox();
            this.lblDEX = new System.Windows.Forms.Label();
            this.txtSTR = new System.Windows.Forms.TextBox();
            this.lblSTR = new System.Windows.Forms.Label();
            this.txtSpeed = new System.Windows.Forms.TextBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtDifficultyClass = new System.Windows.Forms.TextBox();
            this.lblDifficultyClass = new System.Windows.Forms.Label();
            this.txtArmorClass = new System.Windows.Forms.TextBox();
            this.lblArmorClass = new System.Windows.Forms.Label();
            this.txtShield = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtHP = new System.Windows.Forms.TextBox();
            this.lblHP = new System.Windows.Forms.Label();
            this.gbSpells.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.gbPassiveSkills.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.txtDeity.SuspendLayout();
            this.txtClass.SuspendLayout();
            this.txtBackground.SuspendLayout();
            this.txtAncestry.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAge)).BeginInit();
            this.gbLanguages.SuspendLayout();
            this.gbPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // gbSpells
            // 
            this.gbSpells.Controls.Add(this.btnDeleteAction);
            this.gbSpells.Controls.Add(this.lsActions);
            this.gbSpells.Controls.Add(this.btnAddAction);
            this.gbSpells.Location = new System.Drawing.Point(413, 27);
            this.gbSpells.Name = "gbSpells";
            this.gbSpells.Size = new System.Drawing.Size(275, 175);
            this.gbSpells.TabIndex = 68;
            this.gbSpells.TabStop = false;
            this.gbSpells.Text = "Actions";
            // 
            // btnDeleteAction
            // 
            this.btnDeleteAction.Location = new System.Drawing.Point(182, 146);
            this.btnDeleteAction.Name = "btnDeleteAction";
            this.btnDeleteAction.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteAction.TabIndex = 68;
            this.btnDeleteAction.Text = "Delete Action";
            this.btnDeleteAction.UseVisualStyleBackColor = true;
            this.btnDeleteAction.Click += new System.EventHandler(this.btnDeleteAction_Click);
            // 
            // lsActions
            // 
            this.lsActions.FormattingEnabled = true;
            this.lsActions.Location = new System.Drawing.Point(9, 19);
            this.lsActions.Name = "lsActions";
            this.lsActions.Size = new System.Drawing.Size(260, 108);
            this.lsActions.TabIndex = 65;
            this.lsActions.SelectedIndexChanged += new System.EventHandler(this.lsActions_SelectedIndexChanged);
            // 
            // btnAddAction
            // 
            this.btnAddAction.Location = new System.Drawing.Point(6, 146);
            this.btnAddAction.Name = "btnAddAction";
            this.btnAddAction.Size = new System.Drawing.Size(71, 23);
            this.btnAddAction.TabIndex = 25;
            this.btnAddAction.Text = "Add Action";
            this.btnAddAction.UseVisualStyleBackColor = true;
            this.btnAddAction.Click += new System.EventHandler(this.btnAddAction_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmDetails,
            this.tsmQuotes,
            this.tsmRelationships,
            this.progressionToolStripMenuItem,
            this.inventoryToolStripMenuItem});
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
            // tsmDetails
            // 
            this.tsmDetails.Name = "tsmDetails";
            this.tsmDetails.Size = new System.Drawing.Size(65, 20);
            this.tsmDetails.Text = "Graphics";
            this.tsmDetails.Click += new System.EventHandler(this.tsmDetails_Click);
            // 
            // tsmQuotes
            // 
            this.tsmQuotes.Name = "tsmQuotes";
            this.tsmQuotes.Size = new System.Drawing.Size(57, 20);
            this.tsmQuotes.Text = "Quotes";
            this.tsmQuotes.Click += new System.EventHandler(this.tsmQuotes_Click);
            // 
            // tsmRelationships
            // 
            this.tsmRelationships.Name = "tsmRelationships";
            this.tsmRelationships.Size = new System.Drawing.Size(89, 20);
            this.tsmRelationships.Text = "Relationships";
            this.tsmRelationships.Click += new System.EventHandler(this.tsmRelationships_Click);
            // 
            // progressionToolStripMenuItem
            // 
            this.progressionToolStripMenuItem.Name = "progressionToolStripMenuItem";
            this.progressionToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.progressionToolStripMenuItem.Text = "Progression";
            // 
            // inventoryToolStripMenuItem
            // 
            this.inventoryToolStripMenuItem.Name = "inventoryToolStripMenuItem";
            this.inventoryToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.inventoryToolStripMenuItem.Text = "Inventory";
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
            // gbPassiveSkills
            // 
            this.gbPassiveSkills.Controls.Add(this.btnDeleteSkill);
            this.gbPassiveSkills.Controls.Add(this.lsPassiveSkills);
            this.gbPassiveSkills.Controls.Add(this.btnAddSkill);
            this.gbPassiveSkills.Controls.Add(this.lblPassiveSkill);
            this.gbPassiveSkills.Controls.Add(this.txtPassiveSkill);
            this.gbPassiveSkills.Location = new System.Drawing.Point(413, 208);
            this.gbPassiveSkills.Name = "gbPassiveSkills";
            this.gbPassiveSkills.Size = new System.Drawing.Size(275, 194);
            this.gbPassiveSkills.TabIndex = 69;
            this.gbPassiveSkills.TabStop = false;
            this.gbPassiveSkills.Text = "Passive Skills";
            // 
            // btnDeleteSkill
            // 
            this.btnDeleteSkill.Location = new System.Drawing.Point(180, 94);
            this.btnDeleteSkill.Name = "btnDeleteSkill";
            this.btnDeleteSkill.Size = new System.Drawing.Size(89, 23);
            this.btnDeleteSkill.TabIndex = 73;
            this.btnDeleteSkill.Text = "Delete Skill";
            this.btnDeleteSkill.UseVisualStyleBackColor = true;
            this.btnDeleteSkill.Click += new System.EventHandler(this.btnDeleteSkill_Click);
            // 
            // lsPassiveSkills
            // 
            this.lsPassiveSkills.FormattingEnabled = true;
            this.lsPassiveSkills.Location = new System.Drawing.Point(6, 19);
            this.lsPassiveSkills.Name = "lsPassiveSkills";
            this.lsPassiveSkills.Size = new System.Drawing.Size(263, 69);
            this.lsPassiveSkills.TabIndex = 72;
            this.lsPassiveSkills.SelectedIndexChanged += new System.EventHandler(this.lsPassiveSkills_SelectedIndexChanged);
            // 
            // btnAddSkill
            // 
            this.btnAddSkill.Location = new System.Drawing.Point(6, 94);
            this.btnAddSkill.Name = "btnAddSkill";
            this.btnAddSkill.Size = new System.Drawing.Size(71, 23);
            this.btnAddSkill.TabIndex = 71;
            this.btnAddSkill.Text = "Add Skill";
            this.btnAddSkill.UseVisualStyleBackColor = true;
            this.btnAddSkill.Click += new System.EventHandler(this.btnAddSkill_Click);
            // 
            // lblPassiveSkill
            // 
            this.lblPassiveSkill.AutoSize = true;
            this.lblPassiveSkill.Location = new System.Drawing.Point(12, 124);
            this.lblPassiveSkill.Name = "lblPassiveSkill";
            this.lblPassiveSkill.Size = new System.Drawing.Size(55, 13);
            this.lblPassiveSkill.TabIndex = 28;
            this.lblPassiveSkill.Text = "Skill name";
            // 
            // txtPassiveSkill
            // 
            this.txtPassiveSkill.Location = new System.Drawing.Point(15, 140);
            this.txtPassiveSkill.Name = "txtPassiveSkill";
            this.txtPassiveSkill.ReadOnly = true;
            this.txtPassiveSkill.Size = new System.Drawing.Size(163, 20);
            this.txtPassiveSkill.TabIndex = 28;
            this.txtPassiveSkill.Text = "None";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDeity);
            this.groupBox1.Controls.Add(this.txtClass);
            this.groupBox1.Controls.Add(this.txtBackground);
            this.groupBox1.Controls.Add(this.txtAncestry);
            this.groupBox1.Controls.Add(this.cbSex);
            this.groupBox1.Controls.Add(this.lblSex);
            this.groupBox1.Controls.Add(this.lblAge);
            this.groupBox1.Controls.Add(this.txtAge);
            this.groupBox1.Controls.Add(this.lblDeity);
            this.groupBox1.Controls.Add(this.lblClass);
            this.groupBox1.Controls.Add(this.lblBackground);
            this.groupBox1.Controls.Add(this.lblAncestry);
            this.groupBox1.Location = new System.Drawing.Point(12, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 101);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Character Information";
            // 
            // txtDeity
            // 
            this.txtDeity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeity.Controls.Add(this.btnSetDeity);
            this.txtDeity.Location = new System.Drawing.Point(80, 69);
            this.txtDeity.Name = "txtDeity";
            this.txtDeity.ReadOnly = true;
            this.txtDeity.Size = new System.Drawing.Size(110, 20);
            this.txtDeity.TabIndex = 76;
            // 
            // btnSetDeity
            // 
            this.btnSetDeity.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSetDeity.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetDeity.Location = new System.Drawing.Point(85, 0);
            this.btnSetDeity.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetDeity.Name = "btnSetDeity";
            this.btnSetDeity.Size = new System.Drawing.Size(21, 16);
            this.btnSetDeity.TabIndex = 33;
            this.btnSetDeity.TabStop = false;
            this.btnSetDeity.Text = "...";
            this.btnSetDeity.UseVisualStyleBackColor = true;
            this.btnSetDeity.Click += new System.EventHandler(this.btnSetDeity_Click);
            // 
            // txtClass
            // 
            this.txtClass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClass.Controls.Add(this.btnSetClass);
            this.txtClass.Location = new System.Drawing.Point(80, 43);
            this.txtClass.Name = "txtClass";
            this.txtClass.ReadOnly = true;
            this.txtClass.Size = new System.Drawing.Size(110, 20);
            this.txtClass.TabIndex = 75;
            // 
            // btnSetClass
            // 
            this.btnSetClass.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSetClass.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetClass.Location = new System.Drawing.Point(85, 0);
            this.btnSetClass.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetClass.Name = "btnSetClass";
            this.btnSetClass.Size = new System.Drawing.Size(21, 16);
            this.btnSetClass.TabIndex = 33;
            this.btnSetClass.TabStop = false;
            this.btnSetClass.Text = "...";
            this.btnSetClass.UseVisualStyleBackColor = true;
            this.btnSetClass.Click += new System.EventHandler(this.btnSetClass_Click);
            // 
            // txtBackground
            // 
            this.txtBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackground.Controls.Add(this.btnSetBackground);
            this.txtBackground.Location = new System.Drawing.Point(279, 17);
            this.txtBackground.Name = "txtBackground";
            this.txtBackground.ReadOnly = true;
            this.txtBackground.Size = new System.Drawing.Size(110, 20);
            this.txtBackground.TabIndex = 74;
            // 
            // btnSetBackground
            // 
            this.btnSetBackground.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSetBackground.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetBackground.Location = new System.Drawing.Point(85, 0);
            this.btnSetBackground.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetBackground.Name = "btnSetBackground";
            this.btnSetBackground.Size = new System.Drawing.Size(21, 16);
            this.btnSetBackground.TabIndex = 33;
            this.btnSetBackground.TabStop = false;
            this.btnSetBackground.Text = "...";
            this.btnSetBackground.UseVisualStyleBackColor = true;
            this.btnSetBackground.Click += new System.EventHandler(this.btnSetBackground_Click);
            // 
            // txtAncestry
            // 
            this.txtAncestry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAncestry.Controls.Add(this.btnSetAncestry);
            this.txtAncestry.Location = new System.Drawing.Point(80, 17);
            this.txtAncestry.Name = "txtAncestry";
            this.txtAncestry.ReadOnly = true;
            this.txtAncestry.Size = new System.Drawing.Size(110, 20);
            this.txtAncestry.TabIndex = 73;
            // 
            // btnSetAncestry
            // 
            this.btnSetAncestry.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSetAncestry.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSetAncestry.Location = new System.Drawing.Point(85, 0);
            this.btnSetAncestry.Margin = new System.Windows.Forms.Padding(0);
            this.btnSetAncestry.Name = "btnSetAncestry";
            this.btnSetAncestry.Size = new System.Drawing.Size(21, 16);
            this.btnSetAncestry.TabIndex = 33;
            this.btnSetAncestry.TabStop = false;
            this.btnSetAncestry.Text = "...";
            this.btnSetAncestry.UseVisualStyleBackColor = true;
            this.btnSetAncestry.Click += new System.EventHandler(this.btnSetAncestry_Click);
            // 
            // cbSex
            // 
            this.cbSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSex.FormattingEnabled = true;
            this.cbSex.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cbSex.Location = new System.Drawing.Point(319, 69);
            this.cbSex.Name = "cbSex";
            this.cbSex.Size = new System.Drawing.Size(70, 21);
            this.cbSex.TabIndex = 72;
            // 
            // lblSex
            // 
            this.lblSex.AutoSize = true;
            this.lblSex.Location = new System.Drawing.Point(239, 72);
            this.lblSex.Name = "lblSex";
            this.lblSex.Size = new System.Drawing.Size(28, 13);
            this.lblSex.TabIndex = 71;
            this.lblSex.Text = "Sex:";
            // 
            // lblAge
            // 
            this.lblAge.AutoSize = true;
            this.lblAge.Location = new System.Drawing.Point(239, 48);
            this.lblAge.Name = "lblAge";
            this.lblAge.Size = new System.Drawing.Size(29, 13);
            this.lblAge.TabIndex = 70;
            this.lblAge.Text = "Age:";
            // 
            // txtAge
            // 
            this.txtAge.Location = new System.Drawing.Point(319, 43);
            this.txtAge.Name = "txtAge";
            this.txtAge.Size = new System.Drawing.Size(70, 20);
            this.txtAge.TabIndex = 69;
            // 
            // lblDeity
            // 
            this.lblDeity.AutoSize = true;
            this.lblDeity.Location = new System.Drawing.Point(6, 72);
            this.lblDeity.Name = "lblDeity";
            this.lblDeity.Size = new System.Drawing.Size(34, 13);
            this.lblDeity.TabIndex = 34;
            this.lblDeity.Text = "Deity:";
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(6, 46);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(35, 13);
            this.lblClass.TabIndex = 32;
            this.lblClass.Text = "Class:";
            // 
            // lblBackground
            // 
            this.lblBackground.AutoSize = true;
            this.lblBackground.Location = new System.Drawing.Point(205, 21);
            this.lblBackground.Name = "lblBackground";
            this.lblBackground.Size = new System.Drawing.Size(68, 13);
            this.lblBackground.TabIndex = 30;
            this.lblBackground.Text = "Background:";
            // 
            // lblAncestry
            // 
            this.lblAncestry.AutoSize = true;
            this.lblAncestry.Location = new System.Drawing.Point(6, 20);
            this.lblAncestry.Name = "lblAncestry";
            this.lblAncestry.Size = new System.Drawing.Size(51, 13);
            this.lblAncestry.TabIndex = 23;
            this.lblAncestry.Text = "Ancestry:";
            // 
            // gbLanguages
            // 
            this.gbLanguages.Controls.Add(this.btnRemoveLanguage);
            this.gbLanguages.Controls.Add(this.btnAddLanguage);
            this.gbLanguages.Controls.Add(this.lsLanguages);
            this.gbLanguages.Location = new System.Drawing.Point(12, 285);
            this.gbLanguages.Name = "gbLanguages";
            this.gbLanguages.Size = new System.Drawing.Size(395, 89);
            this.gbLanguages.TabIndex = 75;
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
            // gbPreview
            // 
            this.gbPreview.Controls.Add(this.lblLevel);
            this.gbPreview.Controls.Add(this.txtLevel);
            this.gbPreview.Controls.Add(this.txtInitiative);
            this.gbPreview.Controls.Add(this.lblInitiative);
            this.gbPreview.Controls.Add(this.txtPerception);
            this.gbPreview.Controls.Add(this.lblPerception);
            this.gbPreview.Controls.Add(this.txtWill);
            this.gbPreview.Controls.Add(this.lblWill);
            this.gbPreview.Controls.Add(this.txtReflex);
            this.gbPreview.Controls.Add(this.lblReflex);
            this.gbPreview.Controls.Add(this.txtFortitude);
            this.gbPreview.Controls.Add(this.lblFortitude);
            this.gbPreview.Controls.Add(this.txtCHA);
            this.gbPreview.Controls.Add(this.lblCHA);
            this.gbPreview.Controls.Add(this.txtWIS);
            this.gbPreview.Controls.Add(this.lblWIS);
            this.gbPreview.Controls.Add(this.txtINT);
            this.gbPreview.Controls.Add(this.lblINT);
            this.gbPreview.Controls.Add(this.txtCON);
            this.gbPreview.Controls.Add(this.lblCON);
            this.gbPreview.Controls.Add(this.txtDEX);
            this.gbPreview.Controls.Add(this.lblDEX);
            this.gbPreview.Controls.Add(this.txtSTR);
            this.gbPreview.Controls.Add(this.lblSTR);
            this.gbPreview.Controls.Add(this.txtSpeed);
            this.gbPreview.Controls.Add(this.lblSpeed);
            this.gbPreview.Controls.Add(this.txtSize);
            this.gbPreview.Controls.Add(this.lblSize);
            this.gbPreview.Controls.Add(this.txtDifficultyClass);
            this.gbPreview.Controls.Add(this.lblDifficultyClass);
            this.gbPreview.Controls.Add(this.txtArmorClass);
            this.gbPreview.Controls.Add(this.lblArmorClass);
            this.gbPreview.Controls.Add(this.txtShield);
            this.gbPreview.Controls.Add(this.label8);
            this.gbPreview.Controls.Add(this.txtHP);
            this.gbPreview.Controls.Add(this.lblHP);
            this.gbPreview.Location = new System.Drawing.Point(694, 27);
            this.gbPreview.Name = "gbPreview";
            this.gbPreview.Size = new System.Drawing.Size(248, 379);
            this.gbPreview.TabIndex = 76;
            this.gbPreview.TabStop = false;
            this.gbPreview.Text = "Preview";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(6, 22);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(36, 13);
            this.lblLevel.TabIndex = 72;
            this.lblLevel.Text = "Level:";
            // 
            // txtLevel
            // 
            this.txtLevel.Location = new System.Drawing.Point(172, 19);
            this.txtLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtLevel.Name = "txtLevel";
            this.txtLevel.Size = new System.Drawing.Size(70, 20);
            this.txtLevel.TabIndex = 71;
            this.txtLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtInitiative
            // 
            this.txtInitiative.Location = new System.Drawing.Point(194, 258);
            this.txtInitiative.Name = "txtInitiative";
            this.txtInitiative.ReadOnly = true;
            this.txtInitiative.Size = new System.Drawing.Size(48, 20);
            this.txtInitiative.TabIndex = 65;
            // 
            // lblInitiative
            // 
            this.lblInitiative.AutoSize = true;
            this.lblInitiative.Location = new System.Drawing.Point(122, 261);
            this.lblInitiative.Name = "lblInitiative";
            this.lblInitiative.Size = new System.Drawing.Size(46, 13);
            this.lblInitiative.TabIndex = 64;
            this.lblInitiative.Text = "Initiative";
            // 
            // txtPerception
            // 
            this.txtPerception.Location = new System.Drawing.Point(68, 258);
            this.txtPerception.Name = "txtPerception";
            this.txtPerception.ReadOnly = true;
            this.txtPerception.Size = new System.Drawing.Size(48, 20);
            this.txtPerception.TabIndex = 63;
            // 
            // lblPerception
            // 
            this.lblPerception.AutoSize = true;
            this.lblPerception.Location = new System.Drawing.Point(6, 261);
            this.lblPerception.Name = "lblPerception";
            this.lblPerception.Size = new System.Drawing.Size(61, 13);
            this.lblPerception.TabIndex = 62;
            this.lblPerception.Text = "Perception:";
            // 
            // txtWill
            // 
            this.txtWill.Location = new System.Drawing.Point(194, 232);
            this.txtWill.Name = "txtWill";
            this.txtWill.ReadOnly = true;
            this.txtWill.Size = new System.Drawing.Size(48, 20);
            this.txtWill.TabIndex = 61;
            // 
            // lblWill
            // 
            this.lblWill.AutoSize = true;
            this.lblWill.Location = new System.Drawing.Point(132, 235);
            this.lblWill.Name = "lblWill";
            this.lblWill.Size = new System.Drawing.Size(27, 13);
            this.lblWill.TabIndex = 60;
            this.lblWill.Text = "Will:";
            // 
            // txtReflex
            // 
            this.txtReflex.Location = new System.Drawing.Point(194, 206);
            this.txtReflex.Name = "txtReflex";
            this.txtReflex.ReadOnly = true;
            this.txtReflex.Size = new System.Drawing.Size(48, 20);
            this.txtReflex.TabIndex = 59;
            // 
            // lblReflex
            // 
            this.lblReflex.AutoSize = true;
            this.lblReflex.Location = new System.Drawing.Point(122, 209);
            this.lblReflex.Name = "lblReflex";
            this.lblReflex.Size = new System.Drawing.Size(40, 13);
            this.lblReflex.TabIndex = 58;
            this.lblReflex.Text = "Reflex:";
            // 
            // txtFortitude
            // 
            this.txtFortitude.Location = new System.Drawing.Point(68, 206);
            this.txtFortitude.Name = "txtFortitude";
            this.txtFortitude.ReadOnly = true;
            this.txtFortitude.Size = new System.Drawing.Size(48, 20);
            this.txtFortitude.TabIndex = 57;
            // 
            // lblFortitude
            // 
            this.lblFortitude.AutoSize = true;
            this.lblFortitude.Location = new System.Drawing.Point(6, 209);
            this.lblFortitude.Name = "lblFortitude";
            this.lblFortitude.Size = new System.Drawing.Size(51, 13);
            this.lblFortitude.TabIndex = 56;
            this.lblFortitude.Text = "Fortitude:";
            // 
            // txtCHA
            // 
            this.txtCHA.Location = new System.Drawing.Point(206, 180);
            this.txtCHA.Name = "txtCHA";
            this.txtCHA.ReadOnly = true;
            this.txtCHA.Size = new System.Drawing.Size(36, 20);
            this.txtCHA.TabIndex = 55;
            // 
            // lblCHA
            // 
            this.lblCHA.AutoSize = true;
            this.lblCHA.Location = new System.Drawing.Point(122, 183);
            this.lblCHA.Name = "lblCHA";
            this.lblCHA.Size = new System.Drawing.Size(32, 13);
            this.lblCHA.TabIndex = 54;
            this.lblCHA.Text = "CHA:";
            // 
            // txtWIS
            // 
            this.txtWIS.Location = new System.Drawing.Point(80, 180);
            this.txtWIS.Name = "txtWIS";
            this.txtWIS.ReadOnly = true;
            this.txtWIS.Size = new System.Drawing.Size(36, 20);
            this.txtWIS.TabIndex = 53;
            // 
            // lblWIS
            // 
            this.lblWIS.AutoSize = true;
            this.lblWIS.Location = new System.Drawing.Point(6, 183);
            this.lblWIS.Name = "lblWIS";
            this.lblWIS.Size = new System.Drawing.Size(31, 13);
            this.lblWIS.TabIndex = 52;
            this.lblWIS.Text = "WIS:";
            // 
            // txtINT
            // 
            this.txtINT.Location = new System.Drawing.Point(206, 154);
            this.txtINT.Name = "txtINT";
            this.txtINT.ReadOnly = true;
            this.txtINT.Size = new System.Drawing.Size(36, 20);
            this.txtINT.TabIndex = 51;
            // 
            // lblINT
            // 
            this.lblINT.AutoSize = true;
            this.lblINT.Location = new System.Drawing.Point(122, 157);
            this.lblINT.Name = "lblINT";
            this.lblINT.Size = new System.Drawing.Size(28, 13);
            this.lblINT.TabIndex = 50;
            this.lblINT.Text = "INT:";
            // 
            // txtCON
            // 
            this.txtCON.Location = new System.Drawing.Point(80, 154);
            this.txtCON.Name = "txtCON";
            this.txtCON.ReadOnly = true;
            this.txtCON.Size = new System.Drawing.Size(36, 20);
            this.txtCON.TabIndex = 49;
            // 
            // lblCON
            // 
            this.lblCON.AutoSize = true;
            this.lblCON.Location = new System.Drawing.Point(6, 157);
            this.lblCON.Name = "lblCON";
            this.lblCON.Size = new System.Drawing.Size(33, 13);
            this.lblCON.TabIndex = 48;
            this.lblCON.Text = "CON:";
            // 
            // txtDEX
            // 
            this.txtDEX.Location = new System.Drawing.Point(206, 128);
            this.txtDEX.Name = "txtDEX";
            this.txtDEX.ReadOnly = true;
            this.txtDEX.Size = new System.Drawing.Size(36, 20);
            this.txtDEX.TabIndex = 47;
            // 
            // lblDEX
            // 
            this.lblDEX.AutoSize = true;
            this.lblDEX.Location = new System.Drawing.Point(122, 131);
            this.lblDEX.Name = "lblDEX";
            this.lblDEX.Size = new System.Drawing.Size(32, 13);
            this.lblDEX.TabIndex = 46;
            this.lblDEX.Text = "DEX:";
            // 
            // txtSTR
            // 
            this.txtSTR.Location = new System.Drawing.Point(80, 128);
            this.txtSTR.Name = "txtSTR";
            this.txtSTR.ReadOnly = true;
            this.txtSTR.Size = new System.Drawing.Size(36, 20);
            this.txtSTR.TabIndex = 45;
            // 
            // lblSTR
            // 
            this.lblSTR.AutoSize = true;
            this.lblSTR.Location = new System.Drawing.Point(6, 131);
            this.lblSTR.Name = "lblSTR";
            this.lblSTR.Size = new System.Drawing.Size(32, 13);
            this.lblSTR.TabIndex = 44;
            this.lblSTR.Text = "STR:";
            // 
            // txtSpeed
            // 
            this.txtSpeed.Location = new System.Drawing.Point(188, 103);
            this.txtSpeed.Name = "txtSpeed";
            this.txtSpeed.ReadOnly = true;
            this.txtSpeed.Size = new System.Drawing.Size(54, 20);
            this.txtSpeed.TabIndex = 43;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(122, 106);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(41, 13);
            this.lblSpeed.TabIndex = 42;
            this.lblSpeed.Text = "Speed:";
            // 
            // txtSize
            // 
            this.txtSize.Location = new System.Drawing.Point(62, 103);
            this.txtSize.Name = "txtSize";
            this.txtSize.ReadOnly = true;
            this.txtSize.Size = new System.Drawing.Size(54, 20);
            this.txtSize.TabIndex = 41;
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(6, 106);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(30, 13);
            this.lblSize.TabIndex = 40;
            this.lblSize.Text = "Size:";
            // 
            // txtDifficultyClass
            // 
            this.txtDifficultyClass.Location = new System.Drawing.Point(206, 77);
            this.txtDifficultyClass.Name = "txtDifficultyClass";
            this.txtDifficultyClass.ReadOnly = true;
            this.txtDifficultyClass.Size = new System.Drawing.Size(36, 20);
            this.txtDifficultyClass.TabIndex = 39;
            // 
            // lblDifficultyClass
            // 
            this.lblDifficultyClass.AutoSize = true;
            this.lblDifficultyClass.Location = new System.Drawing.Point(122, 80);
            this.lblDifficultyClass.Name = "lblDifficultyClass";
            this.lblDifficultyClass.Size = new System.Drawing.Size(78, 13);
            this.lblDifficultyClass.TabIndex = 38;
            this.lblDifficultyClass.Text = "Difficulty Class:";
            // 
            // txtArmorClass
            // 
            this.txtArmorClass.Location = new System.Drawing.Point(80, 77);
            this.txtArmorClass.Name = "txtArmorClass";
            this.txtArmorClass.ReadOnly = true;
            this.txtArmorClass.Size = new System.Drawing.Size(36, 20);
            this.txtArmorClass.TabIndex = 37;
            // 
            // lblArmorClass
            // 
            this.lblArmorClass.AutoSize = true;
            this.lblArmorClass.Location = new System.Drawing.Point(6, 80);
            this.lblArmorClass.Name = "lblArmorClass";
            this.lblArmorClass.Size = new System.Drawing.Size(65, 13);
            this.lblArmorClass.TabIndex = 36;
            this.lblArmorClass.Text = "Armor Class:";
            // 
            // txtShield
            // 
            this.txtShield.Location = new System.Drawing.Point(206, 51);
            this.txtShield.Name = "txtShield";
            this.txtShield.ReadOnly = true;
            this.txtShield.Size = new System.Drawing.Size(36, 20);
            this.txtShield.TabIndex = 35;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(122, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Shield:";
            // 
            // txtHP
            // 
            this.txtHP.Location = new System.Drawing.Point(80, 51);
            this.txtHP.Name = "txtHP";
            this.txtHP.ReadOnly = true;
            this.txtHP.Size = new System.Drawing.Size(36, 20);
            this.txtHP.TabIndex = 33;
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(6, 54);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(25, 13);
            this.lblHP.TabIndex = 32;
            this.lblHP.Text = "HP:";
            // 
            // CharacterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 414);
            this.Controls.Add(this.gbSpells);
            this.Controls.Add(this.gbPassiveSkills);
            this.Controls.Add(this.gbPreview);
            this.Controls.Add(this.gbLanguages);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "CharacterEditor";
            this.Text = "Character Editor";
            this.gbSpells.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.gbPassiveSkills.ResumeLayout(false);
            this.gbPassiveSkills.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.txtDeity.ResumeLayout(false);
            this.txtClass.ResumeLayout(false);
            this.txtBackground.ResumeLayout(false);
            this.txtAncestry.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtAge)).EndInit();
            this.gbLanguages.ResumeLayout(false);
            this.gbPreview.ResumeLayout(false);
            this.gbPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.Button btnAddAction;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmDetails;
        private System.Windows.Forms.ToolStripMenuItem tsmRelationships;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.ListBox lsActions;
        private System.Windows.Forms.ToolStripMenuItem tsmQuotes;
        private System.Windows.Forms.GroupBox gbPassiveSkills;
        private System.Windows.Forms.ListBox lsPassiveSkills;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.Label lblPassiveSkill;
        private System.Windows.Forms.TextBox txtPassiveSkill;
        private System.Windows.Forms.Button btnDeleteAction;
        private System.Windows.Forms.Button btnDeleteSkill;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblAncestry;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblBackground;
        private System.Windows.Forms.ComboBox cbSex;
        private System.Windows.Forms.Label lblSex;
        private System.Windows.Forms.Label lblAge;
        private System.Windows.Forms.NumericUpDown txtAge;
        private System.Windows.Forms.Label lblDeity;
        private System.Windows.Forms.ToolStripMenuItem progressionToolStripMenuItem;
        private System.Windows.Forms.GroupBox gbLanguages;
        private System.Windows.Forms.Button btnRemoveLanguage;
        private System.Windows.Forms.Button btnAddLanguage;
        private System.Windows.Forms.ListBox lsLanguages;
        private System.Windows.Forms.GroupBox gbPreview;
        private System.Windows.Forms.TextBox txtShield;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtHP;
        private System.Windows.Forms.Label lblHP;
        private System.Windows.Forms.TextBox txtCHA;
        private System.Windows.Forms.Label lblCHA;
        private System.Windows.Forms.TextBox txtWIS;
        private System.Windows.Forms.Label lblWIS;
        private System.Windows.Forms.TextBox txtINT;
        private System.Windows.Forms.Label lblINT;
        private System.Windows.Forms.TextBox txtCON;
        private System.Windows.Forms.Label lblCON;
        private System.Windows.Forms.TextBox txtDEX;
        private System.Windows.Forms.Label lblDEX;
        private System.Windows.Forms.TextBox txtSTR;
        private System.Windows.Forms.Label lblSTR;
        private System.Windows.Forms.TextBox txtSpeed;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtDifficultyClass;
        private System.Windows.Forms.Label lblDifficultyClass;
        private System.Windows.Forms.TextBox txtArmorClass;
        private System.Windows.Forms.Label lblArmorClass;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.NumericUpDown txtLevel;
        private System.Windows.Forms.TextBox txtInitiative;
        private System.Windows.Forms.Label lblInitiative;
        private System.Windows.Forms.TextBox txtPerception;
        private System.Windows.Forms.Label lblPerception;
        private System.Windows.Forms.TextBox txtWill;
        private System.Windows.Forms.Label lblWill;
        private System.Windows.Forms.TextBox txtReflex;
        private System.Windows.Forms.Label lblReflex;
        private System.Windows.Forms.TextBox txtFortitude;
        private System.Windows.Forms.Label lblFortitude;
        private System.Windows.Forms.ToolStripMenuItem inventoryToolStripMenuItem;
        private System.Windows.Forms.TextBox txtAncestry;
        private System.Windows.Forms.Button btnSetAncestry;
        private System.Windows.Forms.TextBox txtBackground;
        private System.Windows.Forms.Button btnSetBackground;
        private System.Windows.Forms.TextBox txtDeity;
        private System.Windows.Forms.Button btnSetDeity;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.Button btnSetClass;
    }
}