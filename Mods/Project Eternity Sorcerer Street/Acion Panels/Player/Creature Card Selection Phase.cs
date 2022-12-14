using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelCreatureCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "CreatureCardSelection";
        private const string EndCardText = "End turn";

        private double ArrowAnimationTime;

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, CreatureCard.CreatureCardType, EndCardText)
        {
            DrawDrawInfo = true;
        }

        public ActionPanelCreatureCardSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, CreatureCard.CreatureCardType, EndCardText)
        {
            DrawDrawInfo = true;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;

            base.DoUpdate(gameTime);

            if (InputHelper.InputUpPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelLandInfoPhase(Map, ActivePlayerIndex));
            }
            else if (InputHelper.InputDownPressed())
            {
                RemoveFromPanelList(this);
                AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex));
            }
        }

        public override void OnEndCardSelected()
        {
            Map.EndPlayerPhase();
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCreatureCardSelectionPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            int ActionInfoBoxX = Constants.Width / 16;
            int ActionInfoBoxY = Constants.Height / 3;
            int ActionInfoBoxWidth = Constants.Width / 5;
            int ActionInfoBoxHeight = Constants.Height / 14;
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, Constants.Width / 16, Constants.Height / 10);
            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntArial12, "Creature Selection", new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2), Color.White);
            MenuHelper.DrawUpArrow(g);
            MenuHelper.DrawDownArrow(g);
        }
    }
}
