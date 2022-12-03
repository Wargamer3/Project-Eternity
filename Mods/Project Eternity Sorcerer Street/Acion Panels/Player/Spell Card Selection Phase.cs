using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSpellCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "SpellCard";

        private double ArrowAnimationTime;

        public ActionPanelSpellCardSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, SpellCard.SpellCardType)
        {
            DrawDrawInfo = true;
        }

        public ActionPanelSpellCardSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, SpellCard.SpellCardType)
        {
            DrawDrawInfo = true;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;

            base.DoUpdate(gameTime);

            if (InputHelper.InputUpPressed())
            {
                //Move menu downward and then open the dice menu
                PrepareToRollDice();
            }
            else if (InputHelper.InputDownPressed())
            {
                SwitchToTerritory();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAnimationTimer();
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            int BoxHeight = 70;

            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, 30, Constants.Height / 20);

            GameScreen.DrawBox(g, new Vector2(30, Constants.Height / 20 + BoxHeight * 2), 200, 30, Color.White);
            g.DrawStringCentered(Map.fntArial12, "Spell Selection", new Vector2(130, Constants.Height / 20 + BoxHeight * 2 + 15), Color.White);

        }

        public override void DrawArrows(CustomSpriteBatch g)
        {
            float CardsHeight = Constants.Height - 100;
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, CardsHeight - 95 + (float)Math.Sin(ArrowAnimationTime * 10) * 3f), Color.White);
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 20 - (float)Math.Sin(ArrowAnimationTime * 10) * 3f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpellCardSelectionPhase(Map);
        }
    }
}
