using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class SelfTargetType : RobotTargetType
    {
        public static string Name = "Self Robot";

        public SelfTargetType()
            : this(null)
        {
        }

        public SelfTargetType(TripleThunderRobotContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            ActiveSkillEffect.Copy().ExecuteEffect();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new SelfTargetType(GlobalContext);
        }
    }
}
