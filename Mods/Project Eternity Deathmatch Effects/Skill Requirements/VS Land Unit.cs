using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSLandUnitRequirement : DeathmatchSkillRequirement
    {
        public VSLandUnitRequirement()
            : this(null)
        {
        }

        public VSLandUnitRequirement(DeathmatchContext Context)
            : base(VSLandUnitRequirementName, Context)
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
            return Context.EffectTargetSquad != null && Context.EffectTargetSquad.CurrentMovement == "Land";
        }

        public override BaseSkillRequirement Copy()
        {
            VSLandUnitRequirement NewSkillEffect = new VSLandUnitRequirement(Context);

            return NewSkillEffect;
        }
    }
}
