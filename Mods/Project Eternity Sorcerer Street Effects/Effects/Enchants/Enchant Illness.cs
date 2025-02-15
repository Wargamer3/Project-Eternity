using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//In battle, ST & HP-20 to target creature.
    public sealed class EnchantIllnessEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Illness";

        public EnchantIllnessEffect()
            : base(Name, false)
        {
        }

        public EnchantIllnessEffect(SorcererStreetBattleParams Params)
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
            ChangeStatsEffect DecreaseSTEffect = new ChangeStatsEffect(Params);
            DecreaseSTEffect.Target = ChangeStatsEffect.Targets.Self;
            DecreaseSTEffect.Stat = ChangeStatsEffect.Stats.FinalST;
            DecreaseSTEffect.SignOperator = Core.Operators.SignOperators.MinusEqual;
            DecreaseSTEffect.Value = "320";

            ChangeStatsEffect DecreaseHPEffect = new ChangeStatsEffect(Params);
            DecreaseHPEffect.Target = ChangeStatsEffect.Targets.Self;
            DecreaseHPEffect.Stat = ChangeStatsEffect.Stats.FinalHP;
            DecreaseHPEffect.SignOperator = Core.Operators.SignOperators.MinusEqual;
            DecreaseHPEffect.Value = "20";

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateBattleEnchant(Name, new List<BaseEffect>() { DecreaseSTEffect, DecreaseHPEffect }, IconHolder.Icons.sprCreatureHustle);
            return "Illness";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantIllnessEffect NewEffect = new EnchantIllnessEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
