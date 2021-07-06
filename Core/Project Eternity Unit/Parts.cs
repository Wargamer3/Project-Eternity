using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using System.Collections.Generic;

namespace ProjectEternity.Core.Parts
{
    public enum PartTypes { Standard, Consumable }

    public abstract class UnitPart
    {
        public PartTypes PartType;

        public abstract string Name { get; }

        public int Quantity = 1;
        
        public abstract void ActivatePassiveBuffs();

        public abstract void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect, Dictionary<string, ManualSkillTarget> DicTarget);
    }
}
