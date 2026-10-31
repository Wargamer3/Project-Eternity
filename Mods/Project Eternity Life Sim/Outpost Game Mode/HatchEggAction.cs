using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class HatchEggAction : LifeSimAIAction
    {
        public MatchFacts Match;

        public HatchEggAction()
            : base("Hatch Egg", "Hatch Egg")
        {
        }

        public override AIAction Copy()
        {
            HatchEggAction Clone = new HatchEggAction();
            CopyTo(Clone);
            Clone.Match = Match;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            Outpost Empty = Target();
            if (Empty == null)
            {
                return new List<AutomatedAction>();
            }

            List<AutomatedAction> Steps = LifePlan.GoTo(Params.Owner, Empty.Chunk, Empty.Position);
            Steps.Add(new HatchEggStep(Params.Owner, Match, Empty));
            return StanceSequence.Splice(Params.Owner.CurrentPose, Steps);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Target() == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }

        Outpost Target()
        {
            if (Params == null || Params.Owner == null || Match == null || Match.Eggs <= 0)
            {
                return null;
            }

            return Match.Nearest(Params.Owner.Position, OutpostSide.Empty, false);
        }
    }
}
