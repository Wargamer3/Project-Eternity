using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantAntiMagicEffect : SorcererStreetEffect
    {//Neutralizes the next spell or territory ability that targets target Player (except those that dispel Enchantments).
        public static string Name = "Sorcerer Street Enchant Anti-Magic";

        public EnchantAntiMagicEffect()
            : base(Name, false)
        {
        }

        public EnchantAntiMagicEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateBattleEnchant(Name, new NeutralizeNextSpellEffect(Params), IconHolder.Icons.sprPlayerAntiMagic);
            return "Anti-Magic";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantAntiMagicEffect NewEffect = new EnchantAntiMagicEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
