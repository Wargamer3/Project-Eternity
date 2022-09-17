using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Attacks
{
    public abstract class AttackPEREffect : BaseEffect
    {
        public AttackPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
        }
    }
}
