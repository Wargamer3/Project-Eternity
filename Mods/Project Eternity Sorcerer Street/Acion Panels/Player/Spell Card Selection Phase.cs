using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSpellCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        public ActionPanelSpellCardSelectionPhase(SorcererStreetMap Map, Player ActivePlayer)
            : base(Map.ListActionMenuChoice, Map, ActivePlayer, SpellCard.SpellCardType)
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            if (InputHelper.InputUpPressed())
            {
                //Move menu downward and then open the dice menu
                PrepareToRollDice();
            }
        }

        public override void DrawArrows(CustomSpriteBatch g)
        {
            float CardsHeight = Constants.Height - 100;
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, CardsHeight - 95), Color.White);
            //g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 20), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }
    }
}
