using System.IO;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

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

        public UnitStandardPart(string SkillPath, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect)
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
                NewSkillLevel.ListActivation.Add(new BaseSkillActivation(BR, DicRequirement, DicEffect));
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

                foreach (var ActiveRequirement in Activation.ListRequirement)
                {
                    IsPassive = ActiveRequirement.CanActivatePassive();
                }
                if (IsPassive)
                {
                    foreach (var ActiveEffect in Activation.ListEffect)
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
    }
}
