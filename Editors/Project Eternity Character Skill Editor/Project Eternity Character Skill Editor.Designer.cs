namespace ProjectEternity.Editors.CharacterSkillEditor
{
    partial class ProjectEternityCharacterSkillEditor
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
            this.gbLevels = new System.Windows.Forms.GroupBox();
            this.lstLevels = new System.Windows.Forms.ListBox();
            this.btnRemoveLevel = new System.Windows.Forms.Button();
            this.btnAddLevel = new System.Windows.Forms.Button();
            this.gbRequirements = new System.Windows.Forms.GroupBox();
            this.lstRequirements = new System.Windows.Forms.ListBox();
            this.btnRemoveRequirement = new System.Windows.Forms.Button();
            this.btnAddRequirement = new System.Windows.Forms.Button();
            this.cboRequirementType = new System.Windows.Forms.ComboBox();
            this.pgRequirement = new System.Windows.Forms.PropertyGrid();
            this.gbLifetimeTypes = new System.Windows.Forms.GroupBox();
            this.ckResetWhenRequirementFail = new System.Windows.Forms.CheckBox();
            this.txtMaximumStack = new System.Windows.Forms.NumericUpDown();
            this.txtMaximumLifetime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLifetimeStacking = new System.Windows.Forms.CheckBox();
            this.rbLifetimeBattle = new System.Windows.Forms.RadioButton();
            this.rbLifetimeTurns = new System.Windows.Forms.RadioButton();
            this.rbLifetimePermanent = new System.Windows.Forms.RadioButton();
            this.gbAffectedTypes = new System.Windows.Forms.GroupBox();
            this.cbAffectALLAllies = new System.Windows.Forms.CheckBox();
            this.txtRangeValue = new System.Windows.Forms.NumericUpDown();
            this.cbAffectAuraEnemy = new System.Windows.Forms.CheckBox();
            this.cbAffectAllEnemy = new System.Windows.Forms.CheckBox();
            this.cbAffectSquadEnemy = new System.Windows.Forms.CheckBox();
            this.cbAffectEnemy = new System.Windows.Forms.CheckBox();
            this.cbAffectAura = new System.Windows.Forms.CheckBox();
            this.cbAffectAll = new System.Windows.Forms.CheckBox();
            this.cbAffectSquad = new System.Windows.Forms.CheckBox();
            this.cbAffectSelf = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbActivations = new System.Windows.Forms.GroupBox();
            this.lblActivationChance = new System.Windows.Forms.Label();
            this.txtActivationChance = new System.Windows.Forms.NumericUpDown();
            this.lstActivations = new System.Windows.Forms.ListBox();
            this.btnRemoveActivation = new System.Windows.Forms.Button();
            this.btnAddActivation = new System.Windows.Forms.Button();
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.lstEffects = new System.Windows.Forms.ListBox();
            this.btnRemoveEffect = new System.Windows.Forms.Button();
            this.btnAddEffects = new System.Windows.Forms.Button();
            this.cboEffectType = new System.Windows.Forms.ComboBox();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.gbLevelDetails = new System.Windows.Forms.GroupBox();
            this.txtUpgradePrice = new System.Windows.Forms.NumericUpDown();
            this.lblUpgradePrice = new System.Windows.Forms.Label();
            this.gbLevels.SuspendLayout();
            this.gbRequirements.SuspendLayout();
            this.gbLifetimeTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).BeginInit();
            this.gbAffectedTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRangeValue)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbActivations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtActivationChance)).BeginInit();
            this.gbEffects.SuspendLayout();
            this.gbLevelDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpgradePrice)).BeginInit();
            this.SuspendLayout();
            // 
            // gbLevels
            // 
            this.gbLevels.Controls.Add(this.lstLevels);
            this.gbLevels.Controls.Add(this.btnRemoveLevel);
            this.gbLevels.Controls.Add(this.btnAddLevel);
            this.gbLevels.Location = new System.Drawing.Point(12, 27);
            this.gbLevels.Name = "gbLevels";
            this.gbLevels.Size = new System.Drawing.Size(181, 177);
            this.gbLevels.TabIndex = 0;
            this.gbLevels.TabStop = false;
            this.gbLevels.Text = "Levels";
            // 
            // lstLevels
            // 
            this.lstLevels.FormattingEnabled = true;
            this.lstLevels.Location = new System.Drawing.Point(6, 19);
            this.lstLevels.Name = "lstLevels";
            this.lstLevels.Size = new System.Drawing.Size(169, 121);
            this.lstLevels.TabIndex = 9;
            this.lstLevels.SelectedIndexChanged += new System.EventHandler(this.lstLevels_SelectedIndexChanged);
            // 
            // btnRemoveLevel
            // 
            this.btnRemoveLevel.Location = new System.Drawing.Point(100, 146);
            this.btnRemoveLevel.Name = "btnRemoveLevel";
            this.btnRemoveLevel.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveLevel.TabIndex = 9;
            this.btnRemoveLevel.Text = "Remove";
            this.btnRemoveLevel.UseVisualStyleBackColor = true;
            this.btnRemoveLevel.Click += new System.EventHandler(this.btnRemoveLevel_Click);
            // 
            // btnAddLevel
            // 
            this.btnAddLevel.Location = new System.Drawing.Point(6, 146);
            this.btnAddLevel.Name = "btnAddLevel";
            this.btnAddLevel.Size = new System.Drawing.Size(75, 23);
            this.btnAddLevel.TabIndex = 8;
            this.btnAddLevel.Text = "Add";
            this.btnAddLevel.UseVisualStyleBackColor = true;
            this.btnAddLevel.Click += new System.EventHandler(this.btnAddLevel_Click);
            // 
            // gbRequirements
            // 
            this.gbRequirements.Controls.Add(this.lstRequirements);
            this.gbRequirements.Controls.Add(this.btnRemoveRequirement);
            this.gbRequirements.Controls.Add(this.btnAddRequirement);
            this.gbRequirements.Controls.Add(this.cboRequirementType);
            this.gbRequirements.Controls.Add(this.pgRequirement);
            this.gbRequirements.Enabled = false;
            this.gbRequirements.Location = new System.Drawing.Point(386, 27);
            this.gbRequirements.Name = "gbRequirements";
            this.gbRequirements.Size = new System.Drawing.Size(183, 340);
            this.gbRequirements.TabIndex = 1;
            this.gbRequirements.TabStop = false;
            this.gbRequirements.Text = "Requirements";
            // 
            // lstRequirements
            // 
            this.lstRequirements.FormattingEnabled = true;
            this.lstRequirements.Location = new System.Drawing.Point(6, 19);
            this.lstRequirements.Name = "lstRequirements";
            this.lstRequirements.Size = new System.Drawing.Size(169, 95);
            this.lstRequirements.TabIndex = 10;
            this.lstRequirements.SelectedIndexChanged += new System.EventHandler(this.lstRequirements_SelectedIndexChanged);
            // 
            // btnRemoveRequirement
            // 
            this.btnRemoveRequirement.Location = new System.Drawing.Point(100, 122);
            this.btnRemoveRequirement.Name = "btnRemoveRequirement";
            this.btnRemoveRequirement.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveRequirement.TabIndex = 10;
            this.btnRemoveRequirement.Text = "Remove";
            this.btnRemoveRequirement.UseVisualStyleBackColor = true;
            this.btnRemoveRequirement.Click += new System.EventHandler(this.btnRemoveRequirement_Click);
            // 
            // btnAddRequirement
            // 
            this.btnAddRequirement.Location = new System.Drawing.Point(6, 122);
            this.btnAddRequirement.Name = "btnAddRequirement";
            this.btnAddRequirement.Size = new System.Drawing.Size(75, 23);
            this.btnAddRequirement.TabIndex = 9;
            this.btnAddRequirement.Text = "Add";
            this.btnAddRequirement.UseVisualStyleBackColor = true;
            this.btnAddRequirement.Click += new System.EventHandler(this.btnAddRequirement_Click);
            // 
            // cboRequirementType
            // 
            this.cboRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRequirementType.FormattingEnabled = true;
            this.cboRequirementType.Location = new System.Drawing.Point(6, 151);
            this.cboRequirementType.Name = "cboRequirementType";
            this.cboRequirementType.Size = new System.Drawing.Size(169, 21);
            this.cboRequirementType.TabIndex = 3;
            this.cboRequirementType.SelectedIndexChanged += new System.EventHandler(this.cboRequirementType_SelectedIndexChanged);
            // 
            // pgRequirement
            // 
            this.pgRequirement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgRequirement.Location = new System.Drawing.Point(6, 178);
            this.pgRequirement.Name = "pgRequirement";
            this.pgRequirement.Size = new System.Drawing.Size(169, 156);
            this.pgRequirement.TabIndex = 2;
            this.pgRequirement.ToolbarVisible = false;
            // 
            // gbLifetimeTypes
            // 
            this.gbLifetimeTypes.Controls.Add(this.ckResetWhenRequirementFail);
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumStack);
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumLifetime);
            this.gbLifetimeTypes.Controls.Add(this.label2);
            this.gbLifetimeTypes.Controls.Add(this.label1);
            this.gbLifetimeTypes.Controls.Add(this.cbLifetimeStacking);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeBattle);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeTurns);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimePermanent);
            this.gbLifetimeTypes.Enabled = false;
            this.gbLifetimeTypes.Location = new System.Drawing.Point(764, 147);
            this.gbLifetimeTypes.Name = "gbLifetimeTypes";
            this.gbLifetimeTypes.Size = new System.Drawing.Size(204, 148);
            this.gbLifetimeTypes.TabIndex = 4;
            this.gbLifetimeTypes.TabStop = false;
            this.gbLifetimeTypes.Text = "Lifetime types";
            // 
            // ckResetWhenRequirementFail
            // 
            this.ckResetWhenRequirementFail.AutoSize = true;
            this.ckResetWhenRequirementFail.Location = new System.Drawing.Point(6, 127);
            this.ckResetWhenRequirementFail.Name = "ckResetWhenRequirementFail";
            this.ckResetWhenRequirementFail.Size = new System.Drawing.Size(167, 17);
            this.ckResetWhenRequirementFail.TabIndex = 12;
            this.ckResetWhenRequirementFail.Text = "Reset when Requirements fail";
            this.ckResetWhenRequirementFail.UseVisualStyleBackColor = true;
            // 
            // txtMaximumStack
            // 
            this.txtMaximumStack.Location = new System.Drawing.Point(122, 56);
            this.txtMaximumStack.Name = "txtMaximumStack";
            this.txtMaximumStack.Size = new System.Drawing.Size(76, 20);
            this.txtMaximumStack.TabIndex = 11;
            this.txtMaximumStack.ValueChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // txtMaximumLifetime
            // 
            this.txtMaximumLifetime.Location = new System.Drawing.Point(6, 101);
            this.txtMaximumLifetime.Name = "txtMaximumLifetime";
            this.txtMaximumLifetime.Size = new System.Drawing.Size(76, 20);
            this.txtMaximumLifetime.TabIndex = 10;
            this.txtMaximumLifetime.ValueChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Maximum lifetime";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Maxiumu stack";
            // 
            // cbLifetimeStacking
            // 
            this.cbLifetimeStacking.AutoSize = true;
            this.cbLifetimeStacking.Location = new System.Drawing.Point(124, 20);
            this.cbLifetimeStacking.Name = "cbLifetimeStacking";
            this.cbLifetimeStacking.Size = new System.Drawing.Size(68, 17);
            this.cbLifetimeStacking.TabIndex = 3;
            this.cbLifetimeStacking.Text = "Stacking";
            this.cbLifetimeStacking.UseVisualStyleBackColor = true;
            this.cbLifetimeStacking.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // rbLifetimeBattle
            // 
            this.rbLifetimeBattle.AutoSize = true;
            this.rbLifetimeBattle.Location = new System.Drawing.Point(6, 65);
            this.rbLifetimeBattle.Name = "rbLifetimeBattle";
            this.rbLifetimeBattle.Size = new System.Drawing.Size(52, 17);
            this.rbLifetimeBattle.TabIndex = 2;
            this.rbLifetimeBattle.TabStop = true;
            this.rbLifetimeBattle.Text = "Battle";
            this.rbLifetimeBattle.UseVisualStyleBackColor = true;
            this.rbLifetimeBattle.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // rbLifetimeTurns
            // 
            this.rbLifetimeTurns.AutoSize = true;
            this.rbLifetimeTurns.Location = new System.Drawing.Point(6, 42);
            this.rbLifetimeTurns.Name = "rbLifetimeTurns";
            this.rbLifetimeTurns.Size = new System.Drawing.Size(52, 17);
            this.rbLifetimeTurns.TabIndex = 1;
            this.rbLifetimeTurns.TabStop = true;
            this.rbLifetimeTurns.Text = "Turns";
            this.rbLifetimeTurns.UseVisualStyleBackColor = true;
            this.rbLifetimeTurns.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // rbLifetimePermanent
            // 
            this.rbLifetimePermanent.AutoSize = true;
            this.rbLifetimePermanent.Location = new System.Drawing.Point(6, 19);
            this.rbLifetimePermanent.Name = "rbLifetimePermanent";
            this.rbLifetimePermanent.Size = new System.Drawing.Size(76, 17);
            this.rbLifetimePermanent.TabIndex = 0;
            this.rbLifetimePermanent.TabStop = true;
            this.rbLifetimePermanent.Text = "Permanent";
            this.rbLifetimePermanent.UseVisualStyleBackColor = true;
            this.rbLifetimePermanent.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // gbAffectedTypes
            // 
            this.gbAffectedTypes.Controls.Add(this.cbAffectALLAllies);
            this.gbAffectedTypes.Controls.Add(this.txtRangeValue);
            this.gbAffectedTypes.Controls.Add(this.cbAffectAuraEnemy);
            this.gbAffectedTypes.Controls.Add(this.cbAffectAllEnemy);
            this.gbAffectedTypes.Controls.Add(this.cbAffectSquadEnemy);
            this.gbAffectedTypes.Controls.Add(this.cbAffectEnemy);
            this.gbAffectedTypes.Controls.Add(this.cbAffectAura);
            this.gbAffectedTypes.Controls.Add(this.cbAffectAll);
            this.gbAffectedTypes.Controls.Add(this.cbAffectSquad);
            this.gbAffectedTypes.Controls.Add(this.cbAffectSelf);
            this.gbAffectedTypes.Enabled = false;
            this.gbAffectedTypes.Location = new System.Drawing.Point(763, 27);
            this.gbAffectedTypes.Name = "gbAffectedTypes";
            this.gbAffectedTypes.Size = new System.Drawing.Size(245, 114);
            this.gbAffectedTypes.TabIndex = 2;
            this.gbAffectedTypes.TabStop = false;
            this.gbAffectedTypes.Text = "Affected types";
            // 
            // cbAffectALLAllies
            // 
            this.cbAffectALLAllies.AutoSize = true;
            this.cbAffectALLAllies.Location = new System.Drawing.Point(57, 90);
            this.cbAffectALLAllies.Name = "cbAffectALLAllies";
            this.cbAffectALLAllies.Size = new System.Drawing.Size(71, 17);
            this.cbAffectALLAllies.TabIndex = 10;
            this.cbAffectALLAllies.Text = "ALL allies";
            this.cbAffectALLAllies.UseVisualStyleBackColor = true;
            this.cbAffectALLAllies.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // txtRangeValue
            // 
            this.txtRangeValue.Location = new System.Drawing.Point(187, 64);
            this.txtRangeValue.Name = "txtRangeValue";
            this.txtRangeValue.Size = new System.Drawing.Size(52, 20);
            this.txtRangeValue.TabIndex = 9;
            this.txtRangeValue.ValueChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectAuraEnemy
            // 
            this.cbAffectAuraEnemy.AutoSize = true;
            this.cbAffectAuraEnemy.Location = new System.Drawing.Point(157, 41);
            this.cbAffectAuraEnemy.Name = "cbAffectAuraEnemy";
            this.cbAffectAuraEnemy.Size = new System.Drawing.Size(82, 17);
            this.cbAffectAuraEnemy.TabIndex = 7;
            this.cbAffectAuraEnemy.Text = "Enemy aura";
            this.cbAffectAuraEnemy.UseVisualStyleBackColor = true;
            this.cbAffectAuraEnemy.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectAllEnemy
            // 
            this.cbAffectAllEnemy.AutoSize = true;
            this.cbAffectAllEnemy.Location = new System.Drawing.Point(152, 90);
            this.cbAffectAllEnemy.Name = "cbAffectAllEnemy";
            this.cbAffectAllEnemy.Size = new System.Drawing.Size(87, 17);
            this.cbAffectAllEnemy.TabIndex = 6;
            this.cbAffectAllEnemy.Text = "ALL enemies";
            this.cbAffectAllEnemy.UseVisualStyleBackColor = true;
            this.cbAffectAllEnemy.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectSquadEnemy
            // 
            this.cbAffectSquadEnemy.AutoSize = true;
            this.cbAffectSquadEnemy.Location = new System.Drawing.Point(6, 64);
            this.cbAffectSquadEnemy.Name = "cbAffectSquadEnemy";
            this.cbAffectSquadEnemy.Size = new System.Drawing.Size(92, 17);
            this.cbAffectSquadEnemy.TabIndex = 5;
            this.cbAffectSquadEnemy.Text = "Enemy Squad";
            this.cbAffectSquadEnemy.UseVisualStyleBackColor = true;
            this.cbAffectSquadEnemy.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectEnemy
            // 
            this.cbAffectEnemy.AutoSize = true;
            this.cbAffectEnemy.Location = new System.Drawing.Point(6, 42);
            this.cbAffectEnemy.Name = "cbAffectEnemy";
            this.cbAffectEnemy.Size = new System.Drawing.Size(58, 17);
            this.cbAffectEnemy.TabIndex = 4;
            this.cbAffectEnemy.Text = "Enemy";
            this.cbAffectEnemy.UseVisualStyleBackColor = true;
            this.cbAffectEnemy.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectAura
            // 
            this.cbAffectAura.AutoSize = true;
            this.cbAffectAura.Location = new System.Drawing.Point(157, 18);
            this.cbAffectAura.Name = "cbAffectAura";
            this.cbAffectAura.Size = new System.Drawing.Size(48, 17);
            this.cbAffectAura.TabIndex = 3;
            this.cbAffectAura.Text = "Aura";
            this.cbAffectAura.UseVisualStyleBackColor = true;
            this.cbAffectAura.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectAll
            // 
            this.cbAffectAll.AutoSize = true;
            this.cbAffectAll.Location = new System.Drawing.Point(6, 90);
            this.cbAffectAll.Name = "cbAffectAll";
            this.cbAffectAll.Size = new System.Drawing.Size(45, 17);
            this.cbAffectAll.TabIndex = 2;
            this.cbAffectAll.Text = "ALL";
            this.cbAffectAll.UseVisualStyleBackColor = true;
            this.cbAffectAll.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectSquad
            // 
            this.cbAffectSquad.AutoSize = true;
            this.cbAffectSquad.Location = new System.Drawing.Point(56, 19);
            this.cbAffectSquad.Name = "cbAffectSquad";
            this.cbAffectSquad.Size = new System.Drawing.Size(57, 17);
            this.cbAffectSquad.TabIndex = 1;
            this.cbAffectSquad.Text = "Squad";
            this.cbAffectSquad.UseVisualStyleBackColor = true;
            this.cbAffectSquad.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // cbAffectSelf
            // 
            this.cbAffectSelf.AutoSize = true;
            this.cbAffectSelf.Location = new System.Drawing.Point(6, 19);
            this.cbAffectSelf.Name = "cbAffectSelf";
            this.cbAffectSelf.Size = new System.Drawing.Size(44, 17);
            this.cbAffectSelf.TabIndex = 0;
            this.cbAffectSelf.Text = "Self";
            this.cbAffectSelf.UseVisualStyleBackColor = true;
            this.cbAffectSelf.CheckedChanged += new System.EventHandler(this.UpdateEffect);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDescription);
            this.groupBox3.Location = new System.Drawing.Point(12, 210);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(181, 157);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(169, 132);
            this.txtDescription.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1020, 24);
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
            // gbActivations
            // 
            this.gbActivations.Controls.Add(this.lblActivationChance);
            this.gbActivations.Controls.Add(this.txtActivationChance);
            this.gbActivations.Controls.Add(this.lstActivations);
            this.gbActivations.Controls.Add(this.btnRemoveActivation);
            this.gbActivations.Controls.Add(this.btnAddActivation);
            this.gbActivations.Enabled = false;
            this.gbActivations.Location = new System.Drawing.Point(199, 27);
            this.gbActivations.Name = "gbActivations";
            this.gbActivations.Size = new System.Drawing.Size(181, 177);
            this.gbActivations.TabIndex = 6;
            this.gbActivations.TabStop = false;
            this.gbActivations.Text = "Activations";
            // 
            // lblActivationChance
            // 
            this.lblActivationChance.AutoSize = true;
            this.lblActivationChance.Location = new System.Drawing.Point(7, 122);
            this.lblActivationChance.Name = "lblActivationChance";
            this.lblActivationChance.Size = new System.Drawing.Size(93, 13);
            this.lblActivationChance.TabIndex = 13;
            this.lblActivationChance.Text = "Activation chance";
            // 
            // txtActivationChance
            // 
            this.txtActivationChance.Location = new System.Drawing.Point(106, 120);
            this.txtActivationChance.Name = "txtActivationChance";
            this.txtActivationChance.Size = new System.Drawing.Size(69, 20);
            this.txtActivationChance.TabIndex = 12;
            this.txtActivationChance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtActivationChance.ValueChanged += new System.EventHandler(this.txtActivationChance_ValueChanged);
            // 
            // lstActivations
            // 
            this.lstActivations.FormattingEnabled = true;
            this.lstActivations.Location = new System.Drawing.Point(6, 19);
            this.lstActivations.Name = "lstActivations";
            this.lstActivations.Size = new System.Drawing.Size(169, 95);
            this.lstActivations.TabIndex = 8;
            this.lstActivations.SelectedIndexChanged += new System.EventHandler(this.lstActivations_SelectedIndexChanged);
            // 
            // btnRemoveActivation
            // 
            this.btnRemoveActivation.Location = new System.Drawing.Point(100, 146);
            this.btnRemoveActivation.Name = "btnRemoveActivation";
            this.btnRemoveActivation.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveActivation.TabIndex = 7;
            this.btnRemoveActivation.Text = "Remove";
            this.btnRemoveActivation.UseVisualStyleBackColor = true;
            this.btnRemoveActivation.Click += new System.EventHandler(this.btnRemoveActivation_Click);
            // 
            // btnAddActivation
            // 
            this.btnAddActivation.Location = new System.Drawing.Point(6, 146);
            this.btnAddActivation.Name = "btnAddActivation";
            this.btnAddActivation.Size = new System.Drawing.Size(75, 23);
            this.btnAddActivation.TabIndex = 6;
            this.btnAddActivation.Text = "Add";
            this.btnAddActivation.UseVisualStyleBackColor = true;
            this.btnAddActivation.Click += new System.EventHandler(this.btnAddActivation_Click);
            // 
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.lstEffects);
            this.gbEffects.Controls.Add(this.btnRemoveEffect);
            this.gbEffects.Controls.Add(this.btnAddEffects);
            this.gbEffects.Controls.Add(this.cboEffectType);
            this.gbEffects.Controls.Add(this.pgEffect);
            this.gbEffects.Enabled = false;
            this.gbEffects.Location = new System.Drawing.Point(575, 27);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(183, 340);
            this.gbEffects.TabIndex = 11;
            this.gbEffects.TabStop = false;
            this.gbEffects.Text = "Effects";
            // 
            // lstEffects
            // 
            this.lstEffects.FormattingEnabled = true;
            this.lstEffects.Location = new System.Drawing.Point(6, 19);
            this.lstEffects.Name = "lstEffects";
            this.lstEffects.Size = new System.Drawing.Size(169, 95);
            this.lstEffects.TabIndex = 11;
            this.lstEffects.SelectedIndexChanged += new System.EventHandler(this.lstEffects_SelectedIndexChanged);
            // 
            // btnRemoveEffect
            // 
            this.btnRemoveEffect.Location = new System.Drawing.Point(100, 122);
            this.btnRemoveEffect.Name = "btnRemoveEffect";
            this.btnRemoveEffect.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveEffect.TabIndex = 10;
            this.btnRemoveEffect.Text = "Remove";
            this.btnRemoveEffect.UseVisualStyleBackColor = true;
            this.btnRemoveEffect.Click += new System.EventHandler(this.btnRemoveEffect_Click);
            // 
            // btnAddEffects
            // 
            this.btnAddEffects.Location = new System.Drawing.Point(6, 122);
            this.btnAddEffects.Name = "btnAddEffects";
            this.btnAddEffects.Size = new System.Drawing.Size(75, 23);
            this.btnAddEffects.TabIndex = 9;
            this.btnAddEffects.Text = "Add";
            this.btnAddEffects.UseVisualStyleBackColor = true;
            this.btnAddEffects.Click += new System.EventHandler(this.btnAddEffects_Click);
            // 
            // cboEffectType
            // 
            this.cboEffectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEffectType.FormattingEnabled = true;
            this.cboEffectType.Location = new System.Drawing.Point(6, 151);
            this.cboEffectType.Name = "cboEffectType";
            this.cboEffectType.Size = new System.Drawing.Size(169, 21);
            this.cboEffectType.TabIndex = 3;
            this.cboEffectType.SelectedIndexChanged += new System.EventHandler(this.cboEffectType_SelectedIndexChanged);
            // 
            // pgEffect
            // 
            this.pgEffect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgEffect.Location = new System.Drawing.Point(6, 178);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(169, 156);
            this.pgEffect.TabIndex = 2;
            this.pgEffect.ToolbarVisible = false;
            // 
            // gbLevelDetails
            // 
            this.gbLevelDetails.Controls.Add(this.txtUpgradePrice);
            this.gbLevelDetails.Controls.Add(this.lblUpgradePrice);
            this.gbLevelDetails.Location = new System.Drawing.Point(199, 210);
            this.gbLevelDetails.Name = "gbLevelDetails";
            this.gbLevelDetails.Size = new System.Drawing.Size(181, 157);
            this.gbLevelDetails.TabIndex = 12;
            this.gbLevelDetails.TabStop = false;
            this.gbLevelDetails.Text = "Level Details";
            // 
            // txtUpgradePrice
            // 
            this.txtUpgradePrice.Location = new System.Drawing.Point(89, 19);
            this.txtUpgradePrice.Name = "txtUpgradePrice";
            this.txtUpgradePrice.Size = new System.Drawing.Size(86, 20);
            this.txtUpgradePrice.TabIndex = 12;
            this.txtUpgradePrice.ValueChanged += new System.EventHandler(this.txtUpgradePrice_ValueChanged);
            // 
            // lblUpgradePrice
            // 
            this.lblUpgradePrice.AutoSize = true;
            this.lblUpgradePrice.Location = new System.Drawing.Point(6, 22);
            this.lblUpgradePrice.Name = "lblUpgradePrice";
            this.lblUpgradePrice.Size = new System.Drawing.Size(77, 13);
            this.lblUpgradePrice.TabIndex = 0;
            this.lblUpgradePrice.Text = "Upgrade price:";
            // 
            // ProjectEternityCharacterSkillEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 379);
            this.Controls.Add(this.gbLevelDetails);
            this.Controls.Add(this.gbEffects);
            this.Controls.Add(this.gbActivations);
            this.Controls.Add(this.gbLifetimeTypes);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbAffectedTypes);
            this.Controls.Add(this.gbRequirements);
            this.Controls.Add(this.gbLevels);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ProjectEternityCharacterSkillEditor";
            this.Text = "Project Eternity Character Skill Editor";
            this.gbLevels.ResumeLayout(false);
            this.gbRequirements.ResumeLayout(false);
            this.gbLifetimeTypes.ResumeLayout(false);
            this.gbLifetimeTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).EndInit();
            this.gbAffectedTypes.ResumeLayout(false);
            this.gbAffectedTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRangeValue)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbActivations.ResumeLayout(false);
            this.gbActivations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtActivationChance)).EndInit();
            this.gbEffects.ResumeLayout(false);
            this.gbLevelDetails.ResumeLayout(false);
            this.gbLevelDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtUpgradePrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbLevels;
        private System.Windows.Forms.GroupBox gbRequirements;
        private System.Windows.Forms.PropertyGrid pgRequirement;
        private System.Windows.Forms.ComboBox cboRequirementType;
        private System.Windows.Forms.GroupBox gbAffectedTypes;
        private System.Windows.Forms.CheckBox cbAffectSelf;
        private System.Windows.Forms.CheckBox cbAffectSquad;
        private System.Windows.Forms.CheckBox cbAffectAll;
        private System.Windows.Forms.CheckBox cbAffectAura;
        private System.Windows.Forms.CheckBox cbAffectSquadEnemy;
        private System.Windows.Forms.CheckBox cbAffectEnemy;
        private System.Windows.Forms.CheckBox cbAffectAllEnemy;
        private System.Windows.Forms.CheckBox cbAffectAuraEnemy;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbLifetimeTypes;
        private System.Windows.Forms.RadioButton rbLifetimePermanent;
        private System.Windows.Forms.RadioButton rbLifetimeTurns;
        private System.Windows.Forms.RadioButton rbLifetimeBattle;
        private System.Windows.Forms.CheckBox cbLifetimeStacking;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown txtRangeValue;
        private System.Windows.Forms.GroupBox gbActivations;
        private System.Windows.Forms.Button btnRemoveLevel;
        private System.Windows.Forms.Button btnAddLevel;
        private System.Windows.Forms.Button btnRemoveRequirement;
        private System.Windows.Forms.Button btnAddRequirement;
        private System.Windows.Forms.Button btnRemoveActivation;
        private System.Windows.Forms.Button btnAddActivation;
        private System.Windows.Forms.CheckBox cbAffectALLAllies;
        private System.Windows.Forms.GroupBox gbEffects;
        private System.Windows.Forms.Button btnRemoveEffect;
        private System.Windows.Forms.Button btnAddEffects;
        private System.Windows.Forms.ComboBox cboEffectType;
        private System.Windows.Forms.PropertyGrid pgEffect;
        private System.Windows.Forms.ListBox lstLevels;
        private System.Windows.Forms.ListBox lstRequirements;
        private System.Windows.Forms.ListBox lstActivations;
        private System.Windows.Forms.ListBox lstEffects;
        private System.Windows.Forms.NumericUpDown txtMaximumLifetime;
        private System.Windows.Forms.NumericUpDown txtMaximumStack;
        private System.Windows.Forms.CheckBox ckResetWhenRequirementFail;
        private System.Windows.Forms.GroupBox gbLevelDetails;
        private System.Windows.Forms.NumericUpDown txtUpgradePrice;
        private System.Windows.Forms.Label lblUpgradePrice;
        private System.Windows.Forms.Label lblActivationChance;
        private System.Windows.Forms.NumericUpDown txtActivationChance;
    }
}

