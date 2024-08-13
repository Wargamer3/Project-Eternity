using System;
using ProjectEternity.Core;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class AttackPERContext : Projectile3DContext
    {
        public UnitConquest Owner;
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
