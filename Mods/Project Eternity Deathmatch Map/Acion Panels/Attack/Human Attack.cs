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
    public class ActionPanelHumanAttack : ActionPanelDeathmatch
    {
        private const string PanelName = "HumanAttack";

        private int AttackIndex;
        private Attack WeaponOld;

        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private SupportSquadHolder ActiveSquadSupport;
        private List<Vector3> ListMVHoverPoints;
        private List<Attack> ListAttackActiveSquad;
        private List<Attack> ListAttackActiveSquadSupport;

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
            List<Vector3> ListMVHoverPoints,
             int TargetPlayerIndex, int TargetSquadIndex, SupportSquadHolder TargetSquadSupport)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.ListMVHoverPoints = ListMVHoverPoints;

            this.TargetPlayerIndex = TargetPlayerIndex;
            this.TargetSquadIndex = TargetSquadIndex;
            this.TargetSquadSupport = TargetSquadSupport;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];

            ListAttackActiveSquad = ActiveSquad.CurrentLeader.ListAttack;

            if (ActiveSquadSupport.ActiveSquadSupport != null)
            {
                ListAttackActiveSquadSupport = ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.ListAttack;
            }
            else
            {
                ListAttackActiveSquadSupport = new List<Attack>();
            }
            TargetSquad = Map.ListPlayer[TargetPlayerIndex].ListSquad[TargetSquadIndex];
        }

        public override void OnSelect()
        {
            //Attack Support
            ActiveSquadSupport.PrepareAttackSupport(Map, ActivePlayerIndex, ActiveSquad, TargetPlayerIndex, TargetSquadIndex);
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
                            Map.AttackPicker.Reset(ActiveSquad.CurrentLeader, ListAttackActiveSquad);
                            ActiveSquad.CurrentLeader.UpdateNonMAPAttacks(ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, TargetSquad.Position, Map.ListPlayer[TargetPlayerIndex].Team, TargetSquad.ArrayMapSize, TargetSquad.CurrentTerrainIndex, ActiveSquad.CanMove);

                            AttackIndex = 0;//Make sure you select the first weapon.
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
                --AttackIndex;
                if (AttackIndex < 0)
                    AttackIndex = ListAttackActiveSquad.Count - 1;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttackActiveSquad[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++AttackIndex;
                if (AttackIndex >= ListAttackActiveSquad.Count)
                    AttackIndex = 0;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttackActiveSquad[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                int YStep = 25;

                for (int A = 0; A < ListAttackActiveSquad.Count; ++A)
                {
                    int YStart = 122 + A * YStep;
                    if (ActiveInputManager.IsInZone(0, YStart, Constants.Width, YStart + YStep))
                    {
                        AttackIndex = A;
                        ActiveSquad.CurrentLeader.CurrentAttack = ListAttackActiveSquad[A];
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
                    ActiveSquad.CurrentLeader.AttackAccuracy = Map.CalculateHitRate(ActiveSquad.CurrentLeader, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquad,
                        TargetSquad.CurrentLeader, TargetSquad, TargetSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
                    if (!Map.ListPlayer[TargetPlayerIndex].IsPlayerControlled)
                    {
                        PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, TargetPlayerIndex, TargetSquadIndex);
                    }

                    Map.BattleMenuStage = BattleMenuStages.Default;
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

                for (int U = ActiveSquad.UnitsAliveInSquad - 1; U >= 1; --U)
                {
                    if (ActiveSquad[U].CanAttack)
                        ActiveSquad[U].BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                    else
                        ActiveSquad[U].BattleDefenseChoice = Unit.BattleDefenseChoices.Defend;
                }

                //Simulate defense reaction.
                PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, TargetPlayerIndex, TargetSquadIndex);
                PrepareAttackSquadForBattle(Map, ActiveSquad, ActiveSquad.CurrentLeader.CurrentAttack, TargetSquad);

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
                Map.BattleMenuCursorIndexThird = (int)ActiveSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice;

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
                    PrepareDefenseSquadForBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, TargetPlayerIndex, TargetSquadIndex);

                    if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Spread)
                    {
                        ActiveSquad[Map.BattleMenuCursorIndexSecond].CurrentAttack = ActiveSquad[Map.BattleMenuCursorIndexSecond].PLAAttack;
                        ActiveSquad[Map.BattleMenuCursorIndexSecond].AttackAccuracy = Map.CalculateHitRate(ActiveSquad[Map.BattleMenuCursorIndexSecond], ActiveSquad[Map.BattleMenuCursorIndexSecond].CurrentAttack, ActiveSquad,
                            TargetSquad[Map.BattleMenuCursorIndexSecond], TargetSquad, TargetSquad[Map.BattleMenuCursorIndexSecond].BattleDefenseChoice).ToString() + "%";
                    }
                    else if (Map.BattleMenuOffenseFormationChoice == FormationChoices.Focused)
                    {
                        ActiveSquad[Map.BattleMenuCursorIndexSecond].CurrentAttack = ActiveSquad[Map.BattleMenuCursorIndexSecond].PLAAttack;
                        ActiveSquad[Map.BattleMenuCursorIndexSecond].AttackAccuracy = Map.CalculateHitRate(ActiveSquad[Map.BattleMenuCursorIndexSecond], ActiveSquad[Map.BattleMenuCursorIndexSecond].CurrentAttack, ActiveSquad,
                            TargetSquad.CurrentLeader, TargetSquad, TargetSquad.CurrentLeader.BattleDefenseChoice).ToString() + "%";
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
                    Map.AttackPicker.Reset(ActiveSquadSupport.ActiveSquadSupport.CurrentLeader, ListAttackActiveSquadSupport);
                    //Update weapons so you know which one is in attack range.

                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.DisableAllAttacks();
                    ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.UpdateAllAttacks(
                        ActiveSquadSupport.ActiveSquadSupport.Position, Map.ListPlayer[ActivePlayerIndex].Team,
                        TargetSquad.Position, Map.ListPlayer[TargetPlayerIndex].Team, TargetSquad.ArrayMapSize, TargetSquad.CurrentTerrainIndex,
                        ActiveSquadSupport.ActiveSquadSupport.CanMove);

                    WeaponOld = ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack;
                    AttackIndex = 0;//Make sure you select the first weapon.
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
                --AttackIndex;
                if (AttackIndex < 0)
                    AttackIndex = ListAttackActiveSquadSupport.Count - 1;

                ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack = ListAttackActiveSquadSupport[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++AttackIndex;
                if (AttackIndex >= ListAttackActiveSquadSupport.Count)
                    AttackIndex = 0;

                ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack = ListAttackActiveSquadSupport[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                int YStep = 25;

                for (int A = 0; A < ListAttackActiveSquad.Count; ++A)
                {
                    int YStart = 122 + A * YStep;
                    if (ActiveInputManager.IsInZone(0, YStart, Constants.Width, YStart + YStep))
                    {
                        AttackIndex = A;
                        ActiveSquad.CurrentLeader.CurrentAttack = ListAttackActiveSquad[A];
                        break;
                    }
                }
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                if (ListAttackActiveSquad[AttackIndex].CanAttack)
                {
                    Map.BattleMenuStage = BattleMenuStages.Default;
                    Map.sndConfirm.Play();
                }
                else
                {
                    Map.sndDeny.Play();
                }
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
            AddToPanelListAndSelect(new ActionPanelStartBattle(Map, ActivePlayerIndex, ActiveSquadIndex, ActiveSquad.CurrentLeader.CurrentAttack, ActiveSquadSupport, ListMVHoverPoints, TargetPlayerIndex, TargetSquadIndex, TargetSquadSupport, false));
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ActiveSquadSupport = new SupportSquadHolder();
            ActiveSquad.CurrentLeader.BattleDefenseChoice = (Unit.BattleDefenseChoices)BR.ReadByte();
            string ActiveSquadAttackName = BR.ReadString();

            ListAttackActiveSquad = ActiveSquad.CurrentLeader.ListAttack;
            if (!string.IsNullOrEmpty(ActiveSquadAttackName))
            {
                foreach (Attack ActiveAttack in ListAttackActiveSquad)
                {
                    if (ActiveAttack.ItemName == ActiveSquadAttackName)
                    {
                        ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
                        break;
                    }
                }
            }

            if (ActiveSquadSupport.ActiveSquadSupport != null)
            {
                ListAttackActiveSquadSupport = ActiveSquadSupport.ActiveSquadSupport.CurrentLeader.ListAttack;
            }
            else
            {
                ListAttackActiveSquadSupport = new List<Attack>();
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

            Map.BattleMenuStage = (BattleMenuStages)BR.ReadByte();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendByte((byte)ActiveSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendString(ActiveSquad.CurrentLeader.CurrentAttack.ItemName);

            BW.AppendInt32(TargetPlayerIndex);
            BW.AppendInt32(TargetSquadIndex);
            BW.AppendByte((byte)TargetSquad.CurrentLeader.BattleDefenseChoice);
            BW.AppendString(TargetSquad.CurrentLeader.CurrentAttack.ItemName);

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
