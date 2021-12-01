using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Parts
{
    public class UnitConsumablePart : UnitPart
    {
        public ManualSkill Spirit;

        public UnitConsumablePart(string SkillPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
            : base()
        {
            PartType = PartTypes.Consumable;
            Spirit = new ManualSkill(SkillPath, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
        }

        public override void ActivatePassiveBuffs()
        {
        }

        public override void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Spirit.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
        }

        public override string Name
        {
            get
            {
                return Spirit.Name;
            }
        }

        public override string Description
        {
            get
            {
                return Spirit.Description;
            }
        }
    }
}
