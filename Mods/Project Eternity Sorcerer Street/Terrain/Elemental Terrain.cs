using System;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ElementalTerrain : TerrainSorcererStreet
    {
        public ElementalTerrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            if (MovementRemaining <= 0)
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
