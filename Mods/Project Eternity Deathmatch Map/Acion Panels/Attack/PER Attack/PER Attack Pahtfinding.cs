using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    class PERAttackPahtfinding
    {
        private readonly DeathmatchMap Map;
        private readonly Squad ActiveSquad;

        HashSet<MovementAlgorithmTile> ListAllAttackTerrain = new HashSet<MovementAlgorithmTile>();
        HashSet<MovementAlgorithmTile> ListRefusedAttackTerrain = new HashSet<MovementAlgorithmTile>();
        public List<MovementAlgorithmTile> ListUsableAttackTerrain = new List<MovementAlgorithmTile>();

        public PERAttackPahtfinding(DeathmatchMap Map, Squad ActiveSquad)
        {
            this.Map = Map;
            this.ActiveSquad = ActiveSquad;
        }

        public void ComputeAttackTargets()
        {
            ListAllAttackTerrain.Clear();
            ListRefusedAttackTerrain.Clear();
            ListUsableAttackTerrain.Clear();

            MovementAlgorithmTile ActiveTerrain = Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z);
            Vector3 StartPosition = ActiveSquad.Position + new Vector3(0.5f, 0.5f, 0.5f);
            ListAllAttackTerrain.Add(Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));

            //Start Right, go downward
            Vector2 TestedFinalPos = new Vector2(Map.MapSize.X + 0.5f, ActiveSquad.Position.Y + 0.5f);
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(StartPosition, ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.Y < Map.MapSize.Y);

            //Start bottom right, go bottom left
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(StartPosition, ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (--TestedFinalPos.X >= 0);

            //Start bottom left, go upper left
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(StartPosition, ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (--TestedFinalPos.Y >= 0);

            //Start upper left, go upper right
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(StartPosition, ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.X < Map.MapSize.X);

            //Start upper right, go back to start
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(StartPosition, ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.Y < ActiveSquad.Position.Y);

            //Aim for the ceiling
            for (int X = 0; X < Map.MapSize.X; ++X)
            {
                for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                {
                    ProcessLine(StartPosition, ActiveTerrain, new Vector2(X, Y), Map.LayerManager.ListLayer.Count - 1);
                }
            }
        }

        private void ProcessLine(Vector3 StartPosition, MovementAlgorithmTile StartNode, Vector2 DestinationPosition, int DestinationLayerIndex)
        {
            MovementAlgorithmTile CurrentTerrain = StartNode;
            MovementAlgorithmTile NextTerrain = Map.GetTerrain(StartPosition.X, StartPosition.Y, (int)StartPosition.Z);
            Vector3 Diff = new Vector3(DestinationPosition.X - StartPosition.X, DestinationPosition.Y - StartPosition.Y, DestinationLayerIndex - StartPosition.Z);
            Diff.Normalize();

            float Progress = 0;

            int LastX = (int)StartPosition.X;
            int LastY = (int)StartPosition.Y;
            int LastZ = (int)StartPosition.Z;
            int CurrentX = (int)StartPosition.X;
            int CurrentY = (int)StartPosition.Y;
            int CurrentZ = (int)StartPosition.Z;
            int NextX;
            int NextY;
            int NextZ;

            while (NextTerrain != null)
            {
                Progress += 1;
                NextX = (int)(StartPosition.X + Progress * Diff.X);
                NextY = (int)(StartPosition.Y + Progress * Diff.Y);
                NextZ = (int)(StartPosition.Z + Progress * Diff.Z);

                if (NextX < 0 || NextX >= Map.MapSize.X || NextY < 0 || NextY >= Map.MapSize.Y || NextZ < 0 || NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    return;
                }

                #region Corners

                if (CurrentX != NextX && (CurrentY != NextY || CurrentZ != NextZ)
                    || CurrentY != NextY && (CurrentX != NextX || CurrentZ != NextZ)
                    || CurrentZ != NextZ && (CurrentX != NextX || CurrentY != NextY))
                {
                    int CornerNextX = NextX;
                    int CornerNextY = NextY;
                    int CornerNextZ = NextZ;

                    if (LastX == CurrentX)
                    {
                        CornerNextX = CurrentX;
                    }
                    else if (LastY == CurrentY)
                    {
                        CornerNextY = CurrentY;
                    }
                    else
                    {
                        CornerNextZ = CurrentZ;
                    }

                    if (CornerNextX < 0 || CornerNextX >= Map.MapSize.X || CornerNextY < 0 || CornerNextY >= Map.MapSize.Y || CornerNextZ < 0 || CornerNextZ >= Map.LayerManager.ListLayer.Count)
                    {
                        break;
                    }

                    NextTerrain = Map.GetTerrain(CornerNextX, CornerNextY, CornerNextZ);

                    CurrentX = CornerNextX;
                    CurrentY = CornerNextY;
                    CurrentZ = CornerNextZ;

                    if (ListRefusedAttackTerrain.Contains(NextTerrain))
                    {
                        return;
                    }

                    if (!ListAllAttackTerrain.Contains(NextTerrain))
                    {
                        ListAllAttackTerrain.Add(NextTerrain);

                        if (IsTerrainBlocking(NextTerrain))
                        {
                            ListRefusedAttackTerrain.Add(NextTerrain);
                            ListUsableAttackTerrain.Add(CurrentTerrain);
                            return;
                        }
                        else if (IsTerrainAttackable(NextTerrain))
                        {
                            ListUsableAttackTerrain.Add(NextTerrain);
                        }
                    }
                }

                #endregion

                NextTerrain = Map.GetTerrain(NextX, NextY, NextZ);

                if (ListRefusedAttackTerrain.Contains(NextTerrain))
                {
                    return;
                }

                if (!ListAllAttackTerrain.Contains(NextTerrain))
                {
                    ListAllAttackTerrain.Add(NextTerrain);

                    if (IsTerrainBlocking(NextTerrain))
                    {
                        ListRefusedAttackTerrain.Add(NextTerrain);
                        ListUsableAttackTerrain.Add(CurrentTerrain);
                        return;
                    }
                    else if (IsTerrainAttackable(NextTerrain))
                    {
                        ListUsableAttackTerrain.Add(NextTerrain);
                    }
                }

                LastX = CurrentX;
                LastY = CurrentY;
                LastZ = CurrentZ;

                CurrentX = NextX;
                CurrentY = NextY;
                CurrentZ = NextZ;

                CurrentTerrain = NextTerrain;
            }
        }

        private bool IsTerrainAttackable(MovementAlgorithmTile ActiveTerrain)
        {
            return ActiveTerrain.TerrainTypeIndex == UnitStats.TerrainLandIndex;
        }

        private bool IsTerrainBlocking(MovementAlgorithmTile ActiveTerrain)
        {
            return ActiveTerrain.TerrainTypeIndex == UnitStats.TerrainWallIndex;
        }

    }
}
