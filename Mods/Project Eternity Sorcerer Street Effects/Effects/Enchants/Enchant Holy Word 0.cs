using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord0Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 0";

        public EnchantHolyWord0Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord0Effect(SorcererStreetBattleParams Params)
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
            return "Holy Word 0";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord0Effect NewEffect = new EnchantHolyWord0Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
