using System.IO;

namespace ProjectEternity.Core.Item
{
    public sealed class OnCreatedRequirement : BaseSkillRequirement
    {        
        public OnCreatedRequirement()
            : base(OnCreatedRequirementName)
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
            return new OnCreatedRequirement();
        }
    }
}
