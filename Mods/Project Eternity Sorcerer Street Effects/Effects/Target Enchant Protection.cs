using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TargetEnchantProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Target Enchant Protection";

        public TargetEnchantProtectionEffect()
            : base(Name, false)
        {
        }

        public TargetEnchantProtectionEffect(SorcererStreetBattleParams Params)
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
            TargetEnchantProtectionEffect NewEffect = new TargetEnchantProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
