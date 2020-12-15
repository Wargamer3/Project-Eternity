namespace ProjectEternity.Editors.UnitHubEditor
{
    partial class UnitHubEditor
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
            this.gbVisualNovels = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstVisualNovels = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbOriginalUnit = new System.Windows.Forms.GroupBox();
            this.btnSelectOrginalUnit = new System.Windows.Forms.Button();
            this.txtOriginalUnit = new System.Windows.Forms.TextBox();
            this.gbVisualNovels.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbOriginalUnit.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVisualNovels
            // 
            this.gbVisualNovels.Controls.Add(this.btnRemove);
            this.gbVisualNovels.Controls.Add(this.btnAdd);
            this.gbVisualNovels.Controls.Add(this.lstVisualNovels);
            this.gbVisualNovels.Location = new System.Drawing.Point(12, 27);
            this.gbVisualNovels.Name = "gbVisualNovels";
            this.gbVisualNovels.Size = new System.Drawing.Size(177, 240);
            this.gbVisualNovels.TabIndex = 0;
            this.gbVisualNovels.TabStop = false;
            this.gbVisualNovels.Text = "Visual Novels";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(96, 211);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 211);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstVisualNovels
            // 
            this.lstVisualNovels.FormattingEnabled = true;
            this.lstVisualNovels.Location = new System.Drawing.Point(6, 19);
            this.lstVisualNovels.Name = "lstVisualNovels";
            this.lstVisualNovels.Size = new System.Drawing.Size(165, 186);
            this.lstVisualNovels.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(506, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbOriginalUnit
            // 
            this.gbOriginalUnit.Controls.Add(this.btnSelectOrginalUnit);
            this.gbOriginalUnit.Controls.Add(this.txtOriginalUnit);
            this.gbOriginalUnit.Location = new System.Drawing.Point(195, 27);
            this.gbOriginalUnit.Name = "gbOriginalUnit";
            this.gbOriginalUnit.Size = new System.Drawing.Size(230, 72);
            this.gbOriginalUnit.TabIndex = 4;
            this.gbOriginalUnit.TabStop = false;
            this.gbOriginalUnit.Text = "Original Unit";
            // 
            // btnSelectOrginalUnit
            // 
            this.btnSelectOrginalUnit.Location = new System.Drawing.Point(6, 43);
            this.btnSelectOrginalUnit.Name = "btnSelectOrginalUnit";
            this.btnSelectOrginalUnit.Size = new System.Drawing.Size(120, 23);
            this.btnSelectOrginalUnit.TabIndex = 2;
            this.btnSelectOrginalUnit.Text = "Select Unit";
            this.btnSelectOrginalUnit.UseVisualStyleBackColor = true;
            this.btnSelectOrginalUnit.Click += new System.EventHandler(this.btnSelectOrginalUnit_Click);
            // 
            // txtOriginalUnit
            // 
            this.txtOriginalUnit.Location = new System.Drawing.Point(6, 17);
            this.txtOriginalUnit.Name = "txtOriginalUnit";
            this.txtOriginalUnit.ReadOnly = true;
            this.txtOriginalUnit.Size = new System.Drawing.Size(218, 20);
            this.txtOriginalUnit.TabIndex = 0;
            // 
            // UnitHubEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 275);
            this.Controls.Add(this.gbOriginalUnit);
            this.Controls.Add(this.gbVisualNovels);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitHubEditor";
            this.Text = "Unit Hub Editor";
            this.gbVisualNovels.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbOriginalUnit.ResumeLayout(false);
            this.gbOriginalUnit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVisualNovels;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstVisualNovels;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbOriginalUnit;
        private System.Windows.Forms.Button btnSelectOrginalUnit;
        private System.Windows.Forms.TextBox txtOriginalUnit;
    }
}