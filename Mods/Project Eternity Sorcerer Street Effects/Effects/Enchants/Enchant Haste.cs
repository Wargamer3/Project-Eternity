using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Locks target Player's die roll to 6-8 for 2 rounds.
    public sealed class EnchantHasteEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Haste";

        public EnchantHasteEffect()
            : base(Name, false)
        {
        }

        public EnchantHasteEffect(SorcererStreetBattleParams Params)
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
            return "Haste";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHasteEffect NewEffect = new EnchantHasteEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
