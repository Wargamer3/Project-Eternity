using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantSleepEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Sleep";

        public EnchantSleepEffect()
            : base(Name, false)
        {
        }

        public EnchantSleepEffect(SorcererStreetBattleParams Params)
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
            return "Sleep";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSleepEffect NewEffect = new EnchantSleepEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
