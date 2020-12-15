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
            this.cboRarity = new System.Windows.Forms.ComboBox();
            this.lblRarity = new System.Windows.Forms.Label();
            this.txtMagicCost = new System.Windows.Forms.NumericUpDown();
            this.lblMagicCost = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.gbSkills = new System.Windows.Forms.GroupBox();
            this.btnRemoveSkill = new System.Windows.Forms.Button();
            this.btnAddSkill = new System.Windows.Forms.Button();
            this.lstSkill = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.gbCardInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).BeginInit();
            this.gbSkills.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(377, 24);
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
            this.gbCardInformation.Controls.Add(this.cboRarity);
            this.gbCardInformation.Controls.Add(this.lblRarity);
            this.gbCardInformation.Controls.Add(this.txtMagicCost);
            this.gbCardInformation.Controls.Add(this.lblMagicCost);
            this.gbCardInformation.Controls.Add(this.lblDescription);
            this.gbCardInformation.Controls.Add(this.txtDescription);
            this.gbCardInformation.Controls.Add(this.txtName);
            this.gbCardInformation.Controls.Add(this.lblName);
            this.gbCardInformation.Location = new System.Drawing.Point(12, 27);
            this.gbCardInformation.Name = "gbCardInformation";
            this.gbCardInformation.Size = new System.Drawing.Size(204, 306);
            this.gbCardInformation.TabIndex = 28;
            this.gbCardInformation.TabStop = false;
            this.gbCardInformation.Text = "Card Information";
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
            this.cboRarity.Location = new System.Drawing.Point(75, 73);
            this.cboRarity.Name = "cboRarity";
            this.cboRarity.Size = new System.Drawing.Size(123, 21);
            this.cboRarity.TabIndex = 35;
            // 
            // lblRarity
            // 
            this.lblRarity.AutoSize = true;
            this.lblRarity.Location = new System.Drawing.Point(6, 76);
            this.lblRarity.Name = "lblRarity";
            this.lblRarity.Size = new System.Drawing.Size(37, 13);
            this.lblRarity.TabIndex = 34;
            this.lblRarity.Text = "Rarity:";
            // 
            // txtMagicCost
            // 
            this.txtMagicCost.Location = new System.Drawing.Point(75, 45);
            this.txtMagicCost.Name = "txtMagicCost";
            this.txtMagicCost.Size = new System.Drawing.Size(123, 20);
            this.txtMagicCost.TabIndex = 33;
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
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 97);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 31;
            this.lblDescription.Text = "Description:";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtDescription.Location = new System.Drawing.Point(6, 113);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(192, 185);
            this.txtDescription.TabIndex = 30;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(148, 20);
            this.txtName.TabIndex = 29;
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
            // gbSkills
            // 
            this.gbSkills.Controls.Add(this.btnRemoveSkill);
            this.gbSkills.Controls.Add(this.btnAddSkill);
            this.gbSkills.Controls.Add(this.lstSkill);
            this.gbSkills.Location = new System.Drawing.Point(222, 27);
            this.gbSkills.Name = "gbSkills";
            this.gbSkills.Size = new System.Drawing.Size(142, 227);
            this.gbSkills.TabIndex = 56;
            this.gbSkills.TabStop = false;
            this.gbSkills.Text = "Skills";
            // 
            // btnRemoveSkill
            // 
            this.btnRemoveSkill.Location = new System.Drawing.Point(6, 201);
            this.btnRemoveSkill.Name = "btnRemoveSkill";
            this.btnRemoveSkill.Size = new System.Drawing.Size(130, 23);
            this.btnRemoveSkill.TabIndex = 2;
            this.btnRemoveSkill.Text = "Remove Skill";
            this.btnRemoveSkill.UseVisualStyleBackColor = true;
            this.btnRemoveSkill.Click += new System.EventHandler(this.btnRemoveSkill_Click);
            // 
            // btnAddSkill
            // 
            this.btnAddSkill.Location = new System.Drawing.Point(6, 172);
            this.btnAddSkill.Name = "btnAddSkill";
            this.btnAddSkill.Size = new System.Drawing.Size(130, 23);
            this.btnAddSkill.TabIndex = 1;
            this.btnAddSkill.Text = "Add Skill";
            this.btnAddSkill.UseVisualStyleBackColor = true;
            this.btnAddSkill.Click += new System.EventHandler(this.btnAddSkill_Click);
            // 
            // lstSkill
            // 
            this.lstSkill.FormattingEnabled = true;
            this.lstSkill.Location = new System.Drawing.Point(6, 19);
            this.lstSkill.Name = "lstSkill";
            this.lstSkill.Size = new System.Drawing.Size(130, 147);
            this.lstSkill.TabIndex = 0;
            // 
            // ItemCardEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 337);
            this.Controls.Add(this.gbSkills);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbCardInformation);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ItemCardEditor";
            this.Text = "Item Card Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbCardInformation.ResumeLayout(false);
            this.gbCardInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicCost)).EndInit();
            this.gbSkills.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox gbCardInformation;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblMagicCost;
        private System.Windows.Forms.NumericUpDown txtMagicCost;
        private System.Windows.Forms.Label lblRarity;
        private System.Windows.Forms.ComboBox cboRarity;
        private System.Windows.Forms.GroupBox gbSkills;
        private System.Windows.Forms.Button btnRemoveSkill;
        private System.Windows.Forms.Button btnAddSkill;
        private System.Windows.Forms.ListBox lstSkill;
    }
}