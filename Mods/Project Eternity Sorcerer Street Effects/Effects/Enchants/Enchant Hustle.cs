﻿using System;
using System.IO;
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
