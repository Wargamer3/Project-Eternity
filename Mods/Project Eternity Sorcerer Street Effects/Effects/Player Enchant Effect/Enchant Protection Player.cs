using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player's territories with Enchantments cannot be targeted by Enchantment spells or Enchantment territory 
    public sealed class EnchantProtectionPlayerEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Protection Player";

        public EnchantProtectionPlayerEffect()
            : base(Name, false)
        {
        }

        public EnchantProtectionPlayerEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).CreatureEnchantProtection = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantProtectionPlayerEffect NewEffect = new EnchantProtectionPlayerEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
