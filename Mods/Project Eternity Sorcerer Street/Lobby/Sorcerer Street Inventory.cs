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
        public List<PlayerCharacterInfo> ListCharacter;
        public PlayerCharacter IconUnit;

        public string Name;

        public CharacterInventoryContainer(string Name)
        {
            this.Name = Name;

            DicFolder = new Dictionary<string, CharacterInventoryContainer>();
            ListFolder = new List<CharacterInventoryContainer>();
            ListCharacter = new List<PlayerCharacterInfo>();
            IconUnit = null;
        }

        public void AddCharacter(PlayerCharacterInfo CharacterToAdd)
        {
            if (ListCharacter.Count == 0)
            {
                IconUnit = CharacterToAdd.Character;
            }

            if (!ListCharacter.Contains(CharacterToAdd))
            {
                ListCharacter.Add(CharacterToAdd);
            }
        }
    }

    public class SorcererStreetInventory
    {
        public CardBook GlobalBook;//Just using a regular Book to store player owned cards;
        public Dictionary<string, CardSkinInfo> DicOwnedCardSkin;//Skins for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, CardSkinInfo> DicOwnedCardAlt;//Alts for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath

        public BookInventoryContainer RootBookContainer;
        public CharacterInventoryContainer RootCharacterContainer;

        public Dictionary<string, CardBookInfo> DicOwnedBook;
        public Dictionary<string, CardBookSkinInfo> DicOwnedBookSkin;//Skins for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, CardBookSkinInfo> DicOwnedBookAlt;//Alts for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, PlayerCharacterInfo> DicOwnedCharacter;
        public Dictionary<string, PlayerCharacterSkin> DicOwnedCharacterSkin;//Skins for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, PlayerCharacterSkin> DicOwnedCharacterAlt;//Alts for Character the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, MissionInfo> DicOwnedMission;
        public Dictionary<string, Bot> DicOwnedBot;

        public PlayerCharacterInfo Character;
        public byte ActiveSkinIndex;
        public CardBook ActiveBook;

        public SorcererStreetInventory()
        {
            RootBookContainer = new BookInventoryContainer("ALL");
            RootCharacterContainer = new CharacterInventoryContainer("ALL");

            GlobalBook = ActiveBook = new CardBook("Global");

            DicOwnedBook = new Dictionary<string, CardBookInfo>();
            DicOwnedCharacter = new Dictionary<string, PlayerCharacterInfo>();
            DicOwnedMission = new Dictionary<string, MissionInfo>();
            DicOwnedBot = new Dictionary<string, Bot>();
        }

        public void Load(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffect,
            Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget, Dictionary<string, ManualSkillTarget> DicManualSkillTarget)
        {
            RootBookContainer = new BookInventoryContainer("ALL");
            RootCharacterContainer = new CharacterInventoryContainer("ALL");

            GlobalBook = new CardBook(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

            string ActiveBookName = BR.ReadString();

            string CharacterPath = BR.ReadString();
            ActiveSkinIndex = BR.ReadByte();

            int ListBookCount = BR.ReadInt32();
            DicOwnedBook = new Dictionary<string, CardBookInfo>(ListBookCount);
            for (int B = 0; B < ListBookCount; ++B)
            {
                CardBook LoadedBook = new CardBook(BR, GlobalBook, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                AddBook(LoadedBook);

                if (LoadedBook.BookName == ActiveBookName)
                {
                    ActiveBook = LoadedBook;
                }
            }

            int DicOwnedCharacterCount = BR.ReadInt32();
            DicOwnedCharacter = new Dictionary<string, PlayerCharacterInfo>(DicOwnedCharacterCount);
            for (int C = 0; C < DicOwnedCharacterCount; ++C)
            {
                string CharacterFullName = BR.ReadString();

                PlayerCharacterInfo LoadedCharacter = new PlayerCharacterInfo(new PlayerCharacter(CharacterFullName, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget));

                DicOwnedCharacter.Add(CharacterFullName, LoadedCharacter);
                AddCharacter(LoadedCharacter);

                if (CharacterPath == CharacterFullName)
                {
                    Character = LoadedCharacter;
                }
            }

            int ListOwnedMissionCount = BR.ReadInt32();
            DicOwnedMission = new Dictionary<string, MissionInfo>(ListOwnedMissionCount);
            for (int C = 0; C < ListOwnedMissionCount; ++C)
            {
                string MissionPath = BR.ReadString();
                byte QuantityOwned = BR.ReadByte();

                MissionInfo LoadedMission = new MissionInfo(MissionPath, QuantityOwned);

                DicOwnedMission.Add(MissionPath, LoadedMission);
            }

            int ListOwnedBotCount = BR.ReadInt32();
            DicOwnedBot = new Dictionary<string, Bot>(ListOwnedBotCount);
            for (int C = 0; C < ListOwnedBotCount; ++C)
            {
                string BotName = BR.ReadString();
                PlayerCharacter Character = new PlayerCharacter(BR.ReadString(), Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                int ListBotBookCount = BR.ReadInt32();
                Dictionary<string, CardBook> DicBotOwnedBook = new Dictionary<string, CardBook>(ListBotBookCount);
                for (int B = 0; B < ListBotBookCount; ++B)
                {
                    CardBook LoadedBook = new CardBook(BR, Content, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);

                    DicBotOwnedBook.Add(LoadedBook.BookName, LoadedBook);
                }

                Bot LoadedBot = new Bot(Character, DicBotOwnedBook);

                DicOwnedBot.Add(BotName, LoadedBot);
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

            BW.Write(Character.Character.CharacterPath);
            BW.Write(ActiveSkinIndex);

            BW.Write(DicOwnedBook.Count);
            foreach (CardBookInfo ActiveBook in DicOwnedBook.Values)
            {
                ActiveBook.Book.Save(BW);
            }

            BW.Write(DicOwnedCharacter.Count);
            foreach (PlayerCharacterInfo ActiveCharacter in DicOwnedCharacter.Values)
            {
                BW.Write(ActiveCharacter.Character.CharacterPath);
            }

            BW.Write(DicOwnedMission.Count);
            foreach (MissionInfo ActiveMission in DicOwnedMission.Values)
            {
                BW.Write(ActiveMission.MapPath);
                BW.Write(ActiveMission.QuantityOwned);
            }

            BW.Write(DicOwnedBot.Count);
            foreach (KeyValuePair<string, Bot> ActiveBot in DicOwnedBot)
            {
                BW.Write(ActiveBot.Key);
                BW.Write(ActiveBot.Value.Character.CharacterPath);
                BW.Write(ActiveBot.Value.DicOwnedBook.Count);
                foreach (CardBook ActiveBook in ActiveBot.Value.DicOwnedBook.Values)
                {
                    ActiveBook.Save(BW);
                }
            }
        }

        public void UseBook(CardBook NewCardBook)
        {
            if (!DicOwnedBook.ContainsKey(NewCardBook.BookName))
            {
                DicOwnedBook.Add(NewCardBook.BookName, new CardBookInfo(NewCardBook));
            }

            ActiveBook = NewCardBook;
        }

        public void AddBook(CardBook BookToAdd)
        {
            DicOwnedBook.Add(BookToAdd.BookName, new CardBookInfo(BookToAdd));
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

        public void AddCharacter(PlayerCharacterInfo CharacterToAdd)
        {
            CharacterInventoryContainer CurrentCharacterContainer = RootCharacterContainer;

            CurrentCharacterContainer.AddCharacter(CharacterToAdd);

            string[] Tags = CharacterToAdd.Character.Tags.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
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
