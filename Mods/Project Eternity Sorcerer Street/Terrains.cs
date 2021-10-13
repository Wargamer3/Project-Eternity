using System;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class ElementalTerrain : TerrainSorcererStreet
    {
        public ElementalTerrain(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos, TerrainTypeIndex)
        {

        }

        public override void OnSelect(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            if (DefendingCreature == null)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelCreatureCardSelectionPhase(Map, ActivePlayerIndex));
            }
            else if (Owner.Team == Map.ListPlayer[ActivePlayerIndex].Team)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map));
            }
            else
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPayTollPhase(Map, ActivePlayerIndex, this));
            }
        }
    }

    class GateTerrain : TerrainSorcererStreet
    {
        public GateTerrain(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos, TerrainTypeIndex)
        {

        }

        public override void OnSelect(SorcererStreetMap Map, int ActivePlayerIndex)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelGateCommands(Map, ActivePlayerIndex));
        }
    }
}
