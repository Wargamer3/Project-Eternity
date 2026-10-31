using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class KillCreatureAction : LifeSimAIAction
    {
        public MatchFacts Match;

        public KillCreatureAction()
            : base("Kill Creature", "Kill Creature")
        {
        }

        public override AIAction Copy()
        {
            KillCreatureAction Clone = new KillCreatureAction();
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
            Outpost Enemy = Target();
            if (Enemy == null)
            {
                return new List<AutomatedAction>();
            }

            List<AutomatedAction> Steps = LifePlan.GoTo(Params.Owner, Enemy.Chunk, Enemy.Position);
            Steps.Add(new KillCreatureStep(Params.Owner, Enemy));
            return StanceSequence.Splice(Params.Owner.CurrentPose, Steps);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = Target() == null || !MatchRole.Accepts(Params.Owner, AIActionName) ? 0 : 1;
        }

        Outpost Target()
        {
            if (Params == null || Params.Owner == null || Match == null)
            {
                return null;
            }

            return Match.Nearest(Params.Owner.Position, OutpostSide.Enemy, true);
        }
    }
}
