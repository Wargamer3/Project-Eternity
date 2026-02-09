using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSellTerritory : ActionPanelViewMap
    {
        private const string PanelName = "SellTerritory";

        private int ActivePlayerIndex;
        private Player ActivePlayer;

        public ActionPanelSellTerritory(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelSellTerritory(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            Map.ListHighlightedTerrain.AddRange(ActivePlayer.ListSummonedCreature);
        }

        public override void OnSelect()
        {
            base.OnSelect();
            AddToPanelListAndSelect(new ActionPanelBankruptPopup(Map, ActivePlayerIndex));
        }

        public override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            if (ActiveInputManager.InputConfirmPressed() && ActiveTerrain != null
                && ActivePlayer.ListSummonedCreature.Contains(ActiveTerrain))
            {
                AddToPanelListAndSelect(new ActionPanelSellTerritoryConfirm(Map, ActivePlayerIndex, ActiveTerrain));
            }
            else if (ActivePlayer.Gold >= 0)
            {
                AddToPanelListAndSelect(new ActionPanelPlayerDefault(Map));
                Map.ListHighlightedTerrain.Clear();
            }
            else if (ActivePlayer.ListSummonedCreature.Count <= 0)
            {
                AddToPanelListAndSelect(new ActionPanelBankruptPhase(Map, ActivePlayerIndex));
                Map.ListHighlightedTerrain.Clear();
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSellTerritory(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
        }

        public override string ToString()
        {
            return "Sell terrains you own.";
        }
    }
}
