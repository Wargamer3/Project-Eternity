using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Creature can battle, but cannot collect tolls.
    public sealed class EnchantSleepEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Sleep";

        public EnchantSleepEffect()
            : base(Name, false)
        {
        }

        public EnchantSleepEffect(SorcererStreetBattleParams Params)
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
            ChangeTollEffect NewChangeTollEffect = new ChangeTollEffect(Params);
            NewChangeTollEffect.SignOperator = Core.Operators.SignOperators.Equal;
            NewChangeTollEffect.Value = "0";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewChangeTollEffect, IconHolder.Icons.sprCreatureSleep);
            return "Sleep";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSleepEffect NewEffect = new EnchantSleepEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
