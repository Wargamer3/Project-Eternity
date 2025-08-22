using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public interface IMapHelper
    {
        void InitMap();
        List<IMapEditorTab> GetEditorTabs();
        ITileAttributes GetTileEditor();
        Terrain GetTerrain(int GridX, int GridY, int LayerIndex);
        DrawableTile GetTile(int GridX, int GridY, int LayerIndex);
        void ResizeTerrain(int NewWidth, int NewHeight, Terrain TerrainPreset, DrawableTile TilePreset);
        void ReplaceTerrain(int GridX, int GridY, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers);
        void ReplaceTile(int GridX, int GridY, DrawableTile TilePreset, int LayerIndex, bool ConsiderSubLayers);
        void RemoveTileset(int TilesetIndex);
        BaseMapLayer CreateNewLayer(Terrain TerrainPreset, DrawableTile TilePreset);
        ISubMapLayer CreateNewSubLayer(BaseMapLayer ParentLayer);
        void RemoveLayer(int Index);
        void RemoveSubLayer(BaseMapLayer ParentLayer, ISubMapLayer SubLayer);
        void EditLayer(int Index);
        int GetLayerCount();
        List<BaseMapLayer> GetLayersAndSubLayers();
        MapZone CreateNewZone(ZoneShape.ZoneShapeTypes ZoneType);
        Terrain.TilesetPreset LoadAutotilePreset(string TilesetName, int TilesetIndex);
        Terrain.TilesetPreset LoadTilesetPreset(string TilesetName, int TilesetIndex);
        void CreateTilesetPresetFromSprite(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex);
    }
}