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
        void ReplaceTile(int GridX, int GridY, DrawableTile TilePreset, Terrain TerrainPreset, int LayerIndex, bool ConsiderSubLayers, bool IsAutotile);
        void RemoveTileset(int TilesetIndex);
        void ReplaceDestructibleTileset(int GridX, int GridY, int LayerIndex, DrawableTile TilePreset, Terrain TerrainPreset, DestructibleTilesetPreset Preset);
        BaseMapLayer CreateNewLayer(Terrain TerrainPreset, DrawableTile TilePreset);
        ISubMapLayer CreateNewSubLayer(BaseMapLayer ParentLayer);
        void RemoveLayer(int Index);
        void RemoveSubLayer(BaseMapLayer ParentLayer, ISubMapLayer SubLayer);
        void EditLayer(int Index);
        int GetLayerCount();
        List<BaseMapLayer> GetLayersAndSubLayers();
        MapZone CreateNewZone(ZoneShape.ZoneShapeTypes ZoneType);
        TilesetPreset LoadTilesetPreset(string Folder, string TilesetName, int TilesetIndex);
        DestructibleTilesetPreset LoadDestructibleTilesetPreset(string Folder, string TilesetName, int TilesetIndex);
        TilesetPreset CreateTilesetPresetFromSprite(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex);
    }
}