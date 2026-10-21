using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public partial class SorcererStreetPlayerUnlockInventory
    {
        public class BookUnlockContainer
        {
            public Dictionary<string, BookUnlockContainer> DicFolder;
            public List<BookUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockableBook> ListUnlockedBook;
            public List<UnlockableBook> ListLockedBook;
            public UnlockableBook IconUnit
            {
                get
                {
                    if (ListUnlockedBook.Count > 0)
                    {
                        return ListUnlockedBook[0];
                    }

                    return ListLockedBook[0];
                }
            }

            public string Name;

            public BookUnlockContainer(IEnumerable<UnlockableBook> ListLockedBook)
            {
                this.ListLockedBook = new List<UnlockableBook>(ListLockedBook);
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

            public BookUnlockContainer GetOrAddFolder(string FolderName)
            {
                BookUnlockContainer Folder;
                if (!DicFolder.TryGetValue(FolderName, out Folder))
                {
                    Folder = new BookUnlockContainer(FolderName);
                    DicFolder.Add(FolderName, Folder);
                    ListFolder.Add(Folder);
                }

                return Folder;
            }

            public void AddToUnlocked(UnlockableBook PlayerBook)
            {
                if (!ListUnlockedBook.Contains(PlayerBook))
                {
                    ListUnlockedBook.Add(PlayerBook);
                }

                ListLockedBook.Remove(PlayerBook);
            }

            public void AddToLocked(UnlockableBook PlayerBook)
            {
                if (!ListLockedBook.Contains(PlayerBook))
                {
                    ListLockedBook.Add(PlayerBook);
                }

                ListUnlockedBook.Remove(PlayerBook);
            }
        }

        public class CardUnlockContainer
        {
            public Dictionary<string, CardUnlockContainer> DicFolder;
            public List<CardUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockableCard> ListUnlockedCard;
            public List<UnlockableCard> ListLockedCard;
            public UnlockableCard IconUnit
            {
                get
                {
                    if (ListUnlockedCard.Count > 0)
                    {
                        return ListUnlockedCard[0];
                    }

                    return ListLockedCard[0];
                }
            }

            public string Name;

            public CardUnlockContainer(IEnumerable<UnlockableCard> ListLockedCard)
            {
                this.ListLockedCard = new List<UnlockableCard>(ListLockedCard);
                ListUnlockedCard = new List<UnlockableCard>();
                DicFolder = new Dictionary<string, CardUnlockContainer>();
                ListFolder = new List<CardUnlockContainer>();
                Name = string.Empty;
            }

            public CardUnlockContainer(string Name)
            {
                this.Name = Name;

                DicFolder = new Dictionary<string, CardUnlockContainer>();
                ListFolder = new List<CardUnlockContainer>();
                ListUnlockedCard = new List<UnlockableCard>();
                ListLockedCard = new List<UnlockableCard>();
            }

            public CardUnlockContainer GetOrAddFolder(string FolderName)
            {
                CardUnlockContainer Folder;
                if (!DicFolder.TryGetValue(FolderName, out Folder))
                {
                    Folder = new CardUnlockContainer(FolderName);
                    DicFolder.Add(FolderName, Folder);
                    ListFolder.Add(Folder);
                }

                return Folder;
            }

            public void AddToUnlocked(UnlockableCard PlayerCard)
            {
                if (!ListUnlockedCard.Contains(PlayerCard))
                {
                    ListUnlockedCard.Add(PlayerCard);
                }

                ListLockedCard.Remove(PlayerCard);
            }

            public void AddToLocked(UnlockableCard PlayerCard)
            {
                if (!ListLockedCard.Contains(PlayerCard))
                {
                    ListLockedCard.Add(PlayerCard);
                }

                ListUnlockedCard.Remove(PlayerCard);
            }
        }

        public class CharacterUnlockContainer
        {
            public Dictionary<string, CharacterUnlockContainer> DicFolder;
            public List<CharacterUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockablePlayerCharacter> ListUnlockedCharacter;
            public List<UnlockablePlayerCharacter> ListLockedCharacter;
            public UnlockablePlayerCharacter IconUnit
            {
                get
                {
                    if (ListUnlockedCharacter.Count > 0)
                    {
                        return ListUnlockedCharacter[0];
                    }

                    return ListLockedCharacter[0];
                }
            }

            public string Name;

            public CharacterUnlockContainer(IEnumerable<UnlockablePlayerCharacter> ListLockedCharacter)
            {
                this.ListLockedCharacter = new List<UnlockablePlayerCharacter>(ListLockedCharacter);
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

            public CharacterUnlockContainer GetOrAddFolder(string FolderName)
            {
                CharacterUnlockContainer Folder;
                if (!DicFolder.TryGetValue(FolderName, out Folder))
                {
                    Folder = new CharacterUnlockContainer(FolderName);
                    DicFolder.Add(FolderName, Folder);
                    ListFolder.Add(Folder);
                }

                return Folder;
            }

            public void AddToUnlocked(UnlockablePlayerCharacter PlayerCharacter)
            {
                if (!ListUnlockedCharacter.Contains(PlayerCharacter))
                {
                    ListUnlockedCharacter.Add(PlayerCharacter);
                }

                ListLockedCharacter.Remove(PlayerCharacter);
            }

            public void AddToLocked(UnlockablePlayerCharacter PlayerCharacter)
            {
                if (!ListLockedCharacter.Contains(PlayerCharacter))
                {
                    ListLockedCharacter.Add(PlayerCharacter);
                }

                ListUnlockedCharacter.Remove(PlayerCharacter);
            }
        }

        public Dictionary<string, CardBookSkinInfo> DicOwnedBookSkin;//Skins for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, CardBookSkinInfo> DicOwnedBookAlt;//Alts for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, UnlockableCardSkin> DicOwnedCardSkin;//Skins for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, UnlockableCardSkin> DicOwnedCardAlt;//Alts for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, UnlockableCharacterSkin> DicOwnedCharacterSkin;//Skins for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath
        public Dictionary<string, UnlockableCharacterSkin> DicOwnedCharacterAlt;//Alts for Unit the player doesn't have yet. UnitTypeAndPath + SkinTypeAndPath

        public static CardBook GlobalBook;
        public static bool IsContentFileRead;
        public static bool AreAssetsAllLoaded;
        public static Task LoadTask;
        private static readonly object LockObject = new object();

        public const string UnlockFolder = "User data/Player Unlocks/Sorcerer Street/";

        public BookUnlockContainer RootBookContainer;
        public CardUnlockContainer RootCardContainer;
        public CharacterUnlockContainer RootCharacterContainer;

        public List<UnlockableMission> ListUnlockedMission;
        public List<UnlockableMission> ListLockedMission;

        public bool HasFinishedReadingPlayerShopItems;

        public SorcererStreetPlayerUnlockInventory()
        {
            RootBookContainer = new BookUnlockContainer("ALL");
            RootCardContainer = new CardUnlockContainer("ALL");
            RootCharacterContainer = new CharacterUnlockContainer("ALL");

            ListUnlockedMission = new List<UnlockableMission>();
            ListLockedMission = new List<UnlockableMission>();

            DicOwnedBookSkin = new Dictionary<string, CardBookSkinInfo>();
            DicOwnedBookAlt = new Dictionary<string, CardBookSkinInfo>();
            DicOwnedCardSkin = new Dictionary<string, UnlockableCardSkin>();
            DicOwnedCardAlt = new Dictionary<string, UnlockableCardSkin>();
            DicOwnedCharacterSkin = new Dictionary<string, UnlockableCharacterSkin>();
            DicOwnedCharacterAlt = new Dictionary<string, UnlockableCharacterSkin>();
        }

        public static void EnsureUnlockFileReadingStarted()
        {
            lock (LockObject)
            {
                if (LoadTask != null)
                {
                    return;
                }

                LoadTask = Task.Run(() => { ReadUnlockFileTask(); });
            }
        }

        public static void PopulateShopItemsServerTask()
        {
            ReadUnlockFile();
            lock (LockObject)
            {
                IsContentFileRead = true;
            }
        }

        public static void LoadInventoryAssetsThisFrame()
        {
            if (!IsContentFileRead || AreAssetsAllLoaded)
            {
                return;
            }

            if (BookLoader.LoadNextPriority())
            {
                return;
            }

            if (CardLoader.LoadNextPriority())
            {
                return;
            }

            if (CharacterLoader.LoadNextPriority())
            {
                return;
            }

            if (MissionLoader.LoadNextPriority())
            {
                return;
            }

            if (BookLoader.LoadNext())
            {
                return;
            }

            if (CardLoader.LoadNext())
            {
                return;
            }

            if (CharacterLoader.LoadNext())
            {
                return;
            }

            if (MissionLoader.LoadNext())
            {
                return;
            }

            ConfirmInventoryIsLoaded();
            return;
        }

        public static void ClearPriority()
        {
            BookLoader.ClearPriority();
            CardLoader.ClearPriority();
            CharacterLoader.ClearPriority();
            MissionLoader.ClearPriority();
        }

        public static void PrioritizeBooks(List<UnlockableBook> ListBook)
        {
            BookLoader.Prioritize(ListBook);
        }

        public static void PrioritizeCards(List<UnlockableCard> ListCard)
        {
            CardLoader.Prioritize(ListCard);
        }

        public static void PrioritizeCharacters(List<UnlockablePlayerCharacter> ListCharacter)
        {
            CharacterLoader.Prioritize(ListCharacter);
        }

        public static void PrioritizeBook(string Path)
        {
            BookLoader.Prioritize(Path);
        }

        public static void PrioritizeCard(string Path)
        {
            CardLoader.Prioritize(Path);
        }

        public static void PrioritizeCharacter(string Path)
        {
            CharacterLoader.Prioritize(Path);
        }

        public void LoadPlayerUnlocks(string PlayerName)
        {
            Directory.CreateDirectory(UnlockFolder);
            string FilePath = UnlockFolder + PlayerName + ".bin";
            if (!File.Exists(FilePath))
            {
                SaveLocally(PlayerName);
            }

            FileStream PlayerUnlocksFS = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader PlayerUnlocksBR = new BinaryReader(PlayerUnlocksFS, Encoding.UTF8);
            ReadUnlocks(PlayerUnlocksBR);
            PlayerUnlocksBR.Close();
            PlayerUnlocksFS.Close();
            HasFinishedReadingPlayerShopItems = true;
        }

        public void ReadUnlocks(BinaryReader BR)
        {
            int RemainingNumberOfBooksToLoad = BR.ReadInt32();
            int RemainingNumberOfCardsToLoad = BR.ReadInt32();
            int RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            int RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfBooksToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableBook PlayerBook = FindPlayerBook(ItemPath);
                if (PlayerBook == null)
                {
                    PlayerBook = GetOrAddBook(ItemPath).Copy();
                    RootBookContainer.ListUnlockedBook.Add(PlayerBook);
                }

                --RemainingNumberOfBooksToLoad;
            }

            while (RemainingNumberOfCardsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableCard PlayerCard = FindPlayerCard(ItemPath);
                if (PlayerCard == null)
                {
                    PlayerCard = GetOrAddCard(ItemPath).Copy();
                    RootCardContainer.ListUnlockedCard.Add(PlayerCard);
                }

                --RemainingNumberOfCardsToLoad;
            }

            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockablePlayerCharacter PlayerCharacter = FindPlayerCharacter(ItemPath);
                if (PlayerCharacter == null)
                {
                    PlayerCharacter = GetOrAddCharacter(ItemPath).Copy();
                    RootCharacterContainer.ListUnlockedCharacter.Add(PlayerCharacter);
                }

                --RemainingNumberOfCharactersToLoad;
            }

            while (RemainingNumberOfMissionsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableMission FillerItem = GetOrAddMission(ItemPath);
                if (!ListUnlockedMission.Contains(FillerItem))
                {
                    ListUnlockedMission.Add(FillerItem);
                }

                --RemainingNumberOfMissionsToLoad;
            }
        }

        public void LoadOnlineData(ByteReader BR)
        {
            int RemainingNumberOfBooksToLoad = BR.ReadInt32();
            int RemainingNumberOfCardsToLoad = BR.ReadInt32();
            int RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            int RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfBooksToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableBook PlayerBook = FindPlayerBook(ItemPath);
                if (PlayerBook == null)
                {
                    PlayerBook = GetOrAddBook(ItemPath).Copy();
                    RootBookContainer.ListUnlockedBook.Add(PlayerBook);
                }

                --RemainingNumberOfBooksToLoad;
            }

            while (RemainingNumberOfCardsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableCard PlayerCard = FindPlayerCard(ItemPath);
                if (PlayerCard == null)
                {
                    PlayerCard = GetOrAddCard(ItemPath).Copy();
                    RootCardContainer.ListUnlockedCard.Add(PlayerCard);
                }

                --RemainingNumberOfCardsToLoad;
            }

            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockablePlayerCharacter PlayerCharacter = FindPlayerCharacter(ItemPath);
                if (PlayerCharacter == null)
                {
                    PlayerCharacter = GetOrAddCharacter(ItemPath).Copy();
                    RootCharacterContainer.ListUnlockedCharacter.Add(PlayerCharacter);
                }

                --RemainingNumberOfCharactersToLoad;
            }

            while (RemainingNumberOfMissionsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableMission FillerItem = GetOrAddMission(ItemPath);
                if (!ListUnlockedMission.Contains(FillerItem))
                {
                    ListUnlockedMission.Add(FillerItem);
                }

                --RemainingNumberOfMissionsToLoad;
            }

            HasFinishedReadingPlayerShopItems = true;
        }

        public void SaveLocally(string PlayerName)
        {
            Directory.CreateDirectory(UnlockFolder);
            FileStream FS = new FileStream(UnlockFolder + PlayerName + ".bin", FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);
            WriteUnlocks(BW);
            BW.Close();
            FS.Close();
        }

        public void WriteUnlocks(BinaryWriter BW)
        {
            BW.Write(RootBookContainer.ListUnlockedBook.Count);
            BW.Write(RootCardContainer.ListUnlockedCard.Count);
            BW.Write(RootCharacterContainer.ListUnlockedCharacter.Count);
            BW.Write(ListUnlockedMission.Count);

            foreach (UnlockableBook ActiveBook in RootBookContainer.ListUnlockedBook)
            {
                BW.Write(ActiveBook.Path);
            }

            foreach (UnlockableCard ActiveCard in RootCardContainer.ListUnlockedCard)
            {
                BW.Write(ActiveCard.Path);
            }

            foreach (UnlockablePlayerCharacter ActiveCharacter in RootCharacterContainer.ListUnlockedCharacter)
            {
                BW.Write(ActiveCharacter.Path);
            }

            foreach (UnlockableMission ActiveMission in ListUnlockedMission)
            {
                BW.Write(ActiveMission.Path);
            }
        }

        public UnlockableBook FindPlayerBook(string BookPath)
        {
            foreach (UnlockableBook ActiveBook in RootBookContainer.ListUnlockedBook)
            {
                if (ActiveBook.Path == BookPath)
                {
                    return ActiveBook;
                }
            }

            foreach (UnlockableBook ActiveBook in RootBookContainer.ListLockedBook)
            {
                if (ActiveBook.Path == BookPath)
                {
                    return ActiveBook;
                }
            }

            return null;
        }

        public UnlockableCard FindPlayerCard(string CardPath)
        {
            foreach (UnlockableCard ActiveCard in RootCardContainer.ListUnlockedCard)
            {
                if (ActiveCard.Path == CardPath)
                {
                    return ActiveCard;
                }
            }

            foreach (UnlockableCard ActiveCard in RootCardContainer.ListLockedCard)
            {
                if (ActiveCard.Path == CardPath)
                {
                    return ActiveCard;
                }
            }

            return null;
        }

        public UnlockablePlayerCharacter FindPlayerCharacter(string CharacterPath)
        {
            foreach (UnlockablePlayerCharacter ActiveCharacter in RootCharacterContainer.ListUnlockedCharacter)
            {
                if (ActiveCharacter.Path == CharacterPath)
                {
                    return ActiveCharacter;
                }
            }

            foreach (UnlockablePlayerCharacter ActiveCharacter in RootCharacterContainer.ListLockedCharacter)
            {
                if (ActiveCharacter.Path == CharacterPath)
                {
                    return ActiveCharacter;
                }
            }

            return null;
        }

        public static UnlockableBook GetOrAddBook(string ItemPath)
        {
            return BookLoader.GetOrAdd(ItemPath);
        }

        public static UnlockableCard GetOrAddCard(string ItemPath)
        {
            return CardLoader.GetOrAdd(ItemPath);
        }

        public static UnlockablePlayerCharacter GetOrAddCharacter(string ItemPath)
        {
            return CharacterLoader.GetOrAdd(ItemPath);
        }

        public static UnlockableMission GetOrAddMission(string ItemPath)
        {
            return MissionLoader.GetOrAdd(ItemPath);
        }

        private static void ReadUnlockFileTask()
        {
            ReadUnlockFile();
            lock (LockObject)
            {
                EnqueueMissingContent();
                IsContentFileRead = true;
            }
        }

        private static void ReadUnlockFile()
        {
            IniFileReader Reader = new IniFileReader("Content/Sorcerer Street Lobby Unlocks.ini");
            List<Dictionary<string, string>> Headers = new List<Dictionary<string, string>>();
            while (Reader.CanRead)
            {
                Dictionary<string, string> Header = Reader.ReadAllValues();
                if (Header.Count > 0)
                {
                    Headers.Add(Header);
                }
            }

            lock (LockObject)
            {
                foreach (Dictionary<string, string> Header in Headers)
                {
                    ApplyHeader(Header);
                }
            }
        }

        private static void ApplyHeader(Dictionary<string, string> ActiveHeaderValues)
        {
            string ItemType = ActiveHeaderValues["Type"];
            string ItemPath = ActiveHeaderValues["Path"];
            switch (ItemType)
            {
                case "Book":
                    BookLoader.ReadUnlockHeader(ItemPath, ActiveHeaderValues);
                    break;

                case "Card":
                    CardLoader.ReadUnlockHeader(ItemPath, ActiveHeaderValues);
                    break;

                case "Character":
                    CharacterLoader.ReadUnlockHeader(ItemPath, ActiveHeaderValues);
                    break;

                case "Mission":
                    MissionLoader.ReadUnlockHeader(ItemPath, ActiveHeaderValues);
                    break;

                case "Consumable":
                    break;
            }
        }

        private static void EnqueueMissingContent()
        {
            BookLoader.EnqueueAll();
            CardLoader.EnqueueAll();
            CharacterLoader.EnqueueAll();
            MissionLoader.EnqueueAll();
        }

        private static void ConfirmInventoryIsLoaded()
        {
            lock (LockObject)
            {
                if (!IsContentFileRead || HasUnloadedAssets())
                {
                    return;
                }

                AreAssetsAllLoaded = true;
            }
        }

        private static bool HasUnloadedAssets()
        {
            return BookLoader.HasPending || CardLoader.HasPending || CharacterLoader.HasPending || MissionLoader.HasPending;
        }
    }
}