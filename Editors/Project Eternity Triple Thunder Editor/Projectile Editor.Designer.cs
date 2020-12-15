namespace ProjectEternity.Editors.TripleThunderWeaponEditor
{
    partial class ProjectileEditor
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
            this.gbProjectileInformation = new System.Windows.Forms.GroupBox();
            this.lvlTrailEffect = new System.Windows.Forms.Label();
            this.cboTrailEffect = new System.Windows.Forms.ComboBox();
            this.lblProjectileEffect = new System.Windows.Forms.Label();
            this.cboProjectileEffect = new System.Windows.Forms.ComboBox();
            this.btnTextureTrail = new System.Windows.Forms.Button();
            this.btnAnimatedTrail = new System.Windows.Forms.Button();
            this.txtTrailPath = new System.Windows.Forms.TextBox();
            this.lblTrailPath = new System.Windows.Forms.Label();
            this.lblTrailStyle = new System.Windows.Forms.Label();
            this.cboTrailStyle = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lblProjectileSpeed = new System.Windows.Forms.Label();
            this.txtProjectileSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblAffectedByGravity = new System.Windows.Forms.Label();
            this.ckAffectedByGravity = new System.Windows.Forms.CheckBox();
            this.btnUseTextureProjectile = new System.Windows.Forms.Button();
            this.btnUseAnimatedProjectile = new System.Windows.Forms.Button();
            this.txtProjectilePath = new System.Windows.Forms.TextBox();
            this.lblProjectilePath = new System.Windows.Forms.Label();
            this.gbBulletTypes = new System.Windows.Forms.GroupBox();
            this.btnRemoveBulletType = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBulletType = new System.Windows.Forms.TextBox();
            this.btnAddBulletType = new System.Windows.Forms.Button();
            this.lstBulletTypes = new System.Windows.Forms.ListBox();
            this.gbProjectileInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectileSpeed)).BeginInit();
            this.gbBulletTypes.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbProjectileInformation
            // 
            this.gbProjectileInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProjectileInformation.Controls.Add(this.lvlTrailEffect);
            this.gbProjectileInformation.Controls.Add(this.cboTrailEffect);
            this.gbProjectileInformation.Controls.Add(this.lblProjectileEffect);
            this.gbProjectileInformation.Controls.Add(this.cboProjectileEffect);
            this.gbProjectileInformation.Controls.Add(this.btnTextureTrail);
            this.gbProjectileInformation.Controls.Add(this.btnAnimatedTrail);
            this.gbProjectileInformation.Controls.Add(this.txtTrailPath);
            this.gbProjectileInformation.Controls.Add(this.lblTrailPath);
            this.gbProjectileInformation.Controls.Add(this.lblTrailStyle);
            this.gbProjectileInformation.Controls.Add(this.cboTrailStyle);
            this.gbProjectileInformation.Controls.Add(this.btnCancel);
            this.gbProjectileInformation.Controls.Add(this.btnConfirm);
            this.gbProjectileInformation.Controls.Add(this.lblProjectileSpeed);
            this.gbProjectileInformation.Controls.Add(this.txtProjectileSpeed);
            this.gbProjectileInformation.Controls.Add(this.lblAffectedByGravity);
            this.gbProjectileInformation.Controls.Add(this.ckAffectedByGravity);
            this.gbProjectileInformation.Controls.Add(this.btnUseTextureProjectile);
            this.gbProjectileInformation.Controls.Add(this.btnUseAnimatedProjectile);
            this.gbProjectileInformation.Controls.Add(this.txtProjectilePath);
            this.gbProjectileInformation.Controls.Add(this.lblProjectilePath);
            this.gbProjectileInformation.Location = new System.Drawing.Point(186, 12);
            this.gbProjectileInformation.Name = "gbProjectileInformation";
            this.gbProjectileInformation.Size = new System.Drawing.Size(293, 319);
            this.gbProjectileInformation.TabIndex = 0;
            this.gbProjectileInformation.TabStop = false;
            this.gbProjectileInformation.Text = "Projectiles informations";
            // 
            // lvlTrailEffect
            // 
            this.lvlTrailEffect.AutoSize = true;
            this.lvlTrailEffect.Location = new System.Drawing.Point(6, 193);
            this.lvlTrailEffect.Name = "lvlTrailEffect";
            this.lvlTrailEffect.Size = new System.Drawing.Size(57, 13);
            this.lvlTrailEffect.TabIndex = 57;
            this.lvlTrailEffect.Text = "Trail effect";
            // 
            // cboTrailEffect
            // 
            this.cboTrailEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTrailEffect.FormattingEnabled = true;
            this.cboTrailEffect.Items.AddRange(new object[] {
            "Alpha Blend",
            "Additive",
            "Negative"});
            this.cboTrailEffect.Location = new System.Drawing.Point(166, 190);
            this.cboTrailEffect.Name = "cboTrailEffect";
            this.cboTrailEffect.Size = new System.Drawing.Size(121, 21);
            this.cboTrailEffect.TabIndex = 56;
            // 
            // lblProjectileEffect
            // 
            this.lblProjectileEffect.AutoSize = true;
            this.lblProjectileEffect.Location = new System.Drawing.Point(6, 139);
            this.lblProjectileEffect.Name = "lblProjectileEffect";
            this.lblProjectileEffect.Size = new System.Drawing.Size(80, 13);
            this.lblProjectileEffect.TabIndex = 55;
            this.lblProjectileEffect.Text = "Projectile effect";
            // 
            // cboProjectileEffect
            // 
            this.cboProjectileEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProjectileEffect.FormattingEnabled = true;
            this.cboProjectileEffect.Items.AddRange(new object[] {
            "Alpha Blend",
            "Additive",
            "Negative"});
            this.cboProjectileEffect.Location = new System.Drawing.Point(166, 136);
            this.cboProjectileEffect.Name = "cboProjectileEffect";
            this.cboProjectileEffect.Size = new System.Drawing.Size(121, 21);
            this.cboProjectileEffect.TabIndex = 54;
            // 
            // btnTextureTrail
            // 
            this.btnTextureTrail.Location = new System.Drawing.Point(146, 255);
            this.btnTextureTrail.Name = "btnTextureTrail";
            this.btnTextureTrail.Size = new System.Drawing.Size(134, 23);
            this.btnTextureTrail.TabIndex = 53;
            this.btnTextureTrail.Text = "Use Texture Trail";
            this.btnTextureTrail.UseVisualStyleBackColor = true;
            this.btnTextureTrail.Click += new System.EventHandler(this.btnTextureTrail_Click);
            // 
            // btnAnimatedTrail
            // 
            this.btnAnimatedTrail.Location = new System.Drawing.Point(6, 255);
            this.btnAnimatedTrail.Name = "btnAnimatedTrail";
            this.btnAnimatedTrail.Size = new System.Drawing.Size(134, 23);
            this.btnAnimatedTrail.TabIndex = 52;
            this.btnAnimatedTrail.Text = "Use Animated Trail";
            this.btnAnimatedTrail.UseVisualStyleBackColor = true;
            this.btnAnimatedTrail.Click += new System.EventHandler(this.btnAnimatedTrail_Click);
            // 
            // txtTrailPath
            // 
            this.txtTrailPath.Location = new System.Drawing.Point(6, 229);
            this.txtTrailPath.Name = "txtTrailPath";
            this.txtTrailPath.ReadOnly = true;
            this.txtTrailPath.Size = new System.Drawing.Size(274, 20);
            this.txtTrailPath.TabIndex = 51;
            // 
            // lblTrailPath
            // 
            this.lblTrailPath.AutoSize = true;
            this.lblTrailPath.Location = new System.Drawing.Point(6, 213);
            this.lblTrailPath.Name = "lblTrailPath";
            this.lblTrailPath.Size = new System.Drawing.Size(54, 13);
            this.lblTrailPath.TabIndex = 50;
            this.lblTrailPath.Text = "Trail path:";
            // 
            // lblTrailStyle
            // 
            this.lblTrailStyle.AutoSize = true;
            this.lblTrailStyle.Location = new System.Drawing.Point(6, 166);
            this.lblTrailStyle.Name = "lblTrailStyle";
            this.lblTrailStyle.Size = new System.Drawing.Size(51, 13);
            this.lblTrailStyle.TabIndex = 49;
            this.lblTrailStyle.Text = "Trail style";
            // 
            // cboTrailStyle
            // 
            this.cboTrailStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTrailStyle.FormattingEnabled = true;
            this.cboTrailStyle.Items.AddRange(new object[] {
            "None",
            "Line",
            "Sprite"});
            this.cboTrailStyle.Location = new System.Drawing.Point(166, 163);
            this.cboTrailStyle.Name = "cboTrailStyle";
            this.cboTrailStyle.Size = new System.Drawing.Size(121, 21);
            this.cboTrailStyle.TabIndex = 48;
            this.cboTrailStyle.SelectedIndexChanged += new System.EventHandler(this.cboTrailStyle_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(212, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 47;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfirm.Location = new System.Drawing.Point(6, 290);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 46;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lblProjectileSpeed
            // 
            this.lblProjectileSpeed.AutoSize = true;
            this.lblProjectileSpeed.Location = new System.Drawing.Point(6, 89);
            this.lblProjectileSpeed.Name = "lblProjectileSpeed";
            this.lblProjectileSpeed.Size = new System.Drawing.Size(82, 13);
            this.lblProjectileSpeed.TabIndex = 45;
            this.lblProjectileSpeed.Text = "Projectile speed";
            // 
            // txtProjectileSpeed
            // 
            this.txtProjectileSpeed.Location = new System.Drawing.Point(160, 87);
            this.txtProjectileSpeed.Name = "txtProjectileSpeed";
            this.txtProjectileSpeed.Size = new System.Drawing.Size(120, 20);
            this.txtProjectileSpeed.TabIndex = 44;
            // 
            // lblAffectedByGravity
            // 
            this.lblAffectedByGravity.AutoSize = true;
            this.lblAffectedByGravity.Location = new System.Drawing.Point(6, 114);
            this.lblAffectedByGravity.Name = "lblAffectedByGravity";
            this.lblAffectedByGravity.Size = new System.Drawing.Size(95, 13);
            this.lblAffectedByGravity.TabIndex = 43;
            this.lblAffectedByGravity.Text = "Affected by gravity";
            // 
            // ckAffectedByGravity
            // 
            this.ckAffectedByGravity.AutoSize = true;
            this.ckAffectedByGravity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckAffectedByGravity.Location = new System.Drawing.Point(265, 114);
            this.ckAffectedByGravity.Margin = new System.Windows.Forms.Padding(3, 4, 3, 5);
            this.ckAffectedByGravity.Name = "ckAffectedByGravity";
            this.ckAffectedByGravity.Size = new System.Drawing.Size(15, 14);
            this.ckAffectedByGravity.TabIndex = 42;
            this.ckAffectedByGravity.UseVisualStyleBackColor = true;
            // 
            // btnUseTextureProjectile
            // 
            this.btnUseTextureProjectile.Location = new System.Drawing.Point(146, 58);
            this.btnUseTextureProjectile.Name = "btnUseTextureProjectile";
            this.btnUseTextureProjectile.Size = new System.Drawing.Size(134, 23);
            this.btnUseTextureProjectile.TabIndex = 3;
            this.btnUseTextureProjectile.Text = "Use Texture Projectile";
            this.btnUseTextureProjectile.UseVisualStyleBackColor = true;
            this.btnUseTextureProjectile.Click += new System.EventHandler(this.btnUseTextureProjectile_Click);
            // 
            // btnUseAnimatedProjectile
            // 
            this.btnUseAnimatedProjectile.Location = new System.Drawing.Point(6, 58);
            this.btnUseAnimatedProjectile.Name = "btnUseAnimatedProjectile";
            this.btnUseAnimatedProjectile.Size = new System.Drawing.Size(134, 23);
            this.btnUseAnimatedProjectile.TabIndex = 2;
            this.btnUseAnimatedProjectile.Text = "Use Animated Projectile";
            this.btnUseAnimatedProjectile.UseVisualStyleBackColor = true;
            this.btnUseAnimatedProjectile.Click += new System.EventHandler(this.btnUseAnimatedProjectile_Click);
            // 
            // txtProjectilePath
            // 
            this.txtProjectilePath.Location = new System.Drawing.Point(6, 32);
            this.txtProjectilePath.Name = "txtProjectilePath";
            this.txtProjectilePath.ReadOnly = true;
            this.txtProjectilePath.Size = new System.Drawing.Size(274, 20);
            this.txtProjectilePath.TabIndex = 1;
            // 
            // lblProjectilePath
            // 
            this.lblProjectilePath.AutoSize = true;
            this.lblProjectilePath.Location = new System.Drawing.Point(6, 16);
            this.lblProjectilePath.Name = "lblProjectilePath";
            this.lblProjectilePath.Size = new System.Drawing.Size(77, 13);
            this.lblProjectilePath.TabIndex = 0;
            this.lblProjectilePath.Text = "Projectile path:";
            // 
            // gbBulletTypes
            // 
            this.gbBulletTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBulletTypes.Controls.Add(this.btnRemoveBulletType);
            this.gbBulletTypes.Controls.Add(this.label1);
            this.gbBulletTypes.Controls.Add(this.txtBulletType);
            this.gbBulletTypes.Controls.Add(this.btnAddBulletType);
            this.gbBulletTypes.Controls.Add(this.lstBulletTypes);
            this.gbBulletTypes.Location = new System.Drawing.Point(12, 12);
            this.gbBulletTypes.Name = "gbBulletTypes";
            this.gbBulletTypes.Size = new System.Drawing.Size(168, 319);
            this.gbBulletTypes.TabIndex = 1;
            this.gbBulletTypes.TabStop = false;
            this.gbBulletTypes.Text = "Bullet types";
            // 
            // btnRemoveBulletType
            // 
            this.btnRemoveBulletType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBulletType.Location = new System.Drawing.Point(87, 159);
            this.btnRemoveBulletType.Name = "btnRemoveBulletType";
            this.btnRemoveBulletType.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveBulletType.TabIndex = 3;
            this.btnRemoveBulletType.Text = "Remove";
            this.btnRemoveBulletType.UseVisualStyleBackColor = true;
            this.btnRemoveBulletType.Click += new System.EventHandler(this.btnRemoveBulletType_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Type name:";
            // 
            // txtBulletType
            // 
            this.txtBulletType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBulletType.Location = new System.Drawing.Point(6, 133);
            this.txtBulletType.Name = "txtBulletType";
            this.txtBulletType.Size = new System.Drawing.Size(156, 20);
            this.txtBulletType.TabIndex = 2;
            this.txtBulletType.TextChanged += new System.EventHandler(this.txtBulletType_TextChanged);
            // 
            // btnAddBulletType
            // 
            this.btnAddBulletType.Location = new System.Drawing.Point(6, 159);
            this.btnAddBulletType.Name = "btnAddBulletType";
            this.btnAddBulletType.Size = new System.Drawing.Size(75, 23);
            this.btnAddBulletType.TabIndex = 2;
            this.btnAddBulletType.Text = "Add";
            this.btnAddBulletType.UseVisualStyleBackColor = true;
            this.btnAddBulletType.Click += new System.EventHandler(this.btnAddBulletType_Click);
            // 
            // lstBulletTypes
            // 
            this.lstBulletTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBulletTypes.FormattingEnabled = true;
            this.lstBulletTypes.Location = new System.Drawing.Point(6, 19);
            this.lstBulletTypes.Name = "lstBulletTypes";
            this.lstBulletTypes.Size = new System.Drawing.Size(156, 95);
            this.lstBulletTypes.TabIndex = 2;
            this.lstBulletTypes.SelectedIndexChanged += new System.EventHandler(this.lstBulletTypes_SelectedIndexChanged);
            // 
            // ProjectileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 343);
            this.Controls.Add(this.gbBulletTypes);
            this.Controls.Add(this.gbProjectileInformation);
            this.Name = "ProjectileEditor";
            this.Text = "Projectile Editor";
            this.gbProjectileInformation.ResumeLayout(false);
            this.gbProjectileInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProjectileSpeed)).EndInit();
            this.gbBulletTypes.ResumeLayout(false);
            this.gbBulletTypes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbProjectileInformation;
        private System.Windows.Forms.Button btnUseAnimatedProjectile;
        private System.Windows.Forms.Label lblProjectilePath;
        private System.Windows.Forms.Button btnUseTextureProjectile;
        public System.Windows.Forms.TextBox txtProjectilePath;
        private System.Windows.Forms.Label lblProjectileSpeed;
        public System.Windows.Forms.NumericUpDown txtProjectileSpeed;
        private System.Windows.Forms.Label lblAffectedByGravity;
        public System.Windows.Forms.CheckBox ckAffectedByGravity;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Label lblTrailStyle;
        private System.Windows.Forms.Button btnTextureTrail;
        private System.Windows.Forms.Button btnAnimatedTrail;
        public System.Windows.Forms.TextBox txtTrailPath;
        private System.Windows.Forms.Label lblTrailPath;
        public System.Windows.Forms.ComboBox cboTrailStyle;
        private System.Windows.Forms.Label lvlTrailEffect;
        public System.Windows.Forms.ComboBox cboTrailEffect;
        private System.Windows.Forms.Label lblProjectileEffect;
        public System.Windows.Forms.ComboBox cboProjectileEffect;
        private System.Windows.Forms.GroupBox gbBulletTypes;
        private System.Windows.Forms.Button btnRemoveBulletType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBulletType;
        private System.Windows.Forms.Button btnAddBulletType;
        private System.Windows.Forms.ListBox lstBulletTypes;
    }
}
