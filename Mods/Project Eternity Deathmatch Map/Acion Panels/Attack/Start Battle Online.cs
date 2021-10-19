using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;
using System.Collections.Generic;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelStartBattleOnline : ActionPanelDeathmatch
    {
        private const string PanelName = "StartBattleOnline ";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private SupportSquadHolder ActiveSquadSupport;

        private int TargetPlayerIndex;
        private int TargetSquadIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;

        private bool IsDefending;

        private SquadBattleResult AttackingResult;
        private SquadBattleResult DefendingResult;

        private List<GameScreen> ListNextAnimationScreen;

        public ActionPanelStartBattleOnline(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListNextAnimationScreen = new List<GameScreen>();
        }

        public ActionPanelStartBattleOnline(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport,
             int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport, bool IsDefending)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ActiveSquadSupport = ActiveSquadSupport;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;
            this.TargetSquadSupport = TargetSquadSupport;

            this.IsDefending = IsDefending;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];

            ListNextAnimationScreen = new List<GameScreen>();
        }

        public override void OnSelect()
        {
            if (Map.IsOfflineOrServer)
            {
                if (IsDefending || Map.ListPlayer[TargetPlayerIndex].IsPlayerControlled)
                {
                    InitPlayerDefence();
                }
                else
                {
                    InitPlayerAttack();
                    ActiveSquad.CurrentLeader.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ListNextAnimationScreen.Count > 0)
            {
                Map.PushScreen(ListNextAnimationScreen[0]);
                ListNextAnimationScreen.Remove(ListNextAnimationScreen[0]);
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquadSupport = new SupportSquadHolder();

            TargetPlayerIndex = BR.ReadInt32();
            TargetSquadIndex = BR.ReadInt32();
            TargetSquadSupport = new SupportSquadHolder();

            IsDefending = BR.ReadBoolean();

            Map.BattleMenuStage = (BattleMenuStages)BR.ReadByte();

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);

            BW.AppendInt32(TargetPlayerIndex);
            BW.AppendInt32(TargetSquadIndex);

            BW.AppendBoolean(IsDefending);

            BW.AppendByte((byte)Map.BattleMenuStage);

            if (Map.IsServer)
            {
                BW.AppendInt32(AttackingResult.ArrayResult.Length);
                for (int R = 0; R < AttackingResult.ArrayResult.Length; ++R)
                {
                    BW.AppendInt32(AttackingResult.ArrayResult[R].Accuracy);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].AttackAttackerFinalEN);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].AttackDamage);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackMissed);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackShootDown);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackSwordCut);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].AttackWasCritical);
                    BW.AppendString(AttackingResult.ArrayResult[R].Barrier);
                    BW.AppendBoolean(AttackingResult.ArrayResult[R].Shield);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].TargetPlayerIndex);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].TargetSquadIndex);
                    BW.AppendInt32(AttackingResult.ArrayResult[R].TargetUnitIndex);
                }

                BW.AppendInt32(DefendingResult.ArrayResult.Length);
                for (int R = 0; R < DefendingResult.ArrayResult.Length; ++R)
                {
                    BW.AppendInt32(DefendingResult.ArrayResult[R].Accuracy);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].AttackAttackerFinalEN);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].AttackDamage);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackMissed);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackShootDown);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackSwordCut);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].AttackWasCritical);
                    BW.AppendString(DefendingResult.ArrayResult[R].Barrier);
                    BW.AppendBoolean(DefendingResult.ArrayResult[R].Shield);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].TargetPlayerIndex);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].TargetSquadIndex);
                    BW.AppendInt32(DefendingResult.ArrayResult[R].TargetUnitIndex);
                }
            }
            else
            {

            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelStartBattleOnline(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public void InitPlayerBattle(bool IsActiveSquadOnRight)
        {
            int FinalActivePlayerIndex = ActivePlayerIndex;
            int FinalActiveSquadIndex = ActiveSquadIndex;
            Squad FinalActiveSquad = ActiveSquad;
            SupportSquadHolder FinalActiveSquadSupport = ActiveSquadSupport;

            int FinalTargetPlayerIndex = TargetPlayerIndex;
            int FinalTargetSquadIndex = TargetSquadIndex;
            Squad FinalTargetSquad = TargetSquad;
            SupportSquadHolder FinalTargetSquadSupport = TargetSquadSupport;

            if (TargetSquad.CurrentLeader.Boosts.AttackFirstModifier && !ActiveSquad.CurrentLeader.Boosts.AttackFirstModifier)
            {
                FinalActivePlayerIndex = TargetPlayerIndex;
                FinalActiveSquadIndex = TargetSquadIndex;
                FinalActiveSquad = TargetSquad;
                FinalActiveSquadSupport = TargetSquadSupport;

                FinalTargetPlayerIndex = ActivePlayerIndex;
                FinalTargetSquadIndex = ActiveSquadIndex;
                FinalTargetSquad = ActiveSquad;
                FinalTargetSquadSupport = ActiveSquadSupport;
            }

            bool ShowAnimation = Constants.ShowAnimation && FinalActiveSquad.CurrentLeader.CurrentAttack.GetAttackAnimations(FormulaParser.ActiveParser).Start.AnimationName != null;
            ListNextAnimationScreen.Clear();
            Map.NonDemoScreen.ListNonDemoBattleFrame.Clear();
            ListActionMenuChoice.RemoveAllSubActionPanels();

            AttackingResult = Map.CalculateFinalHP(FinalActiveSquad, FinalActiveSquadSupport.ActiveSquadSupport, FinalActivePlayerIndex,
                                                                    Map.BattleMenuOffenseFormationChoice, FinalTargetSquad, FinalTargetSquadSupport.ActiveSquadSupport,
                                                                    FinalTargetPlayerIndex, FinalTargetSquadIndex, true, true);

            DefendingResult = new SquadBattleResult(new BattleResult[1] { new BattleResult() });

            AnimationScreen.AnimationUnitStats UnitStats = new AnimationScreen.AnimationUnitStats(FinalActiveSquad, FinalTargetSquad, IsActiveSquadOnRight);
            if (ShowAnimation)
            {
                if (IsActiveSquadOnRight)
                {
                    ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.RightAttackLeft, AttackingResult));
                }
                else
                {
                    ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.LeftAttackRight, AttackingResult));
                }
            }

            if (AttackingResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(AttackingResult.ArrayResult[0].AttackDamage) > 0)
            {
                //Counter.
                if (FinalTargetSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    DefendingResult = Map.CalculateFinalHP(FinalTargetSquad, null, FinalTargetPlayerIndex,
                                                        Map.BattleMenuDefenseFormationChoice, FinalActiveSquad, null,
                                                        FinalActivePlayerIndex, FinalActiveSquadIndex, true, true);

                    if (ShowAnimation)
                    {
                        if (IsActiveSquadOnRight)
                        {
                            ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.RightConteredByLeft, DefendingResult));
                        }
                        else
                        {
                            ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats, AnimationScreen.BattleAnimationTypes.LeftConteredByRight, DefendingResult));
                        }
                    }
                }
            }

            if (ShowAnimation)
            {
                Map.PushScreen(ListNextAnimationScreen[0]);
                ListNextAnimationScreen.RemoveAt(0);
                ListNextAnimationScreen.Add(new EndBattleAnimationScreen(Map, FinalActiveSquad, FinalActiveSquadSupport, FinalActivePlayerIndex,
                    FinalTargetSquad, FinalTargetSquadSupport, FinalTargetPlayerIndex, AttackingResult, DefendingResult));
            }
            else
            {
                Map.NonDemoScreen.InitNonDemo(FinalActiveSquad, FinalActiveSquadSupport, FinalActivePlayerIndex, AttackingResult, Map.BattleMenuOffenseFormationChoice,
                    FinalTargetSquad, FinalTargetSquadSupport, FinalTargetPlayerIndex, DefendingResult, Map.BattleMenuDefenseFormationChoice, IsActiveSquadOnRight);

                Map.NonDemoScreen.Alive = true;
                Map.ListGameScreen.Insert(0, Map.NonDemoScreen);
            }

            //AttackingSquad Activations.
            for (int U = 0; U < FinalActiveSquad.UnitsAliveInSquad; U++)
            {
                FinalActiveSquad[U].UpdateSkillsLifetime(SkillEffect.LifetimeTypeBattle);
            }
            //DefendingSquad Activations.
            for (int U = 0; U < FinalTargetSquad.UnitsAliveInSquad; U++)
            {
                FinalTargetSquad[U].UpdateSkillsLifetime(SkillEffect.LifetimeTypeBattle);
            }

            Map.FinalizeMovement(FinalActiveSquad, (int)Map.GetTerrain(FinalActiveSquad).MovementCost);
            FinalActiveSquad.EndTurn();

            bool HasAfterAttack = false;
            ActionPanelDeathmatch AfterAttack = new ActionPanelMainMenu(Map, FinalActivePlayerIndex, FinalActiveSquadIndex);

            if (FinalActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Attack)
            {
                HasAfterAttack = true;
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, FinalActivePlayerIndex, FinalActiveSquadIndex, FinalActiveSquad.CanMove));
            }

            if (FinalActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Move)
            {
                HasAfterAttack = true;
                Map.CursorPosition = FinalActiveSquad.Position;
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelMovePart1(Map, FinalActivePlayerIndex, FinalActiveSquadIndex, FinalActiveSquad.Position, Map.CameraPosition, true));
            }

            if (HasAfterAttack)
            {
                AfterAttack.AddChoiceToCurrentPanel(new ActionPanelWait(Map, FinalActiveSquad));
                ListActionMenuChoice.Add(AfterAttack);
            }
        }

        public void InitPlayerAttack()
        {
            //Play battle theme.
            if (ActiveSquad.CurrentLeader.BattleTheme == null || ActiveSquad.CurrentLeader.BattleThemeName != GameScreen.FMODSystem.sndActiveBGMName)
            {
                if (ActiveSquad.CurrentLeader.BattleTheme != null)
                {
                    if (GameScreen.FMODSystem.sndActiveBGM != null)
                        GameScreen.FMODSystem.sndActiveBGM.Stop();

                    ActiveSquad.CurrentLeader.BattleTheme.SetLoop(true);
                    ActiveSquad.CurrentLeader.BattleTheme.PlayAsBGM();
                    GameScreen.FMODSystem.sndActiveBGMName = ActiveSquad.CurrentLeader.BattleThemeName;
                }
            }

            InitPlayerBattle(true);
        }

        public void InitPlayerDefence()
        {
            //Play battle theme.
            if (TargetSquad.CurrentLeader.BattleTheme == null || TargetSquad.CurrentLeader.BattleThemeName != GameScreen.FMODSystem.sndActiveBGMName)
            {
                if (TargetSquad.CurrentLeader.BattleTheme != null)
                {
                    if (GameScreen.FMODSystem.sndActiveBGM != null)
                        GameScreen.FMODSystem.sndActiveBGM.Stop();

                    TargetSquad.CurrentLeader.BattleTheme.SetLoop(true);
                    TargetSquad.CurrentLeader.BattleTheme.PlayAsBGM();
                    GameScreen.FMODSystem.sndActiveBGMName = TargetSquad.CurrentLeader.BattleThemeName;
                }
            }

            InitPlayerBattle(false);
        }
    }
}
