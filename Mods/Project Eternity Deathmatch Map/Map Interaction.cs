using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public char GetTerrainLetterAttribute(UnitStats UnitStat, byte MovementTypeIndex)
        {
            return Grades[UnitStat.TerrainAttributeValue(MovementTypeIndex)];
        }

        public bool CheckForObstacleAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            return CheckForSquadAtPosition(PlayerIndex, Position, Displacement) >= 0;
        }

        public override bool CheckForObstacleAtPosition(Vector3 Position, Vector3 Displacement)
        {
            bool ObstacleFound = false;

            for (int P = 0; P < ListPlayer.Count && !ObstacleFound; P++)
                ObstacleFound = CheckForObstacleAtPosition(P, Position, Displacement);

            return ObstacleFound;
        }

        public int CheckForSquadAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListSquad.Count == 0)
                return -1;

            Vector3 FinalPosition = Position + new Vector3(Displacement.X * TileSize.X, Displacement.Y * TileSize.Y, Displacement.Z * LayerHeight);

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
