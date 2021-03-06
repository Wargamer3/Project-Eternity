﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class FogOfWarGridOverlay : BattleMapOverlay
    {
        MovementAlgorithmFogOfWar FogOfWarIntensityFinder;
        ConquestMap Map;
        FogOfWarTile[,] FogOfWarMap;

        public FogOfWarGridOverlay(ConquestMap Map)
        {
            this.Map = Map;
            FogOfWarMap = new FogOfWarTile[Map.MapSize.X, Map.MapSize.Y];
            FogOfWarIntensityFinder = new MovementAlgorithmFogOfWar(FogOfWarMap);
            for (int X = FogOfWarMap.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = FogOfWarMap.GetLength(1) - 1; Y >= 0; --Y)
                {
                    FogOfWarMap[X, Y] = new FogOfWarTile(X, Y);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            FogOfWarIntensityFinder.ResetNodes();

            for (int X = FogOfWarMap.GetLength(0) - 1; X >= 0; --X)
            {
                for (int Y = FogOfWarMap.GetLength(1) - 1; Y >= 0; --Y)
                {
                    FogOfWarMap[X, Y].MovementCost = 50;
                }
            }
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int U = 0; U < Map.ListPlayer[P].ListUnit.Count; U++)
                {
                    FogOfWarMap[(int)Map.ListPlayer[P].ListUnit[U].X, (int)Map.ListPlayer[P].ListUnit[U].Y].MovementCost = -1;
                }
            }

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                for (int U = 0; U < Map.ListPlayer[P].ListUnit.Count; U++)
                {
                    FogOfWarIntensityFinder.UpdatePath(FogOfWarMap[(int)Map.ListPlayer[P].ListUnit[U].X, (int)Map.ListPlayer[P].ListUnit[U].Y], Map.ListPlayer[P].ListUnit[U].Components, Map.ListPlayer[P].ListUnit[U].UnitStat, 10);
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            for (int X = FogOfWarMap.GetLength(0) - 1; X >= 0 ; --X)
            {
                for (int Y = FogOfWarMap.GetLength(1) - 1; Y >= 0; --Y)
                {
                    if (FogOfWarMap[X, Y].MovementCost >= 5)
                        g.Draw(GameScreen.sprPixel, new Rectangle(X * Map.TileSize.X, Y * Map.TileSize.Y, Map.TileSize.X, Map.TileSize.Y), Color.FromNonPremultiplied(0, 0, 0, 255));
                }
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }

    public class FogOfWarTile : MovementAlgorithmTile
    {
        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public FogOfWarTile(int PosX, int PosY)
        {
            Position.X = PosX;
            Position.Y = PosY;
            TerrainTypeIndex = 0;
            MVEnterCost = 0;
            MVMoveCost = 1;
        }
    }

    public class MovementAlgorithmFogOfWar : MovementAlgorithm
    {
        FogOfWarTile[,] FogOfWarMap;

        public MovementAlgorithmFogOfWar(FogOfWarTile[,] FogOfWarMap)
        {
            this.FogOfWarMap = FogOfWarMap;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            return 1;
        }

        public override MovementAlgorithmTile GetTile(float PosX, float PosY, int Layerindex)
        {
            if (PosX < 0 || PosY < 0 || PosX >= FogOfWarMap.GetLength(0) || PosY >= FogOfWarMap.GetLength(1))
            {
                return null;
            }

            return FogOfWarMap[(int)PosX, (int)PosY];
        }
    }
}
