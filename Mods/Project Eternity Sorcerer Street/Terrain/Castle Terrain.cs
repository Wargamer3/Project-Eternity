namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CastleTerrain : TerrainSorcererStreet
    {
        public CastleTerrain(int XPos, int YPos, int LayerIndex, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, LayerIndex, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelCastlePhase(Map, ActivePlayerIndex, MovementRemaining));
        }
    }
}
