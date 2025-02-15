using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class LandLevelLockEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Land Level Lock";

        public LandLevelLockEffect()
            : base(Name, false)
        {
        }

        public LandLevelLockEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).LandLevelLock = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            LandLevelLockEffect NewEffect = new LandLevelLockEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
