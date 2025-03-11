namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
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
            this.gbWhitelist = new System.Windows.Forms.GroupBox();
            this.btnDeleteWhitelist = new System.Windows.Forms.Button();
            this.btnSetWhitelist = new System.Windows.Forms.Button();
            this.txtWhitelist = new System.Windows.Forms.TextBox();
            this.btnAddWhitelist = new System.Windows.Forms.Button();
            this.lsWhitelist = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnDeleteBlacklist = new System.Windows.Forms.Button();
            this.btnSetBlacklist = new System.Windows.Forms.Button();
            this.txtBlacklist = new System.Windows.Forms.TextBox();
            this.btnAddBlacklist = new System.Windows.Forms.Button();
            this.lsBlacklist = new System.Windows.Forms.ListBox();
            this.gbSpells = new System.Windows.Forms.GroupBox();
            this.btnDeleteSpell = new System.Windows.Forms.Button();
            this.txtSpellCost = new System.Windows.Forms.NumericUpDown();
            this.btnSetSpell = new System.Windows.Forms.Button();
            this.lsSpells = new System.Windows.Forms.ListBox();
            this.lblSpell = new System.Windows.Forms.Label();
            this.txtSpell = new System.Windows.Forms.TextBox();
            this.lblSPCost = new System.Windows.Forms.Label();
            this.btnAddSpell = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmQuotes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRelationships = new System.Windows.Forms.ToolStripMenuItem();
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.NumericUpDown();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDeleteAIBook = new System.Windows.Forms.Button();
            this.btnSetAIBook = new System.Windows.Forms.Button();
            this.btnAddAIBook = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.lsAIBooks = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSkills = new System.Windows.Forms.TabPage();
            this.gbPassiveSkills = new System.Windows.Forms.GroupBox();
            this.btnDeleteSkill = new System.Windows.Forms.Button();
            this.lsPassiveSkills = new System.Windows.Forms.ListBox();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lblPassiveSkill = new System.Windows.Forms.Label();
            this.txtPassiveSkill = new System.Windows.Forms.TextBox();
            this.btnSetSkill = new System.Windows.Forms.Button();
            this.tabRestrictions = new System.Windows.Forms.TabPage();
            this.tabBooks = new System.Windows.Forms.TabPage();
            this.gbWhitelist.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbSpells.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpellCost)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSkills.SuspendLayout();
            this.gbPassiveSkills.SuspendLayout();
            this.tabRestrictions.SuspendLayout();
            this.tabBooks.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbWhitelist
            // 
            this.gbWhitelist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbWhitelist.Controls.Add(this.btnDeleteWhitelist);
            this.gbWhitelist.Controls.Add(this.btnSetWhitelist);
            this.gbWhitelist.Controls.Add(this.txtWhitelist);
            this.gbWhitelist.Controls.Add(this.btnAddWhitelist);
            this.gbWhitelist.Controls.Add(this.lsWhitelist);
            this.gbWhitelist.Location = new System.Drawing.Point(6, 6);
            this.gbWhitelist.Name = "gbWhitelist";
            this.gbWhitelist.Size = new System.Drawing.Size(256, 178);
            this.gbWhitelist.TabIndex = 0;
            this.gbWhitelist.TabStop = false;
            this.gbWhitelist.Text = "Whitelist";
            // 
            // btnDeleteWhitelist
            // 
            this.btnDeleteWhitelist.Location = new System.Drawing.Point(175, 149);
            this.btnDeleteWhitelist.Name = "btnDeleteWhitelist";
            this.btnDeleteWhitelist.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteWhitelist.TabIndex = 4;
            this.btnDeleteWhitelist.Text = "Delete";
            this.btnDeleteWhitelist.UseVisualStyleBackColor = true;
            this.btnDeleteWhitelist.Click += new System.EventHandler(this.btnDeleteWhitelist_Click);
            // 
            // btnSetWhitelist
            // 
            this.btnSetWhitelist.Location = new System.Drawing.Point(87, 149);
            this.btnSetWhitelist.Name = "btnSetWhitelist";
            this.btnSetWhitelist.Size = new System.Drawing.Size(75, 23);
            this.btnSetWhitelist.TabIndex = 3;
            this.btnSetWhitelist.Text = "Set";
            this.btnSetWhitelist.UseVisualStyleBackColor = true;
            this.btnSetWhitelist.Click += new System.EventHandler(this.btnSetWhitelist_Click);
            // 
            // txtWhitelist
            // 
            this.txtWhitelist.Location = new System.Drawing.Point(6, 120);
            this.txtWhitelist.Name = "txtWhitelist";
            this.txtWhitelist.ReadOnly = true;
            this.txtWhitelist.Size = new System.Drawing.Size(239, 20);
            this.txtWhitelist.TabIndex = 2;
            // 
            // btnAddWhitelist
            // 
            this.btnAddWhitelist.Location = new System.Drawing.Point(6, 149);
            this.btnAddWhitelist.Name = "btnAddWhitelist";
            this.btnAddWhitelist.Size = new System.Drawing.Size(75, 23);
            this.btnAddWhitelist.TabIndex = 1;
            this.btnAddWhitelist.Text = "Add";
            this.btnAddWhitelist.UseVisualStyleBackColor = true;
            this.btnAddWhitelist.Click += new System.EventHandler(this.btnAddWhitelist_Click);
            // 
            // lsWhitelist
            // 
            this.lsWhitelist.FormattingEnabled = true;
            this.lsWhitelist.Location = new System.Drawing.Point(6, 19);
            this.lsWhitelist.Name = "lsWhitelist";
            this.lsWhitelist.Size = new System.Drawing.Size(239, 95);
            this.lsWhitelist.TabIndex = 0;
            this.lsWhitelist.SelectedIndexChanged += new System.EventHandler(this.lsWhitelist_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnDeleteBlacklist);
            this.groupBox2.Controls.Add(this.btnSetBlacklist);
            this.groupBox2.Controls.Add(this.txtBlacklist);
            this.groupBox2.Controls.Add(this.btnAddBlacklist);
            this.groupBox2.Controls.Add(this.lsBlacklist);
            this.groupBox2.Location = new System.Drawing.Point(268, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 178);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Blacklist";
            // 
            // btnDeleteBlacklist
            // 
            this.btnDeleteBlacklist.Location = new System.Drawing.Point(175, 149);
            this.btnDeleteBlacklist.Name = "btnDeleteBlacklist";
            this.btnDeleteBlacklist.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteBlacklist.TabIndex = 5;
            this.btnDeleteBlacklist.Text = "Delete";
            this.btnDeleteBlacklist.UseVisualStyleBackColor = true;
            this.btnDeleteBlacklist.Click += new System.EventHandler(this.btnDeleteBlacklist_Click);
            // 
            // btnSetBlacklist
            // 
            this.btnSetBlacklist.Location = new System.Drawing.Point(87, 149);
            this.btnSetBlacklist.Name = "btnSetBlacklist";
            this.btnSetBlacklist.Size = new System.Drawing.Size(75, 23);
            this.btnSetBlacklist.TabIndex = 6;
            this.btnSetBlacklist.Text = "Add";
            this.btnSetBlacklist.UseVisualStyleBackColor = true;
            this.btnSetBlacklist.Click += new System.EventHandler(this.btnSetBlacklist_Click);
            // 
            // txtBlacklist
            // 
            this.txtBlacklist.Location = new System.Drawing.Point(6, 120);
            this.txtBlacklist.Name = "txtBlacklist";
            this.txtBlacklist.ReadOnly = true;
            this.txtBlacklist.Size = new System.Drawing.Size(239, 20);
            this.txtBlacklist.TabIndex = 5;
            // 
            // btnAddBlacklist
            // 
            this.btnAddBlacklist.Location = new System.Drawing.Point(6, 149);
            this.btnAddBlacklist.Name = "btnAddBlacklist";
            this.btnAddBlacklist.Size = new System.Drawing.Size(75, 23);
            this.btnAddBlacklist.TabIndex = 4;
            this.btnAddBlacklist.Text = "Add";
            this.btnAddBlacklist.UseVisualStyleBackColor = true;
            this.btnAddBlacklist.Click += new System.EventHandler(this.btnAddBlacklist_Click);
            // 
            // lsBlacklist
            // 
            this.lsBlacklist.FormattingEnabled = true;
            this.lsBlacklist.Location = new System.Drawing.Point(6, 19);
            this.lsBlacklist.Name = "lsBlacklist";
            this.lsBlacklist.Size = new System.Drawing.Size(239, 95);
            this.lsBlacklist.TabIndex = 3;
            this.lsBlacklist.SelectedIndexChanged += new System.EventHandler(this.lsBlacklist_SelectedIndexChanged);
            // 
            // gbSpells
            // 
            this.gbSpells.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbSpells.Controls.Add(this.btnDeleteSpell);
            this.gbSpells.Controls.Add(this.txtSpellCost);
            this.gbSpells.Controls.Add(this.btnSetSpell);
            this.gbSpells.Controls.Add(this.lsSpells);
            this.gbSpells.Controls.Add(this.lblSpell);
            this.gbSpells.Controls.Add(this.txtSpell);
            this.gbSpells.Controls.Add(this.lblSPCost);
            this.gbSpells.Controls.Add(this.btnAddSpell);
            this.gbSpells.Location = new System.Drawing.Point(6, 6);
            this.gbSpells.Name = "gbSpells";
            this.gbSpells.Size = new System.Drawing.Size(256, 178);
            this.gbSpells.TabIndex = 68;
            this.gbSpells.TabStop = false;
            this.gbSpells.Text = "Spells";
            // 
            // btnDeleteSpell
            // 
            this.btnDeleteSpell.Location = new System.Drawing.Point(163, 94);
            this.btnDeleteSpell.Name = "btnDeleteSpell";
            this.btnDeleteSpell.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteSpell.TabIndex = 68;
            this.btnDeleteSpell.Text = "Delete Spell";
            this.btnDeleteSpell.UseVisualStyleBackColor = true;
            this.btnDeleteSpell.Click += new System.EventHandler(this.btnDeleteSpell_Click);
            // 
            // txtSpellCost
            // 
            this.txtSpellCost.Location = new System.Drawing.Point(178, 141);
            this.txtSpellCost.Name = "txtSpellCost";
            this.txtSpellCost.Size = new System.Drawing.Size(70, 20);
            this.txtSpellCost.TabIndex = 67;
            this.txtSpellCost.ValueChanged += new System.EventHandler(this.txtSpellCost_ValueChanged);
            // 
            // btnSetSpell
            // 
            this.btnSetSpell.Location = new System.Drawing.Point(86, 94);
            this.btnSetSpell.Name = "btnSetSpell";
            this.btnSetSpell.Size = new System.Drawing.Size(71, 23);
            this.btnSetSpell.TabIndex = 66;
            this.btnSetSpell.Text = "Set Spell";
            this.btnSetSpell.UseVisualStyleBackColor = true;
            this.btnSetSpell.Click += new System.EventHandler(this.btnSetSpell_Click);
            // 
            // lsSpells
            // 
            this.lsSpells.FormattingEnabled = true;
            this.lsSpells.Location = new System.Drawing.Point(9, 19);
            this.lsSpells.Name = "lsSpells";
            this.lsSpells.Size = new System.Drawing.Size(239, 69);
            this.lsSpells.TabIndex = 65;
            this.lsSpells.SelectedIndexChanged += new System.EventHandler(this.lsSpells_SelectedIndexChanged);
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
            // lblSPCost
            // 
            this.lblSPCost.AutoSize = true;
            this.lblSPCost.Location = new System.Drawing.Point(186, 124);
            this.lblSPCost.Name = "lblSPCost";
            this.lblSPCost.Size = new System.Drawing.Size(44, 13);
            this.lblSPCost.TabIndex = 27;
            this.lblSPCost.Text = "SP cost";
            // 
            // btnAddSpell
            // 
            this.btnAddSpell.Location = new System.Drawing.Point(9, 94);
            this.btnAddSpell.Name = "btnAddSpell";
            this.btnAddSpell.Size = new System.Drawing.Size(71, 23);
            this.btnAddSpell.TabIndex = 25;
            this.btnAddSpell.Text = "Add Spell";
            this.btnAddSpell.UseVisualStyleBackColor = true;
            this.btnAddSpell.Click += new System.EventHandler(this.btnAddSpell_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmDetails,
            this.tsmQuotes,
            this.tsmRelationships});
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
            this.tsmDetails.Size = new System.Drawing.Size(54, 20);
            this.tsmDetails.Text = "Details";
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
            // gbItemInformation
            // 
            this.gbItemInformation.Controls.Add(this.lblPrice);
            this.gbItemInformation.Controls.Add(this.txtPrice);
            this.gbItemInformation.Controls.Add(this.txtDescription);
            this.gbItemInformation.Controls.Add(this.lblDescription);
            this.gbItemInformation.Controls.Add(this.lblName);
            this.gbItemInformation.Controls.Add(this.txtName);
            this.gbItemInformation.Location = new System.Drawing.Point(12, 27);
            this.gbItemInformation.Name = "gbItemInformation";
            this.gbItemInformation.Size = new System.Drawing.Size(395, 170);
            this.gbItemInformation.TabIndex = 67;
            this.gbItemInformation.TabStop = false;
            this.gbItemInformation.Text = "Item Information";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(6, 47);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(34, 13);
            this.lblPrice.TabIndex = 75;
            this.lblPrice.Text = "Price:";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(50, 45);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(70, 20);
            this.txtPrice.TabIndex = 74;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 84);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(383, 82);
            this.txtDescription.TabIndex = 73;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 68);
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnDeleteAIBook);
            this.groupBox4.Controls.Add(this.btnSetAIBook);
            this.groupBox4.Controls.Add(this.btnAddAIBook);
            this.groupBox4.Controls.Add(this.textBox3);
            this.groupBox4.Controls.Add(this.lsAIBooks);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(256, 178);
            this.groupBox4.TabIndex = 70;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "AI Books";
            // 
            // btnDeleteAIBook
            // 
            this.btnDeleteAIBook.Location = new System.Drawing.Point(175, 152);
            this.btnDeleteAIBook.Name = "btnDeleteAIBook";
            this.btnDeleteAIBook.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAIBook.TabIndex = 71;
            this.btnDeleteAIBook.Text = "Delete";
            this.btnDeleteAIBook.UseVisualStyleBackColor = true;
            this.btnDeleteAIBook.Click += new System.EventHandler(this.btnDeleteAIBook_Click);
            // 
            // btnSetAIBook
            // 
            this.btnSetAIBook.Location = new System.Drawing.Point(87, 152);
            this.btnSetAIBook.Name = "btnSetAIBook";
            this.btnSetAIBook.Size = new System.Drawing.Size(75, 23);
            this.btnSetAIBook.TabIndex = 67;
            this.btnSetAIBook.Text = "Set";
            this.btnSetAIBook.UseVisualStyleBackColor = true;
            this.btnSetAIBook.Click += new System.EventHandler(this.btnSetAIBook_Click);
            // 
            // btnAddAIBook
            // 
            this.btnAddAIBook.Location = new System.Drawing.Point(6, 152);
            this.btnAddAIBook.Name = "btnAddAIBook";
            this.btnAddAIBook.Size = new System.Drawing.Size(75, 23);
            this.btnAddAIBook.TabIndex = 5;
            this.btnAddAIBook.Text = "Add";
            this.btnAddAIBook.UseVisualStyleBackColor = true;
            this.btnAddAIBook.Click += new System.EventHandler(this.btnAddAIBook_Click);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(6, 126);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(239, 20);
            this.textBox3.TabIndex = 5;
            // 
            // lsAIBooks
            // 
            this.lsAIBooks.FormattingEnabled = true;
            this.lsAIBooks.Location = new System.Drawing.Point(6, 19);
            this.lsAIBooks.Name = "lsAIBooks";
            this.lsAIBooks.Size = new System.Drawing.Size(239, 95);
            this.lsAIBooks.TabIndex = 66;
            this.lsAIBooks.SelectedIndexChanged += new System.EventHandler(this.lsAIBooks_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSkills);
            this.tabControl1.Controls.Add(this.tabRestrictions);
            this.tabControl1.Controls.Add(this.tabBooks);
            this.tabControl1.Location = new System.Drawing.Point(413, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(537, 216);
            this.tabControl1.TabIndex = 71;
            // 
            // tabSkills
            // 
            this.tabSkills.Controls.Add(this.gbSpells);
            this.tabSkills.Controls.Add(this.gbPassiveSkills);
            this.tabSkills.Location = new System.Drawing.Point(4, 22);
            this.tabSkills.Name = "tabSkills";
            this.tabSkills.Padding = new System.Windows.Forms.Padding(3);
            this.tabSkills.Size = new System.Drawing.Size(529, 190);
            this.tabSkills.TabIndex = 0;
            this.tabSkills.Text = "Skills";
            this.tabSkills.UseVisualStyleBackColor = true;
            // 
            // gbPassiveSkills
            // 
            this.gbPassiveSkills.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPassiveSkills.Controls.Add(this.btnDeleteSkill);
            this.gbPassiveSkills.Controls.Add(this.lsPassiveSkills);
            this.gbPassiveSkills.Controls.Add(this.btnAddSkill);
            this.gbPassiveSkills.Controls.Add(this.lblPassiveSkill);
            this.gbPassiveSkills.Controls.Add(this.txtPassiveSkill);
            this.gbPassiveSkills.Controls.Add(this.btnSetSkill);
            this.gbPassiveSkills.Location = new System.Drawing.Point(268, 6);
            this.gbPassiveSkills.Name = "gbPassiveSkills";
            this.gbPassiveSkills.Size = new System.Drawing.Size(255, 178);
            this.gbPassiveSkills.TabIndex = 69;
            this.gbPassiveSkills.TabStop = false;
            this.gbPassiveSkills.Text = "Passive Skills";
            // 
            // btnDeleteSkill
            // 
            this.btnDeleteSkill.Location = new System.Drawing.Point(160, 94);
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
            this.lsPassiveSkills.Size = new System.Drawing.Size(243, 69);
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
            // btnSetSkill
            // 
            this.btnSetSkill.Location = new System.Drawing.Point(83, 94);
            this.btnSetSkill.Name = "btnSetSkill";
            this.btnSetSkill.Size = new System.Drawing.Size(71, 23);
            this.btnSetSkill.TabIndex = 25;
            this.btnSetSkill.Text = "Set Skill";
            this.btnSetSkill.UseVisualStyleBackColor = true;
            this.btnSetSkill.Click += new System.EventHandler(this.btnSetSkill_Click);
            // 
            // tabRestrictions
            // 
            this.tabRestrictions.Controls.Add(this.gbWhitelist);
            this.tabRestrictions.Controls.Add(this.groupBox2);
            this.tabRestrictions.Location = new System.Drawing.Point(4, 22);
            this.tabRestrictions.Name = "tabRestrictions";
            this.tabRestrictions.Padding = new System.Windows.Forms.Padding(3);
            this.tabRestrictions.Size = new System.Drawing.Size(529, 190);
            this.tabRestrictions.TabIndex = 1;
            this.tabRestrictions.Text = "Restrictions";
            this.tabRestrictions.UseVisualStyleBackColor = true;
            // 
            // tabBooks
            // 
            this.tabBooks.Controls.Add(this.groupBox4);
            this.tabBooks.Location = new System.Drawing.Point(4, 22);
            this.tabBooks.Name = "tabBooks";
            this.tabBooks.Size = new System.Drawing.Size(529, 190);
            this.tabBooks.TabIndex = 3;
            this.tabBooks.Text = "Books";
            this.tabBooks.UseVisualStyleBackColor = true;
            // 
            // CharacterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 251);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "CharacterEditor";
            this.Text = "Character Editor";
            this.gbWhitelist.ResumeLayout(false);
            this.gbWhitelist.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gbSpells.ResumeLayout(false);
            this.gbSpells.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpellCost)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSkills.ResumeLayout(false);
            this.gbPassiveSkills.ResumeLayout(false);
            this.gbPassiveSkills.PerformLayout();
            this.tabRestrictions.ResumeLayout(false);
            this.tabBooks.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWhitelist;
        private System.Windows.Forms.TextBox txtWhitelist;
        private System.Windows.Forms.Button btnAddWhitelist;
        private System.Windows.Forms.ListBox lsWhitelist;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.TextBox txtSpell;
        private System.Windows.Forms.Label lblSPCost;
        private System.Windows.Forms.Button btnAddSpell;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmDetails;
        private System.Windows.Forms.ToolStripMenuItem tsmRelationships;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnSetSpell;
        private System.Windows.Forms.ListBox lsSpells;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox lsAIBooks;
        private System.Windows.Forms.Label lblSpell;
        private System.Windows.Forms.Button btnDeleteAIBook;
        private System.Windows.Forms.Button btnSetAIBook;
        private System.Windows.Forms.Button btnAddAIBook;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripMenuItem tsmQuotes;
        private System.Windows.Forms.NumericUpDown txtSpellCost;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSkills;
        private System.Windows.Forms.TabPage tabRestrictions;
        private System.Windows.Forms.GroupBox gbPassiveSkills;
        private System.Windows.Forms.ListBox lsPassiveSkills;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.Label lblPassiveSkill;
        private System.Windows.Forms.TextBox txtPassiveSkill;
        private System.Windows.Forms.Button btnSetSkill;
        private System.Windows.Forms.TabPage tabBooks;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown txtPrice;
        private System.Windows.Forms.TextBox txtBlacklist;
        private System.Windows.Forms.Button btnAddBlacklist;
        private System.Windows.Forms.ListBox lsBlacklist;
        private System.Windows.Forms.Button btnSetWhitelist;
        private System.Windows.Forms.Button btnSetBlacklist;
        private System.Windows.Forms.Button btnDeleteSpell;
        private System.Windows.Forms.Button btnDeleteSkill;
        private System.Windows.Forms.Button btnDeleteWhitelist;
        private System.Windows.Forms.Button btnDeleteBlacklist;
    }
}