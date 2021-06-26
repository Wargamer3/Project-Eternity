using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public void CreateAnimation(string AnimationName, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
            SquadBattleResult BattleResult, AnimationScreen.AnimationUnitStats UnitStats, AnimationBackground ActiveTerrain, string ExtraText, bool IsLeftAttacking)
        {
            AnimationScreen NewAnimationScreen = new AnimationScreen(AnimationName, Map, ActiveSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveTerrain, ExtraText, IsLeftAttacking);

            ListNextAnimationScreen.Add(NewAnimationScreen);
        }

        public void CreateAnimation(AnimationInfo Info, DeathmatchMap Map, Squad ActiveSquad, Squad EnemySquad, Attack ActiveAttack,
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

            ListNextAnimationScreen.Add(NewAnimationScreen);
        }

        public List<GameScreen> GenerateNextAnimationScreens(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, Squad TargetSquad, SupportSquadHolder TargetSquadSupport,
            AnimationScreen.AnimationUnitStats UnitStats, AnimationScreen.BattleAnimationTypes BattleAnimationType, SquadBattleResult AttackingResult)
        {
            bool IsActiveSquadOnRight = BattleAnimationType == AnimationScreen.BattleAnimationTypes.RightAttackLeft || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftConteredByRight;
            bool HorionztalMirror = BattleAnimationType == AnimationScreen.BattleAnimationTypes.RightConteredByLeft || BattleAnimationType == AnimationScreen.BattleAnimationTypes.LeftAttackRight;
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

            AnimationBackground ActiveAnimationBackground = new AnimationBackground2D("Backgrounds 2D/Ground", Content, GraphicsDevice);

            AttackAnimations AttackerAnimations = ActiveAttack.GetAttackAnimations(FormulaParser.ActiveParser);
            CreateAnimation(AttackingSquad.CurrentLeader.Animations.MoveFoward, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);

            CreateAnimation(AttackerAnimations.Start, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, ExtraTextIntro, HorionztalMirror);

            if (BattleResult.ArrayResult[0].AttackMissed)
            {
                CreateAnimation(AttackerAnimations.EndMiss, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, ExtraTextMiss, HorionztalMirror);
            }
            else
            {
                // Check for support
                if (EnemySupport != null)
                {
                    CreateAnimation("Default Animations/Support In", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) > 0)
                    {
                        CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);
                        CreateAnimation("Default Animations/Support Out", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);
                    }
                    else
                    {
                        CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);
                        CreateAnimation("Default Animations/Support Destroyed", this, EnemySquad, EnemySupport, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, "", HorionztalMirror);
                    }
                }
                else
                {
                    if (BattleResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(BattleResult.ArrayResult[0].AttackDamage) <= 0)
                    {
                        CreateAnimation(AttackerAnimations.EndDestroyed, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, ExtraTextKill, HorionztalMirror);
                    }
                    else
                    {
                        CreateAnimation(AttackerAnimations.EndHit, this, AttackingSquad, EnemySquad, ActiveAttack, BattleResult, UnitStats, ActiveAnimationBackground, ExtraTextHit, HorionztalMirror);
                    }
                }
            }

            return ListNextAnimationScreen;
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

        public void InitPlayerBattle(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex,
            Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int DefenderPlayerIndex,
            bool IsActiveSquadOnRight)
        {
            if (TargetSquad.CurrentLeader.Boosts.AttackFirstModifier && !ActiveSquad.CurrentLeader.Boosts.AttackFirstModifier)
            {
                Squad ActiveSquadTemp = ActiveSquad;
                SupportSquadHolder ActiveSquadSupportTemp = ActiveSquadSupport;
                int AttackerPlayerIndexTemp = AttackerPlayerIndex;

                ActiveSquad = TargetSquad;
                ActiveSquadSupport = TargetSquadSupport;
                AttackerPlayerIndex = DefenderPlayerIndex;

                TargetSquad = ActiveSquadTemp;
                TargetSquadSupport = ActiveSquadSupportTemp;
                DefenderPlayerIndex = AttackerPlayerIndexTemp;
            }

            ActivePlayerIndex = AttackerPlayerIndex;
            ActiveSquadIndex = ListPlayer[AttackerPlayerIndex].ListSquad.IndexOf(ActiveSquad);
            TargetPlayerIndex = DefenderPlayerIndex;
            TargetSquadIndex = ListPlayer[TargetPlayerIndex].ListSquad.IndexOf(TargetSquad);

            bool ShowAnimation = Constants.ShowAnimation && ActiveSquad.CurrentLeader.CurrentAttack.GetAttackAnimations(FormulaParser.ActiveParser).Start.AnimationName != null;
            ListNextAnimationScreen.Clear();
            NonDemoScreen.ListNonDemoBattleFrame.Clear();
            ListActionMenuChoice.RemoveAllSubActionPanels();

            SquadBattleResult AttackingResult = CalculateFinalHP(ActiveSquad, ActiveSquadSupport.ActiveSquadSupport, ActivePlayerIndex,
                                                                    BattleMenuOffenseFormationChoice, TargetSquad, TargetSquadSupport.ActiveSquadSupport, TargetPlayerIndex, true, true);

            AnimationScreen.AnimationUnitStats UnitStats = new AnimationScreen.AnimationUnitStats(ActiveSquad, TargetSquad, IsActiveSquadOnRight);
            SquadBattleResult DefendingResult = new SquadBattleResult(new BattleResult[1] { new BattleResult() });
            if (ShowAnimation)
            {
                if (IsActiveSquadOnRight)
                {
                    GenerateNextAnimationScreens(ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.RightAttackLeft, AttackingResult);
                }
                else
                {
                    GenerateNextAnimationScreens(ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.LeftAttackRight, AttackingResult);
                }
            }

            if (AttackingResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(AttackingResult.ArrayResult[0].AttackDamage) > 0)
            {
                //Counter.
                if (TargetSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingResult = CalculateFinalHP(TargetSquad, null, TargetPlayerIndex,
                                                        BattleMenuDefenseFormationChoice, ActiveSquad, null, ActivePlayerIndex, true, true);

                    if (ShowAnimation)
                    {
                        if (IsActiveSquadOnRight)
                        {
                            GenerateNextAnimationScreens(ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.RightConteredByLeft, DefendingResult);
                        }
                        else
                        {
                            GenerateNextAnimationScreens(ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.LeftConteredByRight, DefendingResult);
                        }
                    }
                }
            }

            if (ShowAnimation)
            {
                PushScreen(ListNextAnimationScreen[0]);
                ListNextAnimationScreen.RemoveAt(0);
                ListNextAnimationScreen.Add(new EndBattleAnimationScreen(this, ActiveSquad, ActiveSquadSupport, AttackerPlayerIndex,
                    TargetSquad, TargetSquadSupport, DefenderPlayerIndex, AttackingResult, DefendingResult));
            }
            else
            {
                NonDemoScreen.InitNonDemo(ActiveSquad, ActiveSquadSupport, AttackerPlayerIndex, AttackingResult, BattleMenuOffenseFormationChoice,
                    TargetSquad, TargetSquadSupport, DefenderPlayerIndex, DefendingResult, BattleMenuDefenseFormationChoice, IsActiveSquadOnRight);

                NonDemoScreen.Alive = true;
                ListGameScreen.Insert(0, NonDemoScreen);
            }

            //AttackingSquad Activations.
            for (int U = 0; U < ActiveSquad.UnitsAliveInSquad; U++)
            {
                ActiveSquad[U].UpdateSkillsLifetime(SkillEffect.LifetimeTypeBattle);
            }
            //DefendingSquad Activations.
            for (int U = 0; U < TargetSquad.UnitsAliveInSquad; U++)
            {
                TargetSquad[U].UpdateSkillsLifetime(SkillEffect.LifetimeTypeBattle);
            }

            FinalizeMovement(ActiveSquad, (int)GetTerrain(ActiveSquad).MovementCost);
            ActiveSquad.EndTurn();

            bool HasAfterAttack = false;
            ActionPanelDeathmatch AfterAttack = new ActionPanelMainMenu(this, ActiveSquad, AttackerPlayerIndex);

            if (ActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Attack)
            {
                HasAfterAttack = true;
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelAttackPart1(ActiveSquad.CanMove, ActiveSquad, AttackerPlayerIndex, this));
            }

            if (ActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Move)
            {
                HasAfterAttack = true;
                CursorPosition = ActiveSquad.Position;
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelMovePart1(this, ActiveSquad.Position, CameraPosition, ActiveSquad, AttackerPlayerIndex, true));
            }

            if (HasAfterAttack)
            {
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelWait(this, ActiveSquad));
                ListActionMenuChoice.Add(AfterAttack);
            }
        }

        public void InitPlayerAttack(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex, Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int DefenderPlayerIndex)
        {
            //Play battle theme.
            if (ActiveSquad.CurrentLeader.BattleTheme == null || ActiveSquad.CurrentLeader.BattleThemeName != FMODSystem.sndActiveBGMName)
            {
                if (ActiveSquad.CurrentLeader.BattleTheme != null)
                {
                    if (FMODSystem.sndActiveBGM != null)
                        FMODSystem.sndActiveBGM.Stop();

                    ActiveSquad.CurrentLeader.BattleTheme.SetLoop(true);
                    ActiveSquad.CurrentLeader.BattleTheme.PlayAsBGM();
                    FMODSystem.sndActiveBGMName = ActiveSquad.CurrentLeader.BattleThemeName;
                }
            }

            InitPlayerBattle(ActiveSquad, ActiveSquadSupport, AttackerPlayerIndex, TargetSquad, TargetSquadSupport, DefenderPlayerIndex, true);
        }

        public void InitPlayerDefence(Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int AttackerPlayerIndex, Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int DefenderPlayerIndex)
        {
            //Play battle theme.
            if (TargetSquad.CurrentLeader.BattleTheme == null || TargetSquad.CurrentLeader.BattleThemeName != FMODSystem.sndActiveBGMName)
            {
                if (TargetSquad.CurrentLeader.BattleTheme != null)
                {
                    if (FMODSystem.sndActiveBGM != null)
                        FMODSystem.sndActiveBGM.Stop();

                    TargetSquad.CurrentLeader.BattleTheme.SetLoop(true);
                    TargetSquad.CurrentLeader.BattleTheme.PlayAsBGM();
                    FMODSystem.sndActiveBGMName = TargetSquad.CurrentLeader.BattleThemeName;
                }
            }

            InitPlayerBattle(ActiveSquad, ActiveSquadSupport, AttackerPlayerIndex, TargetSquad, TargetSquadSupport, DefenderPlayerIndex, false);
        }
    }
}
