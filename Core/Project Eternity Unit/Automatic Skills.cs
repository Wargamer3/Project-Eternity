using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core.Skill
{
    public abstract class UnitSkillRequirement : BaseSkillRequirement
    {
        protected UnitEffectContext GlobalContext;

        public UnitSkillRequirement(string EffectTypeName)
            : this(EffectTypeName, null)
        {
        }

        public UnitSkillRequirement(string EffectTypeName, UnitEffectContext GlobalContext)
            : base(EffectTypeName)
        {
            this.GlobalContext = GlobalContext;
        }
    }

    public abstract class PassiveSkillRequirement : UnitSkillRequirement
    {
        public PassiveSkillRequirement(string EffectTypeName)
            : this(EffectTypeName, null)
        {
        }

        public PassiveSkillRequirement(string EffectTypeName, UnitEffectContext GlobalContext)
            : base(EffectTypeName, GlobalContext)
        {
        }
    }

    public abstract class ActiveSkillRequirement : UnitSkillRequirement
    {
        public ActiveSkillRequirement(string EffectTypeName)
            : this(EffectTypeName, null)
        {
        }

        public ActiveSkillRequirement(string EffectTypeName, UnitEffectContext GlobalContext)
            : base(EffectTypeName, GlobalContext)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }
    }
}
