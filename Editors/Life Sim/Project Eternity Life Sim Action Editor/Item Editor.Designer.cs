namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class ItemEditor
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrait = new System.Windows.Forms.Button();
            this.btnAddTrait = new System.Windows.Forms.Button();
            this.lsTraits = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBulk = new System.Windows.Forms.TextBox();
            this.lblBulk = new System.Windows.Forms.Label();
            this.gbSpells = new System.Windows.Forms.GroupBox();
            this.btnDeleteAction = new System.Windows.Forms.Button();
            this.lsActions = new System.Windows.Forms.ListBox();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbSpells.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(954, 24);
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemoveTrait);
            this.groupBox2.Controls.Add(this.btnAddTrait);
            this.groupBox2.Controls.Add(this.lsTraits);
            this.groupBox2.Location = new System.Drawing.Point(12, 178);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(395, 85);
            this.groupBox2.TabIndex = 75;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Traits";
            // 
            // btnRemoveTrait
            // 
            this.btnRemoveTrait.Location = new System.Drawing.Point(273, 48);
            this.btnRemoveTrait.Name = "btnRemoveTrait";
            this.btnRemoveTrait.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveTrait.TabIndex = 75;
            this.btnRemoveTrait.Text = "Remove Trait";
            this.btnRemoveTrait.UseVisualStyleBackColor = true;
            this.btnRemoveTrait.Click += new System.EventHandler(this.btnRemoveTrait_Click);
            // 
            // btnAddTrait
            // 
            this.btnAddTrait.Location = new System.Drawing.Point(273, 19);
            this.btnAddTrait.Name = "btnAddTrait";
            this.btnAddTrait.Size = new System.Drawing.Size(116, 23);
            this.btnAddTrait.TabIndex = 74;
            this.btnAddTrait.Text = "Add Trait";
            this.btnAddTrait.UseVisualStyleBackColor = true;
            this.btnAddTrait.Click += new System.EventHandler(this.btnAddTrait_Click);
            // 
            // lsTraits
            // 
            this.lsTraits.FormattingEnabled = true;
            this.lsTraits.Location = new System.Drawing.Point(6, 19);
            this.lsTraits.Name = "lsTraits";
            this.lsTraits.Size = new System.Drawing.Size(261, 56);
            this.lsTraits.TabIndex = 73;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTags);
            this.groupBox1.Controls.Add(this.lblTags);
            this.groupBox1.Controls.Add(this.txtBulk);
            this.groupBox1.Controls.Add(this.lblBulk);
            this.groupBox1.Location = new System.Drawing.Point(12, 269);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(395, 133);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base Stats";
            // 
            // txtBulk
            // 
            this.txtBulk.Location = new System.Drawing.Point(62, 19);
            this.txtBulk.Name = "txtBulk";
            this.txtBulk.Size = new System.Drawing.Size(105, 20);
            this.txtBulk.TabIndex = 74;
            // 
            // lblBulk
            // 
            this.lblBulk.AutoSize = true;
            this.lblBulk.Location = new System.Drawing.Point(9, 22);
            this.lblBulk.Name = "lblBulk";
            this.lblBulk.Size = new System.Drawing.Size(31, 13);
            this.lblBulk.TabIndex = 73;
            this.lblBulk.Text = "Bulk:";
            // 
            // gbSpells
            // 
            this.gbSpells.Controls.Add(this.btnDeleteAction);
            this.gbSpells.Controls.Add(this.lsActions);
            this.gbSpells.Controls.Add(this.btnAddAction);
            this.gbSpells.Location = new System.Drawing.Point(413, 27);
            this.gbSpells.Name = "gbSpells";
            this.gbSpells.Size = new System.Drawing.Size(275, 175);
            this.gbSpells.TabIndex = 80;
            this.gbSpells.TabStop = false;
            this.gbSpells.Text = "Actions";
            // 
            // btnDeleteAction
            // 
            this.btnDeleteAction.Location = new System.Drawing.Point(182, 146);
            this.btnDeleteAction.Name = "btnDeleteAction";
            this.btnDeleteAction.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteAction.TabIndex = 68;
            this.btnDeleteAction.Text = "Delete Action";
            this.btnDeleteAction.UseVisualStyleBackColor = true;
            this.btnDeleteAction.Click += new System.EventHandler(this.btnDeleteAction_Click);
            // 
            // lsActions
            // 
            this.lsActions.FormattingEnabled = true;
            this.lsActions.Location = new System.Drawing.Point(9, 19);
            this.lsActions.Name = "lsActions";
            this.lsActions.Size = new System.Drawing.Size(260, 108);
            this.lsActions.TabIndex = 65;
            // 
            // btnAddAction
            // 
            this.btnAddAction.Location = new System.Drawing.Point(6, 146);
            this.btnAddAction.Name = "btnAddAction";
            this.btnAddAction.Size = new System.Drawing.Size(71, 23);
            this.btnAddAction.TabIndex = 25;
            this.btnAddAction.Text = "Add Action";
            this.btnAddAction.UseVisualStyleBackColor = true;
            this.btnAddAction.Click += new System.EventHandler(this.btnAddAction_Click);
            // 
            // txtTags
            // 
            this.txtTags.Location = new System.Drawing.Point(9, 58);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(383, 69);
            this.txtTags.TabIndex = 76;
            // 
            // lblTags
            // 
            this.lblTags.AutoSize = true;
            this.lblTags.Location = new System.Drawing.Point(6, 42);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(34, 13);
            this.lblTags.TabIndex = 75;
            this.lblTags.Text = "Tags:";
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 414);
            this.Controls.Add(this.gbSpells);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "ItemEditor";
            this.Text = "Item Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbSpells.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemoveTrait;
        private System.Windows.Forms.Button btnAddTrait;
        private System.Windows.Forms.ListBox lsTraits;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtBulk;
        private System.Windows.Forms.Label lblBulk;
        private System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.GroupBox gbSpells;
        private System.Windows.Forms.Button btnDeleteAction;
        private System.Windows.Forms.ListBox lsActions;
        private System.Windows.Forms.Button btnAddAction;
    }
}