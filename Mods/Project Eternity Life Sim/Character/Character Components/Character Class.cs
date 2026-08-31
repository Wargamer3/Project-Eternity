using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CharacterClass
    {
        public readonly string RelativePath;

        public string Name;
        public string Description;
        public byte BaseHP;

        public List<Trait> ListTraits;
        public List<string> ListTraitsRelativePath;

        public List<Language> ListLanguage;
        public List<string> ListLanguageRelativePath;

        public Dictionary<string, ProficiencyLink> DicProficiencyLevelByName;

        public List<Unlockable> ListUnlockable;
        public List<string> ListUnlockableRelativePath;

        public List<StatsToAsign> ListStatBoosts;

        public CharacterClass()
        {
            RelativePath = string.Empty;
            ListStatBoosts = new List<StatsToAsign>();

            Name = string.Empty;
            Description = string.Empty;
            BaseHP = 0;

            ListTraits = new List<Trait>(0);
            ListTraitsRelativePath = new List<string>(0);

            ListLanguage = new List<Language>(0);
            ListLanguageRelativePath = new List<string>(0);

            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>(0);

            ListUnlockable = new List<Unlockable>(0);
            ListUnlockableRelativePath = new List<string>(0);
        }

        public CharacterClass(string CharacterClassPath, ContentManager Content)
        {
            RelativePath = CharacterClassPath;
            ListStatBoosts = new List<StatsToAsign>();

            FileStream FS = new FileStream("Content/Life Sim/Classes/" + CharacterClassPath + ".pec", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            BaseHP = BR.ReadByte();
            
            byte ListTraitsCount = BR.ReadByte();
            ListTraits = new List<Trait>(ListTraitsCount);
            ListTraitsRelativePath = new List<string>(ListTraitsCount);
            for (int T = 0; T < ListTraitsCount; ++T)
            {
                ListTraitsRelativePath.Add(BR.ReadString());
            }

            byte ListLanguageCount = BR.ReadByte();
            ListLanguage = new List<Language>(ListLanguageCount);
            ListLanguageRelativePath = new List<string>(ListLanguageCount);
            for (int L = 0; L < ListLanguageCount; ++L)
            {
                ListLanguageRelativePath.Add(BR.ReadString());
            }

            byte ProficienciesCount = BR.ReadByte();
            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>(ProficienciesCount);
            for (int L = 0; L < ProficienciesCount; ++L)
            {
                ProficiencyLink LoadedProficiencyLink = new ProficiencyLink(BR);
                DicProficiencyLevelByName.Add(LoadedProficiencyLink.LinkedProficiencyRelativePath, LoadedProficiencyLink);
            }

            byte ListUnlockableCount = BR.ReadByte();
            ListUnlockable = new List<Unlockable>(ListUnlockableCount);
            ListUnlockableRelativePath = new List<string>(ListUnlockableCount);
            for (int U = 0; U < ListUnlockableCount; ++U)
            {
                ListUnlockable.Add(new Unlockable(BR));
            }

            BR.Close();
            FS.Close();
        }

        public void Init(LifeSimCharacterParams Params)
        {
            ListStatBoosts.Clear();

            foreach (ProficiencyLink ActiveProficiency in DicProficiencyLevelByName.Values)
            {
                ActiveProficiency.ComputeStatBonuses(Params.Owner);
            }

            foreach (Unlockable ActiveUnlockable in ListUnlockable)
            {
                ActiveUnlockable.Init(Params, this);
                ActiveUnlockable.CheckUnlocks();
            }
        }
    }
}
