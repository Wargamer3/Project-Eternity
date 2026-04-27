using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class CharacterAncestry
    {
        public enum CharacterSizes { Tiny, Small, Medium, Large, Huge, Gargantuan }

        public readonly string RelativePath;

        public string Name;
        public string Description;
        public byte BaseHP;
        public byte Speed;
        public CharacterSizes Size;

        public List<Trait> ListTraits;
        public List<string> ListTraitsRelativePath;

        public List<Language> ListLanguage;
        public List<string> ListLanguageRelativePath;

        public List<Unlockable> ListUnlockable;
        public List<string> ListUnlockableRelativePath;

        public List<StatsToAsign> ListStatBoosts;
        public Dictionary<string, ProficiencyLink> DicProficiencyLevelByName;

        public CharacterAncestry(string AncestryPath, ContentManager Content)
        {
            RelativePath = AncestryPath;
            ListStatBoosts = new List<StatsToAsign>();
            DicProficiencyLevelByName = new Dictionary<string, ProficiencyLink>();

            FileStream FS = new FileStream("Content/Life Sim/Ancestries/" + AncestryPath + ".pea", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();
            BaseHP = BR.ReadByte();
            Speed = BR.ReadByte();
            Size = (CharacterSizes)BR.ReadByte();

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

        public void Init(LifeSimParams Owner)
        {
            ListStatBoosts.Clear();

            foreach (Unlockable ActiveUnlockable in ListUnlockable)
            {
                ActiveUnlockable.Init(Owner, this);
                ActiveUnlockable.CheckUnlocks();
            }
        }
    }
}
