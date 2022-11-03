namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class TowerTerrain : TerrainSorcererStreet
    {
        public TowerTerrain(int XPos, int YPos, int LayerIndex, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, LayerIndex, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
            Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelTowerCommands(Map, ActivePlayerIndex, TerrainTypeIndex, MovementRemaining));
        }
    }
}
