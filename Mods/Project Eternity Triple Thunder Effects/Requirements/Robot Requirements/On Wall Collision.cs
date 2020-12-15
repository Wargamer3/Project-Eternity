using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnWallCollisionRobotRequirement : TripleThunderRobotRequirement
    {
        public OnWallCollisionRobotRequirement()
            : this(null)
        {
        }

        public OnWallCollisionRobotRequirement(TripleThunderRobotContext GlobalContext)
            : base(OnWallCollisionName, GlobalContext)
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
            OnWallCollisionRobotRequirement NewSkillEffect = new OnWallCollisionRobotRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
