using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public abstract class ManualSkillActivationDeathmatch : ManualSkillTarget
    {
        public DeathmatchContext Context;

        protected ManualSkillActivationDeathmatch(string TargetType, bool MustBeUsedAlone, DeathmatchContext Context)
            : base(TargetType, MustBeUsedAlone)
        {
            this.Context = Context;
        }
    }
}
