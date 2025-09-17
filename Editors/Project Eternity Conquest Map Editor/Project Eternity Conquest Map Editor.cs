using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.MapEditor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestEditor : ProjectEternityMapEditor
    {
        public class ConquestMapHelper : IMapHelper
        {
            private ConquestMap ActiveMap;

            public ConquestMapHelper(ConquestMap ActiveMap)
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

                ListTab.Add(new ConquestTilesetTab());
                ListTab.Add(new EventPointsTab());
                ListTab.Add(new SpawnsTab());
                ListTab.Add(new BuildingsTab());
                ListTab.Add(new SceneryTab());
                ListTab.Add(new ScriptsTab());
                ListTab.Add(new LayerTab());
                ListTab.Add(new PropTab());
                ListTab.Add(new ZoneTab());

                return ListTab;
            }

            public ITileAttributes GetTileEditor()
            {
                return new TileAttributes();
            }

            public Terrain GetTerrain(int GridX, int GridY, int LayerIndex)
            {
                return ActiveMap.GetTerrain(new Vector3(GridX, GridY, LayerIndex));
            }

            public DrawableTile GetTile(int GridX, int GridY, int LayerIndex)
            {
                return ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTile[GridX, GridY];
            }

            public void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset)
            {
                for (int L = 0; L < GetLayerCount(); ++L)
                {
                    //Init the MapTiles.
                    TerrainConquest[,] ArrayTerrain = new TerrainConquest[NewWidth, NewHeight];
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
                                    TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(X, Y), L);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.Owner = ActiveMap;
                                    NewTerrain.WorldPosition = new Vector3(X * ActiveMap.TileSize.X, Y * ActiveMap.TileSize.Y, (L + NewTerrain.Height) * ActiveMap.LayerHeight);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new TerrainConquest(X, Y, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, L, ActiveMap.LayerHeight, ActiveMap.LayerManager.ListLayer[L].Depth);
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

            public void ReplaceTerrain(int GridX, int GridY, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers)
            {
                TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(GridX, GridY), LayerIndex);
                NewTerrain.Owner = ActiveMap;
                NewTerrain.WorldPosition = new Vector3(GridX * ActiveMap.TileSize.X, GridY * ActiveMap.TileSize.Y, (LayerIndex + NewTerrain.Height) * ActiveMap.LayerHeight);

                if (ConsiderSubLayers)
                {
                    GetRealLayer(LayerIndex).ArrayTerrain[GridX, GridY] = NewTerrain;
                }
                else
                {
                    ActiveMap.LayerManager.ListLayer[LayerIndex].ArrayTerrain[GridX, GridY] = NewTerrain;
                }
            }

            public void ReplaceTile(int GridX, int GridY, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers, bool IsAutotile)
            {
                if (LayerIndex == 0)
                {
                    TilePreset.Terrain3DInfo = new Terrain3D();
                    TilePreset.Terrain3DInfo.TerrainStyle = Terrain3D.TerrainStyles.Cube;
                }

                DrawableTile NewTile = new DrawableTile(TilePreset);

                MapLayer ActiveLayer;

                if (ConsiderSubLayers)
                {
                    ActiveLayer = GetRealLayer(LayerIndex);
                }
                else
                {
                    ActiveLayer = ActiveMap.LayerManager.ListLayer[LayerIndex];
                }

                if (IsAutotile)
                {
                    ActiveMap.ListTilesetPreset[TilePreset.TilesetIndex].UpdateAutotTile(TilePreset, GridX, GridY, ActiveMap.TileSize.X, ActiveMap.TileSize.Y, ActiveLayer.ArrayTile, ActiveMap.ListTilesetPreset);
                }
                else
                {
                    ActiveLayer.ArrayTile[GridX, GridY] = NewTile;
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

                TerrainConquest[,] ArrayTerrain = new TerrainConquest[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];
                DrawableTile[,] ArrayTile2D = new DrawableTile[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];

                for (int X = 0; X < ActiveMap.MapSize.X; X++)
                {
                    for (int Y = 0; Y < ActiveMap.MapSize.Y; Y++)
                    {
                        TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(X, Y), ActiveMap.LayerManager.ListLayer.Count - 1);
                        DrawableTile NewTile = new DrawableTile(TilePreset);
                        NewTerrain.Owner = ActiveMap;
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
                    for (int X = 0; X < ActiveMap.MapSize.X; ++X)
                    {
                        for (int Y = 0; Y < ActiveMap.MapSize.Y; ++Y)
                        {
                            NewMapLayer.ArrayTerrain[X, Y].LayerDepth = NewMapLayer.Depth;
                        }
                    }

                    ActiveMap.Reset();
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
                return new MapZoneConquest(ActiveMap, ZoneType);
            }

            public TilesetPreset LoadAutotilePreset(string TilesetName, int TilesetIndex)
            {
                FileStream FS = new FileStream("Content/Maps/Autotiles Presets/" + TilesetName + ".peat", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();

                TilesetPreset NewTilesetPreset = new ConquestTilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);

                BR.Close();
                FS.Close();

                ActiveMap.ListTilesetPreset.Add(NewTilesetPreset);
                return NewTilesetPreset;
            }

            public TilesetPreset LoadTilesetPreset(string TilesetName, int TilesetIndex)
            {
                FileStream FS = new FileStream("Content/Maps/Tilesets Presets/" + TilesetName + ".pet", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();

                TilesetPreset NewTilesetPreset = new ConquestTilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);

                BR.Close();
                FS.Close();

                ActiveMap.ListTilesetPreset.Add(NewTilesetPreset);
                return NewTilesetPreset;
            }

            public TilesetPreset CreateTilesetPresetFromSprite(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                return new ConquestTilesetPreset(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            }
        }

        private static ConquestParams Params;

        public ProjectEternityConquestEditor()
            : base()
        {
            InitializeComponent();

            KeyPreview = true;

            if (Params == null)
            {
                Params = new ConquestParams(new BattleContext());
            }
        }

        public ProjectEternityConquestEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                ConquestMap NewMap = new ConquestMap(FilePath, new GameModeInfo(), ProjectEternityConquestEditor.Params);
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
                new EditorInfo(new string[] { GUIRootPathMaps, GUIRootPathConquestMaps }, "Maps/Conquest/", new string[] { ".pem" }, typeof(ProjectEternityConquestEditor))
            };

            return Info;
        }
                
        private void LoadMap(string Path)
        {
            string MapLogicName = FilePath.Substring(0, FilePath.Length - 4).Substring(22);

            BattleMapViewer.Preload();
            ConquestMap ActiveConquestMap = new ConquestMap(MapLogicName, new GameModeInfo(), ProjectEternityConquestEditor.Params);
            Helper = new ConquestMapHelper(ActiveConquestMap);
            InitMap(ActiveConquestMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity Conquest Map Editor";
        }
    }
}
