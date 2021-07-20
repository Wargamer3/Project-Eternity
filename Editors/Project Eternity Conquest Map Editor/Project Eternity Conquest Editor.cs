using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.ConquestMapScreen;
using ProjectEternity.Editors.MapEditor;

namespace ProjectEternity.Editors.ConquestMapEditor
{
    public partial class ProjectEternityConquestEditor : ProjectEternityMapEditor
    {
        class ConquestMapHelper : IMapHelper
        {
            private ConquestMap ActiveMap;

            public ConquestMapHelper(ConquestMap ActiveMap)
            {
                this.ActiveMap = ActiveMap;
            }

            public ITileAttributes GetTileEditor()
            {
                return new TileAttributes();
            }

            public Terrain GetTerrain(int X, int Y, int LayerIndex)
            {
                return ActiveMap.GetTerrain(X, Y, LayerIndex);
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
                                ArrayTerrain[X, Y] = ActiveMap.ListLayer[L].ArrayTerrain[X, Y];
                                ArrayTile2D[X, Y] = ((Map2D)ActiveMap.ListLayer[L].LayerGrid).GetTile(X, Y);
                            }
                            else
                            {
                                if (ActiveMap.ListTilesetPreset.Count > 0)
                                {
                                    TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.Position = new Vector3(X, Y, 0);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new TerrainConquest(X, Y);
                                    ArrayTile2D[X, Y] = ((Map2D)ActiveMap.ListLayer[L].LayerGrid).GetTile(X, Y);
                                }
                            }
                        }
                    }

                    ActiveMap.ListLayer[L].ArrayTerrain = ArrayTerrain;
                    ((Map2D)ActiveMap.ListLayer[L].LayerGrid).ReplaceGrid(ArrayTile2D);
                }

                ActiveMap.MapSize = new Point(NewWidth, NewHeight);
            }

            public void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex)
            {
                TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset);
                NewTerrain.Position = new Vector3(X, Y, 0);

                ActiveMap.ListLayer[LayerIndex].ArrayTerrain[X, Y] = NewTerrain;
            }

            public void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex)
            {
                DrawableTile NewTile = new DrawableTile(TilePreset);

                ((Map2D)ActiveMap.ListLayer[LayerIndex].LayerGrid).ReplaceTile(X, Y, NewTile);
            }

            public IMapLayer CreateNewLayer()
            {
                MapLayer NewLayer = new MapLayer(ActiveMap, null, null);
                ActiveMap.ListLayer.Add(NewLayer);
                return NewLayer;
            }

            public ISubMapLayer CreateNewSubLayer(IMapLayer ParentLayer)
            {
                SubMapLayer NewLayer = new SubMapLayer();
                ((MapLayer)ParentLayer).ListSubLayer.Add(NewLayer);
                return NewLayer;
            }

            public void RemoveLayer(int Index)
            {
                ActiveMap.ListLayer.RemoveAt(Index);
            }

            public void RemoveSubLayer(IMapLayer ParentLayer, ISubMapLayer SubLayer)
            {
                ((MapLayer)ParentLayer).ListSubLayer.Remove((SubMapLayer)SubLayer);
            }

            public void EditLayer(int Index)
            {
                MapLayer NewMapLayer = ActiveMap.ListLayer[Index];

                ExtraLayerAttributes NewForm = new ExtraLayerAttributes(NewMapLayer.StartupDelay, NewMapLayer.ToggleDelayOn, NewMapLayer.ToggleDelayOff, NewMapLayer.Depth);

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

            public List<object> GetLayers()
            {
                List<object> ListLayers = new List<object>();

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
        }
        
        public ProjectEternityConquestEditor()
            : base()
        {
            InitializeComponent();
        }

        public ProjectEternityConquestEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                ConquestMap NewMap = new ConquestMap(FilePath, 0, null);
                ActiveMap = BattleMapViewer.ActiveMap = NewMap;
                NewMap.ListLayer.Add(new MapLayer(NewMap, NewMap.ListBackgrounds, NewMap.ListForegrounds));
                BattleMapViewer.ActiveMap.ArrayMultiplayerColor = new Color[] { Color.Turquoise, Color.White, Color.SteelBlue, Color.Silver, Color.SandyBrown, Color.Salmon, Color.Purple, Color.PaleGreen, Color.Orange, Color.Gold, Color.ForestGreen, Color.Firebrick, Color.Chartreuse, Color.Beige, Color.DeepPink, Color.DarkMagenta };

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
            ConquestMap ActiveConquestMap = new ConquestMap(MapLogicName, 0, null);
            Helper = new ConquestMapHelper(ActiveConquestMap);
            InitMap(ActiveConquestMap);

            this.Text = BattleMapViewer.ActiveMap.MapName + " - Project Eternity Conquest Map Editor";
        }

        //Open a tile attributes dialog.
        protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            TileAttributes TA = new TileAttributes(TerrainTypeIndex);
            if (TA.ShowDialog() == DialogResult.OK)
            {//Set the current tile attributes based on the TileAttibutes return.
                TerrainTypeIndex = TA.TerrainTypeIndex;
            }
        }
    }
}
