﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class WorldMap2D : Map2D
    {
        private readonly WorldMap ActiveMap;

        public WorldMap2D(WorldMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds)
            : base(Map, ListBackgrounds, ListForegrounds)
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

        public WorldMap2D(WorldMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds, BinaryReader BR)
            : this(Map, ListBackgrounds, ListForegrounds)
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
                    DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListUnit[S], !ActiveMap.ListPlayer[P].ListUnit[S].CanMove && P == ActiveMap.ActivePlayerIndex);
                }

                for (int C = 0; C < ActiveMap.ListPlayer[P].ListConstruction.Count; C++)
                {
                    DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListConstruction[C], !ActiveMap.ListPlayer[P].ListConstruction[C].CanMove && P == ActiveMap.ActivePlayerIndex);
                }
            }
        }

        public override void BeginDrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    ActiveMap.ListPlayer[P].ListUnit[S].DrawTimeOfDayOverlayOnMap(g, ActiveMap.ListPlayer[P].ListUnit[S].Position, 24);
                }
            }
        }

        public override void DrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListUnit.Count; S++)
                {
                    ActiveMap.ListPlayer[P].ListUnit[S].DrawOverlayOnMap(g, ActiveMap.ListPlayer[P].ListUnit[S].Position);
                }
            }
        }
    }
}
