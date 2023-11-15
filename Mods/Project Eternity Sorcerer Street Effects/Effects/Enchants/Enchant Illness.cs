using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, ST & HP-20 to target creature.
    public sealed class EnchantIllnessEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Illness";

        public EnchantIllnessEffect()
            : base(Name, false)
        {
        }

        public EnchantIllnessEffect(SorcererStreetBattleParams Params)
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
            return "Illness";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantIllnessEffect NewEffect = new EnchantIllnessEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
