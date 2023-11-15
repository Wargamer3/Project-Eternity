using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord8Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 8";

        public EnchantHolyWord8Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord8Effect(SorcererStreetBattleParams Params)
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
            return "Holy Word 8";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord8Effect NewEffect = new EnchantHolyWord8Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
