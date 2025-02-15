using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target enemy Player cannot claim tolls for 2 rounds.
    public sealed class EnchantBlackoutEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Blackout";

        private sealed class BlackoutEffect : SorcererStreetEffect
        {
            public BlackoutEffect(SorcererStreetBattleParams Params)
                : base("Sorcerer Street Enchant Blackout", false, Params)
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
                Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollLimit = true;
                return "Blackout";
            }

            protected override BaseEffect DoCopy()
            {
                BlackoutEffect NewEffect = new BlackoutEffect(Params);

                return NewEffect;
            }

            protected override void DoCopyMembers(BaseEffect Copy)
            {
            }
        }
        public EnchantBlackoutEffect()
            : base(Name, false)
        {
        }

        public EnchantBlackoutEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, new BlackoutEffect(Params), IconHolder.Icons.sprPlayerToll);
            return "Blackout";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantBlackoutEffect NewEffect = new EnchantBlackoutEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
