using System;
using System.IO;
using System.Text;
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

            public List<IMapEditorTab> GetEditorTabs()
            {
                List<IMapEditorTab> ListTab = new List<IMapEditorTab>();

                ListTab.Add(new SorcererStreetTilesetTab());
                ListTab.Add(new EventPointsTab());
                ListTab.Add(new ScriptsTab());
                ListTab.Add(new LayerTab());
                ListTab.Add(new SorcererStreetAreasTab());
                ListTab.Add(new PropTab());
                ListTab.Add(new ZoneTab());

                return ListTab;
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
                return ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTile[X, Y];
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
                                ArrayTile2D[X, Y] = ActiveMap.LayerManager.ListLayer[L].ArrayTile[X, Y];
                            }
                            else
                            {
                                if (ActiveMap.ListTilesetPreset.Count > 0)
                                {
                                    TerrainSorcererStreet NewTerrain = new TerrainSorcererStreet(TerrainPreset, new Point(X, Y), L);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.Owner = ActiveMap;
                                    NewTerrain.WorldPosition = new Vector3(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y, (L + NewTerrain.Height) * ActiveMap.LayerHeight);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, L, ActiveMap.LayerHeight, ActiveMap.LayerManager.ListLayer[L].Depth);
                                    ArrayTile2D[X, Y] = ActiveMap.LayerManager.ListLayer[L].ArrayTile[X, Y];
                                }
                            }
                        }
                    }

                    ActiveMap.LayerManager.ListLayer[L].ArrayTerrain = ArrayTerrain;
                    ActiveMap.LayerManager.ListLayer[L].ArrayTile = ArrayTile2D;
                }

                ActiveMap.MapSize = new Point(NewWidth, NewHeight);
            }

            public void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers)
            {
                TerrainSorcererStreet NewTerrain = new TerrainSorcererStreet(TerrainPreset, new Point(X, Y), LayerIndex);
                NewTerrain.Owner = ActiveMap;
                NewTerrain.WorldPosition = new Vector3(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y, (LayerIndex + NewTerrain.Height) * ActiveMap.LayerHeight);

                if (ConsiderSubLayers)
                {
                    GetRealLayer(LayerIndex).ArrayTerrain[X, Y] = NewTerrain;
                }
                else
                {
                    ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTerrain[X, Y] = NewTerrain;
                }
            }

            public void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers, bool IsAutotile)
            {
                DrawableTile NewTile = new DrawableTile(TilePreset);

                if (ConsiderSubLayers)
                {
                    GetRealLayer(LayerIndex).ArrayTile[X, Y] = NewTile;
                }
                else
                {
                    ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTile[X, Y] = NewTile;
                }

                ActiveMap.Reset();
            }

            public void RemoveTileset(int TilesetIndex)
            {
                ActiveMap.ListTileSet.RemoveAt(TilesetIndex);
                ActiveMap.ListTilesetPreset.RemoveAt(TilesetIndex);

                foreach (MapLayer ActiveLayer in ActiveMap.LayerManager.ListLayer)
                {
                    ActiveLayer.RemoveTileset(TilesetIndex);
                }
            }

            public void ReplaceDestructibleTileset(int GridX, int GridY, int LayerIndex, DestructibleTilesetPreset Preset)
            {
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
                        TerrainSorcererStreet NewTerrain = NewLayer.ArrayTerrain[X, Y];
                        DrawableTile NewTile = new DrawableTile(TilePreset);
                        NewTerrain.WorldPosition = new Vector3(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y, (ActiveMap.LayerManager.ListLayer.Count - 1 + NewTerrain.Height) * ActiveMap.LayerHeight);

                        ArrayTerrain[X, Y] = NewTerrain;
                        ArrayTile2D[X, Y] = NewTile;
                    }
                }

                NewLayer.ArrayTerrain = ArrayTerrain;
                NewLayer.ArrayTile = ArrayTile2D;

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

            public TilesetPreset LoadTilesetPreset(string Folder, string TilesetName, int TilesetIndex)
            {
                FileStream FS = new FileStream("Content/" + Folder + "/" + TilesetName + ".pet", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();

                TilesetPreset NewTilesetPreset = new TilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);

                BR.Close();
                FS.Close();

                ActiveMap.ListTilesetPreset.Add(NewTilesetPreset);

                return NewTilesetPreset;
            }

            public DestructibleTilesetPreset LoadDestructibleTilesetPreset(string Folder, string TilesetName, int TilesetIndex)
            {
                FileStream FS = new FileStream("Content/" + Folder + "/" + TilesetName + ".pet", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();

                DestructibleTilesetPreset NewTilesetPreset = new DestructibleTilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);

                BR.Close();
                FS.Close();

                ActiveMap.ListTemporaryTilesetPreset.Add(NewTilesetPreset);

                return NewTilesetPreset;
            }

            public TilesetPreset CreateTilesetPresetFromSprite(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new SorcererStreetTilesetPreset(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
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
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SorcererStreetMap NewMap = new SorcererStreetMap(FilePath, new GameModeInfo(), ProjectEternitySorcererStreetEditor.Params);
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathMaps, EditorHelper.GUIRootPathSorcererStreetMaps }, "Sorcerer Street/Maps/", new string[] { ".pem" }, typeof(ProjectEternitySorcererStreetEditor))
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
    }
}
