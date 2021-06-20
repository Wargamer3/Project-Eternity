using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSWaterUnitRequirement : DeathmatchSkillRequirement
    {
        public VSWaterUnitRequirement()
            : this(null)
        {
        }

        public VSWaterUnitRequirement(DeathmatchContext Context)
            : base(VSWaterUnitRequirementName, Context)
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
            return Context.EffectTargetSquad != null && Context.EffectTargetSquad.CurrentMovement == "Sea";
        }

        public override BaseSkillRequirement Copy()
        {
            VSWaterUnitRequirement NewSkillEffect = new VSWaterUnitRequirement(Context);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
