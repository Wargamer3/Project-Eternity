using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelEndTurn : ActionPanelDeathmatch
    {
        private int ConfirmMenuChoice;
        private Texture2D sprCursorConfirmEndNo;
        private Texture2D sprCursorConfirmEndYes;

        public ActionPanelEndTurn(DeathmatchMap Map, Texture2D sprCursorConfirmEndNo, Texture2D sprCursorConfirmEndYes)
            : base("End Turn", Map, false)
        {
            this.sprCursorConfirmEndNo = sprCursorConfirmEndNo;
            this.sprCursorConfirmEndYes = sprCursorConfirmEndYes;

            ConfirmMenuChoice = 0;
        }

        public override void OnSelect()
        {
            ConfirmMenuChoice = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                if (ConfirmMenuChoice == 0)
                {
                    RemoveAllActionPanels();
                    ActionPanelPhaseChange PhaseEnd = new ActionPanelPhaseChange(Map);
                    Map.ListActionMenuChoice.Add(PhaseEnd);
                    PhaseEnd.OnSelect();
                }
                else if (ConfirmMenuChoice == 1)
                {
                    RemoveFromPanelList(this);
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveFromPanelList(this);
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (ConfirmMenuChoice == 1)
                    ConfirmMenuChoice = 0;
            }
            else if (InputHelper.InputRightPressed())
            {
                if (ConfirmMenuChoice == 0)
                    ConfirmMenuChoice = 1;
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int SquadRemaining = 0;
            foreach (Squad ActiveSquad in Map.ListPlayer[Map.ActivePlayerIndex].ListSquad)
                if (ActiveSquad.CurrentLeader != null && ActiveSquad.CanMove)
                    ++SquadRemaining;

            if (ConfirmMenuChoice == 0)
                g.Draw(sprCursorConfirmEndYes, new Vector2((Constants.Width - sprCursorConfirmEndYes.Width) / 2, (Constants.Height - sprCursorConfirmEndYes.Height) / 2), Color.White);
            else
                g.Draw(sprCursorConfirmEndNo, new Vector2((Constants.Width - sprCursorConfirmEndNo.Width) / 2, (Constants.Height - sprCursorConfirmEndNo.Height) / 2), Color.White);

            TextHelper.DrawText(g, SquadRemaining.ToString(), new Vector2((Constants.Width - sprCursorConfirmEndYes.Width) / 2 + 123,
                                                               (Constants.Height - sprCursorConfirmEndNo.Height) / 2 + 26), Color.White);
        }
    }
}
