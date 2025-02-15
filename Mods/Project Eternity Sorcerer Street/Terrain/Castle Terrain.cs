namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class CastleTerrain : TerrainSorcererStreet
    {
        public const string CastleReachedLifetimeType = "Sorcerer Street Castle Reached";

        public CastleTerrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelCastlePhase(Map, ActivePlayerIndex, MovementRemaining));
        }
    }
}
