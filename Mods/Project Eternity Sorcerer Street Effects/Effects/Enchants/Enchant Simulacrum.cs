using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature neutralizes all non-scroll attack damage in battle. User loses (amount of neutralized damage x3G) magic.
    public sealed class EnchantSimulacrumEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Simulacrum";

        public EnchantSimulacrumEffect()
            : base(Name, false)
        {
        }

        public EnchantSimulacrumEffect(SorcererStreetBattleParams Params)
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
            return "Simulacrum";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSimulacrumEffect NewEffect = new EnchantSimulacrumEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
