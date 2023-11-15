using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetAutomaticTargetType : AutomaticSkillTargetType
    {
        protected readonly SorcererStreetBattleContext GlobalContext;

        public SorcererStreetAutomaticTargetType(string TargetType, SorcererStreetBattleContext GlobalContext)
            : base(TargetType)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
