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

namespace ProjectEternity.Editors.TilesetEditor
{
    public abstract partial class ProjectEternityTilesetPresetEditor : BaseEditor
    {
        public interface ITilesetPresetHelper
        {
            string[] GetTerrainTypes();
            void OnTerrainSelected(Terrain SelectedTerrain);
            void EditTerrainTypes();
            TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index);
            TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex);
            DestructibleTilesetPreset LoadDestructiblePreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index);
            TilesetPresetInformation CreateDestructiblePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex);
            string GetEditorPath();
        }

        protected ITilesetPresetHelper Helper;

        protected List<string> ListBattleBackgroundAnimationPath;

        public Point TileSize;
        protected Point TileOriginPoint;
        protected TilesetPreset Preset;
        protected TilesetPresetInformation TilesetInfo;
        protected int BrushIndex = 0;
        bool AllowEvent;

        private enum ItemSelectionChoices { Tile, BattleBackgroundAnimation };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProjectEternityTilesetPresetEditor()
        {
            InitializeComponent();

            AllowEvent = true;
            TileSize = new Point(32, 32);
            ListBattleBackgroundAnimationPath = new List<string>();
            InitHelper();
            TilesetInfo = Helper.CreatePreset(string.Empty, 32, 32, 32, 32, 0);
        }

        public ProjectEternityTilesetPresetEditor(string FilePath, object[] Params)
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
                new EditorInfo(new string[] { GUIRootPathMapTilesetImages, GUIRootPathMapTilesets }, "Maps/Tilesets/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathMapTilesetPresetsDeathmatch, GUIRootPathMapTilesetPresets, GUIRootPathMapTilesets }, "Maps/Tilesets Presets/Deathmatch/", new string[] { ".pet" }, typeof(ProjectEternityTilesetPresetEditor), true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.Unicode);

            BW.Write(TileSize.X);
            BW.Write(TileSize.Y);

            BW.Write((byte)0);
            BW.Write((byte)1);

            TilesetInfo.Write(BW);

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
            string Name = Path.Substring(0, Path.Length - 4).Substring(8);

            this.Text = Name + " - Project Eternity Tileset Preset Editor";

            InitHelper();

            FileStream FS = new FileStream("Content/" + Name + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            Preset = Helper.LoadPreset(BR, TileSize.X, TileSize.Y, 0);
            TilesetInfo = Preset.ArrayTilesetInformation[0];

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
            ListBattleBackgroundAnimationPath.AddRange(Preset.ListBattleBackgroundAnimationPath);
            foreach (string BattleBackgroundAnimationPath in Preset.ListBattleBackgroundAnimationPath)
            {
                cboBattleAnimationBackground.Items.Add(BattleBackgroundAnimationPath);
                cboBattleAnimationForeground.Items.Add(BattleBackgroundAnimationPath);
            }

            AllowEvent = false;

            if (Preset.ArrayTilesetInformation.Length > 0)
            {
                viewerTileset.Preload();
                if (!string.IsNullOrWhiteSpace(TilesetInfo.TilesetName))
                    InitTileset(TilesetInfo.TilesetName);

                SelectTile(0, 0);
            }

            AllowEvent = true;
        }

        protected abstract void InitHelper();

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        #region Tileset

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(ShowContextMenuWithItem(Helper.GetEditorPath()));
        }

        private void InitTileset(string TilesetName)
        {
            //Add the file name to the tile combo box.
            txtTilesetName.Text = TilesetName;

            Texture2D Tileset = viewerTileset.content.Load<Texture2D>("Maps/Tilesets/" + TilesetName);

            if (Tileset.Width >= viewerTileset.Width)
            {
                sclTileWidth.Maximum = Tileset.Width - viewerTileset.Width;
                sclTileWidth.Visible = true;
            }
            else
                sclTileWidth.Visible = false;
            if (Tileset.Height >= viewerTileset.Height)
            {
                sclTileHeight.Maximum = Tileset.Height - viewerTileset.Height;
                sclTileHeight.Visible = true;
            }
            else
                sclTileHeight.Visible = false;

            viewerTileset.InitTileset(Tileset, TileSize);
        }

        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = viewerTileset.DrawOffset;
            DrawOffset.X = e.NewValue;
            viewerTileset.DrawOffset = DrawOffset;
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = viewerTileset.DrawOffset;
            DrawOffset.Y = e.NewValue;
            viewerTileset.DrawOffset = DrawOffset;
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
            Point DrawOffset = viewerTileset.DrawOffset;//Used to avoid warnings.
            int FinalX = (X + DrawOffset.X) / TileSize.X;
            int FinalY = ((Y + DrawOffset.X)) / TileSize.Y;

            if (FinalX < 0 || FinalY < 0 || FinalX >= TilesetInfo.ArrayTerrain.GetLength(0) || FinalY >= TilesetInfo.ArrayTerrain.GetLength(1))
            {
                return;
            }
            //Set the ActiveTile to the mouse position.
            viewerTileset.SelectTile(TileOriginPoint, new Point(FinalX * TileSize.X, FinalY * TileSize.Y),
                                                 Control.ModifierKeys == Keys.Shift, BrushIndex);

            Terrain PresetTerrain = TilesetInfo.ArrayTerrain[FinalX, FinalY];
            DrawableTile PresetTile = TilesetInfo.ArrayTiles[FinalX, FinalY];

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
            if (cboTerrainType.SelectedIndex < 0 || !AllowEvent)
            {
                return;
            }

            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.TerrainTypeIndex = (byte)cboTerrainType.SelectedIndex;

            TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y] = PresetTile;
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
            if (!AllowEvent)
            {
                return;
            }

            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;
        }

        private void cboBattleAnimationForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvent)
            {
                return;
            }

            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = TilesetInfo.ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = TilesetInfo.ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

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
                            TilesetInfo.TilesetName = TilePath.Substring(0, TilePath.Length - 4).Substring(22);
                            InitTileset(TilesetInfo.TilesetName);
                            int TilesetWidth = viewerTileset.sprTileset.Width / TileSize.X;
                            int TilesetHeight = viewerTileset.sprTileset.Height / TileSize.Y;
                            TilesetInfo.ArrayTerrain = new Terrain[TilesetWidth, TilesetHeight];
                            TilesetInfo.ArrayTiles = new DrawableTile[TilesetWidth, TilesetHeight];
                            for (int X = TilesetWidth - 1; X >= 0; --X)
                            {
                                for (int Y = TilesetHeight - 1; Y >= 0; --Y)
                                {
                                    TilesetInfo.ArrayTerrain[X, Y] = new Terrain(X, Y, TileSize.X, TileSize.Y, 0, 0, 0);
                                    TilesetInfo.ArrayTerrain[X, Y].TerrainTypeIndex = 1;
                                    TilesetInfo.ArrayTiles[X, Y] = new DrawableTile(new Rectangle(0, 0, TileSize.X, TileSize.Y), 0);
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
