using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player moves backward instead of forward upon rolling the die.
    public sealed class EnchantBackwardEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Backward";

        private sealed class BackwardEffect : SorcererStreetEffect
        {
            public BackwardEffect(SorcererStreetBattleParams Params)
                : base("Sorcerer Street Backward", false, Params)
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
                Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).Backward = true;
                return "Backward";
            }

            protected override BaseEffect DoCopy()
            {
                BackwardEffect NewEffect = new BackwardEffect(Params);

                return NewEffect;
            }

            protected override void DoCopyMembers(BaseEffect Copy)
            {
            }
        }

        public EnchantBackwardEffect()
            : base(Name, false)
        {
        }

        public EnchantBackwardEffect(SorcererStreetBattleParams Params)
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
            BackwardEffect NewBackwardEffect = new BackwardEffect(Params);
            NewBackwardEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewBackwardEffect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewBackwardEffect, IconHolder.Icons.sprPlayerMovement);
            return "Backward";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantBackwardEffect NewEffect = new EnchantBackwardEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
