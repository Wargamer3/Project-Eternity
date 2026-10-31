namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceSupplyDepotAction : SpendAction
    {
        public PlaceSupplyDepotAction() : base("Place Supply Depot", "Supply Depot") { }
        protected override int CostOf(MatchFacts Match) { return Match.DepotCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }
    }
}
