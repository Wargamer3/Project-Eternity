using System;
using System.Collections.Generic;
using ProjectEternity.Core;
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
        public void ComputeTargetPlayerOffense(int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport, int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            if (ListPlayer[ActivePlayerIndex].IsPlayerControlled)
            {
                ActionPanelHumanAttack PlayerDefence = new ActionPanelHumanAttack(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, true));
            }
        }

        public void ComputeTargetPlayerDefence(int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport, int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            if (ListPlayer[TargetPlayerIndex].IsPlayerControlled)
            {
                ActionPanelHumanDefend PlayerDefence = new ActionPanelHumanDefend(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                //Skip defense
                ComputeTargetPlayerOffense(ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
            }
        }

        public void ReadyNextMAPAttack(int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport, int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
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

            PrepareDefenseSquadForBattle(this, ActivePlayerIndex, ActiveSquadIndex, TargetPlayerIndex, TargetSquadIndex);
            PrepareAttackSquadForBattle(this, ActiveSquad, TargetSquad);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, false ));
        }

        public void AttackWithMAPAttack(int ActivePlayerIndex, int ActiveSquadIndex, Stack<Tuple<int, int>> ListMAPAttackTarget)
        {
            Squad ActiveSquad = ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            this.ListMAPAttackTarget = ListMAPAttackTarget;
            Tuple<int, int> FirstEnemy = ListMAPAttackTarget.Pop();
            PrepareSquadsForBattle(ActivePlayerIndex, ActiveSquadIndex, FirstEnemy.Item1, FirstEnemy.Item2);

            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(this, ActivePlayerIndex, ActiveSquad, FirstEnemy.Item1, FirstEnemy.Item2);

            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(this, FirstEnemy.Item1, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2]);

            ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelStartBattle(this, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, FirstEnemy.Item1, FirstEnemy.Item2, TargetSquadSupport, false));

            PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2].Position));
            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
        }

        public void SelectMAPEnemies(int ActivePlayerIndex, int ActiveSquadIndex, List<Vector3> AttackChoice)
        {
            if (ActiveSquad.CurrentLeader.CurrentAttack.MAPAttributes.Delay > 0)
            {
                ListDelayedAttack.Add(new DelayedAttack(ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad, ActivePlayerIndex, AttackChoice));
                ListActionMenuChoice.RemoveAllSubActionPanels();
                ActiveSquad.EndTurn();
            }
            else
            {
                Stack<Tuple<int, int>> ListMAPAttackTarget = GetEnemies(ActiveSquad.CurrentLeader.CurrentAttack, AttackChoice);

                if (ListMAPAttackTarget.Count > 0)
                {
                    GlobalDeathmatchContext.ArrayAttackPosition = AttackChoice.ToArray();

                    AttackWithMAPAttack(ActivePlayerIndex, ActiveSquadIndex, ListMAPAttackTarget);

                    //Remove Ammo if needed.
                    if (ActiveSquad.CurrentLeader.CurrentAttack.MaxAmmo > 0)
                        --ActiveSquad.CurrentLeader.CurrentAttack.Ammo;
                }
            }
        }

        public List<Tuple<int, int>> CanSquadAttackWeapon(Squad ActiveSquad, Vector3 Position, Attack ActiveWeapon, int MinRange, int MaxRange, bool CanMove, StatsBoosts ActiveUnitBoosts)
        {
            bool CanAttackPostMovement = (ActiveWeapon.Sec & WeaponSecondaryProperty.PostMovement) == WeaponSecondaryProperty.PostMovement || ActiveUnitBoosts.PostMovementModifier.Attack;

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

                    ActiveWeapon.UpdateAttack(ActiveSquad.CurrentLeader, Position, ListPlayer[P].ListSquad[TargetSelect].Position,
                        ListPlayer[P].ListSquad[TargetSelect].ArrayMapSize, ListPlayer[P].ListSquad[TargetSelect].CurrentMovement, CanMove);

                    //Make sure you can use it.
                    if (ActiveWeapon.CanAttack)
                    {
                        List<Vector3> ListRealChoice;

                        if (CanMove)
                        {
                            //Move to be in range.
                            ListRealChoice = new List<Vector3>(GetMVChoice(ActiveSquad));
                        }
                        else
                        {
                            ListRealChoice = new List<Vector3>();
                        }

                        ListRealChoice.Add(ActiveSquad.Position);

                        for (int M = 0; M < ListRealChoice.Count; M++)
                        {//Remove every MV that would make it impossible to attack.
                            float Distance = Math.Abs(ListRealChoice[M].X - ListPlayer[P].ListSquad[TargetSelect].X) + Math.Abs(ListRealChoice[M].Y - ListPlayer[P].ListSquad[TargetSelect].Y);
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

        public Stack<Tuple<int, int>> GetEnemies(Attack ActiveAttack, List<Vector3> ListAttackPosition)
        {
            Stack<Tuple<int, int>> ListMAPAttackTarget = new Stack<Tuple<int, int>>();//Player index, Squad index.

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int i = 0; i < ListAttackPosition.Count; i++)
                {
                    //Find if a Unit is under the cursor.
                    int TargetIndex = CheckForSquadAtPosition(P, ListAttackPosition[i], Vector3.Zero);
                    //If one was found.
                    if (TargetIndex >= 0 && (ActiveAttack.MAPAttributes.FriendlyFire ||
                                                ListPlayer[ActivePlayerIndex].Team != ListPlayer[P].Team))
                    {
                        ListMAPAttackTarget.Push(new Tuple<int, int>(P, TargetIndex));
                    }
                }
            }

            return ListMAPAttackTarget;
        }

        public void PrepareSquadsForBattle(int ActivePlayerIndex, int ActiveSquadIndex,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            AttackingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

            UpdateWingmansSelection(AttackingSquad, DefendingSquad, BattleMenuOffenseFormationChoice);

            //Simulate defense reaction.
            PrepareDefenseSquadForBattle(this, ActivePlayerIndex, ActiveSquadIndex, DefendingPlayerIndex, DefendingSquadIndex);
            
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
                    ActiveSquad.CurrentWingmanA.AttackIndex = -1;
                    ActiveSquad.CurrentWingmanA.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanA.PLAAttack >= 0)
                    {
                        Attack PLAAttack = ActiveSquad.CurrentWingmanA.ListAttack[ActiveSquad.CurrentWingmanA.PLAAttack];

                        PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentMovement, true);

                        if (PLAAttack.CanAttack)
                        {
                            ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            ActiveSquad.CurrentWingmanA.AttackIndex = ActiveSquad.CurrentWingmanA.PLAAttack;
                            ActiveSquad.CurrentWingmanA.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad,
                                ActiveSquad.CurrentWingmanA, OppositeSquad, OppositeSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                        }
                    }
                }

                if (ActiveSquad.CurrentWingmanB != null)
                {
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanB.AttackIndex = -1;
                    ActiveSquad.CurrentWingmanB.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanB.PLAAttack >= 0)
                    {
                        Attack PLAAttack = ActiveSquad.CurrentWingmanB.ListAttack[ActiveSquad.CurrentWingmanA.PLAAttack];

                        PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentMovement, true);

                        if (PLAAttack.CanAttack)
                        {
                            ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            ActiveSquad.CurrentWingmanB.AttackIndex = ActiveSquad.CurrentWingmanB.PLAAttack;
                            ActiveSquad.CurrentWingmanB.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad,
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
                    ActiveSquad.CurrentWingmanA.AttackIndex = -1;
                    ActiveSquad.CurrentWingmanA.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanA.PLAAttack >= 0)
                    {
                        Attack PLAAttack = ActiveSquad.CurrentWingmanA.ListAttack[ActiveSquad.CurrentWingmanA.PLAAttack];
                        
                        if (OppositeSquad.CurrentWingmanA != null)
                        {
                            PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentMovement, true);

                            if (PLAAttack.CanAttack)
                            {
                                ActiveSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                                ActiveSquad.CurrentWingmanA.AttackIndex = ActiveSquad.CurrentWingmanA.PLAAttack;
                                ActiveSquad.CurrentWingmanA.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad,
                                    ActiveSquad.CurrentWingmanA, OppositeSquad, OppositeSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                            }
                        }
                    }
                }

                if (ActiveSquad.CurrentWingmanB != null)
                {
                    ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    ActiveSquad.CurrentWingmanB.AttackIndex = -1;
                    ActiveSquad.CurrentWingmanB.AttackAccuracy = "";

                    if (ActiveSquad.CurrentWingmanB.PLAAttack >= 0)
                    {
                        Attack PLAAttack = ActiveSquad.CurrentWingmanB.ListAttack[ActiveSquad.CurrentWingmanA.PLAAttack];
                        
                        if (OppositeSquad.CurrentWingmanB != null)
                        {
                            PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.ArrayMapSize, OppositeSquad.CurrentMovement, true);

                            if (PLAAttack.CanAttack)
                            {
                                ActiveSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                                ActiveSquad.CurrentWingmanB.AttackIndex = ActiveSquad.CurrentWingmanB.PLAAttack;
                                ActiveSquad.CurrentWingmanB.AttackAccuracy = CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad,
                                    ActiveSquad.CurrentWingmanB, OppositeSquad, OppositeSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                            }
                        }
                    }
                }
            }
        }

        public static void PrepareAttackSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            AttackingSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(AttackingSquad.CurrentLeader, AttackingSquad,
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
                    AttackingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(AttackingSquad.CurrentLeader, AttackingSquad,
                                                                        DefendingSquad.CurrentWingmanA, DefendingSquad,
                                                                        DefendingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }

                if (DefendingSquad.CurrentWingmanB != null)
                {
                    AttackingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(AttackingSquad.CurrentLeader, AttackingSquad,
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
            int ActivePlayerIndex, int ActiveSquadIndex,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            switch (DefendingSquad.SquadDefenseBattleBehavior)
            {
                case "Always Counterattack":
                    AlwaysCounterattackDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, DefendingPlayerIndex, DefendingSquadIndex);
                    break;

                case "Simple Counterattack":
                    SimpleCounterattackDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, DefendingPlayerIndex, DefendingSquadIndex);
                    break;

                case "Always Block":
                    AlwaysBlockDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Always Dodge":
                    AlwaysDodgeDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Smart Counterattack":
                default:
                    DefaultDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, DefendingPlayerIndex, DefendingSquadIndex);
                    break;
            }
        }

        private static void AlwaysCounterattackDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.AttackIndex = -1;

            UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, 1,
                                    null, BattleMap.WingmanDamageModifier,
                                    null, BattleMap.WingmanDamageModifier, 0,
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
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanA.AttackIndex = DefendingSquad.CurrentWingmanA.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanA.AttackIndex = -1;

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
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 1, true, true, Map);
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
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanB.AttackIndex = DefendingSquad.CurrentWingmanB.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanB.AttackIndex = -1;

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
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null,
                        BattleMap.WingmanDamageModifier, CounterUnitIndex, DefendingPlayerIndex, DefendingSquadIndex, 2, true, true, Map);
                }

                if (DefendingSquad.CurrentWingmanB.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.DoNothing;
                }
            }

            #endregion

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void SimpleCounterattackDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.AttackIndex = -1;

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1,
                                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                                        AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier,
                                        0,
                                        DefendingPlayerIndex, DefendingSquadIndex, 0,
                                        false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
                                        0,
                                        DefendingPlayerIndex, DefendingSquadIndex, 0,
                                        false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(ActivePlayerIndex, ActiveSquadIndex,
                                        AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
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
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanA.AttackIndex = DefendingSquad.CurrentWingmanA.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanA.AttackIndex = -1;
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
                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1,
                        null, WingmanDamageModifier,
                        null,BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1,
                        null, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, false, Map);
                }
            }

            #endregion

            #region Wingman B

            if (DefendingSquad.CurrentWingmanB != null)
            {
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanB.AttackIndex = DefendingSquad.CurrentWingmanB.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanB.AttackIndex = -1;
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
                        AttackingSquad.CurrentWingmanB, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1,
                        null, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1,
                        null, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, false, Map);
                }
            }

            #endregion
            
            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void AlwaysBlockDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
            DefendingSquad.CurrentLeader.AttackIndex = -1;
            
            if (DefendingSquad.CurrentWingmanA != null)
            {
                DefendingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                DefendingSquad.CurrentWingmanA.AttackIndex = -1;
            }

            if (DefendingSquad.CurrentWingmanB != null)
            {
                DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                DefendingSquad.CurrentWingmanB.AttackIndex = -1;
            }

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void AlwaysDodgeDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DefendingSquad.CurrentLeader.AttackIndex = -1;

            if (DefendingSquad.CurrentWingmanA != null)
            {
                DefendingSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                DefendingSquad.CurrentWingmanA.AttackIndex = -1;
            }

            if (DefendingSquad.CurrentWingmanB != null)
            {
                DefendingSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                DefendingSquad.CurrentWingmanB.AttackIndex = -1;
            }

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void DefaultDefenseSquadForBattle(DeathmatchMap Map,
            int ActivePlayerIndex, int ActiveSquadIndex,
            int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad AttackingSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);
            DefendingSquad.CurrentLeader.AttackIndex = -1;

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1,
                    AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                    AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier,
                    0,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1,
                    null, BattleMap.WingmanDamageModifier,
                    null, BattleMap.WingmanDamageModifier,
                    0,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(
                    ActivePlayerIndex, ActiveSquadIndex,
                    AttackingSquad.CurrentLeader, 1,
                    null, BattleMap.WingmanDamageModifier,
                    null, BattleMap.WingmanDamageModifier,
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
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanA, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyA = null;

                if (AttackingSquad.CurrentWingmanB != null)
                {
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(DefendingSquad.CurrentLeader, DefendingSquad,
                                                                        AttackingSquad.CurrentWingmanB, AttackingSquad,
                                                                        AttackingSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                }
                else
                    DefendingSquad.CurrentLeader.MAPAttackAccuracyB = null;
            }

            #region Wingman A

            if (DefendingSquad.CurrentWingmanA != null)
            {
                if (DefendingSquad.CurrentWingmanA.CanAttack && DefendingSquad.CurrentWingmanA.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingSquad.CurrentWingmanA.AttackIndex = DefendingSquad.CurrentWingmanA.PLAAttack;
                }
                else
                {
                    DefendingSquad.CurrentWingmanA.AttackIndex = -1;
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
                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1,
                        null, WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1,
                        null, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 1,
                        true, true, Map);
                }
            }

            #endregion

            #region Wingman B

            if (DefendingSquad.CurrentWingmanB != null)
            {
                if (DefendingSquad.CurrentWingmanB.CanAttack && DefendingSquad.CurrentWingmanB.PLAAttack >= 0 &&
                    Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL &&
                    DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    DefendingSquad.CurrentWingmanB.AttackIndex = DefendingSquad.CurrentWingmanB.PLAAttack;
                else
                    DefendingSquad.CurrentWingmanB.AttackIndex = -1;

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
                        AttackingSquad.CurrentWingmanB, 1,
                        null, BattleMap.WingmanDamageModifier,
                        null, BattleMap.WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        null, 1,
                        null, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(
                        ActivePlayerIndex, ActiveSquadIndex,
                        AttackingSquad.CurrentLeader, 1,
                        null, WingmanDamageModifier,
                        null, WingmanDamageModifier,
                        TargetUnitIndex,
                        DefendingPlayerIndex, DefendingSquadIndex, 2,
                        true, true, Map);
                }
            }

            #endregion

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void UnitBaseDefencePattern(
            int ActivePlayerIndex, int ActiveSquadIndex,
            Unit Attacker1, float DamageModifier1,
            Unit Attacker2, float DamageModifier2,
            Unit Attacker3, float DamageModifier3,
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
                HitRate1 = Map.CalculateHitRate(Attacker1, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            if (Attacker2 == null || Attacker2.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                HitRate2 = 0;
            else
                HitRate2 = Map.CalculateHitRate(Attacker2, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            if (Attacker3 == null || Attacker3.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack)
                HitRate3 = 0;
            else
                HitRate3 = Map.CalculateHitRate(Attacker3, AttackerSquad, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack);

            int DamageTaken = 0;
            int TempDamage;

            if (HitRate1 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker1, AttackerSquad, DamageModifier1, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate2 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker2, AttackerSquad, DamageModifier2, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - (TempDamage + DamageTaken) < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate3 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker3, AttackerSquad, DamageModifier3, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
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

                DefenderUnit.UpdateNonMAPAttacks(DefenderSquad.Position, AttackerSquad.Position, AttackerSquad.ArrayMapSize, AttackerSquad.CurrentMovement, true);

                if (!UsePLAWeapon)
                {
                    int FinalAttackChoice = -1;

                    //Find the weapon with the most power.
                    for (DefenderUnit.AttackIndex = 0; DefenderUnit.AttackIndex < DefenderUnit.ListAttack.Count; ++DefenderUnit.AttackIndex)
                    {
                        if (DefenderUnit.CurrentAttack.CanAttack)
                        {
                            int Damage = Map.DamageFormula(DefenderUnit, DefenderSquad, 1, ActivePlayerIndex, ActiveSquadIndex, TargetCounterIndex, TargetCounter.BattleDefenseChoice, false).AttackDamage;

                            if (Damage > DamageOld)
                            {
                                FinalAttackChoice = DefenderUnit.AttackIndex;
                                DamageOld = Damage;
                            }
                        }
                    }
                    DefenderUnit.AttackIndex = FinalAttackChoice;
                }
            }

            //You can counter and survive the attack.
            if (DefenderUnit.AttackIndex >= 0)
            {
                DefenderUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                DefenderUnit.AttackAccuracy = Map.CalculateHitRate(DefenderUnit, DefenderSquad, TargetCounter, AttackerSquad, TargetCounter.BattleDefenseChoice).ToString() + "%";
            }
            else if (TryToDefendAttack)//Can't counter or will die if attacked.
            {
                DamageTaken = 0;

                if (HitRate1 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker1, AttackerSquad, DamageModifier1, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate2 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker2, AttackerSquad, DamageModifier2, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate3 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker3, AttackerSquad, DamageModifier3, TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
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

        public void GetLeftRightSquads(bool IsActiveSquadOnRight,
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

        public AnimationScreen CreateAnimation(string AnimationName, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
            SquadBattleResult BattleResult, AnimationScreen.AnimationUnitStats UnitStats, AnimationBackground ActiveTerrain, string ExtraText, bool IsLeftAttacking)
        {
            AnimationScreen NewAnimationScreen = new AnimationScreen(AnimationName, Map, ActiveSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveTerrain, ExtraText, IsLeftAttacking);

            return NewAnimationScreen;
        }

        public AnimationScreen CreateAnimation(AnimationInfo Info, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
            SquadBattleResult BattleResult, AnimationScreen.AnimationUnitStats UnitStats, AnimationBackground ActiveTerrain, string ExtraText, bool IsLeftAttacking)
        {
            AnimationScreen NewAnimationScreen = new AnimationScreen(Info.AnimationName, Map, ActiveSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveTerrain, ExtraText, IsLeftAttacking);

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
            AnimationScreen.AnimationUnitStats UnitStats, AnimationScreen.BattleAnimationTypes BattleAnimationType, SquadBattleResult AttackingResult)
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
            string ActiveTerrain = NonDemoRightSquad.CurrentMovement;
            SquadBattleResult BattleResult = AttackingResult;
            Squad EnemySquad = NonDemoLeftSquad;
            Squad EnemySupport = NonDemoLeftSupport;

            if (BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftAttackRight || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftConteredByRight)
            {
                AttackingSquad = NonDemoLeftSquad;
                ActiveUnitSupport = NonDemoLeftSupport;
                ActiveAttack = AttackingSquad.CurrentLeader.CurrentAttack;
                ActiveTerrain = NonDemoLeftSquad.CurrentMovement;
                EnemySquad = NonDemoRightSquad;
                EnemySupport = NonDemoRightSupport;
            }

            string ExtraTextIntro = AttackingSquad.CurrentLeader.ItemName + " Attacks!";
            string ExtraTextHit = AttackingSquad.CurrentLeader.ItemName + " hits! " + EnemySquad.CurrentLeader.ItemName + " takes " + BattleResult.ArrayResult[0].AttackDamage + " damage!";
            string ExtraTextMiss = AttackingSquad.CurrentLeader.ItemName + " misses! " + EnemySquad.CurrentLeader.ItemName + " takes 0 damage!";
            string ExtraTextKill = EnemySquad.CurrentLeader.ItemName + " is destroyed!";

            AnimationBackground ActiveSquadBackground;
            AnimationBackground TargetSquadBackground;

            string ActiveSquadBackgroundPath = "Backgrounds 2D/Empty";
            string TargetSquadBackgroundPath = "Backgrounds 2D/Empty";
            DrawableTile ActiveSquadTile = GetTile(ActiveSquad);
            DrawableTile TargetSquadTile = GetTile(TargetSquad);
            Terrain ActiveSquadTerrain = GetTerrain(ActiveSquad);
            Terrain TargetSquadTerrain = GetTerrain(TargetSquad);

            if (ActiveSquadTerrain.BattleBackgroundAnimationIndex >= 0)
            {
                ActiveSquadBackgroundPath = ListTilesetPreset[ActiveSquadTile.Tileset].ListBattleBackgroundAnimationPath[ActiveSquadTerrain.BattleBackgroundAnimationIndex];
            }

            if (TargetSquadTerrain.BattleBackgroundAnimationIndex >= 0)
            {
                TargetSquadBackgroundPath = ListTilesetPreset[TargetSquadTile.Tileset].ListBattleBackgroundAnimationPath[TargetSquadTerrain.BattleBackgroundAnimationIndex];
            }

            ActiveSquadBackground = new AnimationBackground2D(ActiveSquadBackgroundPath, Content, GraphicsDevice);

            if (ActiveSquadBackgroundPath == TargetSquadBackgroundPath)
            {
                TargetSquadBackground = ActiveSquadBackground;
            }
            else
            {
                TargetSquadBackground = new AnimationBackground2D(TargetSquadBackgroundPath, Content, GraphicsDevice);
            }

            AttackAnimations AttackerAnimations = ActiveAttack.GetAttackAnimations(FormulaParser.ActiveParser);

            if (!IsCounter)
            {
                ListNextAnimationScreen.Add(CreateAnimation(AttackingSquad.CurrentLeader.Animations.MoveFoward, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
            }
            ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.Start, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, ExtraTextIntro, HorionztalMirror));

            if (BattleResult.ArrayResult[0].AttackMissed)
            {
                ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndMiss, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, TargetSquadBackground, ExtraTextMiss, HorionztalMirror));
            }
            else
            {
                // Check for support
                if (EnemySupport != null)
                {
                    ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support In", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) > 0)
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
                        ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support Out", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
                    }
                    else
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
                        ListNextAnimationScreen.Add(CreateAnimation("Default Animations/Support Destroyed", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveSquadBackground, "", HorionztalMirror));
                    }
                }
                else
                {
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) <= 0)
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, TargetSquadBackground, ExtraTextKill, HorionztalMirror));
                    }
                    else
                    {
                        ListNextAnimationScreen.Add(CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, TargetSquadBackground, ExtraTextHit, HorionztalMirror));
                    }
                }
            }

            return ListNextAnimationScreen;
        }
    }
}
