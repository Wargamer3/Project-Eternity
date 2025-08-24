using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.TilesetEditor
{
    partial class ProjectEternityAutotileTilesetPresetEditor
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
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmImportTileset = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTileset = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTilesetType = new System.Windows.Forms.ComboBox();
            this.txtTilesetName = new System.Windows.Forms.TextBox();
            this.lblActiveTileset = new System.Windows.Forms.Label();
            this.sclTileWidth = new System.Windows.Forms.HScrollBar();
            this.sclTileHeight = new System.Windows.Forms.VScrollBar();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.gbTileInformation = new System.Windows.Forms.GroupBox();
            this.btnEditTerrainTypes = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstTerrainBonus = new System.Windows.Forms.ListBox();
            this.txtBonusValue = new System.Windows.Forms.NumericUpDown();
            this.lblTerrainBonusType = new System.Windows.Forms.Label();
            this.cboTerrainBonusType = new System.Windows.Forms.ComboBox();
            this.btnClearBonuses = new System.Windows.Forms.Button();
            this.lblTerrainBonusActivation = new System.Windows.Forms.Label();
            this.btnRemoveBonus = new System.Windows.Forms.Button();
            this.lblBonusValue = new System.Windows.Forms.Label();
            this.btnAddNewBonus = new System.Windows.Forms.Button();
            this.cboTerrainBonusActivation = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBattleAnimationForeground = new System.Windows.Forms.Label();
            this.cboBattleAnimationForeground = new System.Windows.Forms.ComboBox();
            this.lblBattleAnimationBackground = new System.Windows.Forms.Label();
            this.cboBattleAnimationBackground = new System.Windows.Forms.ComboBox();
            this.btnNewBattleAnimationBackground = new System.Windows.Forms.Button();
            this.btnDeleteBattleAnimationBackground = new System.Windows.Forms.Button();
            this.lblTerrainType = new System.Windows.Forms.Label();
            this.cboTerrainType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTilesetMain = new System.Windows.Forms.TabPage();
            this.viewerTilesetMain = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.tabTilesetRiver = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.viewerTilesetRiver = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.tabTilesetShoal = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar2 = new System.Windows.Forms.VScrollBar();
            this.tilesetViewerControl1 = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.tabTilesetWaterfall = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
            this.vScrollBar3 = new System.Windows.Forms.VScrollBar();
            this.tilesetViewerControl2 = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.mnuToolBar.SuspendLayout();
            this.gbTileset.SuspendLayout();
            this.gbTileInformation.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabTilesetMain.SuspendLayout();
            this.tabTilesetRiver.SuspendLayout();
            this.tabTilesetShoal.SuspendLayout();
            this.tabTilesetWaterfall.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmImportTileset});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(935, 24);
            this.mnuToolBar.TabIndex = 17;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmImportTileset
            // 
            this.tsmImportTileset.Name = "tsmImportTileset";
            this.tsmImportTileset.Size = new System.Drawing.Size(91, 20);
            this.tsmImportTileset.Text = "Import Tileset";
            this.tsmImportTileset.Click += new System.EventHandler(this.tsmImportTileset_Click);
            // 
            // gbTileset
            // 
            this.gbTileset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTileset.Controls.Add(this.label1);
            this.gbTileset.Controls.Add(this.cbTilesetType);
            this.gbTileset.Location = new System.Drawing.Point(12, 27);
            this.gbTileset.Name = "gbTileset";
            this.gbTileset.Size = new System.Drawing.Size(471, 493);
            this.gbTileset.TabIndex = 19;
            this.gbTileset.TabStop = false;
            this.gbTileset.Text = "Tilesets";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Autotile type";
            // 
            // cbTilesetType
            // 
            this.cbTilesetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTilesetType.FormattingEnabled = true;
            this.cbTilesetType.Items.AddRange(new object[] {
            "Normal",
            "Road",
            "Water",
            "Ocean",
            "Pipes"});
            this.cbTilesetType.Location = new System.Drawing.Point(6, 32);
            this.cbTilesetType.Name = "cbTilesetType";
            this.cbTilesetType.Size = new System.Drawing.Size(227, 21);
            this.cbTilesetType.TabIndex = 23;
            this.cbTilesetType.SelectedIndexChanged += new System.EventHandler(this.cbTilesetType_SelectedIndexChanged);
            // 
            // txtTilesetName
            // 
            this.txtTilesetName.Location = new System.Drawing.Point(6, 19);
            this.txtTilesetName.Name = "txtTilesetName";
            this.txtTilesetName.ReadOnly = true;
            this.txtTilesetName.Size = new System.Drawing.Size(227, 20);
            this.txtTilesetName.TabIndex = 20;
            // 
            // lblActiveTileset
            // 
            this.lblActiveTileset.AutoSize = true;
            this.lblActiveTileset.Location = new System.Drawing.Point(6, 3);
            this.lblActiveTileset.Name = "lblActiveTileset";
            this.lblActiveTileset.Size = new System.Drawing.Size(67, 13);
            this.lblActiveTileset.TabIndex = 8;
            this.lblActiveTileset.Text = "Active tileset";
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sclTileWidth.Location = new System.Drawing.Point(3, 379);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(431, 17);
            this.sclTileWidth.TabIndex = 8;
            this.sclTileWidth.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileWidth_Scroll);
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sclTileHeight.Location = new System.Drawing.Point(434, 76);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 303);
            this.sclTileHeight.TabIndex = 7;
            this.sclTileHeight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileHeight_Scroll);
            // 
            // btnAddTile
            // 
            this.btnAddTile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTile.Location = new System.Drawing.Point(6, 52);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(448, 23);
            this.btnAddTile.TabIndex = 10;
            this.btnAddTile.Text = "Select tileset";
            this.btnAddTile.UseVisualStyleBackColor = true;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // gbTileInformation
            // 
            this.gbTileInformation.Controls.Add(this.btnEditTerrainTypes);
            this.gbTileInformation.Controls.Add(this.groupBox2);
            this.gbTileInformation.Controls.Add(this.groupBox1);
            this.gbTileInformation.Controls.Add(this.lblTerrainType);
            this.gbTileInformation.Controls.Add(this.cboTerrainType);
            this.gbTileInformation.Location = new System.Drawing.Point(489, 27);
            this.gbTileInformation.Name = "gbTileInformation";
            this.gbTileInformation.Size = new System.Drawing.Size(434, 493);
            this.gbTileInformation.TabIndex = 20;
            this.gbTileInformation.TabStop = false;
            this.gbTileInformation.Text = "Tile Information";
            // 
            // btnEditTerrainTypes
            // 
            this.btnEditTerrainTypes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditTerrainTypes.Location = new System.Drawing.Point(157, 29);
            this.btnEditTerrainTypes.Name = "btnEditTerrainTypes";
            this.btnEditTerrainTypes.Size = new System.Drawing.Size(66, 24);
            this.btnEditTerrainTypes.TabIndex = 24;
            this.btnEditTerrainTypes.Text = "Edit";
            this.btnEditTerrainTypes.UseVisualStyleBackColor = true;
            this.btnEditTerrainTypes.Click += new System.EventHandler(this.btnEditTerrainTypes_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lstTerrainBonus);
            this.groupBox2.Controls.Add(this.txtBonusValue);
            this.groupBox2.Controls.Add(this.lblTerrainBonusType);
            this.groupBox2.Controls.Add(this.cboTerrainBonusType);
            this.groupBox2.Controls.Add(this.btnClearBonuses);
            this.groupBox2.Controls.Add(this.lblTerrainBonusActivation);
            this.groupBox2.Controls.Add(this.btnRemoveBonus);
            this.groupBox2.Controls.Add(this.lblBonusValue);
            this.groupBox2.Controls.Add(this.btnAddNewBonus);
            this.groupBox2.Controls.Add(this.cboTerrainBonusActivation);
            this.groupBox2.Location = new System.Drawing.Point(229, 19);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(199, 271);
            this.groupBox2.TabIndex = 23;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Terrain Bonus";
            // 
            // lstTerrainBonus
            // 
            this.lstTerrainBonus.FormattingEnabled = true;
            this.lstTerrainBonus.Location = new System.Drawing.Point(12, 19);
            this.lstTerrainBonus.Name = "lstTerrainBonus";
            this.lstTerrainBonus.Size = new System.Drawing.Size(177, 82);
            this.lstTerrainBonus.TabIndex = 5;
            // 
            // txtBonusValue
            // 
            this.txtBonusValue.Location = new System.Drawing.Point(12, 200);
            this.txtBonusValue.Name = "txtBonusValue";
            this.txtBonusValue.Size = new System.Drawing.Size(177, 20);
            this.txtBonusValue.TabIndex = 18;
            // 
            // lblTerrainBonusType
            // 
            this.lblTerrainBonusType.Location = new System.Drawing.Point(9, 104);
            this.lblTerrainBonusType.Name = "lblTerrainBonusType";
            this.lblTerrainBonusType.Size = new System.Drawing.Size(100, 13);
            this.lblTerrainBonusType.TabIndex = 8;
            this.lblTerrainBonusType.Text = "Terrain bonus type";
            // 
            // cboTerrainBonusType
            // 
            this.cboTerrainBonusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusType.FormattingEnabled = true;
            this.cboTerrainBonusType.Location = new System.Drawing.Point(12, 120);
            this.cboTerrainBonusType.Name = "cboTerrainBonusType";
            this.cboTerrainBonusType.Size = new System.Drawing.Size(177, 21);
            this.cboTerrainBonusType.TabIndex = 7;
            // 
            // btnClearBonuses
            // 
            this.btnClearBonuses.Location = new System.Drawing.Point(132, 242);
            this.btnClearBonuses.Name = "btnClearBonuses";
            this.btnClearBonuses.Size = new System.Drawing.Size(57, 23);
            this.btnClearBonuses.TabIndex = 14;
            this.btnClearBonuses.Text = "Clear";
            this.btnClearBonuses.UseVisualStyleBackColor = true;
            // 
            // lblTerrainBonusActivation
            // 
            this.lblTerrainBonusActivation.Location = new System.Drawing.Point(12, 144);
            this.lblTerrainBonusActivation.Name = "lblTerrainBonusActivation";
            this.lblTerrainBonusActivation.Size = new System.Drawing.Size(121, 13);
            this.lblTerrainBonusActivation.TabIndex = 16;
            this.lblTerrainBonusActivation.Text = "Terrain bonus activation";
            // 
            // btnRemoveBonus
            // 
            this.btnRemoveBonus.Location = new System.Drawing.Point(69, 242);
            this.btnRemoveBonus.Name = "btnRemoveBonus";
            this.btnRemoveBonus.Size = new System.Drawing.Size(57, 23);
            this.btnRemoveBonus.TabIndex = 13;
            this.btnRemoveBonus.Text = "Remove";
            this.btnRemoveBonus.UseVisualStyleBackColor = true;
            // 
            // lblBonusValue
            // 
            this.lblBonusValue.AutoSize = true;
            this.lblBonusValue.Location = new System.Drawing.Point(12, 184);
            this.lblBonusValue.Name = "lblBonusValue";
            this.lblBonusValue.Size = new System.Drawing.Size(66, 13);
            this.lblBonusValue.TabIndex = 10;
            this.lblBonusValue.Text = "Bonus value";
            // 
            // btnAddNewBonus
            // 
            this.btnAddNewBonus.Location = new System.Drawing.Point(6, 242);
            this.btnAddNewBonus.Name = "btnAddNewBonus";
            this.btnAddNewBonus.Size = new System.Drawing.Size(57, 23);
            this.btnAddNewBonus.TabIndex = 12;
            this.btnAddNewBonus.Text = "Add";
            this.btnAddNewBonus.UseVisualStyleBackColor = true;
            // 
            // cboTerrainBonusActivation
            // 
            this.cboTerrainBonusActivation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusActivation.FormattingEnabled = true;
            this.cboTerrainBonusActivation.Location = new System.Drawing.Point(12, 160);
            this.cboTerrainBonusActivation.Name = "cboTerrainBonusActivation";
            this.cboTerrainBonusActivation.Size = new System.Drawing.Size(177, 21);
            this.cboTerrainBonusActivation.TabIndex = 15;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblBattleAnimationForeground);
            this.groupBox1.Controls.Add(this.cboBattleAnimationForeground);
            this.groupBox1.Controls.Add(this.lblBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.cboBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.btnNewBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.btnDeleteBattleAnimationBackground);
            this.groupBox1.Location = new System.Drawing.Point(9, 59);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 141);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Battle Background";
            // 
            // lblBattleAnimationForeground
            // 
            this.lblBattleAnimationForeground.AutoSize = true;
            this.lblBattleAnimationForeground.Location = new System.Drawing.Point(6, 60);
            this.lblBattleAnimationForeground.Name = "lblBattleAnimationForeground";
            this.lblBattleAnimationForeground.Size = new System.Drawing.Size(136, 13);
            this.lblBattleAnimationForeground.TabIndex = 33;
            this.lblBattleAnimationForeground.Text = "Battle animation foreground";
            // 
            // cboBattleAnimationForeground
            // 
            this.cboBattleAnimationForeground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationForeground.FormattingEnabled = true;
            this.cboBattleAnimationForeground.Location = new System.Drawing.Point(6, 76);
            this.cboBattleAnimationForeground.Name = "cboBattleAnimationForeground";
            this.cboBattleAnimationForeground.Size = new System.Drawing.Size(202, 21);
            this.cboBattleAnimationForeground.TabIndex = 32;
            this.cboBattleAnimationForeground.SelectedIndexChanged += new System.EventHandler(this.cboBattleAnimationForeground_SelectedIndexChanged);
            // 
            // lblBattleAnimationBackground
            // 
            this.lblBattleAnimationBackground.AutoSize = true;
            this.lblBattleAnimationBackground.Location = new System.Drawing.Point(6, 20);
            this.lblBattleAnimationBackground.Name = "lblBattleAnimationBackground";
            this.lblBattleAnimationBackground.Size = new System.Drawing.Size(142, 13);
            this.lblBattleAnimationBackground.TabIndex = 31;
            this.lblBattleAnimationBackground.Text = "Battle animation background";
            // 
            // cboBattleAnimationBackground
            // 
            this.cboBattleAnimationBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationBackground.FormattingEnabled = true;
            this.cboBattleAnimationBackground.Location = new System.Drawing.Point(6, 36);
            this.cboBattleAnimationBackground.Name = "cboBattleAnimationBackground";
            this.cboBattleAnimationBackground.Size = new System.Drawing.Size(202, 21);
            this.cboBattleAnimationBackground.TabIndex = 30;
            this.cboBattleAnimationBackground.SelectedIndexChanged += new System.EventHandler(this.cboBattleAnimationBackground_SelectedIndexChanged);
            // 
            // btnNewBattleAnimationBackground
            // 
            this.btnNewBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewBattleAnimationBackground.Location = new System.Drawing.Point(6, 111);
            this.btnNewBattleAnimationBackground.Name = "btnNewBattleAnimationBackground";
            this.btnNewBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnNewBattleAnimationBackground.TabIndex = 1;
            this.btnNewBattleAnimationBackground.Text = "New";
            this.btnNewBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnNewBattleAnimationBackground.Click += new System.EventHandler(this.btnNewBattleAnimationBackground_Click);
            // 
            // btnDeleteBattleAnimationBackground
            // 
            this.btnDeleteBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteBattleAnimationBackground.Location = new System.Drawing.Point(87, 111);
            this.btnDeleteBattleAnimationBackground.Name = "btnDeleteBattleAnimationBackground";
            this.btnDeleteBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnDeleteBattleAnimationBackground.TabIndex = 2;
            this.btnDeleteBattleAnimationBackground.Text = "Delete";
            this.btnDeleteBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnDeleteBattleAnimationBackground.Click += new System.EventHandler(this.btnDeleteBattleAnimationBackground_Click);
            // 
            // lblTerrainType
            // 
            this.lblTerrainType.Location = new System.Drawing.Point(6, 16);
            this.lblTerrainType.Name = "lblTerrainType";
            this.lblTerrainType.Size = new System.Drawing.Size(71, 13);
            this.lblTerrainType.TabIndex = 0;
            this.lblTerrainType.Text = "Terrain type";
            // 
            // cboTerrainType
            // 
            this.cboTerrainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainType.FormattingEnabled = true;
            this.cboTerrainType.Items.AddRange(new object[] {
            "Air",
            "Land",
            "Sea",
            "Space"});
            this.cboTerrainType.Location = new System.Drawing.Point(6, 32);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(145, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTilesetMain);
            this.tabControl1.Controls.Add(this.tabTilesetRiver);
            this.tabControl1.Controls.Add(this.tabTilesetShoal);
            this.tabControl1.Controls.Add(this.tabTilesetWaterfall);
            this.tabControl1.Location = new System.Drawing.Point(12, 86);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(465, 428);
            this.tabControl1.TabIndex = 25;
            // 
            // tabTilesetMain
            // 
            this.tabTilesetMain.Controls.Add(this.btnAddTile);
            this.tabTilesetMain.Controls.Add(this.sclTileWidth);
            this.tabTilesetMain.Controls.Add(this.txtTilesetName);
            this.tabTilesetMain.Controls.Add(this.sclTileHeight);
            this.tabTilesetMain.Controls.Add(this.viewerTilesetMain);
            this.tabTilesetMain.Controls.Add(this.lblActiveTileset);
            this.tabTilesetMain.Location = new System.Drawing.Point(4, 22);
            this.tabTilesetMain.Name = "tabTilesetMain";
            this.tabTilesetMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabTilesetMain.Size = new System.Drawing.Size(457, 402);
            this.tabTilesetMain.TabIndex = 0;
            this.tabTilesetMain.Text = "Main";
            this.tabTilesetMain.UseVisualStyleBackColor = true;
            // 
            // viewerTilesetMain
            // 
            this.viewerTilesetMain.Location = new System.Drawing.Point(5, 76);
            this.viewerTilesetMain.Name = "viewerTilesetMain";
            this.viewerTilesetMain.Size = new System.Drawing.Size(426, 300);
            this.viewerTilesetMain.TabIndex = 25;
            this.viewerTilesetMain.Text = "tilesetViewerControl1";
            this.viewerTilesetMain.Click += new System.EventHandler(this.viewerTileset_Click);
            this.viewerTilesetMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.viewerTileset_MouseDown);
            this.viewerTilesetMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.viewerTileset_MouseMove);
            // 
            // tabTilesetRiver
            // 
            this.tabTilesetRiver.Controls.Add(this.button1);
            this.tabTilesetRiver.Controls.Add(this.hScrollBar1);
            this.tabTilesetRiver.Controls.Add(this.vScrollBar1);
            this.tabTilesetRiver.Controls.Add(this.viewerTilesetRiver);
            this.tabTilesetRiver.Location = new System.Drawing.Point(4, 22);
            this.tabTilesetRiver.Name = "tabTilesetRiver";
            this.tabTilesetRiver.Padding = new System.Windows.Forms.Padding(3);
            this.tabTilesetRiver.Size = new System.Drawing.Size(457, 361);
            this.tabTilesetRiver.TabIndex = 1;
            this.tabTilesetRiver.Text = "River";
            this.tabTilesetRiver.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(5, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(448, 23);
            this.button1.TabIndex = 28;
            this.button1.Text = "Select tileset";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar1.Location = new System.Drawing.Point(5, 338);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(431, 17);
            this.hScrollBar1.TabIndex = 27;
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar1.Location = new System.Drawing.Point(436, 35);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 303);
            this.vScrollBar1.TabIndex = 26;
            // 
            // viewerTilesetRiver
            // 
            this.viewerTilesetRiver.Location = new System.Drawing.Point(4, 35);
            this.viewerTilesetRiver.Name = "viewerTilesetRiver";
            this.viewerTilesetRiver.Size = new System.Drawing.Size(449, 320);
            this.viewerTilesetRiver.TabIndex = 29;
            this.viewerTilesetRiver.Text = "tilesetViewerControl1";
            // 
            // tabTilesetShoal
            // 
            this.tabTilesetShoal.Controls.Add(this.button2);
            this.tabTilesetShoal.Controls.Add(this.hScrollBar2);
            this.tabTilesetShoal.Controls.Add(this.vScrollBar2);
            this.tabTilesetShoal.Controls.Add(this.tilesetViewerControl1);
            this.tabTilesetShoal.Location = new System.Drawing.Point(4, 22);
            this.tabTilesetShoal.Name = "tabTilesetShoal";
            this.tabTilesetShoal.Padding = new System.Windows.Forms.Padding(3);
            this.tabTilesetShoal.Size = new System.Drawing.Size(457, 361);
            this.tabTilesetShoal.TabIndex = 2;
            this.tabTilesetShoal.Text = "Shoal";
            this.tabTilesetShoal.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(5, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(448, 23);
            this.button2.TabIndex = 28;
            this.button2.Text = "Select tileset";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // hScrollBar2
            // 
            this.hScrollBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar2.Location = new System.Drawing.Point(5, 338);
            this.hScrollBar2.Name = "hScrollBar2";
            this.hScrollBar2.Size = new System.Drawing.Size(431, 17);
            this.hScrollBar2.TabIndex = 27;
            // 
            // vScrollBar2
            // 
            this.vScrollBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar2.Location = new System.Drawing.Point(436, 35);
            this.vScrollBar2.Name = "vScrollBar2";
            this.vScrollBar2.Size = new System.Drawing.Size(17, 303);
            this.vScrollBar2.TabIndex = 26;
            // 
            // tilesetViewerControl1
            // 
            this.tilesetViewerControl1.Location = new System.Drawing.Point(4, 35);
            this.tilesetViewerControl1.Name = "tilesetViewerControl1";
            this.tilesetViewerControl1.Size = new System.Drawing.Size(449, 320);
            this.tilesetViewerControl1.TabIndex = 29;
            this.tilesetViewerControl1.Text = "tilesetViewerControl1";
            // 
            // tabTilesetWaterfall
            // 
            this.tabTilesetWaterfall.Controls.Add(this.button3);
            this.tabTilesetWaterfall.Controls.Add(this.hScrollBar3);
            this.tabTilesetWaterfall.Controls.Add(this.vScrollBar3);
            this.tabTilesetWaterfall.Controls.Add(this.tilesetViewerControl2);
            this.tabTilesetWaterfall.Location = new System.Drawing.Point(4, 22);
            this.tabTilesetWaterfall.Name = "tabTilesetWaterfall";
            this.tabTilesetWaterfall.Padding = new System.Windows.Forms.Padding(3);
            this.tabTilesetWaterfall.Size = new System.Drawing.Size(457, 361);
            this.tabTilesetWaterfall.TabIndex = 3;
            this.tabTilesetWaterfall.Text = "Waterfall";
            this.tabTilesetWaterfall.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(5, 6);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(448, 23);
            this.button3.TabIndex = 28;
            this.button3.Text = "Select tileset";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // hScrollBar3
            // 
            this.hScrollBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBar3.Location = new System.Drawing.Point(5, 338);
            this.hScrollBar3.Name = "hScrollBar3";
            this.hScrollBar3.Size = new System.Drawing.Size(431, 17);
            this.hScrollBar3.TabIndex = 27;
            // 
            // vScrollBar3
            // 
            this.vScrollBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScrollBar3.Location = new System.Drawing.Point(436, 35);
            this.vScrollBar3.Name = "vScrollBar3";
            this.vScrollBar3.Size = new System.Drawing.Size(17, 303);
            this.vScrollBar3.TabIndex = 26;
            // 
            // tilesetViewerControl2
            // 
            this.tilesetViewerControl2.Location = new System.Drawing.Point(4, 35);
            this.tilesetViewerControl2.Name = "tilesetViewerControl2";
            this.tilesetViewerControl2.Size = new System.Drawing.Size(449, 320);
            this.tilesetViewerControl2.TabIndex = 29;
            this.tilesetViewerControl2.Text = "tilesetViewerControl1";
            // 
            // ProjectEternityAutotileTilesetPresetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 531);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.gbTileInformation);
            this.Controls.Add(this.gbTileset);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityAutotileTilesetPresetEditor";
            this.Text = "Tile Attributes";
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.gbTileset.ResumeLayout(false);
            this.gbTileInformation.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabTilesetMain.ResumeLayout(false);
            this.tabTilesetMain.PerformLayout();
            this.tabTilesetRiver.ResumeLayout(false);
            this.tabTilesetShoal.ResumeLayout(false);
            this.tabTilesetWaterfall.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbTileset;
        private System.Windows.Forms.Button btnAddTile;
        private System.Windows.Forms.Label lblActiveTileset;
        private TextBox txtTilesetName;
        private Label label1;
        private ComboBox cbTilesetType;
        private HScrollBar sclTileWidth;
        private VScrollBar sclTileHeight;
        private GroupBox gbTileInformation;
        private Button btnDeleteBattleAnimationBackground;
        private Button btnNewBattleAnimationBackground;
        private NumericUpDown txtBonusValue;
        private Label lblTerrainType;
        private ComboBox cboTerrainType;
        private Label lblTerrainBonusActivation;
        private ComboBox cboTerrainBonusActivation;
        private Button btnClearBonuses;
        private Button btnRemoveBonus;
        private Button btnAddNewBonus;
        private ListBox lstTerrainBonus;
        private Label lblBonusValue;
        private ComboBox cboTerrainBonusType;
        private Label lblTerrainBonusType;
        private ToolStripMenuItem tsmImportTileset;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private GameScreens.BattleMapScreen.TilesetViewerControl viewerTilesetMain;
        private Label lblBattleAnimationForeground;
        private ComboBox cboBattleAnimationForeground;
        private Label lblBattleAnimationBackground;
        private ComboBox cboBattleAnimationBackground;
        private Button btnEditTerrainTypes;
        private TabControl tabControl1;
        private TabPage tabTilesetMain;
        private TabPage tabTilesetRiver;
        private Button button1;
        private HScrollBar hScrollBar1;
        private VScrollBar vScrollBar1;
        private GameScreens.BattleMapScreen.TilesetViewerControl viewerTilesetRiver;
        private TabPage tabTilesetShoal;
        private Button button2;
        private HScrollBar hScrollBar2;
        private VScrollBar vScrollBar2;
        private GameScreens.BattleMapScreen.TilesetViewerControl tilesetViewerControl1;
        private TabPage tabTilesetWaterfall;
        private Button button3;
        private HScrollBar hScrollBar3;
        private VScrollBar vScrollBar3;
        private GameScreens.BattleMapScreen.TilesetViewerControl tilesetViewerControl2;
    }
}