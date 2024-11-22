using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord1Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 1";

        private sealed class HolyWord1Effect : SorcererStreetEffect
        {
            public HolyWord1Effect(SorcererStreetBattleParams Params)
                : base("Sorcerer Street Holy Word 1", false, Params)
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
                Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).DiceValue = 1;
                return "Holy Word 1";
            }

            protected override BaseEffect DoCopy()
            {
                HolyWord1Effect NewEffect = new HolyWord1Effect(Params);

                return NewEffect;
            }

            protected override void DoCopyMembers(BaseEffect Copy)
            {
            }
        }

        public EnchantHolyWord1Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord1Effect(SorcererStreetBattleParams Params)
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
            //Play Activation Animation

            //Enchant Player
            Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].Enchant = EnchantHelper.CreateEnchant(Name, new HolyWord1Effect(Params), IconHolder.Icons.sprPlayerHaste);
            return "Holy Word 1";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord1Effect NewEffect = new EnchantHolyWord1Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
