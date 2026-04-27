namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class WeaponEditor
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
            this.numericUpDown9 = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.gbBaseStats = new System.Windows.Forms.GroupBox();
            this.cbHands = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cbCategory = new System.Windows.Forms.ComboBox();
            this.lblGroup = new System.Windows.Forms.Label();
            this.cbGroup = new System.Windows.Forms.ComboBox();
            this.txtBulk = new System.Windows.Forms.TextBox();
            this.lblBulk = new System.Windows.Forms.Label();
            this.lblWeaponType = new System.Windows.Forms.Label();
            this.cbWeaponType = new System.Windows.Forms.ComboBox();
            this.txtDamage = new System.Windows.Forms.TextBox();
            this.lblHands = new System.Windows.Forms.Label();
            this.lblDamage = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrait = new System.Windows.Forms.Button();
            this.btnAddTrait = new System.Windows.Forms.Button();
            this.lsTraits = new System.Windows.Forms.ListBox();
            this.gbBonusStats = new System.Windows.Forms.GroupBox();
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
            this.gbRangeStats = new System.Windows.Forms.GroupBox();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.lblReload = new System.Windows.Forms.Label();
            this.txtReload = new System.Windows.Forms.NumericUpDown();
            this.lblRange = new System.Windows.Forms.Label();
            this.gbSpells = new System.Windows.Forms.GroupBox();
            this.btnDeleteAction = new System.Windows.Forms.Button();
            this.lsActions = new System.Windows.Forms.ListBox();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.gbPassiveSkills = new System.Windows.Forms.GroupBox();
            this.btnDeleteSkill = new System.Windows.Forms.Button();
            this.lsPassiveSkills = new System.Windows.Forms.ListBox();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lblPassiveSkill = new System.Windows.Forms.Label();
            this.txtPassiveSkill = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown9)).BeginInit();
            this.gbBaseStats.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.gbBonusStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.gbRangeStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReload)).BeginInit();
            this.gbSpells.SuspendLayout();
            this.gbPassiveSkills.SuspendLayout();
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
            this.gbItemInformation.Controls.Add(this.numericUpDown9);
            this.gbItemInformation.Controls.Add(this.label3);
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
            // numericUpDown9
            // 
            this.numericUpDown9.Location = new System.Drawing.Point(319, 19);
            this.numericUpDown9.Name = "numericUpDown9";
            this.numericUpDown9.Size = new System.Drawing.Size(70, 20);
            this.numericUpDown9.TabIndex = 75;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 74;
            this.label3.Text = "Price:";
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
            this.txtName.Size = new System.Drawing.Size(223, 20);
            this.txtName.TabIndex = 22;
            // 
            // gbBaseStats
            // 
            this.gbBaseStats.Controls.Add(this.cbHands);
            this.gbBaseStats.Controls.Add(this.lblCategory);
            this.gbBaseStats.Controls.Add(this.cbCategory);
            this.gbBaseStats.Controls.Add(this.lblGroup);
            this.gbBaseStats.Controls.Add(this.cbGroup);
            this.gbBaseStats.Controls.Add(this.txtBulk);
            this.gbBaseStats.Controls.Add(this.lblBulk);
            this.gbBaseStats.Controls.Add(this.lblWeaponType);
            this.gbBaseStats.Controls.Add(this.cbWeaponType);
            this.gbBaseStats.Controls.Add(this.txtDamage);
            this.gbBaseStats.Controls.Add(this.lblHands);
            this.gbBaseStats.Controls.Add(this.lblDamage);
            this.gbBaseStats.Location = new System.Drawing.Point(12, 269);
            this.gbBaseStats.Name = "gbBaseStats";
            this.gbBaseStats.Size = new System.Drawing.Size(395, 101);
            this.gbBaseStats.TabIndex = 74;
            this.gbBaseStats.TabStop = false;
            this.gbBaseStats.Text = "Base Stats";
            // 
            // cbHands
            // 
            this.cbHands.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHands.FormattingEnabled = true;
            this.cbHands.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cbHands.Location = new System.Drawing.Point(64, 45);
            this.cbHands.Name = "cbHands";
            this.cbHands.Size = new System.Drawing.Size(103, 21);
            this.cbHands.TabIndex = 79;
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(6, 77);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(52, 13);
            this.lblCategory.TabIndex = 78;
            this.lblCategory.Text = "Category:";
            // 
            // cbCategory
            // 
            this.cbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCategory.FormattingEnabled = true;
            this.cbCategory.Items.AddRange(new object[] {
            "Martial",
            "Ammunition"});
            this.cbCategory.Location = new System.Drawing.Point(64, 74);
            this.cbCategory.Name = "cbCategory";
            this.cbCategory.Size = new System.Drawing.Size(103, 21);
            this.cbCategory.TabIndex = 77;
            // 
            // lblGroup
            // 
            this.lblGroup.AutoSize = true;
            this.lblGroup.Location = new System.Drawing.Point(223, 77);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(39, 13);
            this.lblGroup.TabIndex = 76;
            this.lblGroup.Text = "Group:";
            // 
            // cbGroup
            // 
            this.cbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGroup.FormattingEnabled = true;
            this.cbGroup.Items.AddRange(new object[] {
            "Sword",
            "Firearm"});
            this.cbGroup.Location = new System.Drawing.Point(268, 74);
            this.cbGroup.Name = "cbGroup";
            this.cbGroup.Size = new System.Drawing.Size(121, 21);
            this.cbGroup.TabIndex = 75;
            // 
            // txtBulk
            // 
            this.txtBulk.Location = new System.Drawing.Point(284, 19);
            this.txtBulk.Name = "txtBulk";
            this.txtBulk.Size = new System.Drawing.Size(105, 20);
            this.txtBulk.TabIndex = 74;
            // 
            // lblBulk
            // 
            this.lblBulk.AutoSize = true;
            this.lblBulk.Location = new System.Drawing.Point(231, 22);
            this.lblBulk.Name = "lblBulk";
            this.lblBulk.Size = new System.Drawing.Size(31, 13);
            this.lblBulk.TabIndex = 73;
            this.lblBulk.Text = "Bulk:";
            // 
            // lblWeaponType
            // 
            this.lblWeaponType.AutoSize = true;
            this.lblWeaponType.Location = new System.Drawing.Point(184, 48);
            this.lblWeaponType.Name = "lblWeaponType";
            this.lblWeaponType.Size = new System.Drawing.Size(78, 13);
            this.lblWeaponType.TabIndex = 72;
            this.lblWeaponType.Text = "Weapon Type:";
            // 
            // cbWeaponType
            // 
            this.cbWeaponType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeaponType.FormattingEnabled = true;
            this.cbWeaponType.Items.AddRange(new object[] {
            "Melee",
            "Range"});
            this.cbWeaponType.Location = new System.Drawing.Point(268, 45);
            this.cbWeaponType.Name = "cbWeaponType";
            this.cbWeaponType.Size = new System.Drawing.Size(121, 21);
            this.cbWeaponType.TabIndex = 71;
            // 
            // txtDamage
            // 
            this.txtDamage.Location = new System.Drawing.Point(62, 19);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(105, 20);
            this.txtDamage.TabIndex = 70;
            // 
            // lblHands
            // 
            this.lblHands.AutoSize = true;
            this.lblHands.Location = new System.Drawing.Point(6, 48);
            this.lblHands.Name = "lblHands";
            this.lblHands.Size = new System.Drawing.Size(41, 13);
            this.lblHands.TabIndex = 69;
            this.lblHands.Text = "Hands:";
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(6, 22);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(50, 13);
            this.lblDamage.TabIndex = 23;
            this.lblDamage.Text = "Damage:";
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
            // gbBonusStats
            // 
            this.gbBonusStats.Controls.Add(this.numericUpDown8);
            this.gbBonusStats.Controls.Add(this.numericUpDown7);
            this.gbBonusStats.Controls.Add(this.numericUpDown6);
            this.gbBonusStats.Controls.Add(this.numericUpDown5);
            this.gbBonusStats.Controls.Add(this.numericUpDown4);
            this.gbBonusStats.Controls.Add(this.numericUpDown3);
            this.gbBonusStats.Controls.Add(this.label17);
            this.gbBonusStats.Controls.Add(this.label18);
            this.gbBonusStats.Controls.Add(this.label15);
            this.gbBonusStats.Controls.Add(this.label16);
            this.gbBonusStats.Controls.Add(this.label13);
            this.gbBonusStats.Controls.Add(this.label14);
            this.gbBonusStats.Location = new System.Drawing.Point(690, 27);
            this.gbBonusStats.Name = "gbBonusStats";
            this.gbBonusStats.Size = new System.Drawing.Size(252, 375);
            this.gbBonusStats.TabIndex = 77;
            this.gbBonusStats.TabStop = false;
            this.gbBonusStats.Text = "Bonus Stats";
            // 
            // numericUpDown8
            // 
            this.numericUpDown8.Location = new System.Drawing.Point(46, 19);
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
            this.numericUpDown7.Location = new System.Drawing.Point(46, 45);
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
            this.numericUpDown6.Location = new System.Drawing.Point(46, 71);
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
            this.numericUpDown5.Location = new System.Drawing.Point(176, 19);
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
            this.numericUpDown4.Location = new System.Drawing.Point(176, 45);
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
            this.numericUpDown3.Location = new System.Drawing.Point(176, 71);
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
            this.label17.Location = new System.Drawing.Point(122, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 13);
            this.label17.TabIndex = 74;
            this.label17.Text = "CHA:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(6, 73);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(31, 13);
            this.label18.TabIndex = 73;
            this.label18.Text = "WIS:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(122, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(28, 13);
            this.label15.TabIndex = 72;
            this.label15.Text = "INT:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 47);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(33, 13);
            this.label16.TabIndex = 71;
            this.label16.Text = "CON:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(122, 21);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 70;
            this.label13.Text = "DEX:";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 21);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 13);
            this.label14.TabIndex = 69;
            this.label14.Text = "STR:";
            // 
            // gbRangeStats
            // 
            this.gbRangeStats.Controls.Add(this.txtRange);
            this.gbRangeStats.Controls.Add(this.lblReload);
            this.gbRangeStats.Controls.Add(this.txtReload);
            this.gbRangeStats.Controls.Add(this.lblRange);
            this.gbRangeStats.Location = new System.Drawing.Point(12, 376);
            this.gbRangeStats.Name = "gbRangeStats";
            this.gbRangeStats.Size = new System.Drawing.Size(395, 49);
            this.gbRangeStats.TabIndex = 78;
            this.gbRangeStats.TabStop = false;
            this.gbRangeStats.Text = "Range Stats";
            // 
            // txtRange
            // 
            this.txtRange.Location = new System.Drawing.Point(62, 19);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(105, 20);
            this.txtRange.TabIndex = 70;
            // 
            // lblReload
            // 
            this.lblReload.AutoSize = true;
            this.lblReload.Location = new System.Drawing.Point(218, 22);
            this.lblReload.Name = "lblReload";
            this.lblReload.Size = new System.Drawing.Size(44, 13);
            this.lblReload.TabIndex = 69;
            this.lblReload.Text = "Reload:";
            // 
            // txtReload
            // 
            this.txtReload.Location = new System.Drawing.Point(268, 19);
            this.txtReload.Name = "txtReload";
            this.txtReload.Size = new System.Drawing.Size(121, 20);
            this.txtReload.TabIndex = 68;
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(6, 22);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(42, 13);
            this.lblRange.TabIndex = 23;
            this.lblRange.Text = "Range:";
            // 
            // gbSpells
            // 
            this.gbSpells.Controls.Add(this.btnDeleteAction);
            this.gbSpells.Controls.Add(this.lsActions);
            this.gbSpells.Controls.Add(this.btnAddAction);
            this.gbSpells.Location = new System.Drawing.Point(413, 27);
            this.gbSpells.Name = "gbSpells";
            this.gbSpells.Size = new System.Drawing.Size(275, 175);
            this.gbSpells.TabIndex = 79;
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
            this.gbPassiveSkills.TabIndex = 80;
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
            // 
            // lsPassiveSkills
            // 
            this.lsPassiveSkills.FormattingEnabled = true;
            this.lsPassiveSkills.Location = new System.Drawing.Point(6, 19);
            this.lsPassiveSkills.Name = "lsPassiveSkills";
            this.lsPassiveSkills.Size = new System.Drawing.Size(263, 69);
            this.lsPassiveSkills.TabIndex = 72;
            // 
            // btnAddSkill
            // 
            this.btnAddSkill.Location = new System.Drawing.Point(6, 94);
            this.btnAddSkill.Name = "btnAddSkill";
            this.btnAddSkill.Size = new System.Drawing.Size(71, 23);
            this.btnAddSkill.TabIndex = 71;
            this.btnAddSkill.Text = "Add Skill";
            this.btnAddSkill.UseVisualStyleBackColor = true;
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
            // WeaponEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 441);
            this.Controls.Add(this.gbSpells);
            this.Controls.Add(this.gbPassiveSkills);
            this.Controls.Add(this.gbRangeStats);
            this.Controls.Add(this.gbBonusStats);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbBaseStats);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "WeaponEditor";
            this.Text = "Weapon Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown9)).EndInit();
            this.gbBaseStats.ResumeLayout(false);
            this.gbBaseStats.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.gbBonusStats.ResumeLayout(false);
            this.gbBonusStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.gbRangeStats.ResumeLayout(false);
            this.gbRangeStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtReload)).EndInit();
            this.gbSpells.ResumeLayout(false);
            this.gbPassiveSkills.ResumeLayout(false);
            this.gbPassiveSkills.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbBaseStats;
        private System.Windows.Forms.Label lblHands;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemoveTrait;
        private System.Windows.Forms.Button btnAddTrait;
        private System.Windows.Forms.ListBox lsTraits;
        private System.Windows.Forms.GroupBox gbBonusStats;
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
        private System.Windows.Forms.NumericUpDown numericUpDown9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDamage;
        private System.Windows.Forms.TextBox txtBulk;
        private System.Windows.Forms.Label lblBulk;
        private System.Windows.Forms.Label lblWeaponType;
        private System.Windows.Forms.ComboBox cbWeaponType;
        private System.Windows.Forms.GroupBox gbRangeStats;
        private System.Windows.Forms.TextBox txtRange;
        private System.Windows.Forms.Label lblReload;
        private System.Windows.Forms.NumericUpDown txtReload;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.ComboBox cbHands;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cbCategory;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.Button btnDeleteAction;
        private System.Windows.Forms.ListBox lsActions;
        private System.Windows.Forms.Button btnAddAction;
        private System.Windows.Forms.GroupBox gbPassiveSkills;
        private System.Windows.Forms.Button btnDeleteSkill;
        private System.Windows.Forms.ListBox lsPassiveSkills;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.Label lblPassiveSkill;
        private System.Windows.Forms.TextBox txtPassiveSkill;
    }
}