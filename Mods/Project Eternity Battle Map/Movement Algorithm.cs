using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using static ProjectEternity.GameScreens.BattleMapScreen.MovementAlgorithmTile;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public abstract class MovementAlgorithm
    {
        private readonly List<MovementAlgorithmTile> ListOpenNode;
        private readonly List<MovementAlgorithmTile> ListCloseNode;
        private readonly List<MovementAlgorithmTile> ListAllNode;

        public MovementAlgorithm()
        {
            ListOpenNode = new List<MovementAlgorithmTile>();
            ListCloseNode = new List<MovementAlgorithmTile>();
            ListAllNode = new List<MovementAlgorithmTile>();
        }

        private List<MovementAlgorithmTile> GetSuccessors(MovementAlgorithmTile StartingNode, int MaxMovement, UnitMapComponent MapComponent, UnitStats UnitStat, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListSuccessors = new List<MovementAlgorithmTile>();

            if (StartingNode.MovementCost >= MaxMovement)
            {
                return ListSuccessors;
            }

            ListSuccessors.AddRange(AddSuccessor(StartingNode, -1, 0, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 1, 0, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 0, -1, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 0, 1, MapComponent, UnitStat, IgnoreObstacles));

            return ListSuccessors;
        }

        private List<MovementAlgorithmTile> GetSuccessors(MovementAlgorithmTile StartingNode, UnitMapComponent MapComponent, UnitStats UnitStat, bool IgnoreObstacles)
        {
            List<MovementAlgorithmTile> ListSuccessors = new List<MovementAlgorithmTile>();

            ListSuccessors.AddRange(AddSuccessor(StartingNode, -1, 0, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 1, 0, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 0, -1, MapComponent, UnitStat, IgnoreObstacles));
            ListSuccessors.AddRange(AddSuccessor(StartingNode, 0, 1, MapComponent, UnitStat, IgnoreObstacles));

            return ListSuccessors;
        }

        public List<MovementAlgorithmTile> FindPath(List<MovementAlgorithmTile> ListAStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, int MaxMovement, bool IgnoreObstacles)
        {
            ResetNodes();

            return UpdatePath(ListAStartNode, MapComponent, UnitStat, MaxMovement, IgnoreObstacles);
        }

        public List<MovementAlgorithmTile> FindPath(List<MovementAlgorithmTile> ListAStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, Vector3 EndPosition, bool IgnoreObstacles)
        {
            ResetNodes();

            return UpdatePath(ListAStartNode, MapComponent, UnitStat, EndPosition, IgnoreObstacles);
        }

        public List<MovementAlgorithmTile> UpdatePath(List<MovementAlgorithmTile> ListAStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, int MaxMovement, bool IgnoreObstacles)
        {
            MovementAlgorithmTile CurrentNode;

            foreach (MovementAlgorithmTile AStartNode in ListAStartNode)
            {
                ListOpenNode.Add(AStartNode);
                ListAllNode.Add(AStartNode);
            }

            while (ListOpenNode.Count > 0)
            {
                //Use the node with the lowest cost.(Sort it first)
                CurrentNode = ListOpenNode[0];
                int Lowest = 0;
                for (int i = 1; i < ListOpenNode.Count; i++)
                {
                    if (ListOpenNode[i].MovementCost < CurrentNode.MovementCost)
                    {
                        CurrentNode = ListOpenNode[i];
                        Lowest = i;
                    }
                }

                ListOpenNode.RemoveAt(Lowest);
                ListCloseNode.Add(CurrentNode);

                // Get successors to the current node
                List<MovementAlgorithmTile> ListSuccessors = GetSuccessors(CurrentNode, MaxMovement, MapComponent, UnitStat, IgnoreObstacles);
                foreach (MovementAlgorithmTile Neighbor in ListSuccessors)
                {
                    //Cost to move to this Neighbor
                    float MovementCostToNeighbor = GetMVCost(MapComponent, UnitStat, CurrentNode, Neighbor);
                    if (MovementCostToNeighbor < 0)
                    {
                        continue;
                    }

                    if (!ListAllNode.Contains(Neighbor))
                        ListAllNode.Add(Neighbor);

                    MovementCostToNeighbor += CurrentNode.MovementCost;

                    //Bad path with higher movement cost then it already has.
                    if (ListCloseNode.Contains(Neighbor) && MovementCostToNeighbor >= Neighbor.MovementCost)
                        continue;

                    //New path or Neighbor have a lower movement cost then before.
                    if (!ListOpenNode.Contains(Neighbor) || MovementCostToNeighbor < Neighbor.MovementCost)
                    {
                        if (Neighbor.ParentTemp == null || CurrentNode.ParentTemp == null || Neighbor.ParentTemp.WorldPosition.Z == CurrentNode.ParentTemp.WorldPosition.Z)
                        {
                            Neighbor.ParentTemp = CurrentNode;
                            Neighbor.ParentReal = CurrentNode;
                        }
                        Neighbor.MovementCost = MovementCostToNeighbor;

                        if (!ListOpenNode.Contains(Neighbor))
                            ListOpenNode.Add(Neighbor);
                    }
                }
            }

            return new List<MovementAlgorithmTile>(ListAllNode);
        }

        public List<MovementAlgorithmTile> UpdatePath(List<MovementAlgorithmTile> ListAStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, Vector3 EndPosition, bool IgnoreObstacles)
        {
            MovementAlgorithmTile CurrentNode;

            foreach (MovementAlgorithmTile AStartNode in ListAStartNode)
            {
                ListOpenNode.Add(AStartNode);
                ListAllNode.Add(AStartNode);
            }

            while (ListOpenNode.Count > 0)
            {
                //Use the node with the lowest cost.(Sort it first)
                CurrentNode = ListOpenNode[0];
                int Lowest = 0;
                for (int i = 1; i < ListOpenNode.Count; i++)
                {
                    if (ListOpenNode[i].MovementCost < CurrentNode.MovementCost)
                    {
                        CurrentNode = ListOpenNode[i];
                        Lowest = i;
                    }
                }

                ListOpenNode.RemoveAt(Lowest);
                ListCloseNode.Add(CurrentNode);

                // Get successors to the current node
                List<MovementAlgorithmTile> ListSuccessors = GetSuccessors(CurrentNode, MapComponent, UnitStat, IgnoreObstacles);
                foreach (MovementAlgorithmTile Neighbor in ListSuccessors)
                {
                    //Cost to move to this Neighbor
                    float MovementCostToNeighbor = GetMVCost(MapComponent, UnitStat, CurrentNode, Neighbor);
                    if (MovementCostToNeighbor < 0)
                    {
                        continue;
                    }

                    if (!ListAllNode.Contains(Neighbor))
                        ListAllNode.Add(Neighbor);

                    MovementCostToNeighbor += CurrentNode.MovementCost;

                    //Bad path with higher movement cost then it already has.
                    if (ListCloseNode.Contains(Neighbor) && MovementCostToNeighbor >= Neighbor.MovementCost)
                        continue;

                    //New path or Neighbor have a lower movement cost then before.
                    if (!ListOpenNode.Contains(Neighbor) || MovementCostToNeighbor < Neighbor.MovementCost)
                    {
                        if (Neighbor.ParentTemp == null || CurrentNode.ParentTemp == null || Neighbor.ParentTemp.WorldPosition.Z == CurrentNode.ParentTemp.WorldPosition.Z)
                        {
                            Neighbor.ParentTemp = CurrentNode;
                            Neighbor.ParentReal = CurrentNode;
                        }
                        Neighbor.MovementCost = MovementCostToNeighbor;

                        if (!ListOpenNode.Contains(Neighbor))
                            ListOpenNode.Add(Neighbor);
                    }

                    if (Neighbor.WorldPosition.X == EndPosition.X && Neighbor.WorldPosition.Y == EndPosition.Y && Neighbor.LayerIndex == EndPosition.Z)
                    {
                        return ListAllNode;
                    }
                }
            }

            return ListAllNode;
        }

        public void ResetNodes()
        {
            ListOpenNode.Clear();
            ListCloseNode.Clear();
            ListAllNode.Clear();
        }

        protected abstract List<MovementAlgorithmTile> AddSuccessor(MovementAlgorithmTile StartingNode, float OffsetX, float OffsetY, UnitMapComponent MapComponent, UnitStats UnitStat, bool IgnoreObstacles);

        public abstract float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo);

        public abstract MovementAlgorithmTile GetTile(int PosX, int PosY, int LayerIndex);

        public abstract bool IsBlocked(MovementAlgorithmTile CurrentNode);
    }
}