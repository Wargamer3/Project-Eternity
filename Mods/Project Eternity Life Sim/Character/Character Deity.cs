using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CharacterDeity
    {
        public readonly string RelativePath;

        public string Name;
        public string Description;

        public string DeityCategoryRelativePath;
        public DeityCategory DeityCategory;
        public string DeityEdicts;
        public string DeityAnathema;
        public string AreaOfCocern;
        public string ReligiousSymbol;
        public string SacredAnimal;
        public string SacredColors;
        public string ParentPantheon;
        public List<string> ListPantheonMember;

        public DevoteeBenefits DevoteeBenefits;
        public DivineIntercession DivineIntercession;

        public CharacterDeity()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListPantheonMember = new List<string>();
        }

        public CharacterDeity(string DeityPath, ContentManager Content)
        {
            RelativePath = DeityPath;

            FileStream FS = new FileStream("Content/Life Sim/Deities/" + DeityPath + ".ped", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            DeityCategoryRelativePath = BR.ReadString();
            DeityEdicts = BR.ReadString();
            DeityAnathema = BR.ReadString();
            AreaOfCocern = BR.ReadString();
            ReligiousSymbol = BR.ReadString();
            SacredAnimal = BR.ReadString();
            SacredColors = BR.ReadString();

            bool IsDeity = BR.ReadBoolean();
            if (IsDeity)
            {
                ParentPantheon = BR.ReadString();
            }
            else
            {
                byte PanthenonMembersCount = BR.ReadByte();
                ListPantheonMember = new List<string>(PanthenonMembersCount);
                for (int M = 0; M < PanthenonMembersCount; ++M)
                {
                    ListPantheonMember.Add(BR.ReadString());
                }
            }

            DevoteeBenefits = new DevoteeBenefits(BR);

            DivineIntercession = new DivineIntercession(BR);

            BR.Close();
            FS.Close();
        }

        public void Init(LifeSimParams Params)
        {
        }
    }

    public class DevoteeBenefits
    {
        public string DivineAttributeRelativePath;
        public CharacterBackground DivineAttribute;

        public string DivineFontRelativePath;
        public Action DivineFont;

        public string DivineSanctification;

        public string DivineSkillRelativePath;
        public CharacterSkill DivineSkill;

        public string FavoriteWeapon;

        public List<Domain> ListDomain;
        public List<string> ListDomainRelativePath;

        public List<Domain> ListAlternateDomain;
        public List<string> ListAlternateDomainRelativePath;

        public List<CharacterAction> ListSpell;
        public List<string> ListSpellRelativePath;

        public DevoteeBenefits(BinaryReader BR)
        {
            DivineAttributeRelativePath = BR.ReadString();
            DivineFontRelativePath = BR.ReadString();
            DivineSanctification = BR.ReadString();
            DivineSkillRelativePath = BR.ReadString();
            FavoriteWeapon = BR.ReadString();

            byte ListDomainCount = BR.ReadByte();
            ListDomain = new List<Domain>(ListDomainCount);
            ListDomainRelativePath = new List<string>(ListDomainCount);
            for (int M = 0; M < ListDomainCount; ++M)
            {
                ListDomainRelativePath.Add(BR.ReadString());
            }

            byte ListAlternateDomainCount = BR.ReadByte();
            ListAlternateDomain = new List<Domain>(ListAlternateDomainCount);
            ListAlternateDomainRelativePath = new List<string>(ListAlternateDomainCount);
            for (int M = 0; M < ListAlternateDomainCount; ++M)
            {
                ListAlternateDomainRelativePath.Add(BR.ReadString());
            }

            byte ListSpellCount = BR.ReadByte();
            ListSpell = new List<CharacterAction>(ListSpellCount);
            ListSpellRelativePath = new List<string>(ListSpellCount);
            for (int M = 0; M < ListSpellCount; ++M)
            {
                ListSpellRelativePath.Add(BR.ReadString());
                byte LockedLevel = BR.ReadByte();
            }
        }
    }

    public class DivineIntercession
    {
        public string Description;
        public string MinorBoon;
        public string ModerateBoon;
        public string MajorBoon;

        public string MinorCurse;
        public string ModerateCurse;
        public string MajorCurse;

        public DivineIntercession(BinaryReader BR)
        {
            Description = BR.ReadString();

            MinorBoon = BR.ReadString();
            ModerateBoon = BR.ReadString();
            MajorBoon = BR.ReadString();

            MinorCurse = BR.ReadString();
            ModerateCurse = BR.ReadString();
            MajorCurse = BR.ReadString();
        }
    }
}
