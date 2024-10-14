namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class TowerTerrain : TerrainSorcererStreet
    {
        public TowerTerrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTowerCommands(Map, ActivePlayerIndex, TerrainTypeIndex, MovementRemaining));
        }
    }
}
