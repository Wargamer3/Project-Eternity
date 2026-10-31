namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceSpikeTrapAction : SpendAction
    {
        public PlaceSpikeTrapAction() : base("Place Spike Trap", "Spike Trap") { }
        protected override int CostOf(MatchFacts Match) { return Match.SpikeCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }
    }
}
