using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.WorldMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Editors.WorldMapEditor
{
    public partial class ProjectEternityWorldMapEditor : ProjectEternityMapEditor
    {
        class WorldMapHelper : IMapHelper
        {
            private WorldMap ActiveMap;

            public WorldMapHelper(WorldMap ActiveMap)
            {
                this.ActiveMap = ActiveMap;
            }

            public void InitMap()
            {
                ActiveMap.Load();
            }

            public ITileAttributes GetTileEditor()
            {
                return new TileAttributes();
            }

            public Terrain GetTerrain(int X, int Y, int LayerIndex)
            {
                return ActiveMap.GetTerrain(X, Y, LayerIndex);
            }

            public DrawableTile GetTile(int X, int Y, int LayerIndex)
            {
                return ActiveMap.ListLayer[LayerIndex].OriginalLayerGrid.GetTile(X, Y);
            }

            public void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset)
            {
                for (int L = 0; L < GetLayerCount(); ++L)
                {
                    //Init the MapTiles.
                    Terrain[,] ArrayTerrain = new Terrain[NewWidth, NewHeight];
                    DrawableTile[,] ArrayTile2D = new DrawableTile[NewWidth, NewHeight];
                    for (int X = 0; X < NewWidth; X++)
                    {
                        for (int Y = 0; Y < NewHeight; Y++)
                        {
                            if (X < ActiveMap.MapSize.X && Y < ActiveMap.MapSize.Y)
                            {
                                ArrayTerrain[X, Y] = ActiveMap.ListLayer[L].ArrayTerrain[X, Y];
                                ArrayTile2D[X, Y] = ActiveMap.ListLayer[L].OriginalLayerGrid.GetTile(X, Y);
                            }
                            else
                            {
                                if (ActiveMap.ListTilesetPreset.Count > 0)
                                {
                                    Terrain NewTerrain = new Terrain(TerrainPreset, new Point(X, Y), L);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.WorldPosition = new Vector3(X, Y, 0);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new Terrain(X, Y, L, ActiveMap.ListLayer[L].Depth);
                                    ArrayTile2D[X, Y] = ActiveMap.ListLayer[L].OriginalLayerGrid.GetTile(X, Y);
                                }
                            }
                        }
                    }

                    ActiveMap.ListLayer[L].ArrayTerrain = ArrayTerrain;
                    ActiveMap.ListLayer[L].OriginalLayerGrid.ReplaceGrid(ArrayTile2D);
                }

                ActiveMap.MapSize = new Point(NewWidth, NewHeight);
            }

            public void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers)
            {
                Terrain NewTerrain = new Terrain(TerrainPreset, new Point(X, Y), LayerIndex);
                NewTerrain.WorldPosition = new Vector3(X, Y, 0);

                ActiveMap.ListLayer[LayerIndex].ArrayTerrain[X, Y] = NewTerrain;
            }

            public void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers)
            {
                DrawableTile NewTile = new DrawableTile(TilePreset);
                
                ActiveMap.ListLayer[LayerIndex].OriginalLayerGrid.ReplaceTile(X, Y, NewTile);
            }

            public void RemoveTileset(int TilesetIndex)
            {
                ActiveMap.ListTileSet.RemoveAt(TilesetIndex);
                ActiveMap.ListTilesetPreset.RemoveAt(TilesetIndex);

                foreach (MapLayer ActiveLayer in ActiveMap.ListLayer)
                {
                    ActiveLayer.LayerGrid.RemoveTileset(TilesetIndex);
                }
            }

            public BaseMapLayer CreateNewLayer(Terrain TerrainPreset, DrawableTile TilePreset)
            {
                MapLayer NewLayer = new MapLayer(ActiveMap, ActiveMap.ListLayer.Count);
                ActiveMap.ListLayer.Add(NewLayer);
                return NewLayer;
            }

            public ISubMapLayer CreateNewSubLayer(BaseMapLayer ParentLayer)
            {
                SubMapLayer NewLayer = new SubMapLayer();
                ((MapLayer)ParentLayer).ListSubLayer.Add(NewLayer);
                return NewLayer;
            }

            public void RemoveLayer(int Index)
            {
                ActiveMap.ListLayer.RemoveAt(Index);
            }

            public void RemoveSubLayer(BaseMapLayer ParentLayer, ISubMapLayer SubLayer)
            {
                ((MapLayer)ParentLayer).ListSubLayer.Remove((SubMapLayer)SubLayer);
            }

            public void EditLayer(int Index)
            {
                MapLayer NewMapLayer = ActiveMap.ListLayer[Index];

                ExtraLayerAttributes NewForm = new ExtraLayerAttributes(NewMapLayer.StartupDelay,
                    NewMapLayer.ToggleDelayOn,
                    NewMapLayer.ToggleDelayOff,
                    NewMapLayer.Depth);

                if (NewForm.ShowDialog() == DialogResult.OK)
                {
                    NewMapLayer.StartupDelay = (int)NewForm.txtAnimationStartupDelay.Value;
                    NewMapLayer.ToggleDelayOn = (int)NewForm.txtAnimationToggleDelayOn.Value;
                    NewMapLayer.ToggleDelayOff = (int)NewForm.txtAnimationToggleDelayOff.Value;
                    NewMapLayer.Depth = (int)NewForm.txtDepth.Value;
                }
            }

            public int GetLayerCount()
            {
                return ActiveMap.ListLayer.Count;
            }

            public List<BaseMapLayer> GetLayersAndSubLayers()
            {
                List<BaseMapLayer> ListLayers = new List<BaseMapLayer>();

                foreach (MapLayer ActiveMapLayer in ActiveMap.ListLayer)
                {
                    ListLayers.Add(ActiveMapLayer);
                    foreach (SubMapLayer ActiveSubMapLayer in ActiveMapLayer.ListSubLayer)
                    {
                        ListLayers.Add(ActiveSubMapLayer);
                    }
                }

                return ListLayers;
            }

            public MapZone CreateNewZone(ZoneShape.ZoneShapeTypes ZoneType)
            {
                throw new NotImplementedException();
            }
        }

        public class Zone
        {
            public System.Drawing.SolidBrush ZoneBrush;
            public List<Point> ListTile;
            public int NumberOfUnitsRequired;

            public Zone(System.Drawing.SolidBrush ZoneBrush)
            {
                this.ZoneBrush = ZoneBrush;
                ListTile = new List<Point>();
                NumberOfUnitsRequired = 0;
            }
        }

        private enum ItemSelectionChoices { Tile, BGM, UnitPosition, Cutscene };
        
        public ProjectEternityWorldMapEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityWorldMapEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(24);

            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                WorldMap NewMap = new WorldMap(MapLogicName, string.Empty);
                ActiveMap = BattleMapViewer.ActiveMap = NewMap;
                NewMap.ListLayer.Add(new MapLayer(NewMap, 0));

                SaveItem(FilePath, FilePath);
            }

            LoadMap(FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathWorldMaps, GUIRootPathMaps }, "Maps/World Maps/", new string[] { ".pem" }, typeof(ProjectEternityWorldMapEditor))
            };

            return Info;
        }
        
        private void LoadMap(string MapPath)
        {
            string Name = MapPath.Substring(0, MapPath.Length - 4).Substring(MapPath.LastIndexOf("Maps") + 5);
            BattleMapViewer.Preload();
            WorldMap ActiveWorldMap = new WorldMap(Name, string.Empty);
            Helper = new WorldMapHelper(ActiveWorldMap);

            InitMap(ActiveWorldMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity World Map Editor";
        }

        #region Methods
        
        protected override void pnMapPreview_MouseUp(object sender, MouseEventArgs e)
        {
            Vector3 MapPreviewStartingPos = BattleMapViewer.ActiveMap.CameraPosition;//Used to avoid warnings.
            Point MousePos = new Point((int)(e.X + MapPreviewStartingPos.X) / BattleMapViewer.ActiveMap.TileSize.X, (int)(e.Y + MapPreviewStartingPos.Y) / BattleMapViewer.ActiveMap.TileSize.Y);

            switch (tabToolBox.SelectedIndex)
            {
                case 0:
                case 1:
                    pnMapPreview_MouseMove(sender, e);
                    break;

                case 2:
                    BattleMapViewer.Scripting_MouseUp(e);
                    break;
                case 3:
                    if (lvZones.SelectedIndices.Count == 0)
                        return;

                    Zone ActiveZone = (Zone)lvZones.SelectedItems[0].Tag;

                    if (e.Button == MouseButtons.Left)
                    {
                        if (!ActiveZone.ListTile.Contains(MousePos))
                        {
                            ActiveZone.ListTile.Add(MousePos);
                        }
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        ActiveZone.ListTile.Remove(MousePos);
                    }
                    break;

                case 4:
                    break;
            }
        }
        
        #endregion
                    
        #region Zone Events
        
        private void lvZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvZones.SelectedIndices.Count == 0)
            {
                txtZoneName.Enabled = false;
                spnNumberOfUnitToControl.Enabled = false;
                panZoneColor.BackColor = System.Drawing.Color.Gray;
                return;
            }

            txtZoneName.Enabled = true;
            spnNumberOfUnitToControl.Enabled = true;
            panZoneColor.Enabled = true;
            txtZoneName.Text = lvZones.SelectedItems[0].Text;
            spnNumberOfUnitToControl.Value = ((Zone)lvZones.SelectedItems[0].Tag).NumberOfUnitsRequired;
            panZoneColor.BackColor = ((Zone)lvZones.SelectedItems[0].Tag).ZoneBrush.Color;
        }

        private void btnAddZone_Click(object sender, EventArgs e)
        {
            ListViewItem NewItem = new ListViewItem("New Zone");
            NewItem.Tag = new Zone(new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(190, System.Drawing.Color.White)));
            lvZones.Items.Add(NewItem);
        }

        private void txtZoneName_TextChanged(object sender, EventArgs e)
        {
            if (lvZones.SelectedIndices.Count == 0)
                return;

            lvZones.SelectedItems[0].Text = txtZoneName.Text;
        }

        private void spnNumberOfUnitToControl_ValueChanged(object sender, EventArgs e)
        {
            if (lvZones.SelectedIndices.Count == 0)
                return;

            Zone SelectedZone = (Zone)lvZones.SelectedItems[0].Tag;
            SelectedZone.NumberOfUnitsRequired = (int)spnNumberOfUnitToControl.Value;
        }

        private void panZoneColor_Click(object sender, EventArgs e)
        {
            if (lvZones.SelectedIndices.Count == 0)
                return;

            Zone SelectedZone = (Zone)lvZones.SelectedItems[0].Tag;
            ColorDialog cd = new ColorDialog();
            cd.Color = SelectedZone.ZoneBrush.Color;

            if (cd.ShowDialog() == DialogResult.OK)
            {
                SelectedZone.ZoneBrush.Color = System.Drawing.Color.FromArgb(190, cd.Color);
                panZoneColor.BackColor = cd.Color;
            }
        }
        
        #endregion
    }
}
