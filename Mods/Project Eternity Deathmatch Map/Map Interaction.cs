using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public char GetTerrainLetterAttribute(Core.Units.UnitStats UnitStat, string TerrainType)
        {
            return Grades[UnitStat.TerrainAttributeValue(TerrainType)];
        }

        public string GetTerrainType(float PosX, float PosY, int LayerIndex)
        {
            return GetTerrainType(GetTerrain(PosX, PosY, LayerIndex));
        }

        public string GetTerrainType(MovementAlgorithmTile ActiveTerrain)
        {
            return ListTerrainType[ActiveTerrain.TerrainTypeIndex];
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

            float CurrentZ = Position.Z;
            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X >= MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y >= MapSize.Y)
                return -1;

            float ZChange = GetTerrain(FinalPosition.X, FinalPosition.Y, ActiveLayerIndex).Position.Z - Position.Z;
            FinalPosition.Z += ZChange;

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
        public void OnNewTurn()
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int S = 0; S < ListPlayer[P].ListSquad.Count; S++)
                {
                    Squad ActiveSquad = ListPlayer[P].ListSquad[S];
                    if (ActiveSquad.CurrentLeader == null)
                        continue;

                    //Remove 5 EN each time the Squad spend a turn in the air.
                    if (ActiveSquad.CurrentMovement == Core.Units.UnitStats.TerrainAir)
                        ActiveSquad.CurrentLeader.ConsumeEN(5);

                    for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
                    {
                        Terrain ActiveTerrain = GetTerrain(ActiveSquad);
                        //Terrain passive bonus.
                        for (int i = 0; i < ActiveTerrain.ListActivation.Length; i++)
                            switch (ActiveTerrain.ListActivation[i])
                            {
                                case TerrainActivation.OnEveryTurns:
                                    switch (ActiveTerrain.ListBonus[i])
                                    {
                                        case TerrainBonus.HPRegen:
                                            ActiveSquad[U].HealUnit((int)(ActiveTerrain.ListBonusValue[i] / 100.0f) * ActiveSquad[U].MaxHP);
                                            break;

                                        case TerrainBonus.ENRegen:
                                            ActiveSquad[U].RefillEN((int)(ActiveTerrain.ListBonusValue[i] / 100.0f) * ActiveSquad[U].MaxEN);
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
                        ActiveSquad[U].OnTurnEnd(ActiveSquad);
                    }
                }
            }
        }

        public bool UpdateMapNavigation()
        {
            bool CursorMoved = CursorControl();//Move the cursor
            if (InputHelper.InputLButtonPressed())
            {
                if (ListPlayer[ActivePlayerIndex].ListSquad.Count == 0)
                    return CursorMoved;

                int UnitIndex = 0;
                if (ActiveSquad != null)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.IndexOf(ActiveSquad);

                ++UnitIndex;

                if (UnitIndex >= ListPlayer[ActivePlayerIndex].ListSquad.Count)
                    UnitIndex = 0;

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
                --UnitIndex;
                if (UnitIndex < 0)
                    UnitIndex = ListPlayer[ActivePlayerIndex].ListSquad.Count - 1;

                ActiveSquadIndex = UnitIndex;
                CursorPosition = ActiveSquad.Position;
                CursorPositionVisible = CursorPosition;

                if (ActiveSquad.X < CameraPosition.X || ActiveSquad.Y < CameraPosition.Y ||
                    ActiveSquad.X >= CameraPosition.X + ScreenSize.X || ActiveSquad.Y >= CameraPosition.Y + ScreenSize.Y)
                {
                    PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ActiveSquad.Position));
                }
            }

            return CursorMoved;
        }
    }
}
