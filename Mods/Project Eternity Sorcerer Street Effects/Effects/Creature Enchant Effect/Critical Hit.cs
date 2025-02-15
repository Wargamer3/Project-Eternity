using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class CriticalHitEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Critical Hit";

        public CriticalHitEffect()
            : base(Name, false)
        {
        }

        public CriticalHitEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).CriticalHit = true;
            return "Critical Hit";
        }

        protected override BaseEffect DoCopy()
        {
            CriticalHitEffect NewEffect = new CriticalHitEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
