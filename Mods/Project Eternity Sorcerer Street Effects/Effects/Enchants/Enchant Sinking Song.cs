using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: Deals only 1/2 damage to all creatures for 3 rounds.
    public sealed class EnchantSinkingSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Sinking Song";

        public EnchantSinkingSongEffect()
            : base(Name, false)
        {
        }

        public EnchantSinkingSongEffect(SorcererStreetBattleParams Params)
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
            ChangeDamageEffect NewChangeDamageEffect = new ChangeDamageEffect(Params);
            NewChangeDamageEffect.Target = ChangeDamageEffect.Targets.Opponent;
            NewChangeDamageEffect.Value = "0.5";
            NewChangeDamageEffect.SignOperator = Core.Operators.NumberTypes.Relative;

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, NewChangeDamageEffect, IconHolder.Icons.sprPlayerSinkingSong);
            return "Sinking Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSinkingSongEffect NewEffect = new EnchantSinkingSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
