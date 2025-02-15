using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ImmunityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Effect Immunity";

        public ImmunityEffect()
            : base(Name, false)
        {
        }

        public ImmunityEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).ItemProtection = true;
            return "Immune to Destroy Item and Steal Item effects";
        }

        protected override BaseEffect DoCopy()
        {
            ImmunityEffect NewEffect = new ImmunityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
