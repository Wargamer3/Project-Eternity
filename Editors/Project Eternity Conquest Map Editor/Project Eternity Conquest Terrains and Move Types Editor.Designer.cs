
namespace ProjectEternity.Editors.ConquestMapEditor
{
    partial class ProjectEternityConquestTerrainsAndMoveTypesEditor
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
            this.gbTerrains = new System.Windows.Forms.GroupBox();
            this.txtTerrainName = new System.Windows.Forms.TextBox();
            this.lblTerrainName = new System.Windows.Forms.Label();
            this.lsTerrains = new System.Windows.Forms.ListBox();
            this.btnDeleteTerrain = new System.Windows.Forms.Button();
            this.btnAddNewTerrain = new System.Windows.Forms.Button();
            this.lblTerrainStarValue = new System.Windows.Forms.Label();
            this.txtTerrainDefenceValue = new System.Windows.Forms.NumericUpDown();
            this.dgvMoveTypes = new System.Windows.Forms.DataGridView();
            this.clMoveType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbTerrainMoveTypes = new System.Windows.Forms.GroupBox();
            this.gbMoveTypes = new System.Windows.Forms.GroupBox();
            this.txtMoveTypeName = new System.Windows.Forms.TextBox();
            this.lblMoveTypeName = new System.Windows.Forms.Label();
            this.lsMoveTypes = new System.Windows.Forms.ListBox();
            this.btnDeleteMoveType = new System.Windows.Forms.Button();
            this.btnAddNewMoveType = new System.Windows.Forms.Button();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTerrains.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTerrainDefenceValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoveTypes)).BeginInit();
            this.gbTerrainMoveTypes.SuspendLayout();
            this.gbMoveTypes.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTerrains
            // 
            this.gbTerrains.Controls.Add(this.txtTerrainName);
            this.gbTerrains.Controls.Add(this.lblTerrainName);
            this.gbTerrains.Controls.Add(this.lsTerrains);
            this.gbTerrains.Controls.Add(this.btnDeleteTerrain);
            this.gbTerrains.Controls.Add(this.btnAddNewTerrain);
            this.gbTerrains.Controls.Add(this.lblTerrainStarValue);
            this.gbTerrains.Controls.Add(this.txtTerrainDefenceValue);
            this.gbTerrains.Location = new System.Drawing.Point(12, 27);
            this.gbTerrains.Name = "gbTerrains";
            this.gbTerrains.Size = new System.Drawing.Size(154, 398);
            this.gbTerrains.TabIndex = 0;
            this.gbTerrains.TabStop = false;
            this.gbTerrains.Text = "Terrains";
            // 
            // txtTerrainName
            // 
            this.txtTerrainName.Location = new System.Drawing.Point(14, 275);
            this.txtTerrainName.Name = "txtTerrainName";
            this.txtTerrainName.Size = new System.Drawing.Size(126, 20);
            this.txtTerrainName.TabIndex = 16;
            this.txtTerrainName.TextChanged += new System.EventHandler(this.txtTerrainName_TextChanged);
            // 
            // lblTerrainName
            // 
            this.lblTerrainName.AutoSize = true;
            this.lblTerrainName.Location = new System.Drawing.Point(11, 259);
            this.lblTerrainName.Name = "lblTerrainName";
            this.lblTerrainName.Size = new System.Drawing.Size(35, 13);
            this.lblTerrainName.TabIndex = 15;
            this.lblTerrainName.Text = "Name";
            // 
            // lsTerrains
            // 
            this.lsTerrains.FormattingEnabled = true;
            this.lsTerrains.Location = new System.Drawing.Point(6, 19);
            this.lsTerrains.Name = "lsTerrains";
            this.lsTerrains.Size = new System.Drawing.Size(142, 225);
            this.lsTerrains.TabIndex = 14;
            this.lsTerrains.SelectedIndexChanged += new System.EventHandler(this.lsTerrains_SelectedIndexChanged);
            // 
            // btnDeleteTerrain
            // 
            this.btnDeleteTerrain.Location = new System.Drawing.Point(14, 369);
            this.btnDeleteTerrain.Name = "btnDeleteTerrain";
            this.btnDeleteTerrain.Size = new System.Drawing.Size(126, 23);
            this.btnDeleteTerrain.TabIndex = 9;
            this.btnDeleteTerrain.Text = "Delete Terrain";
            this.btnDeleteTerrain.UseVisualStyleBackColor = true;
            this.btnDeleteTerrain.Click += new System.EventHandler(this.btnDeleteTerrain_Click);
            // 
            // btnAddNewTerrain
            // 
            this.btnAddNewTerrain.Location = new System.Drawing.Point(14, 340);
            this.btnAddNewTerrain.Name = "btnAddNewTerrain";
            this.btnAddNewTerrain.Size = new System.Drawing.Size(126, 23);
            this.btnAddNewTerrain.TabIndex = 7;
            this.btnAddNewTerrain.Text = "Add New Terrain";
            this.btnAddNewTerrain.UseVisualStyleBackColor = true;
            this.btnAddNewTerrain.Click += new System.EventHandler(this.btnAddNewTerrain_Click);
            // 
            // lblTerrainStarValue
            // 
            this.lblTerrainStarValue.AutoSize = true;
            this.lblTerrainStarValue.Location = new System.Drawing.Point(11, 298);
            this.lblTerrainStarValue.Name = "lblTerrainStarValue";
            this.lblTerrainStarValue.Size = new System.Drawing.Size(26, 13);
            this.lblTerrainStarValue.TabIndex = 5;
            this.lblTerrainStarValue.Text = "Star";
            // 
            // txtTerrainDefenceValue
            // 
            this.txtTerrainDefenceValue.Location = new System.Drawing.Point(14, 314);
            this.txtTerrainDefenceValue.Name = "txtTerrainDefenceValue";
            this.txtTerrainDefenceValue.Size = new System.Drawing.Size(126, 20);
            this.txtTerrainDefenceValue.TabIndex = 4;
            this.txtTerrainDefenceValue.ValueChanged += new System.EventHandler(this.txtTerrainDefenceValue_ValueChanged);
            // 
            // dgvMoveTypes
            // 
            this.dgvMoveTypes.AllowUserToAddRows = false;
            this.dgvMoveTypes.AllowUserToDeleteRows = false;
            this.dgvMoveTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMoveTypes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMoveTypes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMoveTypes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clMoveType,
            this.clCost});
            this.dgvMoveTypes.Location = new System.Drawing.Point(6, 19);
            this.dgvMoveTypes.MultiSelect = false;
            this.dgvMoveTypes.Name = "dgvMoveTypes";
            this.dgvMoveTypes.RowHeadersVisible = false;
            this.dgvMoveTypes.Size = new System.Drawing.Size(604, 374);
            this.dgvMoveTypes.TabIndex = 6;
            this.dgvMoveTypes.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMoveTypes_CellValueChanged);
            // 
            // clMoveType
            // 
            this.clMoveType.HeaderText = "Move Type";
            this.clMoveType.Name = "clMoveType";
            this.clMoveType.ReadOnly = true;
            // 
            // clCost
            // 
            this.clCost.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clCost.HeaderText = "Cost";
            this.clCost.Name = "clCost";
            // 
            // gbTerrainMoveTypes
            // 
            this.gbTerrainMoveTypes.Controls.Add(this.dgvMoveTypes);
            this.gbTerrainMoveTypes.Location = new System.Drawing.Point(332, 26);
            this.gbTerrainMoveTypes.Name = "gbTerrainMoveTypes";
            this.gbTerrainMoveTypes.Size = new System.Drawing.Size(616, 399);
            this.gbTerrainMoveTypes.TabIndex = 7;
            this.gbTerrainMoveTypes.TabStop = false;
            this.gbTerrainMoveTypes.Text = "Terrain Move Types";
            // 
            // gbMoveTypes
            // 
            this.gbMoveTypes.Controls.Add(this.txtMoveTypeName);
            this.gbMoveTypes.Controls.Add(this.lblMoveTypeName);
            this.gbMoveTypes.Controls.Add(this.lsMoveTypes);
            this.gbMoveTypes.Controls.Add(this.btnDeleteMoveType);
            this.gbMoveTypes.Controls.Add(this.btnAddNewMoveType);
            this.gbMoveTypes.Location = new System.Drawing.Point(172, 27);
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
            this.mnuToolBar.Size = new System.Drawing.Size(964, 24);
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
            // ProjectEternityConquestTerrainsAndMoveTypesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 429);
            this.Controls.Add(this.mnuToolBar);
            this.Controls.Add(this.gbMoveTypes);
            this.Controls.Add(this.gbTerrainMoveTypes);
            this.Controls.Add(this.gbTerrains);
            this.Name = "ProjectEternityConquestTerrainsAndMoveTypesEditor";
            this.Text = "Project Eternity Conquest Terrains and Move Types Editor";
            this.Load += new System.EventHandler(this.ProjectEternityConquestTerrainsAndMoveTypesEditor_Load);
            this.gbTerrains.ResumeLayout(false);
            this.gbTerrains.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTerrainDefenceValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMoveTypes)).EndInit();
            this.gbTerrainMoveTypes.ResumeLayout(false);
            this.gbMoveTypes.ResumeLayout(false);
            this.gbMoveTypes.PerformLayout();
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTerrains;
        private System.Windows.Forms.Label lblTerrainStarValue;
        private System.Windows.Forms.NumericUpDown txtTerrainDefenceValue;
        public System.Windows.Forms.DataGridView dgvMoveTypes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clMoveType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clCost;
        private System.Windows.Forms.Button btnDeleteTerrain;
        private System.Windows.Forms.Button btnAddNewTerrain;
        private System.Windows.Forms.GroupBox gbTerrainMoveTypes;
        private System.Windows.Forms.TextBox txtTerrainName;
        private System.Windows.Forms.Label lblTerrainName;
        private System.Windows.Forms.ListBox lsTerrains;
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