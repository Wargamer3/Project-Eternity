using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class SelfAttackTargetType : AttackTargetType
    {
        public static string Name = "Self Attack";

        public SelfAttackTargetType()
            : this(null)
        {

        }

        public SelfAttackTargetType(TripleThunderAttackContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new SelfAttackTargetType(GlobalContext);
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            ActiveSkillEffect.Copy().ExecuteEffect();
            GlobalContext.OwnerProjectile.ListActiveSkill.AddRange(ActiveSkillEffect.ListFollowingSkill);
        }
    }
}
