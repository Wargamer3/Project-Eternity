using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SitAction : LifeSimStep
    {
        public SitAction(PlayerCharacter Owner)
            : base("Sit", Owner, 0.4)
        {
        }

        public override Pose PoseNeeded => Pose.Sit;

        public override Pose PoseLeft => Pose.Sit;

        protected override AutomatedAction Copy()
        {
            return new SitAction(Owner);
        }
    }

    public class SitInteraction : LifeInteraction
    {
        public override string Name => "Sit";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || !LifeOrder.IsSeat(Context.Box))
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            LifeSimMap Map = Context.Map;
            Choices.Add(new InteractionChoice { Label = "Sit", Order = 20, Run = () => LifeOrder.Sit(Who, Box, Map) });
        }
    }

    public class EatInteraction : LifeInteraction
    {
        public override string Name => "Eat";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null)
            {
                return;
            }

            string Item = Context.Box.Interaction;
            if (Item != "Fridge" && Item != "Kitchen Fridge")
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            LifeSimMap Map = Context.Map;
            Choices.Add(new InteractionChoice { Label = "Eat", Order = 40, Run = () => LifeOrder.Fridge(Who, Box, Map) });
        }
    }

    public class CookInteraction : LifeInteraction
    {
        public override string Name => "Cook";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || Context.Box.Interaction != "Counter")
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            HouseholdState Home = Context.Home;
            Choices.Add(new InteractionChoice { Label = "Cook", Order = 41, Run = () => LifeOrder.Cook(Who, Home) });
        }
    }

    public class WorkInteraction : LifeInteraction
    {
        public override string Name => "Work";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || !Works(Context.Box.Interaction))
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            HouseholdState Home = Context.Home;
            Choices.Add(new InteractionChoice { Label = "Work", Order = 50, Run = () => DungeonOrders.WorkHere(Who, Box, Home) });
        }

        static bool Works(string Item)
        {
            return Item == "Garden Plot" || Item == "Fishing Spot" || Item == "Forge" || Item == "Alchemy Bench" || Item == "Ore Rock";
        }
    }

    public class PracticeInteraction : LifeInteraction
    {
        public override string Name => "Practice";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || Context.Box.Interaction != PracticeEncounter.Interaction)
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            LifeSimMap Map = Context.Map;
            PlaySession Session = Context.Session;
            Choices.Add(new InteractionChoice { Label = "Practice", Order = 60, Run = () => LifeOrder.Practice(Who, Box, Map, Session) });
        }
    }

    public class DungeonGateInteraction : LifeInteraction
    {
        public override string Name => "DungeonGate";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || Context.Box.Interaction != "Dungeon Gate" || Context.EnterDungeon == null)
            {
                return;
            }

            System.Action Enter = Context.EnterDungeon;
            Choices.Add(new InteractionChoice { Label = "Enter", Order = 70, Run = Enter });
        }
    }
}
