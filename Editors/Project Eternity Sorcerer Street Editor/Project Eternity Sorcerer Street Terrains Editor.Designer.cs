namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    partial class ProjectEternitySorcererStreetTerrainsEditor
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
            this.gbMoveTypes = new System.Windows.Forms.GroupBox();
            this.txtMoveTypeName = new System.Windows.Forms.TextBox();
            this.lblMoveTypeName = new System.Windows.Forms.Label();
            this.lsMoveTypes = new System.Windows.Forms.ListBox();
            this.btnDeleteMoveType = new System.Windows.Forms.Button();
            this.btnAddNewMoveType = new System.Windows.Forms.Button();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbMoveTypes.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMoveTypes
            // 
            this.gbMoveTypes.Controls.Add(this.txtMoveTypeName);
            this.gbMoveTypes.Controls.Add(this.lblMoveTypeName);
            this.gbMoveTypes.Controls.Add(this.lsMoveTypes);
            this.gbMoveTypes.Controls.Add(this.btnDeleteMoveType);
            this.gbMoveTypes.Controls.Add(this.btnAddNewMoveType);
            this.gbMoveTypes.Location = new System.Drawing.Point(12, 27);
            this.gbMoveTypes.Name = "gbMoveTypes";
            this.gbMoveTypes.Size = new System.Drawing.Size(154, 398);
            this.gbMoveTypes.TabIndex = 11;
            this.gbMoveTypes.TabStop = false;
            this.gbMoveTypes.Text = "Move Types";
            // 
            // txtMoveTypeName
            // 
            this.txtMoveTypeName.Location = new System.Drawing.Point(14, 314);
            this.txtMoveTypeName.Name = "txtMoveTypeName";
            this.txtMoveTypeName.Size = new System.Drawing.Size(100, 20);
            this.txtMoveTypeName.TabIndex = 13;
            this.txtMoveTypeName.TextChanged += new System.EventHandler(this.txtMoveTypeName_TextChanged);
            // 
            // lblMoveTypeName
            // 
            this.lblMoveTypeName.AutoSize = true;
            this.lblMoveTypeName.Location = new System.Drawing.Point(11, 298);
            this.lblMoveTypeName.Name = "lblMoveTypeName";
            this.lblMoveTypeName.Size = new System.Drawing.Size(35, 13);
            this.lblMoveTypeName.TabIndex = 12;
            this.lblMoveTypeName.Text = "Name";
            // 
            // lsMoveTypes
            // 
            this.lsMoveTypes.FormattingEnabled = true;
            this.lsMoveTypes.Location = new System.Drawing.Point(6, 19);
            this.lsMoveTypes.Name = "lsMoveTypes";
            this.lsMoveTypes.Size = new System.Drawing.Size(142, 264);
            this.lsMoveTypes.TabIndex = 11;
            this.lsMoveTypes.SelectedIndexChanged += new System.EventHandler(this.lsMoveTypes_SelectedIndexChanged);
            // 
            // btnDeleteMoveType
            // 
            this.btnDeleteMoveType.Location = new System.Drawing.Point(14, 369);
            this.btnDeleteMoveType.Name = "btnDeleteMoveType";
            this.btnDeleteMoveType.Size = new System.Drawing.Size(126, 23);
            this.btnDeleteMoveType.TabIndex = 9;
            this.btnDeleteMoveType.Text = "Delete Move Type";
            this.btnDeleteMoveType.UseVisualStyleBackColor = true;
            this.btnDeleteMoveType.Click += new System.EventHandler(this.btnDeleteMoveType_Click);
            // 
            // btnAddNewMoveType
            // 
            this.btnAddNewMoveType.Location = new System.Drawing.Point(14, 340);
            this.btnAddNewMoveType.Name = "btnAddNewMoveType";
            this.btnAddNewMoveType.Size = new System.Drawing.Size(126, 23);
            this.btnAddNewMoveType.TabIndex = 8;
            this.btnAddNewMoveType.Text = "Add New Move Type";
            this.btnAddNewMoveType.UseVisualStyleBackColor = true;
            this.btnAddNewMoveType.Click += new System.EventHandler(this.btnAddNewMoveType_Click);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(174, 24);
            this.mnuToolBar.TabIndex = 18;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // ProjectEternitySorcererStreetTerrainsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(174, 429);
            this.Controls.Add(this.mnuToolBar);
            this.Controls.Add(this.gbMoveTypes);
            this.Name = "ProjectEternitySorcererStreetTerrainsEditor";
            this.Text = "Project Eternity Conquest Terrains and Move Types Editor";
            this.Load += new System.EventHandler(this.ProjectEternityConquestTerrainsAndMoveTypesEditor_Load);
            this.gbMoveTypes.ResumeLayout(false);
            this.gbMoveTypes.PerformLayout();
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbMoveTypes;
        private System.Windows.Forms.TextBox txtMoveTypeName;
        private System.Windows.Forms.Label lblMoveTypeName;
        private System.Windows.Forms.ListBox lsMoveTypes;
        private System.Windows.Forms.Button btnDeleteMoveType;
        private System.Windows.Forms.Button btnAddNewMoveType;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
    }
}