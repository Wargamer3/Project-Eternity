using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TollImmunityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Toll Immunity";

        public TollImmunityEffect()
            : base(Name, false)
        {
        }

        public TollImmunityEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).TollProtection = true;

            return "Immune to tolls";
        }

        protected override BaseEffect DoCopy()
        {
            TollImmunityEffect NewEffect = new TollImmunityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties


        #endregion
    }
}
