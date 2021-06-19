using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed class VSUnderwaterUnitRequirement : DeathmatchSkillRequirement
    {
        public VSUnderwaterUnitRequirement()
            : this(null)
        {
        }

        public VSUnderwaterUnitRequirement(DeathmatchContext Context)
            : base(VSUnderwaterUnitRequirementName, Context)
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
            return Context.EffectTargetSquad != null && Context.EffectTargetSquad.CurrentMovement == "Sea" && Context.EffectTargetSquad.IsUnderTerrain;
        }

        public override BaseSkillRequirement Copy()
        {
            VSWaterUnitRequirement NewSkillEffect = new VSWaterUnitRequirement(Context);

            return NewSkillEffect;
        }
    }
}
