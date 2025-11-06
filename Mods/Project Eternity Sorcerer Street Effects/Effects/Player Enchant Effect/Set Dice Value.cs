using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetDiceValueEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Dice Value";

        private int DiceValue;

        public SetDiceValueEffect()
            : base(Name, false)
        {
        }

        public SetDiceValueEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetDiceValueEffect(SorcererStreetBattleParams Params, int DiceValue)
            : base(Name, false, Params)
        {
            this.DiceValue = DiceValue;
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).DiceValue = DiceValue;
            return "Set Dice Value";
        }

        protected override BaseEffect DoCopy()
        {
            SetDiceValueEffect NewEffect = new SetDiceValueEffect(Params, DiceValue);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetDiceValueEffect SetDiceValueEffectCopy = (SetDiceValueEffect)Copy;
            DiceValue = SetDiceValueEffectCopy.DiceValue;
        }
    }

}
