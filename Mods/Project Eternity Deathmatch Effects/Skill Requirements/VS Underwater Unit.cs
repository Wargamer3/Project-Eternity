using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSUnderwaterUnitRequirement : DeathmatchSkillRequirement
    {
        public VSUnderwaterUnitRequirement()
            : this(null)
        {
        }

        public VSUnderwaterUnitRequirement(DeathmatchParams Params)
            : base(VSUnderwaterUnitRequirementName, Params)
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
            return Params.GlobalContext.EffectTargetSquad != null && Params.GlobalContext.EffectTargetSquad.CurrentTerrainIndex == UnitStats.TerrainUnderwaterIndex && Params.GlobalContext.EffectTargetSquad.IsUnderTerrain;
        }

        public override BaseSkillRequirement Copy()
        {
            VSUnderwaterUnitRequirement NewSkillEffect = new VSUnderwaterUnitRequirement(Params);

            return NewSkillEffect;
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
