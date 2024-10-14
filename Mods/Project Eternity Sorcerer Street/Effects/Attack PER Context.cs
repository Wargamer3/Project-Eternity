using System;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class AttackPERContext : Projectile3DContext
    {
        public SorcererStreetUnit Owner;
        public new PERAttack OwnerProjectile;

        public AttackPERContext()
        {
        }

        public AttackPERContext(AttackPERContext GlobalContext)
        {
            Owner = GlobalContext.Owner;
            OwnerProjectile = GlobalContext.OwnerProjectile;
            OwnerSandbox = GlobalContext.OwnerSandbox;
        }
    }
}
