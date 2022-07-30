using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ShopItemUnit
    {
        public string Path;
        public int Price;
        public int RequiredLevel;
        public bool Hidden;

        public Unit UnitToBuy;

        public ShopItemUnit(string Path, int Price, int RequiredLevel, bool Hidden, Unit UnitToBuy)
        {
            this.Path = Path;
            this.Price = Price;
            this.RequiredLevel = RequiredLevel;
            this.Hidden = Hidden;
            this.UnitToBuy = UnitToBuy;
        }
    }

    public class ShopItemCharacter
    {
        public string Path;
        public int Price;
        public int RequiredLevel;
        public bool Hidden;

        public Character CharacterToBuy;

        public ShopItemCharacter(string Path, int Price, int RequiredLevel, bool Hidden, Character CharacterToBuy)
        {
            this.Path = Path;
            this.Price = Price;
            this.RequiredLevel = RequiredLevel;
            this.Hidden = Hidden;
            this.CharacterToBuy = CharacterToBuy;
        }
    }

    public class BattleMapPlayerShopInventory
    {
        public static Dictionary<string, ShopItemCharacter> DicCharacterDatabase = new Dictionary<string, ShopItemCharacter>();
        public static Dictionary<string, ShopItemUnit> DicUnitDatabase = new Dictionary<string, ShopItemUnit>();
        private static Dictionary<string, ShopItemCharacter>.Enumerator DicCharacterDatabaseEnumeretor;
        private static Dictionary<string, ShopItemUnit>.Enumerator DicUnitDatabaseEnumerator;

        private static CancellationTokenSource CancelToken = new CancellationTokenSource();
        private static Task ShopLoadingTask;

        private static IniFileReader GlobalShopCharactersIniAsync;
        private static IniFileReader GlobalShopUnitsIniAsync;

        private static List<string> ListCharacterToPrioritiseLoadPath;//Path of characters to load in background;
        private static List<string> ListUnitToPrioritiseLoadPath;//Path of units to load in background;

        private List<ShopItemCharacter> ListUnlockedCharacter = new List<ShopItemCharacter>();
        private List<ShopItemUnit> ListUnlockedUnit = new List<ShopItemUnit>();

        public HashSet<ShopItemCharacter> ListAvailableCharacterToBuy = new HashSet<ShopItemCharacter>();
        public HashSet<ShopItemUnit> ListAvailableUnitToBuy = new HashSet<ShopItemUnit>();
        public List<ShopItemCharacter> ListLockedCharacter = new List<ShopItemCharacter>();
        public List<ShopItemUnit> ListLockedUnit = new List<ShopItemUnit>();

        public bool IsLoading => !CancelToken.IsCancellationRequested;

        private FileStream PlayerUnlocksFS;
        private BinaryReader PlayerUnlocksBR;
        private uint RemainingNumberOfCharactersToLoad;
        private uint RemainingNumberOfUnitsToLoad;
        private bool HasFinishedReadingPlayerShopItems;

        public BattleMapPlayerShopInventory()
        {
            ListCharacterToPrioritiseLoadPath = new List<string>();
            ListUnitToPrioritiseLoadPath = new List<string>();

             CancelToken = new CancellationTokenSource();
        }

        public void UpdateAvailableItems(int PlayerLevel)
        {
            ListLockedCharacter = new List<ShopItemCharacter>(DicCharacterDatabase.Values);
            ListLockedUnit = new List<ShopItemUnit>(DicUnitDatabase.Values);

            foreach (ShopItemCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                ListLockedCharacter.Remove(ActiveCharacter);
            }

            foreach (ShopItemUnit ActiveUnit in ListUnlockedUnit)
            {
                ListLockedUnit.Remove(ActiveUnit);
            }

            foreach (ShopItemCharacter ActiveCharacter in BattleMapPlayerShopInventory.DicCharacterDatabase.Values)
            {
                if (ActiveCharacter.RequiredLevel <= PlayerLevel)
                {
                    ListAvailableCharacterToBuy.Add(ActiveCharacter);
                    ListLockedCharacter.Remove(ActiveCharacter);
                }
                else if (ActiveCharacter.Hidden)
                {
                    ListLockedCharacter.Remove(ActiveCharacter);
                }
            }

            foreach (ShopItemUnit ActiveUnit in BattleMapPlayerShopInventory.DicUnitDatabase.Values)
            {
                if (ActiveUnit.RequiredLevel <= PlayerLevel)
                {
                    ListAvailableUnitToBuy.Add(ActiveUnit);
                    ListLockedUnit.Remove(ActiveUnit);
                }
                else if (ActiveUnit.Hidden)
                {
                    ListLockedUnit.Remove(ActiveUnit);
                }
            }
        }

        public void PopulateUnlockedPlayerItems(string PlayerName)
        {
            PlayerUnlocksFS = new FileStream("User data/Shop Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.Open, FileAccess.Read);
            PlayerUnlocksBR = new BinaryReader(PlayerUnlocksFS, Encoding.UTF8);

            int RemainingNumberOfItemsToPreload = 10;
            RemainingNumberOfCharactersToLoad = /*PlayerUnlocksBR.ReadUInt32()*/0;
            while (!HasFinishedReadingPlayerShopItems && RemainingNumberOfItemsToPreload > 0)
            {
                PopulateNextUnlockedPlayerItem();
                --RemainingNumberOfItemsToPreload;
            }

            if (HasFinishedReadingPlayerShopItems)
            {
                PlayerUnlocksBR.Close();
                PlayerUnlocksFS.Close();

                if (ShopLoadingTask == null)
                {
                    ShopLoadingTask = Task.Run(() => { PopulateShopItemsAsyncTask(); });
                }
            }
            else
            {
                Task.Run(() => { PopulateUnlockedPlayerItemsAsyncTask(); });
            }
        }

        private void PopulateUnlockedPlayerItemsAsyncTask()
        {
            int RemainingTimeBeforeUpdate = 10;

            while (!HasFinishedReadingPlayerShopItems)
            {
                PopulateNextUnlockedPlayerItem();
                if (--RemainingTimeBeforeUpdate <= 0)
                {
                    RemainingTimeBeforeUpdate = 10;

                    if (RemainingNumberOfCharactersToLoad > 0)
                    {
                        lock (ListAvailableCharacterToBuy)
                        {
                            for (int C = ListAvailableCharacterToBuy.Count; C < ListUnlockedCharacter.Count; ++C)
                            {
                                ListAvailableCharacterToBuy.Add(ListUnlockedCharacter[C]);
                            }
                        }
                    }
                    else if (RemainingNumberOfUnitsToLoad > 0)
                    {
                        lock (ListAvailableUnitToBuy)
                        {
                            for (int U = ListAvailableUnitToBuy.Count; U < ListUnlockedUnit.Count; ++U)
                            {
                                ListAvailableUnitToBuy.Add(ListUnlockedUnit[U]);
                            }
                        }
                    }
                }
            }

            PlayerUnlocksBR.Close();
            PlayerUnlocksFS.Close();

            if (ShopLoadingTask == null)
            {
                ShopLoadingTask = Task.Run(() => { PopulateShopItemsAsyncTask(); });
            }
        }

        private void PopulateNextUnlockedPlayerItem()
        {
            if (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                ShopItemCharacter FillerItem = new ShopItemCharacter(ItemPath, -1, -1, false, null);

                DicCharacterDatabase.Add(ItemPath, FillerItem);
                ListUnlockedCharacter.Add(FillerItem);
                --RemainingNumberOfCharactersToLoad;

                if (RemainingNumberOfCharactersToLoad <= 0)
                {
                    lock (ListAvailableCharacterToBuy)
                    {
                        for (int C = ListAvailableCharacterToBuy.Count; C < ListUnlockedCharacter.Count; ++C)
                        {
                            ListAvailableCharacterToBuy.Add(ListUnlockedCharacter[C]);
                        }
                    }
                }
            }
            else if (RemainingNumberOfUnitsToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                ShopItemUnit FillerItem = new ShopItemUnit(ItemPath, -1, -1, false, null);

                DicUnitDatabase.Add(ItemPath, FillerItem);
                ListUnlockedUnit.Add(FillerItem);
                --RemainingNumberOfUnitsToLoad;

                if (RemainingNumberOfUnitsToLoad <= 0)
                {
                    lock (ListAvailableCharacterToBuy)
                    {
                        lock (ListAvailableUnitToBuy)
                        {
                            for (int U = ListAvailableUnitToBuy.Count; U < ListUnlockedUnit.Count; ++U)
                            {
                                ListAvailableUnitToBuy.Add(ListUnlockedUnit[U]);
                            }
                        }
                    }
                }
            }
            else
            {
                HasFinishedReadingPlayerShopItems = true;
            }
        }

        public void SaveLocally(string PlayerName)
        {
            FileStream FS = new FileStream("User data/Shop Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(ListAvailableCharacterToBuy.Count);

            foreach (ShopItemCharacter ActiveCharacter in ListAvailableCharacterToBuy)
            {
                BW.Write(ActiveCharacter.Path);
            }

            BW.Write(ListAvailableUnitToBuy.Count);

            foreach (ShopItemUnit ActiveUnit in ListAvailableUnitToBuy)
            {
                BW.Write(ActiveUnit.Path);
            }

            BW.Close();
            FS.Close();
        }

        public void LoadShopCharacter(int StartIndex, int EndIndex, List<ShopItemCharacter> ListShopPlayerCharacter)
        {
            lock (ListCharacterToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListCharacterToPrioritiseLoadPath.Add(ListShopPlayerCharacter[ActiveIndex].Path);
                }
            }
        }

        public void LoadShopUnit(int StartIndex, int EndIndex, List<ShopItemUnit> ListShopPlayerUnit)
        {
            lock (ListUnitToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListUnitToPrioritiseLoadPath.Add(ListShopPlayerUnit[ActiveIndex].Path);
                }
            }
        }

        private static void PopulateShopItemsAsyncTask()
        {
            GlobalShopCharactersIniAsync = new IniFileReader("Content/Battle Lobby Shop Characters.ini");
            GlobalShopUnitsIniAsync = new IniFileReader("Content/Battle Lobby Shop Units.ini");

            while (GlobalShopCharactersIniAsync.CanRead || GlobalShopUnitsIniAsync.CanRead)
            {
                PopulateShopItem();
            }

            LoadShopItemsAsyncTask();
        }

        private static void PopulateShopItem()
        {
            Dictionary<string, string> ActiveHeaderValues = GlobalShopCharactersIniAsync.ReadNextHeader();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemPath = ActiveHeaderValues["Path"];
                string Price = ActiveHeaderValues["Price"];
                string RequiredLevel = ActiveHeaderValues["RequiredLevel"];
                string Hidden = ActiveHeaderValues["Hidden"];

                ShopItemCharacter FillerCharacter;
                if (DicCharacterDatabase.TryGetValue(ItemPath, out FillerCharacter))
                {
                    FillerCharacter.Price = int.Parse(Price);
                    FillerCharacter.RequiredLevel = int.Parse(RequiredLevel);
                    FillerCharacter.Hidden = bool.Parse(Hidden);
                }
                else
                {
                    DicCharacterDatabase.Add(ItemPath, new ShopItemCharacter(ItemPath, int.Parse(Price), int.Parse(RequiredLevel), bool.Parse(Hidden), null));
                }
            }

            ActiveHeaderValues = GlobalShopUnitsIniAsync.ReadNextHeader();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemPath = ActiveHeaderValues["Path"];
                string Price = ActiveHeaderValues["Price"];
                string RequiredLevel = ActiveHeaderValues["RequiredLevel"];
                string Hidden = ActiveHeaderValues["Hidden"];

                ShopItemUnit FillerUnit;
                if (DicUnitDatabase.TryGetValue(ItemPath, out FillerUnit))
                {
                    FillerUnit.Price = int.Parse(Price);
                    FillerUnit.RequiredLevel = int.Parse(RequiredLevel);
                    FillerUnit.Hidden = bool.Parse(Hidden);
                }
                else
                {
                    DicUnitDatabase.Add(ItemPath, new ShopItemUnit(ItemPath, int.Parse(Price), int.Parse(RequiredLevel), bool.Parse(Hidden), null));
                }
            }
        }

        private static void LoadShopItemsAsyncTask()
        {
            DicCharacterDatabaseEnumeretor = DicCharacterDatabase.GetEnumerator();
            DicUnitDatabaseEnumerator = DicUnitDatabase.GetEnumerator();

            while (!CancelToken.IsCancellationRequested)
            {
                LoadNextShopItem();
            }
        }

        private static void LoadNextShopItem()
        {
            ShopItemCharacter CharacterToLoadInfo = null;
            ShopItemUnit UnitToLoadInfo = null;

            if (ListCharacterToPrioritiseLoadPath.Count > 0)
            {
                CharacterToLoadInfo = DicCharacterDatabase[ListCharacterToPrioritiseLoadPath[0]];

                ListCharacterToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicCharacterDatabaseEnumeretor.MoveNext())
            {
                CharacterToLoadInfo = DicCharacterDatabaseEnumeretor.Current.Value;
            }
            else if (ListUnitToPrioritiseLoadPath.Count > 0)
            {
                UnitToLoadInfo = DicUnitDatabase[ListUnitToPrioritiseLoadPath[0]];

                ListUnitToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicUnitDatabaseEnumerator.MoveNext())
            {
                UnitToLoadInfo = DicUnitDatabaseEnumerator.Current.Value;
            }

            if (CharacterToLoadInfo != null)
            {
                Character NewCharacter = new Character(CharacterToLoadInfo.Path, GameScreen.ContentFallback, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget, PlayerManager.DicManualSkillTarget);
                NewCharacter.Level = 1;

                CharacterToLoadInfo.CharacterToBuy = NewCharacter;
            }
            else if (UnitToLoadInfo != null)
            {
                Unit NewUnit = Unit.FromFullName(UnitToLoadInfo.Path, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);

                UnitToLoadInfo.UnitToBuy = NewUnit;
            }
            else
            {
                CancelToken.Cancel();
            }
        }
    }
}
