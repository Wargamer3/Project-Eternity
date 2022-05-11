using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Editors.MusicPlayer;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.Editors.MapEditor
{
    public partial class ProjectEternityMapEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Tile, TileAsBackground, BGM, UnitPosition, Cutscene };

        private ItemSelectionChoices ItemSelectionChoice;

        protected int TerrainTypeIndex = 0;

        private CheckBox cbShowGrid;
        private CheckBox cbPreviewMap;
        private CheckBox cbShowAllLayers;
        private CheckBox cbShowTerrainType;
        private CheckBox cbShowTerrainHeight;

        protected BattleMap ActiveMap;
        protected IMapHelper Helper;

        //Spawn point related stuff.
        private EventPoint ActiveSpawn;

        protected ITileAttributes TileAttributesEditor;

        public ProjectEternityMapEditor()
        {
            InitializeComponent();

            #region cbShowGrid

            //Init the ShowGrid button (as it can't be done with the tool box)
            cbShowGrid = new CheckBox
            {
                Text = "Show grid"
            };
            //Link a CheckedChanged event to a method.
            cbShowGrid.CheckedChanged += new EventHandler(cbShowGrid_CheckedChanged);
            cbShowGrid.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowGrid.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowGrid));

            #endregion

            #region cbPreviewMap

            cbPreviewMap = new CheckBox
            {
                Text = "Preview Map"
            };
            //Link a CheckedChanged event to a method.
            cbPreviewMap.CheckedChanged += new EventHandler(cbPreviewMap_CheckedChanged);
            cbPreviewMap.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            cbPreviewMap.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbPreviewMap));

            #endregion

            #region cbShowAllLayers

            cbShowAllLayers = new CheckBox
            {
                Text = "Show All Layers"
            };
            //Link a CheckedChanged event to a method.
            cbShowAllLayers.CheckedChanged += new EventHandler(cbShowAllLayers_CheckedChanged);
            cbShowAllLayers.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowAllLayers.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowAllLayers));

            #endregion

            #region cbShowTerrainType

            cbShowTerrainType = new CheckBox
            {
                Text = "Show terrain type"
            };
            //Link a CheckedChanged event to a method.
            cbShowTerrainType.CheckedChanged += new EventHandler(cbShowTerrainType_CheckedChanged);
            cbShowTerrainType.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowTerrainType.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowTerrainType));

            #endregion

            #region cbShowTerrainType

            cbShowTerrainHeight = new CheckBox
            {
                Text = "Show terrain height"
            };
            //Link a CheckedChanged event to a method.
            cbShowTerrainHeight.CheckedChanged += new EventHandler(cbShowTerrainHeight_CheckedChanged);
            cbShowTerrainHeight.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowTerrainHeight.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowTerrainHeight));

            #endregion

            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
        }

        public ProjectEternityMapEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                DeathmatchMap NewMap = new DeathmatchMap(FilePath, string.Empty);
                ActiveMap = BattleMapViewer.ActiveMap = NewMap;
                NewMap.LayerManager.ListLayer.Add(new MapLayer(NewMap, 0));

                SaveItem(FilePath, FilePath);
            }

            LoadMap(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathMaps, GUIRootPathDeathmatchMaps }, "Maps/Deathmatch/", new string[] { ".pem" }, typeof(ProjectEternityMapEditor)),
                new EditorInfo(new string[] { GUIRootPathMapBGM }, "Maps/BGM/", new string[] { ".mp3" }, typeof(ProjectEternityMusicPlayerEditor), false)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            BattleMapViewer.ActiveMap.MapName = ItemName;

            ActiveMap.Save(ItemPath);
        }

        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(24);

            BattleMapViewer.Preload();
            DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, string.Empty);
            Helper = new DeathmatchMapHelper(NewMap);
            InitMap(NewMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity Deathmatch Map Editor";
        }

        protected void InitMap(BattleMap NewMap)
        {
            ActiveMap = NewMap;
            ActiveMap.IsEditor = true;
            BattleMapViewer.ActiveMap = NewMap;
            NewMap.ListGameScreen = new List<GameScreens.GameScreen>();
            NewMap.Content = BattleMapViewer.content;
            Helper.InitMap();
            ActiveMap.TogglePreview(true);

            BattleMapViewer.SetListMapScript(NewMap.ListMapScript);
            BattleMapViewer.Helper.OnSelect = (SelectedObject, RightClick) =>
            {
                if (RightClick && SelectedObject != null)
                {
                    BattleMapViewer.cmsScriptMenu.Show(BattleMapViewer, PointToClient(Cursor.Position));
                }
                else
                {
                    pgScriptProperties.SelectedObject = SelectedObject;
                }
            };

            for (int S = BattleMapViewer.ActiveMap.ListMapScript.Count - 1; S >= 0; --S)
            {
                BattleMapViewer.Helper.InitScript(BattleMapViewer.ActiveMap.ListMapScript[S]);
            }

            if (NewMap.ListMultiplayerColor.Count > 0)
            {
                for (int C = 0; C < NewMap.ListMultiplayerColor.Count; C++)
                {
                    cbDeadthmatch.Items.Add(C + 1);
                }

                btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(BattleMapViewer.ActiveMap.ListMultiplayerColor[0].R, BattleMapViewer.ActiveMap.ListMultiplayerColor[0].G, BattleMapViewer.ActiveMap.ListMultiplayerColor[0].B);
            }
        }

        private void DrawInfo(int MouseX, int MouseY)
        {//Draw the mouse position minus the map starting point.
            tslInformation.Text = string.Format("X: {0}, Y: {1}", MouseX, MouseY);
            if (tabToolBox.SelectedIndex == 0)
            {
                //Add the selection informations.
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    tslInformation.Text += " Tile attribues";
                else
                {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        tslInformation.Text += " Multiple";
                    else
                        tslInformation.Text += " Hold shift to place multiple tiles";
                    tslInformation.Text += ", hold ctrl to change the selected tile attributes";
                }
            }
            else if (tabToolBox.SelectedIndex == 1)
            {
                if (TilesetViewer.ActiveTile != null)
                    tslInformation.Text += " Left click to place a new spawn point";
                tslInformation.Text += " Right click to remove a spawn point";
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    tslInformation.Text += " Multiple";
                else
                    tslInformation.Text += " Hold shift to place multiple tiles";
            }
        }

        #region Tile set change

        //Change the ActiveTile to the mouse position.
        private void TileViewer_Click(object sender, EventArgs e)
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
            //Initialise the scroll bars.
            if (BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width >= TilesetViewer.Width)
            {
                sclTileWidth.Maximum = BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width - TilesetViewer.Width;
                sclTileWidth.Visible = true;
            }
            else
                sclTileWidth.Visible = false;
            if (BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height >= TilesetViewer.Height)
            {
                sclTileHeight.Maximum = BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height - TilesetViewer.Height;
                sclTileHeight.Visible = true;
            }
            else
                sclTileHeight.Visible = false;

            TilesetViewer.InitTileset(BattleMapViewer.ActiveMap.ListTileSet[cboTiles.SelectedIndex], BattleMapViewer.ActiveMap.TileSize);
        }

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapTilesets));
        }

        private void btnAddNewTileSetAsBackground_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                ItemSelectionChoice = ItemSelectionChoices.TileAsBackground;
                ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapTilesetImages));
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
                Point TilePos = TilesetViewer.ActiveTile;
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex]);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
                }
            }
        }

        private void btn3DTileAttributes_Click(object sender, EventArgs e)
        {
            Point TilePos = TilesetViewer.ActiveTile;
            TileAttributesEditor3D TileAttributesEditor = new TileAttributesEditor3D(
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y],
                ActiveMap);

            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
            {
            }
        }

        #endregion

        #region Map

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.X || KeyboardHelper.KeyHold(Microsoft.Xna.Framework.Input.Keys.X))
            {
                PlaceTile((int)BattleMapViewer.ActiveMap.CursorPosition.X, (int)BattleMapViewer.ActiveMap.CursorPosition.Y, (int)BattleMapViewer.ActiveMap.CursorPosition.Z, false);
            }
            return true;
        }

        protected virtual void pnMapPreview_MouseMove(object sender, MouseEventArgs e)
        {
            Vector3 MapPreviewStartingPos = new Vector3(
                BattleMapViewer.ActiveMap.CameraPosition.X * BattleMapViewer.ActiveMap.TileSize.X,
                BattleMapViewer.ActiveMap.CameraPosition.Y * BattleMapViewer.ActiveMap.TileSize.Y,
                BattleMapViewer.ActiveMap.CameraPosition.Z);

            DrawInfo((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y);
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                switch (tabToolBox.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 4:
                        Map_MouseMove(e);
                        break;

                    case 2:
                        BattleMapViewer.Scripting_MouseMove(e);
                        break;

                    case 5:
                        BattleMapViewer.Zones_MouseMove(e);
                        break;
                }
            }
            else
            {
                switch (tabToolBox.SelectedIndex)
                {
                    case 0:
                    case 1:

                        break;

                    case 2:
                        BattleMapViewer.Scripting_MouseMove(e);
                        break;

                    case 5:
                        BattleMapViewer.Zones_MouseMove(e);
                        break;
                }
            }
        }

        protected virtual void pnMapPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                switch (tabToolBox.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 4:
                        pnMapPreview_MouseMove(sender, e);
                        break;

                    case 2:
                        BattleMapViewer.Scripting_MouseUp(e);
                        break;

                    case 5:
                        BattleMapViewer.Zones_MouseUp(e);
                        break;
                }
            }
        }

        protected virtual void pnMapPreview_MouseDown(object sender, MouseEventArgs e)
        {
            switch (tabToolBox.SelectedIndex)
            {
                case 0:
                case 1:

                    break;

                case 2:
                    BattleMapViewer.Scripting_MouseDown(e);
                    break;

                case 5:
                    BattleMapViewer.Zones_MouseDown(e);
                    break;
            }
        }

        private void Map_MouseMove(MouseEventArgs e)
        {
            Vector3 MapPreviewStartingPos = new Vector3(
                BattleMapViewer.ActiveMap.CameraPosition.X * BattleMapViewer.ActiveMap.TileSize.X,
                BattleMapViewer.ActiveMap.CameraPosition.Y * BattleMapViewer.ActiveMap.TileSize.Y,
                BattleMapViewer.ActiveMap.CameraPosition.Z);

            int MouseX = (int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X;
            int MouseY = (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y;

            if (MouseX < 0 || MouseX >= ActiveMap.MapSize.X || MouseY < 0 || MouseY >= ActiveMap.MapSize.Y)
                return;

            if (e.Button == MouseButtons.Left)
            {
                //Tile tab
                if (tabToolBox.SelectedIndex == 0)
                {//If there is at least one tile set.
                    if (cboTiles.Items.Count > 0)
                    {
                        //If Control key is pressed.
                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                        {//Get the Tile under the mouse base on the map starting pos.
                            Point TilePos = new Point(MouseX, MouseY);
                            Terrain SelectedTerrain = Helper.GetTerrain(TilePos.X, TilePos.Y, lsLayers.SelectedIndex);

                            TileAttributesEditor.Init(SelectedTerrain, ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex]);

                            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                            {
                                Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, lsLayers.SelectedIndex, true);
                            }
                        }
                        //Just create a new Tile.
                        else if (BattleMapViewer.ActiveMap.TileSize.X != 0)
                        {
                            PlaceTile((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y, lsLayers.SelectedIndex, true);
                        }
                    }
                }
                //Spawn tab
                else if (tabToolBox.SelectedIndex == 1)
                {
                    HandleEventPoint(MouseX, MouseY, ActiveSpawn);
                }
                //Spawn tab
                else if (tabToolBox.SelectedIndex == 4)
                {
                    HandleProps(MouseX, MouseY);
                }
            }

            #region Right click

            else if (e.Button == MouseButtons.Right)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {//Get the Tile under the mouse base on the map starting pos.
                    Point TilePos = new Point(MouseX, MouseY);
                    DrawableTile SelectedTerrain = Helper.GetTile(TilePos.X, TilePos.Y, lsLayers.SelectedIndex);

                    TileAttributesEditor3D TileAttributesEditor = new TileAttributesEditor3D(
                        SelectedTerrain,
                        ActiveMap);

                    if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                    {
                    }
                }
                //If there is a map loaded
                else if (BattleMapViewer.ActiveMap.TileSize.X != 0)
                {
                    //Spawn tab
                    if (tabToolBox.SelectedIndex == 1)
                    {
                        HandleEventPoint(MouseX, MouseY, null);
                    }
                    //Trigger tab
                    else if (tabToolBox.SelectedIndex == 2)
                    {
                    }
                    //Spawn tab
                    else if (tabToolBox.SelectedIndex == 4)
                    {
                        RemoveProps(MouseX, MouseY);
                    }
                }
            }

            #endregion
        }

        private void PlaceTile(int X, int Y, int LayerIndex, bool ConsiderSubLayers)
        {
            Point TilePos = TilesetViewer.ActiveTile;
            if (TilePos.X >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(0) * ActiveMap.TileSize.X
                || TilePos.Y >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(1) * ActiveMap.TileSize.Y)
            {
                return;
            }

            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            Helper.ReplaceTerrain(X, Y,
                PresetTerrain, LayerIndex, ConsiderSubLayers);

            Helper.ReplaceTile(X, Y,
                PresetTile, LayerIndex, ConsiderSubLayers);
        }

        #endregion

        #region Spawn points

        private void HandleEventPoint(int X, int Y, EventPoint Spawn)
        {//If there is an active Spawn and a map loaded.
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                if (btnTeleporters.Checked)
                {
                    NewTeleporterPoint(X, Y, Spawn);
                }
                else if (btnMapSwitches.Checked)
                {
                    NewMapSwitchPoint(X, Y, Spawn);
                }
                else if (btnSpawnDM.Checked)
                {
                    NewSpawnMultiplayer(X, Y, Spawn);
                }
                else
                {
                    NewSpawnSingleplayer(X, Y, Spawn);
                }
            }
        }

        private void NewSpawnSingleplayer(int X, int Y, EventPoint Spawn)
        {
            int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, TopLayerIndex);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListSingleplayerSpawns.Count; S++)
            {//If it exist.
                if (TopLayer.ListSingleplayerSpawns[S].Position.X == X && TopLayer.ListSingleplayerSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    TopLayer.ListSingleplayerSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        TopLayer.ListSingleplayerSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListSingleplayerSpawns.Add(Spawn);
            }
        }

        private void NewSpawnMultiplayer(int X, int Y, EventPoint Spawn)
        {
            int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, TopLayerIndex);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListMultiplayerSpawns.Count; S++)
            {//If it exist.
                if (TopLayer.ListMultiplayerSpawns[S].Position.X == X && TopLayer.ListMultiplayerSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    TopLayer.ListMultiplayerSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        TopLayer.ListMultiplayerSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListMultiplayerSpawns.Add(Spawn);
            }
        }

        private void NewMapSwitchPoint(int X, int Y, EventPoint Spawn)
        {
            int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];
            MapSwitchPoint OldEventPoint = null;

            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListMapSwitchPoint.Count; S++)
            {//If it exist.
                if (TopLayer.ListMapSwitchPoint[S].Position.X == X && TopLayer.ListMapSwitchPoint[S].Position.Y == Y)
                {
                    OldEventPoint = TopLayer.ListMapSwitchPoint[S];
                }
            }

            if (Spawn != null)
            {
                if (OldEventPoint == null)
                {
                    MapSwitchPoint NewMapSwitchPoint = new MapSwitchPoint(Spawn);
                    NewMapSwitchPoint.Position = new Vector3(X, Y, TopLayerIndex);
                    TopLayer.ListMapSwitchPoint.Add(NewMapSwitchPoint);
                    pgEventPoints.SelectedObject = NewMapSwitchPoint;
                }
                else
                {
                    pgEventPoints.SelectedObject = OldEventPoint;
                }
            }
            else if (OldEventPoint != null)
            {
                TopLayer.ListMapSwitchPoint.Remove(OldEventPoint);
            }
        }

        private void NewTeleporterPoint(int X, int Y, EventPoint Spawn)
        {
            int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
            BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];
            TeleportPoint OldEventPoint = null;

            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < TopLayer.ListTeleportPoint.Count; S++)
            {//If it exist.
                if (TopLayer.ListTeleportPoint[S].Position.X == X && TopLayer.ListTeleportPoint[S].Position.Y == Y)
                {
                    OldEventPoint = TopLayer.ListTeleportPoint[S];
                }
            }

            if (Spawn != null)
            {
                if (OldEventPoint == null)
                {
                    TeleportPoint NewMapSwitchPoint = new TeleportPoint(Spawn);
                    NewMapSwitchPoint.Position = new Vector3(X, Y, TopLayerIndex);
                    TopLayer.ListTeleportPoint.Add(NewMapSwitchPoint);
                    pgEventPoints.SelectedObject = NewMapSwitchPoint;
                }
                else
                {
                    pgEventPoints.SelectedObject = OldEventPoint;
                }
            }
            else if (OldEventPoint != null)
            {
                TopLayer.ListTeleportPoint.Remove(OldEventPoint);
            }
        }

        #region Selection spawn changes

        private void ResetSpawn(CheckBox Sender)
        {
            ActiveSpawn = null;
            if (Sender != btnSpawnPlayer)
                btnSpawnPlayer.Checked = false;
            if (Sender != btnSpawnEnemy)
                btnSpawnEnemy.Checked = false;
            if (Sender != btnSpawnDM)
                btnSpawnDM.Checked = false;
            if (Sender != btnEventSpawn)
                btnEventSpawn.Checked = false;
            if (Sender != btnMapSwitches)
                btnMapSwitches.Checked = false;
            if (Sender != btnTeleporters)
                btnTeleporters.Checked = false;
        }

        private void btnSpawnPlayer_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnPlayer.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnPlayer);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnPlayer.Text, btnSpawnPlayer.BackColor.R, btnSpawnPlayer.BackColor.G, btnSpawnPlayer.BackColor.B);
            }
        }

        private void btnSpawnEnemy_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnEnemy.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnEnemy);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnEnemy.Text, btnSpawnEnemy.BackColor.R, btnSpawnEnemy.BackColor.G, btnSpawnEnemy.BackColor.B);
            }
        }

        private void btnSpawnNeutral_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnNeutral.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnNeutral);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnNeutral.Text, btnSpawnNeutral.BackColor.R, btnSpawnNeutral.BackColor.G, btnSpawnNeutral.BackColor.B);
            }
        }

        private void btnSpawnAlly_CheckedChanged(object sender, EventArgs e)
        {
            if (btnSpawnAlly.Checked)
            {
                //Reset the Spawn buttons
                ResetSpawn(btnSpawnAlly);
                //Set a new ActiveSpawn.
                ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnAlly.Text, btnSpawnAlly.BackColor.R, btnSpawnAlly.BackColor.G, btnSpawnAlly.BackColor.B);
            }
        }

        private void btnSpawnDM_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox CheckBoxSender = (CheckBox)sender;
            //Reset the Spawn buttons
            ResetSpawn((CheckBox)sender);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, CheckBoxSender.Text, CheckBoxSender.BackColor.R, CheckBoxSender.BackColor.G, CheckBoxSender.BackColor.B);
        }

        #endregion

        #region Multiplayer

        private void btnSpawnDM_MouseMove(object sender, MouseEventArgs e)
        {//If left clicked and moving, open the team selector.
            if (e.Button == MouseButtons.Left)
                cbDeadthmatch.DroppedDown = true;
        }

        private void btnSpawnDM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {//If left click and over the little black cursor at the bottom right, open the team selector.
                if (e.X > btnSpawnDM.Width - 10 && e.Y > btnSpawnDM.Height - 10)
                    cbDeadthmatch.DroppedDown = true;
            }
        }

        private void btnSpawnDM_MouseDown(object sender, MouseEventArgs e)
        {//If right clicked, open a new color Dialog.
            if (e.Button == MouseButtons.Right)
            {
                ColorDialog CD = new ColorDialog();
                if (CD.ShowDialog() == DialogResult.OK)
                {//Change the button color and the color in the list at the same time with the returned color.
                    btnSpawnDM.BackColor = CD.Color;
                    int MPColorIndex = Math.Max(0, cbDeadthmatch.SelectedIndex);
                    BattleMapViewer.ActiveMap.ListMultiplayerColor[MPColorIndex] = Color.FromNonPremultiplied(CD.Color.R, CD.Color.G, CD.Color.B, 255);
                    if (btnSpawnDM.Checked)
                    {
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.R;
                        ActiveSpawn.ColorGreen = btnSpawnDM.BackColor.G;
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.B;
                    }

                    foreach (BaseMapLayer ActiveLayer in Helper.GetLayersAndSubLayers())
                    {
                        for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                        {
                            if (ActiveLayer.ListMultiplayerSpawns[S].Tag == btnSpawnDM.Text)
                            {
                                ActiveLayer.ListMultiplayerSpawns[S].ColorRed = CD.Color.R;
                                ActiveLayer.ListMultiplayerSpawns[S].ColorGreen = CD.Color.G;
                                ActiveLayer.ListMultiplayerSpawns[S].ColorBlue = CD.Color.B;
                            }
                        }
                    }
                }
            }
        }

        //A new team is selected.
        private void cbDeadthmatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSpawnDM.Text = cbDeadthmatch.Text;//Give the button the selected text.
            btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(BattleMapViewer.ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].R,
                                                                 BattleMapViewer.ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].G,
                                                                 BattleMapViewer.ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].B);//Give the button the selected color.
            btnSpawnDM.Checked = true;//Press the button.
            //Update the ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnDM.Text, btnSpawnDM.BackColor.R, btnSpawnDM.BackColor.G, btnSpawnDM.BackColor.B);
        }

        private void btnAddDeathmatchTeam_Click(object sender, EventArgs e)
        {
            Color[] ArrayColorChoices = new Color[] { Color.Turquoise, Color.White, Color.SteelBlue, Color.Silver, Color.SandyBrown, Color.Salmon, Color.Purple, Color.PaleGreen, Color.Orange, Color.Gold, Color.ForestGreen, Color.Firebrick, Color.Chartreuse, Color.Beige, Color.DeepPink, Color.DarkMagenta };
            BattleMapViewer.ActiveMap.ListMultiplayerColor.Add(ArrayColorChoices[Math.Min(ArrayColorChoices.Length - 1, BattleMapViewer.ActiveMap.ListMultiplayerColor.Count)]);
            cbDeadthmatch.Items.Add(BattleMapViewer.ActiveMap.ListMultiplayerColor.Count);
        }

        private void btnRemoveDeathmatchTeam_Click(object sender, EventArgs e)
        {
            if (cbDeadthmatch.SelectedIndex >= 0)
            {
                BattleMapViewer.ActiveMap.ListMultiplayerColor.RemoveAt(cbDeadthmatch.SelectedIndex);
                cbDeadthmatch.Items.RemoveAt(cbDeadthmatch.SelectedIndex);
            }
        }

        #endregion

        private void btnEventSpawn_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEventSpawn.Checked)
            {
                ResetSpawn((CheckBox)sender);
                ActiveSpawn = new EventPoint(Vector3.Zero, "O", Color.DarkViolet.R, Color.DarkViolet.G, Color.DarkViolet.B);
            }
        }
        
        private void btnMapSwitches_CheckedChanged(object sender, EventArgs e)
        {
            //Reset the Spawn buttons
            ResetSpawn(btnMapSwitches);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnMapSwitches.Text, btnMapSwitches.BackColor.R, btnMapSwitches.BackColor.G, btnMapSwitches.BackColor.B);
        }

        private void btnTeleporters_CheckedChanged(object sender, EventArgs e)
        {
            //Reset the Spawn buttons
            ResetSpawn(btnTeleporters);
            //Set a new ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnTeleporters.Text, btnTeleporters.BackColor.R, btnTeleporters.BackColor.G, btnTeleporters.BackColor.B);
        }

        #endregion

        #region Scroll bars

        private void sclTileWidth_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = TilesetViewer.DrawOffset;
            DrawOffset.X = e.NewValue;
            TilesetViewer.DrawOffset = DrawOffset;
        }

        private void sclTileHeight_Scroll(object sender, ScrollEventArgs e)
        {
            Point DrawOffset = TilesetViewer.DrawOffset;
            DrawOffset.Y = e.NewValue;
            TilesetViewer.DrawOffset = DrawOffset;
        }

        #endregion

        private void lstChoices_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is ListBox))
                return;
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            BattleMapViewer.Helper.CreateScript((MapScript)((ListBox)sender).SelectedItem);
        }

        #region Extra Layers

        private void btnAddExtraLayer_Click(object sender, EventArgs e)
        {
            Point TilePos = TilesetViewer.ActiveTile;
            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            lsLayers.Items.Add(Helper.CreateNewLayer(PresetTerrain, PresetTile));
        }

        private void btnRemoveExtraLayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                int Index = lsLayers.SelectedIndex;
                Helper.RemoveLayer(Index);
                lsLayers.Items.RemoveAt(Index);
            }
        }

        private void btnAddSublayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                for (int L = lsLayers.SelectedIndex; L >= 0; --L)
                {
                    if (lsLayers.Items[L] is BaseMapLayer)
                    {
                        lsLayers.Items.Insert(L + 1, Helper.CreateNewSubLayer((BaseMapLayer)lsLayers.Items[L]));
                        break;
                    }
                }
            }
        }

        private void btnRemoveSublayer_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                for (int L = lsLayers.SelectedIndex; L >= 0; --L)
                {
                    if (lsLayers.Items[L] is BaseMapLayer)
                    {
                        Helper.RemoveSubLayer((BaseMapLayer)lsLayers.Items[L], (ISubMapLayer)lsLayers.Items[lsLayers.SelectedIndex]);
                        lsLayers.Items.RemoveAt(lsLayers.SelectedIndex);
                    }
                }
            }
        }

        private void btnLayerAttributes_Click(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                Helper.EditLayer(lsLayers.SelectedIndex);
            }
        }
        
        private void lsLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsLayers.SelectedIndex >= 0)
            {
                if (cbShowAllLayers.Checked)
                {
                    ActiveMap.ShowLayerIndex = -1;
                }
                else
                {
                    ActiveMap.ShowLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
                }
            }
        }

        #endregion

        #region Props

        private void HandleProps(int X, int Y)
        {
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
                BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];

                //Loop in the Prop list to find if a Prop already exist at the X, Y position.
                for (int P = 0; P < TopLayer.ListProp.Count; P++)
                {
                    if (TopLayer.ListProp[P].Position.X == X && TopLayer.ListProp[P].Position.Y == Y)
                    {
                        pgPropProperties.SelectedObject = TopLayer.ListProp[P];
                        return;
                    }
                }

                InteractiveProp ActiveProp = null;
                if (tabPropsChoices.SelectedIndex == 0 && lsInteractiveProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsInteractiveProps.SelectedItem;
                }
                else if (tabPropsChoices.SelectedIndex == 1 && lsPhysicalProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsPhysicalProps.SelectedItem;
                }
                else if (tabPropsChoices.SelectedIndex == 2 && lsVisualProps.SelectedItem != null)
                {
                    ActiveProp = (InteractiveProp)lsVisualProps.SelectedItem;
                }
                else
                {
                    return;
                }

                ActiveProp = ActiveProp.Copy(new Vector3(X, Y, 0), TopLayerIndex);
                pgPropProperties.SelectedObject = ActiveProp;

                TopLayer.ListProp.Add(ActiveProp);
            }
        }

        private void RemoveProps(int X, int Y)
        {
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                int TopLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
                BaseMapLayer TopLayer = Helper.GetLayersAndSubLayers()[lsLayers.SelectedIndex];

                //Loop in the Prop list to find if a Prop already exist at the X, Y position.
                for (int P = 0; P < TopLayer.ListProp.Count; P++)
                {
                    if (TopLayer.ListProp[P].Position.X == X && TopLayer.ListProp[P].Position.Y == Y)
                    {
                        TopLayer.ListProp.RemoveAt(P);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Zones

        private void btnAddZoneRectangle_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Rectangle);
            BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneOval_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Oval);
            BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneFullMap_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Full);
            BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnRemoveZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone.RemoveAt(lsZones.SelectedIndex);
                lsZones.Items.RemoveAt(lsZones.SelectedIndex);
            }
        }

        private void btnEditZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                new ZoneEditor(BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex]).ShowDialog();
            }
        }

        private void lsZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                pgZoneProperties.SelectedObject = BattleMapViewer.ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex];
            }
        }

        #endregion

        private void cbShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ActiveMap.ShowGrid = cbShowGrid.Checked;
        }

        private void cbShowTerrainType_CheckedChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ActiveMap.ShowTerrainType = cbShowTerrainType.Checked;
        }

        private void cbShowTerrainHeight_CheckedChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ActiveMap.ShowTerrainHeight = cbShowTerrainHeight.Checked;
        }

        private void cbPreviewMap_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveMap != null)
            {
                ActiveMap.TogglePreview(cbPreviewMap.Checked);
            }
        }

        private void cbShowAllLayers_CheckedChanged(object sender, EventArgs e)
        {
            if (ActiveMap != null)
            {
                if (cbShowAllLayers.Checked)
                {
                    ActiveMap.ShowLayerIndex = -1;
                }
                else
                {
                    ActiveMap.ShowLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
                }
            }
        }

        private int GetRealTopLayerIndex(int LayerIndex)
        {
            int RealIndex = 0;
            int LastTopLayerIndex = -1;

            foreach (object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                if (!(ActiveLayer is ISubMapLayer))
                {
                    ++LastTopLayerIndex;
                }
                if (RealIndex == LayerIndex)
                {
                    return LastTopLayerIndex;
                }

                ++RealIndex;
            }

            return LastTopLayerIndex;
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
                            if (TilePath.StartsWith("Content/Maps/Tileset Presets"))
                            {
                                string Name = TilePath.Substring(0, TilePath.Length - 4).Substring(29);
                                Terrain.TilesetPreset NewTileset = Terrain.TilesetPreset.FromFile(Name, BattleMapViewer.ActiveMap.ListTilesetPreset.Count);
                                string Output = GetItemPathInRoot(GUIRootPathMapTilesets, NewTileset.TilesetName);
                                BattleMapViewer.ActiveMap.ListTilesetPreset.Add(NewTileset);
                                BattleMapViewer.ActiveMap.ListTileSet.Add(TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName));

                                cboTiles.Items.Add(Name);
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
                                        Helper.ReplaceTerrain(X, Y, new Terrain(PresetTerrain), 0, true);
                                        Helper.ReplaceTile(X, Y, new DrawableTile(PresetTile), 0, true);
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
                            cboTiles.SelectedIndex = BattleMapViewer.ActiveMap.ListTileSet.Count - 1;

                            //Initialise the scroll bars.
                            if (BattleMapViewer.ActiveMap.ListTileSet.Last().Width >= TilesetViewer.Width)
                            {
                                sclTileWidth.Maximum = BattleMapViewer.ActiveMap.ListTileSet.Last().Width - TilesetViewer.Width - 1;
                                sclTileWidth.Visible = true;
                            }
                            else
                                sclTileWidth.Visible = false;
                            if (BattleMapViewer.ActiveMap.ListTileSet.Last().Height >= TilesetViewer.Height)
                            {
                                sclTileHeight.Maximum = BattleMapViewer.ActiveMap.ListTileSet.Last().Height - TilesetViewer.Height - 1;
                                sclTileHeight.Visible = true;
                            }
                            else
                                sclTileHeight.Visible = false;

                            //Asign a new tile at the every position, based on its atribtues.
                            for (int X = BattleMapViewer.ActiveMap.MapSize.X - 1; X >= 0; --X)
                            {
                                for (int Y = BattleMapViewer.ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                {
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y, lsLayers.SelectedIndex, 0,
                                       0, 0, 1, new TerrainActivation[0], new TerrainBonus[0], new int[0]),
                                       0, true);

                                    Helper.ReplaceTile(X, Y, 
                                       new DrawableTile(
                                           new Rectangle((X % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(0))) * ActiveMap.TileSize.X,
                                                        (Y % (ActiveMap.ListTilesetPreset.Last().ArrayTerrain.GetLength(1))) * ActiveMap.TileSize.Y,
                                                        ActiveMap.TileSize.X, ActiveMap.TileSize.Y),
                                           cboTiles.Items.Count - 1),
                                       0, true);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapStatistics MS = new MapStatistics(ActiveMap.MapName, ActiveMap.MapSize, ActiveMap.TileSize, ActiveMap.CameraType, ActiveMap.CameraPosition, ActiveMap.PlayersMin, ActiveMap.PlayersMax, ActiveMap.Description);
            if (MS.ShowDialog() == DialogResult.OK)
            {
                Point MapSize = new Point((int)MS.txtMapWidth.Value, (int)MS.txtMapHeight.Value);
                Point TileSize = new Point((int)MS.txtTileWidth.Value, (int)MS.txtTileHeight.Value);
                Vector3 CameraPosition = new Vector3((float)MS.txtCameraStartPositionX.Value, (float)MS.txtCameraStartPositionY.Value, 0);

                if (TileSize.X != ActiveMap.TileSize.X || TileSize.Y != ActiveMap.TileSize.Y)
                {
                    for (int T = 0; T < ActiveMap.ListTilesetPreset.Count; ++T)
                    {
                        ActiveMap.ListTilesetPreset[T] = new Terrain.TilesetPreset(ActiveMap.ListTilesetPreset[T].TilesetName,
                                                                                    ActiveMap.ListTilesetPreset[T].ArrayTerrain.GetLength(0) * ActiveMap.TileSize.X,
                                                                                    ActiveMap.ListTilesetPreset[T].ArrayTerrain.GetLength(1) * ActiveMap.TileSize.Y,
                                                                                    TileSize.X, TileSize.Y, T);
                    }

                    ActiveMap.TileSize = new Point(TileSize.X, TileSize.Y);
                }

                ActiveMap.CameraType = MS.cbCameraType.Text;
                ActiveMap.CameraPosition = CameraPosition;
                ActiveMap.PlayersMin = (byte)MS.txtPlayersMin.Value;
                ActiveMap.PlayersMax = (byte)MS.txtPlayersMax.Value;
                ActiveMap.Description = MS.txtDescription.Text;

                Point TilePos = TilesetViewer.ActiveTile;
                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                Helper.ResizeTerrain(MapSize.X, MapSize.Y, PresetTerrain, PresetTile);

                BattleMapViewer.RefreshScrollbars();
            }

            ActiveMap.ListBackgroundsPath.Clear();
            ActiveMap.ListBackgroundsPath.AddRange(MS.ListBackgroundsPath);
            ActiveMap.ListForegroundsPath.Clear();
            ActiveMap.ListForegroundsPath.AddRange(MS.ListForegroundsPath);
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tabToolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ViewerIndex = tabToolBox.SelectedIndex;
        }

        private void ProjectEternityMapEditor_Shown(object sender, EventArgs e)
        {
            if (BattleMapViewer.ActiveMap == null)
                return;

            lstEvents.Items.AddRange(BattleMapViewer.ActiveMap.DicMapEvent.Values.ToArray());
            lstConditions.Items.AddRange(BattleMapViewer.ActiveMap.DicMapCondition.Values.ToArray());
            lstTriggers.Items.AddRange(BattleMapViewer.ActiveMap.DicMapTrigger.Values.ToArray());
            BattleMapViewer.RefreshScrollbars();

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, BattleMapViewer.Width, BattleMapViewer.Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;

            ActiveMap.fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

            #region Tiles

            for (int T = 0; T < BattleMapViewer.ActiveMap.ListTilesetPreset.Count; T++)
            {
                ItemInfo Item = GetItemByKey(GUIRootPathMapTilesetImages, BattleMapViewer.ActiveMap.ListTilesetPreset[T].TilesetName);

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


            #region Layers

            foreach(object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                lsLayers.Items.Add(ActiveLayer);
            }

            if (lsLayers.Items.Count > 0)
            {
                lsLayers.SelectedIndex = 0;
            }

            #endregion

            #region Props

            foreach (InteractiveProp Instance in BattleMapViewer.ActiveMap.DicInteractiveProp.Values)
            {
                if (Instance.PropCategory == InteractiveProp.PropCategories.Interactive)
                {
                    lsInteractiveProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Physical)
                {
                    lsPhysicalProps.Items.Add(Instance);
                }
                else if (Instance.PropCategory == InteractiveProp.PropCategories.Visual)
                {
                    lsVisualProps.Items.Add(Instance);
                }
            }

            #endregion
        }
    }
}