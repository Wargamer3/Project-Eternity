using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelAttackPart1 : ActionPanelDeathmatch
    {
        private bool CanMove;
        private Squad ActiveSquad;
        private int ActivePlayerIndex;

        public ActionPanelAttackPart1(bool CanMove, Squad ActiveSquad, int ActivePlayerIndex, DeathmatchMap Map)
            : base("Attack", Map)
        {
            this.ActiveSquad = ActiveSquad;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.CanMove = CanMove;
        }

        public override void OnSelect()
        {
            Map.CursorPosition = ActiveSquad.Position;
            Map.CursorPositionVisible = Map.CursorPosition;

            //Update weapons so you know which one is in attack range.
            Map.UpdateAllAttacks(ActiveSquad.CurrentLeader, ActiveSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, CanMove);

            ActiveSquad.CurrentLeader.AttackIndex = 0;//Make sure you select the first weapon.
        }

        public override void DoUpdate(GameTime gameTime)
        {
            //Move the cursor.
            if (InputHelper.InputUpPressed())
            {
                --ActiveSquad.CurrentLeader.AttackIndex;
                if (ActiveSquad.CurrentLeader.AttackIndex < 0)
                    ActiveSquad.CurrentLeader.AttackIndex = ActiveSquad.CurrentLeader.ListAttack.Count - 1;

                Map.sndSelection.Play();
            }
            else if (InputHelper.InputDownPressed())
            {
                ++ActiveSquad.CurrentLeader.AttackIndex;
                if (ActiveSquad.CurrentLeader.AttackIndex >= ActiveSquad.CurrentLeader.ListAttack.Count)
                    ActiveSquad.CurrentLeader.AttackIndex = 0;

                Map.sndSelection.Play();
            }
            else if (MouseHelper.MouseMoved())
            {
                int YStep = 25;
                int YStart = 122;

                if (MouseHelper.MouseStateCurrent.Y >= YStart)
                {
                    int NewValue = (MouseHelper.MouseStateCurrent.Y - YStart) / YStep;
                    if (NewValue >= 0 && NewValue < ActiveSquad.CurrentLeader.ListAttack.Count)
                    {
                        ActiveSquad.CurrentLeader.AttackIndex = NewValue;
                    }
                }
            }
            //Exit the weapon panel.
            if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                if (ActiveSquad.CurrentLeader.CurrentAttack.CanAttack)
                {
                    if (ActiveSquad.CurrentLeader.CurrentAttack.Pri == Core.Attacks.WeaponPrimaryProperty.MAP)
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackMAP(Map, ActiveSquad, ActivePlayerIndex));
                    }
                    else
                    {
                        AddToPanelListAndSelect(new ActionPanelAttackPart2(Map, ActiveSquad, ActivePlayerIndex));
                    }
                }
                else
                {
                    Map.sndDeny.Play();
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            //Draw the weapon selection menu.
            Map.DrawAttackPanel(g, Map.fntFinlanderFont, ActiveSquad.CurrentLeader, ActiveSquad.CurrentLeader.AttackIndex);
        }
    }
}
