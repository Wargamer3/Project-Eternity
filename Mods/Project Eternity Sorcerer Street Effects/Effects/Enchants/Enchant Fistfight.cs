using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//No effects or abilities can be used during battle on the target territory. / Doublecast
    public sealed class EnchantFistfightEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Fistfight";

        private sealed class FistfightEffect : SorcererStreetEffect
        {
            public FistfightEffect(SorcererStreetBattleParams Params)
                : base("Sorcerer Street Holy Word 0", false, Params)
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
                Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).EffectLimit = true;
                return "Fistfight";
            }

            protected override BaseEffect DoCopy()
            {
                FistfightEffect NewEffect = new FistfightEffect(Params);

                return NewEffect;
            }

            protected override void DoCopyMembers(BaseEffect Copy)
            {
            }
        }

        public EnchantFistfightEffect()
            : base(Name, false)
        {
        }

        public EnchantFistfightEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, new FistfightEffect(Params), IconHolder.Icons.sprCreatureFistfight);
            return "Fistfight";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantFistfightEffect NewEffect = new EnchantFistfightEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
