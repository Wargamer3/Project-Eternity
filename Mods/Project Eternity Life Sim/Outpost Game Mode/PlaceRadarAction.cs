namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceRadarAction : SpendAction
    {
        public PlaceRadarAction() : base("Place Radar", "Radar") { }
        protected override int CostOf(MatchFacts Match) { return Match.RadarCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }
    }
}
