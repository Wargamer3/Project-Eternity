using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//All creatures cannot use territory abilities.
    public sealed class GlobalAbilityDisableTerritoryAbilitiesEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Disable Territory Abilities";

        public GlobalAbilityDisableTerritoryAbilitiesEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableTerritoryAbilitiesEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityDisableTerritoryAbilitiesEffect NewEffect = new GlobalAbilityDisableTerritoryAbilitiesEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
