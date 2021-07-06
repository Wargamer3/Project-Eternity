using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using System.Collections.Generic;

namespace ProjectEternity.Core.Parts
{
    public class UnitConsumablePart : UnitPart
    {
        public ManualSkill Spirit;

        public UnitConsumablePart(string SkillPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
            : base()
        {
            PartType = PartTypes.Consumable;
            Spirit = new ManualSkill(SkillPath, DicRequirement, DicEffect);
        }

        public override void ActivatePassiveBuffs()
        {
        }

        public override void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, ManualSkillTarget> DicTarget)
        {
            Spirit.ReloadSkills(DicRequirement, DicEffect, DicTarget);
        }

        public override string Name
        {
            get
            {
                return Spirit.Name;
            }
        }
    }
}
