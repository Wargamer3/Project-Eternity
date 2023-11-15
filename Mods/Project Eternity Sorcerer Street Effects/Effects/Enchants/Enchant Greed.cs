using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Raises target territory's toll value to 1.5x. / In Battle: HP-10
    public sealed class EnchantGreedEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Greed";

        public EnchantGreedEffect()
            : base(Name, false)
        {
        }

        public EnchantGreedEffect(SorcererStreetBattleParams Params)
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
            EnchantGreedEffect NewEffect = new EnchantGreedEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
