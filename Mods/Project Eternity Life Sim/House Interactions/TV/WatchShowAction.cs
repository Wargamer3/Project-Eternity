using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class WatchShowInteraction : LifeInteraction
    {
        public override string Name => "WatchShowAction";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || !FurnitureUse.IsScreen(Context.Box))
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            LifeSimMap Map = Context.Map;
            if (!Box.Open)
            {
                Choices.Add(new InteractionChoice { Label = "Open TV", Order = 30, Run = () => LifeOrder.Power(Who, Box, Map, true) });
                return;
            }

            Choices.Add(new InteractionChoice { Label = "Watch", Order = 31, Run = () => LifeOrder.Watch(Who, Box, Map) });
            Choices.Add(new InteractionChoice { Label = "Close TV", Order = 32, Run = () => LifeOrder.Power(Who, Box, Map, false) });
        }
    }

    public class WatchShowAction : LifeSimStep
    {
        readonly bool Sitting;

        public WatchShowAction(PlayerCharacter Owner, bool Sitting)
            : base("Watch", Owner, 0.4)
        {
            this.Sitting = Sitting;
        }

        public override Pose PoseNeeded => Sitting ? Pose.Sit : Pose.Stand;

        public override Pose PoseLeft => Sitting ? Pose.Sit : Pose.Stand;

        protected override AutomatedAction Copy()
        {
            return new WatchShowAction(Owner, Sitting);
        }
    }
}
