using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationSquad : AutomaticDeathmatchTargetType
    {
        public static string Name = "Squad";

        public EffectActivationSquad()
            : this(null)
        {
        }

        public EffectActivationSquad(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int U = GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (ActiveSkillEffect.CanActivate())
                    return true;
            }

            return false;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            for (int U = GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                GlobalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                    GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerSquad[U], GlobalContext.EffectTargetUnit.Pilot);
                
                GlobalContext.EffectTargetUnit.Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSquad(GlobalContext);
        }
    }
}
