using System.IO;

namespace ProjectEternity.Core.Item
{
    public sealed class AfterMovingRequirement : BaseSkillRequirement
    {        
        public AfterMovingRequirement()
            : base(AfterMovingRequirementName)
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
            return new AfterMovingRequirement();
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
