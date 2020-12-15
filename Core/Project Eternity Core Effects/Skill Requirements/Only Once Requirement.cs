using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Effects
{
    /// <summary>
    /// Only use this one after another requirement, execution order is important
    /// </summary>
    public sealed class OnlyOnceRequirement : PassiveSkillRequirement
    {
        public static string Name = "Only Once Requirement";

        private bool HasActivated;

        public OnlyOnceRequirement()
            : this(null)
        {
        }

        public OnlyOnceRequirement(UnitEffectContext GlobalContext)
            : base(Name, GlobalContext)
        {
            HasActivated = false;
        }

        protected override void Load(BinaryReader BR)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override bool CanActivatePassive()
        {
            if (HasActivated)
            {
                return false;
            }
            else
            {
                HasActivated = true;
                return true;
            }
        }

        public override BaseSkillRequirement Copy()
        {
            OnlyOnceRequirement NewSkillEffect = new OnlyOnceRequirement(GlobalContext);

            return NewSkillEffect;
        }
    }
}
