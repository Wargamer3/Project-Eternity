using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public abstract class LifeSimRequirement : BaseSkillRequirement
    {
        protected readonly LifeSimCharacterParams Params;

        protected LifeSimRequirement(string EffectTypeName, LifeSimCharacterParams Params)
            : base(EffectTypeName)
        {
            this.Params = Params;
        }
    }
}
