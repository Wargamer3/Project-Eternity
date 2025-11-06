using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class SorcererStreetRequirement : BaseSkillRequirement
    {
        protected readonly SorcererStreetBattleParams Params;

        protected SorcererStreetRequirement(string EffectTypeName, SorcererStreetBattleParams Params)
            : base(EffectTypeName)
        {
            this.Params = Params;
        }
    }
}
