﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHumanAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "HumanAttack";

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private SupportSquadHolder ActiveSquadSupport;

        private int TargetPlayerIndex;
        private int TargetSquadIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;

        int ActiveSquadSupportIndexOld;//Index of the support squad used for reset.

        public ActionPanelHumanAttack(DeathmatchMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelHumanAttack(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport,
             int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ActiveSquadSupport = ActiveSquadSupport;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;
            this.TargetSquadSupport = TargetSquadSupport;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
        }

        public override void OnSelect()
        {
            //Attack Support
            ActiveSquadSupport.PrepareAttackSupport(Map, Map.ActivePlayerIndex, ActiveSquad, TargetPlayerIndex, TargetSquadIndex);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (Map.BattleMenuStage)
            {
                case BattleMenuStages.Default:
                    UpdateBaseMenu(gameTime);
                    break;

                case BattleMenuStages.ChooseAttack:
                    UpdateAttackSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseFormation:
                    UpdateFormationSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseSquadMember:
                    UpdateSquadMemberSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseSquadMemberDefense:
                    UpdateSquadMemberDefenseSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseSupport:
                    UpdateSupportSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseSupportAttack:
                    UpdateSupportAttackSelection(gameTime);
                    break;
            }
        }

        private void UpdateBaseMenu(GameTime gameTime)
        {
            if (ActiveInputManager.InputLeftPressed())
            {
                Map.BattleMenuCursorIndex--;

                if (Map.BattleMenuCursorIndex == BattleMenuChoices.Support)
                {//Can't pick Support. (No Support Squads nearby)
                    if (ActiveSquadSupport.Count <= 0)
                        Map.BattleMenuCursorIndex = BattleMenuChoices.Formation;
                }
                if (Map.BattleMenuCursorIndex == BattleMenuChoices.Formation)
                {//Can't pick Formation. (No wingmans or ALL attack)
                    if (ActiveSquad.UnitsAliveInSquad == 1 || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL)
                        Map.BattleMenuCursorIndex = BattleMenuChoices.Action;
                }
                if (Map.BattleMenuCursorIndex < BattleMenuChoices.Start)
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Demo;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputRightPressed())
            {
                Map.BattleMenuCursorIndex++;

                if (Map.BattleMenuCursorIndex == BattleMenuChoices.Formation)
                {//Can't pick Formation. (No wingmans or ALL attack)
                    if (ActiveSquad.UnitsAliveInSquad == 1 || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL)
                        Map.BattleMenuCursorIndex = BattleMenuChoices.Support;
                }
                if (Map.BattleMenuCursorIndex == BattleMenuChoices.Support)
                {//Can't pick Support. (No Support Squads nearby)
                    if (ActiveSquadSupport.Count <= 0)
                        Map.BattleMenuCursorIndex = BattleMenuChoices.Demo;
                }
                if (Map.BattleMenuCursorIndex > BattleMenuChoices.Demo)
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Start;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                if (ActiveInputManager.IsInZone(0, Constants.Height - 30, 125, Constants.Height))
                {
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Start;
                }
                else if (ActiveInputManager.IsInZone(125, Constants.Height - 30, 255, Constants.Height))
                {
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Action;
                }
                else if (ActiveInputManager.IsInZone(255, Constants.Height - 30, 385, Constants.Height)
                    && (ActiveSquad.UnitsAliveInSquad > 1 && Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL))
                {
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Formation;
                }
                else if (ActiveInputManager.IsInZone(385, Constants.Height - 30, 510, Constants.Height) && ActiveSquadSupport.Count > 0)
                {
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Support;
                }
                else if (ActiveInputManager.IsInZone(510, Constants.Height - 30, 635, Constants.Height))
                {
                    Map.BattleMenuCursorIndex = BattleMenuChoices.Demo;
                }
            }
            else if (ActiveInputManager.InputCommand1Pressed())
            {
                Map.SpiritMenu.InitSpiritScreen(ActiveSquad);

                Map.BattleMenuStage = BattleMenuStages.Default;
            }
            else if (ActiveInputManager.InputCommand2Pressed())
            {
                //Constants.ShowAnimation = !Constants.ShowAnimation;

                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                switch (Map.BattleMenuCursorIndex)
                {
                    //Begin attack.
                    case BattleMenuChoices.Start:
                        StartBattle();
                        break;

                    case BattleMenuChoices.Action:
                        if (ActiveSquad.UnitsAliveInSquad == 1)
                        {
                            ActiveSquad.CurrentLeader.UpdateNonMAPAttacks(ActiveSquad.Position, TargetSquad.Position, TargetSquad.ArrayMapSize, TargetSquad.CurrentMovement, ActiveSquad.CanMove);

                            ActiveSquad.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
                            Map.BattleMenuStage = BattleMenuStages.ChooseAttack;
                        }
                        else
                        {
                            Map.BattleMenuStage = BattleMenuStages.ChooseSquadMember;//Choose squad member.
                            Map.BattleMenuCursorIndexSecond = 1;//Leader is attacking, can't put him on defend or evade.
                        }
                        break;

                    case BattleMenuChoices.Formation:
                        Map.BattleMenuCursorIndexSecond = (int)Map.BattleMenuOffenseFormationChoice;
                        Map.BattleMenuStage = BattleMenuStages.ChooseFormation;
                        break;

                    case BattleMenuChoices.Support:
                        Map.BattleMenuCursorIndexSecond = 0;
                        ActiveSquadSupportIndexOld = ActiveSquadSupport.ActiveSquadSupportIndex;
                        Map.BattleMenuStage = BattleMenuStages.ChooseSupport;
                        break;

                    case BattleMenuChoices.Demo:
                        Constants.ShowAnimation = !Constants.ShowAnimation;
                        break;
                }

                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                CancelPanel();
            }
        }

        private void UpdateAttackSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                --ActiveSquad.CurrentLeader.AttackIndex;
                if (ActiveSquad.CurrentLeader.AttackIndex < 0)
                    ActiveSquad.CurrentLeader.AttackIndex = ActiveSquad.CurrentLeader.ListAttack.Count - 1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++ActiveSquad.CurrentLeader.AttackIndex;
                if (ActiveSquad.CurrentLeader.AttackIndex >= ActiveSquad.CurrentLeader.ListAttack.Count)
                    ActiveSquad.CurrentLeader.AttackIndex = 0;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                int YStep = 25;

                for (int A = 0; A < ActiveSquad.CurrentLeader.ListAttack.Count; ++A)
                {
                    int YStart = 122 + A * YStep;
                    if (ActiveInputManager.IsInZone(0, YStart, Constants.Width, YStart + YStep))
                    {
                        ActiveSquad.CurrentLeader.AttackIndex = A;
                        break;
                    }
                }
            }
            //Exit the weapon panel.
            else if (ActiveInputManager.InputConfirmPressed())
            {
                if (ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                {
                    if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
                        Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;

                    Map.BattleMenuStage = BattleMenuStages.Default;
                    ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad,
                        TargetSquad.CurrentLeader, TargetSquad, TargetSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                    if (!Map.ListPlayer[TargetPlayerIndex].IsPlayerControlled)
                    {
                        PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, TargetPlayerIndex, TargetSquadIndex);
                    }

                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndCancel.Play();
            }
        }

        private void UpdateFormationSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < 1 ? 1 : 0;
                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (Map.BattleMenuCursorIndexSecond == 0)
                    Map.BattleMenuOffenseFormationChoice = FormationChoices.Focused;
                else if (Map.BattleMenuCursorIndexSecond == 1)
                    Map.BattleMenuOffenseFormationChoice = FormationChoices.Spread;

                Map.UpdateWingmansSelection(ActiveSquad, TargetSquad, Map.BattleMenuOffenseFormationChoice);

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

                //Simulate defense reaction.
                PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, TargetPlayerIndex, TargetSquadIndex);
                PrepareAttackSquadForBattle(Map, ActiveSquad, TargetSquad);

                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndCancel.Play();
            }
        }

        private void UpdateSquadMemberSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < ActiveSquad.UnitsAliveInSquad - 1 ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 1 ? 1 : 0;
                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (Map.BattleMenuCursorIndexSecond == 1)
                    Map.BattleMenuCursorIndexThird = (int)ActiveSquad.CurrentWingmanA.BattleDefenseChoice;
                else if (Map.BattleMenuCursorIndexSecond == 2)
                    Map.BattleMenuCursorIndexThird = (int)ActiveSquad.CurrentWingmanB.BattleDefenseChoice;

                Map.BattleMenuStage = BattleMenuStages.ChooseSquadMemberDefense;
                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndCancel.Play();
            }
        }

        private void UpdateSquadMemberDefenseSelection(GameTime gameTime)
        {

            if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexThird++;
                if (Map.BattleMenuCursorIndexThird >= 3)
                {
                    if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird >= 3 &&
                        (!ActiveSquad[Map.BattleMenuCursorIndexSecond].CanAttack || ActiveSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL))
                        Map.BattleMenuCursorIndexThird = 1;
                    else
                        Map.BattleMenuCursorIndexThird = 0;
                }

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexThird--;
                if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird <= 0 &&
                    (!ActiveSquad[Map.BattleMenuCursorIndexSecond].CanAttack || ActiveSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuOffenseFormationChoice == FormationChoices.ALL))
                    Map.BattleMenuCursorIndexThird = 2;
                else if (Map.BattleMenuCursorIndexThird < 0)
                    Map.BattleMenuCursorIndexThird = 2;

                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                ActiveSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice = (Unit.BattleDefenseChoices)Map.BattleMenuCursorIndexThird;

                if (ActiveSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    //Simulate defense reaction.
                    PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, TargetPlayerIndex, TargetSquadIndex);

                    if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread)
                    {
                        if (Map.BattleMenuCursorIndexSecond == 1)
                        {
                            ActiveSquad.CurrentWingmanA.AttackIndex = ActiveSquad.CurrentWingmanA.PLAAttack;
                            ActiveSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad,
                                TargetSquad.CurrentWingmanA, TargetSquad, TargetSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                        }
                        else if (Map.BattleMenuCursorIndexSecond == 2)
                        {
                            ActiveSquad.CurrentWingmanB.AttackIndex = ActiveSquad.CurrentWingmanB.PLAAttack;
                            ActiveSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad,
                                TargetSquad.CurrentWingmanB, TargetSquad, TargetSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                        }
                    }
                    else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                    {
                        if (Map.BattleMenuCursorIndexSecond == 1)
                        {
                            ActiveSquad.CurrentWingmanA.AttackIndex = ActiveSquad.CurrentWingmanA.PLAAttack;
                            ActiveSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentWingmanA, ActiveSquad,
                                TargetSquad.CurrentLeader, TargetSquad, TargetSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                        }
                        else if (Map.BattleMenuCursorIndexSecond == 2)
                        {
                            ActiveSquad.CurrentWingmanB.AttackIndex = ActiveSquad.CurrentWingmanB.PLAAttack;
                            ActiveSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentWingmanB, ActiveSquad,
                                TargetSquad.CurrentLeader, TargetSquad, TargetSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                        }
                    }
                }

                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.ChooseSquadMember;
                Map.sndCancel.Play();
            }
        }

        private void UpdateSupportSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                --ActiveSquadSupport.ActiveSquadSupportIndex;
                if (ActiveSquadSupport.ActiveSquadSupportIndex < -1)
                    ActiveSquadSupport.ActiveSquadSupportIndex = ActiveSquadSupport.Count - 1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++ActiveSquadSupport.ActiveSquadSupportIndex;
                if (ActiveSquadSupport.ActiveSquadSupportIndex >= ActiveSquadSupport.Count)
                    ActiveSquadSupport.ActiveSquadSupportIndex = -1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                if (ActiveSquadSupport.ActiveSquadSupportIndex >= 0)
                {
                    Map.BattleMenuStage = BattleMenuStages.ChooseSupportAttack;
                    //Update weapons so you know which one is in attack range.

                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.DisableAllAttacks();
                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.UpdateAllAttacks(
                        ActiveSquadSupport.ActiveSquadSupport.Position,
                        TargetSquad.Position, TargetSquad.ArrayMapSize, TargetSquad.CurrentMovement,
                        ActiveSquadSupport.ActiveSquadSupport.CanMove);

                    Map.WeaponIndexOld = ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex;
                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
                }
                else
                {
                    Map.BattleMenuStage = BattleMenuStages.Default;
                }
                Map.sndConfirm.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndCancel.Play();
            }
        }

        private void UpdateSupportAttackSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                --ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex;
                if (ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex < 0)
                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex = ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.ListAttack.Count - 1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex;
                if (ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex >= ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.ListAttack.Count)
                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.AttackIndex = 0;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                if (ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack.CanAttack)
                {
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    Map.sndConfirm.Play();
                }
                else
                    Map.sndDeny.Play();
            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                ActiveSquadSupport.ActiveSquadSupportIndex = ActiveSquadSupportIndexOld;
                Map.BattleMenuStage = BattleMenuStages.ChooseSupport;
                Map.sndCancel.Play();
            }
        }

        private void StartBattle()
        {
            AddToPanelListAndSelect(new ActionPanelStartBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, false));
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)BR.ReadByte();
            ActiveSquad.CurrentLeader.AttackIndex = BR.ReadInt32();

            TargetPlayerIndex = BR.ReadInt32();
            TargetSquadIndex = BR.ReadInt32();
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            TargetSquadSupport = new SupportSquadHolder();
            TargetSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)BR.ReadByte();
            TargetSquad.CurrentLeader.AttackIndex = BR.ReadInt32();

            Map.BattleMenuStage = (BattleMenuStages)BR.ReadByte();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendByte((byte)ActiveSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendInt32(ActiveSquad.CurrentLeader.AttackIndex);

            BW.AppendInt32(TargetPlayerIndex);
            BW.AppendInt32(TargetSquadIndex);
            BW.AppendByte((byte)TargetSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendInt32(TargetSquad.CurrentLeader.AttackIndex);

            BW.AppendByte((byte)Map.BattleMenuStage);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelHumanAttack(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Map.BattleSumaryAttackDraw(g, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport);
        }
    }
}
