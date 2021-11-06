namespace ProjectEternity.Editors.UnitTripleThunderEditor
{
    partial class UnitTripleThunderEditor
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
            this.gbUnitStats = new System.Windows.Forms.GroupBox();
            this.lblJumpSpeed = new System.Windows.Forms.Label();
            this.txtJumpSpeed = new System.Windows.Forms.NumericUpDown();
            this.cbIsDynamic = new System.Windows.Forms.CheckBox();
            this.cbHasKnockback = new System.Windows.Forms.CheckBox();
            this.lblTopSpeed = new System.Windows.Forms.Label();
            this.txtTopSpeed = new System.Windows.Forms.NumericUpDown();
            this.lblAccel = new System.Windows.Forms.Label();
            this.txtAcceleration = new System.Windows.Forms.NumericUpDown();
            this.lblMaxEN = new System.Windows.Forms.Label();
            this.txtMaxEN = new System.Windows.Forms.NumericUpDown();
            this.lblMaxHP = new System.Windows.Forms.Label();
            this.txtMaxHP = new System.Windows.Forms.NumericUpDown();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSounds = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnAddWeapon = new System.Windows.Forms.Button();
            this.btnRemoveWeapon = new System.Windows.Forms.Button();
            this.lstWeapons = new System.Windows.Forms.ListBox();
            this.gbUnitStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtJumpSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTopSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxEN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxHP)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUnitStats
            // 
            this.gbUnitStats.Controls.Add(this.lblJumpSpeed);
            this.gbUnitStats.Controls.Add(this.txtJumpSpeed);
            this.gbUnitStats.Controls.Add(this.cbIsDynamic);
            this.gbUnitStats.Controls.Add(this.cbHasKnockback);
            this.gbUnitStats.Controls.Add(this.lblTopSpeed);
            this.gbUnitStats.Controls.Add(this.txtTopSpeed);
            this.gbUnitStats.Controls.Add(this.lblAccel);
            this.gbUnitStats.Controls.Add(this.txtAcceleration);
            this.gbUnitStats.Controls.Add(this.lblMaxEN);
            this.gbUnitStats.Controls.Add(this.txtMaxEN);
            this.gbUnitStats.Controls.Add(this.lblMaxHP);
            this.gbUnitStats.Controls.Add(this.txtMaxHP);
            this.gbUnitStats.Location = new System.Drawing.Point(12, 27);
            this.gbUnitStats.Name = "gbUnitStats";
            this.gbUnitStats.Size = new System.Drawing.Size(322, 178);
            this.gbUnitStats.TabIndex = 2;
            this.gbUnitStats.TabStop = false;
            this.gbUnitStats.Text = "Unit Stats";
            // 
            // lblJumpSpeed
            // 
            this.lblJumpSpeed.AutoSize = true;
            this.lblJumpSpeed.Location = new System.Drawing.Point(6, 125);
            this.lblJumpSpeed.Name = "lblJumpSpeed";
            this.lblJumpSpeed.Size = new System.Drawing.Size(66, 13);
            this.lblJumpSpeed.TabIndex = 11;
            this.lblJumpSpeed.Text = "Jump Speed";
            // 
            // txtJumpSpeed
            // 
            this.txtJumpSpeed.DecimalPlaces = 2;
            this.txtJumpSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.txtJumpSpeed.Location = new System.Drawing.Point(144, 123);
            this.txtJumpSpeed.Name = "txtJumpSpeed";
            this.txtJumpSpeed.Size = new System.Drawing.Size(120, 20);
            this.txtJumpSpeed.TabIndex = 10;
            // 
            // cbIsDynamic
            // 
            this.cbIsDynamic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbIsDynamic.AutoSize = true;
            this.cbIsDynamic.Checked = true;
            this.cbIsDynamic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsDynamic.Location = new System.Drawing.Point(144, 155);
            this.cbIsDynamic.Name = "cbIsDynamic";
            this.cbIsDynamic.Size = new System.Drawing.Size(78, 17);
            this.cbIsDynamic.TabIndex = 9;
            this.cbIsDynamic.Text = "Is Dynamic";
            this.cbIsDynamic.UseVisualStyleBackColor = true;
            // 
            // cbHasKnockback
            // 
            this.cbHasKnockback.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbHasKnockback.AutoSize = true;
            this.cbHasKnockback.Location = new System.Drawing.Point(6, 155);
            this.cbHasKnockback.Name = "cbHasKnockback";
            this.cbHasKnockback.Size = new System.Drawing.Size(102, 17);
            this.cbHasKnockback.TabIndex = 8;
            this.cbHasKnockback.Text = "Has knockback";
            this.cbHasKnockback.UseVisualStyleBackColor = true;
            // 
            // lblTopSpeed
            // 
            this.lblTopSpeed.AutoSize = true;
            this.lblTopSpeed.Location = new System.Drawing.Point(6, 99);
            this.lblTopSpeed.Name = "lblTopSpeed";
            this.lblTopSpeed.Size = new System.Drawing.Size(60, 13);
            this.lblTopSpeed.TabIndex = 7;
            this.lblTopSpeed.Text = "Top Speed";
            // 
            // txtTopSpeed
            // 
            this.txtTopSpeed.DecimalPlaces = 2;
            this.txtTopSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.txtTopSpeed.Location = new System.Drawing.Point(144, 97);
            this.txtTopSpeed.Name = "txtTopSpeed";
            this.txtTopSpeed.Size = new System.Drawing.Size(120, 20);
            this.txtTopSpeed.TabIndex = 6;
            // 
            // lblAccel
            // 
            this.lblAccel.AutoSize = true;
            this.lblAccel.Location = new System.Drawing.Point(6, 73);
            this.lblAccel.Name = "lblAccel";
            this.lblAccel.Size = new System.Drawing.Size(66, 13);
            this.lblAccel.TabIndex = 5;
            this.lblAccel.Text = "Acceleration";
            // 
            // txtAcceleration
            // 
            this.txtAcceleration.DecimalPlaces = 2;
            this.txtAcceleration.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.txtAcceleration.Location = new System.Drawing.Point(144, 71);
            this.txtAcceleration.Name = "txtAcceleration";
            this.txtAcceleration.Size = new System.Drawing.Size(120, 20);
            this.txtAcceleration.TabIndex = 4;
            // 
            // lblMaxEN
            // 
            this.lblMaxEN.AutoSize = true;
            this.lblMaxEN.Location = new System.Drawing.Point(6, 47);
            this.lblMaxEN.Name = "lblMaxEN";
            this.lblMaxEN.Size = new System.Drawing.Size(45, 13);
            this.lblMaxEN.TabIndex = 3;
            this.lblMaxEN.Text = "Max EN";
            // 
            // txtMaxEN
            // 
            this.txtMaxEN.Location = new System.Drawing.Point(144, 45);
            this.txtMaxEN.Name = "txtMaxEN";
            this.txtMaxEN.Size = new System.Drawing.Size(120, 20);
            this.txtMaxEN.TabIndex = 2;
            // 
            // lblMaxHP
            // 
            this.lblMaxHP.AutoSize = true;
            this.lblMaxHP.Location = new System.Drawing.Point(6, 21);
            this.lblMaxHP.Name = "lblMaxHP";
            this.lblMaxHP.Size = new System.Drawing.Size(45, 13);
            this.lblMaxHP.TabIndex = 1;
            this.lblMaxHP.Text = "Max HP";
            // 
            // txtMaxHP
            // 
            this.txtMaxHP.Location = new System.Drawing.Point(144, 19);
            this.txtMaxHP.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtMaxHP.Name = "txtMaxHP";
            this.txtMaxHP.Size = new System.Drawing.Size(120, 20);
            this.txtMaxHP.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmSounds});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(504, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmSounds
            // 
            this.tsmSounds.Name = "tsmSounds";
            this.tsmSounds.Size = new System.Drawing.Size(58, 20);
            this.tsmSounds.Text = "Sounds";
            this.tsmSounds.Click += new System.EventHandler(this.tsmSounds_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btnAddWeapon);
            this.groupBox3.Controls.Add(this.btnRemoveWeapon);
            this.groupBox3.Controls.Add(this.lstWeapons);
            this.groupBox3.Location = new System.Drawing.Point(340, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(154, 178);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Weapons";
            // 
            // btnAddWeapon
            // 
            this.btnAddWeapon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddWeapon.Location = new System.Drawing.Point(6, 116);
            this.btnAddWeapon.Name = "btnAddWeapon";
            this.btnAddWeapon.Size = new System.Drawing.Size(142, 23);
            this.btnAddWeapon.TabIndex = 3;
            this.btnAddWeapon.Text = "Add Weapon";
            this.btnAddWeapon.UseVisualStyleBackColor = true;
            this.btnAddWeapon.Click += new System.EventHandler(this.btnAddWeapon_Click);
            // 
            // btnRemoveWeapon
            // 
            this.btnRemoveWeapon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveWeapon.Location = new System.Drawing.Point(6, 145);
            this.btnRemoveWeapon.Name = "btnRemoveWeapon";
            this.btnRemoveWeapon.Size = new System.Drawing.Size(142, 23);
            this.btnRemoveWeapon.TabIndex = 2;
            this.btnRemoveWeapon.Text = "RemoveWeapon";
            this.btnRemoveWeapon.UseVisualStyleBackColor = true;
            this.btnRemoveWeapon.Click += new System.EventHandler(this.btnRemoveWeapon_Click);
            // 
            // lstWeapons
            // 
            this.lstWeapons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstWeapons.FormattingEnabled = true;
            this.lstWeapons.Location = new System.Drawing.Point(6, 19);
            this.lstWeapons.Name = "lstWeapons";
            this.lstWeapons.Size = new System.Drawing.Size(142, 69);
            this.lstWeapons.TabIndex = 0;
            // 
            // UnitTripleThunderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 217);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbUnitStats);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitTripleThunderEditor";
            this.Text = "Triple Thunder Unit Editor";
            this.gbUnitStats.ResumeLayout(false);
            this.gbUnitStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtJumpSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTopSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAcceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxEN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxHP)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbUnitStats;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lstWeapons;
        private System.Windows.Forms.Button btnAddWeapon;
        private System.Windows.Forms.Button btnRemoveWeapon;
        private System.Windows.Forms.Label lblMaxHP;
        private System.Windows.Forms.NumericUpDown txtMaxHP;
        private System.Windows.Forms.Label lblMaxEN;
        private System.Windows.Forms.NumericUpDown txtMaxEN;
        private System.Windows.Forms.Label lblTopSpeed;
        private System.Windows.Forms.NumericUpDown txtTopSpeed;
        private System.Windows.Forms.Label lblAccel;
        private System.Windows.Forms.NumericUpDown txtAcceleration;
        private System.Windows.Forms.CheckBox cbHasKnockback;
        private System.Windows.Forms.CheckBox cbIsDynamic;
        private System.Windows.Forms.Label lblJumpSpeed;
        private System.Windows.Forms.NumericUpDown txtJumpSpeed;
        private System.Windows.Forms.ToolStripMenuItem tsmSounds;
    }
}

