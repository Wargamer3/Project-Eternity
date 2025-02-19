using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DisableLandBonusRestrictionsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Disable Land Bonus Restrictions";

        public DisableLandBonusRestrictionsEffect()
            : base(Name, false)
        {
        }

        public DisableLandBonusRestrictionsEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).LandEffectNoLimit = true;
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            DisableLandBonusRestrictionsEffect NewEffect = new DisableLandBonusRestrictionsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
