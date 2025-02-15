using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Creature takes additional damage at the end of battle. (50% MHP)
    public sealed class EnchantPoisonEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Poison";

        public EnchantPoisonEffect()
            : base(Name, false)
        {
        }

        public EnchantPoisonEffect(SorcererStreetBattleParams Params)
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
            DealDamageEffect NewDealDamageEffect = new DealDamageEffect(Params);
            NewDealDamageEffect.DamageToDeal = "opponent.maxhp/2";

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetBattleEndRequirement(), NewDealDamageEffect, IconHolder.Icons.sprCreaturePoison);
            return "Poison";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantPoisonEffect NewEffect = new EnchantPoisonEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
