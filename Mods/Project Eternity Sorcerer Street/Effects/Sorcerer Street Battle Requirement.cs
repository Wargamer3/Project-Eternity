using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetBattleRequirement : BaseSkillRequirement
    {
        protected readonly SorcererStreetBattleContext GlobalContext;

        protected SorcererStreetBattleRequirement(string EffectTypeName, SorcererStreetBattleContext GlobalContext)
            : base(EffectTypeName)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
