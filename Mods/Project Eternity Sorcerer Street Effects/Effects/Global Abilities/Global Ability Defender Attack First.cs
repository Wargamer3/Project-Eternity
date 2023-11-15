using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, all defending creatures gain Attacks First ability.
    public sealed class GlobalAbilityDefenderAttackFirstEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Defender Attack First";

        public GlobalAbilityDefenderAttackFirstEffect()
            : base(Name, false)
        {
        }

        public GlobalAbilityDefenderAttackFirstEffect(SorcererStreetBattleParams Params)
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
            GlobalAbilityDefenderAttackFirstEffect NewEffect = new GlobalAbilityDefenderAttackFirstEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
