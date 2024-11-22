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
        private CheckBox cbShowGrid;
        private CheckBox cbPreviewMap;
        private CheckBox cbShowTerrainType;
        private CheckBox cbShowTerrainHeight;
        private CheckBox cbShow3DObjects;

        private List<IMapEditorTab> ListTab = new List<IMapEditorTab>();

        protected BattleMap ActiveMap => BattleMapViewer.ActiveMap;
        protected IMapHelper Helper;

        //Spawn point related stuff.
        private System.Drawing.Point LastMousePosition;

        private static DeathmatchParams Params;

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
            ListTab = Helper.GetEditorTabs();

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.BattleMapViewer = BattleMapViewer;
                tabToolBox.TabPages.Add(ActiveTab.InitTab(mnuToolBar));
            }
        }

        private void DrawInfo()
        {//Draw the mouse position minus the map starting point.
            tslInformation.Text = string.Format("X: {0}, Y: {1}, Z: {2}", ActiveMap.CursorPosition.X, ActiveMap.CursorPosition.Y, ActiveMap.CursorPosition.Z);

            if (ActiveMap.CursorPosition.X < 0 || ActiveMap.CursorPosition.X >= ActiveMap.MapSize.X || ActiveMap.CursorPosition.Y < 0 || ActiveMap.CursorPosition.Y >= ActiveMap.MapSize.Y)
                return;

            ListTab[tabToolBox.SelectedIndex].DrawInfo(tslInformation);

            tslInformation.Text += ", Arrow keys, Q, E and Shift to move and rotate the camera";
        }

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
                ActiveMap.CursorPosition.X -= (ActiveMap.CursorPosition.X > 0) ? ActiveMap.TileSize.X : 0;
                KeyProcessed = true;
            }
            else if (IsMovingRight)
            {
                ActiveMap.CursorPosition.X += (ActiveMap.CursorPosition.X < ActiveMap.MapSize.X - 1) ? ActiveMap.TileSize.X : 0;
                KeyProcessed = true;
            }

            if (IsMovingUp)
            {
                ActiveMap.CursorPosition.Y -= (ActiveMap.CursorPosition.Y > 0) ? ActiveMap.TileSize.Y : 0;
                KeyProcessed = true;
            }
            else if (IsMovingDown)
            {
                ActiveMap.CursorPosition.Y += (ActiveMap.CursorPosition.Y < ActiveMap.MapSize.Y - 1) ? ActiveMap.TileSize.Y : 0;
                KeyProcessed = true;
            }
            else if (keyData == Keys.C)
            {
                cbShow3DObjects.Checked = cbShow3DObjects.Checked;
            }

            KeyProcessed |= ListTab[tabToolBox.SelectedIndex].TabProcessCmdKey(ref msg, keyData);

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
                ActiveMap.Camera2DPosition.X,
                ActiveMap.Camera2DPosition.Y,
                ActiveMap.Camera2DPosition.Z);

            ActiveMap.CursorPosition.X = (int)Math.Max(0, Math.Min((ActiveMap.MapSize.X - 1) * ActiveMap.TileSize.X, (e.X + MapPreviewStartingPos.X)));
            ActiveMap.CursorPosition.Y = (int)Math.Max(0, Math.Min((ActiveMap.MapSize.Y - 1) * ActiveMap.TileSize.Y, (e.Y +  MapPreviewStartingPos.Y)));
            ActiveMap.CursorPosition.X = (int)Math.Floor(ActiveMap.CursorPosition.X / ActiveMap.TileSize.X) * ActiveMap.TileSize.X + ActiveMap.TileSize.X / 2;
            ActiveMap.CursorPosition.Y = (int)Math.Floor(ActiveMap.CursorPosition.Y / ActiveMap.TileSize.Y) * ActiveMap.TileSize.Y + ActiveMap.TileSize.Y / 2;

            DrawInfo();

            ListTab[tabToolBox.SelectedIndex].OnMouseMove(e);
        }

        protected virtual void pnMapPreview_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                ListTab[tabToolBox.SelectedIndex].TabOnMouseUp(e);
            }
        }

        protected virtual void pnMapPreview_MouseDown(object sender, MouseEventArgs e)
        {
            LastMousePosition = e.Location;

            ListTab[tabToolBox.SelectedIndex].TabOnMouseDown(e);
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

            ListTab[tabToolBox.SelectedIndex].OnMapResize(MapSize.X, MapSize.Y);

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
            BattleMapViewer.ActiveTab = ListTab[tabToolBox.SelectedIndex];
        }

        private void ProjectEternityMapEditor_Shown(object sender, EventArgs e)
        {
            if (BattleMapViewer.ActiveMap == null)
                return;

            BattleMapViewer.Reset();
            Helper.InitMap();

            ActiveMap.TogglePreview(true);

            BattleMapViewer.Helper = Helper;

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.Helper = Helper;
            }

            BattleMapViewer.RefreshScrollbars();

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, BattleMapViewer.Width, BattleMapViewer.Height, 0, 0, -1f);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Matrix projectionMatrix = HalfPixelOffset * Projection;

            ActiveMap.fxOutline.Parameters["Projection"].SetValue(projectionMatrix);

            foreach (IMapEditorTab ActiveTab in ListTab)
            {
                ActiveTab.OnMapLoaded();
            }
        }
    }
}