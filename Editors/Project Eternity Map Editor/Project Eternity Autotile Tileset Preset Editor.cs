using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Builder;
using static ProjectEternity.Editors.TilesetEditor.ProjectEternityTilesetPresetEditor;

namespace ProjectEternity.Editors.TilesetEditor
{
    public partial class ProjectEternityAutotileTilesetPresetEditor : BaseEditor
    {
        protected ITilesetPresetHelper Helper;

        protected string TilesetName;
        protected Point TileSize;
        protected Terrain[,] ArrayTerrain;
        protected DrawableTile[,] ArrayTiles;
        protected List<string> ListBattleBackgroundAnimationPath;

        protected Point TileOriginPoint;
        int BrushIndex = 0;

        private enum ItemSelectionChoices { Tile, BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProjectEternityAutotileTilesetPresetEditor()
        {
            InitializeComponent();

            TilesetName = "";
            TileSize = new Point(32, 32);
            ArrayTerrain = new Terrain[0, 0];
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public ProjectEternityAutotileTilesetPresetEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadTileset(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathMapAutotilesImages, GUIRootPathMapAutotiles }, "Maps/Autotiles/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathMapAutotilesPresetsDeathmatch, GUIRootPathMapAutotilesPresets, GUIRootPathMapAutotiles }, "Maps/Autotiles Presets/Deathmatch/", new string[] { ".peat" }, typeof(ProjectEternityAutotileTilesetPresetEditor), true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            BW.Write(TilesetName);
            byte TilesetTypeIndex = (byte)cbTilesetType.SelectedIndex;
            if (TilesetTypeIndex < 0 || TilesetTypeIndex > cbTilesetType.Items.Count)
            {
                TilesetTypeIndex = 0;
            }
            BW.Write(TilesetTypeIndex);

            BW.Write(ArrayTerrain.GetLength(0));
            BW.Write(ArrayTerrain.GetLength(1));

            //Tiles
            for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
            {
                for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                {
                    ArrayTerrain[X, Y].Save(BW);
                }
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
            string Name = Path.Substring(0, Path.Length - 5).Substring(31);

            this.Text = Name + " - Project Eternity Autotile Tileset Preset Editor";

            InitHelper();

            FileStream FS = new FileStream("Content/Maps/Autotiles Presets/" + Name + ".peat", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            Terrain.TilesetPreset NewTilesetPreset = Helper.LoadPreset(BR, TileSize.X, TileSize.Y, 0);

            BR.Close();
            FS.Close();

            cboTerrainType.Items.Clear();
            string[] ArrayTerrainType = Helper.GetTerrainTypes();
            foreach (var ActiveTerrainType in ArrayTerrainType)
            {
                cboTerrainType.Items.Add(ActiveTerrainType);
            }

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


            TilesetName = NewTilesetPreset.TilesetName;
            cbTilesetType.SelectedIndex = (int)NewTilesetPreset.TilesetType;

            ArrayTerrain = NewTilesetPreset.ArrayTerrain;
            ArrayTiles = NewTilesetPreset.ArrayTiles;

            viewerTilesetMain.Preload();
            if (!string.IsNullOrWhiteSpace(TilesetName))
                InitTileset(TilesetName);

            SelectTile(0, 0);
        }

        protected virtual void InitHelper()
        {
            Helper = new DeathmatchTilesetPresetHelper();
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void cbTilesetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            tabControl1.TabPages.Add(tabTilesetMain);
            tabTilesetMain.Text = "Main";

            if (cbTilesetType.SelectedIndex == (int)Terrain.TilesetPreset.TilesetTypes.Ocean)
            {
                tabControl1.TabPages.Add(tabTilesetRiver);
                tabControl1.TabPages.Add(tabTilesetShoal);
                tabControl1.TabPages.Add(tabTilesetWaterfall);
                tabTilesetMain.Text = "Sea";
                tabTilesetRiver.Text = "River";
                tabTilesetShoal.Text = "Shoal";
                tabTilesetWaterfall.Text = "Waterfall";
            }
        }

        #region Tileset

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapAutotilesImages));
        }

        private void InitTileset(string TilesetName)
        {
            //Add the file name to the tile combo box.
            txtTilesetName.Text = TilesetName;

            Texture2D Tileset = viewerTilesetMain.content.Load<Texture2D>("Maps/Autotiles/" + TilesetName);

            if (Tileset.Width >= viewerTilesetMain.Width)
            {
                sclTileWidth.Maximum = Tileset.Width - viewerTilesetMain.Width;
                sclTileWidth.Visible = true;
            }
            else
                sclTileWidth.Visible = false;
            if (Tileset.Height >= viewerTilesetMain.Height)
            {
                sclTileHeight.Maximum = Tileset.Height - viewerTilesetMain.Height;
                sclTileHeight.Visible = true;
            }
            else
                sclTileHeight.Visible = false;

            viewerTilesetMain.InitTileset(Tileset, TileSize);
        }

        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = viewerTilesetMain.DrawOffset;
            DrawOffset.X = e.NewValue;
            viewerTilesetMain.DrawOffset = DrawOffset;
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = viewerTilesetMain.DrawOffset;
            DrawOffset.Y = e.NewValue;
            viewerTilesetMain.DrawOffset = DrawOffset;
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
                var filePath = SpriteFileDialog.FileName;
                var fileName = SpriteFileDialog.SafeFileName;
                var Builder = new ContentBuilder();
                Builder.Add(filePath, fileName.Substring(0, fileName.Length - 4), "TextureImporter", "TextureProcessor");
                string buildError = Builder.Build();

                string NewSpriteFileName = Path.GetFileNameWithoutExtension(FilePath);
                string SpriteFolder = "Content\\Maps\\Tileset Presets";
                string NewSpriteFileFolder = Path.GetDirectoryName(FilePath).Substring(28);
                Builder.CopyBuildOutput(fileName, NewSpriteFileName, SpriteFolder + "\\" + NewSpriteFileFolder);

                InitTileset(NewSpriteFileFolder + " \\" + NewSpriteFileName);
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
            Point DrawOffset = viewerTilesetMain.DrawOffset;//Used to avoid warnings.
            int FinalX = (X + DrawOffset.X) / TileSize.X;
            int FinalY = ((Y + DrawOffset.X)) / TileSize.Y;

            if (FinalX < 0 || FinalY < 0 || FinalX >= ArrayTerrain.GetLength(0) || FinalY >= ArrayTerrain.GetLength(1))
            {
                return;
            }
            //Set the ActiveTile to the mouse position.
            viewerTilesetMain.SelectTile(TileOriginPoint, new Point(FinalX * TileSize.X, FinalY * TileSize.Y),
                                                 Control.ModifierKeys == Keys.Shift, BrushIndex);

            Terrain PresetTerrain = ArrayTerrain[FinalX, FinalY];
            DrawableTile PresetTile = ArrayTiles[FinalX, FinalY];

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

            Point TilePos = viewerTilesetMain.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;

            ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y] = PresetTile;
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
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAnimationsBackgroundsAll));
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
            Point TilePos = viewerTilesetMain.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;
        }

        private void cboBattleAnimationForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point TilePos = viewerTilesetMain.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

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
                            TilesetName = TilePath.Substring(0, TilePath.Length - 4).Substring(23);
                            InitTileset(TilesetName);
                            int TilesetWidth = viewerTilesetMain.sprTileset.Width / TileSize.X;
                            int TilesetHeight = viewerTilesetMain.sprTileset.Height / TileSize.Y;
                            ArrayTerrain = new Terrain[TilesetWidth, TilesetHeight];
                            ArrayTiles = new DrawableTile[TilesetWidth, TilesetHeight];
                            for (int X = TilesetWidth - 1; X >= 0; --X)
                            {
                                for (int Y = TilesetHeight - 1; Y >= 0; --Y)
                                {
                                    ArrayTerrain[X, Y] = new Terrain(X, Y, TileSize.X, TileSize.Y, 0, 0, 0);
                                    ArrayTerrain[X, Y].TerrainTypeIndex = 1;
                                    ArrayTiles[X, Y] = new DrawableTile(new Rectangle(0, 0, TileSize.X, TileSize.Y), 0);
                                }
                            }
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
