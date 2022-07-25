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

        public VSWaterUnitRequirement(DeathmatchParams Params)
            : base(VSWaterUnitRequirementName, Params)
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
            return Params.GlobalContext.EffectTargetSquad != null && Params.GlobalContext.EffectTargetSquad.CurrentMovement == "Sea";
        }

        public override BaseSkillRequirement Copy()
        {
            VSWaterUnitRequirement NewSkillEffect = new VSWaterUnitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
