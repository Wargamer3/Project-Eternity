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

            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X >= MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y >= MapSize.Y)
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
                if (ListPlayer[PlayerIndex].ListSquad[S].IsUnitAtPosition(FinalPosition))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }
        
        public bool UpdateMapNavigation(PlayerInput ActiveInputManager)
        {
            bool CursorMoved = CursorControl(ActiveInputManager);//Move the cursor
            if (CursorMoved)
            {
                foreach (TeleportPoint ActiveTeleport in LayerManager.ListLayer[(int)CursorPosition.Z].ListTeleportPoint)
                {
                    if (ActiveTeleport.Position.X == CursorPosition.X && ActiveTeleport.Position.Y == CursorPosition.Y)
                    {
                        CursorPosition.X = ActiveTeleport.OtherMapEntryPoint.X;
                        CursorPosition.Y = ActiveTeleport.OtherMapEntryPoint.Y;
                        CursorPosition.Z = ActiveTeleport.OtherMapEntryLayer;
                        break;
                    }
                }
            }

            if (InputHelper.InputLButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListSquad.Count == 0)
                    return CursorMoved;

                int UnitIndex = 0;
                if (ActiveSquad != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad);

                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    ++UnitIndex;

                    if (UnitIndex >= ListPlayer[ActivePlayerIndex].ListSquad.Count)
                        UnitIndex = 0;

                    if (ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CurrentLeader != null && ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (++UnitIndex >= ListPlayer[ActivePlayerIndex].ListSquad.Count)
                            UnitIndex = 0;
                    }
                    while (ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CurrentLeader == null);
                }

                ActiveSquadIndex = UnitIndex;
                CursorPosition = ActiveSquad.Position;
                CursorPositionVisible = CursorPosition;

                if (ActiveSquad.X < Camera2DPosition.X || ActiveSquad.Y < Camera2DPosition.Y ||
                    ActiveSquad.X >= Camera2DPosition.X + ScreenSize.X || ActiveSquad.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.Position));
                }
            }
            else if (InputHelper.InputRButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListSquad.Count == 0)
                    return CursorMoved;

                int UnitIndex = 0;
                if (ActiveSquad != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad);
                int StartIndex = UnitIndex;
                bool UnmovedSquadFound = false;

                do
                {
                    --UnitIndex;

                    if (UnitIndex < 0)
                        UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.Count - 1;

                    if (ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CurrentLeader != null && ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CanMove)
                    {
                        UnmovedSquadFound = true;
                    }
                }
                while (StartIndex != UnitIndex && !UnmovedSquadFound);

                if (!UnmovedSquadFound)
                {
                    do
                    {
                        if (--UnitIndex < 0)
                            UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.Count - 1;
                    }
                    while (ListPlayer[ActivePlayerIndex].ListSquad[UnitIndex].CurrentLeader == null);
                }

                ActiveSquadIndex = UnitIndex;
                CursorPosition = ActiveSquad.Position;
                CursorPositionVisible = CursorPosition;

                if (ActiveSquad.X < Camera2DPosition.X || ActiveSquad.Y < Camera2DPosition.Y ||
                    ActiveSquad.X >= Camera2DPosition.X + ScreenSize.X || ActiveSquad.Y >= Camera2DPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.Position));
                }
            }

            return CursorMoved;
        }
    }
}
