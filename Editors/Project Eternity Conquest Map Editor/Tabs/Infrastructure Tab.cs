using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Editors.ConquestMapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.MapEditor
{
    public class SceneryTab : IMapEditorTab
    {
        private enum ItemSelectionChoices { Tile, Autotile };

        private ItemSelectionChoices ItemSelectionChoice;

        private TabPage tabInfrastructure;
        private ListView lvInfrastructure;
        protected ComboBox cboTiles;
        private Button btnAddTile;
        private Button btnRemoveTile;
        private Button btnTileAttributes;
        private Button btn3DTileAttributes;

        protected ComboBox cboAutotiles;
        private Button btnAddAutotile;
        private Button btnRemoveAutotile;

        private ImageList imageList;

        private List<UnitConquest> ListFactionUnit;

        protected ITileAttributes TileAttributesEditor;

        public BattleMapViewerControl BattleMapViewer { get; set; }
        public IMapHelper Helper { get; set; }
        private ConquestMap ActiveMap;

        public TabPage InitTab(MenuStrip mnuToolBar)
        {
            ListFactionUnit = new List<UnitConquest>();

            ConquestSpawnUserControl SpawnControl = new ConquestSpawnUserControl();
            tabInfrastructure = SpawnControl.tabControl1.TabPages[2];

            this.lvInfrastructure = SpawnControl.lvIScenery;

            this.btnAddTile = SpawnControl.btnSceneryAddTileset;
            this.btnRemoveTile = SpawnControl.btnSceneryRemoveTileset;
            this.btnTileAttributes = SpawnControl.btnSceneryTileAttributes;
            this.btn3DTileAttributes = SpawnControl.btnScenery3DTileAttributes;
            this.cboTiles = SpawnControl.cboSceneryTilesets;

            this.cboAutotiles = SpawnControl.cboSceneryAutotile;
            this.btnAddAutotile = SpawnControl.btnSceneryAddAutotile;
            this.btnRemoveAutotile = SpawnControl.btnSceneryRemoveAutotile;

            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            this.btnRemoveTile.Click += new System.EventHandler(this.btnRemoveTile_Click);
            this.btnTileAttributes.Click += new System.EventHandler(this.btnTileAttributes_Click);
            this.btn3DTileAttributes.Click += new System.EventHandler(this.btn3DTileAttributes_Click);
            this.cboTiles.SelectedIndexChanged += new System.EventHandler(this.cboTiles_SelectedIndexChanged);

            this.btnAddAutotile.Click += new System.EventHandler(this.btnAddAutotile_Click);
            this.btnRemoveAutotile.Click += new System.EventHandler(this.btnRemoveAutotile_Click);

            imageList = new ImageList();
            imageList.ImageSize = new Size(32, 32);
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            lvInfrastructure.LargeImageList = imageList;

            lvInfrastructure.SelectedIndexChanged += lvUnits_SelectedIndexChanged;

            return tabInfrastructure;
        }

        public void OnMapLoaded()
        {
            ActiveMap = (ConquestMap)BattleMapViewer.ActiveMap;

            TileAttributesEditor = Helper.GetTileEditor();

            for (int T = 0; T < ActiveMap.ListTemporaryTilesetPreset.Count; T++)
            {
                TilesetPreset ActiveTilesetPreset = ActiveMap.ListTemporaryTilesetPreset[T];

                AddSceneryImage(ActiveTilesetPreset.ArrayTilesetInformation[0].TilesetName, ActiveMap.ListTemporaryTileSet[T]);
                if (ActiveTilesetPreset.TilesetType == TilesetPreset.TilesetTypes.Regular)
                {
                    cboTiles.Items.Add(ActiveTilesetPreset.ArrayTilesetInformation[0].TilesetName);
                }
                else
                {
                    if (string.IsNullOrEmpty(ActiveTilesetPreset.ArrayTilesetInformation[0].TilesetName))
                    {
                        cboTiles.Items.Add("Autotile Not Finished");
                    }
                    else
                    {
                        cboTiles.Items.Add("Autotile " + ActiveTilesetPreset.ArrayTilesetInformation[0].TilesetName);
                    }

                    if (ActiveTilesetPreset.TilesetType != TilesetPreset.TilesetTypes.Slave)
                    {
                        cboAutotiles.Items.Add(ActiveTilesetPreset.ArrayTilesetInformation[0].TilesetName);
                    }
                }
            }
        }

        public bool TabProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }

        public void TabOnMouseDown(MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        public void TabOnMouseUp(MouseEventArgs e)
        {
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            int GridX = (int)(ActiveMap.CursorPosition.X) / ActiveMap.TileSize.X;
            int GridY = (int)(ActiveMap.CursorPosition.Y) / ActiveMap.TileSize.Y;
            int TopLayerIndex = BattleMapViewer.GetRealTopLayerIndex(BattleMapViewer.SelectedListLayerIndex);

            if (e.Button == MouseButtons.Left)
            {
                TerrainConquest NewTerrain = new TerrainConquest((TerrainConquest)ActiveMap.ListTemporaryTilesetPreset[lvInfrastructure.SelectedIndices[0]].ArrayTilesetInformation[0].ArrayTerrain[0, 0], new Microsoft.Xna.Framework.Point(GridX, GridY), TopLayerIndex);
                NewTerrain.Owner = ActiveMap;
                NewTerrain.WorldPosition = new Microsoft.Xna.Framework.Vector3(GridX * ActiveMap.TileSize.X, GridY * ActiveMap.TileSize.Y, (TopLayerIndex + NewTerrain.Height) * ActiveMap.LayerHeight);

                DestructableTerrain NewTemporaryTerrain = new DestructableTerrain();
                NewTemporaryTerrain.ReplacementTerrain = NewTerrain;
                NewTemporaryTerrain.ReplacementTile = ActiveMap.ListTemporaryTilesetPreset[lvInfrastructure.SelectedIndices[0]].ArrayTilesetInformation[0].ArrayTiles[0, 0];
                NewTemporaryTerrain.RemainingHP = 10;

                var Position = new Microsoft.Xna.Framework.Vector3(GridX, GridY, TopLayerIndex);
                if (ActiveMap.DicTemporaryTerrain.ContainsKey(Position))
                {
                    ActiveMap.DicTemporaryTerrain[Position] = NewTemporaryTerrain;
                }
                else
                {
                    ActiveMap.DicTemporaryTerrain.Add(Position, NewTemporaryTerrain);
                }

                ActiveMap.Reset();
            }
            else if (e.Button == MouseButtons.Right)
            {
                var Position = new Microsoft.Xna.Framework.Vector3(GridX, GridY, TopLayerIndex);
                if (ActiveMap.DicTemporaryTerrain.ContainsKey(Position))
                {
                    ActiveMap.DicTemporaryTerrain.Remove(Position);
                }

                ActiveMap.Reset();
            }
        }

        #region Tileset

        private void cboTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesets));
        }

        private void btnRemoveTile_Click(object sender, EventArgs e)
        {//If there's a tile set selected.
            if (cboTiles.SelectedIndex >= 0)
            {//Put the current index in a buffer.
                int Index = cboTiles.SelectedIndex;

                Helper.RemoveTileset(Index);
                //Move the current tile set.
                cboTiles.Items.RemoveAt(Index);

                //Replace the index with a new one.
                if (cboTiles.Items.Count > 0)
                {
                    if (Index >= cboTiles.Items.Count)
                        cboTiles.SelectedIndex = cboTiles.Items.Count - 1;
                    else
                        cboTiles.SelectedIndex = Index;
                }
                else
                    cboTiles.Text = "";
            }
        }

        //Open a tile attributes dialog.
        protected virtual void btnTileAttributes_Click(object sender, EventArgs e)
        {
            if (cboTiles.SelectedIndex >= 0)
            {
                Rectangle TilePos = new Rectangle(0, 0, ActiveMap.TileSize.X, ActiveMap.TileSize.Y);
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
                }
            }
        }

        private void btn3DTileAttributes_Click(object sender, EventArgs e)
        {
            if (ActiveMap.ListTilesetPreset.Count <= 0)
            {
                return;
            }

            Rectangle TilePos = new Rectangle(0, 0, ActiveMap.TileSize.X, ActiveMap.TileSize.Y);
            TileAttributesEditor3D TileAttributesEditor = new TileAttributesEditor3D(
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTilesetInformation[0].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y],
                ActiveMap);

            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
            {
            }
        }

        #endregion

        #region Autotile

        private void btnAddAutotile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Autotile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapAutotilesPresets));
        }

        private void btnRemoveAutotile_Click(object sender, EventArgs e)
        {//If there's a tile set selected.
            if (cboTiles.SelectedIndex >= 0)
            {//Put the current index in a buffer.
                int Index = cboTiles.SelectedIndex;

                Helper.RemoveTileset(Index);
                //Move the current tile set.
                cboTiles.Items.RemoveAt(Index);

                //Replace the index with a new one.
                if (cboTiles.Items.Count > 0)
                {
                    if (Index >= cboTiles.Items.Count)
                        cboTiles.SelectedIndex = cboTiles.Items.Count - 1;
                    else
                        cboTiles.SelectedIndex = Index;
                }
                else
                    cboTiles.Text = "";
            }
        }

        #endregion

        public void OnMapResize(int NewMapSizeX, int NewMapSizeY)
        {
        }

        public void DrawInfo(ToolStripStatusLabel tslInformation)
        {
            tslInformation.Text += " Left click to place a new spawn point";
            tslInformation.Text += " Right click to remove a spawn point";
        }

        public void DrawMap(CustomSpriteBatch g, GraphicsDevice GraphicsDevice)
        {
            BattleMapViewer.DrawMap();
        }

        private void lvUnits_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public Image Texture2Image(Texture2D sprTexture2D)
        {
            Image ReturnImage;
            using (MemoryStream MS = new MemoryStream())
            {
                sprTexture2D.SaveAsPng(MS, sprTexture2D.Width, sprTexture2D.Height);
                MS.Seek(0, SeekOrigin.Begin);
                ReturnImage = Image.FromStream(MS);
            }

            if (ReturnImage.Width > ActiveMap.TileSize.X)
            {
                Bitmap bmpImage = new Bitmap(ReturnImage);
                ReturnImage = bmpImage.Clone(new Rectangle(0, 0, ActiveMap.TileSize.X, ActiveMap.TileSize.Y), bmpImage.PixelFormat);
            }
            return ReturnImage;
        }

        private Texture2D AddTilesetPreset(TilesetPreset NewTileset)
        {
            for (int BackgroundIndex = 0; BackgroundIndex < NewTileset.ListBattleBackgroundAnimationPath.Count; BackgroundIndex++)
            {
                string NewBattleBackgroundPath = NewTileset.ListBattleBackgroundAnimationPath[BackgroundIndex];

                if (ActiveMap.ListTemporaryBattleBackgroundAnimationPath.Contains(NewBattleBackgroundPath))
                {
                    byte MapBackgroundIndex = (byte)ActiveMap.ListTemporaryBattleBackgroundAnimationPath.IndexOf(NewBattleBackgroundPath);

                    for (int X = 0; X < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(0); ++X)
                    {
                        for (int Y = 0; Y < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(1); ++Y)
                        {
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = MapBackgroundIndex;
                            }
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex = MapBackgroundIndex;
                            }
                        }
                    }
                }
                else
                {
                    byte NewBattleBackgroundIndex = (byte)ActiveMap.ListTemporaryBattleBackgroundAnimationPath.Count;
                    ActiveMap.ListTemporaryBattleBackgroundAnimationPath.Add(NewBattleBackgroundPath);

                    for (int X = 0; X < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(0); ++X)
                    {
                        for (int Y = 0; Y < NewTileset.ArrayTilesetInformation[0].ArrayTerrain.GetLength(1); ++Y)
                        {
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = NewBattleBackgroundIndex;
                            }
                            if (NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                            {
                                NewTileset.ArrayTilesetInformation[0].ArrayTerrain[X, Y].BattleForegroundAnimationIndex = NewBattleBackgroundIndex;
                            }
                        }
                    }
                }
            }

            string TilesetName = NewTileset.ArrayTilesetInformation[0].TilesetName;

            if (NewTileset.TilesetType == TilesetPreset.TilesetTypes.Regular)
            {
                Texture2D NewTilesetSprite = BattleMapViewer.TilesetViewer.content.Load<Texture2D>("Maps/Tilesets/" + TilesetName);
                ActiveMap.ListTemporaryTileSet.Add(NewTilesetSprite);
                AddSceneryImage(TilesetName, NewTilesetSprite);

                return NewTilesetSprite;
            }
            else if (!string.IsNullOrEmpty(NewTileset.ArrayTilesetInformation[0].TilesetName))
            {
                Texture2D NewTilesetSprite = BattleMapViewer.TilesetViewer.content.Load<Texture2D>("Maps/Autotiles/" + TilesetName);
                ActiveMap.ListTemporaryTileSet.Add(NewTilesetSprite);
                AddSceneryImage(TilesetName, NewTilesetSprite);

                return NewTilesetSprite;
            }
            else
            {
                return null;
            }
        }

        private void AddSceneryImage(string TilesetName, Texture2D sprTileset)
        {
            int ImageIndex = lvInfrastructure.Items.Count;
            lvInfrastructure.Items.Add(TilesetName, ImageIndex);
            lvInfrastructure.Items[ImageIndex++].Text = TilesetName;

            imageList.Images.Add("itemImageKey", Texture2Image(sprTileset));
        }

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
                            if (TilePath.StartsWith("Content/Maps/Tilesets Presets"))
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(30);
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    continue;
                                }

                                TilesetPreset NewTileset = Helper.LoadTilesetPreset(Name, ActiveMap.ListTemporaryTilesetPreset.Count);

                                AddTilesetPreset(NewTileset);

                                cboTiles.Items.Add(Name);
                            }
                            else
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(22);
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    continue;
                                }

                                Texture2D Tile = BattleMapViewer.TilesetViewer.content.Load<Texture2D>("Maps/Tilesets/" + Name);

                                ActiveMap.ListTemporaryTilesetPreset.Add(Helper.CreateTilesetPresetFromSprite(Name, Tile.Width, Tile.Height, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, ActiveMap.ListTemporaryTilesetPreset.Count));
                                ActiveMap.ListTemporaryTileSet.Add(Tile);
                                //Add the file name to the tile combo box.
                                cboTiles.Items.Add(Name);
                                AddSceneryImage(Name, Tile);
                            }

                            cboTiles.SelectedIndex = ActiveMap.ListTemporaryTilesetPreset.Count - 1;
                        }
                        break;

                    case ItemSelectionChoices.Autotile:
                        string AutotilePath = Items[I];
                        if (AutotilePath != null)
                        {
                            string Name = AutotilePath.Substring(0, AutotilePath.Length - 5).Substring(31);
                            if (cboTiles.Items.Contains(Name))
                            {
                                MessageBox.Show("This autotile is already listed.\r\n" + Name);
                                continue;
                            }

                            TilesetPreset NewTileset = Helper.LoadAutotilePreset(Name, ActiveMap.ListTemporaryTilesetPreset.Count);

                            for (int i = 0; i < NewTileset.ArrayTilesetInformation.Length; i++)
                            {
                                TilesetPreset ExtraTileset = NewTileset;
                                if (i > 0)
                                {
                                    ExtraTileset = NewTileset.CreateSlave(i);
                                    ActiveMap.ListTemporaryTilesetPreset.Add(ExtraTileset);
                                }
                                Texture2D NewTilesetSprite = AddTilesetPreset(ExtraTileset);

                                BattleMapViewer.TilesetViewer.ListAutoTileSprite.Add(NewTilesetSprite);
                                BattleMapViewer.TilesetViewer.ListAutoTileTilesetPresets.Add(ExtraTileset);
                                cboTiles.Items.Add(Name);
                                cboAutotiles.Items.Add(Name);
                            }
                        }
                        break;

                }
            }
        }
    }
}
