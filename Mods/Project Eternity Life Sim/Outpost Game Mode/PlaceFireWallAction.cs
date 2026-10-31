namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceFireWallAction : SpendAction
    {
        public PlaceFireWallAction() : base("Place Fire Wall", "Fire Wall") { }
        protected override int CostOf(MatchFacts Match) { return Match.FireWallCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Obstacle; } }
        protected override float Radius { get { return NavCut.DefaultRadius; } }
    }
}
