using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GlobalAbilityAbilityDamageReducedEffect : SorcererStreetEffect
    {//Damage to all creatures from spells and territory abilities is reduced by 10.
        public static string Name = "Sorcerer Street Global Ability Ability Damage Reduced";

        public GlobalAbilityAbilityDamageReducedEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityAbilityDamageReducedEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityAbilityDamageReducedEffect NewEffect = new GlobalAbilityAbilityDamageReducedEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
