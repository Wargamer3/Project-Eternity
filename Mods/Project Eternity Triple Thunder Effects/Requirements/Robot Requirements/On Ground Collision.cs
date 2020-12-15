using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnGroundCollisionRobotRequirement : TripleThunderRobotRequirement
    {
        public OnGroundCollisionRobotRequirement()
            : this(null)
        {
        }

        public OnGroundCollisionRobotRequirement(TripleThunderRobotContext GlobalContext)
            : base(OnGroundCollisionName, GlobalContext)
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
            OnGroundCollisionRobotRequirement NewSkillEffect = new OnGroundCollisionRobotRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
