using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TargetProtectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Target Protection";

        public TargetProtectionEffect()
            : base(Name, false)
        {
        }

        public TargetProtectionEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).TargetProtection = true;
            return "Cannot be targeted by spells or territory abilities";
        }

        protected override BaseEffect DoCopy()
        {
            TargetProtectionEffect NewEffect = new TargetProtectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
