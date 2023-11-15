using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GlobalAbilityDisableEndBattleAbilitiesEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Disable End Battle Abilities";

        public GlobalAbilityDisableEndBattleAbilitiesEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDisableEndBattleAbilitiesEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityDisableEndBattleAbilitiesEffect NewEffect = new GlobalAbilityDisableEndBattleAbilitiesEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
