using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TilesetPresetInformation
    {
        public string TilesetName;
        public Terrain[,] ArrayTerrain;
        public DrawableTile[,] ArrayTiles;
        public int AnimationFrames;
        public List<int> ListAllowedTerrainTypeIndex;

        public TilesetPresetInformation(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            this.TilesetName = TilesetName;

            ListAllowedTerrainTypeIndex = new List<int>();

            ArrayTerrain = new Terrain[TilesetWidth / TileSizeX, TilesetHeight / TileSizeY];
            ArrayTiles = new DrawableTile[ArrayTerrain.GetLength(0), ArrayTerrain.GetLength(1)];

            //Tiles
            for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
            {
                for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                {
                    Terrain NewTerrain = CreateTerrain(X, Y, TileSizeX, TileSizeY);
                    DrawableTile NewTile = new DrawableTile(new Rectangle(X * TileSizeX, Y * TileSizeY, TileSizeX, TileSizeY), TilesetIndex);

                    NewTerrain.TerrainTypeIndex = 0;

                    ArrayTerrain[X, Y] = NewTerrain;
                    ArrayTiles[X, Y] = NewTile;
                }
            }
        }

        public TilesetPresetInformation(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            TilesetName = BR.ReadString();
            AnimationFrames = BR.ReadByte();

            int ListAllowedTerrainTypeIndexCount = BR.ReadByte();
            ListAllowedTerrainTypeIndex = new List<int>(ListAllowedTerrainTypeIndexCount);

            for (int I = 0; I < ListAllowedTerrainTypeIndexCount; I++)
            {
                ListAllowedTerrainTypeIndex.Add(BR.ReadByte());
            }

            ArrayTerrain = new Terrain[BR.ReadInt32(), BR.ReadInt32()];
            ArrayTiles = new DrawableTile[ArrayTerrain.GetLength(0), ArrayTerrain.GetLength(1)];

            //Tiles
            for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
            {
                for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                {
                    Terrain NewTerrain = ReadTerrain(BR, X, Y, 0, 0);
                    Terrain3D Terrain3DInfo = new Terrain3D(BR, TileSizeX, TileSizeY);
                    DrawableTile NewTile = new DrawableTile(new Rectangle(X * TileSizeX, Y * TileSizeY, TileSizeX, TileSizeY), TilesetIndex);
                    NewTile.Terrain3DInfo = Terrain3DInfo;
                    ArrayTerrain[X, Y] = NewTerrain;
                    ArrayTiles[X, Y] = NewTile;
                }
            }
        }

        public virtual Terrain CreateTerrain(int X, int Y, int TileSizeX, int TileSizeY)
        {
            return new Terrain(X, Y, TileSizeX, TileSizeY, 0, 0, 0);
        }

        protected virtual Terrain ReadTerrain(BinaryReader BR, int X, int Y, int LayerIndex, int LayerDepth)
        {
            return new Terrain(BR, X, Y, 1, 1, LayerIndex, 0, LayerDepth);
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write(TilesetName);
            BW.Write((byte)AnimationFrames);

            BW.Write((byte)ListAllowedTerrainTypeIndex.Count);

            for (int I = 0; I < ListAllowedTerrainTypeIndex.Count; I++)
            {
                BW.Write((byte)ListAllowedTerrainTypeIndex[I]);
            }

            BW.Write(ArrayTerrain.GetLength(0));
            BW.Write(ArrayTerrain.GetLength(1));

            //Tiles
            for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
            {
                for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                {
                    ArrayTerrain[X, Y].Save(BW);
                    ArrayTiles[X, Y].Terrain3DInfo.Save(BW);
                }
            }
        }
    }
}
