﻿using System;
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
        private CheckBox cbShowTerrainType;
        private CheckBox cbShowTerrainHeight;
        private CheckBox cbShow3DObjects;

        private List<IMapEditorTab> ListTab = new List<IMapEditorTab>();
        EventPointsTab EventPointsTab;
        ScriptsTab ScriptsTab;
        PropTab PropTab;
        ZoneTab ZoneTab;
        LayerTab LayerTab;

        protected BattleMap ActiveMap => BattleMapViewer.ActiveMap;
        protected IMapHelper Helper;

        //Spawn point related stuff.
        private System.Drawing.Point LastMousePosition;

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

            EventPointsTab = new EventPointsTab();
            ListTab.Add(EventPointsTab);

            ScriptsTab = new ScriptsTab();
            ListTab.Add(ScriptsTab);

            LayerTab = new LayerTab();
            ListTab.Add(LayerTab);

            PropTab = new PropTab();
            ListTab.Add(PropTab);

            ZoneTab = new ZoneTab();
            ListTab.Add(ZoneTab);

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.BattleMapViewer = BattleMapViewer;
                ActiveTab.TilesetViewer = TilesetViewer;
                tabToolBox.TabPages.Add(ActiveTab.InitTab(mnuToolBar));
            }

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

            BattleMapViewer.Helper = Helper;

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.Helper = Helper;
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

            BattleMapViewer.SelectedTilesetIndex = cboTiles.SelectedIndex;
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
            else if (keyData == Keys.C)
            {
                cbShow3DObjects.Checked = cbShow3DObjects.Checked;
            }

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                KeyProcessed |= ActiveTab.ProcessCmdKey(ref msg, keyData);
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

            ActiveMap.CursorPosition.X = (int)Math.Max(0, Math.Min(ActiveMap.MapSize.X - 1, (e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X));
            ActiveMap.CursorPosition.Y = (int)Math.Max(0, Math.Min(ActiveMap.MapSize.Y - 1, (e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y));

            DrawInfo();

            Map_MouseMove(e);
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
                                    PlaceTile(X + (int)(MapPreviewStartingPos.X) / ActiveMap.TileSize.X, Y + (int)(MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y, BattleMapViewer.SelectedListLayerIndex, true, BrushIndex);
                                }
                            }
                        }
                        else
                        {
                            pnMapPreview_MouseMove(sender, e);
                        }

                        BattleMapViewer.TileReplacementZone = new Rectangle();
                        break;

                    default:
                        ListTab[tabToolBox.SelectedIndex - 1].OnMouseUp(e);
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

                default:
                    ListTab[tabToolBox.SelectedIndex - 1].OnMouseDown(e);
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
            if (tabToolBox.SelectedIndex == 0 )
            {
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
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
                            Terrain SelectedTerrain = Helper.GetTerrain(TilePos.X, TilePos.Y, BattleMapViewer.SelectedListLayerIndex);

                            TileAttributesEditor.Init(SelectedTerrain, ActiveMap);

                            if (TileAttributesEditor.ShowDialog() == DialogResult.OK)
                            {
                                Helper.ReplaceTerrain(TilePos.X, TilePos.Y, TileAttributesEditor.ActiveTerrain, BattleMapViewer.SelectedListLayerIndex, true);
                            }
                        }
                        else if (e.Button == MouseButtons.Right)
                        {//Get the Tile under the mouse base on the map starting pos.
                            Point TilePos = new Point(MouseX, MouseY);
                            DrawableTile SelectedTerrain = Helper.GetTile(TilePos.X, TilePos.Y, BattleMapViewer.SelectedListLayerIndex);

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
                        PlaceTile((int)(e.X + MapPreviewStartingPos.X) / ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / ActiveMap.TileSize.Y, BattleMapViewer.SelectedListLayerIndex, true, BrushIndex);
                    }
                }
            }
            else
            {
                ListTab[tabToolBox.SelectedIndex - 1].OnMouseMove(e, MouseX, MouseY);
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

            if (TilesetViewer.ListSmartTilesetPresets.Count > 0)
            {
                if (TilePos.Y == 0)
                {
                    if (TilePos.X >= TilesetViewer.ListSmartTilesetPresets.Count)
                    {
                        return;
                    }

                    Terrain SmartPresetTerrain = TilesetViewer.ListSmartTilesetPresets[TilePos.X].ArrayTerrain[0, 0];
                    DrawableTile SmartPresetTile = TilesetViewer.ListSmartTilesetPresets[TilePos.X].ArrayTiles[0, 0];

                    Helper.ReplaceTerrain(X, Y,
                        SmartPresetTerrain, LayerIndex, ConsiderSubLayers);

                    Helper.ReplaceTile(X, Y,
                        SmartPresetTile, LayerIndex, ConsiderSubLayers);

                    return;
                }
                else
                {
                    TilePos.Y -= 1;
                }
            }

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
                                Microsoft.Xna.Framework.Graphics.Texture2D NewTilesetSprite = TilesetViewer.content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Maps/Tilesets/" + NewTileset.TilesetName);
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

                                TilesetViewer.ListSmartTilesetPresets.Add(NewTileset);
                                TilesetViewer.ListTilesetPresetsSprite.Add(NewTilesetSprite);
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
                                    Helper.ReplaceTerrain(X, Y, new Terrain(X, Y, BattleMapViewer.SelectedListLayerIndex, 0,
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
            foreach (GameModeInfoHolder ActiveRow in MS.frmDefaultGameModesConditions.lstGameModes.Items)
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
            if (tabToolBox.SelectedIndex > 0)
            {
                BattleMapViewer.ActiveTab = ListTab[tabToolBox.SelectedIndex - 1];
            }
            else
            {
                BattleMapViewer.ActiveTab = null;
            }
        }

        private void ProjectEternityMapEditor_Shown(object sender, EventArgs e)
        {
            if (BattleMapViewer.ActiveMap == null)
                return;

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

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.OnMapLoaded();
            }
        }
    }
}