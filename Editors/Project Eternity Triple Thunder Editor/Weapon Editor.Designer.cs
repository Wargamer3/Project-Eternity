namespace ProjectEternity.Editors.TripleThunderWeaponEditor
{
    partial class WeaponEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmExplosionAttributes = new System.Windows.Forms.ToolStripMenuItem();
            this.gbWeaponProperties = new System.Windows.Forms.GroupBox();
            this.lblSkillChain = new System.Windows.Forms.Label();
            this.txtSkillChain = new System.Windows.Forms.TextBox();
            this.btnSelectSkillChain = new System.Windows.Forms.Button();
            this.lblUseRangedProperties = new System.Windows.Forms.Label();
            this.ckUseRangedProperties = new System.Windows.Forms.CheckBox();
            this.lblMaxAngle = new System.Windows.Forms.Label();
            this.txtMaxAngle = new System.Windows.Forms.NumericUpDown();
            this.lblMinAngle = new System.Windows.Forms.Label();
            this.txtMinAngle = new System.Windows.Forms.NumericUpDown();
            this.lblMaxDurability = new System.Windows.Forms.Label();
            this.txtMaxDurability = new System.Windows.Forms.NumericUpDown();
            this.lblAmmoRegen = new System.Windows.Forms.Label();
            this.txtAmmoRegen = new System.Windows.Forms.NumericUpDown();
            this.lblAmmoPerMagazine = new System.Windows.Forms.Label();
            this.txtAmmoPerMagazine = new System.Windows.Forms.NumericUpDown();
            this.gbRangedProperties = new System.Windows.Forms.GroupBox();
            this.lblWeaponType = new System.Windows.Forms.Label();
            this.cbWeaponType = new System.Windows.Forms.ComboBox();
            this.lblRecoilRecoverySpeed = new System.Windows.Forms.Label();
            this.txtRecoilRecoverySpeed = new System.Windows.Forms.NumericUpDown();
            this.txtDamage = new System.Windows.Forms.NumericUpDown();
            this.lblDamage = new System.Windows.Forms.Label();
            this.txtProjectile = new System.Windows.Forms.TextBox();
            this.lblProjectile = new System.Windows.Forms.Label();
            this.btnSelectProjectile = new System.Windows.Forms.Button();
            this.lblProjectileType = new System.Windows.Forms.Label();
            this.cbProjectileType = new System.Windows.Forms.ComboBox();
            this.lblNumberOfProjectiles = new System.Windows.Forms.Label();
            this.txtNumberOfProjectiles = new System.Windows.Forms.NumericUpDown();
            this.lblMaxRecoil = new System.Windows.Forms.Label();
            this.txtMaxRecoil = new System.Windows.Forms.NumericUpDown();
            this.lblRecoil = new System.Windows.Forms.Label();
            this.txtRecoil = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1.SuspendLayout();
            this.gbWeaponProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxDurability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoRegen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoPerMagazine)).BeginInit();
            this.gbRangedProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoilRecoverySpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxRecoil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoil)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmExplosionAttributes});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(12, 4, 0, 4);
            this.menuStrip1.Size = new System.Drawing.Size(1138, 44);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(77, 36);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmExplosionAttributes
            // 
            this.tsmExplosionAttributes.Name = "tsmExplosionAttributes";
            this.tsmExplosionAttributes.Size = new System.Drawing.Size(236, 36);
            this.tsmExplosionAttributes.Text = "Explosion attributes";
            this.tsmExplosionAttributes.Click += new System.EventHandler(this.tsmExplosionAttributes_Click);
            // 
            // gbWeaponProperties
            // 
            this.gbWeaponProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbWeaponProperties.Controls.Add(this.lblWeaponType);
            this.gbWeaponProperties.Controls.Add(this.lblSkillChain);
            this.gbWeaponProperties.Controls.Add(this.cbWeaponType);
            this.gbWeaponProperties.Controls.Add(this.txtSkillChain);
            this.gbWeaponProperties.Controls.Add(this.btnSelectSkillChain);
            this.gbWeaponProperties.Controls.Add(this.lblUseRangedProperties);
            this.gbWeaponProperties.Controls.Add(this.ckUseRangedProperties);
            this.gbWeaponProperties.Controls.Add(this.lblMaxAngle);
            this.gbWeaponProperties.Controls.Add(this.txtMaxAngle);
            this.gbWeaponProperties.Controls.Add(this.lblMinAngle);
            this.gbWeaponProperties.Controls.Add(this.txtMinAngle);
            this.gbWeaponProperties.Controls.Add(this.lblMaxDurability);
            this.gbWeaponProperties.Controls.Add(this.txtMaxDurability);
            this.gbWeaponProperties.Location = new System.Drawing.Point(24, 63);
            this.gbWeaponProperties.Margin = new System.Windows.Forms.Padding(6);
            this.gbWeaponProperties.Name = "gbWeaponProperties";
            this.gbWeaponProperties.Padding = new System.Windows.Forms.Padding(6);
            this.gbWeaponProperties.Size = new System.Drawing.Size(538, 514);
            this.gbWeaponProperties.TabIndex = 6;
            this.gbWeaponProperties.TabStop = false;
            this.gbWeaponProperties.Text = "Weapon Properties";
            // 
            // lblSkillChain
            // 
            this.lblSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSkillChain.AutoSize = true;
            this.lblSkillChain.Location = new System.Drawing.Point(12, 263);
            this.lblSkillChain.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblSkillChain.Name = "lblSkillChain";
            this.lblSkillChain.Size = new System.Drawing.Size(114, 25);
            this.lblSkillChain.TabIndex = 37;
            this.lblSkillChain.Text = "Skill Chain";
            // 
            // txtSkillChain
            // 
            this.txtSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSkillChain.Location = new System.Drawing.Point(12, 294);
            this.txtSkillChain.Margin = new System.Windows.Forms.Padding(6);
            this.txtSkillChain.Name = "txtSkillChain";
            this.txtSkillChain.ReadOnly = true;
            this.txtSkillChain.Size = new System.Drawing.Size(288, 31);
            this.txtSkillChain.TabIndex = 36;
            // 
            // btnSelectSkillChain
            // 
            this.btnSelectSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectSkillChain.Location = new System.Drawing.Point(316, 290);
            this.btnSelectSkillChain.Margin = new System.Windows.Forms.Padding(6);
            this.btnSelectSkillChain.Name = "btnSelectSkillChain";
            this.btnSelectSkillChain.Size = new System.Drawing.Size(210, 44);
            this.btnSelectSkillChain.TabIndex = 35;
            this.btnSelectSkillChain.Text = "Select Skill Chain";
            this.btnSelectSkillChain.UseVisualStyleBackColor = true;
            this.btnSelectSkillChain.Click += new System.EventHandler(this.btnSelectSkillChain_Click);
            // 
            // lblUseRangedProperties
            // 
            this.lblUseRangedProperties.AutoSize = true;
            this.lblUseRangedProperties.Location = new System.Drawing.Point(12, 218);
            this.lblUseRangedProperties.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblUseRangedProperties.Name = "lblUseRangedProperties";
            this.lblUseRangedProperties.Size = new System.Drawing.Size(235, 25);
            this.lblUseRangedProperties.TabIndex = 34;
            this.lblUseRangedProperties.Text = "Use Ranged Properties";
            // 
            // ckUseRangedProperties
            // 
            this.ckUseRangedProperties.AutoSize = true;
            this.ckUseRangedProperties.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckUseRangedProperties.Location = new System.Drawing.Point(498, 218);
            this.ckUseRangedProperties.Margin = new System.Windows.Forms.Padding(6, 8, 6, 10);
            this.ckUseRangedProperties.Name = "ckUseRangedProperties";
            this.ckUseRangedProperties.Size = new System.Drawing.Size(28, 27);
            this.ckUseRangedProperties.TabIndex = 33;
            this.ckUseRangedProperties.UseVisualStyleBackColor = true;
            this.ckUseRangedProperties.CheckedChanged += new System.EventHandler(this.ckUseRangedProperties_CheckedChanged);
            // 
            // lblMaxAngle
            // 
            this.lblMaxAngle.AutoSize = true;
            this.lblMaxAngle.Location = new System.Drawing.Point(12, 175);
            this.lblMaxAngle.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMaxAngle.Name = "lblMaxAngle";
            this.lblMaxAngle.Size = new System.Drawing.Size(114, 25);
            this.lblMaxAngle.TabIndex = 5;
            this.lblMaxAngle.Text = "Max Angle";
            // 
            // txtMaxAngle
            // 
            this.txtMaxAngle.Location = new System.Drawing.Point(286, 173);
            this.txtMaxAngle.Margin = new System.Windows.Forms.Padding(6);
            this.txtMaxAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtMaxAngle.Name = "txtMaxAngle";
            this.txtMaxAngle.Size = new System.Drawing.Size(240, 31);
            this.txtMaxAngle.TabIndex = 4;
            // 
            // lblMinAngle
            // 
            this.lblMinAngle.AutoSize = true;
            this.lblMinAngle.Location = new System.Drawing.Point(12, 132);
            this.lblMinAngle.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMinAngle.Name = "lblMinAngle";
            this.lblMinAngle.Size = new System.Drawing.Size(108, 25);
            this.lblMinAngle.TabIndex = 3;
            this.lblMinAngle.Text = "Min Angle";
            // 
            // txtMinAngle
            // 
            this.txtMinAngle.Location = new System.Drawing.Point(286, 130);
            this.txtMinAngle.Margin = new System.Windows.Forms.Padding(6);
            this.txtMinAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtMinAngle.Name = "txtMinAngle";
            this.txtMinAngle.Size = new System.Drawing.Size(240, 31);
            this.txtMinAngle.TabIndex = 2;
            // 
            // lblMaxDurability
            // 
            this.lblMaxDurability.AutoSize = true;
            this.lblMaxDurability.Location = new System.Drawing.Point(12, 89);
            this.lblMaxDurability.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMaxDurability.Name = "lblMaxDurability";
            this.lblMaxDurability.Size = new System.Drawing.Size(224, 25);
            this.lblMaxDurability.TabIndex = 1;
            this.lblMaxDurability.Text = "Max durability / Ammo";
            // 
            // txtMaxDurability
            // 
            this.txtMaxDurability.Location = new System.Drawing.Point(286, 87);
            this.txtMaxDurability.Margin = new System.Windows.Forms.Padding(6);
            this.txtMaxDurability.Name = "txtMaxDurability";
            this.txtMaxDurability.Size = new System.Drawing.Size(240, 31);
            this.txtMaxDurability.TabIndex = 0;
            // 
            // lblAmmoRegen
            // 
            this.lblAmmoRegen.AutoSize = true;
            this.lblAmmoRegen.Location = new System.Drawing.Point(12, 140);
            this.lblAmmoRegen.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblAmmoRegen.Name = "lblAmmoRegen";
            this.lblAmmoRegen.Size = new System.Drawing.Size(133, 25);
            this.lblAmmoRegen.TabIndex = 5;
            this.lblAmmoRegen.Text = "Ammo regen";
            // 
            // txtAmmoRegen
            // 
            this.txtAmmoRegen.Location = new System.Drawing.Point(286, 137);
            this.txtAmmoRegen.Margin = new System.Windows.Forms.Padding(6);
            this.txtAmmoRegen.Name = "txtAmmoRegen";
            this.txtAmmoRegen.Size = new System.Drawing.Size(240, 31);
            this.txtAmmoRegen.TabIndex = 4;
            // 
            // lblAmmoPerMagazine
            // 
            this.lblAmmoPerMagazine.AutoSize = true;
            this.lblAmmoPerMagazine.Location = new System.Drawing.Point(12, 90);
            this.lblAmmoPerMagazine.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblAmmoPerMagazine.Name = "lblAmmoPerMagazine";
            this.lblAmmoPerMagazine.Size = new System.Drawing.Size(208, 25);
            this.lblAmmoPerMagazine.TabIndex = 3;
            this.lblAmmoPerMagazine.Text = "Ammo per magazine";
            // 
            // txtAmmoPerMagazine
            // 
            this.txtAmmoPerMagazine.Location = new System.Drawing.Point(286, 87);
            this.txtAmmoPerMagazine.Margin = new System.Windows.Forms.Padding(6);
            this.txtAmmoPerMagazine.Name = "txtAmmoPerMagazine";
            this.txtAmmoPerMagazine.Size = new System.Drawing.Size(240, 31);
            this.txtAmmoPerMagazine.TabIndex = 2;
            // 
            // gbRangedProperties
            // 
            this.gbRangedProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbRangedProperties.Controls.Add(this.lblRecoilRecoverySpeed);
            this.gbRangedProperties.Controls.Add(this.txtRecoilRecoverySpeed);
            this.gbRangedProperties.Controls.Add(this.txtDamage);
            this.gbRangedProperties.Controls.Add(this.lblDamage);
            this.gbRangedProperties.Controls.Add(this.txtProjectile);
            this.gbRangedProperties.Controls.Add(this.lblProjectile);
            this.gbRangedProperties.Controls.Add(this.btnSelectProjectile);
            this.gbRangedProperties.Controls.Add(this.lblProjectileType);
            this.gbRangedProperties.Controls.Add(this.cbProjectileType);
            this.gbRangedProperties.Controls.Add(this.lblNumberOfProjectiles);
            this.gbRangedProperties.Controls.Add(this.txtNumberOfProjectiles);
            this.gbRangedProperties.Controls.Add(this.lblMaxRecoil);
            this.gbRangedProperties.Controls.Add(this.txtMaxRecoil);
            this.gbRangedProperties.Controls.Add(this.lblRecoil);
            this.gbRangedProperties.Controls.Add(this.txtRecoil);
            this.gbRangedProperties.Controls.Add(this.lblAmmoRegen);
            this.gbRangedProperties.Controls.Add(this.txtAmmoPerMagazine);
            this.gbRangedProperties.Controls.Add(this.txtAmmoRegen);
            this.gbRangedProperties.Controls.Add(this.lblAmmoPerMagazine);
            this.gbRangedProperties.Enabled = false;
            this.gbRangedProperties.Location = new System.Drawing.Point(576, 52);
            this.gbRangedProperties.Margin = new System.Windows.Forms.Padding(6);
            this.gbRangedProperties.Name = "gbRangedProperties";
            this.gbRangedProperties.Padding = new System.Windows.Forms.Padding(6);
            this.gbRangedProperties.Size = new System.Drawing.Size(538, 525);
            this.gbRangedProperties.TabIndex = 8;
            this.gbRangedProperties.TabStop = false;
            this.gbRangedProperties.Text = "Ranged Properties";
            // 
            // lblWeaponType
            // 
            this.lblWeaponType.AutoSize = true;
            this.lblWeaponType.Location = new System.Drawing.Point(12, 47);
            this.lblWeaponType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblWeaponType.Name = "lblWeaponType";
            this.lblWeaponType.Size = new System.Drawing.Size(139, 25);
            this.lblWeaponType.TabIndex = 41;
            this.lblWeaponType.Text = "Weapon type";
            // 
            // cbWeaponType
            // 
            this.cbWeaponType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeaponType.FormattingEnabled = true;
            this.cbWeaponType.Items.AddRange(new object[] {
            "Combo",
            "Simple"});
            this.cbWeaponType.Location = new System.Drawing.Point(288, 44);
            this.cbWeaponType.Margin = new System.Windows.Forms.Padding(6, 6, 6, 4);
            this.cbWeaponType.Name = "cbWeaponType";
            this.cbWeaponType.Size = new System.Drawing.Size(238, 33);
            this.cbWeaponType.TabIndex = 40;
            // 
            // lblRecoilRecoverySpeed
            // 
            this.lblRecoilRecoverySpeed.AutoSize = true;
            this.lblRecoilRecoverySpeed.Location = new System.Drawing.Point(12, 290);
            this.lblRecoilRecoverySpeed.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblRecoilRecoverySpeed.Name = "lblRecoilRecoverySpeed";
            this.lblRecoilRecoverySpeed.Size = new System.Drawing.Size(226, 25);
            this.lblRecoilRecoverySpeed.TabIndex = 39;
            this.lblRecoilRecoverySpeed.Text = "Recoil recovery speed";
            // 
            // txtRecoilRecoverySpeed
            // 
            this.txtRecoilRecoverySpeed.Location = new System.Drawing.Point(286, 287);
            this.txtRecoilRecoverySpeed.Margin = new System.Windows.Forms.Padding(6);
            this.txtRecoilRecoverySpeed.Name = "txtRecoilRecoverySpeed";
            this.txtRecoilRecoverySpeed.Size = new System.Drawing.Size(240, 31);
            this.txtRecoilRecoverySpeed.TabIndex = 38;
            // 
            // txtDamage
            // 
            this.txtDamage.Location = new System.Drawing.Point(286, 37);
            this.txtDamage.Margin = new System.Windows.Forms.Padding(6);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(240, 31);
            this.txtDamage.TabIndex = 36;
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(12, 40);
            this.lblDamage.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(92, 25);
            this.lblDamage.TabIndex = 37;
            this.lblDamage.Text = "Damage";
            // 
            // txtProjectile
            // 
            this.txtProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtProjectile.Location = new System.Drawing.Point(12, 469);
            this.txtProjectile.Margin = new System.Windows.Forms.Padding(6);
            this.txtProjectile.Name = "txtProjectile";
            this.txtProjectile.ReadOnly = true;
            this.txtProjectile.Size = new System.Drawing.Size(288, 31);
            this.txtProjectile.TabIndex = 33;
            // 
            // lblProjectile
            // 
            this.lblProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProjectile.AutoSize = true;
            this.lblProjectile.Location = new System.Drawing.Point(12, 439);
            this.lblProjectile.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProjectile.Name = "lblProjectile";
            this.lblProjectile.Size = new System.Drawing.Size(101, 25);
            this.lblProjectile.TabIndex = 35;
            this.lblProjectile.Text = "Projectile";
            // 
            // btnSelectProjectile
            // 
            this.btnSelectProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectProjectile.Location = new System.Drawing.Point(316, 466);
            this.btnSelectProjectile.Margin = new System.Windows.Forms.Padding(6);
            this.btnSelectProjectile.Name = "btnSelectProjectile";
            this.btnSelectProjectile.Size = new System.Drawing.Size(210, 44);
            this.btnSelectProjectile.TabIndex = 34;
            this.btnSelectProjectile.Text = "Select projectile";
            this.btnSelectProjectile.UseVisualStyleBackColor = true;
            this.btnSelectProjectile.Click += new System.EventHandler(this.btnSelectProjectile_Click);
            // 
            // lblProjectileType
            // 
            this.lblProjectileType.AutoSize = true;
            this.lblProjectileType.Location = new System.Drawing.Point(12, 392);
            this.lblProjectileType.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProjectileType.Name = "lblProjectileType";
            this.lblProjectileType.Size = new System.Drawing.Size(148, 25);
            this.lblProjectileType.TabIndex = 32;
            this.lblProjectileType.Text = "Projectile type";
            // 
            // cbProjectileType
            // 
            this.cbProjectileType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProjectileType.FormattingEnabled = true;
            this.cbProjectileType.Items.AddRange(new object[] {
            "Hitscan",
            "Projectile"});
            this.cbProjectileType.Location = new System.Drawing.Point(284, 387);
            this.cbProjectileType.Margin = new System.Windows.Forms.Padding(6, 6, 6, 4);
            this.cbProjectileType.Name = "cbProjectileType";
            this.cbProjectileType.Size = new System.Drawing.Size(238, 33);
            this.cbProjectileType.TabIndex = 31;
            this.cbProjectileType.SelectedIndexChanged += new System.EventHandler(this.cbProjectileType_SelectedIndexChanged);
            // 
            // lblNumberOfProjectiles
            // 
            this.lblNumberOfProjectiles.AutoSize = true;
            this.lblNumberOfProjectiles.Location = new System.Drawing.Point(12, 340);
            this.lblNumberOfProjectiles.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblNumberOfProjectiles.Name = "lblNumberOfProjectiles";
            this.lblNumberOfProjectiles.Size = new System.Drawing.Size(215, 25);
            this.lblNumberOfProjectiles.TabIndex = 27;
            this.lblNumberOfProjectiles.Text = "Number of projectiles";
            // 
            // txtNumberOfProjectiles
            // 
            this.txtNumberOfProjectiles.Location = new System.Drawing.Point(286, 337);
            this.txtNumberOfProjectiles.Margin = new System.Windows.Forms.Padding(6);
            this.txtNumberOfProjectiles.Name = "txtNumberOfProjectiles";
            this.txtNumberOfProjectiles.Size = new System.Drawing.Size(240, 31);
            this.txtNumberOfProjectiles.TabIndex = 26;
            // 
            // lblMaxRecoil
            // 
            this.lblMaxRecoil.AutoSize = true;
            this.lblMaxRecoil.Location = new System.Drawing.Point(12, 240);
            this.lblMaxRecoil.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblMaxRecoil.Name = "lblMaxRecoil";
            this.lblMaxRecoil.Size = new System.Drawing.Size(111, 25);
            this.lblMaxRecoil.TabIndex = 23;
            this.lblMaxRecoil.Text = "Max recoil";
            // 
            // txtMaxRecoil
            // 
            this.txtMaxRecoil.Location = new System.Drawing.Point(286, 237);
            this.txtMaxRecoil.Margin = new System.Windows.Forms.Padding(6);
            this.txtMaxRecoil.Name = "txtMaxRecoil";
            this.txtMaxRecoil.Size = new System.Drawing.Size(240, 31);
            this.txtMaxRecoil.TabIndex = 22;
            // 
            // lblRecoil
            // 
            this.lblRecoil.AutoSize = true;
            this.lblRecoil.Location = new System.Drawing.Point(12, 190);
            this.lblRecoil.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblRecoil.Name = "lblRecoil";
            this.lblRecoil.Size = new System.Drawing.Size(72, 25);
            this.lblRecoil.TabIndex = 21;
            this.lblRecoil.Text = "Recoil";
            // 
            // txtRecoil
            // 
            this.txtRecoil.Location = new System.Drawing.Point(286, 187);
            this.txtRecoil.Margin = new System.Windows.Forms.Padding(6);
            this.txtRecoil.Name = "txtRecoil";
            this.txtRecoil.Size = new System.Drawing.Size(240, 31);
            this.txtRecoil.TabIndex = 20;
            // 
            // WeaponEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1138, 604);
            this.Controls.Add(this.gbRangedProperties);
            this.Controls.Add(this.gbWeaponProperties);
            this.Controls.Add(this.menuStrip1);
            this.Margin = new System.Windows.Forms.Padding(12);
            this.Name = "WeaponEditor";
            this.Text = "Triple Thunder Weapon Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbWeaponProperties.ResumeLayout(false);
            this.gbWeaponProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinAngle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxDurability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoRegen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoPerMagazine)).EndInit();
            this.gbRangedProperties.ResumeLayout(false);
            this.gbRangedProperties.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoilRecoverySpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxRecoil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoil)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbWeaponProperties;
        private System.Windows.Forms.Label lblMaxDurability;
        private System.Windows.Forms.NumericUpDown txtMaxDurability;
        private System.Windows.Forms.Label lblAmmoRegen;
        private System.Windows.Forms.NumericUpDown txtAmmoRegen;
        private System.Windows.Forms.Label lblAmmoPerMagazine;
        private System.Windows.Forms.NumericUpDown txtAmmoPerMagazine;
        private System.Windows.Forms.Label lblMaxAngle;
        private System.Windows.Forms.NumericUpDown txtMaxAngle;
        private System.Windows.Forms.Label lblMinAngle;
        private System.Windows.Forms.NumericUpDown txtMinAngle;
        private System.Windows.Forms.GroupBox gbRangedProperties;
        private System.Windows.Forms.Label lblMaxRecoil;
        private System.Windows.Forms.NumericUpDown txtMaxRecoil;
        private System.Windows.Forms.Label lblRecoil;
        private System.Windows.Forms.NumericUpDown txtRecoil;
        private System.Windows.Forms.Label lblNumberOfProjectiles;
        private System.Windows.Forms.NumericUpDown txtNumberOfProjectiles;
        private System.Windows.Forms.Label lblProjectileType;
        private System.Windows.Forms.ComboBox cbProjectileType;
        private System.Windows.Forms.Label lblUseRangedProperties;
        private System.Windows.Forms.CheckBox ckUseRangedProperties;
        private System.Windows.Forms.TextBox txtProjectile;
        private System.Windows.Forms.Label lblProjectile;
        private System.Windows.Forms.Button btnSelectProjectile;
        private System.Windows.Forms.ToolStripMenuItem tsmExplosionAttributes;
        private System.Windows.Forms.NumericUpDown txtDamage;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.Label lblSkillChain;
        private System.Windows.Forms.TextBox txtSkillChain;
        private System.Windows.Forms.Button btnSelectSkillChain;
        private System.Windows.Forms.Label lblRecoilRecoverySpeed;
        private System.Windows.Forms.NumericUpDown txtRecoilRecoverySpeed;
        private System.Windows.Forms.Label lblWeaponType;
        private System.Windows.Forms.ComboBox cbWeaponType;
    }
}

