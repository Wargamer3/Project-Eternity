using System.Collections.Generic;
using ProjectEternity.Core.Units;

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

        private MovementAlgorithmTile AddSuccessor(MovementAlgorithmTile ActiveNode, int MaxMovement, float AX, float AY, int LayerIndex)
        {
            MovementAlgorithmTile ActiveTile = GetTile(AX, AY, LayerIndex);

            //Wall
            if (ActiveTile == null || ActiveTile.MVEnterCost == -1 || ActiveTile.MovementCost == -1)
            {
                return null;
            }

            //If the NewNode is the parent, skip it.
            if (ActiveNode.Parent == null || ActiveNode.Position.X != AX || ActiveNode.Position.Y != AY)
            {
                //Used for an undefined map or if you don't need to calculate the whole map.
                //ListSuccessors.Add(new AStarNode(ActiveNode, AX, AY));
                ActiveTile.Parent = ActiveNode;
                return ActiveTile;
            }

            return null;
        }

        private List<MovementAlgorithmTile> GetSuccessors(MovementAlgorithmTile ActiveNode, int MaxMovement, int LayerIndex)
        {
            List<MovementAlgorithmTile> ListSuccessors = new List<MovementAlgorithmTile>();

            if (ActiveNode.MovementCost >= MaxMovement)
            {
                return ListSuccessors;
            }

            MovementAlgorithmTile Successor = null;
            if ((Successor = AddSuccessor(ActiveNode, MaxMovement, ActiveNode.Position.X - 1, ActiveNode.Position.Y, LayerIndex)) != null)
                ListSuccessors.Add(Successor);
            if ((Successor = AddSuccessor(ActiveNode, MaxMovement, ActiveNode.Position.X + 1, ActiveNode.Position.Y, LayerIndex)) != null)
                ListSuccessors.Add(Successor);
            if ((Successor = AddSuccessor(ActiveNode, MaxMovement, ActiveNode.Position.X, ActiveNode.Position.Y - 1, LayerIndex)) != null)
                ListSuccessors.Add(Successor);
            if ((Successor = AddSuccessor(ActiveNode, MaxMovement, ActiveNode.Position.X, ActiveNode.Position.Y + 1, LayerIndex)) != null)
                ListSuccessors.Add(Successor);

            //Diagonal movement
            //AddSuccessor(ActiveNode, ActiveNode.XPos + 1, ActiveNode.YPos + 1);
            //AddSuccessor(ActiveNode, ActiveNode.XPos + 1, ActiveNode.YPos - 1);
            //AddSuccessor(ActiveNode, ActiveNode.XPos - 1, ActiveNode.YPos + 1);
            //AddSuccessor(ActiveNode, ActiveNode.XPos - 1, ActiveNode.YPos - 1);

            return ListSuccessors;
        }

        public List<MovementAlgorithmTile> FindPath(MovementAlgorithmTile AStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, int MaxMovement)
        {
            ResetNodes();

            return UpdatePath(AStartNode, MapComponent, UnitStat, MaxMovement);
        }

        public List<MovementAlgorithmTile> UpdatePath(MovementAlgorithmTile AStartNode, UnitMapComponent MapComponent, UnitStats UnitStat, int MaxMovement)
        {
            MovementAlgorithmTile CurrentNode;

            ListOpenNode.Add(AStartNode);
            ListAllNode.Add(AStartNode);

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
                List<MovementAlgorithmTile> ListSuccessors = GetSuccessors(CurrentNode, MaxMovement, MapComponent.LayerIndex);
                foreach (MovementAlgorithmTile Neighbor in ListSuccessors)
                {
                    if (!ListAllNode.Contains(Neighbor))
                        ListAllNode.Add(Neighbor);

                    //Cost to move to this Neighbor
                    float MovementCostToNeighbor = CurrentNode.MovementCost + GetMVCost(MapComponent, UnitStat, CurrentNode, Neighbor);

                    //Bad path with higher movement cost then it already has.
                    if (ListCloseNode.Contains(Neighbor) && MovementCostToNeighbor >= Neighbor.MovementCost)
                        continue;

                    //New path or Neighbor have a lower movement cost then before.
                    if (!ListOpenNode.Contains(Neighbor) || MovementCostToNeighbor < Neighbor.MovementCost)
                    {
                        Neighbor.Parent = CurrentNode;
                        Neighbor.MovementCost = MovementCostToNeighbor;

                        if (!ListOpenNode.Contains(Neighbor))
                            ListOpenNode.Add(Neighbor);
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

        public abstract float GetMVCost(UnitMapComponent MapComponent, UnitStats UnitStat, MovementAlgorithmTile CurrentNode, MovementAlgorithmTile TerrainToGo);

        public abstract MovementAlgorithmTile GetTile(float PosX, float PosY, int LayerIndex);
    }
}