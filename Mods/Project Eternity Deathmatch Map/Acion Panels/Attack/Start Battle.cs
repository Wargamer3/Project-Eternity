using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelStartBattle : ActionPanelDeathmatch
    {
        private const string PanelName = "StartBattle";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private Attack CurrentAttack;
        private SupportSquadHolder ActiveSquadSupport;
        private List<Vector3> ListMVHoverPoints;

        private int TargetPlayerIndex;
        private int TargetSquadIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;

        private bool IsDefending;

        private SquadBattleResult AttackingResult;
        private SquadBattleResult DefendingResult;

        private List<GameScreen> ListNextAnimationScreen;

        public ActionPanelStartBattle(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
            ListNextAnimationScreen = new List<GameScreen>();
            SendBackToSender = true;
        }

        public ActionPanelStartBattle(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, Attack CurrentAttack, SupportSquadHolder ActiveSquadSupport,
            List<Vector3> ListMVHoverPoints,
             int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport, bool IsDefending)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.CurrentAttack = CurrentAttack;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.ListMVHoverPoints = ListMVHoverPoints;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;
            this.TargetSquadSupport = TargetSquadSupport;

            this.IsDefending = IsDefending;

            SendBackToSender = true;

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
            else if (CurrentAttack.Pri == WeaponPrimaryProperty.PER)
            {
                ListActionMenuChoice.Remove(this);
            }
            else
            {
                ListActionMenuChoice.RemoveAllSubActionPanels();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)BR.ReadByte();
            string ActiveSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(ActiveSquadAttackName))
            {
                foreach (Attack ActiveAttack in ActiveSquad.CurrentLeader.ListAttack)
                {
                    if (ActiveAttack.ItemName == ActiveSquadAttackName)
                    {
                        ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            TargetPlayerIndex = BR.ReadInt32();
            TargetSquadIndex = BR.ReadInt32();
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            TargetSquadSupport = new SupportSquadHolder();
            TargetSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)BR.ReadByte();
            string TargetSquadAttackName = BR.ReadString();
            if (!string.IsNullOrEmpty(TargetSquadAttackName))
            {
                foreach (Attack ActiveAttack in TargetSquad.CurrentLeader.ListAttack)
                {
                    if (ActiveAttack.ItemName == TargetSquadAttackName)
                    {
                        TargetSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            IsDefending = BR.ReadBoolean();

            Map.BattleMenuStage = (BattleMenuStages)BR.ReadByte();

            if (Map.IsServer)
            {
            }
            else
            {
                int AttackingResultArrayResultLength = BR.ReadInt32();
                AttackingResult.ArrayResult = new BattleResult[AttackingResultArrayResultLength];

                for (int R = 0; R < AttackingResultArrayResultLength; ++R)
                {
                    AttackingResult.ArrayResult[R] = new BattleResult();
                    AttackingResult.ArrayResult[R].Accuracy = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackAttackerFinalEN = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackDamage = BR.ReadInt32();
                    AttackingResult.ArrayResult[R].AttackMissed = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackShootDown = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackSwordCut = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].AttackWasCritical = BR.ReadBoolean();
                    AttackingResult.ArrayResult[R].Barrier = BR.ReadString();
                    AttackingResult.ArrayResult[R].Shield = BR.ReadBoolean();

                    int TargetPlayerIndex = BR.ReadInt32();
                    int TargetSquadIndex = BR.ReadInt32();
                    int TargetUnitIndex = BR.ReadInt32();

                    AttackingResult.ArrayResult[R].TargetPlayerIndex = TargetPlayerIndex;
                    AttackingResult.ArrayResult[R].TargetSquadIndex = TargetSquadIndex;
                    AttackingResult.ArrayResult[R].TargetUnitIndex = TargetUnitIndex;

                    AttackingResult.ArrayResult[R].SetTarget(TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex][TargetUnitIndex]);
                }

                int DefendingResultArrayResultLength = BR.ReadInt32();
                DefendingResult.ArrayResult = new BattleResult[DefendingResultArrayResultLength];

                for (int R = 0; R < DefendingResultArrayResultLength; ++R)
                {
                    DefendingResult.ArrayResult[R] = new BattleResult();
                    DefendingResult.ArrayResult[R].Accuracy = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackAttackerFinalEN = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackDamage = BR.ReadInt32();
                    DefendingResult.ArrayResult[R].AttackMissed = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackShootDown = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackSwordCut = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].AttackWasCritical = BR.ReadBoolean();
                    DefendingResult.ArrayResult[R].Barrier = BR.ReadString();
                    DefendingResult.ArrayResult[R].Shield = BR.ReadBoolean();

                    int TargetPlayerIndex = BR.ReadInt32();
                    int TargetSquadIndex = BR.ReadInt32();
                    int TargetUnitIndex = BR.ReadInt32();

                    DefendingResult.ArrayResult[R].TargetPlayerIndex = TargetPlayerIndex;
                    DefendingResult.ArrayResult[R].TargetSquadIndex = TargetSquadIndex;
                    DefendingResult.ArrayResult[R].TargetUnitIndex = TargetUnitIndex;

                    DefendingResult.ArrayResult[R].SetTarget(TargetPlayerIndex, TargetSquadIndex, TargetUnitIndex, Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex][TargetUnitIndex]);
                }
            }

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

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendByte((byte)ActiveSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendString(ActiveSquad.CurrentLeader.ItemName);

            BW.AppendInt32(TargetPlayerIndex);
            BW.AppendInt32(TargetSquadIndex);
            BW.AppendByte((byte)TargetSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendString(TargetSquad.CurrentLeader.ItemName);

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
            return new ActionPanelStartBattle(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }

        public void InitPlayerBattle(bool IsActiveSquadOnRight)
        {
            int FinalActivePlayerIndex = ActivePlayerIndex;
            int FinalActiveSquadIndex = ActiveSquadIndex;
            Squad FinalActiveSquad = ActiveSquad;
            Attack FinalAttack = CurrentAttack;
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
                FinalAttack = FinalActiveSquad.CurrentLeader.CurrentAttack;
                FinalActiveSquadSupport = TargetSquadSupport;

                FinalTargetPlayerIndex = ActivePlayerIndex;
                FinalTargetSquadIndex = ActiveSquadIndex;
                FinalTargetSquad = ActiveSquad;
                FinalTargetSquadSupport = ActiveSquadSupport;
            }

            bool ShowAnimation = Constants.ShowAnimation && FinalAttack.Pri != WeaponPrimaryProperty.MAP && FinalAttack.Pri != WeaponPrimaryProperty.PER
                && FinalAttack.GetAttackAnimations(Map.ActiveParser).Start.AnimationName != null;
            ListNextAnimationScreen.Clear();
            Map.NonDemoScreen.ListNonDemoBattleFrame.Clear();

            if (Map.IsOfflineOrServer)
            {
                AttackingResult = Map.CalculateFinalHP(FinalActiveSquad, FinalAttack, FinalActiveSquadSupport.ActiveSquadSupport, FinalActivePlayerIndex,
                                                                    Map.BattleMenuOffenseFormationChoice, FinalTargetSquad, FinalTargetSquadSupport.ActiveSquadSupport,
                                                                    FinalTargetPlayerIndex, FinalTargetSquadIndex, true, true);

                DefendingResult = new SquadBattleResult(new BattleResult[1] { new BattleResult() });
            }
            else
            {
                Map.CalculateFinalHP(FinalActiveSquad, FinalAttack, FinalActiveSquadSupport.ActiveSquadSupport, FinalActivePlayerIndex,
                                                                       Map.BattleMenuOffenseFormationChoice, FinalTargetSquad, FinalTargetSquadSupport.ActiveSquadSupport,
                                                                       FinalTargetPlayerIndex, FinalTargetSquadIndex, true, true);
            }

            AnimationScreen.AnimationUnitStats UnitStats = new AnimationScreen.AnimationUnitStats(FinalActiveSquad, FinalTargetSquad, IsActiveSquadOnRight);
            AnimationBackground TargetSquadBackground = null;
            AnimationBackground TargetSquadForeground = null;
            if (ShowAnimation)
            {
                AnimationScreen.BattleAnimationTypes BattleAnimationType = AnimationScreen.BattleAnimationTypes.LeftAttackRight;

                if (IsActiveSquadOnRight)
                {
                    BattleAnimationType = AnimationScreen.BattleAnimationTypes.RightAttackLeft;
                }

                ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats, BattleAnimationType,
                    AttackingResult, out TargetSquadBackground, null, out TargetSquadForeground, null));
            }

            if (AttackingResult.ArrayResult[0].Target.ComputeRemainingHPAfterDamage(AttackingResult.ArrayResult[0].AttackDamage) > 0)
            {
                //Counter.
                if (FinalTargetSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    if (Map.IsOfflineOrServer)
                    {
                        DefendingResult = Map.CalculateFinalHP(FinalTargetSquad, FinalTargetSquad.CurrentLeader.CurrentAttack, null, FinalTargetPlayerIndex,
                                                        Map.BattleMenuDefenseFormationChoice, FinalActiveSquad, null,
                                                        FinalActivePlayerIndex, FinalActiveSquadIndex, true, true);
                    }
                    else
                    {
                        Map.CalculateFinalHP(FinalTargetSquad, FinalTargetSquad.CurrentLeader.CurrentAttack, null, FinalTargetPlayerIndex,
                                                        Map.BattleMenuDefenseFormationChoice, FinalActiveSquad, null,
                                                        FinalActivePlayerIndex, FinalActiveSquadIndex, true, true);
                    }

                    if (ShowAnimation)
                    {
                        AnimationScreen.BattleAnimationTypes BattleAnimationType = AnimationScreen.BattleAnimationTypes.LeftConteredByRight;

                        if (IsActiveSquadOnRight)
                        {
                            BattleAnimationType = AnimationScreen.BattleAnimationTypes.RightConteredByLeft;
                        }

                        ListNextAnimationScreen.AddRange(Map.GenerateNextAnimationScreens(FinalActiveSquad, FinalActiveSquadSupport, FinalTargetSquad, FinalTargetSquadSupport, UnitStats,
                            BattleAnimationType, DefendingResult, out _, TargetSquadBackground, out _, TargetSquadForeground));
                    }
                }
            }

            if (Map.IsClient)
            {
                if (ShowAnimation)
                {
                    Map.PushScreen(ListNextAnimationScreen[0]);
                    ListNextAnimationScreen.RemoveAt(0);
                    ListNextAnimationScreen.Add(new EndBattleAnimationScreen(Map, FinalActiveSquad, FinalActiveSquadSupport, FinalActivePlayerIndex,
                        FinalTargetSquad, FinalTargetSquadSupport, FinalTargetPlayerIndex, AttackingResult, DefendingResult));
                }
                else
                { 
                    NonDemoScreen NonDemoScreen = new NonDemoScreen(Map);
                    Map.PushScreen(NonDemoScreen);
                    NonDemoScreen.InitNonDemo(FinalActiveSquad, FinalAttack, FinalActiveSquadSupport, FinalActivePlayerIndex, AttackingResult, Map.BattleMenuOffenseFormationChoice,
                         FinalTargetSquad, FinalTargetSquadSupport, FinalTargetPlayerIndex, DefendingResult, Map.BattleMenuDefenseFormationChoice, IsActiveSquadOnRight);
                }
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

            if (FinalAttack.Pri != WeaponPrimaryProperty.PER)
            {
                if (FinalAttack.Parent != null && !FinalAttack.IsChargeable)
                {
                    FinalActiveSquad.CurrentLeader.UseChargeAttack();
                }
                else if (FinalAttack.Pri == WeaponPrimaryProperty.Dash)
                {
                    Map.MovementAnimation.Add(FinalActiveSquad, FinalActiveSquad.Position, GetDashPosition(FinalActiveSquad, FinalAttack, FinalTargetSquad));
                }

                if (!AttackingResult.ArrayResult[0].AttackMissed)
                {
                    HandleKnockback(FinalActivePlayerIndex, FinalActiveSquadIndex, FinalActiveSquad, FinalAttack, FinalTargetPlayerIndex, FinalTargetSquadIndex, FinalTargetSquad);
                }

                Map.FinalizeMovement(FinalActiveSquad, (int)Map.GetTerrain(FinalActiveSquad).MovementCost, ListMVHoverPoints);
                FinalActiveSquad.EndTurn();
            }

            if (Map.IsClient)
            {
                bool HasAfterAttack = false;
                ActionPanelDeathmatch AfterAttack = new ActionPanelMainMenu(Map, FinalActivePlayerIndex, FinalActiveSquadIndex);

                if (FinalActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Attack)
                {
                    HasAfterAttack = true;
                    AfterAttack.AddChoiceToCurrentPanel(new ActionPanelAttackPart1(Map, FinalActivePlayerIndex, FinalActiveSquadIndex, FinalActiveSquad.CanMove, new List<Vector3>()));
                }

                if (FinalActiveSquad.CurrentLeader.Boosts.PostAttackModifier.Move)
                {
                    HasAfterAttack = true;
                    Map.CursorPosition = FinalActiveSquad.Position;
                    AfterAttack.AddChoiceToCurrentPanel(new ActionPanelMovePart1(Map, FinalActivePlayerIndex, FinalActiveSquadIndex, Map.GetTerrain(FinalActiveSquad), ActiveSquad.Direction, Map.CameraPosition, true));
                }

                if (HasAfterAttack)
                {
                    AfterAttack.AddChoiceToCurrentPanel(new ActionPanelWait(Map, FinalActiveSquad, new List<Vector3>()));
                    ListActionMenuChoice.Add(AfterAttack);
                }
            }
        }

        public void InitPlayerAttack()
        {
            if (Map.IsClient)
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
            }

            InitPlayerBattle(true);
        }

        public void InitPlayerDefence()
        {
            if (Map.IsClient)
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
            }

            InitPlayerBattle(false);
        }

        private Vector3 GetDashPosition(Squad ActiveSquad, Attack FinalAttack, Squad TargetSquad)
        {
            Vector3 FinalPosition = ActiveSquad.Position;

            float DiffX = TargetSquad.X - ActiveSquad.X;
            float DiffY = TargetSquad.Y - ActiveSquad.Y;
            int MovementX = Math.Min(FinalAttack.DashMaxReach, (int)Math.Abs(DiffX) - 1);
            int MovementY = Math.Min(FinalAttack.DashMaxReach, (int)Math.Abs(DiffY) - 1);

            FinalPosition = new Vector3(ActiveSquad.X + MovementX * Math.Sign(DiffX), ActiveSquad.Y + MovementY * Math.Sign(DiffY), ActiveSquad.Z);
            ActiveSquad.SetPosition(FinalPosition);

            return FinalPosition;
        }

        private void HandleKnockback(int ActivePlayerIndex, int ActiveSquadIndex, Squad ActiveSquad, Attack FinalAttack, int TargetPlayerIndex, int TargetSquadIndex, Squad TargetSquad)
        {
            Vector3 Diff = TargetSquad.Position - ActiveSquad.Position;
            Diff.Normalize();

            if (FinalAttack.KnockbackAttributes.EnemyKnockback > 0)
            {
                TargetSquad.Speed += Diff * FinalAttack.KnockbackAttributes.EnemyKnockback;
                Map.ListActionMenuChoice.Add(new ActionPanelAutoMove(Map, TargetPlayerIndex, TargetSquadIndex, TargetSquad));
            }
            if (FinalAttack.KnockbackAttributes.SelfKnockback > 0)
            {
                ActiveSquad.Speed -= Diff * FinalAttack.KnockbackAttributes.SelfKnockback;
                Map.ListActionMenuChoice.Add(new ActionPanelAutoMove(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad));
            }
        }
    }
}
