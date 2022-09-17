using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class Projectile3DParams
    {
        public class SharedProjectileParams
        {
            public Vector3 OwnerPosition;
            public Vector3 OwnerAngle;
            public ContentManager Content;
        }

        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        protected readonly Projectile3DContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly Projectile3DContext LocalContext;

        //Global params that can't be cached in the LocalContext.
        public readonly SharedProjectileParams SharedParams;

        public Projectile3DParams(Projectile3DContext GlobalContext)
            : this(GlobalContext, new Projectile3DContext(), new SharedProjectileParams())
        {
        }

        public Projectile3DParams(Projectile3DParams Clone)
            : this(Clone.GlobalContext)
        {
            SharedParams = Clone.SharedParams;
        }

        protected Projectile3DParams(Projectile3DContext GlobalContext, Projectile3DContext LocalContext, SharedProjectileParams SharedParams)
        {
            this.GlobalContext = GlobalContext;
            this.LocalContext = LocalContext;
            this.SharedParams = SharedParams;

            LocalContext.OwnerSandbox = GlobalContext.OwnerSandbox;
            LocalContext.OwnerProjectile = GlobalContext.OwnerProjectile;
        }
    }
}
