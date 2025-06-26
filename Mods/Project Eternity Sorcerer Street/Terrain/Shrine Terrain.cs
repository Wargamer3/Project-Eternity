namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //idea: global land transfer spell arandomly changed the colors of all unoccupied lands.
    public class ShrineTerrain : TerrainSorcererStreet
    {
        public ShrineTerrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            if (MovementRemaining == 0)
            {
                Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex, true));
            }
        }
    }
}
