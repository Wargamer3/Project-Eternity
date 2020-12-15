namespace ProjectEternity.Editors.CardEditor
{
    partial class CreatureCardEditor
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
            this.gbCardInformation = new System.Windows.Forms.GroupBox();
            this.cboRarity = new System.Windows.Forms.ComboBox();
            this.lblRarity = new System.Windows.Forms.Label();
            this.txtMagicCost = new System.Windows.Forms.NumericUpDown();
            this.lblMagicCost = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.gbCreatureInformation = new System.Windows.Forms.GroupBox();
            this.txtMaxST = new System.Windows.Forms.NumericUpDown();
            this.lblMaxST = new System.Windows.Forms.Label();
            this.txtMaxHP = new System.Windows.Forms.NumericUpDown();
            this.lblMaxHP = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAir = new System.Windows.Forms.NumericUpDown();
            this.lblAir = new System.Windows.Forms.Label();
            this.txtEarth = new System.Windows.Forms.NumericUpDown();
            this.lblEarth = new System.Windows.Forms.Label();
            this.txtWater = new System.Windows.Forms.NumericUpDown();
            this.lblWater = new System.Windows.Forms.Label();
            this.txtFire = new System.Windows.Forms.NumericUpDown();
            this.lblFire = new System.Windows.Forms.Label();
            this.txtNeutral = new System.Windows.Forms.NumericUpDown();
            this.lblNeutral = new System.Windows.Forms.Label();
            this.gbAffinities = new System.Windows.Forms.GroupBox();
            this.cbAffinityAir = new System.Windows.Forms.CheckBox();
            this.cbAffinityFire = new System.Windows.Forms.CheckBox();
            this.cbAffinityWater = new System.Windows.Forms.CheckBox();
            this.cbAffinityEarth = new System.Windows.Forms.CheckBox();
            this.cbAffinityNeutral = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbLandLimitAir = new System.Windows.Forms.CheckBox();
            this.cbLandLimitFire = new System.Windows.Forms.CheckBox();
            this.cbLandLimitWater = new System.Windows.Forms.CheckBox();
            this.cbLandLimitEarth = new System.Windows.Forms.CheckBox();
            this.cbLandLimitNeutral = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbItemLimitArmor = new System.Windows.Forms.CheckBox();
            this.cbItemLimitTools = new System.Windows.Forms.CheckBox();
            this.cbItemLimitScrolls = new System.Windows.Forms.CheckBox();
            this.cbItemLimitWeapon = new System.Windows.Forms.CheckBox();
            this.gbSkills = new System.Windows.Forms.GroupBox();
            this.btnRemoveSkill = new System.Windows.Forms.Button();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lstSkill = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.gbCardInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).BeginInit();
            this.gbCreatureInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxST)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxHP)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAir)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEarth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWater)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNeutral)).BeginInit();
            this.gbAffinities.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gbSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(650, 24);
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
            // gbCardInformation
            // 
            this.gbCardInformation.Controls.Add(this.cboRarity);
            this.gbCardInformation.Controls.Add(this.lblRarity);
            this.gbCardInformation.Controls.Add(this.txtMagicCost);
            this.gbCardInformation.Controls.Add(this.lblMagicCost);
            this.gbCardInformation.Controls.Add(this.lblDescription);
            this.gbCardInformation.Controls.Add(this.txtDescription);
            this.gbCardInformation.Controls.Add(this.txtName);
            this.gbCardInformation.Controls.Add(this.lblName);
            this.gbCardInformation.Location = new System.Drawing.Point(12, 27);
            this.gbCardInformation.Name = "gbCardInformation";
            this.gbCardInformation.Size = new System.Drawing.Size(204, 306);
            this.gbCardInformation.TabIndex = 28;
            this.gbCardInformation.TabStop = false;
            this.gbCardInformation.Text = "Card Information";
            // 
            // cboRarity
            // 
            this.cboRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRarity.FormattingEnabled = true;
            this.cboRarity.Items.AddRange(new object[] {
            "Normal",
            "Strange",
            "Rare",
            "Extra"});
            this.cboRarity.Location = new System.Drawing.Point(75, 73);
            this.cboRarity.Name = "cboRarity";
            this.cboRarity.Size = new System.Drawing.Size(123, 21);
            this.cboRarity.TabIndex = 35;
            // 
            // lblRarity
            // 
            this.lblRarity.AutoSize = true;
            this.lblRarity.Location = new System.Drawing.Point(6, 76);
            this.lblRarity.Name = "lblRarity";
            this.lblRarity.Size = new System.Drawing.Size(37, 13);
            this.lblRarity.TabIndex = 34;
            this.lblRarity.Text = "Rarity:";
            // 
            // txtMagicCost
            // 
            this.txtMagicCost.Location = new System.Drawing.Point(75, 45);
            this.txtMagicCost.Name = "txtMagicCost";
            this.txtMagicCost.Size = new System.Drawing.Size(123, 20);
            this.txtMagicCost.TabIndex = 33;
            // 
            // lblMagicCost
            // 
            this.lblMagicCost.AutoSize = true;
            this.lblMagicCost.Location = new System.Drawing.Point(6, 47);
            this.lblMagicCost.Name = "lblMagicCost";
            this.lblMagicCost.Size = new System.Drawing.Size(63, 13);
            this.lblMagicCost.TabIndex = 32;
            this.lblMagicCost.Text = "Magic Cost:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 97);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 31;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDescription.Location = new System.Drawing.Point(6, 113);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(192, 185);
            this.txtDescription.TabIndex = 30;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(148, 20);
            this.txtName.TabIndex = 29;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 28;
            this.lblName.Text = "Name:";
            // 
            // gbCreatureInformation
            // 
            this.gbCreatureInformation.Controls.Add(this.txtMaxST);
            this.gbCreatureInformation.Controls.Add(this.lblMaxST);
            this.gbCreatureInformation.Controls.Add(this.txtMaxHP);
            this.gbCreatureInformation.Controls.Add(this.lblMaxHP);
            this.gbCreatureInformation.Location = new System.Drawing.Point(222, 27);
            this.gbCreatureInformation.Name = "gbCreatureInformation";
            this.gbCreatureInformation.Size = new System.Drawing.Size(142, 73);
            this.gbCreatureInformation.TabIndex = 29;
            this.gbCreatureInformation.TabStop = false;
            this.gbCreatureInformation.Text = "Creature Information";
            // 
            // txtMaxST
            // 
            this.txtMaxST.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxST.Location = new System.Drawing.Point(63, 45);
            this.txtMaxST.Name = "txtMaxST";
            this.txtMaxST.Size = new System.Drawing.Size(73, 20);
            this.txtMaxST.TabIndex = 39;
            // 
            // lblMaxST
            // 
            this.lblMaxST.AutoSize = true;
            this.lblMaxST.Location = new System.Drawing.Point(6, 48);
            this.lblMaxST.Name = "lblMaxST";
            this.lblMaxST.Size = new System.Drawing.Size(47, 13);
            this.lblMaxST.TabIndex = 38;
            this.lblMaxST.Text = "Max ST:";
            // 
            // txtMaxHP
            // 
            this.txtMaxHP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxHP.Location = new System.Drawing.Point(63, 19);
            this.txtMaxHP.Name = "txtMaxHP";
            this.txtMaxHP.Size = new System.Drawing.Size(73, 20);
            this.txtMaxHP.TabIndex = 37;
            // 
            // lblMaxHP
            // 
            this.lblMaxHP.AutoSize = true;
            this.lblMaxHP.Location = new System.Drawing.Point(6, 22);
            this.lblMaxHP.Name = "lblMaxHP";
            this.lblMaxHP.Size = new System.Drawing.Size(48, 13);
            this.lblMaxHP.TabIndex = 36;
            this.lblMaxHP.Text = "Max HP:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAir);
            this.groupBox1.Controls.Add(this.lblAir);
            this.groupBox1.Controls.Add(this.txtEarth);
            this.groupBox1.Controls.Add(this.lblEarth);
            this.groupBox1.Controls.Add(this.txtWater);
            this.groupBox1.Controls.Add(this.lblWater);
            this.groupBox1.Controls.Add(this.txtFire);
            this.groupBox1.Controls.Add(this.lblFire);
            this.groupBox1.Controls.Add(this.txtNeutral);
            this.groupBox1.Controls.Add(this.lblNeutral);
            this.groupBox1.Location = new System.Drawing.Point(502, 171);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(143, 162);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Terrain Requirements";
            // 
            // txtAir
            // 
            this.txtAir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAir.Location = new System.Drawing.Point(64, 124);
            this.txtAir.Name = "txtAir";
            this.txtAir.Size = new System.Drawing.Size(73, 20);
            this.txtAir.TabIndex = 49;
            // 
            // lblAir
            // 
            this.lblAir.AutoSize = true;
            this.lblAir.Location = new System.Drawing.Point(6, 126);
            this.lblAir.Name = "lblAir";
            this.lblAir.Size = new System.Drawing.Size(22, 13);
            this.lblAir.TabIndex = 48;
            this.lblAir.Text = "Air:";
            // 
            // txtEarth
            // 
            this.txtEarth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEarth.Location = new System.Drawing.Point(64, 97);
            this.txtEarth.Name = "txtEarth";
            this.txtEarth.Size = new System.Drawing.Size(73, 20);
            this.txtEarth.TabIndex = 47;
            // 
            // lblEarth
            // 
            this.lblEarth.AutoSize = true;
            this.lblEarth.Location = new System.Drawing.Point(6, 99);
            this.lblEarth.Name = "lblEarth";
            this.lblEarth.Size = new System.Drawing.Size(35, 13);
            this.lblEarth.TabIndex = 46;
            this.lblEarth.Text = "Earth:";
            // 
            // txtWater
            // 
            this.txtWater.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWater.Location = new System.Drawing.Point(64, 71);
            this.txtWater.Name = "txtWater";
            this.txtWater.Size = new System.Drawing.Size(73, 20);
            this.txtWater.TabIndex = 45;
            // 
            // lblWater
            // 
            this.lblWater.AutoSize = true;
            this.lblWater.Location = new System.Drawing.Point(6, 73);
            this.lblWater.Name = "lblWater";
            this.lblWater.Size = new System.Drawing.Size(39, 13);
            this.lblWater.TabIndex = 44;
            this.lblWater.Text = "Water:";
            // 
            // txtFire
            // 
            this.txtFire.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFire.Location = new System.Drawing.Point(64, 45);
            this.txtFire.Name = "txtFire";
            this.txtFire.Size = new System.Drawing.Size(73, 20);
            this.txtFire.TabIndex = 43;
            // 
            // lblFire
            // 
            this.lblFire.AutoSize = true;
            this.lblFire.Location = new System.Drawing.Point(6, 47);
            this.lblFire.Name = "lblFire";
            this.lblFire.Size = new System.Drawing.Size(27, 13);
            this.lblFire.TabIndex = 42;
            this.lblFire.Text = "Fire:";
            // 
            // txtNeutral
            // 
            this.txtNeutral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNeutral.Location = new System.Drawing.Point(64, 19);
            this.txtNeutral.Name = "txtNeutral";
            this.txtNeutral.Size = new System.Drawing.Size(73, 20);
            this.txtNeutral.TabIndex = 41;
            // 
            // lblNeutral
            // 
            this.lblNeutral.AutoSize = true;
            this.lblNeutral.Location = new System.Drawing.Point(6, 21);
            this.lblNeutral.Name = "lblNeutral";
            this.lblNeutral.Size = new System.Drawing.Size(44, 13);
            this.lblNeutral.TabIndex = 40;
            this.lblNeutral.Text = "Neutral:";
            // 
            // gbAffinities
            // 
            this.gbAffinities.Controls.Add(this.cbAffinityAir);
            this.gbAffinities.Controls.Add(this.cbAffinityFire);
            this.gbAffinities.Controls.Add(this.cbAffinityWater);
            this.gbAffinities.Controls.Add(this.cbAffinityEarth);
            this.gbAffinities.Controls.Add(this.cbAffinityNeutral);
            this.gbAffinities.Location = new System.Drawing.Point(370, 27);
            this.gbAffinities.Name = "gbAffinities";
            this.gbAffinities.Size = new System.Drawing.Size(126, 138);
            this.gbAffinities.TabIndex = 34;
            this.gbAffinities.TabStop = false;
            this.gbAffinities.Text = "Affinities";
            // 
            // cbAffinityAir
            // 
            this.cbAffinityAir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAffinityAir.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAffinityAir.Location = new System.Drawing.Point(9, 111);
            this.cbAffinityAir.Name = "cbAffinityAir";
            this.cbAffinityAir.Size = new System.Drawing.Size(111, 17);
            this.cbAffinityAir.TabIndex = 53;
            this.cbAffinityAir.Text = "Air:";
            this.cbAffinityAir.UseVisualStyleBackColor = true;
            // 
            // cbAffinityFire
            // 
            this.cbAffinityFire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAffinityFire.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAffinityFire.Location = new System.Drawing.Point(9, 42);
            this.cbAffinityFire.Name = "cbAffinityFire";
            this.cbAffinityFire.Size = new System.Drawing.Size(111, 17);
            this.cbAffinityFire.TabIndex = 52;
            this.cbAffinityFire.Text = "Fire:";
            this.cbAffinityFire.UseVisualStyleBackColor = true;
            // 
            // cbAffinityWater
            // 
            this.cbAffinityWater.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAffinityWater.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAffinityWater.Location = new System.Drawing.Point(9, 65);
            this.cbAffinityWater.Name = "cbAffinityWater";
            this.cbAffinityWater.Size = new System.Drawing.Size(111, 17);
            this.cbAffinityWater.TabIndex = 51;
            this.cbAffinityWater.Text = "Water:";
            this.cbAffinityWater.UseVisualStyleBackColor = true;
            // 
            // cbAffinityEarth
            // 
            this.cbAffinityEarth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAffinityEarth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAffinityEarth.Location = new System.Drawing.Point(9, 88);
            this.cbAffinityEarth.Name = "cbAffinityEarth";
            this.cbAffinityEarth.Size = new System.Drawing.Size(111, 17);
            this.cbAffinityEarth.TabIndex = 50;
            this.cbAffinityEarth.Text = "Earth:";
            this.cbAffinityEarth.UseVisualStyleBackColor = true;
            // 
            // cbAffinityNeutral
            // 
            this.cbAffinityNeutral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAffinityNeutral.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbAffinityNeutral.Location = new System.Drawing.Point(9, 19);
            this.cbAffinityNeutral.Name = "cbAffinityNeutral";
            this.cbAffinityNeutral.Size = new System.Drawing.Size(111, 17);
            this.cbAffinityNeutral.TabIndex = 49;
            this.cbAffinityNeutral.Text = "Neutral:";
            this.cbAffinityNeutral.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbLandLimitAir);
            this.groupBox3.Controls.Add(this.cbLandLimitFire);
            this.groupBox3.Controls.Add(this.cbLandLimitWater);
            this.groupBox3.Controls.Add(this.cbLandLimitEarth);
            this.groupBox3.Controls.Add(this.cbLandLimitNeutral);
            this.groupBox3.Location = new System.Drawing.Point(370, 171);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(126, 162);
            this.groupBox3.TabIndex = 54;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Land Limits";
            // 
            // cbLandLimitAir
            // 
            this.cbLandLimitAir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLandLimitAir.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLandLimitAir.Location = new System.Drawing.Point(9, 111);
            this.cbLandLimitAir.Name = "cbLandLimitAir";
            this.cbLandLimitAir.Size = new System.Drawing.Size(111, 17);
            this.cbLandLimitAir.TabIndex = 53;
            this.cbLandLimitAir.Text = "Air:";
            this.cbLandLimitAir.UseVisualStyleBackColor = true;
            // 
            // cbLandLimitFire
            // 
            this.cbLandLimitFire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLandLimitFire.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLandLimitFire.Location = new System.Drawing.Point(9, 42);
            this.cbLandLimitFire.Name = "cbLandLimitFire";
            this.cbLandLimitFire.Size = new System.Drawing.Size(111, 17);
            this.cbLandLimitFire.TabIndex = 52;
            this.cbLandLimitFire.Text = "Fire:";
            this.cbLandLimitFire.UseVisualStyleBackColor = true;
            // 
            // cbLandLimitWater
            // 
            this.cbLandLimitWater.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLandLimitWater.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLandLimitWater.Location = new System.Drawing.Point(9, 65);
            this.cbLandLimitWater.Name = "cbLandLimitWater";
            this.cbLandLimitWater.Size = new System.Drawing.Size(111, 17);
            this.cbLandLimitWater.TabIndex = 51;
            this.cbLandLimitWater.Text = "Water:";
            this.cbLandLimitWater.UseVisualStyleBackColor = true;
            // 
            // cbLandLimitEarth
            // 
            this.cbLandLimitEarth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLandLimitEarth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLandLimitEarth.Location = new System.Drawing.Point(9, 88);
            this.cbLandLimitEarth.Name = "cbLandLimitEarth";
            this.cbLandLimitEarth.Size = new System.Drawing.Size(111, 17);
            this.cbLandLimitEarth.TabIndex = 50;
            this.cbLandLimitEarth.Text = "Earth:";
            this.cbLandLimitEarth.UseVisualStyleBackColor = true;
            // 
            // cbLandLimitNeutral
            // 
            this.cbLandLimitNeutral.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLandLimitNeutral.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbLandLimitNeutral.Location = new System.Drawing.Point(9, 19);
            this.cbLandLimitNeutral.Name = "cbLandLimitNeutral";
            this.cbLandLimitNeutral.Size = new System.Drawing.Size(111, 17);
            this.cbLandLimitNeutral.TabIndex = 49;
            this.cbLandLimitNeutral.Text = "Neutral:";
            this.cbLandLimitNeutral.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbItemLimitArmor);
            this.groupBox4.Controls.Add(this.cbItemLimitTools);
            this.groupBox4.Controls.Add(this.cbItemLimitScrolls);
            this.groupBox4.Controls.Add(this.cbItemLimitWeapon);
            this.groupBox4.Location = new System.Drawing.Point(502, 27);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(143, 138);
            this.groupBox4.TabIndex = 55;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Item Limits";
            // 
            // cbItemLimitArmor
            // 
            this.cbItemLimitArmor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbItemLimitArmor.Location = new System.Drawing.Point(9, 42);
            this.cbItemLimitArmor.Name = "cbItemLimitArmor";
            this.cbItemLimitArmor.Size = new System.Drawing.Size(128, 17);
            this.cbItemLimitArmor.TabIndex = 52;
            this.cbItemLimitArmor.Text = "Armor:";
            this.cbItemLimitArmor.UseVisualStyleBackColor = true;
            // 
            // cbItemLimitTools
            // 
            this.cbItemLimitTools.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbItemLimitTools.Location = new System.Drawing.Point(9, 65);
            this.cbItemLimitTools.Name = "cbItemLimitTools";
            this.cbItemLimitTools.Size = new System.Drawing.Size(128, 17);
            this.cbItemLimitTools.TabIndex = 51;
            this.cbItemLimitTools.Text = "Tools:";
            this.cbItemLimitTools.UseVisualStyleBackColor = true;
            // 
            // cbItemLimitScrolls
            // 
            this.cbItemLimitScrolls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbItemLimitScrolls.Location = new System.Drawing.Point(9, 88);
            this.cbItemLimitScrolls.Name = "cbItemLimitScrolls";
            this.cbItemLimitScrolls.Size = new System.Drawing.Size(128, 17);
            this.cbItemLimitScrolls.TabIndex = 50;
            this.cbItemLimitScrolls.Text = "Scrolls:";
            this.cbItemLimitScrolls.UseVisualStyleBackColor = true;
            // 
            // cbItemLimitWeapon
            // 
            this.cbItemLimitWeapon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbItemLimitWeapon.Location = new System.Drawing.Point(9, 19);
            this.cbItemLimitWeapon.Name = "cbItemLimitWeapon";
            this.cbItemLimitWeapon.Size = new System.Drawing.Size(128, 17);
            this.cbItemLimitWeapon.TabIndex = 49;
            this.cbItemLimitWeapon.Text = "Weapon:";
            this.cbItemLimitWeapon.UseVisualStyleBackColor = true;
            // 
            // gbSkills
            // 
            this.gbSkills.Controls.Add(this.btnRemoveSkill);
            this.gbSkills.Controls.Add(this.btnAddSkill);
            this.gbSkills.Controls.Add(this.lstSkill);
            this.gbSkills.Location = new System.Drawing.Point(222, 106);
            this.gbSkills.Name = "gbSkills";
            this.gbSkills.Size = new System.Drawing.Size(142, 227);
            this.gbSkills.TabIndex = 56;
            this.gbSkills.TabStop = false;
            this.gbSkills.Text = "Skills";
            // 
            // btnRemoveSkill
            // 
            this.btnRemoveSkill.Location = new System.Drawing.Point(6, 201);
            this.btnRemoveSkill.Name = "btnRemoveSkill";
            this.btnRemoveSkill.Size = new System.Drawing.Size(130, 23);
            this.btnRemoveSkill.TabIndex = 2;
            this.btnRemoveSkill.Text = "Remove Skill";
            this.btnRemoveSkill.UseVisualStyleBackColor = true;
            this.btnRemoveSkill.Click += new System.EventHandler(this.btnRemoveSkill_Click);
            // 
            // btnAddSkill
            // 
            this.btnAddSkill.Location = new System.Drawing.Point(6, 172);
            this.btnAddSkill.Name = "btnAddSkill";
            this.btnAddSkill.Size = new System.Drawing.Size(130, 23);
            this.btnAddSkill.TabIndex = 1;
            this.btnAddSkill.Text = "Add Skill";
            this.btnAddSkill.UseVisualStyleBackColor = true;
            this.btnAddSkill.Click += new System.EventHandler(this.btnAddSkill_Click);
            // 
            // lstSkill
            // 
            this.lstSkill.FormattingEnabled = true;
            this.lstSkill.Location = new System.Drawing.Point(6, 19);
            this.lstSkill.Name = "lstSkill";
            this.lstSkill.Size = new System.Drawing.Size(130, 147);
            this.lstSkill.TabIndex = 0;
            // 
            // CreatureCardEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 337);
            this.Controls.Add(this.gbSkills);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbAffinities);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbCreatureInformation);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbCardInformation);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "CreatureCardEditor";
            this.Text = "Creature Card Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbCardInformation.ResumeLayout(false);
            this.gbCardInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).EndInit();
            this.gbCreatureInformation.ResumeLayout(false);
            this.gbCreatureInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxST)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxHP)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAir)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEarth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWater)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNeutral)).EndInit();
            this.gbAffinities.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.gbSkills.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox gbCardInformation;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblMagicCost;
        private System.Windows.Forms.NumericUpDown txtMagicCost;
        private System.Windows.Forms.Label lblRarity;
        private System.Windows.Forms.ComboBox cboRarity;
        private System.Windows.Forms.GroupBox gbCreatureInformation;
        private System.Windows.Forms.NumericUpDown txtMaxHP;
        private System.Windows.Forms.Label lblMaxHP;
        private System.Windows.Forms.NumericUpDown txtMaxST;
        private System.Windows.Forms.Label lblMaxST;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown txtNeutral;
        private System.Windows.Forms.Label lblNeutral;
        private System.Windows.Forms.NumericUpDown txtAir;
        private System.Windows.Forms.Label lblAir;
        private System.Windows.Forms.NumericUpDown txtEarth;
        private System.Windows.Forms.Label lblEarth;
        private System.Windows.Forms.NumericUpDown txtWater;
        private System.Windows.Forms.Label lblWater;
        private System.Windows.Forms.NumericUpDown txtFire;
        private System.Windows.Forms.Label lblFire;
        private System.Windows.Forms.GroupBox gbAffinities;
        private System.Windows.Forms.CheckBox cbAffinityAir;
        private System.Windows.Forms.CheckBox cbAffinityFire;
        private System.Windows.Forms.CheckBox cbAffinityWater;
        private System.Windows.Forms.CheckBox cbAffinityEarth;
        private System.Windows.Forms.CheckBox cbAffinityNeutral;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbLandLimitAir;
        private System.Windows.Forms.CheckBox cbLandLimitFire;
        private System.Windows.Forms.CheckBox cbLandLimitWater;
        private System.Windows.Forms.CheckBox cbLandLimitEarth;
        private System.Windows.Forms.CheckBox cbLandLimitNeutral;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox cbItemLimitArmor;
        private System.Windows.Forms.CheckBox cbItemLimitTools;
        private System.Windows.Forms.CheckBox cbItemLimitScrolls;
        private System.Windows.Forms.CheckBox cbItemLimitWeapon;
        private System.Windows.Forms.GroupBox gbSkills;
        private System.Windows.Forms.Button btnRemoveSkill;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.ListBox lstSkill;
    }
}