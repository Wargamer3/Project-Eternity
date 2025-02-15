using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//	Decreases target territory's toll value by 1/2.
    public sealed class EnchantFogEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Fog";

        public EnchantFogEffect()
            : base(Name, false)
        {
        }

        public EnchantFogEffect(SorcererStreetBattleParams Params)
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
            NewChangeTollEffect.SignOperator = Core.Operators.SignOperators.DividedEqual;
            NewChangeTollEffect.Value = "2";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewChangeTollEffect, IconHolder.Icons.sprCreatureFog);
            return "Fog";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantFogEffect NewEffect = new EnchantFogEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
