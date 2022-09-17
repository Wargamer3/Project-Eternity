using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class SelfAttackTargetType : AttackTargetType
    {
        public static string Name = "Attack PER";

        public SelfAttackTargetType()
            : this(null)
        {

        }

        public SelfAttackTargetType(AttackPERParams Params)
            : base(Name, Params)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new SelfAttackTargetType(Params);
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            ActiveSkillEffect.Copy().ExecuteEffect();
        }
    }
}
