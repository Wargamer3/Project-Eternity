﻿using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord0Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 0";

        public EnchantHolyWord0Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord0Effect(SorcererStreetBattleParams Params)
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
            SetDiceValueEffect NewHolyWord0Effect = new SetDiceValueEffect(Params, 0);
            NewHolyWord0Effect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewHolyWord0Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord0Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 0";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord0Effect NewEffect = new EnchantHolyWord0Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
