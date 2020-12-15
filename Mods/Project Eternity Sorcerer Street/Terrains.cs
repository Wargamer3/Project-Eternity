using System;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class ElementalTerrain : TerrainSorcererStreet
    {
        public ElementalTerrain(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos, TerrainTypeIndex)
        {

        }

        public override void OnSelect(SorcererStreetMap Map, Player ActivePlayer)
        {
            if (DefendingCreature == null)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelCreatureCardSelectionPhase(Map, ActivePlayer));
            }
            else if (Owner.Team == ActivePlayer.Team)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map, ActivePlayer));
            }
            else
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPayTollPhase(Map, ActivePlayer, this));
            }
        }
    }

    class GateTerrain : TerrainSorcererStreet
    {
        public GateTerrain(int XPos, int YPos, int TerrainTypeIndex)
            : base(XPos, YPos, TerrainTypeIndex)
        {

        }

        public override void OnSelect(SorcererStreetMap Map, Player ActivePlayer)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelGateCommands(Map, ActivePlayer));
        }
    }
}
