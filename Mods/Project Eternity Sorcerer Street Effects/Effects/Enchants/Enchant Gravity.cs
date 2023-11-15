using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//All Players' next die roll yields a 1.
    public sealed class EnchantGravityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Gravity";

        public EnchantGravityEffect()
            : base(Name, false)
        {
        }

        public EnchantGravityEffect(SorcererStreetBattleParams Params)
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
            return "Gravity";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantGravityEffect NewEffect = new EnchantGravityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
