using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchMap2D : Map2D
    {
        private readonly DeathmatchMap ActiveMap;

        public DeathmatchMap2D(DeathmatchMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds)
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

        public DeathmatchMap2D(DeathmatchMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds, BinaryReader BR)
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

            Load(BR);
        }

        public override void DrawPlayers(CustomSpriteBatch g)
        {
            DrawDelayedAttacks(g);

            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListSquad.Count; S++)
                {
                    DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListSquad[S], !ActiveMap.ListPlayer[P].ListSquad[S].CanMove && P == ActiveMap.ActivePlayerIndex);
                }
            }
        }

        public override void BeginDrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListSquad.Count; S++)
                {
                    if (ActiveMap.ListPlayer[P].ListSquad[S].CurrentLeader != null)
                    {
                        ActiveMap.ListPlayer[P].ListSquad[S].DrawTimeOfDayOverlayOnMap(g, ActiveMap.ListPlayer[P].ListSquad[S].Position, 24);
                    }
                }
            }
        }

        public override void DrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListSquad.Count; S++)
                {
                    ActiveMap.ListPlayer[P].ListSquad[S].DrawOverlayOnMap(g, ActiveMap.ListPlayer[P].ListSquad[S].Position);
                }
            }
        }

        private void DrawDelayedAttacks(CustomSpriteBatch g)
        {
            int BorderX = (int)(TileSize.X * 0.1);
            int BorderY = (int)(TileSize.Y * 0.1);

            foreach (DelayedAttack ActiveAttack in ActiveMap.ListDelayedAttack)
            {
                foreach (Vector3 ActivePosition in ActiveAttack.ListAttackPosition)
                {
                    g.Draw(GameScreen.sprPixel,
                        new Rectangle(
                            (int)(ActivePosition.X - CameraPosition.X) * TileSize.X + BorderX,
                            (int)(ActivePosition.Y - CameraPosition.Y) * TileSize.Y + BorderY,
                            TileSize.X - BorderX * 2,
                            TileSize.Y - BorderY * 2), Color.FromNonPremultiplied(139, 0, 0, 190));
                }
            }
        }
    }
}
