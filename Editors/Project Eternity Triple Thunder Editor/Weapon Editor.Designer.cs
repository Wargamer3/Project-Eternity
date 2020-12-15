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
            this.gbAnimations = new System.Windows.Forms.GroupBox();
            this.rbAirborne = new System.Windows.Forms.RadioButton();
            this.rbDash = new System.Windows.Forms.RadioButton();
            this.rbRunning = new System.Windows.Forms.RadioButton();
            this.rbMoving = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.txtCombo = new System.Windows.Forms.TextBox();
            this.btnSelectCombo = new System.Windows.Forms.Button();
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
            this.lblReloadAnimation = new System.Windows.Forms.Label();
            this.txtReloadAnimation = new System.Windows.Forms.TextBox();
            this.lblAmmoRegen = new System.Windows.Forms.Label();
            this.btnReloadAnimation = new System.Windows.Forms.Button();
            this.txtAmmoRegen = new System.Windows.Forms.NumericUpDown();
            this.lblAmmoPerMagazine = new System.Windows.Forms.Label();
            this.txtAmmoPerMagazine = new System.Windows.Forms.NumericUpDown();
            this.gbRangedProperties = new System.Windows.Forms.GroupBox();
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
            this.lblRecoilRecoverySpeed = new System.Windows.Forms.Label();
            this.txtRecoilRecoverySpeed = new System.Windows.Forms.NumericUpDown();
            this.gbAnimations.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbWeaponProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinAngle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxDurability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoRegen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmoPerMagazine)).BeginInit();
            this.gbRangedProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxRecoil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoilRecoverySpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // gbAnimations
            // 
            this.gbAnimations.Controls.Add(this.rbAirborne);
            this.gbAnimations.Controls.Add(this.rbDash);
            this.gbAnimations.Controls.Add(this.rbRunning);
            this.gbAnimations.Controls.Add(this.rbMoving);
            this.gbAnimations.Controls.Add(this.rbNone);
            this.gbAnimations.Controls.Add(this.txtCombo);
            this.gbAnimations.Controls.Add(this.btnSelectCombo);
            this.gbAnimations.Location = new System.Drawing.Point(12, 27);
            this.gbAnimations.Name = "gbAnimations";
            this.gbAnimations.Size = new System.Drawing.Size(270, 93);
            this.gbAnimations.TabIndex = 1;
            this.gbAnimations.TabStop = false;
            this.gbAnimations.Text = "Animations";
            // 
            // rbAirborne
            // 
            this.rbAirborne.AutoSize = true;
            this.rbAirborne.Location = new System.Drawing.Point(6, 42);
            this.rbAirborne.Name = "rbAirborne";
            this.rbAirborne.Size = new System.Drawing.Size(64, 17);
            this.rbAirborne.TabIndex = 16;
            this.rbAirborne.Text = "Airborne";
            this.rbAirborne.UseVisualStyleBackColor = true;
            this.rbAirborne.CheckedChanged += new System.EventHandler(this.rbAnimations_CheckedChanged);
            // 
            // rbDash
            // 
            this.rbDash.AutoSize = true;
            this.rbDash.Location = new System.Drawing.Point(200, 19);
            this.rbDash.Name = "rbDash";
            this.rbDash.Size = new System.Drawing.Size(50, 17);
            this.rbDash.TabIndex = 15;
            this.rbDash.Text = "Dash";
            this.rbDash.UseVisualStyleBackColor = true;
            this.rbDash.CheckedChanged += new System.EventHandler(this.rbAnimations_CheckedChanged);
            // 
            // rbRunning
            // 
            this.rbRunning.AutoSize = true;
            this.rbRunning.Location = new System.Drawing.Point(129, 19);
            this.rbRunning.Name = "rbRunning";
            this.rbRunning.Size = new System.Drawing.Size(65, 17);
            this.rbRunning.TabIndex = 14;
            this.rbRunning.Text = "Running";
            this.rbRunning.UseVisualStyleBackColor = true;
            this.rbRunning.CheckedChanged += new System.EventHandler(this.rbAnimations_CheckedChanged);
            // 
            // rbMoving
            // 
            this.rbMoving.AutoSize = true;
            this.rbMoving.Location = new System.Drawing.Point(63, 19);
            this.rbMoving.Name = "rbMoving";
            this.rbMoving.Size = new System.Drawing.Size(60, 17);
            this.rbMoving.TabIndex = 10;
            this.rbMoving.Text = "Moving";
            this.rbMoving.UseVisualStyleBackColor = true;
            this.rbMoving.CheckedChanged += new System.EventHandler(this.rbAnimations_CheckedChanged);
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(6, 19);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(42, 17);
            this.rbNone.TabIndex = 6;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "Idle";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rbAnimations_CheckedChanged);
            // 
            // txtCombo
            // 
            this.txtCombo.Location = new System.Drawing.Point(6, 65);
            this.txtCombo.Name = "txtCombo";
            this.txtCombo.ReadOnly = true;
            this.txtCombo.Size = new System.Drawing.Size(146, 20);
            this.txtCombo.TabIndex = 1;
            // 
            // btnSelectCombo
            // 
            this.btnSelectCombo.Location = new System.Drawing.Point(158, 63);
            this.btnSelectCombo.Name = "btnSelectCombo";
            this.btnSelectCombo.Size = new System.Drawing.Size(106, 23);
            this.btnSelectCombo.TabIndex = 1;
            this.btnSelectCombo.Text = "Select combo";
            this.btnSelectCombo.UseVisualStyleBackColor = true;
            this.btnSelectCombo.Click += new System.EventHandler(this.btnSelectCombo_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmExplosionAttributes});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(569, 24);
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
            // tsmExplosionAttributes
            // 
            this.tsmExplosionAttributes.Name = "tsmExplosionAttributes";
            this.tsmExplosionAttributes.Size = new System.Drawing.Size(123, 20);
            this.tsmExplosionAttributes.Text = "Explosion attributes";
            this.tsmExplosionAttributes.Click += new System.EventHandler(this.tsmExplosionAttributes_Click);
            // 
            // gbWeaponProperties
            // 
            this.gbWeaponProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbWeaponProperties.Controls.Add(this.lblSkillChain);
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
            this.gbWeaponProperties.Location = new System.Drawing.Point(13, 123);
            this.gbWeaponProperties.Name = "gbWeaponProperties";
            this.gbWeaponProperties.Size = new System.Drawing.Size(269, 216);
            this.gbWeaponProperties.TabIndex = 6;
            this.gbWeaponProperties.TabStop = false;
            this.gbWeaponProperties.Text = "Weapon Properties";
            // 
            // lblSkillChain
            // 
            this.lblSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSkillChain.AutoSize = true;
            this.lblSkillChain.Location = new System.Drawing.Point(6, 133);
            this.lblSkillChain.Name = "lblSkillChain";
            this.lblSkillChain.Size = new System.Drawing.Size(56, 13);
            this.lblSkillChain.TabIndex = 37;
            this.lblSkillChain.Text = "Skill Chain";
            // 
            // txtSkillChain
            // 
            this.txtSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSkillChain.Location = new System.Drawing.Point(6, 149);
            this.txtSkillChain.Name = "txtSkillChain";
            this.txtSkillChain.ReadOnly = true;
            this.txtSkillChain.Size = new System.Drawing.Size(146, 20);
            this.txtSkillChain.TabIndex = 36;
            // 
            // btnSelectSkillChain
            // 
            this.btnSelectSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectSkillChain.Location = new System.Drawing.Point(158, 147);
            this.btnSelectSkillChain.Name = "btnSelectSkillChain";
            this.btnSelectSkillChain.Size = new System.Drawing.Size(105, 23);
            this.btnSelectSkillChain.TabIndex = 35;
            this.btnSelectSkillChain.Text = "Select Skill Chain";
            this.btnSelectSkillChain.UseVisualStyleBackColor = true;
            this.btnSelectSkillChain.Click += new System.EventHandler(this.btnSelectSkillChain_Click);
            // 
            // lblUseRangedProperties
            // 
            this.lblUseRangedProperties.AutoSize = true;
            this.lblUseRangedProperties.Location = new System.Drawing.Point(6, 98);
            this.lblUseRangedProperties.Name = "lblUseRangedProperties";
            this.lblUseRangedProperties.Size = new System.Drawing.Size(117, 13);
            this.lblUseRangedProperties.TabIndex = 34;
            this.lblUseRangedProperties.Text = "Use Ranged Properties";
            // 
            // ckUseRangedProperties
            // 
            this.ckUseRangedProperties.AutoSize = true;
            this.ckUseRangedProperties.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckUseRangedProperties.Location = new System.Drawing.Point(248, 98);
            this.ckUseRangedProperties.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
            this.ckUseRangedProperties.Name = "ckUseRangedProperties";
            this.ckUseRangedProperties.Size = new System.Drawing.Size(15, 14);
            this.ckUseRangedProperties.TabIndex = 33;
            this.ckUseRangedProperties.UseVisualStyleBackColor = true;
            this.ckUseRangedProperties.CheckedChanged += new System.EventHandler(this.ckUseRangedProperties_CheckedChanged);
            // 
            // lblMaxAngle
            // 
            this.lblMaxAngle.AutoSize = true;
            this.lblMaxAngle.Location = new System.Drawing.Point(7, 73);
            this.lblMaxAngle.Name = "lblMaxAngle";
            this.lblMaxAngle.Size = new System.Drawing.Size(57, 13);
            this.lblMaxAngle.TabIndex = 5;
            this.lblMaxAngle.Text = "Max Angle";
            // 
            // txtMaxAngle
            // 
            this.txtMaxAngle.Location = new System.Drawing.Point(143, 71);
            this.txtMaxAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtMaxAngle.Name = "txtMaxAngle";
            this.txtMaxAngle.Size = new System.Drawing.Size(120, 20);
            this.txtMaxAngle.TabIndex = 4;
            // 
            // lblMinAngle
            // 
            this.lblMinAngle.AutoSize = true;
            this.lblMinAngle.Location = new System.Drawing.Point(6, 47);
            this.lblMinAngle.Name = "lblMinAngle";
            this.lblMinAngle.Size = new System.Drawing.Size(54, 13);
            this.lblMinAngle.TabIndex = 3;
            this.lblMinAngle.Text = "Min Angle";
            // 
            // txtMinAngle
            // 
            this.txtMinAngle.Location = new System.Drawing.Point(142, 45);
            this.txtMinAngle.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtMinAngle.Name = "txtMinAngle";
            this.txtMinAngle.Size = new System.Drawing.Size(120, 20);
            this.txtMinAngle.TabIndex = 2;
            // 
            // lblMaxDurability
            // 
            this.lblMaxDurability.AutoSize = true;
            this.lblMaxDurability.Location = new System.Drawing.Point(6, 21);
            this.lblMaxDurability.Name = "lblMaxDurability";
            this.lblMaxDurability.Size = new System.Drawing.Size(111, 13);
            this.lblMaxDurability.TabIndex = 1;
            this.lblMaxDurability.Text = "Max durability / Ammo";
            // 
            // txtMaxDurability
            // 
            this.txtMaxDurability.Location = new System.Drawing.Point(142, 19);
            this.txtMaxDurability.Name = "txtMaxDurability";
            this.txtMaxDurability.Size = new System.Drawing.Size(120, 20);
            this.txtMaxDurability.TabIndex = 0;
            // 
            // lblReloadAnimation
            // 
            this.lblReloadAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblReloadAnimation.AutoSize = true;
            this.lblReloadAnimation.Location = new System.Drawing.Point(6, 266);
            this.lblReloadAnimation.Name = "lblReloadAnimation";
            this.lblReloadAnimation.Size = new System.Drawing.Size(90, 13);
            this.lblReloadAnimation.TabIndex = 19;
            this.lblReloadAnimation.Text = "Reload Animation";
            // 
            // txtReloadAnimation
            // 
            this.txtReloadAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtReloadAnimation.Location = new System.Drawing.Point(6, 282);
            this.txtReloadAnimation.Name = "txtReloadAnimation";
            this.txtReloadAnimation.ReadOnly = true;
            this.txtReloadAnimation.Size = new System.Drawing.Size(146, 20);
            this.txtReloadAnimation.TabIndex = 17;
            // 
            // lblAmmoRegen
            // 
            this.lblAmmoRegen.AutoSize = true;
            this.lblAmmoRegen.Location = new System.Drawing.Point(6, 73);
            this.lblAmmoRegen.Name = "lblAmmoRegen";
            this.lblAmmoRegen.Size = new System.Drawing.Size(66, 13);
            this.lblAmmoRegen.TabIndex = 5;
            this.lblAmmoRegen.Text = "Ammo regen";
            // 
            // btnReloadAnimation
            // 
            this.btnReloadAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReloadAnimation.Location = new System.Drawing.Point(158, 280);
            this.btnReloadAnimation.Name = "btnReloadAnimation";
            this.btnReloadAnimation.Size = new System.Drawing.Size(105, 23);
            this.btnReloadAnimation.TabIndex = 18;
            this.btnReloadAnimation.Text = "Select combo";
            this.btnReloadAnimation.UseVisualStyleBackColor = true;
            this.btnReloadAnimation.Click += new System.EventHandler(this.btnReloadAnimation_Click);
            // 
            // txtAmmoRegen
            // 
            this.txtAmmoRegen.Location = new System.Drawing.Point(143, 71);
            this.txtAmmoRegen.Name = "txtAmmoRegen";
            this.txtAmmoRegen.Size = new System.Drawing.Size(120, 20);
            this.txtAmmoRegen.TabIndex = 4;
            // 
            // lblAmmoPerMagazine
            // 
            this.lblAmmoPerMagazine.AutoSize = true;
            this.lblAmmoPerMagazine.Location = new System.Drawing.Point(6, 47);
            this.lblAmmoPerMagazine.Name = "lblAmmoPerMagazine";
            this.lblAmmoPerMagazine.Size = new System.Drawing.Size(102, 13);
            this.lblAmmoPerMagazine.TabIndex = 3;
            this.lblAmmoPerMagazine.Text = "Ammo per magazine";
            // 
            // txtAmmoPerMagazine
            // 
            this.txtAmmoPerMagazine.Location = new System.Drawing.Point(143, 45);
            this.txtAmmoPerMagazine.Name = "txtAmmoPerMagazine";
            this.txtAmmoPerMagazine.Size = new System.Drawing.Size(120, 20);
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
            this.gbRangedProperties.Controls.Add(this.txtReloadAnimation);
            this.gbRangedProperties.Controls.Add(this.lblReloadAnimation);
            this.gbRangedProperties.Controls.Add(this.txtAmmoPerMagazine);
            this.gbRangedProperties.Controls.Add(this.txtAmmoRegen);
            this.gbRangedProperties.Controls.Add(this.lblAmmoPerMagazine);
            this.gbRangedProperties.Controls.Add(this.btnReloadAnimation);
            this.gbRangedProperties.Enabled = false;
            this.gbRangedProperties.Location = new System.Drawing.Point(288, 27);
            this.gbRangedProperties.Name = "gbRangedProperties";
            this.gbRangedProperties.Size = new System.Drawing.Size(269, 312);
            this.gbRangedProperties.TabIndex = 8;
            this.gbRangedProperties.TabStop = false;
            this.gbRangedProperties.Text = "Ranged Properties";
            // 
            // txtDamage
            // 
            this.txtDamage.Location = new System.Drawing.Point(143, 19);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(120, 20);
            this.txtDamage.TabIndex = 36;
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(6, 21);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(47, 13);
            this.lblDamage.TabIndex = 37;
            this.lblDamage.Text = "Damage";
            // 
            // txtProjectile
            // 
            this.txtProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtProjectile.Location = new System.Drawing.Point(6, 242);
            this.txtProjectile.Name = "txtProjectile";
            this.txtProjectile.ReadOnly = true;
            this.txtProjectile.Size = new System.Drawing.Size(146, 20);
            this.txtProjectile.TabIndex = 33;
            // 
            // lblProjectile
            // 
            this.lblProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProjectile.AutoSize = true;
            this.lblProjectile.Location = new System.Drawing.Point(6, 226);
            this.lblProjectile.Name = "lblProjectile";
            this.lblProjectile.Size = new System.Drawing.Size(50, 13);
            this.lblProjectile.TabIndex = 35;
            this.lblProjectile.Text = "Projectile";
            // 
            // btnSelectProjectile
            // 
            this.btnSelectProjectile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectProjectile.Location = new System.Drawing.Point(158, 240);
            this.btnSelectProjectile.Name = "btnSelectProjectile";
            this.btnSelectProjectile.Size = new System.Drawing.Size(105, 23);
            this.btnSelectProjectile.TabIndex = 34;
            this.btnSelectProjectile.Text = "Select projectile";
            this.btnSelectProjectile.UseVisualStyleBackColor = true;
            this.btnSelectProjectile.Click += new System.EventHandler(this.btnSelectProjectile_Click);
            // 
            // lblProjectileType
            // 
            this.lblProjectileType.AutoSize = true;
            this.lblProjectileType.Location = new System.Drawing.Point(6, 204);
            this.lblProjectileType.Name = "lblProjectileType";
            this.lblProjectileType.Size = new System.Drawing.Size(73, 13);
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
            this.cbProjectileType.Location = new System.Drawing.Point(142, 201);
            this.cbProjectileType.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.cbProjectileType.Name = "cbProjectileType";
            this.cbProjectileType.Size = new System.Drawing.Size(121, 21);
            this.cbProjectileType.TabIndex = 31;
            this.cbProjectileType.SelectedIndexChanged += new System.EventHandler(this.cbProjectileType_SelectedIndexChanged);
            // 
            // lblNumberOfProjectiles
            // 
            this.lblNumberOfProjectiles.AutoSize = true;
            this.lblNumberOfProjectiles.Location = new System.Drawing.Point(6, 177);
            this.lblNumberOfProjectiles.Name = "lblNumberOfProjectiles";
            this.lblNumberOfProjectiles.Size = new System.Drawing.Size(106, 13);
            this.lblNumberOfProjectiles.TabIndex = 27;
            this.lblNumberOfProjectiles.Text = "Number of projectiles";
            // 
            // txtNumberOfProjectiles
            // 
            this.txtNumberOfProjectiles.Location = new System.Drawing.Point(143, 175);
            this.txtNumberOfProjectiles.Name = "txtNumberOfProjectiles";
            this.txtNumberOfProjectiles.Size = new System.Drawing.Size(120, 20);
            this.txtNumberOfProjectiles.TabIndex = 26;
            // 
            // lblMaxRecoil
            // 
            this.lblMaxRecoil.AutoSize = true;
            this.lblMaxRecoil.Location = new System.Drawing.Point(6, 125);
            this.lblMaxRecoil.Name = "lblMaxRecoil";
            this.lblMaxRecoil.Size = new System.Drawing.Size(55, 13);
            this.lblMaxRecoil.TabIndex = 23;
            this.lblMaxRecoil.Text = "Max recoil";
            // 
            // txtMaxRecoil
            // 
            this.txtMaxRecoil.Location = new System.Drawing.Point(143, 123);
            this.txtMaxRecoil.Name = "txtMaxRecoil";
            this.txtMaxRecoil.Size = new System.Drawing.Size(120, 20);
            this.txtMaxRecoil.TabIndex = 22;
            // 
            // lblRecoil
            // 
            this.lblRecoil.AutoSize = true;
            this.lblRecoil.Location = new System.Drawing.Point(6, 99);
            this.lblRecoil.Name = "lblRecoil";
            this.lblRecoil.Size = new System.Drawing.Size(37, 13);
            this.lblRecoil.TabIndex = 21;
            this.lblRecoil.Text = "Recoil";
            // 
            // txtRecoil
            // 
            this.txtRecoil.Location = new System.Drawing.Point(143, 97);
            this.txtRecoil.Name = "txtRecoil";
            this.txtRecoil.Size = new System.Drawing.Size(120, 20);
            this.txtRecoil.TabIndex = 20;
            // 
            // lblRecoilRecoverySpeed
            // 
            this.lblRecoilRecoverySpeed.AutoSize = true;
            this.lblRecoilRecoverySpeed.Location = new System.Drawing.Point(6, 151);
            this.lblRecoilRecoverySpeed.Name = "lblRecoilRecoverySpeed";
            this.lblRecoilRecoverySpeed.Size = new System.Drawing.Size(113, 13);
            this.lblRecoilRecoverySpeed.TabIndex = 39;
            this.lblRecoilRecoverySpeed.Text = "Recoil recovery speed";
            // 
            // txtRecoilRecoverySpeed
            // 
            this.txtRecoilRecoverySpeed.Location = new System.Drawing.Point(143, 149);
            this.txtRecoilRecoverySpeed.Name = "txtRecoilRecoverySpeed";
            this.txtRecoilRecoverySpeed.Size = new System.Drawing.Size(120, 20);
            this.txtRecoilRecoverySpeed.TabIndex = 38;
            // 
            // WeaponEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 353);
            this.Controls.Add(this.gbRangedProperties);
            this.Controls.Add(this.gbWeaponProperties);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbAnimations);
            this.Name = "WeaponEditor";
            this.Text = "Triple Thunder Weapon Editor";
            this.gbAnimations.ResumeLayout(false);
            this.gbAnimations.PerformLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxRecoil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRecoilRecoverySpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAnimations;
        private System.Windows.Forms.RadioButton rbAirborne;
        private System.Windows.Forms.RadioButton rbDash;
        private System.Windows.Forms.RadioButton rbRunning;
        private System.Windows.Forms.RadioButton rbMoving;
        private System.Windows.Forms.RadioButton rbNone;
        private System.Windows.Forms.TextBox txtCombo;
        private System.Windows.Forms.Button btnSelectCombo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbWeaponProperties;
        private System.Windows.Forms.Label lblMaxDurability;
        private System.Windows.Forms.NumericUpDown txtMaxDurability;
        private System.Windows.Forms.Label lblReloadAnimation;
        private System.Windows.Forms.TextBox txtReloadAnimation;
        private System.Windows.Forms.Label lblAmmoRegen;
        private System.Windows.Forms.Button btnReloadAnimation;
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
    }
}

