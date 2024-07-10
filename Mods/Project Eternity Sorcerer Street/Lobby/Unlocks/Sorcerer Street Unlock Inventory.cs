﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetPlayerUnlockInventory
    {
        public struct BookUnlockContainer
        {
            public Dictionary<string, BookUnlockContainer> DicFolder;
            public List<BookUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockableBook> ListUnlockedBook;
            public List<UnlockableBook> ListLockedBook;
            public UnlockableBook IconUnit { get { if (ListUnlockedBook.Count > 0) return ListUnlockedBook[0]; else return ListLockedBook[0]; } }

            public string Name;

            public BookUnlockContainer(IEnumerable<UnlockableBook> ListLockedUnit)
            {
                this.ListLockedBook = new List<UnlockableBook>(ListLockedUnit);
                ListUnlockedBook = new List<UnlockableBook>();
                DicFolder = new Dictionary<string, BookUnlockContainer>();
                ListFolder = new List<BookUnlockContainer>();
                Name = string.Empty;
            }

            public BookUnlockContainer(string Name)
            {
                this.Name = Name;

                DicFolder = new Dictionary<string, BookUnlockContainer>();
                ListFolder = new List<BookUnlockContainer>();
                ListUnlockedBook = new List<UnlockableBook>();
                ListLockedBook = new List<UnlockableBook>();
            }
        }

        public struct CharacterUnlockContainer
        {
            public Dictionary<string, CharacterUnlockContainer> DicFolder;
            public List<CharacterUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockablePlayerCharacter> ListUnlockedCharacter;
            public List<UnlockablePlayerCharacter> ListLockedCharacter;
            public UnlockablePlayerCharacter IconUnit { get { if (ListUnlockedCharacter.Count > 0) return ListUnlockedCharacter[0]; else return ListLockedCharacter[0]; } }

            public string Name;

            public CharacterUnlockContainer(IEnumerable<UnlockablePlayerCharacter> ListLockedUnit)
            {
                this.ListLockedCharacter = new List<UnlockablePlayerCharacter>(ListLockedUnit);
                ListUnlockedCharacter = new List<UnlockablePlayerCharacter>();
                DicFolder = new Dictionary<string, CharacterUnlockContainer>();
                ListFolder = new List<CharacterUnlockContainer>();
                Name = string.Empty;
            }

            public CharacterUnlockContainer(string Name)
            {
                this.Name = Name;

                DicFolder = new Dictionary<string, CharacterUnlockContainer>();
                ListFolder = new List<CharacterUnlockContainer>();
                ListUnlockedCharacter = new List<UnlockablePlayerCharacter>();
                ListLockedCharacter = new List<UnlockablePlayerCharacter>();
            }
        }

        public static Dictionary<string, UnlockableBook> DicBookDatabase = new Dictionary<string, UnlockableBook>();
        public static Dictionary<string, UnlockablePlayerCharacter> DicCharacterDatabase = new Dictionary<string, UnlockablePlayerCharacter>();
        public static Dictionary<string, UnlockableMission> DicMissionDatabase = new Dictionary<string, UnlockableMission>();

        public static Dictionary<string, UnlockableBook>.Enumerator DicBookDatabaseEnumerator;
        public static Dictionary<string, UnlockablePlayerCharacter>.Enumerator DicCharacterDatabaseEnumeretor;
        public static Dictionary<string, UnlockableMission>.Enumerator DicMissionDatabaseEnumerator;

        private static IniFileReader GlobalUnlockIniAsync;

        public static bool DatabaseLoaded;
        public static Task LoadTask;
        private static object LockObject = new object();

        private static List<string> ListCharacterToPrioritiseLoadPath;//Path of characters to load in background;
        private static List<string> ListBookToPrioritiseLoadPath;//Path of units to load in background;
        private static List<string> ListMissionToPrioritiseLoadPath;//Path of missions to load in background;

        public BookUnlockContainer RootBookContainer;
        public CharacterUnlockContainer RootCharacterContainer;

        public List<UnlockableMission> ListUnlockedMission;
        public List<UnlockableMission> ListLockedMission;

        private FileStream PlayerUnlocksFS;
        private BinaryReader PlayerUnlocksBR;
        private int RemainingNumberOfBooksToLoad;
        private int RemainingNumberOfCharactersToLoad;
        private int RemainingNumberOfMissionsToLoad;
        public bool HasFinishedReadingPlayerShopItems;

        public SorcererStreetPlayerUnlockInventory()
        {
            RootBookContainer = new BookUnlockContainer("ALL");
            RootCharacterContainer = new CharacterUnlockContainer("ALL");

            ListUnlockedMission = new List<UnlockableMission>();
            ListLockedMission = new List<UnlockableMission>();

            ListCharacterToPrioritiseLoadPath = new List<string>();
            ListBookToPrioritiseLoadPath = new List<string>();
            ListMissionToPrioritiseLoadPath = new List<string>();
        }

        public void LoadOnlineData(ByteReader BR)
        {
            RemainingNumberOfBooksToLoad = BR.ReadInt32();
            RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfBooksToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableBook FillerItem;

                if (!DicBookDatabase.TryGetValue(ItemPath, out FillerItem))
                {
                    FillerItem = new UnlockableBook(ItemPath);
                    DicBookDatabase.Add(ItemPath, FillerItem);
                }

                RootBookContainer.ListUnlockedBook.Add(FillerItem);

                --RemainingNumberOfBooksToLoad;
            }
            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockablePlayerCharacter FillerItem;

                if (!DicCharacterDatabase.TryGetValue(ItemPath, out FillerItem))
                {
                    FillerItem = new UnlockablePlayerCharacter(ItemPath);
                    DicCharacterDatabase.Add(ItemPath, FillerItem);
                }

                RootCharacterContainer.ListUnlockedCharacter.Add(FillerItem);

                --RemainingNumberOfCharactersToLoad;
            }
            while (RemainingNumberOfMissionsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableMission FillerItem;

                if (!DicMissionDatabase.TryGetValue(ItemPath, out FillerItem))
                {
                    FillerItem = new UnlockableMission(ItemPath);
                    DicMissionDatabase.Add(ItemPath, FillerItem);
                }

                ListUnlockedMission.Add(FillerItem);
                --RemainingNumberOfMissionsToLoad;
            }
        }

        public void SaveLocally(string PlayerName)
        {
            FileStream FS = new FileStream("User data/Player Unlocks/Sorcerer Street/" + PlayerName + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(RootBookContainer.ListUnlockedBook.Count);
            BW.Write(RootCharacterContainer.ListUnlockedCharacter.Count);
            BW.Write(ListUnlockedMission.Count);

            foreach (UnlockableBook ActiveBook in RootBookContainer.ListUnlockedBook)
            {
                BW.Write(ActiveBook.Path);
            }

            foreach (UnlockablePlayerCharacter ActiveCharacter in RootCharacterContainer.ListUnlockedCharacter)
            {
                BW.Write(ActiveCharacter.Path);
            }

            foreach (UnlockableMission ActiveMission in ListUnlockedMission)
            {
                BW.Write(ActiveMission.Path);
            }

            BW.Close();
            FS.Close();
        }

        public static void PopulateShopItemsServerTask()
        {
            GlobalUnlockIniAsync = new IniFileReader("Content/Battle Lobby Unlocks.ini");

            while (GlobalUnlockIniAsync.CanRead)
            {
                PopulateUnlockItems();
            }
        }

        public void LoadShopBook(int StartIndex, int EndIndex, List<UnlockableBook> ListShopPlayerBook)
        {
            lock (ListBookToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListBookToPrioritiseLoadPath.Add(ListShopPlayerBook[ActiveIndex].Path);
                }
            }
        }

        public void LoadShopCharacter(int StartIndex, int EndIndex, List<UnlockablePlayerCharacter> ListShopPlayerCharacter)
        {
            lock (ListCharacterToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListCharacterToPrioritiseLoadPath.Add(ListShopPlayerCharacter[ActiveIndex].Path);
                }
            }
        }

        public void LoadPlayerUnlocks(string PlayerName)
        {
            if (!File.Exists("User data/Player Unlocks/Sorcerer Street/" + PlayerName + ".bin"))
            {
                SaveLocally(PlayerName);
            }

            //Load unlocks first, then the shop items.
            PlayerUnlocksFS = new FileStream("User data/Player Unlocks/Sorcerer Street/" + PlayerName + ".bin", FileMode.Open, FileAccess.Read);
            PlayerUnlocksBR = new BinaryReader(PlayerUnlocksFS, Encoding.UTF8);

            RemainingNumberOfBooksToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfCharactersToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfMissionsToLoad = PlayerUnlocksBR.ReadInt32();

            Task.Run(() => { LoadPlayerUnlocksAsyncTask(); });
        }

        private void LoadPlayerUnlocksAsyncTask()
        {
            while (!HasFinishedReadingPlayerShopItems)
            {
                LoadNextPlayerUnlock();
            }

            PlayerUnlocksBR.Close();
            PlayerUnlocksFS.Close();

            lock (LockObject)
            {
                if (LoadTask == null)
                {
                    LoadTask = Task.Run(() => { PopulatUnlocksAsyncTask(); });
                }
            }
        }

        private void LoadNextPlayerUnlock()
        {
            if (RemainingNumberOfBooksToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                UnlockableBook FillerItem = new UnlockableBook(ItemPath);

                DicBookDatabase.Add(ItemPath, FillerItem);
                RootBookContainer.ListUnlockedBook.Add(FillerItem);
                --RemainingNumberOfBooksToLoad;
            }
            else if (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                UnlockablePlayerCharacter FillerItem = new UnlockablePlayerCharacter(ItemPath);

                if (!DicCharacterDatabase.ContainsKey(ItemPath))
                {
                    DicCharacterDatabase.Add(ItemPath, FillerItem);
                    RootCharacterContainer.ListUnlockedCharacter.Add(FillerItem);
                }
                --RemainingNumberOfCharactersToLoad;
            }
            else if (RemainingNumberOfMissionsToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                UnlockableMission FillerItem = new UnlockableMission(ItemPath);

                DicMissionDatabase.Add(ItemPath, FillerItem);
                ListUnlockedMission.Add(FillerItem);
                --RemainingNumberOfMissionsToLoad;
            }
            else
            {
                HasFinishedReadingPlayerShopItems = true;
            }
        }

        private static void PopulatUnlocksAsyncTask()
        {
            GlobalUnlockIniAsync = new IniFileReader("Content/Sorcerer Street Lobby Unlocks.ini");

            while (GlobalUnlockIniAsync.CanRead)
            {
                PopulateUnlockItems();
            }

            DicBookDatabaseEnumerator = DicBookDatabase.GetEnumerator();
            DicCharacterDatabaseEnumeretor = DicCharacterDatabase.GetEnumerator();
            DicMissionDatabaseEnumerator = DicMissionDatabase.GetEnumerator();

            while (!DatabaseLoaded)
            {
                LoadUnlocksContentAsyncTask();
            }
        }

        private static void PopulateUnlockItems()
        {
            Dictionary<string, string> ActiveHeaderValues = GlobalUnlockIniAsync.ReadAllValues();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemType = ActiveHeaderValues["Type"];
                string ItemPath = ActiveHeaderValues["Path"];
                switch (ItemType)
                {
                    case "Book":
                        if (!DicBookDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableBook NewUnitUnlock = new UnlockableBook(ItemPath, ActiveHeaderValues);
                            DicBookDatabase.Add(ItemPath, NewUnitUnlock);
                        }
                        else
                        {
                            DicBookDatabase[ItemPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "Character":
                        if (!DicCharacterDatabase.ContainsKey(ItemPath))
                        {
                            UnlockablePlayerCharacter NewCharacterUnlock = new UnlockablePlayerCharacter(ItemPath, ActiveHeaderValues);
                            DicCharacterDatabase.Add(ItemPath, NewCharacterUnlock);
                        }
                        else
                        {
                            DicCharacterDatabase[ItemPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "Mission":
                        if (!DicMissionDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableMission NewMissionUnlock = new UnlockableMission(ItemPath, ActiveHeaderValues);
                            DicMissionDatabase.Add(ItemPath, NewMissionUnlock);
                        }
                        else
                        {
                            DicMissionDatabase[ItemPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "Consumable":
                        break;
                }
            }
        }

        private static void LoadUnlocksContentAsyncTask()
        {
            UnlockableBook BookToLoadInfo = null;
            UnlockablePlayerCharacter CharacterToLoadInfo = null;
            UnlockableMission MissionToLoadInfo = null;

            if (ListBookToPrioritiseLoadPath.Count > 0)
            {
                BookToLoadInfo = DicBookDatabase[ListBookToPrioritiseLoadPath[0]];

                ListBookToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicBookDatabaseEnumerator.MoveNext())
            {
                BookToLoadInfo = DicBookDatabaseEnumerator.Current.Value;
            }
            else if (ListCharacterToPrioritiseLoadPath.Count > 0)
            {
                CharacterToLoadInfo = DicCharacterDatabase[ListCharacterToPrioritiseLoadPath[0]];

                ListCharacterToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicCharacterDatabaseEnumeretor.MoveNext())
            {
                CharacterToLoadInfo = DicCharacterDatabaseEnumeretor.Current.Value;
            }
            else if (ListMissionToPrioritiseLoadPath.Count > 0)
            {
                MissionToLoadInfo = DicMissionDatabase[ListMissionToPrioritiseLoadPath[0]];

                ListMissionToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicMissionDatabaseEnumerator.MoveNext())
            {
                MissionToLoadInfo = DicMissionDatabaseEnumerator.Current.Value;
            }

            if (BookToLoadInfo != null)
            {
                CardBook NewBook = CardBook.GetCardBook(BookToLoadInfo.Path, ((Player)PlayerManager.ListLocalPlayer[0]).Inventory.GlobalBook, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                BookToLoadInfo.BookToBuy = NewBook;
            }
            else if (CharacterToLoadInfo != null)
            {
                Character NewCharacter = new Character(CharacterToLoadInfo.Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;

                CharacterToLoadInfo.CharacterToBuy = NewCharacter;
            }
            else if (MissionToLoadInfo != null)
            {
                MissionToLoadInfo.MissionToBuy = new MissionInfo(MissionToLoadInfo.Path, 0);
            }
            else
            {
                DatabaseLoaded = true;
            }
        }
    }
}
