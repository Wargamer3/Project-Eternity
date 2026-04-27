namespace ProjectEternity.Editors.LifeSimMapEditor
{
    partial class ExtraTabsUserControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabScenery = new System.Windows.Forms.TabPage();
            this.gbSceneryTiletset = new System.Windows.Forms.GroupBox();
            this.btnScenery3DTileAttributes = new System.Windows.Forms.Button();
            this.btnSceneryTileAttributes = new System.Windows.Forms.Button();
            this.btnSceneryRemoveTileset = new System.Windows.Forms.Button();
            this.btnSceneryAddTileset = new System.Windows.Forms.Button();
            this.lblTileset = new System.Windows.Forms.Label();
            this.cboSceneryTilesets = new System.Windows.Forms.ComboBox();
            this.gbSceneryViewer = new System.Windows.Forms.GroupBox();
            this.lvIScenery = new System.Windows.Forms.ListView();
            this.gbSceneryAutotile = new System.Windows.Forms.GroupBox();
            this.btnSceneryRemoveAutotile = new System.Windows.Forms.Button();
            this.btnSceneryAddAutotile = new System.Windows.Forms.Button();
            this.lblAutotile = new System.Windows.Forms.Label();
            this.cboSceneryAutotile = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabScenery.SuspendLayout();
            this.gbSceneryTiletset.SuspendLayout();
            this.gbSceneryViewer.SuspendLayout();
            this.gbSceneryAutotile.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabScenery);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(339, 632);
            this.tabControl1.TabIndex = 0;
            // 
            // tabScenery
            // 
            this.tabScenery.Controls.Add(this.gbSceneryTiletset);
            this.tabScenery.Controls.Add(this.gbSceneryViewer);
            this.tabScenery.Controls.Add(this.gbSceneryAutotile);
            this.tabScenery.Location = new System.Drawing.Point(4, 22);
            this.tabScenery.Name = "tabScenery";
            this.tabScenery.Padding = new System.Windows.Forms.Padding(3);
            this.tabScenery.Size = new System.Drawing.Size(331, 606);
            this.tabScenery.TabIndex = 2;
            this.tabScenery.Text = "Scenery";
            this.tabScenery.UseVisualStyleBackColor = true;
            // 
            // gbSceneryTiletset
            // 
            this.gbSceneryTiletset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryTiletset.Controls.Add(this.btnScenery3DTileAttributes);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryTileAttributes);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryRemoveTileset);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryAddTileset);
            this.gbSceneryTiletset.Controls.Add(this.lblTileset);
            this.gbSceneryTiletset.Controls.Add(this.cboSceneryTilesets);
            this.gbSceneryTiletset.Location = new System.Drawing.Point(6, 378);
            this.gbSceneryTiletset.Name = "gbSceneryTiletset";
            this.gbSceneryTiletset.Size = new System.Drawing.Size(315, 122);
            this.gbSceneryTiletset.TabIndex = 12;
            this.gbSceneryTiletset.TabStop = false;
            this.gbSceneryTiletset.Text = "Tileset";
            // 
            // btnScenery3DTileAttributes
            // 
            this.btnScenery3DTileAttributes.Location = new System.Drawing.Point(160, 88);
            this.btnScenery3DTileAttributes.Name = "btnScenery3DTileAttributes";
            this.btnScenery3DTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btnScenery3DTileAttributes.TabIndex = 8;
            this.btnScenery3DTileAttributes.Text = "3D Tile Attributes";
            this.btnScenery3DTileAttributes.UseVisualStyleBackColor = true;
            // 
            // btnSceneryTileAttributes
            // 
            this.btnSceneryTileAttributes.Location = new System.Drawing.Point(9, 88);
            this.btnSceneryTileAttributes.Name = "btnSceneryTileAttributes";
            this.btnSceneryTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btnSceneryTileAttributes.TabIndex = 7;
            this.btnSceneryTileAttributes.Text = "Tile Attributes";
            this.btnSceneryTileAttributes.UseVisualStyleBackColor = true;
            // 
            // btnSceneryRemoveTileset
            // 
            this.btnSceneryRemoveTileset.Location = new System.Drawing.Point(75, 59);
            this.btnSceneryRemoveTileset.Name = "btnSceneryRemoveTileset";
            this.btnSceneryRemoveTileset.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryRemoveTileset.TabIndex = 3;
            this.btnSceneryRemoveTileset.Text = "Remove";
            this.btnSceneryRemoveTileset.UseVisualStyleBackColor = true;
            // 
            // btnSceneryAddTileset
            // 
            this.btnSceneryAddTileset.Location = new System.Drawing.Point(9, 59);
            this.btnSceneryAddTileset.Name = "btnSceneryAddTileset";
            this.btnSceneryAddTileset.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryAddTileset.TabIndex = 2;
            this.btnSceneryAddTileset.Text = "Add";
            this.btnSceneryAddTileset.UseVisualStyleBackColor = true;
            // 
            // lblTileset
            // 
            this.lblTileset.AutoSize = true;
            this.lblTileset.Location = new System.Drawing.Point(6, 16);
            this.lblTileset.Name = "lblTileset";
            this.lblTileset.Size = new System.Drawing.Size(23, 13);
            this.lblTileset.TabIndex = 1;
            this.lblTileset.Text = "List";
            // 
            // cboSceneryTilesets
            // 
            this.cboSceneryTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSceneryTilesets.FormattingEnabled = true;
            this.cboSceneryTilesets.Location = new System.Drawing.Point(9, 32);
            this.cboSceneryTilesets.Name = "cboSceneryTilesets";
            this.cboSceneryTilesets.Size = new System.Drawing.Size(291, 21);
            this.cboSceneryTilesets.TabIndex = 0;
            // 
            // gbSceneryViewer
            // 
            this.gbSceneryViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryViewer.Controls.Add(this.lvIScenery);
            this.gbSceneryViewer.Location = new System.Drawing.Point(6, 6);
            this.gbSceneryViewer.Name = "gbSceneryViewer";
            this.gbSceneryViewer.Size = new System.Drawing.Size(319, 366);
            this.gbSceneryViewer.TabIndex = 11;
            this.gbSceneryViewer.TabStop = false;
            this.gbSceneryViewer.Text = "Scenery";
            // 
            // lvIScenery
            // 
            this.lvIScenery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvIScenery.HideSelection = false;
            this.lvIScenery.Location = new System.Drawing.Point(6, 19);
            this.lvIScenery.Name = "lvIScenery";
            this.lvIScenery.Size = new System.Drawing.Size(307, 341);
            this.lvIScenery.TabIndex = 0;
            this.lvIScenery.UseCompatibleStateImageBehavior = false;
            // 
            // gbSceneryAutotile
            // 
            this.gbSceneryAutotile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryAutotile.Controls.Add(this.btnSceneryRemoveAutotile);
            this.gbSceneryAutotile.Controls.Add(this.btnSceneryAddAutotile);
            this.gbSceneryAutotile.Controls.Add(this.lblAutotile);
            this.gbSceneryAutotile.Controls.Add(this.cboSceneryAutotile);
            this.gbSceneryAutotile.Location = new System.Drawing.Point(6, 506);
            this.gbSceneryAutotile.Name = "gbSceneryAutotile";
            this.gbSceneryAutotile.Size = new System.Drawing.Size(319, 94);
            this.gbSceneryAutotile.TabIndex = 5;
            this.gbSceneryAutotile.TabStop = false;
            this.gbSceneryAutotile.Text = "Autotile";
            // 
            // btnSceneryRemoveAutotile
            // 
            this.btnSceneryRemoveAutotile.Location = new System.Drawing.Point(72, 59);
            this.btnSceneryRemoveAutotile.Name = "btnSceneryRemoveAutotile";
            this.btnSceneryRemoveAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryRemoveAutotile.TabIndex = 3;
            this.btnSceneryRemoveAutotile.Text = "Remove";
            this.btnSceneryRemoveAutotile.UseVisualStyleBackColor = true;
            // 
            // btnSceneryAddAutotile
            // 
            this.btnSceneryAddAutotile.Location = new System.Drawing.Point(6, 59);
            this.btnSceneryAddAutotile.Name = "btnSceneryAddAutotile";
            this.btnSceneryAddAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryAddAutotile.TabIndex = 2;
            this.btnSceneryAddAutotile.Text = "Add";
            this.btnSceneryAddAutotile.UseVisualStyleBackColor = true;
            // 
            // lblAutotile
            // 
            this.lblAutotile.AutoSize = true;
            this.lblAutotile.Location = new System.Drawing.Point(6, 16);
            this.lblAutotile.Name = "lblAutotile";
            this.lblAutotile.Size = new System.Drawing.Size(23, 13);
            this.lblAutotile.TabIndex = 1;
            this.lblAutotile.Text = "List";
            // 
            // cboSceneryAutotile
            // 
            this.cboSceneryAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSceneryAutotile.FormattingEnabled = true;
            this.cboSceneryAutotile.Location = new System.Drawing.Point(9, 32);
            this.cboSceneryAutotile.Name = "cboSceneryAutotile";
            this.cboSceneryAutotile.Size = new System.Drawing.Size(279, 21);
            this.cboSceneryAutotile.TabIndex = 0;
            // 
            // ExtraTabsUserControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "ExtraTabsUserControl";
            this.Size = new System.Drawing.Size(349, 635);
            this.tabControl1.ResumeLayout(false);
            this.tabScenery.ResumeLayout(false);
            this.gbSceneryTiletset.ResumeLayout(false);
            this.gbSceneryTiletset.PerformLayout();
            this.gbSceneryViewer.ResumeLayout(false);
            this.gbSceneryAutotile.ResumeLayout(false);
            this.gbSceneryAutotile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabScenery;
        private System.Windows.Forms.GroupBox gbSceneryViewer;
        public System.Windows.Forms.ListView lvIScenery;
        private System.Windows.Forms.GroupBox gbSceneryTiletset;
        public System.Windows.Forms.Button btnScenery3DTileAttributes;
        public System.Windows.Forms.Button btnSceneryTileAttributes;
        public System.Windows.Forms.Button btnSceneryRemoveTileset;
        public System.Windows.Forms.Button btnSceneryAddTileset;
        private System.Windows.Forms.Label lblTileset;
        public System.Windows.Forms.ComboBox cboSceneryTilesets;
        private System.Windows.Forms.GroupBox gbSceneryAutotile;
        public System.Windows.Forms.Button btnSceneryRemoveAutotile;
        public System.Windows.Forms.Button btnSceneryAddAutotile;
        private System.Windows.Forms.Label lblAutotile;
        public System.Windows.Forms.ComboBox cboSceneryAutotile;
    }
}
