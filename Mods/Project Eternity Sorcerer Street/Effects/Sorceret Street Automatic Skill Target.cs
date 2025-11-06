using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetAutomaticTargetType : AutomaticSkillTargetType
    {
        protected readonly SorcererStreetBattleParams Params;

        public SorcererStreetAutomaticTargetType(string TargetType, SorcererStreetBattleParams Params)
            : base(TargetType)
        {
            this.Params = Params;
        }
    }
}
