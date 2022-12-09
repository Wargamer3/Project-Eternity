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
                SwitchToMainMenu();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAnimationTimer();
        }

        public void SwitchToMainMenu()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelMainMenuPhase(Map, ActivePlayerIndex));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, Constants.Width / 16, Constants.Height / 10);

            int ActionInfoBoxX = Constants.Width / 16;
            int ActionInfoBoxY = Constants.Height / 3;
            int ActionInfoBoxWidth = Constants.Width / 5;
            int ActionInfoBoxHeight = Constants.Height / 14;
            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntArial12, "Spell Selection", new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2), Color.White);
        }

        public override void DrawArrows(CustomSpriteBatch g)
        {
            float Y = Constants.Height - Constants.Height / 6;
            float Scale = Constants.Width / 3764.70581f;

            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Y - 340 * Scale + (float)Math.Sin(ArrowAnimationTime * 10) * 3f), Color.White);
            g.Draw(Map.sprArrowUp, new Vector2(Constants.Width / 2, Constants.Height - 20 - (float)Math.Sin(ArrowAnimationTime * 10) * 3f), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0f);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpellCardSelectionPhase(Map);
        }
    }
}
