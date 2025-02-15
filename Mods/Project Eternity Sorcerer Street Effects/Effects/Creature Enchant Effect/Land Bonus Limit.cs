using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature cannot receive land effect.
    public sealed class LandBonusLimitEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Land Bonus Limit";

        public LandBonusLimitEffect()
            : base(Name, false)
        {
        }

        public LandBonusLimitEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).LandEffectLimit = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            LandBonusLimitEffect NewEffect = new LandBonusLimitEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
