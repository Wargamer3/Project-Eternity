using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class DeathmatchMap
    {
        public void ComputeTargetPlayerOffense(int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack, SupportSquadHolder ActiveSquadSupport, List<Vector3> ListMVHoverPoints,
            int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            if (ListPlayer[ActivePlayerIndex].IsPlayerControlled)
            {
                ActionPanelHumanAttack PlayerDefence = new ActionPanelHumanAttack(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, true));
            }
        }

        public void ComputeTargetPlayerDefence(int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack, SupportSquadHolder ActiveSquadSupport, List<Vector3> ListMVHoverPoints,
            int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            if (ListPlayer[TargetPlayerIndex].IsPlayerControlled)
            {
                ActionPanelHumanDefend PlayerDefence = new ActionPanelHumanDefend(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                //Skip defense
                ComputeTargetPlayerOffense(ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
            }
        }

        public void ReadyNextMAPAttack(int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack, SupportSquadHolder ActiveSquadSupport, List<Vector3> ListMVHoverPoints,
            int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            Squad ActiveSquad = ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad TargetSquad = ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];

            ActiveSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

            UpdateWingmansSelection(ActiveSquad, TargetSquad, BattleMenuOffenseFormationChoice);

            if (ActiveSquad.CurrentWingmanA != null)
            {
                if (ActiveSquad.CurrentWingmanA.CanAttack)
                    ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                else
                    ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
            }
            if (ActiveSquad.CurrentWingmanB != null)
            {
                if (ActiveSquad.CurrentWingmanB.CanAttack)
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                else
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
            }

            PrepareDefenseSquadForBattle(this, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, TargetPlayerIndex, TargetSquadIndex);
            PrepareAttackSquadForBattle(this, ActiveSquad, CurrentAttack, TargetSquad);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, false ));
        }

        public void AttackWithMAPAttack(int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack, List<Vector3> ListMVHoverPoints, Stack<Tuple<int, int>> ListMAPAttackTarget)
        {
            Squad ActiveSquad = ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            this.ListMAPAttackTarget = ListMAPAttackTarget;
            Tuple<int, int> FirstEnemy = ListMAPAttackTarget.Pop();
            ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2].CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
            PrepareSquadsForBattle(ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, FirstEnemy.Item1, FirstEnemy.Item2);

            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(this, ActivePlayerIndex, ActiveSquad, FirstEnemy.Item1, FirstEnemy.Item2);

            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(this, FirstEnemy.Item1, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2]);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, FirstEnemy.Item1, FirstEnemy.Item2, TargetSquadSupport, false));

            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
        }

        public void SelectMAPEnemies(int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> ListMVHoverPoints, List<MovementAlgorithmTile> AttackChoice)
        {
            if (ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Delay > 0)
            {
                ListDelayedAttack.Add(new DelayedAttack(ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad, ActivePlayerIndex, AttackChoice));
                ListActionMenuChoice.RemoveAllSubActionPanels();
                ActiveSquad.EndTurn();
            }
            else
            {
                Stack<Tuple<int, int>> ListMAPAttackTarget = GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.FriendlyFire, AttackChoice);

                if (ListMAPAttackTarget.Count > 0)
                {
                    GlobalBattleParams.GlobalContext.ArrayAttackPosition = AttackChoice.ToArray();

                    AttackWithMAPAttack(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ListMVHoverPoints, ListMAPAttackTarget);

                    //Remove Ammo if needed.
                    if (ActiveSquad.CurrentLeader.CurrentAttack.MaxAmmo > 0)
                        ActiveSquad.CurrentLeader.CurrentAttack.ConsumeAmmo();
                }
            }
        }

        public List<Tuple<int, int>> CanSquadAttackWeapon(Squad ActiveSquad, Vector3 Position, Attack ActiveWeapon, int MinRange, int MaxRange, bool CanMove, Unit ActiveUnit)
        {
            BattleMap ActiveMap = this;
            if (ActivePlatform != null)
            {
                ActiveMap = ActivePlatform.Map;
            }

            bool CanAttackPostMovement = ActiveWeapon.IsPostMovement(ActiveUnit);

            if (!ActiveSquad.CanMove && !CanAttackPostMovement)
            {
                return new List<Tuple<int, int>>();
            }

            //List of targets to attack.
            List<Tuple<int, int>> ListTargetUnit = new List<Tuple<int, int>>();

            //Check if there is an enemy in attack range.
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                //If the player is from the same team as the current player or is dead, skip it.
                if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team || !ListPlayer[P].IsAlive)
                    continue;

                for (int TargetSelect = 0; TargetSelect < ListPlayer[P].ListSquad.Count; TargetSelect++)
                {
                    if (ListPlayer[P].ListSquad[TargetSelect].CurrentLeader == null)
                        continue;

                    ActiveWeapon.UpdateAttack(ActiveSquad.CurrentLeader, Position, ListPlayer[ActivePlayerIndex].Team, ListPlayer[P].ListSquad[TargetSelect].Position, ListPlayer[P].Team,
                        ListPlayer[P].ListSquad[TargetSelect].ArrayMapSize, ListPlayer[P].ListSquad[TargetSelect].CurrentTerrainIndex, CanMove);

                    //Make sure you can use it.
                    if (ActiveWeapon.CanAttack)
                    {
                        List<MovementAlgorithmTile> ListRealChoice;

                        if (CanMove)
                        {
                            //Move to be in range.
                            ListRealChoice = new List<MovementAlgorithmTile>(GetMVChoice(ActiveSquad, ActiveMap));
                        }
                        else
                        {
                            ListRealChoice = new List<MovementAlgorithmTile>();
                        }

                        ListRealChoice.Add(GetTerrain(ActiveSquad));

                        for (int M = 0; M < ListRealChoice.Count; M++)
                        {//Remove every MV that would make it impossible to attack.
                            float Distance = Math.Abs(ListRealChoice[M].WorldPosition.X - ListPlayer[P].ListSquad[TargetSelect].X) + Math.Abs(ListRealChoice[M].WorldPosition.Y - ListPlayer[P].ListSquad[TargetSelect].Y);
                            //Check if you can attack it if you moved.
                            if (Distance < MinRange || Distance > MaxRange)
                                ListRealChoice.RemoveAt(M--);
                        }

                        //Must find a spot to move if got there, just to make sure it won't crash in case of logic error.
                        if (ListRealChoice.Count != 0)
                        {
                            ListTargetUnit.Add(new Tuple<int, int>(P, TargetSelect));
                        }
                    }
                }
            }

            return ListTargetUnit;
        }

        public Stack<Tuple<int, int>> GetEnemies(bool FriendlyFire, List<MovementAlgorithmTile> ListAttackPosition)
        {
            Stack<Tuple<int, int>> ListMAPAttackTarget = new Stack<Tuple<int, int>>();//Player index, Squad index.

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int i = 0; i < ListAttackPosition.Count; i++)
                {
                    //Find if a Unit is under the cursor.
                    int TargetIndex = CheckForSquadAtPosition(P, ListAttackPosition[i].WorldPosition, Vector3.Zero);
                    //If one was found.
                    if (TargetIndex >= 0 && (FriendlyFire ||
                                                ListPlayer[ActivePlayerIndex].Team != ListPlayer[P].Team))
                    {
                        ListMAPAttackTarget.Push(new Tuple<int, int>(P, TargetIndex));
                    }
                }
            }

            return ListMAPAttackTarget;
        }

        public void PrepareSquadsForBattle(int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            AttackingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

            UpdateWingmansSelection(AttackingSquad, DefendingSquad, BattleMenuOffenseFormationChoice);

            //Simulate defense reaction.
            PrepareDefenseSquadForBattle(this, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, DefendingPlayerIndex, DefendingSquadIndex);
            
            //Reset the enemy counter.
            BattleMenuStage = BattleMenuStages.Default;
            BattleMenuCursorIndex = 0;
        }

        public void UpdateWingmansSelection(Squad ActiveSquad, Squad OppositeSquad, FormationChoices FormationChoice)
        {
            if (FormationChoice == FormationChoices.Focused)
            {
                if (ActiveSquad.CurrentWingmanA != null)
                {
                    ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanA.CurrentAttack = null;
                    ActiveSquad.CurrentWingmanA.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanA.PLAAttack != null)
                    {
                        ActiveSquad.CurrentWingmanA.PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, ListPlayer[ActivePlayerIndex].Team, OppositeSquad.Position, ListPlayer[TargetPlayerIndex].Team, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentTerrainIndex, true);

                        if (ActiveSquad.CurrentWingmanA.PLAAttack.CanAttack)
                        {
                            ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            ActiveSquad.CurrentWingmanA.CurrentAttack = ActiveSquad.CurrentWingmanA.PLAAttack;
                            ActiveSquad.CurrentWingmanA.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad.CurrentWingmanA.CurrentAttack, ActiveSquad,
                                ActiveSquad.CurrentWingmanA, OppositeSquad, OppositeSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                        }
                    }
                }

                if (ActiveSquad.CurrentWingmanB != null)
                {
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanB.CurrentAttack = null;
                    ActiveSquad.CurrentWingmanB.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanB.PLAAttack != null)
                    {
                        ActiveSquad.CurrentWingmanB.PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, ListPlayer[ActivePlayerIndex].Team, OppositeSquad.Position, ListPlayer[TargetPlayerIndex].Team, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentTerrainIndex, true);

                        if (ActiveSquad.CurrentWingmanB.PLAAttack.CanAttack)
                        {
                            ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            ActiveSquad.CurrentWingmanB.CurrentAttack = ActiveSquad.CurrentWingmanB.PLAAttack;
                            ActiveSquad.CurrentWingmanB.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad.CurrentWingmanB.CurrentAttack, ActiveSquad,
                                ActiveSquad.CurrentWingmanB, OppositeSquad, OppositeSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                        }
                    }
                }
            }
            else if (FormationChoice == FormationChoices.Spread)
            {
                if (ActiveSquad.CurrentWingmanA != null)
                {
                    ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanA.CurrentAttack = null;
                    ActiveSquad.CurrentWingmanA.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanA.PLAAttack != null)
                    {
                        if (OppositeSquad.CurrentWingmanA != null)
                        {
                            ActiveSquad.CurrentWingmanA.PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, ListPlayer[ActivePlayerIndex].Team, OppositeSquad.Position, ListPlayer[TargetPlayerIndex].Team, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentTerrainIndex, true);

                            if (ActiveSquad.CurrentWingmanA.PLAAttack.CanAttack)
                            {
                                ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                                ActiveSquad.CurrentWingmanA.CurrentAttack = ActiveSquad.CurrentWingmanA.PLAAttack;
                                ActiveSquad.CurrentWingmanA.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad.CurrentWingmanA.CurrentAttack, ActiveSquad,
                                    ActiveSquad.CurrentWingmanA, OppositeSquad, OppositeSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                            }
                        }
                    }
                }

                if (ActiveSquad.CurrentWingmanB != null)
                {
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanB.CurrentAttack = null;
                    ActiveSquad.CurrentWingmanB.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanB.PLAAttack != null)
                    {
                        if (OppositeSquad.CurrentWingmanB != null)
                        {
                            ActiveSquad.CurrentWingmanB.PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, ListPlayer[ActivePlayerIndex].Team, OppositeSquad.Position, ListPlayer[TargetPlayerIndex].Team, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentTerrainIndex, true);

                            if (ActiveSquad.CurrentWingmanB.PLAAttack.CanAttack)
                            {
                                ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                                ActiveSquad.CurrentWingmanB.CurrentAttack = ActiveSquad.CurrentWingmanB.PLAAttack;
                                ActiveSquad.CurrentWingmanB.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad.CurrentWingmanB.CurrentAttack, ActiveSquad,
                                    ActiveSquad.CurrentWingmanB, OppositeSquad, OppositeSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                            }
                        }
                    }
                }
            }
        }

        public static void PrepareAttackSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Attack CurrentAttack, Squad DefendingSquad)
        {
            AttackingSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(AttackingSquad.CurrentLeader, CurrentAttack, AttackingSquad,
                DefendingSquad.CurrentLeader, DefendingSquad, DefendingSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";

            if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
            {
                if (AttackingSquad.CurrentWingmanA != null)
                {
                    AttackingSquad.CurrentLeader.AttackAccuracy = "";
                    AttackingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                }
                if (AttackingSquad.CurrentWingmanB != null)
                {
                    AttackingSquad.CurrentWingmanB.AttackAccuracy = "";
                    AttackingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                }

                if (DefendingSquad.CurrentWingmanA != null)
                {
                    AttackingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(AttackingSquad.CurrentLeader, CurrentAttack, AttackingSquad,
                                                                        DefendingSquad.CurrentWingmanA, DefendingSquad,
                                                                        DefendingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }

                if (DefendingSquad.CurrentWingmanB != null)
                {
                    AttackingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(AttackingSquad.CurrentLeader, CurrentAttack, AttackingSquad,
                                                                        DefendingSquad.CurrentWingmanB, DefendingSquad,
                                                                        DefendingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
            }
            else
            {
                Map.UpdateWingmansSelection(AttackingSquad, DefendingSquad, Map.BattleMenuOffenseFormationChoice);
            }
        }

        /// <summary>
        /// Simulate the player reaction when attacked.
        /// </summary>
        public static void PrepareDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            if (CurrentAttack.Pri == WeaponPrimaryProperty.MAP || CurrentAttack.Pri == WeaponPrimaryProperty.PER)
                return;

            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            switch (DefendingSquad.SquadDefenseBattleBehavior)
            {
                case "Always Counterattack":
                    AlwaysCounterattackDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, DefendingPlayerIndex, DefendingSquadIndex);
                    break;

                case "Simple Counterattack":
                    SimpleCounterattackDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, DefendingPlayerIndex, DefendingSquadIndex);
                    break;

                case "Always Block":
                    AlwaysBlockDefenseSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
                    break;

                case "Always Dodge":
                    AlwaysDodgeDefenseSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
                    break;

                case "Smart Counterattack":
                default:
                    DefaultDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, CurrentAttack, DefendingPlayerIndex, DefendingSquadIndex);
                    break;
            }
        }

        private static void AlwaysCounterattackDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.CurrentAttack = null;

            UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, 1, CurrentAttack,
                                    null, BattleMap.WingmanDamageModifier, null,
                                    null, BattleMap.WingmanDamageModifier, null, 0,
                                    DefendingPlayerIndex, DefendingSquadIndex, 0, false, true, Map);

            if (DefendingSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
            {
                DefendingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.DoNothing;
            }

            if (DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack &&
                DefendingSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
            {
                Map.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.ALL;

                if (AttackingSquad.CurrentWingmanA != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanA.CurrentAttack = DefendingSquad.CurrentWingmanA.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanA.CurrentAttack = null;

                int CounterUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    CounterUnitIndex = 1;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    CounterUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
                }

                if (DefendingSquad.CurrentWingmanA.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.DoNothing;
                }
            }

            #endregion

            #region Wingman B

            if (DefendingSquad.CurrentWingmanB != null)
            {
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanB.CurrentAttack = DefendingSquad.CurrentWingmanB.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanB.CurrentAttack = null;

                int CounterUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    CounterUnitIndex = 2;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    CounterUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }

                if (DefendingSquad.CurrentWingmanB.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.DoNothing;
                }
            }

            #endregion

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
        }

        private static void SimpleCounterattackDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];
            Attack CurrentWingmanAAttack = null;
            Attack CurrentWingmanBAttack = null;

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.CurrentAttack = null;
            if (AttackingSquad.CurrentWingmanA != null)
            {
                CurrentWingmanAAttack = AttackingSquad.CurrentWingmanA.CurrentAttack;
            }
            if (AttackingSquad.CurrentWingmanB != null)
            {
                CurrentWingmanBAttack = AttackingSquad.CurrentWingmanB.CurrentAttack;
            }

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, CurrentWingmanAAttack,
                                        AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier, CurrentWingmanBAttack,
                                        0,
                                        DefendingPlayerIndex, DefendingSquadIndex, 0,
                                        false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                                        null, BattleMap.WingmanDamageModifier, null,
                                        null, BattleMap.WingmanDamageModifier, null,
                                        0,
                                        DefendingPlayerIndex, DefendingSquadIndex, 0,
                                        false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                                        null, BattleMap.WingmanDamageModifier, null,
                                        null, BattleMap.WingmanDamageModifier, null,
                                        0,
                                        DefendingPlayerIndex, DefendingSquadIndex, 0,
                                        false, false, Map);
            }

            if (DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack &&
                DefendingSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
            {
                Map.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.ALL;

                if (AttackingSquad.CurrentWingmanA != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanA.CurrentAttack = DefendingSquad.CurrentWingmanA.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanA.CurrentAttack = null;
                }

                int TargetUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    TargetUnitIndex = 1;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    TargetUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, CurrentWingmanAAttack,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1, null,
                        null, WingmanDamageModifier, null,
                        null,BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
            }

            #endregion

            #region Wingman B

            if (DefendingSquad.CurrentWingmanB != null)
            {
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanB.CurrentAttack = DefendingSquad.CurrentWingmanB.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanB.CurrentAttack = null;
                }

                int TargetUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    TargetUnitIndex = 2;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    TargetUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentWingmanB, WingmanDamageModifier, CurrentWingmanBAttack,
                        null, WingmanDamageModifier, null,
                        null, WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1, null,
                        null, WingmanDamageModifier, null,
                        null, WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                        null, WingmanDamageModifier, null,
                        null, WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
            }

            #endregion
            
            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
        }

        private static void AlwaysBlockDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Attack CurrentAttack, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
            DefendingSquad.CurrentLeader.CurrentAttack = null;
            
            if (DefendingSquad.CurrentWingmanA != null)
            {
                DefendingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                DefendingSquad.CurrentWingmanA.CurrentAttack = null;
            }

            if (DefendingSquad.CurrentWingmanB != null)
            {
                DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                DefendingSquad.CurrentWingmanB.CurrentAttack = null;
            }

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
        }

        private static void AlwaysDodgeDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Attack CurrentAttack, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DefendingSquad.CurrentLeader.CurrentAttack = null;

            if (DefendingSquad.CurrentWingmanA != null)
            {
                DefendingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                DefendingSquad.CurrentWingmanA.CurrentAttack = null;
            }

            if (DefendingSquad.CurrentWingmanB != null)
            {
                DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                DefendingSquad.CurrentWingmanB.CurrentAttack = null;
            }

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
        }

        private static void DefaultDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];
            Attack CurrentWingmanAAttack = null;
            Attack CurrentWingmanBAttack = null;

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.CurrentAttack = null;
            if (AttackingSquad.CurrentWingmanA != null)
            {
                CurrentWingmanAAttack = AttackingSquad.CurrentWingmanA.CurrentAttack;
            }
            if (AttackingSquad.CurrentWingmanB != null)
            {
                CurrentWingmanBAttack = AttackingSquad.CurrentWingmanB.CurrentAttack;
            }

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1, CurrentAttack,
                    AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, CurrentWingmanAAttack,
                    AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier, CurrentWingmanBAttack,
                    0,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1, CurrentAttack,
                    null, BattleMap.WingmanDamageModifier, null,
                    null, BattleMap.WingmanDamageModifier, null,
                    0,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1, CurrentAttack,
                    null, BattleMap.WingmanDamageModifier, null,
                    null, BattleMap.WingmanDamageModifier, null,
                    0,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    false, true, Map);
            }

            if (DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack &&
                DefendingSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
            {
                Map.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.ALL;

                if (AttackingSquad.CurrentWingmanA != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanA.CurrentAttack = DefendingSquad.CurrentWingmanA.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanA.CurrentAttack = null;
                }

                int TargetUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    TargetUnitIndex = 1;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    TargetUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, CurrentWingmanAAttack,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1, null,
                        null, WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
            }

            #endregion

            #region Wingman B

            if (DefendingSquad.CurrentWingmanB != null)
            {
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack != null &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanB.CurrentAttack = DefendingSquad.CurrentWingmanB.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanB.CurrentAttack = null;

                Unit TargetUnit = null;


                int TargetUnitIndex = -1;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    TargetUnitIndex = 2;
                }
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                {
                    TargetUnitIndex = 0;
                }

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentWingmanB, 1, CurrentWingmanBAttack,
                        null, BattleMap.WingmanDamageModifier, null,
                        null, BattleMap.WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1, null,
                        null, WingmanDamageModifier, null,
                        null, WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1, CurrentAttack,
                        null, WingmanDamageModifier, null,
                        null, WingmanDamageModifier, null,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
            }

            #endregion

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, CurrentAttack, DefendingSquad);
        }

        private static void UnitBaseDefencePattern(
            int ActivePlayerIndex, int ActiveSquadIndex,
            Unit Attacker1, float DamageModifier1, Attack CurrentAttack1,
            Unit Attacker2, float DamageModifier2, Attack CurrentAttack2,
            Unit Attacker3, float DamageModifier3, Attack CurrentAttack3,
            int TargetCounterIndex,
            int TargetPlayerIndex, int TargetSquadIndex, int TargetUnitIndex,
            bool UsePLAWeapon, bool TryToDefendAttack,
            DeathmatchMap Map)
        {
            Squad AttackerSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Unit TargetCounter = AttackerSquad[TargetCounterIndex];

            Squad DefenderSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            Unit DefenderUnit = DefenderSquad[TargetUnitIndex];

            int HitRate1;
            int HitRate2;
            int HitRate3;

            if (Attacker1 == null || Attacker1.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                HitRate1 = 0;
            else
                HitRate1 = Map.CalculateHitRate(Attacker1, CurrentAttack1, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            if (Attacker2 == null || Attacker2.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                HitRate2 = 0;
            else
                HitRate2 = Map.CalculateHitRate(Attacker2, CurrentAttack2, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            if (Attacker3 == null || Attacker3.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                HitRate3 = 0;
            else
                HitRate3 = Map.CalculateHitRate(Attacker3, CurrentAttack3, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            int DamageTaken = 0;
            int TempDamage;

            if (HitRate1 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker1, CurrentAttack1, AttackerSquad, DamageModifier1, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate2 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker2, CurrentAttack2, AttackerSquad, DamageModifier2, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - (TempDamage + DamageTaken) < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate3 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker3, CurrentAttack3, AttackerSquad, DamageModifier3, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - (TempDamage + DamageTaken) < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            //The Unit won't die from being attacked.
            if (DefenderUnit.HP - DamageTaken > 0 && TargetCounter != null)
            {
                //Try to find a weapon to counter attack.
                int DamageOld = 0;

                DefenderUnit.UpdateNonMAPAttacks(DefenderSquad.Position, Map.ListPlayer[TargetPlayerIndex].Team, AttackerSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, AttackerSquad.ArrayMapSize, AttackerSquad.CurrentTerrainIndex, true);

                if (!UsePLAWeapon)
                {
                    Attack FinalAttackChoice = null;

                    //Find the weapon with the most power.
                    foreach (Attack ActiveAttack in DefenderUnit.ListAttack)
                    {
                        DefenderUnit.CurrentAttack = ActiveAttack;
                        if (DefenderUnit.CurrentAttack.CanAttack)
                        {
                            int Damage = Map.DamageFormula(DefenderUnit, DefenderUnit.CurrentAttack, DefenderSquad, 1, ActivePlayerIndex, ActiveSquadIndex, TargetCounterIndex, TargetCounter.BattleDefenseChoice, false).AttackDamage;

                            if (Damage > DamageOld)
                            {
                                FinalAttackChoice = ActiveAttack;
                                DamageOld = Damage;
                            }
                        }
                    }

                    DefenderUnit.CurrentAttack = FinalAttackChoice;
                }
            }

            //You can counter and survive the attack.
            if (DefenderUnit.CurrentAttack != null)
            {
                DefenderUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                DefenderUnit.AttackAccuracy = Map.CalculateHitRate(DefenderUnit, DefenderUnit.CurrentAttack, DefenderSquad, TargetCounter, AttackerSquad, TargetCounter.BattleDefenseChoice).ToString() + "%";
            }
            else if (TryToDefendAttack)//Can't counter or will die if attacked.
            {
                DamageTaken = 0;

                if (HitRate1 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker1, CurrentAttack1, AttackerSquad, DamageModifier1, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate2 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker2, CurrentAttack2, AttackerSquad, DamageModifier2, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate3 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker3, CurrentAttack3, AttackerSquad, DamageModifier3, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (DefenderUnit.HP - DamageTaken > 0)
                {
                    DefenderUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                }
                else//Would still die from the attack.
                {//Try to evade it.
                    DefenderUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                }
            }
            else
            {
                DefenderUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            }
        }

        private void GetLeftRightSquads(bool IsActiveSquadOnRight,
            Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, Squad TargetSquad, SupportSquadHolder TargetSquadSupport,
            out Squad NonDemoRightSquad, out Squad NonDemoRightSupport,
            out Squad NonDemoLeftSquad, out Squad NonDemoLeftSupport)
        {
            NonDemoLeftSupport = null;
            NonDemoRightSupport = null;

            if (IsActiveSquadOnRight)
            {
                NonDemoRightSquad = ActiveSquad;
                NonDemoLeftSquad = TargetSquad;

                if (ActiveSquadSupport.ActiveSquadSupport != null)
                {
                    NonDemoRightSupport = ActiveSquadSupport.ActiveSquadSupport;
                }
                if (TargetSquadSupport.ActiveSquadSupport != null)
                {
                    NonDemoLeftSupport = TargetSquadSupport.ActiveSquadSupport;
                }
            }
            else
            {
                NonDemoRightSquad = TargetSquad;
                NonDemoLeftSquad = ActiveSquad;

                if (TargetSquadSupport != null)
                {
                    NonDemoRightSupport = TargetSquadSupport.ActiveSquadSupport;
                }
                if (ActiveSquadSupport.ActiveSquadSupport != null)
                {
                    NonDemoLeftSupport = ActiveSquadSupport.ActiveSquadSupport;
                }
            }
        }

        private AnimationScreen CreateAnimation(string AnimationName, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
            SquadBattleResult BattleResult, AnimationScreen.AnimationUnitStats UnitStats, AnimationBackground ActiveAnimationBackground, AnimationBackground ActiveAnimationForeground, string ExtraText, bool IsLeftAttacking)
        {
            AnimationScreen NewAnimationScreen = new AnimationScreen(AnimationName, Map, ActiveSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                ActiveAnimationBackground, ActiveAnimationForeground, ExtraText, IsLeftAttacking);

            return NewAnimationScreen;
        }

        private AnimationScreen CreateAnimation(AnimationInfo Info, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
            SquadBattleResult BattleResult, AnimationScreen.AnimationUnitStats UnitStats, AnimationBackground ActiveAnimationBackground, AnimationBackground ActiveAnimationForeground, string ExtraText, bool IsLeftAttacking)
        {
            AnimationScreen NewAnimationScreen = new AnimationScreen(Info.AnimationName, Map, ActiveSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                ActiveAnimationBackground, ActiveAnimationForeground, ExtraText, IsLeftAttacking);

            NewAnimationScreen.Load();
            NewAnimationScreen.UpdateKeyFrame(0);

            Dictionary<int, Timeline> DicExtraTimeline = Info.GetExtraTimelines(NewAnimationScreen);

            foreach (KeyValuePair<int, Timeline> ActiveExtraTimeline in DicExtraTimeline)
            {
                NewAnimationScreen.ListAnimationLayer[0].AddTimelineEvent(ActiveExtraTimeline.Key, ActiveExtraTimeline.Value);
            }

            return NewAnimationScreen;
        }

        public List<GameScreen> GenerateNextAnimationScreens(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, Squad TargetSquad, SupportSquadHolder TargetSquadSupport,
            AnimationScreen.AnimationUnitStats UnitStats, AnimationScreen.BattleAnimationTypes BattleAnimationType, SquadBattleResult AttackingResult,
            out AnimationBackground TargetSquadBackground, AnimationBackground OldAttackerBackground,
            out AnimationBackground TargetSquadForeground, AnimationBackground OldAttackerForeground)
        {
            List<GameScreen> ListNextAnimationScreen = new List<GameScreen>();

            bool IsActiveSquadOnRight = BattleAnimationType == AnimationScreen.BattleAnimationTypes.RightAttackLeft || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftConteredByRight;
            bool HorionztalMirror = BattleAnimationType == AnimationScreen.BattleAnimationTypes.RightConteredByLeft || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftAttackRight;
            bool IsCounter = BattleAnimationType == AnimationScreen.BattleAnimationTypes.RightConteredByLeft || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftConteredByRight;

            Squad NonDemoRightSquad;
            Squad NonDemoRightSupport;
            Squad NonDemoLeftSquad;
            Squad NonDemoLeftSupport;

            GetLeftRightSquads(IsActiveSquadOnRight, ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport, out NonDemoRightSquad, out NonDemoRightSupport, out NonDemoLeftSquad, out NonDemoLeftSupport);

            Squad AttackingSquad = NonDemoRightSquad;
            Squad ActiveUnitSupport = NonDemoRightSupport;
            Attack ActiveAttack = AttackingSquad.CurrentLeader.CurrentAttack;
            byte ActiveTerrain = NonDemoRightSquad.CurrentTerrainIndex;
            SquadBattleResult BattleResult = AttackingResult;
            Squad EnemySquad = NonDemoLeftSquad;
            Squad SupportDefend = NonDemoLeftSupport;

            if (BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftAttackRight || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftConteredByRight)
            {
                AttackingSquad = NonDemoLeftSquad;
                ActiveUnitSupport = NonDemoLeftSupport;
                ActiveAttack = AttackingSquad.CurrentLeader.CurrentAttack;
                ActiveTerrain = NonDemoLeftSquad.CurrentTerrainIndex;
                EnemySquad = NonDemoRightSquad;

                if (!IsCounter)
                {
                    SupportDefend = NonDemoRightSupport;
                }
            }

            string ExtraTextIntro = AttackingSquad.CurrentLeader.ItemName + " Attacks!";
            string ExtraTextHit = AttackingSquad.CurrentLeader.ItemName + " hits! " + EnemySquad.CurrentLeader.ItemName + " takes " + BattleResult.ArrayResult[0].AttackDamage + " damage!";
            string ExtraTextMiss = AttackingSquad.CurrentLeader.ItemName + " misses! " + EnemySquad.CurrentLeader.ItemName + " takes 0 damage!";
            string ExtraTextKill = EnemySquad.CurrentLeader.ItemName + " is destroyed!";

            AnimationBackground ActiveSquadBackground;
            TargetSquadBackground = null;
            AnimationBackground ActiveSquadForeground;
            TargetSquadForeground = null;

            string ActiveSquadBackgroundPath = "Backgrounds 2D/Empty";
            string TargetSquadBackgroundPath = "Backgrounds 2D/Empty";
            string ActiveSquadForegroundPath = "Backgrounds 2D/Empty";
            string TargetSquadForegroundPath = "Backgrounds 2D/Empty";
            DrawableTile ActiveSquadTile = GetTile(AttackingSquad);
            DrawableTile TargetSquadTile = GetTile(EnemySquad);
            Terrain ActiveSquadTerrain = GetTerrain(AttackingSquad);
            Terrain TargetSquadTerrain = GetTerrain(EnemySquad);

            if (ActiveSquadTerrain.BattleBackgroundAnimationIndex >= 0)
            {
                ActiveSquadBackgroundPath = ListBattleBackgroundAnimationPath[ActiveSquadTerrain.BattleBackgroundAnimationIndex];
            }

            if (TargetSquadTerrain.BattleBackgroundAnimationIndex >= 0)
            {
                TargetSquadBackgroundPath = ListBattleBackgroundAnimationPath[TargetSquadTerrain.BattleBackgroundAnimationIndex];
            }

            if (ActiveSquadTerrain.BattleForegroundAnimationIndex >= 0)
            {
                ActiveSquadForegroundPath = ListBattleBackgroundAnimationPath[ActiveSquadTerrain.BattleForegroundAnimationIndex];
            }

            if (TargetSquadTerrain.BattleForegroundAnimationIndex >= 0)
            {
                TargetSquadForegroundPath = ListBattleBackgroundAnimationPath[TargetSquadTerrain.BattleForegroundAnimationIndex];
            }

            if (OldAttackerBackground != null)
            {
                ActiveSquadBackground = OldAttackerBackground;
            }
            else
            {
                ActiveSquadBackground = new AnimationBackground2D(ActiveSquadBackgroundPath, Content, GraphicsDevice);
            }

            if (OldAttackerForeground != null)
            {
                ActiveSquadForeground= OldAttackerForeground;
            }
            else
            {
                ActiveSquadForeground = new AnimationBackground2D(ActiveSquadForegroundPath, Content, GraphicsDevice);
            }

            if (ActiveSquadBackgroundPath == TargetSquadBackgroundPath)
            {
                TargetSquadBackground = ActiveSquadBackground;
            }
            else
            {
                TargetSquadBackground = new AnimationBackground2D(TargetSquadBackgroundPath, Content, GraphicsDevice);
            }

            if (ActiveSquadForegroundPath == TargetSquadForegroundPath)
            {
                TargetSquadForeground = ActiveSquadForeground;
            }
            else
            {
                TargetSquadForeground = new AnimationBackground2D(TargetSquadForegroundPath, Content, GraphicsDevice);
            }

            AttackAnimations AttackerAnimations = ActiveAttack.GetAttackAnimations(ActiveParser);

            if (!IsCounter)
            {
                ListNextAnimationScreen.Add(CreateAnimation(AttackingSquad.CurrentLeader.Animations.MoveFoward, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                    ActiveSquadBackground, ActiveSquadForeground, "", HorionztalMirror));
            }

            ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.Start, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                ActiveSquadBackground, ActiveSquadForeground, ExtraTextIntro, HorionztalMirror));

            if (BattleResult.ArrayResult[0].AttackMissed)
            {
                ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndMiss, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                    TargetSquadBackground, TargetSquadForeground, ExtraTextMiss, HorionztalMirror));
            }
            else
            {
                // Check for support
                if (SupportDefend != null)
                {
                    ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support In", this, EnemySquad, SupportDefend, ActiveAttack, BattleResult, UnitStats,
                        TargetSquadBackground, TargetSquadForeground, "", HorionztalMirror));
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) > 0)
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, SupportDefend, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, "", HorionztalMirror));
                        ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support Out", this, EnemySquad, SupportDefend, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, "", HorionztalMirror));
                    }
                    else
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, SupportDefend, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, "", HorionztalMirror));
                        ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support Destroyed", this, EnemySquad, SupportDefend, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, "", HorionztalMirror));
                    }
                }
                else
                {
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) <= 0)
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, ExtraTextKill, HorionztalMirror));
                    }
                    else
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats,
                            TargetSquadBackground, TargetSquadForeground, ExtraTextHit, HorionztalMirror));
                    }
                }
            }

            return ListNextAnimationScreen;
        }
    }
}
