using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//MHP+20 to target creature (up to a maximum of 100).
    public sealed class EnchantMutationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Mutation";

        public EnchantMutationEffect()
            : base(Name, false)
        {
        }

        public EnchantMutationEffect(SorcererStreetBattleParams Params)
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
            return "Mutation";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantMutationEffect NewEffect = new EnchantMutationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
