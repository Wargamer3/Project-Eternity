using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Attacks
{
    public abstract class SquadPEREffect : BaseEffect
    {
        public SquadPEREffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
        }
    }
}
