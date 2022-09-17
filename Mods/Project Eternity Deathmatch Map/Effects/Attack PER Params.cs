using System;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class AttackPERParams : Projectile3DParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        private readonly new AttackPERContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly new AttackPERContext LocalContext;

        public AttackPERParams(AttackPERContext GlobalContext)
            : this(GlobalContext, new AttackPERContext(), new SharedProjectileParams())
        {
        }

        private AttackPERParams(AttackPERContext GlobalContext, AttackPERContext LocalContext, SharedProjectileParams SharedParams)
            : base(GlobalContext, LocalContext, SharedParams)
        {
            this.GlobalContext = GlobalContext;
            this.LocalContext = LocalContext;
        }

        public AttackPERParams(AttackPERParams Clone)
            : this(Clone.GlobalContext, new AttackPERContext(), Clone.SharedParams)
        {
            LocalContext.Owner = GlobalContext.Owner;
            LocalContext.Map = GlobalContext.Map;
        }
    }
}
