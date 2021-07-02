using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHumanDefend : ActionPanelDeathmatch
    {
        private Squad ActiveSquad;
        private SupportSquadHolder ActiveSquadSupport;
        private int ActivePlayerIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;
        private int TargetPlayerIndex;

        public ActionPanelHumanDefend(DeathmatchMap Map, Squad ActiveSquad, SupportSquadHolder ActiveSquadSupport, int ActivePlayerIndex,
            Squad TargetSquad, SupportSquadHolder TargetSquadSupport, int TargetPlayerIndex)
            : base("Human Defend", Map, false)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.TargetSquad = TargetSquad;
            this.TargetSquadSupport = TargetSquadSupport;
            this.TargetPlayerIndex = TargetPlayerIndex;
        }

        public override void OnSelect()
        {
            Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;
            if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
            {
                Map.GetAttackChoice(ActiveSquad.CurrentLeader, ActiveSquad.Position);
                Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
            }

            Map.sndConfirm.Play();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (Map.BattleMenuStage)
            {
                #region Default

                case BattleMenuStages.Default:
                    if (InputHelper.InputLeftPressed())
                    {
                        Map.BattleMenuCursorIndex--;

                        if (Map.BattleMenuCursorIndex == BattleMenuChoices.Formation)
                        {//Can't pick Formation. (No wingmans or ALL attack)
                            if (TargetSquad.UnitsAliveInSquad == 1 || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL)
                                Map.BattleMenuCursorIndex = BattleMenuChoices.Action;
                        }
                        else if (Map.BattleMenuCursorIndex < BattleMenuChoices.Start)
                            Map.BattleMenuCursorIndex = BattleMenuChoices.Demo;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputRightPressed())
                    {
                        Map.BattleMenuCursorIndex++;

                        if (Map.BattleMenuCursorIndex == BattleMenuChoices.Formation)
                        {//Can't pick Formation. (No wingmans or ALL attack)
                            if (TargetSquad.UnitsAliveInSquad == 1 || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL)
                                Map.BattleMenuCursorIndex = BattleMenuChoices.Support;
                        }
                        else if (Map.BattleMenuCursorIndex > BattleMenuChoices.Demo)
                            Map.BattleMenuCursorIndex = BattleMenuChoices.Start;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputCommand2Pressed())
                    {
                        Constants.ShowAnimation = !Constants.ShowAnimation;

                        Map.sndSelection.Play();
                    }
                    if (InputHelper.InputConfirmPressed())
                    {
                        switch (Map.BattleMenuCursorIndex)
                        {
                            case BattleMenuChoices.Start:
                                Map.ComputeTargetPlayerOffense(ActiveSquad, ActiveSquadSupport, ActivePlayerIndex, TargetSquad, TargetSquadSupport, TargetPlayerIndex);
                                break;

                            case BattleMenuChoices.Action:
                                Map.BattleMenuCursorIndexSecond = 0;
                                if (TargetSquad.UnitsAliveInSquad == 1)
                                {
                                    Map.BattleMenuStage = BattleMenuStages.ChooseDefense;
                                    Map.BattleMenuCursorIndexSecond = (int)TargetSquad.CurrentLeader.BattleDefenseChoice;
                                }
                                else
                                {
                                    Map.BattleMenuStage = BattleMenuStages.ChooseSquadMember;//Choose squad member.
                                }
                                break;

                            case BattleMenuChoices.Formation:
                                if (Map.BattleMenuDefenseFormationChoice != FormationChoices.ALL)
                                {
                                    Map.BattleMenuCursorIndexSecond = (int)Map.BattleMenuDefenseFormationChoice;
                                    Map.BattleMenuStage = BattleMenuStages.ChooseFormation;
                                }
                                break;

                            case BattleMenuChoices.Support:
                                Map.BattleMenuCursorIndexSecond = 0;
                                Map.BattleMenuStage = BattleMenuStages.ChooseSupport;
                                break;

                            case BattleMenuChoices.Demo:
                                Constants.ShowAnimation = !Constants.ShowAnimation;
                                break;
                        }

                        Map.sndConfirm.Play();
                    }
                    else if (InputHelper.InputCancelPressed() && Map.ListPlayer[ActivePlayerIndex].IsHuman)//Can't cancel out of AI attacks.
                    {
                        CancelPanel();
                    }
                    break;

                #endregion

                #region Choose defense

                case DeathmatchMap.BattleMenuStages.ChooseDefense:
                    if (InputHelper.InputUpPressed())
                    {
                        Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < 2 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (Map.BattleMenuCursorIndexSecond == 0)
                        {
                            TargetSquad.CurrentLeader.UpdateNonMAPAttacks(TargetSquad.Position, ActiveSquad.Position, ActiveSquad.ArrayMapSize, ActiveSquad.CurrentMovement, true);
                            Map.WeaponIndexOld = TargetSquad.CurrentLeader.AttackIndex;
                            TargetSquad.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
                            Map.BattleMenuStage = BattleMenuStages.ChooseAttack;
                        }
                        else if (Map.BattleMenuCursorIndexSecond == 1)
                        {
                            TargetSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                            Map.BattleMenuStage = BattleMenuStages.Default;
                            ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad, TargetSquad.CurrentLeader, TargetSquad, Unit.BattleDefenseChoices.Defend).ToString() + "%";
                        }
                        else if (Map.BattleMenuCursorIndexSecond == 2)
                        {
                            TargetSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                            Map.BattleMenuStage = BattleMenuStages.Default;
                            ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad, TargetSquad.CurrentLeader, TargetSquad, Unit.BattleDefenseChoices.Evade).ToString() + "%";
                        }

                        Map.sndConfirm.Play();
                    }
                    break;

                #endregion

                #region Choose formation

                case BattleMenuStages.ChooseFormation:
                    if (InputHelper.InputUpPressed())
                    {
                        Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < 1 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (Map.BattleMenuCursorIndexSecond == 0)
                            Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;
                        else if (Map.BattleMenuCursorIndexSecond == 1)
                            Map.BattleMenuDefenseFormationChoice = FormationChoices.Spread;

                        Map.UpdateWingmansSelection(TargetSquad, ActiveSquad, Map.BattleMenuDefenseFormationChoice);

                        Map.BattleMenuStage = BattleMenuStages.Default;
                        Map.sndConfirm.Play();
                    }
                    break;

                #endregion

                #region Choose attack

                case BattleMenuStages.ChooseAttack://Attack selection.
                                                   //Move the cursor.
                    if (InputHelper.InputUpPressed())
                    {
                        --TargetSquad.CurrentLeader.AttackIndex;
                        if (TargetSquad.CurrentLeader.AttackIndex < 0)
                            TargetSquad.CurrentLeader.AttackIndex = TargetSquad.CurrentLeader.ListAttack.Count - 1;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++TargetSquad.CurrentLeader.AttackIndex;
                        if (TargetSquad.CurrentLeader.AttackIndex >= TargetSquad.CurrentLeader.ListAttack.Count)
                            TargetSquad.CurrentLeader.AttackIndex = 0;

                        Map.sndSelection.Play();
                    }
                    //Exit the weapon panel.
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (TargetSquad.CurrentLeader.CurrentAttack.CanAttack)
                        {
                            TargetSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                            if (TargetSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
                            {
                                Map.BattleMenuDefenseFormationChoice = FormationChoices.ALL;

                                if (TargetSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack &&
                                    TargetSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
                                {
                                    Map.BattleMenuDefenseFormationChoice = FormationChoices.ALL;

                                    if (TargetSquad.CurrentWingmanA != null)
                                        TargetSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;

                                    if (ActiveSquad.CurrentWingmanA != null)
                                    {
                                        TargetSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad,
                                            ActiveSquad.CurrentWingmanA, ActiveSquad, ActiveSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                                    }
                                    else
                                        TargetSquad.CurrentLeader.MAPAttackAccuracyA = "0%";

                                    if (TargetSquad.CurrentWingmanB != null)
                                        TargetSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;

                                    if (ActiveSquad.CurrentWingmanB != null)
                                    {
                                        TargetSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad,
                                            ActiveSquad.CurrentWingmanB, ActiveSquad, ActiveSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                                    }
                                    else
                                        TargetSquad.CurrentLeader.MAPAttackAccuracyB = "0%";
                                }
                            }
                            else
                            {
                                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL)
                                    Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;

                                Map.UpdateWingmansSelection(TargetSquad, ActiveSquad, Map.BattleMenuOffenseFormationChoice);
                            }
                            Map.BattleMenuStage = BattleMenuStages.Default;

                            TargetSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                            //Simulate offense reaction.
                            PrepareAttackSquadForBattle(Map, ActiveSquad, TargetSquad);
                            //The Defense Leader now attack.
                            TargetSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad,
                                ActiveSquad.CurrentLeader, ActiveSquad, Unit.BattleDefenseChoices.Attack).ToString() + "%";

                            Map.sndConfirm.Play();
                        }
                        else
                        {
                            Map.sndDeny.Play();
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        Map.BattleMenuStage = BattleMenuStages.Default;
                        Map.sndCancel.Play();
                    }
                    break;

                #endregion

                #region Choose squad member

                case BattleMenuStages.ChooseSquadMember:

                    if (InputHelper.InputDownPressed())
                    {
                        Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < TargetSquad.UnitsAliveInSquad - 1 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                        Map.sndSelection.Play();
                    }
                    if (InputHelper.InputConfirmPressed())
                    {
                        if (Map.BattleMenuCursorIndexSecond == 0)
                            Map.BattleMenuCursorIndexThird = (int)TargetSquad.CurrentLeader.BattleDefenseChoice;
                        else if (Map.BattleMenuCursorIndexSecond == 1)
                            Map.BattleMenuCursorIndexThird = (int)TargetSquad.CurrentWingmanA.BattleDefenseChoice;
                        else if (Map.BattleMenuCursorIndexSecond == 2)
                            Map.BattleMenuCursorIndexThird = (int)TargetSquad.CurrentWingmanB.BattleDefenseChoice;

                        Map.BattleMenuStage = BattleMenuStages.ChooseSquadMemberDefense;
                        Map.sndConfirm.Play();
                    }
                    break;

                #endregion

                #region Choose squad member defense

                case BattleMenuStages.ChooseSquadMemberDefense:

                    if (InputHelper.InputUpPressed())
                    {
                        Map.BattleMenuCursorIndexThird++;
                        if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird >= 3 &&
                            (!TargetSquad[Map.BattleMenuCursorIndexSecond].CanAttack || TargetSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL))
                            Map.BattleMenuCursorIndexThird = 1;
                        else if (Map.BattleMenuCursorIndexThird >= 3)
                            Map.BattleMenuCursorIndexThird = 0;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        Map.BattleMenuCursorIndexThird--;
                        if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird <= 0 &&
                            (!TargetSquad[Map.BattleMenuCursorIndexSecond].CanAttack || TargetSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL))
                            Map.BattleMenuCursorIndexThird = 2;
                        else if (Map.BattleMenuCursorIndexThird < 0)
                            Map.BattleMenuCursorIndexThird = 2;

                        Map.sndSelection.Play();
                    }
                    if (InputHelper.InputConfirmPressed())
                    {
                        if ((Unit.BattleDefenseChoices)Map.BattleMenuCursorIndexThird == Unit.BattleDefenseChoices.Attack)
                        {
                            if (Map.BattleMenuCursorIndexSecond == 0)
                            {
                                TargetSquad.CurrentLeader.UpdateNonMAPAttacks(TargetSquad.Position, ActiveSquad.Position, ActiveSquad.ArrayMapSize, ActiveSquad.CurrentMovement, TargetSquad.CanMove);

                                Map.WeaponIndexOld = TargetSquad.CurrentLeader.AttackIndex;
                                TargetSquad.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
                                Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.ChooseAttack;
                            }
                            else
                            {
                                TargetSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                                //Simulate offense reaction.
                                DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, TargetSquad);

                                if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread)
                                {
                                    if (Map.BattleMenuCursorIndexSecond == 1)
                                    {
                                        TargetSquad.CurrentWingmanA.AttackIndex = TargetSquad.CurrentWingmanA.PLAAttack;
                                        TargetSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanA, TargetSquad,
                                           ActiveSquad.CurrentWingmanA, ActiveSquad, ActiveSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                                    }
                                    else if (Map.BattleMenuCursorIndexSecond == 2)
                                    {
                                        TargetSquad.CurrentWingmanB.AttackIndex = TargetSquad.CurrentWingmanB.PLAAttack;
                                        TargetSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanB, TargetSquad,
                                            ActiveSquad.CurrentWingmanB, ActiveSquad, ActiveSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                                    }
                                }
                                else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                                {
                                    if (Map.BattleMenuCursorIndexSecond == 1)
                                    {
                                        TargetSquad.CurrentWingmanA.AttackIndex = TargetSquad.CurrentWingmanA.PLAAttack;
                                        TargetSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanA, TargetSquad,
                                            ActiveSquad.CurrentLeader, ActiveSquad, ActiveSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                                    }
                                    else if (Map.BattleMenuCursorIndexSecond == 2)
                                    {
                                        TargetSquad.CurrentWingmanB.AttackIndex = TargetSquad.CurrentWingmanB.PLAAttack;
                                        TargetSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanB, TargetSquad,
                                            ActiveSquad.CurrentLeader, ActiveSquad, ActiveSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                                    }
                                }
                                Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                            }
                        }
                        else if (Map.BattleMenuCursorIndexSecond == 0)
                        {
                            TargetSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)Map.BattleMenuCursorIndexThird;
                            Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;

                            //Simulate offense reaction.
                            DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, TargetSquad);

                            if (TargetSquad.CurrentWingmanA != null && TargetSquad.CurrentWingmanA.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                                TargetSquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                            if (TargetSquad.CurrentWingmanB != null && TargetSquad.CurrentWingmanB.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                                TargetSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;

                            Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                        }
                        else
                        {
                            TargetSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice = (Unit.BattleDefenseChoices)Map.BattleMenuCursorIndexThird;
                            //Simulate offense reaction.
                            DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, TargetSquad);
                            Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                        }

                        Map.sndConfirm.Play();
                    }
                    break;

                #endregion

                #region Choose support

                case BattleMenuStages.ChooseSupport:
                    if (InputHelper.InputUpPressed())
                    {
                        --TargetSquadSupport.ActiveSquadSupportIndex;
                        if (TargetSquadSupport.ActiveSquadSupportIndex < -1)
                            TargetSquadSupport.ActiveSquadSupportIndex = TargetSquadSupport.Count - 1;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++TargetSquadSupport.ActiveSquadSupportIndex;
                        if (TargetSquadSupport.ActiveSquadSupportIndex >= TargetSquadSupport.Count)
                            TargetSquadSupport.ActiveSquadSupportIndex = -1;

                        Map.sndSelection.Play();
                    }
                    else if (InputHelper.InputConfirmPressed())
                    {
                        Map.BattleMenuStage = BattleMenuStages.Default;
                        Map.sndConfirm.Play();
                    }
                    break;

                    #endregion
            }
        }

        protected override void OnCancelPanel()
        {
            base.OnCancelPanel();

            switch (Map.BattleMenuStage)
            {
                case BattleMenuStages.ChooseDefense:
                    Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                    Map.sndCancel.Play();
                    break;

                case BattleMenuStages.ChooseFormation:
                    Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                    Map.sndCancel.Play();
                    break;

                case BattleMenuStages.ChooseAttack://Attack selection.
                    TargetSquad.CurrentLeader.AttackIndex = Map.WeaponIndexOld;

                    if (TargetSquad.UnitsAliveInSquad == 1)
                        Map.BattleMenuStage = BattleMenuStages.ChooseDefense;
                    else
                        Map.BattleMenuStage = BattleMenuStages.ChooseSquadMemberDefense;

                    Map.sndCancel.Play();
                    break;

                case BattleMenuStages.ChooseSquadMember:
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    Map.sndCancel.Play();
                    break;

                case BattleMenuStages.ChooseSquadMemberDefense:
                    Map.BattleMenuStage = BattleMenuStages.ChooseSquadMember;

                    Map.sndCancel.Play();
                    break;

                case BattleMenuStages.ChooseSupport:
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    Map.sndConfirm.Play();
                    break;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            Map.BattleSumaryDefenceDraw(g, ActiveSquad, ActiveSquadSupport, TargetSquad, TargetSquadSupport);
        }
    }
}
