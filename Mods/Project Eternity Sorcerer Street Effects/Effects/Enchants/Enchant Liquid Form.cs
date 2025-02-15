using System;
using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, target creature's ST & HP= (random value between 10-70).
    public sealed class EnchantLiquidFormEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Liquid Form";

        public EnchantLiquidFormEffect()
            : base(Name, false)
        {
        }

        public EnchantLiquidFormEffect(SorcererStreetBattleParams Params)
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
            ChangeStatsEffect IncreaseSTEffect = new ChangeStatsEffect(Params);
            IncreaseSTEffect.Target = ChangeStatsEffect.Targets.Self;
            IncreaseSTEffect.Stat = ChangeStatsEffect.Stats.FinalST;
            IncreaseSTEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            IncreaseSTEffect.Value = "10+random.60";

            ChangeStatsEffect IncreaseHPEffect = new ChangeStatsEffect(Params);
            IncreaseHPEffect.Target = ChangeStatsEffect.Targets.Self;
            IncreaseHPEffect.Stat = ChangeStatsEffect.Stats.FinalHP;
            IncreaseHPEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            IncreaseHPEffect.Value = "10+random.60";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, new List<BaseEffect>() { IncreaseSTEffect, IncreaseHPEffect }, IconHolder.Icons.sprCreatureLiquidForm);
            return "Liquid Form";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantLiquidFormEffect NewEffect = new EnchantLiquidFormEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
