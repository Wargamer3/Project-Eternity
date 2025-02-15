using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: ST+20 for 4 rounds.
    public sealed class EnchantBraveSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Brave Song";

        public EnchantBraveSongEffect()
            : base(Name, false)
        {
        }

        public EnchantBraveSongEffect(SorcererStreetBattleParams Params)
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
            IncreaseSTEffect.Value = "20";
            IncreaseSTEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            IncreaseSTEffect.Lifetime[0].LifetimeTypeValue = 4;

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateBattleEnchant(Name, IncreaseSTEffect, IconHolder.Icons.sprPlayerBraveSong);
            return "Brave Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantBraveSongEffect NewEffect = new EnchantBraveSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
