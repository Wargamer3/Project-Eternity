using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetMap
    {
        public void CenterCamera()
        {
            if (ListPlayer[ActivePlayerIndex].GamePiece.X < Camera2DPosition.X || ListPlayer[ActivePlayerIndex].GamePiece.Y < Camera2DPosition.Y ||
                ListPlayer[ActivePlayerIndex].GamePiece.X >= Camera2DPosition.X + ScreenSize.X || ListPlayer[ActivePlayerIndex].GamePiece.Y >= Camera2DPosition.Y + ScreenSize.Y)
            {
                PushScreen(new CenterOnSquadCutscene(null, this, ListPlayer[ActivePlayerIndex].GamePiece.Position));
            }
        }

        public Tuple<int, int> CheckForEnemies(int ActivePlayerIndex, Vector3 PositionToCheck, bool FriendlyFire)
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                int TargetFound = CheckForTargetAtPosition(P, PositionToCheck, Vector3.Zero);

                if (TargetFound >= -1 && (FriendlyFire || ListPlayer[ActivePlayerIndex].TeamIndex != ListPlayer[P].TeamIndex))
                {
                    return new Tuple<int, int>(P, TargetFound);
                }
            }

            return null;
        }

        public int CheckForTargetAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].TeamIndex < 0)
                return -2;

            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X >= MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y >= MapSize.Y)
                return -2;

            Player ActivePlayer = ListPlayer[PlayerIndex];

            if (ActivePlayer.GamePiece.Position.X == FinalPosition.X && ActivePlayer.GamePiece.Position.Y == FinalPosition.Y)
            {
                return -1;
            }
            else
            {
                for (int C = 0; C < ActivePlayer.ListCreatureOnBoard.Count; C++)
                {
                    SorcererStreetUnit ActiveCreature = ActivePlayer.ListCreatureOnBoard[C];
                    if (ActiveCreature.Position.X == FinalPosition.X && ActiveCreature.Position.Y == FinalPosition.Y)
                    {
                        return C;
                    }
                }
            }

            return -2;
        }

        public void AttackWithExplosion(int ActivePlayerIndex, SorcererStreetUnit Owner, ExplosionOptions ExplosionOption, Vector3 Position)
        {
            Stack<Tuple<int, int>> ListAttackTarget = new Stack<Tuple<int, int>>();

            for (float OffsetX = -ExplosionOption.ExplosionRadius; OffsetX < ExplosionOption.ExplosionRadius; ++OffsetX)
            {
                for (float OffsetY = -ExplosionOption.ExplosionRadius; OffsetY < ExplosionOption.ExplosionRadius; ++OffsetY)
                {
                    for (float OffsetZ = -ExplosionOption.ExplosionRadius; OffsetZ < ExplosionOption.ExplosionRadius; ++OffsetZ)
                    {
                        if (Math.Abs(OffsetX) + Math.Abs(OffsetY) + Math.Abs(OffsetZ) < ExplosionOption.ExplosionRadius)
                        {
                            Tuple<int, int> ActiveTarget = CheckForEnemies(ActivePlayerIndex, new Vector3(Position.X + OffsetX, Position.Y + OffsetY, Position.Z + OffsetZ), true);

                            if (ActiveTarget != null)
                            {
                                ListAttackTarget.Push(ActiveTarget);
                            }
                        }
                    }
                }
            }

            if (ListAttackTarget.Count > 0)
            {
                foreach (Tuple<int, int> ActiveTarget in ListAttackTarget)
                {
                    if (ActiveTarget.Item2 >= 0)
                    {
                        SorcererStreetUnit CreatureToKill = ListPlayer[ActiveTarget.Item1].ListCreatureOnBoard[ActiveTarget.Item2];

                        TerrainSorcererStreet SquadTerrain = GetTerrain(CreatureToKill.Position);

                        Vector3 FinalSpeed = new Vector3(CreatureToKill.Position.X - Position.X, CreatureToKill.Position.Y - Position.Y, SquadTerrain.WorldPosition.Z - Position.Z);

                        float DiffTotal = FinalSpeed.Length() / 3f;

                        if (DiffTotal < ExplosionOption.ExplosionRadius)
                        {
                            float WindForce = 1 - (DiffTotal / ExplosionOption.ExplosionRadius);
                            float WindValue = ExplosionOption.ExplosionWindPowerAtEdge + WindForce * (ExplosionOption.ExplosionWindPowerAtCenter - ExplosionOption.ExplosionWindPowerAtEdge);

                            FinalSpeed.Normalize();

                            FinalSpeed *= WindValue;
                            CreatureToKill.Speed = FinalSpeed;

                            if (CreatureToKill.IsOnGround)
                            {
                                CreatureToKill.SetPosition(SquadTerrain.WorldPosition + new Vector3(0.5f, 0.5f, 0f));
                                if (FinalSpeed.Z < 0)
                                {
                                    CreatureToKill.IsOnGround = false;
                                }
                            }
                        }
                    }

                    AttackDirectly(ActivePlayerIndex, new List<Vector3>(), ListAttackTarget);
                }
            }
        }

        public void AttackDirectly(int ActivePlayerIndex, List<Vector3> ListMVHoverPoints, Stack<Tuple<int, int>> ListMAPAttackTarget)
        {
            if (ListMAPAttackTarget.Count > 0)
            {
                foreach (var FirstEnemy in ListMAPAttackTarget)
                {
                    //ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, CurrentAttack, ListMVHoverPoints, FirstEnemy.Item1, FirstEnemy.Item2, false));
                }
            }
        }

        public Stack<Tuple<int, int>> GetEnemies(bool FriendlyFire, List<MovementAlgorithmTile> ListAttackPosition)
        {
            Stack<Tuple<int, int>> ListMAPAttackTarget = new Stack<Tuple<int, int>>();//Player index, Squad index.

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int i = 0; i < ListAttackPosition.Count; i++)
                {
                    //Find if a Unit is under the cursor.
                    int TargetIndex = CheckForTargetAtPosition(P, ListAttackPosition[i].WorldPosition, Vector3.Zero);
                    //If one was found.
                    if (TargetIndex >= 0 && (FriendlyFire ||
                                                ListPlayer[ActivePlayerIndex].TeamIndex != ListPlayer[P].TeamIndex))
                    {
                        ListMAPAttackTarget.Push(new Tuple<int, int>(P, TargetIndex));
                    }
                }
            }

            return ListMAPAttackTarget;
        }

        public List<MovementAlgorithmTile> GetAllTerrain(UnitMapComponent ActiveUnit, SorcererStreetMap ActiveMap)
        {
            List<MovementAlgorithmTile> ListTerrainFound = new List<MovementAlgorithmTile>();
            for (int X = 0; X < ActiveUnit.ArrayMapSize.GetLength(0); ++X)
            {
                for (int Y = 0; Y < ActiveUnit.ArrayMapSize.GetLength(1); ++Y)
                {
                    if (ActiveUnit.ArrayMapSize[X, Y])
                    {
                        ListTerrainFound.Add(ActiveMap.LayerManager.ListLayer[(int)ActiveUnit.Z].ArrayTerrain[(int)ActiveUnit.X + X, (int)ActiveUnit.Y + Y]);
                    }
                }
            }

            return ListTerrainFound;
        }

        public List<MovementAlgorithmTile> GetAttackChoice(SorcererStreetUnit ActiveSquad, int RangeMaximum)
        {
            SorcererStreetMap ActiveMap = this;
            if (ActivePlatform != null)
            {
                ActiveMap = (SorcererStreetMap)ActivePlatform.Map;
            }

            //Init A star.
            List<MovementAlgorithmTile> ListAllNode = Pathfinder.FindPath(GetAllTerrain(ActiveSquad, ActiveMap), ActiveSquad, ActiveSquad.UnitStat, RangeMaximum, true);

            List<MovementAlgorithmTile> MovementChoice = new List<MovementAlgorithmTile>();

            for (int i = 0; i < ListAllNode.Count; i++)
            {
                ListAllNode[i].ParentTemp = null;//Unset parents
                ListAllNode[i].MovementCost = 0;

                if (ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainWallIndex || ListAllNode[i].TerrainTypeIndex == UnitStats.TerrainVoidIndex)
                {
                    continue;
                }

                MovementChoice.Add(ListAllNode[i]);
            }

            return MovementChoice;
        }

        public void SelectMAPEnemies(int ActivePlayerIndex, int ActiveSquadIndex, MAPAttackAttributes MAPAttributes, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
        {
            if (MAPAttributes.Delay > 0)
            {
                SorcererStreetUnit ActiveSquad = ListPlayer[ActivePlayerIndex].ListCreatureOnBoard[ActiveSquadIndex];
                ListDelayedAttack.Add(new DelayedAttack(ActiveSquad, ActivePlayerIndex, AttackChoice));
                ListActionMenuChoice.RemoveAllSubActionPanels();
                ActiveSquad.EndTurn();
            }
            else
            {
                Stack<Tuple<int, int>> ListMAPAttackTarget = GetEnemies(MAPAttributes.FriendlyFire, AttackChoice);

                if (ListMAPAttackTarget.Count > 0)
                {
                    SorcererStreetParams.GlobalContext.ArrayAttackPosition = AttackChoice.ToArray();

                    AttackDirectly(ActivePlayerIndex, ListMVHoverPoints, ListMAPAttackTarget);
                }
            }
        }
    }
}
