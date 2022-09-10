namespace ProjectEternity.Editors.AttackEditor
{
    partial class PERAttackEditor
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
            this.gbProjectileAttributes = new System.Windows.Forms.GroupBox();
            this.lblAttackType = new System.Windows.Forms.Label();
            this.cbAttackType = new System.Windows.Forms.ComboBox();
            this.btn3DModelPath = new System.Windows.Forms.Button();
            this.txt3DModelPath = new System.Windows.Forms.TextBox();
            this.cbHoming = new System.Windows.Forms.CheckBox();
            this.lblMaxLifetime = new System.Windows.Forms.Label();
            this.txtMaxLifetime = new System.Windows.Forms.NumericUpDown();
            this.cbCanBeShotDown = new System.Windows.Forms.CheckBox();
            this.btnUseTextureProjectile = new System.Windows.Forms.Button();
            this.btnUseAnimatedProjectile = new System.Windows.Forms.Button();
            this.txtProjectilePath = new System.Windows.Forms.TextBox();
            this.lblProjectilePaths = new System.Windows.Forms.Label();
            this.lblProjectileSpeed = new System.Windows.Forms.Label();
            this.txtProjectileSpeed = new System.Windows.Forms.NumericUpDown();
            this.cbAffectedByGravity = new System.Windows.Forms.CheckBox();
            this.gbAttackAttributes = new System.Windows.Forms.GroupBox();
            this.lblUpwardMaxSpread = new System.Windows.Forms.Label();
            this.txtUpwardMaxSpread = new System.Windows.Forms.NumericUpDown();
            this.lblForwardMaxSpread = new System.Windows.Forms.Label();
            this.txtForwardMaxSpread = new System.Windows.Forms.NumericUpDown();
            this.lblLateralMaxSpread = new System.Windows.Forms.Label();
            this.txtLateralMaxSpread = new System.Windows.Forms.NumericUpDown();
            this.lblNumberOfProjectiles = new System.Windows.Forms.Label();
            this.txtNumberOfProjectiles = new System.Windows.Forms.NumericUpDown();
            this.lblSkillChain = new System.Windows.Forms.Label();
            this.txtSkillChain = new System.Windows.Forms.TextBox();
            this.btnSelectSkillChain = new System.Windows.Forms.Button();
            this.gbOnCollision = new System.Windows.Forms.GroupBox();
            this.lblBounceLimit = new System.Windows.Forms.Label();
            this.txtBounceLimit = new System.Windows.Forms.NumericUpDown();
            this.rbStop = new System.Windows.Forms.RadioButton();
            this.rbBounce = new System.Windows.Forms.RadioButton();
            this.rbDestroySelf = new System.Windows.Forms.RadioButton();
            this.gbProjectileAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLifetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectileSpeed)).BeginInit();
            this.gbAttackAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpwardMaxSpread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtForwardMaxSpread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLateralMaxSpread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).BeginInit();
            this.gbOnCollision.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBounceLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // gbProjectileAttributes
            // 
            this.gbProjectileAttributes.Controls.Add(this.lblAttackType);
            this.gbProjectileAttributes.Controls.Add(this.cbAttackType);
            this.gbProjectileAttributes.Controls.Add(this.btn3DModelPath);
            this.gbProjectileAttributes.Controls.Add(this.txt3DModelPath);
            this.gbProjectileAttributes.Controls.Add(this.cbHoming);
            this.gbProjectileAttributes.Controls.Add(this.lblMaxLifetime);
            this.gbProjectileAttributes.Controls.Add(this.txtMaxLifetime);
            this.gbProjectileAttributes.Controls.Add(this.cbCanBeShotDown);
            this.gbProjectileAttributes.Controls.Add(this.btnUseTextureProjectile);
            this.gbProjectileAttributes.Controls.Add(this.btnUseAnimatedProjectile);
            this.gbProjectileAttributes.Controls.Add(this.txtProjectilePath);
            this.gbProjectileAttributes.Controls.Add(this.lblProjectilePaths);
            this.gbProjectileAttributes.Controls.Add(this.lblProjectileSpeed);
            this.gbProjectileAttributes.Controls.Add(this.txtProjectileSpeed);
            this.gbProjectileAttributes.Controls.Add(this.cbAffectedByGravity);
            this.gbProjectileAttributes.Location = new System.Drawing.Point(12, 12);
            this.gbProjectileAttributes.Name = "gbProjectileAttributes";
            this.gbProjectileAttributes.Size = new System.Drawing.Size(416, 146);
            this.gbProjectileAttributes.TabIndex = 0;
            this.gbProjectileAttributes.TabStop = false;
            this.gbProjectileAttributes.Text = "Projectile Attributes";
            // 
            // lblAttackType
            // 
            this.lblAttackType.AutoSize = true;
            this.lblAttackType.Location = new System.Drawing.Point(217, 123);
            this.lblAttackType.Name = "lblAttackType";
            this.lblAttackType.Size = new System.Drawing.Size(65, 13);
            this.lblAttackType.TabIndex = 58;
            this.lblAttackType.Text = "Attack Type";
            // 
            // cbAttackType
            // 
            this.cbAttackType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttackType.FormattingEnabled = true;
            this.cbAttackType.Items.AddRange(new object[] {
            "Shoot",
            "Throw",
            "Kick",
            "Hold"});
            this.cbAttackType.Location = new System.Drawing.Point(289, 119);
            this.cbAttackType.Name = "cbAttackType";
            this.cbAttackType.Size = new System.Drawing.Size(121, 21);
            this.cbAttackType.TabIndex = 3;
            // 
            // btn3DModelPath
            // 
            this.btn3DModelPath.Location = new System.Drawing.Point(146, 89);
            this.btn3DModelPath.Name = "btn3DModelPath";
            this.btn3DModelPath.Size = new System.Drawing.Size(108, 23);
            this.btn3DModelPath.TabIndex = 57;
            this.btn3DModelPath.Text = "3D Model";
            this.btn3DModelPath.UseVisualStyleBackColor = true;
            this.btn3DModelPath.Click += new System.EventHandler(this.btn3DModelPath_Click);
            // 
            // txt3DModelPath
            // 
            this.txt3DModelPath.Location = new System.Drawing.Point(146, 63);
            this.txt3DModelPath.Name = "txt3DModelPath";
            this.txt3DModelPath.ReadOnly = true;
            this.txt3DModelPath.Size = new System.Drawing.Size(108, 20);
            this.txt3DModelPath.TabIndex = 56;
            // 
            // cbHoming
            // 
            this.cbHoming.AutoSize = true;
            this.cbHoming.Location = new System.Drawing.Point(260, 67);
            this.cbHoming.Name = "cbHoming";
            this.cbHoming.Size = new System.Drawing.Size(62, 17);
            this.cbHoming.TabIndex = 55;
            this.cbHoming.Text = "Homing";
            this.cbHoming.UseVisualStyleBackColor = true;
            // 
            // lblMaxLifetime
            // 
            this.lblMaxLifetime.AutoSize = true;
            this.lblMaxLifetime.Location = new System.Drawing.Point(264, 94);
            this.lblMaxLifetime.Name = "lblMaxLifetime";
            this.lblMaxLifetime.Size = new System.Drawing.Size(66, 13);
            this.lblMaxLifetime.TabIndex = 54;
            this.lblMaxLifetime.Text = "Max Lifetime";
            // 
            // txtMaxLifetime
            // 
            this.txtMaxLifetime.Location = new System.Drawing.Point(336, 90);
            this.txtMaxLifetime.Name = "txtMaxLifetime";
            this.txtMaxLifetime.Size = new System.Drawing.Size(71, 20);
            this.txtMaxLifetime.TabIndex = 53;
            // 
            // cbCanBeShotDown
            // 
            this.cbCanBeShotDown.AutoSize = true;
            this.cbCanBeShotDown.Location = new System.Drawing.Point(261, 44);
            this.cbCanBeShotDown.Name = "cbCanBeShotDown";
            this.cbCanBeShotDown.Size = new System.Drawing.Size(117, 17);
            this.cbCanBeShotDown.TabIndex = 52;
            this.cbCanBeShotDown.Text = "Can Be Shot Down";
            this.cbCanBeShotDown.UseVisualStyleBackColor = true;
            // 
            // btnUseTextureProjectile
            // 
            this.btnUseTextureProjectile.Location = new System.Drawing.Point(6, 118);
            this.btnUseTextureProjectile.Name = "btnUseTextureProjectile";
            this.btnUseTextureProjectile.Size = new System.Drawing.Size(134, 23);
            this.btnUseTextureProjectile.TabIndex = 51;
            this.btnUseTextureProjectile.Text = "Use Texture Projectile";
            this.btnUseTextureProjectile.UseVisualStyleBackColor = true;
            this.btnUseTextureProjectile.Click += new System.EventHandler(this.btnUseTextureProjectile_Click);
            // 
            // btnUseAnimatedProjectile
            // 
            this.btnUseAnimatedProjectile.Location = new System.Drawing.Point(6, 89);
            this.btnUseAnimatedProjectile.Name = "btnUseAnimatedProjectile";
            this.btnUseAnimatedProjectile.Size = new System.Drawing.Size(134, 23);
            this.btnUseAnimatedProjectile.TabIndex = 50;
            this.btnUseAnimatedProjectile.Text = "Use Animated Projectile";
            this.btnUseAnimatedProjectile.UseVisualStyleBackColor = true;
            this.btnUseAnimatedProjectile.Click += new System.EventHandler(this.btnUseAnimatedProjectile_Click);
            // 
            // txtProjectilePath
            // 
            this.txtProjectilePath.Location = new System.Drawing.Point(6, 63);
            this.txtProjectilePath.Name = "txtProjectilePath";
            this.txtProjectilePath.ReadOnly = true;
            this.txtProjectilePath.Size = new System.Drawing.Size(134, 20);
            this.txtProjectilePath.TabIndex = 49;
            // 
            // lblProjectilePaths
            // 
            this.lblProjectilePaths.AutoSize = true;
            this.lblProjectilePaths.Location = new System.Drawing.Point(6, 47);
            this.lblProjectilePaths.Name = "lblProjectilePaths";
            this.lblProjectilePaths.Size = new System.Drawing.Size(82, 13);
            this.lblProjectilePaths.TabIndex = 48;
            this.lblProjectilePaths.Text = "Projectile paths:";
            // 
            // lblProjectileSpeed
            // 
            this.lblProjectileSpeed.AutoSize = true;
            this.lblProjectileSpeed.Location = new System.Drawing.Point(6, 21);
            this.lblProjectileSpeed.Name = "lblProjectileSpeed";
            this.lblProjectileSpeed.Size = new System.Drawing.Size(82, 13);
            this.lblProjectileSpeed.TabIndex = 47;
            this.lblProjectileSpeed.Text = "Projectile speed";
            // 
            // txtProjectileSpeed
            // 
            this.txtProjectileSpeed.Location = new System.Drawing.Point(94, 19);
            this.txtProjectileSpeed.Name = "txtProjectileSpeed";
            this.txtProjectileSpeed.Size = new System.Drawing.Size(71, 20);
            this.txtProjectileSpeed.TabIndex = 46;
            // 
            // cbAffectedByGravity
            // 
            this.cbAffectedByGravity.AutoSize = true;
            this.cbAffectedByGravity.Location = new System.Drawing.Point(261, 21);
            this.cbAffectedByGravity.Name = "cbAffectedByGravity";
            this.cbAffectedByGravity.Size = new System.Drawing.Size(116, 17);
            this.cbAffectedByGravity.TabIndex = 2;
            this.cbAffectedByGravity.Text = "Affected by Gravity";
            this.cbAffectedByGravity.UseVisualStyleBackColor = true;
            // 
            // gbAttackAttributes
            // 
            this.gbAttackAttributes.Controls.Add(this.lblUpwardMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.txtUpwardMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.lblForwardMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.txtForwardMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.lblLateralMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.txtLateralMaxSpread);
            this.gbAttackAttributes.Controls.Add(this.lblNumberOfProjectiles);
            this.gbAttackAttributes.Controls.Add(this.txtNumberOfProjectiles);
            this.gbAttackAttributes.Controls.Add(this.lblSkillChain);
            this.gbAttackAttributes.Controls.Add(this.txtSkillChain);
            this.gbAttackAttributes.Controls.Add(this.btnSelectSkillChain);
            this.gbAttackAttributes.Location = new System.Drawing.Point(12, 164);
            this.gbAttackAttributes.Name = "gbAttackAttributes";
            this.gbAttackAttributes.Size = new System.Drawing.Size(249, 165);
            this.gbAttackAttributes.TabIndex = 1;
            this.gbAttackAttributes.TabStop = false;
            this.gbAttackAttributes.Text = "Attack Attributes";
            // 
            // lblUpwardMaxSpread
            // 
            this.lblUpwardMaxSpread.AutoSize = true;
            this.lblUpwardMaxSpread.Location = new System.Drawing.Point(6, 99);
            this.lblUpwardMaxSpread.Name = "lblUpwardMaxSpread";
            this.lblUpwardMaxSpread.Size = new System.Drawing.Size(104, 13);
            this.lblUpwardMaxSpread.TabIndex = 48;
            this.lblUpwardMaxSpread.Text = "Upward Max Spread";
            // 
            // txtUpwardMaxSpread
            // 
            this.txtUpwardMaxSpread.DecimalPlaces = 2;
            this.txtUpwardMaxSpread.Location = new System.Drawing.Point(171, 97);
            this.txtUpwardMaxSpread.Name = "txtUpwardMaxSpread";
            this.txtUpwardMaxSpread.Size = new System.Drawing.Size(71, 20);
            this.txtUpwardMaxSpread.TabIndex = 47;
            // 
            // lblForwardMaxSpread
            // 
            this.lblForwardMaxSpread.AutoSize = true;
            this.lblForwardMaxSpread.Location = new System.Drawing.Point(6, 73);
            this.lblForwardMaxSpread.Name = "lblForwardMaxSpread";
            this.lblForwardMaxSpread.Size = new System.Drawing.Size(105, 13);
            this.lblForwardMaxSpread.TabIndex = 46;
            this.lblForwardMaxSpread.Text = "Forward Max Spread";
            // 
            // txtForwardMaxSpread
            // 
            this.txtForwardMaxSpread.DecimalPlaces = 2;
            this.txtForwardMaxSpread.Location = new System.Drawing.Point(171, 71);
            this.txtForwardMaxSpread.Name = "txtForwardMaxSpread";
            this.txtForwardMaxSpread.Size = new System.Drawing.Size(71, 20);
            this.txtForwardMaxSpread.TabIndex = 45;
            // 
            // lblLateralMaxSpread
            // 
            this.lblLateralMaxSpread.AutoSize = true;
            this.lblLateralMaxSpread.Location = new System.Drawing.Point(6, 47);
            this.lblLateralMaxSpread.Name = "lblLateralMaxSpread";
            this.lblLateralMaxSpread.Size = new System.Drawing.Size(99, 13);
            this.lblLateralMaxSpread.TabIndex = 44;
            this.lblLateralMaxSpread.Text = "Lateral Max Spread";
            // 
            // txtLateralMaxSpread
            // 
            this.txtLateralMaxSpread.DecimalPlaces = 2;
            this.txtLateralMaxSpread.Location = new System.Drawing.Point(171, 45);
            this.txtLateralMaxSpread.Name = "txtLateralMaxSpread";
            this.txtLateralMaxSpread.Size = new System.Drawing.Size(71, 20);
            this.txtLateralMaxSpread.TabIndex = 43;
            // 
            // lblNumberOfProjectiles
            // 
            this.lblNumberOfProjectiles.AutoSize = true;
            this.lblNumberOfProjectiles.Location = new System.Drawing.Point(6, 21);
            this.lblNumberOfProjectiles.Name = "lblNumberOfProjectiles";
            this.lblNumberOfProjectiles.Size = new System.Drawing.Size(106, 13);
            this.lblNumberOfProjectiles.TabIndex = 42;
            this.lblNumberOfProjectiles.Text = "Number of projectiles";
            // 
            // txtNumberOfProjectiles
            // 
            this.txtNumberOfProjectiles.Location = new System.Drawing.Point(171, 19);
            this.txtNumberOfProjectiles.Name = "txtNumberOfProjectiles";
            this.txtNumberOfProjectiles.Size = new System.Drawing.Size(71, 20);
            this.txtNumberOfProjectiles.TabIndex = 38;
            // 
            // lblSkillChain
            // 
            this.lblSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSkillChain.AutoSize = true;
            this.lblSkillChain.Location = new System.Drawing.Point(6, 123);
            this.lblSkillChain.Name = "lblSkillChain";
            this.lblSkillChain.Size = new System.Drawing.Size(56, 13);
            this.lblSkillChain.TabIndex = 41;
            this.lblSkillChain.Text = "Skill Chain";
            // 
            // txtSkillChain
            // 
            this.txtSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSkillChain.Location = new System.Drawing.Point(6, 139);
            this.txtSkillChain.Name = "txtSkillChain";
            this.txtSkillChain.ReadOnly = true;
            this.txtSkillChain.Size = new System.Drawing.Size(126, 20);
            this.txtSkillChain.TabIndex = 40;
            // 
            // btnSelectSkillChain
            // 
            this.btnSelectSkillChain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectSkillChain.Location = new System.Drawing.Point(138, 136);
            this.btnSelectSkillChain.Name = "btnSelectSkillChain";
            this.btnSelectSkillChain.Size = new System.Drawing.Size(105, 23);
            this.btnSelectSkillChain.TabIndex = 39;
            this.btnSelectSkillChain.Text = "Select Skill Chain";
            this.btnSelectSkillChain.UseVisualStyleBackColor = true;
            this.btnSelectSkillChain.Click += new System.EventHandler(this.btnSelectSkillChain_Click);
            // 
            // gbOnCollision
            // 
            this.gbOnCollision.Controls.Add(this.lblBounceLimit);
            this.gbOnCollision.Controls.Add(this.txtBounceLimit);
            this.gbOnCollision.Controls.Add(this.rbStop);
            this.gbOnCollision.Controls.Add(this.rbBounce);
            this.gbOnCollision.Controls.Add(this.rbDestroySelf);
            this.gbOnCollision.Location = new System.Drawing.Point(267, 164);
            this.gbOnCollision.Name = "gbOnCollision";
            this.gbOnCollision.Size = new System.Drawing.Size(161, 117);
            this.gbOnCollision.TabIndex = 2;
            this.gbOnCollision.TabStop = false;
            this.gbOnCollision.Text = "On Collision";
            // 
            // lblBounceLimit
            // 
            this.lblBounceLimit.AutoSize = true;
            this.lblBounceLimit.Location = new System.Drawing.Point(6, 90);
            this.lblBounceLimit.Name = "lblBounceLimit";
            this.lblBounceLimit.Size = new System.Drawing.Size(68, 13);
            this.lblBounceLimit.TabIndex = 49;
            this.lblBounceLimit.Text = "Bounce Limit";
            // 
            // txtBounceLimit
            // 
            this.txtBounceLimit.Location = new System.Drawing.Point(84, 88);
            this.txtBounceLimit.Name = "txtBounceLimit";
            this.txtBounceLimit.Size = new System.Drawing.Size(71, 20);
            this.txtBounceLimit.TabIndex = 48;
            // 
            // rbStop
            // 
            this.rbStop.AutoSize = true;
            this.rbStop.Location = new System.Drawing.Point(6, 42);
            this.rbStop.Name = "rbStop";
            this.rbStop.Size = new System.Drawing.Size(47, 17);
            this.rbStop.TabIndex = 2;
            this.rbStop.Text = "Stop";
            this.rbStop.UseVisualStyleBackColor = true;
            // 
            // rbBounce
            // 
            this.rbBounce.AutoSize = true;
            this.rbBounce.Location = new System.Drawing.Point(6, 65);
            this.rbBounce.Name = "rbBounce";
            this.rbBounce.Size = new System.Drawing.Size(62, 17);
            this.rbBounce.TabIndex = 1;
            this.rbBounce.Text = "Bounce";
            this.rbBounce.UseVisualStyleBackColor = true;
            // 
            // rbDestroySelf
            // 
            this.rbDestroySelf.AutoSize = true;
            this.rbDestroySelf.Checked = true;
            this.rbDestroySelf.Location = new System.Drawing.Point(6, 19);
            this.rbDestroySelf.Name = "rbDestroySelf";
            this.rbDestroySelf.Size = new System.Drawing.Size(82, 17);
            this.rbDestroySelf.TabIndex = 0;
            this.rbDestroySelf.TabStop = true;
            this.rbDestroySelf.Text = "Destroy Self";
            this.rbDestroySelf.UseVisualStyleBackColor = true;
            // 
            // PERAttackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 341);
            this.Controls.Add(this.gbOnCollision);
            this.Controls.Add(this.gbAttackAttributes);
            this.Controls.Add(this.gbProjectileAttributes);
            this.Name = "PERAttackEditor";
            this.Text = "PER Attack Editor";
            this.gbProjectileAttributes.ResumeLayout(false);
            this.gbProjectileAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxLifetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectileSpeed)).EndInit();
            this.gbAttackAttributes.ResumeLayout(false);
            this.gbAttackAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpwardMaxSpread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtForwardMaxSpread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLateralMaxSpread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNumberOfProjectiles)).EndInit();
            this.gbOnCollision.ResumeLayout(false);
            this.gbOnCollision.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBounceLimit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbProjectileAttributes;
        private System.Windows.Forms.GroupBox gbAttackAttributes;
        private System.Windows.Forms.Label lblProjectileSpeed;
        public System.Windows.Forms.NumericUpDown txtProjectileSpeed;
        private System.Windows.Forms.Label lblSkillChain;
        private System.Windows.Forms.TextBox txtSkillChain;
        private System.Windows.Forms.Button btnSelectSkillChain;
        private System.Windows.Forms.Label lblNumberOfProjectiles;
        private System.Windows.Forms.Button btnUseTextureProjectile;
        private System.Windows.Forms.Button btnUseAnimatedProjectile;
        public System.Windows.Forms.TextBox txtProjectilePath;
        private System.Windows.Forms.Label lblProjectilePaths;
        private System.Windows.Forms.Label lblForwardMaxSpread;
        private System.Windows.Forms.Label lblLateralMaxSpread;
        private System.Windows.Forms.GroupBox gbOnCollision;
        private System.Windows.Forms.Label lblBounceLimit;
        public System.Windows.Forms.NumericUpDown txtBounceLimit;
        private System.Windows.Forms.Label lblMaxLifetime;
        public System.Windows.Forms.NumericUpDown txtMaxLifetime;
        public System.Windows.Forms.CheckBox cbAffectedByGravity;
        public System.Windows.Forms.NumericUpDown txtNumberOfProjectiles;
        public System.Windows.Forms.CheckBox cbCanBeShotDown;
        public System.Windows.Forms.NumericUpDown txtForwardMaxSpread;
        public System.Windows.Forms.NumericUpDown txtLateralMaxSpread;
        public System.Windows.Forms.RadioButton rbStop;
        public System.Windows.Forms.RadioButton rbBounce;
        public System.Windows.Forms.RadioButton rbDestroySelf;
        private System.Windows.Forms.Label lblUpwardMaxSpread;
        public System.Windows.Forms.NumericUpDown txtUpwardMaxSpread;
        public System.Windows.Forms.CheckBox cbHoming;
        private System.Windows.Forms.Button btn3DModelPath;
        public System.Windows.Forms.TextBox txt3DModelPath;
        private System.Windows.Forms.Label lblAttackType;
        public System.Windows.Forms.ComboBox cbAttackType;
    }
}