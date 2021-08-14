﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class MapEditor : Form
    {
        private enum ItemSelectionChoices { Map, Tile, TileAsBackground };

        private ItemSelectionChoices ItemSelectionChoice;

        protected BattleMap ActiveMap { get { return BattleMapViewer.ActiveMap; } }

        protected IMapHelper Helper;
        protected ITileAttributes TileAttributesEditor;

        public MapEditorSelector.ChangeTerrainAttribute TerrainAttribute;

        public MapEditor(MapEditorSelector.ChangeTerrainAttribute TerrainAttribute)
        {
            InitializeComponent();

            this.TerrainAttribute = TerrainAttribute;
        }

        private void btnPreviewMap_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Map;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathDeathmatchMaps, "Select a Map", false));
        }

        private void BattleMapViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (BattleMapViewer.ActiveMap != null)
            {
                if (rbEditMap.Checked)
                {
                    EditMap(sender, e);
                }
                else if (rbSelectTilesToReplace.Checked)
                {
                    SelectTilesToReplace(sender, e);
                }
            }
        }

        private void BattleMapViewer_MouseMove(object sender, MouseEventArgs e)
        {
            BattleMapViewer_MouseDown(sender, e);
        }

        private void BattleMapViewer_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void EditMap(object sender, MouseEventArgs e)
        {
            if (BattleMapViewer.ActiveMap != null)
            {
                Vector3 MapPreviewStartingPos = new Vector3(
                    BattleMapViewer.ActiveMap.CameraPosition.X * BattleMapViewer.ActiveMap.TileSize.X,
                    BattleMapViewer.ActiveMap.CameraPosition.Y * BattleMapViewer.ActiveMap.TileSize.Y,
                    BattleMapViewer.ActiveMap.CameraPosition.Z);

                int MouseX = (int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X;
                int MouseY = (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y;

                if (e.Button == MouseButtons.Left)
                {
                    if (cboTiles.Items.Count > 0)
                    {
                        //If Control key is pressed.
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {//Get the Tile under the mouse base on the map starting pos.
                            Point TilePos = new Point(MouseX, MouseY);
                            Terrain SelectedTerrain = Helper.GetTerrain(TilePos.X, TilePos.Y, BattleMapViewer.ActiveMap.ActiveLayerIndex);

                            TileAttributesEditor.Init(SelectedTerrain, ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex]);

                            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                            {
                                Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, BattleMapViewer.ActiveMap.ActiveLayerIndex);
                            }
                        }
                        //Just create a new Tile.
                        else if (BattleMapViewer.ActiveMap.TileSize.X != 0)
                        {
                            Point TilePos = TilesetViewer.ActiveTile;
                            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
                            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                            Helper.ReplaceTerrain((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y,
                                PresetTerrain, BattleMapViewer.ActiveMap.ActiveLayerIndex);

                            Helper.ReplaceTile((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y,
                                PresetTile, BattleMapViewer.ActiveMap.ActiveLayerIndex);
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    int FinalX = e.X / BattleMapViewer.ActiveMap.TileSize.X;
                    int FinalY = e.Y / BattleMapViewer.ActiveMap.TileSize.X;

                    if (FinalX < 0 || FinalX >= BattleMapViewer.ActiveMap.MapSize.X
                        || FinalY < 0 || FinalY >= BattleMapViewer.ActiveMap.MapSize.Y)
                    {
                        return;
                    }

                    for (int S = 0; S < BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count; S++)
                    {
                        if (BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.X == FinalX && BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.Y == FinalY)
                        {
                            BattleMapViewer.ActiveMap.ListSingleplayerSpawns.RemoveAt(S);
                            return;
                        }
                    }
                }
            }
        }

        private void SelectTilesToReplace(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int FinalX = e.X / BattleMapViewer.ActiveMap.TileSize.X;
                int FinalY = e.Y / BattleMapViewer.ActiveMap.TileSize.X;

                if (FinalX < 0 || FinalX >= BattleMapViewer.ActiveMap.MapSize.X
                    || FinalY < 0 || FinalY >= BattleMapViewer.ActiveMap.MapSize.Y)
                {
                    return;
                }

                for (int S = 0; S < BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count; S++)
                {
                    if (BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.X == FinalX && BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.Y == FinalY)
                    {
                        return;
                    }
                }

                BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Add(new EventPoint(new Vector3(FinalX, FinalY, 0), BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count.ToString(), 255, 255, 255));
            }
            else if (e.Button == MouseButtons.Right)
            {
                int FinalX = e.X / BattleMapViewer.ActiveMap.TileSize.X;
                int FinalY = e.Y / BattleMapViewer.ActiveMap.TileSize.X;

                if (FinalX < 0 || FinalX >= BattleMapViewer.ActiveMap.MapSize.X
                    || FinalY < 0 || FinalY >= BattleMapViewer.ActiveMap.MapSize.Y)
                {
                    return;
                }

                for (int S = 0; S < BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count; S++)
                {
                    if (BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.X == FinalX && BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.Y == FinalY)
                    {
                        TerrainAttribute.ListTerrainChangeLocation.RemoveAt(S);
                        TerrainAttribute.ListTileChangeLocation.RemoveAt(S);
                        BattleMapViewer.ActiveMap.ListSingleplayerSpawns.RemoveAt(S);
                        return;
                    }
                }
            }
        }

        //Change the ActiveTile to the mouse position.
        private void TilesetViewer_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                Point DrawOffset = TilesetViewer.DrawOffset;//Used to avoid warnings.
                //Set the ActiveTile to the mouse position.
                TilesetViewer.ActiveTile = new Point(((((MouseEventArgs)e).X + DrawOffset.X) / BattleMapViewer.ActiveMap.TileSize.X) * BattleMapViewer.ActiveMap.TileSize.X,
                                                     ((((MouseEventArgs)e).Y + DrawOffset.Y) / BattleMapViewer.ActiveMap.TileSize.Y) * BattleMapViewer.ActiveMap.TileSize.Y);
            }
        }

        private void cboTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            TilesetViewer.InitTileset(BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex], BattleMapViewer.ActiveMap.TileSize);
        }

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesets));
        }

        private void btnAddNewTileSetAsBackground_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.TileAsBackground;
                ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMapTilesetImages));
            }
        }

        private void btnRemoveTile_Click(object sender, EventArgs e)
        {//If there's a tile set selected.
            if (cboTiles.SelectedIndex >= 0)
            {//Put the current index in a buffer.
                int Index = cboTiles.SelectedIndex;

                Helper.RemoveTileset(Index);
                //Move the current tile set.
                cboTiles.Items.RemoveAt(Index);
                TerrainAttribute.ListTileset.RemoveAt(Index);

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
        protected void btnTileAttributes_Click(object sender, EventArgs e)
        {
            if (cboTiles.SelectedIndex >= 0)
            {
                Point TilePos = TilesetViewer.ActiveTile;
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex]);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            TerrainAttribute.ListTerrainChangeLocation.Clear();
            TerrainAttribute.ListTileChangeLocation.Clear();

            for (int S = 0; S < BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count; S++)
            {
                int FinalX = (int)BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.X;
                int FinalY = (int)BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.Y;
                TerrainAttribute.ListTerrainChangeLocation.Add(Helper.GetTerrain(FinalX, FinalY, 0));
                TerrainAttribute.ListTileChangeLocation.Add(Helper.GetTile(FinalX, FinalY, 0));
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Map:
                        string MapLogicName = Items[0].Substring(0, Items[0].Length - 4).Substring(29);
                        BattleMapViewer.Preload();
                        DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, 0, new List<Core.Units.Squad>());
                        BattleMapViewer.ActiveMap = NewMap;
                        Helper = new DeathmatchMapHelper(NewMap);
                        NewMap.ListGameScreen = new List<GameScreen>();
                        NewMap.Content = BattleMapViewer.content;
                        NewMap.Load();
                        NewMap.TogglePreview(false);
                        NewMap.CursorPositionVisible = new Vector3(-1, -1, 0);

                        BattleMapViewer.SetListMapScript(NewMap.ListMapScript);
                        BattleMapViewer.Helper.OnSelect = (SelectedObject, RightClick) =>
                        {
                            if (RightClick && SelectedObject != null)
                            {
                                BattleMapViewer.cmsScriptMenu.Show(BattleMapViewer, PointToClient(Cursor.Position));
                            }
                        };

                        for (int S = BattleMapViewer.ActiveMap.ListMapScript.Count - 1; S >= 0; --S)
                        {
                            BattleMapViewer.Helper.InitScript(BattleMapViewer.ActiveMap.ListMapScript[S]);
                        }

                        for (int T = 0; T < TerrainAttribute.ListTerrainChangeLocation.Count; T++)
                        {
                            Vector3 NewDestinationPoint = TerrainAttribute.ListTerrainChangeLocation[T].Position;
                            BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Add(new EventPoint(NewDestinationPoint, T.ToString(), 255, 255, 255));
                        }

                        BattleMapViewer.RefreshScrollbars();

                        Matrix Projection = Matrix.CreateOrthographicOffCenter(0, BattleMapViewer.Width, BattleMapViewer.Height, 0, 0, -1f);
                        Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                        Matrix projectionMatrix = HalfPixelOffset * Projection;

                        ActiveMap.fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

                        #region Tiles

                        for (int T = 0; T < BattleMapViewer.ActiveMap.ListTilesetPreset.Count; T++)
                        {
                            TerrainAttribute.ListTileset.Add(BattleMapViewer.ActiveMap.ListTilesetPreset[T].TilesetName);

                            ItemInfo Item = BaseEditor.GetItemByKey(BaseEditor.GUIRootPathMapTilesetImages, BattleMapViewer.ActiveMap.ListTilesetPreset[T].TilesetName);

                            if (Item.Path != null)
                            {
                                if (Item.Name.StartsWith("Tileset presets"))
                                {
                                    cboTiles.Items.Add(Item.Name);
                                }
                                else
                                {
                                    cboTiles.Items.Add(Item.Name);
                                }
                            }
                            else
                            {
                                MessageBox.Show(BattleMapViewer.ActiveMap.ListTilesetPreset[T].TilesetName + " not found, loading default tileset instead.");
                                cboTiles.Items.Add("Default");
                            }
                        }

                        if (BattleMapViewer.ActiveMap.ListTilesetPreset.Count > 0)
                        {
                            cboTiles.SelectedIndex = 0;
                        }

                        if (cboTiles.SelectedIndex >= 0)
                        {
                            TilesetViewer.InitTileset(BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex], BattleMapViewer.ActiveMap.TileSize);
                        }
                        else
                        {
                            TilesetViewer.InitTileset(string.Empty, BattleMapViewer.ActiveMap.TileSize);
                        }

                        TileAttributesEditor = Helper.GetTileEditor();

                        #endregion

                        for (int T = 0; T < TerrainAttribute.ListTerrainChangeLocation.Count; T++)
                        {
                            Terrain ActiveTerrain = TerrainAttribute.ListTerrainChangeLocation[T];
                            int PosX = (int)ActiveTerrain.Position.X;
                            int PosY = (int)ActiveTerrain.Position.Y;
                            DrawableTile ActiveTile = TerrainAttribute.ListTileChangeLocation[T];
                            Helper.ReplaceTerrain(PosX, PosY, ActiveTerrain, 0);
                            Helper.ReplaceTile(PosX, PosY, ActiveTile, 0);
                        }

                        break;

                    case ItemSelectionChoices.Tile:
                        string TilePath = Items[I];
                        if (TilePath != null)
                        {
                            if (TilePath.StartsWith("Content/Maps/Tileset Presets"))
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(29);
                                Terrain.TilesetPreset NewTileset = Terrain.TilesetPreset.FromFile(Name, BattleMapViewer.ActiveMap.ListTilesetPreset.Count);
                                string Output = BaseEditor.GetItemPathInRoot(BaseEditor.GUIRootPathMapTilesets, NewTileset.TilesetName);
                                BattleMapViewer.ActiveMap.ListTilesetPreset.Add(NewTileset);
                                BattleMapViewer.ActiveMap.ListTileSet.Add(TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName));

                                cboTiles.Items.Add(Name);
                                TerrainAttribute.ListTileset.Add(Name);
                            }
                            else
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(22);
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    return;
                                }
                                Microsoft.Xna.Framework.Graphics.Texture2D Tile = TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + Name);

                                BattleMapViewer.ActiveMap.ListTilesetPreset.Add(new Terrain.TilesetPreset(Name, Tile.Width, Tile.Height, BattleMapViewer.ActiveMap.TileSize.X, BattleMapViewer.ActiveMap.TileSize.Y, BattleMapViewer.ActiveMap.ListTilesetPreset.Count));
                                BattleMapViewer.ActiveMap.ListTileSet.Add(Tile);
                                //Add the file name to the tile combo box.
                                cboTiles.Items.Add(Name);
                                TerrainAttribute.ListTileset.Add(Name);
                            }

                            cboTiles.SelectedIndex = BattleMapViewer.ActiveMap.ListTilesetPreset.Count - 1;

                            if (BattleMapViewer.ActiveMap.ListTileSet.Count == 1)
                            {
                                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[0].ArrayTerrain[0, 0];
                                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[0].ArrayTiles[0, 0];

                                //Asign a new tile at the every position, based on its atribtues.
                                for (int X = BattleMapViewer.ActiveMap.MapSize.X - 1; X >= 0; --X)
                                {
                                    for (int Y = BattleMapViewer.ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                    {
                                        Helper.ReplaceTerrain(X, Y, new Terrain(PresetTerrain), 0);
                                        Helper.ReplaceTile(X, Y, new DrawableTile(PresetTile), 0);
                                    }
                                }
                            }
                        }
                        break;

                    case ItemSelectionChoices.TileAsBackground:
                        string TileAsBackgroundPath = Items[I];
                        if (TileAsBackgroundPath != null)
                        {
                            string TileName = Path.GetFileNameWithoutExtension(TileAsBackgroundPath);
                            if (cboTiles.Items.Contains(TileName))
                            {
                                MessageBox.Show("This tile is already listed.\r\n" + TileName);
                                return;
                            }

                            BattleMapViewer.ActiveMap.ListTileSet.Add(TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + TileName));
                            //Add the file name to the tile combo box.
                            cboTiles.Items.Add(TileName);
                            TerrainAttribute.ListTileset.Add(TileName);
                            cboTiles.SelectedIndex = BattleMapViewer.ActiveMap.ListTileSet.Count - 1;

                            //Asign a new tile at the every position, based on its atribtues.
                            for (int X = BattleMapViewer.ActiveMap.MapSize.X - 1; X >= 0; --X)
                            {
                                for (int Y = BattleMapViewer.ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                {
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y,
                                       0, 0, 1, new TerrainActivation[0], new TerrainBonus[0], new int[0], -1),
                                       0);

                                    Helper.ReplaceTile(X, Y,
                                       new DrawableTile(
                                           new Rectangle((X % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(0))) * ActiveMap.TileSize.X,
                                                        (Y % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(1))) * ActiveMap.TileSize.Y,
                                                        ActiveMap.TileSize.X, ActiveMap.TileSize.Y),
                                           cboTiles.Items.Count - 1),
                                       0);
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}
