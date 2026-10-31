namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceSpeedStripAction : SpendAction
    {
        public PlaceSpeedStripAction() : base("Place Speed Strip", "Speed Strip") { }
        protected override int CostOf(MatchFacts Match) { return Match.SpeedStripCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }
    }
}
