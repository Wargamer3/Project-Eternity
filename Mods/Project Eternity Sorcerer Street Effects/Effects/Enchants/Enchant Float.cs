using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player's die roll yields a 5 for 2 rounds.
    public sealed class EnchantFloatEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Float";

        public EnchantFloatEffect()
            : base(Name, false)
        {
        }

        public EnchantFloatEffect(SorcererStreetBattleParams Params)
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
            //Play Activation Animation

            //Enchant Player
            SetDiceValueEffect NewFloatEffect = new SetDiceValueEffect(Params, 5);
            NewFloatEffect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewFloatEffect.Lifetime[0].LifetimeTypeValue = 2;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewFloatEffect, IconHolder.Icons.sprPlayerMovement);
            return "Float";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantFloatEffect NewEffect = new EnchantFloatEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
