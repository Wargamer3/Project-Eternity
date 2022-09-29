using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSFlyingUnitRequirement : DeathmatchSkillRequirement
    {
        public VSFlyingUnitRequirement()
            : this(null)
        {
        }

        public VSFlyingUnitRequirement(DeathmatchParams Params)
            : base(VSFlyingUnitRequirementName, Params)
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
            return Params.GlobalContext.EffectTargetSquad != null && Params.GlobalContext.EffectTargetSquad.CurrentTerrainIndex == UnitStats.TerrainAirIndex;
        }

        public override BaseSkillRequirement Copy()
        {
            VSFlyingUnitRequirement NewSkillEffect = new VSFlyingUnitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
