using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Prevents target enemy Player from using creature cards for 2 rounds.
    public sealed class EnchantSilenceEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Silence";

        public EnchantSilenceEffect()
            : base(Name, false)
        {
        }

        public EnchantSilenceEffect(SorcererStreetBattleParams Params)
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
            return "Silence";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSilenceEffect NewEffect = new EnchantSilenceEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
