using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSFlyingUnitRequirement : DeathmatchSkillRequirement
    {
        public VSFlyingUnitRequirement()
            : this(null)
        {
        }

        public VSFlyingUnitRequirement(DeathmatchContext Context)
            : base(VSFlyingUnitRequirementName, Context)
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
            return Context.EffectTargetSquad != null && Context.EffectTargetSquad.IsFlying;
        }

        public override BaseSkillRequirement Copy()
        {
            VSFlyingUnitRequirement NewSkillEffect = new VSFlyingUnitRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
