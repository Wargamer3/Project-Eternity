namespace ProjectEternity.Editors.UnitNormalEditor
{
    partial class UnitNormalEditor
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
            this.tsmExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPrice = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPartsSlots = new System.Windows.Forms.Label();
            this.txtPartsSlots = new System.Windows.Forms.NumericUpDown();
            this.txtBaseMovement = new System.Windows.Forms.TextBox();
            this.txtBaseMobility = new System.Windows.Forms.TextBox();
            this.txtBaseArmor = new System.Windows.Forms.TextBox();
            this.txtBaseEN = new System.Windows.Forms.TextBox();
            this.txtBaseHP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tabList = new System.Windows.Forms.TabControl();
            this.tabAnimations = new System.Windows.Forms.TabPage();
            this.btnSelectAnimation = new System.Windows.Forms.Button();
            this.lstAnimations = new System.Windows.Forms.ListView();
            this.txtAnimationName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabAbilities = new System.Windows.Forms.TabPage();
            this.lstAbilities = new System.Windows.Forms.ListBox();
            this.btnAddAbility = new System.Windows.Forms.Button();
            this.btnMoveUpAbility = new System.Windows.Forms.Button();
            this.btnRemoveAbility = new System.Windows.Forms.Button();
            this.btnMoveDownAbility = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemovePilot = new System.Windows.Forms.Button();
            this.btnAddPilot = new System.Windows.Forms.Button();
            this.lstPilots = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtEXP = new System.Windows.Forms.NumericUpDown();
            this.lblEXP = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbTerrainAir = new System.Windows.Forms.ComboBox();
            this.cbTerrainLand = new System.Windows.Forms.ComboBox();
            this.cbTerrainSea = new System.Windows.Forms.ComboBox();
            this.cbTerrainSpace = new System.Windows.Forms.ComboBox();
            this.lblTerrainSpace = new System.Windows.Forms.Label();
            this.lblTerrainSea = new System.Windows.Forms.Label();
            this.lblTerrainLand = new System.Windows.Forms.Label();
            this.lblTerrainAir = new System.Windows.Forms.Label();
            this.cboUnderwater = new System.Windows.Forms.GroupBox();
            this.cboMovementUnderwater = new System.Windows.Forms.CheckBox();
            this.cboMovementUnderground = new System.Windows.Forms.CheckBox();
            this.cboMovementSpace = new System.Windows.Forms.CheckBox();
            this.cboMovementSea = new System.Windows.Forms.CheckBox();
            this.cboMovementLand = new System.Windows.Forms.CheckBox();
            this.cboMovementAir = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnEditMapSize = new System.Windows.Forms.Button();
            this.rbSizeSS = new System.Windows.Forms.RadioButton();
            this.rbSizeS = new System.Windows.Forms.RadioButton();
            this.rbSizeM = new System.Windows.Forms.RadioButton();
            this.rbSizeL = new System.Windows.Forms.RadioButton();
            this.rbSizeLL = new System.Windows.Forms.RadioButton();
            this.rbSizeLLL = new System.Windows.Forms.RadioButton();
            this.gbAttacks = new System.Windows.Forms.GroupBox();
            this.btnEditAttack = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsSlots)).BeginInit();
            this.tabList.SuspendLayout();
            this.tabAnimations.SuspendLayout();
            this.tabAbilities.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEXP)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.cboUnderwater.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.gbAttacks.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmExport,
            this.tsmDetails});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(734, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmExport
            // 
            this.tsmExport.Name = "tsmExport";
            this.tsmExport.Size = new System.Drawing.Size(53, 20);
            this.tsmExport.Text = "Export";
            this.tsmExport.Click += new System.EventHandler(this.tsmExport_Click);
            // 
            // tsmDetails
            // 
            this.tsmDetails.Name = "tsmDetails";
            this.tsmDetails.Size = new System.Drawing.Size(54, 20);
            this.tsmDetails.Text = "Details";
            this.tsmDetails.Click += new System.EventHandler(this.tsmDetails_Click);
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(418, 22);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(34, 13);
            this.lblPrice.TabIndex = 24;
            this.lblPrice.Text = "Price:";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(458, 19);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(82, 20);
            this.txtPrice.TabIndex = 25;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 58);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(534, 82);
            this.txtDescription.TabIndex = 27;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 42);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 26;
            this.lblDescription.Text = "Description:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPartsSlots);
            this.groupBox1.Controls.Add(this.txtPartsSlots);
            this.groupBox1.Controls.Add(this.txtBaseMovement);
            this.groupBox1.Controls.Add(this.txtBaseMobility);
            this.groupBox1.Controls.Add(this.txtBaseArmor);
            this.groupBox1.Controls.Add(this.txtBaseEN);
            this.groupBox1.Controls.Add(this.txtBaseHP);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Location = new System.Drawing.Point(12, 181);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 172);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unit boosts";
            // 
            // lblPartsSlots
            // 
            this.lblPartsSlots.AutoSize = true;
            this.lblPartsSlots.Location = new System.Drawing.Point(6, 145);
            this.lblPartsSlots.Name = "lblPartsSlots";
            this.lblPartsSlots.Size = new System.Drawing.Size(60, 13);
            this.lblPartsSlots.TabIndex = 11;
            this.lblPartsSlots.Text = "Parts Slots:";
            // 
            // txtPartsSlots
            // 
            this.txtPartsSlots.Location = new System.Drawing.Point(174, 143);
            this.txtPartsSlots.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.txtPartsSlots.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtPartsSlots.Name = "txtPartsSlots";
            this.txtPartsSlots.Size = new System.Drawing.Size(90, 20);
            this.txtPartsSlots.TabIndex = 10;
            this.txtPartsSlots.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // txtBaseMovement
            // 
            this.txtBaseMovement.Location = new System.Drawing.Point(121, 117);
            this.txtBaseMovement.Name = "txtBaseMovement";
            this.txtBaseMovement.Size = new System.Drawing.Size(143, 20);
            this.txtBaseMovement.TabIndex = 9;
            // 
            // txtBaseMobility
            // 
            this.txtBaseMobility.Location = new System.Drawing.Point(121, 91);
            this.txtBaseMobility.Name = "txtBaseMobility";
            this.txtBaseMobility.Size = new System.Drawing.Size(143, 20);
            this.txtBaseMobility.TabIndex = 8;
            // 
            // txtBaseArmor
            // 
            this.txtBaseArmor.Location = new System.Drawing.Point(121, 65);
            this.txtBaseArmor.Name = "txtBaseArmor";
            this.txtBaseArmor.Size = new System.Drawing.Size(143, 20);
            this.txtBaseArmor.TabIndex = 7;
            // 
            // txtBaseEN
            // 
            this.txtBaseEN.Location = new System.Drawing.Point(121, 39);
            this.txtBaseEN.Name = "txtBaseEN";
            this.txtBaseEN.Size = new System.Drawing.Size(143, 20);
            this.txtBaseEN.TabIndex = 6;
            // 
            // txtBaseHP
            // 
            this.txtBaseHP.Location = new System.Drawing.Point(121, 13);
            this.txtBaseHP.Name = "txtBaseHP";
            this.txtBaseHP.Size = new System.Drawing.Size(143, 20);
            this.txtBaseHP.TabIndex = 5;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 120);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Base Movement:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Base Mobility:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Base Armor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Base EN:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Base HP:";
            // 
            // tabList
            // 
            this.tabList.Controls.Add(this.tabAnimations);
            this.tabList.Controls.Add(this.tabAbilities);
            this.tabList.Location = new System.Drawing.Point(288, 181);
            this.tabList.Name = "tabList";
            this.tabList.SelectedIndex = 0;
            this.tabList.Size = new System.Drawing.Size(272, 298);
            this.tabList.TabIndex = 18;
            // 
            // tabAnimations
            // 
            this.tabAnimations.Controls.Add(this.btnSelectAnimation);
            this.tabAnimations.Controls.Add(this.lstAnimations);
            this.tabAnimations.Controls.Add(this.txtAnimationName);
            this.tabAnimations.Controls.Add(this.label7);
            this.tabAnimations.Location = new System.Drawing.Point(4, 22);
            this.tabAnimations.Name = "tabAnimations";
            this.tabAnimations.Padding = new System.Windows.Forms.Padding(3);
            this.tabAnimations.Size = new System.Drawing.Size(264, 272);
            this.tabAnimations.TabIndex = 0;
            this.tabAnimations.Text = "Animations";
            this.tabAnimations.UseVisualStyleBackColor = true;
            // 
            // btnSelectAnimation
            // 
            this.btnSelectAnimation.Location = new System.Drawing.Point(150, 198);
            this.btnSelectAnimation.Name = "btnSelectAnimation";
            this.btnSelectAnimation.Size = new System.Drawing.Size(108, 23);
            this.btnSelectAnimation.TabIndex = 8;
            this.btnSelectAnimation.Text = "Select Animation";
            this.btnSelectAnimation.UseVisualStyleBackColor = true;
            this.btnSelectAnimation.Click += new System.EventHandler(this.btnSelectAnimation_Click);
            // 
            // lstAnimations
            // 
            this.lstAnimations.HideSelection = false;
            this.lstAnimations.Location = new System.Drawing.Point(3, 3);
            this.lstAnimations.Name = "lstAnimations";
            this.lstAnimations.Size = new System.Drawing.Size(258, 189);
            this.lstAnimations.TabIndex = 5;
            this.lstAnimations.UseCompatibleStateImageBehavior = false;
            this.lstAnimations.View = System.Windows.Forms.View.List;
            this.lstAnimations.SelectedIndexChanged += new System.EventHandler(this.lstAnimations_SelectedIndexChanged);
            // 
            // txtAnimationName
            // 
            this.txtAnimationName.Location = new System.Drawing.Point(6, 227);
            this.txtAnimationName.Name = "txtAnimationName";
            this.txtAnimationName.ReadOnly = true;
            this.txtAnimationName.Size = new System.Drawing.Size(252, 20);
            this.txtAnimationName.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 211);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Animation Path:";
            // 
            // tabAbilities
            // 
            this.tabAbilities.Controls.Add(this.lstAbilities);
            this.tabAbilities.Controls.Add(this.btnAddAbility);
            this.tabAbilities.Controls.Add(this.btnMoveUpAbility);
            this.tabAbilities.Controls.Add(this.btnRemoveAbility);
            this.tabAbilities.Controls.Add(this.btnMoveDownAbility);
            this.tabAbilities.Location = new System.Drawing.Point(4, 22);
            this.tabAbilities.Name = "tabAbilities";
            this.tabAbilities.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbilities.Size = new System.Drawing.Size(264, 272);
            this.tabAbilities.TabIndex = 1;
            this.tabAbilities.Text = "Abilities";
            this.tabAbilities.UseVisualStyleBackColor = true;
            // 
            // lstAbilities
            // 
            this.lstAbilities.FormattingEnabled = true;
            this.lstAbilities.Location = new System.Drawing.Point(3, 3);
            this.lstAbilities.Name = "lstAbilities";
            this.lstAbilities.Size = new System.Drawing.Size(258, 199);
            this.lstAbilities.TabIndex = 18;
            // 
            // btnAddAbility
            // 
            this.btnAddAbility.Location = new System.Drawing.Point(3, 214);
            this.btnAddAbility.Name = "btnAddAbility";
            this.btnAddAbility.Size = new System.Drawing.Size(126, 23);
            this.btnAddAbility.TabIndex = 19;
            this.btnAddAbility.Text = "Add ability";
            this.btnAddAbility.UseVisualStyleBackColor = true;
            this.btnAddAbility.Click += new System.EventHandler(this.btnAddAbility_Click);
            // 
            // btnMoveUpAbility
            // 
            this.btnMoveUpAbility.Location = new System.Drawing.Point(3, 243);
            this.btnMoveUpAbility.Name = "btnMoveUpAbility";
            this.btnMoveUpAbility.Size = new System.Drawing.Size(126, 23);
            this.btnMoveUpAbility.TabIndex = 21;
            this.btnMoveUpAbility.Text = "Move up";
            this.btnMoveUpAbility.UseVisualStyleBackColor = true;
            this.btnMoveUpAbility.Click += new System.EventHandler(this.btnMoveUpAbility_Click);
            // 
            // btnRemoveAbility
            // 
            this.btnRemoveAbility.Location = new System.Drawing.Point(135, 214);
            this.btnRemoveAbility.Name = "btnRemoveAbility";
            this.btnRemoveAbility.Size = new System.Drawing.Size(126, 23);
            this.btnRemoveAbility.TabIndex = 20;
            this.btnRemoveAbility.Text = "Remove ability";
            this.btnRemoveAbility.UseVisualStyleBackColor = true;
            this.btnRemoveAbility.Click += new System.EventHandler(this.btnRemoveAbility_Click);
            // 
            // btnMoveDownAbility
            // 
            this.btnMoveDownAbility.Location = new System.Drawing.Point(135, 243);
            this.btnMoveDownAbility.Name = "btnMoveDownAbility";
            this.btnMoveDownAbility.Size = new System.Drawing.Size(126, 23);
            this.btnMoveDownAbility.TabIndex = 22;
            this.btnMoveDownAbility.Text = "Move down";
            this.btnMoveDownAbility.UseVisualStyleBackColor = true;
            this.btnMoveDownAbility.Click += new System.EventHandler(this.btnMoveDownAbility_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemovePilot);
            this.groupBox2.Controls.Add(this.btnAddPilot);
            this.groupBox2.Controls.Add(this.lstPilots);
            this.groupBox2.Location = new System.Drawing.Point(12, 359);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(271, 120);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pilot whitelist";
            // 
            // btnRemovePilot
            // 
            this.btnRemovePilot.Location = new System.Drawing.Point(5, 71);
            this.btnRemovePilot.Name = "btnRemovePilot";
            this.btnRemovePilot.Size = new System.Drawing.Size(86, 23);
            this.btnRemovePilot.TabIndex = 3;
            this.btnRemovePilot.Text = "Remove pilot";
            this.btnRemovePilot.UseVisualStyleBackColor = true;
            this.btnRemovePilot.Click += new System.EventHandler(this.btnRemovePilot_Click);
            // 
            // btnAddPilot
            // 
            this.btnAddPilot.Location = new System.Drawing.Point(5, 42);
            this.btnAddPilot.Name = "btnAddPilot";
            this.btnAddPilot.Size = new System.Drawing.Size(86, 23);
            this.btnAddPilot.TabIndex = 2;
            this.btnAddPilot.Text = "Add pilot";
            this.btnAddPilot.UseVisualStyleBackColor = true;
            this.btnAddPilot.Click += new System.EventHandler(this.btnAddPilot_Click);
            // 
            // lstPilots
            // 
            this.lstPilots.FormattingEnabled = true;
            this.lstPilots.Location = new System.Drawing.Point(98, 19);
            this.lstPilots.Name = "lstPilots";
            this.lstPilots.Size = new System.Drawing.Size(167, 95);
            this.lstPilots.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtEXP);
            this.groupBox3.Controls.Add(this.lblEXP);
            this.groupBox3.Controls.Add(this.txtName);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.lblPrice);
            this.groupBox3.Controls.Add(this.txtDescription);
            this.groupBox3.Controls.Add(this.lblDescription);
            this.groupBox3.Controls.Add(this.txtPrice);
            this.groupBox3.Location = new System.Drawing.Point(12, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(548, 148);
            this.groupBox3.TabIndex = 28;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Item Information";
            // 
            // txtEXP
            // 
            this.txtEXP.Location = new System.Drawing.Point(340, 19);
            this.txtEXP.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.txtEXP.Name = "txtEXP";
            this.txtEXP.Size = new System.Drawing.Size(73, 20);
            this.txtEXP.TabIndex = 31;
            // 
            // lblEXP
            // 
            this.lblEXP.AutoSize = true;
            this.lblEXP.Location = new System.Drawing.Point(303, 21);
            this.lblEXP.Name = "lblEXP";
            this.lblEXP.Size = new System.Drawing.Size(31, 13);
            this.lblEXP.TabIndex = 30;
            this.lblEXP.Text = "EXP:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(247, 20);
            this.txtName.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Name:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbTerrainAir);
            this.groupBox4.Controls.Add(this.cbTerrainLand);
            this.groupBox4.Controls.Add(this.cbTerrainSea);
            this.groupBox4.Controls.Add(this.cbTerrainSpace);
            this.groupBox4.Controls.Add(this.lblTerrainSpace);
            this.groupBox4.Controls.Add(this.lblTerrainSea);
            this.groupBox4.Controls.Add(this.lblTerrainLand);
            this.groupBox4.Controls.Add(this.lblTerrainAir);
            this.groupBox4.Location = new System.Drawing.Point(566, 27);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(156, 125);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Terrain";
            // 
            // cbTerrainAir
            // 
            this.cbTerrainAir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTerrainAir.FormattingEnabled = true;
            this.cbTerrainAir.Items.AddRange(new object[] {
            "-",
            "S",
            "A",
            "B",
            "C",
            "D"});
            this.cbTerrainAir.Location = new System.Drawing.Point(53, 19);
            this.cbTerrainAir.Name = "cbTerrainAir";
            this.cbTerrainAir.Size = new System.Drawing.Size(97, 21);
            this.cbTerrainAir.TabIndex = 7;
            // 
            // cbTerrainLand
            // 
            this.cbTerrainLand.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTerrainLand.FormattingEnabled = true;
            this.cbTerrainLand.Items.AddRange(new object[] {
            "-",
            "S",
            "A",
            "B",
            "C",
            "D"});
            this.cbTerrainLand.Location = new System.Drawing.Point(53, 45);
            this.cbTerrainLand.Name = "cbTerrainLand";
            this.cbTerrainLand.Size = new System.Drawing.Size(97, 21);
            this.cbTerrainLand.TabIndex = 6;
            // 
            // cbTerrainSea
            // 
            this.cbTerrainSea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTerrainSea.FormattingEnabled = true;
            this.cbTerrainSea.Items.AddRange(new object[] {
            "-",
            "S",
            "A",
            "B",
            "C",
            "D"});
            this.cbTerrainSea.Location = new System.Drawing.Point(53, 71);
            this.cbTerrainSea.Name = "cbTerrainSea";
            this.cbTerrainSea.Size = new System.Drawing.Size(97, 21);
            this.cbTerrainSea.TabIndex = 5;
            // 
            // cbTerrainSpace
            // 
            this.cbTerrainSpace.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTerrainSpace.FormattingEnabled = true;
            this.cbTerrainSpace.Items.AddRange(new object[] {
            "-",
            "S",
            "A",
            "B",
            "C",
            "D"});
            this.cbTerrainSpace.Location = new System.Drawing.Point(53, 98);
            this.cbTerrainSpace.Name = "cbTerrainSpace";
            this.cbTerrainSpace.Size = new System.Drawing.Size(97, 21);
            this.cbTerrainSpace.TabIndex = 4;
            // 
            // lblTerrainSpace
            // 
            this.lblTerrainSpace.AutoSize = true;
            this.lblTerrainSpace.Location = new System.Drawing.Point(6, 100);
            this.lblTerrainSpace.Name = "lblTerrainSpace";
            this.lblTerrainSpace.Size = new System.Drawing.Size(41, 13);
            this.lblTerrainSpace.TabIndex = 3;
            this.lblTerrainSpace.Text = "Space:";
            // 
            // lblTerrainSea
            // 
            this.lblTerrainSea.AutoSize = true;
            this.lblTerrainSea.Location = new System.Drawing.Point(6, 74);
            this.lblTerrainSea.Name = "lblTerrainSea";
            this.lblTerrainSea.Size = new System.Drawing.Size(29, 13);
            this.lblTerrainSea.TabIndex = 2;
            this.lblTerrainSea.Text = "Sea:";
            // 
            // lblTerrainLand
            // 
            this.lblTerrainLand.AutoSize = true;
            this.lblTerrainLand.Location = new System.Drawing.Point(6, 48);
            this.lblTerrainLand.Name = "lblTerrainLand";
            this.lblTerrainLand.Size = new System.Drawing.Size(34, 13);
            this.lblTerrainLand.TabIndex = 1;
            this.lblTerrainLand.Text = "Land:";
            // 
            // lblTerrainAir
            // 
            this.lblTerrainAir.AutoSize = true;
            this.lblTerrainAir.Location = new System.Drawing.Point(6, 22);
            this.lblTerrainAir.Name = "lblTerrainAir";
            this.lblTerrainAir.Size = new System.Drawing.Size(22, 13);
            this.lblTerrainAir.TabIndex = 0;
            this.lblTerrainAir.Text = "Air:";
            // 
            // cboUnderwater
            // 
            this.cboUnderwater.Controls.Add(this.cboMovementUnderwater);
            this.cboUnderwater.Controls.Add(this.cboMovementUnderground);
            this.cboUnderwater.Controls.Add(this.cboMovementSpace);
            this.cboUnderwater.Controls.Add(this.cboMovementSea);
            this.cboUnderwater.Controls.Add(this.cboMovementLand);
            this.cboUnderwater.Controls.Add(this.cboMovementAir);
            this.cboUnderwater.Location = new System.Drawing.Point(566, 158);
            this.cboUnderwater.Name = "cboUnderwater";
            this.cboUnderwater.Size = new System.Drawing.Size(156, 167);
            this.cboUnderwater.TabIndex = 30;
            this.cboUnderwater.TabStop = false;
            this.cboUnderwater.Text = "Movement types";
            // 
            // cboMovementUnderwater
            // 
            this.cboMovementUnderwater.AutoSize = true;
            this.cboMovementUnderwater.Location = new System.Drawing.Point(6, 134);
            this.cboMovementUnderwater.Name = "cboMovementUnderwater";
            this.cboMovementUnderwater.Size = new System.Drawing.Size(81, 17);
            this.cboMovementUnderwater.TabIndex = 5;
            this.cboMovementUnderwater.Text = "Underwater";
            this.cboMovementUnderwater.UseVisualStyleBackColor = true;
            // 
            // cboMovementUnderground
            // 
            this.cboMovementUnderground.AutoSize = true;
            this.cboMovementUnderground.Location = new System.Drawing.Point(6, 111);
            this.cboMovementUnderground.Name = "cboMovementUnderground";
            this.cboMovementUnderground.Size = new System.Drawing.Size(88, 17);
            this.cboMovementUnderground.TabIndex = 4;
            this.cboMovementUnderground.Text = "Underground";
            this.cboMovementUnderground.UseVisualStyleBackColor = true;
            // 
            // cboMovementSpace
            // 
            this.cboMovementSpace.AutoSize = true;
            this.cboMovementSpace.Location = new System.Drawing.Point(6, 88);
            this.cboMovementSpace.Name = "cboMovementSpace";
            this.cboMovementSpace.Size = new System.Drawing.Size(57, 17);
            this.cboMovementSpace.TabIndex = 3;
            this.cboMovementSpace.Text = "Space";
            this.cboMovementSpace.UseVisualStyleBackColor = true;
            // 
            // cboMovementSea
            // 
            this.cboMovementSea.AutoSize = true;
            this.cboMovementSea.Location = new System.Drawing.Point(6, 65);
            this.cboMovementSea.Name = "cboMovementSea";
            this.cboMovementSea.Size = new System.Drawing.Size(45, 17);
            this.cboMovementSea.TabIndex = 2;
            this.cboMovementSea.Text = "Sea";
            this.cboMovementSea.UseVisualStyleBackColor = true;
            // 
            // cboMovementLand
            // 
            this.cboMovementLand.AutoSize = true;
            this.cboMovementLand.Location = new System.Drawing.Point(6, 42);
            this.cboMovementLand.Name = "cboMovementLand";
            this.cboMovementLand.Size = new System.Drawing.Size(50, 17);
            this.cboMovementLand.TabIndex = 1;
            this.cboMovementLand.Text = "Land";
            this.cboMovementLand.UseVisualStyleBackColor = true;
            // 
            // cboMovementAir
            // 
            this.cboMovementAir.AutoSize = true;
            this.cboMovementAir.Location = new System.Drawing.Point(6, 19);
            this.cboMovementAir.Name = "cboMovementAir";
            this.cboMovementAir.Size = new System.Drawing.Size(38, 17);
            this.cboMovementAir.TabIndex = 0;
            this.cboMovementAir.Text = "Air";
            this.cboMovementAir.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnEditMapSize);
            this.groupBox6.Controls.Add(this.rbSizeSS);
            this.groupBox6.Controls.Add(this.rbSizeS);
            this.groupBox6.Controls.Add(this.rbSizeM);
            this.groupBox6.Controls.Add(this.rbSizeL);
            this.groupBox6.Controls.Add(this.rbSizeLL);
            this.groupBox6.Controls.Add(this.rbSizeLLL);
            this.groupBox6.Location = new System.Drawing.Point(566, 331);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(156, 94);
            this.groupBox6.TabIndex = 31;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Size";
            // 
            // btnEditMapSize
            // 
            this.btnEditMapSize.Location = new System.Drawing.Point(6, 65);
            this.btnEditMapSize.Name = "btnEditMapSize";
            this.btnEditMapSize.Size = new System.Drawing.Size(144, 23);
            this.btnEditMapSize.TabIndex = 6;
            this.btnEditMapSize.Text = "Edit Map Size";
            this.btnEditMapSize.UseVisualStyleBackColor = true;
            this.btnEditMapSize.Click += new System.EventHandler(this.btnEditMapSize_Click);
            // 
            // rbSizeSS
            // 
            this.rbSizeSS.AutoSize = true;
            this.rbSizeSS.Location = new System.Drawing.Point(98, 42);
            this.rbSizeSS.Name = "rbSizeSS";
            this.rbSizeSS.Size = new System.Drawing.Size(39, 17);
            this.rbSizeSS.TabIndex = 5;
            this.rbSizeSS.TabStop = true;
            this.rbSizeSS.Text = "SS";
            this.rbSizeSS.UseVisualStyleBackColor = true;
            // 
            // rbSizeS
            // 
            this.rbSizeS.AutoSize = true;
            this.rbSizeS.Location = new System.Drawing.Point(55, 42);
            this.rbSizeS.Name = "rbSizeS";
            this.rbSizeS.Size = new System.Drawing.Size(32, 17);
            this.rbSizeS.TabIndex = 4;
            this.rbSizeS.TabStop = true;
            this.rbSizeS.Text = "S";
            this.rbSizeS.UseVisualStyleBackColor = true;
            // 
            // rbSizeM
            // 
            this.rbSizeM.AutoSize = true;
            this.rbSizeM.Location = new System.Drawing.Point(6, 42);
            this.rbSizeM.Name = "rbSizeM";
            this.rbSizeM.Size = new System.Drawing.Size(34, 17);
            this.rbSizeM.TabIndex = 3;
            this.rbSizeM.TabStop = true;
            this.rbSizeM.Text = "M";
            this.rbSizeM.UseVisualStyleBackColor = true;
            // 
            // rbSizeL
            // 
            this.rbSizeL.AutoSize = true;
            this.rbSizeL.Location = new System.Drawing.Point(98, 19);
            this.rbSizeL.Name = "rbSizeL";
            this.rbSizeL.Size = new System.Drawing.Size(31, 17);
            this.rbSizeL.TabIndex = 2;
            this.rbSizeL.TabStop = true;
            this.rbSizeL.Text = "L";
            this.rbSizeL.UseVisualStyleBackColor = true;
            // 
            // rbSizeLL
            // 
            this.rbSizeLL.AutoSize = true;
            this.rbSizeLL.Location = new System.Drawing.Point(55, 19);
            this.rbSizeLL.Name = "rbSizeLL";
            this.rbSizeLL.Size = new System.Drawing.Size(37, 17);
            this.rbSizeLL.TabIndex = 1;
            this.rbSizeLL.Text = "LL";
            this.rbSizeLL.UseVisualStyleBackColor = true;
            // 
            // rbSizeLLL
            // 
            this.rbSizeLLL.AutoSize = true;
            this.rbSizeLLL.Checked = true;
            this.rbSizeLLL.Location = new System.Drawing.Point(6, 19);
            this.rbSizeLLL.Name = "rbSizeLLL";
            this.rbSizeLLL.Size = new System.Drawing.Size(43, 17);
            this.rbSizeLLL.TabIndex = 0;
            this.rbSizeLLL.TabStop = true;
            this.rbSizeLLL.Text = "LLL";
            this.rbSizeLLL.UseVisualStyleBackColor = true;
            // 
            // gbAttacks
            // 
            this.gbAttacks.Controls.Add(this.btnEditAttack);
            this.gbAttacks.Location = new System.Drawing.Point(566, 431);
            this.gbAttacks.Name = "gbAttacks";
            this.gbAttacks.Size = new System.Drawing.Size(156, 48);
            this.gbAttacks.TabIndex = 32;
            this.gbAttacks.TabStop = false;
            this.gbAttacks.Text = "Attacks";
            // 
            // btnEditAttack
            // 
            this.btnEditAttack.Location = new System.Drawing.Point(6, 19);
            this.btnEditAttack.Name = "btnEditAttack";
            this.btnEditAttack.Size = new System.Drawing.Size(144, 23);
            this.btnEditAttack.TabIndex = 0;
            this.btnEditAttack.Text = "Edit Attacks";
            this.btnEditAttack.UseVisualStyleBackColor = true;
            this.btnEditAttack.Click += new System.EventHandler(this.btnEditAttacks_Click);
            // 
            // UnitNormalEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 491);
            this.Controls.Add(this.gbAttacks);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.cboUnderwater);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.tabList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitNormalEditor";
            this.Text = "Unit Normal Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartsSlots)).EndInit();
            this.tabList.ResumeLayout(false);
            this.tabAnimations.ResumeLayout(false);
            this.tabAnimations.PerformLayout();
            this.tabAbilities.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEXP)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.cboUnderwater.ResumeLayout(false);
            this.cboUnderwater.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.gbAttacks.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBaseMovement;
        private System.Windows.Forms.TextBox txtBaseMobility;
        private System.Windows.Forms.TextBox txtBaseArmor;
        private System.Windows.Forms.TextBox txtBaseEN;
        private System.Windows.Forms.TextBox txtBaseHP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl tabList;
        private System.Windows.Forms.TabPage tabAnimations;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemovePilot;
        private System.Windows.Forms.Button btnAddPilot;
        private System.Windows.Forms.ListBox lstPilots;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lblTerrainAir;
        private System.Windows.Forms.ComboBox cbTerrainAir;
        private System.Windows.Forms.ComboBox cbTerrainLand;
        private System.Windows.Forms.ComboBox cbTerrainSea;
        private System.Windows.Forms.ComboBox cbTerrainSpace;
        private System.Windows.Forms.Label lblTerrainSpace;
        private System.Windows.Forms.Label lblTerrainSea;
        private System.Windows.Forms.Label lblTerrainLand;
        private System.Windows.Forms.GroupBox cboUnderwater;
        private System.Windows.Forms.CheckBox cboMovementAir;
        private System.Windows.Forms.CheckBox cboMovementLand;
        private System.Windows.Forms.CheckBox cboMovementSpace;
        private System.Windows.Forms.CheckBox cboMovementSea;
        private System.Windows.Forms.CheckBox cboMovementUnderground;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rbSizeLLL;
        private System.Windows.Forms.RadioButton rbSizeSS;
        private System.Windows.Forms.RadioButton rbSizeS;
        private System.Windows.Forms.RadioButton rbSizeM;
        private System.Windows.Forms.RadioButton rbSizeL;
        private System.Windows.Forms.RadioButton rbSizeLL;
        private System.Windows.Forms.TabPage tabAbilities;
        private System.Windows.Forms.ListBox lstAbilities;
        private System.Windows.Forms.Button btnAddAbility;
        private System.Windows.Forms.Button btnMoveUpAbility;
        private System.Windows.Forms.Button btnRemoveAbility;
        private System.Windows.Forms.Button btnMoveDownAbility;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbAttacks;
        private System.Windows.Forms.Button btnEditAttack;
        private System.Windows.Forms.TextBox txtAnimationName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListView lstAnimations;
        private System.Windows.Forms.Button btnSelectAnimation;
        private System.Windows.Forms.Label lblPartsSlots;
        private System.Windows.Forms.NumericUpDown txtPartsSlots;
        private System.Windows.Forms.Button btnEditMapSize;
        private System.Windows.Forms.NumericUpDown txtEXP;
        private System.Windows.Forms.Label lblEXP;
        private System.Windows.Forms.ToolStripMenuItem tsmExport;
        private System.Windows.Forms.ToolStripMenuItem tsmDetails;
        private System.Windows.Forms.CheckBox cboMovementUnderwater;
    }
}