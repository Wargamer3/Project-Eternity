using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetMap2D : Map2D
    {
        private readonly SorcererStreetMap ActiveMap;

        public SorcererStreetMap2D(SorcererStreetMap Map)
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

        public SorcererStreetMap2D(SorcererStreetMap Map, BinaryReader BR)
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

        public override void DrawPlayers(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
                g.Draw(GameScreen.sprPixel,
                    new Rectangle((int)ActiveMap.ListPlayer[P].GamePiece.X * ActiveMap.TileSize.X, (int)ActiveMap.ListPlayer[P].GamePiece.Y * ActiveMap.TileSize.Y,
                    ActiveMap.TileSize.X / 2, ActiveMap.TileSize.Y / 2), Color.FromNonPremultiplied(127, 127, 127, 127));

                g.DrawString(ActiveMap.fntArial12, "P", new Vector2(ActiveMap.ListPlayer[P].GamePiece.X * ActiveMap.TileSize.X + 2, ActiveMap.ListPlayer[P].GamePiece.Y * ActiveMap.TileSize.Y), Color.Red);
            }

            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    if (ActiveMap.GetTerrain(X, Y, 0).DefendingCreature != null)
                    {
                        g.Draw(GameScreen.sprPixel,
                            new Rectangle((int)X * ActiveMap.TileSize.X + 16, (int)Y * ActiveMap.TileSize.Y,
                            ActiveMap.TileSize.X / 2, ActiveMap.TileSize.Y / 2), Color.FromNonPremultiplied(127, 127, 127, 127));

                        g.DrawString(ActiveMap.fntArial12, "C", new Vector2(X * ActiveMap.TileSize.X + 2 + 16, Y * ActiveMap.TileSize.Y), Color.Red);
                    }
                }
            }
        }

        public override void BeginDrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
            }
        }

        public override void DrawNightOverlay(CustomSpriteBatch g)
        {
            for (int P = 0; P < ActiveMap.ListPlayer.Count; P++)
            {
            }
        }
    }
}
