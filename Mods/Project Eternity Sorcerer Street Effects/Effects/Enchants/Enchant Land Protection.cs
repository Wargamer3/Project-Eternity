using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target territory cannot be targeted by spells or territory abilities (except those that dispel Single Enchant effects).
    public sealed class EnchantLandProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Land Protection";

        public EnchantLandProtectionEffect()
            : base(Name, false)
        {
        }

        public EnchantLandProtectionEffect(SorcererStreetBattleParams Params)
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
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantLandProtectionEffect NewEffect = new EnchantLandProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
