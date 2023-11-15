using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GlobalAbilityDisableCopySummonEffect : SorcererStreetEffect
    {//Creatures of the same type as those already in play cannot be summoned.
        public static string Name = "Sorcerer Street Global Ability Disable Copy Summon";

        public GlobalAbilityDisableCopySummonEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableCopySummonEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            GlobalAbilityDisableCopySummonEffect NewEffect = new GlobalAbilityDisableCopySummonEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
