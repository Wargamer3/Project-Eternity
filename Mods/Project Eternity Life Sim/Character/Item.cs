using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Item
    {
        public readonly string RelativePath;

        public string Name;
        public string Description;

        public string Bulk;
        public string Tags;
        public string[] ArrayTags;

        public List<Trait> ListTraits;
        public List<string> ListTraitsRelativePath;

        public CharacterAction[] ArrayCharacterAction;
        public string[] ArrayCharacterActionPath;

        public Item()
        {
            RelativePath = string.Empty;

            Name = string.Empty;
            Description = string.Empty;

            ListTraits = new List<Trait>(0);
            ListTraitsRelativePath = new List<string>(0);
        }

        public Item(string ItemPath, ContentManager Content)
        {
            FileStream FS = new FileStream("Content/Life Sim/Items/" + ItemPath + ".pei", FileMode.Open, FileAccess.Read);
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

            Bulk = BR.ReadString();
            byte ArrayTagsCount = BR.ReadByte();
            ArrayTags = new string[ArrayTagsCount];
            for (int T = 0; T < ArrayTagsCount; ++T)
            {
                ArrayTags[T] = BR.ReadString();
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
    }
}
