using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetSelfTargetType : SorcererStreetBattleTargetType
    {
        public static string Name = "Sorcerer Street Self";

        public SorcererStreetSelfTargetType()
            : this(null)
        {
        }

        public SorcererStreetSelfTargetType(SorcererStreetBattleContext GlobalContext)
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
            return new SorcererStreetSelfTargetType(GlobalContext);
        }
    }
}
