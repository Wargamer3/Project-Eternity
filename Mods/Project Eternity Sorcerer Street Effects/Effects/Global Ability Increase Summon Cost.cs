using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Cards of rarity S require 1.5x magic to use. Cards of rarity R and E require double.
    public sealed class GlobalAbilityIncreaseSummonCostEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Increase Summon Cost";

        public GlobalAbilityIncreaseSummonCostEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityIncreaseSummonCostEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityIncreaseSummonCostEffect NewEffect = new GlobalAbilityIncreaseSummonCostEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
