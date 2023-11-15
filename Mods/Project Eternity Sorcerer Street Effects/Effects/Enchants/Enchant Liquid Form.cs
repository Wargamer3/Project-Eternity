using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, target creature's ST & HP= (random value between 10-70).
    public sealed class EnchantLiquidFormEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Liquid Form";

        public EnchantLiquidFormEffect()
            : base(Name, false)
        {
        }

        public EnchantLiquidFormEffect(SorcererStreetBattleParams Params)
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
            return "Liquid Form";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantLiquidFormEffect NewEffect = new EnchantLiquidFormEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
