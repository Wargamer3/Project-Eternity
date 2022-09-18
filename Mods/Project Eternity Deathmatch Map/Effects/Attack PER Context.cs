using System;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class AttackPERContext : Projectile3DContext
    {
        public DeathmatchMap Map;
        public Squad Owner;
        public new PERAttack OwnerProjectile;
    }
}
