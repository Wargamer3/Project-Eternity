using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceDeployableStep : LifeSimStep
    {
        private readonly MatchFacts Match;
        private readonly MapInfo Chunk;
        private readonly Vector3 Place;
        private readonly string Tag;
        private readonly WorldMarkerKind Kind;
        private readonly int Cost;
        private readonly float CutRadius;
        private readonly bool HasTraversal;
        private readonly Traversal TraversalKind;
        private readonly Vector3 TraversalEnd;

        public PlaceDeployableStep(PlayerCharacter Owner, MatchFacts Match, MapInfo Chunk, Vector3 Place, string Tag, WorldMarkerKind Kind, int Cost, float CutRadius, bool HasTraversal = false, Traversal TraversalKind = Traversal.JumpPad, Vector3 TraversalEnd = default)
            : base("Place " + Tag, Owner, 0.3)
        {
            this.Match = Match;
            this.Chunk = Chunk;
            this.Place = Place;
            this.Tag = Tag;
            this.Kind = Kind;
            this.Cost = Cost;
            this.CutRadius = CutRadius;
            this.HasTraversal = HasTraversal;
            this.TraversalKind = TraversalKind;
            this.TraversalEnd = TraversalEnd;
        }

        protected override void OnFinished()
        {
            if (Match == null || Owner == null || Owner.Belief == null || Match.Points < Cost)
            {
                return;
            }

            Match.Points -= Cost;
            WorldMarker Marker = new WorldMarker();
            Marker.Name = Tag + " " + (Chunk == null ? 0 : Chunk.ListMarker.Count);
            Marker.Tag = Tag;
            Marker.Kind = Kind;
            Marker.LocalPosition = Place;
            Marker.CutRadius = CutRadius;
            Marker.HasTraversal = HasTraversal;
            Marker.TraversalKind = TraversalKind;
            Marker.TraversalEnd = TraversalEnd;
            if (Chunk != null)
            {
                Chunk.ListMarker.Add(Marker);
            }

            BelievedObject Known = new BelievedObject();
            Known.Name = Marker.Name;
            Known.Tag = Marker.Tag;
            Known.Chunk = Chunk;
            Known.LocalPosition = Place;
            Known.Kind = Kind;
            Known.CutRadius = CutRadius;
            Known.HasTraversal = HasTraversal;
            Known.TraversalKind = TraversalKind;
            Known.TraversalEnd = TraversalEnd;
            Owner.Belief.ListObject.Add(Known);
        }

        protected override AutomatedAction Copy()
        {
            return new PlaceDeployableStep(Owner, Match, Chunk, Place, Tag, Kind, Cost, CutRadius, HasTraversal, TraversalKind, TraversalEnd);
        }
    }
}
