using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class SleepInBedInteraction : LifeInteraction
    {
        public override string Name => "SleepInBedAction";

        public override void Offer(InteractionContext Context, List<InteractionChoice> Choices)
        {
            if (Context == null || Context.Box == null || Context.Other != null || !LifeOrder.IsBed(Context.Box))
            {
                return;
            }

            PlayerCharacter Who = Context.Actor;
            FurnitureItem Box = Context.Box;
            LifeSimMap Map = Context.Map;
            Choices.Add(new InteractionChoice { Label = "Sleep", Order = 10, Run = () => LifeOrder.Bed(Who, Box, Map) });
        }
    }

    public class SleepInBedAction : LifeSimStep
    {
        readonly int Comfort;

        public SleepInBedAction(PlayerCharacter Owner)
            : this(Owner, 0)
        {
        }

        public SleepInBedAction(PlayerCharacter Owner, int Comfort)
            : base("Sleep", Owner, 0.4)
        {
            this.Comfort = Comfort;
        }

        protected override void OnFinished()
        {
            Resting.Sleep(Owner.Life, Comfort, Owner.SimHour, Owner);
            Owner.DayAttackBonus = Owner.Life.AttackNow(Owner.SimHour);
            Owner.DayCraftBonus = Owner.Life.CraftNow(Owner.SimHour);
        }

        protected override AutomatedAction Copy()
        {
            return new SleepInBedAction(Owner, Comfort);
        }
    }
}
