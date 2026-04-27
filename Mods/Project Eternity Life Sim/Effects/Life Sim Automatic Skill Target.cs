using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimAutomaticTargetType : AutomaticSkillTargetType
    {
        protected LifeSimParams Params;

        public LifeSimAutomaticTargetType(string TargetType)
            : base(TargetType)
        {
        }
    }
}
