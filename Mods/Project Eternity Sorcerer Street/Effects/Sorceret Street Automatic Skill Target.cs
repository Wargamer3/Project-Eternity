using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetBattleTargetType : AutomaticSkillTargetType
    {
        protected readonly SorcererStreetBattleContext GlobalContext;

        public SorcererStreetBattleTargetType(string TargetType, SorcererStreetBattleContext GlobalContext)
            : base(TargetType)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
