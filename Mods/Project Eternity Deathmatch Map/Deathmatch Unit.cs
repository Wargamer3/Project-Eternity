using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public abstract class DeathmatchUnit : Unit
    {
        protected DeathmatchParams Params;

        protected DeathmatchUnit(DeathmatchParams Params)
        {
            this.Params = Params;
        }

        protected DeathmatchUnit(string Name, DeathmatchParams Params)
            : base(Name)
        {
            this.Params = Params;
        }

        public override void ReloadSkills(Unit Copy, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            DeathmatchUnit DeathmatchUnitCopy = (DeathmatchUnit)Copy;

            this.Params = DeathmatchUnitCopy.Params;

            base.ReloadSkills(Copy, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
        }
    }
}
