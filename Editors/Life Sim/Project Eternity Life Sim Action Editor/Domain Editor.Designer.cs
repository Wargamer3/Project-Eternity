namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class DomainEditor
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
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.gbDomainSpells = new System.Windows.Forms.GroupBox();
            this.lblAdvancedSpell = new System.Windows.Forms.Label();
            this.txtAdvancedSpell = new System.Windows.Forms.TextBox();
            this.lblDomainSpell = new System.Windows.Forms.Label();
            this.txtDomainSpell = new System.Windows.Forms.TextBox();
            this.btnSetAdvancedSpell = new System.Windows.Forms.Button();
            this.btnSetDomainSpell = new System.Windows.Forms.Button();
            this.gbLinkedDomain = new System.Windows.Forms.GroupBox();
            this.lblLinkedDomainName = new System.Windows.Forms.Label();
            this.txtLinkedDomain = new System.Windows.Forms.TextBox();
            this.btnSetDomain = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.gbDomainSpells.SuspendLayout();
            this.gbLinkedDomain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(419, 24);
            this.menuStrip1.TabIndex = 66;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbItemInformation
            // 
            this.gbItemInformation.Controls.Add(this.txtDescription);
            this.gbItemInformation.Controls.Add(this.lblDescription);
            this.gbItemInformation.Controls.Add(this.lblName);
            this.gbItemInformation.Controls.Add(this.txtName);
            this.gbItemInformation.Location = new System.Drawing.Point(12, 27);
            this.gbItemInformation.Name = "gbItemInformation";
            this.gbItemInformation.Size = new System.Drawing.Size(395, 145);
            this.gbItemInformation.TabIndex = 67;
            this.gbItemInformation.TabStop = false;
            this.gbItemInformation.Text = "Basic Information";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 58);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(383, 82);
            this.txtDescription.TabIndex = 73;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 42);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 72;
            this.lblDescription.Text = "Description:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 23;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(339, 20);
            this.txtName.TabIndex = 22;
            // 
            // gbDomainSpells
            // 
            this.gbDomainSpells.Controls.Add(this.lblAdvancedSpell);
            this.gbDomainSpells.Controls.Add(this.txtAdvancedSpell);
            this.gbDomainSpells.Controls.Add(this.lblDomainSpell);
            this.gbDomainSpells.Controls.Add(this.txtDomainSpell);
            this.gbDomainSpells.Controls.Add(this.btnSetAdvancedSpell);
            this.gbDomainSpells.Controls.Add(this.btnSetDomainSpell);
            this.gbDomainSpells.Location = new System.Drawing.Point(12, 178);
            this.gbDomainSpells.Name = "gbDomainSpells";
            this.gbDomainSpells.Size = new System.Drawing.Size(395, 80);
            this.gbDomainSpells.TabIndex = 75;
            this.gbDomainSpells.TabStop = false;
            this.gbDomainSpells.Text = "Domain Spells";
            // 
            // lblAdvancedSpell
            // 
            this.lblAdvancedSpell.AutoSize = true;
            this.lblAdvancedSpell.Location = new System.Drawing.Point(12, 48);
            this.lblAdvancedSpell.Name = "lblAdvancedSpell";
            this.lblAdvancedSpell.Size = new System.Drawing.Size(82, 13);
            this.lblAdvancedSpell.TabIndex = 79;
            this.lblAdvancedSpell.Text = "Advanced Spell";
            // 
            // txtAdvancedSpell
            // 
            this.txtAdvancedSpell.Location = new System.Drawing.Point(100, 45);
            this.txtAdvancedSpell.Name = "txtAdvancedSpell";
            this.txtAdvancedSpell.ReadOnly = true;
            this.txtAdvancedSpell.Size = new System.Drawing.Size(167, 20);
            this.txtAdvancedSpell.TabIndex = 78;
            // 
            // lblDomainSpell
            // 
            this.lblDomainSpell.AutoSize = true;
            this.lblDomainSpell.Location = new System.Drawing.Point(12, 22);
            this.lblDomainSpell.Name = "lblDomainSpell";
            this.lblDomainSpell.Size = new System.Drawing.Size(69, 13);
            this.lblDomainSpell.TabIndex = 77;
            this.lblDomainSpell.Text = "Domain Spell";
            // 
            // txtDomainSpell
            // 
            this.txtDomainSpell.Location = new System.Drawing.Point(100, 19);
            this.txtDomainSpell.Name = "txtDomainSpell";
            this.txtDomainSpell.ReadOnly = true;
            this.txtDomainSpell.Size = new System.Drawing.Size(167, 20);
            this.txtDomainSpell.TabIndex = 76;
            // 
            // btnSetAdvancedSpell
            // 
            this.btnSetAdvancedSpell.Location = new System.Drawing.Point(273, 48);
            this.btnSetAdvancedSpell.Name = "btnSetAdvancedSpell";
            this.btnSetAdvancedSpell.Size = new System.Drawing.Size(116, 23);
            this.btnSetAdvancedSpell.TabIndex = 75;
            this.btnSetAdvancedSpell.Text = "Set Spell";
            this.btnSetAdvancedSpell.UseVisualStyleBackColor = true;
            this.btnSetAdvancedSpell.Click += new System.EventHandler(this.btnSetAdvancedSpell_Click);
            // 
            // btnSetDomainSpell
            // 
            this.btnSetDomainSpell.Location = new System.Drawing.Point(273, 19);
            this.btnSetDomainSpell.Name = "btnSetDomainSpell";
            this.btnSetDomainSpell.Size = new System.Drawing.Size(116, 23);
            this.btnSetDomainSpell.TabIndex = 74;
            this.btnSetDomainSpell.Text = "Set Spell";
            this.btnSetDomainSpell.UseVisualStyleBackColor = true;
            this.btnSetDomainSpell.Click += new System.EventHandler(this.btnSetDomainSpell_Click);
            // 
            // gbLinkedDomain
            // 
            this.gbLinkedDomain.Controls.Add(this.lblLinkedDomainName);
            this.gbLinkedDomain.Controls.Add(this.txtLinkedDomain);
            this.gbLinkedDomain.Controls.Add(this.btnSetDomain);
            this.gbLinkedDomain.Location = new System.Drawing.Point(12, 264);
            this.gbLinkedDomain.Name = "gbLinkedDomain";
            this.gbLinkedDomain.Size = new System.Drawing.Size(395, 54);
            this.gbLinkedDomain.TabIndex = 76;
            this.gbLinkedDomain.TabStop = false;
            this.gbLinkedDomain.Text = "Linked Domain";
            // 
            // lblLinkedDomainName
            // 
            this.lblLinkedDomainName.AutoSize = true;
            this.lblLinkedDomainName.Location = new System.Drawing.Point(12, 22);
            this.lblLinkedDomainName.Name = "lblLinkedDomainName";
            this.lblLinkedDomainName.Size = new System.Drawing.Size(74, 13);
            this.lblLinkedDomainName.TabIndex = 77;
            this.lblLinkedDomainName.Text = "Domain Name";
            // 
            // txtLinkedDomain
            // 
            this.txtLinkedDomain.Location = new System.Drawing.Point(100, 19);
            this.txtLinkedDomain.Name = "txtLinkedDomain";
            this.txtLinkedDomain.ReadOnly = true;
            this.txtLinkedDomain.Size = new System.Drawing.Size(167, 20);
            this.txtLinkedDomain.TabIndex = 76;
            // 
            // btnSetDomain
            // 
            this.btnSetDomain.Location = new System.Drawing.Point(273, 19);
            this.btnSetDomain.Name = "btnSetDomain";
            this.btnSetDomain.Size = new System.Drawing.Size(116, 23);
            this.btnSetDomain.TabIndex = 74;
            this.btnSetDomain.Text = "Set Domain";
            this.btnSetDomain.UseVisualStyleBackColor = true;
            this.btnSetDomain.Click += new System.EventHandler(this.btnSetDomain_Click);
            // 
            // DomainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 327);
            this.Controls.Add(this.gbLinkedDomain);
            this.Controls.Add(this.gbDomainSpells);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "DomainEditor";
            this.Text = "Domain Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.gbDomainSpells.ResumeLayout(false);
            this.gbDomainSpells.PerformLayout();
            this.gbLinkedDomain.ResumeLayout(false);
            this.gbLinkedDomain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.GroupBox gbDomainSpells;
        private System.Windows.Forms.Button btnSetAdvancedSpell;
        private System.Windows.Forms.Button btnSetDomainSpell;
        private System.Windows.Forms.Label lblAdvancedSpell;
        private System.Windows.Forms.TextBox txtAdvancedSpell;
        private System.Windows.Forms.Label lblDomainSpell;
        private System.Windows.Forms.TextBox txtDomainSpell;
        private System.Windows.Forms.GroupBox gbLinkedDomain;
        private System.Windows.Forms.Label lblLinkedDomainName;
        private System.Windows.Forms.TextBox txtLinkedDomain;
        private System.Windows.Forms.Button btnSetDomain;
    }
}