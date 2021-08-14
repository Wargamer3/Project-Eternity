namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class MapEditor
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
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.BattleMapViewer = new ProjectEternity.GameScreens.BattleMapScreen.BattleMapViewerControl();
            this.btnPreviewMap = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbTilesets = new System.Windows.Forms.GroupBox();
            this.TilesetViewer = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.btnTileAttributes = new System.Windows.Forms.Button();
            this.btnRemoveTile = new System.Windows.Forms.Button();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.btnAddNewTileSetAsBackground = new System.Windows.Forms.Button();
            this.cboTiles = new System.Windows.Forms.ComboBox();
            this.rbEditMap = new System.Windows.Forms.RadioButton();
            this.rbSelectTilesToReplace = new System.Windows.Forms.RadioButton();
            this.gbMap.SuspendLayout();
            this.gbTilesets.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMap
            // 
            this.gbMap.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbMap.Controls.Add(this.BattleMapViewer);
            this.gbMap.Location = new System.Drawing.Point(12, 12);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(456, 521);
            this.gbMap.TabIndex = 0;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Map";
            // 
            // BattleMapViewer
            // 
            this.BattleMapViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BattleMapViewer.Location = new System.Drawing.Point(3, 16);
            this.BattleMapViewer.Name = "BattleMapViewer";
            this.BattleMapViewer.Size = new System.Drawing.Size(450, 502);
            this.BattleMapViewer.TabIndex = 0;
            this.BattleMapViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseDown);
            this.BattleMapViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseMove);
            this.BattleMapViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseUp);
            // 
            // btnPreviewMap
            // 
            this.btnPreviewMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreviewMap.Location = new System.Drawing.Point(12, 539);
            this.btnPreviewMap.Name = "btnPreviewMap";
            this.btnPreviewMap.Size = new System.Drawing.Size(116, 23);
            this.btnPreviewMap.TabIndex = 1;
            this.btnPreviewMap.Text = "Preview Map";
            this.btnPreviewMap.UseVisualStyleBackColor = true;
            this.btnPreviewMap.Click += new System.EventHandler(this.btnPreviewMap_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(558, 539);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(639, 539);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // gbTilesets
            // 
            this.gbTilesets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTilesets.Controls.Add(this.TilesetViewer);
            this.gbTilesets.Controls.Add(this.btnTileAttributes);
            this.gbTilesets.Controls.Add(this.btnRemoveTile);
            this.gbTilesets.Controls.Add(this.btnAddTile);
            this.gbTilesets.Controls.Add(this.btnAddNewTileSetAsBackground);
            this.gbTilesets.Controls.Add(this.cboTiles);
            this.gbTilesets.Location = new System.Drawing.Point(474, 12);
            this.gbTilesets.Name = "gbTilesets";
            this.gbTilesets.Size = new System.Drawing.Size(243, 521);
            this.gbTilesets.TabIndex = 4;
            this.gbTilesets.TabStop = false;
            this.gbTilesets.Text = "Tilesets";
            // 
            // TilesetViewer
            // 
            this.TilesetViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TilesetViewer.Location = new System.Drawing.Point(3, 165);
            this.TilesetViewer.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewer.Name = "TilesetViewer";
            this.TilesetViewer.Size = new System.Drawing.Size(237, 353);
            this.TilesetViewer.TabIndex = 16;
            this.TilesetViewer.Click += new System.EventHandler(this.TilesetViewer_Click);
            // 
            // btnTileAttributes
            // 
            this.btnTileAttributes.Location = new System.Drawing.Point(10, 136);
            this.btnTileAttributes.Name = "btnTileAttributes";
            this.btnTileAttributes.Size = new System.Drawing.Size(221, 23);
            this.btnTileAttributes.TabIndex = 13;
            this.btnTileAttributes.Text = "Tile attributes";
            this.btnTileAttributes.UseVisualStyleBackColor = true;
            this.btnTileAttributes.Click += new System.EventHandler(this.btnTileAttributes_Click);
            // 
            // btnRemoveTile
            // 
            this.btnRemoveTile.Location = new System.Drawing.Point(10, 107);
            this.btnRemoveTile.Name = "btnRemoveTile";
            this.btnRemoveTile.Size = new System.Drawing.Size(221, 23);
            this.btnRemoveTile.TabIndex = 11;
            this.btnRemoveTile.Text = "Remove active tile set";
            this.btnRemoveTile.UseVisualStyleBackColor = true;
            this.btnRemoveTile.Click += new System.EventHandler(this.btnRemoveTile_Click);
            // 
            // btnAddTile
            // 
            this.btnAddTile.Location = new System.Drawing.Point(10, 49);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(221, 23);
            this.btnAddTile.TabIndex = 10;
            this.btnAddTile.Text = "Add new tile set";
            this.btnAddTile.UseVisualStyleBackColor = true;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // btnAddNewTileSetAsBackground
            // 
            this.btnAddNewTileSetAsBackground.Location = new System.Drawing.Point(10, 78);
            this.btnAddNewTileSetAsBackground.Name = "btnAddNewTileSetAsBackground";
            this.btnAddNewTileSetAsBackground.Size = new System.Drawing.Size(221, 23);
            this.btnAddNewTileSetAsBackground.TabIndex = 12;
            this.btnAddNewTileSetAsBackground.Text = "Add new tile set as background";
            this.btnAddNewTileSetAsBackground.UseVisualStyleBackColor = true;
            this.btnAddNewTileSetAsBackground.Click += new System.EventHandler(this.btnAddNewTileSetAsBackground_Click);
            // 
            // cboTiles
            // 
            this.cboTiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTiles.FormattingEnabled = true;
            this.cboTiles.Location = new System.Drawing.Point(6, 19);
            this.cboTiles.Name = "cboTiles";
            this.cboTiles.Size = new System.Drawing.Size(227, 21);
            this.cboTiles.TabIndex = 9;
            this.cboTiles.SelectedIndexChanged += new System.EventHandler(this.cboTiles_SelectedIndexChanged);
            // 
            // rbEditMap
            // 
            this.rbEditMap.AutoSize = true;
            this.rbEditMap.Checked = true;
            this.rbEditMap.Location = new System.Drawing.Point(134, 545);
            this.rbEditMap.Name = "rbEditMap";
            this.rbEditMap.Size = new System.Drawing.Size(66, 17);
            this.rbEditMap.TabIndex = 5;
            this.rbEditMap.TabStop = true;
            this.rbEditMap.Text = "Edit map";
            this.rbEditMap.UseVisualStyleBackColor = true;
            // 
            // rbSelectTilesToReplace
            // 
            this.rbSelectTilesToReplace.AutoSize = true;
            this.rbSelectTilesToReplace.Location = new System.Drawing.Point(207, 545);
            this.rbSelectTilesToReplace.Name = "rbSelectTilesToReplace";
            this.rbSelectTilesToReplace.Size = new System.Drawing.Size(126, 17);
            this.rbSelectTilesToReplace.TabIndex = 6;
            this.rbSelectTilesToReplace.Text = "Select tiles to replace";
            this.rbSelectTilesToReplace.UseVisualStyleBackColor = true;
            // 
            // MapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 574);
            this.Controls.Add(this.rbSelectTilesToReplace);
            this.Controls.Add(this.rbEditMap);
            this.Controls.Add(this.gbTilesets);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnPreviewMap);
            this.Controls.Add(this.gbMap);
            this.Name = "MapEditor";
            this.Text = "Map Editor";
            this.gbMap.ResumeLayout(false);
            this.gbTilesets.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMap;
        private System.Windows.Forms.Button btnPreviewMap;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        public BattleMapScreen.BattleMapViewerControl BattleMapViewer;
        private System.Windows.Forms.GroupBox gbTilesets;
        protected BattleMapScreen.TilesetViewerControl TilesetViewer;
        private System.Windows.Forms.Button btnTileAttributes;
        private System.Windows.Forms.Button btnRemoveTile;
        private System.Windows.Forms.Button btnAddTile;
        private System.Windows.Forms.Button btnAddNewTileSetAsBackground;
        protected System.Windows.Forms.ComboBox cboTiles;
        private System.Windows.Forms.RadioButton rbEditMap;
        private System.Windows.Forms.RadioButton rbSelectTilesToReplace;
    }
}