using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IMapHelper
    {
        ITileAttributes GetTileEditor();
        Terrain GetTerrain(int X, int Y, int LayerIndex);
        DrawableTile GetTile(int X, int Y, int LayerIndex);
        void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset);
        void ReplaceTerrain(int X, int Y, Terrain TerrainPreset, int LayerIndex);
        void ReplaceTile(int X, int Y, DrawableTile TilePreset, int LayerIndex);
        void RemoveTileset(int TilesetIndex);
        IMapLayer CreateNewLayer();
        ISubMapLayer CreateNewSubLayer(IMapLayer ParentLayer);
        void RemoveLayer(int Index);
        void RemoveSubLayer(IMapLayer ParentLayer, ISubMapLayer SubLayer);
        void EditLayer(int Index);
        int GetLayerCount();
        List<object> GetLayers();
    }
}