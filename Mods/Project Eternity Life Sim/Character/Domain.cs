using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Domain
    {
        public string Name;
        public string Description;

        public string DomainSpellRelativePath;
        public Action DomainSpell;

        public string AdvancedDomainSpellRelativePath;
        public Action AdvancedDomainSpell;

        public string AdvancedLinkedDomainRelativePath;
        public Domain AdvancedLinkedDomain;

        public Domain(string DomainPath, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            FileStream FS = new FileStream("Content/Life Sim/Domains/" + DomainPath + ".ped", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            DomainSpellRelativePath = BR.ReadString();
            AdvancedDomainSpellRelativePath = BR.ReadString();
            AdvancedLinkedDomainRelativePath = BR.ReadString();

            BR.Close();
            FS.Close();
        }
    }
}
