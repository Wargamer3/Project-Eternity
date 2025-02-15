using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    //Cannot invade enemy territories.
    public sealed class InvasionProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Invasion Protection";

        public InvasionProtectionEffect()
            : base(Name, false)
        {
        }

        public InvasionProtectionEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).InvasionProtection = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            InvasionProtectionEffect NewEffect = new InvasionProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
