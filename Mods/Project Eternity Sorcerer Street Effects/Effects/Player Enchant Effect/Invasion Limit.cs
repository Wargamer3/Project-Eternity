using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //Cannot invade enemy territories.
    public sealed class InvasionLimitEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Invasion Limit";

        public InvasionLimitEffect()
            : base(Name, false)
        {
        }

        public InvasionLimitEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).InvasionLimit = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            InvasionLimitEffect NewEffect = new InvasionLimitEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
