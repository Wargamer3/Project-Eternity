using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ParalysisEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Paralysis";

        public ParalysisEffect()
            : base(Name, false)
        {
        }

        public ParalysisEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).Paralysis = true;
            return "Paralysis";
        }

        protected override BaseEffect DoCopy()
        {
            ParalysisEffect NewEffect = new ParalysisEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
