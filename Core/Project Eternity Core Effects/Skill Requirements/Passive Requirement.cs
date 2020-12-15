using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    public sealed class PassiveRequirement : PassiveSkillRequirement
    {
        public static string Name = "Passive Requirement";

        public PassiveRequirement()
            : this(null)
        {
        }

        public PassiveRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override bool CanActivatePassive()
        {
            return true;
        }

        public override BaseSkillRequirement Copy()
        {
            PassiveRequirement NewSkillEffect = new PassiveRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
