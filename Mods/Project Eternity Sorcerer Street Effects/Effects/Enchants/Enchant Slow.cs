﻿using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Locks target Player's die roll to 1-3 for 2 rounds.
    public sealed class EnchantSlowEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Slow";

        public EnchantSlowEffect()
            : base(Name, false)
        {
        }

        public EnchantSlowEffect(SorcererStreetBattleParams Params)
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
            return "Slow";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSlowEffect NewEffect = new EnchantSlowEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
