using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelChooseTerritory : ActionPanelViewMap
    {
        private const string PanelName = "ChooseTerritory";

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private bool AllTerritory;

        public ActionPanelChooseTerritory(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelChooseTerritory(SorcererStreetMap Map, int ActivePlayerIndex, bool AllTerritory)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.AllTerritory = AllTerritory;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            if (ActiveInputManager.InputConfirmPressed() && ActiveTerrain != null
                && ActiveTerrain.DefendingCreature != null
                && ActiveTerrain.TerrainTypeIndex != 0
                && ((AllTerritory && ActivePlayer.ListSummonedCreature.Contains(ActiveTerrain)) || Map.ListPassedTerrain.Contains(ActiveTerrain)))
            {
                AddToPanelListAndSelect(new ActionPanelTerritoryActions(Map, ActivePlayerIndex, ActiveTerrain, AllTerritory));

            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                RemoveFromPanelList(this);
                Map.Camera3DOverride = null;
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelChooseTerritory(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);
            foreach (TerrainSorcererStreet ActiveTerrain in Map.ListPassedTerrain)
            {
            }
        }

        public override string ToString()
        {
            return "You can level up your land you've passed through and change terrain. When you use this command, your turn will end.";
        }
    }
}
