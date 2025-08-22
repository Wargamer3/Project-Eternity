
namespace ProjectEternity.Editors.MapEditor
{
    partial class TabsUserControl
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
            this.tabTilesets = new System.Windows.Forms.TabPage();
            this.gbToolsRight = new System.Windows.Forms.GroupBox();
            this.rbEyedropRight = new System.Windows.Forms.RadioButton();
            this.rbBucketRight = new System.Windows.Forms.RadioButton();
            this.rbPencilRight = new System.Windows.Forms.RadioButton();
            this.gbToolsLeft = new System.Windows.Forms.GroupBox();
            this.rbEyedropLeft = new System.Windows.Forms.RadioButton();
            this.rbBucketLeft = new System.Windows.Forms.RadioButton();
            this.rbPencilLeft = new System.Windows.Forms.RadioButton();
            this.gbTiletset = new System.Windows.Forms.GroupBox();
            this.sclTileHeight = new System.Windows.Forms.VScrollBar();
            this.sclTileWidth = new System.Windows.Forms.HScrollBar();
            this.btn3DTileAttributes = new System.Windows.Forms.Button();
            this.btnTileAttributes = new System.Windows.Forms.Button();
            this.TilesetViewer = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.gbAutotile = new System.Windows.Forms.GroupBox();
            this.btnRemoveAutotile = new System.Windows.Forms.Button();
            this.btnAddAutotile = new System.Windows.Forms.Button();
            this.lblAutotile = new System.Windows.Forms.Label();
            this.cboAutotile = new System.Windows.Forms.ComboBox();
            this.btnAddTilesetAsBackground = new System.Windows.Forms.Button();
            this.btnRemoveTileset = new System.Windows.Forms.Button();
            this.btnAddTileset = new System.Windows.Forms.Button();
            this.lblTileset = new System.Windows.Forms.Label();
            this.cboTilesets = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabTilesets.SuspendLayout();
            this.gbToolsRight.SuspendLayout();
            this.gbToolsLeft.SuspendLayout();
            this.gbTiletset.SuspendLayout();
            this.gbAutotile.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabTilesets);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(333, 691);
            this.tabControl1.TabIndex = 0;
            // 
            // tabTilesets
            // 
            this.tabTilesets.Controls.Add(this.gbToolsRight);
            this.tabTilesets.Controls.Add(this.gbToolsLeft);
            this.tabTilesets.Controls.Add(this.gbTiletset);
            this.tabTilesets.Location = new System.Drawing.Point(4, 22);
            this.tabTilesets.Name = "tabTilesets";
            this.tabTilesets.Padding = new System.Windows.Forms.Padding(3);
            this.tabTilesets.Size = new System.Drawing.Size(325, 665);
            this.tabTilesets.TabIndex = 0;
            this.tabTilesets.Text = "Tilesets";
            this.tabTilesets.UseVisualStyleBackColor = true;
            // 
            // gbToolsRight
            // 
            this.gbToolsRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbToolsRight.Controls.Add(this.rbEyedropRight);
            this.gbToolsRight.Controls.Add(this.rbBucketRight);
            this.gbToolsRight.Controls.Add(this.rbPencilRight);
            this.gbToolsRight.Location = new System.Drawing.Point(150, 559);
            this.gbToolsRight.Name = "gbToolsRight";
            this.gbToolsRight.Size = new System.Drawing.Size(138, 100);
            this.gbToolsRight.TabIndex = 6;
            this.gbToolsRight.TabStop = false;
            this.gbToolsRight.Text = "Tools [Right]";
            // 
            // rbEyedropRight
            // 
            this.rbEyedropRight.AutoSize = true;
            this.rbEyedropRight.Location = new System.Drawing.Point(6, 65);
            this.rbEyedropRight.Name = "rbEyedropRight";
            this.rbEyedropRight.Size = new System.Drawing.Size(79, 17);
            this.rbEyedropRight.TabIndex = 3;
            this.rbEyedropRight.TabStop = true;
            this.rbEyedropRight.Text = "Eyedropper";
            this.rbEyedropRight.UseVisualStyleBackColor = true;
            // 
            // rbBucketRight
            // 
            this.rbBucketRight.AutoSize = true;
            this.rbBucketRight.Location = new System.Drawing.Point(6, 42);
            this.rbBucketRight.Name = "rbBucketRight";
            this.rbBucketRight.Size = new System.Drawing.Size(59, 17);
            this.rbBucketRight.TabIndex = 1;
            this.rbBucketRight.TabStop = true;
            this.rbBucketRight.Text = "Bucket";
            this.rbBucketRight.UseVisualStyleBackColor = true;
            // 
            // rbPencilRight
            // 
            this.rbPencilRight.AutoSize = true;
            this.rbPencilRight.Location = new System.Drawing.Point(6, 19);
            this.rbPencilRight.Name = "rbPencilRight";
            this.rbPencilRight.Size = new System.Drawing.Size(54, 17);
            this.rbPencilRight.TabIndex = 0;
            this.rbPencilRight.TabStop = true;
            this.rbPencilRight.Text = "Pencil";
            this.rbPencilRight.UseVisualStyleBackColor = true;
            // 
            // gbToolsLeft
            // 
            this.gbToolsLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbToolsLeft.Controls.Add(this.rbEyedropLeft);
            this.gbToolsLeft.Controls.Add(this.rbBucketLeft);
            this.gbToolsLeft.Controls.Add(this.rbPencilLeft);
            this.gbToolsLeft.Location = new System.Drawing.Point(6, 559);
            this.gbToolsLeft.Name = "gbToolsLeft";
            this.gbToolsLeft.Size = new System.Drawing.Size(138, 100);
            this.gbToolsLeft.TabIndex = 5;
            this.gbToolsLeft.TabStop = false;
            this.gbToolsLeft.Text = "Tools [Left]";
            // 
            // rbEyedropLeft
            // 
            this.rbEyedropLeft.AutoSize = true;
            this.rbEyedropLeft.Location = new System.Drawing.Point(6, 65);
            this.rbEyedropLeft.Name = "rbEyedropLeft";
            this.rbEyedropLeft.Size = new System.Drawing.Size(79, 17);
            this.rbEyedropLeft.TabIndex = 2;
            this.rbEyedropLeft.TabStop = true;
            this.rbEyedropLeft.Text = "Eyedropper";
            this.rbEyedropLeft.UseVisualStyleBackColor = true;
            // 
            // rbBucketLeft
            // 
            this.rbBucketLeft.AutoSize = true;
            this.rbBucketLeft.Location = new System.Drawing.Point(6, 42);
            this.rbBucketLeft.Name = "rbBucketLeft";
            this.rbBucketLeft.Size = new System.Drawing.Size(59, 17);
            this.rbBucketLeft.TabIndex = 1;
            this.rbBucketLeft.TabStop = true;
            this.rbBucketLeft.Text = "Bucket";
            this.rbBucketLeft.UseVisualStyleBackColor = true;
            // 
            // rbPencilLeft
            // 
            this.rbPencilLeft.AutoSize = true;
            this.rbPencilLeft.Location = new System.Drawing.Point(6, 19);
            this.rbPencilLeft.Name = "rbPencilLeft";
            this.rbPencilLeft.Size = new System.Drawing.Size(54, 17);
            this.rbPencilLeft.TabIndex = 0;
            this.rbPencilLeft.TabStop = true;
            this.rbPencilLeft.Text = "Pencil";
            this.rbPencilLeft.UseVisualStyleBackColor = true;
            // 
            // gbTiletset
            // 
            this.gbTiletset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTiletset.Controls.Add(this.sclTileHeight);
            this.gbTiletset.Controls.Add(this.sclTileWidth);
            this.gbTiletset.Controls.Add(this.btn3DTileAttributes);
            this.gbTiletset.Controls.Add(this.btnTileAttributes);
            this.gbTiletset.Controls.Add(this.TilesetViewer);
            this.gbTiletset.Controls.Add(this.gbAutotile);
            this.gbTiletset.Controls.Add(this.btnAddTilesetAsBackground);
            this.gbTiletset.Controls.Add(this.btnRemoveTileset);
            this.gbTiletset.Controls.Add(this.btnAddTileset);
            this.gbTiletset.Controls.Add(this.lblTileset);
            this.gbTiletset.Controls.Add(this.cboTilesets);
            this.gbTiletset.Location = new System.Drawing.Point(6, 6);
            this.gbTiletset.Name = "gbTiletset";
            this.gbTiletset.Size = new System.Drawing.Size(315, 547);
            this.gbTiletset.TabIndex = 4;
            this.gbTiletset.TabStop = false;
            this.gbTiletset.Text = "Tileset";
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Location = new System.Drawing.Point(292, 117);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 307);
            this.sclTileHeight.TabIndex = 10;
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Location = new System.Drawing.Point(3, 424);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(291, 17);
            this.sclTileWidth.TabIndex = 9;
            // 
            // btn3DTileAttributes
            // 
            this.btn3DTileAttributes.Location = new System.Drawing.Point(160, 88);
            this.btn3DTileAttributes.Name = "btn3DTileAttributes";
            this.btn3DTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btn3DTileAttributes.TabIndex = 8;
            this.btn3DTileAttributes.Text = "3D Tile Attributes";
            this.btn3DTileAttributes.UseVisualStyleBackColor = true;
            // 
            // btnTileAttributes
            // 
            this.btnTileAttributes.Location = new System.Drawing.Point(9, 88);
            this.btnTileAttributes.Name = "btnTileAttributes";
            this.btnTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btnTileAttributes.TabIndex = 7;
            this.btnTileAttributes.Text = "Tile Attributes";
            this.btnTileAttributes.UseVisualStyleBackColor = true;
            // 
            // TilesetViewer
            // 
            this.TilesetViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TilesetViewer.Location = new System.Drawing.Point(6, 117);
            this.TilesetViewer.Name = "TilesetViewer";
            this.TilesetViewer.Size = new System.Drawing.Size(303, 324);
            this.TilesetViewer.TabIndex = 6;
            this.TilesetViewer.Text = "TilesetViewer";
            // 
            // gbAutotile
            // 
            this.gbAutotile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAutotile.Controls.Add(this.btnRemoveAutotile);
            this.gbAutotile.Controls.Add(this.btnAddAutotile);
            this.gbAutotile.Controls.Add(this.lblAutotile);
            this.gbAutotile.Controls.Add(this.cboAutotile);
            this.gbAutotile.Location = new System.Drawing.Point(6, 447);
            this.gbAutotile.Name = "gbAutotile";
            this.gbAutotile.Size = new System.Drawing.Size(303, 94);
            this.gbAutotile.TabIndex = 5;
            this.gbAutotile.TabStop = false;
            this.gbAutotile.Text = "Autotile";
            // 
            // btnRemoveAutotile
            // 
            this.btnRemoveAutotile.Location = new System.Drawing.Point(72, 59);
            this.btnRemoveAutotile.Name = "btnRemoveAutotile";
            this.btnRemoveAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnRemoveAutotile.TabIndex = 3;
            this.btnRemoveAutotile.Text = "Remove";
            this.btnRemoveAutotile.UseVisualStyleBackColor = true;
            // 
            // btnAddAutotile
            // 
            this.btnAddAutotile.Location = new System.Drawing.Point(6, 59);
            this.btnAddAutotile.Name = "btnAddAutotile";
            this.btnAddAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnAddAutotile.TabIndex = 2;
            this.btnAddAutotile.Text = "Add";
            this.btnAddAutotile.UseVisualStyleBackColor = true;
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
            // cboAutotile
            // 
            this.cboAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAutotile.FormattingEnabled = true;
            this.cboAutotile.Location = new System.Drawing.Point(9, 32);
            this.cboAutotile.Name = "cboAutotile";
            this.cboAutotile.Size = new System.Drawing.Size(279, 21);
            this.cboAutotile.TabIndex = 0;
            // 
            // btnAddTilesetAsBackground
            // 
            this.btnAddTilesetAsBackground.Location = new System.Drawing.Point(141, 59);
            this.btnAddTilesetAsBackground.Name = "btnAddTilesetAsBackground";
            this.btnAddTilesetAsBackground.Size = new System.Drawing.Size(159, 23);
            this.btnAddTilesetAsBackground.TabIndex = 4;
            this.btnAddTilesetAsBackground.Text = "Add tileset as background";
            this.btnAddTilesetAsBackground.UseVisualStyleBackColor = true;
            // 
            // btnRemoveTileset
            // 
            this.btnRemoveTileset.Location = new System.Drawing.Point(75, 59);
            this.btnRemoveTileset.Name = "btnRemoveTileset";
            this.btnRemoveTileset.Size = new System.Drawing.Size(60, 23);
            this.btnRemoveTileset.TabIndex = 3;
            this.btnRemoveTileset.Text = "Remove";
            this.btnRemoveTileset.UseVisualStyleBackColor = true;
            // 
            // btnAddTileset
            // 
            this.btnAddTileset.Location = new System.Drawing.Point(9, 59);
            this.btnAddTileset.Name = "btnAddTileset";
            this.btnAddTileset.Size = new System.Drawing.Size(60, 23);
            this.btnAddTileset.TabIndex = 2;
            this.btnAddTileset.Text = "Add";
            this.btnAddTileset.UseVisualStyleBackColor = true;
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
            // cboTilesets
            // 
            this.cboTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTilesets.FormattingEnabled = true;
            this.cboTilesets.Location = new System.Drawing.Point(9, 32);
            this.cboTilesets.Name = "cboTilesets";
            this.cboTilesets.Size = new System.Drawing.Size(291, 21);
            this.cboTilesets.TabIndex = 0;
            // 
            // TabsUserControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "TabsUserControl";
            this.Size = new System.Drawing.Size(333, 697);
            this.tabControl1.ResumeLayout(false);
            this.tabTilesets.ResumeLayout(false);
            this.gbToolsRight.ResumeLayout(false);
            this.gbToolsRight.PerformLayout();
            this.gbToolsLeft.ResumeLayout(false);
            this.gbToolsLeft.PerformLayout();
            this.gbTiletset.ResumeLayout(false);
            this.gbTiletset.PerformLayout();
            this.gbAutotile.ResumeLayout(false);
            this.gbAutotile.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.TabPage tabTilesets;
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.GroupBox gbToolsRight;
        private System.Windows.Forms.GroupBox gbToolsLeft;
        private System.Windows.Forms.GroupBox gbTiletset;
        private System.Windows.Forms.GroupBox gbAutotile;
        private System.Windows.Forms.Label lblAutotile;
        private System.Windows.Forms.Label lblTileset;
        public System.Windows.Forms.Button btn3DTileAttributes;
        public System.Windows.Forms.Button btnTileAttributes;
        public System.Windows.Forms.RadioButton rbEyedropRight;
        public System.Windows.Forms.RadioButton rbBucketRight;
        public System.Windows.Forms.RadioButton rbPencilRight;
        public System.Windows.Forms.RadioButton rbEyedropLeft;
        public System.Windows.Forms.RadioButton rbBucketLeft;
        public System.Windows.Forms.RadioButton rbPencilLeft;
        public GameScreens.BattleMapScreen.TilesetViewerControl TilesetViewer;
        public System.Windows.Forms.Button btnRemoveAutotile;
        public System.Windows.Forms.Button btnAddAutotile;
        public System.Windows.Forms.ComboBox cboAutotile;
        public System.Windows.Forms.Button btnAddTilesetAsBackground;
        public System.Windows.Forms.Button btnRemoveTileset;
        public System.Windows.Forms.Button btnAddTileset;
        public System.Windows.Forms.ComboBox cboTilesets;
        public System.Windows.Forms.VScrollBar sclTileHeight;
        public System.Windows.Forms.HScrollBar sclTileWidth;
    }
}
