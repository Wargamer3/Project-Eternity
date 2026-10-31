using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Weapon
    {
        public enum HandTypes { OneHand, TwoHands }
        public enum WeaponTypes { Melee, Range }
        public enum WeaponCategories { Martial, Ammunition }
        public enum WeaponGroups { Sword, Firearm }

        public string Name;
        public string Description;

        public List<Trait> ListTraits;
        public List<string> ListTraitsRelativePath;

        public string Damage;
        public string DamageType = "Slashing";
        public string Bulk;

        public HandTypes HandType;
        public WeaponTypes WeaponType;
        public WeaponCategories WeaponCategory;
        public WeaponGroups WeaponGroup;
        public string GroupName;

        public string Range;
        public float NearRange = 2f;
        public float FarRange = 2f;
        public int DamageAmount;
        public byte Reload;
        bool TraitsReady;

        public CharacterAction[] ArrayCharacterAction;
        public string[] ArrayCharacterActionPath;

        public Weapon()
        {
            Name = string.Empty;
            Description = string.Empty;

            ListTraitsRelativePath = new List<string>();
        }

        public Weapon(string WeaponPath, ContentManager Content)
        {
            FileStream FS = new FileStream("Content/Life Sim/Weapons/" + WeaponPath + ".pew", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Name = BR.ReadString();
            Description = BR.ReadString();

            byte ListTraitsCount = BR.ReadByte();
            ListTraits = new List<Trait>(ListTraitsCount);
            ListTraitsRelativePath = new List<string>(ListTraitsCount);
            for (int T = 0; T < ListTraitsCount; ++T)
            {
                ListTraitsRelativePath.Add(BR.ReadString());
            }

            Damage = BR.ReadString();
            Bulk = BR.ReadString();

            HandType = (HandTypes)BR.ReadByte();
            WeaponType = (WeaponTypes)BR.ReadByte();
            WeaponCategory = (WeaponCategories)BR.ReadByte();
            WeaponGroup = (WeaponGroups)BR.ReadByte();

            Range = BR.ReadString();
            Reload = BR.ReadByte();

            float ParsedRange;
            if (float.TryParse(Range, out ParsedRange) && ParsedRange > 0f)
            {
                NearRange = ParsedRange;
                FarRange = ParsedRange;
            }

            byte CharacterActionCount = BR.ReadByte();
            ArrayCharacterAction = new CharacterAction[CharacterActionCount];
            ArrayCharacterActionPath = new string[CharacterActionCount];

            for (int S = 0; S < CharacterActionCount; ++S)
            {
                string CharacterActionName = BR.ReadString();
                ArrayCharacterActionPath[S] = CharacterActionName;
                ArrayCharacterAction[S] = new CharacterAction(CharacterActionName, Content);
            }

            BR.Close();
            FS.Close();
        }

        public void EnsureTraits()
        {
            if (TraitsReady)
            {
                return;
            }

            if (ListTraits != null && ListTraits.Count > 0)
            {
                TraitsReady = true;
                return;
            }

            if (ListTraitsRelativePath == null || ListTraitsRelativePath.Count == 0)
            {
                TraitsReady = true;
                return;
            }

            List<Trait> Loaded = new List<Trait>(ListTraitsRelativePath.Count);
            for (int i = 0; i < ListTraitsRelativePath.Count; i++)
            {
                Loaded.Add(new Trait(ListTraitsRelativePath[i]));
            }

            ListTraits = Loaded;
            TraitsReady = true;
        }

        public void Init(LifeSimCharacterParams Params)
        {
        }
    }
}
