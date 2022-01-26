using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;
using static ProjectEternity.GameScreens.DeathmatchMapScreen.DeathmatchMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelHumanDefend : ActionPanelDeathmatch
    {
        private const string PanelName = "HumanDefend";

        private int AttackIndex;
        private Attack AttackOld;

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private SupportSquadHolder ActiveSquadSupport;

        private int TargetPlayerIndex;
        private int TargetSquadIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;
        List<Attack> ListAttackTargetSquad;

        public ActionPanelHumanDefend(DeathmatchMap Map)
            : base(PanelName, Map, Map.ListPlayer[Map.TargetPlayerIndex].InputManager, false)
        {
        }

        public ActionPanelHumanDefend(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, SupportSquadHolder ActiveSquadSupport,
             int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
            : base(PanelName, Map, Map.ListPlayer[Map.TargetPlayerIndex].InputManager, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ActiveSquadSupport = ActiveSquadSupport;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;
            this.TargetSquadSupport = TargetSquadSupport;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
            ListAttackTargetSquad = TargetSquad.CurrentLeader.ListAttack;
        }

        public override void OnSelect()
        {
            Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;
            if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.ALL)
            {
                Map.GetAttackChoice(ActiveSquad.CurrentLeader, ActiveSquad.Position);
                Map.BattleMenuOffenseFormationChoice = FormationChoices.ALL;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            switch (Map.BattleMenuStage)
            {
                case BattleMenuStages.Default:
                    UpdateBaseMenu(gameTime);
                    break;

                case DeathmatchMap.BattleMenuStages.ChooseDefense:
                    UpdateDefenseSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseFormation:
                    UpdateFormationSelection(gameTime);
                    break;

                case BattleMenuStages.ChooseAttack:
                    UpdateAttackSelection(gameTime);
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
            }
        }

        private void UpdateBaseMenu(GameTime gameTime)
        {
            if (ActiveInputManager.InputLeftPressed())
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
            else if (ActiveInputManager.InputRightPressed())
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
            else if (ActiveInputManager.InputCommand2Pressed())
            {
                Constants.ShowAnimation = !Constants.ShowAnimation;

                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                switch (Map.BattleMenuCursorIndex)
                {
                    case BattleMenuChoices.Start:
                        Map.ComputeTargetPlayerOffense(ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport);
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
            else if (ActiveInputManager.InputCancelPressed() && Map.ListPlayer[ActivePlayerIndex].IsPlayerControlled)//Can't cancel out of AI attacks.
            {
                CancelPanel();
            }
        }

        private void UpdateDefenseSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < 2 ? 1 : 0;
                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (Map.BattleMenuCursorIndexSecond == 0)
                {
                    TargetSquad.CurrentLeader.UpdateNonMAPAttacks(TargetSquad.Position, ActiveSquad.Position, ActiveSquad.ArrayMapSize, ActiveSquad.CurrentMovement, true);
                    AttackOld = TargetSquad.CurrentLeader.CurrentAttack;
                    TargetSquad.CurrentLeader.CurrentAttack = ListAttackTargetSquad[0];//Make sure you select the first weapon.
                    Map.BattleMenuStage = BattleMenuStages.ChooseAttack;
                }
                else if (Map.BattleMenuCursorIndexSecond == 1)
                {
                    TargetSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad, TargetSquad.CurrentLeader, TargetSquad, Unit.BattleDefenseChoices.Defend).ToString() + "%";
                }
                else if (Map.BattleMenuCursorIndexSecond == 2)
                {
                    TargetSquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad, TargetSquad.CurrentLeader, TargetSquad, Unit.BattleDefenseChoices.Evade).ToString() + "%";
                }

                Map.sndConfirm.Play();
            }
        }

        private void UpdateFormationSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < 1 ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if (Map.BattleMenuCursorIndexSecond == 0)
                    Map.BattleMenuDefenseFormationChoice = FormationChoices.Focused;
                else if (Map.BattleMenuCursorIndexSecond == 1)
                    Map.BattleMenuDefenseFormationChoice = FormationChoices.Spread;

                Map.UpdateWingmansSelection(TargetSquad, ActiveSquad, Map.BattleMenuDefenseFormationChoice);

                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndConfirm.Play();
            }
        }

        private void UpdateAttackSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                --AttackIndex;
                if (AttackIndex < 0)
                    AttackIndex = ListAttackTargetSquad.Count - 1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++AttackIndex;
                if (AttackIndex >= ListAttackTargetSquad.Count)
                    AttackIndex = 0;

                Map.sndSelection.Play();
            }
            //Exit the weapon panel.
            if (ActiveInputManager.InputConfirmPressed())
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
                                TargetSquad.CurrentLeader.MAPAttackAccuracyA = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.CurrentAttack, TargetSquad,
                                    ActiveSquad.CurrentWingmanA, ActiveSquad, ActiveSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                            }
                            else
                                TargetSquad.CurrentLeader.MAPAttackAccuracyA = "0%";

                            if (TargetSquad.CurrentWingmanB != null)
                                TargetSquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;

                            if (ActiveSquad.CurrentWingmanB != null)
                            {
                                TargetSquad.CurrentLeader.MAPAttackAccuracyB = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.CurrentAttack, TargetSquad,
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
                    PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);
                    //The Defense Leader now attack.
                    TargetSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.CurrentAttack, TargetSquad,
                        ActiveSquad.CurrentLeader, ActiveSquad, Unit.BattleDefenseChoices.Attack).ToString() + "%";

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

        private void UpdateSquadMemberSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexSecond += Map.BattleMenuCursorIndexSecond < TargetSquad.UnitsAliveInSquad - 1 ? 1 : 0;
                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexSecond -= Map.BattleMenuCursorIndexSecond > 0 ? 1 : 0;
                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
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
        }

        private void UpdateSquadMemberDefenseSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                Map.BattleMenuCursorIndexThird++;
                if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird >= 3 &&
                    (!TargetSquad[Map.BattleMenuCursorIndexSecond].CanAttack || TargetSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL))
                    Map.BattleMenuCursorIndexThird = 1;
                else if (Map.BattleMenuCursorIndexThird >= 3)
                    Map.BattleMenuCursorIndexThird = 0;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                Map.BattleMenuCursorIndexThird--;
                if (Map.BattleMenuCursorIndexSecond > 0 && Map.BattleMenuCursorIndexThird <= 0 &&
                    (!TargetSquad[Map.BattleMenuCursorIndexSecond].CanAttack || TargetSquad.CurrentLeader.BattleDefenseChoice != Unit.BattleDefenseChoices.Attack || Map.BattleMenuDefenseFormationChoice == FormationChoices.ALL))
                    Map.BattleMenuCursorIndexThird = 2;
                else if (Map.BattleMenuCursorIndexThird < 0)
                    Map.BattleMenuCursorIndexThird = 2;

                Map.sndSelection.Play();
            }
            if (ActiveInputManager.InputConfirmPressed())
            {
                if ((Unit.BattleDefenseChoices)Map.BattleMenuCursorIndexThird == Unit.BattleDefenseChoices.Attack)
                {
                    if (Map.BattleMenuCursorIndexSecond == 0)
                    {
                        TargetSquad.CurrentLeader.UpdateNonMAPAttacks(TargetSquad.Position, ActiveSquad.Position, ActiveSquad.ArrayMapSize, ActiveSquad.CurrentMovement, TargetSquad.CanMove);

                        AttackOld = TargetSquad.CurrentLeader.CurrentAttack;
                        TargetSquad.CurrentLeader.CurrentAttack = ListAttackTargetSquad[0];//Make sure you select the first weapon.
                        Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.ChooseAttack;
                    }
                    else
                    {
                        TargetSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;

                        //Simulate offense reaction.
                        DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);

                        if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Spread)
                        {
                            if (Map.BattleMenuCursorIndexSecond == 1)
                            {
                                TargetSquad.CurrentWingmanA.CurrentAttack = TargetSquad.CurrentWingmanA.PLAAttack;
                                TargetSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanA, TargetSquad.CurrentWingmanA.CurrentAttack, TargetSquad,
                                   ActiveSquad.CurrentWingmanA, ActiveSquad, ActiveSquad.CurrentWingmanA.BattleDefenseChoice).ToString() + "%";
                            }
                            else if (Map.BattleMenuCursorIndexSecond == 2)
                            {
                                TargetSquad.CurrentWingmanB.CurrentAttack = TargetSquad.CurrentWingmanB.PLAAttack;
                                TargetSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanB, TargetSquad.CurrentWingmanB.CurrentAttack, TargetSquad,
                                    ActiveSquad.CurrentWingmanB, ActiveSquad, ActiveSquad.CurrentWingmanB.BattleDefenseChoice).ToString() + "%";
                            }
                        }
                        else if (Map.BattleMenuDefenseFormationChoice == FormationChoices.Focused)
                        {
                            if (Map.BattleMenuCursorIndexSecond == 1)
                            {
                                TargetSquad.CurrentWingmanA.CurrentAttack = TargetSquad.CurrentWingmanA.PLAAttack;
                                TargetSquad.CurrentWingmanA.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanA, TargetSquad.CurrentWingmanA.CurrentAttack, TargetSquad,
                                    ActiveSquad.CurrentLeader, ActiveSquad, ActiveSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                            }
                            else if (Map.BattleMenuCursorIndexSecond == 2)
                            {
                                TargetSquad.CurrentWingmanB.CurrentAttack = TargetSquad.CurrentWingmanB.PLAAttack;
                                TargetSquad.CurrentWingmanB.AttackAccuracy = Map.CalculateHitRate(TargetSquad.CurrentWingmanB, TargetSquad.CurrentWingmanB.CurrentAttack, TargetSquad,
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
                    DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);

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
                    DeathmatchMap.PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);
                    Map.BattleMenuStage = DeathmatchMap.BattleMenuStages.Default;
                }

                Map.sndConfirm.Play();
            }
        }

        private void UpdateSupportSelection(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                --TargetSquadSupport.ActiveSquadSupportIndex;
                if (TargetSquadSupport.ActiveSquadSupportIndex < -1)
                    TargetSquadSupport.ActiveSquadSupportIndex = TargetSquadSupport.Count - 1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++TargetSquadSupport.ActiveSquadSupportIndex;
                if (TargetSquadSupport.ActiveSquadSupportIndex >= TargetSquadSupport.Count)
                    TargetSquadSupport.ActiveSquadSupportIndex = -1;

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                Map.BattleMenuStage = BattleMenuStages.Default;
                Map.sndConfirm.Play();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            if (Map.IsActivePlayerLocal(TargetPlayerIndex))
            {
                Update(gameTime);
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

            ListAttackTargetSquad = TargetSquad.CurrentLeader.ListAttack;
            if (!string.IsNullOrEmpty(TargetSquadAttackName))
            {
                foreach (Attack ActiveAttack in ListAttackTargetSquad)
                {
                    if (ActiveAttack.ItemName == TargetSquadAttackName)
                    {
                        TargetSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            Map.BattleMenuStage = (BattleMenuStages)BR.ReadByte();
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

            BW.AppendByte((byte)Map.BattleMenuStage);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelHumanDefend(Map);
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
                    TargetSquad.CurrentLeader.CurrentAttack = AttackOld;

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
            Map.BattleSumaryDefenceDraw(g, ActivePlayerIndex, ActiveSquadIndex, ActiveSquadSupport, TargetPlayerIndex, TargetSquadIndex, ListAttackTargetSquad, TargetSquadSupport);
        }
    }
}
