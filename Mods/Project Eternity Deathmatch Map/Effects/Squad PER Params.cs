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
        private readonly SquadPERContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly SquadPERContext LocalContext;

        public SquadPERParams(SquadPERContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
            LocalContext = new SquadPERContext();
        }

        public SquadPERParams(SquadPERParams Clone)
            : this(Clone.GlobalContext)
        {
            LocalContext.Map = GlobalContext.Map;
            LocalContext.Target = GlobalContext.Target;
            LocalContext.TargetWeapon = GlobalContext.TargetWeapon;
            LocalContext.TargetWeaponAngle = GlobalContext.TargetWeaponAngle;
            LocalContext.TargetWeaponPosition = GlobalContext.TargetWeaponPosition;
        }
    }
}
