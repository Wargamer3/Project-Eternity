﻿using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DisableLapRegenerationEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Disable Lap Regeneration";

        public DisableLapRegenerationEffect()
            : base(Name, false)
        {
        }

        public DisableLapRegenerationEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).LapRegenerationLimit = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            DisableLapRegenerationEffect NewEffect = new DisableLapRegenerationEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
