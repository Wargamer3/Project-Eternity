using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class LifeSimSelfTargetType : LifeSimAutomaticTargetType
    {
        public static string Name = "Life Sim Self";

        public LifeSimSelfTargetType()
            : base(Name)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            Params.ActivationContext.User = Params.BattleContext.Self;
            Params.ActivationContext.Target = Params.BattleContext.Opponent;
            ActiveSkillEffect.Copy().ExecuteEffect();
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new LifeSimSelfTargetType();
        }
    }
}
