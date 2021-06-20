using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnCeilingCollisionRobotRequirement : TripleThunderRobotRequirement
    {
        public OnCeilingCollisionRobotRequirement()
            : this(null)
        {
        }

        public OnCeilingCollisionRobotRequirement(TripleThunderRobotContext GlobalContext)
            : base(OnCeilingCollisionName, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        public override BaseSkillRequirement Copy()
        {
            OnCeilingCollisionRobotRequirement NewSkillEffect = new OnCeilingCollisionRobotRequirement(GlobalContext);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
