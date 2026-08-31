using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Armor
    {
        public string Name;
        public string Description;

        public List<string> ListSpecialEffect;

        public Armor()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListSpecialEffect = new List<string>();
        }

        public Armor(string ArmorPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("Content/Life Sim/Armors/" + ArmorPath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
