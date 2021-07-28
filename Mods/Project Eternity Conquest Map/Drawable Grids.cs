using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestMap2D : Map2D
    {
        private readonly ConquestMap ActiveMap;

        public ConquestMap2D(ConquestMap Map, BattleMapOverlay MapOverlay)
            : base(Map, MapOverlay)
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

        public ConquestMap2D(ConquestMap Map, BattleMapOverlay MapOverlay, BinaryReader BR)
            : base(Map, MapOverlay)
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

        public override void DrawPlayers(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListUnit[S].Components, !ActiveMap.ListPlayer[P].ListUnit[S].CanMove && P == ActiveMap.ActivePlayerIndex);
                }
            }
        }

        public override void BeginDrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    ActiveMap.ListPlayer[P].ListUnit[S].Components.DrawTimeOfDayOverlayOnMap(g, ActiveMap.ListPlayer[P].ListUnit[S].Position, 24);
                }
            }
        }

        public override void DrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    ActiveMap.ListPlayer[P].ListUnit[S].Components.DrawOverlayOnMap(g, ActiveMap.ListPlayer[P].ListUnit[S].Position);
                }
            }
        }
    }
}
