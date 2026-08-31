using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Condition
    {
        public string Name;
        public string Description;

        public List<string> ListTraitToDisable;
        public List<string> ListActionToAdd;

        public Condition()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListTraitToDisable = new List<string>();
        }

        public Condition(string ConditionPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("Content/Life Sim/Conditions/" + ConditionPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListTraitToDisableCount = BR.ReadByte();
            ListTraitToDisable = new List<string>(ListTraitToDisableCount);
            for (int T = 0; T < ListTraitToDisableCount; ++T)
            {
                ListTraitToDisable.Add(BR.ReadString());
            }


            BR.Close();
            FS.Close();
        }
    }
}
