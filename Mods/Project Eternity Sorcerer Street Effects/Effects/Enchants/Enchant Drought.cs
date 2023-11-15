using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Level of target Player's lands cannot be changed for 3 rounds.
    public sealed class EnchantDroughtEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Drought";

        public EnchantDroughtEffect()
            : base(Name, false)
        {
        }

        public EnchantDroughtEffect(SorcererStreetBattleParams Params)
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
            return "Drought";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantDroughtEffect NewEffect = new EnchantDroughtEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
