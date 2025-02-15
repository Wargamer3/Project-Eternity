using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{// Creature inflicts damage upon itself at the end of battle. You take 30% of the damage you do to your opponent.
    public sealed class EnchantConfusionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Confusion";

        public EnchantConfusionEffect()
            : base(Name, false)
        {
        }

        public EnchantConfusionEffect(SorcererStreetBattleParams Params)
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
            DealDamageSelfEffect NewDamageEffect = new DealDamageSelfEffect(Params);
            NewDamageEffect.DamageToDeal = "opponent.damagereceived*0.30";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetBattleEndRequirement(), NewDamageEffect, IconHolder.Icons.sprCreatureBulimia);
            return "Confusion";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantConfusionEffect NewEffect = new EnchantConfusionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
