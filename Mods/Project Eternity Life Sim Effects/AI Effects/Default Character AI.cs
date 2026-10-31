using System.Runtime.CompilerServices;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class DefaultCharacterAI
    {
        public static void Assign(PlayerCharacter Character)
        {
            if (Character == null || Character.HasAnyScript())
            {
                return;
            }

            Add(Character, new WalkToPositionAction());
            Add(Character, new RunToPositionAction());
            Add(Character, new TiptoeAction());
            Add(Character, new DriveAction());
            Add(Character, new EatJerkyAction());
            Add(Character, new GoToRestaurantAction());
            Add(Character, new GoThroughDoorAction());
            Add(Character, new LockDoorAction());
            Add(Character, new UnlockDoorAction());
        }

        static void Add(PlayerCharacter Character, LifeSimAIAction Action)
        {
            Action.Init(Character.SharedMapContex);
            Character.AddAction(Action);
        }
    }

    static class DefaultAIStartup
    {
        [ModuleInitializer]
        internal static void Start()
        {
            DefaultAI.Fill = DefaultCharacterAI.Assign;
        }
    }
}
