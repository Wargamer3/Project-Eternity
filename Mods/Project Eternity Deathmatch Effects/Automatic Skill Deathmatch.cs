using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class AutomaticDeathmatchTargetType : AutomaticSkillTargetType
    {
        protected DeathmatchContext GlobalContext;
        
        public AutomaticDeathmatchTargetType(string TargetType, DeathmatchContext GlobalContext)
            : base(TargetType)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
