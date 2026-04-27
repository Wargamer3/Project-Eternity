using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.UnitTests.AI
{
    public class InterMapMovementAlgorithm
    {
        private readonly List<MapInfo> ListOpenNode;
        private readonly List<MapInfo> ListCloseNode;
        private readonly List<MapInfo> ListAllNode;
        private readonly List<MapInfo> ListFoundNode;

        public InterMapMovementAlgorithm()
        {
            ListOpenNode = new List<MapInfo>();
            ListCloseNode = new List<MapInfo>();
            ListAllNode = new List<MapInfo>();
            ListFoundNode = new List<MapInfo>();
        }

        private List<MapInfo> GetSuccessors(MapInfo StartingNode)
        {
            List<MapInfo> ListSuccessors = new List<MapInfo>();
            foreach (MapInfo ActiveMapInfo in StartingNode.DicNestedMapByName.Values)
            {
                ListSuccessors.Add(ActiveMapInfo);
            }

            return ListSuccessors;
        }

        public List<MapInfo> FindPath(MapInfo RootNode, string EndPosition)
        {
            ResetNodes();

            return UpdatePath(RootNode, EndPosition);
        }

        public List<MapInfo> UpdatePath(MapInfo RootNode, string EndPosition)
        {
            MapInfo CurrentNode;

            ListOpenNode.Add(RootNode);
            ListAllNode.Add(RootNode);
            ListFoundNode.Add(RootNode);

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
                List<MapInfo> ListSuccessors = GetSuccessors(CurrentNode);
                foreach (MapInfo Neighbor in ListSuccessors)
                {
                    //Cost to move to this Neighbor
                    float MovementCostToNeighbor = GetMVCost(Neighbor);
                    if (MovementCostToNeighbor < 0)
                    {
                        continue;
                    }

                    if (!ListAllNode.Contains(Neighbor))
                    {
                        ListAllNode.Add(Neighbor);

                        if (Neighbor.MapMapContainer.AreaName == EndPosition)
                        {
                            ListFoundNode.Add(Neighbor);
                        }
                        MapInfo FoundLocation = Neighbor.FindOwnerMap(EndPosition);
                        if (FoundLocation != null)
                        {
                            ListFoundNode.Add(Neighbor);
                        }
                    }

                    MovementCostToNeighbor += CurrentNode.MovementCost;

                    //Bad path with higher movement cost then it already has.
                    if (ListCloseNode.Contains(Neighbor) && MovementCostToNeighbor >= Neighbor.MovementCost)
                        continue;

                    //New path or Neighbor have a lower movement cost then before.
                    if (!ListOpenNode.Contains(Neighbor) || MovementCostToNeighbor < Neighbor.MovementCost)
                    {
                        if (Neighbor.ParentTemp == null || CurrentNode.ParentTemp == null)
                        {
                            Neighbor.ParentTemp = CurrentNode;
                        }
                        Neighbor.MovementCost = MovementCostToNeighbor;

                        if (!ListOpenNode.Contains(Neighbor))
                            ListOpenNode.Add(Neighbor);
                    }
                }
            }

            Filter();

            return ListFoundNode;
        }

        public void ResetNodes()
        {
            ListOpenNode.Clear();
            ListCloseNode.Clear();
            ListAllNode.Clear();
            ListFoundNode.Clear();
        }

        private void Filter()
        {
            foreach (MapInfo ActiveNode in ListFoundNode)
            {
                ActiveNode.ParentTemp = null;
                ActiveNode.MovementCost = 0;
            }
        }

        public float GetMVCost(MapInfo TerrainToGo)
        {
            return 1;
        }
    }
}