using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class FreeTravelEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Free Travel";

        public FreeTravelEffect()
            : base(Name, false)
        {
        }

        public FreeTravelEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).FreeTravel = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            FreeTravelEffect NewEffect = new FreeTravelEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
