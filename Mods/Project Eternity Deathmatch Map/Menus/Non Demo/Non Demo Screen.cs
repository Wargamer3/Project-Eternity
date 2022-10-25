using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;
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

        public NonDemoScreen(DeathmatchMap Map)
        {
            RequireFocus = true;
            RequireDrawFocus = true;

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

        public void InitNonDemo(Squad AttackingSquad, Attack CurrentAttack, SupportSquadHolder AttackingSupport, int AttackerPlayerIndex, SquadBattleResult AttackerSquadResult, FormationChoices AttackerFormation,
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

            bool CanDefenderCounter = CurrentAttack.Pri != WeaponPrimaryProperty.MAP && CurrentAttack.Pri != WeaponPrimaryProperty.PER;

            NonDemoAnimationTimer = 50;

            int[] ArrayAttackerHP = new int[AttackingSquad.UnitsAliveInSquad];
            for (int U = AttackingSquad.UnitsAliveInSquad - 1; U >= 0; U--)
            {
                ArrayAttackerHP[U] = AttackingSquad[U].HP;
            }

            int[] ArrayDefenderHP = new int[DefendingSquad.UnitsAliveInSquad];
            for (int U = DefendingSquad.UnitsAliveInSquad - 1; U >= 0; U--)
            {
                ArrayDefenderHP[U] = DefendingSquad[U].HP;
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

                ListNonDemoBattleFrame.Add(GetStartFrame(SupportNonDemoBattleFrame, new List<int>() { -1 }, IsRightAttacking));

                NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(SupportNonDemoBattleFrame, new List<int>() { -1 }, IsRightAttacking);

                if (AttackerSquadResult.ResultSupportAttack.AttackMissed)
                {
                    ListNonDemoBattleFrame.Add(GetMissedFrame(AttackNonDemoBattleFrame, new List<int>() { 0 }, IsRightAttacking));
                }
                else
                {
                    if (AttackerSquadResult.ResultSupportAttack.AttackWasCritical)
                    {
                        ListNonDemoBattleFrame.Add(GetCriticalFrame(AttackNonDemoBattleFrame, new List<int>() { 0 }, IsRightAttacking));
                    }

                    NonDemoBattleFrame GetHitNonDemoBattleFrame = GetGetHitAttackSupportFrame(AttackNonDemoBattleFrame, AttackerSquadResult.ResultSupportAttack, IsRightAttacking);

                    ListNonDemoBattleFrame.Add(GetHitNonDemoBattleFrame);
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(SupportNonDemoBattleFrame, new List<int>() { -1 }, IsRightAttacking));
            }
        }


        public void InitNonDemo(int DefenderPlayerIndex, int DefenderSquadIndex, int Damage)
        {
            this.DefenderPlayerIndex = DefenderPlayerIndex;
            this.DefendingSquad = Map.ListPlayer[DefenderPlayerIndex].ListSquad[DefenderSquadIndex];

            BattleResult AttackerSquadResultSingle = new BattleResult();
            AttackerSquadResultSingle.AttackAttackerFinalEN = DefendingSquad.CurrentLeader.EN;

            BattleResult[] ArrayAttackerResult = new BattleResult[1];
            ArrayAttackerResult[0].SetTarget(DefenderPlayerIndex, DefenderSquadIndex, 0, DefendingSquad.CurrentLeader);
            ArrayAttackerResult[0].AttackDamage = Damage;
            AttackerSquadResult = new SquadBattleResult(ArrayAttackerResult);

            NonDemoAnimationTimer = -1;
            CurrentNonDemoBattleFrame = 0;

            NonDemoAnimationTimer = 50;

            float AttackerPositionX = NonDemoRightUnitPosition.X;
            float AttackerPositionY = NonDemoRightUnitPosition.Y;

            NonDemoBattleFrameSquad AttackingSquadFrame = new NonDemoBattleFrameSquad();
            AttackingSquadFrame.ArrayStance = new NonDemoBattleUnitFrame[1];

            AttackingSquadFrame.ArrayStance[0] = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentLeader, AttackerSquadResultSingle),
                AttackerPositionX, AttackerPositionY, false);

            NonDemoBattleFrame DefaultNonDemoBattleFrame = new NonDemoBattleFrame(NonDemoIdleFrame.FrameLength, new NonDemoBattleFrameSquad(), AttackingSquadFrame);

            ListNonDemoBattleFrame.Add(DefaultNonDemoBattleFrame);

            AttackingSquadFrame = new NonDemoBattleFrameSquad();
            AttackingSquadFrame.ArrayStance[0] = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad.CurrentLeader, AttackerSquadResultSingle),
                AttackerPositionX, AttackerPositionY, false);

            AttackingSquadFrame.ArrayStance[0] = new NonDemoGetHitFrame(AttackingSquadFrame.ArrayStance[0], false, Map.fntNonDemoDamage,
                Damage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);

            DefaultNonDemoBattleFrame = new NonDemoBattleFrame(NonDemoGetHitFrame.FrameLength, new NonDemoBattleFrameSquad(), AttackingSquadFrame);

            ListNonDemoBattleFrame.Add(DefaultNonDemoBattleFrame);
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
            List<int> ListAttackersIndex = new List<int>();

            for (int U = Attacker.UnitsAliveInSquad - 1; U >= 1; --U)
            {
                if (ArrayAttackerHP[U] > 0 && ArrayDefenderHP[U] > 0 &&
                    Attacker[U].BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    ListAttackersIndex.Add(U);
                }
            }

            if (ListAttackersIndex.Count > 0)
            {
                ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, ListAttackersIndex, IsRightAttacking));

                NonDemoBattleFrame MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, ListAttackersIndex, IsRightAttacking);

                //Both Missed.
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {//Both Critical.
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, ListAttackersIndex, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    else//Individual
                    {
                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                            if (MissedFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(MissedFrame);
                            }
                            else
                            {
                                CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                                if (CriticalFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(CriticalFrame);
                                }
                            }
                        }
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, ListAttackersIndex, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, 1, -1, ListAttackersIndex, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    else
                    {
                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
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
                            BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                            if (BarrierFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(BarrierFrame);
                            }
                        }
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(DefaultNonDemoBattleFrame, AttackerResult, ListAttackersIndex, IsRightAttacking));

                    for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                        ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(DefaultNonDemoBattleFrame, ListAttackersIndex, IsRightAttacking));
            }
        }

        private void FillCombatAnimationsWingmansFocused(NonDemoBattleFrame DefaultNonDemoBattleFrame, ref bool LeaderHit, Squad Attacker, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            List<int> ListAttackersIndex = new List<int>() { 0 };

            for (int i = 1; i < AttackerResult.ArrayResult.Length && i < Attacker.UnitsAliveInSquad && i < Defender.UnitsAliveInSquad; i++)
            {
                if (ArrayAttackerHP[i] > 0 && Attacker[i].BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, new List<int>() { i }, IsRightAttacking));

                    NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(DefaultNonDemoBattleFrame, new List<int>() { i }, IsRightAttacking);

                    NonDemoBattleFrame MissedFrame = GetMissedFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
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
                                ListAttackersIndex = new List<int>() { -1 };
                                AttackNonDemoBattleFrame = SupportNonDemoBattleFrame;
                            }
                        }

                        LeaderHit = true;
                        NonDemoBattleFrame CriticalFrame = GetCriticalFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                        if (CriticalFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(CriticalFrame);
                        }
                        NonDemoBattleFrame ShieldFrame = GetShieldFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                        NonDemoBattleFrame BarrierFrame = GetBarrierFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);

                        if (ShieldFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(ShieldFrame);
                        }
                        if (BarrierFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(BarrierFrame);
                        }

                        ListNonDemoBattleFrame.Add(GetGetHitFrame(AttackNonDemoBattleFrame, AttackerResult, ListAttackersIndex, IsRightAttacking));
                        ArrayDefenderHP[0] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[0] - AttackerResult.ArrayResult[i].AttackDamage);
                    }

                    ListNonDemoBattleFrame.Add(GetEndFrame(AttackNonDemoBattleFrame, new List<int>() { i }, IsRightAttacking));

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
            List<int> ListAttackersIndex = new List<int>() { 0 };

            ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, ListAttackersIndex, IsRightAttacking));

            for (int U = ArrayDefenderHP.Length - 1; U >= 1; --U)
            {
                if (ArrayDefenderHP.Length >= U && ArrayDefenderHP[U] > 0)
                {
                    ListAttackersIndex.Add(U);
                }
            }

            NonDemoBattleFrame MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, ListAttackersIndex, IsRightAttacking);

            //Both Missed.
            if (MissedFrame != null)
            {
                ListNonDemoBattleFrame.Add(MissedFrame);
            }
            else
            {
                MissedFrame = GetSwordCutOrShootDownFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, 0, ListAttackersIndex, IsRightAttacking);
                //If leader use Sword Cut or shoot down, stop everything
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {
                    //Both Critical.
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, ListAttackersIndex, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    else//Individual
                    {
                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            MissedFrame = GetMissedFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                            if (MissedFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(MissedFrame);
                            }
                            else
                            {
                                CriticalFrame = GetCriticalFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                                if (CriticalFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(CriticalFrame);
                                }
                            }
                        }
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, ListAttackersIndex, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, 0, -1, ListAttackersIndex, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    else
                    {
                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        {
                            ShieldFrame = GetShieldFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
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
                            BarrierFrame = GetBarrierFrame(DefaultNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                            if (BarrierFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(BarrierFrame);
                            }
                        }
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(DefaultNonDemoBattleFrame, AttackerResult, ListAttackersIndex, IsRightAttacking));

                    for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                        ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                }
            }

            ListNonDemoBattleFrame.Add(GetEndFrame(DefaultNonDemoBattleFrame, new List<int>() { 0 }, IsRightAttacking));
        }

        private void FillCombatAnimationsLeader(NonDemoBattleFrame DefaultNonDemoBattleFrame, bool EnemyLeaderAlreadyAttacked, Squad Attacker, Squad AttackerSupport, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            if (ArrayDefenderHP[0] > 0 && Attacker.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                List<int> ListAttackersIndex = new List<int>() { 0 };

                ListNonDemoBattleFrame.Add(GetStartFrame(DefaultNonDemoBattleFrame, ListAttackersIndex, IsRightAttacking));

                NonDemoBattleFrame AttackNonDemoBattleFrame = GetAttackFrame(DefaultNonDemoBattleFrame, ListAttackersIndex, IsRightAttacking);

                int i = 0;

                NonDemoBattleFrame MissedFrame = GetMissedFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame);
                }
                else
                {
                    if (!EnemyLeaderAlreadyAttacked && DefenderSupport != null)
                    {
                        ListNonDemoBattleFrame.Add(GetSwitchSupportDefenceWithLeaderFrame(AttackNonDemoBattleFrame, IsRightAttacking));
                        ListAttackersIndex = new List<int>() { -1 };
                        AttackNonDemoBattleFrame = GetSwitchSupportDefenceWithLeaderHoldFrame(AttackNonDemoBattleFrame, IsRightAttacking);
                    }

                    EnemyLeaderAlreadyAttacked = true;
                    NonDemoBattleFrame CriticalFrame = GetCriticalFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame);
                    }
                    NonDemoBattleFrame ShieldFrame = GetShieldFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);
                    NonDemoBattleFrame BarrierFrame = GetBarrierFrame(AttackNonDemoBattleFrame, AttackerResult, i, i, ListAttackersIndex, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame);
                    }
                    if (BarrierFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(BarrierFrame);
                    }

                    ListNonDemoBattleFrame.Add(GetGetHitFrame(AttackNonDemoBattleFrame, AttackerResult, ListAttackersIndex, IsRightAttacking));
                    ArrayDefenderHP[i] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[i] - AttackerResult.ArrayResult[i].AttackDamage);
                }

                if (EnemyLeaderAlreadyAttacked && DefenderSupport != null)
                {
                    NonDemoBattleFrame SupportNonDemoBattleFrame = GetSwitchSupportDefenceBackWithLeaderFrame(AttackNonDemoBattleFrame, IsRightAttacking);
                    ListNonDemoBattleFrame.Add(SupportNonDemoBattleFrame);
                    AttackNonDemoBattleFrame = DefaultNonDemoBattleFrame;
                }

                ListNonDemoBattleFrame.Add(GetEndFrame(AttackNonDemoBattleFrame, new List<int>() { 0 }, IsRightAttacking));

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

            float AttackerSupportPositionX = (int)NonDemoRightUnitPosition.X + 120;
            float AttackerSupportPositionY = (int)NonDemoRightUnitPosition.Y;

            float DefenderPositionX = NonDemoLeftUnitPosition.X;
            float DefenderPositionY = NonDemoLeftUnitPosition.Y;

            float DefenderSupportPositionX = (int)NonDemoRightUnitPosition.X - 120;
            float DefenderSupportPositionY = (int)NonDemoRightUnitPosition.Y;

            if (!IsAttackerOnRight)
            {
                AttackerPositionX = NonDemoLeftUnitPosition.X;
                AttackerPositionY = NonDemoLeftUnitPosition.Y;

                AttackerSupportPositionX = (int)NonDemoLeftUnitPosition.X - 120;
                AttackerSupportPositionY = (int)NonDemoLeftUnitPosition.Y;

                DefenderPositionX = NonDemoRightUnitPosition.X;
                DefenderPositionY = NonDemoRightUnitPosition.Y;

                DefenderSupportPositionX = (int)NonDemoRightUnitPosition.X + 120;
                DefenderSupportPositionY = (int)NonDemoRightUnitPosition.Y;
            }

            AttackingSquadFrame.ArrayStance = new NonDemoBattleUnitFrame[AttackingSquad.UnitsAliveInSquad];
            DefendingSquadFrame.ArrayStance = new NonDemoBattleUnitFrame[DefendingSquad.UnitsAliveInSquad];

            for (int U = AttackingSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                AttackingSquadFrame.ArrayStance[U] = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(AttackingSquad[U], AttackerSquadResult.ArrayResult[U]),
                    AttackerPositionX + U > 0 ? 5 : 0, AttackerPositionY + U * 50, IsAttackerOnRight);
            }
            for (int U = DefendingSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                DefendingSquadFrame.ArrayStance[U] = new NonDemoIdleFrame(Map, new NonDemoSharedUnitStats(DefendingSquad[U], DefenderSquadResult.ArrayResult[U]),
                    DefenderPositionX + U > 0 ? 5 : 0, DefenderPositionY + U * 50, IsAttackerOnRight);
            }

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

        private NonDemoBattleFrame GetStartFrame(NonDemoBattleFrame PreviousBattleFrame, List<int> ListAttackersIndex, bool IsRightAttacking)
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

            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    AttackingFrame.ArrayStance[UnitIndex] = new NonDemoStartFrame(AttackingFrame.ArrayStance[UnitIndex], IsRightAttacking);
                }
                else
                {
                    AttackingFrame.SupportStance = new NonDemoStartSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
                }
            }

            return new NonDemoBattleFrame(NonDemoStartFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetAttackFrame(NonDemoBattleFrame PreviousBattleFrame, List<int> ListAttackersIndex, bool IsRightAttacking)
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

            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    AttackingFrame.ArrayStance[UnitIndex] = new NonDemoAttackFrame(AttackingFrame.ArrayStance[UnitIndex], IsRightAttacking);
                }
                else
                {
                    AttackingFrame.SupportStance = new NonDemoAttackSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
                }
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

        private NonDemoBattleFrame GetEndFrame(NonDemoBattleFrame PreviousBattleFrame, List<int> ListAttackersIndex, bool IsRightAttacking)
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

            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    AttackingFrame.ArrayStance[UnitIndex] = new NonDemoEndFrame(AttackingFrame.ArrayStance[UnitIndex], IsRightAttacking);
                }
                else
                {
                    AttackingFrame.SupportStance = new NonDemoEndSupportFrame(AttackingFrame.SupportStance, IsRightAttacking);
                }
            }

            return new NonDemoBattleFrame(NonDemoEndFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetGetHitFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, List<int> ListAttackersIndex, bool IsRightAttacking)
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

            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    DefendingFrame.ArrayStance[UnitIndex] = new NonDemoGetHitFrame(DefendingFrame.ArrayStance[UnitIndex], !IsRightAttacking, Map.fntNonDemoDamage,
                        Result.ArrayResult[UnitIndex].AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
                }
                else
                {
                    DefendingFrame.SupportStance = new NonDemoGetHitSupportDefenceFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntNonDemoDamage,
                        Result.ArrayResult[0].AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);
                }
            }

            return new NonDemoBattleFrame(NonDemoGetHitFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetGetHitAttackSupportFrame(NonDemoBattleFrame PreviousBattleFrame, BattleResult Result, bool IsRightAttacking)
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

            DefendingFrame.ArrayStance[0] = new NonDemoGetHitFrame(DefendingFrame.ArrayStance[0], !IsRightAttacking, Map.fntNonDemoDamage,
                Result.AttackDamage, sprNonDemoExplosion.Copy(), sndNonDemoAttack);

            return new NonDemoBattleFrame(NonDemoGetHitFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetMissedFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, List<int> ListAttackersIndex, bool IsRightAttacking)
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
                return GetMissedFrame(PreviousBattleFrame, ListAttackersIndex, IsRightAttacking);
            }

            return null;
        }

        private NonDemoBattleFrame GetMissedFrame(NonDemoBattleFrame PreviousBattleFrame, List<int> ListAttackersIndex, bool IsRightAttacking)
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


            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    DefendingFrame.ArrayStance[UnitIndex] = new NonDemoGetMissedFrame(DefendingFrame.ArrayStance[UnitIndex], !IsRightAttacking, sprNonDemoMiss);
                }
                else
                {
                    DefendingFrame.SupportStance = new NonDemoGetMissedFrame(DefendingFrame.SupportStance, !IsRightAttacking, sprNonDemoMiss);
                }
            }


            return new NonDemoBattleFrame(NonDemoGetMissedFrame.FrameLength, AttackingFrame, DefendingFrame);
        }

        private NonDemoBattleFrame GetSwordCutOrShootDownFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, List<int> ListAttackersIndex, bool IsRightAttacking)
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
                    foreach (int UnitIndex in ListAttackersIndex)
                    {
                        if (UnitIndex >= 0)
                        {
                            DefendingFrame.ArrayStance[UnitIndex] = new NonDemoGetSwordCutFrame(DefendingFrame.ArrayStance[UnitIndex], IsRightAttacking, Map.fntUnitAttack);
                        }
                        else
                        {
                            DefendingFrame.SupportStance = new NonDemoGetSwordCutFrame(DefendingFrame.SupportStance, IsRightAttacking, Map.fntUnitAttack);
                        }
                    }
                }
                else if (AllShootDown)
                {
                    foreach (int UnitIndex in ListAttackersIndex)
                    {
                        if (UnitIndex >= 0)
                        {
                            DefendingFrame.ArrayStance[UnitIndex] = new NonDemoGetShootDownFrame(DefendingFrame.ArrayStance[UnitIndex], IsRightAttacking, Map.fntUnitAttack);
                        }
                        else
                        {
                            DefendingFrame.SupportStance = new NonDemoGetShootDownFrame(DefendingFrame.SupportStance, IsRightAttacking, Map.fntUnitAttack);
                        }
                    }
                }

                return new NonDemoBattleFrame(NonDemoGetShootDownFrame.FrameLength, DefendingFrame, AttackingFrame);
            }

            return null;
        }

        private NonDemoBattleFrame GetCriticalFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, List<int> ListAttackersIndex, bool IsRightAttacking)
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
                return GetCriticalFrame(PreviousBattleFrame, ListAttackersIndex, IsRightAttacking);
            }

            return null;
        }

        private NonDemoBattleFrame GetCriticalFrame(NonDemoBattleFrame PreviousBattleFrame, List<int> ListAttackersIndex, bool IsRightAttacking)
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

            foreach (int UnitIndex in ListAttackersIndex)
            {
                if (UnitIndex >= 0)
                {
                    DefendingFrame.ArrayStance[UnitIndex] = new NonDemoGetCriticalHitFrame(DefendingFrame.ArrayStance[UnitIndex], !IsRightAttacking, sprNonDemoCritical);
                }
                else
                {
                    DefendingFrame.SupportStance = new NonDemoGetCriticalHitFrame(DefendingFrame.SupportStance, !IsRightAttacking, sprNonDemoCritical);
                }
            }

            return new NonDemoBattleFrame(NonDemoGetCriticalHitFrame.FrameLength, DefendingFrame, AttackingFrame);
        }

        private NonDemoBattleFrame GetShieldFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, List<int> ListAttackersIndex, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllShield = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (string.IsNullOrEmpty(Result.ArrayResult[i].Barrier))
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

                foreach (int UnitIndex in ListAttackersIndex)
                {
                    if (UnitIndex >= 0)
                    {
                        DefendingFrame.ArrayStance[UnitIndex] = new NonDemoShieldFrame(DefendingFrame.ArrayStance[UnitIndex], !IsRightAttacking, Map.fntUnitAttack);
                    }
                    else
                    {
                        DefendingFrame.SupportStance = new NonDemoShieldFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntUnitAttack);
                    }
                }

                return new NonDemoBattleFrame(NonDemoShieldFrame.FrameLength, DefendingFrame, AttackingFrame);
            }

            return null;
        }

        private NonDemoBattleFrame GetBarrierFrame(NonDemoBattleFrame PreviousBattleFrame, SquadBattleResult Result, int Start, int End, List<int> ListAttackersIndex, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllBarrier = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (string.IsNullOrEmpty(Result.ArrayResult[i].Barrier))
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

                foreach (int UnitIndex in ListAttackersIndex)
                {
                    if (UnitIndex >= 0)
                    {
                        DefendingFrame.ArrayStance[UnitIndex] = new NonDemoBarrierFrame(DefendingFrame.ArrayStance[UnitIndex], !IsRightAttacking, Map.fntUnitAttack);
                    }
                    else
                    {
                        DefendingFrame.SupportStance = new NonDemoBarrierFrame(DefendingFrame.SupportStance, !IsRightAttacking, Map.fntUnitAttack);
                    }
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

            AttackingFrame.ArrayStance[0] = new NonDemoSwitchWithSupportFrame(AttackingFrame.ArrayStance[0], IsRightAttacking);
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

            AttackingFrame.ArrayStance[0] = new NonDemoSwitchWithSupportHoldFrame(AttackingFrame.ArrayStance[0], IsRightAttacking);
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

        private NonDemoBattleFrame GetSwitchSupportDefenceWithLeaderFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            DefendingFrame.ArrayStance[0] = new NonDemoSwitchWithSupportFrame(DefendingFrame.ArrayStance[0], !IsRightAttacking);
            DefendingFrame.SupportStance = new NonDemoSwitchWithLeaderFrame(DefendingFrame.SupportStance, !IsRightAttacking);

            if (IsRightAttacking)
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, PreviousBattleFrame.RightStance, DefendingFrame);
            }
            else
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, DefendingFrame, PreviousBattleFrame.LeftStance);
            }
        }

        private NonDemoBattleFrame GetSwitchSupportDefenceWithLeaderHoldFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
        {
            NonDemoBattleFrameSquad DefendingFrame;

            if (IsRightAttacking)
            {
                DefendingFrame = PreviousBattleFrame.LeftStance.Copy();
            }
            else
            {
                DefendingFrame = PreviousBattleFrame.RightStance.Copy();
            }

            DefendingFrame.ArrayStance[0] = new NonDemoSwitchWithSupportHoldFrame(DefendingFrame.ArrayStance[0], !IsRightAttacking);
            DefendingFrame.SupportStance = new NonDemoSwitchWithLeaderHoldFrame(DefendingFrame.SupportStance, !IsRightAttacking);

            if (IsRightAttacking)
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, PreviousBattleFrame.RightStance, DefendingFrame);
            }
            else
            {
                return new NonDemoBattleFrame((int)NonDemoBattleFrame.SwitchLength, DefendingFrame, PreviousBattleFrame.LeftStance);
            }
        }

        private NonDemoBattleFrame GetSwitchSupportDefenceBackWithLeaderFrame(NonDemoBattleFrame PreviousBattleFrame, bool IsRightAttacking)
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

            DefendingFrame.ArrayStance[0] = new NonDemoSwitchBackWithSupportFrame(DefendingFrame.ArrayStance[0], !IsRightAttacking);
            DefendingFrame.SupportStance = new NonDemoSwitchBackWithLeaderFrame(DefendingFrame.SupportStance, !IsRightAttacking);

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

            if (AttackingSquad == null || AttackingSquad.IsDead)
            {
                Map.FinalizeBattle(null, null, null, Map.ActivePlayerIndex, DefendingSquad, DefendingSupport, DefenderPlayerIndex, AttackerSquadResult, DefenderSquadResult);
            }
            else
            {
                Map.FinalizeBattle(AttackingSquad, AttackingSquad.CurrentLeader.CurrentAttack, AttackingSupport, AttackerPlayerIndex, DefendingSquad, DefendingSupport, DefenderPlayerIndex, AttackerSquadResult, DefenderSquadResult);
            }

            NonDemoAnimationTimer = 9999;

            if (Map.ListMAPAttackTarget.Count > 0)
            {
                Tuple<int, int> NextTarget = Map.ListMAPAttackTarget.Pop();
                Map.TargetPlayerIndex = NextTarget.Item1;
                Map.TargetSquadIndex = NextTarget.Item2;

                SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                TargetSquadSupport.PrepareDefenceSupport(Map, NextTarget.Item1, Map.ListPlayer[NextTarget.Item1].ListSquad[NextTarget.Item2]);
                Map.ReadyNextMAPAttack(AttackerPlayerIndex, Map.ListPlayer[AttackerPlayerIndex].ListSquad.IndexOf(AttackingSquad),
                    AttackingSquad.CurrentLeader.CurrentAttack, AttackingSupport, new List<Vector3>(),
                    NextTarget.Item1, NextTarget.Item2, TargetSquadSupport);
            }
        }
        
        public override void Draw(CustomSpriteBatch g)
        {
            if (CurrentNonDemoBattleFrame < 0)
                return;

            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Draw(g, NonDemoAnimationTimer);
        }
    }
}
