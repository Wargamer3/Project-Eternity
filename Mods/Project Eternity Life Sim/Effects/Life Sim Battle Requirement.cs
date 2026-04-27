using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimRequirement : BaseSkillRequirement
    {
        protected readonly LifeSimParams Params;

        protected LifeSimRequirement(string EffectTypeName, LifeSimParams Params)
            : base(EffectTypeName)
        {
            this.Params = Params;
        }
    }
}
