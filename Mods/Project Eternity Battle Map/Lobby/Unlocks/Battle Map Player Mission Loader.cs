using System.Collections.Generic;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public static class MissionLoader
        {
            public static readonly Dictionary<string, UnlockableMission> Database = new Dictionary<string, UnlockableMission>();

            private static readonly List<string> ListPrioritizedMission = new List<string>();
            private static readonly List<string> ListMissionToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedMission.Count > 0 || ListMissionToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                if (!Database.ContainsKey(ItemPath))
                {
                    Database.Add(ItemPath, new UnlockableMission(ItemPath, DicHeaderValues));
                }
                else
                {
                    Database[ItemPath].ReadHeaders(DicHeaderValues);
                }
            }

            public static void EnqueueAll()
            {
                foreach (string ItemPath in Database.Keys)
                {
                    Enqueue(ItemPath, false);
                }
            }

            public static UnlockableMission GetOrAdd(string ItemPath)
            {
                lock (LockObject)
                {
                    UnlockableMission Found;
                    if (!Database.TryGetValue(ItemPath, out Found))
                    {
                        Found = new UnlockableMission(ItemPath);
                        Database.Add(ItemPath, Found);
                        if (IsContentFileRead)
                        {
                            Enqueue(ItemPath, false);
                            AreAssetsAllLoaded = false;
                        }
                    }

                    return Found;
                }
            }

            public static void ClearPriority()
            {
                lock (LockObject)
                {
                    for (int MissionIndex = 0; MissionIndex < ListPrioritizedMission.Count; ++MissionIndex)
                    {
                        string Path = ListPrioritizedMission[MissionIndex];
                        UnlockableMission FoundMission;
                        if (Database.TryGetValue(Path, out FoundMission) && !string.IsNullOrEmpty(FoundMission.MissionToBuy.MapPath))
                        {
                            continue;
                        }

                        if (!ListMissionToLoad.Contains(Path))
                        {
                            ListMissionToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedMission.Clear();
                }
            }

            public static bool LoadNextPriority()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListPrioritizedMission.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedMission[0];
                    ListPrioritizedMission.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListMissionToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListMissionToLoad[0];
                    ListMissionToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableMission Found;
                if (Database.TryGetValue(Path, out Found) && !string.IsNullOrEmpty(Found.MissionToBuy.MapPath))
                {
                    return;
                }

                if (ListPrioritizedMission.Contains(Path))
                {
                    return;
                }

                if (ListMissionToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListMissionToLoad.Remove(Path);
                        ListPrioritizedMission.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedMission.Add(Path);
                }
                else
                {
                    ListMissionToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableMission MissionToLoad;
                lock (LockObject)
                {
                    MissionToLoad = Database[Path];
                }

                MissionToLoad.MissionToBuy = new MissionInfo(MissionToLoad.Path, 0);
            }
        }
    }
}
