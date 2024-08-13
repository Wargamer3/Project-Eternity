using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.ConquestMapScreen
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
