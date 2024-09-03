namespace ProjectEternity.Editors.MapEditor
{
    partial class ProjectEternityMapEditor
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
            this.tabToolBox = new System.Windows.Forms.TabControl();
            this.tabTiles = new System.Windows.Forms.TabPage();
            this.btn3DTileAttributes = new System.Windows.Forms.Button();
            this.TilesetViewer = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.btnTileAttributes = new System.Windows.Forms.Button();
            this.sclTileHeight = new System.Windows.Forms.VScrollBar();
            this.sclTileWidth = new System.Windows.Forms.HScrollBar();
            this.btnRemoveTile = new System.Windows.Forms.Button();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.btnAddNewTileSetAsBackground = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cboTiles = new System.Windows.Forms.ComboBox();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmMapProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.BattleMapViewer = new ProjectEternity.GameScreens.BattleMapScreen.BattleMapViewerControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tslInformation = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmGlobalEnvironment = new System.Windows.Forms.ToolStripMenuItem();
            this.tabToolBox.SuspendLayout();
            this.tabTiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolBox
            // 
            this.tabToolBox.Controls.Add(this.tabTiles);
            this.tabToolBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabToolBox.Location = new System.Drawing.Point(0, 0);
            this.tabToolBox.Name = "tabToolBox";
            this.tabToolBox.SelectedIndex = 0;
            this.tabToolBox.Size = new System.Drawing.Size(333, 523);
            this.tabToolBox.TabIndex = 1;
            this.tabToolBox.SelectedIndexChanged += new System.EventHandler(this.tabToolBox_SelectedIndexChanged);
            // 
            // tabTiles
            // 
            this.tabTiles.Controls.Add(this.btn3DTileAttributes);
            this.tabTiles.Controls.Add(this.TilesetViewer);
            this.tabTiles.Controls.Add(this.btnTileAttributes);
            this.tabTiles.Controls.Add(this.sclTileHeight);
            this.tabTiles.Controls.Add(this.sclTileWidth);
            this.tabTiles.Controls.Add(this.btnRemoveTile);
            this.tabTiles.Controls.Add(this.btnAddTile);
            this.tabTiles.Controls.Add(this.btnAddNewTileSetAsBackground);
            this.tabTiles.Controls.Add(this.label1);
            this.tabTiles.Controls.Add(this.cboTiles);
            this.tabTiles.Location = new System.Drawing.Point(4, 22);
            this.tabTiles.Name = "tabTiles";
            this.tabTiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabTiles.Size = new System.Drawing.Size(325, 497);
            this.tabTiles.TabIndex = 2;
            this.tabTiles.Text = "Tiles";
            this.tabTiles.UseVisualStyleBackColor = true;
            // 
            // btn3DTileAttributes
            // 
            this.btn3DTileAttributes.Location = new System.Drawing.Point(126, 111);
            this.btn3DTileAttributes.Name = "btn3DTileAttributes";
            this.btn3DTileAttributes.Size = new System.Drawing.Size(105, 23);
            this.btn3DTileAttributes.TabIndex = 8;
            this.btn3DTileAttributes.Text = "3D Tile attributes";
            this.btn3DTileAttributes.UseVisualStyleBackColor = true;
            this.btn3DTileAttributes.Click += new System.EventHandler(this.btn3DTileAttributes_Click);
            // 
            // TilesetViewer
            // 
            this.TilesetViewer.Location = new System.Drawing.Point(3, 140);
            this.TilesetViewer.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewer.Name = "TilesetViewer";
            this.TilesetViewer.Size = new System.Drawing.Size(302, 340);
            this.TilesetViewer.TabIndex = 7;
            this.TilesetViewer.Click += new System.EventHandler(this.TileViewer_Click);
            // 
            // btnTileAttributes
            // 
            this.btnTileAttributes.Location = new System.Drawing.Point(10, 111);
            this.btnTileAttributes.Name = "btnTileAttributes";
            this.btnTileAttributes.Size = new System.Drawing.Size(105, 23);
            this.btnTileAttributes.TabIndex = 6;
            this.btnTileAttributes.Text = "Tile attributes";
            this.btnTileAttributes.UseVisualStyleBackColor = true;
            this.btnTileAttributes.Click += new System.EventHandler(this.btnTileAttributes_Click);
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Location = new System.Drawing.Point(305, 140);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 340);
            this.sclTileHeight.TabIndex = 6;
            this.sclTileHeight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileHeight_Scroll);
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sclTileWidth.Location = new System.Drawing.Point(3, 480);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(302, 17);
            this.sclTileWidth.TabIndex = 6;
            this.sclTileWidth.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileWidth_Scroll);
            // 
            // btnRemoveTile
            // 
            this.btnRemoveTile.Location = new System.Drawing.Point(148, 53);
            this.btnRemoveTile.Name = "btnRemoveTile";
            this.btnRemoveTile.Size = new System.Drawing.Size(125, 23);
            this.btnRemoveTile.TabIndex = 4;
            this.btnRemoveTile.Text = "Remove";
            this.btnRemoveTile.UseVisualStyleBackColor = true;
            this.btnRemoveTile.Click += new System.EventHandler(this.btnRemoveTile_Click);
            // 
            // btnAddTile
            // 
            this.btnAddTile.Location = new System.Drawing.Point(10, 53);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(132, 23);
            this.btnAddTile.TabIndex = 3;
            this.btnAddTile.Text = "Add";
            this.btnAddTile.UseVisualStyleBackColor = true;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // btnAddNewTileSetAsBackground
            // 
            this.btnAddNewTileSetAsBackground.Location = new System.Drawing.Point(10, 82);
            this.btnAddNewTileSetAsBackground.Name = "btnAddNewTileSetAsBackground";
            this.btnAddNewTileSetAsBackground.Size = new System.Drawing.Size(263, 23);
            this.btnAddNewTileSetAsBackground.TabIndex = 5;
            this.btnAddNewTileSetAsBackground.Text = "Add new tile set as background";
            this.btnAddNewTileSetAsBackground.UseVisualStyleBackColor = true;
            this.btnAddNewTileSetAsBackground.Click += new System.EventHandler(this.btnAddNewTileSetAsBackground_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Active tile set";
            // 
            // cboTiles
            // 
            this.cboTiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTiles.FormattingEnabled = true;
            this.cboTiles.Location = new System.Drawing.Point(6, 23);
            this.cboTiles.Name = "cboTiles";
            this.cboTiles.Size = new System.Drawing.Size(267, 21);
            this.cboTiles.TabIndex = 2;
            this.cboTiles.SelectedIndexChanged += new System.EventHandler(this.cboTiles_SelectedIndexChanged);
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmMapProperties
            // 
            this.tsmMapProperties.Name = "tsmMapProperties";
            this.tsmMapProperties.Size = new System.Drawing.Size(99, 20);
            this.tsmMapProperties.Text = "Map properties";
            this.tsmMapProperties.Click += new System.EventHandler(this.tsmMapProperties_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.BattleMapViewer);
            this.splitContainer.Panel1MinSize = 664;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabToolBox);
            this.splitContainer.Panel2MinSize = 252;
            this.splitContainer.Size = new System.Drawing.Size(1247, 527);
            this.splitContainer.SplitterDistance = 906;
            this.splitContainer.TabIndex = 10;
            // 
            // BattleMapViewer
            // 
            this.BattleMapViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BattleMapViewer.Location = new System.Drawing.Point(0, 0);
            this.BattleMapViewer.Name = "BattleMapViewer";
            this.BattleMapViewer.Size = new System.Drawing.Size(902, 523);
            this.BattleMapViewer.TabIndex = 0;
            this.BattleMapViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseDown);
            this.BattleMapViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseMove);
            this.BattleMapViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseUp);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslInformation});
            this.statusStrip.Location = new System.Drawing.Point(0, 551);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1247, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tslInformation
            // 
            this.tslInformation.Name = "tslInformation";
            this.tslInformation.Size = new System.Drawing.Size(0, 17);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmMapProperties,
            this.tsmGlobalEnvironment});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(1247, 24);
            this.mnuToolBar.TabIndex = 9;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmGlobalEnvironment
            // 
            this.tsmGlobalEnvironment.Name = "tsmGlobalEnvironment";
            this.tsmGlobalEnvironment.Size = new System.Drawing.Size(124, 20);
            this.tsmGlobalEnvironment.Text = "Global Environment";
            this.tsmGlobalEnvironment.Click += new System.EventHandler(this.tsmGlobalEnvironment_Click);
            // 
            // ProjectEternityMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1247, 573);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityMapEditor";
            this.Shown += new System.EventHandler(this.ProjectEternityMapEditor_Shown);
            this.tabToolBox.ResumeLayout(false);
            this.tabTiles.ResumeLayout(false);
            this.tabTiles.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TabControl tabToolBox;
        private System.Windows.Forms.TabPage tabTiles;
        private System.Windows.Forms.Button btnTileAttributes;
        private System.Windows.Forms.VScrollBar sclTileHeight;
        private System.Windows.Forms.HScrollBar sclTileWidth;
        private System.Windows.Forms.Button btnRemoveTile;
        private System.Windows.Forms.Button btnAddTile;
        private System.Windows.Forms.Button btnAddNewTileSetAsBackground;
        private System.Windows.Forms.Label label1;
        protected System.Windows.Forms.ComboBox cboTiles;

        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmMapProperties;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tslInformation;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        protected GameScreens.BattleMapScreen.BattleMapViewerControl BattleMapViewer;
        protected ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl TilesetViewer;
        private System.Windows.Forms.Button btn3DTileAttributes;
        private System.Windows.Forms.ToolStripMenuItem tsmGlobalEnvironment;
    }
}
