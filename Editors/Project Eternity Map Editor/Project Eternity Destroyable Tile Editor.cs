using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Builder;
using static ProjectEternity.Editors.TilesetEditor.ProjectEternityTilesetPresetEditor;

namespace ProjectEternity.Editors.TilesetEditor
{
    public abstract partial class ProjectEternityDestroyableTileEditor : BaseEditor
    {
        public class TabContent
        {
            public TilesetPresetInformation TilesetInfo;

            public TabPage tabTileset;
            public Button btnSelectTileset;
            public TextBox txtTilesetName;
            public HScrollBar sclTileWidth;
            public VScrollBar sclTileHeight;
            public TilesetViewerControl viewerTilesetTab;
            public Label lblActiveTileset;

            public TabContent(string TabName, TilesetPresetInformation TilesetInfo)
            {
                this.TilesetInfo = TilesetInfo;

                tabTileset = new TabPage();
                btnSelectTileset = new Button();
                txtTilesetName = new TextBox();
                sclTileWidth = new HScrollBar();
                sclTileHeight = new VScrollBar();
                viewerTilesetTab = new TilesetViewerControl();
                lblActiveTileset = new Label();

                tabTileset.SuspendLayout();
                tabTileset.Controls.Add(btnSelectTileset);
                tabTileset.Controls.Add(sclTileWidth);
                tabTileset.Controls.Add(txtTilesetName);
                tabTileset.Controls.Add(sclTileHeight);
                tabTileset.Controls.Add(viewerTilesetTab);
                tabTileset.Controls.Add(lblActiveTileset);
                tabTileset.Location = new System.Drawing.Point(4, 22);
                tabTileset.Name = "tabTileset" + TabName;
                tabTileset.Padding = new System.Windows.Forms.Padding(3);
                tabTileset.Size = new System.Drawing.Size(457, 402);
                tabTileset.TabIndex = 1;
                tabTileset.Text = TabName;
                tabTileset.UseVisualStyleBackColor = true;
                // 
                // btnSelectTileset
                // 
                btnSelectTileset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                btnSelectTileset.Location = new System.Drawing.Point(6, 54);
                btnSelectTileset.Name = "btnSelectTileset" + TabName;
                btnSelectTileset.Size = new System.Drawing.Size(448, 23);
                btnSelectTileset.TabIndex = 29;
                btnSelectTileset.Text = "Select tileset";
                btnSelectTileset.UseVisualStyleBackColor = true;
                // 
                // txtTilesetName
                // 
                txtTilesetName.Location = new System.Drawing.Point(6, 21);
                txtTilesetName.Name = "textBox1" + TabName;
                txtTilesetName.ReadOnly = true;
                txtTilesetName.Size = new System.Drawing.Size(227, 20);
                txtTilesetName.TabIndex = 30;
                // 
                // sclTileWidth
                // 
                sclTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
                sclTileWidth.Location = new System.Drawing.Point(3, 381);
                sclTileWidth.Name = "sclTileWidth" + TabName;
                sclTileWidth.Size = new System.Drawing.Size(431, 17);
                sclTileWidth.TabIndex = 27;
                // 
                // sclTileHeight
                // 
                sclTileHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Right)));
                sclTileHeight.Location = new System.Drawing.Point(434, 78);
                sclTileHeight.Name = "vScrollBar1" + TabName;
                sclTileHeight.Size = new System.Drawing.Size(17, 303);
                sclTileHeight.TabIndex = 26;
                // 
                // viewerTilesetTab
                // 
                viewerTilesetTab.Location = new System.Drawing.Point(5, 78);
                viewerTilesetTab.Name = "tilesetViewerControl1" + TabName;
                viewerTilesetTab.Size = new System.Drawing.Size(426, 300);
                viewerTilesetTab.TabIndex = 31;
                viewerTilesetTab.Text = "tilesetViewerControl";
                // 
                // label2
                // 
                lblActiveTileset.AutoSize = true;
                lblActiveTileset.Location = new System.Drawing.Point(6, 5);
                lblActiveTileset.Name = "lblActiveTileset" + TabName;
                lblActiveTileset.Size = new System.Drawing.Size(67, 13);
                lblActiveTileset.TabIndex = 28;
                lblActiveTileset.Text = "Active tileset";

                tabTileset.ResumeLayout(false);
                tabTileset.PerformLayout();
            }
        }

        protected ITilesetPresetHelper Helper;

        protected List<string> ListBattleBackgroundAnimationPath;

        public Point TileSize;
        protected Point TileOriginPoint;
        protected List<TabContent> ListActiveTab;
        protected TabContent ActiveTab => ListActiveTab[tabControl1.SelectedIndex];
        int BrushIndex = 0;
        bool AllowEvent;

        private enum ItemSelectionChoices { Tile, BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProjectEternityDestroyableTileEditor()
        {
            InitializeComponent();

            TileSize = new Point(32, 32);
            ListBattleBackgroundAnimationPath = new List<string>();
            ListActiveTab = new List<TabContent>();
            AllowEvent = true;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            byte TilesetTypeIndex = (byte)cboTilesetType.SelectedIndex;
            if (TilesetTypeIndex < 0 || TilesetTypeIndex > cboTilesetType.Items.Count)
            {
                TilesetTypeIndex = 0;
            }

            BW.Write(TilesetTypeIndex);
            BW.Write((byte)ListActiveTab.Count);

            foreach (TabContent ActiveTab in ListActiveTab)
            {
                ActiveTab.TilesetInfo.Write(BW);
            }

            BW.Write(ListBattleBackgroundAnimationPath.Count);
            foreach (string BattleBackgroundAnimationPath in ListBattleBackgroundAnimationPath)
            {
                BW.Write(BattleBackgroundAnimationPath);
            }

            BW.Flush();
            BW.Close();
            FS.Close();
        }

        protected void LoadTileset(string Path)
        {
            string Name = Path.Substring(0, Path.Length - 5).Substring(8);

            this.Text = Name + " - Project Eternity Destroyable Tiles Preset Editor";

            InitHelper();
            CreateTab("Main");

            FileStream FS = new FileStream("Content/" + Name + ".pedt", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            DestructibleTilesetPreset NewTilesetPreset = Helper.LoadDestructiblePreset(BR, TileSize.X, TileSize.Y, 0);

            BR.Close();
            FS.Close();

            cboBattleAnimationBackground.Items.Clear();
            cboBattleAnimationBackground.Items.Add("None");
            cboBattleAnimationForeground.Items.Clear();
            cboBattleAnimationForeground.Items.Add("None");
            ListBattleBackgroundAnimationPath.AddRange(NewTilesetPreset.ListBattleBackgroundAnimationPath);
            foreach (string BattleBackgroundAnimationPath in NewTilesetPreset.ListBattleBackgroundAnimationPath)
            {
                cboBattleAnimationBackground.Items.Add(BattleBackgroundAnimationPath);
                cboBattleAnimationForeground.Items.Add(BattleBackgroundAnimationPath);
            }

            cboTerrainType.Items.Clear();
            string[] ArrayTerrainType = Helper.GetTerrainTypes();
            foreach (var ActiveTerrainType in ArrayTerrainType)
            {
                cboTerrainType.Items.Add(ActiveTerrainType);
            }

            tabControl1.TabPages.Clear();
            ListActiveTab.Clear();

            for (int i = 0; i < NewTilesetPreset.ArrayTilesetInformation.Length; i++)
            {
                TilesetPresetInformation ActiveTileset = NewTilesetPreset.ArrayTilesetInformation[i];
                CreateTab(i.ToString());

                TabContent ActiveTab = ListActiveTab[i];
                ActiveTab.TilesetInfo.TilesetName = ActiveTileset.TilesetName;

                ActiveTab.TilesetInfo.ArrayTerrain = ActiveTileset.ArrayTerrain;
                ActiveTab.TilesetInfo.ArrayTiles = ActiveTileset.ArrayTiles;

                ActiveTab.viewerTilesetTab.Preload();
                if (!string.IsNullOrWhiteSpace(ActiveTab.TilesetInfo.TilesetName))
                    InitTileset(ActiveTab.TilesetInfo.TilesetName, ActiveTab, false);
            }

            AllowEvent = false;

            cboTilesetType.SelectedIndex = (int)NewTilesetPreset.TilesetType;

            if (NewTilesetPreset.TilesetType == DestructibleTilesAttackAttributes.DestructibleTypes.Ocean)
            {
                tabControl1.TabPages[0].Text = "Sea";
                tabControl1.TabPages[1].Text = "River";
                tabControl1.TabPages[2].Text = "Shoal";
                tabControl1.TabPages[3].Text = "Waterfall";
            }
            else
            {
                if (tabControl1.TabPages.Count == 0)
                {
                    CreateTab("Main");
                }
                else
                {
                    tabControl1.TabPages[0].Text = "Main";
                }
            }

            AllowEvent = true;

            SelectTile(0, 0);
        }

        protected abstract void InitHelper();

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void CreateTab(string TabName)
        {
            TabContent NewTab = new TabContent(TabName, Helper.CreateDestructiblePreset(string.Empty, 0, 0, 32, 32, 0));
            ListActiveTab.Add(NewTab);
            this.tabControl1.Controls.Add(NewTab.tabTileset);

            NewTab.btnSelectTileset.Click += new System.EventHandler(this.btnAddTile_Click);
            NewTab.sclTileWidth.Scroll += new ScrollEventHandler(this.sclTileWidth_Scroll);
            NewTab.sclTileHeight.Scroll += new ScrollEventHandler(this.sclTileHeight_Scroll);

            NewTab.viewerTilesetTab.Click += new EventHandler(this.viewerTileset_Click);
            NewTab.viewerTilesetTab.MouseDown += new MouseEventHandler(this.viewerTileset_MouseDown);
            NewTab.viewerTilesetTab.MouseMove += new MouseEventHandler(this.viewerTileset_MouseMove);
        }

        private void cbTilesetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvent)
            {
                return;
            }
            tabControl1.TabPages.Clear();
            ListActiveTab.Clear();

            if (cboTilesetType.SelectedIndex == (int)TilesetPreset.TilesetTypes.Ocean)
            {
                CreateTab("Sea");
                CreateTab("River");
                CreateTab("Shoal");
                CreateTab("Waterfall");
            }
            else
            {
                CreateTab("Main");
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex >= 0)
            {
                SelectTile(ActiveTab.viewerTilesetTab.ListTileBrush[0].X, ActiveTab.viewerTilesetTab.ListTileBrush[0].Y);
            }
        }

        #region Tileset

        public virtual void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(Helper.GetEditorPath()));
        }

        private void InitTileset(string TilesetName, TabContent ActiveTab, bool NewTileset)
        {
            //Add the file name to the tile combo box.
            ActiveTab.txtTilesetName.Text = TilesetName;

            Texture2D Tileset = ActiveTab.viewerTilesetTab.content.Load<Texture2D>("Assets/Destroyable Tiles/" + TilesetName);

            if (Tileset.Width >= ActiveTab.viewerTilesetTab.Width)
            {
                ActiveTab.sclTileWidth.Maximum = Tileset.Width - ActiveTab.viewerTilesetTab.Width;
                ActiveTab.sclTileWidth.Visible = true;
            }
            else
                ActiveTab.sclTileWidth.Visible = false;
            if (Tileset.Height >= ActiveTab.viewerTilesetTab.Height)
            {
                ActiveTab.sclTileHeight.Maximum = Tileset.Height - ActiveTab.viewerTilesetTab.Height;
                ActiveTab.sclTileHeight.Visible = true;
            }
            else
                ActiveTab.sclTileHeight.Visible = false;

            ActiveTab.viewerTilesetTab.InitTileset(Tileset, TileSize);

            ActiveTab.TilesetInfo.TilesetName = TilesetName;

            if (NewTileset)
            {
                int TilesetWidth = ActiveTab.viewerTilesetTab.sprTileset.Width / TileSize.X;
                int TilesetHeight = ActiveTab.viewerTilesetTab.sprTileset.Height / TileSize.Y;
                ActiveTab.TilesetInfo.ArrayTerrain = new Terrain[TilesetWidth, TilesetHeight];
                ActiveTab.TilesetInfo.ArrayTiles = new DrawableTile[TilesetWidth, TilesetHeight];
                for (int X = TilesetWidth - 1; X >= 0; --X)
                {
                    for (int Y = TilesetHeight - 1; Y >= 0; --Y)
                    {
                        ActiveTab.TilesetInfo.ArrayTerrain[X, Y] = ActiveTab.TilesetInfo.CreateTerrain(X, Y, TileSize.X, TileSize.Y);
                        ActiveTab.TilesetInfo.ArrayTerrain[X, Y].TerrainTypeIndex = 1;
                        ActiveTab.TilesetInfo.ArrayTiles[X, Y] = new DrawableTile(new Rectangle(0, 0, TileSize.X, TileSize.Y), 0);
                    }
                }
            }
        }

        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = ActiveTab.viewerTilesetTab.DrawOffset;
            DrawOffset.X = e.NewValue;
            ActiveTab.viewerTilesetTab.DrawOffset = DrawOffset;
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = ActiveTab.viewerTilesetTab.DrawOffset;
            DrawOffset.Y = e.NewValue;
            ActiveTab.viewerTilesetTab.DrawOffset = DrawOffset;
        }

        private void tsmImportTileset_Click(object sender, EventArgs e)
        {
            var SpriteFileDialog = new OpenFileDialog()
            {
                FileName = "Select a sprite to import",
                Filter = "Sprite files (*.png)|*.png",
                Title = "Open sprite file"
            };

            if (SpriteFileDialog.ShowDialog() == DialogResult.OK)
            {
                var NewSpriteFilePath = SpriteFileDialog.FileName;
                var fileName = SpriteFileDialog.SafeFileName;
                var Builder = new ContentBuilder();
                Builder.Add(NewSpriteFilePath, fileName.Substring(0, fileName.Length - 4), "TextureImporter", "TextureProcessor");
                string buildError = Builder.Build();

                string NewSpriteFileName = Path.GetFileNameWithoutExtension(NewSpriteFilePath);
                string SpriteFolder = "Content/Assets/Destroyable Tiles";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(42);

                if (!string.IsNullOrEmpty(NewSpriteFileFolder))
                {
                    NewSpriteFileFolder += "/";
                }

                Builder.CopyBuildOutput(NewSpriteFileName, NewSpriteFileName, SpriteFolder + NewSpriteFileFolder);

                InitTileset(NewSpriteFileFolder + NewSpriteFileName, ActiveTab, true);
            }
        }

        private void viewerTileset_Click(object sender, EventArgs e)
        {
            SelectTile(((MouseEventArgs)e).X, ((MouseEventArgs)e).Y);
        }

        private void viewerTileset_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SelectTile(e.X, e.Y);
            }
        }

        private void viewerTileset_MouseMove(object sender, MouseEventArgs e)
        {//If there is a map loaded(and so TileSize.X is not 0).
            if (e.Button == MouseButtons.Left)
            {
                SelectTile(e.X, e.Y);
            }
        }

        private void SelectTile(int X, int Y)
        {
            Point DrawOffset = ActiveTab.viewerTilesetTab.DrawOffset;//Used to avoid warnings.
            int FinalX = (X + DrawOffset.X) / TileSize.X;
            int FinalY = ((Y + DrawOffset.X)) / TileSize.Y;

            if (FinalX < 0 || FinalY < 0 || FinalX >= ActiveTab.TilesetInfo.ArrayTerrain.GetLength(0) || FinalY >= ActiveTab.TilesetInfo.ArrayTerrain.GetLength(1))
            {
                return;
            }
            //Set the ActiveTile to the mouse position.
            ActiveTab.viewerTilesetTab.SelectTile(TileOriginPoint, new Point(FinalX * TileSize.X, FinalY * TileSize.Y),
                                                 Control.ModifierKeys == Keys.Shift, BrushIndex);

            Terrain PresetTerrain = ActiveTab.TilesetInfo.ArrayTerrain[FinalX, FinalY];
            DrawableTile PresetTile = ActiveTab.TilesetInfo.ArrayTiles[FinalX, FinalY];

            cboTerrainType.SelectedIndex = PresetTerrain.TerrainTypeIndex;

            if (PresetTerrain.BattleBackgroundAnimationIndex >= 0 && PresetTerrain.BattleBackgroundAnimationIndex < cboBattleAnimationBackground.Items.Count)
            {
                cboBattleAnimationBackground.SelectedIndex = PresetTerrain.BattleBackgroundAnimationIndex;
            }
            else
            {
                cboBattleAnimationBackground.SelectedIndex = 0;
            }

            if (PresetTerrain.BattleForegroundAnimationIndex >= 0 && PresetTerrain.BattleForegroundAnimationIndex < cboBattleAnimationForeground.Items.Count)
            {
                cboBattleAnimationForeground.SelectedIndex = PresetTerrain.BattleForegroundAnimationIndex;
            }
            else
            {
                cboBattleAnimationForeground.SelectedIndex = 0;
            }
        }

        #endregion

        private void cboTerrainType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTerrainType.SelectedIndex < 0)
            {
                return;
            }

            Point TilePos = ActiveTab.viewerTilesetTab.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ActiveTab.TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ActiveTab.TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;

            ActiveTab.TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y] = PresetTile;
        }

        private void btnEditTerrainTypes_Click(object sender, EventArgs e)
        {
            Helper.EditTerrainTypes();
            cboTerrainType.Items.Clear();
            string[] ArrayTerrainType = Helper.GetTerrainTypes();
            foreach (var ActiveTerrainType in ArrayTerrainType)
            {
                cboTerrainType.Items.Add(ActiveTerrainType);
            }

            SelectTile(0, 0);
        }

        #region Backgrounds

        private void btnNewBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BattleBackgroundAnimation;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAnimationsBackgroundsAll));
        }

        private void btnDeleteBattleAnimationBackground_Click(object sender, EventArgs e)
        {
            if (cboBattleAnimationBackground.SelectedIndex >= 0)
            {
                cboBattleAnimationBackground.Items.RemoveAt(cboBattleAnimationBackground.SelectedIndex);
            }
            if (cboBattleAnimationForeground.SelectedIndex >= 0)
            {
                cboBattleAnimationForeground.Items.RemoveAt(cboBattleAnimationForeground.SelectedIndex);
            }
        }

        private void cboBattleAnimationBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point TilePos = ActiveTab.viewerTilesetTab.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ActiveTab.TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ActiveTab.TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;
        }

        private void cboBattleAnimationForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point TilePos = ActiveTab.viewerTilesetTab.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ActiveTab.TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ActiveTab.TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.BattleForegroundAnimationIndex = (byte)cboBattleAnimationForeground.SelectedIndex;
        }

        #endregion

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Tile:
                        string TilePath = Items[I];
                        if (TilePath != null)
                        {
                            InitTileset(TilePath.Substring(0, TilePath.Length - 4).Substring(23), ActiveTab, true);
                        }
                        break;

                    case ItemSelectionChoices.BattleBackgroundAnimation:
                        string BackgroundPath = Items[I];
                        if (BackgroundPath != null)
                        {
                            BackgroundPath = BackgroundPath.Substring(0, BackgroundPath.Length - 5).Substring(19);

                            ListBattleBackgroundAnimationPath.Add(BackgroundPath);
                            cboBattleAnimationBackground.Items.Add(BackgroundPath);
                            cboBattleAnimationForeground.Items.Add(BackgroundPath);
                        }
                        break;
                }
            }
        }
    }
}
