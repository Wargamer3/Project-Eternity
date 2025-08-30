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
using static ProjectEternity.GameScreens.BattleMapScreen.TilesetPreset;

namespace ProjectEternity.Editors.TilesetEditor
{
    public partial class ProjectEternityTilesetPresetEditor : BaseEditor
    {
        public interface ITilesetPresetHelper
        {
            string[] GetTerrainTypes();
            void OnTerrainSelected(Terrain SelectedTerrain);
            void EditTerrainTypes();
            TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index);
            TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex);
            string GetEditorPath();
        }

        public class DeathmatchTilesetPresetHelper : ITilesetPresetHelper
        {
            public DeathmatchTilesetPresetHelper()
            {
            }

            public void EditTerrainTypes()
            {
                throw new NotImplementedException();
            }

            public string[] GetTerrainTypes()
            {
                return new string[]
                {
                    "Land",
                    "Sea",
                    "Air",
                    "Space",
                };
            }

            public TilesetPreset LoadPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index)
            {
                return new TilesetPreset(BR, TileSizeX, TileSizeY, 0);
            }

            public TilesetPresetInformation CreatePreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new TilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            }

            public void OnTerrainSelected(Terrain SelectedTerrain)
            {
                throw new NotImplementedException();
            }

            public string GetEditorPath()
            {
                return GUIRootPathMapTilesetImages;
            }
        }

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

        public ProjectEternityTilesetPresetEditor()
        {
            InitializeComponent();

            TilesetName = "";
            TileSize = new Point(32, 32);
            ArrayTerrain = new Terrain[0, 0];
            ListBattleBackgroundAnimationPath = new List<string>();
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

            BW.Write(TilesetName);
            BW.Write((byte)TilesetPreset.TilesetTypes.Regular);

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
            string Name = Path.Substring(0, Path.Length - 4).Substring(30);

            this.Text = Name + " - Project Eternity Tileset Preset Editor";

            InitHelper();

            FileStream FS = new FileStream("Content/Maps/Tilesets Presets/" + Name + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            TileSize.X = BR.ReadInt32();
            TileSize.Y = BR.ReadInt32();

            TilesetPreset NewTilesetPreset = Helper.LoadPreset(BR, TileSize.X, TileSize.Y, 0);

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


            TilesetName = NewTilesetPreset.ArrayTilesetInformation[0].TilesetName;

            ArrayTerrain = NewTilesetPreset.ArrayTilesetInformation[0].ArrayTerrain;
            ArrayTiles = NewTilesetPreset.ArrayTilesetInformation[0].ArrayTiles;

            viewerTileset.Preload();
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

            if (FinalX < 0 || FinalY < 0 || FinalX >= ArrayTerrain.GetLength(0) || FinalY >= ArrayTerrain.GetLength(1))
            {
                return;
            }
            //Set the ActiveTile to the mouse position.
            viewerTileset.SelectTile(TileOriginPoint, new Point(FinalX * TileSize.X, FinalY * TileSize.Y),
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

            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

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
            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

            Terrain PresetTerrain = ArrayTerrain[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];
            DrawableTile PresetTile = ArrayTiles[TilePos.X / TileSize.X, TilePos.Y / TileSize.Y];

            PresetTerrain.BattleBackgroundAnimationIndex = (byte)cboBattleAnimationBackground.SelectedIndex;
        }

        private void cboBattleAnimationForeground_SelectedIndexChanged(object sender, EventArgs e)
        {
            Point TilePos = viewerTileset.GetTileFromBrush(new Point(0, 0), BrushIndex);

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
                            TilesetName = TilePath.Substring(0, TilePath.Length - 4).Substring(22);
                            InitTileset(TilesetName);
                            int TilesetWidth = viewerTileset.sprTileset.Width / TileSize.X;
                            int TilesetHeight = viewerTileset.sprTileset.Height / TileSize.Y;
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
