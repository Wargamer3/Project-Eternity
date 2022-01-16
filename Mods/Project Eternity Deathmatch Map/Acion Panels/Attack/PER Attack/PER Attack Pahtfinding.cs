using System;
using System.Linq;
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
            ListAllAttackTerrain.Add(Map.GetTerrain(ActiveSquad.Position.X, ActiveSquad.Position.Y, (int)ActiveSquad.Position.Z));
            ActiveTerrain.LayerIndex = (int)ActiveTerrain.Position.Z;

            //Start Right, go downward
            Vector2 TestedFinalPos = new Vector2(Map.MapSize.X, ActiveSquad.Position.Y);
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.Y < Map.MapSize.Y);

            //Start bottom right, go bottom left
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (--TestedFinalPos.X >= 0);

            //Start bottom left, go upper left
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (--TestedFinalPos.Y >= 0);

            //Start upper left, go upper right
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.X < Map.MapSize.X);

            //Start upper right, go back to start
            do
            {
                for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, L);
                }
            }
            while (++TestedFinalPos.Y < ActiveSquad.Position.Y);

            //Aim for the ceiling
            for (int X = 0; X < Map.MapSize.X; ++X)
            {
                for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                {
                    ProcessLine(ActiveTerrain, TestedFinalPos, Map.LayerManager.ListLayer.Count - 1);
                }
            }
        }

        private void ProcessLine(MovementAlgorithmTile StartNode, Vector2 DestinationPosition, int DestinationLayerIndex)
        {
            MovementAlgorithmTile CurrentTerrain = StartNode;
            MovementAlgorithmTile NextTerrain = Map.GetTerrain(StartNode.Position.X, StartNode.Position.Y, StartNode.LayerIndex);
            Vector3 Diff = new Vector3(DestinationPosition.X - StartNode.Position.X, DestinationPosition.Y - StartNode.Position.Y, DestinationLayerIndex - StartNode.LayerIndex);
            Diff.Normalize();
            int NextX = (int)StartNode.Position.X;
            int NextY = (int)StartNode.Position.Y;
            int NextZ = StartNode.LayerIndex;
            float Progress = 0;
            bool IsBlocked = false;

            while (NextTerrain != null)
            {
                Progress += 1;
                NextX = (int)(StartNode.Position.X + Progress * Diff.X);
                NextY = (int)(StartNode.Position.Y + Progress * Diff.Y);
                NextZ = (int)(StartNode.LayerIndex + Progress * Diff.Z);

                if (NextX < 0 || NextX >= Map.MapSize.X || NextY < 0 || NextY >= Map.MapSize.Y || NextZ < 0 || NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    return;
                }

                NextTerrain = Map.GetTerrain(NextX, NextY, NextZ);

                if (ListRefusedAttackTerrain.Contains(NextTerrain))
                {
                    return;
                }

                if (!ListAllAttackTerrain.Contains(NextTerrain))
                {
                    NextTerrain.LayerIndex = NextZ;
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
