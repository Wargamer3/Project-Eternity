using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationSelf : AutomaticDeathmatchTargetType
    {
        public static string Name = "Self";

        public EffectActivationSelf()
            : this(null)
        {
        }

        public EffectActivationSelf(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }
        
        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            GlobalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter);

            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            GlobalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter, 
                GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter);

            GlobalContext.EffectTargetUnit.Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSelf(GlobalContext);
        }
    }
}
