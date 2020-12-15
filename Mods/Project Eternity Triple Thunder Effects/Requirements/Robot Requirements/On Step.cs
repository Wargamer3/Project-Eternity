using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class OnStepRequirement : TripleThunderRobotRequirement
    {
        public OnStepRequirement()
            : this(null)
        {
        }

        public OnStepRequirement(TripleThunderRobotContext GlobalContext)
            : base(OnStepName, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new OnStepRequirement(GlobalContext);
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }
    }
}
