namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //idea: global  land transfer spell arandomly changed the colors of all unoccupied lands.
    public class ShrineTerrain : TerrainSorcererStreet
    {
        public ShrineTerrain(int XPos, int YPos, int LayerIndex, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, LayerIndex, LayerDepth, TerrainTypeIndex)
        {

        }

        public override void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
        }
    }
}
