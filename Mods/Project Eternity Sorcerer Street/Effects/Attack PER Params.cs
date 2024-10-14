using System;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class AttackPERParams : Projectile3DParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly new AttackPERContext GlobalContext;

        public AttackPERParams(AttackPERContext GlobalContext)
            : this(GlobalContext, new AttackPERContext(), new SharedProjectileParams())
        {
        }

        private AttackPERParams(AttackPERContext GlobalContext, AttackPERContext LocalContext, SharedProjectileParams SharedParams)
            : base(GlobalContext, LocalContext, SharedParams)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
