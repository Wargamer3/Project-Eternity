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
            this.gbAttackOriginContext = new System.Windows.Forms.GroupBox();
            this.txtAttackOriginCustomType = new System.Windows.Forms.TextBox();
            this.lblAttackOriginCustomType = new System.Windows.Forms.Label();
            this.lblAttackOriginType = new System.Windows.Forms.Label();
            this.cbAttackOriginType = new System.Windows.Forms.ComboBox();
            this.gbAttackTargetContext = new System.Windows.Forms.GroupBox();
            this.txtAttackTargetCustomType = new System.Windows.Forms.TextBox();
            this.lblAttackTargetCustomType = new System.Windows.Forms.Label();
            this.lblAttackTargetType = new System.Windows.Forms.Label();
            this.cbAttackTargetType = new System.Windows.Forms.ComboBox();
            this.gbAttackContexts = new System.Windows.Forms.GroupBox();
            this.txtAttackContextWeight = new System.Windows.Forms.NumericUpDown();
            this.lblAttackContextWeight = new System.Windows.Forms.Label();
            this.txtAttackContextParserValue = new System.Windows.Forms.TextBox();
            this.btnRemoveAttackContext = new System.Windows.Forms.Button();
            this.lblAttackContextParserValue = new System.Windows.Forms.Label();
            this.txtAttackContextName = new System.Windows.Forms.TextBox();
            this.lblAttackContextName = new System.Windows.Forms.Label();
            this.btnAddAttackContext = new System.Windows.Forms.Button();
            this.lstAttackContexts = new System.Windows.Forms.ListBox();
            this.gbAttackVisibility = new System.Windows.Forms.GroupBox();
            this.txtAttackVisibility = new System.Windows.Forms.TextBox();
            this.lblAttackVisibility = new System.Windows.Forms.Label();
            this.gbAttacks.SuspendLayout();
            this.gbAnimations.SuspendLayout();
            this.gbUpgrades.SuspendLayout();
            this.gbAttackOriginContext.SuspendLayout();
            this.gbAttackTargetContext.SuspendLayout();
            this.gbAttackContexts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackContextWeight)).BeginInit();
            this.gbAttackVisibility.SuspendLayout();
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
            this.gbAnimations.Enabled = false;
            this.gbAnimations.Location = new System.Drawing.Point(481, 12);
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
            this.gbUpgrades.Location = new System.Drawing.Point(710, 12);
            this.gbUpgrades.Name = "gbUpgrades";
            this.gbUpgrades.Size = new System.Drawing.Size(155, 113);
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
            // gbAttackOriginContext
            // 
            this.gbAttackOriginContext.Controls.Add(this.txtAttackOriginCustomType);
            this.gbAttackOriginContext.Controls.Add(this.lblAttackOriginCustomType);
            this.gbAttackOriginContext.Controls.Add(this.lblAttackOriginType);
            this.gbAttackOriginContext.Controls.Add(this.cbAttackOriginType);
            this.gbAttackOriginContext.Enabled = false;
            this.gbAttackOriginContext.Location = new System.Drawing.Point(326, 12);
            this.gbAttackOriginContext.Name = "gbAttackOriginContext";
            this.gbAttackOriginContext.Size = new System.Drawing.Size(149, 137);
            this.gbAttackOriginContext.TabIndex = 3;
            this.gbAttackOriginContext.TabStop = false;
            this.gbAttackOriginContext.Text = "Attack Origin Context";
            // 
            // txtAttackOriginCustomType
            // 
            this.txtAttackOriginCustomType.Location = new System.Drawing.Point(6, 72);
            this.txtAttackOriginCustomType.Name = "txtAttackOriginCustomType";
            this.txtAttackOriginCustomType.ReadOnly = true;
            this.txtAttackOriginCustomType.Size = new System.Drawing.Size(137, 20);
            this.txtAttackOriginCustomType.TabIndex = 8;
            this.txtAttackOriginCustomType.TextChanged += new System.EventHandler(this.txtAttackOriginCustomType_TextChanged);
            // 
            // lblAttackOriginCustomType
            // 
            this.lblAttackOriginCustomType.AutoSize = true;
            this.lblAttackOriginCustomType.Location = new System.Drawing.Point(6, 56);
            this.lblAttackOriginCustomType.Name = "lblAttackOriginCustomType";
            this.lblAttackOriginCustomType.Size = new System.Drawing.Size(72, 13);
            this.lblAttackOriginCustomType.TabIndex = 7;
            this.lblAttackOriginCustomType.Text = "Custom Type:";
            // 
            // lblAttackOriginType
            // 
            this.lblAttackOriginType.AutoSize = true;
            this.lblAttackOriginType.Location = new System.Drawing.Point(6, 16);
            this.lblAttackOriginType.Name = "lblAttackOriginType";
            this.lblAttackOriginType.Size = new System.Drawing.Size(31, 13);
            this.lblAttackOriginType.TabIndex = 3;
            this.lblAttackOriginType.Text = "Type";
            // 
            // cbAttackOriginType
            // 
            this.cbAttackOriginType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttackOriginType.FormattingEnabled = true;
            this.cbAttackOriginType.Items.AddRange(new object[] {
            "Any",
            "Air",
            "Land",
            "Sea",
            "Space",
            "Custom"});
            this.cbAttackOriginType.Location = new System.Drawing.Point(6, 32);
            this.cbAttackOriginType.Name = "cbAttackOriginType";
            this.cbAttackOriginType.Size = new System.Drawing.Size(137, 21);
            this.cbAttackOriginType.TabIndex = 2;
            this.cbAttackOriginType.SelectedIndexChanged += new System.EventHandler(this.cbAttackOriginType_SelectedIndexChanged);
            // 
            // gbAttackTargetContext
            // 
            this.gbAttackTargetContext.Controls.Add(this.txtAttackTargetCustomType);
            this.gbAttackTargetContext.Controls.Add(this.lblAttackTargetCustomType);
            this.gbAttackTargetContext.Controls.Add(this.lblAttackTargetType);
            this.gbAttackTargetContext.Controls.Add(this.cbAttackTargetType);
            this.gbAttackTargetContext.Enabled = false;
            this.gbAttackTargetContext.Location = new System.Drawing.Point(326, 155);
            this.gbAttackTargetContext.Name = "gbAttackTargetContext";
            this.gbAttackTargetContext.Size = new System.Drawing.Size(149, 137);
            this.gbAttackTargetContext.TabIndex = 9;
            this.gbAttackTargetContext.TabStop = false;
            this.gbAttackTargetContext.Text = "Attack Target Context";
            // 
            // txtAttackTargetCustomType
            // 
            this.txtAttackTargetCustomType.Location = new System.Drawing.Point(6, 72);
            this.txtAttackTargetCustomType.Name = "txtAttackTargetCustomType";
            this.txtAttackTargetCustomType.ReadOnly = true;
            this.txtAttackTargetCustomType.Size = new System.Drawing.Size(137, 20);
            this.txtAttackTargetCustomType.TabIndex = 8;
            this.txtAttackTargetCustomType.TextChanged += new System.EventHandler(this.txtAttackTargetCustomType_TextChanged);
            // 
            // lblAttackTargetCustomType
            // 
            this.lblAttackTargetCustomType.AutoSize = true;
            this.lblAttackTargetCustomType.Location = new System.Drawing.Point(6, 56);
            this.lblAttackTargetCustomType.Name = "lblAttackTargetCustomType";
            this.lblAttackTargetCustomType.Size = new System.Drawing.Size(72, 13);
            this.lblAttackTargetCustomType.TabIndex = 7;
            this.lblAttackTargetCustomType.Text = "Custom Type:";
            // 
            // lblAttackTargetType
            // 
            this.lblAttackTargetType.AutoSize = true;
            this.lblAttackTargetType.Location = new System.Drawing.Point(6, 16);
            this.lblAttackTargetType.Name = "lblAttackTargetType";
            this.lblAttackTargetType.Size = new System.Drawing.Size(31, 13);
            this.lblAttackTargetType.TabIndex = 3;
            this.lblAttackTargetType.Text = "Type";
            // 
            // cbAttackTargetType
            // 
            this.cbAttackTargetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttackTargetType.FormattingEnabled = true;
            this.cbAttackTargetType.Items.AddRange(new object[] {
            "Any",
            "Air",
            "Land",
            "Sea",
            "Space",
            "Custom"});
            this.cbAttackTargetType.Location = new System.Drawing.Point(6, 32);
            this.cbAttackTargetType.Name = "cbAttackTargetType";
            this.cbAttackTargetType.Size = new System.Drawing.Size(137, 21);
            this.cbAttackTargetType.TabIndex = 2;
            this.cbAttackTargetType.SelectedIndexChanged += new System.EventHandler(this.cbAttackTargetType_SelectedIndexChanged);
            // 
            // gbAttackContexts
            // 
            this.gbAttackContexts.Controls.Add(this.txtAttackContextWeight);
            this.gbAttackContexts.Controls.Add(this.lblAttackContextWeight);
            this.gbAttackContexts.Controls.Add(this.txtAttackContextParserValue);
            this.gbAttackContexts.Controls.Add(this.btnRemoveAttackContext);
            this.gbAttackContexts.Controls.Add(this.lblAttackContextParserValue);
            this.gbAttackContexts.Controls.Add(this.txtAttackContextName);
            this.gbAttackContexts.Controls.Add(this.lblAttackContextName);
            this.gbAttackContexts.Controls.Add(this.btnAddAttackContext);
            this.gbAttackContexts.Controls.Add(this.lstAttackContexts);
            this.gbAttackContexts.Enabled = false;
            this.gbAttackContexts.Location = new System.Drawing.Point(159, 12);
            this.gbAttackContexts.Name = "gbAttackContexts";
            this.gbAttackContexts.Size = new System.Drawing.Size(161, 317);
            this.gbAttackContexts.TabIndex = 8;
            this.gbAttackContexts.TabStop = false;
            this.gbAttackContexts.Text = "Animation Contexts";
            // 
            // txtAttackContextWeight
            // 
            this.txtAttackContextWeight.Location = new System.Drawing.Point(76, 292);
            this.txtAttackContextWeight.Name = "txtAttackContextWeight";
            this.txtAttackContextWeight.Size = new System.Drawing.Size(79, 20);
            this.txtAttackContextWeight.TabIndex = 14;
            this.txtAttackContextWeight.ValueChanged += new System.EventHandler(this.txtAttackContextWeight_ValueChanged);
            // 
            // lblAttackContextWeight
            // 
            this.lblAttackContextWeight.AutoSize = true;
            this.lblAttackContextWeight.Location = new System.Drawing.Point(3, 276);
            this.lblAttackContextWeight.Name = "lblAttackContextWeight";
            this.lblAttackContextWeight.Size = new System.Drawing.Size(44, 13);
            this.lblAttackContextWeight.TabIndex = 13;
            this.lblAttackContextWeight.Text = "Weight:";
            // 
            // txtAttackContextParserValue
            // 
            this.txtAttackContextParserValue.Location = new System.Drawing.Point(6, 253);
            this.txtAttackContextParserValue.Name = "txtAttackContextParserValue";
            this.txtAttackContextParserValue.Size = new System.Drawing.Size(149, 20);
            this.txtAttackContextParserValue.TabIndex = 12;
            this.txtAttackContextParserValue.TextChanged += new System.EventHandler(this.txtAttackContextParserValue_TextChanged);
            // 
            // btnRemoveAttackContext
            // 
            this.btnRemoveAttackContext.Location = new System.Drawing.Point(89, 172);
            this.btnRemoveAttackContext.Name = "btnRemoveAttackContext";
            this.btnRemoveAttackContext.Size = new System.Drawing.Size(66, 23);
            this.btnRemoveAttackContext.TabIndex = 10;
            this.btnRemoveAttackContext.Text = "Remove";
            this.btnRemoveAttackContext.UseVisualStyleBackColor = true;
            this.btnRemoveAttackContext.Click += new System.EventHandler(this.btnRemoveAttackContext_Click);
            // 
            // lblAttackContextParserValue
            // 
            this.lblAttackContextParserValue.AutoSize = true;
            this.lblAttackContextParserValue.Location = new System.Drawing.Point(6, 237);
            this.lblAttackContextParserValue.Name = "lblAttackContextParserValue";
            this.lblAttackContextParserValue.Size = new System.Drawing.Size(70, 13);
            this.lblAttackContextParserValue.TabIndex = 11;
            this.lblAttackContextParserValue.Text = "Parser Value:";
            // 
            // txtAttackContextName
            // 
            this.txtAttackContextName.Location = new System.Drawing.Point(6, 214);
            this.txtAttackContextName.Name = "txtAttackContextName";
            this.txtAttackContextName.Size = new System.Drawing.Size(149, 20);
            this.txtAttackContextName.TabIndex = 9;
            this.txtAttackContextName.TextChanged += new System.EventHandler(this.txtAttackContextName_TextChanged);
            // 
            // lblAttackContextName
            // 
            this.lblAttackContextName.AutoSize = true;
            this.lblAttackContextName.Location = new System.Drawing.Point(6, 198);
            this.lblAttackContextName.Name = "lblAttackContextName";
            this.lblAttackContextName.Size = new System.Drawing.Size(77, 13);
            this.lblAttackContextName.TabIndex = 8;
            this.lblAttackContextName.Text = "Context Name:";
            // 
            // btnAddAttackContext
            // 
            this.btnAddAttackContext.Location = new System.Drawing.Point(6, 172);
            this.btnAddAttackContext.Name = "btnAddAttackContext";
            this.btnAddAttackContext.Size = new System.Drawing.Size(77, 23);
            this.btnAddAttackContext.TabIndex = 7;
            this.btnAddAttackContext.Text = "Add";
            this.btnAddAttackContext.UseVisualStyleBackColor = true;
            this.btnAddAttackContext.Click += new System.EventHandler(this.btnAddAttackContext_Click);
            // 
            // lstAttackContexts
            // 
            this.lstAttackContexts.FormattingEnabled = true;
            this.lstAttackContexts.Location = new System.Drawing.Point(6, 19);
            this.lstAttackContexts.Name = "lstAttackContexts";
            this.lstAttackContexts.Size = new System.Drawing.Size(149, 147);
            this.lstAttackContexts.TabIndex = 1;
            this.lstAttackContexts.SelectedIndexChanged += new System.EventHandler(this.lstAttackContexts_SelectedIndexChanged);
            // 
            // gbAttackVisibility
            // 
            this.gbAttackVisibility.Controls.Add(this.txtAttackVisibility);
            this.gbAttackVisibility.Controls.Add(this.lblAttackVisibility);
            this.gbAttackVisibility.Location = new System.Drawing.Point(710, 131);
            this.gbAttackVisibility.Name = "gbAttackVisibility";
            this.gbAttackVisibility.Size = new System.Drawing.Size(155, 69);
            this.gbAttackVisibility.TabIndex = 10;
            this.gbAttackVisibility.TabStop = false;
            this.gbAttackVisibility.Text = "Attack Visibility";
            // 
            // txtAttackVisibility
            // 
            this.txtAttackVisibility.Location = new System.Drawing.Point(6, 40);
            this.txtAttackVisibility.Name = "txtAttackVisibility";
            this.txtAttackVisibility.Size = new System.Drawing.Size(143, 20);
            this.txtAttackVisibility.TabIndex = 14;
            this.txtAttackVisibility.TextChanged += new System.EventHandler(this.txtAttackVisibility_TextChanged);
            // 
            // lblAttackVisibility
            // 
            this.lblAttackVisibility.AutoSize = true;
            this.lblAttackVisibility.Location = new System.Drawing.Point(6, 24);
            this.lblAttackVisibility.Name = "lblAttackVisibility";
            this.lblAttackVisibility.Size = new System.Drawing.Size(70, 13);
            this.lblAttackVisibility.TabIndex = 13;
            this.lblAttackVisibility.Text = "Parser Value:";
            // 
            // Attacks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 341);
            this.Controls.Add(this.gbAttackVisibility);
            this.Controls.Add(this.gbAttackContexts);
            this.Controls.Add(this.gbAttackTargetContext);
            this.Controls.Add(this.gbAttackOriginContext);
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
            this.gbAttackOriginContext.ResumeLayout(false);
            this.gbAttackOriginContext.PerformLayout();
            this.gbAttackTargetContext.ResumeLayout(false);
            this.gbAttackTargetContext.PerformLayout();
            this.gbAttackContexts.ResumeLayout(false);
            this.gbAttackContexts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackContextWeight)).EndInit();
            this.gbAttackVisibility.ResumeLayout(false);
            this.gbAttackVisibility.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbAttackOriginContext;
        private System.Windows.Forms.TextBox txtAttackOriginCustomType;
        private System.Windows.Forms.Label lblAttackOriginCustomType;
        private System.Windows.Forms.Label lblAttackOriginType;
        public System.Windows.Forms.ComboBox cbAttackOriginType;
        private System.Windows.Forms.GroupBox gbAttackTargetContext;
        private System.Windows.Forms.TextBox txtAttackTargetCustomType;
        private System.Windows.Forms.Label lblAttackTargetCustomType;
        private System.Windows.Forms.Label lblAttackTargetType;
        public System.Windows.Forms.ComboBox cbAttackTargetType;
        private System.Windows.Forms.GroupBox gbAttackContexts;
        private System.Windows.Forms.TextBox txtAttackContextName;
        private System.Windows.Forms.Label lblAttackContextName;
        private System.Windows.Forms.Button btnAddAttackContext;
        private System.Windows.Forms.ListBox lstAttackContexts;
        private System.Windows.Forms.Button btnRemoveAttackContext;
        private System.Windows.Forms.TextBox txtAttackContextParserValue;
        private System.Windows.Forms.Label lblAttackContextParserValue;
        private System.Windows.Forms.NumericUpDown txtAttackContextWeight;
        private System.Windows.Forms.Label lblAttackContextWeight;
        private System.Windows.Forms.GroupBox gbAttackVisibility;
        private System.Windows.Forms.TextBox txtAttackVisibility;
        private System.Windows.Forms.Label lblAttackVisibility;
    }
}