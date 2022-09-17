using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class SquadTargetType : AutomaticSkillTargetType
    {
        protected readonly SquadPERParams Params;

        public SquadTargetType(string TargetType, SquadPERParams Params)
            : base(TargetType)
        {
            this.Params = Params;
        }
    }
}
