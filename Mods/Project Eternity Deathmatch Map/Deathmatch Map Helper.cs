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
            return ActiveMap.LayerManager.GetTile(X, Y, LayerIndex);
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
                            ArrayTerrain[X, Y] = ActiveMap.LayerManager.ListLayer[L].ArrayTerrain[X, Y];
                            ArrayTile2D[X, Y] = ActiveMap.LayerManager.ListLayer[L].LayerGrid.GetTile(X, Y);
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
                                ArrayTerrain[X, Y] = new Terrain(X, Y, L);
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
            Terrain NewTerrain = new Terrain(TerrainPreset);
            NewTerrain.LayerIndex = LayerIndex;
            NewTerrain.Position = new Vector3(X, Y, TerrainPreset.Position.Z);

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
            }
            else
            {
                ActiveMap.LayerManager.ListLayer[LayerIndex].LayerGrid.ReplaceTile(X, Y, NewTile);
                ActiveMap.LayerManager.LayerHolderDrawable.Reset();
            }
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

        public BaseMapLayer CreateNewLayer()
        {
            MapLayer NewLayer = new MapLayer(ActiveMap, ActiveMap.LayerManager.ListLayer.Count);
            ActiveMap.LayerManager.ListLayer.Add(NewLayer);
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
    }
}