using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
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

        private CheckBox cbShowGrid;
        private CheckBox cbPreviewMap;
        private CheckBox cbShowAllLayers;
        private CheckBox cbShowTerrainType;
        private CheckBox cbShowTerrainHeight;
        private CheckBox cbShow3DObjects;

        protected BattleMap ActiveMap => BattleMapViewer.ActiveMap;
        protected IMapHelper Helper;

        //Spawn point related stuff.
        private EventPoint ActiveSpawn;
        private System.Drawing.Point LastMousePosition;

        private bool AllowEvents = true;

        protected ITileAttributes TileAttributesEditor;

        private static DeathmatchParams Params;
        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(Keys key);

        public ProjectEternityMapEditor()
        {
            InitializeComponent();
            KeyPreview = true;
            if (Params == null)
            {
                Params = new DeathmatchParams(new BattleContext());
            }

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

            #region cbShow3DObjects

            cbShow3DObjects = new CheckBox
            {
                Text = "Show 3D Objects"
            };
            //Link a CheckedChanged event to a method.
            cbShow3DObjects.CheckedChanged += new EventHandler(cbShow3DObjects_CheckedChanged);
            cbShow3DObjects.Checked = false;
            //Make it 10 pixel after the last mnuToolBar item.
            cbShow3DObjects.Padding = new Padding(10, 0, 0, 0);
            mnuToolBar.Items.Add(new ToolStripControlHost(cbShow3DObjects));

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
                DeathmatchMap NewMap = new DeathmatchMap(FilePath, new GameModeInfo(), ProjectEternityMapEditor.Params);
                BattleMapViewer.ActiveMap = NewMap;
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
                new EditorInfo(new string[] { GUIRootPathMapBGM }, "Maps/BGM/", new string[] { ".mp3" }, typeof(ProjectEternityMusicPlayerEditor), false),
                new EditorInfo(new string[] { GUIRootPathMapModels }, "Maps/Models/", new string[] { ".xnb" }, typeof(ProjectEternityMusicPlayerEditor), false, null, true),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            ActiveMap.MapName = ItemName;

            ActiveMap.Save(ItemPath);
        }

        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(24);

            BattleMapViewer.Preload();
            DeathmatchMap NewMap = new DeathmatchMap(MapLogicName, new GameModeInfo(), Params);
            Helper = new DeathmatchMapHelper(NewMap);
            InitMap(NewMap);

            this.Text = ActiveMap.MapName + " - Project Eternity Deathmatch Map Editor";
        }

        protected void InitMap(BattleMap NewMap)
        {
            BattleMapViewer.ActiveMap = NewMap;
            ActiveMap.IsEditor = true;
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

            for (int S = NewMap.ListMapScript.Count - 1; S >= 0; --S)
            {
                BattleMapViewer.Helper.InitScript(NewMap.ListMapScript[S]);
            }

            if (NewMap.ListMultiplayerColor.Count > 0)
            {
                for (int C = 0; C < NewMap.ListMultiplayerColor.Count; C++)
                {
                    cbDeadthmatch.Items.Add(C + 1);
                }

                btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(NewMap.ListMultiplayerColor[0].R, NewMap.ListMultiplayerColor[0].G, NewMap.ListMultiplayerColor[0].B);
            }
        }

        private void DrawInfo()
        {//Draw the mouse position minus the map starting point.
            tslInformation.Text = string.Format("X: {0}, Y: {1}, Z: {2}", ActiveMap.CursorPosition.X, ActiveMap.CursorPosition.Y, ActiveMap.CursorPosition.Z);

            if (ActiveMap.CursorPosition.X < 0 || ActiveMap.CursorPosition.X >= ActiveMap.MapSize.X || ActiveMap.CursorPosition.Y < 0 || ActiveMap.CursorPosition.Y >= ActiveMap.MapSize.Y || cboTiles.Items.Count == 0)
                return;

            Terrain SelectedTerrain = Helper.GetTerrain((int)ActiveMap.CursorPosition.X, (int)ActiveMap.CursorPosition.Y, (int)ActiveMap.CursorPosition.Z);

            tslInformation.Text += " " + TileAttributesEditor.GetTerrainName(SelectedTerrain.TerrainTypeIndex);

            if (tabToolBox.SelectedIndex == 0)
            {
                //Add the selection informations.
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    tslInformation.Text += " Tile attribues";
                else
                {
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                        tslInformation.Text += " Rectangle";
                    else
                        tslInformation.Text += " Hold shift to place tiles in a rectangle";
                    tslInformation.Text += ", hold ctrl to change the selected tile attributes";
                }
            }
            else if (tabToolBox.SelectedIndex == 1)
            {
                tslInformation.Text += " Left click to place a new spawn point";
                tslInformation.Text += " Right click to remove a spawn point";
            }

            tslInformation.Text += ", Arrow keys, Q, E and Shift to move and rotate the camera";
        }

        #region Tile set change

        //Change the ActiveTile to the mouse position.
        private void TileViewer_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0)
            {
                int BrushIndex = 0;
                if (((MouseEventArgs)e).Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                Point DrawOffset = TilesetViewer.DrawOffset;//Used to avoid warnings.
                //Set the ActiveTile to the mouse position.
                TilesetViewer.SelectTile(new Point(((((MouseEventArgs)e).X + DrawOffset.X) / ActiveMap.TileSize.X) * ActiveMap.TileSize.X,
                                                     ((((MouseEventArgs)e).Y + DrawOffset.Y) / ActiveMap.TileSize.Y) * ActiveMap.TileSize.Y),
                                                     Control.ModifierKeys == Keys.Shift, BrushIndex);
            }
        }

        private void cboTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Initialise the scroll bars.
            if (ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width >= TilesetViewer.Width)
            {
                sclTileWidth.Maximum = ActiveMap.ListTileSet[cboTiles.SelectedIndex].Width - TilesetViewer.Width;
                sclTileWidth.Visible = true;
            }
            else
                sclTileWidth.Visible = false;
            if (ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height >= TilesetViewer.Height)
            {
                sclTileHeight.Maximum = ActiveMap.ListTileSet[cboTiles.SelectedIndex].Height - TilesetViewer.Height;
                sclTileHeight.Visible = true;
            }
            else
                sclTileHeight.Visible = false;

            TilesetViewer.InitTileset(ActiveMap.ListTileSet[cboTiles.SelectedIndex], ActiveMap.TileSize);
        }

        private void btnAddTile_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Tile;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapTilesets));
        }

        private void btnAddNewTileSetAsBackground_Click(object sender, EventArgs e)
        {//If there is a map loaded(and so ActiveMap.TileSize.X is not 0).
            if (ActiveMap.TileSize.X != 0)
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
                Rectangle TilePos = TilesetViewer.ListTileBrush[0];
                Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                {
                    ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y] = TileAttributesEditor.ActiveTerrain;
                }
            }
        }

        private void btn3DTileAttributes_Click(object sender, EventArgs e)
        {
            if (ActiveMap.ListTilesetPreset.Count <= 0)
            {
                return;
            }

            Rectangle TilePos = TilesetViewer.ListTileBrush[0];
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
            if (ActiveMap.CameraType != "3D" || !cbPreviewMap.Checked)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            bool KeyProcessed = false;

            bool IsMovingLeft = keyData == Keys.Left;
            bool IsMovingRight = keyData == Keys.Right;
            bool IsMovingUp = keyData == Keys.Up;
            bool IsMovingDown = keyData == Keys.Down;

            float PlatformAngle = ActiveMap.Camera3DYawAngle;

            if (PlatformAngle >= 315 || PlatformAngle < 45)
            {
                //do nothing
            }
            else if (PlatformAngle >= 45 && PlatformAngle < 135)
            {
                IsMovingLeft = keyData == Keys.Up;
                IsMovingRight = keyData == Keys.Down;
                IsMovingUp = keyData == Keys.Right;
                IsMovingDown = keyData == Keys.Left;
            }
            else if (PlatformAngle >= 135 && PlatformAngle < 225)
            {
                IsMovingLeft = keyData == Keys.Right;
                IsMovingRight = keyData == Keys.Left;
                IsMovingUp = keyData == Keys.Down;
                IsMovingDown = keyData == Keys.Up;
            }
            else if (PlatformAngle >= 225 && PlatformAngle < 315)
            {
                IsMovingLeft = keyData == Keys.Down;
                IsMovingRight = keyData == Keys.Up;
                IsMovingUp = keyData == Keys.Left;
                IsMovingDown = keyData == Keys.Right;
            }

            if (keyData == (Keys.Left | Keys.Shift))
            {
                ActiveMap.Camera3DYawAngle -= 90;
                if (ActiveMap.Camera3DYawAngle < 0)
                {
                    ActiveMap.Camera3DYawAngle += 360;
                }
                KeyProcessed = true;
            }
            else if (keyData == (Keys.Right | Keys.Shift))
            {
                ActiveMap.Camera3DYawAngle += 90;
                if (ActiveMap.Camera3DYawAngle > 360)
                {
                    ActiveMap.Camera3DYawAngle -= 360;
                }
                KeyProcessed = true;
            }
            else if (keyData == (Keys.Up | Keys.Shift) || keyData == (Keys.Down | Keys.Shift))
            {
                if (ActiveMap.Camera3DPitchAngle == 45)
                {
                    ActiveMap.Camera3DPitchAngle = 1;
                }
                else
                {
                    ActiveMap.Camera3DPitchAngle = 45;
                }
                KeyProcessed = true;
            }
            else if (IsMovingLeft)
            {
                ActiveMap.CursorPosition.X -= (ActiveMap.CursorPosition.X > 0) ? 1 : 0;
                KeyProcessed = true;
            }
            else if (IsMovingRight)
            {
                ActiveMap.CursorPosition.X += (ActiveMap.CursorPosition.X < ActiveMap.MapSize.X - 1) ? 1 : 0;
                KeyProcessed = true;
            }

            if (IsMovingUp)
            {
                ActiveMap.CursorPosition.Y -= (ActiveMap.CursorPosition.Y > 0) ? 1 : 0;
                KeyProcessed = true;
            }
            else if (IsMovingDown)
            {
                ActiveMap.CursorPosition.Y += (ActiveMap.CursorPosition.Y < ActiveMap.MapSize.Y - 1) ? 1 : 0;
                KeyProcessed = true;
            }

            if (keyData == Keys.Q)
            {
                float NextZ = ActiveMap.CursorPosition.Z + 1;

                if (NextZ >= lsLayers.Items.Count)
                {
                    NextZ = lsLayers.Items.Count - 1;
                }

                ActiveMap.CursorPosition.Z = NextZ;
                SetLayerIndex();
                KeyProcessed = true;
            }
            else if (keyData == Keys.E)
            {
                float NextZ = ActiveMap.CursorPosition.Z - 1;

                if (NextZ < 0)
                {
                    NextZ = 0;
                }

                ActiveMap.CursorPosition.Z = NextZ;
                SetLayerIndex();
                KeyProcessed = true;
            }

            else if (keyData == Keys.C)
            {
                cbShow3DObjects.Checked = cbShow3DObjects.Checked;
            }

            if ((GetAsyncKeyState(Keys.X) & 0x8000) > 0)
            {
                PlaceTile((int)ActiveMap.CursorPosition.X, (int)ActiveMap.CursorPosition.Y, (int)ActiveMap.CursorPosition.Z, false, 0);
                KeyProcessed = true;
            }

            if (!KeyProcessed)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            DrawInfo();
            return true;
        }

        protected virtual void pnMapPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if (ActiveMap.CameraType == "3D" && cbPreviewMap.Checked)
            {
                if (e.Button == MouseButtons.Right)
                {
                    float Rotation = LastMousePosition.X - e.X;
                    ActiveMap.Camera3DYawAngle += Rotation;
                    if (ActiveMap.Camera3DYawAngle > 360)
                    {
                        ActiveMap.Camera3DYawAngle -= 360;
                    }
                    else if (ActiveMap.Camera3DYawAngle < 0)
                    {
                        ActiveMap.Camera3DYawAngle += 360;
                    }
                }

                LastMousePosition = e.Location;

                return;
            }

            Vector3 MapPreviewStartingPos = new Vector3(
                ActiveMap.Camera2DPosition.X * ActiveMap.TileSize.X,
                ActiveMap.Camera2DPosition.Y * ActiveMap.TileSize.Y,
                ActiveMap.Camera2DPosition.Z);

            ActiveMap.CursorPosition.X = (int)(e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X;
            ActiveMap.CursorPosition.Y = (int)(e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y;

            DrawInfo();

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
                        Rectangle TileReplacementZone = BattleMapViewer.TileReplacementZone;

                        if (TileReplacementZone.Width > 0 && ActiveMap.TileSize.X != 0)
                        {
                            int BrushIndex = 0;
                            if (e.Button == MouseButtons.Right)
                            {
                                BrushIndex = 1;
                            }

                            Vector3 MapPreviewStartingPos = new Vector3(
                                ActiveMap.Camera2DPosition.X * ActiveMap.TileSize.X,
                                ActiveMap.Camera2DPosition.Y * ActiveMap.TileSize.Y,
                                ActiveMap.Camera2DPosition.Z);

                            for (int X = TileReplacementZone.X; X < TileReplacementZone.Right; ++X)
                            {
                                for (int Y = TileReplacementZone.Y; Y < TileReplacementZone.Bottom; ++Y)
                                {
                                    PlaceTile(X + (int)(MapPreviewStartingPos.X) / ActiveMap.TileSize.X, Y + (int)(MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y, lsLayers.SelectedIndex, true, BrushIndex);
                                }
                            }
                        }
                        else
                        {
                            pnMapPreview_MouseMove(sender, e);
                        }

                        BattleMapViewer.TileReplacementZone = new Rectangle();
                        break;

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
            LastMousePosition = e.Location;

            switch (tabToolBox.SelectedIndex)
            {
                case 0:

                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        Vector3 MapPreviewStartingPos = new Vector3(
                            ActiveMap.Camera2DPosition.X * ActiveMap.TileSize.X,
                            ActiveMap.Camera2DPosition.Y * ActiveMap.TileSize.Y,
                            ActiveMap.Camera2DPosition.Z);

                        int MouseX = (int)(e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X;
                        int MouseY = (int)(e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y;

                        BattleMapViewer.TileReplacementZone = new Rectangle(MouseX, MouseY, 1, 1);
                    }
                    break;

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
                ActiveMap.Camera2DPosition.X * ActiveMap.TileSize.X,
                ActiveMap.Camera2DPosition.Y * ActiveMap.TileSize.Y,
                ActiveMap.Camera2DPosition.Z);

            int MouseX = (int)(e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X;
            int MouseY = (int)(e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y;

            if (MouseX < 0 || MouseX >= ActiveMap.MapSize.X || MouseY < 0 || MouseY >= ActiveMap.MapSize.Y || cboTiles.Items.Count == 0)
                return;

            //Tile tab
            if (tabToolBox.SelectedIndex == 0)
            {
                int BrushIndex = 0;
                if (e.Button == MouseButtons.Right)
                {
                    BrushIndex = 1;
                }

                Rectangle TileReplacementZone = BattleMapViewer.TileReplacementZone;
                if (TileReplacementZone.Width > 0)
                {
                    if (MouseX > TileReplacementZone.X)
                    {
                        TileReplacementZone.Width = MouseX - TileReplacementZone.X + 1;
                    }
                    else if (MouseX < TileReplacementZone.X)
                    {
                        int Right = TileReplacementZone.Right;
                        TileReplacementZone.X = MouseX;
                        TileReplacementZone.Width = Right - MouseX;
                    }
                    if (MouseY > TileReplacementZone.Y)
                    {
                        TileReplacementZone.Height = MouseY - TileReplacementZone.Y + 1;
                    }
                    else if (MouseY < TileReplacementZone.Y)
                    {
                        int Bottom = TileReplacementZone.Bottom;
                        TileReplacementZone.Y = MouseY;
                        TileReplacementZone.Height = Bottom - MouseY;
                    }

                    BattleMapViewer.TileReplacementZone = TileReplacementZone;
                }
                else if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        Point TilePos = new Point(MouseX, MouseY);
                        Terrain SelectedTerrain = Helper.GetTerrain(TilePos.X, TilePos.Y, lsLayers.SelectedIndex);

                        TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                        if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                        {
                            Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, lsLayers.SelectedIndex, true);
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
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
                }
                //Just create a new Tile.
                else if (ActiveMap.TileSize.X != 0)
                {
                    PlaceTile((int)(e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y, lsLayers.SelectedIndex, true, BrushIndex);
                }
            }
            //Spawn tab
            else if (tabToolBox.SelectedIndex == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    HandleEventPoint(MouseX, MouseY, ActiveSpawn);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    HandleEventPoint(MouseX, MouseY, null);
                }
            }
            //Spawn tab
            else if (tabToolBox.SelectedIndex == 4)
            {
                if (e.Button == MouseButtons.Left)
                {
                    HandleProps(MouseX, MouseY);
                }
                else if (e.Button == MouseButtons.Right)
                {
                    RemoveProps(MouseX, MouseY);
                }
            }
        }

        private void PlaceTile(int X, int Y, int LayerIndex, bool ConsiderSubLayers, int BrushIndex)
        {
            if (X < 0 || X >= ActiveMap.MapSize.X
                || Y < 0 || Y >= ActiveMap.MapSize.Y)
            {
                return;
            }

            Point TilePos = TilesetViewer.GetTileFromBrush(new Point(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y), BrushIndex);

            if (TilePos.X < 0 || TilePos.X >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(0) * ActiveMap.TileSize.X
                || TilePos.Y < 0 || TilePos.Y >= ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain.GetLength(1) * ActiveMap.TileSize.Y)
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
            if (ActiveMap.TileSize.X != 0)
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
                else if (btnSpawnPlayer.Checked || btnSpawnAlly.Checked || btnSpawnEnemy.Checked || btnSpawnNeutral.Checked)
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
            for (int S = 0; S < TopLayer.ListCampaignSpawns.Count; S++)
            {//If it exist.
                if (TopLayer.ListCampaignSpawns[S].Position.X == X && TopLayer.ListCampaignSpawns[S].Position.Y == Y)
                {
                    //Delete it.
                    TopLayer.ListCampaignSpawns.RemoveAt(S);
                    if (Spawn != null)
                    {
                        //Add the new one.
                        TopLayer.ListCampaignSpawns.Add(Spawn);
                    }
                    return;
                }
            }
            if (Spawn != null)
            {
                //Add the new SpawnPoint.
                TopLayer.ListCampaignSpawns.Add(Spawn);
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
                if (TopLayer.ListMultiplayerSpawns[S].Position.X == X && TopLayer.ListMultiplayerSpawns[S].Position.Y == Y && (Spawn == null || TopLayer.ListMultiplayerSpawns[S].Tag == Spawn.Tag))
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
                    ActiveMap.ListMultiplayerColor[MPColorIndex] = Color.FromNonPremultiplied(CD.Color.R, CD.Color.G, CD.Color.B, 255);
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
            btnSpawnDM.BackColor = System.Drawing.Color.FromArgb(ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].R,
                                                                 ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].G,
                                                                 ActiveMap.ListMultiplayerColor[cbDeadthmatch.SelectedIndex].B);//Give the button the selected color.
            btnSpawnDM.Checked = true;//Press the button.
            //Update the ActiveSpawn.
            ActiveSpawn = new EventPoint(Vector3.Zero, btnSpawnDM.Text, btnSpawnDM.BackColor.R, btnSpawnDM.BackColor.G, btnSpawnDM.BackColor.B);
        }

        private void btnAddDeathmatchTeam_Click(object sender, EventArgs e)
        {
            Color[] ArrayColorChoices = new Color[] { Color.Turquoise, Color.White, Color.SteelBlue, Color.Silver, Color.SandyBrown, Color.Salmon, Color.Purple, Color.PaleGreen, Color.Orange, Color.Gold, Color.ForestGreen, Color.Firebrick, Color.Chartreuse, Color.Beige, Color.DeepPink, Color.DarkMagenta };
            ActiveMap.ListMultiplayerColor.Add(ArrayColorChoices[Math.Min(ArrayColorChoices.Length - 1, ActiveMap.ListMultiplayerColor.Count)]);
            cbDeadthmatch.Items.Add(ActiveMap.ListMultiplayerColor.Count);
        }

        private void btnRemoveDeathmatchTeam_Click(object sender, EventArgs e)
        {
            if (cbDeadthmatch.SelectedIndex >= 0)
            {
                ActiveMap.ListMultiplayerColor.RemoveAt(cbDeadthmatch.SelectedIndex);
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
            Rectangle TilePos = TilesetViewer.ListTileBrush[0];
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
                lsLayers.SelectedIndex = lsLayers.Items.Count - 1;
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
            if (!AllowEvents)
            {
                return;
            }

            if (lsLayers.SelectedIndex >= 0)
            {
                if (cbShowAllLayers.Checked)
                {
                    ActiveMap.ShowLayerIndex = -1;
                }
                else
                {
                    ActiveMap.ShowLayerIndex = GetRealTopLayerIndex(lsLayers.SelectedIndex);
                    ActiveMap.CursorPosition.Z = ActiveMap.ShowLayerIndex;
                }
            }
        }

        #endregion

        #region Props

        private void HandleProps(int X, int Y)
        {
            if (ActiveMap.TileSize.X != 0)
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
            if (ActiveMap.TileSize.X != 0)
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
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneOval_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Oval);
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnAddZoneFullMap_Click(object sender, EventArgs e)
        {
            MapZone NewZone = Helper.CreateNewZone(ZoneShape.ZoneShapeTypes.Full);
            ActiveMap.MapEnvironment.ListMapZone.Add(NewZone);
            lsZones.Items.Add("Zone");
            pgZoneProperties.SelectedObject = NewZone;
            lsZones.SelectedIndex = lsZones.Items.Count - 1;
        }

        private void btnRemoveZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                ActiveMap.MapEnvironment.ListMapZone.RemoveAt(lsZones.SelectedIndex);
                lsZones.Items.RemoveAt(lsZones.SelectedIndex);
            }
        }

        private void btnEditZone_Click(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                new ZoneEditor(ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex]).ShowDialog();
            }
        }

        private void lsZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsZones.SelectedIndex >= 0)
            {
                pgZoneProperties.SelectedObject = ActiveMap.MapEnvironment.ListMapZone[lsZones.SelectedIndex];
            }
        }

        #endregion

        private void cbShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMap.ShowGrid = cbShowGrid.Checked;
        }

        private void cbShowTerrainType_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMap.ShowTerrainType = cbShowTerrainType.Checked;
        }

        private void cbShowTerrainHeight_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMap.ShowTerrainHeight = cbShowTerrainHeight.Checked;
        }

        private void cbShow3DObjects_CheckedChanged(object sender, EventArgs e)
        {
            ActiveMap.Show3DObjects = cbShow3DObjects.Checked;
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

        private void SetLayerIndex()
        {
            AllowEvents = false;
            int LastTopLayerIndex = -1;

            foreach (object ActiveLayer in Helper.GetLayersAndSubLayers())
            {
                if (LastTopLayerIndex == (int)ActiveMap.CursorPosition.Z)
                {
                    lsLayers.SelectedIndex = LastTopLayerIndex;
                    return;
                }

                if (!(ActiveLayer is ISubMapLayer))
                {
                    ++LastTopLayerIndex;
                }
            }

            lsLayers.SelectedIndex = LastTopLayerIndex;
            AllowEvents = true;
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
                                if (cboTiles.Items.Contains(Name))
                                {
                                    MessageBox.Show("This tile is already listed.\r\n" + Name);
                                    continue;
                                }

                                Terrain.TilesetPreset NewTileset = Terrain.TilesetPreset.FromFile(Name, ActiveMap.ListTilesetPreset.Count);
                                for (int BackgroundIndex = 0; BackgroundIndex < NewTileset.ListBattleBackgroundAnimationPath.Count; BackgroundIndex++)
                                {
                                    string NewBattleBackgroundPath = NewTileset.ListBattleBackgroundAnimationPath[BackgroundIndex];

                                    if (ActiveMap.ListBattleBackgroundAnimationPath.Contains(NewBattleBackgroundPath))
                                    {
                                        byte MapBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.IndexOf(NewBattleBackgroundPath);

                                        for (int X = 0; X < NewTileset.ArrayTerrain.GetLength(0); ++X)
                                        {
                                            for (int Y = 0; Y < NewTileset.ArrayTerrain.GetLength(1); ++Y)
                                            {
                                                if (NewTileset.ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = MapBackgroundIndex;
                                                }
                                                if (NewTileset.ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BattleForegroundAnimationIndex = MapBackgroundIndex;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        byte NewBattleBackgroundIndex = (byte)ActiveMap.ListBattleBackgroundAnimationPath.Count;
                                        ActiveMap.ListBattleBackgroundAnimationPath.Add(NewBattleBackgroundPath);

                                        for (int X = 0; X < NewTileset.ArrayTerrain.GetLength(0); ++X)
                                        {
                                            for (int Y = 0; Y < NewTileset.ArrayTerrain.GetLength(1); ++Y)
                                            {
                                                if (NewTileset.ArrayTerrain[X, Y].BattleBackgroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BattleBackgroundAnimationIndex = NewBattleBackgroundIndex;
                                                }
                                                if (NewTileset.ArrayTerrain[X, Y].BattleForegroundAnimationIndex == BackgroundIndex)
                                                {
                                                    NewTileset.ArrayTerrain[X, Y].BattleForegroundAnimationIndex = NewBattleBackgroundIndex;
                                                }
                                            }
                                        }
                                    }
                                }

                                ActiveMap.ListTilesetPreset.Add(NewTileset);
                                ActiveMap.ListTileSet.Add(TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName));

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
                                Microsoft.Xna.Framework.Graphics.Texture2D Tile = TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + Name);

                                ActiveMap.ListTilesetPreset.Add(new Terrain.TilesetPreset(Name, Tile.Width, Tile.Height, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, ActiveMap.ListTilesetPreset.Count));
                                ActiveMap.ListTileSet.Add(Tile);
                                //Add the file name to the tile combo box.
                                cboTiles.Items.Add(Name);
                            }

                            cboTiles.SelectedIndex = ActiveMap.ListTilesetPreset.Count - 1;

                            if (ActiveMap.ListTileSet.Count == 1)
                            {
                                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[0].ArrayTerrain[0, 0];
                                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[0].ArrayTiles[0, 0];

                                //Asign a new tile at the every position, based on its atribtues.
                                for (int X = ActiveMap.MapSize.X - 1; X >= 0; --X)
                                {
                                    for (int Y = ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                    {
                                        Helper.ReplaceTerrain(X, Y, PresetTerrain, 0, true);
                                        Helper.ReplaceTile(X, Y, PresetTile, 0, true);
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

                            ActiveMap.ListTileSet.Add(TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + TileName));
                            //Add the file name to the tile combo box.
                            cboTiles.Items.Add(TileName);
                            cboTiles.SelectedIndex = ActiveMap.ListTileSet.Count - 1;

                            //Initialise the scroll bars.
                            if (ActiveMap.ListTileSet.Last().Width >= TilesetViewer.Width)
                            {
                                sclTileWidth.Maximum = ActiveMap.ListTileSet.Last().Width - TilesetViewer.Width - 1;
                                sclTileWidth.Visible = true;
                            }
                            else
                                sclTileWidth.Visible = false;
                            if (ActiveMap.ListTileSet.Last().Height >= TilesetViewer.Height)
                            {
                                sclTileHeight.Maximum = ActiveMap.ListTileSet.Last().Height - TilesetViewer.Height - 1;
                                sclTileHeight.Visible = true;
                            }
                            else
                                sclTileHeight.Visible = false;

                            //Asign a new tile at the every position, based on its atribtues.
                            for (int X = ActiveMap.MapSize.X - 1; X >= 0; --X)
                            {
                                for (int Y = ActiveMap.MapSize.Y - 1; Y >= 0; --Y)
                                {
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y, lsLayers.SelectedIndex, 0,
                                       0, new TerrainActivation[0], new TerrainBonus[0], new int[0]),
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

        protected virtual MapStatistics OpenMapProperties()
        {
            return new MapStatistics(ActiveMap);
        }

        protected virtual void ApplyMapPropertiesChanges(MapStatistics MS)
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

            ActiveMap.MapName = MS.txtMapName.Text;
            ActiveMap.CameraType = MS.cbCameraType.Text;
            ActiveMap.Camera2DPosition = CameraPosition;
            ActiveMap.OrderNumber = (uint)MS.txtOrderNumber.Value;
            ActiveMap.PlayersMin = (byte)MS.frmDefaultGameModesConditions.txtPlayersMin.Value;
            ActiveMap.PlayersMax = (byte)MS.frmDefaultGameModesConditions.txtPlayersMax.Value;
            ActiveMap.MaxSquadsPerPlayer = (byte)MS.frmDefaultGameModesConditions.txtMaxSquadsPerPlayer.Value;
            ActiveMap.Description = MS.txtDescription.Text;

            ActiveMap.ListMandatoryMutator.Clear();
            foreach (DataGridViewRow ActiveRow in MS.frmDefaultGameModesConditions.dgvMandatoryMutators.Rows)
            {
                if (ActiveRow.Cells[0].Value != null)
                {
                    ActiveMap.ListMandatoryMutator.Add((string)ActiveRow.Cells[0].Value);
                }
            }

            ActiveMap.ListGameType.Clear();
            foreach (GameModeInfo ActiveRow in MS.frmDefaultGameModesConditions.lstGameModes.Items)
            {
                ActiveMap.ListGameType.Add(ActiveRow);
            }

            ActiveMap.MapEnvironment.TimeStart = (float)MS.txtTimeStart.Value;
            ActiveMap.MapEnvironment.HoursInDay = (float)MS.txtHoursInDay.Value;

            if (MS.rbLoopFirstDay.Checked)
            {
                ActiveMap.MapEnvironment.TimeLoopType = EnvironmentManager.TimeLoopTypes.FirstDay;
            }
            else if (MS.rbLoopLastDay.Checked)
            {
                ActiveMap.MapEnvironment.TimeLoopType = EnvironmentManager.TimeLoopTypes.LastDay;
            }
            else
            {
                ActiveMap.MapEnvironment.TimeLoopType = EnvironmentManager.TimeLoopTypes.Stop;
            }

            ActiveMap.MapEnvironment.TimeMultiplier = (float)MS.txtlblTimeMultiplier.Value;
            if (MS.rbUseTurns.Checked)
            {
                ActiveMap.MapEnvironment.TimePeriodType = EnvironmentManager.TimePeriods.Turns;
            }
            else
            {
                ActiveMap.MapEnvironment.TimePeriodType = EnvironmentManager.TimePeriods.RealTime;
            }

            Rectangle TilePos = TilesetViewer.ListTileBrush[0];
            if (cboTiles.SelectedIndex >= 0)
            {
                Terrain PresetTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];
                DrawableTile PresetTile = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTiles[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

                Helper.ResizeTerrain(MapSize.X, MapSize.Y, PresetTerrain, PresetTile);
            }

            BattleMapViewer.RefreshScrollbars();

            ActiveMap.ListBackgroundsPath.Clear();
            ActiveMap.ListBackgroundsPath.AddRange(MS.ListBackgroundsPath);
            ActiveMap.ListForegroundsPath.Clear();
            ActiveMap.ListForegroundsPath.AddRange(MS.ListForegroundsPath);

            ActiveMap.TogglePreview(cbPreviewMap.Checked);
        }

        private void tsmMapProperties_Click(object sender, EventArgs e)
        {
            MapStatistics MS = OpenMapProperties();

            if (MS.ShowDialog() == DialogResult.OK)
            {
                ApplyMapPropertiesChanges(MS);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void tsmGlobalEnvironment_Click(object sender, EventArgs e)
        {
            ZoneEditor GlobalZoneEditor = new ZoneEditor(ActiveMap.MapEnvironment.GlobalZone);

            if (GlobalZoneEditor.ShowDialog() == DialogResult.OK)
            {
                ActiveMap.MapEnvironment.GlobalZone = GlobalZoneEditor.ZoneToEdit;
            }
        }

        private void tabToolBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BattleMapViewer.ViewerIndex = tabToolBox.SelectedIndex;
        }

        private void ProjectEternityMapEditor_Shown(object sender, EventArgs e)
        {
            if (BattleMapViewer.ActiveMap == null)
                return;

            lstEvents.Items.AddRange(ActiveMap.DicMapEvent.Values.ToArray());
            lstConditions.Items.AddRange(ActiveMap.DicMapCondition.Values.ToArray());
            lstTriggers.Items.AddRange(ActiveMap.DicMapTrigger.Values.ToArray());
            BattleMapViewer.RefreshScrollbars();

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, BattleMapViewer.Width, BattleMapViewer.Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;

            ActiveMap.fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

            #region Tiles

            for (int T = 0; T < ActiveMap.ListTilesetPreset.Count; T++)
            {
                ItemInfo Item = GetItemByKey(GUIRootPathMapTilesetImages, ActiveMap.ListTilesetPreset[T].TilesetName);

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
                    MessageBox.Show(ActiveMap.ListTilesetPreset[T].TilesetName + " not found, loading default tileset instead.");
                    cboTiles.Items.Add("Default");
                }
            }

            if (ActiveMap.ListTilesetPreset.Count > 0)
            {
                cboTiles.SelectedIndex = 0;
            }

            if (cboTiles.SelectedIndex >= 0)
            {
                TilesetViewer.InitTileset(ActiveMap.ListTileSet[cboTiles.SelectedIndex], ActiveMap.TileSize);
            }
            else
            {
                TilesetViewer.InitTileset(string.Empty, ActiveMap.TileSize);
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

            foreach (InteractiveProp Instance in ActiveMap.DicInteractiveProp.Values)
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