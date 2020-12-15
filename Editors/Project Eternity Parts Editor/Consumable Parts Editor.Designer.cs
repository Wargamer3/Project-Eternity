namespace ProjectEternity.Editors.PartsEditor
{
    partial class ProjectEternityConsumablePartEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lvEffects = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cboEffectType = new System.Windows.Forms.ComboBox();
            this.gbEffectInformation = new System.Windows.Forms.GroupBox();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.gbLifetimeTypes = new System.Windows.Forms.GroupBox();
            this.txtMaximumLifetime = new System.Windows.Forms.NumericUpDown();
            this.txtMaximumStack = new System.Windows.Forms.NumericUpDown();
            this.rbLifetimeOnAction = new System.Windows.Forms.RadioButton();
            this.rbLifetimeOnEnemyAttack = new System.Windows.Forms.RadioButton();
            this.rbLifetimeOnAttack = new System.Windows.Forms.RadioButton();
            this.rbLifetimeOnEnemyHit = new System.Windows.Forms.RadioButton();
            this.rbLifetimeOnHit = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLifetimeStacking = new System.Windows.Forms.CheckBox();
            this.rbLifetimeBattle = new System.Windows.Forms.RadioButton();
            this.rbLifetimeTurns = new System.Windows.Forms.RadioButton();
            this.rbLifetimePermanent = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.gbAffectedTypes = new System.Windows.Forms.GroupBox();
            this.rbAffectEveryone = new System.Windows.Forms.RadioButton();
            this.rbAffectAllEnemies = new System.Windows.Forms.RadioButton();
            this.rbAffectEnemySquad = new System.Windows.Forms.RadioButton();
            this.rbAffectEnemy = new System.Windows.Forms.RadioButton();
            this.rbAffectAllAllies = new System.Windows.Forms.RadioButton();
            this.rbAffectAllySquad = new System.Windows.Forms.RadioButton();
            this.rbAffectAlly = new System.Windows.Forms.RadioButton();
            this.rbAffectSelfSquad = new System.Windows.Forms.RadioButton();
            this.rbAffectSelf = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtRange = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbEffectInformation.SuspendLayout();
            this.gbLifetimeTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.gbAffectedTypes.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.lvEffects);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(203, 254);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Effects";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(122, 225);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 225);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvEffects
            // 
            this.lvEffects.Location = new System.Drawing.Point(6, 19);
            this.lvEffects.MultiSelect = false;
            this.lvEffects.Name = "lvEffects";
            this.lvEffects.Size = new System.Drawing.Size(191, 200);
            this.lvEffects.TabIndex = 0;
            this.lvEffects.UseCompatibleStateImageBehavior = false;
            this.lvEffects.View = System.Windows.Forms.View.List;
            this.lvEffects.SelectedIndexChanged += new System.EventHandler(this.lvEffects_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(702, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // cboEffectType
            // 
            this.cboEffectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEffectType.FormattingEnabled = true;
            this.cboEffectType.Location = new System.Drawing.Point(9, 19);
            this.cboEffectType.Name = "cboEffectType";
            this.cboEffectType.Size = new System.Drawing.Size(185, 21);
            this.cboEffectType.TabIndex = 2;
            this.cboEffectType.SelectedIndexChanged += new System.EventHandler(this.cboEffectType_SelectedIndexChanged);
            // 
            // gbEffectInformation
            // 
            this.gbEffectInformation.Controls.Add(this.pgEffect);
            this.gbEffectInformation.Controls.Add(this.cboEffectType);
            this.gbEffectInformation.Enabled = false;
            this.gbEffectInformation.Location = new System.Drawing.Point(221, 27);
            this.gbEffectInformation.Name = "gbEffectInformation";
            this.gbEffectInformation.Size = new System.Drawing.Size(200, 254);
            this.gbEffectInformation.TabIndex = 3;
            this.gbEffectInformation.TabStop = false;
            this.gbEffectInformation.Text = "Effect information";
            // 
            // pgEffect
            // 
            this.pgEffect.Location = new System.Drawing.Point(6, 46);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(188, 202);
            this.pgEffect.TabIndex = 3;
            // 
            // gbLifetimeTypes
            // 
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumLifetime);
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumStack);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeOnAction);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeOnEnemyAttack);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeOnAttack);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeOnEnemyHit);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeOnHit);
            this.gbLifetimeTypes.Controls.Add(this.label2);
            this.gbLifetimeTypes.Controls.Add(this.label1);
            this.gbLifetimeTypes.Controls.Add(this.cbLifetimeStacking);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeBattle);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeTurns);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimePermanent);
            this.gbLifetimeTypes.Enabled = false;
            this.gbLifetimeTypes.Location = new System.Drawing.Point(427, 27);
            this.gbLifetimeTypes.Name = "gbLifetimeTypes";
            this.gbLifetimeTypes.Size = new System.Drawing.Size(263, 150);
            this.gbLifetimeTypes.TabIndex = 5;
            this.gbLifetimeTypes.TabStop = false;
            this.gbLifetimeTypes.Text = "Lifetime types";
            // 
            // txtMaximumLifetime
            // 
            this.txtMaximumLifetime.Location = new System.Drawing.Point(6, 124);
            this.txtMaximumLifetime.Name = "txtMaximumLifetime";
            this.txtMaximumLifetime.Size = new System.Drawing.Size(80, 20);
            this.txtMaximumLifetime.TabIndex = 17;
            // 
            // txtMaximumStack
            // 
            this.txtMaximumStack.Location = new System.Drawing.Point(183, 124);
            this.txtMaximumStack.Name = "txtMaximumStack";
            this.txtMaximumStack.Size = new System.Drawing.Size(74, 20);
            this.txtMaximumStack.TabIndex = 16;
            this.txtMaximumStack.ValueChanged += new System.EventHandler(this.txtMaximumStack_ValueChanged);
            // 
            // rbLifetimeOnAction
            // 
            this.rbLifetimeOnAction.AutoSize = true;
            this.rbLifetimeOnAction.Location = new System.Drawing.Point(88, 88);
            this.rbLifetimeOnAction.Name = "rbLifetimeOnAction";
            this.rbLifetimeOnAction.Size = new System.Drawing.Size(71, 17);
            this.rbLifetimeOnAction.TabIndex = 14;
            this.rbLifetimeOnAction.TabStop = true;
            this.rbLifetimeOnAction.Text = "On action";
            this.rbLifetimeOnAction.UseVisualStyleBackColor = true;
            this.rbLifetimeOnAction.CheckedChanged += new System.EventHandler(this.rbLifetimeOnAction_CheckedChanged);
            // 
            // rbLifetimeOnEnemyAttack
            // 
            this.rbLifetimeOnEnemyAttack.AutoSize = true;
            this.rbLifetimeOnEnemyAttack.Location = new System.Drawing.Point(88, 65);
            this.rbLifetimeOnEnemyAttack.Name = "rbLifetimeOnEnemyAttack";
            this.rbLifetimeOnEnemyAttack.Size = new System.Drawing.Size(106, 17);
            this.rbLifetimeOnEnemyAttack.TabIndex = 13;
            this.rbLifetimeOnEnemyAttack.TabStop = true;
            this.rbLifetimeOnEnemyAttack.Text = "On enemy attack";
            this.rbLifetimeOnEnemyAttack.UseVisualStyleBackColor = true;
            this.rbLifetimeOnEnemyAttack.CheckedChanged += new System.EventHandler(this.rbLifetimeOnEnemyAttack_CheckedChanged);
            // 
            // rbLifetimeOnAttack
            // 
            this.rbLifetimeOnAttack.AutoSize = true;
            this.rbLifetimeOnAttack.Location = new System.Drawing.Point(88, 42);
            this.rbLifetimeOnAttack.Name = "rbLifetimeOnAttack";
            this.rbLifetimeOnAttack.Size = new System.Drawing.Size(72, 17);
            this.rbLifetimeOnAttack.TabIndex = 12;
            this.rbLifetimeOnAttack.TabStop = true;
            this.rbLifetimeOnAttack.Text = "On attack";
            this.rbLifetimeOnAttack.UseVisualStyleBackColor = true;
            this.rbLifetimeOnAttack.CheckedChanged += new System.EventHandler(this.rbLifetimeOnAttack_CheckedChanged);
            // 
            // rbLifetimeOnEnemyHit
            // 
            this.rbLifetimeOnEnemyHit.AutoSize = true;
            this.rbLifetimeOnEnemyHit.Location = new System.Drawing.Point(88, 19);
            this.rbLifetimeOnEnemyHit.Name = "rbLifetimeOnEnemyHit";
            this.rbLifetimeOnEnemyHit.Size = new System.Drawing.Size(87, 17);
            this.rbLifetimeOnEnemyHit.TabIndex = 11;
            this.rbLifetimeOnEnemyHit.TabStop = true;
            this.rbLifetimeOnEnemyHit.Text = "On enemy hit";
            this.rbLifetimeOnEnemyHit.UseVisualStyleBackColor = true;
            this.rbLifetimeOnEnemyHit.CheckedChanged += new System.EventHandler(this.rbLifetimeOnEnemyHit_CheckedChanged);
            // 
            // rbLifetimeOnHit
            // 
            this.rbLifetimeOnHit.AutoSize = true;
            this.rbLifetimeOnHit.Location = new System.Drawing.Point(6, 88);
            this.rbLifetimeOnHit.Name = "rbLifetimeOnHit";
            this.rbLifetimeOnHit.Size = new System.Drawing.Size(53, 17);
            this.rbLifetimeOnHit.TabIndex = 10;
            this.rbLifetimeOnHit.TabStop = true;
            this.rbLifetimeOnHit.Text = "On hit";
            this.rbLifetimeOnHit.UseVisualStyleBackColor = true;
            this.rbLifetimeOnHit.CheckedChanged += new System.EventHandler(this.rbLifetimeOnHit_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Maximum lifetime";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(180, 108);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Maxiumum stack";
            // 
            // cbLifetimeStacking
            // 
            this.cbLifetimeStacking.AutoSize = true;
            this.cbLifetimeStacking.Location = new System.Drawing.Point(183, 88);
            this.cbLifetimeStacking.Name = "cbLifetimeStacking";
            this.cbLifetimeStacking.Size = new System.Drawing.Size(68, 17);
            this.cbLifetimeStacking.TabIndex = 3;
            this.cbLifetimeStacking.Text = "Stacking";
            this.cbLifetimeStacking.UseVisualStyleBackColor = true;
            this.cbLifetimeStacking.CheckedChanged += new System.EventHandler(this.cbLifetimeStacking_CheckedChanged);
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
            this.rbLifetimeBattle.CheckedChanged += new System.EventHandler(this.rbLifetimeBattle_CheckedChanged);
            // 
            // rbLifetimeTurns
            // 
            this.rbLifetimeTurns.AutoSize = true;
            this.rbLifetimeTurns.Location = new System.Drawing.Point(7, 42);
            this.rbLifetimeTurns.Name = "rbLifetimeTurns";
            this.rbLifetimeTurns.Size = new System.Drawing.Size(52, 17);
            this.rbLifetimeTurns.TabIndex = 1;
            this.rbLifetimeTurns.TabStop = true;
            this.rbLifetimeTurns.Text = "Turns";
            this.rbLifetimeTurns.UseVisualStyleBackColor = true;
            this.rbLifetimeTurns.CheckedChanged += new System.EventHandler(this.rbLifetimeTurns_CheckedChanged);
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
            this.rbLifetimePermanent.CheckedChanged += new System.EventHandler(this.rbLifetimePermanent_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDescription);
            this.groupBox3.Location = new System.Drawing.Point(427, 235);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(263, 117);
            this.groupBox3.TabIndex = 6;
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
            this.txtDescription.Size = new System.Drawing.Size(251, 92);
            this.txtDescription.TabIndex = 0;
            // 
            // gbAffectedTypes
            // 
            this.gbAffectedTypes.Controls.Add(this.rbAffectEveryone);
            this.gbAffectedTypes.Controls.Add(this.rbAffectAllEnemies);
            this.gbAffectedTypes.Controls.Add(this.rbAffectEnemySquad);
            this.gbAffectedTypes.Controls.Add(this.rbAffectEnemy);
            this.gbAffectedTypes.Controls.Add(this.rbAffectAllAllies);
            this.gbAffectedTypes.Controls.Add(this.rbAffectAllySquad);
            this.gbAffectedTypes.Controls.Add(this.rbAffectAlly);
            this.gbAffectedTypes.Controls.Add(this.rbAffectSelfSquad);
            this.gbAffectedTypes.Controls.Add(this.rbAffectSelf);
            this.gbAffectedTypes.Location = new System.Drawing.Point(12, 287);
            this.gbAffectedTypes.Name = "gbAffectedTypes";
            this.gbAffectedTypes.Size = new System.Drawing.Size(409, 65);
            this.gbAffectedTypes.TabIndex = 7;
            this.gbAffectedTypes.TabStop = false;
            this.gbAffectedTypes.Text = "Affected types";
            // 
            // rbAffectEveryone
            // 
            this.rbAffectEveryone.AutoSize = true;
            this.rbAffectEveryone.Location = new System.Drawing.Point(333, 42);
            this.rbAffectEveryone.Name = "rbAffectEveryone";
            this.rbAffectEveryone.Size = new System.Drawing.Size(70, 17);
            this.rbAffectEveryone.TabIndex = 16;
            this.rbAffectEveryone.Text = "Everyone";
            this.rbAffectEveryone.UseVisualStyleBackColor = true;
            // 
            // rbAffectAllEnemies
            // 
            this.rbAffectAllEnemies.AutoSize = true;
            this.rbAffectAllEnemies.Location = new System.Drawing.Point(249, 42);
            this.rbAffectAllEnemies.Name = "rbAffectAllEnemies";
            this.rbAffectAllEnemies.Size = new System.Drawing.Size(78, 17);
            this.rbAffectAllEnemies.TabIndex = 15;
            this.rbAffectAllEnemies.Text = "All enemies";
            this.rbAffectAllEnemies.UseVisualStyleBackColor = true;
            // 
            // rbAffectEnemySquad
            // 
            this.rbAffectEnemySquad.AutoSize = true;
            this.rbAffectEnemySquad.Location = new System.Drawing.Point(140, 42);
            this.rbAffectEnemySquad.Name = "rbAffectEnemySquad";
            this.rbAffectEnemySquad.Size = new System.Drawing.Size(89, 17);
            this.rbAffectEnemySquad.TabIndex = 14;
            this.rbAffectEnemySquad.Text = "Enemy squad";
            this.rbAffectEnemySquad.UseVisualStyleBackColor = true;
            // 
            // rbAffectEnemy
            // 
            this.rbAffectEnemy.AutoSize = true;
            this.rbAffectEnemy.Location = new System.Drawing.Point(140, 19);
            this.rbAffectEnemy.Name = "rbAffectEnemy";
            this.rbAffectEnemy.Size = new System.Drawing.Size(57, 17);
            this.rbAffectEnemy.TabIndex = 13;
            this.rbAffectEnemy.Text = "Enemy";
            this.rbAffectEnemy.UseVisualStyleBackColor = true;
            // 
            // rbAffectAllAllies
            // 
            this.rbAffectAllAllies.AutoSize = true;
            this.rbAffectAllAllies.Location = new System.Drawing.Point(249, 19);
            this.rbAffectAllAllies.Name = "rbAffectAllAllies";
            this.rbAffectAllAllies.Size = new System.Drawing.Size(62, 17);
            this.rbAffectAllAllies.TabIndex = 12;
            this.rbAffectAllAllies.Text = "All allies";
            this.rbAffectAllAllies.UseVisualStyleBackColor = true;
            // 
            // rbAffectAllySquad
            // 
            this.rbAffectAllySquad.AutoSize = true;
            this.rbAffectAllySquad.Location = new System.Drawing.Point(68, 42);
            this.rbAffectAllySquad.Name = "rbAffectAllySquad";
            this.rbAffectAllySquad.Size = new System.Drawing.Size(73, 17);
            this.rbAffectAllySquad.TabIndex = 11;
            this.rbAffectAllySquad.Text = "Ally squad";
            this.rbAffectAllySquad.UseVisualStyleBackColor = true;
            // 
            // rbAffectAlly
            // 
            this.rbAffectAlly.AutoSize = true;
            this.rbAffectAlly.Location = new System.Drawing.Point(68, 19);
            this.rbAffectAlly.Name = "rbAffectAlly";
            this.rbAffectAlly.Size = new System.Drawing.Size(41, 17);
            this.rbAffectAlly.TabIndex = 10;
            this.rbAffectAlly.Text = "Ally";
            this.rbAffectAlly.UseVisualStyleBackColor = true;
            // 
            // rbAffectSelfSquad
            // 
            this.rbAffectSelfSquad.AutoSize = true;
            this.rbAffectSelfSquad.Location = new System.Drawing.Point(6, 42);
            this.rbAffectSelfSquad.Name = "rbAffectSelfSquad";
            this.rbAffectSelfSquad.Size = new System.Drawing.Size(56, 17);
            this.rbAffectSelfSquad.TabIndex = 9;
            this.rbAffectSelfSquad.Text = "Squad";
            this.rbAffectSelfSquad.UseVisualStyleBackColor = true;
            // 
            // rbAffectSelf
            // 
            this.rbAffectSelf.AutoSize = true;
            this.rbAffectSelf.Checked = true;
            this.rbAffectSelf.Location = new System.Drawing.Point(6, 19);
            this.rbAffectSelf.Name = "rbAffectSelf";
            this.rbAffectSelf.Size = new System.Drawing.Size(43, 17);
            this.rbAffectSelf.TabIndex = 8;
            this.rbAffectSelf.TabStop = true;
            this.rbAffectSelf.Text = "Self";
            this.rbAffectSelf.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtRange);
            this.groupBox4.Location = new System.Drawing.Point(427, 184);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(86, 45);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Range";
            // 
            // txtRange
            // 
            this.txtRange.Location = new System.Drawing.Point(6, 19);
            this.txtRange.Mask = "##";
            this.txtRange.Name = "txtRange";
            this.txtRange.PromptChar = ' ';
            this.txtRange.Size = new System.Drawing.Size(74, 20);
            this.txtRange.TabIndex = 0;
            this.txtRange.Text = "-1";
            // 
            // ProjectEternityConsumablePartsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 361);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.gbAffectedTypes);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbLifetimeTypes);
            this.Controls.Add(this.gbEffectInformation);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ProjectEternityConsumablePartEditor";
            this.Text = "Project Eternity Consumable Part Editor";
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbEffectInformation.ResumeLayout(false);
            this.gbLifetimeTypes.ResumeLayout(false);
            this.gbLifetimeTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbAffectedTypes.ResumeLayout(false);
            this.gbAffectedTypes.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ListView lvEffects;
        private System.Windows.Forms.ComboBox cboEffectType;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox gbEffectInformation;
        private System.Windows.Forms.PropertyGrid pgEffect;
        private System.Windows.Forms.GroupBox gbLifetimeTypes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbLifetimeStacking;
        private System.Windows.Forms.RadioButton rbLifetimeBattle;
        private System.Windows.Forms.RadioButton rbLifetimeTurns;
        private System.Windows.Forms.RadioButton rbLifetimePermanent;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.GroupBox gbAffectedTypes;
        private System.Windows.Forms.RadioButton rbAffectSelf;
        private System.Windows.Forms.RadioButton rbAffectSelfSquad;
        private System.Windows.Forms.RadioButton rbAffectAllySquad;
        private System.Windows.Forms.RadioButton rbAffectAlly;
        private System.Windows.Forms.RadioButton rbAffectAllAllies;
        private System.Windows.Forms.RadioButton rbAffectEnemySquad;
        private System.Windows.Forms.RadioButton rbAffectEnemy;
        private System.Windows.Forms.RadioButton rbAffectAllEnemies;
        private System.Windows.Forms.RadioButton rbAffectEveryone;
        private System.Windows.Forms.RadioButton rbLifetimeOnHit;
        private System.Windows.Forms.RadioButton rbLifetimeOnEnemyHit;
        private System.Windows.Forms.RadioButton rbLifetimeOnAttack;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.MaskedTextBox txtRange;
        private System.Windows.Forms.RadioButton rbLifetimeOnAction;
        private System.Windows.Forms.RadioButton rbLifetimeOnEnemyAttack;
        private System.Windows.Forms.NumericUpDown txtMaximumStack;
        private System.Windows.Forms.NumericUpDown txtMaximumLifetime;
    }
}