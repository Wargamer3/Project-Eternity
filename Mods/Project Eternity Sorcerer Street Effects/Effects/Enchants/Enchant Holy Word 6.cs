using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord6Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 6";

        public EnchantHolyWord6Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord6Effect(SorcererStreetBattleParams Params)
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
            return "Holy Word 6";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord6Effect NewEffect = new EnchantHolyWord6Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
