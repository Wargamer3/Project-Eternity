using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target territory cannot be invaded. / Target territory's toll value becomes 0.
    public sealed class EnchantPeaceEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Peace";

        public EnchantPeaceEffect()
            : base(Name, false)
        {
        }

        public EnchantPeaceEffect(SorcererStreetBattleParams Params)
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
            return "Peace";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantPeaceEffect NewEffect = new EnchantPeaceEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
