using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Territory level cannot be lowered.
    public sealed class LandLevelDowngradeLockEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Land Level Downgrade Lock";

        public LandLevelDowngradeLockEffect()
            : base(Name, false)
        {
        }

        public LandLevelDowngradeLockEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).LandLevelDowngradeLock = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            LandLevelDowngradeLockEffect NewEffect = new LandLevelDowngradeLockEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
