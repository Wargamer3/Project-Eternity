using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class AttackTargetType : AutomaticSkillTargetType
    {
        protected readonly AttackPERParams Params;

        public AttackTargetType(string TargetType, AttackPERParams Params)
            : base(TargetType)
        {
            this.Params = Params;
        }
    }
}
