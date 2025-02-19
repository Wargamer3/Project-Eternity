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
                AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex, ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).AllowTerrainCommands));
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

            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            ActionPanelPlayerDefault.DrawPhase(g, Map, "Creature Selection");
            MenuHelper.DrawUpArrow(g);
            MenuHelper.DrawDownArrow(g);
        }
    }
}
