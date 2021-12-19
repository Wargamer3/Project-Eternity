using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class DeathmatchMap2D : Map2D
    {
        private readonly DeathmatchMap ActiveMap;

        public DeathmatchMap2D(DeathmatchMap Map)
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

        public DeathmatchMap2D(DeathmatchMap Map, BinaryReader BR)
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

        public override void DrawPlayers(CustomSpriteBatch g, int LayerIndex)
        {
            DrawDelayedAttacks(g);

            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                for (int S = 0; S < ActiveMap.ListPlayer[P].ListSquad.Count; S++)
                {
                    if (ActiveMap.ListPlayer[P].ListSquad[S].LayerIndex == LayerIndex)
                    {
                        DrawUnitMap(g, ActiveMap.ListPlayer[P].Color, ActiveMap.ListPlayer[P].ListSquad[S], !ActiveMap.ListPlayer[P].ListSquad[S].CanMove && P == ActiveMap.ActivePlayerIndex);
                    }
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
