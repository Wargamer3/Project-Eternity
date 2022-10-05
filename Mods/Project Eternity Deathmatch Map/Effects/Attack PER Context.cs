using System;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class AttackPERContext : Projectile3DContext
    {
        public Squad Owner;
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
