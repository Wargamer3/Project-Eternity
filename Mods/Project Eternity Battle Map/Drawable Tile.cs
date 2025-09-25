using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct DrawableTile
    {
        public Rectangle Origin;//X, Y origin from at which the tile is located in the TileSet.
        public Rectangle[] ArraySubTile;//X, Y origin from at which the tile is located in the TileSet.
        public int TilesetIndex;

        public Terrain3D Terrain3DInfo;

        public DrawableTile(BinaryReader BR, int TileWidth, int TileHeight)
        {
            TilesetIndex = BR.ReadInt32();
            Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TileWidth, TileHeight);
            bool HasArraySubTile = BR.ReadBoolean();
            if (HasArraySubTile)
            {
                float OffsetSizeX = (TileWidth / 2) / (float)TileWidth * TileWidth;
                float OffsetSizeY = (1 - (TileHeight / 4) / (float)TileHeight) * TileHeight;

                int ArraySubTileLength = BR.ReadByte();
                ArraySubTile = new Rectangle[ArraySubTileLength];
                for (int T = 0; T < ArraySubTileLength; ++T)
                {
                    int IndexX = (T % (ArraySubTileLength / 2));
                    int IndexY = (T / (ArraySubTileLength / 2));

                    if (IndexY == 0)
                    {
                        ArraySubTile[T] = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TileWidth / (ArraySubTileLength / 2), (int)OffsetSizeY);
                    }
                    else
                    {
                        ArraySubTile[T] = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TileWidth / (ArraySubTileLength / 2), TileHeight - (int)OffsetSizeY);
                    }
                }
            }
            else
            {
                ArraySubTile = new Rectangle[0];
            }
            bool HasTerrain3DInfo = BR.ReadBoolean();
            Terrain3DInfo = null;
            if (HasTerrain3DInfo)
            {
                Terrain3DInfo = new Terrain3D(BR, TileWidth, TileHeight);
            }
        }

        public DrawableTile(Rectangle[] ArraySubTile)
        {
            this.ArraySubTile = ArraySubTile;
            Origin = Rectangle.Empty;
            TilesetIndex = 0;
            Terrain3DInfo = null;
        }

        public DrawableTile(Rectangle Origin, int TilesetIndex)
        {
            this.Origin = Origin;
            this.TilesetIndex = TilesetIndex;
            ArraySubTile = new Rectangle[0];
            Terrain3DInfo = new Terrain3D();
        }

        public DrawableTile(DrawableTile TilePreset)
            : this()
        {
            this.Origin = TilePreset.Origin;
            this.TilesetIndex = TilePreset.TilesetIndex;
            this.ArraySubTile = new Rectangle[TilePreset.ArraySubTile.Length];
            for (int T = 0; T < TilePreset.ArraySubTile.Length; ++T)
            {
                ArraySubTile[T] = TilePreset.ArraySubTile[T];
            }
            if (TilePreset.Terrain3DInfo != null)
            {
                this.Terrain3DInfo = new Terrain3D(TilePreset.Terrain3DInfo);
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TilesetIndex);
            BW.Write(Origin.X);
            BW.Write(Origin.Y);
            if (ArraySubTile != null && ArraySubTile.Length > 0)
            {
                BW.Write(true);
                BW.Write((byte)ArraySubTile.Length);
                for (int T = 0; T < ArraySubTile.Length; ++T)
                {
                    BW.Write(ArraySubTile[T].X);
                    BW.Write(ArraySubTile[T].Y);
                }
            }
            else
            {
                BW.Write(false);
            }
            BW.Write(Terrain3DInfo != null);
            if (Terrain3DInfo != null)
            {
                Terrain3DInfo.Save(BW);
            }
        }
    }
}
