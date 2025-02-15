using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantAntiElementEffect : SorcererStreetEffect
    {//Target creature cannot receive land effect.
        public static string Name = "Sorcerer Street Enchant Anti-Element";

        public EnchantAntiElementEffect()
            : base(Name, false)
        {
        }

        public EnchantAntiElementEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreatePassiveEnchant(Name, new LandBonusLimitEffect(Params), IconHolder.Icons.sprCreatureAntiElement);
            return "Anti-Element";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantAntiElementEffect NewEffect = new EnchantAntiElementEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
