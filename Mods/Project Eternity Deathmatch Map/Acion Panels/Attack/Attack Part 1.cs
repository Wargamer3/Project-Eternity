﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Attacks;

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

        public ActionPanelAttackPart1(DeathmatchMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelAttackPart1(DeathmatchMap Map, int ActivePlayerIndex, int ActiveSquadIndex, bool CanMove)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveSquadIndex = ActiveSquadIndex;
            this.CanMove = CanMove;

            ActiveSquad = Map.ListPlayer[ActivePlayerIndex].ListSquad[ActiveSquadIndex];
            ListAttack = ActiveSquad.CurrentLeader.ListAttack;
        }

        public override void OnSelect()
        {
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

            //Move the cursor.
            if (ActiveInputManager.InputUpPressed())
            {
                --AttackIndex;
                if (AttackIndex < 0)
                    AttackIndex = ListAttack.Count - 1;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[AttackIndex];

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ++AttackIndex;
                if (AttackIndex >= ListAttack.Count)
                    AttackIndex = 0;

                ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[AttackIndex];

                Map.sndSelection.Play();
            }
            else if (ActiveInputManager.InputMovePressed())
            {
                for (int A = 0; A < ListAttack.Count; A++)
                {
                    if (ActiveInputManager.IsInZone(0, YStart + A * YStep, Constants.Width, YStart + (A + 1) * YStep))
                    {
                        AttackIndex = A;
                        ActiveSquad.CurrentLeader.CurrentAttack = ListAttack[A];
                        break;
                    }
                }
            }
            //Exit the weapon panel.
            if (ActiveInputManager.InputConfirmPressed(0, YStart + AttackIndex * YStep, Constants.Width, YStart + (AttackIndex + 1) * YStep))
            {
                if (ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                {
                    if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.PER)
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackPER(Map, ActivePlayerIndex, ActiveSquadIndex));
                    }
                    else if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == WeaponPrimaryProperty.MAP)
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackMAP(Map, ActivePlayerIndex, ActiveSquadIndex));
                    }
                    else if (ActiveSquad.CurrentLeader.CurrentAttack.IsChargeable)
                    {
                        AddToPanelListAndSelect(new ActionPanelChargeAttack(Map, ActivePlayerIndex, ActiveSquadIndex));
                    }
                    else
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackPart2(Map, ActivePlayerIndex, ActiveSquadIndex));
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
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveSquadIndex);
            BW.AppendInt32(AttackIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelAttackPart1(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the weapon selection menu.
            Map.DrawAttackPanel(g, Map.fntFinlanderFont, ActiveSquad.CurrentLeader, ListAttack, ListAttack[AttackIndex]);
        }
    }
}
