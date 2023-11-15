﻿using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature's HP and MHP cannot be altered by spells or territory abilities. / Territory level cannot be lowered.
    public sealed class EnchantHolyAsylumEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Asylum";

        public EnchantHolyAsylumEffect()
            : base(Name, false)
        {
        }

        public EnchantHolyAsylumEffect(SorcererStreetBattleParams Params)
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
            return "Holy Asylum";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyAsylumEffect NewEffect = new EnchantHolyAsylumEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
