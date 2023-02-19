using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    public partial class ProjectEternitySorcererStreetEditor : ProjectEternityMapEditor
    {
        class SorcererStreetMapHelper : IMapHelper
        {
            private SorcererStreetMap ActiveMap;

            public SorcererStreetMapHelper(SorcererStreetMap ActiveMap)
            {
                this.ActiveMap = ActiveMap;
            }

            public void InitMap()
            {
                ActiveMap.Load();
                ActiveMap.Init();
            }

            public ITileAttributes GetTileEditor()
            {
                return new TileAttributes();
            }

            public Terrain GetTerrain(int X, int Y, int LayerIndex)
            {
                return ActiveMap.GetTerrain(new Vector3(X, Y, LayerIndex));
            }

            public DrawableTile GetTile(int X, int Y, int LayerIndex)
            {
                return ActiveMap.GetTerrain(new Vector3(X, Y, LayerIndex)).DrawableTile;
            }

            public void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset)
            {
                for (int L = 0; L < GetLayerCount(); ++L)
                {
                    //Init the MapTiles.
                    TerrainSorcererStreet[,] ArrayTerrain = new TerrainSorcererStreet[NewWidth, NewHeight];
                    DrawableTile[,] ArrayTile2D = new DrawableTile[NewWidth, NewHeight];
                    for (int X = 0; X < NewWidth; X++)
                    {
                        for (int Y = 0; Y < NewHeight; Y++)
                        {
                            if (X < ActiveMap.MapSize.X && Y < ActiveMap.MapSize.Y)
                            {
                                ArrayTerrain[X, Y] = ActiveMap.LayerManager.ListLayer[L].ArrayTerrain[X, Y];
                                ArrayTile2D[X, Y] = ActiveMap.LayerManager.ListLayer[L].LayerGrid.GetTile(X, Y);
                            }
                            else
                            {
                                if (ActiveMap.ListTilesetPreset.Count > 0)
                                {
                                    TerrainSorcererStreet NewTerrain = new TerrainSorcererStreet(TerrainPreset, new Point(X, Y), L);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.WorldPosition = new Vector3(X, Y, 0);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, L, ActiveMap.LayerManager.ListLayer[L].Depth);
                                    ArrayTile2D[X, Y] = ActiveMap.LayerManager.ListLayer[L].LayerGrid.GetTile(X, Y);
                                }
                            }
                        }
                    }

                    ActiveMap.LayerManager.ListLayer[L].ArrayTerrain = ArrayTerrain;
                    ActiveMap.LayerManager.ListLayer[L].LayerGrid.ReplaceGrid(ArrayTile2D);
                }

                ActiveMap.MapSize = new Point(NewWidth, NewHeight);
            }

            public void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers)
            {
                TerrainSorcererStreet NewTerrain = new TerrainSorcererStreet(TerrainPreset, new Point(X, Y), LayerIndex);
                NewTerrain.Owner = ActiveMap;
                NewTerrain.WorldPosition = new Vector3(X, Y, TerrainPreset.Height);

                if (ConsiderSubLayers)
                {
                    GetRealLayer(LayerIndex).ArrayTerrain[X, Y] = NewTerrain;
                }
                else
                {
                    ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y] = NewTerrain;
                }
            }

            public void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers)
            {
                DrawableTile NewTile = new DrawableTile(TilePreset);

                if (ConsiderSubLayers)
                {
                    GetRealLayer(LayerIndex).LayerGrid.ReplaceTile(X, Y, NewTile);
                    GetRealLayer(LayerIndex).ArrayTerrain[X, Y].DrawableTile = NewTile;
                }
                else
                {
                    ActiveMap.LayerManager.ListLayer[LayerIndex].LayerGrid.ReplaceTile(X, Y, NewTile);
                    ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y].DrawableTile = NewTile;
                }

                ActiveMap.Reset();
            }

            public void RemoveTileset(int TilesetIndex)
            {
                ActiveMap.ListTileSet.RemoveAt(TilesetIndex);
                ActiveMap.ListTilesetPreset.RemoveAt(TilesetIndex);

                foreach (MapLayer ActiveLayer in ActiveMap.LayerManager.ListLayer)
                {
                    ActiveLayer.LayerGrid.RemoveTileset(TilesetIndex);
                }
            }

            private MapLayer GetRealLayer(int LayerIndex)
            {
                int RealIndex = 0;

                for (int L = 0; L < ActiveMap.LayerManager.ListLayer.Count; ++L)
                {
                    if (LayerIndex == RealIndex)
                    {
                        return ActiveMap.LayerManager.ListLayer[L];
                    }

                    ++RealIndex;

                    for (int L2 = 0; L2 < ActiveMap.LayerManager.ListLayer[L].ListSubLayer.Count; ++L2)
                    {
                        if (LayerIndex == RealIndex)
                        {
                            return ActiveMap.LayerManager.ListLayer[L].ListSubLayer[L2];
                        }

                        ++RealIndex;
                    }
                }
                return null;
            }

            public BaseMapLayer CreateNewLayer(Terrain TerrainPreset, DrawableTile TilePreset)
            {
                MapLayer NewLayer = new MapLayer(ActiveMap, ActiveMap.LayerManager.ListLayer.Count);

                TerrainSorcererStreet[,] ArrayTerrain = new TerrainSorcererStreet[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];
                DrawableTile[,] ArrayTile2D = new DrawableTile[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];

                for (int X = 0; X < ActiveMap.MapSize.X; X++)
                {
                    for (int Y = 0; Y < ActiveMap.MapSize.Y; Y++)
                    {
                        TerrainSorcererStreet NewTerrain = new TerrainSorcererStreet(TerrainPreset, new Point(X, Y), ActiveMap.LayerManager.ListLayer.Count - 1);
                        DrawableTile NewTile = new DrawableTile(TilePreset);
                        NewTerrain.DrawableTile = NewTile;
                        NewTerrain.Owner = ActiveMap;
                        NewTerrain.WorldPosition = new Vector3(X, Y, ActiveMap.LayerManager.ListLayer.Count - 1);

                        ArrayTerrain[X, Y] = NewTerrain;
                        ArrayTile2D[X, Y] = NewTile;
                    }
                }

                NewLayer.ArrayTerrain = ArrayTerrain;
                NewLayer.LayerGrid.ReplaceGrid(ArrayTile2D);

                ActiveMap.LayerManager.ListLayer.Add(NewLayer);
                ActiveMap.Reset();

                return NewLayer;
            }

            public ISubMapLayer CreateNewSubLayer(BaseMapLayer ParentLayer)
            {
                MapLayer RealParent = (MapLayer)ParentLayer;
                SubMapLayer NewLayer = new SubMapLayer(ActiveMap, ActiveMap.LayerManager.ListLayer.IndexOf(RealParent));
                RealParent.ListSubLayer.Add(NewLayer);
                return NewLayer;
            }

            public void RemoveLayer(int Index)
            {
                ActiveMap.LayerManager.ListLayer.RemoveAt(Index);
            }

            public void RemoveSubLayer(BaseMapLayer ParentLayer, ISubMapLayer SubLayer)
            {
                ((MapLayer)ParentLayer).ListSubLayer.Remove((SubMapLayer)SubLayer);
            }

            public void EditLayer(int Index)
            {
                MapLayer NewMapLayer = GetRealLayer(Index);

                ExtraLayerAttributes NewForm = new ExtraLayerAttributes(NewMapLayer.StartupDelay, NewMapLayer.ToggleDelayOn, NewMapLayer.ToggleDelayOff, NewMapLayer.Depth);

                if (NewForm.ShowDialog() == DialogResult.OK)
                {
                    NewMapLayer.StartupDelay = (int)NewForm.txtAnimationStartupDelay.Value;
                    NewMapLayer.ToggleDelayOn = (int)NewForm.txtAnimationToggleDelayOn.Value;
                    NewMapLayer.ToggleDelayOff = (int)NewForm.txtAnimationToggleDelayOff.Value;
                    NewMapLayer.Depth = (float)NewForm.txtDepth.Value;
                }
            }

            public int GetLayerCount()
            {
                return ActiveMap.LayerManager.ListLayer.Count;
            }

            public List<BaseMapLayer> GetLayersAndSubLayers()
            {
                List<BaseMapLayer> ListLayers = new List<BaseMapLayer>();

                foreach (MapLayer ActiveMapLayer in ActiveMap.LayerManager.ListLayer)
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
                return new MapZoneSorcererStreet(ActiveMap, ZoneType);
            }
        }
        
        private static SorcererStreetBattleParams Params;

        public ProjectEternitySorcererStreetEditor()
            : base()
        {
            InitializeComponent();
            if (Params == null)
            {
                Params = new SorcererStreetBattleParams(new SorcererStreetBattleContext());
            }

        }

        public ProjectEternitySorcererStreetEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SorcererStreetMap NewMap = new SorcererStreetMap(FilePath, new GameModeInfo(), ProjectEternitySorcererStreetEditor.Params);
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
                new EditorInfo(new string[] { GUIRootPathMaps, GUIRootPathSorcererStreetMaps }, "Maps/Sorcerer Street/", new string[] { ".pem" }, typeof(ProjectEternitySorcererStreetEditor))
            };

            return Info;
        }
                
        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(29);

            BattleMapViewer.Preload();
            SorcererStreetMap ActiveMap = new SorcererStreetMap(MapLogicName, new GameModeInfo(), ProjectEternitySorcererStreetEditor.Params);
            Helper = new SorcererStreetMapHelper(ActiveMap);
            InitMap(ActiveMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity Sorcerer Street Map Editor";
        }

        protected override MapStatistics OpenMapProperties()
        {
            return new SorcererStreetMapStatistics((SorcererStreetMap)ActiveMap);
        }

        protected override void ApplyMapPropertiesChanges(MapStatistics MS)
        {
            SorcererStreetMapStatistics PropertiesEditor = (SorcererStreetMapStatistics)MS;
            base.ApplyMapPropertiesChanges(MS);

            SorcererStreetMap ActiveSorcererStreetMap = (SorcererStreetMap)ActiveMap;
            ActiveSorcererStreetMap.MagicAtStart = (int)PropertiesEditor.txtMagicAtStart.Value;
            ActiveSorcererStreetMap.MagicGainPerLap = (int)PropertiesEditor.txtMagicPerLap.Value;
            ActiveSorcererStreetMap.TowerMagicGain = (int)PropertiesEditor.txtMagicPerTower.Value;
            ActiveSorcererStreetMap.MagicGoal = (int)PropertiesEditor.txtMagicGoal.Value;
            ActiveSorcererStreetMap.HighestDieRoll = (int)PropertiesEditor.txtHighestDieRoll.Value;
    }
    protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            Rectangle TilePos = TilesetViewer.TileBrushSize;
            Terrain SelectedTerrain = ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y];

            TileAttributes TA = new TileAttributes(SelectedTerrain.TerrainTypeIndex, SelectedTerrain.Height);
            if (TA.ShowDialog() == DialogResult.OK)
            {
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y].TerrainTypeIndex = TA.TerrainTypeIndex;
                ActiveMap.ListTilesetPreset[cboTiles.SelectedIndex].ArrayTerrain[TilePos.X / ActiveMap.TileSize.X, TilePos.Y / ActiveMap.TileSize.Y].Height = TA.Height;
            }
        }
    }
}
