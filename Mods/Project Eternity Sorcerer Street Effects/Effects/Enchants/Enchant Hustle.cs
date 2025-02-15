using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, ST & HP+30 to target creature.
    public sealed class EnchantHustleEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Hustle";

        public EnchantHustleEffect()
            : base(Name, false)
        {
        }

        public EnchantHustleEffect(SorcererStreetBattleParams Params)
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
            IncreaseSTEffect.Value = "30";

            ChangeStatsEffect IncreaseHPEffect = new ChangeStatsEffect(Params);
            IncreaseHPEffect.Target = ChangeStatsEffect.Targets.Self;
            IncreaseHPEffect.Stat = ChangeStatsEffect.Stats.FinalHP;
            IncreaseHPEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            IncreaseHPEffect.Value = "30";

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateBattleEnchant(Name, new List<BaseEffect>() { IncreaseSTEffect, IncreaseHPEffect }, IconHolder.Icons.sprCreatureHustle);
            return "Hustle";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHustleEffect NewEffect = new EnchantHustleEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
