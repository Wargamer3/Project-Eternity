﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelConfirmCreatureSummon : ActionPanelSorcererStreet
    {
        private readonly Player ActivePlayer;
        private readonly CreatureCard ActiveCard;

        private int CursorIndex;

        public ActionPanelConfirmCreatureSummon(SorcererStreetMap Map, Player ActivePlayer, CreatureCard ActiveCard)
            : base("Confirm Creature Summon", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
            this.ActiveCard = ActiveCard;
        }

        public override void OnSelect()
        {
            CursorIndex = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex == 0)
                {
                    FinishPhase();
                }
                else if (CursorIndex == 1)
                {
                    RemoveFromPanelList(this);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                ++CursorIndex;
                if (CursorIndex > 1)
                    CursorIndex = 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                --CursorIndex;
                if (CursorIndex < 0)
                    CursorIndex = 1;
            }
        }

        public void FinishPhase()
        {
            TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);

            ActiveTerrain.DefendingCreature = ActiveCard;
            ActiveTerrain.Owner = ActivePlayer;

            Map.ListActionMenuChoice.RemoveAllActionPanels();
            Map.EndPlayerPhase();
        }

        protected override void OnCancelPanel()
        {
        }

        private void DrawIntro(CustomSpriteBatch g)
        {//Spin card from the left
        }

        private void DrawOutro(CustomSpriteBatch g)
        {//Spin card to its place in the hand
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActiveCard.DrawCard(g);
            ActiveCard.DrawCardInfo(g, Map.fntArial12);

            GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 120), 200, 90, Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Summon this creature?", new Vector2(Constants.Width / 2, Constants.Height - 110), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "Yes", new Vector2(Constants.Width / 2, Constants.Height - 85), Color.White);
            g.DrawStringMiddleAligned(Map.fntArial12, "No", new Vector2(Constants.Width / 2, Constants.Height - 60), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 - 80, Constants.Height - 85 + CursorIndex * 25, 18, 18), Color.White);
        }
    }
}
