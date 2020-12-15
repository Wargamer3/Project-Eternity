namespace ProjectEternity.Editors.UnitNormalEditor
{
    partial class Attacks
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
            this.gbAttacks = new System.Windows.Forms.GroupBox();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnCreateAttack = new System.Windows.Forms.Button();
            this.btnRemoveAttack = new System.Windows.Forms.Button();
            this.btnAddAttack = new System.Windows.Forms.Button();
            this.lstAttack = new System.Windows.Forms.ListBox();
            this.gbAnimations = new System.Windows.Forms.GroupBox();
            this.btnSelectAnimation = new System.Windows.Forms.Button();
            this.txtAnimationName = new System.Windows.Forms.TextBox();
            this.lblAnimationPath = new System.Windows.Forms.Label();
            this.lstAttackAnimations = new System.Windows.Forms.ListBox();
            this.gbUpgrades = new System.Windows.Forms.GroupBox();
            this.lblUpgradeCost = new System.Windows.Forms.Label();
            this.cbUpgradeCost = new System.Windows.Forms.ComboBox();
            this.lblUpgradeValues = new System.Windows.Forms.Label();
            this.cbUpgradeValues = new System.Windows.Forms.ComboBox();
            this.gbAttacks.SuspendLayout();
            this.gbAnimations.SuspendLayout();
            this.gbUpgrades.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbAttacks
            // 
            this.gbAttacks.Controls.Add(this.btnMoveDown);
            this.gbAttacks.Controls.Add(this.btnMoveUp);
            this.gbAttacks.Controls.Add(this.btnCreateAttack);
            this.gbAttacks.Controls.Add(this.btnRemoveAttack);
            this.gbAttacks.Controls.Add(this.btnAddAttack);
            this.gbAttacks.Controls.Add(this.lstAttack);
            this.gbAttacks.Location = new System.Drawing.Point(12, 12);
            this.gbAttacks.Name = "gbAttacks";
            this.gbAttacks.Size = new System.Drawing.Size(141, 317);
            this.gbAttacks.TabIndex = 0;
            this.gbAttacks.TabStop = false;
            this.gbAttacks.Text = "Attacks";
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(6, 259);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(129, 23);
            this.btnMoveDown.TabIndex = 11;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(6, 230);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(129, 23);
            this.btnMoveUp.TabIndex = 10;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnCreateAttack
            // 
            this.btnCreateAttack.Location = new System.Drawing.Point(6, 172);
            this.btnCreateAttack.Name = "btnCreateAttack";
            this.btnCreateAttack.Size = new System.Drawing.Size(129, 23);
            this.btnCreateAttack.TabIndex = 9;
            this.btnCreateAttack.Text = "Create Attack";
            this.btnCreateAttack.UseVisualStyleBackColor = true;
            this.btnCreateAttack.Click += new System.EventHandler(this.btnCreateAttack_Click);
            // 
            // btnRemoveAttack
            // 
            this.btnRemoveAttack.Location = new System.Drawing.Point(6, 201);
            this.btnRemoveAttack.Name = "btnRemoveAttack";
            this.btnRemoveAttack.Size = new System.Drawing.Size(129, 23);
            this.btnRemoveAttack.TabIndex = 8;
            this.btnRemoveAttack.Text = "Remove Attack";
            this.btnRemoveAttack.UseVisualStyleBackColor = true;
            this.btnRemoveAttack.Click += new System.EventHandler(this.btnRemoveAttack_Click);
            // 
            // btnAddAttack
            // 
            this.btnAddAttack.Location = new System.Drawing.Point(6, 288);
            this.btnAddAttack.Name = "btnAddAttack";
            this.btnAddAttack.Size = new System.Drawing.Size(129, 23);
            this.btnAddAttack.TabIndex = 7;
            this.btnAddAttack.Text = "Add Attack";
            this.btnAddAttack.UseVisualStyleBackColor = true;
            this.btnAddAttack.Click += new System.EventHandler(this.btnAddAttack_Click);
            // 
            // lstAttack
            // 
            this.lstAttack.Location = new System.Drawing.Point(6, 19);
            this.lstAttack.Name = "lstAttack";
            this.lstAttack.Size = new System.Drawing.Size(129, 147);
            this.lstAttack.TabIndex = 0;
            this.lstAttack.SelectedIndexChanged += new System.EventHandler(this.lstAttack_SelectedIndexChanged);
            this.lstAttack.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstAttack_MouseDoubleClick);
            // 
            // gbAnimations
            // 
            this.gbAnimations.Controls.Add(this.btnSelectAnimation);
            this.gbAnimations.Controls.Add(this.txtAnimationName);
            this.gbAnimations.Controls.Add(this.lblAnimationPath);
            this.gbAnimations.Controls.Add(this.lstAttackAnimations);
            this.gbAnimations.Location = new System.Drawing.Point(159, 12);
            this.gbAnimations.Name = "gbAnimations";
            this.gbAnimations.Size = new System.Drawing.Size(223, 230);
            this.gbAnimations.TabIndex = 1;
            this.gbAnimations.TabStop = false;
            this.gbAnimations.Text = "Animations";
            // 
            // btnSelectAnimation
            // 
            this.btnSelectAnimation.Location = new System.Drawing.Point(109, 175);
            this.btnSelectAnimation.Name = "btnSelectAnimation";
            this.btnSelectAnimation.Size = new System.Drawing.Size(108, 23);
            this.btnSelectAnimation.TabIndex = 7;
            this.btnSelectAnimation.Text = "Select Animation";
            this.btnSelectAnimation.UseVisualStyleBackColor = true;
            this.btnSelectAnimation.Click += new System.EventHandler(this.btnSelectAnimation_Click);
            // 
            // txtAnimationName
            // 
            this.txtAnimationName.Location = new System.Drawing.Point(6, 204);
            this.txtAnimationName.Name = "txtAnimationName";
            this.txtAnimationName.ReadOnly = true;
            this.txtAnimationName.Size = new System.Drawing.Size(211, 20);
            this.txtAnimationName.TabIndex = 6;
            // 
            // lblAnimationPath
            // 
            this.lblAnimationPath.AutoSize = true;
            this.lblAnimationPath.Location = new System.Drawing.Point(6, 188);
            this.lblAnimationPath.Name = "lblAnimationPath";
            this.lblAnimationPath.Size = new System.Drawing.Size(81, 13);
            this.lblAnimationPath.TabIndex = 5;
            this.lblAnimationPath.Text = "Animation Path:";
            // 
            // lstAttackAnimations
            // 
            this.lstAttackAnimations.FormattingEnabled = true;
            this.lstAttackAnimations.Items.AddRange(new object[] {
            "Start",
            "End Hit",
            "End Miss",
            "End Destroyed",
            "End Blocked",
            "End Parried",
            "End Shoot Down",
            "End Negated"});
            this.lstAttackAnimations.Location = new System.Drawing.Point(6, 19);
            this.lstAttackAnimations.Name = "lstAttackAnimations";
            this.lstAttackAnimations.Size = new System.Drawing.Size(211, 147);
            this.lstAttackAnimations.TabIndex = 1;
            this.lstAttackAnimations.SelectedIndexChanged += new System.EventHandler(this.lstAttackAnimations_SelectedIndexChanged);
            // 
            // gbUpgrades
            // 
            this.gbUpgrades.Controls.Add(this.lblUpgradeCost);
            this.gbUpgrades.Controls.Add(this.cbUpgradeCost);
            this.gbUpgrades.Controls.Add(this.lblUpgradeValues);
            this.gbUpgrades.Controls.Add(this.cbUpgradeValues);
            this.gbUpgrades.Location = new System.Drawing.Point(388, 12);
            this.gbUpgrades.Name = "gbUpgrades";
            this.gbUpgrades.Size = new System.Drawing.Size(155, 230);
            this.gbUpgrades.TabIndex = 2;
            this.gbUpgrades.TabStop = false;
            this.gbUpgrades.Text = "Upgrades";
            // 
            // lblUpgradeCost
            // 
            this.lblUpgradeCost.AutoSize = true;
            this.lblUpgradeCost.Location = new System.Drawing.Point(6, 56);
            this.lblUpgradeCost.Name = "lblUpgradeCost";
            this.lblUpgradeCost.Size = new System.Drawing.Size(72, 13);
            this.lblUpgradeCost.TabIndex = 3;
            this.lblUpgradeCost.Text = "Upgrade Cost";
            // 
            // cbUpgradeCost
            // 
            this.cbUpgradeCost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpgradeCost.FormattingEnabled = true;
            this.cbUpgradeCost.Items.AddRange(new object[] {
            "Cheapest",
            "Cheap",
            "Normal",
            "Expensive"});
            this.cbUpgradeCost.Location = new System.Drawing.Point(6, 72);
            this.cbUpgradeCost.Name = "cbUpgradeCost";
            this.cbUpgradeCost.Size = new System.Drawing.Size(143, 21);
            this.cbUpgradeCost.TabIndex = 2;
            this.cbUpgradeCost.SelectedIndexChanged += new System.EventHandler(this.cbUpgradeCost_SelectedIndexChanged);
            // 
            // lblUpgradeValues
            // 
            this.lblUpgradeValues.AutoSize = true;
            this.lblUpgradeValues.Location = new System.Drawing.Point(6, 16);
            this.lblUpgradeValues.Name = "lblUpgradeValues";
            this.lblUpgradeValues.Size = new System.Drawing.Size(83, 13);
            this.lblUpgradeValues.TabIndex = 1;
            this.lblUpgradeValues.Text = "Upgrade Values";
            // 
            // cbUpgradeValues
            // 
            this.cbUpgradeValues.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpgradeValues.FormattingEnabled = true;
            this.cbUpgradeValues.Items.AddRange(new object[] {
            "Very Slow",
            "Slow",
            "Normal",
            "Fast"});
            this.cbUpgradeValues.Location = new System.Drawing.Point(6, 32);
            this.cbUpgradeValues.Name = "cbUpgradeValues";
            this.cbUpgradeValues.Size = new System.Drawing.Size(143, 21);
            this.cbUpgradeValues.TabIndex = 0;
            this.cbUpgradeValues.SelectedIndexChanged += new System.EventHandler(this.cbUpgradeValues_SelectedIndexChanged);
            // 
            // Attacks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 341);
            this.Controls.Add(this.gbUpgrades);
            this.Controls.Add(this.gbAnimations);
            this.Controls.Add(this.gbAttacks);
            this.Name = "Attacks";
            this.Text = "Attacks";
            this.gbAttacks.ResumeLayout(false);
            this.gbAnimations.ResumeLayout(false);
            this.gbAnimations.PerformLayout();
            this.gbUpgrades.ResumeLayout(false);
            this.gbUpgrades.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAttacks;
        private System.Windows.Forms.GroupBox gbAnimations;
        private System.Windows.Forms.ListBox lstAttackAnimations;
        private System.Windows.Forms.TextBox txtAnimationName;
        private System.Windows.Forms.Label lblAnimationPath;
        private System.Windows.Forms.Button btnRemoveAttack;
        private System.Windows.Forms.Button btnAddAttack;
        private System.Windows.Forms.Button btnSelectAnimation;
        private System.Windows.Forms.GroupBox gbUpgrades;
        private System.Windows.Forms.Label lblUpgradeCost;
        private System.Windows.Forms.Label lblUpgradeValues;
        public System.Windows.Forms.ComboBox cbUpgradeCost;
        public System.Windows.Forms.ComboBox cbUpgradeValues;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnCreateAttack;
        public System.Windows.Forms.ListBox lstAttack;
    }
}