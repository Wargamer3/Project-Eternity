using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayerShopInventory
    {
        public static Dictionary<string, ShopItemCharacter> DicCharacterDatabase = new Dictionary<string, ShopItemCharacter>();
        public static Dictionary<string, ShopItemUnit> DicUnitDatabase = new Dictionary<string, ShopItemUnit>();
        public static Dictionary<string, ShopItemMission> DicMissionDatabase = new Dictionary<string, ShopItemMission>();
        private static Dictionary<string, ShopItemCharacter>.Enumerator DicCharacterDatabaseEnumeretor;
        private static Dictionary<string, ShopItemUnit>.Enumerator DicUnitDatabaseEnumerator;
        private static Dictionary<string, ShopItemMission>.Enumerator DicMissionDatabaseEnumerator;

        private static CancellationTokenSource CancelToken = new CancellationTokenSource();
        private static Task ShopLoadingTask;

        private static IniFileReader GlobalShopCharactersIniAsync;
        private static IniFileReader GlobalShopUnitsIniAsync;
        private static IniFileReader GlobalShopMissionsIniAsync;

        public static bool DatabaseLoaded;
        public static bool IsLoadingDatabase => !CancelToken.IsCancellationRequested;

        private static List<string> ListCharacterToPrioritiseLoadPath;//Path of characters to load in background;
        private static List<string> ListUnitToPrioritiseLoadPath;//Path of units to load in background;
        private static List<string> ListMissionToPrioritiseLoadPath;//Path of missions to load in background;

        public List<ShopItemCharacter> ListUnlockedCharacter;
        public List<ShopItemUnit> ListUnlockedUnit;
        public List<ShopItemMission> ListUnlockedMission;

        public List<ShopItemCharacter> ListLockedCharacter;
        public List<ShopItemUnit> ListLockedUnit;
        public List<ShopItemMission> ListLockedMission;

        public HashSet<ShopItemCharacter> ListAvailableCharacterToBuy = new HashSet<ShopItemCharacter>();
        public HashSet<ShopItemUnit> ListAvailableUnitToBuy = new HashSet<ShopItemUnit>();
        public HashSet<ShopItemMission> ListAvailableMissionToBuy = new HashSet<ShopItemMission>();

        private FileStream PlayerUnlocksFS;
        private BinaryReader PlayerUnlocksBR;
        private int RemainingNumberOfCharactersToLoad;
        private int RemainingNumberOfUnitsToLoad;
        private int RemainingNumberOfMissionsToLoad;
        public bool HasFinishedReadingPlayerShopItems;

        public bool IsInit;

        public BattleMapPlayerShopInventory()
        {
            ListCharacterToPrioritiseLoadPath = new List<string>();
            ListUnitToPrioritiseLoadPath = new List<string>();
            ListMissionToPrioritiseLoadPath = new List<string>();

            ListUnlockedCharacter = new List<ShopItemCharacter>();
            ListUnlockedUnit = new List<ShopItemUnit>();
            ListUnlockedMission = new List<ShopItemMission>();

            ListLockedCharacter = new List<ShopItemCharacter>();
            ListLockedUnit = new List<ShopItemUnit>();
            ListLockedMission = new List<ShopItemMission>();

            CancelToken = new CancellationTokenSource();
        }

        public void UpdateAvailableItems()
        {
            ListLockedCharacter = new List<ShopItemCharacter>(DicCharacterDatabase.Values);
            ListLockedUnit = new List<ShopItemUnit>(DicUnitDatabase.Values);
            ListLockedMission = new List<ShopItemMission>(DicMissionDatabase.Values);

            foreach (ShopItemCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                ListAvailableCharacterToBuy.Add(ActiveCharacter);
                ListLockedCharacter.Remove(ActiveCharacter);
            }

            foreach (ShopItemUnit ActiveUnit in ListUnlockedUnit)
            {
                ListAvailableUnitToBuy.Add(ActiveUnit);
                ListLockedUnit.Remove(ActiveUnit);
            }

            foreach (ShopItemMission ActiveMission in ListUnlockedMission)
            {
                ListAvailableMissionToBuy.Add(ActiveMission);
                ListLockedMission.Remove(ActiveMission);
            }

            for (int C = ListLockedCharacter.Count - 1; C >= 0; C--)
            {
                ShopItemCharacter ActiveCharacter = ListLockedCharacter[C];

                if (ActiveCharacter.HiddenUntilUnlocked)
                {
                    ListLockedCharacter.Remove(ActiveCharacter);
                }
            }

            for (int U = ListLockedUnit.Count - 1; U >= 0; U--)
            {
                ShopItemUnit ActiveUnit = ListLockedUnit[U];

                if (ActiveUnit.HiddenUntilUnlocked)
                {
                    ListLockedUnit.Remove(ActiveUnit);
                }
            }

            for (int M = ListLockedMission.Count - 1; M >= 0; M--)
            {
                ShopItemMission ActiveMission = ListLockedMission[M];

                if (ActiveMission.HiddenUntilUnlocked)
                {
                    ListLockedMission.Remove(ActiveMission);
                }
            }

            IsInit = true;
        }

        public void PopulateUnlockedShopItems(string PlayerName)
        {
            if (!File.Exists("User data/Shop Unlocks/Battle Map/" + PlayerName + ".bin"))
            {
                SaveLocally(PlayerName);
            }

            //Load unlocks first, then the shop items.
            PlayerUnlocksFS = new FileStream("User data/Shop Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.Open, FileAccess.Read);
            PlayerUnlocksBR = new BinaryReader(PlayerUnlocksFS, Encoding.UTF8);

            RemainingNumberOfCharactersToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfUnitsToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfMissionsToLoad = PlayerUnlocksBR.ReadInt32();

            Task.Run(() => { PopulateUnlockedShopItemsAsyncTask(); });
        }

        private void PopulateUnlockedShopItemsAsyncTask()
        {
            int RemainingTimeBeforeUpdate = 10;

            while (!HasFinishedReadingPlayerShopItems)
            {
                PopulateNextUnlockedShopItem();
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
                    else if (RemainingNumberOfMissionsToLoad > 0)
                    {
                        lock (ListAvailableMissionToBuy)
                        {
                            for (int M = ListAvailableMissionToBuy.Count; M < ListUnlockedMission.Count; ++M)
                            {
                                ListAvailableMissionToBuy.Add(ListUnlockedMission[M]);
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

        private void PopulateNextUnlockedShopItem()
        {
            if (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                ShopItemCharacter FillerItem = new ShopItemCharacter(ItemPath);

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
                ShopItemUnit FillerItem = new ShopItemUnit(ItemPath);

                DicUnitDatabase.Add(ItemPath, FillerItem);
                ListUnlockedUnit.Add(FillerItem);
                --RemainingNumberOfUnitsToLoad;

                if (RemainingNumberOfUnitsToLoad <= 0)
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
            else if (RemainingNumberOfMissionsToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                ShopItemMission FillerItem = new ShopItemMission(ItemPath);

                DicMissionDatabase.Add(ItemPath, FillerItem);
                ListUnlockedMission.Add(FillerItem);
                --RemainingNumberOfMissionsToLoad;

                if (RemainingNumberOfMissionsToLoad <= 0)
                {
                    lock (ListAvailableMissionToBuy)
                    {
                        for (int U = ListAvailableMissionToBuy.Count; U < ListUnlockedMission.Count; ++U)
                        {
                            ListAvailableMissionToBuy.Add(ListUnlockedMission[U]);
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

            BW.Write(ListUnlockedCharacter.Count);
            BW.Write(ListUnlockedUnit.Count);
            BW.Write(ListUnlockedMission.Count);

            foreach (ShopItemCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                BW.Write(ActiveCharacter.Path);
            }

            foreach (ShopItemUnit ActiveUnit in ListUnlockedUnit)
            {
                BW.Write(ActiveUnit.Path);
            }

            foreach (ShopItemMission ActiveMission in ListUnlockedMission)
            {
                BW.Write(ActiveMission.Path);
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
            GlobalShopMissionsIniAsync = new IniFileReader("Content/Battle Lobby Shop Missions.ini");

            while (GlobalShopCharactersIniAsync.CanRead || GlobalShopUnitsIniAsync.CanRead || GlobalShopMissionsIniAsync.CanRead)
            {
                PopulateShopItem();
            }

            DatabaseLoaded = true;
            LoadShopItemsContentAsyncTask();
        }

        private static void PopulateShopItem()
        {
            Dictionary<string, string> ActiveHeaderValues = GlobalShopCharactersIniAsync.ReadAllHeaders();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemPath = ActiveHeaderValues["Path"];

                ShopItemCharacter FillerCharacter;
                if (DicCharacterDatabase.TryGetValue(ItemPath, out FillerCharacter))
                {
                    FillerCharacter.Load(ActiveHeaderValues);
                }
                else
                {
                    DicCharacterDatabase.Add(ItemPath, new ShopItemCharacter(ItemPath, ActiveHeaderValues));
                }
            }

            ActiveHeaderValues = GlobalShopUnitsIniAsync.ReadAllHeaders();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemPath = ActiveHeaderValues["Path"];

                ShopItemUnit FillerUnit;
                if (DicUnitDatabase.TryGetValue(ItemPath, out FillerUnit))
                {
                    FillerUnit.Load(ActiveHeaderValues);
                }
                else
                {
                    DicUnitDatabase.Add(ItemPath, new ShopItemUnit(ItemPath, ActiveHeaderValues));
                }
            }

            ActiveHeaderValues = GlobalShopMissionsIniAsync.ReadAllHeaders();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemPath = ActiveHeaderValues["Path"];

                ShopItemMission FillerMission;
                if (DicMissionDatabase.TryGetValue(ItemPath, out FillerMission))
                {
                    FillerMission.Load(ActiveHeaderValues);
                }
                else
                {
                    DicMissionDatabase.Add(ItemPath, new ShopItemMission(ItemPath, ActiveHeaderValues));
                }
            }
        }

        private static void LoadShopItemsContentAsyncTask()
        {
            DicCharacterDatabaseEnumeretor = DicCharacterDatabase.GetEnumerator();
            DicUnitDatabaseEnumerator = DicUnitDatabase.GetEnumerator();
            DicMissionDatabaseEnumerator = DicMissionDatabase.GetEnumerator();

            while (!CancelToken.IsCancellationRequested)
            {
                LoadNextShopItemContent();
            }
        }

        private static void LoadNextShopItemContent()
        {
            ShopItemCharacter CharacterToLoadInfo = null;
            ShopItemUnit UnitToLoadInfo = null;
            ShopItemMission MissionToLoadInfo = null;

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
            else if (ListMissionToPrioritiseLoadPath.Count > 0)
            {
                MissionToLoadInfo = DicMissionDatabase[ListMissionToPrioritiseLoadPath[0]];

                ListMissionToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicMissionDatabaseEnumerator.MoveNext())
            {
                MissionToLoadInfo = DicMissionDatabaseEnumerator.Current.Value;
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
            else if (MissionToLoadInfo != null)
            {
                MissionToLoadInfo.MissionToBuy = new MissionInfo(MissionToLoadInfo.Path, -1);
            }
            else
            {
                CancelToken.Cancel();
            }
        }
    }
}
