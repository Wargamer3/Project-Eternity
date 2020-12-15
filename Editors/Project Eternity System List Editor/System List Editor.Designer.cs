namespace ProjectEternity.Editors.SystemListEditor
{
    partial class SystemListEditor
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
            this.lstParts = new System.Windows.Forms.ListBox();
            this.btnAddPart = new System.Windows.Forms.Button();
            this.gbParts = new System.Windows.Forms.GroupBox();
            this.btnMoveDownPart = new System.Windows.Forms.Button();
            this.btnMoveUpPart = new System.Windows.Forms.Button();
            this.btnRemoveParts = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbBuyableSkills = new System.Windows.Forms.GroupBox();
            this.btnMoveDownBuyableSkill = new System.Windows.Forms.Button();
            this.btnMoveUpBuyableSkill = new System.Windows.Forms.Button();
            this.btnRemoveBuyableSkill = new System.Windows.Forms.Button();
            this.lstBuyableSkills = new System.Windows.Forms.ListBox();
            this.btnAddBuyableSkill = new System.Windows.Forms.Button();
            this.gbSpirits = new System.Windows.Forms.GroupBox();
            this.btnMoveDownSpirit = new System.Windows.Forms.Button();
            this.btnMoveUpSpirit = new System.Windows.Forms.Button();
            this.btnRemoveSpirit = new System.Windows.Forms.Button();
            this.lstSpirits = new System.Windows.Forms.ListBox();
            this.btnAddSpirit = new System.Windows.Forms.Button();
            this.gbSkills = new System.Windows.Forms.GroupBox();
            this.btnMoveDownSkill = new System.Windows.Forms.Button();
            this.btnMoveUpSkill = new System.Windows.Forms.Button();
            this.btnRemoveSkill = new System.Windows.Forms.Button();
            this.lstSkills = new System.Windows.Forms.ListBox();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.gbAbilities = new System.Windows.Forms.GroupBox();
            this.btnMoveDownAbility = new System.Windows.Forms.Button();
            this.btnMoveUpAbility = new System.Windows.Forms.Button();
            this.btnRemoveAbility = new System.Windows.Forms.Button();
            this.lstAbilities = new System.Windows.Forms.ListBox();
            this.btnAddAbility = new System.Windows.Forms.Button();
            this.gbParts.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbBuyableSkills.SuspendLayout();
            this.gbSpirits.SuspendLayout();
            this.gbSkills.SuspendLayout();
            this.gbAbilities.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstParts
            // 
            this.lstParts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstParts.FormattingEnabled = true;
            this.lstParts.Location = new System.Drawing.Point(6, 19);
            this.lstParts.Name = "lstParts";
            this.lstParts.Size = new System.Drawing.Size(171, 290);
            this.lstParts.TabIndex = 0;
            // 
            // btnAddPart
            // 
            this.btnAddPart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddPart.Location = new System.Drawing.Point(6, 315);
            this.btnAddPart.Name = "btnAddPart";
            this.btnAddPart.Size = new System.Drawing.Size(171, 23);
            this.btnAddPart.TabIndex = 2;
            this.btnAddPart.Text = "Add Part";
            this.btnAddPart.UseVisualStyleBackColor = true;
            this.btnAddPart.Click += new System.EventHandler(this.btnAddPart_Click);
            // 
            // gbParts
            // 
            this.gbParts.Controls.Add(this.btnMoveDownPart);
            this.gbParts.Controls.Add(this.btnMoveUpPart);
            this.gbParts.Controls.Add(this.btnRemoveParts);
            this.gbParts.Controls.Add(this.lstParts);
            this.gbParts.Controls.Add(this.btnAddPart);
            this.gbParts.Location = new System.Drawing.Point(12, 27);
            this.gbParts.Name = "gbParts";
            this.gbParts.Size = new System.Drawing.Size(183, 427);
            this.gbParts.TabIndex = 3;
            this.gbParts.TabStop = false;
            this.gbParts.Text = "Parts";
            // 
            // btnMoveDownPart
            // 
            this.btnMoveDownPart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownPart.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownPart.Name = "btnMoveDownPart";
            this.btnMoveDownPart.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownPart.TabIndex = 5;
            this.btnMoveDownPart.Text = "Move Down Part";
            this.btnMoveDownPart.UseVisualStyleBackColor = true;
            this.btnMoveDownPart.Click += new System.EventHandler(this.btnMoveDownPart_Click);
            // 
            // btnMoveUpPart
            // 
            this.btnMoveUpPart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpPart.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpPart.Name = "btnMoveUpPart";
            this.btnMoveUpPart.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpPart.TabIndex = 4;
            this.btnMoveUpPart.Text = "Move Up Part";
            this.btnMoveUpPart.UseVisualStyleBackColor = true;
            this.btnMoveUpPart.Click += new System.EventHandler(this.btnMoveUpPart_Click);
            // 
            // btnRemoveParts
            // 
            this.btnRemoveParts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveParts.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveParts.Name = "btnRemoveParts";
            this.btnRemoveParts.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveParts.TabIndex = 3;
            this.btnRemoveParts.Text = "Remove Part";
            this.btnRemoveParts.UseVisualStyleBackColor = true;
            this.btnRemoveParts.Click += new System.EventHandler(this.btnRemoveParts_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(953, 24);
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
            // gbBuyableSkills
            // 
            this.gbBuyableSkills.Controls.Add(this.btnMoveDownBuyableSkill);
            this.gbBuyableSkills.Controls.Add(this.btnMoveUpBuyableSkill);
            this.gbBuyableSkills.Controls.Add(this.btnRemoveBuyableSkill);
            this.gbBuyableSkills.Controls.Add(this.lstBuyableSkills);
            this.gbBuyableSkills.Controls.Add(this.btnAddBuyableSkill);
            this.gbBuyableSkills.Location = new System.Drawing.Point(201, 27);
            this.gbBuyableSkills.Name = "gbBuyableSkills";
            this.gbBuyableSkills.Size = new System.Drawing.Size(183, 427);
            this.gbBuyableSkills.TabIndex = 6;
            this.gbBuyableSkills.TabStop = false;
            this.gbBuyableSkills.Text = "Buyable Skills";
            // 
            // btnMoveDownBuyableSkill
            // 
            this.btnMoveDownBuyableSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownBuyableSkill.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownBuyableSkill.Name = "btnMoveDownBuyableSkill";
            this.btnMoveDownBuyableSkill.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownBuyableSkill.TabIndex = 5;
            this.btnMoveDownBuyableSkill.Text = "Move Down Buyable Skill";
            this.btnMoveDownBuyableSkill.UseVisualStyleBackColor = true;
            this.btnMoveDownBuyableSkill.Click += new System.EventHandler(this.btnMoveDownBuyableSkill_Click);
            // 
            // btnMoveUpBuyableSkill
            // 
            this.btnMoveUpBuyableSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpBuyableSkill.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpBuyableSkill.Name = "btnMoveUpBuyableSkill";
            this.btnMoveUpBuyableSkill.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpBuyableSkill.TabIndex = 4;
            this.btnMoveUpBuyableSkill.Text = "Move Up Buyable Skill";
            this.btnMoveUpBuyableSkill.UseVisualStyleBackColor = true;
            this.btnMoveUpBuyableSkill.Click += new System.EventHandler(this.btnMoveUpBuyableSkill_Click);
            // 
            // btnRemoveBuyableSkill
            // 
            this.btnRemoveBuyableSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBuyableSkill.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveBuyableSkill.Name = "btnRemoveBuyableSkill";
            this.btnRemoveBuyableSkill.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveBuyableSkill.TabIndex = 3;
            this.btnRemoveBuyableSkill.Text = "Remove Buyable Skill";
            this.btnRemoveBuyableSkill.UseVisualStyleBackColor = true;
            this.btnRemoveBuyableSkill.Click += new System.EventHandler(this.btnRemoveBuyableSkill_Click);
            // 
            // lstBuyableSkills
            // 
            this.lstBuyableSkills.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBuyableSkills.FormattingEnabled = true;
            this.lstBuyableSkills.Location = new System.Drawing.Point(6, 19);
            this.lstBuyableSkills.Name = "lstBuyableSkills";
            this.lstBuyableSkills.Size = new System.Drawing.Size(171, 290);
            this.lstBuyableSkills.TabIndex = 0;
            // 
            // btnAddBuyableSkill
            // 
            this.btnAddBuyableSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBuyableSkill.Location = new System.Drawing.Point(6, 315);
            this.btnAddBuyableSkill.Name = "btnAddBuyableSkill";
            this.btnAddBuyableSkill.Size = new System.Drawing.Size(171, 23);
            this.btnAddBuyableSkill.TabIndex = 2;
            this.btnAddBuyableSkill.Text = "Add Buyable Skill";
            this.btnAddBuyableSkill.UseVisualStyleBackColor = true;
            this.btnAddBuyableSkill.Click += new System.EventHandler(this.btnAddBuyableSkill_Click);
            // 
            // gbSpirits
            // 
            this.gbSpirits.Controls.Add(this.btnMoveDownSpirit);
            this.gbSpirits.Controls.Add(this.btnMoveUpSpirit);
            this.gbSpirits.Controls.Add(this.btnRemoveSpirit);
            this.gbSpirits.Controls.Add(this.lstSpirits);
            this.gbSpirits.Controls.Add(this.btnAddSpirit);
            this.gbSpirits.Location = new System.Drawing.Point(384, 27);
            this.gbSpirits.Name = "gbSpirits";
            this.gbSpirits.Size = new System.Drawing.Size(183, 427);
            this.gbSpirits.TabIndex = 6;
            this.gbSpirits.TabStop = false;
            this.gbSpirits.Text = "Spirits (Search)";
            // 
            // btnMoveDownSpirit
            // 
            this.btnMoveDownSpirit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownSpirit.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownSpirit.Name = "btnMoveDownSpirit";
            this.btnMoveDownSpirit.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownSpirit.TabIndex = 5;
            this.btnMoveDownSpirit.Text = "Move Down Spirit";
            this.btnMoveDownSpirit.UseVisualStyleBackColor = true;
            this.btnMoveDownSpirit.Click += new System.EventHandler(this.btnMoveDownSpirit_Click);
            // 
            // btnMoveUpSpirit
            // 
            this.btnMoveUpSpirit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpSpirit.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpSpirit.Name = "btnMoveUpSpirit";
            this.btnMoveUpSpirit.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpSpirit.TabIndex = 4;
            this.btnMoveUpSpirit.Text = "Move Up Spirit";
            this.btnMoveUpSpirit.UseVisualStyleBackColor = true;
            this.btnMoveUpSpirit.Click += new System.EventHandler(this.btnMoveUpSpirit_Click);
            // 
            // btnRemoveSpirit
            // 
            this.btnRemoveSpirit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSpirit.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveSpirit.Name = "btnRemoveSpirit";
            this.btnRemoveSpirit.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveSpirit.TabIndex = 3;
            this.btnRemoveSpirit.Text = "Remove Spirit";
            this.btnRemoveSpirit.UseVisualStyleBackColor = true;
            this.btnRemoveSpirit.Click += new System.EventHandler(this.btnRemoveSpirit_Click);
            // 
            // lstSpirits
            // 
            this.lstSpirits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSpirits.FormattingEnabled = true;
            this.lstSpirits.Location = new System.Drawing.Point(6, 19);
            this.lstSpirits.Name = "lstSpirits";
            this.lstSpirits.Size = new System.Drawing.Size(171, 290);
            this.lstSpirits.TabIndex = 0;
            // 
            // btnAddSpirit
            // 
            this.btnAddSpirit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSpirit.Location = new System.Drawing.Point(6, 315);
            this.btnAddSpirit.Name = "btnAddSpirit";
            this.btnAddSpirit.Size = new System.Drawing.Size(171, 23);
            this.btnAddSpirit.TabIndex = 2;
            this.btnAddSpirit.Text = "Add Spirit";
            this.btnAddSpirit.UseVisualStyleBackColor = true;
            this.btnAddSpirit.Click += new System.EventHandler(this.btnAddSpirit_Click);
            // 
            // gbSkills
            // 
            this.gbSkills.Controls.Add(this.btnMoveDownSkill);
            this.gbSkills.Controls.Add(this.btnMoveUpSkill);
            this.gbSkills.Controls.Add(this.btnRemoveSkill);
            this.gbSkills.Controls.Add(this.lstSkills);
            this.gbSkills.Controls.Add(this.btnAddSkill);
            this.gbSkills.Location = new System.Drawing.Point(573, 27);
            this.gbSkills.Name = "gbSkills";
            this.gbSkills.Size = new System.Drawing.Size(183, 427);
            this.gbSkills.TabIndex = 7;
            this.gbSkills.TabStop = false;
            this.gbSkills.Text = "Skills (Search)";
            // 
            // btnMoveDownSkill
            // 
            this.btnMoveDownSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownSkill.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownSkill.Name = "btnMoveDownSkill";
            this.btnMoveDownSkill.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownSkill.TabIndex = 5;
            this.btnMoveDownSkill.Text = "Move Down Skill";
            this.btnMoveDownSkill.UseVisualStyleBackColor = true;
            this.btnMoveDownSkill.Click += new System.EventHandler(this.btnMoveDownSkill_Click);
            // 
            // btnMoveUpSkill
            // 
            this.btnMoveUpSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpSkill.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpSkill.Name = "btnMoveUpSkill";
            this.btnMoveUpSkill.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpSkill.TabIndex = 4;
            this.btnMoveUpSkill.Text = "Move Up Skill";
            this.btnMoveUpSkill.UseVisualStyleBackColor = true;
            this.btnMoveUpSkill.Click += new System.EventHandler(this.btnMoveUpSkill_Click);
            // 
            // btnRemoveSkill
            // 
            this.btnRemoveSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveSkill.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveSkill.Name = "btnRemoveSkill";
            this.btnRemoveSkill.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveSkill.TabIndex = 3;
            this.btnRemoveSkill.Text = "Remove Skill";
            this.btnRemoveSkill.UseVisualStyleBackColor = true;
            this.btnRemoveSkill.Click += new System.EventHandler(this.btnRemoveSkill_Click);
            // 
            // lstSkills
            // 
            this.lstSkills.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstSkills.FormattingEnabled = true;
            this.lstSkills.Location = new System.Drawing.Point(6, 19);
            this.lstSkills.Name = "lstSkills";
            this.lstSkills.Size = new System.Drawing.Size(171, 290);
            this.lstSkills.TabIndex = 0;
            // 
            // btnAddSkill
            // 
            this.btnAddSkill.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddSkill.Location = new System.Drawing.Point(6, 315);
            this.btnAddSkill.Name = "btnAddSkill";
            this.btnAddSkill.Size = new System.Drawing.Size(171, 23);
            this.btnAddSkill.TabIndex = 2;
            this.btnAddSkill.Text = "Add Skill";
            this.btnAddSkill.UseVisualStyleBackColor = true;
            this.btnAddSkill.Click += new System.EventHandler(this.btnAddSkill_Click);
            // 
            // gbAbilities
            // 
            this.gbAbilities.Controls.Add(this.btnMoveDownAbility);
            this.gbAbilities.Controls.Add(this.btnMoveUpAbility);
            this.gbAbilities.Controls.Add(this.btnRemoveAbility);
            this.gbAbilities.Controls.Add(this.lstAbilities);
            this.gbAbilities.Controls.Add(this.btnAddAbility);
            this.gbAbilities.Location = new System.Drawing.Point(762, 27);
            this.gbAbilities.Name = "gbAbilities";
            this.gbAbilities.Size = new System.Drawing.Size(183, 427);
            this.gbAbilities.TabIndex = 8;
            this.gbAbilities.TabStop = false;
            this.gbAbilities.Text = "Abilities (Search)";
            // 
            // btnMoveDownAbility
            // 
            this.btnMoveDownAbility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownAbility.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownAbility.Name = "btnMoveDownAbility";
            this.btnMoveDownAbility.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownAbility.TabIndex = 5;
            this.btnMoveDownAbility.Text = "Move Down Ability";
            this.btnMoveDownAbility.UseVisualStyleBackColor = true;
            this.btnMoveDownAbility.Click += new System.EventHandler(this.btnMoveDownAbility_Click);
            // 
            // btnMoveUpAbility
            // 
            this.btnMoveUpAbility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpAbility.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpAbility.Name = "btnMoveUpAbility";
            this.btnMoveUpAbility.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpAbility.TabIndex = 4;
            this.btnMoveUpAbility.Text = "Move Up Ability";
            this.btnMoveUpAbility.UseVisualStyleBackColor = true;
            this.btnMoveUpAbility.Click += new System.EventHandler(this.btnMoveUpAbility_Click);
            // 
            // btnRemoveAbility
            // 
            this.btnRemoveAbility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveAbility.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveAbility.Name = "btnRemoveAbility";
            this.btnRemoveAbility.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveAbility.TabIndex = 3;
            this.btnRemoveAbility.Text = "Remove Ability";
            this.btnRemoveAbility.UseVisualStyleBackColor = true;
            this.btnRemoveAbility.Click += new System.EventHandler(this.btnRemoveAbility_Click);
            // 
            // lstAbilities
            // 
            this.lstAbilities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAbilities.FormattingEnabled = true;
            this.lstAbilities.Location = new System.Drawing.Point(6, 19);
            this.lstAbilities.Name = "lstAbilities";
            this.lstAbilities.Size = new System.Drawing.Size(171, 290);
            this.lstAbilities.TabIndex = 0;
            // 
            // btnAddAbility
            // 
            this.btnAddAbility.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAbility.Location = new System.Drawing.Point(6, 315);
            this.btnAddAbility.Name = "btnAddAbility";
            this.btnAddAbility.Size = new System.Drawing.Size(171, 23);
            this.btnAddAbility.TabIndex = 2;
            this.btnAddAbility.Text = "Add Ability";
            this.btnAddAbility.UseVisualStyleBackColor = true;
            this.btnAddAbility.Click += new System.EventHandler(this.btnAddAbility_Click);
            // 
            // SystemListEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 466);
            this.Controls.Add(this.gbAbilities);
            this.Controls.Add(this.gbSkills);
            this.Controls.Add(this.gbSpirits);
            this.Controls.Add(this.gbBuyableSkills);
            this.Controls.Add(this.gbParts);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SystemListEditor";
            this.Text = "System List Editor";
            this.gbParts.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbBuyableSkills.ResumeLayout(false);
            this.gbSpirits.ResumeLayout(false);
            this.gbSkills.ResumeLayout(false);
            this.gbAbilities.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstParts;
        private System.Windows.Forms.Button btnAddPart;
        private System.Windows.Forms.GroupBox gbParts;
        private System.Windows.Forms.Button btnRemoveParts;
        private System.Windows.Forms.Button btnMoveDownPart;
        private System.Windows.Forms.Button btnMoveUpPart;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbBuyableSkills;
        private System.Windows.Forms.Button btnMoveDownBuyableSkill;
        private System.Windows.Forms.Button btnMoveUpBuyableSkill;
        private System.Windows.Forms.Button btnRemoveBuyableSkill;
        private System.Windows.Forms.ListBox lstBuyableSkills;
        private System.Windows.Forms.Button btnAddBuyableSkill;
        private System.Windows.Forms.GroupBox gbSpirits;
        private System.Windows.Forms.Button btnMoveDownSpirit;
        private System.Windows.Forms.Button btnMoveUpSpirit;
        private System.Windows.Forms.Button btnRemoveSpirit;
        private System.Windows.Forms.ListBox lstSpirits;
        private System.Windows.Forms.Button btnAddSpirit;
        private System.Windows.Forms.GroupBox gbSkills;
        private System.Windows.Forms.Button btnMoveDownSkill;
        private System.Windows.Forms.Button btnMoveUpSkill;
        private System.Windows.Forms.Button btnRemoveSkill;
        private System.Windows.Forms.ListBox lstSkills;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.GroupBox gbAbilities;
        private System.Windows.Forms.Button btnMoveDownAbility;
        private System.Windows.Forms.Button btnMoveUpAbility;
        private System.Windows.Forms.Button btnRemoveAbility;
        private System.Windows.Forms.ListBox lstAbilities;
        private System.Windows.Forms.Button btnAddAbility;
    }
}