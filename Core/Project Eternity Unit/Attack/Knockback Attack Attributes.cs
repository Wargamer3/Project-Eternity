using System;
using System.IO;

namespace ProjectEternity.Core.Attacks
{
    public struct KnockbackAttackAttributes
    {
        public byte EnemyKnockback;
        public byte SelfKnockback;

        public KnockbackAttackAttributes(BinaryReader BR)
        {
            EnemyKnockback = BR.ReadByte();
            SelfKnockback = BR.ReadByte();
        }
    }
}
