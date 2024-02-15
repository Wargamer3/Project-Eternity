using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public struct BookInventoryContainer
    {
        public Dictionary<string, BookInventoryContainer> DicFolder;
        public List<BookInventoryContainer> ListFolder;//Share the same folders as the dictionnary
        public List<CardBook> ListBook;
        public CardBook IconUnit;

        public string Name;

        public BookInventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolder = new Dictionary<string, BookInventoryContainer>();
            ListFolder = new List<BookInventoryContainer>();
            ListBook = new List<CardBook>();
            IconUnit = null;
        }

        public void AddBook(CardBook UnitToAdd)
        {
            if (ListBook.Count == 0)
            {
                IconUnit = UnitToAdd;
            }

            if (!ListBook.Contains(UnitToAdd))
            {
                ListBook.Add(UnitToAdd);
            }
        }
    }

    public struct CharacterInventoryContainer
    {
        public Dictionary<string, CharacterInventoryContainer> DicFolder;
        public List<CharacterInventoryContainer> ListFolder;//Share the same folders as the dictionnary
        public List<PlayerCharacter> ListCharacter;
        public PlayerCharacter IconUnit;

        public string Name;

        public CharacterInventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolder = new Dictionary<string, CharacterInventoryContainer>();
            ListFolder = new List<CharacterInventoryContainer>();
            ListCharacter = new List<PlayerCharacter>();
            IconUnit = null;
        }

        public void AddCharacter(PlayerCharacter UnitToAdd)
        {
            if (ListCharacter.Count == 0)
            {
                IconUnit = UnitToAdd;
            }

            if (!ListCharacter.Contains(UnitToAdd))
            {
                ListCharacter.Add(UnitToAdd);
            }
        }
    }

    public class SorcererStreetInventory
    {
        public CardBook GlobalBook;//Just using a regular Book to store player owned cards;

        public BookInventoryContainer RootBookContainer;
        public CharacterInventoryContainer RootCharacterContainer;

        public Dictionary<string, CardBook> DicOwnedBook;
        public Dictionary<string, PlayerCharacter> DicOwnedCharacter;
        public Dictionary<string, MissionInfo> DicOwnedMission;

        public PlayerCharacter Character;
        public CardBook ActiveBook;

        public SorcererStreetInventory()
        {
            RootBookContainer = new BookInventoryContainer("ALL");
            RootCharacterContainer = new CharacterInventoryContainer("ALL");

            GlobalBook = ActiveBook = new CardBook("Global");

            DicOwnedBook = new Dictionary<string, CardBook>();
            DicOwnedCharacter = new Dictionary<string, PlayerCharacter>();
            DicOwnedMission = new Dictionary<string, MissionInfo>();
        }

        public void Load(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            GlobalBook = new CardBook(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            string ActiveBookName = BR.ReadString();

            int ListBookCount = BR.ReadInt32();
            for (int B = 0; B < ListBookCount; ++B)
            {
                CardBook LoadedBook = new CardBook(BR, GlobalBook, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                DicOwnedBook.Add(LoadedBook.BookName, LoadedBook);
                AddBook(LoadedBook);

                if (LoadedBook.BookName == ActiveBookName)
                {
                    ActiveBook = LoadedBook;
                }
            }

            int DicOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, PlayerCharacter>(DicOwnedCharacterCount);
            for (int C = 0; C < DicOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();

                PlayerCharacter LoadedCharacter = new PlayerCharacter(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                DicOwnedCharacter.Add(CharacterFullName, LoadedCharacter);
                AddCharacter(LoadedCharacter);
            }

            int ListOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>();
            for (int C = 0; C < ListOwnedMissionCount; ++C)
            {
                string MissionPath = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                MissionInfo LoadedMission = new MissionInfo(MissionPath, QuantityOwned);

                DicOwnedMission.Add(MissionPath, LoadedMission);
            }
        }

        public void Load(ByteReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
        }

        public void Save(BinaryWriter BW)
        {
            GlobalBook.Save(BW);

            BW.Write(ActiveBook.BookName);

            BW.Write(DicOwnedBook.Count);
            foreach (CardBook ActiveBook in DicOwnedBook.Values)
            {
                ActiveBook.Save(BW);
            }

            BW.Write(DicOwnedCharacter.Count);
            foreach (PlayerCharacter ActiveCharacter in DicOwnedCharacter.Values)
            {
                BW.Write(ActiveCharacter.CharacterPath);
            }

            BW.Write(DicOwnedMission.Count);
            foreach (MissionInfo ActiveMission in DicOwnedMission.Values)
            {
                BW.Write(ActiveMission.MapPath);
                BW.Write(ActiveMission.QuantityOwned);
            }
        }

        public void UseBook(CardBook NewCardBook)
        {
            if (!DicOwnedBook.ContainsKey(NewCardBook.BookName))
            {
                DicOwnedBook.Add(NewCardBook.BookName, NewCardBook);
            }

            ActiveBook = NewCardBook;
        }

        public void AddBook(CardBook BookToAdd)
        {
            BookInventoryContainer CurrentCharacterContainer = RootBookContainer;

            CurrentCharacterContainer.AddBook(BookToAdd);

            string[] Tags = BookToAdd.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ActiveTag in Tags)
            {
                CurrentCharacterContainer = RootBookContainer;

                CurrentCharacterContainer.AddBook(BookToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    BookInventoryContainer NewContainer;
                    if (!CurrentCharacterContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new BookInventoryContainer(ActiveFolder);
                        CurrentCharacterContainer.DicFolder.Add(ActiveFolder, NewContainer);
                        CurrentCharacterContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentCharacterContainer = NewContainer;

                    CurrentCharacterContainer.AddBook(BookToAdd);
                }
            }
        }

        public void AddCharacter(PlayerCharacter CharacterToAdd)
        {
            CharacterInventoryContainer CurrentCharacterContainer = RootCharacterContainer;

            CurrentCharacterContainer.AddCharacter(CharacterToAdd);

            string[] Tags = CharacterToAdd.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ActiveTag in Tags)
            {
                CurrentCharacterContainer = RootCharacterContainer;

                CurrentCharacterContainer.AddCharacter(CharacterToAdd);

                string[] SubFolders = ActiveTag.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ActiveFolder in SubFolders)
                {
                    CharacterInventoryContainer NewContainer;
                    if (!CurrentCharacterContainer.DicFolder.TryGetValue(ActiveFolder, out NewContainer))
                    {
                        NewContainer = new CharacterInventoryContainer(ActiveFolder);
                        CurrentCharacterContainer.DicFolder.Add(ActiveFolder, NewContainer);
                        CurrentCharacterContainer.ListFolder.Add(NewContainer);
                    }

                    CurrentCharacterContainer = NewContainer;

                    CurrentCharacterContainer.AddCharacter(CharacterToAdd);
                }
            }
        }
    }
}
