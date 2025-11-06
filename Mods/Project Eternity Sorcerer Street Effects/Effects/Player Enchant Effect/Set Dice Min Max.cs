using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetDiceMinMaxEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Dice Min Max Value";

        private int DiceMin;
        private int DiceMax;

        public SetDiceMinMaxEffect()
            : base(Name, false)
        {
        }

        public SetDiceMinMaxEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetDiceMinMaxEffect(SorcererStreetBattleParams Params, int DiceMin, int DiceMax)
            : base(Name, false, Params)
        {
            this.DiceMin = DiceMin;
            this.DiceMax = DiceMax;
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).DiceValueMin = DiceMin;
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).DiceValueMax = DiceMax;
            return "Set Dice Min Max Value";
        }

        protected override BaseEffect DoCopy()
        {
            SetDiceMinMaxEffect NewEffect = new SetDiceMinMaxEffect(Params, DiceMin, DiceMax);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetDiceMinMaxEffect SetDiceValueEffectCopy = (SetDiceMinMaxEffect)Copy;
            DiceMin = SetDiceValueEffectCopy.DiceMin;
            DiceMax = SetDiceValueEffectCopy.DiceMax;
        }
    }

}
