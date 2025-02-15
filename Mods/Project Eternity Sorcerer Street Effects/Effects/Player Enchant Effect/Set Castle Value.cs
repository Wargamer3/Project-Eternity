using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetCastleValueEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Castle Value";

        private float CastleValueMultiplier;

        public SetCastleValueEffect()
            : base(Name, false)
        {
        }

        public SetCastleValueEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetCastleValueEffect(SorcererStreetBattleParams Params, float DiceValue)
            : base(Name, false, Params)
        {
            this.CastleValueMultiplier = DiceValue;
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).CastleValueMultiplier = CastleValueMultiplier;
            return "Set Castle Value";
        }

        protected override BaseEffect DoCopy()
        {
            SetCastleValueEffect NewEffect = new SetCastleValueEffect(Params, CastleValueMultiplier);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetCastleValueEffect SetDiceValueEffectCopy = (SetCastleValueEffect)Copy;
            CastleValueMultiplier = SetDiceValueEffectCopy.CastleValueMultiplier;
        }
    }

}
