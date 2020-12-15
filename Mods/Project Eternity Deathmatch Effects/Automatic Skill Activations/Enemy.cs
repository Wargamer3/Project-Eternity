using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationEnemy : AutomaticDeathmatchTargetType
    {
        public static string Name = "Enemy";

        public EffectActivationEnemy()
            : this(null)
        {
        }

        public EffectActivationEnemy(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            string LifeType = "";

            if (ActiveSkillEffect.LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + GlobalContext.Map.ActivePlayerIndex;
            }

            GlobalContext.EffectTargetUnit.Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationEnemy(GlobalContext);
        }
    }
}
