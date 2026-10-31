using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceJumpPadAction : SpendAction
    {
        public Vector3 LinkEnd;

        public PlaceJumpPadAction() : base("Place Jump Pad", "Jump Pad") { }

        public override AIAction Copy()
        {
            PlaceJumpPadAction Clone = new PlaceJumpPadAction();
            CopyTo(Clone);
            Clone.Match = Match;
            Clone.Place = Place;
            Clone.LinkEnd = LinkEnd;
            return Clone;
        }

        protected override int CostOf(MatchFacts Match) { return Match.JumpPadCost; }
        protected override WorldMarkerKind MarkerKind { get { return WorldMarkerKind.Object; } }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (!CanAfford())
            {
                return new List<AutomatedAction>();
            }

            return DeployablePlan.Build(Params.Owner, Match, Place, TagName, MarkerKind, CostOf(Match), 0f, true, Traversal.JumpPad, LinkEnd);
        }
    }
}
