using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnDestroyedRobotRequirement : TripleThunderRobotRequirement
    {
        public OnDestroyedRobotRequirement()
            : this(null)
        {
        }

        public OnDestroyedRobotRequirement(TripleThunderRobotContext GlobalContext)
            : base(OnDestroyedName, GlobalContext)
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
            OnDestroyedRobotRequirement NewSkillEffect = new OnDestroyedRobotRequirement(GlobalContext);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
