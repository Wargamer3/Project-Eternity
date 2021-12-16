using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Editors.MusicPlayer;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

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
            BattleMapViewer.ShowGrid = false;
            cbShowGrid = new CheckBox
            {
                Text = "Show grid"
            };
            //Link a CheckedChanged event to a method.
            cbShowGrid.CheckedChanged += new EventHandler(cbShowGrid_CheckedChanged);
            cbShowGrid.Checked = true;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShowGrid.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShowGrid));

            #endregion

            #region cbPreviewMap

            //Init the Preview Map button (as it can't be done with the tool box)
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

            //Init the Preview Map button (as it can't be done with the tool box)
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

            #region Scripting

            lstEvents.Items.Add(new MapEvent(140, 70, "Game", new string[0], new string[] { "Game Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Phase", new string[0], new string[] { "Phase Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Turn", new string[0], new string[] { "Turn Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Unit Moved", new string[0], new string[] { "Unit Moved" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "On Battle", new string[0], new string[] { "Battle Start", "Battle End" }));

            #endregion
        }

        public ProjectEternityMapEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                DeathmatchMap NewMap = new DeathmatchMap(FilePath, string.Empty, null);
                ActiveMap = BattleMapViewer.ActiveMap = NewMap;
                NewMap.ListLayer.Add(new MapLayer(NewMap));
                BattleMapViewer.ActiveMap.ArrayMultiplayerColor = new Color[] { Color.Turquoise, Color.White, Color.SteelBlue, Color.Silver, Color.SandyBrown, Color.Salmon, Color.Purple, Color.PaleGreen, Color.Orange, Color.Gold, Color.ForestGreen, Color.Firebrick, Color.Chartreuse, Color.Beige, Color.DeepPink, Color.DarkMagenta };

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
            DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, string.Empty, new Dictionary<string, List<Core.Units.Squad>>());
            Helper = new DeathmatchMapHelper(NewMap);
            InitMap(NewMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity Deathmatch Map Editor";
        }

        protected void InitMap(BattleMap NewMap)
        {
            ActiveMap = NewMap;
            BattleMapViewer.ActiveMap = NewMap;
            NewMap.ListGameScreen = new List<GameScreens.GameScreen>();
            NewMap.Content = BattleMapViewer.content;
            Helper.InitMap();
            ActiveMap.TogglePreview(true);
            NewMap.CursorPositionVisible = new Vector3(-1, -1, 0);

            BattleMapViewer.SetListMapScript(NewMap.ListMapScript);
            BattleMapViewer.Helper.OnSelect = (SelectedObject, RightClick) => {
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

            btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(BattleMapViewer.ActiveMap.ArrayMultiplayerColor[0].R, BattleMapViewer.ActiveMap.ArrayMultiplayerColor[0].G, BattleMapViewer.ActiveMap.ArrayMultiplayerColor[0].B);
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

        #endregion

        #region Map

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
                        Map_MouseMove(e);
                        break;

                    case 2:
                        BattleMapViewer.Scripting_MouseMove(e);
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
                        pnMapPreview_MouseMove(sender, e);
                        break;

                    case 2:
                        BattleMapViewer.Scripting_MouseUp(e);
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
                                Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, lsLayers.SelectedIndex);
                            }
                        }
                        //Just create a new Tile.
                        else if (BattleMapViewer.ActiveMap.TileSize.X != 0)
                        {
                            Point TilePos = TilesetViewer.ActiveTile;
                            Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
                            DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                            Helper.ReplaceTerrain((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y,
                                PresetTerrain, lsLayers.SelectedIndex);

                            Helper.ReplaceTile((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y,
                                PresetTile, lsLayers.SelectedIndex);
                        }
                    }
                }
                //Spawn tab
                else if (tabToolBox.SelectedIndex == 1)
                {
                    HandleEventPoint(MouseX, MouseY, ActiveSpawn);
                }
            }

            #region Right click

            else if (e.Button == MouseButtons.Right)
            {
                //If there is a map loaded
                if (BattleMapViewer.ActiveMap.TileSize.X != 0)
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
                }
            }

            #endregion
        }

        #endregion

        #region Spawn points

        private void HandleEventPoint(int X, int Y, EventPoint Spawn)
        {//If there is an active Spawn and a map loaded.
            if (BattleMapViewer.ActiveMap.TileSize.X != 0)
            {
                if (btnMapSwitches.Checked)
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
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, 0);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Count; S++)
            {//If it exist.
                if (BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.X == X && BattleMapViewer.ActiveMap.ListSingleplayerSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    BattleMapViewer.ActiveMap.ListSingleplayerSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                BattleMapViewer.ActiveMap.ListSingleplayerSpawns.Add(Spawn);
            }
        }

        private void NewSpawnMultiplayer(int X, int Y, EventPoint Spawn)
        {
            if (Spawn != null)
            {
                Spawn = new EventPoint(Spawn);
                Spawn.Position = new Vector3(X, Y, 0);
            }
            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < BattleMapViewer.ActiveMap.ListMultiplayerSpawns.Count; S++)
            {//If it exist.
                if (BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].Position.X == X && BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    BattleMapViewer.ActiveMap.ListMultiplayerSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        BattleMapViewer.ActiveMap.ListMultiplayerSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                BattleMapViewer.ActiveMap.ListMultiplayerSpawns.Add(Spawn);
            }
        }

        private void NewMapSwitchPoint(int X, int Y, EventPoint Spawn)
        {
            MapSwitchPoint OldEventPoint = null;

            //Loop in the SpawnPoint list to find if a SpawnPoint already exist at the X, Y position.
            for (int S = 0; S < BattleMapViewer.ActiveMap.ListMapSwitchPoint.Count; S++)
            {//If it exist.
                if (BattleMapViewer.ActiveMap.ListMapSwitchPoint[S].Position.X == X && BattleMapViewer.ActiveMap.ListMapSwitchPoint[S].Position.Y == Y)
                {
                    OldEventPoint = BattleMapViewer.ActiveMap.ListMapSwitchPoint[S];
                }
            }

            if (Spawn != null)
            {
                if (OldEventPoint == null)
                {
                    MapSwitchPoint NewMapSwitchPoint = new MapSwitchPoint(Spawn);
                    NewMapSwitchPoint.Position = new Vector3(X, Y, 0);
                    BattleMapViewer.ActiveMap.ListMapSwitchPoint.Add(NewMapSwitchPoint);
                    pgEventPoints.SelectedObject = NewMapSwitchPoint;
                }
                else
                {
                    pgEventPoints.SelectedObject = OldEventPoint;
                }
            }
            else if (OldEventPoint != null)
            {
                BattleMapViewer.ActiveMap.ListMapSwitchPoint.Remove(OldEventPoint);
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
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                cbDeadthmatch.DroppedDown = true;
        }

        private void btnSpawnDM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
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
                    BattleMapViewer.ActiveMap.ArrayMultiplayerColor[cbDeadthmatch.SelectedIndex] = Color.FromNonPremultiplied(CD.Color.R, CD.Color.G, CD.Color.B, 255);
                    if (btnSpawnDM.Checked)
                    {
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.R;
                        ActiveSpawn.ColorGreen = btnSpawnDM.BackColor.G;
                        ActiveSpawn.ColorRed = btnSpawnDM.BackColor.B;
                    }

                    for (int S = 0; S < BattleMapViewer.ActiveMap.ListMultiplayerSpawns.Count; S++)
                    {
                        if (BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].Tag == btnSpawnDM.Text)
                        {
                            BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].ColorRed = CD.Color.R;
                            BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].ColorGreen = CD.Color.G;
                            BattleMapViewer.ActiveMap.ListMultiplayerSpawns[S].ColorBlue = CD.Color.B;
                        }
                    }
                }
            }
        }

        //A new team is selected.
        private void cbDeadthmatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSpawnDM.Text = cbDeadthmatch.Text;//Give the button the selected text.
            btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(BattleMapViewer.ActiveMap.ArrayMultiplayerColor[cbDeadthmatch.SelectedIndex].R,
                                                                 BattleMapViewer.ActiveMap.ArrayMultiplayerColor[cbDeadthmatch.SelectedIndex].G,
                                                                 BattleMapViewer.ActiveMap.ArrayMultiplayerColor[cbDeadthmatch.SelectedIndex].B);//Give the button the selected color.
            btnSpawnDM.Checked = true;//Press the button.
            //Update the ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnDM.Text, btnSpawnDM.BackColor.R, btnSpawnDM.BackColor.G, btnSpawnDM.BackColor.B);
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
            lsLayers.Items.Add(Helper.CreateNewLayer());
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
                    if (lsLayers.Items[L] is IMapLayer)
                    {
                        lsLayers.Items.Insert(L + 1, Helper.CreateNewSubLayer((IMapLayer)lsLayers.Items[L]));
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
                    if (lsLayers.Items[L] is IMapLayer)
                    {
                        Helper.RemoveSubLayer((IMapLayer)lsLayers.Items[L], (ISubMapLayer)lsLayers.Items[lsLayers.SelectedIndex]);
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
                    ActiveMap.ShowLayerIndex = lsLayers.SelectedIndex;
                }
            }
        }

        #endregion

        private void cbShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ShowGrid = cbShowGrid.Checked;
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
                    ActiveMap.ShowLayerIndex = lsLayers.SelectedIndex;
                }
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
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y,
                                       0, 0, 1, new TerrainActivation[0], new TerrainBonus[0], new int[0]),
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

        private void mapPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapStatistics MS = new MapStatistics(ActiveMap.MapName, ActiveMap.MapSize, ActiveMap.TileSize, ActiveMap.CameraPosition, ActiveMap.PlayersMin, ActiveMap.PlayersMax, ActiveMap.Description);
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

            #region Scripting

            lstEvents.Items.Add(new MapEvent(140, 70, "Game", new string[0], new string[] { "Game Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Phase", new string[0], new string[] { "Phase Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Turn", new string[0], new string[] { "Turn Start" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "Unit Moved", new string[0], new string[] { "Unit Moved" }));
            lstEvents.Items.Add(new MapEvent(140, 70, "On Battle", new string[0], new string[] { "Battle Start", "Battle End" }));

            string[] Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MapCondition> ListMapCondition = ReflectionHelper.GetObjectsFromBaseTypes<MapCondition>(typeof(BattleCondition), ActiveAssembly.GetTypes());

                foreach (MapCondition Instance in ListMapCondition)
                {
                    lstConditions.Items.Add(Instance);
                }
            }

            Files = Directory.GetFiles("Scripts", "*.dll");
            for (int F = 0; F < Files.Length; F++)
            {
                System.Reflection.Assembly ActiveAssembly = System.Reflection.Assembly.LoadFile(Path.GetFullPath(Files[F]));
                List<MapTrigger> ListMapTrigger = ReflectionHelper.GetObjectsFromBaseTypes<MapTrigger>(typeof(BattleTrigger), ActiveAssembly.GetTypes());

                foreach (MapTrigger Instance in ListMapTrigger)
                {
                    lstTriggers.Items.Add(Instance);
                }
            }

            #endregion

            #region Layers

            foreach(object ActiveLayer in Helper.GetLayers())
            {
                lsLayers.Items.Add(ActiveLayer);
            }

            if (lsLayers.Items.Count > 0)
            {
                lsLayers.SelectedIndex = 0;
            }

            #endregion
        }
    }
}