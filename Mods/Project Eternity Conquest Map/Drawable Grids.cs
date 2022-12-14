using System;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestMap2D : Map2D
    {
        private readonly ConquestMap ActiveMap;

        public ConquestMap2D(ConquestMap Map)
            : base(Map)
        {
            ActiveMap = Map;

            DrawableTile[,] ArrayTile = new DrawableTile[MapSize.X, MapSize.Y];

            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(new Rectangle(0, 0, ActiveMap.TileSize.X, ActiveMap.TileSize.Y), 0);
                }
            }

            ReplaceGrid(ArrayTile);
        }

        public ConquestMap2D(ConquestMap Map, BinaryReader BR)
            : base(Map)
        {
            ActiveMap = Map;

            DrawableTile[,] ArrayTile = new DrawableTile[MapSize.X, MapSize.Y];

            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(new Rectangle(0, 0, ActiveMap.TileSize.X, ActiveMap.TileSize.Y), 0);
                }
            }

            ReplaceGrid(ArrayTile);

            Load(BR);
        }

        public void DrawPlayers(CustomSpriteBatch g, int LayerIndex)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    //DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListUnit[S].Components, !ActiveMap.ListPlayer[P].ListUnit[S].CanMove && P == ActiveMap.ActivePlayerIndex);
                }
            }
        }
    }
}
