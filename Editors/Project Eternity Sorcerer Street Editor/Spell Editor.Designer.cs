namespace ProjectEternity.Editors.SorcererStreetSpellEditor
{
    partial class SpellEditor
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
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.btnRemoveEffect = new System.Windows.Forms.Button();
            this.btnAddEffect = new System.Windows.Forms.Button();
            this.lvEffects = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.cboEffectType = new System.Windows.Forms.ComboBox();
            this.gbEffectInformation = new System.Windows.Forms.GroupBox();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.gbLifetimeTypes = new System.Windows.Forms.GroupBox();
            this.txtRange = new System.Windows.Forms.NumericUpDown();
            this.lblMaximumRange = new System.Windows.Forms.Label();
            this.txtMaximumLifetime = new System.Windows.Forms.NumericUpDown();
            this.txtMaximumStack = new System.Windows.Forms.NumericUpDown();
            this.lblMaximumLifetime = new System.Windows.Forms.Label();
            this.lblMaximumStack = new System.Windows.Forms.Label();
            this.cbLifetimeStacking = new System.Windows.Forms.CheckBox();
            this.rbLifetimeBattle = new System.Windows.Forms.RadioButton();
            this.rbLifetimeTurns = new System.Windows.Forms.RadioButton();
            this.rbLifetimePermanent = new System.Windows.Forms.RadioButton();
            this.gbDescription = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.gbTarget = new System.Windows.Forms.GroupBox();
            this.pgTargetType = new System.Windows.Forms.PropertyGrid();
            this.cboTargetType = new System.Windows.Forms.ComboBox();
            this.gbRequirementInformation = new System.Windows.Forms.GroupBox();
            this.pgRequirement = new System.Windows.Forms.PropertyGrid();
            this.cboRequirementType = new System.Windows.Forms.ComboBox();
            this.gbRequirements = new System.Windows.Forms.GroupBox();
            this.btnRemoveRequirement = new System.Windows.Forms.Button();
            this.btnAddRequirement = new System.Windows.Forms.Button();
            this.lvRequirements = new System.Windows.Forms.ListView();
            this.rbCastleReached = new System.Windows.Forms.RadioButton();
            this.gbEffects.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbEffectInformation.SuspendLayout();
            this.gbLifetimeTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).BeginInit();
            this.gbDescription.SuspendLayout();
            this.gbTarget.SuspendLayout();
            this.gbRequirementInformation.SuspendLayout();
            this.gbRequirements.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.btnRemoveEffect);
            this.gbEffects.Controls.Add(this.btnAddEffect);
            this.gbEffects.Controls.Add(this.lvEffects);
            this.gbEffects.Location = new System.Drawing.Point(424, 33);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(203, 144);
            this.gbEffects.TabIndex = 0;
            this.gbEffects.TabStop = false;
            this.gbEffects.Text = "Effects";
            // 
            // btnRemoveEffect
            // 
            this.btnRemoveEffect.Location = new System.Drawing.Point(122, 111);
            this.btnRemoveEffect.Name = "btnRemoveEffect";
            this.btnRemoveEffect.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveEffect.TabIndex = 2;
            this.btnRemoveEffect.Text = "Remove";
            this.btnRemoveEffect.UseVisualStyleBackColor = true;
            this.btnRemoveEffect.Click += new System.EventHandler(this.btnRemoveEffect_Click);
            // 
            // btnAddEffect
            // 
            this.btnAddEffect.Location = new System.Drawing.Point(6, 111);
            this.btnAddEffect.Name = "btnAddEffect";
            this.btnAddEffect.Size = new System.Drawing.Size(75, 23);
            this.btnAddEffect.TabIndex = 2;
            this.btnAddEffect.Text = "Add";
            this.btnAddEffect.UseVisualStyleBackColor = true;
            this.btnAddEffect.Click += new System.EventHandler(this.btnAddEffect_Click);
            // 
            // lvEffects
            // 
            this.lvEffects.HideSelection = false;
            this.lvEffects.Location = new System.Drawing.Point(6, 19);
            this.lvEffects.MultiSelect = false;
            this.lvEffects.Name = "lvEffects";
            this.lvEffects.Size = new System.Drawing.Size(191, 86);
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
            this.menuStrip1.Size = new System.Drawing.Size(848, 24);
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
            this.gbEffectInformation.Location = new System.Drawing.Point(427, 183);
            this.gbEffectInformation.Name = "gbEffectInformation";
            this.gbEffectInformation.Size = new System.Drawing.Size(200, 254);
            this.gbEffectInformation.TabIndex = 3;
            this.gbEffectInformation.TabStop = false;
            this.gbEffectInformation.Text = "Effect information";
            // 
            // pgEffect
            // 
            this.pgEffect.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgEffect.Location = new System.Drawing.Point(6, 46);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(188, 202);
            this.pgEffect.TabIndex = 3;
            // 
            // gbLifetimeTypes
            // 
            this.gbLifetimeTypes.Controls.Add(this.rbCastleReached);
            this.gbLifetimeTypes.Controls.Add(this.txtRange);
            this.gbLifetimeTypes.Controls.Add(this.lblMaximumRange);
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumLifetime);
            this.gbLifetimeTypes.Controls.Add(this.txtMaximumStack);
            this.gbLifetimeTypes.Controls.Add(this.lblMaximumLifetime);
            this.gbLifetimeTypes.Controls.Add(this.lblMaximumStack);
            this.gbLifetimeTypes.Controls.Add(this.cbLifetimeStacking);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeBattle);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimeTurns);
            this.gbLifetimeTypes.Controls.Add(this.rbLifetimePermanent);
            this.gbLifetimeTypes.Enabled = false;
            this.gbLifetimeTypes.Location = new System.Drawing.Point(633, 27);
            this.gbLifetimeTypes.Name = "gbLifetimeTypes";
            this.gbLifetimeTypes.Size = new System.Drawing.Size(203, 216);
            this.gbLifetimeTypes.TabIndex = 5;
            this.gbLifetimeTypes.TabStop = false;
            this.gbLifetimeTypes.Text = "Lifetime types";
            // 
            // txtRange
            // 
            this.txtRange.Location = new System.Drawing.Point(117, 186);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(80, 20);
            this.txtRange.TabIndex = 19;
            // 
            // lblMaximumRange
            // 
            this.lblMaximumRange.AutoSize = true;
            this.lblMaximumRange.Location = new System.Drawing.Point(111, 170);
            this.lblMaximumRange.Name = "lblMaximumRange";
            this.lblMaximumRange.Size = new System.Drawing.Size(86, 13);
            this.lblMaximumRange.TabIndex = 18;
            this.lblMaximumRange.Text = "Maximum Range";
            // 
            // txtMaximumLifetime
            // 
            this.txtMaximumLifetime.Location = new System.Drawing.Point(6, 124);
            this.txtMaximumLifetime.Name = "txtMaximumLifetime";
            this.txtMaximumLifetime.Size = new System.Drawing.Size(80, 20);
            this.txtMaximumLifetime.TabIndex = 17;
            this.txtMaximumLifetime.ValueChanged += new System.EventHandler(this.txtMaximumLifetime_TextChanged);
            // 
            // txtMaximumStack
            // 
            this.txtMaximumStack.Location = new System.Drawing.Point(6, 186);
            this.txtMaximumStack.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMaximumStack.Name = "txtMaximumStack";
            this.txtMaximumStack.Size = new System.Drawing.Size(74, 20);
            this.txtMaximumStack.TabIndex = 16;
            this.txtMaximumStack.ValueChanged += new System.EventHandler(this.txtMaximumStack_ValueChanged);
            // 
            // lblMaximumLifetime
            // 
            this.lblMaximumLifetime.AutoSize = true;
            this.lblMaximumLifetime.Location = new System.Drawing.Point(6, 108);
            this.lblMaximumLifetime.Name = "lblMaximumLifetime";
            this.lblMaximumLifetime.Size = new System.Drawing.Size(86, 13);
            this.lblMaximumLifetime.TabIndex = 7;
            this.lblMaximumLifetime.Text = "Maximum lifetime";
            // 
            // lblMaximumStack
            // 
            this.lblMaximumStack.AutoSize = true;
            this.lblMaximumStack.Location = new System.Drawing.Point(6, 170);
            this.lblMaximumStack.Name = "lblMaximumStack";
            this.lblMaximumStack.Size = new System.Drawing.Size(86, 13);
            this.lblMaximumStack.TabIndex = 5;
            this.lblMaximumStack.Text = "Maxiumum stack";
            // 
            // cbLifetimeStacking
            // 
            this.cbLifetimeStacking.AutoSize = true;
            this.cbLifetimeStacking.Location = new System.Drawing.Point(6, 150);
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
            // gbDescription
            // 
            this.gbDescription.Controls.Add(this.txtDescription);
            this.gbDescription.Location = new System.Drawing.Point(633, 249);
            this.gbDescription.Name = "gbDescription";
            this.gbDescription.Size = new System.Drawing.Size(203, 188);
            this.gbDescription.TabIndex = 6;
            this.gbDescription.TabStop = false;
            this.gbDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(191, 163);
            this.txtDescription.TabIndex = 0;
            // 
            // gbTarget
            // 
            this.gbTarget.Controls.Add(this.pgTargetType);
            this.gbTarget.Controls.Add(this.cboTargetType);
            this.gbTarget.Location = new System.Drawing.Point(12, 27);
            this.gbTarget.Name = "gbTarget";
            this.gbTarget.Size = new System.Drawing.Size(200, 254);
            this.gbTarget.TabIndex = 4;
            this.gbTarget.TabStop = false;
            this.gbTarget.Text = "Target";
            // 
            // pgTargetType
            // 
            this.pgTargetType.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgTargetType.Location = new System.Drawing.Point(6, 46);
            this.pgTargetType.Name = "pgTargetType";
            this.pgTargetType.Size = new System.Drawing.Size(188, 202);
            this.pgTargetType.TabIndex = 3;
            // 
            // cboTargetType
            // 
            this.cboTargetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTargetType.FormattingEnabled = true;
            this.cboTargetType.Location = new System.Drawing.Point(9, 19);
            this.cboTargetType.Name = "cboTargetType";
            this.cboTargetType.Size = new System.Drawing.Size(185, 21);
            this.cboTargetType.TabIndex = 2;
            this.cboTargetType.SelectedIndexChanged += new System.EventHandler(this.cboTargetType_SelectedIndexChanged);
            // 
            // gbRequirementInformation
            // 
            this.gbRequirementInformation.Controls.Add(this.pgRequirement);
            this.gbRequirementInformation.Controls.Add(this.cboRequirementType);
            this.gbRequirementInformation.Enabled = false;
            this.gbRequirementInformation.Location = new System.Drawing.Point(221, 183);
            this.gbRequirementInformation.Name = "gbRequirementInformation";
            this.gbRequirementInformation.Size = new System.Drawing.Size(200, 254);
            this.gbRequirementInformation.TabIndex = 5;
            this.gbRequirementInformation.TabStop = false;
            this.gbRequirementInformation.Text = "Requirement information";
            // 
            // pgRequirement
            // 
            this.pgRequirement.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgRequirement.Location = new System.Drawing.Point(6, 46);
            this.pgRequirement.Name = "pgRequirement";
            this.pgRequirement.Size = new System.Drawing.Size(188, 202);
            this.pgRequirement.TabIndex = 3;
            // 
            // cboRequirementType
            // 
            this.cboRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRequirementType.FormattingEnabled = true;
            this.cboRequirementType.Location = new System.Drawing.Point(9, 19);
            this.cboRequirementType.Name = "cboRequirementType";
            this.cboRequirementType.Size = new System.Drawing.Size(185, 21);
            this.cboRequirementType.TabIndex = 2;
            this.cboRequirementType.SelectedIndexChanged += new System.EventHandler(this.cboRequirementType_SelectedIndexChanged);
            // 
            // gbRequirements
            // 
            this.gbRequirements.Controls.Add(this.btnRemoveRequirement);
            this.gbRequirements.Controls.Add(this.btnAddRequirement);
            this.gbRequirements.Controls.Add(this.lvRequirements);
            this.gbRequirements.Location = new System.Drawing.Point(218, 33);
            this.gbRequirements.Name = "gbRequirements";
            this.gbRequirements.Size = new System.Drawing.Size(203, 144);
            this.gbRequirements.TabIndex = 4;
            this.gbRequirements.TabStop = false;
            this.gbRequirements.Text = "Requirements";
            // 
            // btnRemoveRequirement
            // 
            this.btnRemoveRequirement.Location = new System.Drawing.Point(122, 111);
            this.btnRemoveRequirement.Name = "btnRemoveRequirement";
            this.btnRemoveRequirement.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveRequirement.TabIndex = 2;
            this.btnRemoveRequirement.Text = "Remove";
            this.btnRemoveRequirement.UseVisualStyleBackColor = true;
            this.btnRemoveRequirement.Click += new System.EventHandler(this.btnRemoveRequirement_Click);
            // 
            // btnAddRequirement
            // 
            this.btnAddRequirement.Location = new System.Drawing.Point(6, 111);
            this.btnAddRequirement.Name = "btnAddRequirement";
            this.btnAddRequirement.Size = new System.Drawing.Size(75, 23);
            this.btnAddRequirement.TabIndex = 2;
            this.btnAddRequirement.Text = "Add";
            this.btnAddRequirement.UseVisualStyleBackColor = true;
            this.btnAddRequirement.Click += new System.EventHandler(this.btnAddRequirement_Click);
            // 
            // lvRequirements
            // 
            this.lvRequirements.HideSelection = false;
            this.lvRequirements.Location = new System.Drawing.Point(6, 19);
            this.lvRequirements.MultiSelect = false;
            this.lvRequirements.Name = "lvRequirements";
            this.lvRequirements.Size = new System.Drawing.Size(191, 86);
            this.lvRequirements.TabIndex = 0;
            this.lvRequirements.UseCompatibleStateImageBehavior = false;
            this.lvRequirements.View = System.Windows.Forms.View.List;
            this.lvRequirements.SelectedIndexChanged += new System.EventHandler(this.lvRequirements_SelectedIndexChanged);
            // 
            // rbCastleReached
            // 
            this.rbCastleReached.AutoSize = true;
            this.rbCastleReached.Location = new System.Drawing.Point(88, 19);
            this.rbCastleReached.Name = "rbCastleReached";
            this.rbCastleReached.Size = new System.Drawing.Size(101, 17);
            this.rbCastleReached.TabIndex = 20;
            this.rbCastleReached.TabStop = true;
            this.rbCastleReached.Text = "Castle Reached";
            this.rbCastleReached.UseVisualStyleBackColor = true;
            this.rbCastleReached.CheckedChanged += new System.EventHandler(this.rbCastleReached_CheckedChanged);
            // 
            // SpellEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 449);
            this.Controls.Add(this.gbRequirementInformation);
            this.Controls.Add(this.gbRequirements);
            this.Controls.Add(this.gbTarget);
            this.Controls.Add(this.gbDescription);
            this.Controls.Add(this.gbLifetimeTypes);
            this.Controls.Add(this.gbEffectInformation);
            this.Controls.Add(this.gbEffects);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpellEditor";
            this.Text = "Spell Editor";
            this.gbEffects.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbEffectInformation.ResumeLayout(false);
            this.gbLifetimeTypes.ResumeLayout(false);
            this.gbLifetimeTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumLifetime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaximumStack)).EndInit();
            this.gbDescription.ResumeLayout(false);
            this.gbDescription.PerformLayout();
            this.gbTarget.ResumeLayout(false);
            this.gbRequirementInformation.ResumeLayout(false);
            this.gbRequirements.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbEffects;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ListView lvEffects;
        private System.Windows.Forms.ComboBox cboEffectType;
        private System.Windows.Forms.Button btnRemoveEffect;
        private System.Windows.Forms.Button btnAddEffect;
        private System.Windows.Forms.GroupBox gbEffectInformation;
        private System.Windows.Forms.PropertyGrid pgEffect;
        private System.Windows.Forms.GroupBox gbLifetimeTypes;
        private System.Windows.Forms.Label lblMaximumLifetime;
        private System.Windows.Forms.Label lblMaximumStack;
        private System.Windows.Forms.CheckBox cbLifetimeStacking;
        private System.Windows.Forms.RadioButton rbLifetimeBattle;
        private System.Windows.Forms.RadioButton rbLifetimeTurns;
        private System.Windows.Forms.RadioButton rbLifetimePermanent;
        private System.Windows.Forms.GroupBox gbDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.NumericUpDown txtMaximumStack;
        private System.Windows.Forms.NumericUpDown txtMaximumLifetime;
        private System.Windows.Forms.NumericUpDown txtRange;
        private System.Windows.Forms.Label lblMaximumRange;
        private System.Windows.Forms.GroupBox gbTarget;
        private System.Windows.Forms.PropertyGrid pgTargetType;
        private System.Windows.Forms.ComboBox cboTargetType;
        private System.Windows.Forms.GroupBox gbRequirementInformation;
        private System.Windows.Forms.PropertyGrid pgRequirement;
        private System.Windows.Forms.ComboBox cboRequirementType;
        private System.Windows.Forms.GroupBox gbRequirements;
        private System.Windows.Forms.Button btnRemoveRequirement;
        private System.Windows.Forms.Button btnAddRequirement;
        private System.Windows.Forms.ListView lvRequirements;
        private System.Windows.Forms.RadioButton rbCastleReached;
    }
}

