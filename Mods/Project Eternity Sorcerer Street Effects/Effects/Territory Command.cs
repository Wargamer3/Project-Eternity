﻿using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TerritoryCommandEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Territory Command";

        public TerritoryCommandEffect()
            : base(Name, false)
        {
        }

        public TerritoryCommandEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).AllowTerrainCommands = true;

            return "Immune to tolls";
        }

        protected override BaseEffect DoCopy()
        {
            TerritoryCommandEffect NewEffect = new TerritoryCommandEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties


        #endregion
    }
}
