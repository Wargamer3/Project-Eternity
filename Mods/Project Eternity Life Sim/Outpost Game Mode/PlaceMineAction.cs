namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceMineAction : SpendAction
    {
        public PlaceMineAction() : base("Place Mine", "Mine") { }
        protected override int CostOf(MatchFacts Match) { return Match.MineCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }
    }
}
