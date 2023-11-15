namespace ProjectEternity.Editors.CardEditor
{
    partial class SpellCardEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbCardInformation = new System.Windows.Forms.GroupBox();
            this.txtCardSacrificed = new System.Windows.Forms.NumericUpDown();
            this.lblCardSacrificed = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.txtActivationAnimation = new System.Windows.Forms.TextBox();
            this.lblActivationAnimation = new System.Windows.Forms.Label();
            this.btnSetActivationAnimation = new System.Windows.Forms.Button();
            this.lblSkill = new System.Windows.Forms.Label();
            this.txtSkill = new System.Windows.Forms.TextBox();
            this.btnSetSkill = new System.Windows.Forms.Button();
            this.cboRarity = new System.Windows.Forms.ComboBox();
            this.lblRarity = new System.Windows.Forms.Label();
            this.txtMagicCost = new System.Windows.Forms.NumericUpDown();
            this.lblMagicCost = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.gbDescription = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.gbTags = new System.Windows.Forms.GroupBox();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.cbDoublecast = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.gbCardInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardSacrificed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).BeginInit();
            this.gbDescription.SuspendLayout();
            this.gbTags.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(434, 24);
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
            // gbCardInformation
            // 
            this.gbCardInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbCardInformation.Controls.Add(this.cbDoublecast);
            this.gbCardInformation.Controls.Add(this.txtCardSacrificed);
            this.gbCardInformation.Controls.Add(this.lblCardSacrificed);
            this.gbCardInformation.Controls.Add(this.cboType);
            this.gbCardInformation.Controls.Add(this.lblType);
            this.gbCardInformation.Controls.Add(this.txtActivationAnimation);
            this.gbCardInformation.Controls.Add(this.lblActivationAnimation);
            this.gbCardInformation.Controls.Add(this.btnSetActivationAnimation);
            this.gbCardInformation.Controls.Add(this.lblSkill);
            this.gbCardInformation.Controls.Add(this.txtSkill);
            this.gbCardInformation.Controls.Add(this.btnSetSkill);
            this.gbCardInformation.Controls.Add(this.cboRarity);
            this.gbCardInformation.Controls.Add(this.lblRarity);
            this.gbCardInformation.Controls.Add(this.txtMagicCost);
            this.gbCardInformation.Controls.Add(this.lblMagicCost);
            this.gbCardInformation.Controls.Add(this.txtName);
            this.gbCardInformation.Controls.Add(this.lblName);
            this.gbCardInformation.Location = new System.Drawing.Point(12, 27);
            this.gbCardInformation.Name = "gbCardInformation";
            this.gbCardInformation.Size = new System.Drawing.Size(204, 300);
            this.gbCardInformation.TabIndex = 29;
            this.gbCardInformation.TabStop = false;
            this.gbCardInformation.Text = "Card Information";
            // 
            // txtCardSacrificed
            // 
            this.txtCardSacrificed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCardSacrificed.Location = new System.Drawing.Point(130, 71);
            this.txtCardSacrificed.Name = "txtCardSacrificed";
            this.txtCardSacrificed.Size = new System.Drawing.Size(68, 20);
            this.txtCardSacrificed.TabIndex = 54;
            // 
            // lblCardSacrificed
            // 
            this.lblCardSacrificed.AutoSize = true;
            this.lblCardSacrificed.Location = new System.Drawing.Point(6, 73);
            this.lblCardSacrificed.Name = "lblCardSacrificed";
            this.lblCardSacrificed.Size = new System.Drawing.Size(82, 13);
            this.lblCardSacrificed.TabIndex = 55;
            this.lblCardSacrificed.Text = "Card Sacrificed:";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Single Flash",
            "Multi Flash",
            "Single Enchant",
            "Multi Enchant"});
            this.cboType.Location = new System.Drawing.Point(75, 124);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(123, 21);
            this.cboType.TabIndex = 3;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(6, 127);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(34, 13);
            this.lblType.TabIndex = 49;
            this.lblType.Text = "Type:";
            // 
            // txtActivationAnimation
            // 
            this.txtActivationAnimation.Location = new System.Drawing.Point(6, 273);
            this.txtActivationAnimation.Name = "txtActivationAnimation";
            this.txtActivationAnimation.Size = new System.Drawing.Size(148, 20);
            this.txtActivationAnimation.TabIndex = 48;
            this.txtActivationAnimation.TabStop = false;
            // 
            // lblActivationAnimation
            // 
            this.lblActivationAnimation.AutoSize = true;
            this.lblActivationAnimation.Location = new System.Drawing.Point(6, 257);
            this.lblActivationAnimation.Name = "lblActivationAnimation";
            this.lblActivationAnimation.Size = new System.Drawing.Size(106, 13);
            this.lblActivationAnimation.TabIndex = 47;
            this.lblActivationAnimation.Text = "Activation Animation:";
            // 
            // btnSetActivationAnimation
            // 
            this.btnSetActivationAnimation.Location = new System.Drawing.Point(160, 271);
            this.btnSetActivationAnimation.Name = "btnSetActivationAnimation";
            this.btnSetActivationAnimation.Size = new System.Drawing.Size(38, 23);
            this.btnSetActivationAnimation.TabIndex = 46;
            this.btnSetActivationAnimation.TabStop = false;
            this.btnSetActivationAnimation.Text = "Set";
            this.btnSetActivationAnimation.UseVisualStyleBackColor = true;
            this.btnSetActivationAnimation.Click += new System.EventHandler(this.btnSetActivationAnimation_Click);
            // 
            // lblSkill
            // 
            this.lblSkill.AutoSize = true;
            this.lblSkill.Location = new System.Drawing.Point(6, 189);
            this.lblSkill.Name = "lblSkill";
            this.lblSkill.Size = new System.Drawing.Size(29, 13);
            this.lblSkill.TabIndex = 45;
            this.lblSkill.Text = "Skill:";
            // 
            // txtSkill
            // 
            this.txtSkill.Location = new System.Drawing.Point(6, 205);
            this.txtSkill.Name = "txtSkill";
            this.txtSkill.Size = new System.Drawing.Size(192, 20);
            this.txtSkill.TabIndex = 44;
            this.txtSkill.TabStop = false;
            // 
            // btnSetSkill
            // 
            this.btnSetSkill.Location = new System.Drawing.Point(6, 231);
            this.btnSetSkill.Name = "btnSetSkill";
            this.btnSetSkill.Size = new System.Drawing.Size(130, 23);
            this.btnSetSkill.TabIndex = 43;
            this.btnSetSkill.TabStop = false;
            this.btnSetSkill.Text = "Set Skill";
            this.btnSetSkill.UseVisualStyleBackColor = true;
            this.btnSetSkill.Click += new System.EventHandler(this.btnSetSkill_Click);
            // 
            // cboRarity
            // 
            this.cboRarity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRarity.FormattingEnabled = true;
            this.cboRarity.Items.AddRange(new object[] {
            "Normal",
            "Strange",
            "Rare",
            "Extra"});
            this.cboRarity.Location = new System.Drawing.Point(75, 97);
            this.cboRarity.Name = "cboRarity";
            this.cboRarity.Size = new System.Drawing.Size(123, 21);
            this.cboRarity.TabIndex = 2;
            // 
            // lblRarity
            // 
            this.lblRarity.AutoSize = true;
            this.lblRarity.Location = new System.Drawing.Point(6, 100);
            this.lblRarity.Name = "lblRarity";
            this.lblRarity.Size = new System.Drawing.Size(37, 13);
            this.lblRarity.TabIndex = 34;
            this.lblRarity.Text = "Rarity:";
            // 
            // txtMagicCost
            // 
            this.txtMagicCost.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtMagicCost.Location = new System.Drawing.Point(75, 45);
            this.txtMagicCost.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMagicCost.Name = "txtMagicCost";
            this.txtMagicCost.Size = new System.Drawing.Size(123, 20);
            this.txtMagicCost.TabIndex = 1;
            // 
            // lblMagicCost
            // 
            this.lblMagicCost.AutoSize = true;
            this.lblMagicCost.Location = new System.Drawing.Point(6, 47);
            this.lblMagicCost.Name = "lblMagicCost";
            this.lblMagicCost.Size = new System.Drawing.Size(63, 13);
            this.lblMagicCost.TabIndex = 32;
            this.lblMagicCost.Text = "Magic Cost:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(148, 20);
            this.txtName.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 28;
            this.lblName.Text = "Name:";
            // 
            // gbDescription
            // 
            this.gbDescription.Controls.Add(this.txtDescription);
            this.gbDescription.Location = new System.Drawing.Point(222, 27);
            this.gbDescription.Name = "gbDescription";
            this.gbDescription.Size = new System.Drawing.Size(200, 154);
            this.gbDescription.TabIndex = 33;
            this.gbDescription.TabStop = false;
            this.gbDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(192, 129);
            this.txtDescription.TabIndex = 4;
            // 
            // gbTags
            // 
            this.gbTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTags.Controls.Add(this.txtTags);
            this.gbTags.Location = new System.Drawing.Point(222, 187);
            this.gbTags.Name = "gbTags";
            this.gbTags.Size = new System.Drawing.Size(200, 140);
            this.gbTags.TabIndex = 34;
            this.gbTags.TabStop = false;
            this.gbTags.Text = "Tags";
            // 
            // txtTags
            // 
            this.txtTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTags.Location = new System.Drawing.Point(3, 16);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(194, 121);
            this.txtTags.TabIndex = 0;
            // 
            // cbDoublecast
            // 
            this.cbDoublecast.AutoSize = true;
            this.cbDoublecast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbDoublecast.Location = new System.Drawing.Point(8, 151);
            this.cbDoublecast.Name = "cbDoublecast";
            this.cbDoublecast.Size = new System.Drawing.Size(80, 17);
            this.cbDoublecast.TabIndex = 56;
            this.cbDoublecast.Text = "Doublecast";
            this.cbDoublecast.UseVisualStyleBackColor = true;
            // 
            // SpellCardEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 339);
            this.Controls.Add(this.gbTags);
            this.Controls.Add(this.gbDescription);
            this.Controls.Add(this.gbCardInformation);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SpellCardEditor";
            this.Text = "Item Card Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbCardInformation.ResumeLayout(false);
            this.gbCardInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCardSacrificed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).EndInit();
            this.gbDescription.ResumeLayout(false);
            this.gbDescription.PerformLayout();
            this.gbTags.ResumeLayout(false);
            this.gbTags.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbCardInformation;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox txtActivationAnimation;
        private System.Windows.Forms.Label lblActivationAnimation;
        private System.Windows.Forms.Button btnSetActivationAnimation;
        private System.Windows.Forms.Label lblSkill;
        private System.Windows.Forms.TextBox txtSkill;
        private System.Windows.Forms.Button btnSetSkill;
        private System.Windows.Forms.ComboBox cboRarity;
        private System.Windows.Forms.Label lblRarity;
        private System.Windows.Forms.NumericUpDown txtMagicCost;
        private System.Windows.Forms.Label lblMagicCost;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.GroupBox gbDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.GroupBox gbTags;
        public System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.NumericUpDown txtCardSacrificed;
        private System.Windows.Forms.Label lblCardSacrificed;
        private System.Windows.Forms.CheckBox cbDoublecast;
    }
}