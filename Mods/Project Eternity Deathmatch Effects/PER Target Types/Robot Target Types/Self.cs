using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class SelfTargetType : SquadTargetType
    {
        public static string Name = "Squad PER";

        public SelfTargetType()
            : this(null)
        {
        }

        public SelfTargetType(SquadPERParams Params)
            : base(Name, Params)
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
            return new SelfTargetType(Params);
        }
    }
}
