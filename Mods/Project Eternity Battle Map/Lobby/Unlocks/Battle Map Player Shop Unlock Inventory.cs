﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class BattleMapPlayerShopUnlockInventory
    {
        public struct UnitUnlockContainer
        {
            public Dictionary<string, UnitUnlockContainer> DicFolder;
            public List<UnitUnlockContainer> ListFolder;//Share the same folders as the dictionnary
            public List<UnlockableUnit> ListUnlockedUnit;
            public List<UnlockableUnit> ListLockedUnit;
            public UnlockableUnit IconUnit { get { if (ListUnlockedUnit.Count > 0) return ListUnlockedUnit[0]; else return ListLockedUnit[0]; } }

            public string Name;

            public UnitUnlockContainer(IEnumerable<UnlockableUnit> ListLockedUnit)
            {
                this.ListLockedUnit = new List<UnlockableUnit>(ListLockedUnit);
                ListUnlockedUnit = new List<UnlockableUnit>();
                DicFolder = new Dictionary<string, UnitUnlockContainer>();
                ListFolder = new List<UnitUnlockContainer>();
                Name = string.Empty;
            }

            public UnitUnlockContainer(string Name)
            {
                this.Name = Name;

                DicFolder = new Dictionary<string, UnitUnlockContainer>();
                ListFolder = new List<UnitUnlockContainer>();
                ListUnlockedUnit = new List<UnlockableUnit>();
                ListLockedUnit = new List<UnlockableUnit>();
            }
        }

        //Used to keep track of what is already unlocked to not unlock it again
        public static Dictionary<string, UnlockableCharacter> DicCharacterDatabase = new Dictionary<string, UnlockableCharacter>();
        public static Dictionary<string, UnlockableUnit> DicUnitDatabase = new Dictionary<string, UnlockableUnit>();//UnitTypeAndPath
        public static Dictionary<string, UnlockableUnitSkin> DicUnitSkinDatabase = new Dictionary<string, UnlockableUnitSkin>();//UnitTypeAndPath + SkinTypeAndPath
        public static Dictionary<string, UnlockableUnitAlt> DicUnitAltDatabase = new Dictionary<string, UnlockableUnitAlt>();//UnitTypeAndPath + SkinTypeAndPath
        public static Dictionary<string, UnlockableMission> DicMissionDatabase = new Dictionary<string, UnlockableMission>();

        public static Dictionary<string, UnlockableCharacter>.Enumerator DicCharacterDatabaseEnumeretor;
        public static Dictionary<string, UnlockableUnit>.Enumerator DicUnitDatabaseEnumerator;
        public static Dictionary<string, UnlockableUnitSkin>.Enumerator DicUnitSkinDatabaseEnumerator;
        public static Dictionary<string, UnlockableUnitAlt>.Enumerator DicUnitAltDatabaseEnumerator;
        public static Dictionary<string, UnlockableMission>.Enumerator DicMissionDatabaseEnumerator;

        private static IniFileReader GlobalUnlockIniAsync;

        public static bool DatabaseLoaded;
        public static Task LoadTask;
        private static object LockObject = new object();

        private static List<string> ListCharacterToPrioritiseLoadPath;//Path of characters to load in background;
        private static List<string> ListUnitToPrioritiseLoadPath;//Path of units to load in background;
        private static List<string> ListUnitSkinToPrioritiseLoadPath;//Path of units to load in background;
        private static List<string> ListUnitAltToPrioritiseLoadPath;//Path of units to load in background;
        private static List<string> ListMissionToPrioritiseLoadPath;//Path of missions to load in background;

        //List of items to display in the shop
        public List<UnlockableCharacter> ListUnlockedCharacter;
        public UnitUnlockContainer RootUnitContainer;
        public List<UnlockableMission> ListUnlockedMission;

        public List<UnlockableCharacter> ListLockedCharacter;
        public List<UnlockableMission> ListLockedMission;

        private FileStream PlayerUnlocksFS;
        private BinaryReader PlayerUnlocksBR;
        private int RemainingNumberOfCharactersToLoad;
        private int RemainingNumberOfUnitsToLoad;
        private int RemainingNumberOfMissionsToLoad;
        public bool HasFinishedReadingPlayerShopItems;

        public BattleMapPlayerShopUnlockInventory()
        {
            ListUnlockedCharacter = new List<UnlockableCharacter>();
            RootUnitContainer = new UnitUnlockContainer("ALL");
            ListUnlockedMission = new List<UnlockableMission>();

            ListLockedCharacter = new List<UnlockableCharacter>();
            ListLockedMission = new List<UnlockableMission>();

            ListCharacterToPrioritiseLoadPath = new List<string>();
            ListUnitToPrioritiseLoadPath = new List<string>();
            ListUnitSkinToPrioritiseLoadPath = new List<string>();
            ListUnitAltToPrioritiseLoadPath = new List<string>();
            ListMissionToPrioritiseLoadPath = new List<string>();
        }

        public void LoadOnlineData(ByteReader BR)
        {
            RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            RemainingNumberOfUnitsToLoad = BR.ReadInt32();
            RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableCharacter FillerItem;

                if (!DicCharacterDatabase.TryGetValue(ItemPath, out FillerItem))
                {
                    FillerItem = new UnlockableCharacter(ItemPath);
                    DicCharacterDatabase.Add(ItemPath, FillerItem);
                }

                ListUnlockedCharacter.Add(FillerItem);

                --RemainingNumberOfCharactersToLoad;
            }
            while (RemainingNumberOfUnitsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableUnit FillerItem;

                if (!DicUnitDatabase.TryGetValue(ItemPath, out FillerItem))
                {
                    FillerItem = new UnlockableUnit(ItemPath);
                    DicUnitDatabase.Add(ItemPath, FillerItem);
                }

                RootUnitContainer.ListUnlockedUnit.Add(FillerItem);
                int ListUnlockedUnitSkinCount = BR.ReadByte();
                for (int S = ListUnlockedUnitSkinCount - 1; S >= 0; --S)
                {
                    FillerItem.ListUnlockedSkin.Add(new UnlockableUnitSkin(ItemPath, BR.ReadString()));
                }

                int ListUnlockedUnitAltCount = BR.ReadByte();
                for (int S = ListUnlockedUnitAltCount - 1; S >= 0; --S)
                {
                    FillerItem.ListUnlockedAlt.Add(new UnlockableUnitAlt(ItemPath, BR.ReadString()));
                }

                --RemainingNumberOfUnitsToLoad;
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
            FileStream FS = new FileStream("User data/Player Unlocks/Battle Map/" + PlayerName + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(ListUnlockedCharacter.Count);
            BW.Write(RootUnitContainer.ListUnlockedUnit.Count);
            BW.Write(ListUnlockedMission.Count);

            foreach (UnlockableCharacter ActiveCharacter in ListUnlockedCharacter)
            {
                BW.Write(ActiveCharacter.Path);
            }

            foreach (UnlockableUnit ActiveUnit in RootUnitContainer.ListUnlockedUnit)
            {
                BW.Write(ActiveUnit.Path);
                BW.Write((byte)ActiveUnit.ListUnlockedSkin.Count);

                foreach (UnlockableUnitSkin ActiveSkin in ActiveUnit.ListUnlockedSkin)
                {
                    BW.Write(ActiveSkin.SkinTypeAndPath);
                }

                BW.Write((byte)ActiveUnit.ListUnlockedAlt.Count);

                foreach (UnlockableUnitAlt ActiveAlt in ActiveUnit.ListUnlockedAlt)
                {
                    BW.Write(ActiveAlt.AltTypeAndPath);
                }
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
                PopulateUnlockDictionariesAsync();
            }
        }

        public void LoadShopCharacter(int StartIndex, int EndIndex, List<UnlockableCharacter> ListShopPlayerCharacter)
        {
            lock (ListCharacterToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListCharacterToPrioritiseLoadPath.Add(ListShopPlayerCharacter[ActiveIndex].Path);
                }
            }
        }

        public void LoadShopUnit(int StartIndex, int EndIndex, List<UnlockableUnit> ListShopPlayerUnit)
        {
            lock (ListUnitToPrioritiseLoadPath)
            {
                for (int ActiveIndex = StartIndex; ActiveIndex < EndIndex; --ActiveIndex)
                {
                    ListUnitToPrioritiseLoadPath.Add(ListShopPlayerUnit[ActiveIndex].Path);
                }
            }
        }

        public void LoadUnlockedPlayerItems(string PlayerName)
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

            Task.Run(() => { LoadUnlockedPlayerItemsAsyncTask(); });
        }

        private void LoadUnlockedPlayerItemsAsyncTask()
        {
            while (!HasFinishedReadingPlayerShopItems)
            {
                LoadNextUnlockedPlayerItems();
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

        private void LoadNextUnlockedPlayerItems()
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
                string UnitTypeAndPath = PlayerUnlocksBR.ReadString();

                if (!DicUnitDatabase.ContainsKey(UnitTypeAndPath))
                {
                    UnlockableUnit FillerItem = new UnlockableUnit(UnitTypeAndPath);
                    DicUnitDatabase.Add(UnitTypeAndPath, FillerItem);
                    RootUnitContainer.ListUnlockedUnit.Add(FillerItem);
                }

                int ListUnlockedUnitSkinCount = PlayerUnlocksBR.ReadByte();
                for (int S = ListUnlockedUnitSkinCount - 1; S >= 0; --S)
                {
                    string SkinTypeAndPath = PlayerUnlocksBR.ReadString();

                    UnlockableUnitSkin FillerItem = new UnlockableUnitSkin(UnitTypeAndPath, SkinTypeAndPath);
                    DicUnitSkinDatabase.Add(UnitTypeAndPath + SkinTypeAndPath, FillerItem);
                    DicUnitDatabase[UnitTypeAndPath].ListUnlockedSkin.Add(FillerItem);
                }

                int ListUnlockedUnitAltCount = PlayerUnlocksBR.ReadByte();
                for (int S = ListUnlockedUnitAltCount - 1; S >= 0; --S)
                {
                    string AltPath = PlayerUnlocksBR.ReadString();

                    UnlockableUnitAlt FillerItem = new UnlockableUnitAlt(UnitTypeAndPath, AltPath);
                    DicUnitAltDatabase.Add(UnitTypeAndPath + AltPath, FillerItem);
                    DicUnitDatabase[UnitTypeAndPath].ListUnlockedAlt.Add(FillerItem);
                }

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

        private static void PopulatUnlocksAsyncTask()
        {
            GlobalUnlockIniAsync = new IniFileReader("Content/Battle Lobby Unlocks.ini");

            while (GlobalUnlockIniAsync.CanRead)
            {
                PopulateUnlockDictionariesAsync();
            }

            DicCharacterDatabaseEnumeretor = DicCharacterDatabase.GetEnumerator();
            DicUnitDatabaseEnumerator = DicUnitDatabase.GetEnumerator();
            DicUnitSkinDatabaseEnumerator = DicUnitSkinDatabase.GetEnumerator();
            DicUnitAltDatabaseEnumerator = DicUnitAltDatabase.GetEnumerator();
            DicMissionDatabaseEnumerator = DicMissionDatabase.GetEnumerator();

            while (!DatabaseLoaded)
            {
                LoadUnlocksDictionariesContentAsyncTask();
            }
        }

        private static void PopulateUnlockDictionariesAsync()
        {
            Dictionary<string, string> ActiveHeaderValues = GlobalUnlockIniAsync.ReadAllValues();

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
                        else
                        {
                            DicCharacterDatabase[ItemPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "Unit":
                        if (!DicUnitDatabase.ContainsKey(ItemPath))
                        {
                            UnlockableUnit NewUnitUnlock = new UnlockableUnit(ItemPath, ActiveHeaderValues);
                            DicUnitDatabase.Add(ItemPath, NewUnitUnlock);
                        }
                        else
                        {
                            DicUnitDatabase[ItemPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "UnitSkin":
                        string FullSkinPath = ItemPath + ActiveHeaderValues["SkinPath"];
                        if (!DicUnitSkinDatabase.ContainsKey(FullSkinPath))
                        {
                            UnlockableUnitSkin NewUnitUnlock = new UnlockableUnitSkin(ItemPath, ActiveHeaderValues);
                            DicUnitSkinDatabase.Add(FullSkinPath, NewUnitUnlock);
                        }
                        else
                        {
                            DicUnitSkinDatabase[FullSkinPath].ReadHeaders(ActiveHeaderValues);
                        }
                        break;

                    case "UnitAlt":
                        string FullAltPath = ItemPath + ActiveHeaderValues["AltPath"];
                        if (!DicUnitAltDatabase.ContainsKey(FullAltPath))
                        {
                            UnlockableUnitAlt NewUnitUnlock = new UnlockableUnitAlt(ItemPath, ActiveHeaderValues);
                            DicUnitAltDatabase.Add(FullAltPath, NewUnitUnlock);
                        }
                        else
                        {
                            DicUnitAltDatabase[FullAltPath].ReadHeaders(ActiveHeaderValues);
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

        private static void LoadUnlocksDictionariesContentAsyncTask()
        {
            UnlockableCharacter CharacterToLoadInfo = null;
            UnlockableUnit UnitToLoadInfo = null;
            UnlockableUnitSkin UnitSkinToLoadInfo = null;
            UnlockableUnitAlt UnitAltToLoadInfo = null;
            UnlockableMission MissionToLoadInfo = null;

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
            else if (ListUnitSkinToPrioritiseLoadPath.Count > 0)
            {
                UnitSkinToLoadInfo = DicUnitSkinDatabase[ListUnitSkinToPrioritiseLoadPath[0]];

                ListUnitSkinToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicUnitSkinDatabaseEnumerator.MoveNext())
            {
                UnitSkinToLoadInfo = DicUnitSkinDatabaseEnumerator.Current.Value;
            }
            else if (ListUnitAltToPrioritiseLoadPath.Count > 0)
            {
                UnitAltToLoadInfo = DicUnitAltDatabase[ListUnitAltToPrioritiseLoadPath[0]];

                ListUnitAltToPrioritiseLoadPath.RemoveAt(0);
            }
            else if (DicUnitAltDatabaseEnumerator.MoveNext())
            {
                UnitAltToLoadInfo = DicUnitAltDatabaseEnumerator.Current.Value;
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
            else if (UnitSkinToLoadInfo != null)
            {
                Unit NewUnit = Unit.FromFullName(UnitSkinToLoadInfo.SkinTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                UnitSkinToLoadInfo.UnitSkinToBuy = NewUnit;
            }
            else if (UnitAltToLoadInfo != null)
            {
                Unit NewUnit = Unit.FromFullName(UnitAltToLoadInfo.AltTypeAndPath, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                UnitAltToLoadInfo.UnitAltToBuy = NewUnit;
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
