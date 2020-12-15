namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    partial class ExplosionAttributesEditor
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
            this.btnUseTextureProjectile = new System.Windows.Forms.Button();
            this.btnUseAnimatedProjectile = new System.Windows.Forms.Button();
            this.txtProjectilePath = new System.Windows.Forms.TextBox();
            this.lblExplosionPath = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lblExplosionWindPowerToSelfMultiplier = new System.Windows.Forms.Label();
            this.txtExplosionWindPowerToSelfMultiplier = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionDamageToSelfMultiplier = new System.Windows.Forms.Label();
            this.txtExplosionDamageToSelfMultiplier = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionDamageAtEdge = new System.Windows.Forms.Label();
            this.txtExplosionDamageAtEdge = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionDamageAtCenter = new System.Windows.Forms.Label();
            this.txtExplosionDamageAtCenter = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionWindPowerAtEdge = new System.Windows.Forms.Label();
            this.txtExplosionWindPowerAtEdge = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionWindPowerAtCenter = new System.Windows.Forms.Label();
            this.txtExplosionWindPowerAtCenter = new System.Windows.Forms.NumericUpDown();
            this.lblExplosionRadius = new System.Windows.Forms.Label();
            this.txtExplosionRadius = new System.Windows.Forms.NumericUpDown();
            this.txtSoundPath = new System.Windows.Forms.TextBox();
            this.lblSoundPath = new System.Windows.Forms.Label();
            this.btnSelectSound = new System.Windows.Forms.Button();
            this.gbProjectileInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerToSelfMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageToSelfMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageAtEdge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageAtCenter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerAtEdge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerAtCenter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // gbProjectileInformation
            // 
            this.gbProjectileInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbProjectileInformation.Controls.Add(this.btnSelectSound);
            this.gbProjectileInformation.Controls.Add(this.txtSoundPath);
            this.gbProjectileInformation.Controls.Add(this.lblSoundPath);
            this.gbProjectileInformation.Controls.Add(this.btnUseTextureProjectile);
            this.gbProjectileInformation.Controls.Add(this.btnUseAnimatedProjectile);
            this.gbProjectileInformation.Controls.Add(this.txtProjectilePath);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionPath);
            this.gbProjectileInformation.Controls.Add(this.btnCancel);
            this.gbProjectileInformation.Controls.Add(this.btnConfirm);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionWindPowerToSelfMultiplier);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionWindPowerToSelfMultiplier);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionDamageToSelfMultiplier);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionDamageToSelfMultiplier);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionDamageAtEdge);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionDamageAtEdge);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionDamageAtCenter);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionDamageAtCenter);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionWindPowerAtEdge);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionWindPowerAtEdge);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionWindPowerAtCenter);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionWindPowerAtCenter);
            this.gbProjectileInformation.Controls.Add(this.lblExplosionRadius);
            this.gbProjectileInformation.Controls.Add(this.txtExplosionRadius);
            this.gbProjectileInformation.Location = new System.Drawing.Point(12, 12);
            this.gbProjectileInformation.Name = "gbProjectileInformation";
            this.gbProjectileInformation.Size = new System.Drawing.Size(311, 374);
            this.gbProjectileInformation.TabIndex = 1;
            this.gbProjectileInformation.TabStop = false;
            this.gbProjectileInformation.Text = "Explosion attributes";
            // 
            // btnUseTextureProjectile
            // 
            this.btnUseTextureProjectile.Location = new System.Drawing.Point(160, 244);
            this.btnUseTextureProjectile.Name = "btnUseTextureProjectile";
            this.btnUseTextureProjectile.Size = new System.Drawing.Size(145, 23);
            this.btnUseTextureProjectile.TabIndex = 65;
            this.btnUseTextureProjectile.Text = "Use Texture Projectile";
            this.btnUseTextureProjectile.UseVisualStyleBackColor = true;
            this.btnUseTextureProjectile.Click += new System.EventHandler(this.btnUseTextureProjectile_Click);
            // 
            // btnUseAnimatedProjectile
            // 
            this.btnUseAnimatedProjectile.Location = new System.Drawing.Point(9, 244);
            this.btnUseAnimatedProjectile.Name = "btnUseAnimatedProjectile";
            this.btnUseAnimatedProjectile.Size = new System.Drawing.Size(145, 23);
            this.btnUseAnimatedProjectile.TabIndex = 64;
            this.btnUseAnimatedProjectile.Text = "Use Animated Projectile";
            this.btnUseAnimatedProjectile.UseVisualStyleBackColor = true;
            this.btnUseAnimatedProjectile.Click += new System.EventHandler(this.btnUseAnimatedProjectile_Click);
            // 
            // txtProjectilePath
            // 
            this.txtProjectilePath.Location = new System.Drawing.Point(9, 218);
            this.txtProjectilePath.Name = "txtProjectilePath";
            this.txtProjectilePath.ReadOnly = true;
            this.txtProjectilePath.Size = new System.Drawing.Size(296, 20);
            this.txtProjectilePath.TabIndex = 63;
            // 
            // lblExplosionPath
            // 
            this.lblExplosionPath.AutoSize = true;
            this.lblExplosionPath.Location = new System.Drawing.Point(9, 202);
            this.lblExplosionPath.Name = "lblExplosionPath";
            this.lblExplosionPath.Size = new System.Drawing.Size(80, 13);
            this.lblExplosionPath.TabIndex = 62;
            this.lblExplosionPath.Text = "Explosion Path:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(230, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 61;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfirm.Location = new System.Drawing.Point(6, 345);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 60;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lblExplosionWindPowerToSelfMultiplier
            // 
            this.lblExplosionWindPowerToSelfMultiplier.AutoSize = true;
            this.lblExplosionWindPowerToSelfMultiplier.Location = new System.Drawing.Point(6, 99);
            this.lblExplosionWindPowerToSelfMultiplier.Name = "lblExplosionWindPowerToSelfMultiplier";
            this.lblExplosionWindPowerToSelfMultiplier.Size = new System.Drawing.Size(194, 13);
            this.lblExplosionWindPowerToSelfMultiplier.TabIndex = 59;
            this.lblExplosionWindPowerToSelfMultiplier.Text = "Explosion Wind Power To Self Multiplier";
            // 
            // txtExplosionWindPowerToSelfMultiplier
            // 
            this.txtExplosionWindPowerToSelfMultiplier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionWindPowerToSelfMultiplier.Location = new System.Drawing.Point(223, 97);
            this.txtExplosionWindPowerToSelfMultiplier.Name = "txtExplosionWindPowerToSelfMultiplier";
            this.txtExplosionWindPowerToSelfMultiplier.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionWindPowerToSelfMultiplier.TabIndex = 58;
            this.txtExplosionWindPowerToSelfMultiplier.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionDamageToSelfMultiplier
            // 
            this.lblExplosionDamageToSelfMultiplier.AutoSize = true;
            this.lblExplosionDamageToSelfMultiplier.Location = new System.Drawing.Point(6, 177);
            this.lblExplosionDamageToSelfMultiplier.Name = "lblExplosionDamageToSelfMultiplier";
            this.lblExplosionDamageToSelfMultiplier.Size = new System.Drawing.Size(176, 13);
            this.lblExplosionDamageToSelfMultiplier.TabIndex = 57;
            this.lblExplosionDamageToSelfMultiplier.Text = "Explosion Damage To Self Multiplier";
            // 
            // txtExplosionDamageToSelfMultiplier
            // 
            this.txtExplosionDamageToSelfMultiplier.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionDamageToSelfMultiplier.Location = new System.Drawing.Point(223, 175);
            this.txtExplosionDamageToSelfMultiplier.Name = "txtExplosionDamageToSelfMultiplier";
            this.txtExplosionDamageToSelfMultiplier.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionDamageToSelfMultiplier.TabIndex = 56;
            this.txtExplosionDamageToSelfMultiplier.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionDamageAtEdge
            // 
            this.lblExplosionDamageAtEdge.AutoSize = true;
            this.lblExplosionDamageAtEdge.Location = new System.Drawing.Point(6, 151);
            this.lblExplosionDamageAtEdge.Name = "lblExplosionDamageAtEdge";
            this.lblExplosionDamageAtEdge.Size = new System.Drawing.Size(136, 13);
            this.lblExplosionDamageAtEdge.TabIndex = 55;
            this.lblExplosionDamageAtEdge.Text = "Explosion Damage At Edge";
            // 
            // txtExplosionDamageAtEdge
            // 
            this.txtExplosionDamageAtEdge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionDamageAtEdge.Location = new System.Drawing.Point(223, 149);
            this.txtExplosionDamageAtEdge.Name = "txtExplosionDamageAtEdge";
            this.txtExplosionDamageAtEdge.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionDamageAtEdge.TabIndex = 54;
            this.txtExplosionDamageAtEdge.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionDamageAtCenter
            // 
            this.lblExplosionDamageAtCenter.AutoSize = true;
            this.lblExplosionDamageAtCenter.Location = new System.Drawing.Point(6, 125);
            this.lblExplosionDamageAtCenter.Name = "lblExplosionDamageAtCenter";
            this.lblExplosionDamageAtCenter.Size = new System.Drawing.Size(142, 13);
            this.lblExplosionDamageAtCenter.TabIndex = 53;
            this.lblExplosionDamageAtCenter.Text = "Explosion Damage At Center";
            // 
            // txtExplosionDamageAtCenter
            // 
            this.txtExplosionDamageAtCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionDamageAtCenter.Location = new System.Drawing.Point(223, 123);
            this.txtExplosionDamageAtCenter.Name = "txtExplosionDamageAtCenter";
            this.txtExplosionDamageAtCenter.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionDamageAtCenter.TabIndex = 52;
            this.txtExplosionDamageAtCenter.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionWindPowerAtEdge
            // 
            this.lblExplosionWindPowerAtEdge.AutoSize = true;
            this.lblExplosionWindPowerAtEdge.Location = new System.Drawing.Point(6, 73);
            this.lblExplosionWindPowerAtEdge.Name = "lblExplosionWindPowerAtEdge";
            this.lblExplosionWindPowerAtEdge.Size = new System.Drawing.Size(154, 13);
            this.lblExplosionWindPowerAtEdge.TabIndex = 51;
            this.lblExplosionWindPowerAtEdge.Text = "Explosion Wind Power At Edge";
            // 
            // txtExplosionWindPowerAtEdge
            // 
            this.txtExplosionWindPowerAtEdge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionWindPowerAtEdge.Location = new System.Drawing.Point(223, 71);
            this.txtExplosionWindPowerAtEdge.Name = "txtExplosionWindPowerAtEdge";
            this.txtExplosionWindPowerAtEdge.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionWindPowerAtEdge.TabIndex = 50;
            this.txtExplosionWindPowerAtEdge.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionWindPowerAtCenter
            // 
            this.lblExplosionWindPowerAtCenter.AutoSize = true;
            this.lblExplosionWindPowerAtCenter.Location = new System.Drawing.Point(6, 47);
            this.lblExplosionWindPowerAtCenter.Name = "lblExplosionWindPowerAtCenter";
            this.lblExplosionWindPowerAtCenter.Size = new System.Drawing.Size(160, 13);
            this.lblExplosionWindPowerAtCenter.TabIndex = 49;
            this.lblExplosionWindPowerAtCenter.Text = "Explosion Wind Power At Center";
            // 
            // txtExplosionWindPowerAtCenter
            // 
            this.txtExplosionWindPowerAtCenter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionWindPowerAtCenter.Location = new System.Drawing.Point(223, 45);
            this.txtExplosionWindPowerAtCenter.Name = "txtExplosionWindPowerAtCenter";
            this.txtExplosionWindPowerAtCenter.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionWindPowerAtCenter.TabIndex = 48;
            this.txtExplosionWindPowerAtCenter.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // lblExplosionRadius
            // 
            this.lblExplosionRadius.AutoSize = true;
            this.lblExplosionRadius.Location = new System.Drawing.Point(6, 21);
            this.lblExplosionRadius.Name = "lblExplosionRadius";
            this.lblExplosionRadius.Size = new System.Drawing.Size(88, 13);
            this.lblExplosionRadius.TabIndex = 47;
            this.lblExplosionRadius.Text = "Explosion Radius";
            // 
            // txtExplosionRadius
            // 
            this.txtExplosionRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExplosionRadius.Location = new System.Drawing.Point(223, 19);
            this.txtExplosionRadius.Name = "txtExplosionRadius";
            this.txtExplosionRadius.Size = new System.Drawing.Size(82, 20);
            this.txtExplosionRadius.TabIndex = 46;
            this.txtExplosionRadius.ValueChanged += new System.EventHandler(this.txtExplosionAttributes_ValueChanged);
            // 
            // txtSoundPath
            // 
            this.txtSoundPath.Location = new System.Drawing.Point(9, 286);
            this.txtSoundPath.Name = "txtSoundPath";
            this.txtSoundPath.ReadOnly = true;
            this.txtSoundPath.Size = new System.Drawing.Size(296, 20);
            this.txtSoundPath.TabIndex = 67;
            // 
            // lblSoundPath
            // 
            this.lblSoundPath.AutoSize = true;
            this.lblSoundPath.Location = new System.Drawing.Point(9, 270);
            this.lblSoundPath.Name = "lblSoundPath";
            this.lblSoundPath.Size = new System.Drawing.Size(66, 13);
            this.lblSoundPath.TabIndex = 66;
            this.lblSoundPath.Text = "Sound Path:";
            // 
            // btnSelectSound
            // 
            this.btnSelectSound.Location = new System.Drawing.Point(9, 312);
            this.btnSelectSound.Name = "btnSelectSound";
            this.btnSelectSound.Size = new System.Drawing.Size(145, 23);
            this.btnSelectSound.TabIndex = 68;
            this.btnSelectSound.Text = "Select Sound";
            this.btnSelectSound.UseVisualStyleBackColor = true;
            this.btnSelectSound.Click += new System.EventHandler(this.btnSelectSound_Click);
            // 
            // ExplosionAttributesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 398);
            this.Controls.Add(this.gbProjectileInformation);
            this.Name = "ExplosionAttributesEditor";
            this.Text = "Explosion Attributes Editor";
            this.gbProjectileInformation.ResumeLayout(false);
            this.gbProjectileInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerToSelfMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageToSelfMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageAtEdge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionDamageAtCenter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerAtEdge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionWindPowerAtCenter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExplosionRadius)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbProjectileInformation;
        private System.Windows.Forms.Label lblExplosionRadius;
        private System.Windows.Forms.Label lblExplosionWindPowerAtCenter;
        private System.Windows.Forms.Label lblExplosionDamageAtEdge;
        private System.Windows.Forms.Label lblExplosionDamageAtCenter;
        private System.Windows.Forms.Label lblExplosionWindPowerAtEdge;
        private System.Windows.Forms.Label lblExplosionDamageToSelfMultiplier;
        private System.Windows.Forms.Label lblExplosionWindPowerToSelfMultiplier;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnUseTextureProjectile;
        private System.Windows.Forms.Button btnUseAnimatedProjectile;
        private System.Windows.Forms.Label lblExplosionPath;
        private System.Windows.Forms.NumericUpDown txtExplosionRadius;
        private System.Windows.Forms.NumericUpDown txtExplosionWindPowerAtCenter;
        private System.Windows.Forms.NumericUpDown txtExplosionDamageAtEdge;
        private System.Windows.Forms.NumericUpDown txtExplosionDamageAtCenter;
        private System.Windows.Forms.NumericUpDown txtExplosionWindPowerAtEdge;
        private System.Windows.Forms.NumericUpDown txtExplosionDamageToSelfMultiplier;
        private System.Windows.Forms.NumericUpDown txtExplosionWindPowerToSelfMultiplier;
        private System.Windows.Forms.TextBox txtProjectilePath;
        private System.Windows.Forms.TextBox txtSoundPath;
        private System.Windows.Forms.Label lblSoundPath;
        private System.Windows.Forms.Button btnSelectSound;
    }
}
