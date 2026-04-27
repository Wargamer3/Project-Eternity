using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
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
        public string Bulk;

        public HandTypes HandType;
        public WeaponTypes WeaponType;
        public WeaponCategories WeaponCategory;
        public WeaponGroups WeaponGroup;

        public string Range;
        public byte Reload;

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
    }
}
