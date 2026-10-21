using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public class UnitUnlockContainer
        {
            public Dictionary<string, UnitUnlockContainer> DicFolder;
            public List<UnitUnlockContainer> ListFolder;
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

            public UnitUnlockContainer GetOrAddFolder(string FolderName)
            {
                UnitUnlockContainer Folder;
                if (!DicFolder.TryGetValue(FolderName, out Folder))
                {
                    Folder = new UnitUnlockContainer(FolderName);
                    DicFolder.Add(FolderName, Folder);
                    ListFolder.Add(Folder);
                }

                return Folder;
            }

            public void AddToUnlocked(UnlockableUnit PlayerUnit)
            {
                if (!ListUnlockedUnit.Contains(PlayerUnit))
                {
                    ListUnlockedUnit.Add(PlayerUnit);
                }

                ListLockedUnit.Remove(PlayerUnit);
            }

            public void AddToLocked(UnlockableUnit PlayerUnit)
            {
                if (!ListLockedUnit.Contains(PlayerUnit))
                {
                    ListLockedUnit.Add(PlayerUnit);
                }

                ListUnlockedUnit.Remove(PlayerUnit);
            }
        }

        public static bool IsContentFileRead;
        public static bool AreAssetsAllLoaded;
        public static Task LoadTask;
        private static readonly object LockObject = new object();

        public const string UnlockFolder = "User data/Player Unlocks/Battle Map/";

        public List<UnlockableCharacter> ListUnlockedCharacter;
        public UnitUnlockContainer RootUnitContainer;
        public List<UnlockableMission> ListUnlockedMission;

        public List<UnlockableCharacter> ListLockedCharacter;
        public List<UnlockableMission> ListLockedMission;

        public bool HasFinishedReadingPlayerShopItems;

        public BattleMapPlayerShopUnlockInventory()
        {
            ListUnlockedCharacter = new List<UnlockableCharacter>();
            RootUnitContainer = new UnitUnlockContainer("ALL");
            ListUnlockedMission = new List<UnlockableMission>();

            ListLockedCharacter = new List<UnlockableCharacter>();
            ListLockedMission = new List<UnlockableMission>();
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

            if (CharacterLoader.LoadNextPriority())
            {
                return;
            }

            if (UnitLoader.LoadNextPriority())
            {
                return;
            }

            if (UnitSkinLoader.LoadNextPriority())
            {
                return;
            }

            if (UnitAltLoader.LoadNextPriority())
            {
                return;
            }

            if (MissionLoader.LoadNextPriority())
            {
                return;
            }

            if (CharacterLoader.LoadNext())
            {
                return;
            }

            if (UnitLoader.LoadNext())
            {
                return;
            }

            if (UnitSkinLoader.LoadNext())
            {
                return;
            }

            if (UnitAltLoader.LoadNext())
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
            CharacterLoader.ClearPriority();
            UnitLoader.ClearPriority();
            UnitSkinLoader.ClearPriority();
            UnitAltLoader.ClearPriority();
            MissionLoader.ClearPriority();
        }

        public static void PrioritizeCharacters(List<UnlockableCharacter> ListCharacter)
        {
            CharacterLoader.Prioritize(ListCharacter);
        }

        public static void PrioritizeUnits(List<UnlockableUnit> ListUnit)
        {
            for (int UnitIndex = 0; UnitIndex < ListUnit.Count; ++UnitIndex)
            {
                UnlockableUnit ActiveUnit = ListUnit[UnitIndex];
                UnitLoader.Prioritize(ActiveUnit.Path);
                UnitSkinLoader.Prioritize(ActiveUnit.ListUnlockedSkin);
                UnitSkinLoader.Prioritize(ActiveUnit.ListLockedSkin);
                UnitAltLoader.Prioritize(ActiveUnit.ListUnlockedAlt);
                UnitAltLoader.Prioritize(ActiveUnit.ListLockedAlt);
            }
        }

        public static void PrioritizeCharacter(string Path)
        {
            CharacterLoader.Prioritize(Path);
        }

        public static void PrioritizeUnit(string Path)
        {
            UnitLoader.Prioritize(Path);
        }

        public static void PrioritizeUnitSkin(string Path)
        {
            UnitSkinLoader.Prioritize(Path);
        }

        public static void PrioritizeUnitAlt(string Path)
        {
            UnitAltLoader.Prioritize(Path);
        }

        public void LoadUnlockedPlayerItems(string PlayerName)
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
            int RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            int RemainingNumberOfUnitsToLoad = BR.ReadInt32();
            int RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableCharacter FillerItem = GetOrAddCharacter(ItemPath);
                if (!ListUnlockedCharacter.Contains(FillerItem))
                {
                    ListUnlockedCharacter.Add(FillerItem);
                }

                --RemainingNumberOfCharactersToLoad;
            }

            while (RemainingNumberOfUnitsToLoad > 0)
            {
                string UnitTypeAndPath = BR.ReadString();
                UnlockableUnit PlayerUnit = FindPlayerUnit(UnitTypeAndPath);
                if (PlayerUnit == null)
                {
                    PlayerUnit = GetOrAddUnit(UnitTypeAndPath).Copy();
                    RootUnitContainer.ListUnlockedUnit.Add(PlayerUnit);
                }

                int ListUnlockedUnitSkinCount = BR.ReadByte();
                for (int S = ListUnlockedUnitSkinCount - 1; S >= 0; --S)
                {
                    string SkinTypeAndPath = BR.ReadString();
                    UnlockableUnitSkin FillerItem = GetOrAddSkin(UnitTypeAndPath, SkinTypeAndPath);
                    if (!PlayerUnit.ListUnlockedSkin.Contains(FillerItem))
                    {
                        PlayerUnit.ListUnlockedSkin.Add(FillerItem);
                    }
                }

                int ListUnlockedUnitAltCount = BR.ReadByte();
                for (int S = ListUnlockedUnitAltCount - 1; S >= 0; --S)
                {
                    string AltPath = BR.ReadString();
                    UnlockableUnitAlt FillerItem = GetOrAddAlt(UnitTypeAndPath, AltPath);
                    if (!PlayerUnit.ListUnlockedAlt.Contains(FillerItem))
                    {
                        PlayerUnit.ListUnlockedAlt.Add(FillerItem);
                    }
                }

                --RemainingNumberOfUnitsToLoad;
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
            int RemainingNumberOfCharactersToLoad = BR.ReadInt32();
            int RemainingNumberOfUnitsToLoad = BR.ReadInt32();
            int RemainingNumberOfMissionsToLoad = BR.ReadInt32();

            while (RemainingNumberOfCharactersToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableCharacter FillerItem = GetOrAddCharacter(ItemPath);
                if (!ListUnlockedCharacter.Contains(FillerItem))
                {
                    ListUnlockedCharacter.Add(FillerItem);
                }

                --RemainingNumberOfCharactersToLoad;
            }

            while (RemainingNumberOfUnitsToLoad > 0)
            {
                string ItemPath = BR.ReadString();
                UnlockableUnit PlayerUnit = FindPlayerUnit(ItemPath);
                if (PlayerUnit == null)
                {
                    PlayerUnit = GetOrAddUnit(ItemPath).Copy();
                    RootUnitContainer.ListUnlockedUnit.Add(PlayerUnit);
                }

                int ListUnlockedUnitSkinCount = BR.ReadByte();
                for (int S = ListUnlockedUnitSkinCount - 1; S >= 0; --S)
                {
                    UnlockableUnitSkin Skin = GetOrAddSkin(ItemPath, BR.ReadString());
                    if (!PlayerUnit.ListUnlockedSkin.Contains(Skin))
                    {
                        PlayerUnit.ListUnlockedSkin.Add(Skin);
                    }
                }

                int ListUnlockedUnitAltCount = BR.ReadByte();
                for (int S = ListUnlockedUnitAltCount - 1; S >= 0; --S)
                {
                    UnlockableUnitAlt Alt = GetOrAddAlt(ItemPath, BR.ReadString());
                    if (!PlayerUnit.ListUnlockedAlt.Contains(Alt))
                    {
                        PlayerUnit.ListUnlockedAlt.Add(Alt);
                    }
                }

                --RemainingNumberOfUnitsToLoad;
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
        }

        public UnlockableUnit FindPlayerUnit(string UnitPath)
        {
            foreach (UnlockableUnit ActiveUnit in RootUnitContainer.ListUnlockedUnit)
            {
                if (ActiveUnit.Path == UnitPath)
                {
                    return ActiveUnit;
                }
            }

            foreach (UnlockableUnit ActiveUnit in RootUnitContainer.ListLockedUnit)
            {
                if (ActiveUnit.Path == UnitPath)
                {
                    return ActiveUnit;
                }
            }

            return null;
        }

        public static UnlockableCharacter GetOrAddCharacter(string ItemPath)
        {
            return CharacterLoader.GetOrAdd(ItemPath);
        }

        public static UnlockableUnit GetOrAddUnit(string ItemPath)
        {
            return UnitLoader.GetOrAdd(ItemPath);
        }

        public static UnlockableUnitSkin GetOrAddSkin(string UnitPath, string SkinPath)
        {
            return UnitSkinLoader.GetOrAdd(UnitPath, SkinPath);
        }

        public static UnlockableUnitAlt GetOrAddAlt(string UnitPath, string AltPath)
        {
            return UnitAltLoader.GetOrAdd(UnitPath, AltPath);
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
            IniFileReader Reader = new IniFileReader("Content/Battle Lobby Unlocks.ini");
            List<Dictionary<string, string>> ListHeaderValues = new List<Dictionary<string, string>>();
            while (Reader.CanRead)
            {
                Dictionary<string, string> Header = Reader.ReadAllValues();
                if (Header.Count > 0)
                {
                    ListHeaderValues.Add(Header);
                }
            }

            lock (LockObject)
            {
                foreach (Dictionary<string, string> DicHeaderValues in ListHeaderValues)
                {
                    string ItemType = DicHeaderValues["Type"];
                    string ItemPath = DicHeaderValues["Path"];
                    switch (ItemType)
                    {
                        case "Character":
                            CharacterLoader.ReadUnlockHeader(ItemPath, DicHeaderValues);
                            break;

                        case "Unit":
                            UnitLoader.ReadUnlockHeader(ItemPath, DicHeaderValues);
                            break;

                        case "UnitSkin":
                            UnitSkinLoader.ReadUnlockHeader(ItemPath, DicHeaderValues);
                            break;

                        case "UnitAlt":
                            UnitAltLoader.ReadUnlockHeader(ItemPath, DicHeaderValues);
                            break;

                        case "Mission":
                            MissionLoader.ReadUnlockHeader(ItemPath, DicHeaderValues);
                            break;

                        case "Consumable":
                            break;
                    }
                }
            }
        }

        private static void EnqueueMissingContent()
        {
            CharacterLoader.EnqueueAll();
            UnitLoader.EnqueueAll();
            UnitSkinLoader.EnqueueAll();
            UnitAltLoader.EnqueueAll();
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
            return CharacterLoader.HasPending || UnitLoader.HasPending || UnitSkinLoader.HasPending || UnitAltLoader.HasPending || MissionLoader.HasPending;
        }
    }
}
