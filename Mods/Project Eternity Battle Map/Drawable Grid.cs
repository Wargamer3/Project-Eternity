using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public struct DrawableTile
    {
        public Rectangle Origin;//X, Y origin from at which the tile is located in the TileSet.
        public int TilesetIndex;

        public Terrain3D Terrain3DInfo;

        public DrawableTile(BinaryReader BR, int TileWidth, int TileHeight)
        {
            TilesetIndex = BR.ReadInt32();
            Origin = new Rectangle(BR.ReadInt32(), BR.ReadInt32(), TileWidth, TileHeight);
            bool HasTerrain3DInfo = BR.ReadBoolean();
            Terrain3DInfo = null;
            if (HasTerrain3DInfo)
            {
                Terrain3DInfo = new Terrain3D(BR, TileWidth, TileHeight);
            }
        }

        public DrawableTile(Rectangle Origin, int TilesetIndex)
        {
            this.Origin = Origin;
            this.TilesetIndex = TilesetIndex;
            Terrain3DInfo = new Terrain3D();
        }

        public DrawableTile(DrawableTile TilePreset)
            : this()
        {
            this.Origin = TilePreset.Origin;
            this.TilesetIndex = TilePreset.TilesetIndex;
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
            BW.Write(Terrain3DInfo != null);
            if (Terrain3DInfo != null)
            {
                Terrain3DInfo.Save(BW);
            }
        }
    }

    public interface DrawableGrid
    {
        void Save(BinaryWriter BW);
        void Load(BinaryReader BR);
        void RemoveTileset(int TilesetIndex);
    }

    public abstract class Map2D : DrawableGrid
    {
        private BattleMap Map;

        protected Point MapSize { get { return Map.MapSize; } }

        protected Point TileSize { get { return Map.TileSize; } }

        protected Vector3 CameraPosition { get { return Map.CameraPosition; } }

        public DrawableTile[,] ArrayTile;

        public float Depth;

        public Map2D(BattleMap Map)
        {
            this.Map = Map;
            Depth = 1f;
        }

        public void Save(BinaryWriter BW)
        {
            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y].Save(BW);
                }
            }
        }

        public void Load(BinaryReader BR)
        {
            ArrayTile = new DrawableTile[MapSize.X, MapSize.Y];

            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(BR, TileSize.X, TileSize.Y);
                }
            }
        }

        public DrawableTile GetTile(int X, int Y)
        {
            return ArrayTile[X, Y];
        }
        
        public void ReplaceTile(int X, int Y, DrawableTile ReplacementTile)
        {
            this.ArrayTile[X, Y] = ReplacementTile;
        }

        public void ReplaceGrid(DrawableTile[,] ReplacementGrid)
        {
            ArrayTile = ReplacementGrid;
        }

        public void ReplaceForegrounds(List<AnimationBackground> ListForegrounds)
        {
            ListForegrounds.Clear();
            ListForegrounds.AddRange(ListForegrounds);
        }

        public void RemoveTileset(int TilesetIndex)
        {
            for (int X = ArrayTile.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = ArrayTile.GetLength(1) - 1; Y >= 0; --Y)
                {
                    if (ArrayTile[X, Y].TilesetIndex > TilesetIndex)
                    {
                        --ArrayTile[X, Y].TilesetIndex;
                    }
                }
            }
        }
    }
}
