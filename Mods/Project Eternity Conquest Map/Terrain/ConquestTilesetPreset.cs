using System.IO;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestTilesetPreset : TilesetPreset
    {
        public class ConquestTilesetPresetInformation : TilesetPresetInformation
        {
            public ConquestTilesetPresetInformation(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public ConquestTilesetPresetInformation(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(BR, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public override Terrain CreateTerrain(int X, int Y, int TileSizeX, int TileSizeY)
            {
                return new TerrainConquest(X, Y, TileSizeX, TileSizeY, 0, 0, 0);
            }

            protected override Terrain ReadTerrain(BinaryReader BR, int X, int Y, int LayerIndex, int LayerDepth)
            {
                return new TerrainConquest(BR, X, Y, 0, 0, LayerIndex, 0, LayerDepth);
            }
        }

        public ConquestTilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
        {
        }

        public ConquestTilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex, bool LoadBackgroundPaths = true)
            : base(BR, TileSizeX, TileSizeY, TilesetIndex, LoadBackgroundPaths)
        {
        }

        protected override TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected override TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }
    }
}
