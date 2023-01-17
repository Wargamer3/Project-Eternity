using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPart1 : ActionPanelDeathmatch
    {
        private const string PanelName = "Attack";

        private int AttackIndex;

        private bool CanMove;
        private int ActivePlayerIndex;
        private int ActiveSquadIndex;
        private Squad ActiveSquad;
        private List<Attack> ListAttack;
        private List<Vector3> ListMVHoverPoints;

        public ActionPanelAttackPart1(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackPart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, bool CanMove, List<Vector3> ListMVHoverPoints)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.CanMove = CanMove;
            this.ListMVHoverPoints = ListMVHoverPoints;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ListAttack = ActiveSquad.CurrentLeader.ListAttack;
        }

        public override void OnSelect()
        {
            Map.AttackPicker.Reset(ActiveSquad.CurrentLeader, ListAttack);
            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;

            //Update weapons so you know which one is in attack range.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, CanMove);

            ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[0];//Make sure you select the first weapon.
        }

        public override void DoUpdate(GameTime gameTime)
        {
            int YStep = 25;
            int YStart = 122;

            if (ActiveInputManager.InputUpPressed())
            {
                --AttackIndex;
                if (AttackIndex < 0)
                    AttackIndex = ListAttack.Count - 1;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++AttackIndex;
                if (AttackIndex >= ListAttack.Count)
                    AttackIndex = 0;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[AttackIndex];

                Map.AttackPicker.SetCursorIndex(AttackIndex);

                Map.sndSelection.Play();

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                for (int A = 0; A < ListAttack.Count; A++)
                {
                    if (AttackIndex != A && ActiveInputManager.IsInZone(0, YStart + A * YStep, Constants.Width, YStart + (A + 1) * YStep))
                    {
                        AttackIndex = A;
                        ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[A];
                        Map.AttackPicker.SetCursorIndex(AttackIndex);

                        Map.sndSelection.Play();

                        if (Map.OnlineClient != null)
                        {
                            Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                        }
                        break;
                    }
                }
            }
            //Exit the weapon panel.
            if (ActiveInputManager.InputConfirmPressed(0, YStart + AttackIndex * YStep, Constants.Width, YStart + (AttackIndex + 1) * YStep))
            {
                if (ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                {
                    if (ActiveSquad.CurrentLeader.CurrentAttack.IsChargeable)
                    {
                        AddToPanelListAndSelect(new ActionPanelChargeAttack(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                    }
                    else if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.PER)
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackPER(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                    }
                    else if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackMAP(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                    }
                    else
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackPart2(Map, ActivePlayerIndex, ActiveSquadIndex, ListMVHoverPoints));
                    }
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveSquadIndex = BR.ReadInt32();
            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ListAttack = ActiveSquad.CurrentLeader.ListAttack;
            AttackIndex = BR.ReadInt32();

            if (Map.AttackPicker != null)
            {
                Map.AttackPicker.Reset(ActiveSquad.CurrentLeader, ListAttack);
                Map.AttackPicker.SetCursorIndex(AttackIndex);
            }
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            AttackIndex = ArrayUpdateData[0];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendInt32(AttackIndex);
        }

        public override byte[] DoWriteUpdate()
        {

            return new byte[] { (byte)AttackIndex };
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackPart1(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the weapon selection menu.
            Map.DrawAttackPanel(g, Map.fntFinlanderFont);
        }
    }
}
