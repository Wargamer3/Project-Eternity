using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Core.Parts
{
    public class UnitStandardPart : UnitPart
    {
        public BaseAutomaticSkill Skill;

        public UnitStandardPart()
        {
            Skill = new BaseAutomaticSkill();
            Skill.ListSkillLevel.Add(new BaseSkillLevel());
        }

        public UnitStandardPart(string SkillPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            Skill = new BaseAutomaticSkill();
            Skill.Name = Path.GetFileNameWithoutExtension(SkillPath);
            Skill.CurrentLevel = 1;
            Skill.ListSkillLevel = new List<BaseSkillLevel>();

            FileStream FS = new FileStream(SkillPath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Skill.Description = BR.ReadString();

            BaseSkillLevel NewSkillLevel = new BaseSkillLevel();
            Skill.ListSkillLevel.Add(NewSkillLevel);
            NewSkillLevel.ActivationsCount = BR.ReadInt32();

            int ListActivationRequirementCount = BR.ReadInt32();
            for (int R = 0; R < ListActivationRequirementCount; R++)
            {
                NewSkillLevel.ListActivation.Add(new BaseSkillActivation(BR, DicRequirement, DicEffect, DicAutomaticSkillTarget));
            }

            FS.Close();
            BR.Close();
        }

        public PartTypes GetPartType()
        {
            return PartTypes.Standard;
        }

        public override void ActivatePassiveBuffs()
        {
            foreach (BaseSkillActivation Activation in Skill.ListSkillLevel[0].ListActivation)
            {
                bool IsPassive = false;

                foreach (BaseSkillRequirement ActiveRequirement in Activation.ListRequirement)
                {
                    IsPassive = ActiveRequirement.CanActivatePassive();
                }
                if (IsPassive)
                {
                    foreach (BaseEffect ActiveEffect in Activation.ListEffect)
                    {
                        if (ActiveEffect.EffectTypeName == "Unit Stat Effect")
                        {
                            ActiveEffect.ResetState();
                            ActiveEffect.ExecuteEffect();
                        }
                    }
                }
            }
        }

        public override void ReloadSkills(Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            Skill.ReloadSkills(DicRequirement, DicEffect, DicAutomaticSkillTarget);
        }

        [CategoryAttribute("Level Attributes"),
        DescriptionAttribute(".")]
        public int ActivationsCount
        {
            get { return Skill.ListSkillLevel[0].ActivationsCount; }
            set { Skill.ListSkillLevel[0].ActivationsCount = value; }
        }

        public override string Name
        {
            get
            {
                return Skill.Name;
            }
        }

        public override string Description
        {
            get
            {
                return Skill.Description;
            }
        }
    }
}
