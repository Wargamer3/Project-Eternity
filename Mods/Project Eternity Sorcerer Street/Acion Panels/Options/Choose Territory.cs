using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using System.Collections.Generic;

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
                AddToPanelListAndSelect(new ActionPanelTerritoryActions(Map, ActivePlayerIndex, ActiveTerrain));

            }
            else if (ActiveInputManager.InputCancelPressed())
            {
                RemoveFromPanelList(this);
                Map.Camera3DOverride = null;
            }
        }

        public static List<TerrainSorcererStreet> GetAllAccessibleTerritories(SorcererStreetMap Map, Player ActivePlayer, bool AllTerritory)
        {
            if (AllTerritory)
            {
                return ActivePlayer.ListSummonedCreature;
            }

            return Map.ListPassedTerrain;
        }

        public static List<TerrainSorcererStreet> GetAllUpgradableTerritories(SorcererStreetMap Map, Player ActivePlayer, bool AllTerritory)
        {
            List<TerrainSorcererStreet> ListAllAccessibleTerritory = GetAllAccessibleTerritories(Map, ActivePlayer, AllTerritory);
            List<TerrainSorcererStreet> ListAllUpgradableTerritories = new List<TerrainSorcererStreet>(ListAllAccessibleTerritory.Count);

            foreach (TerrainSorcererStreet ActiveTerrain in ListAllAccessibleTerritory)
            {
                int NextUpgradePrice = ActionPanelTerrainLevelUpCommands.GetFinalUpgradePrice(ActiveTerrain, ActiveTerrain.LandLevel);
                bool IsWillingToPurchase = ActivePlayer.Inventory.Character.Character.PlayerCharacterAIParameter.IsWillingToPurchase(ActivePlayer.Gold, NextUpgradePrice);

                if (IsWillingToPurchase)
                {
                    ListAllUpgradableTerritories.Add(ActiveTerrain);
                }
            }

            return ListAllUpgradableTerritories;
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
