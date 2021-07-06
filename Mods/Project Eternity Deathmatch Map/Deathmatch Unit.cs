using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchUnit : Unit
    {
        protected DeathmatchMap Map;

        protected DeathmatchUnit(DeathmatchMap Map)
        {
            this.Map = Map;
        }

        protected DeathmatchUnit(string Name, DeathmatchMap Map)
            : base(Name)
        {
            this.Map = Map;
        }

        public override void ReloadSkills(Unit Copy, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            DeathmatchUnit DeathmatchUnitCopy = (DeathmatchUnit)Copy;

            this.Map = DeathmatchUnitCopy.Map;

            base.ReloadSkills(Copy, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
        }
    }
}
