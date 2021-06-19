using System;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class DeathmatchMap
    {
        public void ComputeTargetPlayerOffense(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int ActivePlayerIndex, Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int TargetPlayerIndex)
        {
            if (ListPlayer[ActivePlayerIndex].IsHuman)
            {
                ActionPanelHumanAttack PlayerDefence = new ActionPanelHumanAttack(this, ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, TargetSquad, TargetSquadSupport, TargetPlayerIndex);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                //Begin attack.
                InitPlayerDefence(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, TargetSquad, TargetSquadSupport, TargetPlayerIndex);
            }
        }

        public void ComputeTargetPlayerDefence(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int ActivePlayerIndex, Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int TargetPlayerIndex)
        {
            if (ListPlayer[TargetPlayerIndex].IsHuman)
            {
                ActionPanelHumanDefend PlayerDefence = new ActionPanelHumanDefend(this, ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, TargetSquad, TargetSquadSupport, TargetPlayerIndex);
                ListActionMenuChoice.Add(PlayerDefence);
                PlayerDefence.OnSelect();
            }
            else
            {
                //Skip defense
                ComputeTargetPlayerOffense(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, TargetSquad, TargetSquadSupport, TargetPlayerIndex);
            }
        }

        public void ReadyNextMAPAttack(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int ActivePlayerIndex, Squad Defender, SupportSquadHolder TargetSquadSupport, int TargetPlayerIndex)
        {
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

            PrepareDefenseSquadForBattle(this, ActiveSquad, TargetSquad);
            PrepareAttackSquadForBattle(this, ActiveSquad, TargetSquad);

            InitPlayerAttack(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, Defender, TargetSquadSupport, TargetPlayerIndex);
        }

        public void AttackWithMAPAttack(Squad ActiveSquad, int ActivePlayerIndex, Stack<Tuple<int, int>> ListMAPAttackTarget)
        {
            this.ListMAPAttackTarget = ListMAPAttackTarget;
            Tuple<int, int> FirstEnemy = ListMAPAttackTarget.Pop();
            PrepareSquadsForBattle(ActiveSquad, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2]);

            SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquadSupport.PrepareAttackSupport(this, ActivePlayerIndex, ActiveSquad, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2]);

            SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
            TargetSquadSupport.PrepareDefenceSupport(this, FirstEnemy.Item1, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2]);

            InitPlayerAttack(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2], TargetSquadSupport, FirstEnemy.Item1);
            PushScreen(new CenterOnSquadCutscene(CenterCamera, this, ListPlayer[FirstEnemy.Item1].ListSquad[FirstEnemy.Item2].Position));
            ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
        }

        public void SelectMAPEnemies(Squad ActiveSquad, int ActivePlayerIndex, List<Vector3> AttackChoice)
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

                    AttackWithMAPAttack(ActiveSquad, ActivePlayerIndex, ListMAPAttackTarget);

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

                    ActiveWeapon.UpdateAttack(ActiveSquad.CurrentLeader, Position, ListPlayer[P].ListSquad[TargetSelect].Position, ListPlayer[P].ListSquad[TargetSelect].CurrentMovement, CanMove);

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

        public void PrepareSquadsForBattle(Squad ActiveSquad, Squad TargetSquad)
        {
            ActiveSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

            UpdateWingmansSelection(ActiveSquad, TargetSquad, BattleMenuOffenseFormationChoice);

            //Simulate defense reaction.
            PrepareDefenseSquadForBattle(this, ActiveSquad, TargetSquad);
            
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

                        PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.CurrentMovement, true);

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

                        PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.CurrentMovement, true);

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
                            PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanA, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.CurrentMovement, true);

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
                            PLAAttack.UpdateAttack(ActiveSquad.CurrentWingmanB, ActiveSquad.Position, OppositeSquad.Position, OppositeSquad.CurrentMovement, true);

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
        public static void PrepareDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            switch (DefendingSquad.SquadDefenseBattleBehavior)
            {
                case "Always Counterattack":
                    AlwaysCounterattackDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Simple Counterattack":
                    SimpleCounterattackDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Always Block":
                    AlwaysBlockDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Always Dodge":
                    AlwaysDodgeDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;

                case "Smart Counterattack":
                default:
                    DefaultDefenseSquadForBattle(Map, AttackingSquad, DefendingSquad);
                    break;
            }
        }

        private static void AlwaysCounterattackDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.AttackIndex = -1;

            UnitBaseDefencePattern(null, 1,
                                    null, BattleMap.WingmanDamageModifier,
                                    null, BattleMap.WingmanDamageModifier,
                                    AttackingSquad, AttackingSquad.CurrentLeader,
                                    DefendingSquad.CurrentLeader, DefendingSquad, false, true, Map);

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

                Unit TargetUnit = null;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                    TargetUnit = AttackingSquad.CurrentWingmanA;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(null, 1, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
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

                Unit TargetUnit = null;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                    TargetUnit = AttackingSquad.CurrentWingmanB;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(null, WingmanDamageModifier, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
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

        private static void SimpleCounterattackDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);

            DefendingSquad.CurrentLeader.AttackIndex = -1;

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                                        AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, false, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, false, Map);
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

                Unit TargetUnit = null;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                    TargetUnit = AttackingSquad.CurrentWingmanA;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, false, Map);
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

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                    TargetUnit = AttackingSquad.CurrentWingmanB;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentWingmanB, WingmanDamageModifier, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, false, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, false, Map);
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

        private static void DefaultDefenseSquadForBattle(DeathmatchMap Map, Squad AttackingSquad, Squad DefendingSquad)
        {
            Map.UpdateWingmansSelection(DefendingSquad, AttackingSquad, Map.BattleMenuDefenseFormationChoice);
            DefendingSquad.CurrentLeader.AttackIndex = -1;

            if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Focused)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier,
                                        AttackingSquad.CurrentWingmanB, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.Spread)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, true, Map);
            }
            else if (Map.BattleMenuOffenseFormationChoice == BattleMap.FormationChoices.ALL)
            {
                UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1,
                                        null, BattleMap.WingmanDamageModifier,
                                        null, BattleMap.WingmanDamageModifier,
                                        AttackingSquad, AttackingSquad.CurrentLeader,
                                        DefendingSquad.CurrentLeader, DefendingSquad, false, true, Map);
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

                Unit TargetUnit = null;

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                    TargetUnit = AttackingSquad.CurrentWingmanA;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanA != null)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentWingmanA, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1, null, BattleMap.WingmanDamageModifier, null, BattleMap.WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanA, DefendingSquad, true, true, Map);
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

                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                    TargetUnit = AttackingSquad.CurrentWingmanB;
                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                    TargetUnit = AttackingSquad.CurrentLeader;

                if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread && AttackingSquad.CurrentWingmanB != null)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentWingmanB, WingmanDamageModifier, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                {
                    UnitBaseDefencePattern(null, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
                }
                else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL)
                {
                    UnitBaseDefencePattern(AttackingSquad.CurrentLeader, 1, null, WingmanDamageModifier, null, WingmanDamageModifier, AttackingSquad, TargetUnit,
                                        DefendingSquad.CurrentWingmanB, DefendingSquad, true, true, Map);
                }
            }

            #endregion

            //Compute the Attacker's accuracy and Wingmans reaction.
            PrepareAttackSquadForBattle(Map, AttackingSquad, DefendingSquad);
        }

        private static void UnitBaseDefencePattern(Unit Attacker1, float DamageModifier1,
                                            Unit Attacker2, float DamageModifier2,
                                            Unit Attacker3, float DamageModifier3,
            Squad AttackerSquad, Unit TargetCounter,
            Unit DefenderUnit, Squad DefenderSquad, bool UsePLAWeapon, bool TryToDefendAttack,
            DeathmatchMap Map)
        {
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
                TempDamage = Map.DamageFormula(Attacker1, AttackerSquad, DamageModifier1, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate2 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker2, AttackerSquad, DamageModifier2, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
                if (DefenderUnit.HP - (TempDamage + DamageTaken) < DefenderUnit.Boosts.HPMinModifier)
                    DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                else
                    DamageTaken += TempDamage;
            }

            if (HitRate3 > 0)
            {
                TempDamage = Map.DamageFormula(Attacker3, AttackerSquad, DamageModifier3, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Attack, false).AttackDamage;
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

                DefenderUnit.UpdateNonMAPAttacks(DefenderSquad.Position, AttackerSquad.Position, AttackerSquad.CurrentMovement, true);

                if (!UsePLAWeapon)
                {
                    int FinalAttackChoice = -1;

                    //Find the weapon with the most power.
                    for (DefenderUnit.AttackIndex = 0; DefenderUnit.AttackIndex < DefenderUnit.ListAttack.Count; ++DefenderUnit.AttackIndex)
                    {
                        if (DefenderUnit.CurrentAttack.CanAttack)
                        {
                            int Damage = Map.DamageFormula(DefenderUnit, DefenderSquad, 1, TargetCounter, AttackerSquad, TargetCounter.BattleDefenseChoice, false).AttackDamage;

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
                    TempDamage = Map.DamageFormula(Attacker1, AttackerSquad, DamageModifier1, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate2 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker2, AttackerSquad, DamageModifier2, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
                    if (DefenderUnit.HP - TempDamage < DefenderUnit.Boosts.HPMinModifier)
                        DamageTaken = DefenderUnit.HP - DefenderUnit.Boosts.HPMinModifier;
                    else
                        DamageTaken += TempDamage;
                }

                if (HitRate3 > 0)
                {
                    TempDamage = Map.DamageFormula(Attacker3, AttackerSquad, DamageModifier3, DefenderUnit, DefenderSquad, Unit.BattleDefenseChoices.Defend, false).AttackDamage;
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
    }
}
