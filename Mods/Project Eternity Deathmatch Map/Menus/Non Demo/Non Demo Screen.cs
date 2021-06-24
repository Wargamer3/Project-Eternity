using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class NonDemoScreen : GameScreen
    {
        #region Ressources

        private Texture2D sprNonDemoMiss;
        private Texture2D sprNonDemoCritical;
        private FMODSound sndNonDemoAttack;
        private AnimatedSprite sprNonDemoExplosionLeader;
        private AnimatedSprite sprNonDemoExplosionWingmanA;
        private AnimatedSprite sprNonDemoExplosionWingmanB;

        #endregion

        #region Variables

        private int NonDemoAnimationTimer;
        public List<NonDemoBattleFrame> ListNonDemoBattleFrame;
        private int CurrentNonDemoBattleFrame;

        private Squad NonDemoRightSquad;
        private Squad NonDemoRightSupport;
        private int[] NonDemoRightSquadStartingHP;
        private int NonDemoRightSupportStartingHP;
        private int[] NonDemoRightSquadStartingEN;
        private int NonDemoRightSupportStartingEN;
        private Squad NonDemoLeftSquad;
        private Squad NonDemoLeftSupport;
        private int[] NonDemoLeftSquadStartingHP;
        private int NonDemoLeftSupportStartingHP;
        private int[] NonDemoLeftSquadStartingEN;
        private int NonDemoLeftSupportStartingEN;
        private readonly Vector2 NonDemoRightUnitPosition = new Vector2(Constants.Width / 2 + 47, 160);
        private readonly Vector2 NonDemoLeftUnitPosition = new Vector2(Constants.Width / 2 - 47 - 113, 160);

        private SquadBattleResult NonDemoRightSquadResult;
        private SquadBattleResult NonDemoLeftSquadResult;

        DeathmatchMap Map;
        Squad Attacker;
        SupportSquadHolder ActiveSquadSupport;
        Squad Defender;
        SupportSquadHolder DefenderSquadSupport;
        SquadBattleResult AttackerSquadResult;
        SquadBattleResult DefenderSquadResult;
        private int AttackerPlayerIndex;
        private int DefenderPlayerIndex;

        #endregion

        public enum NonDemoUnitStances
        {
            Invisible, Idle, Start, Attack, End, GetHit, GetMissed, GetSwordCut, GetShootDown, Barrier, Shield, GetCriticalHit, SwitchWithSupport, SwitchWithLeader, SwitchBackWithSupport, SwitchBackWithLeader
        };

        [Flags]
        public enum NonDemoUnitStancePositions : byte
        {
            Leader = 0x1, WingmanA = 0x2, WingmanB = 0x4, Support = 0x8
        };

        public struct NonDemoBattleFrame
        {
            public struct NonDemoBattleFrameSquad
            {
                public NonDemoUnitStances LeaderStance;
                public NonDemoUnitStances WingmanAStance;
                public NonDemoUnitStances WingmanBStance;
                public NonDemoUnitStances SupportAttackStance;
                public bool DefenseSupport;
            }

            public static readonly float SwitchLength = 25f;

            public NonDemoBattleFrameSquad RightStance;
            public NonDemoBattleFrameSquad LeftStance;
            
            public int FrameLength;
            public SquadBattleResult Result;

            public NonDemoBattleFrame(NonDemoBattleFrame Default, SquadBattleResult Result,
                NonDemoUnitStancePositions NonDemoUnitStancePosition, NonDemoUnitStances NonDemoUnitStance, bool IsRight)
            {
                this.Result = Result;
                RightStance = new NonDemoBattleFrameSquad();
                LeftStance = new NonDemoBattleFrameSquad();
                RightStance.LeaderStance = Default.RightStance.LeaderStance;
                RightStance.WingmanAStance = Default.RightStance.WingmanAStance;
                RightStance.WingmanBStance = Default.RightStance.WingmanBStance;
                RightStance.SupportAttackStance = Default.RightStance.SupportAttackStance;

                LeftStance.LeaderStance = Default.LeftStance.LeaderStance;
                LeftStance.WingmanAStance = Default.LeftStance.WingmanAStance;
                LeftStance.WingmanBStance = Default.LeftStance.WingmanBStance;
                LeftStance.SupportAttackStance = Default.LeftStance.SupportAttackStance;

                if (IsRight)
                {
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                        RightStance.LeaderStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                        RightStance.WingmanAStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                        RightStance.WingmanBStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                        RightStance.SupportAttackStance = NonDemoUnitStance;
                }
                else
                {
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.Leader) == NonDemoUnitStancePositions.Leader)
                        LeftStance.LeaderStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.WingmanA) == NonDemoUnitStancePositions.WingmanA)
                        LeftStance.WingmanAStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.WingmanB) == NonDemoUnitStancePositions.WingmanB)
                        LeftStance.WingmanBStance = NonDemoUnitStance;
                    if ((NonDemoUnitStancePosition & NonDemoUnitStancePositions.Support) == NonDemoUnitStancePositions.Support)
                        LeftStance.SupportAttackStance = NonDemoUnitStance;
                }

                switch (NonDemoUnitStance)
                {
                    case NonDemoUnitStances.Start:
                    case NonDemoUnitStances.End:
                        FrameLength = 8;
                        break;

                    case NonDemoUnitStances.GetMissed:
                    case NonDemoUnitStances.GetHit:
                        FrameLength = 46;
                        break;

                    case NonDemoUnitStances.SwitchWithSupport:
                    case NonDemoUnitStances.SwitchWithLeader:
                    case NonDemoUnitStances.SwitchBackWithSupport:
                    case NonDemoUnitStances.SwitchBackWithLeader:
                        FrameLength = (int)SwitchLength;
                        break;

                    default:
                        FrameLength = 46;
                        break;
                }

                RightStance.DefenseSupport = Default.RightStance.DefenseSupport;
                LeftStance.DefenseSupport = Default.LeftStance.DefenseSupport;
            }
        }

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
            sprNonDemoExplosionLeader = new AnimatedSprite(Content, "Animations/Bitmap Animations/Explosion_strip3", new Vector2(), 10f);
            sprNonDemoExplosionWingmanA = new AnimatedSprite(Content, "Animations/Bitmap Animations/Explosion_strip3", new Vector2(), 10f);
            sprNonDemoExplosionWingmanB = new AnimatedSprite(Content, "Animations/Bitmap Animations/Explosion_strip3", new Vector2(), 10f);

            sprNonDemoExplosionLeader.EndAnimation();
            sprNonDemoExplosionWingmanA.EndAnimation();
            sprNonDemoExplosionWingmanB.EndAnimation();
        }

        public void InitNonDemo(Squad Attacker, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex, SquadBattleResult AttackerSquadResult, FormationChoices AttackerFormation,
            Squad Defender, SupportSquadHolder DefenderSquadSupport, int DefenderPlayerIndex, SquadBattleResult DefenderSquadResult, FormationChoices DefenderFormation, bool IsRightAttacking)
        {
            this.Attacker = Attacker;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.AttackerPlayerIndex = AttackerPlayerIndex;
            this.Defender = Defender;
            this.DefenderSquadSupport = DefenderSquadSupport;
            this.DefenderPlayerIndex = DefenderPlayerIndex;
            this.AttackerSquadResult = AttackerSquadResult;
            this.DefenderSquadResult = DefenderSquadResult;

            NonDemoAnimationTimer = -1;
            CurrentNonDemoBattleFrame = 0;

            Map.GetLeftRightSquads(IsRightAttacking, Attacker, ActiveSquadSupport, Defender, DefenderSquadSupport, out NonDemoRightSquad, out NonDemoRightSupport, out NonDemoLeftSquad, out NonDemoLeftSupport);

            NonDemoRightSquadResult = AttackerSquadResult;
            NonDemoLeftSquadResult = DefenderSquadResult;

            if (!IsRightAttacking)
            {
                NonDemoRightSquadResult = DefenderSquadResult;
                NonDemoLeftSquadResult = AttackerSquadResult;
            }

            bool CanDefenderCounter = Attacker.CurrentLeader.CurrentAttack.Pri != Core.Attacks.WeaponPrimaryProperty.MAP;

            var DefaultNonDemoBattleFrame = GetDefaultNonDemoBattleFrame(Attacker, ActiveSquadSupport.ActiveSquadSupport, Defender, DefenderSquadSupport.ActiveSquadSupport, IsRightAttacking);

            NonDemoRightSquadStartingHP = new int[NonDemoRightSquad.UnitsAliveInSquad];
            NonDemoRightSquadStartingEN = new int[NonDemoRightSquad.UnitsAliveInSquad];
            for (int U = NonDemoRightSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                NonDemoRightSquadStartingHP[U] = NonDemoRightSquad[U].HP;
                NonDemoRightSquadStartingEN[U] = NonDemoRightSquad[U].EN;
            }
            if (NonDemoRightSupport != null)
            {
                NonDemoRightSupportStartingHP = NonDemoRightSupport.CurrentLeader.HP;
                NonDemoRightSupportStartingEN = NonDemoRightSupport.CurrentLeader.EN;
            }

            NonDemoLeftSquadStartingHP = new int[NonDemoLeftSquad.UnitsAliveInSquad];
            NonDemoLeftSquadStartingEN = new int[NonDemoLeftSquad.UnitsAliveInSquad];
            for (int U = NonDemoLeftSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                NonDemoLeftSquadStartingHP[U] = NonDemoLeftSquad[U].HP;
                NonDemoLeftSquadStartingEN[U] = NonDemoLeftSquad[U].EN;
            }
            if (NonDemoLeftSupport != null)
            {
                NonDemoLeftSupportStartingHP = NonDemoLeftSupport.CurrentLeader.HP;
                NonDemoLeftSupportStartingEN = NonDemoLeftSupport.CurrentLeader.EN;
            }

            NonDemoAnimationTimer = 50;

            int[] ArrayAttackerHP = new int[Attacker.UnitsAliveInSquad];
            for (int i = 0; i < Attacker.UnitsAliveInSquad; i++)
            {
                ArrayAttackerHP[i] = Attacker[i].HP;
            }

            int[] ArrayDefenderHP = new int[Defender.UnitsAliveInSquad];
            for (int i = 0; i < Defender.UnitsAliveInSquad; i++)
            {
                ArrayDefenderHP[i] = Defender[i].HP;
            }

            ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.Idle, IsRightAttacking));

            FillCombatAnimations(Attacker, ActiveSquadSupport.ActiveSquadSupport, AttackerSquadResult, AttackerFormation,
                Defender, DefenderSquadSupport.ActiveSquadSupport, IsRightAttacking, ref ArrayDefenderHP, ArrayAttackerHP);

            if (ArrayDefenderHP[0] > 0 && CanDefenderCounter && Defender.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                FillCombatAnimations(Defender, null, DefenderSquadResult, DefenderFormation,
                    Attacker, null, !IsRightAttacking, ref ArrayAttackerHP, ArrayDefenderHP);
            }

            if (AttackerSquadResult.ResultSupportAttack.Target != null)
            {
                NonDemoBattleFrame SupportNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.SwitchWithSupport, IsRightAttacking);
                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(SupportNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.SwitchWithLeader, IsRightAttacking));

                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.Start, IsRightAttacking));

                NonDemoBattleFrame AttackNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.Attack, IsRightAttacking);

                if (AttackerSquadResult.ResultSupportAttack.AttackMissed)
                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.GetMissed, !IsRightAttacking));
                else
                {
                    if (AttackerSquadResult.ResultSupportAttack.AttackWasCritical)
                        ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.GetCriticalHit, !IsRightAttacking));

                    NonDemoBattleFrame GetHitNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.GetHit, !IsRightAttacking);

                    ListNonDemoBattleFrame.Add(GetHitNonDemoBattleFrame);
                }

                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerSquadResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.End, IsRightAttacking));
            }
        }

        private void FillCombatAnimations(Squad Attacker, Squad AttackerSupport, SquadBattleResult AttackerResult, FormationChoices AttackerFormation,
            Squad Defender, Squad DefenderSupport, bool IsRightAttacking, ref int[] ArrayDefenderHP, int[] ArrayAttackerHP)
        {
            var DefaultNonDemoBattleFrame = GetDefaultNonDemoBattleFrame(Attacker, AttackerSupport, Defender, DefenderSupport, IsRightAttacking);

            bool LeaderHit = false;

            #region Spread

            if (AttackerFormation == FormationChoices.Spread)
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
                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.Start, IsRightAttacking));
                    NonDemoBattleFrame AttackNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.Attack, IsRightAttacking);

                    NonDemoBattleFrame? MissedFrame = GetMissedFrame(AttackerResult, 1, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                    //Both Missed.
                    if (MissedFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(MissedFrame.Value);
                    }
                    else
                    {//Both Critical.
                        NonDemoBattleFrame? CriticalFrame = GetCriticalFrame(AttackerResult, 1, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                        if (CriticalFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                        }
                        else//Individual
                        {
                            for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                MissedFrame = GetMissedFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (MissedFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(MissedFrame.Value);
                                }
                                else
                                {
                                    CriticalFrame = GetCriticalFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                    if (CriticalFrame != null)
                                    {
                                        ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                                    }
                                }
                            }
                        }
                        NonDemoBattleFrame? ShieldFrame = GetShieldFrame(AttackerResult, 1, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                        NonDemoBattleFrame? BarrierFrame = GetBarrierFrame(AttackerResult, 1, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                        if (ShieldFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                        }
                        else
                        {
                            for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                ShieldFrame = GetShieldFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (ShieldFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                                }
                            }
                        }
                        if (BarrierFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                        }
                        else
                        {
                            for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                BarrierFrame = GetBarrierFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (BarrierFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                                }
                            }
                        }

                        ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.GetHit, !IsRightAttacking));

                        for (int i = 1; i < AttackerResult.ArrayResult.Length; i++)
                            ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                    }

                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.End, IsRightAttacking));
                }
            }

            #endregion

            #region Focused

            else if (AttackerFormation == FormationChoices.Focused)
            {
                NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
                NonDemoUnitStancePositions[] ArrayAttackerStance = { NonDemoUnitStancePositions.WingmanA, NonDemoUnitStancePositions.WingmanB };

                for (int i = 1; i < AttackerResult.ArrayResult.Length && i < Attacker.UnitsAliveInSquad && i < Defender.UnitsAliveInSquad; i++)
                {
                    if (ArrayAttackerHP[i] > 0 && Attacker[i].BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                    {
                        ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, ArrayAttackerStance[i], NonDemoUnitStances.Start, IsRightAttacking));
                        NonDemoBattleFrame AttackNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, ArrayAttackerStance[i], NonDemoUnitStances.Attack, IsRightAttacking);

                        NonDemoBattleFrame? MissedFrame = GetMissedFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                        if (MissedFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(MissedFrame.Value);
                        }
                        else
                        {
                            if (!LeaderHit)
                            {
                                if ((DefaultNonDemoBattleFrame.LeftStance.DefenseSupport && IsRightAttacking) || (DefaultNonDemoBattleFrame.RightStance.DefenseSupport && !IsRightAttacking))
                                {
                                    NonDemoBattleFrame SupportNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.SwitchWithSupport, !IsRightAttacking);
                                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(SupportNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.SwitchWithLeader, !IsRightAttacking));
                                    Targets = NonDemoUnitStancePositions.Support;
                                }
                            }

                            LeaderHit = true;
                            NonDemoBattleFrame? CriticalFrame = GetCriticalFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                            if (CriticalFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                            }
                            NonDemoBattleFrame? ShieldFrame = GetShieldFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                            NonDemoBattleFrame? BarrierFrame = GetBarrierFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                            if (ShieldFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                            }
                            if (BarrierFrame != null)
                            {
                                ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                            }

                            ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.GetHit, !IsRightAttacking));
                            ArrayDefenderHP[0] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[0] - AttackerResult.ArrayResult[i].AttackDamage);
                        }

                        ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, ArrayAttackerStance[i], NonDemoUnitStances.End, IsRightAttacking));

                        if (ArrayDefenderHP[0] <= 0)
                        {
                            return;
                        }
                    }
                }
            }

            #endregion

            #region ALL

            else if (ArrayAttackerHP[0] > 0 && Attacker.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack && AttackerFormation == FormationChoices.ALL)
            {
                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.Start, IsRightAttacking));
                NonDemoBattleFrame AttackNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.Attack, IsRightAttacking);
                NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
                if (ArrayDefenderHP.Length > 2 && ArrayDefenderHP[2] > 0)
                {
                    Targets = NonDemoUnitStancePositions.Leader | NonDemoUnitStancePositions.WingmanA | NonDemoUnitStancePositions.WingmanB;
                }
                else if (ArrayDefenderHP.Length > 1 && ArrayDefenderHP[1] > 0)
                {
                    Targets = NonDemoUnitStancePositions.Leader | NonDemoUnitStancePositions.WingmanA;
                }

                NonDemoBattleFrame? MissedFrame = GetMissedFrame(AttackerResult, 0, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                //Both Missed.
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame.Value);
                }
                else
                {
                    MissedFrame = GetSwordCutOrShootDownFrame(AttackerResult, 0, 0, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                    //If leader use Sword Cut or shoot down, stop everything
                    if (MissedFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(MissedFrame.Value);
                    }
                    else
                    {
                        //Both Critical.
                        NonDemoBattleFrame? CriticalFrame = GetCriticalFrame(AttackerResult, 0, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                        if (CriticalFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                        }
                        else//Individual
                        {
                            for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                MissedFrame = GetMissedFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (MissedFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(MissedFrame.Value);
                                }
                                else
                                {
                                    CriticalFrame = GetCriticalFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                    if (CriticalFrame != null)
                                    {
                                        ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                                    }
                                }
                            }
                        }
                        NonDemoBattleFrame? ShieldFrame = GetShieldFrame(AttackerResult, 0, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                        NonDemoBattleFrame? BarrierFrame = GetBarrierFrame(AttackerResult, 0, -1, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                        if (ShieldFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                        }
                        else
                        {
                            for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                ShieldFrame = GetShieldFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (ShieldFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                                }
                            }
                        }
                        if (BarrierFrame != null)
                        {
                            ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                        }
                        else
                        {
                            for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                            {
                                BarrierFrame = GetBarrierFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                                if (BarrierFrame != null)
                                {
                                    ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                                }
                            }
                        }

                        ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.GetHit, !IsRightAttacking));

                        for (int i = 0; i < AttackerResult.ArrayResult.Length; i++)
                            ArrayDefenderHP[i] = Math.Max(Defender[i].Boosts.HPMinModifier, Defender[i].HP - AttackerResult.ArrayResult[i].AttackDamage);
                    }
                }

                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.End, IsRightAttacking));

                return;
            }

            #endregion

            if (ArrayDefenderHP[0] > 0 && Attacker.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.Start, IsRightAttacking));
                NonDemoBattleFrame AttackNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.Attack, IsRightAttacking);

                NonDemoUnitStancePositions Targets = NonDemoUnitStancePositions.Leader;
                int i = 0;

                NonDemoBattleFrame? MissedFrame = GetMissedFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                if (MissedFrame != null)
                {
                    ListNonDemoBattleFrame.Add(MissedFrame.Value);
                }
                else
                {
                    if (!LeaderHit)
                    {
                        if ((DefaultNonDemoBattleFrame.LeftStance.DefenseSupport && IsRightAttacking) || (DefaultNonDemoBattleFrame.RightStance.DefenseSupport && !IsRightAttacking))
                        {
                            NonDemoBattleFrame SupportNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.SwitchWithSupport, !IsRightAttacking);
                            ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(SupportNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.SwitchWithLeader, !IsRightAttacking));
                            Targets = NonDemoUnitStancePositions.Support;
                        }
                    }
                    LeaderHit = true;
                    NonDemoBattleFrame? CriticalFrame = GetCriticalFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                    if (CriticalFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(CriticalFrame.Value);
                    }
                    NonDemoBattleFrame? ShieldFrame = GetShieldFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);
                    NonDemoBattleFrame? BarrierFrame = GetBarrierFrame(AttackerResult, i, i, AttackNonDemoBattleFrame, Targets, IsRightAttacking);

                    if (ShieldFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(ShieldFrame.Value);
                    }
                    if (BarrierFrame != null)
                    {
                        ListNonDemoBattleFrame.Add(BarrierFrame.Value);
                    }

                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(AttackNonDemoBattleFrame, AttackerResult, Targets, NonDemoUnitStances.GetHit, !IsRightAttacking));
                    ArrayDefenderHP[i] = Math.Max(Defender.CurrentLeader.Boosts.HPMinModifier, ArrayDefenderHP[i] - AttackerResult.ArrayResult[i].AttackDamage);
                }

                if (LeaderHit && ((DefaultNonDemoBattleFrame.LeftStance.DefenseSupport && IsRightAttacking) || (DefaultNonDemoBattleFrame.RightStance.DefenseSupport && !IsRightAttacking)))
                {
                    NonDemoBattleFrame SupportNonDemoBattleFrame = new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.SwitchBackWithSupport, !IsRightAttacking);
                    ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(SupportNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Support, NonDemoUnitStances.SwitchBackWithLeader, !IsRightAttacking));
                }

                ListNonDemoBattleFrame.Add(new NonDemoBattleFrame(DefaultNonDemoBattleFrame, AttackerResult, NonDemoUnitStancePositions.Leader, NonDemoUnitStances.End, IsRightAttacking));

                if (ArrayDefenderHP[i] <= 0)
                {
                    return;
                }
            }
        }

        private NonDemoBattleFrame GetDefaultNonDemoBattleFrame(Squad AttackingSquad, Squad AttackingSupport, Squad DefendingSquad, Squad DefendingSupport, bool IsAttackerOnRight)
        {
            NonDemoBattleFrame DefaultNonDemoBattleFrame = new NonDemoBattleFrame();
            NonDemoBattleFrame.NonDemoBattleFrameSquad AttackingSquadFrame = new NonDemoBattleFrame.NonDemoBattleFrameSquad();
            NonDemoBattleFrame.NonDemoBattleFrameSquad DefendingSquadFrame = new NonDemoBattleFrame.NonDemoBattleFrameSquad();

            AttackingSquadFrame.LeaderStance = NonDemoUnitStances.Idle;
            DefendingSquadFrame.LeaderStance = NonDemoUnitStances.Idle;

            #region Wingmans

            if (NonDemoRightSquad.CurrentWingmanB != null)
            {
                AttackingSquadFrame.WingmanAStance = NonDemoUnitStances.Idle;
                AttackingSquadFrame.WingmanBStance = NonDemoUnitStances.Idle;
            }
            else
            {
                AttackingSquadFrame.WingmanBStance = NonDemoUnitStances.Invisible;

                if (NonDemoRightSquad.CurrentWingmanA != null)
                    AttackingSquadFrame.WingmanAStance = NonDemoUnitStances.Idle;
                else
                    AttackingSquadFrame.WingmanAStance = NonDemoUnitStances.Invisible;
            }

            if (NonDemoLeftSquad.CurrentWingmanB != null)
            {
                DefendingSquadFrame.WingmanAStance = NonDemoUnitStances.Idle;
                DefendingSquadFrame.WingmanBStance = NonDemoUnitStances.Idle;
            }
            else
            {
                DefendingSquadFrame.WingmanBStance = NonDemoUnitStances.Invisible;

                if (NonDemoLeftSquad.CurrentWingmanA != null)
                    DefendingSquadFrame.WingmanAStance = NonDemoUnitStances.Idle;
                else
                    DefendingSquadFrame.WingmanAStance = NonDemoUnitStances.Invisible;
            }

            #endregion

            #region Support

            AttackingSquadFrame.SupportAttackStance = NonDemoUnitStances.Invisible;

            //Support Attack
            if (AttackingSupport != null)
            {
                AttackingSquadFrame.SupportAttackStance = NonDemoUnitStances.Idle;
            }

            //Support Defend
            DefendingSquadFrame.SupportAttackStance = NonDemoUnitStances.Invisible;
            if (DefendingSupport != null)
            {
                DefendingSquadFrame.DefenseSupport = true;
            }
            
            #endregion

            if (IsAttackerOnRight)
            {
                DefaultNonDemoBattleFrame.RightStance = AttackingSquadFrame;
                DefaultNonDemoBattleFrame.LeftStance = DefendingSquadFrame;
            }
            else
            {
                DefaultNonDemoBattleFrame.RightStance = DefendingSquadFrame;
                DefaultNonDemoBattleFrame.LeftStance = AttackingSquadFrame;
            }

            return DefaultNonDemoBattleFrame;
        }

        private NonDemoBattleFrame? GetMissedFrame(SquadBattleResult Result, int Start, int End, NonDemoBattleFrame AttackNonDemoBattleFrame, NonDemoUnitStancePositions Target, bool IsRightAttacking)
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

            NonDemoBattleFrame? SwordCutOrShootDownFrame = GetSwordCutOrShootDownFrame(Result, Start, End, AttackNonDemoBattleFrame, Target, IsRightAttacking);
            NonDemoBattleFrame? ActiveNonDemoBattleFrame = null;

            if (AllMissed)
            {
                if (SwordCutOrShootDownFrame != null)
                    ActiveNonDemoBattleFrame = SwordCutOrShootDownFrame;
                else
                    ActiveNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.GetMissed, !IsRightAttacking);
            }

            return ActiveNonDemoBattleFrame;
        }

        private NonDemoBattleFrame? GetSwordCutOrShootDownFrame(SquadBattleResult Result, int Start, int End, NonDemoBattleFrame AttackNonDemoBattleFrame, NonDemoUnitStancePositions Target, bool IsRightAttacking)
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

            NonDemoBattleFrame? ActiveNonDemoBattleFrame = null;

            if (AllMissed)
            {
                if (AllSwordCut)
                {
                    ActiveNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.GetSwordCut, !IsRightAttacking);
                }
                else if (AllShootDown)
                {
                    ActiveNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.GetShootDown, !IsRightAttacking);
                }
            }

            return ActiveNonDemoBattleFrame;
        }

        private NonDemoBattleFrame? GetCriticalFrame(SquadBattleResult Result, int Start, int End, NonDemoBattleFrame AttackNonDemoBattleFrame, NonDemoUnitStancePositions Target, bool IsRightAttacking)
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

            NonDemoBattleFrame? ActiveNonDemoBattleFrame = null;

            if (AllCritical)
            {
                return new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.GetCriticalHit, !IsRightAttacking);
            }
            return ActiveNonDemoBattleFrame;
        }

        private NonDemoBattleFrame? GetShieldFrame(SquadBattleResult Result, int Start, int End, NonDemoBattleFrame AttackNonDemoBattleFrame, NonDemoUnitStancePositions Target, bool IsRightAttacking)
        {
            if (End == -1)
                End = Result.ArrayResult.Length - 1;

            bool AllShield = true;

            for (int i = Start; i <= End && i < Result.ArrayResult.Length; i++)
            {
                if (!Result.ArrayResult[i].Shield)
                {
                    AllShield = false;
                }
            }

            NonDemoBattleFrame? ActiveNonDemoBattleFrame = null;

            if (AllShield)
            {
                ActiveNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.Shield, !IsRightAttacking);
            }
            return ActiveNonDemoBattleFrame;
        }

        private NonDemoBattleFrame? GetBarrierFrame(SquadBattleResult Result, int Start, int End, NonDemoBattleFrame AttackNonDemoBattleFrame, NonDemoUnitStancePositions Target, bool IsRightAttacking)
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

            NonDemoBattleFrame? ActiveNonDemoBattleFrame = null;

            if (AllBarrier)
            {
                ActiveNonDemoBattleFrame = new NonDemoBattleFrame(AttackNonDemoBattleFrame, Result, Target, NonDemoUnitStances.Barrier, !IsRightAttacking);
            }
            return ActiveNonDemoBattleFrame;
        }

        public override void Update(GameTime gameTime)
        {
            --NonDemoAnimationTimer;
            if (Constants.ShowAnimation)//Animation was cancelled, show the non demo 2 times faster.
                --NonDemoAnimationTimer;

            //Update the explosion if needed.
            if (!sprNonDemoExplosionLeader.AnimationEnded)
                sprNonDemoExplosionLeader.Update(gameTime);
            if (!sprNonDemoExplosionWingmanA.AnimationEnded)
                sprNonDemoExplosionWingmanA.Update(gameTime);
            if (!sprNonDemoExplosionWingmanB.AnimationEnded)
                sprNonDemoExplosionWingmanB.Update(gameTime);

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

                    #region Get Hit

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.GetHit)
                    {//Support.
                        if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.DefenseSupport && (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.GetHit || ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.GetCriticalHit))
                            NonDemoRightSupportStartingHP = Math.Max(NonDemoRightSupport.CurrentLeader.Boosts.HPMinModifier, NonDemoRightSupportStartingHP - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage);
                        else
                            NonDemoRightSquadStartingHP[0] = Math.Max(NonDemoRightSquad.CurrentLeader.Boosts.HPMinModifier, NonDemoRightSquadStartingHP[0] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage);

                        InitExplosion(ref sprNonDemoExplosionLeader, NonDemoRightUnitPosition.X + 20, NonDemoRightUnitPosition.Y + 23);
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanAStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoRightSquadStartingHP[1] = Math.Max(NonDemoRightSquad.CurrentWingmanA.Boosts.HPMinModifier, NonDemoRightSquadStartingHP[1] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackDamage);
						InitExplosion(ref sprNonDemoExplosionWingmanA, NonDemoRightUnitPosition.X + 20, NonDemoRightUnitPosition.Y + 73);
					}
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanBStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoRightSquadStartingHP[2] = Math.Max(NonDemoRightSquad.CurrentWingmanB.Boosts.HPMinModifier, NonDemoRightSquadStartingHP[2] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackDamage);
						InitExplosion(ref sprNonDemoExplosionWingmanB, NonDemoRightUnitPosition.X + 20, NonDemoRightUnitPosition.Y + 123);
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoRightSupportStartingHP = Math.Max(NonDemoRightSupport.CurrentLeader.Boosts.HPMinModifier, NonDemoRightSupportStartingHP - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackDamage);
                        InitExplosion(ref sprNonDemoExplosionLeader, NonDemoRightUnitPosition.X + 20, NonDemoRightUnitPosition.Y + 23);
                    }

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.GetHit)
                    {
                        if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.DefenseSupport && (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.GetHit || ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.GetCriticalHit))
                            NonDemoLeftSupportStartingHP = Math.Max(NonDemoLeftSupport.CurrentLeader.Boosts.HPMinModifier, NonDemoLeftSupportStartingHP - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage);
                        else
                            NonDemoLeftSquadStartingHP[0] = Math.Max(NonDemoLeftSquad.CurrentLeader.Boosts.HPMinModifier, NonDemoLeftSquadStartingHP[0] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage);

                        InitExplosion(ref sprNonDemoExplosionLeader, NonDemoLeftUnitPosition.X + 20, NonDemoLeftUnitPosition.Y + 23);
					}
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanAStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoLeftSquadStartingHP[1] = Math.Max(NonDemoLeftSquad.CurrentWingmanA.Boosts.HPMinModifier, NonDemoLeftSquadStartingHP[1] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackDamage);
						InitExplosion(ref sprNonDemoExplosionWingmanA, NonDemoLeftUnitPosition.X + 20, NonDemoLeftUnitPosition.Y + 73);
					}
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanBStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoLeftSquadStartingHP[2] = Math.Max(NonDemoLeftSquad.CurrentWingmanB.Boosts.HPMinModifier, NonDemoLeftSquadStartingHP[2] - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackDamage);
						InitExplosion(ref sprNonDemoExplosionWingmanB, NonDemoLeftUnitPosition.X + 20, NonDemoLeftUnitPosition.Y + 123);
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance == NonDemoUnitStances.GetHit)
                    {
                        NonDemoLeftSupportStartingHP = Math.Max(NonDemoLeftSupport.CurrentLeader.Boosts.HPMinModifier, NonDemoLeftSupportStartingHP - ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackDamage);
                        InitExplosion(ref sprNonDemoExplosionLeader, NonDemoLeftUnitPosition.X + 20, NonDemoLeftUnitPosition.Y + 23);
                    }

                    #endregion

                    #region Attack

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoRightSquadStartingEN[0] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanAStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoRightSquadStartingEN[1] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanBStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoRightSquadStartingEN[2] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoRightSupportStartingEN = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackAttackerFinalEN;
                    }

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoLeftSquadStartingEN[0] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanAStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoLeftSquadStartingEN[1] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanBStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoLeftSquadStartingEN[2] = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackAttackerFinalEN;
                    }
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoLeftSupportStartingEN = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackAttackerFinalEN;
                    }

                    #endregion
				}
            }
        }

        private void NonDemoBattleFinished()
        {
            RemoveWithoutUnloading(this);
            
            Map.FinalizeBattle(Attacker, ActiveSquadSupport, AttackerPlayerIndex, Defender, DefenderSquadSupport, DefenderPlayerIndex, AttackerSquadResult, DefenderSquadResult);

            NonDemoAnimationTimer = -1;

            if (Map.ListMAPAttackTarget.Count > 0)
            {
                Tuple<int, int> NextTarget = Map.ListMAPAttackTarget.Pop();
                Map.TargetPlayerIndex = NextTarget.Item1;
                Map.TargetSquadIndex = NextTarget.Item2;

                SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();
                TargetSquadSupport.PrepareDefenceSupport(Map, NextTarget.Item1, Map.ListPlayer[NextTarget.Item1].ListSquad[NextTarget.Item2]);
                Map.ReadyNextMAPAttack(Attacker, ActiveSquadSupport, AttackerPlayerIndex, Map.ListPlayer[NextTarget.Item1].ListSquad[NextTarget.Item2], TargetSquadSupport, DefenderPlayerIndex);
            }
        }
        
        private void InitExplosion(ref AnimatedSprite sprExplosion, float PositionX, float PositionY)
        {
            sprExplosion.Position.X = PositionX;
            sprExplosion.Position.Y = PositionY;
            sprExplosion.RestartAnimation();

            sndNonDemoAttack.Play();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (NonDemoRightSquad.IsFlying)
            {
                g.Draw(Map.sprUnitHover, new Vector2((NonDemoRightSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoRightSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
                g.Draw(NonDemoRightSquad.CurrentLeader.SpriteMap, new Vector2((NonDemoRightSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoRightSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y - 7), Color.White);
            }
            else
            {
                g.Draw(NonDemoRightSquad.CurrentLeader.SpriteMap, new Vector2((NonDemoRightSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoRightSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }
            if (NonDemoLeftSquad.IsFlying)
            {
                g.Draw(Map.sprUnitHover, new Vector2((NonDemoLeftSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoLeftSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
                g.Draw(NonDemoLeftSquad.CurrentLeader.SpriteMap, new Vector2((NonDemoLeftSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoLeftSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y - 7), Color.White);
            }
            else
            {
                g.Draw(NonDemoLeftSquad.CurrentLeader.SpriteMap, new Vector2((NonDemoLeftSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (NonDemoLeftSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.White);
            }

            if (CurrentNonDemoBattleFrame < 0)
                return;

            int DrawX;
            int DrawY;

            #region Right Squad

            DrawX = (int)NonDemoRightUnitPosition.X;
            DrawY = (int)NonDemoRightUnitPosition.Y;

            //Support
            if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance != NonDemoUnitStances.Invisible)
            {
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance == NonDemoUnitStances.Idle)
                {
                    NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSquad.CurrentLeader.SpriteMap,
                        NonDemoRightSquadStartingHP[0], NonDemoRightSquad.CurrentLeader.MaxHP,
                        NonDemoRightSquadStartingEN[0], NonDemoRightSquad.CurrentLeader.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance, true,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage.ToString(),
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].Barrier);

                    NonDemoDrawUnit(g, DrawX + 120, DrawY, NonDemoRightSupport.CurrentLeader.SpriteMap,
                        NonDemoRightSupportStartingHP, NonDemoRightSupport.CurrentLeader.MaxHP,
                        NonDemoRightSupportStartingEN, NonDemoRightSupport.CurrentLeader.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance, true,
                            "", "");
                }
                else//Support switching or support attack
                {
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.Idle)
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY - 50, NonDemoRightSquad.CurrentLeader.SpriteMap,
                            NonDemoRightSquadStartingHP[0], NonDemoRightSquad.CurrentLeader.MaxHP,
                            NonDemoRightSquadStartingEN[0], NonDemoRightSquad.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance, true,
                            "", "");
                    }
                    else
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSquad.CurrentLeader.SpriteMap,
                            NonDemoRightSquadStartingHP[0], NonDemoRightSquad.CurrentLeader.MaxHP,
                            NonDemoRightSquadStartingEN[0], NonDemoRightSquad.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance, true,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage.ToString(),
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].Barrier);
                    }

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSupport.CurrentLeader.SpriteMap,
                            NonDemoRightSupportStartingHP, NonDemoRightSupport.CurrentLeader.MaxHP,
                            NonDemoRightSupportStartingEN, NonDemoRightSupport.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance, true,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackDamage.ToString(),
                            "");
                    }
                    else
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSupport.CurrentLeader.SpriteMap,
                            NonDemoRightSupportStartingHP, NonDemoRightSupport.CurrentLeader.MaxHP,
                            NonDemoRightSupportStartingEN, NonDemoRightSupport.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance, true,
                            "", "");
                    }
                }
            }
            else
            {
                BattleResult LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0];
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.GetHit)
                {
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanAStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.WingmanBStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack;
                    }
                }

                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSquad.CurrentLeader.SpriteMap,
                    NonDemoRightSquadStartingHP[0], NonDemoRightSquad.CurrentLeader.MaxHP,
                    NonDemoRightSquadStartingEN[0], NonDemoRightSquad.CurrentLeader.MaxEN,
                    ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance, true,
                    LeaderGettingAttackedResult.AttackDamage.ToString(),
                    LeaderGettingAttackedResult.Barrier);
            }

            if (NonDemoRightSquad.CurrentWingmanA != null)
            {
                DrawX = (int)NonDemoRightUnitPosition.X + 5;
                DrawY = (int)NonDemoRightUnitPosition.Y + 50;
                string AttackDamage = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 1)
                    AttackDamage = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackDamage.ToString();
                string Barrier = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 1)
                    Barrier = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].Barrier;

                //Wingman A.
                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSquad.CurrentWingmanA.SpriteMap,
                        NonDemoRightSquadStartingHP[1], NonDemoRightSquad.CurrentWingmanA.MaxHP,
                        NonDemoRightSquadStartingEN[1], NonDemoRightSquad.CurrentWingmanA.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanAStance, true, AttackDamage, Barrier);
            }

            if (NonDemoRightSquad.CurrentWingmanB != null)
            {
                DrawX = (int)NonDemoRightUnitPosition.X + 5;
                DrawY = (int)NonDemoRightUnitPosition.Y + 100;
                string AttackDamage = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 2)
                    AttackDamage = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackDamage.ToString();
                string Barrier = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 2)
                    Barrier = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].Barrier;

                //Wingman B.
                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoRightSquad.CurrentWingmanB.SpriteMap,
                        NonDemoRightSquadStartingHP[2], NonDemoRightSquad.CurrentWingmanB.MaxHP,
                        NonDemoRightSquadStartingEN[2], NonDemoRightSquad.CurrentWingmanB.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanBStance, true, AttackDamage, Barrier);
            }

            #endregion

            #region Left Squad

            DrawX = (int)NonDemoLeftUnitPosition.X;
            DrawY = (int)NonDemoLeftUnitPosition.Y;

            //Support
            if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance != NonDemoUnitStances.Invisible)
            {
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance == NonDemoUnitStances.Idle)
                {
                    NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSquad.CurrentLeader.SpriteMap,
                        NonDemoLeftSquadStartingHP[0], NonDemoLeftSquad.CurrentLeader.MaxHP,
                        NonDemoLeftSquadStartingEN[0], NonDemoLeftSquad.CurrentLeader.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage.ToString(),
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].Barrier);

                    NonDemoDrawUnit(g, DrawX - 120, DrawY, NonDemoLeftSupport.CurrentLeader.SpriteMap,
                        NonDemoLeftSupportStartingHP, NonDemoLeftSupport.CurrentLeader.MaxHP,
                        NonDemoLeftSupportStartingEN, NonDemoLeftSupport.CurrentLeader.MaxEN,
                        ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance, false,
                            "", "");
                }
                else//Support switching or support attack
                {
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.Idle)
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY - 50, NonDemoLeftSquad.CurrentLeader.SpriteMap,
                            NonDemoLeftSquadStartingHP[0], NonDemoLeftSquad.CurrentLeader.MaxHP,
                            NonDemoLeftSquadStartingEN[0], NonDemoLeftSquad.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false,
                            "", "");
                    }
                    else
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSquad.CurrentLeader.SpriteMap,
                            NonDemoLeftSquadStartingHP[0], NonDemoLeftSquad.CurrentLeader.MaxHP,
                            NonDemoLeftSquadStartingEN[0], NonDemoLeftSquad.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].AttackDamage.ToString(),
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0].Barrier);
                    }

                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSupport.CurrentLeader.SpriteMap,
                            NonDemoLeftSupportStartingHP, NonDemoLeftSupport.CurrentLeader.MaxHP,
                            NonDemoLeftSupportStartingEN, NonDemoLeftSupport.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance, false,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack.AttackDamage.ToString(),
                            "");
                    }
                    else
                    {
                        NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSupport.CurrentLeader.SpriteMap,
                            NonDemoLeftSupportStartingHP, NonDemoLeftSupport.CurrentLeader.MaxHP,
                            NonDemoLeftSupportStartingEN, NonDemoLeftSupport.CurrentLeader.MaxEN,
                            ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.SupportAttackStance, false,
                            "", "");
                    }
                }
            }
            else
            {
                BattleResult LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0];
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance == NonDemoUnitStances.GetHit)
                {
                    if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.LeaderStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[0];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanAStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.WingmanBStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2];
                    }
                    else if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].RightStance.SupportAttackStance == NonDemoUnitStances.Attack)
                    {
                        LeaderGettingAttackedResult = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ResultSupportAttack;
                    }
                }

                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSquad.CurrentLeader.SpriteMap,
                    NonDemoLeftSquadStartingHP[0], NonDemoLeftSquad.CurrentLeader.MaxHP,
                    NonDemoLeftSquadStartingEN[0], NonDemoLeftSquad.CurrentLeader.MaxEN,
                    ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false,
                    LeaderGettingAttackedResult.AttackDamage.ToString(),
                    LeaderGettingAttackedResult.Barrier);
            }

            if (NonDemoLeftSquad.CurrentWingmanA != null)
            {
                DrawX = (int)NonDemoLeftUnitPosition.X + 5;
                DrawY = (int)NonDemoLeftUnitPosition.Y + 50;
                string AttackDamage = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 1)
                    AttackDamage = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].AttackDamage.ToString();
                string Barrier = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 1)
                    Barrier = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[1].Barrier;

                //Wingman A.
                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSquad.CurrentWingmanA.SpriteMap,
                    NonDemoLeftSquadStartingHP[1], NonDemoLeftSquad.CurrentWingmanA.MaxHP,
                    NonDemoLeftSquadStartingEN[1], NonDemoLeftSquad.CurrentWingmanA.MaxEN,
                    ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false, AttackDamage, Barrier);
            }

            if (NonDemoLeftSquad.CurrentWingmanB != null)
            {
                DrawX = (int)NonDemoLeftUnitPosition.X + 5;
                DrawY = (int)NonDemoLeftUnitPosition.Y + 100;
                string AttackDamage = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 2)
                    AttackDamage = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].AttackDamage.ToString();
                string Barrier = "";
                if (ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult.Length > 2)
                    Barrier = ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].Result.ArrayResult[2].Barrier;

                //Wingman B.
                NonDemoDrawUnit(g, DrawX, DrawY, NonDemoLeftSquad.CurrentWingmanB.SpriteMap,
                    NonDemoLeftSquadStartingHP[2], NonDemoLeftSquad.CurrentWingmanB.MaxHP,
                    NonDemoLeftSquadStartingEN[2], NonDemoLeftSquad.CurrentWingmanB.MaxEN,
                    ListNonDemoBattleFrame[CurrentNonDemoBattleFrame].LeftStance.LeaderStance, false, AttackDamage, Barrier);
            }

            #endregion

            if (!sprNonDemoExplosionLeader.AnimationEnded)
                sprNonDemoExplosionLeader.Draw(g);

            if (!sprNonDemoExplosionWingmanA.AnimationEnded)
                sprNonDemoExplosionWingmanA.Draw(g);

            if (!sprNonDemoExplosionWingmanB.AnimationEnded)
                sprNonDemoExplosionWingmanB.Draw(g);
        }

        private void NonDemoDrawUnit(CustomSpriteBatch g, int DrawPositionX, int DrawPositionY,
            Texture2D UnitSpriteMap, int UnitHP, int UnitMaxHP, int UnitEN, int UnitMaxEN,
            NonDemoUnitStances ActiveStance, bool IsRight, string Damage, string BarrierName)
        {
            if (ActiveStance == NonDemoUnitStances.Invisible)
                return;

            #region Switch

            if (ActiveStance == NonDemoUnitStances.SwitchWithSupport)
            {
                int LeaderPosY = 50 - (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 50);
                DrawPositionY -= LeaderPosY;
            }
            else if (ActiveStance == NonDemoUnitStances.SwitchWithLeader)
            {
                int SupportPosX = (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 120);
                if (IsRight)
                {
                    DrawPositionX += SupportPosX;
                }
                else
                {
                    DrawPositionX -= SupportPosX;
                }
            }
            else if (ActiveStance == NonDemoUnitStances.SwitchBackWithSupport)
            {
                int LeaderPosY = (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 50);
                DrawPositionY -= LeaderPosY;
            }
            else if (ActiveStance == NonDemoUnitStances.SwitchBackWithLeader)
            {
                int SupportPosX = 150 - (int)(NonDemoAnimationTimer / NonDemoBattleFrame.SwitchLength * 120);
                if (IsRight)
                {
                    DrawPositionX += SupportPosX;
                }
                else
                {
                    DrawPositionX -= SupportPosX;
                }
            }

            #endregion

            DrawBox(g, new Vector2(DrawPositionX, DrawPositionY), 113, 45, Color.White);

            DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallHP, new Vector2(DrawPositionX + 55, DrawPositionY + 9), UnitHP, UnitMaxHP);
            DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallEN, new Vector2(DrawPositionX + 55, DrawPositionY + 26), UnitEN, UnitMaxEN);

            g.DrawStringRightAligned(Map.fntBattleNumberSmall, UnitHP.ToString(), new Vector2(DrawPositionX + 102, DrawPositionY + 1), Color.White);

            g.DrawStringRightAligned(Map.fntBattleNumberSmall, UnitEN.ToString(), new Vector2(DrawPositionX + 103, DrawPositionY + 18), Color.White);

            switch (ActiveStance)
            {
                case NonDemoUnitStances.SwitchWithSupport:
                case NonDemoUnitStances.SwitchWithLeader:
                case NonDemoUnitStances.SwitchBackWithSupport:
                case NonDemoUnitStances.SwitchBackWithLeader:
                case NonDemoUnitStances.Idle:
                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);
                    break;

                #region Start

                case NonDemoUnitStances.Start:
                    if (IsRight)
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 - 8 + NonDemoAnimationTimer, DrawPositionY + 8), Color.White);
                    }
                    else
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 + 8 - NonDemoAnimationTimer, DrawPositionY + 8), Color.White);
                    }
                    break;

                #endregion

                #region Attack

                case NonDemoUnitStances.Attack:
                    if (IsRight)
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 - 8, DrawPositionY + 8), Color.White);
                    }
                    else
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 + 8, DrawPositionY + 8), Color.White);
                    }
                    break;

                #endregion

                #region End

                case NonDemoUnitStances.End:
                    if (IsRight)
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 - NonDemoAnimationTimer, DrawPositionY + 8), Color.White);
                    }
                    else
                    {
                        g.Draw(UnitSpriteMap, new Vector2(
                            DrawPositionX + 2 + NonDemoAnimationTimer, DrawPositionY + 8), Color.White);
                    }
                    break;

                #endregion

                #region Get Hit

                case NonDemoUnitStances.GetHit:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                        if (NonDemoAnimationTimer >= 38)
                        {
                            g.DrawString(Map.fntNonDemoDamage, Damage, new Vector2(
                                DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                        }
                        else if (NonDemoAnimationTimer >= 30)
                        {
                            g.DrawString(Map.fntNonDemoDamage, Damage, new Vector2(
                                DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                        }
                        else
                        {
                            g.DrawString(Map.fntNonDemoDamage, Damage, new Vector2(
                                DrawPositionX + 20, DrawPositionY + 5), Color.White);
                        }
                    break;

                #endregion

                #region Get Missed

                case NonDemoUnitStances.GetMissed:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.Draw(sprNonDemoMiss, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.Draw(sprNonDemoMiss, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.Draw(sprNonDemoMiss, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion

                #region Get Sword Cut

                case NonDemoUnitStances.GetSwordCut:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.DrawString(Map.fntUnitAttack, "SWORD CUT", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.DrawString(Map.fntUnitAttack, "SWORD CUT", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.DrawString(Map.fntUnitAttack, "SWORD CUT", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion

                #region Get Shoot Down

                case NonDemoUnitStances.GetShootDown:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.DrawString(Map.fntUnitAttack, "SHOOT DOWN", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.DrawString(Map.fntUnitAttack, "SHOOT DOWN", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.DrawString(Map.fntUnitAttack, "SHOOT DOWN", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion

                #region Get Critical Hit

                case NonDemoUnitStances.GetCriticalHit:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.Draw(sprNonDemoCritical, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.Draw(sprNonDemoCritical, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.Draw(sprNonDemoCritical, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion

                #region Shield

                case NonDemoUnitStances.Shield:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.DrawString(Map.fntUnitAttack, "SHIELD", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.DrawString(Map.fntUnitAttack, "SHIELD", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.DrawString(Map.fntUnitAttack, "SHIELD", new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion

                #region Barrier

                case NonDemoUnitStances.Barrier:

                    g.Draw(UnitSpriteMap, new Vector2(
                        DrawPositionX + 2, DrawPositionY + 8), Color.White);

                    if (NonDemoAnimationTimer >= 38)
                    {
                        g.DrawString(Map.fntUnitAttack, BarrierName, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - 46 + NonDemoAnimationTimer), Color.White);
                    }
                    else if (NonDemoAnimationTimer >= 30)
                    {
                        g.DrawString(Map.fntUnitAttack, BarrierName, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5 - NonDemoAnimationTimer + 30), Color.White);
                    }
                    else
                    {
                        g.DrawString(Map.fntUnitAttack, BarrierName, new Vector2(
                            DrawPositionX + 20, DrawPositionY + 5), Color.White);
                    }
                    break;

                #endregion
            }
        }
    }
}
