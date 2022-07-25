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

        public VSLandUnitRequirement(DeathmatchParams Params)
            : base(VSLandUnitRequirementName, Params)
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
            return Params.GlobalContext.EffectTargetSquad != null && Params.GlobalContext.EffectTargetSquad.CurrentMovement == "Land";
        }

        public override BaseSkillRequirement Copy()
        {
            VSLandUnitRequirement NewSkillEffect = new VSLandUnitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
