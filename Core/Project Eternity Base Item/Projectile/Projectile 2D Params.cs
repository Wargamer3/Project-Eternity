using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class Projectile2DParams
    {
        public class SharedProjectileParams
        {
            public Vector2 OwnerPosition;
            public float OwnerAngle;
            public ContentManager Content;
        }

        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        protected readonly Projectile2DContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly Projectile2DContext LocalContext;

        //Global params that can't be cached in the LocalContext.
        public readonly SharedProjectileParams SharedParams;

        public Projectile2DParams(Projectile2DContext GlobalContext)
            : this(GlobalContext, new Projectile2DContext(), new SharedProjectileParams())
        {
        }

        public Projectile2DParams(Projectile2DParams Clone)
            : this(Clone.GlobalContext)
        {
            SharedParams = Clone.SharedParams;
        }

        protected Projectile2DParams(Projectile2DContext GlobalContext, Projectile2DContext LocalContext, SharedProjectileParams SharedParams)
        {
            this.GlobalContext = GlobalContext;
            this.LocalContext = LocalContext;
            this.SharedParams = SharedParams;

            LocalContext.OwnerSandbox = GlobalContext.OwnerSandbox;
            LocalContext.OwnerProjectile = GlobalContext.OwnerProjectile;
        }
    }
}
