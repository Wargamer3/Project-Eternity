using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
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

        public void Reset()
        {
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
                    FogOfWarIntensityFinder.UpdatePath(new List<MovementAlgorithmTile>() { FogOfWarMap[(int)Map.ListPlayer[P].ListUnit[U].X, (int)Map.ListPlayer[P].ListUnit[U].Y] }, Map.ListPlayer[P].ListUnit[U].Components, Map.ListPlayer[P].ListUnit[U].UnitStat, 10, false);
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

        public void SetCrossfadeValue(double Value)
        {
            throw new NotImplementedException();
        }
    }

    public class FogOfWarTile : MovementAlgorithmTile
    {
        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public FogOfWarTile(int PosX, int PosY)
            : base(PosX, PosY, 0, 0)
        {
            WorldPosition.X = PosX;
            WorldPosition.Y = PosY;
            TerrainTypeIndex = 0;
        }
    }

    public class MovementAlgorithmFogOfWar : MovementAlgorithm
    {
        FogOfWarTile[,] FogOfWarMap;

        public MovementAlgorithmFogOfWar(FogOfWarTile[,] FogOfWarMap)
        {
            this.FogOfWarMap = FogOfWarMap;
        }

        protected override List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile StartingNode, float OffsetX, float OffsetY,
            UnitMapComponent MapComponent, UnitStats Stats, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListTerrainSuccessor = new List<MovementAlgorithmTile>();
            MovementAlgorithmTile ActiveTile = GetTile((int)(StartingNode.WorldPosition.X + OffsetX), (int)(StartingNode.WorldPosition.X + OffsetY), StartingNode.LayerIndex);
            //Wall
            if (ActiveTile == null || ActiveTile.MovementCost == -1
                || ActiveTile.TerrainTypeIndex == UnitStats.TerrainWallIndex || ActiveTile.TerrainTypeIndex == UnitStats.TerrainVoidIndex)
            {
                return ListTerrainSuccessor;
            }

            //If the NewNode is the parent, skip it.
            if (StartingNode.ParentTemp == null)
            {
                //Used for an undefined map or if you don't need to calculate the whole map.
                //ListSuccessors.Add(new AStarNode(ActiveNode, AX, AY));
                ActiveTile.ParentTemp = StartingNode;
                ListTerrainSuccessor.Add(ActiveTile);
            }

            return ListTerrainSuccessor;
        }

        public override float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo)
        {
            return 1;
        }

        public override MovementAlgorithmTile GetTile(int PosX, int PosY, int Layerindex)
        {
            if (PosX < 0 || PosY < 0 || PosX >= FogOfWarMap.GetLength(0) || PosY >= FogOfWarMap.GetLength(1))
            {
                return null;
            }

            return FogOfWarMap[PosX, PosY];
        }

        public override bool IsBlocked(MovementAlgorithmTile CurrentNode)
        {
            return false;
        }
    }
}
