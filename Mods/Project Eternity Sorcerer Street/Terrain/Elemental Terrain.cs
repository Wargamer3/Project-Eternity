using System;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ElementalTerrain : TerrainSorcererStreet
    {
        public ElementalTerrain(int XPos, int YPos, int LayerIndex, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, LayerIndex, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            if (MovementRemaining == 0)
            {
                if (DefendingCreature == null)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelLandInfoPhase(Map, ActivePlayerIndex));
                }
                else if (PlayerOwner.TeamIndex == Map.ListPlayer[ActivePlayerIndex].TeamIndex)
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTerrainLevelUpCommands(Map));
                }
                else
                {
                    Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelPayTollPhase(Map, ActivePlayerIndex, this));
                }
            }
        }
    }
}
