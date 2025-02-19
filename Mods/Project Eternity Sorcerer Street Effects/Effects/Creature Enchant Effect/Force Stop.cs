using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ForceStopEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Force Stop";

        public ForceStopEffect()
            : base(Name, false)
        {
        }

        public ForceStopEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).ForceStop = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ForceStopEffect NewEffect = new ForceStopEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
