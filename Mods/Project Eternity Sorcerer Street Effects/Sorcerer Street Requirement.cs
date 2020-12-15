using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetRequirement : BaseSkillRequirement
    {
        protected readonly SorcererStreetBattleContext GlobalContext;

        protected SorcererStreetRequirement(string EffectTypeName, SorcererStreetBattleContext GlobalContext)
            : base(EffectTypeName)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
