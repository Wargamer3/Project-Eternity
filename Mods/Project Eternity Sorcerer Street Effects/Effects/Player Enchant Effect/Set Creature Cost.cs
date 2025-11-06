using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetCreatureCostEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Creature Cost";

        private float CreatuteCostMultiplier;

        public SetCreatureCostEffect()
            : base(Name, false)
        {
        }

        public SetCreatureCostEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetCreatureCostEffect(SorcererStreetBattleParams Params, float CreatuteCostMultiplier)
            : base(Name, false, Params)
        {
            this.CreatuteCostMultiplier = CreatuteCostMultiplier;
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).CreatureCostMultiplier = CreatuteCostMultiplier;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            SetCreatureCostEffect NewEffect = new SetCreatureCostEffect(Params, CreatuteCostMultiplier);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetCreatureCostEffect SetDiceValueEffectCopy = (SetCreatureCostEffect)Copy;
            CreatuteCostMultiplier = SetDiceValueEffectCopy.CreatuteCostMultiplier;
        }
    }

}
