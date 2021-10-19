using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.NonDemoScreen.NonDemoBattleFrame;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public partial class NonDemoScreen : GameScreen
    {
        public class NonDemoSharedUnitStats
        {
            public readonly int StartingHP;
            public readonly int StartingEN;
            public readonly int EndEN;
            public readonly Unit SharedUnit;
            public readonly BattleResult AttackerSquadResult;

            public int VisibleHP;
            public int VisibleEN;

            public NonDemoSharedUnitStats(Unit SharedUnit, BattleResult AttackerSquadResult)
            {
                this.SharedUnit = SharedUnit;
                this.AttackerSquadResult = AttackerSquadResult;

                VisibleHP = StartingHP = SharedUnit.HP;
                VisibleEN = StartingEN = SharedUnit.EN;
                EndEN = AttackerSquadResult.AttackAttackerFinalEN;
            }
        }

        #region Ressources

        private Texture2D sprNonDemoMiss;
        private Texture2D sprNonDemoCritical;
        private FMODSound sndNonDemoAttack;
        private AnimatedSprite sprNonDemoExplosion;

        #endregion

        #region Variables

        private int NonDemoAnimationTimer;
        public List<NonDemoBattleFrame> ListNonDemoBattleFrame;
        private int CurrentNonDemoBattleFrame;

        private readonly Vector2 NonDemoRightUnitPosition = new Vector2(Constants.Width / 2 + 47, 160);
        private readonly Vector2 NonDemoLeftUnitPosition = new Vector2(Constants.Width / 2 - 47 - 113, 160);

        DeathmatchMap Map;

        Squad AttackingSquad;
        SupportSquadHolder AttackingSupport;
        Squad DefendingSquad;
        SupportSquadHolder DefendingSupport;

        SquadBattleResult AttackerSquadResult;
        SquadBattleResult DefenderSquadResult;
        private int AttackerPlayerIndex;
        private int DefenderPlayerIndex;

        #endregion

        [Flags]
        public enum NonDemoUnitStancePositions : byte
        {
            Leader = 0x1, WingmanA = 0x2, WingmanB = 0x4, Support = 0x8
        };

        public NonDemoScreen(DeathmatchMap Map)
        {
            this.Map = Map;
            ListNonDemoBattleFrame = new List<NonDemoBattleFrame>();
            if (Map != null)
                ListGameScreen = Map.ListGameScreen;
        }

        public override void Load()
        {
            sndNonDemoAttack = new FMODSound(FMODSystem, "Content/SFX/hit sound.mp3");

            sprNonDemoMiss = Content.Load<Texture2D>("Battle/Miss");
            sprNonDemoCritical = Content.Load<Texture2D>("Battle/Critical");
            sprNonDemoExplosion = new AnimatedSprite(Content, "Animations/Bitmap Animations/Explosion_strip3", new Vector2(), 10f);

            sprNonDemoExplosion.EndAnimation();
        }

        public void InitNonDemo(Squad AttackingSquad, SupportSquadHolder AttackingSupport, int AttackerPlayerIndex, SquadBattleResult AttackerSquadResult, FormationChoices AttackerFormation,
            Squad DefendingSquad, SupportSquadHolder DefendingSupport, int DefenderPlayerIndex, SquadBattleResult DefenderSquadResult, FormationChoices DefenderFormation, bool IsRightAttacking)
        {
            this.AttackingSquad = AttackingSquad;
            this.AttackingSupport = AttackingSupport;
            this.AttackerPlayerIndex = AttackerPlayerIndex;
            this.DefendingSquad = DefendingSquad;
            this.DefendingSupport = DefendingSupport;
            this.DefenderPlayerIndex = DefenderPlayerIndex;
            this.AttackerSquadResult = AttackerSquadResult;
            this.DefenderSquadResult = DefenderSquadResult;

            NonDemoAnimationTimer = -1;
            CurrentNonDemoBattleFrame = 0;

            bool CanDefenderCounter = AttackingSquad.CurrentLeader.CurrentAttack.Pri != Core.Attacks.WeaponPrimaryProperty.MAP;

            NonDemoAnimationTimer = 50;

            int[] ArrayAttackerHP = new int[AttackingSquad.UnitsAliveInSquad];
            for (int i = 0; i < AttackingSquad.UnitsAliveInSquad; i++)
            {
                ArrayAttackerHP[i] = AttackingSquad[i].HP;
            }

            int[] ArrayDefenderHP = new int[DefendingSquad.UnitsAliveInSquad];
            for (int i = 0; i < DefendingSquad.UnitsAliveInSquad; i++)
            {
                ArrayDefenderHP[i] = DefendingSquad[i].HP;
            }

            NonDemoBattleFrame DefaultNonDemoBattleFrame = GetDefaultNonDemoBattleFrame(AttackerSquadResult, AttackingSquad, AttackingSupport.ActiveSquadSupport,
                                                            DefenderSquadResult, DefendingSquad, DefendingSupport.ActiveSquadSupport, IsRightAttacking);

            ListNonDemoBattleFrame.Add(DefaultNonDemoBattleFrame);

            FillCombatAnimations(DefaultNonDemoBattleFrame, AttackingSquad, AttackingSupport.ActiveSquadSupport, AttackerSquadResult, AttackerFormation,
                DefendingSquad, DefendingSupport.ActiveSquadSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);

            if (ArrayDefenderHP[0] > 0 && CanDefenderCounter && DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                FillCombatAnimations(DefaultNonDemoBattleFrame, DefendingSquad, null, DefenderSquadResult, DefenderFormation,
                    AttackingSquad, null, !IsRightAttacking, ref ArrayAttackerHP, ArrayDefenderHP);
            }

            if (AttackerSquadResult.ResultSupportAttack.Target != null)
            {
                ListNonDemoBattleFrame.Add(GetSwitchWithLeaderFrame(DefaultNonDemoBattleFrame, IsRightAttacking));
                NonDemoBattleFrame SupportNonDemoBattleFrame = GetSwitchWithLeaderHoldFrame(DefaultNonDemoBattleFrame, IsRightAttacking);

                ListNonDemoBattleFrame.Add(GetStartFrame(SupportNonDemoBattleFrame, NonDemoUnitStancePositions.Support, IsRightAttacking));

                NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(SupportNonDemoBattleFrame, NonDemoUnitStancePositions.Support, IsRightAttacking);

                if (AttackerSquadResult.ResultSupportAttack.AttackMissed)
                {
                    ListNonDemoBattleFrame.Add(GetMissedFrame(AttackNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));
                }
                else
                {
                    if (AttackerSquadResult.ResultSupportAttack.AttackWasCritical)
                    {
                        ListNonDemoBattleFrame.Add(GetCriticalFrame(AttackNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));
                    }

                    NonDemoBattleFrame GetHitNonDemoBattleFrame = GetGetHitFrame(AttackNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, IsRightAttacking);

                    ListNonDemoBattleFrame.Add(GetHitNonDemoBattleFrame);
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(SupportNonDemoBattleFrame, NonDemoUnitStancePositions.Support, IsRightAttacking));
            }
        }

        private void FillCombatAnimations(NonDemoBattleFrame DefaultNonDemoBattleFrame, Squad Attacker, Squad AttackerSupport, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            bool LeaderHit = false;

            if (AttackerFormation == FormationChoices.Spread)
            {
                FillCombatAnimationsWingmansSpread(DefaultNonDemoBattleFrame, Attacker, AttackerResult, AttackerFormation, Defender, DefenderSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);
            }
            else if (AttackerFormation == FormationChoices.Focused)
            {
                FillCombatAnimationsWingmansFocused(DefaultNonDemoBattleFrame, ref LeaderHit, Attacker, AttackerResult, AttackerFormation, Defender, DefenderSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);
            }
            else if (ArrayAttackerHP[0] > 0 && Attacker.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack && AttackerFormation == FormationChoices.ALL)
            {
                FillCombatAnimationsAll(DefaultNonDemoBattleFrame, Attacker, AttackerSupport, AttackerResult, AttackerFormation, Defender, DefenderSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);

                return;
            }

            FillCombatAnimationsLeader(DefaultNonDemoBattleFrame, LeaderHit, Attacker, AttackerSupport, AttackerResult, AttackerFormation, Defender, DefenderSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);
        }

        private void FillCombatAnimationsWingmansSpread(NonDemoBattleFrame DefaultNonDemoBattleFrame, Squad Attacker, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            bool UseWingmans = false;

            if (ArrayAttackerHP[1] > 0 && ArrayDefenderHP[1] > 0 &&
                Attacker.CurrentWingmanA.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                UseWingmans = true;
            }

            NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.WingmanA;

            if (ArrayAttackerHP[2] > 0 && ArrayDefenderHP[2] > 0 &&
                Attacker.CurrentWingmanB.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                Targets = NonDemoUnitStancePositions.WingmanA | NonDemoUnitStancePositions.WingmanB;
            }

            if (UseWingmans)
            {
                ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, Targets, IsRightAttacking));

                NonDemoBattleFrame MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, Targets, IsRightAttacking);

                //Both Missed.
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {//Both Critical.
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, Targets, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    else//Individual
                    {
                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (MissedFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(MissedFrame);
                            }
                            else
                            {
                                CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                                if (CriticalFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(CriticalFrame);
                                }
                            }
                        }
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, Targets, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, Targets, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    else
                    {
                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (ShieldFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(ShieldFrame);
                            }
                        }
                    }
                    if (BarrierFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(BarrierFrame);
                    }
                    else
                    {
                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (BarrierFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(BarrierFrame);
                            }
                        }
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(DefaultNonDemoBattleFrame, AttackerResult, Targets, IsRightAttacking));

                    for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(DefaultNonDemoBattleFrame, Targets, IsRightAttacking));
            }
        }

        private void FillCombatAnimationsWingmansFocused(NonDemoBattleFrame DefaultNonDemoBattleFrame, ref bool LeaderHit, Squad Attacker, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
            NonDemoUnitStancePositions[] ArrayAttackerStance = { NonDemoUnitStancePositions.WingmanA, NonDemoUnitStancePositions.WingmanB };

            for (int i = 1; i < AttackerResult.ArrayResult.Length && i < Attacker.UnitsAliveInSquad && i < Defender.UnitsAliveInSquad; i++)
            {
                if (ArrayAttackerHP[i] > 0 && Attacker[i].BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, ArrayAttackerStance[i], IsRightAttacking));

                    NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(DefaultNonDemoBattleFrame, ArrayAttackerStance[i], IsRightAttacking);

                    NonDemoBattleFrame MissedFrame = GetMissedFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                    if (MissedFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(MissedFrame);
                    }
                    else
                    {
                        if (!LeaderHit)
                        {
                            if (DefendingSupport.ActiveSquadSupport != null)
                            {
                                NonDemoBattleFrame SupportNonDemoBattleFrame = GetSwitchWithLeaderFrame(AttackNonDemoBattleFrame, IsRightAttacking);
                                ListNonDemoBattleFrame.Add(SupportNonDemoBattleFrame);
                                Targets = NonDemoUnitStancePositions.Support;
                                AttackNonDemoBattleFrame = SupportNonDemoBattleFrame;
                            }
                        }

                        LeaderHit = true;
                        NonDemoBattleFrame CriticalFrame = GetCriticalFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                        if (CriticalFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(CriticalFrame);
                        }
                        NonDemoBattleFrame ShieldFrame = GetShieldFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                        NonDemoBattleFrame BarrierFrame = GetBarrierFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);

                        if (ShieldFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(ShieldFrame);
                        }
                        if (BarrierFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(BarrierFrame);
                        }

                        ListNonDemoBattleFrame.Add(GetGetHitFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, IsRightAttacking));
                        ArrayDefenderHP[0] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[0] - AttackerResult.ArrayResult[i].AttackDamage);
                    }

                    ListNonDemoBattleFrame.Add(GetEndFrame(AttackNonDemoBattleFrame, ArrayAttackerStance[i], IsRightAttacking));

                    if (ArrayDefenderHP[0] <= 0)
                    {
                        return;
                    }
                }
            }
        }

        private void FillCombatAnimationsAll(NonDemoBattleFrame DefaultNonDemoBattleFrame, Squad Attacker, Squad AttackerSupport, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));

            NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
            if (ArrayDefenderHP.Length > 2 && ArrayDefenderHP[2] > 0)
            {
                Targets = NonDemoUnitStancePositions.Leader | NonDemoUnitStancePositions.WingmanA | NonDemoUnitStancePositions.WingmanB;
            }
            else if (ArrayDefenderHP.Length > 1 && ArrayDefenderHP[1] > 0)
            {
                Targets = NonDemoUnitStancePositions.Leader | NonDemoUnitStancePositions.WingmanA;
            }

            NonDemoBattleFrame MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, Targets, IsRightAttacking);

            //Both Missed.
            if (MissedFrame != null)
            {
                ListNonDemoBattleFrame.Add(MissedFrame);
            }
            else
            {
                MissedFrame = GetSwordCutOrShootDownFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, 0, Targets, IsRightAttacking);
                //If leader use Sword Cut or shoot down, stop everything
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {
                    //Both Critical.
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, Targets, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    else//Individual
                    {
                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (MissedFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(MissedFrame);
                            }
                            else
                            {
                                CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                                if (CriticalFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(CriticalFrame);
                                }
                            }
                        }
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, Targets, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, Targets, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    else
                    {
                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (ShieldFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(ShieldFrame);
                            }
                        }
                    }
                    if (BarrierFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(BarrierFrame);
                    }
                    else
                    {
                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                            if (BarrierFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(BarrierFrame);
                            }
                        }
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(DefaultNonDemoBattleFrame, AttackerResult, Targets, IsRightAttacking));

                    for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                }
            }

            ListNonDemoBattleFrame.Add(GetEndFrame(DefaultNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));
        }

        private void FillCombatAnimationsLeader(NonDemoBattleFrame DefaultNonDemoBattleFrame, bool EnemyLeaderHit ,Squad Attacker, Squad AttackerSupport, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            if (ArrayDefenderHP[0] > 0 && Attacker.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));

                NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(DefaultNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking);

                NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
                int i = 0;

                NonDemoBattleFrame MissedFrame = GetMissedFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {
                    if (!EnemyLeaderHit)
                    {
                        if (DefendingSupport.ActiveSquadSupport != null)
                        {
                            NonDemoBattleFrame SupportNonDemoBattleFrame = GetSwitchWithLeaderFrame(AttackNonDemoBattleFrame, IsRightAttacking);
                            ListNonDemoBattleFrame.Add(SupportNonDemoBattleFrame);
                            Targets = NonDemoUnitStancePositions.Support;
                            AttackNonDemoBattleFrame = SupportNonDemoBattleFrame;
                        }
                    }
                    EnemyLeaderHit = true;
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, Targets, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    if (BarrierFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(BarrierFrame);
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, IsRightAttacking));
                    ArrayDefenderHP[i] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[i] - AttackerResult.ArrayResult[i].AttackDamage);
                }

                if (EnemyLeaderHit && DefendingSupport.ActiveSquadSupport != null)
                {
                    NonDemoBattleFrame SupportNonDemoBattleFrame = GetSwitchBackWithLeaderFrame(AttackNonDemoBattleFrame, IsRightAttacking);
                    ListNonDemoBattleFrame.Add(SupportNonDemoBattleFrame);
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(AttackNonDemoBattleFrame, NonDemoUnitStancePositions.Leader, IsRightAttacking));

                if (ArrayDefenderHP[i] <= 0)
                {
                    return;
                }
            }
        }

        private NonDemoBattleFrame GetDefaultNonDemoBattleFrame(SquadBattleResult AttackerSquadResult, Squad AttackingSquad, Squad AttackingSupport, SquadBattleResult DefenderSquadResult, Squad DefendingSquad, Squad DefendingSupport, bool IsAttackerOnRight)
        {
            NonDemoBattleFrameSquad AttackingSquadFrame = new NonDemoBattleFrameSquad();
            NonDemoBattleFrameSquad DefendingSquadFrame = new NonDemoBattleFrameSquad();

            float AttackerPositionX = NonDemoRightUnitPosition.X;
            float AttackerPositionY = NonDemoRightUnitPosition.Y;

            float AttackerWingmanAPositionX = (int)NonDemoRightUnitPosition.X + 5;
            float AttackerWingmanAPositionY = (int)NonDemoRightUnitPosition.Y + 50;

            float AttackerWingmanBPositionX = (int)NonDemoRightUnitPosition.X + 5;
            float AttackerWingmanBPositionY = (int)NonDemoRightUnitPosition.Y + 100;

            float AttackerSupportPositionX = (int)NonDemoRightUnitPosition.X + 120;
            float AttackerSupportPositionY = (int)NonDemoRightUnitPosition.Y;

            float DefenderPositionX = NonDemoLeftUnitPosition.X;
            float DefenderPositionY = NonDemoLeftUnitPosition.Y;

            float DefenderWingmanAPositionX = (int)NonDemoLeftUnitPosition.X + 5;
            float DefenderWingmanAPositionY = (int)NonDemoLeftUnitPosition.Y + 50;

            float DefenderWingmanBPositionX = (int)NonDemoLeftUnitPosition.X + 5;
            float DefenderWingmanBPositionY = (int)NonDemoLeftUnitPosition.Y + 100;

            float DefenderSupportPositionX = (int)NonDemoRightUnitPosition.X - 120;
            float DefenderSupportPositionY = (int)NonDemoRightUnitPosition.Y;

            if (!IsAttackerOnRight)
            {
                AttackerPositionX = NonDemoLeftUnitPosition.X;
                AttackerPositionY = NonDemoLeftUnitPosition.Y;

                AttackerWingmanAPositionX = (int)NonDemoLeftUnitPosition.X + 5;
                AttackerWingmanAPositionY = (int)NonDemoLeftUnitPosition.Y + 50;

                AttackerWingmanBPositionX = (int)NonDemoLeftUnitPosition.X + 5;
                AttackerWingmanBPositionY = (int)NonDemoLeftUnitPosition.Y + 100;

                AttackerSupportPositionX = (int)NonDemoLeftUnitPosition.X - 120;
                AttackerSupportPositionY = (int)NonDemoLeftUnitPosition.Y;

                DefenderPositionX = NonDemoRightUnitPosition.X;
                DefenderPositionY = NonDemoRightUnitPosition.Y;

                DefenderWingmanAPositionX = (int)NonDemoRightUnitPosition.X + 5;
                DefenderWingmanAPositionY = (int)NonDemoRightUnitPosition.Y + 50;

                DefenderWingmanBPositionX = (int)NonDemoRightUnitPosition.X + 5;
                DefenderWingmanBPositionY = (int)NonDemoRightUnitPosition.Y + 100;

                DefenderSupportPositionX = (int)NonDemoRightUnitPosition.X + 120;
                DefenderSupportPositionY = (int)NonDemoRightUnitPosition.Y;
            }

            AttackingSquadFrame.LeaderStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(AttackingSquad.CurrentLeader, AttackerSquadResult.ArrayResult[0]),
                AttackerPositionX, AttackerPositionY, IsAttackerOnRight);

            DefendingSquadFrame.LeaderStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentLeader, DefenderSquadResult.ArrayResult[0]),
                DefenderPositionX, DefenderPositionY, IsAttackerOnRight);

            #region Wingmans

            if (AttackingSquad.CurrentWingmanB != null)
            {
                AttackingSquadFrame.WingmanAStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(AttackingSquad.CurrentWingmanA, AttackerSquadResult.ArrayResult[1]),
                    AttackerWingmanAPositionX, AttackerWingmanAPositionY, IsAttackerOnRight);

                AttackingSquadFrame.WingmanBStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(AttackingSquad.CurrentWingmanB, AttackerSquadResult.ArrayResult[2]),
                    AttackerWingmanBPositionX, AttackerWingmanBPositionY, IsAttackerOnRight);
            }
            else
            {
                AttackingSquadFrame.WingmanBStance = null;

                if (AttackingSquad.CurrentWingmanA != null)
                {
                    AttackingSquadFrame.WingmanAStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentWingmanA, AttackerSquadResult.ArrayResult[1]),
                        AttackerWingmanAPositionX, AttackerWingmanAPositionY, IsAttackerOnRight);
                }
                else
                {
                    AttackingSquadFrame.WingmanAStance = null;
                }
            }

            if (DefendingSquad.CurrentWingmanB != null)
            {
                DefendingSquadFrame.WingmanAStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentWingmanA, DefenderSquadResult.ArrayResult[1]),
                    DefenderWingmanAPositionX, DefenderWingmanAPositionY, IsAttackerOnRight);

                DefendingSquadFrame.WingmanBStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentWingmanB, DefenderSquadResult.ArrayResult[2]),
                    DefenderWingmanBPositionX, AttackerWingmanAPositionY, IsAttackerOnRight);
            }
            else
            {
                DefendingSquadFrame.WingmanBStance = null;

                if (DefendingSquad.CurrentWingmanA != null)
                {
                    DefendingSquadFrame.WingmanAStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentWingmanA, DefenderSquadResult.ArrayResult[1]),
                    DefenderWingmanBPositionX, DefenderWingmanBPositionY, IsAttackerOnRight);
                }
                else
                {
                    DefendingSquadFrame.WingmanAStance = null;
                }
            }

            #endregion

            #region Support

            AttackingSquadFrame.SupportStance = null;

            //Support Attack
            if (AttackingSupport != null)
            {
                AttackingSquadFrame.SupportStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(AttackingSupport.CurrentLeader, AttackerSquadResult.ResultSupportAttack),
                    AttackerSupportPositionX, AttackerSupportPositionY, IsAttackerOnRight);
            }

            //Support Defend
            DefendingSquadFrame.SupportStance = null;
            if (DefendingSupport != null)
            {
                DefendingSquadFrame.SupportStance = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSupport.CurrentLeader, DefenderSquadResult.ResultSupportAttack),
                    DefenderSupportPositionX, DefenderSupportPositionY, IsAttackerOnRight);
            }

            #endregion

            if (IsAttackerOnRight)
            {
                return new NonDemoBattleFrame(NonDemoIdleFrame.FrameLength, AttackingSquadFrame, DefendingSquadFrame);
            }
            else
            {
                return new NonDemoBattleFrame(NonDemoIdleFrame.FrameLength, DefendingSquadFrame, AttackingSquadFrame);
            }
        }

        private NonDemoBattleFrame GetStartFrame(NonDemoBattleFrame PreviousBattleFrame, NonDemoUnitStancePositions Attackers, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            if ((Attackers & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                AttackingFrame.LeaderStance = new NonDemoStartFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                AttackingFrame.WingmanAStance = new NonDemoStartFrame(AttackingFrame.WingmanAStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                AttackingFrame.WingmanBStance = new NonDemoStartFrame(AttackingFrame.WingmanBStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                AttackingFrame.SupportStance = new NonDemoStartSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
            }

            return new NonDemoBattleFrame(NonDemoStartFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetAttackFrame(NonDemoBattleFrame PreviousBattleFrame, NonDemoUnitStancePositions Attackers, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
            }

            if ((Attackers & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                AttackingFrame.LeaderStance = new NonDemoAttackFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                AttackingFrame.WingmanAStance = new NonDemoAttackFrame(AttackingFrame.WingmanAStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                AttackingFrame.WingmanBStance = new NonDemoAttackFrame(AttackingFrame.WingmanBStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                AttackingFrame.SupportStance = new NonDemoAttackSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
            }

            if (IsRightAttacking)
            {
                return new NonDemoBattleFrame(NonDemoAttackFrame.FrameLength, AttackingFrame, PreviousBattleFrame.LeftStance);
            }
            else
            {
                return new NonDemoBattleFrame(NonDemoAttackFrame.FrameLength, PreviousBattleFrame.RightStance, AttackingFrame);
            }
        }

        private NonDemoBattleFrame GetEndFrame(NonDemoBattleFrame PreviousBattleFrame, NonDemoUnitStancePositions Attackers, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            if ((Attackers & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                AttackingFrame.LeaderStance = new NonDemoEndFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                AttackingFrame.WingmanAStance = new NonDemoEndFrame(AttackingFrame.WingmanAStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                AttackingFrame.WingmanBStance = new NonDemoEndFrame(AttackingFrame.WingmanBStance, IsRightAttacking);
            }
            if ((Attackers & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                AttackingFrame.SupportStance = new NonDemoEndSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
            }

            return new NonDemoBattleFrame(NonDemoEndFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetGetHitFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                DefendingFrame.LeaderStance = new NonDemoGetHitFrame(DefendingFrame.LeaderStance, !IsRightAttacking, Map.fntNonDemoDamage,
                    Result.ArrayResult[0].AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                DefendingFrame.WingmanAStance = new NonDemoGetHitFrame(DefendingFrame.WingmanAStance, !IsRightAttacking, Map.fntNonDemoDamage,
                    Result.ArrayResult[1].AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                DefendingFrame.WingmanBStance = new NonDemoGetHitFrame(DefendingFrame.WingmanBStance, !IsRightAttacking, Map.fntNonDemoDamage,
                    Result.ArrayResult[2].AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
            }
            if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                DefendingFrame.SupportStance = new NonDemoGetHitFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntNonDemoDamage,
                    Result.ResultSupportAttack.AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
            }

            return new NonDemoBattleFrame(NonDemoGetHitFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetMissedFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllMissed = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (!Result.ArrayResult[i].AttackMissed)
                {
                    AllMissed = false;
                }
            }

            if (AllMissed)
            {
                return GetMissedFrame(PreviousBattleFrame, Targets, IsRightAttacking);
            }

            return null;
        }

        private NonDemoBattleFrame GetMissedFrame(NonDemoBattleFrame PreviousBattleFrame, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                DefendingFrame.LeaderStance = new NonDemoGetMissedFrame(DefendingFrame.LeaderStance, !IsRightAttacking, sprNonDemoMiss);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                DefendingFrame.WingmanAStance = new NonDemoGetMissedFrame(DefendingFrame.WingmanAStance, !IsRightAttacking, sprNonDemoMiss);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                DefendingFrame.WingmanBStance = new NonDemoGetMissedFrame(DefendingFrame.WingmanBStance, !IsRightAttacking, sprNonDemoMiss);
            }
            if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                DefendingFrame.SupportStance = new NonDemoGetMissedFrame(DefendingFrame.SupportStance, !IsRightAttacking, sprNonDemoMiss);
            }

            return new NonDemoBattleFrame(NonDemoGetMissedFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetSwordCutOrShootDownFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllMissed = true;
            bool AllSwordCut = true;
            bool AllShootDown = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (!Result.ArrayResult[i].AttackMissed)
                {
                    AllMissed = false;
                }
                if (!Result.ArrayResult[i].AttackSwordCut)
                {
                    AllSwordCut = false;
                }
                if (!Result.ArrayResult[i].AttackShootDown)
                {
                    AllShootDown = false;
                }
            }

            if (AllMissed)
            {
                NonDemoBattleFrameSquad AttackingFrame;
                NonDemoBattleFrameSquad DefendingFrame;

                if (IsRightAttacking)
                {
                    AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                    DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
                }
                else
                {
                    AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                    DefendingFrame = PreviousBattleFrame.RightStance.Copy();
                }

                if (AllSwordCut)
                {
                    if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                    {
                        DefendingFrame.LeaderStance = new NonDemoGetSwordCutFrame(DefendingFrame.LeaderStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                    {
                        DefendingFrame.WingmanAStance = new NonDemoGetSwordCutFrame(DefendingFrame.WingmanAStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                    {
                        DefendingFrame.WingmanBStance = new NonDemoGetSwordCutFrame(DefendingFrame.WingmanBStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                    {
                        DefendingFrame.SupportStance = new NonDemoGetSwordCutFrame(DefendingFrame.SupportStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                }
                else if (AllShootDown)
                {
                    if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                    {
                        DefendingFrame.LeaderStance = new NonDemoGetShootDownFrame(DefendingFrame.LeaderStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                    {
                        DefendingFrame.WingmanAStance = new NonDemoGetShootDownFrame(DefendingFrame.WingmanAStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                    {
                        DefendingFrame.WingmanBStance = new NonDemoGetShootDownFrame(DefendingFrame.WingmanBStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                    if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                    {
                        DefendingFrame.SupportStance = new NonDemoGetShootDownFrame(DefendingFrame.SupportStance, IsRightAttacking, Map.fntUnitAttack);
                    }
                }

                return new NonDemoBattleFrame(NonDemoGetShootDownFrame.FrameLength, DefendingFrame, AttackingFrame);
            }

            return null;
        }

        private NonDemoBattleFrame GetCriticalFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllCritical = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (!Result.ArrayResult[i].AttackWasCritical)
                {
                    AllCritical = false;
                }
            }

            if (AllCritical)
            {
                return GetCriticalFrame(PreviousBattleFrame, Targets, IsRightAttacking);
            }

            return null;
        }

        private NonDemoBattleFrame GetCriticalFrame(NonDemoBattleFrame PreviousBattleFrame, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
            {
                DefendingFrame.LeaderStance = new NonDemoGetCriticalHitFrame(DefendingFrame.LeaderStance, !IsRightAttacking, sprNonDemoCritical);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
            {
                DefendingFrame.WingmanAStance = new NonDemoGetCriticalHitFrame(DefendingFrame.WingmanAStance, !IsRightAttacking, sprNonDemoCritical);
            }
            if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
            {
                DefendingFrame.WingmanBStance = new NonDemoGetCriticalHitFrame(DefendingFrame.WingmanBStance, !IsRightAttacking, sprNonDemoCritical);
            }
            if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
            {
                DefendingFrame.SupportStance = new NonDemoGetCriticalHitFrame(DefendingFrame.SupportStance, !IsRightAttacking, sprNonDemoCritical);
            }

            return new NonDemoBattleFrame(NonDemoGetCriticalHitFrame.FrameLength, DefendingFrame, AttackingFrame);
        }

        private NonDemoBattleFrame GetShieldFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllShield = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (Result.ArrayResult[i].Barrier == null)
                {
                    AllShield = false;
                }
            }

            if (AllShield)
            {
                NonDemoBattleFrameSquad AttackingFrame;
                NonDemoBattleFrameSquad DefendingFrame;

                if (IsRightAttacking)
                {
                    AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                    DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
                }
                else
                {
                    AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                    DefendingFrame = PreviousBattleFrame.RightStance.Copy();
                }

                if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                {
                    DefendingFrame.LeaderStance = new NonDemoShieldFrame(DefendingFrame.LeaderStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                {
                    DefendingFrame.WingmanAStance = new NonDemoShieldFrame(DefendingFrame.WingmanAStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                {
                    DefendingFrame.WingmanBStance = new NonDemoShieldFrame(DefendingFrame.WingmanBStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                {
                    DefendingFrame.SupportStance = new NonDemoShieldFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntUnitAttack);
                }

                return new NonDemoBattleFrame(NonDemoShieldFrame.FrameLength, DefendingFrame, AttackingFrame);
            }

            return null;
        }

        private NonDemoBattleFrame GetBarrierFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, NonDemoUnitStancePositions Targets, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllBarrier = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (Result.ArrayResult[i].Barrier == null)
                {
                    AllBarrier = false;
                }
            }

            if (AllBarrier)
            {
                NonDemoBattleFrameSquad AttackingFrame;
                NonDemoBattleFrameSquad DefendingFrame;

                if (IsRightAttacking)
                {
                    AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                    DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
                }
                else
                {
                    AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                    DefendingFrame = PreviousBattleFrame.RightStance.Copy();
                }

                if ((Targets & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                {
                    DefendingFrame.LeaderStance = new NonDemoBarrierFrame(DefendingFrame.LeaderStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                {
                    DefendingFrame.WingmanAStance = new NonDemoBarrierFrame(DefendingFrame.WingmanAStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                {
                    DefendingFrame.WingmanBStance = new NonDemoBarrierFrame(DefendingFrame.WingmanBStance, !IsRightAttacking, Map.fntUnitAttack);
                }
                if ((Targets & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                {
                    DefendingFrame.SupportStance = new NonDemoBarrierFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntUnitAttack);
                }

                return new NonDemoBattleFrame(NonDemoBarrierFrame.FrameLength, DefendingFrame, AttackingFrame);
            }

            return null;
        }

        private NonDemoBattleFrame GetSwitchWithLeaderFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
            }

            AttackingFrame.LeaderStance = new NonDemoSwitchWithSupportFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            AttackingFrame.SupportStance = new NonDemoSwitchWithLeaderFrame(AttackingFrame.SupportStance, IsRightAttacking);

            if (IsRightAttacking)
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, AttackingFrame, PreviousBattleFrame.LeftStance);
            }
            else
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, PreviousBattleFrame.RightStance, AttackingFrame);
            }
        }

        private NonDemoBattleFrame GetSwitchWithLeaderHoldFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
            }

            AttackingFrame.LeaderStance = new NonDemoSwitchWithSupportHoldFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            AttackingFrame.SupportStance = new NonDemoSwitchWithLeaderHoldFrame(AttackingFrame.SupportStance, IsRightAttacking);

            if (IsRightAttacking)
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, AttackingFrame, PreviousBattleFrame.LeftStance);
            }
            else
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, PreviousBattleFrame.RightStance, AttackingFrame);
            }
        }

        private NonDemoBattleFrame GetSwitchBackWithLeaderFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad AttackingFrame;
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                AttackingFrame = PreviousBattleFrame.RightStance.Copy();
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                AttackingFrame = PreviousBattleFrame.LeftStance.Copy();
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            AttackingFrame.LeaderStance = new NonDemoSwitchBackWithSupportFrame(AttackingFrame.LeaderStance, IsRightAttacking);
            AttackingFrame.SupportStance = new NonDemoSwitchBackWithLeaderFrame(AttackingFrame.SupportStance, IsRightAttacking);

            return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, AttackingFrame, DefendingFrame);
        }

        public override void Update(GameTime gameTime)
        {
            --NonDemoAnimationTimer;
            if (Constants.ShowAnimation)//Animation was cancelled, show the non demo 2 times faster.
                --NonDemoAnimationTimer;

            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Update(gameTime);

            if (NonDemoAnimationTimer <= 0)
            {
                //Animation finished.
                if (++CurrentNonDemoBattleFrame >= ListNonDemoBattleFrame.Count)
                {
                    NonDemoBattleFinished();
                }
                else
                {
                    NonDemoAnimationTimer = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].FrameLength;

                    ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].OnEnd();
				}
            }
        }

        private void NonDemoBattleFinished()
        {
            RemoveWithoutUnloading(this);
            
            Map.FinalizeBattle(AttackingSquad, AttackingSupport, AttackerPlayerIndex, DefendingSquad, DefendingSupport, DefenderPlayerIndex, AttackerSquadResult, DefenderSquadResult);

            NonDemoAnimationTimer = -1;

            if (Map.ListMAPAttackTarget.Count > 0)
            {
                Tuple<int, int> NextTarget = Map.ListMAPAttackTarget.Pop();
                Map.TargetPlayerIndex = NextTarget.Item1;
                Map.TargetSquadIndex = NextTarget.Item2;

                SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                TargetSquadSupport.PrepareDefenceSupport(Map, NextTarget.Item1, Map.ListPlayer[NextTarget.Item1].ListSquad[NextTarget.Item2]);
                Map.ReadyNextMAPAttack(AttackerPlayerIndex, Map.ListPlayer[AttackerPlayerIndex].ListSquad.IndexOf(AttackingSquad), AttackingSupport, NextTarget.Item1, NextTarget.Item2, TargetSquadSupport);
            }
        }
        
        public override void Draw(CustomSpriteBatch g)
        {
            //Draw fighting Units over the original as they are no grayed.
            if (AttackingSquad.IsFlying)
            {
                g.Draw(Map.sprUnitHover, new Vector2((AttackingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (AttackingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
                g.Draw(AttackingSquad.CurrentLeader.SpriteMap, new Vector2((AttackingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (AttackingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y - 7), Color.White);
            }
            else
            {
                g.Draw(AttackingSquad.CurrentLeader.SpriteMap, new Vector2((AttackingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (AttackingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }

            if (DefendingSquad.IsFlying)
            {
                g.Draw(Map.sprUnitHover, new Vector2((DefendingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (DefendingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
                g.Draw(DefendingSquad.CurrentLeader.SpriteMap, new Vector2((DefendingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (DefendingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y - 7), Color.White);
            }
            else
            {
                g.Draw(DefendingSquad.CurrentLeader.SpriteMap, new Vector2((DefendingSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (DefendingSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }

            if (CurrentNonDemoBattleFrame < 0)
                return;

            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Draw(g, NonDemoAnimationTimer);
        }
    }
}
