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
            RelativePath = string.Empty;

            Name = string.Empty;
            Description = string.Empty;

            DeityCategoryRelativePath = string.Empty;
            DeityEdicts = string.Empty;
            DeityAnathema = string.Empty;
            AreaOfCocern = string.Empty;
            ReligiousSymbol = string.Empty;
            SacredAnimal = string.Empty;
            SacredColors = string.Empty;

            ParentPantheon = string.Empty;
            ListPantheonMember = new List<string>(0);

            DevoteeBenefits = new DevoteeBenefits();

            DivineIntercession = new DivineIntercession();
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

        public void Init(LifeSimCharacterParams Params)
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

        public DevoteeBenefits()
        {
            DivineAttributeRelativePath = string.Empty;
            DivineFontRelativePath = string.Empty;
            DivineSanctification = string.Empty;
            DivineSkillRelativePath = string.Empty;
            FavoriteWeapon = string.Empty;

            ListDomain = new List<Domain>(0);
            ListDomainRelativePath = new List<string>(0);

            ListAlternateDomain = new List<Domain>(0);
            ListAlternateDomainRelativePath = new List<string>(0);

            ListSpell = new List<CharacterAction>(0);
            ListSpellRelativePath = new List<string>(0);
        }

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

        public DivineIntercession()
        {
            Description = string.Empty;

            MinorBoon = string.Empty;
            ModerateBoon = string.Empty;
            MajorBoon = string.Empty;

            MinorCurse = string.Empty;
            ModerateCurse = string.Empty;
            MajorCurse = string.Empty;
        }

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
