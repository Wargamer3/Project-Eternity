using Microsoft.Xna.Framework;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public char GetTerrainLetterAttribute(UnitStats UnitStat, byte MovementTypeIndex)
        {
            return Grades[UnitStat.TerrainAttributeValue(MovementTypeIndex)];
        }

        public byte GetTerrainType(float PosX, float PosY, int LayerIndex)
        {
            return GetTerrain(PosX, PosY, LayerIndex).TerrainTypeIndex;
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
                if (ListPlayer[PlayerIndex].ListSquad[S].CurrentLeader == null)
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
        
        /// <summary>
        /// Called every time every players has finished their actions.
        /// </summary>
        internal void OnNewTurn()
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = ListPlayer[P].ListSquad[S];
                    if (ActiveSquad.CurrentLeader == null)
                        continue;

                    //Remove 5 EN each time the Squad spend a turn in the air.
                    int ENUsedPerTurn = TerrainRestrictions.GetENUsedPerTurnCost(ActiveSquad, ActiveSquad.CurrentLeader.UnitStat, ActiveSquad.CurrentTerrainIndex);
                    if (ENUsedPerTurn > 0)
                        ActiveSquad.CurrentLeader.ConsumeEN(ENUsedPerTurn);

                    for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                    {
                        foreach (Terrain ActiveTerrain in GetAllTerrain(ActiveSquad, this))
                        {
                            //Terrain passive bonus.
                            for (int i = 0; i < ActiveTerrain.ListActivation.Length; i++)
                                switch (ActiveTerrain.ListActivation[i])
                                {
                                    case TerrainActivation.OnEveryTurns:
                                        switch (ActiveTerrain.ListBonus[i])
                                        {
                                            case TerrainBonus.HPRegen:
                                                ActiveSquad[U].HealUnit((int)(ActiveTerrain.ListBonusValue[i] / 100.0f * ActiveSquad[U].MaxHP));
                                                break;

                                            case TerrainBonus.ENRegen:
                                                ActiveSquad[U].RefillEN((int)(ActiveTerrain.ListBonusValue[i] / 100.0f * ActiveSquad[U].MaxEN));
                                                break;
                                            case TerrainBonus.HPRestore:
                                                ActiveSquad[U].HealUnit(ActiveTerrain.ListBonusValue[i]);
                                                break;

                                            case TerrainBonus.ENRestore:
                                                ActiveSquad[U].RefillEN(ActiveTerrain.ListBonusValue[i]);
                                                break;
                                        }
                                        break;
                                }
                        }
                    }
                }
            }

            ActivePlayerIndex = 0;
            GameTurn++;

            UpdateMapEvent(EventTypeTurn, 0);
            
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = ListPlayer[P].ListSquad[S];
                    if (ActiveSquad.CurrentLeader == null)
                        continue;

                    ActiveSquad.StartTurn();

                    //Update Effect based on Turns.
                    for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                    {
                        ActiveSquad[U].OnTurnEnd(ActivePlayerIndex, ActiveSquad);
                    }
                }
            }
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

                if (ActiveSquad.X < CameraPosition.X || ActiveSquad.Y < CameraPosition.Y ||
                    ActiveSquad.X >= CameraPosition.X + ScreenSize.X || ActiveSquad.Y >= CameraPosition.Y + ScreenSize.Y)
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

                if (ActiveSquad.X < CameraPosition.X || ActiveSquad.Y < CameraPosition.Y ||
                    ActiveSquad.X >= CameraPosition.X + ScreenSize.X || ActiveSquad.Y >= CameraPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.Position));
                }
            }

            if (!IsOfflineOrServer)
            {
                OnlineClient.Host.Send(new MoveCursorScriptClient(CursorPosition.X, CursorPosition.Y));
            }

            return CursorMoved;
        }
    }
}
