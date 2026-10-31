namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class TraitEditor
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
            this.gbEffects = new System.Windows.Forms.GroupBox();
            this.lstEffects = new System.Windows.Forms.ListBox();
            this.cboEffect = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.pgEffect = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.gbEffects.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(417, 24);
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
            // gbEffects
            // 
            this.gbEffects.Controls.Add(this.lstEffects);
            this.gbEffects.Controls.Add(this.cboEffect);
            this.gbEffects.Controls.Add(this.btnAdd);
            this.gbEffects.Controls.Add(this.btnRemove);
            this.gbEffects.Controls.Add(this.pgEffect);
            this.gbEffects.Location = new System.Drawing.Point(12, 178);
            this.gbEffects.Name = "gbEffects";
            this.gbEffects.Size = new System.Drawing.Size(395, 320);
            this.gbEffects.TabIndex = 68;
            this.gbEffects.TabStop = false;
            this.gbEffects.Text = "Effects";
            // 
            // lstEffects
            // 
            this.lstEffects.FormattingEnabled = true;
            this.lstEffects.Location = new System.Drawing.Point(9, 20);
            this.lstEffects.Name = "lstEffects";
            this.lstEffects.Size = new System.Drawing.Size(250, 95);
            this.lstEffects.TabIndex = 0;
            this.lstEffects.SelectedIndexChanged += new System.EventHandler(this.lstEffects_SelectedIndexChanged);
            // 
            // cboEffect
            // 
            this.cboEffect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEffect.Location = new System.Drawing.Point(265, 20);
            this.cboEffect.Name = "cboEffect";
            this.cboEffect.Size = new System.Drawing.Size(120, 21);
            this.cboEffect.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(265, 48);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(120, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(265, 76);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(120, 23);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // pgEffect
            // 
            this.pgEffect.Location = new System.Drawing.Point(9, 124);
            this.pgEffect.Name = "pgEffect";
            this.pgEffect.Size = new System.Drawing.Size(376, 186);
            this.pgEffect.TabIndex = 4;
            // 
            // TraitEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 510);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Controls.Add(this.gbEffects);
            this.Name = "TraitEditor";
            this.Text = "Trait Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.gbEffects.ResumeLayout(false);
            this.gbEffects.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbEffects;
        private System.Windows.Forms.ListBox lstEffects;
        private System.Windows.Forms.ComboBox cboEffect;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.PropertyGrid pgEffect;
    }
}