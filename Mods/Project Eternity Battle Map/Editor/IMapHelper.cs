using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IMapHelper
    {
        void InitMap();
        void InitEditor(TabControl tabToolBox);
        ITileAttributes GetTileEditor();
        Terrain GetTerrain(int X, int Y, int LayerIndex);
        DrawableTile GetTile(int X, int Y, int LayerIndex);
        void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset);
        void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers);
        void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers);
        void RemoveTileset(int TilesetIndex);
        BaseMapLayer CreateNewLayer(Terrain TerrainPreset, DrawableTile TilePreset);
        ISubMapLayer CreateNewSubLayer(BaseMapLayer ParentLayer);
        void RemoveLayer(int Index);
        void RemoveSubLayer(BaseMapLayer ParentLayer, ISubMapLayer SubLayer);
        void EditLayer(int Index);
        int GetLayerCount();
        List<BaseMapLayer> GetLayersAndSubLayers();
        MapZone CreateNewZone(ZoneShape.ZoneShapeTypes ZoneType);
    }
}