using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimAutomaticTargetType : AutomaticSkillTargetType
    {
        protected LifeSimCharacterParams Params;

        public LifeSimAutomaticTargetType(string TargetType)
            : base(TargetType)
        {
        }
    }
}
