using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayerUnlockInventory
    {
        public static Dictionary<string, UnlockableCharacter> DicCharacterDatabase = new Dictionary<string, UnlockableCharacter>();
        public static Dictionary<string, UnlockableUnit> DicUnitDatabase = new Dictionary<string, UnlockableUnit>();
        public static Dictionary<string, UnlockableMission> DicMissionDatabase = new Dictionary<string, UnlockableMission>();

        private static CancellationTokenSource CancelToken = new CancellationTokenSource();
        private static Task UnlocksDatabaseLoadingTask;

        private static IniFileReader GlobalUnlockIniAsync;

        public static bool DatabaseLoaded;
        public static bool IsLoadingDatabase => !CancelToken.IsCancellationRequested;

        public List<UnlockableCharacter> ListUnlockedCharacter;
        public List<UnlockableUnit> ListUnlockedUnit;
        public List<UnlockableMission> ListUnlockedMission;

        public List<UnlockableCharacter> ListLockedCharacter;
        public List<UnlockableUnit> ListLockedUnit;
        public List<UnlockableMission> ListLockedMission;

        private FileStream PlayerUnlocksFS;
        private BinaryReader PlayerUnlocksBR;
        private int RemainingNumberOfCharactersToLoad;
        private int RemainingNumberOfUnitsToLoad;
        private int RemainingNumberOfMissionsToLoad;
        public bool HasFinishedReadingPlayerShopItems;

        public bool IsInit;

        public BattleMapPlayerUnlockInventory()
        {
            ListUnlockedCharacter = new List<UnlockableCharacter>();
            ListUnlockedUnit = new List<UnlockableUnit>();
            ListUnlockedMission = new List<UnlockableMission>();

            ListLockedCharacter = new List<UnlockableCharacter>();
            ListLockedUnit = new List<UnlockableUnit>();
            ListLockedMission = new List<UnlockableMission>();

            CancelToken = new CancellationTokenSource();
        }

        public void UpdateAvailableItems()
        {
            ListLockedCharacter = new List<UnlockableCharacter>(DicCharacterDatabase.Values);
            ListLockedUnit = new List<UnlockableUnit>(DicUnitDatabase.Values);
            ListLockedMission = new List<UnlockableMission>(DicMissionDatabase.Values);

            foreach (UnlockableCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                ListLockedCharacter.Remove(ActiveCharacter);
            }

            foreach (UnlockableUnit ActiveUnit in ListUnlockedUnit)
            {
                ListLockedUnit.Remove(ActiveUnit);
            }

            foreach (UnlockableMission ActiveMission in ListUnlockedMission)
            {
                ListLockedMission.Remove(ActiveMission);
            }

            IsInit = true;
        }

        public void PopulateUnlockedPlayerItems(string PlayerName)
        {
            if (!File.Exists("User data/Player Unlocks/Battle Map/" + PlayerName + ".bin"))
            {
                SaveLocally(PlayerName);
            }

            //Load unlocks first, then the shop items.
            PlayerUnlocksFS = new FileStream("User data/Player Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.Open, FileAccess.Read);
            PlayerUnlocksBR = new BinaryReader(PlayerUnlocksFS, Encoding.UTF8);

            RemainingNumberOfCharactersToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfUnitsToLoad = PlayerUnlocksBR.ReadInt32();
            RemainingNumberOfMissionsToLoad = PlayerUnlocksBR.ReadInt32();

            Task.Run(() => { PopulateUnlockedPlayerItemsAsyncTask(); });
        }

        private void PopulateUnlockedPlayerItemsAsyncTask()
        {
            while (!HasFinishedReadingPlayerShopItems)
            {
                PopulateNextUnlockedPlayerItem();
            }

            PlayerUnlocksBR.Close();
            PlayerUnlocksFS.Close();

            if (UnlocksDatabaseLoadingTask == null)
            {
                UnlocksDatabaseLoadingTask = Task.Run(() => { PopulateShopItemsAsyncTask(); });
            }
        }

        private void PopulateNextUnlockedPlayerItem()
        {
            if (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                UnlockableCharacter FillerItem = new UnlockableCharacter(ItemPath);

                DicCharacterDatabase.Add(ItemPath, FillerItem);
                ListUnlockedCharacter.Add(FillerItem);
                --RemainingNumberOfCharactersToLoad;
            }
            else if (RemainingNumberOfUnitsToLoad > 0)
            {
                string ItemPath = PlayerUnlocksBR.ReadString();
                UnlockableUnit FillerItem = new UnlockableUnit(ItemPath);

                DicUnitDatabase.Add(ItemPath, FillerItem);
                ListUnlockedUnit.Add(FillerItem);
                --RemainingNumberOfUnitsToLoad;
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

        private static void PopulateShopItemsAsyncTask()
        {
            GlobalUnlockIniAsync = new IniFileReader("Content/Battle Lobby Unlocks.ini");

            while (GlobalUnlockIniAsync.CanRead)
            {
                PopulateUnlockItems();
            }

            DatabaseLoaded = true;

            CancelToken.Cancel();
        }

        private static void PopulateUnlockItems()
        {
            Dictionary<string, string> ActiveHeaderValues = GlobalUnlockIniAsync.ReadAllHeaders();

            if (ActiveHeaderValues.Count > 0)
            {
                string ItemType = ActiveHeaderValues["Type"];
                string ItemPath = ActiveHeaderValues["Path"];
                switch (ItemType)
                {
                    case "Character":
                        if (!DicCharacterDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableCharacter NewCharacterUnlock = new UnlockableCharacter(ItemPath, ActiveHeaderValues);
                            DicCharacterDatabase.Add(ItemPath, NewCharacterUnlock);
                        }
                        break;

                    case "Unit":
                        if (!DicUnitDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableUnit NewUnitUnlock = new UnlockableUnit(ItemPath, ActiveHeaderValues);
                            DicUnitDatabase.Add(ItemPath, NewUnitUnlock);
                        }
                        break;

                    case "Mission":
                        if (!DicMissionDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableMission NewMissionUnlock = new UnlockableMission(ItemPath, ActiveHeaderValues);
                            DicMissionDatabase.Add(ItemPath, NewMissionUnlock);
                        }
                        break;

                    case "Consumable":
                        break;
                }
            }
        }

        public void SaveLocally(string PlayerName)
        {
            FileStream FS = new FileStream("User data/Player Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(ListUnlockedCharacter.Count);
            BW.Write(ListUnlockedUnit.Count);
            BW.Write(ListUnlockedMission.Count);

            foreach (UnlockableCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                BW.Write(ActiveCharacter.Path);
            }

            foreach (UnlockableUnit ActiveUnit in ListUnlockedUnit)
            {
                BW.Write(ActiveUnit.Path);
            }

            foreach (UnlockableMission ActiveMission in ListUnlockedMission)
            {
                BW.Write(ActiveMission.Path);
            }

            BW.Close();
            FS.Close();
        }
    }
}
