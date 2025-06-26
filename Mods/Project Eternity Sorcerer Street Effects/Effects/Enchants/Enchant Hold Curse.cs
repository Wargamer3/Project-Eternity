using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player's territories with Enchantments cannot be targeted by Enchantment spells or Enchantment territory abilities for 8 rounds.
    public sealed class EnchantHoldCurseEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Hold Curse";

        public EnchantHoldCurseEffect()
            : base(Name, false)
        {
        }

        public EnchantHoldCurseEffect(SorcererStreetBattleParams Params)
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
            EnchantProtectionPlayerEffect NewEnchantProtectionEffect = new EnchantProtectionPlayerEffect(Params);
            NewEnchantProtectionEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewEnchantProtectionEffect.Lifetime[0].LifetimeTypeValue = 8;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewEnchantProtectionEffect, IconHolder.Icons.sprPlayerHoldCurse);
            return "Hold Curse";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHoldCurseEffect NewEffect = new EnchantHoldCurseEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
