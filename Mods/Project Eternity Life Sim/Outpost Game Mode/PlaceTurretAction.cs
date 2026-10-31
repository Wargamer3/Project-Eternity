using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PlaceTurretAction : LifeSimAIAction
    {
        public MatchFacts Match;
        public Vector3 Place;

        public PlaceTurretAction()
            : base("Place Turret", "Place Turret")
        {
        }

        public override AIAction Copy()
        {
            PlaceTurretAction Clone = new PlaceTurretAction();
            CopyTo(Clone);
            Clone.Match = Match;
            Clone.Place = Place;
            return Clone;
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            return true;
        }

        public override List<AutomatedAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            if (!CanAfford())
            {
                return new List<AutomatedAction>();
            }

            List<AutomatedAction> Steps = LifePlan.GoTo(Params.Owner, Params.CurrentMapInfo, Place);
            Steps.Add(new PlaceDeployableStep(Params.Owner, Match, Params.CurrentMapInfo, Place, "Turret", WorldMarkerKind.Object, Match.TurretCost, 0f));
            return StanceSequence.Splice(Params.Owner.CurrentPose, Steps);
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            AIWeight = CanAfford() && MatchRole.Accepts(Params.Owner, AIActionName) ? 1 : 0;
        }

        bool CanAfford()
        {
            return Params != null && Params.Owner != null && Match != null && Match.Points >= Match.TurretCost;
        }
    }
}
