using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class EnemyTargetType : RobotTargetType
    {
        public static string Name = "Enemy Robot";

        public EnemyTargetType()
            : this(null)
        {
        }

        public EnemyTargetType(TripleThunderRobotContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            GlobalContext.Target.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
        }

        public override AutomaticSkillTargetType Copy()
        {
            throw new NotImplementedException();
        }
    }
}
