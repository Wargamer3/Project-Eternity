using System;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class SquadPERParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly SquadPERContext GlobalContext;

        public SquadPERParams(SquadPERContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
        }

    }
}
