using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetInventory
    {
        public string CharacterModelPath;
        public CardBook GlobalBook;//Just using a regular Book to store player owned cards;

        public List<CardBook> ListBook;

        public CardBook ActiveBook;

        public SorcererStreetInventory()
        {
            CharacterModelPath = string.Empty;
            ListBook = new List<CardBook>();
            GlobalBook = ActiveBook = new CardBook("Global");
        }

        public void Load(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            CharacterModelPath = BR.ReadString();

            GlobalBook = new CardBook(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            string ActiveBookName = BR.ReadString();

            int ListBookCount = BR.ReadInt32();
            for (int B = 0; B < ListBookCount; ++B)
            {
                CardBook LoadedBook = new CardBook(BR, GlobalBook, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
                ListBook.Add(LoadedBook);

                if (LoadedBook.BookName == ActiveBookName)
                {
                    ActiveBook = LoadedBook;
                }
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(CharacterModelPath);

            GlobalBook.Save(BW);
            BW.Write(ActiveBook.BookName);

            BW.Write(ListBook.Count);
            for (int B = 0; B < ListBook.Count; ++B)
            {
                ListBook[B].Save(BW);
            }
        }
    }
}
