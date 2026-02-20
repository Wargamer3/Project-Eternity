using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public bool CheckForObstacleAtPosition(int PlayerIndex, Vector3 WorldPosition, Vector3 Displacement)
        {
            return CheckForSquadAtPosition(PlayerIndex, WorldPosition, Displacement) >= 0;
        }

        public override bool CheckForObstacleAtPosition(Vector3 WorldPosition, Vector3 Displacement)
        {
            bool ObstacleFound = false;

            for (int P = 0; P < ListPlayer.Count && !ObstacleFound; P++)
                ObstacleFound = CheckForObstacleAtPosition(P, WorldPosition, Displacement);

            return ObstacleFound;
        }

        public int CheckForSquadAtPosition(int PlayerIndex, Vector3 WorldPosition, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListSquad.Count == 0)
                return -1;

            Vector3 FinalPosition = WorldPosition + new Vector3(Displacement.X * TileSize.X, Displacement.Y * TileSize.Y, Displacement.Z * LayerHeight);

            if (!IsInsideMap(FinalPosition))
                return -1;

            int S = 0;
            bool SquadFound = false;
            //Check if there's a Construction.
            while (S < ListPlayer[PlayerIndex].ListSquad.Count && !SquadFound)
            {
                if (ListPlayer[PlayerIndex].ListSquad[S].CurrentLeader == null || ListPlayer[PlayerIndex].ListSquad[S].IsDead)
                {
                    ++S;
                    continue;
                }
                if (ListPlayer[PlayerIndex].ListSquad[S].IsUnitAtPosition(FinalPosition, TileSize))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }
    }
}
