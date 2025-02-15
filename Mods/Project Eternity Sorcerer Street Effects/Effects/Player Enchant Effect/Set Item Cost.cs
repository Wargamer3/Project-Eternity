using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SetItemCostEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Set Item Cost";

        private float ItemCostMultiplier;

        public SetItemCostEffect()
            : base(Name, false)
        {
        }

        public SetItemCostEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        public SetItemCostEffect(SorcererStreetBattleParams Params, float CreatuteCostMultiplier)
            : base(Name, false, Params)
        {
            this.ItemCostMultiplier = CreatuteCostMultiplier;
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
            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).ItemCostMultiplier = ItemCostMultiplier;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            SetItemCostEffect NewEffect = new SetItemCostEffect(Params, ItemCostMultiplier);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SetItemCostEffect SetDiceValueEffectCopy = (SetItemCostEffect)Copy;
            ItemCostMultiplier = SetDiceValueEffectCopy.ItemCostMultiplier;
        }
    }

}
