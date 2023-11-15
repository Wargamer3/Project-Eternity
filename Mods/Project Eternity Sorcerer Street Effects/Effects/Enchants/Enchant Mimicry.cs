using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature receives land effect from lands of any element.
    public sealed class EnchantMimicryEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Mimicry";

        public EnchantMimicryEffect()
            : base(Name, false)
        {
        }

        public EnchantMimicryEffect(SorcererStreetBattleParams Params)
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
            EnchantMimicryEffect NewEffect = new EnchantMimicryEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
