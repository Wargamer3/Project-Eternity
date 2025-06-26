using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: HP+20 for 4 rounds.
    public sealed class EnchantToughSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Tough Song";

        public EnchantToughSongEffect()
            : base(Name, false)
        {
        }

        public EnchantToughSongEffect(SorcererStreetBattleParams Params)
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
            ChangeStatsEffect IncreaseHPEffect = new ChangeStatsEffect(Params);
            IncreaseHPEffect.Target = ChangeStatsEffect.Targets.Self;
            IncreaseHPEffect.Stat = ChangeStatsEffect.Stats.FinalHP;
            IncreaseHPEffect.SignOperator = Core.Operators.SignOperators.PlusEqual;
            IncreaseHPEffect.Value = "20";
            IncreaseHPEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            IncreaseHPEffect.Lifetime[0].LifetimeTypeValue = 4;

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, IncreaseHPEffect, IconHolder.Icons.sprPlayerToughSong);
            return "Tough Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantToughSongEffect NewEffect = new EnchantToughSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
