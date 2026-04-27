namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class ItemEditor
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
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSkills = new System.Windows.Forms.TabPage();
            this.gbPassiveSkills = new System.Windows.Forms.GroupBox();
            this.btnDeleteSkill = new System.Windows.Forms.Button();
            this.lsPassiveSkills = new System.Windows.Forms.ListBox();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lblPassiveSkill = new System.Windows.Forms.Label();
            this.txtPassiveSkill = new System.Windows.Forms.TextBox();
            this.btnSetSkill = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrait = new System.Windows.Forms.Button();
            this.btnAddTrait = new System.Windows.Forms.Button();
            this.lsTraits = new System.Windows.Forms.ListBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numericUpDown8 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.gbSpells.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpellCost)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSkills.SuspendLayout();
            this.gbPassiveSkills.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSpells
            // 
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
            this.gbSpells.Size = new System.Drawing.Size(256, 175);
            this.gbSpells.TabIndex = 68;
            this.gbSpells.TabStop = false;
            this.gbSpells.Text = "Actions";
            // 
            // btnDeleteSpell
            // 
            this.btnDeleteSpell.Location = new System.Drawing.Point(163, 94);
            this.btnDeleteSpell.Name = "btnDeleteSpell";
            this.btnDeleteSpell.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteSpell.TabIndex = 68;
            this.btnDeleteSpell.Text = "Delete Spell";
            this.btnDeleteSpell.UseVisualStyleBackColor = true;
            this.btnDeleteSpell.Click += new System.EventHandler(this.btnDeleteAction_Click);
            // 
            // txtSpellCost
            // 
            this.txtSpellCost.Location = new System.Drawing.Point(178, 141);
            this.txtSpellCost.Name = "txtSpellCost";
            this.txtSpellCost.Size = new System.Drawing.Size(70, 20);
            this.txtSpellCost.TabIndex = 67;
            this.txtSpellCost.ValueChanged += new System.EventHandler(this.txtActionCost_ValueChanged);
            // 
            // btnSetSpell
            // 
            this.btnSetSpell.Location = new System.Drawing.Point(86, 94);
            this.btnSetSpell.Name = "btnSetSpell";
            this.btnSetSpell.Size = new System.Drawing.Size(71, 23);
            this.btnSetSpell.TabIndex = 66;
            this.btnSetSpell.Text = "Set Spell";
            this.btnSetSpell.UseVisualStyleBackColor = true;
            this.btnSetSpell.Click += new System.EventHandler(this.btnSetAction_Click);
            // 
            // lsSpells
            // 
            this.lsSpells.FormattingEnabled = true;
            this.lsSpells.Location = new System.Drawing.Point(9, 19);
            this.lsSpells.Name = "lsSpells";
            this.lsSpells.Size = new System.Drawing.Size(239, 69);
            this.lsSpells.TabIndex = 65;
            this.lsSpells.SelectedIndexChanged += new System.EventHandler(this.lsActions_SelectedIndexChanged);
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
            this.btnAddSpell.Click += new System.EventHandler(this.btnAddAction_Click);
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSkills);
            this.tabControl1.Location = new System.Drawing.Point(413, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(275, 383);
            this.tabControl1.TabIndex = 71;
            // 
            // tabSkills
            // 
            this.tabSkills.Controls.Add(this.gbSpells);
            this.tabSkills.Controls.Add(this.gbPassiveSkills);
            this.tabSkills.Location = new System.Drawing.Point(4, 22);
            this.tabSkills.Name = "tabSkills";
            this.tabSkills.Padding = new System.Windows.Forms.Padding(3);
            this.tabSkills.Size = new System.Drawing.Size(267, 357);
            this.tabSkills.TabIndex = 0;
            this.tabSkills.Text = "Skills";
            this.tabSkills.UseVisualStyleBackColor = true;
            // 
            // gbPassiveSkills
            // 
            this.gbPassiveSkills.Controls.Add(this.btnDeleteSkill);
            this.gbPassiveSkills.Controls.Add(this.lsPassiveSkills);
            this.gbPassiveSkills.Controls.Add(this.btnAddSkill);
            this.gbPassiveSkills.Controls.Add(this.lblPassiveSkill);
            this.gbPassiveSkills.Controls.Add(this.txtPassiveSkill);
            this.gbPassiveSkills.Controls.Add(this.btnSetSkill);
            this.gbPassiveSkills.Location = new System.Drawing.Point(6, 187);
            this.gbPassiveSkills.Name = "gbPassiveSkills";
            this.gbPassiveSkills.Size = new System.Drawing.Size(255, 175);
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
            // 
            // btnAddTrait
            // 
            this.btnAddTrait.Location = new System.Drawing.Point(273, 19);
            this.btnAddTrait.Name = "btnAddTrait";
            this.btnAddTrait.Size = new System.Drawing.Size(116, 23);
            this.btnAddTrait.TabIndex = 74;
            this.btnAddTrait.Text = "Add Trait";
            this.btnAddTrait.UseVisualStyleBackColor = true;
            // 
            // lsTraits
            // 
            this.lsTraits.FormattingEnabled = true;
            this.lsTraits.Location = new System.Drawing.Point(6, 19);
            this.lsTraits.Name = "lsTraits";
            this.lsTraits.Size = new System.Drawing.Size(261, 56);
            this.lsTraits.TabIndex = 73;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numericUpDown8);
            this.groupBox4.Controls.Add(this.numericUpDown7);
            this.groupBox4.Controls.Add(this.numericUpDown6);
            this.groupBox4.Controls.Add(this.numericUpDown5);
            this.groupBox4.Controls.Add(this.numericUpDown4);
            this.groupBox4.Controls.Add(this.numericUpDown3);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Controls.Add(this.label13);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.numericUpDown2);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Location = new System.Drawing.Point(690, 27);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(252, 375);
            this.groupBox4.TabIndex = 77;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Bonus Stats";
            // 
            // numericUpDown8
            // 
            this.numericUpDown8.Location = new System.Drawing.Point(46, 56);
            this.numericUpDown8.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown8.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown8.Name = "numericUpDown8";
            this.numericUpDown8.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown8.TabIndex = 80;
            // 
            // numericUpDown7
            // 
            this.numericUpDown7.Location = new System.Drawing.Point(46, 82);
            this.numericUpDown7.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown7.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown7.Name = "numericUpDown7";
            this.numericUpDown7.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown7.TabIndex = 79;
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.Location = new System.Drawing.Point(46, 108);
            this.numericUpDown6.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown6.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown6.TabIndex = 78;
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Location = new System.Drawing.Point(176, 56);
            this.numericUpDown5.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown5.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown5.TabIndex = 77;
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Location = new System.Drawing.Point(176, 82);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown4.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown4.TabIndex = 76;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(176, 108);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown3.TabIndex = 75;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(122, 110);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 13);
            this.label17.TabIndex = 74;
            this.label17.Text = "CHA:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 110);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 73;
            this.label18.Text = "WIS:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(122, 84);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(28, 13);
            this.label15.TabIndex = 72;
            this.label15.Text = "INT:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 84);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 71;
            this.label16.Text = "CON:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(122, 58);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 70;
            this.label13.Text = "DEX:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 58);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 69;
            this.label14.Text = "STR:";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(176, 19);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown2.TabIndex = 68;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 23;
            this.label4.Text = "Free Boost:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(12, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 133);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base Stats";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(62, 19);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(105, 20);
            this.textBox2.TabIndex = 74;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 73;
            this.label5.Text = "Bulk:";
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 414);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "ItemEditor";
            this.Text = "Item Editor";
            this.gbSpells.ResumeLayout(false);
            this.gbSpells.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpellCost)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabSkills.ResumeLayout(false);
            this.gbPassiveSkills.ResumeLayout(false);
            this.gbPassiveSkills.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.TextBox txtSpell;
        private System.Windows.Forms.Label lblSPCost;
        private System.Windows.Forms.Button btnAddSpell;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Button btnSetSpell;
        private System.Windows.Forms.ListBox lsSpells;
        private System.Windows.Forms.Label lblSpell;
        private System.Windows.Forms.NumericUpDown txtSpellCost;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSkills;
        private System.Windows.Forms.GroupBox gbPassiveSkills;
        private System.Windows.Forms.ListBox lsPassiveSkills;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.Label lblPassiveSkill;
        private System.Windows.Forms.TextBox txtPassiveSkill;
        private System.Windows.Forms.Button btnSetSkill;
        private System.Windows.Forms.Button btnDeleteSpell;
        private System.Windows.Forms.Button btnDeleteSkill;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemoveTrait;
        private System.Windows.Forms.Button btnAddTrait;
        private System.Windows.Forms.ListBox lsTraits;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numericUpDown8;
        private System.Windows.Forms.NumericUpDown numericUpDown7;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
    }
}