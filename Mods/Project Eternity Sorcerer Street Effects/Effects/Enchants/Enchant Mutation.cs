using System;
using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//MHP+20 to target creature (up to a maximum of 100). / Adds Poison.
    public sealed class EnchantMutationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Mutation";

        public EnchantMutationEffect()
            : base(Name, false)
        {
        }

        public EnchantMutationEffect(SorcererStreetBattleParams Params)
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
            EnchantPoisonEffect NewPoisonEffect = new EnchantPoisonEffect(Params);
            ChangeStatsEffect NewChangeStatsEffect = new ChangeStatsEffect(Params);
            NewChangeStatsEffect.Target = ChangeStatsEffect.Targets.Self;
            NewChangeStatsEffect.Stat = ChangeStatsEffect.Stats.MaxHP;
            NewChangeStatsEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            NewChangeStatsEffect.Value = "20";
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, new List<BaseEffect>() { NewPoisonEffect, NewChangeStatsEffect }, IconHolder.Icons.sprCreatureMimicry);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantMutationEffect NewEffect = new EnchantMutationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
