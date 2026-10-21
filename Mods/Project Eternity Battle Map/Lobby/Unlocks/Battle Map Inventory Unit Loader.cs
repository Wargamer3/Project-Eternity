using System.Collections.Generic;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public partial class BattleMapPlayerShopUnlockInventory
    {
        public static class UnitLoader
        {
            public static readonly Dictionary<string, UnlockableUnit> Database = new Dictionary<string, UnlockableUnit>();

            private static readonly List<string> ListPrioritizedUnit = new List<string>();
            private static readonly List<string> ListUnitToLoad = new List<string>();

            public static bool HasPending
            {
                get { return ListPrioritizedUnit.Count > 0 || ListUnitToLoad.Count > 0; }
            }

            public static void ReadUnlockHeader(string ItemPath, Dictionary<string, string> DicHeaderValues)
            {
                if (!Database.ContainsKey(ItemPath))
                {
                    Database.Add(ItemPath, new UnlockableUnit(ItemPath, DicHeaderValues));
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

            public static UnlockableUnit GetOrAdd(string ItemPath)
            {
                lock (LockObject)
                {
                    UnlockableUnit Found;
                    if (!Database.TryGetValue(ItemPath, out Found))
                    {
                        Found = new UnlockableUnit(ItemPath);
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
                    for (int UnitIndex = 0; UnitIndex < ListPrioritizedUnit.Count; ++UnitIndex)
                    {
                        string Path = ListPrioritizedUnit[UnitIndex];
                        UnlockableUnit FoundUnit;
                        if (Database.TryGetValue(Path, out FoundUnit) && FoundUnit.UnitToBuy != null)
                        {
                            continue;
                        }

                        if (!ListUnitToLoad.Contains(Path))
                        {
                            ListUnitToLoad.Add(Path);
                        }
                    }

                    ListPrioritizedUnit.Clear();
                }
            }

            public static void Prioritize(string Path)
            {
                lock (LockObject)
                {
                    UnlockableUnit Found;
                    if (!Database.TryGetValue(Path, out Found))
                    {
                        return;
                    }

                    if (Found.UnitToBuy != null)
                    {
                        return;
                    }

                    Enqueue(Path, true);
                }
            }

            public static bool LoadNextPriority()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListPrioritizedUnit.Count == 0)
                    {
                        return false;
                    }

                    Path = ListPrioritizedUnit[0];
                    ListPrioritizedUnit.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            public static bool LoadNext()
            {
                string Path;
                lock (LockObject)
                {
                    if (ListUnitToLoad.Count == 0)
                    {
                        return false;
                    }

                    Path = ListUnitToLoad[0];
                    ListUnitToLoad.RemoveAt(0);
                }

                Load(Path);
                return true;
            }

            private static void Enqueue(string Path, bool Priority)
            {
                UnlockableUnit Found;
                if (Database.TryGetValue(Path, out Found) && Found.UnitToBuy != null)
                {
                    return;
                }

                if (ListPrioritizedUnit.Contains(Path))
                {
                    return;
                }

                if (ListUnitToLoad.Contains(Path))
                {
                    if (Priority)
                    {
                        ListUnitToLoad.Remove(Path);
                        ListPrioritizedUnit.Add(Path);
                    }

                    return;
                }

                if (Priority)
                {
                    ListPrioritizedUnit.Add(Path);
                }
                else
                {
                    ListUnitToLoad.Add(Path);
                }
            }

            private static void Load(string Path)
            {
                UnlockableUnit UnitToLoad;
                lock (LockObject)
                {
                    UnitToLoad = Database[Path];
                }

                Unit LoadedUnit = Unit.FromFullName(UnitToLoad.Path, GameScreen.ContentFallback, PlayerManager.DicUnitType, PlayerManager.DicRequirement, PlayerManager.DicEffect, PlayerManager.DicAutomaticSkillTarget);
                UnitToLoad.UnitToBuy = LoadedUnit;
            }
        }
    }
}
