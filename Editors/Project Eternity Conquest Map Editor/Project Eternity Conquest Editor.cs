﻿using System;
using System.IO;
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
                    TerrainConquest[,] ArrayTerrain = new TerrainConquest[NewWidth, NewHeight];
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
                                    TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(X, Y), L);
                                    DrawableTile NewTile = new DrawableTile(TilePreset);
                                    NewTerrain.DrawableTile = NewTile;
                                    NewTerrain.WorldPosition = new Vector3(X, Y, 0);

                                    ArrayTerrain[X, Y] = NewTerrain;
                                    ArrayTile2D[X, Y] = NewTile;
                                }
                                else
                                {
                                    ArrayTerrain[X, Y] = new TerrainConquest(X, Y, L, ActiveMap.LayerManager.ListLayer[L].Depth);
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
                TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(X, Y), LayerIndex);
                NewTerrain.Owner = ActiveMap;
                NewTerrain.WorldPosition = new Vector3(X, Y, LayerIndex + TerrainPreset.Height);

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

                TerrainConquest[,] ArrayTerrain = new TerrainConquest[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];
                DrawableTile[,] ArrayTile2D = new DrawableTile[ActiveMap.MapSize.X, ActiveMap.MapSize.Y];

                for (int X = 0; X < ActiveMap.MapSize.X; X++)
                {
                    for (int Y = 0; Y < ActiveMap.MapSize.Y; Y++)
                    {
                        TerrainConquest NewTerrain = new TerrainConquest(TerrainPreset, new Point(X, Y), ActiveMap.LayerManager.ListLayer.Count - 1);
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

        //Open a tile attributes dialog.
        protected override void btnTileAttributes_Click(object sender, EventArgs e)
        {
            TileAttributes TA = new TileAttributes(0);
            if (TA.ShowDialog() == DialogResult.OK)
            {//Set the current tile attributes based on the TileAttibutes return.
            }
        }
    }
}
