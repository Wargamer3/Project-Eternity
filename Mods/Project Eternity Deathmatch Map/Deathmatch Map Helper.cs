using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchMapHelper : IMapHelper
    {
        private DeathmatchMap ActiveMap;

        public DeathmatchMapHelper(DeathmatchMap ActiveMap)
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
                                Terrain NewTerrain = new Terrain(TerrainPreset);
                                DrawableTile NewTile = new DrawableTile(TilePreset);
                                NewTerrain.Position = new Vector3(X, Y, 0);

                                ArrayTerrain[X, Y] = NewTerrain;
                                ArrayTile2D[X, Y] = NewTile;
                            }
                            else
                            {
                                ArrayTerrain[X, Y] = new Terrain(X, Y);
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

        public void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex)
        {
            Terrain NewTerrain = new Terrain(TerrainPreset);
            NewTerrain.Position = new Vector3(X, Y, TerrainPreset.Position.Z);

            GetRealLayer(LayerIndex).ArrayTerrain[X, Y] = NewTerrain;
        }

        public void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex)
        {
            DrawableTile NewTile = new DrawableTile(TilePreset);

            GetRealLayer(LayerIndex).OriginalLayerGrid.ReplaceTile(X, Y, NewTile);
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

        private MapLayer GetRealLayer(int LayerIndex)
        {
            int RealIndex = 0;

            for (int L = 0; L < ActiveMap.ListLayer.Count; ++L)
            {
                if (LayerIndex == RealIndex)
                {
                    return ActiveMap.ListLayer[L];
                }

                ++RealIndex;

                for (int L2 = 0; L2 < ActiveMap.ListLayer[L].ListSubLayer.Count; ++L2)
                {
                    if (LayerIndex == RealIndex)
                    {
                        return ActiveMap.ListLayer[L].ListSubLayer[L2];
                    }

                    ++RealIndex;
                }
            }
            return null;
        }

        public IMapLayer CreateNewLayer()
        {
            MapLayer NewLayer = new MapLayer(ActiveMap);
            ActiveMap.ListLayer.Add(NewLayer);
            return NewLayer;
        }

        public ISubMapLayer CreateNewSubLayer(IMapLayer ParentLayer)
        {
            SubMapLayer NewLayer = new SubMapLayer(ActiveMap);
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
}